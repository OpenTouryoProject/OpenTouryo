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
//*  2013/02/15  西野  大介        新規作成
//*  2014/03/13  西野  大介        devps(1703):Createメソッドを使用してcryptoオブジェクトを作成します。
//*  2014/03/13  西野  大介        devps(1725):暗号クラスの使用終了時にデータをクリアする。
//**********************************************************************************

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using System.Web.Security;
using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Util
{
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

    /// <summary>ハッシュを取得するクラス</summary>
    public class GetHash
    {
        /// <summary>パスワードをDB保存する際には塩味パスワードとして保存する。</summary>
        /// <param name="rawPasswd">ユーザが入力した生のパスワード</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="saltLength">ソルトの文字列長</param>
        /// <returns>塩味パスワード</returns>
        public static string GetSaltedPasswd(string rawPasswd, EnumHashAlgorithm eha, int saltLength)
        {
            // ランダム・ソルト文字列を生成（区切り記号は含まなくても良い）
            string salt = Membership.GeneratePassword(saltLength, 0);
            // 塩味パスワード（文字列）を生成して返す。
            return salt + GetHash.GetHashString(salt + rawPasswd, eha);

            // バイト配列仕様は、フィールドが文字列の可能性が高いので辞めた。
        }

        /// <summary>パスワードを比較して認証する。</summary>
        /// <param name="rawPasswd">ユーザが入力した生のパスワード</param>
        /// <param name="saltedPasswd">塩味パスワード</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="saltLength">ソルトの文字列長</param>
        /// <returns>
        /// true：パスワードは一致した。
        /// false：パスワードは一致しない。
        /// </returns>
        public static bool EqualSaltedPasswd(string rawPasswd, string saltedPasswd, EnumHashAlgorithm eha, int saltLength)
        {
            // ソルト部分を取得
            string salt = saltedPasswd.Substring(0, saltLength);

            // 引数のsaltedPasswdと、rawPasswdから自作したsaltedPasswdを比較
            if (saltedPasswd == salt + GetHash.GetHashString(salt + rawPasswd, eha))
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

        /// <summary>文字列のハッシュ値を計算して返す。</summary>
        /// <param name="sourceString">文字列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>ハッシュ値（文字列）</returns>
        public static string GetHashString(string sourceString, EnumHashAlgorithm eha)
        {
            // ハッシュ（Base64）
            return CustomEncode.ToBase64String(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8), eha));
        }

        /// <summary>バイト配列のハッシュ値を計算して返す。</summary>
        /// <param name="asb">バイト配列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetHashBytes(byte[] asb, EnumHashAlgorithm eha)
        {
            // ハッシュ（キー無し）サービスプロバイダを生成
            HashAlgorithm ha = GetHash.CreateHashAlgorithmServiceProvider(eha);

            // ハッシュ値を計算して返す。
            byte[] temp = ha.ComputeHash(asb);
            ha.Clear(); // devps(1725)
            return temp;
        }

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
}
