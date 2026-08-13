# --- 引数 ---
# Lang : 対象の言語。CS（既定）/ VB / Both。1 と 3 にそのまま渡す。
[CmdletBinding()]
param(
    [ValidateSet("CS", "VB", "Both")]
    [string]$Lang = "CS"
)

# --- 処理部：他のps1ファイルを順次実行 ---
# ※ ダブル クリック起動でもカレント ディレクトリに依存しないよう $PSScriptRoot を使う。
# ※ 順序は固定（RELEASE.md 3 節）。1 のクリーンとアセンブリ配置が 2・3 の前提になる。
#
# UseLang = $true のものにだけ -Lang を渡す。
# 2_RunAllTests.ps1 に渡さないのは、VB 側にテスト プロジェクトが無く（#542）、
# 単体テストが CS の Frameworks\Tests に集約されているため。
$scripts = @(
    @{ Name = "1_BuildAll.ps1";    UseLang = $true }
    @{ Name = "2_RunAllTests.ps1"; UseLang = $false }
    @{ Name = "3_SmokeTest.ps1";   UseLang = $true }
)
$results = @()

foreach ($s in $scripts)
{
    # -Lang VB では単体テストの対象が無いので飛ばす。
    # 黙って飛ばすと「通った」と読めてしまうため、結果には必ず残す。
    if ($Lang -eq "VB" -and -not $s.UseLang)
    {
        Write-Host ("{0} は VB 版の対象外のため飛ばします。" -f $s.Name) -ForegroundColor Yellow
        $results += [pscustomobject]@{ スクリプト = $s.Name; 終了コード = "対象外" }
        continue
    }

    $splat = @{}
    if ($s.UseLang) { $splat.Lang = $Lang }

    & (Join-Path $PSScriptRoot $s.Name) @splat
    $results += [pscustomobject]@{ スクリプト = $s.Name; 終了コード = $LASTEXITCODE }
}

# --- 結果のまとめ ---
Write-Host ""
Write-Host "================ 全体のまとめ ================"
$results | Format-Table -AutoSize | Out-String | Write-Host

# 1_BuildAll.ps1 は既知の署名エラー（MSB3482）で 1 になることがある。
# 終了コードだけで判断せず、エラー一覧の内容を確認すること（RELEASE.md 3 節）。
#
# 「対象外」は飛ばした印であって失敗ではないので、NG に数えない。
$ng = @($results | Where-Object { $_.終了コード -ne 0 -and $_.終了コード -ne "対象外" })
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
