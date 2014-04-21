'**********************************************************************************
'* サンプル
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_Framework_myYesNoMessageDialog
'* クラス日本語名  ：「YES」・「NO」メッセージ・ダイアログ（サンプル ※ プロジェクト毎、必要に応じて改修）
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
Imports System.Data
Imports System.Configuration
Imports System.Collections

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Exceptions

' 部品
Imports Touryo.Infrastructure.Public.Util
Imports Touryo.Infrastructure.Public.Log

''' <summary>「YES」・「NO」メッセージ・ダイアログ</summary>
''' <remarks>サンプル ※ プロジェクト毎、必要に応じて改修</remarks>
Partial Public Class Aspx_Framework_myYesNoMessageDialog
    Inherits System.Web.UI.Page
    ''' <summary>初期化処理</summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        ' 親画面の画面GUIDを設定（QueryString）から取得
        Dim parentScreenGuid As String = DirectCast(Request.QueryString(FxHttpQueryStringIndex.PARENT_SCREEN_GUID), String)

        ' メッセージIDとメッセージをセッションより取得し、設定
        Me.lblmessage.Text = DirectCast(Session(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGE), String)

        Me.lblmessageID.Text = DirectCast(Session(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGEID), String)

        ' タイトル設定
        Me.Title = DirectCast(Session(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_NAME), String)

        ' アイコンへのパスを取得
        Dim iconPath As String

        ' 選択入力を促すアイコン（？）を設定
        iconPath = GetConfigParameter.GetConfigValue(FxLiteral.QUESTION_ICON_PATH)

        ' エラー処理
        If iconPath = "" Then
            Throw New FrameworkException(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1(0), [String].Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1(1), FxLiteral.QUESTION_ICON_PATH))
        End If

        Me.imgIcon.ImageUrl = iconPath

        ' 表示の完了後、セッションを消去する。
        Session.Remove(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGE)
        Session.Remove(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGEID)
        Session.Remove(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_NAME)

        ' ACCESSログ出力 ----------------------------------------------

        ' ------------
        ' メッセージ部
        ' ------------
        ' ユーザ名, IPアドレス,
        ' レイヤ, 画面名, コントロール名, 処理名
        ' 処理時間（実行時間）, 処理時間（CPU時間）
        ' エラーメッセージID, エラーメッセージ等
        ' ------------
        Dim strLogMessage As String = "," + Me.GetUserInfo().UserName + "," + Request.UserHostAddress + ",init" + ",YesNoMessageDialog" + ","

        ' Log4Netへログ出力
        LogIF.InfoLog("ACCESS", strLogMessage)
    End Sub

    ''' <summary>ユーザ情報を取得する</summary>
    ''' <returns>ユーザ情報</returns>
    Private Function GetUserInfo() As MyUserInfo
        Dim ui As MyUserInfo = DirectCast(UserInfoHandle.GetUserInformation(), MyUserInfo)

        ' 再取得する。
        If ui Is Nothing Then
            ' Cookie認証チケット
            Dim authCookie As HttpCookie = Context.Request.Cookies("formauth")

            If authCookie Is Nothing Then
                ' 認証チケットがない場合
                ' ダミーのユーザ情報を設定する。
                ui = New MyUserInfo("未認証", Request.UserHostAddress)
            Else
                ' 認証チケットがある場合
                ' ユーザ情報を再取得する。

                ' 認証チケット
                Dim authTicket As FormsAuthenticationTicket = FormsAuthentication.Decrypt(authCookie.Value)

                ' ユーザ名を復元
                ui = New MyUserInfo(authTicket.Name, Request.UserHostAddress)
            End If
        End If

        Return ui
    End Function
End Class
