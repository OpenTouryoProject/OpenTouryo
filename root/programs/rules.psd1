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
        @{
            # 検証（Assert.）の削除。ファイルごとの差し引きなので、書き直し・同じファイルの中での移動は数えない。
            #   Assert.Ignore は検証ではないので除く（足すほうは「Ignore の追加」が見る）。#671（汎用認証サイトの #324 に揃えた）
            Name  = '検証の削除'
            Paths = @('^root/programs/[^/]+/Frameworks/Tests/NUnit/.+\.cs$')
            Count = 'Removed'
            Match = @( @{ Pattern = '\bAssert\s*\.\s*(?!Ignore\b)\w+' } )
        }
    )

    # 書き忘れの組（共通の規則の TrailerRequirements に足される。警告だけ。OpenTouryoCodingAgentAssets#6）
    #   リスク「高」の名前空間（QUALITY.md 1 節の品質マップ）を fixed で変えたら、fixed の前に別モデルのレビュー（quality-reviewer。4 節）
    TrailerRequirements = @(
        @{
            Name    = 'Reviewed-with'
            Message = '(?i)\bfixed\s+#\d+'
            Paths   = @(
                '^root/programs/CS/Frameworks/Infrastructure/Public/Security/'
                '^root/programs/CS/Frameworks/Infrastructure/Framework/Authentication/'
            )
            Value   = '^(quality-reviewer|human)\b'
            Hint    = 'リスク「高」の名前空間（Public.Security・Framework.Authentication）なので、fixed の前に quality-reviewer で別モデルのレビューを回し、Reviewed-with: quality-reviewer (<モデル ID>) を書く（QUALITY.md 4 節）'
        }
    )
}
