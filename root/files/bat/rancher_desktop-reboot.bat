@echo off
setlocal

echo Rancher Desktop を終了しています...
rdctl shutdown

echo アプリの終了を待機中（最大 60 秒）...
set /a n=0
:waitexit
tasklist /fi "imagename eq Rancher Desktop.exe" 2>nul | find /i "Rancher Desktop.exe" > nul
if errorlevel 1 goto exited
set /a n+=1
if %n% geq 30 (
    echo 終了しないため、強制終了します。
    taskkill /f /t /im "Rancher Desktop.exe" > nul 2>&1
    goto exited
)
timeout /t 2 /nobreak > nul
goto waitexit
:exited

echo WSL ディストリビューションを停止しています...
wsl --terminate rancher-desktop > nul 2>&1
wsl --terminate rancher-desktop-data > nul 2>&1
timeout /t 3 /nobreak > nul

echo Rancher Desktop を起動しています...
rdctl start
if errorlevel 1 (
    echo 起動に失敗しました。終了コード: %errorlevel%
    pause
    exit /b 1
)

echo バックエンドの起動を待機中（最大 180 秒）...
set /a n=0
:waitstart
rdctl list-settings > nul 2>&1
if not errorlevel 1 goto started
set /a n+=1
if %n% geq 60 (
    echo タイムアウトしました。タスクトレイの状態を確認してください。
    pause
    exit /b 1
)
timeout /t 3 /nobreak > nul
goto waitstart
:started

echo 再起動が完了しました。
pause
