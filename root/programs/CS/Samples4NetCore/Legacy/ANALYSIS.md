# ANALYSIS.md — net48 版から移植したサンプル（CS/Samples4NetCore/Legacy）

対象: `root/programs/CS/Samples4NetCore/Legacy`（net10.0 / net10.0-windows7.0） / 最終更新: 2026-10-16

上位: [`Samples4NetCore/ANALYSIS.md`](../ANALYSIS.md)（区画の俯瞰・前提・ビルド・net48 との対応）

---

## 1. これは何か

`root/programs/CS/Samples/`（net48）を .NET 10 へ移植した**独立のコピー**。
**ファイルを共有していない**（`Compile Include` によるリンクは無い）。名前どおり「レガシー」の扱いで、
.NET の新しい Web アプリの手本は `Backend/` の方である（[`Backend/ANALYSIS.md`](../Backend/ANALYSIS.md)）。

| ディレクトリ | 中身 | net48 版との差 | 個別の分析（net48 版） |
|---|---|---|---|
| `2CS_sample` | `2CSClientWin` / `2CSClientWPF` / `CustCtrl` / `GenDaoAndBatUpd` / `TimeStamp` | **`AsyncEvent_sample` は移植していない** | [`Samples/2CS_sample/ANALYSIS.md`](../../Samples/2CS_sample/ANALYSIS.md) |
| `Bat_sample` | `SimpleBatch` / `RerunnableBatch` ×3 | 同等 | [`Samples/Bat_sample/ANALYSIS.md`](../../Samples/Bat_sample/ANALYSIS.md) |
| `CLI_sample` | `Simple_CLI` / `DAG_Login_CLI` / `LIR_Login_CLI` | **こちらにしか実装が無い**（4 節） | [`Samples/CLI_sample/ANALYSIS.md`](../../Samples/CLI_sample/ANALYSIS.md) |
| `WS_sample` | `WSServer` / `WSIFType` / クライアント 3 種（`Win` / `Win2` / `WPF`） | **`WSClientWinCone_sample` は移植していない。** リモート呼び出しは使えない（5 節） | [`Samples/WS_sample/ANALYSIS.md`](../../Samples/WS_sample/ANALYSIS.md) |

**各サンプルの中身（何を見せているか・前提・落とし穴）は、net48 版の文書がそのまま当てはまる。**
本書は .NET 10 版に特有のことだけを扱う。

---

## 2. net48 版との同一性（実測、2026-10-16）

同じ相対パスのファイルを、バイト単位で比べた結果。

| サンプル群 | 同一 | 差異あり | net48 側のみ | .NET 側のみ |
|---|---:|---:|---:|---:|
| `2CS_sample` | 138 | 18 | 27 | 6 |
| `Bat_sample` | 73 | 14 | 9 | 4 |
| `WS_sample` | 65 | 14 | 55 | 3 |

- **`Business/LayerB.cs` や `Dao/*.cs` の多くはバイト単位で同一。** 差異は主に `*.csproj` / `*.sln` / `Program.cs`
- net48 側のみ = `app.config`（.NET は `appsettings.json`）、`Properties/AssemblyInfo.cs`（SDK 形式では自動生成）、
  未移植のサンプル（`AsyncEvent_sample` / `WSClientWinCone_sample`）、`WS_sample/ASPNETWebService`（.NET 版は `Backend/` にある）

**B 層・D 層のロジックを直したら、net48 版（と VB 版）の同じファイルも直す。**

---

## 3. 移植の定型

### 3.1 設定の初期化（`Program.cs`）

**.NET では、最初に `GetConfigParameter.InitConfiguration` を呼ぶ。** net48 版との実質的な差はほぼこれだけである。

```csharp
// バッチ（実行ファイルの場所を基準にする）
string dir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory
    .FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
GetConfigParameter.InitConfiguration(dir + "/appsettings.json");

// 2 層 C/S など（作業ディレクトリ基準。見つからなければ実行ファイルの場所で探し直す）
GetConfigParameter.InitConfiguration("appsettings.json");
```

