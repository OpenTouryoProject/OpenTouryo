//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：OAuth2AndOIDCConst
//* クラス日本語名  ：OAuth2とOIDCの各種定数
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/10  西野 大介         新規作成（汎用認証サイトからのコード移行）
//*  201X/XX/XX  西野 大介         ...
//*  2020/02/27  西野 大介         FAPI CIBAのパラメタを追加
//*  2020/12/18  西野 大介         Device AuthZのパラメタを追加
//*  2026/09/12  玄人 幸道         RFC 6750 / RFC 7662 / CIBA / OIDCの定数を追加
//*  2026/09/25  玄人 幸道         client_assertion（RFC 7523）の定数を追加
//*  2026/10/22  玄人 幸道         DPoP（RFC 9449）・DCR（RFC 7591 / 7592）・Discovery・promptの定数を追加（#634）
//*  2026/10/22  玄人 幸道         DPoP・DCR・Discoveryの区分をparamの外（PKCE / Device AuthZ / CIBAの並び）へ出し、
//*                                WWW-Authenticateのパラメタを Authorization の外へ移した。preferred_usernameを追加。
//*                                responseのsummaryの誤記を直した
//**********************************************************************************

// urnはClaimのurnで、
// ASP.NETとClaimとJwtのMember間のインターフェイスを形成する。

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2とOIDCの各種定数</summary>
    public class OAuth2AndOIDCConst
    {
        /// <summary>urnのベース部分</summary>
        public static readonly string UrnBase = "urn:oauth:";

        #region param
        /// <summary>client_id</summary>
        public const string client_id = "client_id";

        /// <summary>client_secret</summary>
        public const string client_secret = "client_secret";

        /// <summary>client_assertion（private_key_jwt / client_secret_jwtで使用）</summary>
        public const string client_assertion = "client_assertion";

        /// <summary>client_assertion_type</summary>
        public const string client_assertion_type = "client_assertion_type";

        /// <summary>client_assertionのtype（RFC 7523 §2.2）</summary>
        public const string JwtBearerClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";

        /// <summary>grant_type</summary>
        public const string grant_type = "grant_type";

        /// <summary>response_type</summary>
        public const string response_type = "response_type";

        /// <summary>response_mode</summary>
        public const string response_mode = "response_mode";

        /// <summary>redirect_uri</summary>
        public const string redirect_uri = "redirect_uri";

        /// <summary>scope</summary>
        public const string scope = "scope";

        #region RequestObject & ResponseObject(JARM)
        /// <summary>request</summary>
        public const string request = "request";

        /// <summary>response</summary>
        public const string response = "response";

        /// <summary>request_uri</summary>
        public const string request_uri = "request_uri";

        /// <summary>request_uriのurn</summary>
        public static readonly string UrnRequestUriBase = UrnBase + request + ":";

        #region claims
        /// <summary>claims</summary>
        public const string claims = "claims";

        /// <summary>claims_userinfo</summary>
        public const string claims_userinfo = "userinfo";

        /// <summary>claims_id_token</summary>
        public const string claims_id_token = "id_token";

        /// <summary>claims_essential</summary>
        public const string claims_essential = "essential";

        /// <summary>claims_value</summary>
        public const string claims_value = "value";

        /// <summary>claims_values</summary>
        public const string claims_values = "values";
        #endregion

        #endregion

        #region Option

        /// <summary>max_age</summary>
        public const string max_age = "max_age";

        /// <summary>prompt</summary>
        public const string prompt = "prompt";

        /// <summary>login_hint</summary>
        public const string login_hint = "login_hint";

        #region prompt の値（OIDC Core 3.1.2.1）

        /// <summary>prompt の値 : none（対話しない）</summary>
        public const string PromptNone = "none";

        /// <summary>prompt の値 : login（再認証を求める）</summary>
        public const string PromptLogin = "login";

        /// <summary>prompt の値 : consent（同意を取り直す）</summary>
        public const string PromptConsent = "consent";

        /// <summary>prompt の値 : select_account（アカウントを選ばせる）</summary>
        public const string PromptSelectAccount = "select_account";

        #endregion

        #endregion

        #region 暗号的
        /// <summary>state</summary>
        public const string state = "state";

        /// <summary>nonce</summary>
        public const string nonce = "nonce";

        /// <summary>code</summary>
        public const string code = "code";
        
        /// <summary>token</summary>
        public const string token = "token";

        /// <summary>IDToken</summary>
        public const string IDToken = "id_token";

        /// <summary>token_type</summary>
        public const string token_type = "token_type";

        /// <summary>expires_in</summary>
        public const string expires_in = "expires_in";

        /// <summary>assertion</summary>
        public const string assertion = "assertion";
        #endregion

        #region WebAPI

        /// <summary>token_type_hint</summary>
        public const string token_type_hint = "token_type_hint";

        /// <summary>active（イントロスペクションの応答フィールド）</summary>
        public const string active = "active";

        #endregion

        #endregion

        #region token

        /// <summary>AccessToken</summary>
        public const string AccessToken = "access_token";

        /// <summary>RefreshToken</summary>
        public const string RefreshToken = "refresh_token";

        ///// <summary>IDToken</summary> // 移動
        //public const string IDToken = "id_token";

        #endregion

        #region GrantType

        /// <summary>Authorization Codeグラント種別</summary>
        public const string AuthorizationCodeGrantType = "authorization_code";

        /// <summary>Implicitグラント種別</summary>
        public const string ImplicitGrantType = "implicit"; // well-knownで利用。

        /// <summary>Resource Owner Password Credentialsグラント種別</summary>
        public const string ResourceOwnerPasswordCredentialsGrantType = "password";

        /// <summary>Client Credentialsグラント種別</summary>
        public const string ClientCredentialsGrantType = "client_credentials";

        /// <summary>Refresh Tokenグラント種別</summary>
        public const string RefreshTokenGrantType = RefreshToken;

        /// <summary>JWT bearer token authorizationグラント種別</summary>
        public const string JwtBearerTokenFlowGrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer";

        /// <summary>OAuth 2.0 Device Authorizationグラント種別</summary>
        public const string DeviceAuthZGrantType = "urn:ietf:params:oauth:grant-type:device_code";

        /// <summary>CIBA(Client Initiated Backchannel Authentication)グラント種別</summary>
        public const string CibaGrantType = "urn:openid:params:grant-type:ciba";

        #endregion

        #region ResponseType

        /// <summary>Authorization Codeグラント種別</summary>
        public const string AuthorizationCodeResponseType = code;

        /// <summary>Implicitグラント種別</summary>
        public const string ImplicitResponseType = token;

        /// <summary>OIDC - Implicit</summary>
        public const string OidcImplicit1_ResponseType = IDToken;

        /// <summary>OIDC - Implicit2</summary>
        public const string OidcImplicit2_ResponseType = IDToken + " " + token;

        /// <summary>OIDC - Hybrid(IdToken)</summary>
        public const string OidcHybrid2_IdToken_ResponseType = code + " " + IDToken;

        /// <summary>OIDC - Hybrid(Token)</summary>
        public const string OidcHybrid2_Token_ResponseType = code + " " + token;

        /// <summary>OIDC - Hybrid(IdToken and Token)</summary>
        public const string OidcHybrid3_ResponseType = code + " " + IDToken + " " + token;

        #endregion
        
        #region HTTP Header

        /// <summary>Location</summary>
        public const string HttpHeader_Location = "Location";

        #region Authorization

        /// <summary>Authorization</summary>
        public const string HttpHeader_Authorization = "Authorization";

        /// <summary>Basic</summary>
        public const string Basic = "Basic";

        /// <summary>Bearer</summary>
        public const string Bearer = "Bearer";

        /// <summary>DPoP（Authorization のスキームと、token_type の値。RFC 9449 5 / 7.1）</summary>
        public const string DPoP = "DPoP";

        #endregion

        #region WWW-Authenticate（チャレンジ）

        /// <summary>WWW-Authenticate（RFC 6750 3 / RFC 9449 7.1）</summary>
        public const string HttpHeader_WWWAuthenticate = "WWW-Authenticate";

        /// <summary>realm（RFC 6750 3）</summary>
        public const string realm = "realm";

        /// <summary>algs（証明の署名に使える alg。RFC 9449 7.1）</summary>
        public const string algs = "algs";

        #endregion

        #region DPoP（RFC 9449）

        /// <summary>DPoP（証明を運ぶ要求ヘッダ。RFC 9449 4.1）</summary>
        public const string HttpHeader_DPoP = "DPoP";

        /// <summary>DPoP-Nonce（サーバが渡す nonce。RFC 9449 8）</summary>
        public const string HttpHeader_DPoPNonce = "DPoP-Nonce";

        #endregion

        #endregion

        #region Scope

        #region 標準

        /// <summary>profileを要求するscope</summary>
        public const string Scope_Profile = "profile";

        /// <summary>emailを要求するscope</summary>
        public const string Scope_Email = "email";

        /// <summary>phoneを要求するscope</summary>
        public const string Scope_Phone = "phone";

        /// <summary>addressを要求するscope</summary>
        public const string Scope_Address = "address";

        #endregion

        #region 拡張

        /// <summary>authを要求するscope（認可画面を出さない）</summary>
        public const string Scope_Auth = "auth";

        /// <summary>useridを要求するscope</summary>
        public const string Scope_UserID = "userid";

        /// <summary>rolesを要求するscope</summary>
        public const string Scope_Roles = roles;

        #endregion

        #region id_token

        /// <summary>id_tokenを要求するscope</summary>
        public const string Scope_Openid = "openid";

        #endregion

        #endregion

        #region Claims

        // ★ Scopeと同じ文字列は定義しない。

        /// <summary>Claimのurnのベース部分</summary>
        public static readonly string UrnClaimBase = UrnBase + "claim:";

        #region 予約

        #region 末端

        /// <summary>iss</summary>
        public const string iss = "iss";

        /// <summary>aud</summary>
        public const string aud = "aud";

        /// <summary>sub</summary>
        public const string sub = "sub";

        /// <summary>exp</summary>
        public const string exp = "exp";

        /// <summary>nbf</summary>
        public const string nbf = "nbf";

        /// <summary>iat</summary>
        public const string iat = "iat";

        /// <summary>jti</summary>
        public const string jti = "jti";

        #endregion

        #region urn

        /// <summary>issuerクレームのurn</summary>
        public static readonly string UrnIssuerClaim = UrnClaimBase + iss;

        /// <summary>audienceクレームのurn</summary>
        public static readonly string UrnAudienceClaim = UrnClaimBase + aud;

        /// <summary>subjectクレームのurn</summary>
        public static readonly string UrnSubjectClaim = UrnClaimBase + sub;

        /// <summary>expクレームのurn</summary>
        public static readonly string UrnExpirationTimeClaim = UrnClaimBase + exp;

        /// <summary>nbfクレームのurn</summary>
        public static readonly string UrnNotBeforeClaim = UrnClaimBase + nbf;

        /// <summary>iatクレームのurn</summary>
        public static readonly string UrnIssuedAtClaim = UrnClaimBase + iat;

        /// <summary>jtiクレームのurn</summary>
        public static readonly string UrnJwtIdClaim = UrnClaimBase + jti;

        #endregion

        #endregion

        #region 標準

        #region 末端

        /// <summary>email_verified</summary>
        public const string email_verified = "email_verified";

        /// <summary>phone_number</summary>
        public const string phone_number = "phone_number";

        /// <summary>phone_number_verified</summary>
        public const string phone_number_verified = "phone_number_verified";

        /// <summary>preferred_username（OIDC Core 5.1）</summary>
        public const string preferred_username = "preferred_username";

        #endregion

        #region urn

        /// <summary>emailクレームのurn</summary>
        public static readonly string UrnEmailClaim = UrnClaimBase + Scope_Email;

        /// <summary>email_verifiedクレームのurn</summary>
        public static readonly string UrnEmailVerifiedUrn = UrnClaimBase + email_verified;

        /// <summary>phone_numberクレームのurn</summary>
        public static readonly string UrnPhoneNumberClaim = UrnClaimBase + phone_number;

        /// <summary>phone_number_verifiedクレームのurn</summary>
        public static readonly string UrnPhoneNumberVerifiedClaim = UrnClaimBase + phone_number_verified;

        #endregion

        #endregion

        #region OIDC

        #region 末端

        /// <summary>at_hash</summary>
        public const string at_hash = "at_hash";

        /// <summary>c_hash</summary>
        public const string c_hash = "c_hash";

        /// <summary>
        /// acr（Authentication Context Class Reference
        /// </summary>
        public const string acr = "acr";

        /// <summary>
        /// amr（仕様の範囲外で、利用者が規則を決めて運用
        /// </summary>
        public const string amr = "amr";

        /// <summary>
        /// azp（aud ≠ client_idのケース
        /// </summary>
        public const string azp = "azp";

        /// <summary>
        /// auth_time（max_age が含まれていた場合は必須
        /// </summary>
        public const string auth_time = "auth_time";

        #endregion

        #region urn
        /// <summary>nonceクレームのurn</summary>
        public static readonly string UrnNonceClaim = UrnClaimBase + nonce;

        /// <summary>at_hashクレームのurn</summary>
        public static readonly string UrnAtHashClaim = UrnClaimBase + at_hash;

        /// <summary>c_hashクレームのurn</summary>
        public static readonly string UrnCHashClaim = UrnClaimBase + c_hash;

        /// <summary>acrクレームのurn</summary>
        public static readonly string UrnAcrClaim = UrnClaimBase + acr;

        /// <summary>amrクレームのurn</summary>
        public static readonly string UrnAmrClaim = UrnClaimBase + amr;

        /// <summary>azpクレームのurn</summary>
        public static readonly string UrnAzpClaim = UrnClaimBase + azp;

        /// <summary>auth_timeクレームのurn</summary>
        public static readonly string UrnAuthTimeClaim = UrnClaimBase + auth_time;
        #endregion

        #endregion

        #region FAPI・送信者制約（cnf の x5t / jkt）

        #region 末端

        /// <summary>s_hash</summary>
        public const string s_hash = "s_hash";

        /// <summary>cnf</summary>
        public const string cnf = "cnf";

        /// <summary>x5t</summary>
        public const string x5t = "x5t";

        /// <summary>x5u</summary>
        public const string x5u = "x5u";

        /// <summary>jkt（DPoP の鍵の指紋。cnf のメンバ。RFC 9449 6）</summary>
        public const string jkt = "jkt";

        // 独自

        /// <summary>fapi</summary>
        public const string fapi = "fapi"; // 使用中

        #endregion

        #region urn

        /// <summary>s_hashクレームのurn</summary>
        public static readonly string UrnSHashClaim = UrnClaimBase + s_hash;

        /// <summary>cnfクレームのurn</summary>
        public static readonly string UrnCnfClaim = UrnClaimBase + cnf;

        /// <summary>x5tクレームのurn</summary>
        public static readonly string UrnCnfX5tClaim = UrnCnfClaim + ":" + x5t;

        /// <summary>x5uクレームのurn</summary>
        public static readonly string UrnCnfX5uClaim = UrnCnfClaim + ":" + x5u;

        /// <summary>jktクレームのurn（DPoP。RFC 9449 6）</summary>
        public static readonly string UrnCnfJktClaim = UrnCnfClaim + ":" + jkt;

        //// 独自

        ///// <summary>fapiクレームのurn</summary>
        //public static readonly string UrnFApiClaim = UrnClaimBase + fapi;

        #endregion

        #endregion

        #region 拡張

        #region 末端

        /// <summary>scopes</summary>
        public const string scopes = "scopes";

        /// <summary>roles</summary>
        public const string roles = "roles";

        #endregion

        #region urn

        // AccessTokenに格納する

        /// <summary>scopesクレームのurn</summary>
        public static readonly string UrnScopesClaim = UrnClaimBase + scopes;

        /// <summary>claimsクレームのurn</summary>
        public static readonly string UrnClaimsClaim = UrnClaimBase + claims;

        /// <summary>rolesクレームのurn</summary>
        public static readonly string UrnRolesClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        #endregion

        #endregion

        #endregion

        #region PKCE

        /// <summary>code_verifier</summary>
        public const string code_verifier = "code_verifier";

        /// <summary>code_challenge</summary>
        public const string code_challenge = "code_challenge";

        /// <summary>code_challenge_method</summary>
        public const string code_challenge_method = "code_challenge_method"; 

        /// <summary>PKCE plain</summary>
        public const string PKCE_plain = "plain";

        /// <summary>PKCE S256</summary>
        public const string PKCE_S256 = "S256";

        #endregion

        #region Device AuthZ

        #region 認可リクエスト
        // client_id
        #endregion

        #region 認可レスポンス
        /// <summary>Device Code</summary>
        public const string device_code = "device_code";

        /// <summary>User Code</summary>
        public const string user_code = "user_code";

        /// <summary>Verification Uri</summary>
        public const string verification_uri = "verification_uri";

        /// <summary>Verification Uri Complete</summary>
        public const string verification_uri_complete = "verification_uri_complete";

        /// <summary>Polling Interval</summary>
        public const string PollingInterval = "interval";
        #endregion

        #endregion

        #region FAPI CIBA

        #region 認可リクエスト
        /// <summary>Login Hint Token</summary>
        public const string login_hint_token = "login_hint_token";

        /// <summary>Id Token Hint</summary>
        public const string id_token_hint = "id_token_hint";

        /// <summary>Requested Expiry</summary>
        public const string requested_expiry = "requested_expiry";
        
        /// <summary>Client Notification Token</summary>
        public const string client_notification_token = "client_notification_token";
        #endregion

        #region 認可レスポンス
        /// <summary>Auth Req Id</summary>
        public const string auth_req_id = "auth_req_id";

        // expires_in
        // interval

        /// <summary>Binding Message</summary>
        public const string binding_message = "binding_message";
        //user_code
        #endregion

        #endregion

        #region DPoP

        // 証明のヘッダ（typ / jwk）は JwtConst、cnf.jkt は Claims の FAPI、
        // HTTP ヘッダとスキームは HTTP Header、error 値は Error に置いている。

        /// <summary>dpop_jkt（認可要求で、コードを縛る鍵の指紋を先に示す。RFC 9449 10）</summary>
        public const string dpop_jkt = "dpop_jkt";

        /// <summary>htm（証明の HTTP メソッド。RFC 9449 4.2）</summary>
        public const string htm = "htm";

        /// <summary>htu（証明の宛先。RFC 9449 4.2）</summary>
        public const string htu = "htu";

        /// <summary>ath（証明に入れる access_token の指紋。RFC 9449 4.2）</summary>
        public const string ath = "ath";

        #endregion

        #region DCR

        #region クライアント メタデータ
        /// <summary>redirect_uris（RFC 7591 2）</summary>
        public const string redirect_uris = "redirect_uris";

        /// <summary>software_statement（RFC 7591 2.3）</summary>
        public const string software_statement = "software_statement";
        #endregion

        #region 登録の応答
        /// <summary>client_id_issued_at（RFC 7591 3.2.1）</summary>
        public const string client_id_issued_at = "client_id_issued_at";

        /// <summary>client_secret_expires_at（RFC 7591 3.2.1）</summary>
        public const string client_secret_expires_at = "client_secret_expires_at";

        /// <summary>registration_client_uri（登録を管理する口。RFC 7592 3）</summary>
        public const string registration_client_uri = "registration_client_uri";

        /// <summary>registration_access_token（登録を管理するトークン。RFC 7592 3）</summary>
        public const string registration_access_token = "registration_access_token";
        #endregion

        #endregion

        #region Discovery

        /// <summary>Discovery の口のパス（OIDC Discovery 4.1）</summary>
        public const string WellKnownOpenIdConfiguration = "/.well-known/openid-configuration";

        #region メタデータの項目
        /// <summary>issuer</summary>
        public const string issuer = "issuer";

        /// <summary>jwks_uri</summary>
        public const string jwks_uri = "jwks_uri";

        /// <summary>registration_endpoint（RFC 7591 / RFC 8414 2）</summary>
        public const string registration_endpoint = "registration_endpoint";
        #endregion

        #endregion

        #region LoA(acr)

        #region urn
        /// <summary>LoAのurnのベース部分</summary>
        public static readonly string UrnLoABase = "urn:mace:incommon:iap:";
        
        /// <summary>LoA1のurn</summary>
        public static readonly string UrnLoA1 = UrnLoABase + "bronze";

        /// <summary>LoA2のurn</summary>
        public static readonly string UrnLoA2 = UrnLoABase + "silver";
        #endregion

        #endregion

        #region Error

        /// <summary>error</summary>
        public const string error = "error";

        /// <summary>error_description</summary>
        public const string error_description = "error_description";

        /// <summary>error_uri</summary>
        public const string error_uri = "error_uri";

        #region error値

        // エラー コードは、ここに定数として定義する。
        // 但し、トークン エンドポイントのポーリング状態
        // （authorization_pendingなど）は、状態遷移を伴うため、
        // OAuth2AndOIDCEnumのDeviceAuthZState、CibaStateに定義し、
        // ToStringByEmitで文字列化して使用する（access_deniedのみ両方に在る）。

        #region RFC 6749
        /// <summary>invalid_request</summary>
        public const string invalid_request = "invalid_request";

        /// <summary>invalid_client</summary>
        public const string invalid_client = "invalid_client";

        /// <summary>unauthorized_client</summary>
        public const string unauthorized_client = "unauthorized_client";

        /// <summary>invalid_grant</summary>
        public const string invalid_grant = "invalid_grant";

        /// <summary>unsupported_grant_type</summary>
        public const string unsupported_grant_type = "unsupported_grant_type";

        /// <summary>access_denied</summary>
        public const string access_denied = "access_denied";

        /// <summary>unsupported_response_type</summary>
        public const string unsupported_response_type = "unsupported_response_type";

        /// <summary>invalid_scope</summary>
        public const string invalid_scope = "invalid_scope";

        /// <summary>server_error</summary>
        public const string server_error = "server_error";

        /// <summary>temporarily_unavailable</summary>
        public const string temporarily_unavailable = "temporarily_unavailable";
        #endregion

        #region RFC 6750
        /// <summary>invalid_token（Bearerトークンが無効）</summary>
        public const string invalid_token = "invalid_token";

        /// <summary>insufficient_scope（トークンのscopeが足りない）</summary>
        public const string insufficient_scope = "insufficient_scope";
        #endregion

        #region OIDC Core
        /// <summary>login_required（prompt=noneで、未認証）</summary>
        public const string login_required = "login_required";

        /// <summary>consent_required（prompt=noneで、同意が無い）</summary>
        public const string consent_required = "consent_required";

        /// <summary>interaction_required（prompt=noneで、対話が必要）</summary>
        public const string interaction_required = "interaction_required";

        /// <summary>account_selection_required（prompt=noneで、アカウントの選択が必要）</summary>
        public const string account_selection_required = "account_selection_required";
        #endregion

        #region CIBA Core
        /// <summary>unknown_user_id（login_hintなどのユーザが見つからない）</summary>
        public const string unknown_user_id = "unknown_user_id";
        #endregion

        #region RFC 9449（DPoP）
        /// <summary>invalid_dpop_proof（DPoP の証明が使えない。RFC 9449 5）</summary>
        public const string invalid_dpop_proof = "invalid_dpop_proof";

        /// <summary>use_dpop_nonce（サーバの nonce を入れて出し直す。RFC 9449 8 / 9）</summary>
        public const string use_dpop_nonce = "use_dpop_nonce";
        #endregion

        #region RFC 7591（DCR）
        /// <summary>invalid_redirect_uri（RFC 7591 3.2.2）</summary>
        public const string invalid_redirect_uri = "invalid_redirect_uri";

        /// <summary>invalid_client_metadata（RFC 7591 3.2.2）</summary>
        public const string invalid_client_metadata = "invalid_client_metadata";

        /// <summary>invalid_software_statement（RFC 7591 3.2.2）</summary>
        public const string invalid_software_statement = "invalid_software_statement";

        /// <summary>unapproved_software_statement（RFC 7591 3.2.2）</summary>
        public const string unapproved_software_statement = "unapproved_software_statement";
        #endregion

        #endregion

        // urnは無し。

        #endregion
    }
}
