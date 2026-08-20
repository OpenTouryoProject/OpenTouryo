<#
.SYNOPSIS
    サンプル アプリを起動して疎通を確認し、合否を一覧する。

.DESCRIPTION
    リリース時の「サンプルを幾つか見繕って手動で疎通を行う」を機械化したもの（#513 段階 3）。

    ＜段階 1・2 との違い＞
      段階 1（2_RunAllTests.ps1）は「出力が前回と同じか」を見る回帰テスト。
      本スクリプトは「起動して、想定どおり動き、例外なく終わるか」を見る疎通テスト。
      期待結果ファイルは持たず、判定条件を定義側に書く。

    ＜対象＞
      非 UI 系（バッチ・CLI）と Web 系（Web アプリ・Web サービス）。
      WinForms / WPF 系は UI Automation が必要で維持費が高いため対象外とし、
      リリース チェックリストの手作業項目として残す。

    ＜ビルド＞
      リポジトリ既定のビルド バッチを呼ぶ。理由は 2_RunAllTests.ps1 と同じで、
      csproj を直接ビルドすると nuget.exe restore が行うネイティブ DLL の配置が漏れる。

      なお 0_ExecAllBat.bat は途中で 1_DeleteDir.bat を繰り返し実行し、
      配下の bin / obj を再帰的に削除する。このため 1_BuildAll.ps1 の完走後に
      残るのは最後にビルドされた Core サンプルだけで、net48 サンプルは残らない。
      本スクリプトが自分でビルドするのはこのため。

    ＜DB＞
      SQL Server の Northwind を使用する。
      RerunnableBatch 系は Orders(830 件) を読み Orders2 へ INSERT するため、
      実行前に Orders2 を空にする必要がある。実行後は 830 件＝初期状態に戻る。

    ＜C# 版と VB 版＞
      VB 版は C# 版からの移植で、疎通の手順がそのまま通る（#542 で突き合わせ済み）。
        ・接続文字列名          ConnectionString_SQL
        ・MVC のログイン画面    Views/Home/Login.cshtml が同一
        ・認証が要る画面        /Crud1/Index（Crud1Controller に Authorize）
        ・WebForms のコントロール ctl00$ContentPlaceHolder_A$txtUserID ほか
      このため判定（Flow / Verify）は両者で共有し、対象定義だけを分ける。
      分けて書くと、サンプルの画面を直したとき片方だけ直す事故が起きる。

.PARAMETER Lang
    対象の言語。CS（既定）/ VB / Both。

    ※ 既定に VB を含めない。リリース時の検証（RELEASE.md 3 節）は C# 版が対象で、
    　 VB を含めると毎回 VB のビルドが乗るため。

.PARAMETER Only
    対象名の部分一致で絞る（例: -Only "Rerunnable"）。

.PARAMETER SkipBuild
    ビルドを省略し、既存のバイナリで疎通のみ行う。

.PARAMETER OutputDir
    ログの保存先。既定は %TEMP%\OpenTouryoSmokeTest。

.EXAMPLE
    .\3_SmokeTest.ps1

.EXAMPLE
    .\3_SmokeTest.ps1 -Only "net48" -SkipBuild

.EXAMPLE
    .\3_SmokeTest.ps1 -Lang VB

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
     2026/08/07  玄人 幸道         Orders2が無ければ作成するようにした
     2026/08/13  玄人 幸道         Lang を追加（VB 版の疎通に対応）
     2026/08/15  玄人 幸道         Args を持たない対象が PowerShell 5.1 で起動できず、
                                   しかも前回の出力で OK と判定されていたのを修正
#>
[CmdletBinding()]
param(
    [ValidateSet("CS", "VB", "Both")]
    [string]$Lang = "CS",
    [string]$Only,
    [switch]$SkipBuild,
    [string]$OutputDir = (Join-Path $env:TEMP "OpenTouryoSmokeTest")
)

$ErrorActionPreference = "Continue"

# ------------------------------------------------------------------
# カレント ディレクトリからの実行を許可する
# ------------------------------------------------------------------
# PowerShell は子プロセスに NoDefaultCurrentDirectoryInExePath=1 を渡すが、
# ビルド バッチは出力フォルダへ cd したうえでパス区切りを含まない名前で
# 実行するため、これが効いていると exe を起動できない。
# （TESTING.md の「PowerShell から .bat を呼ぶときの注意」と同じ）
Remove-Item Env:\NoDefaultCurrentDirectoryInExePath -EA SilentlyContinue

# 本スクリプトは root\programs に置き、その配下の CS / VB を対象とする。
# ビルド バッチもサンプルも各言語のフォルダにあるため、そこを起点にする。
#
# ツール（DaoGen_Tool / DeployZipPackWithHTTP）は C# にしか無いので、
# それらの準備・検証は $csRoot を直接参照してよい。
$csRoot = Join-Path $PSScriptRoot "CS"

# 構成ファイル（接続文字列）と Orders2 の DDL を、どちらから読むか。
# 中身は CS / VB で同じだが、「実際に使われる構成ファイルから読む」という
# 建て付けを崩さないため、対象に含まれる方を使う。
$configRoot = if ($Lang -eq "VB") { Join-Path $PSScriptRoot "VB" } else { $csRoot }

New-Item -ItemType Directory -Force $OutputDir | Out-Null

# サマリの整形。Format-Table は 5.1 で全角の桁を数えないため、自前で揃える。
. (Join-Path $PSScriptRoot "SummaryTable.ps1")

# 対象に与える stdin（空）。
# Start-Process -RedirectStandardInput は実在するファイルを要求するため、先に作る。
# これが無いと Console.ReadKey() が本当にキー入力を待つ。
$emptyIn = Join-Path $OutputDir "empty.in"
if (-not (Test-Path $emptyIn))
{
    New-Item -ItemType File $emptyIn | Out-Null
}

# ------------------------------------------------------------------
# コンソールのコード ページを先に UTF-8 にしておく
# ------------------------------------------------------------------
# ビルド バッチが呼ぶ z_Common.bat は先頭で chcp 65001 を実行する。
# コード ページはコンソール全体の設定なので、子プロセスで変えても
# 呼び出し元の画面に影響し、日本語環境の既定 932 からの切り替わりで
# 画面が再描画されて**それまでの表示が消える**。
# 何も表示していない今のうちに切り替えておけば、実行中は変化しない。
# 戻さないのは、戻す操作でも再描画が起きてサマリが消えるため。
if ((cmd /c chcp) -notmatch '65001')
{
    cmd /c chcp 65001 | Out-Null
}

# コンソールのコード ページとは別に、PowerShell 側の出力エンコードも合わせる。
# Windows PowerShell 5.1 は起動時の値を保持しており、実行中にコード ページが
# 変わっても追随しない。ここを合わせないと、バッチ（UTF-8 で出力）の内容を
# 旧コード ページで解釈してしまい、ログとエラー一覧が文字化けする。
# ※ 2 回目以降の実行ではコード ページが既に 65001 で上の分岐を通らないため、
#    この処理は分岐の外に置く必要がある。
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# ------------------------------------------------------------------
# 接続文字列
# ------------------------------------------------------------------
# サンプルが実際に使う App.config から読む。
# ここで別途ハードコードすると、サンプル側の変更に追随できなくなる。
function Get-SampleConnectionString
{
    $config = Join-Path $configRoot "Samples\Bat_sample\SimpleBatch_sample\App.config"
    if (-not (Test-Path $config)) { return $null }

    $xml = [xml](Get-Content $config -Raw)
    $node = $xml.configuration.connectionStrings.add |
            Where-Object { $_.name -eq "ConnectionString_SQL" }
    return $node.connectionString
}

$connString = Get-SampleConnectionString

function Invoke-Sql([string]$sql)
{
    $c = New-Object System.Data.SqlClient.SqlConnection $connString
    $c.Open()
    try
    {
        $cmd = $c.CreateCommand()
        $cmd.CommandText = $sql
        return $cmd.ExecuteScalar()
    }
    finally { $c.Close() }
}

# ------------------------------------------------------------------
# ログの初期化（#571）
# ------------------------------------------------------------------
# **使う前に必ず消す。**
#   補助ログ（Web ホスト・ZIP 生成など）は対象ごとの固定名で $OutputDir に残る。
#   消さずに使うと、**前回の内容が残ったまま読まれる**ことがある
#   （「二重に出力される」という症状は、これで説明が付く）。
#
#   対象の実行（$out / $err）側は既に同じことをしている。作法を揃える。
function Reset-Log([string]$path)
{
    Remove-Item $path, "$path.err" -Force -ErrorAction SilentlyContinue
    return $path
}

