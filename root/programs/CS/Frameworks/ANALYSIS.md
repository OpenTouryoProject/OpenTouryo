# ANALYSIS.md — Open棟梁 フレームワーク本体（CS/Frameworks）コード分析

対象: `root/programs/CS/Frameworks` / ブランチ: `develop`
最終更新: 2026-10-17

本書は **コーディング・エージェントが本リポジトリで作業する際の Context** を目的とした分析結果である。
「どこに何があるか」「どの規約に従うべきか」「何を壊しやすいか」を記す。

**本書は俯瞰である。** アセンブリごとの詳細は、下の案内表の文書が一次情報を持つ（#613）。
本書には、**領域をまたいで効くこと**（呼び出しの流れ、例外モデル、条件コンパイル、ビルド順、バージョン管理、横断の落とし穴）を残す。

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

### 下位の文書（案内表）

**作業対象のディレクトリに近い文書から読む。** 各文書の末尾に「落とし穴」と「作業チェックリスト」がある。

| ディレクトリ | 文書 | 扱うこと |
|---|---|---|
| `Infrastructure/` | [`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) | **部品とテンプレートの区別**、アセンブリの依存の向き、ソリューションと参照の方式、TFM と出力先、ビルド対象外のファイル、`ServiceInterface`、VB 版 |
| `Infrastructure/Public/` | [`Public/ANALYSIS.md`](Infrastructure/Public/ANALYSIS.md) | 汎用ライブラリ（Db / Dto / Str / IO / Log / Util / Reflection ほか）と `Dam*` |
| `Infrastructure/Public/Security/` | [`Public/Security/ANALYSIS.md`](Infrastructure/Public/Security/ANALYSIS.md) | 暗号・署名・JWT・鍵交換・パスワード ハッシュ |
| `Infrastructure/Framework/` | [`Framework/ANALYSIS.md`](Infrastructure/Framework/ANALYSIS.md) | P/B/D の親クラス、通信制御、例外、**認証（OAuth2 / OIDC / SAML2）**、`Framework.RichClient` |
| `Infrastructure/Business/` | [`Business/ANALYSIS.md`](Infrastructure/Business/ANALYSIS.md) | 業務フレームワーク（サーバ側のテンプレート）。データ プロバイダの選択、`MyBaseDao` / `CmnDao`、`_3TierEngine` |
| `Infrastructure/Business/RichClient/` | [`Business/RichClient/ANALYSIS.md`](Infrastructure/Business/RichClient/ANALYSIS.md) | 同上（WinForms / WPF） |
| `Infrastructure/CustomControl/` | [`CustomControl/ANALYSIS.md`](Infrastructure/CustomControl/ANALYSIS.md) | ASP.NET Web Forms のカスタム コントロール |
| `Infrastructure/CustomControl/RichClient/` | [`CustomControl/RichClient/ANALYSIS.md`](Infrastructure/CustomControl/RichClient/ANALYSIS.md) | WinForms のカスタム コントロール |
| `Tools/` | [`Tools/ANALYSIS.md`](Tools/ANALYSIS.md) | 開発支援ツール（墨壺・DPQuery・ZIP の HTTP 配布） |
| `Tests/` | [`Tests/README.md`](Tests/README.md) | テストの区分（`Golden` / `NUnit` / `Else`）と置き場の決め方 |

---

## 2. ディレクトリとアセンブリの対応

### 2.1 `Infrastructure/`（フレームワーク本体）

| ディレクトリ | アセンブリ | 種類 | 役割 |
|---|---|---|---|
| `Public/` | `OpenTouryo.Public` | 部品 | 汎用ライブラリ。フレームワーク非依存で単体利用可 |
| `Public/Security/` | `OpenTouryo.Public.Security` | 部品 | 暗号・署名・JWT/JWS/JWE・鍵交換・パスワードハッシュ |
| `Public/Db/Dam*/` | `OpenTouryo.Dam*` | 部品 | DBMS 別データアクセス制御（Dam）の別アセンブリ |
| `Framework/` | `OpenTouryo.Framework` | 部品 | フレームワーク中核（P/B/D の親クラス、通信、認証、例外） |
| `Framework/RichClient/` | `OpenTouryo.Framework.RichClient` | 部品 | WinForms/WPF 向け |
| `Business/` | `OpenTouryo.Business` | **テンプレート** | **業務フレームワーク**。アプリ側でカスタマイズ前提 |
| `Business/RichClient/` | `OpenTouryo.Business.RichClient` | テンプレート | 同上（リッチクライアント） |
| `CustomControl/` | `OpenTouryo.CustomControl` | テンプレート | ASP.NET Web Forms カスタム コントロール（net48 のみ） |
| `CustomControl/RichClient/` | `OpenTouryo.CustomControl.RichClient` | テンプレート | WinForms カスタム コントロール |
| `ServiceInterface/` | — | ホスト | サービス公開ホスト（ASP.NET WebAPI / WCF-TCP）。net48 のみ |

**部品とテンプレートで、変更の影響の読み方が違う**（部品は公開 API の互換、テンプレートは VB 版の追随）。
依存の向き・参照の方式・出力先は [`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) 3〜5 節。

