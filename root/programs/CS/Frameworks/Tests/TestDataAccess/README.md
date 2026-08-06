# TestDataAccess — データ アクセスのテスト

`Dam` を通したデータ アクセスを、**複数の DBMS に対して**確認する（#520）。

| | |
|---|---|
| 実行と判定 | [`root/programs/TESTING.md`](../../../../TESTING.md)（`2_RunAllTests.ps1` が回す） |
| CI での前提の揃え方 | [`root/programs/BUILDING.md`](../../../../BUILDING.md) 9 節 |
| SQL 定義ファイルの書式 | `opentouryo-query-definition` スキル |

**本書は、このプロジェクトが何をどう確認しているかを述べる。**
`TESTING.md` は 8 ケース全体の運用を扱い、こちらの詳細は持たない。

---

## 実行モード（`/MODE`）

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

`TestCode` と分けているのは、**このプロジェクトが DB を前提にする**ため。
混ぜると、DB が無い環境では `TestCode` ごと動かなくなる。

> **ただし、個々のテストが全て接続するわけではない。**
> SQL を組み立てるだけのものは接続を必要としない（次節）。

## 何をテストしているか

**分かれ目は「接続するか」ではなく「実行するか、組み立てだけか」。**
クラス単位でもない。`TestDataAccessDpq` は中で両方を行う。

| クラス／観点 | 見るもの | DB |
|---|---|---|
| `TestSQLUtility` | `SQLUtility` が生成する SQL を **4 DBMS 分**（#515） | **要らない** |
| `TestDataAccessPattern` | `Dam` の実行系メソッドを一通り | 要る |
| `TestDataAccessUpdate` | 更新系（INSERT / UPDATE / DELETE）とトランザクション | 要る |
| `TestDataAccessDpq` 1〜5 | 動的 SQL の組み立て結果を**件数**で | 要る |
| `TestDataAccessDpq` 6 | 動的 SQL の**組み立て結果そのもの**（`ExecGenerateSQL`） | **要らない** |
| `DataProvider` | データ プロバイダごとの差異（Dam・囲い文字・パラメタ記号）を吸収 | — |
| `TestTable` | テスト用の表 `TestOrders` の作成・投入・破棄 | — |

> **`ExecGenerateSQL` は接続を使わない。**
> 他の `Exec*` と違って `_cmd.Connection` を設定せず、`PreExecQuery` も
> `CommandText` とパラメタしか触らない。
> **実装の都合で接続下から呼んでいるだけ**で、機能としては接続を必要としない。

### `TestSQLUtility` と `TestDataAccessUpdate` の役割分担

**重なっているのは「複合主キーの INSERT / UPDATE」だけ。** 残りは片方でしか見られない。

| 観点 | `TestSQLUtility` | `TestDataAccessUpdate` |
|---|---|---|
| 単一主キー | ○ | × |
| 複合主キー | ○ | ○（**重複**） |
| **生成しないこと**（更新対象列なし・主キーなし） | **○** | **×** |
| 生成された SQL テキストそのもの | ○ | × |
| 正しい行に正しい値が入るか | × | ○ |
| **CI での 4 DBMS 分の生成** | **○** | ×（CI は SQL Server のみ） |

**「生成しないこと」は実行では確かめられない。** 生成物が無いので実行しようがない。
これは #515 で追加したガード（`colSet.Count == 0 \|\| colWhere.Count == 0` の早期 return）の
回帰テストで、**外すと "SE" や "WH" のような壊れた文字列を返す退行を検知できなくなる。**

**CI で 4 DBMS 分を見られるのも `TestSQLUtility` だけ。** CI は SQL Server にしか接続しない。
**#515 は PostgreSQL / MySQL の不具合**だったので、ここが CI での唯一の防波堤になる。

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

### 更新系（`TestDataAccessUpdate`）

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

#### 表を作るときの注意

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


### 動的パラメタライズドクエリ（`TestDataAccessDpq`）

**同じ 1 本の XML から、パラメタの与え方だけで異なる SQL が組み立つ**ことを見る。
仕様は `opentouryo-query-definition` スキルが一次情報。

#### 6 つの観点

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

#### 1. `<WHERE>` / `<IF>` の組み合わせ

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

#### 2. `<IF>` / `<ELSE>`（テキスト内・タグ内の両方）

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

#### 3. `<LIST>`（IN 句）

1 つのパラメタ名に複数値を与えると `@名_1, @名_2 …` へ自動展開される。
**与えた数がそのまま件数に出る**ため、展開されたかを件数で判別できる（2 値 → 2 件、3 値 → 3 件）。

#### 4. パラメタの作用範囲

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

#### 5. `<DELCMA>` / `<INSCOL>`

列と値を対で増減させ、**どの要素が残っても構文が壊れない**ことを実際に INSERT して確かめる。
末尾を落とす場合と**先頭を落とす場合**の両方を通す（先頭側のカンマ除去が要るため）。

#### 6. 組み立て結果だけを見る（`ExecGenerateSQL`）

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

#### 書くときに踏んだ落とし穴

| 事象 | 原因 |
|---|---|
| `<IF>` で `ArgumentException` | **パラメタも `name` 属性も無い `<IF>`**。有効・無効の判断材料が無い。CDATA 内のパラメタは認識される |
| 整形式でない `.xml` で例外 | 「書式が不正なら静的にフォールバック」は**タグの綴り等**の話。XML として壊れていれば例外 |

#### パラメタの先頭記号は DBMS で異なる

| DBMS | 記号 |
|---|---|
| Oracle（`ODP`） | **`:`** |
| SQL Server / MySQL / PostgreSQL | `@` |

PostgreSQL は `:` から `@` に変更された経緯がある（`DamPstGrS.cs` の更新履歴）。

**XML は実行時に書き出して捨てる。** 記号と囲い文字が DBMS で異なるため、
共有の SQL 置き場に置くと 4 方言ぶんの重複になる。表と同じ考え方。

#### 記法の参考

`root/files/resource/Test/dpq/query/` に `DPQuery_Tool` 用の資産がある
（`<SUB>` / `<JOIN>` を使った複雑な例も含む）。

> あちらの `<PARAM>` タグは**ツールが値を与えるためのもの**。
> 実行時は `SetParameter` / `SetUserParameter` で与える。

#### 対象外

| 項目 | 理由 |
|---|---|
| パラメタの型・サイズ指定 | `SetParameter` の 3〜5 引数版 |
| Out / RetVal パラメタ、ストアド | 4 DBMS で書き方が大きく異なる |
| 埋め込みリソース | `MyBaseDao.UseEmbeddedResource`。配布形態の話 |
## クロス DB テストの実施手順（`LOCAL`）

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

#### 接続文字列

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

#### 実測（全 DBMS 起動時）

| DBMS | 参照系 5 パターン | 更新系 |
|---|---|---|
| SQL Server（`SQL`） | net48 5/5 ／ .NET 10 5/5 | **全項目一致** |
| Oracle（`ODP`） | net48 5/5 ／ .NET 10 5/5 | **全項目一致** |
| MySQL（`MCN`） | net48 5/5 ／ .NET 10 5/5 | **全項目一致** |
| PostgreSQL（`NPS`） | net48 対象外 ／ .NET 10 5/5 | **全項目一致**（.NET 10 のみ） |

更新系は **4 DBMS が同一の出力**になる（件数と値だけを出しているため）。

#### 失敗したときの切り分け

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

