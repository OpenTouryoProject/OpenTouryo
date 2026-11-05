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
//*  2026/11/05  玄人 幸道         外部ログインを ROPC から LIR（認可コード ＋ PKCE、ループバック）に替えた（#649）
//*                                （MPAS のスタブの Device_Client。id_token を検証し、表示名は UserInfo の email）
//**********************************************************************************

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Security.Pwd;
using Touryo.Infrastructure.Public.Util;

namespace WSClientWinCone_sample
{
    /// <summary>Login</summary>
    public partial class Login : MyBaseControllerWin
    {
        /// <summary>ブラウザからの応答を待つ上限</summary>
        private static readonly TimeSpan ResponseTimeout = TimeSpan.FromMinutes(5);

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

        #region 外部ログイン

        /// <summary>外部ログイン</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>
        /// **LIR（OAuth 2.0 for Native Apps のループバック。RFC 8252 §7.3）で、MPAS にログインする。**
        /// 既定のブラウザで認可し、http://localhost:12345/ で応答を受け、認可コードを PKCE（RFC 7636）で引き換える。
        /// **ROPC（パスワードを、この画面で受けて IdP に送る）は使わない**（RFC 9700 2.4。MPAS の雛形も無効にしている）。
        /// そのため、この画面のユーザ ID・パスワードの欄は、外部ログインでは使わない。
        ///
        /// **ブラウザでの操作を待つので、async で待つ**（UI スレッドを止めない）。
        /// 基盤は UOC メソッドを、戻り値を見ずに呼ぶ（Latebind）ので、async void でよい。
        /// **例外は、ここで受ける**（async void の例外は、基盤の UOC_ABEND に届かない）。
        /// </remarks>
        protected async void UOC_btnButton2_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.btnButton1.Enabled = false;
            this.btnButton2.Enabled = false;

            try
            {
                Tuple<string, string> result = await this.ExLoginAsync();
                if (result != null)
                {
                    // ログインに成功
                    //   **表示名は UserInfo の email**（識別子は id_token の sub。WebApp のサンプルと同じ）
                    MyBaseControllerWin.UserInfo.UserName = result.Item2;
                    MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName;

                    Program.FlagEnd = false; // フラグ完了
                    Program.AccessToken = result.Item1; // AccessToken（リソース サーバへの Bearer）
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "外部ログイン", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!this.IsDisposed)
                {
                    this.btnButton1.Enabled = true;
                    this.btnButton2.Enabled = true;
                }
            }
        }

