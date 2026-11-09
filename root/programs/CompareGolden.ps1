<#
.SYNOPSIS
    OpenTouryo の Golden の比較（rules.psd1 の FileRules の Comparer。#657）。

.DESCRIPTION
    置き換えの前の検出器（CheckExpectationChange.ps1）の Get-GoldenOptions と CompareResult.ps1 の呼び出しを、
    本体の差し込み口（& <Comparer> -Pairs <Path・Expected・Actual の並び>）の形にしたもの。
    比較のオプション（SkipLog4net・NormBase64）は 2_RunAllTests.ps1 -List -PassThru から受け取る（二重に持たない）。
    定義に無いファイル（ResultLocal*.txt など）は、オプション無し（最も厳しい）で比べる。
#>
param(
    [object[]]$Pairs,
    # 2_RunAllTests.ps1・CompareResult.ps1 のあるフォルダ。既定は本スクリプトのフォルダ（root/programs）。
    [string]$ProgramsRoot = $PSScriptRoot
)

$ErrorActionPreference = "Stop"

# キーは Tests からの相対パス（区切りは /、小文字）
$map = @{}
$defs = & (Join-Path $ProgramsRoot "2_RunAllTests.ps1") -List -PassThru 6>$null
foreach ($d in @($defs))
{
    if (-not $d.Result -or $d.Kind -eq "NUnit") { continue }
    $map[($d.Result -replace '\\', '/').ToLowerInvariant()] = $d
}

foreach ($p in $Pairs)
{
    $opt = $map[($p.Path -replace '^.*?/Frameworks/Tests/', '').ToLowerInvariant()]
    $r = & (Join-Path $ProgramsRoot "CompareResult.ps1") -Expected $p.Expected -Actual $p.Actual -PassThru `
            -SkipLog4netTrace:([bool]($opt -and $opt.SkipLog4net)) -NormalizeBase64:([bool]($opt -and $opt.NormBase64)) 6>$null
    $r = @($r | Where-Object { $_ -and $_.PSObject.Properties["Result"] }) | Select-Object -Last 1
    if (-not $r) { throw ("CompareResult.ps1 の結果を受け取れませんでした : {0}" -f $p.Path) }
    [pscustomobject]@{
        Path    = $p.Path
        Changed = ($r.Result -ne "OK")
        Detail  = $(if ($r.Result -ne "OK") { "正規化後の差分 {0} 件" -f $r.DiffCount } else { "" })
    }
}
