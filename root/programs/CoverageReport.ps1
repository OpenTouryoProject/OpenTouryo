<#
.SYNOPSIS
    NUnit のカバレッジ（coverage.cobertura.xml）を合算して、COVERAGE.md の表に書き込む。

.DESCRIPTION
    2_RunAllTests.ps1 -Coverage がドット ソースで読む部品。

    ＜何をするか＞
      1 回の実行で作られた net10.0 のカバレッジ ファイル（TestPublic・TestPublicNLog・TestPublicDb・TestFramework と、
      Linux の回・クロス DB の各回）を、アセンブリごとに**通った行の和集合**で数え、COVERAGE.md の表を作り直す。
      **前の値とは混ぜない。** 表は毎回丸ごと置き換える。

      例：OpenTouryo.Public には、TestPublic で通った行に加えて、
          TestPublicNLog（NLog の経路）と TestPublicDb（BaseDam・DamSqlSvr など）で通った行も入る。
          Dam（DamManagedOdp 等）には、その DBMS を起動した回で通った行が入る。

    ＜行と分岐＞
      行  … (ファイルのフル パス, 行番号) で重複を除き、どれかの回で 1 回でも通れば通ったと数える。
             **相対パスでは数えない**（<source> の根が計測ごとに違い、同じ行を二重に数えるため）。
      分岐 … 行ごとに、最も多くの分岐を通した回の数を採る（condition-coverage の「通過/全体」）。
             どの分岐を通ったかまではファイルに無いため、回をまたいだ和集合は取れない。
             **下限の値になる**（実際はこれ以上）。

    ＜書き換える範囲＞
      COVERAGE.md の <!-- COVERAGE:BEGIN --> から <!-- COVERAGE:END --> までだけ。
      その外（説明文）は人が書く。マーカーが無ければ何もしない。

.EXAMPLE
    . (Join-Path $PSScriptRoot "CoverageReport.ps1")
    $files = @([pscustomobject]@{ Measure = "NUnit TestPublic (net10.0)"; File = $path })
    Write-CoverageReport -Files $files -Path $md -RepoRoot $repoRoot

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/10/19  玄人 幸道         新規作成（2_RunAllTests.ps1 -Coverage の結果を COVERAGE.md に残す）
     2026/10/19  玄人 幸道         行をフル パスで数えるようにした（計測ごとに <source> の根が違い、二重に数えていた）
     2026/10/19  玄人 幸道         指定したアセンブリを、モジュールごとの行と計の行で出せるようにした（-Modules）
#>

# 書き換える範囲の印
$script:CoverageBegin = "<!-- COVERAGE:BEGIN -->"
$script:CoverageEnd   = "<!-- COVERAGE:END -->"

# いまのコミット（短縮）。ワーキング ツリーに未コミットの変更があれば「+」を付ける。
#   **テストの副産物（Result*.txt）と COVERAGE.md 自身は数えない。** 回すたびに書き換わるため、
#   数えると常に「+」になって意味が無くなる。
function Get-CoverageCommit([string]$RepoRoot)
{
    $sha = (& git -C $RepoRoot rev-parse --short HEAD 2>$null)
    if (-not $sha) { return "-" }

    $dirty = @(& git -C $RepoRoot status --porcelain -- . ":(exclude)**/Result*.txt" ":(exclude)**/COVERAGE.md" 2>$null |
               Where-Object { $_ })
    if ($dirty.Count -gt 0) { return "$sha+" }
    return $sha
}

# class の filename を、比べられるフル パス（区切りは /、小文字）にする。
#   <source> が複数あるときは、そのファイルが実在するものを選ぶ（無ければ最初の <source>）。
function Resolve-CoverageFile([string]$FileName, [string[]]$Sources)
{
    $f = $FileName -replace '\\', '/'
    $full = $f
    if (-not ($f -match '^[A-Za-z]:/' -or $f.StartsWith('/')) -and $Sources.Count -gt 0)
    {
        $full = ($Sources[0] -replace '\\', '/').TrimEnd('/') + '/' + $f
        foreach ($s in $Sources)
        {
            $cand = ($s -replace '\\', '/').TrimEnd('/') + '/' + $f
            if (Test-Path -LiteralPath $cand) { $full = $cand; break }
        }
    }
    return $full.ToLowerInvariant()
}

