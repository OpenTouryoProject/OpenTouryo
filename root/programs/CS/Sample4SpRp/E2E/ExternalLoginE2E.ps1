<#
.SYNOPSIS
    WebApp のサンプル（C#・VB の MVC・WebForms）の外部ログイン（OIDC・SAML2）を、MPAS のスタブで通す E2E（#649）。

.DESCRIPTION
    ＜何を確かめるか＞
      **外部ログインのボタンを押してから、ログインして認証が要る画面に届くまで**を、ブラウザの代わりに HTTP で辿る。
        1. アプリのログイン画面で、外部ログインのボタンを押す（OIDC : external / btnButton2、SAML2 : saml2 / btnButton3）
        2. スタブへのリダイレクトを確かめる（OIDC : /authorize、SAML2 : /saml2request）
        3. スタブのログイン画面・同意画面を、テスト利用者で通す
        4. アプリの戻り先（OIDC : OAuth2AuthorizationCodeGrantClient、SAML2 : Saml2AssertionConsumerService）へ戻り、
           **認証が要る画面が 200 を返す**ことを確かめる（MVC : /Crud1/Index、WebForms : Aspx/start/menu.aspx）

      **疎通テスト（3_SmokeTest.ps1）はフォーム ログインしか見ない。** 外部ログインはここで見る。

    ＜前提（このスクリプトは起動しない）＞
      | 前提 | 確かめ方 |
      |---|---|
      | MPAS のスタブ | https://localhost:44310 の Discovery が 200（..\README.md の 2 節。StubCompose.ps1） |
      | SQL Server（Northwind） | localhost:1433 に繋がる（MVC の /Crud1/Index が DB を読む） |
      | ASP.NET 状態サービス | aspnet_state が動いている（WebForms のセッション） |
      | ビルド | 1_BuildAll.ps1（C#・VB の WebApp_sample、WebAppCore_sample） |
      | SAML2 | **MPAS の雛形のクライアントに redirect_uri_saml が要る**（無いと IdP は応答を返さない） |

    ＜アプリの待ち受け＞
      **戻り先は MPAS の雛形に登録された URL なので、その URL で動かす**（MVC 44334、WebForms 44333。どれも https）。
      net48 は IIS Express（生成した applicationhost.config）、net10.0 は Kestrel。**1 つずつ起動して止める。**
      **そのポートを他のプロセス（Visual Studio など）が使っていれば、その対象は NG にする**（止めはしない）。

    ＜ブラウザの代わり＞
      HttpClient（リダイレクトは自分で辿る。Cookie の入れ物は 1 つ）。
      **スタブとアプリは同じ localhost なので、Cookie はポートを問わず共有される**（ブラウザと同じ）。

.PARAMETER Lang
    CS / VB / Both（既定）

.PARAMETER Flow
    OIDC / SAML2 / Both（既定）

.PARAMETER Only
    対象名の部分一致で絞る（複数可）

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。**省けば、環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**
    （root\files\tools\PowerShell\RepoPath.ps1。#652）。どれを使ったかを表示する。

.PARAMETER StubAppSettings
    スタブの土台にした appsettings.json（テスト利用者のパスワード TestUserPWD を読む）。
    省けば MPAS の既定（MpasRoot\root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json）。

.PARAMETER OutputDir
    アプリのログと、生成した IIS Express の構成を置く場所

.EXAMPLE
    .\ExternalLoginE2E.ps1

.EXAMPLE
    .\ExternalLoginE2E.ps1 -Lang CS -Flow OIDC -Only "net10.0"

.NOTES
    終了コード : 0 = すべて OK、1 = NG あり、2 = 前提が足りない

    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/04  玄人 幸道         新規（#649）
     2026/11/07  玄人 幸道         MPAS の場所を、引数 ＞ 環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定 で決める（#652）
