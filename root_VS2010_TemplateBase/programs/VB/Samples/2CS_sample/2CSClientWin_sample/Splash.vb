'**********************************************************************************
'* サンプル アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Splash
'* クラス日本語名  ：スプラッシュ画面
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Imports System.Threading

Imports Touryo.Infrastructure.Public.Log

    ''' <summary>
    ''' スプラッシュ画面
    ''' </summary>
    ''' <remarks>
    ''' DOBON.NET > プログラミング道 > .NET Tips
    '''  >  フォーム >  スプラッシュウィンドウを表示する
    ''' http://dobon.net/vb/dotnet/form/splashwindow.html
    ''' 
    ''' ここでは、フレームワークは使用しない。
    ''' </remarks>
    Partial Public Class Splash
        Inherits Form
        ''' <summary>コンストラクタ</summary>
        Public Sub New()
            InitializeComponent()

            ' プロパティの初期化
            Me.TopMost = True
            Me.ShowInTaskbar = False
            Me.FormBorderStyle = FormBorderStyle.None
            Me.StartPosition = FormStartPosition.CenterScreen

            ' イベントの設定
            AddHandler Me.Click, New System.EventHandler(AddressOf Splash.Splash_Click)
            AddHandler Me.label1.Click, New System.EventHandler(AddressOf Splash.Splash_Click)

            ' ログの初期化
            LogIF.InfoLog("ACCESS", "Splash")
        End Sub

        ''' <summary>
        ''' スプラッシュ画面のクリックイベント
        ''' </summary>
        Private Shared Sub Splash_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' 副スレッド（スプラッシュ画面を生成したスレッド）

            ' スピンロックを終了させ次画面を表示する。
            Splash._spinLock = True
        End Sub

#Region "静的変数"

        ''' <summary>実行済みフラグ</summary>
        Private Shared _hasExecuted As Boolean = False

        ''' <summary>スプラッシュ画面表示スレッド</summary>
        Private Shared _thread As Thread = Nothing

        ''' <summary>スピンロック用フラグ</summary>
        ''' <remarks>volatile:スレッドセーフ</remarks>
        Private Shared _spinLock As Boolean = False

        ''' <summary>スピンロック用フラグ（Getter）</summary>
        Public Shared ReadOnly Property SpinLock() As Boolean
            Get
                Return Splash._spinLock
            End Get
        End Property

        ''' <summary>スプラッシュ画面（シングルトン）</summary>
        ''' <remarks>volatile:スレッドセーフ</remarks>
        Private Shared _splashForm As New Splash()

        ''' <summary>次の画面（シングルトン）</summary>
        ''' <remarks>volatile:スレッドセーフ</remarks>
        Private Shared _nextForm As Form = Nothing

        ''' <summary>次の画面（Getter）</summary>
        Public Shared ReadOnly Property NextForm() As Form
            Get
                Return Splash._nextForm
            End Get
        End Property

#End Region

#Region "スプラッシュ画面を表示する"

        ''' <summary>スプラッシュ画面を表示する</summary>
        ''' <param name="nextForm">次の画面</param>
        Public Shared Sub ShowSplash(ByVal nextForm As Form)
            ' 主スレッド（スプラッシュ画面を生成していないスレッド）

            ' 二回以上は起動できない。
            If Splash._hasExecuted Then
                Return
            Else
                Splash._hasExecuted = True
            End If

            '#Region "スプラッシュ画面を表示"

            ' 次の画面を設定する。
            Splash._nextForm = nextForm

            ' スレッドの作成
            Splash._thread = New Thread(New ThreadStart(AddressOf ShowSplashByThread))

            ' スレッドの開始
            Splash._thread.Start()

            '#End Region
        End Sub

        ''' <summary>Thread関数でスプラッシュ画面を表示する。</summary>
        Private Shared Sub ShowSplashByThread()
            ' 副スレッド（スプラッシュ画面を生成したスレッド）

            ' スプラッシュ画面を

            ' ・作成
            Splash._splashForm = New Splash()

            ' ・閉じるイベントハンドラを仕掛
            AddHandler Splash._nextForm.Activated, New EventHandler(AddressOf Splash.Login_Activated)

            ' ・表示
            Application.Run(Splash._splashForm)
        End Sub

#End Region

#Region "スプラッシュ画面を閉じる"

        ''' <summary>
        ''' ログイン画面がアクティブになった時、スプラッシュ画面を閉じる
        ''' </summary>
        Private Shared Sub Login_Activated(ByVal sender As Object, ByVal e As EventArgs)
            ' 主スレッド（スプラッシュ画面を生成していないスレッド）

            ' なので、スプラッシュ画面を閉じるメソッドをInvoke
            If Splash._splashForm IsNot Nothing AndAlso Not Splash._splashForm.IsDisposed Then
                Splash._splashForm.Invoke(New MethodInvoker(AddressOf Splash.CloseSplash))
            End If

            ' nullクリア
            Splash._splashForm = Nothing
            Splash._nextForm = Nothing
            Splash._thread = Nothing
        End Sub

        ''' <summary>スプラッシュ画面を閉じる。</summary>
        Private Shared Sub CloseSplash()
            ' 副スレッド（スプラッシュ画面を生成したスレッド）

            ' なので、スプラッシュ画面をそのまま閉じる
            Splash._splashForm.Close()
        End Sub

#End Region
    End Class
