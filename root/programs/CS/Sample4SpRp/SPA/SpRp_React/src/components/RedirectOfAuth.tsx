// ---------------------------------------------------------------
// 認可応答の戻り先（/RedirectEndpoint。#649）
//   応答（フラグメント）を処理し、成功したらトップへ戻る。失敗したら理由を出す。
// ---------------------------------------------------------------

import { useEffect, useState } from 'react';
import constants from '../const';
import oauth_oidc from '../touryo/oauth_oidc';

// **応答は 1 度だけ処理する**（StrictMode の開発時は effect が 2 回走る。2 回目は state が消えていて失敗する）
let handling: Promise<unknown> | null = null;

export default function RedirectOfAuth() {
  const [message, setMessage] = useState('...');

  useEffect(() => {
    if (!handling) {
      handling = oauth_oidc.handleAuthorizationResponse();
    }
    handling
      .then(() => { window.location.replace(constants.FrontendHostRootUrl); })
      .catch((e: Error) => { setMessage(e.message); });
  }, []);

  return (
    <div>
      <p>{message}</p>
      <a href={constants.FrontendHostRootUrl}>Top</a>
    </div>
  );
}
