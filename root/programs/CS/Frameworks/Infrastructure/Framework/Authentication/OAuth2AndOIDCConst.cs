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

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2とOIDCの各種定数</summary>
    public class OAuth2AndOIDCConst
    {
        #region GrantType

        /// <summary>Authorization Codeグラント種別</summary>
        public const string AuthorizationCodeGrantType = "authorization_code";

        /// <summary>Implicitグラント種別</summary>
        public const string ImplicitGrantType = "-"; // Implicitには無い。

        /// <summary>Resource Owner Password Credentialsグラント種別</summary>
        public const string ResourceOwnerPasswordCredentialsGrantType = "password";

        /// <summary>Client Credentialsグラント種別</summary>
        public const string ClientCredentialsGrantType = "client_credentials";

        /// <summary>Refresh Tokenグラント種別</summary>
        public const string RefreshTokenGrantType = "refresh_token";

        /// <summary>JWT bearer token authorizationグラント種別</summary>
        public const string JwtBearerTokenFlowGrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer";

        #endregion

        #region ResponseType

        /// <summary>Authorization Codeグラント種別</summary>
        public const string AuthorizationCodeResponseType = "code";

        /// <summary>Implicitグラント種別</summary>
        public const string ImplicitResponseType = "token";

        /// <summary>OIDC - Implicit</summary>
        public const string OidcImplicit1_ResponseType = "id_token";

        /// <summary>OIDC - Implicit2</summary>
        public const string OidcImplicit2_ResponseType = "id_token token";

        /// <summary>OIDC - Hybrid(IdToken)</summary>
        public const string OidcHybrid2_IdToken_ResponseType = "code id_token";

        /// <summary>OIDC - Hybrid(Token)</summary>
        public const string OidcHybrid2_Token_ResponseType = "code token";

        /// <summary>OIDC - Hybrid(IdToken and Token)</summary>
        public const string OidcHybrid3_ResponseType = "code id_token token";

        #endregion

        #region Claimのurn

        /// <summary>ベース部分</summary>
        public static readonly string Claim_Base = "urn:oauth:";

        #region 標準Claim

        /// <summary>issuerクレームのurn</summary>
        public static readonly string Claim_Issuer = Claim_Base + "iss";

        /// <summary>audienceクレームのurn</summary>
        public static readonly string Claim_Audience = Claim_Base + "aud";

        /// <summary>scopeクレームのurn</summary>
        public static readonly string Claim_Scope = Claim_Base + "scope";

        #endregion

        #region 拡張Claim

        #region JWT

        /// <summary>expクレームのurn</summary>
        public static readonly string Claim_ExpirationTime = Claim_Base + "exp";

        /// <summary>nbfクレームのurn</summary>
        public static readonly string Claim_NotBefore = Claim_Base + "nbf";

        /// <summary>iatクレームのurn</summary>
        public static readonly string Claim_IssuedAt = Claim_Base + "iat";

        /// <summary>jtiクレームのurn</summary>
        public static readonly string Claim_JwtId = Claim_Base + "jti";

        #region OIDC

        /// <summary>nonceクレームのurn</summary>
        public static readonly string Claim_Nonce = Claim_Base + "nonce";

        #endregion

        #endregion

        #endregion

        #endregion

        #region Scope（id_token用）
        
        /// <summary>id_tokenを要求するscope</summary>
        public const string Scope_Openid = "openid";

        /// <summary>profileを要求するscope</summary>
        public const string Scope_Profile = "profile";

        /// <summary>emailを要求するscope</summary>
        public const string Scope_Email = "email";

        /// <summary>phoneを要求するscope</summary>
        public const string Scope_Phone = "phone";

        /// <summary>addressを要求するscope</summary>
        public const string Scope_Address = "address";
        
        #endregion
        
    }
}
