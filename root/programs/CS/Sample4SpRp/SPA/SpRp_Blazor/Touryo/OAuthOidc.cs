//**********************************************************************************
//* クラス名        ：OAuthOidc
//* クラス日本語名  ：SPA 用の PKCE プロファイルで、サインインする（#649）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/05  玄人 幸道         新規作成（#649）。React 版（src/touryo/oauth_oidc.ts）を C# に移した
//*  2026/11/05  玄人 幸道         リソース サーバへの要求（Bearer）を作る CreateHttpRequestAsync を追加（#649）
//*  2026/11/05  玄人 幸道         id_token の exp を、小数の NumericDate でも読むようにした（#647 のレビューの指摘）
//*  2026/11/07  玄人 幸道         Hybrid（code id_token）を足した（#668。汎用認証サイトの自己テストから移した）
//**********************************************************************************

using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.JSInterop;

namespace SpRp_Blazor.Touryo;

/// <summary>SPA 用の PKCE プロファイルで、サインインする</summary>
/// <remarks>
/// **React 版（SpRp_React/src/touryo/oauth_oidc.ts）と同じ手順である。** 取り込み元は FrontendTemplates の
/// UI/SPA/React/vite-react-ts/src/touryo（76bd771）。
///
/// ＜フロー＞ 認可コード ＋ PKCE（S256。RFC 7636）、応答はフラグメント（response_mode=fragment）。
/// **秘密を持たない（パブリック クライアント）。** トークン要求は code_verifier で引き換える。
///
/// ＜確かめること＞
/// | 値 | 何のため |
/// |---|---|
/// | state | 自分が出した要求への応答か（CSRF。RFC 6749 §10.12） |
/// | 応答の iss（在れば） | どの IdP の応答か（Mix-Up。RFC 9207） |
/// | id_token の iss・aud・exp・nonce | OpenID Connect Core 1.0 §3.1.3.7 |
/// | UserInfo の sub | id_token の sub と同じか（OpenID Connect Core 1.0 §5.3.2） |
///
/// **id_token の署名は確かめない。** トークン エンドポイントから TLS で直接受けた id_token は、
/// 署名の代わりに TLS のサーバ検証で発行者を確かめてよい（OpenID Connect Core 1.0 §3.1.3.7 の 6）。
///
/// ＜Hybrid（code id_token。#668）＞ React 版と同じ。**クライアントは SPA_Hybrid**（Constants.HybridClientId。秘密なし）。
/// **認可応答（フラグメント）で届く id_token は、署名を確かめる**（§3.3.2.12 → §3.2.2.11）。
/// | 値 | 何のため |
/// |---|---|
/// | 認可応答の id_token の署名 | jwks_uri の鍵（kid で引く）。alg は RS256・ES256・ES384・ES512 だけ。**検証は Web Crypto**（wwwroot/js/jwsVerify.js。ブラウザでは .NET の RSA・ECDSA を使えない） |
/// | 認可応答の id_token の iss・aud・exp・nonce | 上と同じ |
/// | 認可応答の id_token の c_hash | **必須**（§3.3.2.11）。届いた code と結び付いているか |
/// | トークン応答の id_token の iss・sub | 認可応答の id_token と同じか（§3.3.3.6） |
/// **code は PKCE で引き換える。** **code token・code id_token token は扱わない**（access_token をフロント チャネルで返す）。
///
/// ＜保存先＞ **sessionStorage**（JS 相互運用）。state・nonce・code_verifier は認可の往復だけに使い、使ったら消す。
/// access_token も sessionStorage に置くので、**XSS があれば読まれる**（サンプルの簡素さを優先した）。
/// </remarks>
public class OAuthOidc
{
    #region ストレージのキー

    private const string KeyFlow = "flow";
    private const string KeyState = "state";
    private const string KeyNonce = "nonce";
    private const string KeyCodeVerifier = "code_verifier";
    private const string KeyAccessToken = "access_token";
    private const string KeyUserInfo = "user_info";

    /// <summary>乱数の文字（RFC 7636 §4.1 の unreserved の部分集合）</summary>
    private const string RandomChars = "abcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>flow : 認可コード ＋ PKCE</summary>
    public const string FlowPkce = "pkce";

    /// <summary>flow : Hybrid（code id_token ＋ PKCE。#668）</summary>
    public const string FlowHybrid = "hybrid";

    #endregion

    private readonly IJSRuntime _js;
    private readonly HttpClient _http;

