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
'* クラス名        ：WinCustomTextBox
'* クラス日本語名  ：テキスト ボックス（Win）のカスタム・コントロール（テンプレート）
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
Imports System.Windows.Forms.Design

Imports System.Diagnostics
Imports System.Globalization

Imports Touryo.Infrastructure.Framework.RichClient.Util
Imports Touryo.Infrastructure.Public.Str

Namespace Touryo.Infrastructure.CustomControl.RichClient
    ''' <summary>System.Windows.Forms.TextBoxのカスタム・コントロール</summary>
    <DefaultProperty("Text")> _
    <Designer(GetType(WinCustomTextBox.WinCustomTextBoxDesigner))> _
    Public Class WinCustomTextBox
        Inherits TextBox
        Implements ICheck, IEdit, IGetValue, INotifyPropertyChanged
        '''<summary>デザイナ上の表示をカスタマイズするインナークラス</summary>
        '''<remarks>
        '''自作コントロールにおいて、不要なプロパティをデザイナANDインテリセンスで隠したい時
        '''http://jehupc.exblog.jp/8157762/
        '''継承と属性プログラミングで実現するRAD開発 － ＠IT
        '''http://www.atmarkit.co.jp/fdotnet/winexp/winexp02/winexp02_03.html
        '''</remarks>
        Friend Class WinCustomTextBoxDesigner
            Inherits ControlDesigner
            '''<summary>下記で指定してあるプロパティはデザイナでは非表示とする。</summary>
            Protected Overrides Sub PostFilterProperties(ByVal Properties As IDictionary)
                Properties.Remove("Text2")
                Properties.Remove("Text3")
                Properties.Remove("Value")
            End Sub
        End Class

#Region "初期処理"

        ''' <summary>コンストラクタ</summary>
        Public Sub New()
            Me.InitializeComponent()
        End Sub

        ''' <summary>初期化</summary>
        Private Sub InitializeComponent()
            Me.SuspendLayout()
            ' 
            ' WinCustomTextBox
            ' 
            AddHandler Me.Layout, AddressOf Me.WinCustomTextBox_Layout
            'AddHandler Me.TextChanged, AddressOf Me.WinCustomTextBox_TextChanged

            AddHandler Me.Enter, AddressOf Me.WinCustomTextBox_Enter
            AddHandler Me.Validating, AddressOf Me.WinCustomTextBox_Validating
            AddHandler Me.Validated, AddressOf Me.WinCustomTextBox_Validated
            AddHandler Me.Leave, AddressOf Me.WinCustomTextBox_Leave

            AddHandler Me.KeyDown, AddressOf Me.WinCustomTextBox_KeyDown
            AddHandler Me.KeyUp, AddressOf Me.WinCustomTextBox_KeyUp

            AddHandler Me.MouseDown, AddressOf Me.WinCustomTextBox_MouseDown

            Me.ResumeLayout(False)

        End Sub

#End Region

#Region "プロパティ拡張"

#Region "変更イベントや、変更通知"

        ''' <summary>Valueプロパティの変更イベント</summary>
        Public Event ValueChanged As EventHandler

        ''' <summary>ValueChangedイベントを発生させる</summary>
        Protected Overridable Sub OnValueChanged(ByVal e As EventArgs)
            RaiseEvent ValueChanged(Me, e)
        End Sub

        ''' <summary>変更通知イベント（汎用）</summary>
        Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>変更通知イベント発生（汎用）</summary>
        ''' <param name="propertyName">プロパティ名</param>
        Protected Sub NotifyPropertyChanged(ByVal propertyName As [String])
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

#End Region

        ''' <summary>Textプロパティにテキスト変更時の編集機能を追加実装する。</summary>
        Public Overrides Property Text() As String

            Get
                Return MyBase.Text
            End Get

            Set(ByVal value As String)
                '/ これは、DataGridViewTextBoxCell.InitializeEditingControl経由のset_Textを無視するためのコード
                '/ 解り難いので、Debugログより前に処理する。
                'if (Environment.StackTrace.IndexOf("_83B12F0F_CEA3_4f93_9233_B86EFA149BB2") != -1)
                '{
                '    return;
                '}

                'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + value.ToString());
                'Debug.WriteLine(Environment.StackTrace);

                If Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") <> -1 Then
                    ' Binding（Target_Validate）から呼ばれた時は無視。
                    Return
                End If

                ' まず設定。
                MyBase.Text = value


                If Me.Focused Then
                    ' フォーカスがある場合は編集（ReEdite）しない。

                ElseIf Environment.StackTrace.IndexOf("System.Windows.Forms.DataGridView.InitializeEditingControlValue") <> -1 Then
                    ' DataGridView（InitializeEditingControlValue）から呼ばれた時は編集（ReEdite）しない。

                Else
                    ' 上記以外は編集（ReEdite）する。
                    Me.ReEdit()

                End If
            End Set

        End Property

        ''' <summary>Text2プロパティ</summary>
        <Category("表示"), Description("ユーザ入力のTextプロパティ")> _
        Public Property Text2() As String
            Get
                ' コピーを作成
                Dim wctxt As New WinCustomTextBox()

                ' 編集属性を指定
                wctxt.MaxLength = Me.MaxLength

                wctxt.IsNumeric = Me.IsNumeric
                wctxt.EditInitialValue = Me.EditInitialValue
                wctxt.EditAddFigure = Me.EditAddFigure
                wctxt.EditPadding = Me.EditPadding
                wctxt.EditDigitsAfterDP = Me.EditDigitsAfterDP
                wctxt.EditDigitsAfterDP_Editing = Me.EditDigitsAfterDP_Editing

                'wctxt.DisplayUnits = this.DisplayUnits;

                ' 編集して返す。
                wctxt.Text = MyBase.Text
                wctxt.Edit()
                Return wctxt.Text
            End Get

            Set(ByVal value As String)
                If Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") <> -1 Then
                    ' Binding（Target_Validate）から呼ばれた時は無視。
                    Return
                End If

                ' コピーを作成
                Dim wctxt As New WinCustomTextBox()

                ' 編集属性を指定
                wctxt.MaxLength = Me.MaxLength

                wctxt.IsNumeric = Me.IsNumeric
                wctxt.EditInitialValue = Me.EditInitialValue
                wctxt.EditAddFigure = Me.EditAddFigure
                wctxt.EditPadding = Me.EditPadding
                wctxt.EditDigitsAfterDP = Me.EditDigitsAfterDP
                wctxt.EditDigitsAfterDP_Editing = Me.EditDigitsAfterDP_Editing

                'wctxt.DisplayUnits = this.DisplayUnits;

                ' 編集してTextに設定する。
                wctxt.Text = value
                wctxt.Edit()
                MyBase.Text = wctxt.Text
            End Set
        End Property

        ''' <summary>Text3プロパティ</summary>
        <Category("表示"), Description("編集処理込のTextプロパティ")> _
        Public Property Text3() As String
            Get
                ' コピーを作成
                Dim wctxt As New WinCustomTextBox()

                ' 編集属性を指定
                wctxt.MaxLength = Me.MaxLength

                wctxt.IsNumeric = Me.IsNumeric
                wctxt.EditInitialValue = Me.EditInitialValue
                wctxt.EditAddFigure = Me.EditAddFigure
                wctxt.EditPadding = Me.EditPadding
                wctxt.EditDigitsAfterDP = Me.EditDigitsAfterDP
                wctxt.EditDigitsAfterDP_Editing = Me.EditDigitsAfterDP_Editing

                'wctxt.DisplayUnits = this.DisplayUnits;

                ' 編集して返す。
                wctxt.Text = MyBase.Text
                'wctxt.ReEdit(); // .TextでReEditされるので。
                Return wctxt.Text
            End Get

            Set(ByVal value As String)
                If Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") <> -1 Then
                    ' Binding（Target_Validate）から呼ばれた時は無視。
                    Return
                End If

                ' コピーを作成
                Dim wctxt As New WinCustomTextBox()

                ' 編集属性を指定
                wctxt.MaxLength = Me.MaxLength

                wctxt.IsNumeric = Me.IsNumeric
                wctxt.EditInitialValue = Me.EditInitialValue
                wctxt.EditAddFigure = Me.EditAddFigure
                wctxt.EditPadding = Me.EditPadding
                wctxt.EditDigitsAfterDP = Me.EditDigitsAfterDP
                wctxt.EditDigitsAfterDP_Editing = Me.EditDigitsAfterDP_Editing

                'wctxt.DisplayUnits = this.DisplayUnits;

                ' 編集してTextに設定する。
                wctxt.Text = value
                'wctxt.ReEdit(); // .TextでReEditされるので。
                MyBase.Text = wctxt.Text
            End Set
        End Property

        ''' <summary>単位変換に対応したValueプロパティの実体値（退避用）</summary>
        ''' <remarks>
        ''' ポイント
        ''' ・内部から触る値なので、
        ''' ・string型であること。
        ''' ・単位変換後の値とする。
        ''' </remarks>
        Private _Value As String = ""

        ''' <summary>単位変換に対応したValueプロパティ</summary>
        ''' <remarks>内部からは触らない値</remarks>
        <Category("表示"), Description("単位変換に対応したValueプロパティ")> _
        Public Property Value() As Object
            Get
                'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + this._Value.ToString());
                'Debug.WriteLine(Environment.StackTrace);

                If String.IsNullOrEmpty(DirectCast(Me._Value, String)) Then
                    ' 空文字列の場合は、DBNullを返す。
                    Return DBNull.Value
                Else
                    ' 単位変更して戻す（これではフォーットが崩れる問題あり）。
                    Return Me.FromKMGT(Me._Value)
                End If
            End Get

            Set(ByVal value As Object)
                'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + value.ToString());
                'Debug.WriteLine(Environment.StackTrace);

                If Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") <> -1 Then
                    ' Binding（Target_Validate）から呼ばれた時は無視。
                    Return
                End If

                '　型変換（直ちにstring型に）
                Dim temp As String
                If value Is Nothing Then
                    ' nullは、空文字列
                    temp = ""
                ElseIf value.[GetType]() Is GetType(DBNull) Then
                    ' DBNullは、空文字列
                    temp = ""
                Else
                    ' その他は、ToString()
                    ' 指数問題はToKMGT内で解決
                    temp = value.ToString()
                End If

                ' 単位変換し、
                temp = Me.ToKMGT(temp)

                ' Textへ設定して、
                MyBase.Text = temp

                ' Valueにも退避
                ' なにもしない。
                If Me._Value = temp Then
                Else
                    ' 変更された値の反映
                    Me._Value = temp
                    ' Valueプロパティの変更通知
                    Me.OnValueChanged(EventArgs.Empty)
                    Me.NotifyPropertyChanged("Value")
                End If


                If Me.Focused Then
                    ' フォーカスがある場合は編集（ReEdite）しない。
                Else
                    ' 上記以外は編集（ReEdite）する。
                    Me.ReEdit()
                End If
            End Set
        End Property

        ''' <summary>単位変換</summary>
        ''' <param name="value">値</param>
        ''' <returns>単位を変更し文字列化した値</returns>
        Private Function ToKMGT(ByVal value As String) As String
            Dim dbl As Double = 0
            Dim ret As String = ""

            If Me._displayUnits IsNot Nothing Then
                If Double.TryParse(value, dbl) Then
                    ' 単位を変更
                    dbl = (dbl / Math.Pow(10, CInt(Me._displayUnits)))
                    ' 小数点以下30桁の精度まで保証
                    ret = dbl.ToString("F30")
                    ' 余分な0を削除（第二引数はダミー
                    ret = Me.DeleteZeroAfterDP(ret, New EditDigitsAfterDP(CutMethod._4sya5nyu, 1))
                Else
                    ' エラー時、初期化
                    Me.InitializeValue(ret)
                End If
            Else
                ' そのまま
                ret = value
            End If

            Return ret
        End Function

        ''' <summary>単位変換（逆）</summary>
        ''' <param name="value">値</param>
        ''' <returns>単位を戻し文字列化した値</returns>
        Private Function FromKMGT(ByVal value As String) As String
            Dim dbl As Double = 0
            Dim ret As String = ""

            If Me._displayUnits IsNot Nothing Then
                If Double.TryParse(value, dbl) Then
                    ' 単位を変更、初期化
                    dbl = (dbl * System.Math.Pow(10, CInt(Me._displayUnits)))
                    ' 小数点以下30桁の精度まで保証
                    ret = dbl.ToString("F30")
                    ' 余分な0を削除（第二引数はダミー
                    ret = Me.DeleteZeroAfterDP(ret, New EditDigitsAfterDP(CutMethod._4sya5nyu, 1))
                Else
                    ' エラー時、初期化
                    Me.InitializeValue(ret)
                End If
            Else
                ' そのまま
                ret = value
            End If

            Return ret
        End Function

#End Region

#Region "値取得（IGetValue）"

        ''' <summary>
        ''' Text値をDateTime型にキャストして返す。
        ''' </summary>
        ''' <returns>DateTime値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDateTime() As DateTime Implements IGetValue.GetDateTime
            Return DateTime.Parse(MyBase.Text)
        End Function

        ''' <summary>
        ''' Text値をDateTime型にキャストして返す。
        ''' </summary>
        ''' <param name="provider">書式</param>
        ''' <returns>DateTime値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDateTime(ByVal provider As IFormatProvider) As DateTime Implements IGetValue.GetDateTime
            Return DateTime.Parse(MyBase.Text, provider)
        End Function

        ''' <summary>
        ''' Text値をDateTime型にキャストして返す。
        ''' </summary>
        ''' <param name="provider">書式</param>
        ''' <param name="styles">スタイル</param>
        ''' <returns>DateTime値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDateTime(ByVal provider As IFormatProvider, ByVal styles As DateTimeStyles) As DateTime Implements IGetValue.GetDateTime
            Return DateTime.Parse(MyBase.Text, provider, styles)
        End Function

        ''' <summary>
        ''' Text値をDecimal型にキャストして返す。
        ''' </summary>
        ''' <returns>Decimal値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDecimal() As Decimal Implements IGetValue.GetDecimal
            Return Decimal.Parse(Me.DeleteFigure())
        End Function

        ''' <summary>
        ''' Text値をDouble型にキャストして返す。
        ''' </summary>
        ''' <returns>Double値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDouble() As Double Implements IGetValue.GetDouble
            Return Double.Parse(Me.DeleteFigure())
        End Function

        ''' <summary>
        ''' Text値をFloat型にキャストして返す。
        ''' </summary>
        ''' <returns>Float値</returns>
        <DebuggerStepThrough()> _
        Public Function GetFloat() As Single Implements IGetValue.GetFloat
            Return Single.Parse(Me.DeleteFigure())
        End Function

        ''' <summary>
        ''' Text値をInt16型にキャストして返す。
        ''' </summary>
        ''' <returns>Int16値</returns>
        <DebuggerStepThrough()> _
        Public Function GetInt16() As Short Implements IGetValue.GetInt16
            Return Short.Parse(Me.DeleteFigure())
        End Function

        ''' <summary>
        ''' Text値をInt32型にキャストして返す。
        ''' </summary>
        ''' <returns>Int32値</returns>
        <DebuggerStepThrough()> _
        Public Function GetInt32() As Integer Implements IGetValue.GetInt32
            Return Integer.Parse(Me.DeleteFigure())
        End Function

        ''' <summary>
        ''' Text値をInt64型にキャストして返す。
        ''' </summary>
        ''' <returns>Int64値</returns>
        <DebuggerStepThrough()> _
        Public Function GetInt64() As Long Implements IGetValue.GetInt64
            Return Long.Parse(Me.DeleteFigure())
        End Function

        ''' <summary>桁区切り文字を削除</summary>
        ''' <returns>桁区切り文字削除後の文字列</returns>
        Private Function DeleteFigure() As String
            If Me.EditAddFigure <> EditAddFigure.None Then
                ' EditAddFigure.Noneでない場合
                Return MyBase.Text.Replace(",", "")
            Else
                ' EditAddFigure.Noneの場合
                Return MyBase.Text
            End If
        End Function

