#Requires -Version 5.1
<#
.SYNOPSIS
    コンテナで動いている ASPNETWebService を確かめる（#582）。

.DESCRIPTION
    **この Resource Server には画面が無い。**
    `MVC_Sample` はブラウザで開けば動作が分かるが、こちらは WebAPI だけなので、
    確かめる手段（クライアント）が要る。それがこのスクリプトである。

    `1_PublishAndUp.bat` でコンテナを起動した状態で実行する。

    ＜見るもの＞

      1. HTTP → HTTPS のリダイレクト（appsettings.Container.json の UseHttpsRedirection=on）
      2. OpenAPI（IDL）が仕様として読める形で返ること（#580）
      3. WebAPI が応答すること（DB を使わないもの／使うもの）

    **どれも「200 が返る」だけでは足りない。**
    リダイレクト先・IDL の中身・応答の JSON まで見る。

    ＜自己署名証明書＞

      `0_SetupCert.ps1` が作る開発用証明書は自己署名なので、検証を通さないと繋がらない。
      **このスクリプトの中だけで**検証を無効にしている（プロセス全体には残さない）。

.PARAMETER HttpPort
    HTTP のポート。既定は 8090（docker-compose.yml のホスト側）。

.PARAMETER HttpsPort
    HTTPS のポート。既定は 8091。

.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File .\3_Test.ps1

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/21  玄人 幸道         新規作成（#582）
#>
[CmdletBinding()]
param(
    [int]$HttpPort = 8090,
    [int]$HttpsPort = 8091
)

$ErrorActionPreference = "Stop"

$httpBase = "http://localhost:$HttpPort"
$httpsBase = "https://localhost:$HttpsPort"

$script:ng = 0

function Write-Result([string]$name, [bool]$ok, [string]$detail)
{
    if ($ok)
    {
        Write-Host ("  OK   {0,-34} {1}" -f $name, $detail) -ForegroundColor Green
    }
    else
    {
        Write-Host ("  NG   {0,-34} {1}" -f $name, $detail) -ForegroundColor Red
        $script:ng++
    }
}

# ------------------------------------------------------------------
# 自己署名証明書を通す
# ------------------------------------------------------------------
# **5.1 と 7 で書き方が違う。**
#   7 は Invoke-WebRequest -SkipCertificateCheck が使えるが、5.1 には無い。
#   5.1 は ServicePointManager のコールバックで通す。
[Net.ServicePointManager]::SecurityProtocol =
    [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12

$isPS7 = $PSVersionTable.PSVersion.Major -ge 6

if (-not $isPS7)
{
    Add-Type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class SmokeCertPolicy : ICertificatePolicy {
    public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem) {
        return true;
    }
}
"@
    [Net.ServicePointManager]::CertificatePolicy = New-Object SmokeCertPolicy
}

function Invoke-Api
{
    param([string]$Uri, [string]$Method = "GET", [switch]$NoRedirect)

    $a = @{ Uri = $Uri; Method = $Method; UseBasicParsing = $true; TimeoutSec = 30 }
    if ($NoRedirect) { $a["MaximumRedirection"] = 0 }
    if ($isPS7) { $a["SkipCertificateCheck"] = $true }

    try
    {
        return @{ Ok = $true; Res = (Invoke-WebRequest @a) }
    }
    catch
    {
        # **リダイレクトは例外になることがある。** 応答が取れれば、そこから読む。
        $r = $_.Exception.Response
        if ($null -ne $r) { return @{ Ok = $false; Res = $r; Ex = $_ } }
        return @{ Ok = $false; Res = $null; Ex = $_ }
    }
}

Write-Host ""
Write-Host "============================================"
Write-Host "  ASPNETWebService (Docker) の確認"
Write-Host "============================================"
Write-Host ("  HTTP  : {0}" -f $httpBase)
Write-Host ("  HTTPS : {0}" -f $httpsBase)
Write-Host ""

# ------------------------------------------------------------------
# 1. HTTP → HTTPS のリダイレクト
# ------------------------------------------------------------------
# **リダイレクト先まで見る。** 3xx が返るだけでは、どこへ飛ぶか分からない。
$r = Invoke-Api ($httpBase + "/openapi/v1.json") -NoRedirect

$status = 0
$location = ""

if ($null -ne $r.Res)
{
    $status = [int]$r.Res.StatusCode
    try { $location = [string]$r.Res.Headers["Location"] } catch { }
    if (-not $location -and $r.Res.PSObject.Properties.Name -contains "Headers")
    {
        try { $location = [string]$r.Res.Headers.Location } catch { }
    }
}

