//**********************************************************************************
//* Copyright (C) 2007,2017 Hitachi Solutions,Ltd.
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
//* クラス名        ：JWT_HS256
//* クラス日本語名  ：JWT_HS256クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/13  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;
using Newtonsoft.Json;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Util.JWT
{
    /// <summary>JWT_HS256</summary>
    public class JWT_HS256 : JWT
    {
        #region mem & prop & constructor

        /// <summary>キー</summary>
        private byte[] _key = null;

        /// <summary>検証用JWK</summary>
        public string JWK = "{'k': 'password'}";

        /// <summary>Constructor</summary>
        public JWT_HS256(byte[] key)
        {
            this._key = key;
            this.JWK = this.JWK.Replace("password", CustomEncode.ToBase64UrlString(key));
        }

        #endregion

        #region HS256署名・検証

        /// <summary>HS256のJWT生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWTの文字列表現</returns>
        public override string Create(string payloadJson)
        {
            // ヘッダー
            var headerObject = new Header { alg = "HS256" };
            string headerJson = JsonConvert.SerializeObject(headerObject, Formatting.None);
            byte[] headerBytes = CustomEncode.StringToByte(headerJson, CustomEncode.UTF_8);
            string headerEncoded = CustomEncode.ToBase64UrlString(headerBytes);

            // ペイロード
            var payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            var payloadEncoded = CustomEncode.ToBase64UrlString(payloadBytes);

            // 署名
            byte[] data = CustomEncode.StringToByte(headerEncoded + "." + payloadEncoded, CustomEncode.UTF_8);
            HMACSHA256 sa = new HMACSHA256(this._key);
            string signEncoded = CustomEncode.ToBase64UrlString(sa.ComputeHash(data));

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
            Header headerObject = (Header)JsonConvert.DeserializeObject(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(Header));

            if (headerObject.alg == "HS256" && headerObject.typ == "JWT")
            {
                byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);

                HMACSHA256 sa = new HMACSHA256(this._key);

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
