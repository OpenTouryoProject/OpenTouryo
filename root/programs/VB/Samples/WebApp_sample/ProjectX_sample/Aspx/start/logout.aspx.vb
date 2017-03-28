'**********************************************************************************
'* サンプル画面（Ｐ層）
'**********************************************************************************

' サンプル画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：logout
'* クラス日本語名  ：ログアウト画面（Forms認証対応）
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

Namespace Aspx.Start
    ''' <summary>ログアウト画面（Forms認証対応）</summary>
    Partial Public Class logout
        Inherits MyBaseController
#Region "Page LoadのUOCメソッド"

        ''' <summary>
        ''' Page LoadのUOCメソッド（個別：初回Load）
        ''' </summary>
        ''' <remarks>
        ''' 実装必須
        ''' </remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する

            ' TODO:

            ' ログオフ（認証チケットを削除する）
            FormsAuthentication.SignOut()

            ' ログインページ（login.aspx）に遷移する。
            Response.Redirect("login.aspx")
        End Sub

        ''' <summary>
        ''' Page LoadのUOCメソッド（個別：Post Back）
        ''' </summary>
        ''' <remarks>
        ''' 実装必須
        ''' </remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する

            ' TODO:
            ' ここでは何もしない
        End Sub

#End Region
    End Class
End Namespace
