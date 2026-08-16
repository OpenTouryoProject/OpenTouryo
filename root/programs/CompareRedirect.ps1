<#
.SYNOPSIS
    bindingRedirect の newVersion を、そのプロジェクトが配布するアセンブリの版と突き合わせる。

.DESCRIPTION
    `bindingRedirect` は「この範囲の要求が来たら、この版を読め」という宣言である。
    **宣言した版が配布されていなければ、その範囲の要求は解決に失敗する。**

    ＜今は動いているのが厄介＞

      実体の版が範囲の外にあるうちは、転送されず直接束縛されるため**表面化しない。**
      **古い版を要求する依存が 1 つ増えた瞬間に落ちる**、という形で顕在化する。
      `0_RunAll.ps1` は通る（実測）。**通ることは「正しい」の証拠にならない。**

    ＜net10.0 とは別の現象＞

      | | net10.0 | net48（本スクリプトの対象） |
      |---|---|---|
      | bindingRedirect | **仕組みが無い** | ある |
      | ずれると | **即 FileNotFoundException** | **黙って通る** |

      net10.0 側の話は Samples4NetCore/ANALYSIS.md 7 節。

    ＜判定の基準は「そのプロジェクトの配下」＞

      **リポジトリ全体から探してはいけない。**（#556 で踏んだ）
      bindingRedirect が決めるのは**そのアプリが読む先**であり、
      他のプロジェクトの bin に在るかどうかは関係がない。

      全体から探すと、**ビルドの状態で答えが変わる。**
      1_BuildAll.ps1 は bin を消すため、測るタイミングで結果が動いてしまう。

    ＜ビルド後に実行すること＞

      配布物を見るため、**ビルドしていないと「判定不能」ばかりになる。**
      0_RunAll.ps1 に組み込むなら 1_BuildAll.ps1 の後。

      **「判定不能」は「問題なし」ではない。** 材料が無いだけである。
      不一致には数えないが、件数は必ず出す。

.PARAMETER Detail
    「判定不能」の内訳も表示する。

.PARAMETER Only
    対象を相対パスの部分一致で絞る（例: -Only "WS_sample"）。

.PARAMETER Check
    **合否を返す。** 不一致が 1 件でもあれば終了コード 1 を返す。
    「判定不能」では落とさない。

.EXAMPLE
    .\CompareRedirect.ps1

.EXAMPLE
    .\CompareRedirect.ps1 -Detail -Only "Tests"

.EXAMPLE
    .\CompareRedirect.ps1 -Check

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/17  玄人 幸道         新規作成（#556）

    -Check を付けない限り、終了コードは常に 0。

    **bindingRedirect は NuGet の生成物である。** 手で直しても、パッケージ操作で
    また書き換わる。だからこそ「直す」より「ずれを検知できる状態にしておく」方を採る。
#>
[CmdletBinding()]
param(
    [switch]$Detail,
    [string]$Only,
    [switch]$Check
)

$ErrorActionPreference = "Continue"

$root = $PSScriptRoot
. (Join-Path $root "SummaryTable.ps1")

if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# ------------------------------------------------------------------
# config から bindingRedirect を読む
# ------------------------------------------------------------------
function Get-Redirects([string]$path)
{
    try { [xml]$x = Get-Content $path -Raw -EA Stop } catch { return @() }

    $list = @()
    foreach ($d in $x.SelectNodes("//*[local-name()='dependentAssembly']"))
    {
        $id = $d.SelectSingleNode("*[local-name()='assemblyIdentity']")
        $br = $d.SelectSingleNode("*[local-name()='bindingRedirect']")
        if (-not $id -or -not $br) { continue }

        $name = $id.GetAttribute("name")
        $new  = $br.GetAttribute("newVersion")
        if (-not $name -or -not $new) { continue }

        $list += [PSCustomObject]@{ Name = $name; New = $new }
    }
    return $list
}

# ------------------------------------------------------------------
# 対象（追跡下の config のうち、bindingRedirect を持つもの）
# ------------------------------------------------------------------
$tracked = & git -C $root ls-files
if ($LASTEXITCODE -ne 0 -or -not $tracked)
{
    Write-Host "  git ls-files が使えないため、中止する。" -ForegroundColor Red
    exit 0
}

$rows      = @()
$mismatch  = @()
$unknown   = @()
$total     = 0
$okCount   = 0

