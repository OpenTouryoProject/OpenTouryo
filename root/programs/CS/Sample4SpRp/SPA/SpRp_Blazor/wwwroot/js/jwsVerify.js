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
// JWS の署名を、Web Crypto API で確かめる（#668。Touryo/OAuthOidc.cs が JS 相互運用で呼ぶ）
//
//   **Blazor WebAssembly では、.NET の RSA・ECDSA を使えない**（ブラウザでは PlatformNotSupportedException）。
//   そのため、署名の検証だけを Web Crypto に任せる。**鍵の選び方とクレームの確認は C# 側で行う。**
//   React 版（SpRp_React/src/touryo/oauth_oidc.ts の verifyFrontChannelIdToken）と同じ組み合わせである。
// ---------------------------------------------------------------

window.sprpJws = (() => {
  // 受ける alg（非対称だけ。none・HS* は受けない）
  const ALGS = {
    RS256: { importParams: { name: 'RSASSA-PKCS1-v1_5', hash: 'SHA-256' }, verifyParams: 'RSASSA-PKCS1-v1_5' },
    ES256: { importParams: { name: 'ECDSA', namedCurve: 'P-256' }, verifyParams: { name: 'ECDSA', hash: 'SHA-256' } },
    ES384: { importParams: { name: 'ECDSA', namedCurve: 'P-384' }, verifyParams: { name: 'ECDSA', hash: 'SHA-384' } },
    ES512: { importParams: { name: 'ECDSA', namedCurve: 'P-521' }, verifyParams: { name: 'ECDSA', hash: 'SHA-512' } },
  };

  function base64UrlToBytes(value) {
    const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4);
    return Uint8Array.from(atob(padded), (ch) => ch.charCodeAt(0));
  }

  // jws : "ヘッダ.ペイロード.署名"、jwkJson : 公開鍵の成分だけの JWK（JSON 文字列）
  // 戻り値 : 確かめられたら true。alg が受けないもの・形が違うものも false
  async function verify(alg, jwkJson, jws) {
    const a = ALGS[alg];
    const parts = (jws || '').split('.');
    if (!a || parts.length !== 3) {
      return false;
    }

    try {
      const key = await crypto.subtle.importKey('jwk', JSON.parse(jwkJson), a.importParams, false, ['verify']);
      return await crypto.subtle.verify(
        a.verifyParams, key,
        base64UrlToBytes(parts[2]),
        new TextEncoder().encode(parts[0] + '.' + parts[1]));
    } catch {
      // 鍵が読めない・署名の長さが違う、など
      return false;
    }
  }

  return { verify };
})();