if ($status -lt 300 -or $status -ge 400 -or $location -notlike "https://*")
{
    Write-Result "HTTP -> HTTPS リダイレクト" $false ("status={0} location={1}" -f $status, $location)
}
elseif ($location -notlike ("*:{0}/*" -f $HttpsPort))
{
    # **飛び先のポートまで見る。**（#582）
    #   3xx が返り https で始まるだけでは足りない。
    #   コンテナ内のポート（8081）へ飛ばしていて、ホストから辿れなかった。
    #   UseHttpsRedirection は ASPNETCORE_HTTPS_PORT（**単数形**）を読む。
    Write-Result "HTTP -> HTTPS リダイレクト" $false `
        ("飛び先のポートが違う（ホストから辿れない） : {0}" -f $location)
}
else
{
    # **実際に辿れることまで確かめる。**
    $f = Invoke-Api $location

    if ($f.Ok)
    {
        Write-Result "HTTP -> HTTPS リダイレクト" $true `
            ("{0} -> {1} （辿れる）" -f $status, $location)
    }
    else
    {
        Write-Result "HTTP -> HTTPS リダイレクト" $false `
            ("飛び先へ辿れない : {0}" -f $location)
    }
}

# ------------------------------------------------------------------
# 2. OpenAPI（IDL）
# ------------------------------------------------------------------
# **200 が返るだけでは足りない。** 壊れた文書でも 200 は返る。
$r = Invoke-Api ($httpsBase + "/openapi/v1.json")

if (-not $r.Ok)
{
    Write-Result "OpenAPI (IDL)" $false ("取得できない : " + $r.Ex.Exception.Message)
}
else
{
    $doc = $null
    try { $doc = $r.Res.Content | ConvertFrom-Json } catch { }

    if ($null -eq $doc)
    {
        Write-Result "OpenAPI (IDL)" $false "JSON として読めない"
    }
    elseif (-not $doc.openapi)
    {
        Write-Result "OpenAPI (IDL)" $false "openapi の版が無い"
    }
    else
    {
        $paths = @($doc.paths.PSObject.Properties.Name)

        # **代表的な API が名前で載っていること。**
        #   件数だけだと、コントローラが入れ替わっても気づけない。
        $want = @("/api/Json/Select", "/api/BatchUpdate/BatchUpdate")
        $miss = @($want | Where-Object { $paths -notcontains $_ })

        if ($miss.Count -gt 0)
        {
            Write-Result "OpenAPI (IDL)" $false ("載っていない : " + ($miss -join ", "))
        }
        else
        {
            Write-Result "OpenAPI (IDL)" $true ("openapi {0} / paths {1} 件" -f $doc.openapi, $paths.Count)
        }
    }
}

# ------------------------------------------------------------------
# 3. WebAPI（DB を使わない）
# ------------------------------------------------------------------
# **まず DB に依らないもので、アプリが動いていることを確かめる。**
#   ここが通れば、次が失敗したときに「DB 側の問題」と切り分けられる。
#
#   **メソッドは IDL のとおり GET。**（#582）
#   最初 POST で叩いて 405 になった。**IDL に書いてある**（paths./api/Json/test.get）。
$r = Invoke-Api ($httpsBase + "/api/Json/test") -Method GET

if (-not $r.Ok)
{
    Write-Result "WebAPI /api/Json/test" $false ("応答が無い : " + $r.Ex.Exception.Message)
}
else
{
    $body = $r.Res.Content
    Write-Result "WebAPI /api/Json/test" $true ("{0} / {1} バイト" -f [int]$r.Res.StatusCode, $body.Length)
}

# ------------------------------------------------------------------
# 4. WebAPI（DB を使う）
# ------------------------------------------------------------------
# **リソース（Sql / Xml）とデータベース接続まで通ることを見る。**
#   コンテナ側の %OT_RESOURCE_ROOT% と connectionStrings が効いているかの確認になる。
$r = Invoke-Api ($httpsBase + "/api/BatchUpdate/SelectCount") -Method POST

if (-not $r.Ok)
{
    Write-Result "WebAPI /api/BatchUpdate/SelectCount" $false ("応答が無い : " + $r.Ex.Exception.Message)
}
else
{
    $body = $r.Res.Content
    $ok = $false
    $detail = $body

    try
    {
        $j = $body | ConvertFrom-Json

        # 件数が返ること（0 以上の数値）
        if ($null -ne $j)
        {
            $props = @($j.PSObject.Properties.Name)
            $detail = ("{0} / {1}" -f [int]$r.Res.StatusCode, ($props -join ", "))
            $ok = $true
        }
    }
    catch
    {
        $detail = "JSON として読めない : " + $body
    }

    Write-Result "WebAPI /api/BatchUpdate/SelectCount" $ok $detail
}

# ------------------------------------------------------------------
# 判定
# ------------------------------------------------------------------
Write-Host ""
Write-Host "============================================"

if ($script:ng -eq 0)
{
    Write-Host "  すべて OK です。" -ForegroundColor Green
    Write-Host "============================================"
    Write-Host ""
    exit 0
}

Write-Host ("  **{0} 件が NG**" -f $script:ng) -ForegroundColor Red
Write-Host "============================================"
Write-Host ""
Write-Host "  コンテナのログ : docker compose logs -f"
Write-Host ""
exit 1
