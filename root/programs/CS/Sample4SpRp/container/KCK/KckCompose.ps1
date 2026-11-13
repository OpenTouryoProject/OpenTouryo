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

.PARAMETER Action
    Up（起動し、Discovery が 200 を返すまで待つ。既定）/ Down（停止して消す）/ Config（設定の解決だけを見る。起動しない）

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
#>
[CmdletBinding()]
param(
    [ValidateSet('Up', 'Down', 'Config')]
    [string] $Action = 'Up',

    [int] $WaitSeconds = 300
)

$ErrorActionPreference = 'Stop'

# パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
$here = Split-Path -Parent $PSCommandPath
$compose = Join-Path $here 'docker-compose.yml'
$certDir = Join-Path $here 'cert'
$discovery = 'https://localhost:44320/realms/sprp/.well-known/openid-configuration'

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
    $clientCer = 'C:\root\files\resource\X509\SHA256RSAClientCert.cer'
    if (-not (Test-Path $clientCer))
    {
        throw "テスト用のクライアント証明書がありません : $clientCer（リソースを C:\root\files\resource に置く。root\Readme.ja.md）"
    }
    Copy-Item $clientCer (Join-Path $certDir 'mpas-test-client.pem') -Force
    Write-Output "  MTLS : $clientCer を信頼する（https-client-auth=request）"
}

switch ($Action)
{
    'Up'     { & docker compose -f $compose up -d }
    'Down'   { & docker compose -f $compose down }
    'Config' { & docker compose -f $compose config }
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
        if (-not (& docker compose -f $compose ps --status running -q))
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
