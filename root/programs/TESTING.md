# TESTING.md — 単体テストの実行と判定

対象: `root/programs/CS/Frameworks/Tests`（C# 側）
配置: `root/programs`
本書は、リリース時に行っていた「単体テスト スクリプトの実行結果を diff で目視」を、
**機械判定に置き換える**ための手順と判定基準を記述する（#513 段階 1）。

リリース時の作業全体は [`RELEASE.md`](RELEASE.md) を参照。
実行順は `1_BuildAll.ps1` → `2_RunAllTests.ps1` → `3_SmokeTest.ps1`。

---

## 1. 使い方

```powershell
cd root\programs

# ビルド バッチを実行し、再生成された結果を HEAD 版と比較して一覧表示する
.\2_RunAllTests.ps1

# バッチを実行せず、いま手元にある結果ファイルだけを比較する
.\2_RunAllTests.ps1 -SkipBuild

# 個別に比較する
.\CompareResult.ps1 -Expected <HEAD 版> -Actual TestCode\Result48.txt
.\CompareResult.ps1 -Expected <HEAD 版> -Actual TestBatch\ResultSimpleBatch48.txt -SkipLog4netTrace
```

終了コードは `0` = 全 OK、`1` = NG あり、`2` = ファイル無し。

---

## 2. 何を何と比較しているか

**従来の運用をそのまま機械化している。** 置き換えたのは最後の「目視」だけ。

`y_Build_TestCode*.bat` は「ビルド → テスト実行 → **結果を `Result*.txt` へ出力**」まで行う。
この `Result*.txt` は Git 管理下にあり、バッチを実行すると上書きされる。
つまり従来の手順は次のとおりだった。

1. バッチを実行して `Result*.txt` を再生成する
2. `git diff` で「前回のリリース時の結果」との差分を目視する

`2_RunAllTests.ps1` はこの構造を変えず、2 を機械比較に置き換える。

| | 実体 | 取得方法 |
|---|---|---|
| **期待値** | HEAD にコミットされている `Result*.txt` | `git show HEAD:<path>`（参照系のみ） |
| **実測値** | バッチが再生成したワーキング ツリーの `Result*.txt` | バッチの実行 |

> **副作用**: ワーキング ツリーの `Result*.txt` が書き換わる。これは従来のバッチ運用と同じ。

**生 `git diff` を目視してはいけない。** 実行日時が全行に入るため、
内容が同じでも**ほぼ全行が差分になる**（実測で 6 ファイル約 2,458 行）。

```diff
-[2025/11/18 15:19:08,286],[INFO ],[1],,,,----->>,...
+[2026/08/01 22:40:19,772],[INFO ],[1],,,,----->>,...
```

判定は本スクリプトの**正規化後の差分**で行う。0 なら内容は同じ。
生 diff を読むのは、正規化後に差分が出たときだけでよい。
`Result*.txt` のコミットは、内容が変わった（新しい基準にしたい）ときだけでよい。

### 対象テスト（8 ケース）

| テスト | 結果ファイル（＝期待値） | ビルド バッチ | 備考 |
|---|---|---|---|
| TestCode (net48) | `TestCode/Result48.txt` | `y_Build_TestCode_Public.bat` | 部品層の総合テスト（約 2,300 行） |
| TestCode (net10.0) | `TestCode/ResultCore100.txt` | 同上 | 同上 |
| TestDataAccess (net48) | `TestDataAccess/Result48.txt` | `y_Build_TestCode_DataAccess.bat` | データ アクセス（#520）。**実行モードで対象 DBMS が変わる** |
| TestDataAccess (net10.0) | `TestDataAccess/ResultCore100.txt` | 同上 | 同上 |
| SimpleBatch (net48) | `TestBatch/ResultSimpleBatch48.txt` | `y_Build_TestCode_Batch.bat` | **DB 接続あり**（Northwind） |
| SimpleBatch (net10.0) | `TestBatch/ResultSimpleBatchCore100.txt` | 同上 | 同上 |
| EncAndDecUtilCUI (net48) | `EncAndDecUtilCUI/Result48.txt` | `y_Build_TestCode_SecCUI.bat` | 暗号・JWT・XML 署名 |
| EncAndDecUtilCUI (net10.0) | `EncAndDecUtilCUI/ResultCore100.txt` | 同上 | 同上 |

