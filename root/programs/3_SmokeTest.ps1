<#
.SYNOPSIS
    サンプル アプリを起動して疎通を確認し、合否を一覧する。

.DESCRIPTION
    リリース時の「サンプルを幾つか見繕って手動で疎通を行う」を機械化したもの（#513 段階 3）。

    ＜段階 1・2 との違い＞
      段階 1（2_RunAllTests.ps1）は「出力が前回と同じか」を見る回帰テスト。
      本スクリプトは「起動して、想定どおり動き、例外なく終わるか」を見る疎通テスト。
      期待結果ファイルは持たず、判定条件を定義側に書く。

    ＜対象＞
      非 UI 系（バッチ・CLI）と Web 系（Web アプリ・Web サービス）。
      WinForms / WPF 系は UI Automation が必要で維持費が高いため対象外とし、
      リリース チェックリストの手作業項目として残す。

    ＜ビルド＞
      リポジトリ既定のビルド バッチを呼ぶ。理由は 2_RunAllTests.ps1 と同じで、
      csproj を直接ビルドすると nuget.exe restore が行うネイティブ DLL の配置が漏れる。

      なお 0_ExecAllBat.bat は途中で 1_DeleteDir.bat を繰り返し実行し、
      配下の bin / obj を再帰的に削除する。このため 1_BuildAll.ps1 の完走後に
      残るのは最後にビルドされた Core サンプルだけで、net48 サンプルは残らない。
      本スクリプトが自分でビルドするのはこのため。

    ＜DB＞
      SQL Server の Northwind を使用する。
      RerunnableBatch 系は Orders(830 件) を読み Orders2 へ INSERT するため、
      実行前に Orders2 を空にする必要がある。実行後は 830 件＝初期状態に戻る。

    ＜C# 版と VB 版＞
      VB 版は C# 版からの移植で、疎通の手順がそのまま通る（#542 で突き合わせ済み）。
        ・接続文字列名          ConnectionString_SQL
        ・MVC のログイン画面    Views/Home/Login.cshtml が同一
        ・認証が要る画面        /Crud1/Index（Crud1Controller に Authorize）
        ・WebForms のコントロール ctl00$ContentPlaceHolder_A$txtUserID ほか
      このため判定（Flow / Verify）は両者で共有し、対象定義だけを分ける。
      分けて書くと、サンプルの画面を直したとき片方だけ直す事故が起きる。

.PARAMETER Lang
    対象の言語。CS（既定）/ VB / Both。

    ※ 既定に VB を含めない。リリース時の検証（RELEASE.md 3 節）は C# 版が対象で、
    　 VB を含めると毎回 VB のビルドが乗るため。

.PARAMETER Only
    対象名の部分一致で絞る（例: -Only "Rerunnable"）。

.PARAMETER SkipBuild
    ビルドを省略し、既存のバイナリで疎通のみ行う。

.PARAMETER OutputDir
    ログの保存先。既定は %TEMP%\OpenTouryoSmokeTest。

.EXAMPLE
    .\3_SmokeTest.ps1

.EXAMPLE
    .\3_SmokeTest.ps1 -Only "net48" -SkipBuild

.EXAMPLE
    .\3_SmokeTest.ps1 -Lang VB

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
     2026/08/07  玄人 幸道         Orders2が無ければ作成するようにした
     2026/08/13  玄人 幸道         Lang を追加（VB 版の疎通に対応）
     2026/08/15  玄人 幸道         Args を持たない対象が PowerShell 5.1 で起動できず、
                                   しかも前回の出力で OK と判定されていたのを修正
#>
[CmdletBinding()]
param(
    [ValidateSet("CS", "VB", "Both")]
    [string]$Lang = "CS",
    [string]$Only,
    [switch]$SkipBuild,
    [string]$OutputDir = (Join-Path $env:TEMP "OpenTouryoSmokeTest")
)

$ErrorActionPreference = "Continue"

# ------------------------------------------------------------------
# カレント ディレクトリからの実行を許可する
# ------------------------------------------------------------------
# PowerShell は子プロセスに NoDefaultCurrentDirectoryInExePath=1 を渡すが、
# ビルド バッチは出力フォルダへ cd したうえでパス区切りを含まない名前で
# 実行するため、これが効いていると exe を起動できない。
# （TESTING.md の「PowerShell から .bat を呼ぶときの注意」と同じ）
Remove-Item Env:\NoDefaultCurrentDirectoryInExePath -EA SilentlyContinue

