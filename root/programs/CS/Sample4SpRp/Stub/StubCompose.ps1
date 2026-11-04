<#
.SYNOPSIS
    SpRp サンプルの IdP（MPAS スタブ）を、起動・停止する（#649）。

.DESCRIPTION
    ＜何をするか＞
      **MPAS の container\Stub\docker-compose.yml を、このディレクトリの設定（sprp-stub.json）で動かす。**
      スタブの定義そのものは MPAS 側に在り、ここには持たない（二重に持たない）。

    ＜なぜスクリプトにするか＞
      **STUB_SETTINGS は絶対パスで渡す必要がある**（--env-file の相対パスは MPAS の container\Stub\ から見るため）。
      環境変数で渡せば、パラメタのファイルを作らずに済む。

    ＜前提＞
      **MPAS 側で、先に一度 container\Stub\1_PublishStub.ps1 を実行しておく**（publish と証明書）。

.PARAMETER Action
    Up（起動。既定）/ Down（停止して消す）/ Config（設定の解決だけを見る。起動しない）

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。

.EXAMPLE
    .\StubCompose.ps1

.EXAMPLE
    .\StubCompose.ps1 -Action Down

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/04  玄人 幸道         新規（#649）
#>
[CmdletBinding()]
param(
    [ValidateSet('Up', 'Down', 'Config')]
    [string] $Action = 'Up',

    [string] $MpasRoot = 'C:\MultiPurposeAuthSite'
)

$ErrorActionPreference = 'Stop'

# パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
$here = Split-Path -Parent $PSCommandPath
$stubDir = Join-Path $MpasRoot 'container\Stub'
$compose = Join-Path $stubDir 'docker-compose.yml'
$settings = Join-Path $here 'sprp-stub.json'

if (-not (Test-Path $compose))
{
    throw "MPAS のスタブが見つかりません : $compose（-MpasRoot で場所を指定してください）"
}

if ($Action -eq 'Up')
{
    # **publish と証明書が無いと、イメージを作れない・HTTPS で待ち受けられない**
    $published = Join-Path $stubDir 'app\publish\MultiPurposeAuthSite.dll'
    $cert = Join-Path $stubDir 'cert\aspnetapp.pfx'

    if (-not (Test-Path $published) -or -not (Test-Path $cert))
    {
        throw "MPAS のスタブの準備ができていません。先に $stubDir\1_PublishStub.ps1 を実行してください。"
    }
}

# **設定は絶対パスで渡す**（compose は / 区切りでも \ 区切りでも受ける）
$env:STUB_SETTINGS = (Resolve-Path $settings).Path

switch ($Action)
{
    'Up'     { docker compose -f $compose up -d --build }
    'Down'   { docker compose -f $compose down }
    'Config' { docker compose -f $compose config }
}

if ($LASTEXITCODE -ne 0)
{
    throw "docker compose が失敗しました（終了コード $LASTEXITCODE）。"
}

if ($Action -eq 'Up')
{
    Write-Output ""
    Write-Output "  起動しました : https://localhost:44310/.well-known/openid-configuration が 200 を返せば準備完了"
}
