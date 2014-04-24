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
'* クラス名        ：Aspx_Common_testScreenCtrl
'* クラス日本語名  ：画面遷移制御機能テスト画面用のマスタ ページ
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Framework.Presentation

''' <summary>画面遷移制御機能テスト画面用のマスタ ページ</summary>
Public Partial Class Aspx_Common_testScreenCtrl
	Inherits BaseMasterController
	''' <summary>
	''' btnMButton1のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Public Function UOC_btnMButton1_Click(fxEventArgs As FxEventArgs) As String

		Return "WebForm0"
	End Function

	'---

	''' <summary>
	''' lbnMLinkButton1のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Public Function UOC_lbnMLinkButton1_Click(fxEventArgs As FxEventArgs) As String

		Return "WebForm3"
	End Function

	''' <summary>
	''' lbnMLinkButton2のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Public Function UOC_lbnMLinkButton2_Click(fxEventArgs As FxEventArgs) As String

		Return "WebForm1"
	End Function

	''' <summary>
	''' lbnMLinkButton3のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Public Function UOC_lbnMLinkButton3_Click(fxEventArgs As FxEventArgs) As String

		Return "WebForm2"
	End Function

	''' <summary>
	''' lbnMLinkButton4のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Public Function UOC_lbnMLinkButton4_Click(fxEventArgs As FxEventArgs) As String

		Return "WebForm4"
	End Function

	''' <summary>
	''' lbnMLinkButton5のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Public Function UOC_lbnMLinkButton5_Click(fxEventArgs As FxEventArgs) As String

		Return "WebForm5"
	End Function
End Class
