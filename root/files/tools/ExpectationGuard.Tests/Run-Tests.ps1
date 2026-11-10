# expectation-guard-source: OpenTouryoCodingAgentAssets@b9da2ca0cc9d src/quality/ExpectationGuard/tests/Run-Tests.ps1 sha256:eac9ea0c8127077ce728aa84ec27d39aca41fc5a206ed2fae1e8e58fa4cca4d7
<#
.SYNOPSIS
    期待値の変更の検出器の、判定の用例（cases.psd1）を回す（OpenTouryoCodingAgentAssets#3）。

.DESCRIPTION
    用例ごとに一時のリポジトリを作り、検出器（このフォルダの 1 つ上の CheckExpectationChange.ps1 と rules.common.psd1）を
    guard/ に写し、規則は rules.test.psd1 を guard/rules.psd1 として置く。導入先の規則（rules.psd1）は使わない。
    **本体の判定を確かめるもので、導入先の規則を確かめるものではない。**

    原本（src/quality/ExpectationGuard/tests/）でも、導入先に写したもの（<本体のフォルダ>/ExpectationGuard.Tests/）でも動く。

.PARAMETER Id
    回す用例の Id（省略時はすべて）。

.PARAMETER Keep
    一時のリポジトリを消さない（失敗の調べに使う）。

.OUTPUTS
    終了コード 0 = すべて期待どおり、1 = 期待と違う用例がある、2 = 実行できない
#>
[CmdletBinding()]
param(
    [string[]]$Id,
    [switch]$Keep
)

$ErrorActionPreference = "Stop"

$here    = $PSScriptRoot
# 本体の場所。install.ps1 が写すときに、導入先のルートからのフォルダで埋める（判定の用例を本体と別の場所に置けるように。#13）。
#   埋まっていなければ（原本のまま）、1 つ上のフォルダ。
$checkerDir = 'root/programs'
if ($checkerDir.StartsWith('@@'))
{
    $guard = Split-Path -Parent $here
}
else
{
    $top = (& git -C $here rev-parse --show-toplevel 2>$null)
    if ($LASTEXITCODE -ne 0 -or -not $top) { Write-Host ("  リポジトリの中で実行してください : {0}" -f $here) -ForegroundColor Red; exit 2 }
    $guard = [IO.Path]::GetFullPath((Join-Path $top $checkerDir))
}
$checker = Join-Path $guard "CheckExpectationChange.ps1"
$common  = Join-Path $guard "rules.common.psd1"
foreach ($f in @($checker, $common, (Join-Path $here "cases.psd1"), (Join-Path $here "rules.test.psd1"), (Join-Path $here "TestComparer.ps1")))
{
    if (-not (Test-Path -LiteralPath $f)) { Write-Host ("  見つかりません : {0}" -f $f) -ForegroundColor Red; exit 2 }
}

# Import-PowerShellDataFile は使わない（本体の Read-DataFile と同じ理由）
$sb = [scriptblock]::Create([IO.File]::ReadAllText((Join-Path $here "cases.psd1")))
$sb.CheckRestrictedLanguage([string[]]@(), [string[]]@(), $false)
$data  = & $sb
$cases = @($data.Cases)
if ($Id) { $cases = @($cases | Where-Object { $_.Id -in $Id }) }

$utf8 = New-Object Text.UTF8Encoding $false

function Invoke-GitQuiet([string]$Repo, [string[]]$Arguments)
{
    $out = & git -C $Repo -c core.autocrlf=false -c core.safecrlf=false @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) { throw ("git {0} が失敗しました : {1}" -f ($Arguments -join " "), ($out -join "`n")) }
}

function Get-Content2([string]$Value)
{
    if ($Value -match '^@fixture:(.+)$') { $Value = $data.Fixtures[$Matches[1]] }
    return (($Value -replace "`r`n", "`n").TrimEnd("`n") + "`n")
}

function Write-RepoFile([string]$Repo, [string]$Path, [string]$Value)
{
    $full = Join-Path $Repo $Path
    $dir = Split-Path -Parent $full
    if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
    [IO.File]::WriteAllText($full, (Get-Content2 $Value), $utf8)
}

function Add-Guard([string]$Repo)
{
    $g = Join-Path $Repo "guard"
    New-Item -ItemType Directory -Force $g | Out-Null
    Copy-Item -LiteralPath $checker -Destination (Join-Path $g "CheckExpectationChange.ps1")
    Copy-Item -LiteralPath $common  -Destination (Join-Path $g "rules.common.psd1")
    Copy-Item -LiteralPath (Join-Path $here "rules.test.psd1") -Destination (Join-Path $g "rules.psd1")
    Copy-Item -LiteralPath (Join-Path $here "TestComparer.ps1") -Destination (Join-Path $g "TestComparer.ps1")
}

