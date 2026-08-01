<#
.SYNOPSIS
    サンプル アプリを起動して疎通を確認し、合否を一覧する。

.DESCRIPTION
    リリース時の「サンプルを幾つか見繕って手動で疎通を行う」を機械化したもの（#513 段階 3）。

    ＜段階 1・2 との違い＞
      段階 1（RunAllTests.ps1）は「出力が前回と同じか」を見る回帰テスト。
      本スクリプトは「起動して、想定どおり動き、例外なく終わるか」を見る疎通テスト。
      期待結果ファイルは持たず、判定条件を定義側に書く。

    ＜対象＞
      非 UI 系（バッチ・CLI）と Web 系（Web アプリ・Web サービス）。
      WinForms / WPF 系は UI Automation が必要で維持費が高いため対象外とし、
      リリース チェックリストの手作業項目として残す。

    ＜ビルド＞
      リポジトリ既定のビルド バッチを呼ぶ。理由は RunAllTests.ps1 と同じで、
      csproj を直接ビルドすると nuget.exe restore が行うネイティブ DLL の配置が漏れる。

      なお 0_ExecAllBat.bat は途中で 1_DeleteDir.bat を繰り返し実行し、
      配下の bin / obj を再帰的に削除する。このため BuildAll.ps1 の完走後に
      残るのは最後にビルドされた Core サンプルだけで、net48 サンプルは残らない。
      本スクリプトが自分でビルドするのはこのため。

    ＜DB＞
      SQL Server の Northwind を使用する。
      RerunnableBatch 系は Orders(830 件) を読み Orders2 へ INSERT するため、
      実行前に Orders2 を空にする必要がある。実行後は 830 件＝初期状態に戻る。

.PARAMETER Only
    対象名の部分一致で絞る（例: -Only "Rerunnable"）。

.PARAMETER SkipBuild
    ビルドを省略し、既存のバイナリで疎通のみ行う。

.PARAMETER OutputDir
    ログの保存先。既定は %TEMP%\OpenTouryoSmokeTest。

.EXAMPLE
    .\SmokeTest.ps1

