# SPA のサンプル（SpRp_React・SpRp_Blazor）

**SPA 用の PKCE プロファイル**（認可コード ＋ PKCE S256、`response_mode=fragment`、**秘密なし**）で、MPAS（のスタブ）にサインインし、
**得たアクセス トークンを Bearer で付けて、リソース サーバ（`Samples4NetCore/Backend/ASPNETWebService`）を呼ぶ**（6 節）。
**簡素なサンプルである。** 正式なものは [FrontendTemplates](https://github.com/OpenTouryoProject/FrontendTemplates) にある。

作りと、確かめていることは [`../README.md`](../README.md) の 4.1 節。本書は**動かし方と、確かめ方**を書く。

| | React | Blazor（WebAssembly スタンドアロン） |
|---|---|---|
| フォルダ | `SpRp_React` | `SpRp_Blazor` |
| 起動 | `StartUp.bat`（`npm run dev`） | `StartUp.bat`（`dotnet run`） |
| 待ち受け | `http://localhost:5173` | `http://localhost:5173` |

**どちらも 5173 で動かす**（MPAS の雛形の `SPA_Application` の戻り先が `http://localhost:5173/RedirectEndpoint`）。
**同時には動かせない。** `StartUp.bat` は、5173 が使用中なら起動せずに止まる。

---

## 1. 前提

### 1.1 MPAS 側（スタブ）

1. **`SPA_Application`（`f374a155…`）から `client_secret` を外す。** 雛形（`_appsettings.json`）と手元の `appsettings.json` の両方。
   **MPAS は、`/token`・`/userinfo` の CORS で許すオリジンを、秘密を持たないクライアントの戻り先から導く**ため、
   秘密があると `http://localhost:5173` からの要求がブラウザで止まる
2. **スタブを起動し直す**（土台の設定は起動時に読む）

   ```powershell
   cd root\programs\CS\Sample4SpRp\Stub
   .\StubCompose.ps1 -Action Down
   .\StubCompose.ps1 -Action Up
   ```

3. **CORS が通ったかを確かめる**（任意）。`http://localhost:5173` と出れば通っている。
   何も出なければ、秘密が残っているか、スタブが古い設定のまま

   ```powershell
   Invoke-WebRequest -Method Options https://localhost:44310/token -SkipHttpErrorCheck -Headers @{
     Origin='http://localhost:5173'; 'Access-Control-Request-Method'='POST'; 'Access-Control-Request-Headers'='content-type' } |
     % { $_.Headers['Access-Control-Allow-Origin'] }
   ```

### 1.2 手元

| | React | Blazor |
|---|---|---|
| 要るもの | Node.js（npm） | .NET 10 SDK |
| 初回 | `StartUp.bat` が `npm ci` を流す（`node_modules` が無いときだけ。`package-lock.json` どおりに入る） | 無し（`dotnet run` が復元する） |

---

## 2. 起動する

**各フォルダの `StartUp.bat` をダブルクリックする**（止めるのは Ctrl+C）。

| | 中でしていること | 気を付けること |
|---|---|---|
| React | `npm run dev -- --port 5173 --strictPort` | **`--strictPort` で、5173 が使えなければ失敗させる**（Vite は既定で 5174 などに逃げ、戻り先と合わなくなる） |
| Blazor | `dotnet run --no-launch-profile --urls http://localhost:5173` | **URL を bat で渡す。** `Properties/launchSettings.json` はリポジトリの `.gitignore` の対象で、取得した手元に無いことがある |

- 起動の表示に `Local: http://localhost:5173/`（React）／`Now listening on: http://localhost:5173`（Blazor）と出れば OK
- Visual Studio から Blazor を起動するなら、プロファイルは `http`（`launchSettings.json` が在る場合）

---

## 3. サインインする

1. **新しいタブ**（またはプライベート ウィンドウ）で `http://localhost:5173` を開く
   - **React と Blazor は同じオリジンなので、sessionStorage を共有する**（キー名も同じ）。
     **もう一方で確かめたタブを使い回すと、その結果（サインイン済み）が見える**
   - Blazor は、初回は WebAssembly の読み込みに数秒かかる
2. **Sign In** を押す（React は左上、Blazor は上の帯の右、「About」の左）。スタブのログイン画面に移る
3. **テスト利用者 `tanaka@example.com` でログインする。** パスワードは、スタブの土台の `appsettings.json` の `TestUserPWD`。
   同意画面が出たら許可する（**同意済みなら出ない**。MPAS は利用者 × クライアントで同意を覚える）
4. `/RedirectEndpoint` に戻り、トップへ移る。**ボタンが Sign Out（tanaka@example.com）になれば成功**

**失敗したら、`/RedirectEndpoint` の画面に理由が出る**（「state が一致しません」「トークン要求が断られました : …」など）。

> **2 回目のサインインで、スタブのログイン画面が出ないことがある。**
> ブラウザにスタブのログインのセッション（Cookie）が残っていると、ログイン画面を飛ばして戻るためである。
> ログイン画面から確かめたいときは、プライベート ウィンドウを使う。

---

## 4. 中身を確かめる（DevTools。任意）

| 見る場所 | 確かめること |
|---|---|
| Network の `authorize` | `code_challenge`・`code_challenge_method=S256`・`response_mode=fragment`・`nonce`・`redirect_uri` が付いている |
| Network の `token`（POST） | **`client_secret` が無く**、`code_verifier` がある |
| Network の `token`・`userinfo` の応答 | `Access-Control-Allow-Origin: http://localhost:5173` が付いている |
| Application → Session Storage（`http://localhost:5173`） | `access_token`・`user_info` が在る。**`state`・`nonce`・`code_verifier` は消えている**（使ったら消す） |
| アドレス バー | 戻ってきたとき、**`#code=…` が残っていない**（履歴から消す） |

---

## 5. 失敗する側を確かめる（任意）

| すること | 期待 |
|---|---|
| 戻ったときの URL（`/RedirectEndpoint#code=…`）を Network で拾い、**もう一度開く** | 「state が一致しません」（state は 1 度しか使わない） |
| **Sign Out** を押す | **Sign In** に戻り、Session Storage の `access_token`・`user_info` が消える |
| 同じ URL を**新しいタブ**で開く | 未サインイン（sessionStorage はタブごと） |

---

## 6. リソース サーバを Bearer で呼ぶ

**サインインで得たアクセス トークンを `Authorization: Bearer` に付けて、
`Samples4NetCore/Backend/ASPNETWebService` の `GET api/json/claims` を呼ぶ。**
リソース サーバは、**検証に通れば `userName` に sub を、通らなければ「未認証」を返す**（トークンが無くても 200 で通す）。

| | React | Blazor |
|---|---|---|
| 呼ぶ部品 | `src/components/CallApi.tsx` | `Pages/CallApi.razor`（`Home` に置いた） |
| ヘッダを作る | `oauth_oidc.createHttpRequestHeader` | `OAuthOidc.CreateHttpRequestAsync` |
| 接続先 | `src/const.ts` の `ClaimsUrl` | `Constants.cs` の `ClaimsUrl` |

### 6.1 リソース サーバを起動する

```powershell
cd root\programs\CS
.\6_Build_WSSrvCore_sample.bat        # 初回・変更の後（WSServer_sample を建ててから、ASPNETWebService を建てる）

cd Samples4NetCore\Backend\ASPNETWebService\ASPNETWebService
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --no-build --no-launch-profile --urls https://localhost:44335
```

- **ポートを明示する。** `Properties/launchSettings.json`（44335）は、リポジトリの `.gitignore` の対象で、取得した手元に無いことがある
- **`https` で待ち受けるので、開発用の証明書をブラウザが信頼している必要がある**（`dotnet dev-certs https --trust`）。
  信頼していないと、SPA の `fetch` が失敗する（`https://localhost:44335/api/json/test` を一度ブラウザで開くと分かる）
- **SQL Server は要らない**（`claims` は DB を読まない）
- **検証の設定**（`appsettings.json`）: `SpRp_Issuer` = `https://localhost:44310`、鍵は `SpRp_JwkSetUri` = `/jwkcerts`（スタブ）
- CORS は、どのオリジンも許す（`AllowAnyOrigin`。資格情報は送らない構成）

### 6.2 確かめる

| すること | 期待（画面の下に JSON が出る） |
|---|---|
| **サインインの前**に **Call API（トークン無し）** を押す | `"userName": "未認証"` |
| **サインインの後**に **Call API（Bearer）** を押す | `"userName": "<sub>"`、`"scopes": "openid,profile,email"` |
| Sign Out の後に押す | `"userName": "未認証"` に戻る |

- **sub は、UserInfo の sub と同じ値**（MPAS はクライアントごとに sub が違うことがある。WS クライアントの sub とは違ってよい）
- DevTools の Network : `claims` の要求に `Authorization: Bearer …` が付いている。**その前にプリフライト（OPTIONS）が出る**
  （`Authorization` ヘッダを付けるため）

---

## 7. 困ったとき

| 症状 | 見ること |
|---|---|
| `StartUp.bat` が「Port 5173 is already in use」で止まる | もう一方（React / Blazor）や、他のアプリが 5173 を使っていないか |
| サインインの後、`/RedirectEndpoint` で止まる（トークン要求が通らない） | **CORS**（1.1 節の 3）。DevTools の Console に CORS のエラーが出ている |
| スタブの画面で「redirect_uri が違う」などのエラー | **5173 以外で動いていないか**（戻り先は 1 文字も違ってはならない） |
| Blazor で、ビルドし直したのに古い版が出る | WebAssembly のファイルはキャッシュされる。**Ctrl+Shift+R** で読み込み直す |
| **Call API** で `Failed to fetch`（React）／`TypeError: Failed to fetch`（Blazor） | リソース サーバが動いているか（6.1 節）。**開発用の証明書をブラウザが信頼しているか** |
| **サインインしたのに `"未認証"`** | リソース サーバの `SpRp_Issuer`・`SpRp_JwkSetUri` がスタブを向いているか。**スタブを起動し直すと鍵が変わる**ことがあるので、サインインし直す |
| Blazor の画面の下に黄色い帯（An unhandled error has occurred） | DevTools の Console に C# の例外が出ている（サインインの失敗は例外にせず、`/RedirectEndpoint` に出す） |
