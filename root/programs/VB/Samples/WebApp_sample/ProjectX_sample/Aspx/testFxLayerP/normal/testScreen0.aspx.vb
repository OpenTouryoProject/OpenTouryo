'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testFxLayerP_normal_testScreen0
'* クラス日本語名  ：例外テスト画面（Ｐ層）
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

''' <summary>例外テスト画面（Ｐ層）</summary>
Public Partial Class Aspx_testFxLayerP_normal_testScreen0
	Inherits MyBaseController
	''' <summary>不正操作防止機能の局所化</summary>
	Private Sub Page_Init(sender As Object, e As EventArgs)

		' OFFの場合、当該画面だけ、不正操作防止機能をONにする。
		Me.CanCheckIllegalOperation = True

		For Each key As String In Request.Form.Keys
			If key.IndexOf("btnIllegalOperationCheckOFF") <> -1 Then
				' btnIllegalOperationCheckOFFボタンによりサブミットされた。
				Me.CanCheckIllegalOperation = False
			End If

			If key.IndexOf("btnIllegalOperationCheckON") <> -1 Then
				' btnIllegalOperationCheckONボタンによりサブミットされた。
				Me.CanCheckIllegalOperation = True
			End If
		Next
	End Sub

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

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	#Region "例外処理"

	''' <summary>
	''' btnAppExのクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnAppEx_Click(fxEventArgs As FxEventArgs) As String
		' 業務例外のスロー
		Throw New BusinessApplicationException("E0001", "システム", "停止")
	End Function

	''' <summary>
	''' btnSysExのクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnSysEx_Click(fxEventArgs As FxEventArgs) As String
		' システム例外
		Throw New BusinessSystemException("xxxxx", "P層でスローしたシステム例外")
	End Function

	''' <summary>
	''' btnElseExのクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnElseEx_Click(fxEventArgs As FxEventArgs) As String
		' システム例外
		Throw New Exception("P層でスローしたその他、一般的な例外")
	End Function

	#End Region

	#Region "ユーザ情報のハンドル"

	#Region "キー無し"


	''' <summary>btnSetUserInfoのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnSetUserInfo_Click(fxEventArgs As FxEventArgs) As String
		' ユーザ情報を設定
		UserInfoHandle.SetUserInformation(New MyUserInfo(Me.txtUserName.Text, Request.UserHostAddress))
		Return String.Empty
	End Function

	''' <summary>btnGetUserInfoのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnGetUserInfo_Click(fxEventArgs As FxEventArgs) As String
		' ユーザ情報を取得（ベースクラス２経由）
		If Me.UserInfo Is Nothing Then
			' nullはありえない
			lblUserName.Text = "インスタンスが設定されていません。"
		Else
			lblUserName.Text = Me.UserInfo.UserName
		End If
		Return String.Empty
	End Function

	''' <summary>btnUpdUserInfoのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnUpdUserInfo_Click(fxEventArgs As FxEventArgs) As String
		' ユーザ情報を更新（ベースクラス２経由）
		If Me.UserInfo Is Nothing Then
			' nullはありえない
			lblUserName.Text = "インスタンスが設定されていません。"
		Else
			Me.UserInfo.UserName = Me.txtUserName.Text
		End If
		Return String.Empty
	End Function

	''' <summary>btnDelUserInfoのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnDelUserInfo_Click(fxEventArgs As FxEventArgs) As String
		' ユーザ情報を削除
		UserInfoHandle.DeleteUserInformation()
		Return String.Empty
	End Function

	#End Region

	#End Region

	#Region "サブシステム情報のハンドル"

	''' <summary>btnSetSubSysInfoのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnSetSubSysInfo_Click(fxEventArgs As FxEventArgs) As String
		' サブシステム情報の設定
		Me.SubsysInfo(Me.txtSubSysID.Text)(Me.txtSubSysInfoKey.Text) = Me.txtSubSysInfo.Text
		Return String.Empty
	End Function

	''' <summary>btnGetSubSysInfoのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnGetSubSysInfo_Click(fxEventArgs As FxEventArgs) As String
		' サブシステム情報の取得
		Me.lblSubSysInfo.Text = DirectCast(Me.SubsysInfo(Me.txtSubSysID.Text)(Me.txtSubSysInfoKey.Text), String)
		Return String.Empty
	End Function

	''' <summary>btnDelSubSysInfoのクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnDelSubSysInfo_Click(fxEventArgs As FxEventArgs) As String
		' サブシステム情報の取得

		If Me.txtSubSysInfoKey.Text = "" Then
			' キーが無い場合、ハッシュテーブルごと削除
			Me.SubsysInfo(Me.txtSubSysID.Text).Clear()
		Else
			' キーが有る場合、キー毎に削除
			Me.SubsysInfo(Me.txtSubSysID.Text).Remove(Me.txtSubSysInfoKey.Text)
		End If

		Return String.Empty
	End Function

	#End Region

	#End Region
End Class
