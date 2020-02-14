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
//* クラス名        ：MyBaseAsyncApiController (Filters)
//* クラス日本語名  ：非同期 ASP.NET WebAPI用 ベースクラス２相当（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/08/11  西野 大介         新規作成
//*  2018/12/12  西野 大介         インターフェイスの拡張
//*  2020/02/12  西野 大介         属性ベース機構の強化（EnumHttpAuthHeaderの導入）
//**********************************************************************************

#pragma warning disable 1998

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;

using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;

using Microsoft.Owin;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>非同期 ASP.NET WebAPI用 ベースクラス２</summary>
    /// <remarks>（ActionFilterAttributeとして）自由に利用できる。</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MyBaseAsyncApiController : ActionFilterAttribute, IAuthenticationFilter, IActionFilter, IExceptionFilter
    {
        #region インスタンス変数

        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        ///// <summary>UserInfo</summary>
        //protected MyUserInfo UserInfo;

        /// <summary>ControllerName</summary>
        protected string ControllerName = "";

        /// <summary>ActionName</summary>
        protected string ActionName = "";

        /// <summary>HttpAuthHeader</summary>
        protected EnumHttpAuthHeader HttpAuthHeader = EnumHttpAuthHeader.None;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <remarks>自由に利用できる。</remarks>
        public MyBaseAsyncApiController() : base()
        {
            this.HttpAuthHeader = EnumHttpAuthHeader.None;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="httpAuthHeader">EnumHttpAuthHeader</param>
        /// <remarks>自由に利用できる。</remarks>
        public MyBaseAsyncApiController(EnumHttpAuthHeader httpAuthHeader) : base()
        {
            this.HttpAuthHeader = httpAuthHeader;
        }

        #endregion

        #region 認証・認可

        /// <summary>
        /// プロセスが承認を要求したときに呼び出します。
        /// https://msdn.microsoft.com/ja-jp/library/dn314618.aspx
        /// https://msdn.microsoft.com/ja-jp/magazine/dn781361.aspx
        /// </summary>
        /// <param name="authenticationContext">HttpAuthenticationContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public async Task AuthenticateAsync(HttpAuthenticationContext authenticationContext, CancellationToken cancellationToken)
        {
            // 認証ユーザ情報をメンバにロードする
            await this.GetUserInfoAsync(authenticationContext);
        }

        /// <summary>
        /// ・・・ChallengeAsync・・・
        /// https://msdn.microsoft.com/ja-jp/library/dn573281.aspx
        /// https://msdn.microsoft.com/ja-jp/magazine/dn781361.aspx
        /// </summary>
        /// <param name="authenticationChallengeContext">HttpAuthenticationChallengeContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext authenticationChallengeContext, CancellationToken cancellationToken)
        {
            authenticationChallengeContext.Result = new ResultWithChallenge(authenticationChallengeContext.Result);
            return;
        }
        
        #endregion

        #region OnAction

        /// <summary>
        /// アクション メソッドの呼び出し前に発生します。
        /// https://msdn.microsoft.com/ja-jp/library/dn573278.aspx
        /// </summary>
        /// <param name="actionContext">HttpActionContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
		public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // Controller・Action名を取得する。
            this.GetControllerAndActionName(actionContext);

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
                "," + userName + //this.UserInfo.UserName +
                "," + ipAddress + //this.UserInfo.IPAddress +
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

            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            string strLogMessage =
                "," + userName + //this.UserInfo.UserName +
                "," + ipAddress + //this.UserInfo.IPAddress +
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

        /// <summary>Controller/Action名を取得する</summary>
        /// <param name="actionContext">HttpActionContext</param>
        private void GetControllerAndActionName(HttpActionContext actionContext)
        {
            // MSBuild で ビルド不可。
            //this.ControllerName = actionContext?.ControllerContext?.ControllerDescriptor?.ControllerName;
            //this.ActionName = actionContext?.ActionDescriptor?.ActionName;

            if (actionContext != null)
            {
                HttpControllerContext controllerContext = actionContext.ControllerContext;
                if (controllerContext != null)
                {
                    HttpControllerDescriptor controllerDescriptor = controllerContext.ControllerDescriptor;
                    if (controllerDescriptor != null)
                    {
                        this.ControllerName = controllerDescriptor.ControllerName;
                    }
                }

                HttpActionDescriptor actionDescriptor = actionContext.ActionDescriptor;
                if (actionDescriptor != null)
                {
                    this.ActionName = actionDescriptor.ActionName;
                }
            }
        }

        /// <summary>ユーザ情報を取得する</summary>
        /// <param name="authenticationContext">HttpAuthenticationContext</param>
        private async Task GetUserInfoAsync(HttpAuthenticationContext authenticationContext)
        {
            // Authorization: <type> <credentials>
            if (this.HttpAuthHeader == EnumHttpAuthHeader.None)
            {
                // 認証なし
                return;
            }
            else if (this.HttpAuthHeader.HasFlag(EnumHttpAuthHeader.Basic))
            {
                // Basic認証の認証アルゴリズムを追加
                // Authorization: Basic XXXXXXXXXX
                return;
            }
            else if (this.HttpAuthHeader.HasFlag(EnumHttpAuthHeader.Bearer))
            {
                // Bearer認証の認証アルゴリズムを追加 --------------------------
                // Authorization: Bearer XXXXXXXXXX
                // JWTアサーションを処理し、認証、UserInfoを生成するなど。
                // -------------------------------------------------------------
                List<Claim> claims = null;

                if (authenticationContext.Request.Headers.Authorization != null)
                {
                    try
                    {
                        if (authenticationContext.Request.Headers.Authorization.Scheme.ToLower() == "bearer")
                        {

                            string access_token = authenticationContext.Request.Headers.Authorization.Parameter;

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
                                    new Claim("IpAddress", MyBaseAsyncApiController.GetClientIpAddress(authenticationContext.Request))
                                };

                                // The request message contains valid credential.
                                authenticationContext.Principal = new ClaimsPrincipal(
                                    new List<ClaimsIdentity> { new ClaimsIdentity(claims, "Token") });

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
                    catch
                    {
                        // 例外発生 ≒ 未認証扱い。
                    }
                }
                else
                {
                    // Authorization Headerが存在しない。
                }

                #region 未認証状態の場合の扱い

                if (this.HttpAuthHeader.HasFlag(EnumHttpAuthHeader.None))
                {
                    // 未認証状態のclaimsを作成格納
                    claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "未認証"),
                        new Claim(ClaimTypes.Role, ""),
                        new Claim(OAuth2AndOIDCConst.UrnScopesClaim, ""),
                        new Claim(OAuth2AndOIDCConst.UrnAudienceClaim, ""),
                        new Claim("IpAddress", MyBaseAsyncApiController.GetClientIpAddress(authenticationContext.Request))
                    };

                    authenticationContext.Principal = new ClaimsPrincipal(new List<ClaimsIdentity> { new ClaimsIdentity(claims, "Token") });
                }
                else
                {

                    // 認証エラーを返す。
                    // ASP.NET Web API 2 | Microsoft Docs
                    // https://docs.microsoft.com/ja-jp/aspnet/web-api/overview/security/authentication-filters
                    authenticationContext.ErrorResult = new AuthenticationFailureResult(
                    authenticationContext.Request, "Bearer Credentials is invalid.");
                }

                return;

                #endregion
            }
        }

        /// <summary>GetClientIpAddress</summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>IPAddress(文字列)</returns>
        public static string GetClientIpAddress(HttpRequestMessage request)
        {
            IEnumerable<string> headerValues = null;
            string clientIp = "";

            if (request.Headers.TryGetValues("X-Forwarded-For", out headerValues) == true)
            {
                string xForwardedFor = headerValues.FirstOrDefault();
                clientIp = xForwardedFor.Split(',').GetValue(0).ToString().Trim();
            }
            else
            {
                if (request.Properties.ContainsKey("MS_HttpContext"))
                {
                    clientIp = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                }
                else if (request.Properties.ContainsKey("MS_OwinContext"))
                {
                    OwinContext owinContext = request.Properties["MS_OwinContext"] as OwinContext;
                    if (owinContext != null)
                    {
                        clientIp = owinContext.Request.RemoteIpAddress;
                    }
                }
            }

            if (clientIp == "::1"/*localhost*/)
            {
                clientIp = "localhost";
            }
            else
            {
                clientIp = clientIp.Split(':').GetValue(0).ToString().Trim(); 
            }

            return clientIp;
        }

        /// <summary>GetClaimsIdentity</summary>
        /// <returns>ClaimsIdentity</returns>
        public static ClaimsIdentity GetClaimsIdentity()
        {
            return ((ClaimsIdentity)HttpContext.Current.User.Identity);
        }

        /// <summary>GetRawClaims</summary>
        /// <returns>IEnumerable(Claim)</returns>
        public static IEnumerable<Claim> GetRawClaims()
        {
            ClaimsIdentity claimsIdentity = MyBaseAsyncApiController.GetClaimsIdentity();

            if (claimsIdentity == null)
            {
                return null;
            }
            else
            {
                return claimsIdentity.Claims;
            }
        }

        /// <summary>GetClaims</summary>
        /// <param name="userName">string</param>
        /// <param name="roles">string</param>
        /// <param name="scopes">string</param>
        /// <param name="ipAddress">string</param>
        public static void GetClaims(out string userName, out string roles, out string scopes, out string ipAddress)
        {
            IEnumerable<Claim> claims = MyBaseAsyncApiController.GetRawClaims();

            if (claims == null)
            {
                // claims == null
                userName = "未認証";
                roles = "";
                scopes = "";
                ipAddress = "";
            }
            else
            {
                // claims != null
                Claim claim = null;

                claim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (claim == null)
                {
                    userName = "未認証";
                }
                else
                {
                    userName = claim.Value;
                }

                claim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (claim == null)
                {
                    roles = "";
                }
                else
                {
                    roles = claim.Value;
                }

                claim = claims.FirstOrDefault(c => c.Type == OAuth2AndOIDCConst.UrnScopesClaim);
                if (claim == null)
                {
                    scopes = "";
                }
                else
                {
                    scopes = claim.Value;
                }

                claim = claims.FirstOrDefault(c => c.Type == "IpAddress");
                if (claim == null)
                {
                    ipAddress = "";
                }
                else
                {
                    ipAddress = claim.Value;
                }
            }
        }

        #endregion
    }
}
