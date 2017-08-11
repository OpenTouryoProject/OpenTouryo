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
//* クラス名        ：GetMessage
//* クラス日本語名  ：メッセージ取得クラス
//*                   XMLファイルから、メッセージIDに対応するメッセージを取得する。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/03/13  西野 大介         新規作成
//*  2009/03/29  西野 大介         定義が無い場合、間違っている場合の処理を追加
//*  2009/04/20  西野 大介         埋め込まれたリソースXMLファイルのロード メソッドの変更
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#14 ： XMLチェック処理追加
//*                                ・#15 ： XML要素のリテラル化
//*  2010/09/24  西野 大介         ジェネリック対応（XMLのDictionary化）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2011/01/19  西野 大介         環境変数の組み込み処理に対応
//*  2014/02/07  Rituparna Biswas  Changes Made In GetMessageDescription for Internationalization
//*  2014/02/19  西野 大介         取り込み（細かな修正）
//*  2017/08/11  西野 大介         2014年のInternationalizationのrefactoringを実施
//**********************************************************************************


using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>メッセージ取得クラス</summary>
    public class GetMessage
    {
        /// <summary>Lock Object</summary>
        private static Object _lock = new Object();

        /// <summary>
        /// 国毎のディクショナリ
        /// Dictionary of each country
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>>
            DicMSG = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Get message description from message ID
        /// </summary>
        /// <param name="messageID">
        /// messageID
        /// </param>
        /// <returns>
        /// message description
        /// </returns>
        public static string GetMessageDescription(string messageID)
        {
            // FxBusinessMessageCulture
            string fxBusinessMessageCulture = GetConfigParameter.GetConfigValue(FxLiteral.BUSINESSMESSAGECULTUER);
            
            CultureInfo uiCulture = null;

            if (string.IsNullOrEmpty(fxBusinessMessageCulture))
            {
                uiCulture = Thread.CurrentThread.CurrentUICulture;
            }
            else
            {
                uiCulture = new CultureInfo(fxBusinessMessageCulture);
            }

            return GetMessage.GetMessageDescription(messageID, uiCulture);
        }

        /// <summary>
        /// Get message description from message ID
        /// </summary>
        /// <param name="messageID">
        /// messageID
        /// </param>
        /// <param name="uiCulture">
        /// uiCulture
        /// </param>
        /// <returns>
        /// Get message description
        /// </returns>
        public static string GetMessageDescription(string messageID, CultureInfo uiCulture)
        {
            // defaultFile
            string defaultFilePath = GetConfigParameter.GetConfigValue(FxLiteral.XML_MSG_DEFINITION);
            string defaultFileName = "";

            // culture
            CultureInfo currentUICulture = null;
            string uICultureName = "";

            // cultureWiseFile
            string cultureWiseFilePath = "";
            string cultureWiseFileName = "";

            string content = "";

            lock (GetMessage._lock)
            {
                do
                {
                    if (currentUICulture == null)
                    {
                        // 初回
                        currentUICulture = uiCulture;
                    }
                    else
                    {
                        // フォールバック
                        currentUICulture = currentUICulture.Parent;
                    }

                    // uICultureName
                    uICultureName = currentUICulture.Name;

                    // リソース名の国際化
                    defaultFileName = GetMessage.GetCultureWiseFile(
                        uICultureName, defaultFilePath, out cultureWiseFilePath, out cultureWiseFileName);

                    if (GetMessage.DicMSG.ContainsKey(cultureWiseFilePath))
                    {
                        // ロード済み
                        if (GetMessage.DicMSG[cultureWiseFilePath].ContainsKey(messageID))
                        {
                            content = GetMessage.DicMSG[cultureWiseFilePath][messageID];
                        }
                    }
                    else
                    {
                        // 未ロード
                        if (GetMessage.LoadFromFile(cultureWiseFilePath))
                        {
                            // ロードできた。
                            if (GetMessage.DicMSG[cultureWiseFilePath].ContainsKey(messageID))
                            {
                                content = GetMessage.DicMSG[cultureWiseFilePath][messageID];
                            }
                        }
                        else
                        {
                            // ロードできなかった。
                            // フォールバック
                        }
                    }
                }
                while (
                    !string.IsNullOrEmpty(uICultureName)      // フォールバックが終わった。
                    && string.IsNullOrEmpty(content)  // ファイルを読み取れなかった。
                );

                if (string.IsNullOrEmpty(content)) // 既定（英語）
                {
                    if (GetMessage.DicMSG.ContainsKey(defaultFilePath))
                    {
                        // ロード済み
                        if (GetMessage.DicMSG[defaultFilePath].ContainsKey(messageID))
                        {
                                content = GetMessage.DicMSG[defaultFilePath][messageID];
                        }
                    }
                    else
                    {
                        // 未ロード
                        if (GetMessage.LoadFromFile(defaultFilePath))
                        {
                            // ロードできた。
                            if (GetMessage.DicMSG[defaultFilePath].ContainsKey(messageID))
                            {
                                    content = GetMessage.DicMSG[defaultFilePath][messageID];
                            }
                        }
                        else
                        {
                            // ロードできなかった。
                            // → 終了
                        }
                    }
                }
            }

            return content;
        }

        /// <summary>Get culture wise file</summary>
        /// <param name="cultureName">cultureName</param>
        /// <param name="defaultFilePath">defaultFilePath</param>
        /// <param name="cultureWiseFilePath">cultureWiseFilePath</param>
        /// <param name="cultureWiseFileName">cultureWiseFileName</param>
        /// <returns>defaultFileName</returns>
        private static string GetCultureWiseFile(
            string cultureName,
            string defaultFilePath,
            out string cultureWiseFilePath,
            out string cultureWiseFileName
            )
        {
            string defaultFileName = Path.GetFileName(defaultFilePath);
            int temp = defaultFileName.Length - 4; // -4 は *.xxx の拡張子部分
            //                                               ~~~~

            // cultureWiseFile
            cultureWiseFileName = defaultFileName.Substring(0, temp) + "_" + cultureName + defaultFileName.Substring(temp);
            cultureWiseFilePath = defaultFilePath.Replace(defaultFileName, cultureWiseFileName);

            // defaultFileName
            return defaultFileName;
        }

        /// <summary>XmlDocumentをLoad</summary>
        /// <param name="filePath">string</param>
        /// <returns>
        /// 真 : ロードできた
        /// 偽 : ロードできなかった
        /// </returns>
        private static bool LoadFromFile(string filePath)
        {
            if (EmbeddedResourceLoader.Exists(filePath, false))
            {
                XmlDocument xMLMSG = new XmlDocument();

                // Load
                xMLMSG.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(filePath));
                // Save
                GetMessage.DicMSG[filePath] = GetMessage.FillDictionary(xMLMSG);

                return true;
            }
            else if (ResourceLoader.Exists(filePath, false))
            {
                XmlDocument xMLMSG = new XmlDocument();
                
                // Load
                xMLMSG.Load(PubCmnFunction.BuiltStringIntoEnvironmentVariable(filePath));
                // Save
                GetMessage.DicMSG[filePath] = GetMessage.FillDictionary(xMLMSG);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// XmlDocumentからDictionary(string, string)に値を充填
        /// </summary>
        /// <param name="xMLMSG">XmlDocument of MSGDefinition.xml</param>
        /// <returns>Dictionary(string, string)</returns>
        private static Dictionary<string, string> FillDictionary(XmlDocument xMLMSG)
        {
            Dictionary<string, string> innerDictionary = new Dictionary<string, string>();
            XmlNodeList xmlNodeList = xMLMSG.GetElementsByTagName(FxLiteral.XML_MSG_TAG_MESSAGE);

            foreach (XmlNode xmlNodeMSG in xmlNodeList)
            {
                // 属性の取得
                XmlNode xmlNodeId = xmlNodeMSG.Attributes.GetNamedItem(FxLiteral.XML_CMN_ATTR_ID);
                XmlNode xmlNodeDsc = xmlNodeMSG.Attributes.GetNamedItem(FxLiteral.XML_MSG_ATTR_DESCRIPTION);

                if (xmlNodeId == null)
                {
                    // id属性なしの場合

                    throw new FrameworkException(
                        FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR[0],
                        String.Format(FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR[1],
                            String.Format(FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR_ATTR, FxLiteral.XML_CMN_ATTR_ID, "－")));
                }

                if (xmlNodeDsc == null)
                {
                    // description属性なしの場合

                    throw new FrameworkException(
                        FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR[0],
                        String.Format(FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR[1],
                            String.Format(FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR_ATTR, FxLiteral.XML_MSG_ATTR_DESCRIPTION, xmlNodeId.Value)));
                }

                innerDictionary.Add(xmlNodeId.Value, xmlNodeDsc.Value);
            }

            return innerDictionary;
        }
    }
}
