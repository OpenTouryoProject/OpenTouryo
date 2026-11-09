# expectation-guard-source: OpenTouryoCodingAgentAssets@a81584579945 src/quality/ExpectationGuard/tests/TestComparer.ps1 sha256:0a521f5338575d3cfa707cde9c0a18ca460524057cfddf35c21dc8d5f047bfd4
<#
.SYNOPSIS
    判定の用例で使う、FileRules の Comparer の見本（Golden の正規化の代わり）。

.DESCRIPTION
    「Time:」で始まる行（実行のたびに変わる値の例）を除いて比べる。
    Comparer の約束（rules.sample.psd1 の FileRules）を満たす最小の実装でもある。
      呼び方   : & <Comparer> -Pairs <Path・Expected・Actual の並び>
      返すもの : 対ごとに Path・Changed（真偽）・Detail（表示の補足。省略可）
#>
param([object[]]$Pairs)

foreach ($p in $Pairs)
{
    $norm = {
        param($file)
        @([IO.File]::ReadAllLines($file) | Where-Object { $_ -notmatch '^Time:' })
    }
    $a = & $norm $p.Expected
    $b = & $norm $p.Actual
    $diff = @(Compare-Object $a $b -CaseSensitive)
    [pscustomobject]@{
        Path    = $p.Path
        Changed = ($diff.Count -gt 0)
        Detail  = $(if ($diff.Count -gt 0) { "正規化後の差分 {0} 行" -f $diff.Count } else { "" })
    }
}
