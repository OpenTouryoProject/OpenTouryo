'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：sampleScreen
'* クラス日本語名  ：サンプル画面用のMaster Page
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util

Namespace Aspx.Common
    ''' <summary>サンプル画面用のMaster Page</summary>
    Partial Public Class sampleScreen
        Inherits BaseMasterController
        ''' <summary>Master PageにEvent Handlerを実装可能にしたのでそのテスト。</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnMPButton_Click(fxEventArgs As FxEventArgs) As String
            Response.Write("UOC_btnMPButton_Clickを実行できた。")

            Return ""
        End Function

        ''' <summary>UserName</summary>
        Public ReadOnly Property UserName() As String
            Get
                Dim user = DirectCast(UserInfoHandle.GetUserInformation(), MyUserInfo)

                If user Is Nothing Then
                    Return "anonymous"
                Else
                    Return user.UserName
                End If
            End Get
        End Property
    End Class
End Namespace
