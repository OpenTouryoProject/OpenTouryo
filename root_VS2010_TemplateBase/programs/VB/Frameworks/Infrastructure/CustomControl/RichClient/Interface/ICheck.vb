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
'* クラス名        ：ICheck
'* クラス日本語名  ：チェック処理に関するプロパティ グリッドのインターフェイス（テンプレート）
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
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text

' System.Windows
Imports System.Windows
Imports System.Windows.Forms

Namespace Touryo.Infrastructure.CustomControl.RichClient
	''' <summary>チェック処理に関するプロパティ グリッドのインターフェイス（テンプレート）</summary>
	Public Interface ICheck
		''' <summary>入力文字種チェック</summary>
		<Category("Check"), Description("入力文字種チェック")> _
		Property CheckType() As CheckType

		''' <summary>入力文字種チェックのデフォルト</summary>
		''' <returns>
		''' デフォルト以外：true
		''' デフォルト：false
		''' </returns>
		Function ShouldSerializeCheckType() As Boolean

		''' <summary>正規表現チェック</summary>
		<DefaultValue(""), Category("Check"), Description("正規表現チェック")> _
		Property CheckRegExp() As String

		''' <summary>正規表現チェック</summary>
		<DefaultValue(False), Category("Check"), Description("禁則文字チェック")> _
		Property CheckProhibitedChar() As Boolean

		''' <summary>チェック処理</summary>
		''' <returns>
		''' ・エラーなし：true
		''' ・エラーあり：false
		''' </returns>
		Function Validate() As Boolean

		''' <summary>チェック処理</summary>
		''' <param name="result">結果文字列</param>
		''' <returns>
		''' ・エラーなし：true
		''' ・エラーあり：false
		''' </returns>
		Function Validate(ByRef result As String()) As Boolean
	End Interface
End Namespace
