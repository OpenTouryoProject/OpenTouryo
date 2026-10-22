//**********************************************************************************
//* Copyright (C) 2017 Hitachi Solutions,Ltd.
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
//* クラス名        ：SAML2Client
//* クラス日本語名  ：SAML2Client（ライブラリ）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/04  西野 大介         新規作成
//*  2026/10/07  玄人 幸道         VerifyResponseのout statusCodeを、検証の前に設定するようにした（#598）。
//*                                検証に失敗するとnullのままで、呼び出し側が理由を切り分けられなかった。
//*  2026/10/11  玄人 幸道         VerifyResponseに、検証用の証明書（*.cer）のパスを受けるオーバーロードを追加（#600）。
//*                                CmnClientParams.RsaCerFilePath固定で、自分以外のIdPの応答を検証できなかった。
//*  2026/10/12  玄人 幸道         VerifyResponseのhashAlgorithmNameに、SigAlgとの照合の説明を追加（#604）
//*  2026/10/14  玄人 幸道         CreateRedirectRequest / CreatePostRequestに、署名に使う鍵を受けるオーバーロードを追加（#606）。
//*                                CmnClientParams.RsaPfxFilePath固定で、相手（IdP）ごとに別の鍵で署名できなかった。
//*  2026/10/22  玄人 幸道         理由を返すVerifyResponseWithReasonを追加（汎用認証サイトから移した。#634 の C-3）
//*  2026/10/22  玄人 幸道         SigAlgの直書きをSAML2Const.QueryParamSigAlgに寄せた
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Client（ライブラリ）</summary>
    public class SAML2Client
    {
        #region CreateRequest
        /// <summary>CreateRedirectRequest</summary>
        /// <param name="createRoR">SAML2Enum.RequestOrResponse</param>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="iss">string</param>
        /// <param name="assertionConsumerService">string</param>
        /// <param name="relayState">string</param>
        /// <param name="id">out string</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <returns>queryString</returns>
        /// <remarks>
        /// 署名には CmnClientParams.RsaPfxFilePath の鍵を使う。
        /// 相手（IdP）ごとに鍵を替える場合は、digitalSign を受けるオーバーロードを使う（#606）。
        /// </remarks>
        public static string CreateRedirectRequest(
            SAML2Enum.RequestOrResponse createRoR,
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState, out string id,
            HashAlgorithmName? hashAlgorithmName = null) // 既定値の変更 SHA1 → SHA256
        {
            // DigitalSignX509
            HashAlgorithmName han = hashAlgorithmName ?? HashAlgorithmName.SHA256;
            DigitalSignX509 dsX509 = new DigitalSignX509(
                CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword, han);

            return SAML2Client.CreateRedirectRequest(
                createRoR, protocolBinding, nameIDFormat,
                iss, assertionConsumerService, relayState, dsX509, out id);
        }

        /// <summary>CreateRedirectRequest</summary>
        /// <param name="createRoR">SAML2Enum.RequestOrResponse</param>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="iss">string</param>
        /// <param name="assertionConsumerService">string</param>
        /// <param name="relayState">string</param>
        /// <param name="digitalSign">
        /// 署名に使う DigitalSign（自分の秘密鍵）。SigAlg も、この DigitalSign のアルゴリズムから決まる。
        /// null なら、CmnClientParams.RsaPfxFilePath の鍵と SHA256 を使う。
        /// </param>
        /// <param name="id">out string</param>
        /// <returns>queryString</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// SAML2Bindings.EncodeAndSignRedirect は null なら署名しないが、
        /// こちらは null でも署名する（設定の鍵を使う。#606）。
        /// </remarks>
        public static string CreateRedirectRequest(
            SAML2Enum.RequestOrResponse createRoR,
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState,
            DigitalSign digitalSign, out string id)
        {
            // 鍵の指定が無ければ、従来どおり設定の1本を使う（#606）。
            if (digitalSign == null)
            {
                digitalSign = new DigitalSignX509(
                    CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword, HashAlgorithmName.SHA256);
            }

            // SamlRequestの生成
            string samlRequest = SAML2Bindings.CreateRequest(
                iss, protocolBinding, nameIDFormat,
                assertionConsumerService, out id).OuterXml;

            // SamlRequestのエンコと、QueryStringを生成（ + 署名）
            return SAML2Bindings.EncodeAndSignRedirect(
                createRoR, samlRequest, relayState, digitalSign);
        }

        /// <summary>CreatePostRequest</summary>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="iss">string</param>
        /// <param name="assertionConsumerService">string</param>
        /// <param name="relayState">string</param>
        /// <param name="id">out string</param>
        /// <returns>samlRequest</returns>
        /// <remarks>
        /// 署名には CmnClientParams.RsaPfxFilePath の鍵を使う。
        /// 相手（IdP）ごとに鍵を替える場合は、rsa を受けるオーバーロードを使う（#606）。
        /// </remarks>
        public static string CreatePostRequest(
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState, out string id)
        {
            return SAML2Client.CreatePostRequest(
                protocolBinding, nameIDFormat,
                iss, assertionConsumerService, relayState, null, out id);
        }

        /// <summary>CreatePostRequest</summary>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="iss">string</param>
        /// <param name="assertionConsumerService">string</param>
        /// <param name="relayState">string</param>
        /// <param name="rsa">
        /// 署名に使う RSA（自分の秘密鍵）。
        /// null なら、CmnClientParams.RsaPfxFilePath の鍵を使う。
        /// </param>
        /// <param name="id">out string</param>
        /// <returns>samlRequest</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// SAML2Bindings.EncodeAndSignPost は null なら署名しないが、
        /// こちらは null でも署名する（設定の鍵を使う。#606）。
        /// </remarks>
        public static string CreatePostRequest(
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState,
            RSA rsa, out string id)
        {
            // 鍵の指定が無ければ、従来どおり設定の1本を使う（#606）。
            if (rsa == null)
            {
#if NETSTD
                X509Certificate2 x509 = X509CertificateLoader.LoadPkcs12FromFile(CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword);
#else
                X509Certificate2 x509 = new X509Certificate2(CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword);
#endif
                rsa = x509.GetRSAPrivateKey();
            }

            // SamlRequestの生成
            string samlRequest = SAML2Bindings.CreateRequest(
                iss, protocolBinding, nameIDFormat,
                assertionConsumerService, out id).OuterXml;

            // SamlRequestのエンコと署名
            return SAML2Bindings.EncodeAndSignPost(samlRequest, id, rsa);
        }
        #endregion

        #region VerifyResponse

        /// <summary>VerifyResponse</summary>
        /// <param name="queryString">string</param>
        /// <param name="samlResponse">string</param>
        /// <param name="nameId">out string</param>
        /// <param name="iss">out string</param>
        /// <param name="aud">out string</param>
        /// <param name="inResponseTo">out string</param>
        /// <param name="recipient">out string</param>
        /// <param name="notOnOrAfter">out DateTime</param>
        /// <param name="statusCode">SAML2Enum.StatusCode</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="samlResponse2">XmlDocument</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// 署名の検証には CmnClientParams.RsaCerFilePath の証明書を使う。
        /// 相手（IdP）ごとに証明書を替える場合は、cerFilePath を受けるオーバーロードを使う（#600）。
        ///
        /// statusCode は、応答を読めた時点（署名・構造の検証より前）で設定する（#598）。
        /// false が返ったときに、呼び出し側が理由を切り分けるためである。
        ///
        /// したがって、false のときの statusCode は検証されていない値である。
        /// 表示・ログによる切り分けにだけ使い、認証の判断には戻り値を使うこと。
        /// </remarks>
        public static bool VerifyResponse(
            string queryString, string samlResponse,
            out string nameId, out string iss, out string aud,
            out string inResponseTo, out string recipient, out DateTime? notOnOrAfter,
            out SAML2Enum.StatusCode? statusCode,
            out SAML2Enum.NameIDFormat? nameIDFormat, 
            out SAML2Enum.AuthnContextClassRef? authnContextClassRef,
            out XmlDocument samlResponse2,
            HashAlgorithmName? hashAlgorithmName = null) // 既定値の変更 SHA1 → SHA256
        {
            return SAML2Client.VerifyResponse(
                queryString, samlResponse, null,
                out nameId, out iss, out aud,
                out inResponseTo, out recipient, out notOnOrAfter,
                out statusCode, out nameIDFormat, out authnContextClassRef,
                out samlResponse2, hashAlgorithmName);
        }

        /// <summary>VerifyResponse</summary>
        /// <param name="queryString">string</param>
        /// <param name="samlResponse">string</param>
        /// <param name="cerFilePath">
        /// 署名の検証に使う、相手（IdP）のX.509証明書(*.cer)へのパス。
        /// null か空文字なら、CmnClientParams.RsaCerFilePath を使う。
        /// </param>
        /// <param name="nameId">out string</param>
        /// <param name="iss">out string</param>
        /// <param name="aud">out string</param>
        /// <param name="inResponseTo">out string</param>
        /// <param name="recipient">out string</param>
        /// <param name="notOnOrAfter">out DateTime</param>
        /// <param name="statusCode">SAML2Enum.StatusCode</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="samlResponse2">XmlDocument</param>
        /// <param name="hashAlgorithmName">
        /// HashAlgorithmName（Redirect の署名の検証に使う）。
        /// Redirect の SigAlg が、このハッシュの RSA を示していなければ失敗する（#604）。
        /// POST の署名は、XML の SignatureMethod に従って検証する。
        /// </param>
        /// <returns>bool</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// 検証できるのは「cerFilePath の証明書で署名されていること」までである。
        /// その証明書の持ち主と、out iss が一致することは、呼び出し側で照合すること（#600）。
        ///
        /// statusCode は、応答を読めた時点（署名・構造の検証より前）で設定する（#598）。
        /// false が返ったときに、呼び出し側が理由を切り分けるためである。
        ///
        /// したがって、false のときの statusCode は検証されていない値である。
        /// 表示・ログによる切り分けにだけ使い、認証の判断には戻り値を使うこと。
        /// </remarks>
        public static bool VerifyResponse(
            string queryString, string samlResponse, string cerFilePath,
            out string nameId, out string iss, out string aud,
            out string inResponseTo, out string recipient, out DateTime? notOnOrAfter,
            out SAML2Enum.StatusCode? statusCode,
            out SAML2Enum.NameIDFormat? nameIDFormat,
            out SAML2Enum.AuthnContextClassRef? authnContextClassRef,
            out XmlDocument samlResponse2,
            HashAlgorithmName? hashAlgorithmName = null)
        {
            bool verified = false;

            // out string
            nameId = "";
            iss = "";
            aud = "";

            inResponseTo = "";
            recipient = "";

            // out DateTime?
            notOnOrAfter = null;

            // out SAML2Enum
            statusCode = null;
            nameIDFormat = null;
            authnContextClassRef = null;

            // out XmlDocument
            samlResponse2 = null;

#region 準備
            // Decode
            string decodeSaml = "";
            if (!string.IsNullOrEmpty(queryString))
            {
                decodeSaml = SAML2Bindings.DecodeRedirect(queryString);
            }
            else if (!string.IsNullOrEmpty(samlResponse))
            {
                decodeSaml = SAML2Bindings.DecodePost(samlResponse);
            }
            else
            {
                return false;
            }

            // XmlDocument
            samlResponse2 = new XmlDocument();
            samlResponse2.PreserveWhitespace = false;
            samlResponse2.LoadXml(decodeSaml);

            // XmlNamespaceManager
            XmlNamespaceManager samlNsMgr = SAML2Bindings.CreateNamespaceManager(samlResponse2);

            // StatusCode
            // 検証に失敗しても呼び出し側が理由を切り分けられるよう、検証の前に設定する（#598）。
            SAML2Enum.StringToEnum(
                SAML2Bindings.GetStatusCodeInResponse(
                    samlResponse2, samlNsMgr), out statusCode);
#endregion

#region 検証
            // Metadata利用を検討
            // 証明書の指定が無ければ、従来どおり設定の1本を使う（#600）。
            if (string.IsNullOrEmpty(cerFilePath))
            {
                cerFilePath = CmnClientParams.RsaCerFilePath;
            }

            HashAlgorithmName han = hashAlgorithmName ?? HashAlgorithmName.SHA256;
            DigitalSignX509 dsX509 = new DigitalSignX509(cerFilePath, "", han);

            if (!string.IsNullOrEmpty(queryString))
            {
                // VerifyRedirect
                if (SAML2Bindings.VerifyRedirect(queryString, dsX509))
                {
                    // XPathによる検証
                    verified = SAML2Bindings.VerifyByXPath(
                        samlResponse2, SAML2Enum.SamlSchema.Response, samlNsMgr);
                }
            }
            else
            {
                // VerifyPost
                string id = SAML2Bindings.GetIdInResponse(samlResponse2, samlNsMgr);
                if (SAML2Bindings.VerifyPost(decodeSaml, id, dsX509.X509Certificate.GetRSAPublicKey()))
                {
                    // XPathによる検証
                    verified = SAML2Bindings.VerifyByXPath(
                        samlResponse2, SAML2Enum.SamlSchema.Response, samlNsMgr);
                }
            }
#endregion

            // 値のチェック
            if (verified)
            {
                string temp1 = "";
                string temp2 = "";

                // StatusCode（準備の時点で設定済み）
                if (statusCode == SAML2Enum.StatusCode.Success)
                {
                    // iss
                    temp1 = SAML2Bindings.GetIssuerInResponse(samlResponse2, samlNsMgr);
                    temp2 = SAML2Bindings.GetIssuerInAssertion(samlResponse2, samlNsMgr);

                    if (temp1 == temp2)
                    {
                        iss = temp1;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(temp1))
                        {
                            // temp2のみ値がある
                            if (!string.IsNullOrEmpty(temp2)) iss = temp2;
                        }
                        else if (string.IsNullOrEmpty(temp2))
                        {
                            // temp1のみ値がある
                            if (!string.IsNullOrEmpty(temp1)) iss = temp1;
                        }
                        else
                        {
                            // Assertionは使用できない。
                            return false;
                        } 
                    }

                    // NameID
                    string format = "";
                    SAML2Bindings.GetNameIDInAssertion(
                        samlResponse2, samlNsMgr, out format, out nameId);
                    SAML2Enum.StringToEnum(format, out nameIDFormat);

                    // SubjectConfirmationData
                    string _inResponseTo = "";
                    string _notOnOrAfter = "";
                    XmlNode subjectConfirmationData = 
                        SAML2Bindings.GetSubjectConfirmationDataInAssertion(
                            samlResponse2, samlNsMgr, out _inResponseTo, out _notOnOrAfter, out recipient);

                    temp1 = _inResponseTo;
                    temp2 = SAML2Bindings.GetInResponseToInResponse(samlResponse2, samlNsMgr);

                    if (temp1 == temp2)
                    {
                        inResponseTo = temp1;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(temp1))
                        {
                            // temp2のみ値がある
                            if (!string.IsNullOrEmpty(temp2)) inResponseTo = temp2;
                        }
                        else if (string.IsNullOrEmpty(temp2))
                        {
                            // temp1のみ値がある
                            if (!string.IsNullOrEmpty(temp1)) inResponseTo = temp1;
                        }
                        else
                        {
                            // Assertionは使用できない。
                            return false;
                        }
                    }

                    // Conditions

                    // - aud
                    aud = SAML2Bindings.GetAudienceInAssertion(samlResponse2, samlNsMgr);

                    // - notOnOrAfter
                    temp1 = _notOnOrAfter;
                    temp2 = SAML2Bindings.GetConditionsNotOnOrAfterInAssertion(samlResponse2, samlNsMgr);
                    DateTime time1 = DateTime.MinValue;
                    DateTime time2 = DateTime.MinValue;

                    if (temp1 == temp2)
                    {
                        // 等しい場合
                        notOnOrAfter = FormatConverter.FromW3cTimestamp(temp1);
                    }
                    else
                    {
                        // 等しくない場合
                        time1 = FormatConverter.FromW3cTimestamp(temp1);
                        time2 = FormatConverter.FromW3cTimestamp(temp2);

                        // 短い値を使用
                        if (time1.Ticks <= time2.Ticks)
                        {
                            notOnOrAfter = time1;
                        }
                        else
                        {
                            notOnOrAfter = time2;
                        }
                    }

                    // 現在時刻と比較
                    if (!(DateTime.UtcNow.Ticks <= notOnOrAfter.Value.Ticks))
                    {
                        return false; // Assertionは使用できない。
                    }

                    // AuthnContextClassRef
                    SAML2Enum.StringToEnum(
                        SAML2Bindings.GetAuthnContextClassRefInAssertion(
                        samlResponse2, samlNsMgr), out authnContextClassRef);
                }
                else
                {
                    return false; // Assertionは使用できない。
                }
            }

            return verified;
        }

        /// <summary>VerifyResponseWithReason（理由を返す応答の検証）</summary>
        /// <param name="queryString">string（Redirect Binding のときのクエリ文字列。POST なら null か空文字）</param>
        /// <param name="samlResponse">string（POST Binding のときの SAMLResponse）</param>
        /// <param name="rsaKeys">IEnumerable（署名を検証する相手の鍵。ロールオーバー中は複数。SAML2Metadata.GetSigningKeys）</param>
        /// <param name="expectedIssuer">string（期待する Issuer＝相手の entityID）</param>
        /// <param name="expectedAcsUrl">string（自分の ACS URL。Audience と Recipient に照合する。空なら照合しない）</param>
        /// <param name="expectedInResponseTo">string（送った AuthnRequest の ID。空なら照合しない）</param>
        /// <param name="relayState">string（受け取った RelayState）</param>
        /// <param name="expectedRelayState">string（送った state。空なら照合しない）</param>
        /// <param name="sigAlgs">string[]（Redirect で受け入れる SigAlg。null なら SHA256 / 384 / 512）</param>
        /// <returns>SAML2VerifyResult</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// VerifyResponse は bool しか返さず、落ちた理由が分からない（#634 の C-3）。
        /// こちらは確かめた項目ごとの結果と、判定の理由を返す。
        /// 判定の中身（署名・スキーマ・食い違い・期限・StatusCode）は VerifyResponse と同じで、
        /// それに Issuer / Audience / Recipient / InResponseTo / RelayState の照合を足している。
        ///
        /// - 鍵が複数なら、署名が通るまで順に試す（SAML2 の署名には kid に当たるものが無い）
        /// - Redirect の SigAlg は sigAlgs から選ぶ（SAML2Bindings.VerifyRedirect の RSA の版）
        /// - 外から来た値なので、読めなくても例外にせず、結果を返す
        /// - **認証の判断には IsValid だけを使うこと**
        /// </remarks>
        public static SAML2VerifyResult VerifyResponseWithReason(
            string queryString, string samlResponse, IEnumerable<RSA> rsaKeys,
            string expectedIssuer, string expectedAcsUrl, string expectedInResponseTo,
            string relayState, string expectedRelayState, string[] sigAlgs = null)
        {
            SAML2VerifyResult ret = new SAML2VerifyResult();

            ret.IsRedirect = !string.IsNullOrEmpty(queryString);
            ret.RelayState = relayState ?? "";
            ret.RelayStateMatched = string.IsNullOrEmpty(expectedRelayState)
                ? (bool?)null : (relayState == expectedRelayState);

            if (!ret.IsRedirect && string.IsNullOrEmpty(samlResponse))
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.NoResponse, "There is no SAMLResponse.");
            }

            List<RSA> keys = new List<RSA>();
            if (rsaKeys != null)
            {
                foreach (RSA rsa in rsaKeys)
                {
                    if (rsa != null) keys.Add(rsa);
                }
            }

            if (keys.Count == 0)
            {
                // 鍵が無ければ検証できない。通してはならない。
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.NoKey, "There is no key to verify the signature.");
            }

            #region 準備（読めるところまで読む）

            string decodeSaml = "";
            XmlDocument samlResponse2 = null;
            XmlNamespaceManager samlNsMgr = null;

            try
            {
                if (ret.IsRedirect)
                {
                    ret.SigAlg = CustomEncode.UrlDecode(
                        StringExtractor.GetParameterFromQueryString(SAML2Const.QueryParamSigAlg, queryString)) ?? "";

                    if (string.IsNullOrEmpty(ret.SigAlg))
                    {
                        return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.NoSigAlg, "There is no SigAlg.");
                    }

                    decodeSaml = SAML2Bindings.DecodeRedirect(queryString);
                }
                else
                {
                    decodeSaml = SAML2Bindings.DecodePost(samlResponse);
                }

                samlResponse2 = new XmlDocument();
                samlResponse2.PreserveWhitespace = false;
                samlResponse2.XmlResolver = null;
                samlResponse2.LoadXml(decodeSaml);

                samlNsMgr = SAML2Bindings.CreateNamespaceManager(samlResponse2);

                // StatusCode は検証より前に読む（エラー応答でも理由を出せるように。#598）。
                SAML2Enum.StatusCode? statusCode = null;
                SAML2Enum.StringToEnum(SAML2Bindings.GetStatusCodeInResponse(samlResponse2, samlNsMgr), out statusCode);
                ret.StatusCode = statusCode;
                ret.Response = samlResponse2;
            }
            catch (Exception ex)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.Unreadable,
                    "The response could not be read. (" + ex.GetType().Name + ")");
            }

            #endregion

            #region 署名と XML（スキーマ）

            bool verified = false;

            for (int i = 0; i < keys.Count && !verified; i++)
            {
                try
                {
                    bool signed = ret.IsRedirect
                        ? SAML2Bindings.VerifyRedirect(queryString, keys[i], sigAlgs)
                        : SAML2Bindings.VerifyPost(decodeSaml,
                            SAML2Bindings.GetIdInResponse(samlResponse2, samlNsMgr), keys[i]);

                    if (signed)
                    {
                        verified = SAML2Bindings.VerifyByXPath(samlResponse2, SAML2Enum.SamlSchema.Response, samlNsMgr);
                        if (verified) ret.VerifiedKeyIndex = i;
                    }
                }
                catch (Exception)
                {
                    // この鍵では検証できなかった。次の鍵を試す。
                }
            }

            #endregion

            #region 値（Assertion の中身）

            if (verified && ret.StatusCode != SAML2Enum.StatusCode.Success)
            {
                ret.SignatureVerified = false;
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.ErrorStatus,
                    "The response is an error response. (StatusCode=" + ret.StatusCode + ")");
            }

            if (!verified)
            {
                if (ret.StatusCode != null && ret.StatusCode != SAML2Enum.StatusCode.Success)
                {
                    return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.ErrorStatus,
                        "The response is an error response. (StatusCode=" + ret.StatusCode + ")");
                }

                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.SignatureInvalid,
                    "The signature or the XML schema could not be verified. (keys tried: " + keys.Count + ")");
            }

            bool conflicted = false;

            ret.Issuer = SAML2Client.Pick(
                SAML2Bindings.GetIssuerInResponse(samlResponse2, samlNsMgr),
                SAML2Bindings.GetIssuerInAssertion(samlResponse2, samlNsMgr), ref conflicted);

            string format = "";
            string nameId = "";
            SAML2Bindings.GetNameIDInAssertion(samlResponse2, samlNsMgr, out format, out nameId);
            ret.NameId = nameId ?? "";
            SAML2Enum.NameIDFormat? nameIDFormat = null;
            SAML2Enum.StringToEnum(format, out nameIDFormat);
            ret.NameIdFormat = nameIDFormat;

            string inResponseTo1 = "";
            string notOnOrAfter1 = "";
            string recipient = "";
            SAML2Bindings.GetSubjectConfirmationDataInAssertion(
                samlResponse2, samlNsMgr, out inResponseTo1, out notOnOrAfter1, out recipient);
            ret.Recipient = recipient ?? "";

            ret.InResponseTo = SAML2Client.Pick(inResponseTo1,
                SAML2Bindings.GetInResponseToInResponse(samlResponse2, samlNsMgr), ref conflicted);

            ret.Audience = SAML2Bindings.GetAudienceInAssertion(samlResponse2, samlNsMgr) ?? "";

            // NotOnOrAfter は 2 か所に在る。短い方を採る（VerifyResponse と同じ）。
            ret.NotOnOrAfter = SAML2Client.Earlier(notOnOrAfter1,
                SAML2Bindings.GetConditionsNotOnOrAfterInAssertion(samlResponse2, samlNsMgr));
            ret.NotExpired = (ret.NotOnOrAfter == null)
                ? (bool?)null : (DateTime.UtcNow.Ticks <= ((DateTime)ret.NotOnOrAfter).Ticks);

            SAML2Enum.AuthnContextClassRef? authnContextClassRef = null;
            SAML2Enum.StringToEnum(
                SAML2Bindings.GetAuthnContextClassRefInAssertion(samlResponse2, samlNsMgr), out authnContextClassRef);
            ret.AuthnContextClassRef = authnContextClassRef;

            // 属性は、署名を確かめた応答からだけ読む。
            ret.Attributes = SAML2Attributes.Read(samlResponse2, samlNsMgr);

            if (conflicted)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.Conflicted,
                    "The Issuer or InResponseTo conflicts between the Response and the Assertion.");
            }

            if (ret.NotExpired != true)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.Expired,
                    "The assertion has no NotOnOrAfter, or it has expired.");
            }

            ret.SignatureVerified = true;

            #endregion

            #region 照合（期待値との突き合わせ）

            ret.IssuerMatched = !string.IsNullOrEmpty(ret.Issuer) && ret.Issuer == expectedIssuer;

            if (!string.IsNullOrEmpty(expectedAcsUrl))
            {
                ret.AudienceMatched = (ret.Audience == expectedAcsUrl);
                ret.RecipientMatched = (ret.Recipient == expectedAcsUrl);
            }

            if (!string.IsNullOrEmpty(expectedInResponseTo))
            {
                ret.InResponseToMatched = (ret.InResponseTo == expectedInResponseTo);
            }

            if (!ret.IssuerMatched)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.IssuerMismatch,
                    "The Issuer does not match the expected issuer.");
            }

            if (ret.AudienceMatched == false)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.AudienceMismatch,
                    "The Audience does not match the ACS URL.");
            }

            if (ret.RecipientMatched == false)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.RecipientMismatch,
                    "The Recipient does not match the ACS URL.");
            }

            if (ret.InResponseToMatched == false)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.InResponseToMismatch,
                    "The InResponseTo does not match the ID of the request.");
            }

            if (ret.RelayStateMatched == false)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.RelayStateMismatch,
                    "The RelayState does not match the state.");
            }

            #endregion

            ret.IsValid = true;
            return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.Valid,
                "The signature, Issuer, Audience, Recipient, InResponseTo and RelayState were verified.");
        }

        /// <summary>理由を設定する</summary>
        /// <param name="ret">SAML2VerifyResult</param>
        /// <param name="reason">SAML2VerifyResult.Reasons</param>
        /// <param name="description">string</param>
        /// <returns>SAML2VerifyResult</returns>
        private static SAML2VerifyResult SetReason(
            SAML2VerifyResult ret, SAML2VerifyResult.Reasons reason, string description)
        {
            ret.Reason = reason;
            ret.ReasonDescription = description;
            return ret;
        }

        /// <summary>2 か所に書かれた同じ値を 1 つに決める（VerifyResponse と同じ決め方）</summary>
        /// <param name="value1">string</param>
        /// <param name="value2">string</param>
        /// <param name="conflicted">ref bool（両方に値が在って食い違っていたら true にする）</param>
        /// <returns>採った値</returns>
        private static string Pick(string value1, string value2, ref bool conflicted)
        {
            if (value1 == value2) return value1 ?? "";
            if (string.IsNullOrEmpty(value1)) return value2 ?? "";
            if (string.IsNullOrEmpty(value2)) return value1;

            conflicted = true;
            return value1;
        }

        /// <summary>2 つの W3C タイムスタンプのうち、早い方を返す</summary>
        /// <param name="value1">string</param>
        /// <param name="value2">string</param>
        /// <returns>早い方（どちらも読めなければ null）</returns>
        private static DateTime? Earlier(string value1, string value2)
        {
            DateTime? time1 = SAML2Client.ToTime(value1);
            DateTime? time2 = SAML2Client.ToTime(value2);

            if (time1 == null) return time2;
            if (time2 == null) return time1;

            return (((DateTime)time1).Ticks <= ((DateTime)time2).Ticks) ? time1 : time2;
        }

        /// <summary>W3C タイムスタンプを読む（読めなければ null）</summary>
        /// <param name="value">string</param>
        /// <returns>DateTime?</returns>
        private static DateTime? ToTime(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            try
            {
                return FormatConverter.FromW3cTimestamp(value);
            }
            catch (Exception)
            {
                return null;
            }
        }
#endregion
    }
}