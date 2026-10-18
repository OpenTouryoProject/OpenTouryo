# ANALYSIS.md — Open棟梁 サンプル アプリ（CS/Samples）コード分析

対象: `root/programs/CS/Samples`（**.NET Framework 4.8 専用**） / ブランチ: `develop`
最終更新: 2026-10-17

本書は **コーディング・エージェントが本ディレクトリで作業する際の Context** を目的とする。
フレームワーク本体の分析は [`Frameworks/ANALYSIS.md`](../Frameworks/ANALYSIS.md) を、
.NET (Core) 版サンプルは [`Samples4NetCore/ANALYSIS.md`](../Samples4NetCore/ANALYSIS.md) を参照。

**本書は俯瞰である。** 個々のサンプルの中身・前提・落とし穴は、下の案内表の文書が一次情報を持つ（#613）。
本書には、**サンプルに共通すること**（前提、標準の構成、呼び出しの形、`ActionType` の約束、設定、ビルド、Legacy との関係）を残す。

---

## 1. これは何か

Open棟梁フレームワークの **使い方を示すサンプル兼テンプレート**。
実務では「該当するサンプルをコピーして中身を書き換える」のが標準的な使い方であり、
コード中に **`// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。`** という
断り書きが全ファイルに入っている。つまり **これらは「消して良い雛形」として書かれている**。

同時に **フレームワークの動作確認（テストベッド）** も兼ねており、
特に `WebForms_Sample` は P層機能（画面遷移制御・二重送信検出・ダイアログ等）の網羅テストになっている。

**全プロジェクトが `net48`（旧形式 csproj）。** SDK 形式は 1 つもない。

### 下位の文書（案内表）