foreach ($t in $tracked)
{
    if ($t -notmatch '\.config$') { continue }
    if ($t -match '/packages/')   { continue }
    if ($Only -and ($t -notlike "*$Only*")) { continue }

    $full = Join-Path $root ($t -replace '/', '\')
    if (-not (Test-Path $full)) { continue }

    $redirects = @(Get-Redirects $full)
    if ($redirects.Count -eq 0) { continue }

    $rel = $t -replace '^root/programs/', ''
    $dir = Split-Path $full -Parent

    # **参照されている名前だけを読む。**
    #   配下の DLL を全部 GetAssemblyName にかけると桁違いに遅い（#556 で 7.3 分かかった）。
    $wanted = @{}
    foreach ($r in $redirects) { $wanted[$r.Name] = $true }

    $found = @{}
    Get-ChildItem $dir -Recurse -File -Filter *.dll -EA SilentlyContinue | ForEach-Object {
        if (-not $wanted.ContainsKey($_.BaseName)) { return }
        try { $v = [System.Reflection.AssemblyName]::GetAssemblyName($_.FullName).Version.ToString() }
        catch { return }

        if (-not $found.ContainsKey($_.BaseName))
        {
            $found[$_.BaseName] = New-Object System.Collections.Generic.HashSet[string]
        }
        $null = $found[$_.BaseName].Add($v)
    }

    $ok = 0; $ng = 0; $unk = 0

    foreach ($r in $redirects)
    {
        $total++
        $vers = $found[$r.Name]

        if ($null -eq $vers)
        {
            $unk++
            $unknown += [PSCustomObject]@{ Config = $rel; Name = $r.Name; New = $r.New }
        }
        elseif ($vers.Contains($r.New))
        {
            $ok++
        }
        else
        {
            $ng++
            $mismatch += [PSCustomObject]@{
                Config = $rel; Name = $r.Name; New = $r.New
                実体   = (($vers | Sort-Object) -join ", ")
            }
        }
    }

    $okCount += $ok

    $note = "一致 $ok"
    if ($ng  -gt 0) { $note += " / **不一致 $ng**" }
    if ($unk -gt 0) { $note += " / 判定不能 $unk" }

    $rows += [PSCustomObject]@{
        対象 = $rel
        結果 = $(if ($ng -gt 0) { "不一致" } elseif ($ok -eq 0) { "不明" } else { "OK" })
        内容 = $note
    }
}

# ------------------------------------------------------------------
# 出力
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ bindingRedirect ================"
Write-SummaryTable -Rows $rows -Columns @("対象", "結果", "内容")

Write-Host ""
Write-Host ("  宣言 {0} 件 : 一致 {1} / **不一致 {2}** / 判定不能 {3}" -f `
    $total, $okCount, $mismatch.Count, $unknown.Count)

if ($unknown.Count -gt 0)
{
    Write-Host "  **判定不能は「問題なし」ではない。** そのプロジェクトの配下に実体が無いだけで、"
    Write-Host "  ビルドしてから測り直せば判定できる（-Detail で内訳）。"
}

if ($mismatch.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 不一致（宣言した版が、そのプロジェクトの配下に無い）==="
    foreach ($m in ($mismatch | Sort-Object Config, Name))
    {
        Write-Host ("  " + $m.Config)
        Write-Host ("      {0} : 宣言 {1} / 配下の実体 {2}" -f $m.Name, $m.New, $m.実体)
    }
}

if ($Detail -and $unknown.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 判定不能（配下に実体が無い。ビルドしていない可能性）==="
    foreach ($u in ($unknown | Sort-Object Config, Name))
    {
        Write-Host ("  {0} : {1} → {2}" -f $u.Config, $u.Name, $u.New)
    }
}

if ($Check)
{
    Write-Host ""
    Write-Host "================ 判定 ================"

    if ($mismatch.Count -eq 0)
    {
        Write-Host ("  不一致なし。（判定不能 {0} 件）" -f $unknown.Count) -ForegroundColor Green
    }
    else
    {
        Write-Host ("  **不一致 {0} 件**" -f $mismatch.Count) -ForegroundColor Red
        Write-Host ""
        Write-Host "  newVersion を実体に合わせること。**ただし NuGet の生成物なので、"
        Write-Host "  パッケージ操作でまた書き換わる。**（#556）"
    }
}

Write-Host ""

if ($Check -and $mismatch.Count -gt 0) { exit 1 }
exit 0
