<#
.SYNOPSIS
    CLI のサンプル（SpRp_CLI と、旧サンプルの DAG_Login_CLI・LIR_Login_CLI）のログインを、MPAS のスタブで通す E2E（#649）。

.DESCRIPTION
    ＜何を確かめるか＞
      **CLI を起動し、表示された URL をブラウザの代わりに HTTP で辿って、CLI が成功で終わるまで**を見る。

      | フロー | 辿り方 |
      |---|---|
      | device | 表示された verification_uri_complete を開き、ログインして承認（Allow）する。CLI はポーリングでトークンを得る |
      | pkce   | 表示された認可要求の URL を開き、ログイン・同意する。戻り（http://localhost:12345/?code=…）を CLI の待ち受けへ届ける |

      **成功の判定**：終了コード 0、id_token を検証した旨の行、UserInfo を得た旨の行（SpRp_CLI : 「OK   UserInfo」、旧 : 「NORMAL_END」）。

      **ブラウザを使わないグラント**（SpRp_CLI の cc・jwtbearer・ropc。#668）は、CLI を起動して終わるのを待つだけである。
      **成功の判定**：終了コード 0、access_token を得た旨の行（ropc は、さらに id_token を検証し UserInfo を得た旨の行）。

      **対照（確かめる処理が効いているか）**：これが無いと、検証を省いた実装でも通ってしまう。
      | フロー | すること | 期待 |
      |---|---|---|
      | device | 承認せずに拒否（Deny）する | 終了コード 1、access_denied |
      | pkce   | 戻りの state を書き換えて届ける | 終了コード 1、state の不一致 |
      | cc     | 秘密を書き換えた設定で動かす | 終了コード 1、invalid_client |
      | jwtbearer | 別の鍵（SHA256RSA_Server.pfx）で署名させる | 終了コード 1、invalid_client |
      | ropc   | 違うパスワードを入れる | 終了コード 1、access_denied |

      **cc・jwtbearer の対照は、出力フォルダを -OutputDir に写し、設定ファイルだけを書き換えて動かす**（ビルドの成果物は変えない）。
      **対照は正常の後に回す**（MPAS は、クライアント認証と利用者のパスワードの「連続した」失敗を数え、成功で消す。#305）。

    ＜CLI の起動のしかた＞
      **ブラウザを開かせない・対話をさせない**ために、サンプルのオプションを使う。
        SpRp_CLI      : device / pkce --no-browser
        DAG_Login_CLI : login --yes（Sharprompt の確認を飛ばす）
        LIR_Login_CLI : login --no-browser

    ＜前提（このスクリプトは起動しない）＞
      | 前提 | 確かめ方 |
      |---|---|
      | MPAS のスタブ | https://localhost:44310 の Discovery が 200（..\README.md の 2 節。StubCompose.ps1） |
      | ビルド | SpRp_CLI : CS\11_Build_SpRp_sample.bat、旧 : CS\5_Build_CLICore_sample.bat |
      | ポート 12345 | 空いている（pkce の戻り先。**どの CLI も同じポートを使うので、1 つずつ動かす**） |
      | cc・jwtbearer・ropc | **スタブに ..\Stub\Sample4SpRp.stub.json が要る**（StubCompose.ps1 の既定）（SpRp_CLI_Batch のクライアントと、ROPC を開ける設定） |

      SQL Server は要らない。
      **ropc の前に、スタブのログイン画面を一度開く**（MPAS は、アカウントの画面を初めて開いたときにテスト利用者を作る）。

.PARAMETER Flow
    device / pkce / cc / jwtbearer / ropc / All（既定）。Both は All と同じ（以前の名前）

.PARAMETER Only
    対象名の部分一致で絞る（複数可）

.PARAMETER SkipControl
    対照（拒否・state の書き換え）を回さない

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。**省けば、環境変数 MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**
    （root\files\tools\PowerShell\RepoPath.ps1。#652）。どれを使ったかを表示する。

