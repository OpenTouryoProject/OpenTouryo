# DeployZipPackWithHTTP（ZIP パッケージの HTTP 配布ツール）

WWW サーバに置いた ZIP を HTTP で取得し、クライアントへ展開するツール。
**マニフェスト ファイル（`*.mft`）が配布の単位**で、そこに配置先・起動 EXE・
ZIP の一覧と MD5 を書く。

GUI と CUI の両方を持ち、**CUI は非対話**なのでエージェントから実行できる。

本書は**エージェントが実行するため**の情報を記す。
**引数の一覧は書かない。** `/HELP` を実行すれば得られ、二重管理になるため。

---

## 1. ヘルプの実行（まずこれを見る）

```powershell
# net48
cd root\programs\CS\Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug
.\OpenTouryo.DeployZipPackWithHTTP.exe /HELP

# .NET 10
cd root\programs\CS\Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\net10.0-windows7.0
.\OpenTouryo.DeployZipPackWithHTTP.exe /HELP
```

`/HELP` が**引数仕様の一次情報**。次が得られる。

- 起動の切り替え（引数なし＝GUI、`/HELP`、`/CUI`）
- CUI のオプション（強制更新・出力抑止・起動抑止）
- WWW / Proxy の認証情報
- アンインストール

**`/HELP` は GUI アプリのままコンソールへ書く**（`AttachConsole`）。
標準出力を取るには次節のリダイレクトが要る。

---

## 2. 標準出力は `Start-Process` で受ける

**パイプで受けてはいけない。** WinForms アプリ（`WinExe`）であり、
`AttachConsole` で親コンソールに書くため、`|` では取りこぼす。

```powershell
$exe = "...\bin\Debug\OpenTouryo.DeployZipPackWithHTTP.exe"
$out = "$env:TEMP\dz_out.txt"

Start-Process $exe -ArgumentList "/CUI","/NB","/WWWURL","http://localhost:51099/FormAppRoot.mft" `
    -NoNewWindow -Wait -WorkingDirectory (Split-Path $exe) `
    -RedirectStandardOutput $out

Get-Content $out -Encoding UTF8 | Where-Object { $_ -notmatch "^log4net" }
```

**log4net の内部トレースが大量に混ざる**ので、読むときは落とすとよい。

---

## 3. エージェントが踏みやすい罠

### 3.1 `/NB` を付けないと EXE が起動する

マニフェストの `exe` 行の**先頭**のアセンブリを、**配置後に `Process.Start` する。**

```csharp
if (Program.IsBoot)          // /NB が指定されていない
{
    ...
    p.Start();               // 配置した EXE を実行する
}
```

**中身を確かめていない配布物に対しては、必ず `/NB` を付ける。**
起動されると GUI が開き、非対話の実行が止まる。

> `/NB` が抑止するのは**起動だけ**。`exe` 行を使った
> 「動いていたら配置を中止する」判定は、`/NB` を付けても働く（4 節）。

### 3.2 配置先はマニフェストが決める。環境変数が使える

`ins` 行の値がそのまま配置先になる。**引数で上書きできない。**
別の場所へ入れたいならマニフェストを作り直す。

**`%変数%` は展開される。**

```csharp
string InsDir = StringVariableOperator.BuiltStringIntoEnvironmentVariable(entry.InstallDir);
```

```
ins %TEMP%\FormAppRoot\          ← 環境に依存しない
ins C:\Users\xxxx\...\FormAppRoot\   ← 特定の環境でしか動かない
```

**手で書くマニフェストは環境変数を使うこと。** 絶対パスを直書きすると、
別の環境やサービス アカウントで動かしたときに配置先が無い。

> 展開するのは `ins` 行だけではない。`StringVariableOperator` を通す箇所は
> 同じ書式（`%` で囲む）を受け付ける。

### 3.5 引数があるときは GUI を開かない

