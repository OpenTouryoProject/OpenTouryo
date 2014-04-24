'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'
'  
' 
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
'* クラス名        ：MyBaseAsyncFunc
'* クラス日本語名  ：非同期コード親クラス２（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2010/10/29  西野  大介        新規作成
'*  2010/12/06  西野  大介        スタート メソッドの追加
'*  2010/12/06  西野  大介        スレッド数管理と画面ロック、アンロック
'*  2011/02/27  西野  大介        上記処理をクリティカルセクションに格納
'**********************************************************************************

' System
Imports System
Imports System.Threading
Imports System.Diagnostics

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.RichClient.Asynchronous
	''' <summary>
	''' 非同期コード親クラス２
	''' </summary>
	Public Class MyBaseAsyncFunc
		Inherits BaseAsyncFunc
		''' <summary>ロック オブジェクト</summary>
		''' <remarks>クリティカルセクション化のため</remarks>
		Private Shared _lock As New Object()

		''' <summary>ログ出力の可否フラグ</summary>
		''' <remarks>
		''' 自動デプロイ環境ではログ出力が出来ない
		''' ことがあるので、その場合に利用すること。
		''' </remarks>
		Protected Shared CanOutPutLog As Boolean = True

		''' <summary>性能測定</summary>
		Private perfRec As PerformanceRecorder

		''' <summary>コンストラクタ</summary>
		''' <param name="_this">WPFやWinFormの要素</param>
		Public Sub New(_this As Object)
			MyBase.New(_this)
		End Sub

		''' <summary>開始処理</summary>
		Protected Overrides Sub UOC_Pre()
			' ACCESSログ出力 ----------------------------------------------

			If MyBaseAsyncFunc.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージID, エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & "－" & "," & "-----*" & "," & Convert.ToString(Me.UIElementName) & "," & Convert.ToString(Me.AsyncFunc.Method.Name)

				' Log4Netへログ出力
				LogIF.InfoLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------

			' 性能測定開始
			Me.perfRec = New PerformanceRecorder()
			Me.perfRec.StartsPerformanceRecord()
		End Sub

		''' <summary>終了処理</summary>
		Protected Overrides Sub UOC_After()
			' 性能測定終了
			Me.perfRec.EndsPerformanceRecord()

			' ACCESSログ出力 ----------------------------------------------

			If MyBaseAsyncFunc.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージID, エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & "－" & "," & "*-----" & "," & Convert.ToString(Me.UIElementName) & "," & Convert.ToString(Me.AsyncFunc.Method.Name) & "," & Convert.ToString(perfRec.ExecTime) & "," & Convert.ToString(perfRec.CpuTime)

				' Log4Netへログ出力
				LogIF.InfoLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------
		End Sub

		''' <summary>例外処理</summary>
		Protected Overrides Sub UOC_ABEND(ex As Exception)
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

			If MyBaseAsyncFunc.CanOutPutLog Then
				' ------------
				' メッセージ部
				' ------------
				' ユーザ名, レイヤ, 画面名, コントロール名,
				' 処理時間（実行時間）, 処理時間（CPU時間）
				' エラーメッセージ等
				' ------------
				Dim strLogMessage As String = "," & "－" & "," & "*-----" & "," & Convert.ToString(Me.UIElementName) & "," & Convert.ToString(Me.AsyncFunc.Method.Name) & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & ex.Message

				' Log4Netへログ出力
				LogIF.WarnLog("ACCESS", strLogMessage)
			End If

			' -------------------------------------------------------------    
		End Sub

		''' <summary>最終処理</summary>
		Protected Overrides Sub UOC_Finally()
			' ★ ここのクリティカルセクションで
			' ★ 同期呼び出し（Invoke）すると、
			' ★ デッドロックが発生するので注意する。
			SyncLock MyBaseAsyncFunc._lock
				' スレッド数デクリメント＆画面アンロック
				BaseAsyncFunc.ThreadCount -= 1
				Me.WindowUnlock()
			End SyncLock
		End Sub

		#Region "開始方法の既定"

		''' <summary>開始方法を規定する</summary>
		''' <returns>
		''' true：開始した
		''' false：開始できなかった
		''' </returns>
		Public Function Start() As Boolean
			SyncLock MyBaseAsyncFunc._lock
				' スレッド数の最大数（既定値は、１）
				Dim maxThreadCount As Integer = FxCmnFunction.GetNumFromConfig(FxLiteral.MAX_THREAD_COUNT, 1)

				If BaseAsyncFunc.ThreadCount >= maxThreadCount Then
					' 開始できなかった
					Return False
				End If

				' スレッド数インクリメント＆画面ロック
				BaseAsyncFunc.ThreadCount += 1
				Me.WindowLock()

				' 非同期実行（スレッド）
                Dim th As New Thread(AddressOf Me.CmnCallback)
				th.Start()

				' 開始した
				Return True
			End SyncLock
		End Function

		''' <summary>開始方法を規定する（スレッドプール）</summary>
		''' <returns>
		''' true：開始した
		''' false：開始できなかった
		''' </returns>
		Public Function StartByThreadPool() As Boolean
			SyncLock MyBaseAsyncFunc._lock
				' スレッド数の最大数（既定値は、１）
				Dim maxThreadCount As Integer = FxCmnFunction.GetNumFromConfig(FxLiteral.MAX_THREAD_COUNT, 1)

				If BaseAsyncFunc.ThreadCount >= maxThreadCount Then
					' 開始できなかった
					Return False
				End If

				' スレッド数インクリメント＆画面ロック
				BaseAsyncFunc.ThreadCount += 1
				Me.WindowLock()

				' 非同期実行（スレッドプール）
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Me.CmnCallbackP), Me)

				' 開始した
				Return True
			End SyncLock
		End Function

		#End Region
	End Class
End Namespace
