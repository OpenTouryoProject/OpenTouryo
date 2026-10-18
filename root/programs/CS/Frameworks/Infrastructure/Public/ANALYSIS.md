# ANALYSIS.md — 汎用ライブラリ（Infrastructure/Public）

対象: `root/programs/CS/Frameworks/Infrastructure/Public`（`Security/` を除く） / 最終更新: 2026-10-16

上位: [`Infrastructure/ANALYSIS.md`](../ANALYSIS.md) / 全体: [`Frameworks/ANALYSIS.md`](../../ANALYSIS.md)
下位: [`Security/ANALYSIS.md`](Security/ANALYSIS.md)（`OpenTouryo.Public.Security`）

---

## 1. これは何か

`OpenTouryo.Public`（名前空間 `Touryo.Infrastructure.Public`）。**フレームワークに依存しない汎用ライブラリ**で、
単体で NuGet（`Touryo.Infrastructure.Public`）として使える。`Framework` 以下はすべてこれに依存する。

**ここに在るものを、アプリやサンプルで再実装しないこと。** 文字コード判定・設定の読み取り・
動的な呼び出しなど、フレームワーク全体が同じ実装を前提にしている。

`Public/Db/Dam*/`（DBMS 別のデータ アクセス）は**別のアセンブリ**だが、ソースはここにあるので本書で扱う（4.2 節）。

---

## 2. 構成

| フォルダ | 名前空間 | 主なクラス | 一言で |
|---|---|---|---|
| `Db/` | `Public.Db` | `BaseDam` `DamSqlSvr` `DamODBC` `DamOLEDB` `SQLUtility` `DbEnum` | データ アクセス制御（Dam）。4 節 |
| `Dto/` | `Public.Dto` | `DTTables` `DTTable` `DTRow` `DTColumn`、`DataToPoco` `PocoToPoco` `DataToDictionary` | マーシャリングできる自前の DataTable と、詰め替え |
| `Str/` | `Public.Str` | `CustomEncode` `StringChecker` `FormatChecker` `StringConverter` `FormatConverter` `CheckCharCode` `JIS2k4Checker` `StringVariableOperator` `StringExtractor` `ToStringExtension` | 文字列の符号化・検査・変換 |
| `IO/` | `Public.IO` | `ResourceLoader` `EmbeddedResourceLoader` `DeflateCompression` `ExponentialBackoff` `ZipperV2` `UnZipperV2` `BinarySerialize` | ファイル・埋め込みリソースの読み込み、圧縮、再試行 |
| `Log/` | `Public.Log` | `LogIF`、`Log_log4net` / `Log_nlog`、`CustomEventLog` `SecurityEventLog` | ログの窓口（5 節） |
| `Util/` | `Public.Util` | `GetConfigParameter` `PerformanceRecorder` `RandomValueGenerator` `EnvInfo` `ArrayOperator` `CustomMarshaler` `PublicExceptionMessage` `PubLiteral` `PubCmnFunction` `SharedMemory` | 設定の読み取りほか（6 節） |
| `Reflection/` | `Public.Reflection` | `Latebind` `MyAssemblies` | **フレームワークの動的呼び出しの心臓部**（7 節） |
| `FastReflection/` | `Public.FastReflection` | `AccessorCacher` `CompiledExpressionCreater` `InstanceCreator<T>` `EnumToString*Extensions` | 式木・Emit による高速なリフレクション |
| `Diagnostics/` | `Public.Diagnostics` | `MyDebug` `ObjectInspector` `StackFrameOperator` | デバッグ出力、オブジェクトのダンプ、呼び出し元の取得 |
| `Xml/` | `Public.Xml` | `XmlLib` | XPath・タグ名・ID で属性やノードを引く（`TestXml.xml` / `TestXsd.xsd` を埋め込み済み） |
| `Win32/` / `WinProc/` | `Public.Win32` ほか | `CmnWin32` `MMapFileWin32` `ProcessWin32` `SecurityWin32` … | P/Invoke。**net48 のみ** |
| `Resources/` | — | `PublicExceptionMessageResource` | 例外メッセージ（既定と `ja-JP`） |