.EXAMPLE
    .\SmokeTest.ps1 -Only "net48" -SkipBuild

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
#>
[CmdletBinding()]
param(
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

$csRoot = $PSScriptRoot
New-Item -ItemType Directory -Force $OutputDir | Out-Null

# ------------------------------------------------------------------
# 接続文字列
# ------------------------------------------------------------------
# サンプルが実際に使う App.config から読む。
# ここで別途ハードコードすると、サンプル側の変更に追随できなくなる。
function Get-SampleConnectionString
{
    $config = Join-Path $csRoot "Samples\Bat_sample\SimpleBatch_sample\App.config"
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
# 対象の定義
# ------------------------------------------------------------------
# Name    : 表示名
# Bat     : ビルドに使うバッチ（root\programs\CS 配下）
# Exe     : 実行ファイル（net48）。Dll を指定した場合は dotnet で実行する。
# Args    : コマンドライン引数
# Pre     : 実行前に行う準備（スクリプト ブロック）
# Expect  : 標準出力がこの正規表現に一致すれば成功
# Verify  : 追加の検証（スクリプト ブロック）。$true を返せば成功
#
# ※ サンプルは末尾に Console.ReadKey() を持つため、出力をリダイレクトすると
#    必ず例外で終わる。これはテスト内容とは無関係なので、判定から除外する。
$batchArgs = @("/DAP", "SQL", "/MODE1", "individual", "/MODE2", "static", "/EXROLLBACK", "-")

# RerunnableBatch 系は Orders → Orders2 の INSERT。実行前に Orders2 を空にする。
$clearOrders2 = { Invoke-Sql "DELETE FROM [Orders2]" | Out-Null }
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
        $Session
    )

    $p = @{
        Uri = $Uri; Method = $Method; WebSession = $Session
        SkipHttpErrorCheck = $true; MaximumRedirection = 0
    }
    if ($Body) { $p.Body = $Body }

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
# 単なる 200 応答ではなく、ログインを通して認証が要る画面まで到達させることで、
# ホスティング・構成・ルーティング・認証・セッションまでを一度に確認する。

# MVC_Sample (net48) : FormsAuthentication。ユーザー名が空でなければ認証される。
$mvcLoginFlow = {
    param($base)

    $ses = New-WebSession

    $r1 = Invoke-Http "$base/Home/Login" -Session $ses
    if ($r1.Status -ne 200) { return @{ Ok = $false; Detail = "GET /Home/Login = $($r1.Status)" } }

    # ValidateAntiForgeryToken のため、画面からトークンを取り出して送り返す。
    $tok = ([regex]::Match($r1.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')).Groups[1].Value
    if (-not $tok) { return @{ Ok = $false; Detail = "__RequestVerificationToken が取得できない" } }

    # 認証に成功すると FormsAuthentication がリダイレクトするため 302 になる。
    # 500 はセッション状態サービスの停止など、環境側の問題であることが多い。
    $body = @{ UserName = "smoke"; Password = "smoke"; normal = "ログイン"; __RequestVerificationToken = $tok }
    $r2 = Invoke-Http "$base/Home/Login" -Method POST -Body $body -Session $ses
    if ($r2.Status -ge 400) { return @{ Ok = $false; Detail = "POST /Home/Login = $($r2.Status)" } }

    # 認証が要る画面。未認証ならログイン画面へ 302 されるため、200 なら認証が通っている。
    $r3 = Invoke-Http "$base/Crud1/Index" -Session $ses
    if ($r3.Status -ne 200) { return @{ Ok = $false; Detail = "GET /Crud1/Index = $($r3.Status)（認証が通っていない）" } }

    return @{ Ok = $true; Detail = "ログイン後 /Crud1/Index = 200" }
}

# WebForms_Sample (net48) : 入口はログイン画面。
$webFormsFlow = {
    param($base)

    $ses = New-WebSession
    $r = Invoke-Http "$base/Aspx/start/login.aspx" -Session $ses
    if ($r.Status -ne 200) { return @{ Ok = $false; Detail = "GET login.aspx = $($r.Status)" } }
    # WebForms なので __VIEWSTATE があること＝ページのライフサイクルが動いていること。
    if ($r.Content -notmatch '__VIEWSTATE') { return @{ Ok = $false; Detail = "__VIEWSTATE が無い" } }

    return @{ Ok = $true; Detail = "login.aspx = 200（__VIEWSTATE あり）" }
}

# MVC_Sample (net10.0) : 既定ルートは Home/Index。
$mvcCoreFlow = {
    param($base)

    $ses = New-WebSession
    $r = Invoke-Http "$base/" -Session $ses
    if ($r.Status -ne 200) { return @{ Ok = $false; Detail = "GET / = $($r.Status)" } }

    return @{ Ok = $true; Detail = "GET / = 200 ($($r.Length) bytes)" }
}

$targets = @(
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

    # --- Web アプリ ---
    @{
        Name = "MVC_Sample (net48)";      Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51081
        Site = "Samples\WebApp_sample\MVC_Sample\MVC_Sample"
        Need = "aspnet_state"
        Flow = $mvcLoginFlow
    }
    @{
        Name = "WebForms_Sample (net48)"; Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51082
        Site = "Samples\WebApp_sample\WebForms_Sample\WebForms_Sample"
        Need = "aspnet_state"
        Flow = $webFormsFlow
    }
    @{
        Name = "MVC_Sample (net10.0)";    Bat = "10_Build_WebAppCore_sample.bat"
        Kind = "Web";  WebHost = "Kestrel";  Port = 51083
        Exe  = "Samples4NetCore\Backend\MVC_Sample\MVC_Sample\bin\Debug\net10.0\MVC_Sample.dll"
        Flow = $mvcCoreFlow
    }
)

# ------------------------------------------------------------------
# ビルド
# ------------------------------------------------------------------
function Invoke-BuildBat([string]$bat)
{
    $path = Join-Path $csRoot $bat
    if (-not (Test-Path $path))
    {
        Write-Host ("  ビルド バッチが見つかりません : {0}" -f $bat) -ForegroundColor Red
        return $false
    }

    $log = Join-Path $OutputDir ("build_" + ($bat -replace '[^A-Za-z0-9]','_') + ".log")

    Push-Location $csRoot
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
    if ($t.WebHost -eq "IISExpress")
    {
        $iis = Join-Path $env:ProgramFiles "IIS Express\iisexpress.exe"
        if (-not (Test-Path $iis)) { return $null }

        $site = Join-Path $csRoot $t.Site
        return Start-Process $iis `
            -ArgumentList "/path:`"$site`"", "/port:$($t.Port)", "/systray:false" `
            -PassThru -WindowStyle Hidden `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
    else
    {
        $dll = Join-Path $csRoot $t.Exe
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
$selected = $targets | Where-Object { -not $Only -or $_.Name -like "*$Only*" }

if (-not $SkipBuild)
{
    foreach ($bat in ($selected | ForEach-Object { $_.Bat } | Select-Object -Unique))
    {
        Write-Host ("=== ビルド : {0} ===" -f $bat) -ForegroundColor Cyan
        $sw = [Diagnostics.Stopwatch]::StartNew()
        [void](Invoke-BuildBat $bat)
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
    $exe = Join-Path $csRoot $t.Exe
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
    Write-Host "  実行中 ..."
    $sw = [Diagnostics.Stopwatch]::StartNew()
    Push-Location (Split-Path $exe)
    if ($exe -like "*.dll")
    {
        # 「/DAP」のように「/」で始まる引数は dotnet 自身のオプションと紛らわしいため、
        # 「--」で区切って渡す（リポジトリのビルド バッチも同じ渡し方をしている）。
        # 一方 System.CommandLine を使う CLI では「--」以降が未解析トークン扱いになり、
        # サブコマンドが認識されなくなるため、区切らずに渡す。
        $needSep = @($t.Args | Where-Object { $_ -like "/*" }).Count -gt 0
        if ($needSep) { & dotnet $exe -- @($t.Args) *>&1 | Out-File $out -Encoding UTF8 }
        else          { & dotnet $exe    @($t.Args) *>&1 | Out-File $out -Encoding UTF8 }
    }
    else
    {
        & $exe @($t.Args) *>&1 | Out-File $out -Encoding UTF8
    }
    Pop-Location
    $sw.Stop()

    $text = Get-Content $out -Raw -EA SilentlyContinue
    if ($null -eq $text) { $text = "" }

    # 判定
    # ※ 末尾の Console.ReadKey() 由来の例外はテスト内容と無関係なので除外する。
    $readKeyNoise = 'Cannot read keys when either application does not have a console'
    $fatal = ($text -split "`r?`n") | Where-Object {
        $_ -match 'Unhandled exception|ハンドルされない例外|System\.\w+Exception' -and
        $_ -notmatch $readKeyNoise
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
$results | Format-Table -AutoSize -Wrap | Out-String | Write-Host
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
