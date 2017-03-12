'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Form2
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

''' <summary>Form2</summary>
Partial Public Class Form2
    Inherits MyBaseControllerWin

    ''' <summary>テストのため画面を識別するID</summary>
    Public ID As String = ""

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()

        ' テストのため画面を識別するIDを付与する。
        Me.ID = Guid.NewGuid().ToString()
    End Sub

    ''' <summary>フォームロードのUOCメソッド</summary>
    Protected Overrides Sub UOC_FormInit()
        'base.UOC_FormInit();

        ' 画面数とIDを画面に表示する。
        Me.txtStatus.Text = String.Format("現在 {0}枚目の表示", MyBaseControllerWin.GetWindowsCount(Me.[GetType]()))

        Me.txtGuid.Text = String.Format("画面を識別するID:{0}", Me.ID)
    End Sub

    ''' <summary>Formを識別するIDをリストする</summary>
    Protected Sub UOC_btnFormList_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Dim temp As String = ""

        ' 当該Formインスタンスリストを取得する。
        Dim fl As List(Of Form) = MyBaseControllerWin.GetWindowInstances(Me.[GetType]())

        ' 表示する文字列を作成する。
        For Each f2 As Form2 In fl
            temp += "・" & f2.ID & vbCrLf
        Next

        ' メッセージボックスにリストする。
        MessageBox.Show(temp, "Form2のID一覧", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>全Formインスタンス数を表示する</summary>
    Protected Sub UOC_btnFormCount_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' メッセージボックスに表示する。
        MessageBox.Show(BaseControllerWin.GetWindowsCount().ToString(), "全Formインスタンス数", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>自分を閉じる</summary>
    Protected Sub UOC_btnClose_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.Close()
    End Sub

End Class
