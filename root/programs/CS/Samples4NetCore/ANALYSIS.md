# ANALYSIS.md — Open棟梁 サンプル アプリ（CS/Samples4NetCore）コード分析

対象: `root/programs/CS/Samples4NetCore`（**.NET 10.0**） / ブランチ: `develop`
最終更新: 2026-07-31

本書は **コーディング・エージェントが本ディレクトリで作業する際の Context** を目的とする。
フレームワーク本体は `../Frameworks/ANALYSIS.md`、net48 版サンプルは `../Samples/ANALYSIS.md` を参照。

---

## 1. これは何か

Open棟梁の **.NET (Core) 系サンプル**。3 つの区画に分かれており、性格がまったく違う。

| 区画 | 中身 | 性格 |
|---|---|---|
| **`Backend/`** | `MVC_Sample`（ASP.NET Core MVC） | **現行の推奨サンプル。** ここが本命 |
| **`Frontend/`** | README のみ | 別リポジトリ `FrontendTemplates` へ移動済み |
| **`Legacy/`** | `2CS_sample` / `Bat_sample` / `CLI_sample` / `WS_sample` | `../Samples`（net48）を .NET へ移植した**独立コピー**。名前どおり「レガシー」扱い |

**全プロジェクトが SDK 形式 csproj / `net10.0`（GUI は `net10.0-windows7.0`）。**

---

## 2. 前提条件

### 2-1. フレームワークを先にビルドする（最重要）

**18 プロジェクト中 17 が `Build_netcore100\net10.0\*.dll` を `HintPath` で直接参照する**
（例外は `Legacy/CLI_sample/Simple_CLI` のみ。こちらはフレームワーク非依存）。
`ProjectReference` ではないので、**フレームワークをビルドしない限り古い / 存在しない DLL を見る。**

```xml
<Reference Include="OpenTouryo.Framework">
  <HintPath>..\..\..\..\Frameworks\Infrastructure\Build_netcore100\net10.0\OpenTouryo.Framework.dll</HintPath>
</Reference>
```

```
dotnet build ../Frameworks/Infrastructure/Nuget_netcore100.sln     ★先にこれ
dotnet build ../Frameworks/Infrastructure/Business_netcore100.sln
→ その後に各サンプル
```

