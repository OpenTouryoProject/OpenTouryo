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
//* クラス名        ：GetPasswordHashV2
//* クラス日本語名  ：Password Hashを取得するクラス（v2
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         クラスの再編（GetKeyedHash -> GetKeyedHash, GetPasswordHashVn, MsgAuthCode）
//**********************************************************************************

using System.Security.Cryptography;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Pwd
{
    /// <summary>Password Hashを取得するクラス（v2</summary>
    public class GetPasswordHashV2 : GetKeyedHash
    {
        #region Hash

        /// <summary>
        /// Password entered by the userをDB保存する際、
        /// Salted and hashed passwordとして保存する必要がある。
        /// </summary>
        /// <param name="rawPassword">>Password entered by the user.</param>
        /// <param name="eha">Hashアルゴリズム列挙型</param>
        /// <param name="saltLength">saltの文字列長</param>
        /// <returns>Salted and hashed password.</returns>
        /// <see ref="http://www.atmarkit.co.jp/ait/articles/1110/06/news154_2.html"/>
        public static string GetSaltedPassword(string rawPassword, EnumHashAlgorithm eha, int saltLength)
        {
            // overloadへ
            return GetPasswordHashV2.GetSaltedPassword(rawPassword, eha, saltLength, 1);
        }

        /// <summary>
        /// Password entered by the userをDB保存する際、
        /// Salted and hashed passwordとして保存する必要がある。
        /// </summary>
        /// <param name="rawPassword">>Password entered by the user.</param>
        /// <param name="eha">Hashアルゴリズム列挙型</param>
        /// <param name="saltLength">saltの文字列長</param>
        /// <param name="stretchCount">Stretch回数</param>
        /// <returns>Salted and hashed password.</returns>
        public static string GetSaltedPassword(string rawPassword, EnumHashAlgorithm eha, int saltLength, int stretchCount)
        {
            // ランダムsalt文字列を生成（区切り記号は含まなくても良い）
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
        /// <param name="eha">Hashアルゴリズム列挙型</param>
        /// <returns>
        /// true：パスワードは一致した。
        /// false：パスワードは一致しない。
        /// </returns>
        public static bool EqualSaltedPassword(string rawPassword, string saltedPassword, EnumHashAlgorithm eha)
        {
            // salt部分を取得
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

        #region KeyedHash

        /// <summary>
        /// Password entered by the userをDB保存する際、
        /// Salted and hashed passwordとして保存する必要がある。
        /// </summary>
        /// <param name="rawPassword">Password entered by the user.</param>
        /// <param name="ekha">Hashアルゴリズム列挙型</param>
        /// <param name="key">キー</param>
        /// <param name="saltLength">saltの文字列長</param>
        /// <param name="stretchCount">Stretch回数</param>
        /// <returns>Salted and hashed password.</returns>
        public static string GetSaltedPassword(string rawPassword,
            EnumKeyedHashAlgorithm ekha, string key, int saltLength, int stretchCount)
        {
            // overloadへ
            return GetPasswordHashV2.GetSaltedPassword(
                rawPassword, ekha, key, saltLength, stretchCount, HashAlgorithmName.SHA256);
        }

        /// <summary>
        /// Password entered by the userをDB保存する際、
        /// Salted and hashed passwordとして保存する必要がある。
        /// </summary>
        /// <param name="rawPassword">Password entered by the user.</param>
        /// <param name="ekha">Hashアルゴリズム列挙型</param>
        /// <param name="key">キー</param>
        /// <param name="saltLength">saltの文字列長</param>
        /// <param name="stretchCount">Stretch回数</param>
        /// <param name="hashAlgorithmName">Hashアルゴリズム名</param>
        /// <returns>Salted and hashed password.</returns>
        public static string GetSaltedPassword(string rawPassword,
            EnumKeyedHashAlgorithm ekha, string key, int saltLength, int stretchCount, HashAlgorithmName hashAlgorithmName)
        {
            // ランダムsalt文字列を生成（区切り記号は含まなくても良い）
            string salt = GetPassword.Generate(saltLength, 0); //Membership.GeneratePassword(saltLength, 0);
            byte[] saltByte = CustomEncode.StringToByte(salt, CustomEncode.UTF_8);

            // KeyedHashのキーを生成する。
            Rfc2898DeriveBytes passwordKey = new Rfc2898DeriveBytes(key, saltByte, stretchCount, hashAlgorithmName);

            // Salted and hashed password（文字列）を生成して返す。
            return

                // key
                CustomEncode.ToBase64String(CustomEncode.StringToByte(key, CustomEncode.UTF_8))

                // saltByte
                + "." + CustomEncode.ToBase64String(saltByte)

                // stretchCount
                + "." + CustomEncode.ToBase64String(CustomEncode.StringToByte(stretchCount.ToString(), CustomEncode.UTF_8))

                // Salted and hashed password
                + "." + CustomEncode.ToBase64String(
                    GetPasswordHashV2.GetKeyedHashBytes(
                        CustomEncode.StringToByte(salt + rawPassword, CustomEncode.UTF_8),
                        ekha, passwordKey.GetBytes(24)));
        }

        /// <summary>パスワードを比較して認証する。</summary>
        /// <param name="rawPassword">Password entered by the user.</param>
        /// <param name="saltedPassword">Salted and hashed password.</param>
        /// <param name="ekha">Hashアルゴリズム列挙型</param>
        /// <returns>
        /// true：パスワードは一致した。
        /// false：パスワードは一致しない。
        /// </returns>
        public static bool EqualSaltedPassword(string rawPassword, string saltedPassword, EnumKeyedHashAlgorithm ekha)
        {
            // overloadへ
            return EqualSaltedPassword(rawPassword, saltedPassword, ekha, HashAlgorithmName.SHA256);
        }

        /// <summary>パスワードを比較して認証する。</summary>
        /// <param name="rawPassword">Password entered by the user.</param>
        /// <param name="saltedPassword">Salted and hashed password.</param>
        /// <param name="ekha">Hashアルゴリズム列挙型</param>
        /// <param name="hashAlgorithmName">Hashアルゴリズム名</param>
        /// <returns>
        /// true：パスワードは一致した。
        /// false：パスワードは一致しない。
        /// </returns>
        public static bool EqualSaltedPassword(
            string rawPassword, string saltedPassword, EnumKeyedHashAlgorithm ekha, HashAlgorithmName hashAlgorithmName)
        {
            // salt部分を取得
            string[] temp = saltedPassword.Split('.');

            // key
            string key = CustomEncode.ByteToString(CustomEncode.FromBase64String(temp[0]), CustomEncode.UTF_8);

            // saltByte
            byte[] saltByte = CustomEncode.FromBase64String(temp[1]);
            
            // salt
            string salt = CustomEncode.ByteToString(saltByte, CustomEncode.UTF_8);
            
            // stretchCount
            int stretchCount = int.Parse(CustomEncode.ByteToString(CustomEncode.FromBase64String(temp[2]), CustomEncode.UTF_8));
            
            // Salted and hashed password
            string hashedPassword = temp[3];

            // KeyedHashのキーを生成する。
            Rfc2898DeriveBytes passwordKey = new Rfc2898DeriveBytes(key, saltByte, stretchCount, hashAlgorithmName);

            // 引数のsaltedPasswordと、rawPasswordから自作したsaltedPasswordを比較
            string compare = CustomEncode.ToBase64String(
                GetPasswordHashV2.GetKeyedHashBytes(
                    CustomEncode.StringToByte(salt + rawPassword, CustomEncode.UTF_8),
                    ekha, passwordKey.GetBytes(24)));

            if (hashedPassword == compare)
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
    }
}