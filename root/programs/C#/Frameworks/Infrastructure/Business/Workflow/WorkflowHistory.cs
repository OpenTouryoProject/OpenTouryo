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
//* クラス名        ：WorkflowHistory
//* クラス日本語名  ：ワークフロー履歴エンティティ
//*
//* 作成者          ：西野  大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/06/11  西野  大介        新規作成
//**********************************************************************************

using System.Globalization;
using System.ComponentModel;

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Util;

namespace Touryo.Infrastructure.Business.Workflow
{
    /// <summary>ワークフロー履歴エンティティ</summary>
    public class WorkflowHistory
    {
        /// <summary>履歴番号</summary>
        /// <remarks></remarks>
        public int HistoryNo { get; private set; }

        #region 検索条件

        /// <summary>サブシステムID</summary>
        public string SubSystemId { get; private set; }

        /// <summary>ワークフロー名</summary>
        public string WorkflowName { get; private set; }

        /// <summary>ワークフロー・パス識別番号</summary>
        public string WorkflowPathNo { get; private set; }

        /// <summary>処理名称</summary>
        public string ActionName { get; private set; }

        #endregion

        /// <summary>業務データ・サロゲートキー</summary>
        /// <remarks>管理する業務データを一意に特定するためのキー。</remarks>
        public string SurrogatekeyBusinessdata { get; private set; }

        /// <summary>次回ワークフロー・パス識別番号</summary>
        public string NextWorkflowPathNo { get; private set; }

        #region ステータス

        /// <summary>遷移先でのステータス</summary>
        /// <remarks>
        /// ワークフローの状態を表すコード値
        /// ・Request        ：依頼した状態
        /// ・Working        ：受付後作業中の状態
        /// ・Check          ：対象が審査済の状態
        /// ・Approve        ：対象が承認済の状態
        /// ・NeedToCalc     ：再積算が必要な状態
        /// ・CheckError     ：チェックがエラーである状態
        /// ・CheckOk        ：チェックがOKである状態
        /// ・Cancel         ：取り消された状態
        /// ・Completion     ：全完した状態
        /// ・TurnBack       ：差し戻した状態
        /// ステータスはステータスコードマスタテーブルにて管理する。
        /// </remarks>
        public string ToStatus { get; private set; }

        /// <summary>経過時間</summary>
        public string ElapsedTime { get; private set; }

        /// <summary>返信〆切日</summary>
        public string ReplyDeadline { get; private set; }

        #endregion

        #region 遷移元・遷移先

        /// <summary>遷移元会社ID</summary>
        public string FromCompanyId { get; private set; }
        /// <summary>遷移元組織ID</summary>
        public string FromSectionId { get; private set; }
        /// <summary>遷移元作業者ID</summary>
        public string FromUserId { get; private set; }

        /// <summary>遷移先会社ID</summary>
        public string ToCompanyId { get; private set; }
        /// <summary>遷移先組織ID</summary>
        public string ToSectionId { get; private set; }
        /// <summary>遷移先作業者ID</summary>
        public string ToUserId { get; private set; }

        #endregion

        #region 業務用予備項目

        /// <summary>業務用予備項目(文字列)１</summary>
        public string OptionalText1 { get; private set; }
        /// <summary>業務用予備項目(文字列)２</summary>
        public string OptionalText2 { get; private set; }
        /// <summary>業務用予備項目(文字列)３</summary>
        public string OptionalText3 { get; private set; }
        /// <summary>業務用予備項目(日付)１</summary>
        public string OptionalDate { get; private set; }

        #endregion

        /// <summary>コンストラクタ</summary>
        public WorkflowHistory() { }

        /// <summary>
        /// コンストラクタ
        /// ワークフロー履歴からプロパティ初期値
        /// </summary>
        /// <param name="historyNo"></param>
        /// <param name="subSystemId"></param>
        /// <param name="workflowName"></param>
        /// <param name="actionName"></param>
        /// <param name="workflowPathNo"></param>
        /// <param name="nextWorkflowPathNo"></param>
        /// <param name="surrogatekeyBusinessdata"></param>
        /// <param name="toStatus"></param>
        /// <param name="elapsedTime"></param>
        /// <param name="replyDeadline"></param>
        /// <param name="fromCompanyId"></param>
        /// <param name="fromSectionId"></param>
        /// <param name="fromUserId"></param>
        /// <param name="toCompanyId"></param>
        /// <param name="toSectionId"></param>
        /// <param name="toUserId"></param>
        /// <param name="optionalText1"></param>
        /// <param name="optionalText2"></param>
        /// <param name="optionalText3"></param>
        /// <param name="optionalDate"></param>
        public WorkflowHistory(
            // id, no
            int historyNo, 
            string subSystemId,
            string workflowName,
            string actionName,
            string workflowPathNo,
            string nextWorkflowPathNo,
            string surrogatekeyBusinessdata,
            // status
            string toStatus,
            string elapsedTime,
            string replyDeadline,
            // to-from
            string fromCompanyId,
            string fromSectionId,
            string fromUserId,
            string toCompanyId,
            string toSectionId,
            string toUserId,
            // optional
            string optionalText1,
            string optionalText2,
            string optionalText3,
            string optionalDate)
        {
            // id, no
            this.HistoryNo = historyNo;
            this.SubSystemId = subSystemId;
            this.WorkflowName = workflowName;
            this.ActionName = actionName;
            this.WorkflowPathNo = workflowPathNo;
            this.NextWorkflowPathNo = nextWorkflowPathNo;
            this.SurrogatekeyBusinessdata = surrogatekeyBusinessdata;
            // status
            this.ToStatus = toStatus;
            this.ElapsedTime = elapsedTime;
            this.ReplyDeadline = replyDeadline;
            // to-from
            this.FromCompanyId = fromCompanyId;
            this.FromSectionId = fromSectionId;
            this.FromUserId = fromUserId;
            this.ToCompanyId = toCompanyId;
            this.ToSectionId = toSectionId;
            this.ToUserId = toUserId;
            // optional
            this.OptionalText1 = optionalText1;
            this.OptionalText2 = optionalText2;
            this.OptionalText3 = optionalText3;
            this.OptionalDate = optionalDate;
        }
    }
}