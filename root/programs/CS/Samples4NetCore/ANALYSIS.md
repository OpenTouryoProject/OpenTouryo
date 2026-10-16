# ANALYSIS.md — Open棟梁 サンプル アプリ（CS/Samples4NetCore）コード分析

対象: `root/programs/CS/Samples4NetCore`（**.NET 10.0**） / ブランチ: `develop`
最終更新: 2026-10-17

本書は **コーディング・エージェントが本ディレクトリで作業する際の Context** を目的とする。
フレームワーク本体は [`Frameworks/ANALYSIS.md`](../Frameworks/ANALYSIS.md)、net48 版サンプルは [`Samples/ANALYSIS.md`](../Samples/ANALYSIS.md) を参照。

**本書は俯瞰である。** 区画ごとの詳細は、下の案内表の文書が一次情報を持つ（#613）。
本書には、**区画に共通すること**（前提、ビルド、パッケージの版の揃え方、net48 版との対応）を残す。

---

## 1. これは何か

Open棟梁の **.NET (Core) 系サンプル**。3 つの区画に分かれており、性格がまったく違う。

| 区画 | 中身 | 性格 | 文書 |
|---|---|---|---|
| **`Backend/`** | `MVC_Sample`（ASP.NET Core MVC）/ `ASPNETWebService`（ResourceServer） | **現行の推奨サンプル。** ここが本命 | [`Backend/ANALYSIS.md`](Backend/ANALYSIS.md) |
| **`Legacy/`** | `2CS_sample` / `Bat_sample` / `CLI_sample` / `WS_sample` | `../Samples`（net48）を .NET へ移植した**独立コピー**。名前どおり「レガシー」扱い | [`Legacy/ANALYSIS.md`](Legacy/ANALYSIS.md) |
| **`Docker/`** | `Backend/` のサンプルを Linux コンテナで動かす一式 | サンプルのコードは持たない。**対象ごとに分かれる**（#548、#582） | [`Docker/README.md`](Docker/README.md) |

**全プロジェクトが SDK 形式 csproj / `net10.0`（GUI は `net10.0-windows7.0`）。**

かつて在った `Frontend/` は、別リポジトリ `OpenTouryoProject/FrontendTemplates` へ移して、ここには残っていない。

---

## 2. 前提条件

### 2-1. フレームワークを先にビルドする（最重要）

**19 プロジェクト中 18 が `Build_netcore100\net10.0\*.dll` を `HintPath` で直接参照する**
（2026-10-17 の実測。例外は `Legacy/CLI_sample/Simple_CLI` のみ。こちらはフレームワーク非依存）。
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

**`Backend/ASPNETWebService` は、さらに `Legacy/WS_sample/Build/net10.0/` の `WSServer_sample` / `WSIFType_sample` に依存する**
（net48 版と同じ構図。`6_Build_WSSrvCore_sample.bat` が先に作る。[`Backend/ANALYSIS.md`](Backend/ANALYSIS.md) 3.2 節）。

### 2-2. リポジトリを `C:\root\` に配置する

`Backend/*` の 2 つと、`Legacy/` の一部（`2CSClientWin` / `2CSClientWPF` / `SimpleBatch` / `WSClientWin` / `WSClientWPF`）の
`appsettings.json` は、`C:/root/files/resource/...` を直書きしている。

```json
"FxXMLSPDefinition": "C:/root/files/resource/Xml/SPDefinition.xml",
"SqlTextFilePath":   "C:/root/files/resource/Sql"
```

チェックアウト先が `C:\root` 以外（例: `C:\OpenTouryo\root`）の場合、**ビルドは通るが実行時に落ちる**。
net48 側と違い **区切り文字が `/`**（JSON なのでエスケープ回避）。

`Legacy/` の残りは、定義ファイルをファイル名だけで指定して出力先へコピーするか、埋め込みリソースから読む
（[`Legacy/ANALYSIS.md`](Legacy/ANALYSIS.md) 3.2 節）。コンテナでは `%OT_RESOURCE_ROOT%` で置き場を差し替える
（[`CONFIGURATION.md`](../../CONFIGURATION.md) 5 節）。

