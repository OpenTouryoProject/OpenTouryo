<#
.SYNOPSIS
    SpRp サンプルの IdP（Keycloak）を、起動・停止する。

.DESCRIPTION
    ＜何をするか＞
      **このフォルダの docker-compose.yml を動かす。** MPAS のスタブ（..\Stub\StubCompose.ps1）と並ぶ、もう 1 つの IdP である。
      **同じサンプル・同じ E2E を、MPAS 以外の IdP で通す**ために使う（E2E は -Idp KCK）。

      | 値 | 内容 |
      |---|---|
      | issuer | https://localhost:44320/realms/sprp |
      | Discovery | https://localhost:44320/realms/sprp/.well-known/openid-configuration |
      | 利用者・クライアント | realm\sprp-realm.json（**MPAS のスタブと同じ client_id・戻り先・秘密**で登録してある） |
      | 管理コンソール | https://localhost:44320/admin/（admin / admin。**テスト専用**） |

    ＜証明書＞
      **Up のとき、cert\localhost.pem・localhost.key が無ければ、dotnet dev-certs で書き出す**（ASP.NET Core の開発用証明書）。
      **この PC で信頼済みの証明書を使う**ので、サンプル（net48・net10.0）は、そのまま HTTPS で Keycloak を呼べる。
      cert\ は .gitignore の対象（秘密鍵を含む）。

    ＜クライアント証明書（MTLS）＞
      **常に受け付ける**（https-client-auth=request。求めるが、無くても通す）。FAPI2 の par_mtls（tls_client_auth）が使う。
      **信頼するのは、テスト用のクライアント証明書（C:\root\files\resource\X509\SHA256RSAClientCert.cer。自己署名）だけ**で、Up のたびに cert\ に写す。
      証明書を持たないクライアント（CLI など）は、そのまま通る。

    ＜状態は残らない＞
      **start-dev で動かし、Down でコンテナごと消す**（利用者・同意・セッションは、再起動で realm の定義に戻る）。

    ＜前提＞
      Docker（compose v2）、.NET SDK（dev-certs）。**MPAS のスタブとは同時に起動しない**（メモリ。TESTING.md の冒頭）。

    ＜WSL2 の Docker で動かす（-Wsl2。#712）＞
      **Windows に docker コマンドが無い（または Windows コンテナ用の docker しか無い）ときに、WSL2 の中の Docker で動かす。**
      docker compose を wsl 経由で呼び、compose ファイルのパスを wslpath で WSL のパスにする。
      **Up のとき、WSL2 の VM が止まらないよう、常駐のプロセス（sleep infinity）を立てる**（VM は、wsl のプロセスが無いとアイドルで止まり、
      コンテナに SIGTERM が届く。LocalServicesOnDocker の WSL2 版と同じ理由）。Down で止める。
      **Discovery の待ちは Windows から https://localhost:44320 を呼ぶので、開発用証明書を、Windows が信頼しておく**（CI は win-sts のステップで入れる）。

.PARAMETER Action
    Up（起動し、Discovery が 200 を返すまで待つ。既定）/ Down（停止して消す）/ Config（設定の解決だけを見る。起動しない）

.PARAMETER Wsl2
    WSL2 の中の Docker で動かす（上の「WSL2 の Docker で動かす」。#712）。

.PARAMETER Distro
    -Wsl2 で使う WSL のディストリビューション名（省けば既定のもの）。

.PARAMETER WaitSeconds
    Up のとき、Discovery が応答するまで待つ秒数（既定 300。手元では 40〜80 秒。ほかの処理と重なると遅くなる）

.EXAMPLE
    .\KckCompose.ps1

.EXAMPLE
    .\KckCompose.ps1 -Action Down