**引数なし＝GUI、引数あり＝CUI** である。
引数を付けたのにどのモードにも当たらない場合は、**エラーで終わる**（終了コード 1）。

```
認識できない引数です : /NOSUCHSWITCH  使い方は /HELP を参照してください。
```

2026/08/08 まで、この場合も GUI を開いていた（#528）。
**非対話で呼ぶと画面が出たまま応答を待ち続け、呼び出し側が停止する。**
綴りの誤りだけでなく、**古いビルドに新しいスイッチを渡したとき**にも起きる
（`/ZIPGEN` を実装する前のビルドで実際に踏んだ）。

### 3.3 設定と履歴は EXE の隣に置かれる

| ファイル | 内容 |
|---|---|
| `current.json` | 前回入力（URL・ユーザー・プロキシ等） |
| `histories.json` | 接続先の履歴。**アンインストール時の削除対象一覧を持つ** |

**2026/08/08 まで `current.bin` / `histories.bin`（BinaryFormatter）だった**（#528）。
`BinaryFormatter` が .NET 9 以降で削除されたため JSON に替えた。
**旧 `.bin` は読まない。** 履歴が一度空になるだけで、URL を入れ直せばよい。

`/FORCE` は履歴を消して強制的に取り直す。**試行錯誤のときはこれを使う。**
### 3.6 `/FORCE` は配置先を丸ごと消してから配る

```csharp
// 強制モードでは一度削除してから
if (Program.IsForce)
{
    if (Directory.Exists(entry.InstallDir))
    {
        Directory.Delete(entry.InstallDir, true);   // ← 丸ごと削除
    }
}
```

**「入れ直す」モードである。** 差分でも通常の上書きでもない。

削除が途中で失敗すると、**消しかけの状態で止まる。**
この削除はバックアップより前に行われる（バックアップは ZIP を展開する直前で取る）ため、
**この失敗はロールバックの対象にならない。**

実測では、配置先の `bbb\top2.dll` を掴んだ状態で `/FORCE` を実行し、
21 ファイルが 1 ファイルまで消えた。

```
別のプロセスで使用されているため、プロセスはファイル 'top2.dll' にアクセスできません。
   場所 System.IO.Directory.Delete(...)
   場所 DeployZipPackWithHTTP.Program.ExecUpdate(...) 行 1096
```

**復旧は難しくない。** 掴んでいるプロセスを終わらせて、**もう一度 `/FORCE`** でよい
（実測でも 21 ファイルに戻った）。配置物はサーバから作り直せるものなので、
消えて困る情報は無い。

> **前チェックは万能ではない。** あれは `exe` 行のアセンブリが動いているかを見るだけで、
> **EXE 以外のファイルが掴まれている場合は素通りする。**

**本当に戻らないのは、配布物でないファイルを配置先に置いていた場合。**
`/FORCE` は配置先を**中身ごと**消すため、アプリが書いた設定やログも消える。
これは失敗したときに限らず、**成功しても同じ**である。

| | `/FORCE` なし | `/FORCE` あり |
|---|---|---|
| 展開の失敗 | ロールバックされる（下記） | ― |
| 削除の失敗 | 起きない | 消しかけで止まる。**再度 `/FORCE` で復旧** |
| 配布物でないファイル | 残る | **成否によらず消える** |

> **常用しない。** 差分が効かなくなり（履歴を消すため）、毎回すべて取り直しになる。

### 3.4 引数の渡し方（`/MFTGEN` で必ず踏む）

コマンドライン解析（`StringVariableOperator.GetCommandArgs`）に癖がある。

