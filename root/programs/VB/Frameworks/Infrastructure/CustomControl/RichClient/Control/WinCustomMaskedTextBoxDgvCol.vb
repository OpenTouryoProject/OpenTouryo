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
'* クラス名        ：WinCustomMaskedTextBoxDgvCol
'* クラス日本語名  ：DataGridViewのWinCustomMaskedTextBoxカラム（テンプレート）
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
	''' <summary>DataGridViewのWinCustomMaskedTextBoxカラム</summary>
	Public Class WinCustomMaskedTextBoxDgvCol
		Inherits DataGridViewColumn
		''' <summary>
		''' CellTemplateとするDataGridViewMaskedTextBoxCellを
		''' 指定して基本クラスのコンストラクタを呼び出す
		''' </summary>
		Public Sub New()
			MyBase.New(New WinCustomMaskedTextBoxDgvCell())
		End Sub

		''' <summary>初期値編集プロパティに適用する値</summary>
        <Category("Edit"), Description("初期値編集")> _
        Public Property EditInitialValue() As EditInitialValue
            Get
                Return m_EditInitialValue
            End Get
            Set(ByVal value As EditInitialValue)
                m_EditInitialValue = Value
            End Set
        End Property
		Private m_EditInitialValue As EditInitialValue

		''' <summary>Maskプロパティに適用する値</summary>
        <Category("Edit"), Description("入力後のマスク")> _
        Public Property Mask() As String
            Get
                Return m_Mask
            End Get
            Set(ByVal value As String)
                m_Mask = Value
            End Set
        End Property
		Private m_Mask As String

		''' <summary>Mask_Editingプロパティに適用する値</summary>
        <Category("Edit"), Description("入力中のマスク")> _
        Public Property Mask_Editing() As String
            Get
                Return m_Mask_Editing
            End Get
            Set(ByVal value As String)
                m_Mask_Editing = value
            End Set
        End Property
		Private m_Mask_Editing As String

		''' <summary>半角指定（マスクで指定できないため）</summary>
        <DefaultValue(False), Category("Edit"), Description("半角指定（マスクで指定できないため）")> _
        Public Property EditToHankaku() As Boolean
            Get
                Return m_EditToHankaku
            End Get
            Set(ByVal value As Boolean)
                m_EditToHankaku = value
            End Set
        End Property
		Private m_EditToHankaku As Boolean

		''' <summary>YYYYMMDDのM、Dが１桁の時に補正処理を行う。</summary>
        <DefaultValue(False), Category("Edit"), Description("YYYYMMDDのM、Dが１桁の時に補正処理を行う。")> _
        Public Property EditToYYYYMMDD() As Boolean
            Get
                Return m_EditToYYYYMMDD
            End Get
            Set(ByVal value As Boolean)
                m_EditToYYYYMMDD = value
            End Set
        End Property
		Private m_EditToYYYYMMDD As Boolean

		''' <summary>クローンの作製</summary>
		''' <returns>クローン</returns>
		Public Overrides Function Clone() As Object
			' base.Cloneの後に、
			Dim col As WinCustomMaskedTextBoxDgvCol = DirectCast(MyBase.Clone(), WinCustomMaskedTextBoxDgvCol)

			' 追加したプロパティをコピー
			' チェック系は不要、編集系を設定
			col.EditInitialValue = Me.EditInitialValue

			col.Mask = Me.Mask
			col.Mask_Editing = Me.Mask_Editing

			col.EditToHankaku = Me.EditToHankaku
			col.EditToYYYYMMDD = Me.EditToYYYYMMDD

			Return col
		End Function

		''' <summary>CellTemplateの取得と設定</summary>
		Public Overrides Property CellTemplate() As DataGridViewCell
			' DataGridViewMaskedTextBoxCellしか
			' CellTemplateに設定できないようにする
			Get
				' 取得
				Return MyBase.CellTemplate
			End Get
			Set
				If Not (TypeOf value Is WinCustomMaskedTextBoxDgvCell) Then
					Throw New InvalidCastException("WinCustomMaskedTextBoxDgvCellオブジェクトを指定してください。")
				End If
				MyBase.CellTemplate = value
			End Set
		End Property
	End Class
End Namespace