        /// <summary>外部ログイン（LIR）</summary>
        /// <returns>（access_token, 表示名）。失敗なら null（理由は表示済み）</returns>
        /// <remarks>
        /// 接続先とクライアントは app.config にある（MPAS のスタブと、その雛形の Device_Client）。
        /// **エンドポイントは Discovery から引く**（issuer を SpRp_Isser と照合する）。
        ///
        /// **確かめること**
        /// | 値 | 何のため |
        /// |---|---|
        /// | `state` | 自分が出した要求への応答か（CSRF。RFC 6749 §10.12） |
        /// | `iss`（応答に在れば） | どの IdP の応答か（Mix-Up。RFC 9207） |
        /// | `code_verifier` | コードを横取りされても引き換えられない（RFC 7636） |
        /// | id_token の署名・`iss`・`aud`・`nonce` | OpenID Connect Core 1.0 §3.1.3.7 |
        /// | UserInfo の `sub` | id_token の `sub` と同じか（OpenID Connect Core 1.0 §5.3.2） |
        ///
        /// **アクセス トークンは、ここでは検証しない。** 受けるのはリソース サーバで、クライアントには中身の約束が無い。
        /// </remarks>
        private async Task<Tuple<string, string>> ExLoginAsync()
        {
            OAuth2AndOIDCClient.HttpClient = new HttpClient();

            // Discovery（**issuer を照合する**。OpenID Connect Discovery 1.0 §4.3）
            string json = await OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(new Uri(CmnClientParams.SpRp_DiscoveryUri));
            if (string.IsNullOrEmpty(json))
            {
                return this.Fail("Discovery（" + CmnClientParams.SpRp_DiscoveryUri + "）を読めません。IdP（スタブ）は起動していますか？");
            }

            JObject discovery = JObject.Parse(json);
            string issuer = (string)discovery[OAuth2AndOIDCConst.issuer];
            if (issuer != CmnClientParams.Isser)
            {
                return this.Fail("Discovery の issuer（" + issuer + "）が、期待する値（" + CmnClientParams.Isser + "）と違います。");
            }

            // パラメタ
            //   **state・nonce・code_verifier は、毎回作り直す**（使い回すと、確かめる意味が無くなる）
            string client_id = OAuth2AndOIDCParams.ClientID;
            string redirect_uri = CmnClientParams.SpRp_RedirectUri;
            string scope = GetConfigParameter.GetConfigValue("SpRp_Scope");
            string state = GetPassword.Base64UrlSecret(32);
            string nonce = GetPassword.Base64UrlSecret(32);
            string code_verifier = GetPassword.Base64UrlSecret(32); // 43 文字（RFC 7636 §4.1 の下限）
            string code_challenge = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(code_verifier);

            // **値はエンコードする**（scope の空白など）。**redirect_uri は、トークン要求と同じ値を送る**（RFC 6749 §4.1.3）
            string target = (string)discovery["authorization_endpoint"]
                + "?" + OAuth2AndOIDCConst.client_id + "=" + Uri.EscapeDataString(client_id)
                + "&" + OAuth2AndOIDCConst.response_type + "=" + OAuth2AndOIDCConst.AuthorizationCodeResponseType
                + "&" + OAuth2AndOIDCConst.scope + "=" + Uri.EscapeDataString(scope)
                + "&" + OAuth2AndOIDCConst.redirect_uri + "=" + Uri.EscapeDataString(redirect_uri)
                + "&" + OAuth2AndOIDCConst.state + "=" + Uri.EscapeDataString(state)
                + "&" + OAuth2AndOIDCConst.nonce + "=" + Uri.EscapeDataString(nonce)
                + "&" + OAuth2AndOIDCConst.code_challenge + "=" + Uri.EscapeDataString(code_challenge)
                + "&" + OAuth2AndOIDCConst.code_challenge_method + "=" + OAuth2AndOIDCConst.PKCE_S256;

            // 認可リクエスト・レスポンス
            NameValueCollection query = await Login.AuthorizeInBrowserAsync(target, redirect_uri);
            if (query == null)
            {
                return this.Fail("ブラウザからの応答を受けられませんでした（" + Login.ResponseTimeout.TotalMinutes + " 分）。");
            }

            if (!string.IsNullOrEmpty(query[OAuth2AndOIDCConst.error]))
            {
                return this.Fail("認可が断られました : " + query[OAuth2AndOIDCConst.error]);
            }

            if (query[OAuth2AndOIDCConst.state] != state)
            {
                return this.Fail("state が一致しません（自分が出した要求への応答ではない）。");
            }

            string iss = query[OAuth2AndOIDCConst.iss];
            if (!string.IsNullOrEmpty(iss) && iss != issuer)
            {
                return this.Fail("応答の iss（" + iss + "）が、期待する IdP と違います。");
            }

            string code = query[OAuth2AndOIDCConst.code];

            // Tokenリクエスト（**パブリック クライアントなので、秘密は送らない**）
            JObject token = JObject.Parse(await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                new Uri((string)discovery["token_endpoint"]), client_id, "", redirect_uri, code, code_verifier));

            if (token.ContainsKey(OAuth2AndOIDCConst.error))
            {
                return this.Fail("トークン要求が断られました : " + (string)token[OAuth2AndOIDCConst.error]);
            }

            string access_token = (string)token[OAuth2AndOIDCConst.AccessToken];
            string id_token = (string)token[OAuth2AndOIDCConst.IDToken];

            // id_tokenの検証
            string sub = string.IsNullOrEmpty(id_token) ? null
                : await Login.VerifyIdTokenAsync(id_token, access_token, code, issuer, client_id, nonce);
            if (sub == null)
            {
                return this.Fail("id_token の検証に失敗しました（無い、署名・iss・aud・exp・ハッシュ・nonce のいずれか）。");
            }

            // UserInfoリクエスト（表示名を得る）
            string displayName = Login.GetDisplayName(await OAuth2AndOIDCClient.GetUserInfoAsync(
                new Uri((string)discovery["userinfo_endpoint"]), access_token), sub);
            if (displayName == null)
            {
                return this.Fail("UserInfo を得られません（エラーの応答、または sub が id_token と違う）。");
            }

