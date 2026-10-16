# ANALYSIS.md — 業務フレームワーク（リッチ クライアント）（Infrastructure/Business/RichClient）

対象: `root/programs/CS/Frameworks/Infrastructure/Business/RichClient` / 最終更新: 2026-10-16

上位: [`Business/ANALYSIS.md`](../ANALYSIS.md) / [`Infrastructure/ANALYSIS.md`](../../ANALYSIS.md) / 全体: [`Frameworks/ANALYSIS.md`](../../../ANALYSIS.md)

---

## 1. これは何か

`OpenTouryo.Business.RichClient`（名前空間 `Touryo.Infrastructure.Business.RichClient`）。
**WinForms / WPF のアプリ向けの、親クラス 2（テンプレート）**である。
親クラス 1 は `Framework.RichClient`（[`Framework/ANALYSIS.md`](../../Framework/ANALYSIS.md) 9 節）。

サーバ側の `Business` と同じく、**利用者がソースを取り込んで改造する前提**で、NuGet には無い。

想定する構成は 2 つある。

| 構成 | B 層が動く場所 | 使うクラス |
|---|---|---|
| **2 層 C/S** | クライアントのプロセス（DB に直接つなぐ） | `MyFcBaseLogic2CS` / `MyBaseLogic2CS` |
| **3 層**（Web サービス経由） | サーバ（`CallController` で呼ぶ） | サーバ側の `MyFcBaseLogic`（[`Business/ANALYSIS.md`](../ANALYSIS.md)） |

画面（`MyBaseControllerWin`）と非同期処理（`MyBaseAsyncFunc`）は、どちらの構成でも使う。

---

## 2. 構成

| フォルダ | クラス | 親クラス 1 | 一言で |
|---|---|---|---|
| `Presentation/` | `MyBaseControllerWin` | `BaseControllerWin` | 画面の親クラス 2。`MyCreatePrefixAndEvtHndHt` で接頭辞とイベント ハンドラを対応付け、`UOC_CMNFormInit` / `UOC_PreAction` / `UOC_ABEND` などで共通の前後処理を書く |
| `Business/` | `MyFcBaseLogic2CS`（自動振り分け付き）`MyBaseLogic2CS` | `BaseLogic2CS` | 2 層 C/S の B 層の親クラス 2 |
| `Asynchronous/` | `MyBaseAsyncFunc` | `BaseAsyncFunc` | 画面を固めずに処理を走らせる。`Start()` で開始、`UOC_Pre` / `UOC_After` / `UOC_ABEND` / `UOC_Finally` |
| `Util/` | `RcMyCmnFunction` | — | 共通関数 |

### 2.1 データ プロバイダの選択

`MyFcBaseLogic2CS` / `MyBaseLogic2CS` の `UOC_ConnectionOpen` は、**サーバ側の `MyFcBaseLogic` と同じ表**で選ぶ
（`SQL` / `OLE`（net48 のみ）/ `ODB` / `ODP` / `MCN` / `NPS`（net10.0 のみ）。
**どれにも当たらなければ SQL Server**。[`Business/ANALYSIS.md`](../ANALYSIS.md) 3.1 節）。
**片方を直したら、もう片方（サーバ側を含む 4 クラス）も揃えること。**

---

## 3. TFM 差

| | net48 | net10.0 |
|---|---|---|
| TFM | `v4.8` | `net10.0-windows7.0`（`UseWindowsForms` / `UseWPF`） |
| 外しているもの | なし | `Properties/AssemblyInfo.cs` のみ |
| 参照（`HintPath`） | `Build_net48\` の `Public` / `Framework` / `Framework.RichClient` / `Business` / `Dam*` | `Build_netcore100\net10.0\` の同上、`Framework.RichClient` は `net10.0-windows7.0\` |

**サーバ側と違い、`MyBaseLogic2CS`（自動振り分けなし）も net10.0 に入る。**
サーバ側の `MyBaseLogic` は net48 専用である。

**`Business` の DLL を参照する**ので、`3_Build_Business_*.bat` の後に `3_Build_BusinessRichClient_*.bat` を回す。

---

## 4. 落とし穴

1. **プロバイダ選択のコードが 4 クラスに複製されている**（2.1 節）。1 つだけ直すと、構成によって接続先が変わる。
2. **`Business` を先にビルドしないと、古い `OpenTouryo.Business.dll` を見る**（3 節）。
3. net10.0 版は **Windows でしかビルド・実行できない**（`net10.0-windows7.0`）。

---

## 5. 作業チェックリスト

- [ ] 2 層 C/S と 3 層のどちらの構成の話か（1 節）
- [ ] サーバ側のテンプレート（`MyFcBaseLogic` など）と揃える必要は無いか（2.1 節）
- [ ] ビルドは `2_` → `3_Build_Business_*` → `3_Build_BusinessRichClient_*`
- [ ] サンプル（`Samples/2CS_sample`、`Samples4NetCore/Legacy` の 2CS）の疎通で確かめる
- [ ] VB 版（`root/programs/VB/Frameworks/Infrastructure/Business/RichClient`）の追随の要否を報告する
