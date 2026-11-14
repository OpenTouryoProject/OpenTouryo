@echo off
powershell.exe -ExecutionPolicy Bypass -File ".\Check-Containers.ps1" "-Stats" "-Watch" "20"
pause
