'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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

' System.Web
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Touryo.Infrastructure.CustomControl
	''' <summary>チェック結果格納クラス</summary>
	Public Class CheckResult
		''' <summary>コントロール名</summary>
		Private _ctrlName As String = ""

		''' <summary>コントロール名</summary>
		Public Property CtrlName() As String
			Get
				Return Me._ctrlName
			End Get

			Private Set
				Me._ctrlName = value
			End Set
		End Property

		''' <summary>チェックエラー情報名</summary>
		Private _checkErrorInfo As String() = Nothing

		''' <summary>チェックエラー情報名</summary>
		Public Property CheckErrorInfo() As String()
			Get
				Return Me._checkErrorInfo
			End Get

			Set
				Me._checkErrorInfo = value
			End Set
		End Property

		''' <summary>コンストラクタ</summary>
		''' <param name="ctrlName">コントロール名</param>
		Public Sub New(ctrlName As String)
			Me.CtrlName = ctrlName
		End Sub
	End Class
End Namespace
