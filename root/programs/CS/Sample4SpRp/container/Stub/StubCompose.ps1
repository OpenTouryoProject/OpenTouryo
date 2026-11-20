<#
.SYNOPSIS
    SpRp サンプルの IdP（MPAS スタブ）を、起動・停止する（#649）。

.DESCRIPTION
    ＜何をするか＞
      **MPAS の container\Stub\docker-compose.yml を動かす。** スタブの定義は MPAS 側に在り、ここには持たない。

    ＜既定で Sample4SpRp.stub.json を渡す（#668）＞
      **既定では、このフォルダの Sample4SpRp.stub.json を、スタブの追加の設定として渡す。**
      雛形（_appsettings.json）に無いクライアント（SpRp_MVC・SpRp_MVC_PKCE・SpRp_CLI_Batch・SPA_Hybrid）と、
      ROPC を開ける設定が入っている。**雛形のクライアントは、そのまま残る**（Device_Client・MVC_Sample など）。
      **以前は既定が MPAS の既定（appsettings.Stub.sample.json）で、-Settings を付け忘れると、
      雛形に無いクライアントが居らず「Invalid client_id.」になっていた。**

      | 指定 | 渡す設定 |
      |---|---|
      | 無し | **Sample4SpRp.stub.json** |
      | -Settings <パス> | そのファイル |
      | -MpasDefault | MPAS の既定（container\Stub\appsettings.Stub.sample.json） |

    ＜なぜスクリプトにするか＞
      **MPAS の場所と、スタブの準備（publish・証明書）を確かめてから動かすため。**
      **-Settings は絶対パスで渡す必要がある**（--env-file の相対パスは MPAS の container\Stub\ から見るため）。

    ＜起動・停止は MPAS のバッチに任せる＞
      **Up は container\Stub\2_DockerComposeUp.bat、Down は 3_DockerComposeDown.bat を呼ぶ**（MPAS#340）。
      **Up では、publish の成果物が MPAS のソースより古ければ警告が出る**（起動は止めない）。
      Config だけは、バッチが無いので docker compose を直接呼ぶ。
      **-Settings は、環境変数 STUB_SETTINGS でバッチへ渡る**（バッチには、パラメタのファイルを渡さない）。

    ＜前提＞
      **MPAS 側で、先に一度 container\Stub\1_PublishStub.ps1 を実行しておく**（publish と証明書。**MPAS を更新したときも**）。

    ＜WSL2 の Docker で動かす（-Wsl2。#713）＞
      **Windows に docker コマンドが無い（または Windows コンテナ用の docker しか無い）ときに、WSL2 の中の Docker で動かす**（windows-latest など）。
      MPAS のバッチ（.bat）は Windows の docker を呼ぶので使わず、**docker compose を wsl 経由で呼ぶ**。compose ファイルと、環境変数で渡すパス
      （STUB_SETTINGS・STUB_APPSETTINGS・STUB_RESOURCE_DIR）は、wslpath で WSL のパスにする。
      STUB_RESOURCE_DIR・STUB_APPSETTINGS を呼んだ側が決めていなければ、Linux と同じ既定（MPAS の root/files/resource と、appsettings.json（無ければ _appsettings.json））にする。
      **Up のとき、WSL2 の VM が止まらないよう、常駐のプロセス（sleep infinity）を立てる**（VM は、wsl のプロセスが無いとアイドルで止まり、
      コンテナに SIGTERM が届く。KckCompose.ps1 -Wsl2 と同じ理由）。Down で止める。
      **MPAS の publish（1_PublishStub.ps1）は、Windows で行う**（この -Wsl2 は、起動・停止だけ）。

.PARAMETER Action
    Up（起動。既定）/ Down（停止して消す）/ Config（設定の解決だけを見る。起動しない）

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。**省けば、環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**
    （root\files\tools\PowerShell\RepoPaths\RepoPath.ps1。#652）。どれを使ったかを表示する。

.PARAMETER Settings
    スタブに渡すアプリの設定（appsettings.json と同じ書き方の JSON）。省けば、このフォルダの Sample4SpRp.stub.json。

.PARAMETER MpasDefault
    Sample4SpRp.stub.json を渡さず、MPAS の既定（appsettings.Stub.sample.json）で動かす。-Settings とは併せて使えない。

.PARAMETER Wsl2
    WSL2 の中の Docker で動かす（上の「WSL2 の Docker で動かす」。#713）。

.PARAMETER Distro
    -Wsl2 で使う WSL のディストリビューション名（省けば既定のもの）。

