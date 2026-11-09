# expectation-guard-source: OpenTouryoCodingAgentAssets@6a3d9f49bcc3 src/quality/ExpectationGuard/CheckExpectationChange.ps1 sha256:4090217affb8199d8743407a2b1e626a24d9834078923cb6bd7e3fb622e2aaef
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
      TrailerRules       トレーラーの値の検査。語彙に無い値は止め、必要なのに無いときは警告する。
                         どちらのときも語彙の一覧（Meanings があれば意味も）を出す。
      GeneratedRules     -CheckGenerated のとき、生成物の作り直しの漏れを見る（期待値の変更の代わりに）。
    **アサーションの緩和は見ない**（機械的に判定できない）。レビューの観点で拾う。

    承認：コミット メッセージに「Expectation-Change: <理由>」の行があれば、人が承認したものとする。
    # で始まる行と、git commit -v の切り取り線から後は見ない。

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

.OUTPUTS
    終了コード 0 = 期待値の変更が無い、またはトレーラーがある（警告はあってもよい）
               1 = トレーラーの無い期待値の変更がある、語彙に無いトレーラーの値がある
                   （-CheckGenerated では、作り直しの漏れがある）
               2 = 実行できない（引数の誤り・規則の誤り・git の失敗）

.EXAMPLE
    .\CheckExpectationChange.ps1 -Commit HEAD
.EXAMPLE
    .\CheckExpectationChange.ps1 -Staged -MessageFile .git\COMMIT_EDITMSG
.EXAMPLE
    .\CheckExpectationChange.ps1 -Range develop..HEAD
.EXAMPLE
    .\CheckExpectationChange.ps1 -Range develop..HEAD -CheckGenerated

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
    [switch]$Quiet
)

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
    foreach ($k in @("Guard", "FileRules", "LineRules", "KeyedArgumentRules", "TrailerRules", "GeneratedRules", "DiffScope"))
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

function Get-Trailer([string[]]$Lines, [string]$Name)
{
    $found = @()
    foreach ($l in (Get-MessageLines $Lines))
    {
        if ($l -match ('^' + [regex]::Escape($Name) + ':\s*(\S.*)$')) { $found += $Matches[1].Trim() }
    }
    return , $found
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
$result = [pscustomobject]@{ Target = $null; Commit = $null; Findings = @(); Approved = @(); Warnings = @(); Errors = @(); ExitCode = 0 }

function Complete([int]$Code)
{
    $result.ExitCode = $Code
    if ($PassThru) { Write-Output $result }
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
        $commits = @((Invoke-GitChecked @("rev-list", "--no-merges", "--reverse", $Range)).Lines)
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
        if ($worst -eq 1)
        {
            Write-Msg "  既にあるコミットは、メッセージを書き直す（git rebase -i で reword）。push 済みなら force push になるので、人が判断する。"
        }
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

    # --- トレーラーの検査 -------------------------------------------------
    $body = (Get-MessageLines $message) -join "`n"
    $helpRules = @()   # 語彙の一覧（語と意味）を出す規則
    foreach ($t in @($rules.TrailerRules))
    {
        $values = Get-Trailer $message $t.Name
        $needHelp = $false
        foreach ($v in $values)
        {
            $word = ($v -split '\s+')[0]
            if ($t.Vocabulary -and ($word -cnotin @($t.Vocabulary)))
            {
                $result.Errors += ("{0}: {1} は語彙にありません（{2}）" -f $t.Name, $word, (@($t.Vocabulary) -join " / "))
                $needHelp = $true
            }
        }
        if ($values.Count -eq 0 -and $t.RequiredWhen -and $body -match $t.RequiredWhen)
        {
            $result.Warnings += ("{0}（{1}）に {2}: がありません。{3}" -f $label, $Matches[0].Trim(), $t.Name, $t.MissingHint)
            $needHelp = $true
        }
        if ($needHelp -and $t.Vocabulary) { $helpRules += $t }
    }
    foreach ($w in $result.Warnings) { Write-Warn $w }

    # --- 判定 -----------------------------------------------------------
    $exit = 0
    if ($result.Errors.Count -gt 0)
    {
        Write-Msg ""
        Write-Msg ("  **トレーラーの値が誤っています**（{0}）" -f $label) -ForegroundColor Red
        foreach ($x in $result.Errors) { Write-Msg ("    " + $x) -ForegroundColor Yellow }
        $exit = 1
    }
    # 語彙の一覧（語と意味）。意味は規則の Meanings（語 → 意味）。無い語は語だけ出す
    foreach ($t in $helpRules)
    {
        Write-Msg ""
        Write-Msg ("  {0}: の手段は次から選ぶ（{0}: <手段> [<補足>]）" -f $t.Name)
        foreach ($w in @($t.Vocabulary))
        {
            $m = $(if ($t.Meanings -and $t.Meanings.ContainsKey($w)) { "：" + $t.Meanings[$w] } else { "" })
            Write-Msg ("    {0}{1}" -f $w, $m)
        }
    }
    if ($result.Errors.Count -gt 0 -or $helpRules.Count -gt 0)
    {
        Write-Msg ""
    }
    if ($result.Findings.Count -eq 0) { Complete $exit }

    $trailers = Get-Trailer $message $rules.ApprovalTrailer
    if ($trailers.Count -gt 0)
    {
        $result.Approved = $trailers
        Write-Msg ("  期待値の変更 {0} 件（{1}）。トレーラーで承認済み : {2}" -f $result.Findings.Count, $label, ($trailers -join " / "))
        Complete $exit
    }

    $width = ($result.Findings | ForEach-Object { $_.Kind.Length } | Measure-Object -Maximum).Maximum
    Write-Msg ""
    Write-Msg ("  **トレーラーの無い期待値の変更があります**（{0}{1}）" -f $label, $(if ($rules.Reference) { "。" + $rules.Reference } else { "" })) -ForegroundColor Red
    foreach ($x in $result.Findings)
    {
        $tail = $(if ($x.Count -gt 0) { "（{0} 件）" -f $x.Count } else { "" })
        Write-Msg ("    {0} : {1}{2}" -f $x.Kind.PadRight($width, [char]0x3000), $x.Path, $tail) -ForegroundColor Yellow
    }
    Write-Msg ""
    Write-Msg "  差分を確かめたうえで、コミット メッセージの末尾に理由を書いてください。"
    Write-Msg ("    {0}: <何が・なぜ変わったか（仕様の該当箇所・Issue 番号）>" -f $rules.ApprovalTrailer)
    Write-Msg ""
    Complete 1
}
catch
{
    Write-Msg ("  期待値の変更を確かめられませんでした : {0}" -f $_.Exception.Message) -ForegroundColor Red
    $result.Errors += $_.Exception.Message
    $result.ExitCode = 2
    if ($PassThru) { Write-Output $result }
    exit 2
}
finally
{
    foreach ($t in $tempFiles) { Remove-Item -LiteralPath $t -Force -ErrorAction SilentlyContinue }
}
