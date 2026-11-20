<#
.SYNOPSIS
    Sample4SpRp の E2E が使う IdP（-Idp Stub / KCK）の違いを、1 か所にまとめる（E2E から dot-source する）。

.DESCRIPTION
    ＜何をまとめるか＞
      **E2E のケースと対照は、IdP によらず同じ。** 違うのは、次だけである。

      | 違い | Stub（MPAS のスタブ） | KCK（Keycloak） |
      |---|---|---|
      | issuer | https://localhost:44310 | https://localhost:44320/realms/sprp |
      | 起動 | ..\container\Stub\StubCompose.ps1 | ..\container\KCK\KckCompose.ps1 |
      | サンプルの設定 | そのまま（既定がスタブ） | **SpRp_Issuer・SpRp_DiscoveryUri だけを差し替える**（Config）。外部ログインのサンプルは、口（認可・トークン・UserInfo・JWK・SAML2 のメタデータ）も（WebConfig） |
      | ログイン画面 | id="LoginForm"（Email・Password） | id="kc-form-login"（username・password） |
      | 同意画面 | name="submit.Grant" | /login-actions/consent（accept・cancel） |
      | テスト利用者のパスワード | スタブの土台の appsettings.json の TestUserPWD | container\KCK\realm\sprp-realm.json |

      **client_id・戻り先・秘密は、Keycloak にもスタブと同じ値で登録してある**（sprp-realm.json）。サンプルの設定は、issuer と Discovery だけを変えればよい。

    ＜読むときの注意＞
      **Keycloak は、同意を利用者ごとに覚える。** 一度同意した利用者には、同意画面が出ない。
      **Device の拒否（対照）は、同意したことの無い利用者（deny_tanaka）で行う**（DenyUser）。

    ＜Linux の上で動かすとき（#703）＞
      **net10.0 の対象だけを回す**（各 E2E が絞る）。窓の指定（-WindowStyle）を外し、
      サンプルの設定の C:/root/... を、このリポジトリの root/... に読み替えて渡す（Get-E2EPathConfig）。

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/13  玄人 幸道         新規（Keycloak の KCK を足した。#690）
     2026/11/13  玄人 幸道         外部ログインのサンプル用の差し替え（WebConfig）と、SAML2 の口（Saml2Path）を足した（#692）
     2026/11/18  玄人 幸道         Linux の上で動かすときの違い（窓・C:/root のパス）を足した（#703）
     2026/11/19  玄人 幸道         MPAS のスタブの同意画面の送信を、フォームの action へ送るようにした（MPAS#359 で、送信の口が <認可の口>/consent に移った。#710）
#>

# IdP の値を返す。$e2eDir : E2E フォルダ（container\KCK の場所を決める）
function Get-E2EIdp([string]$idp, [string]$e2eDir)
{
    if ($idp -eq 'KCK')
    {
        $base = 'https://localhost:44320'
        $issuer = "$base/realms/sprp"
        $realm = Join-Path $e2eDir '..\container\KCK\realm\sprp-realm.json'
        return @{
            Kind      = 'KCK'
            Name      = 'Keycloak'
            Base      = $base
            Issuer    = $issuer
            Discovery = "$issuer/.well-known/openid-configuration"
            Authorize = "$issuer/protocol/openid-connect/auth"
            StartHint = '..\container\KCK\KckCompose.ps1 で起動する'
            # **サンプルの設定のうち、差し替えるもの**（キーは部品が読むもの。CmnClientParams）
            Config    = @{ SpRp_Issuer = $issuer; SpRp_DiscoveryUri = "$issuer/.well-known/openid-configuration" }
            # **外部ログインのサンプル（MVC_Sample・WebForms_Sample）は、口を Discovery ではなく設定から読む**ので、口も差し替える（#692）
            WebConfig = @{
                SpRp_Issuer           = $issuer
                SpRp_DiscoveryUri     = "$issuer/.well-known/openid-configuration"
                SpRp_JwkSetUri        = "$issuer/protocol/openid-connect/certs"
                SpRp_AuthRequestUri   = "$issuer/protocol/openid-connect/auth"
                SpRp_TokenRequestUri  = "$issuer/protocol/openid-connect/token"
                SpRp_UserInfoUri      = "$issuer/protocol/openid-connect/userinfo"
                SpRp_Saml2MetadataUri = "$issuer/protocol/saml/descriptor"
                SpRp_Saml2EntityId    = $issuer
            }
            Saml2Path = "$issuer/protocol/saml"
            User      = 'tanaka@example.com'
            DenyUser  = 'deny_tanaka@example.com'
            RealmFile = $realm
        }
    }

    $base = 'https://localhost:44310'
    return @{
        Kind      = 'Stub'
        Name      = 'MPAS のスタブ'
        Base      = $base
        Issuer    = $base
        Discovery = "$base/.well-known/openid-configuration"
        Authorize = "$base/authorize"
        StartHint = '..\container\Stub\StubCompose.ps1 で起動する'
        Config    = @{}
        WebConfig = @{}
        Saml2Path = "$base/saml2request"
        User      = 'tanaka@example.com'
        DenyUser  = 'tanaka@example.com'
        RealmFile = ''
    }
}

# ------------------------------------------------------------------
# Linux の上で動かすときの違い（#703 ph3。CI の linux-e2e）
# ------------------------------------------------------------------
#   **Linux の上では、net10.0 の対象だけを回す**（net48・IIS Express は Windows でしか動かない。各 E2E が絞る）。
#   5.1 には $IsLinux が無い（$null）ので、Windows として扱う。
function Test-E2EOnLinux
{
    return [bool]$IsLinux
}

