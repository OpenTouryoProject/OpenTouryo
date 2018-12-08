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
//* クラス名        ：GetKeyedHash
//* クラス日本語名  ：ハッシュ（キー付き）を取得するクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         クラスの再編（GetKeyedHash -> GetKeyedHash, GetPasswordHashVn, MsgAuthCode）
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>ハッシュ（キー付き）を取得するクラス</summary>
    public class GetKeyedHash
    {
        /// <summary>文字列のハッシュ値を計算して返す。</summary>
        /// <param name="data">データ（文字列）</param>
        /// <param name="ekha">ハッシュ（キー付き）アルゴリズム列挙型</param>
        /// <param name="key">キー（文字列）</param>
        /// <returns>ハッシュ値（base64文字列）</returns>
        public static string GetKeyedHashString(string data, EnumKeyedHashAlgorithm ekha, string key)
        {
            return CustomEncode.ToBase64String(GetKeyedHashBytes(
                CustomEncode.StringToByte(data, CustomEncode.UTF_8),
                ekha, CustomEncode.StringToByte(key, CustomEncode.UTF_8)));
        }

        /// <summary>バイト配列のハッシュ値を計算して返す。</summary>
        /// <param name="data">データ（バイト配列）</param>
        /// <param name="ekha">ハッシュ（キー付き）アルゴリズム列挙型</param>
        /// <param name="key">キー（バイト配列）</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetKeyedHashBytes(byte[] data, EnumKeyedHashAlgorithm ekha, byte[] key)
        {
            // HMACMD5      ：どのサイズのキーでも受け入れる ◯
            // HMACRIPEMD160：どのサイズのキーでも受け入れる ◯
            // HMACSHA1     ：どのサイズのキーでも受け入れる ◯
            // HMACSHA256   ：どのサイズのキーでも受け入れる ◯
            // HMACSHA384   ：どのサイズのキーでも受け入れる ◯
            // HMACSHA512   ：どのサイズのキーでも受け入れる ◯
#if NETSTD
#else
            // MACTripleDES ：長さが 16 または 24 バイトのキーを受け入れる
            if (ekha == EnumKeyedHashAlgorithm.MACTripleDES)
            {
                if (24 <= key.Length)
                {
                    key = PubCmnFunction.ShortenByteArray(key, 24);
                }
                else if (16 <= key.Length)
                {
                    key = PubCmnFunction.ShortenByteArray(key, 16);
                }
                else
                {
                    throw new ArgumentException(
                        PublicExceptionMessage.ARGUMENT_INCORRECT, "byte[] key");
                }
            }
#endif

            // ハッシュ（キー付き）サービスプロバイダを生成
            KeyedHashAlgorithm kha = HashAlgorithmCmnFunc.
                CreateKeyedHashAlgorithmSP(ekha, key);

            // ハッシュ（キー付き）を生成して返す。
            byte[] temp = kha.ComputeHash(data);

            kha.Clear(); // devps(1725)

            return temp;
        }
    }
}