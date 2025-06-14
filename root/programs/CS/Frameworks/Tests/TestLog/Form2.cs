using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

using log4net;
using log4net.Config;
using log4net.Repository;

using NLog;
using NLog.Config;

using Touryo.Infrastructure.Public.IO;
using System.Xml.Linq;

namespace TestLog
{
    public partial class Form2 : Form
    {
        /// <summary>log4netロガー</summary>
        private log4net.ILog logger1;
        /// <summary>NLogロガー</summary>
        private NLog.Logger logger2;
        /// <summary>log4netメッセージ・ヘッダ</summary>
        private string message = "";
        /// <summary>NLogメッセージ・ヘッダ</summary>
        private string message_N = "";
        /// <summary>log4net定義ファイル</summary>
        private string fileName = "SampleLogConf.xml";
        /// <summary>NLog定義ファイル</summary>
        private string fileName_N = "SampleLogConf_N.xml";
        /// <summary>log4net定義リソース</summary>
        private string embeddedResourceName = "TestLog.SampleLogConf.xml";
        /// <summary>NLog定義リソース</summary>
        private string embeddedResourceName_N = "TestLog.SampleLogConf_N.xml";

        /// <summary></summary>
        public Form2()
        {
            InitializeComponent();

            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resources = asm.GetManifestResourceNames();

            foreach (string name in resources)
            {
                System.Diagnostics.Debug.WriteLine(name);
            }

            Assembly assembly = Assembly.GetEntryAssembly(); //Assembly.GetExecutingAssembly();
            XDocument xmlDoc = XDocument.Load(assembly.GetManifestResourceStream(this.embeddedResourceName));

            init_log4net();
            init_nlog();
        }

        /// <summary></summary>
        /// <exception cref="FileNotFoundException"></exception>
        private void init_log4net()
        {
#if (NETSTD || NETCOREAPP)
            ILoggerRepository logRep = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
#else
#endif
            if (ResourceLoader.Exists(this.fileName, false))
            {
                this.message = "log4netファイルからロード";
                System.Diagnostics.Debug.WriteLine(this.message);

                // ログ定義 [リソース ファイル] → ストリームを開く
                FileStream s = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

#if (NETSTD || NETCOREAPP)
                XmlConfigurator.Configure(logRep, s);
#else
            XmlConfigurator.Configure(s);
#endif
                s.Close();
            }
            else if (EmbeddedResourceLoader.Exists(this.embeddedResourceName, false))
            {
                this.message = "log4net埋め込まれたリソースからロード";
                System.Diagnostics.Debug.WriteLine(this.message);

                // ログ定義 [埋め込まれたリソース] → XmlDocument
                XmlDocument xmlDef = new XmlDocument();
                xmlDef.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(this.embeddedResourceName));

#if (NETSTD || NETCOREAPP)
                XmlConfigurator.Configure(logRep, (XmlElement)xmlDef["log4net"]);
#else
                XmlConfigurator.Configure(xmlDef["log4net"]);
#endif
            }
            else
            {
                throw new FileNotFoundException("log4netのログ定義ファイルが見つかりません。");
            }
            
            this.logger1 = log4net.LogManager.GetLogger(Assembly.GetEntryAssembly(), "ACCESS");
        }

        /// <summary></summary>
        /// <exception cref="FileNotFoundException"></exception>
        private void init_nlog()
        {
            if (ResourceLoader.Exists(this.fileName_N, false))
            {
                this.message_N = "NLogファイルからロード";
                System.Diagnostics.Debug.WriteLine(this.message_N);

                NLog.LogManager.Configuration = new XmlLoggingConfiguration(this.fileName_N);
            }
            else if (EmbeddedResourceLoader.Exists(this.embeddedResourceName_N, false))
            {
                this.message_N = "NLog埋め込まれたリソースからロード";
                System.Diagnostics.Debug.WriteLine(this.message_N);

                XmlDocument xmlDef = new XmlDocument();
                xmlDef.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(this.embeddedResourceName_N));
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(new XmlNodeReader(xmlDef["nlog"]));
            }
            else
            {
                throw new FileNotFoundException("NLogのログ定義ファイルが見つかりません。");
            }

            this.logger2 = NLog.LogManager.GetLogger("ACCESS");
        }

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            logger1.Error(this.message + " " + this.textBox1.Text);
        }

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            logger2.Error(this.message_N + " " + this.textBox2.Text);
        }
    }
}
