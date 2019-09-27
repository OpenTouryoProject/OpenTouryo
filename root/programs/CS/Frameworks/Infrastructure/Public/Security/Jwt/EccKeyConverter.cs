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
//* クラス名        ：EccKeyConverter
//* クラス日本語名  ：EccKeyConverterクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
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
    /// <summary>ECC関係のカギ変換処理を実装する。</summary>
    public abstract class EccKeyConverter
    {
        #region mem & prop
        /// <summary>アルゴリズム</summary>
        protected JWS_ECDSA.ES ESnnn = JWS_ECDSA.ES._256;
        /// <summary>アルゴリズム</summary>
        protected string JwtConstESnnn = JwtConst.ES256;
        /// <summary>アルゴリズム</summary>
        protected HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        /// <summary>アルゴリズム</summary>
        protected EnumHashAlgorithm HashAlgorithm = EnumHashAlgorithm.SHA256_M;

        /// <summary>ECCurveDic</summary>
        protected Dictionary<string, ECCurve> ECCurveDic = new Dictionary<string, ECCurve>()
        {
            { JwtConst.P256 , ECCurve.NamedCurves.nistP256 },
            { JwtConst.P384 , ECCurve.NamedCurves.nistP384 },
            { JwtConst.P521 , ECCurve.NamedCurves.nistP521 },
        };
        #endregion

        #region constructor
        /// <summary>constructor</summary>
        /// <param name="esNNN">JWS_ECDSA.ES</param>
        public EccKeyConverter(JWS_ECDSA.ES esNNN)
        {
            this.ESnnn = esNNN;

            switch (this.ESnnn)
            {
                case JWS_ECDSA.ES._256:
                    this.JwtConstESnnn = JwtConst.ES256;
                    this._hashAlgorithmName = HashAlgorithmName.SHA256;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA256_M;
                    break;

                case JWS_ECDSA.ES._384:
                    this.JwtConstESnnn = JwtConst.ES384;
                    this._hashAlgorithmName = HashAlgorithmName.SHA384;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA384_M;
                    break;

                case JWS_ECDSA.ES._512:
                    this.JwtConstESnnn = JwtConst.ES512;
                    this._hashAlgorithmName = HashAlgorithmName.SHA512;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA512_M;
                    break;
            }
        }
        #endregion

        #region method
        #region ECCurve
        /// <summary>GetECCurveFromCrvString</summary>
        /// <param name="crvString">string</param>
        /// <returns>ECCurve</returns>
        public ECCurve GetECCurveFromCrvString(string crvString)
        {
            return this.ECCurveDic[crvString];
        }

        /// <summary>GetCrvStringFromECCurve</summary>
        /// <param name="ecc">ECCurve</param>
        /// <returns>crvの文字列値（P-nnn）</returns>
        public string GetCrvStringFromECCurve(ECCurve ecc)
        {
            foreach (string key in this.ECCurveDic.Keys)
            {
                ECCurve test = this.ECCurveDic[key];

                // 1. compare  ECCurve
                if (object.Equals(test, ecc)) return key;

                // 2. compare ECCurve.Oid
                if (test.Oid != null && ecc.Oid != null)
                {
                    if (object.Equals(test.Oid, ecc.Oid)) return key;
                }

                // 3. compare ECCurve.Oid.Value
                if (test.Oid.Value != null && ecc.Oid.Value != null)
                {
                    if (object.Equals(test.Oid.Value, ecc.Oid.Value)) return key;
                }

                // 4. compare ECCurve.Oid.FriendlyName
                if (test.Oid.FriendlyName != null && ecc.Oid.FriendlyName != null)
                {
                    if (object.Equals(test.Oid.FriendlyName, ecc.Oid.FriendlyName)) return key;
                }
            }

            throw new ArgumentOutOfRangeException("ecc", ecc, "Invalid");
        }

        /// <summary>GetCrvString</summary>
        /// <param name="x">byte[]</param>
        /// <returns>CrvString</returns>
        public string GetCrvStringFromXCoordinate(byte[] x)
        {
            int partSize = x.Length;

            if (partSize == 32)
            {
                return JwtConst.P256;
            }
            else if (partSize == 48)
            {
                return JwtConst.P384;
            }
            else if (partSize == 66)
            {
                return JwtConst.P521;
            }
            else
            {
                throw new ArgumentException("Size of X must equal to 32, 48 or 66 bytes");
            }
        }
        #endregion

        #region CreateJwkFromDictionary
        /// <summary>CreateJwkFromDictionary</summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>JwkString</returns>
        internal string CreateJwkFromDictionary(
            Dictionary<string, string> dic,
            JsonSerializerSettings settings = null)
        {
            // JSON Web Key (JWK) Thumbprint
            // https://openid-foundation-japan.github.io/rfc7638.ja.html
            // kid : https://openid-foundation-japan.github.io/rfc7638.ja.html#Example
            //       https://openid-foundation-japan.github.io/rfc7638.ja.html#MembersUsed
            //       kidには、JWK の JWK Thumbprint 値などが用いられるらしい。
            //       ★ EC 公開鍵の必須メンバを辞書順に並べると、crv, kty, x, y となる。
            dic[JwtConst.kid] = CustomEncode.ToBase64UrlString(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(
                        JsonConvert.SerializeObject(new
                        {
                            crv = dic[JwtConst.crv],
                            kty = dic[JwtConst.kty],
                            x = dic[JwtConst.x],
                            y = dic[JwtConst.y]
                        }),
                        CustomEncode.UTF_8),
                    this.HashAlgorithm));

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
        #endregion
    }
}