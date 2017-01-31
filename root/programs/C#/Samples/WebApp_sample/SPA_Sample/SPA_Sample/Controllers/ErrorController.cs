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
//* クラス名        ：ErrorController
//* クラス日本語名  ：エラー処理用コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/08/27  Supragyan         Created ErrorController class to display error messages and informations
//*  2015/09/03  Supragyan         Rename Position data class to Exception data class
//*  2015/09/03  Supragyan         Modified Index Action method
//*  2015/09/04  Supragyan         Modified ArrayList to List of ExceptionData on Index action method
//**********************************************************************************

using SPA_Sample.Models;

using System;
using System.Web.Mvc;
using System.Collections.Generic;

using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Str;

namespace SPA_Sample.Controllers
{
    #region ErrorController
    /// <summary>
    /// ErrorController class
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>Session情報：リピータ処理用</summary>
        private List<ExceptionData> listData = new List<ExceptionData>();

        #region Index
        /// <summary>
        /// Index Action method to display an error message error information on the screen
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            //To get an error message from Session
            string err_msg = (string)Session[FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE];

            //To get an error information from Session
            string err_info = (string)Session[FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION];

            //To encode error message and display on Error screen
            ViewBag.label1Data = CustomEncode.HtmlEncode(err_msg);

            //To encode error information and display on Error screen
            ViewBag.label2Data = CustomEncode.HtmlEncode(err_info);

            // 画面にセッション情報を表示する------------------------------------------

            if (Session != null)
            {
                //foreach
                foreach (string strKey in Session)
                {
                    if (Session[strKey] == null)
                    {
                        //Add key and value to PositionData
                        listData.Add(new ExceptionData(strKey, "null"));
                    }
                    else
                    {
                        //Add key and value to PositionData
                        listData.Add(new ExceptionData(strKey, CustomEncode.HtmlEncode(Session[strKey].ToString())));
                    }
                }
                //データバインド
                ViewBag.datas = listData;
            }

            if (Session[FxHttpContextIndex.SESSION_ABANDON_FLAG] != null)
            {
                // セッション情報を削除する------------------------------------------------
                if ((bool)Session[FxHttpContextIndex.SESSION_ABANDON_FLAG])
                {
                    // セッション タイムアウト検出用Cookieを消去
                    // ※ Removeが正常に動作しないため、値を空文字に設定 ＝ 消去とする

                    // Set-Cookie HTTPヘッダをレスポンス
                    Response.Cookies.Set(FxCmnFunction.DeleteCookieForSessionTimeoutDetection());

                    try
                    {
                        //To store error information from session before clear the session
                        ErrorInformation.ErrorMessage = (string)Session[FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE];
                        ErrorInformation.ErrorInfo = (string)Session[FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION];
                        ErrorInformation.ErrorDatas = listData;

                        // セッションを消去                       
                        Session.Abandon();
                    }
                    catch (Exception ex)
                    {
                        // エラー発生時
                        // このカバレージを通過する場合、
                        // おそらく起動した画面のパスが間違っている。
                        Console.WriteLine("このカバレージを通過する場合、おそらく起動した画面のパスが間違っている。");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return View();
        }

        #endregion
    }

    #endregion

    # region ExceptionData

    /// <summary>
    /// ExceptionData class to set key and value for throwing exception 
    /// </summary>
    public class ExceptionData
    {
        /// <summary>キー</summary>
        private string _key;

        /// <summary>値</summary>
        private string _value;

        /// <summary>コンストラクタ</summary>
        public ExceptionData(string key, string value)
        {
            this._key = key;
            this._value = value;
        }

        /// <summary>キー</summary>
        public string key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>値</summary>
        public string value
        {
            get
            {
                return _value;
            }
        }
    }

    # endregion
}