### 2.2 `Tools/` — 開発支援ツール（WinForms）

`DaoGen_Tool`（墨壺。D 層の自動生成）/ `DPQuery_Tool`（DPQ の編集・検証）/ `DeployZipPackWithHTTP`（ZIP の HTTP 配布）。
構成・検証・落とし穴は [`Tools/ANALYSIS.md`](Tools/ANALYSIS.md)、CLI での使い方は各ツールの `README.md`。

### 2.3 `Tests/` — テスト（**`Golden` / `Else` は xUnit/NUnit ではない**）

**判定の方式でフォルダを分けている**（#610）。区分ごとの中身と、置き場の決め方は
[`Tests/README.md`](Tests/README.md) が一次情報。運用（何をいつ回すか）は [`TESTING.md`](../../TESTING.md)。

**テスト プロジェクトはフレームワークを `ProjectReference` ではなく `HintPath` で参照する。**
→ **フレームワークを先にビルドしないとテストは古い DLL を見る。**

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
[DAM] BaseDam ← DamSqlSvr / DamODBC / DamOLEDB / DamMySQL / DamPstGrS / DamManagedOdp
```

各層の中身: 通信は [`Framework/ANALYSIS.md`](Infrastructure/Framework/ANALYSIS.md) 3 節、
`UOC_ConnectionOpen`（データ プロバイダの選択）は [`Business/ANALYSIS.md`](Infrastructure/Business/ANALYSIS.md) 3.1 節、
Dam は [`Public/ANALYSIS.md`](Infrastructure/Public/ANALYSIS.md) 4 節。

### 3.1 引数・戻り値クラス

`BaseParameterValue`（`ScreenId` / `ControlId` / `MethodName` / `ActionType`、すべて読み取り専用）
→ `MyParameterValue`（+ `MyUserInfo User`）→ `_3TierParameterValue`（検索条件・ソート・ページング等）

`BaseReturnValue`（`ErrorFlag` / `ErrorMessageID` / `ErrorMessage` / `ErrorInfo`）
→ `MyReturnValue` → `_3TierReturnValue`

- `[Serializable]`。WS 越えのため。
- `ActionType` は `"SQL%common"` のように **`%` 区切りの複合文字列**。
  **先頭要素だけがフレームワーク（`Business` のテンプレート）の解釈するデータ プロバイダ選択キー**で、
  `UOC_ConnectionOpen` で分岐する。使えるキーと、**どれにも当たらなければ黙って SQL Server になる**ことは
  [`Business/ANALYSIS.md`](Infrastructure/Business/ANALYSIS.md) 3.1 節。2 番目以降はアプリ（サンプル）側の約束である。

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

| 層 | クラス | 所管の文書 |
|---|---|---|
| Dam（DBMS 差異の吸収） | `BaseDam` と `Dam*` | [`Public/ANALYSIS.md`](Infrastructure/Public/ANALYSIS.md) 4 節 |
| Dao の親 | `BaseDao` | [`Framework/ANALYSIS.md`](Infrastructure/Framework/ANALYSIS.md) 6 節 |
| Dao のテンプレート | `MyBaseDao`（`SetSqlByFile2`・SQLTRACE）/ `CmnDao`（汎用 Dao）/ `_3TierEngine` | [`Business/ANALYSIS.md`](Infrastructure/Business/ANALYSIS.md) 3.3・4 節 |

- `CmnDao` は `BaseDao` のメソッドを **`new` で隠蔽**して `public` 化している（**`override` ではない**。12 節 3）。
- `_3TierEngine` は `DaoGen_Tool` が生成するコードとセットで使う。**net48 専用**（6.3 節）。

### 4.3 分離レベル

`DbEnum.IsolationLevelEnum`: `NotConnect`(接続しない) / `NoTransaction` / `ReadUncommitted` /
`ReadCommitted` / `RepeatableRead` / `Serializable` / `Snapshot` / `DefaultTransaction` / `User`。
`User` は「B層テンプレート側で決める」の意で、`MyBaseLogic` では `ReadCommitted` にフォールバックする。

---

### 4.4 **非同期の実体（`~Async` は「別スレッドで同期実行」）**（#402）

**D層に非同期の API は無い。** `BaseDam` は `ExecuteReader` / `ExecuteScalar` /
`ExecuteNonQuery` の同期版だけを持つ。

上位の `~Async` も、**別スレッドで同期版を走らせているだけ**である。

```csharp
// CallController.cs / BaseLogic.cs
return Task<...>.Factory.StartNew(() => {
    return this.Invoke(...);           // 同期版
});
```

**これはブロックの移動であって、除去ではない。**
呼び出し元のスレッドは解放されるが、**プールのスレッドが 1 本、DB 応答の間ずっと塞がる。**

#### **非同期化しない、と判断している**（#402）

業務系の DBMS アプリケーションでは、**非同期のサポートは重要ではない**と判断した。

**1 要求が 1 コネクションを保持する設計だからである。**

```
UOC_ConnectionOpen で Dam を作り、コネクションとトランザクションを開く
  → B層の処理がすべて終わるまで保持 → Commit / Rollback
