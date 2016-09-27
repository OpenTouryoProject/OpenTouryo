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
//* クラス名        ：InProcessNameService
//* クラス日本語名  ：インプロセス呼び出しの名前解決クラス
//*                   論理名称からアセンブリ名、クラス名を取得
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/02  西野  大介        新規作成
//*  2009/04/15  西野  大介        設計変更により２つに分割
//*  2009/04/20  西野  大介        埋め込まれたリソースXMLファイルのロード メソッドの変更
//*  2009/04/21  西野  大介        FrameworkExceptionの追加に伴い、実装変更
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#13 ： 外設・XML定義との不一致
//*                                ・#14 ： XMLチェック処理追加
//*                                ・#15 ： XML要素のリテラル化
//*                                ・#31 ： エラーメッセージ不正
//*  2011/01/19  西野  大介        環境変数の組み込み処理に対応
//**********************************************************************************

using System.Reflection;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Transmission
{
    /// <summary>インプロセス呼び出しの名前解決クラス</summary>
    /// <remarks>CallController、ServiceInterfaceから利用するので、public</remarks>
    public class InProcessNameService
    {
        /// <summary>インプロセス呼び出しの名前解決定義</summary>
        private XmlDocument XMLTMD_InProcess = new XmlDocument();

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <remarks>CallController、ServiceInterfaceから利用するので、public</remarks>
        public InProcessNameService()
        {
            // インプロセス呼び出しの名前解決定義をロードする。

            #region  埋め込まれたリソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）

            if (EmbeddedResourceLoader.Exists(
                    GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_INPROCESS_DEFINITION), false))
            {
                // インプロセス呼び出しの名前解決定義（XmlDocument）を[埋め込まれたリソース]で初期化
                this.XMLTMD_InProcess.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(
                    GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_INPROCESS_DEFINITION)));

                // 戻す
                return;
            }
            else
            {
                // 何もしない。
            }

            #endregion

            #region リソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）

            if (ResourceLoader.Exists(
                GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_INPROCESS_DEFINITION), false))
            {
                // インプロセス呼び出しの名前解決定義（XmlDocument）を[リソース]で初期化
                this.XMLTMD_InProcess.Load(
                    PubCmnFunction.BuiltStringIntoEnvironmentVariable(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_INPROCESS_DEFINITION)));

                // 戻す
                return;
            }
            else
            {
                // 何もしない。
            }

            #endregion

            #region チェック（定義の有無や、定義の誤り）

            if (GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_INPROCESS_DEFINITION) == null
                    || GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_INPROCESS_DEFINITION) == "")
            {
                // 定義が無い（offの扱い）。

                // インプロセス呼び出しの名前解決定義（XmlDocument）を空で初期化
                this.XMLTMD_InProcess.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><TMD></TMD>");
            }
            else
            {
                // 定義が間違っている（エラー）。

                // 例外をスロー
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[1],
                                    FxLiteral.XML_TM_INPROCESS_DEFINITION));
            }

            #endregion
        }

        #endregion

        // #13,14,15-start

        #region 名前解決（論理名称からアセンブリ名、クラス名を取得）

        /// <summary>名前解決（論理名称からアセンブリ名、クラス名を取得）</summary>
        /// <param name="name">論理名称</param>
        /// <param name="assemblyName">アセンブリ名</param>
        /// <param name="className">クラス名</param>
        /// <remarks>CallController、ServiceInterfaceから利用するので、public</remarks>
        public void NameResolution(string name, out string assemblyName, out string className)
        {
            // 初期化
            assemblyName = "";
            className = "";

            // 属性チェック用
            XmlNode xmlNode = null;

            // Transmissionタグを取得
            XmlElement xmlTransmission = this.XMLTMD_InProcess.GetElementById(name);
            
            // チェック
            if (xmlTransmission == null)
            {
                // Transmissionタグがない場合

                // #31-start

                // 例外をスロー
                throw new FrameworkException(
                    FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[0],
                    String.Format(FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[1],
                        String.Format(FrameworkExceptionMessage.NAMESERVICE_XML_FORMAT_ERROR_tm, name)));

                // #31-end
            }
            else
            {
                // Transmissionタグがある場合

                #region アセンブリ名

                xmlNode = xmlTransmission.Attributes[FxLiteral.XML_TM_INPROCESS_ATTR_ASSEMBLYNAME];

                if (xmlNode == null)
                {
                    // assemblyName属性なしの場合

                    // 例外をスロー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[0],
                        System.String.Format(
                            FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[1],
                            System.String.Format(FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName1, name)));
                }
                else
                {
                    // assemblyName属性ありの場合
                    assemblyName = xmlNode.Value;

                    if (assemblyName == "")
                    {
                        // assemblyNameが空の場合

                        // 例外をスロー
                        throw new FrameworkException(
                            FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[0],
                            System.String.Format(
                                FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[1],
                                System.String.Format(FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName2, name)));
                    }
                }

                #endregion

                #region クラス名

                xmlNode = xmlTransmission.Attributes[FxLiteral.XML_TM_INPROCESS_ATTR_CLASSNAME];

                if (xmlNode == null)
                {
                    // className属性なしの場合

                    // 例外をスロー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[0],
                        System.String.Format(
                            FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[1],
                            System.String.Format(FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName1, name)));
                }
                else
                {
                    // className属性ありの場合
                    className = xmlNode.Value;

                    if (className == "")
                    {
                        // classNameが空の場合

                        // 例外をスロー
                        throw new FrameworkException(
                            FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[0],
                            System.String.Format(
                                FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR[1],
                                System.String.Format(FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName2, name)));
                    }
                }
                
                #endregion
            }
        }

        #endregion

        // #13,14,15-end
    }
}
