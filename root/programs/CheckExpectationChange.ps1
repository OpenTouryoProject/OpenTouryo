# expectation-guard-source: OpenTouryoCodingAgentAssets@b9da2ca0cc9d src/quality/ExpectationGuard/CheckExpectationChange.ps1 sha256:78634f35fbc0b11b1d0f1c260e30d5efdec1acdf6470b86103f820e0e156bc16
<#
.SYNOPSIS
    期待値の変更を検出し、トレーラー Expectation-Change が付いているかを確かめる（共通の本体）。

.DESCRIPTION
    段 1（Claude Code の PreToolUse フック）・段 2（commit-msg フック）・段 3（PR の CI）が使う検出器の、共通の本体。
    **本体は規則を持たない。** 何を期待値と呼ぶかは、同じフォルダの規則のファイルに書く。
      rules.common.psd1  … 共通の規則（原本の写し。仕組みのファイル・コメントの行・承認のトレーラーなど）
      rules.psd1         … リポジトリごとの規則（導入先が持つ。見本は rules.sample.psd1）
    配列は共通の規則に足し、値はリポジトリの規則で上書きする。規則の書き方は rules.sample.psd1 を見る。

    本体が行うこと（規則の種類ごと）
      Guard              仕組み自体のファイルの変更・削除・名前の変更。**追加は数えない**（導入は緩めることではない）。
                         本体と規則のファイル自身は、規則に書かなくても加わる。
      FileRules          ファイル単位の期待値（Golden など）。追加・削除は、それだけで期待値の変更。
                         変更は、Comparer（差し込みのスクリプト）があればそれで比べ、無ければ差分があれば期待値の変更。
      LineRules          ファイルごとに、パターンに当たる行の、消えた数と足した数を差し引く。
                         書き直し（引数を変えるなど）は数えない。CommentLine に当たる行は数えない
                         （コメント アウトは「消えた」と数える）。
      KeyedArgumentRules 呼び出しの引数を、**コミット全体で**キーごとに比べる（ファイルをまたいで移しただけなら数えない）。
      TrailerRequirements トレーラーの書き忘れ。条件の組（Message・Trailer・Paths。すべて当たれば）なのに Name のトレーラー
                         （Value があれば、それに当たる値）が無ければ警告する。IgnoreLine に当たる行だけの変更は数えない。
      TrailerRules       トレーラーの値の検査。語彙に無い値は止め、必要なのに無いときは警告する。
                         語彙は Vocabulary と、VocabularyFiles（ファイルの名前。例 .claude/agents/*.md）。List なら「,」で分けて見る。
                         止めたときは、書いた行・直した行・直した最後の段落・書き方（Syntax）・例（Example）・
                         語彙の一覧（Meanings があれば意味も）・コミットの直し方を出す（人がそのまま写して直せるように）。
      GeneratedRules     -CheckGenerated のとき、生成物の作り直しの漏れを見る（期待値の変更の代わりに）。
      ReviewRules        要確認。ファイルごとに、パターンに当たる行を足した数から消した数を引き、残れば一覧に出す。
                         **止めない・トレーラーを求めない**（終了コードは変えない）。GitHub Actions では注釈（notice）にする。
                         段 1（PreToolUse）では見ない。範囲は ReviewScope（省略時はリポジトリ全体）で、DiffScope は使わない。
    **アサーションの緩和は見ない**（機械的に判定できない）。レビューの観点で拾う。

    承認：コミット メッセージに「Expectation-Change: <理由>」の行があれば、人が承認したものとする。
    # で始まる行と、git commit -v の切り取り線から後は見ない。
    **トレーラーは git と同じ読み方で読む**（git interpret-trailers。メッセージの最後の段落だけ）。集計（git log の %(trailers)）に
    載らない書き方（最後の段落に無い・全角の「：」・名前の綴りの違い・値が空）は、規則のトレーラーと承認のトレーラーとも止める。

.PARAMETER Staged
    ステージした差分（HEAD との差）を見る。commit-msg フックが使う。
    **マージの途中（MERGE_HEAD がある）なら見ない。** 取り込む側のコミットで確かめ済みのため。
.PARAMETER MessageFile
    -Staged のときの、コミット メッセージのファイル（commit-msg フックの第 1 引数）。
.PARAMETER Commit
    コミットと、その最初の親との差を見る。メッセージはそのコミットのもの。**マージ コミットは見ない。**
.PARAMETER Range
    <base>..<head> の範囲の、マージでないコミットを 1 つずつ -Commit で見る。PR の CI が使う。
    **導入より前のコミット（規則の IntroducedBy が木に無いコミット）は見ない。**
.PARAMETER CheckGenerated
    -Range と併せて、GeneratedRules（生成物の作り直しの漏れ）を見る。
.PARAMETER PreToolUse
    Claude Code の PreToolUse フックとして動く（標準入力の JSON を読み、該当すれば ask を返す）。
    **止めない。失敗しても何もしない**（終了コード 2 は返さない。2 はツールを止めてしまう）。
.PARAMETER VerifyCopy
    導入先のファイルのうち、先頭に「expectation-guard-source:」の行があるもの（install.ps1 が写したもの）を、
    行に記した SHA-256 と比べる。ずれていても**警告に留める**（終了コードは 0）。
.PARAMETER RulesFile
    リポジトリごとの規則のファイル。既定は本体と同じフォルダの rules.psd1。
.PARAMETER PassThru
    結果をオブジェクトでも返す（判定の用例・置き換えの前後の比較に使う）。
.PARAMETER Quiet
    表示を出さない（-PassThru と併せて使う）。
.PARAMETER Json
    表示を出さず、結果を JSON で標準出力に出す（-Staged・-Commit で使う）。突き合わせ役（opentouryo-expectation-reviewer）の入力。
      Target・Commit・DiffCommand（差分を見るコマンド）・Findings（Kind・Path・Count）・Approved（承認の理由）・Review・Warnings・Errors・ExitCode

.OUTPUTS
    終了コード 0 = 期待値の変更が無い、またはトレーラーがある（警告はあってもよい）
               1 = トレーラーの無い期待値の変更がある、語彙に無いトレーラーの値・git が読まないトレーラーの行がある
                   （-CheckGenerated では、作り直しの漏れがある）
               2 = 実行できない（引数の誤り・規則の誤り・git の失敗）

.EXAMPLE
    .\CheckExpectationChange.ps1 -Commit HEAD
.EXAMPLE
    .\CheckExpectationChange.ps1 -Staged -MessageFile .git\COMMIT_EDITMSG
.EXAMPLE
    .\CheckExpectationChange.ps1 -Range origin/develop..HEAD
    PR の前に手元で、PR の CI と同じ範囲を見る（git fetch の後に）。手元の develop は古いことがあり、
    取り込み済みのコミットまで範囲に入る（OpenTouryo で、手元の develop..HEAD が 320 件・origin/develop..HEAD が 5 件だった）。
.EXAMPLE
    .\CheckExpectationChange.ps1 -Range origin/develop..HEAD -CheckGenerated

.NOTES
    原本：OpenTouryoCodingAgentAssets の src/quality/ExpectationGuard/（OpenTouryoCodingAgentAssets#3）。
    導入先では install.ps1 -IncludeQualityGuard で写す。**写しを直さず、原本を直して写し直す。**
    OpenTouryo #640 と MultiPurposeAuthSite #315 の検出器から、共通の処理を切り出した。
    Windows PowerShell 5.1 で動くこと。
#>
[CmdletBinding()]
param(
    [switch]$Staged,
    [string]$MessageFile,
    [string]$Commit,
    [string]$Range,
    [switch]$CheckGenerated,
    [switch]$PreToolUse,
    [switch]$VerifyCopy,
    [string]$RulesFile,
    [switch]$PassThru,
    [switch]$Quiet,
    [switch]$Json
)
if ($Json) { $Quiet = $true }

$ErrorActionPreference = "Stop"

# 空の木（親の無いコミットの比較元）
$emptyTree = "4b825dc642cb6eb9a060e54bf8d69288fbee4904"
# install.ps1 が写したファイルの印（VerifyCopy・install.ps1 と同じ）
$sourceMarker = 'expectation-guard-source:'

# ------------------------------------------------------------------
# git の呼び出し
# ------------------------------------------------------------------
#   **コンソールのコード ページに触れずに、UTF-8 で受け取る。** フックはコンソールを人と共有しており、
#   [Console]::OutputEncoding を変えると、人の画面の表示まで変わる。
#   カレントは変えない。フックでは git が GIT_INDEX_FILE 等を相対パスで渡すことがある。
#   OutFile を指定すると、標準出力をそのままファイルへ書く（blob をバイトのまま取り出すため）。
function Invoke-Git([string[]]$Arguments, [string]$OutFile, [string]$WorkingDirectory)
{
    $quoted = foreach ($a in $Arguments)
    {
        if ($a -match '[\s"]') { '"' + ($a -replace '"', '\"') + '"' } else { $a }
    }

    $psi = New-Object Diagnostics.ProcessStartInfo
    $psi.FileName = "git"
    $psi.Arguments = "-c core.quotepath=false " + ($quoted -join " ")
    $psi.WorkingDirectory = $(if ($WorkingDirectory) { $WorkingDirectory } else { (Get-Location).ProviderPath })
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

    return [pscustomobject]@{ ExitCode = $p.ExitCode; Lines = $lines; Text = $text; Error = $errTask.Result }
}

# 失敗したら止める版
function Invoke-GitChecked([string[]]$Arguments, [string]$OutFile)
{
    $r = Invoke-Git $Arguments $OutFile
    if ($r.ExitCode -ne 0) { throw ("git {0} が失敗しました : {1}" -f ($Arguments -join " "), $r.Error.Trim()) }
    return $r
}

# あるコミット（または索引）のファイルの中身。無ければ空文字。
function Get-Blob([string]$Spec)
{
    $r = Invoke-Git @("show", $Spec)
    if ($r.ExitCode -ne 0) { return "" }
    return $r.Text
}

# ------------------------------------------------------------------
# 表示
# ------------------------------------------------------------------
#   **出力がパイプなら、UTF-8 のバイトを直接書く。**
#   フックを mintty・TortoiseGit・CI から動かすと、出力はパイプになる。5.1 はパイプへ、
#   コンソールのコード ページ（日本語の Windows では 932）で書き出すため、受け取る側（UTF-8）で化ける。
#   [Console]::OutputEncoding を変えると人のコンソールの表示まで変わるので、使わない（Invoke-Git と同じ）。
$script:rawOut = $null
function Write-Msg([string]$Text = "", [ConsoleColor]$ForegroundColor)
{
    if ($Quiet) { return }
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

# 警告。GitHub Actions では注釈にする（PR の画面に出る）。
function Write-Warn([string]$Text)
{
    if ($env:GITHUB_ACTIONS -eq "true") { Write-Msg ("::warning::" + $Text.Trim()) } else { Write-Msg ("  警告 : " + $Text.Trim()) -ForegroundColor Yellow }
}

# ------------------------------------------------------------------
# 規則
# ------------------------------------------------------------------
#   psd1 は、データだけの言語（Restricted Language）として読む。
#   **Import-PowerShellDataFile は使わない。** PowerShell 7 から起動した git の sh で commit-msg フックが 5.1 を動かすと、
#   PSModulePath が 7 のモジュールを先に指し、5.1 からこのコマンドレットが見えなくなる（2026-11-06 に実測）。
function Read-DataFile([string]$Path)
{
    $sb = [scriptblock]::Create([IO.File]::ReadAllText($Path))
    $sb.CheckRestrictedLanguage([string[]]@(), [string[]]@(), $false)
    return (& $sb)
}

#   配列は共通に足し、値はリポジトリの規則で上書きする。
function Read-Rules([string]$CommonPath, [string]$RepoPath)
{
    $rules = @{}
    foreach ($p in @($CommonPath, $RepoPath))
    {
        if (-not $p -or -not (Test-Path -LiteralPath $p)) { continue }
        $h = Read-DataFile $p
        foreach ($k in $h.Keys)
        {
            $v = $h[$k]
            if ($v -is [array] -and $rules.ContainsKey($k)) { $rules[$k] = @($rules[$k]) + @($v) }
            else { $rules[$k] = $v }
        }
    }
    foreach ($k in @("Guard", "FileRules", "LineRules", "KeyedArgumentRules", "TrailerRules", "GeneratedRules", "ReviewRules", "DiffScope", "ReviewScope", "TrailerRequirements", "ApprovalReasonPatterns"))
    {
        if (-not $rules.ContainsKey($k)) { $rules[$k] = @() } else { $rules[$k] = @($rules[$k]) }
    }
    # トレーラーの規則は、同じ Name なら後（リポジトリの規則）が前（共通の規則）を置き換える（二重に数えない）
    $byName = [ordered]@{}
    foreach ($t in $rules["TrailerRules"]) { $byName[$t.Name] = $t }
    $rules["TrailerRules"] = @($byName.Values)
    if (-not $rules.ContainsKey("ApprovalTrailer")) { $rules["ApprovalTrailer"] = "Expectation-Change" }
    if (-not $rules.ContainsKey("CommentLine"))     { $rules["CommentLine"] = '^\s*//' }
    return $rules
}

