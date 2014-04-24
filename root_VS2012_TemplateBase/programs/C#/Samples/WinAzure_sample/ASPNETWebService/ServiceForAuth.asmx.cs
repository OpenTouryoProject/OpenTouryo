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
//* クラス名        ：ServiceForAuth
//* クラス日本語名  ：認証サービスを公開する。
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//*  2012/08/25  西野  大介        Assembly.LoadFile → .Load（ASP.NETシャドウコピー対応）
//**********************************************************************************

using System.IO;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.EnterpriseServices;

// System
using System;
using System.Xml;
using System.Data;
using System.Collections;

// System.Web
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Dao;
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
    /// 認証サービスを公開する。
    /// </summary>
    [WebService(Namespace = FxLiteral.WS_NAME_SPACE)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ServiceForAuth : System.Web.Services.WebService
    {
        #region コンストラクタ

        public ServiceForAuth()
        {
            //デザインされたコンポーネントを使用する場合、次の行をコメントを解除してください 
            //InitializeComponent(); 
            MyBaseDao.UseEmbeddedResource = true;
        }

        #endregion

        /// <summary>チャレンジを返す</summary>
        /// <returns>チャレンジ</returns>
        [WebMethod(
            // IIS - ASP.NET（ワーカ プロセス）間のバッファリング
            BufferResponse = true,
            // キャッシュ無効化
            CacheDuration = 0,
            // Webサービスの説明
            Description = "Authentication Service(GetChallenge)",
            // Sessionのサポート
            EnableSession = true,
            // オーバーロード時の識別用エイリアス
            MessageName = "",
            // トランザクション サポート
            TransactionOption = TransactionOption.NotSupported
            )]
        public string GetChallenge()
        {
            // チャレンジ
            Session["challenge"] = Guid.NewGuid().ToString();
            return (string)Session["challenge"];
        }

        /// <summary>認証チケットを返す</summary>
        /// <param name="encUid">チャレンジで暗号化されたユーザID</param>
        /// <param name="encPwd">チャレンジで暗号化されたパスワード</param>
        /// <returns>認証チケット</returns>
        [WebMethod(
            // IIS - ASP.NET（ワーカ プロセス）間のバッファリング
            BufferResponse = true,
            // キャッシュ無効化
            CacheDuration = 0,
            // Webサービスの説明
            Description = "Authentication Service(GetAuthTicket)",
            // Sessionのサポート
            EnableSession = true,
            // オーバーロード時の識別用エイリアス
            MessageName = "",
            // トランザクション サポート
            TransactionOption = TransactionOption.NotSupported
            )]
        public string GetAuthTicket(string encUid, string encPwd)
        {
            try
            {
                // ユーザIDの復号化
                string uid = SymmetricCryptography.DecryptString(
                    encUid, (string)Session["challenge"], EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);
                // パスワードの復号化
                string pwd = SymmetricCryptography.DecryptString(
                    encPwd, (string)Session["challenge"], EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);

                // 認証する。
                bool isAuthenticated = false;

                #region 認証処理のＵＯＣ

                // ★★　コンテキストの情報を使用するなどして
                //       認証処理をＵＯＣする（必要に応じて）。

                //// Ｂ層・Ｄ層呼出し
                ////   認証チェックとタイムスタンプの更新
                //MyUserInfo userInfo =new MyUserInfo(
                //    "未認証：" + uid, HttpContext.Current.Request.UserHostAddress);

                //BaseReturnValue returnValue = (BaseReturnValue)Latebind.InvokeMethod(
                //    "xxxx", "yyyy",
                //    FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME,
                //    new object[] {
                //        new AuthParameterValue("－", "－", "zzzz", "",userInfo, pwd),
                //        DbEnum.IsolationLevelEnum.User });

                //// 認証されたか・されなかったか
                //isAuthenticated = !returnValue.ErrorFlag;

                isAuthenticated = true;

                #endregion

                if (isAuthenticated)
                {
                    // 認証チケットを作成して暗号化する（DateTime.Nowにより可変に）。
                    string[] authTicket = { uid, pwd, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") };

                    return SymmetricCryptography.EncryptString(
                        CustomEncode.ToBase64String(
                            BinarySerialize.ObjectToBytes(authTicket)),
                        GetConfigParameter.GetConfigValue("private-key"),
                        EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);
                }
                else
                {
                    // 認証失敗
                    return string.Empty;
                }
            }
            catch
            {
                // 認証失敗
                return string.Empty;
            }
            finally
            {
                // セッションの解放
                Session.Abandon();
            }
        }

        /// <summary>認証チケットを検証する。</summary>
        /// <param name="authTicket">認証チケット（暗号化）</param>
        /// <returns>認証チケット（復号化）</returns>
        /// <remarks>注：テスト用です。</remarks>
        [WebMethod(
            // IIS - ASP.NET（ワーカ プロセス）間のバッファリング
            BufferResponse = true,
            // キャッシュ無効化
            CacheDuration = 0,
            // Webサービスの説明
            Description = "Authentication Service(ValidateAuthTicket)",
            // Sessionのサポート
            EnableSession = false,
            // オーバーロード時の識別用エイリアス
            MessageName = "",
            // トランザクション サポート
            TransactionOption = TransactionOption.NotSupported
            )]
        public string[] ValidateAuthTicket(string authTicket)
        {
            try
            {
                // 認証チケットの復号化
                return (string[])BinarySerialize.BytesToObject(
                    CustomEncode.FromBase64String(
                        SymmetricCryptography.DecryptString(
                            authTicket, GetConfigParameter.GetConfigValue("private-key"),
                            EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)));
            }
            catch
            {
                // 認証失敗
                return null;
            }
        }
    }
}
