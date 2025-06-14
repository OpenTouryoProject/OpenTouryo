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
#else
#endif
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("Form1", "？", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (result == DialogResult.Yes)
            {
                Application.Run(new Form1());
            }
            else
            {
                Application.Run(new Form2());
            }
        }
    }
}
