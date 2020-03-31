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
//* クラス名        ：FxController
//* クラス日本語名  ：ASP.NET WebAPI JSON-RPCの.NETオブジェクトの
//*                   バイナリ転送用メソッドを公開するサービス インターフェイス基盤
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/08/18  西野 大介         新規作成
//*  2019/11/18  西野 大介         .NET Core化
//**********************************************************************************

using System;
//using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Transmission;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Util;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Reflection;

namespace ASPNETWebService.Controllers
{
    /// <summary>
    /// ASP.NET WebAPI JSON-RPCの.NETオブジェクトの
    /// バイナリ転送用メソッドを公開するサービス インターフェイス基盤
    /// </summary>
    [EnableCors]
    //[ApiController]
    public class FxController : ControllerBase
    {
        #region 疎通テスト用

        /// <summary>
        /// 疎通テスト用
        /// http(s)://hostName:portNum/testで疎通テスト可能。
        /// </summary>
        /// <returns>Dictionary(string, string)</returns>
        [HttpGet]
        [Route("test")]
        public Dictionary<string, string> test()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("AAA", "aaa");
            dic.Add("BBB", "bbb");
            dic.Add("CCC", "ccc");

            return dic;
        }

        #endregion

        #region グローバル変数

        /// <summary>インプロセス呼び出しの名前解決シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static InProcessNameService IPR_NS = new InProcessNameService();

        #endregion

        #region ASP.NET WebAPI JSON-RPCの.NETオブジェクトのバイナリ転送用メソッド

