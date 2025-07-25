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
//* クラス名        ：LogManager
//* クラス日本語名  ：log4netロガー管理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2008/09/19  西野 大介         設計不良の対策（シングルトンクラス → 静的クラスに変更）
//*  2009/01/28  西野 大介         クラス名の変更（WebLogManager → LogManager）
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#12 ： 定義ファイルなしの場合、ログ出力しない仕様に変更
//*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/12/03  西野 大介         log4netの埋め込まれたリソース対応（RichClient）
//*  2011/01/19  西野 大介         環境変数の組み込み処理に対応
//*  2011/10/09  西野 大介         国際化対応
//*  2018/03/28  西野 大介         .NET Standard対応で、I/F変更あり。
//**********************************************************************************

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using log4net.Config;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

#if (NETSTD || NETCOREAPP)
using System.Reflection;
using log4net.Repository;
#else
#endif

namespace Touryo.Infrastructure.Public.Log
{
    /// <summary>log4netロガー管理クラス</summary>
    internal class LogManager_log4net
    {
        #region クラス変数

        /// <summary>
        /// log4netロガー（log4net.ILog）をキャッシュする
        /// </summary>
        private static Dictionary<string, log4net.ILog> _logIfHt = new Dictionary<string, log4net.ILog>();

        /// <summary>
        /// 排他のためのクラス変数
        /// </summary>
        private static readonly object _lock = new object();

        #endregion

        #region 静的メソッド

        /// <summary>
        /// log4net.ILogインスタンスの取得
        /// </summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>log4net.ILog</returns>
        public static log4net.ILog GetLog4netIf(string loggerName)
        {
            lock (LogManager_log4net._lock)
            {
                // null対策
                if (LogManager_log4net._logIfHt == null)
                {
                    LogManager_log4net._logIfHt = new Dictionary<string, log4net.ILog>();
                }

                // すでにlog4net.ILogインスタンスが存在する。
                if (LogManager_log4net._logIfHt.ContainsKey(loggerName)) // Dic化でnullチェック変更
                {
                    // 生成済みのlog4net.ILogインスタンスを返す。
                    return (log4net.ILog)LogManager_log4net._logIfHt[loggerName];
                }
                else
                {
                    // #12-start

                    // 定義ファイル
                    string log4netConfFile = GetConfigParameter.GetConfigValue(PubLiteral.LOG4NET_CONF_FILE);

                    // log4netの設定ファイルのパス
                    if (log4netConfFile == null || log4netConfFile == "")
                    {
                        // 定義ファイルのパスが無い場合

                        // 空のロガーを返す（エラーにはならない）
#if (NETSTD || NETCOREAPP)
                        return log4net.LogManager.GetLogger(Assembly.GetEntryAssembly(), "");
#else
                        return log4net.LogManager.GetLogger("");
#endif
                    }
                    else
                    {
#if (NETSTD || NETCOREAPP)
                        // Repositoryなる何か。
                        ILoggerRepository logRep = log4net.LogManager.CreateRepository(
                            Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
#else
#endif

                        // 埋め込まれたリソース ローダで存在チェック
                        if (EmbeddedResourceLoader.Exists(log4netConfFile, false))
                        {
                            // ログ定義 [埋め込まれたリソース]
                            XmlDocument xmlDef = new XmlDocument();

                            // Exceptionが上がり得る。
                            xmlDef.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(log4netConfFile));

                            if (xmlDef["log4net"] == null)
                            {
                                // XmlElement（log4net）が無い場合
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.XML_ELEMENT_ERROR,
                                    PublicExceptionMessage.XML_ELEMENT_ERROR_LOG4NET));
                            }

                            // log4net
#if (NETSTD || NETCOREAPP)
                            XmlConfigurator.Configure(logRep,
                                (XmlElement)xmlDef["log4net"]);
#else
                            XmlConfigurator.Configure(xmlDef["log4net"]);
#endif
                        }
                        else
                        {
                            // リソース ローダで存在チェック（存在しなければエラー）
                            ResourceLoader.Exists(log4netConfFile, true);

                            // ログ定義 [リソース ファイル] → ストリームを開く
                            FileStream s = new FileStream(
                                StringVariableOperator.BuiltStringIntoEnvironmentVariable(log4netConfFile),
                                FileMode.Open, FileAccess.Read, FileShare.Read);

                            // log4netのXML形式の設定ファイルを読み込む。
#if (NETSTD || NETCOREAPP)
                            XmlConfigurator.Configure(logRep, s);
#else
                            XmlConfigurator.Configure(s);
#endif

                            s.Close();
                        }

                        // log4net.ILogインスタンスを初期化する。
#if (NETSTD || NETCOREAPP)
                        LogManager_log4net._logIfHt.Add(
                            loggerName,
                            log4net.LogManager.GetLogger(Assembly.GetEntryAssembly(), loggerName));
#else
                        LogManager_log4net._logIfHt.Add(loggerName, log4net.LogManager.GetLogger(loggerName));
#endif

                        // 生成したlog4net.ILogインスタンスを返す。
                        return (log4net.ILog)LogManager_log4net._logIfHt[loggerName];
                    }

                    // #12-end
                }
            }
        }

        #endregion
    }
}

