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
//* クラス名        ：JwkOperator
//* クラス日本語名  ：JWK の操作（RFC 7638 の指紋・公開鍵だけか）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の A-4）
//*  2026/10/22  玄人 幸道         指紋の JSON のキー名の直書きを JwtConst に寄せた
//**********************************************************************************

using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWK の操作（RFC 7638 の指紋・公開鍵だけか）</summary>
    /// <remarks>
    /// 指紋は *KeyConverter も kid を作るときに計算しているが、
    /// 自分の JWK を作るときだけで、「この JWK の指紋を取る」口が無かった（#634）。
    /// </remarks>
    public class JwkOperator
    {
        /// <summary>秘密鍵の項目（RFC 7518 6.2.2 / 6.3.2 / 6.4.1）</summary>
        private static readonly string[] PrivateMembers = new string[]
        {
            JwtConst.d, JwtConst.p, JwtConst.q, JwtConst.dp, JwtConst.dq, JwtConst.qi, JwtConst.oth, JwtConst.k
        };

        #region Thumbprint

        /// <summary>JWK の指紋（RFC 7638。ハッシュは SHA-256）</summary>
        /// <param name="jwk">JWK</param>
        /// <returns>BASE64URL(SHA-256(正規化した JSON))。作れなければ空文字</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// DPoP の jkt（RFC 9449 6）は、常にこの SHA-256 の値である。
        /// *KeyConverter が作る kid は、変換器のハッシュに連動する（RS384 なら SHA-384）ので、
        /// kid をそのまま jkt として使ってはならない（照合が黙って落ちる）。
        /// </remarks>
        public static string Thumbprint(JObject jwk)
        {
            return JwkOperator.Thumbprint(jwk, EnumHashAlgorithm.SHA256);
        }

        /// <summary>JWK の指紋（RFC 7638。ハッシュを指定する）</summary>
        /// <param name="jwk">JWK</param>
        /// <param name="hashAlgorithm">ハッシュ</param>
        /// <returns>BASE64URL(hash(正規化した JSON))。作れなければ空文字</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 必須の項目だけを、辞書順・空白なしの JSON にしてハッシュする（RFC 7638 3.2）。
        ///
        /// | kty | 正規化する項目 |
        /// |---|---|
        /// | RSA | e / kty / n |
        /// | EC | crv / kty / x / y |
        /// | oct | k / kty |
        ///
        /// 秘密鍵の JWK を渡しても、公開鍵の項目だけで計算する（RFC 7638 3.2.1）。
        /// 読めない値でも例外にせず、空文字を返す（外から来る値を扱うため）。
        /// </remarks>
        public static string Thumbprint(JObject jwk, EnumHashAlgorithm hashAlgorithm)
        {
            string canonical = JwkOperator.Canonicalize(jwk);

            if (string.IsNullOrEmpty(canonical))
            {
                return "";
            }

            return CustomEncode.ToBase64UrlString(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(canonical, CustomEncode.UTF_8), hashAlgorithm));
        }

        /// <summary>指紋の対象にする JSON を作る（RFC 7638 3.2）</summary>
        /// <param name="jwk">JWK</param>
        /// <returns>正規化した JSON（作れなければ null）</returns>
        private static string Canonicalize(JObject jwk)
        {
            if (jwk == null)
            {
                return null;
            }

            string kty = JwkOperator.GetString(jwk, JwtConst.kty);

            if (kty == JwtConst.RSA)
            {
                string e = JwkOperator.GetString(jwk, JwtConst.e);
                string n = JwkOperator.GetString(jwk, JwtConst.n);

                if (string.IsNullOrEmpty(e) || string.IsNullOrEmpty(n))
                {
                    return null;
                }

                return "{" + JwkOperator.Member(JwtConst.e, e)
                    + "," + JwkOperator.Member(JwtConst.kty, kty)
                    + "," + JwkOperator.Member(JwtConst.n, n) + "}";
            }
            else if (kty == JwtConst.EC)
            {
                string crv = JwkOperator.GetString(jwk, JwtConst.crv);
                string x = JwkOperator.GetString(jwk, JwtConst.x);
                string y = JwkOperator.GetString(jwk, JwtConst.y);

                if (string.IsNullOrEmpty(crv) || string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                {
                    return null;
                }

                return "{" + JwkOperator.Member(JwtConst.crv, crv)
                    + "," + JwkOperator.Member(JwtConst.kty, kty)
                    + "," + JwkOperator.Member(JwtConst.x, x)
                    + "," + JwkOperator.Member(JwtConst.y, y) + "}";
            }
            else if (kty == JwtConst.oct)
            {
                string k = JwkOperator.GetString(jwk, JwtConst.k);

                if (string.IsNullOrEmpty(k))
                {
                    return null;
                }

                return "{" + JwkOperator.Member(JwtConst.k, k)
                    + "," + JwkOperator.Member(JwtConst.kty, kty) + "}";
            }

            // 扱わない鍵の種類
            return null;
        }

        /// <summary>正規化した JSON の 1 メンバ（"名前":"値"）</summary>
        /// <param name="name">メンバ名</param>
        /// <param name="value">値</param>
        /// <returns>"名前":"値"</returns>
        /// <remarks>メンバの並び（辞書順。RFC 7638 3.3）は、呼び出し側の連結の順で決まる。</remarks>
        private static string Member(string name, string value)
        {
            return JsonConvert.ToString(name) + ":" + JsonConvert.ToString(value);
        }

        #endregion

        #region 公開鍵

        /// <summary>公開鍵だけの JWK か（秘密鍵の項目が無いか）</summary>
        /// <param name="jwk">JWK</param>
        /// <returns>秘密鍵の項目が無ければ true（null は false）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// RFC 9449 4.3 : DPoP の証明の jwk に、秘密鍵が入っていてはならない。
        /// 共通鍵（oct の k）も秘密なので、入っていれば false を返す。
        /// </remarks>
        public static bool IsPublicOnly(JObject jwk)
        {
            if (jwk == null)
            {
                return false;
            }

            foreach (string member in JwkOperator.PrivateMembers)
            {
                if (jwk[member] != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>秘密鍵の項目を落とした JWK を作る</summary>
        /// <param name="jwk">JWK（秘密鍵でもよい）</param>
        /// <returns>公開鍵の JWK（引数は変えない）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 秘密鍵の JWK から、DPoP の証明のヘッダに入れる公開鍵を作るときに使う。
        /// </remarks>
        public static JObject ToPublic(JObject jwk)
        {
            if (jwk == null)
            {
                throw new ArgumentNullException(nameof(jwk));
            }

            JObject ret = (JObject)jwk.DeepClone();

            foreach (string member in JwkOperator.PrivateMembers)
            {
                ret.Remove(member);
            }

            return ret;
        }

        #endregion

        #region private

        /// <summary>文字列の項目を読む（文字列でなければ null）</summary>
        /// <param name="jwk">JWK</param>
        /// <param name="name">項目名</param>
        /// <returns>値</returns>
        private static string GetString(JObject jwk, string name)
        {
            JToken token = jwk[name];
            return (token != null && token.Type == JTokenType.String) ? (string)token : null;
        }

        #endregion
    }
}
