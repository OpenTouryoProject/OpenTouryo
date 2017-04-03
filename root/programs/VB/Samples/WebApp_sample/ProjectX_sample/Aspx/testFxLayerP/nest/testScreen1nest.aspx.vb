'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testScreen1nest
'* クラス日本語名  ：テスト画面１のネスト版（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.IO

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Util

Namespace Aspx.TestFxLayerP.Nest
    ''' <summary>テスト画面１のネスト版（Ｐ層）</summary>
    Partial Public Class testScreen1nest
        Inherits MyBaseController
        ''' <summary>二重送信防止機能の確認用</summary>
        Private SleepCnt As Integer = 5000

#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:
            Response.Write(Me.ContentPageFileNoEx & "<br/>")

            ' QueryStringの通知
            Dim qs As String = ""
            For Each qsKey As String In Request.QueryString.AllKeys
                qs += (qsKey & "=") + Request.QueryString(qsKey) & ";"
            Next
            Response.Write(qs)
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:

        End Sub

#End Region

#Region "Master Page上のフレームワーク対象Control"

#Region "既存のルートのMaster Page上のフレームワーク対象Control"

#Region "基本処理"

        ''' <summary>
        ''' btnMButton21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_btnMButton21_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' lbnMLinkButton21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_lbnMLinkButton21_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' ibnMImageButton21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_ibnMImageButton21_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行 - " &
                "x:" & fxEventArgs.X.ToString() &
                ",y:" & fxEventArgs.Y.ToString(), FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' impMImageMap21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_impMImageMap21_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行 - " &
                "pbv:" & fxEventArgs.PostBackValue, FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "画面遷移処理"

        ''' <summary>
        ''' btnMButton22のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_btnMButton22_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' lbnMLinkButton22のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_lbnMLinkButton22_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' ibnMImageButton22のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_ibnMImageButton22_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' impMImageMap2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_impMImageMap22_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

#End Region

