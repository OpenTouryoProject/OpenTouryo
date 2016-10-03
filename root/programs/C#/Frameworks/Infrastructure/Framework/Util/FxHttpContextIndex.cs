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
//* クラス名        ：FxHttpContextIndex
//* クラス日本語名  ：HttpContextのインデックスに使用する文字列定数を定義する定数クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/03/13  西野  大介        ラベルの追加
//*  2009/07/31  西野  大介        ラベルの追加
//*  2013/12/23  西野  大介        アクセス修飾子をすべてpublicに変更した。
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>HttpContextのインデックスに使用する文字列定数を定義する</summary>
    public class FxHttpContextIndex
    {
        /// <summary>画面遷移時に次画面に「ウィンドウGUID」を渡すためのキー</summary>
        public const string BROWSER_WINDOW_GUID = "BrowserWindowGUID";

        #region エラー画面への情報引継ぎ用（Form情報を取りたいので、Transfer＋HttpContext）

        /// <summary>システムエラー発生時、システムエラー情報をHttpContextに設定～取得するためのキー</summary>
        public const string SYSTEM_EXCEPTION_INFORMATION = "SystemExceptionInformation";

        /// <summary>システムエラー発生時、エラーメッセージをHttpContextに設定～取得するためのキー</summary>
        public const string SYSTEM_EXCEPTION_MESSAGE = "SystemExceptionMessage";

        /// <summary>システムエラー発生時、セッション削除フラグをHttpContextに設定～取得するためのキー</summary>
        public const string SESSION_ABANDON_FLAG = "SessionAbandonFlag";

        #endregion
    }
}
