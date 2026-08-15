<#
.SYNOPSIS
    HTTPS の開発用証明書を、PEM 形式で書き出す（#548）。

.DESCRIPTION
    ＜パスワードを持たせない＞

    参照元（ASPNETMVCOnDocker）は PFX ＋ .env の CERT_PASSWORD を使うが、
    ここでは PEM ＋ 秘密鍵ファイルにして、**パスワードという管理対象を無くす**。

      ・.env が要らない（平文パスワードのファイルを作らない）
      ・compose での ${CERT_PASSWORD} 展開が要らない
      ・初回に決めるパスワードが無い

    守る対象は localhost 限定・自己署名・1 年で失効する開発用証明書であり、
    本番の TLS は前段のリバース プロキシが終端する想定である（README 参照）。
    PFX ＋ パスワードにしても、結局 PFX とパスワードの両方がホストに置かれるので、
    実質の防御力は変わらない。守るのはファイルの権限である。

    ＜出力＞
      .\https\aspnetapp.pem   証明書
      .\https\aspnetapp.key   秘密鍵（**リポジトリには含めない。.gitignore 済み**）

.PARAMETER SkipTrust
    証明書を信頼済みにする手順（dotnet dev-certs https --trust）を省く。
    ブラウザで警告が出てもよい場合や、確認ダイアログを出したくない場合に指定する。

.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File .\0_SetupCert.ps1

.NOTES
    Windows PowerShell 5.1 / PowerShell 7 の両方で動く。
#>
param(
    [switch]$SkipTrust
)

$ErrorActionPreference = "Stop"

$httpsDir = Join-Path $PSScriptRoot "https"
$pemPath  = Join-Path $httpsDir "aspnetapp.pem"
$keyPath  = Join-Path $httpsDir "aspnetapp.key"

Write-Host "============================================"
Write-Host "  HTTPS 開発用証明書のセットアップ（PEM）"
Write-Host "============================================"
Write-Host ""

if (-not (Test-Path $httpsDir))
{
    New-Item -ItemType Directory -Path $httpsDir | Out-Null
}

# ------------------------------------------------------------------
# PEM 形式で書き出す
# ------------------------------------------------------------------
# -np（--no-password）を付けるので、パスワードの入力は無い。
Write-Host "[1/2] 証明書を PEM で書き出す ..."
dotnet dev-certs https --format Pem -ep $pemPath -np

if ($LASTEXITCODE -ne 0)
{
    Write-Host "[エラー] 証明書の書き出しに失敗しました。" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $pemPath) -or -not (Test-Path $keyPath))
{
    # 終了コードだけでは判断しない（生成物の存在も確認する）。
    Write-Host "[エラー] 出力が見つかりません : $pemPath / $keyPath" -ForegroundColor Red
    exit 1
}

# ------------------------------------------------------------------
# 信頼済みにする
# ------------------------------------------------------------------
if ($SkipTrust)
{
    Write-Host "[2/2] 信頼設定は省略しました（-SkipTrust）。"
    Write-Host "      ブラウザで証明書の警告が出ます。"
}
else
{
    Write-Host "[2/2] 証明書を信頼済みにする ..."
    Write-Host "      **Windows のセキュリティ警告が出たら「はい」を選ぶこと。**"

    dotnet dev-certs https --trust

    if ($LASTEXITCODE -ne 0)
    {
        # 信頼設定は失敗しても起動はできる（ブラウザで警告が出るだけ）。
        Write-Host "[警告] 信頼設定に失敗しました。手動での信頼が要ります。" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "============================================"
Write-Host "  完了"
Write-Host "============================================"
Write-Host "  証明書 : $pemPath"
Write-Host "  秘密鍵 : $keyPath"
Write-Host ""
Write-Host "  次は 1_PublishAndUp.bat を実行してください。"
Write-Host ""