# ------------------------------------------------------------------
# 対象の定義
# ------------------------------------------------------------------
# Name    : 表示名（ログのファイル名にもなるため、言語をまたいで重複させない）
# Dir     : バッチとサンプルのあるフォルダ（root\programs からの相対）。
#           省略時は対象言語のフォルダ。
# Bat     : ビルドに使うバッチ（Dir 配下）
# Exe     : 実行ファイル（net48）。Dll を指定した場合は dotnet で実行する。
# Args    : コマンドライン引数
# Pre     : 実行前に行う準備（スクリプト ブロック）
# Expect  : 標準出力がこの正規表現に一致すれば成功
# Verify  : 追加の検証（スクリプト ブロック）。$true を返せば成功
#
# ※ サンプルは末尾に Console.ReadKey() を持つため、出力をリダイレクトすると
#    必ず例外で終わる。これはテスト内容とは無関係なので、判定から除外する。
$batchArgs = @("/DAP", "SQL", "/MODE1", "individual", "/MODE2", "static", "/EXROLLBACK", "-")

# ------------------------------------------------------------------
# Orders2（Northwind 標準には無い表）
# ------------------------------------------------------------------
# instnwnd.sql に含まれないため、**DB を作り直すたびに消える**。
# 無いまま実行すると「オブジェクト名 'Orders2' が無効です」で事前準備が落ち、
# 原因が読み取れない。ここで作ってしまう。
#
# DDL は同梱の CREATE ORDERS2.sql をそのまま流す。**ここに書き写さない。**
# （同じ DDL がサンプル配下に 9 つ重複しており、さらに増やす意味がない）
function Initialize-Orders2
{
    $exists = Invoke-Sql "SELECT OBJECT_ID('dbo.Orders2', 'U')"
    if ($null -ne $exists -and $exists -isnot [DBNull]) { return }

    $ddl = Join-Path $configRoot "Samples\Bat_sample\RerunnableBatch_sample\CREATE ORDERS2.sql"
    if (-not (Test-Path $ddl))
    {
        throw "Orders2 が無く、DDL も見つかりません : $ddl"
    }

    Write-Host "  Orders2 がありません。作成します（$ddl）。"

    # sqlcmd ではなく SqlClient で流すため、GO（バッチ区切り）は自前で分ける。
    # SqlClient は GO を解釈できず、構文エラーになる。
    foreach ($batch in ((Get-Content $ddl -Raw) -split '(?im)^\s*GO\s*$'))
    {
        # USE は流さない。接続先は接続文字列に従う（別 DB を指していても壊さない）。
        if ($batch -match '\S' -and $batch -notmatch '(?im)^\s*USE\s')
        {
            Invoke-Sql $batch | Out-Null
        }
    }
}

# RerunnableBatch 系は Orders → Orders2 の INSERT。実行前に Orders2 を空にする。
$clearOrders2 = {
    Initialize-Orders2
    Invoke-Sql "DELETE FROM [Orders2]" | Out-Null
}
# 実行後は Orders と同数（830 件）になっていること。
$verifyOrders2 = {
    $src = [int](Invoke-Sql "SELECT COUNT(*) FROM [Orders]")
    $dst = [int](Invoke-Sql "SELECT COUNT(*) FROM [Orders2]")
    Write-Host ("     Orders {0} 件 → Orders2 {1} 件" -f $src, $dst)
    return ($dst -eq $src -and $src -gt 0)
}

# ------------------------------------------------------------------
# DaoGen_Tool（墨壺）の CUI モード
# ------------------------------------------------------------------
# #508 で追加された CUI。2 モードを DAODEFGEN → DAOSQLGEN の順に実行し、
# 前段が出力した定義 CSV を後段の入力に使う。
#
# ＜パス区切りの注意＞
#   コマンドライン解析（StringVariableOperator.GetCommandArgs）は
#   「\」をエスケープ文字として扱うため、パスの区切りは「/」にする。
#   「C:\temp\out」と書くと「\」が消えて別のパスになる（ツールの /HELP にも記載）。
#
# ＜テンプレート＞
#   root/files/tools/DGenTemplates（DaoTemplate*.cs / *Template.xml などの平置き）。

$daoGenTemplate = ((Join-Path $csRoot "..\..\files\tools\DGenTemplates" | Resolve-Path).Path) -replace '\\', '/'

# 対象を 2 テーブルに絞る。全テーブルを回すと時間がかかるだけで、
# 疎通の確認としては同じことを見ている。
$daoGenTables = "Shippers,Orders"

# 作業フォルダを作る（net48 / Core で分ける）
function New-DaoGenWork([string]$tag)
{
    $work = Join-Path $OutputDir "daogen_$tag"
    New-Item -ItemType Directory -Force (Join-Path $work "gen") | Out-Null
    # 前回の生成物を残さない（残っていると「生成された」の判定が甘くなる）
    Get-ChildItem $work -Recurse -File -EA SilentlyContinue | Remove-Item -Force -EA SilentlyContinue
    return $work
}

