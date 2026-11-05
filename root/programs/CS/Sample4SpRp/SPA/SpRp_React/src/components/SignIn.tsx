// ---------------------------------------------------------------
// サインイン・サインアウト（#649）
//   サインインは、認可要求の URL へ移るだけ（応答は RedirectOfAuth が受ける）。
//   **表示名は UserInfo の email**（識別子は sub。WebApp のサンプルと同じ）。
// ---------------------------------------------------------------

import { useState } from 'react';
import oauth_oidc from '../touryo/oauth_oidc';

export default function SignIn() {
  const [userInfo, setUserInfo] = useState(oauth_oidc.getUserInfo());
  const [message, setMessage] = useState('');

  const signIn = async () => {
    try {
      window.location.href = await oauth_oidc.createAuthorizationRequestUrl();
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
        : <button type="button" onClick={signIn}>Sign In</button>}
      {message && <p>{message}</p>}
    </div>
  );
}
