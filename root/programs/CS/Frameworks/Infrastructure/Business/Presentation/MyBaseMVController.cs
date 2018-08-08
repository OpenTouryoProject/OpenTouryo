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
//* クラス名        ：MyBaseMVController
//* クラス日本語名  ：ASP.NET MVC用 画面コード親クラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/08/04  Supragyan         Added code for SessionTimeout to OnActionExecuting method.
//*  2015/08/31  Supragyan         Modified OnException method to display error message on Error screen
//*  2015/09/03  Supragyan         Modified ExceptionType,Session,RedireResult on OnException method 
//*  2015/10/27  Sai               Moved the code of SessionTimeout from OnActionExecuting method to BaseMVController class.  
//*  2015/10/30  Sai               Added else part to the filterContext If statement in OnException method to resolve
//*                                the exception occurs in the redirection method in the child action as per the comments in Github.  
//*  2015/11/03  Sai               Implemeted performance measurement in the methods
//*                                OnActionExecuting, OnActionExecuted, OnResultExecuting and OnResultExecuted
//*  2017/01/23  西野 大介         UserInfoプロパティとGetUserInfoメソッドを追加した。
//*  2017/01/24  西野 大介         ControllerName, ActionNameプロパティとGetRouteDataメソッドを追加した。
//*  2017/01/24  西野 大介         ログ出力の見直し（OnResultメソッドではDebugを使用、ViewではViewNameを表示。）
//*  2017/01/24  西野 大介         ログ出力の見直し（ログ出力フォーマットの全面的な見直し）
//*  2017/02/14  西野 大介         OnException内での null reference 対策を行った。
//*  2017/02/14  西野 大介         スイッチ付きのキャッシュ無効化処理を追加した。
//*  2017/02/28  西野 大介         OnExceptionのErrorMessage生成処理の見直し。
//*  2017/02/28  西野 大介         TransferErrorScreenメソッドを追加した。
//*  2017/02/28  西野 大介         エラーログの見直し（その他の例外の場合、ex.ToString()を出力）
//*  2018/07/19  西野 大介         復元後のユーザー情報をSessionに設定するコードを追加
//**********************************************************************************

using System;

using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

