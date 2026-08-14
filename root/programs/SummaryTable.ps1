<#
.SYNOPSIS
    集計結果を、全角文字を含んでいても桁が揃う表として出力する。

.DESCRIPTION
    ＜なぜ Format-Table を使わないのか＞
      Windows PowerShell 5.1 の Format-Table は、列の幅を**表示桁数ではなく文字数**で
      決める。日本語（全角）は 1 文字で 2 桁を占めるため、見出し・罫線・データが
      それぞれ別々にずれる。PowerShell 7 は表示桁数で数えるので揃う。

        5.1                              7
        ステップ           結果      秒   ステップ                  結果        秒
        ----           --      -   --------                  ----        --
        Clean (net48 基盤)  OK   10.7   Clean (net48 基盤)        OK       10.70

      利用者は 5.1（powershell.exe）で実行するため、こちらに合わせる必要がある
      （CODING.md 5 節）。-AutoSize や -Wrap の指定では直らない。

    ＜対処＞
      表示桁数を自前で数え、PadRight ではなく空白の連結で桁を合わせる。
      .NET の PadRight も文字数で数えるため、使ってはいけない。

    ＜最終列＞
      最終列だけは幅を揃えず、コンソール幅に収まるよう折り返す
      （Format-Table -Wrap に相当）。内容が長い列を最後に置くこと。

.PARAMETER Rows
    表にするオブジェクトの配列。最初の要素のプロパティを、その順で列にする。

.PARAMETER Indent
    行頭に入れる空白の数。既定は 0。

.EXAMPLE
    . (Join-Path $PSScriptRoot "SummaryTable.ps1")
    Write-SummaryTable $results

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/13  玄人 幸道         新規作成（5.1 で全角を含む表の桁がずれるため）
#>

# 表示桁数を数える。
#
# 全角（East Asian Wide / Fullwidth）は 2 桁として数える。
# 範囲は Unicode の East Asian Width に基づく代表的なものに絞ってある。
# 本リポジトリの表に出るのは日本語・記号だけなので、これで足りる。
function Get-DisplayWidth
{
    param([string]$Text)

    if ([string]::IsNullOrEmpty($Text)) { return 0 }

    $w = 0
    foreach ($c in $Text.ToCharArray())
    {
        $n = [int]$c

        if (($n -ge 0x1100 -and $n -le 0x115F) -or   # ハングル字母
            ($n -ge 0x2E80 -and $n -le 0xA4CF) -or   # CJK 部首・かな・漢字・記号
            ($n -ge 0xAC00 -and $n -le 0xD7A3) -or   # ハングル音節
            ($n -ge 0xF900 -and $n -le 0xFAFF) -or   # CJK 互換漢字
            ($n -ge 0xFE30 -and $n -le 0xFE6F) -or   # CJK 互換形・小字形
            ($n -ge 0xFF00 -and $n -le 0xFF60) -or   # 全角英数・記号
            ($n -ge 0xFFE0 -and $n -le 0xFFE6))      # 全角通貨記号
        {
            $w += 2
        }
        else
        {
            $w += 1
        }
    }

    return $w
}

# 指定した表示桁数まで、右側を空白で埋める。
function Add-Padding
{
    param([string]$Text, [int]$Width)

    $pad = $Width - (Get-DisplayWidth $Text)
    if ($pad -le 0) { return $Text }
    return $Text + (' ' * $pad)
}

# 指定した表示桁数まで、左側を空白で埋める（数値列用）。
function Add-LeftPadding
{
    param([string]$Text, [int]$Width)

    $pad = $Width - (Get-DisplayWidth $Text)
    if ($pad -le 0) { return $Text }
    return (' ' * $pad) + $Text
}

# 表示桁数で折り返す。語の区切りを優先し、1 語で収まらないときだけ途中で切る。
function Split-ByWidth
{
    param([string]$Text, [int]$Width)

    if ([string]::IsNullOrEmpty($Text)) { return @('') }
    if ($Width -lt 1) { return @($Text) }

    $lines = New-Object System.Collections.Generic.List[string]
    $line  = ''

    foreach ($word in ($Text -split ' '))
    {
        $candidate = if ($line -eq '') { $word } else { $line + ' ' + $word }

        if ((Get-DisplayWidth $candidate) -le $Width)
        {
            $line = $candidate
            continue
        }

        if ($line -ne '') { $lines.Add($line); $line = '' }

        # 1 語で収まらないものは、表示桁数で切る。
        $rest = $word
        while ((Get-DisplayWidth $rest) -gt $Width)
        {
            $take = ''
            foreach ($c in $rest.ToCharArray())
            {
                if ((Get-DisplayWidth ($take + $c)) -gt $Width) { break }
                $take += $c
            }
            $lines.Add($take)
            $rest = $rest.Substring($take.Length)
        }
        $line = $rest
    }

    if ($line -ne '') { $lines.Add($line) }
    if ($lines.Count -eq 0) { $lines.Add('') }

    return $lines.ToArray()
}

