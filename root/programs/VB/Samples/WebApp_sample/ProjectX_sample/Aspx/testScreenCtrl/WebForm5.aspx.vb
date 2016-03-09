'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testScreenCtrl_WebForm5
'* クラス日本語名  ：画面遷移制御機能テスト画面５（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>画面遷移制御機能テスト画面５（Ｐ層）</summary>
Partial Public Class Aspx_testScreenCtrl_WebForm5
    Inherits MyBaseController
    ' 部品使用する・しないフラグ
    Private IsFx As Boolean = False

#Region "ページロードのUOCメソッド"

    ''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    ''' <remarks>実装必須</remarks>
    Protected Overloads Overrides Sub UOC_FormInit()
        ' フォーム初期化（初回ロード）時に実行する処理を実装する
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
            lblQueryString.Text += qsKey + "=" + Request.QueryString(qsKey) + ";"
        Next
    End Sub

    ''' <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    ''' <remarks>実装必須</remarks>
    Protected Overloads Overrides Sub UOC_FormInit_PostBack()
        ' フォーム初期化（ポストバック）時に実行する処理を実装する
        ' TODO:

        ' 状態の通知
        Dim lblStatus As Label = DirectCast(Me.GetMasterWebControl("lblStatus"), Label)
        lblStatus.Text = "これは、ポスト（ポストバックです）"

        ' ---

        ' Fxを使用するモード
        If DirectCast(Me.GetContentWebControl("CheckBox1"), CheckBox).Checked Then
            Me.IsFx = True
        End If
    End Sub

#End Region

#Region "マスタ ページ上のフレームワーク対象コントロール"

#Region "チェック可能な画面遷移（外サイトへ）"

    ''' <summary>
    ''' ibnMImageButton1のクリックイベント
    ''' </summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_testScreenCtrl_ibnMImageButton1_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 外部サイトへ（QueryString付き）
        Return "google?q=WebForm5"
    End Function

#End Region

#End Region

#Region "コンテンツ ページ上のフレームワーク対象コントロール"

