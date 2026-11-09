<#
.SYNOPSIS
    関連リポジトリの場所を、ユーザーの環境変数（OT_OTR_PATH・OT_MPAS_PATH など）に登録する（#652）。

.DESCRIPTION
    ＜何をするか＞
      CSV の行ごとに、Variable に DefaultPath を登録する。
      **-Apply を付けないと、何も変えない**（登録した場合にどうなるかを表示するだけ）。

    ＜読む CSV＞
      **同じフォルダの RepoPaths.csv（手元用。.gitignore の対象）が在ればそれを、無ければ _RepoPaths.csv（雛形。コミットする）を読む。**
      自分の配置が雛形と違うなら、_RepoPaths.csv を RepoPaths.csv に写して DefaultPath を直す。**雛形は書き換えない。**
      **雛形で登録するときは、警告を出す**（手元の配置と合っている保証が無い）。
      **そのフォルダが無い行は、登録しない**（打ち間違いや、まだ clone していないリポジトリ）。

    ＜登録した後＞
      **ユーザーの環境変数は、起動し直したプロセスから見える。** 開いているコンソールや Visual Studio は、開き直す。

    ＜使う側＞
      RepoPath.ps1 の Resolve-RepoPath が、引数 ＞ 環境変数 ＞ CSV の既定 の順に場所を決める。

.PARAMETER Csv
    読む CSV（列 : Variable, Repository, DefaultPath, UsedBy）。省けば同じフォルダの RepoPaths.csv（無ければ _RepoPaths.csv）。

.PARAMETER Only
    登録する変数を絞る（例：-Only OT_MPAS_PATH, OT_LSOD_PATH）。

.PARAMETER Apply
    実際に登録する。付けなければ、表示だけ。

.EXAMPLE
    .\Register-RepoPaths.ps1                       # どうなるかを見るだけ

.EXAMPLE
    .\Register-RepoPaths.ps1 -Apply                # RepoPaths.csv（無ければ _RepoPaths.csv）で登録する

.EXAMPLE
    Copy-Item .\_RepoPaths.csv .\RepoPaths.csv     # 自分の配置に直すなら、写して DefaultPath を直す

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/07  玄人 幸道         新規（#652）
     2026/11/07  玄人 幸道         RepoPaths.csv は手元用（.gitignore）にし、無ければ雛形の _RepoPaths.csv を読む
     2026/11/09  玄人 幸道         変数名の頭に OT_ を付けた。古い名前（OTR_PATH など）が登録されていれば、消してよいものとして表示する（消さない）
#>
[CmdletBinding()]
param(
    [string] $Csv = '',

    [string[]] $Only,

    [switch] $Apply
)

$ErrorActionPreference = 'Stop'

# パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
$here = Split-Path -Parent $PSCommandPath
. (Join-Path $here 'RepoPath.ps1')

if ([string]::IsNullOrEmpty($Csv))
{
    # RepoPath.ps1 が決めた CSV（手元用の RepoPaths.csv、無ければ雛形の _RepoPaths.csv）
    $Csv = $script:RepoPathsCsv
}
if (-not (Test-Path $Csv))
{
    throw "CSV が見つかりません : $Csv"
}

$rows = Get-RepoPathRows -CsvPath (Resolve-Path $Csv).Path
if ($Only)
{
    # **-File で渡すと、カンマ区切りが 1 つの文字列になる**（powershell.exe -File は配列を作らない）。分けて受ける
    $Only = @($Only | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })

    # **CSV に無い名前は、黙って捨てない**（打ち間違いに気付けるように）
    foreach ($o in $Only)
    {
        if (@($rows | Where-Object { $_.Variable -eq $o }).Count -eq 0)
        {
            Write-Host ("  -Only '{0}' は CSV にありません（飛ばします）" -f $o) -ForegroundColor Yellow
        }
    }

    $rows = @($rows | Where-Object { $Only -contains $_.Variable })
}
if ($rows.Count -eq 0)
{
    throw "登録する行がありません（-Only を確かめる）: $Csv"
}

Write-Host ("=== 関連リポジトリの場所（ユーザーの環境変数）。CSV : {0} ===" -f $Csv) -ForegroundColor Cyan
if ((Split-Path -Leaf $Csv) -eq '_RepoPaths.csv')
{
    # **雛形の既定は、手元の配置と合っている保証が無い**（違う場所を登録しないよう、警告する）
    Write-Host "  警告 : 雛形（_RepoPaths.csv）の既定で登録します。手元の配置と違うなら、RepoPaths.csv に写して直してから実行してください。" -ForegroundColor Yellow
}
if (-not $Apply)
{
    Write-Host "  **表示だけ**（登録するには -Apply を付ける）" -ForegroundColor Yellow
}

$changed = 0
foreach ($r in $rows)
{
    $value   = ([string]$r.DefaultPath).TrimEnd('\', '/')
    $current = [Environment]::GetEnvironmentVariable($r.Variable, 'User')

    if (-not (Test-Path $value))
    {
        Write-Host ("  {0,-16} 登録しない : フォルダが無い（{1}）" -f $r.Variable, $value) -ForegroundColor Red
        continue
    }

    if ($current -eq $value)
    {
        Write-Host ("  {0,-16} 同じ       : {1}" -f $r.Variable, $value)
        continue
    }

    $what = $(if ([string]::IsNullOrEmpty($current)) { "新規" } else { "変える（今 : $current）" })
    if ($Apply)
    {
        [Environment]::SetEnvironmentVariable($r.Variable, $value, 'User')
        $changed++
        Write-Host ("  {0,-16} 登録した   : {1}（{2}）" -f $r.Variable, $value, $what) -ForegroundColor Green
    }
    else
    {
        Write-Host ("  {0,-16} 登録する   : {1}（{2}）" -f $r.Variable, $value, $what) -ForegroundColor Yellow
    }
}

if ($Apply -and $changed -gt 0)
{
    Write-Host ""
    Write-Host "  **開いているコンソール・Visual Studio は、開き直すと新しい値が見える。**" -ForegroundColor Yellow
}

# 古い名前（2026/11/09 に頭へ OT_ を付ける前の名前）が登録されていれば、表示する。**消すのは人**（ほかのリポジトリがまだ使っていないかを見てから）
$olds = @(foreach ($r in $rows)
{
    $old = Get-RepoPathOldName $r.Variable
    if ($old)
    {
        $v = [Environment]::GetEnvironmentVariable($old, 'User')
        if (-not [string]::IsNullOrEmpty($v)) { [pscustomobject]@{ Old = $old; New = $r.Variable; Value = $v } }
    }
})
if ($olds.Count -gt 0)
{
    Write-Host ""
    Write-Host "  古い名前が登録されています（もう読みません。新しい名前を登録した後に、消してよい）:" -ForegroundColor Yellow
    foreach ($o in $olds)
    {
        Write-Host ("    {0,-16} → {1,-16} : {2}" -f $o.Old, $o.New, $o.Value) -ForegroundColor Yellow
    }
    Write-Host "  消すには : [Environment]::SetEnvironmentVariable('<古い名前>', `$null, 'User')" -ForegroundColor Yellow
}
