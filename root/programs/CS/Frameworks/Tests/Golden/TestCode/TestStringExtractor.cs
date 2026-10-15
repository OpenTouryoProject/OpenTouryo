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
//* クラス名        ：TestStringExtractor
//* クラス日本語名  ：StringExtractor・ToStringExtensionのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#522）
//**********************************************************************************

using System;
using System.Xml;

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Str;

namespace TestCode
{
    /// <summary>StringExtractor・ToStringExtensionのテスト</summary>
    /// <remarks>
    /// 文字列からの抽出（クエリ文字列・XML 属性）と、XmlDocument の文字列化（#522）。
    /// </remarks>
    public class TestStringExtractor
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestStringExtractor.TestGetParameterFromQueryString();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestStringExtractor.TestGetAttributeFromXml();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestStringExtractor.TestXmlToString();
        }

        #endregion

        #region private

        /// <summary>クエリ文字列からの抽出</summary>
        private static void TestGetParameterFromQueryString()
        {
            MyDebug.OutputDebugAndConsole("StringExtractor.GetParameterFromQueryString");

            const string url = "/path/page?a=1&b=xyz&c=";

            TestStringExtractor.OutputQuery("先頭の値", "a", url);
            TestStringExtractor.OutputQuery("途中の値", "b", url);
            TestStringExtractor.OutputQuery("空の値", "c", url);
            TestStringExtractor.OutputQuery("無い名前", "z", url);

            // クエリ文字列が無い場合
            TestStringExtractor.OutputQuery("クエリ無し", "a", "/path/page");

            // 名前の一部が他の名前に含まれる場合
            // （"a" が "ab" に前方一致してしまわないこと）
            TestStringExtractor.OutputQuery("前方一致の紛れ", "a", "/p?ab=9&a=1");
        }

        /// <summary>XML の属性値の抽出</summary>
        /// <remarks>
        /// XML パーサを通さない文字列処理。整形式でなくても動く代わりに、
        /// **同名の属性が複数あると先頭が取れる**などの癖がある。
        /// </remarks>
        private static void TestGetAttributeFromXml()
        {
            MyDebug.OutputDebugAndConsole("StringExtractor.GetAttributeFromXml");

            const string xml = "<root id=\"1\" name=\"abc\"><child id=\"2\" /></root>";

            TestStringExtractor.OutputAttr("最初の属性", xml, "id");
            TestStringExtractor.OutputAttr("2 番目の属性", xml, "name");
            TestStringExtractor.OutputAttr("無い属性", xml, "notexist");
        }

        /// <summary>XmlDocument の文字列化</summary>
        /// <remarks>
        /// **XmlWriterSettings を明示する。** 既定のままだと改行や宣言の有無が
        /// 実装に依存し、期待結果と一致しなくなる。
        /// </remarks>
        private static void TestXmlToString()
        {
            MyDebug.OutputDebugAndConsole("ToStringExtension.XmlToString");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><child id=\"1\">値</child></root>");

            // 宣言なし・インデントなしで、1 行に固定する。
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = false;

            MyDebug.OutputDebugAndConsole("宣言なし : " + doc.XmlToString(settings));

            // インデントあり（改行が入る）
            XmlWriterSettings indented = new XmlWriterSettings();
            indented.OmitXmlDeclaration = true;
            indented.Indent = true;
            indented.IndentChars = "  ";
            indented.NewLineChars = "\n";

            MyDebug.OutputDebugAndConsole("インデントあり :");
            MyDebug.OutputDebugAndConsole(doc.XmlToString(indented));
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>クエリ文字列の抽出結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="paramName">パラメタ名</param>
        /// <param name="rawUrl">URL</param>
        private static void OutputQuery(string caseName, string paramName, string rawUrl)
        {
            try
            {
                string ret = StringExtractor.GetParameterFromQueryString(paramName, rawUrl);

                MyDebug.OutputDebugAndConsole(
                    "[" + caseName + "] " + paramName + " → "
                    + (ret == null ? "(null)" : "\"" + ret + "\""));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole("[" + caseName + "] 例外 : " + ex.GetType().FullName);
            }
        }

        /// <summary>XML 属性の抽出結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="xml">XML</param>
        /// <param name="attrName">属性名</param>
        private static void OutputAttr(string caseName, string xml, string attrName)
        {
            try
            {
                string ret = StringExtractor.GetAttributeFromXml(xml, attrName);

                MyDebug.OutputDebugAndConsole(
                    "[" + caseName + "] " + attrName + " → "
                    + (ret == null ? "(null)" : "\"" + ret + "\""));
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole("[" + caseName + "] 例外 : " + ex.GetType().FullName);
            }
        }

        #endregion
    }
}
