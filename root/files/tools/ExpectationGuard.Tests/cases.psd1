# expectation-guard-source: OpenTouryoCodingAgentAssets@b9da2ca0cc9d src/quality/ExpectationGuard/tests/cases.psd1 sha256:77f8981d2ef6f8ce08819c1543eae970362d65765bbf96026000a9516c99946e
# 判定の用例（OpenTouryoCodingAgentAssets#3）。「この差分なら検出する／しない」を、文章ではなくテストで固定する。
#
#   各用例は、一時のリポジトリで次を行う。
#     1. 最初のコミット：Fixtures のファイル（Base の対応）と、検出器（guard/）を置く
#        （GuardAt を書くと、検出器は Commits のその番目〔1 から〕で足す。導入より前のコミットの用例）
#     2. Commits を順にコミットする（Mode = Staged なら、最後の 1 つはステージだけ）
#     3. Mode で検出器を動かし、Expect と比べる
#   Mode : Commit（既定。-Commit HEAD）・Staged・Range（最初のコミット..HEAD）・Generated（Range ＋ -CheckGenerated）・PreToolUse
#   Expect : Exit（終了コード）・Kinds（検出の種類。順は問わない）・Warnings（警告の件数）・Ask（PreToolUse で ask を返すか）
#   ファイルの中身の「@fixture:名前」は Fixtures のその中身。
#
#   Category
#     検出する / 検出しない        … 正しい判定
#     既知の取りこぼし / 過検出（既知）… 今の判定を固定する。**直したら、用例も直す**（直ったことが分かる）
@{
    Base = @{
        'tests/FooTests.cs'      = '@fixture:Foo'
        'tests/Unit/BarTests.cs' = '@fixture:Bar'
        'tests/E2E/LoginTests.cs' = '@fixture:Login'
        'tests/TESTCASES.md'     = "# TESTCASES`n"
        'golden/Result1.txt'     = "Time: 10:00`nOK 1`n"
        '.githooks/commit-msg'   = "#!/bin/sh`nexit 0`n"
        '.claude/agents/reviewer.md' = "---`nname: reviewer`n---`nテストを自分で実行する`n"
        '.claude/agents/opentouryo-expectation-reviewer.md' = "---`nname: opentouryo-expectation-reviewer`n---`n理由と差分を突き合わせる`n"
    }

    Fixtures = @{
        Foo = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }

    [Test]
    public void B() { Assert.That(2, Is.EqualTo(2)); }
}
'@
        Bar = @'
