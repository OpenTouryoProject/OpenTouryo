# --- 処理部：他のps1ファイルを順次実行 ---
# ※ ダブル クリック起動でもカレント ディレクトリに依存しないよう $PSScriptRoot を使う。
# ※ 順序は固定（RELEASE.md 3 節）。1 のクリーンとアセンブリ配置が 2・3 の前提になる。
$scripts = @("1_BuildAll.ps1", "2_RunAllTests.ps1", "3_SmokeTest.ps1")
$results = @()

foreach ($s in $scripts)
{
    & (Join-Path $PSScriptRoot $s)
    $results += [pscustomobject]@{ スクリプト = $s; 終了コード = $LASTEXITCODE }
}

# --- 結果のまとめ ---
Write-Host ""
Write-Host "================ 全体のまとめ ================"
$results | Format-Table -AutoSize | Out-String | Write-Host

# 1_BuildAll.ps1 は既知の署名エラー（MSB3482）で 1 になることがある。
# 終了コードだけで判断せず、エラー一覧の内容を確認すること（RELEASE.md 3 節）。
$ng = @($results | Where-Object { $_.終了コード -ne 0 })
if ($ng.Count -eq 0)
{
    Write-Host "すべて OK です。" -ForegroundColor Green
}
else
{
    Write-Host ("{0} 本が 0 以外で終了しました。上のログを確認してください。" -f $ng.Count) -ForegroundColor Yellow
}

# --- 画面を残すための処理 ---
Read-Host "`nEnterキーを押すとウィンドウを閉じます"
