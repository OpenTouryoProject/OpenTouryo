# expectation-guard-source: OpenTouryoCodingAgentAssets@b9da2ca0cc9d src/quality/ExpectationGuard/tests/rules.test.psd1 sha256:524776bd95532d8c16221668ef103f2edd9f230087e187a2dcd528a912ba40bd
# 判定の用例で使う規則（すべての種類を 1 つずつ持つ）。用例のリポジトリでは guard/rules.psd1 として置く。
@{
    Guard = @(
        '^guard/TestComparer\.ps1$'
    )

    LineRules = @(
        @{
            Name  = 'テストの削除'
            Paths = @('^tests/.+\.cs$')
            Count = 'Removed'
            Match = @(
                @{ Pattern = '[\[,]\s*(Test|TestCase|TestCaseSource|Fact|Theory|InlineData)\s*[\],(]' }
            )
        }
        @{
            Name  = 'Skip・Ignore の追加'
            Paths = @('^tests/.+\.cs$')
            Count = 'Added'
            Match = @(
                @{ Pattern = '[\[,]\s*(Ignore|Explicit)\s*[\],(]|\bAssert\s*\.\s*Ignore\s*\(|[\[,(]\s*Skip\s*=' }
            )
        }
        @{
            Name  = '根拠の削除'
            Paths = @('^tests/.+\.cs$')
            Count = 'Removed'
            Match = @(
                @{ Pattern = '[\[,]\s*Basis\s*\(' }
            )
        }
        @{
            Name  = '検証の削除'
            Paths = @('^tests/.+\.cs$')
            Count = 'Removed'
            Match = @(
                @{ Pattern = '\.\s*Verify\s*\(' }
                @{ Pattern = '\bAssert\s*\.'; Paths = @('^tests/Unit/') }
            )
        }
    )

    FileRules = @(
        @{
            Name           = 'Golden'
            Paths          = @('^golden/.+\.txt$')
            Comparer       = 'guard/TestComparer.ps1'
            CommandPattern = 'golden[\\/]'
        }
    )

    KeyedArgumentRules = @(
        @{
            Paths       = @('^tests/.+\.cs$')
            Call        = 'Report'
            KeyIndex    = 0
            ValueIndex  = 3
            MinArgs     = 4
            RemovedName = '根拠の削除（Report）'
            ChangedName = '根拠の変更（Report）'
        }
    )

    TrailerRules = @(
        @{
            Name         = 'Found-By'
            Vocabulary   = @('spec-check', 'type-review', 'test-added')
            RequiredWhen = '(?i)\bfixed\s+#\d+'
            MissingHint  = 'Found-By: <手段>'
        }
    )

    GeneratedRules = @(
        @{
            Name           = 'TESTCASES.md'
            Generated      = 'tests/TESTCASES.md'
            Sources        = @('^tests/E2E/.+\.cs$')
            Items          = @(
                @{ Pattern = '(?<![\w])Report\s*\('; Take = 'All'; Label = 'Report' }
                @{ Pattern = '\.\s*(?<label>Step|Verify)\s*\('; Take = 'First' }
                @{ Pattern = 'public\s+(?:async\s+)?(?:Task|void)\s+(?<name>\w+)\s*\('; Take = 'Name'; Label = 'test' }
            )
            ExemptTrailer  = 'TestCases-Unchanged'
            Hint           = @('テストを回して TESTCASES.md を作り直してください。')
            CommandPattern = 'TESTCASES\.md'
        }
    )
}
