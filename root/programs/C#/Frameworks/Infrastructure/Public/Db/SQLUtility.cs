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
//*  2013/07/03  西野  大介        新規作成
//*  2013/09/20  西野  大介        設計変更：GetInsertSQLParts、GetUpdateSQLParts
//*  2013/10/04  西野  大介        Convert関数：30文字以上はサイズ指定（エスケープ前）
//*  2014/01/24  Sai Krishna       added code for batch processing supporting PostGreSQL
//*  2014/01/24  Santoshkumar      added code for batch processing supporting Oracle
//*  2014/01/30  Sai Krishna       added code for batch processing supporting MySQL
//*  2014/01/30  Santoshkumar      added code for batch processing supporting DB2
//*  2014/03/04 Santoshkumar       Modified code for converting char data type for batch processing supporting DB2 and Oracle
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
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

            // As per the PostGreSQL and MySQL syntax for batch update
            // we have to use a "CASE ... WHEN ... THEN" while updating multiple records.
            if (this._dbms == DbEnum.DBMSType.PstGrS || this._dbms == DbEnum.DBMSType.MySQL)
            {
                sbSet = new StringBuilder("SET ");
                sbWhere = new StringBuilder("\r\nWHERE ");
                int count = 0;

                // 更新対象列
                foreach (string set in colSet)
                {
                    sbSet.Append(set + " = CASE");

                    foreach (string where in colWhere)
                    {
                        if (count < colWhere.Count)
                            sbWhere.Append(where + " IN (");
                        foreach (DataRow dr in dt.Rows)
                        {
                            sbSet.Append(
                                " WHEN " + where + " =" + this.ConvertParameterToSQL(dr[where])
                                + " THEN " + this.ConvertParameterToSQL(dr[set]) + "\r\n");

                            // パラメタをSQLに変換する。
                            if (count < colWhere.Count)
                                sbWhere.Append(this.ConvertParameterToSQL(dr[where]) + ", ");
                        }
                        // Sharpen last comma and AND
                        if (count < colWhere.Count)
                        {
                            sbWhere.Remove((sbWhere.Length - 2), 2);
                            sbWhere.Append(") AND ");
                        }
                        count++;
                    }
                    // Adding END to the CASE
                    sbSet.Append(" END,\r\n");
                }

                // Removing Last AND.
                tempWhere = sbWhere.ToString().Substring(0, sbWhere.Length - 4);
                tempSet = sbSet.ToString();
                // Removing Last END.
                tempSet = tempSet.Substring(0, tempSet.Length - 3);

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
