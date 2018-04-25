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
//**********************************************************************************

#if NETSTD
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
#else
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

        /// <summary>
        /// 設定ファイルに定義されている任意の値を取得する
        /// </summary>
        /// <param name="key">設定ファイルに定義されているJSONキー</param>
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

#else

#endif

        /// <summary>
        /// 設定ファイルのappSettingsタグに定義されている値を取得する
        /// </summary>
        /// <param name="key">設定ファイルに定義されているキー名</param>
        /// <returns>設定ファイルに定義されている値</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetConfigValue(string key)
        {
            // 設定ファイルの内容を返す
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

        /// <summary>
        /// 設定ファイルのconnectionStringsタグに定義
        /// されているデータベースへの接続文字列を取得する
        /// </summary>
        /// <param name="key">設定ファイルに定義されているキー名</param>
        /// <returns>設定ファイルに定義されている接続文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetConnectionString(string key)
        {
            // 接続文字列の取得
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
    }
}
