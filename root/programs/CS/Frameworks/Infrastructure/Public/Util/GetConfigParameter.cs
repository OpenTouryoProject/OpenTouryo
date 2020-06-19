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
//* クラス名        ：GetConfigParameter
//* クラス日本語名  ：Configファイルから、パラメータを取得するクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2010/11/22  西野 大介         NullReferenceException対応
//*  2018/03/28  西野 大介         .NET Standard対応で、I/F変更あり。
//*  2020/06/19  西野 大介         コンテナ・モードの時は環境変数を優先する。
//*  2020/06/19  西野 大介         以下のSectionのみを対象とするため、
//*                                GetConfigSectionメソッドを廃止した。
//*                                - appSettings
//*                                - connectionStrings
//*                                ※ JSONキーを使った場合、コンテナ・モードに対応しない。
//**********************************************************************************

#if NETSTD
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
#else
using System;
using System.Configuration;
#endif

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>設定ファイルから、パラメータを取得する機能を提供する。</summary>
    /// <remarks>自由に利用できる。</remarks>
    public static class GetConfigParameter
    {

#if NETSTD

        /// <summary>IConfiguration</summary>
        private static IConfiguration _configuration = null;

        /// <summary>IConfiguration</summary>
        public static IConfiguration Configuration
        {
            get
            {
                return GetConfigParameter._configuration;
            }
        }

        #region 初期化

        /// <summary>IConfigurationの初期化</summary>
        /// <param name="configuration">IConfiguration</param>
        public static void InitConfiguration(IConfiguration configuration)
        {
            GetConfigParameter._configuration = configuration;
        }

        /// <summary>IConfigurationの初期化</summary>
        /// <param name="builder">IConfigurationBuilder</param>
        public static void InitConfiguration(IConfigurationBuilder builder)
        {
            GetConfigParameter._configuration = builder.Build();
        }

        /// <summary>IConfigurationの初期化</summary>
        /// <param name="jsonFileName">string</param>
        public static void InitConfiguration(string jsonFileName)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();

            // currentディレクトリのappsettings.jsonを読み込む。
            builder = builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFileName);

            GetConfigParameter._configuration = builder.Build();
        }

        /// <summary>IConfigurationの初期化</summary>
        public static void InitConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();

            // currentディレクトリのappsettings.jsonを読み込む。
            builder = builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

        #region 参考：メモリ内プロバイダによる POCO クラスとのバインディング

            //Dictionary dict = new Dictionary<string, string>() {
            //    {"XXXX", "xxxx"},
            //    {"YYYY", "yyyy"}
            //};
            //builder.AddInMemoryCollection(dict);

        #endregion

            GetConfigParameter._configuration = builder.Build();

        #region 参考：既存のconfigに POCO クラスとのバインドを追加

            //AppSettings appConfig = new AppSettings();
            //GetConfigParameter._configuration.GetSection("App").Bind(appConfig);

        #endregion

            // その他、Entity FrameworkやCommand Lineのプロバイダも作成可能。

            // ASP.NET Core の構成 | Microsoft Docs
            // https://docs.microsoft.com/ja-jp/aspnet/core/fundamentals/configuration/index?tabs=basicconfiguration
        }

        #endregion

        #region JSONキー
        
        /// <summary>
        /// 設定ファイルに定義されている任意の値を取得する
        /// </summary>
        /// <param name="key">設定ファイルに定義されている値のJSONキー</param>
        /// <returns>設定ファイルに定義されている値</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetAnyConfigValue(string key)
        {
            // 設定ファイルの内容を返す
            if (GetConfigParameter._configuration == null)
            {
                throw new ArgumentException(
                    // Resource は NG（無限ループになる。
                    //PublicExceptionMessage.NOT_INITIALIZED,
                    "NOT_INITIALIZED",
                    "InitConfiguration method is not called.");
            }
            else
            {
                return GetConfigParameter._configuration[key];
            }
        }

        /// <summary>
        /// 設定ファイルに定義されている任意のセクションを取得する
        /// </summary>
        /// <param name="key">設定ファイルに定義されているセクションのJSONキー</param>
        /// <returns>設定ファイルに定義されているセクション</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static IConfigurationSection GetAnyConfigSection(string key)
        {
            // 設定ファイルの内容を返す
            if (GetConfigParameter._configuration == null)
            {
                throw new ArgumentException(
                    // Resource は NG（無限ループになる。
                    //PublicExceptionMessage.NOT_INITIALIZED,
                    "NOT_INITIALIZED",
                    "InitConfiguration method is not called.");
            }
            else
            {
                return GetConfigParameter._configuration.GetSection(key);
            }
        }

        #endregion
