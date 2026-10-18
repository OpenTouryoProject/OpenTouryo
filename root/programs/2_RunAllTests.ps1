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

    ＜NUnit のテスト（#610）＞
      Tests\NUnit のテストは**期待値ファイルを持たない。** テスト自身が合否を出す。
      バッチ（y_Build_TestCode_NUnit.bat）が書き出す *.trx を読み、失敗 0 件なら OK とする。
      trx は Git 管理外（.gitignore）なので、ワーキング ツリーは書き換わらない。
      net10.0 ではカバレッジも収集し、アセンブリごとの行カバレッジを表示する（判定はしない）。

    ＜ビルドをバッチに委ねている理由＞
      csproj / sln を直接 msbuild すると、nuget.exe restore が行うネイティブ DLL の
      配置（Microsoft.Data.SqlClient.SNI 等）が漏れ、実行時に DllNotFoundException になる。
      「何をどうビルドするか」の正はバッチ側に置く。

    ＜前提＞
      ・Frameworks をビルド済み（Build_net48 / Build_netcore100 が存在すること）
      ・SQL Server の Northwind に接続できること
        （TestDataAccess と TestBatch が使用。冒頭で確認する）

      **ASP.NET 状態サービス（aspnet_state）は要らない。**
      System.Web / System.Web.Extensions は csproj の参照だけで、
      HttpContext も sessionState も使っていない。
      CI も、このスクリプトの**後**で aspnet_state を開始している
      （build-windows.yml）。要るのは 3_SmokeTest.ps1 の Web アプリだけ。

    ＜副作用＞
      ワーキング ツリーの Result*.txt が書き換わる。これは従来のバッチ運用と同じで、
      内容の検収（コミットするか戻すか）は人が git diff で判断する。

.PARAMETER OutputDir
    HEAD 版の期待値とビルド ログの出力先。既定は %TEMP%\OpenTouryoTestResults。

.PARAMETER Only
    対象名の部分一致で絞る（例: -Only "TestBatch"）。複数指定できる。
    **Result*.txt を書き換えるのは、絞った対象だけになる。**
.PARAMETER List
    -Only に指定できる対象名を一覧表示して終わる。**ここが一次情報。**
.PARAMETER PassThru
    -List と併せて指定すると、名前・バッチの対応をオブジェクトで返す
    （SelectTargets.ps1 が使う）。
.PARAMETER SkipBuild
    バッチの実行を省略し、ワーキング ツリーにある既存の Result*.txt を比較する。
.PARAMETER CrossDb
    Dam のクロス DB テスト（NUnit の TestPublicDb）を、**DBMS ごとに「起動 → テスト → 停止」**で回す（#623）。
    対象は TestPublicDb だけになる。**-LocalServicesDir が必須。**
    **コンテナを起動・停止するのは、これを指定したときだけ。** 既定の動きは変わらない。

    ＜なぜ 1 つずつか＞
      DB のコンテナを同時に複数起動すると、Docker の VM がメモリを使い切り、
      最悪の場合システムごとハングする（LocalServicesOnDocker の CHEATSHEET.md）。
      同時に動かすのは、常に SQL Server（必須）ともう 1 つだけにする。

    ＜流れ＞
      1. SQL Server が動いていなければ起動する（終わったら止める）
      2. 通常どおりバッチでビルドし、TestPublicDb を回す（SQL Server だけの回）
      3. Oracle → MySQL → PostgreSQL の順に、起動 → TestPublicDb だけを --no-build で回す → 停止
         結果は NUnit\TestResults\CrossDb\<サービス名> に分けて残す（カバレッジも回ごとに残る）

    **始める前から動いていた DBMS は、起動も停止もしない。**
    **起動したのに、その DBMS のテストが 1 件も実行されなければ NG にする**（スキップのまま OK にしない）。
.PARAMETER LocalServicesDir
    LocalServicesOnDocker のフォルダ（-CrossDb で使う）。
    **docker コマンドが使えれば Rancher Desktop 版（*.ps1）、使えなければ WSL2 版（*_wsl2.ps1）を使う。**

.EXAMPLE
    .\2_RunAllTests.ps1

.EXAMPLE
    .\2_RunAllTests.ps1 -SkipBuild

.EXAMPLE
    .\2_RunAllTests.ps1 -Only "TestBatch"

