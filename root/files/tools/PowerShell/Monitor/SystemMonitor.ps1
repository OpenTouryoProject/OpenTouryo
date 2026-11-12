<#
.SYNOPSIS
    メモリ使用量とCPU利用状況を一定間隔で表示します。
.PARAMETER Interval
    更新間隔（秒）。既定値は 3。
.PARAMETER Top
    表示する上位プロセス数。既定値は 5。
.EXAMPLE
    .\SystemMonitor.ps1
    .\SystemMonitor.ps1 -Interval 5 -Top 10
    終了するには Ctrl+C を押します。
#>
param(
    [int]$Interval = 3,
    [int]$Top = 5
)

function Get-Bar {
    param([double]$Percent, [int]$Width = 30)
    $filled = [math]::Round($Width * $Percent / 100)
    $filled = [math]::Max(0, [math]::Min($Width, $filled))
    return ('#' * $filled) + ('-' * ($Width - $filled))
}

function Get-LevelColor {
    param([double]$Percent)
    if ($Percent -ge 90) { 'Red' }
    elseif ($Percent -ge 70) { 'Yellow' }
    else { 'Green' }
}

$logicalCores = (Get-CimInstance Win32_ComputerSystem).NumberOfLogicalProcessors

try {
    try { [Console]::CursorVisible = $false } catch {}
    while ($true) {
        # --- CPU（ロケール非依存のパフォーマンスクラスを使用）---
        $cpuPerf  = Get-CimInstance Win32_PerfFormattedData_PerfOS_Processor
        $cpuTotal = [double]($cpuPerf | Where-Object Name -eq '_Total').PercentProcessorTime
        $cpuCores = $cpuPerf | Where-Object Name -ne '_Total' | Sort-Object { [int]$_.Name }

        # --- メモリ ---
        $os       = Get-CimInstance Win32_OperatingSystem
        $memTotal = $os.TotalVisibleMemorySize / 1MB   # KB -> GB
        $memFree  = $os.FreePhysicalMemory / 1MB
        $memUsed  = $memTotal - $memFree
        $memPct   = if ($memTotal -gt 0) { $memUsed / $memTotal * 100 } else { 0 }

        # --- 上位プロセス ---
        $procs = Get-CimInstance Win32_PerfFormattedData_PerfProc_Process |
            Where-Object { $_.Name -notin '_Total', 'Idle' }
        $topCpu = $procs | Sort-Object PercentProcessorTime -Descending | Select-Object -First $Top
        $topMem = $procs | Sort-Object WorkingSetPrivate -Descending | Select-Object -First $Top

        # --- 表示 ---
        Clear-Host
        Write-Host ("=== システムモニター  {0}  (更新間隔: {1}秒 / Ctrl+C で終了) ===" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Interval) -ForegroundColor Cyan
        Write-Host ''

        Write-Host ('CPU    [{0}] {1,5:N1} %' -f (Get-Bar $cpuTotal), $cpuTotal) -ForegroundColor (Get-LevelColor $cpuTotal)
        Write-Host ('メモリ [{0}] {1,5:N1} %  ({2:N2} / {3:N2} GB, 空き {4:N2} GB)' -f (Get-Bar $memPct), $memPct, $memUsed, $memTotal, $memFree) -ForegroundColor (Get-LevelColor $memPct)
        Write-Host ''

        Write-Host '--- コア別 CPU 使用率 ---' -ForegroundColor Cyan
        foreach ($c in $cpuCores) {
            $p = [double]$c.PercentProcessorTime
            Write-Host ('  Core {0,2} [{1}] {2,5:N1} %' -f $c.Name, (Get-Bar $p 20), $p) -ForegroundColor (Get-LevelColor $p)
        }
        Write-Host ''

        Write-Host "--- CPU 使用率 上位 $Top プロセス ---" -ForegroundColor Cyan
        $topCpu | ForEach-Object {
            [pscustomobject]@{
                'プロセス' = $_.Name
                'PID'      = $_.IDProcess
                'CPU(%)'   = '{0:N1}' -f ($_.PercentProcessorTime / $logicalCores)
                'メモリ(MB)' = '{0:N1}' -f ($_.WorkingSetPrivate / 1MB)
            }
        } | Format-Table -AutoSize | Out-String | Write-Host

        Write-Host "--- メモリ使用量 上位 $Top プロセス ---" -ForegroundColor Cyan
        $topMem | ForEach-Object {
            [pscustomobject]@{
                'プロセス' = $_.Name
                'PID'      = $_.IDProcess
                'CPU(%)'   = '{0:N1}' -f ($_.PercentProcessorTime / $logicalCores)
                'メモリ(MB)' = '{0:N1}' -f ($_.WorkingSetPrivate / 1MB)
            }
        } | Format-Table -AutoSize | Out-String | Write-Host

        Start-Sleep -Seconds $Interval
    }
}
finally {
    try { [Console]::CursorVisible = $true } catch {}
}
