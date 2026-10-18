# ANALYSIS.md — 現行の Web サンプル（CS/Samples4NetCore/Backend）

対象: `root/programs/CS/Samples4NetCore/Backend`（net10.0） / 最終更新: 2026-10-16

上位: [`Samples4NetCore/ANALYSIS.md`](../ANALYSIS.md)（区画の俯瞰・前提・ビルド・net48 との対応）

---

## 1. これは何か

**.NET 10 の ASP.NET Core で Open棟梁 を使う、現行の推奨サンプル**（2 本）。
どちらも `Microsoft.NET.Sdk.Web`、`Host.CreateDefaultBuilder` ＋ `Startup` 方式（Minimal API ではない）。

| サンプル | 種類 | 親クラス 2 | 認証 |
|---|---|---|---|
| `MVC_Sample` | ASP.NET Core MVC（画面あり） | `MyBaseMVControllerCore` | Cookie 認証（フォーム ログイン、OAuth2 の認可コード グラント） |
| `ASPNETWebService` | Web API（**ResourceServer**。画面なし） | `MyBaseAsyncApiController` 属性 | `Authorization: Bearer`（無くても通す） |

コンテナで動かす一式は [`Docker/README.md`](../Docker/README.md)。

---

## 2. フレームワークの組み込み方（新しい Web アプリを起こすときの定型）

```csharp
// Program.cs
OAuth2AndOIDCClient.HttpClient = new HttpClient();   // 静的な HttpClient を差し込む

// Startup.cs（コンストラクタ）
GetConfigParameter.InitConfiguration(configuration);  // ★必須。無いと設定がすべて読めない

// Startup.Configure
app._UseHttpContextAccessor();   // ★必須。MyHttpContext.Current を使えるようにする
app.UseSession(...);             // セッションを使うなら（MVC_Sample のみ）
```

- **`InitConfiguration` / `_UseHttpContextAccessor` / `UseSession` の 3 点が定型**
- `_UseHttpContextAccessor` は `Framework.StdMigration.HttpContextExtensions` の拡張メソッド。
  `System.Web.HttpContext.Current` に当たるものを `MyHttpContext.Current` として用意する
  （[`Framework/ANALYSIS.md`](../../Frameworks/Infrastructure/Framework/ANALYSIS.md) 4.2 節）
- `Host.CreateDefaultBuilder` が環境変数を構成に含め、その `IConfiguration` を `InitConfiguration` に渡すので、
  **環境変数（`appSettings__キー`）の上書きがフレームワークの読む値にも効く**（4 節）

---

## 3. 各サンプル

### 3.1 `MVC_Sample`

| コントローラ | 内容 |
|---|---|
| `HomeController` | `Login`（フォーム ログイン。**ユーザ名が空でなければ認証する**）、`OAuth2AuthorizationCodeGrantClient`（認可コード → トークン → id_token の検証 → UserInfo → `SignInAsync`）、`Logout`、`Scroll` |
| `Crud1Controller` / `Crud2Controller` | CRUD。**2 つは実質同じ**（どちらも `Html.BeginForm`。違いは遷移先だけ） |
| `ErrorController` / `PingController` | エラー画面・死活確認 |

- `Crud1` / `Crud2` / `Home` / `Error` に `[Authorize(AuthenticationSchemes = Cookie)]`。ログインまわりは `[AllowAnonymous]`
- **B 層は `await layerB.DoBusinessLogicAsync(...)` で呼ぶ**（net48 版は同期の `DoBusinessLogic`）。
  ただし中身は別スレッドで同期版を走らせているだけである（[`Frameworks/ANALYSIS.md`](../../Frameworks/ANALYSIS.md) 4.4 節）
- B 層・D 層は `Logic/{Business, Common, Dao}`（`LayerB` / `LayerD` / 自動生成 Dao `DaoShippers`）
- `wwwroot/lib/`（bootstrap など）は**リポジトリに直接置いている**。npm / grunt は使わない
- `launchSettings.json` のプロファイル: `http`(5219) / `https`(7014) / `IIS Express`(26422, ssl 44383)

**OAuth2 の流れを動かすには、別リポジトリの汎用認証サイト（MultiPurposeAuthSite）が要る。**
接続先は `appsettings.json` の `SpRp_*` / `OAuth2AndOidc*` / `JwkSetUri`（既定は `https://localhost:44300/MultiPurposeAuthSite/...`）。
単体ではフォーム ログインまでしか動かない。キーの意味（相手の値か自分の値か）は
[`Framework/ANALYSIS.md`](../../Frameworks/Infrastructure/Framework/ANALYSIS.md) 8.1 節。