`EncAndDecUtilCUI/ResultCore100OnLinux.txt` は Linux 実行時の期待結果のため、
Windows からの `2_RunAllTests.ps1` の対象外。

### `TestDataAccess` の実行モード（#520）

**クロス DB は CI では実行できない。** LocalServicesOnDocker の各 DBMS は Linux コンテナで、
`windows-latest` は Linux コンテナを動かせないため（[`BUILDING.md`](BUILDING.md) 9 節）。
このため対象を切り替えられるようにしてある。

```
TestDataAccessFx.exe /MODE SQLONLY     … SQL Server だけ（既定。CI はこちら）
TestDataAccessFx.exe /MODE LOCAL       … ローカルで起動している DBMS をすべて
```

**期待値（`Result*.txt`）は `SQLONLY` で記録している。** `y_Build_TestCode_DataAccess.bat` も
`SQLONLY` を明示して呼ぶ。`LOCAL` は手元での確認用で、比較の対象にはしない。

> `LOCAL` の対象は net48 と .NET (Core) で異なる。
> **PostgreSQL（`NPS`）は `DamPstGrS` が .NET (Core) 専用**のため、net48 では対象外。

`TestCode` と分けているのは、**DB に接続するかどうかで前提が大きく違う**ため。
混ぜると、DB が無い環境では `TestCode` ごと動かなくなる。

#### 何をテストしているか

| クラス | 接続 | 内容 |
|---|---|---|
| `TestSQLUtility` | **しない** | `SQLUtility` が生成する SQL を 4 DBMS 分（#515） |
| `TestDataAccessPattern` | **する** | `Dam` の実行系メソッドを、対象 DBMS ごとに一通り |
| `TestDataAccessUpdate` | **する** | 更新系（INSERT / UPDATE / DELETE）とトランザクション |
| `TestDataAccessDpq` | **する** | 動的パラメタライズドクエリ（`<IF>` / `<WHERE>` / `<VAL>`） |
| `DataProvider` | — | データ プロバイダごとの差異（Dam・囲い文字・パラメタ記号）を吸収 |
| `TestTable` | — | テスト用の表 `TestOrders` の作成・投入・破棄 |

`TestDataAccessPattern` が呼ぶのは次の 5 つ。いずれも `Shippers`（3 件）に対する読み取りで、
**件数だけを出力する**ため、DBMS が違っても結果は同じになる。

```
- ExecSelectScalar        : 3
- ExecSelectFill_DT       : 3
- ExecSelectFill_DS       : 3
- ExecSelect_DR           : 3
- SetSqlByFile ＋ Scalar  : 3     ← 静的SQL（DBMS ごとのフォルダから読む）
```

**`Dam` を直接使っている。** Ｂ層・Ｄ層を経由すると引数クラス・戻り値クラス・
`LayerB` / `LayerD` の一式が要るが、ここで見たいのは実行系の挙動だから。
Ｂ層・Ｄ層を通した確認は `TestBatch`（SimpleBatch）が担う。

#### 更新系（`TestDataAccessUpdate`）

**専用の表 `TestOrders` を自分で作り、最後に落とす。** Northwind の表を更新すると
`SimpleBatch` や `3_SmokeTest.ps1` の前提が壊れるため、**既存のデータには一切触れない。**

```
- INSERT（GetInsertSQLParts）  : 3 件
  投入後 : 1, 10, 999, x
  投入後 : 2, 20, 888, y
  投入後 : 3, 30, 777, z
- UPDATE（GetUpdateSQLParts）  : 2 件
  更新後 : 1, 10, 100, a
  更新後 : 2, 20, 200, b
  更新後 : 3, 30, 777, z     ← 更新対象外。無傷であること
- ロールバック後の件数        : 3
- コミット後の件数            : 2
- DELETE                      : 2 件
- 削除後の件数                : 0
```

