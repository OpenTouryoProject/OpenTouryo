# SPA のサンプル（SpRp_React・SpRp_Blazor）

**SPA 用の PKCE プロファイル**（認可コード ＋ PKCE S256、`response_mode=fragment`、**秘密なし**）で、MPAS（のスタブ）にサインインする。
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

## 6. 困ったとき

| 症状 | 見ること |
|---|---|
| `StartUp.bat` が「Port 5173 is already in use」で止まる | もう一方（React / Blazor）や、他のアプリが 5173 を使っていないか |
| サインインの後、`/RedirectEndpoint` で止まる（トークン要求が通らない） | **CORS**（1.1 節の 3）。DevTools の Console に CORS のエラーが出ている |
| スタブの画面で「redirect_uri が違う」などのエラー | **5173 以外で動いていないか**（戻り先は 1 文字も違ってはならない） |
| Blazor で、ビルドし直したのに古い版が出る | WebAssembly のファイルはキャッシュされる。**Ctrl+Shift+R** で読み込み直す |
| Blazor の画面の下に黄色い帯（An unhandled error has occurred） | DevTools の Console に C# の例外が出ている（サインインの失敗は例外にせず、`/RedirectEndpoint` に出す） |
