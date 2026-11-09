# 期待値の変更の検出器の、OpenTouryo の規則（QUALITY.md 3 節。#657）。
#   本体（CheckExpectationChange.ps1）と共通の規則（rules.common.psd1）は、OpenTouryoCodingAgentAssets の写しであり、ここでは直さない。
#   このファイルは OpenTouryo が持つ。置き換えの前の検出器の、OpenTouryo に固有の部分を移したもの
#   （OpenTouryoCodingAgentAssets の docs/expectation-guard/rules.OpenTouryo.psd1 の下書きから）。
#   置き換えの前後で、導入の後の全コミット（90 件）の判定が同じであることを確かめた（#657）。
@{
    Reference    = 'QUALITY.md 3 節'
    IntroducedBy = 'root/programs/CheckExpectationChange.ps1'
    DiffScope    = @('root/programs/')

    Guard = @(
        '^root/programs/(CheckExpectationChange|CompareResult|CompareGolden)\.ps1$'
    )

    FileRules = @(
        @{
            Name           = 'Golden'
            Paths          = @('^root/programs/[^/]+/Frameworks/Tests/Golden/.+/Result[^/]*\.txt$')
            Comparer       = 'root/programs/CompareGolden.ps1'
            CommandPattern = 'Tests[\\/]+Golden'
        }
    )

    LineRules = @(
        @{
            Name  = 'テストの削除'
            Paths = @('^root/programs/[^/]+/Frameworks/Tests/NUnit/.+\.cs$')
            Count = 'Removed'
            Match = @( @{ Pattern = '[\[,]\s*(Test|TestCase|TestCaseSource)\s*[\],(]' } )
        }
        @{
            Name  = 'Ignore の追加'
            Paths = @('^root/programs/[^/]+/Frameworks/Tests/NUnit/.+\.cs$')
            Count = 'Added'
            Match = @( @{ Pattern = '[\[,]\s*(Ignore|Explicit)\s*[\],(]|\bAssert\s*\.\s*Ignore\s*\(' } )
        }
        @{
            Name  = '根拠の削除'
            Paths = @('^root/programs/[^/]+/Frameworks/Tests/NUnit/.+\.cs$')
            Count = 'Removed'
            Match = @( @{ Pattern = '[\[,]\s*Basis\s*\(' } )
        }
    )
}