**`SQLUtility` が生成した SQL を実際に実行し、結果まで検証する。**
#515 は「構文としては妥当でも、**誤った行に誤った値が入る**」不具合だった。
生成結果の目視では見つけにくいため、**更新対象外の行 (3,30) を 1 件混ぜ、
無傷であることまで確認する。** ここが崩れたら複合主キーの扱いが壊れている。

> PostgreSQL と MySQL は `CASE ... WHEN ... THEN` による一括 UPDATE という別実装。
> #515 の該当パスであり、実 DB での確認はこの 2 つで特に意味がある。

##### 表を作るときの注意

**囲い文字を `SQLUtility` が生成するものと一致させること。**

| DBMS | 囲い文字 | 囲わないと |
|---|---|---|
| SQL Server | `[ ]` | （既定の照合順序では問題にならない） |
| Oracle | `" "` | **大文字**に畳まれる |
| PostgreSQL | `" "` | **小文字**に畳まれる |
| MySQL | `` ` `` | （列名は大小同一視） |

Oracle と PostgreSQL は、囲わずに表を作ると生成された SQL の `"Qty"` が
**「存在しない列」**になる。`DataProvider.Quote` に集約してある。

> `DROP TABLE IF EXISTS` は Oracle では 23ai 以降でしか使えないため、
> 分岐せずに「投げて握る」形にしている。
> DDL は Oracle では暗黙にコミットされるため、トランザクションの外で行う。

> **接続できない場合も落とさない。** `LOCAL` でコンテナが起動していないときは、
> 例外の**型名だけ**を出して次のデータ プロバイダへ進む。
> メッセージを出さないのは、OS の表示言語で変わり差分になるため（`BUILDING.md` 9 節）。
>
> ```
> [ODP]
> - 例外 : Oracle.ManagedDataAccess.Client.OracleException
> ```


#### 動的パラメタライズドクエリ（`TestDataAccessDpq`）

**同じ 1 本の XML から、パラメタの与え方だけで異なる SQL が組み立つ**ことを見る。
仕様は `opentouryo-query-definition` スキルが一次情報。

##### 6 つの観点

| # | 観点 | 見るもの |
|---|---|---|
| 1 | `<WHERE>` / `<IF>` の組み合わせ | 条件の増減と、先頭 `AND` の除去 |
| 2 | **`<IF>` / `<ELSE>`** | **「未設定」と「`null` を設定」は別物**。テキスト内・タグ内の両方 |
| 3 | `<LIST>` | IN 句への自動展開 |
| 4 | パラメタの作用範囲 | テキスト内は全タグ、タグ内は最初の 1 タグ |
| 5 | `<DELCMA>` / `<INSCOL>` | 要素が消えたときのカンマ処理 |
| 6 | 組み立て結果そのもの | `ExecGenerateSQL`（実行しない） |

**1〜5 は件数で見る。** DBMS ごとに型や表示が変わるため、値を出すと差分になる。
**6 は組み立て結果を見る。** 分岐やエスケープは実行しなくても分かり、表の中身にも依存しない。

##### 1. `<WHERE>` / `<IF>` の組み合わせ

`<IF>` が 2 つなので、設定／未設定の組み合わせは **2 の 2 乗 = 4 通り**。
**全数を通したうえで**、AND であることを判別する 1 件を足している。

| ケース | P1 | P2 | 結果 | 検証対象 |
|---|---|---|---|---|
| 両方指定 | ○ | ○ | 1 件 | 両方が入る（正の確認） |
| **両方指定(不一致)** | ○ | ○ | **0 件** | **AND であること** |
| OrderIDのみ | ○ | × | 1 件 | **末尾**の `<IF>` が落ちる |
| Qtyのみ | × | ○ | 1 件 | **先頭**が落ちたとき `AND` が除去される |
| 指定なし | × | × | 4 件 | `<WHERE>` ごと消える |

**「Qtyのみ」が最も壊れやすい箇所を見ている。**
2 番目以降の `<IF>` は**テキストの先頭に `AND` を書く**規約なので、
1 番目が落ちたときに除去されないと `WHERE AND ...` となって構文エラーになる。

**「両方指定(不一致)」は判別のために足した。**
`P1=1`（1 行目）と `P2=888`（2 行目）で**指す行をずらしてある**。

| 実装が実際には… | 結果 |
|---|---|
| AND で連結（正しい） | **0 件** |
| OR で連結（誤り） | 2 件 |
| 片方を無視（誤り） | 1 件 |

3 者が異なる件数になるため、1 ケースで判別できる。
**「両方指定」だけでは、どれでも 1 件になり判別できない。**

##### 2. `<IF>` / `<ELSE>`（テキスト内・タグ内の両方）

**パラメタの種類で、状態の数が違う。両方を通す。**

```xml
<IF>AND "Note" = @P3<ELSE>AND "Note" IS NULL</ELSE></IF>          ← テキスト内
<IF name="F1">AND "Note" = 'x'<ELSE>AND "Note" IS NULL</ELSE></IF> ← タグ内
```

| | 有効（`<IF>`） | `<ELSE>` | ブロック削除 | 状態数 |
|---|---|---|---|---|
| テキスト内（`@P3`） | 値を設定 | **`null`** | 未設定 | 3 |
| タグ内（`name="F1"`） | `true` | **`false`** ／ `null` | 未設定 | **4** |

**タグ内には `false` という状態が増える。** テキスト内に「偽」は無く、値か `null` か未設定しかない。

| ケース | 結果 |
|---|---|
| テキスト内 : 値 / `null` / 未設定 | 1 件 / 1 件 / 4 件 |
| タグ内 : `true` / `false` / `null` / 未設定 | 1 件 / 1 件 / 1 件 / 4 件 |

> **条件から外したいときに `null` を渡してはならない。**
> 逆に `IS NULL` が残って外れない。外したいなら**設定しない**。
> **実行時エラーにならず件数が静かに変わる**ので、気付きにくい。

このため、テスト用の表には **`Note` が NULL の行を 1 件**入れてある。

##### 3. `<LIST>`（IN 句）

1 つのパラメタ名に複数値を与えると `@名_1, @名_2 …` へ自動展開される。
**与えた数がそのまま件数に出る**ため、展開されたかを件数で判別できる（2 値 → 2 件、3 値 → 3 件）。

##### 4. パラメタの作用範囲

**同名を書いたときの作用範囲が違う。**

| | 書き方 | 作用範囲 |
|---|---|---|
| テキスト内パラメタ | `<IF>AND X = @P1</IF>` | **同名を書いた全タグ** |
| タグ内パラメタ | `<IF name="F1">` | **最初の 1 タグだけ** |

判別できるように、2 つの `<IF>` が**両立しない条件**にしてある。

| | 結果 | 意味 |
|---|---|---|
| テキスト内 | **0 件** | 両方に作用（`OrderID = 1` かつ `= 2`） |
| タグ内 | **1 件** | 最初だけ作用（`OrderID = 1`） |

##### 5. `<DELCMA>` / `<INSCOL>`

列と値を対で増減させ、**どの要素が残っても構文が壊れない**ことを実際に INSERT して確かめる。
末尾を落とす場合と**先頭を落とす場合**の両方を通す（先頭側のカンマ除去が要るため）。

##### 6. 組み立て結果だけを見る（`ExecGenerateSQL`）

| 対象 | 確認 |
|---|---|
| `<SELECT>` / `<CASE>` / `<DEFAULT>` | 値による分岐（`a1` / `b2` / 一致なし） |
| 比較演算子 | `&lt;` と `<![CDATA[ ]]>` の両方が正しく組み立つ |
| **`<JOIN>` / `<SUB>`** | **結合・副問い合わせが丸ごと出入りする** |
| `IsDPQ` | `.xml`（タグあり）は `True`、`.sql` は `False` |

```
比較演算子    : SELECT [OrderID] FROM [TestOrders] WHERE [Qty] < 900 AND [Qty] > 100