```

**同時実行数は、スレッド数ではなくコネクション プールの上限で決まる。**
スレッドを解放できても、プールが空かなければ次には進めない。
非同期化で増えるのは「**待てる要求の数**」であって、
「**同時に処理できる要求の数**」ではない。

| 論点 | 既存の機構 |
|---|---|
| 同時実行数の制御 | コネクション プール（上限） |
| 応答しない処理の打ち切り | `FxSqlCommandTimeout`（既定 30 秒） |

**非同期化でしか得られないのは「実行中の中断」だけ**だが、
業務システムでは利用者が処理を中断する場面が限られ、多くは `CommandTimeout` で足りる。

#### **プロバイダによって非同期の実体が違う**

**足すとしても、一様には効かない。**
`DbCommand` の `~Async` は、プロバイダが上書きしなければ
**基底クラスの既定実装＝呼び出し元スレッドで同期実行**になる。
**`await` してもそこで止まる**ため、今より悪くなり得る。

| Dam | ライブラリ | 非同期の実体 |
|---|---|---|
| `DamSqlSvr` | `Microsoft.Data.SqlClient` | 真の非同期 |
| `DamPstGrS` | `Npgsql` | 真の非同期 |
| `DamMySQL` | `MySql.Data` | **要検証** |
| `DamManagedOdp` | `Oracle.ManagedDataAccess` | **要検証** |
| `DamODBC` / `DamOLEDB` | 標準 | **効果なし**（既定実装） |

> **`~Async` という名前は「非同期である」と読まれる。**
> 効かないプロバイダに足すと、名前だけが非同期になり、実態を隠す。

---

## 5. 設定（XML 定義ファイル ＋ appSettings）

**設定ファイルの仕組み（種類・読み込み順・環境変数での上書き・`%変数%` の展開・パスの落とし穴）は
[`CONFIGURATION.md`](../../CONFIGURATION.md) が一次情報。** 本節は、フレームワークが読む定義ファイルとキーの俯瞰に留める。

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
パスには `%ENV%` 形式の環境変数を展開できる（[`CONFIGURATION.md`](../../CONFIGURATION.md) 5 節）。

### 5.2 主な appSettings キー（`Fx` プレフィクス）

| 分類 | 代表的なキー | 読む側 |
|---|---|---|
| 画面制御（Web Forms） | `FxSessionTimeOutCheck` `FxDoubleTransmissionCheck` `FxScreenTransitionMode` `FxErrorScreenPath` `FxPrefixOf*` | `Framework` の P 層 |
| DB | `FxSqlCacheSwitch` `FxSqlCommandTimeout` `FxSqlTraceLog` / `sqlTextFilePath` / `ConnectionString_*` | `Public/Db`・`MyBaseDao` |
| ログ | `FxLog4NetConfFile` / `LogLib`（`"nlog"` で NLog、既定は log4net） | `Public/Log` |
| 国際化 | `FxExceptionMessageCulture` `FxBusinessMessageCulture` | `Framework` |
| 実行環境 | `FxContainerization`（**true のとき環境変数を appSettings より優先**） | `GetConfigParameter`（[`CONFIGURATION.md`](../../CONFIGURATION.md) 4-2） |
| `_3TierEngine` | `MethodNameHeaderS/FooterS` `MethodLabel_*` `UpdateParamHeader/Footer` `LikeParamHeader/Footer` | `Business`（net48） |

**キーの全量は各サンプルの `app.config` / `appsettings.json` を見る。** 本表は目安である。

### 5.3 設定の読み取り API

`Public/Util/GetConfigParameter.cs` に集約。**.NET では使用前に `GetConfigParameter.InitConfiguration(...)` が必須**
（呼ばないと `NOT_INITIALIZED` の `ArgumentException`。例外を握る API では黙って false になる）。オーバーロードと、初期化前に読みに来る部品は
[`Public/ANALYSIS.md`](Infrastructure/Public/ANALYSIS.md) 6.1 節。

---

## 6. マルチ ターゲットと条件コンパイル（**最も事故りやすい箇所**）

### 6.1 ターゲット

| 系統 | csproj 命名 | TFM |
|---|---|---|
| .NET Framework | `*_net48.csproj` / `*_net48.sln` | `v4.8`（旧形式 csproj） |
| .NET | `*_netcore100.csproj` / `*_netcore100.sln` | `net10.0`（SDK 形式） |
| リッチクライアント (.NET) | 同上 | `net10.0-windows7.0` |

出力先は `Infrastructure/Build_net48/` と `Infrastructure/Build_netcore100/net10.0/`（`.gitignore` 対象。
詳細は [`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) 5 節）。

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
`Compile Remove` の追加が必要。逆に旧形式（net48）は列挙なので、**書き足さないと入らない。**

