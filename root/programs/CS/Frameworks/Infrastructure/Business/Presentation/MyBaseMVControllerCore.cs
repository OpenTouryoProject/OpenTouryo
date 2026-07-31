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
//*  2021/05/23  西野 大介         キャッシュ制御ヘッダの二重追加エラーの対応
//*  2026/07/31  玄人 幸道         net48版から移植漏れのアクセスログ出力点を追加
//*                                （View、OnResultExecuting、OnResultExecuted）
//**********************************************************************************

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
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
    /// <remarks>
    /// IResultFilter を実装しているのは、ASP.NET Core の Controller が
    /// IActionFilter / IAsyncActionFilter しか実装しておらず、
    /// OnResultExecuting / OnResultExecuted を override できないため。
    /// コントローラが IResultFilter を実装していると、MVC が ControllerResultFilter を
    /// 自動的にフィルタ パイプラインへ追加し、本クラスの実装を呼び出す。
    /// （フィルタ属性ではなくコントローラ側に実装するのは、属性インスタンスが
    /// 　リクエスト間で共有され、性能測定用の状態を持たせられないため。）
    /// </remarks>
    [MyMVCCoreFilter()]
    public class MyBaseMVControllerCore : BaseMVControllerCore, IResultFilter
    {
        /// <summary>性能測定（アクション実行区間）</summary>
        private PerformanceRecorder perfRec;

        /// <summary>性能測定（結果実行＝レンダリング区間）</summary>
        /// <remarks>
        /// アクション実行区間の perfRec とは別インスタンスにする。
        /// .NET (Core) 版の PerformanceRecorder は Stopwatch ベースで、
        /// EndsPerformanceRecord の後に再度 Ends を呼んでも値が変わらないため、
        /// perfRec を使い回すとレンダリング区間が測定できないため。
        /// </remarks>
        private PerformanceRecorder perfRecOfResult;

        /// <summary>UserInfo</summary>
        protected MyUserInfo UserInfo;

        /// <summary>ログのユーザ名・IPアドレス部を生成する</summary>
        /// <returns>",ユーザ名,IPアドレス"</returns>
        /// <remarks>
        /// Session が OFF の場合は UserInfo が null のままになり得るため、
        /// ログ出力が原因で例外を出さないよう null を考慮する。
        /// </remarks>
        private string GetLogUserPart()
        {
            return "," + (this.UserInfo != null ? this.UserInfo.UserName : "")
                 + "," + (this.UserInfo != null ? this.UserInfo.IPAddress : "");
        }

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

        #region View

        /// <summary>
        /// 応答にビューを表示する ViewResult オブジェクトを作成します。
        /// Controller.View メソッド (Microsoft.AspNetCore.Mvc)
        /// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.controller.view
        /// </summary>
        /// <param name="viewName">ビュー名</param>
        /// <param name="model">モデル</param>
        /// <returns>ViewResult オブジェクト</returns>
        /// <remarks>
        /// net48 版はマスタ ページを取る View(IView, object) / View(string, string, object) を
        /// オーバーライドしているが、ASP.NET Core にマスタ ページの概念は無い。
        /// Core では他の View オーバーロードが最終的に本メソッドへ集約されるため、ここだけで足りる。
        /// </remarks>
        [NonAction]
        public override ViewResult View(string viewName, object model)
        {
            ViewResult vr = base.View(viewName, model);

            // View() / View(model) で呼ばれた場合、ViewName は null になる
            // （実行時にアクション名で解決されるため）。その場合はアクション名を使う。
            string vn = string.IsNullOrEmpty(vr.ViewName) ? this.ActionName : vr.ViewName;
            string[] temp = vn.Split('.');

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                this.GetLogUserPart() +
                "," + "----->>" +
                "," + this.ControllerName +
                "," + this.ActionName + " -> " + temp[temp.Length - 1];

            LogIF.InfoLog("ACCESS", strLogMessage);

            return vr;
        }

        #endregion

        #region OnResult

        /// <summary>
        /// アクション メソッドによって返されたアクション結果が実行される前に呼び出されます。
        /// Controller.OnResultExecuting メソッド (Microsoft.AspNetCore.Mvc)
        /// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.controller.onresultexecuting
        /// </summary>
        /// <param name="context">
        /// 型: Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 結果実行（レンダリング）区間の性能測定を開始する。
            this.perfRecOfResult = new PerformanceRecorder();
            this.perfRecOfResult.StartsPerformanceRecord();

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                this.GetLogUserPart() +
                "," + "----->" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnResultExecuting)";

            LogIF.DebugLog("ACCESS", strLogMessage);
        }

        /// <summary>
        /// アクション メソッドによって返されたアクション結果が実行された後に呼び出されます。
        /// Controller.OnResultExecuted メソッド (Microsoft.AspNetCore.Mvc)
        /// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.controller.onresultexecuted
        /// </summary>
        /// <param name="context">
        /// 型: Microsoft.AspNetCore.Mvc.Filters.ResultExecutedContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        public void OnResultExecuted(ResultExecutedContext context)
        {
            // OnResultExecuting より前にエラーが発生した場合は、
            // perfRecOfResult が null の場合があるので、null 対策コードを挿入する。
            if (this.perfRecOfResult == null)
            {
                // null の場合、新しいインスタンスを生成し、性能測定開始。
                this.perfRecOfResult = new PerformanceRecorder();
                this.perfRecOfResult.StartsPerformanceRecord();
            }

            this.perfRecOfResult.EndsPerformanceRecord();

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                this.GetLogUserPart() +
                "," + "<-----" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnResultExecuted)" +
                "," + this.perfRecOfResult.ExecTime +
                "," + this.perfRecOfResult.CpuTime;

            LogIF.DebugLog("ACCESS", strLogMessage);
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
                MyHttpContext.Current.Response.Headers.Remove("Cache-Control");
                MyHttpContext.Current.Response.Headers.Add("Cache-Control",
                    new StringValues(new string[] { "no-cache", "no-store", "must-revalidate" }));
                MyHttpContext.Current.Response.Headers.Remove("Pragma");
                MyHttpContext.Current.Response.Headers.Add("Pragma", new StringValues("no-cache"));
                MyHttpContext.Current.Response.Headers.Remove("Expires");
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
