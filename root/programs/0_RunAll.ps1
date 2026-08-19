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

# --- 処理部 ---
# 1 → 2 → 3 を順に実行する。**その前に、設定ファイルの突き合わせを警告として行う**（#553）。
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

# --- 設定ファイルの突き合わせ（警告のみ）---（#553）
#
# **合否には影響させない。** CS / VB の設定の差は、ビルドや疎通とは別の観点である。
# ここで終了コードを汚すと、0_RunAll.ps1 の結果をそのまま合否として読めなくなる（#555）。
#
# 一覧は出さない（25 行が毎回流れると、本来の検証ログが読みにくくなる）。
# **異常があったときだけ「見に行け」と伝える。**
#
# -Lang に関わらず常に行う。比較は本質的に CS ↔ VB であり、片方だけを回すときにも意味がある。
function Test-ConfigSync
{
    # **6>$null が要る。** Write-Host はパイプラインに流れないので、
    # | Out-Null では一覧を抑止できない（PowerShell 5.0 以降は情報ストリーム）。
    & (Join-Path $PSScriptRoot "CompareConfig.ps1") -Check 6>$null | Out-Null
    return ($LASTEXITCODE -eq 0)
}

$configOk = Test-ConfigSync
if (-not $configOk)
{
    Write-Host ""
    Write-Host "【警告】設定ファイルに想定外の差分があります。" -ForegroundColor Yellow
    Write-Host "        .\CompareConfig.ps1 -Check で内容を確認してください。"
    Write-Host ""
}

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

    # --- bindingRedirect の突き合わせ（警告のみ）---（#556）
    #
    # **ビルドの直後に行う。** 見るのは「そのプロジェクトが配布するアセンブリ」なので、
    # ビルド前だと材料が無く「判定不能」ばかりになる。
    # 設定ファイルどうしを比べる CompareConfig.ps1（先頭で実行）とは、そこが違う。
    if ($s.Name -eq "1_BuildAll.ps1")
    {
        # 6>$null で一覧を抑止する（上と同じ理由）。
        & (Join-Path $PSScriptRoot "CompareRedirect.ps1") -Check 6>$null | Out-Null
        $redirectOk = ($LASTEXITCODE -eq 0)

        if (-not $redirectOk)
        {
            Write-Host ""
            Write-Host "【警告】bindingRedirect が、配布されないアセンブリの版を指しています。" -ForegroundColor Yellow
            Write-Host "        .\CompareRedirect.ps1 -Check で内容を確認してください。"
            Write-Host ""
        }

        # --- パッケージの版の突き合わせ（警告のみ）---（#569）
        #
        # **版は 4 か所に散らばる。**（#566 / #568）
        # packages.config を基準に、csproj のパス表記（②）と
        # Reference の Version（③）が外れていないかを見る。
        #
        # ③ は HintPath の DLL を読むため、**復元してからでないと判定できない。**
        # ここ（ビルドの直後）なら材料が揃っている。
        & (Join-Path $PSScriptRoot "ComparePackage.ps1") -Check 6>$null | Out-Null
        $packageOk = ($LASTEXITCODE -eq 0)

        if (-not $packageOk)
        {
            Write-Host ""
            Write-Host "【警告】packages.config と csproj で、パッケージの版が食い違っています。" -ForegroundColor Yellow
            Write-Host "        .\ComparePackage.ps1 -Check で内容を確認してください。"
            Write-Host ""
        }
    }
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

# 警告は、ここにも出す。
# **通しは長い。途中の警告は流れて見落とされる。**
if (-not $configOk)
{
    Write-Host ""
    Write-Host "【警告】設定ファイルに想定外の差分があります（CompareConfig.ps1 -Check）。" -ForegroundColor Yellow
    Write-Host "        合否には数えていません。CONFIGURATION.md 11 節を参照。"
}

if ($null -ne $redirectOk -and -not $redirectOk)
{
    Write-Host ""
    Write-Host "【警告】bindingRedirect が、配布されないアセンブリの版を指しています" -ForegroundColor Yellow
    Write-Host "        （CompareRedirect.ps1 -Check）。合否には数えていません。"
}

if ($null -ne $packageOk -and -not $packageOk)
{
    Write-Host ""
    Write-Host "【警告】packages.config と csproj で、パッケージの版が食い違っています" -ForegroundColor Yellow
    Write-Host "        （ComparePackage.ps1 -Check）。合否には数えていません。"
}

# --- 画面を残すための処理 ---
Read-Host "`nEnterキーを押すとウィンドウを閉じます"
