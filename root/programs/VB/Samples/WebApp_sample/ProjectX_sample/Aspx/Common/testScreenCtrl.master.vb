'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testScreenCtrl
'* クラス日本語名  ：画面遷移制御機能テスト画面用のMaster Page
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Framework.Presentation

Namespace Aspx.Common
    ''' <summary>画面遷移制御機能テスト画面用のMaster Page</summary>
    Partial Public Class testScreenCtrl
        Inherits BaseMasterController
        ''' <summary>
        ''' btnMButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_btnMButton1_Click(fxEventArgs As FxEventArgs) As String

            Return "WebForm0"
        End Function

        '---

        ''' <summary>
        ''' lbnMLinkButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_lbnMLinkButton1_Click(fxEventArgs As FxEventArgs) As String

            Return "WebForm3"
        End Function

        ''' <summary>
        ''' lbnMLinkButton2のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_lbnMLinkButton2_Click(fxEventArgs As FxEventArgs) As String

            Return "WebForm1"
        End Function

        ''' <summary>
        ''' lbnMLinkButton3のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_lbnMLinkButton3_Click(fxEventArgs As FxEventArgs) As String

            Return "WebForm2"
        End Function

        ''' <summary>
        ''' lbnMLinkButton4のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_lbnMLinkButton4_Click(fxEventArgs As FxEventArgs) As String

            Return "WebForm4"
        End Function

        ''' <summary>
        ''' lbnMLinkButton5のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_lbnMLinkButton5_Click(fxEventArgs As FxEventArgs) As String

            Return "WebForm5"
        End Function
    End Class
End Namespace