### 2-3. データベース

SQL Server の **Northwind** が既定。`Legacy/Bat_sample/RerunnableBatch_sample*` は `ORDERS2` テーブルが追加で必要。
接続文字列は net48 版と同一のサンプル値（`localhost` / `sa` / `seigi@123` 等）。
Core 側は **PostgreSQL (`ConnectionString_NPS`) が追加**されている点が net48 版との差（`NPS` は net10.0 でだけ使える。
[`Business/ANALYSIS.md`](../Frameworks/Infrastructure/Business/ANALYSIS.md) 3.1 節）。

---

## 3. `Backend/` — 現行の推奨サンプル

ASP.NET Core（`Microsoft.NET.Sdk.Web` / `net10.0`）。`Startup.cs` 方式（Minimal API ではない）。

| サンプル | 種類 | 認証 |
|---|---|---|
| `MVC_Sample` | MVC（画面あり。`MyBaseMVControllerCore` を継承） | Cookie 認証（フォーム ログイン、OAuth2 の認可コード グラント） |
| `ASPNETWebService` | Web API（ResourceServer。OpenAPI を出す） | `Authorization: Bearer`（無くても通す） |

**.NET (Core) で Open棟梁 を動かすための定型は 3 点**（`InitConfiguration` / `_UseHttpContextAccessor` / `UseSession`）。
コードの形、各サンプルのコントローラ、環境で切り替える設定（`UseHttpsRedirection` など）と実測した落とし穴、
疎通テストは [`Backend/ANALYSIS.md`](Backend/ANALYSIS.md)。

**OAuth2 のフローを動かすには、別リポジトリの MultiPurposeAuthSite が要る。** 単体ではログイン画面までしか動かない。

---

## 4. `Legacy/` — net48 版からの移植

`../Samples/` の **独立コピー**。`Compile Include` によるファイル共有は**していない**。

| ディレクトリ | net48 版との差 |
|---|---|
| `Legacy/2CS_sample` | **`AsyncEvent_sample` が未移植** |
| `Legacy/Bat_sample` | 同等 |
| `Legacy/CLI_sample` | **こちらにしか実体が無い**（`System.CommandLine` の beta は意図的に据え置き） |
| `Legacy/WS_sample` | **`WSClientWinCone_sample` が未移植。** リモート プロトコルは使えない（`InProcess` のみ） |

**B層・D層のロジックを直したら `../Samples/` 側の同名ファイルも同じ修正が必要。**
同一性の実測、移植の定型（`InitConfiguration`、`appsettings.json` の書き方、`SqlTextFilePath` の 3 通り）、
CLI と WS の事情は [`Legacy/ANALYSIS.md`](Legacy/ANALYSIS.md)。

---

## 5. 設定（`appsettings.json`）

**設定ファイルの仕組み（読み込み順、`__` 区切りの環境変数での上書き、`FxContainerization` との違い、`%変数%` の展開）は
[`CONFIGURATION.md`](../../CONFIGURATION.md) が一次情報。**

net48 の `app.config` → Core は `appsettings.json`。**`appSettings` / `connectionStrings` という
セクション名を JSON でもそのまま使う**のが Open棟梁の流儀。

- **`appsettings.json` はコメント付き JSON（JSONC）**で書かれている。
  `Microsoft.Extensions.Configuration.Json` は `//` コメントを許容するが、厳密な JSON パーサで読むと壊れる。
- `Legacy/*` は定義ファイルを csproj の `CopyToOutputDirectory` で出力先へ配る。**定義ファイルを足したら csproj にも足す**
  （[`Legacy/ANALYSIS.md`](Legacy/ANALYSIS.md) 3.2 節）。`Backend/*` は Web SDK なので `appsettings.json` は自動でコピーされる。
