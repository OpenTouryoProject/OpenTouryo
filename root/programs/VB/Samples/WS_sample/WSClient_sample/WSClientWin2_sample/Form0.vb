'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Form0
'* クラス日本語名  ：サンプル アプリ画面
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Business.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Presentation

''' <summary>Form0</summary>
Partial Public Class Form0
    Inherits MyBaseControllerWin

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>フォームロードのUOCメソッド</summary>
    Protected Overrides Sub UOC_FormInit()
    End Sub

    ''' <summary>Form1を表示</summary>
    Private Sub UOC_btnOpenForm1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Dim f As Form = New Form1()
        f.Show()
    End Sub

    ''' <summary>Form3を表示</summary>
    Private Sub UOC_btnOpenForm3_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Dim f As Form = New Form3()
        f.Show()
    End Sub

    ''' <summary>自分を閉じる</summary>
    Private Sub UOC_btnClose_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.Close()
    End Sub
End Class