.PARAMETER StubAppSettings
    スタブの土台にした appsettings.json（テスト利用者のパスワード TestUserPWD を読む）。
    省けば MPAS の既定（MpasRoot\root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json）。

.PARAMETER OutputDir
    CLI の出力（1 件ごとのログ）を置く場所

.EXAMPLE
    .\CliLoginE2E.ps1

.EXAMPLE
    .\CliLoginE2E.ps1 -Flow pkce -Only "net48"

.NOTES
    終了コード : 0 = すべて OK、1 = NG あり、2 = 前提が足りない

    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/05  玄人 幸道         新規（#649）
     2026/11/07  玄人 幸道         SpRp_CLI の cc・jwtbearer・ropc を足した（#668）
     2026/11/07  玄人 幸道         MPAS の場所を、引数 ＞ 環境変数 MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定 で決める（#652）
#>
[CmdletBinding()]
param(
    [ValidateSet('device', 'pkce', 'cc', 'jwtbearer', 'ropc', 'All', 'Both')]
    [string] $Flow = 'All',

    [string[]] $Only,

    [switch] $SkipControl,

    [string] $MpasRoot = '',

    [string] $StubAppSettings = '',

    [string] $OutputDir = (Join-Path $env:TEMP 'OpenTouryoE2E')
)

$ErrorActionPreference = 'Stop'

# コンソールの出力を UTF-8 に（CODING.md 5 節）。
#   **CLI は、このコンソールを引き継ぐ**（下の Start-Cli）。ここで UTF-8 にしておくと、CLI の出力も UTF-8 で読める。
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# **CLI の標準入力に BOM を書かせない**（ropc のパスワード）。
#   5.1（.NET Framework）は、子プロセスの標準入力を [Console]::InputEncoding で開き、**起動した時点で BOM を書く**。
#   net10.0 の CLI は、それをパスワードの先頭の 1 文字として読む（net48 の CLI は読み飛ばす）。
[Console]::InputEncoding = New-Object Text.UTF8Encoding $false

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

# **pkce の戻り先**（MPAS の雛形の Device_Client の redirect_uri_code。各 CLI の SpRp_RedirectUri と同じ）
$redirectUri = 'http://localhost:12345/'

# **ropc の利用者は、利用者名**（ログイン画面で入れるメール アドレスではない）
$ropcUser = 'tanaka'

# ------------------------------------------------------------------
# 対象
# ------------------------------------------------------------------
$sprp = Join-Path $progRoot 'CS\Sample4SpRp\DotNet\CLI\SpRp_CLI\bin\Debug'
$legacy = Join-Path $progRoot 'CS\Samples4NetCore\Legacy\CLI_sample'
$dag = Join-Path $legacy 'DAG_Login_CLI\DAG_Login_CLI\bin\Debug\net10.0\DAG_Login_CLI.dll'
$lir = Join-Path $legacy 'LIR_Login_CLI\LIR_Login_CLI\bin\Debug\net10.0\LIR_Login_CLI.dll'

