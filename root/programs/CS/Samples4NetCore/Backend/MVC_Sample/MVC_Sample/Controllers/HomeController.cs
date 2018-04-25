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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;

namespace MVC_Sample.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : MyBaseMVControllerCore
    {
        #region 認証不要

        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Login
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Session消去
            //this.FxSessionAbandon();

            return this.View();
        }

        /// <summary>
        /// POST: /Home/Login
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>IActionResultを非同期的に返す。</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // 通常ログイン
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    // 認証情報を作成する。
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, model.UserName));

                    // 認証情報を保存する。
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);

                    // サイン アップする。
                    await AuthenticationHttpContextExtensions.SignInAsync(
                        this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                    //基盤に任せるのでリダイレクトしない。
                    return View(model);

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
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            //FormsAuthentication.SignOut();
            await AuthenticationHttpContextExtensions.SignOutAsync(
                this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);

            return this.Redirect(Url.Action("Index", "Home"));
        }

        #endregion
    }
}