# パスがパターンのどれかに当たるか（パターンが無ければ当たらない）
function Test-AnyMatch([string]$Path, $Patterns)
{
    foreach ($p in @($Patterns)) { if ($p -and $Path -match $p) { return $true } }
    return $false
}

# 本体のフォルダの、リポジトリのルートからのパス（例 "root/programs/"）と、ルートの場所。
#   CI では本体をベース側の取り出し（_base）から動かすので、カレントではなく本体の場所で求める。
function Get-SelfLocation
{
    $prefix = Invoke-Git @("rev-parse", "--show-prefix") $null $PSScriptRoot
    $top    = Invoke-Git @("rev-parse", "--show-toplevel") $null $PSScriptRoot
    if ($prefix.ExitCode -ne 0 -or $top.ExitCode -ne 0) { return $null }
    $pre = $(if ($prefix.Lines.Count -gt 0) { $prefix.Lines[0] } else { "" })
    return [pscustomobject]@{ Prefix = $pre; Top = $top.Lines[0] }
}

# ------------------------------------------------------------------
# メッセージのトレーラー
# ------------------------------------------------------------------
function Get-MessageLines([string[]]$Lines)
{
    $out = @()
    foreach ($l in $Lines)
    {
        if ($l -match '^# -+ >8 -+') { break }   # git commit -v の切り取り線
        if ($l.StartsWith("#")) { continue }
        $out += $l
    }
    return , $out
}

#   行のどこにあっても拾う。生成物の作り直しの漏れ（範囲のメッセージをまとめて見る）でだけ使う。
#   コミットごとの検査は、git と同じ読み方の Get-GitTrailers を使う。
function Get-Trailer([string[]]$Lines, [string]$Name)
{
    $found = @()
    foreach ($l in (Get-MessageLines $Lines))
    {
        if ($l -match ('^' + [regex]::Escape($Name) + ':\s*(\S.*)$')) { $found += $Matches[1].Trim() }
    }
    return , $found
}

# git がトレーラーとして読む行（メッセージの最後の段落）。**集計（git log の %(trailers)）と同じ読み方にするため、git に読ませる。**
#   キーは大文字・小文字を区別しない（git と同じ）。値が空の行も返す。
function Get-GitTrailers([string[]]$Lines)
{
    $tmp = [IO.Path]::GetTempFileName()
    $script:tempFiles += $tmp
    [IO.File]::WriteAllText($tmp, ((Get-MessageLines $Lines) -join "`n") + "`n", (New-Object Text.UTF8Encoding $false))
    $r = Invoke-GitChecked @("interpret-trailers", "--parse", "--no-divider", $tmp)
    $out = @()
    foreach ($l in $r.Lines)
    {
        if ($l -match '^([^:]+):\s?(.*)$') { $out += [pscustomobject]@{ Key = $Matches[1].Trim(); Value = $Matches[2].Trim() } }
    }
    return , $out
}

function Select-Trailer($Parsed, [string]$Name)
{
    @($Parsed) | Where-Object { $_ -and $_.Key -ieq $Name } | ForEach-Object { $_.Value }
}

function Get-EditDistance([string]$A, [string]$B)
{
    # 1 行前と今の行だけを持つ（Windows PowerShell 5.1 は、2 次元の配列の添え字に式を書けない）
    $prev = New-Object 'int[]' ($B.Length + 1)
    $curr = New-Object 'int[]' ($B.Length + 1)
    for ($j = 0; $j -le $B.Length; $j++) { $prev[$j] = $j }
    for ($i = 1; $i -le $A.Length; $i++)
    {
        $curr[0] = $i
        for ($j = 1; $j -le $B.Length; $j++)
        {
            $cost = $(if ($A[$i - 1] -ceq $B[$j - 1]) { 0 } else { 1 })
            $curr[$j] = [Math]::Min([Math]::Min($prev[$j] + 1, $curr[$j - 1] + 1), $prev[$j - 1] + $cost)
        }
        $tmp = $prev; $prev = $curr; $curr = $tmp
    }
    return $prev[$B.Length]
}

# 綴りの違いを許す幅（長い語ほど広く）
function Test-NearWord([string]$A, [string]$B)
{
    $limit = $(if ($B.Length -ge 10) { 2 } else { 1 })
    return ((Get-EditDistance $A $B) -le $limit)
}

# 語彙のうち、書いた語に近いもの（大文字・小文字の違い、綴りの 1〜2 文字の違い）。無ければ $null
function Get-NearestWord([string]$Word, [string[]]$Vocabulary)
{
    foreach ($v in $Vocabulary) { if ($v -ieq $Word) { return $v } }
    foreach ($v in $Vocabulary) { if (Test-NearWord $Word.ToLowerInvariant() $v.ToLowerInvariant()) { return $v } }
    return $null
}

