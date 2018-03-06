'**********************************************************************************
'* フレームワーク・テスト UI（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：SampleChildControl
'* クラス日本語名  ：UserControl上のEvent Handlerをハンドルする（ネスト）。
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

Namespace Aspx.Common.Wuc
    ''' <summary>SampleChildControl</summary>
    Partial Public Class SampleChildControl
        Inherits UserControl
        ''' <summary>User ControlにEvent Handlerを実装可能にしたのでそのテスト（ネスト）。</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnUCChildButton_Click(fxEventArgs As FxEventArgs) As String
            Me.lblUCChildResult.Text = "UOC_btnUCChildButton_Clickを実行できた。"
            Return ""
        End Function
    End Class
End Namespace
