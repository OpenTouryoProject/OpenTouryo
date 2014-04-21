'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testFxLayerP_normal_testNestMasterScreen
'* クラス日本語名  ：マスタページのテスト画面（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>テスト画面ネスト（Ｐ層）</summary>
Public Partial Class Aspx_testFxLayerP_normal_testNestMasterScreen
	Inherits MyBaseController
	#Region "ページロードのUOCメソッド"

	''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する
		' TODO:
	End Sub

	''' <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit_PostBack()
		' フォーム初期化（ポストバック）時に実行する処理を実装する
		' TODO:
	End Sub

	#End Region

	#Region "共通処理"

	''' <summary>共通処理１</summary>
	''' <param name="buttonID">ボタンID</param>
	''' <param name="x">CPFを表す文字数</param>
	Private Sub commonM(buttonID As String, x As Integer)
		' コントロールの取得
		DirectCast(Me.GetMasterWebControl("lblMSG"), Label).Text = buttonID

		' ラベルを非表示
		Dim ctrl As Control = Me.GetMasterWebControl("lblTest" & buttonID.Substring(buttonID.Length - x, x))

		ctrl.Visible = Not (ctrl.Visible)
	End Sub

	''' <summary>共通処理２</summary>
	''' <param name="buttonID">ボタンID</param>
	''' <param name="x">CPFを表す文字数</param>
	Private Sub commonC(buttonID As String, x As Integer)
		' コントロールの取得
		DirectCast(Me.GetMasterWebControl("lblMSG"), Label).Text = buttonID

		' ラベルを非表示
		Dim ctrl As Control = Me.GetContentWebControl("lblTest" & buttonID.Substring(buttonID.Length - x, x))

		ctrl.Visible = Not (ctrl.Visible)
	End Sub

	#End Region

	#Region "イベント処理"

	#Region "マスタ"

	#Region "rootMasterPage"

	''' <summary>btnButtonのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_rootMasterPage_btnButton_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 0)
		Return System.[String].Empty
	End Function

	#End Region

	#Region "branchMasterPage1"

	''' <summary>btnButtonAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage1_btnButtonA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 1)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage1_btnButtonB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 1)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage1_btnButtonC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 1)
		Return System.[String].Empty
	End Function

	#End Region

	#Region "branchMasterPage2"

	''' <summary>btnButtonAAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonAA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonABのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonAB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonACのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonAC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonBA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonBB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonBC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonCA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonCB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_branchMasterPage2_btnButtonCC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonM(fxEventArgs.ButtonID, 2)
		Return System.[String].Empty
	End Function

	#End Region

	#End Region

	#Region "コンテンツ"

	''' <summary>btnButtonAAAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonAAA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonAABのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonAAB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonAACのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonAAC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonABAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonABA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonABBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonABB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonABCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonABC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonACAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonACA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonACBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonACB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonACCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonACC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBAAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBAA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBABのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBAB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBACのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBAC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBBAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBBA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBBBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBBB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBBCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBBC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBCAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBCA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBCBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBCB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonBCCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonBCC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCAAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCAA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCABのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCAB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCACのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCAC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCBAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCBA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCBBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCBB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCBCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCBC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCCAのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCCA_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCCBのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCCB_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	''' <summary>btnButtonCCCのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButtonCCC_Click(fxEventArgs As FxEventArgs) As String
		Me.commonC(fxEventArgs.ButtonID, 3)
		Return System.[String].Empty
	End Function

	'''/// ちなみに存在しないコントロールを検索した場合どうなるかチェックする。
	'''/Control ctrl = null;
	'''/ctrl = this.GetMasterWebControl("xxxx");
	'''/ctrl = this.GetContentWebControl("xxxx");

	#End Region

	#End Region
End Class
