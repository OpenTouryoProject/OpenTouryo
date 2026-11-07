// Apache License
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

// ---------------------------------------------------------------
// 取り込み元 : FrontendTemplates（UI/SPA/React/vite-react-ts/src/touryo/oauth_oidc.ts、76bd771）
//   このサンプル（Sample4SpRp、#649）用に手を入れてある。
//
// ＜フロー＞ SPA 用の PKCE プロファイル（MPAS）
//   認可コード ＋ PKCE（S256。RFC 7636）、応答はフラグメント（response_mode=fragment）。
//   **秘密を持たない（パブリック クライアント）。** トークン要求は code_verifier で引き換える。
//
// ＜確かめること＞
//   | 値 | 何のため |
//   |---|---|
//   | state | 自分が出した要求への応答か（CSRF。RFC 6749 §10.12） |
//   | 応答の iss（在れば） | どの IdP の応答か（Mix-Up。RFC 9207） |
//   | id_token の iss・aud・exp・nonce | OpenID Connect Core 1.0 §3.1.3.7 |
//   | UserInfo の sub | id_token の sub と同じか（OpenID Connect Core 1.0 §5.3.2） |
//
//   **id_token の署名は確かめない。** トークン エンドポイントから TLS で直接受けた id_token は、
//   署名の代わりに TLS のサーバ検証で発行者を確かめてよい（OpenID Connect Core 1.0 §3.1.3.7 の 6）。
//
// ＜Hybrid（code id_token。#668）＞
//   汎用認証サイトの自己テストから移した。**クライアントは SPA_Hybrid**（const.ts。秘密なし）。
//   **認可応答（フラグメント）で id_token が届くので、その id_token は署名を確かめる**
//   （フロント チャネルは TLS で発行者を確かめられない。OpenID Connect Core 1.0 §3.3.2.12 → §3.2.2.11）。
//   | 値 | 何のため |
//   |---|---|
//   | 認可応答の id_token の署名 | jwks_uri の鍵（kid で引く）。alg は RS256・ES256・ES384・ES512 だけ（none・HS* を受けない） |
//   | 認可応答の id_token の iss・aud・exp・nonce | 上と同じ |
//   | 認可応答の id_token の c_hash | **必須**（§3.3.2.11）。届いた code と結び付いているか（code の差し替え） |
//   | トークン応答の id_token の iss・sub | 認可応答の id_token と同じか（§3.3.3.6） |
//   **code は PKCE で引き換える**（code_verifier。秘密は送らない）。
//   **code token・code id_token token は扱わない**（access_token をフロント チャネルで返す。MPAS の雛形も閉じている）。
//
// ＜保存先＞
//   **sessionStorage**（タブを閉じれば消える。他のタブとは共有しない）。
//   state・nonce・code_verifier は**認可の往復だけに使い、使ったら消す。**
//   access_token も sessionStorage に置くので、**XSS があれば読まれる**（サンプルの簡素さを優先した。
//   実運用では、BFF やメモリでの保持を検討すること）。
// ---------------------------------------------------------------

import constants from '../const';
import common from './common';

// UserInfo の型定義
export interface UserInfo {
  sub: string;
  email?: string;
  [key: string]: unknown;
}

interface TokenResponse {
  access_token?: string;
  id_token?: string;
  error?: string;
  error_description?: string;
  [key: string]: unknown;
}

interface IdTokenClaims {
  iss?: string;
  aud?: string | string[];
  exp?: number;
  nonce?: string;
  sub?: string;
  c_hash?: string;
  [key: string]: unknown;
}

// フロー（SignIn のボタンで選ぶ）
export type Flow = 'pkce' | 'hybrid';

// ---------------------------------------------------------------
// ストレージのキー定数
// ---------------------------------------------------------------
const STORAGE_KEYS = {
  FLOW: 'flow',
  STATE: 'state',
  NONCE: 'nonce',
  CODE_VERIFIER: 'code_verifier',
  ACCESS_TOKEN: 'access_token',
  USER_INFO: 'user_info',
} as const;

// フローごとの client_id と response_type
function clientIdOf(flow: Flow): string {
  return flow === 'hybrid' ? constants.HybridClientId : constants.ClientId;
}

function responseTypeOf(flow: Flow): string {
  return flow === 'hybrid' ? 'code id_token' : 'code';
}

const storage = window.sessionStorage;

// ---------------------------------------------------------------
// URLのパラメタを抽出する。
// ---------------------------------------------------------------

