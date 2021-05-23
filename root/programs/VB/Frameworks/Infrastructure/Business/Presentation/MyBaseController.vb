'**********************************************************************************
'* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License. 
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：MyBaseController
'* クラス日本語名  ：画面コード親クラス２（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2009/04/21  西野 大介         FrameworkExceptionの追加に伴い、実装変更
'*  2009/06/02  西野 大介         sln - IR版からの修正
'*                                ・#5  ： コントロール数取得処理（デフォルト値不正）
'*                                ・#19 ： InnerException対策
'*                                ・#21 ： 不要なコードブロックの削除
'*                                ・#z  ： Finally節のUOCメソッドを追加した。
'*  2009/07/21  西野 大介         コントロール取得処理の仕様変更
'*  2009/07/31  西野 大介         エラー時に、セッションを開放しないで、
'*                                業務を続行可能にする処理を追加（不正操作エラー）
'*  2009/08/10  西野 大介         他の修正により、urlの引数がnullとなり得るので、修正。
'*  2009/09/01  西野 大介         サブシステム情報クラスの実装を追加した。
'*  2009/09/25  西野 大介         セッション ステートレス対応。
'*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
'*  2010/10/21  西野 大介         幾つかのイベント処理の正式対応（ベースクラス２→１へ）
'*  2010/11/21  西野 大介         集約イベント ハンドラをprotectedに変更（動的追加を考慮）
'*  2011/01/14  西野 大介         エラー時に、セッションを開放しないで、
'*                                業務を続行可能にする処理を追加（画面遷移制御チェック エラー）
'*  2012/04/05  西野 大介         \n → \r\n 化
'*  2012/06/14  西野 大介         コントロール検索の再帰処理性能の集約＆効率化。
'*  2012/06/18  西野 大介         OriginalStackTrace（ログ出力）の品質向上
'*  2013/01/18  西野 大介         public static TransferErrorScreen2追加（他から呼出可能に）
'*  2013/01/18  西野 大介         public static GetUserInfo2追加（他から呼出可能に）
'*  2017/02/14  西野 大介         キャッシュ無効化処理にスイッチを追加した。
'*  2017/02/28  西野 大介         ExceptionDispatchInfoを取り入れ、OriginalStackTraceを削除
'*  2017/02/28  西野 大介         TransferErrorScreen2のErrorMessage生成処理の見直し。
'*  2016/03/03  Supragyan         Resolved the URL issue of error screen transition path (merge)
'*  2016/03/03  Supragyan         Modified default relative path of the sample application screens (merge)
'*  2017/02/28  西野 大介         エラーログの見直し（その他の例外の場合、ex.ToString()を出力）
'*  2018/07/19  西野 大介         復元後のユーザー情報をSessionに設定するコードを追加
'*  2021/05/23  西野 大介         キャッシュ制御ヘッダの二重追加エラーの対応
'**********************************************************************************

Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Presentation
    ''' <summary>画面コード親クラス２</summary>
    ''' <remarks>（オーバーライドして）自由に利用できる。</remarks>
    Public MustInherit Class MyBaseController
        Inherits BaseController
        ''' <summary>ユーザ情報</summary>
        ''' <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        Protected UserInfo As MyUserInfo

        ' 2009/09/01-start
        ''' <summary>サブシステム情報</summary>
        ''' <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        Protected SubsysInfo As MySubsysInfo
        ' 2009/09/01-end

        ''' <summary>性能測定</summary>
        Private perfRec As PerformanceRecorder

#Region "全画面共通の処理"

#Region "Ｐ層イベント追加（不要であれば削除すること）"
        ' Ｐ層フレームワークのイベント処理機能へ
        ' コントロール イベントを追加するコード

#Region "コントロールの検索、取得、イベントハンドラの設定処理"

        ''' <summary>イベント追加処理</summary>
        Private Sub addControlEvent()
            ' 2009/07/21-start

            '#Region "コントロール取得処理"

            '#Region "旧処理"
            '/ CHECK BOX
            'MyCmnFunction.GetCtrlAndSetClickEventHandler(
            '    this, GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX),
            '    new System.EventHandler(this.Check_CheckedChanged), this.ControlHt);
            '#End Region

            ' プレフィックス
            Dim prefix As String = ""
            ' プレフィックスとイベント ハンドラのディクショナリを生成
            Dim prefixAndEvtHndHt As New Dictionary(Of String, Object)()

            ' CHECK BOX
            prefix = GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX)
            If Not String.IsNullOrEmpty(prefix) Then
                prefixAndEvtHndHt.Add(prefix, New System.EventHandler(AddressOf Me.Check_CheckedChanged))
            End If

            ' コントロール検索＆イベントハンドラ設定
            MyCmnFunction.GetCtrlAndSetClickEventHandler2(Me, prefixAndEvtHndHt, Me.ControlHt)

            '#End Region

            ' 2009/07/21-end
        End Sub