function Write-SummaryTable
{
    param(
        [object[]]$Rows,
        [int]$Indent = 0
    )

    if ($null -eq $Rows -or $Rows.Count -eq 0) { return }

    $names = @($Rows[0].PSObject.Properties | ForEach-Object { $_.Name })
    if ($names.Count -eq 0) { return }

    # 値を文字列にしておく（$null は空文字）。
    $cells = @()
    foreach ($r in $Rows)
    {
        $row = @{}
        foreach ($n in $names)
        {
            $v = $r.$n
            if ($null -eq $v) { $row[$n] = '' } else { $row[$n] = [string]$v }
        }
        $cells += $row
    }

    # 数値だけの列は右寄せにする（Format-Table と同じ見え方にするため）。
    $numeric = @{}
    foreach ($n in $names)
    {
        $isNum = $true
        foreach ($row in $cells)
        {
            $s = $row[$n]
            if ($s -eq '') { continue }
            $d = 0.0
            if (-not [double]::TryParse($s, [ref]$d)) { $isNum = $false; break }
        }
        $numeric[$n] = $isNum
    }

    # 列幅（最終列を除く）
    $widths = @{}
    foreach ($n in $names)
    {
        $w = Get-DisplayWidth $n
        foreach ($row in $cells)
        {
            $v = Get-DisplayWidth $row[$n]
            if ($v -gt $w) { $w = $v }
        }
        $widths[$n] = $w
    }

    # コンソール幅。取得できない場合（リダイレクト時など）は 120 とする。
    $console = 0
    try { $console = $Host.UI.RawUI.WindowSize.Width } catch { }
    if ($console -le 0) { $console = 120 }

    $last = $names[$names.Count - 1]

    # 最終列を折り返すのは、文字列の列のときだけ。
    # 数値なら幅が知れているので、他の列と同じく揃えて右寄せする
    # （1_BuildAll.ps1 の「秒」など。Format-Table もそう見せていた）。
    $wrapLast = -not $numeric[$last]

    $head = $Indent
    foreach ($n in $names) { if ($n -ne $last) { $head += $widths[$n] + 1 } }

    $lastWidth = $console - $head - 1
    if ($lastWidth -lt 20) { $lastWidth = 20 }

    $pad = ' ' * $Indent

    # 1 セルを、列の幅まで詰めて返す。数値の列は右寄せ（見出しも合わせる）。
    function Format-Cell
    {
        param([string]$Text, [int]$Width, [bool]$Right)

        if ($Right) { return (Add-LeftPadding $Text $Width) }
        return (Add-Padding $Text $Width)
    }

    # 見出し
    $line = $pad
    foreach ($n in $names)
    {
        if ($n -eq $last -and $wrapLast) { $line += $n; break }
        $line += (Format-Cell $n $widths[$n] $numeric[$n])
        if ($n -ne $last) { $line += ' ' }
    }
    Write-Host $line

    # 罫線
    #
    # 折り返す最終列だけは、列幅ではなく**見出しの幅**に合わせる。
    # 列幅に合わせると、内容の一番長い行と同じだけ罫線が伸びてしまう。
    $line = $pad
    foreach ($n in $names)
    {
        $w = if ($n -eq $last -and $wrapLast) { Get-DisplayWidth $n } else { $widths[$n] }
        $line += ('-' * $w)
        if ($n -ne $last) { $line += ' ' }
    }
    Write-Host $line

    # 本体
    foreach ($row in $cells)
    {
        $line = $pad
        foreach ($n in $names)
        {
            if ($n -eq $last -and $wrapLast) { break }
            $line += (Format-Cell $row[$n] $widths[$n] $numeric[$n])
            if ($n -ne $last) { $line += ' ' }
        }

        if (-not $wrapLast)
        {
            Write-Host $line
            continue
        }

        # 最終列は折り返す。2 行目以降は列の位置まで下げる。
        #
        # **@() で受けること。** PowerShell は要素 1 個の配列を返すとスカラーに
        # 展開するため、そのまま [0] を取ると**文字列の 1 文字目**になる。
        $parts = @(Split-ByWidth $row[$last] $lastWidth)
        Write-Host ($line + $parts[0])

        for ($i = 1; $i -lt $parts.Count; $i++)
        {
            Write-Host ((' ' * $head) + $parts[$i])
        }
    }
}
