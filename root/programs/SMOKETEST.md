# SMOKETEST.md — サンプルの疎通確認

対象: `root/programs/CS`（C# 側、既定）／ `root/programs/VB`（VB 側、`-Lang VB`）
配置: `root/programs`
本書は、リリース時に行っていた「サンプルを幾つか見繕って手動で疎通を行う」を、
**合否が出る形に機械化**するための手順と判定基準を記述する（#513 段階 3）。

---

## 1. 使い方

```powershell
cd root\programs

# 全対象の疎通を確認する
.\3_SmokeTest.ps1

# 一部だけ（動作確認用）
.\3_SmokeTest.ps1 -Only "net48"
.\3_SmokeTest.ps1 -Only "Rerunnable" -SkipBuild

# VB 側の疎通（10 節）
.\3_SmokeTest.ps1 -Lang VB
```

終了コードは `0` = 全対象 OK、`1` = NG あり。

| オプション | 内容 |
|---|---|
| `-Lang <CS\|VB\|Both>` | 対象の言語（既定 `CS`）。10 節 |
| `-Only <文字列>` | 対象名の部分一致で絞る |
| `-SkipBuild` | ビルドを省略し、既存のバイナリで疎通のみ行う |
| `-OutputDir <パス>` | ログの保存先（既定 `%TEMP%\OpenTouryoSmokeTest`） |

---

## 2. 段階 1・2 との違い

| | 見るもの | 期待値 |
|---|---|---|
| `1_BuildAll.ps1`（段階 2） | ビルドが通るか | エラー 0 件 |
| `2_RunAllTests.ps1`（段階 1） | 出力が前回と同じか | HEAD の `Result*.txt` |
| **`3_SmokeTest.ps1`（段階 3）** | **起動して想定どおり動くか** | **定義側に書いた判定条件** |

段階 1 は回帰テスト、段階 3 は疎通テストで、目的が違う。
疎通テストは期待結果ファイルを持たず、判定条件を `3_SmokeTest.ps1` の対象定義に書く。

**実行順は `1_BuildAll.ps1` → `2_RunAllTests.ps1` → `3_SmokeTest.ps1`。**
リリース時の作業全体は [`RELEASE.md`](RELEASE.md) を参照。

---

## 3. 対象（23 件）

### バッチ（8 件）

| 対象 | 判定 |
|---|---|
| `SimpleBatch_sample` (net48 / net10.0) | `〇件のデータがあります` が出力される |
| `RerunnableBatch_sample` (net48 / net10.0) | `Orders2` の件数が `Orders` と一致する |
| `RerunnableBatch_sample2` (net48 / net10.0) | 同上 |
| `RerunnableBatch_sample3` (net48 / net10.0) | 同上 |

`RerunnableBatch` 系は `Orders`(830 件) を読み `Orders2` へ INSERT する。3 本の違いは
INSERT の方法（1 件ずつ／SQL 連結／INSERT 文組み立て）で、いずれも結果は同じになる。

**実行前に `Orders2` を空にする必要がある。** `OrderID` が主キーのため、
残っていると重複で落ちる。スクリプトが `DELETE FROM [Orders2]` を行ってから実行する。
実行後は 830 件＝初期状態に戻るため、後始末は不要。

#### `Orders2` はスクリプトが作る

**`Orders2` は Northwind 標準の表ではない。** `instnwnd.sql` に含まれないため、
**DB を作り直すたびに消える**。そのたびに手で作るのは現実的でないので、
事前準備の中で存在を確認し、無ければ作る。

```
Orders2 がありません。作成します（...\RerunnableBatch_sample\CREATE ORDERS2.sql）。
```

DDL はサンプル同梱の `CREATE ORDERS2.sql` をそのまま流す。**スクリプトに書き写さない**
（同じ DDL がサンプル配下に 9 つ重複しており、増やす意味がない）。

流すときの注意が 2 つある。

- **`GO` は自前で分割する。** `SqlClient` は `GO` を解釈できない（`sqlcmd` の
  バッチ区切りであって T-SQL ではない）。このファイルは 3 バッチに分かれる。
- **`USE [Northwind]` は流さない。** 接続先は接続文字列に従うべきで、
  流すと接続文字列が別 DB を指していた場合にそちらへ表を作ってしまう。

> CI では `Set up Northwind` ステップが `instnwnd.sql` の直後に同じ DDL を流している。
> そちらは `sqlcmd` なので `GO` をそのまま解釈できる。

### CLI（1 件）