# トレーラーの名前の比べ方（大文字・小文字・「-」・空白・「_」を無視）
function Get-NameKey([string]$Name) { return ($Name -replace '[\s\-_]', '').ToLowerInvariant() }

# トレーラーらしい行（名前・区切り・値）。区切りの全角の「：」と、名前の中の空白も拾う（直す対象として）
$trailerLike = '^(?<lead>\s*)(?<name>[A-Za-z][A-Za-z0-9]*(?:[\s\-_]+[A-Za-z0-9]+)*)\s*(?<sep>[:：])\s*(?<value>.*)$'

# **git に読まれない、トレーラーのつもりの行。** 書いた行・直した名前・理由を返す（値の語彙は呼ぶ側で見る）。
#   見るのは、名前が規則のトレーラー（綴りの違いを含む）の行のうち、
#   トレーラーらしい行だけの段落にある行と、最後の段落の末尾に続く行。本文の文章の中の行と、1 行目（件名）は見ない。
function Find-TrailerProblems([string[]]$Lines, [string[]]$Names, $Parsed)
{
    $msg = [string[]](Get-MessageLines $Lines)
    $paras = @(); $cur = @()
    for ($i = 1; $i -lt $msg.Count; $i++)
    {
        if ($msg[$i].Trim() -eq "") { if ($cur.Count -gt 0) { $paras += , $cur; $cur = @() }; continue }
        $cur += $msg[$i]
    }
    if ($cur.Count -gt 0) { $paras += , $cur }

    $keys = @{}
    foreach ($n in $Names) { if ($n) { $keys[(Get-NameKey $n)] = $n } }
    $out = @()
    for ($p = 0; $p -lt $paras.Count; $p++)
    {
        $isLast  = ($p -eq $paras.Count - 1)
        $allLike = (@($paras[$p] | Where-Object { $_ -notmatch $trailerLike }).Count -eq 0)
        if (-not $isLast -and -not $allLike) { continue }
        # 本文が混じった最後の段落では、末尾に続くトレーラーらしい行だけを見る（本文の直後に空行なしで書いたもの）
        $from = 0
        if (-not $allLike)
        {
            for ($q = $paras[$p].Count - 1; $q -ge 0; $q--) { if ($paras[$p][$q] -notmatch $trailerLike) { $from = $q + 1; break } }
        }
        foreach ($l in @($paras[$p] | Select-Object -Skip $from))
        {
            if ($l -notmatch $trailerLike) { continue }
            $written = $Matches['name']; $sep = $Matches['sep']; $lead = $Matches['lead']; $value = $Matches['value'].Trim()
            $k = Get-NameKey $written
            $canon = $null
            if ($keys.ContainsKey($k)) { $canon = $keys[$k] }
            else { foreach ($kk in $keys.Keys) { if (Test-NearWord $k $kk) { $canon = $keys[$kk]; break } } }
            if (-not $canon) { continue }

            $nameOk = ($written -ieq $canon)
            $read = (@(@($Parsed) | Where-Object { $_ -and $_.Key -ieq $written -and $_.Value -ceq $value }).Count -gt 0)
            if ($nameOk -and $read) { continue }

            $reasons = @()
            if (-not $nameOk) { $reasons += ("名前は {0}" -f $canon) }
            if ($sep -ne ':')  { $reasons += "区切りは半角の「:」" }
            if ($lead)         { $reasons += "行の頭に空白を入れない" }
            if (-not $isLast)  { $reasons += "最後の段落に無い" }
            elseif (-not $allLike) { $reasons += "最後の段落に本文の行が混じっている" }
            elseif ($nameOk -and $sep -eq ':' -and -not $lead) { $reasons += "同じ段落のほかの行の誤りを直せば、git が読む" }
            if ($reasons.Count -eq 0) { $reasons += "git がトレーラーとして読まない" }
            $out += [pscustomobject]@{ Name = $canon; Written = $l; Value = $value; Reasons = $reasons; Read = $read }
        }
    }
    return , $out
}

# 規則の語彙と意味。VocabularyFiles（ファイルの名前を語にする。例 .claude/agents/*.md → 役の定義の名前）は、
#   確かめるコミット（ステージなら索引）の木から取る。
function Get-RuleVocabulary($Rule, [string]$Sha)
{
    $words = @()
    $meanings = @{}
    foreach ($glob in @($Rule.VocabularyFiles))
    {
        if (-not $glob) { continue }
        $dir = ($glob -replace '[^/]*$', '')
        $files = $(if ($Sha) { (Invoke-GitChecked @("ls-tree", "--full-tree", "--name-only", $Sha, "--", $dir)).Lines }
                   else      { (Invoke-GitChecked @("ls-files", "--full-name", "--", ":(top)$dir")).Lines })
        foreach ($f in @($files | Where-Object { $_ -like $glob } | Sort-Object))
        {
            $w = [IO.Path]::GetFileNameWithoutExtension($f)
            if ($words -ccontains $w) { continue }
            $words += $w
            $meanings[$w] = $(if ($Rule.VocabularyFilesMeaning) { $Rule.VocabularyFilesMeaning -f $f } else { $f })
        }
    }
    foreach ($w in @($Rule.Vocabulary)) { if ($w -and ($words -cnotcontains $w)) { $words += $w } }
    if ($Rule.Meanings) { foreach ($k in $Rule.Meanings.Keys) { $meanings[$k] = $Rule.Meanings[$k] } }
    return [pscustomobject]@{ Words = $words; Meanings = $meanings; Checked = ($null -ne $Rule.Vocabulary -or $null -ne $Rule.VocabularyFiles) }
}

# 値を語彙で確かめる。List なら「,」で分けた項目ごと（括弧の中の「,」では分けない）。項目の最初の語を見る。
#   Problems：誤りの説明。Fixed：直した値（近い語があれば置き換え、無ければ Placeholder のまま。補足は残す）
function Test-TrailerValue($Rule, [string]$Value, $Vocab)
{
    $ph = $(if ($Rule.Placeholder) { $Rule.Placeholder } else { "<語>" })
    if ($Value.Trim() -eq "") { return [pscustomobject]@{ Problems = @("値がありません"); Fixed = $ph } }
    $items = $(if ($Rule.List) { @($Value -split ',(?![^(]*\))') } else { @($Value) })
    $problems = @()
    $fixed = @()
    foreach ($it in $items)
    {
        $t = $it.Trim()
        if ($t -eq "") { continue }
        $word = ($t -split '\s+')[0]
        $rest = $t.Substring($word.Length)
        if (-not $Vocab.Checked -or ($word -cin @($Vocab.Words))) { $fixed += $t; continue }
        $near = Get-NearestWord $word @($Vocab.Words)
        if ($near) { $problems += ("{0} は語彙にありません（{1} のことなら、直した行のとおり）" -f $word, $near); $fixed += ($near + $rest) }
        else       { $problems += ("{0} は語彙にありません（下の一覧から選ぶ）" -f $word); $fixed += ($ph + $rest) }
    }
    return [pscustomobject]@{ Problems = $problems; Fixed = ($fixed -join ", ") }
}

