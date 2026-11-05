// ---------------------------------------------------------------
// リソース サーバを Bearer で呼ぶ（#649）
//   Samples4NetCore/Backend/ASPNETWebService の GET api/json/claims。
//   **サインインしていれば Authorization: Bearer を付ける**（createHttpRequestHeader）。
//   リソース サーバは、検証に通れば userName に sub を、通らなければ「未認証」を返す。
// ---------------------------------------------------------------

import { useState } from 'react';
import constants from '../const';
import common from '../touryo/common';
import oauth_oidc from '../touryo/oauth_oidc';

export default function CallApi() {
  const [result, setResult] = useState('');

  const callApi = async () => {
    setResult('...');
    try {
      const response = await fetch(constants.ClaimsUrl, {
        method: 'GET',
        headers: oauth_oidc.createHttpRequestHeader(false),
      });
      common.fetchStatusHandler(response);
      setResult(JSON.stringify(await response.json(), null, 2));
    } catch (e) {
      // CORS で止まったときも、ここに来る（理由は DevTools の Console に出る）
      setResult((e as Error).message);
    }
  };

  return (
    <div>
      <button type="button" onClick={callApi}>
        Call API（{oauth_oidc.getAccessToken() ? 'Bearer' : 'トークン無し'}）
      </button>
      {result && <pre style={{ textAlign: 'left' }}>{result}</pre>}
    </div>
  );
}
