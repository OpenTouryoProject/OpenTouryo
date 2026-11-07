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
//* クラス名        ：OpJwt
//* クラス日本語名  ：相手（OP）の JWT（id_token・JARM の応答）を、jwks_uri の鍵で検証する
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/07  玄人 幸道         新規作成（#668。SpRp_CLI の IdTokenVerifier に、JARM の応答を足した）
//**********************************************************************************

using System;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace SpRp_MVC.Auth
{
    /// <summary>相手（OP）の JWT を、jwks_uri の鍵で検証する</summary>
    /// <remarks>
    /// **鍵は、ヘッダの kid で jwks_uri から引く**（`RemoteJwkSetStore`。鍵のロールオーバーに追随する）。
    /// jwks_uri は Discovery から引き、**Discovery の issuer を照合する**（設定の SpRp_DiscoveryUri / SpRp_Issuer）。
    ///
    /// **alg は、扱う非対称の alg（`JwsFactory.SupportedAlgs`）に限る。**
    /// `none` や HS*（共有鍵）を受けると、署名の検証が意味を持たなくなる。
    ///
    /// **失敗は例外にせず、理由を返す**（画面に出す。null なら成功）。
    /// </remarks>
    public static class OpJwt
    {
        /// <summary>id_token を検証する</summary>
        /// <param name="id_token">id_token</param>
        /// <param name="access_token">access_token（at_hash の検証）</param>
        /// <param name="code">code（c_hash の検証）</param>
        /// <param name="issuer">期待する iss</param>
        /// <param name="client_id">期待する aud</param>
        /// <param name="expectedNonce">認可要求で送った nonce</param>
        /// <returns>（失敗の理由、クレーム）。成功なら理由は null</returns>
        public static async Task<(string Error, JObject Claims)> VerifyIdTokenAsync(
            string id_token, string access_token, string code,
            string issuer, string client_id, string expectedNonce)
        {
            (string error, JWS jws) = await OpJwt.ResolveKeyAsync(id_token, "id_token");
            if (error != null)
            {
                return (error, null);
            }

            // **署名・iss・aud・exp と、at_hash / c_hash を確かめる**（IdToken.Verify。#601）
            //   **ハッシュは、在れば確かめる**（認可コード フローでは、どちらも任意。OpenID Connect Core 1.0 §3.1.3.6）
            if (!IdToken.Verify(id_token, access_token, code, "",
                jws, issuer, client_id, out string _, out string nonce, out JObject claims))
            {
                return ("id_token の検証に失敗しました（署名・iss・aud・exp・ハッシュのいずれか）。", null);
            }

            // **nonce は、呼ぶ側が確かめる**（IdToken.Verify は取り出すだけ。OpenID Connect Core 1.0 §3.1.3.7 の 11）
            if (nonce != expectedNonce)
            {
                return ("id_token の nonce が、認可要求で送った値と違います。", null);
            }

            return (null, claims);
        }

        /// <summary>JARM の応答（response）を検証する</summary>
        /// <param name="response">認可応答の response（JWT）</param>
        /// <param name="issuer">期待する iss</param>
        /// <param name="client_id">期待する aud</param>
        /// <returns>（失敗の理由、ペイロード）。成功なら理由は null</returns>
        /// <remarks>
        /// **署名・iss・aud・exp を確かめる**（JARM §4.4。ResponseObject.Verify）。
        /// **エラーの応答も同じく確かめる**（#644）。エラーの中身は、返したペイロードの error で見る。
        /// </remarks>
        public static async Task<(string Error, JObject Payload)> VerifyResponseObjectAsync(
            string response, string issuer, string client_id)
        {
            (string error, JWS jws) = await OpJwt.ResolveKeyAsync(response, "応答（JARM）");
            if (error != null)
            {
                return (error, null);
            }

            if (!ResponseObject.Verify(response, jws, issuer, client_id, out JObject payload))
            {
                return ("応答（JARM）の検証に失敗しました（署名・iss・aud・exp のいずれか）。", null);
            }

            return (null, payload);
        }

        /// <summary>ヘッダの alg と kid から、検証鍵を作る</summary>
        /// <param name="jwt">JWT</param>
        /// <param name="what">何の JWT か（理由の文言）</param>
        /// <returns>（失敗の理由、検証鍵）</returns>
        private static async Task<(string Error, JWS Jws)> ResolveKeyAsync(string jwt, string what)
        {
            string alg = JwtReader.TryReadAlg(jwt);
            string kid = JwtReader.TryReadKid(jwt);

            if (!JwsFactory.IsSupported(alg))
            {
                return (what + " の alg（" + alg + "）は受けません。", null);
            }

            JObject jwk = await RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid);
            if (jwk == null)
            {
                return (what + " の kid（" + kid + "）の鍵を、jwks_uri から引けません。", null);
            }

            JWS jws = JwsFactory.CreateFromJwk(alg, jwk);
            if (jws == null)
            {
                return (what + " の alg（" + alg + "）と、鍵の種類が合いません。", null);
            }

            return (null, jws);
        }
    }
}
