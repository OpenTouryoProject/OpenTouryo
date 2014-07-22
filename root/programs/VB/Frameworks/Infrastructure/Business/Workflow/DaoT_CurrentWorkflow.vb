'**********************************************************************************
'* クラス名        ：DaoT_CurrentWorkflow
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
Public Class DaoT_CurrentWorkflow
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


	''' <summary>WorkflowControlNo列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PK_WorkflowControlNo() As Object
		Get
			Return Me.HtParameter("WorkflowControlNo")
		End Get
		Set
			Me.HtParameter("WorkflowControlNo") = value
		End Set
	End Property



	''' <summary>HistoryNo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property HistoryNo() As Object
		Get
			Return Me.HtParameter("HistoryNo")
		End Get
		Set
			Me.HtParameter("HistoryNo") = value
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

	''' <summary>FromUserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property FromUserInfo() As Object
		Get
			Return Me.HtParameter("FromUserInfo")
		End Get
		Set
			Me.HtParameter("FromUserInfo") = value
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

	''' <summary>ToUserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ToUserInfo() As Object
		Get
			Return Me.HtParameter("ToUserInfo")
		End Get
		Set
			Me.HtParameter("ToUserInfo") = value
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

	''' <summary>ExclusiveKey列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ExclusiveKey() As Object
		Get
			Return Me.HtParameter("ExclusiveKey")
		End Get
		Set
			Me.HtParameter("ExclusiveKey") = value
		End Set
	End Property

	''' <summary>ReplyDeadline列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ReplyDeadline() As Object
		Get
			Return Me.HtParameter("ReplyDeadline")
		End Get
		Set
			Me.HtParameter("ReplyDeadline") = value
		End Set
	End Property

	''' <summary>StartDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property StartDate() As Object
		Get
			Return Me.HtParameter("StartDate")
		End Get
		Set
			Me.HtParameter("StartDate") = value
		End Set
	End Property

	''' <summary>AcceptanceDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property AcceptanceDate() As Object
		Get
			Return Me.HtParameter("AcceptanceDate")
		End Get
		Set
			Me.HtParameter("AcceptanceDate") = value
		End Set
	End Property

	''' <summary>AcceptanceUserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property AcceptanceUserId() As Object
		Get
			Return Me.HtParameter("AcceptanceUserId")
		End Get
		Set
			Me.HtParameter("AcceptanceUserId") = value
		End Set
	End Property

	''' <summary>AcceptanceUserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property AcceptanceUserInfo() As Object
		Get
			Return Me.HtParameter("AcceptanceUserInfo")
		End Get
		Set
			Me.HtParameter("AcceptanceUserInfo") = value
		End Set
	End Property


	''' <summary>Set_WorkflowControlNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_WorkflowControlNo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_WorkflowControlNo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_WorkflowControlNo_forUPD") = value
		End Set
	End Property


	''' <summary>Set_HistoryNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_HistoryNo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_HistoryNo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_HistoryNo_forUPD") = value
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


	''' <summary>Set_FromUserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_FromUserInfo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_FromUserInfo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_FromUserInfo_forUPD") = value
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


	''' <summary>Set_ToUserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ToUserInfo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ToUserInfo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ToUserInfo_forUPD") = value
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


	''' <summary>Set_ExclusiveKey_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ExclusiveKey_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ExclusiveKey_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ExclusiveKey_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ReplyDeadline_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ReplyDeadline_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ReplyDeadline_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ReplyDeadline_forUPD") = value
		End Set
	End Property


	''' <summary>Set_StartDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_StartDate_forUPD() As Object
		Get
			Return Me.HtParameter("Set_StartDate_forUPD")
		End Get
		Set
			Me.HtParameter("Set_StartDate_forUPD") = value
		End Set
	End Property


	''' <summary>Set_AcceptanceDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_AcceptanceDate_forUPD() As Object
		Get
			Return Me.HtParameter("Set_AcceptanceDate_forUPD")
		End Get
		Set
			Me.HtParameter("Set_AcceptanceDate_forUPD") = value
		End Set
	End Property


	''' <summary>Set_AcceptanceUserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_AcceptanceUserId_forUPD() As Object
		Get
			Return Me.HtParameter("Set_AcceptanceUserId_forUPD")
		End Get
		Set
			Me.HtParameter("Set_AcceptanceUserId_forUPD") = value
		End Set
	End Property


	''' <summary>Set_AcceptanceUserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_AcceptanceUserInfo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_AcceptanceUserInfo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_AcceptanceUserInfo_forUPD") = value
		End Set
	End Property



	''' <summary>WorkflowControlNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property WorkflowControlNo_Like() As Object
		Get
			Return Me.HtParameter("WorkflowControlNo_Like")
		End Get
		Set
			Me.HtParameter("WorkflowControlNo_Like") = value
		End Set
	End Property


	''' <summary>HistoryNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property HistoryNo_Like() As Object
		Get
			Return Me.HtParameter("HistoryNo_Like")
		End Get
		Set
			Me.HtParameter("HistoryNo_Like") = value
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


	''' <summary>FromUserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property FromUserInfo_Like() As Object
		Get
			Return Me.HtParameter("FromUserInfo_Like")
		End Get
		Set
			Me.HtParameter("FromUserInfo_Like") = value
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


	''' <summary>ToUserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ToUserInfo_Like() As Object
		Get
			Return Me.HtParameter("ToUserInfo_Like")
		End Get
		Set
			Me.HtParameter("ToUserInfo_Like") = value
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


	''' <summary>ExclusiveKey_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ExclusiveKey_Like() As Object
		Get
			Return Me.HtParameter("ExclusiveKey_Like")
		End Get
		Set
			Me.HtParameter("ExclusiveKey_Like") = value
		End Set
	End Property


	''' <summary>ReplyDeadline_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ReplyDeadline_Like() As Object
		Get
			Return Me.HtParameter("ReplyDeadline_Like")
		End Get
		Set
			Me.HtParameter("ReplyDeadline_Like") = value
		End Set
	End Property


	''' <summary>StartDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property StartDate_Like() As Object
		Get
			Return Me.HtParameter("StartDate_Like")
		End Get
		Set
			Me.HtParameter("StartDate_Like") = value
		End Set
	End Property


	''' <summary>AcceptanceDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property AcceptanceDate_Like() As Object
		Get
			Return Me.HtParameter("AcceptanceDate_Like")
		End Get
		Set
			Me.HtParameter("AcceptanceDate_Like") = value
		End Set
	End Property


	''' <summary>AcceptanceUserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property AcceptanceUserId_Like() As Object
		Get
			Return Me.HtParameter("AcceptanceUserId_Like")
		End Get
		Set
			Me.HtParameter("AcceptanceUserId_Like") = value
		End Set
	End Property


	''' <summary>AcceptanceUserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property AcceptanceUserInfo_Like() As Object
		Get
			Return Me.HtParameter("AcceptanceUserInfo_Like")
		End Get
		Set
			Me.HtParameter("AcceptanceUserInfo_Like") = value
		End Set
	End Property


	#End Region

	#Region "クエリ メソッド"

	#Region "Insert"

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	Public Function S1_Insert() As Integer
		' ファイルからSQL（Insert）を設定する。
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_S1_Insert.sql")

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
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_D1_Insert.xml")

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
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_S2_Select.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Select）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	''' <summary>検索条件を指定し、結果セットを参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub D2_Select(dt As DataTable)
		' ファイルからSQL（DynSel）を設定する。
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_D2_Select.xml")

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
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_S3_Update.xml")

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
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_D3_Update.xml")

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
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_S4_Delete.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Delete）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function D4_Delete() As Integer
		' ファイルからSQL（DynDel）を設定する。
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_D4_Delete.xml")

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
		Me.SetSqlByFile2("DaoT_CurrentWorkflow_D5_SelCnt.xml")

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