# Exe : 実行ファイル（.dll なら dotnet で動かす）
$targets = @(
    @{ Name = 'SpRp_CLI (net48)';        Flow = 'device'; Exe = "$sprp\net48\SpRp_CLI.exe";   Args = 'device' }
    @{ Name = 'SpRp_CLI (net48)';        Flow = 'pkce';   Exe = "$sprp\net48\SpRp_CLI.exe";   Args = 'pkce --no-browser' }
    @{ Name = 'SpRp_CLI (net10.0)';      Flow = 'device'; Exe = "$sprp\net10.0\SpRp_CLI.dll"; Args = 'device' }
    @{ Name = 'SpRp_CLI (net10.0)';      Flow = 'pkce';   Exe = "$sprp\net10.0\SpRp_CLI.dll"; Args = 'pkce --no-browser' }
    @{ Name = 'DAG_Login_CLI (net10.0)'; Flow = 'device'; Exe = $dag;                          Args = 'login --yes' }
    @{ Name = 'LIR_Login_CLI (net10.0)'; Flow = 'pkce';   Exe = $lir;                          Args = 'login --no-browser' }
    @{ Name = 'SpRp_CLI (net48)';        Flow = 'cc';        Exe = "$sprp\net48\SpRp_CLI.exe";   Args = 'cc' }
    @{ Name = 'SpRp_CLI (net48)';        Flow = 'jwtbearer'; Exe = "$sprp\net48\SpRp_CLI.exe";   Args = 'jwtbearer' }
    @{ Name = 'SpRp_CLI (net48)';        Flow = 'ropc';      Exe = "$sprp\net48\SpRp_CLI.exe";   Args = "ropc $ropcUser" }
    @{ Name = 'SpRp_CLI (net10.0)';      Flow = 'cc';        Exe = "$sprp\net10.0\SpRp_CLI.dll"; Args = 'cc' }
    @{ Name = 'SpRp_CLI (net10.0)';      Flow = 'jwtbearer'; Exe = "$sprp\net10.0\SpRp_CLI.dll"; Args = 'jwtbearer' }
    @{ Name = 'SpRp_CLI (net10.0)';      Flow = 'ropc';      Exe = "$sprp\net10.0\SpRp_CLI.dll"; Args = "ropc $ropcUser" }
)

$grants = @('cc', 'jwtbearer', 'ropc')

$targets = @($targets | Where-Object { $Flow -eq 'All' -or $Flow -eq 'Both' -or $_.Flow -eq $Flow })
if ($Only)
{
    $targets = @($targets | Where-Object { $t = $_; @($Only | Where-Object { $t.Name -like "*$_*" }).Count -gt 0 })
}

$cases = @('正常')
if (-not $SkipControl) { $cases += '対照' }

# ------------------------------------------------------------------
# HTTP（ブラウザの代わり）。ExternalLoginE2E.ps1 と同じ作り
# ------------------------------------------------------------------
# **リダイレクトは自分で辿る**（どこへ飛んだかを確かめ、CLI の戻り先の手前で止めるため）。**Cookie の入れ物は 1 つ。**
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

# 3xx を辿る（GET で）。**CLI の戻り先（$redirectUri）へのリダイレクトは辿らない**（届け方を、呼ぶ側が決める）
function Invoke-Follow($client, $res)
{
    for ($i = 0; $i -lt 15 -and $res.Status -ge 300 -and $res.Status -lt 400 -and $res.Location; $i++)
    {
        if ($res.Location.StartsWith($redirectUri)) { return $res }
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

# **ログイン画面 → 同意画面**の順に出る（同意は、覚えていれば出ない）。出たものだけを通す。
#   それ以外の画面（Device AuthZ の確認画面、エラー画面）や、スタブの外へのリダイレクトで戻る。
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
            return $res
        }
    }
    return $res
}

