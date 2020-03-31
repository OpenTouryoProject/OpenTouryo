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
//* クラス名        ：Program
//* クラス日本語名  ：アプリケーションのメイン エントリ ポイント
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2014/04/28  Rituparna         Add DefaultCulture key in app.Config file and take the culture value from app.Config file.
//*                                Created Resource folder and Resource.ja-JP.resx,Resource.resx files inside the Resource folder.
//*                                Added proper key and values in those files for English and Japanese languages.
//*  2014/05/12  Rituparna         Removed <start> and <End> tags, added check while reading DefaultCulture from app.config file   
//*  2018/10/29  西野 大介         NETCOREAPP対応で、configの初期化
//**********************************************************************************

using System;
using System.Threading;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Util;

namespace DPQuery_Tool
{
    /// <summary>アプリケーションのメイン エントリ ポイント</summary>
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if NETCOREAPP
            // configの初期化
            GetConfigParameter.InitConfiguration("appsettings.json");
#else
#endif
            try
            {
                // Add DefaultCulture key in app.Config file and take the culture value from app.Config file.
                string culture = GetConfigParameter.GetConfigValue("DefaultCulture");        
                if (!string.IsNullOrEmpty(culture))
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(RM_GetString("EntryPoint"), ex.Message));
            }
        }


        private static string RM_GetString(string key)
        {
            //get the string value from resource file  by proper passing key.
            ResourceManager rm = Resources.Resource.ResourceManager;
            return rm.GetString(key);
        }
    }
}
