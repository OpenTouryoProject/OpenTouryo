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
//* クラス名        ：JWS_RS256
//* クラス日本語名  ：JWS RS256生成クラス
//*
//*                  RFC 7515 - JSON Web Signature (JWS)
//*                  > A.2.  Example JWS Using RSASSA-PKCS1-v1_5 SHA-256
//*                  https://tools.ietf.org/html/rfc7515#appendix-A.2
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//*  2017/12/25  西野 大介         暗号化ライブラリ追加に伴うコード追加・修正
//*  2018/08/15  西野 大介         後方互換から、基底クラスに変更
//*                                （汎用認証サイトでしか使っていないと思われるため）
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//*  2019/01/29  西野 大介         リファクタリング（プロバイダ処理を末端に）
//**********************************************************************************

using System;

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWS RS256生成クラス</summary>
    public abstract class JWS_RS256 : JWS
    {
        #region mem & prop & constructor

        /// <summary>_JWSHeader</summary>
        private JWS_Header _JWSHeader = new JWS_Header
        {
            alg = JwtConst.RS256
        };

        /// <summary>JWSHeader</summary>
        public JWS_Header JWSHeader
        {
            protected set
            {
                this._JWSHeader = value;
            }

            get
            {
                return this._JWSHeader;
            }
        }

        /// <summary>EnumDigitalSignAlgorithm</summary>
        /// <remarks>Constructorで使うのでstaticとなった</remarks>
        public static EnumDigitalSignAlgorithm DigitalSignAlgorithm
        {
            get
            {
#if NETSTD
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    return EnumDigitalSignAlgorithm.RsaCSP_SHA256;
                }
                else
                {
                    return EnumDigitalSignAlgorithm.RsaOpenSsl_SHA256;
                }
#else
                return EnumDigitalSignAlgorithm.RsaCSP_SHA256;
#endif
            }
        }

        #endregion

        #region RS256署名・検証

        /// <summary>RS256のJWS生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWSの文字列表現</returns>
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
            string signEncoded = CustomEncode.ToBase64UrlString(this.Create2(temp)); // 派生を呼ぶ

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

            if (headerObject.alg.ToUpper() == JwtConst.RS256
                && headerObject.typ.ToUpper() == JwtConst.JWT)
            {
                byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);
                return this.Verify2(data, sign); // 派生を呼ぶ
            }
            else
            {
                return false;
            }
        }

        /// <summary>Create2</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected abstract byte[] Create2(byte[] data);

        /// <summary>Verify2</summary>
        /// <param name="data">byte[]</param>
        /// <param name="sign">byte[]</param>
        /// <returns>bool</returns>
        protected abstract bool Verify2(byte[] data, byte[] sign);

        #endregion
    }
}
