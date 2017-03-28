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
//* クラス名        ：GetPassword
//* クラス日本語名  ：RNGCryptoServiceProviderを使ったパスワード生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>GetPassword</summary>
    public class GetPassword
    {
        /// <summary>Base64Secretを生成</summary>
        /// <param name="byteSize">サイズ（バイト）</param>
        /// <returns>Base64Secret</returns>
        public static string Base64Secret(int byteSize)
        {
            return CustomEncode.ToBase64String(GetPassword.RandomByte(byteSize));
        }

        /// <summary>Base64UrlSecretを生成</summary>
        /// <param name="byteSize">サイズ（バイト）</param>
        /// <returns>Base64UrlSecret</returns>
        public static string Base64UrlSecret(int byteSize)
        {
            return CustomEncode.ToBase64UrlString(GetPassword.RandomByte(byteSize));
        }

        /// <summary>RandomByte</summary>
        /// <param name="byteSize">Byteサイズ</param>
        /// <returns>RandomByte</returns>
        public static byte[] RandomByte(int byteSize)
        {
            byte[] data = new byte[byteSize];
            RNGCryptoServiceProvider.Create().GetBytes(data);
            return data;
        }

        /// <summary>PasswordをGenerate</summary>
        /// <param name="length">長さ（1-127）</param>
        /// <param name="numberOfNonAlphanumericCharacters">Alphanumeric以外の文字数(0-length)</param>
        /// <returns>Password</returns>
        public static string Generate(int length, int numberOfNonAlphanumericCharacters)
        {
            // 以下を参考に実装したが、元のコードにバグがあったので修正も含めている。
            // ASP.NETを使ってランダムなパスワードを生成する － インターネットコム
            // https://internetcom.jp/developer/20060131/26.html

            RNGCryptoServiceProvider rng = null;

            if ((length < 1) || (length > 128))
            {
                throw new ArgumentException("Password length is incorrect.");
            }

            if ((numberOfNonAlphanumericCharacters > length) || (numberOfNonAlphanumericCharacters < 0))
            {
                throw new ArgumentException("Minimum required non alphanumeric character count is incorrect.");
            }

            byte[] bufferPwd = new byte[length];
            byte[] bufferSelectIdx = new byte[numberOfNonAlphanumericCharacters];
            byte[] bufferSelectChar = new byte[numberOfNonAlphanumericCharacters];

            // chPassword contains the password's characters as it's built up
            char[] chPassword = new char[length];

            // chPunctionations contains the list of legal non-alphanumeric characters
            char[] chPunctuations = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".ToCharArray(); //"!@@$%^^*()_-+=[{]};:>|./?".ToCharArray();

            // Get a cryptographically strong series of bytes
            rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bufferPwd);

            // C# ASCII Table - Dot Net Perls
            // https://www.dotnetperls.com/ascii-table

            for (int i = 0; i < length; i++)
            {
                // Convert each byte into its representative character
                // 何故 87 ? ---> 62 の間違い（ 0~9(10) + A~Z(26) + a~z(26) = 62 ）
                uint rndChr = (bufferPwd[i] % (uint)62); // 0-61(の62文字)

                if ((rndChr < 10))
                {
                    // 48 + (0~9) = 48~57 => charで、0~9を表す。
                    chPassword[i] = Convert.ToChar(Convert.ToUInt16(48 + rndChr));
                }
                else
                {
                    if ((rndChr < 36))
                    {
                        // 55 + (10~35) = 65~90 => charで、A~Zを表す。
                        chPassword[i] = Convert.ToChar(Convert.ToUInt16(55 + rndChr));
                    }
                    else
                    {
                        // 61 + (36~61) = 97~122 => charで、a~zを表す。
                        chPassword[i] = Convert.ToChar(Convert.ToUInt16(61 + rndChr));
                    }
                }
            }

            // 0-lengthの位置に、適当なNAC文字を入れる。

            // Get a cryptographically strong series of bytes
            rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bufferSelectIdx); // index選択用
            rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bufferSelectChar); // char選択用

            List<char> list = new List<char>();
            list.AddRange(chPunctuations); // .Contains() を使いたい。

            for (int i = 0; i < numberOfNonAlphanumericCharacters; i++)
            {
                uint selectIdx = (bufferSelectIdx[i] % (uint)length); // index選択
                uint selectChar = (bufferSelectChar[i] % (uint)chPunctuations.Length); // char選択

                if (list.Contains(chPassword[selectIdx]))
                {
                    // 既にalphabet以外が入っている場合、先頭から埋める。
                    for (int j = 0; j < length; j++)
                    {
                        if (list.Contains(chPassword[j]))
                        {
                            // 既にalphabet以外が入っている。
                        }
                        else
                        {
                            chPassword[j] = chPunctuations[selectChar];
                            break; // 忘れてた。
                        }
                    }
                }
                else
                {
                    chPassword[selectIdx] = chPunctuations[selectChar];
                }
            }

            return new string(chPassword);
        }
    }
}