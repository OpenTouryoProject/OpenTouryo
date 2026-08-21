<#
.SYNOPSIS
    疎通テストの基盤ユーティリティ

.DESCRIPTION
    接続文字列・SQL 実行・ログの初期化・Orders2 の準備と、
    ツール（DaoGen_Tool / DeployZipPackWithHTTP）の準備と検証。

    **環境に触るものをここへ集める。** どこで DB や作業フォルダを
    触っているかが 1 か所で分かる。

    **3_SmokeTest.ps1 からドット ソースで読まれる。**
    単体では動かない（#571 で分割）。
#>

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

    # **取り逃がしも消す。**（#578）
    #   残った iisexpress は 51084 を握り続ける。
    #   次の起動は 0x800700b7 で失敗するが、**ポートには繋がる**ため
    #   起動待ちを通過し、**前回の web フォルダが配られ続ける。**
    #   配置は成功するので異常が出ず、判定だけが 0 件になる。
    try
    {
        Get-Process -Name "iisexpress" -EA SilentlyContinue |
            Stop-Process -Force -EA SilentlyContinue
    }
    catch { }
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
# プロキシ経由の配置（#578）
# ------------------------------------------------------------------
#   **#575 で HttpWebRequest を HttpClient へ移した経路の確認。**
#   HttpClientHandler では Proxy = null が「使わない」にならず
#   UseProxy = false が要る、という差があったため、実際に通して確かめる。
$deployProxyPort = 51085
$deployProxyUser = "pxuser"
$deployProxyPwd  = "pxpass"

# **localhost を使わない。**（#578）
#   .NET Framework の WebProxy は**ループバック宛を無条件にバイパスする**
#   （BypassProxyOnLocal = False でも IsBypassed が True）。
#   localhost のままだと net48 ではプロキシを通らず、経路を確かめられない。
#   hosts は書き換えず、**プロキシ側で 127.0.0.1 に繋ぎ替える**（st_Proxy.ps1 -MapHost）。
$deployHostAlias = "deploy.smoketest"

function Get-ProxyLog([string]$tag)
{
    return (Join-Path $OutputDir ("deploy_proxy_{0}.log" -f $tag))
}

function Stop-DeployProxy
{
    if ($null -ne $script:deployProxyProc)
    {
        try { $script:deployProxyProc | Stop-Process -Force -EA SilentlyContinue } catch { }
        $script:deployProxyProc = $null
    }

    # **取り逃がしも消す。**
    #   前回の実行が異常終了すると待ち受けを握ったまま残り、
    #   次の起動が黙って失敗する（ポートは繋がるので気づけない）。
    try
    {
        Get-CimInstance Win32_Process -Filter "Name='powershell.exe' OR Name='pwsh.exe'" -EA SilentlyContinue |
            Where-Object { $_.CommandLine -like "*st_Proxy.ps1*" } |
            ForEach-Object { Stop-Process -Id $_.ProcessId -Force -EA SilentlyContinue }
    }
    catch { }
}

function Start-DeployProxy([string]$tag, [bool]$auth)
{
    # **前回の残りを先に止める。** 掴んだままだと待ち受けに失敗する。
    Stop-DeployProxy

    $log = Get-ProxyLog $tag
    if (Test-Path $log) { Remove-Item $log -Force -EA SilentlyContinue }

    $ps1 = Join-Path $PSScriptRoot "st_Proxy.ps1"

    # **利用者の既定は 5.1 だが、ここは疎通の足場なので実行中のホストに合わせる。**
    $host_ = if ($PSVersionTable.PSVersion.Major -ge 6) { "pwsh" } else { "powershell" }

    $a = @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", $ps1,
           "-Port", $deployProxyPort, "-LogPath", $log,
           "-MapHost", $deployHostAlias)
    if ($auth) { $a += @("-User", $deployProxyUser, "-Password", $deployProxyPwd) }

    $script:deployProxyProc = Start-Process $host_ -ArgumentList $a -PassThru -WindowStyle Hidden

    # 起動を待つ。
    #
    # **TCP で繋がるかだけでは足りない。**（Start-DeployWeb とは事情が違う）
    #   前回のプロキシが待ち受けを握ったままだと接続は成功するが、
    #   こちらのプロセスは「ポート使用中」で死んでいる。
    #   **自分のログに [start] が出るまで待つ。**
    for ($i = 0; $i -lt 40; $i++)
    {
        Start-Sleep -Milliseconds 300

        if ($script:deployProxyProc.HasExited) { return $false }

        if (Test-Path $log)
        {
            $head = @(Get-Content $log -TotalCount 5 -EA SilentlyContinue)
            if ($head | Where-Object { $_ -match "\[start\]" }) { return $true }
        }
    }

    return $false
}

