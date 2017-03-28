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
'* クラス名        ：WinCustomMaskedTextBox
'* クラス日本語名  ：マスク テキスト ボックス（Win）のカスタム・コントロール（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2016/01/28  Sai               Corrected IsIndispensabile property spelling
'*  2017/01/31  西野 大介         "Indispensable" ---> "Required"
'**********************************************************************************

Imports System.Text
Imports System.Globalization
Imports System.Drawing
Imports System.ComponentModel

Imports System.Windows.Forms
Imports System.Windows.Forms.Design

Imports Touryo.Infrastructure.Framework.RichClient.Util
Imports Touryo.Infrastructure.Public.Str

Namespace Touryo.Infrastructure.CustomControl.RichClient
    ''' <summary>System.Windows.Forms.MaskedTextBoxのカスタム・コントロール</summary>
    <DefaultProperty("Text")> _
    <Designer(GetType(WinCustomMaskedTextBox.WinCustomMaskedTextBoxDesigner))> _
    Public Class WinCustomMaskedTextBox
        Inherits MaskedTextBox
        Implements ICheck, IGetValue, INotifyPropertyChanged

        '''<summary>デザイナ上の表示をカスタマイズするインナークラス</summary>
        '''<remarks>
        '''自作コントロールにおいて、不要なプロパティをデザイナANDインテリセンスで隠したい時
        '''http://jehupc.exblog.jp/8157762/
        '''継承と属性プログラミングで実現するRAD開発 － ＠IT
        '''http://www.atmarkit.co.jp/fdotnet/winexp/winexp02/winexp02_03.html
        '''</remarks>
        Friend Class WinCustomMaskedTextBoxDesigner
            Inherits ControlDesigner
            '''<summary>下記で指定してあるプロパティはデザイナでは非表示とする。</summary>
            Protected Overrides Sub PostFilterProperties(ByVal Properties As IDictionary)
                Properties.Remove("Text2")
                Properties.Remove("Text3")
            End Sub
        End Class

#Region "初期処理"

        ''' <summary>コンストラクタ</summary>
        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()

            ' Textはマスク適用強制
            DirectCast(Me, MaskedTextBox).TextMaskFormat = MaskFormat.IncludeLiterals
        End Sub

        ''' <summary>コンストラクタ</summary>
        ''' <param name="mask">マスク</param>
        Public Sub New(ByVal mask As String)
            MyBase.New(mask)
            Me.InitializeComponent()
        End Sub

        ''' <summary>初期化</summary>
        Private Sub InitializeComponent()
            Me.SuspendLayout()
            ' 
            ' WinCustomMaskedTextBox
            ' 
            AddHandler Me.Layout, AddressOf Me.WinCustomMaskedTextBox_Layout
            AddHandler Me.TextChanged, AddressOf Me.WinCustomMaskedTextBox_TextChanged

            AddHandler Me.Enter, AddressOf Me.WinCustomMaskedTextBox_Enter
            AddHandler Me.Validating, AddressOf Me.WinCustomMaskedTextBox_Validating
            AddHandler Me.Validated, AddressOf Me.WinCustomMaskedTextBox_Validated
            AddHandler Me.Leave, AddressOf Me.WinCustomMaskedTextBox_Leave

            AddHandler Me.KeyDown, AddressOf Me.WinCustomMaskedTextBox_KeyDown
            AddHandler Me.KeyUp, AddressOf Me.WinCustomMaskedTextBox_KeyUp

            AddHandler Me.MouseDown, AddressOf Me.WinCustomMaskedTextBox_MouseDown

            Me.ResumeLayout(False)

        End Sub

#End Region

#Region "プロパティ拡張"

#Region "変更イベントや、変更通知"

        ' Text2、3プロパティの変更イベントは実装しないTextプロパティのものを使用する。

        ''' <summary>変更通知イベント（汎用）</summary>
        Public Event PropertyChanged( _
            ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>変更通知イベント発生（汎用）</summary>
        ''' <param name="propertyName">プロパティ名</param>
        Private Sub NotifyPropertyChanged(ByVal propertyName As [String])
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

#End Region

        ''' <summary>Textプロパティにテキスト変更時の変更通知機能を追加実装する。</summary>
        Public Overrides Property Text() As String
            Get
                Return MyBase.Text
            End Get

            Set(ByVal value As String)
                If Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") <> -1 Then
                    ' Binding（Target_Validate）から呼ばれた時は無視。
                    Return
                End If

                ' ここでは、直接入力やbase.Textの呼び出しの際に変更されないため、
                ' 実装箇所を、WinCustomMaskedTextBox_TextChangedに変更する。

                '/ Text2、3プロパティの変更通知
                'this.NotifyPropertyChanged("Text2");
                '/this.NotifyPropertyChanged("Text3");
                MyBase.Text = value
            End Set
        End Property

        ''' <summary>
        ''' TextMaskFormatは初期値、IncludeLiteralsのままReadonlyに変更。
        ''' このため、Textプロパティでは必ずマスク適用時の値を取得する。
        ''' </summary>
        <Category("動作"), Description("TextMaskFormatは初期値、IncludeLiteralsのままReadonlyに。")> _
        Public Shadows ReadOnly Property TextMaskFormat() As MaskFormat
            Get
                Return DirectCast(Me, MaskedTextBox).TextMaskFormat
            End Get
        End Property

        ''' <summary>Text2プロパティではマスクを除いた値を取得する。</summary>
        ''' <remarks>Bindingsで使用可能なようにset_Text2を用意した。</remarks>
        <Category("表示"), Description("Text2プロパティではマスクを除いた値を設定・取得する。")> _
        Public Property Text2() As String
            Get
                ' ExcludePromptAndLiteralsの値を取る。
                Dim mtb As New MaskedTextBox()
                mtb.Mask = MyBase.Mask
                mtb.Text = MyBase.Text
                mtb.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals
                Return mtb.Text
            End Get

            Set(ByVal value As String)
                If Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") <> -1 Then
                    ' Binding（Target_Validate）から呼ばれた時は無視。
                    Return
                End If

                ' ユーザ入力
                MyBase.Text = value
            End Set
        End Property

        ''' <summary>Text3プロパティでは表示時マスク適用時の値を取得する。</summary>
        <Category("表示"), Description("Text3プロパティでは表示時マスク適用時の値を取得する。")> _
        Public ReadOnly Property Text3() As String
            Get
                If Not String.IsNullOrEmpty(Me.OriginalMask) Then
                    ' 表示時マスク有り（入力中）
                    Dim mtb As New MaskedTextBox(Me.OriginalMask)
                    mtb.Text = Me.Text2

                    ' 表示時マスク適用時の値
                    mtb.TextMaskFormat = MaskFormat.IncludeLiterals
                    Return mtb.Text
                ElseIf Not String.IsNullOrEmpty(Me.Mask) Then
                    ' 表示時マスク有り（入力後）
                    Dim mtb As New MaskedTextBox(Me.Mask)
                    mtb.Text = Me.Text2

                    ' 表示時マスク適用時の値
                    mtb.TextMaskFormat = MaskFormat.IncludeLiterals
                    Return mtb.Text
                Else
                    ' マスクなし。
                    Return MyBase.Text
                End If
            End Get
        End Property

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
            ' 区切りを削除（数値用途限定）
            ' マスクでこの処理でＯＫにしておく前提。
            ' 例えば「,,,1,,,1,,,」でも、「11」になるので。
            Return MyBase.Text.Replace(",", "")
        End Function

#End Region

#Region "デザインタイム・プロパティ"

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
        Public Property EditInitialValue() As EditInitialValue
            Get
                Return Me._editInitialValue
            End Get
            Set(ByVal value As EditInitialValue)
                Me._editInitialValue = value
            End Set
        End Property

        ''' <summary>初期値編集のチェックのデフォルト</summary>
        ''' <returns>
        ''' デフォルト以外：true
        ''' デフォルト：false
        ''' </returns>
        Public Function ShouldSerializeEditInitialValue() As Boolean
            Return Me._editInitialValue <> EditInitialValue.Blank
        End Function

        ''' <summary>オリジナルのマスク</summary>
        ''' <remarks>退避しておくためのワーク</remarks>
        Friend Property OriginalMask() As String
            Get
                Return m_OriginalMask
            End Get
            Private Set(ByVal value As String)
                m_OriginalMask = value
            End Set
        End Property
        Private m_OriginalMask As String

        ''' <summary>入力中のマスク</summary>
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

        ''' <summary>半角編集</summary>
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

        ''' <summary>日付編集</summary>
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

#End Region

#End Region

#Region "イベント系"

        ' ＜イベントの説明＞
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

#Region "初期化処理（Layout）"

        ''' <summary>初期化処理</summary>
        Private Sub WinCustomMaskedTextBox_Layout(ByVal sender As Object, ByVal e As EventArgs)
            If (Not RcFxCmnFunction.IsDesignMode()) Then
                ' デザイン・モードでは無い場合。
                If MyBase.Text = "" AndAlso Me.CheckType.IsNumeric AndAlso Me.EditInitialValue = EditInitialValue.Zero Then
                    ' 「0」初期化
                    MyBase.Text = "0"
                    Me.ReEdit()
                End If
            End If
        End Sub

#End Region

#Region "チェック処理（Validating、Validated）"

        ''' <summary>背景色（バックアップ）</summary>
        Private _backupBkColor As System.Nullable(Of Color) = Nothing

        ''' <summary>チェック処理</summary>
        Private Sub WinCustomMaskedTextBox_Validating(ByVal sender As Object, ByVal e As CancelEventArgs)
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
        Private Sub WinCustomMaskedTextBox_Validated(ByVal sender As Object, ByVal e As EventArgs)
            ' 編集処理
            Me.ReEdit()
        End Sub

#End Region

#Region "フォーマット処理（Enter、Leave、TextChanged）"

        ''' <summary>MouseDown状態の確認用フラグ</summary>
        Private IsMouseDown As Boolean = False

        ''' <summary>マウスが入った</summary>
        Private Sub WinCustomMaskedTextBox_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            Me.IsMouseDown = True
        End Sub

        ''' <summary>フォーカス Enter</summary>
        Private Sub WinCustomMaskedTextBox_Enter(ByVal sender As Object, ByVal e As EventArgs)
            ' 編集処理
            Me.Edit()

            If Not Me.IsMouseDown Then
                ' MouseDown状態で無ければ全選択
                Me.BeginInvoke(New MethodInvoker(AddressOf MethodInvokerDelegate_SelectAll))
            End If
        End Sub

        ''' <summary>（MethodInvoker）delegate廃止（VB化時に問題）</summary>
        Private Sub MethodInvokerDelegate_SelectAll()
            Me.SelectAll()
        End Sub

        ''' <summary>ロスト フォーカス</summary>
        Private Sub WinCustomMaskedTextBox_Leave(ByVal sender As Object, ByVal e As EventArgs)
            Me.IsMouseDown = False
        End Sub

        ''' <summary>テキスト変更時</summary>
        Private Sub WinCustomMaskedTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
            ' Text2、3プロパティの変更通知
            'this.NotifyPropertyChanged("Text3");
            Me.NotifyPropertyChanged("Text2")
        End Sub

#End Region

#Region "フィルタ処理（KeyPress）"

        ' <summary>フィルタ処理</summary>

        'private void WinCustomMaskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        '{
        '    ・・・
        '}

        '↓Maskの場合はKeyPressに行かない・・・

        ''' <summary>ProcessCmdKey</summary>
        ''' <param name="msg">ウィンドウ メッセージ</param>
        ''' <param name="keyData">Keys</param>
        ''' <returns>
        ''' 文字がコントロールによって
        ''' ・処理された場合は true。
        ''' ・それ以外の場合は false
        ''' </returns>
        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
            ' 数値指定がある場合
            If Me.CheckType.IsNumeric Then
                If keyData = Keys.OemPeriod OrElse keyData = Keys.[Decimal] Then
                    ' 「.」キー

                    ' 数値に'.'が入力済み
                    If MyBase.Text.IndexOf("."c) <> -1 Then
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
                    '|| keyData == (Keys.Oemplus | Keys.Shift) || keyData == Keys.Add
                    ' ・「漢字モード」、
                    ' ・「TAB」キー、「SHIFT＋TAB」キー、
                    ' ・「BSP・DEL」、「HOME・END」
                    ' ・「←・→」キー、「SHIFT＋←・→」（選択）キー
                    ' ・「0-9」、「+」（除外）、「,」キー

                    ' 入力を許可
                ElseIf keyData = Keys.ProcessKey OrElse keyData = Keys.Tab OrElse keyData = (Keys.Tab Or Keys.Shift) OrElse keyData = Keys.Back OrElse keyData = Keys.Delete OrElse keyData = Keys.Home OrElse keyData = Keys.[End] OrElse keyData = Keys.Left OrElse keyData = Keys.Right OrElse keyData = (Keys.Left Or Keys.Shift) OrElse keyData = (Keys.Right Or Keys.Shift) OrElse (keyData >= Keys.D0 AndAlso keyData <= Keys.D9) OrElse (keyData >= Keys.NumPad0 AndAlso keyData <= Keys.NumPad9) OrElse keyData = Keys.Oemcomma Then
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
        Private Sub WinCustomMaskedTextBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
            ' ここではダメでした。
        End Sub

#End Region

#Region "復元処理（KeyUp）"

        ''' <summary>０の復元</summary>
        Private Sub WinCustomMaskedTextBox_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
            ' 無限ループ対応
            Dim text As String = MyBase.Text

            If e.KeyCode = Keys.Delete OrElse e.KeyCode = Keys.Back Then
                ' DELキーとBackSpaceキー

                ' クリアされてしまった。
                If text = "" Then
                    ' 初期化
                    If Me.InitializeValue(text) Then
                        ' DELキーとBackSpaceキーの無効化
                        e.Handled = True
                        ' Textプロパティをクリア。
                        MyBase.Text = text
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
            Dim txt As String = Me.Text2

            ' 半角指定（マスクで指定できないため）
            If Me.EditToHankaku Then
                ' ワーク
                Dim temp As String = ""
                Dim sb As New StringBuilder()

                ' 半角化する。
                temp = [Public].Str.StringConverter.ToHankaku(txt)

                ' 残っている全角文字を削る。
                For Each ch As Char In temp
                    If StringChecker.IsHankaku(ch.ToString()) Then
                        ' 半角だけ追加する。
                        sb.Append(ch)
                    End If
                Next

                If txt <> sb.ToString() Then
                    ' 変更された場合は再設定
                    txt = sb.ToString()
                    MyBase.Text = txt
                End If
            End If

            ' YYYYMMDDのM、Dが１桁の時に補正処理を行う。
            If Me.EditToYYYYMMDD Then
                If [Public].Str.StringConverter.EditYYYYMMDDString(txt) Then
                    ' 変更された場合は再設定
                    MyBase.Text = txt
                End If
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
        ''' マスク前の値をチェックする
        ''' ・必須入力チェック
        ''' ・数値チェック
        ''' 
        ''' マスク後の値をチェックする
        ''' ・半角チェック
        ''' ・全角チェック
        ''' ・片仮名チェック
        ''' ・半角片仮名チェック
        ''' ・平仮名チェック
        ''' ・日付チェック
        ''' ・正規表現チェック
        ''' ・禁則文字チェック
        ''' </remarks>
        Public Function Validate() As Boolean Implements ICheck.Validate
            '' 初期化
            'Me.HasNumericCheckError = False

            Dim temp As String() = Nothing
            Dim ret As Boolean = Me.Validate(temp)

            'For Each s As String In temp
            '	If s = CmnCheckFunction.IsNumericCheckErrorMessage Then
            '		' 数値チェックエラー
            '		Me.HasNumericCheckError = True
            '	End If
            'Next

            Return ret
        End Function

        ''' <summary>編集中 or 編集後テキストを返す。</summary>
        ''' <param name="editingText">編集中テキスト</param>
        ''' <param name="editedText">編集後テキスト</param>
        Private Sub GetTexts(ByRef editingText As String, ByRef editedText As String)
            ' マスク後の値を取得
            Dim mtb As MaskedTextBox = Nothing

            ' どちらのマスクを適用するか。
            If Me.OriginalMask Is Nothing OrElse Me.OriginalMask = "" Then
                ' ReEdit後の場合
                If Me.Mask_Editing Is Nothing OrElse Me.Mask_Editing = "" Then
                    ' この場合は、何も設定されていないので、
                    editingText = MyBase.Text
                    ' ＝そのまま
                    ' ＝そのまま
                    editedText = MyBase.Text
                Else
                    ' 編集中マスクを適用
                    mtb = New MaskedTextBox(Me.Mask_Editing)
                    mtb.Text = Me.Text2
                    editingText = mtb.Text

                    ' ＝そのまま
                    editedText = MyBase.Text
                End If
            Else
                ' ReEdit前の場合

                ' 編集後マスクを適用
                mtb = New MaskedTextBox(Me.OriginalMask)
                mtb.Text = Me.Text2
                editedText = mtb.Text

                ' ＝そのまま
                editingText = MyBase.Text
            End If
        End Sub

        ''' <summary>チェック処理</summary>
        ''' <param name="result">結果文字列</param>
        ''' <returns>
        ''' ・エラーなし：true
        ''' ・エラーあり：false
        ''' </returns>
        ''' <remarks>
        ''' マスク前の値をチェックする
        ''' ・必須入力チェック
        ''' ・数値チェック
        ''' 
        ''' マスク後の値をチェックする
        ''' ・半角チェック
        ''' ・全角チェック
        ''' ・片仮名チェック
        ''' ・半角片仮名チェック
        ''' ・平仮名チェック
        ''' ・日付チェック
        ''' ・正規表現チェック
        ''' ・禁則文字チェック
        ''' </remarks>
        Public Function Validate(ByRef result As String()) As Boolean Implements ICheck.Validate
            ' フラグ
            Dim hasError As Boolean = False
            ' ワーク
            Dim lstRet As New List(Of String)()

            ' 生入力
            Dim text As String = Me.Text2
            ' 編集中
            Dim editingText As String = ""
            ' 編集後
            Dim editedText As String = ""
            ' 編集中・編集後
            Me.GetTexts(editingText, editedText)

            If Me.CheckType IsNot Nothing Then
                ' 必須入力チェック
                If Me.CheckType.Required Then
                    If (text = "") Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.RequiredCheckErrorMessage)
                    End If
                End If

                ' 数値チェック（空文字列は対象外）
                If Me.CheckType.IsNumeric AndAlso text.Trim() <> "" Then
                    If Not StringChecker.IsNumeric(text) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsNumericCheckErrorMessage)
                    End If
                End If

                ' 半角チェック
                If Me.CheckType.IsHankaku Then
                    If Not StringChecker.IsHankaku(text) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsHankakuCheckErrorMessage)
                    End If
                End If

                ' 全角チェック
                If Me.CheckType.IsZenkaku Then
                    If Not StringChecker.IsZenkaku(text) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsZenkakuCheckErrorMessage)
                    End If
                End If

                ' 片仮名チェック
                If Me.CheckType.IsKatakana Then
                    If Not StringChecker.IsKatakana(text) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsKatakanaCheckErrorMessage)
                    End If
                End If

                ' 半角片仮名チェック
                If Me.CheckType.IsHanKatakana Then
                    If Not StringChecker.IsKatakana_Hankaku(text) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsHanKatakanaCheckErrorMessage)
                    End If
                End If

                ' 平仮名チェック
                If Me.CheckType.IsHiragana Then
                    If Not StringChecker.IsHiragana(text) Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsHiraganaCheckErrorMessage)
                    End If
                End If

                ' 日付チェック（空文字列は対象外）
                If Me.CheckType.IsDate AndAlso text.Trim() <> "" Then
                    ' 編集後を使用してチェック
                    ' 編集中・編集後（再取得）
                    Me.GetTexts(editingText, editedText)

                    Dim dateTime__1 As DateTime
                    If Not DateTime.TryParse(editedText, dateTime__1) OrElse text.IndexOfAny(New Char() {" "c, "　"c}) <> -1 Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsDateCheckErrorMessage)
                    End If
                End If
            End If

            ' 正規表現チェック（空文字列は対象外）
            If Me.CheckRegExp IsNot Nothing AndAlso Me.CheckRegExp <> "" AndAlso text.Trim() <> "" Then
                If Not StringChecker.Match(text, Me.CheckRegExp) Then
                    hasError = True
                    lstRet.Add(CmnCheckFunction.RegularExpressionCheckErrorMessage)
                End If
            End If

            ' 禁則文字チェック
            If Me.CheckProhibitedChar Then
                For Each ch As Char In CmnCheckFunction.ProhibitedChars
                    If text.IndexOf(ch) <> -1 Then
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
            If Not Me.Edited Then
                ' falseの時、１回だけ
                Me.Edited = True
                Me.OriginalMask = Me.Mask
                Me.Mask = Me.Mask_Editing
            End If
        End Sub

        ''' <summary>編集（逆）</summary>
        Friend Sub ReEdit()
            ' フォーマット変更
            If Me.Edited Then
                ' trueの時、１回だけ
                Me.Edited = False
                Me.Mask = Me.OriginalMask
                Me.OriginalMask = ""
            End If
        End Sub

        ''' <summary>値を初期化する</summary>
        ''' <param name="text">初期値</param>
        ''' <returns>
        ''' true：初期化された。
        ''' false：初期化されていない。
        ''' </returns>
        Private Function InitializeValue(ByRef text As String) As Boolean
            ' 数値の場合
            If Me.CheckType.IsNumeric Then
                ' 数値の場合

                If Me.EditInitialValue = EditInitialValue.Blank Then
                    ' 空文字列クリア
                    text = ""
                    Return True
                ElseIf Me.EditInitialValue = EditInitialValue.Zero Then
                    ' 「0」クリア
                    text = "0"
                    Return True
                    '・・・必要に応じてカスタマイズ可能・・・
                Else
                End If
                ' 数値以外の場合
            Else
            End If

            Return False
        End Function

#End Region

    End Class
End Namespace
