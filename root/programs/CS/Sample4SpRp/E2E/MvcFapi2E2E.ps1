<#
.SYNOPSIS
    SpRp_MVC（FAPI 2.0 のクライアント）の認可コード フローを、MPAS のスタブで通す E2E（#668 の追加）。

.DESCRIPTION
    ＜何を確かめるか＞
      **SpRp_MVC の Fapi2/Start（Request Object を預ける）から、IdP（スタブ）のログイン・同意を通り、
      戻り（Fapi2/Callback）の結果が OK になるまで**を、ブラウザの代わりに HTTP で辿る。

      | flow | response_mode | 正常 | 対照（書き換えると NG になること） |
      |---|---|---|---|
      | par（PAR ＋ private_key_jwt） | query | id_token の検証・UserInfo | state を書き換える / **request_uri を使い回す** / **client_id を書き換える** |
      | par | query.jwt（JARM） | 同上 | 応答（JWT）の署名を書き換える / 素の応答に落とす |
      | ros（/ros ＋ 署名だけ） | query | 同上 | state を書き換える / **request_uri を使い回す** |
      | ros | query.jwt（JARM） | 同上 | 応答（JWT）の署名を書き換える |
      | par_mtls（PAR ＋ tls_client_auth。**-Mtls のときだけ**） | query・query.jwt | 同上 | state を書き換える |

      **par_mtls は、スタブが証明書を受け付けるときだけ通る**（..\container\Stub\StubCompose.ps1 -Mtls。MultiPurposeAuthSite#351）。
      **-Mtls を付けなければ、par_mtls のケースは作らない**（既定のスタブでは、PAR が invalid_client で断られる）。

      **対照が要るのは、確かめる処理を省いた実装でも、正常の側は通ってしまうため**（MvcFapi1E2E.ps1 と同じ考え方）。

      **FAPI2 の認可要求は client_id と request_uri だけ**（値は、預けた署名付きの Request Object の中にある）。
      - **正常の側で、認可要求に client_id・request_uri 以外のパラメタが無いことを見る**（PKCE などを素のクエリに出していない）
      - **IdP の側の対照**（アプリの戻りに届く前に、IdP が断るべきもの）
        - Reuse    : **一度使った request_uri で、もう一度認可要求する**（RFC 9126 §4。request_uri は 1 回だけ使うべき）
        - ClientId : **認可要求の client_id を、預けた Request Object と違うクライアントにする**（RFC 9126 §4・RFC 9101 §5.2）
        どちらも、**code を持った戻りがアプリに届かなければ OK**（スタブのエラー画面か、error を持った戻り）

    ＜前提（このスクリプトは起動しない）＞
      | 前提 | 確かめ方 |
      |---|---|
      | MPAS のスタブ | **SpRp_MVC のクライアントを登録して起動する**（..\container\Stub\StubCompose.ps1。既定で Sample4SpRp.stub.json を渡す） |
      | ビルド | CS\11_Build_SpRp_sample.bat（または DotNet\MVC\1_Build.bat） |
      | ポート 44336 | 空いている（SpRp_MVC は、IdP に登録した戻り先の URL で動かす） |

      **SQL Server は要らない。**

    ＜IdP を Keycloak にする（-Idp KCK）＞
      **ケースと対照は同じ**（MvcFapi1E2E.ps1 と同じく、SpRp_MVC の issuer・Discovery だけを環境変数で差し替える。Idp.ps1）。
      **Keycloak は ..\container\KCK\KckCompose.ps1 で起動する**（クライアント証明書は常に受け付けるので、-Mtls もそのまま回せる）。
      | 違い | Stub | KCK |
      |---|---|---|
      | ros | 回す | **回さない**（/ros は MPAS 独自。Keycloak の Discovery に request_object_endpoint が無い） |
      | par_mtls | StubCompose.ps1 -Mtls で起動したとき | 常に（Keycloak を https-client-auth=request で起動している） |

    ＜ブラウザの代わり＞
      HttpClient（リダイレクトは自分で辿る。Cookie の入れ物は 1 つ。部品は MvcFapi1E2E.ps1 と同じ作り）。
      **1 件ごとに入れ物を作り直す**（アプリのセッション = state などを、前の件から持ち越さない）。

