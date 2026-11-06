<#
.SYNOPSIS
    SpRp_MVC（FAPI 1.0 のクライアント）の認可コード フローを、MPAS のスタブで通す E2E（#668）。

.DESCRIPTION
    ＜何を確かめるか＞
      **SpRp_MVC の Fapi1/Start から、IdP（スタブ）のログイン・同意を通り、戻り（Fapi1/Callback）の結果が OK になるまで**を、
      ブラウザの代わりに HTTP で辿る。

      | flow | response_mode | 正常 | 対照（書き換えると NG になること） |
      |---|---|---|---|
      | oauth2（private_key_jwt） | query | access_token を得る | state を書き換える |
      | oidc（private_key_jwt） | query | ＋ id_token の検証・UserInfo | state を書き換える |
      | oidc（private_key_jwt） | query.jwt（JARM） | 同上 | 応答（JWT）の署名を書き換える / 素の応答に落とす |
      | pkce（PKCE S256） | query | 同上 | state を書き換える |
      | pkce（PKCE S256） | query.jwt（JARM） | 同上 | 応答（JWT）の署名を書き換える |

      **対照が要るのは、確かめる処理を省いた実装でも、正常の側は通ってしまうため**（CliLoginE2E.ps1 と同じ考え方）。
      **戻り（IdP → アプリ）のリダイレクトの手前で止め、URL を書き換えてからアプリへ届ける。**

    ＜前提（このスクリプトは起動しない）＞
      | 前提 | 確かめ方 |
      |---|---|
      | MPAS のスタブ | **SpRp_MVC のクライアントを登録して起動する**（..\Stub\StubCompose.ps1 -Settings ..\Stub\Sample4SpRp.stub.json） |
      | ビルド | CS\11_Build_SpRp_sample.bat（または DotNet\MVC\1_Build.bat） |
      | ポート 44336 | 空いている（SpRp_MVC は、IdP に登録した戻り先の URL で動かす） |

      **SQL Server は要らない。**

    ＜ブラウザの代わり＞
      HttpClient（リダイレクトは自分で辿る。Cookie の入れ物は 1 つ。部品は ExternalLoginE2E.ps1 と同じ作り）。
      **1 件ごとに入れ物を作り直す**（アプリのセッション = state などを、前の件から持ち越さない）。

.PARAMETER Only
    ケース名の部分一致で絞る（複数可。例 "oidc"、"query.jwt"）

.PARAMETER SkipControl
    対照のケースを省く

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。

.PARAMETER StubAppSettings
    スタブの土台にした appsettings.json（テスト利用者のパスワード TestUserPWD を読む）。
    省けば MPAS の既定（MpasRoot\root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json）。

.PARAMETER OutputDir
    アプリのログを置く場所

.EXAMPLE
    .\MvcFapi1E2E.ps1

.EXAMPLE
    .\MvcFapi1E2E.ps1 -Only "pkce" -SkipControl

.NOTES
    終了コード : 0 = すべて OK、1 = NG あり、2 = 前提が足りない

    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/07  玄人 幸道         新規（#668）
