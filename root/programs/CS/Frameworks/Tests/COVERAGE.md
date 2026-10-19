# COVERAGE.md — NUnit のカバレッジ

対象: `root/programs/CS/Frameworks/Tests/NUnit`（C# 側の NUnit テスト）

本書の表は、**`2_RunAllTests.ps1 -Coverage` が書く**（[`CoverageReport.ps1`](../../../CoverageReport.ps1)）。
**`COVERAGE:BEGIN` から `COVERAGE:END` の間は手で直さない。** 次の `-Coverage` で上書きされる。

> **カバレッジは判定に使わない。** どこを書き足すかを選ぶための指針である（[`README.md`](README.md)「カバレッジを見て対象を選ぶ」）。
> **Golden のテストが通している行は数えていない。** 低い数字は「NUnit で確かめていない」という意味である。

## 更新のしかた

```powershell
cd root\programs
.\2_RunAllTests.ps1 -Coverage -LocalServicesDir <LocalServicesOnDocker のフォルダ>
```

1. NUnit の 3 プロジェクト（`TestPublic`・`TestPublicNLog`・`TestPublicDb`）を 1 回のバッチで回して判定する
2. `-CrossDb` と同じく、Oracle → MySQL → PostgreSQL を **1 つずつ**「起動 → `TestPublicDb` → 停止」する
3. **全部 OK なら**、この実行で作られた net10.0 のカバレッジを合算して、表を丸ごと置き換える

- **普通の実行（`-Coverage` なし）は、本書を書き換えない。** カバレッジは画面に表示するだけ
- **1 つでも NG があれば書かない**（前の値のまま）。`-Only` / `-SkipBuild` / `-Linux` / `-CrossDb` とは同時に指定できない
- DB のコンテナは LocalServicesOnDocker の CHEATSHEET に従って扱われる（[`TESTING.md`](../../../TESTING.md) の冒頭）
- 本書が書き換わるのは `Result*.txt` と同じ副作用である。**コミットするかは人が決める**

## 読み方

- **合算**：アセンブリごとに、**どれかの計測で 1 回でも通った行**を数える（行の和集合）。
  たとえば `OpenTouryo.Public` には、`TestPublic` で通った行に加えて、
  `TestPublicNLog`（NLog の経路）と `TestPublicDb`（`BaseDam`・`DamSqlSvr` など、各 DBMS の回を含む）で通った行も入る。
  Dam（`DamManagedOdp` / `DamMySQL` / `DamPstGrS`）には、その DBMS を起動した回で通った行が入る
- **行**：通った行 / 全体の行。行は (ファイル, 行番号) で重複を除いて数える
- **分岐（下限）**：分岐のある行ごとに、**最も多くの分岐を通した計測**の数を採る。
  どの分岐を通ったかまではカバレッジのファイルに無いため、計測をまたいだ和集合は取れない。
  **実際の値はこれ以上**である
- **net10.0 だけ**を数える（net48・Linux の回はカバレッジを取らない）
- **コミット**：計測したときの HEAD。ワーキング ツリーに未コミットの変更があれば `+` が付く
  （テストの副産物の `Result*.txt` と、本書の変更は数えない）

クラス・行の単位の詳細は、各計測の `coverage.cobertura.xml`（`NUnit\TestResults` の下。`2_RunAllTests.ps1` が場所を表示する）を見る。

<!-- COVERAGE:BEGIN -->

- 計測した日時 : 2026/10/19 13:15
- コミット : aed13054+
- 合算した計測（6 件） : NUnit TestPublic (net10.0) / NUnit TestPublicNLog (net10.0) / NUnit TestPublicDb (net10.0) / TestPublicDb (net10.0) [oracle] / TestPublicDb (net10.0) [mysql] / TestPublicDb (net10.0) [postgres]

| アセンブリ | 行 | 行数（通過／全体） | 分岐（下限） | 分岐数（通過／全体） |
|---|---:|---:|---:|---:|
| OpenTouryo.DamManagedOdp | 89.1% | 261 / 293 | 83.3% | 55 / 66 |
| OpenTouryo.DamMySQL | 89.1% | 261 / 293 | 83.3% | 55 / 66 |
| OpenTouryo.DamPstGrS | 89.0% | 259 / 291 | 83.3% | 55 / 66 |
| OpenTouryo.Public | 80.0% | 6,104 / 7,632 | 68.1% | 1,405 / 2,064 |
| OpenTouryo.Public.Security | 79.2% | 2,610 / 3,297 | 80.1% | 463 / 578 |

<!-- COVERAGE:END -->
