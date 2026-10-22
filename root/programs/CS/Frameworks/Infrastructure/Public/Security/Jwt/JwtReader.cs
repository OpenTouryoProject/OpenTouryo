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
//* クラス名        ：JwtReader
//* クラス日本語名  ：JWT を検証せずに読む
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の E-1 / G-7）
//**********************************************************************************

using System;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWT を検証せずに読む</summary>
    /// <remarks>
    /// kid や alg を見てから検証鍵を選ぶので、**検証の前に**ヘッダやペイロードを読む必要がある。
    /// 外から来た値を扱うため、**読めなければ null を返し、例外にしない。**
    ///
    /// **ここで読んだ値を、検証の代わりにしてはならない。**
    /// 鍵を選ぶ・理由を返すなど、検証の前の判断にだけ使う。
    /// </remarks>
    public class JwtReader
    {
        /// <summary>ヘッダを読む（JWS / JWE のコンパクト形式）</summary>
        /// <param name="jwt">JWS（3 区切り）または JWE（5 区切り）</param>
        /// <returns>ヘッダ（読めなければ null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static JObject TryReadHeader(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                return null;
            }

            string[] segments = jwt.Split('.');

            if (segments.Length != 3 && segments.Length != 5)
            {
                return null;
            }

            return JwtReader.TryReadSegment(segments[0]);
        }

        /// <summary>ペイロードを読む（JWS のコンパクト形式）</summary>
        /// <param name="jwt">JWS（3 区切り）</param>
        /// <returns>ペイロード（読めなければ null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// JWE のペイロードは暗号化されているので読めない（null を返す）。
        /// JObject で返すのは、入れ子のクレーム（cnf など）を Dictionary では受けられないため。
        /// </remarks>
        public static JObject TryReadPayload(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                return null;
            }

            string[] segments = jwt.Split('.');

            if (segments.Length != 3)
            {
                return null;
            }

            return JwtReader.TryReadSegment(segments[1]);
        }

        /// <summary>ヘッダの alg を読む</summary>
        /// <param name="jwt">JWS / JWE</param>
        /// <returns>alg（読めなければ null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string TryReadAlg(string jwt)
        {
            return JwtReader.GetString(JwtReader.TryReadHeader(jwt), JwtConst.alg);
        }

        /// <summary>ヘッダの kid を読む</summary>
        /// <param name="jwt">JWS / JWE</param>
        /// <returns>kid（読めなければ null）</returns>
        /// <remarks>自由に利用できる。kid は任意（RFC 7515 4.1.4）なので、無いことも多い。</remarks>
        public static string TryReadKid(string jwt)
        {
            return JwtReader.GetString(JwtReader.TryReadHeader(jwt), JwtConst.kid);
        }

        /// <summary>ヘッダの typ を読む</summary>
        /// <param name="jwt">JWS / JWE</param>
        /// <returns>typ（読めなければ null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 照合には JWS.Verify(jwt, typ) を使うこと（大文字小文字・"application/" の扱いがある）。
        /// </remarks>
        public static string TryReadTyp(string jwt)
        {
            return JwtReader.GetString(JwtReader.TryReadHeader(jwt), JwtConst.typ);
        }

        #region private

        /// <summary>BASE64URL の JSON を読む</summary>
        /// <param name="segment">区切りの 1 つ</param>
        /// <returns>JObject（オブジェクトでなければ null）</returns>
        private static JObject TryReadSegment(string segment)
        {
            if (string.IsNullOrEmpty(segment))
            {
                return null;
            }

            try
            {
                return JToken.Parse(CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(segment), CustomEncode.UTF_8)) as JObject;
            }
            catch (Exception)
            {
                // BASE64URL でない・JSON でない（外から来た値なので、例外にしない）。
                return null;
            }
        }

        /// <summary>文字列の項目を読む（文字列でなければ null）</summary>
        /// <param name="obj">JObject（null 可）</param>
        /// <param name="name">項目名</param>
        /// <returns>値</returns>
        private static string GetString(JObject obj, string name)
        {
            if (obj == null)
            {
                return null;
            }

            JToken token = obj[name];
            return (token != null && token.Type == JTokenType.String) ? (string)token : null;
        }

        #endregion
    }
}