#>
[CmdletBinding()]
param(
    [string[]] $Only,

    [switch] $SkipControl,

    [string] $MpasRoot = 'C:\MultiPurposeAuthSite',

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

$appPort = 44336
$appBase = "https://localhost:$appPort"
$callback = "$appBase/Fapi1/Callback"
$appDll = Join-Path $here '..\DotNet\MVC\SpRp_MVC\bin\Debug\net10.0\SpRp_MVC.dll'

# ------------------------------------------------------------------
# ケース
# ------------------------------------------------------------------
#   Tamper : None（正常）/ State（state を書き換える）/ Signature（JARM の署名を書き換える）/ Plain（JARM を素の応答に落とす）
#   Expect : 結果の画面の NG の理由に含まれるべき文言（対照が、狙った確認で落ちたことを見る）
$cases = @(
    @{ Flow = 'oauth2'; Mode = '';          Tamper = 'None' }
    @{ Flow = 'oauth2'; Mode = '';          Tamper = 'State';     Expect = 'state が' }
    @{ Flow = 'oidc';   Mode = '';          Tamper = 'None' }
    @{ Flow = 'oidc';   Mode = '';          Tamper = 'State';     Expect = 'state が' }
    @{ Flow = 'oidc';   Mode = 'query.jwt'; Tamper = 'None' }
    @{ Flow = 'oidc';   Mode = 'query.jwt'; Tamper = 'Signature'; Expect = '応答（JARM）の検証に失敗' }
    @{ Flow = 'oidc';   Mode = 'query.jwt'; Tamper = 'Plain';     Expect = 'response がありません' }
    @{ Flow = 'pkce';   Mode = '';          Tamper = 'None' }
    @{ Flow = 'pkce';   Mode = '';          Tamper = 'State';     Expect = 'state が' }
    @{ Flow = 'pkce';   Mode = 'query.jwt'; Tamper = 'None' }
    @{ Flow = 'pkce';   Mode = 'query.jwt'; Tamper = 'Signature'; Expect = '応答（JARM）の検証に失敗' }
)

foreach ($c in $cases)
{
    $c.Name = $c.Flow + ' / ' + $(if ($c.Mode) { $c.Mode } else { 'query' })
    $c.Kind = if ($c.Tamper -eq 'None') { '正常' } else { '対照（' + $c.Tamper + '）' }
}

if ($SkipControl) { $cases = @($cases | Where-Object { $_.Tamper -eq 'None' }) }
if ($Only)
{
    $cases = @($cases | Where-Object { $c = $_; @($Only | Where-Object { $c.Name -like "*$_*" }).Count -gt 0 })
}

# ------------------------------------------------------------------
# HTTP（ブラウザの代わり。ExternalLoginE2E.ps1 と同じ作り）
# ------------------------------------------------------------------
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

function Get-Field([string]$html, [string]$name)
{
    $m = [regex]::Match($html, 'name="' + [regex]::Escape($name) + '"[^>]*value="([^"]*)"')
    if (-not $m.Success)
    {
        $m = [regex]::Match($html, 'value="([^"]*)"[^>]*name="' + [regex]::Escape($name) + '"')
    }
    if ($m.Success) { return [System.Net.WebUtility]::HtmlDecode($m.Groups[1].Value) }
    return $null
}

# ------------------------------------------------------------------
# スタブ（IdP）の画面を通し、アプリへ戻る手前で止める
# ------------------------------------------------------------------
# **スタブの中のリダイレクトだけを辿る。** アプリの戻り先へのリダイレクトが来たら、その URL を返す（届けない）。
# 戻り値 : @{ Callback = <戻り先の URL>; Error = <止まった理由> }
function Invoke-UntilCallback($client, $res, [string]$password)
{
    for ($i = 0; $i -lt 20; $i++)
    {
        if ($res.Status -ge 300 -and $res.Status -lt 400 -and $res.Location)
        {
            if ($res.Location.StartsWith($callback)) { return @{ Callback = $res.Location } }
            if (-not $res.Location.StartsWith($stubBase))
            {
                return @{ Error = "スタブからアプリ以外へリダイレクトした（$(($res.Location -split '\?')[0])）" }
            }
            $res = Send-Http $client 'GET' $res.Location $null
            continue
        }

        if ($res.Status -ne 200 -or -not $res.Url.StartsWith($stubBase))
        {
            return @{ Error = "スタブで止まった（$($res.Status) $(($res.Url -split '\?')[0])）" }
        }

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
            # ログイン画面でも同意画面でもない（エラー画面など。クライアントが登録されていないと、ここになる）
            return @{ Error = "スタブのエラー画面（$(($res.Url -split '\?')[0])。Sample4SpRp.stub.json を -Settings で渡して起動したか）" }
        }
    }
    return @{ Error = 'スタブの画面が終わらない' }
}

# ------------------------------------------------------------------
# 戻り先の URL を書き換える（対照）
# ------------------------------------------------------------------
function Get-QueryValue([string]$url, [string]$name)
{
    $m = [regex]::Match($url, '[?&]' + [regex]::Escape($name) + '=([^&#]*)')
    if ($m.Success) { return [Uri]::UnescapeDataString($m.Groups[1].Value) }
    return $null
}

