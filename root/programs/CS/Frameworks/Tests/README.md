# Tests — フレームワークのテスト

**判定の方式でフォルダを分けている**（#610）。
実行と判定の手順は [`TESTING.md`](../../../TESTING.md)（`Golden` / `NUnit`）と
[`SMOKETEST.md`](../../../SMOKETEST.md)（`Else` の一部）が一次情報。

| フォルダ | 判定の方式 | 回すもの |
|---|---|---|
| `Golden/` | **ゴールデン ファイル方式。** 出力を、コミット済みの `Result*.txt` と比較する（後退テスト） | `2_RunAllTests.ps1` |
| `NUnit/` | NUnit による単体テスト。**テスト自身が合否を出す** | `2_RunAllTests.ps1` |
| `Else/` | 上のどちらでもないもの（自分で合否を出す EXE、GUI、目視の確認、補助ファイル） | 一部は `3_SmokeTest.ps1` |

| フォルダ | 中身 |
|---|---|
| `Golden/` | `TestCode` / `TestDataAccess` / `TestBatch` / `EncAndDecUtilCUI` |
| `NUnit/` | `TestPublic` |
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

## NUnit のテストを書き足すとき（#610）

`NUnit/TestPublic` は `Touryo.Infrastructure.Public` の単体テスト。
**名前空間ごとにフォルダを切り、クラスごとに `<クラス名>Tests.cs` を置く**（例: `Str/StringCheckerTests.cs`）。

```
y_Build_TestCode_NUnit.bat     … ビルド → net48 / net10.0 で実行 → TestResults\*.trx
.\2_RunAllTests.ps1 -Only NUnit
```

- **1 つの csproj で net48 / net10.0 をマルチ ターゲットにしている。**
  Golden のように 2 プロジェクトへ `Link` で共有する作りではない
- **TFM で結果が違う箇所は `#if NETCOREAPP` で書き分け、テスト名と `<summary>` に違いを書く。**
  例: `StringConverter` は net48 が VB の `StrConv`、.NET が Zipangu で、実装が違う
- **net48 の既定は C# 7.3。** NUnit 4 の `Assert.Throws(() => ...)` は
  `TestDelegate` と `Action` の間で曖昧になり、net48 だけコンパイルに失敗する。
  **`new TestDelegate(() => ...)` で渡す**
- フレームワークは `HintPath` で参照する。**`Public` が使うパッケージは、
  テストで通る範囲だけ csproj にも並べる**（推移的依存が効かないため）
- **読み違えやすい挙動を見つけたら、それ自体をテストにする**
  （例: `IsNumbers("")` は true、`IsNumeric("")` は false）。
  不具合と思われるものは、テストで固めずに報告する

### カバレッジを見て対象を選ぶ

**どこを書き足すかは、カバレッジの低いところから選ぶ。** 判定には使わない。

`2_RunAllTests.ps1` が net10.0 の結果でアセンブリごとの行カバレッジを表示し、
`coverage.cobertura.xml`（クラス・行の単位）の場所を出す。
**計測するのは `OpenTouryo.*` だけ**（`coverlet.runsettings`）。

> **Golden のテストが通している行は数えていない。** コンソール EXE の出力比較であり、
> カバレッジの計測には載っていない。低い数字は「NUnit で確かめていない」という意味である。

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
