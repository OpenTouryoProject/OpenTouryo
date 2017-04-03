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
//*  2011/02/19  西野 大介         取り込み（細かな修正）
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
        private static Object thisLock = new Object();

        /// <summary>
        /// 国毎のディクショナリ
        /// Dictionary of each country
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> DicMSG = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// message IDに対応したメッセージ記述。<br/>
        /// Get message description from message ID
        /// </summary>
        /// <param name="messageID">
        /// messageID
        /// </param>
        /// <returns>
        /// メッセージ記述<br/>
        /// Get message description
        /// </returns>
        public static string GetMessageDescription(string messageID)
        {
            // FxBusinessMessageCulture
            string fxBusinessMessageCulture = GetConfigParameter.GetConfigValue(FxLiteral.BUSINESSMESSAGECULTUER);

            // Check FxBusinessMessageCulture
            CultureInfo uiCulture = null;

            if (string.IsNullOrEmpty(fxBusinessMessageCulture))
            {
                // Use CurrentUICulture
                uiCulture = Thread.CurrentThread.CurrentUICulture;
            }
            else
            {
                try
                {
                    // Use FxBusinessMessageCulture
                    uiCulture = new CultureInfo(fxBusinessMessageCulture);
                }
                catch
                {
                    // Use CurrentUICulture
                    uiCulture = Thread.CurrentThread.CurrentUICulture;
                }
            }

            // call overload.
            return GetMessage.GetMessageDescription(messageID, uiCulture);
        }

        /// <summary>
        /// message IDに対応したメッセージ記述。<br/>
        /// Get message description from message ID
        /// </summary>
        /// <param name="messageID">
        /// messageID
        /// </param>
        /// <param name="uiCulture">
        /// uiCulture
        /// </param>
        /// <returns>
        /// メッセージ記述<br/>
        /// Get message description
        /// </returns>
        public static string GetMessageDescription(string messageID, CultureInfo uiCulture)
        {
            bool notExist = false;

            while (true)
            {
                if (uiCulture.Parent == null || uiCulture.Parent.Name == string.Empty)
                {
                    // parent is not exist.
                    // and break this loop.
                    return GetMessage.GetMessageDescription(messageID, uiCulture.Name, false, out notExist);
                }
                else
                {
                    // parent is exist.
                    string temp = GetMessage.GetMessageDescription(messageID, uiCulture.Name, true, out notExist);

                    if (notExist)
                    {
                        // not exist
                        // next, use parent.
                        uiCulture = uiCulture.Parent;
                    }
                    else
                    {
                        // exist
                        return temp;
                    }
                }
            }
        }

        /// <summary>
        /// Get message description from message ID
        /// </summary>
        /// <param name="messageID">
        /// messageID
        /// </param>
        /// <param name="cultureName">
        /// cultureName
        /// </param>
        /// <param name="useChildUICulture">
        /// 子カルチャを使用している場合：true<br/>
        /// use ChildUICulture : true
        /// </param>        
        /// <param name="notExist">
        /// 子カルチャを使用している場合で、<br/>
        /// ファイルを発見できなかった場合はtrueを返す。<br/>
        /// if useChildUICulture == true<br/>
        /// then If the file is not found, return true.
        /// </param>
        /// <returns>
        /// メッセージ記述<br/>
        /// Get message description
        /// </returns>
        private static string GetMessageDescription(
            string messageID, string cultureName,
            bool useChildUICulture, out bool notExist)
        {
            #region local
            
            //bool xmlfilenotExist = false;
            notExist = false;

            string tempXMLFileName = string.Empty;
            string tempXMLFilePath = string.Empty;

            string defaultXMLFileName = string.Empty;
            string defaultXMLFilePath = string.Empty;
            string cultureWiseXMLFileName = string.Empty;
            string cultureWiseXMLFilePath = string.Empty;

            string cultureWiseXMLParentFileName = string.Empty;
            string cultureWiseXMLParentFilePath = string.Empty;

            bool defaultXMLFilePath_ResourceLoaderExists = false;
            bool defaultXMLFilePath_EmbeddedResourceLoaderExists = false;
            bool cultureWiseXMLFilePath_ResourceLoaderExists = false;
            bool cultureWiseXMLFilePath_EmbeddedResourceLoaderExists = false;

            #endregion

            defaultXMLFilePath = GetConfigParameter.GetConfigValue(FxLiteral.XML_MSG_DEFINITION);

            GetMessage.GetCultureWiseXMLFileName(
                cultureName, defaultXMLFilePath, out defaultXMLFileName, out cultureWiseXMLFilePath, out cultureWiseXMLFileName);

            // This has been added for Threadsafe
            lock (thisLock)
            {
                #region ContainsKey
                
                // Check that XML file is already loaded.
                if (GetMessage.DicMSG.ContainsKey(cultureWiseXMLFileName))
                {
                    // culture wise XML file is already loaded.
                    if (GetMessage.DicMSG[cultureWiseXMLFileName].ContainsKey(messageID))
                    {
                        return GetMessage.DicMSG[cultureWiseXMLFileName][messageID];
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    // culture wise XML file isn't loaded.
                }

                if (GetMessage.DicMSG.ContainsKey(defaultXMLFileName))
                {
                    // default XML file is already loaded.

                    if (useChildUICulture)
                    {
                        // next, try load.
                    }
                    else
                    {
                        // default XML
                        if (DicMSG[defaultXMLFileName].ContainsKey(messageID))
                        {
                            return GetMessage.DicMSG[defaultXMLFileName][messageID];
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
                else
                {
                    // default XML file isn't loaded.
                }

                #endregion

                #region FillDictionary

                // cultureWiseXMLFilePath
                if (EmbeddedResourceLoader.Exists(cultureWiseXMLFilePath, false))
                {
                    // Exists cultureWiseXMLFile
                    cultureWiseXMLFilePath_EmbeddedResourceLoaderExists = true;
                }
                else
                {
                    // not exists
                    if (ResourceLoader.Exists(cultureWiseXMLFilePath, false))
                    {
                        // Exists cultureWiseXMLFile
                        cultureWiseXMLFilePath_ResourceLoaderExists = true;
                    }
                    else
                    {
                        // not exists

                        // defaultXMLFilePath
                        if (EmbeddedResourceLoader.Exists(defaultXMLFilePath, false))
                        {
                            // Exists defaultXMLFilePath
                            defaultXMLFilePath_EmbeddedResourceLoaderExists = true;
                        }
                        else
                        {
                            if (ResourceLoader.Exists(defaultXMLFilePath, false))
                            {
                                // Exists defaultXMLFilePath
                                defaultXMLFilePath_ResourceLoaderExists = true;
                            }
                        }
                    }
                }

                // select file path
                if (cultureWiseXMLFilePath_ResourceLoaderExists
                    || cultureWiseXMLFilePath_EmbeddedResourceLoaderExists)
                {
                    // cultureWiseXMLFile
                    tempXMLFileName = cultureWiseXMLFileName;
                    tempXMLFilePath = cultureWiseXMLFilePath;
                }
                else
                {
                    // If the file is not found,
                    if (useChildUICulture)
                    {
                        // Look for use the culture info of the parent.
                        notExist = true;
                        return string.Empty;
                    }
                    else
                    {
                        if (defaultXMLFilePath_ResourceLoaderExists
                            || defaultXMLFilePath_EmbeddedResourceLoaderExists)
                        {
                            // defaultXMLFilePath
                            tempXMLFileName = defaultXMLFileName;
                            tempXMLFilePath = defaultXMLFilePath;
                        }
                        else
                        {
                            // use empty XML.
                        }
                    }
                }

                // select load method.
                XmlDocument xMLMSG = new XmlDocument();
                Dictionary<string, string> innerDictionary = new Dictionary<string, string>();

                if (defaultXMLFilePath_EmbeddedResourceLoaderExists
                    || cultureWiseXMLFilePath_EmbeddedResourceLoaderExists)
                {
                    // Use EmbeddedResourceLoader
                    xMLMSG.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(tempXMLFilePath));

                    //added by ritu
                    GetMessage.FillDictionary(xMLMSG, innerDictionary);

                    // and initialize DicMSG[tempXMLFileName]
                    DicMSG[tempXMLFileName] = innerDictionary;
                }
                else if (defaultXMLFilePath_ResourceLoaderExists
                    || cultureWiseXMLFilePath_ResourceLoaderExists)
                {
                    // Load normally.
                    xMLMSG.Load(PubCmnFunction.BuiltStringIntoEnvironmentVariable(tempXMLFilePath));

                    //added by ritu
                    GetMessage.FillDictionary(xMLMSG, innerDictionary);
                    // and initialize DicMSG[tempXMLFileName]
                    DicMSG[tempXMLFileName] = innerDictionary;
                }
                else
                {
                    // If the file is not found, initialized as empty XML
                    xMLMSG.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><MSGD></MSGD>");

                    //added by ritu
                    GetMessage.FillDictionary(xMLMSG, innerDictionary);

                    // and initialize DicMSG[tempXMLFileName]
                    DicMSG[tempXMLFileName] = innerDictionary;
                    //xmlfilenotExist = true;
                }

                // and return GetMessageDescription.
                if (DicMSG[tempXMLFileName].ContainsKey(messageID))
                {
                    return GetMessage.DicMSG[tempXMLFileName][messageID];
                }
                else
                {
                    return string.Empty;
                }

                #endregion
            }
        }

        /// <summary>Get culture wise XML file name</summary>
        /// <param name="cultureName">cultureName</param>
        /// <param name="defaultXMLFilePath">defaultXMLFilePath</param>
        /// <param name="defaultXMLFileName">defaultXMLFileName</param>
        /// <param name="cultureWiseXMLFilePath">cultureWiseXMLFilePath</param>
        /// <param name="cultureWiseXMLFileName">cultureWiseXMLFileName</param>
        private static void GetCultureWiseXMLFileName(
            string cultureName,
            string defaultXMLFilePath,
            out string defaultXMLFileName,
            out string cultureWiseXMLFilePath,
            out string cultureWiseXMLFileName
            )
        {
            defaultXMLFileName = Path.GetFileName(defaultXMLFilePath);

            // There is room for improvement here.
            //cultureWiseXMLFileName = defaultXMLFileName.Split('.')[0] + "." + cultureName + "." + defaultXMLFileName.Split('.')[1];

            //as mentioned for improvement the coding has been changed
            cultureWiseXMLFileName = defaultXMLFileName.Substring(0, defaultXMLFileName.Length - 4) + "_" + cultureName + defaultXMLFileName.Substring(defaultXMLFileName.Length - 4);
            cultureWiseXMLFilePath = defaultXMLFilePath.Replace(defaultXMLFileName, cultureWiseXMLFileName); // There is room for improvement here.
        }

        /// <summary>
        /// XMLからDictionaryに値を充填<br/>
        /// Add values to innerDictionary
        /// </summary>
        /// <param name="xMLMSG">
        /// XmlDocument(MSGDefinition.xml)
        /// </param>
        /// <param name="innerDictionary">
        /// innerDictionar Value
        /// </param>
        private static void FillDictionary(XmlDocument xMLMSG, Dictionary<string, string> innerDictionary)
        {
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
        }
    }
}
