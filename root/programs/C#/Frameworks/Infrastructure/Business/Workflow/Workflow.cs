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

        /// <summary>DataRow</summary>
        private DataRow Dr = null;

        /// <summary>新しいワークフローを準備します。</summary>
        /// <param name="subSystemId">サブシステムID（必須）</param>
        /// <param name="workflowName">ワークフロー名（必須）</param>
        /// <param name="fromUserId">FromユーザID（必須、御中ID指定可能）</param>
        /// <param name="toUserId">ToユーザID（出力）</param>
        /// <returns>
        /// 真：ワークフローが一意のためWFを開始できます。
        /// 偽：ワークフローが一意で無いためWFを開始できません。
        /// </returns>
        public bool PreStartWorkflow(
            string subSystemId, string workflowName,
            decimal[] fromUserId, out decimal toUserId)
        {
            DataTable dt = new DataTable();
            DaoM_Workflow daoM_Workflow = new DaoM_Workflow(this.Dam);

            // 検索条件
            daoM_Workflow.SubSystemId = subSystemId;
            daoM_Workflow.WorkflowName = workflowName;
            daoM_Workflow.WorkflowNo = 1; // スタートなので「1」固定
            //daoM_Workflow.FromUserId = fromUserId; // 御中ID指定可能、配列化
            
            // ワークフローの取得
            daoM_Workflow.D2_Select(dt);

            // 件数チェック
            if (dt.Rows.Count == 1)
            {
                // メンバに保持
                this.Dr = dt.Rows[0];
                // 戻り値を返す。
                toUserId = (decimal)dt.Rows[0]["ToUserId"];

                // リターン
                return true; // 正常終了
            }
            else
            {
                // メンバに保持
                this.Dr = null;
                // 戻り値を返す。
                toUserId = 0;

                // リターン
                return false; // 異常終了：ワークフローが一意で無いため開始できません。
            }
        }

        /// <summary>新しいワークフローを開始します。</summary>
        /// <param name="workflowControlNo">ワークフロー管理番号（必須）</param>
        /// <param name="fromUserInfo">Fromユーザ情報（必須）</param>
        /// <param name="toUserInfo">Toユーザ情報（必須）</param>
        /// <param name="workflowReserveArea">T_Workflowの予備領域（任意）</param>
        /// <param name="currentWorkflowReserveArea">T_CurrentWorkflowの予備領域（任意）</param>
        /// <param name="replyDeadline">回答希望日（任意）</param>
        public void StartWorkflow(
            string workflowControlNo, string fromUserInfo, string toUserInfo,
            string workflowReserveArea, string currentWorkflowReserveArea, DateTime? replyDeadline)
        {
            // --------------------------------------------------
            // T_WorkflowへのINSERT
            // --------------------------------------------------
            DaoT_Workflow daoT_Workflow = new DaoT_Workflow(this.Dam);

            daoT_Workflow.PK_WorkflowControlNo = workflowControlNo;
            daoT_Workflow.SubSystemId = this.Dr["SubSystemId"];
            daoT_Workflow.WorkflowName = this.Dr["WorkflowName"];
            daoT_Workflow.UserId = this.Dr["FromUserId"];
            daoT_Workflow.UserInfo = fromUserInfo;
            daoT_Workflow.ReserveArea = workflowReserveArea;
            daoT_Workflow.CreatedDate = DateTime.Now;
            //daoT_Workflow.EndDate = DBNull;

            daoT_Workflow.D1_Insert();

            // --------------------------------------------------
            // T_CurrentWorkflowへのINSERT
            // --------------------------------------------------
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            daoT_CurrentWorkflow.PK_WorkflowControlNo = workflowControlNo;
            daoT_CurrentWorkflow.HistoryNo = 1;
            daoT_CurrentWorkflow.WfPositionId = this.Dr["WfPositionid"];
            daoT_CurrentWorkflow.WorkflowNo = this.Dr["WorkflowNo"];
            daoT_CurrentWorkflow.FromUserId = this.Dr["FromUserId"];
            daoT_CurrentWorkflow.FromUserInfo = fromUserInfo;
            daoT_CurrentWorkflow.ActionType = this.Dr["ActionType"];
            daoT_CurrentWorkflow.ToUserId = this.Dr["ToUserId"];
            daoT_CurrentWorkflow.ToUserInfo = toUserInfo;
            daoT_CurrentWorkflow.NextWfPositionId = this.Dr["NextWfPositionId"];
            daoT_CurrentWorkflow.NextWorkflowNo = this.Dr["NextWorkflowNo"];
            daoT_CurrentWorkflow.ReserveArea = currentWorkflowReserveArea;
            //daoT_CurrentWorkflow.ExclusiveKey = "";
            daoT_CurrentWorkflow.ReplyDeadline = replyDeadline;

            daoT_CurrentWorkflow.D1_Insert();

            // 状態を破棄
            this.Dr = null;
        }

        /// <summary>ワークフローの依頼を取得します</summary>
        /// <param name="subSystemId">サブシステムID(null、空文字指定可能)</param>
        /// <param name="workflowName">ワークフロー名(null、空文字指定可能)</param>
        /// <param name="userId">ワークフローの受信ユーザ（御中ID指定可能）</param>
        /// <returns>カレントのワークフロー</returns>
        public DataTable GetWfRequest(string subSystemId, string workflowName,
            decimal[] userId)
        {
            // --------------------------------------------------
            // ワークフローの依頼を取得
            // --------------------------------------------------
            // CurrentWorkflowのSELECT
            CmnDao dao = new CmnDao(this.Dam);

            dao.SQLFileName = "GetWfRequest.xml";

            if (!string.IsNullOrEmpty(subSystemId))
            {
                dao.SetParameter("SubSystemId", subSystemId);
            }

            if (!string.IsNullOrEmpty(workflowName))
            {
                dao.SetParameter("WkflowName", workflowName);
            }

            // 御中IDに対応
            if (userId.Length != 0)
            {
                ArrayList al_usersId = new ArrayList();
                foreach (decimal dcm in userId)
                {
                    al_usersId.Add(dcm);
                }

                dao.SetParameter("ToUserId", al_usersId);
            }

            DataTable dt = new DataTable();
            dao.ExecSelectFill_DT(dt);
            return dt;
        }

        /// <summary>ワークフローの依頼を受付ます。</summary>
        /// <param name="dr">選択したワークフロー</param>
        /// <param name="acceptanceDate">受付日</param>
        /// <param name="acceptanceUserId">受付ユーザID</param>
        /// <param name="acceptanceUserInfo">受付ユーザ情報</param>
        public void AcceptWfRequest(DataRow dr,
            DateTime acceptanceDate, decimal acceptanceUserId, string acceptanceUserInfo)
        {
            // --------------------------------------------------
            // 受付（acceptance項目を更新）
            // --------------------------------------------------
            // T_CurrentWorkflowのUPDATE
            DaoT_CurrentWorkflow daoT_CurrentWorkflow = new DaoT_CurrentWorkflow(this.Dam);

            daoT_CurrentWorkflow.PK_WorkflowControlNo = dr["WorkflowControlNo"];
            daoT_CurrentWorkflow.Set_AcceptanceDate_forUPD = acceptanceDate;
            //daoT_CurrentWorkflow.Set_AcceptanceUserId_forUPD = acceptanceUserId;
            //daoT_CurrentWorkflow.Set_AcceptanceUserInfo_forUPD = acceptanceUserInfo;

            daoT_CurrentWorkflow.D1_Insert();
        }

        /// <summary>処理中のワークフローを取得します。</summary>
        /// <param name="subSystemId">サブシステムID(null、空文字指定可能)</param>
        /// <param name="workflowName">ワークフロー名(null、空文字指定可能)</param>
        /// <param name="userId">ワークフローの受信ユーザ（御中指定不可能）</param>
        /// <returns>処理中のワークフロー</returns>
        public DataTable GetProcessingWf(string subSystemId, string workflowName, decimal userId)
        {
            // --------------------------------------------------
            // 処理中のワークフローを取得
            // --------------------------------------------------
            // CurrentWorkflowのSELECT
            CmnDao dao = new CmnDao(this.Dam);

            dao.SQLFileName = "GetProcessingWf.xml";

            if (!string.IsNullOrEmpty(subSystemId))
            {
                dao.SetParameter("SubSystemId", subSystemId);
            }

            if (!string.IsNullOrEmpty(workflowName))
            {
                dao.SetParameter("WkflowName", workflowName);
            }

            // AcceptanceUserId
            dao.SetParameter("UserId", userId);

            DataTable dt = new DataTable();
            dao.ExecSelectFill_DT(dt);
            return dt;
        }
    }
}
