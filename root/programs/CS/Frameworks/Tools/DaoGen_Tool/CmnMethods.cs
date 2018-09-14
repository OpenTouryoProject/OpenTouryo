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
//* クラス名        ：CmnMethods
//* クラス日本語名  ：共通関数クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2012/11/21  西野 大介         Entity、DataSet自動生成の対応
//**********************************************************************************

using System;
using System.Text;
using System.Data;
using System.Collections;

namespace DaoGen_Tool
{
    /// <summary>共通関数クラス</summary>
    public class CmnMethods
    {

        #region スキーマ情報の表示

        /// <summary>スキーマ情報の表示</summary>
        /// <param name="table">スキーマ情報</param>
        /// <param name="writeLine">線を引くか引かないか</param>
        /// <returns>スキーマ情報</returns>
        public static string DisplayDataString(DataTable table, bool writeLine)
        {
            StringBuilder sb = new StringBuilder();

            if (writeLine)
            {
                // 行の区切り
                sb.Append("============================" + Environment.NewLine);
            }

            // 行
            foreach (System.Data.DataRow row in table.Rows)
            {
                // 列
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    // カラム名 = 値
                    sb.Append(col.ColumnName + " = " + row[col] + Environment.NewLine);
                }
                if (writeLine)
                {
                    // 行の区切り
                    sb.Append("============================" + Environment.NewLine);
                }
            }

            return sb.ToString();
        }

