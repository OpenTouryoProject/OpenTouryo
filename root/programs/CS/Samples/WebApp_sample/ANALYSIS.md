# ANALYSIS.md — Web アプリのサンプル（CS/Samples/WebApp_sample）

対象: `root/programs/CS/Samples/WebApp_sample`（net48） / 最終更新: 2026-10-16

上位: [`Samples/ANALYSIS.md`](../ANALYSIS.md)（前提・呼び出しパターン・`ActionType`・設定・Legacy との関係）

---

## 1. これは何か

ASP.NET（.NET Framework）の Web アプリのサンプル 2 本。

| サンプル | 方式 | 親クラス 2 | 規模 |
|---|---|---|---|
| `WebForms_Sample` | ASP.NET Web Forms | `MyBaseController`（ページ）、`CustomControl` | **最大のサンプル**（`.aspx` 38 画面） |
| `MVC_Sample` | ASP.NET MVC5 | `MyBaseMVController` | コントローラ 5 つ |

**`WebForms_Sample` は、P 層の機能（画面遷移の制御・二重送信の検出・ダイアログ・マスタ ページの入れ子など）の網羅テストを兼ねる。**
ただし自動で確かめているのはログインまで（4 節）で、残りは手で見る。

---

## 2. `WebForms_Sample`

### 2.1 画面（`Aspx/`）

| フォルダ | 画面数 | 内容 |
|---|---:|---|
| `start/` | 3 | `login` / `menu` / `logout` |
| `testFxLayerP/` | 19 | **P 層の機能の網羅テスト**（最重要）。`normal/`（基本）、`nest/`（マスタ ページの入れ子）、`table/`（`GridView` / `ListView` / `Repeater`）、`withAjax/`（クライアント コールバック・Ajax 拡張）、`wcc/`（カスタム コントロール `WebCustomTextBox`）、ダイアログ（`testDLScreen` / `testDLFrame` / `testDialogAtOnLoad`）、遷移先（`testTransitionAheadScreen`） |
| `testScreenCtrl/` | 6 | 画面遷移の制御（`FxScreenTransitionMode` / `SCDefinition.xml`） |
| `sample/` | 5 | 業務サンプル。`crud/`（CRUD。`sampleScreen_cc` はカスタム コントロール版）、`3Tier/`（三層データバインド） |
| `Framework/` | 3 | 基盤の画面（OK / YesNo ダイアログ、`Ping`） |
| `OAuth2/` | 1 | OAuth2 の Authorization Code Grant のクライアント |
| `Common/` | 1 | エラー画面（ほかにマスタ ページ一式。`Common/Master/testNest/` は入れ子の検証） |

**イベント ハンドラは、コントロール ID の接頭辞で `BaseController` が結び付ける**（`btn` → `UOC_btnXXX_Click`。
[`Framework/ANALYSIS.md`](../../Frameworks/Infrastructure/Framework/ANALYSIS.md) 4.1 節）。
接頭辞の設定は `app.config` の `FxPrefixOf*`。

### 2.2 B 層・D 層（`AppCode/`）

| フォルダ | 内容 |
|---|---|
| `AppCode/sample/Business/` | `LayerB`（`MyFcBaseLogic`）、`GetMasterData`（リスト用のマスタ データを読む部品） |
| `AppCode/sample/Dao/` | `LayerD`、自動生成 Dao（`DaoShippers` / `DaoCategories` / `DaoSuppliers`） |
| `AppCode/sample/3TierTableAdapter/` | `ProductsTableAdapter`（三層データバインド。`CmnTableAdapter` と `_3TierEngine`、net48 のみ） |
| `AppCode/testPublic/Business/` | `TestMTC` / `TestMTC_mcn` / `TestMTC_txg`（**手動トランザクション制御**。複数コネクション版を含む） |

### 2.3 設定

- **`appSettings` は `Web.config` ではなく `app.config` にある**（`<appSettings file="app.config" />`）。設定を探すときは `app.config` を見る
- `sessionState` は **`StateServer`**（`127.0.0.1:42424`）。**ASP.NET 状態サービス（`aspnet_state`）が動いていないと、セッションを使う要求がエラーになる**（疎通テストも前提に挙げている）
- 認証は Forms 認証。`<deny users="?" />` で全画面が要認証。ログイン後は `Aspx/Start/menu.aspx` へ
- `_3TierEngine` の命名規約キー（`MethodNameHeaderS` など）は、自動生成 Dao のメソッド名と連動する（上位 7-2）

