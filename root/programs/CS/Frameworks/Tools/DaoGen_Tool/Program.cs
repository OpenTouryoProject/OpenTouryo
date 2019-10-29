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
//*  2014/04/30  Santosh san       Internationalization: Added Method to get the strings
//*                                from the resource files based on the keys values passed.
//*                                and replaced to this method wherever hard coded values.
//*                                Also Added code to get the Culture information from app.config file.
//*  2018/10/29  西野 大介         NETCOREAPP対応で、configの初期化
//**********************************************************************************

using System;
using System.Threading;
using System.Resources;
using System.Globalization;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Util;

namespace DaoGen_Tool
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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                 string strDefaultCulture = "";

                 /// To Get the Culture info from app.config file 
                 strDefaultCulture = GetConfigParameter.GetConfigValue("DefaultCulture");
                 if (!string.IsNullOrEmpty(strDefaultCulture))
                 {
                     Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(strDefaultCulture);
                     Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(strDefaultCulture);
                 }

                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show(RM_GetString("EntryPoint") + ex.Message);
            }
        }
        
        /// <summary>This Method gets the string values from resource file based on the key passed</summary>
        private static string RM_GetString(string key)
        {
            ResourceManager rm = Resources.Resource.ResourceManager;
            return rm.GetString(key);
        }
    }
}
