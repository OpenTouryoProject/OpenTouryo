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
//* クラス名        ：Saml2AssertionConsumerService
//* クラス日本語名  ：SAML2 の応答を受ける画面（ACS。HTTP-Redirect）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/04  玄人 幸道         新規作成（#649）
//*  2026/11/14  玄人 幸道         Audience を SP の entityID（SpRp_Saml2SpEntityId）と照合する版に替えた（空なら従来どおり ACS の URL。#693）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using System.Web.Security;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Util;

namespace WebForms_Sample.Aspx.Saml2
{
    /// <summary>SAML2 の応答を受ける画面（ACS。HTTP-Redirect）</summary>
    /// <remarks>
    /// 要求は Start/login.aspx（btnButton3）が出す。
    /// **検証は SAML2Client.VerifyResponseWithReason**（署名・Issuer・Audience・Recipient・InResponseTo・RelayState・期限）。
    /// **使ってよいかの判断は IsValid だけで行う。**
    /// **Forms 認証の対象外にする**（Web.config の location。未認証で受けるため）。
    /// </remarks>
    public partial class Saml2AssertionConsumerService : System.Web.UI.Page
    {
        #region 応答（ACS）

        /// <summary>Page_Load</summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        protected async void Page_Load(object sender, EventArgs e)
        {
            // **Redirect Binding は、クエリ文字列そのものが署名の対象**。受け取ったままの値（RawUrl）を使う。
            string rawUrl = Request.RawUrl ?? "";
            int q = rawUrl.IndexOf('?');
            string queryString = (q < 0) ? "" : rawUrl.Substring(q + 1);

            // 1 度しか使わない
            string expectedRelayState = (string)Session["saml2_relay_state"];
            string expectedRequestId = (string)Session["saml2_request_id"];
            Session["saml2_relay_state"] = null;
            Session["saml2_request_id"] = null;

            if (string.IsNullOrEmpty(expectedRelayState) || string.IsNullOrEmpty(expectedRequestId))
            {
                // 自分が送った要求への応答ではない
                return;
            }

            // **署名を検証する鍵は、メタデータの署名鍵**（ロールオーバー中は複数。順に試す）
            SAML2Metadata metadata = await SAML2Metadata.GetAsync();
            List<RSA> keys = (metadata == null) ? new List<RSA>() : metadata.GetSigningKeys();

            // **Audience は、自分（SP）の entityID（SpRp_Saml2SpEntityId）と照合する**（SAML Profiles 4.1.4.2。空なら、従来どおり ACS の URL と。#693）
            SAML2VerifyResult result = SAML2Client.VerifyResponseWithReason(
                queryString, null, keys,
                CmnClientParams.SpRp_Saml2EntityId, CmnClientParams.SpRp_Saml2SpEntityId, CmnClientParams.SpRp_Saml2AcsUri,
                expectedRequestId, Request.QueryString["RelayState"], expectedRelayState, null);

            //  **識別子は NameID**（IdP の entityID と組で使う）。**Transient は受けない**（毎回変わる。SAML Core 8.3.8）。
            if (!result.IsValid
                || string.IsNullOrEmpty(result.NameId)
                || result.NameIdFormat == SAML2Enum.NameIDFormat.Transient)
            {
                // ログインに失敗（OAuth2/OAuth2AuthorizationCodeGrantClient.aspx と同じく、何も出さない）
                return;
            }

            // ログインに成功
            //   **識別子は NameID**（Forms 認証の名前）。**表示名は属性の mail**（MyUserInfo。無ければ NameID）。
            string email = SAML2Attributes.GetEmail(result.Attributes);
            FormsAuthentication.RedirectFromLoginPage(result.NameId, false);
            MyUserInfo ui = new MyUserInfo(string.IsNullOrEmpty(email) ? result.NameId : email, Request.UserHostAddress);
            UserInfoHandle.SetUserInformation(ui);
        }

        #endregion
    }
}