function Set-QueryValue([string]$url, [string]$name, [string]$value)
{
    return [regex]::Replace($url, '([?&]' + [regex]::Escape($name) + '=)[^&#]*',
        { param($m) $m.Groups[1].Value + [Uri]::EscapeDataString($value) })
}

# base64url の 1 文字目を別の文字にする（長さと文字種は変えない）
function Edit-Base64Url([string]$s)
{
    $first = if ($s[0] -eq 'A') { 'B' } else { 'A' }
    return $first + $s.Substring(1)
}

function ConvertFrom-Base64Url([string]$s)
{
    $t = $s.Replace('-', '+').Replace('_', '/')
    switch ($t.Length % 4) { 2 { $t += '==' } 3 { $t += '=' } }
    return [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($t))
}

function Edit-Callback([string]$url, [string]$tamper)
{
    switch ($tamper)
    {
        'State'
        {
            $state = Get-QueryValue $url 'state'
            if (-not $state) { throw '戻りに state がありません' }
            return Set-QueryValue $url 'state' (Edit-Base64Url $state)
        }
        'Signature'
        {
            $jwt = Get-QueryValue $url 'response'
            if (-not $jwt) { throw '戻りに response（JARM）がありません' }
            $parts = $jwt.Split('.')
            $parts[$parts.Length - 1] = Edit-Base64Url $parts[$parts.Length - 1]
            return Set-QueryValue $url 'response' ($parts -join '.')
        }
        'Plain'
        {
            # **JARM の中身（code・state）を、署名の無い素の応答として届ける**（ダウングレード）
            $jwt = Get-QueryValue $url 'response'
            if (-not $jwt) { throw '戻りに response（JARM）がありません' }
            $payload = (ConvertFrom-Base64Url $jwt.Split('.')[1]) | ConvertFrom-Json
            return $callback + '?code=' + [Uri]::EscapeDataString($payload.code) + '&state=' + [Uri]::EscapeDataString($payload.state)
        }
    }
    return $url
}

