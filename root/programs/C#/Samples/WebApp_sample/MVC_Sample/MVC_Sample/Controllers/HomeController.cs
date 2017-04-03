//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：HomeController
//* クラス日本語名  ：認証用サンプル アプリ・コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using MVC_Sample.Models.ViewModels;

using System.Web.Mvc;
using System.Web.Security;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;

namespace MVC_Sample.Controllers
{
    /// <summary>HomeController</summary>
    [Authorize]
    public class HomeController : MyBaseMVController
    {
        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Login
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Session消去
            this.FxSessionAbandon();

            return this.View();
        }

        /// <summary>
        /// POST: /Home/Login
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!string.IsNullOrEmpty(model.UserName))
            {
                // 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                // 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                // を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                FormsAuthentication.RedirectFromLoginPage(model.UserName, false);

                // 認証情報を保存する。
                MyUserInfo ui = new MyUserInfo(model.UserName, Request.UserHostAddress);
                UserInfoHandle.SetUserInformation(ui);

                //基盤に任せるのでリダイレクトしない。
                //return this.Redirect(ReturnUrl);
                return new EmptyResult();
            }
            else
            {
                // ユーザー認証 失敗
                this.ModelState.AddModelError(string.Empty, "指定されたユーザー名またはパスワードが正しくありません。");

                // Session消去
                this.FxSessionAbandon();

                // ポストバック的な
                return this.View(model);
            }
        }

        /// <summary>
        /// Get: /Home/Scroll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Scroll()
        {
            return this.View();
        }

        /// <summary>
        /// Get: /Home/Logout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.Redirect(Url.Action("Index", "Home"));
        }
    }
}