JOIN+SUB あり : SELECT o.[OrderID] FROM [TestOrders] o
                INNER JOIN [OtherTable] t ON o.[OrderID] = t.[OrderID]
                WHERE o.[Qty] = 999 AND o.[OrderID] IN (SELECT [OrderID] FROM [OtherTable])
JOIN のみ     : ... INNER JOIN [OtherTable] t ON ... WHERE o.[Qty] = 999
JOIN+SUB なし : SELECT o.[OrderID] FROM [TestOrders] o
```

> **実行しないので、存在しない表を書いてよい。**
> `<JOIN>` / `<SUB>` は結合や副問い合わせを扱うため
> 「表が 1 つでは組めない」と考えがちだが、**組み立てだけなら表の実在は要らない。**

> **無効にするときは「設定しない」。** `false` や `null` を渡すと `<ELSE>` が要り、
> 無ければエラーになる（`<JOIN>` / `<SUB>` は `true` / `false` / `null` 以外もエラー）。

> **`ExecGenerateSQL` を実装しているのは `DamSqlSvr` だけ。**
> 他は `NotImplementedException` を投げるため、「未実装」と出して先へ進める。
> 組み立て結果は DBMS によらないので、SQL Server で見れば足りる。

##### 書くときに踏んだ落とし穴

| 事象 | 原因 |
|---|---|
| `<IF>` で `ArgumentException` | **パラメタも `name` 属性も無い `<IF>`**。有効・無効の判断材料が無い。CDATA 内のパラメタは認識される |
| 整形式でない `.xml` で例外 | 「書式が不正なら静的にフォールバック」は**タグの綴り等**の話。XML として壊れていれば例外 |

##### パラメタの先頭記号は DBMS で異なる

| DBMS | 記号 |
|---|---|
| Oracle（`ODP`） | **`:`** |
| SQL Server / MySQL / PostgreSQL | `@` |

PostgreSQL は `:` から `@` に変更された経緯がある（`DamPstGrS.cs` の更新履歴）。

**XML は実行時に書き出して捨てる。** 記号と囲い文字が DBMS で異なるため、
共有の SQL 置き場に置くと 4 方言ぶんの重複になる。表と同じ考え方。

##### 記法の参考

`root/files/resource/Test/dpq/query/` に `DPQuery_Tool` 用の資産がある
（`<SUB>` / `<JOIN>` を使った複雑な例も含む）。

> あちらの `<PARAM>` タグは**ツールが値を与えるためのもの**。
> 実行時は `SetParameter` / `SetUserParameter` で与える。

##### 対象外

| 項目 | 理由 |
|---|---|
| パラメタの型・サイズ指定 | `SetParameter` の 3〜5 引数版 |
| Out / RetVal パラメタ、ストアド | 4 DBMS で書き方が大きく異なる |
| 埋め込みリソース | `MyBaseDao.UseEmbeddedResource`。配布形態の話 |
#### クロス DB テストの実施手順（`LOCAL`）

**① DBMS を起動する**

各 DBMS は [LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker)
のコンテナで動かす。

```powershell
cd <LocalServicesOnDocker のパス>
.\Start-Services.ps1
```

`test\dotnet\start.bat` で、各サービスへ接続できるかを先に確かめられる。
**ここで NG のものは、こちらのテストでも当然 NG になる。**

**② ビルドする**

```powershell
cd root\programs\CS
cmd /c "echo. | call y_Build_TestCode_DataAccess.bat"
```

**③ `/MODE LOCAL` で実行する**

```powershell
# net48（SQL Server / Oracle / MySQL）
cd root\programs\CS\Frameworks\Tests\TestDataAccess\net48\bin\Debug
.\TestDataAccessFx.exe /MODE LOCAL

