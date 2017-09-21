'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：UserControl3
'* クラス日本語名  ：ユーザコントロール
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

Imports Touryo.Infrastructure.Framework.RichClient.Presentation

''' <summary>UserControlChild</summary>
Public Class UserControlChild

    ''' <summary>UOC_btnUCButton1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_btnUCButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_btnUCButton1_Click")
    End Sub

End Class