> net48 側の `Samples` が参照する `Build\` とは**別のディレクトリ**。
> `4_Build_CopyAssemblies.bat`（`Build_net48` → `Build\`）は **.NET (Core) 側には関係しない**。

### 2-2. リポジトリを `C:\root\` に配置する

`Backend/MVC_Sample` と `Legacy/*` の設定は `C:/root/files/resource/...` を直書きしている。

```json
"FxXMLSPDefinition": "C:/root/files/resource/XML/SPDefinition.xml",
"SqlTextFilePath":   "C:/root/files/resource/Sql"
```

チェックアウト先が `C:\root` 以外（例: `C:\OpenTouryo\root`）の場合、**ビルドは通るが実行時に落ちる**。
net48 側と違い **区切り文字が `/`**（JSON なのでエスケープ回避）。

### 2-3. データベース

SQL Server の **Northwind** が既定。`Legacy/Bat_sample/RerunnableBatch_sample*` は `ORDERS2` テーブルが追加で必要。
接続文字列は net48 版と同一のサンプル値（`localhost` / `sa` / `seigi@123` 等）。
Core 側は **PostgreSQL (`ConnectionString_NPS`) が追加**されている点が net48 版との差。

---

## 3. `Backend/MVC_Sample` — 現行の推奨サンプル

ASP.NET Core MVC（`Microsoft.NET.Sdk.Web` / `net10.0`）。`Startup.cs` 方式（Minimal API ではない）。

### 3-1. フレームワークの組み込み方（テンプレートとして重要）

```csharp
// Program.cs
public static void Main(string[] args)
{
    OAuth2AndOIDCClient.HttpClient = new HttpClient();   // ★静的 HttpClient を差し込む
    Program.BuildWebHost(args).Run();
}

// Startup.cs
public Startup(IConfiguration configuration)
{
    Configuration = configuration;
    GetConfigParameter.InitConfiguration(configuration);  // ★必須。これが無いと Fx* 設定が全て null
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app._UseHttpContextAccessor();   // ★必須。MyHttpContext.Current を有効化する拡張メソッド
    ...
    app.UseSession(new SessionOptions() { ... });   // セッション機能を使うなら必須
}
```

**この 3 点（`InitConfiguration` / `_UseHttpContextAccessor` / `UseSession`）が
.NET (Core) で Open棟梁 を動かすための定型**。`_UseHttpContextAccessor` は
`Touryo.Infrastructure.Framework.StdMigration.HttpContextExtensions` の拡張メソッドで、
`System.Web.HttpContext.Current` 相当を `MyHttpContext.Current` として復活させる。

### 3-2. 構成

```
MVC_Sample/
├─ Program.cs / Startup.cs
├─ Controllers/
│   ├─ Crud1Controller.cs   … Ajax（部分更新）版 CRUD
│   ├─ Crud2Controller.cs   … Html.BeginForm（全体更新）版 CRUD
│   ├─ HomeController.cs    … ログイン / OAuth2 認可コードグラント / ログアウト
│   ├─ ErrorController.cs
│   └─ PingController.cs    … 死活監視（MyBaseMVControllerCore を継承しない素の Controller）
├─ Logic/Business/LayerB.cs      … B層
├─ Logic/Common/Test*Value.cs    … 引数・戻り値クラス
├─ Logic/Dao/{LayerD,DaoShippers}.cs … D層
├─ Models/ViewModels/*.cs
├─ Views/{Crud1,Crud2,Home,Error,Shared}/*.cshtml
├─ wwwroot/{css,js,images}/       … bootstrap 等はリポジトリに直接格納（npm 不使用）
├─ appsettings.json / appsettings.Development.json
└─ Properties/launchSettings.json
```

- コントローラは **`MyBaseMVControllerCore` を継承**（`Touryo.Infrastructure.Business.Presentation`）。
- **`[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]`** が
  `Crud1` / `Crud2` に付与されており、Cookie 認証必須。
- B層呼び出しは **`await layerB.DoBusinessLogicAsync(...)`**（net48 版は同期の `DoBusinessLogic`）。

```csharp
public async Task<IActionResult> SelectCount(CrudViweModel model)
{
    LayerB layerB = new LayerB();
    TestReturnValue r = (TestReturnValue)await layerB.DoBusinessLogicAsync(
        testParameterValue, this.SelectIsolationLevel(model.DdlIso));
    ...
}
```

- `Crud1` = Ajax 部分更新（`_ChartView.cshtml` / `_MessageView.cshtml` を部分レンダリング）
- `Crud2` = `Html.BeginForm` で全体更新
  → **同じ業務ロジックを 2 つの UI 方式で呼ぶ対比サンプル**になっている。

### 3-3. 認証（`HomeController`）

- フォーム ログイン: `ClaimsIdentity` / `ClaimsPrincipal` を組み立てて `SignInAsync`
- **OAuth2 認可コードグラント**: `OAuth2AndOIDCClient.GetAccessTokenByCodeAsync` →
  id_token 検証 → `GetUserInfoAsync` → `SignInAsync`
- 接続先は `appsettings.json` の `SpRp_*` / `OAuth2AndOidc*` / `JwkSetUri`
  （既定は `https://localhost:44300/MultiPurposeAuthSite/...` = 別リポジトリの汎用認証サイト）

**この OAuth2 フローを動かすには MultiPurposeAuthSite が別途必要。** 単体では Login 画面までしか動かない。

### 3-4. npm / grunt は使わない

**`wwwroot/lib/` 配下（bootstrap 等）はリポジトリに直接格納**されており、
`package.json` / `gruntfile.js` / `RestoreLib1.bat` / `RestoreLib2.bat` は存在しない
（コミット `2a08482f` で npm/grunt を廃止した）。`10_Build_WebAppCore_sample.bat` も npm 手順を持たない。

### 3-5. 起動

```
dotnet run --project Backend/MVC_Sample/MVC_Sample/MVC_Sample.csproj
```

`launchSettings.json` のプロファイル: `http`(5219) / `https`(7014) / `IIS Express`(26422, ssl 44383)。

---

## 4. `Legacy/` — net48 版からの移植

`../Samples/` の **独立コピー**。`Compile Include` によるファイル共有は**していない**。

| ディレクトリ | 中身 | net48 版との差 |
|---|---|---|
| `Legacy/2CS_sample` | 2CSClientWin / 2CSClientWPF / CustCtrl / GenDaoAndBatUpd / TimeStamp | **`AsyncEvent_sample` が未移植** |
| `Legacy/Bat_sample` | SimpleBatch / RerunnableBatch ×3 | 同等 |
| `Legacy/CLI_sample` | Simple_CLI / DAG_Login_CLI / LIR_Login_CLI | **こちらにしか実体が無い**（後述） |
| `Legacy/WS_sample` | WSServer / WSIFType / WSClient ×3 | **`WSClientWinCone_sample` が未移植** |

### 4-1. `Samples`（net48）との同一性 — 実測

| サンプル群 | 同一ファイル | 差異あり | net48 側のみ |
|---|---|---|---|
| `2CS_sample` | 138 | 18 | 26 |
| `Bat_sample` | 73 | 14 | 8 |
| `WS_sample` | 65 | 14 | 36 |

- **`Business/LayerB.cs` や `Dao/*.cs` は多くがバイト単位で同一。**
- 差異があるのは主に `*.csproj` / `*.sln` / `Program.cs`。
- net48 側のみ = `app.config`（Core は `appsettings.json`）、`Properties/AssemblyInfo.cs`（SDK 形式は自動生成）。

→ **B層・D層のロジックを直したら `../Samples/` 側の同名ファイルも同じ修正が必要。**

### 4-2. `Program.cs` の差分（移植の定型）

Core 側は **冒頭で config を明示初期化する**のが唯一の実質的な差。

```csharp
// configの初期化（net48 版には無い）
string dir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory
    .FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
GetConfigParameter.InitConfiguration(dir + "/appsettings.json");
```

以降の `TestParameterValue` 生成 → `layerB.DoBusinessLogic(...)` は net48 版と同一。

### 4-3. `Legacy/CLI_sample` — ここにしか無い

net48 版は README のみ（Sharprompt が .NET Framework サポートを終了したため）。
実体は Core 側だけに存在する。

| プロジェクト | 内容 |
|---|---|
| `Simple_CLI` | `System.CommandLine` + `Sharprompt` の CLI テンプレート。**フレームワーク非依存**（`OpenTouryo.*` 参照なし） |
| `DAG_Login_CLI` | OAuth 2.0 **Device Authorization Grant** クライアント |
| `LIR_Login_CLI` | OAuth 2.0 for Native Apps の **Loopback Interface Redirection** |

`DAG` / `LIR` は `OpenTouryo.Framework` / `Public` / `Public.Security` を参照する。

> **注意**: `System.CommandLine` が **`2.0.0-beta1.21216.1`（2021 年の beta）** で固定されている。
> net48 版 README には「beta が解除され Ctrl-C で CancellationToken がハングする問題が修正されたら移植する」
> と書かれており、この beta 依存は**意図的に据え置かれている**。安易に上げない。

### 4-4. `Legacy/WS_sample` — リモート プロトコルは実質使えない

`TMProtocolDefinition.xml` は net48 版からコピーされており `protocol="2"〜"5"`（asmx / WCF-HTTP /
WCF-TCP / WebAPI）の定義が残っているが、**.NET (Core) の `FxEnum.TmProtocol` は `InProcess`(1) しか持たない**
（`../Frameworks/ANALYSIS.md` 6.4 節）。

さらに `7_Build_Framework_WSCore.bat` はサービス ホスト側のビルドを丸ごと `rem` で無効化しており、
理由が `Core系のBinarySerializeの完全廃止対応` と記されている。

→ **Core 側の WS_sample は `testInProcess`（インプロセス呼び出し）でのみ動作する。**
定義ファイルに remote の記述が残っていることに惑わされないこと。

---

## 5. 設定（`appsettings.json`）

net48 の `app.config` → Core は `appsettings.json`。**`appSettings` / `connectionStrings` という
セクション名を JSON でもそのまま使う**のが Open棟梁の流儀。

```json
{
  "connectionStrings": {
    "ConnectionString_SQL": "Data Source=localhost;Initial Catalog=Northwind;...",
    "ConnectionString_NPS": "HOST=localhost;DATABASE=postgres;..."
  },
  "appSettings": {
    "FxXMLSPDefinition": "C:/root/files/resource/XML/SPDefinition.xml",
    "FxLog4NetConfFile": "C:/root/files/resource/Log/SampleLogConf.xml",
    "FxSqlTraceLog": "on",
    "SqlTextFilePath": "C:/root/files/resource/Sql"
  }
}
```

- **`appsettings.json` はコメント付き JSON（JSONC）**で書かれている。
  `Microsoft.Extensions.Configuration.Json` は `//` コメントを許容するが、
  厳密な JSON パーサで読むと壊れるので注意。
- `Legacy/*` は `csproj` で `<CopyToOutputDirectory>Always</CopyToOutputDirectory>` を
  `appsettings.json` / `MSGDefinition.xml` / `SPDefinition.xml` / `SampleLogConf2CS.xml` に指定している。
  **定義ファイルを追加したら csproj にもコピー指定を足す**必要がある。
- `Backend/MVC_Sample` は Web SDK なので `appsettings.json` は自動でコピーされる。

---

## 6. ビルド

`root/programs/CS/` 直下の bat（内部は `dotnet restore` + `dotnet msbuild`）。

| bat | 対象 |
|---|---|
| `5_Build_2CSCore_sample.bat` | `Legacy/2CS_sample/*` |
| `5_Build_BatCore_sample.bat` | `Legacy/Bat_sample/*`（4 sln） |
| `5_Build_CLICore_sample.bat` | `Legacy/CLI_sample/*` |
| `6_Build_WSSrvCore_sample.bat` | `Legacy/WS_sample/WSServer_sample` → `Temp` → `Build/net10.0` へ xcopy |
| `7_Build_Framework_WSCore.bat` | **中身が `rem` で無効化されている**（no-op） |
| `8_Build_WSClntCore_sample.bat` | `Legacy/WS_sample/WSClient_sample/*` |
| `10_Build_WebAppCore_sample.bat` | `Backend/MVC_Sample` |

bat を経由せず **`dotnet build <sln>` を直接叩いてよい**（Core 側は MSBuild.exe 不要）。
ただし **フレームワークを先にビルドする**こと（2-1 節）。

---

## 7. 依存パッケージのバージョン ドリフト（実測）

サンプルはフレームワークを `HintPath` で直接参照するため **NuGet の推移的解決が効かない**。
.NET には binding redirect も無いので、**サンプル側のパッケージ版がフレームワーク側より古いと
起動直後に `FileNotFoundException`（`Could not load file or assembly ...`）で落ちる。**

`log4net` は全プロジェクトで **3.3.0**（フレームワーク本体と一致）に統一済み。

一方、以下は依然としてプロジェクトごとに版が違う。

| パッケージ | 混在しているバージョン |
|---|---|
| `Microsoft.Data.SqlClient` | **6.0.2**（Legacy） / **6.1.3**（Backend/MVC_Sample） / 6.0.1（フレームワーク） |
| `Newtonsoft.Json` | **13.0.3**（Legacy／フレームワーク） / **13.0.4**（Backend/MVC_Sample） |
| `Microsoft.Extensions.Configuration*` | 9.0.5 / 9.0.6 / 9.0.4（フレームワーク） |
| `System.Data.Odbc` | 9.0.5 / 9.0.6 |

サンプル側が**新しい**分には動作するが、**フレームワーク側が上がったとき（Dependabot 等）に
サンプルが取り残されると即座に起動不能になる**。フレームワークの依存を上げたら、
サンプル側（`Backend` / `Legacy` の全 csproj）も同時に上げること。

---

## 8. 落とし穴

1. **フレームワークを先にビルドしないと `HintPath` 参照が解決できない。** これが最頻の失敗要因。
2. **`GetConfigParameter.InitConfiguration(...)` を呼ばないと `Fx*` 設定が全て null**
   になり、定義ファイルが読めず例外になる。net48 には無かった手順。
3. **`app._UseHttpContextAccessor()` を呼ばないと `MyHttpContext.Current` が null。**
   Web アプリで必須。
4. **`C:/root/files/...` の絶対パス依存。** ビルドは通るが実行時に落ちる。
5. **`Legacy/` は `../Samples/` の独立コピー。** 片方を直したらもう片方も直す（4-1 節）。
6. **Core では `CallController` のリモート プロトコルが使えない**（`InProcess` のみ）。
   `TMProtocolDefinition.xml` に残る `protocol="2"〜"5"` は死んだ定義（4-4 節）。
7. **`_3TierEngine` / `MyBaseLogic` は .NET (Core) 版フレームワークに含まれない**
   （`Business_netcore100.csproj` が `Compile Remove` している）。
   Core のサンプルが使うのは `MyFcBaseLogic` 系のみ。
8. **`System.CommandLine` の beta 固定は意図的**（4-3 節）。
9. **`appsettings.json` はコメント付き JSON。**
10. **`Legacy/WS_sample/Build/` `Temp/` はビルド生成物**（`Build/` は `.gitignore` 済み）。
11. **`ReturnValue` を設定し忘れると戻り値が null**（フレームワーク仕様。`../Frameworks/ANALYSIS.md` 3.2 節）。
12. **`Frontend/` に実体は無い。** `OpenTouryoProject/FrontendTemplates` を見ること。
13. **`Backend/ASPNETWebService/` にも実体は無い。** `OpenTouryoProject/ResourceServerTemplates` へ移動済み。

---

## 9. `Samples`（net48）との対応表

| | `../Samples`（net48） | `Samples4NetCore` |
|---|---|---|
| TFM | `net48`（旧形式 csproj） | `net10.0` / `net10.0-windows7.0`（SDK 形式） |
| フレームワーク参照 | `Build\`（`4_Build_CopyAssemblies.bat` が生成） | `Build_netcore100\net10.0\` を直接 |
| 設定 | `app.config`（+ Web は `Web.config`） | `appsettings.json` |
| config 初期化 | 不要（暗黙） | **`InitConfiguration()` が必須** |
| Web | Web Forms（38 画面）+ MVC5 | ASP.NET Core MVC のみ |
| B層呼び出し | 同期 `DoBusinessLogic` | 非同期 `DoBusinessLogicAsync`（Backend） |
| WS リモート | asmx / WCF-HTTP / WCF-TCP / WebAPI | **InProcess のみ** |
| CLI | README のみ | **実体あり** |
| 非同期イベント | `AsyncEvent_sample` あり | 未移植 |
| NuGet | `packages.config`（Web） | `PackageReference` |

---

## 10. エージェント向け作業チェックリスト

- [ ] `AGENTS.md` のポリシー遵守（**git 操作をしない**）
- [ ] **フレームワークを先にビルド**（`Nuget_netcore100.sln` → `Business_netcore100.sln`）
- [ ] `Legacy/` を触ったら `../Samples/` 側の同名ファイルの追随要否を判定・報告（4-1 節）
- [ ] Web アプリを新規に起こすなら `InitConfiguration` / `_UseHttpContextAccessor` / `UseSession` を忘れない
- [ ] 定義 XML を追加したら `Legacy/*` の csproj に `CopyToOutputDirectory` を追記
- [ ] パッケージ バージョンを変えるならフレームワーク本体との整合を確認（7 節）
- [ ] 新規 .cs にはヘッダ コメント（Apache License / クラス名・日本語名・更新履歴）を付与、既存変更時は履歴に 1 行追記。
      **Copyright ブロックは新規には付けない**（`../../CODING.md` 1 節）
- [ ] `UOC_` メソッドの冒頭で `this.ReturnValue = ...` を設定
- [ ] 実行確認をするなら `C:\root\files` と Northwind DB の準備状況を先に確認
