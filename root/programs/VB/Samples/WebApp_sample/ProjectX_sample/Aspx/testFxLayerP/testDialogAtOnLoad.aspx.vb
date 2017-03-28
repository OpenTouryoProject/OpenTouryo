'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testDialogAtOnLoad
'* クラス日本語名  ：onloadでのDialog表示のテスト画面（Ｐ層）
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

Namespace Aspx.TestFxLayerP
    ''' <summary>onloadでのDialog表示のテスト画面（Ｐ層）</summary>
    Partial Public Class testDialogAtOnLoad
        Inherits MyBaseController
#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:

            ' 子画面表示メソッドをForm初期化（初回Load）時に実行するテスト
            ' （過去、サポートしていない時期があった）。

            ' 取得
            Dim dialogAtOnLoad As String = DirectCast(Session("DialogAtOnLoad"), String)

            ' Sessionをクリア
            Session("DialogAtOnLoad") = ""

            ' ViewStateへ移動
            ViewState("DialogAtOnLoad") = dialogAtOnLoad

            ' ---

            If dialogAtOnLoad = "ok" Then
                ' OKMessage Dialog表示
                Me.ShowOKMessageDialog("onload（初回Load）で", "「OK」Message Dialog表示", FxEnum.IconType.Information, "onloadでのテスト")
            ElseIf dialogAtOnLoad = "yesno" Then
                ' Yes・NoMessage Dialog表示
                Me.ShowYesNoMessageDialog("onload（初回Load）で", "「YES」・「NO」Message Dialog表示", "onloadでのテスト")
            ElseIf dialogAtOnLoad = "modal" Then
                ' 業務Modal Dialog表示
                Me.ShowModalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
            ElseIf dialogAtOnLoad = "modaless" Then
                ' 業務Modeless画面表示
                Me.ShowNormalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
            End If
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:

            ' こちらは、Page LoadのUOCメソッド（個別：Post Back）の動作確認用。

            If DirectCast(Me.GetContentWebControl("CheckBox1"), CheckBox).Checked Then
                Dim dialogAtOnLoad As String = DirectCast(ViewState("DialogAtOnLoad"), String)

                If dialogAtOnLoad = "ok" Then
                    ' OKMessage Dialog表示
                    Me.ShowOKMessageDialog("onload（Post Back）で",
                                           "OKMessage Dialog表示", FxEnum.IconType.Information, "onloadでのテスト")
                ElseIf dialogAtOnLoad = "yesno" Then
                    ' Yes・NoMessage Dialog表示

                    ' →　Post BackのonloadでShowYesNoMessageDialogは実行できない。
                    '     ShowYesNoMessageDialogの後処理のPost Backで無限ループになる。

                    ' 1. Post BackのonloadでShowYesNoMessageDialog
                    ' 2. Yes・NoMessage Dialog表示
                    ' 3. Yes・NoMessage Dialogの後処理のPost Back
                    ' 4. 1.に戻る（無限ループ）。
                    Me.ShowYesNoMessageDialog("onload（Post Back）で", "Yes・NoMessage Dialog表示", "onloadでのテスト")
                ElseIf dialogAtOnLoad = "modal" Then
                    ' 業務Modal Dialog表示

                    '→　Post BackのonloadでShowModalScreenは実行できない。
                    '    ShowModalScreenの後処理のPost Backで無限ループになる。

                    ' 1. Post BackのonloadでShowModalScreen
                    ' 2. 業務Modal Dialog表示
                    ' 3. 業務Modal Dialogの後処理のPost Back
                    ' 4. 1.に戻る（無限ループ）。
                    '    ※ 閉じる処理が、NoPostback（WithAllParent中間を飛ばす場合）なら可能
                    '    ※ サンプルでは、後処理で、ShowOKMessageDialogも実行され、
                    '       コチラが優先されるので、無限ループにはならない。
                    Me.ShowModalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
                ElseIf dialogAtOnLoad = "modaless" Then
                    ' 業務Modeless画面表示
                    Me.ShowNormalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
                End If
            End If
        End Sub

#End Region

#Region "Content Page上のフレームワーク対象Control"

        ''' <summary>
        ''' btnButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' 画面を閉じる
            Me.CloseModalScreen()

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' btnButton2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
            ' 画面を閉じる
            Me.CloseModalScreen_NoPostback()

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' btnButton2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
            ' 画面を閉じる
            Me.CloseModalScreen_WithAllParent()

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

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
                childFxEventArgs.ButtonID & "Buttonを押して閉じた時の後処理", FxEnum.IconType.Information, "テスト結果")
        End Sub

#End Region
    End Class
End Namespace
