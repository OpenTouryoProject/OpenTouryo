//**********************************************************************************
//* クラス名        ：DaoT_CurrentWorkflow
//* クラス日本語名  ：自動生成Ｄａｏクラス
//*
//* 作成日時        ：2014/7/18
//* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*  2012/06/14  西野 大介         ResourceLoaderに加え、EmbeddedResourceLoaderに対応
//*  2013/09/09  西野 大介         ExecGenerateSQLメソッドを追加した（バッチ更新用）。
//**********************************************************************************

using System.Data;
using System.Collections;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Business.Workflow
{
    /// <summary>自動生成Ｄａｏクラス</summary>
    public class DaoT_CurrentWorkflow : MyBaseDao
    {
        #region インスタンス変数

        /// <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
        protected Hashtable HtUserParameter = new Hashtable();
        /// <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
        protected Hashtable HtParameter = new Hashtable();

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public DaoT_CurrentWorkflow(BaseDam dam) : base(dam) { }

        #endregion

        #region 共通関数（パラメタの制御）

        /// <summary>ユーザ パラメタ（文字列置換）をハッシュ テーブルに設定する。</summary>
        /// <param name="userParamName">ユーザ パラメタ名</param>
        /// <param name="userParamValue">ユーザ パラメタ値</param>
        public void SetUserParameteToHt(string userParamName, string userParamValue)
        {
            // ユーザ パラメタをハッシュ テーブルに設定
            this.HtUserParameter[userParamName] = userParamValue;
        }

        /// <summary>パラメタ ライズド クエリのパラメタをハッシュ テーブルに設定する。</summary>
        /// <param name="paramName">パラメタ名</param>
        /// <param name="paramValue">パラメタ値</param>
        public void SetParameteToHt(string paramName, object paramValue)
        {
            // ユーザ パラメタをハッシュ テーブルに設定
            this.HtParameter[paramName] = paramValue;
        }

        /// <summary>
        /// ・ユーザ パラメタ（文字列置換）
        /// ・パラメタ ライズド クエリのパラメタ
        /// を格納するハッシュ テーブルをクリアする。
        /// </summary>
        public void ClearParametersFromHt()
        {
            // ユーザ パラメタ（文字列置換）用ハッシュ テーブルを初期化
            this.HtUserParameter = new Hashtable();
            // パラメタ ライズド クエリのパラメタ用ハッシュ テーブルを初期化
            this.HtParameter = new Hashtable();
        }

        /// <summary>パラメタの設定（内部用）</summary>
        protected void SetParametersFromHt()
        {
            // ユーザ パラメタ（文字列置換）を設定する。
            foreach (string userParamName in this.HtUserParameter.Keys)
            {
                this.SetUserParameter(userParamName, this.HtUserParameter[userParamName].ToString());
            }

            // パラメタ ライズド クエリのパラメタを設定する。
            foreach (string paramName in this.HtParameter.Keys)
            {
                this.SetParameter(paramName, this.HtParameter[paramName]);
            }
        }

        #endregion

        #region プロパティ プロシージャ（setter、getter）


        /// <summary>WorkflowControlNo列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object PK_WorkflowControlNo
        {
            set
            {
                this.HtParameter["WorkflowControlNo"] = value;
            }
            get
            {
                return this.HtParameter["WorkflowControlNo"];
            }
        }



        /// <summary>HistoryNo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object HistoryNo
        {
            set
            {
                this.HtParameter["HistoryNo"] = value;
            }
            get
            {
                return this.HtParameter["HistoryNo"];
            }
        }

        /// <summary>WfPositionId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object WfPositionId
        {
            set
            {
                this.HtParameter["WfPositionId"] = value;
            }
            get
            {
                return this.HtParameter["WfPositionId"];
            }
        }

        /// <summary>WorkflowNo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object WorkflowNo
        {
            set
            {
                this.HtParameter["WorkflowNo"] = value;
            }
            get
            {
                return this.HtParameter["WorkflowNo"];
            }
        }

        /// <summary>FromUserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object FromUserId
        {
            set
            {
                this.HtParameter["FromUserId"] = value;
            }
            get
            {
                return this.HtParameter["FromUserId"];
            }
        }

        /// <summary>FromUserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object FromUserInfo
        {
            set
            {
                this.HtParameter["FromUserInfo"] = value;
            }
            get
            {
                return this.HtParameter["FromUserInfo"];
            }
        }

        /// <summary>ActionType列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object ActionType
        {
            set
            {
                this.HtParameter["ActionType"] = value;
            }
            get
            {
                return this.HtParameter["ActionType"];
            }
        }

        /// <summary>ToUserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object ToUserId
        {
            set
            {
                this.HtParameter["ToUserId"] = value;
            }
            get
            {
                return this.HtParameter["ToUserId"];
            }
        }

        /// <summary>ToUserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object ToUserInfo
        {
            set
            {
                this.HtParameter["ToUserInfo"] = value;
            }
            get
            {
                return this.HtParameter["ToUserInfo"];
            }
        }

        /// <summary>ToUserPositionTitlesId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object ToUserPositionTitlesId
        {
            set
            {
                this.HtParameter["ToUserPositionTitlesId"] = value;
            }
            get
            {
                return this.HtParameter["ToUserPositionTitlesId"];
            }
        }

        /// <summary>NextWfPositionId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object NextWfPositionId
        {
            set
            {
                this.HtParameter["NextWfPositionId"] = value;
            }
            get
            {
                return this.HtParameter["NextWfPositionId"];
            }
        }

        /// <summary>NextWorkflowNo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object NextWorkflowNo
        {
            set
            {
                this.HtParameter["NextWorkflowNo"] = value;
            }
            get
            {
                return this.HtParameter["NextWorkflowNo"];
            }
        }

        /// <summary>ReserveArea列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object ReserveArea
        {
            set
            {
                this.HtParameter["ReserveArea"] = value;
            }
            get
            {
                return this.HtParameter["ReserveArea"];
            }
        }

        /// <summary>ExclusiveKey列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object ExclusiveKey
        {
            set
            {
                this.HtParameter["ExclusiveKey"] = value;
            }
            get
            {
                return this.HtParameter["ExclusiveKey"];
            }
        }

        /// <summary>ReplyDeadline列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object ReplyDeadline
        {
            set
            {
                this.HtParameter["ReplyDeadline"] = value;
            }
            get
            {
                return this.HtParameter["ReplyDeadline"];
            }
        }

        /// <summary>StartDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object StartDate
        {
            set
            {
                this.HtParameter["StartDate"] = value;
            }
            get
            {
                return this.HtParameter["StartDate"];
            }
        }

        /// <summary>AcceptanceDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object AcceptanceDate
        {
            set
            {
                this.HtParameter["AcceptanceDate"] = value;
            }
            get
            {
                return this.HtParameter["AcceptanceDate"];
            }
        }

        /// <summary>AcceptanceUserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object AcceptanceUserId
        {
            set
            {
                this.HtParameter["AcceptanceUserId"] = value;
            }
            get
            {
                return this.HtParameter["AcceptanceUserId"];
            }
        }

        /// <summary>AcceptanceUserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object AcceptanceUserInfo
        {
            set
            {
                this.HtParameter["AcceptanceUserInfo"] = value;
            }
            get
            {
                return this.HtParameter["AcceptanceUserInfo"];
            }
        }


        /// <summary>Set_WorkflowControlNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_WorkflowControlNo_forUPD
        {
            set
            {
                this.HtParameter["Set_WorkflowControlNo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_WorkflowControlNo_forUPD"];
            }
        }


        /// <summary>Set_HistoryNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_HistoryNo_forUPD
        {
            set
            {
                this.HtParameter["Set_HistoryNo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_HistoryNo_forUPD"];
            }
        }


        /// <summary>Set_WfPositionId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_WfPositionId_forUPD
        {
            set
            {
                this.HtParameter["Set_WfPositionId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_WfPositionId_forUPD"];
            }
        }


        /// <summary>Set_WorkflowNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_WorkflowNo_forUPD
        {
            set
            {
                this.HtParameter["Set_WorkflowNo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_WorkflowNo_forUPD"];
            }
        }


        /// <summary>Set_FromUserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_FromUserId_forUPD
        {
            set
            {
                this.HtParameter["Set_FromUserId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_FromUserId_forUPD"];
            }
        }


        /// <summary>Set_FromUserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_FromUserInfo_forUPD
        {
            set
            {
                this.HtParameter["Set_FromUserInfo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_FromUserInfo_forUPD"];
            }
        }


        /// <summary>Set_ActionType_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_ActionType_forUPD
        {
            set
            {
                this.HtParameter["Set_ActionType_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_ActionType_forUPD"];
            }
        }


        /// <summary>Set_ToUserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_ToUserId_forUPD
        {
            set
            {
                this.HtParameter["Set_ToUserId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_ToUserId_forUPD"];
            }
        }


        /// <summary>Set_ToUserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_ToUserInfo_forUPD
        {
            set
            {
                this.HtParameter["Set_ToUserInfo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_ToUserInfo_forUPD"];
            }
        }


        /// <summary>Set_ToUserPositionTitlesId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_ToUserPositionTitlesId_forUPD
        {
            set
            {
                this.HtParameter["Set_ToUserPositionTitlesId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_ToUserPositionTitlesId_forUPD"];
            }
        }


        /// <summary>Set_NextWfPositionId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_NextWfPositionId_forUPD
        {
            set
            {
                this.HtParameter["Set_NextWfPositionId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_NextWfPositionId_forUPD"];
            }
        }


        /// <summary>Set_NextWorkflowNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_NextWorkflowNo_forUPD
        {
            set
            {
                this.HtParameter["Set_NextWorkflowNo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_NextWorkflowNo_forUPD"];
            }
        }


        /// <summary>Set_ReserveArea_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_ReserveArea_forUPD
        {
            set
            {
                this.HtParameter["Set_ReserveArea_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_ReserveArea_forUPD"];
            }
        }


        /// <summary>Set_ExclusiveKey_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_ExclusiveKey_forUPD
        {
            set
            {
                this.HtParameter["Set_ExclusiveKey_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_ExclusiveKey_forUPD"];
            }
        }


        /// <summary>Set_ReplyDeadline_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_ReplyDeadline_forUPD
        {
            set
            {
                this.HtParameter["Set_ReplyDeadline_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_ReplyDeadline_forUPD"];
            }
        }


        /// <summary>Set_StartDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_StartDate_forUPD
        {
            set
            {
                this.HtParameter["Set_StartDate_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_StartDate_forUPD"];
            }
        }


        /// <summary>Set_AcceptanceDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_AcceptanceDate_forUPD
        {
            set
            {
                this.HtParameter["Set_AcceptanceDate_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_AcceptanceDate_forUPD"];
            }
        }


        /// <summary>Set_AcceptanceUserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_AcceptanceUserId_forUPD
        {
            set
            {
                this.HtParameter["Set_AcceptanceUserId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_AcceptanceUserId_forUPD"];
            }
        }


        /// <summary>Set_AcceptanceUserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_AcceptanceUserInfo_forUPD
        {
            set
            {
                this.HtParameter["Set_AcceptanceUserInfo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_AcceptanceUserInfo_forUPD"];
            }
        }



        /// <summary>WorkflowControlNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object WorkflowControlNo_Like
        {
            set
            {
                this.HtParameter["WorkflowControlNo_Like"] = value;
            }
            get
            {
                return this.HtParameter["WorkflowControlNo_Like"];
            }
        }


        /// <summary>HistoryNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object HistoryNo_Like
        {
            set
            {
                this.HtParameter["HistoryNo_Like"] = value;
            }
            get
            {
                return this.HtParameter["HistoryNo_Like"];
            }
        }


        /// <summary>WfPositionId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object WfPositionId_Like
        {
            set
            {
                this.HtParameter["WfPositionId_Like"] = value;
            }
            get
            {
                return this.HtParameter["WfPositionId_Like"];
            }
        }


        /// <summary>WorkflowNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object WorkflowNo_Like
        {
            set
            {
                this.HtParameter["WorkflowNo_Like"] = value;
            }
            get
            {
                return this.HtParameter["WorkflowNo_Like"];
            }
        }


        /// <summary>FromUserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object FromUserId_Like
        {
            set
            {
                this.HtParameter["FromUserId_Like"] = value;
            }
            get
            {
                return this.HtParameter["FromUserId_Like"];
            }
        }


        /// <summary>FromUserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object FromUserInfo_Like
        {
            set
            {
                this.HtParameter["FromUserInfo_Like"] = value;
            }
            get
            {
                return this.HtParameter["FromUserInfo_Like"];
            }
        }


        /// <summary>ActionType_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object ActionType_Like
        {
            set
            {
                this.HtParameter["ActionType_Like"] = value;
            }
            get
            {
                return this.HtParameter["ActionType_Like"];
            }
        }


        /// <summary>ToUserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object ToUserId_Like
        {
            set
            {
                this.HtParameter["ToUserId_Like"] = value;
            }
            get
            {
                return this.HtParameter["ToUserId_Like"];
            }
        }


        /// <summary>ToUserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object ToUserInfo_Like
        {
            set
            {
                this.HtParameter["ToUserInfo_Like"] = value;
            }
            get
            {
                return this.HtParameter["ToUserInfo_Like"];
            }
        }


        /// <summary>ToUserPositionTitlesId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object ToUserPositionTitlesId_Like
        {
            set
            {
                this.HtParameter["ToUserPositionTitlesId_Like"] = value;
            }
            get
            {
                return this.HtParameter["ToUserPositionTitlesId_Like"];
            }
        }


        /// <summary>NextWfPositionId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object NextWfPositionId_Like
        {
            set
            {
                this.HtParameter["NextWfPositionId_Like"] = value;
            }
            get
            {
                return this.HtParameter["NextWfPositionId_Like"];
            }
        }


        /// <summary>NextWorkflowNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object NextWorkflowNo_Like
        {
            set
            {
                this.HtParameter["NextWorkflowNo_Like"] = value;
            }
            get
            {
                return this.HtParameter["NextWorkflowNo_Like"];
            }
        }


        /// <summary>ReserveArea_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object ReserveArea_Like
        {
            set
            {
                this.HtParameter["ReserveArea_Like"] = value;
            }
            get
            {
                return this.HtParameter["ReserveArea_Like"];
            }
        }


        /// <summary>ExclusiveKey_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object ExclusiveKey_Like
        {
            set
            {
                this.HtParameter["ExclusiveKey_Like"] = value;
            }
            get
            {
                return this.HtParameter["ExclusiveKey_Like"];
            }
        }


        /// <summary>ReplyDeadline_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object ReplyDeadline_Like
        {
            set
            {
                this.HtParameter["ReplyDeadline_Like"] = value;
            }
            get
            {
                return this.HtParameter["ReplyDeadline_Like"];
            }
        }


        /// <summary>StartDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object StartDate_Like
        {
            set
            {
                this.HtParameter["StartDate_Like"] = value;
            }
            get
            {
                return this.HtParameter["StartDate_Like"];
            }
        }


        /// <summary>AcceptanceDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object AcceptanceDate_Like
        {
            set
            {
                this.HtParameter["AcceptanceDate_Like"] = value;
            }
            get
            {
                return this.HtParameter["AcceptanceDate_Like"];
            }
        }


        /// <summary>AcceptanceUserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object AcceptanceUserId_Like
        {
            set
            {
                this.HtParameter["AcceptanceUserId_Like"] = value;
            }
            get
            {
                return this.HtParameter["AcceptanceUserId_Like"];
            }
        }


        /// <summary>AcceptanceUserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object AcceptanceUserInfo_Like
        {
            set
            {
                this.HtParameter["AcceptanceUserInfo_Like"] = value;
            }
            get
            {
                return this.HtParameter["AcceptanceUserInfo_Like"];
            }
        }


        #endregion

        #region クエリ メソッド

        #region Insert

        /// <summary>１レコード挿入する。</summary>
        /// <returns>挿入された行の数</returns>
        public int S1_Insert()
        {
            // ファイルからSQL（Insert）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_S1_Insert.sql");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（Insert）を実行し、戻り値を戻す。
            return this.ExecInsUpDel_NonQuery();
        }

        /// <summary>１レコード挿入する。</summary>
        /// <returns>挿入された行の数</returns>
        /// <remarks>パラメタで指定した列のみ挿入値が有効になる。</remarks>
        public int D1_Insert()
        {
            // ファイルからSQL（DynIns）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_D1_Insert.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（DynIns）を実行し、戻り値を戻す。
            return this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region Select

        /// <summary>主キーを指定し、１レコード参照する。</summary>
        /// <param name="dt">結果を格納するDataTable</param>
        public void S2_Select(DataTable dt)
        {
            // ファイルからSQL（Select）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_S2_Select.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（Select）を実行し、戻り値を戻す。
            this.ExecSelectFill_DT(dt);
        }

        /// <summary>検索条件を指定し、結果セットを参照する。</summary>
        /// <param name="dt">結果を格納するDataTable</param>
        public void D2_Select(DataTable dt)
        {
            // ファイルからSQL（DynSel）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_D2_Select.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（DynSel）を実行し、戻り値を戻す。
            this.ExecSelectFill_DT(dt);
        }

        #endregion

        #region Update

        /// <summary>主キーを指定し、１レコード更新する。</summary>
        /// <returns>更新された行の数</returns>
        /// <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
        public int S3_Update()
        {
            // ファイルからSQL（Update）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_S3_Update.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（Update）を実行し、戻り値を戻す。
            return this.ExecInsUpDel_NonQuery();
        }

        /// <summary>任意の検索条件でデータを更新する。</summary>
        /// <returns>更新された行の数</returns>
        /// <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
        public int D3_Update()
        {
            // ファイルからSQL（DynUpd）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_D3_Update.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（DynUpd）を実行し、戻り値を戻す。
            return this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region Delete

        /// <summary>主キーを指定し、１レコード削除する。</summary>
        /// <returns>削除された行の数</returns>
        public int S4_Delete()
        {
            // ファイルからSQL（Delete）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_S4_Delete.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（Delete）を実行し、戻り値を戻す。
            return this.ExecInsUpDel_NonQuery();
        }

        /// <summary>任意の検索条件でデータを削除する。</summary>
        /// <returns>削除された行の数</returns>
        public int D4_Delete()
        {
            // ファイルからSQL（DynDel）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_D4_Delete.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（DynDel）を実行し、戻り値を戻す。
            return this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region 拡張メソッド

        /// <summary>テーブルのレコード件数を取得する</summary>
        /// <returns>テーブルのレコード件数</returns>
        public object D5_SelCnt()
        {
            // ファイルからSQL（DynSelCnt）を設定する。
            this.SetSqlByFile2("DaoT_CurrentWorkflow_D5_SelCnt.xml");

            // パラメタの設定
            this.SetParametersFromHt();

            // SQL（SELECT COUNT）を実行し、戻り値を戻す。
            return this.ExecSelectScalar();
        }

        /// <summary>静的SQLを生成する。</summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="sqlUtil">SQLユーティリティ</param>
        /// <returns>生成した静的SQL</returns>
        public string ExecGenerateSQL(string fileName, SQLUtility sqlUtil)
        {
            // ファイルからSQLを設定する。
            this.SetSqlByFile2(fileName);

            // パラメタの設定
            this.SetParametersFromHt();

            return base.ExecGenerateSQL(sqlUtil);
        }

        #endregion

        #endregion
    }
}