#Region "チェック可能な画面遷移"

    ''' <summary>btnButton1のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>画面遷移可能（○）</remarks>
    Protected Function UOC_btnButton1_Click(ByVal fxEventArgs As FxEventArgs) As String
        Return "5→1"
    End Function

    ''' <summary>btnButton2のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>画面遷移可能（○）</remarks>
    Protected Function UOC_btnButton2_Click(ByVal fxEventArgs As FxEventArgs) As String
        Return "5→2"
    End Function

    ''' <summary>btnButton3のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>画面遷移可能（○）</remarks>
    Protected Function UOC_btnButton3_Click(ByVal fxEventArgs As FxEventArgs) As String
        Return "5→3"
    End Function

    ''' <summary>btnButton3のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>画面遷移可能（○）</remarks>
    Protected Function UOC_btnButton4_Click(ByVal fxEventArgs As FxEventArgs) As String
        Return "5→4"
    End Function

    ''' <summary>btnButton3のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>画面遷移不可能（×）</remarks>
    Protected Function UOC_btnButton5_Click(ByVal fxEventArgs As FxEventArgs) As String
        Return "5→5"
    End Function

#End Region

#Region "チェック不可能な画面遷移"

#Region "Transfer or FxTransfer"

    ''' <summary>btnButton6のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
    Protected Function UOC_btnButton6_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxTransfer("./WebForm1.aspx")
        Else
            Server.Transfer("./WebForm1.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton7のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
    Protected Function UOC_btnButton7_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxTransfer("./WebForm2.aspx")
        Else
            Server.Transfer("./WebForm2.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton8のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
    Protected Function UOC_btnButton8_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxTransfer("./WebForm3.aspx")
        Else
            Server.Transfer("./WebForm3.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton9のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
    Protected Function UOC_btnButton9_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxTransfer("./WebForm4.aspx")
        Else
            Server.Transfer("./WebForm4.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton10のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Transfer）（×）</remarks>
    Protected Function UOC_btnButton10_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxTransfer("./WebForm5.aspx")
        Else
            Server.Transfer("./WebForm5.aspx")
        End If

        Return ""
    End Function

#End Region

#Region "Redirect or FxRedirect"

    ''' <summary>btnButton11のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
    Protected Function UOC_btnButton11_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxRedirect("./WebForm1.aspx")
        Else
            Response.Redirect("./WebForm1.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton12のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
    Protected Function UOC_btnButton12_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxRedirect("./WebForm2.aspx")
        Else
            Response.Redirect("./WebForm2.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton13のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
    Protected Function UOC_btnButton13_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxRedirect("./WebForm3.aspx")
        Else
            Response.Redirect("./WebForm3.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton14のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
    Protected Function UOC_btnButton14_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxRedirect("./WebForm4.aspx")
        Else
            Response.Redirect("./WebForm4.aspx")
        End If

        Return ""
    End Function

    ''' <summary>btnButton15のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
    Protected Function UOC_btnButton15_Click(ByVal fxEventArgs As FxEventArgs) As String
        If Me.IsFx Then
            Me.FxRedirect("./WebForm5.aspx")
        Else
            Response.Redirect("./WebForm5.aspx")
        End If

        Return ""
    End Function

#End Region

#End Region

#Region "ポストバック"

    ''' <summary>btnButton16のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>違法な画面遷移（Redirect）（×）</remarks>
    Protected Function UOC_btnButton16_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowOKMessageDialog("ポストバックの", "テストです", FxEnum.IconType.Information, "テスト")
        Return ""
    End Function

#End Region

#Region "子画面の表示"

#Region "window open"

    ''' <summary>btnButton17のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>window open</remarks>
    Protected Function UOC_btnButton17_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowNormalScreen("./WebForm1.aspx")
        Return ""
    End Function

    ''' <summary>btnButton18のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>window open</remarks>
    Protected Function UOC_btnButton18_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowNormalScreen("./WebForm2.aspx")
        Return ""
    End Function

    ''' <summary>btnButton19のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>window open</remarks>
    Protected Function UOC_btnButton19_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowNormalScreen("./WebForm3.aspx")
        Return ""
    End Function

    ''' <summary>btnButton20のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>window open</remarks>
    Protected Function UOC_btnButton20_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowNormalScreen("./WebForm4.aspx")
        Return ""
    End Function

    ''' <summary>btnButton21のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>window open</remarks>
    Protected Function UOC_btnButton21_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowNormalScreen("./WebForm5.aspx")
        Return ""
    End Function

#End Region

#Region "dialog"

    ''' <summary>btnButton22のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>dialog</remarks>
    Protected Function UOC_btnButton22_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm1.aspx")
        Return ""
    End Function

    ''' <summary>btnButton23のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>dialog</remarks>
    Protected Function UOC_btnButton23_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm2.aspx")
        Return ""
    End Function

    ''' <summary>btnButton24のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>dialog</remarks>
    Protected Function UOC_btnButton24_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm3.aspx")
        Return ""
    End Function

    ''' <summary>btnButton25のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>dialog</remarks>
    Protected Function UOC_btnButton25_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm4.aspx")
        Return ""
    End Function

    ''' <summary>btnButton26のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>dialog</remarks>
    Protected Function UOC_btnButton26_Click(ByVal fxEventArgs As FxEventArgs) As String
        Me.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm5.aspx")
        Return ""
    End Function

#End Region

#End Region

#Region "ブラウザ ウィンドウ別セッション領域"

    ''' <summary>btnButton27のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>dialog</remarks>
    Protected Function UOC_btnButton27_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' ブラウザ ウィンドウ別セッション領域 - 設定
        Me.SetDataToBrowserWindow("msg", Me.TextBox1.Text)
        Return ""
    End Function

    ''' <summary>btnButton28のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>dialog</remarks>
    Protected Function UOC_btnButton28_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' ブラウザ ウィンドウ別セッション領域 - 取得
        Me.TextBox1.Text = DirectCast(Me.GetDataFromBrowserWindow("msg"), String)
        Return ""
    End Function

#End Region

#End Region
End Class