.PARAMETER Mtls
    **スタブに、クライアント証明書（MTLS）を受け付けさせる**（MultiPurposeAuthSite#351。SpRp_MVC の FAPI2 の par_mtls が使う。#668 の追加）。
    MPAS の container\Stub\hooks\MtlsTestHook.dll を、環境変数 STUB_MTLS_HOOK で渡す（1_PublishStub.ps1 が作る）。
    **テスト専用**（発行元を問わずに証明書を受け、Subject と登録の一致だけを見る）。**付けなければ、今までどおり受け付けない。**

.EXAMPLE
    .\StubCompose.ps1

.EXAMPLE
    .\StubCompose.ps1 -Action Down

.EXAMPLE
    .\StubCompose.ps1 -MpasDefault

.EXAMPLE
    .\StubCompose.ps1 -Mtls

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/04  玄人 幸道         新規（#649）
     2026/11/04  玄人 幸道         既定は MPAS の設定のまま動かす（雛形のクライアントを使う）。-Settings を追加
     2026/11/07  玄人 幸道         起動・停止を MPAS の 2_DockerComposeUp.bat・3_DockerComposeDown.bat に任せた（MPAS#340）
     2026/11/07  玄人 幸道         既定で Sample4SpRp.stub.json を渡すようにし、-MpasDefault を足した（#668）
     2026/11/07  玄人 幸道         MPAS のバッチを呼ぶ間だけコンソールを UTF-8 にした（日本語の echo が化けていた。#668）
     2026/11/07  玄人 幸道         MPAS の場所を、引数 ＞ 環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定 で決める（#652）
     2026/11/10  玄人 幸道         -Mtls を足した（スタブにクライアント証明書を受け付けさせる。MultiPurposeAuthSite#351。#668 の追加）
     2026/11/13  玄人 幸道         RepoPath.ps1 を root/files/tools/PowerShell/RepoPaths から読む（#687）
     2026/11/13  玄人 幸道         Sample4SpRp\Stub から Sample4SpRp\container\Stub へ移した（Keycloak の KCK と並べる。#690）
     2026/11/19  玄人 幸道         Linux でも起動・停止できるようにした（MPAS の pwsh 版を呼ぶ。STUB_RESOURCE_DIR・STUB_APPSETTINGS の既定を差し替える。#703）
     2026/11/19  玄人 幸道         -Wsl2（と -Distro）を追加した（WSL2 の Docker で動かす。windows-latest 用。#713）
#>
[CmdletBinding()]
param(
    [ValidateSet('Up', 'Down', 'Config')]
    [string] $Action = 'Up',

    [string] $MpasRoot = '',

    [string] $Settings = '',

    [switch] $MpasDefault,

    [switch] $Mtls,

    [switch] $Wsl2,

    [string] $Distro = ''
)

$ErrorActionPreference = 'Stop'

if ($MpasDefault -and -not [string]::IsNullOrEmpty($Settings))
{
    throw '-Settings と -MpasDefault は、併せて使えません。'
}

# **既定は、このフォルダの Sample4SpRp.stub.json**（パスの既定値は param() に書かない。5.1 の -File で $PSScriptRoot が空になる。11_CODING.md 5 節）
if (-not $MpasDefault -and [string]::IsNullOrEmpty($Settings))
{
    $Settings = Join-Path (Split-Path -Parent $PSCommandPath) 'Sample4SpRp.stub.json'
}

# **MPAS の場所は、引数 ＞ 環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**（#652）
#   パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。11_CODING.md 5 節）
. (Join-Path (Split-Path -Parent $PSCommandPath) '../../../../../files/tools/PowerShell/RepoPaths/RepoPath.ps1')
$MpasRoot = Resolve-RepoPath -Variable OT_MPAS_PATH -Argument $MpasRoot -ArgumentName '-MpasRoot'
if (-not $MpasRoot)
{
    throw "MPAS の場所が決まりません（-MpasRoot で渡すか、環境変数 OT_MPAS_PATH を登録してください）。"
}

$stubDir = Join-Path $MpasRoot 'container/Stub'
$compose = Join-Path $stubDir 'docker-compose.yml'

# **-Wsl2 なら、docker compose は wsl 経由で WSL2 の中の docker を呼ぶ**（#713）
$keepAliveFile = Join-Path ([IO.Path]::GetTempPath()) 'stub-wsl-keepalive.pid'
$wslBase = @()
if ($Wsl2)
{
    if ($Distro) { $wslBase += @('-d', $Distro) }
    $wslBase += @('-u', 'root', '--')
}