| 対象 | 判定 |
|---|---|
| `Simple_CLI` (net10.0) | `cmd1 --an-int 123` が `Sub command cmd1: 123` を出力する |

net48 版は `System.CommandLine` / `Sharprompt` の .NET Framework サポート終了により
ドロップされている（`5_Build_CLI_sample.bat` 参照）。
`interactive` サブコマンドは対話プロンプトを使うため対象外。

### DaoGen_Tool（墨壺）の CUI モード（6 件）

#508 で追加された CUI。net48 / net10.0 それぞれ 3 件。

> **ツール自体の使い方は
> [`CS/Frameworks/Tools/DaoGen_Tool/README.md`](CS/Frameworks/Tools/DaoGen_Tool/README.md) が一次情報。**
> 実行ファイルの場所、ヘルプの出し方、エージェントが踏みやすい罠を記載している。

| 対象 | 判定 |
|---|---|
| `/HELP` | ヘルプの見出しが出力される |
| `/CUI /MODE DAODEFGEN` | DB のスキーマから定義 CSV が生成され、対象テーブルが含まれる |
| `/CUI /MODE DAOSQLGEN` | 定義 CSV から Dao(`.cs`)・動的 SQL(`.xml`)・静的 SQL(`.sql`) が生成される |

**2 モードは連続して実行する。** `DAODEFGEN` が出力した定義 CSV を
`DAOSQLGEN` の入力に使うため、ツール単体ではなく
「DB → 定義 → 生成」の一連の流れを確認できる。

対象テーブルは `Shippers,Orders` の 2 つに絞っている。
全テーブルを回しても時間がかかるだけで、疎通としては同じことを見ているため。

生成先は `%TEMP%\OpenTouryoSmokeTest\daogen_*`。
実行前に前回の生成物を消す。残っていると「生成された」の判定が甘くなる。

> **GUI 側の確認は手作業に残る。** 引数なしで起動すると `Application.Run(new Form1())` になる。

#### パス区切りの注意

コマンドライン解析（`StringVariableOperator.GetCommandArgs`）は
**`\` をエスケープ文字として扱う**ため、パスの区切りは `/` にする。

```
OK : /OUTPUT "C:/temp/out"
OK : /OUTPUT "C:\\temp\\out"
NG : /OUTPUT "C:\temp\out"    ← \ が消える
```

ツール自身の `/HELP` にも記載されている。

#### 標準出力の捕捉

`DaoGen_Tool` は `WinExe` で、CUI 時は `AttachConsole(-1)` でコンソールに接続する。
このため**リダイレクトの方法によって出力が取れない**。

| 方法 | 結果 |
|---|---|
| `Start-Process -RedirectStandardOutput` | **取れる**（現在の方式） |
| PowerShell の `& $exe ... *>&1 \| Out-File` | 取れるが、**stderr が壊れる**（下記） |
| `cmd /c "... > file"` | 取れない（0 行） |

**`3_SmokeTest.ps1` は `Start-Process` でファイルへ直接リダイレクトする。**
パイプ（`\| Out-File`）でも出力自体は取れるが、そちらは別の問題を抱える（9 節）。

### DeployZipPackWithHTTP の CUI モード（4 件）

| 対象 | 判定 |
|---|---|
| `DeployZip /MFTGEN` (net48 / net10.0) | マニュフェストが生成され、**MD5 が同梱のものと一致する** |
| `DeployZip 配置` (net48 / net10.0) | **配置結果が圧縮前のフォルダと全ファイル一致する** |

`#528` で `/ZIPGEN`（ZIP 生成）と `/MFTGEN`（マニュフェスト生成）を追加し、
**圧縮 → マニュフェスト → 配布 → 配置**を CUI で通せるようになった。
前段の出力を後段の入力に使う点は `DaoGen_Tool` と同じ。

**配布物（ZIP）は追跡していない。** `Pre` で `FormAppRoot` から毎回作る。

```
/ZIPGEN /TOPONLY            → root.zip（ルート直下だけ、書庫内ルート無し）
/ZIPGEN /ROOTINZIP aaa      → aaa.zip （フォルダごと、書庫内ルート = aaa）
```

**両方のモードを通る。** GUI のチェック ボックス
（「個別のフォルダ圧縮」／「ルート フォルダからの圧縮」）に対応する分岐である。

`Sample/FormAppRoot` が配布前の姿なので、**配置結果を MD5 で突き合わせられる**（21 ファイル）。

