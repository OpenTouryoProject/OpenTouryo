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
//*  2026/10/22  玄人 幸道         VerifyResponseWithReasonに、相手と自分の値を設定から読む版を追加
//*  2026/10/22  玄人 幸道         鍵・証明書を受ける版（CreateRedirectRequest / CreatePostRequest / VerifyResponse）から、
//*                                null・空なら設定を読む分岐を外し、設定を読む版（引数の少ない版）へ移した。
//*                                鍵を受ける版は null・空を ArgumentNullException にする（黙って無署名にしないため）
//*  2026/10/22  玄人 幸道         処理区分ごとに partial クラスへ分割した（SAML2Client.*.cs）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Client（ライブラリ）</summary>
    /// <remarks>
    /// 処理区分ごとに partial クラスのファイルへ分けている（公開 API は 1 つの型のまま）。
    /// 各ファイルの Region は「設定を読む版 → 引数で受ける版 → private」の順に並べる
    /// （設定は、引数の少ない版だけが読む）。
    ///
    /// | ファイル | 区分 |
    /// |---|---|
    /// | SAML2Client.cs | 要求の作成（SP → IdP。Redirect / POST） |
    /// | SAML2Client.VerifyResponse.cs | 応答の検証（bool を返す従来の版） |
    /// | SAML2Client.VerifyResponseWithReason.cs | 応答の検証（理由を返す版。#634） |
    ///
    /// 応答の検証の結果クラスは SAML2VerifyResult.cs、低水準の符号化・署名・XPath は SAML2Bindings.cs。
    /// </remarks>
    public partial class SAML2Client
    {
        #region 設定を読む版

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
            // 設定の鍵（CmnClientParams.RsaPfxFilePath）を、ここで作って渡す。
#if NETSTD
            X509Certificate2 x509 = X509CertificateLoader.LoadPkcs12FromFile(CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword);
#else
            X509Certificate2 x509 = new X509Certificate2(CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword);
#endif
            return SAML2Client.CreatePostRequest(
                protocolBinding, nameIDFormat,
                iss, assertionConsumerService, relayState, x509.GetRSAPrivateKey(), out id);
        }

        #endregion

        #region 引数（鍵）で受ける版

        /// <summary>CreateRedirectRequest</summary>
        /// <param name="createRoR">SAML2Enum.RequestOrResponse</param>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="iss">string</param>
        /// <param name="assertionConsumerService">string</param>
        /// <param name="relayState">string</param>
        /// <param name="digitalSign">
        /// 署名に使う DigitalSign（自分の秘密鍵）。SigAlg も、この DigitalSign のアルゴリズムから決まる。
        /// null は受けない（ArgumentNullException）。
        /// </param>
        /// <param name="id">out string</param>
        /// <returns>queryString</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// 設定（CmnClientParams.RsaPfxFilePath）の鍵で署名するなら、digitalSign を受けない版を使う。
        /// SAML2Bindings.EncodeAndSignRedirect は null なら署名しないので、
        /// null を素通しにすると、黙って無署名の要求を出すことになる。そのため null は例外にする。
        /// </remarks>
        public static string CreateRedirectRequest(
            SAML2Enum.RequestOrResponse createRoR,
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState,
            DigitalSign digitalSign, out string id)
        {
            // 設定の鍵は、digitalSign を受けない版が作って渡す。
            if (digitalSign == null)
            {
                throw new ArgumentNullException(nameof(digitalSign));
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
        /// <param name="rsa">
        /// 署名に使う RSA（自分の秘密鍵）。
        /// null は受けない（ArgumentNullException）。
        /// </param>
        /// <param name="id">out string</param>
        /// <returns>samlRequest</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// 設定（CmnClientParams.RsaPfxFilePath）の鍵で署名するなら、rsa を受けない版を使う。
        /// SAML2Bindings.EncodeAndSignPost は null なら署名しないので、
        /// null を素通しにすると、黙って無署名の要求を出すことになる。そのため null は例外にする。
        /// </remarks>
        public static string CreatePostRequest(
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState,
            RSA rsa, out string id)
        {
            // 設定の鍵は、rsa を受けない版が作って渡す。
            if (rsa == null)
            {
                throw new ArgumentNullException(nameof(rsa));
            }

            // SamlRequestの生成
            string samlRequest = SAML2Bindings.CreateRequest(
                iss, protocolBinding, nameIDFormat,
                assertionConsumerService, out id).OuterXml;

            // SamlRequestのエンコと署名
            return SAML2Bindings.EncodeAndSignPost(samlRequest, id, rsa);
        }

        #endregion
    }
}
