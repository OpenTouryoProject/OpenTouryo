namespace SpRp_Blazor;

/// <summary>接続先とクライアント（#649）</summary>
/// <remarks>
/// IdP は MPAS のスタブ、クライアントは MPAS の雛形の SPA_Application（Sample4SpRp/README.md の 3 節）。
/// **戻り先（RedirectUri）は、雛形の redirect_uri_code と 1 文字も違ってはならない。**
/// React 版と同じ戻り先なので、**5173 で動かす**（Properties/launchSettings.json）。同時には動かさない。
/// </remarks>
public static class Constants
{
    /// <summary>このアプリのルート</summary>
    public const string FrontendHostRootUrl = "http://localhost:5173";

    /// <summary>IdP（MPAS のスタブ）のルート</summary>
    public const string AuthServerRootUrl = "https://localhost:44310";

    /// <summary>client_id（雛形の SPA_Application）</summary>
    public const string ClientId = "f374a155909d486a9234693c34e94479";

    /// <summary>client_id（Hybrid : code id_token。#668）</summary>
    /// <remarks>
    /// **別のクライアント SPA_Hybrid**（秘密なし）。MPAS は Hybrid の戻り先を redirect_uri_token から引き、
    /// 雛形の SPA_Application はそこが空のため。登録は Sample4SpRp/container/Stub/Sample4SpRp.stub.json（StubCompose.ps1 が既定で渡す）。
    /// </remarks>
    public const string HybridClientId = "2ee77c67c8624f39a423c9b6c319c106";

    /// <summary>求める scope</summary>
    public const string Scope = "openid profile email";

    /// <summary>戻り先</summary>
    public const string RedirectUri = FrontendHostRootUrl + "/RedirectEndpoint";

    /// <summary>期待する issuer</summary>
    public const string Issuer = AuthServerRootUrl;

    /// <summary>認可エンドポイント</summary>
    public const string AuthRequestUrl = AuthServerRootUrl + "/authorize";

    /// <summary>トークン エンドポイント</summary>
    public const string TokenRequestUrl = AuthServerRootUrl + "/token";

    /// <summary>UserInfo エンドポイント</summary>
    public const string UserInfoRequestUrl = AuthServerRootUrl + "/userinfo";

    /// <summary>Discovery（Hybrid の id_token の署名を確かめる鍵は、ここの jwks_uri から引く。#668）</summary>
    public const string DiscoveryUrl = AuthServerRootUrl + "/.well-known/openid-configuration";

    /// <summary>リソース サーバ（Samples4NetCore/Backend/ASPNETWebService）。アクセス トークンを Bearer で送る</summary>
    public const string ResourcesServerRootUrl = "https://localhost:44335";

    /// <summary>Bearer の検証の結果（Claims）を返す口</summary>
    public const string ClaimsUrl = ResourcesServerRootUrl + "/api/json/claims";
}
