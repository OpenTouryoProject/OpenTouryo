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
//* クラス名        ：ServiceForRt
//* クラス日本語名  ：WCF Webサービス（サービス インターフェイス基盤）
//*                   REST（XML、JSON）汎用Webメソッドを公開する。
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/08/13  西野 大介         新規作成
//**********************************************************************************

// System
using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using System.Reflection;

// System.Web
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;

using Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest;

namespace Touryo.Infrastructure.Framework.ServiceInterface.WCFWebService
{
    /// <summary>
    /// WCF Webサービス（サービス インターフェイス基盤）
    /// REST（XML、JSON）汎用Webメソッドを公開する。
    /// </summary>
    public class ServiceForRt : IServiceForRt
    {
        /// <summary>インプロセス呼び出しの名前解決シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static InProcessNameService IPR_NS = new InProcessNameService();

        /// <summary>XML汎用Webメソッド</summary>
        /// <param name="param">XML 形式で送信された引数（ParamDataContract）</param>
        /// <returns>XML 形式で送信される戻り値（ReturnDataContract）</returns>
        public ReturnDataContract XML(ParamDataContract param)
        {
            return this.Call(param);
        }

        /// <summary>JSON汎用Webメソッド</summary>
        /// <param name="param">JSON 形式で送信された引数（ParamDataContract）</param>
        /// <returns>JSON 形式で送信される戻り値（ReturnDataContract）</returns>
        public ReturnDataContract JSON(ParamDataContract param)
        {
            return this.Call(param);
        }

        /// <summary>WCF サービス本体</summary>
        /// <param name="param">REST 形式で送信された引数（ParamDataContract）</param>
        /// <returns>REST 形式で送信される戻り値（ReturnDataContract）</returns>
        private ReturnDataContract Call(ParamDataContract param)
        {
            // ステータス
            string status = "－";

            // 戻り値
            ReturnDataContract retValue = new ReturnDataContract();

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
                ServiceForRt.IPR_NS.NameResolution(param.ServiceName, out assemblyName, out className);

                #endregion

                #region 引数の.NETオブジェクト化（ＵＯＣ）

                // ★
                status = FxLiteral.SIF_STATUS_DESERIALIZE;

                // ★★　引数の.NETオブジェクト化をＵＯＣする（必要に応じて）。

                // 引数文字列の.NETオブジェクト化

                // クライアントの IP アドレス
                string IpAddress = string.Empty;

                // クライアントの IP アドレスを取得
                OperationContext context = OperationContext.Current;
                if (context.IncomingMessageProperties.ContainsKey(RemoteEndpointMessageProperty.Name) == true)
                {
                    RemoteEndpointMessageProperty property = (RemoteEndpointMessageProperty)context.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];
                    IpAddress = property.Address;
                }

                // ParamDataContractを使用して初期化するなど
                muParameterValue = new MuParameterValue(
                    param.ScreenId == null ? string.Empty : param.ScreenId,     // 画面名
                    param.ControlId == null ? string.Empty : param.ControlId,   // ボタン名
                    param.MethodName == null ? string.Empty : param.MethodName, // メソッド名
                    param.ActionType == null ? string.Empty : param.ActionType, // アクションタイプ
                    new MyUserInfo(param.UserName, IpAddress));

                // ParameterValueを引数のBeanフィールドに設定
                muParameterValue.Bean = param.Info;

                // 引数クラスをパラメタ セットに格納
                object[] paramSet = new object[] { muParameterValue, DbEnum.IsolationLevelEnum.User };

                #endregion

                #region 認証処理（ＵＯＣ）

                // ★
                status = FxLiteral.SIF_STATUS_AUTHENTICATION;

                //// ★★　認証が通っているかどうか確認する。
                //if (!HttpContext.Current.Request.IsAuthenticated)
                //{
                //    throw new BusinessSystemException("Authentication", "認証されていません。");
                //}

                // ★★　コンテキストの情報を使用するなどして
                //       認証処理をＵＯＣする（必要に応じて）。

                //// 認証チケットの復号化
                //string[] authTicket = (string[])BinarySerialize.BytesToObject(
                //    CustomEncode.FromBase64String(
                //        SymmetricCryptography.DecryptString(
                //            context, GetConfigParameter.GetConfigValue("private-key"),
                //            EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)));

                //// ユーザIDの設定
                //muParameterValue.User.UserName = authTicket[0];

                // 認証チケットの整合性を確認
                // Forms認証では、machinekeyを使用している。
                // 必要に応じて認証サービス側も作り変える。