.EXAMPLE
    .\2_RunAllTests.ps1 -CrossDb -LocalServicesDir D:\git\LocalServicesOnDocker

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
     2026/08/17  玄人 幸道         テストごとに Base64 の正規化を選べるようにした（#562）
     2026/10/15  玄人 幸道         Tests を Golden / NUnit / Else に整理した（#610）
     2026/10/15  玄人 幸道         NUnit の結果（trx）を判定する対象を加えた（#610）
     2026/10/15  玄人 幸道         -Only の複数指定と -List -PassThru を追加（#610）
     2026/10/15  玄人 幸道         NUnit の TestPublicNLog を加え、結果をプロジェクトごとに分けた（#610）
     2026/10/15  玄人 幸道         NUnit のスキップを件数と名前で出すようにした（#610）
     2026/10/15  玄人 幸道         NUnit の TestPublicDb（Dam のクロス DB）を加えた（#610）
     2026/10/15  玄人 幸道         -List -PassThru に NeedDb を加えた（#610）
     2026/10/18  玄人 幸道         -CrossDb / -LocalServicesDir を追加した（#623）
     2026/10/18  玄人 幸道         NUnit のカバレッジを、別のプロジェクトの結果から表示していたのを修正（#623 の後）
#>
[CmdletBinding()]
param(
    [string]$OutputDir = (Join-Path $env:TEMP "OpenTouryoTestResults"),
    [string[]]$Only,
    [switch]$List,
    [switch]$PassThru,
    [switch]$SkipBuild,
    [switch]$CrossDb,
    [string]$LocalServicesDir
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

# サマリの整形。Format-Table は 5.1 で全角の桁を数えないため、自前で揃える。
. (Join-Path $PSScriptRoot "SummaryTable.ps1")
. (Join-Path $PSScriptRoot "Prerequisite.ps1")

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
    $destDir = Join-Path $testsRoot "Golden\EncAndDecUtilCUI"
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
# NormBase64  : Base64 らしき長い値を伏せるか（#562）
#               **伏せると、その範囲の変化は差分に出ない。**
#               識別子（IsHankaku/IsZenkaku など）にも当たるため、
#               **実行のたびに値が変わるテストだけ $true にする。**
# NeedDb      : SQL Server の Northwind を使うか（#588）
#               **単体テストも DB を使う。** 冒頭でまとめて確認する。
# Kind        : "NUnit" なら、Result は *.trx で、HEAD とは比較しない（#610）
#               **テスト自身が合否を出すため、期待値ファイルを持たない。**
# Coverage    : カバレッジ（coverage.cobertura.xml）を表示するか（#610）
$tests = @(
    @{
        Name = "TestCode (net48)";           Bat = "y_Build_TestCode_Public.bat"
        Result = "Golden\TestCode\Result48.txt";            SkipLog4net = $false; NormBase64 = $false
    }
    @{
        Name = "TestCode (net10.0)";         Bat = "y_Build_TestCode_Public.bat"
        Result = "Golden\TestCode\ResultCore100.txt";       SkipLog4net = $false; NormBase64 = $false
    }
    @{
        Name = "TestDataAccess (net48)";     Bat = "y_Build_TestCode_DataAccess.bat"
        Result = "Golden\TestDataAccess\Result48.txt";      SkipLog4net = $false; NormBase64 = $false
        NeedDb = $true
    }
    @{
        Name = "TestDataAccess (net10.0)";   Bat = "y_Build_TestCode_DataAccess.bat"
        Result = "Golden\TestDataAccess\ResultCore100.txt"; SkipLog4net = $false; NormBase64 = $false
        NeedDb = $true
    }
    @{
        Name = "TestBatch (net48)";          Bat = "y_Build_TestCode_Batch.bat"
        Result = "Golden\TestBatch\ResultSimpleBatch48.txt";     SkipLog4net = $true; NormBase64 = $false
        NeedDb = $true
    }
    @{
        Name = "TestBatch (net10.0)";        Bat = "y_Build_TestCode_Batch.bat"
        Result = "Golden\TestBatch\ResultSimpleBatchCore100.txt"; SkipLog4net = $true; NormBase64 = $false
        NeedDb = $true
    }
    @{
        Name = "EncAndDecUtilCUI (net48)";   Bat = "y_Build_TestCode_SecCUI.bat"
        Result = "Golden\EncAndDecUtilCUI\Result48.txt";        SkipLog4net = $false; NormBase64 = $true
    }
    @{
        Name = "EncAndDecUtilCUI (net10.0)"; Bat = "y_Build_TestCode_SecCUI.bat"
        Result = "Golden\EncAndDecUtilCUI\ResultCore100.txt";   SkipLog4net = $false; NormBase64 = $true
    }
    @{
        Name = "NUnit TestPublic (net48)";   Bat = "y_Build_TestCode_NUnit.bat"
        Result = "NUnit\TestResults\TestPublic\TestPublic_net48.trx";   Kind = "NUnit"
    }
    @{
        Name = "NUnit TestPublic (net10.0)"; Bat = "y_Build_TestCode_NUnit.bat"
        Result = "NUnit\TestResults\TestPublic\TestPublic_net10.0.trx"; Kind = "NUnit"; Coverage = $true
    }
    # LogIF を NLog に切り替えた状態（別プロセスで動かすため別プロジェクト。#610）
    @{
        Name = "NUnit TestPublicNLog (net48)";   Bat = "y_Build_TestCode_NUnit.bat"
        Result = "NUnit\TestResults\TestPublicNLog\TestPublicNLog_net48.trx";   Kind = "NUnit"
    }
    @{
        Name = "NUnit TestPublicNLog (net10.0)"; Bat = "y_Build_TestCode_NUnit.bat"
        Result = "NUnit\TestResults\TestPublicNLog\TestPublicNLog_net10.0.trx"; Kind = "NUnit"; Coverage = $true
    }
    # Dam のクロス DB（#610）。SQL Server は必須、他の DBMS は接続できなければスキップ。
    @{
        Name = "NUnit TestPublicDb (net48)";   Bat = "y_Build_TestCode_NUnit.bat"
        Result = "NUnit\TestResults\TestPublicDb\TestPublicDb_net48.trx";   Kind = "NUnit"
        NeedDb = $true
    }
    @{
        Name = "NUnit TestPublicDb (net10.0)"; Bat = "y_Build_TestCode_NUnit.bat"
        Result = "NUnit\TestResults\TestPublicDb\TestPublicDb_net10.0.trx"; Kind = "NUnit"; Coverage = $true
        NeedDb = $true
    }
)

# ------------------------------------------------------------------
# 対象の絞り込み（#576）
# ------------------------------------------------------------------
#   **依存関係の外まで回す必要はない。**
#   このスクリプトはワーキング ツリーの Result*.txt を書き換えるため、
#   不要な対象まで回すと、人が確認・コミットする差分が増える。

# **-Only に何を指定できるかは、ここが一次情報である。**（#576）
#   文書に書き写すと二重管理になり、対象が増減したときに古くなる。
if ($List)
{
    Write-Host ("=== -Only に指定できる名前（部分一致）{0} ===" -f "") -ForegroundColor Cyan
    foreach ($x in $tests) { Write-Host ("  " + $x.Name) }
    Write-Host ("  ---- {0} 件 ----" -f $tests.Count)

    # 名前とバッチの対応を返す（#610）。SelectTargets.ps1 が使う。
    # 単体テストは C# 側にしかないので、Dir は常に CS。
    if ($PassThru)
    {
        # NeedDb は SelectTargets.ps1 -Stage fixing が DB を使う対象を後回しにするのに使う（#610）。
        foreach ($x in $tests) { [pscustomobject]@{ Name = $x.Name; Bat = $x.Bat; Dir = "CS"; NeedDb = [bool]$x.NeedDb } }
    }
    exit 0
}

# **-Only は複数指定できる。どれかに当たれば対象。**（#610）
#   1 つでも空振りしたら止める（打ち間違いが緑になるのを防ぐ。#576）。
if ($Only)
{
    foreach ($o in $Only)
    {
        if (@($tests | Where-Object { $_.Name -like "*$o*" }).Count -eq 0)
        {
            Write-Host ("  **-Only '$o' に一致する対象がありません。**") -ForegroundColor Red
            Write-Host ("  -List で一覧を出せます。") -ForegroundColor Yellow
            exit 1
        }
    }

    $tests = @($tests | Where-Object { $cand = $_; @($Only | Where-Object { $cand.Name -like "*$_*" }).Count -gt 0 })

    Write-Host ("  -Only '{0}' : {1} 件に絞りました" -f ($Only -join "', '"), $tests.Count) -ForegroundColor Yellow
}

# ------------------------------------------------------------------
# クロス DB（#623）
# ------------------------------------------------------------------
#   **DBMS ごとに 1 つずつ起動して回す。** 同時に複数起動すると、
#   Docker の VM がメモリを使い切り、システムごとハングすることがある
#   （LocalServicesOnDocker の CHEATSHEET.md）。
#
# サービス名・TestPublicDb の DBMS の略号（DbProvider.cs）・ポート・回す TFM
#   PostgreSQL は DamPstGrS が .NET 専用のため net10.0 だけ。
$crossDbServices = @(
    @{ Service = "oracle";   Dbms = "ODP"; Port = 1521; Tfms = @("net48", "net10.0") }
    @{ Service = "mysql";    Dbms = "MCN"; Port = 3306; Tfms = @("net48", "net10.0") }
    @{ Service = "postgres"; Dbms = "NPS"; Port = 5432; Tfms = @("net10.0") }
)
$crossDbProject = "TestPublicDb"

if ($CrossDb)
{
    if (-not $LocalServicesDir -or -not (Test-Path (Join-Path $LocalServicesDir "Start-Services.ps1")))
    {
        Write-Host "  **-CrossDb には -LocalServicesDir（LocalServicesOnDocker のフォルダ）が要ります。**" -ForegroundColor Red
        Write-Host ("  指定 : '{0}'（Start-Services.ps1 が見つかりません）" -f $LocalServicesDir) -ForegroundColor Yellow
        exit 1
    }

    # **対象は TestPublicDb だけにする。** 他のテストは DBMS を切り替えても結果が変わらない。
    $tests = @($tests | Where-Object { $_.Name -like "*$crossDbProject*" })
    if ($tests.Count -eq 0)
    {
        Write-Host "  **-CrossDb の対象（TestPublicDb）が、-Only で外れています。**" -ForegroundColor Red
        exit 1
    }
    Write-Host ("  -CrossDb : 対象を {0} の {1} 件に絞りました" -f $crossDbProject, $tests.Count) -ForegroundColor Yellow

    # **docker コマンドが使えれば Rancher Desktop 版、使えなければ WSL2 版。**
    #   2 つは別の Docker を見るので、混ぜると同じサービスが二重に立つ。
    $suffix = "_wsl2"
    if (Get-Command docker -EA SilentlyContinue)
    {
        & docker info *> $null
        if ($LASTEXITCODE -eq 0) { $suffix = "" }
    }
    $svcStart = Join-Path $LocalServicesDir ("Start-Services{0}.ps1" -f $suffix)
    $svcStop  = Join-Path $LocalServicesDir ("Stop-Services{0}.ps1" -f $suffix)
    Write-Host ("  -CrossDb : {0} を使います" -f $(if ($suffix) { "WSL2 版（*_wsl2.ps1）" } else { "Rancher Desktop 版（*.ps1）" })) -ForegroundColor Yellow

    foreach ($s in @($svcStart, $svcStop))
    {
        if (-not (Test-Path $s))
        {
            Write-Host ("  **見つかりません : {0}**" -f $s) -ForegroundColor Red
            exit 1
        }
    }
}

# サービスの起動・停止（#623）
#   **別のプロセスで動かす。** 向こうのスクリプトは exit で終わり、
#   コード ページやエラー設定も変えるので、こちらのスコープに持ち込まない。
#   **サービス名は必ず渡す。** 省略すると全サービスが対象になる。
function Invoke-LocalService([string]$Script, [string]$Service)
{
    $psExe = (Get-Process -Id $PID).Path
    & $psExe -NoProfile -ExecutionPolicy Bypass -File $Script $Service -NoPause *>&1 |
        ForEach-Object { Write-Host ("    " + $_) }
    return ($LASTEXITCODE -eq 0)
}

# ポートが開いているか（始める前から動いている DBMS を見分ける。#623）
#   待ち時間はこちらで断ち切る（Prerequisite.ps1 の Test-SqlServer と同じ理由）。
function Test-LocalPort([int]$Port, [int]$TimeoutMs = 2000)
{
    $c = New-Object System.Net.Sockets.TcpClient
    try
    {
        $task = $c.ConnectAsync("localhost", $Port)
        return ($task.Wait($TimeoutMs) -and $c.Connected)
    }
    catch { return $false }
    finally { $c.Dispose() }
}

# ------------------------------------------------------------------
# 前提の確認（#588）
# ------------------------------------------------------------------
# **単体テストも DB を使う。** TestDataAccess と TestBatch が Northwind に繋ぐ。
#   落ちるまで分からないと、差分を見て初めて気付くことになる。
#
# **接続文字列は TestDataAccess の App.config から読む。**
#   対象が実際に使う設定を見る（3_SmokeTest.ps1 がサンプルの
#   App.config を見るのと同じ理屈）。
#
# **aspnet_state は見ない。** 単体テストは ASP.NET を使わない
#   （上の＜前提＞を参照）。
$needDb = @($tests | Where-Object { $_.NeedDb }).Count -gt 0

$dbConfig = Join-Path $testsRoot "Golden\TestDataAccess\App.config"

# **-CrossDb では、SQL Server（必須）が動いていなければ起動する。**（#623）
#   起動したものだけを、終わったら止める（始める前から動いていれば、そのまま）。
$startedSqlServer = $false
if ($CrossDb -and -not (Test-SqlServer (Get-ConnectionStringFromConfig $dbConfig)).Ok)
{
    Write-Host "=== クロス DB : SQL Server を起動します ===" -ForegroundColor Cyan
    if (-not (Invoke-LocalService $svcStart "sqlserver"))
    {
        Write-Host "  **SQL Server を起動できませんでした。**" -ForegroundColor Red
        exit 1
    }
    $startedSqlServer = $true
}

if ($needDb)
{
    Show-Prerequisites -DbConfig $dbConfig
}

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
    # NUnit は期待値ファイルを持たない（#610）
    if ($t.Kind -eq "NUnit") { continue }

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
# **ビルドが失敗したバッチ名を覚えておく。**（#564）
#   失敗しても前回の実行ファイルが動くため、
#   結果は期待値と一致し「OK」になってしまう。
$buildFailed = @{}

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
            $buildFailed[$bat] = @("バッチが見つかりません : $path")
            continue
        }

        $log = Join-Path $OutputDir ("build_" + ($bat -replace '[^A-Za-z0-9]','_') + ".log")
        $sw  = [Diagnostics.Stopwatch]::StartNew()

        Push-Location $csRoot
        # バッチ末尾の pause に対応するため stdin を与える。
        cmd /c "echo. | call `"$path`"" *>&1 | Out-File $log -Encoding UTF8
        Pop-Location

        $sw.Stop()

        # ------------------------------------------------------------------
        # **ログからビルド エラーを拾う。**（#564）
        # ------------------------------------------------------------------
        #   ＜終了コードを見ない理由＞
        #     バッチは msbuild を 2 回、テストの実行を 2 回、最後に pause を呼ぶ。
        #     $LASTEXITCODE は**最後のものしか返らない**ので、
        #     ビルドの成否を表さない。
        #
        #   ＜1_BuildAll.ps1 と同じ見方にする＞
        #     コード部分はロケールによらないため、これを拾う。
        #     "ビルドに失敗しました" 等のサマリ文言は日本語環境でしか出ない。
        #     コードを伴わない「: error :」形式（NuGet の restore 失敗）もあるため、
        #     コード部分は省略可能として扱う。
        $buildErrors = @(
            Get-Content $log -Encoding UTF8 -EA SilentlyContinue |
            Where-Object { $_ -match ':\s*error(\s+[A-Za-z]+\d+)?\s*:' -or $_ -match '^\s*\[ERROR\]' } |
            ForEach-Object { $_.Trim() } |
            Select-Object -Unique)

        if ($buildErrors.Count -eq 0)
        {
            Write-Host ("  完了 ({0:N1} 秒)  ログ : {1}" -f $sw.Elapsed.TotalSeconds, $log)
        }
        else
        {
            $buildFailed[$bat] = $buildErrors

            Write-Host ("  **ビルド エラー {0} 件** ({1:N1} 秒)  ログ : {2}" -f `
                $buildErrors.Count, $sw.Elapsed.TotalSeconds, $log) -ForegroundColor Red

            foreach ($e in ($buildErrors | Select-Object -First 5))
            {
                Write-Host ("    " + $e) -ForegroundColor Red
            }
            if ($buildErrors.Count -gt 5)
            {
                Write-Host ("    ... 他 {0} 件（ログを参照）" -f ($buildErrors.Count - 5)) -ForegroundColor Red
            }

            Write-Host "  **前回の実行ファイルが動くため、比較は当てになりません。**" -ForegroundColor Red
        }
    }
}

