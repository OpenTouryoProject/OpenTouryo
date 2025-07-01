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
//* クラス名        ：LogManager_nlog
//* クラス日本語名  ：NLogロガー管理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2025/06/15  西野 大介         新規作成
//**********************************************************************************

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using NLog;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using NLog.Config;

#if (NETSTD || NETCOREAPP)
using System.Reflection;
using log4net.Repository;
#else
#endif

namespace Touryo.Infrastructure.Public.Log
{
    /// <summary>NLogロガー管理クラス</summary>
    internal class LogManager_nlog
    {
        #region クラス変数

        /// <summary>
        /// NLogロガー（NLog.Logger）をキャッシュする
        /// </summary>
        private static Dictionary<string, NLog.Logger> _logIfHt = new Dictionary<string, NLog.Logger>();

        /// <summary>
        /// 排他のためのクラス変数
        /// </summary>
        private static readonly object _lock = new object();

        #endregion

        #region 静的メソッド

        /// <summary>
        /// NLog.Loggerインスタンスの取得
        /// </summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>NLog.Logger</returns>
        public static NLog.Logger GetNLogIf(string loggerName)
        {
            lock (LogManager_nlog._lock)
            {
                // null対策
                if (LogManager_nlog._logIfHt == null)
                {
                    LogManager_nlog._logIfHt = new Dictionary<string, NLog.Logger>();
                }

                // すでにNLog.Loggerインスタンスが存在する。
                if (LogManager_nlog._logIfHt.ContainsKey(loggerName)) // Dic化でnullチェック変更
                {
                    // 生成済みのNLog.Loggerインスタンスを返す。
                    return (NLog.Logger)LogManager_nlog._logIfHt[loggerName];
                }
                else
                {
                    // 定義ファイル
                    string nlogConfFile = GetConfigParameter.GetConfigValue(PubLiteral.LOG4NET_CONF_FILE);

                    // NLogの設定ファイルのパス
                    if (nlogConfFile == null || nlogConfFile == "")
                    {
                        // 定義ファイルのパスが無い場合

                        // 空のロガーを返す（エラーにはならない）
                        return NLog.LogManager.GetLogger("");
                    }
                    else
                    {
                        // 埋め込まれたリソース ローダで存在チェック
                        if (EmbeddedResourceLoader.Exists(nlogConfFile, false))
                        {
                            // ログ定義 [埋め込まれたリソース]
                            XmlDocument xmlDef = new XmlDocument();

                            // Exceptionが上がり得る。
                            xmlDef.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(nlogConfFile));

                            if (xmlDef["nlog"] == null)
                            {
                                // XmlElement（nlog）が無い場合
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.XML_ELEMENT_ERROR,
                                    PublicExceptionMessage.XML_ELEMENT_ERROR_LOG4NET));
                            }

                            // nlog
                            NLog.LogManager.Configuration = new XmlLoggingConfiguration(new XmlNodeReader(xmlDef["nlog"]));
                        }
                        else
                        {
                            // リソース ローダで存在チェック（存在しなければエラー）
                            ResourceLoader.Exists(nlogConfFile, true);

                            // nlog
                            NLog.LogManager.Configuration = new XmlLoggingConfiguration(nlogConfFile);
                        }

                        // NLog.Loggerインスタンスを初期化する。
                        LogManager_nlog._logIfHt.Add(loggerName, NLog.LogManager.GetLogger(loggerName));

                        // 生成したNLog.Loggerインスタンスを返す。
                        return (NLog.Logger)LogManager_nlog._logIfHt[loggerName];
                    }
                }
            }
        }

        #endregion
    }
}

