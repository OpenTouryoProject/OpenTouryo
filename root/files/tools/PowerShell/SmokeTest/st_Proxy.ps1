<#
.SYNOPSIS
    疎通テスト用の最小 HTTP プロキシ

.DESCRIPTION
    DeployZipPackWithHTTP の /ProxyURL 経路を確かめるために立てる（#578）。

    **HttpListener は使えない。** プロキシへの要求は要求行が絶対 URI
    （GET http://host:port/path HTTP/1.1）で来るため、
    プレフィクス登録で受ける HttpListener では扱えない。TcpListener で受ける。

    **平文 HTTP だけを扱う。** CONNECT（HTTPS トンネル）は実装しない。
    配布に使うのは平文であり、そこを確かめれば足りる。

    -User を指定すると Basic 認証を要求する（407 を返す）。
    ProxyUID / ProxyPWD が WebProxy.Credentials に載るかを確かめるため。

    **要求行をログに残す。** 配置が成功しただけでは、
    プロキシを無視して直結した場合と区別がつかない。
    ログに URL が並んで初めて「経路を通った」と言える。

    **単体で起動する**（3_SmokeTest.ps1 から Start-Process で呼ばれる）。

.PARAMETER Port
    待ち受けるポート。

.PARAMETER LogPath
    要求行を書き出す先。

.PARAMETER User
    指定すると Basic 認証を要求する。省略時は素通し。

.PARAMETER Password
    Basic 認証のパスワード。

.PARAMETER MapHost
    この名前宛の要求を -MapTo へ繋ぎ替える。

    **.NET Framework の WebProxy は localhost 宛を無条件にバイパスする**
    （BypassProxyOnLocal = False でも IsBypassed が True）。
    配信サーバが localhost にある限り、net48 ではプロキシを通らない。
    hosts を書き換えるのは環境を変えるので採らず、**プロキシ側で解決する。**

.PARAMETER MapTo
    -MapHost の繋ぎ替え先。既定は localhost。

    **Host ヘッダも書き換える。** IIS Express は Host が localhost でない要求を
    受け付けないため、別名のまま渡すと 4xx になる。

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/21  玄人 幸道         新規作成（#578）
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [int]$Port,
    [Parameter(Mandatory)] [string]$LogPath,
    [string]$User,
    [string]$Password,
    [string]$MapHost,
    [string]$MapTo = "localhost"
)

$ErrorActionPreference = "Stop"

$CRLF = [char]13 + [char]10
$enc  = New-Object System.Text.ASCIIEncoding

function Write-ProxyLog([string]$text)
{
    # **1 行ごとに閉じる。** 開きっぱなしだと、読む側から見えない。
    Add-Content -Path $LogPath -Value $text -Encoding UTF8
}

# 資格情報が要るなら、期待値を先に作っておく
$expected = $null
if ($User)
{
    $expected = [Convert]::ToBase64String($enc.GetBytes($User + ":" + $Password))
}

# ヘッダ部（CRLFCRLF まで）をバイトで読む。**本体は読まない**（GET/HEAD のみ）。
function Read-Header($stream)
{
    $buf = New-Object System.Collections.Generic.List[byte]
    $one = New-Object byte[] 1

    while ($true)
    {
        $n = $stream.Read($one, 0, 1)
        if ($n -le 0) { return $null }

        $buf.Add($one[0])
        $c = $buf.Count

        if ($c -ge 4 -and
            $buf[$c - 4] -eq 13 -and $buf[$c - 3] -eq 10 -and
            $buf[$c - 2] -eq 13 -and $buf[$c - 1] -eq 10)
        {
            return $enc.GetString($buf.ToArray())
        }

        if ($c -gt 65536) { return $null }   # 異常に長いものは捨てる
    }
}

$listener = New-Object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Loopback, $Port)
$listener.Start()

Write-ProxyLog ("[start] port=" + $Port + " auth=" + $(if ($User) { "basic" } else { "none" }))

