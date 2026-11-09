# expectation-guard-source: OpenTouryoCodingAgentAssets@a81584579945 src/quality/ExpectationGuard/rules.common.psd1 sha256:9c30e5bd2199cb562d1f5d1b3fc427c64198847ec34e0fb5cea05df625e02a7d
# 期待値の変更の検出器の、共通の規則（OpenTouryoCodingAgentAssets#3）。
#   原本は OpenTouryoCodingAgentAssets の src/quality/ExpectationGuard/。導入先では写しであり、直さない。
#   リポジトリごとの規則は rules.psd1（見本は rules.sample.psd1）。配列はここに足され、値は rules.psd1 で上書きされる。
@{
    # 承認のトレーラー
    ApprovalTrailer = 'Expectation-Change'

    # 数えない行（コメント。コメント アウトは「消えた」と数える）
    CommentLine = '^\s*//'

    # 仕組み自体のファイル（変更・削除・名前の変更を、期待値の変更と同じに扱う。追加は数えない）。
    #   本体（CheckExpectationChange.ps1）と規則のファイル（本ファイル・rules.psd1）は、本体が自分で加える。
    Guard = @(
        '^\.githooks/'
        '^\.claude/settings\.json$'
        '^\.claude/hooks/'
        '^\.claude/agents/'   # サブ エージェント（レビュー役など）の定義。緩めればレビューも緩む（OpenTouryo#680・MultiPurposeAuthSite#347）
        '^\.github/workflows/expectation-change\.yml$'
    )

    # トレーラーの検査（OpenTouryoCodingAgentAssets#5）。
    #   Found-By: <手段> [<補足>] … バグの修正（fixed #NNN）を、どこで・何で見つけたか。
    #   値の最初の語が語彙に無ければ止める。fixed #NNN なのに無ければ警告に留める（バグかどうかは機械に分からない）。
    #   集計：git log --format='%h %(trailers:key=Found-By,valueonly,separator=|)' <範囲>
    #   rules.psd1 に同じ Name の規則を書くと、こちらを置き換える。
    TrailerRules = @(
        @{
            Name         = 'Found-By'
            # 語彙（この順に一覧を出す）
            Vocabulary   = @(
                'spec-check'
                'type-review'
                'test-added'
                'test-failure'
                'review-other-model'
                'review-same-model'
                'static-analysis'
                'downstream'
                'upstream'
                'user-report'
            )
            # 語の意味。値を誤ったとき・書き忘れたときに、語と並べて出す
            Meanings     = @{
                'spec-check'         = '原典（RFC・仕様）の確認で見つけた。根拠（[Basis]）を付ける作業を含む'
                'type-review'        = '型ごと・類似の不良の見直しで見つけた'
                'test-added'         = 'テスト（単体・E2E）を足して見つけた'
                'test-failure'       = '既存のテスト・CI の失敗で見つけた（断続的なものを含む）'
                'review-other-model' = '書いたモデルとは別のモデルのレビューで見つけた'
                'review-same-model'  = '書いたモデルのレビューで見つけた（別のセッションを含む）'
                'static-analysis'    = 'CodeQL などの静的解析で見つけた'
                'downstream'         = '利用する側（サンプル・汎用認証サイト・アプリ）の作業で見つけた（このリポジトリを使う側）'
                'upstream'           = '使っているライブラリの変更の取り込み・波及で見つけた（このリポジトリが使う側）'
                'user-report'        = '利用者からの報告'
            }
            RequiredWhen = '(?i)\bfixed\s+#\d+'
            MissingHint  = 'バグの修正なら、見つけた手段を書く（Found-By: <手段> [<補足>]）。機能の追加なら不要。'
        }
    )

    # 段 1：コマンドで書き換えそうな操作（2>$null・*>&1 のような、捨てるだけのリダイレクトは除く）。
    #   FileRules・GeneratedRules の CommandPattern と両方に当たると、人の確認付きにする（**推測である。** 漏れは段 2・段 3 が塞ぐ）。
    WriteInCommand = '(?<![0-9*])>(?!\s*(\$null|/dev/null|&))|\b(Set-Content|Add-Content|Out-File|Copy-Item|Move-Item|Remove-Item|Rename-Item|WriteAll(Text|Bytes|Lines))\b|(^|[\s;|&(])(cp|mv|rm|copy|move|del|tee)\s|\bsed\s+(-\w*\s+)*-i|\bgit\s+(checkout|restore|reset|stash|apply|am|cherry-pick)\b'
}