**① `\` はエスケープ文字として食べられる**

```
NG : /MFTFILE "C:\temp\a.mft"     ← \ が消えて C:tempa.mft になる
OK : /MFTFILE "C:/temp/a.mft"     ← / なら素通り
OK : /MFTFILE "C:\\temp\\a.mft"   ← \\ で \ 1 個
```

**② ただし `/INSDIR` だけは `\` を残すこと**

`ins` 行の値は**そのまま配置先になる**。`/` のままだと配置に失敗し、
**それまでの配置物ごとロールバックされる。** `\\` でエスケープする。

```
OK : /INSDIR "c:\\FormAppRoot\\"   → ins c:\FormAppRoot\
NG : /INSDIR "c:/FormAppRoot/"     → 配置時に失敗してロールバック
```

**③ 空白を含む値は、自分で引用符を付ける**

PowerShell の `Start-Process -ArgumentList` は**空白を含む要素に引用符を付けない**。
付けないと空白で切れる。

```powershell
NG : "/EXENAME", "top.exe, top1.exe"      → exe top.exe,   で切れる
OK : "/EXENAME", '"top.exe, top1.exe"'    → 引用符を値に埋め込む
```

> **終了コードでは検出できないものがある。** ①③は `0` を返しつつ内容が壊れる。
> **生成物を必ず開いて確かめること。**

---

## 4. マニフェスト（`*.mft`）

```
ins c:\FormAppRoot\
exe top.exe, top1.exe, top2.exe, top3.exe
zip aaa.zip
md5 O3HVJ9PGdCYEn0bQHCnvYg==
zip bbb.zip
md5 jSKAbZeoGgjIPJek53/kAA==
```

| 行 | 意味 |
|---|---|
| `ins` | 配置先フォルダ |
| `exe` | **アプリを構成する EXE をすべて**（カンマ区切り）。下記 |
| `zip` | 取得する ZIP。**`.zip` 以外は無視される** |
| `md5` | 直前の `zip` の MD5（Base64）。**一致しないと配置しない** |

ZIP は**マニフェストと同じ場所**から取りに行く（URL のフォルダを基準にする）。

### `exe` は「起動する EXE」ではない

**起動されるのは先頭の 1 つだけ。** 2 番目以降は起動には使われない。
にもかかわらず全て挙げるのは、**配置してよいかの判定に使う**ため。

```csharp
foreach (Process pp in Process.GetProcesses())
{
    for (int i = 0; i < ary_exeFiles.Length; i++)      // ← 列挙した全件を突き合わせる
    {
        if (ary_exeFiles[i].ToUpper() == pp.MainModule.FileName.ToUpper())
        {
            p.StartInfo = new ProcessStartInfo(ary_exeFiles[i]);
            return false;                              // 1 つでも動いていれば false
        }
    }
}