以降の `TestParameterValue` の生成 → `layerB.DoBusinessLogic(...)` は net48 版と同じ。

### 3.2 設定ファイル（`appsettings.json`）

- `app.config` の代わりに `appsettings.json`。**`appSettings` / `connectionStrings` というセクション名を JSON でもそのまま使う**
- **コメント付きの JSON**（`//`）。`Microsoft.Extensions.Configuration.Json` は許すが、厳密な JSON パーサでは読めない
- 定義ファイル（`MSGDefinition.xml` / `SPDefinition.xml` / `SampleLogConf2CS.xml` など）は**ファイル名だけ**で指定し、
  csproj の `CopyToOutputDirectory=Always` で出力先へ配る。**定義ファイルを足したら、csproj にもコピーの指定を足す**
  （[`CONFIGURATION.md`](../../../CONFIGURATION.md) 7-3。`%変数%` にすると壊れる）
- 埋め込みリソースから読む書き方も、コメントで並べてある（`_2CSClientWin_sample.SPDefinition.xml` など。
  既定の名前空間 `RootNamespace` が数字で始まれないため `_` が付く）。切り替えるときは csproj の `EmbeddedResource` も要る

`SqlTextFilePath` の置き方は net48 版と同じく 3 通り:

| 値 | サンプル |
|---|---|
| `C:/root/files/resource/Sql`（絶対パス。区切りは `/`） | `2CSClientWin` / `2CSClientWPF` / `SimpleBatch` / `WSClientWin` / `WSClientWPF` |
| `./Dao`（出力先へコピー） | `TimeStamp` / `RerunnableBatch*` |
| `GenDaoAndBatUpd_sample.Dao`（埋め込みリソース） | `GenDaoAndBatUpd` |

### 3.3 プロジェクト

- すべて SDK 形式。GUI は `net10.0-windows7.0`
- **部品は `Frameworks/Infrastructure/Build_netcore100/` の DLL を `HintPath` で参照する**（17 プロジェクト中 16。例外は `Simple_CLI`）。
  **推移的な依存が効かない**ので、ドライバや設定のパッケージを各 csproj にも並べている（6 節）

---

## 4. `CLI_sample`（ここにしか実装が無い）

| プロジェクト | 内容 | 部品の参照 |
|---|---|---|
| `Simple_CLI` | `System.CommandLine` ＋ `Sharprompt` の CLI の雛形 | **なし**（フレームワーク非依存） |
| `DAG_Login_CLI` | OAuth 2.0 **Device Authorization Grant** のクライアント | `Public` / `Public.Security` / `Framework` |
| `LIR_Login_CLI` | OAuth 2.0 for Native Apps の **Loopback Interface Redirection** | 同上 |

- **`System.CommandLine` は `2.0.0-beta1.21216.1`（2021 年の beta）のまま。** net48 版の README にある
  「beta が解除され、Ctrl-C で `CancellationToken` がハングする問題が直ったら移植する」という判断と結び付いており、
  **意図的に据え置いている。** 上げるなら、net48 版の扱いと合わせて判断する
- 疎通テストは `Simple_CLI` だけ（`cmd1 --an-int 123` → `Sub command cmd1: 123`）。`interactive` サブコマンドは対話なので対象外。
  `DAG` / `LIR` は認証サイトが要るので対象外

---

## 5. `WS_sample`（インプロセスでしか動かない）

`TMProtocolDefinition.xml` は net48 版からの写しで、`protocol="4"`（WCF-TCP）・`"5"`（Web API）の定義が残っている。
しかし **.NET の `FxEnum.TmProtocol` は `InProcess`(1) しか持たない**
（[`Framework/ANALYSIS.md`](../../Frameworks/Infrastructure/Framework/ANALYSIS.md) 3.2 節）。
これらの定義を使うと `FrameworkException` になる（#543）。