// -----------------------------------------------------------
// フラグメント（# ～ の部分）を取得し、アドレス バーから消す。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  Record<string, string>
// -----------------------------------------------------------
// **code は 1 度しか使えない。履歴に残さない**（history.replaceState でフラグメントを消す）。
// -----------------------------------------------------------
export function getParameterFromFragment(): Record<string, string> {
  const temp = window.location.hash;
  window.history.replaceState(null, '', window.location.pathname + window.location.search);

  if (temp.indexOf("#") === 0) {
    // # が1文字目にある場合
    // 2文字目以降を object に parse
    return parseQueryString(temp.substring(1));
  } else {
    return {}; // 空
  }
}

// -----------------------------------------------------------
// QueryString を object に parse する。
// ---------------------------------------------------------------
// 引数    queryString
// 戻り値  Record<string, string>
// -----------------------------------------------------------
function parseQueryString(queryString: string): Record<string, string> {
  const data: Record<string, string> = {};
  new URLSearchParams(queryString).forEach((value, key) => {
    data[key] = value;
  });
  return data;
}

// ---------------------------------------------------------------
// SHA-256 ハッシュ化 (Web Crypto API 使用・非同期)
// ---------------------------------------------------------------
// 引数    input: ハッシュ化する文字列
// 戻り値  SHA-256 ハッシュの ArrayBuffer (Promise)
// ---------------------------------------------------------------
async function sha256(input: string): Promise<ArrayBuffer> {
  const encoded = new TextEncoder().encode(input);
  return await crypto.subtle.digest('SHA-256', encoded);
}

// ---------------------------------------------------------------
// 状態の初期化
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
// **state・nonce・code_verifier は、毎回作り直す**（使い回すと、確かめる意味が無くなる）。
// code_verifier は 64 文字（RFC 7636 §4.1 : 43〜128 文字）。
// ---------------------------------------------------------------
export function initSignUpStatus(): void {
  clearSignInStatus();

  storage.setItem(STORAGE_KEYS.STATE, common.getRandomString(43));
  storage.setItem(STORAGE_KEYS.NONCE, common.getRandomString(43));
  storage.setItem(STORAGE_KEYS.CODE_VERIFIER, common.getRandomString(64));
}

// ---------------------------------------------------------------
// サインインの状態を消す（サインアウト）
// ---------------------------------------------------------------
export function clearSignInStatus(): void {
  storage.removeItem(STORAGE_KEYS.FLOW);
  storage.removeItem(STORAGE_KEYS.STATE);
  storage.removeItem(STORAGE_KEYS.NONCE);
  storage.removeItem(STORAGE_KEYS.CODE_VERIFIER);
  storage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
  storage.removeItem(STORAGE_KEYS.USER_INFO);
}

// ---------------------------------------------------------------
// code_challenge の取得（S256）
// ---------------------------------------------------------------
// 引数    －
// 戻り値  code_challenge 文字列 (Promise)
// ---------------------------------------------------------------
// **plain は使わない**（S256 が使えるなら S256 を使う。RFC 7636 §4.2）。
// ---------------------------------------------------------------
export async function getCodeChallenge(): Promise<string> {
  const code_verifier = storage.getItem(STORAGE_KEYS.CODE_VERIFIER);

  if (!code_verifier) {
    throw new Error('code_verifier が見つかりません。initSignUpStatus() を先に呼び出してください。');
  }

  return common.base64URLEncode(await sha256(code_verifier));
}

// ---------------------------------------------------------------
// 認可要求の URL を作る
// ---------------------------------------------------------------
// 引数    flow（pkce / hybrid）
// 戻り値  認可要求の URL (Promise)
// ---------------------------------------------------------------
// **値はエンコードする**（scope の空白など）。
// **redirect_uri は、トークン要求と同じ値を送る**（RFC 6749 §4.1.3）。
// **hybrid も PKCE を付ける**（code を引き換えるのは、秘密を持たないこのアプリなので）。
// ---------------------------------------------------------------
export async function createAuthorizationRequestUrl(flow: Flow = 'pkce'): Promise<string> {
  initSignUpStatus();
  storage.setItem(STORAGE_KEYS.FLOW, flow);

  const params = new URLSearchParams({
    client_id: clientIdOf(flow),
    response_type: responseTypeOf(flow),
    scope: constants.Scope,
    redirect_uri: constants.RedirectUri,
    state: storage.getItem(STORAGE_KEYS.STATE) ?? '',
    nonce: storage.getItem(STORAGE_KEYS.NONCE) ?? '',
    code_challenge: await getCodeChallenge(),
    code_challenge_method: 'S256',
    response_mode: 'fragment',
  });

  return constants.AuthRequestUrl + '?' + params.toString();
}