#endif

        /// <summary>
        /// 設定ファイルのappSettingsSectionに定義されている値を取得する
        /// ※ コンテナ・モードの時は環境変数を優先する。
        /// </summary>
        /// <param name="key">設定ファイルのappSettingsSectionに定義されているキー名</param>
        /// <param name="checkContainerization">コンテナ・モードのチェックの要否</param>
        /// <returns>設定ファイルのappSettingsSectionに定義されている値</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetConfigValue(string key, bool checkContainerization = true)
        {
            string temp = null;

            // コンテナ・モードの時は環境変数を優先する。
            // ※ この場合、Section名を考慮しない。
            if (checkContainerization)
                temp = GetConfigParameter.CheckContainerization(key);

            if (string.IsNullOrEmpty(temp))
            {
                // 通常時、設定ファイルの内容を返す。
#if NETSTD
                if (GetConfigParameter._configuration == null)
                {
                    throw new ArgumentException(
                        // Resource は NG（無限ループになる。
                        //PublicExceptionMessage.NOT_INITIALIZED,
                        "NOT_INITIALIZED",
                        "InitConfiguration method is not called.");
                }
                else
                {
                    return GetConfigParameter._configuration["appSettings:" + key];
                }
#else
                return ConfigurationManager.AppSettings[key];
#endif
            }
            else
            {
                // コンテナ・モードの時、環境変数の内容を返す。
                return temp;
            }
        }

        /// <summary>
        /// 設定ファイルのconnectionStringsSectionに定義
        /// されているデータベースへの接続文字列を取得する
        /// ※ コンテナ・モードの時は環境変数を優先する。
        /// </summary>
        /// <param name="key">設定ファイルのconnectionStringsSectionに定義されているキー名</param>
        /// <returns>設定ファイルのconnectionStringsSectionに定義されている接続文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetConnectionString(string key)
        {
            // コンテナ・モードの時は環境変数を優先する。
            // ※ この場合、Section名を考慮しない。
            string temp = GetConfigParameter.CheckContainerization(key);

            if (string.IsNullOrEmpty(temp))
            {
                // 通常時、設定ファイルの内容を返す。
#if NETSTD
                if (GetConfigParameter._configuration == null)
                {
                    throw new ArgumentException(
                        // Resource は NG（無限ループになる。
                        //PublicExceptionMessage.NOT_INITIALIZED,
                        "NOT_INITIALIZED",
                        "InitConfiguration method is not called.");
                }
                else
                {
                    return GetConfigParameter._configuration["connectionStrings:" + key];
                }
#else
                ConnectionStringSettings connString = ConfigurationManager.ConnectionStrings[key];

                // NullReferenceException対応
                if (connString == null)
                {
                    return null;
                }
                else
                {
                    return connString.ConnectionString;
                }
#endif
            }
            else
            {
                // コンテナ・モードの時、環境変数の内容を返す。
                return temp;
            }
        }

        /// <summary>コンテナ化されている場合、環境変数の取得を試みる。</summary>
        /// <param name="key">環境変数として定義されているキー名</param>
        /// <returns>環境変数として定義されている値</returns>
        /// <remarks>自由に利用できる。</remarks>
        private static string CheckContainerization(string key)
        {
            // モードのチェック
            string containerization =
                GetConfigParameter.GetConfigValue(PubLiteral.CONTAINERIZATION, false);

            // デフォルト値対策：設定なし（null）の場合の扱いを決定
            if (containerization == null)
            {
                // OFF扱い
                containerization = PubLiteral.OFF;
            }

            // ON / OFF
            if (containerization.ToUpper() == PubLiteral.ON)
            {
                // 環境変数の値を返す。
                return System.Environment.GetEnvironmentVariable(key);
            }
            else if (containerization.ToUpper() == PubLiteral.OFF)
            {
                // null を返す。
                return null;
            }
            else
            {
                // パラメータ・エラー（書式不正）
                throw new ArgumentException(String.Format(
                    PublicExceptionMessage.SWITCH_ERROR, PubLiteral.CONTAINERIZATION));
            }
        }
    }
}
