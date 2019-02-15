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
//**********************************************************************************

// urnはClaimのurnで、
// ASP.NETとClaimとJwtのMember間のインターフェイスを形成する。

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2とOIDCの各種定数</summary>
    public class OAuth2AndOIDCConst
    {
        #region param

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

        /// <summary>state</summary>
        public const string state = "state";

        /// <summary>code</summary>
        public const string code = "code";

        /// <summary>assertion</summary>
        public const string assertion = "assertion";
        
        /// <summary>token</summary>
        public const string token = "token";

        #region WebAPI

        /// <summary>token</summary>
        public const string token_type = "token_type";

        /// <summary>token_type_hint</summary>
        public const string token_type_hint = "token_type_hint";

        #endregion

        #endregion

        #region token

        /// <summary>AccessToken</summary>
        public const string AccessToken = "access_token";

        /// <summary>RefreshToken</summary>
        public const string RefreshToken = "refresh_token";

        /// <summary>IDToken</summary>
        public const string IDToken = "id_token";

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
        public const string Scope_Roles = "roles";

        #endregion

        #region id_token

        /// <summary>id_tokenを要求するscope</summary>
        public const string Scope_Openid = "openid";

        #endregion

        #endregion

        #region Claims

        // ★ Scopeと同じ文字列は定義しない。

        /// <summary>ベース部分</summary>
        public static readonly string Claim_Base = "urn:oauth:";

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
        public static readonly string Claim_Issuer = Claim_Base + iss;

        /// <summary>audienceクレームのurn</summary>
        public static readonly string Claim_Audience = Claim_Base + aud;

        /// <summary>subjectクレームのurn</summary>
        public static readonly string Claim_Subject = Claim_Base + sub;

        /// <summary>expクレームのurn</summary>
        public static readonly string Claim_ExpirationTime = Claim_Base + exp;

        /// <summary>nbfクレームのurn</summary>
        public static readonly string Claim_NotBefore = Claim_Base + nbf;

        /// <summary>iatクレームのurn</summary>
        public static readonly string Claim_IssuedAt = Claim_Base + iat;

        /// <summary>jtiクレームのurn</summary>
        public static readonly string Claim_JwtId = Claim_Base + jti;

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

        #endregion

        #region urn

        /// <summary>emailクレームのurn</summary>
        public static readonly string Claim_Email = Claim_Base + Scope_Email;

        /// <summary>email_verifiedクレームのurn</summary>
        public static readonly string Claim_EmailVerified = Claim_Base + email_verified;

        /// <summary>phone_numberクレームのurn</summary>
        public static readonly string Claim_PhoneNumber = Claim_Base + phone_number;

        /// <summary>phone_number_verifiedクレームのurn</summary>
        public static readonly string Claim_PhoneNumberVerified = Claim_Base + phone_number_verified;

        #endregion

        #endregion

        #region OIDC

        #region 末端

        /// <summary>nonce</summary>
        public const string nonce = "nonce";

        /// <summary>at_hash</summary>
        public const string at_hash = "at_hash";

        /// <summary>c_hash</summary>
        public const string c_hash = "c_hash";

        #endregion

        #region urn

        /// <summary>nonceクレームのurn</summary>
        public static readonly string Claim_Nonce = Claim_Base + nonce;

        /// <summary>at_hashクレームのurn</summary>
        public static readonly string Claim_AtHash = Claim_Base + at_hash;

        /// <summary>c_hashクレームのurn</summary>
        public static readonly string Claim_CHash = Claim_Base + c_hash;

        #endregion

        #endregion

        #region FAPI

        #region 末端
        
        /// <summary>s_hash</summary>
        public const string s_hash = "s_hash";

        /// <summary>cnf</summary>
        public const string cnf = "cnf";

        /// <summary>x5t</summary>
        public const string x5t = "x5t";

        /// <summary>x5u</summary>
        public const string x5u = "x5u";

        // 独自

        /// <summary>fapi</summary>
        public const string fapi = "fapi";

        #endregion

        #region urn

        /// <summary>s_hashクレームのurn</summary>
        public static readonly string Claim_SHash = Claim_Base + s_hash;

        /// <summary>cnfクレームのurn</summary>
        public static readonly string Claim_Cnf = Claim_Base + cnf + ":";

        /// <summary>x5tクレームのurn</summary>
        public static readonly string Claim_CnfX5t = Claim_Cnf + x5t;

        /// <summary>x5uクレームのurn</summary>
        public static readonly string Claim_CnfX5u = Claim_Cnf + x5u;

        // 独自

        /// <summary>fapiクレームのurn</summary>
        public static readonly string Claim_FApi = Claim_Base + fapi;

        #endregion

        #endregion

        #region 拡張

        #region 末端

        /// <summary>scopes</summary>
        public const string scopes = "scopes";

        #endregion

        #region urn

        /// <summary>scopeクレームのurn</summary>
        public static readonly string Claim_Scopes = Claim_Base + scopes;

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
    }
}