    /// <summary>constructor</summary>
    /// <param name="js">IJSRuntime（sessionStorage・history）</param>
    /// <param name="http">HttpClient（IdP への要求。URL は絶対で渡す）</param>
    public OAuthOidc(IJSRuntime js, HttpClient http)
    {
        _js = js;
        _http = http;
    }

    #region 認可要求

    /// <summary>認可要求の URL を作る</summary>
    /// <param name="flow">FlowPkce / FlowHybrid</param>
    /// <returns>認可要求の URL</returns>
    /// <remarks>
    /// **state・nonce・code_verifier は、毎回作り直す**（使い回すと、確かめる意味が無くなる）。
    /// **乱数は暗号論的なもの**（RandomNumberGenerator。RFC 7636 §7.1）。code_verifier は 64 文字（§4.1 : 43〜128）。
    /// **値はエンコードする。redirect_uri は、トークン要求と同じ値を送る**（RFC 6749 §4.1.3）。
    /// **hybrid も PKCE を付ける**（code を引き換えるのは、秘密を持たないこのアプリなので）。
    /// </remarks>
    public async Task<string> CreateAuthorizationRequestUrlAsync(string flow = FlowPkce)
    {
        await ClearSignInStatusAsync();

        string state = RandomNumberGenerator.GetString(RandomChars, 43);
        string nonce = RandomNumberGenerator.GetString(RandomChars, 43);
        string codeVerifier = RandomNumberGenerator.GetString(RandomChars, 64);

        await SetItemAsync(KeyFlow, flow == FlowHybrid ? FlowHybrid : FlowPkce);
        await SetItemAsync(KeyState, state);
        await SetItemAsync(KeyNonce, nonce);
        await SetItemAsync(KeyCodeVerifier, codeVerifier);

        // **plain は使わない**（S256 が使えるなら S256 を使う。RFC 7636 §4.2）
        string codeChallenge = Base64Url.EncodeToString(SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier)));

        Dictionary<string, string> query = new()
        {
            ["client_id"] = ClientIdOf(flow),
            ["response_type"] = flow == FlowHybrid ? "code id_token" : "code",
            ["scope"] = Constants.Scope,
            ["redirect_uri"] = Constants.RedirectUri,
            ["state"] = state,
            ["nonce"] = nonce,
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = "S256",
            ["response_mode"] = "fragment",
        };

        return Constants.AuthRequestUrl + "?"
            + string.Join("&", query.Select(kv => kv.Key + "=" + Uri.EscapeDataString(kv.Value)));
    }

    #endregion

    #region 認可応答

    /// <summary>認可応答を処理する（戻り先の画面から呼ぶ）</summary>
    /// <param name="currentUri">いまの URL（NavigationManager.Uri。フラグメントを含む）</param>
    /// <returns>UserInfo の JSON</returns>
    /// <exception cref="InvalidOperationException">失敗（理由をメッセージに入れる）</exception>
    public async Task<JsonElement> HandleAuthorizationResponseAsync(string currentUri)
    {
        Dictionary<string, string> p = ParseFragment(new Uri(currentUri).Fragment);

        // **code は 1 度しか使えない。履歴に残さない**（フラグメントを消す）
        await _js.InvokeVoidAsync("history.replaceState", null, "", new Uri(currentUri).AbsolutePath);

        // **1 度しか使わない**ので、先に取り出して消す
        string flow = await GetItemAsync(KeyFlow) == FlowHybrid ? FlowHybrid : FlowPkce;
        string? state = await GetItemAsync(KeyState);
        string? nonce = await GetItemAsync(KeyNonce);
        string? codeVerifier = await GetItemAsync(KeyCodeVerifier);
        await RemoveItemAsync(KeyFlow);
        await RemoveItemAsync(KeyState);
        await RemoveItemAsync(KeyNonce);
        await RemoveItemAsync(KeyCodeVerifier);
        string clientId = ClientIdOf(flow);

        if (p.TryGetValue("error", out string? error))
        {
            throw new InvalidOperationException("認可が断られました : " + error + Describe(p.GetValueOrDefault("error_description")));
        }

        if (string.IsNullOrEmpty(state) || p.GetValueOrDefault("state") != state)
        {
            throw new InvalidOperationException("state が一致しません（自分が出した要求への応答ではない）。");
        }

        if (p.TryGetValue("iss", out string? iss) && iss != Constants.Issuer)
        {
            throw new InvalidOperationException("応答の iss（" + iss + "）が、期待する IdP と違います。");
        }

        if (!p.TryGetValue("code", out string? code) || string.IsNullOrEmpty(codeVerifier) || string.IsNullOrEmpty(nonce))
        {
            throw new InvalidOperationException("認可コード、または認可要求の状態がありません。");
        }

        // **Hybrid は、認可応答の id_token を、引き換えの前に確かめる**（署名・c_hash を含む。§3.3.2.12）
        string? frontSub = null;
        if (flow == FlowHybrid)
        {
            if (!p.TryGetValue("id_token", out string? frontIdToken) || string.IsNullOrEmpty(frontIdToken))
            {
                throw new InvalidOperationException("Hybrid（code id_token）なのに、認可応答に id_token がありません。");
            }
            frontSub = await VerifyFrontChannelIdTokenAsync(frontIdToken, code, nonce, clientId);
        }

        JsonElement token = await CallConvertCodeToTokenAsync(code, codeVerifier, clientId);
        string? accessToken = GetString(token, "access_token");
        string? idToken = GetString(token, "id_token");
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(idToken))
        {
            throw new InvalidOperationException("トークンの応答に access_token か id_token がありません。");
        }

        string sub = VerifyIdTokenClaims(idToken, nonce, clientId);

        // **トークン応答の id_token は、認可応答の id_token と同じ iss・sub であること**（§3.3.3.6。iss は両方とも Constants.Issuer と照合済み）
        if (frontSub != null && frontSub != sub)
        {
            throw new InvalidOperationException("トークン応答の id_token の sub が、認可応答の id_token と違います。");
        }

        JsonElement userInfo = await CallUserInfoAsync(accessToken);
        if (GetString(userInfo, "sub") != sub)
        {
            throw new InvalidOperationException("UserInfo の sub が、id_token の sub と違います。");
        }

        await SetItemAsync(KeyAccessToken, accessToken);
        await SetItemAsync(KeyUserInfo, userInfo.GetRawText());
        return userInfo;
    }

    /// <summary>/token にリクエスト</summary>
    /// <remarks>**パブリック クライアントなので、秘密は送らない**（code_verifier で引き換える）。</remarks>
    private async Task<JsonElement> CallConvertCodeToTokenAsync(string code, string codeVerifier, string clientId)
    {
        using HttpResponseMessage response = await _http.PostAsync(Constants.TokenRequestUrl,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = clientId,
                ["code"] = code,
                ["redirect_uri"] = Constants.RedirectUri,
                ["code_verifier"] = codeVerifier,
            }));

        // **エラーの応答も JSON で返る**（400 ＋ error。RFC 6749 §5.2）
        JsonElement data = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();
        string? error = GetString(data, "error");
        if (!string.IsNullOrEmpty(error))
        {
            throw new InvalidOperationException("トークン要求が断られました : " + error + Describe(GetString(data, "error_description")));
        }
        response.EnsureSuccessStatusCode();
        return data;
    }

    /// <summary>id_token のクレームを確かめる</summary>
    /// <returns>sub</returns>
    /// <remarks>**署名は確かめない**（クラスの remarks を参照）。iss・aud・exp・nonce を見る。</remarks>
    private static string VerifyIdTokenClaims(string idToken, string nonce, string clientId)
    {
        string[] parts = idToken.Split('.');
        if (parts.Length != 3)
        {
            throw new InvalidOperationException("id_token が JWS の形ではありません。");
        }

        return CheckIdTokenClaims(JsonDocument.Parse(Base64Url.DecodeFromChars(parts[1])).RootElement, nonce, clientId);
    }

    /// <summary>id_token のクレーム（iss・aud・exp・nonce・sub）を確かめる</summary>
    /// <returns>sub</returns>
    private static string CheckIdTokenClaims(JsonElement claims, string nonce, string clientId)
    {
        if (GetString(claims, "iss") != Constants.Issuer)
        {
            throw new InvalidOperationException("id_token の iss（" + GetString(claims, "iss") + "）が、期待する IdP と違います。");
        }

        bool audOk = claims.TryGetProperty("aud", out JsonElement aud) && (aud.ValueKind == JsonValueKind.Array
            ? aud.EnumerateArray().Any(a => a.GetString() == clientId)
            : aud.GetString() == clientId);
        if (!audOk)
        {
            throw new InvalidOperationException("id_token の aud に、このクライアントが含まれていません。");
        }

        // **exp は小数でもよい**（RFC 7519 2 の NumericDate。#647）。GetInt64 だと小数で例外になる
        if (!claims.TryGetProperty("exp", out JsonElement exp) || exp.ValueKind != JsonValueKind.Number
            || !exp.TryGetDecimal(out decimal expSeconds)
            || expSeconds < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            throw new InvalidOperationException("id_token の期限（exp）が切れています。");
        }

        if (GetString(claims, "nonce") != nonce)
        {
            throw new InvalidOperationException("id_token の nonce が、認可要求で送った値と違います。");
        }

        string? sub = GetString(claims, "sub");
        if (string.IsNullOrEmpty(sub))
        {
            throw new InvalidOperationException("id_token に sub がありません。");
        }
        return sub;
    }

    /// <summary>認可応答（フロント チャネル）の id_token を確かめる（Hybrid。#668）</summary>
    /// <returns>sub</returns>
    /// <remarks>
    /// **署名・iss・aud・exp・nonce・c_hash を確かめる**（OpenID Connect Core 1.0 §3.3.2.12 → §3.2.2.11、§3.3.2.11）。
    /// **c_hash は必須**（code id_token）。無い・合わないなら、届いた code は、この id_token と結び付いていない。
    /// **署名の検証だけを Web Crypto に任せる**（wwwroot/js/jwsVerify.js）。鍵の選び方とクレームの確認は、ここで行う。
    /// </remarks>
    private async Task<string> VerifyFrontChannelIdTokenAsync(string idToken, string code, string nonce, string clientId)
    {
        string[] parts = idToken.Split('.');
        if (parts.Length != 3)
        {
            throw new InvalidOperationException("認可応答の id_token が JWS の形ではありません。");
        }

        JsonElement header = JsonDocument.Parse(Base64Url.DecodeFromChars(parts[0])).RootElement;
        string? alg = GetString(header, "alg");
        string? kid = GetString(header, "kid");

        // **受ける alg と、鍵の種類・c_hash のハッシュ**（none・HS* は受けない）
        (string Kty, Func<byte[], byte[]> Hash)? spec = alg switch
        {
            "RS256" => ("RSA", SHA256.HashData),
            "ES256" => ("EC", SHA256.HashData),
            "ES384" => ("EC", SHA384.HashData),
            "ES512" => ("EC", SHA512.HashData),
            _ => null,
        };
        if (spec is not (string kty, Func<byte[], byte[]> hash))
        {
            throw new InvalidOperationException("認可応答の id_token の alg（" + alg + "）は受けません。");
        }
        if (string.IsNullOrEmpty(kid))
        {
            throw new InvalidOperationException("認可応答の id_token に kid がありません（鍵を引けない）。");
        }

        string jwk = await GetJwkAsync(kid, kty);
        if (!await _js.InvokeAsync<bool>("sprpJws.verify", alg, jwk, idToken))
        {
            throw new InvalidOperationException("認可応答の id_token の署名を確かめられません。");
        }

        JsonElement claims = JsonDocument.Parse(Base64Url.DecodeFromChars(parts[1])).RootElement.Clone();
        string sub = CheckIdTokenClaims(claims, nonce, clientId);

        // **c_hash = code のハッシュ（alg の SHA）の左半分を base64url にしたもの**（§3.3.2.11）
        byte[] digest = hash(Encoding.ASCII.GetBytes(code));
        string cHash = Base64Url.EncodeToString(digest.AsSpan(0, digest.Length / 2));
        if (GetString(claims, "c_hash") != cHash)
        {
            throw new InvalidOperationException("認可応答の id_token の c_hash が、届いた code と合いません（または c_hash がありません）。");
        }

        return sub;
    }

    /// <summary>jwks_uri から、kid の鍵を引く</summary>
    /// <returns>公開鍵の成分だけの JWK（JSON 文字列）</returns>
    /// <remarks>
    /// **Discovery の issuer を照合する**（OpenID Connect Discovery 1.0 §4.3）。**鍵の種類（kty）も確かめる。**
    /// 公開鍵の成分だけを渡す（alg・use などの付帯情報は、importKey の照合で弾かれることがあるため）。
    /// </remarks>
    private async Task<string> GetJwkAsync(string kid, string kty)
    {
        JsonElement discovery = JsonDocument.Parse(await _http.GetStringAsync(Constants.DiscoveryUrl)).RootElement;
        string? jwksUri = GetString(discovery, "jwks_uri");
        if (GetString(discovery, "issuer") != Constants.Issuer || string.IsNullOrEmpty(jwksUri))
        {
            throw new InvalidOperationException("Discovery の issuer（" + GetString(discovery, "issuer") + "）が、期待する IdP と違います。");
        }

        JsonElement jwks = JsonDocument.Parse(await _http.GetStringAsync(jwksUri)).RootElement;
        if (jwks.TryGetProperty("keys", out JsonElement keys) && keys.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement k in keys.EnumerateArray())
            {
                if (GetString(k, "kid") == kid && GetString(k, "kty") == kty)
                {
                    Dictionary<string, string?> pub = kty == "RSA"
                        ? new() { ["kty"] = kty, ["n"] = GetString(k, "n"), ["e"] = GetString(k, "e") }
                        : new() { ["kty"] = kty, ["crv"] = GetString(k, "crv"), ["x"] = GetString(k, "x"), ["y"] = GetString(k, "y") };
                    return JsonSerializer.Serialize(pub);
                }
            }
        }

        throw new InvalidOperationException("id_token の kid（" + kid + "）の鍵を、jwks_uri から引けません。");
    }

    /// <summary>flow で使う client_id</summary>
    private static string ClientIdOf(string flow) => flow == FlowHybrid ? Constants.HybridClientId : Constants.ClientId;

    /// <summary>/userinfo にリクエスト</summary>
    private async Task<JsonElement> CallUserInfoAsync(string accessToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, Constants.UserInfoRequestUrl);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.ParseAdd("application/json");

        using HttpResponseMessage response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();
    }

    #endregion

    #region リソース サーバへの要求

    /// <summary>リソース サーバへの要求を作る（React 版の createHttpRequestHeader に当たる）</summary>
    /// <param name="method">HTTP メソッド</param>
    /// <param name="url">URL</param>
    /// <returns>要求（**サインインしていれば Authorization: Bearer を付ける**）</returns>
    public async Task<HttpRequestMessage> CreateHttpRequestAsync(HttpMethod method, string url)
    {
        HttpRequestMessage request = new(method, url);
        request.Headers.Accept.ParseAdd("application/json");

        string? accessToken = await GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
        return request;
    }

    #endregion

    #region 状態

    /// <summary>access_token の取得（リソース サーバへの要求に使う）</summary>
    public async Task<string?> GetAccessTokenAsync() => await GetItemAsync(KeyAccessToken);

    /// <summary>user_info の取得</summary>
    /// <returns>UserInfo の JSON（サインインしていなければ null）</returns>
    public async Task<JsonElement?> GetUserInfoAsync()
    {
        string? json = await GetItemAsync(KeyUserInfo);
        return string.IsNullOrEmpty(json) ? null : JsonDocument.Parse(json).RootElement.Clone();
    }

    /// <summary>サインインの状態を消す（サインアウト）</summary>
    public async Task ClearSignInStatusAsync()
    {
        foreach (string key in new[] { KeyFlow, KeyState, KeyNonce, KeyCodeVerifier, KeyAccessToken, KeyUserInfo })
        {
            await RemoveItemAsync(key);
        }
    }

    #endregion

    #region 部品

    /// <summary>フラグメント（# ～）を辞書にする</summary>
    private static Dictionary<string, string> ParseFragment(string fragment)
    {
        Dictionary<string, string> data = new();
        foreach (string pair in fragment.TrimStart('#').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            int i = pair.IndexOf('=');
            string key = i < 0 ? pair : pair[..i];
            string value = i < 0 ? "" : pair[(i + 1)..];
            data[Uri.UnescapeDataString(key.Replace('+', ' '))] = Uri.UnescapeDataString(value.Replace('+', ' '));
        }
        return data;
    }

    private static string? GetString(JsonElement e, string name)
        => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.String
            ? v.GetString() : null;

    /// <summary>**error_description は任意**（RFC 6749 §5.2）。無ければ括弧を付けない</summary>
    private static string Describe(string? description)
        => string.IsNullOrEmpty(description) ? "" : "（" + description + "）";

    private ValueTask<string?> GetItemAsync(string key) => _js.InvokeAsync<string?>("sessionStorage.getItem", key);

    private ValueTask SetItemAsync(string key, string value) => _js.InvokeVoidAsync("sessionStorage.setItem", key, value);

    private ValueTask RemoveItemAsync(string key) => _js.InvokeVoidAsync("sessionStorage.removeItem", key);

    #endregion
}
