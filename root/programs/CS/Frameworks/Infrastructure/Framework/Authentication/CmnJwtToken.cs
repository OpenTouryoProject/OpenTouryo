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
//* クラス名        ：CmnJwtToken
//* クラス日本語名  ：CmnJwtTokenクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/28  西野 大介         新規作成（分割）
//**********************************************************************************

using System;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OAuth2やOIDC関連のJwtToken処理
    /// </summary>
    public class CmnJwtToken
    {
        /// <summary>汎用認証サイトの発行したJWT形式のTokenを検証する。</summary>
        /// <param name="jwtToken">
        /// JWT形式のTokenで以下の項目が必要
        ///  - iss
        ///  - aud
        ///  - iat
        ///  - exp
        ///  - sub
        ///  - roles  (option)
        ///  - scopes (option)
        ///  - その他 (option)
        /// </param>
        /// <param name="jwtPayload">
        /// JWS, JWS + JEWの場合があるのでペイロードを返す。
        /// </param>
        /// <returns>検証結果</returns>
        public static bool Verify(string jwtToken, out string jwtPayload)
        {
            jwtPayload = "";

            JWE jwe = null;
            JWS jws = null;

            // 復号化
            bool isFAPI2 = false;
            if (3 < jwtToken.Split('.').Length)
            {
                isFAPI2 = true;

                // ヘッダ
                JWE_Header jweHeader = JsonConvert.DeserializeObject<JWE_Header>(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwtToken.Split('.')[0]), CustomEncode.UTF_8));

                if (jweHeader.alg == JwtConst.RSA_OAEP)
                {
                    jwe = new JWE_RsaOaepAesGcm_X509(
                        OAuth2AndOIDCParams.RS256Pfx,
                        OAuth2AndOIDCParams.RS256Pwd);
                }
                else if (jweHeader.alg == JwtConst.RSA1_5)
                {
                    jwe = new JWE_Rsa15A128CbcHS256_X509(
                        OAuth2AndOIDCParams.RS256Pfx,
                        OAuth2AndOIDCParams.RS256Pwd);
                }
                else
                {
                    throw new NotSupportedException(string.Format(
                        "This jwe alg of {0} is not supported.", jweHeader.alg));
                }

                jwe.Decrypt(jwtToken, out jwtToken);
            }
            else
            {
                isFAPI2 = false;
            }

            // 検証
            // ヘッダ
            JWS_Header jwsHeader = JsonConvert.DeserializeObject<JWS_Header>(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwtToken.Split('.')[0]), CustomEncode.UTF_8));

            if (jwsHeader.alg == JwtConst.ES256 && isFAPI2) { } // 正常
            else if (jwsHeader.alg == JwtConst.RS256 && !isFAPI2) { } // 正常
            else { throw new NotSupportedException("Unexpected combination of JWS and JWE."); }

            // 証明書を使用するか、Jwkを使用するか判定
            if (string.IsNullOrEmpty(jwsHeader.jku)
                || string.IsNullOrEmpty(jwsHeader.kid))
            {
                // 旧バージョン
                // 証明書を使用
                if (isFAPI2)
                {
#if NET45 || NET46
                    throw new NotSupportedException("FAPI2 is not supported in this dotnet version.");
#else
                    jws = new JWS_ES256_X509(OAuth2AndOIDCParams.ES256Cer, "");
#endif
                }
                else
                {
                    jws = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
                }
            }
            else
            {
                // 新バージョン

                // Client or AuthZ(検証用
                if (string.IsNullOrEmpty(OAuth2AndOIDCParams.JwkSetFilePath))
                {
                    // Client側
                    JObject jwkObject = JwkSetStore.GetInstance().GetJwkObject(jwsHeader.kid);

                    // チェック
                    if (jwkObject == null)
                    {
                        // 書込
                        jwkObject = JwkSetStore.GetInstance().SetJwkSetObject(jwsHeader.jku, jwsHeader.kid);
                    }

                    // チェック
                    if (jwkObject == null)
                    {
                        // 証明書を使用
                        if (isFAPI2)
                        {
#if NET45 || NET46
                            throw new NotSupportedException("FAPI2 is not supported in this dotnet version.");
#else
                            jws = new JWS_ES256_X509(OAuth2AndOIDCParams.ES256Cer, "");
#endif
                        }
                        else
                        {
                            jws = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
                        }
                    }
                    else
                    {
                        // Jwkを使用
                        if (isFAPI2)
                        {
#if NET45 || NET46
                            throw new NotSupportedException("FAPI2 is not supported in this dotnet version.");
#else
                            jws = new JWS_ES256_Param(EccPublicKeyConverter.JwkToParam(jwkObject), false);
#endif
                        }
                        else
                        {
                            jws = new JWS_RS256_Param(RsaPublicKeyConverter.JwkToParam(jwkObject));
                        }
                    }
                }
                else
                {
                    // AuthZ側(検証用カバレッジ
                    // ※ AuthZ側でClient側テストを行うためのカバレージ
                    JObject jwkObject = null;

                    if (ResourceLoader.Exists(OAuth2AndOIDCParams.JwkSetFilePath, false))
                    {
                        JwkSet jwkSet = JwkSet.LoadJwkSet(OAuth2AndOIDCParams.JwkSetFilePath);
                        jwkObject = JwkSet.GetJwkObject(jwkSet, jwsHeader.kid);
                    }

                    if (jwkObject == null)
                    {
                        // 証明書を使用
                        if (isFAPI2)
                        {
#if NET45 || NET46
                            throw new NotSupportedException("FAPI2 is not supported in this dotnet version.");
#else
                            jws = new JWS_ES256_X509(OAuth2AndOIDCParams.ES256Cer, "");
#endif
                        }
                        else
                        {
                            jws = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
                        }
                    }
                    else
                    {
                        // Jwkを使用
                        if (isFAPI2)
                        {
#if NET45 || NET46
                            throw new NotSupportedException("FAPI2 is not supported in this dotnet version.");
#else
                            jws = new JWS_ES256_Param(EccPublicKeyConverter.JwkToParam(jwkObject), false);
#endif
                        }
                        else
                        {
                            jws = new JWS_RS256_Param(RsaPublicKeyConverter.JwkToParam(jwkObject));
                        }
                    }
                }
            }

            bool ret = jws.Verify(jwtToken);

            if (ret)
            {
                jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(jwtToken.Split('.')[1]), CustomEncode.us_ascii);
            }

            return ret;
        }
    }
}