# Start-Process に足す引数（Windows では窓を出さない。**Linux の pwsh は -WindowStyle を受け付けない**）
function Get-E2EHiddenWindow
{
    if (Test-E2EOnLinux) { return @{} }
    return @{ WindowStyle = 'Hidden' }
}

# **C:/root/... を、このリポジトリの root/... に読み替える**（Linux の上だけ。Windows では、そのまま返す）。
#   サンプルの設定は、Windows に展開したときの配置（C:/root/...）を指す。Linux では作業フォルダからの相対パスとして読まれる。
function Convert-E2EPath([string]$path, [string]$e2eDir)
{
    if (-not (Test-E2EOnLinux)) { return $path }
    $m = [regex]::Match($path, '^C:[\\/]+root[\\/]+(.*)$')
    if (-not $m.Success) { return $path }
    # E2E → Sample4SpRp → CS → programs → root → リポジトリ
    $repoRoot = (Resolve-Path (Join-Path $e2eDir '../../../../..')).Path
    return (Join-Path $repoRoot ('root/' + $m.Groups[1].Value)) -replace '\\', '/'
}

# **サンプルの appsettings.json のうち、C:/root/... を指す値を、読み替えた値で返す**（Linux の上だけ。Windows では空）。
#   差し替えは、ソースを変えずに環境変数 appSettings__<キー>（Kestrel）か、写しの appsettings.json（CLI）で行う
#   （サンプルの appsettings.json に書いてある、別の環境へ移すときのやり方。13_CONFIGURATION.md 3〜7 節）。
function Get-E2EPathConfig([string]$appSettingsJson, [string]$e2eDir)
{
    $config = @{}
    if (-not (Test-E2EOnLinux) -or -not (Test-Path $appSettingsJson)) { return $config }
    foreach ($m in [regex]::Matches([IO.File]::ReadAllText($appSettingsJson), '"([A-Za-z0-9_]+)"\s*:\s*"(C:/root/[^"]*)"'))
    {
        $config[$m.Groups[1].Value] = Convert-E2EPath $m.Groups[2].Value $e2eDir
    }
    return $config
}

# Keycloak のテスト利用者のパスワード（sprp-realm.json の users）。読めなければ $null
function Get-KckPassword($idpInfo, [string]$userName)
{
    if (-not (Test-Path $idpInfo.RealmFile)) { return $null }
    $realm = [IO.File]::ReadAllText($idpInfo.RealmFile) | ConvertFrom-Json
    $u = @($realm.users | Where-Object { $_.username -eq $userName })
    if ($u.Count -eq 0) { return $null }
    return [string]$u[0].credentials[0].value
}

# **MPAS のスタブの同意画面（name="submit.Grant"）の送信先を返す**（ブラウザと同じく、フォームの action を読む）。
#   **MPAS#359 から、同意の送信は <認可の口>/consent（フォームの action）が受ける。** 認可の URL へ送ると、
#   クエリとフォームでパラメタが重なり、invalid_request（A parameter is included more than once.）になる（#710）。
#   $res   : Send-Http の戻り値（Status・Url・Body）
#   戻り値 : 送信先の URL（action が無ければ、従来どおり画面の URL）
function Get-StubConsentUrl($res)
{
    foreach ($form in [regex]::Matches($res.Body, '(?is)<form\b[^>]*>.*?</form>'))
    {
        if ($form.Value -notmatch 'name="submit\.Grant"') { continue }

        $m = [regex]::Match($form.Value, '(?is)^<form\b[^>]*?\baction="([^"]*)"')
        if ($m.Success -and $m.Groups[1].Value -ne '')
        {
            return (New-Object Uri((New-Object Uri($res.Url)), [System.Net.WebUtility]::HtmlDecode($m.Groups[1].Value))).AbsoluteUri
        }
    }

    return $res.Url
}

# **Keycloak の画面（ログイン・同意）なら、送る先と値を返す**（それ以外の画面なら $null）。
#   $res     : Send-Http の戻り値（Status・Url・Body）
#   $email   : ログインに使う利用者（Keycloak は、利用者名でもメール アドレスでも通る）
#   $consent : 同意画面で押すボタン（accept / cancel）
#   戻り値   : @{ Url = <POST 先>; Form = <hashtable> }
function Get-KckFormPost($res, [string]$email, [string]$password, [string]$consent = 'accept')
{
    if ($res.Body -match 'id="kc-form-login"')
    {
        $m = [regex]::Match($res.Body, 'id="kc-form-login"[^>]*action="([^"]*)"')
        if (-not $m.Success) { return $null }
        return @{
            Url  = (New-Object Uri((New-Object Uri($res.Url)), [System.Net.WebUtility]::HtmlDecode($m.Groups[1].Value))).AbsoluteUri
            Form = @{ username = $email; password = $password; credentialId = '' }
        }
    }

    $m = [regex]::Match($res.Body, '<form[^>]*action="([^"]*/login-actions/consent[^"]*)"')
    if ($m.Success)
    {
        $code = [regex]::Match($res.Body, 'name="code" value="([^"]*)"')
        $form = @{ code = $(if ($code.Success) { $code.Groups[1].Value } else { '' }) }
        if ($consent -eq 'cancel') { $form['cancel'] = 'No' } else { $form['accept'] = 'Yes' }
        return @{
            Url  = (New-Object Uri((New-Object Uri($res.Url)), [System.Net.WebUtility]::HtmlDecode($m.Groups[1].Value))).AbsoluteUri
            Form = $form
        }
    }

    return $null
}
