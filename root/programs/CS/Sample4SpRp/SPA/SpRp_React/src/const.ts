// ---------------------------------------------------------------
// 接続先とクライアント（#649）
//   IdP は MPAS のスタブ、クライアントは MPAS の雛形の SPA_Application（Sample4SpRp/README.md の 3 節）。
//   **戻り先（RedirectUri）は、雛形の redirect_uri_code と 1 文字も違ってはならない。**
//   Vite の既定のポート（5173）で動かす。
// ---------------------------------------------------------------

export const FrontendHostRootUrl = 'http://localhost:5173';
export const AuthServerRootUrl = 'https://localhost:44310';

// リソース サーバ（Samples4NetCore/Backend/ASPNETWebService）。アクセス トークンを Bearer で送る
export const ResourcesServerRootUrl = 'https://localhost:44335';

const constants = Object.freeze({
    ClientId: 'f374a155909d486a9234693c34e94479',
    Scope: 'openid profile email',
    FrontendHostRootUrl,
    RedirectUri: `${FrontendHostRootUrl}/RedirectEndpoint`,
    Issuer: AuthServerRootUrl,
    AuthRequestUrl: `${AuthServerRootUrl}/authorize`,
    TokenRequestUrl: `${AuthServerRootUrl}/token`,
    UserInfoRequestUrl: `${AuthServerRootUrl}/userinfo`,
    ResourcesServerRootUrl,
    ClaimsUrl: `${ResourcesServerRootUrl}/api/json/claims`,
} as const);

export default constants;
