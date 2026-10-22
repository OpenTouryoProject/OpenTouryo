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
//* クラス名        ：OAuth2AndOIDCClient
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/04/24  西野 大介         新規
//*  2018/08/10  西野 大介         汎用認証サイトからのコード移行
//*  2019/08/01  西野 大介         client_secret_postのサポートを追加
//*  2020/03/04  西野 大介         FAPI CIBAの認可リクエスト（WebAPI）を追加
//*  2020/12/18  西野 大介         Device AuthZの認可リクエスト（WebAPI）を追加
//*  2026/09/25  玄人 幸道         PAR（RFC 9126）とCIBAのRequest Object直接送信を追加
//*  2026/09/25  玄人 幸道         FAPI 2.0向けにprivate_key_jwt / tls_client_authを追加
//*  2026/09/26  玄人 幸道         private_key_jwtのトークン要求で、client_assertionも併せて送る
//*  2026/10/22  玄人 幸道         HttpClientのget・SendAsync、DPoP（RFC 9449）で送る口、
//*                                DCR（RFC 7591 / 7592）のクライアント側を追加（#634）
//*  2026/10/22  玄人 幸道         処理区分ごとに partial クラスへ分割した（OAuth2AndOIDCClient.*.cs）
//*  2026/10/22  玄人 幸道         公開情報を取りに行く GET を GetStringAsync（internal）に切り出した
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Net.Http;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    // ライブラリ内でawaitする場合は、ConfigureAwait(false)を使う。

    /// <summary>OAuth2AndOIDCClient（ライブラリ）</summary>
    /// <remarks>
    /// 処理区分ごとに partial クラスのファイルへ分けている（公開 API は 1 つの型のまま）。
    ///
    /// | ファイル | 区分 |
    /// |---|---|
    /// | OAuth2AndOIDCClient.cs | 共有基盤（HttpClient・送信・クライアント認証） |
    /// | OAuth2AndOIDCClient.Grants.cs | 基本フロー（認可コード・PKCE・Client Credentials・ROPC） |
    /// | OAuth2AndOIDCClient.TokenManagement.cs | トークン運用・管理（Refresh・Revoke・Introspect） |
    /// | OAuth2AndOIDCClient.OpenIDConnect.cs | OpenID Connect・メタデータ・鍵（UserInfo・JWK Set・Discovery） |
    /// | OAuth2AndOIDCClient.ExtensionGrants.cs | 拡張グラント・分離フロー（JWT Bearer・Device AuthZ・CIBA） |
    /// | OAuth2AndOIDCClient.AuthZRequest.cs | 認可要求の保護（JAR の預け先・PAR） |
    /// | OAuth2AndOIDCClient.DPoP.cs | 送信者制約トークン（DPoP） |
    /// | OAuth2AndOIDCClient.ClientRegistration.cs | 動的クライアント登録（DCR） |
    /// </remarks>
    public partial class OAuth2AndOIDCClient
    {
        #region HttpClient

        /// <summary>HttpClient</summary>
        private static HttpClient _HttpClient = null;

        /// <summary>HttpClient</summary>
        /// <remarks>
        /// get は #634（H-1）で足した。利用側が、自分の渡した共有の HttpClient
        /// （TLS・プロキシ・クライアント証明書の扱いを揃えたもの）を、他のサーバ間通信にも使えるように。
        /// </remarks>
        public static HttpClient HttpClient
        {
            get
            {
                return OAuth2AndOIDCClient._HttpClient;
            }
            set
            {
                OAuth2AndOIDCClient._HttpClient = value;
            }
        }

        /// <summary>この HttpClient で送る</summary>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        /// <returns>HttpResponseMessage</returns>
        /// <remarks>
        /// 自由に利用できる（#634 の H-1）。
        /// ライブラリが口を持たない要求（ヘッダを足したい・状態コードを見たいなど）を、
        /// 同じ HttpClient で送るために使う。
        /// </remarks>
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage == null)
            {
                throw new ArgumentNullException(nameof(httpRequestMessage));
            }

            return await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
        }

        /// <summary>URL を GET して、本文を返す（失敗しても例外にしない）</summary>
        /// <param name="uri">Uri</param>
        /// <returns>本文（HttpClient が無い・通信に失敗したときは空文字）</returns>
        /// <remarks>
        /// 相手の公開情報（JWK Set・Discovery・SAML2 メタデータ）を取りに行く口が共有する。
        /// 以前は GetJwkSetAsync の中身で、名前が JwkSet のまま他の取得にも使われていた。
        /// 取れなくても例外にしない（呼び出し側が、設定の値など別の手段に倒せるように）。
        /// 状態コードは見ない（エラーの本文もそのまま返す。従来の GetJwkSetAsync と同じ）。
        /// </remarks>
        internal static async Task<string> GetStringAsync(Uri uri)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
            };

            // HttpResponseMessage

            // ピーキーな jwks_uri 実装に例外処理を追加
            if (OAuth2AndOIDCClient._HttpClient == null)
            {
                // HttpClientが無い。
                Debug.WriteLine("HttpClient is not set in OAuth2AndOIDCClient.GetStringAsync method.");
            }
            else
            {
                try
                {
                    httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
                    return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // HttpClientで失敗。
                    Debug.WriteLine("Exception was catched in OAuth2AndOIDCClient.GetStringAsync method: " + ex.ToString());
                }
            }

            return ""; // 空
        }

        #endregion

        #region クライアント認証（RFC 6749 2.3 / RFC 7523 2.2 / RFC 8705）

        /// <summary>クライアント認証を付加する</summary>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        /// <param name="body">Dictionary（本文。client_idと資格情報を追加する）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <param name="alwaysSetClientId">bool（認証方式に依らずclient_idを本文に載せるか）</param>
        /// <remarks>
        /// FAPI 2.0 は、秘密ベースの方式（client_secret_basic / client_secret_post）をprofileから外し、
        /// private_key_jwt と MTLS（tls_client_auth）だけを認めている。
        /// 
        /// client_idを本文に載せるかは、エンドポイントによって違う。
        /// PARは認可リクエストのパラメタを送る口なので、client_idが必須（RFC 9126 §2.1）だが、
        /// CIBAのバックチャネル認証は「認証方式が要求するパラメタ以外をJWTの外に置いてはならない」
        /// （CIBA Core 1.0 §7.1.1）ため、client_secret_postとtls_client_authのときだけになる。
        /// そこで、常に載せるかどうかをalwaysSetClientIdで受け取る。
        /// 
        /// tls_client_authは、クライアント証明書がTLSの層で示されている必要があるため、
        /// HttpClientプロパティに、証明書を載せたHttpClientHandlerのHttpClientを設定しておく
        /// （証明書は、CmnClientParams.ClientCertPfxFilePath / ClientCertPfxPasswordで取得できる）。
        /// HttpClientプロパティはstaticなので、差し替えはプロセス全体に効く。
        /// 1つのプロセスが複数のクライアントとして振る舞い、一部だけmTLSという構成では、
        /// 差し替えの順序に注意する。
        /// </remarks>
        private static void SetClientAuthentication(
            HttpRequestMessage httpRequestMessage, Dictionary<string, string> body,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod, bool alwaysSetClientId)
        {
            if (alwaysSetClientId)
            {
                body[OAuth2AndOIDCConst.client_id] = client_id;
            }

            if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
            {
                // RFC 6749 §2.3.1
                httpRequestMessage.Headers.Authorization
                    = AuthenticationHeader.CreateBasicAuthenticationHeaderValue(client_id, client_secret);
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_post)
            {
                // RFC 6749 §2.3.1（この方式はclient_idも本文で送る）
                body[OAuth2AndOIDCConst.client_id] = client_id;
                body[OAuth2AndOIDCConst.client_secret] = client_secret;
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.private_key_jwt
                || authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_jwt)
            {
                // RFC 7523 §2.2（署名の鍵が違うだけで、送り方は同じ）
                body[OAuth2AndOIDCConst.client_assertion_type] = OAuth2AndOIDCConst.JwtBearerClientAssertionType;
                body[OAuth2AndOIDCConst.client_assertion] = client_assertion;
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.tls_client_auth)
            {
                // RFC 8705 §2.1（本文はclient_idだけで、証明書はTLSの層で示す）
                body[OAuth2AndOIDCConst.client_id] = client_id;
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT, nameof(authMethod));
            }
        }

        #endregion
    }
}
