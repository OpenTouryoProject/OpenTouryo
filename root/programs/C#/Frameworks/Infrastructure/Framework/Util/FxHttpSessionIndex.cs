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
//* クラス名        ：FxHttpSessionIndex
//* クラス日本語名  ：Sessionのインデックスに使用する文字列定数を定義する定数クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/03/13  西野  大介        ラベル名の変更、ラベルの追加
//*  2009/04/05  西野  大介        ラベルの追加（Mobile用のエラー画面遷移）
//*  2009/07/31  西野  大介        セッション情報の自動削除機能を追加
//*  2009/07/31  西野  大介        不正操作の検出機能を追加
//*  2009/09/01  西野  大介        サブシステム セッション スコープの追加
//*  2013/12/23  西野  大介        アクセス修飾子をすべてpublicに変更した。
//*  2015/10/29  Sai               Added constant string to store dummy key and value.
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>Sessionのインデックスに使用する文字列定数を定義する</summary>
    public static class FxHttpSessionIndex
    {
        /// <summary>セッション単位のロックを提供するオブジェクトを格納するSessionキー</summary>
        public const string SESSION_LOCK = "SessionLock";

        /// <summary>認証ユーザ情報を格納するSessionキー</summary>
        public const string AUTHENTICATION_USER_INFORMATION = "AuthenticationUserInformation";

        ///// <summary>認証ユーザ情報を格納するSessionキー（複数）</summary>
        //public const string AUTHENTICATION_USER_INFORMATIONS = "AuthenticationUserInformations:";

        /// <summary>サブシステム情報を格納するSessionキー</summary>
        public const string SUB_SYSTEM_INFORMATION = "SubSystemInformation";

        /// <summary>ボタン履歴を保持する「キュー」を格納するSessionキー</summary>
        public const string BUTTON_HISTORY = "ButtonHistory";

        /// <summary> Session key that contains the dummy value. </summary>
        public const string DUMMY = "dummy";

        /// <summary>画面GUIDを保持する「キュー」を格納するSessionキー</summary>
        public const string SCREEN_GUID_QUEUE = "ScreenGuidQueue";

        /// <summary>ウィンドウGUIDを保持する「キュー」を格納するSessionキー</summary>
        public const string WINDOW_GUID_QUEUE = "WindowGuidQueue";

        /// <summary>リクエスト チケットGUIDを保持する「キュー」を格納するSessionキー</summary>
        public const string REQUEST_TICKET_GUID_QUEUE = "RequestTicketGuidQueue";

        #region ｘｘ別セッション

        #region 親画面別

        /// <summary>親画面別セッション領域のルートSessionキー</summary>
        public const string SESSION_SCOPE_OF_PARENT_SCREEN_BY_GUID = "SessionScopeOfParentScreenByGUID:";

        #endregion

        #region ウィンドウ別

        /// <summary>ウィンドウ別セッション領域のルートSessionキー</summary>
        public const string SESSION_SCOPE_OF_BROWSER_WINDOW_BY_GUID = "SessionScopeOfBrowserWindowByGUID:";

        /// <summary>前画面情報を保持するウィンドウ別セッション領域のSessionキー</summary>
        public const string SCREEN_TRANSITION_INFO = "ScreenTransitionInfo";

        #endregion

        #endregion

        #region モーダル・ダイアログへのデータ受け渡し用

        /// <summary>モーダル・ダイアログのダイアログ名を格納するSessionキー</summary>
        public const string MODAL_DIALOG_NAME = ":ModalDialogName";

        /// <summary>モーダル・ダイアログのアイコン・タイプを格納するSessionキー</summary>
        public const string MODAL_DIALOG_ICONTYPE = ":ModalDialogIconType";

        /// <summary>モーダル・ダイアログに渡すためメッセージＩＤを格納するSessionキー</summary>
        public const string MODAL_DIALOG_MESSAGEID = ":ModalDialogMessageId";

        /// <summary>モーダル・ダイアログに渡すためメッセージを格納するSessionキー</summary>
        public const string MODAL_DIALOG_MESSAGE = ":ModalDialogMessage";

        #endregion

        #region エラー画面への情報引継ぎ用（Mobile用、MobilePage.RedirectToMobilePage＋HttpSession）

        /// <summary>システムエラー発生時、システムエラー情報をHttpContextに設定～取得するためのキー</summary>
        public const string SYSTEM_EXCEPTION_INFORMATION = "SystemExceptionInformation";

        /// <summary>システムエラー発生時、エラーメッセージをHttpContextに設定～取得するためのキー</summary>
        public const string SYSTEM_EXCEPTION_MESSAGE = "SystemExceptionMessage";

        #endregion
    }
}
