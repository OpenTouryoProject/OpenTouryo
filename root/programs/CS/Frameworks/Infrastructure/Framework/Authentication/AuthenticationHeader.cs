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
//* クラス名        ：AuthenticationHeader
//* クラス日本語名  ：AuthenticationHeader
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/12/26  西野 大介         新規作成
//*  2026/09/12  玄人 幸道         方式の後ろに値の無い Authorization ヘッダで
//*                                例外になっていたのを修正
//*  2026/09/25  玄人 幸道         Basic認証の資格情報を、RFC 6749 §2.3.1 に従って
//*                                符号化・復号するようにした
//*  2026/10/22  玄人 幸道         DPoPスキーム（RFC 9449）の読み書き、WWW-Authenticateの組み立て、
//*                                Basicの復号前後のどちらでも照合する版を追加（#634）
//*  2026/10/30  玄人 幸道         Basicの資格情報を、最初の ":" で分けるようにした（password は ":" を含みうる。RFC 7617 2。#643）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>AuthenticationHeader</summary>
    public class AuthenticationHeader
    {
        #region OAuth2.0

        /// <summary>
        /// GetCredentials(AuthenticationScheme)
        /// </summary>
        /// <param name="authHeader">string</param>
        /// <param name="credentials">string[]</param>
        /// <returns>AuthenticationScheme</returns>
        /// <remarks>
        /// 資格情報を取り出せない場合は、空の配列と "" を返す。
        /// Basic は、**最初の ":" で 2 つに分ける**（user-id は ":" を含まず、最初の ":" より後ろはすべて password。RFC 7617 2）。
        /// ":" が無ければ 1 つ。2026/10/30 まで、すべての ":" で分けていた（#643）。
        /// </remarks>
        public static string GetCredentials(string authHeader, out string[] credentials)
        {
            if (!string.IsNullOrEmpty(authHeader))
            {
                string[] temp = authHeader.Split(' ');

                // 方式の後ろに値が無い場合は、資格情報の無い要求として扱う。
                // 以前は temp[1] を確かめずに読み、IndexOutOfRangeException になっていた。
                if (temp.Length >= 2 && !string.IsNullOrEmpty(temp[1]))
                {
                    if (temp[0] == OAuth2AndOIDCConst.Basic)
                    {
                        try
                        {
                            // 最初の ":" で分ける（password は ":" を含みうる。RFC 7617 2。#643）
                            credentials = CustomEncode.ByteToString(
                                CustomEncode.FromBase64String(temp[1]), CustomEncode.us_ascii).Split(new char[] { ':' }, 2);

                            return OAuth2AndOIDCConst.Basic;
                        }
                        catch (FormatException)
                        {
                            // Base64 として読めない。資格情報が無いものとして扱う。
                        }
                    }
                    else if (temp[0] == OAuth2AndOIDCConst.Bearer)
                    {
                        credentials = new string[] { temp[1] };

                        return OAuth2AndOIDCConst.Bearer;
                    }
                }
            }

            credentials = new string[] { };
            return "";
        }

        #region Basic client_id-

        /// <summary>
        /// GetCredentials(Basic)
        /// authHeaderからclient_id と client_secret（Basic認証）を抽出取得
        /// </summary>
        /// <param name="authHeader">string</param>
        /// <param name="client_id">string</param>
        /// <param name="client_secret">string</param>
        /// <returns>bool</returns>
        public static bool GetCredentials(string authHeader, out string client_id, out string client_secret)
        {
            client_id = "";
            client_secret = "";
            string[] credentials = null;

            if (AuthenticationHeader.GetCredentials(authHeader, out credentials) == OAuth2AndOIDCConst.Basic)
            {
                // Length == 1 の ケースもサポート
                if (credentials.Length == 1)
                {
                    client_id = credentials[0];
                    return true;
                }
                else if (credentials.Length == 2)
                {
                    client_id = credentials[0];
                    client_secret = credentials[1];
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// GetCredentials(Basic、RFC 6749 §2.3.1 の復号あり)
        /// URLデコード処理を施して安全に取得するオーバーロード
        /// </summary>
        /// <param name="authHeader">string</param>
        /// <param name="client_id">string（復号後）</param>
        /// <param name="client_secret">string（復号後）</param>
        /// <param name="rawClientId">string（復号前＝ヘッダに載っていたまま）</param>
        /// <param name="rawClientSecret">string（復号前＝ヘッダに載っていたまま）</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// RFC 6749 §2.3.1 は、client_idとclient_secretを
        /// application/x-www-form-urlencodedで符号化してからBase64にすることを求めている。
        /// このため、受け側は復号してから照合する。
        /// ただし、符号化しないクライアントも在るため、復号前の値も返す。
        /// 両方と照合すれば、どちらのクライアントも受けられる。
        /// </remarks>
        public static bool GetCredentials(string authHeader,
            out string client_id, out string client_secret,
            out string rawClientId, out string rawClientSecret)
        {
            client_id = "";
            client_secret = "";

            if (AuthenticationHeader.GetCredentials(authHeader, out rawClientId, out rawClientSecret))
            {
                client_id = CustomEncode.UrlDecode(rawClientId);
                client_secret = CustomEncode.UrlDecode(rawClientSecret);

                return true;
            }

            return false;
        }

        /// <summary>
        /// GetCredentials(Basic、復号前後のどちらでも照合する)
        /// verifier（検証デリゲート）を引数に取ることで、柔軟な認証チェックを実現
        /// </summary>
        /// <param name="authHeader">string</param>
        /// <param name="verifier">
        /// 照合する処理（client_id, client_secret → 認証できれば true）。
        /// 設定やストアを読むだけの、副作用の無い処理を渡すこと（最大 2 回呼ばれる）。
        /// </param>
        /// <param name="client_id">string（照合に通った方。どちらも通らなければ復号後）</param>
        /// <param name="client_secret">string（同上）</param>
        /// <returns>Basicの資格情報が在ったか（照合の結果ではない）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// RFC 6749 §2.3.1 に従うクライアントは、記号を含む値を符号化して送る。
        /// 符号化しないクライアント（#592 より前の Open棟梁 など）も配備に残っている。
        /// そこで、まず復号後の値で照合し、通らなければ復号前の値で照合する（#634 の H-3）。
        /// 英数字だけの値は、どちらも同じ文字列なので、照合は 1 回で済む。
        /// 照合の結果は、呼び出し側が返された値で改めて判断すること。
        /// </remarks>
        public static bool GetCredentials(string authHeader,
            Func<string, string, bool> verifier, out string client_id, out string client_secret)
        {
            if (verifier == null)
            {
                throw new ArgumentNullException(nameof(verifier));
            }

            if (!AuthenticationHeader.GetCredentials(authHeader,
                out client_id, out client_secret, out string rawClientId, out string rawClientSecret))
            {
                return false;
            }

            if ((client_id != rawClientId || client_secret != rawClientSecret)
                && !verifier(client_id, client_secret)
                && verifier(rawClientId, rawClientSecret))
            {
                // 符号化しないクライアントが、記号を含む値を送ってきた。
                client_id = rawClientId;
                client_secret = rawClientSecret;
            }

            return true;
        }

        /// <summary>CreateBasicAuthenticationHeaderValue</summary>
        /// <param name="id">string</param>
        /// <param name="secret">string</param>
        /// <returns>AuthenticationHeaderValue</returns>
        /// <remarks>
        /// RFC 6749 §2.3.1 は、idとsecretを
        /// application/x-www-form-urlencodedで符号化してから ":" で連結し、
        /// Base64にすることを求めている。
        /// 英数字だけなら符号化しても同じ文字列なので、見た目は変わらない。
        /// 
        /// 空白は "+" ではなく "%20" になる（CustomEncode.UrlEncodeがUri.EscapeDataStringのため）。
        /// RFC 6749 Appendix B の例は "+" だが、"%20" はフォーム形式の復号器でも
        /// 素朴なURI復号器（Uri.UnescapeDataString）でも空白に戻るのに対し、
        /// "+" は後者では "+" のまま残る。壊れにくい方を選んでいる。
        /// 字面どおりに寄せるなら、WebUtility.UrlEncodeを使う。
        /// 
        /// 変わるのは "+" "/" "=" "%" ":" などを含む場合で、
        /// 受け側は復号してから照合する必要がある
        /// （復号ありのGetCredentialsを参照。復号しないサーバも在るため、
        /// 復号前の値とも照合する形にしておくと、どちらとも繋がる）。
        /// </remarks>
        public static AuthenticationHeaderValue CreateBasicAuthenticationHeaderValue(string id, string secret)
        {
            // id + x509 のパターンをサポート
            if (!string.IsNullOrEmpty(id)) // && !string.IsNullOrEmpty(secret))
            {
                return new AuthenticationHeaderValue(
                    OAuth2AndOIDCConst.Basic,
                    CustomEncode.ToBase64String(CustomEncode.StringToByte(
                        string.Format("{0}:{1}",
                            CustomEncode.UrlEncode(id),
                            CustomEncode.UrlEncode(secret ?? "")), CustomEncode.us_ascii)));
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Bearer token

        /// <summary>GetCredentials(Bearer)</summary>
        /// <param name="authHeader">string</param>
        /// <param name="bearerToken">string</param>
        /// <returns>bool</returns>
        public static bool GetCredentials(string authHeader, out string bearerToken)
        {
            bearerToken = "";
            string[] credentials = null;

            if (AuthenticationHeader.GetCredentials(authHeader, out credentials) == OAuth2AndOIDCConst.Bearer)
            {
                if (credentials.Length == 1)
                {
                    bearerToken = credentials[0];
                    return true;
                }
            }

            return false;
        }

        /// <summary>CreateBearerAuthenticationHeaderValue</summary>
        /// <param name="accessToken">string</param>
        /// <returns>AuthenticationHeaderValue</returns>
        public static AuthenticationHeaderValue CreateBearerAuthenticationHeaderValue(string accessToken)
        {
            return new AuthenticationHeaderValue(OAuth2AndOIDCConst.Bearer, accessToken);
        }

        #endregion

        #region DPoP（RFC 9449）

        /// <summary>
        /// authHeaderから認証スキーム（Bearer または DPoP）とaccess_tokenを安全に抽出・検証
        /// </summary>
        /// <param name="authHeader">string（Authorizationヘッダ）</param>
        /// <param name="accessToken">out string</param>
        /// <param name="scheme">out string（OAuth2AndOIDCConst.Bearer or DPoP。読めなければ空）</param>
        /// <returns>読めればtrue</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// スキームは大文字小文字を区別しない（RFC 7235 2.1）。
        ///
        /// **スキームを返すのが要点である。**
        /// cnf.jkt で縛ったトークンを Bearer で出してきた要求は、断らなければならない
        /// （縛りを「使う側の出し方」で外せてはいけない。RFC 9449 7.2）。
        /// </remarks>
        public static bool GetAccessToken(string authHeader, out string accessToken, out string scheme)
        {
            accessToken = "";
            scheme = "";

            if (string.IsNullOrEmpty(authHeader))
            {
                return false;
            }

            string value = authHeader.Trim();
            int index = value.IndexOf(' ');

            if (index <= 0)
            {
                return false;
            }

            // 上記のチェック後、認証スキームを除いた本体を切り出し余分な空白を除去
            string token = value.Substring(index + 1).Trim();

            // b64token / token68 は空白を含まない（RFC 6750 2.1）。
            if (string.IsNullOrEmpty(token) || 0 <= token.IndexOf(' '))
            {
                return false;
            }

            string s = value.Substring(0, index);

            if (string.Equals(s, OAuth2AndOIDCConst.Bearer, StringComparison.OrdinalIgnoreCase))
            {
                scheme = OAuth2AndOIDCConst.Bearer;
            }
            else if (string.Equals(s, OAuth2AndOIDCConst.DPoP, StringComparison.OrdinalIgnoreCase))
            {
                scheme = OAuth2AndOIDCConst.DPoP;
            }
            else
            {
                return false;
            }

            accessToken = token;
            return true;
        }

        /// <summary>CreateDPoPAuthenticationHeaderValue</summary>
        /// <param name="accessToken">string（DPoPで縛られたaccess_token）</param>
        /// <returns>AuthenticationHeaderValue</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// DPoPで縛られたトークンは、Bearerではなく、DPoPスキームで送る（RFC 9449 7.1）。
        /// 同じ要求の DPoP ヘッダに、ath を入れた証明を添えること（DPoPProof.Create）。
        /// </remarks>
        public static AuthenticationHeaderValue CreateDPoPAuthenticationHeaderValue(string accessToken)
        {
            return new AuthenticationHeaderValue(OAuth2AndOIDCConst.DPoP, accessToken);
        }

        #endregion

        #endregion

        #region チャレンジのパラメタ

        // チャレンジのパラメタ生成
        // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"

        /// <summary>チャレンジのスキームを決める</summary>
        /// <param name="authHeader">string（要求のAuthorizationヘッダ）</param>
        /// <returns>DPoPは OAuth2AndOIDCConst.DPoP、それ以外は .Bearer</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// DPoPスキームで来た要求には、DPoPで答える（RFC 9449 7.1）。
        /// それ以外は Bearer（DPoPの話をしていない相手に、別のスキームを突き付けない）。
        /// </remarks>
        public static string GetChallengeScheme(string authHeader)
        {
            AuthenticationHeader.GetAccessToken(authHeader, out string _, out string scheme);

            return (scheme == OAuth2AndOIDCConst.DPoP) ? OAuth2AndOIDCConst.DPoP : OAuth2AndOIDCConst.Bearer;
        }

        /// <summary>チャレンジのパラメタを作る</summary>
        /// <param name="scheme">string（OAuth2AndOIDCConst.Bearer / .DPoP）</param>
        /// <param name="realm">string（保護資源の名前。空なら付けない）</param>
        /// <param name="error">string（トークンが無い要求なら空）</param>
        /// <param name="errorDescription">string（空なら付けない）</param>
        /// <param name="algs">string[]（DPoPのときに示す、証明の署名に使えるalg。nullなら付けない）</param>
        /// <returns>string（例 : realm="api", error="invalid_token"）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// - トークンが無い要求（ヘッダ無し・他の方式）には、エラー情報を付けない（RFC 6750 3.1 : SHOULD NOT）
        /// - DPoPのときは algs を付ける（RFC 9449 7.1）
        /// - 値は quoted-string にする。" と \ は \ で逃がす（RFC 9110 5.6.4）
        /// ヘッダの付け方は P層のフレームワークごとに違うので、呼び出し側で付ける
        /// （AuthenticationHeaderValue(scheme, parameter) にも渡せる）。
        /// </remarks>
        public static string CreateChallengeParameter(
            string scheme, string realm, string error, string errorDescription, string[] algs)
        {
            List<string> items = new List<string>();

            if (!string.IsNullOrEmpty(realm))
            {
                items.Add(AuthenticationHeader.QuotedParameter(OAuth2AndOIDCConst.realm, realm));
            }

            if (!string.IsNullOrEmpty(error))
            {
                items.Add(AuthenticationHeader.QuotedParameter(OAuth2AndOIDCConst.error, error));

                if (!string.IsNullOrEmpty(errorDescription))
                {
                    items.Add(AuthenticationHeader.QuotedParameter(
                        OAuth2AndOIDCConst.error_description, errorDescription));
                }
            }

            if (scheme == OAuth2AndOIDCConst.DPoP && algs != null && 0 < algs.Length)
            {
                items.Add(AuthenticationHeader.QuotedParameter(
                    OAuth2AndOIDCConst.algs, string.Join(" ", algs)));
            }

            return string.Join(", ", items);
        }

        /// <summary>auth-param を quoted-string で作る</summary>
        /// <param name="name">string</param>
        /// <param name="value">string</param>
        /// <returns>name="value"</returns>
        private static string QuotedParameter(string name, string value)
        {
            return name + "=\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }

        #endregion
    }
}
