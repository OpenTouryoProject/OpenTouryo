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
//**********************************************************************************

// 方法 : キー コンテナーに非対称キーを格納する | Microsoft Docs
// https://docs.microsoft.com/ja-jp/dotnet/standard/security/how-to-store-asymmetric-keys-in-a-key-container

using System.Security.Cryptography;

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

        /// <summary>_easa</summary>
        private EnumASymmetricAlgorithm _easa = EnumASymmetricAlgorithm.RsaCsp;

        /// <summary>ASymmetricAlgorithm</summary>
        public EnumASymmetricAlgorithm ASymmetricAlgorithm
        {
            set
            {
                this._easa = value;
            }
            get
            {
                return this._easa;
            }
        }

        /// <summary>Constructor</summary>
        /// <param name="algorithm">EnumASymmetricAlgorithm</param>
        public ASymmetricCryptography(EnumASymmetricAlgorithm algorithm)
        {
            this._easa = algorithm;
        }

        #endregion

        #region Encrypt(publicKey)

        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public string EncryptString(string sourceString, string publicKey)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(this.EncryptBytes(source, publicKey));
        }

#if NET45
        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public string EncryptString(string sourceString, string publicKey, bool fOAEP)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                this.EncryptBytes(source, publicKey, fOAEP));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public byte[] EncryptBytes(byte[] source, string publicKey)
        {
            return this.EncryptBytes(source, publicKey, false);
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public byte[] EncryptBytes(byte[] source, string publicKey, bool fOAEP)
        {
            byte[] temp = null;

            // NET45はCSPのみ。
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)
                AsymmetricAlgorithmCmnFunc.CreateCryptographySP(this._easa);

            rsa.FromXmlString(publicKey);

            if (this._easa == EnumASymmetricAlgorithm.RsaCsp)
            {
                // 暗号化
                temp = rsa.Encrypt(source, fOAEP);
                rsa.PersistKeyInCsp = false;
            }

            rsa.Clear(); // devps(1725)

            return temp;
        }
#else
        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public string EncryptString(string sourceString, string publicKey, RSAEncryptionPadding padding)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                this.EncryptBytes(source, publicKey, padding));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public byte[] EncryptBytes(byte[] source, string publicKey)
        {
            return this.EncryptBytes(source, publicKey, RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public byte[] EncryptBytes(byte[] source, string publicKey, RSAEncryptionPadding padding)
        {
            byte[] temp = null;

            RSA rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateCryptographySP(this._easa);

            // 公開鍵
            rsa.FromXmlString(publicKey);

            // 暗号化
            temp = rsa.Encrypt(source, padding);

            if (rsa is RSACryptoServiceProvider)
            {
                ((RSACryptoServiceProvider)rsa).PersistKeyInCsp = false;
            }

            rsa.Clear(); // devps(1725)

            return temp;            
        }
#endif

            #endregion

        #region Decrypt(privateKey)

        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化された文字列</returns>
        public string DecryptString(string sourceString, string privateKey)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                this.DecryptBytes(source, privateKey), CustomEncode.UTF_8);
        }

#if NET45
        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで復号化された文字列</returns>
        public string DecryptString(string sourceString, string privateKey, bool fOAEP)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                this.DecryptBytes(source, privateKey, fOAEP), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public byte[] DecryptBytes(byte[] source, string privateKey)
        {
            return this.DecryptBytes(source, privateKey, false);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public byte[] DecryptBytes(byte[] source, string privateKey, bool fOAEP)
        {
            byte[] temp = null;

            if (this._easa == EnumASymmetricAlgorithm.RsaCsp)
            {
                // NET45はCSPのみ。
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)
                    AsymmetricAlgorithmCmnFunc.CreateCryptographySP(this._easa);

                // 秘密鍵
                rsa.FromXmlString(privateKey);

                // 復号化
                temp = rsa.Decrypt(source, fOAEP);

                // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                rsa.PersistKeyInCsp = false;

                rsa.Clear(); // devps(1725)
            }

            return temp;
        }
#else
        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで復号化された文字列</returns>
        public string DecryptString(string sourceString, string privateKey, RSAEncryptionPadding padding)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                this.DecryptBytes(source, privateKey, padding), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public byte[] DecryptBytes(byte[] source, string privateKey)
        {
            return this.DecryptBytes(source, privateKey, RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public byte[] DecryptBytes(byte[] source, string privateKey, RSAEncryptionPadding padding)
        {
            byte[] temp = null;

            RSA rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateCryptographySP(this._easa);

            // 秘密鍵
            rsa.FromXmlString(privateKey);

            // 復号化
            temp = rsa.Decrypt(source, padding);

            if (rsa is RSACryptoServiceProvider)
            {
                ((RSACryptoServiceProvider)rsa).PersistKeyInCsp = false;
            }

            rsa.Clear(); // devps(1725)

            return temp;
        }
#endif

        #endregion

        #region GetKeyst(public&private)

        /// <summary>秘密鍵と公開鍵を取得する。</summary>
        /// <param name="publicKey">公開鍵</param>
        /// <param name="privateKey">秘密鍵</param>
        public void GetKeys(out string publicKey, out string privateKey)
        {
            publicKey = "";
            privateKey = "";

            RSA rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateCryptographySP(this._easa);

            // インスタンスが同じなら対応したキーペアが取れる。
            // ・公開鍵をXML形式で取得
            publicKey = rsa.ToXmlString(false);
            // ・秘密鍵をXML形式で取得
            privateKey = rsa.ToXmlString(true);

            if (rsa is RSACryptoServiceProvider)
            {
                ((RSACryptoServiceProvider)rsa).PersistKeyInCsp = false;
            }

            rsa.Clear(); // devps(1725)
        }

        #endregion
    }
}