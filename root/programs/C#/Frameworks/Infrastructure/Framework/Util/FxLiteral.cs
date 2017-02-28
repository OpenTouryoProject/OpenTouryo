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
//* クラス名        ：FxLiteral
//* クラス日本語名  ：Framework層のリテラル クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/22  西野 大介         新規作成
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#15 ： XML要素のリテラル化
//*                                ・#x  ： クライアント証明書対応
//*                                ・#y  ： 汎用サービスインターフェイスの追加のため
//*  2009/07/21  西野 大介         コントロール取得処理の仕様変更
//*  2009/07/31  西野 大介         セッション情報の自動削除機能を追加
//*  2009/07/31  西野 大介         不正操作の検出機能を追加
//*  2009/09/15  西野 大介         Webサービス ブリッジ用サービス I/Fを追加
//*  2010/06/11  西野 大介         共有情報取得機能を追加
//*  2010/09/20  西野 大介         業務モードレス画面表示時のターゲット指定対応
//*  2010/09/24  西野 大介         ジェネリック対応（XMLのDictionary化）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/10/04  西野 大介         ボタン履歴情報記録機能のON / OFFスイッチ変更
//*  2010/10/21  西野 大介         幾つかのイベント処理の正式対応（ベースクラス２→１へ）
//*  2010/10/21  西野 大介         RepeaterコントロールのItemCommandイベント追加
//*  2011/11/20  西野 大介         リッチクライアント用P層フレームワークを追加
//*  2011/12/06  西野 大介         非同期処理スレッド数の最大値の定義を追加
//*  2011/01/18  西野 大介         GridViewコントロールのRowCommand、SelectedIndexChanged、
//*                                RowUpdating、RowDeleting、PageIndexChanging、Sortingイベントを追加する。
//*  2011/03/01  西野 大介         Formのキーイベント処理用のP層イベント処理の追加など
//*  2011/10/09  西野 大介         国際化対応
//*  2013/12/23  西野 大介         アクセス修飾子をすべてpublicに変更した。
//*  2014/02/03  西野 大介         国際化対応のスイッチ（app.config）を追加した。
//*  2014/08/18  Sai-San           Added constants for ListView events and prefix.
//*  2014/10/03  Rituparna         Added constants for ListView events and prefix for supporting ItemCommand event.
//*  2014/10/03  Rituparna         Added constants for RadioButtonList,CheckBoxList events and prefix. 
//*  2015/04/16  Supragyan         Added constants for Textbox events and prefix. 
//**********************************************************************************

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>Framework層のリテラル クラス</summary>
    public class FxLiteral
    {
        #region app.configのキー（とデフォルト値）

        #region P層

        #region スイッチ＆パラメタ

        /// <summary>セッションタイムアウト検出処理のON / OFFを設定するキー</summary>
        public const string SESSION_TIMEOUT_CHECK = "FxSessionTimeOutCheck";

        /// <summary>二重送信の検出処理のON / OFFを設定するキー</summary>
        public const string DOUBLE_TRANSMISSION_CHECK = "FxDoubleTransmissionCheck";

        /// <summary>不正操作の検出処理のON / OFFを設定するキー</summary>
        /// <remarks>リクエスト チケットGUIDのキュー長を管理する。</remarks>
        public const string REQUEST_TICKET_GUID_MAX_QUEUE_LENGTH = "FxRequestTicketGuidMaxQueueLength";

        ///// <summary>ボタン履歴情報記録機能のON / OFFを設定するキー</summary>
        //public const string BUTTON_HISTORY_RECORDER = "FxButtonHistoryRecorder"; // 下記のキュー長で制御

        /// <summary>ボタン履歴情報記録機能のキュー長を設定するキー</summary>
        public const string BUTTON_HISTORY_MAX_QUEUE_LENGTH = "FxButtonhistoryMaxQueueLength";

        /// <summary>デフォルトのボタン履歴情報記録機能のキュー長</summary>
        public const int BUTTON_HISTORY_DEFAULT_QUEUE_LENGTH = 50;

        /// <summary>親画面別セッション領域の自動削除機能のON / OFFを設定するキー</summary>
        /// <remarks>画面GUIDのキュー長を管理する。</remarks>
        public const string SCREEEN_GUID_MAX_QUEUE_LENGTH = "FxScreeenGuidMaxQueueLength";

        /// <summary>ウィンドウ別セッション領域の自動削除機能のON / OFFを設定するキー</summary>
        /// <remarks>ウィンドウGUIDのキュー長を管理する。</remarks>
        public const string WINDOW_GUID_MAX_QUEUE_LENGTH = "FxWindowGuidMaxQueueLength";

        /// <summary>画面遷移制御機能のモード（ T | R | OFF）を設定するキー</summary>
        /// <remarks>T : Transfer、R : Redirect</remarks>
        public const string SCREEN_TRANSITION_MODE = "FxScreenTransitionMode";

        /// <summary>画面遷移チェック機能のON / OFFを設定するキー</summary>
        public const string SCREEN_TRANSITION_CHECK = "FxScreenTransitionCheck";
        
        #endregion

        #region ダイアログ関係

        #region パス

        #region 画面

        /// <summary>OKメッセージ ダイアログへのパスを設定するキー</summary>
        public const string OK_MESSAGE_DIALOG_PATH = "FxOKMessageDialogPath";

        /// <summary>YES / NOメッセージ ダイアログへのパスを設定するキー</summary>
        public const string YES_NO_MESSAGE_DIALOG_PATH = "FxYesNoMessageDialogPath";

        /// <summary>ダイアログをロードするフレームへのパスを設定するキー</summary>
        public const string DIALOG_FRAME_PATH = "FxDialogFramePath";

        /// <summary>エラー画面へのパスを設定するキー</summary>
        public const string ERROR_SCREEN_PATH = "FxErrorScreenPath";

        #endregion

        #region アイコン

        /// <summary>情報を表すアイコン（ｉ）へのパスを設定するキー</summary>
        public const string INFORMATION_ICON_PATH = "FxInformationIconPath";

        /// <summary>警告時のアイコン（！）へのパスを設定するキー</summary>
        public const string WARNING_ICON_PATH = "FxWarningIconPath";

        /// <summary>エラー時のアイコン（×）へのパスを設定するキー</summary>
        public const string ERROR_ICON_PATH = "FxErrorIconPath";

        /// <summary>選択入力を促すアイコン（？）へのパスを設定するキー</summary>
        public const string QUESTION_ICON_PATH = "FxQuestionIconPath";

        #endregion

        #endregion

        #region スタイル

        /// <summary>ダイアログのスタイルを設定するキー</summary>
        public const string DEFAULT_FX_DIALOG_STYLE = "FxDefaultFxDialogStyle";

        /// <summary>業務ダイアログのスタイルを設定するキー</summary>
        public const string DEFAULT_BUSINESS_DIALOG_STYLE = "FxDefaultBusinessDialogStyle";

        /// <summary>業務モードレス画面のスタイルを設定するキー</summary>
        public const string DEFAULT_NORMAL_SCREEN_STYLE = "FxDefaultNormalScreenStyle";

        #endregion

        #endregion

        #endregion

        #region 各機能

        /// <summary>共有情報定義のXML</summary>
        public const string XML_SP_DEFINITION = "FxXMLSPDefinition";

        /// <summary>メッセージ定義のXML</summary>
        public const string XML_MSG_DEFINITION = "FxXMLMSGDefinition";

        /// <summary>画面遷移制御のXML</summary>
        public const string XML_SC_DEFINITION = "FxXMLSCDefinition";

        /// <summary>トランザクション制御のXML</summary>
        public const string XML_TC_DEFINITION = "FxXMLTCDefinition";

        /// <summary>呼び出しプロトコルの名前解決のXML</summary>
        public const string XML_TM_PROTOCOL_DEFINITION = "FxXMLTMProtocolDefinition";

        /// <summary>インプロセス呼び出しの名前解決のXML</summary>
        public const string XML_TM_INPROCESS_DEFINITION = "FxXMLTMInProcessDefinition";

        /// <summary>国際化対応のスイッチ（業務メッセージ）</summary>
        public const string BUSINESSMESSAGECULTUER = "FxBusinessMessageCulture";

        #endregion

        #region 非同期

        /// <summary>非同期処理スレッドの最大数</summary>
        public const string MAX_THREAD_COUNT = "FxMaxThreadCount";

        #endregion

        #endregion

        #region P層のリテラル

        #region Ctrlプレフィックス

        /// <summary>ボタンのプレフィックスを設定するキー</summary>
        public const string PREFIX_OF_BUTTON = "FxPrefixOfButton";

        /// <summary>リンク ボタンのプレフィックスを設定するキー</summary>
        public const string PREFIX_OF_LINK_BUTTON = "FxPrefixOfLinkButton";

        /// <summary>イメージ ボタンのプレフィックスを設定するキー</summary>
        public const string PREFIX_OF_IMAGE_BUTTON = "FxPrefixOfImageButton";

        /// <summary>イメージ マップのプレフィックスを設定するキー</summary>
        public const string PREFIX_OF_IMAGE_MAP = "FxPrefixOfImageMap";

        /// <summary>コマンドのプレフィックスを設定するキー</summary>
        public const string PREFIX_OF_COMMAND = "FxPrefixOfCommand";

        /// <summary>ドロップ ダウン リストのプレフィックスを設定するキー。</summary>
        public const string PREFIX_OF_DROP_DOWN_LIST = "FxPrefixOfDropDownList";

        /// <summary>リスト ボタンのプレフィックスを設定するキー。</summary>
        public const string PREFIX_OF_LIST_BOX = "FxPrefixOfListBox";

        /// <summary>ラジオ ボタンのプレフィックスを設定するキー。</summary>
        public const string PREFIX_OF_RADIO_BUTTON = "FxPrefixOfRadioButton";        

        /// <summary>リピータのプレフィックスを設定するキー。</summary>
        public const string PREFIX_OF_REPEATER = "FxPrefixOfRepeater";

        /// <summary>グリッド ビューのプレフィックスを設定するキー。</summary>
        public const string PREFIX_OF_GRIDVIEW = "FxPrefixOfGridView";

        /// <summary>Key to set the prefix of the List view.</summary>
        public const string PREFIX_OF_LISTVIEW = "FxPrefixOfListView";

        /// <summary>Key to set the prefix of the RadioButtonList.</summary>
        public const string PREFIX_OF_RADIOBUTTONLIST = "FxPrefixOfRadioButtonList";

        /// <summary>Key to set the prefix of the CheckboxList.</summary>
        public const string PREFIX_OF_CHECKBOXLIST = "FxPrefixOfCheckBoxList";

        /// <summary>Key to set the prefix of the Text box.</summary>
        public const string PREFIX_OF_TEXTBOX = "FxPrefixOfTextBox";

        #region Windows Forms

        /// <summary>ピクチャー ボックスのプレフィックスを設定するキー。</summary>
        public const string PREFIX_OF_PICTURE_BOX = "FxPrefixOfPictureBox";

        /// <summary>コンボ ボックスのプレフィックスを設定するキー。</summary>
        public const string PREFIX_OF_COMBO_BOX = "FxPrefixOfComboBox";       

        #endregion

        #region WPF / XBAP

        // ・・・

        #endregion

        #endregion

        #region イベントハンドラ関係

        #region イベントハンドラのヘッダ・フッタ

        /// <summary>UOCメソッドヘッダ</summary>
        public const string UOC_METHOD_HEADER = "UOC_";

        /// <summary>UOCメソッドフッタ（Click）</summary>
        public const string UOC_METHOD_FOOTER_CLICK = "Click";

        /// <summary>UOCメソッドフッタ（SelectedIndexChanged）</summary>
        public const string UOC_METHOD_FOOTER_SELECTED_INDEX_CHANGED = "SelectedIndexChanged";

        /// <summary>UOCメソッドフッタ（CheckedChanged）</summary>
        public const string UOC_METHOD_FOOTER_CHECKED_CHANGED = "CheckedChanged";

        #region 一覧系

        #region 基本

        /// <summary>UOCメソッドフッタ（ItemCommand）</summary>
        public const string UOC_METHOD_FOOTER_ITEM_COMMAND = "ItemCommand";

        #endregion

        #region 高機能

        /// <summary>UOCメソッドフッタ（PageIndexChanging）</summary>
        public const string UOC_METHOD_FOOTER_PAGE_INDEX_CHANGING = "PageIndexChanging";

        /// <summary>UOCメソッドフッタ（Sorting）</summary>
        public const string UOC_METHOD_FOOTER_SORTING = "Sorting";

        #endregion

        #region GridView

        /// <summary>UOCメソッドフッタ（RowCommand）</summary>
        public const string UOC_METHOD_FOOTER_ROW_COMMAND = "RowCommand";

        /// <summary>UOCメソッドフッタ（RowUpdating）</summary>
        public const string UOC_METHOD_FOOTER_ROW_UPDATING = "RowUpdating";

        /// <summary>UOCメソッドフッタ（RowDeleting）</summary>
        public const string UOC_METHOD_FOOTER_ROW_DELETING = "RowDeleting";

        #endregion

        #region ListView

        /// <summary>UOCメソッドフッタ（RowUpdating）</summary>
        public const string UOC_METHOD_FOOTER_LISTVIEW_ROW_UPDATING = "ItemUpdating";

        /// <summary>UOCメソッドフッタ（RowDeleting）</summary>
        public const string UOC_METHOD_FOOTER_LISTVIEW_ROW_DELETING = "ItemDeleting";

        /// <summary>UOCメソッドフッタ（RowCommand）</summary>
        public const string UOC_METHOD_FOOTER_LISTVIEW_ROW_ITEMCOMMAND = "OnItemCommand";

        /// <summary>UOCメソッドフッタ（PagePropertiesChanged）</summary>
        public const string UOC_METHOD_FOOTER_LISTVIEW_PAGE_PROPERTIES_CHANGED = "PagePropertiesChanged";

        /// <summary>UOCメソッドフッタ（Sorting）</summary>
        public const string UOC_METHOD_FOOTER_LISTVIEW_ROW_SORTING = "Sorting";

        #endregion

        #region Textbox

        /// <summary>UOCメソッドフッタ（Text Changed）</summary>
        public const string UOC_METHOD_FOOTER_TEXT_CHANGED = "TextChanged";

        #endregion

        #endregion       

        #region Windows Forms

        /// <summary>UOCメソッドフッタ（KeyDown）</summary>
        public const string UOC_METHOD_FOOTER_KEY_DOWN = "KeyDown";

        /// <summary>UOCメソッドフッタ（KeyPress）</summary>
        public const string UOC_METHOD_FOOTER_KEY_PRESS = "KeyPress";

        /// <summary>UOCメソッドフッタ（KeyUp）</summary>
        public const string UOC_METHOD_FOOTER_KEY_UP = "KeyUp";

        #endregion

        #region WPF / XBAP

        // ・・・

        #endregion

        #endregion

        #region イベント引数の替え玉

        /// <summary>ページ ロード イベント</summary>
        public const string EVENT_PAGE_LOAD = "Page_Load";

        /// <summary>YES / NOメッセージダイアログの[×]ボタンの後処理イベント</summary>
        public const string EVENT_AFTER_YES_NO_X = "AfterYesNo_X";

        /// <summary>YES / NOメッセージダイアログの[YES]ボタンの後処理イベント</summary>
        public const string EVENT_AFTER_YES_NO_YES = "AfterYesNo_Yes";

        /// <summary>YES / NOメッセージダイアログの[NO]ボタンの後処理イベント</summary>
        public const string EVENT_AFTER_YES_NO_NO = "AfterYesNo_No";

        /// <summary>モーダル ダイアログの後処理イベント</summary>
        public const string EVENT_AFTER_MODAL_DIALOG = "AfterModalDialog";

        #region RichClient

        /// <summary>フォーム ロード イベント</summary>
        /// <remarks>Windows Formsで追加</remarks>
        public const string EVENT_FORM_LOAD = "Form_Load";

        /// <summary>フォーム クローズド イベント</summary>
        /// <remarks>Windows Formsで追加</remarks>
        public const string EVENT_FORM_CLOSED = "Form_Closed";

        /// <summary>ウィンドウ ローデッド イベント</summary>
        /// <remarks>WPF / XBAPで追加</remarks>
        public const string EVENT_WINDOW_LOADED = "Window_Loaded";

        /// <summary>ウィンドウ アンローデッド イベント</summary>
        /// <remarks>WPF / XBAPで追加</remarks>
        public const string EVENT_WINDOW_UNLOADED = "Window_Unloaded";

        /// <summary>ページ ローデッド イベント</summary>
        /// <remarks>WPF / XBAPで追加</remarks>
        public const string EVENT_PAGE_LOADED = "Page_Loaded";

        /// <summary>ページ アンローデッド イベント</summary>
        /// <remarks>WPF / XBAPで追加</remarks>
        public const string EVENT_PAGE_UNLOADED = "Page_Unloaded";

        #endregion

        #endregion

        #endregion

        #region HIDDENタグのリテラル

        /// <summary>WindowGuid</summary>
        public const string HIDDEN_WINDOW_GUID = "WindowGuid";

        /// <summary>ChildScreenType</summary>
        public const string HIDDEN_CHILD_SCREEN_TYPE = "ChildScreenType";

        /// <summary>ChildScreenUrl</summary>
        public const string HIDDEN_CHILD_SCREEN_URL = "ChildScreenUrl";

        /// <summary>CloseFlag</summary>
        public const string HIDDEN_CLOSE_FLAG = "CloseFlag";

        /// <summary>SubmitFlag</summary>
        public const string HIDDEN_SUBMIT_FLAG = "SubmitFlag";

        /// <summary>FxDialogStyle</summary>
        public const string HIDDEN_FX_DIALOG_STYLE = "FxDialogStyle";

        /// <summary>BusinessDialogStyle</summary>
        public const string HIDDEN_BUSINESS_DIALOG_STYLE = "BusinessDialogStyle";

        /// <summary>NormalScreenStyle</summary>
        public const string HIDDEN_NORMAL_SCREEN_STYLE = "NormalScreenStyle";

        /// <summary>NormalScreenTarget</summary>
        public const string HIDDEN_NORMAL_SCREEN_TARGET = "NormalScreenTarget";

        /// <summary>DialogFrameUrl</summary>
        public const string HIDDEN_DIALOG_FRAME_URL = "DialogFrameUrl";

        /// <summary>ScreenGuid</summary>
        public const string HIDDEN_SCREEN_GUID = "ScreenGuid";

        /// <summary>RequestTicketGuid</summary>
        public const string HIDDEN_REQUEST_TICKET_GUID = "RequestTicketGuid"; // 2009/07/31-この行

        #endregion

        /// <summary>クライアントコールバックを示すRequest.Formパラメタのキー</summary>
        public const string CALLBACK_ID = "__CALLBACKID";

        #endregion

        #region 通信制御のリテラル

        #region S-I/F

        #region シグネチャ(ビルド時利用のためconstである必要がある)

        /// <summary>Webサービスの名前空間</summary>
        public const string WS_NAME_SPACE = "http://tempuri.org/";

        /// <summary>WebサービスのWebメソッド名（.NET オンライン）</summary>
        public const string WS_METHOD_NAME_DOTNET_ONLINE = "DotNETOnlineWS";

        /// <summary>WebサービスのWebメソッド説明（.NET オンライン）</summary>
        public const string WS_METHOD_DESCRIPTION_DOTNET_ONLINE
            = "Service interface base for .NET on-line which uses an ASP.NET Web Service. ";

        /// <summary>WebサービスのWebメソッド説明（汎用）</summary>
        public const string WS_METHOD_DESCRIPTION_MULTI_USE
            = "In general service interface base which uses an ASP.NET Web Service. ";

        /// <summary>WebサービスのWebメソッド説明（汎用）</summary>
        public const string WS_METHOD_DESCRIPTION_WEB_SERVICE_BRIDGE
            = "Service interface base for Web Service bridges which uses an ASP.NET Web Service. ";

        #endregion

        #region 実装

        /// <summary>サービス インターフェイスの状態（開始）</summary>
        public const string SIF_STATUS_START = "Processing start. "; //"処理開始";

        /// <summary>サービス インターフェイスの状態（認証処理）</summary>
        public const string SIF_STATUS_AUTHENTICATION = "Attestation processing. "; //"認証処理";

        /// <summary>サービス インターフェイスの状態（名前解決）</summary>
        public const string SIF_STATUS_NAME_SERVICE = "Name resolution. "; //"名前解決";

        /// <summary>サービス インターフェイスの状態（デシリアライズ処理）</summary>
        public const string SIF_STATUS_DESERIALIZE = "Formation of a .NET object(Deserialization). "; //".NETオブジェクト化";

        /// <summary>サービス インターフェイスの状態（Ｂ層・Ｄ層呼出し）</summary>
        public const string SIF_STATUS_INVOKE = "B layer and D layer call. "; //"Ｂ層・Ｄ層呼出し";

        /// <summary>サービス インターフェイスの状態（シリアライズ処理）</summary>
        public const string SIF_STATUS_SERIALIZE = "Formation of a Base64 character string(Serialization). "; //"Base64文字列化";

        /// <summary>サービス インターフェイスの状態（正常終了）</summary>
        public const string SIF_STATUS_N_END = "Normal end. "; //"正常終了";

        /// <summary>サービス インターフェイスの状態（異常終了）</summary>
        public const string SIF_STATUS_A_END = "Abnormal end. "; //"異常終了";

        /// <summary>インプロセス呼び出しで呼ぶメソッド名</summary>
        public const string TRANSMISSION_INPROCESS_METHOD_NAME = "DoBusinessLogic";

        #endregion

        #endregion

        #region WS Proxy

        /// <summary>サーバ：クライアント認証のセキュリティ資格情報：ユーザ名</summary>
        /// <remarks>基本認証、ダイジェスト認証、NTLM 認証、Kerberos 認証</remarks>
        public const string TRANSMISSION_HTTP_PROP_USER_NAME = "USERNAME";
        /// <summary>サーバ：クライアント認証のセキュリティ資格情報：パスワード</summary>
        /// <remarks>基本認証、ダイジェスト認証、NTLM 認証、Kerberos 認証</remarks>
        public const string TRANSMISSION_HTTP_PROP_PASSWORD = "PASSWORD";
        /// <summary>サーバ：クライアント認証のセキュリティ資格情報：ドメイン（PC名）</summary>
        /// <remarks>基本認証、ダイジェスト認証、NTLM 認証、Kerberos 認証</remarks>
        public const string TRANSMISSION_HTTP_PROP_DOMAIN = "DOMAIN";

        /// <summary>プロキシ経由の要求を行うためのプロキシ情報：URL</summary>
        public const string TRANSMISSION_HTTP_PROP_PROXY_URL = "PROXYURL";

        /// <summary>プロキシ：クライアント認証のセキュリティ資格情報：ユーザ名</summary>
        /// <remarks>基本認証、ダイジェスト認証、NTLM 認証、Kerberos 認証</remarks>
        public const string TRANSMISSION_HTTP_PROP_PROXY_USER_NAME = "PUSERNAME";
        /// <summary>プロキシ：クライアント認証のセキュリティ資格情報：パスワード</summary>
        /// <remarks>基本認証、ダイジェスト認証、NTLM 認証、Kerberos 認証</remarks>
        public const string TRANSMISSION_HTTP_PROP_PROXY_PASSWORD = "PPASSWORD";
        /// <summary>プロキシ：クライアント認証のセキュリティ資格情報：ドメイン（PC名）</summary>
        /// <remarks>基本認証、ダイジェスト認証、NTLM 認証、Kerberos 認証</remarks>
        public const string TRANSMISSION_HTTP_PROP_PROXY_DOMAIN = "PDOMAIN";

        /// <summary>クライアント証明書へのパス</summary>
        public const string TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE = "CERTFILE";

        /// <summary>クライアント証明書ＤＢのパスワード</summary>
        public const string TRANSMISSION_HTTP_PROP_X509CERTIFICATE_PASSWORD = "CERTPASSWORD";

        /// <summary>HTTP圧縮の有効・無効</summary>
        public const string TRANSMISSION_HTTP_PROP_ENABLEDE_COMPRESSION = "COMPRESSION";

        /// <summary>要求と共に送信されるユーザ エージェント ヘッダの値</summary>
        public const string TRANSMISSION_HTTP_PROP_USER_AGENT = "USERAGENT";

        /// <summary>接続グループ</summary>
        public const string TRANSMISSION_HTTP_PROP_CONNECTION_GROUP_NAME = "CONNGROUPNAME";

        #endregion

        #region WCF

        /// <summary>WCF HTTPのコントラクト名</summary>
        public const string SERVICE_CONTRACT_ATTRIBUTE_CONFIGURATION_NAME = "Transmission.IWCFHTTPSvcForFx";

        /// <summary>WCF HTTPのEndPointConfigName</summary>
        public const string WCF_HTTP_ENDPOINT_CONFIGNAME = "Transmission.WCFHTTPSvcForFx";

        /// <summary>WCF TCP/IPのEndPointConfigName</summary>
        public const string WCF_TCPIP_ENDPOINT_CONFIGNAME = "Touryo.Infrastructure.Business.Transmission.WCFTCPSvcForFx";

        #endregion

        #endregion

        #region 設定のリテラル

        #region 汎用定義

        /// <summary>ON / OFFのON</summary>
        public const string ON = "ON";

        /// <summary>ON / OFFのOFF</summary>
        public const string OFF = "OFF";

        /// <summary>拒否/許可の拒否</summary>
        public const string DENY = "DENY";

        /// <summary>拒否/許可の許可</summary>
        public const string ALLOW = "ALLOW";

        #endregion

        #region 画面遷移関係

        /// <summary>HTTPメソッドのGET</summary>
        public const string GET = "GET";

        /// <summary>HTTPメソッドのPOST</summary>
        public const string POST = "POST";

        /// <summary>画面遷移メソッド：TRANSFER</summary>
        public const string TRANSFER = "T";

        /// <summary>画面遷移メソッド：REDIRECT</summary>
        public const string REDIRECT = "R";

        #endregion

        #region トランザクション制御

        /// <summary>分離レベル（接続しない）</summary>
        public const string ISO_LEVEL_NOT_CONNECT = "NC";

        /// <summary>分離レベル（ノートランザクション）</summary>
        public const string ISO_LEVEL_NO_TRANSACTION = "NT";

        /// <summary>分離レベル（ダーティーリード）</summary>
        public const string ISO_LEVEL_READ_UNCOMMITTED = "UC";

        /// <summary>分離レベル（リードコミット）</summary>
        public const string ISO_LEVEL_READ_COMMIT = "RC";

        /// <summary>分離レベル（リピータブルリード）</summary>
        public const string ISO_LEVEL_REPEATABLE_READ = "RR";

        /// <summary>分離レベル（リシアライザブル）</summary>
        public const string ISO_LEVEL_SERIALIZABLE = "SZ";

        /// <summary>分離レベル（スナップショット）</summary>
        public const string ISO_LEVEL_SNAPSHOT = "SS";

        /// <summary>分離レベル（デフォルトの分離レベル）</summary>
        public const string ISO_LEVEL_DEFAULT = "DF";

        #endregion

        #endregion

        #region 値文字列

        /// <summary>Boolean：true</summary>
        public const string VALUE_STR_TRUE = "TRUE";

        /// <summary>Boolean：false</summary>
        public const string VALUE_STR_FALSE = "FALSE";

        /// <summary>null</summary>
        public const string VALUE_STR_NULL = "NULL";

        /// <summary>DUMMY文字列</summary>
        public const string VALUE_STR_DUMMY_STRING = "dummy";

        #endregion

        #region XML定義のリテラル

        #region タグ

        #region 画面遷移定義

        /// <summary>CmnTransitionタグ</summary>
        public const string XML_SC_TAG_CMN_TRANSITION = "CmnTransition";

        /// <summary>Screenタグ</summary>
        public const string XML_SC_TAG_SCREEN = "Screen";

        /// <summary>Transitionタグ</summary>
        public const string XML_SC_TAG_TRANSITION = "Transition";

        #endregion

        #region トランザクション定義

        /// <summary>TransactionPatternタグ</summary>
        public const string XML_TX_TAG_TRANSACTION_PATTERN = "TransactionPattern";

        /// <summary>TransactionGroupタグ</summary>
        public const string XML_TX_TAG_TRANSACTION_GROUP = "TransactionGroup";

        #endregion

        #region 名前解決定義

        /// <summary>Transmissionタグ</summary>
        public const string XML_TM_TAG_TRANSMISSION = "Transmission";

        /// <summary>Urlタグ</summary>
        public const string XML_TM_PROTOCOL_TAG_URL = "Url";

        /// <summary>Propタグ</summary>
        public const string XML_TM_PROTOCOL_TAG_PROP = "Prop";

        #endregion

        #region メッセージ、共有情報定義

        /// <summary>Messageタグ</summary>
        public const string XML_MSG_TAG_MESSAGE = "Message";

        /// <summary>SharedPropタグ</summary>
        public const string XML_SP_TAG_SHARED_PROPERTY = "SharedProp";

        #endregion

        #endregion

        #region 属性

        #region 汎用属性

        /// <summary>汎用属性（id）</summary>
        public const string XML_CMN_ATTR_ID = "id";

        /// <summary>汎用属性（key）</summary>
        public const string XML_CMN_ATTR_KEY = "key";

        /// <summary>汎用属性（value）</summary>
        public const string XML_CMN_ATTR_VALUE = "value";

        #endregion

        #region 画面遷移定義

        /// <summary>画面遷移ラベル</summary>
        public const string XML_SC_ATTR_LABEL = "label";

        /// <summary>画面遷移モード</summary>
        public const string XML_SC_ATTR_MODE = "mode";

        /// <summary>直リン可否</summary>
        public const string XML_SC_ATTR_DIRECTLINK = "directLink";

        #endregion

        #region トランザクション定義

        /// <summary>接続文字列</summary>
        public const string XML_TX_ATTR_CONNKEY = "connkey";

        /// <summary>分離レベル</summary>
        public const string XML_TX_ATTR_ISOLEVEL = "isolevel";

        #endregion

        #region 名前解決定義

        /// <summary>プロトコル</summary>
        public const string XML_TM_PROTOCOL_ATTR_PROTOCOL = "protocol";

        /// <summary>ＵＲＬ</summary>
        public const string XML_TM_PROTOCOL_ATTR_URL = "url";

        /// <summary>ＵＲＬ（IDREF）</summary>
        public const string XML_TM_PROTOCOL_ATTR_URL_REF = "url_ref";

        /// <summary>タイムアウト</summary>
        public const string XML_TM_PROTOCOL_ATTR_TIMEOUT = "timeout";

        /// <summary>プロパティ（IDREF）</summary>
        public const string XML_TM_PROTOCOL_ATTR_PROP_REF = "prop_ref";

        /// <summary>アセンブリ名</summary>
        public const string XML_TM_INPROCESS_ATTR_ASSEMBLYNAME = "assemblyName";

        /// <summary>クラス名</summary>
        public const string XML_TM_INPROCESS_ATTR_CLASSNAME = "className";

        #endregion

        #region メッセージ、共有情報定義

        /// <summary>メッセージ内容</summary>
        public const string XML_MSG_ATTR_DESCRIPTION = "description";

        #endregion

        #endregion

        #endregion
    }
}