# 複数のカバレッジ ファイルを、アセンブリ（package）ごとに合算する。
#   Files : Measure（計測名）と File（coverage.cobertura.xml のパス）を持つオブジェクトの配列
function Get-CoverageUnion([object[]]$Files)
{
    # アセンブリ → @{ Lines = 行の集合（キー → 通ったか）; Branch = 行キー → @(通過, 全体) }
    $acc = @{}
    $latest = 0L

    foreach ($f in $Files)
    {
        if (-not (Test-Path $f.File)) { continue }
        [xml]$cx = Get-Content $f.File -Encoding UTF8 -Raw

        $ts = 0L
        if ([long]::TryParse("$($cx.coverage.timestamp)", [ref]$ts) -and $ts -gt $latest) { $latest = $ts }

        # **行は「ファイルのフル パス＋行番号」で数える。**
        #   class の filename は <source>（coverlet が実行ごとに決める共通の根）からの相対パスで、
        #   計測ごとに根が違う（TestFramework の回は Framework を含むので、根が 1 つ上になる）。
        #   相対パスのまま数えると、同じ行が別の行として二重に数えられる。
        $sources = @($cx.coverage.sources.source | ForEach-Object { "$_" } | Where-Object { $_ })
        $fullOf  = @{}

        foreach ($p in @($cx.coverage.packages.package))
        {
            if (-not $acc.ContainsKey($p.name)) { $acc[$p.name] = @{ Lines = @{}; Branch = @{} } }
            $a = $acc[$p.name]

            foreach ($c in @($p.classes.class))
            {
                $fn = "$($c.filename)"
                if (-not $fullOf.ContainsKey($fn)) { $fullOf[$fn] = Resolve-CoverageFile $fn $sources }
                $full = $fullOf[$fn]

                foreach ($l in @($c.lines.line))
                {
                    if ($null -eq $l) { continue }
                    $k = "$full|$($l.number)"

                    $hit = ([int]$l.hits -gt 0)
                    if (-not $a.Lines.ContainsKey($k)) { $a.Lines[$k] = $hit }
                    elseif ($hit) { $a.Lines[$k] = $true }

                    # 分岐のある行は condition-coverage="50% (1/2)"
                    if ("$($l.'condition-coverage')" -match '\((\d+)/(\d+)\)')
                    {
                        $cov = [int]$Matches[1]; $all = [int]$Matches[2]
                        if (-not $a.Branch.ContainsKey($k) -or $a.Branch[$k][0] -lt $cov) { $a.Branch[$k] = @($cov, $all) }
                    }
                }
            }
        }
    }

    $rows = @()
    foreach ($name in ($acc.Keys | Sort-Object))
    {
        $a = $acc[$name]
        $lineValid   = $a.Lines.Count
        $lineCovered = @($a.Lines.Values | Where-Object { $_ }).Count
        $brCovered = 0; $brValid = 0
        foreach ($b in $a.Branch.Values) { $brCovered += $b[0]; $brValid += $b[1] }

        $rows += [pscustomobject]@{
            Assembly      = $name
            LineCovered   = $lineCovered
            LineValid     = $lineValid
            BranchCovered = $brCovered
            BranchValid   = $brValid
        }
    }

    $date = "-"
    if ($latest -gt 0)
    {
        $date = ([DateTimeOffset]::FromUnixTimeSeconds($latest)).ToLocalTime().ToString("yyyy/MM/dd HH:mm")
    }

    return [pscustomobject]@{ Rows = $rows; Date = $date; Acc = $acc }
}

# アセンブリのうち、指定したモジュール（ファイル）ごとの行を作る。最後に、それらの計の行を足す。
#   Module : プロジェクトのフォルダからの相対パス（例 "Authentication/SAML2Client.cs"）。
#            フル パスの末尾で照合する（大文字小文字は区別しない）。
function Get-CoverageModuleRows([hashtable]$AssemblyAcc, [string]$Assembly, [string[]]$Modules)
{
    $rows = @()
    $totLine = 0; $totLineValid = 0; $totBr = 0; $totBrValid = 0

    foreach ($m in $Modules)
    {
        $suffix = "/" + ($m -replace '\\', '/').ToLowerInvariant() + "|"
        $lineValid = 0; $lineCovered = 0; $brCovered = 0; $brValid = 0

        foreach ($k in $AssemblyAcc.Lines.Keys)
        {
            # キーは「フル パス|行番号」。行番号の手前までが、モジュールのパスで終わるもの
            $path = $k.Substring(0, $k.LastIndexOf('|') + 1)
            if (-not $path.EndsWith($suffix)) { continue }

            $lineValid++
            if ($AssemblyAcc.Lines[$k]) { $lineCovered++ }
            if ($AssemblyAcc.Branch.ContainsKey($k)) { $brCovered += $AssemblyAcc.Branch[$k][0]; $brValid += $AssemblyAcc.Branch[$k][1] }
        }

        $rows += [pscustomobject]@{
            Assembly = ("{0} : {1}" -f $Assembly, $m)
            LineCovered = $lineCovered; LineValid = $lineValid; BranchCovered = $brCovered; BranchValid = $brValid
        }
        $totLine += $lineCovered; $totLineValid += $lineValid; $totBr += $brCovered; $totBrValid += $brValid
    }

    $rows += [pscustomobject]@{
        Assembly = ("**{0}（対象モジュールの計）**" -f $Assembly)
        LineCovered = $totLine; LineValid = $totLineValid; BranchCovered = $totBr; BranchValid = $totBrValid
    }
    return $rows
}

