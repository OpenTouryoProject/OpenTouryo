# ANALYSIS.md — 業務フレームワーク（Infrastructure/Business）

対象: `root/programs/CS/Frameworks/Infrastructure/Business`（`RichClient/` を除く） / 最終更新: 2026-10-16

上位: [`Infrastructure/ANALYSIS.md`](../ANALYSIS.md) / 全体: [`Frameworks/ANALYSIS.md`](../../ANALYSIS.md)
下位: [`RichClient/ANALYSIS.md`](RichClient/ANALYSIS.md)（`OpenTouryo.Business.RichClient`）

---

## 1. これは何か

`OpenTouryo.Business`（名前空間 `Touryo.Infrastructure.Business`）。**業務フレームワーク＝テンプレート層**である。

- `Framework` の**親クラス 1** を継承した**親クラス 2**（`MyBase*`）を持ち、アプリはこれを継承する
- **利用者がソースを取り込み、自分の業務に合わせて改造する前提**で配っている（NuGet には無い）。
  クラス日本語名の「（テンプレート）」は、この意味である
- バージョンは **`1.0.0.0` で、部品（3.0.0）とは別系統**（`Properties/AssemblyInfo.cs`）

**ここを直すと、サンプルの振る舞いが変わる。** サンプルはこのテンプレートをそのまま使っている。
一方、**利用者の手元の改造版には届かない**（次に取り込むまで）。

---

## 2. 構成

| フォルダ | 主なクラス | 一言で |
|---|---|---|
| `Business/` | `MyFcBaseLogic`（自動振り分け付き）`MyBaseLogic`（net48 のみ）`_3TierEngine`（net48 のみ） | B 層の親クラス 2（3 節） |
| `Common/` | `MyParameterValue` `MyReturnValue`（`MyUserInfo` を足す）、`MuParameterValue` `MuReturnValue`（汎用。文字列と Bean を持つ）、`_3TierParameterValue` `_3TierReturnValue`（net48 のみ） | 引数・戻り値の親クラス 2 |
| `Dao/` | `MyBaseDao` `CmnDao` `BaseConsolidateDao` | D 層の親クラス 2（4 節） |
| `Presentation/` | `MyBaseController`（Web Forms）`MyBaseMVController`（MVC5）`MyBaseMVControllerCore` `MyMVCCoreFilterAttribute`（ASP.NET Core）`MyBaseAsyncApiController`（Web API。net48 と net10.0 で別ファイル）ほか | P 層の親クラス 2（5 節） |
| `Exceptions/` | `MyBusinessApplicationExceptionMessage` `MyBusinessSystemExceptionMessage` | 業務例外・システム例外のメッセージ ID と文言（リソースは `Resources/`） |
| `Util/` | `MyUserInfo` `MySubsysInfo` `MyLiteral` `MyCmnFunction` `MyAttribute` `GMTMaster`、`MyTimeZone` `MyTimeZoneEnum`（net10.0 のみ） | ユーザ情報・定数・時刻帯ほか |
| `Str/` | `JISX0208_1983Checker` | JIS X 0208-1983 の範囲チェック |
| `Csp/` | `CstSqlSessionStateProvider` | SQL Server を使うセッション状態プロバイダ（**net48 のみ**） |

---

## 3. B 層（`MyFcBaseLogic` / `MyBaseLogic`）

| クラス | TFM | `UOC_DoAction` |
|---|---|---|
| `MyFcBaseLogic` | net48 / net10.0 | **`"UOC_" + MethodName` を `Latebind` で呼ぶ**（自動振り分け） |
| `MyBaseLogic` | net48 のみ | アプリが `UOC_DoAction` を書く |