**ここだけ ZIP 部品（`ZipperV2` / `UnZipperV2`）を通る。**
単体テスト（`TestCode/TestZipV2.cs`）は部品の振る舞いを見るが、
配布フロー全体を通すのはこの 2 件だけ。

#### マニュフェストの MD5 は計算し直して突き合わせる

```powershell
$want = $md5s[$i].Substring(4).Trim()
$got  = [Convert]::ToBase64String($md5.ComputeHash([IO.File]::ReadAllBytes($path)))
```

**作り置きの ZIP を追跡していた頃は、ここで気付けなかった。**
`FormAppRoot` を直したのに ZIP を作り直さないと MD5 が合わなくなるが、
比較相手も作り置きだと両方が古いままで通ってしまう。

#### 配信は IIS Express で立てる

Web アプリと違い**対象は EXE** なので、本文側の Web ホスト起動の仕組みには乗らない。
`Pre` で起動し、`Verify` の最後で止める。

**起動待ちにコンテンツを要求してはいけない。** 404 でも「起動している」ため、
応答の内容で判定すると、ファイルが無いときに待ち続け、
**IIS Express が残って次回のポート登録を塞ぐ**（`0x800700b7`）。TCP で繋がるかだけを見る。

#### 配置先は `-OutputDir` の下

同梱のマニュフェストは `c:\FormAppRoot\` を指すが、**疎通確認で環境を汚さない**よう、
`$OutputDir\deploy_<tag>\ins` を指すマニュフェストを作り直してから配布する。

#### `/NB` を必ず付ける

マニュフェストの `exe` 行のアセンブリを、**配置後に起動してしまう**ため。
起動すると GUI が開き、非対話の実行が止まる。

> 引数の渡し方に癖がある（`\` がエスケープされる・`/INSDIR` だけは `\` を残す・
> 空白を含む値は自分で引用符を付ける）。
> **一次情報は [`Tools/DeployZipPackWithHTTP/README.md`](CS/Frameworks/Tools/DeployZipPackWithHTTP/README.md) の 3.4 節。**

#### 古いビルドに新しいスイッチを渡すと、以前は GUI が開いた

**引数があるのにどのモードにも当たらない場合、GUI へ落ちる実装だった**（#528 で修正）。
非対話では画面が出たまま止まるため、疎通確認や CI が停止する。
現在は**エラーで終わる**（終了コード 1）。

`/ZIPGEN` を実装した直後、`.NET 10` 側を建て直さずに回して実際に踏んだ。
**片方のビルドだけを更新しない**こと。

### 通信制御の接続オプション（1 件）

`CallController` の接続オプション（`TMProtocolDefinition.xml` の `Prop`）が、
実際の HTTP 要求に反映されているかを見る（#546）。

| 対象 | 判定 |
|---|---|
| `TestTransmission` (net48) | 対象側が出す `NG : 0 件` |

対象は **`Frameworks/Tests/TestTransmission`** の net48 コンソールで、
**オリジンとプロキシを自前で立て、そこへ `CallController` から呼ぶ**。
判定は対象側が行い、項目ごとに `OK` / `NG` を出して末尾に件数を出す。

```
=== プロキシ認証（testProxyAuth）===
  [OK] 戻り値                 : サーバからの戻り値
  [OK] プロキシ経由              : True
  [OK] 要求行が絶対 URI          : True
  [OK] Proxy-Authorization : pxuser:pxpass

NG : 0 件
```

#### 見ているオプション（7 つ）

| オプション | 見ていること |
|---|---|
| `ProxyUrl` | プロキシに要求が届き、**要求行が絶対 URI** になっている |
| `PUserName` / `PPassword` | **407 → Basic で再送**され、値が届いている |
| `UserName` / `Password` | **401 → Basic で再送**され、値が届いている |
| `UserAgent` | オリジンに届いた `User-Agent` の値 |
| `Compression` | `Accept-Encoding` に gzip が付き、**gzip の応答を復号できている** |

**オプションはクライアント側の設定なので、サーバが受け取った内容を記録して
突き合わせないと、効いたかどうかを判定できない。** このため、値を返すだけの
オリジンではなく、**受け取ったヘッダを記録するオリジン**を用意している。

#### 外部環境が要らない

オリジンもプロキシも `TcpListener` で立て、**1 プロセスに閉じている**。

- **`HttpListener` を使わない。** URL の予約（`netsh http add urlacl`）が要る場合があり、
  `TcpListener` ならその手当てなしに動く
- **プロセスを分けない。** 起動順と後始末が要らず、`BinaryFormatter` の型解決も確実になる
- 使うポートは **51090（オリジン）と 51091（プロキシ）**

#### **宛先に実在しないホスト名を使っている理由**

**.NET Framework の `WebProxy.IsBypassed` は、ループバック宛を常に迂回する。**

```
net48   : http://127.0.0.1:51090/  IsBypassed=True    ← プロキシを使わない
.NET 10 : 同上                     IsBypassed=False   ← 使う
```

`127.0.0.1` や `localhost` を宛先にすると、**プロキシを設定していても直接オリジンへ行き、
プロキシの検証にならない**（実際、最初の実行は「プロキシ 0 回」だった）。

このため、プロキシを使うケースの宛先を **`http://fx-origin.test:51090/`** とし、
**テスト用プロキシが名前を解決せず、必ずオリジンへ繋ぎ替える**ようにしてある。
`hosts` ファイルの編集（管理者権限）も、ファイアウォールへの露出も要らない。

