# ANALYSIS.md — Open棟梁 フレームワーク本体（CS/Frameworks）コード分析

対象: `root/programs/CS/Frameworks` / ブランチ: `develop`
最終更新: 2026-07-31

本書は **コーディング・エージェントが本リポジトリで作業する際の Context** を目的とした分析結果である。
「どこに何があるか」「どの規約に従うべきか」「何を壊しやすいか」を記す。

---

## 1. これは何か

`Open棟梁 (OpenTouryo)` は、日立ソリューションズ発の .NET 用アプリケーション・フレームワーク（Apache-2.0）。
2007年から継続開発されており、**P層（画面）／B層（業務ロジック）／D層（データアクセス）の3層モデル**と、
**「UOC」= User Own Coding（利用者が実装する拡張ポイント）** という設計思想が全体を貫く。

- 上流ドキュメント: `OpenTouryoDocuments` リポジトリ（本リポジトリ外）
- 配布形態: NuGet パッケージ（`Touryo.Infrastructure.*` / アセンブリ名 `OpenTouryo.*`）
- プロジェクト・ポリシーは **リポジトリ ルートの `AGENTS.md`（`CLAUDE.md` はそれへのポインタ）** に定義済み。
  → **エージェントは git 操作（add/commit/push/checkout/branch/reset/restore/stash）を行わない。**
  作業結果はワーキング ツリーに残し、変更内容を報告するに留める。

---

## 2. ディレクトリとアセンブリの対応

### 2.1 `Infrastructure/`（フレームワーク本体）

| ディレクトリ | アセンブリ | ルート名前空間 | 役割 |
|---|---|---|---|
| `Public/` | `OpenTouryo.Public` | `Touryo.Infrastructure.Public` | 汎用ライブラリ。フレームワーク非依存で単体利用可 |
| `Public/Security/` | `OpenTouryo.Public.Security` | `...Public.Security` | 暗号・署名・JWT/JWS/JWE・鍵交換・パスワードハッシュ |
| `Public/Db/Dam*/` | `OpenTouryo.Dam*` | `...Public.Db` | DBMS 別データアクセス制御（Dam）の別アセンブリ |
| `Framework/` | `OpenTouryo.Framework` | `...Framework` | フレームワーク中核（P/B/D の親クラス、通信、認証、例外） |
| `Framework/RichClient/` | `OpenTouryo.Framework.RichClient` | `...Framework.RichClient` | WinForms/WPF 向け |
| `Business/` | `OpenTouryo.Business` | `...Business` | **業務フレームワーク＝テンプレート層**。アプリ側でカスタマイズ前提 |
| `Business/RichClient/` | `OpenTouryo.Business.RichClient` | `...Business.RichClient` | 同上（リッチクライアント） |
| `CustomControl/` | `OpenTouryo.CustomControl` | `...CustomControl` | ASP.NET Web Forms カスタム コントロール（net48 のみ） |
| `CustomControl/RichClient/` | `OpenTouryo.CustomControl.RichClient` | `...CustomControl.RichClient` | WinForms カスタム コントロール |
| `ServiceInterface/` | — | — | サービス公開ホスト（ASP.NET WebAPI / WCF-TCP）。net48 のみ |

ファイル数の目安: Public 188 / Framework 76 / CustomControl 50 / Business 42 / ServiceInterface 10。

### 2.2 `Tools/` — 開発支援 GUI ツール（WinForms）

- `DaoGen_Tool` : DB スキーマから D層 Dao クラス＋SQL(XML) を自動生成
- `DPQuery_Tool` : DPQ（動的パラメタライズド・クエリ）XML の編集・検証

### 2.3 `Tests/` — テスト（**xUnit/NUnit ではない**）

| 名前 | 内容 | 専用の文書 |
|---|---|---|
| `TestCode` | コンソール EXE。`Program.cs` から `Test*.Root()` を順次呼ぶ手動確認型。期待値は `Result48.txt` / `ResultCore100.txt` と比較。**DB には接続しない** | [`README.md`](Tests/TestCode/README.md) |
| `TestDataAccess` | データ アクセスのテスト（#520）。`TestCode` と同じ構成だが**DB に接続する**。`/MODE` で対象 DBMS を切り替える（既定は SQL Server のみ） | [`README.md`](Tests/TestDataAccess/README.md) |
| `TestLog` | ログ出力確認（log4net/NLog、1〜3） | — |
| `TestBatch` | バッチ起動確認 | — |
| `EncAndDecUtil` / `EncAndDecUtilCUI` | 暗号・署名ユーティリティの GUI/CUI 確認 | — |

**`TestCode` と `TestDataAccess` は専用の `README.md` を持つ。**
「何をテストしているか」「ケースを書き足すときの決まり」「ケース設計の根拠」は
そちらが一次情報。`TESTING.md` は 8 ケース全体の運用を扱い、この詳細は持たない。

**テストを書き足す前に `TestCode/README.md` を読むこと。**
結果ファイルとの比較で判定するため、**環境で変わる値を出してはならない**という制約がある
（例外はメッセージを出さず型名だけ、日時は固定値、`csproj` は net48 / core100 の両方を直す）。

**重要:** テスト プロジェクトはフレームワークを `ProjectReference` ではなく
**`HintPath` で `Infrastructure/Build_netcore100/net10.0/*.dll` を参照**する。
→ **フレームワークを先にビルドしないとテストは古い DLL を見る。**
また `net48` / `core100` の 2 プロジェクトが `..\*.cs` を `Link` で共有する（実体は 1 つ）。

---

## 3. アーキテクチャ：呼び出しフロー

