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
'* クラス名        ：ListItem
'* クラス日本語名  ：リスト用アイテム クラス（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'**********************************************************************************

Namespace Touryo.Infrastructure.CustomControl
	''' <summary>
	''' コンボボックスなどのリスト系コントロールに追加する項目クラス
	''' 識別子（ID）、名称（Name）、表示（ToString）のそれぞれを分けて取得できる。
	''' </summary>
	Public Class ListItem
		''' <summary>識別子</summary>
		Private _id As String = ""
		''' <summary>名称</summary>
		Private _name As String = ""

		''' <summary>コンストラクタ</summary>
		''' <param name="id">識別子</param>
		''' <param name="name">名称</param>
		Public Sub New(id As String, name As String)
			Me._id = id
			Me._name = name
		End Sub

		''' <summary>識別子</summary>
		Public ReadOnly Property Id() As String
			Get
				Return Me._id
			End Get
		End Property

		''' <summary>名称</summary>
		''' <remarks>
		''' ComboBoxで
		''' ・ .ValueMember = "ID";
		''' ・ .DisplayMember = "Name";
		''' などと設定する。
		''' </remarks>
		Public ReadOnly Property Name() As String
			Get
				Return Me._name
			End Get
		End Property

		''' <summary>表示名</summary>
		''' <remarks>
		''' ComboBoxで
		''' ・ .ValueMember = "ID";
		''' ・ .DisplayMember = "Name2";
		''' などと設定することで表示を変更可能。
		''' </remarks>
		Public ReadOnly Property Name2() As String
			Get
				Return Me._id & " : " & Me._name
			End Get
		End Property

		''' <summary>表示名</summary>
		''' <remarks>
		''' ComboBoxの
		''' ・ .ValueMember = "ID";
		''' ・ .DisplayMember = "Name";
		''' などと設定することで表示を変更可能。
		''' </remarks>
		Public Overrides Function ToString() As String
			Return Me._id & " : " & Me._name
		End Function
	End Class
End Namespace
