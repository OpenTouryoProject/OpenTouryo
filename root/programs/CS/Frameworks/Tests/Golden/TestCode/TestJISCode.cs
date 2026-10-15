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
//* クラス名        ：TestJISCode
//* クラス日本語名  ：JISコード関連のテスト
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/07/31  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class JISCode
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            JIS2k4Checker jis2k4 = new JIS2k4Checker();
            string temp;
            int index;
            int s_length;
            int si_length;
            int byte_length;

            temp = "あああ";
            jis2k4.GetStringInfo(temp, out s_length, out si_length, out byte_length);
            MyDebug.OutputDebugAndConsole(
                "jis2k4.GetStringInfo - " + temp + ": \n"
                + "Char長:" + s_length.ToString() + "; \n"
                + "文字列長" + si_length.ToString() + "; \n"
                + "バイト長" + byte_length.ToString() + "; \n");

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckSurrogatesPairChar - " + temp + ": "
                + jis2k4.CheckSurrogatesPairChar(temp));
            
            temp = "あ𩸽あ";
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckSurrogatesPairChar - " + temp + ": "
                + jis2k4.CheckSurrogatesPairChar(temp));

            temp = "あああ";
            jis2k4.CheckSurrogatesPairChar(temp, out index);
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckSurrogatesPairChar - " + temp + ": "
                + index + "文字目");

            temp = "あ𩸽あ";
            jis2k4.CheckSurrogatesPairChar(temp, out index);
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckSurrogatesPairChar - " + temp + ": "
                + index + "文字目");

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "あ𩸽あ";
            MyDebug.OutputDebugAndConsole(
                "jis2k4.DeleteSurrogatesPairChar - " + temp + ": "
                + jis2k4.DeleteSurrogatesPairChar(temp));

            MyDebug.OutputDebugAndConsole(
                "jis2k4.DeleteSurrogatesPairChar - " + temp + ": "
                + jis2k4.DeleteSurrogatesPairChar(temp, '●'));

            MyDebug.OutputDebugAndConsole(
                "jis2k4.DeleteSurrogatesPairChar - " + temp + ": "
                + jis2k4.DeleteSurrogatesPairChar(temp, "●"));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckCharAddedWithJIS2k4 - " + temp + ": "
                + jis2k4.CheckCharAddedWithJIS2k4(temp));

            temp = "あ噓あ";
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckCharAddedWithJIS2k4 - " + temp + ": "
                + jis2k4.CheckCharAddedWithJIS2k4(temp));

            temp = "あああ";
            jis2k4.CheckCharAddedWithJIS2k4(temp, out index);
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckCharAddedWithJIS2k4 - " + temp + ": "
                + index + "文字目");

            temp = "あ噓あ";
            jis2k4.CheckCharAddedWithJIS2k4(temp, out index);
            MyDebug.OutputDebugAndConsole(
                "jis2k4.CheckCharAddedWithJIS2k4 - " + temp + ": "
                + index + "文字目");

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "あ噓あ";
            MyDebug.OutputDebugAndConsole(
                "jis2k4.DeleteCharAddedWithJIS2k4 - " + temp + ": "
                + jis2k4.DeleteCharAddedWithJIS2k4(temp));

            MyDebug.OutputDebugAndConsole(
                "jis2k4.DeleteCharAddedWithJIS2k4 - " + temp + ": "
                + jis2k4.DeleteCharAddedWithJIS2k4(temp, '●'));

            MyDebug.OutputDebugAndConsole(
                "jis2k4.DeleteCharAddedWithJIS2k4 - " + temp + ": "
                + jis2k4.DeleteCharAddedWithJIS2k4(temp, "●"));
        }
        #endregion
    }
}