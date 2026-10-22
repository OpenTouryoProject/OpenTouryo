# Tests — フレームワークのテスト

**判定の方式でフォルダを分けている**（#610）。
実行と判定の手順は [`TESTING.md`](../../../TESTING.md)（`Golden` / `NUnit`）と
[`SMOKETEST.md`](../../../SMOKETEST.md)（`Else` の一部）が一次情報。

> **DB を使うテスト（`TestDataAccess` / `TestPublicDb`）のコンテナは、LocalServicesOnDocker の
> [`CHEATSHEET.md`](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker/blob/master/CHEATSHEET.md) に従って扱う。**
> **メモリを節約しないと、最悪の場合、コンテナがシステムごとハングさせる。**
> 必要なものだけを名前で起動し、必須でないものを同時に複数起動せず、使い終わったら止める
> （要点は [`TESTING.md`](../../../TESTING.md) の冒頭）。

| フォルダ | 判定の方式 | 回すもの |
|---|---|---|
| `Golden/` | **ゴールデン ファイル方式。** 出力を、コミット済みの `Result*.txt` と比較する（後退テスト） | `2_RunAllTests.ps1` |
| `NUnit/` | NUnit による単体テスト。**テスト自身が合否を出す** | `2_RunAllTests.ps1` |
| `Else/` | 上のどちらでもないもの（自分で合否を出す EXE、GUI、目視の確認、補助ファイル） | 一部は `3_SmokeTest.ps1` |

上位: [`Frameworks/ANALYSIS.md`](../ANALYSIS.md)（全体の俯瞰・ビルド）

---

## 各プロジェクト

| 区分 | 名前 | 内容 | 専用の文書 |
|---|---|---|---|
| `Golden` | `TestCode` | コンソール EXE。`Program.cs` から `Test*.Root()` を順に呼ぶ。期待値は `Result48.txt` / `ResultCore100.txt` と比較。**DB には接続しない** | [`README.md`](Golden/TestCode/README.md) |
| `Golden` | `TestDataAccess` | データ アクセスのテスト（#520）。`TestCode` と同じ構成だが**DB に接続する**。`/MODE` で対象の DBMS を切り替える（既定は SQL Server のみ） | [`README.md`](Golden/TestDataAccess/README.md) |
| `Golden` | `TestBatch` | バッチ起動の確認 | — |
| `Golden` | `EncAndDecUtilCUI` | 暗号・署名ユーティリティの CUI 確認。結果ファイルは net48 / net10.0 / Linux の 3 つ。`jose-jwt` との相互運用も見る | [`Public/Security/ANALYSIS.md`](../Infrastructure/Public/Security/ANALYSIS.md) 8 節 |
| `NUnit` | `TestPublic` | `Public` 層の単体テスト（#610）。**1 つの csproj で net48 / net10.0 をマルチ ターゲット**にする。テスト自身が合否を出す | 本書 |
| `NUnit` | `TestPublicNLog` | `LogIF` を NLog に切り替えた状態のテスト（#610）。**使うライブラリはプロセスで 1 回しか決まらない**ため、別プロジェクト（別プロセス）にしている | 本書 |
| `NUnit` | `TestPublicDb` | Dam のクロス DB テスト（#610）。**DB に接続する**ので別プロジェクト。SQL Server は必須、他の DBMS は接続できなければスキップ | 本書 |
| `NUnit` | `TestFramework` | `OpenTouryo.Framework` のうち**単体で使える部品**（`Authentication` の鍵・値を引数で受ける版、`Exceptions`）。親クラスは組み合わせて使うので疎通に任せる | 本書 |
| `Else` | `TestTransmission` | 通信制御の接続オプション（#546）。**オリジンとプロキシを自前で立て**、`CallController` から呼んで、要求に反映されているかを見る。**期待値ファイルを持たず、自分で合否を出す**（疎通テストから呼ばれる） | [`SMOKETEST.md`](../../../SMOKETEST.md) 3 節 |
| `Else` | `TestWebAPIClient` | Web API の非ブラウザ クライアント（疎通テストから呼ばれる） | [`SMOKETEST.md`](../../../SMOKETEST.md) |
| `Else` | `TestLog` | ログ出力の確認（log4net / NLog、1〜3） | — |
| `Else` | `EncAndDecUtil` | 暗号・署名ユーティリティの GUI 確認 | — |
| `Else` | `LaunchJson` | リモート デバッグ用の `launch.json` の例（テストではない） | — |

**`TestCode` と `TestDataAccess` は専用の `README.md` を持つ。**
「何をテストしているか」「ケースを書き足すときの決まり」「ケース設計の根拠」はそちらが一次情報。
[`TESTING.md`](../../../TESTING.md) は Golden と NUnit を含む全体の運用を扱い、この詳細は持たない。