// ---------------------------------------------------------------
// 認可応答を処理する（戻り先の画面から呼ぶ）
// ---------------------------------------------------------------
// 引数    －
// 戻り値  UserInfo (Promise)。失敗なら Error を投げる（理由をメッセージに入れる）
// ---------------------------------------------------------------
export async function handleAuthorizationResponse(): Promise<UserInfo> {
  const params = getParameterFromFragment();

  // **1 度しか使わない**ので、先に取り出して消す
  const flow: Flow = storage.getItem(STORAGE_KEYS.FLOW) === 'hybrid' ? 'hybrid' : 'pkce';
  const state = storage.getItem(STORAGE_KEYS.STATE);
  const nonce = storage.getItem(STORAGE_KEYS.NONCE);
  const code_verifier = storage.getItem(STORAGE_KEYS.CODE_VERIFIER);
  storage.removeItem(STORAGE_KEYS.FLOW);
  storage.removeItem(STORAGE_KEYS.STATE);
  storage.removeItem(STORAGE_KEYS.NONCE);
  storage.removeItem(STORAGE_KEYS.CODE_VERIFIER);
  const clientId = clientIdOf(flow);

  if (params.error) {
    throw new Error('認可が断られました : ' + params.error
      + (params.error_description ? '（' + params.error_description + '）' : ''));
  }

  if (!state || params.state !== state) {
    throw new Error('state が一致しません（自分が出した要求への応答ではない）。');
  }

  if (params.iss && params.iss !== constants.Issuer) {
    throw new Error('応答の iss（' + params.iss + '）が、期待する IdP と違います。');
  }

  if (!params.code || !code_verifier || !nonce) {
    throw new Error('認可コード、または認可要求の状態がありません。');
  }

  // **Hybrid は、認可応答の id_token を、引き換えの前に確かめる**（署名・c_hash を含む。§3.3.2.12）
  let frontChannel: IdTokenClaims | null = null;
  if (flow === 'hybrid') {
    if (!params.id_token) {
      throw new Error('Hybrid（code id_token）なのに、認可応答に id_token がありません。');
    }
    frontChannel = await verifyFrontChannelIdToken(params.id_token, params.code, nonce, clientId);
  }

  const token = await callConvertCodeToToken(params.code, code_verifier, clientId);
  if (!token.access_token || !token.id_token) {
    throw new Error('トークンの応答に access_token か id_token がありません。');
  }

  const sub = verifyIdTokenClaims(token.id_token, nonce, clientId);

  // **トークン応答の id_token は、認可応答の id_token と同じ iss・sub であること**（§3.3.3.6）
  if (frontChannel && (frontChannel.iss !== constants.Issuer || frontChannel.sub !== sub)) {
    throw new Error('トークン応答の id_token の iss・sub が、認可応答の id_token と違います。');
  }

  const userInfo = await callUserInfo(token.access_token);
  if (userInfo.sub !== sub) {
    throw new Error('UserInfo の sub が、id_token の sub と違います。');
  }

  storage.setItem(STORAGE_KEYS.ACCESS_TOKEN, token.access_token);
  storage.setItem(STORAGE_KEYS.USER_INFO, JSON.stringify(userInfo));
  return userInfo;
}

// ---------------------------------------------------------------
// /token にリクエスト
// ---------------------------------------------------------------
// **パブリック クライアントなので、秘密は送らない**（code_verifier で引き換える）。
// ---------------------------------------------------------------
async function callConvertCodeToToken(code: string, code_verifier: string, clientId: string): Promise<TokenResponse> {
  const body = new URLSearchParams({
    grant_type: 'authorization_code',
    client_id: clientId,
    code: code,
    redirect_uri: constants.RedirectUri,
    code_verifier: code_verifier,
  });

  const response = await fetch(constants.TokenRequestUrl, {
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: body,
  });

  // **エラーの応答も JSON で返る**（400 ＋ error。RFC 6749 §5.2）
  const data = await response.json() as TokenResponse;
  if (data.error) {
    throw new Error('トークン要求が断られました : ' + data.error
      + (data.error_description ? '（' + data.error_description + '）' : ''));
  }
  common.fetchStatusHandler(response);
  return data;
}