# ------------------------------------------------------------------
# 1 件を通す（戻り値 : NG の理由。OK なら空）
# ------------------------------------------------------------------
function Invoke-Case($c, [string]$password)
{
    $client = New-Browser
    try
    {
        $start = "$appBase/Fapi1/Start?flow=$($c.Flow)&responseMode=$([Uri]::EscapeDataString($c.Mode))"
        $r = Send-Http $client 'GET' $start $null
        if ($r.Status -lt 300 -or $r.Status -ge 400 -or -not $r.Location.StartsWith("$stubBase/authorize"))
        {
            return "Start の後が $($r.Status)（$stubBase/authorize へのリダイレクトではない）"
        }

        $u = Invoke-UntilCallback $client $r $password
        if ($u.Error) { return $u.Error }

        $url = Edit-Callback $u.Callback $c.Tamper
        $res = Send-Http $client 'GET' $url $null
        if ($res.Status -ne 200) { return "戻りが $($res.Status)" }

        $m = [regex]::Match($res.Body, 'id="result" data-result="(OK|NG)">([^<]*)<')
        if (-not $m.Success) { return '結果の画面ではない' }

        $result = $m.Groups[1].Value
        $text = [System.Net.WebUtility]::HtmlDecode($m.Groups[2].Value)

        if ($c.Tamper -eq 'None')
        {
            if ($result -ne 'OK') { return $text }
            return ''
        }

        # **対照：NG になり、しかも狙った確認で落ちたこと**（別の理由で落ちたのなら、狙いは確かめられていない）
        if ($result -eq 'OK') { return "書き換えたのに OK になった（$($c.Tamper)）" }
        if (-not $text.Contains($c.Expect)) { return "NG だが、理由が違う（期待 : $($c.Expect)。実際 : $text）" }
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

# ------------------------------------------------------------------
# 前提
# ------------------------------------------------------------------
Write-Output '=== 前提 ==='
$lack = @()

$probe = New-Browser
$d = Send-Http $probe 'GET' "$stubBase/.well-known/openid-configuration" $null
$probe.Dispose()
if ($d.Status -ne 200) { $lack += "MPAS のスタブが応答しません（$stubBase。..\Stub\StubCompose.ps1 -Settings ..\Stub\Sample4SpRp.stub.json で起動する）" }

if (-not (Test-Path $appDll)) { $lack += "ビルドされていません : $appDll（CS\11_Build_SpRp_sample.bat）" }

if (Test-PortInUse $appPort) { $lack += "ポート $appPort が使用中です（Visual Studio などで SpRp_MVC を動かしていないか）" }

#   **テスト利用者のパスワードは、スタブの土台の appsettings.json の TestUserPWD**（MPAS の container\Stub\README.md 3 節）
$appSettings = $StubAppSettings
if ([string]::IsNullOrEmpty($appSettings))
{
    $appSettings = Join-Path $MpasRoot 'root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json'
}
$password = $null
if (Test-Path $appSettings)
{
    $m = [regex]::Match([IO.File]::ReadAllText($appSettings), '"TestUserPWD"\s*:\s*"([^"]*)"')
    if ($m.Success) { $password = $m.Groups[1].Value }
}
if (-not $password) { $lack += "テスト利用者のパスワードを読めません（$appSettings の TestUserPWD。-MpasRoot か -StubAppSettings で場所を指定する）" }

if ($cases.Count -eq 0) { $lack += 'ケースがありません（-Only を確かめる）' }

if ($lack.Count -gt 0)
{
    foreach ($l in $lack) { Write-Output "  NG  $l" }
    exit 2
}
Write-Output '  OK  スタブ・ビルド・ポート・テスト利用者'

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
$results = @()
$log = Join-Path $OutputDir 'app_SpRp_MVC.log'
$proc = $null

try
{
    # **コンテンツ ルートを合わせるため、出力フォルダを作業ディレクトリにする**（appsettings.json の Kestrel の節で 44336 を待ち受ける）
    $dllFull = (Resolve-Path $appDll).Path
    $proc = Start-Process 'dotnet' -ArgumentList "`"$dllFull`"" `
        -PassThru -WindowStyle Hidden -WorkingDirectory (Split-Path $dllFull) `
        -RedirectStandardOutput $log -RedirectStandardError "$log.err"

    $up = $false
    $probe = New-Browser
    try
    {
        for ($i = 0; $i -lt 60 -and -not $up; $i++)
        {
            Start-Sleep -Milliseconds 500
            $up = (Send-Http $probe 'GET' "$appBase/" $null).Status -eq 200
        }
    }
    finally { $probe.Dispose() }

    if (-not $up)
    {
        Write-Output "  NG  SpRp_MVC が起動しない（ログ : $log）"
        exit 1
    }

    Write-Output ''
    Write-Output "=== SpRp_MVC（$appBase） ==="

    foreach ($c in $cases)
    {
        try
        {
            $ng = Invoke-Case $c $password
        }
        catch
        {
            $ng = $_.Exception.Message
        }

        $ok = [string]::IsNullOrEmpty($ng)
        $detail = if (-not $ok) { $ng }
            elseif ($c.Tamper -eq 'None') { if ($c.Flow -eq 'oauth2') { 'access_token を得た' } else { 'id_token を検証し、UserInfo を得た' } }
            else { "$($c.Tamper) を書き換えると NG（$($c.Expect)…）" }

        $results += [pscustomobject]@{ ケース = $c.Name; 種別 = $c.Kind; 結果 = $(if ($ok) { 'OK' } else { 'NG' }); 内容 = $detail }
        Write-Output ("  {0,-4}{1,-16}{2,-20}{3}" -f $(if ($ok) { 'OK' } else { 'NG' }), $c.Kind, $c.Name, $detail)
    }
}
finally
{
    if ($proc -and -not $proc.HasExited)
    {
        $proc.Kill()
        $proc.WaitForExit(10000) | Out-Null
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
