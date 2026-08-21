<#
.SYNOPSIS
    疎通テストのサーバー操作

.DESCRIPTION
    Web ホスト（IIS Express / Kestrel）の起動と停止、HTTP 要求。

    **起動待ちは TCP で繋がるかだけを見る。**
    コンテンツを要求すると、404 でも起動しているため待ち続け、
    プロセスが残る。残った IIS Express は URL の登録を握るので、
    次回以降の起動が 0x800700b7 で失敗し続ける。

    **3_SmokeTest.ps1 からドット ソースで読まれる。**
    単体では動かない（#571 で分割）。
#>

function Stop-ApiWeb
{
    if ($script:apiWebProc -and -not $script:apiWebProc.HasExited)
    {
        $script:apiWebProc.Kill()
        $script:apiWebProc.WaitForExit(5000)
    }
    $script:apiWebProc = $null
}

function Start-ApiWeb([string]$kind, [int]$port)
{
    # **前回の残りを先に止める。**（Start-DeployWeb と同じ理由）
    Stop-ApiWeb

    $log = Reset-Log (Join-Path $OutputDir "api_web_$kind.log")

    if ($kind -eq "net48")
    {
        $iis = Join-Path $env:ProgramFiles "IIS Express\iisexpress.exe"
        if (-not (Test-Path $iis)) { return $false }

        $site = Join-Path $PSScriptRoot "CS\Samples\WS_sample\ASPNETWebService\ASPNETWebService"
        $script:apiWebProc = Start-Process $iis `
            -ArgumentList "/path:`"$site`"", "/port:$port", "/systray:false" `
            -PassThru -WindowStyle Hidden `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }
    else
    {
        $dll = Join-Path $PSScriptRoot ("CS\Samples4NetCore\Backend\ASPNETWebService" +
            "\ASPNETWebService\bin\Debug\net10.0\ASPNETWebService.dll")
        if (-not (Test-Path $dll)) { return $false }

        # コンテンツ ルートを合わせるため、出力フォルダを作業ディレクトリにする。
        $script:apiWebProc = Start-Process "dotnet" `
            -ArgumentList "`"$dll`"", "--urls", "http://localhost:$port" `
            -PassThru -WindowStyle Hidden -WorkingDirectory (Split-Path $dll) `
            -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    }

    # **TCP で繋がるかだけを見る。**（SMOKETEST.md 9 節）
    for ($i = 0; $i -lt 40; $i++)
    {
        Start-Sleep -Milliseconds 500
        try
        {
            $client = New-Object System.Net.Sockets.TcpClient
            $client.Connect("localhost", $port)
            $client.Close()
            return $true
        }
        catch { }
    }

    Stop-ApiWeb
    return $false
}

# ------------------------------------------------------------------
# HTTP 要求
# ------------------------------------------------------------------
# リダイレクトを追わずに状態コードを見たいが、Invoke-WebRequest は
# -MaximumRedirection 0 で 3xx を受け取ると、-SkipHttpErrorCheck を付けていても
# 「The maximum redirection count has been exceeded」で終了エラーになる。
# ここで捕まえ、3xx を正常な結果として返す。
# Cookie を引き継ぐため、セッションは呼び出し側で作って渡す。
function New-WebSession
{
    return New-Object Microsoft.PowerShell.Commands.WebRequestSession
}

function Invoke-Http
{
    param(
        [string]$Uri,
        [string]$Method = "GET",
        $Body,
        $Session,
        # JSON を送る先（WebAPI）で使う。省略すると Invoke-WebRequest の既定
        # （ハッシュテーブルなら application/x-www-form-urlencoded）になる。
        [string]$ContentType
    )

    $p = @{
        Uri = $Uri; Method = $Method; WebSession = $Session
        MaximumRedirection = 0
        # 5.1 は既定で Internet Explorer のエンジンを使い、未構成だと解析に失敗する。
        # 7 では受け付けられて無視されるため、常に付けてよい。
        UseBasicParsing = $true
    }
    # -SkipHttpErrorCheck は PowerShell 7 以降にしかない。
    # 5.1 に渡すとパラメータ束縛で失敗するので付けない。
    # （5.1 では 4xx/5xx が例外になるが、下の catch で状態コードを取り出す）
    if ($PSVersionTable.PSVersion.Major -ge 6) { $p.SkipHttpErrorCheck = $true }
    if ($Body) { $p.Body = $Body }
    if ($ContentType) { $p.ContentType = $ContentType }

    try
    {
        $r = Invoke-WebRequest @p -ErrorAction Stop
        return [pscustomobject]@{ Status = [int]$r.StatusCode; Content = $r.Content; Length = $r.RawContentLength }
    }
    catch
    {
        $resp = $_.Exception.Response
        if ($resp) { return [pscustomobject]@{ Status = [int]$resp.StatusCode; Content = ""; Length = 0 } }
        return [pscustomobject]@{ Status = -1; Content = $_.Exception.Message; Length = 0 }
    }
}
