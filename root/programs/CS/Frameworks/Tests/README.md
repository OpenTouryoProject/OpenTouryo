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
| `NUnit/` | `TestPublic` / `TestPublicNLog`（`LogIF` を NLog にした状態。別プロセスで動かすため分けている） |
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
- **csproj で `LangVersion` を `latest` にしている。** net48 の既定（C# 7.3）では、
  NUnit 4 の `Assert.Throws(() => ...)` が `TestDelegate` と `Action` の間で曖昧になり、
  net48 だけコンパイルに失敗する。`new TestDelegate(...)` で逃げると、
  NUnit 4.6 では旧形式の警告（CS0618）になる。**素直にラムダで書けばよい**
- **カルチャに依存する部品（数値・日付の文字列化）は `[SetCulture("ja-JP")]` で固定する**
- 環境変数・カルチャなど**プロセスの状態を変えたら、`[TearDown]` で戻す**
- **ファイルを書くテストは、`%TEMP%\OpenTouryoNUnit\<GUID>` を作業フォルダにし、`[TearDown]` で消す**
- **.NET では、設定を読む部品の前に `GetConfigParameter.InitConfiguration` が要る。**
  `TestSetup.cs`（`[SetUpFixture]`）が全テストの前に空の構成を与えている。
  設定を読むと気付きにくい部品もある（例: `EmbeddedResourceLoader` は内部で `Azure` キーを読む）。
  呼び忘れると `NOT_INITIALIZED` の `ArgumentException` になり、`Exists` のように
  例外を握る API では **黙って false が返る**
- **テストに設定値を与えるときは、net48 の `App.config` と .NET の `TestSetup.cs` の両方に同じ値を書く。**
  いまは `FxLog4NetConfFile`（`NUnitLogConf.xml`。`LogIF` のテストが使う）だけ
- **`LogIF` の NLog 側は `TestPublicNLog` に書く。** `LogLib` は静的コンストラクタで
  1 回だけ読まれ、同じプロセスでは log4net のまま変えられないため、別プロジェクトにしている
  （`dotnet test` はテスト アセンブリごとに別プロセスで動かす）。
  **プロセスの状態を変える設定が要るテストは、同じやり方でプロジェクトを分ける**
- **プロジェクトを足したら、`TestPublic.sln` に加え、`y_Build_TestCode_NUnit.bat` の
  `call :RunTests` と `2_RunAllTests.ps1` の対象に足す。**
  バッチの `.sln` のパスは変数を使わずに書く（`SelectTargets.ps1` がそこから
  「このバッチが建てるもの」を読むため。`%VAR%` を含むと解決できず、黙って外れる）
- **Db のテストは DB に接続しない。** `SQLUtility` と、`DamSqlSvr.ExecGenerateSQL`
  （組み立てた SQL を返すだけで、接続を使わない）で、動的パラメタライズド クエリのタグを確かめる。
  XML は `SetSqlByCommand` に文字列で渡せる（動的かどうかは中身で決まる）。
  **実行して件数を見るものは Golden の `TestDataAccess`**（DB が要る）
- 埋め込みリソースの検証には、`OpenTouryo.Public` に埋め込まれている
  `Xml/TestXml.xml` / `Xml/TestXsd.xsd` を使える
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
