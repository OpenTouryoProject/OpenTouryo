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
//* クラス名        ：DigitalSignXML
//* クラス日本語名  ：DigitalSignXMLクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>
    /// DigitalSignXMLクラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    public class DigitalSignXML : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        /// <summary>AsymmetricAlgorithm</summary>
        private AsymmetricAlgorithm _aa = null;
        /// <summary>HashAlgorithm</summary>
        private HashAlgorithm _ha = null;

        /// <summary>
        /// XMLPrivateKey
        /// RFC 3275のXML秘密鍵
        /// </summary>
        public string XMLPrivateKey { get; set; }

        /// <summary>
        /// XMLPublicKey
        /// RFC 3275のXML公開鍵
        /// </summary>
        public string XMLPublicKey { get; set; }

        /// <summary>
        /// Constructor
        /// RFC 3275のXMLからキーペアを設定する。
        /// </summary>
        public DigitalSignXML(EnumDigitalSignAlgorithm eaa)
        {
            this.CreateAsymmetricAlgorithmServiceProvider(eaa, out this._aa, out this._ha);

            if (string.IsNullOrEmpty(this.XMLPrivateKey))
            {
                // 秘密鍵をXML形式で取得
                this.XMLPrivateKey = this._aa.ToXmlString(true);
                // 公開鍵をXML形式で取得
                this.XMLPublicKey = this._aa.ToXmlString(false);
            }
        }

        #endregion

        #region デジタル署名(XML)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
        {
            // ハッシュ
            byte[] hashedByte = this._ha.ComputeHash(data);
            // デジタル署名
            byte[] signedByte = null;

            if (this._aa is RSACryptoServiceProvider)
            {
                // RSAPKCS1SignatureFormatterオブジェクトを作成
                RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(this._aa);

                rsaFormatter.SetHashAlgorithm(this.GetHashAlgorithmName(this._ha));
                signedByte = rsaFormatter.CreateSignature(hashedByte);
            }
            else if (this._aa is DSACryptoServiceProvider)
            {
                // DSASignatureFormatterオブジェクトを作成
                DSASignatureFormatter dsaFormatter = new DSASignatureFormatter(this._aa);

                // デジタル署名を作成
                dsaFormatter.SetHashAlgorithm("SHA1");
                signedByte = dsaFormatter.CreateSignature(hashedByte);
            }

            return signedByte;
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            // 以下でイケるらしい。
            this._aa.FromXmlString(this.XMLPublicKey);

            if (this._aa is RSACryptoServiceProvider)
            {   
                return ((RSACryptoServiceProvider)this._aa).VerifyData(data, this.GetHashAlgorithmName(this._ha), sign);
            }
            else
            {
                return ((DSACryptoServiceProvider)this._aa).VerifyData(data, sign);
            }

            #region old
            //// ハッシュ
            //byte[] hashedByte = this._ha.ComputeHash(data);

            //// 結果フラグ
            //bool flg = false;

            //if (this._aa is RSACryptoServiceProvider)
            //{
            //    // 公開鍵
            //    this._aa.FromXmlString(this.XMLPublicKey);

            //    // RSAPKCS1SignatureDeformatterオブジェクトを作成
            //    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(this._aa);
            //    rsaDeformatter.SetHashAlgorithm(this.GetHashAlgorithmName(this._ha));

            //    // 検証する
            //    flg = rsaDeformatter.VerifySignature(hashedByte, sign);
            //}
            //else if (this._aa is DSACryptoServiceProvider)
            //{
            //    // 公開鍵
            //    this._aa.FromXmlString(this.XMLPublicKey);

            //    // DSASignatureFormatterオブジェクトを作成
            //    DSASignatureDeformatter dsaSignatureDeformatter = new DSASignatureDeformatter(this._aa);
            //    dsaSignatureDeformatter.SetHashAlgorithm("SHA1");

            //    // 検証する
            //    flg = dsaSignatureDeformatter.VerifySignature(hashedByte, sign);
            //}

            //return flg;
            #endregion
        }

        #endregion

        #region 内部関数

        #region Provider系

        /// <summary>公開鍵・暗号化サービスプロバイダの生成</summary>
        /// <returns>公開鍵・暗号化サービスプロバイダ</returns>
        private void CreateAsymmetricAlgorithmServiceProvider(EnumDigitalSignAlgorithm eaa, out AsymmetricAlgorithm aa, out HashAlgorithm ha)
        {
            aa = null;
            ha = null;

            // 公開鍵・暗号化サービスプロバイダ
            if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_MD5)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = MD5.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA1)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA1.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA256.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA384)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA384.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA512)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA512.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.DSACryptoServiceProvider_SHA1)
            {
                // DSACryptoServiceProvider
                aa = new DSACryptoServiceProvider();
                ha = SHA1.Create();
            }
        }

        /// <summary>GetHashAlgorithmName</summary>
        /// <param name="ha">HashAlgorithm</param>
        /// <returns>HashAlgorithmName</returns>
        private string GetHashAlgorithmName(HashAlgorithm ha)
        {
            string haName = "";

            if (ha is MD5)
            {
                haName = "MD5";
            }
            else if (ha is SHA1)
            {
                haName = "SHA1";
            }
            else if (ha is SHA256)
            {
                haName = "SHA256";
            }
            else if (ha is SHA384)
            {
                haName = "SHA384";
            }
            else if (ha is SHA512)
            {
                haName = "SHA512";
            }

            return haName;
        }

        #endregion
        
        #region MyDispose (派生の末端を呼ぶ)

        /// <summary>MyDispose (派生の末端を呼ぶ)</summary>
        /// <param name="isDisposing">isDisposing</param>
        protected override void MyDispose(bool isDisposing)
        {
            if (this.IsDisposed)
            {
                // 後処理済み。
                // 何もしない。
            }
            else
            {
                // 後処理。
                if (isDisposing)
                {
                    // Dispose all owned managed objects
                    if (this._aa is RSACryptoServiceProvider)
                    {
                        // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                        // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                        ((RSACryptoServiceProvider)this._aa).PersistKeyInCsp = false;
                        this._aa.Clear();
                    }
                    else
                    {
                        // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                        // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                        ((DSACryptoServiceProvider)this._aa).PersistKeyInCsp = false;
                        this._aa.Clear();
                    }
                }

                // Release unmanaged resources
                // 無し

                this.IsDisposed = true;
            }
        }

        #endregion

        #endregion
    }
}
