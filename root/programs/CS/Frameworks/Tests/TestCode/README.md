# TestCode — 部品層（`Public`）のテスト

`Touryo.Infrastructure.Public` を中心とした部品層の総合テスト。**DB には接続しない。**
データ アクセスは [`TestDataAccess`](../TestDataAccess/README.md) に分けてある（#520）。

コンソール EXE で、`Program.cs` が `Test*.Root()` を順に呼び、
出力を `Result48.txt` / `ResultCore100.txt` に書く。
判定はこのファイルとの比較で行う（[`TESTING.md`](../../../../TESTING.md) が一次情報）。

```
y_Build_TestCode_Public.bat     … ビルド → 実行 → 結果ファイル出力
```

`net48` と `core100` の 2 プロジェクトが `..\*.cs` を `Link` で共有する（実体は 1 つ）。

---

## 何をテストしているか

`Program.cs` の呼び出し順に対応する。

### Basic

| ファイル | 対象 | 見ていること |
|---|---|---|
| `TestOutputLog` | `Public.Log` | log4net / NLog へのログ出力（`LogIF`） |
| `TestGetMessageAndProperty` | `Framework.Common` / `Framework.Util` | `GetMessage`（メッセージ定義 XML）、`GetSharedProperty` |
| `TestStringChecker` | `Public.Str` | 数字・英字・かな・漢字・Shift_JIS の判定 |
| `TestFormatChecker` | `Public.Str` | 郵便番号・電話番号・日付などの書式判定 |
| `TestStringVariableOperator` | `Public.Str` | `a=1;b=2` 形式の解析と、環境変数の埋め込み（#522） |
| `TestStringExtractor` | `Public.Str` | クエリ文字列・XML 属性からの抽出、`XmlToString`（#522） |
| `TestUtil` | `Public.Util` | 型変換・配列操作・設定の読み取り・環境情報（#522） |
| `TestStringConverter` | `Public.Str` | 半角/全角・ひらがな/カタカナの変換 |
| `TestFormatConverter` | `Public.Str` | 西暦/和暦、数値の書式化 |
| `TestCustomEncode` | `Public.Str` | Base64URL・パーセント エンコード等 |
| `TestJISCode` | `Public.Str` | JIS コードの判定 |

### Extension

| ファイル | 対象 | 見ていること |
|---|---|---|
| `TestEnumToStringExtensions` | `Public.FastReflection` | 列挙体 ⇔ 文字列 |
| `TestXmlLib` | `Public.Xml` | XML の読み書き |
| `TestDeflateCompression` | `Public.IO` | Deflate の圧縮・伸張 |
| `TestResourceLoader` | `Public.IO` | ファイル・埋め込みリソースの読み取り（#522） |

### Dto

| ファイル | 対象 | 見ていること |
|---|---|---|
| `TestDto` | `Public.Dto` | `DataToPoco` / `PocoToPoco` / `DataToDictionary` の項目移送（#522） |

### Diagnostics

| ファイル | 対象 | 見ていること |
|---|---|---|
| `TestObjectInspector` | `Public.Diagnostics` | オブジェクトの再帰的な文字列化と**再帰の上限**（#522） |

### Reflection

| ファイル | 対象 | 見ていること |
|---|---|---|
| `TestLatebind` | `Public.Reflection` | 遅延バインドによる呼び出し（`NonPublic` を含む） |
| `TestFastReflection` | `Public.FastReflection` | `AccessorCacher` / `InstanceCreator` 等の高速リフレクション（#522） |

`TestEmbedded.txt` は `TestResourceLoader` が読む埋め込みリソース。
**`LogicalName` を `TestCode.TestEmbedded.txt` に固定している**
（既定のままだと `TestCodeFx` / `TestCodeCore` で名前が食い違うため）。

---

## テストを書き足すときの決まり

結果ファイルとの比較で判定するため、**実行するたびに変わる値を出してはならない。**
`CompareResult.ps1` の正規化で吸収できるもの（日時・処理時間・GUID 等）もあるが、
**吸収に頼らず、そもそも出さない**方がよい。

### 例外は必ず捕まえ、型名だけを出す

捕まえないと `Program.Main` まで抜けて**実行が中断**し、
さらに**パス入りのスタック トレースが結果ファイルに書き込まれる**（環境依存の差分になる）。

```
場所 ...\ArrayOperator.cs:行 62
```

メッセージ本文も出してはならない。**OS とランタイムの言語で変わる**ため
（CI は英語環境、ローカルは日本語環境）。

```csharp
catch (Exception ex)
{
    // メッセージは環境の言語で変わるため、型名だけを出す。
    MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
}
```

### 環境で変わる値は「取れること」までに留める

`EnvInfo`（マシン名・OS・ビット数）や `GetConfigParameter` の設定値がこれにあたる。
設定値は **net48 と .NET (Core) でパス区切りが違う**ため、値そのものは比較できない。

```csharp
MyDebug.OutputDebugAndConsole(
    "EnvInfo.MachineName が取れる : " + !string.IsNullOrEmpty(EnvInfo.MachineName));
```

### 日時は固定値を与える

`DateTime.Now` を使わない。UTC で与えれば、時差のある環境でも同じ結果になる。

```csharp
DateTimeOffset dto = new DateTimeOffset(2026, 8, 6, 12, 34, 56, TimeSpan.Zero);
```

### `csproj` は 2 つとも直す

`net48/TestCodeFx.csproj` と `core100/TestCodeCore.csproj` の**両方**に
`Compile` を足す。片方だけだと、そちらのフレームワークでしかテストが動かない。

---

## 記録している「仕様」（不具合ではない）

読み違えやすい挙動は、**誤用したときの結果も含めて**ケースに残してある。

| 対象 | 挙動 |
|---|---|
| `PubCmnFunction.GetFileNameNoEx` | 第 2 引数は**パス区切り**。拡張子の区切りだと思って `'.'` を渡すと空文字が返る |
| `ArrayOperator.CopyArray` | **配列を伸ばせない。** コピー長が「コピー先配列の長さ」で固定のため、コピー先を大きくするとコピー元が足りず落ちる。書込開始位置を 0 より後ろにしても落ちる |
| `StringExtractor.GetParameterFromQueryString` | 「値が空」と「名前が無い」を**区別できない**（どちらも `""`） |
| `ObjectInspector` | 再帰の深さは **5 まで**。入口で加算・出口で減算するカウンタで抑えている |
| `Latebind` | `NonPublic` を含むため **private も呼べる**。呼び先の例外は `TargetInvocationException` に包まれる |

---

## 差分が大量に出たときの読み方

**テストを足した直後は、差分件数が実態より大きく出る。**
`CompareResult.ps1` は `Compare-Object -SyncWindow 20` で比較するため、
20 行を超える挿入があると以降の行が総崩れになる。

**合否（OK / NG）は正しい。誤るのは件数と一覧の見え方だけ。**
実差分を見たいときは `-SyncWindow` を外して突き合わせる。詳細は
[`TESTING.md`](../../../../TESTING.md) の「差分の『件数』は当てにならないことがある」。
