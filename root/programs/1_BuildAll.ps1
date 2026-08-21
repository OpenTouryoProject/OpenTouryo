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

    ＜C# 版と VB 版＞
      本体は言語に依存しない。出力の解析（Get-Diagnostics）が見る MSBuild の
      エラー行は「: error CS1002:」「: error BC30451:」のようにコード部の綴りが
      違うだけで、書式は同じだからである（#542）。
      このため、違うのは「どのバッチを、どのフォルダで呼ぶか」だけになる。

.PARAMETER Lang
    対象の言語。CS（既定）/ VB / Both。

    ※ VB 側は C# 側の成果物に依存する（VB\1_GetLibrariesFromCS.bat が
    　 CS\Frameworks\Infrastructure\Build_net48 を取りに行く）。このため
    　 VB のステップ表は、先頭に C# 側の 2_Build_NuGet_net48.bat を含む。
    　 VB\0_ExecAllBat.bat が cd "..\CS" してから呼んでいるのと同じ理由。

.PARAMETER Only
    ステップ名の部分一致で対象を絞る（例: -Only "net48"）。動作確認用。

.PARAMETER List
    -Only に指定できるステップ名を一覧表示して終わる。**ここが一次情報。**
.PARAMETER SkipClean
    クリーン処理（1_DeleteDir / 1_DeleteFile）を省略する。
    ※ リリース判定では省略しないこと。前回のビルド成果物が残っていると、
    　 ビルドが通ったように見えることがある。

.PARAMETER OutputDir
    各ステップの出力ログの保存先。既定は %TEMP%\OpenTouryoBuildLogs。

.PARAMETER IgnoreErrors
    「既知のエラー」として合否判定から除外する正規表現。複数指定できる。
    除外したものは黙って消さず、件数と内容をサマリに別枠で出す。

    環境に依存して必ず出るエラーを、終了コードを汚さずに扱うための引数。
    例: ClickOnce の署名エラー（MSB3482）は、csproj が証明書の拇印を
    　　直接指定しているため、その証明書が無い環境では必ず失敗する。

.EXAMPLE
    .\1_BuildAll.ps1

.EXAMPLE
    .\1_BuildAll.ps1 -Only "Framework_Tool" -SkipClean

.EXAMPLE
    .\1_BuildAll.ps1 -IgnoreErrors "MSB3482"

.EXAMPLE
    .\1_BuildAll.ps1 -Lang VB

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
     2026/08/05  玄人 幸道         IgnoreErrors を追加（CI での既知エラーの除外）
     2026/08/13  玄人 幸道         Lang を追加（VB 版のビルドに対応）
#>
[CmdletBinding()]
param(
    [ValidateSet("CS", "VB", "Both")]
    [string]$Lang = "CS",
    [string]$Only,
    [switch]$List,
    [switch]$SkipClean,
    [switch]$WarnDetail,
    [string]$OutputDir = (Join-Path $env:TEMP "OpenTouryoBuildLogs"),
    [string[]]$IgnoreErrors = @()
)

# 本スクリプトは root\programs に置き、その配下の CS / VB を対象とする。
# ビルド バッチはそれぞれの直下にあるため、ステップごとにそこへ移動して呼ぶ。
New-Item -ItemType Directory -Force $OutputDir | Out-Null

