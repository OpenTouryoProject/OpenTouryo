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
//* クラス名        ：MyMVCCoreFilterAttribute
//* クラス日本語名  ：ASP.NET MVC Core用 画面コード親クラス２相当（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/08  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.StdMigration;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>ASP.NET MVC Core用 画面コード親クラス２相当（テンプレート）</summary>
    /// <remarks>（ActionFilterAttributeとして）自由に利用できる。</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MyMVCCoreFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        /// <summary>UserInfo</summary>
        protected MyUserInfo UserInfo;

        /// <summary>ControllerName</summary>
        protected string ControllerName = "";

        /// <summary>ActionName</summary>
        protected string ActionName = "";

        #region OnException

        /// <summary>
        /// 非同期の例外フィルターを実行します。
        /// https://msdn.microsoft.com/ja-jp/library/hh835353.aspx
        /// </summary>
        /// <param name="exceptionContext">HttpActionExecutedContext</param>
        public void OnException(ExceptionContext exceptionContext)
        {
            //// Calling base class method.
            //base.OnException(exceptionContext);
            // エラーログの出力
            this.OutputErrorLog(exceptionContext);
            // エラー画面に画面遷移する
            this.TransferErrorScreen(exceptionContext);
        }

        /// <summary>エラーログの出力</summary>
        /// <param name="exceptionContext">ExceptionContext</param>
        private void OutputErrorLog(ExceptionContext exceptionContext)
        {
            this.GetRouteData(exceptionContext.RouteData);

            // 内部で await するが、呼出し元は同期なので、結果として同期実行になる。
            this.GetUserInfoAsync();

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
                "," + bottomException.Message + "\r\n" +
                "," + bottomException.StackTrace + "\r\n" +
                "," + ex.ToString(); // Exception.ToString()はRootのExceptionに対して行なう。

            LogIF.ErrorLog("ACCESS", strLogMessage);
        }

        /// <summary>例外発生時に、エラー画面に画面遷移</summary>
        /// <param name="exceptionContext">ExceptionContext</param>
        private void TransferErrorScreen(ExceptionContext exceptionContext)
        {
            exceptionContext.ExceptionHandled = true;

            //  Development、Staging、Production
            string tmp = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(tmp))
            {
                // Production
            }
            else if(tmp.ToUpper() != "Development".ToUpper())
            {
                // Staging or Production
            }
            else
            {
                // Development

                // 開発時は、app.UseDeveloperExceptionPage(); を使用するため。
                exceptionContext.ExceptionHandled = false;
                return; // 何もしないで戻る。
            }

            // 非同期ControllerのInnerException対策（底のExceptionを取得する）。
            Exception ex = exceptionContext.Exception;
            Exception bottomException = ex;
            while (bottomException.InnerException != null)
            {
                bottomException = bottomException.InnerException;
            }

            #region 例外型を判別しエラーメッセージIDを取得

            // エラー画面へのパスを取得 --- チェック不要（ベースクラスでチェック済み）
            string errorScreenPath = GetConfigParameter.GetConfigValue(FxLiteral.ERROR_SCREEN_PATH);

            // エラーメッセージＩＤ
            string errMsgId = this.GetExceptionMessageID(ex);

            #endregion

            #region エラー時に、セッションを開放しないで、業務を続行可能にする処理を追加。

            ISession session = MyHttpContext.Current.Session;

            // 不正操作エラー or 画面遷移制御チェック エラー
            if (errMsgId == "IllegalOperationCheckError"
                || errMsgId == "ScreenControlCheckError")
            {
                // セッションをクリアしない
                session.SetInt32(FxHttpContextIndex.SESSION_ABANDON_FLAG, Convert.ToInt32(false));
            }
            else
            {
                // セッションをクリアする
                session.SetInt32(FxHttpContextIndex.SESSION_ABANDON_FLAG, Convert.ToInt32(true));
            }

            #endregion

            // エラー画面へ遷移
            exceptionContext.HttpContext.Response.Redirect(errorScreenPath);
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

        #endregion
    }
}