.PARAMETER Only
    ケース名の部分一致で絞る（複数可。例 "par"、"query.jwt"）

.PARAMETER SkipControl
    対照のケースを省く

.PARAMETER Mtls
    par_mtls（tls_client_auth）のケースも回す。**スタブを StubCompose.ps1 -Mtls で起動しておく。**

.PARAMETER Idp
    Stub（MPAS のスタブ。既定）/ KCK（Keycloak。..\container\KCK）

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。**省けば、環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**
    （root\files\tools\PowerShell\RepoPaths\RepoPath.ps1。#652）。どれを使ったかを表示する。-Idp KCK では使わない。

.PARAMETER StubAppSettings
    スタブの土台にした appsettings.json（テスト利用者のパスワード TestUserPWD を読む）。
    省けば MPAS の既定（MpasRoot\root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json）。-Idp KCK では使わない。

.PARAMETER OutputDir
    アプリのログを置く場所

.EXAMPLE
    .\MvcFapi2E2E.ps1

.EXAMPLE
    .\MvcFapi2E2E.ps1 -Only "par" -SkipControl

.EXAMPLE
    .\MvcFapi2E2E.ps1 -Idp KCK -Mtls

.NOTES
    終了コード : 0 = すべて OK、1 = NG あり、2 = 前提が足りない

    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/09  玄人 幸道         新規（#668 の追加。MvcFapi1E2E.ps1 と同じ作り）
     2026/11/10  玄人 幸道         -Mtls（par_mtls のケース）を足した（MultiPurposeAuthSite#351）
     2026/11/13  玄人 幸道         RepoPath.ps1・SummaryTable.ps1 を root/files/tools/PowerShell の下（RepoPaths・Verify）から読む（#687）
     2026/11/13  玄人 幸道         -Idp KCK（Keycloak）を足した。IdP の違いは Idp.ps1 にまとめた（#690）
