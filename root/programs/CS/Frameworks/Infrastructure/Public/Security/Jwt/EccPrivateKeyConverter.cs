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
//* クラス名        ：EccPrivateKeyConverter
//* クラス日本語名  ：EccPrivateKeyConverterクラス
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
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//using Jose;
//using Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>
    /// ECC関係のカギ変換処理を実装する。
    /// 秘密鍵用のミニマム実装
    /// </summary>
    public class EccPrivateKeyConverter : EccKeyConverter
    {
        #region constructor
        /// <summary>constructor</summary>
        /// <param name="esNNN">JWS_ECDSA.ES</param>
        public EccPrivateKeyConverter(JWS_ECDSA.ES esNNN = JWS_ECDSA.ES._256) : base(esNNN) { }
        #endregion

        #region method
#if NET45 || NET46
#else
        #region ParamToJwk
        /// <summary>ParamToJwk</summary>
        /// <param name="ecParams">ECParameters</param>
        /// <returns>Jwk公開鍵</returns>
        public string ParamToJwk(ECParameters ecParams)
        {
            return this.ParamToJwk(ecParams, null);
        }

        /// <summary>ParamToJwk</summary>
        /// <param name="ecParams">ECParameters</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public string ParamToJwk(ECParameters ecParams, JsonSerializerSettings settings)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic[JwtConst.kty] = JwtConst.EC; // 必須
            dic[JwtConst.alg] = this.JwtConstESnnn;

            // 楕円曲線
            dic[JwtConst.crv] = this.GetCrvStringFromECCurve(ecParams.Curve);

            // Public
            dic[JwtConst.x] = CustomEncode.ToBase64UrlString(ecParams.Q.X);
            dic[JwtConst.y] = CustomEncode.ToBase64UrlString(ecParams.Q.Y);

            // Private
            dic[JwtConst.d] = CustomEncode.ToBase64UrlString(ecParams.D);

            return this.CreateJwkFromDictionary(dic, settings);
        }
        #endregion

        #region JwkToParam
        /// <summary>JwkToParam</summary>
        /// <param name="jwkString">string</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public ECParameters JwkToParam(string jwkString)
        {
            return this.JwkToParam(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(jwkString));
        }

        /// <summary>JwkToParam</summary>
        /// <param name="jwk">JObject</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public ECParameters JwkToParam(Dictionary<string, string> jwk)
        {
            ECParameters ecParams = new ECParameters();

            // 楕円曲線
            ecParams.Curve = this.ECCurveDic[(string)jwk[JwtConst.crv]];

            // Public
            ecParams.Q.X = CustomEncode.FromBase64UrlString((string)jwk[JwtConst.x]);
            ecParams.Q.Y = CustomEncode.FromBase64UrlString((string)jwk[JwtConst.y]);
            // Private
            ecParams.D = CustomEncode.FromBase64UrlString((string)jwk[JwtConst.d]);

            return ecParams;
        }
        #endregion
#endif
        #endregion
    }
}
