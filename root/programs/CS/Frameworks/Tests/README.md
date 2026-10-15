# Tests — フレームワークのテスト

**判定の方式でフォルダを分けている**（#610）。
実行と判定の手順は [`TESTING.md`](../../../TESTING.md)（`Golden`）と
[`SMOKETEST.md`](../../../SMOKETEST.md)（`Else` の一部）が一次情報。

| フォルダ | 判定の方式 | 回すもの |
|---|---|---|
| `Golden/` | **ゴールデン ファイル方式。** 出力を、コミット済みの `Result*.txt` と比較する（後退テスト） | `2_RunAllTests.ps1` |
| `NUnit/` | NUnit による単体テスト。**テスト自身が合否を出す**（#610 の後段で新設する） | — |
| `Else/` | 上のどちらでもないもの（自分で合否を出す EXE、GUI、目視の確認、補助ファイル） | 一部は `3_SmokeTest.ps1` |

| フォルダ | 中身 |
|---|---|
| `Golden/` | `TestCode` / `TestDataAccess` / `TestBatch` / `EncAndDecUtilCUI` |
| `Else/` | `TestTransmission` / `TestWebAPIClient` / `TestLog` / `EncAndDecUtil` / `LaunchJson` |

各プロジェクトの内容は [`ANALYSIS.md`](../ANALYSIS.md) 2.3 節。

---

## 置き場の決め方

**「何をテストするか」ではなく「どう合否を決めるか」で置く。**

- **出力を期待値ファイルと突き合わせる** → `Golden/`
  `2_RunAllTests.ps1` の対象に加える。**環境で変わる値を出してはならない**
  （[`Golden/TestCode/README.md`](Golden/TestCode/README.md)）
- **テスト フレームワークの Assert で判定する** → `NUnit/`
- **それ以外** → `Else/`

---

## 移動・追加するときに踏むもの

**フォルダの深さが変わると、相対パスが全部ずれる。** 移動した時に直したのは次のとおり。

| 場所 | 相対パスの基点 |
|---|---|
| `*.csproj` の `HintPath` | **プロジェクトのフォルダ**から `Infrastructure\Build_*` へ（`Golden/TestCode/net48` からは `..\..\..\..\Infrastructure`） |
| `README.md` のリンク | README の置き場所から `root/programs/*.md` へ |
| `root/programs/CS/y_Build_Test*.bat` | `root/programs/CS` から（`Frameworks\Tests\Golden\...`） |
| `2_RunAllTests.ps1` の `Result` | この `Tests` フォルダから |
| `st_Targets.ps1` の `Exe` | `root/programs/CS` から |
| `.gitignore` | リポジトリのルートから（テスト証明書の作業用コピー） |

> **期待値は `git show HEAD:<パス>` で取る。** 移動をコミットするまでは、
> HEAD に新しいパスが無いため `2_RunAllTests.ps1` は「HEAD から取得できません」になる。
> コミットの前に確かめるなら、旧パスの HEAD 版と `CompareResult.ps1` で比べる。
