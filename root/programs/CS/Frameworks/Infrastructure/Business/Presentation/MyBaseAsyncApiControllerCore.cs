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
//* クラス名        ：MyBaseAsyncApiControllerCore (Filters)
//* クラス日本語名  ：非同期 ASP.NET Core WebAPI用 ベースクラス２相当（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/04/09  西野 大介         新規作成
//*  2018/12/12  西野 大介         インターフェイスの拡張
//**********************************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.StdMigration;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>非同期 ASP.NET WebAPI用 ベースクラス２</summary>
    /// <remarks>（ActionFilterAttributeとして）自由に利用できる。</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MyBaseAsyncApiController : ActionFilterAttribute, IAsyncAuthorizationFilter, IAsyncActionFilter, IExceptionFilter
    {
        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        ///// <summary>UserInfo</summary>
        //protected MyUserInfo UserInfo;

        /// <summary>ControllerName</summary>
        protected string ControllerName = "";

        /// <summary>ActionName</summary>
        protected string ActionName = "";

        #region 認証・認可

        /// <summary>AuthorizationFilterContext</summary>
        /// <param name="authenticationContext">AuthorizationFilterContext</param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext authenticationContext)
        {
            // 認証ユーザ情報をメンバにロードする
            await this.GetUserInfoAsync(authenticationContext);
        }

        #endregion

        #region OnAction

        /// <summary>
        /// アクション メソッドの呼び出し前に発生します(netcore)。
        /// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.aspnetcore.mvc.filters.actionfilterattribute.onactionexecutionasync
        /// </summary>
        /// <param name="context">HttpActionContext</param>
        /// <param name="next">ActionExecutionDelegate</param>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            #region OnActionExecutingAsyncから移行

            // Controller・Action名を取得する。
            this.GetControllerAndActionName(context);

            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 権限チェック ------------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // 閉塞チェック ------------------------------------------------
            // ・・・
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

            string strLogMessage =
                "," + userName + //this.UserInfo.UserName +
                "," + ipAddress + //this.UserInfo.IPAddress +
                "," + "----->" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnActionExecuting)";

            LogIF.InfoLog("ACCESS", strLogMessage);

            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            #endregion

            await base.OnActionExecutionAsync(context, next);

            #region OnActionExecutedAsyncから移行

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // -----------

            strLogMessage =
                "," + userName + //this.UserInfo.UserName +
                "," + ipAddress + //this.UserInfo.IPAddress +
                "," + "<-----" +
                "," + this.ControllerName +
                "," + this.ActionName + "(OnActionExecuted)" +
                "," + perfRec.ExecTime +
                "," + perfRec.CpuTime;

            LogIF.InfoLog("ACCESS", strLogMessage);
            
            #endregion
        }

        #endregion

        #region OnException

        /// <summary>
        /// 非同期の例外フィルターを実行します。
        /// https://msdn.microsoft.com/ja-jp/library/hh835353.aspx
        /// </summary>
        /// <param name="exceptionContext">HttpActionExecutedContext</param>
        public void OnException(ExceptionContext exceptionContext)
        {
            // エラーログの出力
            this.OutputErrorLog(exceptionContext);
        }

        /// <summary>エラーログの出力</summary>
        /// <param name="exceptionContext">HttpActionExecutedContext</param>
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

            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            string strLogMessage =
                "," + userName + // (this.UserInfo != null ? this.UserInfo.UserName : "null") +
                "," + ipAddress + //(this.UserInfo != null ? this.UserInfo.IPAddress : "null") +
                "," + "<-----" +
                "," + this.ControllerName +
                "," + this.ActionName + "(ExecuteExceptionFilterAsync)" +
                "," + //this.perfRec.ExecTime +
                "," + //this.perfRec.CpuTime + 
                "," + GetExceptionMessageID(bottomException) +
                "," + bottomException.Message + "\r\n" +
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

        /// <summary>GetControllerAndActionName</summary>
        /// <param name="context">ActionExecutingContext</param>
        private void GetControllerAndActionName(ActionExecutingContext context)
        {
            // MSBuild で ビルド不可。
            //this.ControllerName = actionContext?.ControllerContext?.ControllerDescriptor?.ControllerName;
            //this.ActionName = actionContext?.ActionDescriptor?.ActionName;
            if (context != null)
            {
                ControllerBase controller = (ControllerBase)context.Controller;
                if (controller != null)
                {
                    ControllerContext controllerContext = controller.ControllerContext;
                    if (controllerContext != null)
                    {
                        ControllerActionDescriptor controllerActionDescriptor = controllerContext.ActionDescriptor;
                        if (controllerActionDescriptor != null)
                        {
                            this.ControllerName = controllerActionDescriptor.ControllerName;
                        }
                    }
                }

                ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                if (actionDescriptor != null)
                {
                    this.ActionName = actionDescriptor.ActionName;
                }
            }
        }

        /// <summary>ユーザ情報を取得する</summary>
        /// <param name="authorizationContext">AuthorizationFilterContext</param>
        /// <remarks>awaitするメソッドを追加して呼ぶ可能性も高いのでasyncを付与</remarks>
        private async Task GetUserInfoAsync(AuthorizationFilterContext authorizationContext)
        {
            // カスタム認証処理 --------------------------------------------
            // Authorization: Bearer ヘッダから
            // JWTアサーションを処理し、認証、UserInfoを生成するなど。
            // -------------------------------------------------------------
            List<Claim> claims = null;

            if (authorizationContext.HttpContext.Request.Headers != null)
            {
                StringValues authHeaders = "";

                if (authorizationContext.HttpContext.Request.Headers.TryGetValue("Authorization", out authHeaders))
                {
                    string access_token = authHeaders[0].Split(' ')[1];

                    string sub = "";
                    List<string> roles = null;
                    List<string> scopes = null;
                    JObject jobj = null;

                    if (AccessToken.Verify(access_token, out sub, out roles, out scopes, out jobj))
                    {
                        // ActionFilterAttributeとApiController間の情報共有はcontext.Principalを使用する。
                        // ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
                        claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, sub),
                            new Claim(ClaimTypes.Role, string.Join(",", roles)),
                            new Claim(OAuth2AndOIDCConst.UrnScopesClaim, string.Join(",", scopes)),
                            new Claim(OAuth2AndOIDCConst.UrnAudienceClaim, (string)jobj[OAuth2AndOIDCConst.aud]),
                            new Claim("IpAddress", MyBaseAsyncApiController.GetClientIpAddress())
                        };

                        // ClaimsPrincipalを設定
                        MyHttpContext.Current.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Token"));

                        return;
                    }
                    else
                    {
                        // JWTの内容検証に失敗
                    }
                }
                else
                {
                    // Authorization HeaderがBearerでない。
                }
            }
            else
            {
                // Authorization Headerが存在しない。
            }

            #region 未認証状態の場合の扱い

            // The request message contains invalid credential
            //context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);

            // 未認証状態のclaimsを作成格納
            claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "未認証"),
                new Claim(ClaimTypes.Role, ""),
                new Claim(OAuth2AndOIDCConst.UrnScopesClaim, ""),
                new Claim(OAuth2AndOIDCConst.UrnAudienceClaim, ""),
                new Claim("IpAddress", MyBaseAsyncApiController.GetClientIpAddress())
            };

            // The request message contains valid credential.
            MyHttpContext.Current.User.AddIdentity(new ClaimsIdentity(claims, "Token"));

            return;

            #endregion
        }

        /// <summary>GetClientIpAddress</summary>
        /// <returns>IPAddress(文字列)</returns>
        private static string GetClientIpAddress()
        {
            return (new GetClientIpAddress()).GetAddress();
        }

        /// <summary>GetClaimsIdentity</summary>
        /// <returns>ClaimsIdentity</returns>
        public static ClaimsIdentity GetClaimsIdentity()
        {
            return MyHttpContext.Current.User.Identities.FirstOrDefault(c => c.AuthenticationType == "Token");
        }

        /// <summary>GetRawClaims</summary>
        /// <returns>IEnumerable(Claim)</returns>
        public static IEnumerable<Claim> GetRawClaims()
        {
            return MyBaseAsyncApiController.GetClaimsIdentity().Claims;
        }

        /// <summary>GetClaims</summary>
        /// <param name="userName">string</param>
        /// <param name="roles">string</param>
        /// <param name="scopes">string</param>
        /// <param name="ipAddress">string</param>
        public static void GetClaims(out string userName, out string roles, out string scopes, out string ipAddress)
        {
            // MyHttpContext.Current.User.Identity側ではなく、Identities側に入っている。
            // Identityは認証ミドルウェアを使用する必要がある？（coreでjwtをどう処理するのか？）
            IEnumerable<Claim>  claims = MyBaseAsyncApiController.GetRawClaims();
            userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            scopes = claims.FirstOrDefault(c => c.Type == OAuth2AndOIDCConst.UrnScopesClaim).Value;
            ipAddress = claims.FirstOrDefault(c => c.Type == "IpAddress").Value;
        }

        #endregion
    }
}
