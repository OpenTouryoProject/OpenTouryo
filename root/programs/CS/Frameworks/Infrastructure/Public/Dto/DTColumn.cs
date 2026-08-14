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
//*  2026/08/14  玄人 幸道         AutoCastのカルチャを固定した（#544）。
//**********************************************************************************

using System;
using System.Globalization;
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
            // **カルチャを固定する。**（#544）
            //
            // 既定のカルチャで解釈すると、小数点が「,」になる環境で書いた値を
            // 別の環境で読んだときに、**例外にならずに値が変わる**
            // （"1234,56" を ja-JP で読むと 123456 になる）。
            // 書き出し側（CustomMarshaler.StringFromPrimitivetype）も固定してある。
            //
            // 「.」を小数点に使うカルチャ（ja-JP / en-US など）では、
            // 解釈は従来と同じである。
            IFormatProvider provider = CultureInfo.InvariantCulture;

            switch (dtType)
            {
                case DTType.Boolean:
                    return Convert.ToBoolean(o, provider);

                case DTType.ByteArray:
                    // バイト配列の自動変換はサポートしない
                    throw new Exception(
                        "It is a data type which is not supporting automatic conversion (System.Byte[]). ");

                case DTType.Char:
                    return Convert.ToChar(o, provider);

                case DTType.DateTime:
                    return Convert.ToDateTime(o, provider);

                case DTType.Decimal:
                    return Convert.ToDecimal(o, provider);

                case DTType.Double:
                    return Convert.ToDouble(o, provider);

                case DTType.Int16:
                    return Convert.ToInt16(o, provider);

                case DTType.Int32:
                    return Convert.ToInt32(o, provider);

                case DTType.Int64:
                    return Convert.ToInt64(o, provider);

                case DTType.Single:
                    return Convert.ToSingle(o, provider);

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

        #region 値と文字列の相互変換

        // ＜なぜここに置くか＞（#544）
        //   以前は Util.CustomMarshaler にあったが、
        //   ・CustomMarshaler の本体はアンマネージ構造体の相互運用（Marshal クラス）で、
        //     DTType の変換とは別の責務だった（同じ「マーシャリング」という語が
        //     2 つの意味で使われていた）
        //   ・Util から Dto への参照が、この 2 メソッドのためだけに生じていた
        //     （Dto → Util の参照と合わせて循環していた）
        //   ・対になる AutoCast / CheckType / EnumToString は、もともとここにある
        //   という理由で、DTType の変換をこのクラスに集約した。
        //
        //   CustomMarshaler 側には、当面は転送するだけのメソッドを残してある。

        /// <summary>プリミティブな値を文字列にする</summary>
        /// <param name="primitiveType">値</param>
        /// <param name="checkType">行単位で区切る形式向けに、改行を退避するか</param>
        /// <returns>文字列（値が null なら null）</returns>
        /// <remarks>
        /// **カルチャに依存しない形にする。**
        /// 機械の間で持ち回る形式を作るためのメソッドで、
        /// 既定のカルチャで書くと、小数点が「,」になる環境で書いた値を
        /// 別の環境で読んだときに、**例外にならずに値が変わる**（#544）。
        /// </remarks>
        public static string StringFromPrimitivetype(object primitiveType, bool checkType)
        {
            string convertedString = null;

            if (primitiveType != null)
            {
                if (DTColumn.CheckType(primitiveType, DTType.DateTime))
                {
                    // ISO 8601（ラウンドトリップ書式）で書き出す。
                    //
                    // ＜旧書式（yyyy/M/d-H:m:s.fff）から変えた理由＞（#544）
                    //   ・他システムと JSON をやり取りできない（ISO 8601 が事実上の標準）
                    //   ・ミリ秒より細かい値と DateTimeKind が落ちる
                    //   ・カルチャに依存しない形であることが、書式からは読み取れない
                    //
                    // 読み込み（PrimitivetypeFromString）は旧書式も受け付けるため、
                    // 過去に書き出したデータはそのまま読める。
                    convertedString = ((DateTime)primitiveType)
                        .ToString("o", CultureInfo.InvariantCulture);
                }
                else if (DTColumn.CheckType(primitiveType, DTType.ByteArray))
                {
                    convertedString = Convert.ToBase64String((byte[])primitiveType);
                }
                else if (DTColumn.CheckType(primitiveType, DTType.String))
                {
                    if (checkType == true)
                    {
                        // 行単位で区切る形式（DTTables.Save）向けに、改行を退避する。
                        //
                        // 「\r」を「\rrnr:」、「\n」を「\rrnn:」に置き換えると、
                        // WriteLine / ReadLine を通したときに
                        //   … 元の文字列の前半
                        //   rnr: 元の文字列の後半
                        // のように分かれ、DTTables.Load 側で連結して元に戻せる。
                        //
                        // ＜以前は、置換が効いていなかった＞（#544）
                        //   3 行とも primitiveType.ToString() から書き始めており、
                        //   前の置換結果を捨てていたため、最後の 1 行だけが効いていた。
                        //   その結果、改行を含む文字列は Load 側で
                        //   **改行以降が捨てられていた**（例外は出ない）。
                        convertedString = primitiveType.ToString()
                            .Replace("\r", "\rrnr:")
                            .Replace("\n", "\rrnn:");
                    }
                    else
                    {
                        convertedString = primitiveType.ToString();
                    }
                }
                else if (DTColumn.CheckType(primitiveType, DTType.Double))
                {
                    // 「R」は最短のラウンドトリップ書式。
                    //
                    // ＜既定の書式では往復しない＞（#544）
                    //   .NET Framework の既定は有効桁 15 桁で、
                    //   ランダムな Double の 94% が元の値に戻らない。
                    //   Double.MaxValue に至っては、読み戻しで OverflowException になる
                    //   （15 桁への丸めで、表現できる上限を超えるため）。
                    //   .NET (Core) 3.0 以降の既定は最短ラウンドトリップだが、
                    //   両者で挙動が割れるため、書式を明示して揃える。
                    convertedString = ((double)primitiveType).ToString("R", CultureInfo.InvariantCulture);
                }
                else if (DTColumn.CheckType(primitiveType, DTType.Single))
                {
                    // 理由は Double と同じ（Single は 68% が往復しなかった）。
                    convertedString = ((float)primitiveType).ToString("R", CultureInfo.InvariantCulture);
                }
                else
                {
                    // 「.」を小数点に使うカルチャ（ja-JP / en-US など）では、
                    // 出力は従来と同じである。
                    convertedString = Convert.ToString(primitiveType, CultureInfo.InvariantCulture);
                }
            }

            return convertedString;
        }

        /// <summary>文字列をプリミティブな値に戻す</summary>
        /// <param name="colType">列の型</param>
        /// <param name="cellString">文字列</param>
        /// <returns>値（文字列が null なら null）</returns>
        public static object PrimitivetypeFromString(DTType colType, string cellString)
        {
            object convertedPrimitiveType = null;

            if (cellString != null)
            {
                // ByteArray
                if (colType == DTType.ByteArray)
                {
                    convertedPrimitiveType = Convert.FromBase64String(cellString);
                }
                // DateTime
                else if (colType == DTType.DateTime)
                {
                    convertedPrimitiveType = DTColumn.StringToDateTime(cellString);
                }
                // String
                else if (colType == DTType.String)
                {
                    // 変換の必要が無いので、そのまま返す。
                    //
                    // ＜以前は例外になっていた＞（#544）
                    //   AutoCast に落ちており、AutoCast は String を
                    //   「自動変換の対象外」として例外を投げる。このため
                    //   呼び出し側が手前で String を分岐して避けていた。
                    //   本メソッド単体では使えない状態だったので、ここで受ける。
                    convertedPrimitiveType = cellString;
                }
                else
                {
                    convertedPrimitiveType = DTColumn.AutoCast(colType, cellString);
                }
            }

            return convertedPrimitiveType;
        }

        /// <summary>
        /// 文字列を DateTime に戻す（ISO 8601 と旧書式の両方を受け付ける）。
        /// </summary>
        /// <param name="cellString">セルの文字列</param>
        /// <returns>DateTime</returns>
        /// <remarks>
        /// StringFromPrimitivetype は ISO 8601 で書き出すが、
        /// それ以前に書き出したデータ（yyyy/M/d-H:m:s.fff）も読めるようにしてある（#544）。
        /// 旧書式を落とすと、永続化・退避した既存データが読めなくなるため。
        /// </remarks>
        private static DateTime StringToDateTime(string cellString)
        {
            // ISO 8601（ラウンドトリップ書式）
            //
            // RoundtripKind を指定して、DateTimeKind（Utc / Local / Unspecified）を保つ。
            // 指定しないと、末尾に Z が付いた値が Local へ変換されてしまう。
            DateTime iso;
            if (DateTime.TryParseExact(cellString, "o",
                    CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out iso))
            {
                return iso;
            }

            // 旧書式（yyyy/M/d-H:m:s.fff）
            //
            // 「-」で日付部と時刻部を分けている。ISO 8601 は日付部にも「-」を含むため、
            // 上の ISO の判定を必ず先に行うこと。
            string ymd = cellString.Split('-')[0];
            string hmsf = cellString.Split('-')[1];

            return new DateTime(
                int.Parse(ymd.Split('/')[0]),
                int.Parse(ymd.Split('/')[1]),
                int.Parse(ymd.Split('/')[2]),
                int.Parse(hmsf.Split(':')[0]),
                int.Parse(hmsf.Split(':')[1]),
                int.Parse(hmsf.Split(':')[2].Split('.')[0]),
                int.Parse(hmsf.Split(':')[2].Split('.')[1]));
        }

        #endregion

        #endregion
    }
}
