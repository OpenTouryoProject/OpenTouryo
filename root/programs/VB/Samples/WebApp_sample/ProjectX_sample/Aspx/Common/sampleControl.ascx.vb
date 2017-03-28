'**********************************************************************************
'* フレームワーク・テスト UI（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：WebUserControl
'* クラス日本語名  ：WebUserControl上のEvent Handlerをハンドルする。
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Framework.Presentation

Namespace Aspx.Common
    ''' <summary>WebUserControl class</summary>
    Partial Public Class WebUserControl
        Inherits System.Web.UI.UserControl
        ''' <summary>User ControlにEvent Handlerを実装可能にしたのでそのテスト。</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnUCButton_Click(fxEventArgs As FxEventArgs) As String
            Me.lblResult.Text = "UOC_btnUCButton_Clickを実行できた。"
            Return ""
        End Function
    End Class
End Namespace
