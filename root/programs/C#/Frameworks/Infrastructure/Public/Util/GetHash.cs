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
//* クラス名        ：GetHash
//* クラス日本語名  ：ハッシュを取得するクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/02/15  西野 大介         新規作成
//*  2014/03/13  西野 大介         devps(1703):Createメソッドを使用してcryptoオブジェクトを作成します。
//*  2014/03/13  西野 大介         devps(1725):暗号クラスの使用終了時にデータをクリアする。
//*  2017/01/10  西野 大介         ストレッチ回数を指定可能にし、新設したGetPasswordを利用するように変更。
//*  2017/01/10  西野 大介         saltedPasswdのformat変更(salt+stretchCount+hashedPassword)。
//*  2017/01/10  西野 大介         上記のformat変更に伴い、EqualSaltedPasswd側のI/F変更が発生。
//**********************************************************************************

using System.Security.Cryptography;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Util
{
    #region Enum

    /// <summary>
    /// ハッシュアルゴリズムのサービスプロバイダの種類
    /// </summary>
    public enum EnumHashAlgorithm
    {
        /// <summary>Default</summary>
        Default,

        /// <summary>MD5CryptoServiceProvider</summary>
        MD5CryptoServiceProvider,

        /// <summary>SHA1CryptoServiceProvider</summary>
        SHA1CryptoServiceProvider,

        /// <summary>SHA1Managed</summary>
        SHA1Managed,

        /// <summary>SHA256Managed</summary>
        SHA256Managed,

        /// <summary>SHA384Managed</summary>
        SHA384Managed,

        /// <summary>SHA512Managed</summary>
        SHA512Managed,
    };

    #endregion

    #region GetHash

    /// <summary>ハッシュを取得するクラス</summary>
    public class GetHash
    {
        #region GetSaltedPassword

        /// <summary>
        /// Password entered by the userをDB保存する際、
        /// Salted and hashed passwordとして保存する必要がある。
        /// </summary>
        /// <param name="rawPassword">>Password entered by the user.</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="saltLength">ソルトの文字列長</param>
        /// <returns>Salted and hashed password.</returns>
        /// <see ref="http://www.atmarkit.co.jp/ait/articles/1110/06/news154_2.html"/>
        public static string GetSaltedPassword(string rawPassword, EnumHashAlgorithm eha, int saltLength)
        {
            // overloadへ
            return GetHash.GetSaltedPassword(rawPassword, eha, saltLength, 1);
        }

        /// <summary>
        /// Password entered by the userをDB保存する際、
        /// Salted and hashed passwordとして保存する必要がある。
        /// </summary>
        /// <param name="rawPassword">>Password entered by the user.</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="saltLength">ソルトの文字列長</param>
        /// <param name="stretchCount">ストレッチ回数</param>
        /// <returns>Salted and hashed password.</returns>
        public static string GetSaltedPassword(string rawPassword, EnumHashAlgorithm eha, int saltLength, int stretchCount)
        {
            // ランダム・ソルト文字列を生成（区切り記号は含まなくても良い）
            string salt = GetPassword.Generate(saltLength, 0); //Membership.GeneratePassword(saltLength, 0);

            // Salted and hashed password（文字列）を生成して返す。
            return
                CustomEncode.ToBase64String(CustomEncode.StringToByte(salt, CustomEncode.UTF_8))
                + "." + CustomEncode.ToBase64String(CustomEncode.StringToByte(stretchCount.ToString(), CustomEncode.UTF_8))
                + "." + CustomEncode.ToBase64String(CustomEncode.StringToByte(GetHash.GetHashString(salt + rawPassword, eha, stretchCount), CustomEncode.UTF_8));
        }

        /// <summary>パスワードを比較して認証する。</summary>
        /// <param name="rawPassword">Password entered by the user.</param>
        /// <param name="saltedPassword">Salted and hashed password.</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>
        /// true：パスワードは一致した。
        /// false：パスワードは一致しない。
        /// </returns>
        public static bool EqualSaltedPassword(string rawPassword, string saltedPassword, EnumHashAlgorithm eha)
        {
            // ソルト部分を取得
            string[] temp = saltedPassword.Split('.');
            string salt = CustomEncode.ByteToString(CustomEncode.FromBase64String(temp[0]), CustomEncode.UTF_8);
            int stretchCount = int.Parse(CustomEncode.ByteToString(CustomEncode.FromBase64String(temp[1]), CustomEncode.UTF_8));
            string hashedPassword = CustomEncode.ByteToString(CustomEncode.FromBase64String(temp[2]), CustomEncode.UTF_8);

            // 引数のsaltedPasswordと、rawPasswordから自作したsaltedPasswordを比較
            if (hashedPassword == GetHash.GetHashString(salt + rawPassword, eha, stretchCount))
            {
                // 一致した。
                return true;
            }
            else
            {
                // 一致しなかった。
                return false;
            }
        }

        #endregion

        #region GetHash

        #region String

        /// <summary>文字列のハッシュ値を計算して返す。</summary>
        /// <param name="sourceString">文字列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>ハッシュ値（文字列）</returns>
        public static string GetHashString(string sourceString, EnumHashAlgorithm eha)
        {
            // overloadへ
            return GetHash.GetHashString(sourceString, eha, 1);
        }

        /// <summary>文字列のハッシュ値を計算して返す。</summary>
        /// <param name="sourceString">文字列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="stretchCount">ストレッチ回数</param>
        /// <returns>ハッシュ値（文字列）</returns>
        public static string GetHashString(string sourceString, EnumHashAlgorithm eha, int stretchCount)
        {
            return CustomEncode.ToBase64String(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8), eha, stretchCount));
        }

        #endregion

        #region Bytes

        /// <summary>バイト配列のハッシュ値を計算して返す。</summary>
        /// <param name="asb">バイト配列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetHashBytes(byte[] asb, EnumHashAlgorithm eha)
        {
            // overloadへ
            return GetHash.GetHashBytes(asb, eha, 1);
        }

        /// <summary>バイト配列のハッシュ値を計算して返す。</summary>
        /// <param name="asb">バイト配列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="stretchCount">ストレッチ回数</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetHashBytes(byte[] asb, EnumHashAlgorithm eha, int stretchCount)
        {
            // ハッシュ（キー無し）サービスプロバイダを生成
            HashAlgorithm ha = GetHash.CreateHashAlgorithmServiceProvider(eha);

            // ハッシュ値を計算して返す。
            byte[] temp = ha.ComputeHash(asb);

            for (int i = 0; i < stretchCount; i++)
            {
                // stretchCountが1以上なら繰り返す。
                temp = ha.ComputeHash(temp);
            }

            ha.Clear(); // devps(1725)
            return temp;
        }

        #endregion

        #endregion

        #region 内部関数

        /// <summary>ハッシュ（キー無し）サービスプロバイダの生成</summary>
        /// <param name="eha">ハッシュ（キー無し）サービスプロバイダの列挙型</param>
        /// <returns>ハッシュ（キー無し）サービスプロバイダ</returns>
        private static HashAlgorithm CreateHashAlgorithmServiceProvider(EnumHashAlgorithm eha)
        {
            // ハッシュ（キー無し）サービスプロバイダ
            HashAlgorithm ha = null;

            if (eha == EnumHashAlgorithm.Default)
            {
                // 既定の暗号化サービスプロバイダ
                ha = HashAlgorithm.Create(); // devps(1703)
            }
            else if (eha == EnumHashAlgorithm.MD5CryptoServiceProvider)
            {
                // MD5CryptoServiceProviderサービスプロバイダ
                ha = MD5CryptoServiceProvider.Create(); // devps(1703)
            }
            else if (eha == EnumHashAlgorithm.SHA1CryptoServiceProvider)
            {
                // SHA1CryptoServiceProviderサービスプロバイダ
                ha = SHA1CryptoServiceProvider.Create(); // devps(1703)
            }
            else if (eha == EnumHashAlgorithm.SHA1Managed)
            {
                // SHA1Managedサービスプロバイダ
                ha = SHA1Managed.Create(); // devps(1703)
            }
            else if (eha == EnumHashAlgorithm.SHA256Managed)
            {
                // SHA256Managedサービスプロバイダ
                ha = SHA256Managed.Create(); // devps(1703)
            }
            else if (eha == EnumHashAlgorithm.SHA384Managed)
            {
                // SHA384Managedサービスプロバイダ
                ha = SHA384Managed.Create(); // devps(1703)
            }
            else if (eha == EnumHashAlgorithm.SHA512Managed)
            {
                // SHA512Managedサービスプロバイダ
                ha = SHA512Managed.Create(); // devps(1703)
            }

            return ha;
        }

        #endregion
    }

    #endregion
}
