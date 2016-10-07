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
//* クラス名        ：EmbeddedResourceLoader
//* クラス日本語名  ：[埋め込まれたリソース ファイル]の読み込みクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/05  西野  大介        新規作成（XBAP、ClickOnce対応）
//*  2009/04/20  西野  大介        XMLロード時のエンコーディング指定を不要に
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#4 ： 対象ファイルが存在しない場合、NullReference
//*  2011/05/18  西野  大介        Azure対応（アセンブリの取得方法の変更）
//*  2012/07/20  西野  大介        Azureで動作しない状態になっていたので修正
//*  2012/10/23  西野  大介        Azureで別のアセンブリからロード可能に修正
//**********************************************************************************

// Reflection
// 埋め込まれたリソース ファイルの読み込みのため。
using System.Reflection;

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

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>[埋め込まれたリソース ファイル]の読み込みクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public static class EmbeddedResourceLoader
    {
        /// <summary>[埋め込まれたリソース ファイル]を含むアセンブリを取得する</summary>
        /// <param name="assemblyString">
        /// アセンブリ名
        /// http://msdn.microsoft.com/ja-jp/library/k8xx4k69.aspx
        /// </param>
        /// <returns>Assembly</returns>
        private static Assembly GetEntryAssembly(string assemblyString)
        {
            // Azureスイッチ
            string azure = GetConfigParameter.GetConfigValue("Azure");

            if (string.IsNullOrEmpty(azure))
            {
                // 通常時
                if (string.IsNullOrEmpty(assemblyString))
                {
                    // assemblyString 指定なし
                    return Assembly.GetEntryAssembly();
                }
                else
                {
                    // assemblyString 指定あり
                    return Assembly.Load(assemblyString);
                }
            }
            else
            {
                // Azureスイッチ・オンの場合
                if (string.IsNullOrEmpty(assemblyString))
                {
                    // assemblyString 指定なし
                    return Assembly.Load(azure);
                }
                else
                {
                    // assemblyString 指定あり
                    return Assembly.Load(assemblyString);
                }

            }
        }

        // #4-start

        #region 存在チェック

        /// <summary>存在チェックのみのメソッド</summary>
        /// <param name="loadfileName">[埋め込まれたリソース ファイル]の名前（名前空間付き）</param>
        /// <param name="throwException">存在しない場合例外をスローするかどうかを指定</param>
        /// <returns>存在する：true、存在しない：false</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool Exists(string loadfileName, bool throwException)
        {
            // オーバーロードを[assemblyString = string.Empty]で呼ぶ。
            return EmbeddedResourceLoader.Exists(string.Empty, loadfileName, throwException);
        }

        /// <summary>存在チェックのみのメソッド</summary>
        /// <param name="assemblyString">
        /// アセンブリ名
        /// http://msdn.microsoft.com/ja-jp/library/k8xx4k69.aspx
        /// </param>
        /// <param name="loadfileName">[埋め込まれたリソース ファイル]の名前（名前空間付き）</param>
        /// <param name="throwException">存在しない場合例外をスローするかどうかを指定</param>
        /// <returns>存在する：true、存在しない：false</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool Exists(string assemblyString, string loadfileName, bool throwException)
        {
            // エントリのアセンブリ
            Assembly thisAssembly = null;
            ManifestResourceInfo manifestResourceInfo = null;

            // 例外処理
            try
            {
                thisAssembly = EmbeddedResourceLoader.GetEntryAssembly(assemblyString);
                manifestResourceInfo = thisAssembly.GetManifestResourceInfo(loadfileName);
            }
            catch (Exception)
            {
                // なにもしない。
            }

            // 存在チェック
            if (manifestResourceInfo != null)
            {
                if (0 != (manifestResourceInfo.ResourceLocation
                & (ResourceLocation.ContainedInManifestFile | ResourceLocation.Embedded)))
                {
                    // 存在する。
                    return true;
                }
            }

            // 存在しない。
            if (throwException)
            {
                throw new ArgumentException(String.Format(
                    PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfileName));
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region [埋め込まれたリソース ファイル]から文字列を読込

        /// <summary>[埋め込まれたリソース ファイル]から文字列を読み込む。</summary>
        /// <param name="loadfileName">[埋め込まれたリソース ファイル]の名前（名前空間付き）</param>
        /// <param name="enc">エンコード</param>
        /// <returns>[埋め込まれたリソース ファイル]から読み込んだ文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string LoadAsString(string loadfileName, Encoding enc)
        {
            // オーバーロードを[assemblyString = string.Empty]で呼ぶ。
            return EmbeddedResourceLoader.LoadAsString(string.Empty, loadfileName, enc);
        }

        /// <summary>[埋め込まれたリソース ファイル]から文字列を読み込む。</summary>
        /// <param name="assemblyString">
        /// アセンブリ名
        /// http://msdn.microsoft.com/ja-jp/library/k8xx4k69.aspx
        /// </param>
        /// <param name="loadfileName">[埋め込まれたリソース ファイル]の名前（名前空間付き）</param>
        /// <param name="enc">エンコード</param>
        /// <returns>[埋め込まれたリソース ファイル]から読み込んだ文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string LoadAsString(string assemblyString, string loadfileName, Encoding enc)
        {
            // エントリのアセンブリ
            Assembly thisAssembly = null;
            ManifestResourceInfo manifestResourceInfo = null;

            // ストリーム
            StreamReader sr = null;

            // 例外処理
            try
            {
                thisAssembly = EmbeddedResourceLoader.GetEntryAssembly(assemblyString);
                manifestResourceInfo = thisAssembly.GetManifestResourceInfo(loadfileName);
            }
            catch (Exception)
            {
                // なにもしない。
            }

            try
            {
                // 存在チェック
                if (manifestResourceInfo != null)
                {
                    if (0 != (manifestResourceInfo.ResourceLocation
                    & (ResourceLocation.ContainedInManifestFile | ResourceLocation.Embedded)))
                    {
                        // 存在する。
                    }
                    else
                    {
                        // 存在しない。
                        throw new ArgumentException(String.Format(
                            PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfileName));
                    }
                }
                else
                {
                    // 存在しない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfileName));
                }

                // 開く
                sr = new StreamReader(thisAssembly.GetManifestResourceStream(loadfileName), enc);

                // 読む
                return sr.ReadToEnd();
            }
            finally
            {
                // nullチェック
                if (sr == null)
                {
                    // 何もしない。
                }
                else
                {
                    // 閉じる
                    sr.Close();
                }
            }
        }

        #endregion

        #region [埋め込まれたリソースXMLファイル]から文字列を読込

        /// <summary>[埋め込まれたリソースXMLファイル]から文字列を読み込む。</summary>
        /// <param name="loadfileName">[埋め込まれたリソースXMLファイル]の名前（名前空間付き）</param>
        /// <returns>[埋め込まれたリソースXMLファイル]から読み込んだ文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string LoadXMLAsString(string loadfileName)
        {
            // オーバーロードを[assemblyString = string.Empty]で呼ぶ。
            return EmbeddedResourceLoader.LoadXMLAsString(string.Empty, loadfileName);
        }

        /// <summary>[埋め込まれたリソースXMLファイル]から文字列を読み込む。</summary>
        /// <param name="assemblyString">
        /// アセンブリ名
        /// http://msdn.microsoft.com/ja-jp/library/k8xx4k69.aspx
        /// </param>
        /// <param name="loadfileName">[埋め込まれたリソースXMLファイル]の名前（名前空間付き）</param>
        /// <returns>[埋め込まれたリソースXMLファイル]から読み込んだ文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string LoadXMLAsString(string assemblyString, string loadfileName)
        {
            // エントリのアセンブリ
            Assembly thisAssembly = null;
            ManifestResourceInfo manifestResourceInfo = null;

            // ストリーム
            StreamReader sr = null;
            // リーダ（XML）
            XmlTextReader xtr = null;

            // 例外処理
            try
            {
                thisAssembly = EmbeddedResourceLoader.GetEntryAssembly(assemblyString);
                manifestResourceInfo = thisAssembly.GetManifestResourceInfo(loadfileName);
            }
            catch (Exception)
            {
                // なにもしない。
            }

            try
            {
                // 存在チェック
                if (manifestResourceInfo != null)
                {
                    if (0 != (manifestResourceInfo.ResourceLocation
                    & (ResourceLocation.ContainedInManifestFile | ResourceLocation.Embedded)))
                    {
                        // 存在する。
                    }
                    else
                    {
                        // 存在しない。
                        throw new ArgumentException(String.Format(
                            PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfileName));
                    }
                }
                else
                {
                    // 存在しない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfileName));
                }

                // 既定のエンコーディングでロードして、
                sr = new StreamReader(thisAssembly.GetManifestResourceStream(loadfileName));

                // XML宣言を取得して、
                // <?xml version="1.0" encoding="xxxx" ?>
                string xmlDeclaration = sr.ReadLine();
                sr.Close();

                // エンコーディング オブジェクトの取得
                Encoding enc;

                try
                {
                    // エンコーディング文字列を取得し、
                    string searchString = "encoding=\"";
                    int start = xmlDeclaration.IndexOf(searchString, 0) + searchString.Length;
                    int end = xmlDeclaration.IndexOf('\"', start);

                    // エンコーディング オブジェクトに変換
                    enc = Encoding.GetEncoding(xmlDeclaration.Substring(start, end - start));
                }
                catch(Exception)
                {
                    // ここでエラーとなった場合、
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.XML_DECLARATION_ERROR, xmlDeclaration));
                }

                // 指定のエンコーディングで再ロード
                sr = new StreamReader(thisAssembly.GetManifestResourceStream(loadfileName), enc);

                // 読む
                return sr.ReadToEnd();
            }
            finally
            {
                // nullチェック
                if (xtr == null)
                {
                    // 何もしない。
                }
                else
                {
                    // 閉じる
                    xtr.Close();
                }

                // nullチェック
                if (sr == null)
                {
                    // 何もしない。
                }
                else
                {
                    // 閉じる
                    sr.Close();
                }
            }
        }

        #endregion

        // #4-end
    }


}