p.StartInfo = new ProcessStartInfo(ary_exeFiles[0]);   // ← 起動は先頭だけ
return true;
```

呼び出しは 3 箇所あり、**2 箇所は起動しない。**

| 箇所 | 用途 | 起動するか |
|---|---|---|
| 配置の**前**チェック | **1 つでも動いていたら配置を中止**する（`E0002`） | しない |
| 配置の後（2 箇所） | 先頭を起動する。`/NB` で抑止できる | する |

**使用中のファイルを上書きしないための番人**である。
`top.exe` が止まっていても `top2.exe` が動いていれば、`bbb.zip` の展開で失敗する。
だから**アプリを構成する EXE をすべて挙げる。**

例外メッセージにも効く。**先頭ではなく「実際に動いていた EXE 名」**が出るため、
利用者は何を終了すればよいか分かる。

> 挙げ漏らすと、**配置の途中で失敗してロールバックされる**（それ自体は安全側）。
> ただし原因が「別の EXE が動いていた」ことだと分かりにくい。

### 失敗したときの守り（2 段）

**第 1 段 : 配置に入る前に止める**

`exe` 行のアセンブリが 1 つでも動いていれば、**ZIP を 1 つも取得せずに中止**する。

```
[FormAppRoot.mft]をダウンロードしました。
EXEファイル[...\aaa\top1.exe]が既に起動されています。
```

**先頭ではなく、実際に動いていたものが示される。**
`exe` 行を相対パスで正しく書いていないと、この検出が効かない。

**第 2 段 : 途中で失敗したら元に戻す**

展開に失敗すると、**その回に触ったものをすべて直前の状態へ戻す。**

```
[aaa.zip]は最新の状態です。
[bbb.zip]を更新します（更新）。
[bbb.zip]をダウンロードしました。
全ての配置対象ファイルを直前の状態に戻します。
[...\bbb\data2.txt]ファイルが開かれているため更新できません。
```

実測では、**先に成功していた `root.zip` の内容まで戻り**、ファイル数も変わらなかった。

> **`/FORCE` を付けるとこの守りが働かない場面がある。** 3.6 節を読むこと。

### ZIP の作成（`/ZIPGEN`）

GUI の「圧縮」タブと同じものを CUI で作れる（#528）。

#### 2 つの圧縮方式

**違いは「書庫内にルート フォルダを作るか」だけ**である。
GUI のチェック ボックスが、そのまま `/ROOTINZIP` の有無に対応する。

| | 個別のフォルダ圧縮 | ルート フォルダからの圧縮 |
|---|---|---|
| GUI | チェック**無し** | チェック**有り** |
| CUI | `/ROOTINZIP <名前>` | 省略 |
| 書庫内ルート | `<名前>/` を作る | 作らない |
| 書庫の中身 | `aaa/top1.exe` | `top1.exe` |
| 展開結果 | `<ins>\aaa\top1.exe` | `<ins>\top1.exe` |

**圧縮の対象（`/SRCDIR`）と組み合わせて考える。**

```powershell
# 個別のフォルダ圧縮 : FormAppRoot\aaa を、書庫内 aaa/ に入れる
/ZIPGEN /SRCDIR "C:/…/FormAppRoot/aaa" /ZIPFILE "C:/…/web/aaa" /ROOTINZIP aaa
        → aaa.zip の中身 : aaa/top1.exe, aaa/data1.csv, ...
        → 展開           : <ins>\aaa\top1.exe

# ルート フォルダからの圧縮 : FormAppRoot をまるごと、書庫内ルート無しで
/ZIPGEN /SRCDIR "C:/…/FormAppRoot" /ZIPFILE "C:/…/web/all"
        → all.zip の中身 : top.exe, aaa/top1.exe, bbb/top2.exe, ...
        → 展開           : <ins>\top.exe, <ins>\aaa\top1.exe
```

**どちらも展開結果は同じにできる。** 違うのは**配布物の分け方**であり、
それが次項の差分配布に効く。

#### `/TOPONLY`（CUI だけ）

**直下のファイルだけ**を対象にし、サブフォルダを含めない。

```powershell
/ZIPGEN /SRCDIR "C:/…/FormAppRoot" /ZIPFILE "C:/…/web/root" /TOPONLY
        → root.zip の中身 : top.exe, readme.txt, ...（aaa/ 等は入らない）