function New-DaoGenArgs([string]$tag, [string]$mode)
{
    $work = Join-Path $OutputDir "daogen_$tag"
    $csv  = ($work + "/DaoDef.csv").Replace("\", "/")
    $gen  = ($work + "/gen").Replace("\", "/")

    if ($mode -eq "DAODEFGEN")
    {
        return @("/CUI", "/MODE", "DAODEFGEN", "/OUTPUT", $csv, "/DAP", "SQL", "/TABLES", $daoGenTables)
    }
    return @("/CUI", "/MODE", "DAOSQLGEN", "/DAODEF", $csv,
             "/TEMPLATE", $daoGenTemplate, "/OUTPUT", $gen,
             "/DAP", "SQL", "/LANG", "CS", "/ENTITY")
}

# 定義 CSV に対象テーブルが並んでいること
function Test-DaoDef([string]$tag)
{
    $csv = Join-Path (Join-Path $OutputDir "daogen_$tag") "DaoDef.csv"
    if (-not (Test-Path $csv)) { Write-Host "     定義 CSV が生成されていない"; return $false }

    $text = Get-Content $csv -Raw
    $ok = ($text -match 'Shippers') -and ($text -match 'Orders')
    Write-Host ("     定義 CSV : {0} 行" -f (Get-Content $csv).Count)
    return $ok
}

# Dao・DTO・SQL が生成されていること
function Test-DaoGen([string]$tag)
{
    $gen = Join-Path (Join-Path $OutputDir "daogen_$tag") "gen"
    $files = @(Get-ChildItem $gen -Recurse -File -EA SilentlyContinue)
    Write-Host ("     生成ファイル : {0} 件" -f $files.Count)

    # Dao クラス（.cs）と 動的 SQL（.xml）と 静的 SQL（.sql）が揃っていること
    $hasCs  = @($files | Where-Object { $_.Name -eq "DaoShippers.cs" }).Count -gt 0
    $hasXml = @($files | Where-Object { $_.Extension -eq ".xml" }).Count -gt 0
    $hasSql = @($files | Where-Object { $_.Extension -eq ".sql" }).Count -gt 0
    return ($hasCs -and $hasXml -and $hasSql)
}

$prepareDaoGen48   = { [void](New-DaoGenWork "net48") }
$prepareDaoGenCore = { [void](New-DaoGenWork "core") }
$daoDefArgs48      = New-DaoGenArgs "net48" "DAODEFGEN"
$daoSqlArgs48      = New-DaoGenArgs "net48" "DAOSQLGEN"
$daoDefArgsCore    = New-DaoGenArgs "core"  "DAODEFGEN"
$daoSqlArgsCore    = New-DaoGenArgs "core"  "DAOSQLGEN"
$verifyDaoDef48    = { Test-DaoDef "net48" }
$verifyDaoGen48    = { Test-DaoGen "net48" }
$verifyDaoDefCore  = { Test-DaoDef "core" }
$verifyDaoGenCore  = { Test-DaoGen "core" }

# ------------------------------------------------------------------
# DeployZipPackWithHTTP の CUI モード
# ------------------------------------------------------------------
# #528 で /MFTGEN（マニュフェスト生成）を追加し、生成から配置まで CUI で通せる。
#
# ＜配置先を C:\ 直下にしない＞
#   同梱サンプルのマニフェストは c:\FormAppRoot\ を指すが、
#   疎通確認で環境を汚さないよう、$OutputDir 配下を指すマニフェストを作り直す。
#
# ＜引数の癖＞（Tools\DeployZipPackWithHTTP\README.md 3.4 節）
#   ・「\」はエスケープ文字として食べられる → パスは「/」で渡す
#   ・ただし /INSDIR だけは「\」を残す（ins 行がそのまま配置先になるため）
#   ・空白を含む値は、自分で引用符を付ける
$deployWebPort = 51084
$deploySampleSrc = Join-Path $csRoot "Frameworks\Tools\DeployZipPackWithHTTP\Sample\FormAppRoot"

# **配布物（ZIP）は追跡していない。** FormAppRoot から毎回作る。
#   /ZIPGEN … ルート直下（/TOPONLY）と各フォルダ（/ROOTINZIP）を別々の ZIP にする
#   /MFTGEN … その ZIP からマニュフェストを作る
# 作り置きを追跡すると、元を直したときの作り直し漏れで MD5 が合わなくなる。
$deployZipNames = @("root", "aaa", "bbb", "ccc")

function Get-DeployWork([string]$tag)
{
    return (Join-Path $OutputDir "deploy_$tag")
}

# 配信フォルダを用意し、FormAppRoot から ZIP を作る
function New-DeployWeb([string]$tag, [string]$exe)
{
    $work = Get-DeployWork $tag
    $web  = Join-Path $work "web"
    $ins  = Join-Path $work "ins"

    Get-ChildItem $work -Recurse -File -EA SilentlyContinue | Remove-Item -Force -EA SilentlyContinue
    New-Item -ItemType Directory -Force $web | Out-Null
    New-Item -ItemType Directory -Force $ins | Out-Null

    # .mft は既定で MIME 未登録のため 404.3 になる
    $conf = '<?xml version="1.0" encoding="utf-8"?>' + "`r`n" +
            '<configuration><system.webServer><staticContent>' +
            '<remove fileExtension=".mft" />' +
            '<mimeMap fileExtension=".mft" mimeType="text/plain" />' +
            '</staticContent></system.webServer></configuration>'
    Set-Content (Join-Path $web "web.config") $conf -Encoding UTF8

    #region ZIP を作る（/ZIPGEN）

    # **パスの区切りは「/」で渡す。** コマンドライン解析が「\」を食べる。
    $src = $deploySampleSrc.Replace("\", "/")

    foreach ($name in $deployZipNames)
    {
        $out = (Join-Path $web $name).Replace("\", "/")

        if ($name -eq "root")
        {
            # ルート直下だけ（サブフォルダは各 ZIP が持つ）。書庫内ルートは作らない。
            $a = @("/ZIPGEN", "/SRCDIR", $src, "/ZIPFILE", $out, "/TOPONLY")
        }
        else
        {
            # フォルダごと。書庫内ルートをフォルダ名にする（個別のフォルダ圧縮）。
            $a = @("/ZIPGEN", "/SRCDIR", "$src/$name", "/ZIPFILE", $out, "/ROOTINZIP", $name)
        }

        $log = Reset-Log (Join-Path $OutputDir "deploy_zipgen_$tag`_$name.log")
        Start-Process $exe -ArgumentList $a -NoNewWindow -Wait `
            -WorkingDirectory (Split-Path $exe) -RedirectStandardOutput $log

        if (-not (Test-Path (Join-Path $web ($name + ".zip"))))
        {
            Write-Host ("     ZIP を生成できない : {0}.zip" -f $name)
            return $work
        }
    }

    #endregion

    return $work
}

function New-MftGenArgs([string]$tag)
{
    $work = Get-DeployWork $tag
    $web  = Join-Path $work "web"

    # **ファイル一覧を採ってはいけない。**
    # 引数は対象定義の時点（Pre より前）で組み立てられるため、ZIP はまだ無い。
    # 名前は決まっているので、そこから組み立てる。
    $zips = @($deployZipNames | Sort-Object |
              ForEach-Object { (Join-Path $web ($_ + ".zip")).Replace("\", "/") }) -join ","

    # ins 行はそのまま配置先になるので「\」を残す（「\\」でエスケープ）
    $ins = (Join-Path $work "ins") + "\"
    $insArg = $ins.Replace("\", "\\")

    $mft = (Join-Path $web "FormAppRoot.mft").Replace("\", "/")

    # **exe 行はインストール先からの相対パス。** サブフォルダのものは、そう書く。
    # 二重起動チェックがこれを使うため、実在しないパスを書くと検出が効かない。
    return @("/MFTGEN", "/ZIPFILES", $zips, "/INSDIR", $insArg,
             "/EXENAME", '"top.exe, aaa\\top1.exe, bbb\\top2.exe"', "/MFTFILE", $mft)
}

# /NB … マニフェストの exe 行で指定されたアセンブリを起動しない
# /FORCE … 履歴を消して毎回やり直す（前回の結果に依存しない）
$deployArgs = @("/CUI", "/NB", "/FORCE", "/WWWURL",
                ("http://localhost:{0}/FormAppRoot.mft" -f $deployWebPort))

# マニュフェストが生成され、MD5 が実ファイルのものと一致すること
function Test-MftGen([string]$tag)
{
    $web = Join-Path (Get-DeployWork $tag) "web"
    $mft = Join-Path $web "FormAppRoot.mft"
    if (-not (Test-Path $mft)) { Write-Host "     マニュフェストが生成されていない"; return $false }

    $lines = @(Get-Content $mft -Encoding UTF8)
    Write-Host ("     マニュフェスト : {0} 行" -f $lines.Count)

    $zips = @($lines | Where-Object { $_ -like "zip *" })
    $md5s = @($lines | Where-Object { $_ -like "md5 *" })

    if (($zips.Count -ne $deployZipNames.Count) -or ($md5s.Count -ne $deployZipNames.Count))
    {
        Write-Host "     zip / md5 の組数が合わない"
        return $false
    }

    # **書かれた MD5 を、実ファイルから計算し直して突き合わせる。**
    # ここが合わないと配布時に弾かれる。作り置きの ZIP を使っていた頃は、
    # 元を直したのに ZIP を作り直さず、ここで気付けなかった。
    $md5 = [System.Security.Cryptography.MD5]::Create()

    for ($i = 0; $i -lt $zips.Count; $i++)
    {
        $name = $zips[$i].Substring(4).Trim()
        $path = Join-Path $web $name

        if (-not (Test-Path $path)) { Write-Host ("     ZIP が無い : {0}" -f $name); return $false }

        $want = $md5s[$i].Substring(4).Trim()
        $got  = [Convert]::ToBase64String($md5.ComputeHash([IO.File]::ReadAllBytes($path)))

        if ($want -ne $got) { Write-Host ("     MD5 が一致しない : {0}" -f $name); return $false }
    }

    return $true
}

# 配置結果が、圧縮前のフォルダと一致すること
function Test-Deploy([string]$tag)
{
    $ins = Join-Path (Get-DeployWork $tag) "ins"
    $md5 = [System.Security.Cryptography.MD5]::Create()

    $src = @(Get-ChildItem $deploySampleSrc -Recurse -File)
    $dst = @(Get-ChildItem $ins -Recurse -File -EA SilentlyContinue)
    Write-Host ("     配置 : {0} / {1} ファイル" -f $dst.Count, $src.Count)

    if ($dst.Count -ne $src.Count) { return $false }

    foreach ($f in $src)
    {
        $rel = $f.FullName.Substring($deploySampleSrc.Length + 1)
        $t = Join-Path $ins $rel
        if (-not (Test-Path $t)) { Write-Host ("     欠落 : {0}" -f $rel); return $false }

        $a = [Convert]::ToBase64String($md5.ComputeHash([IO.File]::ReadAllBytes($f.FullName)))
        $b = [Convert]::ToBase64String($md5.ComputeHash([IO.File]::ReadAllBytes($t)))
        if ($a -ne $b) { Write-Host ("     内容相違 : {0}" -f $rel); return $false }
    }

    return $true
}

# 配信用の IIS Express を起動・停止する
#
# Web 系の対象（Kind = "Web"）は本文側が起動・停止するが、こちらは
# **EXE を実行する対象なので、その仕組みに乗らない。**
# Pre で起動し、Verify の最後で止める。
$script:deployWebProc = $null

function Start-DeployWeb([string]$tag)
{
    $iis = Join-Path $env:ProgramFiles "IIS Express\iisexpress.exe"
    if (-not (Test-Path $iis)) { return $false }

    # **前回の残りを先に止める。**
    # 起動に失敗した IIS Express が URL の登録を握ったままだと、
    # 次の起動が 0x800700b7（既に存在する）で失敗し続ける。
    Stop-DeployWeb

    $web = Join-Path (Get-DeployWork $tag) "web"
    $log = Reset-Log (Join-Path $OutputDir "deploy_web_$tag.log")

    $script:deployWebProc = Start-Process $iis `
        -ArgumentList "/path:`"$web`"", "/port:$deployWebPort", "/systray:false" `
        -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput $log -RedirectStandardError "$log.err"

    # 起動を待つ（接続できるまで最大 15 秒）
    #
    # **コンテンツを要求して待ってはいけない。** 404 でも「起動している」ため、
    # 応答の内容で判定すると、ファイルが無いときに待ち続けて
    # 起動したままのプロセスが残る。TCP で繋がるかだけを見る。
    for ($i = 0; $i -lt 30; $i++)
    {
        Start-Sleep -Milliseconds 500

        $client = New-Object System.Net.Sockets.TcpClient
        try
        {
            $client.Connect("localhost", $deployWebPort)
            if ($client.Connected) { $client.Close(); return $true }
        }
        catch { }
        finally { $client.Dispose() }
    }

    Write-Host "     配信サーバが応答しない"
    Stop-DeployWeb
    return $false
}

function Stop-DeployWeb
{
    if ($null -ne $script:deployWebProc)
    {
        try { $script:deployWebProc | Stop-Process -Force -EA SilentlyContinue } catch { }
        $script:deployWebProc = $null
    }
}

# ZIP を作るのは対象と同じ実行ファイル（net48 / .NET 10 のそれぞれで確かめる）
$deployExe48   = Join-Path $csRoot "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\OpenTouryo.DeployZipPackWithHTTP.exe"
$deployExeCore = Join-Path $csRoot "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\net10.0-windows7.0\OpenTouryo.DeployZipPackWithHTTP.exe"

$prepareDeploy48   = { [void](New-DeployWeb "net48" $deployExe48) }
$prepareDeployCore = { [void](New-DeployWeb "core"  $deployExeCore) }
$mftGenArgs48      = New-MftGenArgs "net48"
$mftGenArgsCore    = New-MftGenArgs "core"
$verifyMftGen48    = { Test-MftGen "net48" }
$verifyMftGenCore  = { Test-MftGen "core" }
# 配置の確認が終わったら、配信サーバを止める（起動しっぱなしにしない）
$startDeployWeb48   = { [void](Start-DeployWeb "net48") }
$startDeployWebCore = { [void](Start-DeployWeb "core") }
$verifyDeploy48    = { $r = Test-Deploy "net48"; Stop-DeployWeb; return $r }
$verifyDeployCore  = { $r = Test-Deploy "core";  Stop-DeployWeb; return $r }

# ------------------------------------------------------------------
# WebAPI（バッチ更新）のホスト（#570）
# ------------------------------------------------------------------
# **TestWebAPIClient は EXE だが、相手に Web サーバが要る。**
# Kind = "Web" は対象自身が Web アプリのときの仕組みなので使えない。
# ここで起動し、Verify の最後で止める（DeployZipPackWithHTTP と同じ形）。
$script:apiWebProc = $null

function Stop-ApiWeb
{
    if ($script:apiWebProc -and -not $script:apiWebProc.HasExited)
    {
        $script:apiWebProc.Kill()
        $script:apiWebProc.WaitForExit(5000)
    }
    $script:apiWebProc = $null
}

function Start-ApiWeb([string]$kind, [int]$port)
{
    # **前回の残りを先に止める。**（Start-DeployWeb と同じ理由）
    Stop-ApiWeb

    $log = Reset-Log (Join-Path $OutputDir "api_web_$kind.log")

    if ($kind -eq "net48")
    {
        $iis = Join-Path $env:ProgramFiles "IIS Express\iisexpress.exe"
        if (-not (Test-Path $iis)) { return $false }

        $site = Join-Path $PSScriptRoot "CS\Samples\WS_sample\ASPNETWebService\ASPNETWebService"
        $script:apiWebProc = Start-Process $iis `
            -ArgumentList "/path:`"$site`"", "/port:$port", "/systray:false" `
            -PassThru -WindowStyle Hidden `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
    else
    {
        $dll = Join-Path $PSScriptRoot ("CS\Samples4NetCore\Backend\ASPNETWebService" +
            "\ASPNETWebService\bin\Debug\net10.0\ASPNETWebService.dll")
        if (-not (Test-Path $dll)) { return $false }

        # コンテンツ ルートを合わせるため、出力フォルダを作業ディレクトリにする。
        $script:apiWebProc = Start-Process "dotnet" `
            -ArgumentList "`"$dll`"", "--urls", "http://localhost:$port" `
            -PassThru -WindowStyle Hidden -WorkingDirectory (Split-Path $dll) `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }

    # **TCP で繋がるかだけを見る。**（SMOKETEST.md 9 節）
    for ($i = 0; $i -lt 40; $i++)
    {
        Start-Sleep -Milliseconds 500
        try
        {
            $client = New-Object System.Net.Sockets.TcpClient
            $client.Connect("localhost", $port)
            $client.Close()
            return $true
        }
        catch { }
    }

    Stop-ApiWeb
    return $false
}

# ------------------------------------------------------------------
# HTTP 要求
# ------------------------------------------------------------------
# リダイレクトを追わずに状態コードを見たいが、Invoke-WebRequest は
# -MaximumRedirection 0 で 3xx を受け取ると、-SkipHttpErrorCheck を付けていても
# 「The maximum redirection count has been exceeded」で終了エラーになる。
# ここで捕まえ、3xx を正常な結果として返す。
# Cookie を引き継ぐため、セッションは呼び出し側で作って渡す。
function New-WebSession
{
    return New-Object Microsoft.PowerShell.Commands.WebRequestSession
}

function Invoke-Http
{
    param(
        [string]$Uri,
        [string]$Method = "GET",
        $Body,
        $Session,
        # JSON を送る先（WebAPI）で使う。省略すると Invoke-WebRequest の既定
        # （ハッシュテーブルなら application/x-www-form-urlencoded）になる。
        [string]$ContentType
    )

    $p = @{
        Uri = $Uri; Method = $Method; WebSession = $Session
        MaximumRedirection = 0
        # 5.1 は既定で Internet Explorer のエンジンを使い、未構成だと解析に失敗する。
        # 7 では受け付けられて無視されるため、常に付けてよい。
        UseBasicParsing = $true
    }
    # -SkipHttpErrorCheck は PowerShell 7 以降にしかない。
    # 5.1 に渡すとパラメータ束縛で失敗するので付けない。
    # （5.1 では 4xx/5xx が例外になるが、下の catch で状態コードを取り出す）
    if ($PSVersionTable.PSVersion.Major -ge 6) { $p.SkipHttpErrorCheck = $true }
    if ($Body) { $p.Body = $Body }
    if ($ContentType) { $p.ContentType = $ContentType }

    try
    {
        $r = Invoke-WebRequest @p -ErrorAction Stop
        return [pscustomobject]@{ Status = [int]$r.StatusCode; Content = $r.Content; Length = $r.RawContentLength }
    }
    catch
    {
        $resp = $_.Exception.Response
        if ($resp) { return [pscustomobject]@{ Status = [int]$resp.StatusCode; Content = ""; Length = 0 } }
        return [pscustomobject]@{ Status = -1; Content = $_.Exception.Message; Length = 0 }
    }
}

# ------------------------------------------------------------------
# Web アプリの疎通手順
# ------------------------------------------------------------------
# 引数でベース URL を受け取り、@{ Ok = $bool; Detail = $string } を返す。
#
# ＜確認の深さを揃える＞
# 3 つとも「ログインを通して、認証が要る画面に到達できること」まで見る。
# 入口ページが 200 を返すだけでは、ホスティングと構成しか確認できない。
# 認証が要る画面まで通せば、ルーティング・認証・セッションまでを一度に確認できる。
#
#   | 対象                 | 認証の実装          | 到達を確認する画面   |
#   |----------------------|---------------------|----------------------|
#   | MVC_Sample (net48)   | FormsAuthentication | /Crud1/Index         |
#   | MVC_Sample (net10.0) | Cookie 認証         | /Crud1/Index         |
#   | WebForms_Sample      | FormsAuthentication | Aspx/start/menu.aspx |
#
# いずれのサンプルも「ユーザー名が空でなければ認証する」実装のため、資格情報は不要。

# MVC_Sample : net48 は FormsAuthentication、net10.0 は Cookie 認証と実装は異なるが、
# 画面構成と URL は同じなので同じ手順で確認できる。
# 認証後の応答は net48 が 302（RedirectFromLoginPage）、net10.0 が 200（View を返す）
# と分かれるため、ここでは 4xx/5xx でないことだけを見る。
$mvcLoginFlow = {
    param($base)

    $ses = New-WebSession

    $r1 = Invoke-Http "$base/Home/Login" -Session $ses
    if ($r1.Status -ne 200) { return @{ Ok = $false; Detail = "GET /Home/Login = $($r1.Status)" } }

    # ValidateAntiForgeryToken のため、画面からトークンを取り出して送り返す。
    $tok = ([regex]::Match($r1.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')).Groups[1].Value
    if (-not $tok) { return @{ Ok = $false; Detail = "__RequestVerificationToken が取得できない" } }

    # 500 はセッション状態サービスの停止など、環境側の問題であることが多い。
    $body = @{ UserName = "smoke"; Password = "smoke"; normal = "ログイン"; __RequestVerificationToken = $tok }
    $r2 = Invoke-Http "$base/Home/Login" -Method POST -Body $body -Session $ses
    if ($r2.Status -ge 400) { return @{ Ok = $false; Detail = "POST /Home/Login = $($r2.Status)" } }

    # 認証が要る画面。未認証ならログイン画面へ 302 されるため、200 なら認証が通っている。
    $r3 = Invoke-Http "$base/Crud1/Index" -Session $ses
    if ($r3.Status -ne 200) { return @{ Ok = $false; Detail = "GET /Crud1/Index = $($r3.Status)（認証が通っていない）" } }

    return @{ Ok = $true; Detail = "ログイン後 /Crud1/Index = 200" }
}

# WebForms_Sample (net48) : Web.config で <deny users="?" /> のため全画面が要認証。
# ログイン後は defaultUrl の menu.aspx へ遷移する。
$webFormsFlow = {
    param($base)

    $ses = New-WebSession

    $r1 = Invoke-Http "$base/Aspx/start/login.aspx" -Session $ses
    if ($r1.Status -ne 200) { return @{ Ok = $false; Detail = "GET login.aspx = $($r1.Status)" } }

    # WebForms のポストバックには、画面が発行した状態フィールドをそのまま返す必要がある。
    # __VIEWSTATE が取れること自体、ページのライフサイクルが動いている証拠でもある。
    $fields = @{}
    foreach ($n in @("__VIEWSTATE", "__VIEWSTATEGENERATOR", "__EVENTVALIDATION"))
    {
        $m = [regex]::Match($r1.Content, ('name="' + $n + '"[^>]*value="([^"]*)"'))
        if (-not $m.Success) { return @{ Ok = $false; Detail = "$n が取得できない" } }
        $fields[$n] = $m.Groups[1].Value
    }

    # マスタ ページ配下のため、コントロール名は ctl00$ContentPlaceHolder_A$ が付く。
    # btnButton1 がログイン ボタン（btnButton2 は「外部ログイン」。CS / VB とも同じ）。
    $fields["ctl00`$ContentPlaceHolder_A`$txtUserID"]   = "smoke"
    $fields["ctl00`$ContentPlaceHolder_A`$txtPassword"] = "smoke"
    $fields["ctl00`$ContentPlaceHolder_A`$btnButton1"]  = "ログイン"

    $r2 = Invoke-Http "$base/Aspx/start/login.aspx" -Method POST -Body $fields -Session $ses
    if ($r2.Status -ge 400) { return @{ Ok = $false; Detail = "POST login.aspx = $($r2.Status)" } }

    # 認証が要る画面。未認証なら login.aspx へ 302 されるため、200 なら認証が通っている。
    $r3 = Invoke-Http "$base/Aspx/start/menu.aspx" -Session $ses
    if ($r3.Status -ne 200) { return @{ Ok = $false; Detail = "GET menu.aspx = $($r3.Status)（認証が通っていない）" } }

    return @{ Ok = $true; Detail = "ログイン後 menu.aspx = 200" }
}


$targetsCS = @(
    # --- DaoGen_Tool（墨壺）の CUI モード ---
    # #508 で追加された /HELP と /CUI。GUI 側の確認は手作業に残る。
    # DAODEFGEN → DAOSQLGEN の順に実行し、前段の出力を後段の入力に使う。
    @{
        Name = "DaoGen_Tool /HELP (net48)";        Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\OpenTouryo.DaoGen_Tool.exe"
        Args = @("/HELP");  Expect = 'DaoGen_Tool（D層自動生成ツール／墨壺）'
    }
    @{
        Name = "DaoGen_Tool DAODEFGEN (net48)";    Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoDefArgs48;  Expect = '生成が完了しました。'
        Pre = $prepareDaoGen48;  Verify = $verifyDaoDef48
    }
    @{
        Name = "DaoGen_Tool DAOSQLGEN (net48)";    Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoSqlArgs48;  Expect = '生成が完了しました。'
        Verify = $verifyDaoGen48
    }
    @{
        Name = "DaoGen_Tool /HELP (net10.0)";      Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe"
        Args = @("/HELP");  Expect = 'DaoGen_Tool（D層自動生成ツール／墨壺）'
    }
    @{
        Name = "DaoGen_Tool DAODEFGEN (net10.0)";  Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoDefArgsCore;  Expect = '生成が完了しました。'
        Pre = $prepareDaoGenCore;  Verify = $verifyDaoDefCore
    }
    @{
        Name = "DaoGen_Tool DAOSQLGEN (net10.0)";  Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoSqlArgsCore;  Expect = '生成が完了しました。'
        Verify = $verifyDaoGenCore
    }

    # --- DeployZipPackWithHTTP の CUI モード ---
    # #528 で /MFTGEN を追加し、生成 → 配布 → 配置 まで CUI で通せるようになった。
    # マニュフェストを作り、IIS Express で配信し、実際に配置して突き合わせる。
    # ※ /NB を付けないと、配置した EXE が起動して止まる。
    @{
        Name = "DeployZip /MFTGEN (net48)";        Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $mftGenArgs48;  Expect = 'マニュフェスト ファイルを生成しました。'
        Pre = $prepareDeploy48;  Verify = $verifyMftGen48
    }
    @{
        Name = "DeployZip 配置 (net48)";           Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $deployArgs;  Expect = '履歴に新規追加しました。'
        Pre = $startDeployWeb48;  Verify = $verifyDeploy48
    }
    @{
        Name = "DeployZip /MFTGEN (net10.0)";      Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\net10.0-windows7.0\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $mftGenArgsCore;  Expect = 'マニュフェスト ファイルを生成しました。'
        Pre = $prepareDeployCore;  Verify = $verifyMftGenCore
    }
    @{
        Name = "DeployZip 配置 (net10.0)";         Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\net10.0-windows7.0\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $deployArgs;  Expect = '履歴に新規追加しました。'
        Pre = $startDeployWebCore;  Verify = $verifyDeployCore
    }

    # --- 通信制御の接続オプション（#546） ---
    #
    # CallController の接続オプション（ProxyUrl / UserName / UserAgent / Compression 等）が、
    # 実際の HTTP 要求に反映されているかを見る。
    #
    # ＜外部環境が要らない＞
    #   オリジンとプロキシを、テスト側が TcpListener で自前に立てる。
    #   1 プロセスに閉じているので、起動・停止の面倒を見る必要がない。
    #
    # ＜net48 だけ＞
    #   ASP.NET WebAPI の経路が .NET Framework 限定である（BinarySerialize を使うため）。
    #
    # ＜判定＞
    #   対象側が項目ごとに OK / NG を出し、末尾に件数を出す。
    @{
        Name = "TestTransmission (net48)";  Bat = "y_Build_TestTransmission.bat"
        Exe  = "Frameworks\Tests\TestTransmission\net48\bin\Debug\TestTransmissionFx.exe"
        Expect = 'NG : 0 件'
    }

    # --- バッチ (net48) ---
    @{
        Name = "SimpleBatch_sample (net48)";      Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\SimpleBatch_sample\bin\Debug\SimpleBatch_sample.exe"
        Args = $batchArgs;  Expect = '\d+件のデータがあります'
    }
    @{
        Name = "RerunnableBatch_sample (net48)";  Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample\bin\Debug\RerunnableBatch_sample.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample2 (net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample2\bin\Debug\RerunnableBatch_sample2.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample3 (net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample3\bin\Debug\RerunnableBatch_sample3.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }

    # --- バッチ (net10.0) ---
    @{
        Name = "SimpleBatch_sample (net10.0)";      Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\SimpleBatch_sample\bin\Debug\net10.0\SimpleBatch_sample.dll"
        Args = $batchArgs;  Expect = '\d+件のデータがあります'
    }
    @{
        Name = "RerunnableBatch_sample (net10.0)";  Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\RerunnableBatch_sample\bin\Debug\net10.0\RerunnableBatch_sample.dll"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample2 (net10.0)"; Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\RerunnableBatch_sample2\bin\Debug\net10.0\RerunnableBatch_sample2.dll"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample3 (net10.0)"; Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\RerunnableBatch_sample3\bin\Debug\net10.0\RerunnableBatch_sample3.dll"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }

    # --- CLI (net10.0) ---
    # net48 版は System.CommandLine / Sharprompt の .NET Fx サポート終了により
    # ドロップされている（5_Build_CLI_sample.bat 参照）。
    # interactive サブコマンドは Prompt を使うため対象外とし、非対話のものを使う。
    @{
        Name = "Simple_CLI (net10.0)";              Bat = "5_Build_CLICore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\CLI_sample\Simple_CLI\Simple_CLI\bin\Debug\net10.0\Simple_CLI.dll"
        Args = @("cmd1", "--an-int", "123");  Expect = 'Sub command cmd1: 123'
    }

    # --- DTO を使用したバッチ更新（WebAPI Client）（#570） ---
    #
    # **DataTable を DTTables 経由で JSON にして往復させ、
    #   RowState と Original が保たれることを、HTTP 越しに確かめる。**
    #
    # ＜対象自身は EXE＞
    #   相手の WebAPI は Pre で起動し、Verify の最後で止める。
    #
    # ＜net48 と net10.0 で同じクライアントを使う＞
    #   接続先を引数で切り替えるだけ。応答の形も揃えてある。
    #
    # ＜判定＞
    #   対象側が項目ごとに OK / NG を出し、末尾に件数を出す。
    @{
        Name = "TestWebAPIClient (net48)";   Bat = "y_Build_TestWebAPIClient.bat"
        Exe  = "Frameworks\Tests\TestWebAPIClient\net48\bin\Debug\TestWebAPIClientFx.exe"
        Args = @("http://localhost:51087")
        Expect = 'NG : 0 件'
        Pre = { if (-not (Start-ApiWeb "net48" 51087)) { throw "WebAPI のホストを起動できません（port 51087）" } }
        Verify = { Stop-ApiWeb; return $true }
    }
    @{
        Name = "TestWebAPIClient (net10.0)"; Bat = "y_Build_TestWebAPIClient.bat"
        Exe  = "Frameworks\Tests\TestWebAPIClient\net48\bin\Debug\TestWebAPIClientFx.exe"
        Args = @("http://localhost:51088")
        Expect = 'NG : 0 件'
        Pre = { if (-not (Start-ApiWeb "net10.0" 51088)) { throw "WebAPI のホストを起動できません（port 51088）" } }
        Verify = { Stop-ApiWeb; return $true }
    }

    # --- Web アプリ ---
    @{
        Name = "WebForms_Sample (net48)"; Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51082
        Site = "Samples\WebApp_sample\WebForms_Sample\WebForms_Sample"
        Need = "aspnet_state"
        Flow = $webFormsFlow
    }
    @{
        Name = "MVC_Sample (net48)";      Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51081
        Site = "Samples\WebApp_sample\MVC_Sample\MVC_Sample"
        Need = "aspnet_state"
        Flow = $mvcLoginFlow
    }
    @{
        Name = "MVC_Sample (net10.0)";    Bat = "10_Build_WebAppCore_sample.bat"
        Kind = "Web";  WebHost = "Kestrel";  Port = 51083
        Exe  = "Samples4NetCore\Backend\MVC_Sample\MVC_Sample\bin\Debug\net10.0\MVC_Sample.dll"
        Flow = $mvcLoginFlow
    }
)

# ------------------------------------------------------------------
# VB 版の対象
# ------------------------------------------------------------------
# VB にあるのは Bat_sample と WebApp_sample だけで、Core 版・CLI_sample・
# ツール（Frameworks\Tools）は無い（#542）。
#
# 判定（Args / Pre / Verify / Flow）は C# 版と同じものを使い回す。
# 上の定義と同じ変数を指しているので、片方だけ直すことができない。
#
# ＜ポートを分ける＞
#   -Lang Both では C# 版と続けて起動するため、51081/51082 とは別にする。
$targetsVB = @(
    # --- バッチ (net48) ---
    @{
        Name = "SimpleBatch_sample (VB net48)";      Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\SimpleBatch_sample\bin\Debug\SimpleBatch_sample.exe"
        Args = $batchArgs;  Expect = '\d+件のデータがあります'
    }
    @{
        Name = "RerunnableBatch_sample (VB net48)";  Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample\bin\Debug\RerunnableBatch_sample.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample2 (VB net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample2\bin\Debug\RerunnableBatch_sample2.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample3 (VB net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample3\bin\Debug\RerunnableBatch_sample3.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }

    # --- Web アプリ ---
    @{
        Name = "MVC_Sample (VB net48)";      Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51085
        Site = "Samples\WebApp_sample\MVC_Sample\MVC_Sample"
        Need = "aspnet_state"
        Flow = $mvcLoginFlow
    }
    @{
        Name = "WebForms_Sample (VB net48)"; Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51086
        Site = "Samples\WebApp_sample\WebForms_Sample\WebForms_Sample"
        Need = "aspnet_state"
        Flow = $webFormsFlow
    }
)

# 省略されている Dir を、その言語のフォルダで補う。
function Add-DefaultDir($items, [string]$dir)
{
    foreach ($i in $items)
    {
        if (-not $i.ContainsKey("Dir")) { $i.Dir = $dir }
    }
    return $items
}

$targets = @()
if ($Lang -ne "VB") { $targets += @(Add-DefaultDir $targetsCS "CS") }
if ($Lang -ne "CS") { $targets += @(Add-DefaultDir $targetsVB "VB") }

# ------------------------------------------------------------------
# VB のビルドは自己完結しない
# ------------------------------------------------------------------
# C# 側は各ビルド バッチが単独で通るが、VB 側は違う。
# 5_Build_Bat_sample.bat だけを呼んでも、参照するアセンブリが揃っていないため
# 建たない。VB\0_ExecAllBat.bat が前段で行っていることを、ここでも行う。
#
#   2_Build_NuGet_net48.bat（CS 側）  … 1_GetLibrariesFromCS.bat が取りに行く実体
#   1_GetLibrariesFromCS.bat          … CS の Build_net48 を VB 配下へ複写
#   3_Build_Business_net48.bat        … VB の Business（B層基盤）
#   3_Build_BusinessRichClient_net48.bat
#   4_Build_CopyAssemblies.bat        … 参照解決用の Build フォルダを作る
#
# ＜0_ExecAllBat.bat を丸ごと呼ばない理由＞
#   ・1_DeleteDir.bat が bin / obj を消す。疎通は成果物を使うので、消したくない
#   ・WinForms / WPF のサンプルまで建てる。疎通の対象ではないので、時間だけ増える
#   ・timeout 5 が入っており、stdin をリダイレクトすると
#     「ERROR: Input redirection is not supported」がログに出る
$prerequisitesVB = @(
    @{ Dir = "CS"; Bat = "2_Build_NuGet_net48.bat" }
    @{ Dir = "VB"; Bat = "1_GetLibrariesFromCS.bat" }
    @{ Dir = "VB"; Bat = "3_Build_Business_net48.bat" }
    @{ Dir = "VB"; Bat = "3_Build_BusinessRichClient_net48.bat" }
    @{ Dir = "VB"; Bat = "4_Build_CopyAssemblies.bat" }
)

# ------------------------------------------------------------------
# ビルド
# ------------------------------------------------------------------
function Invoke-BuildBat([string]$dir, [string]$bat)
{
    $root = Join-Path $PSScriptRoot $dir
    $path = Join-Path $root $bat

    if (-not (Test-Path $path))
    {
        Write-Host ("  ビルド バッチが見つかりません : {0}\{1}" -f $dir, $bat) -ForegroundColor Red
        return $false
    }

    $log = Join-Path $OutputDir ("build_" + $dir + "_" + ($bat -replace '[^A-Za-z0-9]','_') + ".log")

    # バッチは自分のフォルダから呼ぶ。相対パスで参照を解決しているため、
    # 呼び出し元のカレントが違うと、ソリューションを見つけられない。
    Push-Location $root
    # バッチ末尾（および途中）の pause に対応するため stdin を与える。
    cmd /c "echo. | call `"$path`"" *>&1 | Out-File $log -Encoding UTF8
    Pop-Location

    return $true
}

# ------------------------------------------------------------------
# Web ホストの起動・停止
# ------------------------------------------------------------------
# 起動しただけでは応答できないため、ポートが開くまで待ってから疎通する。
function Wait-Port([int]$port, [int]$timeoutSec = 30)
{
    $sw = [Diagnostics.Stopwatch]::StartNew()
    while ($sw.Elapsed.TotalSeconds -lt $timeoutSec)
    {
        $c = New-Object Net.Sockets.TcpClient
        try
        {
            $c.Connect("127.0.0.1", $port)
            if ($c.Connected) { $c.Close(); return $true }
        }
        catch { }
        finally { $c.Dispose() }
        Start-Sleep -Milliseconds 300
    }
    return $false
}

function Start-WebHost($t, [string]$log)
{
    $root = Join-Path $PSScriptRoot $t.Dir

    if ($t.WebHost -eq "IISExpress")
    {
        $iis = Join-Path $env:ProgramFiles "IIS Express\iisexpress.exe"
        if (-not (Test-Path $iis)) { return $null }

        $site = Join-Path $root $t.Site
        return Start-Process $iis `
            -ArgumentList "/path:`"$site`"", "/port:$($t.Port)", "/systray:false" `
            -PassThru -WindowStyle Hidden `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
    else
    {
        $dll = Join-Path $root $t.Exe
        if (-not (Test-Path $dll)) { return $null }

        # コンテンツ ルートを合わせるため、出力フォルダを作業ディレクトリにする。
        return Start-Process "dotnet" `
            -ArgumentList "`"$dll`"", "--urls", "http://localhost:$($t.Port)" `
            -PassThru -WindowStyle Hidden -WorkingDirectory (Split-Path $dll) `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
}

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
Write-Host ("対象 : {0}" -f $Lang) -ForegroundColor Cyan

$selected = @($targets | Where-Object { -not $Only -or $_.Name -like "*$Only*" })

if (-not $SkipBuild)
{
    # ビルドする単位は「フォルダ ＋ バッチ」。同じバッチ名が CS と VB の
    # 両方にあるため（5_Build_Bat_sample.bat 等）、バッチ名だけでは一意化できない。
    #
    # 並びは「C# の対象 → VB の前提 → VB の対象」とする。
    # 前提を先頭に置くと、-Lang Both で VB の準備が C# の対象より先に流れ、
    # 1_BuildAll.ps1（CS → VB の順）と見た目が揃わない。依存関係は
    # 「VB の前提が VB の対象より前」だけなので、この並びで足りる。
    $builds = @()

    $builds += @($selected | Where-Object { $_.Dir -ne "VB" } |
                 ForEach-Object { @{ Dir = $_.Dir; Bat = $_.Bat; Note = "" } })

    $vb = @($selected | Where-Object { $_.Dir -eq "VB" })
    if ($vb.Count -gt 0)
    {
        $builds += @($prerequisitesVB |
                     ForEach-Object { @{ Dir = $_.Dir; Bat = $_.Bat; Note = "（VB の前提）" } })
        $builds += @($vb | ForEach-Object { @{ Dir = $_.Dir; Bat = $_.Bat; Note = "" } })
    }

    $done = @{}
    foreach ($b in $builds)
    {
        $key = $b.Dir + "\" + $b.Bat
        if ($done.ContainsKey($key)) { continue }
        $done[$key] = $true

        Write-Host ("=== ビルド : {0}{1} ===" -f $key, $b.Note) -ForegroundColor Cyan
        $sw = [Diagnostics.Stopwatch]::StartNew()
        [void](Invoke-BuildBat $b.Dir $b.Bat)
        $sw.Stop()
        Write-Host ("  完了 ({0:N1} 秒)" -f $sw.Elapsed.TotalSeconds)
    }
}

$results = @()

foreach ($t in $selected)
{
    Write-Host ""
    Write-Host ("=== {0} ===" -f $t.Name) -ForegroundColor Cyan

    $safe = ($t.Name -replace '[^A-Za-z0-9]', '_')
    $out  = Join-Path $OutputDir "$safe.txt"

    # ------------------------------------------------------------------
    # Web アプリ
    # ------------------------------------------------------------------
    if ($t.Kind -eq "Web")
    {
        # 前提サービスの確認
        # ※ サービスの開始はシステムの状態を変えるため、ここでは行わない。
        #    足りない場合は対処方法を示して NG にする。
        if ($t.Need)
        {
            $svc = Get-Service $t.Need -EA SilentlyContinue
            if (-not $svc -or $svc.Status -ne "Running")
            {
                $msg = "前提サービス {0} が動いていない → Start-Service {0}" -f $t.Need
                Write-Host ("  {0}" -f $msg) -ForegroundColor Yellow
                $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "前提未達"; 内容 = $msg }
                continue
            }
        }

        $sw = [Diagnostics.Stopwatch]::StartNew()
        # **使う前に消す。**（#571）
        #   残っていると、起動に失敗しても前回のログが読まれる。
        $null = Reset-Log $out

        $proc = Start-WebHost $t $out

        if (-not $proc)
        {
            Write-Host "  Web ホストを起動できません（実行ファイル／IIS Express が無い）" -ForegroundColor Red
            $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "起動不可"; 内容 = "-" }
            continue
        }

        try
        {
            Write-Host ("  起動中 ... (PID {0}, port {1})" -f $proc.Id, $t.Port)
            if (-not (Wait-Port $t.Port))
            {
                Write-Host "  ポートが開きません" -ForegroundColor Red
                $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "NG"; 内容 = "ポートが開かない" }
                continue
            }

            $flow = & $t.Flow "http://localhost:$($t.Port)"
            $sw.Stop()

            $verdict = if ($flow.Ok) { "OK" } else { "NG" }
            Write-Host ("     {0}" -f $flow.Detail)
            Write-Host ("  {0}  ({1:N1} 秒)" -f $verdict, $sw.Elapsed.TotalSeconds) `
                -ForegroundColor $(if ($flow.Ok) { "Green" } else { "Red" })

            $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = $verdict; 内容 = $flow.Detail }
        }
        finally
        {
            # 子プロセスごと確実に落とす。
            Stop-Process -Id $proc.Id -Force -EA SilentlyContinue
        }
        continue
    }

    # ------------------------------------------------------------------
    # プロセス（バッチ・CLI）
    # ------------------------------------------------------------------
    $exe = Join-Path (Join-Path $PSScriptRoot $t.Dir) $t.Exe
    if (-not (Test-Path $exe))
    {
        Write-Host "  実行ファイルが見つかりません" -ForegroundColor Red
        $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "実行ファイル無し"; 内容 = "-" }
        continue
    }

    # 事前準備
    if ($t.Pre)
    {
        try { & $t.Pre }
        catch
        {
            Write-Host ("  事前準備に失敗 : {0}" -f $_.Exception.Message) -ForegroundColor Red
            $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "準備失敗"; 内容 = "-" }
            continue
        }
    }

    # 実行
    #
    # ＜Start-Process で、PowerShell を経由せずに入出力を扱う理由＞
    #
    #   1. stdin を与えないと止まる
    #      サンプルは終了前に Console.ReadKey() を呼ぶ（SimpleBatch_sample/Program.cs 等）。
    #      stdin がコンソールのままだと**本当にキー入力を待つ**。
    #      実測では SimpleBatch_sample (net48) が 386 秒かかった。
    #      空ファイルを与えると、待たずに InvalidOperationException になる
    #      （この例外は下の判定で除外する）。
    #
    #   2. PowerShell のパイプを通すと、stderr が壊れる
    #      「| Out-File」で受けると、native の stderr が ErrorRecord になり
    #      **コンソール幅で折り返される**。折り返しは語の途中にも入るため、
    #      「does not h ave a console」のように**単語が割れて復元できない**。
    #      幅は環境で違うので、後段の除外規則をいくら調整しても直らない。
    #
    #   Start-Process でファイルへ直接リダイレクトすれば、どちらも起きない。
    #   出力は生のまま残り、環境による差も出ない。
    $err = "$out.err"

    # **前回の出力を必ず消してから実行する。**
    #   $out / $err は対象ごとの固定名で、$OutputDir（既定は %TEMP%）に残る。
    #   消さずに実行すると、**起動に失敗しても前回の出力が判定に使われ、
    #   実行していないのに OK になる**（所要時間が 0.0 秒になるのが兆候）。
    Remove-Item $out, $err -Force -ErrorAction SilentlyContinue

    Write-Host "  実行中 ..."
    $sw = [Diagnostics.Stopwatch]::StartNew()

    if ($exe -like "*.dll")
    {
        # 「/DAP」のように「/」で始まる引数は dotnet 自身のオプションと紛らわしいため、
        # 「--」で区切って渡す（リポジトリのビルド バッチも同じ渡し方をしている）。
        # 一方 System.CommandLine を使う CLI では「--」以降が未解析トークン扱いになり、
        # サブコマンドが認識されなくなるため、区切らずに渡す。
        $needSep = @($t.Args | Where-Object { $_ -like "/*" }).Count -gt 0

        $argList = @("`"$exe`"")
        if ($needSep) { $argList += "--" }
        $argList += @($t.Args | Where-Object { $null -ne $_ })

        Start-Process "dotnet" -ArgumentList $argList -NoNewWindow -Wait `
            -WorkingDirectory (Split-Path $exe) `
            -RedirectStandardInput $emptyIn -RedirectStandardOutput $out -RedirectStandardError $err
    }
    else
    {
        # **null 要素を落としてから数える。**
        #   Args を持たない対象では $t.Args が $null になり、@($null) は
        #   「要素数 1・中身 null」の配列になる。**Count は 0 にならない**ので、
        #   下のガードをすり抜けて -ArgumentList に null 入りの配列が渡る。
        #   これを Windows PowerShell 5.1 は拒否する（PS 7 は空文字に変換して通す）。
        #
        #     PS 5.1 : Start-Process : パラメーター 'ArgumentList の引数を確認できません。…
        #     PS 7   : そのまま動く
        #
        #   5.1 でだけ落ちるため、7 で検証していると気付けない。
        $argList = @($t.Args | Where-Object { $null -ne $_ })

        if ($argList.Count -eq 0)
        {
            # -ArgumentList に空の配列を渡すとエラーになる。
            Start-Process $exe -NoNewWindow -Wait `
                -WorkingDirectory (Split-Path $exe) `
                -RedirectStandardInput $emptyIn -RedirectStandardOutput $out -RedirectStandardError $err
        }
        else
        {
            Start-Process $exe -ArgumentList $argList -NoNewWindow -Wait `
                -WorkingDirectory (Split-Path $exe) `
                -RedirectStandardInput $emptyIn -RedirectStandardOutput $out -RedirectStandardError $err
        }
    }

    $sw.Stop()

    # 標準出力と標準エラーを両方見る。
    #
    # **-Encoding UTF8 を必ず付ける。**
    #   Start-Process は子プロセスの生バイトをそのまま書くため、BOM が付かない。
    #   Windows PowerShell 5.1 の Get-Content は BOM が無いと既定の ANSI（日本語環境では
    #   CP932）で読むため、UTF-8 の日本語が化けて期待値に一致しなくなる。
    #   （従来の Out-File -Encoding UTF8 は BOM 付きだったので、たまたま成立していた）
    $text = (Get-Content $out -Raw -Encoding UTF8 -EA SilentlyContinue) + "`n" `
          + (Get-Content $err -Raw -Encoding UTF8 -EA SilentlyContinue)
    if ($null -eq $text) { $text = "" }

    # 判定
    #
    # ※ 末尾の Console.ReadKey() 由来の例外はテスト内容と無関係なので除外する。
    #
    #   PowerShell は native コマンドの stderr を ErrorRecord にし、
    #   **コンソール幅で折り返す**。折り返しで語が分断されると行単位の除外が効かない。
    #   実際 dotnet 起動の 4 件だけが、除外できずに NG になった。
    #     例外 : dotnet.exe : Unhandled exception. ... does not h
    #                                                         ↑ ここで改行
    #   このため、**空白を畳んで 1 行にしてから**除外・判定する。
    #   コンソール幅に依存しなくなる。
    $flat = ($text -replace '\s+', ' ')

    # ReadKey 由来の例外を、宣言からスタック トレースまで丸ごと落とす。
    #
    # ＜メッセージ本文で判定しない理由＞
    #   文言も見出しも環境で変わる。実測した 3 通り。
    #     .NET (Core) 英語 : Unhandled exception. System.InvalidOperationException: Cannot read keys ...
    #     .NET Fx     英語 : Unhandled Exception: System.InvalidOperationException: Cannot read keys ...
    #     .NET Fx     日本語: Exception.ToString() が失敗したため、例外文字列を表示できません。
    #   見出しは「.」と「:」で違い、日本語環境では**型名すら出ない**。
    #   このため本文ではなく、**言語に依存しないスタック トレース**で捕まえる。
    #
    # ＜クラス名も実行環境で違う＞
    #     .NET Fx     : at System.Console.ReadKey(Boolean intercept)
    #     .NET (Core) : at System.ConsolePal.ReadKey(Boolean intercept)
    #   このため System.〇〇.ReadKey として受ける。
    $flat = $flat -replace `
        '(Unhandled Exception[.:]\s*)?System\.\w+(\.\w+)*Exception[^|]*?at System\.\w+\.ReadKey[^ ]*', ''

    $fatal = @()
    $m0 = [regex]::Match($flat, '(Unhandled exception|ハンドルされない例外|System\.\w+Exception).{0,160}')
    if ($m0.Success)
    {
        $fatal = @($m0.Value.Trim())
    }

    $ok = $true
    $detail = ""

    if ($fatal)
    {
        $ok = $false
        $detail = ($fatal | Select-Object -First 1).Trim()
        Write-Host ("  例外 : {0}" -f $detail) -ForegroundColor Red
    }
    elseif ($t.Expect -and $text -notmatch $t.Expect)
    {
        $ok = $false
        $detail = "期待する出力が無い : " + $t.Expect
        Write-Host ("  {0}" -f $detail) -ForegroundColor Red
    }

    if ($ok -and $t.Expect)
    {
        $m = [regex]::Match($text, $t.Expect)
        $detail = $m.Value
        Write-Host ("     出力 : {0}" -f $m.Value)
    }

    if ($ok -and $t.Verify)
    {
        if (-not (& $t.Verify))
        {
            $ok = $false
            $detail = "検証に失敗"
            Write-Host "  検証に失敗" -ForegroundColor Red
        }
    }

    $verdict = if ($ok) { "OK" } else { "NG" }
    Write-Host ("  {0}  ({1:N1} 秒)" -f $verdict, $sw.Elapsed.TotalSeconds) `
        -ForegroundColor $(if ($ok) { "Green" } else { "Red" })

    $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = $verdict; 内容 = $detail }
}

# ------------------------------------------------------------------
# サマリ
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ サマリ ================"
Write-Host ""
Write-SummaryTable $results
Write-Host ""
Write-Host ("  ログ : {0}" -f $OutputDir)

$ng = @($results | Where-Object { $_.結果 -ne "OK" })
if ($ng.Count -eq 0)
{
    Write-Host "  全対象 OK" -ForegroundColor Green
    exit 0
}
else
{
    Write-Host ("  {0} 件が NG" -f $ng.Count) -ForegroundColor Yellow
    exit 1
}