### 3.2 `ASPNETWebService`（ResourceServer）

| コントローラ | ルート | 内容 |
|---|---|---|
| `JsonController` | `api/Json/<action>` | Legacy の `WSServer_sample` の B 層を、CRUD の個別 Web メソッドとして公開する |
| `BatchUpdateController` | `api/BatchUpdate/<action>` | DTO（`DTTables`）の往復とバッチ更新。`RowState` / `Original` を保ち、楽観排他する |
| `SampleDataController` / `ValuesController` | `api/<controller>/<action>` | 動作確認用 |

- **`Legacy/WS_sample/Build/net10.0/` の `WSServer_sample.dll` / `WSIFType_sample.dll` を `HintPath` で参照する。**
  `6_Build_WSSrvCore_sample.bat` が先に `WSServer_sample` を建ててそこへ複写し、続けて本プロジェクトを建てる
- 自前の `Logic/`（`SuppliersLayerB` / `DaoSuppliers`）も持つ
- **OpenAPI を標準機能で出す**（`AddOpenApi` / `MapOpenApi`、`/openapi/v1.json`。#580）。
  `Microsoft.OpenApi` は脆弱性のある推移的な版を避けるため csproj に明示している
- `MVC_Sample` と違い、**Cookie 認証・セッション・データ保護を持たない**（`AddMvc` も不要。#582）。
  一方、**CORS を持つのはこちらだけ**
- `appsettings.json` の定義ファイルのパスは `C:/root/files/resource/XML/...`（**`XML` が大文字**）。
  Windows では通るが、Linux では開けない（[`CONFIGURATION.md`](../../../CONFIGURATION.md) 7-1。コンテナでは別の設定で上書きしている）

---

## 4. 環境で切り替える設定（#541 / #549）

**`appsettings.json` の値は、環境変数でそのまま上書きできる。** 区切りは `__`（下線 2 つ）。

```
appSettings__FxXMLSPDefinition=/app/files/resource/Xml/SPDefinition.xml
connectionStrings__ConnectionString_SQL=Data Source=db;...
```

> **`FxContainerization` は要らない。** あちらは ON のとき「接頭辞なしのキー名」で環境変数を読む別の仕組みである
> （[`CONFIGURATION.md`](../../../CONFIGURATION.md) 4 節）。

両サンプルとも、環境ごとに変えたい振る舞いを `appSettings` のキーで切り替える口を持つ
（`Startup.cs` 冒頭のコメントに一覧がある）。**既定はいずれも従来どおり**（サンプルは平文 HTTP で動かすため）。

| キー | 既定 | 効果 | `ASPNETWebService` |
|---|---|---|---|
| `UseHttpsRedirection` | `off` | `on` で `app.UseHttpsRedirection()` を挟む | あり |
| `CookieSecurePolicy` | (空) | `always` で Cookie に `Secure` を必ず付ける | —（Cookie を使わない） |
| `DataProtectionKeyPath` | (空) | データ保護の鍵を、指定フォルダへ永続化する | — |
| `UseForwardedHeaders` | `off` | `X-Forwarded-Proto` / `-For` を取り込む（#549） | あり |
| `ForwardedHeadersKnownProxies` | (空) | 信用する前段のアドレス（空 = 範囲を制限しない） | あり |

### 4.1 踏むこと（実測）

**① `UseHttpsRedirection` は、`on` にしただけでは何もしない。**
リダイレクト先のポートが決まらないと、警告を 1 行出して素通りする。**有効にしたのに HTTP のまま 200 が返る。**

```
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.
```

| 与え方 | 結果（実測） |
|---|---|
| `ASPNETCORE_HTTPS_PORT=44399`（**単数**） | 307 `Location: https://localhost:44399/...` |
| `HTTPS_PORT=44399` | 307（同上） |
| `ASPNETCORE_HTTPS_PORTS=44399`（**複数**） | **効かない**（200 のまま） |

複数形は Kestrel が待ち受けるポートを決めるもので、**このミドルウェアが読むのは単数形。**

**② `CookieSecurePolicy=always` を平文 HTTP でやると、ログインできなくなる。**
Antiforgery の Cookie も返らなくなるため、症状は**「ログイン画面は出るが、POST が 400」**になる。

**③ データ保護は `AddCookie` ではなくアプリ全体に設定する。**
`AddCookie` の `options.DataProtectionProvider` は**認証 Cookie にしか効かない。**
セッションと Antiforgery も同じ鍵を使うので、`services.AddDataProtection()` 側に置く。

```csharp
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
    .SetApplicationName("MVC_Sample");   // 既定はコンテンツ ルートのパス由来
```

