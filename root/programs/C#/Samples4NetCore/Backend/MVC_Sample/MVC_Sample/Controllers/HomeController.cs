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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Security;

namespace MVC_Sample.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : MyBaseMVControllerCore
    {
        #region 認証不要

        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
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
            //this.FxSessionAbandon();

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
            // 通常ログイン
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    //// 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                    //// 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                    //// を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                    //FormsAuthentication.RedirectFromLoginPage(model.UserName, false);

                    //// 認証情報を保存する。
                    //MyUserInfo ui = new MyUserInfo(model.UserName, Request.UserHostAddress);
                    //UserInfoHandle.SetUserInformation(ui);

                    ////基盤に任せるのでリダイレクトしない。
                    ////return this.Redirect(ReturnUrl);
                    //return new EmptyResult();
                }
                else
                {
                    // ユーザー認証 失敗
                    this.ModelState.AddModelError(string.Empty, "指定されたユーザー名またはパスワードが正しくありません。");
                }
            }
            else
            {
                // LoginViewModelの検証に失敗
            }

            // Session消去
            //this.FxSessionAbandon();

            // ポストバック的な
            return this.View(model);
        }

        /// <summary>
        /// Get: /Home/Error
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion

        #region 認証必要

        /// <summary>
        /// Get: /Home/Scroll
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Scroll()
        {
            return View();
        }

        /// <summary>
        /// Get: /Home/Logout
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult Logout()
        {
            //FormsAuthentication.SignOut();
            return this.Redirect(Url.Action("Index", "Home"));
        }

        #endregion
    }
}
