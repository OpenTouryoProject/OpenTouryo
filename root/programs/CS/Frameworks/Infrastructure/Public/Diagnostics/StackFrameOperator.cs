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
//* クラス名        ：StackFrameOperator
//* クラス日本語名  ：StackFrame操作クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/28  西野 大介         新規作成（分割
//*  2026/10/16  玄人 幸道         転送用に「呼び出し元の、さらに呼び出し元」を返す internal メソッドを追加
//*                                （PubCmnFunction の Obsolete メソッドが、転送元自身を返していたため。#616）
//*                                公開の 3 メソッドに NoInlining を付けた（インライン展開でフレームがずれないように）
//**********************************************************************************

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Touryo.Infrastructure.Public.Diagnostics
{
    /// <summary>配列操作クラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class StackFrameOperator
    {
        /// <summary>カンレントMethod名を取得する。</summary>
        /// <returns>カンレントMethod名</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodName()
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        /// <summary>カンレントProperty名を取得する。</summary>
        /// <returns>カンレントProperty名</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentPropertyName()
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);

            // Property名に変換
            return StackFrameOperator.ToPropertyName(sf.GetMethod().Name);
        }

        /// <summary>カンレント・コード情報を取得する。</summary>
        /// <param name="filePath">ファイル・パス</param>
        /// <param name="fileLineNumber">ファイル行数</param>
        /// <param name="methodSignature">メソッド・シグネチャ</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GetCurrentCodeInfo(out string filePath, out string fileLineNumber, out string methodSignature)
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);

            StackFrameOperator.ToCodeInfo(sf, out filePath, out fileLineNumber, out methodSignature);
        }

        #region 呼び出し元の、さらに呼び出し元（転送用）

        // ＜なぜ要るか＞（PubCmnFunction の Obsolete メソッド）
        //   上の 3 つは「1 つ上の呼び出し元」を返す。これを別のメソッドから転送すると、
        //   1 つ上は転送したメソッド自身になり、本来の呼び出し元が分からない。
        //   転送する側は、こちらを呼ぶ（「2 つ上」を返す）。
        //
        // ＜NoInlining＞（上の 3 つも同じ）
        //   JIT がインライン展開すると、フレームが 1 つ減って別のメソッドを指す。
        //   転送する側にも同じ指定が要る。

        /// <summary>呼び出し元の、さらに呼び出し元の Method 名を取得する。</summary>
        /// <returns>Method 名</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static string GetCallerMethodName()
        {
            return new StackTrace(true).GetFrame(2).GetMethod().Name;
        }

        /// <summary>呼び出し元の、さらに呼び出し元の Property 名を取得する。</summary>
        /// <returns>Property 名（Property でなければ空文字）</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static string GetCallerPropertyName()
        {
            return StackFrameOperator.ToPropertyName(new StackTrace(true).GetFrame(2).GetMethod().Name);
        }

        /// <summary>呼び出し元の、さらに呼び出し元のコード情報を取得する。</summary>
        /// <param name="filePath">ファイル・パス</param>
        /// <param name="fileLineNumber">ファイル行数</param>
        /// <param name="methodSignature">メソッド・シグネチャ</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void GetCallerCodeInfo(out string filePath, out string fileLineNumber, out string methodSignature)
        {
            StackFrameOperator.ToCodeInfo(
                new StackTrace(true).GetFrame(2), out filePath, out fileLineNumber, out methodSignature);
        }

        #endregion

        #region private

        /// <summary>Method 名を Property 名に変換する（get_ / set_ を削る）。</summary>
        /// <param name="currentMethodName">Method 名</param>
        /// <returns>Property 名（Property でなければ空文字）</returns>
        private static string ToPropertyName(string currentMethodName)
        {
            if (string.IsNullOrEmpty(currentMethodName))
            {
                return "";
            }
            else
            {
                if (currentMethodName.IndexOf("get_") == 0 ||
                    currentMethodName.IndexOf("set_") == 0)
                {
                    // 先頭のget_、set_がある場合、これを削る。
                    return currentMethodName.Substring(4);
                }
                else
                {
                    // Propertyではない。
                    return "";
                }
            }
        }

        /// <summary>StackFrame からコード情報を取り出す。</summary>
        /// <param name="sf">StackFrame</param>
        /// <param name="filePath">ファイル・パス</param>
        /// <param name="fileLineNumber">ファイル行数</param>
        /// <param name="methodSignature">メソッド・シグネチャ</param>
        private static void ToCodeInfo(StackFrame sf, out string filePath, out string fileLineNumber, out string methodSignature)
        {
            // ファイル・パス
            filePath = sf.GetFileName();
            // ファイル行数
            fileLineNumber = sf.GetFileLineNumber().ToString();
            // メソッド・シグネチャ
            methodSignature = sf.GetMethod().ToString();
        }

        #endregion
    }
}