function Invoke-Case($Case)
{
    $repo = Join-Path ([IO.Path]::GetTempPath()) ("eg-" + $Case.Id + "-" + [guid]::NewGuid().ToString("N").Substring(0, 8))
    New-Item -ItemType Directory -Force $repo | Out-Null
    $repo = (Resolve-Path $repo).ProviderPath
    try
    {
        Invoke-GitQuiet $repo @("init", "-q")
        Invoke-GitQuiet $repo @("config", "user.name", "expectation-guard-tests")
        Invoke-GitQuiet $repo @("config", "user.email", "expectation-guard-tests@localhost")
        Invoke-GitQuiet $repo @("config", "commit.gpgsign", "false")
        Invoke-GitQuiet $repo @("config", "core.autocrlf", "false")

        $guardAt = $(if ($Case.GuardAt) { [int]$Case.GuardAt } else { 0 })
        foreach ($k in $data.Base.Keys) { Write-RepoFile $repo $k $data.Base[$k] }
        if ($guardAt -eq 0) { Add-Guard $repo }
        Invoke-GitQuiet $repo @("add", "-A")
        Invoke-GitQuiet $repo @("commit", "-q", "-m", "base")

        $mode = $(if ($Case.Mode) { $Case.Mode } else { "Commit" })
        $commits = @($Case.Commits | Where-Object { $_ })
        $msgFile = Join-Path $repo ".git\CASE_MSG"
        for ($i = 0; $i -lt $commits.Count; $i++)
        {
            $c = $commits[$i]
            if ($guardAt -eq $i + 1) { Add-Guard $repo }
            if ($c.Files) { foreach ($k in $c.Files.Keys) { Write-RepoFile $repo $k $c.Files[$k] } }
            foreach ($d in @($c.Delete)) { if ($d) { Remove-Item -LiteralPath (Join-Path $repo $d) -Force } }
            Invoke-GitQuiet $repo @("add", "-A")
            [IO.File]::WriteAllText($msgFile, (Get-Content2 $c.Message), $utf8)
            if ($mode -eq "Staged" -and $i -eq $commits.Count - 1) { break }
            Invoke-GitQuiet $repo @("commit", "-q", "--allow-empty", "-F", $msgFile)
        }

        $body = Join-Path $repo "guard\CheckExpectationChange.ps1"
        if ($mode -eq "PreToolUse")
        {
            $json = (ConvertTo-Json $Case.Input -Depth 6 -Compress) -replace '@ROOT@', ($repo -replace '\\', '/')
            $psi = New-Object Diagnostics.ProcessStartInfo
            $psi.FileName = (Get-Process -Id $PID).Path
            $psi.Arguments = '-NoProfile -ExecutionPolicy Bypass -File "{0}" -PreToolUse' -f $body
            $psi.UseShellExecute = $false
            $psi.RedirectStandardInput = $true
            $psi.RedirectStandardOutput = $true
            $psi.RedirectStandardError = $true
            $psi.StandardOutputEncoding = $utf8
            $psi.EnvironmentVariables["CLAUDE_PROJECT_DIR"] = $repo
            $p = [Diagnostics.Process]::Start($psi)
            $bytes = $utf8.GetBytes($json)
            $p.StandardInput.BaseStream.Write($bytes, 0, $bytes.Length)
            $p.StandardInput.Close()
            $errTask = $p.StandardError.ReadToEndAsync()
            $out = $p.StandardOutput.ReadToEnd()
            $p.WaitForExit()
            $ask = $false
            if ($out.Trim()) { $ask = ((ConvertFrom-Json $out).hookSpecificOutput.permissionDecision -eq "ask") }
            return [pscustomobject]@{ Exit = $p.ExitCode; Kinds = @(); Warnings = 0; Ask = $ask; Detail = ($out.Trim() + " " + $errTask.Result.Trim()) }
        }

        Push-Location $repo
        try
        {
            $root = (& git rev-list --max-parents=0 HEAD)
            switch ($mode)
            {
                "Commit"    { $r = @(& $body -Commit HEAD -PassThru -Quiet) }
                "Staged"    { $r = @(& $body -Staged -MessageFile $msgFile -PassThru -Quiet) }
                "Range"     { $r = @(& $body -Range "$root..HEAD" -PassThru -Quiet) }
                "Generated" { $r = @(& $body -Range "$root..HEAD" -CheckGenerated -PassThru -Quiet) }
                default     { throw "不明な Mode : $mode" }
            }
            $code = $LASTEXITCODE
        }
        finally { Pop-Location }

        $r = @($r | Where-Object { $_ -and $_.PSObject.Properties["Findings"] })
        $kinds = @($r | ForEach-Object { $_.Findings } | ForEach-Object { $_.Kind } | Sort-Object -Unique)
        $warn = @($r | ForEach-Object { $_.Warnings }).Count
        # 要確認（ReviewRules）：種類の並び（重ねない）と、件数（ファイル×種類）
        $reviewItems = @($r | Where-Object { $_.PSObject.Properties["Review"] } | ForEach-Object { $_.Review })
        $reviewKinds = @($reviewItems | ForEach-Object { $_.Kind } | Sort-Object -Unique)
        $detail = (@($r | ForEach-Object { $_.Findings } | ForEach-Object { "{0} : {1}" -f $_.Kind, $_.Path }) + @($reviewItems | ForEach-Object { "要確認 {0} : {1}" -f $_.Kind, $_.Path }) + @($r | ForEach-Object { $_.Errors })) -join " / "
        $errs = @($r | ForEach-Object { $_.Errors }).Count
        return [pscustomobject]@{ Exit = $code; Kinds = $kinds; Warnings = $warn; Errors = $errs; Review = $reviewItems.Count; ReviewKinds = $reviewKinds; Ask = $null; Detail = $detail }
    }
    finally
    {
        if ($Keep) { Write-Host ("    （残した : {0}）" -f $repo) }
        else { Remove-Item -LiteralPath $repo -Recurse -Force -ErrorAction SilentlyContinue }
    }
}

