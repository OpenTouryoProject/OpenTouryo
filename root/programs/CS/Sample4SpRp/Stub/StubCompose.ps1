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

.PARAMETER Action
    Up（起動。既定）/ Down（停止して消す）/ Config（設定の解決だけを見る。起動しない）

.PARAMETER MpasRoot
    MultiPurposeAuthSite のリポジトリの場所。**省けば、環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**
    （root\files\tools\PowerShell\RepoPath.ps1。#652）。どれを使ったかを表示する。

.PARAMETER Settings
    スタブに渡すアプリの設定（appsettings.json と同じ書き方の JSON）。省けば、このフォルダの Sample4SpRp.stub.json。

.PARAMETER MpasDefault
    Sample4SpRp.stub.json を渡さず、MPAS の既定（appsettings.Stub.sample.json）で動かす。-Settings とは併せて使えない。

.EXAMPLE
    .\StubCompose.ps1

.EXAMPLE
    .\StubCompose.ps1 -Action Down

.EXAMPLE
    .\StubCompose.ps1 -MpasDefault

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
#>
[CmdletBinding()]
param(
    [ValidateSet('Up', 'Down', 'Config')]
    [string] $Action = 'Up',

    [string] $MpasRoot = '',

    [string] $Settings = '',

    [switch] $MpasDefault
)

$ErrorActionPreference = 'Stop'

if ($MpasDefault -and -not [string]::IsNullOrEmpty($Settings))
{
    throw '-Settings と -MpasDefault は、併せて使えません。'
}

# **既定は、このフォルダの Sample4SpRp.stub.json**（パスの既定値は param() に書かない。5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
if (-not $MpasDefault -and [string]::IsNullOrEmpty($Settings))
{
    $Settings = Join-Path (Split-Path -Parent $PSCommandPath) 'Sample4SpRp.stub.json'
}

# **MPAS の場所は、引数 ＞ 環境変数 OT_MPAS_PATH ＞ RepoPaths.csv（無ければ _RepoPaths.csv）の既定**（#652）
#   パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
. (Join-Path (Split-Path -Parent $PSCommandPath) '..\..\..\..\files\tools\PowerShell\RepoPath.ps1')
$MpasRoot = Resolve-RepoPath -Variable OT_MPAS_PATH -Argument $MpasRoot -ArgumentName '-MpasRoot'
if (-not $MpasRoot)
{
    throw "MPAS の場所が決まりません（-MpasRoot で渡すか、環境変数 OT_MPAS_PATH を登録してください）。"
}

$stubDir = Join-Path $MpasRoot 'container\Stub'
$compose = Join-Path $stubDir 'docker-compose.yml'
$upBat = Join-Path $stubDir '2_DockerComposeUp.bat'
$downBat = Join-Path $stubDir '3_DockerComposeDown.bat'

if (-not (Test-Path $compose))
{
    throw "MPAS のスタブが見つかりません : $compose（-MpasRoot か環境変数 OT_MPAS_PATH で、MPAS の場所を指定してください）"
}

if (($Action -ne 'Config') -and (-not (Test-Path $upBat) -or -not (Test-Path $downBat)))
{
    throw "MPAS のスタブの起動バッチが見つかりません : $stubDir（MPAS#340 より前の版です。MPAS を更新してください）"
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

# **MPAS のバッチを呼ぶ間だけ、コンソールを UTF-8（65001）にする**（#668）。
#   MPAS の 2_DockerComposeUp.bat などは、日本語の echo を UTF-8 で書いている。
#   **cmd は、バッチをコンソールの出力のコード ページで読む**ので、既定の 932 のままだと、その行が化ける。
#   [Console]::OutputEncoding を変えると、コンソールのコード ページも変わる。**終わったら元に戻す**（呼んだ側のコンソールを変えたままにしない）。
$oldEncoding = [Console]::OutputEncoding
try
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false

    switch ($Action)
    {
        'Up'     { & $upBat }
        'Down'   { & $downBat }
        'Config' { docker compose -f $compose config }
    }
}
finally
{
    [Console]::OutputEncoding = $oldEncoding
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
