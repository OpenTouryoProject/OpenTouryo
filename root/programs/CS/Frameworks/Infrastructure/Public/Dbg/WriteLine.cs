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
//* クラス名        ：WriteLine
//* クラス日本語名  ：WriteLineクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/12  西野 大介         新規作成（X PlatformのDebugで利用拡大？
//*  2018/11/29  西野 大介         ASP.NET Coreの出力で2つ出力されるので...
//**********************************************************************************

using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Dbg
{
    /// <summary>WriteLineクラス</summary>
    public class WriteLine
    {
        #region Security

        #region JWT

        /// <summary>JwtInspector</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jwt">string</param>
        public static void JwtInspector(string testLabel, string jwt)
        {
            string[] aryStr = jwt.Split('.');

            if (aryStr.Length == 3)
            {
                WriteLine.JwsInspector(testLabel, aryStr);
            }
            else if (aryStr.Length == 5)
            {
                WriteLine.JweInspector(testLabel, aryStr);
            }
            else { }
        }

        #region JWS

        /// <summary>JwsInspector</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jws">string[]</param>
        public static void JwsInspector(string testLabel, string[] jws)
        {
            WriteLine.OutPutDebugAndConsole(testLabel, "JWS Header: " + 
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jws[0]), CustomEncode.UTF_8));

            WriteLine.OutPutDebugAndConsole(testLabel, "JWS Payload: " +
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jws[1]), CustomEncode.UTF_8));

            WriteLine.OutPutDebugAndConsole(testLabel, "JWS Signature: " + jws[2]);
        }

        #endregion

        #region JWE

        /// <summary>JweInspector</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jwe">string[]</param>
        public static void JweInspector(string testLabel, string[] jwe)
        {
            WriteLine.OutPutDebugAndConsole(testLabel, "JWE Header: " +
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwe[0]), CustomEncode.UTF_8));

            WriteLine.OutPutDebugAndConsole(testLabel, "JWE Encrypted Key: " + jwe[1]);
            WriteLine.OutPutDebugAndConsole(testLabel, "JWE Initialization Vector: " + jwe[2]);
            WriteLine.OutPutDebugAndConsole(testLabel, "JWE Ciphertext: " + jwe[3]);
            WriteLine.OutPutDebugAndConsole(testLabel, "JWE Authentication Tag: " + jwe[4]);
        }

        #endregion

        #endregion

        #region X.509

        /// <summary>PrivateX509KeyInspector</summary>
        /// <param name="testLabel">string</param>
        /// <param name="privateX509Key">X509Certificate2</param>
        public static void PrivateX509KeyInspector(string testLabel, X509Certificate2 privateX509Key)
        {
            // X509Certificate2.PrivateKey
            WriteLine.OutPutDebugAndConsole(
                testLabel, "X509Certificate2.PrivateKey: "
                + (privateX509Key == null ? "is null" : "is not null"));

            if (privateX509Key == null)
            {
                return;
            }

            // SignatureAlgorithm.FriendlyName
            try
            {
                WriteLine.OutPutDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + privateX509Key.SignatureAlgorithm.FriendlyName);
            }
            catch (Exception ex)
            {
                WriteLine.OutPutDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + ex.GetType().ToString() + ", " + ex.Message);
            }

            // PrivateKey.ToString()
            try
            {
                if (privateX509Key.HasPrivateKey)
                {
                    WriteLine.OutPutDebugAndConsole(
                        testLabel, "PrivateKey.ToString(): "
                        + (privateX509Key.PrivateKey == null ? "is null" : "is "
                        + privateX509Key.PrivateKey.ToString()));
                }
            }
            catch (Exception ex)
            {
                WriteLine.OutPutDebugAndConsole(
                    testLabel, "PrivateKey.ToString(): " 
                    + ex.GetType().ToString() + ", " + ex.Message);
            }
        }

        /// <summary>PublicX509KeyInspector</summary>
        /// <param name="testLabel">string</param>
        /// <param name="publicX509Key">X509Certificate2</param>
        public static void PublicX509KeyInspector(string testLabel, X509Certificate2 publicX509Key)
        {
            // X509Certificate2.PublicKey
            WriteLine.OutPutDebugAndConsole(
                testLabel, "publicX509Key: " 
                + (publicX509Key == null ? "is null" : "is not null"));
            if (publicX509Key == null)
            {
                return;
            }

            // SignatureAlgorithm.FriendlyName
            try
            {
                WriteLine.OutPutDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + publicX509Key.SignatureAlgorithm.FriendlyName);
            }
            catch (Exception ex)
            {
                WriteLine.OutPutDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + ex.GetType().ToString() + ", " + ex.Message);
            }

            // PublicKey.ToString()
            if (publicX509Key.PublicKey != null)
            {
                WriteLine.OutPutDebugAndConsole(
                    testLabel, "PublicKey.ToString(): " 
                    + (publicX509Key.PublicKey == null ? "is null" : "is " 
                    + publicX509Key.PublicKey.ToString()));

                // PublicKey.Key.ToString()
                try
                {
                    if (publicX509Key.PublicKey.Key != null)
                    {
                        WriteLine.OutPutDebugAndConsole(
                            testLabel, "PublicKey.Key.ToString(): " 
                            + (publicX509Key.PublicKey.Key == null ? "is null" : "is " 
                            + publicX509Key.PublicKey.Key.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    WriteLine.OutPutDebugAndConsole(
                        testLabel, "PublicKey.Key.ToString(): " 
                        + ex.GetType().ToString() + ", " + ex.Message);
                }
            }
        }

        #endregion

        #endregion

        #region OutPutDebugAndConsole
        /// <summary>OutPutDebugAndConsole</summary>
        /// <param name="testLabel">string</param>
        /// <param name="s">string</param>
        public static void OutPutDebugAndConsole(string testLabel, string s)
        {
            OutPutDebugAndConsole(testLabel + " > " + s);
        }

        /// <summary>EnableOutPutToConsole</summary>
        public static bool EnableOutPutToConsole = true;
        /// <summary>EnableOutPutToDebug</summary>
        public static bool EnableOutPutToDebug = true;

        /// <summary>OutPutDebugAndConsole</summary>
        /// <param name="s">string</param>
        public static void OutPutDebugAndConsole(string s)
        {
            if (EnableOutPutToConsole) Console.WriteLine(s);
            if (EnableOutPutToDebug) Debug.WriteLine(s);
        }
        #endregion
    }
}