# ------------------------------------------------------------------
# C# の呼び出しの引数を読む
# ------------------------------------------------------------------
#   行の差分では、複数行にまたがる引数を比べられないので、引数の単位で読む。
#   文字列の外の空白を除いて返す（折り返しを変えただけなら、同じになる）。
function Get-CallArguments([string]$Text, [int]$Start)
{
    $args2 = New-Object Collections.ArrayList
    $sb = New-Object Text.StringBuilder
    $depth = 0
    $i = $Start
    while ($i -lt $Text.Length)
    {
        $c = $Text[$i]
        $next = $(if ($i + 1 -lt $Text.Length) { $Text[$i + 1] } else { [char]0 })

        # コメント
        if ($c -eq '/' -and $next -eq '/')
        {
            $e = $Text.IndexOf("`n", $i)
            if ($e -lt 0) { break }
            $i = $e + 1
            continue
        }
        if ($c -eq '/' -and $next -eq '*')
        {
            $e = $Text.IndexOf("*/", $i + 2)
            if ($e -lt 0) { break }
            $i = $e + 2
            continue
        }

        # 文字列（@"..." は "" で引用符を表す。それ以外は \ でエスケープする）
        if ($c -eq '"')
        {
            $verbatim = ($i -gt 0 -and ($Text[$i - 1] -eq '@' -or ($i -gt 1 -and $Text[$i - 2] -eq '@' -and $Text[$i - 1] -eq '$')))
            [void]$sb.Append($c)
            $i++
            while ($i -lt $Text.Length)
            {
                $d = $Text[$i]
                [void]$sb.Append($d)
                $i++
                if ($verbatim)
                {
                    if ($d -eq '"')
                    {
                        if ($i -lt $Text.Length -and $Text[$i] -eq '"') { [void]$sb.Append('"'); $i++; continue }
                        break
                    }
                }
                else
                {
                    if ($d -eq '\' -and $i -lt $Text.Length) { [void]$sb.Append($Text[$i]); $i++; continue }
                    if ($d -eq '"') { break }
                }
            }
            continue
        }

        # 文字リテラル（'"' や '(' で数え違えないように）
        if ($c -eq "'")
        {
            $e = $i + 1
            if ($e -lt $Text.Length -and $Text[$e] -eq '\') { $e++ }
            $e = $Text.IndexOf("'", $e + 1)
            if ($e -lt 0) { break }
            [void]$sb.Append($Text.Substring($i, $e - $i + 1))
            $i = $e + 1
            continue
        }

        if ($c -eq '(' -or $c -eq '[' -or $c -eq '{') { $depth++ }
        elseif ($c -eq ')' -or $c -eq ']' -or $c -eq '}')
        {
            if ($depth -eq 0)
            {
                [void]$args2.Add($sb.ToString())
                return , $args2.ToArray()
            }
            $depth--
        }
        elseif ($c -eq ',' -and $depth -eq 0)
        {
            [void]$args2.Add($sb.ToString())
            [void]$sb.Clear()
            $i++
            continue
        }

        if (-not [char]::IsWhiteSpace($c)) { [void]$sb.Append($c) }
        $i++
    }
    return , $args2.ToArray()
}

# 呼び出しの引数を集める（キー → 値）
function Add-KeyedArguments([string]$Text, $Rule, [hashtable]$Map)
{
    if (-not $Text) { return }
    $keyIndex = $(if ($null -ne $Rule.KeyIndex) { [int]$Rule.KeyIndex } else { 0 })
    $valueIndex = [int]$Rule.ValueIndex
    $minArgs = $(if ($null -ne $Rule.MinArgs) { [int]$Rule.MinArgs } else { $valueIndex + 1 })
    foreach ($m in [regex]::Matches($Text, ('(?<![\w])' + [regex]::Escape($Rule.Call) + '\s*\(')))
    {
        $a = Get-CallArguments $Text ($m.Index + $m.Length)
        if ($a.Count -ge $minArgs) { $Map[$a[$keyIndex]] = $a[$valueIndex] }
    }
}

# 生成物に現れる内容を、出てくる順に並べる（GeneratedRules の Items）
#   Items の各パターンを 1 つの選択（|）にまとめて、左から読む（重なる当たりを二重に数えない）。
#     Take = All   … 呼び出しの引数をすべて（Label:引数|引数|…）
#     Take = First … 呼び出しの最初の引数（label グループ、無ければ Label:引数）
#     Take = Name  … name グループ（Label:名前）。引数は読まない
function Get-GeneratedItems([string]$Text, $Items)
{
    if (-not $Text) { return , @() }
    $parts = @()
    for ($k = 0; $k -lt @($Items).Count; $k++) { $parts += ('(?<i{0}>{1})' -f $k, @($Items)[$k].Pattern) }
    $result = New-Object Collections.ArrayList
    foreach ($m in [regex]::Matches($Text, ($parts -join '|')))
    {
        for ($k = 0; $k -lt @($Items).Count; $k++) { if ($m.Groups["i$k"].Success) { break } }
        $item = @($Items)[$k]
        $label = $(if ($m.Groups["label"].Success) { $m.Groups["label"].Value } else { $item.Label })
        if ($item.Take -eq "Name") { [void]$result.Add($label + ":" + $m.Groups["name"].Value); continue }
        $a = Get-CallArguments $Text ($m.Index + $m.Length)
        if ($a.Count -eq 0) { continue }
        if ($item.Take -eq "First") { [void]$result.Add($label + ":" + $a[0]) }
        else { [void]$result.Add($label + ":" + ($a -join "|")) }
    }
    return , $result.ToArray()
}

# ------------------------------------------------------------------
# 写しの検査（install.ps1 と同じ計算）
# ------------------------------------------------------------------
#   UTF-8 で読み（BOM を除く）、改行を LF にそろえ、印の行（先頭 3 行の中の最初のもの）を除いた残りの SHA-256。
function Get-CopyHash([string]$Text)
{
    $lines = New-Object Collections.ArrayList
    $lines.AddRange(@(($Text -replace "`r`n", "`n") -split "`n"))
    for ($i = 0; $i -lt [Math]::Min(3, $lines.Count); $i++)
    {
        if ($lines[$i] -match ('^\s*#.*' + [regex]::Escape($sourceMarker))) { $lines.RemoveAt($i); break }
    }
    $sha = [Security.Cryptography.SHA256]::Create()
    $bytes = (New-Object Text.UTF8Encoding $false).GetBytes(($lines -join "`n"))
    return ([BitConverter]::ToString($sha.ComputeHash($bytes)) -replace '-', '').ToLowerInvariant()
}

# ------------------------------------------------------------------
# 段 1：PreToolUse フック
# ------------------------------------------------------------------
function Invoke-PreToolUse($Rules)
{
    # ask を返す。**コンソールのコード ページに触れずに、UTF-8 で書く。**
    function Write-Ask([string]$Reason)
    {
        $json = @{
            hookSpecificOutput = @{
                hookEventName            = "PreToolUse"
                permissionDecision       = "ask"
                permissionDecisionReason = $Reason + ("（期待値の変更{0}。コミットにはトレーラー {1}: <理由> が要る）" -f $(if ($Rules.Reference) { "。" + $Rules.Reference } else { "" }), $Rules.ApprovalTrailer)
            }
        } | ConvertTo-Json -Compress -Depth 4
        $bytes = (New-Object Text.UTF8Encoding $false).GetBytes($json)
        $out = [Console]::OpenStandardOutput()
        $out.Write($bytes, 0, $bytes.Length)
        $out.Flush()
    }

    # 行を数える（CommentLine に当たる行は数えない）
    function Get-LineCount([string]$Text, $Rule, [string]$Path)
    {
        if (-not $Text) { return 0 }
        $n = 0
        foreach ($l in ($Text -split "`n"))
        {
            if ($l -match $Rules.CommentLine) { continue }
            foreach ($m in @($Rule.Match))
            {
                if ($m.Paths -and -not (Test-AnyMatch $Path $m.Paths)) { continue }
                if ($l -match $m.Pattern) { $n++; break }
            }
        }
        return $n
    }

    $reader = New-Object IO.StreamReader([Console]::OpenStandardInput(), (New-Object Text.UTF8Encoding $false))
    $hook = $reader.ReadToEnd() | ConvertFrom-Json
    $tool = "$($hook.tool_name)"
    $in   = $hook.tool_input

    $fileRules = @($Rules.FileRules) + @($Rules.GeneratedRules)

    if ($tool -in @("Bash", "PowerShell"))
    {
        # **推測である。** 漏れは段 2・段 3 が塞ぐ。パスを書かない書き換え（テストの実行など）は対象にしない。
        $cmd = "$($in.command)"
        if (-not $Rules.WriteInCommand -or $cmd -notmatch $Rules.WriteInCommand) { return }
        foreach ($r in $fileRules)
        {
            if ($r.CommandPattern -and $cmd -match $r.CommandPattern)
            {
                Write-Ask ("{0} を書き換える可能性のあるコマンドです" -f $r.Name)
                return
            }
        }
        return
    }

    if ($tool -notin @("Edit", "Write", "MultiEdit")) { return }

    # リポジトリのルートからのパスにする（ルートの外なら見ない）
    $root = $env:CLAUDE_PROJECT_DIR
    if (-not $root) { $root = (Invoke-Git @("rev-parse", "--show-toplevel")).Lines | Select-Object -First 1 }
    if (-not $root) { return }
    $root = ($root -replace '\\', '/').TrimEnd('/') + '/'
    $full = ("$($in.file_path)" -replace '\\', '/')
    if (-not $full.StartsWith($root, [StringComparison]::OrdinalIgnoreCase)) { return }
    $path = $full.Substring($root.Length)

    foreach ($r in @($Rules.FileRules))
    {
        if (Test-AnyMatch $path $r.Paths) { Write-Ask ("{0} を編集します : {1}" -f $r.Name, [IO.Path]::GetFileName($path)); return }
    }
    foreach ($r in @($Rules.GeneratedRules))
    {
        if ($path -eq $r.Generated) { Write-Ask ("{0}（生成物）を手で編集します" -f $r.Name); return }
    }

    $lineRules = @($Rules.LineRules | Where-Object { Test-AnyMatch $path $_.Paths })
    if ($lineRules.Count -eq 0) { return }

    # 編集の前後の文字列。Write は既存のファイルと、新しい内容を比べる。
    $before = @(); $after = @()
    switch ($tool)
    {
        "Edit"      { $before += "$($in.old_string)"; $after += "$($in.new_string)" }
        "MultiEdit" { foreach ($e in @($in.edits)) { $before += "$($e.old_string)"; $after += "$($e.new_string)" } }
        "Write"
        {
            if (Test-Path -LiteralPath $in.file_path) { $before += [IO.File]::ReadAllText($in.file_path) }
            $after += "$($in.content)"
        }
    }
    $b = $before -join "`n"; $a = $after -join "`n"

    $reasons = @()
    foreach ($r in $lineRules)
    {
        $n = $(if ($r.Count -eq "Added") { (Get-LineCount $a $r $path) - (Get-LineCount $b $r $path) } else { (Get-LineCount $b $r $path) - (Get-LineCount $a $r $path) })
        if ($n -gt 0) { $reasons += ("{0} {1} 件" -f $r.Name, $n) }
    }
    if ($reasons.Count -gt 0)
    {
        Write-Ask ("テストを変えます（{0}） : {1}" -f ($reasons -join "、"), [IO.Path]::GetFileName($path))
    }
}

# ------------------------------------------------------------------
# 本体
# ------------------------------------------------------------------
$tempFiles = @()
$result = [pscustomobject]@{ Target = $null; Commit = $null; DiffCommand = $null; Findings = @(); Approved = @(); Review = @(); Warnings = @(); Errors = @(); ExitCode = 0 }

function Write-Result
{
    if ($PassThru) { Write-Output $result }
    if ($Json) { Write-Output ($result | ConvertTo-Json -Depth 6) }
}

function Complete([int]$Code)
{
    $result.ExitCode = $Code
    Write-Result
    exit $Code
}

try
{
    $commonRules = Join-Path $PSScriptRoot "rules.common.psd1"
    if (-not $RulesFile) { $RulesFile = Join-Path $PSScriptRoot "rules.psd1" }

    # --- 段 1 ------------------------------------------------------------
    if ($PreToolUse)
    {
        try { Invoke-PreToolUse (Read-Rules $commonRules $RulesFile) }
        catch
        {
            # 失敗しても止めない。理由だけ標準エラーへ出す（Claude Code はブロックしない）
            [Console]::Error.WriteLine("CheckExpectationChange.ps1 -PreToolUse: " + $_.Exception.Message)
            exit 1
        }
        exit 0
    }

    # --- 写しの検査 ------------------------------------------------------
    if ($VerifyCopy)
    {
        # --full-name：どのフォルダで実行しても、ルートからのパスで受ける（無いとカレントからの相対になり、0 件を見て「違いは 0 件」になる）
        $files = (Invoke-GitChecked @("ls-files", "--full-name", "--", ":(top)")).Lines
        $top = (Invoke-GitChecked @("rev-parse", "--show-toplevel")).Lines[0]
        $checked = 0; $bad = 0
        foreach ($f in $files)
        {
            $full = Join-Path $top $f
            if (-not (Test-Path -LiteralPath $full -PathType Leaf)) { continue }
            $head = @(Get-Content -LiteralPath $full -TotalCount 3 -Encoding UTF8 -ErrorAction SilentlyContinue)
            $line = $head | Where-Object { $_ -match ('^\s*#.*' + [regex]::Escape($sourceMarker) + '.*\bsha256:([0-9a-f]{64})\b') } | Select-Object -First 1
            if (-not $line) { continue }
            [void]($line -match 'sha256:([0-9a-f]{64})')
            $expected = $Matches[1]
            $checked++
            if ((Get-CopyHash ([IO.File]::ReadAllText($full))) -ne $expected)
            {
                $bad++
                Write-Warn ("{0} は、写したときの内容と違います（原本を直して install.ps1 -IncludeQualityGuard で写し直す）" -f $f)
            }
        }
        Write-Msg ("  写しの検査 : {0} 件を見て、{1} 件が写したときと違います" -f $checked, $bad)
        exit 0
    }

    $rules = Read-Rules $commonRules $RulesFile
    $self = Get-SelfLocation

    # 仕組みのファイル（本体と規則のファイル自身を加える）
    $guard = @($rules.Guard)
    if ($self)
    {
        foreach ($p in @($PSCommandPath, $commonRules, $RulesFile))
        {
            $full = [IO.Path]::GetFullPath($p)
            $top = [IO.Path]::GetFullPath($self.Top)
            if ($full.StartsWith($top, [StringComparison]::OrdinalIgnoreCase))
            {
                $guard += ('^' + [regex]::Escape(($full.Substring($top.Length).TrimStart('\', '/') -replace '\\', '/')) + '$')
            }
        }
    }
    $introducedBy = $rules.IntroducedBy
    if (-not $introducedBy -and $self) { $introducedBy = $self.Prefix + [IO.Path]::GetFileName($PSCommandPath) }

    if (@(@($Staged.IsPresent, [bool]$Commit, [bool]$Range) | Where-Object { $_ }).Count -ne 1)
    {
        Write-Msg "  -Staged・-Commit・-Range のどれか 1 つを指定してください。" -ForegroundColor Red
        Complete 2
    }
    if ($CheckGenerated -and -not $Range)
    {
        Write-Msg "  -CheckGenerated は -Range と併せて指定してください。" -ForegroundColor Red
        Complete 2
    }

    # --- 生成物の作り直しの漏れ -----------------------------------------
    if ($CheckGenerated)
    {
        if ($Range -notmatch '^(.+)\.\.(.+)$')
        {
            Write-Msg "  -Range は <base>..<head> の形で指定してください。" -ForegroundColor Red
            Complete 2
        }
        $head = $Matches[2]
        $base = (Invoke-GitChecked @("merge-base", $Matches[1], $head)).Lines[0]
        $result.Target = $Range
        if (@($rules.GeneratedRules).Count -eq 0) { Write-Msg "  生成物の規則はありません。"; Complete 0 }

        $names = (Invoke-GitChecked @("diff", "--name-only", $base, $head, "--", ":(top)")).Lines
        $messages = (Invoke-GitChecked @("log", "--format=%B", "$base..$head")).Lines
        $worst = 0
        foreach ($g in @($rules.GeneratedRules))
        {
            if ($names -contains $g.Generated) { Write-Msg ("  {0} は作り直されています。" -f $g.Name); continue }

            $changed = @()
            foreach ($n in $names)
            {
                if (-not (Test-AnyMatch $n $g.Sources)) { continue }
                $old = Get-GeneratedItems (Get-Blob "${base}:$n") $g.Items
                $new = Get-GeneratedItems (Get-Blob "${head}:$n") $g.Items
                if (($old -join "`n") -cne ($new -join "`n")) { $changed += $n }
            }
            if ($changed.Count -eq 0) { Write-Msg ("  {0} に現れる内容の変更はありません。" -f $g.Name); continue }

            $reasons = @()
            if ($g.ExemptTrailer) { $reasons = Get-Trailer $messages $g.ExemptTrailer }
            if ($reasons.Count -gt 0)
            {
                $result.Approved += $reasons
                Write-Msg ("  {0} に現れる内容の変更 {1} 件。トレーラーで承認済み : {2}" -f $g.Name, $changed.Count, ($reasons -join " / "))
                continue
            }

            foreach ($x in $changed) { $result.Findings += [pscustomobject]@{ Kind = ($g.Name + " の作り直しの漏れ"); Path = $x; Count = 1 } }
            Write-Msg ""
            Write-Msg ("  **テストの内容を変えたのに、{0} が作り直されていません**" -f $g.Name) -ForegroundColor Red
            foreach ($x in $changed) { Write-Msg ("    " + $x) -ForegroundColor Yellow }
            Write-Msg ""
            foreach ($h in @($g.Hint)) { Write-Msg ("  " + $h) }
            if ($g.ExemptTrailer)
            {
                Write-Msg "  生成物に現れない変更なら、トレーラーに理由を書いてください。"
                Write-Msg ("    {0}: <理由>" -f $g.ExemptTrailer)
            }
            Write-Msg ""
            $worst = 1
        }
        Complete $worst
    }

    # --- 範囲は、コミットごとに自分自身を -Commit で呼ぶ。最も悪い終了コードを返す ---
    if ($Range)
    {
        if ($Json) { throw "-Json は -Staged・-Commit で使う（範囲は、コミットごとに -Commit <sha> -Json で）" }
        $commits =@((Invoke-GitChecked @("rev-list", "--no-merges", "--reverse", $Range)).Lines)
        Write-Msg ("  {0} の {1} 件のコミットを見ます" -f $Range, $commits.Count)
        $worst = 0
        $skipped = 0
        foreach ($c in $commits)
        {
            # **導入より前のコミットは見ない。** 規約が無かった時点のコミットに、トレーラーは求められない。
            #   IntroducedBy がそのコミットの木に在るかで判定する（-Commit で個別に見れば、導入前でも見る）。
            if ($introducedBy -and (Invoke-Git @("cat-file", "-e", "${c}:$introducedBy")).ExitCode -ne 0) { $skipped++; continue }
            & $PSCommandPath -Commit $c -RulesFile $RulesFile -PassThru:$PassThru -Quiet:$Quiet
            if ($LASTEXITCODE -gt $worst) { $worst = $LASTEXITCODE }
        }
        if ($skipped -gt 0) { Write-Msg ("  導入より前のコミット {0} 件は見ていません" -f $skipped) }
        exit $worst
    }

    # --- 比較の両端と、メッセージ ---------------------------------------
    if ($Staged)
    {
        if (-not $MessageFile -or -not (Test-Path $MessageFile))
        {
            Write-Msg ("  -MessageFile が見つかりません : {0}" -f $MessageFile) -ForegroundColor Red
            Complete 2
        }
        if ((Invoke-Git @("rev-parse", "-q", "--verify", "MERGE_HEAD")).ExitCode -eq 0) { Complete 0 }

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
        $result.Commit = $sha
        if ($parents.Count -gt 1) { Complete 0 }

        $base = $(if ($parents.Count -eq 1) { $parents[0] } else { $emptyTree })
        $diffEnds = @($base, $sha)
        $newSpec  = { param($path) "${sha}:$path" }
        $message  = @((Invoke-GitChecked @("log", "-1", "--format=%B", $sha)).Lines)
        $label    = "コミット " + $sha.Substring(0, 8)
    }
    $result.Target = $label
    $result.DiffCommand = "git diff -M " + ($diffEnds -join " ")

    function Add-Finding([string]$Kind, [string]$Path, [int]$Count = 0)
    {
        $script:result.Findings += [pscustomobject]@{ Kind = $Kind; Path = $Path; Count = $Count }
    }

    # --- 仕組み自体・ファイル単位の期待値・キーごとの引数 ----------------
    $names = (Invoke-GitChecked (@("diff", "-M", "--name-status") + $diffEnds + @("--", ":(top)"))).Lines
    $pairs = @{}          # FileRules の比較の対（規則の番号 → 対の並び）
    $keyed = @{}          # KeyedArgumentRules（規則の番号 → @{ Old; New }）
    for ($k = 0; $k -lt @($rules.KeyedArgumentRules).Count; $k++) { $keyed[$k] = @{ Old = @{}; New = @{} } }

    foreach ($n in $names)
    {
        $f = $n -split "`t"
        $status = $f[0].Substring(0, 1)
        $oldPath = $f[1]
        $newPath = $f[$f.Count - 1]

        # 仕組みのファイルなら、ファイル単位の期待値としては見ない（二重に数えない）
        if ($status -ne "A" -and (Test-AnyMatch $oldPath $guard))
        {
            Add-Finding "仕組みの変更" $(if ($oldPath -eq $newPath) { $oldPath } else { "$oldPath → $newPath" })
        }
        else
        {
            for ($k = 0; $k -lt @($rules.FileRules).Count; $k++)
            {
                $r = @($rules.FileRules)[$k]
                if (-not ((Test-AnyMatch $oldPath $r.Paths) -or (Test-AnyMatch $newPath $r.Paths))) { continue }
                if ($status -eq "A") { Add-Finding ($r.Name + " の追加") $newPath; break }
                if ($status -eq "D") { Add-Finding ($r.Name + " の削除") $oldPath; break }
                if (-not $r.Comparer) { Add-Finding ($r.Name + " の変更") $newPath; break }

                $old = [IO.Path]::GetTempFileName(); $new = [IO.Path]::GetTempFileName()
                $tempFiles += $old, $new
                [void](Invoke-GitChecked @("show", "${base}:$oldPath") $old)
                [void](Invoke-GitChecked @("show", (& $newSpec $newPath)) $new)
                if (-not $pairs.ContainsKey($k)) { $pairs[$k] = @() }
                $pairs[$k] += [pscustomobject]@{ Path = $newPath; Expected = $old; Actual = $new }
                break
            }
        }

        # キーごとの引数は、コミット全体で比べる
        for ($k = 0; $k -lt @($rules.KeyedArgumentRules).Count; $k++)
        {
            $r = @($rules.KeyedArgumentRules)[$k]
            if ($status -ne "A" -and (Test-AnyMatch $oldPath $r.Paths) -and $base -ne $emptyTree) { Add-KeyedArguments (Get-Blob "${base}:$oldPath") $r $keyed[$k].Old }
            if ($status -ne "D" -and (Test-AnyMatch $newPath $r.Paths)) { Add-KeyedArguments (Get-Blob (& $newSpec $newPath)) $r $keyed[$k].New }
        }
    }

    # 差し込みの比較。Comparer は、リポジトリのルートからのパス（本体と同じ版を使う）。
    #   呼び方 : & <Comparer> -Pairs <Path・Expected・Actual の並び>
    #   返すもの : Path・Changed（真偽）・Detail（表示の補足。省略可）のオブジェクト
    foreach ($k in ($pairs.Keys | Sort-Object))
    {
        $r = @($rules.FileRules)[$k]
        $cmpTop = $(if ($self) { $self.Top } else { (Get-Location).ProviderPath })
        $comparer = $(if ([IO.Path]::IsPathRooted($r.Comparer)) { $r.Comparer } else { Join-Path $cmpTop $r.Comparer })
        if (-not (Test-Path -LiteralPath $comparer)) { throw ("比較のスクリプトが見つかりません : {0}" -f $r.Comparer) }
        $out = @(& $comparer -Pairs $pairs[$k] 6>$null | Where-Object { $_ -and $_.PSObject.Properties["Changed"] })
        foreach ($p in $pairs[$k])
        {
            $o = $out | Where-Object { $_.Path -eq $p.Path } | Select-Object -Last 1
            if (-not $o) { throw ("比較の結果を受け取れませんでした : {0}" -f $p.Path) }
            if ($o.Changed)
            {
                $detail = $(if ($o.PSObject.Properties["Detail"] -and $o.Detail) { "（" + $o.Detail + "）" } else { "" })
                Add-Finding ($r.Name + " の変更") ($p.Path + $detail)
            }
        }
    }

    for ($k = 0; $k -lt @($rules.KeyedArgumentRules).Count; $k++)
    {
        $r = @($rules.KeyedArgumentRules)[$k]
        $old = $keyed[$k].Old; $new = $keyed[$k].New
        foreach ($id in ($old.Keys | Sort-Object))
        {
            if (-not $new.ContainsKey($id)) { Add-Finding $r.RemovedName $id; continue }
            if ($old[$id] -cne $new[$id]) { Add-Finding $r.ChangedName $id }
        }
    }

    # --- 行の差し引き -----------------------------------------------------
    #   1 回の diff を「diff --git」の行でファイルに分け、規則ごとに消えた行・足した行を数える。
    #   パスはリポジトリのルートからにする（:(top)）。カレントは変えないので、相対のパスでは、
    #   サブ フォルダで実行したときに何も拾わなくなる。
    if (@($rules.LineRules).Count -gt 0)
    {
        $scope = @($rules.DiffScope | ForEach-Object { ":(top)$_" })
        if ($scope.Count -eq 0) { $scope = @(":(top)") }
        $diff = (Invoke-GitChecked (@("diff", "-M", "-U0", "--no-color") + $diffEnds + @("--") + $scope)).Lines
        $file = $null
        $active = @()
        $count = @{}
        foreach ($l in $diff)
        {
            if ($l -match '^diff --git a/.+ b/(.+)$')
            {
                $file = $Matches[1]
                $active = @()
                for ($k = 0; $k -lt @($rules.LineRules).Count; $k++) { if (Test-AnyMatch $file @($rules.LineRules)[$k].Paths) { $active += $k } }
                if ($active.Count -eq 0) { $file = $null; continue }
                $count[$file] = @{}
                foreach ($k in $active) { $count[$file][$k] = @{ Del = 0; Add = 0 } }
                continue
            }
            if (-not $file) { continue }
            if ($l.StartsWith("+++") -or $l.StartsWith("---")) { continue }
            if (-not ($l.StartsWith("+") -or $l.StartsWith("-"))) { continue }

            $body = $l.Substring(1)
            if ($body -match $rules.CommentLine) { continue }
            $del = $l.StartsWith("-")
            foreach ($k in $active)
            {
                foreach ($m in @(@($rules.LineRules)[$k].Match))
                {
                    if ($m.Paths -and -not (Test-AnyMatch $file $m.Paths)) { continue }
                    if ($body -match $m.Pattern)
                    {
                        if ($del) { $count[$file][$k].Del++ } else { $count[$file][$k].Add++ }
                        break
                    }
                }
            }
        }
        foreach ($p in ($count.Keys | Sort-Object))
        {
            foreach ($k in ($count[$p].Keys | Sort-Object))
            {
                $r = @($rules.LineRules)[$k]; $c = $count[$p][$k]
                $n = $(if ($r.Count -eq "Added") { $c.Add - $c.Del } else { $c.Del - $c.Add })
                if ($n -gt 0) { Add-Finding $r.Name $p $n }
            }
        }
    }

    # --- 要確認（ReviewRules） ---------------------------------------------
    #   止めない。ファイルごとに、当たる行の「足した数 − 消した数」が残れば一覧に出す（移しただけなら出ない）。
    #   注釈のために、足した行の番号（新しい側）を覚えておく。
    #   範囲は ReviewScope（省略時はリポジトリ全体）で、規則の Paths で絞る。**DiffScope は使わない。**
    #   DiffScope はテストの在りかで、要確認が見たい製品のコードはその外にある（MultiPurposeAuthSite の DiffScope は root/programs/Tests/）。
    if (@($rules.ReviewRules).Count -gt 0)
    {
        $scope = @($rules.ReviewScope | ForEach-Object { ":(top)$_" })
        if ($scope.Count -eq 0) { $scope = @(":(top)") }
        $diff = (Invoke-GitChecked (@("diff", "-M", "-U0", "--no-color") + $diffEnds + @("--") + $scope)).Lines
        $file = $null
        $lineNo = 0
        $rv = @{}   # パス → 規則の番号 → @{ Add; Del; Lines }
        foreach ($l in $diff)
        {
            if ($l -match '^diff --git a/.+ b/(.+)$')
            {
                $file = $Matches[1]
                $active = @()
                for ($k = 0; $k -lt @($rules.ReviewRules).Count; $k++) { if (Test-AnyMatch $file @($rules.ReviewRules)[$k].Paths) { $active += $k } }
                if ($active.Count -eq 0) { $file = $null }
                continue
            }
            if (-not $file) { continue }
            if ($l -match '^@@ -\d+(,\d+)? \+(\d+)(,\d+)? @@') { $lineNo = [int]$Matches[2]; continue }
            if ($l.StartsWith("+++") -or $l.StartsWith("---")) { continue }
            if (-not ($l.StartsWith("+") -or $l.StartsWith("-"))) { continue }

            $add = $l.StartsWith("+")
            $body = $l.Substring(1)
            $here = $lineNo
            if ($add) { $lineNo++ }
            if ($body -match $rules.CommentLine) { continue }
            foreach ($k in $active)
            {
                if ($body -notmatch @($rules.ReviewRules)[$k].Pattern) { continue }
                if (-not $rv.ContainsKey($file)) { $rv[$file] = @{} }
                if (-not $rv[$file].ContainsKey($k)) { $rv[$file][$k] = @{ Add = 0; Del = 0; Lines = @() } }
                if ($add) { $rv[$file][$k].Add++; $rv[$file][$k].Lines += $here } else { $rv[$file][$k].Del++ }
            }
        }
        foreach ($p in ($rv.Keys | Sort-Object))
        {
            foreach ($k in ($rv[$p].Keys | Sort-Object))
            {
                $r = @($rules.ReviewRules)[$k]; $c = $rv[$p][$k]
                $n = $c.Add - $c.Del
                if ($n -gt 0) { $result.Review += [pscustomobject]@{ Kind = $r.Name; Path = $p; Count = $n; Lines = @($c.Lines); Hint = $r.Hint } }
            }
        }
        if ($result.Review.Count -gt 0)
        {
            Write-Msg ""
            Write-Msg ("  要確認 {0} 件（{1}）。止めない。レビューで判断を確かめる" -f $result.Review.Count, $label) -ForegroundColor Cyan
            foreach ($x in $result.Review)
            {
                $hint = $(if ($x.Hint) { "（" + $x.Hint + "）" } else { "" })
                if ($env:GITHUB_ACTIONS -eq "true")
                {
                    foreach ($ln in $x.Lines) { Write-Msg ("::notice file={0},line={1},title=要確認：{2}::{3}{4}" -f $x.Path, $ln, $x.Kind, $label, $hint) }
                }
                Write-Msg ("    {0} : {1}（{2} 件。行 {3}）{4}" -f $x.Kind, $x.Path, $x.Count, ($x.Lines -join ","), $hint)
            }
        }
    }

    # --- トレーラーの検査 -------------------------------------------------
    #   git と同じ読み方（メッセージの最後の段落）で読む。集計（git log の %(trailers)）に載らない行は、書き方の誤りとして止める。
    #   誤りごとに、書いた行と直した行を出す（人がそのまま写して直せるように）。
    $body = (Get-MessageLines $message) -join "`n"
    $parsed = Get-GitTrailers $message
    $approvalSyntax = "{0}: <何が・なぜ変わったか（仕様の該当箇所・Issue 番号）>" -f $rules.ApprovalTrailer
    $vocabOf = @{}
    foreach ($t in @($rules.TrailerRules)) { $vocabOf[$t.Name] = Get-RuleVocabulary $t $(if ($Staged) { $null } else { $sha }) }
    $fixes = @()       # 直す行：Name・Written（書いた行）・Fixed（直した行）・Reasons
    $helpNames = @()   # 書き方と語彙の一覧を出す規則の名前

    # 書き方・置き場所（git に読まれない行）
    $trailerNames = @(@($rules.TrailerRules) | ForEach-Object { $_.Name }) + @($rules.ApprovalTrailer)
    foreach ($p in (Find-TrailerProblems $message $trailerNames $parsed))
    {
        $rule = @($rules.TrailerRules) | Where-Object { $_.Name -eq $p.Name } | Select-Object -First 1
        $value = $p.Value
        $reasons = @($p.Reasons)
        if ($rule)
        {
            $v = Test-TrailerValue $rule $value $vocabOf[$rule.Name]
            $reasons += $v.Problems
            $value = $v.Fixed
            $helpNames += $rule.Name
        }
        elseif ($value -eq "") { $reasons += "値（理由）がありません"; $value = ($approvalSyntax -replace '^[^:]+:\s*', '') }
        $fixes += [pscustomobject]@{ Name = $p.Name; Written = $p.Written; Fixed = ("{0}: {1}" -f $p.Name, $value); Reasons = $reasons; Key = $(if ($p.Read) { $p.Written.Trim() } else { $null }) }
    }

    # 値（語彙）と、書き忘れ
    foreach ($t in @($rules.TrailerRules))
    {
        foreach ($tr in @(@($parsed) | Where-Object { $_ -and $_.Key -ieq $t.Name }))
        {
            $v = Test-TrailerValue $t $tr.Value $vocabOf[$t.Name]
            if ($v.Problems.Count -eq 0) { continue }
            $fixes += [pscustomobject]@{ Name = $t.Name; Written = ("{0}: {1}" -f $tr.Key, $tr.Value); Fixed = ("{0}: {1}" -f $t.Name, $v.Fixed); Reasons = $v.Problems; Key = ("{0}: {1}" -f $tr.Key, $tr.Value) }
            $helpNames += $t.Name
        }
        $values = @(Select-Trailer $parsed $t.Name)
        $pending = @($fixes | Where-Object { $_.Name -eq $t.Name }).Count -gt 0   # 書いたが読まれない行は、直す行に出ている
        if ($values.Count -eq 0 -and -not $pending -and $t.RequiredWhen -and $body -match $t.RequiredWhen)
        {
            $result.Warnings += ("{0}（{1}）に {2}: がありません。{3}" -f $label, $Matches[0].Trim(), $t.Name, $t.MissingHint)
            $helpNames += $t.Name
        }
    }

    # 書き忘れ（TrailerRequirements。#6）。止めない。
    #   組の中の条件（Message・Trailer・Paths）がすべて当たるのに、Name のトレーラー（Value があれば、それに当たる値）が無ければ警告する。
    #   Paths は変えたファイルに当て、IgnoreLine があれば、それに当たる行（コメント・空行）だけの変更は数えない。
    $changedPaths = @($names | Where-Object { $_ } | ForEach-Object { ($_ -split "`t")[-1] })
    foreach ($q in @($rules.TrailerRequirements))
    {
        if (-not $q -or -not $q.Name) { continue }
        $why = @()
        if ($q.Message)
        {
            if (-not ($body -match $q.Message)) { continue }
            $why += $Matches[0].Trim()
        }
        if ($q.Trailer)
        {
            if (@(Select-Trailer $parsed $q.Trailer | Where-Object { $_ -ne "" }).Count -eq 0) { continue }
            $why += ("{0}: がある" -f $q.Trailer)
        }
        if (@($q.Paths | Where-Object { $_ }).Count -gt 0)   # 書いていない組は @($null) が 1 件になるので、空を除いて数える
        {
            $hit = @($changedPaths | Where-Object { Test-AnyMatch $_ @($q.Paths) })
            if ($q.IgnoreLine -and $hit.Count -gt 0)
            {
                $hit = @($hit | Where-Object {
                    $lines = (Invoke-GitChecked (@("diff", "-U0", "--no-color") + $diffEnds + @("--", ":(top)$_"))).Lines
                    @($lines | Where-Object { ($_.StartsWith("+") -or $_.StartsWith("-")) -and -not ($_.StartsWith("+++") -or $_.StartsWith("---")) -and ($_.Substring(1) -notmatch $q.IgnoreLine) }).Count -gt 0
                })
            }
            if ($hit.Count -eq 0) { continue }
            $why += ("{0}{1}" -f $hit[0], $(if ($hit.Count -gt 1) { " ほか {0} 件" -f ($hit.Count - 1) } else { "" }))
        }
        if ($why.Count -eq 0) { continue }
        $vals = @(Select-Trailer $parsed $q.Name | Where-Object { $_ -ne "" })
        if ($q.Value) { $vals = @($vals | Where-Object { $_ -match $q.Value }) }
        if ($vals.Count -gt 0) { continue }
        if (@($fixes | Where-Object { $_.Name -eq $q.Name }).Count -gt 0) { continue }   # 書いたが読まれない行は、直す行に出ている
        # Value は正規表現なので、表示では記号を除く（'^(quality-reviewer|human)\b' → 'quality-reviewer・human'）
        $what = $(if ($q.Value) { "{0}: {1}" -f $q.Name, ($q.Value -replace '\\b|\^|\$|[()]', '' -replace '\|', '・') } else { "{0}:" -f $q.Name })
        $result.Warnings += ("{0}（{1}）に {2} がありません。{3}" -f $label, ($why -join "・"), $what, $q.Hint)
        if (@($rules.TrailerRules | Where-Object { $_.Name -eq $q.Name }).Count -gt 0) { $helpNames += $q.Name }
    }

    foreach ($tr in @(@($parsed) | Where-Object { $_ -and $_.Key -ieq $rules.ApprovalTrailer -and $_.Value -eq "" }))
    {
        $fixes += [pscustomobject]@{ Name = $rules.ApprovalTrailer; Written = ("{0}:" -f $tr.Key); Fixed = $approvalSyntax; Reasons = @("値（理由）がありません"); Key = ("{0}: " -f $tr.Key) }
    }
    foreach ($f in $fixes) { $result.Errors += ("{0}（{1}）" -f $f.Written.Trim(), ($f.Reasons -join "。")) }
    foreach ($w in $result.Warnings) { Write-Warn $w }

    # 直し方（コミットを作り直す手順）。止めるときに最後に出す
    function Write-FixHow
    {
        Write-Msg ""
        Write-Msg "  直し方："
        if ($Staged)
        {
            Write-Msg "    コミットは作られていません。書いたメッセージは残っているので、開き直して直し、もう一度コミットする。"
            Write-Msg '      git commit -e -F "$(git rev-parse --git-path COMMIT_EDITMSG)"'
            Write-Msg "    （最初に -a などを付けていたら、同じものを付ける。GUI なら、画面のメッセージを直してもう一度コミットする）"
        }
        else
        {
            $rebaseBase = $(if ($parents.Count -eq 0) { "--root" } else { $sha.Substring(0, 8) + "^" })
            Write-Msg "    コミットのメッセージを書き直す。"
            Write-Msg "      最新のコミットなら    : git commit --amend"
            Write-Msg ("      それより前のコミットなら : git rebase -i {0}（このコミットの pick を reword にする）" -f $rebaseBase)
            Write-Msg "    push 済みなら git push --force-with-lease が要る（PR の履歴が書き換わるので、人が判断する）。"
        }
        Write-Msg ""
    }

    # --- 判定 -----------------------------------------------------------
    $exit = 0
    if ($fixes.Count -gt 0)
    {
        Write-Msg ""
        Write-Msg ("  **トレーラーを直してください**（{0}）" -f $label) -ForegroundColor Red
        foreach ($f in $fixes)
        {
            if ($env:GITHUB_ACTIONS -eq "true") { Write-Msg ("::error title=トレーラー：{0}::{1}：{2} → {3}" -f $f.Name, $label, $f.Written.Trim(), $f.Fixed) }
            Write-Msg ("    " + ($f.Reasons -join "。")) -ForegroundColor Yellow
            Write-Msg ("      書いた行：" + $f.Written)
            Write-Msg ("      直した行：" + $f.Fixed)
        }
        $exit = 1
    }
    # 書き方・例・語彙の一覧（語と意味）。意味の無い語は語だけ出す
    foreach ($t in @($rules.TrailerRules | Where-Object { $helpNames -contains $_.Name }))
    {
        $vocab = $vocabOf[$t.Name]
        Write-Msg ""
        Write-Msg ("  {0} の書き方：{1}" -f $t.Name, $(if ($t.Syntax) { $t.Syntax } else { "{0}: <語> [<補足>]" -f $t.Name }))
        if ($t.Example) { Write-Msg ("    例：" + $t.Example) }
        if ($vocab.Checked)
        {
            Write-Msg ("    {0}は次から選ぶ" -f $(if ($t.Placeholder) { $t.Placeholder } else { "<語>" }))
            foreach ($w in @($vocab.Words))
            {
                $m = $(if ($vocab.Meanings.ContainsKey($w)) { "：" + $vocab.Meanings[$w] } else { "" })
                Write-Msg ("      {0}{1}" -f $w, $m)
            }
        }
    }
    if ($fixes.Count -gt 0 -or $helpNames.Count -gt 0)
    {
        Write-Msg ""
        Write-Msg "  トレーラーは、メッセージの最後の段落（本文との間に空行を 1 つ）に、1 行に 1 つずつ書く。"
        Write-Msg "  git は最後の段落だけをトレーラーとして読む（集計に載る）。Co-Authored-By などとは、空行を挟まずに同じ段落に並べる。"
    }
    # 直した最後の段落（そのまま貼れる形）。読まれなかった行を前に、git が読んでいた行（直したものは直した行）を後に
    if ($fixes.Count -gt 0)
    {
        $block = @($fixes | Where-Object { -not $_.Key } | ForEach-Object { $_.Fixed })
        foreach ($tr in @($parsed))
        {
            if (-not $tr) { continue }
            $line = "{0}: {1}" -f $tr.Key, $tr.Value
            $f = @($fixes | Where-Object { $_.Key -and $_.Key.TrimEnd() -ceq $line.TrimEnd() }) | Select-Object -First 1
            $block += $(if ($f) { $f.Fixed } else { $line })
        }
        Write-Msg ""
        Write-Msg "  直した最後の段落（本文の後に空行を 1 つ置き、この行をそのまま置く。< > の所は書き換える。最後の段落に無かった行は、元の場所から消す）："
        foreach ($l in $block) { Write-Msg ("    " + $l) }
    }
    if ($result.Findings.Count -eq 0)
    {
        if ($exit -eq 1) { Write-FixHow }
        Complete $exit
    }

    $trailers = @(Select-Trailer $parsed $rules.ApprovalTrailer | Where-Object { $_ -ne "" })
    if ($trailers.Count -gt 0)
    {
        $result.Approved = $trailers
        Write-Msg ("  期待値の変更 {0} 件（{1}）。トレーラーで承認済み : {2}" -f $result.Findings.Count, $label, ($trailers -join " / "))
        # 理由の書式（OpenTouryoCodingAgentAssets#6）。止めない。Issue 番号・仕様の節の無い理由は、突き合わせ（根拠）ができない
        $patterns = @($rules.ApprovalReasonPatterns)
        if ($patterns.Count -gt 0)
        {
            foreach ($t in $trailers)
            {
                if (@($patterns | Where-Object { $t -match $_ }).Count -gt 0) { continue }
                $w = ("{0}: の理由に、Issue 番号・仕様の節がありません（{1}）。{2}" -f $rules.ApprovalTrailer, $t, $rules.ApprovalReasonHint)
                $result.Warnings += $w
                Write-Warn $w
            }
        }
        if ($exit -eq 1) { Write-FixHow }
        Complete $exit
    }

    # 承認のトレーラーを書いたが git が読まない（直す行に出ている）なら、「無い」とは言わず、直した行で直すよう促す
    $approvalWritten = @($fixes | Where-Object { $_.Name -eq $rules.ApprovalTrailer }).Count -gt 0
    $width = ($result.Findings | ForEach-Object { $_.Kind.Length } | Measure-Object -Maximum).Maximum
    Write-Msg ""
    $head = $(if ($approvalWritten) { "**期待値の変更が承認になっていません**（{0}。書いた {1}: の書き方が誤っているため）" } else { "**トレーラーの無い期待値の変更があります**（{0}" })
    Write-Msg (($head -f $label, $rules.ApprovalTrailer) + $(if ($approvalWritten) { "" } else { $(if ($rules.Reference) { "。" + $rules.Reference } else { "" }) + "）" })) -ForegroundColor Red
    foreach ($x in $result.Findings)
    {
        $tail = $(if ($x.Count -gt 0) { "（{0} 件）" -f $x.Count } else { "" })
        Write-Msg ("    {0} : {1}{2}" -f $x.Kind.PadRight($width, [char]0x3000), $x.Path, $tail) -ForegroundColor Yellow
    }
    Write-Msg ""
    if ($approvalWritten)
    {
        Write-Msg ("  上の「直した行」のとおりに {0}: を直せば、承認になる。" -f $rules.ApprovalTrailer)
    }
    else
    {
        Write-Msg "  差分を確かめたうえで、コミット メッセージの最後の段落に、理由を 1 行で書いてください。"
        Write-Msg ("    " + $approvalSyntax)
        Write-Msg ("    例：{0}: 一覧の初期表示の件数を 20 件に変えた（仕様の変更。#123）" -f $rules.ApprovalTrailer)
    }
    Write-FixHow
    Complete 1
}
catch
{
    Write-Msg ("  期待値の変更を確かめられませんでした : {0}" -f $_.Exception.Message) -ForegroundColor Red
    $result.Errors += $_.Exception.Message
    $result.ExitCode = 2
    Write-Result
    exit 2
}
finally
{
    foreach ($t in $tempFiles) { Remove-Item -LiteralPath $t -Force -ErrorAction SilentlyContinue }
}
