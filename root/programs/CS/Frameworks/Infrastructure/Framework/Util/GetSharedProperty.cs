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
//* クラス名        ：GetSharedProperty
//* クラス日本語名  ：共有情報取得クラス
//*                   XMLファイルから、共有情報IDに対応する値を取得する。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/06/11  西野 大介         新規作成
//*  2010/09/24  西野 大介         ジェネリック対応（XMLのDictionary化）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2011/01/19  西野 大介         環境変数の組み込み処理に対応
//**********************************************************************************

using System;
using System.Xml;
using System.Collections.Generic;

using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>共有情報取得クラス</summary>
    /// <remarks>
    /// インナークラスをシングルトンとするデザイン パターンを採用
    /// 自由に利用できる。
    /// </remarks>
    public class GetSharedProperty
    {
        /// <summary>共有情報定義</summary>
        private static SharedPropertyManager _spM = new SharedPropertyManager();

        /// <summary>共有情報IDから、共有情報値を取得</summary>
        /// <param name="sharedPropKey">共有情報キー</param>
        /// <returns>共有情報値</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetSharedPropertyValue(string sharedPropKey)
        {
            return GetSharedProperty._spM.GetSharedPropertyValue(sharedPropKey);
        }

        #region インナークラス

        /// <summary>コンストラクタ</summary>
        /// <remarks>
        /// シングルトンなので、初期化は起動時の１回のみ。
        /// インナークラス
        /// </remarks>
        private class SharedPropertyManager
        {
            /// <summary>共有情報定義をロードする</summary>
            private Dictionary<string, string> DicSP = new Dictionary<string, string>();

            #region コンストラクタ

            /// <summary>コンストラクタ</summary>
            /// <remarks>インナークラス</remarks>
            public SharedPropertyManager()
            {
                // 共有情報定義をロードする。
                XmlDocument xMLSP = new XmlDocument();

                if (GetConfigParameter.GetConfigValue(FxLiteral.XML_SP_DEFINITION) == null
                    || GetConfigParameter.GetConfigValue(FxLiteral.XML_SP_DEFINITION) == "")
                {
                    // 定義が無い（offの扱い）。

                    // 共有情報定義（XmlDocument）を空で初期化
                    xMLSP.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><SPD></SPD>");
                }
                else
                {
                    //// 定義が間違っている（エラー）。

                    //// エラーをスロー
                    //throw new FrameworkException(
                    //    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[0],
                    //    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[1],
                    //                    FxLiteral.XML_SP_DEFINITION));

                    #region  埋め込まれたリソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）

                    if (EmbeddedResourceLoader.Exists(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_SP_DEFINITION), false))
                    {

                        // 共有情報定義（XmlDocument）を[埋め込まれたリソース]で初期化
                        xMLSP.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(
                            GetConfigParameter.GetConfigValue(FxLiteral.XML_SP_DEFINITION)));

                        //// 戻す
                        //return;
                    }
                    else
                    {
                        // 何もしない。
                    }

                    #endregion

                    #region リソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）

                    if (ResourceLoader.Exists(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_SP_DEFINITION), false))
                    {
                        // 共有情報定義（XmlDocument）を[リソース]で初期化
                        xMLSP.Load(
                            PubCmnFunction.BuiltStringIntoEnvironmentVariable(
                                GetConfigParameter.GetConfigValue(FxLiteral.XML_SP_DEFINITION)));

                        //// 戻す
                        //return;
                    }
                    else
                    {
                        // 何もしない。
                    }

                    #endregion
                }

                #region すべてのSHAREDPROPタグをDictionary化

                // すべてのSHAREDPROPタグを取得、大文字・小文字は区別する。
                XmlNodeList xmlNodeList = xMLSP.GetElementsByTagName(FxLiteral.XML_SP_TAG_SHARED_PROPERTY);

                foreach (XmlNode xmlNodeSP in xmlNodeList)
                {
                    // 属性の取得
                    XmlNode xmlNodeKey = xmlNodeSP.Attributes.GetNamedItem(FxLiteral.XML_CMN_ATTR_KEY);
                    XmlNode xmlNodeVal = xmlNodeSP.Attributes.GetNamedItem(FxLiteral.XML_CMN_ATTR_VALUE);

                    if (xmlNodeKey == null)
                    {
                        // id属性なしの場合

                        throw new FrameworkException(
                            FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR[0],
                            String.Format(FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR[1],
                                String.Format(FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR_ATTR, FxLiteral.XML_CMN_ATTR_KEY, "－")));
                    }

                    if (xmlNodeVal == null)
                    {
                        // description属性なしの場合

                        throw new FrameworkException(
                            FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR[0],
                            String.Format(FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR[1],
                                String.Format(FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR_ATTR, FxLiteral.XML_CMN_ATTR_VALUE, xmlNodeKey.Value)));
                    }

                    this.DicSP.Add(xmlNodeKey.Value, xmlNodeVal.Value);
                }

                #endregion
            }

            #endregion

            #region 共有情報取得

            /// <summary>共有情報IDから、共有情報値を取得</summary>
            /// <param name="sharedPropKey">共有情報キー</param>
            /// <returns>共有情報値</returns>
            /// <remarks>インナークラス</remarks>
            public string GetSharedPropertyValue(string sharedPropKey)
            {
                if (this.DicSP.ContainsKey(sharedPropKey))
                {
                    // キーが有る
                    return this.DicSP[sharedPropKey];
                }
                else
                {
                    // キーが無い
                    return "";
                }
            }

            #endregion
        }

        #endregion
    }
}
