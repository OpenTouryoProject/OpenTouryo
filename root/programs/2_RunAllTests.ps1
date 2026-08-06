<#
.SYNOPSIS
    単体テストのビルド バッチを実行し、再生成された結果ファイルを HEAD と比較して合否を一覧する。

.DESCRIPTION
    リリース時の「単体テスト スクリプトの実行結果を diff で目視」を機械化したもの（#513 段階 1）。

    ＜既存の運用＞
      y_Build_TestCode*.bat は「ビルド → テスト実行 → 結果を Result*.txt へ出力」まで行う。
      この Result*.txt は Git 管理下にあり、バッチを実行すると上書きされる。
      すなわち従来の手順は次のとおりだった。

        1. バッチを実行して Result*.txt を再生成する
        2. git diff で「前回のリリース時の結果」との差分を目視する

    ＜本スクリプト＞
      同じ構造のまま、2 の目視を機械比較に置き換える。

        期待値 : HEAD にコミットされている Result*.txt（git show で取得）
        実測値 : バッチが再生成したワーキング ツリーの Result*.txt

      比較は CompareResult.ps1 が行い、実行ごとに変わる値（日時・処理時間・
      スレッド ID・パス・署名値等）を正規化してから差分を取る。

    ＜ビルドをバッチに委ねている理由＞
      csproj / sln を直接 msbuild すると、nuget.exe restore が行うネイティブ DLL の
      配置（Microsoft.Data.SqlClient.SNI 等）が漏れ、実行時に DllNotFoundException になる。
      「何をどうビルドするか」の正はバッチ側に置く。

    ＜前提＞
      ・Frameworks をビルド済み（Build_net48 / Build_netcore100 が存在すること）
      ・SQL Server の Northwind に接続できること（TestBatch が使用）

    ＜副作用＞
      ワーキング ツリーの Result*.txt が書き換わる。これは従来のバッチ運用と同じで、
      内容の検収（コミットするか戻すか）は人が git diff で判断する。

.PARAMETER OutputDir
    HEAD 版の期待値とビルド ログの出力先。既定は %TEMP%\OpenTouryoTestResults。

.PARAMETER SkipBuild
    バッチの実行を省略し、ワーキング ツリーにある既存の Result*.txt を比較する。

.EXAMPLE
    .\2_RunAllTests.ps1

.EXAMPLE
    .\2_RunAllTests.ps1 -SkipBuild

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
#>
[CmdletBinding()]
param(
    [string]$OutputDir = (Join-Path $env:TEMP "OpenTouryoTestResults"),
    [switch]$SkipBuild
)

$ErrorActionPreference = "Continue"

# ------------------------------------------------------------------
# カレント ディレクトリからの実行を許可する
# ------------------------------------------------------------------
# PowerShell は子プロセスに NoDefaultCurrentDirectoryInExePath=1 を渡す。
# 一方 y_Build_TestCode*.bat は、出力フォルダへ cd したうえで
#   SimpleBatch.exe /Dap SQL ...
# のようにパス区切りを含まない名前で実行するため、この変数が効いていると
#   'SimpleBatch.exe' is not recognized as an internal or external command
# となり、結果ファイルが空になる（バッチ自体は成功扱いで進む）。
# バッチを直接ダブル クリックした場合と同じ条件にするため、ここで解除する。
Remove-Item Env:\NoDefaultCurrentDirectoryInExePath -EA SilentlyContinue

# 本スクリプトは root\programs に置き、C# 側（root\programs\CS）を対象とする。
$progRoot  = $PSScriptRoot
# ビルド バッチ（y_Build_TestCode*.bat）が置かれている root\programs\CS
$csRoot    = Join-Path $progRoot "CS"
# 単体テストのプロジェクトと結果ファイルが置かれている場所
$testsRoot = Join-Path $csRoot "Frameworks\Tests"
$repoRoot  = (& git -C $testsRoot rev-parse --show-toplevel)
New-Item -ItemType Directory -Force $OutputDir | Out-Null

