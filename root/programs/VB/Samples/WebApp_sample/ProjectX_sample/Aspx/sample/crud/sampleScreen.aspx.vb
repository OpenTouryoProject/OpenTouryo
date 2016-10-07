'**********************************************************************************
'* サンプル アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_sample_crud_sampleScreen
'* クラス日本語名  ：サンプル アプリ画面
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'*********************************************************************************
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

' MyType
Imports ProjectX_sample.MyType

''' <summary>サンプル アプリ画面</summary>
Public Partial Class Aspx_sample_crud_sampleScreen
	Inherits MyBaseController
	#Region "ページロードのUOCメソッド"

	''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する
		' TODO:
		Me.ddlIso.SelectedIndex = 1
	End Sub

	''' <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit_PostBack()
		' フォーム初期化（ポストバック）時に実行する処理を実装する
		' TODO:
	End Sub

	#End Region

	#Region "ＣＲＵＤ処理メソッド"

	#Region "参照系"

	''' <summary>
	''' btnMButton1のクリックイベント（件数取得）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton1_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectCount", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
        Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			label.Text = testReturnValue.Obj.ToString() & "件のデータがあります"
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnMButton2のクリックイベント（一覧取得（dt））
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton2_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DT", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
		Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			Me.GridView1.DataSource = testReturnValue.Obj
			Me.GridView1.DataBind()
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnMButton3のクリックイベント（一覧取得（ds））
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton3_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DS", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
		Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			Dim ds As DataSet = DirectCast(testReturnValue.Obj, DataSet)
			Me.GridView1.DataSource = ds.Tables(0)
			Me.GridView1.DataBind()
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnMButton4のクリックイベント（一覧取得（dr））
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
    Protected Function UOC_sampleScreen_btnMButton4_Click(fxEventArgs As FxEventArgs) As String
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DR", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' B層を生成
        Dim myBusiness As New LayerB()

        ' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

        ' 結果表示するメッセージ エリア
        Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
        label.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.GridView1.DataSource = testReturnValue.Obj
            Me.GridView1.DataBind()
        End If

        ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        Return ""
    End Function

	''' <summary>
	''' btnMButton5のクリックイベント（一覧取得（動的sql））
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton5_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DSQL", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 動的SQLの要素を設定
		testParameterValue.OrderColumn = Me.ddlOrderColumn.SelectedValue
		testParameterValue.OrderSequence = Me.ddlOrderSequence.SelectedValue

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
		Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			Me.GridView1.DataSource = testReturnValue.Obj
			Me.GridView1.DataBind()
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnMButton6のクリックイベント（参照処理）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton6_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "Select", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 情報の設定
		testParameterValue.ShipperID = Integer.Parse(Me.TextBox1.Text)

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
		Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			Me.TextBox1.Text = testReturnValue.ShipperID.ToString()
			Me.TextBox2.Text = testReturnValue.CompanyName
			Me.TextBox3.Text = testReturnValue.Phone
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#Region "更新系"

	''' <summary>
	''' btnMButton7のクリックイベント（追加処理）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton7_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "Insert", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 情報の設定
		testParameterValue.CompanyName = Me.TextBox2.Text
		testParameterValue.Phone = Me.TextBox3.Text

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
		Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			label.Text = testReturnValue.Obj.ToString() & "件追加"
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnMButton8のクリックイベント（更新処理）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton8_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "Update", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 情報の設定
		testParameterValue.ShipperID = Integer.Parse(Me.TextBox1.Text)
		testParameterValue.CompanyName = Me.TextBox2.Text
		testParameterValue.Phone = Me.TextBox3.Text

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
		Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			label.Text = testReturnValue.Obj.ToString() & "件更新"
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnMButton9のクリックイベント（削除処理）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMButton9_Click(fxEventArgs As FxEventArgs) As String
		' 引数クラスを生成
		' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "Delete", Convert.ToString(Me.ddlDap.SelectedValue) & "%" & Convert.ToString(Me.ddlMode1.SelectedValue) & "%" & Convert.ToString(Me.ddlMode2.SelectedValue) & "%" & Convert.ToString(Me.ddlExRollback.SelectedValue), Me.UserInfo)

		' 情報の設定
		testParameterValue.ShipperID = Integer.Parse(TextBox1.Text)

		' 分離レベルの設定
		Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

		' B層を生成
		Dim myBusiness As New LayerB()

		' 業務処理を実行
        Dim testReturnValue As TestReturnValue = DirectCast(myBusiness.DoBusinessLogic(DirectCast(testParameterValue, BaseParameterValue), iso), TestReturnValue)

		' 結果表示するメッセージ エリア
		Dim label As Label = DirectCast(Me.GetMasterWebControl("Label1"), Label)
		label.Text = ""

		If testReturnValue.ErrorFlag = True Then
			' 結果（業務続行可能なエラー）
			label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
			label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
			label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
		Else
			' 結果（正常系）
			label.Text = testReturnValue.Obj.ToString() & "件削除"
		End If

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#End Region

	#Region "Ｐ層で例外をスロー"

	''' <summary>
	''' btnButton1のクリックイベント（業務例外）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
		Throw New BusinessApplicationException("Ｐ層で「業務例外」をスロー", "Ｐ層で「業務例外」をスロー", "Ｐ層で「業務例外」をスロー")
	End Function

	''' <summary>
	''' btnButton2のクリックイベント（システム例外）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
		Throw New BusinessSystemException("Ｐ層で「システム例外」をスロー", "Ｐ層で「システム例外」をスロー")
	End Function

	''' <summary>
	''' btnButton3のクリックイベント（その他、一般的な例外）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
		Throw New Exception("Ｐ層で「その他、一般的な例外」をスロー")
	End Function

	''' <summary>
	''' btnButton4のクリックイベント（その他、一般的な例外）
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton4_Click(fxEventArgs As FxEventArgs) As String
		Me.GridView1.DataSource = Nothing
		Me.GridView1.DataBind()

		Return ""
	End Function

	#End Region

	#Region "分離レベルの設定メソッド"

	''' <summary>分離レベルの設定</summary>
	Private Function SelectIsolationLevel() As DbEnum.IsolationLevelEnum
		If Me.ddlIso.SelectedValue = "NC" Then
			Return DbEnum.IsolationLevelEnum.NotConnect
		ElseIf Me.ddlIso.SelectedValue = "NT" Then
			Return DbEnum.IsolationLevelEnum.NoTransaction
		ElseIf Me.ddlIso.SelectedValue = "RU" Then
			Return DbEnum.IsolationLevelEnum.ReadUncommitted
		ElseIf Me.ddlIso.SelectedValue = "RC" Then
			Return DbEnum.IsolationLevelEnum.ReadCommitted
		ElseIf Me.ddlIso.SelectedValue = "RR" Then
			Return DbEnum.IsolationLevelEnum.RepeatableRead
		ElseIf Me.ddlIso.SelectedValue = "SZ" Then
			Return DbEnum.IsolationLevelEnum.Serializable
		ElseIf Me.ddlIso.SelectedValue = "SS" Then
			Return DbEnum.IsolationLevelEnum.Snapshot
		ElseIf Me.ddlIso.SelectedValue = "DF" Then
			Return DbEnum.IsolationLevelEnum.DefaultTransaction
		Else
			Throw New Exception("分離レベルの設定がおかしい")
		End If
	End Function

	#End Region

	#Region "マスタページ、ユーザコントロールのイベント"

	''' <summary>マスタページにイベントハンドラを実装可能にしたのでそのテスト。</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleScreen_btnMPButton_Click(fxEventArgs As FxEventArgs) As String
		Response.Write("UOC_sampleScreen_btnMPButton_Clickを実行できた。")

		Return ""
	End Function

	''' <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_sampleControl1_btnUCButton_Click(fxEventArgs As FxEventArgs) As String
		Response.Write("UOC_sampleControl1_btnUCButton_Clickを実行できた。")

		Return ""
    End Function

    ''' <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_sampleControl2_btnUCButton_Click(fxEventArgs As FxEventArgs) As String
        Response.Write("UOC_sampleControl2_btnUCButton_Clickを実行できた。")

        Return ""
    End Function

	#End Region
End Class
