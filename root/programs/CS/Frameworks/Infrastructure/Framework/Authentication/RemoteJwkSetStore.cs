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
//* クラス名        ：RemoteJwkSetStore
//* クラス日本語名  ：相手の jwks_uri（discovery）から、kid で検証鍵を引く
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の B）
//*  2026/10/22  玄人 幸道         取得を OAuth2AndOIDCClient.GetStringAsync に変えた（discovery も読むため）
//*  2026/10/22  玄人 幸道         設定（JwkSetUri / SpRp_DiscoveryUri / SpRp_Isser）を読む版を追加
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>相手の jwks_uri（discovery）から、kid で検証鍵を引く</summary>
    /// <remarks>
    /// JwkSetStore は、設定の JwkSetUri 1 本だけを持つ（自分の認可サーバの JWK Set）。
    /// こちらは**相手の数だけ**持つ（URL を鍵にしたキャッシュ）。
    /// RP 側（上流の id_token を検証する）と、AS 側（private_key_jwt のクライアントを認証する）の両方が使う。
    ///
    /// | 口 | 使う場面 |
    /// |---|---|
    /// | GetJwkAsync | JWK Set の URL から引く（無ければ取りに行く） |
    /// | GetJwkByDiscoveryAsync | discovery から jwks_uri を引き、issuer を照合して引く |
    /// | PickJwksFromCache | **同期の経路**から、キャッシュだけを読む（取りに行かない） |
    /// | RefillInBackground | 同期の経路で鍵が無かったとき、要求の外で補充する（待たない） |
    /// | WarmUpAsync | 登録のときなど、待てる経路で取りに行く |
    ///
    /// **同期の経路では待たない。** net48 の ASP.NET では、同期の経路で await を待つと、
    /// 継続が AspNetSynchronizationContext に積まれ、その順番を握っている自分が待つので止まる。
    /// そこでキャッシュを読むだけにして、足りなければ背景で補充する（その 1 本は断り、次で通す）。
    ///
    /// **取得は外から起こせる**（kid を偽った JWT を並べる）ので、
    /// 同じ URL の取得は MinRefetchInterval に 1 回まで、同時に 1 本までに抑える。
    ///
    /// 引けなければ null（空）を返し、例外にしない（呼び出し側が別の手段に倒せるように）。
    /// 返す JWK は複製であり、キャッシュを書き換えない。
    /// 検証鍵は JwsFactory.CreateFromJwk(alg, jwk) で作る（alg と鍵の対応もそこで確かめる）。
    /// </remarks>
    public class RemoteJwkSetStore
    {
        #region キャッシュ

        /// <summary>キャッシュの 1 件</summary>
        private class Entry
        {
            /// <summary>JWK Set（keys の配列を持つ JSON。取れなかったときは null）</summary>
            public JObject JwkSet { get; set; }

            /// <summary>取りに行った時刻（UTC。失敗も含む）</summary>
            public DateTime FetchedAt { get; set; }
        }

        /// <summary>取得した JWK Set（鍵は JWK Set の URL、または discovery の URL）</summary>
        private static readonly Dictionary<string, Entry> _cache = new Dictionary<string, Entry>();

        /// <summary>取得中の URL</summary>
        private static readonly HashSet<string> _fetching = new HashSet<string>();

        /// <summary>_cache / _fetching の排他</summary>
        private static readonly object _lock = new object();

        /// <summary>取り直しの下限</summary>
        private static TimeSpan _minRefetchInterval = TimeSpan.FromMinutes(1);

        /// <summary>取り直しの下限（既定 1 分）</summary>
        /// <remarks>
        /// 自由に利用できる。これより短い間隔では、同じ URL を取りに行かない。
        /// プロセス全体に効く。
        /// </remarks>
        public static TimeSpan MinRefetchInterval
        {
            get { return RemoteJwkSetStore._minRefetchInterval; }
            set { RemoteJwkSetStore._minRefetchInterval = value; }
        }

        /// <summary>キャッシュを消す</summary>
        /// <remarks>自由に利用できる（鍵の入れ替えを即座に反映したいとき、試験など）。</remarks>
        public static void Clear()
        {
            lock (RemoteJwkSetStore._lock)
            {
                RemoteJwkSetStore._cache.Clear();
            }
        }

        #endregion

        #region 非同期（待てる経路）

        /// <summary>設定の JwkSetUri から、kid の JWK を引く</summary>
        /// <param name="kid">string（JWT のヘッダの kid）</param>
        /// <returns>JWK（引けなければ null。JwkSetUri が空でも null）</returns>
        /// <remarks>
        /// 自由に利用できる。GetJwkAsync(jwkSetUrl, kid) に、OAuth2AndOIDCParams.JwkSetUri を渡す。
        /// 相手が 1 つの配備向け。相手が複数なら、URL を受ける版を使う。
        /// </remarks>
        public static Task<JObject> GetJwkAsync(string kid)
        {
            return RemoteJwkSetStore.GetJwkAsync(OAuth2AndOIDCParams.JwkSetUri, kid);
        }

        /// <summary>設定の SpRp_DiscoveryUri から jwks_uri を引き、kid の JWK を引く</summary>
        /// <param name="kid">string（JWT のヘッダの kid）</param>
        /// <returns>JWK（引けなければ null。SpRp_DiscoveryUri / SpRp_Isser が空でも null）</returns>
        /// <remarks>
        /// 自由に利用できる。GetJwkByDiscoveryAsync(discoveryUrl, expectedIssuer, kid) に、
        /// CmnClientParams.SpRp_DiscoveryUri と CmnClientParams.Isser（SpRp_Isser）を渡す。
        /// 相手が 1 つの配備向け。相手が複数なら、URL と Issuer を受ける版を使う。
        /// </remarks>
        public static Task<JObject> GetJwkByDiscoveryAsync(string kid)
        {
            return RemoteJwkSetStore.GetJwkByDiscoveryAsync(
                CmnClientParams.SpRp_DiscoveryUri, CmnClientParams.Isser, kid);
        }

        /// <summary>JWK Set の URL から、kid の JWK を引く</summary>
        /// <param name="jwkSetUrl">string（相手の JWK Set の URL）</param>
        /// <param name="kid">string（JWT のヘッダの kid）</param>
        /// <returns>JWK（引けなければ null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// まずキャッシュから引き、無ければ（下限を過ぎていれば）取り直す（鍵のロールオーバー）。
        /// discovery を読まないので、issuer の照合は無い。信頼の根拠は、この URL を設定した人である。
        /// </remarks>
        public static async Task<JObject> GetJwkAsync(string jwkSetUrl, string kid)
        {
            if (string.IsNullOrEmpty(jwkSetUrl) || string.IsNullOrEmpty(kid))
            {
                return null;
            }

            JObject jwk = RemoteJwkSetStore.PickOne(jwkSetUrl, kid, out bool canRefetch);

            if (jwk == null && canRefetch)
            {
                await RemoteJwkSetStore.FetchJwkSetAsync(jwkSetUrl).ConfigureAwait(false);
                jwk = RemoteJwkSetStore.PickOne(jwkSetUrl, kid, out bool _);
            }

            return jwk;
        }

        /// <summary>discovery から jwks_uri を引き、kid の JWK を引く</summary>
        /// <param name="discoveryUrl">string（相手の discovery の URL）</param>
        /// <param name="expectedIssuer">string（期待する Issuer。discovery の issuer と違えば、鍵を使わない）</param>
        /// <param name="kid">string（JWT のヘッダの kid）</param>
        /// <returns>JWK（引けなければ null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// **期待する Issuer を必ず受け取る。** 引いた鍵を、期待する発行者に結び付けるため。
        /// この確認が無いと、設定の URL を差し替えられた配備で、別の発行者の鍵を信じてしまう。
        /// </remarks>
        public static async Task<JObject> GetJwkByDiscoveryAsync(
            string discoveryUrl, string expectedIssuer, string kid)
        {
            if (string.IsNullOrEmpty(discoveryUrl)
                || string.IsNullOrEmpty(expectedIssuer)
                || string.IsNullOrEmpty(kid))
            {
                return null;
            }

            JObject jwk = RemoteJwkSetStore.PickOne(discoveryUrl, kid, out bool canRefetch);

            if (jwk == null && canRefetch)
            {
                string jwksUri = await RemoteJwkSetStore.ResolveJwksUriAsync(
                    discoveryUrl, expectedIssuer).ConfigureAwait(false);

                if (string.IsNullOrEmpty(jwksUri))
                {
                    RemoteJwkSetStore.Store(discoveryUrl, null);
                    return null;
                }

                JObject jwkSet = await RemoteJwkSetStore.GetJsonAsync(jwksUri).ConfigureAwait(false);
                RemoteJwkSetStore.Store(discoveryUrl, jwkSet);

                jwk = RemoteJwkSetStore.PickOne(discoveryUrl, kid, out bool _);
            }

            return jwk;
        }

        /// <summary>discovery を読み、jwks_uri を返す</summary>
        /// <param name="discoveryUrl">string（相手の discovery の URL）</param>
        /// <param name="expectedIssuer">string（期待する Issuer）</param>
        /// <returns>jwks_uri（issuer が違う・読めなければ空文字）</returns>
        /// <remarks>自由に利用できる。キャッシュしない。</remarks>
        public static async Task<string> ResolveJwksUriAsync(string discoveryUrl, string expectedIssuer)
        {
            JObject discovery = await RemoteJwkSetStore.GetJsonAsync(discoveryUrl).ConfigureAwait(false);

            if (discovery == null)
            {
                Debug.WriteLine("RemoteJwkSetStore: the discovery document could not be read.");
                return "";
            }

            if (RemoteJwkSetStore.GetString(discovery, OAuth2AndOIDCConst.issuer) != expectedIssuer)
            {
                Debug.WriteLine("RemoteJwkSetStore: the discovery issuer did not match the expected issuer.");
                return "";
            }

            return RemoteJwkSetStore.GetString(discovery, OAuth2AndOIDCConst.jwks_uri) ?? "";
        }

        /// <summary>取りに行って、キャッシュへ入れる（下限を見ない）</summary>
        /// <param name="jwkSetUrl">string（相手の JWK Set の URL）</param>
        /// <returns>鍵が 1 本でも載ったなら true</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// クライアントの登録のときなど、**待てて、かつ外から自由に起こせない経路**で使う
        /// （鍵を入れ替えた相手が、その場で反映できる）。
        /// 外から起こせる経路では、下限を見る GetJwkAsync / RefillInBackground を使うこと。
        /// </remarks>
        public static async Task<bool> WarmUpAsync(string jwkSetUrl)
        {
            if (string.IsNullOrEmpty(jwkSetUrl))
            {
                return false;
            }

            await RemoteJwkSetStore.FetchJwkSetAsync(jwkSetUrl).ConfigureAwait(false);

            return RemoteJwkSetStore.HasKeysInCache(jwkSetUrl);
        }

        #endregion

        #region 同期（待たない経路）

        /// <summary>キャッシュに在る JWK を引く（取りに行かない）</summary>
        /// <param name="jwkSetUrl">string（相手の JWK Set の URL、または discovery の URL）</param>
        /// <param name="kid">string（空なら、在る鍵を順に返す）</param>
        /// <param name="max">int（kid が空のときに返す本数の上限）</param>
        /// <returns>JWK の一覧（無ければ 0 件）</returns>
        /// <remarks>
        /// 自由に利用できる。同期の経路から呼ぶための口である。
        /// kid は任意（RFC 7515 4.1.4）なので、書かない相手が居る。
        /// そのときは在る鍵を順に試すことになるので、本数に上限を置く。
        /// kty では絞らない（alg と鍵の対応は JwsFactory.CreateFromJwk が確かめる）。
        /// </remarks>
        public static List<JObject> PickJwksFromCache(string jwkSetUrl, string kid, int max)
        {
            List<JObject> ret = new List<JObject>();

            if (string.IsNullOrEmpty(jwkSetUrl))
            {
                return ret;
            }

            lock (RemoteJwkSetStore._lock)
            {
                if (!RemoteJwkSetStore._cache.TryGetValue(jwkSetUrl, out Entry entry)
                    || entry.JwkSet == null)
                {
                    return ret;
                }

                JArray keys = entry.JwkSet[JwtConst.keys] as JArray;

                if (keys == null)
                {
                    return ret;
                }

                foreach (JToken key in keys)
                {
                    JObject jwk = key as JObject;

                    if (jwk == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(kid))
                    {
                        // kid が在るなら、その 1 本だけ。
                        if (RemoteJwkSetStore.GetString(jwk, JwtConst.kid) == kid)
                        {
                            ret.Add((JObject)jwk.DeepClone());
                            break;
                        }

                        continue;
                    }

                    ret.Add((JObject)jwk.DeepClone());

                    if (max <= ret.Count)
                    {
                        break;
                    }
                }
            }

            return ret;
        }

        /// <summary>キャッシュに鍵が在るか</summary>
        /// <param name="jwkSetUrl">string</param>
        /// <returns>1 本でも在れば true</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool HasKeysInCache(string jwkSetUrl)
        {
            return 0 < RemoteJwkSetStore.PickJwksFromCache(jwkSetUrl, "", 1).Count;
        }

        /// <summary>要求の外で、キャッシュを補充する（待たない）</summary>
        /// <param name="jwkSetUrl">string（相手の JWK Set の URL）</param>
        /// <remarks>
        /// 自由に利用できる。
        /// 同期の経路で「鍵が無い」と分かったときに呼ぶ。ここでは待たないので、その 1 本には間に合わない。
        /// 取得中の URL と、下限の内側の URL は取りに行かない。
        /// 同期コンテキストを持たないスレッド（Task.Run）で取りに行く。例外は飲む。
        /// </remarks>
        public static void RefillInBackground(string jwkSetUrl)
        {
            if (string.IsNullOrEmpty(jwkSetUrl))
            {
                return;
            }

            lock (RemoteJwkSetStore._lock)
            {
                if (RemoteJwkSetStore._fetching.Contains(jwkSetUrl))
                {
                    return;
                }

                if (RemoteJwkSetStore._cache.TryGetValue(jwkSetUrl, out Entry entry)
                    && (DateTime.UtcNow - entry.FetchedAt) <= RemoteJwkSetStore._minRefetchInterval)
                {
                    return;
                }

                RemoteJwkSetStore._fetching.Add(jwkSetUrl);
            }

            Task.Run(async () =>
            {
                try
                {
                    await RemoteJwkSetStore.FetchJwkSetAsync(jwkSetUrl).ConfigureAwait(false);
                }
                finally
                {
                    lock (RemoteJwkSetStore._lock)
                    {
                        RemoteJwkSetStore._fetching.Remove(jwkSetUrl);
                    }
                }
            });
        }

        #endregion

        #region URL

        /// <summary>鍵の出どころとして使える URL か</summary>
        /// <param name="jwkSetUrl">string</param>
        /// <returns>https か、ループバックの http なら true</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 平文の http では鍵そのものを差し替えられる（検証鍵なので、差し替えられれば認証が破れる）。
        /// 登録の検査など、URL を受け付ける所で使うこと（取得そのものは、この判定を見ない）。
        /// </remarks>
        public static bool IsUsableKeySource(string jwkSetUrl)
        {
            return UriOperator.IsHttpsOrLoopback(jwkSetUrl);
        }

        /// <summary>口の URL の基底から、discovery の URL を組み立てる</summary>
        /// <param name="anyEndpointUrl">string（相手の口の URL。基底（スキーム・ホスト・ポート）だけを使う）</param>
        /// <returns>discovery の URL（組み立てられなければ空文字）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// Issuer にパスを含む相手（例 : https://host/tenant）には使えない
        /// （OIDC Discovery 4.1 は Issuer の後ろに付ける）。そのときは URL を設定で与えること。
        /// </remarks>
        public static string DiscoveryUrlOf(string anyEndpointUrl)
        {
            if (!Uri.TryCreate(anyEndpointUrl, UriKind.Absolute, out Uri uri))
            {
                return "";
            }

            return uri.GetLeftPart(UriPartial.Authority) + OAuth2AndOIDCConst.WellKnownOpenIdConfiguration;
        }

        #endregion

        #region private

        /// <summary>キャッシュから kid の JWK を 1 本引く</summary>
        /// <param name="key">string（キャッシュの鍵）</param>
        /// <param name="kid">string</param>
        /// <param name="canRefetch">out bool（取り直してよいか）</param>
        /// <returns>JWK（無ければ null）</returns>
        private static JObject PickOne(string key, string kid, out bool canRefetch)
        {
            lock (RemoteJwkSetStore._lock)
            {
                canRefetch = !RemoteJwkSetStore._cache.TryGetValue(key, out Entry entry)
                    || RemoteJwkSetStore._minRefetchInterval < (DateTime.UtcNow - entry.FetchedAt);
            }

            List<JObject> jwks = RemoteJwkSetStore.PickJwksFromCache(key, kid, 1);
            return (jwks.Count == 0) ? null : jwks[0];
        }

        /// <summary>JWK Set を取ってキャッシュする</summary>
        /// <param name="jwkSetUrl">string</param>
        /// <returns>Task</returns>
        /// <remarks>失敗しても取りに行った時刻は記録する（失敗した相手を叩き続けないため）。</remarks>
        private static async Task FetchJwkSetAsync(string jwkSetUrl)
        {
            JObject jwkSet = await RemoteJwkSetStore.GetJsonAsync(jwkSetUrl).ConfigureAwait(false);
            RemoteJwkSetStore.Store(jwkSetUrl, jwkSet);
        }

        /// <summary>キャッシュへ入れる</summary>
        /// <param name="key">string</param>
        /// <param name="jwkSet">JObject（取れなかったときは null）</param>
        /// <remarks>取れなかったときは、前に取れた JWK Set を残す（時刻だけ進める）。</remarks>
        private static void Store(string key, JObject jwkSet)
        {
            lock (RemoteJwkSetStore._lock)
            {
                if (jwkSet == null && RemoteJwkSetStore._cache.TryGetValue(key, out Entry old))
                {
                    jwkSet = old.JwkSet;
                }

                RemoteJwkSetStore._cache[key] = new Entry()
                {
                    JwkSet = jwkSet,
                    FetchedAt = DateTime.UtcNow
                };
            }
        }

        /// <summary>URL を GET して、JSON のオブジェクトとして読む</summary>
        /// <param name="url">string</param>
        /// <returns>JObject（読めなければ null）</returns>
        /// <remarks>
        /// 取得は OAuth2AndOIDCClient の HttpClient で行う（TLS・プロキシの扱いが揃う）。
        /// </remarks>
        private static async Task<JObject> GetJsonAsync(string url)
        {
            try
            {
                string json = await OAuth2AndOIDCClient.GetStringAsync(new Uri(url)).ConfigureAwait(false);

                if (string.IsNullOrEmpty(json))
                {
                    Debug.WriteLine("RemoteJwkSetStore: empty response.");
                    return null;
                }

                return JToken.Parse(json) as JObject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RemoteJwkSetStore: " + ex.GetType().Name);
                return null;
            }
        }

        /// <summary>文字列の項目を読む（文字列でなければ null）</summary>
        /// <param name="obj">JObject</param>
        /// <param name="name">string</param>
        /// <returns>値</returns>
        private static string GetString(JObject obj, string name)
        {
            JToken token = obj[name];
            return (token != null && token.Type == JTokenType.String) ? (string)token : null;
        }

        #endregion
    }
}
