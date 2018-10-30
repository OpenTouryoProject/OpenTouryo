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
//**********************************************************************************

using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>下位互換のため</summary>
    public class UnSymmetricCryptography : ASymmetricCryptography { }

    /// <summary>非対称アルゴリズムによる暗号化・復号化クラス</summary>
    /// <remarks>
    /// 自由に利用できる。
    /// </remarks>
    public class ASymmetricCryptography
    {
        /// <summary>_easa</summary>
        private static EnumASymmetricAlgorithm _easa = EnumASymmetricAlgorithm.RSACryptoServiceProvider;

        /// <summary>ASymmetricAlgorithm</summary>
        public static EnumASymmetricAlgorithm ASymmetricAlgorithm
        {
            set
            {
                ASymmetricCryptography._easa = value;
            }
            get
            {
                return ASymmetricCryptography._easa;
            }
        }

        #region Encrypt(publicKey)

        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public static string EncryptString(string sourceString, string publicKey)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                ASymmetricCryptography.EncryptBytes(source, publicKey));
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
        public static string EncryptString(string sourceString, string publicKey, bool fOAEP)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                ASymmetricCryptography.EncryptBytes(source, publicKey, fOAEP));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public static byte[] EncryptBytes(byte[] source, string publicKey)
        {
            return ASymmetricCryptography.EncryptBytes(source, publicKey, false);
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public static byte[] EncryptBytes(byte[] source, string publicKey, bool fOAEP)
        {
            byte[] temp = null;

            if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)ASymmetricCryptography.CreateASymmetricAlgorithm();

                // 公開鍵
                rsa.FromXmlString(publicKey);

                // 暗号化
                temp = rsa.Encrypt(source, fOAEP);

                // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                rsa.PersistKeyInCsp = false;

                rsa.Clear(); // devps(1725)
            }

            return temp;
        }
#else
        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public static string EncryptString(string sourceString, string publicKey, RSAEncryptionPadding padding)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                ASymmetricCryptography.EncryptBytes(source, publicKey, padding));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public static byte[] EncryptBytes(byte[] source, string publicKey)
        {
            return ASymmetricCryptography.EncryptBytes(source, publicKey, RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public static byte[] EncryptBytes(byte[] source, string publicKey, RSAEncryptionPadding padding)
        {
            byte[] temp = null;

            if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)ASymmetricCryptography.CreateASymmetricAlgorithm();

                // 公開鍵
                rsa.FromXmlString(publicKey);

                // 暗号化
                temp = rsa.Encrypt(source, padding);

                // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                rsa.PersistKeyInCsp = false;

                rsa.Clear(); // devps(1725)
            }
            else if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptographyNextGeneration)
            {
                RSACng rsa = (RSACng)ASymmetricCryptography.CreateASymmetricAlgorithm();

                // 公開鍵
                rsa.FromXmlString(publicKey);

                // 暗号化
                temp = rsa.Encrypt(source, padding);

                rsa.Clear(); // devps(1725)

            }

            //else if (this._asa is RSAOpenSsl)
            //{
            //}

            return temp;            
        }
#endif

        #endregion

        #region Decrypt(privateKey)

        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化された文字列</returns>
        public static string DecryptString(string sourceString, string privateKey)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                ASymmetricCryptography.DecryptBytes(source, privateKey), CustomEncode.UTF_8);
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
        public static string DecryptString(string sourceString, string privateKey, bool fOAEP)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                ASymmetricCryptography.DecryptBytes(source, privateKey, fOAEP), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public static byte[] DecryptBytes(byte[] source, string privateKey)
        {
            return ASymmetricCryptography.DecryptBytes(source, privateKey, false);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <param name="fOAEP">
        /// ・true  : OAEPパディング（XP以降）
        /// ・false : PKCS#1 v1.5パディング
        /// </param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public static byte[] DecryptBytes(byte[] source, string privateKey, bool fOAEP)
        {
            byte[] temp = null;

            if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)ASymmetricCryptography.CreateASymmetricAlgorithm();

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
        public static string DecryptString(string sourceString, string privateKey, RSAEncryptionPadding padding)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                ASymmetricCryptography.DecryptBytes(source, privateKey, padding), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public static byte[] DecryptBytes(byte[] source, string privateKey)
        {
            return ASymmetricCryptography.DecryptBytes(source, privateKey, RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <param name="padding">RSAEncryptionPadding</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public static byte[] DecryptBytes(byte[] source, string privateKey, RSAEncryptionPadding padding)
        {
            byte[] temp = null;

            if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)ASymmetricCryptography.CreateASymmetricAlgorithm();

                // 秘密鍵
                rsa.FromXmlString(privateKey);

                // 復号化
                temp = rsa.Decrypt(source, padding);

                // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                rsa.PersistKeyInCsp = false;

                rsa.Clear(); // devps(1725)
            }
            else if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptographyNextGeneration)
            {
                RSACng rsa = (RSACng)ASymmetricCryptography.CreateASymmetricAlgorithm();

                // 秘密鍵
                rsa.FromXmlString(privateKey);

                // 復号化
                temp = rsa.Decrypt(source, padding);
                
                rsa.Clear(); // devps(1725)

            }
            //else if (this._asa is RSAOpenSsl)
            //{
            //}

            return temp;
        }
#endif

        #endregion

        #region GetKeyst(public&private)

        /// <summary>秘密鍵と公開鍵を取得する。</summary>
        /// <param name="publicKey">公開鍵</param>
        /// <param name="privateKey">秘密鍵</param>
        public static void GetKeys(out string publicKey, out string privateKey)
        {
            publicKey = "";
            privateKey = "";

            if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)ASymmetricCryptography.CreateASymmetricAlgorithm();

                // インスタンスが同じなら対応したキーペアが取れる。
                // ・公開鍵をXML形式で取得
                publicKey = rsa.ToXmlString(false);
                // ・秘密鍵をXML形式で取得
                privateKey = rsa.ToXmlString(true);

                // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                rsa.PersistKeyInCsp = false;

                rsa.Clear(); // devps(1725)
            }

#if NET45
#else
            else if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptographyNextGeneration)
            {
                RSACng rsa = (RSACng)ASymmetricCryptography.CreateASymmetricAlgorithm();

                // インスタンスが同じなら対応したキーペアが取れる。
                // ・公開鍵をXML形式で取得
                publicKey = rsa.ToXmlString(false);
                // ・秘密鍵をXML形式で取得
                privateKey = rsa.ToXmlString(true);
                                
                rsa.Clear(); // devps(1725)
            }
#endif

            //else if (this._asa is RSAOpenSsl)
            //{
            //}
        }

        #endregion

        #region CreateASymmetricAlgorithm

        /// <summary>
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダを生成
        /// </summary>
        /// <returns>
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダ
        /// </returns>
        private static AsymmetricAlgorithm CreateASymmetricAlgorithm()
        {
            AsymmetricAlgorithm asa = null;

            if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptoServiceProvider)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                asa = RSACryptoServiceProvider.Create(); // devps(1703)
            }

#if NET45
#else
            else if (ASymmetricCryptography._easa == EnumASymmetricAlgorithm.RSACryptographyNextGeneration)
            {
                // RSACngサービスプロバイダ
                asa = RSACng.Create(); // devps(1703)
            }
#endif

            // .NET Platform Extensions 2.1 ? 以降
            //else if (easa == EnumASymmetricAlgorithm.RSAOpenSsl)
            //{
            //    // RSAOpenSslサービスプロバイダ
            //    asa = RSAOpenSsl.Create(); // devps(1703)
            //}

            return asa;
        }

        #endregion
    }
}