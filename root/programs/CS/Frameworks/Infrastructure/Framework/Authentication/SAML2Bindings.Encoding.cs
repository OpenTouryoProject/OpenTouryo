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
//* クラス名        ：SAML2Bindings（Encoding）
//* クラス日本語名  ：SAML2Bindings（ライブラリ）の共通（符号化・署名 / 復号・検証）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（SAML2Bindings.cs から処理区分ごとに分割した）
//*  2026/10/22  玄人 幸道         StringExtractor.GetParameterFromQueryString（Obsolete）を UriOperator に置き換えた
//*  2026/10/30  玄人 幸道         Redirect の SigAlg を、RSA-PSS・ECDSA でも署名のアルゴリズムどおりに書くようにした。決められなければ例外（#643）
//*  2026/11/05  玄人 幸道         VerifyRedirect：署名の対象を、SAMLRequest（SAMLResponse）・RelayState・SigAlg の 3 つだけから組み立てる
//*                                （SAML 2.0 Bindings 3.4.4.1）。ほかのパラメタが付いていても検証できるようにした（#649）。
//*                                DecodeRedirect も同じ読み方（TryReadRedirectParameters）で SAML を取り出す（名前は完全一致。重複・曖昧な名前は読めないものとする）。
//*                                空の RelayState= は、含めない版（仕様どおり）と含める版（互換）の両方で照合する
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Bindings（ライブラリ）の共通（符号化・署名 / 復号・検証）</summary>
    public partial class SAML2Bindings
    {
        #region HTTP-Redirect（符号化・署名 / 復号・検証）

        /// <summary>EncodeAndSignRedirect</summary>
        /// <param name="type">SAML2Enum.RequestOrResponse</param>
        /// <param name="saml">string</param>
        /// <param name="relayState">string</param>
        /// <param name="ds">
        /// 署名に使う DigitalSign（null なら署名しない）。
        /// SigAlg は、この DigitalSign のアルゴリズムから決める（#604）。
        /// </param>
        /// <returns>RedirectBinding用クエリ文字列</returns>
        /// <remarks>
        /// SigAlg（SAML 2.0 Bindings 3.4.4.1）は、実際の署名アルゴリズムを示す。
        /// 2026/10/12 まで rsa-sha1 固定で、既定の SHA256 の署名にも rsa-sha1 と書いていた（#604）。
        /// RSA（PKCS#1 v1.5）の SHA1 / SHA256 / SHA384 / SHA512、RSA-PSS と ECDSA の SHA256 / SHA384 / SHA512、DSA を書く
        /// （RFC 9231 2.3.2〜2.3.4・2.3.10・2.3.6）。
        /// **それ以外のアルゴリズムは ArgumentException**（誤った SigAlg を出さない）。
        /// 2026/10/30 まで、それ以外（RSA-PSS・ECDSA を含む）は rsa-sha1 と書いていた（#643）。
        /// </remarks>
        public static string EncodeAndSignRedirect(
            SAML2Enum.RequestOrResponse type,
            string saml, string relayState,
            DigitalSign ds = null)
        {
            // --------------------------------------------------
            // - XML → XML宣言のエンコーディング → DEFLATE圧縮
            // -   → Base64エンコード → URLエンコード →  クエリ文字列テンプレへ組込
            // -      → コレをASCIIエンコード → 署名 → Base64エンコード
            // -         → URLエンコード →  Signatureパラメタ追加。
            // --------------------------------------------------
            // ・ヘッダも必要
            //   "<?xml version="1.0" encoding="UTF-8"?>"
            // ・クエリ文字列テンプレート
            //   ・SAMLRequest=value&RelayState=value&SigAlg=value
            //   ・SAMLResponse=value&RelayState=value&SigAlg=value
            // ・クエリ文字列署名
            //   ・クエリ文字列パラメタ値が空文字列の場合は、パラメタ自体を署名の演算から除外する。
            //   ・署名対象は上記テンプレートの文字列で、署名は、SignatureパラメタとしてURLに追加。

            string queryString = "";
            string queryStringTemplate = "";

            #region クエリ文字列テンプレート生成
            if (string.IsNullOrEmpty(saml))
            {
                return "";
            }
            else
            {

                // 第1 QSパラメタ
                switch (type)
                {
                    case SAML2Enum.RequestOrResponse.Request:
                        queryStringTemplate += SAML2Const.QueryParamSAMLRequest + "={SAML}";
                        break;
                    case SAML2Enum.RequestOrResponse.Response:
                        queryStringTemplate += SAML2Const.QueryParamSAMLResponse + "={SAML}";
                        break;
                }

                // 第2 QSパラメタ
                if (string.IsNullOrEmpty(relayState))
                {
                    // RelayStateパラメタなし
                }
                else
                {
                    queryStringTemplate += "&" + SAML2Const.QueryParamRelayState + "={RelayState}";
                }

                // 第3 QSパラメタ
                if (ds == null)
                {
                    // SigAlg, Signatureパラメタなし
                }
                else
                {
                    // 第3 QSパラメタ（実際の署名アルゴリズム。#604）
                    // 決められなければ例外にする（誤った SigAlg を出さない。#643）
                    string sigAlg = SAML2Bindings.GetSigAlg(ds);
                    if (string.IsNullOrEmpty(sigAlg))
                    {
                        throw new ArgumentException(
                            "The signature algorithm of the DigitalSign has no SigAlg URI (RFC 9231).", nameof(ds));
                    }

                    queryStringTemplate += "&" + SAML2Const.QueryParamSigAlg + "=" + CustomEncode.UrlEncode(sigAlg);
                }                
            }
            #endregion

            #region エンコーディング

            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(saml);

            // XML (→ XML宣言のエンコーディングではなく、asciiエンコーディングに変更)
            // → DEFLATE圧縮 → Base64エンコード → URLエンコード
            saml = CustomEncode.UrlEncode(CustomEncode.ToBase64String(
                DeflateCompression.Compress(CustomEncode.StringToByte(saml, CustomEncode.us_ascii))));
            #endregion

            #region 組込 & 署名

            // 署名対象となるクエリ文字列の生成（クエリ文字列のテンプレートへ組込
            queryString = queryStringTemplate;

            // - SAMLReXXXXXパラメタ
            queryString = queryString.Replace("{SAML}", saml);

            // - RelayStateパラメタ
            if (!string.IsNullOrEmpty(relayState))
            {
                queryString = queryString.Replace("{RelayState}", CustomEncode.UrlEncode(relayState));
            }

            // - Signatureパラメタ
            if (ds != null)
            {
                // ASCIIエンコード → 署名 → Base64エンコード → URLエンコード →  Signatureパラメタ追加。
                string signature = CustomEncode.UrlEncode(CustomEncode.ToBase64String(
                    ds.Sign(CustomEncode.StringToByte(
                    	queryString, CustomEncode.us_ascii)))); // enc.CodePage))));

                queryString = queryString + "&" + SAML2Const.QueryParamSignature + "=" + signature;
            }
            #endregion

            return queryString; 
        }

        /// <summary>DecodeRedirect</summary>
        /// <param name="queryString">string</param>
        /// <returns>
        /// デコードされたsaml。読めなければ空文字（SAMLRequest と SAMLResponse の両方・どちらも無い、
        /// 署名に関わるパラメタの重複、エンコードされた名前で署名に関わるパラメタを名乗るもの）。
        /// 2026/11/05 までは、名前を部分一致で探し、両方あれば SAMLRequest を、重複があれば最初の値を返していた
        /// </returns>
        /// <remarks>
        /// **SAMLRequest / SAMLResponse は、VerifyRedirect と同じ読み方（TryReadRedirectParameters）で取り出す**（#649）。
        /// 署名を確かめた値と、ここで取り出す値が、必ず同じものになるようにするため。
        /// パラメタ名は完全一致で見る。署名に関わるパラメタの重複、SAMLRequest と SAMLResponse の両方・どちらも無いものは、空文字を返す。
        /// </remarks>
        public static string DecodeRedirect(string queryString)
        {
            // EcodeRedirectの逆
            // --------------------------------------------------
            // Saml → URLデコード → Base64デコード
            //   → DEFLATE解凍 → XML宣言のエンコーディング → XML
            // --------------------------------------------------
            // Samlの抽出（VerifyRedirect と同じ読み方。#649）
            Dictionary<string, string> raw;
            if (!SAML2Bindings.TryReadRedirectParameters(queryString, out raw))
            {
                return "";
            }

            string request, response;
            raw.TryGetValue(SAML2Const.QueryParamSAMLRequest, out request);
            raw.TryGetValue(SAML2Const.QueryParamSAMLResponse, out response);

            if (string.IsNullOrEmpty(request) == string.IsNullOrEmpty(response))
            {
                // SAMLRequest と SAMLResponse の両方・どちらも無い
                return "";
            }

            string saml = string.IsNullOrEmpty(request) ? response : request;

            byte[] tempByte = DeflateCompression.Decompress(
                CustomEncode.FromBase64String(CustomEncode.UrlDecode(saml)));

            //// XML宣言部分を取得するために、us_asciiでデコード
            //string tempString = CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii);

            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(tempString);

            return CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii); // enc.CodePage);
        }

        /// <summary>VerifyRedirect</summary>
        /// <param name="queryString">string</param>
        /// <param name="ds">検証に使う DigitalSign</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// 渡された DigitalSign のアルゴリズムで検証する（アルゴリズムは呼び出し側が決める）。
        /// SigAlg が、そのアルゴリズムを示していなければ失敗にする（#604 の案 (2)）。
        /// 2026/10/12 まで SigAlg を見ておらず、以前の EncodeAndSignRedirect はココで失敗する可能性がある。
        /// SigAlg を決められないアルゴリズム（EncodeAndSignRedirect が例外にするもの）は、失敗にする。
        /// RSA-PSS・ECDSA は、2026/10/30 から RFC 9231 の URI と照合する。**それより前の版で作った**
        /// （rsa-sha1 と書いた）クエリは、ココで失敗する（#643）。
        /// SigAlg に従って検証するなら、RSA を受けるオーバーロードを使う（#604 の案 (3)）。
        /// </remarks>
        public static bool VerifyRedirect(string queryString, DigitalSign ds)
        {
            // EcodeRedirectの逆

            // 署名の対象・SigAlg・Signature を取り出す（SAML Bindings 3.4.4.1。#649）
            string[] signedOctets;
            string sigAlg, signature;
            if (!SAML2Bindings.TryParseSignedRedirect(queryString, out signedOctets, out sigAlg, out signature))
            {
                return false;
            }

            // SigAlgの確認（#604）
            string expectedSigAlg = SAML2Bindings.GetSigAlg(ds);
            if (string.IsNullOrEmpty(expectedSigAlg) || sigAlg != expectedSigAlg)
            {
                return false;
            }

            // 署名の対象 : ASCIIデコード（候補は、空の RelayState= があるときだけ 2 つ。TryParseSignedRedirect）
            // signature   : パラメタ → URLデコード →  Base64デコード
            byte[] sign = CustomEncode.FromBase64String(CustomEncode.UrlDecode(signature));
            foreach (string candidate in signedOctets)
            {
                if (ds.Verify(CustomEncode.StringToByte(candidate, CustomEncode.us_ascii), sign))
                {
                    // 署名検証 OK
                    return true;
                }
            }

            // 署名検証 NG
            return false;
        }

        /// <summary>VerifyRedirect</summary>
        /// <param name="queryString">string</param>
        /// <param name="rsa">検証に使う RSA の公開鍵</param>
        /// <param name="sigAlgs">
        /// 受け入れる SigAlg（SAML2Const.RSAwithSHA1 / 256 / 384 / 512、RSAPSSwithSHA256 / 384 / 512 から選ぶ）。
        /// null なら、RSAwithSHA256 / 384 / 512（SHA1 と RSA-PSS は受け入れない）。
        /// </param>
        /// <returns>bool</returns>
        /// <remarks>
        /// SigAlg からハッシュとパディングを選び、RSA（PKCS#1 v1.5、または RSA-PSS）で検証する（#604 の案 (3)）。
        /// **RSA-PSS は、既定では受け入れない**。受けるなら sigAlgs に RSAPSSwithSHA256 などを並べる（#643）。
        /// SigAlg は署名の検証前に読む値なので、sigAlgs に無い値なら検証せずに失敗にする。
        /// 相手（IdP / SP）ごとにハッシュが違うときに使う。
        /// </remarks>
        public static bool VerifyRedirect(string queryString, RSA rsa, string[] sigAlgs = null)
        {
            if (sigAlgs == null)
            {
                sigAlgs = new string[] {
                    SAML2Const.RSAwithSHA256, SAML2Const.RSAwithSHA384, SAML2Const.RSAwithSHA512 };
            }

            // 署名の対象・SigAlg・Signature を取り出す（SAML Bindings 3.4.4.1。#649）
            string[] signedOctets;
            string sigAlg, signature;
            if (!SAML2Bindings.TryParseSignedRedirect(queryString, out signedOctets, out sigAlg, out signature))
            {
                return false;
            }

            // SigAlgの確認
            if (Array.IndexOf(sigAlgs, sigAlg) == -1)
            {
                return false;
            }

            HashAlgorithmName han;
            RSASignaturePadding padding = RSASignaturePadding.Pkcs1;
            if (sigAlg == SAML2Const.RSAwithSHA1) han = HashAlgorithmName.SHA1;
            else if (sigAlg == SAML2Const.RSAwithSHA256) han = HashAlgorithmName.SHA256;
            else if (sigAlg == SAML2Const.RSAwithSHA384) han = HashAlgorithmName.SHA384;
            else if (sigAlg == SAML2Const.RSAwithSHA512) han = HashAlgorithmName.SHA512;
            // RSA-PSS（RFC 9231 2.3.10。ソルト長はハッシュ長で、RSASignaturePadding.Pss と同じ。#643）
            else if (sigAlg == SAML2Const.RSAPSSwithSHA256) { han = HashAlgorithmName.SHA256; padding = RSASignaturePadding.Pss; }
            else if (sigAlg == SAML2Const.RSAPSSwithSHA384) { han = HashAlgorithmName.SHA384; padding = RSASignaturePadding.Pss; }
            else if (sigAlg == SAML2Const.RSAPSSwithSHA512) { han = HashAlgorithmName.SHA512; padding = RSASignaturePadding.Pss; }
            else return false; // RSA 以外

            // 署名の対象 : ASCIIデコード（候補は、空の RelayState= があるときだけ 2 つ。TryParseSignedRedirect）
            // signature   : パラメタ → URLデコード →  Base64デコード
            byte[] sign = CustomEncode.FromBase64String(CustomEncode.UrlDecode(signature));
            foreach (string candidate in signedOctets)
            {
                if (rsa.VerifyData(CustomEncode.StringToByte(candidate, CustomEncode.us_ascii), sign, han, padding))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Redirect Binding のクエリ文字列から、署名に関わるパラメタ（SAMLRequest・SAMLResponse・RelayState・SigAlg・Signature）を読む
        /// </summary>
        /// <param name="queryString">受け取ったクエリ文字列（URL エンコードされたまま。先頭の "?" は在ってもよい）</param>
        /// <param name="raw">パラメタ名 → 受け取ったままの（URL エンコードされた）値</param>
        /// <returns>読めたら true。曖昧なら false</returns>
        /// <remarks>
        /// **署名の検証（TryParseSignedRedirect）と SAML の取り出し（DecodeRedirect）は、必ずこの関数で読む**（#649）。
        /// 読み方が別だと、署名を確かめた値と、実際に使う値が食い違いうる（QUALITY.md 4 節「署名の対象と、実際に使う要素の一致」）。
        /// - パラメタ名は、"&amp;" で区切った各組の "=" の前を、**完全一致**で見る（部分一致にしない）
        /// - **署名に関わるパラメタの重複は false**（どれを使うかが曖昧）
        /// - **URL デコードすると署名に関わる名前になる、エンコードされた名前も false**（受け手の Web フレームワークは名前をデコードするので、読み方が食い違う）
        /// - ほかのパラメタは無視する（署名の対象ではない。SAML 2.0 Bindings 3.4.4.1）
        /// </remarks>
        internal static bool TryReadRedirectParameters(string queryString, out Dictionary<string, string> raw)
        {
            raw = new Dictionary<string, string>(StringComparer.Ordinal);

            if (string.IsNullOrEmpty(queryString))
            {
                return false;
            }

            string query = queryString.StartsWith("?") ? queryString.Substring(1) : queryString;

            string[] names = new string[] {
                SAML2Const.QueryParamSAMLRequest, SAML2Const.QueryParamSAMLResponse,
                SAML2Const.QueryParamRelayState, SAML2Const.QueryParamSigAlg, SAML2Const.QueryParamSignature };

            foreach (string pair in query.Split('&'))
            {
                int index = pair.IndexOf('=');
                string name = (index < 0) ? pair : pair.Substring(0, index);
                string value = (index < 0) ? "" : pair.Substring(index + 1);

                if (Array.IndexOf(names, name) == -1)
                {
                    if (Array.IndexOf(names, CustomEncode.UrlDecode(name)) != -1)
                    {
                        // エンコードされた名前で、署名に関わるパラメタを名乗っている（読み方が食い違う）
                        return false;
                    }

                    // 署名の対象でないパラメタは無視する（3.4.4.1）
                    continue;
                }

                if (raw.ContainsKey(name))
                {
                    // 重複（どれを使うかが曖昧）
                    return false;
                }

                raw.Add(name, value);
            }

            return true;
        }

        /// <summary>
        /// Redirect Binding のクエリ文字列から、署名の対象（オクテット列にする文字列）・SigAlg・Signature を取り出す
        /// </summary>
        /// <param name="queryString">受け取ったクエリ文字列（URL エンコードされたまま。先頭の "?" は在ってもよい）</param>
        /// <param name="signedOctets">
        /// 署名の対象の候補（SAMLRequest か SAMLResponse、RelayState（在れば）、SigAlg をこの順に "&amp;" でつないだもの）。
        /// 通常は 1 つ。値が空の RelayState= が付いているときだけ、含めない版（仕様どおり）と含める版（互換）の 2 つ
        /// </param>
        /// <param name="sigAlg">SigAlg（URL デコードしたもの）</param>
        /// <param name="signature">Signature（URL エンコードされたまま）</param>
        /// <returns>取り出せたら true</returns>
        /// <remarks>
        /// **署名の対象は、SAMLRequest（または SAMLResponse）・RelayState・SigAlg の 3 つだけ**から、
        /// 受け取ったままの（URL エンコードされた）値で組み立てる（SAML 2.0 Bindings 3.4.4.1。#649）。
        /// - **ほかのパラメタは無視する。** 2026/11/05 までは、クエリ文字列の全体から Signature だけを除いたものを署名の対象にしていたので、
        ///   SP 側の画面遷移が付けたパラメタ（Open棟梁 の Web Forms の BrowserWindowGUID など）があると、正しい署名でも失敗していた
        /// - **RelayState の値が無ければ、パラメタ全体を署名の演算から除外する**（SAML 2.0 Bindings 3.4.4.1。
        ///   空のパラメタとして含めてはならない。EncodeAndSignRedirect も含めない）。
        ///   ただし、**仕様に反して空の "RelayState=" を含めて署名する相手との互換のため、含める版も候補にする**（どちらかで通れば受ける）。
        ///   どちらの候補も、SAMLRequest（SAMLResponse）・SigAlg を含む全体に署名が掛かっているので、偽造は通らない。
        ///   2026/11/05 までの版（クエリ文字列の全体を署名の対象にした）は、空の RelayState= を含めて照合していた
        /// - SAMLRequest と SAMLResponse の両方・どちらも無い、署名に関わるパラメタの重複、SigAlg・Signature が無いものは false
        /// </remarks>
        private static bool TryParseSignedRedirect(
            string queryString, out string[] signedOctets, out string sigAlg, out string signature)
        {
            signedOctets = null;
            sigAlg = null;
            signature = null;

            Dictionary<string, string> raw;
            if (!SAML2Bindings.TryReadRedirectParameters(queryString, out raw))
            {
                return false;
            }

            string request, response, relayState, rawSigAlg, rawSignature;
            raw.TryGetValue(SAML2Const.QueryParamSAMLRequest, out request);
            raw.TryGetValue(SAML2Const.QueryParamSAMLResponse, out response);
            raw.TryGetValue(SAML2Const.QueryParamRelayState, out relayState);
            raw.TryGetValue(SAML2Const.QueryParamSigAlg, out rawSigAlg);
            raw.TryGetValue(SAML2Const.QueryParamSignature, out rawSignature);

            if (string.IsNullOrEmpty(request) == string.IsNullOrEmpty(response) ||
                string.IsNullOrEmpty(rawSigAlg) || string.IsNullOrEmpty(rawSignature))
            {
                // SAMLRequest と SAMLResponse の両方・どちらも無い、SigAlg・Signature が無い
                return false;
            }

            string message = string.IsNullOrEmpty(request)
                ? SAML2Const.QueryParamSAMLResponse + "=" + response
                : SAML2Const.QueryParamSAMLRequest + "=" + request;
            string tail = "&" + SAML2Const.QueryParamSigAlg + "=" + rawSigAlg;

            if (!string.IsNullOrEmpty(relayState))
            {
                signedOctets = new string[] {
                    message + "&" + SAML2Const.QueryParamRelayState + "=" + relayState + tail };
            }
            else if (relayState != null)
            {
                // 値が空の RelayState=：含めない版（仕様どおり）と、含める版（互換）
                signedOctets = new string[] {
                    message + tail,
                    message + "&" + SAML2Const.QueryParamRelayState + "=" + tail };
            }
            else
            {
                signedOctets = new string[] { message + tail };
            }

            sigAlg = CustomEncode.UrlDecode(rawSigAlg);
            signature = rawSignature;
            return true;
        }

        /// <summary>DigitalSign のアルゴリズムから、SigAlg の値を決める（#604・#643）</summary>
        /// <param name="ds">DigitalSign</param>
        /// <returns>SigAlg（RFC 9231 の URI。決められなければ null）</returns>
        /// <remarks>
        /// 2026/10/30 まで、決められないもの（RSA-PSS・ECDSA を含む）は rsa-sha1 を返していた（#643）。
        /// </remarks>
        private static string GetSigAlg(DigitalSign ds)
        {
            if (ds.AsymmetricAlgorithm is RSA && ds.Padding == RSASignaturePadding.Pkcs1)
            {
                string hashName = ds.HashAlgorithmName.Name;

                if (hashName == HashAlgorithmName.SHA1.Name) return SAML2Const.RSAwithSHA1;
                if (hashName == HashAlgorithmName.SHA256.Name) return SAML2Const.RSAwithSHA256;
                if (hashName == HashAlgorithmName.SHA384.Name) return SAML2Const.RSAwithSHA384;
                if (hashName == HashAlgorithmName.SHA512.Name) return SAML2Const.RSAwithSHA512;
            }
            else if (ds.AsymmetricAlgorithm is RSA && ds.Padding == RSASignaturePadding.Pss)
            {
                // RSA-PSS（RFC 9231 2.3.10）
                string hashName = ds.HashAlgorithmName.Name;

                if (hashName == HashAlgorithmName.SHA256.Name) return SAML2Const.RSAPSSwithSHA256;
                if (hashName == HashAlgorithmName.SHA384.Name) return SAML2Const.RSAPSSwithSHA384;
                if (hashName == HashAlgorithmName.SHA512.Name) return SAML2Const.RSAPSSwithSHA512;
            }
            else if (ds.AsymmetricAlgorithm is ECDsa || ds is DigitalSignECDsaCng)
            {
                // ECDSA（RFC 9231 2.3.6）。
                // DigitalSignECDsaCng の公開鍵だけの版は AsymmetricAlgorithm を持たないので、型でも見る。
                // ハッシュを渡していなければ、ECDsaCng の既定（SHA-256）で署名している。
                string hashName = (ds.HashAlgorithm == null)
                    ? HashAlgorithmName.SHA256.Name : ds.HashAlgorithmName.Name;

                if (hashName == HashAlgorithmName.SHA256.Name) return SAML2Const.ECDSAwithSHA256;
                if (hashName == HashAlgorithmName.SHA384.Name) return SAML2Const.ECDSAwithSHA384;
                if (hashName == HashAlgorithmName.SHA512.Name) return SAML2Const.ECDSAwithSHA512;
            }
            else if (ds.AsymmetricAlgorithm is DSA)
            {
                // DigitalSign.Sign の DSA は SHA1
                return SAML2Const.DSAwithSHA1;
            }

            // 上記以外は、決められない（呼び出し側が、例外か失敗にする）
            return null;
        }

        #endregion

        #region HTTP-POST（符号化・署名 / 復号・検証）

        /// <summary>EncodeAndSignPost</summary>
        /// <param name="saml">string</param>
        /// <param name="referenceId">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>RedirectPost用SAML文字列</returns>
        public static string EncodeAndSignPost(string saml, string referenceId = "", RSA rsa = null)
        {
            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(saml);

            if (rsa == null)
            {
                // 署名しない
            }
            else
            {
                // 署名する
                SignedXml2 signedXml2 = new SignedXml2(rsa);
                saml = signedXml2.Create(saml, referenceId).OuterXml;
            }

            // XML (→ XML宣言のエンコーディングではなく、asciiエンコーディングに変更) → Base64エンコード
            return CustomEncode.ToBase64String(CustomEncode.StringToByte(
                saml, CustomEncode.us_ascii)); //enc.CodePage));
        }

        /// <summary>DecodePost</summary>
        /// <param name="saml">エンコードされたsaml</param>
        /// <returns>デコードされたsaml</returns>
        public static string DecodePost(string saml)
        {
            // EncodePostの逆

            // Base64エンコード → XML宣言のエンコーディング → XML
            byte[] tempByte = CustomEncode.FromBase64String(saml);

            //// XML宣言部分を取得するために、us_asciiでデコード
            //string tempString = CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii);

            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(tempString);

            return CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii); // enc.CodePage);
        }

        /// <summary>DecodePost</summary>
        /// <param name="saml">デコードされたsaml</param>
        /// <param name="referenceId">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>bool</returns>
        public static bool VerifyPost(string saml, string referenceId, RSA rsa)
        {
            SignedXml2 signedXml2 = new SignedXml2(rsa);
            return signedXml2.Verify(saml, referenceId);
        }

        #endregion
    }
}