#>
[CmdletBinding()]
param(
    [string[]] $Only,

    [switch] $SkipControl,

    [switch] $Mtls,

    [ValidateSet('Stub', 'KCK')]
    [string] $Idp = 'Stub',

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
. (Join-Path $progRoot '..\files\tools\PowerShell\Verify\SummaryTable.ps1')  # #687 で移した

New-Item -ItemType Directory -Force $OutputDir | Out-Null

# 5.1（.NET Framework）は既定で TLS 1.2 を使わないことがある
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
Add-Type -AssemblyName System.Net.Http

# **IdP の違い（URL・画面・サンプルの設定の差し替え）は Idp.ps1 にまとめてある**
. (Join-Path $here 'Idp.ps1')
$idpInfo = Get-E2EIdp $Idp $here
$stubBase = $idpInfo.Base   # IdP の画面の URL の頭（KCK でも、この名前のまま使う）
$testUser = $idpInfo.User

$appPort = 44336
$appBase = "https://localhost:$appPort"
$callback = "$appBase/Fapi2/Callback"
$appDll = Join-Path $here '..\DotNet\MVC\SpRp_MVC\bin\Debug\net10.0\SpRp_MVC.dll'

# ClientId の対照で使う、別のクライアント（FAPI1 の SpRp_MVC。fapi1 の登録で、戻り先も違う）
$otherClientId = '75d74cb5a4ae44e3afec154623f0cbb4'

# ------------------------------------------------------------------
# ケース
# ------------------------------------------------------------------
#   Tamper : None（正常）/ State（戻りの state を書き換える）/ Signature（JARM の署名を書き換える）/ Plain（JARM を素の応答に落とす）
#            / Reuse（使った request_uri で、もう一度認可要求する）/ ClientId（認可要求の client_id を書き換える）
#   Expect : 結果の画面の NG の理由に含まれるべき文言（アプリが断る対照。IdP が断る対照は、code が届かないことを見る）
$cases = @(
    @{ Flow = 'par'; Mode = '';          Tamper = 'None' }
    @{ Flow = 'par'; Mode = '';          Tamper = 'State';     Expect = 'state が' }
    @{ Flow = 'par'; Mode = '';          Tamper = 'Reuse' }
    @{ Flow = 'par'; Mode = '';          Tamper = 'ClientId' }
    @{ Flow = 'par'; Mode = 'query.jwt'; Tamper = 'None' }
    @{ Flow = 'par'; Mode = 'query.jwt'; Tamper = 'Signature'; Expect = '応答（JARM）の検証に失敗' }
    @{ Flow = 'par'; Mode = 'query.jwt'; Tamper = 'Plain';     Expect = 'response がありません' }
    @{ Flow = 'ros'; Mode = '';          Tamper = 'None' }
    @{ Flow = 'ros'; Mode = '';          Tamper = 'State';     Expect = 'state が' }
    @{ Flow = 'ros'; Mode = '';          Tamper = 'Reuse' }
    @{ Flow = 'ros'; Mode = 'query.jwt'; Tamper = 'None' }
    @{ Flow = 'ros'; Mode = 'query.jwt'; Tamper = 'Signature'; Expect = '応答（JARM）の検証に失敗' }
)

if ($Mtls)
{
    $cases += @(
        @{ Flow = 'par_mtls'; Mode = '';          Tamper = 'None' }
        @{ Flow = 'par_mtls'; Mode = '';          Tamper = 'State';     Expect = 'state が' }
        @{ Flow = 'par_mtls'; Mode = 'query.jwt'; Tamper = 'None' }
    )
}

foreach ($c in $cases)
{
    $c.Name = $c.Flow + ' / ' + $(if ($c.Mode) { $c.Mode } else { 'query' })
    $c.Kind = if ($c.Tamper -eq 'None') { '正常' } else { '対照（' + $c.Tamper + '）' }
}

# **/ros は MPAS 独自**（Keycloak の Discovery に request_object_endpoint が無い）。Keycloak では回さない
$skippedRos = ($Idp -eq 'KCK' -and @($cases | Where-Object { $_.Flow -eq 'ros' }).Count -gt 0)
if ($Idp -eq 'KCK') { $cases = @($cases | Where-Object { $_.Flow -ne 'ros' }) }

if ($SkipControl) { $cases = @($cases | Where-Object { $_.Tamper -eq 'None' }) }
if ($Only)
{
    $cases = @($cases | Where-Object { $c = $_; @($Only | Where-Object { $c.Name -like "*$_*" }).Count -gt 0 })
}

# ------------------------------------------------------------------
# HTTP（ブラウザの代わり。MvcFapi1E2E.ps1 と同じ作り）
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

        if ($Idp -eq 'KCK')
        {
            # **Keycloak の画面**（ログイン・同意。Idp.ps1）。それ以外の画面は、エラー画面として止める
            $post = Get-KckFormPost $res $testUser $password 'accept'
            if (-not $post)
            {
                $msg = [regex]::Match($res.Body, 'class="[^"]*kc-feedback-text[^"]*"[^>]*>([^<]*)<')
                return @{ Error = "Keycloak のエラー画面（$(($res.Url -split '\?')[0])。$($msg.Groups[1].Value.Trim())）" }
            }
            $res = Send-Http $client 'POST' $post.Url $post.Form
        }
        elseif ($res.Body -match 'id="LoginForm"')
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
            return @{ Error = "スタブのエラー画面（$(($res.Url -split '\?')[0])。スタブを StubCompose.ps1（既定で Sample4SpRp.stub.json を渡す）で起動したか）" }
        }
    }
    return @{ Error = 'スタブの画面が終わらない' }
}