```
[P層] 画面コード
   BaseController(Web Forms) / BaseMVController(MVC5) / BaseMVControllerCore(ASP.NET Core MVC)
      ↓ MyBase*Controller（Business層のテンプレート）を継承してアプリが実装
      ↓
[通信] CallController.Invoke(serviceName, parameterValue)
      ↓ ProtocolNameService でプロトコル解決（TMProtocolDefinition.xml）
      ↓ InProcessNameService で assembly/class 解決（TMInProcessDefinition.xml）
      ↓ Latebind.InvokeMethod → "DoBusinessLogic"
      ↓
[B層] BaseLogic.DoBusinessLogic(parameterValue, iso)
      ├ UOC_ConnectionOpen  … Dam 生成／Connection Open／BeginTransaction
      ├ UOC_PreAction       … ACCESS ログ等
      ├ UOC_DoAction        … ★業務処理。MyFcBaseLogic は "UOC_" + MethodName へ Latebind 振り分け
      ├ UOC_AfterAction
      ├ Commit（_dam と _dams[key] すべて）
      ├ UOC_AfterTransaction
      └ catch → Rollback → UOC_ABEND（例外種別ごとに 3 オーバーロード）
      finally → ConnectionClose
      ↓
[D層] BaseDao ← MyBaseDao ← CmnDao / アプリ個別 Dao
      ↓
[DAM] BaseDam ← DamSqlSvr / DamODBC / DamOLEDB / DamMySQL / DamPstGrS / DamManagedOdp / DamDB2 / DamHiRDB
```

### 3.1 引数・戻り値クラス

`BaseParameterValue`（`ScreenId` / `ControlId` / `MethodName` / `ActionType`、すべて読み取り専用）
→ `MyParameterValue`（+ `MyUserInfo User`）→ `_3TierParameterValue`（検索条件・ソート・ページング等）

`BaseReturnValue`（`ErrorFlag` / `ErrorMessageID` / `ErrorMessage` / `ErrorInfo`）
→ `MyReturnValue` → `_3TierReturnValue`

- `[Serializable]`。WS 越えのため。
- `ActionType` は `"SQL%common"` のように **`%` 区切りの複合文字列**。
  先頭要素が **データ プロバイダ選択キー**（`SQL`/`OLE`/`ODB`/`ODP`/`MCN`/`NPS`/`DB2`/`HIR`）として
  `MyBaseLogic.UOC_ConnectionOpen` / `MyFcBaseLogic.UOC_ConnectionOpen` で分岐する。

### 3.2 自動振り分け（重要なマジック）

`MyFcBaseLogic.UOC_DoAction` は `"UOC_" + parameterValue.MethodName` を **リフレクションで呼ぶ**。
戻り値は `ref` では戻らない（例外時に失われる）ため、**`this.ReturnValue` プロパティ経由で受け渡す**。

```csharp
private void UOC_SelectCount(TestParameterValue p)   // private でよい
{
    TestReturnValue r = new TestReturnValue();
    this.ReturnValue = r;      // ★必ず冒頭で設定する
    ...
}
```

`ReturnValue` の setter は `WasCalledFromDoBusinessLogic` フラグを見ており、
`DoBusinessLogic` 経由でない呼び出しでは `FrameworkException` を投げる。

### 3.3 例外モデル

| 例外 | 意味 | `DoBusinessLogic` の挙動 |
|---|---|---|
| `BusinessApplicationException` | 業務例外（想定内） | Rollback → `ErrorFlag=true` を戻り値に設定 → **リスローしない** |
| `BusinessSystemException` | 業務システム例外 | Rollback → `UOC_ABEND` → **リスロー** |
| `Exception`（その他） | 想定外 | Rollback → `UOC_ABEND(ref)` → **リスローしない**（UOC 側で必要なら throw） |
| `FrameworkException` | フレームワーク内部エラー | 上位へ |

すべて `messageID` プロパティを持つ（`Information` は `BusinessApplicationException` のみ）。

---

## 4. データアクセス（Open棟梁の中核的な差別化要素）

### 4.1 3 つのクエリ形態（`DbEnum.QueryStatusEnum`）

- **SPQ** 静的パラメタライズド・クエリ … `.sql` ファイル
- **DPQ** 動的パラメタライズド・クエリ … `.xml` ファイル。`<WHERE>` / `<IF>` / `<ELSE>` で
  **パラメタが設定されている条件行だけを動的に組み立てる**（SQL インジェクション耐性を保ったまま動的 WHERE を実現）

```xml
<ROOT>
  SELECT [ShipperID],[CompanyName] FROM [Shippers]
  <WHERE>
    WHERE
      <IF>AND [ShipperID] = @ShipperID<ELSE>AND [ShipperID] IS NULL</ELSE></IF>
      <IF>AND [CompanyName] LIKE @CompanyName_Like</IF>
  </WHERE>
</ROOT>
```

サンプルは `root/files/resource/Sql/`（`sqlserver` / `oracle` / `mysql` / `pstgrs` / `db2` / `hirdb` / `ole_odbc` 別サブディレクトリあり）。

### 4.2 Dam / Dao の責務

- `BaseDam`（3198 行、`Public/Db/BaseDam.cs`）… SQL ロード、パラメタ記号変換（`@`/`:`/`?`）、DPQ 展開、
  トランザクション制御、コマンド タイムアウトを担う。DBMS 差異はここに集約。
- `BaseDao` … `SetSqlByFile` / `SetSqlByCommand` / `SetParameter` / `ExecSelectFill_DT` /
  `ExecSelect_DR` / `ExecSelectScalar` / `ExecInsUpDel_NonQuery` を `protected` で提供。
  UOC は `UOC_PreQuery` / `UOC_AfterQuery(sql)` / `UOC_AfterQuery(sql, ex)`。
- `MyBaseDao` … `SetSqlByFile2`（`appSettings:sqlTextFilePath` を基点にパス解決。
  `MyBaseDao.UseEmbeddedResource = true` で埋め込みリソースへ切替）＋ SQLTRACE ログ・性能測定。
- `CmnDao` … Dao クラスを書かずに使える汎用 Dao。パラメタを `Dictionary` に溜め、実行直前に反映。
  `BaseDao` のメソッドを `new` で隠蔽して `public` 化している（**`override` ではない点に注意**）。
- `_3TierEngine`（1452 行）… `_3TierParameterValue` の検索条件辞書から SQL を生成する CRUD エンジン。
  `DaoGen_Tool` が生成するコードとセットで使う。

### 4.3 分離レベル

