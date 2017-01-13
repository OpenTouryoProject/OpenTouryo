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
//* クラス名        ：GetPassword
//* クラス日本語名  ：RNGCryptoServiceProviderを使ったパスワード生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野  大介        新規作成
//**********************************************************************************

// System
using System;
using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>GetPassword</summary>
    public class GetPassword
    {
        /// <summary>Generate</summary>
        /// <param name="length">長さ</param>
        /// <param name="numberOfNonAlphanumericCharacters">Alphanumeric以外の文字数</param>
        /// <returns>Password</returns>
        public static string Generate(int length, int numberOfNonAlphanumericCharacters)
        {
            if (((length < 1) || (length > 128)))
            {
                throw new ArgumentException("Password length is incorrect.");
            }

            if (((numberOfNonAlphanumericCharacters > length) || (numberOfNonAlphanumericCharacters < 0)))
            {
                throw new ArgumentException("Minimum required non alphanumeric character count is incorrect.");
            }

            while (true)
            {
                int i = 0;
                int nonANcount = 0;
                byte[] buffer1 = new byte[length];

                //chPassword contains the password's characters as it's built up
                char[] chPassword = new char[length];

                //chPunctionations contains the list of legal non-alphanumeric characters
                char[] chPunctuations = "!@@$%^^*()_-+=[{]};:>|./?".ToCharArray();

                //Get a cryptographically strong series of bytes
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(buffer1);

                for (i = 0; i <= length - 1; i++)
                {
                    //Convert each byte into its representative character
                    int rndChr = (buffer1[i] % 87);
                    if ((rndChr < 10))
                    {
                        chPassword[i] = Convert.ToChar(Convert.ToUInt16(48 + rndChr));
                    }
                    else
                    {
                        if ((rndChr < 36))
                        {
                            chPassword[i] = Convert.ToChar(Convert.ToUInt16((65 + rndChr) - 10));
                        }
                        else
                        {
                            if ((rndChr < 62))
                            {
                                chPassword[i] = Convert.ToChar(Convert.ToUInt16((97 + rndChr) - 36));
                            }
                            else
                            {
                                chPassword[i] = chPunctuations[rndChr - 62];
                                nonANcount += 1;
                            }
                        }
                    }
                }

                if (nonANcount < numberOfNonAlphanumericCharacters)
                {
                    Random rndNumber = new Random();
                    for (i = 0; i <= (numberOfNonAlphanumericCharacters - nonANcount) - 1; i++)
                    {
                        int passwordPos = 0;
                        do
                        {
                            passwordPos = rndNumber.Next(0, length);
                        } while (!char.IsLetterOrDigit(chPassword[passwordPos]));
                        chPassword[passwordPos] = chPunctuations[rndNumber.Next(0, chPunctuations.Length)];
                    }
                }

                return new string(chPassword);
            }
        }
    }
}