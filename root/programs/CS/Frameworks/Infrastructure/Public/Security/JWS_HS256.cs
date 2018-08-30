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
//* クラス名        ：JWS_HS256
//* クラス日本語名  ：HS256 JWS生成
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/13  西野 大介         新規作成
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2017/12/25  西野 大介         暗号化ライブラリ追加に伴うコード追加・修正
//*  2018/08/15  西野 大介         jwks_uri & kid 対応
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>HS256 JWS生成クラス</summary>
    public class JWS_HS256 : JWS
    {
        #region mem & prop & constructor
        
        /// <summary>キー</summary>
        public byte[] Key { get; protected set; }

        /// <summary>検証用JWK</summary>
        /// <remarks>https://mkjwk.org</remarks>
        public string JWK { get; protected set; }

        /// <summary>Constructor</summary>
        /// <param name="key">byte[]</param>
        public JWS_HS256(byte[] key)
        {
            this.Key = key;

            // mkjwk - JSON Web Key Generator
            // https://mkjwk.org
            // JWK - マイクロソフト系技術情報 Wiki > 詳細 > パラメタ(JWA) > Parameters for Symmetric Keys
            // https://techinfoofmicrosofttech.osscons.jp/index.php?JWK#r5be7fb8

            this.JWK = "{ 'kty': 'oct', 'use': 'sig', 'alg': 'HS256', 'k': 'password' }";
            this.JWK = this.JWK.Replace("password", CustomEncode.ToBase64UrlString(key));
        }

        /// <summary>Constructor</summary>
        /// <param name="jwkString">string</param>
        public JWS_HS256(string jwkString)
        {
            Dictionary<string, string> jwk = new Dictionary<string, string>();
            jwk = JsonConvert.DeserializeObject<Dictionary<string, string>>(jwkString);

            if(jwk.ContainsKey(JwtConst.kty)
                && jwk.ContainsKey(JwtConst.use)
                && jwk.ContainsKey(JwtConst.alg)
                && jwk.ContainsKey(JwtConst.k))
            {
                // 正しいキー
                if (jwk[JwtConst.kty].ToLower() == "oct"
                && jwk[JwtConst.use].ToLower() == "sig"
                && jwk[JwtConst.alg].ToUpper() == JwtConst.HS256
                && !string.IsNullOrEmpty(jwk[JwtConst.k]))
                {
                    // 正しい値
                    this.JWK = jwkString;
                    this.Key = CustomEncode.FromBase64UrlString(jwk[JwtConst.k]);
                    return; // 正常終了
                }
                else { }
            }
            else { }

            // 異常終了
            throw new ArgumentException(
                PublicExceptionMessage.ARGUMENT_INCORRECT,
                "The JWK of HS256 is incorrect.");
        }

        #endregion

        #region HS256署名・検証

        /// <summary>HS256のJWS生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWSの文字列表現</returns>
        public override string Create(string payloadJson)
        {
            // ヘッダー
            JWS_Header headerObject = new JWS_Header { alg = JwtConst.HS256 };

            string headerJson = JsonConvert.SerializeObject(
                headerObject,
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
            byte[] data = CustomEncode.StringToByte(headerEncoded + "." + payloadEncoded, CustomEncode.UTF_8);
            HMACSHA256 sa = new HMACSHA256(this.Key);
            string signEncoded = CustomEncode.ToBase64UrlString(sa.ComputeHash(data));

            // return JWS by RS256
            return headerEncoded + "." + payloadEncoded + "." + signEncoded;
        }

        /// <summary>RS256のJWS検証メソッド</summary>
        /// <param name="jwtString">JWSの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        public override bool Verify(string jwtString)
        {
            string[] temp = jwtString.Split('.');

            // 検証
            JWS_Header headerObject = (JWS_Header)JsonConvert.DeserializeObject(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWS_Header));

            if (headerObject.alg.ToUpper() == JwtConst.HS256 && headerObject.typ.ToUpper() == JwtConst.JWT)
            {
                byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);

                HMACSHA256 sa = new HMACSHA256(this.Key);

                return (CustomEncode.ToBase64UrlString(sign) 
                    == CustomEncode.ToBase64UrlString(sa.ComputeHash(data)));
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
