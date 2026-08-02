<#
.SYNOPSIS
    全ビルド（0_ExecAllBat.bat 相当）を実行し、エラー・警告を集約して合否を判定する。

.DESCRIPTION
    リリース時の「build バッチで全ビルドが通ることを確認」を機械化したもの（#513 段階 2）。

    既存のビルド バッチをそのまま呼び出し、出力を解析して結果を集約する。
    「何をビルドするか」の正はバッチ側に残し、本スクリプトは実行と判定のみを担う。

    ＜なぜラッパーが必要か＞
      ・各バッチは MSBuild の終了コードを伝播しない（%ERRORLEVEL% を見ていない）
      ・各バッチの末尾に pause があり、対話入力を待つ
      ・-v:d（詳細）で出力が膨大なため、目視での確認が難しい
    このため、stdin を与えて実行し、出力から error / warning を抽出して判定する。

.PARAMETER Only
    ステップ名の部分一致で対象を絞る（例: -Only "net48"）。動作確認用。

.PARAMETER SkipClean
    クリーン処理（1_DeleteDir / 1_DeleteFile）を省略する。
    ※ リリース判定では省略しないこと。前回のビルド成果物が残っていると、
    　 ビルドが通ったように見えることがある。

.PARAMETER OutputDir
    各ステップの出力ログの保存先。既定は %TEMP%\OpenTouryoBuildLogs。

.EXAMPLE
    .\1_BuildAll.ps1

.EXAMPLE
    .\1_BuildAll.ps1 -Only "Framework_Tool" -SkipClean

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
#>
[CmdletBinding()]
param(
    [string]$Only,
    [switch]$SkipClean,
    [string]$OutputDir = (Join-Path $env:TEMP "OpenTouryoBuildLogs")
)

# 本スクリプトは root\programs に置き、C# 側（root\programs\CS）を対象とする。
# ビルド バッチはすべて CS 直下にあるため、そこを起点にする。
$csRoot = Join-Path $PSScriptRoot "CS"
New-Item -ItemType Directory -Force $OutputDir | Out-Null

# ------------------------------------------------------------------
# コンソールのコード ページを先に UTF-8 にしておく
# ------------------------------------------------------------------
# z_Common.bat は先頭で chcp 65001 を実行する。コード ページはコンソール
# 全体の設定なので、子プロセスで変えても呼び出し元の画面に影響する。
# 日本語環境の既定は 932 で、932 → 65001 の切り替わりで画面が再描画され、
# **それまでに表示したステップの結果が消える**。
#
# z_Common.bat を最初に呼ぶのは 2_Build_NuGet_net48.bat（3 ステップ目）なので、
# 何も表示していない今のうちに切り替えておけば、実行中は変化しない。
#
# 元に戻さないのは、戻す操作でも再描画が起きてサマリが消えるため。
# 各ビルド バッチも 65001 にしたまま終了するので、挙動は従来と同じ。
if ((cmd /c chcp) -notmatch '65001')
{
    cmd /c chcp 65001 | Out-Null
}

