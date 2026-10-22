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
//* クラス名        ：LogSanitizer
//* クラス日本語名  ：ログに出す値の衛生
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の E-3）
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>ログに出す値の衛生</summary>
    /// <remarks>
    /// 相手から来た値をログに出すときに使う。
    ///
    /// | 口 | 出すもの | 使う場面 |
    /// |---|---|---|
    /// | Sanitize | 改行を落とし、長さを切った値 | エラー コードなど、値そのものを見たいもの |
    /// | Describe | 空か・長さだけ | state / nonce / トークンなど、値を出してはならないもの |
    /// </remarks>
    public class LogSanitizer
    {
        /// <summary>空のときに出す文字列</summary>
        private const string Empty = "(empty)";

        /// <summary>切ったときに末尾に付ける文字列</summary>
        private const string Ellipsis = "...";

        /// <summary>Sanitize の既定の長さ</summary>
        public const int DefaultMaxLength = 64;

        /// <summary>ログに出してよい形にする（長さは既定の 64）</summary>
        /// <param name="value">相手から来た短い値</param>
        /// <returns>改行を落とし、長さを切った値（空なら "(empty)"）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string Sanitize(string value)
        {
            return LogSanitizer.Sanitize(value, LogSanitizer.DefaultMaxLength);
        }

        /// <summary>ログに出してよい形にする</summary>
        /// <param name="value">相手から来た短い値</param>
        /// <param name="maxLength">残す長さ（これを超えたら切って "..." を付ける）</param>
        /// <returns>改行を落とし、長さを切った値（空なら "(empty)"）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 改行（CR / LF）を空白にする。行を割られると、ログの 1 行 1 件が崩れ、
        /// 偽の行を差し込まれる（ログ インジェクション）。
        /// </remarks>
        public static string Sanitize(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return LogSanitizer.Empty;
            }

            string ret = value.Replace('\r', ' ').Replace('\n', ' ');

            if (0 <= maxLength && maxLength < ret.Length)
            {
                ret = ret.Substring(0, maxLength) + LogSanitizer.Ellipsis;
            }

            return ret;
        }

        /// <summary>値そのものを出さずに、有無と長さだけを表す</summary>
        /// <param name="value">値</param>
        /// <returns>"(empty)" または "len=長さ"</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 切り分けに要るのは「空か・長さが違うか」までで、値そのものではないものに使う
        /// （state / nonce / コード / トークンなど）。
        /// </remarks>
        public static string Describe(string value)
        {
            return string.IsNullOrEmpty(value) ? LogSanitizer.Empty : ("len=" + value.Length);
        }
    }
}
