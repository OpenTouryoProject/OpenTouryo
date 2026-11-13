// ---------------------------------------------------------------
// 接続先とクライアント（#649）
//   IdP は MPAS のスタブ、クライアントは MPAS の雛形の SPA_Application（Sample4SpRp/README.md の 3 節）。
//   **戻り先（RedirectUri）は、雛形の redirect_uri_code と 1 文字も違ってはならない。**
//   Vite の既定のポート（5173）で動かす。
//
//   **Hybrid（code id_token）は、別のクライアント SPA_Hybrid で送る**（#668）。
//   MPAS は Hybrid の戻り先を redirect_uri_token から引き、雛形の SPA_Application はそこが空のため。
//   登録は Sample4SpRp/container/Stub/Sample4SpRp.stub.json（StubCompose.ps1 が既定で渡す）。
// ---------------------------------------------------------------

export const FrontendHostRootUrl = 'http://localhost:5173';
export const AuthServerRootUrl = 'https://localhost:44310';

// リソース サーバ（Samples4NetCore/Backend/ASPNETWebService）。アクセス トークンを Bearer で送る
export const ResourcesServerRootUrl = 'https://localhost:44335';

const constants = Object.freeze({
    ClientId: 'f374a155909d486a9234693c34e94479',
    HybridClientId: '2ee77c67c8624f39a423c9b6c319c106',
    Scope: 'openid profile email',
    FrontendHostRootUrl,
    RedirectUri: `${FrontendHostRootUrl}/RedirectEndpoint`,
    Issuer: AuthServerRootUrl,
    AuthRequestUrl: `${AuthServerRootUrl}/authorize`,
    TokenRequestUrl: `${AuthServerRootUrl}/token`,
    UserInfoRequestUrl: `${AuthServerRootUrl}/userinfo`,
    // Hybrid の id_token（フロント チャネル）の署名を確かめる鍵は、Discovery の jwks_uri から引く
    DiscoveryUrl: `${AuthServerRootUrl}/.well-known/openid-configuration`,
    ResourcesServerRootUrl,
    ClaimsUrl: `${ResourcesServerRootUrl}/api/json/claims`,
} as const);

export default constants;
