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
//* クラス名        ：DTColumn
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（列クラス）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/03/xx  西野 大介         新規作成
//*  2011/10/09  西野 大介         国際化対応
//**********************************************************************************

using System;
using System.Text.RegularExpressions;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>列クラス</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class DTColumn
    {
        #region インスタンス変数

        /// <summary>列名</summary>
        private string _colName;
        /// <summary>列型</summary>
        private DTType _colType;

        #endregion

        #region プロパティ

        /// <summary>列名</summary>
        /// <remarks>確認専用（変更されないように）</remarks>
        public string ColName
        {
            get
            {
                return this._colName;
            }
        }

        /// <summary>列型</summary>
        /// <remarks>確認専用（変更されないように）</remarks>
        public DTType ColType
        {
            get
            {
                return this._colType;
            }
        }

        #endregion

        #region コンストラクタ

        ///// <summary>コンストラクタ</summary>
        //public DTColumn() { }
        // ⇒ 列名、列型は必須

        /// <summary>コンストラクタ</summary>
        /// <param name="colName">列名</param>
        /// <param name="colType">列型</param>
        public DTColumn(string colName, DTType colType)
        {
            // 列名の文字制限（正規表現チェックだが、
            // コンバートのことを考え棟梁部品は使用しない。）
            Regex rgx = new Regex("^[a-zA-Z0-9_-]+$");
            Match mch = rgx.Match(colName);

            if (mch.Success)
            {
                this._colName = colName;
                this._colType = colType;
            }
            else
            {
                // 列名が不正
                throw new Exception(
                    "A column name is inaccurate. "
                    + " The regular expression of the character which can be used:"
                    + " \"^[a-zA-Z0-9_-]+$\"");
            }
        }

        #endregion

        #region 型による扱い

        /// <summary>タイプをチェックする</summary>
        /// <param name="o">指定の値</param>
        /// <param name="dtType">指定の型</param>
        /// <returns>true・false</returns>
        /// <remarks>値をセルに設定するときに使用する。</remarks>
        public static bool CheckType(object o, DTType dtType)
        {
            switch (o.GetType().ToString())
            {
                case "System.Boolean":
                    if (dtType == DTType.Boolean) { return true; }
                    else { return false; }
                case "System.Byte[]":
                    if (dtType == DTType.ByteArray) { return true; }
                    else { return false; }
                case "System.Char":
                    if (dtType == DTType.Char) { return true; }
                    else { return false; }
                case "System.DateTime":
                    if (dtType == DTType.DateTime) { return true; }
                    else { return false; }
                case "System.Decimal":
                    if (dtType == DTType.Decimal) { return true; }
                    else { return false; }
                case "System.Double":
                    if (dtType == DTType.Double) { return true; }
                    else { return false; }
                case "System.Int16":
                    if (dtType == DTType.Int16) { return true; }
                    else { return false; }
                case "System.Int32":
                    if (dtType == DTType.Int32) { return true; }
                    else { return false; }
                case "System.Int64":
                    if (dtType == DTType.Int64) { return true; }
                    else { return false; }
                case "System.Single":
                    if (dtType == DTType.Single) { return true; }
                    else { return false; }
                case "System.String":
                    if (dtType == DTType.String) { return true; }
                    else { return false; }

                default:
                    throw new Exception(
                        "it is a data type which is not supported. ");
            }
        }

        #region AutoCast（Convertを使用）

        /// <summary>キャストする</summary>
        /// <param name="dtType">指定の型</param>
        /// <param name="o">指定の値</param>
        /// <returns>変換後の値</returns>
        public static object AutoCast(DTType dtType, object o)
        {
            switch (dtType)
            {
                case DTType.Boolean:
                    return Convert.ToBoolean(o);

                case DTType.ByteArray:
                    // バイト配列の自動変換はサポートしない
                    throw new Exception(
                        "It is a data type which is not supporting automatic conversion (System.Byte[]). ");

                case DTType.Char:
                    return Convert.ToChar(o);

                case DTType.DateTime:
                    return Convert.ToDateTime(o);

                case DTType.Decimal:
                    return Convert.ToDecimal(o);

                case DTType.Double:
                    return Convert.ToDouble(o);

                case DTType.Int16:
                    return Convert.ToInt16(o);

                case DTType.Int32:
                    return Convert.ToInt32(o);

                case DTType.Int64:
                    return Convert.ToInt64(o);

                case DTType.Single:
                    return Convert.ToSingle(o);

                case DTType.String:
                    // 文字列の自動変換はサポートしない
                    throw new Exception(
                        "It is a data type which is not supporting automatic conversion (System.String). ");

                default:
                    throw new Exception(
                        "it is a data type which is not supported. ");
            }
        }

        #endregion

        #region Enum String

        /// <summary>列挙型を電文上の文字列に変換する</summary>
        /// <param name="dtType">列挙型</param>
        /// <returns>電文上の文字列</returns>
        public static string EnumToString(DTType dtType)
        {
            switch (dtType)
            {
                case DTType.Boolean:
                    return "Boolean";
                case DTType.ByteArray:
                    return "ByteArray";
                case DTType.Char:
                    return "Char";
                case DTType.DateTime:
                    return "DateTime";
                case DTType.Decimal:
                    return "Decimal";
                case DTType.Double:
                    return "Double";
                case DTType.Int16:
                    return "Int16";
                case DTType.Int32:
                    return "Int32";
                case DTType.Int64:
                    return "Int64";
                case DTType.Single:
                    return "Single";
                case DTType.String:
                    return "String";

                default:
                    throw new Exception(
                        "it is a data type which is not supported. ");
            }
        }

        /// <summary>電文上の文字列を列挙型に変換する</summary>        
        /// <param name="strType">電文上の文字列</param>
        /// <returns>列挙型</returns>
        public static DTType StringToEnum(string strType)
        {
            // ※ 大文字小文字を区別しない
            switch (strType.ToLower())
            {
                case "boolean":
                    return DTType.Boolean;
                case "bytearray":
                    return DTType.ByteArray;
                case "char":
                    return DTType.Char;
                case "datetime":
                    return DTType.DateTime;
                case "decimal":
                    return DTType.Decimal;
                case "double":
                    return DTType.Double;
                case "int16":
                    return DTType.Int16;
                case "int32":
                    return DTType.Int32;
                case "int64":
                    return DTType.Int64;
                case "single":
                    return DTType.Single;
                case "string":
                    return DTType.String;

                default:
                    throw new Exception(
                        "it is a data type which is not supported. ");
            }
        }

        #endregion

        #endregion
    }
}
