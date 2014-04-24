//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：ServiceForMu
//* クラス日本語名  ：ASP.NET Webサービス（サービス インターフェイス基盤）
//*                   SOAP汎用Webメソッドを公開する。
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/06/18  西野 大介         新規作成
//*  2009/09/15  西野 大介         エラー タイプの情報をログ出力するようにした。
//*  2009/09/29  西野 大介         ユーザ情報をコンテキストに（名前、型の変更）
//*  2010/03/03  西野 大介         呼び出しB層を自動振り分け機能付きのものに変更したため、
//*                                引数クラス作成時にメソッド名を指定するように変更した。
//*  2010/03/11  西野 大介         引数の順番を変更した。
//*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
//*  2010/12/09  西野 大介         引数クラスのベース１・２の（持ち回り情報用の）引数を追加。
//*  2011/01/24  西野 大介         サンプルで利用するため処理の具体化を行った。
//*  2011/07/06  西野 大介         認証サービス・テンプレートに対応したコードを追加
//*  2011/11/21  西野 大介         silverlight専用対応
//*                               ・App_CodeのB層を利用するように名前解決処理を変更。
//*                               ・業務例外が呼び出し元に伝播しない問題の修正を実施。
//*                               ・認証済みか否かのチェックを追加。
//*  2011/12/08  西野 大介         その他、一般的な例外も文字列化して戻す。
//**********************************************************************************

using System.EnterpriseServices;

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

namespace Touryo.Infrastructure.Framework.ServiceInterface.ASPNETWebService
{

    // 名前空間は、必要に応じて書き換え下さい。

    /// <summary>
    /// ASP.NET Webサービス（サービス インターフェイス基盤）
    /// SOAP汎用Webメソッドを公開する。
    /// </summary>
    [WebService(Namespace = FxLiteral.WS_NAME_SPACE)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ServiceForMu : System.Web.Services.WebService
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

        public ServiceForMu()
        {
            //デザインされたコンポーネントを使用する場合、次の行をコメントを解除してください 
            //InitializeComponent(); 
        }

        #endregion

        /// <summary>SOAP汎用Webメソッド</summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="serviceName">サービス名</param>
        /// <param name="cmnParameterValue">引数文字列配列（共通・・・ベース１・２）</param>
        /// <param name="parameterValue">引数文字列（個別・・・サブ）</param>
        /// <param name="returnValue">戻り値文字列</param>
        /// <returns>返すべきエラーの情報</returns>
        /// <remarks>値は全て文字列データ</remarks>
        [WebMethod(
            // IIS - ASP.NET（ワーカ プロセス）間のバッファリング
            BufferResponse = true,
            // キャッシュ無効化
            CacheDuration = 0,
            // Webサービスの説明
            Description = FxLiteral.WS_METHOD_DESCRIPTION_MULTI_USE,
            // Sessionのサポート（コンテキスト情報用・一時利用）
            EnableSession = true,
            // オーバーロード時の識別用エイリアス
            MessageName = "",
            // トランザクション サポート
            TransactionOption = TransactionOption.NotSupported
            )]
        public string Call(ref string context,
            string serviceName, string[] cmnParameterValue, string parameterValue, out string returnValue)
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
                ServiceForMu.IPR_NS.NameResolution(serviceName, out assemblyName, out className);

                #endregion

                #region 引数の.NETオブジェクト化（ＵＯＣ）

                // ★
                status = FxLiteral.SIF_STATUS_DESERIALIZE;

                // ★★　引数の.NETオブジェクト化をＵＯＣする（必要に応じて）。

                // 引数文字列の.NETオブジェクト化
                
                // string[] cmnParameterValueを使用して初期化するなど
                muParameterValue = new MuParameterValue(
                    cmnParameterValue[0], // 画面名
                    cmnParameterValue[1], // ボタン名
                    cmnParameterValue[2], // メソッド名
                    cmnParameterValue[3], // アクションタイプ
                    new MyUserInfo(context, HttpContext.Current.Request.UserHostAddress));

                // parameterValueを引数の文字列フィールドに設定
                muParameterValue.Value = parameterValue;

                // 引数クラスをパラメタ セットに格納
                object[] paramSet = new object[] { muParameterValue, DbEnum.IsolationLevelEnum.User };
                
                #endregion

                #region 認証処理（ＵＯＣ）

                // ★
                status = FxLiteral.SIF_STATUS_AUTHENTICATION;

                // ★★　認証が通っているかどうか確認する。
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

                // 持ち回るならCookieにするか、
                // contextをrefにするなどとする。
                context = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"); // 更新されたかのテストコード

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

                returnValue = muReturnValue.Value;

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
                // Muの場合は、ToStringがデフォ
                //errorToString = ex.Message;
                errorToString = ex.ToString();

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
    }
}