        /// <summary>スキーマ情報の表示</summary>
        /// <param name="table">スキーマ情報</param>
        /// <param name="writeLine">線を引くか引かないか</param>
        public static void DisplayDataConsole(DataTable table, bool writeLine)
        {
            if (writeLine)
            {
                // 行の区切り
                Console.WriteLine("============================");
            }

            // 行
            foreach (System.Data.DataRow row in table.Rows)
            {
                // 列
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    // カラム名 = 値
                    Console.WriteLine("{0} = {1}", col.ColumnName, row[col]);
                }
                if (writeLine)
                {
                    // 行の区切り
                    Console.WriteLine("============================");
                }
            }
        }

        #endregion

        #region カラム ポジションを昇順にソート

        /// <summary>カラム ポジションを昇順にソートする</summary>
        /// <param name="htColumns">カラム（ハッシュテーブル）</param>
        /// <returns>ソート後のポジション情報（アレイリスト）</returns>
        public static ArrayList sortColumn(Hashtable htColumns)
        {
            // アレイリストを使用

            // ソート用配列
            ArrayList sort = new ArrayList();

            // ソート用配列にポジション（Int32）を追加
            foreach (string position in htColumns.Keys)
            {
                // Int32化
                sort.Add(Int32.Parse(position));
            }

            // ソート
            sort.Sort();

            // 結果を返す。
            return sort;
        }

        #endregion

        #region Entity、DataSet自動生成の対応

        #region DB型情報を.NET型情報に変換する

        /// <summary>型情報</summary>
        public static DataTable DataTypes = null;

        /// <summary>DB型情報を.NET型情報に変換する</summary>
        /// <param name="db_TypeInfo">DB型情報</param>
        /// <returns>.NET型情報</returns>
        public static string ConvertToDotNetTypeInfo(string db_TypeInfo)
        {
            // TypeName（DB型情報） → DataType（.NET型情報）
            foreach (DataRow dr in DataTypes.Rows)
            {
                if (dr["TypeName"].ToString().ToUpper() == db_TypeInfo.ToUpper())
                {
                    return dr["DataType"].ToString();
                }
            }

            return null;
        }

        #region DB固有ロジック

        /// <summary>DB型情報を.NET型情報に変換する</summary>
        /// <param name="db_TypeInfo">DB型情報</param>
        /// <returns>.NET型情報</returns>
        public static string ConvertToDotNetTypeInfo_DB2(string db_TypeInfo)
        {
            // SQL_TYPE_NAME（DB型情報） → FRAMEWORK_TYPE（.NET型情報）
            foreach (DataRow dr in DataTypes.Rows)
            {
                if (dr["SQL_TYPE_NAME"].ToString().ToUpper() == db_TypeInfo.ToUpper())
                {
                    return dr["FRAMEWORK_TYPE"].ToString();
                }
            }

            return null;
        }

        /// <summary>DB型番号をDB型名に変換する（OLEDB）</summary>
        /// <param name="db_TypeInfo">DB型番号</param>
        /// <returns>DB型名</returns>
        public static string ConvertToDBTypeInfo_OLEDB(string db_TypeInfo)
        {
            foreach (DataRow dr in DataTypes.Rows)
            {
                if (dr["NativeDataType"].ToString() == db_TypeInfo)
                {
                    return dr["TypeName"].ToString();
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region DTO生成用


        /// <summary>
        /// 型名の調整を行う。
        /// ・.NETの型名からNullableの型名を返す。
        /// 　C#とVBで事なるので注意すること。
        /// ・VBの場合は、配列表記を []→() へ。
        /// </summary>
        /// <param name="typeName">.NETの型名</param>
        /// <param name="isVB">VBか？</param>
        /// <returns>調整後の型名</returns>
        public static string AdjustTypeName(string typeName, bool isVB)
        {
            // ワーク領域へコピー
            string temp = typeName;

            // Nullableの付与
            switch (temp)
            {
                case "System.Byte[]":
                    break; // そのまま
                case "System.Object":
                    break; // そのまま
                case "System.String":
                    break; // そのまま
                default:
                    // Nullableの付与
                    // VS 2005で新しくなったVisual BasicとC#の新機能を総括 － ＠IT
                    // http://www.atmarkit.co.jp/fdotnet/special/vs2005_02/vs2005_02_02.html
                    if (isVB)
                    {
                        // VB
                        // Dim i As Nullable(Of Integer) = Nothing
                        temp = string.Format("Nullable(Of {0})", typeName);
                    }
                    else
                    {
                        // C#
                        // ・Nullable<int> i;
                        // ・int? i;
                        temp = typeName + "?";
                    }

                    break;
            }

            // VBの場合は、配列表記を []→() へ。
            if (isVB)
            {
                // VB
                temp = temp.Replace("[]", "()");
            }
            else
            {
                // C#
                // なにもしない。
            }

            // 調整後の型名を返す。
            return temp;
        }

        /// <summary>.NETの型名からデータセットの型名を返す。</summary>
        /// <param name="typeName">.NETの型名</param>
        /// <returns>XSDの型名</returns>
        public static string ToXsTypeName(string typeName)
        {
            string temp = "";
            switch (typeName)
            {
                case "System.Boolean":
                    temp = "boolean";
                    break;
                case "System.Byte":
                    temp = "unsignedByte";
                    break;
                case "System.Byte[]":
                    temp = "base64Binary";
                    break;
                case "System.DateTime":
                    temp = "dateTime";
                    break;
                case "System.DateTimeOffset":
                    temp = "anyType";
                    break;
                case "System.Decimal":
                    temp = "decimal";
                    break;
                case "System.Double":
                    temp = "double";
                    break;
                case "System.Guid":
                    temp = "string";
                    break;
                case "System.Int16":
                    temp = "short";
                    break;
                case "System.Int32":
                    temp = "int";
                    break;
                case "System.Int64":
                    temp = "long";
                    break;
                case "System.Object":
                    temp = "anyType";
                    break;
                case "System.SByte":
                    temp = "byte";
                    break;
                case "System.Single":
                    temp = "float";
                    break;
                case "System.String":
                    temp = "string";
                    break;
                case "System.TimeSpan":
                    temp = "duration";
                    break;
                case "System.UInt16":
                    temp = "unsignedShort";
                    break;
                case "System.UInt32":
                    temp = "unsignedInt";
                    break;
                case "System.UInt64":
                    temp = "unsignedLong";
                    break;
                default:
                    throw new Exception("非サポートの型です");
            }

            return temp;

            //if (typeName == "System.Boolean") { return "boolean"; }
            //if (typeName == "System.Byte") { return "unsignedByte"; }
            //if (typeName == "System.Byte[]") { return "base64Binary"; }
            //if (typeName == "System.DateTime") { return "dateTime"; }
            //if (typeName == "System.DateTimeOffset") { return "anyType"; }
            //if (typeName == "System.Decimal") { return "decimal"; }
            //if (typeName == "System.Double") { return "double"; }
            //if (typeName == "System.Guid") { return "string"; }
            //if (typeName == "System.Int16") { return "short"; }
            //if (typeName == "System.Int32") { return "int"; }
            //if (typeName == "System.Int64") { return "long"; }
            //if (typeName == "System.Object") { return "anyType"; }
            //if (typeName == "System.SByte") { return "byte"; }
            //if (typeName == "System.Single") { return "float"; }
            //if (typeName == "System.String") { return "string"; }
            //if (typeName == "System.TimeSpan") { return "duration"; }
            //if (typeName == "System.UInt16") { return "unsignedShort"; }
            //if (typeName == "System.UInt32") { return "unsignedInt"; }
            //if (typeName == "System.UInt64") { return "unsignedLong"; }

            //throw new Exception("非サポートの型です");
        }

        #endregion

        #endregion
    }
}