# **配置が成功しただけでは足りない。**
#   プロキシを無視して直結しても成功するため、
#   ログに要求 URL が並ぶことまで見て「経路を通った」と言える。
function Test-ProxyUsed([string]$tag, [bool]$auth)
{
    $log = Get-ProxyLog $tag

    if (-not (Test-Path $log))
    {
        Write-Host "     プロキシのログが無い : $log"
        return $false
    }

    $lines = @(Get-Content $log -EA SilentlyContinue)
    $hit = @($lines | Where-Object { $_ -match ("http://{0}:{1}/" -f $deployHostAlias, $deployWebPort) })

    if ($hit.Count -eq 0)
    {
        Write-Host "     プロキシを経由していない（ログに要求が無い）"
        return $false
    }

    # マニフェストと ZIP の両方が通っていること
    if (-not ($hit | Where-Object { $_ -match "\.mft" }))
    {
        Write-Host "     マニフェストの要求がプロキシを通っていない"
        return $false
    }

    if (-not ($hit | Where-Object { $_ -match "\.zip" }))
    {
        Write-Host "     ZIP の要求がプロキシを通っていない"
        return $false
    }

    if ($auth)
    {
        # **407 が出ていること**が、資格情報の経路を通った証拠になる。
        if (-not ($lines | Where-Object { $_ -match "^\[407\]" }))
        {
            Write-Host "     407 が記録されていない（認証を要求できていない）"
            return $false
        }
    }

    return $true
}

# **WWWURL も別名にする。** localhost のままではバイパスされる。
$deployArgsProxy = @("/CUI", "/NB", "/FORCE", "/WWWURL",
                     ("http://{0}:{1}/FormAppRoot.mft" -f $deployHostAlias, $deployWebPort),
                     "/ProxyURL", ("http://localhost:{0}" -f $deployProxyPort))

$deployArgsProxyAuth = $deployArgsProxy + @("/ProxyUID", $deployProxyUser,
                                            "/ProxyPWD", $deployProxyPwd)

$startProxy48     = { if (-not (Start-DeployProxy "net48" $false)) { throw "プロキシを起動できません（port $deployProxyPort）" }
                      if (-not (Start-DeployWeb "net48"))          { throw "配信サーバを起動できません（port $deployWebPort）" } }
$startProxyCore   = { if (-not (Start-DeployProxy "core" $false))  { throw "プロキシを起動できません（port $deployProxyPort）" }
                      if (-not (Start-DeployWeb "core"))           { throw "配信サーバを起動できません（port $deployWebPort）" } }
$startProxyAuth48 = { if (-not (Start-DeployProxy "net48auth" $true)) { throw "プロキシを起動できません（port $deployProxyPort）" }
                      if (-not (Start-DeployWeb "net48"))             { throw "配信サーバを起動できません（port $deployWebPort）" } }
$startProxyAuthCore = { if (-not (Start-DeployProxy "coreauth" $true)) { throw "プロキシを起動できません（port $deployProxyPort）" }
                        if (-not (Start-DeployWeb "core"))             { throw "配信サーバを起動できません（port $deployWebPort）" } }

$verifyProxy48       = { $r = (Test-Deploy "net48") -and (Test-ProxyUsed "net48" $false);     Stop-DeployWeb; Stop-DeployProxy; return $r }
$verifyProxyCore     = { $r = (Test-Deploy "core")  -and (Test-ProxyUsed "core" $false);      Stop-DeployWeb; Stop-DeployProxy; return $r }
$verifyProxyAuth48   = { $r = (Test-Deploy "net48") -and (Test-ProxyUsed "net48auth" $true);  Stop-DeployWeb; Stop-DeployProxy; return $r }
$verifyProxyAuthCore = { $r = (Test-Deploy "core")  -and (Test-ProxyUsed "coreauth" $true);   Stop-DeployWeb; Stop-DeployProxy; return $r }

# ------------------------------------------------------------------
# WebAPI（バッチ更新）のホスト（#570）
# ------------------------------------------------------------------
# **TestWebAPIClient は EXE だが、相手に Web サーバが要る。**
# Kind = "Web" は対象自身が Web アプリのときの仕組みなので使えない。
# ここで起動し、Verify の最後で止める（DeployZipPackWithHTTP と同じ形）。
$script:apiWebProc = $null