**外しているファイルの一覧は、各下位文書の「TFM 差」の節にある**
（[`Public`](Infrastructure/Public/ANALYSIS.md) 3 節 / [`Public.Security`](Infrastructure/Public/Security/ANALYSIS.md) 3 節 /
[`Framework`](Infrastructure/Framework/ANALYSIS.md) 10 節 / [`Business`](Infrastructure/Business/ANALYSIS.md) 6 節）。

→ **`_3TierEngine` と `MyBaseLogic` は net48 専用**。.NET (Core) 側で使うのは `MyFcBaseLogic` 系。

### 6.4 net48 専用の機能

- ASP.NET Web Forms 一式（`BaseController`、`CustomControl/`）
- `CallController` のリモート プロトコル（**`FxEnum.TmProtocol` は .NET (Core) では `InProcess` のみ**）:
  ASP.NET WS / WCF-HTTP / WCF-TCP / ASP.NET WebAPI(JSON-RPC)
  （[`Framework/ANALYSIS.md`](Infrastructure/Framework/ANALYSIS.md) 3.2 節）
- `ServiceInterface/`（WCF ホスト、ASP.NET WebAPI ホスト。[`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) 7 節）

---

## 7. ビルド

**検証としてのビルドの手順（`1_BuildAll.ps1`、判定基準、既知のエラー）は [`BUILDING.md`](../../BUILDING.md) が一次情報。**
本節は、その下にある bat の構成と、ビルド順・バージョン管理の仕組みを扱う。

`root/programs/CS/` 直下の連番 `.bat` をダブルクリック実行する運用（`_Please run with a double-click...txt`）。

| bat | 内容 |
|---|---|
| `1_DeleteDir.bat` / `1_DeleteFile.bat` | 生成物の削除 |
| `2_Build_NuGet_net48.bat` / `2_Build_NuGet_netcore100.bat` | `Nuget_*.sln` + `Nuget_RichClient_*.sln` をビルド（＝部品） |
| `3_Build_Business_*.bat` / `3_Build_BusinessRichClient_*.bat` | テンプレート（`Business` / `CustomControl` 系） |
| `4_Build_CopyAssemblies.bat` | `Build_net48\` → `Build\` の複写（7.1 節） |
| `4_Build_Framework_Tool.bat` / `4_Build_Framework_ToolCore.bat` | ツール |
| `5_Build_*_sample.bat` / `5_Build_*Core_sample.bat` | サンプル（2CS / Bat / CLI） |
| `6_Build_WSSrv*_sample.bat` | `WSServer_sample` をビルドし `WS_sample\Build\` へ複写（続けてサンプルの `ASPNETWebService`） |
| `7_Build_Framework_WS.bat` | **`ServiceInterface`**（基盤側のホスト。`WS_sample\Build\` を参照するので `6_` の後） |
| `7_Build_Framework_WSCore.bat` | **何もしない**（net10.0 にホストが無い。`1_BuildAll.ps1` の段を残すための空の手順） |
| `8_Build_WSClnt*_sample.bat` | WS クライアント |
| `10_Build_WebApp*_sample.bat` | Web アプリのサンプル |
| `y_Build_Test*.bat` | テスト |
| `0_ExecAllBat.bat` | 全実行 |
| `0_Release4Nuget.bat` | NuGet パッケージ用のビルド（[`NuGet/README.md`](../NuGet/README.md)） |
| `z_Common.bat` / `z_Common2.bat` | ビルド ツールのパス解決（各 bat が冒頭で `call` する） |

サンプル側の bat の詳細は [`Samples/ANALYSIS.md`](../Samples/ANALYSIS.md) 8 節と
[`Samples4NetCore/ANALYSIS.md`](../Samples4NetCore/ANALYSIS.md) 6 節。

.NET (Core) 側は `dotnet restore` → `dotnet msbuild` を呼ぶだけなので、
**エージェントは bat を経由せず直接 `dotnet build "Frameworks\Infrastructure\Nuget_netcore100.sln"` してよい。**
net48 側は MSBuild / devenv が必要（Windows + VS 前提）。

#### `z_Common.bat` と `z_Common2.bat`（MSBuild 版 / devenv 版）

両者は **`%BUILDFILEPATH%`（ビルド ツールのパス）と `%COMMANDLINE%`（ビルド引数）という
同じインターフェイスを提供する差し替え可能な対**であり、呼び出し側の bat は
`call` するファイルを変えるだけでビルド ツールを切り替えられる設計になっている。

| | ビルド ツール | `%COMMANDLINE%` |
|---|---|---|
| `z_Common.bat` | MSBuild.exe | `/p:Configuration=<構成> /p:DebugType=<型> /p:ContinuousIntegrationBuild=<真偽> /p:DeterministicSourcePaths=<真偽> -v:d` |
| `z_Common2.bat` | devenv.com / devenv.exe | `/build <構成>`（devenv 構文） |

`z_Common.bat` の次の 2 つは、**呼び出し側が先に設定していればその値を尊重する**（#531）。
NuGet パッケージ用のビルド（`0_Release4Nuget.bat`）だけが既定と異なる値を渡す。

| 変数 | 既定 | `0_Release4Nuget.bat` | 目的 |
|---|---|---|---|
| `DEBUG_TYPE` | `full` | `portable` | `.snupkg` は portable PDB でなければ受け付けられない |
| `CI_BUILD` | `false` | `true` | PDB のソース パスを `/_/...` に正規化する |

`CI_BUILD` は `ContinuousIntegrationBuild` と `DeterministicSourcePaths` の
**両方**に渡している。前者から後者への変換は `Microsoft.NET.Sdk` のターゲットが
行うため、**旧形式 csproj には効かない**（Source Link の自動組み込みが効かないのと
同じ構図。渡さないと **net48 側だけ絶対パスのまま残る**）。

**以前は `z_Common.bat` を手で書き換えて戻す運用だった。**
戻し忘れると `full` のまま公開してしまうため、呼び出し側から渡す形にした。

`ContinuousIntegrationBuild` を有効にすると、PDB に**ビルドしたマシンの
ローカル パスが残らない**。これは体裁の問題ではない。Visual Studio は
**PDB のパスにファイルが在れば、それを開く**ため、絶対パスのままだと
**ビルドしたマシンでは Source Link を通らず、動作確認にならない**。
詳細は [`NuGet/README.md`](../NuGet/README.md)。

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

**部品（`2_`）→ テンプレート（`3_`）の順は、`HintPath` 参照による制約である。**
`Business` 系は `Nuget_*.sln` の出力 DLL を拾うだけなので、`Framework` を直して `3_` だけを回しても変更は入らない
（[`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) 4.2 節）。