# サマリの整形。Format-Table は 5.1 で全角の桁を数えないため、自前で揃える。
. (Join-Path $PSScriptRoot "SummaryTable.ps1")

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
# ビルド ステップの定義（各言語の 0_ExecAllBat.bat と同じ順序・同じ内容）
# ------------------------------------------------------------------
# Name       : 表示名（ログのファイル名にもなるため、言語をまたいで重複させない）
# Bat        : 呼び出すバッチ
# Dir        : バッチのあるフォルダ（root\programs からの相対）。
#              省略時は対象言語のフォルダ。VB のステップだけ、先頭で "CS" を明示する。
# Clean      : $true のものは -SkipClean で省略される
# SkipIfDone : 同じ実行内で同じバッチが既に走っていれば飛ばす（-Lang Both 用）
$stepsCS = @(
    # --- net48 : 基盤 ---
    @{ Name = "Clean (net48 基盤)";            Bat = "1_DeleteDir.bat";                        Clean = $true; SkipIfDone = $true }
    @{ Name = "Clean files (net48 基盤)";      Bat = "1_DeleteFile.bat";                       Clean = $true; SkipIfDone = $true }
    @{ Name = "NuGet (net48)";                 Bat = "2_Build_NuGet_net48.bat" }
    @{ Name = "Business (net48)";              Bat = "3_Build_Business_net48.bat" }
    @{ Name = "Business.RichClient (net48)";   Bat = "3_Build_BusinessRichClient_net48.bat" }

    # --- netcore100 : 基盤 ---
    @{ Name = "Clean (core 基盤)";             Bat = "1_DeleteDir.bat";                        Clean = $true; SkipIfDone = $true }
    @{ Name = "Clean files (core 基盤)";       Bat = "1_DeleteFile.bat";                       Clean = $true; SkipIfDone = $true }
    @{ Name = "NuGet (netcore100)";            Bat = "2_Build_NuGet_netcore100.bat" }
    @{ Name = "Business (netcore100)";         Bat = "3_Build_Business_netcore100.bat" }
    @{ Name = "Business.RichClient (core)";    Bat = "3_Build_BusinessRichClient_netcore100.bat" }

    # --- 参照用アセンブリのコピー ---
    @{ Name = "CopyAssemblies";                Bat = "4_Build_CopyAssemblies.bat" }

    # --- net48 : ツールとサンプル ---
    @{ Name = "Clean (net48 サンプル)";        Bat = "1_DeleteDir.bat";                        Clean = $true; SkipIfDone = $true }
    @{ Name = "Clean files (net48 サンプル)";  Bat = "1_DeleteFile.bat";                       Clean = $true; SkipIfDone = $true }
    @{ Name = "Framework_Tool (net48)";        Bat = "4_Build_Framework_Tool.bat" }
    @{ Name = "2CS_sample (net48)";            Bat = "5_Build_2CS_sample.bat" }
    @{ Name = "Bat_sample (net48)";            Bat = "5_Build_Bat_sample.bat" }
    @{ Name = "CLI_sample (net48)";            Bat = "5_Build_CLI_sample.bat" }
    @{ Name = "WSSrv_sample (net48)";          Bat = "6_Build_WSSrv_sample.bat" }
    @{ Name = "Framework_WS (net48)";          Bat = "7_Build_Framework_WS.bat" }
    @{ Name = "WSClnt_sample (net48)";         Bat = "8_Build_WSClnt_sample.bat" }
    @{ Name = "WebApp_sample (net48)";         Bat = "10_Build_WebApp_sample.bat" }

    # --- netcore100 : ツールとサンプル ---
    @{ Name = "Clean (core サンプル)";         Bat = "1_DeleteDir.bat";                        Clean = $true; SkipIfDone = $true }
    @{ Name = "Clean files (core サンプル)";   Bat = "1_DeleteFile.bat";                       Clean = $true; SkipIfDone = $true }
    @{ Name = "Framework_ToolCore";            Bat = "4_Build_Framework_ToolCore.bat" }
    @{ Name = "2CSCore_sample";                Bat = "5_Build_2CSCore_sample.bat" }
    @{ Name = "BatCore_sample";                Bat = "5_Build_BatCore_sample.bat" }
    @{ Name = "CLICore_sample";                Bat = "5_Build_CLICore_sample.bat" }
    @{ Name = "WSSrvCore_sample";              Bat = "6_Build_WSSrvCore_sample.bat" }
    @{ Name = "Framework_WSCore";              Bat = "7_Build_Framework_WSCore.bat" }
    @{ Name = "WSClntCore_sample";             Bat = "8_Build_WSClntCore_sample.bat" }
    @{ Name = "WebAppCore_sample";             Bat = "10_Build_WebAppCore_sample.bat" }
)