**net10.0 で使えるのは `MyFcBaseLogic` だけ。** 新しいコードは `MyFcBaseLogic` を前提にする。
自動振り分けの書き方（`this.ReturnValue` を冒頭で設定する）は [`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 3.2 節。

### 3.1 データ プロバイダの選択（`UOC_ConnectionOpen`）

`ActionType`（`"SQL%..."` のような `%` 区切り）の**先頭要素**で Dam と接続文字列を選ぶ。
分離レベルが `NotConnect` なら接続しない。

| キー | Dam | 接続文字列のキー | `MyFcBaseLogic` | `MyBaseLogic` |
|---|---|---|---|---|
| `SQL` | `DamSqlSvr` | `ConnectionString_SQL` | ✓ | ✓ |
| `OLE` | `DamOLEDB` | `ConnectionString_OLE` | net48 のみ | ✓ |
| `ODB` | `DamODBC` | `ConnectionString_ODBC` | ✓ | ✓ |
| `ODP` | `DamManagedOdp` | `ConnectionString_ODP` | ✓ | ✓ |
| `MCN` | `DamMySQL` | `ConnectionString_MCN` | ✓ | ✓ |
| `NPS` | `DamPstGrS` | `ConnectionString_NPS` | **net10.0 のみ** | —（`#if NET48` で外れる） |
| `ORA` / `DB2` / `HIR` | — | — | コメントアウト | コメントアウト |

**どれにも当たらないキーは、黙って SQL Server（`DamSqlSvr` と `ConnectionString_SQL`）になる。**
キーの綴りを誤っても例外にならないので、意図しない DB に接続しても気付きにくい。

`MyBaseLogic` の `#if NETCOREAPP2_0` は、net48 専用のファイルの中にあるので**常に偽**（死んだ分岐）。

### 3.2 前後処理と例外

- `UOC_PreAction` / `UOC_AfterAction` で **`ACCESS` ログ**（`LogIF`）を書く。性能は `PerformanceRecorder` で測る
- `UOC_ABEND` で例外ごとの後始末をする。**`"Other-Business"` / `"Other-System"` というメッセージで例外を振り替える分岐は、テンプレートの例**である
  （業務例外やシステム例外へ振り替えるときの書き方を示している）

### 3.3 `_3TierEngine`（net48 のみ）

`_3TierParameterValue` の検索条件の辞書から SQL を組み立てる CRUD エンジン（約 1450 行）。
`DaoGen_Tool` が生成するコードと、`CmnTableAdapter`（三層データバインド）とセットで使う。

---

## 4. D 層（`MyBaseDao` / `CmnDao`）

| クラス | 何を足すか |
|---|---|
| `MyBaseDao` | `SetSqlByFile2`（**`appSettings:sqlTextFilePath` を基点に SQL ファイルのパスを解決**。`MyBaseDao.UseEmbeddedResource = true` で埋め込みリソースから読む）、`SQLTRACE` ログと性能の測定 |
| `CmnDao` | **Dao クラスを書かずに使える汎用の Dao。** パラメタを `Dictionary` に溜め、実行の直前に反映する |
| `BaseConsolidateDao` | 複数の Dao をまとめる集約クラスの例（`BaseDam` を受け取る） |

**`CmnDao` は `BaseDao` の `protected` メソッドを `new` で隠して `public` にしている（`override` ではない）。**
`BaseDao` 型の変数を通して呼ぶと、親の実装が走る。

---

## 5. P 層（`Presentation/`）

| クラス | 対象 | TFM |
|---|---|---|
| `MyBaseController` | ASP.NET Web Forms（`BaseController` の派生。抽象） | net48 |
| `MyBaseMVController` | ASP.NET MVC5 | net48 |
| `MyBaseMVControllerCore` | ASP.NET Core MVC（`IResultFilter` も実装） | net10.0 |
| `MyMVCCoreFilterAttribute` | ASP.NET Core の `ActionFilterAttribute` として、親クラス 2 と同じ前後処理を差し込む | net10.0 |
| `MyBaseAsyncApiController` | ASP.NET Web API の `ActionFilterAttribute`。認証ヘッダ（`EnumHttpAuthHeader`）の検証、`ACCESS` ログ、例外処理 | net48（`MyBaseAsyncApiController.cs`）／ net10.0（`MyBaseAsyncApiControllerCore.cs`） |
| `AuthenticationFailureResult` / `ResultWithChallenge` | Web API の認証失敗の応答 | net48 |
| `CmnTableAdapter` | 三層データバインドの TableAdapter | net48 |

- **`MyBaseAsyncApiController` は、ファイルが 2 つで、クラス名が同じ**（TFM でどちらか一方だけが入る）。
  net10.0 版は `Bearer` のとき `AccessToken.Verify` でアクセス トークンを検証する
- 画面の入口・出口で `ACCESS` ログを書き、`MyUserInfo` を組み立てる

---

## 6. TFM 差

| | net48 | net10.0 |
|---|---|---|
| 条件コンパイル | `NET48` | `NETCOREAPP`（`NETSTD` は**定義されない**） |
| 外しているもの | `MyBaseMVControllerCore.cs` `MyMVCCoreFilterAttribute.cs` `MyBaseAsyncApiControllerCore.cs` `Util/MyTimeZone.cs` `Util/MyTimeZoneEnum.cs`（列挙に無い） | `Business/MyBaseLogic.cs` `Business/_3TierEngine.cs` `Common/_3Tier*Value.cs` `Csp/**` `RichClient/**`、`Presentation/` の `MyBaseController.cs` `MyBaseMVController.cs` `MyBaseAsyncApiController.cs` `AuthenticationFailureResult.cs` `ResultWithChallenge.cs` `CmnTableAdapter.cs` |
| 参照 | `Build_net48\` の DLL（`HintPath`） | `Build_netcore100\net10.0\` の DLL（`HintPath`） |
| 依存 | — | `Microsoft.AspNetCore.{Authentication, Authentication.Cookies, Http, Http.Extensions, Mvc}` 2.3.0、`System.Security.Cryptography.Xml`、`Newtonsoft.Json` ほか |

**`HintPath` で部品の DLL を拾うので、部品を先にビルドしないと古いものを見る**
（[`Infrastructure/ANALYSIS.md`](../ANALYSIS.md) 4.2 節）。

**`System.Security.Cryptography.Xml` は、他の 3 箇所と同じ版に揃える**
（[`Public/Security/ANALYSIS.md`](../Public/Security/ANALYSIS.md) 7 節）。

---

## 7. VB 版

`root/programs/VB/Frameworks/Infrastructure/Business/` に **VB.NET 版のテンプレート**がある（net48 のみ）。
**仕様を変えたら、VB 版の追随が要るかを報告すること。** ビルドと疎通は `-Lang VB`。

---

## 8. 落とし穴

1. **未知のプロバイダ キーは SQL Server に落ちる**（3.1 節）。
2. **`NPS`（PostgreSQL）は net10.0 だけ、`OLE` は net48 だけ**（3.1 節）。
3. **`CmnDao` は `new` による隠蔽**（4 節）。
4. **`MyBaseLogic` と `_3TierEngine` は net48 専用**（3 節）。net10.0 のサンプルで使おうとするとビルドが通らない。
5. **同じクラス名のファイルが 2 つある**（`MyBaseAsyncApiController`。5 節）。片方だけ直すと、TFM で振る舞いが割れる。
6. **テンプレートを直すと、サンプルの振る舞いが変わる**（1 節）。サンプルの疎通テストも回す。

---

## 9. 作業チェックリスト

- [ ] 直すのは net48 のファイルか、net10.0 のファイルか、両方か（6 節）。同じ役割のファイルが TFM で分かれていないか
- [ ] 文字列リテラルは `MyLiteral` に、メッセージは `Exceptions/` とリソース（既定と `ja-JP`）に足す
- [ ] 部品を先にビルドしてから、`3_Build_Business_*.bat` / `3_Build_BusinessRichClient_*.bat`
- [ ] サンプルの疎通（[`SMOKETEST.md`](../../../../SMOKETEST.md)）で振る舞いを確かめる
- [ ] VB 版の追随の要否を報告する（7 節）
