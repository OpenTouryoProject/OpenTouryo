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
//* クラス名        ：SAML2Client（VerifyResponseWithReason）
//* クラス日本語名  ：SAML2Client（ライブラリ）の応答の検証（理由を返す版。#634）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（SAML2Client.cs から処理区分ごとに分割した）
//*  2026/10/22  玄人 幸道         NotOnOrAfter が読めなければ Expired で断るようにした（読めない方を無視して通していた）。
//*                                Pick を bool 版と共有するため、null を空文字にしない形にした
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
    /// <summary>SAML2Client（ライブラリ）の応答の検証（理由を返す版。#634）</summary>
    public partial class SAML2Client
    {
        #region 設定を読む版

        /// <summary>VerifyResponseWithReason（理由を返す応答の検証。相手と自分の値を設定から読む）</summary>
        /// <param name="queryString">string（Redirect Binding のときのクエリ文字列。POST なら null か空文字）</param>
        /// <param name="samlResponse">string（POST Binding のときの SAMLResponse）</param>
        /// <param name="expectedInResponseTo">string（送った AuthnRequest の ID。空なら照合しない）</param>
        /// <param name="relayState">string（受け取った RelayState）</param>
        /// <param name="expectedRelayState">string（送った state。空なら照合しない）</param>
        /// <returns>SAML2VerifyResult</returns>
        /// <remarks>
        /// 自由に利用できる。相手が 1 つの配備向け。設定から次を読む。
        ///
        /// | 引数 | 設定 |
        /// |---|---|
        /// | 署名を検証する鍵 | CmnClientParams.RsaCerFilePath（SpRp_RsaCerFilePath。VerifyResponse と同じ） |
        /// | 期待する Issuer | CmnClientParams.SpRp_Saml2EntityId |
        /// | 自分の ACS URL | CmnClientParams.SpRp_Saml2AcsUri（空なら Audience / Recipient を照合しない） |
        ///
        /// SpRp_RsaCerFilePath が空なら、鍵が無いものとして NoKey を返す。
        /// メタデータの鍵（ロールオーバー中は複数）で検証するなら、
        /// SAML2Metadata.GetAsync() の GetSigningKeys() を、鍵を受ける版に渡す。
        /// </remarks>
        public static SAML2VerifyResult VerifyResponseWithReason(
            string queryString, string samlResponse,
            string expectedInResponseTo, string relayState, string expectedRelayState)
        {
            List<RSA> keys = new List<RSA>();
            string cerFilePath = CmnClientParams.RsaCerFilePath;

            if (!string.IsNullOrEmpty(cerFilePath))
            {
                keys.Add(new DigitalSignX509(cerFilePath, "", HashAlgorithmName.SHA256).X509Certificate.GetRSAPublicKey());
            }

            return SAML2Client.VerifyResponseWithReason(queryString, samlResponse, keys,
                CmnClientParams.SpRp_Saml2EntityId, CmnClientParams.SpRp_Saml2AcsUri,
                expectedInResponseTo, relayState, expectedRelayState);
        }

        #endregion

        #region 引数（鍵・期待値）で受ける版

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
                SAML2Bindings.GetIssuerInAssertion(samlResponse2, samlNsMgr), ref conflicted) ?? "";

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
                SAML2Bindings.GetInResponseToInResponse(samlResponse2, samlNsMgr), ref conflicted) ?? "";

            ret.Audience = SAML2Bindings.GetAudienceInAssertion(samlResponse2, samlNsMgr) ?? "";

            // NotOnOrAfter は 2 か所に在る。短い方を採る（VerifyResponse と同じ）。
            // 値が在るのに読めなければ、使わない（読めない方を無視して、もう片方で通さない）。
            DateTime? notOnOrAfter = null;
            bool notOnOrAfterReadable = SAML2Client.TryGetEarlier(notOnOrAfter1,
                SAML2Bindings.GetConditionsNotOnOrAfterInAssertion(samlResponse2, samlNsMgr), out notOnOrAfter);
            ret.NotOnOrAfter = notOnOrAfter;
            ret.NotExpired = (notOnOrAfter == null)
                ? (bool?)null : (DateTime.UtcNow.Ticks <= ((DateTime)notOnOrAfter).Ticks);

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

            if (!notOnOrAfterReadable)
            {
                return SAML2Client.SetReason(ret, SAML2VerifyResult.Reasons.Expired,
                    "The NotOnOrAfter of the assertion could not be read.");
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

        #endregion

        #region private

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

        /// <summary>2 か所に書かれた同じ値を 1 つに決める（Response と Assertion の Issuer / InResponseTo）</summary>
        /// <param name="value1">string</param>
        /// <param name="value2">string</param>
        /// <param name="conflicted">ref bool（両方に値が在って食い違っていたら true にする）</param>
        /// <returns>
        /// 採った値。同じなら value1（null なら null のまま）、片方だけ在ればその値、
        /// どちらも空（null と空文字の組を含む）なら空文字。食い違っていたら value1。
        /// </returns>
        /// <remarks>
        /// bool 版の VerifyResponse と、理由つきの版の両方が使う。
        /// null を空文字にしないのは、bool 版の out の値（従来どおり null になりうる）を変えないため。
        /// 理由つきの版は、受け取ったところで空文字にする。
        /// </remarks>
        private static string Pick(string value1, string value2, ref bool conflicted)
        {
            if (value1 == value2) return value1;
            if (string.IsNullOrEmpty(value1)) return string.IsNullOrEmpty(value2) ? "" : value2;
            if (string.IsNullOrEmpty(value2)) return value1;

            conflicted = true;
            return value1;
        }

        /// <summary>2 つの W3C タイムスタンプのうち、早い方を求める</summary>
        /// <param name="value1">string</param>
        /// <param name="value2">string</param>
        /// <param name="earlier">out DateTime?（早い方。どちらも空なら null）</param>
        /// <returns>読めたら true（値が在るのに読めないものが 1 つでも在れば false）</returns>
        /// <remarks>
        /// 読めない方を無視して、もう片方で通すことはしない（分からないものは通さない）。
        /// bool 版の VerifyResponse は、読めなければ例外を投げる従来の振る舞いのまま、これを使わない。
        /// </remarks>
        private static bool TryGetEarlier(string value1, string value2, out DateTime? earlier)
        {
            earlier = null;

            DateTime? time1;
            DateTime? time2;

            if (!SAML2Client.TryToTime(value1, out time1) || !SAML2Client.TryToTime(value2, out time2))
            {
                return false;
            }

            if (time1 == null) { earlier = time2; }
            else if (time2 == null) { earlier = time1; }
            else { earlier = (((DateTime)time1).Ticks <= ((DateTime)time2).Ticks) ? time1 : time2; }

            return true;
        }

        /// <summary>W3C タイムスタンプを読む</summary>
        /// <param name="value">string</param>
        /// <param name="time">out DateTime?（空なら null）</param>
        /// <returns>空か、読めたら true</returns>
        private static bool TryToTime(string value, out DateTime? time)
        {
            time = null;

            if (string.IsNullOrEmpty(value)) return true;

            try
            {
                time = FormatConverter.FromW3cTimestamp(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