# コンソールのコード ページとは別に、PowerShell 側の出力エンコードも合わせる。
# Windows PowerShell 5.1 は起動時の値を保持しており、実行中にコード ページが
# 変わっても追随しない。ここを合わせないと、バッチ（UTF-8 で出力）の内容を
# 旧コード ページで解釈してしまい、ログとエラー一覧が文字化けする。
# ※ 2 回目以降の実行ではコード ページが既に 65001 で上の分岐を通らないため、
#    この処理は分岐の外に置く必要がある。
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# ------------------------------------------------------------------
# ビルド ステップの定義（0_ExecAllBat.bat と同じ順序・同じ内容）
# ------------------------------------------------------------------
# Clean = $true のものは -SkipClean で省略される。
$steps = @(
    # --- net48 : 基盤 ---
    @{ Name = "Clean (net48 基盤)";            Bat = "1_DeleteDir.bat";                        Clean = $true }
    @{ Name = "Clean files (net48 基盤)";      Bat = "1_DeleteFile.bat";                       Clean = $true }
    @{ Name = "NuGet (net48)";                 Bat = "2_Build_NuGet_net48.bat" }
    @{ Name = "Business (net48)";              Bat = "3_Build_Business_net48.bat" }
    @{ Name = "Business.RichClient (net48)";   Bat = "3_Build_BusinessRichClient_net48.bat" }

    # --- netcore100 : 基盤 ---
    @{ Name = "Clean (core 基盤)";             Bat = "1_DeleteDir.bat";                        Clean = $true }
    @{ Name = "Clean files (core 基盤)";       Bat = "1_DeleteFile.bat";                       Clean = $true }
    @{ Name = "NuGet (netcore100)";            Bat = "2_Build_NuGet_netcore100.bat" }
    @{ Name = "Business (netcore100)";         Bat = "3_Build_Business_netcore100.bat" }
    @{ Name = "Business.RichClient (core)";    Bat = "3_Build_BusinessRichClient_netcore100.bat" }

    # --- 参照用アセンブリのコピー ---
    @{ Name = "CopyAssemblies";                Bat = "4_Build_CopyAssemblies.bat" }

    # --- net48 : ツールとサンプル ---
    @{ Name = "Clean (net48 サンプル)";        Bat = "1_DeleteDir.bat";                        Clean = $true }
    @{ Name = "Clean files (net48 サンプル)";  Bat = "1_DeleteFile.bat";                       Clean = $true }
    @{ Name = "Framework_Tool (net48)";        Bat = "4_Build_Framework_Tool.bat" }
    @{ Name = "2CS_sample (net48)";            Bat = "5_Build_2CS_sample.bat" }
    @{ Name = "Bat_sample (net48)";            Bat = "5_Build_Bat_sample.bat" }
    @{ Name = "CLI_sample (net48)";            Bat = "5_Build_CLI_sample.bat" }
    @{ Name = "WSSrv_sample (net48)";          Bat = "6_Build_WSSrv_sample.bat" }
    @{ Name = "Framework_WS (net48)";          Bat = "7_Build_Framework_WS.bat" }
    @{ Name = "WSClnt_sample (net48)";         Bat = "8_Build_WSClnt_sample.bat" }
    @{ Name = "WebApp_sample (net48)";         Bat = "10_Build_WebApp_sample.bat" }

    # --- netcore100 : ツールとサンプル ---
    @{ Name = "Clean (core サンプル)";         Bat = "1_DeleteDir.bat";                        Clean = $true }
    @{ Name = "Clean files (core サンプル)";   Bat = "1_DeleteFile.bat";                       Clean = $true }
    @{ Name = "Framework_ToolCore";            Bat = "4_Build_Framework_ToolCore.bat" }
    @{ Name = "2CSCore_sample";                Bat = "5_Build_2CSCore_sample.bat" }
    @{ Name = "BatCore_sample";                Bat = "5_Build_BatCore_sample.bat" }
    @{ Name = "CLICore_sample";                Bat = "5_Build_CLICore_sample.bat" }
    @{ Name = "WSSrvCore_sample";              Bat = "6_Build_WSSrvCore_sample.bat" }
    @{ Name = "Framework_WSCore";              Bat = "7_Build_Framework_WSCore.bat" }
    @{ Name = "WSClntCore_sample";             Bat = "8_Build_WSClntCore_sample.bat" }
    @{ Name = "WebAppCore_sample";             Bat = "10_Build_WebAppCore_sample.bat" }
)

# ------------------------------------------------------------------
# 出力の解析
# ------------------------------------------------------------------
# MSBuild のエラー・警告行は「: error CS1002:」のような形式で、
# コード部分はロケールによらないため、これを抽出する。
# （"ビルドに成功しました" 等のサマリ文言は日本語環境で変わるため使わない）
#
# ※ コードを伴わない「: error :」形式もある。
# 　 例: NuGet の restore 失敗
# 　 Microsoft.NuGet.targets(198,5): error : Your project does not reference ...
# 　 このためコード部分は省略可能として扱う。
function Get-Diagnostics([string[]]$lines)
{
    $errors   = New-Object System.Collections.Generic.List[string]
    $warnings = New-Object System.Collections.Generic.List[string]

    foreach ($line in $lines)
    {
        if ($line -match ':\s*error(\s+[A-Za-z]+\d+)?\s*:')
        {
            $errors.Add($line.Trim())
        }
        elseif ($line -match ':\s*warning(\s+[A-Za-z]+\d+)?\s*:')
        {
            $warnings.Add($line.Trim())
        }
        elseif ($line -match '^\s*\[ERROR\]')
        {
            # z_Common.bat が MSBuild 未検出時に出力する独自のエラー
            $errors.Add($line.Trim())
        }
    }

    # 同一の指摘が複数プロジェクトから重複して出るため、一意化する。
    return [pscustomobject]@{
        Errors   = @($errors   | Select-Object -Unique)
        Warnings = @($warnings | Select-Object -Unique)
    }
}

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
Push-Location $csRoot
$results = @()
$allErrors = New-Object System.Collections.Generic.List[string]
$total = [Diagnostics.Stopwatch]::StartNew()