public class BarTests
{
    [Test]
    public void C()
    {
        Assert.That(3, Is.EqualTo(3));
        Assert.That(4, Is.EqualTo(4));
    }
}
'@
        Login = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        r.Verify("表示される", true);
        Assert.NotNull(r);
    }
}
'@
    }

    Cases = @(
        # ==============================================================
        # 検出する
        # ==============================================================
        @{
            Id = 'D01'; Category = '検出する'; Title = 'テストの削除'
            Commits = @(@{ Message = 'B を消す'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('テストの削除') }
        }
        @{
            Id = 'D02'; Category = '検出する'; Title = 'Ignore の追加（属性の並び）'
            Commits = @(@{ Message = 'B を無効にする'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }

    [Test, Ignore("あとで")]
    public void B() { Assert.That(2, Is.EqualTo(2)); }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('Skip・Ignore の追加') }
        }
        @{
            Id = 'D03'; Category = '検出する'; Title = 'Skip の追加（xUnit）'
            Commits = @(@{ Message = 'Login を飛ばす'; Files = @{ 'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact(Skip = "環境が無い")]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        r.Verify("表示される", true);
        Assert.NotNull(r);
    }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('Skip・Ignore の追加') }
        }
        @{
            Id = 'D04'; Category = '検出する'; Title = '根拠の削除（[Basis]）'
            Commits = @(
                @{ Message = '根拠を付ける'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test, Basis("RFC 6749 4.1.2")]
    public void A() { Assert.That(1, Is.EqualTo(1)); }

    [Test]
    public void B() { Assert.That(2, Is.EqualTo(2)); }
}
'@ } }
                @{ Message = '根拠を外す'; Files = @{ 'tests/FooTests.cs' = '@fixture:Foo' } }
            )
            Expect = @{ Exit = 1; Kinds = @('根拠の削除') }
        }
        @{
            Id = 'D05'; Category = '検出する'; Title = '検証の削除（単体テストの Assert）'
            Commits = @(@{ Message = 'Assert を 1 つ消す'; Files = @{ 'tests/Unit/BarTests.cs' = @'
public class BarTests
{
    [Test]
    public void C()
    {
        Assert.That(3, Is.EqualTo(3));
    }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('検証の削除') }
        }
        @{
            Id = 'D06'; Category = '検出する'; Title = '検証の削除（E2E の Verify）'
            Commits = @(@{ Message = 'Verify を消す'; Files = @{ 'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        Assert.NotNull(r);
    }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('検証の削除') }
        }
        @{
            Id = 'D07'; Category = '検出する'; Title = '根拠の変更（Report の第 4 引数。複数行）'
            Commits = @(@{ Message = '根拠を書き換える'; Files = @{ 'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.2");
        r.Step("開く");
        r.Verify("表示される", true);
        Assert.NotNull(r);
    }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('根拠の変更（Report）') }
        }
        @{
            Id = 'D08'; Category = '検出する'; Title = '仕組みのファイルの変更（.githooks）'
            Commits = @(@{ Message = 'フックを緩める'; Files = @{ '.githooks/commit-msg' = "#!/bin/sh`n# off`nexit 0`n" } })
            Expect = @{ Exit = 1; Kinds = @('仕組みの変更') }
        }
        @{
            Id = 'D09'; Category = '検出する'; Title = '仕組みのファイルの変更（規則のファイル自身）'
            Commits = @(@{ Message = '規則を消す'; Files = @{ 'guard/rules.psd1' = "@{ }`n" } })
            Expect = @{ Exit = 1; Kinds = @('仕組みの変更') }
        }
        @{
            Id = 'D10'; Category = '検出する'; Title = '仕組みのファイルの削除（共通の規則）'
            Commits = @(@{ Message = '共通の規則を消す'; Delete = @('guard/rules.common.psd1') })
            Expect = @{ Exit = 1; Kinds = @('仕組みの変更') }
        }
        @{
            Id = 'D19'; Category = '検出する'; Title = '仕組みのファイルの変更（.claude/agents のサブ エージェントの定義）'
            Commits = @(@{ Message = 'レビュー役を緩める'; Files = @{ '.claude/agents/reviewer.md' = "---`nname: reviewer`n---`n報告を信じる`n" } })
            Expect = @{ Exit = 1; Kinds = @('仕組みの変更') }
        }
        @{
            Id = 'D11'; Category = '検出する'; Title = 'Golden の変更（正規化しても差分が残る）'
            Commits = @(@{ Message = '期待値を変える'; Files = @{ 'golden/Result1.txt' = "Time: 11:00`nOK 2`n" } })
            Expect = @{ Exit = 1; Kinds = @('Golden の変更') }
        }
        @{
            Id = 'D12'; Category = '検出する'; Title = 'Golden の追加・削除'
            Commits = @(@{ Message = '期待値のファイルを入れ替える'; Files = @{ 'golden/Result2.txt' = "OK 1`n" }; Delete = @('golden/Result1.txt') })
            Expect = @{ Exit = 1; Kinds = @('Golden の追加', 'Golden の削除') }
        }
        @{
            Id = 'D13'; Category = '検出する'; Title = 'コメント アウトは消えたと数える'
            Commits = @(@{ Message = 'B を止める'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }

    // [Test]
    public void B() { Assert.That(2, Is.EqualTo(2)); }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('テストの削除') }
        }
        @{
            Id = 'D14'; Category = '検出する'; Title = 'ステージした変更（commit-msg フック）'
            Mode = 'Staged'
            Commits = @(@{ Message = 'B を消す'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('テストの削除') }
        }
        @{
            Id = 'D15'; Category = '検出する'; Title = '語彙に無いトレーラーの値（Found-By）'
            Commits = @(@{ Message = "fixed #12 直した`n`nFound-By: gut-feeling"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 1; Kinds = @() }
        }
        @{
            Id = 'D18'; Category = '検出する'; Title = 'トレーラーの規則は同じ Name で置き換わる（共通の語彙にあっても、リポジトリの語彙に無ければ止める）'
            Commits = @(@{ Message = "fixed #12 直した`n`nFound-By: user-report`nTested-with: none"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 1; Kinds = @(); Warnings = 0 }
            Note = 'W01 の警告が 1 件（共通とリポジトリで二重に数えない）ことと併せて見る'
        }
        @{
            Id = 'D16'; Category = '検出する'; Title = '生成物の作り直しの漏れ'
            Mode = 'Generated'
            Commits = @(@{ Message = '手順を足す'; Files = @{ 'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        r.Step("入力する");
        r.Verify("表示される", true);
        Assert.NotNull(r);
    }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('TESTCASES.md の作り直しの漏れ') }
        }
        @{
            Id = 'D17'; Category = '検出する'; Title = '範囲（導入の後のコミットは見る）'
            Mode = 'Range'
            Commits = @(
                @{ Message = 'A を足す'; Files = @{ 'src/A.cs' = "class A {}`n" } }
                @{ Message = 'B を消す'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@ } }
            )
            Expect = @{ Exit = 1; Kinds = @('テストの削除') }
        }

        # ==============================================================
        # 検出しない（正しく）
        # ==============================================================
        @{
            Id = 'N01'; Category = '検出しない'; Title = '属性の引数の書き直し'
            Commits = @(
                @{ Message = 'TestCase にする'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [TestCase(1)]
    public void A(int x) { Assert.That(x, Is.EqualTo(1)); }
}
'@ } }
                @{ Message = '引数を変える'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [TestCase(2)]
    public void A(int x) { Assert.That(x, Is.GreaterThan(0)); }
}
'@ } }
            )
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N02'; Category = '検出しない'; Title = 'テストの追加'
            Commits = @(@{ Message = 'C を足す'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }

    [Test]
    public void B() { Assert.That(2, Is.EqualTo(2)); }

    [Test]
    public void C() { Assert.That(3, Is.EqualTo(3)); }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N03'; Category = '検出しない'; Title = '仕組みのファイルの追加'
            Commits = @(@{ Message = 'フックを足す'; Files = @{ '.claude/hooks/Other.ps1' = "exit 0`n" } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N04'; Category = '検出しない'; Title = 'Golden の、正規化で消える差分'
            Commits = @(@{ Message = '回し直した'; Files = @{ 'golden/Result1.txt' = "Time: 23:59`nOK 1`n" } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N05'; Category = '検出しない'; Title = 'Report をファイルをまたいで移す（根拠は同じ。折り返しも違う）'
            Commits = @(@{ Message = '移す'; Files = @{
                'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = LoginReports.Create();
        r.Step("開く");
        r.Verify("表示される", true);
        Assert.NotNull(r);
    }
}
'@
                'tests/E2E/LoginReports.cs' = @'
public static class LoginReports
{
    public static TestReport Create() => Report("RT-1", "ログイン", "正常系", "RFC 6749 4.1");
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N06'; Category = '検出しない'; Title = 'トレーラーで承認'
            Commits = @(@{ Message = "B を消す`n`nExpectation-Change: 仕様の変更（#1）"; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @('テストの削除') }
        }
        @{
            Id = 'N07'; Category = '検出しない'; Title = 'E2E の Assert の削除（前提の確認であって検証ではない）'
            Commits = @(@{ Message = 'Assert を消す'; Files = @{ 'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        r.Verify("表示される", true);
    }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N08'; Category = '検出しない'; Title = '// で始まる行の Ignore は数えない'
            Commits = @(@{ Message = 'メモ'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }

    // [Ignore] にしないこと
    [Test]
    public void B() { Assert.That(2, Is.EqualTo(2)); }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N09'; Category = '検出しない'; Title = '語彙にあるトレーラーの値'
            Commits = @(@{ Message = "fixed #12 直した`n`nFound-By: type-review (#11 の派生)`nTested-with: none"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 0 }
        }
        @{
            Id = 'N10'; Category = '検出しない'; Title = '生成物を作り直した'
            Mode = 'Generated'
            Commits = @(@{ Message = '手順を足す'; Files = @{
                'tests/TESTCASES.md' = "# TESTCASES`n- 入力する`n"
                'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        r.Step("入力する");
        r.Verify("表示される", true);
        Assert.NotNull(r);
    }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N11'; Category = '検出しない'; Title = '生成物に現れない変更（免除のトレーラー）'
            Mode = 'Generated'
            Commits = @(@{ Message = "手順を足す`n`nTestCases-Unchanged: 通しの既定から外れるテスト"; Files = @{ 'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        r.Step("入力する");
        r.Verify("表示される", true);
        Assert.NotNull(r);
    }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'N12'; Category = '検出しない'; Title = '生成物に現れない変更（条件式だけ）'
            Mode = 'Generated'
            Commits = @(@{ Message = '条件を変える'; Files = @{ 'tests/E2E/LoginTests.cs' = @'
public class LoginTests
{
    [Fact]
    public void Login()
    {
        var r = Report("RT-1", "ログイン", "正常系",
            "RFC 6749 4.1");
        r.Step("開く");
        r.Verify("表示される", 1 == 1);
        Assert.NotNull(r);
    }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
            Note = 'Verify は最初の引数（文面）だけを比べる'
        }
        @{
            Id = 'N13'; Category = '検出しない'; Title = '導入より前のコミットは見ない（範囲）'
            Mode = 'Range'; GuardAt = 2
            Commits = @(
                @{ Message = 'B を消す（導入の前）'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@ } }
                @{ Message = '検出器を入れる' }
            )
            Expect = @{ Exit = 0; Kinds = @() }
        }

        # ==============================================================
        # トレーラーの書き方・語彙（#12。git と同じ読み方で読む）
        # ==============================================================
        @{
            Id = 'T01'; Category = '検出する'; Title = '語彙に無いテストの種類（Tested-with。List の項目ごと）'
            Commits = @(@{ Message = "足した`n`nTested-with: unit, gut"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 1; Kinds = @(); Errors = 1 }
        }
        @{
            Id = 'T02'; Category = '検出しない'; Title = '語彙にあるテストの種類（括弧の中の「,」では分けない）'
            Commits = @(@{ Message = "足した`n`nTested-with: unit, e2e (net48, net10.0)"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Errors = 0 }
        }
        @{
            Id = 'T03'; Category = '検出する'; Title = '役の定義に無いレビュー役（Reviewed-with）'
            Commits = @(@{ Message = "足した`n`nReviewed-with: quality-reviewer (claude-sonnet-5-5)"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 1; Kinds = @(); Errors = 1 }
            Note = '用例のリポジトリの役の定義は .claude/agents/reviewer.md だけ'
        }
        @{
            Id = 'T04'; Category = '検出しない'; Title = '役の定義にあるレビュー役と人（行を分ける）'
            Commits = @(@{ Message = "足した`n`nReviewed-with: reviewer (claude-sonnet-5-5)`nReviewed-with: human"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Errors = 0 }
        }
        @{
            Id = 'T05'; Category = '検出しない'; Title = '同じコミットで足した役の定義（確かめるコミットの木から語彙を取る）'
            Commits = @(@{ Message = "役を足した`n`nReviewed-with: new-reviewer"; Files = @{ '.claude/agents/new-reviewer.md' = "---`nname: new-reviewer`n---`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Errors = 0 }
        }
        @{
            Id = 'T06'; Category = '検出しない'; Title = '同じ段で足した役の定義（ステージ。索引から語彙を取る）'
            Mode = 'Staged'
            Commits = @(@{ Message = "役を足した`n`nReviewed-with: new-reviewer"; Files = @{ '.claude/agents/new-reviewer.md' = "---`nname: new-reviewer`n---`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Errors = 0 }
        }
        @{
            Id = 'T07'; Category = '検出する'; Title = '最後の段落に無いトレーラー（Co-Authored-By の前に空行）'
            Commits = @(@{ Message = "fixed #12 直した`n`nFound-By: spec-check`n`nTested-with: none`nCo-Authored-By: A <a@example.com>"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 1; Kinds = @(); Errors = 1; Warnings = 0 }
            Note = '書いたが読まれない行は、直す行に出す（書き忘れの警告にはしない）'
        }
        @{
            Id = 'T08'; Category = '検出する'; Title = '最後の段落に本文の行が混じる'
            Commits = @(@{ Message = "fixed #12 直した`n`n本文。`nFound-By: spec-check"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 1; Kinds = @(); Errors = 1 }
        }
        @{
            Id = 'T09'; Category = '検出する'; Title = '全角の「：」・名前の綴りの違い'
            Commits = @(@{ Message = "fixed #12 直した`n`nFound-By：spec-check`nTested-wiht: unit"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 1; Kinds = @(); Errors = 2 }
        }
        @{
            Id = 'T10'; Category = '検出しない'; Title = '名前の大文字・小文字の違い（git は区別しない）'
            Commits = @(@{ Message = "fixed #12 直した`n`nfound-by: spec-check`nTested-With: unit"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Errors = 0; Warnings = 0 }
        }
        @{
            Id = 'T11'; Category = '検出しない'; Title = '件名と本文の文章の中の「Found-By:」'
            Commits = @(@{ Message = "Found-By: の語彙を足した`n`nFound-By: の語彙に 1 語を足した。`n理由は #5 を見る。"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Errors = 0 }
        }
        @{
            Id = 'T12'; Category = '検出する'; Title = '最後の段落に無い Expectation-Change は承認にしない'
            Commits = @(@{ Message = "B を消す`n`nExpectation-Change: 仕様の変更（#1）`n`nCo-Authored-By: A <a@example.com>"; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('テストの削除'); Errors = 1 }
        }
        @{
            Id = 'T13'; Category = '検出する'; Title = '値の無い Expectation-Change'
            Commits = @(@{ Message = "B を消す`n`nExpectation-Change:"; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('テストの削除'); Errors = 1 }
        }

        # ==============================================================
        # 警告（止めない）
        # ==============================================================
        @{
            Id = 'W01'; Category = '警告'; Title = 'fixed #NNN に Found-By が無い'
            Commits = @(@{ Message = "fixed #12 直した`n`nTested-with: none"; Files = @{ 'src/A.cs' = "class A {}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 1 }
        }

        # ==============================================================
        # 要確認（止めない・トレーラーを求めない。一覧に出すだけ）
        # ==============================================================
        @{
            Id = 'R01'; Category = '要確認'; Title = '[Obsolete] の追加（旧版をオーバーロードで温存）'
            Commits = @(@{ Message = '旧版を残す'; Files = @{ 'src/A.cs' = "class A`n{`n    [Obsolete(""Use B(int)."")]`n    public void B() { }`n    public void B(int x) { }`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Review = 1; ReviewKinds = @('[Obsolete] の追加') }
        }
        @{
            Id = 'R02'; Category = '要確認'; Title = '[Obsolete] の追加（属性の並び）'
            Commits = @(@{ Message = '旧版を残す'; Files = @{ 'src/A.cs' = "class A`n{`n    [EditorBrowsable(EditorBrowsableState.Never), Obsolete]`n    public void B() { }`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Review = 1; ReviewKinds = @('[Obsolete] の追加') }
        }
        @{
            Id = 'R03'; Category = '要確認'; Title = '#pragma warning disable CS0618 の追加'
            Commits = @(@{ Message = '旧版を呼ぶテスト'; Files = @{ 'tests/Unit/OldTests.cs' = "#pragma warning disable CS0612, CS0618`npublic class OldTests { }`n#pragma warning restore CS0612, CS0618`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Review = 1; ReviewKinds = @('#pragma warning disable CS0618 の追加') }
        }
        @{
            Id = 'R04'; Category = '要確認'; Title = '番号の無い #pragma warning disable の追加（すべての警告を消す）'
            Commits = @(@{ Message = '警告を消す'; Files = @{ 'tests/Unit/OldTests.cs' = "#pragma warning disable`npublic class OldTests { }`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Review = 1; ReviewKinds = @('#pragma warning disable CS0618 の追加') }
        }
        @{
            Id = 'R05'; Category = '要確認'; Title = 'コメントの中の [Obsolete]・ほかの番号の #pragma は出さない'
            Commits = @(@{ Message = 'コメント'; Files = @{ 'src/A.cs' = "class A`n{`n    // [Obsolete] は付けない`n#pragma warning disable CS0168`n    public void B() { }`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Review = 0 }
        }
        @{
            Id = 'R06'; Category = '要確認'; Title = '#pragma を消した（出さない）'
            Commits = @(
                @{ Message = '足す'; Files = @{ 'tests/Unit/OldTests.cs' = "#pragma warning disable CS0618`npublic class OldTests { }`n" } }
                @{ Message = '消す'; Files = @{ 'tests/Unit/OldTests.cs' = "public class OldTests { }`n" } }
            )
            Expect = @{ Exit = 0; Kinds = @(); Review = 0 }
        }
        @{
            Id = 'R07'; Category = '要確認'; Title = '[Obsolete] を同じファイルの中で移した（出さない）'
            Commits = @(
                @{ Message = '足す'; Files = @{ 'src/A.cs' = "class A`n{`n    [Obsolete]`n    public void B() { }`n    public void C() { }`n}`n" } }
                @{ Message = '並べ替え'; Files = @{ 'src/A.cs' = "class A`n{`n    public void C() { }`n    [Obsolete]`n    public void B() { }`n}`n" } }
            )
            Expect = @{ Exit = 0; Kinds = @(); Review = 0 }
        }
        @{
            Id = 'R08'; Category = '要確認'; Title = '要確認と期待値の変更が同じコミット（期待値の変更は止まる。要確認も出る）'
            Commits = @(@{ Message = 'B を消し、旧版を残す'; Files = @{
                'tests/FooTests.cs' = "public class FooTests`n{`n    [Test]`n    public void A() { Assert.That(1, Is.EqualTo(1)); }`n}`n"
                'src/A.cs' = "class A`n{`n    [Obsolete]`n    public void B() { }`n}`n"
            } })
            Expect = @{ Exit = 1; Kinds = @('テストの削除'); Review = 1; ReviewKinds = @('[Obsolete] の追加') }
        }
        @{
            Id = 'R09'; Category = '要確認'; Title = 'CS を付けない番号の #pragma warning disable 618 の追加（MultiPurposeAuthSite 399ac20b）'
            Commits = @(@{ Message = '旧版を呼ぶ'; Files = @{ 'src/A.cs' = "#pragma warning disable 618`nclass A { }`n#pragma warning restore 618`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Review = 1; ReviewKinds = @('#pragma warning disable CS0618 の追加') }
        }
        @{
            Id = 'R10'; Category = '要確認'; Title = 'DiffScope の外の [Obsolete] も出す（DiffScope はテストの在りか。MultiPurposeAuthSite a4ea8acc）'
            Commits = @(
                @{ Message = "テストの下だけを読む`n`nExpectation-Change: DiffScope を tests/ にした"; Files = @{ 'guard/rules.psd1' = "@{`n    DiffScope = @('tests/')`n}`n" } }
                @{ Message = '旧版を残す'; Files = @{ 'src/A.cs' = "class A`n{`n    [Obsolete]`n    public void B() { }`n}`n" } }
            )
            Expect = @{ Exit = 0; Kinds = @(); Review = 1; ReviewKinds = @('[Obsolete] の追加') }
        }
        @{
            Id = 'R11'; Category = '要確認'; Title = 'ReviewScope の外は出さない（範囲は導入先が選ぶ）'
            Commits = @(
                @{ Message = "要確認は src の下だけ`n`nExpectation-Change: ReviewScope を src/ にした"; Files = @{ 'guard/rules.psd1' = "@{`n    ReviewScope = @('src/')`n}`n" } }
                @{ Message = '旧版を残す'; Files = @{
                    'src/A.cs' = "class A`n{`n    [Obsolete]`n    public void B() { }`n}`n"
                    'vendor/C.cs' = "class C`n{`n    [Obsolete]`n    public void D() { }`n}`n"
                } }
            )
            Expect = @{ Exit = 0; Kinds = @(); Review = 1; ReviewKinds = @('[Obsolete] の追加') }
        }

        # ==============================================================
        # 承認の理由の書式（#6。止めない。Issue 番号・仕様の節が無ければ警告）
        # ==============================================================
        @{
            Id = 'A01'; Category = '理由の書式'; Title = 'Issue 番号・仕様の節の無い理由は警告（承認にはなる）'
            Commits = @(@{ Message = "B を消す`n`nExpectation-Change: 仕様の変更`nTested-with: unit`nReviewed-with: opentouryo-expectation-reviewer (claude-sonnet-5-5)"; Files = @{ 'tests/FooTests.cs' = "public class FooTests`n{`n    [Test]`n    public void A() { Assert.That(1, Is.EqualTo(1)); }`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @('テストの削除'); Warnings = 1 }
        }
        @{
            Id = 'A02'; Category = '理由の書式'; Title = 'Issue 番号のある理由は警告しない'
            Commits = @(@{ Message = "B を消す`n`nExpectation-Change: 仕様の変更（#123）`nTested-with: unit`nReviewed-with: opentouryo-expectation-reviewer (claude-sonnet-5-5)"; Files = @{ 'tests/FooTests.cs' = "public class FooTests`n{`n    [Test]`n    public void A() { Assert.That(1, Is.EqualTo(1)); }`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @('テストの削除'); Warnings = 0 }
        }
        @{
            Id = 'A03'; Category = '理由の書式'; Title = '仕様の節のある理由は警告しない（RFC・仕様の名前と節番号）'
            Commits = @(@{ Message = "B を消す`n`nExpectation-Change: RFC 7636 4.2 に合わせた`nExpectation-Change: JARM 2.1 の iss を必須にした`nTested-with: unit`nReviewed-with: opentouryo-expectation-reviewer (claude-sonnet-5-5)"; Files = @{ 'tests/FooTests.cs' = "public class FooTests`n{`n    [Test]`n    public void A() { Assert.That(1, Is.EqualTo(1)); }`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @('テストの削除'); Warnings = 0 }
        }
        @{
            Id = 'A04'; Category = '理由の書式'; Title = 'リポジトリの規則で書き方を足せる（テストの ID RT-NNN）'
            Commits = @(@{ Message = "規則を足す`n`nExpectation-Change: RT-352.1 の Skip に合わせた`nReviewed-with: opentouryo-expectation-reviewer";Files = @{ 'guard/rules.psd1' = "@{`n    ApprovalReasonPatterns = @('\bRT-\d+')`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @('仕組みの変更'); Warnings = 0 }
        }

        # ==============================================================
        # トレーラーの書き忘れ（#6。TrailerRequirements。止めない）
        # ==============================================================
        @{
            Id = 'M01'; Category = '書き忘れ'; Title = 'コードを変えたのに Tested-with が無い（警告）'
            Commits = @(@{ Message = '直した'; Files = @{ 'src/A.cs' = "class A { int x = 1; }`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 1 }
        }
        @{
            Id = 'M02'; Category = '書き忘れ'; Title = 'Tested-with: none で満たせる'
            Commits = @(@{ Message = "直した`n`nTested-with: none"; Files = @{ 'src/A.cs' = "class A { int x = 1; }`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 0 }
        }
        @{
            Id = 'M03'; Category = '書き忘れ'; Title = 'コメント・空行だけの変更は数えない（C# の //・VB の ''）'
            Commits = @(
                @{ Message = "足す`n`nTested-with: none"; Files = @{ 'src/A.cs' = "class A`n{`n}`n"; 'src/B.vb' = "Class B`nEnd Class`n" } }
                @{ Message = 'コメントを足した'; Files = @{ 'src/A.cs' = "// A の説明`nclass A`n{`n    /// <summary>説明</summary>`n`n}`n"; 'src/B.vb' = "' B の説明`nClass B`nEnd Class`n" } }
            )
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 0 }
        }
        @{
            Id = 'M04'; Category = '書き忘れ'; Title = 'コード以外（文書）の変更は数えない'
            Commits = @(@{ Message = '文書を直した'; Files = @{ 'docs/README.md' = "# 説明`n" } })
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 0 }
        }
        @{
            Id = 'M05'; Category = '書き忘れ'; Title = '期待値の変更に突き合わせ役の Reviewed-with が無い（警告。ほかの役では満たさない）'
            Commits = @(@{ Message = "B を消す`n`nExpectation-Change: 仕様の変更（#1）`nTested-with: unit`nReviewed-with: reviewer"; Files = @{ 'tests/FooTests.cs' = "public class FooTests`n{`n    [Test]`n    public void A() { Assert.That(1, Is.EqualTo(1)); }`n}`n" } })
            Expect = @{ Exit = 0; Kinds = @('テストの削除'); Warnings = 1 }
        }
        @{
            Id = 'M06'; Category = '書き忘れ'; Title = 'リポジトリの規則で組を足せる（リスク「高」の場所を fixed で変えたら Reviewed-with）'
            Commits = @(
                @{ Message = "規則を足す`n`nExpectation-Change: 書き忘れの組を足した（#6）`nReviewed-with: opentouryo-expectation-reviewer"; Files = @{ 'guard/rules.psd1' = "@{`n    TrailerRequirements = @(`n        @{ Name = 'Reviewed-with'; Message = '(?i)\bfixed\s+#\d+'; Paths = @('^src/Auth/'); Hint = 'リスク「高」' }`n    )`n}`n" } }
                @{ Message = "fixed #12 直した`n`nFound-By: spec-check`nTested-with: unit"; Files = @{ 'src/Auth/A.cs' = "class A { int x = 1; }`n" } }
            )
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 1 }
        }
        @{
            Id = 'M07'; Category = '書き忘れ'; Title = 'リスク「高」の場所でも fixing（途中）は数えない'
            Commits = @(
                @{ Message = "規則を足す`n`nExpectation-Change: 書き忘れの組を足した（#6）`nReviewed-with: opentouryo-expectation-reviewer"; Files = @{ 'guard/rules.psd1' = "@{`n    TrailerRequirements = @(`n        @{ Name = 'Reviewed-with'; Message = '(?i)\bfixed\s+#\d+'; Paths = @('^src/Auth/'); Hint = 'リスク「高」' }`n    )`n}`n" } }
                @{ Message = "fixing #12 途中`n`nTested-with: unit"; Files = @{ 'src/Auth/A.cs' = "class A { int x = 1; }`n" } }
            )
            Expect = @{ Exit = 0; Kinds = @(); Warnings = 0 }
        }

        # ==============================================================
        # 既知の取りこぼし（今の判定を固定する）
        # ==============================================================
        @{
            Id = 'K01'; Category = '既知の取りこぼし'; Title = 'アサーションの値の反転'
            Commits = @(@{ Message = '期待を反転'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.Not.EqualTo(1)); }

    [Test]
    public void B() { Assert.That(2, Is.EqualTo(2)); }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }
        @{
            Id = 'K02'; Category = '既知の取りこぼし'; Title = '同じファイルの中での入れ替え（消して別のテストを足す）'
            Commits = @(@{ Message = 'B を C に'; Files = @{ 'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }

    [Test]
    public void C() { Assert.That(true, Is.True); }
}
'@ } })
            Expect = @{ Exit = 0; Kinds = @() }
        }

        # ==============================================================
        # 過検出（既知）
        # ==============================================================
        @{
            Id = 'O01'; Category = '過検出（既知）'; Title = 'テストを別のファイルへ移す'
            Commits = @(@{ Message = 'B を移す'; Files = @{
                'tests/FooTests.cs' = @'
public class FooTests
{
    [Test]
    public void A() { Assert.That(1, Is.EqualTo(1)); }
}
'@
                'tests/BazTests.cs' = @'
public class BazTests
{
    [Test]
    public void B() { Assert.That(2, Is.EqualTo(2)); }
}
'@ } })
            Expect = @{ Exit = 1; Kinds = @('テストの削除') }
        }

        # ==============================================================
        # 段 1（PreToolUse。止めずに人の確認付きにする）
        # ==============================================================
        @{
            Id = 'P01'; Category = '検出する'; Title = '段 1：テストの属性が減る Edit'
            Mode = 'PreToolUse'
            Input = @{ tool_name = 'Edit'; tool_input = @{ file_path = '@ROOT@/tests/FooTests.cs'; old_string = "    [Test]`n    public void B()"; new_string = "    public void B()" } }
            Expect = @{ Ask = $true }
        }
        @{
            Id = 'P02'; Category = '検出しない'; Title = '段 1：テストを足す Edit'
            Mode = 'PreToolUse'
            Input = @{ tool_name = 'Edit'; tool_input = @{ file_path = '@ROOT@/tests/FooTests.cs'; old_string = "}"; new_string = "    [Test]`n    public void C() { }`n}" } }
            Expect = @{ Ask = $false }
        }
        @{
            Id = 'P03'; Category = '検出する'; Title = '段 1：Golden への Write'
            Mode = 'PreToolUse'
            Input = @{ tool_name = 'Write'; tool_input = @{ file_path = '@ROOT@/golden/Result1.txt'; content = "OK 2`n" } }
            Expect = @{ Ask = $true }
        }
        @{
            Id = 'P04'; Category = '検出する'; Title = '段 1：Golden を書き換えるコマンド'
            Mode = 'PreToolUse'
            Input = @{ tool_name = 'PowerShell'; tool_input = @{ command = 'Set-Content golden\Result1.txt "OK 2"' } }
            Expect = @{ Ask = $true }
        }
        @{
            Id = 'P05'; Category = '検出しない'; Title = '段 1：規則の外のファイルの Edit'
            Mode = 'PreToolUse'
            Input = @{ tool_name = 'Edit'; tool_input = @{ file_path = '@ROOT@/src/A.cs'; old_string = "[Test]"; new_string = "" } }
            Expect = @{ Ask = $false }
        }
        @{
            Id = 'P06'; Category = '検出する'; Title = '段 1：生成物の手での編集'
            Mode = 'PreToolUse'
            Input = @{ tool_name = 'Edit'; tool_input = @{ file_path = '@ROOT@/tests/TESTCASES.md'; old_string = "#"; new_string = "##" } }
            Expect = @{ Ask = $true }
        }
    )
}
