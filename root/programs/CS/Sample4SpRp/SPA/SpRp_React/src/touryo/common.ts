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
// 取り込み元 : FrontendTemplates（UI/SPA/React/vite-react-ts/src/touryo/common.ts、76bd771）
//   このサンプル（Sample4SpRp、#649）用に手を入れてある。
//   **乱数は Web Crypto API（crypto.getRandomValues）から取る**（state・nonce・code_verifier に使うため。RFC 7636 §7.1）
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// fetch のレスポンスのステータスコードをチェック
// ---------------------------------------------------------------
export function fetchStatusHandler(response: Response): Response {
  if (response.status === 200) {
    return response;
  } else {
    throw new Error(response.status + " " + response.statusText);
  }
}

// ---------------------------------------------------------------
// ランダム文字列を取得する。
// ---------------------------------------------------------------
// 引数    l（生成する文字列の長さ）
// 戻り値  ランダム文字列（RFC 7636 §4.1 の unreserved の部分集合 [a-z0-9]）
// ---------------------------------------------------------------
// **暗号論的な乱数（crypto.getRandomValues）を使う。**
// 偏りを出さないため、文字数（36）の倍数に収まらない値は捨てて引き直す。
// ---------------------------------------------------------------
export function getRandomString(l: number): string {
  const c = "abcdefghijklmnopqrstuvwxyz0123456789";
  const cl = c.length;
  const limit = 256 - (256 % cl); // 252（これ以上の値は捨てる）
  let r = "";
  const buf = new Uint8Array(1);
  while (r.length < l) {
    crypto.getRandomValues(buf);
    if (buf[0] < limit) {
      r += c[buf[0] % cl];
    }
  }
  return r;
}

// ---------------------------------------------------------------
// Base64URL エンコーディングする。
// ---------------------------------------------------------------
// 引数    buffer（ArrayBuffer または Uint8Array）
// 戻り値  Base64URL エンコーディングした値
// ---------------------------------------------------------------
export function base64URLEncode(buffer: ArrayBuffer | Uint8Array): string {
  const bytes = buffer instanceof Uint8Array ? buffer : new Uint8Array(buffer);
  // btoa はブラウザ標準の Base64 エンコード関数
  const base64 = btoa(String.fromCharCode(...bytes));
  return base64
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=/g, '');
}

// ---------------------------------------------------------------
// Base64URL をデコードし、UTF-8 の文字列にする。
// ---------------------------------------------------------------
// 引数    value（Base64URL エンコーディングした値）
// 戻り値  文字列（JWT のヘッダ・ペイロードを読むのに使う）
// ---------------------------------------------------------------
export function base64URLDecodeToString(value: string): string {
  const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
  const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4);
  const binary = atob(padded);
  const bytes = Uint8Array.from(binary, (ch) => ch.charCodeAt(0));
  return new TextDecoder().decode(bytes);
}

// ---------------------------------------------------------------
// Base64URL をデコードし、バイト列にする（#668）。
// ---------------------------------------------------------------
// 引数    value（Base64URL エンコーディングした値）
// 戻り値  Uint8Array（JWS の署名を確かめるのに使う）
// ---------------------------------------------------------------
export function base64URLDecodeToBytes(value: string): Uint8Array<ArrayBuffer> {
  const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
  const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4);
  const binary = atob(padded);
  return Uint8Array.from(binary, (ch) => ch.charCodeAt(0));
}

// 全関数をオブジェクトとしてデフォルトエクスポート
const common = {
  fetchStatusHandler,
  getRandomString,
  base64URLEncode,
  base64URLDecodeToString,
  base64URLDecodeToBytes,
};

export default common;
