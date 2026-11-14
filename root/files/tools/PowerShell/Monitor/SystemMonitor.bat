@echo off
powershell.exe -ExecutionPolicy Bypass -File ".\SystemMonitor.ps1" "-Interval" "20" "-Top" "10"
pause
