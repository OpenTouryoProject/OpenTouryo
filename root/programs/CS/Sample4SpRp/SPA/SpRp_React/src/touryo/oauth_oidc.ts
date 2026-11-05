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
  [key: string]: unknown;
}

// ---------------------------------------------------------------
// ストレージのキー定数
// ---------------------------------------------------------------
const STORAGE_KEYS = {
  STATE: 'state',
  NONCE: 'nonce',
  CODE_VERIFIER: 'code_verifier',
  ACCESS_TOKEN: 'access_token',
  USER_INFO: 'user_info',
} as const;

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
// 引数    －
// 戻り値  認可要求の URL (Promise)
// ---------------------------------------------------------------
// **値はエンコードする**（scope の空白など）。
// **redirect_uri は、トークン要求と同じ値を送る**（RFC 6749 §4.1.3）。
// ---------------------------------------------------------------
export async function createAuthorizationRequestUrl(): Promise<string> {
  initSignUpStatus();

  const params = new URLSearchParams({
    client_id: constants.ClientId,
    response_type: 'code',
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
  const state = storage.getItem(STORAGE_KEYS.STATE);
  const nonce = storage.getItem(STORAGE_KEYS.NONCE);
  const code_verifier = storage.getItem(STORAGE_KEYS.CODE_VERIFIER);
  storage.removeItem(STORAGE_KEYS.STATE);
  storage.removeItem(STORAGE_KEYS.NONCE);
  storage.removeItem(STORAGE_KEYS.CODE_VERIFIER);

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

  const token = await callConvertCodeToToken(params.code, code_verifier);
  if (!token.access_token || !token.id_token) {
    throw new Error('トークンの応答に access_token か id_token がありません。');
  }

  const sub = verifyIdTokenClaims(token.id_token, nonce);

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
async function callConvertCodeToToken(code: string, code_verifier: string): Promise<TokenResponse> {
  const body = new URLSearchParams({
    grant_type: 'authorization_code',
    client_id: constants.ClientId,
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
// 引数    id_token, 認可要求で送った nonce
// 戻り値  sub
// ---------------------------------------------------------------
// **署名は確かめない**（ファイルの冒頭の「確かめること」を参照）。iss・aud・exp・nonce を見る。
// ---------------------------------------------------------------
function verifyIdTokenClaims(id_token: string, nonce: string): string {
  const parts = id_token.split('.');
  if (parts.length !== 3) {
    throw new Error('id_token が JWS の形ではありません。');
  }

  const claims = JSON.parse(common.base64URLDecodeToString(parts[1])) as IdTokenClaims;
  const auds = Array.isArray(claims.aud) ? claims.aud : [claims.aud];

  if (claims.iss !== constants.Issuer) {
    throw new Error('id_token の iss（' + claims.iss + '）が、期待する IdP と違います。');
  }
  if (!auds.includes(constants.ClientId)) {
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
