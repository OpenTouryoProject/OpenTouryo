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
        /// <returns>検証結果</returns>
        public static bool Verify(string jwtToken)
        {
            // 検証
            JWS_RS256 jwsRS256 = null;

            // 証明書を使用するか、Jwkを使用するか判定
            JWS_Header jwsHeader = JsonConvert.DeserializeObject<JWS_Header>(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwtToken.Split('.')[0]), CustomEncode.UTF_8));

            if (string.IsNullOrEmpty(jwsHeader.jku)
                || string.IsNullOrEmpty(jwsHeader.kid))
            {
                // 旧バージョン
                // 証明書を使用
                jwsRS256 = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
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
                        jwsRS256 = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
                    }
                    else
                    {
                        // Jwkを使用
                        jwsRS256 = new JWS_RS256_Param(
                            RsaPublicKeyConverter.JwkToParam(jwkObject));
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
                        jwsRS256 = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
                    }
                    else
                    {
                        // Jwkを使用
                        jwsRS256 = new JWS_RS256_Param(
                            RsaPublicKeyConverter.JwkToParam(jwkObject));
                    }
                }
            }

            // JWS検証
            return jwsRS256.Verify(jwtToken);
        }
    }
}
