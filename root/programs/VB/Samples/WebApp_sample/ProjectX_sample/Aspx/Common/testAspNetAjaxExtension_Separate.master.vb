'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testAspNetAjaxExtension_Separate
'* クラス日本語名  ：Ajaxテスト用のマスタ ページ（updateパネルを親・子で２つ別けて使用）
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
    ''' <summary>Ajaxテスト用のマスタ ページ（updateパネルを親・子で２つ別けて使用）</summary>
    Partial Public Class testAspNetAjaxExtension_Separate
        Inherits BaseMasterController
        ''' <summary>btnMButton1のクリックイベント</summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_btnMButton1_Click(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox1.Text = "ajaxのポストバック（ボタンクリック）"

            ' ajaxのイベントハンドラでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>btnMButton2のクリックイベント</summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_btnMButton2_Click(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox2.Text = "通常のポストバック（ボタンクリック）"

            Return ""
        End Function

        ''' <summary>
        ''' ddlMDropDownList1のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_ddlMDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox3.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）"

            ' ajaxのイベントハンドラでは画面遷移しないこと。
            Return ""
        End Function

        ''' <summary>
        ''' ddlMDropDownList2のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_ddlMDropDownList2_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' テキストボックスの値を変更
            Me.TextBox4.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）"

            Return ""
        End Function

        ''' <summary>btnMButton3のクリックイベント</summary>
        ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
        ''' <returns>URL</returns>
        Public Function UOC_btnMButton3_Click(fxEventArgs As FxEventArgs) As String
            Throw New Exception("Ajaxでエラー")

            'return "";
        End Function
    End Class
End Namespace
