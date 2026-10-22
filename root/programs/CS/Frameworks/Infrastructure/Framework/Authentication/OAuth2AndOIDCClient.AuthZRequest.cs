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
//* クラス名        ：OAuth2AndOIDCClient（AuthZRequest）
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）の認可要求の保護（JAR の預け先・PAR）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（OAuth2AndOIDCClient.cs から処理区分ごとに分割した）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（ライブラリ）の認可要求の保護（JAR の預け先・PAR）</summary>
    public partial class OAuth2AndOIDCClient
    {
        #region Request Object の預け先（RFC 9101 5.2.1）

        /// <summary>RequestObjectを登録する</summary>
        /// <param name="requestObjectRegUri">Uri</param>
        /// <param name="requestObject">string</param>
        /// <returns>RequestObjectの登録結果</returns>
        /// <remarks>
        /// Request Object（署名付きJWT）を、生の本文としてPOSTし、request_uriを得る。
        /// RFC 9101（JAR）§5.2.1 が認可サーバによるこの預け先を認めており、この形で正しい。
        /// 本文の形式もクライアント認証も異なるPAR（RFC 9126）には、
        /// PushAuthorizationRequestAsyncメソッドを使用する。
        /// </remarks>
        public static async Task<string> RegisterRequestObjectAsync(
            Uri requestObjectRegUri, string requestObject)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = requestObjectRegUri,
            };

            httpRequestMessage.Content = new StringContent(requestObject);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region PAR（RFC 9126）

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="requestObject">string（Request Object＝署名付きJWT）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（tls_client_authではnull）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>
        /// Request Objectをrequestパラメタに載せて送る形（RFC 9126 §3、RFC 9101）。
        /// private_key_jwt / client_secret_jwtには、client_assertionを取るオーバーロードを使用する。
        /// </remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, string requestObject,
            string client_id, string client_secret,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                authRequestPushUri, requestObject,
                client_id, client_secret, null, authMethod).ConfigureAwait(false);
        }

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="requestObject">string（Request Object＝署名付きJWT）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>Request Objectをrequestパラメタに載せて送る形（RFC 9126 §3、RFC 9101）。</remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, string requestObject,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                authRequestPushUri,
                new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.request, requestObject }
                },
                client_id, client_secret, client_assertion, authMethod).ConfigureAwait(false);
        }

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="authZRequestParams">Dictionary（認可リクエストのパラメタ）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（tls_client_authではnull）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>
        /// private_key_jwt / client_secret_jwtには、client_assertionを取るオーバーロードを使用する。
        /// </remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, Dictionary<string, string> authZRequestParams,
            string client_id, string client_secret,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                authRequestPushUri, authZRequestParams,
                client_id, client_secret, null, authMethod).ConfigureAwait(false);
        }

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="authZRequestParams">Dictionary（認可リクエストのパラメタ）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>
        /// PARは、認可エンドポイントへ送るはずのパラメタを、
        /// Tokenエンドポイントと同じクライアント認証を付けてPOSTする（§2）。
        /// FAPI 2.0 は、この認証をprivate_key_jwtかMTLS（tls_client_auth）に限っている。
        /// client_idは認可リクエストの必須パラメタなので、認証方式に依らず本文に載せる（§2.1）。
        /// 応答のrequest_uriを、認可リクエストのrequest_uriパラメタに指定して使用する。
        /// </remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, Dictionary<string, string> authZRequestParams,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = authRequestPushUri,
            };

            // body（引数のDictionaryは変更しない）
            Dictionary<string, string> body = new Dictionary<string, string>(authZRequestParams);

            // 認証情報の付加（PARはclient_idが必須 : RFC 9126 §2.1）
            OAuth2AndOIDCClient.SetClientAuthentication(
                httpRequestMessage, body, client_id, client_secret, client_assertion, authMethod, true);

            httpRequestMessage.Content = new FormUrlEncodedContent(body);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
