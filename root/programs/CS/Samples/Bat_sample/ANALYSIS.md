# ANALYSIS.md — バッチのサンプル（CS/Samples/Bat_sample）

対象: `root/programs/CS/Samples/Bat_sample`（net48） / 最終更新: 2026-10-16

上位: [`Samples/ANALYSIS.md`](../ANALYSIS.md)（前提・呼び出しパターン・`ActionType`・設定・Legacy との関係）

---

## 1. これは何か

**コンソール EXE から、オンラインの処理と同じ流儀（B 層の `DoBusinessLogic`）でデータ アクセスする**サンプル。
B 層は `MyFcBaseLogic`（自動振り分け）を継承する。

**Open棟梁のデータ アクセスを最初に読むなら、`SimpleBatch_sample` がよい**（上位 9 節）。
B 層の 1 ファイルで、`CmnDao` / 自動生成 Dao / 個別 Dao の 3 経路を見比べられる。

**4 本とも疎通テスト（`3_SmokeTest.ps1`）の対象**で、DB（Northwind）が要る。

---

## 2. サンプル一覧

| サンプル | 見せているもの | `UOC_` メソッド |
|---|---|---|
| `SimpleBatch_sample` | **バッチの最小形。** 引数で `ActionType` を組み立て、`SelectCount` を呼んで件数を出す | `SelectCount` / `SelectAll_DT` / `_DS` / `_DR` / `_DSQL` / `Select` / `Insert` / `Update` / `Delete`、雛形の `UOC_メソッド名` |
| `RerunnableBatch_sample` | **リラン可能なバッチ。** 通常のデータ アクセス（自動生成 Dao の `S1_Insert`。`D1_Insert` に替えると動的 SQL） | `SelectPkList` / `ExecuteBatchProcess` |
| `RerunnableBatch_sample2` | 同上。**`SQLUtility` と `ExecGenerateSQL` で SQL を組み立ててまとめて送り、ラウンドトリップを減らす** | 同上 |
| `RerunnableBatch_sample3` | 同上。**`SQLUtility.GetInsertSQLParts` で `DataTable` から INSERT 文を作る**（静的 SQL のみ） | 同上 |

### 2.1 リラン可能なバッチの形

```
SelectPkList（Orders の主キーを全件、主キー順に。CmnDao ＋ SelectAllOrderID.xml）
  ↓ INTERMEDIATE_COMMIT_COUNT（= 100）件ずつに分ける
ExecuteBatchProcess（1 回 = 1 トランザクション。ReadCommitted）× 件数 / 100 回
  ↓ 主キーで Orders を引き（SelectInOrderID.xml）、Orders2 へ INSERT する
```

- **中間コミット**で、途中で落ちても済んだ分は残る
- 再開位置（`initialIndex`）は `0` 固定で、**途中から再開する処理は `todo` のまま**である
- 処理時間は `PerformanceRecorder` で測って出す（3 本で性能を比べるためのモデル）

### 2.2 引数

`SimpleBatch_sample` の引数（`readme.txt`）:

```
/Dap SQL /Mode1 individual /Mode2 static /EXROLLBACK -
```

`StringVariableOperator.GetCommandArgs('/', ...)` で分解し、**キーは大文字で引く**（`argsDic["/DAP"]`）。
`ActionType` は `/DAP` `%` `/MODE1` `%` `/MODE2` `%` `/EXROLLBACK` になる（各要素の意味は上位 6 節）。

**`RerunnableBatch_sample*` は引数を読むが使わない**（`ActionType` は `"SQL"` 固定）。
疎通テストは 4 本に同じ引数を渡している。

---

## 3. 前提

| サンプル | 要るもの |
|---|---|
| 全部 | Northwind（SQL Server。`ConnectionString_SQL`） |
| `SimpleBatch_sample` | `SqlTextFilePath` = `C:\root\files\resource\Sql`（リポジトリを `C:\root` に置く前提。上位 2-1） |
| `RerunnableBatch_sample*` | **`Orders2` 表**（Northwind の標準に無い）。各フォルダの `CREATE ORDERS2.sql` で作り、実行のたびに `DELETE FROM ORDERS2` で空にする。SQL は `.\Dao`（出力先へコピー） |

定義ファイル（`MSGDefinition.xml` / `SPDefinition.xml`）は、ファイル名だけで指定して出力先へコピーしている。

疎通テストは `Orders2` を自分で用意して空にし、**実行後に `Orders` と同じ件数（830 件）になったか**で判定する
（`st_Utility.ps1` の `$clearOrders2` / `$verifyOrders2`）。
**`Orders2` は DB を作り直すと消える。**

---

## 4. ビルドと実行

- ビルド: `5_Build_Bat_sample.bat`（4 つの sln を順に）
- 実行: Visual Studio ならプロジェクトのプロパティ → デバッグ → コマンドライン引数に `readme.txt` の文字列を入れる
- **終了前に `Console.ReadKey()` を呼ぶ。** コンソールから起動するとキー入力を待つ。
  標準入力をリダイレクトすると、ここで例外になる（疎通テストは空のファイルを標準入力に渡し、この例外を判定から除いている。[`SMOKETEST.md`](../../../SMOKETEST.md) 5 節・7.2 節）

---

## 5. 対応する他の版

| 版 | 場所 |
|---|---|
| .NET 10 | `Samples4NetCore/Legacy/Bat_sample/`（4 本。疎通テストの対象） |
| VB | `root/programs/VB/Samples/Bat_sample/`（4 本。疎通テストの対象） |

**B 層・D 層のソースは 3 つの版で独立に管理している。** 片方を直したら、他も直す。

---

## 6. 落とし穴

1. **`Orders2` が無いと Rerunnable の 3 本が落ちる**（3 節）。
2. **SQL の置き場が `SimpleBatch` と `Rerunnable*` で違う**（`C:\root\...` と `.\Dao`）。
3. **`Console.ReadKey()` で止まる・例外になる**（4 節）。出力に例外があっても、これ由来なら不具合ではない。
4. **`RerunnableBatch_sample*` に引数を変えて渡しても、振る舞いは変わらない**（2.2 節）。
5. **リランの再開は実装されていない**（2.1 節）。「リラン可能」は、中間コミットの単位で分けてあるという意味である。

---

## 7. 作業チェックリスト

- [ ] 部品 → `4_Build_CopyAssemblies.bat` → `5_Build_Bat_sample.bat` の順でビルドする
- [ ] 変えたサンプルだけを `-Only` で疎通する（DB が要る）
- [ ] Legacy 側と VB 側への追随の要否を報告する
- [ ] `ActionType` の要素を足す・変えるなら、上位 6 節の規約と、`SimpleBatch` の `switch` を合わせる
