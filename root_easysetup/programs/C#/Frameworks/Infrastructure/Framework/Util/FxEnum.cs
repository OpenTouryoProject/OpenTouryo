//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：FxEnum
//* クラス日本語名  ：Framework名前空間で使用する列挙型クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/03/27  西野  大介        定義の追加（Ajax）
//*  2009/04/02  西野  大介        定義の追加（エラーの型）
//*  2009/04/21  西野  大介        FrameworkExceptionの追加に伴い、定義追加
//*  2009/07/21  西野  大介        マスタ ページのネストに対応
//*  2012/12/14  西野  大介         WCF-HTTP対応
//*  201x/12/17  西野  大介         WCF-TCP/IP対応
//*  2013/12/23  西野  大介        アクセス修飾子をすべてpublicに変更した。
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>フレームワーク内で使用する列挙型</summary>
    public static class FxEnum
    {
        #region コントロール取得処理

        /// <summary>コントロール検索処理のタイプ</summary>
        public enum ControlSearchType : int
        {
            /// <summary>
            /// マスタ ページ
            /// </summary>
            MasterPage = 1,

            /// <summary>
            /// コンテンツ ページ
            /// </summary>
            ContentsPage
        }
        
        #endregion

        #region ダイアログ関係

        /// <summary>表示する子画面のタイプ</summary>
        public enum ChildScreenType : int
        {
            /// <summary>
            /// 「OK」メッセージ ダイアログ
            /// </summary>
            OKMessageDialog = 1,

            /// <summary>
            /// 「Yes」「No」メッセージ ダイアログ
            /// </summary>
            YesNoMessageDialog,

            /// <summary>
            /// 業務モーダル画面
            /// </summary>
            ModalScreen,

            /// <summary>
            /// 業務モードレス画面
            /// </summary>
            NormalScreen
        }

        /// <summary>画面を閉じる際のモード</summary>
        public enum CloseMode : int
        {
            /// <summary>
            /// 通常の方式
            /// </summary>
            Normal = 1,

            /// <summary>
            /// 親画面でポストバック（後処理）しない。
            /// </summary>
            NoPostback,

            /// <summary>
            /// 全て親ダイアログを閉じ、ルートの親画面でのみ後処理する。
            /// </summary>
            WithAllParent
        }

        /// <summary>後処理のサブミットモード</summary>
        public enum SubmitMode : int
        {
            /// <summary>「Yes」「No」メッセージ ダイアログを×で閉じた。</summary>
            YesNo_X = 1,

            /// <summary>「Yes」「No」メッセージ ダイアログを「Yes」で閉じた。</summary>
            YesNo_Yes,

            /// <summary>「Yes」「No」メッセージ ダイアログを「No」で閉じた。</summary>
            YesNo_No,

            /// <summary>業務モーダル画面の後処理</summary>
            Modal
        }
        
        /// <summary>メッセージ ダイアログの画像を表す列挙型</summary>
        public enum IconType : int
        {
            /// <summary>
            /// [ i ]（情報）
            /// </summary>
            Information,

            /// <summary>
            /// [ ! ]（警告）
            /// </summary>
            Exclamation,

            /// <summary>
            /// [ × ]（エラー）
            /// </summary>
            StopMark
        }

        #endregion

        #region AjaxExtension

        /// <summary>AjaxExtensionの状態を表す列挙型</summary>
        public enum AjaxExtStat : int
        {
            /// <summary>
            /// AjaxExtensionをサポートしない画面
            /// </summary>
            NoAjaxExtension,

            /// <summary>
            /// AjaxExtensionをサポートする画面だが、当該処理はAjaxExtensionでない。
            /// </summary>
            IsNotAjaxExtension,

            /// <summary>
            /// AjaxExtensionをサポートする画面であり、当該処理はAjaxExtensionである。
            /// </summary>
            IsAjaxExtension
        }

        #endregion

        #region 通信制御

        /// <summary>通信制御のプロトコルを表す列挙型</summary>
        public enum TmProtocol : int
        {
            /// <summary>
            /// インプロセス呼び出し
            /// </summary>
            InProcess = 1,

            /// <summary>
            /// Webサービス（WS-I Basic Profile v1.1、IIS ＋ ASP.NET）
            /// </summary>
            AspNetWs,

            /// <summary>
            /// WCF : basicHTTPBinding、wsHTTPBinding
            /// </summary>
            WCF_HTTP,

            /// <summary>
            /// WCF : netTcpBinding
            /// </summary>
            WCF_TCPIP
        }

        /// <summary>エラー型情報を表す列挙型</summary>
        public enum ErrorType : int
        {
            /// <summary>
            /// 業務例外：BusinessApplicationException
            /// </summary>
            BusinessApplicationException,

            /// <summary>
            /// システム例外：BusinessSystemException
            /// </summary>
            BusinessSystemException,

            /// <summary>
            /// フレームワーク例外：FrameworkException
            /// </summary>
            FrameworkException,

            /// <summary>
            /// その他、一般的な例外：ElseException
            /// </summary>
            ElseException
        }

        #endregion
    }
}