- クライアントの画面も、通信方式の選択肢は「インプロセス呼出」だけにしてある（他はコメントアウト）
- **受ける側のホストも無い**（`7_Build_Framework_WSCore.bat` は何も建てない。`BinarySerialize` を .NET で廃止したため）

→ **.NET 版の `WS_sample` は `testInProcess` でだけ動く。** 定義に残るリモートの記述は死んでいる。

`WSServer_sample` は、`Backend/ASPNETWebService` が参照する B 層でもある
（`6_Build_WSSrvCore_sample.bat` が `WS_sample/Build/net10.0/` へ複写する。`Build/` と `Temp/` は生成物）。

---

## 6. 依存パッケージ（2026-10-16 時点）

`HintPath` 参照のため、**部品が使うパッケージを各サンプルの csproj にも並べている。**
.NET には `bindingRedirect` が無いので、**サンプル側の版が部品側より古いと、起動直後に `FileNotFoundException` で落ちる。**

いまは多くが部品と同じ版に揃っている（`log4net` 3.3.0、`Microsoft.Data.SqlClient` 7.0.0、`Newtonsoft.Json` 13.0.4、
`Microsoft.Extensions.Configuration*` 10.0.5）。**揃っていないもの:**

| パッケージ | Legacy | 部品（`Public`） |
|---|---|---|
| `System.Data.Odbc` | 9.0.5（5 件）/ 9.0.6（1 件） | 9.0.4 |
| `System.Security.Cryptography.Pkcs` | 10.0.13（`Bat_sample` の 4 件） | 9.0.21 |

サンプル側が**新しい**分には動く。**部品側が上がったときに、サンプルが取り残されると起動できなくなる。**
部品の依存を上げたら、`Legacy` と `Backend` の全 csproj も見ること。

---

## 7. ビルドと検証

| bat | 対象 |
|---|---|
| `5_Build_2CSCore_sample.bat` | `2CS_sample/*`（5 つ） |
| `5_Build_BatCore_sample.bat` | `Bat_sample/*`（4 つ） |
| `5_Build_CLICore_sample.bat` | `CLI_sample/*`（3 つ） |
| `6_Build_WSSrvCore_sample.bat` | `WS_sample/WSServer_sample` → `Build/net10.0/`（続けて `Backend/ASPNETWebService`） |
| `7_Build_Framework_WSCore.bat` | **何もしない**（5 節） |
| `8_Build_WSClntCore_sample.bat` | `WS_sample/WSClient_sample/*`（3 つ） |

bat を経由せずに `dotnet build <sln>` してよい。ただし**部品を先にビルドする。**

| 疎通テストの対象 | 件数 |
|---|---|
| `Bat_sample`（net10.0） | 4（DB が要る。`Orders2` は疎通テストが用意する） |
| `Simple_CLI`（net10.0） | 1 |
| 2 層 C/S・WS クライアント | **対象外**（GUI） |

---

## 8. 落とし穴

1. **net48 版と独立に管理している**（2 節）。片方だけ直すと食い違う。
2. **`InitConfiguration` を忘れると設定が読めない**（3.1 節）。
3. **定義ファイルを足したら csproj にコピーの指定を足す**（3.2 節）。
4. **WS のリモート定義は死んでいる**（5 節）。
5. **`System.CommandLine` の beta は意図的に据え置き**（4 節）。
6. **パッケージの版が部品とずれると、起動直後に落ちる**（6 節）。

---

## 9. 作業チェックリスト

- [ ] 部品を先にビルドする（`Nuget_netcore100.sln` → `Business_netcore100.sln`）
- [ ] 直したら、net48 版（`Samples/`）と VB 版の同じファイルへの追随の要否を報告する
- [ ] 定義ファイルを足したら、csproj の `CopyToOutputDirectory` を足す
- [ ] パッケージを変えるなら、部品側の版と揃える（6 節）
- [ ] GUI のサンプルを手で確かめていなければ、そう報告する
