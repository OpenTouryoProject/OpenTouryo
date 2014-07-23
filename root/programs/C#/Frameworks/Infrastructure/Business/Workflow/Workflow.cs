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
        /// <param name="fromUserInfo">Fromユーザ情報（必須）</param>
        /// <param name="toUserInfo">Toユーザ情報（必須）</param>
        /// <param name="workflowReserveArea">T_Workflowの予備領域（任意）</param>
        /// <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
        /// <param name="replyDeadline">回答希望日（任意）</param>
        /// <returns>メール・テンプレートID</returns>
        public int StartWorkflow(DataRow startWorkflow, 
            string workflowControlNo, string fromUserInfo, string toUserInfo,
            string workflowReserveArea, string currentWorkflowReserveArea, DateTime? replyDeadline)
        {
            // --------------------------------------------------
            // T_WorkflowへのINSERT
            // --------------------------------------------------
            DaoT_Workflow daoT_Workflow = new DaoT_Workflow(this.Dam);

            daoT_Workflow.PK_WorkflowControlNo = workflowControlNo;
            daoT_Workflow.SubSystemId = startWorkflow["SubSystemId"];
            daoT_Workflow.WorkflowName = startWorkflow["WorkflowName"];
            daoT_Workflow.UserId = startWorkflow["FromUserId"];
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
            daoT_CurrentWorkflow.FromUserId = startWorkflow["FromUserId"];
            daoT_CurrentWorkflow.FromUserInfo = fromUserInfo;
            daoT_CurrentWorkflow.ActionType = startWorkflow["ActionType"];
            daoT_CurrentWorkflow.ToUserId = startWorkflow["ToUserId"];
            daoT_CurrentWorkflow.ToUserInfo = toUserInfo;
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

        /// <summary>ワークフロー依頼を取得します</summary>
        /// <param name="subSystemId">サブシステムID(null、空文字指定可能)</param>
        /// <param name="workflowName">ワークフロー名(null、空文字指定可能)</param>
        /// <param name="userId">ワークフローの受信ユーザ（必須）</param>
        /// <param name="userPositionTitlesId">ユーザの職位ID</param>
        /// <returns>ワークフロー依頼の一覧</returns>
        /// <remarks>
        /// fromUsersId
        /// 　御中IDでの呼び出しと、ユーザIDでの呼び出しは２回に分ける。
        /// </remarks>
        public DataTable GetWfRequest(string subSystemId, string workflowName, decimal? userId, int? userPositionTitlesId)
        {
            // --------------------------------------------------
            // ワークフローの依頼を取得
            // --------------------------------------------------
            // CurrentWorkflowのSELECT
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
        /// <param name="dr">選択したワークフロー依頼</param>
        /// <param name="acceptanceUserId">受付ユーザID</param>
        /// <param name="acceptanceUserInfo">受付ユーザ情報</param>
        public void AcceptWfRequest(DataRow dr, decimal acceptanceUserId, string acceptanceUserInfo)
        {
            // --------------------------------------------------
            // 受付（acceptance項目を更新）
            // --------------------------------------------------
            // T_CurrentWorkflowのUPDATE
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            // PK
            daoT_CurrentWorkflow.PK_WorkflowControlNo = dr["WorkflowControlNo"];

            // Acceptance
            daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = DateTime.Now;
            daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = acceptanceUserId;
            daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = acceptanceUserInfo;

            // 受付（更新）
            daoT_CurrentWorkflow.D3_Update();
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
            // CurrentWorkflowのSELECT
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
                dao.SetParameter("WkflowName", workflowName);
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
        /// <param name="dr">選択した処理中ワークフロー依頼</param>
        /// <param name="fromUserId">FromユーザID（必須）</param>
        /// <returns>次のワークフロー</returns>
        public DataTable GetNexeWfRequest(DataRow dr, decimal fromUserId)
        {
            DataTable dt = new DataTable();
            DaoM_Workflow daoM_Workflow = new DaoM_Workflow(this.Dam);

            // 検索条件
            daoM_Workflow.SubSystemId = dr["SubSystemId"];
            daoM_Workflow.WorkflowName = dr["WorkflowName"];
            daoM_Workflow.WorkflowNo = dr["NextWorkflowNo"];
            daoM_Workflow.FromUserId = fromUserId;

            // ワークフローの取得
            daoM_Workflow.D2_Select(dt);

            // リターン
            return dt;
        }

        /// <summary>ワークフロー承認を依頼する。</summary>
        /// <param name="nextWorkflow">選択したワークフロー承認依頼</param>
        /// <param name="fromUserInfo">Fromユーザ情報（必須）</param>
        /// <param name="toUserInfo">Toユーザ情報（必須）</param>
        /// <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
        /// <param name="replyDeadline">回答希望日（任意）</param>
        /// <returns>メール・テンプレートID</returns>
        public int RequestWfApproval(
            DataRow nextWorkflow,
            string fromUserInfo, string toUserInfo,
            string currentWorkflowReserveArea, DateTime? replyDeadline)
        {
            CmnDao dao = new CmnDao(this.Dam);
            
            // --------------------------------------------------
            // T_CurrentWorkflow→T_WorkflowHistory
            // --------------------------------------------------
            dao.SQLFileName = "RequestApproval_Move.sql";
            dao.ExecInsUpDel_NonQuery();

            // --------------------------------------------------
            // T_CurrentWorkflowへのINSERT
            // --------------------------------------------------
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            daoT_CurrentWorkflow.PK_WorkflowControlNo = nextWorkflow["WorkflowControlNo"];

            // 履歴番号
            dao.SQLFileName = "RequestApproval_Count.sql";
            daoT_CurrentWorkflow.HistoryNo = dao.ExecSelectScalar();

            daoT_CurrentWorkflow.WfPositionId = nextWorkflow["WfPositionId"];
            daoT_CurrentWorkflow.WorkflowNo = nextWorkflow["WorkflowNo"];
            daoT_CurrentWorkflow.FromUserId = nextWorkflow["FromUserId"];
            daoT_CurrentWorkflow.FromUserInfo = fromUserInfo;
            daoT_CurrentWorkflow.ActionType = nextWorkflow["ActionType"];
            daoT_CurrentWorkflow.ToUserId = nextWorkflow["ToUserId"];
            daoT_CurrentWorkflow.ToUserInfo = toUserInfo;
            daoT_CurrentWorkflow.ToUserPositionTitlesId = nextWorkflow["ToUserPositionTitlesId"];
            daoT_CurrentWorkflow.NextWfPositionId = nextWorkflow["NextWfPositionId"];
            daoT_CurrentWorkflow.NextWorkflowNo = nextWorkflow["NextWorkflowNo"];
            daoT_CurrentWorkflow.ReserveArea = currentWorkflowReserveArea;
            //daoT_CurrentWorkflow.ExclusiveKey = "";
            daoT_CurrentWorkflow.ReplyDeadline = replyDeadline;
            daoT_CurrentWorkflow.StartDate = DateTime.Now;
            //daoT_CurrentWorkflow.AcceptanceDate = DBNull.Value;
            //daoT_CurrentWorkflow.AcceptanceUserId = DBNull.Value;
            //daoT_CurrentWorkflow.AcceptanceUserInfo = DBNull.Value;

            daoT_CurrentWorkflow.D1_Insert();

            // ---
            
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
