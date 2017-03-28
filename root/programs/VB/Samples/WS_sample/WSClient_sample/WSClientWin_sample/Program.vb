'**********************************************************************************
'* ３層型 サンプル アプリ
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Program
'* クラス日本語名  ：アプリケーションのメイン エントリ ポイント
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

Imports Touryo.Infrastructure.Business.RichClient.Util

Public Class Program

    Public Sub New()
    End Sub
    ''' <summary>終了するかどうかを表すフラグ</summary>
    Public Shared FlagEnd As Boolean = True

    ''' <summary>
    ''' アプリケーションのメイン エントリ ポイントです。
    ''' </summary>
    <STAThread()> _
    Public Shared Sub Main()
        ' 既定の処理
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' UnhandledExceptionイベント・ハンドラを登録する
        AddHandler Thread.GetDomain().UnhandledException, New UnhandledExceptionEventHandler(AddressOf Application_UnhandledException)

        ' ThreadExceptionイベント・ハンドラを登録する
        AddHandler Application.ThreadException, New ThreadExceptionEventHandler(AddressOf Application_ThreadException)

        ' スプラッシュ画面の表示
        Splash.ShowSplash(New Login())

        ' ＜スピンロック＞
        ' SleepすればCPUオーバヘッドはほとんど無いが
        ' Sleep時間を長く、ループ回数を短くする
        ' ことでよりCPUオーバヘッドを軽減できる。

        For i As Integer = 0 To 29
            If Splash.SpinLock Then
                ' 直ちに抜ける
                Exit For
            End If

            Thread.Sleep(100)
        Next

        ' ThreadExceptionイベント・ハンドラを登録する
        AddHandler Application.ThreadException, New ThreadExceptionEventHandler(AddressOf Application_ThreadException)

        ' 次の画面（ログイン画面）の表示
        Application.Run(Splash.NextForm)
        If Program.FlagEnd Then
            ' ログインしないで終わった場合
            Return
        End If

        ' ThreadExceptionイベント・ハンドラを登録する
        AddHandler Application.ThreadException, New ThreadExceptionEventHandler(AddressOf Application_ThreadException)

        ' 業務画面の表示（業務の開始）
        Application.Run(New Form1())
    End Sub

    ' .NET TIPS > 適切に処理されなかった例外をキャッチするには？
    ' http://www.atmarkit.co.jp/fdotnet/dotnettips/320appexception/appexception.html

    ''' <summary>
    ''' 未処理例外をキャッチするイベント・ハンドラ
    ''' </summary>
    Public Shared Sub Application_ThreadException(ByVal sender As Object, ByVal e As ThreadExceptionEventArgs)
        RcMyCmnFunction.ShowErrorMessageWin(e.Exception, "Application_ThreadExceptionによる例外通知です。")
    End Sub

    ''' <summary>
    ''' 未処理例外をキャッチするイベント・ハンドラ 
    ''' </summary>
    ''' <remarks>
    ''' メイン・スレッド以外の例外はUnhandledExceptionでハンドル
    ''' </remarks>
    Public Shared Sub Application_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim ex As Exception = TryCast(e.ExceptionObject, Exception)
        If ex IsNot Nothing Then
            RcMyCmnFunction.ShowErrorMessageWin(ex, "Application_UnhandledExceptionによる例外通知です。")
        End If
    End Sub
End Class
