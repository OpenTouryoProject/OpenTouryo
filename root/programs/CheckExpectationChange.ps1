<#
.SYNOPSIS
    期待値の変更（QUALITY.md 3 節）を検出し、トレーラー Expectation-Change が付いているかを確かめる（#640）。

.DESCRIPTION
    commit-msg フック（.githooks/commit-msg）と、PR の CI が使う検出器。
    **何を期待値の変更と呼ぶかは、ここが実装である。** 定義は QUALITY.md 3 節。

    ＜Golden＞
      root/programs/*/Frameworks/Tests/Golden/**/Result*.txt のうち、**正規化しても差分が残る**もの。
      Result*.txt は実行のたびに書き換わる（日時・スレッド ID・署名値など）。素の差分で数えると、
      ほぼ毎回の fixed のコミットが引っかかり、承認が形だけになる。
      正規化は CompareResult.ps1 で、オプション（SkipLog4net・NormBase64）は 2_RunAllTests.ps1 の
      テストの定義を -List -PassThru で受け取る（二重に持たない）。
      定義に無いファイル（ResultLocal*.txt など）は、オプション無し（最も厳しい）で比べる。
      ファイルの追加・削除は、それだけで期待値の変更とする。

    ＜NUnit＞
      root/programs/*/Frameworks/Tests/NUnit/**/*.cs の差分で、ファイルごとに次を数える。
        テストの削除   … [Test]・[TestCase]・[TestCaseSource] の行が、足した数より多く消えた
        Ignore の追加  … [Ignore]・[Explicit]・Assert.Ignore( の行が、消した数より多く足された
      消えた行と足した行を差し引くので、書き直し（属性の引数を変えるなど）は数えない。
      // で始まる行は数えない（コメント アウトは「消えた」と数える）。
      **アサーションの緩和は見ない**（機械的に判定できない）。レビューの観点で拾う（QUALITY.md 4 節）。

    ＜仕組み自体＞
      この検出の仕組みのファイル（.githooks/・.claude/settings.json・.claude/hooks/・本スクリプト・
      CompareResult.ps1・.github/workflows/expectation-change.yml）の**変更・削除・名前の変更**も、期待値の変更と同じに扱う。
      仕組みを緩めれば、期待値をいくら変えても通るようになるため。**追加は数えない**（導入は緩めることではない）。

    ＜トレーラー＞
      コミット メッセージに「Expectation-Change: <理由>」の行があれば、人が承認したものとする。
      # で始まる行と、git commit -v の切り取り線から後は見ない。

.PARAMETER Staged
    ステージした差分（HEAD との差）を見る。commit-msg フックが使う。
    **マージの途中（MERGE_HEAD がある）なら見ない。** 取り込む側のコミットで確かめ済みのため。
.PARAMETER MessageFile
    -Staged のときの、コミット メッセージのファイル（commit-msg フックの第 1 引数）。
.PARAMETER Commit
    コミットと、その最初の親との差を見る。メッセージはそのコミットのもの。
    **マージ コミットは見ない**（-Staged と同じ理由）。
.PARAMETER Range
    <base>..<head> の範囲の、マージでないコミットを 1 つずつ -Commit で見る。PR の CI が使う。
    PR を出す前に、手元で develop..HEAD を見ておくのにも使える。
    **本スクリプトがまだ無いコミット（導入より前）は見ない。** 規約が無かった時点のコミットだからである。

.OUTPUTS
    終了コード 0 = 期待値の変更が無い、またはトレーラーがある
               1 = トレーラーの無い期待値の変更がある
               2 = 実行できない（引数の誤り・git の失敗）

.EXAMPLE
    .\CheckExpectationChange.ps1 -Commit HEAD

.EXAMPLE
    .\CheckExpectationChange.ps1 -Staged -MessageFile .git\COMMIT_EDITMSG

.EXAMPLE
    .\CheckExpectationChange.ps1 -Range develop..HEAD

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/10/29  玄人 幸道         新規作成（#640 段 2・段 3 の検出器）
     2026/10/29  玄人 幸道         -Range と、仕組み自体の変更の検出を追加（#640 段 3）
     2026/10/29  玄人 幸道         出力がパイプのとき UTF-8 で書くようにした（フックの表示が 5.1 で化けていた）
#>
[CmdletBinding()]
param(
    [switch]$Staged,
    [string]$MessageFile,
    [string]$Commit,
    [string]$Range
)

$ErrorActionPreference = "Stop"

# 対象のパス（git のパス。区切りは /）
$goldenPattern = '^root/programs/[^/]+/Frameworks/Tests/Golden/.+/Result[^/]*\.txt$'
$nunitPattern  = '^root/programs/[^/]+/Frameworks/Tests/NUnit/.+\.cs$'
$guardPattern  = '^(\.githooks/|\.claude/settings\.json$|\.claude/hooks/|root/programs/(CheckExpectationChange|CompareResult)\.ps1$|\.github/workflows/expectation-change\.yml$)'

# 数える行。属性の並び（[Test, Ignore("...")]）にも当たるよう、[ か , の直後を見る。
#   [TestFixture] は Test の直後が F なので当たらない。
$testAttrPattern   = '[\[,]\s*(Test|TestCase|TestCaseSource)\s*[\],(]'
$ignoreAttrPattern = '[\[,]\s*(Ignore|Explicit)\s*[\],(]|\bAssert\s*\.\s*Ignore\s*\('

# 空の木（親の無いコミットの比較元）
$emptyTree = "4b825dc642cb6eb9a060e54bf8d69288fbee4904"

# ------------------------------------------------------------------
# git の呼び出し
# ------------------------------------------------------------------
#   **コンソールのコード ページに触れずに、UTF-8 で受け取る。** フックはコンソールを人と共有しており、
#   [Console]::OutputEncoding を変えると、人の画面の表示まで変わる（2_RunAllTests.ps1 の chcp と同じ問題）。
#   カレントは変えない。フックでは git が GIT_INDEX_FILE 等を相対パスで渡すことがある。
#   OutFile を指定すると、標準出力をそのままファイルへ書く（blob をバイトのまま取り出すため）。
function Invoke-Git([string[]]$Arguments, [string]$OutFile)
{
    $quoted = foreach ($a in $Arguments)
    {
        if ($a -match '[\s"]') { '"' + ($a -replace '"', '\"') + '"' } else { $a }
    }

    $psi = New-Object Diagnostics.ProcessStartInfo
    $psi.FileName = "git"
    $psi.Arguments = "-c core.quotepath=false " + ($quoted -join " ")
    $psi.WorkingDirectory = (Get-Location).ProviderPath
    $psi.UseShellExecute = $false
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    $psi.StandardOutputEncoding = New-Object Text.UTF8Encoding $false

    $p = [Diagnostics.Process]::Start($psi)
    # 標準エラーは別に読む（両方を順に読むと、バッファが詰まって止まることがある）
    $errTask = $p.StandardError.ReadToEndAsync()
    $text = ""
    if ($OutFile)
    {
        $fs = [IO.File]::Create($OutFile)
        try { $p.StandardOutput.BaseStream.CopyTo($fs) } finally { $fs.Close() }
    }
    else
    {
        $text = $p.StandardOutput.ReadToEnd()
    }
    $p.WaitForExit()

    $lines = @()
    if ($text) { $lines = @($text -split "`n" | ForEach-Object { $_.TrimEnd("`r") }) }
    if ($lines.Count -gt 0 -and $lines[-1] -eq "") { $lines = $lines[0..($lines.Count - 2)] }

    return [pscustomobject]@{ ExitCode = $p.ExitCode; Lines = $lines; Error = $errTask.Result }
}

# 表示。**出力がパイプなら、UTF-8 のバイトを直接書く。**
#   フックを mintty・TortoiseGit・CI から動かすと、出力はパイプになる。5.1 はパイプへ、
#   コンソールのコード ページ（日本語の Windows では 932）で書き出すため、受け取る側（UTF-8）で化ける。
#   [Console]::OutputEncoding を変えると人のコンソールの表示まで変わるので、使わない（Invoke-Git と同じ）。
#   コンソールなら Write-Host のまま（色が付く）。
$script:rawOut = $null
function Write-Msg([string]$Text = "", [ConsoleColor]$ForegroundColor)
{
    if (-not [Console]::IsOutputRedirected)
    {
        if ($PSBoundParameters.ContainsKey("ForegroundColor")) { Write-Host $Text -ForegroundColor $ForegroundColor } else { Write-Host $Text }
        return
    }
    if (-not $script:rawOut) { $script:rawOut = [Console]::OpenStandardOutput() }
    $bytes = (New-Object Text.UTF8Encoding $false).GetBytes($Text + "`n")
    $script:rawOut.Write($bytes, 0, $bytes.Length)
    $script:rawOut.Flush()
}

# 失敗したら止める版
function Invoke-GitChecked([string[]]$Arguments, [string]$OutFile)
{
    $r = Invoke-Git $Arguments $OutFile
    if ($r.ExitCode -ne 0) { throw ("git {0} が失敗しました : {1}" -f ($Arguments -join " "), $r.Error.Trim()) }
    return $r
}

# ------------------------------------------------------------------
# メッセージのトレーラー
# ------------------------------------------------------------------
function Get-ExpectationTrailer([string[]]$Lines)
{
    $found = @()
    foreach ($l in $Lines)
    {
        if ($l -match '^# -+ >8 -+') { break }   # git commit -v の切り取り線
        if ($l.StartsWith("#")) { continue }
        if ($l -match '^Expectation-Change:\s*(\S.*)$') { $found += $Matches[1].Trim() }
    }
    return , $found
}

# ------------------------------------------------------------------
# Golden の比較オプション（2_RunAllTests.ps1 の定義）
# ------------------------------------------------------------------
#   キーは Tests からの相対パス（区切りは /、小文字）
function Get-GoldenOptions
{
    $map = @{}
    $defs = & (Join-Path $PSScriptRoot "2_RunAllTests.ps1") -List -PassThru 6>$null
    foreach ($d in @($defs))
    {
        if (-not $d.Result -or $d.Kind -eq "NUnit") { continue }
        $map[($d.Result -replace '\\', '/').ToLowerInvariant()] = $d
    }
    return $map
}

# ------------------------------------------------------------------
# 本体
# ------------------------------------------------------------------
$tempFiles = @()
try
{
    if (@(@($Staged.IsPresent, [bool]$Commit, [bool]$Range) | Where-Object { $_ }).Count -ne 1)
    {
        Write-Msg "  -Staged・-Commit・-Range のどれか 1 つを指定してください。" -ForegroundColor Red
        exit 2
    }

    # 範囲は、コミットごとに自分自身を -Commit で呼ぶ。最も悪い終了コードを返す。
    if ($Range)
    {
        $commits = @((Invoke-GitChecked @("rev-list", "--no-merges", "--reverse", $Range)).Lines)
        Write-Msg ("  {0} の {1} 件のコミットを見ます" -f $Range, $commits.Count)
        $worst = 0
        $skipped = 0
        foreach ($c in $commits)
        {
            # **導入より前のコミットは見ない。** 規約が無かった時点のコミットに、トレーラーは求められない。
            #   本スクリプトがそのコミットの木に在るかで判定する（-Commit で個別に見れば、導入前でも見る）。
            if ((Invoke-Git @("cat-file", "-e", "${c}:root/programs/CheckExpectationChange.ps1")).ExitCode -ne 0) { $skipped++; continue }
            & $PSCommandPath -Commit $c
            if ($LASTEXITCODE -gt $worst) { $worst = $LASTEXITCODE }
        }
        if ($skipped -gt 0) { Write-Msg ("  導入より前のコミット {0} 件は見ていません" -f $skipped) }
        if ($worst -eq 1)
        {
            Write-Msg "  既にあるコミットは、メッセージを書き直す（git rebase -i で reword）。push 済みなら force push になるので、人が判断する。"
        }
        exit $worst
    }

    # 比較の両端と、メッセージ
    if ($Staged)
    {
        if (-not $MessageFile -or -not (Test-Path $MessageFile))
        {
            Write-Msg ("  -MessageFile が見つかりません : {0}" -f $MessageFile) -ForegroundColor Red
            exit 2
        }
        if ((Invoke-Git @("rev-parse", "-q", "--verify", "MERGE_HEAD")).ExitCode -eq 0) { exit 0 }

        $base = "HEAD"
        if ((Invoke-Git @("rev-parse", "-q", "--verify", "HEAD")).ExitCode -ne 0) { $base = $emptyTree }
        $diffEnds = @("--cached", $base)
        $newSpec  = { param($path) ":$path" }
        $message  = @(Get-Content -LiteralPath $MessageFile -Encoding UTF8)
        $label    = "ステージした変更"
    }
    else
    {
        $sha = (Invoke-GitChecked @("rev-parse", "--verify", "$Commit^{commit}")).Lines[0]
        $parents = @((Invoke-GitChecked @("rev-list", "--parents", "-n", "1", $sha)).Lines[0] -split ' ' | Select-Object -Skip 1)
        if ($parents.Count -gt 1) { exit 0 }

        $base = $(if ($parents.Count -eq 1) { $parents[0] } else { $emptyTree })
        $diffEnds = @($base, $sha)
        $newSpec  = { param($path) "${sha}:$path" }
        $message  = @((Invoke-GitChecked @("log", "-1", "--format=%B", $sha)).Lines)
        $label    = "コミット " + $sha.Substring(0, 8)
    }

    $findings = @()

    # --- 仕組み自体と Golden --------------------------------------------
    $names = (Invoke-GitChecked (@("diff", "-M", "--name-status") + $diffEnds)).Lines
    $options = $null
    foreach ($n in $names)
    {
        $f = $n -split "`t"
        $status = $f[0].Substring(0, 1)
        $oldPath = $f[1]
        $newPath = $f[$f.Count - 1]

        if ($status -ne "A" -and $oldPath -match $guardPattern)
        {
            $findings += ("仕組みの変更     : {0}" -f $(if ($oldPath -eq $newPath) { $oldPath } else { "$oldPath → $newPath" }))
            continue
        }
        if (-not ($oldPath -match $goldenPattern -or $newPath -match $goldenPattern)) { continue }

        if ($status -eq "A") { $findings += "Golden の追加     : $newPath"; continue }
        if ($status -eq "D") { $findings += "Golden の削除     : $oldPath"; continue }

        if ($null -eq $options) { $options = Get-GoldenOptions }
        $rel = ($newPath -replace '^.*?/Frameworks/Tests/', '').ToLowerInvariant()
        $opt = $options[$rel]

        $old = [IO.Path]::GetTempFileName(); $new = [IO.Path]::GetTempFileName()
        $tempFiles += $old, $new
        [void](Invoke-GitChecked @("show", "${base}:$oldPath") $old)
        [void](Invoke-GitChecked @("show", (& $newSpec $newPath)) $new)

        $r = & (Join-Path $PSScriptRoot "CompareResult.ps1") -Expected $old -Actual $new -PassThru `
                -SkipLog4netTrace:([bool]($opt -and $opt.SkipLog4net)) -NormalizeBase64:([bool]($opt -and $opt.NormBase64)) 6>$null
        $r = @($r | Where-Object { $_ -and $_.PSObject.Properties["Result"] }) | Select-Object -Last 1
        if (-not $r) { throw ("CompareResult.ps1 の結果を受け取れませんでした : {0}" -f $newPath) }
        if ($r.Result -ne "OK")
        {
            $findings += ("Golden の変更     : {0}（正規化後の差分 {1} 件）" -f $newPath, $r.DiffCount)
        }
    }

    # --- NUnit ----------------------------------------------------------
    #   1 回の diff を「diff --git」の行でファイルに分け、消えた行・足した行を数える。
    $diff = (Invoke-GitChecked (@("diff", "-M", "-U0", "--no-color") + $diffEnds + @("--", "root/programs/"))).Lines
    $file = $null
    $count = @{}
    foreach ($l in $diff)
    {
        if ($l -match '^diff --git a/.+ b/(.+)$')
        {
            $file = $Matches[1]
            if ($file -notmatch $nunitPattern) { $file = $null; continue }
            $count[$file] = @{ TestDel = 0; TestAdd = 0; IgnDel = 0; IgnAdd = 0 }
            continue
        }
        if (-not $file) { continue }
        if ($l.StartsWith("+++") -or $l.StartsWith("---")) { continue }
        if (-not ($l.StartsWith("+") -or $l.StartsWith("-"))) { continue }

        $body = $l.Substring(1)
        if ($body.TrimStart().StartsWith("//")) { continue }
        $sign = $l.Substring(0, 1)
        if ($body -match $testAttrPattern)   { if ($sign -eq "-") { $count[$file].TestDel++ } else { $count[$file].TestAdd++ } }
        if ($body -match $ignoreAttrPattern) { if ($sign -eq "-") { $count[$file].IgnDel++ }  else { $count[$file].IgnAdd++ } }
    }
    foreach ($k in ($count.Keys | Sort-Object))
    {
        $c = $count[$k]
        if ($c.TestDel -gt $c.TestAdd) { $findings += ("テストの削除      : {0}（{1} 件）" -f $k, ($c.TestDel - $c.TestAdd)) }
        if ($c.IgnAdd -gt $c.IgnDel)   { $findings += ("Ignore の追加     : {0}（{1} 件）" -f $k, ($c.IgnAdd - $c.IgnDel)) }
    }

    # --- 判定 -----------------------------------------------------------
    if ($findings.Count -eq 0) { exit 0 }

    $trailers = Get-ExpectationTrailer $message
    if ($trailers.Count -gt 0)
    {
        Write-Msg ("  期待値の変更 {0} 件（{1}）。トレーラーで承認済み : {2}" -f $findings.Count, $label, ($trailers -join " / "))
        exit 0
    }

    Write-Msg ""
    Write-Msg ("  **トレーラーの無い期待値の変更があります**（{0}。QUALITY.md 3 節）" -f $label) -ForegroundColor Red
    foreach ($x in $findings) { Write-Msg ("    " + $x) -ForegroundColor Yellow }
    Write-Msg ""
    Write-Msg "  差分を確かめたうえで、コミット メッセージの末尾に理由を書いてください。"
    Write-Msg "    Expectation-Change: <何が・なぜ変わったか（仕様の該当箇所・Issue 番号）>"
    Write-Msg ""
    exit 1
}
catch
{
    Write-Msg ("  期待値の変更を確かめられませんでした : {0}" -f $_.Exception.Message) -ForegroundColor Red
    exit 2
}
finally
{
    foreach ($t in $tempFiles) { Remove-Item -LiteralPath $t -Force -ErrorAction SilentlyContinue }
}
