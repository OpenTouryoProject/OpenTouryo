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
//* クラス名        ：TestCustomEncode
//* クラス日本語名  ：CustomEncodeのテスト
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/07/31  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Data;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestCustomEncode
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.HtmlEncode: "
                + CustomEncode.HtmlEncode(
                    "\" id=\"txtXXXXX\" />"
                    + "<script type=\"text/javascript\">alert(\"XSS!!!\")</script>"
                    + "<input name=\"txtXXXXX\" type=\"text\" value=\""));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.UrlEncode: "
                + "http://www.google.co.jp/search?hl=ja&q=" + CustomEncode.UrlEncode("&"));

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.UrlEncode2: "
                + CustomEncode.UrlEncode2("http://www.google.co.jp/search?hl=ja&q=&"));

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.UrlEncode2: "
                + CustomEncode.UrlEncode2("http://www.google.co.jp/search?hl=ja&q=<>"));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestCustomEncode.Decode();
            TestCustomEncode.StringAndByte();
            TestCustomEncode.Hex();
            TestCustomEncode.Base64();
            TestCustomEncode.Base64Url();
            TestCustomEncode.Encodings();
        }
        #endregion

        #region private

        /// <summary>デコード（HtmlDecode、UrlDecode）</summary>
        private static void Decode()
        {
            // エンコードしたものを戻せるか（往復）
            string html = CustomEncode.HtmlEncode("<a href=\"x\">&amp;</a>");
            MyDebug.OutputDebugAndConsole("CustomEncode.HtmlDecode: " + CustomEncode.HtmlDecode(html));

            // 実体参照を直接
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.HtmlDecode: " + CustomEncode.HtmlDecode("&lt;p&gt;&quot;&#39;&amp;&nbsp;"));

            string url = CustomEncode.UrlEncode("あ&=?/ +");
            MyDebug.OutputDebugAndConsole("CustomEncode.UrlEncode: " + url);
            MyDebug.OutputDebugAndConsole("CustomEncode.UrlDecode: " + CustomEncode.UrlDecode(url));

            // **「+」は空白に戻る。** UrlEncode は空白を「+」にするため、
            // 元から「+」だった文字と区別が付かない。
            MyDebug.OutputDebugAndConsole("CustomEncode.UrlDecode: [" + CustomEncode.UrlDecode("a+b%20c") + "]");
        }

        /// <summary>文字列とバイト配列（StringToByte、ByteToString）</summary>
        private static void StringAndByte()
        {
            string str = "あいうAB1";

            foreach (int cp in new int[] { CustomEncode.UTF_8, CustomEncode.shift_jis, CustomEncode.EUC_JP })
            {
                byte[] bytes = CustomEncode.StringToByte(str, cp);

                MyDebug.OutputDebugAndConsole(
                    "CustomEncode.StringToByte(" + cp.ToString() + "): " + CustomEncode.ToHexString(bytes));

                MyDebug.OutputDebugAndConsole(
                    "CustomEncode.ByteToString(" + cp.ToString() + "): " + CustomEncode.ByteToString(bytes, cp));
            }

            // 空文字列
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.StringToByte(empty): ["
                + CustomEncode.ToHexString(CustomEncode.StringToByte("", CustomEncode.UTF_8)) + "]");

            // **コードページが違うと戻らない。** shift_jis のバイト列を UTF-8 で読む。
            byte[] sjis = CustomEncode.StringToByte("あ", CustomEncode.shift_jis);
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.ByteToString(sjis as utf8): "
                + CustomEncode.ToHexString(CustomEncode.StringToByte(
                    CustomEncode.ByteToString(sjis, CustomEncode.UTF_8), CustomEncode.UTF_8)));
        }

        /// <summary>Hex（ToHexString、FormHexString）</summary>
        private static void Hex()
        {
            byte[] bytes = new byte[] { 0x00, 0x0F, 0x10, 0xFF };

            string hex = CustomEncode.ToHexString(bytes);
            MyDebug.OutputDebugAndConsole("CustomEncode.ToHexString: [" + hex + "]");

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.FormHexString: [" + CustomEncode.ToHexString(CustomEncode.FormHexString(hex)) + "]");

            // **空のバイト配列。** 区切りの空白を削る分岐（0 < ret.Length）を通らない。
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.ToHexString(empty): [" + CustomEncode.ToHexString(new byte[0]) + "]");

            // 小文字の Hex も読めるか
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.FormHexString(lower): [" + CustomEncode.ToHexString(CustomEncode.FormHexString("0a bc")) + "]");

            // Hex として読めない
            TestCustomEncode.ShowException("CustomEncode.FormHexString(invalid)",
                delegate { CustomEncode.FormHexString("ZZ"); });
        }

        /// <summary>Base64（ToBase64String、FromBase64String）</summary>
        private static void Base64()
        {
            byte[] bytes = CustomEncode.StringToByte("こんにちは", CustomEncode.UTF_8);

            string b64 = CustomEncode.ToBase64String(bytes);
            MyDebug.OutputDebugAndConsole("CustomEncode.ToBase64String: " + b64);

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.FromBase64String: "
                + CustomEncode.ByteToString(CustomEncode.FromBase64String(b64), CustomEncode.UTF_8));

            // 空のバイト配列
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.ToBase64String(empty): [" + CustomEncode.ToBase64String(new byte[0]) + "]");

            // Base64 として読めない
            TestCustomEncode.ShowException("CustomEncode.FromBase64String(invalid)",
                delegate { CustomEncode.FromBase64String("!!!!"); });
        }

        /// <summary>Base64Url（ToBase64UrlString、FromBase64UrlString）</summary>
        private static void Base64Url()
        {
            // **「+」と「/」が出る入力を選ぶ。** そうでないと置換の分岐を通らない。
            //   FF EF BF → 標準 Base64 では "/++/" になる。
            byte[] plus = new byte[] { 0xFF, 0xEF, 0xBF };
            MyDebug.OutputDebugAndConsole("CustomEncode.ToBase64String   : " + CustomEncode.ToBase64String(plus));
            MyDebug.OutputDebugAndConsole("CustomEncode.ToBase64UrlString: " + CustomEncode.ToBase64UrlString(plus));

            // **パディングの 3 分岐（余り 0 / 2 / 3）を通す。**
            //   3 バイト → 余り 0、1 バイト → 余り 2、2 バイト → 余り 3
            for (int len = 1; len <= 3; len++)
            {
                byte[] bytes = new byte[len];
                for (int i = 0; i < len; i++) { bytes[i] = (byte)(0xFF - i); }

                string b64url = CustomEncode.ToBase64UrlString(bytes);

                MyDebug.OutputDebugAndConsole(
                    "CustomEncode.ToBase64UrlString(" + len.ToString() + "byte): [" + b64url + "]"
                    + " 長さ%4=" + (b64url.Length % 4).ToString());

                MyDebug.OutputDebugAndConsole(
                    "CustomEncode.FromBase64UrlString(" + len.ToString() + "byte): ["
                    + CustomEncode.ToHexString(CustomEncode.FromBase64UrlString(b64url)) + "]");
            }

            // **余り 1 は不正。** default 分岐（Illegal base64url string!）を通す。
            TestCustomEncode.ShowException("CustomEncode.FromBase64UrlString(invalid)",
                delegate { CustomEncode.FromBase64UrlString("AAAAA"); }, true);
        }

        /// <summary>エンコーディングの一覧（GetEncodings）</summary>
        private static void Encodings()
        {
            DataTable dt = CustomEncode.GetEncodings();

            MyDebug.OutputDebugAndConsole("CustomEncode.GetEncodings: 行数 " + dt.Rows.Count.ToString());
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.GetEncodings: 列 " + dt.Columns[0].ColumnName + ", " + dt.Columns[1].ColumnName);

            // 先頭と末尾（中身が環境で変わらないことの確認も兼ねる）
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.GetEncodings[0]: " + dt.Rows[0]["key"].ToString() + " = " + dt.Rows[0]["value"].ToString());

            DataRow last = dt.Rows[dt.Rows.Count - 1];
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.GetEncodings[last]: " + last["key"].ToString() + " = " + last["value"].ToString());
        }

        /// <summary>例外を捕まえて出力する</summary>
        /// <param name="title">表題</param>
        /// <param name="action">実行する処理</param>
        /// <param name="showMessage">メッセージも出力するか</param>
        /// <remarks>
        /// **既定では型名しか出さない。**
        /// フレームワークが投げる例外のメッセージは地域化されるため、
        /// 環境によって出力が変わってしまう（結果は Result*.txt と突き合わせる）。
        /// Open棟梁が自前で投げているものだけ、メッセージも出す。
        /// </remarks>
        private static void ShowException(string title, Action action, bool showMessage = false)
        {
            try
            {
                action();
                MyDebug.OutputDebugAndConsole(title + ": 例外なし");
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole(
                    title + ": " + ex.GetType().Name + (showMessage ? " : " + ex.Message : ""));
            }
        }

        #endregion
    }
}