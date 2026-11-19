@echo off
echo Rancher Desktop を終了しています...
rdctl shutdown

echo 終了完了を待機中...
timeout /t 5 /nobreak > nul

echo Rancher Desktop を起動しています...
rdctl start

echo 再起動が完了しました。
pause