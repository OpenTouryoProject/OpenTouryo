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
'* クラス名        ：WinCustomDropDownList
'* クラス日本語名  ：コンボ ボックス（Win）のカスタム・コントロール（テンプレート）
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

Imports Touryo.Infrastructure.Public.Str

Namespace Touryo.Infrastructure.CustomControl.RichClient
	''' <summary>System.Windows.Forms.ComboBoxのカスタム・コントロール</summary>
	''' <remarks>選択専用</remarks>
	<DefaultProperty("Items")> _
	Public Class WinCustomDropDownList
		Inherits ComboBox
		Implements IMasterData
		'''/ <summary>マスタデータ名</summary>
		Public _masterDataName As String = ""

		''' <summary>マスタデータ名</summary>
        <DefaultValue(""), Description("マスタデータ名")> _
        Public Property MasterDataName() As String Implements IMasterData.MasterDataName
            Get
                Return Me._masterDataName
            End Get
            Set(ByVal value As String)
                ' 全半角スペースは詰め、大文字に揃えて設定する。
                If Value Is Nothing Then
                    Me._masterDataName = Value
                Else
                    Me._masterDataName = Value.Replace("　", "").Replace(" ", "").ToUpper()
                End If
            End Set
        End Property

		#Region "初期処理"

		''' <summary>コンストラクタ</summary>
		Public Sub New()
			MyBase.New()
			Me.InitializeComponent()
		End Sub

		''' <summary>初期化</summary>
		Private Sub InitializeComponent()
			Me.SuspendLayout()
			' 
			' WinCustomComboBox
			' 
			Me.ResumeLayout(False)
		End Sub

		''' <summary>初期処理（Items）</summary>
		Public Sub InitItems()
			' マスタデータ設定
			If Me.Items.Count = 0 Then
				CmnMasterDatasForList.GetMasterData(Me.MasterDataName, Me.Items)
			End If
			' 初期値を設定
			If Me.Items.Count <> 0 Then
				Me.DropDownStyle = ComboBoxStyle.DropDownList
					' Itemsの場合有効
				Me.SelectedIndex = 0
			End If
		End Sub

		''' <summary>初期処理（DataSource）</summary>
		Public Sub InitDataSource()
			' マスタデータ設定
			If Me.Items.Count = 0 Then
				Me.DataSource = CmnMasterDatasForList.GetMasterData(Me.MasterDataName)
			End If
			' 初期値を設定
			If Me.Items.Count <> 0 Then
				Me.DropDownStyle = ComboBoxStyle.DropDownList
					' Itemsの場合有効
				Me.SelectedIndex = 0
			End If
		End Sub

        ' <summary>初期処理（Items）</summary>
        ' <remarks>
        ' コンストラクタのInitializeComponent前に実行する必要があるので、NG
        ' （コンストラクタでWS呼び出しを実装すると、デザイナが上手く表示できない。）
        ' </remarks>

		'protected override void InitLayout()
		'{
		'    base.InitLayout();

		'    // InitDataSourceは
		'    // SelectedIndex設定後の動きが微妙
		'    this.InitItems();
		'    //this.InitDataSource();

		'    // 初期値を設定
		'    if (this.Items.Count != 0)
		'    {
		'        this.DropDownStyle = ComboBoxStyle.DropDownList;
		'        this.SelectedIndex = 0;// Itemsの場合有効
		'    }
		'}

		#End Region
	End Class
End Namespace
