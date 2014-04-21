using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Diagnostics;

using Touryo.Infrastructure.Public.IO;
using AuthServiceClient.ASPNETWebService;

namespace AuthServiceClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>認証Serviceのテスト（秘密鍵暗号化方式）</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // SessionステートフルなWebサービス
            ServiceForAuth sfa = new ServiceForAuth();
            sfa.CookieContainer = new CookieContainer();

            sfa.Proxy = GlobalProxySelection.GetEmptyWebProxy();
            //sfa.Proxy = WebProxy.GetDefaultProxy();
            //sfa.Proxy.Credentials = new NetworkCredential("20228749", "*****");

            // チャレンジ＝秘密鍵
            string challenge = sfa.GetChallenge();
            
            // アカウントの暗号化
            string encUid = SymmetricCryptography.EncryptString(
                    this.textBox1.Text, challenge, EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);
            string encPwd = SymmetricCryptography.EncryptString(
                    this.textBox2.Text, challenge, EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);

            // 認証チケットを取得
            string authTicketBase64 = sfa.GetAuthTicket(encUid, encPwd);
            // 認証チケットを検証
            string[] authTicket = sfa.ValidateAuthTicket(authTicketBase64);

            MessageBox.Show(
                "uid\t: " + authTicket[0] + "\r\n"
                + "pwd\t: " + authTicket[1] + "\r\n"
                + "time\t: " + authTicket[2] + "\r\n", "認証チケット",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>公開鍵暗号化方式</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string publicKey ="";
            string privateKey ="";

            UnSymmetricCryptography.GetKeys(
                out publicKey, out privateKey);

            Debug.WriteLine("");
            Debug.WriteLine("publicKey\t: " + publicKey);
            Debug.WriteLine("privateKey\t: " + privateKey);

            string encUid = UnSymmetricCryptography.EncryptString(this.textBox1.Text, publicKey);
            string encPwd = UnSymmetricCryptography.EncryptString(this.textBox2.Text, publicKey);

            Debug.WriteLine("");
            Debug.WriteLine("encUid\t: " + encUid);
            Debug.WriteLine("encPwd\t: " + encPwd);

            string uid = UnSymmetricCryptography.DecryptString(encUid, privateKey);
            string pwd = UnSymmetricCryptography.DecryptString(encPwd, privateKey);

            Debug.WriteLine("");
            Debug.WriteLine("uid\t: " + uid);
            Debug.WriteLine("pwd\t: " + pwd);
        }
        
    }
}