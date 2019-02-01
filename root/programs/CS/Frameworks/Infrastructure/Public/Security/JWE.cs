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
//* クラス名        ：JWE
//* クラス日本語名  ：JWE抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/01/29  西野 大介         新規作成
//**********************************************************************************

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>JWE</summary>
    public abstract class JWE
    {
        #region mem & prop & constructor

        /// <summary>_JWEHeader</summary>
        protected JWE_Header JWEHeader = null;

        /// <summary>CekByteLength</summary>
        protected int CekByteLength = 0;

        /// <summary>IvByteLength</summary>
        protected int IvByteLength = 0;

        /// <summary>ASymmetricCryptography</summary>
        public ASymmetricCryptography ASymmetricCryptography { get; protected set; }

        #endregion

        #region JWE 生成・復号

        /// <summary>JWE生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWEの文字列表現</returns>
        public string Create(string payloadJson)
        {
            // ヘッダー
            string headerJson = JsonConvert.SerializeObject(
                this.JWEHeader,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

            byte[] headerBytes = CustomEncode.StringToByte(headerJson, CustomEncode.UTF_8);
            string headerEncoded = CustomEncode.ToBase64UrlString(headerBytes);

            // コンテンツ暗号化キー（CEK）
            byte[] cekBytes = GetPassword.RandomByte(this.CekByteLength); 
            byte[] encryptedCekBytes = this.CreateKey(cekBytes); // 派生を呼ぶ
            string encryptedCekEncoded = CustomEncode.ToBase64UrlString(encryptedCekBytes);

            // 初期化ベクトル
            byte[] ivBytes = GetPassword.RandomByte(this.IvByteLength); 
            string ivEncoded = CustomEncode.ToBase64UrlString(ivBytes);

            // 追加認証データ（AAD）
            byte[] aadBytes = CustomEncode.StringToByte(headerEncoded, CustomEncode.us_ascii);

            // ペイロード（認証付き暗号（AEAD）による暗号化）
            byte[] payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            AeadResult result = this.CreateBody(cekBytes, ivBytes, aadBytes, payloadBytes); // 派生を呼ぶ
            byte[] encryptedPayloadBytes = result.Ciphert;
            string encryptedPayloadEncoded = CustomEncode.ToBase64UrlString(encryptedPayloadBytes);

            // 認証タグ（MAC）
            byte[] macBytes = result.Tag;
            string macEncoded = CustomEncode.ToBase64UrlString(macBytes);

            // return JWE
            return headerEncoded + "." +
                encryptedCekEncoded + "." + ivEncoded + "." +
                encryptedPayloadEncoded + "." + macEncoded;
        }

        /// <summary>JWE復号メソッド</summary>
        /// <param name="jwtString">JWEの文字列表現</param>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>復号の結果</returns>
        public bool Decrypt(string jwtString, out string payloadJson)
        {
            try
            {
                string[] temp = jwtString.Split('.');

                // 検証
                JWE_Header headerObject = (JWE_Header)JsonConvert.DeserializeObject(
                    CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWE_Header));

                if (headerObject.alg.ToUpper() == JWEHeader.alg.ToUpper() &&
                    headerObject.enc.ToUpper() == JWEHeader.enc.ToUpper() &&
                    headerObject.typ.ToUpper() == JWEHeader.typ.ToUpper())
                {
                    // コンテンツ暗号化キー（CEK）
                    byte[] encryptedCekBytes = CustomEncode.FromBase64UrlString(temp[1]);
                    byte[] cekBytes = this.DecryptKey(encryptedCekBytes); // 派生を呼ぶ

                    // 初期化ベクトル
                    byte[] ivBytes = CustomEncode.FromBase64UrlString(temp[2]);

                    // 追加認証データ（AAD）
                    byte[] aadBytes = CustomEncode.StringToByte(temp[0], CustomEncode.us_ascii);

                    // ペイロード（認証付き暗号（AEAD）による暗号化）
                    byte[] encryptedPayloadBytes = CustomEncode.FromBase64UrlString(temp[3]);

                    // 認証タグ（MAC）
                    byte[] macBytes = CustomEncode.FromBase64UrlString(temp[4]);

                    // 復号化
                    byte[] payloadBytes = this.DecryptBody( // 派生を呼ぶ
                        cekBytes, ivBytes, aadBytes, new AeadResult()
                        {
                            Ciphert = encryptedPayloadBytes,
                            Tag = macBytes
                        });

                    payloadJson = CustomEncode.ByteToString(payloadBytes, CustomEncode.UTF_8);
                    return true;
                }
                else
                {
                    payloadJson = "";
                    return false;
                }
            }
            catch
            {
                payloadJson = "";
                return false;
            }
        }

        #endregion

        #region 暗号化・復号化

        #region CEK 暗号化・復号化
        /// <summary>CEK 暗号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected abstract byte[] CreateKey(byte[] data);

        /// <summary>CEK 復号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[] </returns>
        protected abstract byte[] DecryptKey(byte[] data);
        #endregion

        #region 本文 暗号化・復号化
        /// <summary>認証付き暗号（AEAD）による本文 暗号化</summary>
        /// <param name="cekBytes">コンテンツ暗号化キー（CEK）</param>
        /// <param name="ivBytes">初期化ベクトル</param>
        /// <param name="aadBytes">追加認証データ（AAD）</param>
        /// <param name="payloadBytes">ペイロード</param>
        /// <returns>AeadResult</returns>
        protected abstract AeadResult CreateBody(byte[] cekBytes, byte[] ivBytes, byte[] aadBytes, byte[] payloadBytes);

        /// <summary>認証付き暗号（AEAD）による本文 復号化</summary>
        /// <param name="cekBytes"></param>
        /// <param name="ivBytes"></param>
        /// <param name="aadBytes"></param>
        /// <param name="aeadResult"></param>
        /// <returns>byte[] </returns>
        protected abstract byte[] DecryptBody(byte[] cekBytes, byte[] ivBytes, byte[] aadBytes, AeadResult aeadResult);
        #endregion

        #endregion
    }
}
