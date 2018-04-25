//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Crud1Controller
//* クラス日本語名  ：knockout.js用サンプル アプリ・コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.Mvc;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;

namespace SPA_Sample.Controllers
{
    /// <summary>
    /// Crud1Controller
    /// knockout.js用サンプル アプリ・コントローラ
    /// </summary>
    public class Crud1Controller : Controller
    {
        /// <summary>Nonce</summary>
        public string Nonce
        {
            get
            {
                if (Session["nonce"] == null)
                {
                    Session["nonce"] = GetPassword.Base64UrlSecret(10);
                }
                return (string)Session["nonce"];
            }
        }

        /// <summary>State</summary>
        public string State
        {
            get
            {
                if (Session["state"] == null)
                {
                    Session["state"] = GetPassword.Base64UrlSecret(10);
                }
                return (string)Session["state"];
            }
        }

        /// <summary>
        /// GET: /Crud1/
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Crud1/GetAccessToken
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult GetAccessToken()
        {
            // 外部ログイン
            return Redirect(string.Format(
                "http://localhost:63359/MultiPurposeAuthSite/Account/OAuthAuthorize"
                + "?client_id=" + OAuth2AndOIDCParams.ClientID
                + "&response_type=token" 
                + "&scope=profile%20email%20phone%20address%20roles"
                + "&state={0}"
                + "&nonce={1}",
                this.State, this.Nonce));
        }

        /// <summary>
        /// GET: /Crud1/Implicit
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Implicit()
        {
            this.ClearExLoginsParams();
            return View();
        }

        /// <summary>ClearExLoginsParam</summary>
        private void ClearExLoginsParams()
        {
            Session["nonce"] = null;
            Session["state"] = null;
        }
    }
}