# .NET 10（＋ PostgreSQL）
cd root\programs\CS\Frameworks\Tests\TestDataAccess\core100\bin\Debug\net10.0
dotnet TestDataAccessCore.dll -- /MODE LOCAL
```

**`2_RunAllTests.ps1` からは実行できない。** バッチが `/MODE SQLONLY` を明示して呼び、
その出力を `Result*.txt` に書くため。
**`LOCAL` の出力で上書きすると期待値が壊れる**（起動している DBMS の数で内容が変わる）。
手動実行だけにしてあるのはこのため。

##### 接続文字列

`App.config`（net48）と `appsettings.json`（.NET (Core)）に持っている。
LocalServicesOnDocker の既定値に合わせてあるので、**そのまま使えば通る**。

| DAP | キー | 既定値の要点 |
|---|---|---|
| `SQL` | `ConnectionString_SQL` | `localhost` / `sa` / `Northwind` |
| `ODP` | `ConnectionString_ODP` | `SCOTT` / `tiger` / `localhost/XE` |
| `MCN` | `ConnectionString_MCN` | `localhost` / `root` / `test` |
| `NPS` | `ConnectionString_NPS` | `localhost` / `postgres` / `postgres`（.NET (Core) のみ） |

> **ADO.NET のキー＝値形式では、パスワードの `@` をエンコードしない。**
> `Password=seigi@123` のままでよい。`%40` が要るのは Mongo のような URI 形式のみ。

##### 実測（全 DBMS 起動時）

| DBMS | 参照系 5 パターン | 更新系 |
|---|---|---|
| SQL Server（`SQL`） | net48 5/5 ／ .NET 10 5/5 | **全項目一致** |
| Oracle（`ODP`） | net48 5/5 ／ .NET 10 5/5 | **全項目一致** |
| MySQL（`MCN`） | net48 5/5 ／ .NET 10 5/5 | **全項目一致** |
| PostgreSQL（`NPS`） | net48 対象外 ／ .NET 10 5/5 | **全項目一致**（.NET 10 のみ） |

更新系は **4 DBMS が同一の出力**になる（件数と値だけを出しているため）。

##### 失敗したときの切り分け

**例外の型名だけでは足りないことがある。** その場合はエラー番号まで見る。

| 症状 | 番号・型 | 原因 |
|---|---|---|
| MySQL に繋がらない | `1042` | **コンテナが起動していない**（ポートを待ち受けていない） |
| 同上 | `1130` | サーバには届いている。アカウントのホスト パターン不一致 |
| 同上 | `1045` | 認証情報の誤り |
| Oracle が `FileNotFoundException` | — | **パッケージ名が net48 と違う。**<br>.NET (Core) は `Oracle.ManagedDataAccess.Core` |

ポートの待ち受けは次で確認できる。空なら①が済んでいない。

```powershell
Get-NetTCPConnection -LocalPort 3306 -State Listen   # MySQL
Get-NetTCPConnection -LocalPort 1521 -State Listen   # Oracle
Get-NetTCPConnection -LocalPort 5432 -State Listen   # PostgreSQL
Get-NetTCPConnection -LocalPort 1433 -State Listen   # SQL Server
```

### ビルドをバッチに委ねている理由

`csproj` / `sln` を直接 MSBuild してはならない。
`nuget.exe restore` が行う**ネイティブ DLL の配置が漏れる**ためで、
ビルドは成功するのに実行時に落ちる、という分かりにくい失敗になる。

```
System.DllNotFoundException
   at Microsoft.Data.SqlClient.SNINativeManagedWrapperX64.SNIInitialize(IntPtr)
