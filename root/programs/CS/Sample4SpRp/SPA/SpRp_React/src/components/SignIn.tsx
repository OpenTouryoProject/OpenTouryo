// ---------------------------------------------------------------
// サインイン・サインアウト（#649）
//   サインインは、認可要求の URL へ移るだけ（応答は RedirectOfAuth が受ける）。
//   **表示名は UserInfo の email**（識別子は sub。WebApp のサンプルと同じ）。
//   **Hybrid（code id_token）のボタンを足した**（#668。クライアントは SPA_Hybrid）。**新しく使わない**（PKCE ＋ openid で代替できる）。
// ---------------------------------------------------------------

import { useState } from 'react';
import oauth_oidc, { type Flow } from '../touryo/oauth_oidc';

export default function SignIn() {
  const [userInfo, setUserInfo] = useState(oauth_oidc.getUserInfo());
  const [message, setMessage] = useState('');

  // flow : pkce（認可コード ＋ PKCE）/ hybrid（code id_token ＋ PKCE。#668）
  const signIn = async (flow: Flow) => {
    try {
      window.location.href = await oauth_oidc.createAuthorizationRequestUrl(flow);
    } catch (e) {
      setMessage((e as Error).message);
    }
  };

  const signOut = () => {
    oauth_oidc.clearSignInStatus();
    setUserInfo(null);
  };

  return (
    <div>
      {userInfo
        ? <button type="button" onClick={signOut}>Sign Out（{userInfo.email ?? userInfo.sub}）</button>
        : <>
            <button type="button" onClick={() => signIn('pkce')}>Sign In</button>{' '}
            <button type="button" onClick={() => signIn('hybrid')}>Sign In（Hybrid : code id_token。非推奨）</button>
          </>}
      {message && <p>{message}</p>}
    </div>
  );
}