#End Region

#Region "デザインタイム・プロパティ"

        ''' <summary>入力制限専用</summary>
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

#Region "チェック プロパティ"

        ''' <summary>Validatingイベントでチェックする</summary>
        ''' [DefaultValue(false),
        <DefaultValue(False), Category("Check"), Description("Validatingイベントでチェックするかどうか")> _
        Public Property CheckValidating() As Boolean
            Get
                Return m_CheckValidating
            End Get
            Set(ByVal value As Boolean)
                m_CheckValidating = value
            End Set
        End Property
        Private m_CheckValidating As Boolean

        '---

        ''' <summary>入力文字種チェック</summary>
        Private _checkType As New CheckType()

        ''' <summary>入力文字種チェック</summary>
        <Category("Check"), Description("入力文字種チェック")> _
        Public Property CheckType() As CheckType Implements ICheck.CheckType
            Get
                Return Me._checkType
            End Get
            Set(ByVal value As CheckType)
                Me._checkType = value
            End Set
        End Property

        ''' <summary>入力文字種チェックのデフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeCheckType() As Boolean Implements ICheck.ShouldSerializeCheckType
            Return Me._checkType <> New CheckType()
        End Function

        '---

        ''' <summary>正規表現チェック</summary>
        <DefaultValue(""), Category("Check"), Description("正規表現チェック")> _
        Public Property CheckRegExp() As String Implements ICheck.CheckRegExp
            Get
                Return m_CheckRegExp
            End Get
            Set(ByVal value As String)
                m_CheckRegExp = value
            End Set
        End Property
        Private m_CheckRegExp As String

        '---

        ''' <summary>正規表現チェック</summary>
        <DefaultValue(False), Category("Check"), Description("禁則文字チェック")> _
        Public Property CheckProhibitedChar() As Boolean Implements ICheck.CheckProhibitedChar
            Get
                Return m_CheckProhibitedChar
            End Get
            Set(ByVal value As Boolean)
                m_CheckProhibitedChar = value
            End Set
        End Property
        Private m_CheckProhibitedChar As Boolean