# Windows のパス（C:\... や D:/...）を WSL のパスにする。それ以外（/ で始まる WSL のパス・コンテナの中のパス）は、そのまま
function ConvertTo-WslPath([string]$path)
{
    if ($path -notmatch '^[A-Za-z]:[\\/]') { return $path }
    $r = (& wsl @wslBase wslpath -a ($path -replace '\\', '/')).Trim()
    if ($LASTEXITCODE -ne 0 -or -not $r) { throw "WSL のパスにできません（wslpath）: $path" }
    return $r
}

# **Linux では、MPAS の pwsh 版（2_DockerComposeUp.ps1・3_DockerComposeDown.ps1。MultiPurposeAuthSite#401）を呼ぶ**（.bat は Windows だけ。#703）
$onUnix = ([Environment]::OSVersion.Platform -eq 'Unix')
$ext = if ($onUnix) { 'ps1' } else { 'bat' }
$upBat = Join-Path $stubDir "2_DockerComposeUp.$ext"
$downBat = Join-Path $stubDir "3_DockerComposeDown.$ext"

if (-not (Test-Path $compose))
{
    throw "MPAS のスタブが見つかりません : $compose（-MpasRoot か環境変数 OT_MPAS_PATH で、MPAS の場所を指定してください）"
}

if (-not $Wsl2 -and ($Action -ne 'Config') -and (-not (Test-Path $upBat) -or -not (Test-Path $downBat)))
{
    throw "MPAS のスタブの起動バッチが見つかりません : $stubDir（MPAS#340 より前の版です。MPAS を更新してください）"
}

if ($Action -eq 'Up')
{
    # **publish と証明書が無いと、イメージを作れない・HTTPS で待ち受けられない**
    $published = Join-Path $stubDir 'app/publish/MultiPurposeAuthSite.dll'
    $cert = Join-Path $stubDir 'cert/aspnetapp.pfx'

    if (-not (Test-Path $published) -or -not (Test-Path $cert))
    {
        throw "MPAS のスタブの準備ができていません。先に $stubDir\1_PublishStub.ps1 を実行してください。"
    }
}

# **設定は、絶対パスにして渡す**（-MpasDefault なら渡さず、MPAS の既定）。
#   **前の実行で残った値を使わないよう、渡さないときは消す**（同じセッションで続けて動かすとき）。
if ($MpasDefault)
{
    Remove-Item Env:\STUB_SETTINGS -ErrorAction SilentlyContinue
    Write-Output "  設定 : MPAS の既定（container\Stub\appsettings.Stub.sample.json。雛形に無いクライアントは居ない）"
}
else
{
    if (-not (Test-Path $Settings))
    {
        throw "スタブの設定が見つかりません : $Settings"
    }
    $env:STUB_SETTINGS = (Resolve-Path $Settings).Path
    Write-Output "  設定 : $($env:STUB_SETTINGS)"
}

# **MTLS（-Mtls）は、環境変数 STUB_MTLS_HOOK でスタブに渡る**（MPAS の docker-compose.yml が DOTNET_STARTUP_HOOKS に入れる。MultiPurposeAuthSite#351）。
#   **前の実行で残った値を使わないよう、付けないときは消す**（STUB_SETTINGS と同じ）。
if ($Mtls)
{
    $hook = Join-Path $stubDir 'hooks/MtlsTestHook.dll'
    if (($Action -eq 'Up') -and -not (Test-Path $hook))
    {
        throw "MTLS のフックがありません : $hook（MPAS の $stubDir\1_PublishStub.ps1 を実行し直してください。MultiPurposeAuthSite#351 より前の版です）"
    }
    $env:STUB_MTLS_HOOK = '/hooks/MtlsTestHook.dll'
    Write-Output "  MTLS : クライアント証明書を受け付ける（STUB_MTLS_HOOK=$($env:STUB_MTLS_HOOK)。テスト専用）"
}
else
{
    Remove-Item Env:\STUB_MTLS_HOOK -ErrorAction SilentlyContinue
}