// ---------------------------------------------------------------
// id_token のクレームを確かめる
// ---------------------------------------------------------------
// 引数    id_token, 認可要求で送った nonce, 期待する aud（client_id）
// 戻り値  sub
// ---------------------------------------------------------------
// **署名は確かめない**（ファイルの冒頭の「確かめること」を参照）。iss・aud・exp・nonce を見る。
// ---------------------------------------------------------------
function verifyIdTokenClaims(id_token: string, nonce: string, clientId: string): string {
  const parts = id_token.split('.');
  if (parts.length !== 3) {
    throw new Error('id_token が JWS の形ではありません。');
  }

  return checkIdTokenClaims(
    JSON.parse(common.base64URLDecodeToString(parts[1])) as IdTokenClaims, nonce, clientId);
}

// ---------------------------------------------------------------
// id_token のクレーム（iss・aud・exp・nonce・sub）を確かめる
// ---------------------------------------------------------------
function checkIdTokenClaims(claims: IdTokenClaims, nonce: string, clientId: string): string {
  const auds = Array.isArray(claims.aud) ? claims.aud : [claims.aud];

  if (claims.iss !== constants.Issuer) {
    throw new Error('id_token の iss（' + claims.iss + '）が、期待する IdP と違います。');
  }
  if (!auds.includes(clientId)) {
    throw new Error('id_token の aud に、このクライアントが含まれていません。');
  }
  if (typeof claims.exp !== 'number' || claims.exp * 1000 < Date.now()) {
    throw new Error('id_token の期限（exp）が切れています。');
  }
  if (claims.nonce !== nonce) {
    throw new Error('id_token の nonce が、認可要求で送った値と違います。');
  }
  if (!claims.sub) {
    throw new Error('id_token に sub がありません。');
  }

  return claims.sub;
}

// ---------------------------------------------------------------
// 認可応答（フロント チャネル）の id_token を確かめる（Hybrid。#668）
// ---------------------------------------------------------------
// 引数    id_token, 認可応答の code, 認可要求で送った nonce, 期待する aud（client_id）
// 戻り値  クレーム（Promise）
// ---------------------------------------------------------------
// **署名・iss・aud・exp・nonce・c_hash を確かめる**（OpenID Connect Core 1.0 §3.3.2.12 → §3.2.2.11、§3.3.2.11）。
// **c_hash は必須**（code id_token）。無い・合わないなら、届いた code は、この id_token と結び付いていない。
// ---------------------------------------------------------------
async function verifyFrontChannelIdToken(
  id_token: string, code: string, nonce: string, clientId: string): Promise<IdTokenClaims> {

  const parts = id_token.split('.');
  if (parts.length !== 3) {
    throw new Error('認可応答の id_token が JWS の形ではありません。');
  }

  const header = JSON.parse(common.base64URLDecodeToString(parts[0])) as { alg?: string; kid?: string };
  const alg = JWS_ALGS[header.alg ?? ''];
  if (!alg) {
    // **none・HS*（共有鍵）は受けない**（署名を確かめる意味が無くなる）
    throw new Error('認可応答の id_token の alg（' + header.alg + '）は受けません。');
  }
  if (!header.kid) {
    throw new Error('認可応答の id_token に kid がありません（鍵を引けない）。');
  }

  // **鍵は jwks_uri から kid で引く**（Discovery の issuer を照合してから）
  const jwk = await getJwk(header.kid, alg.kty);
  const key = await crypto.subtle.importKey('jwk', jwk, alg.importParams, false, ['verify']);
  const ok = await crypto.subtle.verify(
    alg.verifyParams, key,
    common.base64URLDecodeToBytes(parts[2]),
    new TextEncoder().encode(parts[0] + '.' + parts[1]));
  if (!ok) {
    throw new Error('認可応答の id_token の署名を確かめられません。');
  }

  const claims = JSON.parse(common.base64URLDecodeToString(parts[1])) as IdTokenClaims;
  checkIdTokenClaims(claims, nonce, clientId);

  // **c_hash = code のハッシュ（alg の SHA）の左半分を base64url にしたもの**（§3.3.2.11）
  const digest = new Uint8Array(await crypto.subtle.digest(alg.hash, new TextEncoder().encode(code)));
  const c_hash = common.base64URLEncode(digest.slice(0, digest.length / 2));
  if (!claims.c_hash || claims.c_hash !== c_hash) {
    throw new Error('認可応答の id_token の c_hash が、届いた code と合いません（または c_hash がありません）。');
  }

  return claims;
}