```

`CreateZipFromFolder` は再帰するため、ルートを対象にするとサブフォルダまで入る。
実装は選択デリゲート（`SelectionCriteriaDlgt3`）で、**`ZipperV2` は変えていない。**

> **これは近道であって、GUI にできないことではない。**
> **`/TOPONLY` は CUI だけにあり、これが無い GUI ではステージングが要る。**
> 直下のファイルを別フォルダへ集め、そこを圧縮すれば同じ ZIP になる。
>
> ```powershell
> # GUI と同じ経路（/TOPONLY を使わない）でも、結果は一致する
> Get-ChildItem $src -File | Copy-Item -Destination "$t\stage"
> /ZIPGEN /SRCDIR "$t/stage" /ZIPFILE "$t/out/byStage"
> ```
>
> 実測で、`/TOPONLY` で作ったものと**エントリが完全に一致**した。

> **除外拡張子では代替できないことがある。** 同じ拡張子がルートとサブフォルダの
> 両方にあると絞り込めない（同梱サンプルは `.txt` がルートと `ccc` の双方にある）。
> **ステージングの方が確実。**

暗号化・圧縮レベル・文字コードも指定できる（`/CYP` `/PASS` `/CMPLV` `/ENC`）。
除外拡張子は `/EXCLUDEEXT "txt,csv"`。

#### CUI 専用は `/TOPONLY` だけ

他の引数は、すべて GUI の入力に対応する。

| CUI | GUI |
|---|---|
| `/SRCDIR` / `/ZIPFILE` | フォルダ / ファイル名 |
| `/ROOTINZIP` の有無 | チェック ボックス |
| `/EXCLUDEEXT` | 除外拡張子 |
| `/CYP` `/PASS` `/CMPLV` `/ENC` | 各コンボ ボックス |
| **`/TOPONLY`** | **無し**（ステージングで代替） |

---

### 差分配布（ZIP を分ける理由）

**このツールは ZIP 単位で差分を取る。** 分け方が更新の粒度になる。

#### 差分は 2 段構え

**マニフェストが先に効く。** ZIP 単位の判定はその後である。

| 段 | 対象 | 変わっていなければ |
|---|---|---|
| **第 1 段** | `*.mft` の `Last-Modified` | **ZIP を 1 つも見に行かない**（HEAD すら送らない） |
| **第 2 段** | 各 `*.zip` の `Last-Modified` | その ZIP だけ飛ばす |

第 1 段が効くと、**HTTP 要求は HEAD 1 回で終わる。**

```
[.../FormAppRoot.mft]は最新の状態です。       ← これ 1 行で完了
```

**マニフェストを作り直すと第 1 段は破れる。** `/MFTGEN` は毎回書き出すため、
`Last-Modified` が変わる。ZIP を 1 つでも作り直したらマニフェストも作り直しになるので、
**実運用では「第 1 段は初回以外の無変更時に効く」**と考えるとよい。

#### 判定の実装

配布側は、マニフェストの `zip` 行ごとに **HEAD 要求で `Last-Modified` を見る。**

```csharp
// HEAD（Last-Modifiedチェック）
if (Program.LastModifiedCheck_ByHead(entry, history, zipFile))
{
    Program.GetAndSaveContent(entry, zipFile);   // 変わっていた → GET して展開
    isUpdated = true;
}
else
{
    isUpdated = false;                            // 変わっていない → 何もしない
}
```

比較相手は**履歴（`histories.json`）に残した前回の `Last-Modified`** である。

```csharp
httpLastModifiedHis = history.HttpZipLastModified[zipFile];
...
if (httpLastModifiedHis == httpLastModifiedWeb) { return false; }   // 一致 → 飛ばす
```

> **大小ではなく一致で判定する。** サーバ側を古いものに戻した場合も「変わった」と見なす。
> 履歴に無い ZIP（初回・追加された ZIP）は必ず取得する。

#### 実測（`FormAppRoot` を 4 分割、`/FORCE` なし）

| 回 | 状況 | 結果 |
|---|---|---|
| 1 回目 | 初回 | 4 つとも取得・展開 |
| 2 回目 | 変更なし | **第 1 段で完了**。ZIP は見に行かない |
| 3 回目 | `aaa` だけ変更 | `.mft` と **`aaa.zip` だけ**取得。他 3 つは「最新の状態です」 |

```
[aaa.zip]を更新します（更新）。
[aaa.zip]を解凍・インストールします（更新）。
[bbb.zip]は最新の状態です。
[ccc.zip]は最新の状態です。
[root.zip]は最新の状態です。
```

配置結果も、**変更は反映され、他は触られず、ファイル数は変わらない。**

#### だから「個別のフォルダ圧縮」を使う

| 方式 | ZIP の数 | `aaa` の 1 ファイルを直したとき |
|---|---|---|
| **個別のフォルダ圧縮** | フォルダごと | **`aaa.zip` だけ**を取り直す |
| ルート フォルダからの圧縮 | 1 つ | **全体**を取り直す |

同梱の `FormAppRootWeb` は**個別のフォルダ圧縮**で作っている
（`root.zip` ＋ `aaa.zip` / `bbb.zip` / `ccc.zip`）。
**ルート直下のファイルも 1 つの単位**として切り出す必要があるため、
そこだけ `/TOPONLY` を使う。

```powershell
/ZIPGEN /SRCDIR "$src"      /ZIPFILE "$w/root" /TOPONLY          # ルート直下
/ZIPGEN /SRCDIR "$src/aaa"  /ZIPFILE "$w/aaa"  /ROOTINZIP aaa    # 以下、フォルダごと
/ZIPGEN /SRCDIR "$src/bbb"  /ZIPFILE "$w/bbb"  /ROOTINZIP bbb
/ZIPGEN /SRCDIR "$src/ccc"  /ZIPFILE "$w/ccc"  /ROOTINZIP ccc
```

#### 「ルート フォルダからの圧縮」でも分けられる

**`/SRCDIR` はルートのまま、対象を絞って複数回**実行する。
書庫内のパスがルートからの相対になるため、**どの ZIP も正しい位置に展開される。**

```powershell
# 実行ファイルの類だけ
/ZIPGEN /SRCDIR "$src" /ZIPFILE "$w/bin"  /EXCLUDEEXT "txt,csv,bin"

