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
//**********************************************************************************

using System.Diagnostics;

namespace Touryo.Infrastructure.Public.Diagnostics
{
    /// <summary>配列操作クラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class StackFrameOperator
    {
        /// <summary>カンレントMethod名を取得する。</summary>
        /// <returns>カンレントMethod名</returns>
        public static string GetCurrentMethodName()
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        /// <summary>カンレントProperty名を取得する。</summary>
        /// <returns>カンレントProperty名</returns>
        public static string GetCurrentPropertyName()
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);
            string currentMethodName = sf.GetMethod().Name;

            // Property名に変換
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

        /// <summary>カンレント・コード情報を取得する。</summary>
        /// <param name="filePath">ファイル・パス</param>
        /// <param name="fileLineNumber">ファイル行数</param>
        /// <param name="methodSignature">メソッド・シグネチャ</param>
        public static void GetCurrentCodeInfo(out string filePath, out string fileLineNumber, out string methodSignature)
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);

            // ファイル・パス
            filePath = sf.GetFileName();
            // ファイル行数
            fileLineNumber = sf.GetFileLineNumber().ToString();
            // メソッド・シグネチャ
            methodSignature = sf.GetMethod().ToString();
        }
    }
}