- `Backend/*` の環境ごとの切り替えキーと、実測した落とし穴は [`Backend/ANALYSIS.md`](Backend/ANALYSIS.md) 4 節。
- コンテナでの上書き（`appsettings.Container.json` ＋ `docker-compose.yml`）は [`Docker/README.md`](Docker/README.md)。

---

## 6. ビルド

`root/programs/CS/` 直下の bat（内部は `dotnet restore` + `dotnet msbuild`）。検証としてのビルドは [`BUILDING.md`](../../BUILDING.md)。

| bat | 対象 |
|---|---|
| `5_Build_2CSCore_sample.bat` | `Legacy/2CS_sample/*` |
| `5_Build_BatCore_sample.bat` | `Legacy/Bat_sample/*`（4 sln） |
| `5_Build_CLICore_sample.bat` | `Legacy/CLI_sample/*` |
| `6_Build_WSSrvCore_sample.bat` | `Legacy/WS_sample/WSServer_sample` → `Temp` → `Build/net10.0` へ xcopy。続けて `Backend/ASPNETWebService` |
| `7_Build_Framework_WSCore.bat` | **何もしない**（net10.0 にサービスのホストが無い） |
| `8_Build_WSClntCore_sample.bat` | `Legacy/WS_sample/WSClient_sample/*` |
| `10_Build_WebAppCore_sample.bat` | `Backend/MVC_Sample` |

bat を経由せず **`dotnet build <sln>` を直接叩いてよい**（Core 側は MSBuild.exe 不要）。
ただし **フレームワークを先にビルドする**こと（2-1 節）。

---

## 7. 依存パッケージの版を揃える

サンプルはフレームワークを `HintPath` で直接参照するため **NuGet の推移的解決が効かない**。
.NET には binding redirect も無いので、**サンプル側のパッケージ版がフレームワーク側より古いと
起動直後に `FileNotFoundException`（`Could not load file or assembly ...`）で落ちる。**
（net48 は逆で、`bindingRedirect` がずれても黙って通る。そちらは `CompareRedirect.ps1` が見る。）

2026-10-17 時点では、`log4net` / `Microsoft.Data.SqlClient` / `Newtonsoft.Json` / `Microsoft.Extensions.Configuration*` は
部品と同じ版に揃っている。揃っていないものと、その影響は [`Legacy/ANALYSIS.md`](Legacy/ANALYSIS.md) 6 節。
**版は csproj を直接見ること**（Dependabot と #610 の運用で随時変わる）。

サンプル側が**新しい**分には動作するが、**フレームワーク側が上がったとき（Dependabot 等）に
サンプルが取り残されると即座に起動不能になる**。フレームワークの依存を上げたら、
サンプル側（`Backend` / `Legacy` の全 csproj）も同時に見ること。

---

## 8. 落とし穴

1. **フレームワークを先にビルドしないと `HintPath` 参照が解決できない。** これが最頻の失敗要因（2-1 節）。
2. **`GetConfigParameter.InitConfiguration(...)` を呼ばないと設定が読めない**
   （`NOT_INITIALIZED` の `ArgumentException`。例外を握る API では黙って false）。net48 には無かった手順。
3. **`app._UseHttpContextAccessor()` を呼ばないと `MyHttpContext.Current` が null。**
   Web アプリで必須（[`Backend/ANALYSIS.md`](Backend/ANALYSIS.md) 2 節）。
4. **`C:/root/files/...` の絶対パス依存**（一部。2-2 節）。ビルドは通るが実行時に落ちる。
5. **`Legacy/` は `../Samples/` の独立コピー。** 片方を直したらもう片方も直す（4 節）。
6. **Core では `CallController` のリモート プロトコルが使えない**（`InProcess` のみ）。
   `TMProtocolDefinition.xml` に残るリモートの定義は死んでいる（[`Legacy/ANALYSIS.md`](Legacy/ANALYSIS.md) 5 節）。