**Golden のテストを書き足す前に `TestCode/README.md` を読むこと。**
結果ファイルとの比較で判定するため、**環境で変わる値を出してはならない**という制約がある
（例外はメッセージを出さず型名だけ、日時は固定値、`csproj` は net48 / core100 の両方を直す）。

### フレームワークは DLL で参照する

**テスト プロジェクトはフレームワークを `ProjectReference` ではなく `HintPath` で参照する**
（net10.0 は `Infrastructure/Build_netcore100/net10.0/*.dll`、net48 は `Infrastructure/Build_net48/*.dll`。
`Else/TestLog` だけは `Infrastructure/Build/*.dll` で、`4_Build_CopyAssemblies.bat` の後でないとビルドできない）。
→ **フレームワークを先にビルドしないと、テストは古い DLL を見る。**

Golden は `net48` / `core100` の 2 プロジェクトが `..\*.cs` を `Link` で共有する（実体は 1 つ）。
NUnit は 1 つの csproj でマルチ ターゲットにする（下の「NUnit のテストを書き足すとき」）。

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
- **フォルダ名を `Log` にしない。** `.gitignore` の `[Ll]og/`（ログ出力先の除外）に当たり、
  **中のテストが黙って Git から外れる**（ビルドは通るので気付かない）。ログのテストは `Logging/` に置く。
  新しいフォルダを作ったら `git status` で未追跡に出ることを確かめる
- **OS の状態を変えるテストは、権限が無ければ `Assert.Ignore` でスキップし、`[TearDown]` で必ず戻す。**
  例: `Logging/CustomEventLogTests.cs`（net48 専用。専用のログ `OTNUnitLog` を作って消す。管理者のみ）。
  **`CustomEventLog.Delete` はソースが属するログごと消す**ので、既存のログのソースを渡さない
- **Db のテストは DB に接続しない。** `SQLUtility` と、`DamSqlSvr.ExecGenerateSQL`
  （組み立てた SQL を返すだけで、接続を使わない）で、動的パラメタライズド クエリのタグを確かめる。
  XML は `SetSqlByCommand` に文字列で渡せる（動的かどうかは中身で決まる）。
  **実行して件数を見るものは Golden の `TestDataAccess`**（DB が要る）
- **DB に接続するテストは `TestPublicDb` に書く。** 同じテストを DBMS ごとに回す（`TestFixtureSource`）。
  DBMS の差（Dam・接続文字列・囲い文字・パラメタ記号・型名）は `DbProvider.cs` に集める。
  **既存の表は使わず**、一意な名前の表を作って最後に落とす。数値は `Convert` で比べる（Oracle の NUMBER は decimal）。
  Oracle は空文字を NULL として扱うので、空文字を値に使わない
- **Framework のテストは `TestFramework` に書く。対象は単体で使える部品だけ**（他の部品と組み合わせずに、鍵や値を引数で渡して呼べるもの）。
  認証は**鍵・iss・aud・証明書を引数で受ける版**を使い、設定（`CmnClientParams`・`OAuth2AndOIDCParams`）を読む版は使わない
  （配備の設定に依るため。Golden の `TestCode` が見ている）。サーバが要るもの（`OAuth2AndOIDCClient`・`JwkSetStore`）と親クラスは対象外
- 埋め込みリソースの検証には、`OpenTouryo.Public` に埋め込まれている
  `Xml/TestXml.xml` / `Xml/TestXsd.xsd` を使える
- **証明書が要るテスト（`Security/`）は、正本（`root/files/resource/X509`）を csproj のリンクで出力先の `X509` に複写して使う**
  （`Security/TestCertificates.cs`。`*.pfx` のパスワードは `test`）。
  読み込みのフラグは `Exportable` だけにする（`MachineKeySet` は管理者でないと失敗することがある）。
  net10.0 は `X509Certificate2` のコンストラクタが旧式扱い（SYSLIB0057）なので、`TestCertificates.LoadCer` / `LoadPfx` を使う
- **CNG（`ECDsaCng` など）を使うテストは `[Platform("Win")]` を付ける。** Windows 以外では動かない
  （[`Public/Security/ANALYSIS.md`](../Infrastructure/Public/Security/ANALYSIS.md) 3.1 節）
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

**アセンブリごとの値は [`COVERAGE.md`](COVERAGE.md) にある。** `2_RunAllTests.ps1 -Coverage` が、
NUnit の全部とクロス DB を 1 回で回して合算し、書き換える（普通の実行は書き換えない）。

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
