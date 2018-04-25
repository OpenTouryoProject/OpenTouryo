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
//* クラス名        ：ServiceForSb
//* クラス日本語名  ：Soap & Bean 個別Soap Webメソッドを公開するサービス インターフェイス基盤
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/12/02  西野 大介         新規作成
//*  2011/12/08  西野 大介         その他、一般的な例外も文字列化して戻す。
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.EnterpriseServices;
using System.Diagnostics;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

using Touryo.Infrastructure.Framework.Transmission;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

using WSServer_sample.Common;

namespace Touryo.Infrastructure.Framework.ServiceInterface.ASPNETWebService
{

    // 名前空間は、必要に応じて書き換え下さい。

    /// <summary>
    /// Soap & Bean 個別Soap Webメソッドを公開するサービス インターフェイス基盤
    /// </summary>
    [WebService(Namespace = FxLiteral.WS_NAME_SPACE)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ServiceForSb : WebService
    {
        #region グローバル変数

        /// <summary>インプロセス呼び出しの名前解決シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static InProcessNameService IPR_NS = new InProcessNameService();

        #endregion

        #region コンストラクタ

        /// <summary>constructor</summary>
        public ServiceForSb()
        {
            //デザインされたコンポーネントを使用する場合、次の行をコメントを解除してください 
            //InitializeComponent(); 
        }

        #endregion

        #region Soap & Bean Webメソッド

        #region 個別部

        /// <summary>SelectCount</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "SelectCount", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string SelectCount(ref string context, string actionType, out int returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "SelectCount", actionType, null, out temp);
            if (!int.TryParse((string)temp, out returnValue))
            {
                returnValue = -1;
            }

            return ret;
        }

        /// <summary>SelectAll_DT</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "SelectAll_DT", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string SelectAll_DT(ref string context, string actionType, out Shipper[] returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "SelectAll_DT", actionType, null, out temp);
            returnValue = (Shipper[])temp;

            return ret;
        }

        /// <summary>SelectAll_DS</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "SelectAll_DS", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string SelectAll_DS(ref string context, string actionType, out Shipper[] returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "SelectAll_DS", actionType, null, out temp);
            returnValue = (Shipper[])temp;

