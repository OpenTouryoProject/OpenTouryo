//**********************************************************************************
//* ３層型 サンプル アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：login
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Owin.Security.DataHandler.Encoder;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace WSClientWin_sample
{
    /// <summary>login</summary>
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
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:63359/MultiPurposeAuthSite/OAuthBearerToken"),
            };

            // HttpRequestMessage (Headers & Content)
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", new string[] { OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret }))));

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                            { "grant_type", "password" },
                            { "username", userId },
                            { "password", password },
                            { "scope", "profile email phone address roles" },
                });

            // HttpResponseMessage
            httpResponseMessage = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            string response = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
            Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
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