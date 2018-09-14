//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：ErrorController
//* クラス日本語名  ：ErrorController
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
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Mvc;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.StdMigration;
using Touryo.Infrastructure.Framework.Util;

namespace MVC_Sample.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ErrorController : MyBaseMVControllerCore
    {
        /// <summary>
        /// GET: Error/Index
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            ISession session = MyHttpContext.Current.Session;
            int? flg = session.GetInt32(FxHttpContextIndex.SESSION_ABANDON_FLAG);
            if (flg.HasValue)
            {
                if (Convert.ToBoolean(flg.Value))
                {
                    // セッション タイムアウト検出用Cookieを消去
                    // ※ Removeが正常に動作しないため、値を空文字に設定 ＝ 消去とする

                    // Set-Cookie HTTPヘッダをレスポンス
                    FxCmnFunction.DeleteCookieForSessionTimeoutDetection();

                    try
                    {
                        // セッションを消去                       
                        session.Clear();
                    }
                    catch (Exception ex)
                    {
                        // エラー発生時
                        // このカバレージを通過する場合、
                        // おそらく起動した画面のパスが間違っている。
                        Console.WriteLine("このカバレージを通過する場合、おそらく起動した画面のパスが間違っている。");
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