foreach ($s in $steps)
{
    if ($Only -and ($s.Name -notlike "*$Only*") -and ($s.Bat -notlike "*$Only*"))
    {
        continue
    }
    if ($SkipClean -and $s.Clean)
    {
        continue
    }

    $bat = Join-Path $csRoot $s.Bat
    if (-not (Test-Path $bat))
    {
        Write-Host ("  [{0}] バッチが見つかりません : {1}" -f $s.Name, $s.Bat) -ForegroundColor Red
        $results += [pscustomobject]@{ ステップ = $s.Name; 結果 = "バッチ無し"; エラー = "-"; 警告 = "-"; 秒 = "-" }
        continue
    }

    Write-Host ("=== {0} ===" -f $s.Name) -ForegroundColor Cyan

    $safe = ($s.Name -replace '[^A-Za-z0-9]', '_')
    $log  = Join-Path $OutputDir "$safe.log"
    $sw   = [Diagnostics.Stopwatch]::StartNew()

    # 各バッチは末尾に pause を持つため、stdin を与えて実行する
    # （0_ExecAllBat.bat の "echo | call ..." と同じ方式）。
    cmd /c "echo. | call `"$bat`"" *>&1 | Out-File $log -Encoding UTF8
    $sw.Stop()

    $diag = Get-Diagnostics (Get-Content $log -EA SilentlyContinue)

    foreach ($e in $diag.Errors) { $allErrors.Add(("[{0}] {1}" -f $s.Name, $e)) }

    $verdict = if ($diag.Errors.Count -eq 0) { "OK" } else { "NG" }
    $color   = if ($verdict -eq "OK") { "Green" } else { "Red" }
    Write-Host ("  {0}  エラー {1} / 警告 {2}  ({3:N1} 秒)" -f `
        $verdict, $diag.Errors.Count, $diag.Warnings.Count, $sw.Elapsed.TotalSeconds) -ForegroundColor $color

    $results += [pscustomobject]@{
        ステップ = $s.Name
        結果     = $verdict
        エラー   = $diag.Errors.Count
        警告     = $diag.Warnings.Count
        秒       = [Math]::Round($sw.Elapsed.TotalSeconds, 1)
    }
}

$total.Stop()
Pop-Location

# ------------------------------------------------------------------
# サマリ
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ サマリ ================"
$results | Format-Table -AutoSize | Out-String | Write-Host
Write-Host ("  所要時間 : {0:N1} 分" -f $total.Elapsed.TotalMinutes)
Write-Host ("  ログ     : {0}" -f $OutputDir)

if ($allErrors.Count -gt 0)
{
    Write-Host ""
    Write-Host "================ エラー一覧 ================" -ForegroundColor Red
    $allErrors | Select-Object -First 30 | ForEach-Object { Write-Host ("  " + $_) }
    if ($allErrors.Count -gt 30)
    {
        Write-Host ("  ... 他 {0} 件（詳細はログを参照）" -f ($allErrors.Count - 30))
    }
}

$ng = @($results | Where-Object { $_.結果 -ne "OK" })
Write-Host ""
if ($ng.Count -eq 0)
{
    Write-Host "  全ステップ OK" -ForegroundColor Green
    exit 0
}
else
{
    Write-Host ("  {0} ステップが NG" -f $ng.Count) -ForegroundColor Red
    exit 1
}
