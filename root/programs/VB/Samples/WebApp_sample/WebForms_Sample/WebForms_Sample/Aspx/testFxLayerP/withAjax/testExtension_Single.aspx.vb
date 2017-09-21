'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testExtension_Single
'* クラス日本語名  ：ASP.NET AJAX Extensionのテスト画面（Ｐ層）
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

Namespace Aspx.TestFxLayerP.WithAjax
    ''' <summary>ASP.NET AJAX Extensionのテスト画面（Ｐ層）</summary>
    Partial Public Class testExtension_Single
        Inherits MyBaseController
        ''' <summary>二重送信防止機能の確認用</summary>
        Private SleepCnt As Integer = 5000

#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:

            ' ScriptManagerにControlの動作を指定する。
            ' Init、PostBackの双方で都度実行する必要がある。
            Me.InitScriptManagerRegister()
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:

            ' ScriptManagerにControlの動作を指定する。
            ' Init、PostBackの双方で都度実行する必要がある。
            Me.InitScriptManagerRegister()
        End Sub

        ''' <summary>
        ''' ScriptManagerにControlの動作を指定する。
        ''' </summary>
        Private Sub InitScriptManagerRegister()
            ' RegisterPostBackControlメソッドで、
            ' ・btnButton2
            ' ・ddlDropDownList2
            ' を非Ajax化する。

            ' ※ 逆の動作は、RegisterAsyncPostBackControlになる。

            Me.CurrentScriptManager.RegisterPostBackControl(Me.GetContentWebControl("btnButton2"))
            Me.CurrentScriptManager.RegisterPostBackControl(Me.GetContentWebControl("ddlDropDownList2"))
        End Sub

#End Region

#Region "Master Page上のフレームワーク対象Control"

        ''' <summary>btnMButton4のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testAspNetAjaxExtension_Single_btnMButton4_Click(fxEventArgs As FxEventArgs) As String
            ' 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox5"), TextBox)
            textBox.Text = "ajaxのPost Back（Button Click）"

            ' ajaxのEvent Handlerでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>btnMButton5のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testAspNetAjaxExtension_Single_btnMButton5_Click(fxEventArgs As FxEventArgs) As String
            ' 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox6"), TextBox)
            textBox.Text = "通常のPost Back（Button Click）"

            Return ""
        End Function

        ''' <summary>
        ''' ddlMDropDownList3のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testAspNetAjaxExtension_Single_ddlMDropDownList3_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox7"), TextBox)
            textBox.Text = "ajaxのPost Back（DDLのSelected Index Changed）"

            ' ajaxのEvent Handlerでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>
        ''' ddlMDropDownList4のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testAspNetAjaxExtension_Single_ddlMDropDownList4_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox8"), TextBox)
            textBox.Text = "通常のPost Back（DDLのSelected Index Changed）"

            Return ""
        End Function

        ''' <summary>btnMButton6のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_testAspNetAjaxExtension_Single_btnMButton6_Click(fxEventArgs As FxEventArgs) As String
            ' 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            Throw New Exception("Ajaxでエラー")

            'return "";
        End Function

#End Region

#Region "Content Page上のフレームワーク対象Control"

        ''' <summary>btnButton1のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Ajaxを制御する場合は、ScriptManagerを使用する。
            ' このクラスを使用すると、Ajax中であるかどうかを判別できる。
            Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
            Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

            ' 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox1"), TextBox)
            textBox.Text = "ajaxのPost Back（Button Click）"

            ' ajaxのEvent Handlerでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>btnButton2のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
            ' Ajaxを制御する場合は、ScriptManagerを使用する。
            ' このクラスを使用すると、Ajax中であるかどうかを判別できる。
            Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
            Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

            ' 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox2"), TextBox)
            textBox.Text = "通常のPost Back（Button Click）"

            Return ""
        End Function

        ''' <summary>
        ''' ddlDropDownList1のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ddlDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' Ajaxを制御する場合は、ScriptManagerを使用する。
            ' このクラスを使用すると、Ajax中であるかどうかを判別できる。
            Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
            Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

            ' 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox3"), TextBox)
            textBox.Text = "ajaxのPost Back（DDLのSelected Index Changed）"

            ' ajaxのEvent Handlerでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>
        ''' ddlDropDownList2のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ddlDropDownList2_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' Ajaxを制御する場合は、ScriptManagerを使用する。
            ' このクラスを使用すると、Ajax中であるかどうかを判別できる。
            Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
            Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

            ' 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            ' テキストボックスの値を変更
            Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox4"), TextBox)
            textBox.Text = "通常のPost Back（DDLのSelected Index Changed）"

            Return ""
        End Function

        ''' <summary>btnButton3のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
            ' Ajaxを制御する場合は、ScriptManagerを使用する。
            ' このクラスを使用すると、Ajax中であるかどうかを判別できる。
            Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
            Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

            ' 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(Me.SleepCnt)

            Throw New Exception("Ajaxでエラー")

            'return "";
        End Function

#End Region
    End Class
End Namespace
