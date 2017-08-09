'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：WebForm0
'* クラス日本語名  ：画面遷移制御機能テスト画面０（Ｐ層）
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
Imports Touryo.Infrastructure.Framework.Util

Namespace Aspx.TestScreenCtrl
    ''' <summary>画面遷移制御機能テスト画面０（Ｐ層）</summary>
    Partial Public Class WebForm0
        Inherits MyBaseController
        ' 部品使用する・しないフラグ
        Private IsFx As Boolean = False

#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:

            ' 状態の通知
            Dim lblStatus As Label = DirectCast(Me.GetMasterWebControl("lblStatus"), Label)

            If Request.HttpMethod.ToUpper() = "GET" Then
                lblStatus.Text = "これは、Redirectによる遷移"
            ElseIf Request.HttpMethod.ToUpper() = "POST" Then
                lblStatus.Text = "これは、Transferによる遷移"
            Else
                lblStatus.Text = "不明な遷移"
            End If

            ' ---

            ' QueryStringの通知
            Dim lblQueryString As Label = DirectCast(Me.GetMasterWebControl("lblQueryString"), Label)

            For Each qsKey As String In Request.QueryString.AllKeys
                lblQueryString.Text += (qsKey & "=") + Request.QueryString(qsKey) & ";"
            Next
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:

            ' 状態の通知
            Dim lblStatus As Label = DirectCast(Me.GetMasterWebControl("lblStatus"), Label)
            lblStatus.Text = "これは、ポスト（Post Backです）"

            ' ---

            ' Fxを使用するモード
            If DirectCast(Me.GetContentWebControl("CheckBox1"), CheckBox).Checked Then
                Me.IsFx = True
            End If
        End Sub

#End Region

#Region "Master Page上のフレームワーク対象Control"

#Region "チェック可能な画面遷移（外サイトへ）"

        ''' <summary>
        ''' ibnMImageButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testScreenCtrl_ibnMImageButton1_Click(fxEventArgs As FxEventArgs) As String
            ' 外部サイトへ（QueryString無し）
            Return "google"
        End Function

#End Region

#End Region

#Region "Content Page上のフレームワーク対象Control"

#Region "チェック可能な画面遷移"

        ''' <summary>btnButton1のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>画面遷移可能（○）</remarks>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            Return "0→1?testPN2=testPV2"
        End Function

        ''' <summary>btnButton2のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>画面遷移可能（○）</remarks>
        Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
            Return "0→2?testPN2=testPV2"
        End Function

        ''' <summary>btnButton3のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>画面遷移可能（○）</remarks>
        Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
            Return "0→3?testPN2=testPV2"
        End Function

        ''' <summary>btnButton3のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>画面遷移可能（○）</remarks>
        Protected Function UOC_btnButton4_Click(fxEventArgs As FxEventArgs) As String
            Return "0→4?testPN2=testPV2"
        End Function

        ''' <summary>btnButton3のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>画面遷移可能（○）</remarks>
        Protected Function UOC_btnButton5_Click(fxEventArgs As FxEventArgs) As String
            Return "0→5?testPN2=testPV2"
        End Function

#End Region

#Region "チェック不可能な画面遷移"

