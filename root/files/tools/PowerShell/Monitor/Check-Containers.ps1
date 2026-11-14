<#
.SYNOPSIS
    コンテナ(Docker/Podman)の実行状況を1つの表で表示します。
.PARAMETER All    停止中のコンテナも含めます。
.PARAMETER Stats  CPU/メモリ列を表示します。
.PARAMETER K8s    k8s_ で始まるコンテナ(Rancher Desktop等)も個別表示します。既定は件数のみ。
.PARAMETER Image  イメージ列を表示します。
.PARAMETER Engine docker / podman (未指定時は自動検出)。
.PARAMETER Watch  指定秒数ごとに更新します (Ctrl+Cで終了)。
.EXAMPLE
    .\Check-Containers.ps1 -All -Stats
#>
[CmdletBinding()]
param(
    [switch]$All,
    [switch]$Stats,
    [switch]$K8s,
    [switch]$Image,
    [ValidateSet('docker', 'podman')]
    [string]$Engine,
    [int]$Watch = 0
)

$ErrorActionPreference = 'Continue'

if (-not $Engine) {
    foreach ($e in 'docker', 'podman') {
        if (Get-Command $e -ErrorAction SilentlyContinue) { $Engine = $e; break }
    }
}
if (-not $Engine) { Write-Error 'docker または podman が見つかりません。'; exit 1 }

function Format-Ports([string]$ports) {
    if (-not $ports) { return '' }
    ($ports -split ',\s*' |
        Where-Object { $_ -notmatch '^\[::\]' } |
        ForEach-Object { ($_ -replace '^0\.0\.0\.0:', '') -replace '/tcp$', '' } |
        Select-Object -Unique) -join ', '
}

# k8s_<container>_<pod>_<ns>_<uid>_<n> -> ns/container
function Format-K8sName([string]$n) {
    $p = $n -split '_'
    if ($p.Count -ge 5) { return "$($p[3])/$($p[1])" }
    $n
}

function Show-Status {
    & $Engine info 2>$null | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "$Engine デーモンに接続できません。" -ForegroundColor Red
        return $false
    }

    $psArgs = @('ps', '--format', '{{json .}}')
    if ($All) { $psArgs += '-a' }
    $everything = @(& $Engine @psArgs 2>$null | Where-Object { $_ } | ForEach-Object { $_ | ConvertFrom-Json })

    $k8sList = @($everything | Where-Object { $_.Names -like 'k8s_*' })
    $list = @($everything | Where-Object { $_.Names -notlike 'k8s_*' })
    # k8s個別表示: pause(POD)コンテナは除外
    if ($K8s) { $list += @($k8sList | Where-Object { $_.Names -notlike 'k8s_POD_*' }) }

    $statMap = @{}
    if ($Stats) {
        & $Engine stats --no-stream --format '{{json .}}' 2>$null | Where-Object { $_ } |
            ForEach-Object { $s = $_ | ConvertFrom-Json; $statMap[$s.Name] = $s }
    }

    $rows = foreach ($c in $list) {
        $name = if ($c.Names -like 'k8s_*') { Format-K8sName $c.Names } else { $c.Names }
        $s = $statMap[$c.Names]
        [pscustomobject]@{
            Name   = $name
            State  = $c.State
            Status = $c.Status -replace '^Up ', ''
            Cpu    = if ($s) { $s.CPUPerc } else { '' }
            Mem    = if ($s) { ($s.MemUsage -split ' / ')[0] } else { '' }
            Ports  = Format-Ports $c.Ports
            Image  = $c.Image
        }
    }

    $runAll = @($everything | Where-Object State -eq 'running').Count
    Write-Host ("[{0}] {1}  実行中 {2} / 全 {3}" -f $Engine, (Get-Date -Format 'HH:mm:ss'), $runAll, $everything.Count) -ForegroundColor Cyan

    if ($rows) {
        $cols = @('Name', 'State', 'Status')
        if ($Stats) { $cols += 'Cpu', 'Mem' }
        $cols += 'Ports'
        if ($Image) { $cols += 'Image' }

        $w = @{}
        foreach ($k in $cols) {
            $w[$k] = [Math]::Max($k.Length, (($rows | ForEach-Object { "$($_.$k)".Length }) | Measure-Object -Maximum).Maximum)
        }
        $fmt = { param($r) ($cols | ForEach-Object { "$($r.$_)".PadRight($w[$_]) }) -join '  ' }

        $hdr = [pscustomobject]@{}
        foreach ($k in $cols) { $hdr | Add-Member NoteProperty $k $k.ToUpper() }
        Write-Host (& $fmt $hdr).TrimEnd() -ForegroundColor DarkGray

        foreach ($r in $rows) {
            $color = switch ($r.State) {
                'running'    { 'Green' }
                'exited'     { 'DarkGray' }
                'created'    { 'DarkGray' }
                'restarting' { 'Yellow' }
                'paused'     { 'Yellow' }
                default      { 'Red' }
            }
            if ($r.Status -match 'unhealthy') { $color = 'Red' }
            elseif ($r.Status -match 'starting') { $color = 'Yellow' }
            elseif ($r.State -eq 'exited' -and $r.Status -notmatch 'Exited \(0\)') { $color = 'DarkYellow' }
            Write-Host (& $fmt $r).TrimEnd() -ForegroundColor $color
        }
    }
    else {
        Write-Host '対象のコンテナはありません。' -ForegroundColor Yellow
    }

    if (-not $K8s -and $k8sList.Count -gt 0) {
        $kr = @($k8sList | Where-Object State -eq 'running').Count
        Write-Host ("k8s: 実行中 {0} / 全 {1}  (個別表示は -K8s)" -f $kr, $k8sList.Count) -ForegroundColor DarkGray
    }
    return $true
}

if ($Watch -gt 0) {
    while ($true) {
        Clear-Host
        [void](Show-Status)
        Start-Sleep -Seconds $Watch
    }
}
else {
    if (-not (Show-Status)) { exit 1 }
}
