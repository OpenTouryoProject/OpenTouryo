'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testClientCallback
'* クラス日本語名  ：ClientCallbackのテスト画面（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Framework.Presentation

Namespace Aspx.TestFxLayerP.WithAjax
    ''' <summary>ClientCallbackのテスト画面（Ｐ層）</summary>
    Partial Public Class testClientCallback
        Inherits MyBaseController
        Implements System.Web.UI.ICallbackEventHandler
        ' 注意：System.Web.UI.ICallbackEventHandlerの実装のため必要になる。
#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:

            ' クライアント コールバックを有効にする
            ' Init、PostBackの双方で都度実行する必要がある。
            Dim CallServerScript As String = Me.InitClientCallback()

            ' イベント（ロスト フォーカス）にWebForm_DoCallbackを仕掛ける。

            ' スクリプトの編集
            CallServerScript = CallServerScript.Replace("$SvrParam$", "this.value")
            CallServerScript = CallServerScript.Replace("$ClientCallbackReceiveEventHandler$", "CCREH_Reverse")
            CallServerScript = CallServerScript.Replace("$CliParam$", "this.name")

            ' スクリプトの設定
            DirectCast(Me.GetMasterWebControl("TextBox1"), TextBox).Attributes.Add("onblur", CallServerScript)
            DirectCast(Me.GetContentWebControl("TextBox1"), TextBox).Attributes.Add("onblur", CallServerScript)
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:

            ' クライアント コールバックを有効にする
            ' Init、PostBackの双方で都度実行する必要がある。
            Dim CallServerScript As String = Me.InitClientCallback()
        End Sub

        ''' <summary>クライアント コールバックを有効にする。</summary>
        Private Function InitClientCallback() As String
            '/ 第一引数：サーバの画面インスタンス
            '/ 第二引数：WebForm_DoCallback（サーバ呼び出し）関数への引数
            '/           getElementById（javascript） ＋ Control.ClientIDなどを併用することもある。
            '/ 第三引数：コールバック関数
            '/ 第四引数：コールバック関数への引数
            '/           getElementById（javascript） ＋ Control.ClientIDなどを併用することもある。

            ' 第二引数、第三引数は可変にする。
            Return Me.ClientScript.GetCallbackEventReference(Me, "$SvrParam$", "$ClientCallbackReceiveEventHandler$", "$CliParam$")
        End Function

#End Region

        ''' <summary>btnButton1のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' なにもしない。
            Return ""
        End Function

#Region "ICallbackEventHandler メンバ"

        ' 処理結果を記憶する変数
        Private CallbackResult As String = ""

        ''' <summary>ClientCallbackにおいて、処理を実行するイベント ハンドラ</summary>
        ''' <param name="eventArgument"></param>
        Private Sub ICallbackEventHandler_RaiseCallbackEvent(eventArgument As String) Implements ICallbackEventHandler.RaiseCallbackEvent
            Dim sbr As New StringBuilder()

            For i As Integer = eventArgument.Length - 1 To 0 Step -1
                sbr.Append(eventArgument(i))
            Next

            ' 処理結果を設定する。
            Me.CallbackResult = sbr.ToString()


        End Sub

        ''' <summary>ClientCallbackにおいて、値を戻すイベント ハンドラ</summary>
        ''' <returns>処理結果</returns>
        Private Function ICallbackEventHandler_GetCallbackResult() As String Implements ICallbackEventHandler.GetCallbackResult
            '/ テスト用スリープ
            'System.Threading.Thread.Sleep(Me.SleepCnt);

            ' 処理結果を値を戻す。
            Return Me.CallbackResult
        End Function

#End Region
    End Class

End Namespace