.NOTES
    終了コード : 0 = 成功、1 = 失敗

    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/13  玄人 幸道         新規（#690）
     2026/11/13  玄人 幸道         クライアント証明書（MTLS）を常に受け付ける。信頼先の証明書を cert\ に写す（#690 ph2）
     2026/11/18  玄人 幸道         信頼先の証明書が C:\root に無ければ、このリポジトリの root\files\resource から写す（#703）
     2026/11/18  玄人 幸道         Linux では、コンテナが読めるよう cert\ の読み取りを開く（#703）
     2026/11/19  玄人 幸道         コメントを直した（このリポジトリの .cer を、MPAS の現行のものに揃えた。#705）
     2026/11/19  玄人 幸道         -Wsl2 を追加した（WSL2 の Docker で動かす。VM が止まらないよう、常駐のプロセスを立てる。#712）
#>
[CmdletBinding()]
param(
    [ValidateSet('Up', 'Down', 'Config')]
    [string] $Action = 'Up',

    [switch] $Wsl2,

    [string] $Distro = '',

    [int] $WaitSeconds = 300
)

$ErrorActionPreference = 'Stop'

# パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
$here = Split-Path -Parent $PSCommandPath
$compose = Join-Path $here 'docker-compose.yml'
$certDir = Join-Path $here 'cert'
$discovery = 'https://localhost:44320/realms/sprp/.well-known/openid-configuration'

# **docker compose は、このリポジトリの compose ファイルに対して呼ぶ。** -Wsl2 なら、wsl 経由で WSL2 の中の docker を呼ぶ（#712）
$keepAliveFile = Join-Path ([IO.Path]::GetTempPath()) 'kck-wsl-keepalive.pid'
if ($Wsl2)
{
    $wslBase = @()
    if ($Distro) { $wslBase += @('-d', $Distro) }
    $wslBase += @('-u', 'root', '--')
    $composeUnix = (& wsl @wslBase wslpath -a ($compose -replace '\\', '/')).Trim()
    if ($LASTEXITCODE -ne 0 -or -not $composeUnix) { throw "compose ファイルの WSL のパスを取れません（wslpath）: $compose" }
}

function Invoke-Compose([string[]]$ComposeArgs)
{
    if ($Wsl2) { & wsl @wslBase docker compose -f $composeUnix @ComposeArgs }
    else { & docker compose -f $compose @ComposeArgs }
}

if ($Action -eq 'Up')
{
    # **開発用証明書を書き出す**（無いときだけ。PEM・パスワード無し。Keycloak がそのまま読める形）
    $pem = Join-Path $certDir 'localhost.pem'
    $key = Join-Path $certDir 'localhost.key'
    if (-not (Test-Path $pem) -or -not (Test-Path $key))
    {
        New-Item -ItemType Directory -Force $certDir | Out-Null
        & dotnet dev-certs https --export-path $pem --format PEM --no-password | Out-Null
        if ($LASTEXITCODE -ne 0 -or -not (Test-Path $pem) -or -not (Test-Path $key))
        {
            throw "開発用証明書を書き出せません（dotnet dev-certs https --export-path $pem --format PEM --no-password）。"
        }
        Write-Output "  証明書 : $pem を書き出しました（dotnet dev-certs）"
    }
    else
    {
        Write-Output "  証明書 : $pem"
    }

    # **クライアント証明書（MTLS）の信頼先を写す**（テスト用の自己署名の証明書。FAPI2 の par_mtls。毎回写して、元と揃える）
    #   **C:\root に無ければ（Linux など）、このリポジトリの root\files\resource から写す**（#703）。
    #   このリポジトリの .cer は、MPAS の現行のものと同じ（#705。MPAS が作り直したら手で写す。BUILDING.md 9 節）
    $clientCer = 'C:\root\files\resource\X509\SHA256RSAClientCert.cer'
    if (-not (Test-Path $clientCer))
    {
        # KCK → container → Sample4SpRp → CS → programs → root
        $clientCer = Join-Path $here '..\..\..\..\..\files\resource\X509\SHA256RSAClientCert.cer'
    }
    if (-not (Test-Path $clientCer))
    {
        throw "テスト用のクライアント証明書がありません : $clientCer（リソースを C:\root\files\resource に置く。root\Readme.ja.md）"
    }
    Copy-Item $clientCer (Join-Path $certDir 'mpas-test-client.pem') -Force
    Write-Output "  MTLS : $clientCer を信頼する（https-client-auth=request）"

    # **Linux では、コンテナの中の Keycloak の利用者が読めるよう、cert\ の読み取りを開く**（#703）。
    #   dotnet dev-certs は、書き出すファイルを所有者だけが読める（0600）にする。マウントしたファイルは、
    #   ホストの所有者・権限のまま見えるので、Keycloak（ホストとは別の利用者）は読めずに起動で止まる。
    #   Windows（Rancher Desktop・WSL2）では、マウントしたファイルにこの制限が掛からないので要らない。
    #   開くのは、cert\ に書き出した写し（.gitignore の対象。localhost だけの開発用証明書と、テスト用の公開の証明書）だけ。
    if ([bool]$IsLinux)
    {
        & chmod 0644 $pem $key (Join-Path $certDir 'mpas-test-client.pem')
        if ($LASTEXITCODE -ne 0) { throw "cert\ の読み取りを開けません（chmod の終了コード $LASTEXITCODE）。" }
    }
}