#End Region

#Region "集約イベント ハンドラ"

        '// <summary>
        '// CheckBoxのCheckedChangedイベントに対応した集約イベント ハンドラ
        '// </summary>
        'protected void Check_CheckedChanged(object sender, System.EventArgs e)
        '{
        '    // イベント ハンドラの共通引数の作成
        '    FxEventArgs fxEventArgs = new FxEventArgs(
        '        ((System.Web.UI.Control)(sender)).ID,
        '        0, 0, "",
        '        this.GetMethodName(((System.Web.UI.Control)(sender)).ID, 
        '            MyLiteral.UOC_METHOD_FOOTER_CHECKED_CHANGED));

        '    // クリック イベント処理の共通メソッド
        '    this.CMN_Event_Handler(fxEventArgs);
        '}

#End Region

#End Region

#Region "ページロードの共通処理"

        ''' <summary>ページロードのUOCメソッド（共通：初回ロード）</summary>
        ''' <remarks>
        ''' 実装必須
        ''' 画面コード親クラス１から利用される派生の末端
        ''' </remarks>
        Protected Overrides Sub UOC_CMNFormInit()
            ' フォーム初期化（初回ロード）時に実行する処理を実装する
            ' TODO:

            ' フォーム初期化共通処理
            Me.CMN_FormInit("init")
        End Sub

        ''' <summary>ページロードのUOCメソッド（共通：ポストバック）</summary>
        ''' <remarks>
        ''' 実装必須
        ''' 画面コード親クラス１から利用される派生の末端
        ''' </remarks>
        Protected Overrides Sub UOC_CMNFormInit_PostBack()
            ' フォーム初期化（ポストバック）時に実行する処理を実装する
            ' TODO:

            ' イベントの判別
            If Me.IsClientCallback Then
                ' フォーム初期化共通処理
                Me.CMN_FormInit("postback(ClientCallback)")
            ElseIf Me.AjaxExtensionStatus = FxEnum.AjaxExtStat.IsAjaxExtension Then
                ' フォーム初期化共通処理
                Me.CMN_FormInit("postback(AjaxExtension)")
            Else
                ' フォーム初期化共通処理
                Me.CMN_FormInit("postback")
            End If
        End Sub

        ''' <summary>フォーム初期化共通処理</summary>
        ''' <param name="eventName">イベント名（init or postback）</param>
        Private Sub CMN_FormInit(eventName As String)
            ' イベント追加処理を呼び出す
            Me.addControlEvent()

            ' カスタム認証処理 --------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' 2009/09/01-start
            ' 認証ユーザ情報をメンバにロードする --------------------------
            Me.GetUserInfo()
            ' -------------------------------------------------------------

            ' サブシステム情報をメンバにロードする ------------------------
            Me.GetSubsysInfo()
            ' -------------------------------------------------------------
            ' 2009/09/01-end

            ' 権限チェック ------------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' 閉塞チェック ------------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' キャッシュ制御処理 ------------------------------------------
            Me.CacheControlWithSwitch()
            ' -------------------------------------------------------------

            ' ポストバック後の位置復元 ------------------------------------
            Page.MaintainScrollPositionOnPostBack = True
            ' -------------------------------------------------------------

            ' タイトル設定 ------------------------------------------------
            ' this.ContentPageFileNoExプロパティとタイトルを関係付けると良い

            Me.Page.Title = GetConfigParameter.GetConfigValue("BrowserTitle") & "（" & Me.ContentPageFileNoEx & "）"
            ' -------------------------------------------------------------

            ' ACCESSログ出力 ----------------------------------------------

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & UserInfo.UserName &
                "," & Request.UserHostAddress &
                "," & eventName &
                "," & Me.ContentPageFileNoEx

            ' Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------
        End Sub

        ' 2009/09/01 & 2009/09/25-start

        ''' <summary>ユーザ情報を取得する</summary>
        Private Sub GetUserInfo()
            ' メンバへセット
            Me.UserInfo = MyBaseController.GetUserInfo2()
        End Sub

        ''' <summary>ユーザ情報を取得する</summary>
        ''' <remarks>他から呼び出し可能に変更（static）</remarks>
        Public Shared Function GetUserInfo2() As MyUserInfo
            Dim userInfo As MyUserInfo = Nothing

            ' セッションステートレス対応
            ' SessionがOFFの場合
            If HttpContext.Current.Session Is Nothing Then
            Else
                ' 取得を試みる。
                userInfo = DirectCast(UserInfoHandle.GetUserInformation(), MyUserInfo)
                ' nullチェック
                If userInfo Is Nothing Then
                    ' nullの場合、仮の値を生成 / 設定する。
                    Dim userName As String = System.Threading.Thread.CurrentPrincipal.Identity.Name

                    If userName Is Nothing OrElse userName = "" Then
                        ' 未認証状態
                        userInfo = New MyUserInfo("未認証", HttpContext.Current.Request.UserHostAddress)
                    Else
                        ' 認証状態
                        userInfo = New MyUserInfo(userName, HttpContext.Current.Request.UserHostAddress)

                        ' 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                        ' ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
                        ' ・・・

                        ' 復元したユーザ情報をセット
                        UserInfoHandle.SetUserInformation(userInfo)
                    End If
                End If
            End If

            ' 値を戻す。
            Return userInfo
        End Function

        ''' <summary>サブシステム情報を取得する</summary>
        ''' <remarks>nullの場合は、新規生成してSetする。</remarks>
        Private Sub GetSubsysInfo()
            Dim subsysInfo As SubsysInfo

            ' セッションステートレス対応
            ' SessionがOFFの場合
            If HttpContext.Current.Session Is Nothing Then
            Else
                ' 取得を試みる。
                subsysInfo = SubsysInfoHandle.GetSubsysInformation()

                ' nullチェック
                If subsysInfo Is Nothing Then
                    ' nullの場合、新規生成してSetする。
                    Me.SubsysInfo = New MySubsysInfo()
                    SubsysInfoHandle.SetSubsysInformation(Me.SubsysInfo)
                Else
                    ' nullで無い場合、取得した値を設定する。
                    Me.SubsysInfo = DirectCast(subsysInfo, MySubsysInfo)
                End If
            End If
        End Sub

        ' 2009/09/01 & 2009/09/25-end

        ''' <summary>キャッシュ制御処理（スイッチ付き）</summary>
        Private Sub CacheControlWithSwitch()
            ' システムで固定に出来る場合は、ここでキャッシュ無効化する。
            ' また、ユーザープログラムのファイル・ダウンロード処理などで
            ' フレームワークの設定したキャッシュ制御を変更したい場合は、Response.Clearを実行して再設定する。

            ' 画面遷移方法の定義を取得
            Dim noCache As String = GetConfigParameter.GetConfigValue(MyLiteral.CACHE_CONTROL)

            ' デフォルト値対策：設定なし（null）の場合の扱いを決定
            If noCache Is Nothing Then
                ' OFF扱い
                noCache = FxLiteral.OFF
            End If

            If noCache.ToUpper() = FxLiteral.[ON] Then
                ' ON

                ' http - How to control web page caching, across all browsers? - Stack Overflow
                ' http://stackoverflow.com/questions/49547/how-to-control-web-page-caching-across-all-browsers

                ' Using ASP.NET:
                Me.Response.Headers.Remove("Cache-Control")
                Me.Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate")
                ' HTTP 1.1.
                Me.Response.Headers.Remove("Pragma")
                Me.Response.AppendHeader("Pragma", "no-cache")
                ' HTTP 1.0.
                ' Proxies.
                Me.Response.Headers.Remove("Expires")
                Me.Response.AppendHeader("Expires", "0")
            ElseIf noCache.ToUpper() = FxLiteral.OFF Then
                ' OFF
            Else
                ' パラメータ・エラー（書式不正）
                Throw New FrameworkException(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1(0), [String].Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1(1), MyLiteral.CACHE_CONTROL))
            End If
        End Sub

