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
//* クラス名        ：MyBaseApiController (FilterAttribute)
//* クラス日本語名  ：ASP.NET WebAPI用 ベーククラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/08/11  西野 大介         新規作成
//**********************************************************************************

using System;

using System.Web.Mvc;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>ASP.NET WebAPI用 ベーククラス２</summary>
    /// <remarks>（FilterAttributeとして）自由に利用できる。</remarks>
    public class MyBaseApiController : ActionFilterAttribute, IExceptionFilter
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
        /// ActionFilterAttribute.OnActionExecuting Method (ActionExecutingContext) (System.Web.Mvc)
        /// https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onactionexecuting.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ActionExecutingContext
        /// 現在の要求およびアクションに関する情報。
        /// </param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // カスタム認証処理 --------------------------------------------
            // Authorization: Bearer ヘッダから
            // JWTアサーションを処理し、認証、UserInfoを生成するなど。
            // ・・・
            System.Diagnostics.Debug.WriteLine(filterContext.HttpContext.Request.Headers.ToString());
            // -------------------------------------------------------------

            // 認証ユーザ情報をメンバにロードする --------------------------
            this.GetUserInfo(filterContext);
            // -------------------------------------------------------------
            
            // 権限チェック ------------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // 閉塞チェック ------------------------------------------------
            // ・・・
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
        /// ActionFilterAttribute.OnActionExecuted Method (ActionExecutedContext) (System.Web.Mvc)
        /// https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onactionexecuted.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ActionExecutedContext
        /// 現在の要求およびアクションに関する情報。
        /// </param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
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

        #endregion

        #region OnResult

        /// <summary>
        /// アクション メソッドによって返されたアクション結果が実行される前に呼び出されます。  
        /// ActionFilterAttribute.OnResultExecuting Method (ResultExecutingContext) (System.Web.Mvc)
        /// https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onresultexecuting.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ResultExecutingContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
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
        /// ActionFilterAttribute.OnResultExecuted Method (ResultExecutedContext) (System.Web.Mvc)
        /// https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onresultexecuted.aspx
        /// </summary>
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ResultExecutingContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
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
        /// <param name="filterContext">
        /// 型: System.Web.Mvc.ResultExecutingContext
        /// 現在の要求およびアクション結果に関する情報。
        /// </param>
        /// <remarks>
        /// web.config に customErrors mode="on" を追記（無い場合は、OnExceptionメソッドが動かない 
        /// </remarks>
        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {       
            // エラーログの出力
            this.OutputErrorLog(filterContext);
        }

        /// <summary>エラーログの出力</summary>
        /// <param name="filterContext">ExceptionContext</param>
        private void OutputErrorLog(ExceptionContext filterContext)
        {
            // 非同期ControllerのInnerException対策（底のExceptionを取得する）。
            Exception ex = filterContext.Exception;
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
                "," + "----->>" +
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
        private void GetUserInfo(ActionExecutingContext filterContext)
        {
            // nullチェック
            if (this.UserInfo == null)
            {
                // nullの場合、仮の値を生成 / 設定する。
                string userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;

                if (userName == null || userName == "")
                {
                    // 未認証状態
                    this.UserInfo = new MyUserInfo("未認証", filterContext.RequestContext.HttpContext.Request.UserHostAddress);
                }
                else
                {
                    // 認証状態
                    this.UserInfo = new MyUserInfo(userName, filterContext.RequestContext.HttpContext.Request.UserHostAddress);
                    // 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                }
            }
            else
            {
                // nullで無い場合、取得した値を設定する。
            }

            // ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
        }
        
        #endregion
    }
}
