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
//* クラス名        ：Form3
//* クラス日本語名  ：ログ出力テストの画面
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2025/06/15  西野 大介         新規作成
//**********************************************************************************

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Log;

namespace TestLog
{
    public partial class Form3 : Form
    {
        /// <summary>メッセージ・ヘッダ</summary>
        private string message = "";

        /// <summary>Form3</summary>
        public Form3()
        {
            InitializeComponent();

            string fxLog4NetConfFile = GetConfigParameter.GetConfigValue("FxLog4NetConfFile");
            if (fxLog4NetConfFile == "SampleLogConf_N.xml")
            {
                message = "NLogファイルからロード";
            }
            else if (fxLog4NetConfFile == "TestLog.SampleLogConf_N.xml")
            {
                message = "NLog埋め込まれたリソースからロード";
            }
            else
            {
                throw new Exception("FxLog4NetConfFileの値が不正です。");
            }
        }

        /// <summary>button1_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void button1_Click(object sender, EventArgs e)
        {
            LogIF.ErrorLog("ACCESS", this.message + " " + this.textBox1.Text);
        }
    }
}