```

`Microsoft.Data.SqlClient` は SNI をネイティブ DLL で持ち、
`bin\Debug\Microsoft.Data.SqlClient.SNI.x64.dll` 等が必要になる。
「何をどうビルドするか」の正はバッチ側に置く。

### PowerShell から `.bat` を呼ぶときの注意

PowerShell は子プロセスに `NoDefaultCurrentDirectoryInExePath=1` を渡す。
一方バッチは、出力フォルダへ `cd` したうえでパス区切りを含まない名前で実行している。

```bat
cd "Frameworks\Tests\TestBatch\SimpleBatch\bin\Debug"
SimpleBatch.exe /Dap SQL ... > ..\..\..\ResultSimpleBatch48.txt
```

この変数が効いていると次のようになり、**結果ファイルが空のまま、バッチは成功したように進む**。

```
'SimpleBatch.exe' is not recognized as an internal or external command,
```

`2_RunAllTests.ps1` は、バッチを直接実行した場合と同じ条件にするため、この変数を解除している。

---

## 3. 前提条件

- **Frameworks をビルド済み**であること（`Build_net48` / `Build_netcore100` が存在）
- **SQL Server の Northwind に接続できる**こと（SimpleBatch が使用）
- **Northwind のテスト データが標準状態**であること（後述の「4. テスト データ」）

> **GitHub Actions でも実行している。** 前提の揃え方（SQL Server の導入、Northwind の
> ロード、照合順序）は [`BUILDING.md`](BUILDING.md) 9 節が一次情報。
> **期待値は開発環境で生成されたもの**なので、CI 側の環境がずれると差分が出る。
> そのため CI は期待値と実測値の両方を artifact に残す。

### テスト証明書について

`EncAndDecUtilCUI` の csproj は `*.cer` / `*.pfx` を `CopyToOutputDirectory` しているため、
**これらが無いとビルドが `MSB3030` で失敗する**。

これらは **Git 管理外の作業用コピー**であり、**正本はリポジトリ内の
`root/files/resource/X509/`**（10 件が Git 管理下）にある。

`y_Build_TestCode_SecCUI.bat` は先頭で `copy_cert.bat` を呼ぶが、こちらは `C:\root\files\...`
を参照するため、リポジトリだけを clone した環境では配置できない。
`2_RunAllTests.ps1` はリポジトリ内の正本から不足を**自動で配置**するため、手動操作は不要。
個別にビルドする場合のみ、次のいずれかを実行する。

```powershell
# リポジトリ内の正本から配置（推奨・配置場所に依存しない）
Copy-Item ..\..\..\..\files\resource\X509\SHA*.??? EncAndDecUtilCUI\

