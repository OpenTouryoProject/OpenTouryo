'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testScreen0
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

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util

Namespace Aspx.TestFxLayerP.Normal
    ''' <summary>例外テスト画面（Ｐ層）</summary>
    Partial Public Class testScreen0
        Inherits MyBaseController
        ''' <summary>不正操作防止機能の局所化</summary>
        Private Sub Page_Init(sender As Object, e As EventArgs)

            ' OFFの場合、当該画面だけ、不正操作防止機能をONにする。
            Me.CanCheckIllegalOperation = True

            For Each key As String In Request.Form.Keys
                If key.IndexOf("btnIllegalOperationCheckOFF") <> -1 Then
                    ' btnIllegalOperationCheckOFFButtonによりサブミットされた。
                    Me.CanCheckIllegalOperation = False
                End If

                If key.IndexOf("btnIllegalOperationCheckON") <> -1 Then
                    ' btnIllegalOperationCheckONButtonによりサブミットされた。
                    Me.CanCheckIllegalOperation = True
                End If
            Next
        End Sub

#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:
        End Sub

#End Region

#Region "Content Page上のフレームワーク対象Control"

#Region "例外処理"

        ''' <summary>
        ''' btnAppExのClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnAppEx_Click(fxEventArgs As FxEventArgs) As String
            ' 業務例外のスロー
            Throw New BusinessApplicationException("E0001", "システム", "停止")
        End Function

        ''' <summary>
        ''' btnSysExのClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnSysEx_Click(fxEventArgs As FxEventArgs) As String
            ' システム例外
            Throw New BusinessSystemException("xxxxx", "P層でスローしたシステム例外")
        End Function

        ''' <summary>
        ''' btnElseExのClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnElseEx_Click(fxEventArgs As FxEventArgs) As String
            ' システム例外
            Throw New Exception("P層でスローしたその他、一般的な例外")
        End Function

#End Region

#Region "ユーザ情報のハンドル"

#Region "キー無し"


        ''' <summary>btnSetUserInfoのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnSetUserInfo_Click(fxEventArgs As FxEventArgs) As String
            ' ユーザ情報を設定
            UserInfoHandle.SetUserInformation(New MyUserInfo(Me.txtUserName.Text, Request.UserHostAddress))
            Return String.Empty
        End Function

        ''' <summary>btnGetUserInfoのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
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

        ''' <summary>btnUpdUserInfoのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
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

        ''' <summary>btnDelUserInfoのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnDelUserInfo_Click(fxEventArgs As FxEventArgs) As String
            ' ユーザ情報を削除
            UserInfoHandle.DeleteUserInformation()
            Return String.Empty
        End Function

#End Region

#End Region

#Region "サブシステム情報のハンドル"

        ''' <summary>btnSetSubSysInfoのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnSetSubSysInfo_Click(fxEventArgs As FxEventArgs) As String
            ' サブシステム情報の設定
            Me.SubsysInfo(Me.txtSubSysID.Text)(Me.txtSubSysInfoKey.Text) = Me.txtSubSysInfo.Text
            Return String.Empty
        End Function

        ''' <summary>btnGetSubSysInfoのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnGetSubSysInfo_Click(fxEventArgs As FxEventArgs) As String
            ' サブシステム情報の取得
            Me.lblSubSysInfo.Text = DirectCast(Me.SubsysInfo(Me.txtSubSysID.Text)(Me.txtSubSysInfoKey.Text), String)
            Return String.Empty
        End Function

        ''' <summary>btnDelSubSysInfoのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
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
End Namespace
