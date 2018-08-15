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
//* クラス名        ：JWT_RS256_XML
//* クラス日本語名  ：ParamによるJWT(JWS)RS256生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//*  2018/08/15  西野 大介         jwks_uri & kid 対応
//**********************************************************************************

using System.Security.Cryptography;

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>ParamによるJWT(JWS)RS256生成クラス</summary>
    public class JWT_RS256_Param : JWT_RS256
    {
        #region mem & prop & constructor

        /// <summary>DigitalSignParam</summary>
        private DigitalSignParam DigitalSignParam { get; set; }

        /// <summary>秘密鍵のRSAParameters</summary>
        public RSAParameters RsaPrivateParameters
        {
            get
            {
                return ((RSACryptoServiceProvider)this.DigitalSignParam.AsymmetricAlgorithm).ExportParameters(true);
            }
        }

        /// <summary>公開鍵のRSAParameters</summary>
        public RSAParameters RsaPublicParameters
        {
            get
            {
                return ((RSACryptoServiceProvider)this.DigitalSignParam.AsymmetricAlgorithm).ExportParameters(false);
            }
        }

        /// <summary>Constructor</summary>
        public JWT_RS256_Param()
        {
            this.DigitalSignParam = new DigitalSignParam(EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256);
        }

        /// <summary>Constructor</summary>
        /// <param name="param">object</param>
        public JWT_RS256_Param(object param)
        {
            this.DigitalSignParam = new DigitalSignParam(param, SHA256.Create());
        }

        #endregion

        #region RS256署名・検証

        /// <summary>RS256のJWT生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWTの文字列表現</returns>
        public override string Create(string payloadJson)
        {
            // ヘッダー
            string headerJson = JsonConvert.SerializeObject(
                this.JWSHeader,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

            byte[] headerBytes = CustomEncode.StringToByte(headerJson, CustomEncode.UTF_8);
            string headerEncoded = CustomEncode.ToBase64UrlString(headerBytes);

            // ペイロード
            byte[] payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            string payloadEncoded = CustomEncode.ToBase64UrlString(payloadBytes);

            // 署名
            byte[] temp = CustomEncode.StringToByte(headerEncoded + "." + payloadEncoded, CustomEncode.UTF_8);
            string signEncoded = CustomEncode.ToBase64UrlString(this.DigitalSignParam.Sign(temp));

            // return JWT by RS256
            return headerEncoded + "." + payloadEncoded + "." + signEncoded;
        }

        /// <summary>RS256のJWT検証メソッド</summary>
        /// <param name="jwtString">JWTの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        public override bool Verify(string jwtString)
        {
            string[] temp = jwtString.Split('.');

            // 検証
            JWSHeader headerObject = (JWSHeader)JsonConvert.DeserializeObject(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWSHeader));

            if (headerObject.alg == "RS256" && headerObject.typ == "JWT")
            {
                byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);
                return this.DigitalSignParam.Verify(data, sign);
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
