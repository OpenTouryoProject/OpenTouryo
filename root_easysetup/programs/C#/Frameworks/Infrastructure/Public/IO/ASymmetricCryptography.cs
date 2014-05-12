//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：ASymmetricCryptography
//* クラス日本語名  ：非対称アルゴリズムによる暗号化・復号化
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野  大介        新規作成
//*  2013/02/11  西野  大介        クラス名の変更（Unsymmetric→Asymmetric）
//*  2013/02/18  西野  大介        CustomEncode使用に統一
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

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.IO
{
    // 一種類しかないのでEnum不要

    ///// <summary>
    ///// 対称アルゴリズムによる
    ///// 暗号化サービスプロバイダの種類
    ///// </summary>
    //public enum EnumASymmetricAlgorithm
    //{
    //    /// <summary>RSACryptoServiceProvider</summary>
    //    RSACryptoServiceProvider
    //};

    /// <summary>下位互換のため</summary>
    public class UnSymmetricCryptography : ASymmetricCryptography { }

    /// <summary>非対称アルゴリズムによる暗号化・復号化クラス</summary>
    /// <remarks>
    /// 自由に利用できる。
    /// </remarks>
    public class ASymmetricCryptography
    {
        /// <summary>文字列を暗号化する</summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化された文字列</returns>
        public static string EncryptString(string sourceString, string publicKey)
        {
            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] source = CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8);

            // 暗号化（Base64）
            return CustomEncode.ToBase64String(
                ASymmetricCryptography.EncryptBytes(source, publicKey));
        }

        /// <summary>バイト配列を暗号化する</summary>
        /// <param name="source">暗号化するバイト配列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <returns>非対称アルゴリズムで暗号化されたバイト配列</returns>
        public static byte[] EncryptBytes(byte[] source, string publicKey)
        {
            // RSACryptoServiceProviderオブジェクトの作成
            RSACryptoServiceProvider rsa
                = (RSACryptoServiceProvider)RSACryptoServiceProvider.Create(); // devps(1703)
            
            // 公開鍵
            rsa.FromXmlString(publicKey);

            // 暗号化する
            byte[] temp = rsa.Encrypt(source, false);
            rsa.Clear(); // devps(1725)
            return temp;
        }

        /// <summary>暗号化された文字列を復号化する</summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化された文字列</returns>
        public static string DecryptString(string sourceString, string privateKey)
        {
            // 暗号化文字列をbyte型配列に変換する（Base64）
            byte[] source = CustomEncode.FromBase64String(sourceString);

            // 復号化（UTF-8 Enc）
            return CustomEncode.ByteToString(
                ASymmetricCryptography.DecryptBytes(source, privateKey), CustomEncode.UTF_8);
        }

        /// <summary>暗号化されたバイト配列を復号化する</summary>
        /// <param name="source">暗号化されたバイト配列</param>
        /// <param name="privateKey">復号化に使用する秘密鍵</param>
        /// <returns>非対称アルゴリズムで復号化されたバイト配列</returns>
        public static byte[] DecryptBytes(byte[] source, string privateKey)
        {
            // RSACryptoServiceProviderオブジェクトの作成
            RSACryptoServiceProvider rsa 
                = (RSACryptoServiceProvider)RSACryptoServiceProvider.Create(); // devps(1703)

            // 秘密鍵
            rsa.FromXmlString(privateKey);

            // 復号化
            byte[] temp = rsa.Decrypt(source, false);
            rsa.Clear(); // devps(1725)
            return temp;
        }

        /// <summary>秘密鍵と公開鍵を取得する。</summary>
        /// <param name="publicKey">公開鍵</param>
        /// <param name="privateKey">秘密鍵</param>
        public static void GetKeys(out string publicKey, out string privateKey)
        {
            // RSACryptoServiceProviderオブジェクトの作成
            RSA rsa = RSACryptoServiceProvider.Create(); // devps(1703)

            // 公開鍵をXML形式で取得
            publicKey = rsa.ToXmlString(false);
            // 秘密鍵をXML形式で取得
            privateKey = rsa.ToXmlString(true);

            rsa.Clear(); // devps(1725)
        }
    }
}
