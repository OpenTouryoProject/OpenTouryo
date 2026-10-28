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
//* クラス名        ：SQLSanitizer
//* クラス日本語名  ：SQL のリテラル・識別子のサニタイジング
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/28  玄人 幸道         新規作成（SQLUtility の文字列リテラルを DBMS ごとに正しく作るため）
//**********************************************************************************

using System;
using System.Text;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Db
{
    /// <summary>SQL のリテラル・識別子のサニタイジング</summary>
    /// <remarks>
    /// 値を SQL 文に直接埋め込むときに、DBMS ごとの規則で文字列リテラルと識別子を作る。
    /// **値はパラメタで渡すのが原則**で、これはパラメタが使えない場合
    /// （SQLUtility による一括 INSERT / UPDATE の生成など）のための部品である。
    ///
    /// ＜文字列リテラル＞
    ///   SQL Server / Oracle / DB2 … ' を '' にして ' で囲う（\ は特別な文字ではない）。
    ///   PostgreSQL … E'～' にして、\ を \\ に、' を '' にする。
    ///                 E'～' は standard_conforming_strings の設定によらず \ をエスケープ文字として扱うので、
    ///                 設定が on でも off でも同じ値になる。
    ///   MySQL      … 16 進リテラル（_utf8mb4 0x～）にする。
    ///                 MySQL は既定（sql_mode に NO_BACKSLASH_ESCAPES が無い）で \ をエスケープ文字として扱い、
    ///                 ' を '' にするだけでは \ を含む値でリテラルが正しく閉じない。
    ///                 16 進リテラルは引用符もエスケープも使わないので、sql_mode によらず同じ値になる。
    ///
    /// ＜識別子＞
    ///   囲い文字（SQLUtility.OpeningBracket / ClosingBracket と同じ）で囲い、名前に含まれる閉じ側の囲い文字を二重にする。
    ///
    /// 自由に利用できる。
    /// </remarks>
    public class SQLSanitizer
    {
        /// <summary>文字列を、SQL の文字列リテラルにする。</summary>
        /// <param name="value">文字列（空文字可）</param>
        /// <param name="dbms">DBMSの種類</param>
        /// <returns>文字列リテラル（例：SQL Server なら 'it''s'）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string ToStringLiteral(string value, DbEnum.DBMSType dbms)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            switch (dbms)
            {
                case DbEnum.DBMSType.SQLServer:
                case DbEnum.DBMSType.Oracle:
                case DbEnum.DBMSType.DB2:
                    return "'" + value.Replace("'", "''") + "'";

                case DbEnum.DBMSType.PstGrS:
                    return "E'" + value.Replace("\\", "\\\\").Replace("'", "''") + "'";

                case DbEnum.DBMSType.MySQL:
                    if (value.Length == 0)
                    {
                        // 0x だけの 16 進リテラルは書けない。空文字は引用符で書いても \ を含まないので安全。
                        return "''";
                    }
                    // 一括 INSERT では行数 × 列数だけ呼ばれるので、文字列の連結を繰り返す
                    // CustomEncode.ToHexString ではなく BitConverter を使う。
                    return "_utf8mb4 0x"
                        + BitConverter.ToString(Encoding.UTF8.GetBytes(value)).Replace("-", "");

                default:
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        /// <summary>名前を、SQL の識別子（囲い文字で囲ったもの）にする。</summary>
        /// <param name="name">表名・列名などの名前（1 つ。schema.table のような区切りは含めない）</param>
        /// <param name="dbms">DBMSの種類</param>
        /// <returns>識別子（例：SQL Server なら [Order Details]）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string QuoteIdentifier(string name, DbEnum.DBMSType dbms)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            switch (dbms)
            {
                case DbEnum.DBMSType.SQLServer:
                    return "[" + name.Replace("]", "]]") + "]";

                case DbEnum.DBMSType.Oracle:
                case DbEnum.DBMSType.DB2:
                case DbEnum.DBMSType.HiRDB:
                case DbEnum.DBMSType.PstGrS:
                    return "\"" + name.Replace("\"", "\"\"") + "\"";

                case DbEnum.DBMSType.MySQL:
                    return "`" + name.Replace("`", "``") + "`";

                default:
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }
    }
}
