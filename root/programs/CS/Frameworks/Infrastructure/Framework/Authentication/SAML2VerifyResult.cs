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
//* クラス名        ：SAML2VerifyResult
//* クラス日本語名  ：SAML2 の応答を検証した結果（理由つき）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の C-3）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2 の応答を検証した結果（理由つき）</summary>
    /// <remarks>
    /// SAML2Client.VerifyResponse は bool を返すので、落ちた理由が分からない。
    /// ID 連携は「繋がらない」としか分からず、切り分けられない（#634 の C-3）。
    /// こちらは、確かめた項目ごとの結果と、判定の理由を持つ（SAML2Client.VerifyResponseWithReason）。
    ///
    /// **認証の判断には IsValid だけを使うこと。**
    /// IsValid が false のときの各値は、検証されていない値を含みうる（表示・ログの切り分けにだけ使う）。
    /// </remarks>
    public class SAML2VerifyResult
    {
        /// <summary>判定の理由</summary>
        public enum Reasons : int
        {
            /// <summary>すべて照合できた</summary>
            Valid,

            /// <summary>SAMLResponse が無い</summary>
            NoResponse,

            /// <summary>Redirect Binding なのに SigAlg が無い</summary>
            NoSigAlg,

            /// <summary>署名を検証する鍵が無い</summary>
            NoKey,

            /// <summary>応答を読めなかった（XML でない・展開できない）</summary>
            Unreadable,

            /// <summary>エラー応答である（StatusCode が Success でない）</summary>
            ErrorStatus,

            /// <summary>署名または XML（スキーマ）の検証で落ちた</summary>
            SignatureInvalid,

            /// <summary>Response と Assertion で、Issuer か InResponseTo が食い違っている</summary>
            Conflicted,

            /// <summary>NotOnOrAfter が無い、または過ぎている</summary>
            Expired,

            /// <summary>Issuer が期待値と違う</summary>
            IssuerMismatch,

            /// <summary>Audience が自分の ACS URL と違う</summary>
            AudienceMismatch,

            /// <summary>Recipient が自分の ACS URL と違う</summary>
            RecipientMismatch,

            /// <summary>InResponseTo が、送った AuthnRequest の ID と違う</summary>
            InResponseToMismatch,

            /// <summary>RelayState が、送った state と違う</summary>
            RelayStateMismatch
        }

        #region 判定

        /// <summary>使ってよい応答か（認証の判断には、これだけを使う）</summary>
        public bool IsValid { get; internal set; }

        /// <summary>判定の理由</summary>
        public Reasons Reason { get; internal set; }

        /// <summary>判定の理由（ログ向けの英文。値そのものは入れない）</summary>
        public string ReasonDescription { get; internal set; }

        #endregion

        #region 確かめた項目ごとの結果

        /// <summary>署名と XML（スキーマ）と値の整合（食い違い・期限）を確かめられたか</summary>
        public bool SignatureVerified { get; internal set; }

        /// <summary>何本目の鍵で署名を確かめたか（0 起点。確かめられなければ -1）</summary>
        public int VerifiedKeyIndex { get; internal set; }

        /// <summary>Issuer が期待値と一致したか</summary>
        public bool IssuerMatched { get; internal set; }

        /// <summary>Audience が自分の ACS URL と一致したか（期待値が無ければ null。SAML Core 2.5.1.4）</summary>
        public bool? AudienceMatched { get; internal set; }

        /// <summary>Recipient が自分の ACS URL と一致したか（期待値が無ければ null）</summary>
        public bool? RecipientMatched { get; internal set; }

        /// <summary>InResponseTo が送った AuthnRequest の ID と一致したか（期待値が無ければ null。Web SSO Profile 4.1.4.3）</summary>
        public bool? InResponseToMatched { get; internal set; }

        /// <summary>RelayState が送った state と一致したか（期待値が無ければ null）</summary>
        public bool? RelayStateMatched { get; internal set; }

        /// <summary>NotOnOrAfter を過ぎていないか（読めなければ null。SAML Core 2.5.1.2）</summary>
        public bool? NotExpired { get; internal set; }

        #endregion

        #region 読み取った値

        /// <summary>Redirect Binding（GET）で受けたか</summary>
        public bool IsRedirect { get; internal set; }

        /// <summary>SigAlg（Redirect Binding のときだけ）</summary>
        public string SigAlg { get; internal set; }

        /// <summary>RelayState</summary>
        public string RelayState { get; internal set; }

        /// <summary>StatusCode（読めなければ null）</summary>
        public SAML2Enum.StatusCode? StatusCode { get; internal set; }

        /// <summary>Issuer</summary>
        public string Issuer { get; internal set; }

        /// <summary>NameID</summary>
        public string NameId { get; internal set; }

        /// <summary>NameIDFormat</summary>
        public SAML2Enum.NameIDFormat? NameIdFormat { get; internal set; }

        /// <summary>Audience</summary>
        public string Audience { get; internal set; }

        /// <summary>InResponseTo</summary>
        public string InResponseTo { get; internal set; }

        /// <summary>Recipient（SubjectConfirmationData）</summary>
        public string Recipient { get; internal set; }

        /// <summary>NotOnOrAfter（SubjectConfirmationData と Conditions の早い方。UTC）</summary>
        public DateTime? NotOnOrAfter { get; internal set; }

        /// <summary>AuthnContextClassRef</summary>
        public SAML2Enum.AuthnContextClassRef? AuthnContextClassRef { get; internal set; }

        /// <summary>アサーションの属性（SAML2Attributes.Read。署名を確かめられたときだけ。無ければ空）</summary>
        public Dictionary<string, string> Attributes { get; internal set; }

        /// <summary>応答の XML（読めなければ null）</summary>
        public XmlDocument Response { get; internal set; }

        #endregion

        /// <summary>Constructor</summary>
        internal SAML2VerifyResult()
        {
            this.IsValid = false;
            this.Reason = Reasons.SignatureInvalid;
            this.ReasonDescription = "";
            this.VerifiedKeyIndex = -1;
            this.SigAlg = "";
            this.RelayState = "";
            this.Issuer = "";
            this.NameId = "";
            this.Audience = "";
            this.InResponseTo = "";
            this.Recipient = "";
            this.Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