# VB\0_ExecAllBat.bat と同じ順序・同じ内容。
#
# ＜先頭が CS 側なのは移植ミスではない＞
#   VB\1_GetLibrariesFromCS.bat が
#     xcopy /E /Y "..\CS\Frameworks\Infrastructure\Build_net48" ...
#   と C# 側の成果物を取りに行くため、その出力を作る 2_Build_NuGet_net48.bat を
#   先に通す必要がある。VB\0_ExecAllBat.bat も cd "..\CS" して同じことをしている。
#
# ＜クリーンの位置＞
#   VB\1_DeleteDir.bat は 2 セット目で Build_net48 / Build も消す。
#   GetLibrariesFromCS より前に置くことで、毎回 C# 側から採り直す形になる。
#   この順序は入れ替えない。
#
#   なお 1_GetLibrariesFromCS.bat は -Lang Both でも飛ばせない。ビルドではなく
#   「C# 側の出力を VB 配下へ複写する」処理で、C# を建てても VB 側には何も置かれず、
#   しかも直前のクリーンで VB 側の Build_net48 が消えているためである。
#
# ＜netcore100 が無い＞
#   VB 側は net48 だけで、Core 版のサンプルを持たない（#542）。
$stepsVB = @(
    # --- C# 側の成果物（VB の前提） ---
    #
    # CS\1_DeleteDir.bat の対象は packages/obj/bin/bld/Temp/PrecompiledWeb/
    # MigrationBackup/.vs だけで、Build_net48 を消さない（VB 側とはここが違う）。
    # このため -Lang Both では C# の通しで作った出力がそのまま残っており、
    # 建て直す必要が無い。SkipIfDone で飛ばす。
    #
    # ※ -Lang Both でも -Only で C# 側が絞り落とされることがあるため、
    # 　「Both なら無条件で飛ばす」にはしない。実際に走ったかどうかで判断する。
    @{ Name = "VB : NuGet (net48, CS 側)";     Bat = "2_Build_NuGet_net48.bat";  Dir = "CS"
       SkipIfDone = $true }

    # --- 基盤 ---
    @{ Name = "VB : Clean";                    Bat = "1_DeleteDir.bat";          Clean = $true }
    @{ Name = "VB : Clean files";              Bat = "1_DeleteFile.bat";         Clean = $true }
    @{ Name = "VB : GetLibrariesFromCS";       Bat = "1_GetLibrariesFromCS.bat" }
    @{ Name = "VB : Business (net48)";         Bat = "3_Build_Business_net48.bat" }
    @{ Name = "VB : Business.RichClient";      Bat = "3_Build_BusinessRichClient_net48.bat" }
    @{ Name = "VB : CopyAssemblies";           Bat = "4_Build_CopyAssemblies.bat" }

    # --- サンプル ---
    @{ Name = "VB : Bat_sample";               Bat = "5_Build_Bat_sample.bat" }
    @{ Name = "VB : 2CS_sample";               Bat = "5_Build_2CS_sample.bat" }
    @{ Name = "VB : WSSrv_sample";             Bat = "6_Build_WSSrv_sample.bat" }
    @{ Name = "VB : Framework_WS";             Bat = "7_Build_Framework_WS.bat" }
    @{ Name = "VB : WSClntWin_sample";         Bat = "8_Build_WSClntWin_sample.bat" }
    @{ Name = "VB : WSClntWPF_sample";         Bat = "9_Build_WSClntWPF_sample.bat" }
    @{ Name = "VB : WebApp_sample";            Bat = "10_Build_WebApp_sample.bat" }
)

# 省略されている Dir を、その言語のフォルダで補う。
# Dir を明示しているステップ（VB 表の先頭）は、そのまま残す。
function Add-DefaultDir($steps, [string]$dir)
{
    foreach ($s in $steps)
    {
        if (-not $s.ContainsKey("Dir")) { $s.Dir = $dir }
    }
    return $steps
}

$steps = @()
if ($Lang -ne "VB") { $steps += @(Add-DefaultDir $stepsCS "CS") }
if ($Lang -ne "CS") { $steps += @(Add-DefaultDir $stepsVB "VB") }

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