# **Linux では、MPAS の .env の既定（Windows の手元向け）を、環境変数で差し替える**（MultiPurposeAuthSite の container/Stub/README.md 1 節。#703）。
#   STUB_RESOURCE_DIR : 署名鍵などのリソース（既定は C:/root/files/resource）→ MPAS の root/files/resource
#   STUB_APPSETTINGS  : アプリの設定の土台（既定は開発者の手元の appsettings.json。CI には無い）→ 在ればそれ、無ければ雛形（_appsettings.json）
#   **呼んだ側が環境変数を決めていれば、それを使う**
if ($onUnix -or $Wsl2)
{
    if (-not $env:STUB_RESOURCE_DIR)
    {
        $env:STUB_RESOURCE_DIR = Join-Path $MpasRoot 'root/files/resource'
    }

    if (-not $env:STUB_APPSETTINGS)
    {
        $coreDir = Join-Path $MpasRoot 'root/programs/MultiPurposeAuthSiteCore/MultiPurposeAuthSiteCore'
        $base = Join-Path $coreDir 'appsettings.json'
        if (-not (Test-Path $base)) { $base = Join-Path $coreDir '_appsettings.json' }
        $env:STUB_APPSETTINGS = $base
    }

    Write-Output "  $(if ($Wsl2) { 'WSL2' } else { 'Linux' }) : STUB_RESOURCE_DIR=$($env:STUB_RESOURCE_DIR)"
    Write-Output "          STUB_APPSETTINGS=$($env:STUB_APPSETTINGS)"
}

# **-Wsl2 では、環境変数を wsl の中の docker compose へ、env で渡す**（Windows の環境変数は、wsl の中の docker には届かない）。パスは WSL のパスにする
$stubEnv = @()
if ($Wsl2)
{
    foreach ($name in 'STUB_SETTINGS', 'STUB_APPSETTINGS', 'STUB_RESOURCE_DIR', 'STUB_MTLS_HOOK')
    {
        $v = [Environment]::GetEnvironmentVariable($name)
        if ($v) { $stubEnv += ('{0}={1}' -f $name, (ConvertTo-WslPath $v)) }
    }
}

function Invoke-StubCompose([string[]]$ComposeArgs)
{
    $composeUnix = ConvertTo-WslPath ((Resolve-Path $compose).Path)
    & wsl @wslBase env @stubEnv docker compose -f $composeUnix @ComposeArgs
}

# **MPAS のバッチを呼ぶ間だけ、コンソールを UTF-8（65001）にする**（#668）。
#   MPAS の 2_DockerComposeUp.bat などは、日本語の echo を UTF-8 で書いている。
#   **cmd は、バッチをコンソールの出力のコード ページで読む**ので、既定の 932 のままだと、その行が化ける。
#   [Console]::OutputEncoding を変えると、コンソールのコード ページも変わる。**終わったら元に戻す**（呼んだ側のコンソールを変えたままにしない）。
$oldEncoding = [Console]::OutputEncoding
try
{
    if (-not $onUnix)
    {
        [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
    }

    if ($Wsl2)
    {
        switch ($Action)
        {
            'Up'
            {
                # publish の成果物が古ければ警告（起動は止めない）。MPAS の Windows 側のスクリプトを呼ぶ
                $fresh = Join-Path $stubDir '../Common/CheckPublishFreshness.ps1'
                if (Test-Path $fresh) { & $fresh -PublishDir (Join-Path $stubDir 'app/publish') }

                # **WSL2 の VM が止まらないよう、常駐のプロセスを立てる**（Down で止める）
                $keep = Start-Process -FilePath wsl -ArgumentList ($wslBase + @('sleep', 'infinity')) -PassThru -WindowStyle Hidden
                Set-Content -Path $keepAliveFile -Value $keep.Id
                Start-Sleep -Seconds 3

                Invoke-StubCompose @('up', '-d', '--build')
            }
            'Down'
            {
                Invoke-StubCompose @('down')
                if (Test-Path $keepAliveFile)
                {
                    $keepId = [int](Get-Content $keepAliveFile -ErrorAction SilentlyContinue)
                    if ($keepId) { Stop-Process -Id $keepId -ErrorAction SilentlyContinue }
                    Remove-Item $keepAliveFile -ErrorAction SilentlyContinue
                }
            }
            'Config' { Invoke-StubCompose @('config') }
        }
    }
    else
    {
        switch ($Action)
        {
            'Up'     { & $upBat }
            'Down'   { & $downBat }
            'Config' { docker compose -f $compose config }
        }
    }
}
finally
{
    if (-not $onUnix)
    {
        [Console]::OutputEncoding = $oldEncoding
    }
}

if ($LASTEXITCODE -ne 0)
{
    throw "スタブの $Action が失敗しました（終了コード $LASTEXITCODE）。"
}

if ($Action -eq 'Up')
{
    Write-Output ""
    Write-Output "  起動しました : https://localhost:44310/.well-known/openid-configuration が 200 を返せば準備完了"
}
