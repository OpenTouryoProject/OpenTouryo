'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testBlankScreenNoJs
'* クラス日本語名  ：ブランクのMaster Page
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

Namespace Aspx.Common.Master
    ''' <summary>ブランクのMaster Page</summary>
    Partial Public Class testBlankScreenNoJs
        Inherits BaseMasterController
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