#Region "Control取得"

        ''' <summary>
        ''' btnMButton23のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_btnMButton23_Click(fxEventArgs As FxEventArgs) As String
            ' Controlを取得し
            Dim temp As Control = DirectCast(Me.GetFxWebControl(DirectCast(Me.GetMasterWebControl("TextBox4"), TextBox).Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' Message表示
                Me.ShowOKMessageDialog(
                    "GetFxWebControl",
                    "Controlを取得できませんでした。",
                    FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

        ''' <summary>
        ''' lbnMLinkButton23のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_lbnMLinkButton23_Click(fxEventArgs As FxEventArgs) As String
            ' Controlを取得し
            Dim temp As Control = DirectCast(Me.GetMasterWebControl(DirectCast(Me.GetMasterWebControl("TextBox4"), TextBox).Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' Message表示
                Me.ShowOKMessageDialog(
                    "GetMasterWebControl",
                    "Controlを取得できませんでした。",
                    FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

        ''' <summary>
        ''' ibnMImageButton23のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_ibnMImageButton23_Click(fxEventArgs As FxEventArgs) As String
            ' Controlを取得し
            Dim temp As Control = DirectCast(Me.GetContentWebControl(DirectCast(Me.GetMasterWebControl("TextBox4"), TextBox).Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' Message表示
                Me.ShowOKMessageDialog(
                    "GetContentWebControl",
                    "Controlを取得できませんでした。",
                    FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

#End Region

#Region "Dialog表示"

        ''' <summary>
        ''' btnMButton24のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_btnMButton24_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = DirectCast(Me.GetMasterWebControl("TextBox5"), TextBox).Text

            ' 受け渡しデータの設定
            Dim msg As String = DirectCast(Me.GetMasterWebControl("TextBox6"), TextBox).Text

            If DirectCast(Me.GetMasterWebControl("CheckBox2"), CheckBox).Checked = True Then
                ' スタイル指定あり
                Me.ShowOKMessageDialog(
                    "MessageID", "Message：" &
                    msg, FxEnum.IconType.Information, "テスト", style)
            Else
                ' スタイル指定なし
                Me.ShowOKMessageDialog(
                    "MessageID", "Message：" &
                    msg, FxEnum.IconType.Information, "テスト")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' lbnMLinkButton24のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_lbnMLinkButton24_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = DirectCast(Me.GetMasterWebControl("TextBox5"), TextBox).Text

            ' 受け渡しデータの設定
            Dim msg As String = DirectCast(Me.GetMasterWebControl("TextBox6"), TextBox).Text

            If DirectCast(Me.GetMasterWebControl("CheckBox2"), CheckBox).Checked = True Then
                ' スタイル指定あり
                Me.ShowYesNoMessageDialog(
                    "MessageID", "Message：" & msg, "Dialog表示テスト", style)
            Else
                ' スタイル指定なし
                Me.ShowYesNoMessageDialog(
                    "MessageID", "Message：" & msg, "Dialog表示テスト")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' ibnMImageButton24のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_ibnMImageButton24_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = DirectCast(Me.GetMasterWebControl("TextBox5"), TextBox).Text

            ' ネストしていた場合、連結

            ' ModalInterface（Session）からデータを取得
            Dim msg As String = DirectCast(Me.GetDataFromModalInterface("msg"), String)

            ' 受け渡しデータの設定
            msg += "," + DirectCast(Me.GetMasterWebControl("TextBox6"), TextBox).Text

            ' ModalInterface（Session）からデータを取得
            Me.SetDataToModalInterface("msg", msg)

            If DirectCast(Me.GetMasterWebControl("CheckBox2"), CheckBox).Checked = True Then
                ' スタイル指定あり
                ' 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
                Me.ShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx", style)
            Else
                ' スタイル指定なし
                ' 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
                Me.ShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' impMImageMap24のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen1_impMImageMap24_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = DirectCast(Me.GetMasterWebControl("TextBox5"), TextBox).Text

            If DirectCast(Me.GetMasterWebControl("CheckBox2"), CheckBox).Checked = True Then
                ' スタイル指定あり
                Me.ShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx", style)
            Else
                ' スタイル指定なし
                Me.ShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx")
            End If

            Return ""
        End Function

#End Region

#End Region

#Region "追加のブランチのMaster Page上のフレームワーク対象Control"

#Region "testScreen1bmp1.master上"

        ''' <summary>
        ''' btnCPF_AのClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testScreen1bmp1_btnCPF_A_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' btnCPF_BのClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testScreen1bmp1_btnCPF_B_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' btnCPF_CのClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testScreen1bmp1_btnCPF_C_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "testScreen1bmp2.master上"

        ''' <summary>
        ''' btnCPF_A1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testScreen1bmp2_btnCPF_A1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' btnCPF_B1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testScreen1bmp2_btnCPF_B1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' btnCPF_C1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testScreen1bmp2_btnCPF_C1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#End Region

#End Region

#Region "Content Page上のフレームワーク対象Control"

#Region "Content Page１"

#Region "基本処理"

        ''' <summary>
        ''' btnButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行",
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' ibnImageButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行 - " &
                "x:" & fxEventArgs.X.ToString() &
                ",y:" & fxEventArgs.Y.ToString(), FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' impImageMap1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                fxEventArgs.ButtonID & "Click イベント",
                fxEventArgs.MethodName & "の実行 - " &
                "pbv:" & fxEventArgs.PostBackValue,
                FxEnum.IconType.Information, "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "画面遷移処理"

        ''' <summary>
        ''' btnButton2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' lbnLinkButton2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' ibnImageButton2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

        ''' <summary>
        ''' impImageMap2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap2_Click(fxEventArgs As FxEventArgs) As String
            Return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx"
        End Function

#End Region

#Region "Control取得"

        ''' <summary>
        ''' btnButton3のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
            ' Controlを取得し
            Dim temp As Control = DirectCast(Me.GetFxWebControl(Me.TextBox1.Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' Message表示
                Me.ShowOKMessageDialog("GetFxWebControl", "Controlを取得できませんでした。", FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton3のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton3_Click(fxEventArgs As FxEventArgs) As String
            ' Controlを取得し
            Dim temp As Control = DirectCast(Me.GetMasterWebControl(Me.TextBox1.Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' Message表示
                Me.ShowOKMessageDialog("GetMasterWebControl", "Controlを取得できませんでした。", FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

        ''' <summary>
        ''' ibnImageButton3のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton3_Click(fxEventArgs As FxEventArgs) As String
            ' Controlを取得し
            Dim temp As Control = DirectCast(Me.GetContentWebControl(Me.TextBox1.Text), Control)

            If temp Is Nothing Then
                ' 取得できなかった

                ' Message表示
                Me.ShowOKMessageDialog("GetContentWebControl", "Controlを取得できませんでした。", FxEnum.IconType.Information, "テスト結果")
            Else
                ' 取得できた

                ' 消したり出したり
                If temp.Visible = True Then
                    temp.Visible = False
                Else
                    temp.Visible = True
                End If
            End If

            Return ""
        End Function

#End Region

#Region "Dialog表示"

        ''' <summary>
        ''' btnButton4のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton4_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = Me.TextBox2.Text

            ' 受け渡しデータの設定
            Dim msg As String = Me.TextBox3.Text

            If Me.CheckBox1.Checked = True Then
                ' スタイル指定あり
                Me.ShowOKMessageDialog("MessageID", "Message：" & msg, FxEnum.IconType.Information, "テスト", style)
            Else
                ' スタイル指定なし
                Me.ShowOKMessageDialog("MessageID", "Message：" & msg, FxEnum.IconType.Information, "テスト")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton4のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton4_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = Me.TextBox2.Text

            ' 受け渡しデータの設定
            Dim msg As String = Me.TextBox3.Text

            If Me.CheckBox1.Checked = True Then
                ' スタイル指定あり
                Me.ShowYesNoMessageDialog("MessageID", "Message：" & msg, "Dialog表示テスト", style)
            Else
                ' スタイル指定なし
                Me.ShowYesNoMessageDialog("MessageID", "Message：" & msg, "Dialog表示テスト")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' ibnImageButton4のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton4_Click(fxEventArgs As FxEventArgs) As String
            ' ネストしていた場合、連結

            ' ModalInterface（Session）からデータを取得
            Dim msg As String = DirectCast(Me.GetDataFromModalInterface("msg"), String)

            ' 受け渡しデータの設定
            If Me.TextBox3.Text <> "" Then
                msg += "," & Me.TextBox3.Text
            End If

            ' ModalInterface（Session）にデータを設定
            Me.SetDataToModalInterface("msg", msg)

            ' スタイルを取得
            Dim style As String = Me.TextBox2.Text

            If Me.CheckBox1.Checked = True Then
                ' スタイル指定あり
                ' 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
                ' ※ QueryString指定あり
                Me.ShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test", style)
            Else
                ' スタイル指定なし
                ' 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
                ' ※ QueryString指定あり
                Me.ShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' impImageMap4のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap4_Click(fxEventArgs As FxEventArgs) As String
            ' スタイルを取得
            Dim style As String = Me.TextBox2.Text
            ' ターゲットを取得
            Dim target As String = Me.TextBox2a.Text


            If Me.CheckBox1.Checked = True AndAlso Me.CheckBox1a.Checked = True Then
                ' スタイル指定あり
                ' ※ QueryString指定あり
                Me.ShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test", style, target)
            ElseIf Me.CheckBox1.Checked = True Then
                ' スタイル指定あり
                ' ※ QueryString指定あり
                Me.ShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test", style)
            ElseIf Me.CheckBox1a.Checked = True Then
                ' スタイル指定あり
                ' ※ QueryString指定あり
                Me.ShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test", "", target)
            Else
                ' スタイル指定なし
                ' ※ QueryString指定あり
                Me.ShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test")
            End If

            Return ""
        End Function

#End Region

#End Region

#Region "Content Page２"

#Region "ModalDialogのインターフェイス"

        ''' <summary>
        ''' btnButton21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton21_Click(fxEventArgs As FxEventArgs) As String
            ' 親画面別セッション領域 - 設定
            Me.SetDataToModalInterface("msg", Me.TextBox4.Text)
            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton21_Click(fxEventArgs As FxEventArgs) As String
            ' 親画面別セッション領域 - 取得

            ' Message表示
            Me.ShowOKMessageDialog(
                "親画面別セッション（キー：msg）は、",
                DirectCast(Me.GetDataFromModalInterface("msg"), String),
                FxEnum.IconType.Information, "テスト結果")

            Return ""
        End Function

        ''' <summary>
        ''' ibnImageButton21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton21_Click(fxEventArgs As FxEventArgs) As String
            ' 親画面別セッション領域 - キー：msgのみ削除
            Me.DeleteDataFromModalInterface("msg")
            Return ""
        End Function

        ''' <summary>
        ''' impImageMap21のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap21_Click(fxEventArgs As FxEventArgs) As String
            ' 親画面別セッション領域 - 全て削除
            Me.DeleteDataFromModalInterface()
            Return ""
        End Function

#End Region

#Region "自画面を閉じる"

        ''' <summary>
        ''' btnButton22のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton22_Click(fxEventArgs As FxEventArgs) As String
            ' 自画面を閉じる
            Me.CloseModalScreen()
            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton22のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton22_Click(fxEventArgs As FxEventArgs) As String
            ' 自画面を閉じる
            Me.CloseModalScreen_NoPostback()
            Return ""
        End Function

        ''' <summary>
        ''' ImageButton22のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton22_Click(fxEventArgs As FxEventArgs) As String
            ' 自画面を閉じる
            Me.CloseModalScreen_WithAllParent()
            Return ""
        End Function

        ''' <summary>
        ''' ImageMap22のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap22_Click(fxEventArgs As FxEventArgs) As String
            ' 自画面を閉じる
            Me.CloseModalScreen()
            Return ""
        End Function

#End Region

#Region "２重送信防止テスト"

        ''' <summary>
        ''' btnButton23のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton23_Click(fxEventArgs As FxEventArgs) As String
            ' ２重送信防止テスト
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' 確認用のカウンタ
            If Session("cnt") Is Nothing Then
                Session("cnt") = 1
            Else
                Session("cnt") = CInt(Session("cnt")) + 1
            End If

            Response.Write(Session("cnt").ToString())

            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton23のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton23_Click(fxEventArgs As FxEventArgs) As String
            ' ２重送信防止テスト
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' 確認用のカウンタ
            If Session("cnt") Is Nothing Then
                Session("cnt") = 1
            Else
                Session("cnt") = CInt(Session("cnt")) + 1
            End If

            Response.Write(Session("cnt").ToString())

            Return ""
        End Function

        ''' <summary>
        ''' ImageButton23のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton23_Click(fxEventArgs As FxEventArgs) As String
            ' ２重送信防止テスト
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' 確認用のカウンタ
            If Session("cnt") Is Nothing Then
                Session("cnt") = 1
            Else
                Session("cnt") = CInt(Session("cnt")) + 1
            End If

            Response.Write(Session("cnt").ToString())

            Return ""
        End Function

        ''' <summary>
        ''' ImageMap23のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap23_Click(fxEventArgs As FxEventArgs) As String
            ' ２重送信防止テスト
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' 確認用のカウンタ
            If Session("cnt") Is Nothing Then
                Session("cnt") = 1
            Else
                Session("cnt") = CInt(Session("cnt")) + 1
            End If

            Response.Write(Session("cnt").ToString())

            Return ""
        End Function

#End Region

#Region "エラーを起こす"

        ''' <summary>
        ''' btnButton24のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton24_Click(fxEventArgs As FxEventArgs) As String
            ' その他、一般的な例外
            Throw New Exception(fxEventArgs.MethodName & "で、Exceptionをスロー。")

            'return "";
        End Function

        ''' <summary>
        ''' lbnLinkButton24のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton24_Click(fxEventArgs As FxEventArgs) As String
            ' システム例外
            Throw New BusinessSystemException("xxxxx", fxEventArgs.MethodName & "で、BusinessSystemExceptionをスロー。")

            'return "";
        End Function

        ''' <summary>
        ''' ImageButton24のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton24_Click(fxEventArgs As FxEventArgs) As String
            ' 業務例外
            Throw New BusinessApplicationException("xxxxx", fxEventArgs.MethodName & "で、BusinessApplicationExceptionをスロー。", "エラー情報はここでは無視される。")

            'return "";
        End Function

#End Region

#End Region

#Region "Content Page３"

#Region "自画面に画面遷移"

        ''' <summary>
        ''' btnButton31のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton31_Click(fxEventArgs As FxEventArgs) As String
            ' ウィンドウ別セッション領域 - 設定
            Me.SetDataToBrowserWindow("msg", Me.TextBox5.Text)

            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton31のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton31_Click(fxEventArgs As FxEventArgs) As String
            ' ウィンドウ別セッション領域 - 取得

            ' Message表示
            Me.ShowOKMessageDialog(
                "ウィンドウ別セッション（キー：msg）は、",
                DirectCast(Me.GetDataFromBrowserWindow("msg"), String),
                FxEnum.IconType.Information, "テスト結果")

            Return ""
        End Function

        ''' <summary>
        ''' ibnImageButton31のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton31_Click(fxEventArgs As FxEventArgs) As String
            ' 次画面（自画面）に画面遷移
            Return "~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx"
        End Function

        ''' <summary>
        ''' impImageMap31のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap31_Click(fxEventArgs As FxEventArgs) As String
            ' ウィンドウ別セッション領域 - 削除
            Select Case fxEventArgs.PostBackValue
                Case "spot1"
                    ' キー：msgのみ削除
                    Me.DeleteDataFromBrowserWindow("msg")
                    Exit Select
                Case "spot2"
                    ' 全て削除
                    Me.DeleteDataFromBrowserWindow()
                    Exit Select
                Case "spot3"
                    ' 全て削除
                    Me.DeleteDataFromBrowserWindow()
                    Exit Select
            End Select

            Return ""
        End Function

#End Region

#Region "onloadで子画面表示"

        ''' <summary>
        ''' btnButton32のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton32_Click(fxEventArgs As FxEventArgs) As String
            Session("DialogAtOnLoad") = "ok"
            Return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx"
        End Function

        ''' <summary>
        ''' lbnLinkButton32のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton32_Click(fxEventArgs As FxEventArgs) As String
            Session("DialogAtOnLoad") = "yesno"
            Return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx"
        End Function

        ''' <summary>
        ''' ibnImageButton32のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton32_Click(fxEventArgs As FxEventArgs) As String
            Session("DialogAtOnLoad") = "modal"
            Return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx"
        End Function

        ''' <summary>
        ''' impImageMap32のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap32_Click(fxEventArgs As FxEventArgs) As String
            Session("DialogAtOnLoad") = "modaless"
            Return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx"
        End Function

#End Region

#Region "ファイルのダウンロード"

        ''' <summary>btnButton33のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton33_Click(fxEventArgs As FxEventArgs) As String
            Response.Clear()

            Response.ContentType = "application/pdf"

            'attachmentで開く場合は、キャッシュを無効にしないこと。
            Response.CacheControl = "private"

            'こっちは、専用アプリケーションで開く
            Response.AppendHeader("Content-Disposition", "attachment;filename=test.pdf")

            Response.WriteFile(Path.Combine(GetConfigParameter.GetConfigValue("TestFilePath"), "test.pdf"))

            Response.[End]()

            Return ""
        End Function

        ''' <summary>lbnLinkButton33のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton33_Click(fxEventArgs As FxEventArgs) As String
            Response.Clear()

            Response.ContentType = "application/pdf"

            'attachmentで開く場合は、キャッシュを無効にしないこと。
            Response.CacheControl = "private"

            'こっちは、IEからOLEオブジェクトを開く
            Response.AppendHeader("Content-Disposition", "inline;filename=test.pdf")

            Response.WriteFile(Path.Combine(GetConfigParameter.GetConfigValue("TestFilePath"), "test.pdf"))

            Response.[End]()

            Return ""
        End Function

        ''' <summary>ibnImageButton33のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton33_Click(fxEventArgs As FxEventArgs) As String
            Me.ShowModalScreen("~/Aspx/testFxLayerP/testDLScreen.aspx")
            Return ""
        End Function

        ''' <summary>ibnImageButton33のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap33_Click(fxEventArgs As FxEventArgs) As String
            'Me.ShowNormalScreen("~/Aspx/testFxLayerP/testDLScreen.aspx");
            Me.ShowNormalScreen("~/Aspx/testFxLayerP/testDLFrame.aspx")
            Return ""
        End Function

#End Region

#End Region

#End Region

#Region "後処理のUOCメソッド"

        ''' <summary>「YES」・「NO」Message Dialogの「×」が押され閉じられた場合の処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_YesNoDialog_X_Click(parentFxEventArgs As FxEventArgs)
            ' 「YES」・「NO」Message Dialogの「×」が押され閉じられた場合の処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた「YES」・「NO」Message Dialog",
                "[×]Buttonを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
        End Sub

        ''' <summary>「YES」・「NO」Message Dialogの「YES」が押され閉じられた場合の処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_YesNoDialog_Yes_Click(parentFxEventArgs As FxEventArgs)
            ' 「YES」・「NO」Message Dialogの「YES」が押され閉じられた場合の処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた「YES」・「NO」Message Dialog",
                "[Yes]Buttonを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
        End Sub

        ''' <summary>「YES」・「NO」Message Dialogの「NO」が押され閉じられた場合の処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_YesNoDialog_No_Click(parentFxEventArgs As FxEventArgs)
            ' 「YES」・「NO」Message Dialogの「NO」が押され閉じられた場合の処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた「YES」・「NO」Message Dialog",
                "[No]Buttonを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
        End Sub

        ''' <summary>業務Modal画面の後処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">業務Modal画面を開いた（親画面側の）ButtonのButton履歴</param>
        ''' <param name="childFxEventArgs">業務Modal画面を閉じた（若しくは一番最後に押された子画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_ModalDialog_End(parentFxEventArgs As FxEventArgs, childFxEventArgs As FxEventArgs)
            ' 業務Modal画面の後処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた業務Modal Dialogの",
                childFxEventArgs.ButtonID & "Buttonを押して閉じた時の後処理",
                FxEnum.IconType.Information, "テスト結果")
        End Sub

#End Region
    End Class
End Namespace
