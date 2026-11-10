# expectation-guard-source: OpenTouryoCodingAgentAssets@ade897953a72 src/quality/ExpectationGuard/rules.common.psd1 sha256:0e31b6bc86bd6be0479054d220efe4157e7d6f74c02bc78cbf186a78c6e371a4
# 期待値の変更の検出器の、共通の規則（OpenTouryoCodingAgentAssets#3）。
#   原本は OpenTouryoCodingAgentAssets の src/quality/ExpectationGuard/。導入先では写しであり、直さない。
#   リポジトリごとの規則は rules.psd1（見本は rules.sample.psd1）。配列はここに足され、値は rules.psd1 で上書きされる。
@{
    # 承認のトレーラー
    ApprovalTrailer = 'Expectation-Change'

    # 承認の理由の書式（OpenTouryoCodingAgentAssets#6）。どれにも当たらなければ警告（止めない）。
    #   根拠（Issue 番号・仕様の節）の無い理由は、突き合わせ役が「根拠」の観点で確かめられない。
    #   リポジトリの規則（rules.psd1）に書くと足される（例：MultiPurposeAuthSite のテストの ID 'RT-\d+'）。
    ApprovalReasonPatterns = @(
        '#\d+'                                                          # Issue 番号
        '\bRFC\s*\d+'                                                   # RFC
        '§\s*\d'                                                        # 節（§）
        '\d+(\.\d+)*\s*節'                                              # 節（日本語）
        '\b(OIDC|OpenID|FAPI|JARM|OAuth|PKCE|SAML|JWT|JWS|JWE|DPoP)\b[^。]*?\d+(\.\d+)+'  # 仕様の名前と節番号
    )
    ApprovalReasonHint = '理由に、根拠の Issue 番号（#NNN）か仕様の節（RFC 7636 4.2 など）を書く'

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

    # トレーラーの検査（OpenTouryoCodingAgentAssets#5・#12）。
    #   Found-By: <手段> [<補足>]         … バグの修正（fixed #NNN）を、どこで・何で見つけたか
    #   Tested-with: <種類>[, <種類>]    … コミットの前に流したテストの種類
    #   Reviewed-with: <役> [(<モデル ID>)] … レビューした役（役の定義の名前）。役ごとに行を分ける
    #   **記録であって証明ではない。** 人がコミットのときに経緯と合っているかを見る。
    #   値の最初の語（List なら「,」で分けた項目ごと）が語彙に無ければ止める。RequiredWhen に当たるのに無ければ警告に留める。
    #   git が読まない書き方（最後の段落に無い・全角の「：」・名前の綴りの違い）も止める。集計に載らないため。
    #   止めたときは、書いた行・直した行・書き方（Syntax）・例（Example）・語彙と意味（Meanings）を出す。
    #   集計：git log --format='%h %(trailers:key=Found-By,valueonly,separator=|) %(trailers:key=Reviewed-with,valueonly,separator=|)' <範囲>
    #   rules.psd1 に同じ Name の規則を書くと、こちらを置き換える。
    TrailerRules = @(
        @{
            Name         = 'Found-By'
            Syntax       = 'Found-By: <手段> [<補足>]（手段が複数なら行を分ける）'
            Placeholder  = '<手段>'
            Example      = 'Found-By: type-review (#653 の派生)'
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
        @{
            Name         = 'Tested-with'
            Syntax       = 'Tested-with: <種類>[, <種類>]（種類ごとに補足を括弧で付けてよい）'
            Placeholder  = '<種類>'
            Example      = 'Tested-with: unit, e2e (net48)'
            List         = $true
            Vocabulary   = @(
                'unit'
                'e2e'
                'golden'
                'manual'
                'none'
            )
            Meanings     = @{
                'unit'   = '単体テスト（NUnit など。B層のルート メソッドの単体テストを含む）'
                'e2e'    = 'E2E（画面・プロトコル。Playwright など）'
                'golden' = 'Golden（出力を丸ごと写した結果との突き合わせ）'
                'manual' = '人の手作業の確認（画面を開いて見た など）'
                'none'   = 'テストを流していない（文書だけの変更など。書き忘れと区別する）'
            }
        }
        @{
            Name                   = 'Reviewed-with'
            Syntax                 = 'Reviewed-with: <役> [(<モデル ID>)]（役ごとに行を分ける）'
            Placeholder            = '<役>'
            Example                = 'Reviewed-with: quality-reviewer (claude-sonnet-5-5)'
            # 役の定義のファイルの名前を語にする（確かめるコミットの木にあるもの）。役を足しても規則を直さずに済む
            VocabularyFiles        = @('.claude/agents/*.md')
            VocabularyFilesMeaning = '役の定義（{0}）'
            Vocabulary             = @('human')
            Meanings               = @{
                'human' = '人のレビュー（役の定義を使わずに、人が差分を見た）'
            }
        }
    )

    # トレーラーの書き忘れ（OpenTouryoCodingAgentAssets#6）。止めない（警告）。
    #   組の中の条件（Message：メッセージ・Trailer：ほかのトレーラーがある・Paths：変えたファイル）がすべて当たるのに、
    #   Name のトレーラー（Value があれば、それに当たる値）が無ければ警告する。IgnoreLine に当たる行（コメント・空行）だけの変更は数えない。
    #   リポジトリの規則（rules.psd1）に書くと、組が足される。例：リスク「高」の名前空間を fixed で変えたら、別モデルのレビュー
    #     @{ Name = 'Reviewed-with'; Message = '(?i)\bfixed\s+#\d+'; Paths = @('^src/Auth/'); Hint = '…' }
    TrailerRequirements = @(
        @{
            Name       = 'Tested-with'
            Paths      = @('\.(cs|vb)$')
            IgnoreLine = '^\s*(//|''|/\*|\*|$)'
            Hint       = '流したテストの種類を書く。流していなければ Tested-with: none（書き忘れと区別する）'
        }
        @{
            Name    = 'Reviewed-with'
            Trailer = 'Expectation-Change'
            Value   = '^opentouryo-expectation-reviewer\b'
            Hint    = 'コミットの前に、突き合わせ役（opentouryo-expectation-reviewer）で理由と差分を突き合わせ、Reviewed-with: opentouryo-expectation-reviewer (<モデル ID>) を書く'
        }
    )

    # 要確認（OpenTouryoCodingAgentAssets#3 の段階 5）。止めない・トレーラーを求めない。一覧と注釈に出すだけ。
    #   既存のテストを弱めないので期待値の変更には現れないが、互換のための判断なのでレビューの目に入れる。
    #   ファイルごとに「足した数 − 消した数」が残れば出す（移しただけ・消しただけなら出ない）。コメントの行は数えない。
    ReviewRules = @(
        @{
            Name    = '[Obsolete] の追加'
            Paths   = @('\.cs$')
            # [Obsolete]・[Obsolete("…")]・[System.Obsolete]・[ObsoleteAttribute]・[EditorBrowsable(…), Obsolete] など
            Pattern = '\[\s*(\w+\s*(\([^)]*\))?\s*,\s*)*(System\s*\.\s*)?Obsolete(Attribute)?\b'
            Hint    = '旧版の振る舞いを温存した。新しい版へ移る道と、旧版を消す時期を確かめる'
        }
        @{
            Name    = '#pragma warning disable CS0618 の追加'
            Paths   = @('\.cs$')
            # CS0618 を含む #pragma warning disable と、番号の無い（すべての警告を消す）#pragma warning disable。
            #   C# は CS を付けない番号（disable 618）も受ける（MultiPurposeAuthSite 399ac20b）
            Pattern = '^\s*#\s*pragma\s+warning\s+disable(\s*$|\s*//|[^/]*\b(CS)?0*618\b)'
            Hint    = 'そのファイルの、ほかの [Obsolete] の警告も消える。範囲を restore で閉じているか、本当に旧版を使う必要があるかを確かめる'
        }
    )

    # 段 1：コマンドで書き換えそうな操作（2>$null・*>&1 のような、捨てるだけのリダイレクトは除く）。
    #   FileRules・GeneratedRules の CommandPattern と両方に当たると、人の確認付きにする（**推測である。** 漏れは段 2・段 3 が塞ぐ）。
    WriteInCommand = '(?<![0-9*])>(?!\s*(\$null|/dev/null|&))|\b(Set-Content|Add-Content|Out-File|Copy-Item|Move-Item|Remove-Item|Rename-Item|WriteAll(Text|Bytes|Lines))\b|(^|[\s;|&(])(cp|mv|rm|copy|move|del|tee)\s|\bsed\s+(-\w*\s+)*-i|\bgit\s+(checkout|restore|reset|stash|apply|am|cherry-pick)\b'
}
