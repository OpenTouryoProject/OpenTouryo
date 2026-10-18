# ANALYSIS.md — Web サービスのサンプル（CS/Samples/WS_sample）

対象: `root/programs/CS/Samples/WS_sample`（net48） / 最終更新: 2026-10-16

上位: [`Samples/ANALYSIS.md`](../ANALYSIS.md)（前提・呼び出しパターン・`ActionType`・設定・Legacy との関係）

---

## 1. これは何か

**3 層の構成**（クライアント → 通信 → サーバの B 層）のサンプル。サーバ側・I/F 型・クライアント・Web API に分かれる。

```
WSClient_sample/*（WinForms / WPF）
   │ CallController.Invoke("testWebService4", ...)       ← TMProtocolDefinition.xml で解決
   ▼
ServiceInterface（Frameworks/Infrastructure/ServiceInterface。net48）
   │ ASPNETWebService（WebAPIControllerForFx）/ WCFService（net.tcp:7777）
   │ InProcessNameService → WSServer_sample.Business.LayerB   ← TMInProcessDefinition.xml
   ▼
WSServer_sample（B 層・D 層）── WSIFType_sample（引数・戻り値の型。両側で共有）
```

これとは別に、**`ASPNETWebService`（本フォルダの方）は JSON の Web API（ResourceServer）**で、
`WSServer_sample` の B 層を個別の Web メソッドとして公開する（#566）。

| プロジェクト | 種別 | 役割 |
|---|---|---|
| `WSServer_sample` | クラス ライブラリ | **サーバ側の B 層・D 層。** `LayerB`（`MyFcBaseLogic`）は `SimpleBatch_sample` と同じ形（`SelectCount` / `SelectAll_*` / `Select` / `Insert` / `Update` / `Delete`）。`Common/WebApiParams`・`Shipper` は Web API 用 |
| `WSIFType_sample` | クラス ライブラリ | **引数・戻り値の型**（`TestParameterValue` / `TestReturnValue`）。サーバとクライアントで共有する。`[Serializable]` |
| `WSClient_sample/*` | WinForms / WPF | **3 層型のクライアント**（2 節） |
| `ASPNETWebService` | ASP.NET Web API | **ResourceServer**（3 節） |

---

## 2. クライアント（`WSClient_sample/`）

| サンプル | 種別 | 参照 | 見せているもの |
|---|---|---|---|
| `WSClientWin_sample` | WinForms | `WSIFType` / `WSServer` | 基本形。`Splash` → `Login` → `Form1`。インプロセスとリモートを定義で切り替える |
| `WSClientWPF_sample` | WPF | `WSIFType` / `WSServer` | 同上の WPF 版 |
| `WSClientWinCone_sample` | WinForms | `WSIFType` のみ | **B 層を参照しない**（リモート呼び出しだけの構成）。`TMInProcessDefinition` を持たない |
| `WSClientWin2_sample` | WinForms | 部品のみ | **リッチ クライアントの P 層の機能**（同期・非同期の処理、子画面、戻り値を返す画面 `ByReturn`、ユーザ コントロールの入れ子）。B 層は呼ばない |

### 2.1 通信の定義

`TMProtocolDefinition.xml`（`WSClientWin_sample` の例）:

| id | protocol | 行き先 |
|---|---|---|
| `testInProcess` | 1（インプロセス） | 同じプロセスの `WSServer_sample`（`TMInProcessDefinition.xml`） |
| `testWebService` / `testWebService2` | 2 / 3（asmx / WCF-HTTP） | **コメントアウト** |
| `testWebService3` | 4（WCF-TCP） | `net.tcp://localhost:7777/WCFService/WCFTCPSvcForFx/` |
| `testWebService4` | 5（ASP.NET Web API） | `https://localhost/WebAPIControllerForFx`（`WSClientWinCone_sample` は `https://localhost:44349/...`） |

- `_` で始まる id（`_testWebService3` など）は、`url_ref` / `prop_ref` で共通の URL・プロパティを参照する書き方の例
- 定義ファイルは**物理ファイルと埋め込みリソースの両方**を用意し、`app.config` でどちらを読むかを切り替える
  （`TMProtocolDefinition.xml` と、`<既定の名前空間>.TMProtocolDefinition2.xml` など）

### 2.2 実行の前提

- リモート（4・5）を使うなら、先に **`ServiceInterface` のホストを起動する**（`WCFService.exe`、または IIS 上の `ASPNETWebService`）
- **GUI なので疎通テストの対象外**（[`SMOKETEST.md`](../../../SMOKETEST.md)「対象外」）。リリースの手作業項目