# 本スクリプトは root\programs に置き、その配下の CS / VB を対象とする。
# ビルド バッチもサンプルも各言語のフォルダにあるため、そこを起点にする。
#
# ツール（DaoGen_Tool / DeployZipPackWithHTTP）は C# にしか無いので、
# それらの準備・検証は $csRoot を直接参照してよい。
$csRoot = Join-Path $PSScriptRoot "CS"

# 構成ファイル（接続文字列）と Orders2 の DDL を、どちらから読むか。
# 中身は CS / VB で同じだが、「実際に使われる構成ファイルから読む」という
# 建て付けを崩さないため、対象に含まれる方を使う。
$configRoot = if ($Lang -eq "VB") { Join-Path $PSScriptRoot "VB" } else { $csRoot }

New-Item -ItemType Directory -Force $OutputDir | Out-Null

# サマリの整形。Format-Table は 5.1 で全角の桁を数えないため、自前で揃える。
. (Join-Path $PSScriptRoot "SummaryTable.ps1")

# 対象に与える stdin（空）。
# Start-Process -RedirectStandardInput は実在するファイルを要求するため、先に作る。
# これが無いと Console.ReadKey() が本当にキー入力を待つ。
$emptyIn = Join-Path $OutputDir "empty.in"
if (-not (Test-Path $emptyIn))
{
    New-Item -ItemType File $emptyIn | Out-Null
}

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
# 分割したファイルを読む（#571）
# ------------------------------------------------------------------
# **ドット ソースで読む。** 関数と変数を、この実行スコープへ入れるため。
#
# **順序が要る。** st_Targets.ps1 は他の 3 つが定義したものを参照する。
. (Join-Path $PSScriptRoot "st_Utility.ps1")
. (Join-Path $PSScriptRoot "st_Server.ps1")
. (Join-Path $PSScriptRoot "st_Flow.ps1")
. (Join-Path $PSScriptRoot "st_Targets.ps1")



# 省略されている Dir を、その言語のフォルダで補う。
function Add-DefaultDir($items, [string]$dir)
{
    foreach ($i in $items)
    {
        if (-not $i.ContainsKey("Dir")) { $i.Dir = $dir }
    }
    return $items
}

$targets = @()
if ($Lang -ne "VB") { $targets += @(Add-DefaultDir $targetsCS "CS") }
if ($Lang -ne "CS") { $targets += @(Add-DefaultDir $targetsVB "VB") }

# ------------------------------------------------------------------
# VB のビルドは自己完結しない
# ------------------------------------------------------------------
# C# 側は各ビルド バッチが単独で通るが、VB 側は違う。
# 5_Build_Bat_sample.bat だけを呼んでも、参照するアセンブリが揃っていないため
# 建たない。VB\0_ExecAllBat.bat が前段で行っていることを、ここでも行う。
#
#   2_Build_NuGet_net48.bat（CS 側）  … 1_GetLibrariesFromCS.bat が取りに行く実体
#   1_GetLibrariesFromCS.bat          … CS の Build_net48 を VB 配下へ複写
#   3_Build_Business_net48.bat        … VB の Business（B層基盤）
#   3_Build_BusinessRichClient_net48.bat
#   4_Build_CopyAssemblies.bat        … 参照解決用の Build フォルダを作る
#
# ＜0_ExecAllBat.bat を丸ごと呼ばない理由＞
#   ・1_DeleteDir.bat が bin / obj を消す。疎通は成果物を使うので、消したくない
#   ・WinForms / WPF のサンプルまで建てる。疎通の対象ではないので、時間だけ増える
#   ・timeout 5 が入っており、stdin をリダイレクトすると
#     「ERROR: Input redirection is not supported」がログに出る
$prerequisitesVB = @(
    @{ Dir = "CS"; Bat = "2_Build_NuGet_net48.bat" }
    @{ Dir = "VB"; Bat = "1_GetLibrariesFromCS.bat" }
    @{ Dir = "VB"; Bat = "3_Build_Business_net48.bat" }
    @{ Dir = "VB"; Bat = "3_Build_BusinessRichClient_net48.bat" }
    @{ Dir = "VB"; Bat = "4_Build_CopyAssemblies.bat" }
)