# あるいは同梱のバッチ（C:\root\files\resource\X509\ を参照する）
cd EncAndDecUtilCUI
.\copy_cert.bat
```

`copy_cert.bat` は `C:\root\files\...` を参照するが、そちらは Samples の**実行時**要件であり、
単体テストのビルドには不要。リポジトリ内の正本から取る方が確実。

---

## 4. なぜ「目視」だったのか（正規化の必要性）

期待結果と実行結果には、**実行のたびに変わる値**が多数含まれる。
そのため素の diff では必ず差分が出る。実測値は次のとおり。

| テスト | 素の diff | 正規化後 |
|---|---|---|
| TestCode (net48) | 10 件 | **0 件** |
| TestCode (net10.0) | 11 件 | **0 件** |
| EncAndDecUtilCUI (net48) | 52 件 | **0 件** |
| EncAndDecUtilCUI (net10.0) | 52 件 | **0 件** |

`CompareResult.ps1` は次を正規化してから比較する。

| 種別 | 例 |
|---|---|
| 日時 | `[2026/08/01 14:39:50,027]` → `<DATETIME>` |
| 処理時間 | アクセス ログ末尾の `234,125` → `<ELAPSED>` |
| 絶対パス | `C:\OpenTouryo\...` → `<PATH>` |
| GUID | → `<GUID>` |
| XML 署名値 | `<SignatureValue>...</SignatureValue>` → `<XMLSIG>` |
| Base64URL | JWT・JWE の IV・認証タグ・鍵 → `<B64URL>` |
| Base64 | 鍵・ハッシュ → `<B64>` |

加えて、次の行は比較対象から除外する。

- **空行**
- **`-SkipLog4netTrace` 指定時の `log4net: ` 行**
  … `TestBatch` は 149 行中 145 行が log4net の内部トレースで、判定は残り 4 行
- **非対話実行の副産物**
  … サンプルは末尾に `Console.ReadKey()` を持つものがあり、
  出力をリダイレクトすると必ず例外で終わる。テスト内容とは無関係

---

## 5. 判定基準

**正規化後に残った差分は「実質的な差分」**であり、次のいずれかを意味する。

1. **退行** … コードの不具合。要調査
2. **期待結果ファイルの陳腐化** … 仕様変更に追随していない。期待結果を更新する
3. **テスト データの汚染** … DB のレコードが増減している。データを戻す

`3` の実例（2026-08-01 に発生。復旧済み）:

```
SimpleBatch (net48 / net10.0)
  [実測のみ] 4件のデータがあります
  [期待のみ] 3件のデータがあります