$ng = 0
foreach ($c in $cases)
{
    $e = $c.Expect
    try
    {
        $a = Invoke-Case $c
        $why = @()
        if ($null -ne $e.Exit -and $a.Exit -ne [int]$e.Exit) { $why += ("終了コード {0}（期待 {1}）" -f $a.Exit, $e.Exit) }
        if ($null -ne $e.Kinds)
        {
            $want = @($e.Kinds | Sort-Object -Unique)
            if (($want -join "|") -cne (@($a.Kinds) -join "|")) { $why += ("検出 [{0}]（期待 [{1}]）" -f (@($a.Kinds) -join ", "), ($want -join ", ")) }
        }
        if ($null -ne $e.Warnings -and $a.Warnings -ne [int]$e.Warnings) { $why += ("警告 {0} 件（期待 {1}）" -f $a.Warnings, $e.Warnings) }
        if ($null -ne $e.Errors -and $a.Errors -ne [int]$e.Errors) { $why += ("誤り {0} 件（期待 {1}）" -f $a.Errors, $e.Errors) }
        if ($null -ne $e.Review -and $a.Review -ne [int]$e.Review) { $why += ("要確認 {0} 件（期待 {1}）" -f $a.Review, $e.Review) }
        if ($null -ne $e.ReviewKinds)
        {
            $want = @($e.ReviewKinds | Sort-Object -Unique)
            if (($want -join "|") -cne (@($a.ReviewKinds) -join "|")) { $why += ("要確認 [{0}]（期待 [{1}]）" -f (@($a.ReviewKinds) -join ", "), ($want -join ", ")) }
        }
        if ($null -ne $e.Ask -and $a.Ask -ne [bool]$e.Ask) { $why += ("ask {0}（期待 {1}）" -f $a.Ask, $e.Ask) }
    }
    catch
    {
        $why = @("実行できない : " + $_.Exception.Message)
        $a = $null
    }

    if ($why.Count -eq 0)
    {
        Write-Host ("  OK  {0}  [{1}] {2}" -f $c.Id, $c.Category, $c.Title)
    }
    else
    {
        $ng++
        Write-Host ("  NG  {0}  [{1}] {2}" -f $c.Id, $c.Category, $c.Title) -ForegroundColor Red
        foreach ($w in $why) { Write-Host ("        " + $w) -ForegroundColor Yellow }
        if ($a -and $a.Detail) { Write-Host ("        " + $a.Detail) }
    }
}

Write-Host ""
Write-Host ("  {0} 件のうち、期待と違うもの {1} 件" -f $cases.Count, $ng) -ForegroundColor $(if ($ng -eq 0) { "Green" } else { "Red" })
exit $(if ($ng -eq 0) { 0 } else { 1 })
