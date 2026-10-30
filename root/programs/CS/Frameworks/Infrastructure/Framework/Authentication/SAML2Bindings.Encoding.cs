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
//**********************************************************************************

using System;
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
        /// <returns>デコードされたsaml</returns>
        public static string DecodeRedirect(string queryString)
        {
            // EcodeRedirectの逆
            // --------------------------------------------------
            // Saml → URLデコード → Base64デコード
            //   → DEFLATE解凍 → XML宣言のエンコーディング → XML
            // --------------------------------------------------
            // Samlの抽出
            string saml = "";
            if (queryString.IndexOf(SAML2Const.QueryParamSAMLRequest) != -1)
            {
                saml = UriOperator.GetParameterFromQueryString(SAML2Const.QueryParamSAMLRequest, queryString);
            }
            else if (queryString.IndexOf(SAML2Const.QueryParamSAMLResponse) != -1)
            {
                saml = UriOperator.GetParameterFromQueryString(SAML2Const.QueryParamSAMLResponse, queryString);
            }
            else
            {
                return "";
            }

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

            // SigAlgの確認（#604）
            string sigAlg = CustomEncode.UrlDecode(
                UriOperator.GetParameterFromQueryString(SAML2Const.QueryParamSigAlg, queryString));
            string expectedSigAlg = SAML2Bindings.GetSigAlg(ds);
            if (string.IsNullOrEmpty(expectedSigAlg) || sigAlg != expectedSigAlg)
            {
                return false;
            }

            // Signatureの抽出
            string signature = UriOperator.GetParameterFromQueryString(SAML2Const.QueryParamSignature, queryString);
            // Signatureの削除
            queryString = queryString.Replace("&" + SAML2Const.QueryParamSignature + "=" + signature, "");

            // queryString : ASCIIデコード
            // signature   : パラメタ → URLデコード →  Base64デコード
            if (ds.Verify(
                CustomEncode.StringToByte(queryString, CustomEncode.us_ascii),
                CustomEncode.FromBase64String(CustomEncode.UrlDecode(signature))))
            {
                // 署名検証 OK
                return true;
            }
            else
            {
                // 署名検証 NG
                return false;
            }
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

            // SigAlgの確認
            string sigAlg = CustomEncode.UrlDecode(
                UriOperator.GetParameterFromQueryString(SAML2Const.QueryParamSigAlg, queryString));
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

            // Signatureの抽出
            string signature = UriOperator.GetParameterFromQueryString(SAML2Const.QueryParamSignature, queryString);
            // Signatureの削除
            queryString = queryString.Replace("&" + SAML2Const.QueryParamSignature + "=" + signature, "");

            // queryString : ASCIIデコード
            // signature   : パラメタ → URLデコード →  Base64デコード
            return rsa.VerifyData(
                CustomEncode.StringToByte(queryString, CustomEncode.us_ascii),
                CustomEncode.FromBase64String(CustomEncode.UrlDecode(signature)),
                han, padding);
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