#>
[CmdletBinding()]
param(
    [ValidateSet('CS', 'VB', 'Both')]
    [string] $Lang = 'Both',

    [ValidateSet('OIDC', 'SAML2', 'Both')]
    [string] $Flow = 'Both',

    [string[]] $Only,

    [string] $MpasRoot = '',

    [string] $StubAppSettings = '',

    [string] $OutputDir = (Join-Path $env:TEMP 'OpenTouryoE2E')
)

$ErrorActionPreference = 'Stop'

# コンソールの出力を UTF-8 に（CODING.md 5 節）
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
$here = Split-Path -Parent $PSCommandPath
$progRoot = (Resolve-Path (Join-Path $here '..\..\..')).Path   # root\programs（E2E → Sample4SpRp → CS → programs）
. (Join-Path $progRoot 'SummaryTable.ps1')

New-Item -ItemType Directory -Force $OutputDir | Out-Null

# 5.1（.NET Framework）は既定で TLS 1.2 を使わないことがある
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
Add-Type -AssemblyName System.Net.Http

$stubBase = 'https://localhost:44310'
$testUser = 'tanaka@example.com'

# ------------------------------------------------------------------
# 対象
# ------------------------------------------------------------------
$mvcCs   = Join-Path $progRoot 'CS\Samples\WebApp_sample\MVC_Sample\MVC_Sample'
$wfCs    = Join-Path $progRoot 'CS\Samples\WebApp_sample\WebForms_Sample\WebForms_Sample'
$mvcCore = Join-Path $progRoot 'CS\Samples4NetCore\Backend\MVC_Sample\MVC_Sample\bin\Debug\net10.0\MVC_Sample.dll'
$mvcVb   = Join-Path $progRoot 'VB\Samples\WebApp_sample\MVC_Sample\MVC_Sample'
$wfVb    = Join-Path $progRoot 'VB\Samples\WebApp_sample\WebForms_Sample\WebForms_Sample'

$targets = @(
    @{ Name = 'MVC_Sample (net48)';          Lang = 'CS'; Kind = 'MVC';      Host = 'IISExpress'; Path = $mvcCs;   Port = 44334 }
    @{ Name = 'MVC_Sample (net10.0)';        Lang = 'CS'; Kind = 'MVC';      Host = 'Kestrel';    Path = $mvcCore; Port = 44334 }
    @{ Name = 'WebForms_Sample (net48)';     Lang = 'CS'; Kind = 'WebForms'; Host = 'IISExpress'; Path = $wfCs;    Port = 44333 }
    @{ Name = 'MVC_Sample (VB net48)';       Lang = 'VB'; Kind = 'MVC';      Host = 'IISExpress'; Path = $mvcVb;   Port = 44334 }
    @{ Name = 'WebForms_Sample (VB net48)';  Lang = 'VB'; Kind = 'WebForms'; Host = 'IISExpress'; Path = $wfVb;    Port = 44333 }
)

$targets = @($targets | Where-Object { $Lang -eq 'Both' -or $_.Lang -eq $Lang })
if ($Only)
{
    $targets = @($targets | Where-Object { $t = $_; @($Only | Where-Object { $t.Name -like "*$_*" }).Count -gt 0 })
}

$flows = @(if ($Flow -eq 'Both') { 'OIDC', 'SAML2' } else { $Flow })

# ------------------------------------------------------------------
# HTTP（ブラウザの代わり）
# ------------------------------------------------------------------
# **リダイレクトは自分で辿る**（どこへ飛んだかを確かめるため）。**Cookie の入れ物は 1 つ**（ブラウザと同じ）。
function New-Browser
{
    $handler = New-Object System.Net.Http.HttpClientHandler
    $handler.AllowAutoRedirect = $false
    $handler.UseCookies = $true
    $handler.CookieContainer = New-Object System.Net.CookieContainer

    $client = New-Object System.Net.Http.HttpClient($handler)
    $client.Timeout = [TimeSpan]::FromSeconds(120)
    return $client
}

