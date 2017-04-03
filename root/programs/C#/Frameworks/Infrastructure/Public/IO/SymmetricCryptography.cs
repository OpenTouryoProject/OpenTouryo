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
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>
    /// 対称アルゴリズムによる
    /// 暗号化サービスプロバイダの種類
    /// </summary>
    public enum EnumSymmetricAlgorithm
    {
        // AesCryptoServiceProvider, AesManagedは.NET Framework 3.5からの提供。
        // 暗号化プロバイダ選択の優先順は、高い順に、Managed → CAPI(CSP) → CNG。
        // Aesは、ManagedがあるのでCAPI(CSP)のAesCryptoServiceProviderを削除。
        // サポート範囲の変更により、今後、CAPI(CSP)とCNGの優先順位の反転を検討。

        ///// <summary>AesCryptoServiceProvider</summary>
        //AesCryptoServiceProvider,

        /// <summary>AesManaged</summary>
        AesManaged,

        /// <summary>DESCryptoServiceProvider</summary>
        DESCryptoServiceProvider,

        /// <summary>RC2CryptoServiceProvider</summary>
        RC2CryptoServiceProvider,

        /// <summary>RijndaelManaged</summary>
        RijndaelManaged,

        /// <summary>TripleDESCryptoServiceProvider</summary>
        TripleDESCryptoServiceProvider
    };

    /// <summary>対称アルゴリズムによる暗号化・復号化クラス</summary>
    /// <remarks>
    /// 自由に利用できる。
    /// http://dobon.net/vb/dotnet/string/encryptstring.html
    /// </remarks>
    public class SymmetricCryptography
    {
        /// <summary>固定ソルト</summary>
        /// <remarks>以前のVerからデフォルト値を修正しているので、</remarks>
        private static byte[] Salt = CustomEncode.StringToByte(
            "Touryo.Infrastructure.Public.IO.SymmetricCryptography.Salt", CustomEncode.UTF_8);

        /// <summary>固定ストレッチング</summary>
        private static int Stretching = 1000;

        #region Encrypt

        #region EncryptString

        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 暗号化された文字列
        /// </returns>
        public static string EncryptString(
            string sourceString, string password, EnumSymmetricAlgorithm esa)
        {
            return SymmetricCryptography.EncryptString(
                sourceString, password, esa, SymmetricCryptography.Salt, SymmetricCryptography.Stretching);
        }

        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 暗号化された文字列
        /// </returns>
        public static string EncryptString(
            string sourceString, string password, EnumSymmetricAlgorithm esa, byte[] salt)
        {
            return SymmetricCryptography.EncryptString(
                sourceString, password, esa, salt, SymmetricCryptography.Stretching);
        }

        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <param name="stretching">ストレッチング</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 暗号化された文字列
        /// </returns>
        public static string EncryptString(
            string sourceString, string password, EnumSymmetricAlgorithm esa, byte[] salt, int stretching)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                SymmetricCryptography.EncryptBytes(source, password, esa, salt, stretching));
        }

        #endregion

        #region EncryptBytes

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 暗号化されたバイト配列
        /// </returns>
        public static byte[] EncryptBytes(
            byte[] source, string password, EnumSymmetricAlgorithm esa)
        {
            return SymmetricCryptography.EncryptBytes(
                source, password, esa, SymmetricCryptography.Salt, SymmetricCryptography.Stretching);
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 暗号化されたバイト配列
        /// </returns>
        public static byte[] EncryptBytes(
            byte[] source, string password, EnumSymmetricAlgorithm esa, byte[] salt)
        {
            return SymmetricCryptography.EncryptBytes(
                source, password, esa, salt, SymmetricCryptography.Stretching);
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <param name="stretching">ストレッチング</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 暗号化されたバイト配列
        /// </returns>
        public static byte[] EncryptBytes(
            byte[] source, string password, EnumSymmetricAlgorithm esa, byte[] salt, int stretching)
        {
            // 暗号化サービスプロバイダを生成
            SymmetricAlgorithm sa = SymmetricCryptography.CreateSymmetricAlgorithm(esa);

            // パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;

            SymmetricCryptography.GenerateKeyFromPassword(
                password, sa.KeySize, out key, sa.BlockSize, out iv, salt, stretching);

            sa.Key = key;
            sa.IV = iv;

            // 暗号化
            byte[] temp = sa.CreateEncryptor().TransformFinalBlock(source, 0, source.Length);
            sa.Clear(); // devps(1725)
            return temp;
        }

        #endregion

        #endregion

        #region Decrypt

        #region DecryptString

        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 復号化された文字列
        /// </returns>
        public static string DecryptString(
            string sourceString, string password, EnumSymmetricAlgorithm esa)
        {
            return SymmetricCryptography.DecryptString(
                sourceString, password, esa, SymmetricCryptography.Salt, SymmetricCryptography.Stretching);
        }

        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 復号化された文字列
        /// </returns>
        public static string DecryptString(
            string sourceString, string password, EnumSymmetricAlgorithm esa, byte[] salt)
        {
            return SymmetricCryptography.DecryptString(
                sourceString, password, esa, salt, SymmetricCryptography.Stretching);
        }

        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <param name="stretching">ストレッチング</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 復号化された文字列
        /// </returns>
        public static string DecryptString(
            string sourceString, string password, EnumSymmetricAlgorithm esa, byte[] salt, int stretching)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                SymmetricCryptography.DecryptBytes(source, password, esa, salt, stretching), CustomEncode.UTF_8);
        }

        #endregion

        #region DecryptBytes

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 復号化されたバイト配列
        /// </returns>
        public static byte[] DecryptBytes(
            byte[] source, string password, EnumSymmetricAlgorithm esa)
        {
            return SymmetricCryptography.DecryptBytes(
                source, password, esa, SymmetricCryptography.Salt, SymmetricCryptography.Stretching);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 復号化されたバイト配列
        /// </returns>
        public static byte[] DecryptBytes(
            byte[] source, string password, EnumSymmetricAlgorithm esa, byte[] salt)
        {
            return SymmetricCryptography.DecryptBytes(
                source, password, esa, salt, SymmetricCryptography.Stretching);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <param name="esa">
        /// 対称アルゴリズムによる
        /// 暗号化サービスプロバイダの種類
        /// </param>
        /// <param name="salt">ソルト</param>
        /// <param name="stretching">ストレッチング</param>
        /// <returns>
        /// 対称アルゴリズムで
        /// 復号化されたバイト配列
        /// </returns>
        public static byte[] DecryptBytes(
            byte[] source, string password, EnumSymmetricAlgorithm esa, byte[] salt, int stretching)
        {
            // 暗号化サービスプロバイダを生成
            SymmetricAlgorithm sa = SymmetricCryptography.CreateSymmetricAlgorithm(esa);

            // パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;

            SymmetricCryptography.GenerateKeyFromPassword(
                password, sa.KeySize, out key, sa.BlockSize, out iv, salt, stretching);

            sa.Key = key;
            sa.IV = iv;

            // 復号化
            byte[] temp = sa.CreateDecryptor().TransformFinalBlock(source, 0, source.Length);
            sa.Clear(); // devps(1725)
            return temp;
        }

        #endregion

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
        private static void GenerateKeyFromPassword(string password,
            int keySize, out byte[] key, int blockSize, out byte[] iv, byte[] salt, int stretching)
        {
            //パスワードから共有キーと初期化ベクタを作成する

            //Rfc2898DeriveBytesオブジェクトを作成する
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt, stretching);

            //.NET Framework 1.1以下の時は、PasswordDeriveBytesを使用する
            //PasswordDeriveBytes deriveBytes = new PasswordDeriveBytes(password, salt);

            // コンストラクタで指定するように変更した。
            ////反復処理回数を指定する
            //deriveBytes.IterationCount = stretching;

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
        private static SymmetricAlgorithm CreateSymmetricAlgorithm(EnumSymmetricAlgorithm esa)
        {
            SymmetricAlgorithm sa = null;

            // AesCryptoServiceProvider, AesManagedは.NET Framework 3.5からの提供。
            // 暗号化プロバイダ選択の優先順は、高い順に、Managed → CAPI(CSP) → CNG。
            // Aesは、ManagedがあるのでCAPI(CSP)のAesCryptoServiceProviderを削除。
            // サポート範囲の変更により、今後、CAPI(CSP)とCNGの優先順位の反転を検討。

            //if (esa == EnumSymmetricAlgorithm.AesCryptoServiceProvider)
            //{
            //    // AesCryptoServiceProviderサービスプロバイダ
            //    sa = AesCryptoServiceProvider.Create(); // devps(1703)
            //}
            //else
            if (esa == EnumSymmetricAlgorithm.AesManaged)
            {
                // AesManagedサービスプロバイダ
                sa = AesManaged.Create(); // devps(1703)
            }
            else if (esa == EnumSymmetricAlgorithm.DESCryptoServiceProvider)
            {
                // DESCryptoServiceProviderサービスプロバイダ
                sa = DESCryptoServiceProvider.Create(); // devps(1703)
            }
            else if (esa == EnumSymmetricAlgorithm.RC2CryptoServiceProvider)
            {
                // RC2CryptoServiceProviderサービスプロバイダ
                sa = RC2CryptoServiceProvider.Create(); // devps(1703)
            }
            else if (esa == EnumSymmetricAlgorithm.RijndaelManaged)
            {
                // RijndaelManagedサービスプロバイダ
                sa = RijndaelManaged.Create(); // devps(1703)
            }
            else if (esa == EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)
            {
                // TripleDESCryptoServiceProviderサービスプロバイダ
                sa = TripleDESCryptoServiceProvider.Create(); // devps(1703)
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INJUSTICE, "EnumSymmetricAlgorithm esa");
            }

            return sa;
        }

        #endregion
    }
}