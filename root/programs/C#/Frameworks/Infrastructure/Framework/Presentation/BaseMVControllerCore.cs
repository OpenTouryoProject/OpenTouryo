//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：BaseMVControllerCore
//* クラス日本語名  ：ASP.NET Core MVC用 画面コード親クラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/04/19  西野 大介         新規作成
//**********************************************************************************

using System;

using Touryo.Infrastructure.Framework.StdMigration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Presentation
{
    /// <summary>画面コード親クラス２</summary>
    /// <remarks>（オーバーライドして）自由に利用できる。</remarks>
    public class BaseMVControllerCore : Controller
    {
        /// <summary>
        /// アクション メソッドの呼び出し前に呼び出されます。  
        /// Controller.OnActionExecuting メソッド (Microsoft.AspNetCore.Mvc)
        /// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.controller.onactionexecuting
        /// </summary>
        /// <param name="filterContext">
        /// 型: Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext
        /// 現在の要求およびアクションに関する情報。
        /// </param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Calling base class method.
            base.OnActionExecuting(filterContext);

            #region セッションタイムアウト検出処理（IsNewSessionが無いので要検討）

            //// セッションタイムアウト検出処理の定義を取得
            //string sessionTimeOutCheck =
            //    GetConfigParameter.GetConfigValue(FxLiteral.SESSION_TIMEOUT_CHECK);

            //// デフォルト値対策：設定なし（null）の場合の扱いを決定
            //if (sessionTimeOutCheck == null)
            //{
            //    // OFF扱い
            //    sessionTimeOutCheck = FxLiteral.OFF;
            //}

            //// ON / OFF
            //if (sessionTimeOutCheck.ToUpper() == FxLiteral.ON)
            //{
            //    // セッションタイムアウト検出処理（ON）

            //    // セッション状態の確認
            //    if (Session.IsNewSession)
            //    {
            //        // 新しいセッションが開始された

            //        // セッションタイムアウト検出用Cookieをチェック
            //        HttpCookie cookie = Request.Cookies.Get(FxHttpCookieIndex.SESSION_TIMEOUT);

            //        if (cookie == null)
            //        {
            //            // セッションタイムアウト検出用Cookie無し → 新規のアクセス

            //            // セッションタイムアウト検出用Cookieを新規作成（値は空文字以外、何でも良い）

            //            // Set-Cookie HTTPヘッダをレスポンス
            //            Response.Cookies.Set(FxCmnFunction.CreateCookieForSessionTimeoutDetection());
            //            Session[FxHttpSessionIndex.DUMMY] = "dummy";
            //        }
            //        else
            //        {
            //            // セッションタイムアウト検出用Cookie有り

            //            if (cookie.Value == "")
            //            {
            //                // セッションタイムアウト発生後の新規アクセス

            //                // だが、値が消去されている（空文字に設定されている）場合は、
            //                // 一度エラー or セッションタイムアウトになった後の新規のアクセスである。

            //                // セッションタイムアウト検出用Cookieを再作成（値は空文字以外、何でも良い）

            //                // Set-Cookie HTTPヘッダをレスポンス
            //                Response.Cookies.Set(FxCmnFunction.CreateCookieForSessionTimeoutDetection());
            //                Session[FxHttpSessionIndex.DUMMY] = "dummy";
            //            }
            //            else
            //            {
            //                // セッションタイムアウト発生

            //                // エラー画面で以下の処理を実行する。
            //                // ・セッションタイムアウト検出用Cookieを消去
            //                // ・セッションを消去

            //                // セッションタイムアウト例外を発生させる
            //                throw new FrameworkException(
            //                    FrameworkExceptionMessage.SESSION_TIMEOUT[0],
            //                    FrameworkExceptionMessage.SESSION_TIMEOUT[1]);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // セッション継続中
            //    }
            //}
            //else if (sessionTimeOutCheck.ToUpper() == FxLiteral.OFF)
            //{
            //    // セッションタイムアウト検出処理（OFF）
            //}
            //else
            //{
            //    // パラメータ・エラー（書式不正）
            //    throw new FrameworkException(
            //        FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[0],
            //        String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[1],
            //            FxLiteral.SESSION_TIMEOUT_CHECK));
            //}

            #endregion
        }

        ///// <summary>セッションを消去</summary>
        ///// <remarks>併せてSessionタイムアウト検出用Cookieを消去</remarks>
        //protected void FxSessionAbandon()
        //{
        //    // Set-Cookie HTTPヘッダをレスポンス
        //    Response.Cookies.Set(FxCmnFunction.DeleteCookieForSessionTimeoutDetection());
        //    // セッションを消去
        //    Session.Abandon();
        //}
    }
}