# -IgnoreErrors に指定された正規表現のいずれかに一致するか。
# 一致したものは合否判定から外すが、握り潰しにならないよう別枠で一覧する。
function Test-KnownError([string]$line)
{
    foreach ($pattern in $IgnoreErrors)
    {
        if ($line -match $pattern)
        {
            return $true
        }
    }
    return $false
}

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
Write-Host ("対象 : {0}" -f $Lang) -ForegroundColor Cyan

$results = @()
$allErrors = New-Object System.Collections.Generic.List[string]
$allKnown  = New-Object System.Collections.Generic.List[string]
$total = [Diagnostics.Stopwatch]::StartNew()

# 実行済みのバッチ（"フォルダ\バッチ名"）。SkipIfDone の判定に使う。
# ※ 一律の重複排除はしない。SkipIfDone を付けたものだけを対象にする。
#
# **Clean は区画ごとに置いてあるが、実際に走るのは最初の 1 回だけ。**（#557）
#   1_DeleteDir.bat は bin を含めてカレント配下から再帰的に消すため、
#   2 回目以降は**直前の区画でビルドした成果物を消してしまう。**
#   通しは元々クリーン ビルドなので、最初の 1 回で足りる。
#   区画の区切りとしての表示は「実行済み」として残す（黙って消さない）。
$executed = @{}


# **-Only に何を指定できるかは、ここが一次情報である。**（#576）
#   文書に書き写すと二重管理になり、対象が増減したときに古くなる。
if ($List)
{
    Write-Host ("=== -Only に指定できる名前（部分一致）{0} ===" -f "（-Lang $Lang）") -ForegroundColor Cyan
    foreach ($x in $steps) { Write-Host ("  " + $x.Name) }
    Write-Host ("  ---- {0} 件 ----" -f $steps.Count)
    exit 0
}

# **-Only が空振りしたら止める。**（#576）
#   1 件も選ばれないまま進むと「全ステップ OK」と表示され、
#   **打ち間違いが緑になる。** 何も建てていないのに成功に見えるのが最も悪い。
if ($Only)
{
    $matched = @($steps | Where-Object { ($_.Name -like "*$Only*") -or ($_.Bat -like "*$Only*") })

    if ($matched.Count -eq 0)
    {
        Write-Host ("  **-Only '$Only' に一致するステップがありません。**") -ForegroundColor Red
        Write-Host ("  -List で一覧を出せます。") -ForegroundColor Yellow
        exit 1
    }

    Write-Host ("  -Only '$Only' : {0} ステップに絞りました" -f $matched.Count) -ForegroundColor Yellow
}

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

    $key = $s.Dir + "\" + $s.Bat

    if ($s.SkipIfDone -and $executed.ContainsKey($key))
    {
        Write-Host ("=== {0} ===" -f $s.Name) -ForegroundColor Cyan
        Write-Host ("  実行済みのため飛ばします : {0}" -f $key)
        $results += [pscustomobject]@{
            ステップ = $s.Name; 結果 = "実行済み"
            エラー = 0; 既知 = 0; 警告 = 0; 秒 = 0
        }
        continue
    }

    # バッチは自分のフォルダから呼ぶ。相対パスで参照を解決しているため、
    # 呼び出し元のカレントが違うと、ソリューションを見つけられない。
    $dir = Join-Path $PSScriptRoot $s.Dir
    $bat = Join-Path $dir $s.Bat

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
    Push-Location $dir
    cmd /c "echo. | call `"$bat`"" *>&1 | Out-File $log -Encoding UTF8
    Pop-Location
    $sw.Stop()

    $executed[$key] = $true

    $diag = Get-Diagnostics (Get-Content $log -EA SilentlyContinue)

    # 既知のエラーを判定対象から外す。件数はサマリに残すため、捨てずに分けて持つ。
    $stepErrors = New-Object System.Collections.Generic.List[string]
    $stepKnown  = New-Object System.Collections.Generic.List[string]

    foreach ($e in $diag.Errors)
    {
        if (Test-KnownError $e)
        {
            $stepKnown.Add($e)
            $allKnown.Add(("[{0}] {1}" -f $s.Name, $e))
        }
        else
        {
            $stepErrors.Add($e)
            $allErrors.Add(("[{0}] {1}" -f $s.Name, $e))
        }
    }

    $verdict = if ($stepErrors.Count -eq 0) { "OK" } else { "NG" }
    $color   = if ($verdict -eq "OK") { "Green" } else { "Red" }
    $knownNote = if ($stepKnown.Count -gt 0) { " / 既知 {0}" -f $stepKnown.Count } else { "" }
    Write-Host ("  {0}  エラー {1} / 警告 {2}{3}  ({4:N1} 秒)" -f `
        $verdict, $stepErrors.Count, $diag.Warnings.Count, $knownNote, $sw.Elapsed.TotalSeconds) -ForegroundColor $color

    $results += [pscustomobject]@{
        ステップ = $s.Name
        結果     = $verdict
        エラー   = $stepErrors.Count
        既知     = $stepKnown.Count
        警告     = $diag.Warnings.Count
        警告詳細 = $diag.Warnings
        秒       = [Math]::Round($sw.Elapsed.TotalSeconds, 1)
    }
}

