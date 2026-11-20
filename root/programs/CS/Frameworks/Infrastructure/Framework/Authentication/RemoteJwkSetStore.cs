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
//*  2026/11/06  玄人 幸道         remarks の設定キーを、改名後の名前（SpRp_JwkSetUri・SpRp_Issuer）にした（#666）
//*  2026/11/20  玄人 幸道         jwks_uri などを取りに行く HttpClient を差し替える HttpClient プロパティを追加（#707）
//*  2026/11/20  玄人 幸道         同じ URL の取得を同時に 1 本にまとめた（重なった要求は完了を待つ。#686）
//*  2026/11/20  玄人 幸道         GetJwkByDiscoveryAsync が、キャッシュの鍵を引くときも expectedIssuer を照合するようにした（#715）
//*  2026/11/20  玄人 幸道         取得の口に、呼び出しごとに HttpClient を渡せる版を追加。取得中のまとめと取り直しの下限を、URL と HttpClient の組ごとにした（#718）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using System.Net.Http;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>相手の jwks_uri（discovery）から、kid で検証鍵を引く</summary>
    /// <remarks>
    /// JwkSetStore は、設定の SpRp_JwkSetUri 1 本だけを持つ（この配備が信頼する認可サーバの JWK Set。改名前の JwkSetUri は読まない）。
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
    /// | HttpClient（プロパティ） | 取りに行く HttpClient を差し替える（既定は null＝共有の OAuth2AndOIDCClient.HttpClient。プロセス全体に効く。#707） |
    /// | httpClient（引数） | GetJwkAsync・GetJwkByDiscoveryAsync・ResolveJwksUriAsync・WarmUpAsync・RefillInBackground の各オーバーロード。呼び出しごとに取りに行く HttpClient を選ぶ（#718） |
    ///
    /// **取りに行く HttpClient は、引数 → HttpClient プロパティ → 共有の OAuth2AndOIDCClient.HttpClient の順に決める**（引数が null なら、次へ）。
    /// 同じプロセスで行き先の種類ごとに HttpClient を使い分けたいとき（動的登録でクライアントが登録した jwks_uri は絞った HttpClient、
    /// 上流の IdP の jwks_uri は共有の HttpClient、など）は、引数で渡す。
    /// **取得中のまとめと取り直しの下限は、（URL, HttpClient）の組ごと**に数える（絞った HttpClient の取得が断られても、
    /// 共有の HttpClient の取得を巻き込まない）。**取れた鍵のキャッシュは URL ごとに共有する**（同じ URL なら同じ中身なので）。
    ///
    /// **同期の経路では待たない。** net48 の ASP.NET では、同期の経路で await を待つと、
    /// 継続が AspNetSynchronizationContext に積まれ、その順番を握っている自分が待つので止まる。
    /// そこでキャッシュを読むだけにして、足りなければ背景で補充する（その 1 本は断り、次で通す）。
    ///
    /// **取得は外から起こせる**（kid を偽った JWT を並べる）ので、
    /// 同じ URL の取得は MinRefetchInterval に 1 回まで、同時に 1 本までに抑える
    /// （GetJwkAsync・GetJwkByDiscoveryAsync・RefillInBackground。
    /// 取得中に来た要求は、重ねて取りに行かず、その取得の完了を待ってキャッシュを引く。#686）。
    /// WarmUpAsync は、外から起こせない経路なので、取得中でも自分で取りに行く（入れ替えた直後の鍵を取りこぼさないため）。
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

            /// <summary>この JWK Set を取ったときに、discovery の issuer と照合した Issuer（discovery を使わない口では null。#715）</summary>
            public string Issuer { get; set; }
        }

        /// <summary>取得の単位（URL と、取りに行く HttpClient の組。#718）</summary>
        /// <remarks>
        /// HttpClient は、引数 → HttpClient プロパティ の順で決めたもの（null は、共有の OAuth2AndOIDCClient.HttpClient）。
        /// HttpClient は参照で比べる。
        /// </remarks>
        private struct FetchKey : IEquatable<FetchKey>
        {
            public FetchKey(string url, HttpClient httpClient)
            {
                this.Url = url;
                this.HttpClient = httpClient;
            }

            public string Url { get; }

            public HttpClient HttpClient { get; }

            public bool Equals(FetchKey other)
            {
                return string.Equals(this.Url, other.Url, StringComparison.Ordinal)
                    && object.ReferenceEquals(this.HttpClient, other.HttpClient);
            }

            public override bool Equals(object obj)
            {
                return (obj is FetchKey) && this.Equals((FetchKey)obj);
            }

            public override int GetHashCode()
            {
                return (this.Url == null ? 0 : this.Url.GetHashCode() * 31)
                    ^ (this.HttpClient == null ? 0 : System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this.HttpClient));
            }
        }

        /// <summary>取得した JWK Set（鍵は JWK Set の URL、または discovery の URL。HttpClient が違っても共有する。#718）</summary>
        private static readonly Dictionary<string, Entry> _cache = new Dictionary<string, Entry>();

        /// <summary>取得中の（URL, HttpClient）と、その取得の完了を待つ Task（#686。組ごと。#718）</summary>
        private static readonly Dictionary<FetchKey, Task> _fetching = new Dictionary<FetchKey, Task>();

        /// <summary>（URL, HttpClient）ごとに、取りに行った時刻（UTC。失敗も含む。取り直しの下限に使う。#718）</summary>
        /// <remarks>
        /// 絞った HttpClient の取得が断られて時刻が進んでも、共有の HttpClient の取得を下限で止めないよう、組ごとに持つ。
        /// </remarks>
        private static readonly Dictionary<FetchKey, DateTime> _attempted = new Dictionary<FetchKey, DateTime>();

        /// <summary>_cache / _fetching / _attempted の排他</summary>
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

        /// <summary>取得に使う HttpClient（null なら、OAuth2AndOIDCClient.HttpClient の共有のもの）</summary>
        private static volatile HttpClient _httpClient = null;

        /// <summary>jwks_uri（と discovery）を取りに行く HttpClient（既定は null。#707）</summary>
        /// <remarks>
        /// 自由に利用できる。**null なら、いまどおり OAuth2AndOIDCClient.HttpClient（共有）で取りに行く。**
        ///
        /// 登録できる相手が jwks_uri を書き、こちらに取りに行かせる構成（動的登録。RFC 7591）で、
        /// **取りに行く先を絞った HttpClient**（リダイレクトを追わない・接続の時点のアドレスを判定する、など）を使うための口である。
        /// 共有の HttpClient は、上流の IdP・Back-Channel Logout など正規の行き先も通るので、絞れない。
        ///
        /// **プロセス全体に効く**（OAuth2AndOIDCClient.HttpClient と同じ）。
        /// **呼び出しごとに HttpClient を渡さなければ、この部品が取りに行くものは、すべてこの HttpClient で取りに行く**
        /// （jwks_uri・discovery・GetJwkByDiscoveryAsync が読む jwks_uri）。
        /// 上流の IdP の jwks_uri も同じ口を通るので、絞った HttpClient をここへ渡すと、上流にも掛かる。
        /// **行き先の種類ごとに使い分けたいときは、各口の httpClient 引数で渡す**（引数が先に使われる。#718）。
        /// **この部品を通らない取得には効かない**（OAuth2AndOIDCClient.GetJwkSetAsync・Discovery を直接読む口は、共有の HttpClient のまま）。
        ///
        /// 取得のたびに読むので、設定した後の取得から効く（取得済みのキャッシュは、そのまま）。
        /// OAuth2AndOIDCClient.HttpClient とは別に持つ（こちらが null でなければ、jwks_uri・discovery の取得は共有のものを使わない）。
        /// </remarks>
        public static HttpClient HttpClient
        {
            get { return RemoteJwkSetStore._httpClient; }
            set { RemoteJwkSetStore._httpClient = value; }
        }

        /// <summary>キャッシュを消す</summary>
        /// <remarks>自由に利用できる（鍵の入れ替えを即座に反映したいとき、試験など）。取り直しの下限の記録も消す。</remarks>
        public static void Clear()
        {
            lock (RemoteJwkSetStore._lock)
            {
                RemoteJwkSetStore._cache.Clear();
                RemoteJwkSetStore._attempted.Clear();
            }
        }

        /// <summary>取りに行く HttpClient を決める（引数 → HttpClient プロパティ。どちらも null なら null＝共有）</summary>
        /// <param name="httpClient">HttpClient（呼び出しごとに渡されたもの）</param>
        /// <returns>HttpClient（null は、共有の OAuth2AndOIDCClient.HttpClient）</returns>
        private static HttpClient Resolve(HttpClient httpClient)
        {
            return httpClient ?? RemoteJwkSetStore._httpClient;
        }

        #endregion

        #region 非同期（待てる経路）

        /// <summary>設定の SpRp_JwkSetUri から、kid の JWK を引く（改名前の JwkSetUri は読まない。#666）</summary>
        /// <param name="kid">string（JWT のヘッダの kid）</param>
        /// <returns>JWK（引けなければ null。SpRp_JwkSetUri が空でも null）</returns>
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
        /// <returns>JWK（引けなければ null。SpRp_DiscoveryUri / SpRp_Issuer（改名前は SpRp_Isser）が空でも null）</returns>
        /// <remarks>
        /// 自由に利用できる。GetJwkByDiscoveryAsync(discoveryUrl, expectedIssuer, kid) に、
        /// CmnClientParams.SpRp_DiscoveryUri と CmnClientParams.Isser（SpRp_Issuer）を渡す。
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
        /// 同じ URL を取得中なら、重ねて取りに行かず、その取得の完了を待ってから引く（#686）。
        /// discovery を読まないので、issuer の照合は無い。信頼の根拠は、この URL を設定した人である。
        /// </remarks>
        public static Task<JObject> GetJwkAsync(string jwkSetUrl, string kid)
        {
            return RemoteJwkSetStore.GetJwkAsync(jwkSetUrl, kid, null);
        }

        /// <summary>JWK Set の URL から、kid の JWK を引く（取りに行く HttpClient を指定する版。#718）</summary>
        /// <param name="jwkSetUrl">string（相手の JWK Set の URL）</param>
        /// <param name="kid">string（JWT のヘッダの kid）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで取りに行くもの。null なら HttpClient プロパティ、それも null なら共有の OAuth2AndOIDCClient.HttpClient）</param>
        /// <returns>JWK（引けなければ null）</returns>
        /// <remarks>
        /// 自由に利用できる。動作は GetJwkAsync(jwkSetUrl, kid) と同じで、取りに行く HttpClient だけを呼び出しごとに選べる。
        /// **取得中のまとめと、取り直しの下限は、（URL, HttpClient）の組ごと**に数える。
        /// 取れた鍵のキャッシュは URL ごとに共有する（同じ URL なら同じ中身なので）。
        /// </remarks>
        public static async Task<JObject> GetJwkAsync(string jwkSetUrl, string kid, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(jwkSetUrl) || string.IsNullOrEmpty(kid))
            {
                return null;
            }

            HttpClient client = RemoteJwkSetStore.Resolve(httpClient);
            JObject jwk = RemoteJwkSetStore.PickOne(jwkSetUrl, client, kid, out bool canRefetch);

            if (jwk == null && canRefetch)
            {
                await RemoteJwkSetStore.FetchOnceAsync(
                    jwkSetUrl, client, () => RemoteJwkSetStore.FetchJwkSetAsync(jwkSetUrl, client)).ConfigureAwait(false);
                jwk = RemoteJwkSetStore.PickOne(jwkSetUrl, client, kid, out bool _);
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
        ///
        /// **キャッシュから引くときも照合する**（#715）。キャッシュの 1 件は、取ったときに照合した Issuer を持つ。
        /// 同じ discoveryUrl を別の expectedIssuer で呼んだときは、その鍵を使わない
        /// （下限を過ぎていれば discovery を取り直し、そこで照合する。内側なら取りに行かず null）。
        /// </remarks>
        public static Task<JObject> GetJwkByDiscoveryAsync(
            string discoveryUrl, string expectedIssuer, string kid)
        {
            return RemoteJwkSetStore.GetJwkByDiscoveryAsync(discoveryUrl, expectedIssuer, kid, null);
        }

        /// <summary>discovery から jwks_uri を引き、kid の JWK を引く（取りに行く HttpClient を指定する版。#718）</summary>
        /// <param name="discoveryUrl">string（相手の discovery の URL）</param>
        /// <param name="expectedIssuer">string（期待する Issuer。discovery の issuer と違えば、鍵を使わない）</param>
        /// <param name="kid">string（JWT のヘッダの kid）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで取りに行くもの。null なら HttpClient プロパティ、それも null なら共有の OAuth2AndOIDCClient.HttpClient）</param>
        /// <returns>JWK（引けなければ null）</returns>
        /// <remarks>
        /// 自由に利用できる。動作は GetJwkByDiscoveryAsync(discoveryUrl, expectedIssuer, kid) と同じで、
        /// **discovery と、そこから引いた jwks_uri の両方を、渡した HttpClient で取りに行く。**
        /// 取得中のまとめと、取り直しの下限は、（discoveryUrl, HttpClient）の組ごとに数える。
        /// 取れた鍵（と、照合した Issuer）のキャッシュは discoveryUrl ごとに共有する。
        /// </remarks>
        public static async Task<JObject> GetJwkByDiscoveryAsync(
            string discoveryUrl, string expectedIssuer, string kid, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(discoveryUrl)
                || string.IsNullOrEmpty(expectedIssuer)
                || string.IsNullOrEmpty(kid))
            {
                return null;
            }

            HttpClient client = RemoteJwkSetStore.Resolve(httpClient);
            JObject jwk = RemoteJwkSetStore.PickOne(discoveryUrl, client, kid, out bool canRefetch, expectedIssuer);

            if (jwk == null && canRefetch)
            {
                // discovery の取得から jwks_uri の取得・キャッシュまでを、1 本にまとめる（#686）。
                // 発行者が違う・読めないときは、Store(null) で時刻だけ進め、PickOne が null を返す（従来と同じ結果）。
                await RemoteJwkSetStore.FetchOnceAsync(discoveryUrl, client, async () =>
                {
                    string jwksUri = await RemoteJwkSetStore.ResolveJwksUriAsync(
                        discoveryUrl, expectedIssuer, client).ConfigureAwait(false);

                    if (string.IsNullOrEmpty(jwksUri))
                    {
                        RemoteJwkSetStore.Store(discoveryUrl, client, null);
                        return;
                    }

                    JObject jwkSet = await RemoteJwkSetStore.GetJsonAsync(jwksUri, client).ConfigureAwait(false);
                    RemoteJwkSetStore.Store(discoveryUrl, client, jwkSet, expectedIssuer);
                }).ConfigureAwait(false);

                jwk = RemoteJwkSetStore.PickOne(discoveryUrl, client, kid, out bool _, expectedIssuer);
            }

            return jwk;
        }

        /// <summary>discovery を読み、jwks_uri を返す</summary>
        /// <param name="discoveryUrl">string（相手の discovery の URL）</param>
        /// <param name="expectedIssuer">string（期待する Issuer）</param>
        /// <returns>jwks_uri（issuer が違う・読めなければ空文字）</returns>
        /// <remarks>自由に利用できる。キャッシュしない。</remarks>
        public static Task<string> ResolveJwksUriAsync(string discoveryUrl, string expectedIssuer)
        {
            return RemoteJwkSetStore.ResolveJwksUriAsync(discoveryUrl, expectedIssuer, null);
        }

        /// <summary>discovery を読み、jwks_uri を返す（取りに行く HttpClient を指定する版。#718）</summary>
        /// <param name="discoveryUrl">string（相手の discovery の URL）</param>
        /// <param name="expectedIssuer">string（期待する Issuer）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで取りに行くもの。null なら HttpClient プロパティ、それも null なら共有の OAuth2AndOIDCClient.HttpClient）</param>
        /// <returns>jwks_uri（issuer が違う・読めなければ空文字）</returns>
        /// <remarks>自由に利用できる。キャッシュしない。</remarks>
        public static async Task<string> ResolveJwksUriAsync(
            string discoveryUrl, string expectedIssuer, HttpClient httpClient)
        {
            JObject discovery = await RemoteJwkSetStore.GetJsonAsync(
                discoveryUrl, RemoteJwkSetStore.Resolve(httpClient)).ConfigureAwait(false);

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
        /// **取得中かどうかは見ない**（取得中でも、自分で取りに行く）。取得中の取得は、鍵を入れ替える前に始まっていて、
        /// 古い鍵を受け取りうる。この口は、入れ替えた相手の新しい鍵を、その場で反映するためのものである（#686 で、この扱いを残した）。
        /// </remarks>
        public static Task<bool> WarmUpAsync(string jwkSetUrl)
        {
            return RemoteJwkSetStore.WarmUpAsync(jwkSetUrl, null);
        }

        /// <summary>取りに行って、キャッシュへ入れる（下限を見ない。取りに行く HttpClient を指定する版。#718）</summary>
        /// <param name="jwkSetUrl">string（相手の JWK Set の URL）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで取りに行くもの。null なら HttpClient プロパティ、それも null なら共有の OAuth2AndOIDCClient.HttpClient）</param>
        /// <returns>鍵が 1 本でも載ったなら true</returns>
        /// <remarks>
        /// 自由に利用できる。動作は WarmUpAsync(jwkSetUrl) と同じ（取得中かどうかは見ない）。
        /// 取れた鍵は URL ごとのキャッシュに入るので、別の HttpClient で取った同じ URL の鍵も、戻り値の判定（true）に含まれる。
        /// </remarks>
        public static async Task<bool> WarmUpAsync(string jwkSetUrl, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(jwkSetUrl))
            {
                return false;
            }

            await RemoteJwkSetStore.FetchJwkSetAsync(
                jwkSetUrl, RemoteJwkSetStore.Resolve(httpClient)).ConfigureAwait(false);

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
        /// **発行者（Issuer）の照合はしない**（Issuer を受け取らない口）。照合するのは GetJwkByDiscoveryAsync で、
        /// discovery の URL で引くときは、先にその口で照合を通した URL にだけ使うこと（#715）。
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
            RemoteJwkSetStore.RefillInBackground(jwkSetUrl, null);
        }

        /// <summary>要求の外で、キャッシュを補充する（待たない。取りに行く HttpClient を指定する版。#718）</summary>
        /// <param name="jwkSetUrl">string（相手の JWK Set の URL）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで取りに行くもの。null なら HttpClient プロパティ、それも null なら共有の OAuth2AndOIDCClient.HttpClient）</param>
        /// <remarks>
        /// 自由に利用できる。動作は RefillInBackground(jwkSetUrl) と同じ。
        /// 取得中かどうかと下限は、（URL, HttpClient）の組ごとに見る。
        /// </remarks>
        public static void RefillInBackground(string jwkSetUrl, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(jwkSetUrl))
            {
                return;
            }

            HttpClient client = RemoteJwkSetStore.Resolve(httpClient);
            FetchKey fetchKey = new FetchKey(jwkSetUrl, client);
            TaskCompletionSource<bool> owned;

            lock (RemoteJwkSetStore._lock)
            {
                if (RemoteJwkSetStore._fetching.ContainsKey(fetchKey))
                {
                    return;
                }

                if (RemoteJwkSetStore._attempted.TryGetValue(fetchKey, out DateTime attemptedAt)
                    && (DateTime.UtcNow - attemptedAt) <= RemoteJwkSetStore._minRefetchInterval)
                {
                    return;
                }

                // 取得中として、ここで（ロックの中で）登録する。Task.Run の始まりを待たないので、続けて呼ばれても重ならない。
                owned = RemoteJwkSetStore.RegisterFetchLocked(fetchKey);
            }

            Task.Run(async () =>
            {
                try
                {
                    await RemoteJwkSetStore.RunFetchAsync(
                        fetchKey, () => RemoteJwkSetStore.FetchJwkSetAsync(jwkSetUrl, client), owned).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RemoteJwkSetStore: " + ex.GetType().Name);
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
        /// <param name="httpClient">HttpClient（取りに行くもの。取り直してよいかを、この組の取得時刻で決める。#718）</param>
        /// <param name="kid">string</param>
        /// <param name="canRefetch">out bool（取り直してよいか）</param>
        /// <param name="expectedIssuer">string（null でなければ、キャッシュの 1 件の Issuer と一致するときだけ引く。#715）</param>
        /// <returns>JWK（無ければ null）</returns>
        private static JObject PickOne(
            string key, HttpClient httpClient, string kid, out bool canRefetch, string expectedIssuer = null)
        {
            // 照合と引くのを同じロックの中で行う（間に Store が入って、別の Issuer の鍵を引かないように）。
            lock (RemoteJwkSetStore._lock)
            {
                // 取り直しの下限は、（URL, HttpClient）の組ごとに見る（別の HttpClient の失敗で、止めないため。#718）。
                canRefetch = !RemoteJwkSetStore._attempted.TryGetValue(new FetchKey(key, httpClient), out DateTime attemptedAt)
                    || RemoteJwkSetStore._minRefetchInterval < (DateTime.UtcNow - attemptedAt);

                RemoteJwkSetStore._cache.TryGetValue(key, out Entry entry);

                if (expectedIssuer != null && entry != null
                    && !string.Equals(entry.Issuer, expectedIssuer, StringComparison.Ordinal))
                {
                    return null;
                }

                List<JObject> jwks = RemoteJwkSetStore.PickJwksFromCache(key, kid, 1);
                return (jwks.Count == 0) ? null : jwks[0];
            }
        }

        /// <summary>取得中として登録する（_lock を持った状態で呼ぶ）</summary>
        /// <param name="key">FetchKey（URL と HttpClient の組）</param>
        /// <returns>取得を終えたときに完了させる TaskCompletionSource（呼んだ側が持ち主）</returns>
        private static TaskCompletionSource<bool> RegisterFetchLocked(FetchKey key)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            RemoteJwkSetStore._fetching[key] = tcs.Task;
            return tcs;
        }

        /// <summary>同じ URL・同じ HttpClient の取得を、同時に 1 本にまとめる（#686。HttpClient が違えば別の取得。#718）</summary>
        /// <param name="url">string（キャッシュの鍵）</param>
        /// <param name="httpClient">HttpClient（取りに行くもの。null は共有）</param>
        /// <param name="fetch">Func&lt;Task&gt;（取得してキャッシュへ入れる処理）</param>
        /// <returns>
        /// 取得中でなければ、取得を行う Task（例外はそのまま伝わる）。
        /// 取得中なら、その取得の完了を待つ Task（取得が失敗しても正常に完了する。結果はキャッシュを引いて確かめる）
        /// </returns>
        private static Task FetchOnceAsync(string url, HttpClient httpClient, Func<Task> fetch)
        {
            FetchKey key = new FetchKey(url, httpClient);
            TaskCompletionSource<bool> owned;

            lock (RemoteJwkSetStore._lock)
            {
                if (RemoteJwkSetStore._fetching.TryGetValue(key, out Task running))
                {
                    return running;
                }

                owned = RemoteJwkSetStore.RegisterFetchLocked(key);
            }

            return RemoteJwkSetStore.RunFetchAsync(key, fetch, owned);
        }

        /// <summary>取得を行い、終えたら取得中の印を外して、待っている側を起こす</summary>
        /// <param name="key">FetchKey（URL と HttpClient の組）</param>
        /// <param name="fetch">Func&lt;Task&gt;</param>
        /// <param name="owned">TaskCompletionSource&lt;bool&gt;（RegisterFetchLocked の戻り値）</param>
        /// <returns>Task</returns>
        /// <remarks>失敗しても、必ず印を外して完了させる（待っている側を止めない）。</remarks>
        private static async Task RunFetchAsync(FetchKey key, Func<Task> fetch, TaskCompletionSource<bool> owned)
        {
            try
            {
                await fetch().ConfigureAwait(false);
            }
            finally
            {
                lock (RemoteJwkSetStore._lock)
                {
                    RemoteJwkSetStore._fetching.Remove(key);
                }

                owned.TrySetResult(true);
            }
        }

        /// <summary>JWK Set を取ってキャッシュする</summary>
        /// <param name="jwkSetUrl">string</param>
        /// <param name="httpClient">HttpClient（取りに行くもの。null は共有）</param>
        /// <returns>Task</returns>
        /// <remarks>失敗しても取りに行った時刻は記録する（失敗した相手を叩き続けないため）。</remarks>
        private static async Task FetchJwkSetAsync(string jwkSetUrl, HttpClient httpClient)
        {
            JObject jwkSet = await RemoteJwkSetStore.GetJsonAsync(jwkSetUrl, httpClient).ConfigureAwait(false);
            RemoteJwkSetStore.Store(jwkSetUrl, httpClient, jwkSet);
        }

        /// <summary>キャッシュへ入れる</summary>
        /// <param name="key">string</param>
        /// <param name="httpClient">HttpClient（取りに行ったもの。この組の取得時刻を進める。#718）</param>
        /// <param name="jwkSet">JObject（取れなかったときは null）</param>
        /// <param name="issuer">string（discovery の issuer と照合した Issuer。discovery を使わない口では null。#715）</param>
        /// <remarks>
        /// 取れなかったときは、前に取れた JWK Set と、その Issuer を残す（この組の時刻だけ進める）。
        /// 鍵のキャッシュは URL ごとで、HttpClient が違っても共有する（#718）。
        /// </remarks>
        private static void Store(string key, HttpClient httpClient, JObject jwkSet, string issuer = null)
        {
            lock (RemoteJwkSetStore._lock)
            {
                if (jwkSet == null && RemoteJwkSetStore._cache.TryGetValue(key, out Entry old))
                {
                    jwkSet = old.JwkSet;
                    issuer = old.Issuer;
                }

                RemoteJwkSetStore._cache[key] = new Entry()
                {
                    JwkSet = jwkSet,
                    Issuer = issuer
                };

                RemoteJwkSetStore._attempted[new FetchKey(key, httpClient)] = DateTime.UtcNow;
            }
        }

        /// <summary>URL を GET して、JSON のオブジェクトとして読む</summary>
        /// <param name="url">string</param>
        /// <param name="httpClient">HttpClient（取りに行くもの。null なら、共有の OAuth2AndOIDCClient.HttpClient。TLS・プロキシの扱いが揃う）</param>
        /// <returns>JObject（読めなければ null）</returns>
        /// <remarks>HttpClient は、呼び出し側で Resolve（引数 → HttpClient プロパティ）済みのものを受け取る（#707・#718）。</remarks>
        private static async Task<JObject> GetJsonAsync(string url, HttpClient httpClient)
        {
            try
            {
                string json = await OAuth2AndOIDCClient.GetStringAsync(new Uri(url), httpClient).ConfigureAwait(false);

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
