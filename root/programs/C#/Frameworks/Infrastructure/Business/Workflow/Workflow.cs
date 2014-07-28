//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//*  2014/07/15  西野  大介        新規作成
//*
//**********************************************************************************

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;

namespace Touryo.Infrastructure.Business.Workflow
{
    /// <summary>ワークフロークラス</summary>
    public class Workflow : BaseConsolidateDao
    {
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
            daoM_Workflow.WorkflowNo = 1; // スタートなので「1」固定
            daoM_Workflow.FromUserId = fromUserId;
            
            // ワークフローの取得
            daoM_Workflow.D2_Select(dt);
            return dt;
        }

        /// <summary>新しいワークフローを開始します。</summary>
        /// <param name="startWorkflow">新規ワークフロー</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <param name="fromUserId">FromユーザID（個人ID 必須）</param>
        /// <param name="fromUserInfo">Fromユーザ情報（個人ID 必須）</param>
        /// <param name="toUserInfo">Toユーザ情報（必須）</param>
        /// <param name="workflowReserveArea">T_Workflowの予備領域（任意）</param>
        /// <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
        /// <param name="replyDeadline">回答希望日（任意）</param>
        /// <returns>メール・テンプレートID</returns>
        public int StartWorkflow(DataRow startWorkflow,
            string workflowControlNo, decimal fromUserId, string fromUserInfo, string toUserInfo,
            string workflowReserveArea, string currentWorkflowReserveArea, DateTime? replyDeadline)
        {
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
            daoT_Workflow.UserInfo = fromUserInfo; // ユーザ入力が必要。
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
            daoT_CurrentWorkflow.ToUserId = startWorkflow["ToUserId"];
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
        /// <param name="subSystemId">サブシステムID(null、空文字指定可能)</param>
        /// <param name="workflowName">ワークフロー名(null、空文字指定可能)</param>
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
            string subSystemId, string workflowName, decimal? userId, int? userPositionTitlesId)
        {
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

            // ユーザID（必須）
            if (userId != null)
            {
                dao.SetParameter("ToUserId", userId);
            }

            // ユーザの職位ID
            if (userPositionTitlesId != null)
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
        /// <param name="acceptanceUserInfo">受付ユーザ情報</param>
        public void AcceptWfRequest(DataRow workflowRequest, decimal acceptanceUserId, string acceptanceUserInfo)
        {
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
        /// <param name="subSystemId">サブシステムID(null、空文字指定可能)</param>
        /// <param name="workflowName">ワークフロー名(null、空文字指定可能)</param>
        /// <param name="userId">ワークフローの受信ユーザ（御中指定不可能）</param>
        /// <returns>処理中のワークフロー一覧</returns>
        public DataTable GetProcessingWfRequest(string subSystemId, string workflowName, decimal userId)
        {
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

            // AcceptanceUserId
            dao.SetParameter("AcceptanceUserId", userId);

            // 処理中ワークフロー依頼を取得
            DataTable dt = new DataTable();
            dao.ExecSelectFill_DT(dt);

            // リターン
            return dt;
        }

        /// <summary>次のワークフロー依頼を取得します。</summary>
        /// <param name="ProcessingWfReq">選択した処理中ワークフロー依頼</param>
        /// <param name="fromUserId">FromユーザID（必須）</param>
        /// <returns>次のワークフロー</returns>
        /// <remarks>
        /// fromUsersId
        /// 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
        /// </remarks>
        public DataTable GetNextWfRequest(DataRow ProcessingWfReq, decimal fromUserId)
        {
            // --------------------------------------------------
            // 次のワークフロー依頼を取得
            // --------------------------------------------------
            // M_WorkflowのSELECT
            // --------------------------------------------------
            DataTable dt = new DataTable();
            DaoM_Workflow daoM_Workflow = new DaoM_Workflow(this.Dam);

            // 検索条件
            daoM_Workflow.SubSystemId = ProcessingWfReq["SubSystemId"];
            daoM_Workflow.WorkflowName = ProcessingWfReq["WorkflowName"];
            daoM_Workflow.WorkflowNo = ProcessingWfReq["NextWorkflowNo"];
            daoM_Workflow.FromUserId = fromUserId;

            // ワークフローの取得
            daoM_Workflow.D2_Select(dt);

            // リターン
            return dt;
        }

        /// <summary>差戻しのToユーザIDを履歴から取得します。</summary>
        /// <param name="turnBackWorkflow">差戻しのワークフロー</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <returns>ToユーザID</returns>
        public decimal GetTurnBackToUser(DataRow turnBackWorkflow, string workflowControlNo)
        {
            // --------------------------------------------------
            // 差戻しのToユーザIDを履歴から取得
            // --------------------------------------------------
            // T_WorkflowHistoryのSELECT
            // --------------------------------------------------
            CmnDao dao = new CmnDao(this.Dam);
            dao.SQLFileName = "GetTurnBackToUser.sql";
            dao.SetParameter("WorkflowControlNo", workflowControlNo);
            dao.SetParameter("ActionType", "TurnBack");
            dao.SetParameter("NextWorkflowNo", turnBackWorkflow["WorkflowNo"]);

            return (decimal)dao.ExecSelectScalar();
        }

        /// <summary>ワークフロー承認を依頼します。</summary>
        /// <param name="nextWorkflow">選択したワークフロー承認依頼</param>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <param name="fromUserID">FromユーザID（必須）</param>
        /// <param name="fromUserInfo">Fromユーザ情報（必須）</param>
        /// <param name="toUserID">ToユーザID（TurnBackの際に必要）</param>
        /// <param name="toUserInfo">Toユーザ情報（必須）</param>
        /// <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
        /// <param name="replyDeadline">回答希望日（任意）</param>
        /// <returns>メール・テンプレートID</returns>
        public int RequestWfApproval(
            DataRow nextWorkflow,　string workflowControlNo,
            decimal fromUserID, string fromUserInfo,
            decimal toUserID, string toUserInfo,
            string currentWorkflowReserveArea, DateTime? replyDeadline)
        {
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

            // 履歴番号 // 0件 → move → 1件 → 1件 + 1 = 2（履歴番号）
            daoT_CurrentWorkflow.Set_HistoryNo_forUPD = recordCount + 1;

            daoT_CurrentWorkflow.Set_WfPositionId_forUPD = nextWorkflow["WfPositionId"];
            daoT_CurrentWorkflow.Set_WorkflowNo_forUPD = nextWorkflow["WorkflowNo"];
            daoT_CurrentWorkflow.Set_FromUserId_forUPD = fromUserID; // 実際のユーザIDを入力する。
            daoT_CurrentWorkflow.Set_FromUserInfo_forUPD = fromUserInfo;
            daoT_CurrentWorkflow.Set_ActionType_forUPD = nextWorkflow["ActionType"];

            if ((string)nextWorkflow["ActionType"] == "TurnBack")
            {
                daoT_CurrentWorkflow.Set_ToUserId_forUPD = toUserID;
            }
            else
            {
                daoT_CurrentWorkflow.Set_ToUserId_forUPD = nextWorkflow["ToUserId"];
            }
            
            daoT_CurrentWorkflow.Set_ToUserInfo_forUPD = toUserInfo;
            daoT_CurrentWorkflow.Set_ToUserPositionTitlesId_forUPD = nextWorkflow["ToUserPositionTitlesId"];
            daoT_CurrentWorkflow.Set_NextWfPositionId_forUPD = nextWorkflow["NextWfPositionId"];
            daoT_CurrentWorkflow.Set_NextWorkflowNo_forUPD = nextWorkflow["NextWorkflowNo"];
            daoT_CurrentWorkflow.Set_ReserveArea_forUPD = currentWorkflowReserveArea;
            //daoT_CurrentWorkflow.Set_ExclusiveKey_forUPD = "";

            if ((string)nextWorkflow["ActionType"] == "TurnBack")
            {
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

            if (nextWorkflow["NextWorkflowNo"] == DBNull.Value)
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
    }
}