# ------------------------------------------------------------------
# CLI の起動と、出力の読み取り
# ------------------------------------------------------------------
# **このコンソールを引き継がせる**（UseShellExecute = false・CreateNoWindow = false）。
#   Start-Process -WindowStyle Hidden だと新しいコンソール（既定のコード ページ）になり、日本語が化ける。
#   $exe   : 動かす実行ファイル（省けば $t.Exe。対照で、設定を書き換えた写しを動かすとき）
#   $stdin : 標準入力に渡す 1 行（ropc のパスワード。省けば何も渡さない）
function Start-Cli($t, [string]$exe = '', [string]$stdin = $null)
{
    if (-not $exe) { $exe = $t.Exe }

    $psi = New-Object System.Diagnostics.ProcessStartInfo
    if ($exe.EndsWith('.dll'))
    {
        $psi.FileName = 'dotnet'
        $psi.Arguments = "`"$exe`" $($t.Args)"
    }
    else
    {
        $psi.FileName = $exe
        $psi.Arguments = $t.Args
    }
    # **設定ファイルは実行ファイルの隣**（appsettings.json / SpRp_CLI.exe.config）
    $psi.WorkingDirectory = Split-Path $exe
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $false
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    # **入力は、渡すもの（ropc のパスワード）だけ**（対話を求めたら、待たずに失敗させる）。
    #   **パスワードはコマンド ラインに書かない**（プロセスの一覧から見える）
    $psi.RedirectStandardInput = $true
    $psi.StandardOutputEncoding = New-Object Text.UTF8Encoding $false
    $psi.StandardErrorEncoding = New-Object Text.UTF8Encoding $false

    $proc = [System.Diagnostics.Process]::Start($psi)
    if ($null -ne $stdin) { $proc.StandardInput.WriteLine($stdin) }
    $proc.StandardInput.Close()

    return @{
        Proc    = $proc
        Lines   = New-Object System.Collections.Generic.List[string]
        Pending = $null
        Eof     = $false
        Err     = $proc.StandardError.ReadToEndAsync()
    }
}

# 1 行読む（$ms ミリ秒まで待つ）。読めなければ $null
function Read-CliLine($cli, [int]$ms)
{
    if ($cli.Eof) { return $null }
    if (-not $cli.Pending) { $cli.Pending = $cli.Proc.StandardOutput.ReadLineAsync() }

    if (-not $cli.Pending.Wait($ms)) { return $null }

    $line = $cli.Pending.Result
    $cli.Pending = $null
    if ($null -eq $line)
    {
        $cli.Eof = $true
        return $null
    }
    $cli.Lines.Add($line)
    return $line
}

# パターンに合う行が出るまで待つ。合った部分（グループ 1）を返す。出ずに終わる・待ちきれないと $null
function Wait-CliMatch($cli, [string]$pattern, [int]$seconds)
{
    $deadline = (Get-Date).AddSeconds($seconds)
    while ((Get-Date) -lt $deadline -and -not $cli.Eof)
    {
        $line = Read-CliLine $cli 500
        if ($null -ne $line)
        {
            $m = [regex]::Match($line, $pattern)
            if ($m.Success) { return $m.Groups[1].Value }
        }
    }
    return $null
}

# 終わるまで待つ。終了コードを返す（待ちきれなければ $null）
function Wait-CliExit($cli, [int]$seconds)
{
    $deadline = (Get-Date).AddSeconds($seconds)
    while ((Get-Date) -lt $deadline -and -not $cli.Eof)
    {
        Read-CliLine $cli 500 | Out-Null
    }
    if (-not $cli.Eof) { return $null }

    $cli.Proc.WaitForExit(10000) | Out-Null
    if (-not $cli.Proc.HasExited) { return $null }
    return $cli.Proc.ExitCode
}

function Stop-Cli($cli, [string]$log)
{
    if (-not $cli) { return }
    if (-not $cli.Proc.HasExited)
    {
        $cli.Proc.Kill()
        $cli.Proc.WaitForExit(10000) | Out-Null
    }

    $text = ($cli.Lines -join "`r`n")
    if ($cli.Err.Wait(5000) -and $cli.Err.Result) { $text += "`r`n--- stderr ---`r`n" + $cli.Err.Result }
    [IO.File]::WriteAllText($log, $text, (New-Object Text.UTF8Encoding $true))
}

# CLI の出力の最後の数行（NG の理由に添える）
function Get-Tail($cli)
{
    $tail = @($cli.Lines | Where-Object { $_.Trim() } | Select-Object -Last 2) -join ' / '
    if ($cli.Err.IsCompleted -and $cli.Err.Result) { $tail += ' / stderr : ' + (($cli.Err.Result -split "`n")[0]).Trim() }
    return $tail
}

# ------------------------------------------------------------------
# 1 件を通す
# ------------------------------------------------------------------
# 戻り値 : '' なら OK、それ以外は NG の理由
function Invoke-Device($cli, [string]$case, [string]$password)
{
    # 1. CLI が verification_uri_complete を表示する（**別の端末のブラウザで開く URL**）
    $vuc = Wait-CliMatch $cli '(https://[A-Za-z0-9.:\-]+/device_verify\?user_code=[A-Za-z0-9_\-]+)' 30
    if (-not $vuc) { return "verification_uri_complete が出ない（$(Get-Tail $cli)）" }

    # 2. ブラウザの代わりに開き、ログインする → 確認画面（Allow / Deny）
    $client = New-Browser
    try
    {
        $r = Invoke-IdpScreens $client (Send-Http $client 'GET' $vuc $null) $password
        $form = [regex]::Match($r.Body, '<form action="([^"]*device_verify[^"]*)"')
        if ($r.Status -ne 200 -or -not $form.Success -or $r.Body -notmatch 'name="allow"')
        {
            return "Device AuthZ の確認画面が出ない（$($r.Status) $(($r.Url -split '\?')[0])）"
        }

        # 3. 承認する（対照は拒否する）
        $action = (New-Object Uri((New-Object Uri($stubBase)), [System.Net.WebUtility]::HtmlDecode($form.Groups[1].Value))).AbsoluteUri
        $post = @{
            user_code = (Get-Field $r.Body 'user_code')
            __RequestVerificationToken = (Get-Field $r.Body '__RequestVerificationToken')
        }
        if ($case -eq '正常') { $post['allow'] = 'Allow' } else { $post['deny'] = 'Deny' }

        $r = Invoke-IdpScreens $client (Send-Http $client 'POST' $action $post) $password
        if ($r.Status -ge 400 -or $r.Status -lt 0)
        {
            return "承認・拒否の後が $($r.Status)（$(($r.Url -split '\?')[0])）"
        }
    }
    finally
    {
        $client.Dispose()
    }

    # 4. CLI はポーリングで結果を得て、終わる（interval は既定で 5 秒）
    return Test-CliResult $cli $case 'access_denied' 120
}

function Invoke-Pkce($cli, [string]$case, [string]$password)
{
    # 1. CLI が認可要求の URL を表示する（**表示した後に待ち受けを始めている**）
    $url = Wait-CliMatch $cli ('(' + [regex]::Escape("$stubBase/authorize?") + '\S+)') 30
    if (-not $url) { return "認可要求の URL が出ない（$(Get-Tail $cli)）" }

    # 2. ブラウザの代わりに開き、ログイン・同意する。**戻り先へのリダイレクトの手前で止まる**
    $client = New-Browser
    try
    {
        $r = Invoke-IdpScreens $client (Send-Http $client 'GET' $url $null) $password
        if ($r.Status -lt 300 -or $r.Status -ge 400 -or -not $r.Location.StartsWith($redirectUri))
        {
            return "スタブから $redirectUri へ戻らない（$($r.Status) $(($r.Url -split '\?')[0])）"
        }

        # 3. 戻りを CLI の待ち受けへ届ける（対照は state を書き換える）
        $back = $r.Location
        if ($case -ne '正常')
        {
            $back = [regex]::Replace($back, '([?&]state=)[^&]*', '${1}tampered')
        }

        $r = Send-Http $client 'GET' $back $null
        if ($r.Status -ne 200)
        {
            return "CLI の待ち受け（$redirectUri）が応答しない（$($r.Status) $($r.Body)）"
        }
    }
    finally
    {
        $client.Dispose()
    }

    return Test-CliResult $cli $case 'state' 60
}

# ------------------------------------------------------------------
# ブラウザを使わないグラント（cc・jwtbearer・ropc）
# ------------------------------------------------------------------
# **出力フォルダを写し、設定ファイルの値だけを書き換える**（対照用。ビルドの成果物は変えない）。
#   net10.0 は appsettings.json、net48 は SpRp_CLI.exe.config。戻り値は、写しの実行ファイル。
function Copy-PatchedCli($t, [hashtable]$values)
{
    $src = Split-Path $t.Exe
    $safe = ("$($t.Name)_$($t.Flow)" -replace '[^A-Za-z0-9.]', '_')
    $dst = Join-Path $OutputDir "cli_patched_$safe"

    Remove-Item -Recurse -Force $dst -ErrorAction SilentlyContinue
    Copy-Item -Recurse $src $dst

    $config = if ($t.Exe.EndsWith('.dll')) { Join-Path $dst 'appsettings.json' } else { Join-Path $dst 'SpRp_CLI.exe.config' }
    $text = [IO.File]::ReadAllText($config)

    foreach ($k in $values.Keys)
    {
        $v = [string]$values[$k]
        $pattern = if ($t.Exe.EndsWith('.dll')) { '("' + [regex]::Escape($k) + '"\s*:\s*")[^"]*(")' }
                   else { '(key="' + [regex]::Escape($k) + '"\s+value=")[^"]*(")' }
        if (-not [regex]::IsMatch($text, $pattern)) { throw "設定に $k がありません : $config" }
        $text = [regex]::Replace($text, $pattern, { param($m) $m.Groups[1].Value + $v + $m.Groups[2].Value })
    }

    [IO.File]::WriteAllText($config, $text, (New-Object Text.UTF8Encoding $true))
    return Join-Path $dst (Split-Path -Leaf $t.Exe)
}

# 1 件を通す。戻り値は、正常 / 対照の判定（Test-GrantResult）
function Invoke-Grant($t, [string]$case, [string]$password, [hashtable]$wrongKey, [string]$log)
{
    $exe = ''
    $stdin = $null

    if ($t.Flow -eq 'ropc') { $stdin = $password }

    if ($case -ne '正常')
    {
        switch ($t.Flow)
        {
            'cc'        { $exe = Copy-PatchedCli $t @{ SpRp_BatchClientSecret = 'wrong-secret-for-e2e' } }
            'jwtbearer' { $exe = Copy-PatchedCli $t $wrongKey }
            'ropc'      { $stdin = 'wrong-password-for-e2e' }
        }
    }

    $cli = $null
    try
    {
        $cli = Start-Cli $t $exe $stdin
        return Test-GrantResult $cli $t.Flow $case 60
    }
    finally
    {
        Stop-Cli $cli $log
    }
}

#   正常 : 終了コード 0、access_token を得た旨の行（ropc は、さらに id_token を検証し UserInfo を得た旨の行）
#   対照 : 終了コード 1、invalid_client（cc・jwtbearer）/ access_denied（ropc）
function Test-GrantResult($cli, [string]$flow, [string]$case, [int]$seconds)
{
    $code = Wait-CliExit $cli $seconds
    if ($null -eq $code) { return "CLI が終わらない（$seconds 秒。$(Get-Tail $cli)）" }

    $out = $cli.Lines -join "`n"

    if ($case -eq '正常')
    {
        if ($code -ne 0) { return "終了コード $code（$(Get-Tail $cli)）" }
        if ($out -notmatch 'OK   access_token を得ました') { return "access_token を得た旨の行が無い（$(Get-Tail $cli)）" }
        if ($flow -eq 'ropc' -and $out -notmatch 'OK   id_token を検証し、UserInfo を得ました') { return "id_token・UserInfo の行が無い（$(Get-Tail $cli)）" }
        return ''
    }

    $word = if ($flow -eq 'ropc') { 'access_denied' } else { 'invalid_client' }
    if ($code -ne 1) { return "対照なのに終了コード $code（$(Get-Tail $cli)）" }
    if ($out -notmatch [regex]::Escape($word)) { return "対照の理由（$word）が出ていない（$(Get-Tail $cli)）" }
    return ''
}

# CLI の終わり方を判定する
#   正常 : 終了コード 0、id_token を検証した旨の行、UserInfo を得た旨の行
#   対照 : 終了コード 1、$controlWord を含む行（access_denied / state）
function Test-CliResult($cli, [string]$case, [string]$controlWord, [int]$seconds)
{
    $code = Wait-CliExit $cli $seconds
    if ($null -eq $code) { return "CLI が終わらない（$seconds 秒。$(Get-Tail $cli)）" }

    $out = $cli.Lines -join "`n"

    if ($case -eq '正常')
    {
        if ($code -ne 0) { return "終了コード $code（$(Get-Tail $cli)）" }
        if ($out -notmatch 'id_token (を検証しました|is verified)') { return "id_token を検証した旨の行が無い（$(Get-Tail $cli)）" }
        if ($out -notmatch 'OK   UserInfo|NORMAL_END') { return "UserInfo を得た旨の行が無い（$(Get-Tail $cli)）" }
        return ''
    }

    if ($code -ne 1) { return "対照なのに終了コード $code（$(Get-Tail $cli)）" }
    if ($out -notmatch [regex]::Escape($controlWord)) { return "対照の理由（$controlWord）が出ていない（$(Get-Tail $cli)）" }
    return ''
}

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
if ($d.Status -ne 200) { $lack += "MPAS のスタブが応答しません（$stubBase。..\README.md の 2 節の StubCompose.ps1 で起動する）" }

foreach ($exe in @($targets | ForEach-Object { $_.Exe } | Select-Object -Unique))
{
    if (-not (Test-Path $exe)) { $lack += "ビルドされていません : $exe" }
}

if (@($targets | Where-Object { $_.Flow -eq 'pkce' }).Count -gt 0 -and (Test-PortInUse 12345))
{
    $lack += 'ポート 12345 が使用中です（pkce の戻り先。他の CLI を動かしていないか）'
}

#   **テスト利用者のパスワードは、スタブの土台の appsettings.json の TestUserPWD**（MPAS の container\Stub\README.md 3 節）
$appSettings = $StubAppSettings
if ([string]::IsNullOrEmpty($appSettings))
{
    # **MPAS の場所は、引数 ＞ 環境変数 MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**（#652）
    . (Join-Path $progRoot '..\files\tools\PowerShell\RepoPath.ps1')
    $MpasRoot = Resolve-RepoPath -Variable MPAS_PATH -Argument $MpasRoot -ArgumentName '-MpasRoot'
    $appSettings = $(if ($MpasRoot) { Join-Path $MpasRoot 'root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\appsettings.json' } else { '' })
}
$password = $null
if (Test-Path $appSettings)
{
    $m = [regex]::Match([IO.File]::ReadAllText($appSettings), '"TestUserPWD"\s*:\s*"([^"]*)"')
    if ($m.Success) { $password = $m.Groups[1].Value }
}
if (-not $password) { $lack += "テスト利用者のパスワードを読めません（$appSettings の TestUserPWD。-MpasRoot か -StubAppSettings で場所を指定する）" }

#   **jwtbearer の対照は、別の鍵（スタブ自身の署名鍵 SHA256RSA_Server.pfx）で署名させる**。
#   パスワードは、スタブの土台の appsettings.json の RsaPfxPassword（テスト用）。
$wrongKey = $null
if (-not $SkipControl -and @($targets | Where-Object { $_.Flow -eq 'jwtbearer' }).Count -gt 0)
{
    $m = if (Test-Path $appSettings) { [regex]::Match([IO.File]::ReadAllText($appSettings), '"RsaPfxPassword"\s*:\s*"([^"]*)"') } else { $null }
    if ($m -and $m.Success)
    {
        $wrongKey = @{
            SpRp_RsaPfxFilePath = 'C:/root/files/resource/X509/SHA256RSA_Server.pfx'
            SpRp_RsaPfxPassword = $m.Groups[1].Value
        }
    }
    else
    {
        $lack += "jwtbearer の対照の鍵のパスワードを読めません（$appSettings の RsaPfxPassword）"
    }
}

#   **ropc は、テスト利用者が居ないと通らない**。MPAS は、アカウントの画面を初めて開いたときに作るので、ログイン画面を一度開く
if (@($targets | Where-Object { $_.Flow -eq 'ropc' }).Count -gt 0 -and $d.Status -eq 200)
{
    $probe = New-Browser
    $l = Send-Http $probe 'GET' "$stubBase/Account/Login" $null
    $probe.Dispose()
    if ($l.Status -ne 200) { $lack += "スタブのログイン画面が開けません（$($l.Status)。テスト利用者を作れない）" }
}

if ($targets.Count -eq 0) { $lack += '対象がありません（-Flow / -Only を確かめる）' }

if ($lack.Count -gt 0)
{
    foreach ($l in $lack) { Write-Output "  NG  $l" }
    exit 2
}
Write-Output '  OK  スタブ・ビルド・ポート 12345・テスト利用者'
if (@($targets | Where-Object { $grants -contains $_.Flow }).Count -gt 0)
{
    Write-Output '      （cc・jwtbearer・ropc は、スタブに Sample4SpRp.stub.json が要る。StubCompose.ps1 の既定）'
}

# ------------------------------------------------------------------
# 実行（**1 件ずつ**。pkce はどれもポート 12345 で待ち受ける）
# ------------------------------------------------------------------
$results = @()

foreach ($t in $targets)
{
    Write-Output ''
    Write-Output "=== $($t.Name) $($t.Flow)（$($t.Args)） ==="

    foreach ($case in $cases)
    {
        # **ファイル名は英数字だけにする**（ケース名は日本語なので、置き換えると正常と対照が同じ名前になる）
        $caseKey = if ($case -eq '正常') { 'normal' } else { 'control' }
        $safe = ("$($t.Name)_$($t.Flow)_$caseKey" -replace '[^A-Za-z0-9.]', '_')
        $log = Join-Path $OutputDir "cli_$safe.log"
        $cli = $null

        if ($grants -contains $t.Flow)
        {
            # **ブラウザを使わないグラント**：CLI を動かして、終わるのを待つだけ（起動と後始末は Invoke-Grant）
            try
            {
                $ng = Invoke-Grant $t $case $password $wrongKey $log
            }
            catch
            {
                $ng = $_.Exception.Message
            }
        }
        else
        {
            try
            {
                $cli = Start-Cli $t
                $ng = if ($t.Flow -eq 'device') { Invoke-Device $cli $case $password } else { Invoke-Pkce $cli $case $password }
            }
            catch
            {
                $ng = $_.Exception.Message
            }
            finally
            {
                Stop-Cli $cli $log
            }
        }

        $ok = [string]::IsNullOrEmpty($ng)
        $detail = if (-not $ok) { $ng }
                  elseif ($case -eq '正常')
                  {
                      switch ($t.Flow)
                      {
                          'cc'        { 'access_token を得た（終了コード 0）' }
                          'jwtbearer' { 'access_token を得た（終了コード 0）' }
                          default     { 'id_token を検証し、UserInfo を得た（終了コード 0）' }
                      }
                  }
                  else
                  {
                      switch ($t.Flow)
                      {
                          'device'    { '拒否すると access_denied で終了コード 1' }
                          'pkce'      { 'state を書き換えると、不一致で終了コード 1' }
                          'cc'        { '秘密を書き換えると invalid_client で終了コード 1' }
                          'jwtbearer' { '別の鍵で署名すると invalid_client で終了コード 1' }
                          default     { '違うパスワードでは access_denied で終了コード 1' }
                      }
                  }

        $results += [pscustomobject]@{ 対象 = $t.Name; フロー = $t.Flow; ケース = $case; 結果 = $(if ($ok) { 'OK' } else { 'NG' }); 内容 = $detail }
        Write-Output ("  {0,-4}{1,-4}{2}" -f $(if ($ok) { 'OK' } else { 'NG' }), $case, $detail)
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