# ------------------------------------------------------------------
# NUnit の結果（trx）の判定（#610）
# ------------------------------------------------------------------
#   trx の ResultSummary を読む。**差分の列には失敗の件数を出す。**
#   バッチは実行前に古い trx を消すので、ここにあるのは今回の結果である。
#
#   -RequireDbms : その DBMS のテストが 1 件以上実行されたことも求める（-CrossDb。#623）。
#                  **起動したのにスキップされたら、接続できていない。** OK にしない。
#                  DBMS は TestFixtureSource の引数で、trx の className に DamTests("ODP") のように出る。
function Get-NUnitResult([string]$Name, [string]$Trx, [bool]$Coverage, [string]$RequireDbms)
{
    if (-not (Test-Path $Trx))
    {
        Write-Host "  結果ファイル（trx）が生成されていません" -ForegroundColor Red
        return [pscustomobject]@{ テスト = $Name; 結果 = "結果無し"; 差分 = "-" }
    }

    # **変数名を $Trx（引数）と分ける。** PowerShell は大文字・小文字を区別しないので、同じ名前だと引数を上書きする
    #   （カバレッジのファイルを、trx の隣ではなく作業フォルダから探していた。#623 の後で修正）。
    [xml]$trxXml = Get-Content $Trx -Encoding UTF8 -Raw
    $c = $trxXml.TestRun.ResultSummary.Counters
    $total  = [int]$c.total
    $failed = [int]$c.failed + [int]$c.error + [int]$c.timeout + [int]$c.aborted

    # **スキップ（Assert.Ignore）は Counters の notExecuted に入らない。**
    #   total と executed の差にだけ現れ、個々の結果では outcome="NotExecuted" になる。
    #   出さないと「全部走った」と読めてしまうので、件数と名前を必ず出す。
    $skipped = [int]$c.total - [int]$c.executed

    Write-Host ("  合計 {0} 件 / 合格 {1} 件 / 失敗 {2} 件 / 実行せず {3} 件" -f `
        $total, $c.passed, $failed, $skipped)

    foreach ($r in @($trxXml.TestRun.Results.UnitTestResult | Where-Object { $_.outcome -ne "Passed" -and $_.outcome -ne "NotExecuted" }))
    {
        Write-Host ("    [{0}] {1}" -f $r.outcome, $r.testName) -ForegroundColor Red
    }
    foreach ($r in @($trxXml.TestRun.Results.UnitTestResult | Where-Object { $_.outcome -eq "NotExecuted" }))
    {
        Write-Host ("    [スキップ] {0}" -f $r.testName) -ForegroundColor Yellow
    }

    # **0 件の合格は OK にしない。** テストが見つからない構成ミスを見逃すため。
    $ok = ($total -gt 0 -and $failed -eq 0)

    if ($RequireDbms)
    {
        # testId → className（UnitTestResult には className が無い）
        $classOf = @{}
        foreach ($u in @($trxXml.TestRun.TestDefinitions.UnitTest)) { $classOf[$u.id] = $u.TestMethod.className }

        $mark = '("{0}")' -f $RequireDbms
        $ran = @($trxXml.TestRun.Results.UnitTestResult | Where-Object {
            $_.outcome -ne "NotExecuted" -and "$($classOf[$_.testId])".Contains($mark) })

        Write-Host ("  {0} のテスト : 実行 {1} 件" -f $RequireDbms, $ran.Count)
        if ($ran.Count -eq 0)
        {
            Write-Host ("  **{0} のテストが 1 件も実行されていません**（起動したのに接続できていない）" -f $RequireDbms) -ForegroundColor Red
            $ok = $false
        }
    }

    # ------------------------------------------------------------------
    # カバレッジ（表示のみ。判定はしない）
    # ------------------------------------------------------------------
    #   coverlet は実行ごとに GUID のフォルダを作るため、trx の隣を探す。
    if ($Coverage)
    {
        $cov = Get-ChildItem (Split-Path $Trx) -Recurse -Filter "coverage.cobertura.xml" -EA SilentlyContinue |
               Sort-Object LastWriteTime -Descending | Select-Object -First 1

        if ($cov)
        {
            [xml]$cx = Get-Content $cov.FullName -Encoding UTF8 -Raw
            Write-Host "  行カバレッジ（判定には使わない）:"
            foreach ($p in @($cx.coverage.packages.package))
            {
                Write-Host ("    {0,-28} {1,6:P1}" -f $p.name, [double]$p.'line-rate')
            }
            Write-Host ("    詳細 : {0}" -f $cov.FullName)
        }
        else
        {
            Write-Host "  カバレッジのファイルが見つかりません" -ForegroundColor Yellow
        }
    }

    return [pscustomobject]@{
        テスト = $Name
        結果   = $(if ($ok) { "OK" } else { "NG" })
        差分   = $failed
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

    # **ビルドが失敗していたら、比較しても意味が無い。**（#564）
    #   前回の実行ファイルが動いているため、結果は期待値と一致し「OK」になる。
    #   比較を省いて、ビルド失敗として報告する。
    if ($buildFailed.ContainsKey($t.Bat))
    {
        Write-Host "  **ビルドに失敗しているため、比較しません。**" -ForegroundColor Red
        Write-Host ("  （{0} のエラー {1} 件）" -f $t.Bat, $buildFailed[$t.Bat].Count) -ForegroundColor Red

        $results += [pscustomobject]@{
            テスト = $t.Name; 結果 = "ビルド失敗"; 差分 = "-"
        }
        continue
    }

    # NUnit（#610）。判定は Get-NUnitResult（クロス DB の各回と共通。#623）
    if ($t.Kind -eq "NUnit")
    {
        $results += Get-NUnitResult -Name $t.Name -Trx $actual -Coverage ([bool]$t.Coverage)
        continue
    }

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
    #
    # **スイッチはスプラッティングで渡す。**（#562）
    #   if で組み合わせを書き分けると、スイッチが増えるたびに分岐が倍になる。
    $cmpArgs = @{
        Expected = $expectedOf[$t.Name]
        Actual   = $actual
        PassThru = $true
    }

    if ($t.SkipLog4net) { $cmpArgs["SkipLog4netTrace"] = $true }
    if ($t.NormBase64)  { $cmpArgs["NormalizeBase64"]  = $true }

    $cmpResult = & $cmp @cmpArgs

    $results += [pscustomobject]@{
        テスト = $t.Name
        結果   = $cmpResult.Result
        差分   = $cmpResult.DiffCount
    }
}

# ------------------------------------------------------------------
# クロス DB : DBMS ごとに 起動 → テスト → 停止（#623）
# ------------------------------------------------------------------
#   **同時に動かすのは、SQL Server ともう 1 つだけ。**
#   ビルドは上のバッチで済んでいるので、ここは --no-build で TestPublicDb だけを回す。
#   **結果のフォルダを回ごとに分ける。** バッチは実行のたびに TestResults を消すが、
#   ここでは消さないので、回ごとのカバレッジが残る。
if ($CrossDb -and $buildFailed.ContainsKey("y_Build_TestCode_NUnit.bat"))
{
    Write-Host ""
    Write-Host "  **NUnit のビルドに失敗しているため、クロス DB は回しません。**" -ForegroundColor Red
}
elseif ($CrossDb)
{
    $nunitDir  = Join-Path $testsRoot "NUnit"
    $csproj    = Join-Path $nunitDir ("{0}\{0}.csproj" -f $crossDbProject)
    $runsettings = Join-Path $nunitDir "TestPublic\coverlet.runsettings"

    foreach ($db in $crossDbServices)
    {
        Write-Host ""
        Write-Host ("=== クロス DB : {0}（{1}） ===" -f $db.Service, $db.Dbms) -ForegroundColor Cyan

        # **始める前から動いていたものは、起動も停止もしない。**（人の作業状態を変えない）
        $wasRunning = Test-LocalPort $db.Port
        if ($wasRunning)
        {
            Write-Host ("  既に動いています（localhost:{0}）。起動・停止はしません。" -f $db.Port) -ForegroundColor Yellow
        }
        else
        {
            Write-Host ("  起動します : {0}" -f $db.Service)
            if (-not (Invoke-LocalService $svcStart $db.Service))
            {
                Write-Host ("  **{0} を起動できませんでした。**" -f $db.Service) -ForegroundColor Red
                # 中途半端に立ったものを残さない
                Invoke-LocalService $svcStop $db.Service | Out-Null
                foreach ($tfm in $db.Tfms)
                {
                    $results += [pscustomobject]@{ テスト = ("{0} ({1}) [{2}]" -f $crossDbProject, $tfm, $db.Service); 結果 = "起動失敗"; 差分 = "-" }
                }
                continue
            }
        }

        try
        {
            # **回す前に、SQL Server（必須）が応答するのを確かめる。**
            #   DBMS を起動した直後は VM の負荷が高く、SQL Server のログインがタイムアウトして
            #   SQL Server 側のテストが落ちたことがある（2026-10-18、MySQL の起動直後）。
            #   一度つながれば応答は戻るので、数回まで待ってから回す。
            $sqlOk = $false
            for ($i = 1; $i -le 4 -and -not $sqlOk; $i++)
            {
                $sqlOk = (Test-SqlServer (Get-ConnectionStringFromConfig $dbConfig) 15).Ok
                if (-not $sqlOk) { Write-Host ("  SQL Server の応答待ち（{0} 回目）..." -f $i) -ForegroundColor Yellow }
            }
            if (-not $sqlOk)
            {
                Write-Host "  **SQL Server が応答しません。この回の結果は当てになりません。**" -ForegroundColor Red
            }

            $resDir = Join-Path $nunitDir ("TestResults\CrossDb\{0}" -f $db.Service)
            if (Test-Path $resDir) { Remove-Item $resDir -Recurse -Force }

            foreach ($tfm in $db.Tfms)
            {
                $name = "{0} ({1}) [{2}]" -f $crossDbProject, $tfm, $db.Service
                $trxName = "{0}_{1}.trx" -f $crossDbProject, $tfm
                Write-Host ""
                Write-Host ("=== {0} ===" -f $name) -ForegroundColor Cyan

                $testArgs = @("test", $csproj, "--no-build", "-c", "Debug", "-f", $tfm,
                    "--results-directory", $resDir, "--logger", "trx;LogFileName=$trxName")
                # カバレッジは net10.0 だけで取る（y_Build_TestCode_NUnit.bat と同じ）
                if ($tfm -eq "net10.0") { $testArgs += @("--settings", $runsettings, "--collect", "XPlat Code Coverage") }

                $log = Join-Path $OutputDir ("crossdb_{0}_{1}.log" -f $db.Service, ($tfm -replace '[^A-Za-z0-9]', '_'))
                & dotnet @testArgs *>&1 | Out-File $log -Encoding UTF8

                $results += Get-NUnitResult -Name $name -Trx (Join-Path $resDir $trxName) `
                    -Coverage ($tfm -eq "net10.0") -RequireDbms $db.Dbms
            }
        }
        finally
        {
            # **使い終わったら止める。** 失敗しても、Ctrl+C でも止める。
            if (-not $wasRunning)
            {
                Write-Host ("  停止します : {0}" -f $db.Service)
                Invoke-LocalService $svcStop $db.Service | Out-Null
            }
        }
    }
}