`DbEnum.IsolationLevelEnum`: `NotConnect`(接続しない) / `NoTransaction` / `ReadUncommitted` /
`ReadCommitted` / `RepeatableRead` / `Serializable` / `Snapshot` / `DefaultTransaction` / `User`。
`User` は「B層テンプレート側で決める」の意で、`MyBaseLogic` では `ReadCommitted` にフォールバックする。

---

## 5. 設定（XML 定義ファイル ＋ appSettings）

### 5.1 XML 定義ファイル（`root/files/resource/Xml/`）

| appSettings キー | ファイル | 用途 |
|---|---|---|
| `FxXMLMSGDefinition` | `MSGDefinition[_ja/_zh-CN].xml` | メッセージ ID → 文言（`GetMessage`） |
| `FxXMLSPDefinition` | `SPDefinition.xml` | 共有プロパティ（`GetSharedProperty`） |
| `FxXMLSCDefinition` | `SCDefinition.xml` | 画面遷移定義（`BaseController` の遷移チェック） |
| `FxXMLTCDefinition` | `TCDefinition.xml` | トランザクション定義（`TransactionControl`）。接続文字列キー＋分離レベル(`nc/nt/uc/rc/rr/sz/ss/df`) |
| `FxXMLTMInProcessDefinition` | `TMInProcessDefinition.xml` | インプロセス名前解決（論理名→assembly/class） |
| `FxXMLTMProtocolDefinition` | `TMProtocolDefinition.xml` | プロトコル／URL／タイムアウト／プロパティ |

読み込み順は **埋め込みリソース → 物理ファイル → 未設定なら空 XML（OFF 扱い）→ それ以外はエラー**。
パスには `%ENV%` 形式の環境変数を展開できる（`StringVariableOperator.BuiltStringIntoEnvironmentVariable`）。

### 5.2 主な appSettings キー（`Fx` プレフィクス）

- 画面制御: `FxSessionTimeOutCheck` `FxDoubleTransmissionCheck` `FxScreenTransitionMode`
  `FxScreenTransitionCheck` `FxErrorScreenPath` `FxOKMessageDialogPath` `FxYesNoMessageDialogPath`
  `FxDialogFramePath` `Fx*IconPath` `Fx*MaxQueueLength` `FxDefault*Style` `FxCacheControl`
- コントロール接頭辞（集約イベント ハンドラの識別に使う）: `FxPrefixOfButton` `FxPrefixOfTextBox` … 等 15 種
- DB: `FxSqlCacheSwitch` `FxSqlCommandTimeout` `FxSqlDotnetTypeInfo` `FxSqlEncoding` `FxSqlTraceLog`
- ログ: `FxLog4NetConfFile` / `LogLib`（`"nlog"` で NLog、既定は log4net）
- 国際化: `FxExceptionMessageCulture` `FxBusinessMessageCulture`
- 実行環境: `FxContainerization`（**true のとき環境変数を appSettings より優先**。JSON キーの `:` は `__` で表現）
- `_3TierEngine` 用: `MethodNameHeaderS/FooterS` `MethodNameHeaderD/FooterD`
  `MethodLabel_Ins/Sel/Upd/Del/SelCnt` `UpdateParamHeader/Footer` `LikeParamHeader/Footer`
- Dao: `sqlTextFilePath` / 接続文字列 `ConnectionString_SQL|OLE|ODBC|ODP|MCN|NPS|DB2|HIR`

### 5.3 設定の読み取り API

`Public/Util/GetConfigParameter.cs` に集約。
- net48: `System.Configuration`（`app.config` / `web.config`）
- .NET (Core): `Microsoft.Extensions.Configuration`。**使用前に `GetConfigParameter.InitConfiguration(...)` が必須**
  （`IConfiguration` / `IConfigurationBuilder` / JSON ファイル名 / 引数なし=`appsettings.json` の 4 オーバーロード）。
  JSON は `"appSettings": { ... }` セクション配下に置く慣習。

---

## 6. マルチ ターゲットと条件コンパイル（**最も事故りやすい箇所**）

### 6.1 ターゲット

| 系統 | csproj 命名 | TFM |
|---|---|---|
| .NET Framework | `*_net48.csproj` / `*_net48.sln` | `v4.8`（旧形式 csproj） |
| .NET | `*_netcore100.csproj` / `*_netcore100.sln` | `net10.0`（SDK 形式） |
| リッチクライアント (.NET) | 同上 | `net10.0-windows7.0` |

出力先は `Infrastructure/Build_net48/` と `Infrastructure/Build_netcore100/net10.0/`（`.gitignore` 対象）。

### 6.2 プリプロセッサ シンボル

| シンボル | 定義のされ方 | 意味 |
|---|---|---|
| `NETCOREAPP` | **.NET SDK が net10.0 に暗黙定義**（csproj の記述に依存しない） | 「.NET (Core) ビルド」＝実質「非 net48」 |
| `NETSTD` | `Framework` / `Public.Security` / `Dam*` の `DefineConstants` に**明示** | 歴史的経緯（下記） |
| `NET48` | net48 側 | .NET Framework 固有 |
| `NETCOREAPP2_0` | 旧コード内に残存 | 実質デッド分岐 |
| `PERFORMANCE_LOG_SWITCH` | 任意 | 性能ログ |

#### なぜ 2 つあるのか（歴史的経緯）

かつては **.NET Standard でビルドされるライブラリ**と **.NET Core でビルドされるライブラリ**が
併存しており、その区別が `NETSTD` / `NETCOREAPP` だった。
**現在は .NET Standard 版が全て .NET Core（net10.0）に統一された**ため、この区別は意味を失っている。

#### 現在の実際の成立状況（実測）

**`NETCOREAPP` は .NET SDK が net10.0 ターゲットに自動的に定義する暗黙シンボル**であり、
`<DefineConstants>` に書かれていなくても **全 netcore100 プロジェクトで真**になる
（同条件の最小プロジェクトを作って `#warning` で確認済み）。