# **-Wsl2 の Up では、WSL2 の VM が止まらないよう、常駐のプロセスを立てる**（Down で止める）
if ($Wsl2 -and $Action -eq 'Up')
{
    $keep = Start-Process -FilePath wsl -ArgumentList ($wslBase + @('sleep', 'infinity')) -PassThru -WindowStyle Hidden
    Set-Content -Path $keepAliveFile -Value $keep.Id
    Start-Sleep -Seconds 3
}

switch ($Action)
{
    'Up'     { Invoke-Compose @('up', '-d') }
    'Down'   { Invoke-Compose @('down') }
    'Config' { Invoke-Compose @('config') }
}

if ($Wsl2 -and $Action -eq 'Down' -and (Test-Path $keepAliveFile))
{
    $keepId = [int](Get-Content $keepAliveFile -ErrorAction SilentlyContinue)
    if ($keepId) { Stop-Process -Id $keepId -ErrorAction SilentlyContinue }
    Remove-Item $keepAliveFile -ErrorAction SilentlyContinue
}

if ($LASTEXITCODE -ne 0)
{
    throw "Keycloak の $Action が失敗しました（終了コード $LASTEXITCODE）。"
}

if ($Action -ne 'Up') { exit 0 }

# **Discovery が 200 を返すまで待つ**（realm の取り込みが済むまで、Keycloak は応答しない）
#   5.1（.NET Framework）は既定で TLS 1.2 を使わないことがある
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
$sw = [Diagnostics.Stopwatch]::StartNew()
$ready = $false
$exited = $false
while (-not $ready -and $sw.Elapsed.TotalSeconds -lt $WaitSeconds)
{
    try
    {
        $r = Invoke-WebRequest -Uri $discovery -UseBasicParsing -TimeoutSec 5
        $ready = ($r.StatusCode -eq 200)
    }
    catch
    {
        # **コンテナが落ちていれば、待たずに止める**（起動の失敗。realm の定義の誤りなど。ログを見る）
        if (-not (Invoke-Compose @('ps', '--status', 'running', '-q')))
        {
            $exited = $true
            break
        }
        Start-Sleep -Seconds 3
    }
}

Write-Output ''
if ($exited)
{
    Write-Output "  NG  Keycloak のコンテナが止まりました（$([int]$sw.Elapsed.TotalSeconds) 秒。docker compose -f `"$compose`" logs で見る）"
    exit 1
}
if (-not $ready)
{
    Write-Output "  NG  $WaitSeconds 秒待っても、Discovery が応答しません（$discovery。docker compose -f `"$compose`" logs で見る。起動が遅いだけなら -WaitSeconds を延ばす）"
    exit 1
}
Write-Output "  起動しました : $discovery が 200（$([int]$sw.Elapsed.TotalSeconds) 秒）"