**`4_Build_CopyAssemblies.bat`（`Build_net48` → `Build\` の xcopy）を飛ばすと、
net48 のサンプルとツールが一切ビルドできない。**
`Frameworks\Infrastructure\Build\` を `HintPath` 参照するプロジェクトが CS/VB 合わせて **46 件**あるため
（2026-10-17 の実測。net48 のサンプル CS 20・VB 18、`Tools` 3、`Tests` 3、`ServiceInterface` 2）。
`0_ExecAllBat.bat` が `2_`/`3_` → `4_CopyAssemblies` → `5_`〜`10_`（サンプル）の順で呼ぶのはこのため。

`Build\` に入るのは **net48 の成果物のみ**で、.NET (Core) 側に相当物は無い
（`Samples4NetCore` は `Build_netcore100\net10.0\` を直接参照する）。この非対称は設計どおり。

**`ServiceInterface`（`7_`）はサンプルの `WS_sample\Build\`（`6_`）に依存する。** 基盤側のホストが
サンプルのビルド結果を参照している（[`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) 7 節）。

### 7.2 バージョン番号の一元管理

**`Infrastructure/Directory.Build.props` の `OpenTouryoVersion` が唯一の定義箇所。**

```xml
<PropertyGroup>
  <OpenTouryoVersion>3.3.0-alpha1</OpenTouryoVersion>   <!-- 2026-10-17 時点の値 -->
  <Company>Hitachi Solutions</Company>
</PropertyGroup>
```

**手で書き換えない。** `0_SetVersion.ps1` が、ここと net48 の `AssemblyInfo.cs` を一括で書き換える（手順は [`RELEASE.md`](../../RELEASE.md) フェーズ 0）。

| 反映先 | 仕組み |
|---|---|
| SDK 形式アセンブリ（7 個） | 各 csproj の `<Version>$(OpenTouryoVersion)</Version>` |
| NuGet パッケージ | nuspec は `<version>$version$</version>` と `<dependency id="Touryo.Infrastructure.*" version="$version$" />`。`_NuGetPack.bat` が props から値を読み `nuget pack -Properties version=...` で渡す |

- **旧形式 csproj（`*_net48.csproj`）には効かない。** `Microsoft.Common.props` をインポートしないため。
  net48 のバージョンは各プロジェクトの `Properties\AssemblyInfo.cs` が持つ。
- **`Business` 系は 1.0.0 で別系統。** `Business/Properties/AssemblyInfo.cs` が `AssemblyVersion("1.0.0.0")` であり、
  Public / Framework / Public.Security の版（`OpenTouryoVersion` に追随する）とは意図的に分けている。
  そのため `Directory.Build.props` では `<Version>` を全体に設定せず、
  `OpenTouryoVersion` という独自プロパティをパッケージ対象の 7 プロジェクトだけが参照する形にしている。

> **注意**: `Directory.Build.props` の XML コメント内にハイフン 2 個の連続（`--`）を書くと
> XML として不正になり、MSBuild がプロジェクトの読み込みに失敗する。区切り線に使わないこと。

### 7.3 NuGet パッケージ化

`root/programs/CS/NuGet/`（`*.nuspec` + `_NuGetPack.bat`、`in/` に DLL を staging）。
手順は [`NuGet/README.md`](../NuGet/README.md) を参照。

- **nuspec の `<dependencies>` は csproj の `PackageReference` と一致させる**（余分・不足・版ズレなし）。
  依存を増減したら nuspec 側も合わせること。
- パッケージ自身と `Touryo.Infrastructure.*` の相互依存のバージョンは `$version$` トークンで自動追随する（7.2 節）。
- `Symbol_Framework.RichClient.nuspec` のみ、`net10.0-windows7.0` ビルドを `lib\net10.0` に配置している（12 節 14）。