            return Tuple.Create(access_token, displayName);
        }

        /// <summary>失敗を表示する</summary>
        /// <param name="message">理由</param>
        /// <returns>null</returns>
        private Tuple<string, string> Fail(string message)
        {
            MessageBox.Show(this, message, "外部ログイン", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        /// <summary>ブラウザで認可要求を出し、ループバックで応答を受ける</summary>
        /// <param name="target">認可要求の URL</param>
        /// <param name="redirect_uri">戻り先（ループバック）</param>
        /// <returns>応答のクエリ（待ちきれなければ null）</returns>
        /// <remarks>
        /// **戻り先は IdP に登録した値と、1 文字も違ってはならない**（末尾の / も含む）。
        /// **ブラウザを開く前に待ち受ける**（先に開くと、応答を取りこぼすことがある）。
        /// 待ち受けられない（ポートが使用中など）ときは、HttpListenerException が上がる。
        /// </remarks>
        private static async Task<NameValueCollection> AuthorizeInBrowserAsync(string target, string redirect_uri)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(redirect_uri);

            try
            {
                listener.Start();

                // **UseShellExecute で既定のブラウザに渡す**（cmd /c start は & を解釈してしまう）
                Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });

                Task<HttpListenerContext> getContext = listener.GetContextAsync();
                if (await Task.WhenAny(getContext, Task.Delay(Login.ResponseTimeout)) != getContext)
                {
                    return null;
                }

                HttpListenerContext context = getContext.Result;
                NameValueCollection query = context.Request.QueryString;

                // 返信（**結果だけを返す。トークンは画面に出さない**）
                byte[] buffer = Encoding.UTF8.GetBytes(
                    "<HTML><BODY>Accepted the authorization response. You can close this tab.</BODY></HTML>");
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();

                return query;
            }
            finally
            {
                listener.Close();
            }
        }

        /// <summary>相手（OP）の id_token を、jwks_uri の鍵で検証する</summary>
        /// <param name="id_token">id_token</param>
        /// <param name="access_token">access_token（at_hash の検証）</param>
        /// <param name="code">code（c_hash の検証）</param>
        /// <param name="issuer">期待する iss</param>
        /// <param name="client_id">期待する aud</param>
        /// <param name="expectedNonce">認可要求で送った nonce</param>
        /// <returns>sub（検証に失敗したら null）</returns>
        /// <remarks>
        /// **鍵は、ヘッダの kid で jwks_uri から引く**（RemoteJwkSetStore。SpRp_DiscoveryUri の Discovery）。
        /// **alg は非対称のもの（JwsFactory.SupportedAlgs）に限る**（none・HS* を受けない）。
        /// </remarks>
        private static async Task<string> VerifyIdTokenAsync(
            string id_token, string access_token, string code,
            string issuer, string client_id, string expectedNonce)
        {
            string alg = JwtReader.TryReadAlg(id_token);
            if (!JwsFactory.IsSupported(alg)) return null;

            JObject jwk = await RemoteJwkSetStore.GetJwkByDiscoveryAsync(JwtReader.TryReadKid(id_token));
            if (jwk == null) return null;

            JWS jws = JwsFactory.CreateFromJwk(alg, jwk);
            if (jws == null) return null;

            // **署名・iss・aud・exp と、at_hash / c_hash を確かめる**（IdToken.Verify。#601）
            string sub = "";
            string nonce = "";
            JObject claims = null;
            if (!IdToken.Verify(id_token, access_token, code, "",
                jws, issuer, client_id, out sub, out nonce, out claims))
            {
                return null;
            }

            // **nonce は、呼ぶ側が確かめる**（IdToken.Verify は取り出すだけ。OpenID Connect Core 1.0 §3.1.3.7 の 11）
            return (nonce == expectedNonce) ? sub : null;
        }

        /// <summary>UserInfo の応答から、表示名を得る</summary>
        /// <param name="userInfoJson">UserInfo の応答</param>
        /// <param name="sub">id_token の sub</param>
        /// <returns>email（無ければ sub）。エラーの応答や、sub が違えば null</returns>
        /// <remarks>
        /// **エラーの応答も JSON で返る**（insufficient_scope など。RFC 6750 §3.1）。
        /// **UserInfo の sub は、id_token の sub と一致しなければならない**（OpenID Connect Core 1.0 §5.3.2）。
        /// </remarks>
        private static string GetDisplayName(string userInfoJson, string sub)
        {
            JObject userInfo;
            try
            {
                userInfo = JObject.Parse(userInfoJson);
            }
            catch (JsonReaderException)
            {
                return null;
            }

            if (userInfo.ContainsKey(OAuth2AndOIDCConst.error)) return null;
            if ((string)userInfo[OAuth2AndOIDCConst.sub] != sub) return null;

            string email = (string)userInfo["email"]; // email クレーム（OpenID Connect Core 1.0 §5.1）
            return string.IsNullOrEmpty(email) ? sub : email;
        }

        #endregion
    }
}
