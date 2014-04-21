Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

Imports Touryo.Infrastructure.Framework.Presentation

Public Partial Class Aspx_Common_WebUserControl
	Inherits System.Web.UI.UserControl
	''' <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnUCButton_Click(fxEventArgs As FxEventArgs) As String
		Response.Write("UOC_btnUCButton_Clickを実行できた。")

		Return ""
	End Function
End Class
