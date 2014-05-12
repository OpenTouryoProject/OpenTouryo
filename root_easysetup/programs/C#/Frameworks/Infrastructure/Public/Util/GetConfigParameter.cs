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
//* クラス名        ：GetConfigParameter
//* クラス日本語名  ：Configファイルから、パラメータを取得するクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2010/11/22  西野  大介        NullReferenceException対応
//**********************************************************************************

// Config
using System.Configuration;

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

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>設定ファイルから、パラメータを取得する機能を提供する。</summary>
    /// <remarks>自由に利用できる。</remarks>
    public static class GetConfigParameter
    {
        /// <summary>
        /// 設定ファイルのappSettingsタグに定義されている値を取得する
        /// </summary>
        /// <param name="key">設定ファイルに定義されているキー名</param>
        /// <returns>設定ファイルに定義されている値</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetConfigValue(string key)
        {
            //設定ファイルの内容を返す
            return ConfigurationManager.AppSettings[key];
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
            //接続文字列の取得
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
        }
    }
}