#End Region

#Region "Ｐ層フレームワークの共通処理"

#Region "開始 終了処理のUOCメソッド"

        ''' <summary>フレームワークの対象コントロールイベントの開始処理を実装</summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_PreAction(fxEventArgs As FxEventArgs)
            ' フレームワークの対象コントロールイベントの開始処理を実装する
            ' TODO:

            ' 認証ユーザ情報を取得する ------------------------------------
            Me.GetUserInfo()

            ' -------------------------------------------------------------

            ' 権限チェック ------------------------------------------------
            ' -------------------------------------------------------------

            ' 閉塞チェック ------------------------------------------------
            ' -------------------------------------------------------------

            ' ACCESSログ出力 ----------------------------------------------

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & UserInfo.UserName &
                "," & Request.UserHostAddress &
                "," & "----->" &
                "," & Me.ContentPageFileNoEx &
                "," & fxEventArgs.ButtonID

            ' Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------

            ' 性能測定開始
            Me.perfRec = New PerformanceRecorder()
            Me.perfRec.StartsPerformanceRecord()
        End Sub

        ''' <summary>フレームワークの対象コントロールイベントの終了処理を実装</summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_AfterAction(fxEventArgs As FxEventArgs)
            ' フレームワークの対象コントロールイベントの終了処理を実装する
            ' TODO:

            ' 性能測定終了
            Me.perfRec.EndsPerformanceRecord()

            ' 認証ユーザ情報を取得する ------------------------------------
            Me.GetUserInfo()

            ' ACCESSログ出力 ----------------------------------------------

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & UserInfo.UserName &
                "," & Request.UserHostAddress &
                "," & "<-----" &
                "," & Me.ContentPageFileNoEx &
                "," & fxEventArgs.ButtonID &
                "," & "" &
                "," & perfRec.ExecTime &
                "," & perfRec.CpuTime

            ' Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------
        End Sub

        ' #z-start

        ''' <summary>Finally節の処理を実装</summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_Finally(fxEventArgs As FxEventArgs)
            '/ Log4Netへログ出力
            'LogIF.InfoLog("ACCESS", "UOC_Finally:" + fxEventArgs.ButtonID);
        End Sub

        ' #z-end

#End Region

#Region "画面遷移のUOCメソッド"

        ''' <summary>ボタンのクリック・イベントの終了後の画面遷移処理を実装</summary>
        ''' <param name="url">画面遷移する場合のURL</param>
        ''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_Screen_Transition(url As String)

            If url Is Nothing OrElse url = "" Then
                ' urlが空の場合、どこにも遷移せず、ポストバックとなる。
                ' 2009/08/10-この行
            Else
                ' urlが空でない場合、画面遷移する。

                If MyBaseController.TransitionMethod = FxLiteral.OFF Then
                    ' 画面遷移方法を取得（テストプログラム用パラメータ）
                    Dim screenTransitionMethod As String = GetConfigParameter.GetConfigValue("ScreenTransitionMethod")

                    If screenTransitionMethod = "1" Then
                        ' フレームワーク管理下の画面遷移（Transfer）
                        Me.FxTransfer(url)
                    ElseIf screenTransitionMethod = "2" Then
                        ' フレームワーク管理下の画面遷移（Redirect）
                        Me.FxRedirect(url)
                        ' パラメータ指定ミス
                    Else
                    End If
                Else
                    ' 画面遷移制御部品を使用して画面遷移
                    Me.ScreenTransition(url)
                End If
            End If
        End Sub

#End Region

#Region "例外処理のUOCメソッド"

        ''' <summary>業務例外発生時の処理を実装</summary>
        ''' <param name="baEx">BusinessApplicationException</param>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_ABEND(baEx As BusinessApplicationException, fxEventArgs As FxEventArgs)
            ' 業務例外発生時の処理を実装
            ' TODO:

            ' ここに、メッセージの組み立てロジックを実装する。

            ' メッセージ編集処理 ------------------------------------------

            Dim messageID As String = baEx.messageID
            Dim messageDescription As String = ""

            ' メッセージIDから、対応するメッセージを取得する。
            messageDescription = GetMessage.GetMessageDescription(messageID)
            If messageDescription = "" Then
                ' メッセージが取得できなかった場合
                messageDescription = baEx.Message
            Else
                ' メッセージが取得できた場合、
                ' 必要なら、メッセージに、可変文字列を組み込む。

                ' 方式は、プロジェクト毎に検討のこと。
                messageDescription = messageDescription.Replace("%1", baEx.Message)
                messageDescription = messageDescription.Replace("%2", baEx.Information)
            End If

            ' -------------------------------------------------------------

            ' メッセージ表示処理 ------------------------------------------

            '#Region "メッセージボックスを使用して表示する場合。"

            ' 「OK」メッセージダイアログの表示処理
            Me.ShowOKMessageDialog(messageID, messageDescription, FxEnum.IconType.Exclamation, "BusinessApplicationExceptionを使用したダイアログ表示")

            '#End Region

            '#Region "マスタ ページ上のラベルに表示する場合。"

            '/ 結果表示するメッセージ エリア
            'Label label = (Label)this.GetMasterWebControl("Label1");
            'label.Text = "";

            '/ 結果（業務続行可能なエラー）
            'label.Text = "ErrorMessageID:" + baEx.messageID + "\r\n";
            'label.Text += "ErrorMessage:" + baEx.Message + "\r\n";
            'label.Text += "ErrorInfo:" + baEx.Information + "\r\n";

            '#End Region

            ' -------------------------------------------------------------

            ' 性能測定終了

            ' イベント処理開始前にエラーが発生した場合は、
            ' this.perfRecがnullの場合があるので、null対策コードを挿入する。
            If Me.perfRec Is Nothing Then
                ' nullの場合、新しいインスタンスを生成し、性能測定開始。
                Me.perfRec = New PerformanceRecorder()
                perfRec.StartsPerformanceRecord()
            End If

            Me.perfRec.EndsPerformanceRecord()

            ' 認証ユーザ情報を取得する ------------------------------------
            Me.GetUserInfo()

            ' ACCESSログ出力-----------------------------------------------

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & UserInfo.UserName &
                "," & Request.UserHostAddress &
                "," & "<-----" &
                "," & Me.ContentPageFileNoEx &
                "," & fxEventArgs.ButtonID &
                "," & "" &
                "," & Me.perfRec.ExecTime &
                "," & Me.perfRec.CpuTime &
                "," & baEx.messageID &
                "," & baEx.Message ' baEx

            ' Log4Netへログ出力
            LogIF.WarnLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------
        End Sub

        ''' <summary>システム例外発生時の処理を実装</summary>
        ''' <param name="bsEx">BusinessSystemException</param>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_ABEND(bsEx As BusinessSystemException, fxEventArgs As FxEventArgs)
            ' システム例外発生時の処理を実装
            ' TODO:

            ' 性能測定終了

            ' イベント処理開始前にエラーが発生した場合は、
            ' this.perfRecがnullの場合があるので、null対策コードを挿入する。
            If Me.perfRec Is Nothing Then
                ' nullの場合、新しいインスタンスを生成し、性能測定開始。
                Me.perfRec = New PerformanceRecorder()
                perfRec.StartsPerformanceRecord()
            End If

            Me.perfRec.EndsPerformanceRecord()

            ' 認証ユーザ情報を取得する ------------------------------------
            Me.GetUserInfo()

            ' ACCESSログ出力-----------------------------------------------

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & UserInfo.UserName &
                "," & Request.UserHostAddress &
                "," & "<-----" &
                "," & Me.ContentPageFileNoEx &
                "," & fxEventArgs.ButtonID &
                "," & "" &
                "," & Me.perfRec.ExecTime &
                "," & Me.perfRec.CpuTime &
                "," & bsEx.messageID &
                "," & bsEx.Message & vbCr & vbLf &
                bsEx.StackTrace ' bsEX

            ' Log4Netへログ出力
            LogIF.ErrorLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------

            ' エラー画面に画面遷移する ------------------------------------
            Me.TransferErrorScreen(bsEx)
            ' -------------------------------------------------------------
        End Sub

        ''' <summary>一般的な例外発生時の処理を実装</summary>
        ''' <param name="ex">例外オブジェクト</param>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_ABEND(ex As Exception, fxEventArgs As FxEventArgs)
            ' 一般的な例外発生時の処理を実装
            ' TODO:

            ' 性能測定終了

            ' イベント処理開始前にエラーが発生した場合は、
            ' this.perfRecがnullの場合があるので、null対策コードを挿入する。
            If Me.perfRec Is Nothing Then
                ' nullの場合、新しいインスタンスを生成し、性能測定開始。
                Me.perfRec = New PerformanceRecorder()
                perfRec.StartsPerformanceRecord()
            End If

            Me.perfRec.EndsPerformanceRecord()

            ' 認証ユーザ情報を取得する ------------------------------------
            Me.GetUserInfo()

            ' ACCESSログ出力-----------------------------------------------

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & UserInfo.UserName &
                "," & Request.UserHostAddress &
                "," & "<-----" &
                "," & Me.ContentPageFileNoEx &
                "," & fxEventArgs.ButtonID &
                "," & "" &
                "," & Me.perfRec.ExecTime &
                "," & Me.perfRec.CpuTime &
                "," & "other Exception" &
                "," & ex.Message & vbCr & vbLf &
                ex.ToString() ' ex

            ' Log4Netへログ出力
            LogIF.ErrorLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------

            ' エラー画面に画面遷移する ------------------------------------
            Me.TransferErrorScreen(ex)
            ' -------------------------------------------------------------
        End Sub

#End Region

#Region "例外発生時に、エラー画面に画面遷移"

        ''' <summary>例外発生時に、エラー画面に画面遷移</summary>
        ''' <param name="ex">例外オブジェクト</param>
        ''' <remarks>Global_asaxから移植</remarks>
        Private Sub TransferErrorScreen(ex As Exception)
            If Me.AjaxExtensionStatus = FxEnum.AjaxExtStat.IsAjaxExtension Then
                ' Ajax Extensionの場合は、エラー画面を戻さないこと！
                Throw ex
            ElseIf Me.IsClientCallback Then
                ' Client Callbackの場合は、エラー画面を戻さないこと！
                Throw ex
            Else
                MyBaseController.TransferErrorScreen2(ex)
            End If
        End Sub

        ''' <summary>例外発生時に、エラー画面に遷移</summary>
        ''' <param name="ex">例外オブジェクト</param>
        ''' <remarks>他から呼び出し可能に変更（static）</remarks>
        Public Shared Sub TransferErrorScreen2(ex As Exception)
            '#Region "例外型を判別しエラーメッセージIDを取得"

            ' エラーメッセージ
            Dim err_msg As String

            ' エラー情報をセッションから取得
            Dim err_info As String

            ' エラーのタイプ
            Dim arrErrType As String() = ex.[GetType]().ToString().Split("."c)
            Dim errType As String = arrErrType(arrErrType.Length - 1)

            ' エラーメッセージＩＤ
            Dim errMsgId As String = ""

            ' #21-start
            If errType = "BusinessSystemException" Then
                ' システム例外
                Dim bsEx As BusinessSystemException = DirectCast(ex, BusinessSystemException)
                errMsgId = bsEx.messageID
            ElseIf errType = "FrameworkException" Then
                ' フレームワーク例外
                Dim fxEx As FrameworkException = DirectCast(ex, FrameworkException)
                errMsgId = fxEx.messageID
            Else
                ' それ以外の例外
                errMsgId = "－"
            End If
            ' #21-end

            '#End Region

            ' 2009/07/31-start

            '#Region "エラー時に、セッションを解放しないで、業務を続行可能にする処理を追加。"

            ' 不正操作エラー or 画面遷移制御チェック エラー
            If errMsgId = "IllegalOperationCheckError" OrElse errMsgId = "ScreenControlCheckError" Then
                ' セッションをクリアしない
                HttpContext.Current.Items.Add(FxHttpContextIndex.SESSION_ABANDON_FLAG, False)
            Else
                ' セッションをクリアする
                HttpContext.Current.Items.Add(FxHttpContextIndex.SESSION_ABANDON_FLAG, True)
            End If

            '#End Region

            ' 2009/07/31-end

            '#Region "エラー画面に表示するエラー情報を作成"

            err_msg = Environment.NewLine & "Error Message ID : " & errMsgId & Environment.NewLine & "Error Message : " & ex.Message.ToString()

            err_info = Environment.NewLine & "Current Request Url : " & HttpContext.Current.Request.Url.ToString() & Environment.NewLine & "Exception.StackTrace : " & ex.StackTrace & Environment.NewLine & "Exception.ToString() : " & ex.ToString()

            ' Form情報を出力するために、
            ' 遷移方法をServer.Transferに変更。
            ' また、情報受け渡しを、HttpContextに変更。
            HttpContext.Current.Items.Add(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE, err_msg)
            HttpContext.Current.Items.Add(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION, err_info)

            '#End Region

            ' エラー画面へのパスを取得 --- チェック不要（ベースクラスでチェック済み）
            Dim errorScreenPath As String = GetConfigParameter.GetConfigValue(FxLiteral.ERROR_SCREEN_PATH)

            ' Resolves the path of the url with respect to the server
            BaseController.ResolveServerUrl(errorScreenPath)

            ' エラー画面へ画面遷移
            HttpContext.Current.Server.Transfer(errorScreenPath)
        End Sub

#End Region

#End Region

#End Region

#Region "マスタ ページ上のフレームワーク対象コントロール（不要な場合は削除してく下さい）"

#Region "sampleScreen.masterマスタ ページ上のフレームワーク対象コントロールの、共通イベントのUOCメソッド"

        ''' <summary>
        ''' btnMButton101のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_sampleScreen_btnMButton101_Click(fxEventArgs As FxEventArgs) As String
            ' ログオフ（認証チケットを削除する）
            FormsAuthentication.SignOut()

            ' メッセージ表示
            Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
            label.Text = "ログアウトしました。"

            ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            Return "~/Aspx/start/menu.aspx"
        End Function

        ''' <summary>
        ''' btnMButton102のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_sampleScreen_btnMButton102_Click(fxEventArgs As FxEventArgs) As String
            ' ウィンドウの表示
            Me.ShowNormalScreen("~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx")

            ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "TestScreen1.masterマスタ ページ上のフレームワーク対象コントロールの、共通イベントのUOCメソッド"

#Region "基本処理"

        ''' <summary>
        ''' btnMButton1のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_btnMButton1_Click(fxEventArgs As FxEventArgs) As String
            ' メッセージ表示
            Me.ShowOKMessageDialog(fxEventArgs.ButtonID & "クリック イベント", Convert.ToString(fxEventArgs.MethodName) & "の実行", FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' lbnMLinkButton1のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_lbnMLinkButton1_Click(fxEventArgs As FxEventArgs) As String
            ' メッセージ表示
            Me.ShowOKMessageDialog(fxEventArgs.ButtonID & "クリック イベント", Convert.ToString(fxEventArgs.MethodName) & "の実行", FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' ibnMImageButton1のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_ibnMImageButton1_Click(fxEventArgs As FxEventArgs) As String
            ' メッセージ表示
            Me.ShowOKMessageDialog(fxEventArgs.ButtonID & "クリック イベント", Convert.ToString(fxEventArgs.MethodName) & "の実行 - " & "x:" & fxEventArgs.X.ToString() & ",y:" & fxEventArgs.Y.ToString(), FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' impMImageMap1のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_impMImageMap1_Click(fxEventArgs As FxEventArgs) As String
            ' メッセージ表示
            Me.ShowOKMessageDialog(fxEventArgs.ButtonID & "クリック イベント", Convert.ToString(fxEventArgs.MethodName) & "の実行 - " & "pbv:" & Convert.ToString(fxEventArgs.PostBackValue), FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "画面遷移処理"

        ''' <summary>
        ''' btnMButton2のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_btnMButton2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' lbnMLinkButton2のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_lbnMLinkButton2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' ibnMImageButton2のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_ibnMImageButton2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' impMImageMap2のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_impMImageMap2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

#End Region

#Region "コントロール取得"

        ''' <summary>
        ''' btnMButton3のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_btnMButton3_Click(fxEventArgs As FxEventArgs) As String
            ' コントロールを取得し
            Dim temp As Control = DirectCast(Me.GetFxWebControl(DirectCast(Me.GetMasterWebControl("TextBox1"), TextBox).Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' メッセージ表示
                Me.ShowOKMessageDialog("GetFxWebControl", "コントロールを取得できませんでした。", FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

        ''' <summary>
        ''' lbnMLinkButton3のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_lbnMLinkButton3_Click(fxEventArgs As FxEventArgs) As String
            ' コントロールを取得し
            Dim temp As Control = DirectCast(Me.GetMasterWebControl(DirectCast(Me.GetMasterWebControl("TextBox1"), TextBox).Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' メッセージ表示
                Me.ShowOKMessageDialog("GetMasterWebControl", "コントロールを取得できませんでした。", FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

        ''' <summary>
        ''' ibnMImageButton3のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_ibnMImageButton3_Click(fxEventArgs As FxEventArgs) As String
            ' コントロールを取得し
            Dim temp As Control = DirectCast(Me.GetContentWebControl(DirectCast(Me.GetMasterWebControl("TextBox1"), TextBox).Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' メッセージ表示
                Me.ShowOKMessageDialog("GetContentWebControl", "コントロールを取得できませんでした。", FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

#End Region

#Region "ダイアログ表示"

        ''' <summary>
        ''' btnMButton4のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_btnMButton4_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = DirectCast(Me.GetMasterWebControl("TextBox2"), TextBox).Text

            ' 受け渡しデータの設定
            Dim msg As String = DirectCast(Me.GetMasterWebControl("TextBox3"), TextBox).Text

            If DirectCast(Me.GetMasterWebControl("CheckBox1"), CheckBox).Checked = True Then
                ' スタイル指定あり
                Me.ShowOKMessageDialog("メッセージＩＤ", "メッセージ：" & msg, FxEnum.IconType.Information, "テスト", style)
            Else
                ' スタイル指定なし
                Me.ShowOKMessageDialog("メッセージＩＤ", "メッセージ：" & msg, FxEnum.IconType.Information, "テスト")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' lbnMLinkButton4のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen1_lbnMLinkButton4_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = DirectCast(Me.GetMasterWebControl("TextBox2"), TextBox).Text

            If DirectCast(Me.GetMasterWebControl("CheckBox1"), CheckBox).Checked = True Then
                ' スタイル指定あり
                Me.ShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx", style)
            Else
                ' スタイル指定なし
                Me.ShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx")
            End If

            Return ""
        End Function

#End Region

#End Region

#Region "TestScreen2.masterマスタ ページ上のフレームワーク対象コントロールの、共通イベントのUOCメソッド"

        ''' <summary>
        ''' btnMButton1のクリックイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_TestScreen2_btnMButton1_Click(fxEventArgs As FxEventArgs) As String
            ' 動作確認のため、３秒待つ。
            System.Threading.Thread.Sleep(3000)

            ' メッセージ表示
            Me.ShowOKMessageDialog(fxEventArgs.ButtonID & "クリック イベント", Convert.ToString(fxEventArgs.MethodName) & "の実行", FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#End Region
    End Class
End Namespace