| プロジェクト | `NETCOREAPP` | `NETSTD` |
|---|---|---|
| `Public` / `Business` / `Business.RichClient` / `CustomControl.RichClient` / `Framework.RichClient` | ✓（暗黙） | **✗** |
| `Framework` / `Public.Security` / `Dam*` | ✓（暗黙） | ✓（明示） |
| net48 の全プロジェクト | ✗ | ✗ |

したがって現状は次が成り立つ。

- `#if NETCOREAPP` … netcore100 ビルドで**常に真**。
- `#if (NETSTD || NETCOREAPP)` … `#if NETCOREAPP` と**完全に等価**。
- `#if NETSTD` … `NETSTD` を明示定義した 3 系統でのみ真。**それ以外では偽になる。**

#### 注意点

**`NETSTD` を明示していないプロジェクト（`Public` / `Business` / `*.RichClient`）の中で
素の `#if NETSTD` を書くと、黙って net48 側の分岐に落ちる。**
実測では該当箇所は 0 件（下表のとおり、書き分けは現状すべて正しい）だが、コード追加時の落とし穴になる。

| プロジェクト | `(NETSTD \|\| NETCOREAPP)` | `NETSTD` 単独 | `NETCOREAPP` 単独 |
|---|---:|---:|---:|
| `Public` | 36 | 0 | 2 |
| `Public.Security` | 0 | 83 | 0 |
| `Framework` | 21 | 8 | 1 |
| `Framework.RichClient` | 0 | 0 | 1 |
| `Business` | 0 | 0 | 6 |
| `Business.RichClient` | 0 | 0 | 4 |
| `CustomControl.RichClient` | 0 | 0 | 27 |

> **整理の余地**: 上記のとおり 3 つの書き方はすべて `NETCOREAPP`（＝非 net48）と等価に帰着する。
> `<DefineConstants>` から `NETSTD` を削除し、`#if NETSTD` / `#if (NETSTD || NETCOREAPP)` を
> `#if NETCOREAPP` に統一すれば、このシンボル体系は 1 本化できる（対象 約 150 箇所）。
> 機械的だが影響範囲が広いため、実施は別途判断。

### 6.3 csproj の `Compile Remove` によるファイル除外

SDK 形式 csproj はワイルドカード込みなので、**プラットフォーム非対応ファイルを `Compile Remove` で明示的に外している**。
新規ファイルを追加すると **net10.0 側に自動で含まれてしまう**ため、Windows 専用 API を使う場合は
`Compile Remove` の追加が必要。主な除外例:

- `Public_netcore100`: `Win32/**` `WinProc/**` `Security/**` `Db/Dam*/**`、
  `IO/BinarySerialize.cs` `IO/Zip*.cs` `Log/CustomEventLog.cs` `Log/SecurityEventLog.cs`
  `Db/DamOLEDB.cs` `Db/DamOraClient.cs` `Util/SharedMemory.cs`
- `Framework_netcore100`: `RichClient/**`、`Presentation/BaseController.cs`（Web Forms）
  `Presentation/BaseMasterController.cs` `Presentation/BaseMVController.cs` `Presentation/FxEventArgs.cs`
  `Transmission/IWCFTCPSvcForFx.cs` `Util/FxSessionUtil.cs` `Util/FxHttpQueryStringIndex.cs`
- `Business_netcore100`: `Csp/**` `RichClient/**`、
  **`Business/MyBaseLogic.cs` と `Business/_3TierEngine.cs` / `Common/_3Tier*Value.cs`**、
  `Presentation/MyBaseController.cs` `MyBaseMVController.cs` `MyBaseAsyncApiController.cs` 他
- `Public.Security_netcore100`: `IdentityImpersonation.cs` `KeyExg/EcdhCng*.cs`

→ **`_3TierEngine` と `MyBaseLogic` は net48 専用**。.NET (Core) 側で使うのは `MyFcBaseLogic` 系。

### 6.4 net48 専用の機能

- ASP.NET Web Forms 一式（`BaseController` 4836 行、`CustomControl/`）
- `CallController` のリモート プロトコル（`FxEnum.TmProtocol` は .NET (Core) では `InProcess` のみ）:
  ASP.NET WS / WCF-HTTP / WCF-TCP / ASP.NET WebAPI(JSON-RPC)
- `ServiceInterface/`（WCF ホスト、ASP.NET WebAPI ホスト）

---

## 7. ビルド

`root/programs/CS/` 直下の連番 `.bat` をダブルクリック実行する運用（`_Please run with a double-click...txt`）。

| bat | 内容 |
|---|---|
| `2_Build_NuGet_net48.bat` / `2_Build_NuGet_netcore100.bat` | `Nuget_*.sln` + `Nuget_RichClient_*.sln` をビルド（＝フレームワーク本体） |
| `3_Build_Business_*.bat` / `3_Build_BusinessRichClient_*.bat` | Business 層 |
| `4_Build_CopyAssemblies.bat` / `4_Build_Framework_Tool*.bat` | 成果物コピー・ツール |
| `5〜8_*` | サンプル各種 |
| `y_Build_TestCode*.bat` | テスト |
| `0_ExecAllBat.bat` | 全実行 |
| `z_Common.bat` / `z_Common2.bat` | ビルド ツールのパス解決（各 bat が冒頭で `call` する） |

.NET (Core) 側は `dotnet restore` → `dotnet msbuild` を呼ぶだけなので、
**エージェントは bat を経由せず直接 `dotnet build "Frameworks\Infrastructure\Nuget_netcore100.sln"` してよい。**
net48 側は MSBuild / devenv が必要（Windows + VS 前提）。

`dotnet build Nuget_netcore100.sln` は **0 エラー / 20 警告**。
警告はすべて NuGet 脆弱性警告（12 節）で、コンパイル警告ではない。

#### `z_Common.bat` と `z_Common2.bat`（MSBuild 版 / devenv 版）

両者は **`%BUILDFILEPATH%`（ビルド ツールのパス）と `%COMMANDLINE%`（ビルド引数）という
同じインターフェイスを提供する差し替え可能な対**であり、呼び出し側の bat は
`call` するファイルを変えるだけでビルド ツールを切り替えられる設計になっている。

