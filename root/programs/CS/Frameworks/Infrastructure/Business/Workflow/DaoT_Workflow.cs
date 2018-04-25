//**********************************************************************************
//* クラス名        ：DaoT_Workflow
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
    public class DaoT_Workflow : MyBaseDao
    {
        #region インスタンス変数

        /// <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
        protected Hashtable HtUserParameter = new Hashtable();
        /// <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
        protected Hashtable HtParameter = new Hashtable();

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public DaoT_Workflow(BaseDam dam) : base(dam) { }

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



        /// <summary>SubSystemId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object SubSystemId
        {
            set
            {
                this.HtParameter["SubSystemId"] = value;
            }
            get
            {
                return this.HtParameter["SubSystemId"];
            }
        }

        /// <summary>WorkflowName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object WorkflowName
        {
            set
            {
                this.HtParameter["WorkflowName"] = value;
            }
            get
            {
                return this.HtParameter["WorkflowName"];
            }
        }

        /// <summary>UserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object UserId
        {
            set
            {
                this.HtParameter["UserId"] = value;
            }
            get
            {
                return this.HtParameter["UserId"];
            }
        }

        /// <summary>UserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object UserInfo
        {
            set
            {
                this.HtParameter["UserInfo"] = value;
            }
            get
            {
                return this.HtParameter["UserInfo"];
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

        /// <summary>EndDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        public object EndDate
        {
            set
            {
                this.HtParameter["EndDate"] = value;
            }
            get
            {
                return this.HtParameter["EndDate"];
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


        /// <summary>Set_SubSystemId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_SubSystemId_forUPD
        {
            set
            {
                this.HtParameter["Set_SubSystemId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_SubSystemId_forUPD"];
            }
        }


        /// <summary>Set_WorkflowName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_WorkflowName_forUPD
        {
            set
            {
                this.HtParameter["Set_WorkflowName_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_WorkflowName_forUPD"];
            }
        }


        /// <summary>Set_UserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_UserId_forUPD
        {
            set
            {
                this.HtParameter["Set_UserId_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_UserId_forUPD"];
            }
        }


        /// <summary>Set_UserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_UserInfo_forUPD
        {
            set
            {
                this.HtParameter["Set_UserInfo_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_UserInfo_forUPD"];
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


        /// <summary>Set_EndDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        public object Set_EndDate_forUPD
        {
            set
            {
                this.HtParameter["Set_EndDate_forUPD"] = value;
            }
            get
            {
                return this.HtParameter["Set_EndDate_forUPD"];
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


        /// <summary>SubSystemId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object SubSystemId_Like
        {
            set
            {
                this.HtParameter["SubSystemId_Like"] = value;
            }
            get
            {
                return this.HtParameter["SubSystemId_Like"];
            }
        }


        /// <summary>WorkflowName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object WorkflowName_Like
        {
            set
            {
                this.HtParameter["WorkflowName_Like"] = value;
            }
            get
            {
                return this.HtParameter["WorkflowName_Like"];
            }
        }


        /// <summary>UserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object UserId_Like
        {
            set
            {
                this.HtParameter["UserId_Like"] = value;
            }
            get
            {
                return this.HtParameter["UserId_Like"];
            }
        }


        /// <summary>UserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object UserInfo_Like
        {
            set
            {
                this.HtParameter["UserInfo_Like"] = value;
            }
            get
            {
                return this.HtParameter["UserInfo_Like"];
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


        /// <summary>EndDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        public object EndDate_Like
        {
            set
            {
                this.HtParameter["EndDate_Like"] = value;
            }
            get
            {
                return this.HtParameter["EndDate_Like"];
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
            this.SetSqlByFile2("DaoT_Workflow_S1_Insert.sql");

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
            this.SetSqlByFile2("DaoT_Workflow_D1_Insert.xml");

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
            this.SetSqlByFile2("DaoT_Workflow_S2_Select.xml");

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
            this.SetSqlByFile2("DaoT_Workflow_D2_Select.xml");

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
            this.SetSqlByFile2("DaoT_Workflow_S3_Update.xml");

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
            this.SetSqlByFile2("DaoT_Workflow_D3_Update.xml");

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
            this.SetSqlByFile2("DaoT_Workflow_S4_Delete.xml");

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
            this.SetSqlByFile2("DaoT_Workflow_D4_Delete.xml");

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
            this.SetSqlByFile2("DaoT_Workflow_D5_SelCnt.xml");

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