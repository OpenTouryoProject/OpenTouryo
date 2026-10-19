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

1. NUnit の 4 プロジェクト（`TestPublic`・`TestPublicNLog`・`TestPublicDb`・`TestFramework`）を 1 回のバッチで回して判定する
2. `-Linux` と同じく、`TestPublic` を Linux のコンテナで回す（**docker が要る**）
3. `-CrossDb` と同じく、Oracle → MySQL → PostgreSQL を **1 つずつ**「起動 → `TestPublicDb` → 停止」する
4. **全部 OK なら**、この実行で作られた net10.0 のカバレッジを合算して、表を丸ごと置き換える

- **普通の実行（`-Coverage` なし）は、本書を書き換えない。** カバレッジは画面に表示するだけ
- **1 つでも NG があれば書かない**（前の値のまま）。`-Only` / `-SkipBuild` / `-Linux` / `-CrossDb` とは同時に指定できない
- DB のコンテナは LocalServicesOnDocker の CHEATSHEET に従って扱われる（[`TESTING.md`](../../../TESTING.md) の冒頭）
- 本書が書き換わるのは `Result*.txt` と同じ副作用である。**コミットするかは人が決める**

## 読み方

- **合算**：アセンブリごとに、**どれかの計測で 1 回でも通った行**を数える（行の和集合）。
  たとえば `OpenTouryo.Public` には、`TestPublic` で通った行に加えて、
  `TestPublicNLog`（NLog の経路）と `TestPublicDb`（`BaseDam`・`DamSqlSvr` など、各 DBMS の回を含む）で通った行も入る。
  Dam（`DamManagedOdp` / `DamMySQL` / `DamPstGrS`）には、その DBMS を起動した回で通った行が入る。
  **Linux の回の行も入る**（`DigitalSignECDsaOpenSsl` など、OpenSSL を使う Linux 専用のコード）
- **`OpenTouryo.Framework` は、アセンブリ全体ではなく、`TestFramework` の対象モジュールごとの行と、その計の行で出す。**
  対象は単体で使える部品（`Authentication`・`Exceptions`）だけで、親クラス（`Business`・`Dao`・`Presentation`・`Transmission`）は
  組み合わせて使うもので、NUnit では通さない（疎通でカバレッジを取る案は別件）。全体の割合を出しても低く出るだけで、指針にならない。
  対象モジュールの一覧は `2_RunAllTests.ps1` の TestFramework の定義（`CoverageModules`）が持つ。
  **`CmnJwtToken`・`SAML2Client` は、設定（`CmnClientParams` 等）を読む版を対象にしていない**ので、その分だけ低く出る
- **行**：通った行 / 全体の行。行は (ファイル, 行番号) で重複を除いて数える
- **分岐（下限）**：分岐のある行ごとに、**最も多くの分岐を通した計測**の数を採る。
  どの分岐を通ったかまではカバレッジのファイルに無いため、計測をまたいだ和集合は取れない。
  **実際の値はこれ以上**である
- **net10.0 だけ**を数える（net48 の回はカバレッジを取らない）。Linux の回も net10.0 である
- **コミット**：計測したときの HEAD。ワーキング ツリーに未コミットの変更があれば `+` が付く
  （テストの副産物の `Result*.txt` と、本書の変更は数えない）

クラス・行の単位の詳細は、各計測の `coverage.cobertura.xml`（`NUnit\TestResults` の下。`2_RunAllTests.ps1` が場所を表示する）を見る。

<!-- COVERAGE:BEGIN -->

- 計測した日時 : 2026/10/19 22:03
- コミット : ff9b6c76+
- 合算した計測（8 件） : NUnit TestPublic (net10.0) / NUnit TestPublicNLog (net10.0) / NUnit TestPublicDb (net10.0) / NUnit TestFramework (net10.0) / NUnit TestPublic (Linux) / TestPublicDb (net10.0) [oracle] / TestPublicDb (net10.0) [mysql] / TestPublicDb (net10.0) [postgres]

| アセンブリ（: モジュール） | 行 | 行数（通過／全体） | 分岐（下限） | 分岐数（通過／全体） |
|---|---:|---:|---:|---:|
| OpenTouryo.DamManagedOdp | 89.1% | 261 / 293 | 83.3% | 55 / 66 |
| OpenTouryo.DamMySQL | 89.1% | 261 / 293 | 83.3% | 55 / 66 |
| OpenTouryo.DamPstGrS | 89.0% | 259 / 291 | 83.3% | 55 / 66 |
| OpenTouryo.Framework : Authentication/AuthenticationHeader.cs | 97.5% | 78 / 80 | 92.3% | 24 / 26 |
| OpenTouryo.Framework : Authentication/SAML2Enum.cs | 100.0% | 142 / 142 | 100.0% | 56 / 56 |
| OpenTouryo.Framework : Authentication/SAML2Bindings.cs | 91.1% | 422 / 463 | 57.6% | 83 / 144 |
| OpenTouryo.Framework : Authentication/SAML2Client.cs | 61.5% | 110 / 179 | 42.0% | 21 / 50 |
| OpenTouryo.Framework : Authentication/CmnJwtToken.cs | 32.4% | 56 / 173 | 26.0% | 13 / 50 |
| OpenTouryo.Framework : Authentication/AccessToken.cs | 86.3% | 63 / 73 | 80.8% | 21 / 26 |
| OpenTouryo.Framework : Authentication/IdToken.cs | 92.2% | 83 / 90 | 97.1% | 33 / 34 |
| OpenTouryo.Framework : Authentication/ResponseObject.cs | 85.0% | 51 / 60 | 83.3% | 15 / 18 |
| OpenTouryo.Framework : Authentication/JwtAssertion.cs | 97.7% | 128 / 131 | 90.0% | 18 / 20 |
| OpenTouryo.Framework : Authentication/RequestObject.cs | 96.5% | 137 / 142 | 63.5% | 33 / 52 |
| OpenTouryo.Framework : Authentication/ClaimsInRO.cs | 100.0% | 34 / 34 | 85.7% | 12 / 14 |
| OpenTouryo.Framework : Exceptions/BusinessApplicationException.cs | 100.0% | 16 / 16 | - | 0 / 0 |
| OpenTouryo.Framework : Exceptions/BusinessSystemException.cs | 100.0% | 11 / 11 | - | 0 / 0 |
| OpenTouryo.Framework : Exceptions/FrameworkException.cs | 100.0% | 11 / 11 | - | 0 / 0 |
| **OpenTouryo.Framework（対象モジュールの計）** | 83.6% | 1,342 / 1,605 | 67.1% | 329 / 490 |
| OpenTouryo.Public | 80.5% | 6,145 / 7,632 | 68.4% | 1,411 / 2,064 |
| OpenTouryo.Public.Security | 92.1% | 3,037 / 3,297 | 82.5% | 477 / 578 |

<!-- COVERAGE:END -->
