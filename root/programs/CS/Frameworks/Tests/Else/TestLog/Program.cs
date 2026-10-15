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
//* クラス日本語名  ：ログ出力テストのエントリ ポイント
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2025/06/15  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Threading;
using System.Resources;
using System.Globalization;
using System.Windows.Forms;
using Touryo.Infrastructure.Public.Util;

namespace TestLog
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if NETCOREAPP
            // configの初期化
            GetConfigParameter.InitConfiguration("appsettings.json");
#endif

#if Form1
            Application.Run(new Form1());
#elif Form2
            Application.Run(new Form2());
#else
            Application.Run(new Form3());
#endif
        }
    }
}