**`SetApplicationName` も要る。** 既定のアプリケーション名はコンテンツ ルートのパスから決まるため、
鍵を共有していても**配置先のパスが違うと復号できない**（Windows の `C:\...` とコンテナの `/app` など）。
net48 側の対応物は `machineKey`（`Samples/WebApp_sample/MVC_Sample` の `Web.config` のコメント）。

**④ リバース プロキシで TLS を終端するなら `UseForwardedHeaders` が要る（#549）。**
前段で終端すると、アプリから見た接続は HTTP になる。`Request.IsHttps` が false のままなので、
**ブラウザは HTTPS で繋いでいるのに `Secure` 属性が付かない**（#536 でフレームワークが立てるようにした分が効かない）。

実測（`X-Forwarded-Proto: https` を付けて、平文 HTTP でログインしたときの `Set-Cookie`）。

| 設定 | `secure` が付いた Cookie |
|---|---|
| 既定（`off`） | **0 / 1 件** |
| `UseForwardedHeaders=on` | **3 / 4 件**（`SessionTimeOut` / `.AspNetCore.Cookies` / `mvc_session`） |
| `on` ＋ `ForwardedHeadersKnownProxies=10.0.0.1` | **0 / 1 件**（前段の範囲外なので捨てられる） |
| `on` ＋ ヘッダ無し | **0 / 1 件**（素の HTTP のまま。正しい） |

> **`KnownIPNetworks` / `KnownProxies` を設定しないと黙って無視される。**
> 既定ではループバックからの転送しか信用しない。コンテナや Kubernetes では前段が別アドレスになるため、
> **指定しないと何も起きない。**「on にしたのに直らない」の原因はほぼこれ。
>
> 逆に無条件に信用すると、**クライアントがヘッダを詐称できる**。前段だけが到達できるネットワークに閉じてから使うこと。

net48 側はコードから変えられない（`Request.IsSecureConnection` は読み取り専用）ため、
IIS ＋ ARR の URL Rewrite で `HTTPS` サーバ変数を立てる（[`CONFIGURATION.md`](../../../CONFIGURATION.md) 8 節）。

---

## 5. ビルドと検証

| bat | 建てるもの |
|---|---|
| `6_Build_WSSrvCore_sample.bat` | `Legacy/WS_sample/WSServer_sample` → `Build/net10.0/` へ複写 → `ASPNETWebService` |
| `10_Build_WebAppCore_sample.bat` | `MVC_Sample` |

| 疎通テスト | ホスト | 確かめること |
|---|---|---|
| `MVC_Sample (net10.0)` | Kestrel（51083） | `/Home/Login` を GET → 偽造防止トークンを付けて POST → **`/Crud1/Index` が 200** |
| `TestWebAPIClient (net10.0)` | Kestrel（51088） | `ASPNETWebService` に対して CRUD の一巡・DTO の往復・楽観排他 |
| `OpenAPI (net10.0)` | Kestrel（51093） | `/openapi/v1.json` が仕様として読める（版・`paths`・代表的な API が**名前で**載っている） |

どれも DB（Northwind）が要る（OpenAPI を除く）。詳細は [`SMOKETEST.md`](../../../SMOKETEST.md)。

---

## 6. 落とし穴

1. **`InitConfiguration` / `_UseHttpContextAccessor` を忘れると、設定や `MyHttpContext.Current` が読めない**（2 節）。
2. **`ASPNETWebService` は Legacy の `WSServer_sample` に依存する**（3.2 節）。`6_` の bat を経由せずに建てると古い DLL を見る。
3. **`Crud1` と `Crud2` は同じもの**（3.1 節）。片方だけ直すと食い違う。
4. **`ASPNETWebService` の `XML` は大文字**（3.2 節）。Linux では読めない。
5. **4 節の切り替えは、組み合わせを誤ると「設定したのに何も起きない」になる**（4.1 節）。
6. **OAuth2 の流れは外部の認証サイトが要る**（3.1 節）。疎通テストはフォーム ログインだけを見る。

---

## 7. 作業チェックリスト

- [ ] 部品を先にビルドする（`Nuget_netcore100.sln` → `Business_netcore100.sln`）
- [ ] `ASPNETWebService` を触るなら、`6_Build_WSSrvCore_sample.bat` で建てる
- [ ] 設定のキーを足したら、`appsettings.json` と、コンテナ用の `Docker/*/appsettings.Container.json` / `docker-compose.yml` を見る
- [ ] 変えたサンプルだけを `-Only` で疎通する
- [ ] net48 版（`Samples/WebApp_sample/MVC_Sample`、`Samples/WS_sample/ASPNETWebService`）への追随の要否を報告する
