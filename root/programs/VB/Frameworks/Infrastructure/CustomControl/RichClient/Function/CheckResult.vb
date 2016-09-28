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
'* クラス名        ：CheckResult
'* クラス日本語名  ：チェック結果格納クラス（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'**********************************************************************************

' System
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text

' System.Windows
Imports System.Windows
Imports System.Windows.Forms

Namespace Touryo.Infrastructure.CustomControl.RichClient
	''' <summary>チェック結果格納クラス</summary>
	Public Class CheckResult
		''' <summary>コントロール名</summary>
		Public Property CtrlName() As String
			Get
				Return m_CtrlName
			End Get
			Private Set
				m_CtrlName = Value
			End Set
		End Property
		Private m_CtrlName As String
		''' <summary>チェックエラー情報名</summary>
		Public Property CheckErrorInfo() As String()
			Get
				Return m_CheckErrorInfo
			End Get
			Set
				m_CheckErrorInfo = Value
			End Set
		End Property
		Private m_CheckErrorInfo As String()

		''' <summary>コンストラクタ</summary>
		''' <param name="ctrlName">コントロール名</param>
		Public Sub New(ctrlName As String)
			Me.CtrlName = ctrlName
		End Sub
	End Class
End Namespace
