'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：ByReturn
'* クラス日本語名  ：初期化画面
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
Imports Touryo.Infrastructure.Business.RichClient.Asynchronous
Imports Touryo.Infrastructure.Framework.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous

''' <summary>ByReturn</summary>
Partial Public Class ByReturn
    Inherits MyBaseControllerWin

    ''' <summary>非同期実行クラス</summary>
    Dim Af As MyBaseAsyncFunc = Nothing

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()

        Program.FlagEnd = True ' フラグ初期化
    End Sub

    ''' <summary>最大回数</summary>
    Dim Max As Integer = 5

    ''' <summary>現在の折り返し処理回数</summary>
    Private Current As Integer = 1

    ''' <summary>フォームロードのUOCメソッド</summary>
    Protected Overrides Sub UOC_FormInit()

        Me.Af = New MyBaseAsyncFunc(Me)

        ' 非同期処理本体デレゲード
        Me.Af.AsyncFunc = New BaseAsyncFunc.AsyncFuncDelegate(AddressOf MyAsyncFunc)

        ' 進捗報告デレゲード
        Me.Af.ChangeProgress = New BaseAsyncFunc.ChangeProgressDelegate(AddressOf MyChangeProgress)

        ' 結果設定デレゲード
        Me.Af.SetResult = New BaseAsyncFunc.SetResultDelegate(AddressOf MySetResult)

        If Me.Af.Start() Then
            '正常に実行
            Me.label1.Text = String.Format("処理中です・・・:{0}/{1}", Me.Current.ToString(), Me.Max.ToString())
        Else
            ' ここは通らないが念のため
            Me.label1.Text = String.Format("非同期スレッドが最大数に達しています。:{0}", BaseAsyncFunc.ThreadCount.ToString())
        End If
    End Sub

    ''' <summary>開始</summary>
    Private Sub UOC_btnStart_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Program.FlagEnd = False ' フラグ完了
        Me.Close()
    End Sub

    ''' <summary>非同期処理本体デレゲード</summary>
    ''' <param name="param">引数</param>
    Function MyAsyncFunc(ByVal param As Object)

        Dim wait As Integer = 1

        Me.Current = 1
        While Me.Current <= Max
            ' ダミー
            System.Threading.Thread.Sleep(wait * 1000)

            ' 進捗表示
            Af.ExecChangeProgress(String.Format("処理中です・・・:{0}/{1}", Me.Current.ToString(), Me.Max.ToString()))
            Me.Current += 1
        End While

        Return "処理が完了しました。"
    End Function

    ''' <summary>進捗報告デレゲード</summary>
    ''' <param name="param">引数</param>
    Sub MyChangeProgress(ByVal param As Object)
        Me.label1.Text = DirectCast(param, String)
    End Sub

    ''' <summary>結果設定デレゲード</summary>
    ''' <param name="retVal">引数</param>
    Sub MySetResult(ByVal retVal As Object)
        If TypeOf retVal Is Exception Then
            ' 例外発生時
            Me.label1.Text = TryCast(retVal, Exception).ToString()
        Else
            Me.label1.Text = DirectCast(retVal, String)
            Me.btnStart.Visible = True
        End If
    End Sub

End Class