function Send-Http($client, [string]$method, [string]$url, [hashtable]$form)
{
    $req = New-Object System.Net.Http.HttpRequestMessage((New-Object System.Net.Http.HttpMethod($method)), $url)

    if ($form)
    {
        $dic = New-Object 'System.Collections.Generic.Dictionary[string,string]'
        foreach ($k in $form.Keys) { $dic[$k] = [string]$form[$k] }
        $req.Content = New-Object System.Net.Http.FormUrlEncodedContent($dic)
    }

    try
    {
        $res = $client.SendAsync($req).GetAwaiter().GetResult()
    }
    catch
    {
        return [pscustomobject]@{ Status = -1; Url = $url; Location = ''; Body = $_.Exception.GetBaseException().Message }
    }

    $location = ''
    if ($res.Headers.Location)
    {
        $location = (New-Object Uri((New-Object Uri($url)), $res.Headers.Location)).AbsoluteUri
    }

    return [pscustomobject]@{
        Status   = [int]$res.StatusCode
        Url      = $url
        Location = $location
        Body     = $res.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    }
}

# 3xx を辿る（GET で）
function Invoke-Follow($client, $res)
{
    for ($i = 0; $i -lt 15 -and $res.Status -ge 300 -and $res.Status -lt 400 -and $res.Location; $i++)
    {
        $res = Send-Http $client 'GET' $res.Location $null
    }
    return $res
}

function Get-Field([string]$html, [string]$name)
{
    $m = [regex]::Match($html, 'name="' + [regex]::Escape($name) + '"[^>]*value="([^"]*)"')
    if (-not $m.Success)
    {
        # 属性の順が逆（value が先）の書き方
        $m = [regex]::Match($html, 'value="([^"]*)"[^>]*name="' + [regex]::Escape($name) + '"')
    }
    if ($m.Success) { return [System.Net.WebUtility]::HtmlDecode($m.Groups[1].Value) }
    return $null
}

# ------------------------------------------------------------------
# スタブ（IdP）の画面を通す
# ------------------------------------------------------------------
# **ログイン画面 → 同意画面**の順に出る（同意は、覚えていれば出ない）。出たものだけを通す。
function Invoke-IdpScreens($client, $res, [string]$password)
{
    for ($i = 0; $i -lt 6; $i++)
    {
        $res = Invoke-Follow $client $res

        if ($res.Status -ne 200 -or -not $res.Url.StartsWith($stubBase)) { return $res }

        if ($res.Body -match 'id="LoginForm"')
        {
            $res = Send-Http $client 'POST' "$stubBase/Account/Login" @{
                ReturnUrl = (Get-Field $res.Body 'ReturnUrl')
                Email = $testUser; Password = $password
                RememberMe = 'false'; SequenceNo = '0'; Fido2Data = ''; IdpKey = ''
                submitButtonName = 'normal_signin'; submitButton = 'normal_signin'
                __RequestVerificationToken = (Get-Field $res.Body '__RequestVerificationToken')
            }
        }
        elseif ($res.Body -match 'name="submit.Grant"')
        {
            $res = Send-Http $client 'POST' $res.Url @{
                'submit.Grant' = 'Grant'
                __RequestVerificationToken = (Get-Field $res.Body '__RequestVerificationToken')
            }
        }
        else
        {
            # スタブのログイン画面でも同意画面でもない（エラー画面など）
            return $res
        }
    }
    return $res
}

