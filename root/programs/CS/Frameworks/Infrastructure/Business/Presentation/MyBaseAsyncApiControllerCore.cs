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
//*  2020/02/12  西野 大介         属性ベース機構の強化（EnumHttpAuthHeaderの導入）
//**********************************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net.Http.Headers;

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
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>非同期 ASP.NET WebAPI用 ベースクラス２</summary>
    /// <remarks>（ActionFilterAttributeとして）自由に利用できる。</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MyBaseAsyncApiController : ActionFilterAttribute, IAsyncAuthorizationFilter, IAsyncActionFilter, IExceptionFilter
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

        /// <summary>Controller/Action名を取得する</summary>
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

                if (authorizationContext.HttpContext.Request.Headers != null)
                {
                    StringValues authHeaders = "";

                    try
                    {
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
                        new Claim("IpAddress", MyBaseAsyncApiController.GetClientIpAddress())
                    };

                    MyHttpContext.Current.User.AddIdentity(new ClaimsIdentity(claims, "Token"));
                }
                else
                {
                    // 認証エラーを返す。
                    // ASP.NET Core MVCで403エラーをクライアントへ返す - Living Absurd World
                    // https://blog.hmatoba.net/Article/144
                    authorizationContext.Result = new UnauthorizedResult();
                }

                return;

                #endregion
            }
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
            // MyHttpContext.Current.User.Identity側ではなく、Identities側に入っている。
            // Identityは認証ミドルウェアを使用する必要がある？（coreでjwtをどう処理するのか？）
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
