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
//* クラス名        ：MyBaseApiControllerAsync (ActionFilterAttribute)
//* クラス日本語名  ：非同期 ASP.NET WebAPI用 ベーククラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/08/11  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Threading;
using System.Threading.Tasks;

using System.Web;
//using System.Web.Mvc;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using System.Net;
using System.Net.Http;

using Microsoft.Owin;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>非同期 ASP.NET WebAPI用 ベーククラス２</summary>
    /// <remarks>（ActionFilterAttributeとして）自由に利用できる。</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MyBaseApiControllerAsync : ActionFilterAttribute, IAuthenticationFilter, IActionFilter, IExceptionFilter
    {
        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        /// <summary>UserInfo</summary>
        protected MyUserInfo UserInfo;

        /// <summary>ControllerName</summary>
        protected string ControllerName = "";

        /// <summary>ActionName</summary>
        protected string ActionName = "";

        /// <summary>
        /// プロセスが承認を要求したときに呼び出します。
        /// https://msdn.microsoft.com/ja-jp/library/dn314618.aspx
        /// </summary>
        /// <param name="authenticationContext">HttpAuthenticationContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public async Task AuthenticateAsync(HttpAuthenticationContext authenticationContext, CancellationToken cancellationToken)
        {
            // 認証ユーザ情報をメンバにロードする --------------------------
            await this.GetUserInfo(authenticationContext);
            // -------------------------------------------------------------
        }

        /// <summary>
        /// ・・・
        /// https://msdn.microsoft.com/ja-jp/library/dn573281.aspx
        /// </summary>
        /// <param name="authenticationChallengeContext">HttpAuthenticationChallengeContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext authenticationChallengeContext, CancellationToken cancellationToken)
        {
        }

        #region OnAction

        /// <summary>
        /// アクション メソッドの呼び出し前に発生します。
        /// https://msdn.microsoft.com/ja-jp/library/dn573278.aspx
        /// </summary>
        /// <param name="actionContext">HttpActionContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
		public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
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
            await base.OnActionExecutingAsync(actionContext, cancellationToken);

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
        /// アクション メソッドの呼び出し後に発生します。
        /// https://msdn.microsoft.com/ja-jp/library/dn573277.aspx
        /// </summary>
        /// <param name="actionExecutedContext">HttpActionExecutedContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
		public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            // Calling base class method.
            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

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

        #region OnException

        /// <summary>
        /// 非同期の例外フィルターを実行します。
        /// https://msdn.microsoft.com/ja-jp/library/hh835353.aspx
        /// </summary>
        /// <param name="exceptionContext">HttpActionExecutedContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public async Task ExecuteExceptionFilterAsync(HttpActionExecutedContext exceptionContext, CancellationToken cancellationToken)
        {
            // エラーログの出力
            await this.OutputErrorLog(exceptionContext);
        }

        /// <summary>エラーログの出力</summary>
        /// <param name="exceptionContext">HttpActionExecutedContext</param>
        private async Task OutputErrorLog(HttpActionExecutedContext exceptionContext)
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
        /// <param name="authenticationContext">HttpAuthenticationContext</param>
        private async Task GetUserInfo(HttpAuthenticationContext authenticationContext)
        {
            // カスタム認証処理 --------------------------------------------
            // Authorization: Bearer ヘッダから
            // JWTアサーションを処理し、認証、UserInfoを生成するなど。
            // ・・・
            System.Diagnostics.Debug.WriteLine(authenticationContext.Request.Headers.ToString());
            // -------------------------------------------------------------
            
                if (userName == null || userName == "")
                {
                    // 未認証状態
                    this.UserInfo = new MyUserInfo("未認証", this.GetClientIpAddress(authenticationContext.Request));
                }
                else
                {
                    // 認証状態
                    this.UserInfo = new MyUserInfo(userName, this.GetClientIpAddress(authenticationContext.Request));
                    // 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                }
           

            // ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
        }

        /// <summary>GetClientIpAddress</summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>IPAddress(文字列)</returns>
        private string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
            }
            if (request.Properties.ContainsKey("MS_OwinContext"))
            {
                return IPAddress.Parse(((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress).ToString();
            }
            return String.Empty;
        }

        #endregion
    }
}