# ------------------------------------------------------------------
# コンソールのコード ページを先に UTF-8 にしておく
# ------------------------------------------------------------------
# ビルド バッチが呼ぶ z_Common.bat は先頭で chcp 65001 を実行する。
# コード ページはコンソール全体の設定なので、子プロセスで変えても
# 呼び出し元の画面に影響し、日本語環境の既定 932 からの切り替わりで
# 画面が再描画されて**それまでの表示が消える**。
# 何も表示していない今のうちに切り替えておけば、実行中は変化しない。
# 戻さないのは、戻す操作でも再描画が起きてサマリが消えるため。
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
# テスト証明書の配置
# ------------------------------------------------------------------
# EncAndDecUtilCUI の csproj は *.cer / *.pfx を CopyToOutputDirectory しているため、
# これらが無いとビルドが MSB3030 で失敗する。
# y_Build_TestCode_SecCUI.bat も copy_cert.bat を呼ぶが、そちらは C:\root\files\... を
# 参照するため、リポジトリだけを clone した環境では配置できない。
# ここではリポジトリ内の正本（root/files/resource/X509/）から補う。
function Copy-TestCertificates
{
    $destDir = Join-Path $testsRoot "EncAndDecUtilCUI"
    $srcDir  = Join-Path $testsRoot "..\..\..\..\files\resource\X509"

    if (-not (Test-Path $srcDir))
    {
        Write-Host "  証明書の正本フォルダが見つかりません : $srcDir" -ForegroundColor Yellow
        return
    }

    $names = @("SHA256RSA", "SHA256DSA", "SHA256ECDSA", "SHA384ECDSA", "SHA521ECDSA")
    $copied = 0

    foreach ($n in $names)
    {
        foreach ($ext in @("cer", "pfx"))
        {
            $dest = Join-Path $destDir "$n.$ext"
            if (Test-Path $dest) { continue }

            $src = Join-Path $srcDir "$n.$ext"
            if (Test-Path $src)
            {
                Copy-Item $src $dest -Force
                $copied++
            }
            else
            {
                Write-Host ("  証明書が見つかりません : {0}" -f $src) -ForegroundColor Yellow
            }
        }
    }

    if ($copied -gt 0)
    {
        Write-Host ("  テスト証明書を {0} 件配置しました。" -f $copied)
    }
}

# ------------------------------------------------------------------
# テストの定義
# ------------------------------------------------------------------
# Result      : バッチが再生成する結果ファイル（Tests からの相対パス）
#               ＝ 期待値でもある（HEAD 版と比較する）
# Bat         : ビルドとテスト実行を行うバッチ（root\programs\CS 配下）
# SkipLog4net : log4net の内部トレースを比較対象から外すか
$tests = @(
    @{
        Name = "TestCode (net48)";           Bat = "y_Build_TestCode_Public.bat"
        Result = "TestCode\Result48.txt";            SkipLog4net = $false
    }
    @{
        Name = "TestCode (net10.0)";         Bat = "y_Build_TestCode_Public.bat"
        Result = "TestCode\ResultCore100.txt";       SkipLog4net = $false
    }
    @{
        Name = "TestDataAccess (net48)";     Bat = "y_Build_TestCode_DataAccess.bat"
        Result = "TestDataAccess\Result48.txt";      SkipLog4net = $false
    }
    @{
        Name = "TestDataAccess (net10.0)";   Bat = "y_Build_TestCode_DataAccess.bat"
        Result = "TestDataAccess\ResultCore100.txt"; SkipLog4net = $false
    }
    @{
        Name = "SimpleBatch (net48)";        Bat = "y_Build_TestCode_Batch.bat"
        Result = "TestBatch\ResultSimpleBatch48.txt";     SkipLog4net = $true
    }
    @{
        Name = "SimpleBatch (net10.0)";      Bat = "y_Build_TestCode_Batch.bat"
        Result = "TestBatch\ResultSimpleBatchCore100.txt"; SkipLog4net = $true
    }
    @{
        Name = "EncAndDecUtilCUI (net48)";   Bat = "y_Build_TestCode_SecCUI.bat"
        Result = "EncAndDecUtilCUI\Result48.txt";        SkipLog4net = $false
    }
    @{
        Name = "EncAndDecUtilCUI (net10.0)"; Bat = "y_Build_TestCode_SecCUI.bat"
        Result = "EncAndDecUtilCUI\ResultCore100.txt";   SkipLog4net = $false
    }
)

# ------------------------------------------------------------------
# 期待値（HEAD 版）の取り出し
# ------------------------------------------------------------------
# バッチがワーキング ツリーの Result*.txt を上書きするため、
# 上書きされる前に HEAD の内容を退避しておく。
# ※ git show は参照系のみ。ワーキング ツリーには一切触れない。
Write-Host "=== 事前準備 ===" -ForegroundColor Cyan
Copy-TestCertificates

$expectedOf = @{}

