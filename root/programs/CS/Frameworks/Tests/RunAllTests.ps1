<#
.SYNOPSIS
    単体テストを実行し、同梱の期待結果ファイルと比較して合否を一覧する。

.DESCRIPTION
    リリース時の「単体テスト スクリプトの実行結果を diff で目視」を機械化したもの。
    テストをビルド・実行し、CompareResult.ps1 で判定して結果を一覧表示する。

    ＜前提＞
      ・Frameworks をビルド済み（Build_net48 / Build_netcore100 が存在すること）
      ・SQL Server の Northwind に接続できること（TestBatch が使用）

    ＜証明書＞
      EncAndDecUtilCUI は *.cer / *.pfx が無いとビルドが MSB3030 で失敗する。
      これらは Git 管理外の作業用コピーであり、正本はリポジトリ内の
      root/files/resource/X509/ にある。本スクリプトが不足を検知して自動配置する。

.PARAMETER OutputDir
    実行結果の出力先。既定は %TEMP%\OpenTouryoTestResults。

.PARAMETER SkipBuild
    ビルドを省略し、既存のバイナリで実行する。

.EXAMPLE
    .\RunAllTests.ps1

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
$testsRoot = $PSScriptRoot
New-Item -ItemType Directory -Force $OutputDir | Out-Null

# MSBuild（net48 のビルドに使用）
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe |
    Select-Object -First 1

# ------------------------------------------------------------------
# テスト証明書の配置
# ------------------------------------------------------------------
# EncAndDecUtilCUI の csproj は *.cer / *.pfx を CopyToOutputDirectory しているため、
# これらが無いとビルドが MSB3030 で失敗する。
# 実体は Git 管理外の作業用コピーで、正本はリポジトリ内の root/files/resource/X509/。
# ※ copy_cert.bat は C:\root\files\... を参照するが、そちらは Samples の実行時要件であり
#    単体テストのビルドには不要なため、リポジトリ内から直接コピーする。
function Copy-TestCertificates
{
    $destDir = Join-Path $testsRoot "EncAndDecUtilCUI"
    # Tests\ から見たリポジトリ内の正本
    $srcDir = Join-Path $testsRoot "..\..\..\..\files\resource\X509"

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

Write-Host "=== 事前準備 ===" -ForegroundColor Cyan
Copy-TestCertificates

# ------------------------------------------------------------------
# テストの定義
# ------------------------------------------------------------------
# Expected        : 期待結果ファイル（Tests からの相対パス）
# Project         : ビルド対象
# Exe             : 実行ファイル（ビルド出力からの検索名）
# ExeDir          : 実行ファイルの探索起点
# Args            : コマンドライン引数
# SkipLog4net     : log4net の内部トレースを比較対象から外すか
$tests = @(
    @{
        Name = "TestCode (net48)"
        Expected = "TestCode\Result48.txt"
        Project = "TestCode\TestCodeFx48.sln";  Net48 = $true
        ExeDir = "TestCode\net48\bin\Debug";    Exe = "TestCodeFx.exe"
        Args = @();  SkipLog4net = $false
    }
    @{
        Name = "TestCode (net10.0)"
        Expected = "TestCode\ResultCore100.txt"
        Project = "TestCode\core100\TestCodeCore.csproj"; Net48 = $false
        ExeDir = "TestCode\core100\bin\Debug";  Exe = "TestCodeCore.exe"
        Args = @();  SkipLog4net = $false
    }
    @{
        Name = "SimpleBatch (net48)"
        Expected = "TestBatch\ResultSimpleBatch48.txt"
        Project = "TestBatch\SimpleBatch\SimpleBatch.csproj"; Net48 = $true
        ExeDir = "TestBatch\SimpleBatch\bin\Debug"; Exe = "SimpleBatch.exe"
        Args = @("/DAP","SQL","/MODE1","individual","/MODE2","static","/EXROLLBACK","-")
        SkipLog4net = $true
    }
    @{
        Name = "SimpleBatch (net10.0)"
        Expected = "TestBatch\ResultSimpleBatchCore100.txt"
        Project = "TestBatch\SimpleBatchCore\SimpleBatchCore.csproj"; Net48 = $false
        ExeDir = "TestBatch\SimpleBatchCore\bin\Debug"; Exe = "SimpleBatchCore.exe"
        Args = @("/DAP","SQL","/MODE1","individual","/MODE2","static","/EXROLLBACK","-")
        SkipLog4net = $true
    }
    @{
        Name = "EncAndDecUtilCUI (net48)"
        Expected = "EncAndDecUtilCUI\Result48.txt"
        Project = "EncAndDecUtilCUI\net48\EncAndDecUtilCUIFx.csproj"; Net48 = $true
        ExeDir = "EncAndDecUtilCUI\net48\bin\Debug"; Exe = "EncAndDecUtilCUIFx.exe"
        Args = @();  SkipLog4net = $false
    }
    @{
        Name = "EncAndDecUtilCUI (net10.0)"
        Expected = "EncAndDecUtilCUI\ResultCore100.txt"
        Project = "EncAndDecUtilCUI\core100\EncAndDecUtilCUICore.csproj"; Net48 = $false
        ExeDir = "EncAndDecUtilCUI\core100\bin\Debug"; Exe = "EncAndDecUtilCUICore.exe"
        Args = @();  SkipLog4net = $false
    }
)

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
$results = @()

foreach ($t in $tests)
{
    Write-Host ""
    Write-Host ("=== {0} ===" -f $t.Name) -ForegroundColor Cyan

    $safeName = ($t.Name -replace '[^A-Za-z0-9]', '_')
    $actual = Join-Path $OutputDir "$safeName.txt"

    # ビルド
    if (-not $SkipBuild)
    {
        $proj = Join-Path $testsRoot $t.Project
        Write-Host "  ビルド中 ..."
        if ($t.Net48)
        {
            & $msbuild $proj /t:Restore,Build /p:Configuration=Debug /v:quiet /nologo | Out-Null
        }
        else
        {
            & dotnet build $proj -c Debug --nologo -v q | Out-Null
        }

        if ($LASTEXITCODE -ne 0)
        {
            Write-Host "  ビルド失敗" -ForegroundColor Red
            $results += [pscustomobject]@{ テスト = $t.Name; 結果 = "ビルド失敗"; 差分 = "-" }
            continue
        }
    }

    # 実行
    $exe = Get-ChildItem (Join-Path $testsRoot $t.ExeDir) -Recurse -Filter $t.Exe -EA SilentlyContinue |
           Select-Object -First 1
    if (-not $exe)
    {
        Write-Host "  実行ファイルが見つかりません" -ForegroundColor Red
        $results += [pscustomobject]@{ テスト = $t.Name; 結果 = "実行ファイル無し"; 差分 = "-" }
        continue
    }

    Write-Host "  実行中 ..."
    Push-Location $exe.DirectoryName
    & $exe.FullName @($t.Args) 2>&1 | Out-File $actual -Encoding UTF8
    Pop-Location

    # 比較
    $cmp = Join-Path $testsRoot "CompareResult.ps1"
    $expected = Join-Path $testsRoot $t.Expected
    # CompareResult.ps1 は画面表示に Write-Host を使うため、
    # 件数は -PassThru のオブジェクトで受け取る。
    if ($t.SkipLog4net)
    {
        $cmpResult = & $cmp -Expected $expected -Actual $actual -SkipLog4netTrace -PassThru
    }
    else
    {
        $cmpResult = & $cmp -Expected $expected -Actual $actual -PassThru
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
Write-Host ("  出力先 : {0}" -f $OutputDir)

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