| ディレクトリ | 文書 | 扱うこと |
|---|---|---|
| `2CS_sample/` | [`2CS_sample/ANALYSIS.md`](2CS_sample/ANALYSIS.md) | 2 層 C/S（WinForms / WPF）。自動生成 Dao、タイムスタンプ、カスタム コントロール、非同期イベント |
| `Bat_sample/` | [`Bat_sample/ANALYSIS.md`](Bat_sample/ANALYSIS.md) | バッチ。`SimpleBatch` と、リラン可能なバッチ 3 種（性能モデル）、引数 |
| `CLI_sample/` | [`CLI_sample/ANALYSIS.md`](CLI_sample/ANALYSIS.md) | **README のみ**。実装は .NET 10 版にだけある |
| `WS_sample/` | [`WS_sample/ANALYSIS.md`](WS_sample/ANALYSIS.md) | 3 層（クライアント → `ServiceInterface` → B 層）、通信の定義、ResourceServer（`ASPNETWebService`）、`WS_sample\Build\` |
| `WebApp_sample/` | [`WebApp_sample/ANALYSIS.md`](WebApp_sample/ANALYSIS.md) | `WebForms_Sample`（P 層の網羅テスト）と `MVC_Sample`（MVC5） |

---

## 2. 前提条件（これを満たさないと動かない）

### 2-1. リポジトリを `C:\root\` に配置する

**多くのサンプルの設定ファイルは `C:\root\files\resource\...` という絶対パスを直書きしている**
（2026-10-17 の実測で `app.config` 15 中 9。残りは SQL を出力先へコピーするか埋め込みリソースから読む。
置き方は [`2CS_sample/ANALYSIS.md`](2CS_sample/ANALYSIS.md) 2.1 節・[`Bat_sample/ANALYSIS.md`](Bat_sample/ANALYSIS.md) 3 節）。

```xml
<add key="FxXMLSPDefinition" value="C:\root\files\resource\Xml\SPDefinition.xml" />
<add key="SqlTextFilePath"   value="C:\root\files\resource\Sql" />
```

`root/Readme.ja.md` の手順どおり、リポジトリの `root` 配下を `C:\root\` に展開する必要がある。
本リポジトリのチェックアウト先が `C:\OpenTouryo\root\` の場合、**`C:\root\files` を別途用意しないと
実行時に定義ファイルが見つからず落ちる**（ビルドは通る）。

### 2-2. データベース

- **SQL Server の Northwind データベース**が既定（`app.config` の `ConnectionString_SQL`）。
- `Bat_sample/RerunnableBatch_sample*` は追加で **`ORDERS2` テーブル**が必要
  （[`Bat_sample/ANALYSIS.md`](Bat_sample/ANALYSIS.md) 3 節）。
- SQL Server 以外は `C:\root\files\resource\Sql\[DBMS名]\TestTable.txt` でテスト表を作成。
- 接続文字列は `localhost` / `sa` / `seigi@123` 等の **サンプル値**がリポジトリに直書きされている
  （公開 OSS のサンプル設定であり実資格情報ではない。流用時は必ず差し替える）。

### 2-3. フレームワークを先にビルドしておく

**全サンプル（20 プロジェクト）が `Frameworks\Infrastructure\Build\` の DLL を `HintPath` で参照する。**

```xml
<HintPath>..\..\..\Frameworks\Infrastructure\Build\OpenTouryo.Business.dll</HintPath>
```

`Build\` は `4_Build_CopyAssemblies.bat`（`Build_net48` → `Build\` を xcopy）が作るディレクトリ。
したがって **ビルド順は厳守**:

```
2_Build_NuGet_net48.bat        → Public / Framework / Public.Security / Dam*
3_Build_Business_net48.bat     → Business
4_Build_CopyAssemblies.bat     → Build_net48 を Build\ へコピー  ★これを飛ばすと全滅
5〜10_Build_*_sample.bat       → 各サンプル
```

`Build\` を参照するプロジェクトの全体（VB・Tools・Tests を含む）は
[`Frameworks/ANALYSIS.md`](../Frameworks/ANALYSIS.md) 7.1 節。

---

## 3. サンプル一覧

| ディレクトリ | 種別 | 概要 |
|---|---|---|
| `2CS_sample/*`（6 つ） | WinForms / WPF | **2層C/S の基本形**（`2CSClientWin` / `WPF`）、自動生成Dao とバッチ更新、タイムスタンプ（楽観排他）、カスタム コントロール、非同期イベント |
| `Bat_sample/*`（4 つ） | コンソール | **バッチの最小形**（`SimpleBatch`）と、リラン可能なバッチ 3 種 |
| `WS_sample/WSServer_sample` / `WSIFType_sample` | Library | **サービス側の B/D 層**と、サーバ／クライアントで共有する I/F 型 |
| `WS_sample/WSClient_sample/*`（4 つ） | WinForms/WPF | **3層型クライアント**。`CallController` 経由で B層を呼ぶ |
| `WS_sample/ASPNETWebService` | ASP.NET WebAPI | **ResourceServer**。`WSServer_sample` の B 層を CRUD の Web メソッドとして公開する（#566） |
| `WebApp_sample/WebForms_Sample` | ASP.NET Web Forms | **最大のサンプル（.aspx 38 画面）**。P層機能の網羅テスト |
| `WebApp_sample/MVC_Sample` | ASP.NET MVC5 | MVC5 + WebAPI 版 |
| `CLI_sample/*` | — | **README のみ（3 件）**。実装は `../Samples4NetCore/Legacy/CLI_sample/` にのみ存在する |

中身と見どころは各サンプルの文書（1 節の案内表）。

---

## 4. 標準的なプロジェクト構成（この形をコピーする）

```
XXX_sample/
├─ Business/LayerB.cs          … B層（MyFcBaseLogic 継承、UOC_ メソッド群）
├─ Common/TestParameterValue.cs … 引数クラス（MyParameterValue 継承）
├─ Common/TestReturnValue.cs    … 戻り値クラス（MyReturnValue 継承）
├─ Dao/LayerD.cs                … D層（MyBaseDao 継承、個別Dao）
├─ Dao/DaoShippers.cs           … D層（DaoGen_Tool が生成した自動生成Dao）
├─ Program.cs / Form1.cs        … P層（エントリポイント）
├─ MSGDefinition.xml            … メッセージ定義
├─ SPDefinition.xml             … 共有プロパティ定義
├─ SampleLogConf2CS.xml         … log4net 設定
├─ app.config                   … 接続文字列 + appSettings
└─ readme.txt                   … 実行方法（コマンドライン引数など）
```

WS 系はこれに `TMProtocolDefinition.xml` / `TMInProcessDefinition.xml` が加わる。
Web 系は `Logic/{Business,Common,Dao}` または `AppCode/sample/{Business,Common,Dao}` に入る。

---

## 5. 呼び出しパターン

### 5-1. インプロセス（2層C/S・バッチ・Web）

```csharp
// P層
TestParameterValue p = new TestParameterValue(
    screenId, controlId, "SelectCount",         // ← methodName が UOC_ 振り分けキー
    "SQL%individual%static%-",                  // ← ActionType（6 節）
    new MyUserInfo("", ""));

LayerB layerB = new LayerB();
TestReturnValue r = (TestReturnValue)layerB.DoBusinessLogic(p, DbEnum.IsolationLevelEnum.ReadCommitted);

if (r.ErrorFlag) { /* 業務例外：ErrorMessageID / ErrorMessage / ErrorInfo */ }
else             { /* 正常系：r.Obj など */ }
```

### 5-2. サービス呼び出し（WS クライアント）

```csharp
CallController cc = new CallController(context);
object ret = cc.Invoke("testInProcess", parameterValue);   // 論理名で解決
```

論理名 → プロトコル は `TMProtocolDefinition.xml`、
論理名 → assembly/class は `TMInProcessDefinition.xml` で解決される。
定義の実例（`protocol` 1〜5 と行き先）と、リモートを使うときの前提は
[`WS_sample/ANALYSIS.md`](WS_sample/ANALYSIS.md) 2.1・2.2 節。

---

## 6. `ActionType` の規約（サンプル横断のローカル ルール）

`%` 区切りの 4 要素。**フレームワークが解釈するのは [0] のみ**で、[1] 以降はサンプル独自。

| 位置 | 意味 | 値 |
|---|---|---|
| `[0]` | **データ プロバイダ選択**（フレームワークが解釈） | `SQL` `OLE` `ODB` `ODP` `MCN`（net48 で有効なもの。`ORA` / `DB2` / `HIR` はテンプレート側でコメントアウトされている） |
| `[1]` | Dao の種類（サンプル独自） | `common`（`CmnDao`） / `generate`（自動生成Dao） / それ以外（個別 `LayerD`） |
| `[2]` | SQL の種類（サンプル独自） | `static`（`.sql`） / `dynamic`（`.xml` = DPQ） |
| `[3]` | ロールバック試験（サンプル独自） | `Business`（業務例外） / `System`（システム例外） / `-`（正常） |

**[0] がどれにも当たらなければ、黙って SQL Server になる**（キーの綴りを誤っても例外にならない）。
キーと Dam・接続文字列の対応は [`Business/ANALYSIS.md`](../Frameworks/Infrastructure/Business/ANALYSIS.md) 3.1 節。

バッチのコマンドライン例（`SimpleBatch_sample/readme.txt`）:

```
/Dap SQL /Mode1 individual /Mode2 static /EXROLLBACK -
```

`StringVariableOperator.GetCommandArgs('/', out argsDic, out valsLst)` で分解し、
**キーは大文字化されて `argsDic["/DAP"]` のように引かれる**点に注意（[`Bat_sample/ANALYSIS.md`](Bat_sample/ANALYSIS.md) 2.2 節）。

---

## 7. 設定ファイル

**設定ファイルの仕組み（種類・読み込み順・`%変数%` の展開・パスの落とし穴）は
[`CONFIGURATION.md`](../../CONFIGURATION.md) が一次情報。** 本節は、サンプルに共通する置き方に留める。

### 7-1. コンソール / WinForms / WPF — `app.config`

`appSettings` に `Fx*` キー、`connectionStrings` に `ConnectionString_{SQL,OLE,ODBC,ODP,MCN}`。
`<runtime><assemblyBinding>` に **大量の `bindingRedirect`** が入っている（net48 の宿命）。
NuGet パッケージを更新すると、ここも追随が必要（`CompareRedirect.ps1`）。

### 7-2. Web — `Web.config` + `app.config`

`WebForms_Sample` は `<appSettings file="app.config" />` で **appSettings を外部ファイルに分離**している。
`Fx*` キーの実体は `app.config` 側にあるので、**設定を探すときは `Web.config` ではなく `app.config` を見る**。

P層（Web Forms）専用の設定群:

| 種別 | キー |
|---|---|
| コントロール接頭辞 | `FxPrefixOfButton`=btn, `FxPrefixOfTextBox`=txt, `FxPrefixOfGridView`=gvw … |
| 基盤画面パス | `FxErrorScreenPath` `FxOKMessageDialogPath` `FxYesNoMessageDialogPath` `FxDialogFramePath` |
| アイコン | `FxInformationIconPath` `FxWarningIconPath` `FxErrorIconPath` `FxQuestionIconPath` |
| ダイアログ既定スタイル | `FxDefaultFxDialogStyle` `FxDefaultBusinessDialogStyle` `FxDefaultNormalScreenStyle` |
| 画面制御 | `FxSessionTimeOutCheck` `FxDoubleTransmissionCheck` `FxScreenTransitionMode` `FxScreenTransitionCheck` |
| キュー長 | `FxRequestTicketGuidMaxQueueLength` `FxButtonhistoryMaxQueueLength` `FxScreeenGuidMaxQueueLength` `FxWindowGuidMaxQueueLength` |
| XML 定義 | `FxXMLSPDefinition` `FxXMLMSGDefinition` `FxXMLSCDefinition` `FxXMLTCDefinition` `FxXMLTMProtocolDefinition` `FxXMLTMInProcessDefinition` |
| `_3TierEngine` 命名規約 | `DaoClassNameHeader`=Dao, `MethodNameHeaderS`=S, `MethodNameHeaderD`=D, `MethodLabel_Ins`=1_Insert … `UpdateParamHeader`=Set_, `LikeParamFooter`=_Like |

**`_3TierEngine` の命名規約キーは自動生成Daoのメソッド名（`S1_Insert` / `D2_Select` / `D5_SelCnt`）と直結**しており、
片方だけ変えると実行時に解決できなくなる。

### 7-3. `Fx*` キーは大文字小文字を区別しない

`MyBaseDao.SetSqlByFile2` は内部で `GetConfigValue("sqlTextFilePath")`（小文字 s）を引くが、
`app.config` の定義は `SqlTextFilePath`（大文字 S）。
.NET Framework の `appSettings` も `Microsoft.Extensions.Configuration` も**キー比較は大文字小文字を無視する**ため
動作するが、意図的な一致ではないので、**このキー名を「揃える」目的で書き換えない**こと。

---

## 8. ビルドと実行

### ビルド

`root/programs/CS/` 直下の bat（`z_Common.bat` が MSBuild を解決）。検証としてのビルドは [`BUILDING.md`](../../BUILDING.md)。

| bat | 対象 |
|---|---|
| `5_Build_2CS_sample.bat` | `2CS_sample/*` |
| `5_Build_Bat_sample.bat` | `Bat_sample/*`（4 sln を順次） |
| `5_Build_CLI_sample.bat` | `CLI_sample`（**実体が無いので、メッセージを出して `pause` するだけ**。対話で止まるので、無人で回す手順からは外す） |
| `6_Build_WSSrv_sample.bat` | `WSServer_sample` → `WS_sample/Temp` → `WS_sample/Build` へ xcopy。続けて `ASPNETWebService` |
| `7_Build_Framework_WS.bat` | `ServiceInterface`。`WS_sample/Build` を `WCFService/dll` へコピーしてからビルド |
| `8_Build_WSClnt_sample.bat` | `WSClient_sample/*`（4 sln） |
| `10_Build_WebApp_sample.bat` | `WebForms_Sample` → `MVC_Sample` |

**`WS_sample/Build/` は中継点**であり、`ServiceInterface`・`ASPNETWebService`・クライアントが
ここから DLL を拾う（`.gitignore` 済み。[`WS_sample/ANALYSIS.md`](WS_sample/ANALYSIS.md) 4 節）。

Web 系は `packages.config` 方式（NuGet classic）なので、**`nuget.exe restore` が必須**
（各 bat が `..\nuget.exe restore` を実行している）。

### 実行

| 対象 | 前提 | 詳細 |
|---|---|---|
| バッチ | コマンドライン引数（`readme.txt`） | [`Bat_sample/ANALYSIS.md`](Bat_sample/ANALYSIS.md) 4 節 |
| `AsyncEvent_sample` | `test-winx2&wpfx2.bat` から 4 プロセス起動 | [`2CS_sample/ANALYSIS.md`](2CS_sample/ANALYSIS.md) |
| `WebForms_Sample` / `MVC_Sample` | **ASP.NET 状態サービス（`aspnet_state`）**と DB | [`WebApp_sample/ANALYSIS.md`](WebApp_sample/ANALYSIS.md) 2.3・4 節 |
| WS クライアント（リモート） | 先に `ServiceInterface`（WCF/WebAPI ホスト）を起動 | [`WS_sample/ANALYSIS.md`](WS_sample/ANALYSIS.md) 2.2 節 |

**サービスやホストを勝手に起動しない**（`AGENTS.md`）。止まっていたら報告する。
GUI のサンプルは疎通テストの対象外（[`SMOKETEST.md`](../../SMOKETEST.md)）。

---

## 9. 個別サンプルの読みどころ

### `Bat_sample/SimpleBatch_sample` — まずここを読む

B層（`LayerB.UOC_SelectCount` 等）が **`CmnDao` / 自動生成Dao / 個別Dao の 3 経路**を
`switch` で切り替える形になっており、Open棟梁のデータアクセス手段が 1 ファイルで俯瞰できる。

```csharp
switch ((testParameter.ActionType.Split('%'))[1])
{
    case "common":   CmnDao cmnDao = new CmnDao(this.GetDam());
                     cmnDao.SQLFileName = "ShipperCount.sql";   // or .xml (DPQ)
                     testReturn.Obj = cmnDao.ExecSelectScalar(); break;
    case "generate": DaoShippers genDao = new DaoShippers(this.GetDam());
                     testReturn.Obj = genDao.D5_SelCnt();       break;
    default:         LayerD myDao = new LayerD(this.GetDam());
                     myDao.SelectCount(testParameter, testReturn); break;
}
```

### `Dao/LayerD.cs` の `テンプレ()` メソッド

**日本語のメソッド名「テンプレ」**が各サンプルの `LayerD` に入っており、
`SetSqlByFile2` / `SetSqlByCommand` / `SetParameter` / `ExecXxx` の全パターンが
コメント付きで並んでいる。**D層を書くときのチートシート**として機能する。
実運用コードにコピーする際は削除する前提。

### `Dao/DaoShippers.cs` — DaoGen_Tool の出力形

ヘッダに `作成者：棟梁 D層自動生成ツール（墨壺）` と入る。メソッド命名は
`S1_Insert` `D1_Insert` `S2_Select` `D2_Select` `S3_Update` `D3_Update` `S4_Delete` `D4_Delete` `D5_SelCnt`
（`S`=静的SQL、`D`=動的SQL(DPQ)、番号は `MethodLabel_*` 設定に対応）。
**手で編集せず、ツールで再生成する**のが本来の運用（墨壺の CLI は
[`DaoGen_Tool/README.md`](../Frameworks/Tools/DaoGen_Tool/README.md)）。

### その他

| サンプル | 読みどころ | 文書 |
|---|---|---|
| `WebForms_Sample` | P層機能のテストベッド（`Aspx/testFxLayerP/` が最重要）、マルチ トランザクション制御 | [`WebApp_sample/ANALYSIS.md`](WebApp_sample/ANALYSIS.md) 2 節 |
| `RerunnableBatch_sample{,2,3}` | 性能モデル 3 種（通常 / `ExecGenerateSQL` / `DataTable` から INSERT 文） | [`Bat_sample/ANALYSIS.md`](Bat_sample/ANALYSIS.md) 2 節 |

---

## 10. 落とし穴

1. **`4_Build_CopyAssemblies.bat` を飛ばすと全サンプルがビルドできない。**
   `Build\`（`Build_net48` のコピー）を `HintPath` 参照しているため。
2. **`C:\root\files\...` の絶対パス依存。** チェックアウト先が `C:\root` 以外だと実行時に落ちる。
   IDE から F5 する前に `C:\root\files` の存在を確認すること。
3. **`net48` のみ。** .NET (Core) 版は `../Samples4NetCore/Legacy/` にある**別コピー**。
   B層・D層のソースは**ほぼ同一だが独立管理**（11 節）→ **片方を直したらもう片方も直す**。
4. **`ReturnValue` を設定し忘れると戻り値が null。** `UOC_` メソッドの冒頭で必ず
   `this.ReturnValue = testReturn;` する（フレームワーク側の仕様。[`Frameworks/ANALYSIS.md`](../Frameworks/ANALYSIS.md) 3.2 節）。
5. **`app.config` の `bindingRedirect` が大量**。NuGet 更新時は追随が必要。
6. **`WebForms_Sample` / `MVC_Sample` は `packages.config` 方式**。`PackageReference` 移行はされていない。
   `nuget.exe restore` を忘れるとビルドが落ちる。
7. **`CLI_sample` にコードは無い**（README のみ）。実装を探すなら `../Samples4NetCore/Legacy/CLI_sample/`。
   `5_Build_CLI_sample.bat` は `pause` で止まる。
8. **`WS_sample/Build/` `WS_sample/Temp/` はビルド生成物**。コミット対象ではない（`Build/` は `.gitignore` 済み）。
9. **`_3TierEngine` の命名規約キーと自動生成Daoのメソッド名は連動**。片方だけ変えない（7-2 参照）。
10. **VB 版ミラーが存在**（`root/programs/VB/Samples/`）。構成はほぼ 1:1 対応。

---

## 11. `Samples` と `Samples4NetCore/Legacy` の関係

`Samples4NetCore/Legacy/` は `Samples/` を **.NET (Core) 向けに移植した独立コピー**。
`Compile Include` によるファイル共有は**していない**（リンク参照ゼロ）。

- **`Business/LayerB.cs` や `Dao/*.cs` は多くがバイト単位で同一。**
  差異があるのは主に `*.csproj` / `*.sln` / `Program.cs`（Core 側は `GetConfigParameter.InitConfiguration()` 呼び出しが増える）。
- 移植していないもの: `AsyncEvent_sample` / `WSClientWinCone_sample`。`ASPNETWebService` の .NET 版は `Backend/` にある。
- 実測した同一性（ファイル数）と、移植の定型は [`Samples4NetCore/Legacy/ANALYSIS.md`](../Samples4NetCore/Legacy/ANALYSIS.md) 2・3 節。

→ **B層・D層のロジックを修正したら、`Samples4NetCore/Legacy/` 側の同名ファイルも同じ修正が必要**（VB 版も）。

---

## 12. エージェント向け作業チェックリスト

- [ ] `AGENTS.md` のポリシー遵守（**git 操作をしない**）
- [ ] 作業対象のサンプルの文書（1 節の案内表）の「落とし穴」と「作業チェックリスト」を読む
- [ ] 変更が `Samples4NetCore/Legacy/` 側にも必要か判定（11 節）
- [ ] `Frameworks` を先にビルド → `4_Build_CopyAssemblies.bat` を実行（`Build\` を作る）
- [ ] Web 系を触るなら `nuget.exe restore`（`packages.config` 方式）
- [ ] 新規 .cs にはヘッダ コメント（Apache License / クラス名・日本語名・更新履歴）を付与、既存変更時は履歴に 1 行追記。
      **Copyright ブロックは新規には付けない**（[`CODING.md`](../../CODING.md) 1 節）
- [ ] `UOC_` メソッドの冒頭で `this.ReturnValue = ...` を設定
- [ ] `ActionType` の `%` 区切り規約を壊さない（6 節）
- [ ] 設定キーを増やしたら `app.config`（Web は `app.config` 側）に追記
- [ ] 実行確認をするなら `C:\root\files` と Northwind DB の準備状況を先に確認
