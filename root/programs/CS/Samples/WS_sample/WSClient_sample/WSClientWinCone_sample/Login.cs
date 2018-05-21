//**********************************************************************************
//* ３層型 サンプル アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Login
//* クラス日本語名  ：ログイン画面
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace WSClientWinCone_sample
{
    /// <summary>Login</summary>
    public partial class Login : MyBaseControllerWin
    {
        /// <summary>コンストラクタ</summary>
        public Login()
        {
            InitializeComponent();

            Program.FlagEnd = true; //フラグ初期化
        }

        /// <summary>フォームロードのUOCメソッド</summary>
        protected override void UOC_FormInit()
        {   
        }

        /// <summary>ログイン</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            MyBaseControllerWin.UserInfo.UserName = this.textBox1.Text;
            MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName;

            Program.FlagEnd = false; // フラグ完了
            this.Close();
        }

        /// <summary>外部ログイン</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton2_Click(RcFxEventArgs rcFxEventArgs)
        {
            string access_token = this.ExLogin(this.textBox1.Text, this.textBox2.Text).Result;
            if (!string.IsNullOrEmpty(access_token))
            {
                MyBaseControllerWin.UserInfo.UserName = this.textBox1.Text;
                MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName;

                Program.FlagEnd = false; // フラグ完了
                Program.AccessToken = access_token; // AccessToken
                this.Close();
            }
        }

        /// <summary>外部ログイン</summary>
        /// <param name="userId">string</param>
        /// <param name="password">string</param>
        /// <returns>access_token</returns>
        private async Task<string> ExLogin(string userId, string password)
        {
            OAuth2AndOIDCClient.HttpClient = new HttpClient();
            string response = await OAuth2AndOIDCClient.GetAccessTokenByROPAsync(
                new Uri("http://localhost:63359/MultiPurposeAuthSite/OAuthBearerToken"),
                OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret,
                userId, password, "profile email phone address roles").ConfigureAwait(false);

            // access_tokenを取得し、検証
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            // access_tokenの検証コード
            if (dic.ContainsKey("access_token"))
            {
                string access_token = dic["access_token"];

                string sub = "";
                List<string> roles = null;
                List<string> scopes = null;
                JObject jobj = null;

                if (JwtToken.Verify(access_token, out sub, out roles, out scopes, out jobj))
                {
                    // ログインに成功
                    return access_token;
                }
                else
                {
                    // ログインに失敗
                    return "";
                }
            }
            else
            {
                // ログインに失敗
                return "";
            }
        }
    }
}