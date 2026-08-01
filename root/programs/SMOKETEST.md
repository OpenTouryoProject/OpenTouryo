# SMOKETEST.md — サンプルの疎通確認

対象: `root/programs/CS`（C# 側）
配置: `root/programs`
本書は、リリース時に行っていた「サンプルを幾つか見繕って手動で疎通を行う」を、
**合否が出る形に機械化**するための手順と判定基準を記述する（#513 段階 3）。

---

## 1. 使い方

```powershell
cd root\programs

# 全対象の疎通を確認する
.\SmokeTest.ps1

# 一部だけ（動作確認用）
.\SmokeTest.ps1 -Only "net48"
.\SmokeTest.ps1 -Only "Rerunnable" -SkipBuild
```

終了コードは `0` = 全対象 OK、`1` = NG あり。

| オプション | 内容 |
|---|---|
| `-Only <文字列>` | 対象名の部分一致で絞る |
| `-SkipBuild` | ビルドを省略し、既存のバイナリで疎通のみ行う |
| `-OutputDir <パス>` | ログの保存先（既定 `%TEMP%\OpenTouryoSmokeTest`） |

---

## 2. 段階 1・2 との違い

| | 見るもの | 期待値 |
|---|---|---|
| `BuildAll.ps1`（段階 2） | ビルドが通るか | エラー 0 件 |
| `RunAllTests.ps1`（段階 1） | 出力が前回と同じか | HEAD の `Result*.txt` |
| **`SmokeTest.ps1`（段階 3）** | **起動して想定どおり動くか** | **定義側に書いた判定条件** |

段階 1 は回帰テスト、段階 3 は疎通テストで、目的が違う。
疎通テストは期待結果ファイルを持たず、判定条件を `SmokeTest.ps1` の対象定義に書く。

**実行順は `BuildAll.ps1` → `RunAllTests.ps1` → `SmokeTest.ps1`。**
リリース時の作業全体は [`RELEASE.md`](RELEASE.md) を参照。

---

## 3. 対象（18 件）

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

### CLI（1 件）

| 対象 | 判定 |
|---|---|
| `Simple_CLI` (net10.0) | `cmd1 --an-int 123` が `Sub command cmd1: 123` を出力する |

net48 版は `System.CommandLine` / `Sharprompt` の .NET Framework サポート終了により
ドロップされている（`5_Build_CLI_sample.bat` 参照）。
`interactive` サブコマンドは対話プロンプトを使うため対象外。

### DaoGen_Tool（墨壺）の CUI モード（6 件）

#508 で追加された CUI。net48 / net10.0 それぞれ 3 件。

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
| PowerShell の `& $exe ... *>&1 \| Out-File` | **取れる**（68 行） |
| `cmd /c "... > file"` | 取れない（0 行） |

`SmokeTest.ps1` は前者で実行している。

### Web アプリ（3 件）

| 対象 | ホスト | 判定 |
|---|---|---|
| `MVC_Sample` (net48) | IIS Express | ログイン後 `/Crud1/Index` が 200 |
| `WebForms_Sample` (net48) | IIS Express | `login.aspx` が 200・`__VIEWSTATE` あり |
| `MVC_Sample` (net10.0) | Kestrel | `/` が 200 |

`MVC_Sample` (net48) は単なる 200 応答では終わらせず、**ログインを通している**。

1. `GET /Home/Login` … 画面が出ること。`__RequestVerificationToken` を取り出す
2. `POST /Home/Login` … `ValidateAntiForgeryToken` を通ること
3. `GET /Crud1/Index` … `[Authorize]` の画面。未認証なら 302 になるので、**200 なら認証が通っている**

これで、ホスティング・構成・ルーティング・認証・セッションまでを一度に確認できる。
サンプルの `FormsAuthentication` はユーザー名が空でなければ認証されるため、資格情報は不要。

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
- **`Orders2` テーブルが存在する**こと
  - Northwind 標準ではない。無い場合は
    `CS\Samples\Bat_sample\RerunnableBatch_sample\CREATE ORDERS2.sql` を実行する
- **IIS Express** がインストールされていること（net48 の Web アプリ）
- **ASP.NET 状態サービスが開始されている**こと（net48 の Web アプリ）

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

`SmokeTest.ps1` は実行前に確認し、止まっていれば「前提未達」として対処方法を示す。

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

**リポジトリ既定のビルド バッチを呼ぶ。** 理由は `RunAllTests.ps1` と同じで、
`csproj` を直接 MSBuild すると `nuget.exe restore` が行うネイティブ DLL の配置が漏れ、
**ビルドは成功するのに実行時に落ちる**（`TESTING.md` の「ビルドをバッチに委ねている理由」）。

### `BuildAll.ps1` の後にバイナリが残らない理由

`0_ExecAllBat.bat` は途中で `1_DeleteDir.bat` を繰り返し実行し、
配下の `bin` / `obj` / `packages` 等を**再帰的に**削除する。

```
… net48 サンプルをビルド …
Clean (core サンプル)   ← ここで net48 サンプルの bin も消える
… Core サンプルをビルド …
```

