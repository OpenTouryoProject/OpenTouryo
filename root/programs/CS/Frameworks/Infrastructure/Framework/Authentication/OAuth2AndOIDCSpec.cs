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
//* クラス名        ：OAuth2AndOIDCSpec
//* クラス日本語名  ：OAuth2 / OIDC の仕様が決めている小さな処理
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の G / H-2）
//*  2026/10/22  玄人 幸道         PKCE の検証（認可サーバ側）を追加（汎用認証サイトから移した。#634）
//*  2026/10/22  玄人 幸道         PKCE の検証に、plain を受けるかを設定（CmnServerParams.RequirePkceS256）から読む版を追加
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2 / OIDC の仕様が決めている小さな処理</summary>
    /// <remarks>
    /// どれも「RFC・OIDC の仕様がそう決めている」だけの処理で、配備ごとの決めを含まない。
    /// 認可サーバ・リソース サーバ・クライアントのどれからも使える。
    /// </remarks>
    public class OAuth2AndOIDCSpec
    {
        #region 空白区切りの列挙（G-9）

        /// <summary>空白区切りの列挙を分ける</summary>
        /// <param name="value">string（scope / response_type / redirect_uris の登録値など）</param>
        /// <returns>要素の並び（空なら 0 件。重複は残す）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 区切りは空白（半角・全角・タブ・改行）。空の要素は捨てる。
        /// scope が空白区切り（RFC 6749 3.3）なのと同じ流儀である。
        /// カンマは区切りにしない（URI に入りうる。RFC 3986 2.2 の sub-delims）。
        /// URI は空白を含まない（RFC 3986 2）ので、URI の列挙にも使える。
        /// </remarks>
        public static string[] SplitSpaceDelimited(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new string[0];
            }

            return value.Split(
                new char[] { ' ', '\t', '\r', '\n', '　' },
                StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>空白区切りの列挙を、重複の無い一覧にする</summary>
        /// <param name="value">string</param>
        /// <returns>一覧（出てきた順。大文字小文字は区別する）</returns>
        /// <remarks>自由に利用できる。切り方は SplitSpaceDelimited と同じ。</remarks>
        public static List<string> ToDistinctList(string value)
        {
            List<string> ret = new List<string>();

            foreach (string s in OAuth2AndOIDCSpec.SplitSpaceDelimited(value))
            {
                if (!ret.Contains(s))
                {
                    ret.Add(s);
                }
            }

            return ret;
        }

        #endregion

        #region response_type（G-3）

        /// <summary>response_type を正規化する</summary>
        /// <param name="response_type">string</param>
        /// <returns>空白で分け、重複を除き、辞書順に並べて、空白 1 個で繋いだもの（空ならそのまま）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// response_type は順不同の空白区切りの集合である
        /// （OAuth 2.0 Multiple Response Type Encoding Practices 3）。
        /// 辞書順は OAuth2AndOIDCConst の並び（code &lt; id_token &lt; token）と一致するので、
        /// 正規化した値は、OidcHybrid3_ResponseType などと文字列で比べられる。
        /// 値は小文字にする（仕様では大文字小文字を区別するが、寛容に受ける実装が多いため）。
        /// 厳密に扱うなら、呼び出し側で大文字を含む値を先に断ること。
        /// </remarks>
        public static string NormalizeResponseType(string response_type)
        {
            if (string.IsNullOrEmpty(response_type))
            {
                return response_type;
            }

            List<string> normalized = OAuth2AndOIDCSpec.ToDistinctList(response_type.ToLowerInvariant());
            normalized.Sort(StringComparer.Ordinal);

            return string.Join(" ", normalized);
        }

        #endregion

        #region prompt（G-4）

        /// <summary>prompt を空白区切りの集合として分ける</summary>
        /// <param name="prompt">string</param>
        /// <returns>値の一覧（重複なし・小文字。空なら 0 件）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// prompt は空白区切りの集合である（OIDC Core 3.1.2.1）。
        /// 部分文字列で探す（Contains("none")）と prompt=nonexistent で真になり、
        /// 完全一致で比べる（== "none"）と prompt=none login で偽になる。集合として扱うこと。
        /// </remarks>
        public static List<string> SplitPrompt(string prompt)
        {
            return OAuth2AndOIDCSpec.ToDistinctList(
                string.IsNullOrEmpty(prompt) ? prompt : prompt.ToLowerInvariant());
        }

        /// <summary>prompt に、その値が入っているか</summary>
        /// <param name="prompt">string</param>
        /// <param name="value">string（OAuth2AndOIDCConst.PromptNone など）</param>
        /// <returns>入っていれば true</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool HasPrompt(string prompt, string value)
        {
            return OAuth2AndOIDCSpec.SplitPrompt(prompt).Contains(value);
        }

        /// <summary>prompt の組み合わせを確かめる</summary>
        /// <param name="prompt">string</param>
        /// <param name="err">string（不正なら invalid_request）</param>
        /// <param name="errDescription">string</param>
        /// <returns>正しければ true</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// none は他の値と併記できない（OIDC Core 3.1.2.1 :
        /// If this parameter contains none with any other value, an error is returned.）。
        /// 既知でない値は、それだけではエラーにしない（仕様が invalid_request としていない）。
        /// </remarks>
        public static bool CheckPrompt(string prompt, ref string err, ref string errDescription)
        {
            List<string> values = OAuth2AndOIDCSpec.SplitPrompt(prompt);

            if (values.Contains(OAuth2AndOIDCConst.PromptNone) && 1 < values.Count)
            {
                err = OAuth2AndOIDCConst.invalid_request;
                errDescription = "prompt=none must not be combined with other values.";
                return false;
            }

            return true;
        }

        #endregion

        #region max_age（G-5）

        /// <summary>max_age と auth_time を照合する</summary>
        /// <param name="max_age">string（要求の max_age。空なら照合しない）</param>
        /// <param name="authTime">DateTime?（前回の認証時刻。UTC。分からなければ null）</param>
        /// <returns>OAuth2AndOIDCEnum.AuthTimeCheck</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// - max_age は 0 以上の整数（OIDC Core 3.1.2.1）。そうでなければ InvalidMaxAge
        /// - 認証時刻が分からなければ NeedsReAuthentication
        /// - 経過は「秒」で比べる（切り捨て）。auth_time も max_age も秒単位なので、
        ///   秒未満の差を超過とみなさない（小数で比べると、max_age=0 で再認証の直後も超過になり、
        ///   何度でも再認証を求めることになる）
        /// 再認証を求めた後の繰り返しを防ぐ印（Cookie など）は、配備の決めなので、呼び出し側で持つ。
        /// </remarks>
        public static OAuth2AndOIDCEnum.AuthTimeCheck CheckAuthTime(string max_age, DateTime? authTime)
        {
            if (string.IsNullOrEmpty(max_age))
            {
                return OAuth2AndOIDCEnum.AuthTimeCheck.Ok;
            }

            if (!int.TryParse(max_age, out int maxAge) || maxAge < 0)
            {
                return OAuth2AndOIDCEnum.AuthTimeCheck.InvalidMaxAge;
            }

            if (authTime == null)
            {
                return OAuth2AndOIDCEnum.AuthTimeCheck.NeedsReAuthentication;
            }

            long elapsedSeconds = (long)(DateTime.UtcNow - ((DateTime)authTime).ToUniversalTime()).TotalSeconds;

            return (elapsedSeconds <= maxAge)
                ? OAuth2AndOIDCEnum.AuthTimeCheck.Ok : OAuth2AndOIDCEnum.AuthTimeCheck.NeedsReAuthentication;
        }

        #endregion

        #region PKCE（RFC 7636）

        /// <summary>PKCE を検証する（認可サーバ側。設定を読む版）</summary>
        /// <param name="code_challenge_method">string（認可要求で受け取った code_challenge_method）</param>
        /// <param name="code_challenge">string（認可要求で受け取った code_challenge）</param>
        /// <param name="code_verifier">string（トークン要求で受け取った code_verifier）</param>
        /// <param name="usedS256">out bool（S256 で検証できたか）</param>
        /// <returns>検証できれば true</returns>
        /// <remarks>
        /// 自由に利用できる。plain を受けるかは CmnServerParams.RequirePkceS256 で決める
        /// （true なら S256 だけ。無ければ plain も受ける）。
        /// </remarks>
        public static bool VerifyPkce(string code_challenge_method, string code_challenge,
            string code_verifier, out bool usedS256)
        {
            return OAuth2AndOIDCSpec.VerifyPkce(code_challenge_method, code_challenge,
                code_verifier, !CmnServerParams.RequirePkceS256, out usedS256);
        }

        /// <summary>PKCE を検証する（認可サーバ側）</summary>
        /// <param name="code_challenge_method">string（認可要求で受け取った code_challenge_method）</param>
        /// <param name="code_challenge">string（認可要求で受け取った code_challenge）</param>
        /// <param name="code_verifier">string（トークン要求で受け取った code_verifier）</param>
        /// <param name="allowPlain">bool（plain を受けるか。OAuth 2.1 / FAPI は S256 だけ）</param>
        /// <param name="usedS256">out bool（S256 で検証できたか）</param>
        /// <returns>検証できれば true</returns>
        /// <remarks>
        /// 自由に利用できる。クライアント側の OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod の対である
        /// （汎用認証サイトから移した。#634）。
        ///
        /// - 認可要求で PKCE を使っていない（code_challenge_method / code_challenge が空）なら false
        /// - S256 : BASE64URL(SHA256(ASCII(code_verifier))) が code_challenge と一致すること（RFC 7636 4.6）
        /// - plain : allowPlain のときだけ、code_verifier が code_challenge と一致すること
        /// - 未知のメソッドは false
        /// - メソッド名の大文字小文字は区別しない（汎用認証サイトの従来の扱い）
        /// クライアント認証とは別の検証なので、呼び出し側は認証の成否と併せて判断すること。
        /// </remarks>
        public static bool VerifyPkce(string code_challenge_method, string code_challenge,
            string code_verifier, bool allowPlain, out bool usedS256)
        {
            usedS256 = false;

            if (string.IsNullOrEmpty(code_challenge_method)
                || string.IsNullOrEmpty(code_challenge)
                || string.IsNullOrEmpty(code_verifier))
            {
                return false;
            }

            if (string.Equals(code_challenge_method, OAuth2AndOIDCConst.PKCE_S256, StringComparison.OrdinalIgnoreCase))
            {
                usedS256 = string.Equals(code_challenge,
                    OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(code_verifier), StringComparison.Ordinal);

                return usedS256;
            }

            if (string.Equals(code_challenge_method, OAuth2AndOIDCConst.PKCE_plain, StringComparison.OrdinalIgnoreCase))
            {
                return allowPlain && string.Equals(code_challenge, code_verifier, StringComparison.Ordinal);
            }

            return false;
        }

        #endregion

        #region 認可応答の URL（G-6）

        /// <summary>認可応答の URL を組む</summary>
        /// <param name="redirectUri">string（redirect_uri）</param>
        /// <param name="parameters">Dictionary（付けるパラメタ。値が空のものは付けない）</param>
        /// <param name="useFragment">bool（true : 素片（#）、false : クエリ（?））</param>
        /// <param name="iss">string（認可サーバの Issuer。null か空なら付けない）</param>
        /// <returns>URL</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// - **値は必ず URL エンコードする。** state はクライアントが自由に決めるので、
        ///   生で連結すると、リダイレクト先にパラメタを注入できてしまう
        /// - 既にクエリ（素片）を持つ redirect_uri でも壊れないよう、区切りを ? / &amp;（# / &amp;）で切り替える
        /// - iss を付ける（RFC 9207。Mix-Up 攻撃への対策で、成功にも失敗にも付ける）。
        ///   ただし JARM（response=...）のときは付けない（署名された JWT の中に iss が入っている）
        /// - 引数の Dictionary は変えない
        /// </remarks>
        public static string BuildRedirectUrl(string redirectUri,
            Dictionary<string, string> parameters, bool useFragment, string iss)
        {
            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            Dictionary<string, string> p = (parameters == null)
                ? new Dictionary<string, string>() : new Dictionary<string, string>(parameters);

            if (!string.IsNullOrEmpty(iss)
                && !p.ContainsKey(OAuth2AndOIDCConst.iss)
                && !p.ContainsKey(OAuth2AndOIDCConst.response))
            {
                p[OAuth2AndOIDCConst.iss] = iss;
            }

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> kv in p)
            {
                if (string.IsNullOrEmpty(kv.Value))
                {
                    continue;
                }

                if (sb.Length != 0)
                {
                    sb.Append("&");
                }

                sb.Append(Uri.EscapeDataString(kv.Key));
                sb.Append("=");
                sb.Append(Uri.EscapeDataString(kv.Value));
            }

            if (sb.Length == 0)
            {
                return redirectUri;
            }

            if (useFragment)
            {
                return redirectUri + (redirectUri.Contains("#") ? "&" : "#") + sb.ToString();
            }
            else
            {
                return redirectUri + (redirectUri.Contains("?") ? "&" : "?") + sb.ToString();
            }
        }

        #endregion

        #region エラー応答の HTTP ステータス（G-1）

        /// <summary>トークン エンドポイントなどのエラー応答の HTTP ステータス</summary>
        /// <param name="error">string（error の値）</param>
        /// <returns>401（invalid_client / invalid_token）か 400</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// - RFC 6749 5.2 : エラーは 400。invalid_client（クライアント認証の失敗）は 401
        /// - RFC 6750 3.1 : invalid_token は 401
        /// - RFC 8628 3.5 / CIBA Core 11 : ポーリングのエラー（authorization_pending など）は 400
        /// - RFC 9449 8 : /token の use_dpop_nonce は 400
        /// </remarks>
        public static int GetErrorStatusCode(string error)
        {
            if (error == OAuth2AndOIDCConst.invalid_client || error == OAuth2AndOIDCConst.invalid_token)
            {
                return 401;
            }

            return 400;
        }

        /// <summary>保護されたリソースのエラー応答の HTTP ステータス</summary>
        /// <param name="error">string（error の値）</param>
        /// <returns>401（invalid_token / use_dpop_nonce）・403（insufficient_scope）か 400</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// - RFC 6750 3.1 : invalid_request は 400、invalid_token は 401、insufficient_scope は 403
        /// - RFC 9449 9 : リソースの use_dpop_nonce は 401（チャレンジである）。
        ///   /token では 400 なので、口によって状態が違う（GetErrorStatusCode と分けている）
        /// </remarks>
        public static int GetResourceErrorStatusCode(string error)
        {
            if (error == OAuth2AndOIDCConst.invalid_token || error == OAuth2AndOIDCConst.use_dpop_nonce)
            {
                return 401;
            }

            if (error == OAuth2AndOIDCConst.insufficient_scope)
            {
                return 403;
            }

            return 400;
        }

        #endregion

        #region クライアント認証のアサーション（H-2）

        /// <summary>クライアント認証のアサーションを取り出す</summary>
        /// <param name="client_assertion">string（RFC 7523 2.2 の名前）</param>
        /// <param name="client_assertion_type">string</param>
        /// <param name="assertion">string（Open棟梁 の既存のクライアントが送る名前）</param>
        /// <returns>アサーション（無い・受け付けられないなら空文字）</returns>
        /// <remarks>
        /// 自由に利用できる。認可サーバ側で使う。
        /// RFC 7523 2.2 が定めているのは client_assertion（＋ client_assertion_type）で、
        /// assertion は JWT Bearer グラント（2.1）のパラメタであり、別物である。
        ///
        /// ただし、Open棟梁 の OAuth2AndOIDCClient は、#592 より前は assertion だけを送っていた
        /// （現在は両方を送る）。**その互換は、送り手の部品の側で持つ**（#634 の H-2）。
        ///
        /// - client_assertion を優先する。client_assertion_type が来ていて値が違えば、受け付けない
        /// - 無ければ assertion を読む（grant_type が JWT Bearer の要求では、呼び出し側が使い分けること）
        /// </remarks>
        public static string GetClientAssertion(
            string client_assertion, string client_assertion_type, string assertion)
        {
            if (!string.IsNullOrEmpty(client_assertion))
            {
                if (!string.IsNullOrEmpty(client_assertion_type)
                    && client_assertion_type != OAuth2AndOIDCConst.JwtBearerClientAssertionType)
                {
                    return "";
                }

                return client_assertion;
            }

            return assertion ?? "";
        }

        #endregion
    }
}
