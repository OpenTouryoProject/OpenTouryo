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