# ------------------------------------------------------------------
# URL の読み書き・戻り先の書き換え（対照）
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

function Get-QueryNames([string]$url)
{
    $q = ($url -split '\?', 2)[1]
    if (-not $q) { return @() }
    return @($q.Split('&') | ForEach-Object { [Uri]::UnescapeDataString(($_ -split '=', 2)[0]) } | Sort-Object)
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

# IdP が断るべき認可要求を送り、**code を持った戻りがアプリに届かない**ことを見る（戻り値 : NG の理由。OK なら空）
function Test-Refused($client, [string]$authorizeUrl, [string]$password, [string]$what)
{
    $r = Send-Http $client 'GET' $authorizeUrl $null
    $u = Invoke-UntilCallback $client $r $password
    if ($u.Error) { return '' }                                   # スタブのエラー画面などで止まった（断られた）
    if (Get-QueryValue $u.Callback 'code') { return "$what のに、code を持った戻りになった" }
    if (Get-QueryValue $u.Callback 'response')
    {
        # JARM で返った。中身に code が無ければ断られている
        $payload = (ConvertFrom-Base64Url (Get-QueryValue $u.Callback 'response').Split('.')[1]) | ConvertFrom-Json
        if ($payload.code) { return "$what のに、code を持った戻り（JARM）になった" }
    }
    return ''
}

# ------------------------------------------------------------------
# 1 件を通す（戻り値 : NG の理由。OK なら空）
# ------------------------------------------------------------------
function Invoke-Case($c, [string]$password)
{
    $client = New-Browser
    try
    {
        $start = "$appBase/Fapi2/Start?flow=$($c.Flow)&responseMode=$([Uri]::EscapeDataString($c.Mode))"
        $r = Send-Http $client 'GET' $start $null
        if ($r.Status -lt 300 -or $r.Status -ge 400 -or -not $r.Location.StartsWith($idpInfo.Authorize))
        {
            $m = [regex]::Match($r.Body, 'id="result" data-result="(OK|NG)">([^<]*)<')
            $why = if ($m.Success) { [System.Net.WebUtility]::HtmlDecode($m.Groups[2].Value) } else { "$($r.Status)" }
            return "Start の後が $($idpInfo.Authorize) へのリダイレクトではない（$why）"
        }

        # **認可要求は client_id と request_uri だけ**（値は、預けた署名付きの Request Object の中にある。RFC 9126 §4）
        $names = (Get-QueryNames $r.Location) -join ','
        if ($names -ne 'client_id,request_uri') { return "認可要求のパラメタが client_id・request_uri だけではない（$names）" }
        $authorizeUrl = $r.Location

        # **対照（ClientId）：認可要求の client_id を、預けた Request Object と違うクライアントにする**
        if ($c.Tamper -eq 'ClientId')
        {
            return Test-Refused $client (Set-QueryValue $authorizeUrl 'client_id' $otherClientId) $password 'client_id を書き換えた'
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

        # **対照（Reuse）：正常に使い終えた request_uri で、別のブラウザからもう一度認可要求する**（RFC 9126 §4。1 回だけ使う）
        if ($c.Tamper -eq 'Reuse')
        {
            if ($result -ne 'OK') { return "使い回す前の、1 回目が通らない（$text）" }
            $other = New-Browser
            try { return Test-Refused $other $authorizeUrl $password '使った request_uri を使い回した' }
            finally { $other.Dispose() }
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
$d = Send-Http $probe 'GET' $idpInfo.Discovery $null
$probe.Dispose()
if ($d.Status -ne 200) { $lack += "$($idpInfo.Name) が応答しません（$($idpInfo.Discovery)。$($idpInfo.StartHint)）" }

if (-not (Test-Path $appDll)) { $lack += "ビルドされていません : $appDll（CS\11_Build_SpRp_sample.bat）" }

if (Test-PortInUse $appPort) { $lack += "ポート $appPort が使用中です（Visual Studio などで SpRp_MVC を動かしていないか）" }

#   **テスト利用者のパスワードは、スタブの土台の appsettings.json の TestUserPWD**（MPAS の container\Stub\README.md 3 節）
#   **Keycloak は、realm の定義（container\KCK\realm\sprp-realm.json）の値**
$appSettings = $StubAppSettings
if ($Idp -eq 'KCK')
{
    $appSettings = $idpInfo.RealmFile
}
elseif ([string]::IsNullOrEmpty($appSettings))
{
    # **MPAS の場所は、引数 ＞ 環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**（#652）
    . (Join-Path $progRoot '..\files\tools\PowerShell\RepoPaths\RepoPath.ps1')
    $MpasRoot = Resolve-RepoPath -Variable OT_MPAS_PATH -Argument $MpasRoot -ArgumentName '-MpasRoot'
    $appSettings = $(if ($MpasRoot) { Join-Path $MpasRoot 'root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json' } else { '' })
}
$password = $null
if ($Idp -eq 'KCK')
{
    $password = Get-KckPassword $idpInfo 'tanaka'
}
elseif ($appSettings -and (Test-Path $appSettings))
{
    $m = [regex]::Match([IO.File]::ReadAllText($appSettings), '"TestUserPWD"\s*:\s*"([^"]*)"')
    if ($m.Success) { $password = $m.Groups[1].Value }
}
if (-not $password) { $lack += "テスト利用者のパスワードを読めません（$appSettings。-Idp Stub なら TestUserPWD。-MpasRoot か -StubAppSettings で場所を指定する）" }

if ($cases.Count -eq 0) { $lack += 'ケースがありません（-Only を確かめる）' }

if ($lack.Count -gt 0)
{
    foreach ($l in $lack) { Write-Output "  NG  $l" }
    exit 2
}
Write-Output "  OK  $($idpInfo.Name)（$($idpInfo.Issuer)）・ビルド・ポート・テスト利用者"
if (-not $Mtls)
{
    $hint = if ($Idp -eq 'KCK') { '-Mtls を付けたときだけ。Keycloak は常に証明書を受け付ける' } else { '-Mtls を付け、スタブを StubCompose.ps1 -Mtls で起動したときだけ' }
    Write-Output "  --  par_mtls（tls_client_auth）は回さない（$hint）"
}
if ($skippedRos) { Write-Output '  --  ros は回さない（/ros は MPAS 独自。Keycloak の Discovery に request_object_endpoint が無い）' }

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
$results = @()
$log = Join-Path $OutputDir 'app_SpRp_MVC_Fapi2.log'
$proc = $null

try
{
    # **コンテンツ ルートを合わせるため、出力フォルダを作業ディレクトリにする**（appsettings.json の Kestrel の節で 44336 を待ち受ける）
    $dllFull = (Resolve-Path $appDll).Path

    # **IdP がスタブでなければ、issuer・Discovery を環境変数で差し替える**（MvcFapi1E2E.ps1 と同じ。起動したら、このプロセスからは消す）
    foreach ($k in $idpInfo.Config.Keys) { [Environment]::SetEnvironmentVariable("appSettings__$k", $idpInfo.Config[$k]) }
    try
    {
        $proc = Start-Process 'dotnet' -ArgumentList "`"$dllFull`"" `
            -PassThru -WindowStyle Hidden -WorkingDirectory (Split-Path $dllFull) `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
    finally
    {
        foreach ($k in $idpInfo.Config.Keys) { [Environment]::SetEnvironmentVariable("appSettings__$k", $null) }
    }

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
    Write-Output "=== SpRp_MVC（$appBase）の FAPI2 ==="

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
            elseif ($c.Tamper -eq 'None') { 'id_token を検証し、UserInfo を得た' }
            elseif ($c.Tamper -in 'Reuse', 'ClientId') { "$($c.Tamper) を IdP が断った（code が届かない）" }
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
