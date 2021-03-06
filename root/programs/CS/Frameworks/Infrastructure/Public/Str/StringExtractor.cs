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
//* クラス名        ：StringExtractor
//* クラス日本語名  ：文字列からの抽出処理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/28  西野 大介         新規作成（分割
//**********************************************************************************

using System;
using System.Text;
using System.Text.RegularExpressions;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>文字列からの抽出処理クラス</summary>
    public class StringExtractor
    {
        /// <summary>QueryStringから指定のパラメタ値を抽出する</summary>
        /// <param name="paramName">string</param>
        /// <param name="rawUrl">string</param>
        /// <returns>指定のパラメタ値</returns>
        public static string GetParameterFromQueryString(string paramName, string rawUrl)
        {
            string retVal = "";
            string cmnPattern = paramName + "=";

            if (rawUrl.IndexOf(cmnPattern) != -1)
            {
                string regexPattern = "";

                // 先頭 or 中間
                regexPattern ="(" + cmnPattern + ")(?<" + paramName + ">.+?)(\\&)";
                retVal = Regex.Match(rawUrl, regexPattern).Groups[paramName].Value;

                // 末端
                if (string.IsNullOrEmpty(retVal))
                {
                    regexPattern = "(" + cmnPattern + ")(?<" + paramName + ">.+?)($)";
                    retVal = Regex.Match(rawUrl, regexPattern).Groups[paramName].Value;
                }
            }

            return retVal;
        }

        /// <summary>XML文字列から最初に現れた任意の属性の属性値を返す</summary>
        /// <param name="xml">string</param>
        /// <param name="attrName">string</param>
        /// <returns>Encoding</returns>
        public static string GetAttributeFromXml(string xml, string attrName)
        {
            string searchString = attrName + "=\"";
            int start = xml.IndexOf(searchString, 0) + searchString.Length;
            int end = xml.IndexOf('\"', start);
            return xml.Substring(start, end - start);
        }
    }
}