# データだけ（上と補い合う組み合わせにする）
/ZIPGEN /SRCDIR "$src" /ZIPFILE "$w/data" /EXCLUDEEXT "exe,dll,json"
```

こちらは**フォルダの区切りに縛られない**ので、
「実行ファイルは滅多に変わらないが、データは頻繁に変わる」といった
**更新頻度で分ける**使い方に向く。

> **重複と抜けに気を付ける。** 絞り込みは除外指定なので、
> 組み合わせを誤ると**同じファイルが 2 つの ZIP に入る**（後勝ちで上書きされる）か、
> **どの ZIP にも入らない**（配置されない）。
> 分けたら**合計が元のファイル数と一致するか**を確かめること。

| 分け方 | 向く場面 |
|---|---|
| **個別のフォルダ圧縮** | フォルダが機能の単位になっている。分け方が自明 |
| **ルート フォルダからの圧縮**（＋絞り込みで複数回） | 更新頻度や種類で分けたい。フォルダをまたぐ |
| ルート フォルダからの圧縮（1 つだけ） | 分けない。配布物が小さい、更新が常に全体に及ぶ |

#### 差分を効かせるときの注意

- **`/FORCE` を付けると差分が効かない。** 履歴を消すため、**両段とも**素通りして
  毎回すべて取り直す。試行錯誤には便利だが、**差分の確認にはならない。**
- **ZIP を作り直すと `Last-Modified` が変わる。** 中身が同じでも取り直しになる。
  変わっていないフォルダは、**ZIP も作り直さない**こと。
- **MD5 が一致しなければ配置しない。** 差分で取得した ZIP も検証される。
- **履歴は EXE の隣（`histories.json`）にある。** 消すと差分が効かなくなる。
  配置先を消しても履歴は残るため、**「配置先は空なのに最新の状態です」**になり得る。
  その場合は `/FORCE` で取り直す。

### MFT の作成（`/MFTGEN`）

GUI の「声明文作成」タブと同じものを CUI で作れる（#528）。

```powershell
$zips = ((Get-ChildItem $web -Filter *.zip | ForEach-Object { $_.FullName.Replace("\","/") }) -join ",")

Start-Process $exe -ArgumentList `
    "/MFTGEN", `
    "/ZIPFILES", $zips, `
    "/INSDIR",  "c:\\FormAppRoot\\", `
    "/EXENAME", '"top.exe, top1.exe, top2.exe, top3.exe"', `
    "/MFTFILE", "$env:TEMP/FormAppRoot.mft" `
    -NoNewWindow -Wait -WorkingDirectory (Split-Path $exe)
```

**引数の渡し方に 3 つの癖がある**（3.4 節）。上の例はそれを踏まえた形。

> **MD5 は「実ファイルの MD5」である。**
> 2026/08/08 まで `Program.LoadFile` が**最終ブロックを前ブロックの残骸で埋めていた**ため、
> それ以前に作られたマニフェストの値は実ファイルの MD5 と異なる（#528 で修正）。
> **古いマニフェストは検証に失敗する**ので、作り直すこと。

---

## 5. 疎通確認の手順（IIS Express で完結する）

IIS を立てなくてよい。**IIS Express で静的配信すれば足りる**（管理者権限も不要）。

```powershell
# 1) 配信フォルダを用意し、FormAppRoot から ZIP を作る
#    **FormAppRootWeb は追跡していない。** 毎回ここで作る（後述）。
$w = "$env:TEMP\dzweb"
New-Item -ItemType Directory -Force $w | Out-Null

$src = "...\Sample\FormAppRoot".Replace("\", "/")
& $exe /ZIPGEN /SRCDIR $src        /ZIPFILE "$w/root".Replace("\","/") /TOPONLY
& $exe /ZIPGEN /SRCDIR "$src/aaa"  /ZIPFILE "$w/aaa".Replace("\","/")  /ROOTINZIP aaa
& $exe /ZIPGEN /SRCDIR "$src/bbb"  /ZIPFILE "$w/bbb".Replace("\","/")  /ROOTINZIP bbb
& $exe /ZIPGEN /SRCDIR "$src/ccc"  /ZIPFILE "$w/ccc".Replace("\","/")  /ROOTINZIP ccc

# 2) .mft の MIME を登録する（未登録だと 404.3 になる）
@'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <staticContent>
      <remove fileExtension=".mft" />
      <mimeMap fileExtension=".mft" mimeType="text/plain" />
    </staticContent>
  </system.webServer>
</configuration>
'@ | Set-Content "$w\web.config" -Encoding UTF8

# 3) 配信する
Start-Process "${env:ProgramFiles}\IIS Express\iisexpress.exe" `
    -ArgumentList "/path:$w","/port:51099","/systray:false" -WindowStyle Hidden
```

あとは 2 節の形で `/CUI /NB /WWWURL http://localhost:51099/FormAppRoot.mft` を実行する。

### 同梱のサンプル

```
Sample/
  SampleConsoleApp/     … 配布する EXE のソース（net10.0 のコンソール アプリ）
  FormAppRoot/          … 配布物（21 ファイル）。**追跡している**
  FormAppRootWeb/       … 個別のフォルダ圧縮で作った ZIP 一式。**追跡しない（生成物）**
  FormAppRootWeb2/      … ルート フォルダからの圧縮で作った ZIP 一式。**同上**
```

**`FormAppRootWeb` と `FormAppRootWeb2` は、同じ `FormAppRoot` を別の方式で分けたもの。**
どちらを配布しても、展開結果は同じ 21 ファイルになる。

| | 分け方 | ZIP |
|---|---|---|
| `FormAppRootWeb` | **フォルダごと**（`/ROOTINZIP`）＋ルート直下（`/TOPONLY`） | `root` `aaa` `bbb` `ccc` |
| `FormAppRootWeb2` | **ルートから＋種類で分割**（`/EXCLUDEEXT`） | `app`（実行ファイル一式）`data`（データ） |

```powershell
# FormAppRootWeb2 の作り方（除外指定は補い合う組み合わせにする）
/ZIPGEN /SRCDIR "$src" /ZIPFILE "$w2/app"  /EXCLUDEEXT "txt,csv,bin"   → 12 件
/ZIPGEN /SRCDIR "$src" /ZIPFILE "$w2/data" /EXCLUDEEXT "exe,dll,json"  →  9 件
                                                          合計 21 件（重複 0）
```

`FormAppRoot` の EXE は `SampleConsoleApp` を**アセンブリ名だけ変えてビルド**したもの。

```powershell
dotnet build ...\Sample\SampleConsoleApp -c Release -p:AssemblyName=top  -o <一時>
dotnet build ...\Sample\SampleConsoleApp -c Release -p:AssemblyName=top1 -o <一時>
dotnet build ...\Sample\SampleConsoleApp -c Release -p:AssemblyName=top2 -o <一時>
```

`.pdb` を除いた 4 ファイル（`.exe` `.dll` `.deps.json` `.runtimeconfig.json`）を
`FormAppRoot` / `aaa` / `bbb` へ置く。**中身は同じで、名前だけ違う。**
配置後に起動すると、自分の居場所と同じ場所のファイル一覧を出す。

> **`FormAppRootWeb` を追跡しないのは、作り直し漏れを防ぐため。**
> `FormAppRoot` を直したのに ZIP を作り直さないと、MD5 が合わず配布が失敗する。
> 追跡していると、その状態のままコミットできてしまう。

### 確認できること

**配置結果を、配布前のフォルダと突き合わせられる。**

```powershell
# 21 ファイルが一致するはず
$src = "...\Sample\FormAppRoot"; $dst = "C:\FormAppRoot"
$md5 = [System.Security.Cryptography.MD5]::Create()
foreach ($f in Get-ChildItem $src -Recurse -File) {
    $rel = $f.FullName.Substring($src.Length + 1)
    $a = [Convert]::ToBase64String($md5.ComputeHash([IO.File]::ReadAllBytes($f.FullName)))
    $b = [Convert]::ToBase64String($md5.ComputeHash([IO.File]::ReadAllBytes((Join-Path $dst $rel))))
    if ($a -ne $b) { "相違 : $rel" }
}
```

### 後始末

**`C:\FormAppRoot` と IIS Express のプロセスが残る。** 消してよいか判断してから消すこと。
`/UnIns "<マニフェストの URL>"` で、履歴に記録された配置物を削除できる。

---

## 6. ビルド

```
CS\4_Build_Framework_Tool.bat        … net48
CS\4_Build_Framework_ToolCore.bat    … .NET 10
```

いずれも `1_BuildAll.ps1` の `Framework_Tool` / `Framework_ToolCore` から呼ばれる。
**ステップとしては独立していない**ので、サマリでは他のツールとまとめて 1 行になる。

---

## 7. ZIP 部品との関係

**本ツールは `ZipperV2` / `UnZipperV2`（`Public.IO`）の唯一の利用者。**

```
Public/IO/ZipperV2.cs / UnZipperV2.cs   ← SharpZipLib（#524）
```

**ZIP 部品を変えたら、本ツールのビルドと疎通を必ず確かめること。**
単体テスト（`TestCode/TestZipV2.cs`）は部品の振る舞いを見るが、
**配布フロー全体を通すのは本ツールだけ**である。

DotNetZip（非推奨・既知脆弱性 `GHSA-xhg6-9j5j-w4vf`）から移行した際、
次を落としている（#528）。

| 落としたもの | 理由 |
|---|---|
| 自己解凍書庫（`SaveSelfExtractor`） | SharpZipLib に相当が無い。**配布フローは `.zip` しか扱わず、消費経路が無かった** |
| 選択条件の文字列（`"name != *.txt"`） | DotNetZip 独自の DSL。選択デリゲートに一本化した |

---

## 8. 制約

- **GUI の確認は手作業**（`RELEASE.md` 4 節）。CUI は `/MFTGEN` と `/CUI` で
  生成から配置まで通せるため、疎通確認に載せられる
- 文言は `Resources\Resource.resx` / `Resource.ja-JP.resx` に置く。**直書きしない**
  （例外メッセージは `MSGDefinition.xml` / `MSGDefinition_ja-JP.xml` 側）
