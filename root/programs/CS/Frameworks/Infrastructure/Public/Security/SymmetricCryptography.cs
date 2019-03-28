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
//* クラス名        ：SymmetricCryptography
//* クラス日本語名  ：対称アルゴリズムによる暗号化・復号化
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//*  2011/10/09  西野 大介         国際化対応
//*  2013/02/11  西野 大介         Aes系プロバイダ対応（2.0ではサポートされず）
//*  2013/02/12  西野 大介         ソルト、ストレッチング可変のオーバーロード追加
//*  2013/02/18  西野 大介         CustomEncode使用に統一
//*  2014/03/13  西野 大介         devps(1703):Createメソッドを使用してcryptoオブジェクトを作成します。
//*  2014/03/13  西野 大介         devps(1725):暗号クラスの使用終了時にデータをクリアする。
//*  2017/01/10  西野 大介         stretch回数の指定方法をPropertyからConstructorに変更した。
//*  2017/01/10  西野 大介         AesCryptoServiceProviderを削除（.NET3.5から実装されたAesManagedを残す）
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2017/12/25  西野 大介         暗号化ライブラリ追加に伴うコード追加・修正
//*  2018/10/30  西野 大介         各種プロバイダのサポートを追加
//*  2018/10/30  西野 大介         CipherMode, PaddingMode指定の追加（CipherModeによってはIVを無視する）。
//*  2018/11/09  西野 大介         インスタンス・メソッド化
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>対称アルゴリズムによる暗号化・復号化クラス</summary>
    /// <remarks>
    /// 自由に利用できる。
    /// http://dobon.net/vb/dotnet/string/encryptstring.html
    /// </remarks>
    public class SymmetricCryptography
    {
        #region mem & prop & constructor

        /// <summary>ソルト（既定値）</summary>
        public static byte[] DefaultSalt = 
            CustomEncode.StringToByte("OpenTouryo", CustomEncode.UTF_8); // 最低8バイト

        /// <summary>ストレッチング（既定値）</summary>
        public static int DefaultStretching = 1; // 1以上の値（正の整数、自然数）

        /// <summary>EnumSymmetricAlgorithm</summary>
        private EnumSymmetricAlgorithm Algorithm = 0;

        /// <summary>ソルト</summary>
        private byte[] Salt = null;

        /// <summary>ストレッチング</summary>
        private int Stretching = 0;

        /// <summary>Constructor</summary>
        public SymmetricCryptography() :
            this(EnumSymmetricAlgorithm.AES_M) {　}

        /// <summary>Constructor</summary>
        /// <param name="algorithm">対称アルゴリズム</param>
        public SymmetricCryptography(EnumSymmetricAlgorithm algorithm) :
            this(algorithm, SymmetricCryptography.DefaultSalt) { }
        /// <summary>Constructor</summary>
        /// <param name="algorithm">対称アルゴリズム</param>
        /// <param name="salt">ソルト</param>
        public SymmetricCryptography(EnumSymmetricAlgorithm algorithm, byte[] salt) :
            this(algorithm, salt, SymmetricCryptography.DefaultStretching) { }

        /// <summary>Constructor</summary>
        /// <param name="algorithm">対称アルゴリズム</param>
        /// <param name="salt">ソルト</param>
        /// <param name="stretching">ストレッチング</param>
        public SymmetricCryptography(EnumSymmetricAlgorithm algorithm, byte[] salt, int stretching)
        {
            this.Algorithm = algorithm;
            this.Salt = salt;
            this.Stretching = stretching;
        }

        #endregion

        #region Encrypt

        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <returns>暗号化された文字列</returns>
        public string EncryptString(string sourceString, string password)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(this.EncryptBytes(source, password));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <returns>暗号化されたバイト配列</returns>
        public byte[] EncryptBytes(byte[] source, string password)
        {
            // 暗号化サービスプロバイダを生成
            SymmetricAlgorithm sa = this.CreateSymmetricAlgorithm(this.Algorithm);

            // パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;

            this.GenerateKeyFromPassword(
                password, sa.KeySize, out key, sa.BlockSize, out iv, this.Salt, this.Stretching);

            sa.Key = key;
            sa.IV = iv;

            // 暗号化
            byte[] temp = sa.CreateEncryptor().TransformFinalBlock(source, 0, source.Length);
            sa.Clear(); // devps(1725)
            return temp;
        }

        #endregion

        #region Decrypt

        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <returns>復号化された文字列</returns>
        public string DecryptString(
            string sourceString, string password)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(this.DecryptBytes(source, password), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <returns>復号化されたバイト配列</returns>
        public byte[] DecryptBytes(byte[] source, string password)
        {
            // 暗号化サービスプロバイダを生成
            SymmetricAlgorithm sa = this.CreateSymmetricAlgorithm(this.Algorithm);

            // パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;

            this.GenerateKeyFromPassword(
                password, sa.KeySize, out key, sa.BlockSize, out iv, this.Salt, this.Stretching);

            sa.Key = key;
            sa.IV = iv;

            // 復号化
            byte[] temp = sa.CreateDecryptor().TransformFinalBlock(source, 0, source.Length);
            sa.Clear(); // devps(1725)
            return temp;
        }

        #endregion

        #region 内部関数

        /// <summary>パスワードから共有キーと初期化ベクタを生成する</summary>
        /// <param name="password">基になるパスワード</param>
        /// <param name="keySize">共有キーのサイズ（ビット）</param>
        /// <param name="key">作成された共有キー</param>
        /// <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
        /// <param name="iv">作成された初期化ベクタ</param>
        /// <param name="salt">ソルト</param>
        /// <param name="stretching">ストレッチング</param>
        private void GenerateKeyFromPassword(string password,
            int keySize, out byte[] key, int blockSize, out byte[] iv, byte[] salt, int stretching)
        {
            //パスワードから共有キーと初期化ベクタを作成する

            //Rfc2898DeriveBytesオブジェクトを作成する
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt, stretching);

            //.NET Framework 1.1以下の時は、PasswordDeriveBytesを使用する
            //PasswordDeriveBytes deriveBytes = new PasswordDeriveBytes(password, salt);
            
            //共有キーと初期化ベクタを生成する
            key = deriveBytes.GetBytes(keySize / 8);
            iv = deriveBytes.GetBytes(blockSize / 8);
        }

        /// <summary>
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダを生成
        /// </summary>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <returns>
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダ
        /// </returns>
        private SymmetricAlgorithm CreateSymmetricAlgorithm(EnumSymmetricAlgorithm esa)
        {
            //esa = EnumSymmetricAlgorithm.AesManaged
            //    | EnumSymmetricAlgorithm.CipherMode_CBC
            //    | EnumSymmetricAlgorithm.PaddingMode_PKCS7;

            // CipherMode, PaddingMode指定の追加
            CipherMode cm = 0;
            PaddingMode pm = 0;

            // CipherMode
            if (esa.HasFlag(EnumSymmetricAlgorithm.CipherMode_CBC))
            {
                cm = CipherMode.CBC;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.CipherMode_CFB))
            {
                cm = CipherMode.CFB;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.CipherMode_CTS))
            {
                cm = CipherMode.CTS;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.CipherMode_ECB))
            {
                cm = CipherMode.ECB;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.CipherMode_OFB))
            {
                cm = CipherMode.OFB;
            }

            // PaddingMode
            if (esa.HasFlag(EnumSymmetricAlgorithm.PaddingMode_None))
            {
                pm = PaddingMode.None;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.PaddingMode_Zeros))
            {
                pm = PaddingMode.Zeros;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.PaddingMode_ANSIX923))
            {
                pm = PaddingMode.ANSIX923;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.PaddingMode_ISO10126))
            {
                pm = PaddingMode.ISO10126;
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.PaddingMode_PKCS7))
            {
                pm = PaddingMode.PKCS7;
            }

            return CreateSymmetricAlgorithm(esa, cm, pm);
        }

        /// <summary>対称アルゴリズム暗号化サービスプロバイダ生成</summary>
        /// <param name="esa">EnumSymmetricAlgorithm</param>
        /// <param name="cm">CipherMode</param>
        /// <param name="pm">PaddingMode</param>
        /// <returns>SymmetricAlgorithm</returns>
        private SymmetricAlgorithm CreateSymmetricAlgorithm(EnumSymmetricAlgorithm esa, CipherMode cm, PaddingMode pm)
        {
            #region Constructor
            SymmetricAlgorithm sa = null;

            #region Aes
            if (esa.HasFlag(EnumSymmetricAlgorithm.AES_CSP))
            {
                // AesCryptoServiceProviderサービスプロバイダ
                sa = AesCryptoServiceProvider.Create(); // devps(1703)
            }
            else if (esa.HasFlag(EnumSymmetricAlgorithm.AES_M))
            {
                // AesManagedサービスプロバイダ
                sa = AesManaged.Create(); // devps(1703)
            }
#if NET45 || NET46
#else
            else if (esa.HasFlag(EnumSymmetricAlgorithm.AES_CNG))
            {
                // AesCngサービスプロバイダ
                sa = AesCng.Create(); // devps(1703)
            }
#endif
            #endregion

            #region TripleDES
            else if (esa.HasFlag(EnumSymmetricAlgorithm.TDES_CSP))
            {
                // TripleDESCryptoServiceProviderサービスプロバイダ
                sa = TripleDESCryptoServiceProvider.Create(); // devps(1703)
            }

#if NET45 || NET46
#else
            else if (esa.HasFlag(EnumSymmetricAlgorithm.TDES_CNG))
            {
                // TripleDESCngサービスプロバイダ
                sa = TripleDESCng.Create(); // devps(1703)
            }
#endif
            #endregion

            #region Others
            else if (esa.HasFlag(EnumSymmetricAlgorithm.DES_CSP))
            {
                // DESCryptoServiceProviderサービスプロバイダ
                sa = DESCryptoServiceProvider.Create(); // devps(1703)
            }

            else if (esa.HasFlag(EnumSymmetricAlgorithm.RC2_CSP))
            {
                // RC2CryptoServiceProviderサービスプロバイダ
                sa = RC2CryptoServiceProvider.Create(); // devps(1703)
            }

            else if (esa.HasFlag(EnumSymmetricAlgorithm.Rijndael_M))
            {
                // RijndaelManagedサービスプロバイダ
                sa = RijndaelManaged.Create(); // devps(1703)
            }
            #endregion

            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT, "EnumSymmetricAlgorithm esa");
            }
            #endregion

            #region Options
            // cmが設定されている場合。
            if (cm != 0)
            {
                sa.Mode = cm;
            }

            // pmが設定されている場合。
            if (pm != 0)
            {
                sa.Padding = pm;
            }
            #endregion

            return sa;
        }

        #endregion
    }
}