---

## 3. TFM 差

**net10.0（`Public_netcore100.csproj`）で外しているもの。** SDK 形式なので、外さなければ入る。

| 外しているもの | 理由 |
|---|---|
| `Win32/**` `WinProc/**` `Util/SharedMemory.cs` | Windows の P/Invoke |
| `Log/CustomEventLog.cs` `Log/SecurityEventLog.cs` | イベント ログ |
| `IO/BinarySerialize.cs` | `BinaryFormatter`（.NET では廃止） |
| `Db/DamOLEDB.cs` `Db/DamOraClient.cs` | OLE DB / `System.Data.OracleClient` |
| `Db/Dam*/**` `Security/**` | **別のアセンブリ**（同じフォルダにあるので、外さないと二重に入る） |
| `IO/ZipBase.cs` `Zipper.cs` `UnZipper.cs` | 旧 ZIP 部品（net48 でも列挙していない） |

**net48（`Public_net48.csproj`）に無いもの** : `FastReflection/EnumToStringByExpressionTreeExtensions.cs`
（net48 は `EnumToStringByEmitExtensions` を使う）、`Db/DamOraClient.cs`、旧 ZIP 部品。

**TFM で実装が違うもの**（結果も違い得る）。

| クラス | net48 | net10.0 |
|---|---|---|
| `StringConverter`（全角・半角・平仮名・片仮名） | VB の `StrConv` | `Zipangu` |
| `GetConfigParameter` | `System.Configuration`（`app.config` / `web.config`） | `Microsoft.Extensions.Configuration`（**初期化が要る**。6 節） |
| Shift_JIS などを使う API（`StringChecker` / `CustomEncode` の一部） | そのまま使える | **呼び出し側で `CodePagesEncodingProvider` を登録しておく**（`Public` は登録しない） |

