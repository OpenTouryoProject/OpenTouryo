# ANALYSIS.md — 2 層 C/S のサンプル（CS/Samples/2CS_sample）

対象: `root/programs/CS/Samples/2CS_sample`（net48） / 最終更新: 2026-10-16

上位: [`Samples/ANALYSIS.md`](../ANALYSIS.md)（前提・呼び出しパターン・`ActionType`・設定・Legacy との関係）

---

## 1. これは何か

**WinForms / WPF のアプリから、B 層をクライアントのプロセスで動かし、DB に直接つなぐ**（2 層 C/S）サンプル。
B 層は `Business.RichClient` の `MyFcBaseLogic2CS`、画面は `MyBaseControllerWin` を継承する
（[`Business/RichClient/ANALYSIS.md`](../../Frameworks/Infrastructure/Business/RichClient/ANALYSIS.md)）。

**すべて GUI なので、疎通テスト（`3_SmokeTest.ps1`）の対象外**である。
ビルドが通ることまでしか自動では確かめられず、動作はリリースの手作業項目で見る（[`SMOKETEST.md`](../../../SMOKETEST.md)「対象外」）。

---

## 2. サンプル一覧

| サンプル | 種別 | 見せているもの | SQL の置き場（`SqlTextFilePath`） |
|---|---|---|---|
| `2CSClientWin_sample` | WinForms | **2 層 C/S の基本形。** `Splash` → `Login` → `Form1`。`LayerB` / `LayerD` / 自動生成 Dao（`DaoShippers`） | `C:\root\files\resource\Sql`（絶対パス） |
| `2CSClientWPF_sample` | WPF | 同上の WPF 版（`Window1`） | 同上 |
| `GenDaoAndBatUpd_sample` | WinForms | **自動生成 Dao（`DaoGen_Tool` の出力）とバッチ更新。** `LayerB_Static` / `LayerB_Dynamic` / `LayerB_BatUpd` の 3 つの B 層。Northwind の `Categories` / `Products` / `Suppliers` | **埋め込みリソース**（`GenDaoAndBatUpd_sample.Dao`。`MyBaseDao.UseEmbeddedResource = true`） |
| `TimeStamp_sample` | WinForms | **タイムスタンプ（楽観排他）付きの自動生成 Dao**（`ts_test_table1` … `C`） | `.\Dao`（出力先へコピーした SQL） |
| `CustCtrl_sample` | WinForms | **カスタム コントロール**（`WinCustomTextBox` / `WinCustomMaskedTextBox`）のデザイン時プロパティ。**`Readme.txt` が事実上のプロパティの仕様書** | DB を使わない |
| `AsyncEvent_sample` | WinForms ＋ WPF | **プロセス間の非同期イベント**（`AsyncEventFx`、名前付きパイプ）。`test-winx2&wpfx2.bat` が 4 プロセスを起動し、互いに通知する | DB を使わない |

### 2.1 SQL の置き場が 3 通りある

| 方式 | 設定 | サンプル | 注意 |
|---|---|---|---|
| 共通の SQL フォルダ | `SqlTextFilePath` = `C:\root\files\resource\Sql` | `2CSClientWin` / `2CSClientWPF` | リポジトリを `C:\root` に置く前提（上位 2-1） |
| 出力先へコピー | `SqlTextFilePath` = `.\Dao`、csproj で `Content`（`CopyToOutputDirectory=Always`） | `TimeStamp_sample` | 作業ディレクトリ基準。実行ファイルの場所から起動する |
| 埋め込みリソース | `SqlTextFilePath` = `<既定の名前空間>.Dao`、csproj で `EmbeddedResource` | `GenDaoAndBatUpd_sample` | **SQL を足したら `EmbeddedResource` に足さないと実行時に見つからない** |

**自動生成 Dao のサンプル（`GenDaoAndBatUpd` / `TimeStamp`）は、Dao と SQL を同じフォルダに置いている。**
メソッドと SQL ファイル名の対応（`S1_Insert` / `D2_Select` …）は [`Samples/ANALYSIS.md`](../ANALYSIS.md) 9 節。

---

## 3. 前提

| サンプル | 要るもの |
|---|---|
| `2CSClientWin` / `2CSClientWPF` / `GenDaoAndBatUpd` | Northwind（SQL Server。`ConnectionString_SQL`） |
| `TimeStamp_sample` | Northwind に**タイムスタンプ列付きの表**を作る（`root/files/resource/Sql/sqlserver/DaoAutoGen_TimeStamp/CreateTable/*.sql`） |
| `2CSClientWin` / `2CSClientWPF` | `C:\root\files\resource\...`（定義ファイル・SQL） |

`TimeStamp_sample/Readme.txt` の要点（原文のまま）:

- 主キー設定は参照・更新・削除では必須。
- 静的か？動的か？はその際にタイムスタンプが必須か否か。
- 動的では、参照・削除時に val を条件に指定する（更新時は条件に指定しない）。

---

## 4. ビルド

`5_Build_2CS_sample.bat`（6 つの sln を順に）。部品を先にビルドし、`4_Build_CopyAssemblies.bat` で `Frameworks\Infrastructure\Build\` を作っておく。

---

## 5. 対応する他の版

| 版 | 場所 | 違い |
|---|---|---|
| .NET 10 | `Samples4NetCore/Legacy/2CS_sample/` | **`AsyncEvent_sample` は移植していない。** 他の 5 つはある |
| VB | `root/programs/VB/Samples/2CS_sample/` | 6 つともある |

**B 層・D 層のソースは、Legacy 側とほぼ同じで独立に管理している**（上位 11 節）。片方を直したら、もう片方も直す。

---

## 6. 落とし穴

1. **自動では動作を確かめられない**（1 節）。直したら、何を手で確かめたか（確かめていないか）を報告する。
2. **SQL の置き場がサンプルで違う**（2.1 節）。埋め込みにしている `GenDaoAndBatUpd_sample` では、csproj への登録を忘れると実行時に落ちる。
3. **`TimeStamp_sample` は Northwind の標準に無い表を使う**（3 節）。DB を作り直すと消える。
4. **`CustCtrl_sample/Readme.txt` はカスタム コントロールの仕様書を兼ねる。** コントロールのプロパティを変えたら、こちらも直す。

---

## 7. 作業チェックリスト

- [ ] 部品 → `4_Build_CopyAssemblies.bat` → `5_Build_2CS_sample.bat` の順でビルドする
- [ ] 自動生成 Dao は手で直さず、`DaoGen_Tool` で作り直す
- [ ] Legacy 側（`AsyncEvent_sample` 以外）と VB 側への追随の要否を報告する
- [ ] GUI を手で確かめていなければ、そう報告する
