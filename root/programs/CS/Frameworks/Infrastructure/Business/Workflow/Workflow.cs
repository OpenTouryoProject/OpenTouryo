//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：Workflow
//* クラス日本語名  ：ワークフロー
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/07/15  西野 大介         新規作成
//*  2014/11/11  Sai-san           Created methods for Forced termination, Getting SlipIssuanceUserID, 
//*                                TurnBack to Original user using the voucher and Switching person in charge
//**********************************************************************************

using System;
using System.Data;
using System.Collections;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Business.Workflow
{
    /// <summary>デリゲート型を定義</summary>
    public delegate string GetUserInfoDelegate(decimal userID);

    /// <summary>ワークフロークラス</summary>
    public class Workflow : BaseConsolidateDao
    {
        /// <summary>デリゲート静的変数</summary>
        public static GetUserInfoDelegate GetUserInfo;

        /// <summary>コンストラクタ</summary>
        /// <param name="dam">データアクセス制御クラス</param>
        public Workflow(BaseDam dam)
            : base(dam)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        /// <summary>新しいワークフローを準備します。</summary>
        /// <param name="subSystemId">サブシステムID（必須）</param>
        /// <param name="workflowName">ワークフロー名（必須）</param>
        /// <param name="fromUserId">FromユーザID（必須）</param>
        /// <returns>新規ワークフロー</returns>
        /// <remarks>
        /// fromUsersId
        /// 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
        /// </remarks>
        public DataTable PreStartWorkflow(
            string subSystemId, string workflowName, decimal fromUserId)
        {
            #region チェック処理を実装

            if (string.IsNullOrEmpty(subSystemId))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "subSystemId")));
            }
            else if (string.IsNullOrEmpty(workflowName))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowName")));
            }

            #endregion

            // --------------------------------------------------
            // 新しいワークフローを準備
            // --------------------------------------------------
            // M_WorkflowのSELECT
            // --------------------------------------------------
            DataTable dt = new DataTable();
            DaoM_Workflow daoM_Workflow = new DaoM_Workflow(this.Dam);

            // 検索条件
            daoM_Workflow.SubSystemId = subSystemId;
            daoM_Workflow.WorkflowName = workflowName;
            //daoM_Workflow.WorkflowNo = 1; // スタートなので「1」固定
            daoM_Workflow.ActionType = "Start"; // スタートなので"Start"
            daoM_Workflow.FromUserId = fromUserId;

            // ワークフローの取得
            daoM_Workflow.D2_Select(dt);
            return dt;
        }

        /// <summary>新しいワークフローを開始します。</summary>
        /// <param name="startWorkflow">新規ワークフロー</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <param name="fromUserId">FromユーザID（個人ID 必須）</param>
        /// <param name="toUserId">ToユーザID（個人ID 任意）</param>
        /// <param name="workflowReserveArea">T_Workflowの予備領域（任意）</param>
        /// <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
        /// <param name="replyDeadline">回答希望日（任意）</param>
        /// <returns>メール・テンプレートID</returns>
        public int StartWorkflow(DataRow startWorkflow,
            string workflowControlNo, decimal fromUserId, decimal toUserId,
            string workflowReserveArea, string currentWorkflowReserveArea, DateTime? replyDeadline)
        {
            #region チェック処理を実装

            if (string.IsNullOrEmpty(workflowControlNo))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")));
            }

            #endregion

            // ユーザIDからユーザ情報を取得
            string fromUserInfo = Workflow.GetUserInfo(fromUserId);
            string toUserInfo = string.Empty;
            if (toUserId == 0)
            {
                toUserInfo = Workflow.GetUserInfo((decimal)startWorkflow["ToUserId"]);
                toUserId = (decimal)startWorkflow["ToUserId"];
            }
            else
            {
                toUserInfo = Workflow.GetUserInfo(toUserId);
            }

            // --------------------------------------------------
            // 新しいワークフローを開始
            // --------------------------------------------------
            // T_WorkflowへのINSERT
            // --------------------------------------------------
            DaoT_Workflow daoT_Workflow = new DaoT_Workflow(this.Dam);

            daoT_Workflow.PK_WorkflowControlNo = workflowControlNo;
            daoT_Workflow.SubSystemId = startWorkflow["SubSystemId"];
            daoT_Workflow.WorkflowName = startWorkflow["WorkflowName"];
            daoT_Workflow.UserId = fromUserId; // 個人IDが必要。
            daoT_Workflow.UserInfo = fromUserInfo;
            daoT_Workflow.ReserveArea = workflowReserveArea;
            daoT_Workflow.StartDate = DateTime.Now;
            //daoT_Workflow.EndDate = DBNull.Value;

            daoT_Workflow.D1_Insert();

            // --------------------------------------------------
            // T_CurrentWorkflowへのINSERT
            // --------------------------------------------------
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo;
            daoT_CurrentWorkflow.HistoryNo = 1; // スタートなので「1」固定
            daoT_CurrentWorkflow.WfPositionId = startWorkflow["WfPositionId"];
            daoT_CurrentWorkflow.WorkflowNo = startWorkflow["WorkflowNo"];
            daoT_CurrentWorkflow.FromUserId = fromUserId; // 実際のユーザIDを入力する。
            daoT_CurrentWorkflow.FromUserInfo = fromUserInfo; // ユーザ入力が必要。
            daoT_CurrentWorkflow.ActionType = startWorkflow["ActionType"];
            daoT_CurrentWorkflow.ToUserId = toUserId;
            daoT_CurrentWorkflow.ToUserInfo = toUserInfo; // ユーザ入力が必要。
            daoT_CurrentWorkflow.ToUserPositionTitlesId = startWorkflow["ToUserPositionTitlesId"];
            daoT_CurrentWorkflow.NextWfPositionId = startWorkflow["NextWfPositionId"];
            daoT_CurrentWorkflow.NextWorkflowNo = startWorkflow["NextWorkflowNo"];
            daoT_CurrentWorkflow.ReserveArea = currentWorkflowReserveArea;
            //daoT_CurrentWorkflow.ExclusiveKey = "";
            daoT_CurrentWorkflow.ReplyDeadline = replyDeadline;
            daoT_CurrentWorkflow.StartDate = DateTime.Now;
            //daoT_CurrentWorkflow.AcceptanceDate = DBNull.Value;
            //daoT_CurrentWorkflow.AcceptanceUserId = DBNull.Value;
            //daoT_CurrentWorkflow.AcceptanceUserInfo = DBNull.Value;

            daoT_CurrentWorkflow.D1_Insert();

            // リターン（MailTemplateId）
            if (startWorkflow["MailTemplateId"] == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return (int)startWorkflow["MailTemplateId"];
            }
        }

        /// <summary>ワークフロー依頼を取得します。</summary>
        /// <param name="subSystemId">サブシステムID(任意)</param>
        /// <param name="workflowName">ワークフロー名(任意)</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（任意）</param>
        /// <param name="userId">ワークフローの受信ユーザ（必須）</param>
        /// <param name="userPositionTitlesId">
        /// ユーザの職位ID（userIdが御中IDの場合は必須）
        /// </param>
        /// <returns>ワークフロー依頼の一覧</returns>
        /// <remarks>
        /// fromUsersId
        /// 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
        /// </remarks>
        public DataTable GetWfRequest(
            string subSystemId, string workflowName, string workflowControlNo,
            decimal? userId, int? userPositionTitlesId)
        {
            // チェック処理を実装
            // なし。

            // --------------------------------------------------
            // ワークフローの依頼を取得
            // --------------------------------------------------
            // T_CurrentWorkflowのSELECT
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);

            dao.SQLFileName = "GetWfRequest.xml";

            // SubSystemId
            if (!string.IsNullOrEmpty(subSystemId))
            {
                dao.SetParameter("SubSystemId", subSystemId);
            }

            // WkflowName
            if (!string.IsNullOrEmpty(workflowName))
            {
                dao.SetParameter("WkflowName", workflowName);
            }

            // WorkflowControlNo
            if (!string.IsNullOrEmpty(workflowControlNo))
            {
                dao.SetParameter("WorkflowControlNo", workflowControlNo);
            }

            // ユーザID（必須）
            if (userId.HasValue)
            {
                dao.SetParameter("ToUserId", userId);
            }

            // ユーザの職位ID
            if (userPositionTitlesId.HasValue)
            {
                dao.SetParameter("ToUserPositionTitlesId", userPositionTitlesId);
            }

            // ワークフロー依頼を取得
            DataTable dt = new DataTable();
            dao.ExecSelectFill_DT(dt);

            // リターン
            return dt;
        }

        /// <summary>ワークフロー依頼を受付ます。</summary>
        /// <param name="workflowRequest">選択したワークフロー依頼</param>
        /// <param name="acceptanceUserId">受付ユーザID</param>
        public void AcceptWfRequest(DataRow workflowRequest, decimal acceptanceUserId)
        {
            #region チェック処理を実装

            if (workflowRequest == null)
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowRequest")));
            }
            else if (!workflowRequest.Table.Columns.Contains("WorkflowControlNo"))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "WorkflowControlNo", "workflowRequest")));
            }

            #endregion

            // ユーザIDからユーザ情報を取得
            string acceptanceUserInfo = Workflow.GetUserInfo(acceptanceUserId);

            // --------------------------------------------------
            // 受付（T_CurrentWorkflowのacceptance項目を更新）
            // --------------------------------------------------
            // T_CurrentWorkflowのUPDATE
            // --------------------------------------------------
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            // PK
            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowRequest["WorkflowControlNo"];

            // Acceptance
            daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DateTime.Now;
            daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = acceptanceUserId;
            daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = acceptanceUserInfo;

            // 受付（更新）
            daoT_CurrentWorkflow.D3_Update();

            // --------------------------------------------------
            // 履歴に移動（差戻しに対応するため）
            // --------------------------------------------------
            // T_CurrentWorkflow→T_WorkflowHistory
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);
            dao.SQLFileName = "RequestApproval_Move.sql";
            dao.SetParameter("WorkflowControlNo", workflowRequest["WorkflowControlNo"]);
            dao.ExecInsUpDel_NonQuery();
        }

        /// <summary>処理中ワークフロー依頼を取得します。</summary>
        /// <param name="subSystemId">サブシステムID（任意）</param>
        /// <param name="workflowName">ワークフロー名（任意）</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（任意）</param>
        /// <param name="userId">ワークフローの受信ユーザ（御中指定不可能）</param>
        /// <returns>処理中のワークフロー一覧</returns>
        public DataTable GetProcessingWfRequest(
            string subSystemId, string workflowName, string workflowControlNo, decimal userId)
        {
            // チェック処理を実装
            // なし。

            // --------------------------------------------------
            // 処理中のワークフローを取得
            // --------------------------------------------------
            // T_CurrentWorkflowのSELECT
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);
            dao.SQLFileName = "GetProcessingWfRequest.xml";

            // SubSystemId
            if (!string.IsNullOrEmpty(subSystemId))
            {
                dao.SetParameter("SubSystemId", subSystemId);
            }

            // WkflowName
            if (!string.IsNullOrEmpty(workflowName))
            {
                dao.SetParameter("WorkflowName", workflowName);
            }

            // WorkflowControlNo
            if (!string.IsNullOrEmpty(workflowControlNo))
            {
                dao.SetParameter("WorkflowControlNo", workflowControlNo);
            }

            // AcceptanceUserId
            dao.SetParameter("AcceptanceUserId", userId);

            // 処理中ワークフロー依頼を取得
            DataTable dt = new DataTable();
            dao.ExecSelectFill_DT(dt);

            // リターン
            return dt;
        }

        /// <summary>次のワークフロー依頼を取得します。</summary>
        /// <param name="processingWfReq">選択した処理中ワークフロー依頼</param>
        /// <param name="fromUserId">FromユーザID（必須）</param>
        /// <returns>次のワークフロー</returns>
        /// <remarks>
        /// fromUsersId
        /// 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
        /// </remarks>
        public DataTable GetNextWfRequest(DataRow processingWfReq, decimal fromUserId)
        {
            #region チェック処理を実装

            if (processingWfReq == null)
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "processingWfReq")));
            }
            else if (!processingWfReq.Table.Columns.Contains("SubSystemId"))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "SubSystemId", "processingWfReq")));
            }

            #endregion

            // --------------------------------------------------
            // 次のワークフロー依頼を取得
            // --------------------------------------------------
            // M_WorkflowのSELECT
            // --------------------------------------------------
            DataTable dt = new DataTable();
            DaoM_Workflow daoM_Workflow = new DaoM_Workflow(this.Dam);

            // 検索条件
            daoM_Workflow.SubSystemId = processingWfReq["SubSystemId"];
            daoM_Workflow.WorkflowName = processingWfReq["WorkflowName"];
            daoM_Workflow.WorkflowNo = processingWfReq["NextWorkflowNo"];
            daoM_Workflow.FromUserId = fromUserId;

            // ワークフローの取得
            daoM_Workflow.D2_Select(dt);

            // --------------------------------------------------
            // TurnBack, Replyワークフローのフィルタ
            // --------------------------------------------------
            // T_WorkflowHistoryのSELECT
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);

            // 次のワークフロー依頼 = TurnBackの場合のフィルタ処理
            DataRow[] drs = dt.Select("ActionType = 'TurnBack'");

            if (drs.Length > 1)
            {
                // GetTurnBackWorkflow
                // 連続TurnBack場合の対応
                dao.SQLFileName = "GetTurnBackWorkflow.sql";
                dao.SetParameter("WorkflowControlNo", processingWfReq["WorkflowControlNo"]);
                dao.SetParameter("NextWorkflowNo", processingWfReq["NextWorkflowNo"]);
                object temp = dao.ExecSelectScalar();

                // GetTurnBackWorkflow2
                // Start直後にTurnBack場合の対応
                if (temp == null)
                {
                    dao.SQLFileName = "GetTurnBackWorkflow2.sql";
                    dao.SetParameter("WorkflowControlNo", processingWfReq["WorkflowControlNo"]);
                    dao.SetParameter("NextWorkflowNo", processingWfReq["NextWorkflowNo"]);
                    temp = dao.ExecSelectScalar();
                }

                string wfPositionId = (string)temp;

                foreach (DataRow dr in drs)
                {
                    if ((string)dr["NextWfPositionId"] == wfPositionId)
                    {
                        // 対象
                    }
                    else
                    {
                        // 対象外（削除）
                        dr.Delete();
                    }
                }

                // 削除処理の受け入れ
                dt.AcceptChanges();
            }

            // 次のワークフロー依頼 = Replyの場合のフィルタ処理
            drs = dt.Select("ActionType = 'Reply'");

            if (drs.Length > 1)
            {
                dao.SQLFileName = "GetReplyWorkflow.xml";
                dao.SetParameter("WorkflowControlNo", processingWfReq["WorkflowControlNo"]);

                #region CorrespondOfReplyWorkflow

                ArrayList alCorrespondOfReplyWorkflow = new ArrayList();

                foreach (DataRow dr in drs)
                {
                    alCorrespondOfReplyWorkflow.Add(dr["CorrespondOfReplyWorkflow"]);
                }

                dao.SetParameter("CorrespondOfReplyWorkflow", alCorrespondOfReplyWorkflow);

                #endregion

                int workflowNo = (int)dao.ExecSelectScalar();

                foreach (DataRow dr in drs)
                {
                    if ((int)dr["CorrespondOfReplyWorkflow"] == workflowNo)
                    {
                        // これ
                    }
                    else
                    {
                        // 削除
                        dr.Delete();
                    }
                }

                // 削除処理の受け入れ
                dt.AcceptChanges();
            }

            // リターン
            return dt;
        }

        /// <summary>差戻しのToユーザIDを履歴から取得します。</summary>
        /// <param name="turnBackWorkflow">差戻しのワークフロー</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <returns>ToユーザID</returns>
        public decimal GetTurnBackToUser(DataRow turnBackWorkflow, string workflowControlNo)
        {
            #region チェック処理を実装

            if (turnBackWorkflow == null)
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "turnBackWorkflow")));
            }
            else if (!turnBackWorkflow.Table.Columns.Contains("SubSystemId"))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "SubSystemId", "turnBackWorkflow")));
            }
            else if (string.IsNullOrEmpty(workflowControlNo))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")));
            }

            #endregion

            // --------------------------------------------------
            // 差戻しのToユーザIDを履歴から取得
            // --------------------------------------------------
            // T_WorkflowHistoryのSELECT
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);
            dao.SQLFileName = "GetTurnBackToUser.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            dao.SetParameter("ActionType", "TurnBack");
            dao.SetParameter("NextWorkflowNo", turnBackWorkflow["NextWorkflowNo"]);

            return (decimal)dao.ExecSelectScalar();
        }

        /// <summary>返信のToユーザIDを履歴から取得します。</summary>
        /// <param name="replyBackWorkflow">返信ののワークフロー</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <returns>ToユーザID</returns>
        public decimal GetReplyToUser(DataRow replyBackWorkflow, string workflowControlNo)
        {
            #region チェック処理を実装

            if (replyBackWorkflow == null)
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "replyBackWorkflow")));
            }
            else if (!replyBackWorkflow.Table.Columns.Contains("SubSystemId"))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "SubSystemId", "replyBackWorkflow")));
            }
            else if (string.IsNullOrEmpty(workflowControlNo))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")));
            }

            #endregion

            // --------------------------------------------------
            // 差戻しのToユーザIDを履歴から取得
            // --------------------------------------------------
            // T_WorkflowHistoryのSELECT
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);
            dao.SQLFileName = "GetReplyToUser.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            dao.SetParameter("CorrespondOfReplyWorkflow", replyBackWorkflow["CorrespondOfReplyWorkflow"]);

            return (decimal)dao.ExecSelectScalar();
        }

        /// <summary>ワークフロー承認を依頼します。</summary>
        /// <param name="nextWorkflow">選択したワークフロー承認依頼</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <param name="fromUserId">FromユーザID（必須）</param>
        /// <param name="toUserId">ToユーザID（TurnBack、Replyの際に必要）</param>
        /// <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
        /// <param name="replyDeadline">回答希望日（任意）</param>
        /// <returns>メール・テンプレートID</returns>
        public int RequestWfApproval(
            DataRow nextWorkflow, string workflowControlNo,
            decimal fromUserId, decimal? toUserId,
            string currentWorkflowReserveArea, DateTime? replyDeadline)
        {
            #region チェック処理を実装

            if (nextWorkflow == null)
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "nextWorkflow")));
            }
            else if (!nextWorkflow.Table.Columns.Contains("SubSystemId"))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "SubSystemId", "nextWorkflow")));
            }
            else if (string.IsNullOrEmpty(workflowControlNo))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")));
            }

            // ユーザIDからユーザ情報を取得
            string fromUserInfo = Workflow.GetUserInfo(fromUserId);

            string toUserInfo = "";

            if (toUserId.HasValue
                && (string)nextWorkflow["ActionType"] != "End") // Endの時は不要 
            {
                toUserInfo = Workflow.GetUserInfo(toUserId.Value);
            }

            #endregion

            // --------------------------------------------------
            // 現在の履歴件数を取得。
            // --------------------------------------------------
            // T_WorkflowHistoryのCount
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);

            dao.SQLFileName = "RequestApproval_Count.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            int recordCount = ((int)dao.ExecSelectScalar());

            // --------------------------------------------------
            // ワークフロー承認を依頼
            // --------------------------------------------------
            // T_CurrentWorkflowのUPDATE
            // --------------------------------------------------
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            // 主キー情報
            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo;

            // 履歴番号は履歴件数＋１
            daoT_CurrentWorkflow.Set_HistoryNo_forUPD = recordCount + 1;

            daoT_CurrentWorkflow.Set_WfPositionId_forUPD = nextWorkflow["WfPositionId"];
            daoT_CurrentWorkflow.Set_WorkflowNo_forUPD = nextWorkflow["WorkflowNo"];
            daoT_CurrentWorkflow.Set_FromUserId_forUPD = fromUserId; // 実際のユーザIDを入力する。
            daoT_CurrentWorkflow.Set_FromUserInfo_forUPD = fromUserInfo;
            daoT_CurrentWorkflow.Set_ActionType_forUPD = nextWorkflow["ActionType"];

            if (toUserId.HasValue
                && ((string)nextWorkflow["ActionType"] == "TurnBack" || (string)nextWorkflow["ActionType"] == "Reply"))
            {
                // ActionTypeがTurnBack or Replyで、toUserIDがnullで無い場合、
                // 指定のtoUserIDにTurnBack or Replyする。
                daoT_CurrentWorkflow.Set_ToUserId_forUPD = toUserId;
            }
            else
            {
                // 上記以外は、nextWorkflow["ToUserId"]を指定する。
                daoT_CurrentWorkflow.Set_ToUserId_forUPD = nextWorkflow["ToUserId"];
            }

            daoT_CurrentWorkflow.Set_ToUserInfo_forUPD = toUserInfo;
            daoT_CurrentWorkflow.Set_ToUserPositionTitlesId_forUPD = nextWorkflow["ToUserPositionTitlesId"];
            daoT_CurrentWorkflow.Set_NextWfPositionId_forUPD = nextWorkflow["NextWfPositionId"];
            daoT_CurrentWorkflow.Set_NextWorkflowNo_forUPD = nextWorkflow["NextWorkflowNo"];
            daoT_CurrentWorkflow.Set_ReserveArea_forUPD = currentWorkflowReserveArea;
            //daoT_CurrentWorkflow.Set_ExclusiveKey_forUPD = "";

            if ((string)nextWorkflow["ActionType"] == "TurnBack"
                || (string)nextWorkflow["ActionType"] == "Reply")
            {
                // ActionTypeがTurnBack or Replyの場合
                daoT_CurrentWorkflow.Set_ReplyDeadline_forUPD = DBNull.Value;
            }
            else
            {
                daoT_CurrentWorkflow.Set_ReplyDeadline_forUPD = replyDeadline;
            }

            daoT_CurrentWorkflow.Set_StartDate_forUPD = DateTime.Now;
            daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = DBNull.Value;

            daoT_CurrentWorkflow.D3_Update();

            // --------------------------------------------------
            // 完了（T_WorkflowHistoryのEndDate項目を更新）
            // --------------------------------------------------
            // T_WorkflowHistoryのUPDATE
            // --------------------------------------------------
            DaoT_WorkflowHistory daoT_WorkflowHistory = new DaoT_WorkflowHistory(this.Dam);

            // PK
            daoT_WorkflowHistory.PK_WorkflowControlNo = workflowControlNo;
            daoT_WorkflowHistory.PK_HistoryNo = recordCount;

            // EndDate
            daoT_WorkflowHistory.Set_EndDate_forUPD = DateTime.Now;

            daoT_WorkflowHistory.S3_Update();

            //---

            // 完了
            if ((string)nextWorkflow["ActionType"] == "End")
            {
                // --------------------------------------------------
                // 完了の場合（T_WorkflowのEndDate項目を更新）
                // --------------------------------------------------
                // T_WorkflowのUPDATE
                // --------------------------------------------------
                DaoT_Workflow daoT_Workflow = new DaoT_Workflow(this.Dam);

                // PK
                daoT_Workflow.PK_WorkflowControlNo = workflowControlNo;

                // EndDate
                daoT_Workflow.Set_EndDate_forUPD = DateTime.Now;

                daoT_Workflow.S3_Update();

                // --------------------------------------------------
                // 履歴に移動
                // --------------------------------------------------
                // T_CurrentWorkflow→T_WorkflowHistory
                // --------------------------------------------------
                dao.SQLFileName = "RequestApproval_Move.sql";
                dao.SetParameter("WorkflowControlNo", workflowControlNo);
                dao.ExecInsUpDel_NonQuery();
            }

            // リターン（MailTemplateId）
            if (nextWorkflow["MailTemplateId"] == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return (int)nextWorkflow["MailTemplateId"];
            }
        }

        /// <summary>
        /// This method TurnBack to slip iisuance UserID
        /// </summary>
        /// <param name="subsystemId"></param>
        /// <param name="workflowControlNo"></param>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="currentWorkflowReserveArea"></param>
        /// <returns></returns>
        public void TurnbackSlipIssuanceUserID(string subsystemId, string workflowControlNo, decimal fromUserId, decimal? toUserId,
                                              string currentWorkflowReserveArea)
        {
            #region チェック処理を実装

            string toUserInfo = "";

            //If ToUserId is not null ans ActionType is End then getting user information
            if (toUserId.HasValue) // Endの時は不要 
            {
                toUserInfo = Workflow.GetUserInfo(toUserId.Value);
            }

            #endregion

            // --------------------------------------------------
            // 現在の履歴件数を取得。
            // --------------------------------------------------
            //Gets the record count from T_WorkflowHistory table
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);

            dao.SQLFileName = "RequestApproval_Count.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            int recordCount = ((int)dao.ExecSelectScalar());

            // --------------------------------------------------
            // ワークフロー承認を依頼
            // --------------------------------------------------
            // T_CurrentWorkflowのUPDATE
            // --------------------------------------------------
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            // 主キー情報
            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo;

            // 履歴番号は履歴件数＋１
            daoT_CurrentWorkflow.Set_HistoryNo_forUPD = recordCount + 1;

            DaoT_WorkflowHistory daoT_WorkflowHistory = new DaoT_WorkflowHistory(this.Dam);
            //Gets the Slip issuance orignal user id
            DataTable dt = GetSlipIssuanceUserID(subsystemId, workflowControlNo);

            //Updates the T_CurrentWorkflow and T_WorkflowHistory tables data with original slip issuance user information
            daoT_CurrentWorkflow.Set_WfPositionId_forUPD = dt.Rows[0]["WfPositionId"];
            daoT_CurrentWorkflow.Set_WorkflowNo_forUPD = dt.Rows[0]["WorkflowNo"];
            daoT_CurrentWorkflow.Set_FromUserId_forUPD = toUserId; // 実際のユーザIDを入力する。
            daoT_CurrentWorkflow.Set_FromUserInfo_forUPD = toUserInfo;
            //Updating action type to TurnBack
            daoT_CurrentWorkflow.Set_ActionType_forUPD = "TurnBack";

            daoT_CurrentWorkflow.Set_ToUserId_forUPD = dt.Rows[0]["FromUserId"];
            // ユーザIDからユーザ情報を取得
            string fromUserInfo = Workflow.GetUserInfo(fromUserId);
            daoT_CurrentWorkflow.Set_ToUserInfo_forUPD = fromUserInfo;
            daoT_CurrentWorkflow.Set_ToUserPositionTitlesId_forUPD = dt.Rows[0]["ToUserPositionTitlesId"];
            daoT_CurrentWorkflow.Set_NextWfPositionId_forUPD = dt.Rows[0]["NextWfPositionId"];
            daoT_CurrentWorkflow.Set_NextWorkflowNo_forUPD = dt.Rows[0]["NextWorkflowNo"];
            daoT_CurrentWorkflow.Set_ReserveArea_forUPD = currentWorkflowReserveArea;

            //Updating acceptance information with null
            daoT_CurrentWorkflow.Set_ReplyDeadline_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_StartDate_forUPD = DateTime.Now;
            daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.D3_Update();

            // PK
            daoT_WorkflowHistory.PK_WorkflowControlNo = workflowControlNo;
            daoT_WorkflowHistory.PK_HistoryNo = recordCount;

            // EndDate
            daoT_WorkflowHistory.Set_EndDate_forUPD = DateTime.Now;

            daoT_WorkflowHistory.S3_Update();
        }

        /// <summary>
        /// Gets the Slip Issuance UserID of History=1 to TurnBack
        /// </summary>
        /// <param name="subSystemId"></param>
        /// <param name="workflowControlNo"></param>
        /// <returns></returns>
        private DataTable GetSlipIssuanceUserID(string subSystemId, string workflowControlNo)
        {
            #region チェック処理を実装

            if (string.IsNullOrEmpty(subSystemId))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "SubSystemId", "turnBackWorkflow")));
            }
            else if (string.IsNullOrEmpty(workflowControlNo))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")));
            }

            #endregion

            // --------------------------------------------------
            // 差戻しのToユーザIDを履歴から取得
            // --------------------------------------------------
            //Executes the select query of T_WorkflowHistory for getting the original slip issuance user information
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);
            dao.SQLFileName = "GetTurnBackFromUserHistory.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            dao.SetParameter("ActionType", "TurnBack");
            DataTable dt = new DataTable();
            dao.ExecSelectFill_DT(dt);
            return dt;
        }

        /// <summary>
        /// This method is used to terminate the workflow forecefully 
        /// by updating EndDate column of T_Workflow table with enddate.
        /// </summary>
        /// <param name="nextWorkflow"></param>
        /// <param name="workflowControlNo"></param>
        /// <param name="fromUserId"></param>        
        /// <param name="currentWorkflowReserveArea"></param>
        /// <returns></returns>
        public int ForcedTermination(DataRow nextWorkflow, string workflowControlNo, decimal fromUserId, string currentWorkflowReserveArea)
        {
            #region チェック処理を実装

            if (nextWorkflow == null)
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "nextWorkflow")));
            }
            else if (!nextWorkflow.Table.Columns.Contains("SubSystemId"))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "SubSystemId", "nextWorkflow")));
            }
            else if (string.IsNullOrEmpty(workflowControlNo))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")));
            }

            // ユーザIDからユーザ情報を取得
            string fromUserInfo = Workflow.GetUserInfo(fromUserId);

            string toUserInfo = "";

            #endregion

            // --------------------------------------------------
            // 現在の履歴件数を取得。
            // --------------------------------------------------
            //Gets the record count from T_WorkflowHistory table
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);

            dao.SQLFileName = "RequestApproval_Count.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            int recordCount = ((int)dao.ExecSelectScalar());

            // --------------------------------------------------
            // ワークフロー承認を依頼
            // --------------------------------------------------
            // T_CurrentWorkflowのUPDATE
            // --------------------------------------------------
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            // 主キー情報
            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo;

            // 履歴番号は履歴件数＋１
            daoT_CurrentWorkflow.Set_HistoryNo_forUPD = recordCount + 1;

            daoT_CurrentWorkflow.Set_WfPositionId_forUPD = nextWorkflow["WfPositionId"];
            daoT_CurrentWorkflow.Set_WorkflowNo_forUPD = nextWorkflow["WorkflowNo"];
            daoT_CurrentWorkflow.Set_FromUserId_forUPD = fromUserId; // 実際のユーザIDを入力する。
            daoT_CurrentWorkflow.Set_FromUserInfo_forUPD = fromUserInfo;
            //Updates the ActionType with Abnormal termination
            daoT_CurrentWorkflow.Set_ActionType_forUPD = "ABEnd";

            // 上記以外は、nextWorkflow["ToUserId"]を指定する。
            daoT_CurrentWorkflow.Set_ToUserId_forUPD = nextWorkflow["ToUserId"];

            daoT_CurrentWorkflow.Set_ToUserInfo_forUPD = toUserInfo;
            daoT_CurrentWorkflow.Set_ToUserPositionTitlesId_forUPD = nextWorkflow["ToUserPositionTitlesId"];
            daoT_CurrentWorkflow.Set_NextWfPositionId_forUPD = nextWorkflow["NextWfPositionId"];
            daoT_CurrentWorkflow.Set_NextWorkflowNo_forUPD = nextWorkflow["NextWorkflowNo"];
            daoT_CurrentWorkflow.Set_ReserveArea_forUPD = currentWorkflowReserveArea;

            daoT_CurrentWorkflow.Set_StartDate_forUPD = DateTime.Now;
            daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = DBNull.Value;

            daoT_CurrentWorkflow.D3_Update();

            // --------------------------------------------------
            // 完了（T_WorkflowHistoryのEndDate項目を更新）
            // --------------------------------------------------
            // T_WorkflowHistoryのUPDATE
            // --------------------------------------------------
            DaoT_WorkflowHistory daoT_WorkflowHistory = new DaoT_WorkflowHistory(this.Dam);

            // PK
            daoT_WorkflowHistory.PK_WorkflowControlNo = workflowControlNo;
            daoT_WorkflowHistory.PK_HistoryNo = recordCount;

            //Updates the EndDate to current date for forceful termination
            daoT_WorkflowHistory.Set_EndDate_forUPD = DateTime.Now;

            daoT_WorkflowHistory.S3_Update();

            //---

            // 完了
            // --------------------------------------------------
            // 完了の場合（T_WorkflowのEndDate項目を更新）
            // --------------------------------------------------
            // T_WorkflowのUPDATE
            // --------------------------------------------------
            DaoT_Workflow daoT_Workflow = new DaoT_Workflow(this.Dam);

            // PK
            daoT_Workflow.PK_WorkflowControlNo = workflowControlNo;

            //Updates the EndDate to current date for forceful termination
            daoT_Workflow.Set_EndDate_forUPD = DateTime.Now;

            daoT_Workflow.S3_Update();

            // --------------------------------------------------
            // 履歴に移動
            // --------------------------------------------------
            // T_CurrentWorkflow→T_WorkflowHistory
            // --------------------------------------------------
            dao.SQLFileName = "RequestApproval_Move.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            dao.ExecInsUpDel_NonQuery();

            // リターン（MailTemplateId）
            if (nextWorkflow["MailTemplateId"] == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return (int)nextWorkflow["MailTemplateId"];
            }
        }

        /// <summary>
        /// This method is used to accept the workflow by different users once the workflow is accepted 
        /// by updating acceptance columns of T_CurrentWorkflow and T_WorkflowHistory table with null.
        /// </summary>
        /// <param name="nextWorkflow"></param>
        /// <param name="workflowControlNo"></param>        
        /// <returns></returns>
        public void SwitchPersonInCharge(DataRow nextWorkflow, string workflowControlNo)
        {

            #region チェック処理を実装

            if (nextWorkflow == null)
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "nextWorkflow")));
            }
            else if (!nextWorkflow.Table.Columns.Contains("SubSystemId"))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED,
                        "SubSystemId", "nextWorkflow")));
            }
            else if (string.IsNullOrEmpty(workflowControlNo))
            {
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR[1],
                        String.Format(MyBusinessSystemExceptionMessage.WORKFLOW_ERROR_CHECK_EMPTY, "workflowControlNo")));
            }

            #endregion

            //Updates the acceptance information with null in T_CurrentWorkflow table for many users acceptance            
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            // 主キー情報
            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo;

            daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = DBNull.Value;
            daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = DBNull.Value;

            daoT_CurrentWorkflow.D3_Update();
        }
    }
}