#End Region

#Region "編集プロパティ"

        ''' <summary>初期値編集</summary>
        Private _editInitialValue As EditInitialValue = EditInitialValue.Blank

        ''' <summary>初期値編集</summary>
        <Category("Edit"), Description("初期値編集")> _
        Public Property EditInitialValue() As EditInitialValue Implements IEdit.EditInitialValue
            Get
                Return Me._editInitialValue
            End Get
            Set(ByVal value As EditInitialValue)
                Me._editInitialValue = value
            End Set
        End Property

        ''' <summary>初期値編集のデフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeEditInitialValue() As Boolean Implements IEdit.ShouldSerializeEditInitialValue
            Return Me._editInitialValue <> EditInitialValue.Blank
        End Function

        '---

        ''' <summary>桁区切り編集</summary>
        Private _editAddFigure As EditAddFigure = EditAddFigure.None

        ''' <summary>桁区切り編集</summary>
        <Category("Edit"), Description("桁区切り編集")> _
        Public Property EditAddFigure() As EditAddFigure Implements IEdit.EditAddFigure
            Get
                Return Me._editAddFigure
            End Get
            Set(ByVal value As EditAddFigure)
                Me._editAddFigure = value
            End Set
        End Property

        ''' <summary>桁区切り編集のデフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeEditAddFigure() As Boolean Implements IEdit.ShouldSerializeEditAddFigure
            Return Me._editAddFigure <> EditAddFigure.None
        End Function

        '---

        ''' <summary>文字埋め編集</summary>
        Private _editPadding As New EditPadding(PadDirection.None, Nothing)

        ''' <summary>文字埋め編集</summary>
        <Category("Edit"), Description("文字埋め編集")> _
        Public Property EditPadding() As EditPadding Implements IEdit.EditPadding
            Get
                Return Me._editPadding
            End Get
            Set(ByVal value As EditPadding)
                Me._editPadding = value
            End Set
        End Property

        ''' <summary>文字埋め編集のデフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeEditPadding() As Boolean Implements IEdit.ShouldSerializeEditPadding
            Return Me._editPadding <> New EditPadding(PadDirection.None, Nothing)
        End Function

        '---

        ''' <summary>小数点以下編集（入力後）</summary>
        Private _editDigitsAfterDP As New EditDigitsAfterDP(CutMethod.None, 0)

        ''' <summary>小数点以下編集（入力後）</summary>
        <Category("Edit"), Description("小数点以下ｘ桁編集（入力後）")> _
        Public Property EditDigitsAfterDP() As EditDigitsAfterDP Implements IEdit.EditDigitsAfterDP
            Get
                Return Me._editDigitsAfterDP
            End Get
            Set(ByVal value As EditDigitsAfterDP)
                Me._editDigitsAfterDP = value
            End Set
        End Property

        ''' <summary>小数点以下編集（入力後）のデフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeEditDigitsAfterDP() As Boolean Implements IEdit.ShouldSerializeEditDigitsAfterDP
            Return Me._editDigitsAfterDP <> New EditDigitsAfterDP(CutMethod.None, 0)
        End Function

        '---

        ''' <summary>小数点以下編集（入力中）</summary>
        Private _editDigitsAfterDP_Editing As New EditDigitsAfterDP(CutMethod.None, 0)

        ''' <summary>小数点以下編集（入力中）</summary>
        <Category("Edit"), Description("小数点以下ｘ桁編集（入力中）")> _
        Public Property EditDigitsAfterDP_Editing() As EditDigitsAfterDP
            Get
                Return Me._editDigitsAfterDP_Editing
            End Get
            Set(ByVal value As EditDigitsAfterDP)
                Me._editDigitsAfterDP_Editing = value
            End Set
        End Property

        ''' <summary>小数点以下編集（入力中）のデフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeEditDigitsAfterDP_Editing() As Boolean
            Return Me._editDigitsAfterDP_Editing <> New EditDigitsAfterDP(CutMethod.None, 0)
        End Function

        ' ---

        ''' <summary>単位編集</summary>
        Private _displayUnits As System.Nullable(Of UInteger) = Nothing

        ''' <summary>単位編集</summary>
        <Category("Edit"), Description("単位編集")> _
        Public Property DisplayUnits() As System.Nullable(Of UInteger)
            Get
                Return Me._displayUnits
            End Get
            Set(ByVal value As System.Nullable(Of UInteger))
                Me._displayUnits = value
            End Set
        End Property

        ''' <summary>単位編集デフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeDisplayUnits() As Boolean
            Return Me._displayUnits IsNot Nothing
        End Function