---

## 3. `ASPNETWebService`（ResourceServer）

| コントローラ | ルート | 内容 |
|---|---|---|
| `JsonController` | `api/json` | `WSServer_sample` の B 層を、CRUD の個別 Web メソッド（`SelectCount` / `SelectAll_DT` / `_DS` / `_DR` / `_DSQL` / `Select` / `Insert` / `Update` / `Delete`）として公開する。`MVC_Sample` の `Crud1Controller` と同じ処理 |
| `BatchUpdateController` | `api/batchupdate` | **DTO（`DTTables`）の往復とバッチ更新**。`RowState` と `Original` を保ったまま送り返し、`Original` を WHERE に入れて楽観排他する（#567 / #570） |
| `SampleDataController` / `ValuesController` | `api/values` ほか | 動作確認用 |

- 認証は `MyBaseAsyncApiController` 属性（`EnumHttpAuthHeader.None | Bearer`。**認証が無くても通し**、Bearer があればアクセス トークンを検証する）
- OWIN の `Startup` を持つ
- **net48 版には OpenAPI（IDL）が無い**（クラシック ASP.NET のため。#580。.NET 10 版だけが `/openapi/v1.json` を出す）

### 3.1 疎通テスト

`TestWebAPIClient (net48)` が、IIS Express（ポート 51087）で本プロジェクトを起動し、1 本で 3 つを確かめる
（CRUD の一巡で件数が戻る、DTO の往復で `RowState` / `Original` が残る、古い版の更新が弾かれる）。
**DB（Northwind）が要る。** 詳細は [`SMOKETEST.md`](../../../SMOKETEST.md)「DTO を使用したバッチ更新（WebAPI Client）」。

---

## 4. ビルド

| bat | 何をするか |
|---|---|
| `6_Build_WSSrv_sample.bat` | `WSServer_sample`（と `WSIFType_sample`）をビルドし、`WS_sample\Temp\` → **`WS_sample\Build\`** へ複写。続けて `ASPNETWebService` |
| `7_Build_Framework_WS.bat` | `WS_sample\Build\` を `ServiceInterface\WCFService\dll\` へ複写し、`ServiceInterface` をビルド |
| `8_Build_WSClnt_sample.bat` | `WSClient_sample/*` の 4 つの sln |

**`WS_sample\Build\` は中継点である。** `ServiceInterface`・本フォルダの `ASPNETWebService`・クライアントは、
`WSServer_sample.dll` / `WSIFType_sample.dll` を**ここから `HintPath` で拾う**。
**`6_` を飛ばすと `7_` 以降が失敗するか、古い DLL を見る。** `Build\` と `Temp\` は生成物でコミットしない。

---

## 5. 対応する他の版

| 版 | 場所 | 違い |
|---|---|---|
| .NET 10 | `Samples4NetCore/Legacy/WS_sample/`（`WSServer` / `WSIFType` / クライアント 3 種）、`Samples4NetCore/Backend/ASPNETWebService` | **`WSClientWinCone_sample` は移植していない。** .NET 10 の通信は `InProcess` だけ（リモート プロトコルが無い） |
| VB | `root/programs/VB/Samples/WS_sample/` | `ASPNETWebService` は無い |

**B 層・D 層のソースは版ごとに独立に管理している。** 片方を直したら、他も直す。

---

## 6. 落とし穴

1. **`WS_sample\Build\` を経由する**（4 節）。ビルドの順は `6_` → `7_` → `8_`。
2. **`ServiceInterface` はこのサンプルの DLL を参照する**（基盤側のホストが、サンプルのビルドに依存している）。
3. **`MVC_Sample` の B 層は `WSServer_sample` とは別のコピー**（[`WebApp_sample/ANALYSIS.md`](../WebApp_sample/ANALYSIS.md) 3 節）。`JsonController` を直すときは `WSServer_sample` 側。
4. **クライアントは疎通テストで確かめていない**（2.2 節）。
5. **定義ファイルは物理と埋め込みの 2 通り**（2.1 節）。`app.config` が指している方を直す。

---

## 7. 作業チェックリスト

- [ ] 部品 → `4_Build_CopyAssemblies.bat` → `6_` → `7_` → `8_` の順でビルドする
- [ ] B 層を変えたら、`TestWebAPIClient (net48)` を `-Only` で疎通する（DB が要る）
- [ ] `WSIFType_sample` の型を変えたら、サーバとクライアントの両方をビルドし直す
- [ ] Legacy 側・VB 側への追随の要否を報告する
- [ ] クライアントを手で確かめていなければ、そう報告する