$total.Stop()

# ------------------------------------------------------------------
# サマリ
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ サマリ ================"
Write-Host ""
# **警告詳細は列にしない。** 表が壊れるので、集計にだけ使う。
Write-SummaryTable ($results | Select-Object * -ExcludeProperty 警告詳細)

# --- 警告の内訳（#571）---
#
# **件数だけでは何を直せばよいか分からない。** 種類ごとにまとめる。
# 既定では出さない。毎回出ると本題（エラー）が埋もれるため。
if ($WarnDetail)
{
    $withWarn = @($results | Where-Object { $_.警告詳細 -and $_.警告詳細.Count -gt 0 })

    if ($withWarn.Count -eq 0)
    {
        Write-Host ""
        Write-Host "  警告はありません。"
    }
    else
    {
        Write-Host ""
        Write-Host "================ 警告の内訳 ================"

        foreach ($r in ($withWarn | Sort-Object { -$_.警告詳細.Count }))
        {
            Write-Host ""
            Write-Host ("  {0}（{1} 件）" -f $r.ステップ, $r.警告詳細.Count)

            # 「: warning XXnnnn :」から種類を取り出す。取れないものは (種類不明) にまとめる。
            $byCode = $r.警告詳細 | ForEach-Object {
                $m = [regex]::Match($_, ':\s*warning\s+([A-Za-z]+\d+)\s*:')
                if ($m.Success) { $m.Groups[1].Value } else { "(種類不明)" }
            } | Group-Object | Sort-Object Count -Descending

            foreach ($g in $byCode)
            {
                # 代表を 1 つ出す。**同じ種類でも中身が違うことがある**ので、目印になる。
                $sample = @($r.警告詳細 | Where-Object { $_ -match [regex]::Escape($g.Name) })[0]
                if ($null -eq $sample) { $sample = "" }
                $sample = ($sample -replace '\s+', ' ')
                if ($sample.Length -gt 96) { $sample = $sample.Substring(0, 96) + " …" }

                Write-Host ("      {0,4}  {1,-12} {2}" -f $g.Count, $g.Name, $sample)
            }
        }

        Write-Host ""
        Write-Host "  **同じ種類は、たいてい 1 か所の対処でまとめて消える。**"
        Write-Host "  MSB3277 は版の混在（CompareRedirect.ps1 / ComparePackage.ps1 も参照）。"
    }
}
Write-Host ""
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

# 除外したものは必ず表示する。黙って消すと、-IgnoreErrors が広すぎたときに気付けない。
if ($allKnown.Count -gt 0)
{
    Write-Host ""
    Write-Host "======== 既知として除外したエラー ========" -ForegroundColor Yellow
    Write-Host ("  除外条件 : {0}" -f ($IgnoreErrors -join " , "))
    $allKnown | Select-Object -First 30 | ForEach-Object { Write-Host ("  " + $_) }
    if ($allKnown.Count -gt 30)
    {
        Write-Host ("  ... 他 {0} 件（詳細はログを参照）" -f ($allKnown.Count - 30))
    }
}

# 「実行済み」は飛ばした印であって失敗ではないので、NG に数えない。
$ng = @($results | Where-Object { $_.結果 -ne "OK" -and $_.結果 -ne "実行済み" })
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