if ($startedSqlServer)
{
    Write-Host ""
    Write-Host "=== クロス DB : 起動した SQL Server を停止します ===" -ForegroundColor Cyan
    Invoke-LocalService $svcStop "sqlserver" | Out-Null
}

# ------------------------------------------------------------------
# サマリ
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ サマリ ================"
Write-Host ""
Write-SummaryTable $results
Write-Host ""
Write-Host ("  期待値・ログ : {0}" -f $OutputDir)
Write-Host  "  実測値       : ワーキング ツリーの Result*.txt（git diff で確認できます）"

# **ビルド失敗は、差分より先に伝える。**（#564）
#   放っておくと前回の実行ファイルの結果を見ることになり、
#   「OK」の意味が変わってしまう。
if ($buildFailed.Count -gt 0)
{
    Write-Host ""
    Write-Host ("  **ビルドに失敗したバッチが {0} 本あります。**" -f $buildFailed.Count) -ForegroundColor Red

    foreach ($bat in ($buildFailed.Keys | Sort-Object))
    {
        Write-Host ("    {0} : エラー {1} 件" -f $bat, $buildFailed[$bat].Count) -ForegroundColor Red
    }

    Write-Host "  **直さない限り、テストは前回の実行ファイルで動きます。**" -ForegroundColor Red
}

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