---

## 3. `MVC_Sample`

| コントローラ | 内容 |
|---|---|
| `HomeController` | ログイン（Forms 認証）。ユーザー名が空でなければ認証する |
| `Crud1Controller` | CRUD（`Html.BeginForm` 版） |
| `Crud2Controller` | CRUD（`Ajax.BeginForm` 版） |
| `PingController` / `ErrorController` | 死活確認・エラー画面（`MyBaseMVController` を継承しない） |

- B 層・D 層は `Logic/{Business, Common, Dao}`。**`LayerB` は `WS_sample/WSServer_sample` のものとは別のコピー**である
  （`WS_sample/ASPNETWebService` の `JsonController` は `WSServer_sample` 側を使う。[`WS_sample/ANALYSIS.md`](../WS_sample/ANALYSIS.md)）
- `sessionState` は `StateServer`（Cookie 名 `mvc_session`）。`appSettings` は `app.config`
- 複数台に配備するなら `machineKey` が要る（`Web.config` のコメント。仕組みは [`CONFIGURATION.md`](../../../CONFIGURATION.md) 8 節）

---

## 4. ビルドと検証

- ビルド: `10_Build_WebApp_sample.bat`（`WebForms_Sample` → `MVC_Sample`）。
  **`packages.config` 方式なので `nuget.exe restore` が要る**（bat が行う）
- 疎通テスト（IIS Express）:

| 対象 | ポート | 確かめること |
|---|---|---|
| `WebForms_Sample (net48)` | 51082 | `login.aspx` を GET → `__VIEWSTATE` などを付けて POST → **`menu.aspx` が 200**（未認証なら 302） |
| `MVC_Sample (net48)` | 51081 | `/Home/Login` を GET → 偽造防止トークンを付けて POST → **`/Crud1/Index` が 200** |

どちらも `aspnet_state` と DB（Northwind）が要る（[`SMOKETEST.md`](../../../SMOKETEST.md)「Web アプリ」）。
**疎通テストが見ているのはログインと 1 画面だけである。** P 層の機能（`testFxLayerP` など）は手で確かめる。

---

## 5. 対応する他の版

| 版 | 場所 | 違い |
|---|---|---|
| .NET 10 | `Samples4NetCore/Backend/MVC_Sample`（ASP.NET Core MVC。Cookie 認証） | **Web Forms 版は無い**（.NET に Web Forms が無い）。違いは [`Samples4NetCore/ANALYSIS.md`](../../Samples4NetCore/ANALYSIS.md) |
| VB | `root/programs/VB/Samples/WebApp_sample/`（2 本。疎通テストの対象） | — |

---

## 6. 落とし穴

1. **`aspnet_state` が止まっているとセッションが使えない**（2.3 節）。サービスを勝手に起動しない（`AGENTS.md`）。止まっていたら報告する。
2. **設定は `app.config` 側にある**（2.3 節）。`Web.config` に足しても効かないことがある。
3. **`packages.config` 方式**。版を上げたら `packages.config`・csproj・`bindingRedirect` の 3 か所を揃える（`ComparePackage.ps1` / `CompareRedirect.ps1`）。
4. **疎通テストが通っても、P 層の機能は確かめていない**（4 節）。
5. **`MVC_Sample` の B 層は `WSServer_sample` と別のコピー**（3 節）。片方を直しても、もう片方は変わらない。
6. **判定は状態コードで行う。** 本文の部分一致だと、IIS Express のエラー画面（500.19）の HTML に一致してしまったことがある（[`SMOKETEST.md`](../../../SMOKETEST.md)）。

---

## 7. 作業チェックリスト

- [ ] 部品 → `4_Build_CopyAssemblies.bat` → `10_Build_WebApp_sample.bat` の順でビルドする
- [ ] 設定は `app.config` に足す
- [ ] `aspnet_state` と DB の状態を確かめてから疎通する（`-Only`）
- [ ] P 層の機能に触れたら、該当の `testFxLayerP` の画面を手で確かめたか（確かめていないか）を報告する
- [ ] VB 側への追随の要否を報告する
