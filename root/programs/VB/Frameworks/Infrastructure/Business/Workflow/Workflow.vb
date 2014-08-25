'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License. 
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：Workflow
'* クラス日本語名  ：ワークフロー
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2014/07/15  西野  大介        新規作成
'*
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

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

Namespace Touryo.Infrastructure.Business.Workflow
	''' <summary>デリゲート型を定義</summary>
	Public Delegate Function GetUserInfoDelegate(userID As Decimal) As String

	''' <summary>ワークフロークラス</summary>
	Public Class Workflow
		Inherits BaseConsolidateDao
		''' <summary>デリゲート静的変数</summary>
		Public Shared GetUserInfo As GetUserInfoDelegate

		''' <summary>コンストラクタ</summary>
		''' <param name="dam">データアクセス制御クラス</param>
		Public Sub New(dam As BaseDam)
				' Baseのコンストラクタに引数を渡すために必要。
			MyBase.New(dam)
		End Sub

		''' <summary>新しいワークフローを準備します。</summary>
		''' <param name="subSystemId">サブシステムID（必須）</param>
		''' <param name="workflowName">ワークフロー名（必須）</param>
		''' <param name="fromUserId">FromユーザID（必須）</param>
		''' <returns>新規ワークフロー</returns>
		''' <remarks>
		''' fromUsersId
		''' 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
		''' </remarks>
		Public Function PreStartWorkflow(subSystemId As String, workflowName As String, fromUserId As Decimal) As DataTable
			'#Region "チェック処理を実装"

			If String.IsNullOrEmpty(subSystemId) Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "subSystemId")))
			ElseIf String.IsNullOrEmpty(workflowName) Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowName")))
			End If

			'#End Region

			' --------------------------------------------------
			' 新しいワークフローを準備
			' --------------------------------------------------
			' M_WorkflowのSELECT
			' --------------------------------------------------
			Dim dt As New DataTable()
			Dim daoM_Workflow As New DaoM_Workflow(Me.Dam)

			' 検索条件
			daoM_Workflow.SubSystemId = subSystemId
			daoM_Workflow.WorkflowName = workflowName
			'daoM_Workflow.WorkflowNo = 1; // スタートなので「1」固定
			daoM_Workflow.ActionType = "Start"
			' スタートなので"Start"
			daoM_Workflow.FromUserId = fromUserId

			' ワークフローの取得
			daoM_Workflow.D2_Select(dt)
			Return dt
		End Function

		''' <summary>新しいワークフローを開始します。</summary>
        ''' <param name="_startWorkflow">新規ワークフロー</param>
		''' <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
		''' <param name="fromUserId">FromユーザID（個人ID 必須）</param>
		''' <param name="workflowReserveArea">T_Workflowの予備領域（任意）</param>
		''' <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
		''' <param name="replyDeadline">回答希望日（任意）</param>
		''' <returns>メール・テンプレートID</returns>
        Public Function StartWorkflow(_startWorkflow As DataRow, workflowControlNo As String, fromUserId As Decimal, workflowReserveArea As String, currentWorkflowReserveArea As String, replyDeadline As System.Nullable(Of DateTime)) As Integer
            '#Region "チェック処理を実装"

            If String.IsNullOrEmpty(workflowControlNo) Then
                Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")))
            End If

            '#End Region

            ' ユーザIDからユーザ情報を取得
            Dim fromUserInfo As String = Workflow.GetUserInfo(fromUserId)
            Dim toUserInfo As String = Workflow.GetUserInfo(CDec(_startWorkflow("ToUserId")))

            ' --------------------------------------------------
            ' 新しいワークフローを開始
            ' --------------------------------------------------
            ' T_WorkflowへのINSERT
            ' --------------------------------------------------
            Dim daoT_Workflow As New DaoT_Workflow(Me.Dam)

            daoT_Workflow.PK_WorkflowControlNo = workflowControlNo
            daoT_Workflow.SubSystemId = _startWorkflow("SubSystemId")
            daoT_Workflow.WorkflowName = _startWorkflow("WorkflowName")
            daoT_Workflow.UserId = fromUserId
            ' 個人IDが必要。
            daoT_Workflow.UserInfo = fromUserInfo
            daoT_Workflow.ReserveArea = workflowReserveArea
            daoT_Workflow.StartDate = DateTime.Now
            'daoT_Workflow.EndDate = DBNull.Value;

            daoT_Workflow.D1_Insert()

            ' --------------------------------------------------
            ' T_CurrentWorkflowへのINSERT
            ' --------------------------------------------------
            Dim daoT_CurrentWorkflow As New DaoT_CurrentWorkflow(Me.Dam)

            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo
            daoT_CurrentWorkflow.HistoryNo = 1
            ' スタートなので「1」固定
            daoT_CurrentWorkflow.WfPositionId = _startWorkflow("WfPositionId")
            daoT_CurrentWorkflow.WorkflowNo = _startWorkflow("WorkflowNo")
            daoT_CurrentWorkflow.FromUserId = fromUserId
            ' 実際のユーザIDを入力する。
            daoT_CurrentWorkflow.FromUserInfo = fromUserInfo
            ' ユーザ入力が必要。
            daoT_CurrentWorkflow.ActionType = _startWorkflow("ActionType")
            daoT_CurrentWorkflow.ToUserId = _startWorkflow("ToUserId")
            daoT_CurrentWorkflow.ToUserInfo = toUserInfo
            ' ユーザ入力が必要。
            daoT_CurrentWorkflow.ToUserPositionTitlesId = _startWorkflow("ToUserPositionTitlesId")
            daoT_CurrentWorkflow.NextWfPositionId = _startWorkflow("NextWfPositionId")
            daoT_CurrentWorkflow.NextWorkflowNo = _startWorkflow("NextWorkflowNo")
            daoT_CurrentWorkflow.ReserveArea = currentWorkflowReserveArea
            'daoT_CurrentWorkflow.ExclusiveKey = "";
            daoT_CurrentWorkflow.ReplyDeadline = replyDeadline
            daoT_CurrentWorkflow.StartDate = DateTime.Now
            'daoT_CurrentWorkflow.AcceptanceDate = DBNull.Value;
            'daoT_CurrentWorkflow.AcceptanceUserId = DBNull.Value;
            'daoT_CurrentWorkflow.AcceptanceUserInfo = DBNull.Value;

            daoT_CurrentWorkflow.D1_Insert()

            ' リターン（MailTemplateId）
            If IsDBNull(_startWorkflow("MailTemplateId")) Then
                Return 0
            Else
                Return CInt(_startWorkflow("MailTemplateId"))
            End If
        End Function

		''' <summary>ワークフロー依頼を取得します。</summary>
		''' <param name="subSystemId">サブシステムID(任意)</param>
		''' <param name="workflowName">ワークフロー名(任意)</param>
		''' <param name="workflowControlNo">ワークフロー管理番号（任意）</param>
		''' <param name="userId">ワークフローの受信ユーザ（必須）</param>
		''' <param name="userPositionTitlesId">
		''' ユーザの職位ID（userIdが御中IDの場合は必須）
		''' </param>
		''' <returns>ワークフロー依頼の一覧</returns>
		''' <remarks>
		''' fromUsersId
		''' 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
		''' </remarks>
		Public Function GetWfRequest(subSystemId As String, workflowName As String, workflowControlNo As String, userId As System.Nullable(Of Decimal), userPositionTitlesId As System.Nullable(Of Integer)) As DataTable
			' チェック処理を実装
			' なし。

			' --------------------------------------------------
			' ワークフローの依頼を取得
			' --------------------------------------------------
			' T_CurrentWorkflowのSELECT
			' --------------------------------------------------
			Dim dao As New CmnDao(Me.Dam)

			dao.SQLFileName = "GetWfRequest.xml"

			' SubSystemId
			If Not String.IsNullOrEmpty(subSystemId) Then
				dao.SetParameter("SubSystemId", subSystemId)
			End If

			' WkflowName
			If Not String.IsNullOrEmpty(workflowName) Then
				dao.SetParameter("WkflowName", workflowName)
			End If

			' WorkflowControlNo
			If Not String.IsNullOrEmpty(workflowControlNo) Then
				dao.SetParameter("WorkflowControlNo", workflowControlNo)
			End If

			' ユーザID（必須）
			If userId.HasValue Then
				dao.SetParameter("ToUserId", userId)
			End If

			' ユーザの職位ID
			If userPositionTitlesId.HasValue Then
				dao.SetParameter("ToUserPositionTitlesId", userPositionTitlesId)
			End If

			' ワークフロー依頼を取得
			Dim dt As New DataTable()
			dao.ExecSelectFill_DT(dt)

			' リターン
			Return dt
		End Function

		''' <summary>ワークフロー依頼を受付ます。</summary>
		''' <param name="workflowRequest">選択したワークフロー依頼</param>
		''' <param name="acceptanceUserId">受付ユーザID</param>
		Public Sub AcceptWfRequest(workflowRequest As DataRow, acceptanceUserId As Decimal)
			'#Region "チェック処理を実装"

			If workflowRequest Is Nothing Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowRequest")))
			ElseIf Not workflowRequest.Table.Columns.Contains("WorkflowControlNo") Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED, "WorkflowControlNo", "workflowRequest")))
			End If

			'#End Region

			' ユーザIDからユーザ情報を取得
			Dim acceptanceUserInfo As String = Workflow.GetUserInfo(acceptanceUserId)

			' --------------------------------------------------
			' 受付（T_CurrentWorkflowのacceptance項目を更新）
			' --------------------------------------------------
			' T_CurrentWorkflowのUPDATE
			' --------------------------------------------------
			Dim daoT_CurrentWorkflow As New DaoT_CurrentWorkflow(Me.Dam)

			' PK
			daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowRequest("WorkflowControlNo")

			' Acceptance
			daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DateTime.Now
			daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = acceptanceUserId
			daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = acceptanceUserInfo

			' 受付（更新）
			daoT_CurrentWorkflow.D3_Update()

			' --------------------------------------------------
			' 履歴に移動（差戻しに対応するため）
			' --------------------------------------------------
			' T_CurrentWorkflow→T_WorkflowHistory
			' --------------------------------------------------
			Dim dao As New CmnDao(Me.Dam)
			dao.SQLFileName = "RequestApproval_Move.sql"
			dao.SetParameter("WorkflowControlNo", workflowRequest("WorkflowControlNo"))
			dao.ExecInsUpDel_NonQuery()
		End Sub

		''' <summary>処理中ワークフロー依頼を取得します。</summary>
		''' <param name="subSystemId">サブシステムID（任意）</param>
		''' <param name="workflowName">ワークフロー名（任意）</param>
		''' <param name="workflowControlNo">ワークフロー管理番号（任意）</param>
		''' <param name="userId">ワークフローの受信ユーザ（御中指定不可能）</param>
		''' <returns>処理中のワークフロー一覧</returns>
		Public Function GetProcessingWfRequest(subSystemId As String, workflowName As String, workflowControlNo As String, userId As Decimal) As DataTable
			' チェック処理を実装
			' なし。

			' --------------------------------------------------
			' 処理中のワークフローを取得
			' --------------------------------------------------
			' T_CurrentWorkflowのSELECT
			' --------------------------------------------------
			Dim dao As New CmnDao(Me.Dam)
			dao.SQLFileName = "GetProcessingWfRequest.xml"

			' SubSystemId
			If Not String.IsNullOrEmpty(subSystemId) Then
				dao.SetParameter("SubSystemId", subSystemId)
			End If

			' WkflowName
			If Not String.IsNullOrEmpty(workflowName) Then
				dao.SetParameter("WorkflowName", workflowName)
			End If

			' WorkflowControlNo
			If Not String.IsNullOrEmpty(workflowControlNo) Then
				dao.SetParameter("WorkflowControlNo", workflowControlNo)
			End If

			' AcceptanceUserId
			dao.SetParameter("AcceptanceUserId", userId)

			' 処理中ワークフロー依頼を取得
			Dim dt As New DataTable()
			dao.ExecSelectFill_DT(dt)

			' リターン
			Return dt
		End Function

		''' <summary>次のワークフロー依頼を取得します。</summary>
		''' <param name="processingWfReq">選択した処理中ワークフロー依頼</param>
		''' <param name="fromUserId">FromユーザID（必須）</param>
		''' <returns>次のワークフロー</returns>
		''' <remarks>
		''' fromUsersId
		''' 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
		''' </remarks>
		Public Function GetNextWfRequest(processingWfReq As DataRow, fromUserId As Decimal) As DataTable
			'#Region "チェック処理を実装"

			If processingWfReq Is Nothing Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "processingWfReq")))
			ElseIf Not processingWfReq.Table.Columns.Contains("SubSystemId") Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED, "SubSystemId", "processingWfReq")))
			End If

			'#End Region

			' --------------------------------------------------
			' 次のワークフロー依頼を取得
			' --------------------------------------------------
			' M_WorkflowのSELECT
			' --------------------------------------------------
			Dim dt As New DataTable()
			Dim daoM_Workflow As New DaoM_Workflow(Me.Dam)

			' 検索条件
			daoM_Workflow.SubSystemId = processingWfReq("SubSystemId")
			daoM_Workflow.WorkflowName = processingWfReq("WorkflowName")
			daoM_Workflow.WorkflowNo = processingWfReq("NextWorkflowNo")
			daoM_Workflow.FromUserId = fromUserId

			' ワークフローの取得
			daoM_Workflow.D2_Select(dt)

			' --------------------------------------------------
			' TurnBack, Replyワークフローのフィルタ
			' --------------------------------------------------
			' T_WorkflowHistoryのSELECT
			' --------------------------------------------------
			Dim dao As New CmnDao(Me.Dam)

			' 次のワークフロー依頼 = TurnBackの場合のフィルタ処理
			Dim drs As DataRow() = dt.[Select]("ActionType = 'TurnBack'")

			If drs.Length > 1 Then
				' GetTurnBackWorkflow
				' 連続TurnBack場合の対応
				dao.SQLFileName = "GetTurnBackWorkflow.sql"
				dao.SetParameter("WorkflowControlNo", processingWfReq("WorkflowControlNo"))
				dao.SetParameter("NextWorkflowNo", processingWfReq("NextWorkflowNo"))
				Dim temp As Object = dao.ExecSelectScalar()

				' GetTurnBackWorkflow2
				' Start直後にTurnBack場合の対応
				If temp Is Nothing Then
					dao.SQLFileName = "GetTurnBackWorkflow2.sql"
					dao.SetParameter("WorkflowControlNo", processingWfReq("WorkflowControlNo"))
					dao.SetParameter("NextWorkflowNo", processingWfReq("NextWorkflowNo"))
					temp = dao.ExecSelectScalar()
				End If

				Dim wfPositionId As String = DirectCast(temp, String)

				For Each dr As DataRow In drs
							' 対象
					If DirectCast(dr("NextWfPositionId"), String) = wfPositionId Then
					Else
						' 対象外（削除）
						dr.Delete()
					End If
				Next

				' 削除処理の受け入れ
				dt.AcceptChanges()
			End If

			' 次のワークフロー依頼 = Replyの場合のフィルタ処理
			drs = dt.[Select]("ActionType = 'Reply'")

			If drs.Length > 1 Then
				dao.SQLFileName = "GetReplyWorkflow.xml"
				dao.SetParameter("WorkflowControlNo", processingWfReq("WorkflowControlNo"))

				'#Region "CorrespondOfReplyWorkflow"

				Dim alCorrespondOfReplyWorkflow As New ArrayList()

				For Each dr As DataRow In drs
					alCorrespondOfReplyWorkflow.Add(dr("CorrespondOfReplyWorkflow"))
				Next

				dao.SetParameter("CorrespondOfReplyWorkflow", alCorrespondOfReplyWorkflow)

				'#End Region

				Dim workflowNo As Integer = CInt(dao.ExecSelectScalar())

				For Each dr As DataRow In drs
							' これ
					If CInt(dr("CorrespondOfReplyWorkflow")) = workflowNo Then
					Else
						' 削除
						dr.Delete()
					End If
				Next

				' 削除処理の受け入れ
				dt.AcceptChanges()
			End If

			' リターン
			Return dt
		End Function

		''' <summary>差戻しのToユーザIDを履歴から取得します。</summary>
		''' <param name="turnBackWorkflow">差戻しのワークフロー</param>
		''' <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
		''' <returns>ToユーザID</returns>
		Public Function GetTurnBackToUser(turnBackWorkflow As DataRow, workflowControlNo As String) As Decimal
			'#Region "チェック処理を実装"

			If turnBackWorkflow Is Nothing Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "turnBackWorkflow")))
			ElseIf Not turnBackWorkflow.Table.Columns.Contains("SubSystemId") Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED, "SubSystemId", "turnBackWorkflow")))
			ElseIf String.IsNullOrEmpty(workflowControlNo) Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")))
			End If

			'#End Region

			' --------------------------------------------------
			' 差戻しのToユーザIDを履歴から取得
			' --------------------------------------------------
			' T_WorkflowHistoryのSELECT
			' --------------------------------------------------
			Dim dao As New CmnDao(Me.Dam)
			dao.SQLFileName = "GetTurnBackToUser.sql"
			dao.SetParameter("WorkflowControlNo", workflowControlNo)
			dao.SetParameter("ActionType", "TurnBack")
			dao.SetParameter("NextWorkflowNo", turnBackWorkflow("NextWorkflowNo"))

			Return CDec(dao.ExecSelectScalar())
		End Function

		''' <summary>返信のToユーザIDを履歴から取得します。</summary>
		''' <param name="replyBackWorkflow">返信ののワークフロー</param>
		''' <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
		''' <returns>ToユーザID</returns>
		Public Function GetReplyToUser(replyBackWorkflow As DataRow, workflowControlNo As String) As Decimal
			'#Region "チェック処理を実装"

			If replyBackWorkflow Is Nothing Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "replyBackWorkflow")))
			ElseIf Not replyBackWorkflow.Table.Columns.Contains("SubSystemId") Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED, "SubSystemId", "replyBackWorkflow")))
			ElseIf String.IsNullOrEmpty(workflowControlNo) Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")))
			End If

			'#End Region

			' --------------------------------------------------
			' 差戻しのToユーザIDを履歴から取得
			' --------------------------------------------------
			' T_WorkflowHistoryのSELECT
			' --------------------------------------------------
			Dim dao As New CmnDao(Me.Dam)
			dao.SQLFileName = "GetReplyToUser.sql"
			dao.SetParameter("WorkflowControlNo", workflowControlNo)
			dao.SetParameter("CorrespondOfReplyWorkflow", replyBackWorkflow("CorrespondOfReplyWorkflow"))

			Return CDec(dao.ExecSelectScalar())
		End Function

		''' <summary>ワークフロー承認を依頼します。</summary>
		''' <param name="nextWorkflow">選択したワークフロー承認依頼</param>
		''' <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
		''' <param name="fromUserId">FromユーザID（必須）</param>
		''' <param name="toUserId">ToユーザID（TurnBack、Replyの際に必要）</param>
		''' <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
		''' <param name="replyDeadline">回答希望日（任意）</param>
		''' <returns>メール・テンプレートID</returns>
		Public Function RequestWfApproval(nextWorkflow As DataRow, workflowControlNo As String, fromUserId As Decimal, toUserId As System.Nullable(Of Decimal), currentWorkflowReserveArea As String, replyDeadline As System.Nullable(Of DateTime)) As Integer
			'#Region "チェック処理を実装"

			If nextWorkflow Is Nothing Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "nextWorkflow")))
			ElseIf Not nextWorkflow.Table.Columns.Contains("SubSystemId") Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED, "SubSystemId", "nextWorkflow")))
			ElseIf String.IsNullOrEmpty(workflowControlNo) Then
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR(1), [String].Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")))
			End If

			' ユーザIDからユーザ情報を取得
			Dim fromUserInfo As String = Workflow.GetUserInfo(fromUserId)

			Dim toUserInfo As String = ""

			If toUserId.HasValue AndAlso DirectCast(nextWorkflow("ActionType"), String) <> "End" Then
				' Endの時は不要 
				toUserInfo = Workflow.GetUserInfo(toUserId.Value)
			End If

			'#End Region

			' --------------------------------------------------
			' 現在の履歴件数を取得。
			' --------------------------------------------------
			' T_WorkflowHistoryのCount
			' --------------------------------------------------
			Dim dao As New CmnDao(Me.Dam)

			dao.SQLFileName = "RequestApproval_Count.sql"
			dao.SetParameter("WorkflowControlNo", workflowControlNo)
			Dim recordCount As Integer = CInt(dao.ExecSelectScalar())

			' --------------------------------------------------
			' ワークフロー承認を依頼
			' --------------------------------------------------
			' T_CurrentWorkflowのUPDATE
			' --------------------------------------------------
			Dim daoT_CurrentWorkflow As New DaoT_CurrentWorkflow(Me.Dam)

			' 主キー情報
			daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo

			' 履歴番号は履歴件数＋１
			daoT_CurrentWorkflow.Set_HistoryNo_forUPD = recordCount + 1

			daoT_CurrentWorkflow.Set_WfPositionId_forUPD = nextWorkflow("WfPositionId")
			daoT_CurrentWorkflow.Set_WorkflowNo_forUPD = nextWorkflow("WorkflowNo")
			daoT_CurrentWorkflow.Set_FromUserId_forUPD = fromUserId
			' 実際のユーザIDを入力する。
			daoT_CurrentWorkflow.Set_FromUserInfo_forUPD = fromUserInfo
			daoT_CurrentWorkflow.Set_ActionType_forUPD = nextWorkflow("ActionType")

			If toUserId.HasValue AndAlso (DirectCast(nextWorkflow("ActionType"), String) = "TurnBack" OrElse DirectCast(nextWorkflow("ActionType"), String) = "Reply") Then
				' ActionTypeがTurnBack or Replyで、toUserIDがnullで無い場合、
				' 指定のtoUserIDにTurnBack or Replyする。
				daoT_CurrentWorkflow.Set_ToUserId_forUPD = toUserId
			Else
				' 上記以外は、nextWorkflow["ToUserId"]を指定する。
				daoT_CurrentWorkflow.Set_ToUserId_forUPD = nextWorkflow("ToUserId")
			End If

			daoT_CurrentWorkflow.Set_ToUserInfo_forUPD = toUserInfo
			daoT_CurrentWorkflow.Set_ToUserPositionTitlesId_forUPD = nextWorkflow("ToUserPositionTitlesId")
			daoT_CurrentWorkflow.Set_NextWfPositionId_forUPD = nextWorkflow("NextWfPositionId")
			daoT_CurrentWorkflow.Set_NextWorkflowNo_forUPD = nextWorkflow("NextWorkflowNo")
			daoT_CurrentWorkflow.Set_ReserveArea_forUPD = currentWorkflowReserveArea
			'daoT_CurrentWorkflow.Set_ExclusiveKey_forUPD = "";

			If DirectCast(nextWorkflow("ActionType"), String) = "TurnBack" OrElse DirectCast(nextWorkflow("ActionType"), String) = "Reply" Then
				' ActionTypeがTurnBack or Replyの場合
				daoT_CurrentWorkflow.Set_ReplyDeadline_forUPD = DBNull.Value
			Else
				daoT_CurrentWorkflow.Set_ReplyDeadline_forUPD = replyDeadline
			End If

			daoT_CurrentWorkflow.Set_StartDate_forUPD = DateTime.Now
			daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DBNull.Value
			daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = DBNull.Value
			daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = DBNull.Value

			daoT_CurrentWorkflow.D3_Update()

			' --------------------------------------------------
			' 完了（T_WorkflowHistoryのEndDate項目を更新）
			' --------------------------------------------------
			' T_WorkflowHistoryのUPDATE
			' --------------------------------------------------
			Dim daoT_WorkflowHistory As New DaoT_WorkflowHistory(Me.Dam)

			' PK
			daoT_WorkflowHistory.PK_WorkflowControlNo = workflowControlNo
			daoT_WorkflowHistory.PK_HistoryNo = recordCount

			' EndDate
			daoT_WorkflowHistory.Set_EndDate_forUPD = DateTime.Now

			daoT_WorkflowHistory.S3_Update()

			'---

			' 完了
			If DirectCast(nextWorkflow("ActionType"), String) = "End" Then
				' --------------------------------------------------
				' 完了の場合（T_WorkflowのEndDate項目を更新）
				' --------------------------------------------------
				' T_WorkflowのUPDATE
				' --------------------------------------------------
				Dim daoT_Workflow As New DaoT_Workflow(Me.Dam)

				' PK
				daoT_Workflow.PK_WorkflowControlNo = workflowControlNo

				' EndDate
				daoT_Workflow.Set_EndDate_forUPD = DateTime.Now

				daoT_Workflow.S3_Update()

				' --------------------------------------------------
				' 履歴に移動
				' --------------------------------------------------
				' T_CurrentWorkflow→T_WorkflowHistory
				' --------------------------------------------------
				dao.SQLFileName = "RequestApproval_Move.sql"
				dao.SetParameter("WorkflowControlNo", workflowControlNo)
				dao.ExecInsUpDel_NonQuery()
			End If

			' リターン（MailTemplateId）
            If IsDBNull(nextWorkflow("MailTemplateId")) Then
                Return 0
            Else
                Return CInt(nextWorkflow("MailTemplateId"))
            End If
		End Function
	End Class
End Namespace
