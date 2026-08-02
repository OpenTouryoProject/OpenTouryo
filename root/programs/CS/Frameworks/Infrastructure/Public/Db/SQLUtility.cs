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
//* クラス名        ：SQLUtility
//* クラス日本語名  ：SQL生成ユーティリティ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/07/03  西野 大介         新規作成
//*  2013/09/20  西野 大介         設計変更：GetInsertSQLParts、GetUpdateSQLParts
//*  2013/10/04  西野 大介         Convert関数：30文字以上はサイズ指定（エスケープ前）
//*  2014/01/24  Sai Krishna       added code for batch processing supporting PostGreSQL
//*  2014/01/24  Santoshkumar      added code for batch processing supporting Oracle
//*  2014/01/30  Sai Krishna       added code for batch processing supporting MySQL
//*  2014/01/30  Santoshkumar      added code for batch processing supporting DB2
//*  2014/03/04  Santoshkumar      Modified code for converting char data type for batch processing supporting DB2 and Oracle
//*  2026/08/02  玄人 幸道         GetUpdateSQLParts：複合主キーで誤ったUPDATE文を生成する問題を修正
//**********************************************************************************

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Db
{
    /// <summary>SQL生成ユーティリティ</summary>
    public class SQLUtility
    {
        #region パブリック

        /// <summary>囲い文字（開始）</summary>
        public char OpeningBracket
        {
            get
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                        return '[';

                    case DbEnum.DBMSType.Oracle:
                    case DbEnum.DBMSType.DB2:
                    case DbEnum.DBMSType.HiRDB:
                    case DbEnum.DBMSType.PstGrS:
                        return '"';

                    case DbEnum.DBMSType.MySQL:
                        return '`';

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
        }

        /// <summary>囲い文字（終了）</summary>
        public char ClosingBracket
        {
            get
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                        return ']';

                    case DbEnum.DBMSType.Oracle:
                    case DbEnum.DBMSType.DB2:
                    case DbEnum.DBMSType.HiRDB:
                    case DbEnum.DBMSType.PstGrS:
                        return '"';

                    case DbEnum.DBMSType.MySQL:
                        return '`';

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
        }

        #endregion

        #region プライベート

        /// <summary>DBMSの種類</summary>
        private DbEnum.DBMSType _dbms;

        /// <summary>文字列のコンバート先の型</summary>
        private string _convertString = "";

        /// <summary>日付型のFormatString</summary>
        private string _dateTimeFormatString = "";

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="dbms">DBMSの種類</param>
        public SQLUtility(DbEnum.DBMSType dbms)
            : this(dbms, "", "") { }

        /// <summary>コンストラクタ</summary>
        /// <param name="dbms">DBMSの種類</param>
        /// <param name="convertString">文字列変換方法（空の場合は既定値を使用）</param>
        public SQLUtility(DbEnum.DBMSType dbms, string convertString)
            : this(dbms, convertString, "") { }

        /// <summary>コンストラクタ</summary>
        /// <param name="dbms">DBMSの種類</param>
        /// <param name="convertString">文字列変換方法（空の場合は既定値を使用）</param>
        /// <param name="dateTimeFormatString">日付を文字列化する際に使用するFormatString</param>
        public SQLUtility(DbEnum.DBMSType dbms, string convertString, string dateTimeFormatString)
        {
            this._dbms = dbms;

            switch (this._dbms)
            {
                case DbEnum.DBMSType.SQLServer:

                    // convertString
                    if (string.IsNullOrEmpty(convertString))
                    {
                        // 既定値
                        this._convertString = "nvarchar";
                    }
                    else
                    {
                        // 指定の値
                        this._convertString = convertString;
                    }

                    // dateTimeFormatString
                    if (string.IsNullOrEmpty(dateTimeFormatString))
                    {
                        // 既定値
                        this._dateTimeFormatString = "yyyy/MM/dd HH:mm:ss.fff";
                    }
                    else
                    {
                        // 指定の値
                        this._dateTimeFormatString = dateTimeFormatString;
                    }

                    break;

                case DbEnum.DBMSType.PstGrS:

                    // convertString
                    if (string.IsNullOrEmpty(convertString))
                    {
                        // 既定値
                        this._convertString = "text";
                    }
                    else
                    {
                        // 指定の値
                        this._convertString = convertString;
                    }

                    // dateTimeFormatString
                    if (string.IsNullOrEmpty(dateTimeFormatString))
                    {
                        // 既定値
                        this._dateTimeFormatString = "yyyy/MM/dd HH:mm:ss.fff";
                    }
                    else
                    {
                        // 指定の値
                        this._dateTimeFormatString = dateTimeFormatString;
                    }

                    break;

                case DbEnum.DBMSType.Oracle:
                    // convertString
                    if (string.IsNullOrEmpty(convertString))
                    {
                        // 既定値
                        this._convertString = "varchar2";
                    }
                    else
                    {
                        // 指定の値
                        this._convertString = convertString;
                    }

                    // dateTimeFormatString
                    if (string.IsNullOrEmpty(dateTimeFormatString))
                    {
                        // 既定値
                        this._dateTimeFormatString = "dd-MMM-yyyy hh:mm:ss tt";
                    }
                    else
                    {
                        // 指定の値
                        this._dateTimeFormatString = dateTimeFormatString;
                    }

                    break;

                case DbEnum.DBMSType.MySQL:
                    // convertString
                    if (string.IsNullOrEmpty(convertString))
                    {
                        // 既定値
                        this._convertString = "char";
                    }
                    else
                    {
                        // 指定の値
                        this._convertString = convertString;
                    }

                    // dateTimeFormatString
                    if (string.IsNullOrEmpty(dateTimeFormatString))
                    {
                        // 既定値
                        this._dateTimeFormatString = "yyyy/MM/dd HH:mm:ss.fff";
                    }
                    else
                    {
                        // 指定の値
                        this._dateTimeFormatString = dateTimeFormatString;
                    }

                    break;

                case DbEnum.DBMSType.DB2:

                    // convertString
                    if (string.IsNullOrEmpty(convertString))
                    {
                        // 既定値
                        this._convertString = "varchar";
                    }
                    else
                    {
                        // 指定の値
                        this._convertString = convertString;
                    }

                    // dateTimeFormatString
                    if (string.IsNullOrEmpty(dateTimeFormatString))
                    {
                        // 既定値
                        this._dateTimeFormatString = "yyyy-MM-dd-HH.mm.ss.fff";
                    }
                    else
                    {
                        // 指定の値
                        this._dateTimeFormatString = dateTimeFormatString;
                    }
                    break;

                default:
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        #endregion

        /// <summary>Insert系SQLのパーツを生成</summary>
        /// <param name="dt">入力DataTable</param>
        /// <returns>Insert系SQLのパーツ文字列配列</returns>
        public string[] GetInsertSQLParts(DataTable dt)
        {
            // ワーク
            string temp = "";
            StringBuilder sb = null;

            // リスト
            List<string> list = new List<string>();

            // 列情報
            sb = new StringBuilder();
            sb.Append("(");

            foreach (DataColumn dc in dt.Columns)
            {
                sb.Append(this.OpeningBracket + dc.ColumnName + this.ClosingBracket + ", ");
            }

            // 最後の文字を置き換える（[,]→[)]）。
            temp = sb.ToString();
            temp = temp.Substring(0, temp.Length - 2) + ")";
            list.Add(temp);

            // 行情報
            foreach (DataRow dr in dt.Rows)
            {
                sb = new StringBuilder();
                sb.Append("(");

                foreach (object obj in dr.ItemArray)
                {
                    // パラメタをSQLに変換する。
                    sb.Append(this.ConvertParameterToSQL(obj) + ", ");
                }

                // 最後の文字を置き換える（[,]→[)]）。
                temp = sb.ToString();
                temp = temp.Substring(0, temp.Length - 2) + ")";
                list.Add(temp);
            }

            // 文字列配列化して戻す。
            return list.ToArray();
        }

        /// <summary>Update系SQLのパーツを生成</summary>
        /// <param name="dt">入力DataTable</param>
        /// <param name="primaryKeys">主キー情報</param>
        /// <returns>Update系SQLのパーツ文字列配列</returns>
        public string[] GetUpdateSQLParts(DataTable dt, string[] primaryKeys)
        {
            // ワーク
            List<string> colSet = new List<string>();
            string tempSet = "";
            StringBuilder sbSet = null;

            List<string> colWhere = new List<string>();
            string tempWhere = "";
            StringBuilder sbWhere = null;

            // リスト
            List<string> list = new List<string>();

            // 列情報
            foreach (DataColumn dc in dt.Columns)
            {
                bool isPK = false;

                // 主キー？
                foreach (string pkColName in primaryKeys)
                {
                    // 主キー列
                    if (pkColName == dc.ColumnName)
                    {
                        isPK = true;
                        colWhere.Add(dc.ColumnName);
                    }
                }

                // 更新対象列
                if (!isPK)
                {
                    colSet.Add(dc.ColumnName);
                }
            }

            // 生成できないケースは、ここで打ち切る（DBMSによらず共通）。
            //
            // ・更新対象列が無い    … SET句が空のSQLは構文エラーになる。
            // ・主キーが無い        … 行を特定できず、WHERE句が無いと全行更新になる。
            //
            // 打ち切らずに進むと、末尾の切り詰め処理が初期値のStringBuilderを削るため、
            // "SE" や "WH" といった壊れた文字列を返してしまう。
            if (colSet.Count == 0 || colWhere.Count == 0)
            {
                return list.ToArray();
            }

            // PostgreSQL・MySQLには、1文で複数行を別々の値に更新する構文が無いため、
            // 「CASE ... WHEN ... THEN」で行ごとの値を切り替える。
            // 他のDBMS（else側）は1行1文を行数分returnするが、こちらは1文にまとめる。
            //
            // ＜生成される UPDATE 文の全体像＞
            //
            // 本メソッドが返すのは "SET ... WHERE ..." の部分のみで、
            // 呼び出し側が "UPDATE <テーブル名>" を前置して1文を組み立てる。
            //
            //   UPDATE "Orders"                                    ← 呼び出し側が付与
            //   SET "Qty" = CASE
            //         WHEN "OrderID" = 1 AND "ProductID" = 10 THEN 100
            //         WHEN "OrderID" = 2 AND "ProductID" = 20 THEN 200
            //         ELSE "Qty"                                   ← 対象外の行は現状維持
            //       END,
            //       "Note" = CASE
            //         WHEN "OrderID" = 1 AND "ProductID" = 10 THEN 'a'
            //         WHEN "OrderID" = 2 AND "ProductID" = 20 THEN 'b'
            //         ELSE "Note"
            //       END
            //   WHERE ("OrderID" = 1 AND "ProductID" = 10)         ← 主キーの組み合わせで絞る
            //      OR ("OrderID" = 2 AND "ProductID" = 20)
            //
            // ＜構造＞
            //   ・更新対象列（主キー以外）ごとに CASE 式を1つ作る。
            //   ・CASE の WHEN は入力DataTableの行数分並び、行を主キーで特定して値を与える。
            //   ・WHERE は同じ主キーの組み合わせをORで並べ、更新対象の行だけに限定する。
            //
            // ＜複合主キーの扱い＞
            //   主キーが複数列ある場合、WHEN も WHERE も「列ごとに独立」ではなく
            //   「組み合わせ（AND）」で判定しなければならない。
            //   列ごとに独立させると、WHERE が直積になって更新対象でない行に一致し、
            //   さらに CASE も先に一致した枝が勝つため、誤った行に誤った値が入る。
            //
            // ＜ELSE を付ける理由＞
            //   CASE はどの WHEN にも一致しないと NULL を返す。
            //   WHERE で対象行に限定しているため通常は一致するが、
            //   NULL 上書きという最悪の失敗を避けるため、保険として自身の列を指定する。
            if (this._dbms == DbEnum.DBMSType.PstGrS || this._dbms == DbEnum.DBMSType.MySQL)
            {
                sbSet = new StringBuilder("SET ");
                sbWhere = new StringBuilder("\r\nWHERE ");

                // 更新対象列（主キー以外）ごとに CASE 式を作る。
                bool isFirstSet = true;
                foreach (string set in colSet)
                {
                    if (!isFirstSet)
                    {
                        sbSet.Append(",\r\n    ");
                    }
                    isFirstSet = false;

                    sbSet.Append(this.OpeningBracket + set + this.ClosingBracket + " = CASE");

                    // 行ごとに「主キーの組み合わせ ＝ その行の値」を条件とする。
                    foreach (DataRow dr in dt.Rows)
                    {
                        sbSet.Append("\r\n        WHEN " + this.GetPrimaryKeyCondition(dr, colWhere)
                            + " THEN " + this.ConvertParameterToSQL(dr[set]));
                    }

                    // どのWHENにも一致しない行は、現在の値を保つ。
                    sbSet.Append("\r\n        ELSE " + this.OpeningBracket + set + this.ClosingBracket);
                    sbSet.Append("\r\n      END");
                }

                // WHERE は主キーの組み合わせをORで並べ、更新対象の行だけに限定する。
                bool isFirstWhere = true;
                foreach (DataRow dr in dt.Rows)
                {
                    if (!isFirstWhere)
                    {
                        sbWhere.Append("\r\n   OR ");
                    }
                    isFirstWhere = false;

                    sbWhere.Append("(" + this.GetPrimaryKeyCondition(dr, colWhere) + ")");
                }

                tempSet = sbSet.ToString();
                tempWhere = sbWhere.ToString();

                // 結合して追加。
                list.Add(tempSet + " " + tempWhere);
            }
            else
            {
                // Update statement for remaining databases
                // 行情報
                foreach (DataRow dr in dt.Rows)
                {
                    sbSet = new StringBuilder("SET ");
                    sbWhere = new StringBuilder("WHERE ");

                    // 主キー列
                    foreach (string where in colWhere)
                    {
                        // パラメタをSQLに変換する。
                        sbWhere.Append(
                            this.OpeningBracket + where + this.ClosingBracket
                            + " = " + this.ConvertParameterToSQL(dr[where]) + " AND ");
                    }

                    // 更新対象列
                    foreach (string set in colSet)
                    {
                        sbSet.Append(
                            this.OpeningBracket + set + this.ClosingBracket
                            + " = " + this.ConvertParameterToSQL(dr[set]) + ", ");
                    }

                    // 最後の文字を削除。
                    tempWhere = sbWhere.ToString();
                    tempWhere = tempWhere.Substring(0, tempWhere.Length - 4);

                    // 最後の文字を削除。
                    tempSet = sbSet.ToString();
                    tempSet = tempSet.Substring(0, tempSet.Length - 2);

                    // 結合して追加。
                    list.Add(tempSet + " " + tempWhere);
                }
            }

            // 文字列配列化して戻す。
            return list.ToArray();
        }

        /// <summary>1行を特定する主キーの条件式を生成する。</summary>
        /// <param name="dr">対象行</param>
        /// <param name="primaryKeys">主キー列名のリスト</param>
        /// <returns>条件式（例： "OrderID" = 1 AND "ProductID" = 10 ）</returns>
        /// <remarks>
        /// 複合主キーの場合は、列ごとに独立させず AND で結合する。
        /// 独立させると行を一意に特定できず、意図しない行に一致する。
        /// </remarks>
        private string GetPrimaryKeyCondition(DataRow dr, List<string> primaryKeys)
        {
            StringBuilder sb = new StringBuilder();

            bool isFirst = true;
            foreach (string pk in primaryKeys)
            {
                if (!isFirst)
                {
                    sb.Append(" AND ");
                }
                isFirst = false;

                sb.Append(this.OpeningBracket + pk + this.ClosingBracket
                    + " = " + this.ConvertParameterToSQL(dr[pk]));
            }

            return sb.ToString();
        }

        /// <summary>パラメタをSQLに変換する。</summary>
        /// <param name="obj">パラメタ</param>
        /// <returns>SQL化したパラメタ</returns>
        public string ConvertParameterToSQL(object obj)
        {
            StringBuilder sb = new StringBuilder();

            if (obj.GetType() == typeof(char))
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                        // コンバート
                        sb.Append("Convert(" + this._convertString + ", '" + obj.ToString() + "')");
                        break;

                    case DbEnum.DBMSType.MySQL:
                    case DbEnum.DBMSType.PstGrS:
                        sb.Append("Cast('" + obj.ToString() + "' as " + this._convertString + ")");
                        break;

                    case DbEnum.DBMSType.Oracle:
                        sb.Append("TO_CHAR('" + obj.ToString() + "')");
                        break;

                    case DbEnum.DBMSType.DB2:
                        sb.Append("CHAR('" + obj.ToString() + "')");
                        break;

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else if (obj.GetType() == typeof(string))
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                        // コンバート ＋ サニタイジング

                        // Convert関数は30文字以上の場合、サイズ指定が必要
                        if (obj.ToString().Length == 0)
                        {
                            // 0文字ではエラーになるのでサイズ指定しない。
                            sb.Append(
                                "Convert("
                                + this._convertString + ", '')");
                        }
                        else
                        {
                            // 30文字以上はサイズ指定する（エスケープ前の文字数）。
                            sb.Append(
                                "Convert("
                                + this._convertString + "(" + obj.ToString().Length + "), '"
                                + obj.ToString().Replace("'", "''") + "')");
                        }

                        break;

                    case DbEnum.DBMSType.PstGrS:
                        // コンバート ＋ サニタイジング
                        if (obj.ToString().Length == 0)
                        {
                            sb.Append("''");
                        }
                        else
                        {
                            sb.Append(
                                "Cast('"
                                + obj.ToString().Replace("'", "''") + "' as "
                                + this._convertString + ")");
                        }

                        break;

                    case DbEnum.DBMSType.Oracle:
                        // コンバート ＋ サニタイジング
                        // Convert the datatype to the specific data type
                        if (obj.ToString().Length == 0)
                        {
                            // Do not use CAST function here because size of the string will be zero 
                            sb.Append("To_CHAR('')");
                        }
                        else
                        {
                            //use CAST to 
                            sb.Append(
                                "CAST('"
                                + obj.ToString().Replace("'", "''") + "' AS "
                                + this._convertString + "(" + obj.ToString().Length + ")" + ")");
                        }

                        break;

                    case DbEnum.DBMSType.MySQL:
                        // コンバート ＋ サニタイジング
                        if (obj.ToString().Length == 0)
                        {
                            sb.Append("Cast('' as " + this._convertString + ")");
                        }
                        else
                        {
                            sb.Append(
                                "Cast('"
                                + obj.ToString().Replace("'", "''") + "' as "
                                + this._convertString + "(" + obj.ToString().Length + "))");
                        }
                        break;

                    case DbEnum.DBMSType.DB2:
                        // コンバート ＋ サニタイジング
                        if (obj.ToString().Length == 0)
                        {
                            sb.Append("CAST('' AS CHAR)");
                        }
                        else
                        {
                            sb.Append(
                                "CAST('"
                                + obj.ToString().Replace("'", "''") + "' AS "
                                + this._convertString + "(" + obj.ToString().Length + ")" + ")");
                        }
                        break;

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else if (obj.GetType() == typeof(byte))
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                    case DbEnum.DBMSType.MySQL:
                        // Hex文字列化
                        sb.Append("0x" + (CustomEncode.ToHexString((new byte[] { (byte)obj })).Replace(" ", "")));
                        break;

                    case DbEnum.DBMSType.PstGrS:
                        sb.Append("decode('" + (CustomEncode.ToHexString((byte[])obj).Replace(" ", "")) + "', 'hex')");
                        break;

                    case DbEnum.DBMSType.Oracle:
                        sb.Append("hextoraw('" + (CustomEncode.ToHexString((new byte[] { (byte)obj })).Replace(" ", "")) + "')");
                        break;

                    case DbEnum.DBMSType.DB2:
                        sb.Append("x'" + (CustomEncode.ToHexString((new byte[] { (byte)obj })).Replace(" ", "")) + "'");
                        break;

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else if (obj.GetType() == typeof(byte[]))
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                    case DbEnum.DBMSType.MySQL:
                        // Hex文字列化
                        sb.Append("0x" + (CustomEncode.ToHexString((byte[])obj).Replace(" ", "")));
                        break;

                    case DbEnum.DBMSType.PstGrS:
                        sb.Append("decode('" + (CustomEncode.ToHexString((byte[])obj).Replace(" ", "")) + "', 'hex')");
                        break;

                    case DbEnum.DBMSType.Oracle:
                        sb.Append("hextoraw('" + (CustomEncode.ToHexString((byte[])obj).Replace(" ", "")) + "')");
                        break;

                    case DbEnum.DBMSType.DB2:
                        sb.Append("x'" + (CustomEncode.ToHexString((byte[])obj).Replace(" ", "")) + "'");
                        break;

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else if (obj.GetType() == typeof(DateTime))
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                    case DbEnum.DBMSType.Oracle:
                    case DbEnum.DBMSType.MySQL:
                    case DbEnum.DBMSType.DB2:
                        // DateTime文字列化
                        sb.Append("'" + ((DateTime)(obj)).ToString(this._dateTimeFormatString) + "'");
                        break;
		    
                    case DbEnum.DBMSType.PstGrS:
                        sb.Append("Cast('" + ((DateTime)(obj)).ToString(this._dateTimeFormatString) + "' as date)");
                        break;

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else if (obj.GetType() == typeof(DBNull))
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                    case DbEnum.DBMSType.PstGrS:
                    case DbEnum.DBMSType.Oracle:
                    case DbEnum.DBMSType.MySQL:
                    case DbEnum.DBMSType.DB2:
                        // NULL
                        sb.Append("NULL");
                        break;

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else if (obj == null)
            {
                switch (this._dbms)
                {
                    case DbEnum.DBMSType.SQLServer:
                    case DbEnum.DBMSType.Oracle:
                    case DbEnum.DBMSType.MySQL:
                    case DbEnum.DBMSType.DB2:
                        // NULL
                        sb.Append("NULL");
                        break;

                    case DbEnum.DBMSType.PstGrS:
                        // For Postgre DEFAULT
                        sb.Append("DEFAULT");
                        break;

                    default:
                        throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else
            {
                sb.Append(obj.ToString());
            }

            return sb.ToString();
        }
    }
}
