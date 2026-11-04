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
//* クラス名        ：IdTokenVerifier
//* クラス日本語名  ：相手（OP）の id_token を、jwks_uri の鍵で検証する
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
using Touryo.Infrastructure.Public.Security.Jwt;

namespace SpRp_CLI
{
    /// <summary>相手（OP）の id_token を、jwks_uri の鍵で検証する</summary>
    /// <remarks>
    /// **鍵は、ヘッダの kid で jwks_uri から引く**（`RemoteJwkSetStore`。鍵のロールオーバーに追随する）。
    /// jwks_uri は Discovery から引き、**Discovery の issuer を照合する**（設定の SpRp_DiscoveryUri / SpRp_Isser）。
    ///
    /// **alg は、扱う非対称の alg（`JwsFactory.SupportedAlgs`）に限る。**
    /// `none` や HS*（共有鍵）を受けると、署名の検証が意味を持たなくなる。
    /// </remarks>
    public class IdTokenVerifier
    {
        /// <summary>id_token を検証する</summary>
        /// <param name="id_token">id_token</param>
        /// <param name="access_token">access_token（at_hash の検証）</param>
        /// <param name="code">code（c_hash の検証）</param>
        /// <param name="issuer">期待する iss</param>
        /// <param name="client_id">期待する aud</param>
        /// <param name="expectedNonce">認可要求で送った nonce（null なら見ない。Device AuthZ には nonce が無い）</param>
        /// <returns>クレーム（検証に失敗したら null）</returns>
        public static async Task<JObject> VerifyAsync(
            string id_token, string access_token, string code,
            string issuer, string client_id, string expectedNonce)
        {
            string alg = JwtReader.TryReadAlg(id_token);
            string kid = JwtReader.TryReadKid(id_token);

            if (!JwsFactory.IsSupported(alg))
            {
                Console.WriteLine("NG   id_token の alg（" + alg + "）は受けません。");
                return null;
            }

            JObject jwk = await RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid);
            if (jwk == null)
            {
                Console.WriteLine("NG   id_token の kid（" + kid + "）の鍵を、jwks_uri から引けません。");
                return null;
            }

            JWS jws = JwsFactory.CreateFromJwk(alg, jwk);
            if (jws == null)
            {
                Console.WriteLine("NG   id_token の alg（" + alg + "）と、鍵の種類が合いません。");
                return null;
            }

            // **署名・iss・aud・exp と、at_hash / c_hash を確かめる**（IdToken.Verify。#601）
            if (!IdToken.Verify(id_token, access_token, code, "",
                jws, issuer, client_id, out string _, out string nonce, out JObject claims))
            {
                Console.WriteLine("NG   id_token の検証に失敗しました（署名・iss・aud・exp・ハッシュのいずれか）。");
                return null;
            }

            // **nonce は、呼ぶ側が確かめる**（IdToken.Verify は取り出すだけ。OpenID Connect Core 1.0 §3.1.3.7 の 11）
            if (expectedNonce != null && nonce != expectedNonce)
            {
                Console.WriteLine("NG   id_token の nonce が、認可要求で送った値と違います。");
                return null;
            }

            return claims;
        }
    }
}
