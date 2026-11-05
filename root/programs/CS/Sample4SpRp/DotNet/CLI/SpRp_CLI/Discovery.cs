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
//* クラス名        ：Discovery
//* クラス日本語名  ：OP の Discovery（OpenID Connect Discovery 1.0）から、エンドポイントを引く
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/04  玄人 幸道         新規作成（#649）
//**********************************************************************************

using System;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;

namespace SpRp_CLI
{
    /// <summary>OP の Discovery から、エンドポイントを引く</summary>
    /// <remarks>
    /// **issuer を照合する**（OpenID Connect Discovery 1.0 §4.3）。
    /// 照合しないと、Discovery の URL を差し替えられたときに、別の OP の口へ送ってしまう。
    /// </remarks>
    public class Discovery
    {
        /// <summary>issuer</summary>
        public string Issuer { get; private set; }

        /// <summary>authorization_endpoint</summary>
        public Uri AuthorizationEndpoint { get; private set; }

        /// <summary>token_endpoint</summary>
        public Uri TokenEndpoint { get; private set; }

        /// <summary>userinfo_endpoint</summary>
        public Uri UserInfoEndpoint { get; private set; }

        /// <summary>device_authorization_endpoint（RFC 8628 §4。無ければ null）</summary>
        public Uri DeviceAuthorizationEndpoint { get; private set; }

        /// <summary>Discovery を読む</summary>
        /// <param name="discoveryUri">Discovery の URL</param>
        /// <param name="expectedIssuer">期待する issuer</param>
        /// <returns>Discovery</returns>
        public static async Task<Discovery> LoadAsync(string discoveryUri, string expectedIssuer)
        {
            string json = await OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(new Uri(discoveryUri));

            // **繋がらないときは、例外ではなく空が返る**（GetDiscoveryDocumentAsync）
            if (string.IsNullOrEmpty(json))
            {
                throw new InvalidOperationException(
                    "Discovery（" + discoveryUri + "）を読めません。IdP（スタブ）は起動していますか？（Sample4SpRp/README.md の 2 節）");
            }

            JObject doc = JObject.Parse(json);

            string issuer = (string)doc[OAuth2AndOIDCConst.issuer];
            if (issuer != expectedIssuer)
            {
                throw new InvalidOperationException(string.Format(
                    "Discovery の issuer（{0}）が、期待する値（{1}）と違います。", issuer, expectedIssuer));
            }

            return new Discovery
            {
                Issuer = issuer,
                AuthorizationEndpoint = Discovery.ToUri(doc, "authorization_endpoint"),
                TokenEndpoint = Discovery.ToUri(doc, "token_endpoint"),
                UserInfoEndpoint = Discovery.ToUri(doc, "userinfo_endpoint"),
                DeviceAuthorizationEndpoint = Discovery.ToUri(doc, "device_authorization_endpoint"),
            };
        }

        /// <summary>項目を Uri にする（無ければ null）</summary>
        /// <param name="doc">Discovery</param>
        /// <param name="name">項目名</param>
        /// <returns>Uri</returns>
        private static Uri ToUri(JObject doc, string name)
        {
            string value = (string)doc[name];
            return string.IsNullOrEmpty(value) ? null : new Uri(value);
        }
    }
}
