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
//* クラス名        ：_3TierEngine
//* クラス日本語名  ：三層データバインド用の業務コードクラス・エンジン
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/01/10  西野 大介         新規作成
//*  2014/07/14  西野 大介         関連チェック処理を実装可能に
//*  2014/07/17  Sai-San           Added Select count query and select paging query constants and checks for PostgreSQL db support 
//*                                Added UOC_RelatedCheck override method and method calls in methods
//*                                'UOC_InsertRecord', 'UOC_UpdateRecord', 'UOC_DeleteRecord' and 'UOC_BatchUpdate' 
//*  2014/07/21  Rituparna         Added SelectCount and SelectPaging query constatnts and check for MySql db support
//*
//*  2014/08/14  Santosh Avaji     Added and modidfied code for DB2 support
//*  2014/12/10  西野 大介         Modified because there was a problem with the SELECT_PAGING_SQL_TEMPLATE_ORACLE.
//*  2014/12/10  西野 大介         Implementations of the related check process has been changed for problem.
//*                                Change the signature of the CRUD methods. "private" ---> "protected virtual"
//*  2015/04/29  Sandeep           Modified the code of 'UOC_SelectMethod' to retrive 30 records instead of 31 records
//*  2016/04/21  Shashikiran       Implemented 'UOC_UpdateRecordDM' method to perform multiple table update in single transaction
//*  2016/05/10  Shashikiran       Implemented 'UOC_BatchUpdateDM' method to perform batch update in single transaction 
//*  2016/05/25  Shashikiran       Implemented 'UOC_DeleteRecordDM' method to perform delete operation in single transaction
//*  2016/05/26  Shashikiran       Modified the code of 'UOC_BatchUpdateDM' method to perform batch delete operation in single transaction 
//**********************************************************************************