                //// Ｂ層・Ｄ層呼出し
                ////   タイムスタンプのチェックと、更新
                ////   スライディング・タイムアウトの実装、
                ////   必要であればアカウントの検証も実施
                //BaseReturnValue _returnValue = (BaseReturnValue)Latebind.InvokeMethod(
                //    "xxxx", "yyyy",
                //    FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME,
                //    new object[] { new AuthParameterValue("－", "－", "zzzz", "",
                //        muParameterValue.User, authTicket[1]),
                //        DbEnum.IsolationLevelEnum.User });

                //if (_returnValue.ErrorFlag)
                //{
                //    // 認証エラー
                //    throw new BusinessSystemException("xxxx", "認証チケットが不正か、タイムアウトです。");
                //}

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

                #region 戻り値の文字列化

                // ★
                status = FxLiteral.SIF_STATUS_SERIALIZE;

                if (muReturnValue.ErrorFlag)
                {
                    // エラー情報を設定する。
                    ErrorInfo errorInfo = new ErrorInfo();

                    // 業務例外
                    errorInfo.ErrorType = FxEnum.ErrorType.BusinessApplicationException.ToString();
                    errorInfo.MessageID = muReturnValue.ErrorMessageID;
                    errorInfo.Message = muReturnValue.ErrorMessage;
                    errorInfo.Information = muReturnValue.ErrorInfo;

                    // ログ出力用の情報を保存
                    errorType = FxEnum.ErrorType.BusinessApplicationException.ToString(); // 2009/09/15-この行
                    errorMessageID = muReturnValue.ErrorMessageID;
                    errorMessage = muReturnValue.ErrorMessage;
                    errorToString = muReturnValue.ErrorInfo;

                    // エラー情報を戻す。
                    retValue.Error = errorInfo;
                }

                #endregion

                // ★
                status = "";

                // 戻り値を設定
                if (muReturnValue.Bean != null && muReturnValue.Bean is Informations)
                {
                    // 正規の戻り値の場合
                    retValue.Info = (Informations)muReturnValue.Bean;
                }
                else
                {
                    //// 不正な戻り値の場合
                    //retValue.Info = new Informations("");
                    throw new Exception("不正な戻り値");
                }

                // 戻り値を返す。
                return retValue;
            }
            //catch (BusinessApplicationException baEx)
            //{
            // ここには来ない↑
            //}
            catch (BusinessSystemException bsEx)
            {
                // エラー情報を設定する。
                ErrorInfo errorInfo = new ErrorInfo();

                // システム例外
                errorInfo.ErrorType = FxEnum.ErrorType.BusinessSystemException.ToString();
                errorInfo.MessageID = bsEx.messageID;
                errorInfo.Message = bsEx.Message;
                errorInfo.Information = string.Empty;

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.BusinessSystemException.ToString(); // 2009/09/15-この行
                errorMessageID = bsEx.messageID;
                errorMessage = bsEx.Message;

                errorToString = bsEx.ToString();

                // エラー情報を戻す。
                retValue.Error = errorInfo;
                return retValue;
            }
            catch (FrameworkException fxEx)
            {
                // エラー情報を設定する。
                ErrorInfo errorInfo = new ErrorInfo();

                // フレームワーク例外
                // ★ インナーエクセプション情報は消失
                errorInfo.ErrorType = FxEnum.ErrorType.FrameworkException.ToString();
                errorInfo.MessageID = fxEx.messageID;
                errorInfo.Message = fxEx.Message;
                errorInfo.Information = string.Empty;

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.FrameworkException.ToString(); // 2009/09/15-この行
                errorMessageID = fxEx.messageID;
                errorMessage = fxEx.Message;

                errorToString = fxEx.ToString();

                // エラー情報を戻す。
                retValue.Error = errorInfo;
                return retValue;
            }
            catch (Exception ex)
            {
                // エラー情報を設定する。
                ErrorInfo errorInfo = new ErrorInfo();

                // フレームワーク例外
                // ★ インナーエクセプション情報は消失
                errorInfo.ErrorType = FxEnum.ErrorType.ElseException.ToString();
                errorInfo.MessageID = "-";
                errorInfo.Message = ex.ToString();
                errorInfo.Information = string.Empty;

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.ElseException.ToString(); // 2009/09/15-この行
                errorMessageID = "-";
                errorMessage = ex.Message;

                // どちらを戻すべきか？
                // Muの場合は、ToStringがデフォ
                //errorToString = ex.Message;
                errorToString = ex.ToString();

                // エラー情報を戻す。
                retValue.Error = errorInfo;
                return retValue;
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
    }
}
