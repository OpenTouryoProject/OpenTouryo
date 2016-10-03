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
'* クラス名        ：WebCustomDropDownList
'* クラス日本語名  ：ドロップダウン・リスト（Web）のカスタム・コントロール（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'**********************************************************************************

Imports System.Drawing

' System
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.IO

' System.Web
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Touryo.Infrastructure.CustomControl
	''' <summary>ドロップダウン・リスト（Web）のカスタム・コントロール</summary>
	<DefaultProperty("DropDownList")> _
	<ToolboxData("<{0}:WebCustomDropDownList runat=server></{0}:WebCustomDropDownList>")> _
	Public Class WebCustomDropDownList
		Inherits DropDownList
		', IMasterData
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

		''' <summary>カスタムReadOnlyプロパティ</summary>
		<DefaultValue(False), Description("カスタムReadOnlyプロパティ")> _
		Public Property [ReadOnly]() As Boolean
			Get
				' 設定されていない場合はfalseで初期化
				If Me.ViewState("wcc_readOnly") Is Nothing Then
					Me.ViewState("wcc_readOnly") = False
				End If

				Return CBool(Me.ViewState("wcc_readOnly"))
			End Get
			Set
				Me.ViewState("wcc_readOnly") = value
			End Set
		End Property

		#Region "Webと相性が悪いので削除"

		' 具体的には↓。
		'<td>
		'  <asp:DropDownList ID="ddlDropDownList1" runat="server" AutoPostBack="true"
		'    DataSource="<%# this.DropDownListDataSource %>" DataValueField="value" DataTextField="text"
		'    SelectedValue='<%# DataBinder.Eval(Container.DataItem, "dropdownlist") %>'/>
		'</td>

        ''// <summary>マスタデータ名</summary>
		'[DefaultValue(""),
		'Description("マスタデータ名")]
		'public string MasterDataName
		'{
		'    get { return (string)this.ViewState["wcc_masterDataName"]; }
		'    set
		'    {
		'        // 全半角スペースは詰め、大文字に揃えて設定する。
		'        if (value == null)
		'        {
		'            this.ViewState["wcc_masterDataName"] = value;
		'        }
		'        else
		'        {
		'            this.ViewState["wcc_masterDataName"] = value.Replace("　", "").Replace(" ", "").ToUpper();
		'        }
		'    }
		'}

        ''// <summary>初期処理（Items）</summary>
		'public void InitItems()
		'{
		'    // マスタデータ設定
		'    if (this.Items.Count == 0)
		'    {
		'        CmnMasterDatasForList.GetMasterData(this.MasterDataName, this.Items);
		'    }
		'    // 初期値を設定
		'    if (this.Items.Count != 0)
		'    {
		'        this.SelectedIndex = 0;// Itemsの場合有効
		'    }
		'}

        ''// <summary>初期処理（DataSource）</summary>
		'public void InitDataSource()
		'{
		'    // マスタデータ設定
		'    if (this.Items.Count == 0)
		'    {
		'        this.DataSource = CmnMasterDatasForList.GetMasterData(this.MasterDataName);
		'    }
		'    // 初期値を設定
		'    if (this.Items.Count != 0)
		'    {
		'        this.SelectedIndex = 0;// Itemsの場合有効
		'    }
		'}
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
			'    // 通常通りのRender
			'    base.Render(output);
			'}

			' Creating a Read Only DropDownList - CodeProject
			' http://www.codeproject.com/Articles/26642/Creating-a-Read-Only-DropDownList
			' cpol Code Project Open License - CodeProject
			' http://www.codeproject.com/info/cpol10.aspx

			If Me.[ReadOnly] Then
				' 出力をテキスト・ボックスに変換。
				Dim tb As New TextBox()
				tb.Text = Me.SelectedItem.Text
				tb.[ReadOnly] = True

				' 色は要カスタマイズ。
				tb.BackColor = Color.LightGray

				' テキスト・ボックスのレンダーに変換。
				tb.RenderControl(output)
			Else
				MyBase.Render(output)
			End If

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
