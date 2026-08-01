# --- 処理部：他のps1ファイルを順次実行 ---
& .\BuildAll.ps1
& .\RunAllTests.ps1
& .\SmokeTest.ps1

# --- 画面を残すための処理 ---
Write-Host "`nすべての処理が完了しました。" -ForegroundColor Green
Read-Host "Enterキーを押すとウィンドウを閉じます"