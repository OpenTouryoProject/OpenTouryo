'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Form1
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

Imports System.Threading

Imports Touryo.Infrastructure.Business.RichClient.Presentation
Imports Touryo.Infrastructure.Business.RichClient.Asynchronous
Imports Touryo.Infrastructure.Framework.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous

''' <summary>Form1</summary>
Partial Public Class Form1
    Inherits MyBaseControllerWin

    ''' <summary>非同期実行クラス</summary>
    Dim Af As MyBaseAsyncFunc = Nothing

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>フォームロードのUOCメソッド</summary>
    Protected Overrides Sub UOC_FormInit()
        Me.numericUpDown1.Value = 5
        Me.comboBox1.SelectedIndex = 0
    End Sub

    ''' <summary>同期実行</summary>
    Protected Sub UOC_btnSync_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Dim wait As Integer = CInt(Math.Truncate(Me.numericUpDown1.Value))

        Me.AddStatus(String.Format("主スレッド実行中: {0}秒待つ", wait))

        Thread.Sleep(wait * 1000)

        Me.AddStatus("スレッド実行終了")

        ' 結果表示のテスト
        Me.TestOfResultDisplay()
    End Sub

    ''' <summary>非同期実行</summary>
    Protected Sub UOC_btnASync_Click(ByVal rcFxEventArgs As RcFxEventArgs)

        Me.Af = New MyBaseAsyncFunc(Me)

        ' 非同期処理本体デレゲード
        Me.Af.AsyncFunc = New BaseAsyncFunc.AsyncFuncDelegate(AddressOf MyAsyncFunc)

        ' 進捗報告デレゲード
        Me.Af.ChangeProgress = New BaseAsyncFunc.ChangeProgressDelegate(AddressOf MyChangeProgress)

        ' 結果設定デレゲード
        Me.Af.SetResult = New BaseAsyncFunc.SetResultDelegate(AddressOf MySetResult)

        ' 非同期処理を開始させる。
        If Af.Start() Then
            Me.AddStatus(String.Format("キューイングされました、現在のスレッド数:{0}", BaseAsyncFunc.ThreadCount.ToString()))
        Else
            Me.AddStatus(String.Format("非同期スレッドが最大数に達しています。:{0}", BaseAsyncFunc.ThreadCount.ToString()))
        End If
    End Sub

    ''' <summary>テキストボックスに書き込み</summary>
    ''' <param name="text">追加するテキスト</param>
    Private Sub AddStatus(ByVal text As String)
        Me.txtStatus.Text = String.Format("{0}{1}" & vbCrLf, Me.txtStatus.Text, text)
    End Sub

    ''' <summary>画面起動</summary>
    Protected Sub UOC_btnOpenForm2_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        If MyBaseControllerWin.GetWindowsCount(GetType(Form2)) > 3 Then
            MessageBox.Show("５画面以上は起動できません。")
        Else
            Dim f As New Form2()
            f.Show()

            '' イベント二重登録対策のテスト
            'f.ShowDialog()
            'f.Hide()
            'f.ShowDialog()
            '' ココで、Form2のイベントが2回実行される。
            'f.Hide()

        End If
    End Sub

    ''' <summary>非同期処理本体デレゲード</summary>
    ''' <param name="param">引数</param>
    Function MyAsyncFunc(ByVal param As Object) As Object

        Dim wait As Integer = CInt(Math.Truncate(Me.numericUpDown1.Value))

        ' 進捗報告
        Me.Af.ExecChangeProgress(String.Format("スレッド実行中: {0}秒待つ", wait))

        System.Threading.Thread.Sleep(wait * 1000)

        Return "終わり"
    End Function

    ''' <summary>進捗報告デレゲード</summary>
    ''' <param name="param">引数</param>
    Private Sub MyChangeProgress(ByVal param As Object)
        Dim text As String = DirectCast(param, String)
        Me.AddStatus(text)
    End Sub

    ''' <summary>結果設定デレゲード</summary>
    ''' <param name="retVal">引数</param>
    Private Sub MySetResult(ByVal retVal As Object)
        If TypeOf retVal Is Exception Then
            ' 例外発生時
            Dim ex As Exception = DirectCast(retVal, Exception)
            Me.AddStatus(String.Format("スレッド実行終了: エラー発生:{0}", ex.Message))
        Else
            Me.AddStatus("スレッド実行終了")
        End If

        ' 結果表示のテスト
        Me.TestOfResultDisplay()
    End Sub

    ''' <summary>結果表示のテスト</summary>
    Private Sub TestOfResultDisplay()
        If cbxWindow.Checked Then
            ' ダイアログの表示
            Dim f As New Form2()
            f.Show()
            Return
        ElseIf cbxDialog.Checked Then
            ' フォームの表示
            Dim f As New Form2()
            f.ShowDialog()
            Return
        ElseIf cbxClick.Checked Then
            ' PerformClickは動作しない。
            Me.btnButton1.PerformClick()
            Return
        ElseIf cbxDoClick.Checked OrElse cbxDoClick2.Checked Then
            ' DoClickは動作する。
            Me.btnHdnBtn1.DoClick()
            Return
        End If
    End Sub

    ''' <summary>メソッド実装あり</summary>
    Protected Sub UOC_btnButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        MessageBox.Show("UOC_btnButton1_Click")
    End Sub

    ''' <summary>隠しボタンのイベント実装</summary>
    Protected Sub UOC_btnHdnBtn1_Click(ByVal rcFxEventArgs As RcFxEventArgs)

        MessageBox.Show("UOC_btnHdnBtn1_Click")

        If cbxDoClick2.Checked Then

            Me.Af = New MyBaseAsyncFunc(Me)

            ' 非同期処理本体デレゲード
            Me.Af.AsyncFunc = New BaseAsyncFunc.AsyncFuncDelegate(AddressOf MyAsyncFunc)

            ' 進捗報告デレゲード
            Me.Af.ChangeProgress = New BaseAsyncFunc.ChangeProgressDelegate(AddressOf MyChangeProgress)

            ' 結果設定デレゲード
            Me.Af.SetResult = New BaseAsyncFunc.SetResultDelegate(AddressOf MySetResult)

            ' 非同期処理を開始させる。
            If Af.Start() Then
                Me.AddStatus(String.Format("キューイングされました、現在のスレッド数:{0}", BaseAsyncFunc.ThreadCount.ToString()))
            Else
                Me.AddStatus(String.Format("非同期スレッドが最大数に達しています。:{0}", BaseAsyncFunc.ThreadCount.ToString()))
            End If

        End If

    End Sub

    ''' <summary>SetResultで動作するか確認する。</summary>
    Private Sub txtStatus_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Debug.WriteLine("txtStatus_TextChanged")
    End Sub

End Class