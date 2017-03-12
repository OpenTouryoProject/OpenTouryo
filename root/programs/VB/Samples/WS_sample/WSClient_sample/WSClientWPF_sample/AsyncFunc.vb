'**********************************************************************************
'* ３層型 サンプル アプリ
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：AsyncFunc
'* クラス日本語名  ：サンプル アプリ 非同期処理クラス
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports WSIFType_sample

Imports Touryo.Infrastructure.Business.RichClient.Asynchronous
Imports Touryo.Infrastructure.Framework.Transmission

Public Class AsyncFunc
    Inherits MyBaseAsyncFunc

    ''' <summary>コンストラクタ</summary>
    ''' <param name="_this">WPFやWinFormの要素</param>
    Public Sub New(ByVal _this As Object)
        MyBase.New(_this)
    End Sub

    ''' <summary>サービスの論理名</summary>
    Public LogicalName As String = ""

    ''' <summary>非同期</summary>
    ''' <param name="param">引数</param>
    ''' <returns>結果</returns>
    ''' <remarks>
    ''' ここは副スレッドから実行されるので注意。
    ''' 非同期処理クラスに非同期処理を定義すると、
    ''' メンバ変数を引数として利用できる。
    ''' </remarks>
    Public Function btn6_Exec(ByVal param As Object) As Object
        ' 戻り値（キャスト）
        Dim testParameterValue As TestParameterValue = DirectCast(param, TestParameterValue)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 呼出し制御部品（スレッドセーフでないため副スレッド内で作る）
        Dim callCtrl As New CallController("")

        ' Invoke
        testReturnValue = DirectCast(callCtrl.Invoke(Me.LogicalName, testParameterValue), TestReturnValue)

        '' 非同期メッセージボックス表示のテスト
        'Dim dr As DialogResult = Me.ShowAsyncMessageBoxWin( _
        '    "メッセージ", "タイトル", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

        '' 非同期メッセージボックス表示のテスト（エラー）
        'System.Windows.MessageBoxResult mr = af.ShowAsyncMessageBoxWPF("メッセージ", "タイトル",
        ''System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information);

        ' 結果表示
        Return testReturnValue
    End Function
End Class