# ------------------------------------------------------------------
# ビルド
# ------------------------------------------------------------------
function Invoke-BuildBat([string]$dir, [string]$bat)
{
    $root = Join-Path $PSScriptRoot $dir
    $path = Join-Path $root $bat

    if (-not (Test-Path $path))
    {
        Write-Host ("  ビルド バッチが見つかりません : {0}\{1}" -f $dir, $bat) -ForegroundColor Red
        return $false
    }

    $log = Join-Path $OutputDir ("build_" + $dir + "_" + ($bat -replace '[^A-Za-z0-9]','_') + ".log")

    # バッチは自分のフォルダから呼ぶ。相対パスで参照を解決しているため、
    # 呼び出し元のカレントが違うと、ソリューションを見つけられない。
    Push-Location $root
    # バッチ末尾（および途中）の pause に対応するため stdin を与える。
    cmd /c "echo. | call `"$path`"" *>&1 | Out-File $log -Encoding UTF8
    Pop-Location

    return $true
}

# ------------------------------------------------------------------
# Web ホストの起動・停止
# ------------------------------------------------------------------
# 起動しただけでは応答できないため、ポートが開くまで待ってから疎通する。
function Wait-Port([int]$port, [int]$timeoutSec = 30)
{
    $sw = [Diagnostics.Stopwatch]::StartNew()
    while ($sw.Elapsed.TotalSeconds -lt $timeoutSec)
    {
        $c = New-Object Net.Sockets.TcpClient
        try
        {
            $c.Connect("127.0.0.1", $port)
            if ($c.Connected) { $c.Close(); return $true }
        }
        catch { }
        finally { $c.Dispose() }
        Start-Sleep -Milliseconds 300
    }
    return $false
}

function Start-WebHost($t, [string]$log)
{
    $root = Join-Path $PSScriptRoot $t.Dir

    if ($t.WebHost -eq "IISExpress")
    {
        $iis = Join-Path $env:ProgramFiles "IIS Express\iisexpress.exe"
        if (-not (Test-Path $iis)) { return $null }

        $site = Join-Path $root $t.Site
        return Start-Process $iis `
            -ArgumentList "/path:`"$site`"", "/port:$($t.Port)", "/systray:false" `
            -PassThru -WindowStyle Hidden `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
    else
    {
        $dll = Join-Path $root $t.Exe
        if (-not (Test-Path $dll)) { return $null }

        # コンテンツ ルートを合わせるため、出力フォルダを作業ディレクトリにする。
        return Start-Process "dotnet" `
            -ArgumentList "`"$dll`"", "--urls", "http://localhost:$($t.Port)" `
            -PassThru -WindowStyle Hidden -WorkingDirectory (Split-Path $dll) `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
}

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
Write-Host ("対象 : {0}" -f $Lang) -ForegroundColor Cyan

$selected = @($targets | Where-Object { -not $Only -or $_.Name -like "*$Only*" })

if (-not $SkipBuild)
{
    # ビルドする単位は「フォルダ ＋ バッチ」。同じバッチ名が CS と VB の
    # 両方にあるため（5_Build_Bat_sample.bat 等）、バッチ名だけでは一意化できない。
    #
    # 並びは「C# の対象 → VB の前提 → VB の対象」とする。
    # 前提を先頭に置くと、-Lang Both で VB の準備が C# の対象より先に流れ、
    # 1_BuildAll.ps1（CS → VB の順）と見た目が揃わない。依存関係は
    # 「VB の前提が VB の対象より前」だけなので、この並びで足りる。
    $builds = @()

    $builds += @($selected | Where-Object { $_.Dir -ne "VB" } |
                 ForEach-Object { @{ Dir = $_.Dir; Bat = $_.Bat; Note = "" } })

    $vb = @($selected | Where-Object { $_.Dir -eq "VB" })
    if ($vb.Count -gt 0)
    {
        $builds += @($prerequisitesVB |
                     ForEach-Object { @{ Dir = $_.Dir; Bat = $_.Bat; Note = "（VB の前提）" } })
        $builds += @($vb | ForEach-Object { @{ Dir = $_.Dir; Bat = $_.Bat; Note = "" } })
    }

    $done = @{}
    foreach ($b in $builds)
    {
        $key = $b.Dir + "\" + $b.Bat
        if ($done.ContainsKey($key)) { continue }
        $done[$key] = $true

        Write-Host ("=== ビルド : {0}{1} ===" -f $key, $b.Note) -ForegroundColor Cyan
        $sw = [Diagnostics.Stopwatch]::StartNew()
        [void](Invoke-BuildBat $b.Dir $b.Bat)
        $sw.Stop()
        Write-Host ("  完了 ({0:N1} 秒)" -f $sw.Elapsed.TotalSeconds)
    }
}

$results = @()

foreach ($t in $selected)
{
    Write-Host ""
    Write-Host ("=== {0} ===" -f $t.Name) -ForegroundColor Cyan

    $safe = ($t.Name -replace '[^A-Za-z0-9]', '_')
    $out  = Join-Path $OutputDir "$safe.txt"

    # ------------------------------------------------------------------
    # Web アプリ
    # ------------------------------------------------------------------
    if ($t.Kind -eq "Web")
    {
        # 前提サービスの確認
        # ※ サービスの開始はシステムの状態を変えるため、ここでは行わない。
        #    足りない場合は対処方法を示して NG にする。
        if ($t.Need)
        {
            $svc = Get-Service $t.Need -EA SilentlyContinue
            if (-not $svc -or $svc.Status -ne "Running")
            {
                $msg = "前提サービス {0} が動いていない → Start-Service {0}" -f $t.Need
                Write-Host ("  {0}" -f $msg) -ForegroundColor Yellow
                $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "前提未達"; 内容 = $msg }
                continue
            }
        }

        $sw = [Diagnostics.Stopwatch]::StartNew()
        # **使う前に消す。**（#571）
        #   残っていると、起動に失敗しても前回のログが読まれる。
        $null = Reset-Log $out

        $proc = Start-WebHost $t $out

        if (-not $proc)
        {
            Write-Host "  Web ホストを起動できません（実行ファイル／IIS Express が無い）" -ForegroundColor Red
            $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "起動不可"; 内容 = "-" }
            continue
        }

        try
        {
            Write-Host ("  起動中 ... (PID {0}, port {1})" -f $proc.Id, $t.Port)
            if (-not (Wait-Port $t.Port))
            {
                Write-Host "  ポートが開きません" -ForegroundColor Red
                $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "NG"; 内容 = "ポートが開かない" }
                continue
            }

            $flow = & $t.Flow "http://localhost:$($t.Port)"
            $sw.Stop()

            $verdict = if ($flow.Ok) { "OK" } else { "NG" }
            Write-Host ("     {0}" -f $flow.Detail)
            Write-Host ("  {0}  ({1:N1} 秒)" -f $verdict, $sw.Elapsed.TotalSeconds) `
                -ForegroundColor $(if ($flow.Ok) { "Green" } else { "Red" })

            $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = $verdict; 内容 = $flow.Detail }
        }
        finally
        {
            # 子プロセスごと確実に落とす。
            Stop-Process -Id $proc.Id -Force -EA SilentlyContinue
        }
        continue
    }

    # ------------------------------------------------------------------
    # プロセス（バッチ・CLI）
    # ------------------------------------------------------------------
    $exe = Join-Path (Join-Path $PSScriptRoot $t.Dir) $t.Exe
    if (-not (Test-Path $exe))
    {
        Write-Host "  実行ファイルが見つかりません" -ForegroundColor Red
        $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "実行ファイル無し"; 内容 = "-" }
        continue
    }

    # 事前準備
    if ($t.Pre)
    {
        try { & $t.Pre }
        catch
        {
            Write-Host ("  事前準備に失敗 : {0}" -f $_.Exception.Message) -ForegroundColor Red
            $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = "準備失敗"; 内容 = "-" }
            continue
        }
    }

    # 実行
    #
    # ＜Start-Process で、PowerShell を経由せずに入出力を扱う理由＞
    #
    #   1. stdin を与えないと止まる
    #      サンプルは終了前に Console.ReadKey() を呼ぶ（SimpleBatch_sample/Program.cs 等）。
    #      stdin がコンソールのままだと**本当にキー入力を待つ**。
    #      実測では SimpleBatch_sample (net48) が 386 秒かかった。
    #      空ファイルを与えると、待たずに InvalidOperationException になる
    #      （この例外は下の判定で除外する）。
    #
    #   2. PowerShell のパイプを通すと、stderr が壊れる
    #      「| Out-File」で受けると、native の stderr が ErrorRecord になり
    #      **コンソール幅で折り返される**。折り返しは語の途中にも入るため、
    #      「does not h ave a console」のように**単語が割れて復元できない**。
    #      幅は環境で違うので、後段の除外規則をいくら調整しても直らない。
    #
    #   Start-Process でファイルへ直接リダイレクトすれば、どちらも起きない。
    #   出力は生のまま残り、環境による差も出ない。
    $err = "$out.err"

    # **前回の出力を必ず消してから実行する。**
    #   $out / $err は対象ごとの固定名で、$OutputDir（既定は %TEMP%）に残る。
    #   消さずに実行すると、**起動に失敗しても前回の出力が判定に使われ、
    #   実行していないのに OK になる**（所要時間が 0.0 秒になるのが兆候）。
    Remove-Item $out, $err -Force -ErrorAction SilentlyContinue

    Write-Host "  実行中 ..."
    $sw = [Diagnostics.Stopwatch]::StartNew()

    if ($exe -like "*.dll")
    {
        # 「/DAP」のように「/」で始まる引数は dotnet 自身のオプションと紛らわしいため、
        # 「--」で区切って渡す（リポジトリのビルド バッチも同じ渡し方をしている）。
        # 一方 System.CommandLine を使う CLI では「--」以降が未解析トークン扱いになり、
        # サブコマンドが認識されなくなるため、区切らずに渡す。
        $needSep = @($t.Args | Where-Object { $_ -like "/*" }).Count -gt 0

        $argList = @("`"$exe`"")
        if ($needSep) { $argList += "--" }
        $argList += @($t.Args | Where-Object { $null -ne $_ })

        Start-Process "dotnet" -ArgumentList $argList -NoNewWindow -Wait `
            -WorkingDirectory (Split-Path $exe) `
            -RedirectStandardInput $emptyIn -RedirectStandardOutput $out -RedirectStandardError $err
    }
    else
    {
        # **null 要素を落としてから数える。**
        #   Args を持たない対象では $t.Args が $null になり、@($null) は
        #   「要素数 1・中身 null」の配列になる。**Count は 0 にならない**ので、
        #   下のガードをすり抜けて -ArgumentList に null 入りの配列が渡る。
        #   これを Windows PowerShell 5.1 は拒否する（PS 7 は空文字に変換して通す）。
        #
        #     PS 5.1 : Start-Process : パラメーター 'ArgumentList の引数を確認できません。…
        #     PS 7   : そのまま動く
        #
        #   5.1 でだけ落ちるため、7 で検証していると気付けない。
        $argList = @($t.Args | Where-Object { $null -ne $_ })

        if ($argList.Count -eq 0)
        {
            # -ArgumentList に空の配列を渡すとエラーになる。
            Start-Process $exe -NoNewWindow -Wait `
                -WorkingDirectory (Split-Path $exe) `
                -RedirectStandardInput $emptyIn -RedirectStandardOutput $out -RedirectStandardError $err
        }
        else
        {
            Start-Process $exe -ArgumentList $argList -NoNewWindow -Wait `
                -WorkingDirectory (Split-Path $exe) `
                -RedirectStandardInput $emptyIn -RedirectStandardOutput $out -RedirectStandardError $err
        }
    }

    $sw.Stop()

    # 標準出力と標準エラーを両方見る。
    #
    # **-Encoding UTF8 を必ず付ける。**
    #   Start-Process は子プロセスの生バイトをそのまま書くため、BOM が付かない。
    #   Windows PowerShell 5.1 の Get-Content は BOM が無いと既定の ANSI（日本語環境では
    #   CP932）で読むため、UTF-8 の日本語が化けて期待値に一致しなくなる。
    #   （従来の Out-File -Encoding UTF8 は BOM 付きだったので、たまたま成立していた）
    $text = (Get-Content $out -Raw -Encoding UTF8 -EA SilentlyContinue) + "`n" `
          + (Get-Content $err -Raw -Encoding UTF8 -EA SilentlyContinue)
    if ($null -eq $text) { $text = "" }

    # 判定
    #
    # ※ 末尾の Console.ReadKey() 由来の例外はテスト内容と無関係なので除外する。
    #
    #   PowerShell は native コマンドの stderr を ErrorRecord にし、
    #   **コンソール幅で折り返す**。折り返しで語が分断されると行単位の除外が効かない。
    #   実際 dotnet 起動の 4 件だけが、除外できずに NG になった。
    #     例外 : dotnet.exe : Unhandled exception. ... does not h
    #                                                         ↑ ここで改行
    #   このため、**空白を畳んで 1 行にしてから**除外・判定する。
    #   コンソール幅に依存しなくなる。
    $flat = ($text -replace '\s+', ' ')

    # ReadKey 由来の例外を、宣言からスタック トレースまで丸ごと落とす。
    #
    # ＜メッセージ本文で判定しない理由＞
    #   文言も見出しも環境で変わる。実測した 3 通り。
    #     .NET (Core) 英語 : Unhandled exception. System.InvalidOperationException: Cannot read keys ...
    #     .NET Fx     英語 : Unhandled Exception: System.InvalidOperationException: Cannot read keys ...
    #     .NET Fx     日本語: Exception.ToString() が失敗したため、例外文字列を表示できません。
    #   見出しは「.」と「:」で違い、日本語環境では**型名すら出ない**。
    #   このため本文ではなく、**言語に依存しないスタック トレース**で捕まえる。
    #
    # ＜クラス名も実行環境で違う＞
    #     .NET Fx     : at System.Console.ReadKey(Boolean intercept)
    #     .NET (Core) : at System.ConsolePal.ReadKey(Boolean intercept)
    #   このため System.〇〇.ReadKey として受ける。
    $flat = $flat -replace `
        '(Unhandled Exception[.:]\s*)?System\.\w+(\.\w+)*Exception[^|]*?at System\.\w+\.ReadKey[^ ]*', ''

    $fatal = @()
    $m0 = [regex]::Match($flat, '(Unhandled exception|ハンドルされない例外|System\.\w+Exception).{0,160}')
    if ($m0.Success)
    {
        $fatal = @($m0.Value.Trim())
    }

    $ok = $true
    $detail = ""

    if ($fatal)
    {
        $ok = $false
        $detail = ($fatal | Select-Object -First 1).Trim()
        Write-Host ("  例外 : {0}" -f $detail) -ForegroundColor Red
    }
    elseif ($t.Expect -and $text -notmatch $t.Expect)
    {
        $ok = $false
        $detail = "期待する出力が無い : " + $t.Expect
        Write-Host ("  {0}" -f $detail) -ForegroundColor Red
    }

    if ($ok -and $t.Expect)
    {
        $m = [regex]::Match($text, $t.Expect)
        $detail = $m.Value
        Write-Host ("     出力 : {0}" -f $m.Value)
    }

    if ($ok -and $t.Verify)
    {
        if (-not (& $t.Verify))
        {
            $ok = $false
            $detail = "検証に失敗"
            Write-Host "  検証に失敗" -ForegroundColor Red
        }
    }

    $verdict = if ($ok) { "OK" } else { "NG" }
    Write-Host ("  {0}  ({1:N1} 秒)" -f $verdict, $sw.Elapsed.TotalSeconds) `
        -ForegroundColor $(if ($ok) { "Green" } else { "Red" })

    $results += [pscustomobject]@{ 対象 = $t.Name; 結果 = $verdict; 内容 = $detail }
}

# ------------------------------------------------------------------
# サマリ
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ サマリ ================"
Write-Host ""
Write-SummaryTable $results
Write-Host ""
Write-Host ("  ログ : {0}" -f $OutputDir)

$ng = @($results | Where-Object { $_.結果 -ne "OK" })
if ($ng.Count -eq 0)
{
    Write-Host "  全対象 OK" -ForegroundColor Green
    exit 0
}
else
{
    Write-Host ("  {0} 件が NG" -f $ng.Count) -ForegroundColor Yellow
    exit 1
}
