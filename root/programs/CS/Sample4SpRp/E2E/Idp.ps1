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
      | サンプルの設定 | そのまま（既定がスタブ） | **SpRp_Issuer・SpRp_DiscoveryUri だけを差し替える**（Config） |
      | ログイン画面 | id="LoginForm"（Email・Password） | id="kc-form-login"（username・password） |
      | 同意画面 | name="submit.Grant" | /login-actions/consent（accept・cancel） |
      | テスト利用者のパスワード | スタブの土台の appsettings.json の TestUserPWD | container\KCK\realm\sprp-realm.json |

      **client_id・戻り先・秘密は、Keycloak にもスタブと同じ値で登録してある**（sprp-realm.json）。サンプルの設定は、issuer と Discovery だけを変えればよい。

    ＜読むときの注意＞
      **Keycloak は、同意を利用者ごとに覚える。** 一度同意した利用者には、同意画面が出ない。
      **Device の拒否（対照）は、同意したことの無い利用者（deny_tanaka）で行う**（DenyUser）。

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/13  玄人 幸道         新規（Keycloak の KCK を足した。#690）
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
        User      = 'tanaka@example.com'
        DenyUser  = 'tanaka@example.com'
        RealmFile = ''
    }
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