            return ret;
        }

        /// <summary>SelectAll_DR</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "SelectAll_DR", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string SelectAll_DR(ref string context, string actionType, out Shipper[] returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "SelectAll_DR", actionType, null, out temp);
            returnValue = (Shipper[])temp;

            return ret;
        }

        /// <summary>SelectAll_DSQL</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="orderColumn">orderColumn</param>
        /// <param name="orderSequence">orderSequence</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "SelectAll_DSQL", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string SelectAll_DSQL(ref string context, string actionType, string orderColumn, string orderSequence, out Shipper[] returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "SelectAll_DSQL", actionType,
                new string[] { orderColumn, orderSequence }, out temp);
            returnValue = (Shipper[])temp;

            return ret;
        }

        /// <summary>Select</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="shipperID">shipperID</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "Select", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string Select(ref string context, string actionType, int shipperID, out Shipper returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "Select", actionType, shipperID, out temp);
            returnValue = (Shipper)temp;

            return ret;
        }

        /// <summary>Insert</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="companyName">companyName</param>
        /// <param name="phone">phone</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "Insert", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string Insert(ref string context, string actionType, string companyName, string phone, out int returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "Insert", actionType,
                new string[] { companyName, phone }, out temp);
            if (!int.TryParse((string)temp, out returnValue))
            {
                returnValue = -1;
            }

            return ret;
        }

        /// <summary>Update</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="shipper">shipper</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "Update", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string Update(ref string context, string actionType, Shipper shipper, out int returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "Update", actionType, shipper, out temp);
            if (!int.TryParse((string)temp, out returnValue))
            {
                returnValue = -1;
            }

            return ret;
        }

        /// <summary>Delete</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="shipperID">shipperID</param>
        /// <param name="returnValue">戻り値</param>
        /// <returns>返すべきエラーの情報</returns>
        [WebMethod(BufferResponse = true, CacheDuration = 0, Description = "Delete", EnableSession = true, MessageName = "", TransactionOption = TransactionOption.NotSupported)]
        public string Delete(ref string context, string actionType, int shipperID, out int returnValue)
        {
            object temp = null;
            string ret = Call(ref context, "sbWebService", "Delete", actionType, shipperID, out temp);
            if (!int.TryParse((string)temp, out returnValue))
            {
                returnValue = -1;
            }

            return ret;
        }

        #endregion

        #region 共通部

        /// <summary>Soap & Bean 個別Soap Webメソッドの共通部</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="parameterValue">引数Bean（個別・・・サブ）</param>
        /// <param name="returnValue">戻り値Bean（個別・・・サブ）</param>
        /// <returns>返すべきエラーの情報</returns>
        private string Call(ref string context, string serviceName, string methodName, string actionType, object parameterValue, out object returnValue)
        {
            // ステータス
            string status = "－";

            // 初期化のため
            returnValue = "";

            #region 呼出し制御関係の変数

            // アセンブリ名
            string assemblyName = "";

            // クラス名
            string className = "";

            #endregion

            #region 引数・戻り値関係の変数

            // 引数・戻り値の.NETオブジェクト
            MuParameterValue muParameterValue = null;
            MuReturnValue muReturnValue = null;

            // エラー情報（XMLフォーマット）
            XmlDocument wsErrorInfo = new XmlDocument();
            XmlElement wsErrorRoot = null;
            XmlElement wsErrorItem = null;

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
                ServiceForSb.IPR_NS.NameResolution(serviceName, out assemblyName, out className);

                #endregion

                #region 引数の.NETオブジェクト化（ＵＯＣ）

                // ★
                status = FxLiteral.SIF_STATUS_DESERIALIZE;

                // ★★　引数の.NETオブジェクト化をＵＯＣする（必要に応じて）。

                // 引数文字列の.NETオブジェクト化

                // string[] cmnParameterValueを使用して初期化するなど
                muParameterValue = new MuParameterValue(
                    "", //cmnParameterValue[0], // 画面名
                    "", //cmnParameterValue[1], // ボタン名
                    methodName, //cmnParameterValue[2], // メソッド名
                    actionType, //cmnParameterValue[3], // アクションタイプ
                    new MyUserInfo(context, HttpContext.Current.Request.UserHostAddress));

                // parameterValueを引数の文字列フィールドに設定
                muParameterValue.Bean = parameterValue;

                // 引数クラスをパラメタ セットに格納
                object[] paramSet = new object[] { muParameterValue, DbEnum.IsolationLevelEnum.User };

                #endregion

                #region 認証処理（ＵＯＣ）

                // ★
                status = FxLiteral.SIF_STATUS_AUTHENTICATION;

                string access_token = (string)context;
                if (!string.IsNullOrEmpty(access_token))
                {
                    string sub = "";
                    List<string> roles = null;
                    List<string> scopes = null;
                    JObject jobj = null;

                    if (JwtToken.Verify(access_token, out sub, out roles, out scopes, out jobj))
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

                #endregion

                #region Ｂ層・Ｄ層呼出し

                // ★
                status = FxLiteral.SIF_STATUS_INVOKE;

                try
                {
                    // Ｂ層・Ｄ層呼出し

                    //// DLL名も指定するパターン（別DLLに含まれる）
                    //muReturnValue = (MuReturnValue)Latebind.InvokeMethod(
                    //    assemblyName, className, FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME, paramSet);

                    // DLL名は指定しないパターン（ExecutingAssemblyに含まれる）
                    Assembly asm = Assembly.GetExecutingAssembly();

                    // DLL名は指定しないパターンでの例外処理
                    Type t = asm.GetType(className);
                    if (t == null)
                    {
                        throw new BusinessSystemException("NoLBTypeInExecutingAssembly", string.Format("{0}クラスがExecutingAssemblyに存在しません。", className));
                    }

                    object o = Activator.CreateInstance(t);
                    muReturnValue = (MuReturnValue)Latebind.InvokeMethod(o, FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME, paramSet);
                }
                catch (System.Reflection.TargetInvocationException rtEx)
                {
                    // InnerExceptionを投げなおす。
                    throw rtEx.InnerException;
                }

                #endregion

                #region 戻り値

                // ★
                status = FxLiteral.SIF_STATUS_SERIALIZE;

                returnValue = muReturnValue.Bean;

                if (muReturnValue.ErrorFlag)
                {
                    // エラー情報を設定する。
                    wsErrorRoot = wsErrorInfo.CreateElement("ErrorInfo");
                    wsErrorInfo.AppendChild(wsErrorRoot);

                    // 業務例外
                    wsErrorItem = wsErrorInfo.CreateElement("ErrorType");
                    wsErrorItem.InnerText = FxEnum.ErrorType.BusinessApplicationException.ToString();
                    wsErrorRoot.AppendChild(wsErrorItem);

                    wsErrorItem = wsErrorInfo.CreateElement("MessageID");
                    wsErrorItem.InnerText = muReturnValue.ErrorMessageID;
                    wsErrorRoot.AppendChild(wsErrorItem);

                    wsErrorItem = wsErrorInfo.CreateElement("Message");
                    wsErrorItem.InnerText = muReturnValue.ErrorMessage;
                    wsErrorRoot.AppendChild(wsErrorItem);

                    wsErrorItem = wsErrorInfo.CreateElement("Information");
                    wsErrorItem.InnerText = muReturnValue.ErrorInfo;
                    wsErrorRoot.AppendChild(wsErrorItem);

                    // ログ出力用の情報を保存
                    errorType = FxEnum.ErrorType.BusinessApplicationException.ToString(); // 2009/09/15-この行
                    errorMessageID = muReturnValue.ErrorMessageID;
                    errorMessage = muReturnValue.ErrorMessage;
                    errorToString = muReturnValue.ErrorInfo;

                    // エラー情報を戻す。
                    return wsErrorInfo.InnerXml;
                }

                #endregion

                // ★
                status = "";

                // 戻り値を返す。
                return "";
            }
            //catch (BusinessApplicationException baEx)
            //{
            // ここには来ない↑
            //}
            catch (BusinessSystemException bsEx)
            {
                // エラー情報を設定する。
                wsErrorRoot = wsErrorInfo.CreateElement("ErrorInfo");
                wsErrorInfo.AppendChild(wsErrorRoot);

                // システム例外
                wsErrorItem = wsErrorInfo.CreateElement("ErrorType");
                wsErrorItem.InnerText = FxEnum.ErrorType.BusinessSystemException.ToString();
                wsErrorRoot.AppendChild(wsErrorItem);

                wsErrorItem = wsErrorInfo.CreateElement("MessageID");
                wsErrorItem.InnerText = bsEx.messageID;
                wsErrorRoot.AppendChild(wsErrorItem);

                wsErrorItem = wsErrorInfo.CreateElement("Message");
                wsErrorItem.InnerText = bsEx.Message;
                wsErrorRoot.AppendChild(wsErrorItem);

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.BusinessSystemException.ToString(); // 2009/09/15-この行
                errorMessageID = bsEx.messageID;
                errorMessage = bsEx.Message;

                errorToString = bsEx.ToString();

                // エラー情報を戻す。
                return wsErrorInfo.InnerXml;
            }
            catch (FrameworkException fxEx)
            {
                // エラー情報を設定する。
                wsErrorRoot = wsErrorInfo.CreateElement("ErrorInfo");
                wsErrorInfo.AppendChild(wsErrorRoot);

                // フレームワーク例外
                // ★ インナーエクセプション情報は消失
                wsErrorItem = wsErrorInfo.CreateElement("ErrorType");
                wsErrorItem.InnerText = FxEnum.ErrorType.FrameworkException.ToString();
                wsErrorRoot.AppendChild(wsErrorItem);

                wsErrorItem = wsErrorInfo.CreateElement("MessageID");
                wsErrorItem.InnerText = fxEx.messageID;
                wsErrorRoot.AppendChild(wsErrorItem);

                wsErrorItem = wsErrorInfo.CreateElement("Message");
                wsErrorItem.InnerText = fxEx.Message;
                wsErrorRoot.AppendChild(wsErrorItem);

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.FrameworkException.ToString(); // 2009/09/15-この行
                errorMessageID = fxEx.messageID;
                errorMessage = fxEx.Message;

                errorToString = fxEx.ToString();

                // エラー情報を戻す。
                return wsErrorInfo.InnerXml;
            }
            catch (Exception ex)
            {
                // エラー情報を設定する。
                wsErrorRoot = wsErrorInfo.CreateElement("ErrorInfo");
                wsErrorInfo.AppendChild(wsErrorRoot);

                // フレームワーク例外
                // ★ インナーエクセプション情報は消失
                wsErrorItem = wsErrorInfo.CreateElement("ErrorType");
                wsErrorItem.InnerText = FxEnum.ErrorType.ElseException.ToString();
                wsErrorRoot.AppendChild(wsErrorItem);

                wsErrorItem = wsErrorInfo.CreateElement("MessageID");
                wsErrorItem.InnerText = "-";
                wsErrorRoot.AppendChild(wsErrorItem);

                wsErrorItem = wsErrorInfo.CreateElement("Message");
                wsErrorItem.InnerText = ex.ToString();
                wsErrorRoot.AppendChild(wsErrorItem);

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.ElseException.ToString(); // 2009/09/15-この行
                errorMessageID = "-";
                errorMessage = ex.Message;

                // どちらを戻すべきか？
                // Muの場合は、Messageがデフォ
                errorToString = ex.Message;
                //errorToString = ex.ToString();

                // エラー情報を戻す。
                return wsErrorInfo.InnerXml;
                //throw; // コメントアウト
            }
            finally
            {
                // 用途によってSessionを解放するかどうかを検討。

                //// Sessionステートレス
                //Session.Clear();
                //Session.Abandon();

                // 終了ログの出力
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
                        + errorToString + "\r\n");
                }
            }
        }

        #endregion

        #endregion
    }
}