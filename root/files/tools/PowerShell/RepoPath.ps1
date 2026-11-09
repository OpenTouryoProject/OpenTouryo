<#
.SYNOPSIS
    ほかのリポジトリの場所を決める関数 Resolve-RepoPath（#652）。ドット ソースで読み込んで使う。

.DESCRIPTION
    ＜決め方（上ほど強い）＞
      1. 引数（呼び出し側の -MpasRoot など。空でなければ、そのまま使う）
      2. 環境変数（OT_MPAS_PATH など。Register-RepoPaths.ps1 で登録する）
      3. CSV の既定のパス（DefaultPath）。**そのフォルダが在るときだけ**使う
      どれも無ければ $null を返す（止めるのは呼び出し側。登録すべき変数の名前を示すこと）。

    **どれを使ったかを 1 行表示する**（Write-Host。戻り値には混ぜない）。

    ＜変数名と既定のパスの一覧＞
      同じフォルダの CSV が、唯一の出どころである。**スクリプトに既定のパスを書き込まない。**
      **RepoPaths.csv（手元用。.gitignore の対象）が在ればそれを、無ければ _RepoPaths.csv（雛形。コミットする）を読む。**
      自分の配置が雛形と違うなら、_RepoPaths.csv を RepoPaths.csv に写して DefaultPath を直す。
      **雛形の既定を使ったときは、警告を出す**（手元の配置と合っている保証が無い）。

    ＜使い方＞
      . (Join-Path <root\files\tools\PowerShell> 'RepoPath.ps1')
      $MpasRoot = Resolve-RepoPath -Variable OT_MPAS_PATH -Argument $MpasRoot -ArgumentName '-MpasRoot'

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/07  玄人 幸道         新規（#652）
     2026/11/07  玄人 幸道         RepoPaths.csv は手元用（.gitignore）にし、無ければ雛形の _RepoPaths.csv を読む
     2026/11/09  玄人 幸道         変数名の頭に OT_ を付けた（OTR_PATH → OT_OTR_PATH など）。古い名前だけが在れば警告する（読まない）
#>

# **CSV の場所は、読み込んだ時点で決める**（関数の中の $PSScriptRoot に頼らない。5.1 との差を避ける。CODING.md 5 節）
#   手元用の RepoPaths.csv が在ればそれ、無ければ雛形の _RepoPaths.csv
$script:RepoPathsCsv = Join-Path $PSScriptRoot 'RepoPaths.csv'
if (-not (Test-Path $script:RepoPathsCsv))
{
    $script:RepoPathsCsv = Join-Path $PSScriptRoot '_RepoPaths.csv'
}
$script:RepoPathsCsvName = Split-Path -Leaf $script:RepoPathsCsv
$script:RepoPathsCsvIsTemplate = ($script:RepoPathsCsvName -eq '_RepoPaths.csv')

function Get-RepoPathRows
{
    <# CSV の全行（Variable・Repository・DefaultPath・UsedBy） #>
    param([string] $CsvPath = $script:RepoPathsCsv)

    if (-not (Test-Path $CsvPath))
    {
        return @()
    }
    return @(Import-Csv -Path $CsvPath -Encoding UTF8)
}

function Get-RepoPathOldName
{
    <# 頭に OT_ を付ける前の名前（2026/11/09 まで。OT_OTR_PATH → OTR_PATH）。OT_ で始まらなければ $null #>
    param([string] $Variable)

    if ($Variable -like 'OT_*') { return $Variable.Substring(3) }
    return $null
}

function Resolve-RepoPath
{
    <#
    .SYNOPSIS
        ほかのリポジトリの場所を、引数 ＞ 環境変数 ＞ CSV の既定のパス の順に決める。無ければ $null。
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string] $Variable,

        [string] $Argument = '',

        [string] $ArgumentName = ''
    )

    $label = $(if ($ArgumentName) { "$ArgumentName / $Variable" } else { $Variable })

    # 1. 引数
    if (-not [string]::IsNullOrEmpty($Argument))
    {
        Write-Host ("  {0} : {1}（引数）" -f $label, $Argument)
        return $Argument
    }

    # 2. 環境変数（プロセスの値。ユーザーの環境変数は、起動したときに引き継がれる）
    $value = [Environment]::GetEnvironmentVariable($Variable)
    if (-not [string]::IsNullOrEmpty($value))
    {
        $value = $value.TrimEnd('\', '/')
        Write-Host ("  {0} : {1}（環境変数 {2}）" -f $label, $value, $Variable)
        return $value
    }

    # 古い名前（頭に OT_ が無いもの）だけが在れば、知らせる。**読まない**（互換の分岐を残さない）
    $old = Get-RepoPathOldName $Variable
    if ($old -and -not [string]::IsNullOrEmpty([Environment]::GetEnvironmentVariable($old)))
    {
        Write-Host ("    警告 : 環境変数の名前が変わりました（{0} → {1}）。古い名前は読みません。Register-RepoPaths.ps1 -Apply で登録し直してください。" -f $old, $Variable) -ForegroundColor Yellow
    }

    # 3. CSV の既定のパス（在るときだけ。合わないパスを黙って使わない）
    $row = Get-RepoPathRows | Where-Object { $_.Variable -eq $Variable } | Select-Object -First 1
    if ($row -and -not [string]::IsNullOrEmpty($row.DefaultPath) -and (Test-Path $row.DefaultPath))
    {
        if ($script:RepoPathsCsvIsTemplate)
        {
            # **雛形の既定は、手元の配置と合っている保証が無い**（たまたま同じ名前のフォルダを拾うこともある）。警告にする
            Write-Host ("  {0} : {1}（**雛形 {2} の既定**。環境変数 {3} は未登録）" -f $label, $row.DefaultPath, $script:RepoPathsCsvName, $Variable) -ForegroundColor Yellow
            Write-Host ("    警告 : 雛形の既定を使いました。違うなら、環境変数 {0} を登録するか、_RepoPaths.csv を RepoPaths.csv に写して直してください。" -f $Variable) -ForegroundColor Yellow
        }
        else
        {
            Write-Host ("  {0} : {1}（{2} の既定。環境変数 {3} は未登録）" -f $label, $row.DefaultPath, $script:RepoPathsCsvName, $Variable)
        }
        return $row.DefaultPath
    }

    $hint = $(if ($row) { "（{0} の場所。{1} の既定の {2} も無い）" -f $row.Repository, $script:RepoPathsCsvName, $row.DefaultPath } else { "（{0} に {1} の行が無い）" -f $script:RepoPathsCsvName, $Variable })
    Write-Host ("  **{0} が決まりません**{1}。引数で渡すか、環境変数 {2} を登録してください（root\files\tools\PowerShell\Register-RepoPaths.ps1）。" -f $label, $hint, $Variable) -ForegroundColor Red
    return $null
}
