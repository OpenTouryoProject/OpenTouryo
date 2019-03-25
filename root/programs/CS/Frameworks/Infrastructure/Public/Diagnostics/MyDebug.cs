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
//* クラス名        ：MyDebug
//* クラス日本語名  ：MyDebugクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/12  西野 大介         新規作成（X PlatformのDebugで利用拡大？
//*  2018/11/29  西野 大介         ASP.NET Coreの出力で2つ出力されるので...
//*  2019/03/20  西野 大介         リネーム（名前空間、クラス名、メソッド名）
//**********************************************************************************

using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Diagnostics
{
    /// <summary>MyDebugクラス</summary>
    public class MyDebug
    {
        #region Security

        #region JWT

        /// <summary>InspectJwt</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jwt">string</param>
        public static void InspectJwt(string testLabel, string jwt)
        {
            string[] aryStr = jwt.Split('.');

            if (aryStr.Length == 3)
            {
                MyDebug.InspectJws(testLabel, aryStr);
            }
            else if (aryStr.Length == 5)
            {
                MyDebug.InspectJwe(testLabel, aryStr);
            }
            else { }
        }

        #region JWS

        /// <summary>InspectJws</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jws">string[]</param>
        public static void InspectJws(string testLabel, string[] jws)
        {
            MyDebug.OutputDebugAndConsole(testLabel, "JWS Header: " + 
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jws[0]), CustomEncode.UTF_8));

            MyDebug.OutputDebugAndConsole(testLabel, "JWS Payload: " +
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jws[1]), CustomEncode.UTF_8));

            MyDebug.OutputDebugAndConsole(testLabel, "JWS Signature: " + jws[2]);
        }

        #endregion

        #region JWE

        /// <summary>InspectJwe</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jwe">string[]</param>
        public static void InspectJwe(string testLabel, string[] jwe)
        {
            MyDebug.OutputDebugAndConsole(testLabel, "JWE Header: " +
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwe[0]), CustomEncode.UTF_8));

            MyDebug.OutputDebugAndConsole(testLabel, "JWE Encrypted Key: " + jwe[1]);
            MyDebug.OutputDebugAndConsole(testLabel, "JWE Initialization Vector: " + jwe[2]);
            MyDebug.OutputDebugAndConsole(testLabel, "JWE Ciphertext: " + jwe[3]);
            MyDebug.OutputDebugAndConsole(testLabel, "JWE Authentication Tag: " + jwe[4]);
        }

        #endregion

        #endregion

        #region X.509

        /// <summary>InspectPrivateX509Key</summary>
        /// <param name="testLabel">string</param>
        /// <param name="privateX509Key">X509Certificate2</param>
        public static void InspectPrivateX509Key(string testLabel, X509Certificate2 privateX509Key)
        {
            // X509Certificate2.PrivateKey
            MyDebug.OutputDebugAndConsole(
                testLabel, "X509Certificate2.PrivateKey: "
                + (privateX509Key == null ? "is null" : "is not null"));

            if (privateX509Key == null)
            {
                return;
            }

            // SignatureAlgorithm.FriendlyName
            try
            {
                MyDebug.OutputDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + privateX509Key.SignatureAlgorithm.FriendlyName);
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + ex.GetType().ToString() + ", " + ex.Message);
            }

            // PrivateKey.ToString()
            try
            {
                if (privateX509Key.HasPrivateKey)
                {
                    MyDebug.OutputDebugAndConsole(
                        testLabel, "PrivateKey.ToString(): "
                        + (privateX509Key.PrivateKey == null ? "is null" : "is "
                        + privateX509Key.PrivateKey.ToString()));
                }
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole(
                    testLabel, "PrivateKey.ToString(): " 
                    + ex.GetType().ToString() + ", " + ex.Message);
            }
        }

        /// <summary>InspectPublicX509Key</summary>
        /// <param name="testLabel">string</param>
        /// <param name="publicX509Key">X509Certificate2</param>
        public static void InspectPublicX509Key(string testLabel, X509Certificate2 publicX509Key)
        {
            // X509Certificate2.PublicKey
            MyDebug.OutputDebugAndConsole(
                testLabel, "publicX509Key: " 
                + (publicX509Key == null ? "is null" : "is not null"));
            if (publicX509Key == null)
            {
                return;
            }

            // SignatureAlgorithm.FriendlyName
            try
            {
                MyDebug.OutputDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + publicX509Key.SignatureAlgorithm.FriendlyName);
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole(
                    testLabel, "SignatureAlgorithm.FriendlyName: "
                    + ex.GetType().ToString() + ", " + ex.Message);
            }

            // PublicKey.ToString()
            if (publicX509Key.PublicKey != null)
            {
                MyDebug.OutputDebugAndConsole(
                    testLabel, "PublicKey.ToString(): " 
                    + (publicX509Key.PublicKey == null ? "is null" : "is " 
                    + publicX509Key.PublicKey.ToString()));

                // PublicKey.Key.ToString()
                try
                {
                    if (publicX509Key.PublicKey.Key != null)
                    {
                        MyDebug.OutputDebugAndConsole(
                            testLabel, "PublicKey.Key.ToString(): " 
                            + (publicX509Key.PublicKey.Key == null ? "is null" : "is " 
                            + publicX509Key.PublicKey.Key.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    MyDebug.OutputDebugAndConsole(
                        testLabel, "PublicKey.Key.ToString(): " 
                        + ex.GetType().ToString() + ", " + ex.Message);
                }
            }
        }

        #endregion

        #endregion

        #region OutputDebugAndConsole
        /// <summary>OutputDebugAndConsole</summary>
        /// <param name="testLabel">string</param>
        /// <param name="s">string</param>
        public static void OutputDebugAndConsole(string testLabel, string s)
        {
            OutputDebugAndConsole(testLabel + " > " + s);
        }

        /// <summary>CanOutputToConsole</summary>
        public static bool CanOutputToConsole = true;

        /// <summary>CanOutputToDebug</summary>
        public static bool CanOutputToDebug = true;

        /// <summary>OutputDebugAndConsole</summary>
        /// <param name="s">string</param>
        public static void OutputDebugAndConsole(string s)
        {
            if (MyDebug.CanOutputToConsole) Console.WriteLine(s);
            if (MyDebug.CanOutputToDebug) Debug.WriteLine(s);
        }
        #endregion
    }
}