try
{
    while ($true)
    {
        $client = $listener.AcceptTcpClient()

        try
        {
            $cs = $client.GetStream()
            $cs.ReadTimeout  = 15000
            $cs.WriteTimeout = 15000

            $head = Read-Header $cs
            if (-not $head) { continue }

            $lines = $head -split $CRLF
            $reqLine = $lines[0]

            # ---- Basic 認証 ----
            if ($expected)
            {
                $got = $null
                foreach ($l in $lines)
                {
                    if ($l -match '^(?i)Proxy-Authorization:\s*Basic\s+(\S+)') { $got = $Matches[1] }
                }

                if ($got -ne $expected)
                {
                    Write-ProxyLog ("[407] " + $reqLine)

                    $res = "HTTP/1.1 407 Proxy Authentication Required" + $CRLF +
                           'Proxy-Authenticate: Basic realm="OpenTouryoSmokeTest"' + $CRLF +
                           "Content-Length: 0" + $CRLF +
                           "Connection: close" + $CRLF + $CRLF

                    $b = $enc.GetBytes($res)
                    $cs.Write($b, 0, $b.Length)
                    $cs.Flush()
                    continue
                }
            }

            # ---- 要求行を分解（絶対 URI が来る）----
            $parts = $reqLine -split ' '
            if ($parts.Count -lt 3) { continue }

            $method = $parts[0]
            $uri = $null
            if (-not [Uri]::TryCreate($parts[1], [UriKind]::Absolute, [ref]$uri)) { continue }

            Write-ProxyLog ($method + " " + $parts[1])

            # ---- 上流へ繋ぐ ----
            # **別名なら実体へ繋ぎ替える。**（ループバック バイパス回避）
            $target = $uri.Host
            $mapped = $false
            if ($MapHost -and ($uri.Host -eq $MapHost)) { $target = $MapTo; $mapped = $true }

            $up = New-Object System.Net.Sockets.TcpClient
            $up.Connect($target, $uri.Port)
            $us = $up.GetStream()
            $us.ReadTimeout  = 15000
            $us.WriteTimeout = 15000

            # 要求行を相対形に直し、プロキシ用ヘッダを落として転送する。
            $out = $method + " " + $uri.PathAndQuery + " " + $parts[2] + $CRLF

            foreach ($l in $lines[1..($lines.Count - 1)])
            {
                if ($l -eq "") { continue }
                if ($l -match '^(?i)Proxy-') { continue }
                if ($l -match '^(?i)Connection:') { continue }

                # **別名は上流へ渡さない。** 受け付けてもらえない。
                if ($mapped -and ($l -match '^(?i)Host:')) { continue }

                $out += $l + $CRLF
            }

            if ($mapped) { $out += ("Host: {0}:{1}" -f $target, $uri.Port) + $CRLF }

            # **どちらも閉じる前提にする。** 使い回しを考えずに済み、実装が単純になる。
            $out += "Connection: close" + $CRLF + $CRLF

            $b = $enc.GetBytes($out)
            $us.Write($b, 0, $b.Length)
            $us.Flush()

            # ---- 応答を返す ----
            #   **「閉じるまで読む」ではいけない。**
            #   HEAD には本体が無く、上流が接続を保つとタイムアウトまで戻らない。
            #   長さを見て、読むべき分だけ読む。
            $resHead = Read-Header $us
            if (-not $resHead) { $up.Close(); continue }

            $hb = $enc.GetBytes($resHead)
            $cs.Write($hb, 0, $hb.Length)

            $len = -1
            $chunked = $false

            foreach ($l in ($resHead -split $CRLF))
            {
                if ($l -match '^(?i)Content-Length:\s*(\d+)')      { $len = [int]$Matches[1] }
                if ($l -match '^(?i)Transfer-Encoding:.*chunked')  { $chunked = $true }
            }

            # HEAD は本体を持たない（Content-Length があっても読まない）
            if ($method -eq "HEAD") { $len = 0 }

            $buf = New-Object byte[] 8192

            if ($len -eq 0)
            {
                # 何も読まない
            }
            elseif ($len -gt 0)
            {
                $rest = $len
                while ($rest -gt 0)
                {
                    $want = [Math]::Min($buf.Length, $rest)
                    $n = $us.Read($buf, 0, $want)
                    if ($n -le 0) { break }
                    $cs.Write($buf, 0, $n)
                    $rest -= $n
                }
            }
            else
            {
                # 長さ不明（chunked を含む）。閉じるまで読む。
                while ($true)
                {
                    $n = $us.Read($buf, 0, $buf.Length)
                    if ($n -le 0) { break }
                    $cs.Write($buf, 0, $n)
                }
            }

            $cs.Flush()

            $up.Close()
        }
        catch
        {
            Write-ProxyLog ("[error] " + $_.Exception.Message)
        }
        finally
        {
            $client.Close()
        }
    }
}
finally
{
    $listener.Stop()
}
