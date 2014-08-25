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
'* クラス名        ：WinCustomMaskedTextBoxDgvHost
'* クラス日本語名  ：マスク テキスト ボックス（Win）のカスタム・コントロールをDataGridViewでホストする（テンプレート）
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

Imports Touryo.Infrastructure.Public.Str

Namespace Touryo.Infrastructure.CustomControl.RichClient
	''' <summary>WinCustomMaskedTextBoxをDataGridViewでホストする。</summary>
	''' <remarks>IDataGridViewEditingControlを実装する。</remarks>
	Public Class WinCustomMaskedTextBoxDgvHost
		Inherits WinCustomMaskedTextBox
		Implements IDataGridViewEditingControl
		''' <summary>コンストラクタ</summary>
		Public Sub New()
			MyBase.New()
			Me.TabStop = False
		End Sub

		#Region "IDataGridViewEditingControl メンバ"

		'''' <summary>
		'''' マスクに一致しているか？していないか？
		'''' をチェックする際に利用するMaskedTextBox
		'''' </summary>
		'Private Shared _mtb As New MaskedTextBox()

		''' <summary>編集コントロールで変更されたセルの値</summary>
		Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object _
		    Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
            'return this.Text;// 編集処理がある場合工夫が必要
            'System.Diagnostics.Debug.WriteLine("GetEditingControlFormattedValue");
            'System.Diagnostics.Debug.WriteLine("・DataGridViewDataErrorContexts：" + context.ToString());
            'System.Diagnostics.Debug.WriteLine("・this.Text：" + this.Text);

			If context = (DataGridViewDataErrorContexts.Formatting Or DataGridViewDataErrorContexts.Display) Then
                ' 編集モードに入るとき

            ElseIf context = (DataGridViewDataErrorContexts.Parsing Or DataGridViewDataErrorContexts.Commit) Then
                ' マウスで抜けた場合

                ' 例外あり。
                If (Me.Edited) Then
                    ' 下端でEnterで抜けた場合
                    Me.PreValidate()
                    Me.ReEdit()
                End If

            ElseIf context = (DataGridViewDataErrorContexts.Parsing Or DataGridViewDataErrorContexts.Commit Or DataGridViewDataErrorContexts.CurrentCellChange) Then
                ' Tab、Enterで抜けた場合
                Me.PreValidate()
                Me.ReEdit()

            ElseIf context = (DataGridViewDataErrorContexts.Parsing Or DataGridViewDataErrorContexts.Commit Or DataGridViewDataErrorContexts.LeaveControl) Then
                ' 上端でShift + Tabで抜けた場合
                Me.PreValidate()
                Me.ReEdit()

            Else
                ' 不明
            End If

            Return Me.Text
        End Function

		''' <summary>編集コントロールで変更されたセルの値</summary>
		Public Property EditingControlFormattedValue() As Object _
		    Implements IDataGridViewEditingControl.EditingControlFormattedValue
			Get
				' GetEditingControlFormattedValueに任せる。
				Return Me.GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting)
			End Get
			Set
				Me.Text = DirectCast(value, String)
			End Set
		End Property

		''' <summary>セルスタイルを編集コントロールに適用する</summary>
		''' <param name="dataGridViewCellStyle">セルのスタイル</param>
		Public Sub ApplyCellStyleToEditingControl(dataGridViewCellStyle As DataGridViewCellStyle) _
		    Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl

			Me.Font = dataGridViewCellStyle.Font
			Me.ForeColor = dataGridViewCellStyle.ForeColor
			Me.BackColor = dataGridViewCellStyle.BackColor

			' dataGridViewCellStyle.Alignment → this.TextAlign
			Select Case dataGridViewCellStyle.Alignment
				Case DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.TopCenter
					Me.TextAlign = HorizontalAlignment.Center
					Exit Select

				Case DataGridViewContentAlignment.BottomRight, DataGridViewContentAlignment.MiddleRight, DataGridViewContentAlignment.TopRight
					Me.TextAlign = HorizontalAlignment.Right
					Exit Select
				Case Else

					Me.TextAlign = HorizontalAlignment.Left
					Exit Select
			End Select
		End Sub

		''' <summary>編集しているセルがあるDataGridView</summary>
		Public Property EditingControlDataGridView() As DataGridView _
            Implements IDataGridViewEditingControl.EditingControlDataGridView
			Get
				Return m_EditingControlDataGridView
			End Get
			Set
				m_EditingControlDataGridView = Value
			End Set
		End Property
		Private m_EditingControlDataGridView As DataGridView

		''' <summary>編集しているセルの行があるインデックス</summary>
		Public Property EditingControlRowIndex() As Integer _
            Implements IDataGridViewEditingControl.EditingControlRowIndex
			Get
				Return m_EditingControlRowIndex
			End Get
			Set
				m_EditingControlRowIndex = Value
			End Set
		End Property
		Private m_EditingControlRowIndex As Integer

		''' <summary>編集されたか（編集コントロールとセルの値が違うか）</summary>
		Public Property EditingControlValueChanged() As Boolean _
            Implements IDataGridViewEditingControl.EditingControlValueChanged
			Get
				Return m_EditingControlValueChanged
			End Get
			Set
				m_EditingControlValueChanged = Value
			End Set
		End Property
		Private m_EditingControlValueChanged As Boolean

		''' <summary>
		''' 指定されたキーが、編集コントロールによって処理される通常の入力キーか、
		''' DataGridView によって処理される特殊なキーであるかを確認します。
		''' </summary>
		''' <param name="keyData">入力キー</param>
		''' <param name="dataGridViewWantsInputKey">
		''' keyData に格納された Keys を、DataGridView に
		''' 処理させる場合は true。それ以外の場合は false。
		''' </param>
		''' <returns>
		''' true：編集コントロールに処理される入力キー
		''' false：それ以外の場合
		''' </returns>
		Public Function EditingControlWantsInputKey(keyData As Keys, dataGridViewWantsInputKey As Boolean) As Boolean _
            Implements IDataGridViewEditingControl.EditingControlWantsInputKey
			If dataGridViewWantsInputKey Then
				' DataGridView に処理される入力キー

				' Keys.Left、Right、Home、Endを
				' 編集コントロールに処理される通常の入力キーに加える。
				Select Case keyData And Keys.KeyCode
					Case Keys.Right, Keys.Left, Keys.Home, Keys.[End]
						'case Keys.Tab:
						'case Keys.Enter:
						Return True
					Case Else
						' 編集コントロールに処理
						Return False
					' DataGridView に処理
				End Select
			Else
				' DataGridView に処理されないキー
					' 編集コントロールに処理
				Return True
			End If
		End Function

		''' <summary>マウスカーソルがEditingPanel上にあるときのカーソル</summary>
		''' <remarks>EditingPanel：編集コントロールをホストするパネル</remarks>
		Public ReadOnly Property EditingPanelCursor() As Cursor _
            Implements IDataGridViewEditingControl.EditingPanelCursor
			Get
				' ベースに委譲
				Return MyBase.Cursor
			End Get
		End Property

		''' <summary>コントロールで編集する準備をする</summary>
		''' <param name="selectAll"></param>
		''' <remarks>テキストを選択状態にしたり、挿入ポインタを末尾にしたりする</remarks>
		Public Sub PrepareEditingControlForEdit(selectAll As Boolean) _
            Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
			' カスタム コントロールに実装
			' されているためコメントアウト

			'if (selectAll)
			'{
			'    //選択状態にする
			'    this.SelectAll();
			'}
			'else
			'{
			'    //挿入ポインタを末尾にする
			'    this.SelectionStart = this.TextLength;
			'}
		End Sub

		''' <summary>
		''' 値が変更した時に、セルの位置を変更するかどうか
		''' </summary>
		''' <remarks>
		''' true：大きさを変更する場合
		''' false：大きさを変更しない場合
		''' </remarks>
		Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean _
            Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
			Get
				' 大きさを変更しない
				Return False
			End Get
		End Property

		#End Region

		''' <summary>値が変更された時</summary>
		Protected Overrides Sub OnTextChanged(e As EventArgs)
			' ベースに委譲 
			MyBase.OnTextChanged(e)

			' 値が変更された
			Me.EditingControlValueChanged = True
			' ことをDataGridViewに通知する
			Me.EditingControlDataGridView.NotifyCurrentCellDirty(True)
		End Sub
	End Class
End Namespace
