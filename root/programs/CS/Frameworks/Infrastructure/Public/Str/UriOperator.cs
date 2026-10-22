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
//* クラス名        ：UriOperator
//* クラス日本語名  ：URL の比較・判定
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の E-2）
//*  2026/10/22  玄人 幸道         StringExtractor.GetParameterFromQueryString を移した（StringExtractor 側は転送して Obsolete）
//*  2026/10/22  玄人 幸道         DPoP の htu の比較（IsSameWithoutQuery）を DPoPProof へ移した（DPoP 専用で、汎用の部品ではないため）。
//*                                WithoutQuery は GetUrlWithoutQueryAndFragment に改名し、末尾の "/" を落とすのをやめた（htu の比較の側で行う）
//**********************************************************************************

using System;
using System.Text.RegularExpressions;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>URL の操作（判定・抽出）</summary>
    public class UriOperator
    {
        #region 判定

        /// <summary>https か、ループバックの http か</summary>
        /// <param name="url">URL</param>
        /// <returns>どちらかなら true</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 検証鍵（jwks_uri）やメタデータの取得先として使ってよい URL かの判定に使う。
        /// 平文の http では中身を差し替えられるので、https を要求する。
        /// 開発と試験のために、ループバック（localhost / 127.0.0.0/8 / [::1]）だけは http を許す
        /// （RFC 8252 7.3 が折り返し先で同じ扱いをしている）。
        /// </remarks>
        public static bool IsHttpsOrLoopback(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                return false;
            }

            if (uri.Scheme == Uri.UriSchemeHttps)
            {
                return true;
            }

            return uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback;
        }

        #endregion

        #region 抽出

        /// <summary>クエリ（?）と素片（#）を除いた URL を返す</summary>
        /// <param name="url">URL</param>
        /// <returns>クエリと素片を除いた URL（絶対 URL でなければ空文字）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// スキーム・ホスト・ポート・パスを残す（Uri.GetLeftPart(UriPartial.Path)）。
        /// 既定のポートは省かれ、スキームとホストは小文字になる（Uri の正規化に従う）。
        /// 末尾の "/" は落とさない。
        /// </remarks>
        public static string GetUrlWithoutQueryAndFragment(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                return "";
            }

            return uri.GetLeftPart(UriPartial.Path);
        }

        /// <summary>QueryStringから指定のパラメタ値を抽出する</summary>
        /// <param name="paramName">string</param>
        /// <param name="rawUrl">string</param>
        /// <returns>指定のパラメタ値</returns>
        /// <remarks>
        /// 自由に利用できる（StringExtractor から移した）。
        /// 値は URL デコードしない。「値が空」と「名前が無い」は区別できない（どちらも空文字）。
        /// </remarks>
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

        #endregion
    }
}
