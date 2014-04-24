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
'* クラス名        ：WinCustomTextBoxDgvCol
'* クラス日本語名  ：DataGridViewのWinCustomTextBoxカラム（テンプレート）
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
	''' <summary>DataGridViewのWinCustomTextBoxカラム</summary>
	Public Class WinCustomTextBoxDgvCol
		Inherits DataGridViewColumn
		''' <summary>
		''' CellTemplateとするWinCustomTextBoxDgvCellを
		''' 指定して基本クラスのコンストラクタを呼び出す
		''' </summary>
		Public Sub New()
			MyBase.New(New WinCustomTextBoxDgvCell())
		End Sub

		''' <summary>MaxLengthプロパティの設定用</summary>
		Public Property MaxLength() As Integer
			Get
				Return m_MaxLength
			End Get
			Set
				m_MaxLength = Value
			End Set
		End Property
		Private m_MaxLength As Integer

		''' <summary>入力制限専用プロパティに適用する値</summary>
        <DefaultValue(False), Category("Edit"), Description("入力制限専用")> _
        Public Property IsNumeric() As Boolean
            Get
                Return m_IsNumeric
            End Get
            Set(ByVal value As Boolean)
                m_IsNumeric = value
            End Set
        End Property
		Private m_IsNumeric As Boolean

		''' <summary>初期値編集プロパティに適用する値</summary>
        <Category("Edit"), Description("初期値編集")> _
        Public Property EditInitialValue() As EditInitialValue
            Get
                Return m_EditInitialValue
            End Get
            Set(ByVal value As EditInitialValue)
                m_EditInitialValue = value
            End Set
        End Property
		Private m_EditInitialValue As EditInitialValue

        ''' <summary>文字埋め編集プロパティに適用する値</summary>
        <Category("Edit"), Description("文字埋め編集")> _
        Public Property EditPadding() As EditPadding
            Get
                Return m_EditPadding
            End Get
            Set(ByVal value As EditPadding)
                m_EditPadding = value
            End Set
        End Property
		Private m_EditPadding As EditPadding

        ''' <summary>小数点以下編集（入力中）プロパティに適用する値</summary>
        <Category("Edit"), Description("小数点以下ｘ桁編集（入力中）")> _
        Public Property EditDigitsAfterDP_Editing() As EditDigitsAfterDP
            Get
                Return m_EditDigitsAfterDP_Editing
            End Get
            Set(ByVal value As EditDigitsAfterDP)
                m_EditDigitsAfterDP_Editing = value
            End Set
        End Property
		Private m_EditDigitsAfterDP_Editing As EditDigitsAfterDP

		''' <summary>クローンの作製</summary>
		''' <returns>クローン</returns>
		Public Overrides Function Clone() As Object
			' base.Cloneの後に、
			Dim col As WinCustomTextBoxDgvCol = DirectCast(MyBase.Clone(), WinCustomTextBoxDgvCol)

			' 追加したプロパティをコピー
			' チェック系は不要、編集系を設定
			col.MaxLength = Me.MaxLength

			col.IsNumeric = Me.IsNumeric
			col.EditInitialValue = Me.EditInitialValue
			'col.EditAddFigure = this.EditAddFigure;
			col.EditPadding = Me.EditPadding
			'col.EditDigitsAfterDP = this.EditDigitsAfterDP;
			col.EditDigitsAfterDP_Editing = Me.EditDigitsAfterDP_Editing

			'col.DisplayUnits = this.DisplayUnits;

			Return col
		End Function

		''' <summary>CellTemplateの取得と設定</summary>
		Public Overrides Property CellTemplate() As DataGridViewCell
			' WinCustomTextBoxDgvCellしか
			' CellTemplateに設定できないようにする
			Get
				' 取得
				Return MyBase.CellTemplate
			End Get
			Set
				If Not (TypeOf value Is WinCustomTextBoxDgvCell) Then
					Throw New InvalidCastException("WinCustomTextBoxDgvCellオブジェクトを指定してください。")
				End If
				MyBase.CellTemplate = value
			End Set
		End Property
	End Class
End Namespace