#### 対象外のオプション

| | 理由 |
|---|---|
| `CertFile` / `CertPassword` | TLS とクライアント証明書の要求が要る。オリジンの Kestrel 化が前提 |
| `Domain` / `PDomain` | Windows 統合認証が前提。Basic 認証では無視される |
| `ConnGroupName` | **効果が無い。** HttpWebRequest の接続プールを分割する名前だったが、HttpClient へ移って**設定する口が無くなった**。目的（接続の仕切り）は、CallController が**サービス名ごとにハンドラをプールする**ことで満たされている。定数は互換のため残し、定義例からは外した |

#### net48 だけである理由

ASP.NET WebAPI の経路（`protocol="5"`）は **.NET Framework 限定**である。
引数と戻り値の受け渡しに `BinarySerialize` を使っており、
.NET Core 版では `FxEnum.TmProtocol` ごと落とされている
（[`CS/Frameworks/ANALYSIS.md`](CS/Frameworks/ANALYSIS.md) と #543）。

### Web アプリ（3 件）

| 対象 | ホスト | 認証の実装 | 判定 |
|---|---|---|---|
| `MVC_Sample` (net48) | IIS Express | FormsAuthentication | ログイン後 `/Crud1/Index` が 200 |
| `WebForms_Sample` (net48) | IIS Express | FormsAuthentication | ログイン後 `menu.aspx` が 200 |
| `MVC_Sample` (net10.0) | Kestrel | Cookie 認証 | ログイン後 `/Crud1/Index` が 200 |

**3 つとも確認の深さを揃えている。** 入口ページが 200 を返すだけでは
ホスティングと構成しか確認できないため、いずれも**ログインを通し、
認証が要る画面に到達できること**まで見る。

1. ログイン画面を GET … 画面が出ること。ページが発行した状態を取り出す
   （MVC は `__RequestVerificationToken`、WebForms は `__VIEWSTATE` 等）
2. ログインを POST … 偽造防止の検証を通ること
3. 認証が要る画面を GET … **未認証なら 302 になるので、200 なら認証が通っている**

これでホスティング・構成・ルーティング・認証・セッションまでを一度に確認できる。
いずれのサンプルも「ユーザー名が空でなければ認証する」実装のため、資格情報は不要。

> **未認証時に 302 が返ることは実測で確認済み。** 200 が偶然でないことの裏付けになる。
>
> ```
> MVC_Sample (net48)  未認証で /Crud1/Index          → 302
> WebForms_Sample     未認証で /Aspx/start/menu.aspx → 302
> ```

`WebForms_Sample` は `Web.config` の `<deny users="?" />` で全画面が要認証になっており、
ログイン後は `<forms defaultUrl="Aspx/Start/menu.aspx">` の画面へ遷移する。
ポストバックには画面が発行した `__VIEWSTATE` / `__VIEWSTATEGENERATOR` / `__EVENTVALIDATION` を
そのまま返す必要があり、コントロール名はマスタ ページ配下のため
`ctl00$ContentPlaceHolder_A$` が付く。

### 対象外

| 対象 | 理由 |
|---|---|
| `2CS_sample` 系（11 本） | WinForms / WPF。UI Automation が必要で、画面変更に弱く維持費が高い |
| `WSClient_sample` 系（7 本） | 同上 |
| Web サービス（`ASPNETWebService`） | **別リポジトリへ移設済み**。本リポジトリにホストが無い |

