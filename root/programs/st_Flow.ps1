<#
.SYNOPSIS
    疎通テストの検証フロー

.DESCRIPTION
    アプリケーションの確認手順（WebForms / MVC）。

    **ログインを通して、認証が要る画面に到達できること**まで見る。
    入口ページが 200 を返すだけでは、ホスティングと構成しか確認できない。

    **3_SmokeTest.ps1 からドット ソースで読まれる。**
    単体では動かない（#571 で分割）。
#>

# ------------------------------------------------------------------
# Web アプリの疎通手順
# ------------------------------------------------------------------
# 引数でベース URL を受け取り、@{ Ok = $bool; Detail = $string } を返す。
#
# ＜確認の深さを揃える＞
# 3 つとも「ログインを通して、認証が要る画面に到達できること」まで見る。
# 入口ページが 200 を返すだけでは、ホスティングと構成しか確認できない。
# 認証が要る画面まで通せば、ルーティング・認証・セッションまでを一度に確認できる。
#
#   | 対象                 | 認証の実装          | 到達を確認する画面   |
#   |----------------------|---------------------|----------------------|
#   | MVC_Sample (net48)   | FormsAuthentication | /Crud1/Index         |
#   | MVC_Sample (net10.0) | Cookie 認証         | /Crud1/Index         |
#   | WebForms_Sample      | FormsAuthentication | Aspx/start/menu.aspx |
#
# いずれのサンプルも「ユーザー名が空でなければ認証する」実装のため、資格情報は不要。

# MVC_Sample : net48 は FormsAuthentication、net10.0 は Cookie 認証と実装は異なるが、
# 画面構成と URL は同じなので同じ手順で確認できる。
# 認証後の応答は net48 が 302（RedirectFromLoginPage）、net10.0 が 200（View を返す）
# と分かれるため、ここでは 4xx/5xx でないことだけを見る。
$mvcLoginFlow = {
    param($base)

    $ses = New-WebSession

    $r1 = Invoke-Http "$base/Home/Login" -Session $ses
    if ($r1.Status -ne 200) { return @{ Ok = $false; Detail = "GET /Home/Login = $($r1.Status)" } }

    # ValidateAntiForgeryToken のため、画面からトークンを取り出して送り返す。
    $tok = ([regex]::Match($r1.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')).Groups[1].Value
    if (-not $tok) { return @{ Ok = $false; Detail = "__RequestVerificationToken が取得できない" } }

    # 500 はセッション状態サービスの停止など、環境側の問題であることが多い。
    $body = @{ UserName = "smoke"; Password = "smoke"; normal = "ログイン"; __RequestVerificationToken = $tok }
    $r2 = Invoke-Http "$base/Home/Login" -Method POST -Body $body -Session $ses
    if ($r2.Status -ge 400) { return @{ Ok = $false; Detail = "POST /Home/Login = $($r2.Status)" } }

    # 認証が要る画面。未認証ならログイン画面へ 302 されるため、200 なら認証が通っている。
    $r3 = Invoke-Http "$base/Crud1/Index" -Session $ses
    if ($r3.Status -ne 200) { return @{ Ok = $false; Detail = "GET /Crud1/Index = $($r3.Status)（認証が通っていない）" } }

    return @{ Ok = $true; Detail = "ログイン後 /Crud1/Index = 200" }
}

# WebForms_Sample (net48) : Web.config で <deny users="?" /> のため全画面が要認証。
# ログイン後は defaultUrl の menu.aspx へ遷移する。
$webFormsFlow = {
    param($base)

    $ses = New-WebSession

    $r1 = Invoke-Http "$base/Aspx/start/login.aspx" -Session $ses
    if ($r1.Status -ne 200) { return @{ Ok = $false; Detail = "GET login.aspx = $($r1.Status)" } }

    # WebForms のポストバックには、画面が発行した状態フィールドをそのまま返す必要がある。
    # __VIEWSTATE が取れること自体、ページのライフサイクルが動いている証拠でもある。
    $fields = @{}
    foreach ($n in @("__VIEWSTATE", "__VIEWSTATEGENERATOR", "__EVENTVALIDATION"))
    {
        $m = [regex]::Match($r1.Content, ('name="' + $n + '"[^>]*value="([^"]*)"'))
        if (-not $m.Success) { return @{ Ok = $false; Detail = "$n が取得できない" } }
        $fields[$n] = $m.Groups[1].Value
    }

    # マスタ ページ配下のため、コントロール名は ctl00$ContentPlaceHolder_A$ が付く。
    # btnButton1 がログイン ボタン（btnButton2 は「外部ログイン」。CS / VB とも同じ）。
    $fields["ctl00`$ContentPlaceHolder_A`$txtUserID"]   = "smoke"
    $fields["ctl00`$ContentPlaceHolder_A`$txtPassword"] = "smoke"
    $fields["ctl00`$ContentPlaceHolder_A`$btnButton1"]  = "ログイン"

    $r2 = Invoke-Http "$base/Aspx/start/login.aspx" -Method POST -Body $fields -Session $ses
    if ($r2.Status -ge 400) { return @{ Ok = $false; Detail = "POST login.aspx = $($r2.Status)" } }

    # 認証が要る画面。未認証なら login.aspx へ 302 されるため、200 なら認証が通っている。
    $r3 = Invoke-Http "$base/Aspx/start/menu.aspx" -Session $ses
    if ($r3.Status -ne 200) { return @{ Ok = $false; Detail = "GET menu.aspx = $($r3.Status)（認証が通っていない）" } }

    return @{ Ok = $true; Detail = "ログイン後 menu.aspx = 200" }
}