---

## 8. コーディング規約

**[`CODING.md`](../../CODING.md) に移した（`root/programs/CODING.md`）。**

ファイル ヘッダの書式と更新者名、`ArgumentException` 系の引数の順、
`.bat` / `.ps1` の文字コードは、いずれもそちらが一次情報である。

`root/programs/` 配下に共通で効く規約であり、
本書（Frameworks の分析）に置くと他領域から参照しにくいため分離した。

| | `CODING.md` の節 |
|---|---|
| ファイル ヘッダ（**新規と既存で書式が違う**） | 1 |
| その他の規約（コメント・リソース・命名） | 2 |
| 引数を検査する例外 | 3 |
| bat ファイルの文字コード | 4 |
| ps1 の文字コードと PowerShell 5.1 / 7 の両対応 | 5 |

---

## 9. Public 層の主なユーティリティ（再実装しないこと）

| 名前空間 | 主なクラス |
|---|---|
| `Public.Db` | `BaseDam` `SQLUtility` `DbEnum` `Dam*` |
| `Public.Dto` | `DTTable/DTRow/DTColumn`（マーシャリング可能な自前 DataTable）、`DataToPoco` `PocoToPoco` `DataToDictionary` |
| `Public.Str` | `CustomEncode`(Base64/Hex/URL) `StringChecker` `FormatChecker` `StringConverter` `FormatConverter` `CheckCharCode` `JIS2k4Checker` |
| `Public.Security` | 共通鍵・公開鍵暗号、ハッシュ、署名、`Jwt/`（JWS・JWE・JWK）、`Aead/`、`KeyExg/`、`Pwd/`（[`Public/Security/ANALYSIS.md`](Infrastructure/Public/Security/ANALYSIS.md)） |
| `Public.Log` | `LogIF`（静的 façade）。ロガー名は慣習的に `"ACCESS"` と `"SQLTRACE"`。バックエンドは `LogLib` 設定で log4net / NLog |
| `Public.Reflection` | `Latebind`（フレームワークの動的呼び出しの心臓部）、`MyAssemblies` |
| `Public.FastReflection` | `AccessorCacher` `CompiledExpressionCreater` `InstanceCreator<T>` `EnumToString*Extensions` |
| `Public.IO` | `ResourceLoader` `EmbeddedResourceLoader` `DeflateCompression` `ExponentialBackoff` `ZipperV2/UnZipperV2` |
| `Public.Util` | `GetConfigParameter` `PerformanceRecorder` `RandomValueGenerator` `EnvInfo` `PubCmnFunction` |
| `Public.Diagnostics` | `MyDebug`（`OutputDebugAndConsole`）`ObjectInspector` `StackFrameOperator` |
| `Public.Win32` / `WinProc` | P/Invoke 群（net48 のみ） |

各名前空間の中身と落とし穴は [`Public/ANALYSIS.md`](Infrastructure/Public/ANALYSIS.md)。

---

## 10. 認証まわり（`Framework/Authentication/`）

**[`Framework/ANALYSIS.md`](Infrastructure/Framework/ANALYSIS.md) 8 節に移した。** 比較的新しく、直近も更新されている領域（#600〜#606）。

| 内容 | 節 |
|---|---|
| `OAuth2AndOIDCClient`（すべて `static async Task<string>`）、`CmnClientParams`（`SpRp_*`）は**クライアント役の設定**であること | 8.1 |
| トークンの検証（`CmnJwtToken` / `IdToken` / `AccessToken` の `aud` の扱い、相手の値を受けるオーバーロード） | 8.2 |
| SAML2（相手ごとの証明書と署名鍵、`SigAlg`、`VerifyRedirect` の 2 つの版） | 8.3 |

**既存の版のシグネチャを変えず、オーバーロードを足す**という方針が、この領域の変更すべてに通っている
（ビルド済みの呼び出し元が `MissingMethodException` で落ちるため）。

---

## 11. 主要な依存パッケージ（netcore100）

**版は csproj を直接見ること。** Dependabot と #610 の運用で随時変わるため、本書には版を書かない。

| プロジェクト | パッケージ | 一次情報 |
|---|---|---|
| Public | `log4net` `NLog` `Microsoft.Data.SqlClient` `System.Data.Odbc` `Microsoft.Extensions.Configuration*` `Newtonsoft.Json` `SharpZipLib` `Zipangu`、推移的依存を引き上げる `System.Security.Cryptography.Pkcs` | [`Public/ANALYSIS.md`](Infrastructure/Public/ANALYSIS.md) 9 節 |
| Public.Security | `BouncyCastle.NetCore` `System.Security.Cryptography.Xml` `Newtonsoft.Json` | [`Public/Security/ANALYSIS.md`](Infrastructure/Public/Security/ANALYSIS.md) 7 節 |
| Framework | `Microsoft.AspNetCore.{Http,Http.Extensions,Mvc,Session}` `Microsoft.Extensions.PlatformAbstractions` | [`Framework/ANALYSIS.md`](Infrastructure/Framework/ANALYSIS.md) 10 節 |
| Business | `Microsoft.AspNetCore.Authentication{,.Cookies}` `Microsoft.AspNetCore.{Http,Mvc}` `System.Security.Cryptography.Xml` `Newtonsoft.Json` ほか | [`Business/ANALYSIS.md`](Infrastructure/Business/ANALYSIS.md) 6 節 |

