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
//* クラス名        ：ASymmetricCryptography
//* クラス日本語名  ：非対称アルゴリズムによる暗号化・復号化
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//*  2013/02/11  西野 大介         クラス名の変更（Unsymmetric→Asymmetric）
//*  2013/02/18  西野 大介         CustomEncode使用に統一
//*  2014/03/13  西野 大介         devps(1703):Createメソッドを使用してcryptoオブジェクトを作成します。
//*  2014/03/13  西野 大介         devps(1725):暗号クラスの使用終了時にデータをクリアする。
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2018/10/30  西野 大介         各種プロバイダのサポートを追加
//*  2018/10/30  西野 大介         RSAEncryptionPadding指定の追加。
//*  2018/11/09  西野 大介         インスタンス・メソッド化
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//*  2019/01/29  西野 大介         X.509対応（JWE（RSAES-OAEP and AES GCM）対応
//**********************************************************************************

// 方法 : キー コンテナーに非対称キーを格納する | Microsoft Docs
// https://docs.microsoft.com/ja-jp/dotnet/standard/security/how-to-store-asymmetric-keys-in-a-key-container

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    ///// <summary>下位互換のため</summary>
    //public class UnSymmetricCryptography : ASymmetricCryptography { }

    /// <summary>非対称アルゴリズムによる暗号化・復号化クラス</summary>
    /// <remarks>
    /// 自由に利用できる。
    /// </remarks>
    public class ASymmetricCryptography
    {
        #region mem & prop & constructor

        #region 鍵
        /// <summary>_asa</summary>
        private AsymmetricAlgorithm _asa = null;

        /// <summary>AsymmetricAlgorithm</summary>
        public AsymmetricAlgorithm AsymmetricAlgorithm
        {
            get
            {
                return this._asa;
            }
        }

        /// <summary>PublicXmlKey</summary>
        public string PublicXmlKey
        {
            get
            {
                RSA rsa = (RSA)this._asa;
                return rsa.ToXmlString(false);
            }
        }

        /// <summary>PrivateXmlKey</summary>
        public string PrivateXmlKey
        {
            get
            {
                RSA rsa = (RSA)this._asa;
                return rsa.ToXmlString(true);
            }
        }
        #endregion

        #region Constructor

        /// <summary>Constructor</summary>
        /// <param name="algorithm">EnumASymmetricAlgorithm</param>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public ASymmetricCryptography(EnumASymmetricAlgorithm algorithm,
            string certificateFilePath = "", string password = "",
            X509KeyStorageFlags flag = X509KeyStorageFlags.DefaultKeySet)
        {
            this._asa = AsymmetricAlgorithmCmnFunc.CreateCryptographySP(
                algorithm, certificateFilePath, password, flag);
        }

        #endregion

        #endregion

        #region Encrypt(publicXmlKey)
#if NET45
        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicXmlKey">暗号化に使用する公開鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public string EncryptString(string sourceString, string publicXmlKey = "", bool fOAEP = false)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                this.EncryptBytes(source, publicXmlKey, fOAEP));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicXmlKey">暗号化に使用する公開鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public byte[] EncryptBytes(byte[] source, string publicXmlKey = "", bool fOAEP = false)
        {
            // NET45はCSPのみ。
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)this._asa;

            // 公開鍵
            if (string.IsNullOrEmpty(publicXmlKey))
            {
                //rsa.FromXmlString(this.PublicXmlKey);
            }
            else
            {
                rsa.FromXmlString(publicXmlKey);
            }

            // 暗号化
            return rsa.Encrypt(source, fOAEP);
        }
#else
        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicXmlKey">暗号化に使用する公開鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public string EncryptString(string sourceString, string publicXmlKey = "", RSAEncryptionPadding padding = null)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                this.EncryptBytes(source, publicXmlKey, padding));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicXmlKey">暗号化に使用する公開鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public byte[] EncryptBytes(byte[] source, string publicXmlKey = "", RSAEncryptionPadding padding = null)
        {
            RSA rsa = (RSA)this._asa;

            // 公開鍵
            if (string.IsNullOrEmpty(publicXmlKey))
            {
                //rsa.FromXmlString(this.PublicXmlKey);
            }
            else
            {
                rsa.FromXmlString(publicXmlKey);
            }

            // 暗号化
            if (padding == null)
            {
                padding = RSAEncryptionPadding.Pkcs1;
            }
            
            return rsa.Encrypt(source, padding);
        }
#endif

        #endregion

        #region Decrypt(privateXmlKey)

#if NET45
        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="privateXmlKey">復号化に使用する秘密鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで復号化された文字列</returns>
        public string DecryptString(string sourceString, string privateXmlKey = "", bool fOAEP = false)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                this.DecryptBytes(source, privateXmlKey, fOAEP), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateXmlKey">復号化に使用する秘密鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public byte[] DecryptBytes(byte[] source, string privateXmlKey = "", bool fOAEP = false)
        {
            // NET45はCSPのみ。
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)this._asa;

            // 秘密鍵
            if (string.IsNullOrEmpty(privateXmlKey))
            {
                //rsa.FromXmlString(this.PrivateXmlKey);
            }
            else
            {
                rsa.FromXmlString(privateXmlKey);
            }

            // 復号化
            return rsa.Decrypt(source, fOAEP);
        }
#else
        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="privateXmlKey">復号化に使用する秘密鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで復号化された文字列</returns>
        public string DecryptString(string sourceString, string privateXmlKey = "", RSAEncryptionPadding padding = null)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                this.DecryptBytes(source, privateXmlKey, padding), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateXmlKey">復号化に使用する秘密鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public byte[] DecryptBytes(byte[] source, string privateXmlKey = "", RSAEncryptionPadding padding = null)
        {
            RSA rsa = (RSA)this._asa;

            // 秘密鍵
            if (string.IsNullOrEmpty(privateXmlKey))
            {
                //rsa.FromXmlString(this.PrivateXmlKey);
            }
            else
            {
                rsa.FromXmlString(privateXmlKey);
            }

            // 復号化
            if (padding == null)
            {
                padding = RSAEncryptionPadding.Pkcs1;
            }

            return rsa.Decrypt(source, padding);
        }
#endif

        #endregion
    }
}