| | ビルド ツール | `%COMMANDLINE%` |
|---|---|---|
| `z_Common.bat` | MSBuild.exe | `/p:Configuration=<構成> /p:DebugType=<型> -v:d` |
| `z_Common2.bat` | devenv.com / devenv.exe | `/build <構成>`（devenv 構文） |

`z_Common2.bat` は、**MSBuild.exe ではエラーになるが devenv.com では通る、というケースが
散在していた時期に、ビルド ツールを devenv へ差し替えるために用意されたもの**。
現在はどの bat からも呼ばれておらず、変数定義にも不具合が残っているため、そのままでは動作しない
（詳細はファイル冒頭のコメント参照）。**通常は `z_Common.bat` を使う。**

`z_Common.bat` は `vswhere` で MSBuild を解決し、見つからなければ明示エラーで停止する
（固定パス群はフォールバックとして存置）。エディションに依存しない。

### 7.1 ビルド順（厳守）

```
Public → Public.Security → Framework → Business → Tools / Tests / Samples
```

**`4_Build_CopyAssemblies.bat`（`Build_net48` → `Build\` の xcopy）を飛ばすと、
net48 のサンプルとツールが一切ビルドできない。**
`Frameworks\Infrastructure\Build\` を `HintPath` 参照するプロジェクトが CS/VB 合わせて **42 件**あるため
（`Samples/*` の全 net48 サンプル、`Tools/DaoGen_Tool`・`DPQuery_Tool`、`Tests/TestLog*`）。
`0_ExecAllBat.bat` が `2_`/`3_` → `4_CopyAssemblies` → `5_`〜`10_`（サンプル）の順で呼ぶのはこのため。

`Build\` に入るのは **net48 の成果物のみ**で、.NET (Core) 側に相当物は無い
（`Samples4NetCore` は `Build_netcore100\net10.0\` を直接参照する）。この非対称は設計どおり。

### 7.2 バージョン番号の一元管理

**`Infrastructure/Directory.Build.props` の `OpenTouryoVersion` が唯一の定義箇所。**

```xml
<PropertyGroup>
  <OpenTouryoVersion>3.0.0</OpenTouryoVersion>
  <Company>Hitachi Solutions</Company>
</PropertyGroup>
```

| 反映先 | 仕組み |
|---|---|
| SDK 形式アセンブリ（7 個） | 各 csproj の `<Version>$(OpenTouryoVersion)</Version>` |
| NuGet パッケージ | nuspec は `<version>$version$</version>` と `<dependency id="Touryo.Infrastructure.*" version="$version$" />`。`_NuGetPack.bat` が props から値を読み `nuget pack -Properties version=...` で渡す |

- **旧形式 csproj（`*_net48.csproj`）には効かない。** `Microsoft.Common.props` をインポートしないため。
  net48 のバージョンは各プロジェクトの `Properties\AssemblyInfo.cs` が持つ。
- **`Business` 系は 1.0.0 で別系統。** `Business/Properties/AssemblyInfo.cs` が `AssemblyVersion("1.0.0.0")` であり、
  Public / Framework / Public.Security の 3.0.0.0 とは意図的に分けている。
  そのため `Directory.Build.props` では `<Version>` を全体に設定せず、
  `OpenTouryoVersion` という独自プロパティをパッケージ対象の 7 プロジェクトだけが参照する形にしている。

> **注意**: `Directory.Build.props` の XML コメント内にハイフン 2 個の連続（`--`）を書くと
> XML として不正になり、MSBuild がプロジェクトの読み込みに失敗する。区切り線に使わないこと。

### 7.3 NuGet パッケージ化

`root/programs/CS/NuGet/`（`*.nuspec` + `_NuGetPack.bat`、`in/` に DLL を staging）。
手順は `NuGet/_手順の説明.txt` を参照。

- **nuspec の `<dependencies>` は csproj の `PackageReference` と一致している**（余分・不足・版ズレなし）。
  依存を増減したら nuspec 側も合わせること。
- パッケージ自身と `Touryo.Infrastructure.*` の相互依存のバージョンは `$version$` トークンで自動追随する（7.2 節）。
- `Symbol_Framework.RichClient.nuspec` のみ、`net10.0-windows7.0` ビルドを `lib\net10.0` に配置している（12 節）。

---

## 8. コーディング規約（既存コードに合わせること）

### 8.1 ファイル ヘッダ（**新規追加時も必須**。ただし新規と既存で書式が異なる）

#### 新規ファイルに付けるヘッダ（これが現行の書式）

```csharp
#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// ...（定型 15 行）
//
#endregion

//**********************************************************************************
//* クラス名        ：CallController
//* クラス日本語名  ：クライアント ライブラリ
//*
//* 作成者          ：xxx
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/07/31  xxx               新規作成
//**********************************************************************************
```

※ 更新者は ClaudeCode：玄人 幸道、GitHubCopilot：後輩 郎党 で。

> **`Copyright (C) ... Hitachi Solutions,Ltd.` のブロックは、新規ファイルには付けない。**
> 開発元が企業からコミュニティに移ったため。
> Apache License の region と、クラス名・日本語名・更新履歴のブロックは従来どおり必要。

#### 既存ファイルの場合

既存ファイルの先頭には次の Copyright ブロックが付いている。**これは削除せず、そのまま残す。**

```csharp
//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************
```

**既存ファイルを変更した場合は、更新履歴に 1 行追記するのがこのリポジトリの慣習。**

### 8.2 その他の規約

- **コメント・XML ドキュメントは日本語**。`<summary>` は全 public/protected メンバに付与
  （`DocumentationFile` を出力しているため、欠けると警告）。
- `#region` / `#endregion` による細かいブロック分割が徹底されている（`BaseController` は 100 以上）。
- `<remarks>自由に利用できる。</remarks>` … 業務コードから直接呼んでよい API の目印。
  `<remarks>業務コード親クラス１から利用される派生の末端</remarks>` … オーバーライド専用の目印。
- 拡張ポイントは **`UOC_` プレフィクス**（`FxLiteral.UOC_METHOD_HEADER`）。
  P層の集約イベント ハンドラも `UOC_<ControlId>_Click` のような命名規則でリフレクション解決される。
- 定数は `FxLiteral`（Framework, 777 行）/ `MyLiteral`（Business）/ `PubLiteral`（Public）に集約。
  **文字列リテラル直書きではなく、これらに定数を追加する。**
- 命名: `Base*`（Framework 提供の抽象）→ `My*`（Business 層テンプレート、アプリで改変前提）。
  アプリ側は `LayerB` / `LayerD` / `TestParameterValue` / `TestReturnValue` を実装（`Samples/` 参照）。
- 変数はプライベート フィールド `_xxx` ＋ 明示的プロパティ（自動プロパティは新しい箇所のみ）。

### 8.3 引数を検査する例外（`ArgumentException` だけ引数の順が違う）

**3 つとも「引数名」を渡すが、渡す位置が違う。**
`ArgumentException` **だけメッセージが先**で、ここが取り違えの温床になる。

| 例外 | 第 1 引数 | 第 2 引数 |
|---|---|---|
| `ArgumentNullException` | **引数名** | メッセージ |
| `ArgumentOutOfRangeException` | **引数名** | 実際の値（3 引数版）→ メッセージ |
| `ArgumentException` | **メッセージ** | **引数名** |

`ArgumentException("userId")` と書くと、**引数名ではなくメッセージが "userId" になる**。
引数名を伝えたいなら第 2 引数に置く。

```csharp
throw new ArgumentNullException(nameof(bytes));
throw new ArgumentOutOfRangeException("ecc", ecc, "Invalid");
throw new ArgumentException("Length is less than 128 bits.", "cek");   // 引数名は第 2
```

既存コードは 148 箇所すべてこの規約どおり
（`ArgumentException` は 139 箇所あり、大半はメッセージのみ。
メッセージは `PublicExceptionMessage` の定数を使う）。

**引数名は `nameof` で書く。** 引数名を変えたときに追随するため。
文字列リテラルだと乖離しても誰も気付かない。実際、`ArrayOperator.GetLongFromByte` は
分割時（2019/05/28）の旧名 `"bytData"` が残り、**存在しない引数名**を渡していた
（2026/08/06 に修正、#522）。

> **既存箇所の一括置換はしない。** 下位互換の維持が最優先のため
> （[`Contributing.ja.md`](../../../Contributing.ja.md)）、新規・修正箇所から適用する。

### 8.4 bat ファイルの文字コード

**非 ASCII 文字（日本語）を含む `.bat` は UTF-8 BOM 付きにする。**

BOM が無いと、cmd.exe がバッチをバイト オフセットで読み進める際に文字境界がずれ、
**`@rem` コメントの途中から先がコマンドとして実行される**ことがある
（`'xxx' は、内部コマンドまたは外部コマンド…として認識されていません` が出る）。

| BOM | 起動 CP=932 | 起動 CP=65001 |
|---|---|---|
| なし | エラーなし | **エラーあり（間欠）** |
| **あり** | **エラーなし** | **エラーなし** |

- 実害は「紛らわしいエラー表示」に留まり、**後続の実コマンドは飛ばない**。
- `chcp 65001` は**画面出力の文字化け対策**であって、この解析ずれの対策ではない。
  日本語を `echo` する場合に併用する。
- 純粋に ASCII のみの bat に BOM は不要（差分ノイズになるだけ）。
  **日本語を書き足すときに BOM の有無を確認すること。**

`root/programs/CS/` 配下で非 ASCII を含む bat は、すべて BOM 付きになっている。

### 8.5 ps1 ファイルの文字コードと、PowerShell 5.1 / 7 の両対応

**`.ps1` は Windows PowerShell 5.1 と PowerShell 7 の両方で動くこと。**

開発時は `pwsh`（7）で確認しがちだが、利用者は `powershell.exe`（5.1）で実行する。
7 だけで確認すると 5.1 で落ちる。既踏の落とし穴は次の 4 点。

| 事象 | 原因 | 対処 |
|---|---|---|
| 構文エラー・文字化け（`繧ｹ繝・ャ繝・`） | 5.1 は BOM 無しの `.ps1` を **ANSI（Shift_JIS）**として読む | **UTF-8 BOM 付き**で保存する |
| 同じファイルなのに差分が出る | `Get-Content` の既定エンコードが 5.1 は ANSI、7 は UTF-8 | **`-Encoding UTF8`** を明示する |
| HTTP が常に失敗（状態コード `-1`） | `-SkipHttpErrorCheck` は **7 以降にしかない** | バージョンを見て付け外しする |
| 実行中に画面がクリアされ、それまでの結果が消える | 子プロセスの `chcp 65001` はコンソール全体に影響する | スクリプト冒頭で先に切り替える |

```powershell
# 7 専用の引数は、バージョンを見て付け外しする
if ($PSVersionTable.PSVersion.Major -ge 6) { $p.SkipHttpErrorCheck = $true }

# コンソールのコード ページと、PowerShell の出力エンコードは別物。判定を分けること
if ((cmd /c chcp) -notmatch '65001') { cmd /c chcp 65001 | Out-Null }
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}
```

- **`.bat` は「非 ASCII を含むときだけ」BOM 付き**（8.4）だが、
  **`.ps1` は非 ASCII を含むなら必ず BOM 付き**。5.1 が既定で ANSI として読むため。
- 5.1 の `[Console]::OutputEncoding` は**起動時の値のまま**で、実行中にコード ページが
  変わっても追随しない。同じ画面で 2 回目を実行したときに化ける原因になる。
- **変更したら 5.1 でも実行して確かめること。**

```powershell
powershell.exe -NoProfile -Command "Set-Location 'root\programs'; .\3_SmokeTest.ps1"
```

検証スクリプト側での具体的な適用例は
[`SMOKETEST.md`](../../SMOKETEST.md) 「PowerShell 5.1 と 7 の両対応」を参照。

---

## 9. Public 層の主なユーティリティ（再実装しないこと）

| 名前空間 | 主なクラス |
|---|---|
| `Public.Db` | `BaseDam` `SQLUtility` `DbEnum` `Dam*` |
| `Public.Dto` | `DTTable/DTRow/DTColumn`（マーシャリング可能な自前 DataTable）、`DataToPoco` `PocoToPoco` `DataToDictionary` |
| `Public.Str` | `CustomEncode`(Base64/Hex/URL) `StringChecker` `FormatChecker` `StringConverter` `FormatConverter` `CheckCharCode` `JIS2k4Checker` |
| `Public.Security` | `SymmetricCryptography` `ASymmetricCryptography` `GetHash` `GetKeyedHash` `MsgAuthCode` `DigitalSign*` `PrivacyEnhancedMail`、`Jwt/`（JWS RS/ES/HS 256-512、JWE RSA1_5+A128CBC-HS256 / RSA-OAEP+AES-GCM、`JwkSet`、鍵コンバータ）、`Aead/`、`KeyExg/`（ECDH/RSA）、`Pwd/GetPasswordHashV1/V2` |
| `Public.Log` | `LogIF`（静的 façade）。ロガー名は慣習的に `"ACCESS"` と `"SQLTRACE"`。バックエンドは `LogLib` 設定で log4net / NLog |
| `Public.Reflection` | `Latebind`（フレームワークの動的呼び出しの心臓部）、`MyAssemblies` |
| `Public.FastReflection` | `AccessorCacher` `CompiledExpressionCreater` `InstanceCreator<T>` `EnumToString*Extensions` |
| `Public.IO` | `ResourceLoader` `EmbeddedResourceLoader` `DeflateCompression` `ExponentialBackoff` `Zipper/UnZipper`(net48) |
| `Public.Util` | `GetConfigParameter` `PerformanceRecorder` `RandomValueGenerator` `EnvInfo` `PubCmnFunction` |
| `Public.Diagnostics` | `MyDebug`（`OutputDebugAndConsole`）`ObjectInspector` `StackFrameOperator` |
| `Public.Win32` / `WinProc` | P/Invoke 群（net48 のみ） |

---

## 10. 認証まわり（`Framework/Authentication/`）

比較的新しく、直近も更新されている領域（`fixed #503` / `#504` は `SAML2Client.cs`）。

- `OAuth2AndOIDCClient` … Authorization Code / PKCE(S256) / Client Credentials / ROPC /
  Refresh / UserInfo / Revoke / Introspect / JWT Bearer / Device AuthZ / CIBA / Request Object / JWK Set。
  **すべて `static async Task<string>`（生 JSON を返す）**。`HttpClient` は `static` プロパティで差し替え可能。
- `OAuth2AndOIDCConst` / `OAuth2AndOIDCEnum` / `OAuth2AndOIDCParams` / `CmnClientParams`
- トークン型: `CmnJwtToken` `AccessToken` `IdToken` `JwtAssertion` `RequestObject` `ResponseObject` `ClaimsInRO`
- `JwkSetStore` … JWK Set のキャッシュ
- SAML2: `SAML2Client`（`CreateRedirectRequest` / `CreatePostRequest` / `VerifyResponse`）、
  `SAML2Bindings` `SAML2Const` `SAML2Enum` `SAML2Params`
- 依存: `jose-jwt` 5.1.1 / `BouncyCastle.NetCore` 2.2.1（`Public.Security`）

---

## 11. 主要な依存パッケージ（netcore100）

| プロジェクト | パッケージ |
|---|---|
| Public | `log4net` 3.3.0, `NLog` 5.5.0, `Microsoft.Data.SqlClient` 6.0.1, `System.Data.Odbc` 9.0.4, `Newtonsoft.Json` 13.0.3, `Microsoft.Extensions.Configuration*` 9.0.4, `Zipangu` 1.1.8 |
| Public.Security | `jose-jwt` 5.1.1, `BouncyCastle.NetCore` 2.2.1, `System.Security.Cryptography.Xml` 9.0.15, `Newtonsoft.Json` 13.0.3 |
| Framework | `Microsoft.AspNetCore.{Http,Http.Extensions,Mvc,Session}` 2.3.0, `Microsoft.Extensions.PlatformAbstractions` 1.1.0 |
| Business | `Microsoft.AspNetCore.Authentication{,.Cookies}` 2.3.0, `Microsoft.AspNetCore.{Http,Mvc}` 2.3.0, `System.Security.Cryptography.Xml` 9.0.4, `Newtonsoft.Json` 13.0.3 ほか |

**注意 1:** `Microsoft.AspNetCore.*` は **2.3.0（互換シム パッケージ）** を使い続けている。
net10.0 のフレームワーク参照（`Microsoft.AspNetCore.App`）ではない。安易に上げると壊れる可能性が高い。

**注意 2:** `System.Security.Cryptography.Xml` は **`Public.Security` が 9.0.15、`Business` が 9.0.4** と
版がずれている（Dependabot が 1 プロジェクトずつしか上げないため）。どちらも NU1903 が出る。
**修正版は 9.0.18**（12 節に advisory と該当 4 箇所を挙げてある）。

依存は Dependabot で随時更新される。**上表は目安であり、正確な値は csproj を直接見ること。**

---

## 12. 落とし穴 / 既知の不整合（作業前に把握しておく）

1. **`4_Build_CopyAssemblies.bat` を飛ばすと net48 のサンプル・ツールが全滅する**（7.1 節）。
2. **`Compile Remove` を忘れると .NET (Core) ビルドが壊れる**（6.3 節）。
3. **`CmnDao` は `new` による隠蔽**であって override ではない。`BaseDao` 型変数経由で呼ぶと親の実装が走る。
4. **`ReturnValue` を設定し忘れると戻り値が null** になる（3.2 節）。例外時も同様。
5. **`_dams` ディクショナリと `_dam` の二重管理**。Commit/Rollback/Close は両方に対して行われる。
   複数 DB を使う場合は `SetDam(key, dam)` を使い、`GetDam(key)` で取り出す。
6. **`BaseController.cs`（4836 行）と `BaseDam.cs`（3198 行）は巨大**。
   変更時は該当 `#region` に閉じた修正に留め、全体リファクタは避ける（互換性維持が最優先の設計）。
7. **VB 版ミラーが存在**: `root/programs/VB/Frameworks/Infrastructure/` に
   `Business` / `CustomControl` / `ServiceInterface` の VB.NET 版がある（`Public` / `Framework` は C# 版を共有）。
   Business 層テンプレートの仕様を変えると **VB 側も追随が必要**。
8. **作業ツリーに未追跡の生成物が残る**。`Build*/` と `ServiceInterface/*/dll/` は `.gitignore` 済み。
   残るのは `*.cer`（テスト生成物）と `SAML2Client.cs.bak`（残骸）で、いずれもコミット対象ではない。
9. `Public/Security/MyDebug.cs` は `Public/Diagnostics/MyDebug.cs` の**派生クラス**（重複ではない。
   アセンブリ分割の都合）。
10. `TMProtocolDefinition2.xml` など「2」付きの定義ファイルが並存する。用途は用例違い。
11. **`System.Security.Cryptography.Xml` に既知脆弱性（NU1903 / 高）**。
    `1_BuildAll.ps1` の全体で **84 警告**（`Public.Security` の 9.0.15 が 60、`Business` の 9.0.4 が 24）。

    **修正版は 9.0.18。** 2 件の advisory が出ており、9.0.15 では**片方しか直っていない**。

    | Advisory | 影響（9 系） | 修正版 |
    |---|---|---|
    | `GHSA-23rf-6693-g89p`（CVE-2026-50648） | `>= 9.0.0, <= 9.0.17` | **9.0.18** |
    | `GHSA-37gx-xxp4-5rgx`（CVE-2026-33116） | `>= 9.0.0, <= 9.0.14` | 9.0.15 |

    版を上げる場合は **net48 / netcore100 の両 csproj と `NuGet/*.nuspec` の
    `<dependencies>` を同時に直す。** 現状ずれているのは次の 4 箇所。

    ```
    Public/Security/Public.Security_netcore100.csproj   9.0.15
    Business/Business_netcore100.csproj                 9.0.4
    Tests/EncAndDecUtilCUI/core100/*.csproj             9.0.6   ← Dependabot PR #510 が 9.0.18 へ
    NuGet/Symbol_Public.Security.nuspec                 9.0.15
    ```

    **Dependabot は 1 プロジェクトずつしか上げない。** #510 はテスト プロジェクトのみで、
    取り込んでも 84 警告は減らない。**残りは手で揃える**こと
    （手順は [`BUILDING.md`](../../BUILDING.md) 9 節「複数の PR を `deps` に溜める」）。
12. **結果ファイル比較のテストは、検算しないと不具合を「正」として固定する。**
    `ArrayOperator.GetLongFromByte` は桁の重みを `256 * j`（掛け算）で求めており、
    **3 byte 目から誤った値**を返していた（2026/08/06 に修正、#522）。
    1〜2 byte は `256 * 1 == 256 ^ 1` でたまたま一致するため、呼び出し元
    （`CheckCharCode` → Shift_JIS の 1〜2 byte）では顕在化していなかった。
    **期待値を作るときは、出力をそのまま貼らず実装から検算し、境界を跨いだケースを置く**
    （[`Tests/TestCode/README.md`](Tests/TestCode/README.md)）。
13. **`Symbol_Framework.RichClient.nuspec` の lib TFM が不正確**。
    `net10.0-windows7.0` ビルドを `lib\net10.0` に配置している。本来は `lib\net10.0-windows` が正しく、
    現状は Linux 上の net10.0 消費者も解決してしまう。直すとパッケージ解決セマンティクスが変わるため据え置き。

---

## 13. サンプル（フレームワークの正しい使い方の参照先）

> **詳細な分析は各ディレクトリの ANALYSIS.md を参照:**
> - `../Samples/ANALYSIS.md`（net48 版サンプル）
> - `../Samples4NetCore/ANALYSIS.md`（.NET 10 版サンプル）

| パス | 内容 |
|---|---|
| `Samples/WebApp_sample/` | ASP.NET Web Forms / MVC5（net48） |
| `Samples/2CS_sample/` | 2層 C/S（WinForms / WPF） |
| `Samples/Bat_sample/` | バッチ（`SimpleBatch` / `RerunnableBatch` 1〜3） |
| `Samples/CLI_sample/` `Samples/WS_sample/` | CLI / Web サービス |
| `Samples4NetCore/Backend/MVC_Sample` `ASPNETWebService` | .NET (Core) 版 |
| `Samples4NetCore/Legacy/` | 上記の .NET (Core) 移植版 |

典型的な B層実装は `Samples/Bat_sample/SimpleBatch_sample/Business/LayerB.cs`（`MyFcBaseLogic` 継承）。
「テンプレ」`#region` に `UOC_メソッド名` という雛形がそのまま残っているのが本フレームワークの流儀。

---

## 14. エージェント向け作業チェックリスト

- [ ] `AGENTS.md` のポリシー遵守（**git 操作をしない**）
- [ ] 変更対象が net48 / netcore100 / 両方のどれか判定（`Compile Remove` を確認）
- [ ] 条件コンパイルは **`#if NETCOREAPP`（＝非 net48）を使う**。
      `#if NETSTD` は `Framework` / `Public.Security` / `Dam*` でしか真にならないので新規には使わない（6.2 節）
- [ ] 新規 .cs にはヘッダ コメント（Apache License / クラス名・日本語名・更新履歴）を付与。
      **Copyright ブロックは新規には付けない**（8.1 節）
- [ ] 既存 .cs 変更時は更新履歴に 1 行追記
- [ ] public/protected メンバに日本語 `<summary>` を付与（DocumentationFile 出力のため）
- [ ] 文字列リテラルは `FxLiteral` / `MyLiteral` / `PubLiteral` に定数として追加
- [ ] ビルド確認: `dotnet build Frameworks/Infrastructure/Nuget_netcore100.sln`
      （net48 は MSBuild 必須）
- [ ] テスト実行前に **フレームワークを先にビルド**（`Build_netcore100/net10.0/*.dll` を HintPath 参照するため）
- [ ] Business 層テンプレートを変えたら VB 版ミラーの追随要否を報告