// 受ける alg（非対称だけ）と、Web Crypto のパラメタ
const JWS_ALGS: Record<string, {
  kty: string; hash: string;
  importParams: RsaHashedImportParams | EcKeyImportParams;
  verifyParams: AlgorithmIdentifier | EcdsaParams;
}> = {
  RS256: { kty: 'RSA', hash: 'SHA-256', importParams: { name: 'RSASSA-PKCS1-v1_5', hash: 'SHA-256' }, verifyParams: 'RSASSA-PKCS1-v1_5' },
  ES256: { kty: 'EC', hash: 'SHA-256', importParams: { name: 'ECDSA', namedCurve: 'P-256' }, verifyParams: { name: 'ECDSA', hash: 'SHA-256' } },
  ES384: { kty: 'EC', hash: 'SHA-384', importParams: { name: 'ECDSA', namedCurve: 'P-384' }, verifyParams: { name: 'ECDSA', hash: 'SHA-384' } },
  ES512: { kty: 'EC', hash: 'SHA-512', importParams: { name: 'ECDSA', namedCurve: 'P-521' }, verifyParams: { name: 'ECDSA', hash: 'SHA-512' } },
};

// ---------------------------------------------------------------
// jwks_uri から、kid の鍵を引く
// ---------------------------------------------------------------
// **Discovery の issuer を照合する**（OpenID Connect Discovery 1.0 §4.3。別の OP の鍵を使わないため）。
// **鍵の種類（kty）も確かめる**（alg と鍵が合わなければ使わない）。
// 公開鍵の成分だけを渡す（alg・use などの付帯情報は、importKey の照合で弾かれることがあるため）。
// ---------------------------------------------------------------
async function getJwk(kid: string, kty: string): Promise<JsonWebKey> {
  const discovery = await (await fetch(constants.DiscoveryUrl)).json() as { issuer?: string; jwks_uri?: string };
  if (discovery.issuer !== constants.Issuer || !discovery.jwks_uri) {
    throw new Error('Discovery の issuer（' + discovery.issuer + '）が、期待する IdP と違います。');
  }

  const jwks = await (await fetch(discovery.jwks_uri)).json() as { keys?: (JsonWebKey & { kid?: string })[] };
  const found = (jwks.keys ?? []).find((k) => k.kid === kid);
  if (!found || found.kty !== kty) {
    throw new Error('id_token の kid（' + kid + '）の鍵を、jwks_uri から引けません。');
  }

  return kty === 'RSA'
    ? { kty: found.kty, n: found.n, e: found.e }
    : { kty: found.kty, crv: found.crv, x: found.x, y: found.y };
}

// ---------------------------------------------------------------
// /userinfo にリクエスト
// ---------------------------------------------------------------
async function callUserInfo(access_token: string): Promise<UserInfo> {
  const response = await fetch(constants.UserInfoRequestUrl, {
    method: 'GET',
    headers: {
      Authorization: 'Bearer ' + access_token,
      Accept: 'application/json',
    },
  });
  common.fetchStatusHandler(response);
  return await response.json() as UserInfo;
}

// ---------------------------------------------------------------
// HTTPリクエストヘッダの作成（リソース サーバへの要求に使う）
// ---------------------------------------------------------------
export function createHttpRequestHeader(isJsonRpc: boolean): HeadersInit {
  const headers: Record<string, string> = {
    'Accept': 'application/json',
    'Content-Type': isJsonRpc ? 'application/json' : 'application/x-www-form-urlencoded',
  };

  const access_token = getAccessToken();
  if (access_token) {
    headers['Authorization'] = 'Bearer ' + access_token;
  }

  return headers;
}

// ---------------------------------------------------------------
// access_token の取得
// ---------------------------------------------------------------
export function getAccessToken(): string | null {
  return storage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
}

// ---------------------------------------------------------------
// user_info の取得
// ---------------------------------------------------------------
export function getUserInfo(): UserInfo | null {
  const json = storage.getItem(STORAGE_KEYS.USER_INFO);
  return json ? JSON.parse(json) as UserInfo : null;
}

// 全関数をオブジェクトとしてデフォルトエクスポート
const oauth_oidc = {
  getParameterFromFragment,
  initSignUpStatus,
  clearSignInStatus,
  getCodeChallenge,
  createAuthorizationRequestUrl,
  handleAuthorizationResponse,
  createHttpRequestHeader,
  getAccessToken,
  getUserInfo,
};

export default oauth_oidc;
