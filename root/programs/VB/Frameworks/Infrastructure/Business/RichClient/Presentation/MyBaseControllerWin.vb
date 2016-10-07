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
'* クラス名        ：MyBaseControllerWin
'* クラス日本語名  ：画面コード親クラス２（Windowアプリケーション）（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2012/06/14  西野  大介        コントロール検索の再帰処理性能の集約＆効率化。
'*  2012/06/18  西野  大介        OriginalStackTrace（ログ出力）の品質向上
'*  2012/09/19  西野  大介        UOC_CMNAfterFormInitの追加
'*  2013/03/05  西野  大介        UOC_CMNAfterFormInit、UOC_CMNAfterFormEndの呼出処理を追加
'*  2014/03/03  西野  大介        ユーザ コントロールのインスタンスの区別。
'**********************************************************************************

' System
Imports System
Imports System.Xml
Imports System.Data
Imports System.Collections
Imports System.Collections.Generic

' Windowアプリケーション
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

Imports Touryo.Infrastructure.Business.RichClient.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

Imports Touryo.Infrastructure.Framework.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Util

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util
Imports Touryo.Infrastructure.Public.WinProc

Namespace Touryo.Infrastructure.Business.RichClient.Presentation
	''' <summary>画面コード親クラス２（Windowアプリケーション）</summary>
	''' <remarks>（オーバーライドして）自由に利用できる。</remarks>
	Public Class MyBaseControllerWin
		Inherits BaseControllerWin
		' ↑abstractだとVSデザイナが機能しないので外した。

		''' <summary>ユーザ情報</summary>
		''' <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
		Protected Shared UserInfo As New MyUserInfo("－", Environment.MachineName)

		''' <summary>ログ出力の可否フラグ</summary>
		''' <remarks>
		''' 自動デプロイ環境ではログ出力が出来ない
		''' ことがあるので、その場合に利用すること。
		''' </remarks>
		Protected Shared CanOutPutLog As Boolean = True

		''' <summary>性能測定</summary>
		Private perfRec As PerformanceRecorder

		#Region "全画面共通の処理"

		#Region "Ｐ層イベント追加"
		'（不要であれば削除すること）

		' Ｐ層フレームワークのイベント処理機能へ
		' コントロール イベントを追加するコード

		#Region "コントロールの検索、取得、イベントハンドラの設定処理"

		''' <summary>イベント追加処理</summary>
		Private Sub addControlEvent()
			'#Region "Formイベント"

            ' Formイベント
            AddHandler Me.FormClosing, AddressOf Me.Form_CMNEventHandler

            ' FormのKeyイベント
            AddHandler Me.KeyDown, AddressOf Me.Form_KeyDownEventHandler
            AddHandler Me.KeyDown, AddressOf Me.Form_CMNEventHandler

            AddHandler Me.KeyPress, AddressOf Me.Form_KeyPressEventHandler
            AddHandler Me.KeyPress, AddressOf Me.Form_CMNEventHandler

            AddHandler Me.KeyUp, AddressOf Me.Form_KeyUpEventHandler
            AddHandler Me.KeyUp, AddressOf Me.Form_CMNEventHandler

			'#End Region

			'#Region "コントロール取得処理"

			'#Region "旧処理"
            '/ CHECK BOX
			'RcMyCmnFunction.GetCtrlAndSetClickEventHandler(
			'    this, GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX),
			'    new System.EventHandler(this.Check_CheckedChanged), this.ControlHt);

            '/// TOOL STRIP MENU ITEM（再起でファインドできないので各画面で設定）
            '/RcMyCmnFunction.GetCtrlAndSetClickEventHandler(
            '/    this, GetConfigParameter.GetConfigValue("FxPrefixOfToolStripMenuItem"),
            '/    new System.EventHandler(this.Button_Click), this.ControlHt);
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
			RcMyCmnFunction.GetCtrlAndSetClickEventHandler2(Me, prefixAndEvtHndHt, Me.ControlHt)

			'#End Region
		End Sub

		#End Region

		#Region "集約イベント ハンドラ"

        '// <summary>
        '// CheckBoxのCheckedChangedイベントに対応した集約イベント ハンドラ
        '// </summary>
		'protected void Check_CheckedChanged(object sender, System.EventArgs e)
		'{
        '    string name = ((Control)(sender)).Name;
        '    
        '    // イベント ハンドラの共通引数の作成
        '    RcFxEventArgs rcFxEventArgs
        '         = new RcFxEventArgs(name,
        '              this.GetMethodName((Control)(sender),
        '              MyLiteral.UOC_METHOD_FOOTER_CHECKED_CHANGED), sender, e);

        '    // イベント処理の共通メソッド
		'    this.CMN_Event_Handler(rcFxEventArgs);
		'}

		''' <summary>
		''' Item系のClickイベントに対応した集約イベント ハンドラ
		''' </summary>
        Protected Sub Item_Click(sender As Object, e As EventArgs)

            Dim name As String = DirectCast(sender, Control).Name

            ' イベント ハンドラの共通引数の作成
            Dim rcFxEventArgs As New RcFxEventArgs(name, _
                Me.GetMethodName(DirectCast(sender, Control), FxLiteral.UOC_METHOD_FOOTER_CLICK), _
                sender, e)

            ' イベント処理の共通メソッド
            Me.CMN_Event_Handler(rcFxEventArgs)
        End Sub

		#End Region

		#Region "Form系追加イベント"

		#Region "イベントの識別"

		''' <summary>イベントを識別するイベントID</summary>
		Private _eventID As String = ""

		''' <summary>イベントを識別する（KeyDown）</summary>
		Private Sub Form_KeyDownEventHandler(sender As Object, e As EventArgs)
			Me._eventID = "KeyDown"
		End Sub

		''' <summary>イベントを識別する（KeyPress）</summary>
		Private Sub Form_KeyPressEventHandler(sender As Object, e As EventArgs)
			Me._eventID = "KeyPress"
		End Sub

		''' <summary>イベントを識別する（KeyUp）</summary>
		Private Sub Form_KeyUpEventHandler(sender As Object, e As EventArgs)
			Me._eventID = "KeyUp"
		End Sub

		#End Region

		#Region "共通イベント ハンドラ"

		''' <summary>キーイベント</summary>
		Private Sub Form_CMNEventHandler(sender As Object, e As EventArgs)
			' メソッド名
			Dim methodName As String = "UOC_Form_"
			' イベント名
			Dim eventName As String = ""
            ' ((Control)sender).Name & "_";
			' イベントを識別

			If TypeOf e Is KeyEventArgs Then
				Dim temp As String = ""
				Dim kea As KeyEventArgs = DirectCast(e, KeyEventArgs)

				If Me._eventID = "KeyDown" Then
					' KeyDownイベント
					If kea.KeyCode = Keys.Enter Then
						temp = "Enter"
					ElseIf kea.KeyCode = Keys.F1 Then
						temp = "F1"
					ElseIf kea.KeyCode = Keys.F2 Then
						temp = "F2"
					ElseIf kea.KeyCode = Keys.F3 Then
						temp = "F3"
					ElseIf kea.KeyCode = Keys.F4 Then
						If kea.Alt Then
							temp = "AltAndF4"
						Else
							temp = "F4"
						End If
					ElseIf kea.KeyCode = Keys.F5 Then
						temp = "F5"
					ElseIf kea.KeyCode = Keys.F6 Then
						temp = "F6"
					ElseIf kea.KeyCode = Keys.F7 Then
						temp = "F7"
					ElseIf kea.KeyCode = Keys.F8 Then
						temp = "F8"
					ElseIf kea.KeyCode = Keys.F9 Then
						temp = "F9"
					ElseIf kea.KeyCode = Keys.F10 Then
						temp = "F10"
					ElseIf kea.KeyCode = Keys.F11 Then
						temp = "F11"
					ElseIf kea.KeyCode = Keys.F12 Then
						temp = "F12"
					End If
				ElseIf Me._eventID = "KeyPress" Then
				ElseIf Me._eventID = "KeyUp" Then
				End If

				' イベント名を指定
                eventName &= temp & "_" & Me._eventID
				' メソッド名を指定
                methodName &= temp & "_" & Me._eventID
			ElseIf TypeOf e Is FormClosingEventArgs Then
				' FormClosingイベント

				' イベント名を指定
                eventName &= "Closing"
				' メソッド名を指定
                methodName &= "Closing"
			End If
			'else if (e is xxx) { }

			' イベント実行
			If Latebind.CheckTypeOfMethodByName(Me, methodName) Then
				' イベント引数の作成
				Dim rcFxEventArgs As New RcFxEventArgs(eventName, methodName, sender, e)

				Try
					' 開始処理を実行する。
					Me.UOC_PreAction(rcFxEventArgs)

					' イベント処理を実行する。
					Latebind.InvokeMethod_NoErr(Me, methodName, New Object() {rcFxEventArgs})

					' 終了処理を実行する。
					Me.UOC_AfterAction(rcFxEventArgs)
				Catch baEx As BusinessApplicationException
					' アプリケーション例外発生時の処理を実行する。

						' アプリケーション例外はリスローしない。
					Me.UOC_ABEND(baEx, rcFxEventArgs)
				Catch bsEx As BusinessSystemException
					' システム例外発生時の処理を実行する。
					Me.UOC_ABEND(bsEx, rcFxEventArgs)

					' システム例外はリスローする。
					Throw
				Catch ex As Exception
					' 一般的な例外発生時の処理を実行する。
					Me.UOC_ABEND(ex, rcFxEventArgs)

					' 一般的な例外はリスローする。
					Throw
				Finally
					' Finally節のUOCメソッド 
					Me.UOC_Finally(rcFxEventArgs)
				End Try
			End If
		End Sub

		#End Region

		#End Region

		#End Region

		#Region "共通処理"

		#Region "フォーム ロードの共通処理"

		''' <summary>フォーム ロードのUOCメソッド（共通）</summary>
        ''' <remarks>
        ''' 実装必須
        ''' 画面コード親クラス１から利用される派生の末端
        ''' </remarks>
		Protected Overrides Sub UOC_CMNFormInit()
			' フォーム ロード時に実行する処理を実装する
			' TODO:

			' イベント追加処理を呼び出す
			Me.addControlEvent()

			Dim eventName As String = FxLiteral.EVENT_FORM_LOAD

			' フォーム初期化共通処理

			' 権限チェック ------------------------------------------------
			' -------------------------------------------------------------

			' 閉塞チェック ------------------------------------------------
			' -------------------------------------------------------------

			' タイトル設定 ------------------------------------------------
			' this.ContentPageFileNoExプロパティとタイトルを関係付けると良い
			Me.Text = GetConfigParameter.GetConfigValue("Title") & "（" & Convert.ToString(Me.Text) & "）"
			' -------------------------------------------------------------

			' ACCESSログ出力 ----------------------------------------------

			If MyBaseControllerWin.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名
				' ------------
				Dim strLogMessage As String = "," & Convert.ToString(UserInfo.UserName) & "," & "－" & "," & Convert.ToString(Me.Name) & "," & eventName

				' Log4Netへログ出力
				LogIF.InfoLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------
		End Sub

        ''' <summary>
        ''' フォーム ロードのUOCメソッド（個別）
        ''' </summary>
        ''' <remarks>
        ''' サブクラスで利用するのでここでは処理を実装しない。
        ''' </remarks>
        Protected Overrides Sub UOC_FormInit()
        End Sub

        ''' <summary>フォーム ロードのUOCメソッド（共通）</summary>
        ''' <remarks>
        ''' 実装必須
        ''' 画面コード親クラス１から利用される派生の末端
        ''' </remarks>
        Protected Overrides Sub UOC_CMNAfterFormInit()
            ' フォーム ロード時に実行する処理を実装する
            ' TODO:
        End Sub

		#End Region

		#Region "フォーム クローズドの共通処理"

		''' <summary>
		''' フォーム クローズドのUOCメソッド（個別）
		''' </summary>
		''' <remarks>
		''' サブクラスで利用するのでここでは処理を実装しない。
		''' </remarks>
		Protected Overrides Sub UOC_FormEnd()
		End Sub

		''' <summary>フォーム クローズドのUOCメソッド（共通）</summary>
		''' <remarks>
		''' 実装必須
		''' 画面コード親クラス１から利用される派生の末端
		''' </remarks>
		Protected Overrides Sub UOC_CMNFormEnd()
			' フォーム クローズドに実行する処理を実装する
			' TODO:

			Dim eventName As String = FxLiteral.EVENT_FORM_CLOSED

			' ACCESSログ出力 ----------------------------------------------

			If MyBaseControllerWin.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名
				' ------------
				Dim strLogMessage As String = "," & Convert.ToString(UserInfo.UserName) & "," & "－" & "," & Convert.ToString(Me.Name) & "," & eventName

				' Log4Netへログ出力
				LogIF.InfoLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------
		End Sub

		''' <summary>フォーム クローズドのUOCメソッド（共通）</summary>
		''' <remarks>
		''' 実装必須
		''' 画面コード親クラス１から利用される派生の末端
		''' </remarks>
		Protected Overrides Sub UOC_CMNAfterFormEnd()
			' フォーム ロード時に実行する処理を実装する
			' TODO:
		End Sub

		#End Region

		#Region "Ｐ層フレームワークの共通処理"

		#Region "開始 終了処理のUOCメソッド"

		''' <summary>フレームワークの対象コントロールイベントの開始処理を実装</summary>
		''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
		''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_PreAction(rcFxEventArgs As RcFxEventArgs)
			' フレームワークの対象コントロールイベントの開始処理を実装する
			' TODO:

			' 権限チェック ------------------------------------------------
			' -------------------------------------------------------------

			' 閉塞チェック ------------------------------------------------
			' -------------------------------------------------------------

			' ACCESSログ出力 ----------------------------------------------

			If MyBaseControllerWin.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージID, エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & Convert.ToString(UserInfo.UserName) & "," & "----->" & "," & Convert.ToString(Me.Name) & "," & Convert.ToString(rcFxEventArgs.ControlName)

				' Log4Netへログ出力
				LogIF.InfoLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------

			' 性能測定開始
			Me.perfRec = New PerformanceRecorder()
			Me.perfRec.StartsPerformanceRecord()
		End Sub

		''' <summary>フレームワークの対象コントロールイベントの終了処理を実装</summary>
		''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
		''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_AfterAction(rcFxEventArgs As RcFxEventArgs)
			' フレームワークの対象コントロールイベントの終了処理を実装する
			' TODO:

			' 性能測定終了
			Me.perfRec.EndsPerformanceRecord()

			' ACCESSログ出力 ----------------------------------------------

			If MyBaseControllerWin.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージID, エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & Convert.ToString(UserInfo.UserName) & "," & "<-----" & "," & Convert.ToString(Me.Name) & "," & Convert.ToString(rcFxEventArgs.ControlName) & "," & Convert.ToString(perfRec.ExecTime) & "," & Convert.ToString(perfRec.CpuTime)

				' Log4Netへログ出力
				LogIF.InfoLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------
		End Sub

		''' <summary>Finally節の処理を実装</summary>
		''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
		''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_Finally(rcFxEventArgs As RcFxEventArgs)
            '/ Log4Netへログ出力
            'LogIF.InfoLog("ACCESS", "UOC_Finally:" & rcFxEventArgs.ButtonID);

            '' 非同期呼び出しと併用不可能（Invokeも破棄するため）。
            '' 以下のメッセージ（一定？？確認が必要）を追加でDispatchするとControl.Invoke可能。
            'Dim fm As Integer(), dm As Integer()
            'PeekMessage.RemoveMessage(New Integer() {&HC25D, &HC27D}, fm, dm)

            '' フィルタされたメッセージ
            'System.Diagnostics.Debug.WriteLine("fm:")
            'For Each i As Integer In fm
            '    System.Diagnostics.Debug.WriteLine(i)
            'Next

            '' ディスパッチされたメッセージ
            'System.Diagnostics.Debug.WriteLine("dm:")
            'For Each i As Integer In dm
            '    System.Diagnostics.Debug.WriteLine(i)
            'Next
		End Sub

		#End Region

		#Region "例外処理のUOCメソッド"

		''' <summary>業務例外発生時の処理を実装</summary>
		''' <param name="baEx">BusinessApplicationException</param>
		''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
		''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_ABEND(baEx As BusinessApplicationException, rcFxEventArgs As RcFxEventArgs)
			' 業務例外発生時の処理を実装
			' TODO:

			' ここに、メッセージの組み立てロジックを実装する。

			' メッセージ編集処理 ------------------------------------------

			' -------------------------------------------------------------

			' メッセージ表示処理 ------------------------------------------

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

			' ACCESSログ出力-----------------------------------------------

			If MyBaseControllerWin.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージID, エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & Convert.ToString(UserInfo.UserName) & "," & "<-----" & "," & Convert.ToString(Me.Name) & "," & Convert.ToString(rcFxEventArgs.ControlName) & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & Convert.ToString(baEx.messageID) & "," & Convert.ToString(baEx.Message)

				' Log4Netへログ出力
				LogIF.WarnLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------            
		End Sub

		''' <summary>システム例外発生時の処理を実装</summary>
		''' <param name="bsEx">BusinessSystemException</param>
		''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
		''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_ABEND(bsEx As BusinessSystemException, rcFxEventArgs As RcFxEventArgs)
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

			' ACCESSログ出力-----------------------------------------------

			If MyBaseControllerWin.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージID, エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & Convert.ToString(UserInfo.UserName) & "," & "<-----" & "," & Convert.ToString(Me.Name) & "," & Convert.ToString(rcFxEventArgs.ControlName) & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & Convert.ToString(bsEx.messageID) & "," & Convert.ToString(bsEx.Message) & vbCr & vbLf & Convert.ToString(Me.OriginalStackTrace)
				' OriginalStackTrace（ログ出力）の品質向上
				If Me.OriginalStackTrace = "" Then
                    strLogMessage &= bsEx.StackTrace
				Else
                    strLogMessage &= Me.OriginalStackTrace
				End If

				' Log4Netへログ出力
				LogIF.ErrorLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------
		End Sub

		''' <summary>一般的な例外発生時の処理を実装</summary>
		''' <param name="ex">例外オブジェクト</param>
		''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
		''' <remarks>画面コード親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_ABEND(ex As Exception, rcFxEventArgs As RcFxEventArgs)
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

			' ACCESSログ出力-----------------------------------------------

			If MyBaseControllerWin.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージID, エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & Convert.ToString(UserInfo.UserName) & "," & "<-----" & "," & Convert.ToString(Me.Name) & "," & Convert.ToString(rcFxEventArgs.ControlName) & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & "other Exception" & "," & ex.Message & vbCr & vbLf & Convert.ToString(Me.OriginalStackTrace)
				' OriginalStackTrace（ログ出力）の品質向上
				If Me.OriginalStackTrace = "" Then
                    strLogMessage &= ex.StackTrace
				Else
                    strLogMessage &= Me.OriginalStackTrace
				End If

				' Log4Netへログ出力
				LogIF.ErrorLog("ACCESS", strLogMessage)
			End If
			' -------------------------------------------------------------
		End Sub

		#End Region

		#End Region

		#End Region

		#End Region
	End Class
End Namespace