```

Northwind の `Shippers` は標準で 3 件だが、`ShipperID=4` に `Speedy Express` の重複が
存在していた。CRUD サンプルの「追加」操作で挿入されたものと見られる。

このように、**件数などテスト対象の出力そのものは正規化してはならない**。
正規化すると、退行とデータ汚染のいずれも検出できなくなる。

**テスト データを汚す操作を行った後は、DB を戻してから単体テストを実行すること。**

```sql
-- 確認（標準は 3 件）
SELECT ShipperID, CompanyName FROM Shippers ORDER BY ShipperID;
```

### テスト データの戻し方

DB コンテナは**永続ボリュームを使用していない**（`docker-compose.yml` の
`./sqlserver/mssql-db:/var/opt/mssql` はコメントアウト）。
Northwind は起動のたびに `start-up.sh` が再作成するため、
**コンテナを作り直せば初期データに戻る**。

`LocalServicesOnDocker` の PowerShell 版スクリプトを使用する
（**DB の初期化完了待ちを行う**ため、`.bat` 版よりテスト用途に適している）。

| 環境 | 使用するスクリプト |
|---|---|
| Rancher Desktop 導入時（Windows から `docker` が使える） | `Stop-Services.ps1` → `Start-Services.ps1` |
| WSL2 内の Docker を使う場合 | `Stop-Services_wsl2.ps1` → `Start-Services_wsl2.ps1` |

```powershell
cd <LocalServicesOnDocker>
.\Stop-Services.ps1      # docker compose down（コンテナを破棄）
.\Start-Services.ps1     # docker compose up -d ＋ DB 初期化完了待ち
```

> **`docker restart` では戻らない。** 同一コンテナが再開されるだけで書き込み層が保持されるため。
> コンテナの破棄（`down`）と再作成（`up`）が必要。
> なお compose には `restart: always` が指定されているため、ホスト再起動でも戻らない。

初期化完了待ちを省略したい場合は `-NoWait` を指定できるが、
その場合は Northwind のロード完了前にテストが始まり得るため、テスト用途では非推奨。

コンテナを作り直さず、個別に戻すこともできる。

```sql
DELETE FROM Shippers WHERE ShipperID > 3;
```

### 期待結果に含まれる「正常な失敗」

`EncAndDecUtilCUI` の期待結果には `False` を含む行が **3 件**含まれる。
これはプラットフォーム非対応等による既知の失敗であり、**正常**。
実測でも同数であることを確認すること（`CompareResult.ps1` は行単位で比較するため自動的に検出される）。

---

## 6. 正規化ルールを追加するとき

新しい非決定値が現れた場合は、`CompareResult.ps1` の `$normalizers` に追加する。

**追加してよいのは「実行のたびに変わる値」だけ**。
テスト対象の出力（件数・判定結果・型名など）を正規化してはならない。
差分を消すために安易にパターンを広げると、退行を検出できなくなる。

判断に迷う場合は、**同じ環境で 2 回実行して差分が出る値かどうか**を確認するとよい。