#End Region

#End Region

#Region "イベント系"

#Region "イベントの説明"

        ' TextBox1からTextBox2にフォーカスを移動したときのイベントの発生順序。
        ' http://homepage1.nifty.com/rucio/main/dotnet/shokyu/standard23.htm

        ' マウス、またはFocusメソッドでフォーカスを移動する場合
        ' -----------------------------------------------------
        ' TextBox1.LostFocus → TextBox1.Leave
        ' → TextBox1.Validating → TextBox1.Validated
        ' → TextBox2.Enter → TextBox2.GotFocus

        ' その他の方法でフォーカスを移動する場合
        ' -----------------------------------------------------
        ' TextBox1.Leave → TextBox1.Validating → TextBox1.Validated
        ' → TextBox2.Enter → TextBox1.LostFocus → TextBox2.GotFocus

        ' ★共通する法則★
        ' -----------------------------------------------------
        ' TextBox1.Leave → TextBox1.Validating → TextBox1.Validated → TextBox2.Enter
        ' TextBox1.Enter → TextBox1.Leave → TextBox1.Validating → TextBox1.Validated

#End Region

#Region "初期化処理（Layout）"

        ''' <summary>初期化処理</summary>
        Private Sub WinCustomTextBox_Layout(ByVal sender As Object, ByVal e As EventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            If (Not RcFxCmnFunction.IsDesignMode()) Then
                ' デザイン・モードでは無い場合。
                If MyBase.Text = "" AndAlso Me.NumericalPossibility AndAlso Me.EditInitialValue = EditInitialValue.Zero Then
                    ' 「0」初期化
                    MyBase.Text = "0"

                    ' ReEditeする。
                    Me.ReEdit()
                End If
            End If
        End Sub

#End Region

#Region "チェック処理（Validating、Validated）"

        ''' <summary>背景色（バックアップ）</summary>
        Private _backupBkColor As System.Nullable(Of Color) = Nothing

        ''' <summary>チェック処理</summary>
        Private Sub WinCustomTextBox_Validating(ByVal sender As Object, ByVal e As CancelEventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            ' チェック前の変換処理
            Me.PreValidate()

            ' Validatingイベントでチェックするしない。
            If Me.CheckValidating Then
                ' Validatingイベントでチェックする。
                ''/ 数値チェックエラー時はクリア
                'if (this.HasNumericCheckError)
                '{
                '    if (this.EditInitialValue == EditInitialValue.Blank)
                '    {
                '        // 空文字列クリア
                '        base.Text = "";
                '    }
                '    else
                '    {
                '        // 「0」クリア
                '        base.Text = "0";
                '    }
                '}
                If Not Me.Validate() Then
                End If
                ' Validatingイベントでチェックしない。
            Else
            End If
        End Sub

        ''' <summary>チェック後処理</summary>
        Private Sub WinCustomTextBox_Validated(ByVal sender As Object, ByVal e As EventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            ' Valueに退避
            ' なにもしない。
            If Me._Value = MyBase.Text Then
            Else
                ' 変更された値の反映
                Me._Value = MyBase.Text
                ' Valueプロパティの変更通知
                Me.OnValueChanged(EventArgs.Empty)
                Me.NotifyPropertyChanged("Value")
            End If

            ' 編集処理
            Me.ReEdit()
        End Sub

#End Region

#Region "フォーマット処理（Enter、Leave、TextChanged）"

        ''' <summary>MouseDown状態の確認用フラグ</summary>
        Private MouseDowned As Boolean = False

        ''' <summary>マウスが入った</summary>
        Private Sub WinCustomTextBox_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            Me.MouseDowned = True
        End Sub

        ''' <summary>フォーカス Enter</summary>
        Private Sub WinCustomTextBox_Enter(ByVal sender As Object, ByVal e As EventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ((WinCustomTextBox)sender).Text);

            ' 編集処理
            Me.Edit()

            If Not Me.MouseDowned Then
                ' MouseDown状態で無ければ全選択
                Me.BeginInvoke(New MethodInvoker(AddressOf MethodInvokerDelegate_SelectAll))
            End If
        End Sub

        ''' <summary>（MethodInvoker）delegate廃止（VB化時に問題）</summary>
        Private Sub MethodInvokerDelegate_SelectAll()
            Me.SelectAll()
        End Sub

        ''' <summary>ロスト フォーカス</summary>
        Private Sub WinCustomTextBox_Leave(ByVal sender As Object, ByVal e As EventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ((WinCustomTextBox)sender).Text);

            Me.MouseDowned = False
        End Sub

        ' ↓　リアーキし、Textプロパティに移動した。

        '// <summary>テキスト変更時</summary>
        'private void WinCustomTextBox_TextChanged(object sender, EventArgs e)

#End Region

#Region "フィルタ処理（KeyPress）"

        ' <summary>フィルタ処理</summary>
        'private void WinCustomTextBox_KeyPress(object sender, KeyPressEventArgs e)
        '{
        '    ・・・
        '}

        '↓Maskの実装に合わせる。

        ''' <summary>ProcessCmdKey</summary>
        ''' <param name="msg">ウィンドウ メッセージ</param>
        ''' <param name="keyData">Keys</param>
        ''' <returns>
        ''' 文字がコントロールによって
        ''' ・処理された場合は true。
        ''' ・それ以外の場合は false
        ''' </returns>
        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
            ' 数値、桁区切り指定、小数点以下切り捨て指定がある場合

            If Me.NumericalPossibility Then
                If keyData = Keys.OemPeriod OrElse keyData = Keys.[Decimal] Then
                    ' 「.」キー

                    ' 数値に'.'が入力済み
                    If MyBase.Text.IndexOf("."c) <> -1 Then
                        ' '.'キーの無効化
                        Return True
                    End If

                    ' 小数点以下設定が無い場合
                    Dim edad As EditDigitsAfterDP = Me.EditDigitsAfterDP
                    Dim edad_e As EditDigitsAfterDP = Me.EditDigitsAfterDP_Editing

                    ' EditDigitsAfterDPがnullか、
                    ' HowToCutがnullかCutMethod.Noneに指定されていた場合。
                    If (edad Is Nothing OrElse (edad IsNot Nothing AndAlso (edad.HowToCut.HasValue AndAlso edad.HowToCut.Value = CutMethod.None))) _
                    AndAlso (edad_e Is Nothing OrElse (edad_e IsNot Nothing AndAlso (edad_e.HowToCut.HasValue AndAlso edad_e.HowToCut.Value = CutMethod.None))) Then
                        ' '.'キーの無効化
                        Return True


                        ' 入力を許可
                    End If
                ElseIf keyData = Keys.OemMinus OrElse keyData = Keys.Subtract Then
                    ' 「-」キー

                    If Me.SelectionStart = 0 Then
                        ' 先頭
                        If Me.SelectionLength = 0 Then
                            ' 選択でない
                            ' 数値に'-'が入力済み
                            If MyBase.Text.IndexOf("-"c) <> -1 Then
                                ' '-'キーの無効化
                                Return True
                            End If
                            ' 選択
                            ' 先頭の-を選択している筈なので、
                            ' '-'を入力可能とする。
                        Else
                        End If
                    Else
                        ' 先頭以外
                        ' '-'キーの無効化
                        Return True

                        ' 入力を許可
                    End If
                ElseIf (keyData >= Keys.D0 AndAlso keyData <= Keys.D9) OrElse (keyData >= Keys.NumPad0 AndAlso keyData <= Keys.NumPad9) OrElse keyData = Keys.Back OrElse keyData = Keys.Delete OrElse keyData = (Keys.Control Or Keys.Z) OrElse keyData = Keys.Left OrElse keyData = Keys.Right OrElse keyData = Keys.Home OrElse keyData = Keys.[End] OrElse keyData = Keys.Tab OrElse keyData = (Keys.Tab Or Keys.Shift) OrElse keyData = Keys.Up OrElse keyData = Keys.Down OrElse keyData = Keys.Enter OrElse keyData = (Keys.Left Or Keys.Shift) OrElse keyData = (Keys.Right Or Keys.Shift) OrElse keyData = (Keys.Home Or Keys.Shift) OrElse keyData = (Keys.[End] Or Keys.Shift) OrElse keyData = (Keys.Control Or Keys.C) OrElse keyData = (Keys.Control Or Keys.X) OrElse keyData = (Keys.Control Or Keys.V) OrElse keyData = (Keys.Control Or Keys.A) Then
                    ' || keyData == Keys.ProcessKey
                    ' || keyData == (Keys.Oemplus | Keys.Shift) || keyData == Keys.Add
                    ' 入力を許可
                    ' ・「0-9」、「BSP・DEL」。「Ctrl・Z」（編集）
                    ' ・「←・→」、「HOME・END」（カーソル移動）
                    ' ・「TAB」、「SHIFT＋TAB」（タブ遷移）
                    ' ・「↑・↓」、「Enter」（セル移動）
                    ' ・「SHIFT＋←・→」、「SHIFT＋HOME・END」（選択系）
                    ' ・「Ctrl・C」、「Ctrl・X」、「Ctrl・V」、「Ctrl・A」（コピペ系）

                    ' 検討の結果、入力を許可しないようにしたキー。
                    ' ・「漢字モード」、「+」、「,」

                    ' || keyData == Keys.Oemcomma)
                Else
                    ' 数値以外キーの無効化
                    Return True
                End If
            End If

            ' 入力を許可
            Return False
        End Function

#End Region

#Region "クリア処理（KeyDown）"

        ''' <summary>クリア処理</summary>
        Private Sub WinCustomTextBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            ' ここではダメでした。
        End Sub

#End Region

#Region "復元処理（KeyUp）"

        ''' <summary>０の復元</summary>
        Private Sub WinCustomTextBox_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            ' 無限ループ対応
            Dim txt As String = [String].Copy(MyBase.Text)

            If e.KeyCode = Keys.Delete OrElse e.KeyCode = Keys.Back Then
                ' DELキーとBackSpaceキー

                ' クリアされてしまった。
                If txt = "" Then
                    ' 初期化
                    If Me.InitializeValue(txt) Then
                        ' DELキーとBackSpaceキーの無効化
                        e.Handled = True
                        ' Textプロパティをクリア。
                        MyBase.Text = txt
                        ' this.ReEdit(); // ステータス的に行われないため不要。
                        ' 選択状態
                        Me.SelectAll()
                    End If
                End If
                ' なにもしない。
            Else
            End If
        End Sub

#End Region

#End Region

#Region "Validate"

        ''' <summary>チェック前の変換処理</summary>
        Public Sub PreValidate()
            ' 生入力
            Dim txt As String = [String].Copy(MyBase.Text)

            ' 半角化（数値指定されている場合）
            If Me.NumericalPossibility Then
                ' ワーク
                Dim temp As String = ""
                Dim sb As New StringBuilder()

                ' 半角化する。
                temp = [Public].Str.StringConverter.ToHankaku(txt)

                ' 残っている全角＋非数値文字を削る。
                Dim minus As Integer = 0
                Dim period As Boolean = False

                For Each ch As Char In temp
                    If StringChecker.IsNumbers_Hankaku(ch.ToString()) Then
                        ' 半角数値だけ追加する。
                        sb.Append(ch)
                    ElseIf ch = "-"c AndAlso minus = 0 Then
                        ' マイナスは先頭だけ
                        sb.Append(ch)
                    ElseIf ch = "."c AndAlso Not period Then
                        ' ピリオドも先頭の一つだけ。

                        ' ＆小数点以下指定がある場合だけ。
                        Dim edad As EditDigitsAfterDP = Me.EditDigitsAfterDP
                        Dim edad_e As EditDigitsAfterDP = Me.EditDigitsAfterDP_Editing
                        If (edad IsNot Nothing AndAlso (edad.HowToCut.HasValue AndAlso edad.HowToCut.Value <> CutMethod.None)) _
                            OrElse (edad_e IsNot Nothing AndAlso (edad_e.HowToCut.HasValue AndAlso edad_e.HowToCut.Value <> CutMethod.None)) Then
                            period = True
                            sb.Append(ch)
                        Else
                            ' スルー
                        End If
                    Else
                        ' スルー
                    End If

                    minus += 1
                Next

                txt = sb.ToString()

				' 数値の筈が、数値で無い場合に初期化。
				If Not StringChecker.IsNumeric(txt) Then
					Me.InitializeValue(txt)
				End If
            End If

            ' EditDigitsAfterDP_Editing
            ' 空文字列の場合は編集せず。
            If txt <> "" _
                AndAlso Me.EditDigitsAfterDP_Editing IsNot Nothing Then
                ' 小数点数以下ｘ桁四捨五入（丸め）編集
                Select Case Me.EditDigitsAfterDP_Editing.HowToCut
                    Case CutMethod.Banker
                        txt = FormatConverter.Round_Banker(txt, CInt(Me.EditDigitsAfterDP_Editing.DigitsAfterDP))
                        Exit Select
                    Case CutMethod._4sya5nyu
                        txt = FormatConverter.Round_4sya5nyu(txt, CInt(Me.EditDigitsAfterDP_Editing.DigitsAfterDP))
                        Exit Select
                    Case CutMethod.Floor
						txt = FormatConverter.Floor(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RZ)
                        Exit Select
                    Case CutMethod.Ceiling
						txt = FormatConverter.Ceiling(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RI)
						Exit Select
					Case CutMethod.FloorRM
						txt = FormatConverter.Floor(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RM)
						Exit Select
					Case CutMethod.CeilingRP
						txt = FormatConverter.Ceiling(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RP)
                        Exit Select
                    Case Else
                        'case CutMethod.None:
                        Exit Select
                End Select

                ' 少数点以下の０削除
                txt = Me.DeleteZeroAfterDP(txt, Me.EditDigitsAfterDP_Editing)
                '' 少数点以下の０付け足し
                'txt = Me.AddZeroAfterDP(txt, Me.EditDigitsAfterDP_Editing)
                'ここでの０足しは無くす（丸めだけできれば良いので）。
            End If

            If MyBase.Text <> txt Then
                ' 変更された場合は再設定
                MyBase.Text = txt
            End If
        End Sub

        '// <summary>数値チェックエラーの有無</summary>
        '// <remarks>
        '// true：数値チェックエラー有
        '// false：数値チェックエラー無
        '// </remarks>
        'private bool HasNumericCheckError = false;

        ''' <summary>チェック処理</summary>
        ''' <returns>
        ''' ・エラーなし：true
        ''' ・エラーあり：false
        ''' </returns>
        ''' <remarks>
        ''' ・必須入力チェック
        ''' ・数値チェック
        ''' ・半角チェック
        ''' ・全角チェック
        ''' ・片仮名チェック
        ''' ・半角片仮名チェック
        ''' ・平仮名チェック
        ''' ・日付チェック
        ''' 
        ''' ・正規表現チェック
        ''' ・禁則文字チェック
        ''' </remarks>
        Public Function Validate() As Boolean Implements ICheck.Validate
            '/ 初期化
            'this.HasNumericCheckError = false;

            Dim temp As String() = Nothing
            Dim ret As Boolean = Me.Validate(temp)

            'foreach (string s in temp)
            '{
            '    if (s == CmnCheckFunction.IsNumericCheckErrorMessage)
            '    {
            '        // 数値チェックエラー
            '        this.HasNumericCheckError = true;
            '    }
            '}

            Return ret
        End Function

        ''' <summary>チェック処理</summary>
        ''' <param name="result">結果文字列</param>
        ''' <returns>
        ''' ・エラーなし：true
        ''' ・エラーあり：false
        ''' </returns>
        ''' <remarks>
        ''' ・必須入力チェック
        ''' ・数値チェック
        ''' ・半角チェック
        ''' ・全角チェック
        ''' ・片仮名チェック
        ''' ・半角片仮名チェック
        ''' ・平仮名チェック
        ''' ・日付チェック
        ''' 
        ''' ・正規表現チェック
        ''' ・禁則文字チェック
        ''' </remarks>
        Public Function Validate(ByRef result As String()) As Boolean Implements ICheck.Validate
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            ' フラグ
            Dim hasError As Boolean = False
            ' ワーク
            Dim lstRet As New List(Of String)()

            Dim txt As String = [String].Copy(MyBase.Text)

            If Me.CheckType IsNot Nothing Then
                ' 必須入力チェック
                If Me.CheckType.IsIndispensabile Then
                    If (txt = "") Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsIndispensabileCheckErrorMessage)
                    End If
                End If

                ' 数値チェック（空文字列は対象外）
                If Me.CheckType.IsNumeric AndAlso txt.Trim() <> "" Then
                    If Not StringChecker.IsNumeric(txt) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsNumericCheckErrorMessage)
                    End If
                End If

                ' 半角チェック
                If Me.CheckType.IsHankaku Then
                    If Not StringChecker.IsHankaku(txt) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsHankakuCheckErrorMessage)
                    End If
                End If

                ' 全角チェック
                If Me.CheckType.IsZenkaku Then
                    If Not StringChecker.IsZenkaku(txt) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsZenkakuCheckErrorMessage)
                    End If
                End If

                ' 片仮名チェック
                If Me.CheckType.IsKatakana Then
                    If Not StringChecker.IsKatakana(txt) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsKatakanaCheckErrorMessage)
                    End If
                End If

                ' 半角片仮名チェック
                If Me.CheckType.IsHanKatakana Then
                    If Not StringChecker.IsKatakana_Hankaku(txt) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsHanKatakanaCheckErrorMessage)
                    End If
                End If

                ' 平仮名チェック
                If Me.CheckType.IsHiragana Then
                    If Not StringChecker.IsHiragana(txt) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsHiraganaCheckErrorMessage)
                    End If
                End If

                ' 日付チェック（空文字列は対象外）
                If Me.CheckType.IsDate AndAlso txt.Trim() <> "" Then
                    Dim dateTime__1 As DateTime
                    If Not DateTime.TryParse(txt, dateTime__1) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsDateCheckErrorMessage)
                    End If
                End If
            End If

            ' 正規表現チェック（空文字列は対象外）
            If Me.CheckRegExp IsNot Nothing AndAlso Me.CheckRegExp <> "" AndAlso txt.Trim() <> "" Then
                If Not StringChecker.Match(txt, Me.CheckRegExp) Then
                    hasError = True
                    lstRet.Add(CmnCheckFunction.RegularExpressionCheckErrorMessage)
                End If
            End If

            ' 禁則文字チェック
            If Me.CheckProhibitedChar Then
                For Each ch As Char In CmnCheckFunction.ProhibitedChars
                    If txt.IndexOf(ch) <> -1 Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.ProhibitedCharsCheckErrorMessage)
                        Exit For
                    End If
                Next
            End If

            ' 背景変更
            If hasError Then
                ' エラーの背景色
                If Me._backupBkColor Is Nothing Then
                    Me._backupBkColor = CType(Me.BackColor, System.Nullable(Of Color))
                    Me.BackColor = Color.Red
                End If
            Else
                ' 正常時の背景色
                If Me._backupBkColor IsNot Nothing Then
                    Me.BackColor = Me._backupBkColor.Value
                    Me._backupBkColor = Nothing
                End If
            End If

            result = lstRet.ToArray()
            Return Not hasError
        End Function

        '// <summary>
        '// Formを閉じる時だけValidatingイベント内での検証をやめる方法
        '// http://d.hatena.ne.jp/NAL-6295/20081015/p1
        '// </summary>
        'protected override void OnValidating(CancelEventArgs e)
        '{
        '    Control parentControl = this.Parent;

        '    while (parentControl != null)
        '    {   
        '        if (parentControl is Form)
        '        {
        '            // Formの場合
        '            if (parentControl.CausesValidation)
        '            {
        '                // Formの検証が必要なので、
        '                // Validatingイベントを呼ぶ。
        '                break;
        '            }
        '            else
        '            {
        '                // Formの検証が不要
        '                if ((parentControl as Form).ActiveControl == this)
        '                {
        '                    // 自身がアクティブである場合、
        '                    // Validatingイベントを呼ばない。
        '                    return;
        '                }
        '                else if (!(parentControl as Form).ActiveControl.CausesValidation)
        '                {
        '                    // アクティブな自分以外の
        '                    // （親）コントロールが
        '                    // 検証を必要としていない場合、
        '                    // Validatingイベントを呼ばない。
        '                    return;
        '                }
        '                else
        '                {
        '                    // 次へ or 抜ける。
        '                }
        '            }
        '        }

        '        // 遡る。
        '        parentControl = parentControl.Parent;
        '    }

        '    // Validatingイベントを呼ぶ。
        '    base.OnValidating(e);
        '}

#End Region

#Region "編集"

        ''' <summary>編集されたかどうか</summary>
        ''' <remarks>
        ''' true：編集された
        ''' false：編集されていない
        ''' </remarks>
        Protected Edited As Boolean = False

        ''' <summary>編集</summary>
        Friend Sub Edit()
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-start:" + base.Text);
            'Debug.WriteLine(Environment.StackTrace);

            ' Editされた
            Me.Edited = True

            '/ ０対応
            'string temp = "";

            Dim txt As String = ""

            ' Editの取得元は可変。
            If Me.DisplayUnits Is Nothing Then
                ' DisplayUnitsがNULLである。
                txt = [String].Copy(MyBase.Text)
            Else
                ' DisplayUnitsがNULLでない。
                txt = [String].Copy(Me._Value)
            End If

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point1");

            ' EditDigitsAfterDP_Editing
            ' 空文字列の場合は編集せず。
            If txt <> "" _
                AndAlso Me.EditDigitsAfterDP_Editing IsNot Nothing Then

                ' 小数点数以下ｘ桁四捨五入（丸め）編集
                Select Case Me.EditDigitsAfterDP_Editing.HowToCut
                    Case CutMethod.Banker
                        txt = FormatConverter.Round_Banker(txt, CInt(Me.EditDigitsAfterDP_Editing.DigitsAfterDP))
                        Exit Select
                    Case CutMethod._4sya5nyu
                        txt = FormatConverter.Round_4sya5nyu(txt, CInt(Me.EditDigitsAfterDP_Editing.DigitsAfterDP))
                        Exit Select
                    Case CutMethod.Floor
						txt = FormatConverter.Floor(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RZ)
                        Exit Select
                    Case CutMethod.Ceiling
						txt = FormatConverter.Ceiling(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RI)
						Exit Select
					Case CutMethod.FloorRM
						txt = FormatConverter.Floor(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RM)
						Exit Select
					Case CutMethod.CeilingRP
						txt = FormatConverter.Ceiling(txt, Me.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RP)
                        Exit Select
                    Case Else
                        'case CutMethod.None:
                        Exit Select
                End Select

                ' 少数点以下の０削除
                txt = Me.DeleteZeroAfterDP(txt, Me.EditDigitsAfterDP_Editing)
                ' 少数点以下の０付け足し
                txt = Me.AddZeroAfterDP(txt, Me.EditDigitsAfterDP_Editing)
            End If

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point2");

            ' EditAddFigure
            ' 空文字列の場合は編集せず。
            If txt <> "" AndAlso Me.EditAddFigure <> EditAddFigure.None Then
                txt = txt.Replace(",", "")
            End If

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point3");

            ' 表示を変更
            MyBase.Text = txt

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-end:" + base.Text);
        End Sub

        ''' <summary>編集（逆）</summary>
        Friend Sub ReEdit()
            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-start:" + base.Text);
            'Debug.WriteLine(Environment.StackTrace);

            ' ReEditされた
            Me.Edited = False

            ' ０対応
            Dim temp As String = ""

            Dim txt As String = [String].Copy(MyBase.Text)

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point1");

            ' 数値だが、数値で無い場合、
            If Me.NumericalPossibility AndAlso (Not StringChecker.IsNumeric(txt)) Then
                ' 初期化
                Me.InitializeValue(txt)

                ' 処理順を変えたくないので↓に追加した。

                ' 変更された値の反映
                Me._Value = txt
                ' Valueプロパティの変更通知
                Me.OnValueChanged(EventArgs.Empty)
                Me.NotifyPropertyChanged("Value")
            End If

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point2");

            ' EditDigitsAfterDP
            ' 空文字列の場合は編集せず。
            If txt <> "" _
                AndAlso Me.EditDigitsAfterDP IsNot Nothing Then

                ' 小数点数以下ｘ桁四捨五入（丸め）編集
                Select Case Me.EditDigitsAfterDP.HowToCut
                    Case CutMethod.Banker
                        txt = FormatConverter.Round_Banker(txt, CInt(Me.EditDigitsAfterDP.DigitsAfterDP))
                        Exit Select
                    Case CutMethod._4sya5nyu
                        txt = FormatConverter.Round_4sya5nyu(txt, CInt(Me.EditDigitsAfterDP.DigitsAfterDP))
                        Exit Select
                    Case CutMethod.Floor
						txt = FormatConverter.Floor(txt, Me.EditDigitsAfterDP.DigitsAfterDP, FloorToward.RZ)
                        Exit Select
                    Case CutMethod.Ceiling
						txt = FormatConverter.Ceiling(txt, Me.EditDigitsAfterDP.DigitsAfterDP, CeilingToward.RI)
						Exit Select
					Case CutMethod.FloorRM
						txt = FormatConverter.Floor(txt, Me.EditDigitsAfterDP.DigitsAfterDP, FloorToward.RM)
						Exit Select
					Case CutMethod.CeilingRP
						txt = FormatConverter.Ceiling(txt, Me.EditDigitsAfterDP.DigitsAfterDP, CeilingToward.RP)
                        Exit Select
                    Case Else
                        'case CutMethod.None:
                        Exit Select
                End Select

                ' 少数点以下の０削除
                txt = Me.DeleteZeroAfterDP(txt, Me.EditDigitsAfterDP)
                ' 少数点以下の０付け足し
                txt = Me.AddZeroAfterDP(txt, Me.EditDigitsAfterDP)
            End If

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point3");

            ' EditAddFigure
            Dim editAddFigure__1 As EditAddFigure = Me.EditAddFigure

            ' DataBindings対応

            ' Target_Validateからの呼び出しを無視するようにしたため不要となった。

            '/ 現象：小数点以下指定と、Formatの桁区切りが干渉して桁区切りがされない。
            '/ 理由：Binding.Target_Validate→SetPropValueからのTextプロパティ呼出で、
            '/ 編集を行ってしまうと、FormatStringの書式に戻らないため、
            '/ 自力での編集処理が必要になる。
            'foreach (Binding b in this.DataBindings)
            '{
            '    if (b.FormatString.IndexOf("#,##0") != -1)
            '    {
            '        // DataBindingsで3桁区切りが指定されてる。
            '        editAddFigure = EditAddFigure.Af3;
            '    }
            '    else if (b.FormatString.IndexOf("#,###0") != -1)
            '    {
            '        // DataBindingsで4桁区切りが指定されてる。
            '        editAddFigure = EditAddFigure.Af4;
            '    }
            '}

            ' 空文字列の場合は編集せず。
            If txt <> "" _
                AndAlso editAddFigure__1 <> EditAddFigure.None Then
                ' 整数部の桁区切り編集
                If editAddFigure__1 = EditAddFigure.Af3 Then
                    ' 3桁区切り
                    temp = FormatConverter.AddFigure3(txt)
                    If temp <> "0" Then
                        txt = temp
                    End If
                ElseIf editAddFigure__1 = EditAddFigure.Af4 Then
                    ' 4桁区切り
                    temp = FormatConverter.AddFigure4(txt)
                    If temp <> "0" Then
                        txt = temp
                    End If
                End If
            End If

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point4");

            ' EditPadding
            ' 空文字列の場合は編集せず。
            If txt <> "" _
                AndAlso Me.EditPadding IsNot Nothing AndAlso Me.EditPadding.PadDir <> PadDirection.None Then
                ' 文字埋め編集
                Dim ch As Char = " "c

                ' デフォルトは半角スペース埋め
                If Me.EditPadding.PadChar IsNot Nothing Then
                    ch = CChar(Me.EditPadding.PadChar)
                End If

                ' パディング
                If Me.EditPadding.PadDir = PadDirection.Left Then
                    txt = txt.PadLeft(Me.MaxLength, ch)
                ElseIf Me.EditPadding.PadDir = PadDirection.Right Then
                    txt = txt.PadRight(Me.MaxLength, ch)
                End If

            End If

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point5");

            ' 表示を変更（Valueは変更しない）。
            MyBase.Text = txt

            'Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-end:" + base.Text);
        End Sub

        ''' <summary>CheckCharCodeクラス</summary>
        Private Shared CCC As New CheckCharCode("0", "9", Encoding.ASCII)

        ''' <summary>設定から、数値である可能性を探る</summary>
        ''' <remarks>数値、桁区切り指定、小数点以下切り捨て、数値パッド指定がある場合</remarks>
        Private ReadOnly Property NumericalPossibility() As Boolean
            Get
                Dim edad As EditDigitsAfterDP = Me.EditDigitsAfterDP
                Dim edad_e As EditDigitsAfterDP = Me.EditDigitsAfterDP_Editing
                'EditPadding ep = this.EditPadding;

                '|| (ep != null && ep.PadChar.HasValue && WinCustomTextBox.CCC.IsInRange(ep.PadChar.ToString())));

                ' 数値パッドは数値としない（数値埋めで、ASCIIコード入力のケースもある）。
                Return (Me.IsNumeric _
                    OrElse Me.CheckType.IsNumeric _
                    OrElse Me.EditInitialValue = EditInitialValue.Zero _
                    OrElse Me.EditAddFigure <> EditAddFigure.None _
                    OrElse (edad IsNot Nothing AndAlso (edad.HowToCut.HasValue AndAlso edad.HowToCut.Value <> CutMethod.None)) _
                    OrElse (edad_e IsNot Nothing AndAlso (edad_e.HowToCut.HasValue AndAlso edad_e.HowToCut.Value <> CutMethod.None)) _
                    OrElse Me.DisplayUnits.HasValue)
            End Get
        End Property

        ''' <summary>値を初期化する</summary>
        ''' <param name="txt">初期値</param>
        ''' <returns>
        ''' ・true：初期化された。
        ''' ・false：初期化されていない。
        ''' </returns>
        Private Function InitializeValue(ByRef txt As String) As Boolean
            ' 数値である可能性がある。
            If Me.NumericalPossibility Then
                ' 数値である可能性がある場合
                If Me.EditInitialValue = EditInitialValue.Blank Then
                    ' 空文字列クリア
                    txt = ""
                    Return True
                ElseIf Me.EditInitialValue = EditInitialValue.Zero Then
                    ' 「0」クリア
                    txt = "0"

                    ' 少数点以下の０付け足し

                    ' 入力前、入力後で小数点以下設定を変える必要がある。
                    If Me.Focused Then
                        txt = Me.AddZeroAfterDP(txt, Me.EditDigitsAfterDP_Editing)
                    Else
                        txt = Me.AddZeroAfterDP(txt, Me.EditDigitsAfterDP)
                    End If

                    Return True
                    '・・・必要に応じてカスタマイズ可能・・・
                Else
                End If
                ' 数値である可能性が無い場合
            Else
            End If

            Return False
        End Function


        ''' <summary>後ろに小数点以下ｘ桁を付与</summary>
        ''' <param name="txt">入力文字列</param>
        ''' <param name="edad">小数点以下切り捨て指定</param>
        ''' <returns>編集後の文字列</returns>
        Private Function AddZeroAfterDP(ByVal txt As String, ByVal edad As EditDigitsAfterDP) As String
            If (edad IsNot Nothing AndAlso edad.DigitsAfterDP <> 0 _
                AndAlso (edad.HowToCut.HasValue AndAlso edad.HowToCut.Value <> CutMethod.None)) _
                AndAlso txt <> "" AndAlso StringChecker.IsHankaku(txt) AndAlso StringChecker.IsNumeric(txt) Then
                ' 小数点以下切り捨て指定有り（の数値入力）の場合で、
                ' 整数部が入力されている場合。

                ' 少数部が入力されている・いない。
                Dim IndexOfDP As Integer = txt.IndexOf("."c)

                If IndexOfDP = -1 Then
                    ' DP無しの場合、

                    ' MaxLength（最低2文字（.0）の空きが必須
                    If txt.Length + 2 <= Me.MaxLength Then
                        ' DP付与は可能
                        txt += "."

                        For i As Long = 0 To edad.DigitsAfterDP - 1
                            ' MaxLengthに達していたら、forをbreak
                            If Me.MaxLength <= txt.Length Then
                                Exit For
                            End If

                            ' 以降、桁数分０追加
                            txt += "0"
                        Next

                    Else
                        ' DP付与も不可
                    End If
                Else
                    ' DP有りの場合、
                    For i As Integer = txt.Length - (IndexOfDP + 1) To CInt(edad.DigitsAfterDP) - 1
                        ' MaxLengthに達していたら、forをbreak
                        If Me.MaxLength <= txt.Length Then
                            Exit For
                        End If

                        ' 以降、桁数分０追加
                        txt += "0"
                    Next

                End If
            End If

            Return txt
        End Function

        ''' <summary>小数点以下ｘ桁の０を削除</summary>
        ''' <param name="txt">入力文字列</param>
        ''' <param name="edad">小数点以下切り捨て指定</param>
        ''' <returns>編集後の文字列</returns>
        Private Function DeleteZeroAfterDP(ByVal txt As String, ByVal edad As EditDigitsAfterDP) As String
            If (edad IsNot Nothing AndAlso (edad.HowToCut.HasValue AndAlso edad.HowToCut.Value <> CutMethod.None)) _
                AndAlso txt <> "" AndAlso StringChecker.IsHankaku(txt) AndAlso StringChecker.IsNumeric(txt) Then
                ' 小数点以下切り捨て指定有り（の数値入力）の場合で、
                ' 整数部が入力されている場合。

                ' 余分な「0」を削る。
                ' 小数点無し。
                If txt.IndexOf("."c) = -1 Then
                Else
                    ' 小数点有り。

                    ' 後ろから０を削っていく（.も対象となる）。
                    Dim i As Integer = txt.Length - 1

                    ' 空文字列
                    If i = -1 Then
                    Else
                        While txt(i) = "0"c OrElse txt(i) = "."c
                            If i <= 0 Then
                                ' ・・・
                                Exit While
                            Else
                                ' 削る
                                If txt(i) = "0"C Then
                                    txt = txt.Substring(0, i)
                                    ' '0'は継続
                                    i -= 1 ' ＃Devで変換できなかったので、手動で追加
                                    Continue While
                                ElseIf txt(i) = "."C Then
                                    txt = txt.Substring(0, i)
                                    ' '.'で停止
                                    Exit While
                                Else
                                    ' ここは通らない。
                                End If
                            End If
                            i -= 1
                        End While
                    End If
                End If
            End If

            Return txt
        End Function

#End Region
    End Class
End Namespace
