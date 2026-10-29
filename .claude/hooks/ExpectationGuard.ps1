<#
.SYNOPSIS
    期待値の変更（QUALITY.md 3 節）になりうる編集を、人の確認付きにする（#640 の段 1）。

.DESCRIPTION
    Claude Code の PreToolUse フック（.claude/settings.json）から呼ばれる。
    ツールの入力（JSON）を標準入力で受け取り、該当すれば「ask」を返す。該当しなければ何も出さない。

    ＜確認付きにするもの＞
      Edit・Write・MultiEdit
        - Golden の期待値（Tests/Golden/**/Result*.txt）の編集
        - NUnit（Tests/NUnit/**/*.cs）の編集で、テストの属性（[Test]・[TestCase]・[TestCaseSource]）の行が減るもの、
          [Ignore]・[Explicit]・Assert.Ignore( の行が増えるもの、[Basis(...)] の行が減るもの（QUALITY.md 7 節）
      Bash・PowerShell
        - Tests/Golden を指していて、書き込む操作（リダイレクト・Set-Content・Copy-Item・cp・sed -i・git checkout など）を含むもの
          （**推測である。** 漏れは段 2・段 3 が塞ぐ。2_RunAllTests.ps1 の実行のような、パスを書かない書き換えは対象にしない）

    数え方は CheckExpectationChange.ps1 と同じ（// で始まる行は数えない）。
    **このフックは止めない。** 人に見せるだけで、許可すれば編集は進む。承認の記録は段 2・段 3 のトレーラーである。

    **失敗したら何もしない**（エージェントの作業を止めない）。

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/10/29  玄人 幸道         新規作成（#640 段 1）
     2026/10/29  玄人 幸道         期待値の根拠（[Basis]）が減る編集も確認付きにした（QUALITY.md 7 節）
#>

$goldenPattern = '/root/programs/[^/]+/Frameworks/Tests/Golden/.+/Result[^/]*\.txt$'
$nunitPattern  = '/root/programs/[^/]+/Frameworks/Tests/NUnit/.+\.cs$'
$testAttrPattern   = '[\[,]\s*(Test|TestCase|TestCaseSource)\s*[\],(]'
$ignoreAttrPattern = '[\[,]\s*(Ignore|Explicit)\s*[\],(]|\bAssert\s*\.\s*Ignore\s*\('
$basisAttrPattern  = '[\[,]\s*Basis\s*\('

# コマンドで Golden を書き換えそうな操作（2>$null・*>&1 のような、捨てるだけのリダイレクトは除く）
$goldenInCommand = 'Tests[\\/]+Golden'
$writeInCommand  = '(?<![0-9*])>(?!\s*(\$null|/dev/null|&))|\b(Set-Content|Add-Content|Out-File|Copy-Item|Move-Item|Remove-Item|Rename-Item|WriteAll(Text|Bytes|Lines))\b|(^|[\s;|&(])(cp|mv|rm|copy|move|del|tee)\s|\bsed\s+(-\w*\s+)*-i|\bgit\s+(checkout|restore|reset|stash|apply|am|cherry-pick)\b'

# 行を数える（// で始まる行は数えない）
function Get-AttrCount([string]$Text, [string]$Pattern)
{
    if (-not $Text) { return 0 }
    $n = 0
    foreach ($l in ($Text -split "`n"))
    {
        if ($l.TrimStart().StartsWith("//")) { continue }
        if ($l -match $Pattern) { $n++ }
    }
    return $n
}

# ask を返す。**コンソールのコード ページに触れずに、UTF-8 で書く。**
function Write-Ask([string]$Reason)
{
    $json = @{
        hookSpecificOutput = @{
            hookEventName            = "PreToolUse"
            permissionDecision       = "ask"
            permissionDecisionReason = $Reason + "（QUALITY.md 3 節。コミットにはトレーラー Expectation-Change: <理由> が要る）"
        }
    } | ConvertTo-Json -Compress -Depth 4
    $bytes = (New-Object Text.UTF8Encoding $false).GetBytes($json)
    $out = [Console]::OpenStandardOutput()
    $out.Write($bytes, 0, $bytes.Length)
    $out.Flush()
}

try
{
    $reader = New-Object IO.StreamReader([Console]::OpenStandardInput(), (New-Object Text.UTF8Encoding $false))
    $hook = $reader.ReadToEnd() | ConvertFrom-Json
    $tool = "$($hook.tool_name)"
    $in   = $hook.tool_input

    if ($tool -in @("Bash", "PowerShell"))
    {
        $cmd = "$($in.command)"
        if ($cmd -match $goldenInCommand -and $cmd -match $writeInCommand)
        {
            Write-Ask "Golden（期待値）を書き換える可能性のあるコマンドです"
        }
        exit 0
    }

    if ($tool -notin @("Edit", "Write", "MultiEdit")) { exit 0 }

    $path = ("$($in.file_path)" -replace '\\', '/')
    if ($path -match $goldenPattern)
    {
        Write-Ask ("Golden の期待値を編集します : {0}" -f [IO.Path]::GetFileName($path))
        exit 0
    }
    if ($path -notmatch $nunitPattern) { exit 0 }

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
    $testDel = (Get-AttrCount $b $testAttrPattern) - (Get-AttrCount $a $testAttrPattern)
    $ignAdd  = (Get-AttrCount $a $ignoreAttrPattern) - (Get-AttrCount $b $ignoreAttrPattern)
    if ($testDel -gt 0) { $reasons += ("テストの属性が {0} 件減ります" -f $testDel) }
    if ($ignAdd -gt 0)  { $reasons += ("Ignore・Explicit が {0} 件増えます" -f $ignAdd) }
    $basisDel = (Get-AttrCount $b $basisAttrPattern) - (Get-AttrCount $a $basisAttrPattern)
    if ($basisDel -gt 0) { $reasons += ("期待値の根拠（Basis）が {0} 件減ります" -f $basisDel) }

    if ($reasons.Count -gt 0)
    {
        Write-Ask ("NUnit のテストを変えます（{0}） : {1}" -f ($reasons -join "、"), [IO.Path]::GetFileName($path))
    }
    exit 0
}
catch
{
    # 失敗しても止めない。理由だけ標準エラーへ出す（Claude Code はブロックしない）
    [Console]::Error.WriteLine("ExpectationGuard.ps1: " + $_.Exception.Message)
    exit 1
}
