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
'* クラス名        ：WebCustomCheckBox
'* クラス日本語名  ：チェックボックス（Web）のカスタム・コントロール（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'**********************************************************************************

Imports System.ComponentModel

Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Touryo.Infrastructure.CustomControl
	''' <summary>チェックボックス（Web）のカスタム・コントロール</summary>
	<DefaultProperty("Text")> _
	<ToolboxData("<{0}:WebCustomCheckBox runat=server></{0}:WebCustomCheckBox>")> _
	Public Class WebCustomCheckBox
		Inherits CheckBox
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
