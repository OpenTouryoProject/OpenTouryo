'**********************************************************************************
'* クラス名        ：DaoM_Workflow
'* クラス日本語名  ：自動生成Ｄａｏクラス
'*
'* 作成日時        ：2014/7/18
'* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2012/06/14  西野  大介        ResourceLoaderに加え、EmbeddedResourceLoaderに対応
'*  2013/09/09  西野  大介        ExecGenerateSQLメソッドを追加した（バッチ更新用）。
'**********************************************************************************

#Region "using"

' System～
Imports System
Imports System.IO
Imports System.Data
Imports System.Collections

' フレームワーク
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Common

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Util

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Dao

#End Region

''' <summary>自動生成Ｄａｏクラス</summary>
Public Class DaoM_Workflow
	Inherits MyBaseDao
	#Region "インスタンス変数"

	''' <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
	Protected HtUserParameter As New Hashtable()
	''' <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
	Protected HtParameter As New Hashtable()

	#End Region

	#Region "コンストラクタ"

	''' <summary>コンストラクタ</summary>
	Public Sub New(dam As BaseDam)
		MyBase.New(dam)
	End Sub

	#End Region

	#Region "共通関数（パラメタの制御）"

	''' <summary>ユーザ パラメタ（文字列置換）をハッシュ テーブルに設定する。</summary>
	''' <param name="userParamName">ユーザ パラメタ名</param>
	''' <param name="userParamValue">ユーザ パラメタ値</param>
	Public Sub SetUserParameteToHt(userParamName As String, userParamValue As String)
		' ユーザ パラメタをハッシュ テーブルに設定
		Me.HtUserParameter(userParamName) = userParamValue
	End Sub

	''' <summary>パラメタ ライズド クエリのパラメタをハッシュ テーブルに設定する。</summary>
	''' <param name="paramName">パラメタ名</param>
	''' <param name="paramValue">パラメタ値</param>
	Public Sub SetParameteToHt(paramName As String, paramValue As Object)
		' ユーザ パラメタをハッシュ テーブルに設定
		Me.HtParameter(paramName) = paramValue
	End Sub

	''' <summary>
	''' ・ユーザ パラメタ（文字列置換）
	''' ・パラメタ ライズド クエリのパラメタ
	''' を格納するハッシュ テーブルをクリアする。
	''' </summary>
	Public Sub ClearParametersFromHt()
		' ユーザ パラメタ（文字列置換）用ハッシュ テーブルを初期化
		Me.HtUserParameter = New Hashtable()
		' パラメタ ライズド クエリのパラメタ用ハッシュ テーブルを初期化
		Me.HtParameter = New Hashtable()
	End Sub

	''' <summary>パラメタの設定（内部用）</summary>
	Protected Sub SetParametersFromHt()
		' ユーザ パラメタ（文字列置換）を設定する。
		For Each userParamName As String In Me.HtUserParameter.Keys
			Me.SetUserParameter(userParamName, Me.HtUserParameter(userParamName).ToString())
		Next

		' パラメタ ライズド クエリのパラメタを設定する。
		For Each paramName As String In Me.HtParameter.Keys
			Me.SetParameter(paramName, Me.HtParameter(paramName))
		Next
	End Sub

	#End Region

	#Region "プロパティ プロシージャ（setter、getter）"


	''' <summary>Id列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PK_Id() As Object
		Get
			Return Me.HtParameter("Id")
		End Get
		Set
			Me.HtParameter("Id") = value
		End Set
	End Property



	''' <summary>SubSystemId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property SubSystemId() As Object
		Get
			Return Me.HtParameter("SubSystemId")
		End Get
		Set
			Me.HtParameter("SubSystemId") = value
		End Set
	End Property

	''' <summary>WorkflowName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property WorkflowName() As Object
		Get
			Return Me.HtParameter("WorkflowName")
		End Get
		Set
			Me.HtParameter("WorkflowName") = value
		End Set
	End Property

	''' <summary>WfPositionId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property WfPositionId() As Object
		Get
			Return Me.HtParameter("WfPositionId")
		End Get
		Set
			Me.HtParameter("WfPositionId") = value
		End Set
	End Property

	''' <summary>WorkflowNo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property WorkflowNo() As Object
		Get
			Return Me.HtParameter("WorkflowNo")
		End Get
		Set
			Me.HtParameter("WorkflowNo") = value
		End Set
	End Property

	''' <summary>FromUserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property FromUserId() As Object
		Get
			Return Me.HtParameter("FromUserId")
		End Get
		Set
			Me.HtParameter("FromUserId") = value
		End Set
	End Property

	''' <summary>ActionType列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ActionType() As Object
		Get
			Return Me.HtParameter("ActionType")
		End Get
		Set
			Me.HtParameter("ActionType") = value
		End Set
	End Property

	''' <summary>ToUserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ToUserId() As Object
		Get
			Return Me.HtParameter("ToUserId")
		End Get
		Set
			Me.HtParameter("ToUserId") = value
		End Set
	End Property

	''' <summary>ToUserPositionTitlesId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ToUserPositionTitlesId() As Object
		Get
			Return Me.HtParameter("ToUserPositionTitlesId")
		End Get
		Set
			Me.HtParameter("ToUserPositionTitlesId") = value
		End Set
	End Property

	''' <summary>NextWfPositionId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property NextWfPositionId() As Object
		Get
			Return Me.HtParameter("NextWfPositionId")
		End Get
		Set
			Me.HtParameter("NextWfPositionId") = value
		End Set
	End Property

	''' <summary>NextWorkflowNo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property NextWorkflowNo() As Object
		Get
			Return Me.HtParameter("NextWorkflowNo")
		End Get
		Set
			Me.HtParameter("NextWorkflowNo") = value
		End Set
	End Property

	''' <summary>CorrespondOfReplyWorkflow列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property CorrespondOfReplyWorkflow() As Object
		Get
			Return Me.HtParameter("CorrespondOfReplyWorkflow")
		End Get
		Set
			Me.HtParameter("CorrespondOfReplyWorkflow") = value
		End Set
	End Property

	''' <summary>MailTemplateId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property MailTemplateId() As Object
		Get
			Return Me.HtParameter("MailTemplateId")
		End Get
		Set
			Me.HtParameter("MailTemplateId") = value
		End Set
	End Property

	''' <summary>ReserveArea列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ReserveArea() As Object
		Get
			Return Me.HtParameter("ReserveArea")
		End Get
		Set
			Me.HtParameter("ReserveArea") = value
		End Set
	End Property


	''' <summary>Set_Id_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Id_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Id_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Id_forUPD") = value
		End Set
	End Property


	''' <summary>Set_SubSystemId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_SubSystemId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_SubSystemId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_SubSystemId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_WorkflowName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_WorkflowName_forUPD() As Object
		Get
			Return Me.HtParameter("Set_WorkflowName_forUPD")
		End Get
		Set
			Me.HtParameter("Set_WorkflowName_forUPD") = value
		End Set
	End Property


	''' <summary>Set_WfPositionId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_WfPositionId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_WfPositionId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_WfPositionId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_WorkflowNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_WorkflowNo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_WorkflowNo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_WorkflowNo_forUPD") = value
		End Set
	End Property


	''' <summary>Set_FromUserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_FromUserId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_FromUserId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_FromUserId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ActionType_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ActionType_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ActionType_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ActionType_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ToUserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ToUserId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ToUserId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ToUserId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ToUserPositionTitlesId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ToUserPositionTitlesId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ToUserPositionTitlesId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ToUserPositionTitlesId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_NextWfPositionId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_NextWfPositionId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_NextWfPositionId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_NextWfPositionId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_NextWorkflowNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_NextWorkflowNo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_NextWorkflowNo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_NextWorkflowNo_forUPD") = value
		End Set
	End Property


	''' <summary>Set_CorrespondOfReplyWorkflow_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_CorrespondOfReplyWorkflow_forUPD() As Object
		Get
			Return Me.HtParameter("Set_CorrespondOfReplyWorkflow_forUPD")
		End Get
		Set
			Me.HtParameter("Set_CorrespondOfReplyWorkflow_forUPD") = value
		End Set
	End Property


	''' <summary>Set_MailTemplateId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_MailTemplateId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_MailTemplateId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_MailTemplateId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ReserveArea_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ReserveArea_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ReserveArea_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ReserveArea_forUPD") = value
		End Set
	End Property



	''' <summary>Id_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Id_Like() As Object
		Get
			Return Me.HtParameter("Id_Like")
		End Get
		Set
			Me.HtParameter("Id_Like") = value
		End Set
	End Property


	''' <summary>SubSystemId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property SubSystemId_Like() As Object
		Get
			Return Me.HtParameter("SubSystemId_Like")
		End Get
		Set
			Me.HtParameter("SubSystemId_Like") = value
		End Set
	End Property


	''' <summary>WorkflowName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property WorkflowName_Like() As Object
		Get
			Return Me.HtParameter("WorkflowName_Like")
		End Get
		Set
			Me.HtParameter("WorkflowName_Like") = value
		End Set
	End Property


	''' <summary>WfPositionId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property WfPositionId_Like() As Object
		Get
			Return Me.HtParameter("WfPositionId_Like")
		End Get
		Set
			Me.HtParameter("WfPositionId_Like") = value
		End Set
	End Property


	''' <summary>WorkflowNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property WorkflowNo_Like() As Object
		Get
			Return Me.HtParameter("WorkflowNo_Like")
		End Get
		Set
			Me.HtParameter("WorkflowNo_Like") = value
		End Set
	End Property


	''' <summary>FromUserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property FromUserId_Like() As Object
		Get
			Return Me.HtParameter("FromUserId_Like")
		End Get
		Set
			Me.HtParameter("FromUserId_Like") = value
		End Set
	End Property


	''' <summary>ActionType_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ActionType_Like() As Object
		Get
			Return Me.HtParameter("ActionType_Like")
		End Get
		Set
			Me.HtParameter("ActionType_Like") = value
		End Set
	End Property


	''' <summary>ToUserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ToUserId_Like() As Object
		Get
			Return Me.HtParameter("ToUserId_Like")
		End Get
		Set
			Me.HtParameter("ToUserId_Like") = value
		End Set
	End Property


	''' <summary>ToUserPositionTitlesId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ToUserPositionTitlesId_Like() As Object
		Get
			Return Me.HtParameter("ToUserPositionTitlesId_Like")
		End Get
		Set
			Me.HtParameter("ToUserPositionTitlesId_Like") = value
		End Set
	End Property


	''' <summary>NextWfPositionId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property NextWfPositionId_Like() As Object
		Get
			Return Me.HtParameter("NextWfPositionId_Like")
		End Get
		Set
			Me.HtParameter("NextWfPositionId_Like") = value
		End Set
	End Property


	''' <summary>NextWorkflowNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property NextWorkflowNo_Like() As Object
		Get
			Return Me.HtParameter("NextWorkflowNo_Like")
		End Get
		Set
			Me.HtParameter("NextWorkflowNo_Like") = value
		End Set
	End Property


	''' <summary>CorrespondOfReplyWorkflow_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property CorrespondOfReplyWorkflow_Like() As Object
		Get
			Return Me.HtParameter("CorrespondOfReplyWorkflow_Like")
		End Get
		Set
			Me.HtParameter("CorrespondOfReplyWorkflow_Like") = value
		End Set
	End Property


	''' <summary>MailTemplateId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property MailTemplateId_Like() As Object
		Get
			Return Me.HtParameter("MailTemplateId_Like")
		End Get
		Set
			Me.HtParameter("MailTemplateId_Like") = value
		End Set
	End Property


	''' <summary>ReserveArea_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ReserveArea_Like() As Object
		Get
			Return Me.HtParameter("ReserveArea_Like")
		End Get
		Set
			Me.HtParameter("ReserveArea_Like") = value
		End Set
	End Property


	#End Region

	#Region "クエリ メソッド"

	#Region "Insert"

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	Public Function S1_Insert() As Integer
		' ファイルからSQL（Insert）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_S1_Insert.sql")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Insert）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	''' <remarks>パラメタで指定した列のみ挿入値が有効になる。</remarks>
	Public Function D1_Insert() As Integer
		' ファイルからSQL（DynIns）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_D1_Insert.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynIns）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "Select"

	''' <summary>主キーを指定し、１レコード参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub S2_Select(dt As DataTable)
		' ファイルからSQL（Select）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_S2_Select.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Select）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	''' <summary>検索条件を指定し、結果セットを参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub D2_Select(dt As DataTable)
		' ファイルからSQL（DynSel）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_D2_Select.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynSel）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	#End Region

	#Region "Update"

	''' <summary>主キーを指定し、１レコード更新する。</summary>
	''' <returns>更新された行の数</returns>
	''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
	Public Function S3_Update() As Integer
		' ファイルからSQL（Update）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_S3_Update.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Update）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを更新する。</summary>
	''' <returns>更新された行の数</returns>
	''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
	Public Function D3_Update() As Integer
		' ファイルからSQL（DynUpd）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_D3_Update.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynUpd）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "Delete"

	''' <summary>主キーを指定し、１レコード削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function S4_Delete() As Integer
		' ファイルからSQL（Delete）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_S4_Delete.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Delete）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function D4_Delete() As Integer
		' ファイルからSQL（DynDel）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_D4_Delete.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynDel）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "拡張メソッド"

	''' <summary>テーブルのレコード件数を取得する</summary>
	''' <returns>テーブルのレコード件数</returns>
	Public Function D5_SelCnt() As Object
		' ファイルからSQL（DynSelCnt）を設定する。
		Me.SetSqlByFile2("DaoM_Workflow_D5_SelCnt.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（SELECT COUNT）を実行し、戻り値を戻す。
		Return Me.ExecSelectScalar()
	End Function

	''' <summary>静的SQLを生成する。</summary>
	''' <param name="fileName">ファイル名</param>
	''' <param name="sqlUtil">SQLユーティリティ</param>
	''' <returns>生成した静的SQL</returns>
	Public Overloads Function ExecGenerateSQL(fileName As String, sqlUtil As SQLUtility) As String
		' ファイルからSQLを設定する。
		Me.SetSqlByFile2(fileName)

		' パラメタの設定
		Me.SetParametersFromHt()

		Return MyBase.ExecGenerateSQL(sqlUtil)
	End Function

	#End Region

	#End Region
End Class
