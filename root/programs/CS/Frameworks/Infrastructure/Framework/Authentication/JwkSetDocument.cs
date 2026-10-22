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
//* クラス名        ：JwkSetDocument
//* クラス日本語名  ：自分の JWK Set を出す（jwks_uri の本文と ETag）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>自分の JWK Set を出す（jwks_uri の本文と ETag）</summary>
    /// <remarks>
    /// 相手の JWK Set を読む RemoteJwkSetStore の対である（認可サーバ側。汎用認証サイトから移した。#634）。
    ///
    /// | | |
    /// |---|---|
    /// | 本文 | JWK Set のファイルの内容 |
    /// | ETag | 中身の SHA-256（BASE64URL。引用符を含む。同じ中身なら同じ値。弱い ETag にはしない） |
    ///
    /// **ファイルが変わっていなければ、覚えている内容を返す。変わっていれば読み直す**
    /// （起動時に 1 度だけ読む形にすると、鍵の入れ替えに追随できない）。
    /// 変わったかどうかは、ファイルの最終更新時刻と長さで見る（中身は読まない）。
    /// パスにファイルが無ければ覚えない（ResourceLoader が埋め込みリソースなど別の場所から解決することがあるため。毎回読む）。
    ///
    /// Cache-Control の秒数・If-None-Match への 304 など、応答の返し方は、呼び出し側で決める。
    /// </remarks>
    public class JwkSetDocument
    {
        #region 覚えている分

        /// <summary>覚えている 1 件</summary>
        private class Entry
        {
            /// <summary>刻印（最終更新時刻 ＋ 長さ）</summary>
            public string Stamp { get; set; }

            /// <summary>本文</summary>
            public string Content { get; set; }

            /// <summary>ETag</summary>
            public string ETag { get; set; }
        }

        /// <summary>覚えている分（鍵はパス）</summary>
        private static readonly Dictionary<string, Entry> _cache = new Dictionary<string, Entry>();

        /// <summary>_cache の排他</summary>
        private static readonly object _lock = new object();

        #endregion

        #region 設定を読む版

        /// <summary>設定の JwkSetFilePath の JWK Set の本文と ETag を返す</summary>
        /// <param name="etag">out string（ETag。"..." の形）</param>
        /// <returns>JWK Set の本文（JSON）</returns>
        /// <remarks>自由に利用できる。Get(jwkSetFilePath, out etag) に OAuth2AndOIDCParams.JwkSetFilePath を渡す。</remarks>
        public static string Get(out string etag)
        {
            return JwkSetDocument.Get(OAuth2AndOIDCParams.JwkSetFilePath, out etag);
        }

        #endregion

        #region 引数（パス）で受ける版

        /// <summary>JWK Set の本文と ETag を返す</summary>
        /// <param name="jwkSetFilePath">string（JWK Set のファイルのパス。null・空は受けない）</param>
        /// <param name="etag">out string（ETag。"..." の形）</param>
        /// <returns>JWK Set の本文（JSON）</returns>
        /// <remarks>自由に利用できる。読めなければ例外になる（設定の誤りを黙って通さない）。</remarks>
        public static string Get(string jwkSetFilePath, out string etag)
        {
            if (string.IsNullOrEmpty(jwkSetFilePath))
            {
                throw new ArgumentNullException(nameof(jwkSetFilePath));
            }

            string stamp = JwkSetDocument.StampOf(jwkSetFilePath);

            if (stamp != null)
            {
                lock (JwkSetDocument._lock)
                {
                    if (JwkSetDocument._cache.TryGetValue(jwkSetFilePath, out Entry entry) && entry.Stamp == stamp)
                    {
                        etag = entry.ETag;
                        return entry.Content;
                    }
                }
            }

            string content = ResourceLoader.LoadAsString(jwkSetFilePath, Encoding.GetEncoding(CustomEncode.UTF_8));
            etag = JwkSetDocument.ETagOf(content);

            if (stamp != null)
            {
                lock (JwkSetDocument._lock)
                {
                    JwkSetDocument._cache[jwkSetFilePath] = new Entry() { Stamp = stamp, Content = content, ETag = etag };
                }
            }

            return content;
        }

        #endregion

        #region private

        /// <summary>ファイルの刻印（変わったかどうかだけを見る）</summary>
        /// <param name="path">string</param>
        /// <returns>刻印（ファイルが無ければ null ＝ 覚えない）</returns>
        private static string StampOf(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                FileInfo info = new FileInfo(path);
                return info.LastWriteTimeUtc.Ticks.ToString() + "|" + info.Length.ToString();
            }
            catch (Exception)
            {
                // 刻印が取れないときは、覚えない（毎回読む）。
                return null;
            }
        }

        /// <summary>中身から ETag を作る</summary>
        /// <param name="content">string</param>
        /// <returns>ETag（"..." の形）</returns>
        private static string ETagOf(string content)
        {
            return "\"" + CustomEncode.ToBase64UrlString(GetHash.GetHashBytes(
                CustomEncode.StringToByte(content ?? "", CustomEncode.UTF_8), EnumHashAlgorithm.SHA256)) + "\"";
        }

        #endregion
    }
}
