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

マニフェストの `exe` 行で指定されたアセンブリを、**配置後に `Process.Start` する。**

```csharp
if (Program.IsBoot)          // /NB が指定されていない
{
    ...
    p.Start();               // 配置した EXE を実行する
}
```

**中身を確かめていない配布物に対しては、必ず `/NB` を付ける。**
起動されると GUI が開き、非対話の実行が止まる。

### 3.2 配置先はマニフェストが決める

`ins` 行の値がそのまま配置先になる。同梱サンプルは `c:\FormAppRoot\` を指す。
**引数で上書きできない。** 別の場所へ入れたいならマニフェストを作り直す。

### 3.3 設定と履歴は EXE の隣に置かれる

| ファイル | 内容 |
|---|---|
| `current.json` | 前回入力（URL・ユーザー・プロキシ等） |
| `histories.json` | 接続先の履歴。**アンインストール時の削除対象一覧を持つ** |

**2026/08/08 まで `current.bin` / `histories.bin`（BinaryFormatter）だった**（#528）。
`BinaryFormatter` が .NET 9 以降で削除されたため JSON に替えた。
**旧 `.bin` は読まない。** 履歴が一度空になるだけで、URL を入れ直せばよい。

`/FORCE` は履歴を消して強制的に取り直す。**試行錯誤のときはこれを使う。**

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
| `exe` | 配置後に起動するアセンブリ（カンマ区切り）。`/NB` で抑止できる |
| `zip` | 取得する ZIP。**`.zip` 以外は無視される** |
| `md5` | 直前の `zip` の MD5（Base64）。**一致しないと配置しない** |

ZIP は**マニフェストと同じ場所**から取りに行く（URL のフォルダを基準にする）。

### 作成（`/MFTGEN`）

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
# 1) 配信フォルダを用意する
$w = "$env:TEMP\dzweb"
New-Item -ItemType Directory -Force $w | Out-Null
Copy-Item "...\Sample\FormAppRootWeb\*" $w

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

### 確認できること

同梱の `Sample/` は、`FormAppRoot`（配布前）と `FormAppRootWeb`（ZIP 化後）の対で、
**配置結果を突き合わせられる。**

```powershell
# 25 ファイルが一致するはず
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