`jose-jwt` は**部品の依存ではない**（`Tests/Golden/EncAndDecUtilCUI` が相互運用の確認相手として参照する）。

**注意 1:** `Microsoft.AspNetCore.*` は **2.3.0（互換シム パッケージ）** を使い続けている。
net10.0 のフレームワーク参照（`Microsoft.AspNetCore.App`）ではない。安易に上げると壊れる可能性が高い。

**注意 2:** `System.Security.Cryptography.Xml` は **4 箇所すべてを同じ版で揃える**こと。
Dependabot が 1 プロジェクトずつしか上げないため、放っておくと版が割れて NU1903 が残る（12 節 11）。

**注意 3:** 部品の依存を上げたら、**`HintPath` で部品を拾うサンプル（`Samples4NetCore`）の csproj と、
net48 の `packages.config`・`bindingRedirect` も見る**（`ComparePackage.ps1` / `CompareRedirect.ps1`。
[`Samples4NetCore/Legacy/ANALYSIS.md`](../Samples4NetCore/Legacy/ANALYSIS.md) 6 節）。
脆弱性の警告を見つけたときの扱いは `AGENTS.md`「依存パッケージの脆弱性を見つけたら」と [`BUILDING.md`](../../BUILDING.md) 13 節。

---

## 12. 落とし穴 / 既知の不整合（作業前に把握しておく）

**領域をまたぐもの、または経緯として残すものを置く。** 各アセンブリに閉じた落とし穴は、下位文書の「落とし穴」の節にある。
番号は他の文書から参照されているので、**詰めずに残す。**

1. **`4_Build_CopyAssemblies.bat` を飛ばすと net48 のサンプル・ツールが全滅する**（7.1 節）。
2. **`Compile Remove` を忘れると .NET (Core) ビルドが壊れる**（6.3 節）。
3. **`CmnDao` は `new` による隠蔽**であって override ではない。`BaseDao` 型変数経由で呼ぶと親の実装が走る。
4. **`ReturnValue` を設定し忘れると戻り値が null** になる（3.2 節）。例外時も同様。
5. **`_dams` ディクショナリと `_dam` の二重管理**。Commit/Rollback/Close は両方に対して行われる。
   複数 DB を使う場合は `SetDam(key, dam)` を使い、`GetDam(key)` で取り出す。
6. **`BaseController.cs`（Web Forms）と `BaseDam.cs` は巨大**。
   変更時は該当 `#region` に閉じた修正に留め、全体リファクタは避ける（互換性維持が最優先の設計）。