`CS/Samples/WS_sample/WSServer_sample` はクラス ライブラリ（B層・D層）で、
これを載せる Web サービスは
[`OpenTouryoProject/ResourceServerTemplates`](https://github.com/OpenTouryoProject/ResourceServerTemplates)
へ移設されている。このため本リポジトリだけでは HTTP 疎通ができない。

WinForms / WPF 系は、リリース チェックリスト（段階 4）の**手作業項目**として残す。

---

## 4. 前提条件

- **SQL Server の Northwind に接続できる**こと
  - 接続文字列は `CS\Samples\Bat_sample\SimpleBatch_sample\App.config` の
    `ConnectionString_SQL` を読む。ここで別途ハードコードすると追随できなくなるため
- **IIS Express** がインストールされていること（net48 の Web アプリ）
- **ASP.NET 状態サービスが開始されている**こと（net48 の Web アプリ）
- **ポート 51081 - 51086、51090、51091 が空いている**こと
  - 51081 - 51083 … Web アプリ（net48 の 2 つと net10.0）
  - 51084 … `DeployZipPackWithHTTP` の配信
  - 51085 - 51086 … VB 側の Web アプリ（`-Lang VB`。10 節）
  - **51090 - 51091 … 通信制御のオリジンとプロキシ（3 節）**

> **GitHub Actions でも実行している。** 前提の揃え方（SQL Server の導入と Northwind の
> ロード、`C:\root`、ロケール、`aspnet_state` の開始）は
> [`BUILDING.md`](BUILDING.md) 9 節が一次情報。
> IIS Express は `windows-latest` に同梱されているため、追加の導入は要らない。

### ASP.NET 状態サービス

`MVC_Sample` / `WebForms_Sample` の `Web.config` は `StateServer` を使う。

```xml
<sessionState cookieName="mvc_session" timeout="20" cookieless="false"
              mode="StateServer" stateConnectionString="tcpip=127.0.0.1:42424"/>
```

サービスが止まっていると、ログインの POST が **500** になる。

```
System.Web.HttpException: セッション状態要求をセッション状態サーバーに対して作成できませんでした。
```

`3_SmokeTest.ps1` は実行前に確認し、止まっていれば「前提未達」として対処方法を示す。

```powershell
Start-Service aspnet_state      # 管理者権限が必要
```

**スクリプトはサービスを自動起動しない。** システムの状態を変える操作であり、
リリース判定のために黙って環境を書き換えるべきではないため。

---

## 5. 判定基準

対象ごとに次のいずれかで判定する。

| 種別 | 判定 |
|---|---|
| 出力の照合 | 標準出力が `Expect` の正規表現に一致するか |
| 追加検証 | `Verify` のスクリプト ブロックが `$true` を返すか（DB の件数など） |
| Web の疎通 | `Flow` のスクリプト ブロックが `Ok = $true` を返すか |

いずれの場合も、**出力に未処理例外が含まれていれば NG**。ただし次は除外する。

- **`Console.ReadKey()` 由来の例外**
  … サンプルは末尾に `Console.ReadKey()` を持つものがあり、出力をリダイレクトすると
  必ず例外で終わる。テスト内容とは無関係

---

## 6. ビルドについて

**リポジトリ既定のビルド バッチを呼ぶ。** 理由は `2_RunAllTests.ps1` と同じで、
`csproj` を直接 MSBuild すると `nuget.exe restore` が行うネイティブ DLL の配置が漏れ、
**ビルドは成功するのに実行時に落ちる**（`TESTING.md` の「ビルドをバッチに委ねている理由」）。

### `1_BuildAll.ps1` の後にバイナリが残らない理由

`0_ExecAllBat.bat` は途中で `1_DeleteDir.bat` を繰り返し実行し、
配下の `bin` / `obj` / `packages` 等を**再帰的に**削除する。

```
… net48 サンプルをビルド …
Clean (core サンプル)   ← ここで net48 サンプルの bin も消える
… Core サンプルをビルド …
```

このため `1_BuildAll.ps1` の完走後に残るのは最後にビルドされた Core サンプルだけで、
net48 サンプルのバイナリは残らない。`3_SmokeTest.ps1` が自分でビルドするのはこのため。

---

## 7. 修正の経緯 : バッチ サンプルが実行時に落ちていた

着手時、net48 のバッチ サンプル 4 本がすべて起動直後に落ちていた。

```
System.DllNotFoundException
   場所 Microsoft.Data.SqlClient.SNINativeManagedWrapperX64.SNIInitialize(IntPtr)
```

原因は、`5_Build_Bat_sample.bat` と `6_Build_WSSrv_sample.bat` に
**`nuget.exe restore` が無かった**こと。`Microsoft.Data.SqlClient` は SNI を
ネイティブ DLL で持つため、restore を経ないと `bin` に配置されない。

**ビルドは成功する**ため、段階 2（ビルドの合否判定）では検出できない。
疎通テストで初めて表面化する種類の不具合である。

他の 11 バッチと同じ形式で `nuget.exe restore ... %NUGET_MSBUILD%` を追加した。

---

## 7.2 修正の経緯 : 対象の起動方法を `Start-Process` にした

**当初は PowerShell のパイプで実行していた。**

```powershell
& $exe @($t.Args) *>&1 | Out-File $out -Encoding UTF8
```

この形は 2 つの問題を抱えていて、**対症療法を 3 回繰り返しても直らなかった。**

### ① stdin を与えないと止まる

サンプルは終了前に `Console.ReadKey()` を呼ぶ。stdin がコンソールのままだと
**本当にキー入力を待つ**。実測で `SimpleBatch_sample (net48)` が **386 秒**かかった。

**CI では stdin が元からリダイレクトされているため顕在化しない。** 実機だけで止まる。

### ② stderr が壊れる

パイプで受けると、native の stderr が `ErrorRecord` になり**コンソール幅で折り返される。**
折り返しは**語の途中にも入る**。

```
Cannot read keys when either application does not h ave a console ...
                                                   ↑ 単語が割れている
```

**空白を畳んでも復元できない**（折り返しは何かを置き換えたのではなく、挿入されたため）。
幅は環境ごとに違うので、判定側の除外規則をいくら調整しても直らない。

### 現在の方式

**`Start-Process` でファイルへ直接リダイレクトする。** PowerShell を経由しないので、
出力は生のまま残り、環境による差も出ない。

```powershell
Start-Process $exe -ArgumentList $argList -NoNewWindow -Wait `
    -WorkingDirectory (Split-Path $exe) `
    -RedirectStandardInput $emptyIn -RedirectStandardOutput $out -RedirectStandardError $err
```

`-RedirectStandardInput` には**空ファイル**を渡す（実在するファイルが要る）。

> **読み戻しには `-Encoding UTF8` を必ず付ける。**
> `Start-Process` は子プロセスの生バイトをそのまま書くため **BOM が付かない**。
> Windows PowerShell 5.1 の `Get-Content` は BOM が無いと既定の ANSI（CP932）で読むため、
> **日本語の期待値だけが一致しなくなる**（ASCII の期待値は通るので気付きにくい）。
> 従来の `Out-File -Encoding UTF8` は BOM 付きだったため、たまたま成立していた。

**副次的に、`DaoGen_Tool` 実行時の画面の乱れも解消した。**
`AttachConsole(-1)` が親コンソールへ直接書き込んでいたのが原因で、
独立したリダイレクト先を持つようになったため起きなくなった。

---

## 8. 実行結果の例

```
対象                              結果 内容
----                              ---- ----
SimpleBatch_sample (net48)        OK   3件のデータがあります
RerunnableBatch_sample (net48)    OK
RerunnableBatch_sample2 (net48)   OK
RerunnableBatch_sample3 (net48)   OK
SimpleBatch_sample (net10.0)      OK   3件のデータがあります
RerunnableBatch_sample (net10.0)  OK
RerunnableBatch_sample2 (net10.0) OK
RerunnableBatch_sample3 (net10.0) OK
Simple_CLI (net10.0)              OK   Sub command cmd1: 123
DaoGen_Tool /HELP (net48)         OK   DaoGen_Tool（D層自動生成ツール／墨壺）
DaoGen_Tool DAODEFGEN (net48)     OK   生成が完了しました。
DaoGen_Tool DAOSQLGEN (net48)     OK   生成が完了しました。
DaoGen_Tool /HELP (net10.0)       OK   DaoGen_Tool（D層自動生成ツール／墨壺）
DaoGen_Tool DAODEFGEN (net10.0)   OK   生成が完了しました。
DaoGen_Tool DAOSQLGEN (net10.0)   OK   生成が完了しました。
DeployZip /MFTGEN (net48)         OK   マニュフェスト ファイルを生成しました。
DeployZip 配置 (net48)            OK   履歴に新規追加しました。
DeployZip /MFTGEN (net10.0)       OK   マニュフェスト ファイルを生成しました。
DeployZip 配置 (net10.0)          OK   履歴に新規追加しました。
TestTransmission (net48)          OK   NG : 0 件
MVC_Sample (net48)                OK   ログイン後 /Crud1/Index = 200
WebForms_Sample (net48)           OK   ログイン後 menu.aspx = 200
MVC_Sample (net10.0)              OK   ログイン後 /Crud1/Index = 200

  全対象 OK
```

全 23 件（ビルド 8 バッチ ＋ 疎通 23 件）で **約 4 分**。

### リダイレクトの扱い

`Invoke-WebRequest` は `-MaximumRedirection 0` で 3xx を受け取ると、
`-SkipHttpErrorCheck` を付けていても
「The maximum redirection count has been exceeded」で終了エラーになる。

ログイン成功時は `FormsAuthentication` が 302 を返すため、これに該当する。
`3_SmokeTest.ps1` は `Invoke-Http` で捕まえ、3xx を正常な結果として扱う。
素の `Invoke-WebRequest` を使うと、**判定は通るのにエラーが表示される**状態になる。

---

## 9. 対象を追加するとき

`3_SmokeTest.ps1` の `$targets` に定義を足す。

| 項目 | 内容 |
|---|---|
| `Name` | 表示名 |
| `Bat` | ビルドに使うバッチ（`root\programs\CS` 配下） |
| `Exe` | 実行ファイル。`.dll` なら `dotnet` で実行する |
| `Args` | コマンドライン引数 |
| `Pre` | 実行前の準備（スクリプト ブロック） |
| `Expect` | 標準出力に対する正規表現 |
| `Verify` | 追加の検証（スクリプト ブロック） |
| `Kind` | `Web` を指定すると Web アプリ扱い |
| `WebHost` | `IISExpress` または `Kestrel` |
| `Site` / `Port` / `Flow` / `Need` | Web アプリ用 |

**判定条件は「動いていれば必ず満たす」ものにする。**
実行のたびに変わる値（件数以外の可変値、日時など）を条件に入れると、
環境差で落ちるだけの脆いテストになる。

### `Args` は `Pre` より前に組み立てられる

対象定義（`$targets`）は**スクリプトの読み込み時に評価される**。
`Pre` が作るフォルダを `Args` の組み立てで参照すると、**まだ存在しない。**

```powershell
NG : $zips = Get-ChildItem (Join-Path $OutputDir "deploy_net48\web") ...  ← Pre がまだ動いていない
OK : $zips = Get-ChildItem $deploySampleWeb ...                           ← リポジトリ側から採る
```

実行時に決めたいなら、`Args` ではなく `Pre` の中で `$script:` 変数に入れて渡す。

### 対象が EXE でも Web サーバが要ることがある

`Kind = "Web"` は**対象自身が Web アプリ**のときの仕組みで、
「EXE の相手として Web サーバを立てたい」場合には使えない。
`Pre` で起動し、`Verify` の最後で止める（`DeployZipPackWithHTTP` がこの形）。

**起動待ちに、特定のコンテンツを要求してはいけない。**
404 でも「起動している」ため、応答の内容で待つと、
ファイルが無いときに待ち続けたうえで**プロセスが残る**。
残った IIS Express は URL の登録を握るので、
**次回以降の起動が `0x800700b7`（既に存在する）で失敗し続ける。**

```powershell
# TCP で繋がるかだけを見る。開始前に前回の残りも止める。
$client = New-Object System.Net.Sockets.TcpClient
$client.Connect("localhost", $port)
```

### `dotnet` への引数の渡し方

`/DAP` のように `/` で始まる引数は `--` で区切って渡す必要がある。
一方 `System.CommandLine` を使う CLI では `--` 以降が未解析トークン扱いになり、
**サブコマンドが認識されなくなる**。`3_SmokeTest.ps1` は引数を見て自動で切り替える。

### PowerShell 5.1 と 7 の両対応

**両方で動くこと。** 開発時に `pwsh`（7）だけで確認すると、
利用者が `powershell.exe`（5.1）で実行したときに落ちる。

> **規約の実体は
> [`CODING.md`](CODING.md) の 5 節**にある。
> 落とし穴の一覧と対処方法は、そちらを参照すること。

本スクリプト群は、次の 4 点を**実際に踏んだ**うえで対処してある。
同種のスクリプトを追加・変更するときの実例として挙げる。

| 事象 | 踏んだ箇所 |
|---|---|
| BOM 無しで構文エラー・文字化け | 5 本すべて（`0_RunAll.ps1` ほか） |
| `Get-Content` の既定エンコード差で、同じファイルなのに差分が出る | `CompareResult.ps1` |
| `-SkipHttpErrorCheck` が 5.1 に無く、HTTP が常に失敗 | `3_SmokeTest.ps1` の `Invoke-Http` |
| `chcp` による画面クリアと、ログの文字化け | 3 本すべて（冒頭でコード ページを切り替え） |


### PowerShell から `.bat` を呼ぶときの注意

`TESTING.md` と同じく、`NoDefaultCurrentDirectoryInExePath` を解除している。
解除しないと、バッチ内でパス区切りを含まない名前で起動している exe が動かない。

---

## 10. VB 側の疎通（`-Lang VB`）

```powershell
.\3_SmokeTest.ps1 -Lang VB     # VB のみ（6 件）
.\3_SmokeTest.ps1 -Lang Both   # C# 23 件 ＋ VB 6 件
```

**既定に VB を含めない。** リリース時の検証（[`RELEASE.md`](RELEASE.md) 3 節）は
C# 側が対象で、VB を含めると毎回 VB のビルドが乗るためである。

### 対象は 6 件

| C# の対象 | 件数 | VB |
|---|---|---|
| `Bat_sample` (net48) | 4 | **あり** |
| `Bat_sample` (net10.0) | 4 | 無し（Core 版が無い） |
| `CLI_sample` (net10.0) | 1 | 無し（#533 で削除） |
| `DaoGen_Tool` | 6 | 無し（`Frameworks/Tools` は C# のみ） |
| `DeployZipPackWithHTTP` | 4 | 同上 |
| `MVC` / `WebForms` (net48) | 2 | **あり** |
| `MVC` (net10.0) | 1 | 無し |
| 通信制御の接続オプション | 1 | 無し（net48 の C# 側だけ） |

### 判定は C# 版と共有している

VB 版は C# 版からの移植で、疎通の手順がそのまま通る。

| | CS | VB |
|---|---|---|
| 接続文字列名 | `ConnectionString_SQL` | **同一** |
| MVC のログイン画面 | `@Html.AntiForgeryToken()` / `UserName` / `Password` / `name="normal"` | **`Views/Home/Login.cshtml` が同一** |
| 認証が要る画面 | `/Crud1/Index` | `Crud1Controller.vb` に `<Authorize>` |
| WebForms | `ctl00$ContentPlaceHolder_A$txtUserID` ほか | **同一** |

このため `$targetsVB` は `Flow` / `Verify` / `Args` に**同じ変数を指している**。
別ファイル（`3_SmokeTestVB.ps1`）に分けると、**サンプルの画面を直したとき
片方だけ直す事故**が起きるため、そうしていない（#542）。

**Web の待ち受けポートだけは分けてある**（`51085` / `51086`）。
`-Lang Both` では C# 版（`51081` / `51082`）と続けて起動するため。

### VB のビルドは自己完結しない

C# 側は各ビルド バッチが単独で通るが、VB 側は違う。
`5_Build_Bat_sample.bat` だけを呼んでも、参照するアセンブリが揃っていないため建たない。
このため `$prerequisitesVB` として、`VB\0_ExecAllBat.bat` が前段で行っていることを先に通す。

```
CS\2_Build_NuGet_net48.bat            1_GetLibrariesFromCS.bat が取りに行く実体
VB\1_GetLibrariesFromCS.bat           CS の Build_net48 を VB 配下へ複写
VB\3_Build_Business_net48.bat
VB\3_Build_BusinessRichClient_net48.bat
VB\4_Build_CopyAssemblies.bat         参照解決用の Build フォルダを作る
```

**`0_ExecAllBat.bat` を丸ごと呼んではいない。** 理由は 3 つ。

- `1_DeleteDir.bat` が `bin` / `obj` を消す。疎通は成果物を使うので、消したくない
- WinForms / WPF のサンプルまで建てる。疎通の対象ではないので、時間だけ増える
- `timeout 5` が入っており、stdin をリダイレクトすると
  `ERROR: Input redirection is not supported` がログに出る

ビルドの単位は「フォルダ ＋ バッチ」で一意化している。
`5_Build_Bat_sample.bat` のように**同じ名前のバッチが CS と VB の両方にある**ため、
バッチ名だけでは一意化できない。

### ネイティブ DLL の欠落（#542 で修正）

VB のサンプル ビルド バッチが `nuget restore` を呼んでおらず、
`Microsoft.Data.SqlClient.SNI.x64.dll` が出力に入っていなかった。
**ビルドは通り、実行時にだけ `System.DllNotFoundException` で落ちる。**

この疎通確認を用意して初めて分かった不具合である。経緯は
[`BUILDING.md`](BUILDING.md) 10 節。