        /// <summary>
        /// ASP.NET WebAPI JSON-RPCの.NETオブジェクトのバイナリ転送用メソッド
        /// </summary>
        /// <param name="paramDic">
        /// 引数：Dictionary(string, string)
        /// ・ServiceName
        /// ・ContextObject
        /// ・ParameterValueObject
        /// </param>
        /// <returns>
        /// 戻り値：Dictionary(string, string)
        /// ・Return
        /// ・ContextObject
        /// ・ReturnValueObject
        /// </returns>
        [HttpPost]
        //[Route("WebAPIControllerForFx")] → Startup.cs へ
        public async Task<Dictionary<string, string>> DotNETOnlineWebAPI([FromBody] Dictionary<string, string> paramDic)
        {
            // 引数
            string serviceName = paramDic["ServiceName"];
            byte[] contextObject = CustomEncode.FromBase64String(paramDic["ContextObject"]);
            byte[] parameterValueObject = CustomEncode.FromBase64String(paramDic["ParameterValueObject"]);

            // 戻り値
            byte[] ret = null;
            byte[] returnValueObject = null;
            Dictionary<string, string> returnDic = new Dictionary<string, string>();

            // ステータス
            string status = "－";

            // 初期化のため
            returnValueObject = null;

            #region 呼出し制御関係の変数

            // アセンブリ名
            string assemblyName = "";

            // クラス名
            string className = "";

            #endregion

            #region 引数・戻り値関係の変数

            // コンテキスト情報
            object context; // 2009/09/29-この行

            // 引数・戻り値の.NETオブジェクト
            BaseParameterValue parameterValue = null;
            BaseReturnValue returnValue = null;

            // エラー情報（クライアント側で復元するため）
            WSErrorInfo wsErrorInfo = new WSErrorInfo();

            // エラー情報（ログ出力用）
            string errorType = ""; // 2009/09/15-この行
            string errorMessageID = "";
            string errorMessage = "";
            string errorToString = "";

            #endregion

            try
            {
                // 開始ログの出力
                LogIF.InfoLog("SERVICE-IF", FxLiteral.SIF_STATUS_START);

                #region 名前解決

                // ★
                status = FxLiteral.SIF_STATUS_NAME_SERVICE;

                // 名前解決（インプロセス）
                FxController.IPR_NS.NameResolution(serviceName, out assemblyName, out className);

                #endregion

                #region 引数のデシリアライズ

                // ★
                status = FxLiteral.SIF_STATUS_DESERIALIZE;

                // コンテキストクラスの.NETオブジェクト化
                context = BinarySerialize.BytesToObject(contextObject); // 2009/09/29-この行
                // ※ コンテキストの利用方法は任意だが、サービスインターフェイス上での利用に止める。

                // 引数クラスの.NETオブジェクト化
                parameterValue = (BaseParameterValue)BinarySerialize.BytesToObject(parameterValueObject);

                // 引数クラスをパラメタ セットに格納
                object[] paramSet = new object[] { parameterValue, DbEnum.IsolationLevelEnum.User };

                #endregion

                #region 認証処理のＵＯＣ

                // ★
                status = FxLiteral.SIF_STATUS_AUTHENTICATION;

                if (context is string)
                {
                    // System.Stringの場合
                    string access_token = (string)context;
                    if (!string.IsNullOrEmpty(access_token))
                    {
                        string sub = "";
                        List<string> roles = null;
                        List<string> scopes = null;
                        JObject jobj = null;

                        if (AccessToken.Verify(access_token, out sub, out roles, out scopes, out jobj))
                        {
                            // 認証成功
                            Debug.WriteLine("認証成功");
                        }
                        else
                        {
                            // 認証失敗（認証必須ならエラーにする。
                        }
                    }
                    else
                    {
                        // 認証失敗（認証必須ならエラーにする。
                    }
                }
                else
                {
                    // MyUserInfoの場合
                }

                //contextObject = BinarySerialize.ObjectToBytes(hogehoge); // 更新可能だが...。

                #endregion

                #region Ｂ層・Ｄ層呼出し

                // ★
                status = FxLiteral.SIF_STATUS_INVOKE;

                // #17-start
                try
                {
                    // Ｂ層・Ｄ層呼出し
                    Task<BaseReturnValue> result = (Task<BaseReturnValue>)Latebind.InvokeMethod(
                        assemblyName, className,
                        FxLiteral.TRANSMISSION_INPROCESS_ASYNC_METHOD_NAME, paramSet);
                    returnValue = await result;
                }
                catch (System.Reflection.TargetInvocationException rtEx)
                {
                    //// InnerExceptionを投げなおす。
                    //throw rtEx.InnerException;

                    // スタックトレースを保って InnerException を throw
                    ExceptionDispatchInfo.Capture(rtEx.InnerException).Throw();
                }
                // #17-end

                #endregion

                #region 戻り値のシリアライズ

                // ★
                status = FxLiteral.SIF_STATUS_SERIALIZE;

                returnValueObject = BinarySerialize.ObjectToBytes(returnValue);

                #endregion

                // ★
                status = "";

                // 戻り値を返す。
                ret = BinarySerialize.ObjectToBytes("");
            }
            catch (BusinessSystemException bsEx)
            {
                // システム例外

                // エラー情報を設定する。
                wsErrorInfo.ErrorType = FxEnum.ErrorType.BusinessSystemException;
                wsErrorInfo.ErrorMessageID = bsEx.messageID;
                wsErrorInfo.ErrorMessage = bsEx.Message;

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.BusinessSystemException.ToString(); // 2009/09/15-この行
                errorMessageID = bsEx.messageID;
                errorMessage = bsEx.Message;

                errorToString = bsEx.ToString();

                // エラー情報を戻す。
                ret = BinarySerialize.ObjectToBytes(wsErrorInfo);
            }
            catch (FrameworkException fxEx)
            {
                // フレームワーク例外
                // ★ インナーエクセプション情報は消失

                // エラー情報を設定する。
                wsErrorInfo.ErrorType = FxEnum.ErrorType.FrameworkException;
                wsErrorInfo.ErrorMessageID = fxEx.messageID;
                wsErrorInfo.ErrorMessage = fxEx.Message;

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.FrameworkException.ToString(); // 2009/09/15-この行
                errorMessageID = fxEx.messageID;
                errorMessage = fxEx.Message;

                errorToString = fxEx.ToString();

                // エラー情報を戻す。
                ret = BinarySerialize.ObjectToBytes(wsErrorInfo);
            }
            catch (Exception ex)
            {
                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.ElseException.ToString(); // 2009/09/15-この行
                errorMessageID = "－";
                errorMessage = ex.Message;

                errorToString = ex.ToString();

                throw; // SoapExceptionになって伝播
            }
            finally
            {
                //// Sessionステートレス
                //Session.Clear();
                //Session.Abandon();

                // 終了ロクの出力
                if (status == "")
                {
                    // 終了ログ出力
                    LogIF.InfoLog("SERVICE-IF", "正常終了");
                }
                else
                {
                    // 終了ログ出力
                    LogIF.ErrorLog("SERVICE-IF",
                        "異常終了"
                        + "：" + status + "\r\n"
                        + "エラー タイプ：" + errorType + "\r\n" // 2009/09/15-この行
                        + "エラー メッセージID：" + errorMessageID + "\r\n"
                        + "エラー メッセージ：" + errorMessage + "\r\n"
                        + errorToString);
                }
            }

            returnDic.Add("Return", CustomEncode.ToBase64String(ret));
            returnDic.Add("ContextObject", CustomEncode.ToBase64String(contextObject));
            returnDic.Add("ReturnValueObject", CustomEncode.ToBase64String(returnValueObject));
            
            return returnDic;
        }

        #endregion
    }
}
