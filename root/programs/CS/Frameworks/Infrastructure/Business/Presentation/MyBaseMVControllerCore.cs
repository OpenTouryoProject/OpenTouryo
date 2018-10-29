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
//* クラス名        ：MyBaseMVControllerCore
//* クラス日本語名  ：ASP.NET MVC Core用 画面コード親クラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/04/19  西野 大介         新規作成
//*  2018/07/19  西野 大介         復元後のユーザー情報をSessionに設定するコードを追加
//*  2018/08/08  西野 大介         MyMVCCoreFilterAttributeをFilterAttributeとして設定
//**********************************************************************************

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.StdMigration;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

#region イベント実行順
// お楽しみはこれからだ！: イベントの実行順が面白くて
// http://takepara.blogspot.jp/2008/08/blog-post.html
//
// before Execute
//
// - OnAuthorization
//
// - OnActionExecuting
// -- Index action execute ← ここでアクション実行
// -- View
// - OnActionExecuted
//
// - OnResultExecuting
// -- page rendering ← ここでレンダリング
// - OnResultExecuted
//
// after Execute
#endregion

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>画面コード親クラス２</summary>
    /// <remarks>（オーバーライドして）自由に利用できる。</remarks>
    [MyMVCCoreFilter()]
    public class MyBaseMVControllerCore : BaseMVControllerCore
    {
        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        /// <summary>UserInfo</summary>
        protected MyUserInfo UserInfo;

        #region OnAction

        ///// <summary>
        ///// アクション メソッドの呼び出し前に呼び出されます。  
        ///// Controller.OnActionExecuting メソッド (Microsoft.AspNetCore.Mvc)
        ///// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.controller.onactionexecuting
        ///// </summary>
        ///// <param name="filterContext">
        ///// 型: Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext
        ///// 現在の要求およびアクションに関する情報。
        ///// </param>
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // OnActionExecutionAsyncに移行
        //}

        ///// <summary>
        ///// アクション メソッドの呼び出し後に呼び出されます。  
        ///// Controller.OnActionExecuted メソッド (Microsoft.AspNetCore.Mvc)
        ///// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.controller.onActionexecuted
        ///// </summary>
        ///// <param name="filterContext">
        ///// 型: Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext
        ///// 現在の要求およびアクションに関する情報。
        ///// </param>
        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    // OnActionExecutionAsyncに移行
        //}

        /// <summary>
        /// Controller.OnActionExecutionAsync メソッド (Microsoft.AspNetCore.Mvc)
        /// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.controller.onactionexecutionasync
        /// </summary>
        /// <param name="context">Filters.ActionExecutedContext</param>
        /// <param name="next">ActionExecutionDelegate</param>
        /// <returns>Task</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 性能測定
            this.perfRec = null;
            string strLogMessage = "";

            #region OnActionExecuting に相当する処理

            this.GetRouteData(context.RouteData);

            // カスタム認証処理 --------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // 認証ユーザ情報をメンバにロードする --------------------------
            await this.GetUserInfoAsync();
            // -------------------------------------------------------------

            // 権限チェック ------------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // 閉塞チェック ------------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // キャッシュ制御処理 ------------------------------------------
            this.CacheControlWithSwitch();
            // -------------------------------------------------------------

            // 性能測定開始
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            strLogMessage =
                "," + this.UserInfo.UserName +
                "," + this.UserInfo.IPAddress +
                "," + "----->" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnActionExecuting)";

            LogIF.InfoLog("ACCESS", strLogMessage);

            #endregion

            await base.OnActionExecutionAsync(context, next);

            #region OnActionExecuted に相当する処理

            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            strLogMessage =
                "," + this.UserInfo.UserName +
                "," + this.UserInfo.IPAddress +
                "," + "<-----" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnActionExecuted)" +
                "," + perfRec.ExecTime +
                "," + perfRec.CpuTime;

            LogIF.InfoLog("ACCESS", strLogMessage);

            #endregion
        }

        #endregion

        #region 情報取得用

        /// <summary>ユーザ情報を取得する</summary>
        private async Task GetUserInfoAsync()
        {
            // セッションステートレス対応
            if (MyHttpContext.Current.Session == null)
            {
                // SessionがOFFの場合
            }
            else
            {
                // 取得を試みる。
                this.UserInfo = UserInfoHandle.GetUserInformation<MyUserInfo>();

                // nullチェック
                if (this.UserInfo == null)
                {
                    AuthenticateResult authenticateInfo =
                        await AuthenticationHttpContextExtensions.AuthenticateAsync(
                            MyHttpContext.Current, CookieAuthenticationDefaults.AuthenticationScheme);

                    //await MyHttpContext.Current.Authentication.GetAuthenticateInfoAsync(
                    //    CookieAuthenticationDefaults.AuthenticationScheme); // 古い

                    //System.Threading.Thread.CurrentPrincipal.Identity.Name; // .NET Framework

                    string userName = authenticateInfo.Principal?.Identity?.Name; // null 条件演算子

                    if (string.IsNullOrEmpty(userName))
                    {
                        // 未認証状態
                        this.UserInfo = new MyUserInfo("未認証", (new GetClientIpAddress()).GetAddress());
                    }
                    else
                    {
                        // 認証状態
                        this.UserInfo = new MyUserInfo(userName, (new GetClientIpAddress()).GetAddress());

                        // 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                        // ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
                        // ・・・

                        // 復元したユーザ情報をセット
                        UserInfoHandle.SetUserInformation(this.UserInfo);
                    }
                }
            }
        }

        /// <summary>ルーティング情報を取得する</summary>
        /// <param name="routeData">RouteData</param>
        private void GetRouteData(RouteData routeData)
        {
            string[] temp = null;
            temp = routeData.Values["controller"].ToString().Split('.');
            this.ControllerName = routeData.Values["controller"].ToString();
            this.ActionName = routeData.Values["action"].ToString();
        }

        /// <summary>キャッシュ制御処理（スイッチ付き）</summary>
        private void CacheControlWithSwitch()
        {
            // システムで固定に出来る場合は、ここでキャッシュ無効化する。
            // また、ユーザープログラムのファイル・ダウンロード処理などで
            // フレームワークの設定したキャッシュ制御を変更したい場合は、Response.Clearを実行して再設定する。

            // 画面遷移方法の定義を取得
            string noCache = GetConfigParameter.GetConfigValue(MyLiteral.CACHE_CONTROL);

            // デフォルト値対策：設定なし（null）の場合の扱いを決定
            if (noCache == null)
            {
                // OFF扱い
                noCache = FxLiteral.OFF;
            }

            if (noCache.ToUpper() == FxLiteral.ON)
            {
                // ON

                // http - How to control web page caching, across all browsers? - Stack Overflow
                // http://stackoverflow.com/questions/49547/how-to-control-web-page-caching-across-all-browsers

                // IPA ISEC　セキュア・プログラミング講座：Webアプリケーション編　第5章 暴露対策：プロキシキャッシュ対策
                // https://www.ipa.go.jp/security/awareness/vendor/programmingv2/contents/405.html

                // Using ASP.NET-MVC:
                //this.Response.Cache.SetCacheability(HttpCacheability.NoCache);  // HTTP 1.1.
                //this.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
                MyHttpContext.Current.Response.Headers.Add("Cache-Control",
                    new StringValues(new string[] { "no-cache", "no-store", "must-revalidate" }));

                //this.Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                MyHttpContext.Current.Response.Headers.Add("Pragma", new StringValues("no-cache"));
                //this.Response.AppendHeader("Expires", "0"); // Proxies.
                MyHttpContext.Current.Response.Headers.Add("Expires", new StringValues("0"));
            }
            else if (noCache.ToUpper() == FxLiteral.OFF)
            {
                // OFF
            }
            else
            {
                // パラメータ・エラー（書式不正）
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[1],
                        MyLiteral.CACHE_CONTROL));
            }
        }

        #endregion
    }
}
