<#
.SYNOPSIS
    検証の前提（サービス・DB）の確認（#588）

.DESCRIPTION
    **2_RunAllTests.ps1 と 3_SmokeTest.ps1 の両方からドット ソースで読まれる。**
    スモークだけでなく、**単体テストも DB を使う**ため
    （TestDataAccess と TestBatch）、共有の場所に置いている。
    片方だけに置くと、もう片方で複写することになる。

    **表示だけで、判定は変えない。**
    足りなくても止めず、終了コードにも影響しない。
    **開始も停止もしない。** システムの状態を変える操作だからで、
    CI は専用のステップで開始している
    （SMOKETEST.md 4 節・AGENTS.md の線引き）。
#>

# ------------------------------------------------------------------
# 接続文字列を設定ファイルから読む
# ------------------------------------------------------------------
# **対象が実際に使う設定から読む。**
#   ここで別途ハードコードすると、対象側の変更に追随できなくなる。
function Get-ConnectionStringFromConfig([string]$ConfigPath)
{
    if (-not $ConfigPath -or -not (Test-Path $ConfigPath)) { return $null }

    $xml  = [xml](Get-Content $ConfigPath -Raw -Encoding UTF8)
    $node = $xml.configuration.connectionStrings.add |
            Where-Object { $_.name -eq "ConnectionString_SQL" }

    return $node.connectionString
}

# ------------------------------------------------------------------
# SQL Server に繋がるかの確認
# ------------------------------------------------------------------
# **サービス名で確認してはならない。**
#   既定インスタンス（MSSQLSERVER）と名前付き（MSSQL$SQLEXPRESS）で名前が変わり、
#   リモートなら手元にサービスが無い。
#   **決め打つと、CI を誤って「前提未達」にする。**
#   接続文字列で、接続そのものを試す。
function Test-SqlServer([string]$ConnString, [int]$TimeoutSec = 5)
{
    if (-not $ConnString) { return @{ Ok = $false; Detail = "接続文字列を読めない" } }

    $c = $null
    try
    {
        $b = New-Object System.Data.SqlClient.SqlConnectionStringBuilder $ConnString

        # **$b.ConnectTimeout = ... は動かない。**
        #   SqlConnectionStringBuilder は IDictionary で、PowerShell の代入が
        #   プロパティではなくキーへ回るため、
        #   Keyword not supported: 'ConnectTimeout'. になる。
        #   **接続文字列のキー名で書くこと。**
        $b['Connection Timeout'] = $TimeoutSec

        $c = New-Object System.Data.SqlClient.SqlConnection $b.ConnectionString

        # **待ち時間は、こちら側で断ち切る。**
        #   Connection Timeout だけでは上限にならない。実測では、
        #     到達しないホスト    … 約 21 秒（OS の TCP 再送が先に立つ）
        #     閉じたポート      … 約 12 秒（IPv6/IPv4 と再試行）
        #     無いインスタンス  … 約 5 秒
        #   **確認のために待つ時間を環境任せにしないため**、Wait で打ち切る。
        $task = $c.OpenAsync()
        if (-not $task.Wait($TimeoutSec * 1000))
        {
            return @{ Ok = $false; Detail = ("応答がありません（{0} 秒で打ち切り）" -f $TimeoutSec) }
        }

        return @{ Ok = $true; Detail = ("{0} / {1}" -f $c.DataSource, $c.Database) }
    }
    catch
    {
        # GetBaseException で SqlException 本体を取る。
        # そのままだと AggregateException や PowerShell の包みが表に出る。
        # 1 行に畳む。例外の本文は改行を含むことがあり、表示が崩れる。
        $msg = ($_.Exception.GetBaseException().Message -replace '\s+', ' ')
        if ($msg.Length -gt 72) { $msg = $msg.Substring(0, 72) + " …" }
        return @{ Ok = $false; Detail = $msg }
    }
    finally
    {
        # 打ち切った場合、接続はまだ進行中のことがある。ここでは黙って閉じる。
        if ($c) { try { $c.Close() } catch { } }
    }
}

# ------------------------------------------------------------------
# 前提の一覧を出す
# ------------------------------------------------------------------
# **冒頭で見る。** 対象ごとに見るだけだと、終盤でしか使わないものは
#   気付くまでに数分かかる（aspnet_state は約 7 分後）。
#
# **引数で渡されたものだけを見る。** 呼ぶ側が「選ばれた対象が要るもの」を
#   絞って渡すこと。-Only で絞ったときに、無関係な不足を報告しないため。
function Show-Prerequisites
{
    param(
        [string[]]$Services = @(),
        [string]$DbConfig,
        [int]$TimeoutSec = 5
    )

    if ($Services.Count -eq 0 -and -not $DbConfig) { return }

    Write-Host ""
    Write-Host "=== 前提の確認 ===" -ForegroundColor Cyan

    $missing = @()

    foreach ($name in $Services)
    {
        $svc = Get-Service $name -EA SilentlyContinue
        $ok  = ($svc -and $svc.Status -eq "Running")
        $detail = if ($ok) { "Running" } elseif ($svc) { [string]$svc.Status } else { "未導入" }

        Write-Host ("  {0,-12} {1}" -f $name, $detail) `
            -ForegroundColor $(if ($ok) { "Green" } else { "Yellow" })

        if (-not $ok) { $missing += ("Start-Service {0}      # 管理者権限が必要" -f $name) }
    }

    if ($DbConfig)
    {
        $cs = Get-ConnectionStringFromConfig $DbConfig
        $db = Test-SqlServer $cs $TimeoutSec

        Write-Host ("  {0,-12} {1}" -f "SQL Server", $db.Detail) `
            -ForegroundColor $(if ($db.Ok) { "Green" } else { "Yellow" })

        if (-not $db.Ok) { $missing += "SQL Server の Northwind に接続できること（SMOKETEST.md 4 節）" }
    }

    if ($missing.Count -gt 0)
    {
        Write-Host ""
        Write-Host "  **前提が足りません。このまま進めますが、該当の対象は NG になります。**" -ForegroundColor Yellow
        $missing | ForEach-Object { Write-Host ("  " + $_) }
    }
}
