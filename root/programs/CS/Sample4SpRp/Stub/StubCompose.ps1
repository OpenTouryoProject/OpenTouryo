<#
.SYNOPSIS
    SpRp サンプルの IdP（MPAS スタブ）を、起動・停止する（#649）。

.DESCRIPTION
    ＜何をするか＞
      **MPAS の container\Stub\docker-compose.yml を、MPAS の既定の設定のまま動かす。**
      スタブの定義も設定も MPAS 側に在り、ここには持たない（二重に持たない）。

    ＜既定の設定を使う＞
      **サンプルは、MPAS の雛形（_appsettings.json）に在るクライアントを使う**
      （CLI は TestClient3 = ae5a179813234ca290c8de93ef2e31dc）。
      **雛形に無いクライアントが要るときだけ、-Settings で設定の JSON を渡す**（..\README.md の 3 節）。

    ＜なぜスクリプトにするか＞
      **MPAS の場所と、スタブの準備（publish・証明書）を確かめてから動かすため。**
      **-Settings は絶対パスで渡す必要がある**（--env-file の相対パスは MPAS の container\Stub\ から見るため）。

    ＜前提＞
      **MPAS 側で、先に一度 container\Stub\1_PublishStub.ps1 を実行しておく**（publish と証明書）。

.PARAMETER Action
    Up（起動。既定）/ Down（停止して消す）/ Config（設定の解決だけを見る。起動しない）

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。

.PARAMETER Settings
    スタブに渡すアプリの設定（appsettings.json と同じ書き方の JSON）。省けば MPAS の既定（appsettings.Stub.sample.json）。

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
     2026/11/04  玄人 幸道         既定は MPAS の設定のまま動かす（雛形のクライアントを使う）。-Settings を追加
#>
[CmdletBinding()]
param(
    [ValidateSet('Up', 'Down', 'Config')]
    [string] $Action = 'Up',

    [string] $MpasRoot = 'C:\MultiPurposeAuthSite',

    [string] $Settings = ''
)

$ErrorActionPreference = 'Stop'

$stubDir = Join-Path $MpasRoot 'container\Stub'
$compose = Join-Path $stubDir 'docker-compose.yml'

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

# **設定を渡すときだけ、絶対パスにして渡す**（渡さなければ MPAS の既定）。
#   **前の実行で残った値を使わないよう、渡さないときは消す**（同じセッションで続けて動かすとき）。
if ([string]::IsNullOrEmpty($Settings))
{
    Remove-Item Env:\STUB_SETTINGS -ErrorAction SilentlyContinue
}
else
{
    $env:STUB_SETTINGS = (Resolve-Path $Settings).Path
}

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
