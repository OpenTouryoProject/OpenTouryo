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
'* クラス名        ：WebCustomTextBox
'* クラス日本語名  ：テキストボックス（Web）のカスタム・コントロール（テンプレート）
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
Imports System.IO

' System.Web
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Imports System.Diagnostics
Imports System.Globalization

Imports Touryo.Infrastructure.Public.Str

Namespace Touryo.Infrastructure.CustomControl
	''' <summary>テキストボックス（Web）のカスタム・コントロール</summary>
	<DefaultProperty("TextBox")> _
	<ToolboxData("<{0}:WebCustomTextBox runat=server></{0}:WebCustomTextBox>")> _
	Public Class WebCustomTextBox
		Inherits TextBox
        Implements ICheck, IGetValue
		''' <summary>
		''' コンストラクタ
		''' </summary>
        Public Sub New()
            ' コンストラクタでのスタイル系のプロパティ設定は、
            ' CSSとの相性が悪と考え、廃止しました。

            'this.Font.Size = 12;
            'this.ForeColor = System.Drawing.Color.Black;
            'this.Font.Name = "ＭＳ ゴシック";
        End Sub

		#Region "値取得（IGetValue）"

		''' <summary>
		''' Text値をDateTime型にキャストして返す。
		''' </summary>
		''' <returns>DateTime値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDateTime() As DateTime Implements IGetValue.GetDateTime
            Return DateTime.Parse(Me.Text)
        End Function

		''' <summary>
		''' Text値をDateTime型にキャストして返す。
		''' </summary>
		''' <param name="provider">書式</param>
		''' <returns>DateTime値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDateTime(ByVal provider As IFormatProvider) As DateTime Implements IGetValue.GetDateTime
            Return DateTime.Parse(Me.Text, provider)
        End Function

		''' <summary>
		''' Text値をDateTime型にキャストして返す。
		''' </summary>
		''' <param name="provider">書式</param>
		''' <param name="styles">スタイル</param>
		''' <returns>DateTime値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDateTime(ByVal provider As IFormatProvider, ByVal styles As DateTimeStyles) As DateTime Implements IGetValue.GetDateTime
            Return DateTime.Parse(Me.Text, provider, styles)
        End Function

		''' <summary>
		''' Text値をDecimal型にキャストして返す。
		''' </summary>
		''' <returns>Decimal値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDecimal() As Decimal Implements IGetValue.GetDecimal
            Return Decimal.Parse(Me.Text)
        End Function

		''' <summary>
		''' Text値をDouble型にキャストして返す。
		''' </summary>
		''' <returns>Double値</returns>
        <DebuggerStepThrough()> _
        Public Function GetDouble() As Double Implements IGetValue.GetDouble
            Return Double.Parse(Me.Text)
        End Function

		''' <summary>
		''' Text値をFloat型にキャストして返す。
		''' </summary>
		''' <returns>Float値</returns>
        <DebuggerStepThrough()> _
        Public Function GetFloat() As Single Implements IGetValue.GetFloat
            Return Single.Parse(Me.Text)
        End Function

		''' <summary>
		''' Text値をInt16型にキャストして返す。
		''' </summary>
		''' <returns>Int16値</returns>
        <DebuggerStepThrough()> _
        Public Function GetInt16() As Short Implements IGetValue.GetInt16
            Return Short.Parse(Me.Text)
        End Function

		''' <summary>
		''' Text値をInt32型にキャストして返す。
		''' </summary>
		''' <returns>Int32値</returns>
        <DebuggerStepThrough()> _
        Public Function GetInt32() As Integer Implements IGetValue.GetInt32
            Return Integer.Parse(Me.Text)
        End Function

		''' <summary>
		''' Text値をInt64型にキャストして返す。
		''' </summary>
		''' <returns>Int64値</returns>
        <DebuggerStepThrough()> _
        Public Function GetInt64() As Long Implements IGetValue.GetInt64
            Return Long.Parse(Me.Text)
        		End Function

		#End Region

		#Region "デザインタイム・プロパティ"

		#Region "チェック プロパティ"

		' こちらのプロパティは基本的にPGで変更しないのでViewState化しないこととした。
		' 変更＋状態保存が必要であれば、必要に応じてViewState化すること。

        ''// <summary>TEST</summary>
        ''// <remarks>
        ''// The Official Microsoft ASP.NET Forums
        ''// WebCategoryAttribute and WebSysDescription compile errors.
        ''// http://forums.asp.net/t/1330898.aspx/1
        ''// </remarks>
		'[Category("Test"),
		'Description("テスト"), 
		'DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        ''/WebCategory("Appearance"),
        ''/WebSysDescription("WebControl_Font")]
		'public virtual FontInfo TestFont
		'{
		'    get
		'    {
		'        return this.ControlStyle.Font;
		'    }
		'}

		' HOW TO: Create a Web Control with an Expandable Property in the Designer by Using Visual C# .NET
		' http://support.microsoft.com/kb/324301/ja
		''' <summary>入力文字種チェック</summary>
		Private _checkType As New CheckType()

		''' <summary>入力文字種チェック</summary>
        <Category("Check"), Description("入力文字種チェック"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
        Public Property CheckType() As CheckType Implements ICheck.CheckType
            Get
                Return Me._checkType
            End Get
            Set(ByVal value As CheckType)
                Me._checkType = Value
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
		Private _checkRegExp As String = ""

		''' <summary>正規表現チェック</summary>
        <DefaultValue(""), Category("Check"), Description("正規表現チェック")> _
        Public Property CheckRegExp() As String Implements ICheck.CheckRegExp
            Get
                Return Me._checkRegExp
            End Get
            Set(ByVal value As String)
                Me._checkRegExp = Value
            End Set
        End Property

		''' <summary>正規表現チェック</summary>
		Private _checkProhibitedChar As Boolean = False

		''' <summary>正規表現チェック</summary>
        <DefaultValue(False), Category("Check"), Description("禁則文字チェック")> _
        Public Property CheckProhibitedChar() As Boolean Implements ICheck.CheckProhibitedChar
            Get
                Return Me._checkProhibitedChar
            End Get
            Set(ByVal value As Boolean)
                Me._checkProhibitedChar = Value
            End Set
        End Property

		#End Region

		#End Region

		#Region "チェック処理（Validating、Validated）"

		#Region "Validate"

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
            Dim temp As String() = Nothing
            Dim ret As Boolean = Me.Validate(temp)

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
            ' フラグ
            Dim hasError As Boolean = False
            ' ワーク
            Dim lstRet As New List(Of String)()

            Dim text As String = Me.Text

            If Me.CheckType IsNot Nothing Then
                ' 必須入力チェック
                If Me.CheckType.IsIndispensabile Then
                    If (text = "") Then
                        hasError = True
                        lstRet.Add(CmnCheckFunction.IsIndispensabileCheckErrorMessage)
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
                    Dim dateTime__1 As DateTime
                    If Not DateTime.TryParse(text, dateTime__1) Then
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
                If Me.ViewState("wcc_backupBkColor") Is Nothing Then
                    Me.ViewState("wcc_backupBkColor") = CType(Me.BackColor, System.Nullable(Of Color))
                    Me.BackColor = Color.Red
                End If
            Else
                ' 正常時の背景色
                If Me.ViewState("wcc_backupBkColor") IsNot Nothing Then
                    Me.BackColor = DirectCast(Me.ViewState("wcc_backupBkColor"), Color)
                    Me.ViewState("wcc_backupBkColor") = Nothing
                End If
            End If

            result = lstRet.ToArray()
            Return Not hasError
        End Function

		#End Region

		#End Region

		#Region "HTML描画処理のカスタマイズ用テンプレート"

		#Region "Renderの制御"

		''' <summary>Visibleプロパティ、ページのトレースなどの制御を行い、ページにコントロールを表示する。</summary>
		''' <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
		''' <remarks>
		''' このメソッドは、表示中にページによって自動的に呼び出される。
		''' カスタム コントロールの開発者はこのメソッドをオーバーライドできる。
		''' 
		''' WebControl.RenderControlメソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.ui.webcontrols.webcontrol.rendercontrol.aspx
		''' </remarks>
		Public Overrides Sub RenderControl(output As HtmlTextWriter)
			'if (false)
			'{
			'    // RenderControlにRenderの制御処理を作り込む。
			'    // Render処理のHtmlTextWriterを他のWriterでdecorateする。

			'    // （例）
			'    // StringWriterの書き出し先のStringBuilderを生成
			'    StringBuilder sb = new StringBuilder();
			'    // StringWriterを使用するStringWriterを生成
			'    StringWriter sw = new StringWriter(sb);

			'    // 上記StringWriterを使用するHtmlTextWriterを生成（decorate）
			'    HtmlTextWriter decoratedHTW = new HtmlTextWriter(sw);

			'    // StringWriterを使用するHtmlTextWriterを指定し、Renderメソッドを実行する。
			'    // すると、HtmlTextWriter→StringWriterにWriteされる。
			'    this.Render(decoratedHTW);

			'    // RenderメソッドでStringWriterにWriteされた、
			'    // コントロールのHTMLをStringBuilderから取得する。
			'    string html = sb.ToString();

			'    // TODO：string htmlの編集処理を実装する。
			'    // 本来はCustomWriterのWriteメソッド内で、
			'    // 変換処理を実施後にHtmlTextWriterにWriteする。

			'    // ページにコントロールを表示
			'    output.Write(html);
			'}
			'else
			'{

			' 通常通りのRenderControl
			MyBase.RenderControl(output)

			'}
		End Sub

		#End Region

		#Region "Renderの実体"

		''' <summary>
		''' Renderメソッドは、
		''' ・RenderBeginTag（開始のタグ）
		''' ・RenderContents（中間の部分）
		''' ・RenderEndTag（終了のタグ）
		''' の各メソッドをこの順に呼び出して、
		''' コントロールをクライアントに送信する。
		''' </summary>
		''' <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
		''' <remarks>
		''' このメソッドは、表示中にページによって自動的に呼び出される。
		''' また、このメソッドは、主にコントロールの開発者によって使用される。
		''' 
		''' WebControl.Renderメソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.ui.webcontrols.webcontrol.render.aspx
		''' </remarks>
		Protected Overrides Sub Render(output As HtmlTextWriter)
			' Render処理を作り込む。

			'if (false)
			'{
			'    // Renderｘの順次呼び出し
			'    // ・RenderBeginTag（開始のタグ）
			'    // ・RenderContents（中間の部分）
			'    // ・RenderEndTag（終了のタグ）

			'    // 独自にWriteでも良い。
			'    output.Write("xxxx");
			'}
			'else
			'{

			' 通常通りのRender
			MyBase.Render(output)

			'}
		End Sub

		#End Region

		#Region "以下は、RenderBeginTag、RenderContents、RenderEndTagの実装例"

        ''// <summary>RenderBeginTag</summary>
        ''// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
		'public override void RenderBeginTag(HtmlTextWriter output)
		'{
		'    output.Write("<test>");
		'}

        ''// <summary>RenderContents</summary>
        ''// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
		'protected override void RenderContents(HtmlTextWriter output)
		'{
		'    output.Write("test");
		'}

        ''// <summary>RenderEndTagのテスト</summary>
        ''// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
		'public override void RenderEndTag(HtmlTextWriter output)
		'{
		'    output.Write("</test>");
		'}

		#End Region

		#End Region
	End Class
End Namespace
