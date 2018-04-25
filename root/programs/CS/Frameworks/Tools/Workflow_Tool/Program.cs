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
//*  2014/09/05  Sai               Added DefaultCulture key in app.Config file and did coding for reading the culture value from app.Config file.
//*                                Created Resource folder and Resource.ja-JP.resx, Resource.resx files inside the Resource folder.
//*                                Added proper key and values in those files for English and Japanese languages.
//**********************************************************************************

using System;
using System.Threading;
using System.Resources;
using System.Globalization;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Util;

namespace Workflow_Tool
{
    static class Program
    {
        /// <summary>
        /// Getting ResourceManager instance from Resources to apply internationalization
        /// </summary>
        private static ResourceManager ResourceMgr
        {
            get
            {
                return Resources.Resource.ResourceManager;
            }
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //Reading the default culture value from app.Config file.
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
            catch (Exception ex)
            {
                MessageBox.Show(ResourceMgr.GetString("EntryPoint") + ex.Message);
            }
        }
    }
}