7. **`_3TierEngine` / `MyBaseLogic` は .NET (Core) 版フレームワークに含まれない**
   （`Business_netcore100.csproj` が `Compile Remove` している）。
   Core のサンプルが使うのは `MyFcBaseLogic` 系のみ。
8. **`System.CommandLine` の beta 固定は意図的**（[`Legacy/ANALYSIS.md`](Legacy/ANALYSIS.md) 4 節）。
9. **`appsettings.json` はコメント付き JSON。**
10. **`Legacy/WS_sample/Build/` `Temp/` はビルド生成物**（`Build/` は `.gitignore` 済み）。
11. **`ReturnValue` を設定し忘れると戻り値が null**（フレームワーク仕様。[`Frameworks/ANALYSIS.md`](../Frameworks/ANALYSIS.md) 3.2 節）。
12. **`Backend/ASPNETWebService` は `Legacy/WS_sample` のビルド結果に依存する**（2-1 節）。`6_` を飛ばすとビルドできない。
13. **`Docker/` を変えても、CI と疎通テストでは何も確かめられない**（[`Docker/README.md`](Docker/README.md) 4 節）。

---

## 9. `Samples`（net48）との対応表

| | `../Samples`（net48） | `Samples4NetCore` |
|---|---|---|
| TFM | `net48`（旧形式 csproj） | `net10.0` / `net10.0-windows7.0`（SDK 形式） |
| フレームワーク参照 | `Build\`（`4_Build_CopyAssemblies.bat` が生成） | `Build_netcore100\net10.0\` を直接 |
| 設定 | `app.config`（+ Web は `Web.config`） | `appsettings.json` |
| config 初期化 | 不要（暗黙） | **`InitConfiguration()` が必須** |
| Web | Web Forms（38 画面）+ MVC5、Web API（`WS_sample/ASPNETWebService`） | ASP.NET Core MVC（`MVC_Sample`）、Web API（`ASPNETWebService`。OpenAPI あり） |
| B層呼び出し | 同期 `DoBusinessLogic` | 非同期 `DoBusinessLogicAsync`（Backend。中身は別スレッドで同期実行。[`Frameworks/ANALYSIS.md`](../Frameworks/ANALYSIS.md) 4.4 節） |
| WS リモート | asmx / WCF-HTTP / WCF-TCP / WebAPI | **InProcess のみ** |
| CLI | README のみ | **実体あり** |
| 非同期イベント | `AsyncEvent_sample` あり | 未移植 |
| NuGet | `packages.config`（Web） | `PackageReference` |
| コンテナ | 対象外 | `Docker/`（Linux コンテナ） |

---

## 10. エージェント向け作業チェックリスト

- [ ] `AGENTS.md` のポリシー遵守（**git 操作をしない**）
- [ ] 作業対象の区画の文書（1 節の表）の「落とし穴」と「作業チェックリスト」を読む
- [ ] **フレームワークを先にビルド**（`Nuget_netcore100.sln` → `Business_netcore100.sln`）
- [ ] `Legacy/` を触ったら `../Samples/` 側の同名ファイルの追随要否を判定・報告（4 節）
- [ ] Web アプリを新規に起こすなら `InitConfiguration` / `_UseHttpContextAccessor` / `UseSession` を忘れない
- [ ] 定義 XML を追加したら `Legacy/*` の csproj に `CopyToOutputDirectory` を追記
- [ ] パッケージ バージョンを変えるならフレームワーク本体との整合を確認（7 節）
- [ ] 新規 .cs にはヘッダ コメント（Apache License / クラス名・日本語名・更新履歴）を付与、既存変更時は履歴に 1 行追記。
      **Copyright ブロックは新規には付けない**（[`CODING.md`](../../CODING.md) 1 節）
- [ ] `UOC_` メソッドの冒頭で `this.ReturnValue = ...` を設定
- [ ] 実行確認をするなら `C:\root\files` と Northwind DB の準備状況を先に確認
- [ ] コンテナは勝手に起動しない（[`Docker/README.md`](Docker/README.md) 4 節）
