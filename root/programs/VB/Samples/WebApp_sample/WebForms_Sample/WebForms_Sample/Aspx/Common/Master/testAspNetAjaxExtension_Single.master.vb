'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testAspNetAjaxExtension_Single
'* クラス日本語名  ：Ajaxテスト用のMaster Page（updateパネルを親から纏めて使用）
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

Namespace Aspx.Common.Master
    ''' <summary>Ajaxテスト用のMaster Page（updateパネルを親から纏めて使用）</summary>
    Partial Public Class testAspNetAjaxExtension_Single
        Inherits BaseMasterController
        ''' <summary>btnMButton1のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_btnMButton1_Click(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox1.Text = "ajaxのPost Back（Button Click）"

            ' ajaxのEvent Handlerでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>btnMButton2のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_btnMButton2_Click(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox2.Text = "通常のPost Back（Button Click）"

            Return ""
        End Function

        ''' <summary>
        ''' ddlMDropDownList1のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_ddlMDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox3.Text = "ajaxのPost Back（DDLのSelected Index Changed）"

            ' ajaxのEvent Handlerでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>
        ''' ddlMDropDownList2のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_ddlMDropDownList2_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox4.Text = "通常のPost Back（DDLのSelected Index Changed）"

            Return ""
        End Function

        ''' <summary>btnMButton3のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_btnMButton3_Click(fxEventArgs As FxEventArgs) As String
            Throw New Exception("Ajaxでエラー")

            'return "";
        End Function
    End Class
End Namespace
