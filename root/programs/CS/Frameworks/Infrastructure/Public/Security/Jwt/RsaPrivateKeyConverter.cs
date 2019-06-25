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
//* クラス名        ：RsaPrivateKeyConverter
//* クラス日本語名  ：RsaPrivateKeyConverterクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/27  西野 大介         新規作成（秘密鍵 <---> JWKサポート追加
//*  2019/06/25  西野 大介         新規作成（分割
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>
    /// RSA関係のカギ変換処理を実装する。
    /// 秘密鍵用のミニマム実装
    /// </summary>
    public class RsaPrivateKeyConverter : RsaKeyConverter
    {
        #region constructor
        /// <summary>constructor</summary>
        /// <param name="rsNNN">JWS_RSA.RS</param>
        public RsaPrivateKeyConverter(JWS_RSA.RS rsNNN = JWS_RSA.RS._256) : base(rsNNN) { }
        #endregion

        #region method
        #region ParamToJwk
        /// <summary>ParamToJwk</summary>
        /// <param name="rsaParameters">RSAParameters</param>
        /// <returns>Jwk公開鍵</returns>
        public string ParamToJwk(RSAParameters rsaParameters)
        {
            return this.ParamToJwk(rsaParameters, null);
        }

        /// <summary>ParamToJwk</summary>
        /// <param name="rsaParameters">RSAParameters</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public string ParamToJwk(RSAParameters rsaParameters, JsonSerializerSettings settings)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic[JwtConst.kty] = JwtConst.RSA; // 必須
            dic[JwtConst.alg] = this.JwtConstRSnnn;

            // Public
            dic[JwtConst.n] = CustomEncode.ToBase64UrlString(rsaParameters.Modulus);
            dic[JwtConst.e] = CustomEncode.ToBase64UrlString(rsaParameters.Exponent); //"AQAB";

            // Private
            dic[JwtConst.d] = CustomEncode.ToBase64UrlString(rsaParameters.D);
            dic[JwtConst.p] = CustomEncode.ToBase64UrlString(rsaParameters.P);
            dic[JwtConst.q] = CustomEncode.ToBase64UrlString(rsaParameters.Q);
            dic[JwtConst.dp] = CustomEncode.ToBase64UrlString(rsaParameters.DP);
            dic[JwtConst.dq] = CustomEncode.ToBase64UrlString(rsaParameters.DQ);
            dic[JwtConst.qi] = CustomEncode.ToBase64UrlString(rsaParameters.InverseQ);

            // JSON Web Key (JWK) Thumbprint
            // https://openid-foundation-japan.github.io/rfc7638.ja.html
            // kid : https://openid-foundation-japan.github.io/rfc7638.ja.html#Private

            // JwkSetに格納しないので、今のところ、Thumbprintは取らない。

            //dic["ext"] = "false"; // 定義をRFC上に発見できない。

            if (settings == null)
            {
                return JsonConvert.SerializeObject(dic);
            }
            else
            {
                return JsonConvert.SerializeObject(dic, settings);
            }
        }
        #endregion

        #region JwkToParam
        /// <summary>JwkToParam</summary>
        /// <param name="jwkString">string</param>
        /// <returns>RSAParameters（公開鍵）</returns>
        public RSAParameters JwkToParam(string jwkString)
        {
            return this.JwkToParam(
                JsonConvert.DeserializeObject<JObject>(jwkString));
        }

        /// <summary>JwkToParam</summary>
        /// <param name="jwkObject">JObject</param>
        /// <returns>RSAParameters（公開鍵）</returns>
        public RSAParameters JwkToParam(JObject jwkObject)
        {
            if (jwkObject[JwtConst.kty].ToString().ToUpper() == JwtConst.RSA)
            {
                // RSAParameters
                // FromBase64Stringだとエラーになる。
                RSAParameters rsaParameters = new RSAParameters()
                {
                    // Public
                    Modulus = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.n]),
                    Exponent = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.e]),

                    // Private
                    D = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.d]),
                    P = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.p]),
                    Q = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.q]),
                    DP = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.dp]),
                    DQ = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.dq]),
                    InverseQ = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.qi])
                };

                return rsaParameters;
            }

            throw new ArgumentOutOfRangeException("jwkObject", jwkObject, "Invalid");
        }
        #endregion
        #endregion
    }
}
