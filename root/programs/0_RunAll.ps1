# --- 引数 ---
# Lang         : 対象の言語。CS（既定）/ VB / Both。1 と 3 にそのまま渡す。
# IgnoreErrors : 既知のエラーとして合否判定から外す正規表現。1 にだけ渡す。
#
# **既定で ClickOnce の署名エラーを除外する。**（#555）
#   WSClientWinCone_sample.csproj は証明書の拇印を直接指定しているため、
#   その証明書が無い環境では必ず失敗する（BUILDING.md 4 節）。
#   既定で除外しないと、**「NG が 1 本ある」が常態になり、本物の失敗と見分けが付かない。**
#
#   除外したものは 1_BuildAll.ps1 が件数と内容を別枠で表示するので、黙って消えはしない。
#   除外せずに回したいときは -IgnoreErrors @() を渡す。
[CmdletBinding()]
param(
    [ValidateSet("CS", "VB", "Both")]
    [string]$Lang = "CS",
    [string[]]$IgnoreErrors = @('error MSB(3482|3325|3321):.*WSClientWinCone_sample\.csproj')
)

# --- 処理部：他のps1ファイルを順次実行 ---
# ※ ダブル クリック起動でもカレント ディレクトリに依存しないよう $PSScriptRoot を使う。
# ※ 順序は固定（RELEASE.md 3 節）。1 のクリーンとアセンブリ配置が 2・3 の前提になる。
#
# UseLang = $true のものにだけ -Lang を渡す。
# 2_RunAllTests.ps1 に渡さないのは、VB 側にテスト プロジェクトが無く（#542）、
# 単体テストが CS の Frameworks\Tests に集約されているため。
#
# UseIgnore = $true のものにだけ -IgnoreErrors を渡す。
# 受け取るのは 1_BuildAll.ps1 だけである（2 と 3 は同名の引数を持たない）。

# まとめの整形。Format-Table は 5.1 で全角の桁を数えないため、自前で揃える。
. (Join-Path $PSScriptRoot "SummaryTable.ps1")

$scripts = @(
    @{ Name = "1_BuildAll.ps1";    UseLang = $true;  UseIgnore = $true }
    @{ Name = "2_RunAllTests.ps1"; UseLang = $false; UseIgnore = $false }
    @{ Name = "3_SmokeTest.ps1";   UseLang = $true;  UseIgnore = $false }
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
    if ($s.UseLang)   { $splat.Lang = $Lang }
    if ($s.UseIgnore) { $splat.IgnoreErrors = $IgnoreErrors }

    & (Join-Path $PSScriptRoot $s.Name) @splat
    $results += [pscustomobject]@{ スクリプト = $s.Name; 終了コード = $LASTEXITCODE }
}

# --- 結果のまとめ ---
Write-Host ""
Write-Host "================ 全体のまとめ ================"
Write-Host ""
Write-SummaryTable $results
Write-Host ""

# **終了コードをそのまま合否として読んでよい。**（#555）
#   既知の署名エラー（MSB3482）は -IgnoreErrors の既定値で除外しているため、
#   1_BuildAll.ps1 が 1 を返したら、それは**別の理由**である。
#   除外した内容は 1_BuildAll.ps1 のサマリに別枠で出るので、そちらも目は通すこと。
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
