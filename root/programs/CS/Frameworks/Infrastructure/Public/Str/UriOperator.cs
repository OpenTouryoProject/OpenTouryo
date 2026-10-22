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
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>URL の比較・判定</summary>
    public class UriOperator
    {
        /// <summary>クエリと素片を除いて、URL が同じか</summary>
        /// <param name="actual">比べる URL</param>
        /// <param name="expected">期待する URL</param>
        /// <returns>同じなら true（どちらかが絶対 URL でなければ false）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// DPoP の証明の htu の比較（RFC 9449 4.3）に使う。
        ///
        /// - クエリ（?）と素片（#）を落とす
        /// - 末尾の "/" を落とす（同じ口を指す書き方の違い）
        /// - 大文字小文字を区別しない（スキームとホストは RFC 3986 でも区別しない。
        ///   IIS / Kestrel はパスも区別しないので、区別すると実際に受けた要求を「違う」と言うことになる）
        /// - 既定のポートは省いて比べる（Uri の正規化に従う）
        /// </remarks>
        public static bool IsSameWithoutQuery(string actual, string expected)
        {
            if (string.IsNullOrEmpty(actual) || string.IsNullOrEmpty(expected))
            {
                return false;
            }

            if (!Uri.TryCreate(actual, UriKind.Absolute, out Uri a)
                || !Uri.TryCreate(expected, UriKind.Absolute, out Uri b))
            {
                return false;
            }

            return string.Equals(
                UriOperator.WithoutQuery(a), UriOperator.WithoutQuery(b),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>クエリと素片を除いた URL（末尾の "/" も落とす）</summary>
        /// <param name="uri">URL</param>
        /// <returns>文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string WithoutQuery(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
        }

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
    }
}