条件コンパイルは `#if (NETSTD || NETCOREAPP)` と `#if NETCOREAPP` で書いてある。
**このプロジェクトでは `NETSTD` は定義されない**ので、素の `#if NETSTD` を書くと黙って net48 側に落ちる
（[`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 6.2 節）。

---

## 4. Db（データ アクセス制御）

### 4.1 Dam の階層

```
BaseDam（Public/Db/BaseDam.cs、約 3200 行）
  ├ DamSqlSvr        SQL Server（Microsoft.Data.SqlClient）       OpenTouryo.Public
  ├ DamODBC          ODBC                                          OpenTouryo.Public
  ├ DamOLEDB         OLE DB（net48 のみ）                          OpenTouryo.Public
  ├ DamManagedOdp    Oracle（Oracle.ManagedDataAccess[.Core]）     OpenTouryo.DamManagedOdp
  ├ DamMySQL         MySQL（MySql.Data）                            OpenTouryo.DamMySQL
  └ DamPstGrS        PostgreSQL（Npgsql。net10.0 のみ）             OpenTouryo.DamPstGrS
```

**SQL Server・ODBC・OLE DB は `Public` に入っている。他の DBMS は、ドライバへの依存を分けるために別アセンブリ**にしてある。
`DamDB2` / `DamHiRDB` / `DamSqlDbWithMultiShard` / `DamOraClient` は**ビルド対象外**
（[`Infrastructure/ANALYSIS.md`](../ANALYSIS.md) 6 節）。

### 4.2 `BaseDam` が持つもの

DBMS の差を `BaseDam` と各 `Dam*` に閉じ込め、上位（`BaseDao`）からは同じ API に見せる。

| 機能 | API | 補足 |
|---|---|---|
| 接続・トランザクション | `ConnectionOpen` / `BeginTransaction(IsolationLevelEnum)` / `CommitTransaction` / `RollbackTransaction` / `ConnectionClose` | 分離レベルは `DbEnum.IsolationLevelEnum` |
| SQL の設定 | `SetSqlByFile`（`.sql` = 静的、`.xml` = 動的） / `SetSqlByCommand`（文字列。動的かどうかは中身で決まる） | **動的パラメタライズド クエリ（DPQ）の展開はここ** |
| パラメタ | `SetParameter`（型・サイズ・方向の各版） / `SetUserParameter` / `GetParameter` | パラメタ記号は Dam ごと（`@`、`DamManagedOdp` は `:`）。ODBC は名前付きを `?` の順番バインドに置き換える |
| 実行 | `ExecSelectFill_DT` / `_DS` / `ExecSelect_DR` / `ExecSelectScalar` / `ExecInsUpDel_NonQuery` | **同期のみ**（[`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 4.4 節） |
| 確認 | `ExecGenerateSQL(SQLUtility)` / `GetCurrentQuery` / `GetCurrentQueryForLog` | `ExecGenerateSQL` は組み立てた SQL を返すだけで、**接続を使わない** |

`SQLUtility` は SQL の生成を助ける（DBMS ごとの囲い文字、`DataTable` から INSERT / UPDATE の部品を作る、値を SQL のリテラルにする）。

### 4.3 動的パラメタライズド クエリ（DPQ）

`<WHERE>` / `<IF>` / `<ELSE>` などのタグで、**パラメタが設定された条件だけを組み立てる**。
値は常にパラメタで渡すので、SQL インジェクションへの耐性を保ったまま WHERE 句を変えられる。
書式の例は [`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 4.1 節、SQL の例は `root/files/resource/Sql/`（DBMS 別）。

### 4.4 Db のテスト

| 確かめること | 置き場 |
|---|---|
| タグの展開・SQL の組み立て（**DB に接続しない**） | `Tests/NUnit/TestPublic/Db/`（`DynamicQueryTests` / `SQLUtilityTests`） |
| DBMS ごとの実行（クロス DB） | `Tests/NUnit/TestPublicDb/`（SQL Server は必須、他は接続できなければスキップ） |
| サンプル DB（Northwind）に対する実行結果 | `Tests/Golden/TestDataAccess` |

---

## 5. Log

**`LogIF`（静的クラス）が唯一の窓口である。** 呼ぶ側はロガー名とメッセージだけを渡す。

- ロガー名は慣習で `"ACCESS"`（`Business` のテンプレートが、画面と B 層の入口・出口で書く）と `"SQLTRACE"`（`MyBaseDao` が SQL を書く）
- バックエンドは `LogLib` 設定で決まる（`"nlog"` なら NLog、それ以外・未設定は log4net）。
  **静的コンストラクタで 1 回だけ読む**ので、同じプロセスの中では切り替えられない
  （NUnit で NLog 側を別プロジェクト `TestPublicNLog` にしているのはこのため）
- log4net の設定ファイルは `FxLog4NetConfFile`
- `CustomEventLog` / `SecurityEventLog` は Windows のイベント ログ（net48 のみ）。
  **`CustomEventLog.Delete` はソースが属するログごと消す**

---

## 6. Util

### 6.1 `GetConfigParameter`（設定の読み取り）

| | net48 | net10.0 |
|---|---|---|
| 読み元 | `app.config` / `web.config` の `appSettings` / `connectionStrings` | `IConfiguration`（JSON の `appSettings` / `ConnectionStrings` セクション） |
| 初期化 | 不要 | **`InitConfiguration` が必須**（`IConfiguration` / `IConfigurationBuilder` / JSON ファイル名 / `Stream` / 引数なし = `appsettings.json` の 5 版） |

- `GetConfigValue` / `GetConnectionString` / `GetAnyConfigValue`（net10.0 のみ。`:` 区切りの任意のキー）
- **`FxContainerization` が真なら、環境変数を設定ファイルより優先する**（`checkContainerization` 引数で止められる）
- 初期化を忘れると `NOT_INITIALIZED` の `ArgumentException`。**例外を握る API（`Exists` など）では黙って false になる**
- 仕組みの全体（読み込み順、`%変数%` の展開、環境変数による上書き）は [`CONFIGURATION.md`](../../../../CONFIGURATION.md)

### 6.2 その他

- `PerformanceRecorder` … 経過時間と CPU 時間を測る（`MyBaseDao` の SQLTRACE が使う）
- `PublicExceptionMessage` … 例外メッセージをリソースから引く。**キーはプロパティ名**（`StackFrameOperator.GetCurrentPropertyName`）で決まるので、
  プロパティの名前を変えるとリソースのキーも変わる
- `PubLiteral` … `Public` の文字列定数。**新しい文字列リテラルは、ここに定数として足す**
- `PubCmnFunction` … 雑多な関数。**一部は他のクラスへ移した後の転送用（`[Obsolete]`）**で、
  移動先は `StringVariableOperator` / `ArrayOperator` / `StackFrameOperator`。新しいコードは移動先を呼ぶ
- `ArrayOperator` … 配列の複製・連結・切り詰め、`GetLongFromByte`（#522 で桁の重みを修正済み）
- `SharedMemory` … 名前付きの共有メモリ（net48 のみ）

---

## 7. Reflection / FastReflection

`Latebind` は、**名前で型とメソッドを探して呼ぶ**。フレームワークの次の仕組みは、すべてこれに乗っている。

| 使っている所 | 呼ぶもの |
|---|---|
| `CallController`（インプロセス呼び出し） | `TMInProcessDefinition.xml` で解決した assembly / class の `DoBusinessLogic` |
| `MyFcBaseLogic.UOC_DoAction`（自動振り分け） | `"UOC_" + MethodName` |
| `BaseController`（Web Forms の集約イベント ハンドラ） | `UOC_<コントロール ID>_<イベント名>` |

- 呼んだ先で起きた例外は `TargetInvocationException` に包まれて上がる（`CallController` は `InnerException` をスタック トレースごと投げ直す）。
  `InvokeMethod_NoErr` は、**型・メソッドが見つからないときの `ArgumentException` だけ**を握って `null` を返す
- **private メソッドも呼べる**（`BindingFlags.NonPublic`。自動振り分けの `UOC_` メソッドが private でよいのはこのため）
- `MyAssemblies` はアセンブリを読み込む。**`Azure` 設定を読む**ので、net10.0 では `InitConfiguration` の後でないと使えない
  （`EmbeddedResourceLoader` も内部でこれを使う）

`FastReflection` は、繰り返し呼ぶアクセサを式木・Emit でコンパイルして持つ（`DataToPoco` などの詰め替えが使う）。
`AccessorCacher` は、**公開の set / get が無いプロパティでは `SetDelegate` / `GetDelegate` を `null` にする**（`private set` も公開ではない）。
`DataToPoco` / `PocoToPoco` は、それを**移送の対象から外す**（#618。以前は同名の列・項目があると `NullReferenceException` になった）。
`AccessorCacher` を使う詰め替えを足すなら、同じく `null` を確かめること。
**同名で型が違う項目は、`DataToPoco` も `PocoToPoco` も `Convert.ChangeType` で変換して移す**（`PocoToPoco` は #619 から。それまでは `InvalidCastException`）。
`PocoToPoco` は、キャストで済む組み合わせ（派生型 → 基底型・`T` → `Nullable<T>`・整数 ⇔ 列挙型）はそのまま移し、
`null` は Nullable でない値型に入れない（既定値のまま）。
**移送先は構造体でもよい**（#620。それまでは例外にならずに値が入らなかった）。
`SetDelegate` は `object` を受けるので、構造体を渡すと呼ぶたびにボックス化したコピーに値が入る。
**1 度だけボックス化したものに値を入れ、最後に取り出す**こと（詰め替えを足すときも同じ）。
`CompiledExpressionCreater` の Setter は、値型では `Expression.Unbox` でボックスの中身に代入する
（#620 まで `Expression.Convert` で、アンボックス化したコピーに代入していた）。
`DataToDictionary` は、マップで他の列と同じキーに付け替える（か同名の列がある）と、**キーと列名を示す `ArgumentException`**
（`PublicExceptionMessage.DUPLICATE_DICTIONARY_KEY`）を投げる（#622。それまでは `Dictionary.Add` の汎用のメッセージ）。

---

## 8. Diagnostics

- `StackFrameOperator` … 呼び出し元のメソッド名・プロパティ名・ファイルと行を返す（`GetFrame(1)`）。
  **JIT のインライン展開でフレームがずれないよう `NoInlining` を付けている**（#616）。
  1 段深い位置から呼ぶ転送用には、`internal` の `GetCaller*`（`GetFrame(2)`）を使う
- `MyDebug` … `OutputDebugAndConsole`。`Security/MyDebug.cs` は**これの派生クラス**（重複ではない。アセンブリ分割の都合）
- `ObjectInspector` … オブジェクトの中身を文字列にダンプする

---

## 9. 依存パッケージ（net10.0）

`log4net` / `NLog` / `Microsoft.Data.SqlClient` / `System.Data.Odbc` / `Microsoft.Extensions.Configuration*` /
`Newtonsoft.Json` / `SharpZipLib`（`ZipperV2`） / `Zipangu`（`StringConverter`）、
および脆弱性のある推移的依存を引き上げるための `System.Security.Cryptography.Pkcs`。

**版は csproj を直接見ること**（Dependabot で随時変わる）。版を変えたら、nuspec（`NuGet/Symbol_Public.nuspec`）の依存も合わせる。
`Dam*` はそれぞれのドライバ（`Oracle.ManagedDataAccess.Core` / `MySql.Data` / `Npgsql`）に依存する。

---

## 10. 落とし穴

1. **net10.0 で設定を読む前に `InitConfiguration` が要る**（6.1 節）。気付きにくい読み手（`MyAssemblies` / `EmbeddedResourceLoader`）がある。
2. **`#if NETSTD` はこのプロジェクトでは偽**（3 節）。`#if NETCOREAPP` を使う。
3. **新しいファイルは net10.0 に自動で入り、net48 には入らない**（3 節）。Windows 専用 API を使うなら `Compile Remove` を足す。
4. **旧 ZIP 部品・`DamOraClient` などを直しても何も起きない**（3 節・4.1 節）。
5. **`LogIF` のバックエンドはプロセスで 1 回しか決まらない**（5 節）。
6. **`PublicExceptionMessage` のプロパティ名を変えると、引くリソースのキーも変わる**（6.2 節）。
7. **結果ファイルの比較で、不具合が「正」として固定されていたことがある**（`ArrayOperator.GetLongFromByte`、#522）。
   期待値は実装から検算する（[`Tests/Golden/TestCode/README.md`](../../Tests/Golden/TestCode/README.md)）。

---

## 11. 作業チェックリスト

- [ ] net48 / net10.0 のどちらに入るファイルか（3 節）。足したなら両方の csproj を確かめる
- [ ] 条件コンパイルは `#if NETCOREAPP`
- [ ] 文字列リテラルは `PubLiteral` に、例外メッセージはリソース（既定と `ja-JP` の両方）に足す
- [ ] 公開 API を変えない（NuGet で配布している。足すならオーバーロード）
- [ ] テストは `Tests/NUnit/TestPublic/<名前空間>/<クラス名>Tests.cs`（[`Tests/README.md`](../../Tests/README.md)）
- [ ] パッケージの版を変えたら、nuspec と `ComparePackage.ps1` / `CompareRedirect.ps1` も見る