# ------------------------------------------------------------------
# アプリ側：外部ログインのボタンを押す
# ------------------------------------------------------------------
function Invoke-ExternalLoginButton($client, $t, [string]$base, [string]$flow)
{
    if ($t.Kind -eq 'MVC')
    {
        $r = Send-Http $client 'GET' "$base/Home/Login" $null
        if ($r.Status -ne 200) { return $r }

        $button = if ($flow -eq 'OIDC') { 'external' } else { 'saml2' }
        return Send-Http $client 'POST' "$base/Home/Login" @{
            UserName = ''; Password = ''; $button = 'x'
            __RequestVerificationToken = (Get-Field $r.Body '__RequestVerificationToken')
        }
    }
    else
    {
        $r = Send-Http $client 'GET' "$base/Aspx/start/login.aspx" $null
        if ($r.Status -ne 200) { return $r }

        # マスタ ページ配下のため、コントロール名に ctl00$ContentPlaceHolder_A$ が付く（st_Flow.ps1 と同じ）
        $button = if ($flow -eq 'OIDC') { 'btnButton2' } else { 'btnButton3' }
        $form = @{
            'ctl00$ContentPlaceHolder_A$txtUserID' = ''
            'ctl00$ContentPlaceHolder_A$txtPassword' = ''
        }
        $form['ctl00$ContentPlaceHolder_A$' + $button] = 'x'
        foreach ($n in @('__VIEWSTATE', '__VIEWSTATEGENERATOR', '__EVENTVALIDATION'))
        {
            $v = Get-Field $r.Body $n
            if ($null -ne $v) { $form[$n] = $v }
        }
        return Send-Http $client 'POST' "$base/Aspx/start/login.aspx" $form
    }
}

# ------------------------------------------------------------------
# 1 件を通す
# ------------------------------------------------------------------
function Invoke-Case($t, [string]$flow, [string]$password)
{
    $base = "https://localhost:$($t.Port)"
    $client = New-Browser

    try
    {
        # 0. **対照：ログインの前は、認証が要る画面に届かない**（届くなら、この E2E は何も確かめていない）
        $protected = if ($t.Kind -eq 'MVC') { "$base/Crud1/Index" } else { "$base/Aspx/start/menu.aspx" }
        $before = Send-Http $client 'GET' $protected $null
        if ($before.Status -eq 200)
        {
            return "ログインの前から $protected = 200（認証が効いていない）"
        }

        # 1. 外部ログインのボタン → スタブへリダイレクトするか
        $r = Invoke-ExternalLoginButton $client $t $base $flow
        $expected = if ($flow -eq 'OIDC') { "$stubBase/authorize" } else { "$stubBase/saml2request" }

        if ($r.Status -lt 300 -or $r.Status -ge 400 -or -not $r.Location.StartsWith($expected))
        {
            return "ボタンの後が $($r.Status)（$expected へのリダイレクトではない。$(($r.Location -split '\?')[0])）"
        }

        # 2. スタブの画面を通し、アプリへ戻る
        $r = Invoke-IdpScreens $client $r $password
        if ($r.Url.StartsWith($stubBase))
        {
            $hint = if ($flow -eq 'SAML2') { '。MPAS の雛形の redirect_uri_saml は在るか' } else { '' }
            # クエリ（SAMLRequest など）は長いので、パスまでを出す
            return "スタブから戻らない（$($r.Status) $(($r.Url -split '\?')[0])$hint）"
        }

        # 3. 認証が要る画面に届くか（未認証ならログイン画面へ 302 される）
        $p = Send-Http $client 'GET' $protected $null
        if ($p.Status -ne 200)
        {
            return "ログインできていない（戻った先 $($r.Status) $(($r.Url -split '\?')[0]) → $protected = $($p.Status) $(($p.Location -split '\?')[0])）"
        }

        return ''
    }
    finally
    {
        $client.Dispose()
    }
}

# ------------------------------------------------------------------
# アプリの起動と停止
# ------------------------------------------------------------------
function Test-PortInUse([int]$port)
{
    try
    {
        $c = New-Object System.Net.Sockets.TcpClient
        $c.Connect('localhost', $port)
        $c.Close()
        return $true
    }
    catch { return $false }
}