# 通過 / 全体 → "76.6%"（全体が 0 なら "-"）
function Format-CoverageRate([int]$Covered, [int]$Valid)
{
    if ($Valid -le 0) { return "-" }
    return ("{0:N1}%" -f (100.0 * $Covered / $Valid))
}

# COVERAGE.md の BEGIN〜END を、今回のカバレッジで丸ごと置き換える。
#   Modules : アセンブリ名 → モジュールの一覧。ここに在るアセンブリは、全体の 1 行ではなく、
#             モジュールごとの行と、その計の行で出す（一部の部品だけを測るアセンブリのため）。
function Write-CoverageReport([object[]]$Files, [string]$Path, [string]$RepoRoot, [hashtable]$Modules)
{
    if (-not $Files -or $Files.Count -eq 0) { return $false }
    if (-not (Test-Path $Path))
    {
        Write-Host ("  COVERAGE.md が見つかりません : {0}" -f $Path) -ForegroundColor Yellow
        return $false
    }

    $text  = [IO.File]::ReadAllText($Path, [Text.Encoding]::UTF8)
    $begin = $text.IndexOf($script:CoverageBegin)
    $end   = $text.IndexOf($script:CoverageEnd)
    if ($begin -lt 0 -or $end -lt $begin)
    {
        Write-Host ("  COVERAGE.md に {0} / {1} がありません。書き換えません。" -f `
            $script:CoverageBegin, $script:CoverageEnd) -ForegroundColor Yellow
        return $false
    }

    $u = Get-CoverageUnion $Files

    $sb = New-Object Text.StringBuilder
    [void]$sb.AppendLine($script:CoverageBegin)
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine(("- 計測した日時 : {0}" -f $u.Date))
    [void]$sb.AppendLine(("- コミット : {0}" -f (Get-CoverageCommit $RepoRoot)))
    [void]$sb.AppendLine(("- 合算した計測（{0} 件） : {1}" -f $Files.Count, (($Files | ForEach-Object { $_.Measure }) -join " / ")))
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("| アセンブリ（: モジュール） | 行 | 行数（通過／全体） | 分岐（下限） | 分岐数（通過／全体） |")
    [void]$sb.AppendLine("|---|---:|---:|---:|---:|")

    # モジュールごとに出すアセンブリは、全体の 1 行を、モジュールの行と計の行に置き換える
    $tableRows = @()
    foreach ($r in $u.Rows)
    {
        if ($Modules -and $Modules.ContainsKey($r.Assembly))
        {
            $tableRows += Get-CoverageModuleRows $u.Acc[$r.Assembly] $r.Assembly @($Modules[$r.Assembly])
        }
        else
        {
            $tableRows += $r
        }
    }

    foreach ($r in $tableRows)
    {
        [void]$sb.AppendLine(("| {0} | {1} | {2:N0} / {3:N0} | {4} | {5:N0} / {6:N0} |" -f `
            $r.Assembly,
            (Format-CoverageRate $r.LineCovered $r.LineValid), $r.LineCovered, $r.LineValid,
            (Format-CoverageRate $r.BranchCovered $r.BranchValid), $r.BranchCovered, $r.BranchValid))
    }
    [void]$sb.AppendLine("")
    [void]$sb.Append($script:CoverageEnd)

    # 改行は CRLF に揃える（ワーキング ツリーの .md と同じ）。BOM は付けない（他の .md と同じ）。
    $newText = $text.Substring(0, $begin) + ($sb.ToString() -replace "`r?`n", "`r`n") +
               $text.Substring($end + $script:CoverageEnd.Length)
    [IO.File]::WriteAllText($Path, $newText, (New-Object Text.UTF8Encoding $false))
    return $true
}