using Touryo.Infrastructure.Business.Util;
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
    public class MyBaseMVController : BaseMVController
    {
        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        /// <summary>UserInfo</summary>
        protected MyUserInfo UserInfo;

        /// <summary>ControllerName</summary>
        protected string ControllerName = "";

        /// <summary>ActionName</summary>
        protected string ActionName = "";
        
        #region OnAction

        /// <summary>
        /// アクション メソッドの呼び出し前に呼び出されます。  
        /// Controller.OnActionExecuting メソッド (System.Web.Mvc)
        /// http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onactionexecuting.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ActionExecutingContext
        /// 現在の要求およびアクションに関する情報。
        /// </param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.GetRouteData();

            // カスタム認証処理 --------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // 認証ユーザ情報をメンバにロードする --------------------------
            this.GetUserInfo();
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

            // Calling base class method.
            base.OnActionExecuting(filterContext);

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + this.UserInfo.UserName + 
                "," + this.UserInfo.IPAddress +
                "," + "----->" +
                "," + this.ControllerName + 
                "," + this.ActionName + "(OnActionExecuting)";

            LogIF.InfoLog("ACCESS", strLogMessage);

        }

        /// <summary>
        /// アクション メソッドの呼び出し後に呼び出されます。
        /// Controller.OnActionExecuted メソッド (System.Web.Mvc)
        /// http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onactionexecuted.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ActionExecutedContext
        /// 現在の要求およびアクションに関する情報。
        /// </param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Calling base class method.
            base.OnActionExecuted(filterContext);

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
            string strLogMessage =
                "," + this.UserInfo.UserName +
                "," + this.UserInfo.IPAddress +
                "," + "<-----" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnActionExecuted)" +
                "," + perfRec.ExecTime +
                "," + perfRec.CpuTime;

            LogIF.InfoLog("ACCESS", strLogMessage);
        }

        #region View

        /// <summary>
        /// 応答にビューを表示する ViewResult オブジェクトを作成します。
        /// Controller.View メソッド (System.Web.Mvc)
        /// http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.view.aspx
        /// </summary>
        /// <param name="view">ビュー</param>
        /// <param name="model">モデル</param>
        /// <returns>ViewResult オブジェクト</returns>
        protected override ViewResult View(IView view, object model)
        {
            ViewResult vr = base.View(view, model);
            string[] temp = vr.ViewName.Split('.');

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + this.UserInfo.UserName +
                "," + this.UserInfo.IPAddress +
                "," + "----->>" +
                "," + this.ControllerName +
                "," + this.ActionName + " -> " + temp[temp.Length - 1];

            LogIF.InfoLog("ACCESS", strLogMessage);

            return vr;
        }

        /// <summary>
        /// 応答にビューを表示する ViewResult オブジェクトを作成します。
        /// Controller.View メソッド (System.Web.Mvc)
        /// http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.view.aspx
        /// </summary>
        /// <param name="viewName">ビュー名</param>
        /// <param name="masterName">マスター ページ名</param>
        /// <param name="model">モデル</param>
        /// <returns>ViewResult オブジェクト</returns>
        protected override ViewResult View(string viewName, string masterName, object model)
        {
            ViewResult vr = base.View(viewName, masterName, model);
            string[] temp = vr.ViewName.Split('.');

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + this.UserInfo.UserName +
                "," + this.UserInfo.IPAddress +
                "," + "----->>" +
                "," + this.ControllerName +
                "," + this.ActionName + " -> " + temp[temp.Length - 1];

            LogIF.InfoLog("ACCESS", strLogMessage);

            return vr;
        }

        #endregion

        #endregion

        #region OnResult

        /// <summary>
        /// アクション メソッドによって返されたアクション結果が実行される前に呼び出されます。  
        /// Controller.OnResultExecuting メソッド (System.Web.Mvc)
        /// http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onresultexecuting.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ResultExecutingContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // イベント処理開始前にエラーが発生した場合は、
            // this.perfRecがnullの場合があるので、null対策コードを挿入する。
            if (this.perfRec == null)
            {
                // nullの場合、新しいインスタンスを生成し、性能測定開始。
                this.perfRec = new PerformanceRecorder();
                perfRec.StartsPerformanceRecord();
            }

            // Calling base class method.
            base.OnResultExecuting(filterContext);

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + this.UserInfo.UserName +
                "," + this.UserInfo.IPAddress +
                "," + "----->" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnResultExecuting)";

            LogIF.DebugLog("ACCESS", strLogMessage);
        }

        /// <summary>
        /// アクション メソッドによって返されたアクション結果が実行された後に呼び出されます。 
        /// Controller.OnResultExecuted メソッド (System.Web.Mvc)
        /// http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onresultexecuted.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ResultExecutingContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // Calling base class method.
            base.OnResultExecuted(filterContext);

            // イベント処理開始前にエラーが発生した場合は、
            // this.perfRecがnullの場合があるので、null対策コードを挿入する。
            if (this.perfRec == null)
            {
                // nullの場合、新しいインスタンスを生成し、性能測定開始。
                this.perfRec = new PerformanceRecorder();
                perfRec.StartsPerformanceRecord();
            }

            this.perfRec.EndsPerformanceRecord();

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + this.UserInfo.UserName +
                "," + this.UserInfo.IPAddress +
                "," + "<-----" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnResultExecuted)" +
                "," + perfRec.ExecTime +
                "," + perfRec.CpuTime;

            LogIF.DebugLog("ACCESS", strLogMessage);
        }

        #endregion

        #region OnException

        /// <summary>アクションでハンドルされない例外が発生したときに呼び出されます。</summary>
        /// <param name="exceptionContext">
        /// 型: System.Web.Mvc.ResultExecutingContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        /// <remarks>
        /// web.config に customErrors mode="on" を追記（無い場合は、OnExceptionメソッドが動かない 
        /// </remarks>
        protected override void OnException(ExceptionContext exceptionContext)
        {
            // Calling base class method.
            base.OnException(exceptionContext);            
            // エラーログの出力
            this.OutputErrorLog(exceptionContext);
            // エラー画面に画面遷移する
            this.TransferErrorScreen(exceptionContext);
        }

        /// <summary>エラーログの出力</summary>
        /// <param name="exceptionContext">ExceptionContext</param>
        private void OutputErrorLog(ExceptionContext exceptionContext)
        {
            // 非同期ControllerのInnerException対策（底のExceptionを取得する）。
            Exception ex = exceptionContext.Exception;
            Exception bottomException = ex;
            while (bottomException.InnerException != null)
            {
                bottomException = bottomException.InnerException;
            }

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------

            string strLogMessage =
                "," + (this.UserInfo != null ? this.UserInfo.UserName : "null") +
                "," + (this.UserInfo != null ? this.UserInfo.IPAddress : "null") +
                "," + "<-----" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnException)" +
                "," + //this.perfRec.ExecTime +
                "," + //this.perfRec.CpuTime + 
                "," + GetExceptionMessageID(bottomException) +
                "," + bottomException.Message + "\r\n"+
                "," + bottomException.StackTrace + "\r\n" +
                "," + ex.ToString(); // Exception.ToString()はRootのExceptionに対して行なう。

            LogIF.ErrorLog("ACCESS", strLogMessage);
        }

        /// <summary>例外発生時に、エラー画面に画面遷移</summary>
        /// <param name="exceptionContext">ExceptionContext</param>
        private void TransferErrorScreen(ExceptionContext exceptionContext)
        {
            // 非同期ControllerのInnerException対策（底のExceptionを取得する）。
            Exception ex = exceptionContext.Exception;
            Exception bottomException = ex;
            while (bottomException.InnerException != null)
            {
                bottomException = bottomException.InnerException;
            }

            #region 例外型を判別しエラーメッセージIDを取得

            // エラーメッセージ
            string err_msg;

            // エラー情報をセッションから取得
            string err_info;

            // エラー画面へのパスを取得 --- チェック不要（ベースクラスでチェック済み）
            string errorScreenPath = GetConfigParameter.GetConfigValue(FxLiteral.ERROR_SCREEN_PATH);
            
            // エラーメッセージＩＤ
            string errMsgId = this.GetExceptionMessageID(ex);

            #endregion

            #region エラー時に、セッションを開放しないで、業務を続行可能にする処理を追加。

            // 不正操作エラー or 画面遷移制御チェック エラー
            if (errMsgId == "IllegalOperationCheckError"
                || errMsgId == "ScreenControlCheckError")
            {
                // セッションをクリアしない
                Session[FxHttpContextIndex.SESSION_ABANDON_FLAG] = false;
            }
            else
            {
                // セッションをクリアする
                Session[FxHttpContextIndex.SESSION_ABANDON_FLAG] = true;
            }

            #endregion

            #region エラー画面に表示するエラー情報を作成

            err_msg = Environment.NewLine +
                "Error Message ID : " + errMsgId + Environment.NewLine +
                "Error Message : " + bottomException.Message.ToString();

            err_info = System.Environment.NewLine +
                "Current Request Url : " + Request.Url.ToString() + Environment.NewLine +
                "Exception.StackTrace : " + bottomException.StackTrace + Environment.NewLine +
                "Exception.ToString() : " + ex.ToString(); // Exception.ToString()はRootのExceptionに対して行なう。

            // Add exception information to Session
            Session[FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE] = err_msg;
            Session[FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION] = err_info;

            // Add Form information to Session
            Session[FxHttpContextIndex.FORMS_INFORMATION] = Request.Form;

            #endregion

            #region  エラー画面へ画面遷移

            exceptionContext.ExceptionHandled = true;
            exceptionContext.HttpContext.Response.Clear();

            if (exceptionContext.HttpContext.Request.IsAjaxRequest())
            {
                exceptionContext.Result = new JavaScriptResult() { Script = "location.href = '" + errorScreenPath + "'" };
            }
            else if (exceptionContext.IsChildAction)
            {
                exceptionContext.Result = new ContentResult() { Content = "<script>location.href = '" + errorScreenPath + "'</script>" };
            }
            else
            {
                exceptionContext.Result = new RedirectResult(errorScreenPath);
            }

            #endregion
        }

        /// <summary>ExceptionのMessageIDを返す。</summary>
        /// <param name="ex">Exception</param>
        /// <returns>ExceptionのMessageID</returns>
        private string GetExceptionMessageID(Exception ex)
        {
            // Check exception type 
            if (ex is BusinessSystemException)
            {
                // システム例外
                BusinessSystemException bsEx = (BusinessSystemException)ex;
                return bsEx.messageID;
            }
            else if (ex is FrameworkException)
            {
                // フレームワーク例外
                FrameworkException fxEx = (FrameworkException)ex;
                return fxEx.messageID;
            }
            else
            {
                // それ以外の例外
                return "other Exception";
            }
        }

        #endregion

        #region 情報取得用

        /// <summary>ユーザ情報を取得する</summary>
        private void GetUserInfo()
        {
            // セッションステートレス対応
            if (this.HttpContext.Session == null)
            {
                // SessionがOFFの場合
            }
            else
            {
                // 取得を試みる。
                this.UserInfo = (MyUserInfo)UserInfoHandle.GetUserInformation();

                // nullチェック
                if (this.UserInfo == null)
                {
                    // nullの場合、仮の値を生成 / 設定する。
                    string userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;

                    if (userName == null || userName == "")
                    {
                        // 未認証状態
                        this.UserInfo = new MyUserInfo("未認証", this.HttpContext.Request.UserHostAddress);
                    }
                    else
                    {
                        // 認証状態
                        this.UserInfo = new MyUserInfo(userName, this.HttpContext.Request.UserHostAddress);

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
        private void GetRouteData()
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(this.HttpContext);

            string[] temp = null;
            temp =routeData.Values["controller"].ToString().Split('.');
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

                // Using ASP.NET-MVC:
                this.Response.Cache.SetCacheability(HttpCacheability.NoCache);  // HTTP 1.1.
                this.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
                this.Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                this.Response.AppendHeader("Expires", "0"); // Proxies.

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