function Start-App($t)
{
    $safe = ($t.Name -replace '[^A-Za-z0-9.]', '_')
    $log = Join-Path $OutputDir "app_$safe.log"

    if ($t.Host -eq 'Kestrel')
    {
        if (-not (Test-Path $t.Path)) { throw "ビルドされていません : $($t.Path)" }

        # コンテンツ ルートを合わせるため、出力フォルダを作業ディレクトリにする（st_Server.ps1 と同じ）
        return Start-Process 'dotnet' -ArgumentList "`"$($t.Path)`"", '--urls', "https://localhost:$($t.Port)" `
            -PassThru -WindowStyle Hidden -WorkingDirectory (Split-Path $t.Path) `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }

    $iis = Join-Path $env:ProgramFiles 'IIS Express\iisexpress.exe'
    $template = Join-Path $env:ProgramFiles 'IIS Express\config\templates\PersonalWebServer\applicationhost.config'
    if (-not (Test-Path $iis) -or -not (Test-Path $template)) { throw 'IIS Express が見つかりません。' }
    if (-not (Test-Path (Join-Path $t.Path 'bin'))) { throw "ビルドされていません : $($t.Path)" }

    # **https で待ち受けるには構成ファイルが要る**（/path:・/port: は http だけ）。
    #   雛形の既定のサイトを、この対象のサイトに差し替える。証明書は IIS Express の開発用（44300〜44399 に登録済み）。
    $site = '<site name="E2E" id="1" serverAutoStart="true">' +
        '<application path="/" applicationPool="Clr4IntegratedAppPool">' +
        '<virtualDirectory path="/" physicalPath="' + $t.Path + '" /></application>' +
        '<bindings><binding protocol="https" bindingInformation="*:' + $t.Port + ':localhost" /></bindings></site>'
    $xml = [IO.File]::ReadAllText($template)
    $xml = [regex]::Replace($xml, '<site name="WebSite1"[\s\S]*?</site>', $site.Replace('$', '$$'))
    $config = Join-Path $OutputDir "applicationhost_$safe.config"
    [IO.File]::WriteAllText($config, $xml, (New-Object Text.UTF8Encoding $false))

    return Start-Process $iis -ArgumentList "/config:`"$config`"", '/site:E2E', '/systray:false' `
        -PassThru -WindowStyle Hidden -RedirectStandardOutput $log -RedirectStandardError "$log.err"
}

function Wait-App($t)
{
    # **TCP で繋がるか**を待ち、**最初の要求（コンパイル）**が返るまで待つ
    for ($i = 0; $i -lt 60 -and -not (Test-PortInUse $t.Port); $i++) { Start-Sleep -Milliseconds 500 }
    if (-not (Test-PortInUse $t.Port)) { return $false }

    $path = if ($t.Kind -eq 'MVC') { '/Home/Login' } else { '/Aspx/start/login.aspx' }
    $client = New-Browser
    try
    {
        for ($i = 0; $i -lt 10; $i++)
        {
            $r = Send-Http $client 'GET' "https://localhost:$($t.Port)$path" $null
            if ($r.Status -eq 200) { return $true }
            Start-Sleep -Seconds 2
        }
    }
    finally { $client.Dispose() }
    return $false
}

function Stop-App($proc)
{
    if ($proc -and -not $proc.HasExited)
    {
        $proc.Kill()
        $proc.WaitForExit(10000) | Out-Null
    }
}

# ------------------------------------------------------------------
# 前提
# ------------------------------------------------------------------
Write-Output '=== 前提 ==='
$lack = @()

$probe = New-Browser
$d = Send-Http $probe 'GET' "$stubBase/.well-known/openid-configuration" $null
$probe.Dispose()
if ($d.Status -ne 200) { $lack += "MPAS のスタブが応答しません（$stubBase。..\README.md の 2 節の StubCompose.ps1 で起動する）" }

if (-not (Test-PortInUse 1433)) { $lack += 'SQL Server（localhost:1433）に繋がりません（LocalServicesOnDocker の Start-Services.ps1 sqlserver）' }

if (@($targets | Where-Object { $_.Kind -eq 'WebForms' }).Count -gt 0)
{
    $svc = Get-Service aspnet_state -ErrorAction SilentlyContinue
    if (-not $svc -or $svc.Status -ne 'Running') { $lack += 'ASP.NET 状態サービス（aspnet_state）が動いていません' }
}

#   **テスト利用者のパスワードは、スタブの土台の appsettings.json の TestUserPWD**（MPAS の container\Stub\README.md 3 節）
$appSettings = $StubAppSettings
if ([string]::IsNullOrEmpty($appSettings))
{
    # **MPAS の場所は、引数 ＞ 環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**（#652）
    . (Join-Path $progRoot '..\files\tools\PowerShell\RepoPath.ps1')
    $MpasRoot = Resolve-RepoPath -Variable OT_MPAS_PATH -Argument $MpasRoot -ArgumentName '-MpasRoot'
    $appSettings = $(if ($MpasRoot) { Join-Path $MpasRoot 'root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json' } else { '' })
}
$password = $null
if (Test-Path $appSettings)
{
    $m = [regex]::Match([IO.File]::ReadAllText($appSettings), '"TestUserPWD"\s*:\s*"([^"]*)"')
    if ($m.Success) { $password = $m.Groups[1].Value }
}
if (-not $password) { $lack += "テスト利用者のパスワードを読めません（$appSettings の TestUserPWD。-MpasRoot か -StubAppSettings で場所を指定する）" }

if ($targets.Count -eq 0) { $lack += '対象がありません（-Lang / -Only を確かめる）' }

if ($lack.Count -gt 0)
{
    foreach ($l in $lack) { Write-Output "  NG  $l" }
    exit 2
}
Write-Output '  OK  スタブ・SQL Server・状態サービス・テスト利用者'

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
$results = @()

foreach ($t in $targets)
{
    Write-Output ''
    Write-Output "=== $($t.Name)（https://localhost:$($t.Port)） ==="

    if (Test-PortInUse $t.Port)
    {
        # **他のプロセスが使っている**（Visual Studio で動かしている、など）。止めはしない。
        foreach ($f in $flows)
        {
            $results += [pscustomobject]@{ 対象 = $t.Name; フロー = $f; 結果 = 'NG'; 内容 = "ポート $($t.Port) が使用中（他で動かしていないか）" }
        }
        Write-Output "  NG  ポート $($t.Port) が使用中"
        continue
    }

    $proc = $null
    try
    {
        $proc = Start-App $t
        if (-not (Wait-App $t))
        {
            foreach ($f in $flows)
            {
                $results += [pscustomobject]@{ 対象 = $t.Name; フロー = $f; 結果 = 'NG'; 内容 = "起動しない（ログ : $OutputDir）" }
            }
            Write-Output '  NG  起動しない'
            continue
        }

        foreach ($f in $flows)
        {
            $ng = Invoke-Case $t $f $password
            $ok = [string]::IsNullOrEmpty($ng)
            $detail = if ($ok) { 'ログイン後、認証が要る画面 = 200' } else { $ng }
            $results += [pscustomobject]@{ 対象 = $t.Name; フロー = $f; 結果 = $(if ($ok) { 'OK' } else { 'NG' }); 内容 = $detail }
            Write-Output ("  {0,-4}{1,-6}{2}" -f $(if ($ok) { 'OK' } else { 'NG' }), $f, $detail)
        }
    }
    catch
    {
        foreach ($f in $flows)
        {
            $results += [pscustomobject]@{ 対象 = $t.Name; フロー = $f; 結果 = 'NG'; 内容 = $_.Exception.Message }
        }
        Write-Output "  NG  $($_.Exception.Message)"
    }
    finally
    {
        Stop-App $proc
    }
}

Write-Output ''
Write-Output '================ サマリ ================'
Write-Output ''
Write-SummaryTable $results

$ngCount = @($results | Where-Object { $_.結果 -ne 'OK' }).Count
Write-Output ''
if ($ngCount -eq 0)
{
    Write-Output "  すべて OK（$($results.Count) 件）"
    exit 0
}
Write-Output "  NG $ngCount 件 / $($results.Count) 件（ログ : $OutputDir）"
exit 1