このため `BuildAll.ps1` の完走後に残るのは最後にビルドされた Core サンプルだけで、
net48 サンプルのバイナリは残らない。`SmokeTest.ps1` が自分でビルドするのはこのため。

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
MVC_Sample (net48)                OK   ログイン後 /Crud1/Index = 200
WebForms_Sample (net48)           OK   login.aspx = 200（__VIEWSTATE あり）
MVC_Sample (net10.0)              OK   GET / = 200 (3694 bytes)

  全対象 OK
```

全 18 件（ビルド 7 バッチ ＋ 疎通 18 件）で **約 2.4 分**。

### リダイレクトの扱い

`Invoke-WebRequest` は `-MaximumRedirection 0` で 3xx を受け取ると、
`-SkipHttpErrorCheck` を付けていても
「The maximum redirection count has been exceeded」で終了エラーになる。

ログイン成功時は `FormsAuthentication` が 302 を返すため、これに該当する。
`SmokeTest.ps1` は `Invoke-Http` で捕まえ、3xx を正常な結果として扱う。
素の `Invoke-WebRequest` を使うと、**判定は通るのにエラーが表示される**状態になる。

---

## 9. 対象を追加するとき

`SmokeTest.ps1` の `$targets` に定義を足す。

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

### `dotnet` への引数の渡し方

`/DAP` のように `/` で始まる引数は `--` で区切って渡す必要がある。
一方 `System.CommandLine` を使う CLI では `--` 以降が未解析トークン扱いになり、
**サブコマンドが認識されなくなる**。`SmokeTest.ps1` は引数を見て自動で切り替える。

### PowerShell 5.1 と 7 の両対応

**両方で動くこと。** 開発時に `pwsh`（7）だけで確認すると、
利用者が `powershell.exe`（5.1）で実行したときに落ちる。既踏の 3 点。

| 事象 | 原因 | 対処 |
|---|---|---|
| 構文エラー・文字化け（`繧ｹ繝・ャ繝・`） | 5.1 は BOM 無し `.ps1` を **ANSI（Shift_JIS）**として読む | `.ps1` を **UTF-8 BOM 付き**で保存する |
| 同じファイルなのに差分が出る | `Get-Content` の既定エンコードが 5.1 は ANSI、7 は UTF-8 | `-Encoding UTF8` を明示する |
| HTTP が常に失敗（状態コード `-1`） | `-SkipHttpErrorCheck` は **7 以降にしかない** | バージョンを見て付け外しする |
| 実行中に画面が一度クリアされ、それまでの結果が消える | `z_Common.bat` の `chcp 65001` | スクリプト冒頭で先に切り替える |

```powershell
if ($PSVersionTable.PSVersion.Major -ge 6) { $p.SkipHttpErrorCheck = $true }
```

5.1 では 4xx/5xx が例外になるため、`catch` で `$_.Exception.Response` から状態コードを取る。
`-UseBasicParsing` は 5.1 で必要（未構成の IE エンジンを避ける）、7 では無視されるので常に付けてよい。

#### コード ページによる画面クリア

`z_Common.bat` は先頭で `chcp 65001` を実行する。
**コード ページはコンソール全体の設定**なので、子プロセスで変えても呼び出し元の画面に影響し、
日本語環境の既定 `932` からの切り替わりで**画面が再描画されてそれまでの表示が消える**。

`z_Common.bat` を最初に呼ぶのは 3 ステップ目（`2_Build_NuGet_net48.bat`）のため、
そこまでの結果が消えて見える。各スクリプトは**冒頭で先に切り替えて**これを避けている。

```powershell
if ((cmd /c chcp) -notmatch '65001')
{
    cmd /c chcp 65001 | Out-Null
}

if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}
```

- **元に戻さない。** 戻す操作でも再描画が起きてサマリが消える。
  各ビルド バッチも `65001` のまま終了するので、挙動は従来と同じ
- **2 つの判定を分けること。** コンソールのコード ページと
  `[Console]::OutputEncoding` は別物で、揃っているとは限らない

#### なぜ判定を分けるのか

Windows PowerShell 5.1 の `[Console]::OutputEncoding` は**起動時の値のまま**で、
実行中にコンソールのコード ページが変わっても追随しない。
このため次の状態が生まれる。

| | コンソール CP | `OutputEncoding` |
|---|---|---|
| 5.1 を `932` で起動 | 932 | 932 |
| ビルド バッチが `chcp 65001` | **65001** | 932 ← 取り残される |
| **同じ画面で 2 回目を実行** | 65001 | 932 |

2 回目は「コード ページは既に 65001」なので `chcp` の分岐を通らない。
`OutputEncoding` の設定を同じ分岐に入れておくと**永久に 932 のまま**になり、
UTF-8 で出力されるバッチの内容を CP932 で解釈して**ログとエラー一覧が文字化けする**。

```
error MSB3482: 鄂ｲ蜷堺ｸｭ縺ｫ繧ｨ繝ｩ繝ｼ縺檎匱逕溘＠縺ｾ縺励◆   ← 化けた状態
error MSB3482: 署名中にエラーが発生しました              ← 正しい状態
```

### PowerShell から `.bat` を呼ぶときの注意

`TESTING.md` と同じく、`NoDefaultCurrentDirectoryInExePath` を解除している。
解除しないと、バッチ内でパス区切りを含まない名前で起動している exe が動かない。