using System;
using System.Data;
using System.Reflection;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Business
{
    /// <summary>三層データバインド用の業務コードクラス・エンジン</summary>
    public class _3TierEngine : MyFcBaseLogic
    {
        #region 定数・変数

        #region テンプレート類

        /// <summary>データ件数取得SQLテンプレート</summary>
        private const string SELECT_COUNT_SQL_TEMPLATE =
            "SELECT COUNT(*) FROM {0} {1}";

        /// <summary>Select count query for PostgreSQL</summary>
        private const string SELECT_COUNT_POSTGRESQL_TEMPLATE =
            "SELECT COUNT(*) FROM \"{0}\" {1}";

        /// <summary>データ取得SQLテンプレート（DBMSによって可変となる）</summary>
        /// <remarks>SQL Server用</remarks>
        private const string SELECT_PAGING_SQL_TEMPLATE_SQL_SERVER =
            "WITH [OrderedTable] AS (SELECT {0}, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) [rnum] FROM {3} {4}) SELECT {0} FROM [OrderedTable] WHERE [rnum] BETWEEN {5} AND {6}";

        /// <summary>Select query for PostGreSQL</summary>
        private const string SELECT_PAGING_SQL_TEMPLATE_POSTGRESQL =
            "SELECT {0} FROM ( SELECT {0}, ROW_NUMBER() OVER (ORDER BY \"{1}\" {2}) \"RNUM\" FROM \"{3}\" {4} ) AS Temp WHERE \"RNUM\" BETWEEN {5} AND {6}";

        /// <summary>データ取得SQLテンプレート（DBMSによって可変となる）</summary>
        /// <remarks>Oracle用</remarks>
        private const string SELECT_PAGING_SQL_TEMPLATE_ORACLE =
            "SELECT {0} FROM ( SELECT {0}, ROW_NUMBER() OVER (ORDER BY \"{1}\" {2}) \"RNUM\" FROM {3} {4} ) WHERE \"RNUM\" BETWEEN {5} AND {6}";
        //"SELECT {0} FROM ( SELECT {0}, ROW_NUMBER() OVER (ORDER BY \"{1}\" {2}) \"RNUM\" FROM \"{3}\" {4} ) WHERE \"RNUM\" BETWEEN {5} AND {6}";

        /// <summary>Selectpaging query from Mysql Database</summary>
        private const string SELECT_PAGING_MYSQL_TEMPLATE =
            "SELECT * FROM(SELECT * FROM ( SELECT *,  @i := @i + 1 AS RESULT FROM {3},(SELECT @i := 0) TEMP ORDER BY \"{1}\"  {2}) TEMP1 {4})TEMP3 WHERE RESULT BETWEEN {5} AND {6}";

        /// <summary>Select Paging Query For DB2 database</summary>
        private const string SELECT_PAGING_DB2_TEMPLATE =
            "SELECT * FROM (SELECT {0}, ROW_NUMBER() OVER (ORDER BY {1} {2}) AS ROWNUM FROM {3} {4}) WHERE ROWNUM BETWEEN {5} AND {6}";

        /// <summary>Where句生成SQLテンプレート（＝）</summary>
        private const string WHERE_SQL_TEMPLATE_EQUAL = "_s__ColName__e_ = _p__ParamName_";

        /// <summary>Where句生成SQLテンプレート（Like）</summary>
        private const string WHERE_SQL_TEMPLATE_LIKE = "_s__ColName__f__e_ Like _p__ParamName_";

        #endregion

        #region 墨壷2のパラメタ

        // Daoクラス名のヘッダ・フッタ
        private string DaoClassNameHeader;
        private string DaoClassNameFooter;

        // メソッド名のヘッダ・フッタ（静的）
        private string MethodNameHeaderS;
        private string MethodNameFooterS;

        // メソッド名のCRUD名
        private string MethodLabel_Ins;
        private string MethodLabel_Sel;
        private string MethodLabel_Upd;
        private string MethodLabel_Del;
        private string MethodLabel_SelCnt;

        // メソッド名のヘッダ・フッタ（動的）
        private string MethodNameHeaderD;
        private string MethodNameFooterD;

        // Updateのパラメタ名のヘッダ・フッタ
        private string UpdateParamHeader;
        private string UpdateParamFooter;

        // Likeのパラメタ名のヘッダ・フッタ
        private string LikeParamHeader;
        private string LikeParamFooter;

        #endregion

        #endregion

        #region 初期化

        /// <summary>コンストラクタ</summary>
        public _3TierEngine()
        {
            // Daoクラス名のヘッダ・フッタ
            this.DaoClassNameHeader = GetConfigParameter.GetConfigValue("DaoClassNameHeader");
            this.DaoClassNameFooter = GetConfigParameter.GetConfigValue("DaoClassNameFooter");

            // メソッド名のヘッダ・フッタ（静的）
            this.MethodNameHeaderS = GetConfigParameter.GetConfigValue("MethodNameHeaderS");
            this.MethodNameFooterS = GetConfigParameter.GetConfigValue("MethodNameFooterS");

            // メソッド名のCRUD名
            this.MethodLabel_Ins = GetConfigParameter.GetConfigValue("MethodLabel_Ins");
            this.MethodLabel_Sel = GetConfigParameter.GetConfigValue("MethodLabel_Sel");
            this.MethodLabel_Upd = GetConfigParameter.GetConfigValue("MethodLabel_Upd");
            this.MethodLabel_Del = GetConfigParameter.GetConfigValue("MethodLabel_Del");
            this.MethodLabel_SelCnt = GetConfigParameter.GetConfigValue("MethodLabel_SelCnt");

            // メソッド名のヘッダ・フッタ（動的）
            this.MethodNameHeaderD = GetConfigParameter.GetConfigValue("MethodNameHeaderD");
            this.MethodNameFooterD = GetConfigParameter.GetConfigValue("MethodNameFooterD");

            // Updateのパラメタ名のヘッダ・フッタ
            this.UpdateParamHeader = GetConfigParameter.GetConfigValue("UpdateParamHeader");
            this.UpdateParamFooter = GetConfigParameter.GetConfigValue("UpdateParamFooter");

            // Likeのパラメタ名のヘッダ・フッタ
            this.LikeParamHeader = GetConfigParameter.GetConfigValue("LikeParamHeader");
            this.LikeParamFooter = GetConfigParameter.GetConfigValue("LikeParamFooter");
        }

        /// <summary>ListItem用のDatatableを生成する</summary>
        /// <param name="pdt">引数Datatable</param>
        /// <param name="pvalueIndex">値のIndex</param>
        /// <param name="ptextIndex">表示名のIndex</param>
        /// <param name="rdt">DropDownListItemのDatatable</param>
        /// <param name="rvalueIndex">値のIndex</param>
        /// <param name="rtextIndex">表示名のIndex</param>
        public static void CreateDropDownListDataSourceDataTable(
            DataTable pdt, string pvalueIndex, string ptextIndex,
            out DataTable rdt, string rvalueIndex, string rtextIndex)
        {
            // 列生成
            rdt = new DataTable();
            // 自動生成テンプレートのベースがテキスト・ボックスなのでStringで良い。
            rdt.Columns.Add(new DataColumn(rvalueIndex, typeof(String)));
            rdt.Columns.Add(new DataColumn(rtextIndex, typeof(String)));

            // 行生成
            foreach (DataRow pdr in pdt.Rows)
            {
                DataRow rdr = rdt.NewRow();
                rdr[rvalueIndex] = pdr[pvalueIndex].ToString();
                rdr[rtextIndex] = pdr[pvalueIndex].ToString() + " : " + pdr[ptextIndex].ToString();
                rdt.Rows.Add(rdr);
            }

            // 変更のコミット
            rdt.AcceptChanges();
        }

        #endregion

        #region テンプレ

        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_メソッド名(BaseParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue testReturn = new _3TierReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());
            cmnDao.ExecSelectScalar();

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region 一覧系

        /// <summary>データ件数取得処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_SelectCountMethod(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());

            // 検索条件の生成＆指定
            string whereSQL = this.SetSearchConditions(parameterValue, cmnDao);

            string p = ""; // パラメタ記号
            string s = ""; // 囲い記号開始
            string e = ""; // 囲い記号終了
            string f = ""; // For supporting type casting in PostgreSQL
            // 囲い文字の選択
            if (parameterValue.DBMSType == DbEnum.DBMSType.SQLServer)
            {
                p = "@";
                s = "[";
                e = "]";
            }
            //MYSQL and DB2
            else if (parameterValue.DBMSType == DbEnum.DBMSType.MySQL || parameterValue.DBMSType == DbEnum.DBMSType.DB2)
            {
                p = "@";
                s = "\"";
                e = "\"";
            }
            else if (parameterValue.DBMSType == DbEnum.DBMSType.Oracle)
            {
                p = ":";
                s = "\"";
                e = "\"";
            }
            else if (parameterValue.DBMSType == DbEnum.DBMSType.PstGrS)
            {
                p = "@";
                f = "::text";
            }
            else
            {
                p = "@";
                s = "[";
                e = "]";
            }

            if (parameterValue.DBMSType == DbEnum.DBMSType.PstGrS)
            {
                //Set the Query for PostgreSQL database
                cmnDao.SQLText = string.Format(
                        SELECT_COUNT_POSTGRESQL_TEMPLATE,
                        s + parameterValue.TableName + e, whereSQL)
                        .Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f);
            }
            //MYSQL
            else if (parameterValue.DBMSType == DbEnum.DBMSType.MySQL)
            {

                string SQLtext = string.Format(
                SELECT_COUNT_SQL_TEMPLATE,
                     parameterValue.TableName, whereSQL)
                    .Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f).Replace("\"", string.Empty);
                cmnDao.SQLText = SQLtext;
            }

            else
            {
                // SQLを設定して And DB2
                cmnDao.SQLText = string.Format(
                    SELECT_COUNT_SQL_TEMPLATE,
                s + parameterValue.TableName + e, whereSQL)
                    .Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f);
            }

            // パラメタは指定済み

            // データ件数を取得
            returnValue.Obj = cmnDao.ExecSelectScalar();

            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>データ取得処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_SelectMethod(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());

            // 検索条件の生成＆指定
            string whereSQL = this.SetSearchConditions(parameterValue, cmnDao);

            string selectPagingSqlTemplate = "";

            string p = ""; // パラメタ記号
            string s = ""; // 囲い記号開始
            string e = ""; // 囲い記号終了
            string f = ""; // For supporting type casting in PostgreSQL

            // テンプレート、囲い文字の選択
            if (parameterValue.DBMSType == DbEnum.DBMSType.SQLServer)
            {
                selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_SQL_SERVER;

                p = "@";
                s = "[";
                e = "]";
            }
            else if (parameterValue.DBMSType == DbEnum.DBMSType.MySQL)
            {
                selectPagingSqlTemplate = SELECT_PAGING_MYSQL_TEMPLATE;
                p = "@";
                s = "\"";
                e = "\"";
            }
            else if (parameterValue.DBMSType == DbEnum.DBMSType.DB2)
            {
                selectPagingSqlTemplate = SELECT_PAGING_DB2_TEMPLATE;
                p = "@";
                s = "\"";
                e = "\"";
            }
            else if (parameterValue.DBMSType == DbEnum.DBMSType.Oracle)
            {
                selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_ORACLE;

                p = ":";
                s = "\"";
                e = "\"";
            }
            else if (parameterValue.DBMSType == DbEnum.DBMSType.PstGrS)
            {
                selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_POSTGRESQL;

                p = "@";
                f = "::text";
            }
            else
            {
                selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_SQL_SERVER;

                p = "@";
                s = "[";
                e = "]";
            }

            int startRowNum = parameterValue.StartRowIndex + 1;

            string selectPagingSQL = "";
            if (parameterValue.DBMSType == DbEnum.DBMSType.MySQL)
            {

                selectPagingSQL = string.Format(
                selectPagingSqlTemplate,
                new string[] {
                    parameterValue.ColumnList,
                    parameterValue.SortExpression,
                    parameterValue.SortDirection,
                    parameterValue.TableName ,whereSQL,
                    startRowNum.ToString(), (startRowNum + parameterValue.MaximumRows - 1).ToString()}
                ).Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f).Replace("\"", string.Empty);

            }
            else
            {
                // SQL本体の生成（いろいろ組み込み DB2
                //（DBMSによって可変となる可能性有り）
                selectPagingSQL = string.Format(
                selectPagingSqlTemplate,
                new string[] {
                    parameterValue.ColumnList,
                    parameterValue.SortExpression,
                    parameterValue.SortDirection,
                    s + parameterValue.TableName + e , whereSQL,
                    startRowNum.ToString(), (startRowNum + parameterValue.MaximumRows - 1).ToString()}
                   ).Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f);

            }
            // DataTableをインスタンス化
            if (parameterValue.DataTableType == null)
            {
                // == null
                returnValue.Dt = new DataTable();
            }
            else
            {
                // != null

                // 型付きDataTable
                //（パブリック・デフォルト・コンストラクタ）
                returnValue.Dt =
                    (DataTable)parameterValue.DataTableType.InvokeMember(
                        null, BindingFlags.CreateInstance, null, null, null);
            }

            // SQLを設定して
            cmnDao.SQLText = selectPagingSQL;

            // パラメタは指定済み

            // DataTableを取得
            cmnDao.ExecSelectFill_DT(returnValue.Dt);

            // ↑業務処理-----------------------------------------------------
        }

        #region 共通処理

        /// <summary>検索条件の設定</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="cmnDao">共通Dao</param>
        /// <returns>Where句</returns>
        private string SetSearchConditions(
            _3TierParameterValue parameterValue, CmnDao cmnDao)
        {
            //  検索条件
            string whereSQL = "";

            #region AND

            // AndEqualSearchConditions
            // nullチェック
            if (parameterValue.AndEqualSearchConditions == null)
            {
                // == null
            }
            else
            {
                // != null
                foreach (string k in parameterValue.AndEqualSearchConditions.Keys)
                {
                    // フラグ
                    bool isSetted = false;

                    // nullチェック（null相当を要検討
                    if (parameterValue.AndEqualSearchConditions[k] == null)
                    {
                        // == null
                    }
                    else
                    {
                        // != null

                        // 文字列チェック
                        if (parameterValue.AndEqualSearchConditions[k] is string)
                        {
                            // 文字列の場合
                            if ((string)parameterValue.AndEqualSearchConditions[k] == "")
                            {
                                // 空文字列（★ 扱いを検討 → 検索条件の空文字列は検索しない扱い
                            }
                            else
                            {
                                // 空文字列でない。
                                isSetted = true;
                            }
                        }
                        else
                        {
                            // オブジェクトの場合
                            isSetted = true;
                        }
                    }

                    // パラメタを設定
                    if (isSetted)
                    {
                        whereSQL = GenWhereAndSetParameter(
                            WHERE_SQL_TEMPLATE_EQUAL, whereSQL,
                            cmnDao, k, parameterValue.AndEqualSearchConditions[k], false);

                        isSetted = false;
                    }
                }
            }

            // AndLikeSearchConditions
            // nullチェック
            if (parameterValue.AndLikeSearchConditions == null)
            {
                // == null
            }
            else
            {
                // != null
                foreach (string k in parameterValue.AndLikeSearchConditions.Keys)
                {
                    // nullチェック（null相当を要検討
                    if (string.IsNullOrEmpty(parameterValue.AndLikeSearchConditions[k]))
                    {
                        // 空文字列
                    }
                    else
                    {
                        // 空文字列でない。

                        // パラメタを設定
                        whereSQL = GenWhereAndSetParameter(
                            WHERE_SQL_TEMPLATE_LIKE, whereSQL,
                            cmnDao, k, parameterValue.AndLikeSearchConditions[k], true);
                    }
                }
            }

            #endregion

            #region OR

            // OrEqualSearchConditions
            // nullチェック
            if (parameterValue.OrEqualSearchConditions == null)
            {
                // == null
            }
            else
            {
                // != null
                foreach (string k in parameterValue.OrEqualSearchConditions.Keys)
                {
                    // フラグ
                    bool isSetted = false;

                    // nullチェック（null相当を要検討
                    if (parameterValue.OrEqualSearchConditions[k] == null)
                    {
                        // == null
                    }
                    else
                    {
                        // != null

                        // OR条件はループする。
                        int i = 0;
                        foreach (object o in parameterValue.OrEqualSearchConditions[k])
                        {
                            // 文字列チェック
                            if (o is string)
                            {
                                // 文字列の場合
                                if ((string)o == "")
                                {
                                    // 空文字列（★ 扱いを検討 → 検索条件の空文字列は検索しない扱い
                                }
                                else
                                {
                                    // 空文字列でない。
                                    isSetted = true;
                                }
                            }
                            else
                            {
                                // オブジェクトの場合
                                isSetted = true;
                            }

                            // パラメタを設定
                            if (isSetted)
                            {
                                whereSQL = GenWhereOrSetParameter(
                                    WHERE_SQL_TEMPLATE_EQUAL, whereSQL, cmnDao, k, o, i, false);

                                isSetted = false;
                                i++;
                            }
                        }
                    }
                }
            }

            // OrLikeSearchConditions
            // nullチェック
            if (parameterValue.OrLikeSearchConditions == null)
            {
                // == null
            }
            else
            {
                // != null
                foreach (string k in parameterValue.OrLikeSearchConditions.Keys)
                {
                    // nullチェック（null相当を要検討
                    if (parameterValue.OrLikeSearchConditions[k] == null)
                    {
                        // == null
                    }
                    else
                    {
                        // != null

                        // OR条件はループする。
                        int i = 0;
                        foreach (string s in parameterValue.OrLikeSearchConditions[k])
                        {
                            // 文字列の場合
                            if ((string)s == "")
                            {
                                // 空文字列
                            }
                            else
                            {
                                // 空文字列でない。
                                // パラメタを設定
                                whereSQL = GenWhereOrSetParameter(
                                    WHERE_SQL_TEMPLATE_LIKE, whereSQL, cmnDao, k, s, i, true);
                                i++;
                            }
                        }
                    }
                }
            }

            #endregion

            #region その他

            // 追加の検索条件（要：半角スペース）
            whereSQL += " " + parameterValue.ElseWhereSQL;

            // ElseSearchConditions
            // nullチェック
            if (parameterValue.ElseSearchConditions == null)
            {
                // == null
            }
            else
            {
                // != null
                foreach (string k in parameterValue.ElseSearchConditions.Keys)
                {
                    // nullチェック（null相当を要検討
                    if (parameterValue.ElseSearchConditions[k] == null)
                    {
                        // == null
                    }
                    else
                    {
                        // != null
                        cmnDao.SetParameter(k, parameterValue.ElseSearchConditions[k]);
                    }
                }
            }

            #endregion

            // Where句の付与（要：Trim）
            if (!string.IsNullOrEmpty(whereSQL.Trim()))
            {
                // （要：半角スペース）
                whereSQL = "WHERE " + whereSQL;
            }

            // 先頭の論理演算子を削除
            return BaseDam.DeleteFirstLogicalOperatoronWhereClause(whereSQL);
        }

        /// <summary>Where句生成＆パラメタ指定（and）</summary>
        /// <param name="whereSqlTemplate">Where句SQLテンプレート</param>
        /// <param name="whereSQL">生成中のWhere句SQL</param>
        /// <param name="cmnDao">共通Dao</param>
        /// <param name="parameterName">パラメタ名</param>
        /// <param name="parameterValue">パラメタ値</param>
        /// <param name="isLike">Likeか？</param>
        /// <returns>生成したWhere句SQL</returns>
        private string GenWhereAndSetParameter(
            string whereSqlTemplate, string whereSQL,
            CmnDao cmnDao, string parameterName, object parameterValue, bool isLike)
        {
            // Where句生成
            if (string.IsNullOrEmpty(whereSQL))
            {
                // 先頭は何もしない。
            }
            else
            {
                // 以降はAND
                whereSQL += " AND ";
            }

            string temp = "";
            temp = whereSqlTemplate.Replace("_ColName_", parameterName);

            // パラメタ指定
            if (isLike)
            {
                // Like
                whereSQL += temp.Replace("_ParamName_", this.LikeParamHeader + parameterName + this.LikeParamFooter);
                cmnDao.SetParameter(this.LikeParamHeader + parameterName + this.LikeParamFooter, parameterValue);
            }
            else
            {
                // Equal
                whereSQL += temp.Replace("_ParamName_", parameterName);
                cmnDao.SetParameter(parameterName, parameterValue);
            }

            return whereSQL;
        }

        /// <summary>Where句生成＆パラメタ指定（or）</summary>
        /// <param name="whereSqlTemplate">Where句SQLテンプレート</param>
        /// <param name="whereSQL">生成中のWhere句SQL</param>
        /// <param name="cmnDao">共通Dao</param>
        /// <param name="parameterName">パラメタ名</param>
        /// <param name="parameterValue">パラメタ値</param>
        /// <param name="parameterNumber">パラメタ番号</param>
        /// <param name="isLike">Likeか？</param>
        /// <returns>生成したWhere句SQL</returns>
        private string GenWhereOrSetParameter(
            string whereSqlTemplate, string whereSQL,
            CmnDao cmnDao, string parameterName, object parameterValue, int parameterNumber, bool isLike)
        {
            // Where句生成
            if (string.IsNullOrEmpty(whereSQL))
            {
                // 先頭は何もしない。
            }
            else
            {
                // 以降はOR
                whereSQL += " OR ";
            }

            string temp = "";
            temp = whereSqlTemplate.Replace("_ColName_", parameterName);

            // パラメタ指定
            if (isLike)
            {
                // Like
                whereSQL += temp.Replace("_ParamName_", this.LikeParamHeader + parameterName + parameterNumber.ToString() + this.LikeParamFooter);
                cmnDao.SetParameter(this.LikeParamHeader + parameterName + parameterNumber.ToString() + this.LikeParamFooter, parameterValue);
            }
            else
            {
                // Equal
                whereSQL += temp.Replace("_ParamName_", parameterName + parameterNumber.ToString());
                cmnDao.SetParameter(parameterName + parameterNumber.ToString(), parameterValue);
            }

            return whereSQL;
        }

        #endregion

        #endregion

        #region CRUD系

        /// <summary>１件追加処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        protected virtual void UOC_InsertRecord(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());

            // 検索条件の指定

            // 追加値
            // InsertUpdateValues
            foreach (string k in parameterValue.InsertUpdateValues.Keys)
            {
                // nullチェック（null相当を要検討
                if (parameterValue.InsertUpdateValues[k] == null)
                {
                    // == null
                }
                else
                {
                    // != null

                    // 文字列の場合の扱い
                    if (parameterValue.InsertUpdateValues[k] is string)
                    {
                        if (!string.IsNullOrEmpty((string)parameterValue.InsertUpdateValues[k]))
                        {
                            // パラメタ指定
                            cmnDao.SetParameter(k, parameterValue.InsertUpdateValues[k]);
                        }
                    }
                    else
                    {
                        // パラメタ指定
                        cmnDao.SetParameter(k, parameterValue.InsertUpdateValues[k]);
                    }
                }
            }

            // SQLを設定して
            cmnDao.SQLFileName =
                this.DaoClassNameHeader + parameterValue.TableName + this.DaoClassNameFooter
                + "_" + this.MethodNameHeaderD + this.MethodLabel_Ins + this.MethodNameFooterD + ".xml";

            // パラメタは指定済み

            // 追加処理を実行
            returnValue.Obj = cmnDao.ExecInsUpDel_NonQuery();

            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>１件取得処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        protected virtual void UOC_SelectRecord(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());

            // 検索条件の指定

            // AndEqualSearchConditions（主キー
            foreach (string k in parameterValue.AndEqualSearchConditions.Keys)
            {
                // nullチェック（null相当を要検討
                if (parameterValue.AndEqualSearchConditions[k] == null)
                {
                    // == null
                }
                else
                {
                    // != null

                    // 文字列の場合の扱い
                    if (parameterValue.AndEqualSearchConditions[k] is string)
                    {
                        if (!string.IsNullOrEmpty((string)parameterValue.AndEqualSearchConditions[k]))
                        {
                            // パラメタ指定
                            cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                        }
                    }
                    else
                    {
                        // パラメタ指定
                        cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                    }
                }
            }

            // DataTableをインスタンス化
            if (parameterValue.DataTableType == null)
            {
                // == null
                returnValue.Dt = new DataTable();
            }
            else
            {
                // != null

                // 型付きDataTable
                //（パブリック・デフォルト・コンストラクタ）
                returnValue.Dt =
                    (DataTable)parameterValue.DataTableType.InvokeMember(
                        null, BindingFlags.CreateInstance, null, null, null);
            }

            // SQLを設定して
            cmnDao.SQLFileName =
                this.DaoClassNameHeader + parameterValue.TableName + this.DaoClassNameFooter
                + "_" + this.MethodNameHeaderS + this.MethodLabel_Sel + this.MethodNameFooterS + ".xml";

            // パラメタは指定済み

            // DataTableを取得
            cmnDao.ExecSelectFill_DT(returnValue.Dt);

            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>１件更新処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        protected virtual void UOC_UpdateRecord(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());

            // 検索条件の指定

            // AndEqualSearchConditions（主キー
            foreach (string k in parameterValue.AndEqualSearchConditions.Keys)
            {
                // nullチェック（null相当を要検討
                if (parameterValue.AndEqualSearchConditions[k] == null)
                {
                    // == null
                }
                else
                {
                    // != null

                    // 文字列の場合の扱い
                    if (parameterValue.AndEqualSearchConditions[k] is string)
                    {
                        if (!string.IsNullOrEmpty((string)parameterValue.AndEqualSearchConditions[k]))
                        {
                            // パラメタ指定
                            cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                        }
                    }
                    else
                    {
                        // パラメタ指定
                        cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                    }
                }
            }

            // 更新値
            // InsertUpdateValues
            foreach (string k in parameterValue.InsertUpdateValues.Keys)
            {
                // nullチェック（null相当を要検討
                if (parameterValue.InsertUpdateValues[k] == null)
                {
                    // == null
                }
                else
                {
                    // != null

                    // 文字列の場合の扱い
                    if (parameterValue.InsertUpdateValues[k] is string)
                    {
                        if (!string.IsNullOrEmpty((string)parameterValue.InsertUpdateValues[k]))
                        {
                            // パラメタ指定
                            cmnDao.SetParameter(
                                this.UpdateParamHeader + k + this.UpdateParamFooter,
                                parameterValue.InsertUpdateValues[k]);
                        }
                    }
                    else
                    {
                        // パラメタ指定
                        cmnDao.SetParameter(
                            this.UpdateParamHeader + k + this.UpdateParamFooter,
                            parameterValue.InsertUpdateValues[k]);
                    }
                }
            }

            // SQLを設定して
            cmnDao.SQLFileName =
                this.DaoClassNameHeader + parameterValue.TableName + this.DaoClassNameFooter
                + "_" + this.MethodNameHeaderS + this.MethodLabel_Upd + this.MethodNameFooterS + ".xml";

            // パラメタは指定済み

            // 更新処理を実行
            returnValue.Obj = cmnDao.ExecInsUpDel_NonQuery();

            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>１件削除処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        protected virtual void UOC_DeleteRecord(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());

            // 検索条件の指定

            // AndEqualSearchConditions（主キー
            foreach (string k in parameterValue.AndEqualSearchConditions.Keys)
            {
                // nullチェック（null相当を要検討
                if (parameterValue.AndEqualSearchConditions[k] == null)
                {
                    // == null
                }
                else
                {
                    // != null

                    // 文字列の場合の扱い
                    if (parameterValue.AndEqualSearchConditions[k] is string)
                    {
                        if (!string.IsNullOrEmpty((string)parameterValue.AndEqualSearchConditions[k]))
                        {
                            // パラメタ指定
                            cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                        }
                    }
                    else
                    {
                        // パラメタ指定
                        cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                    }
                }
            }

            // SQLを設定して
            cmnDao.SQLFileName =
                this.DaoClassNameHeader + parameterValue.TableName + this.DaoClassNameFooter
                + "_" + this.MethodNameHeaderS + this.MethodLabel_Del + this.MethodNameFooterS + ".xml";

            // パラメタは指定済み

            // 削除処理を実行
            returnValue.Obj = cmnDao.ExecInsUpDel_NonQuery();

            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>バッチ更新処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        protected virtual void UOC_BatchUpdate(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            // ↓業務処理-----------------------------------------------------

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());

            int i = 0; // 件数のカウント
            DataTable dt = (DataTable)parameterValue.Obj;

            // バッチ更新
            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added: // 追加

                        // SQLを設定して
                        cmnDao.SQLFileName =
                            this.DaoClassNameHeader + parameterValue.TableName + this.DaoClassNameFooter
                            + "_" + this.MethodNameHeaderD + this.MethodLabel_Ins + this.MethodNameFooterD + ".xml";

                        // パラメタ指定
                        foreach (DataColumn c in dt.Columns)
                        {
                            // 空文字列も通常の値と同一に扱う
                            cmnDao.SetParameter(c.ColumnName, dr[c]);
                        }

                        // 更新処理を実行
                        i += cmnDao.ExecInsUpDel_NonQuery();

                        break;

                    case DataRowState.Modified: // 更新

                        // SQLを設定して
                        cmnDao.SQLFileName =
                            this.DaoClassNameHeader + parameterValue.TableName + this.DaoClassNameFooter
                            + "_" + this.MethodNameHeaderS + this.MethodLabel_Upd + this.MethodNameFooterS + ".xml";

                        // パラメタ指定
                        foreach (DataColumn dc in dt.Columns)
                        {
                            // 主キー・タイムスタンプ列の設定はUP側で。
                            // また、空文字列も通常の値と同一に扱う。
                            if (parameterValue.AndEqualSearchConditions.ContainsKey(dc.ColumnName))
                            {
                                // Where条件は、DataRowVersion.Originalを付与
                                cmnDao.SetParameter(dc.ColumnName, dr[dc, DataRowVersion.Original]);
                            }
                            else
                            {
                                cmnDao.SetParameter(
                                    this.UpdateParamHeader + dc.ColumnName + this.UpdateParamFooter, dr[dc]);
                            }
                        }

                        // 更新処理を実行
                        i += cmnDao.ExecInsUpDel_NonQuery();

                        break;

                    case DataRowState.Deleted: // 削除

                        // SQLを設定して
                        cmnDao.SQLFileName =
                            this.DaoClassNameHeader + parameterValue.TableName + this.DaoClassNameFooter
                            + "_" + this.MethodNameHeaderS + this.MethodLabel_Del + this.MethodNameFooterS + ".xml";

                        // パラメタ指定
                        foreach (DataColumn c in dt.Columns)
                        {
                            // 主キー・タイムスタンプ列の設定はUP側で。
                            // また、空文字列も通常の値と同一に扱う。
                            if (parameterValue.AndEqualSearchConditions.ContainsKey(c.ColumnName))
                            {
                                // Where条件は、DataRowVersion.Originalを付与
                                cmnDao.SetParameter(c.ColumnName, dr[c, DataRowVersion.Original]);
                            }
                        }

                        // 更新処理を実行
                        i += cmnDao.ExecInsUpDel_NonQuery();

                        break;

                    default: // 上記以外
                        // なにもしない。
                        break;
                }
            }

            // 件数を返却
            returnValue.Obj = i;

            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>Implementing Update process of Multiple tables in Single Transaction</summary>
        /// <param name="parameterValue">Argument class</param>
        protected virtual void UOC_UpdateRecordDM(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            // ↓業務処理-----------------------------------------------------

            int i = 0; // Number count of row update

            // Loop through multiple tables for update operation
            foreach (string tableName in parameterValue.TargetTableNames.Values)
            {
                // 共通Dao
                CmnDao cmnDao = new CmnDao(this.GetDam());

                // AndEqualSearchConditions（主キー
                foreach (string k in parameterValue.AndEqualSearchConditions.Keys)
                {
                    // nullチェック（null相当を要検討
                    if (parameterValue.AndEqualSearchConditions[k] == null)
                    {
                        // == null
                    }
                    else
                    {
                        // != null

                        //  文字列の場合の扱い
                        if (parameterValue.AndEqualSearchConditions[k] is string)
                        {
                            if (!string.IsNullOrEmpty((string)parameterValue.AndEqualSearchConditions[k]))
                            {
                                // パラメタ指定
                                cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                            }
                        }
                        else
                        {
                            // パラメタ指定
                            cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions[k]);
                        }
                    }
                }

                // 更新値
                // InsertUpdateValues
                foreach (string k in parameterValue.InsertUpdateValues.Keys)
                {
                    // nullチェック（null相当を要検討
                    if (parameterValue.InsertUpdateValues[k] == null)
                    {
                        // == null
                    }
                    else
                    {
                        // != null

                        // 文字列の場合の扱い
                        if (parameterValue.InsertUpdateValues[k] is string)
                        {
                            // パラメタ指定
                            if (k.Contains(tableName))
                            {
                                // Setting up parameters by removing tablename from the alias
                                cmnDao.SetParameter(
                                    this.UpdateParamHeader + k.Remove(0, (tableName + "_").Length) + this.UpdateParamFooter,
                                    parameterValue.InsertUpdateValues[k]);
                            }

                        }
                        else
                        {
                            // パラメタ指定
                            if (k.Contains(tableName))
                            {
                                // Setting up parameters by removing tablename from the alias
                                cmnDao.SetParameter(
                                    this.UpdateParamHeader + k.Remove(0, (tableName + "_").Length) + this.UpdateParamFooter,
                                    parameterValue.InsertUpdateValues[k]);
                            }
                        }
                    }
                }

                // SQLを設定して
                cmnDao.SQLFileName =
                    this.DaoClassNameHeader + tableName + this.DaoClassNameFooter
                    + "_" + this.MethodNameHeaderS + this.MethodLabel_Upd + this.MethodNameFooterS + ".xml";

                // 更新処理を実行
                i += cmnDao.ExecInsUpDel_NonQuery();

            }

            returnValue.Obj = i;
            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>Implementing Batch Update process of Multiple tables in Single Transaction</summary>
        /// <param name="parameterValue">Argument class</param>
        protected virtual void UOC_BatchUpdateDM(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            int i = 0; // Number count of row update
            // ↓業務処理-----------------------------------------------------
            foreach (string tableName in parameterValue.TargetTableNames.Values)
            {
                // 共通Dao
                CmnDao cmnDao = new CmnDao(this.GetDam());

                DataTable dt = (DataTable)parameterValue.Obj;

                // バッチ更新
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr.RowState)
                    {
                        case DataRowState.Modified: // 更新

                            // SQLを設定して
                            cmnDao.SQLFileName =
                                this.DaoClassNameHeader + tableName + this.DaoClassNameFooter
                                + "_" + this.MethodNameHeaderS + this.MethodLabel_Upd + this.MethodNameFooterS + ".xml";

                            // パラメタ指定
                            foreach (DataColumn dc in dt.Columns)
                            {
                                // 主キー・タイムスタンプ列の設定はUP側で。
                                // また、空文字列も通常の値と同一に扱う。
                                if (parameterValue.AndEqualSearchConditions.ContainsKey(dc.ColumnName))
                                {
                                    // Where条件は、DataRowVersion.Originalを付与
                                    // Setting up parameters by removing tablename from the alias
                                    cmnDao.SetParameter(dc.ColumnName.Replace(tableName + "_", ""), dr[dc, DataRowVersion.Original]);
                                }
                                else
                                {
                                    // Setting up parameters by removing tablename from the alias
                                    cmnDao.SetParameter(
                                        this.UpdateParamHeader + dc.ColumnName.Replace(tableName + "_", "") + this.UpdateParamFooter, dr[dc]);
                                }
                            }

                            // 更新処理を実行
                            i += cmnDao.ExecInsUpDel_NonQuery();

                            break;

                        case DataRowState.Deleted: // 削除

                            // SQLを設定して
                            cmnDao.SQLFileName =
                                this.DaoClassNameHeader + tableName + this.DaoClassNameFooter
                                + "_" + this.MethodNameHeaderS + this.MethodLabel_Del + this.MethodNameFooterS + ".xml";

                            // パラメタ指定
                            foreach (DataColumn c in dt.Columns)
                            {
                                // 主キー・タイムスタンプ列の設定はUP側で。
                                // また、空文字列も通常の値と同一に扱う。
                                if (parameterValue.AndEqualSearchConditions.ContainsKey(c.ColumnName))
                                {
                                    // Where条件は、DataRowVersion.Originalを付与
                                    cmnDao.SetParameter(c.ColumnName.Replace(tableName + "_", ""), dr[c, DataRowVersion.Original]);
                                }
                            }

                            // 更新処理を実行
                            i += cmnDao.ExecInsUpDel_NonQuery();

                            break;

                        default: // 上記以外
                            // なにもしない。
                            break;
                    }
                }
            }
            // 件数を返却
            returnValue.Obj = i;

            // ↑業務処理-----------------------------------------------------
        }


        /// <summary>Implementing Delete process of Multiple tables in Single Transaction</summary>
        /// <param name="parameterValue">Argument class</param>
        protected virtual void UOC_DeleteRecordDM(_3TierParameterValue parameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // 関連チェック処理
            this.UOC_RelatedCheck(parameterValue);

            int i = 0; // 件数のカウント
            // ↓業務処理-----------------------------------------------------
            foreach (string tableName in parameterValue.TargetTableNames.Values)
            {
                CmnDao cmnDao = new CmnDao(this.GetDam());

                // 検索条件の指定

                // AndEqualSearchConditions（主キー
                foreach (string k in parameterValue.AndEqualSearchConditions.Keys)
                {
                    // nullチェック（null相当を要検討
                    if (parameterValue.AndEqualSearchConditions[k] == null)
                    {
                        // == null
                    }
                    else
                    {
                        // != null

                        // 文字列の場合の扱い
                        if (parameterValue.AndEqualSearchConditions[k] is string)
                        {
                            if (!string.IsNullOrEmpty((string)parameterValue.AndEqualSearchConditions[k]))
                            {
                                // パラメタ指定
                                cmnDao.SetParameter(k.Replace(tableName + "_", ""), parameterValue.AndEqualSearchConditions[k]);
                            }
                        }
                        else
                        {
                            // パラメタ指定
                            cmnDao.SetParameter(k.Replace(tableName + "_", ""), parameterValue.AndEqualSearchConditions[k]);
                        }
                    }
                }

                // SQLを設定して
                cmnDao.SQLFileName =
                    this.DaoClassNameHeader + tableName + this.DaoClassNameFooter
                    + "_" + this.MethodNameHeaderS + this.MethodLabel_Del + this.MethodNameFooterS + ".xml";

                // パラメタは指定済み

                // 削除処理を実行
                i += cmnDao.ExecInsUpDel_NonQuery();
            }
            returnValue.Obj = i;
            // ↑業務処理-----------------------------------------------------

        }

        #endregion

        /// <summary>関連チェック処理を実装可能に</summary>
        /// <param name="parameterValue">引数</param>
        protected virtual void UOC_RelatedCheck(_3TierParameterValue parameterValue) { }
    }
}