7. **VB 版ミラーが存在**: `root/programs/VB/Frameworks/Infrastructure/` に
   `Business` / `CustomControl` / `ServiceInterface` の VB.NET 版がある（`Public` / `Framework` は C# 版を共有）。
   Business 層テンプレートの仕様を変えると **VB 側も追随が必要**（[`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) 8 節）。
8. **生成物は `.gitignore` 済み。** `Build*/` と `ServiceInterface/*/dll/`、テストが出力先に作る `*.cer` は、
   `git status` に出ない（2026-10-17 の実測で未追跡は 0 件）。**出てきたら、それは新しい生成物か残骸なので、コミットせずに報告する。**
9. `Public/Security/MyDebug.cs` は `Public/Diagnostics/MyDebug.cs` の**派生クラス**（重複ではない。
   アセンブリ分割の都合）。
10. `TMProtocolDefinition2.xml` など「2」付きの定義ファイルが並存する。用途は用例違い。
11. **`System.Security.Cryptography.Xml` は参照している 4 箇所を同じ版で揃える**（NU1903 / 高、2026/08/08 に一度解消）。
    **Dependabot は 1 プロジェクトずつしか上げない。** #507 が `Public.Security` だけを
    上げ、#510 がテスト プロジェクトだけを上げたため、一時 4 箇所が 3 通りの版に割れていた（84 警告）。
    **セキュリティ更新は、Dependabot の PR を取り込んだら他の参照も揃えること。**
    揃えておけば「もう脆弱でない」ため、同じ箇所への PR が後から来て競合することも無い
    （運用は [`BUILDING.md`](../../BUILDING.md) 9 節）。
    4 箇所と、net48 がこのパッケージを参照しない（BCL を使う）ことは
    [`Public/Security/ANALYSIS.md`](Infrastructure/Public/Security/ANALYSIS.md) 7 節。
12. **結果ファイル比較のテストは、検算しないと不具合を「正」として固定する。**
    実例が 2 つある。`ArrayOperator.GetLongFromByte` が 3 byte 目から誤った値を返していた（#522。
    [`Public/ANALYSIS.md`](Infrastructure/Public/ANALYSIS.md) 10 節）。
    `EncAndDecUtilCUI` が ES384 / ES512 の `Verify` を `False` で記録していた（#595。
    [`Public/Security/ANALYSIS.md`](Infrastructure/Public/Security/ANALYSIS.md) 3.1 節）。
    **期待値を作るときは、出力をそのまま貼らず実装から検算し、境界を跨いだケースを置く。**
    **`Verify` の期待値が `False` なら、それが仕様か不具合かを確かめること**
    （[`Tests/Golden/TestCode/README.md`](Tests/Golden/TestCode/README.md)）。
13. **`Public/IO` に、ビルド対象外の旧 ZIP 部品が残っている**。
    `ZipBase.cs` / `Zipper.cs` / `UnZipper.cs` は DotNetZip（非推奨・既知脆弱性
    `GHSA-xhg6-9j5j-w4vf`）に依存しており、**net48 / netcore100 の両方で除外**してある。
    - net48 … 列挙形式のため `<Compile>` に載っていない
    - netcore100 … `<Compile Remove="IO\Zipper.cs" />` 等で明示的に除外

    **現役は `ZipperV2` / `UnZipperV2`**（SharpZipLib、#524）。
    旧ファイルを直しても何も起きない。**触るなら V2 の方。**
    I/F 互換は無いが、**引数の並びと名前、列挙体のメンバ名は旧に合わせて**ある。
    自己解凍書庫・選択条件の文字列・`StatusMSG` は落とした（代替は `ExtractedFiles`）。
    ビルド対象外のファイルは他にもある（[`Infrastructure/ANALYSIS.md`](Infrastructure/ANALYSIS.md) 6 節）。
14. **`Symbol_Framework.RichClient.nuspec` の lib TFM が不正確**。
    `net10.0-windows7.0` ビルドを `lib\net10.0` に配置している。本来は `lib\net10.0-windows` が正しく、
    現状は Linux 上の net10.0 消費者も解決してしまう。直すとパッケージ解決セマンティクスが変わるため据え置き。
15. **オーバーロードへの委譲で引数を落とすと、自分自身を呼んでもコンパイルが通る**（警告も出ない）。
    `SymmetricCryptography.GenerateKeyFromPassword` で、**`EncryptString` / `DecryptString` が
    `StackOverflowException` でプロセスごと落ちていた**（**`catch` できない**。#608）。
    **呼ばれないコードは結果ファイルに現れない。** 経緯と、鍵の導出を変えてはいけない理由は
    [`Public/Security/ANALYSIS.md`](Infrastructure/Public/Security/ANALYSIS.md) 6 節。

---

## 13. サンプル（フレームワークの正しい使い方の参照先）

| パス | 内容 | 文書 |
|---|---|---|
| `Samples/` | net48 版サンプル（Web Forms / MVC5 / 2 層 C/S / バッチ / WS） | [`Samples/ANALYSIS.md`](../Samples/ANALYSIS.md) |
| `Samples4NetCore/Backend/` | .NET 10 版の Web アプリ（`MVC_Sample` / `ASPNETWebService`） | [`Samples4NetCore/ANALYSIS.md`](../Samples4NetCore/ANALYSIS.md) |
| `Samples4NetCore/Legacy/` | 上記 net48 版の .NET 10 移植 | 同上 |

典型的な B層実装は `Samples/Bat_sample/SimpleBatch_sample/Business/LayerB.cs`（`MyFcBaseLogic` 継承）。
「テンプレ」`#region` に `UOC_メソッド名` という雛形がそのまま残っているのが本フレームワークの流儀。

---

## 14. エージェント向け作業チェックリスト

- [ ] `AGENTS.md` のポリシー遵守（**git 操作をしない**）
- [ ] 作業対象の**下位文書**（1 節の案内表）の「落とし穴」と「作業チェックリスト」を読む
- [ ] 変更対象が net48 / netcore100 / 両方のどれか判定（`Compile Remove` を確認）
- [ ] 条件コンパイルは **`#if NETCOREAPP`（＝非 net48）を使う**。
      `#if NETSTD` は `Framework` / `Public.Security` / `Dam*` でしか真にならないので新規には使わない（6.2 節）
- [ ] 新規 .cs にはヘッダ コメント（Apache License / クラス名・日本語名・更新履歴）を付与。
      **Copyright ブロックは新規には付けない**（[`CODING.md`](../../CODING.md) 1 節）
- [ ] 既存 .cs 変更時は更新履歴に 1 行追記
- [ ] public/protected メンバに日本語 `<summary>` を付与（DocumentationFile 出力のため）
- [ ] 文字列リテラルは `FxLiteral` / `MyLiteral` / `PubLiteral` に定数として追加
- [ ] ビルド確認: `dotnet build Frameworks/Infrastructure/Nuget_netcore100.sln`
      （net48 は MSBuild 必須）
- [ ] テスト実行前に **フレームワークを先にビルド**（`Build_netcore100/net10.0/*.dll` を HintPath 参照するため）
- [ ] Business 層テンプレートを変えたら VB 版ミラーの追随要否を報告