foreach ($t in $tests)
{
    $full = Join-Path $testsRoot $t.Result
    # git が扱えるようリポジトリ相対・スラッシュ区切りにする
    $rel  = ($full.Substring($repoRoot.Length).TrimStart('\','/') -replace '\\','/')
    $safe = ($t.Name -replace '[^A-Za-z0-9]', '_')
    $dst  = Join-Path $OutputDir "expected_$safe.txt"

    # バイト列を保つため cmd のリダイレクトで受ける
    # （PowerShell のパイプを経由すると再エンコードされる）
    cmd /c "git -C `"$repoRoot`" show `"HEAD:$rel`" > `"$dst`" 2>nul"

    if ((Test-Path $dst) -and (Get-Item $dst).Length -gt 0)
    {
        $expectedOf[$t.Name] = $dst
    }
    else
    {
        Write-Host ("  HEAD から取得できません : {0}" -f $rel) -ForegroundColor Yellow
    }
}

Write-Host ("  期待値（HEAD 版）を {0} 件退避しました。" -f $expectedOf.Count)

# ------------------------------------------------------------------
# ビルドとテスト実行
# ------------------------------------------------------------------
# 1 本のバッチが net48 版と .NET (Core) 版の両方をビルド・実行するため、
# 同一バッチは 1 回だけ実行する。
if (-not $SkipBuild)
{
    foreach ($bat in ($tests | ForEach-Object { $_.Bat } | Select-Object -Unique))
    {
        Write-Host ""
        Write-Host ("=== ビルドと実行 : {0} ===" -f $bat) -ForegroundColor Cyan

        $path = Join-Path $csRoot $bat
        if (-not (Test-Path $path))
        {
            Write-Host "  バッチが見つかりません" -ForegroundColor Red
            continue
        }

        $log = Join-Path $OutputDir ("build_" + ($bat -replace '[^A-Za-z0-9]','_') + ".log")
        $sw  = [Diagnostics.Stopwatch]::StartNew()

        Push-Location $csRoot
        # バッチ末尾の pause に対応するため stdin を与える。
        cmd /c "echo. | call `"$path`"" *>&1 | Out-File $log -Encoding UTF8
        Pop-Location

        $sw.Stop()
        Write-Host ("  完了 ({0:N1} 秒)  ログ : {1}" -f $sw.Elapsed.TotalSeconds, $log)
    }
}

# ------------------------------------------------------------------
# 比較
# ------------------------------------------------------------------
$cmp = Join-Path $progRoot "CompareResult.ps1"
$results = @()

foreach ($t in $tests)
{
    Write-Host ""
    Write-Host ("=== {0} ===" -f $t.Name) -ForegroundColor Cyan

    $actual = Join-Path $testsRoot $t.Result

    if (-not $expectedOf.ContainsKey($t.Name))
    {
        Write-Host "  期待値（HEAD 版）が無いため比較できません" -ForegroundColor Red
        $results += [pscustomobject]@{ テスト = $t.Name; 結果 = "期待値無し"; 差分 = "-" }
        continue
    }
    if (-not (Test-Path $actual))
    {
        Write-Host "  結果ファイルが生成されていません" -ForegroundColor Red
        $results += [pscustomobject]@{ テスト = $t.Name; 結果 = "結果無し"; 差分 = "-" }
        continue
    }

    # CompareResult.ps1 は画面表示に Write-Host を使うため、
    # 件数は -PassThru のオブジェクトで受け取る。
    if ($t.SkipLog4net)
    {
        $cmpResult = & $cmp -Expected $expectedOf[$t.Name] -Actual $actual -SkipLog4netTrace -PassThru
    }
    else
    {
        $cmpResult = & $cmp -Expected $expectedOf[$t.Name] -Actual $actual -PassThru
    }

    $results += [pscustomobject]@{
        テスト = $t.Name
        結果   = $cmpResult.Result
        差分   = $cmpResult.DiffCount
    }
}

# ------------------------------------------------------------------
# サマリ
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ サマリ ================"
$results | Format-Table -AutoSize | Out-String | Write-Host
Write-Host ("  期待値・ログ : {0}" -f $OutputDir)
Write-Host  "  実測値       : ワーキング ツリーの Result*.txt（git diff で確認できます）"

$ng = @($results | Where-Object { $_.結果 -ne "OK" })
if ($ng.Count -eq 0)
{
    Write-Host "  全テスト OK" -ForegroundColor Green
    exit 0
}
else
{
    Write-Host ("  {0} 件が NG" -f $ng.Count) -ForegroundColor Yellow
    exit 1
}