#Region "Transfer or FxTransfer"

        ''' <summary>btnButton6のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
        Protected Function UOC_btnButton6_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxTransfer("./WebForm1.aspx?testPN2=testPV2")
            Else
                Server.Transfer("./WebForm1.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton7のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
        Protected Function UOC_btnButton7_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxTransfer("./WebForm2.aspx?testPN2=testPV2")
            Else
                Server.Transfer("./WebForm2.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton8のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
        Protected Function UOC_btnButton8_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxTransfer("./WebForm3.aspx?testPN2=testPV2")
            Else
                Server.Transfer("./WebForm3.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton9のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
        Protected Function UOC_btnButton9_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxTransfer("./WebForm4.aspx?testPN2=testPV2")
            Else
                Server.Transfer("./WebForm4.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton10のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
        Protected Function UOC_btnButton10_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxTransfer("./WebForm5.aspx?testPN2=testPV2")
            Else
                Server.Transfer("./WebForm5.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

#End Region

#Region "Redirect or FxRedirect"

        ''' <summary>btnButton11のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
        Protected Function UOC_btnButton11_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxRedirect("./WebForm1.aspx?testPN2=testPV2")
            Else
                Response.Redirect("./WebForm1.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton12のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
        Protected Function UOC_btnButton12_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxRedirect("./WebForm2.aspx?testPN2=testPV2")
            Else
                Response.Redirect("./WebForm2.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton13のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
        Protected Function UOC_btnButton13_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxRedirect("./WebForm3.aspx?testPN2=testPV2")
            Else
                Response.Redirect("./WebForm3.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton14のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
        Protected Function UOC_btnButton14_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxRedirect("./WebForm4.aspx?testPN2=testPV2")
            Else
                Response.Redirect("./WebForm4.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

        ''' <summary>btnButton15のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
        Protected Function UOC_btnButton15_Click(fxEventArgs As FxEventArgs) As String
            If Me.IsFx Then
                Me.FxRedirect("./WebForm5.aspx?testPN2=testPV2")
            Else
                Response.Redirect("./WebForm5.aspx?testPN2=testPV2")
            End If

            Return ""
        End Function

#End Region

#End Region

#Region "Post Back"

        ''' <summary>btnButton16のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
        Protected Function UOC_btnButton16_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowOKMessageDialog("Post Backの", "テストです", FxEnum.IconType.Information, "テスト")
            Return ""
        End Function

#End Region

#Region "子画面の表示"

#Region "window open"

        ''' <summary>btnButton17のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>window open</remarks>
        Protected Function UOC_btnButton17_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowNormalScreen("./WebForm1.aspx")
            Return ""
        End Function

        ''' <summary>btnButton18のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>window open</remarks>
        Protected Function UOC_btnButton18_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowNormalScreen("./WebForm2.aspx")
            Return ""
        End Function

        ''' <summary>btnButton19のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>window open</remarks>
        Protected Function UOC_btnButton19_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowNormalScreen("./WebForm3.aspx")
            Return ""
        End Function

        ''' <summary>btnButton20のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>window open</remarks>
        Protected Function UOC_btnButton20_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowNormalScreen("./WebForm4.aspx")
            Return ""
        End Function

        ''' <summary>btnButton21のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>window open</remarks>
        Protected Function UOC_btnButton21_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowNormalScreen("./WebForm5.aspx")
            Return ""
        End Function

#End Region

#Region "dialog"

        ''' <summary>btnButton22のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>dialog</remarks>
        Protected Function UOC_btnButton22_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm1.aspx")
            Return ""
        End Function

        ''' <summary>btnButton23のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>dialog</remarks>
        Protected Function UOC_btnButton23_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm2.aspx")
            Return ""
        End Function

        ''' <summary>btnButton24のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>dialog</remarks>
        Protected Function UOC_btnButton24_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm3.aspx")
            Return ""
        End Function

        ''' <summary>btnButton25のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>dialog</remarks>
        Protected Function UOC_btnButton25_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm4.aspx")
            Return ""
        End Function

        ''' <summary>btnButton26のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>dialog</remarks>
        Protected Function UOC_btnButton26_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm5.aspx")
            Return ""
        End Function

#End Region

#End Region

#Region "ブラウザ ウィンドウ別セッション領域"

        ''' <summary>btnButton27のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton27_Click(fxEventArgs As FxEventArgs) As String
            ' ブラウザ ウィンドウ別セッション領域 - 設定
            Me.SetDataToBrowserWindow("msg", Me.TextBox1.Text)
            Return ""
        End Function

        ''' <summary>btnButton28のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton28_Click(fxEventArgs As FxEventArgs) As String
            ' ブラウザ ウィンドウ別セッション領域 - 取得
            Me.TextBox1.Text = DirectCast(Me.GetDataFromBrowserWindow("msg"), String)
            Return ""
        End Function

#End Region

        ''' <summary>btnButton29のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton29_Click(fxEventArgs As FxEventArgs) As String
            ' 画面遷移（ScreenTransition）
            Dim txt As TextBox = DirectCast(Me.GetContentWebControl("TextBox2"), TextBox)
            Me.ScreenTransition(txt.Text)

            Return ""
        End Function

        ''' <summary>btnButton30のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton30_Click(fxEventArgs As FxEventArgs) As String
            ' 画面遷移（FxTransfer）
            Dim txt As TextBox = DirectCast(Me.GetContentWebControl("TextBox2"), TextBox)
            Me.FxTransfer(txt.Text)

            Return ""
        End Function

        ''' <summary>btnButton31のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton31_Click(fxEventArgs As FxEventArgs) As String
            ' 画面遷移（FxRedirect）
            Dim txt As TextBox = DirectCast(Me.GetContentWebControl("TextBox2"), TextBox)
            Me.FxRedirect(txt.Text)

            Return ""
        End Function

        ''' <summary>btnButton32のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton32_Click(fxEventArgs As FxEventArgs) As String
            ' 画面遷移（Transfer）
            Dim txt As TextBox = DirectCast(Me.GetContentWebControl("TextBox2"), TextBox)
            Server.Transfer(txt.Text)

            Return ""
        End Function

        ''' <summary>btnButton33のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton33_Click(fxEventArgs As FxEventArgs) As String
            ' 画面遷移（Redirect）
            Dim txt As TextBox = DirectCast(Me.GetContentWebControl("TextBox2"), TextBox)
            Response.Redirect(txt.Text)

            Return ""
        End Function

#End Region
    End Class

End Namespace
