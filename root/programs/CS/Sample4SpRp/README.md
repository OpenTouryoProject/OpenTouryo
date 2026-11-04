# SpRp サンプル — 外部の IdP と連携する SP / RP を、アプリ・フローごとに示す

**Open棟梁 の SP / RP の部品（`Touryo.Infrastructure.Framework.Authentication`）を使って、
外部の IdP と連携するアプリを、アプリの種類とフローごとに示す**（#649）。
**IdP には、汎用認証サイト（MultiPurposeAuthSite、MPAS）のスタブを使う**（MPAS の `container\Stub\`）。

**1 つのアプリに 1 つのプロジェクトを置き、フローは画面やコマンドで選ぶ。**
**net48 と net10.0 の両方に出す**（WebForms は net48 だけ）。

## 1. 計画と進み具合

| 系統 | アプリ | フロー | 状態 | 場所 |
|---|---|---|---|---|
| net | **EXE（CLI）** | **Device**・**PKCE** | **済** | [`DotNet/CLI/`](DotNet/CLI/) |
| net | EXE（CLI） | SAML2・AuthZCode・AuthNCode | 未（CLI には向かない。下記） | — |
| net | MVC | SAML2・AuthZCode・AuthNCode・PKCE・Device | 未 | — |
| net | WebForms（net48 のみ） | 同上 | 未 | — |
| net | WinForms | 同上 | 未 | — |
| browser | SPA（React・Blazor） | PKCE | 未 | — |

> **CLI に SAML2・AuthZCode・AuthNCode を載せていないのは、秘密を守れないからである。**
> 認可コードを秘密（`client_secret`）で引き換えるのはコンフィデンシャル クライアント（サーバ側のアプリ）で、
> **CLI のようなネイティブ アプリは PKCE を使う**（RFC 8252 §8.4・§8.5）。SAML2 の SP もサーバ側に置くものである。

## 2. IdP（MPAS スタブ）を動かす

**スタブは、MPAS の既定の設定のまま動かす。** 定義も設定も MPAS 側に在り、ここには起動の手順（[`Stub/StubCompose.ps1`](Stub/StubCompose.ps1)）だけを置く。
**サンプルは、MPAS の雛形（`_appsettings.json`）に在るクライアントを使う**（3 節）。
**スタブの使い方の一次情報は、MPAS の `container\Stub\README.md`** である。

**前提** : Docker（compose v2）、.NET 10 SDK、MPAS のリポジトリ（既定 `C:\MultiPurposeAuthSite`）、
リソース（`C:\root\files\resource`。`root\Readme.ja.md` の手順）。

```powershell
# 1. MPAS 側で、最初に一度だけ（publish と証明書。MPAS を更新したときも）
C:\MultiPurposeAuthSite\container\Stub\1_PublishStub.ps1

# 2. このリポジトリ側で、起動・停止する
cd root\programs\CS\Sample4SpRp\Stub
.\StubCompose.ps1                  # 起動（MPAS の既定の設定のまま）
.\StubCompose.ps1 -Action Config   # 設定の解決だけを見る（起動しない）
.\StubCompose.ps1 -Action Down     # 止めて消す（状態は残らない）
```

**起動の確認** : `https://localhost:44310/.well-known/openid-configuration` が 200 を返せばよい。

| 値 | 内容 |
|---|---|
| issuer・公開 URL | `https://localhost:44310`（MPAS のスタブの既定） |
| テスト利用者 | `tanaka@example.com` / `super_tanaka@example.com`（パスワードは、スタブの土台の `appsettings.json` の `TestUserPWD`。既定は MPAS の手元の `MultiPurposeAuthSiteCore/appsettings.json`） |
| クライアント | **MPAS の雛形に在るもの**（下の 3 節） |

- **MPAS のスタブと同じプロジェクト名（`mpas-stub`）で動く。** MPAS 側でスタブを別の設定で動かしているなら、先に止める
- **状態は残らない**（利用者・同意・セッションは、再起動で消える）
- **スタブの土台は、MPAS の手元の `appsettings.json`**（`STUB_APPSETTINGS`。**無ければ起動しない**）。
  クライアントの登録（`redirect_uri_saml` など）は、**雛形（`_appsettings.json`）と手元の `appsettings.json` の両方**に要る

## 2.1 外部ログインの E2E（[`E2E/ExternalLoginE2E.ps1`](E2E/ExternalLoginE2E.ps1)）

**WebApp のサンプル（C#・VB の MVC・WebForms）の外部ログイン（OIDC・SAML2）を、スタブで通す。**
**疎通テスト（`3_SmokeTest.ps1`）はフォーム ログインしか見ない**ので、外部ログインはここで見る。

```powershell
cd root\programs\CS\Sample4SpRp\E2E
.\ExternalLoginE2E.ps1                              # 5 対象 × 2 フロー（10 件）
.\ExternalLoginE2E.ps1 -Lang CS -Flow OIDC          # 絞る（-Only "net10.0" のように対象名の部分一致でも）
```

| 対象 | 待ち受け | ホスト |
|---|---|---|
| `MVC_Sample (net48)`・`MVC_Sample (VB net48)` | `https://localhost:44334` | IIS Express（構成を生成する） |
| `MVC_Sample (net10.0)` | `https://localhost:44334` | Kestrel |
| `WebForms_Sample (net48)`・`WebForms_Sample (VB net48)` | `https://localhost:44333` | IIS Express（構成を生成する） |

**1 件で確かめること**
1. **対照**：ログインの前は、認証が要る画面（MVC `/Crud1/Index`、WebForms `Aspx/start/menu.aspx`）が 200 を返さない
2. 外部ログインのボタン（OIDC `external` / `btnButton2`、SAML2 `saml2` / `btnButton3`）で、スタブの `/authorize` / `/saml2request` へリダイレクトする
3. スタブのログイン画面・同意画面を、テスト利用者で通す
4. アプリへ戻り、**認証が要る画面が 200 を返す**

- **前提（起動はしない）** : スタブ、SQL Server（MVC の `/Crud1/Index` が DB を読む）、ASP.NET 状態サービス（WebForms）、ビルド。
  **足りなければ終了コード 2**（0 = すべて OK、1 = NG あり）
- **戻り先は MPAS に登録された URL なので、アプリをその URL で動かす。** **ポートが使用中なら、その対象は NG**（Visual Studio で動かしていないか）
- **ブラウザの代わりは HttpClient**（リダイレクトは自分で辿る。**Cookie の入れ物は 1 つ** — スタブとアプリは同じ localhost なので、ブラウザと同じく Cookie を共有する）
- **SAML2 は、MPAS のクライアント（`MVC_Sample`・`WebForms_Sample`）に `redirect_uri_saml` が要る**（無いと「スタブから戻らない」で NG）

## 3. 設定（サンプルと IdP で揃える値）

**サンプル側の設定と、IdP のクライアントの登録は、1 文字も違ってはならない。**
**登録は、MPAS の雛形（`root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\_appsettings.json` の `OAuth2ClientsInformation`）を使う。**
スタブの `appsettings.json` は、この雛形そのものである（MPAS の `container\Stub\README.md` 3 節）。

| サンプル | 雛形のクライアント | client_id | 戻り先（`redirect_uri_code`） |
|---|---|---|---|
| CLI（`DotNet/CLI`） | `Device_Client`（`client_mode` = `device`。秘密なし。旧 `TestClient3`） | `ae5a179813234ca290c8de93ef2e31dc` | `http://localhost:12345/` |

**雛形には、ほかのフェーズで使えそうなものも在る**（`MVC_Sample`・`WebForms_Sample`・`SPA_Application`・`Native_Application`・
`IdFederation`（SAML2））。**どれを使うかは、そのフェーズで決める。**
**雛形に無いクライアントが要るときだけ**、設定の JSON を書いて `StubCompose.ps1 -Settings <パス>` で渡す
（書き方は MPAS の `container\Stub\appsettings.Stub.sample.json`）。

| サンプル側のキー | 意味 | IdP 側（雛形） |
|---|---|---|
| `SpRp_Isser` | 期待する issuer | （スタブの `STUB_ISSUER`） |
| `SpRp_DiscoveryUri` | Discovery の URL。**エンドポイントと `jwks_uri` はここから引く** | — |
| `OAuth2AndOidcClientID` | client_id | `OAuth2ClientsInformation` のキー |
| `SpRp_RedirectUri` | 戻り先 | `redirect_uri_code` |
| `SpRp_Scope` | 求める scope（サンプル独自のキー） | — |

**キーは部品が読むもの**（`CmnClientParams`・`OAuth2AndOIDCParams`）をそのまま使う。
**値の置き場は net48 が `App.config`、net10.0 が `appsettings.json`**（両方を同じ値にする）。

## 4. CLI（`DotNet/CLI/SpRp_CLI`）

```powershell
cd root\programs\CS\Sample4SpRp\DotNet\CLI
dotnet build SpRp_CLI.sln                         # net48 と net10.0 の両方が出る

.\SpRp_CLI\bin\Debug\net48\SpRp_CLI.exe device     # net48
dotnet .\SpRp_CLI\bin\Debug\net10.0\SpRp_CLI.dll pkce   # net10.0
```

**前提** : 基盤のビルド（`root\programs\1_BuildAll.ps1`）。`Build_net48` と `Build_netcore100` の DLL を参照する。

**ダブルクリックで回すなら、`DotNet\CLI\` のバッチを使う**（中身は上と同じ。ASCII のみ）。

| バッチ | 何をするか |
|---|---|
| `1_Build.bat` | 基盤の DLL が在るかを見てから、`dotnet build`（net48 と net10.0） |
| `2_Device.bat [net48\|net10.0]` | `device` を実行する（既定 net10.0） |
| `3_Pkce.bat [net48\|net10.0]` | `pkce` を実行する（既定 net10.0） |
| `z_Run.bat <device\|pkce> [net48\|net10.0]` | 2・3 が呼ぶ共通部分（`pause` しない。**終了コードは `SpRp_CLI` と同じ**） |

| サブコマンド | フロー | 何が起きるか |
|---|---|---|
| `device` | Device Authorization Grant（RFC 8628） | URL とコードを表示する。**別のブラウザで開いてコードを入れ、承認する**と、端末が UserInfo を表示する |
| `pkce` | 認可コード ＋ PKCE（RFC 7636）、ループバック（RFC 8252 §7.3） | **既定のブラウザが開く。** ログインして承認すると、`http://localhost:12345/` で応答を受け、UserInfo を表示する |

**終了コード** : 0 = 成功、1 = 失敗（拒否・期限切れ・検証の失敗・IdP に繋がらない）、2 = 引数の誤り。

**確かめていること**（サンプルとして、省かずに書いてある）

| フロー | 確かめること |
|---|---|
| 共通 | **Discovery の `issuer` が `SpRp_Isser` と一致するか**（OpenID Connect Discovery 1.0 §4.3） |
| `device` | **ポーリングの間隔は IdP の指示に従う**（`interval`、`slow_down` で 5 秒延ばす。§3.5）。`expires_in` を過ぎたらやめる。id_token が来れば、署名・`iss`・`aud`・`exp` を見る（nonce は無い） |
| 共通 | **UserInfo のエラー（`insufficient_scope` など）を失敗にする**（エラーも JSON で返るので、表示するだけでは成功と区別できない） |
| `pkce` | `state`、応答の `iss`（在れば。RFC 9207）、**id_token の署名・`iss`・`aud`・`exp`・`at_hash`・`c_hash`・`nonce`** |

- **id_token の鍵は、ヘッダの `kid` で `jwks_uri` から引く**（`RemoteJwkSetStore.GetJwkByDiscoveryAsync`）。
  **`alg` は非対称のもの（`JwsFactory.SupportedAlgs`）に限る**（`none`・HS* を受けない）
- **パブリック クライアントとして振る舞う**（秘密は持たず、送らない）。`Device_Client` は秘密を登録しておらず、
  `token_endpoint_auth_method` も書いていないので、MPAS は **PKCE だけでの引き換え**と、**client_id だけの Device AuthZ** を受ける
- **旧サンプル（`DAG_Login_CLI`・`LIR_Login_CLI`）と、同じクライアント・同じ戻り先を使う。** 同時には動かさないこと（ポート 12345 を取り合う）
- **デバイス認可要求には `scope`（`SpRp_Scope`）を付ける**（`DeviceAuthZRequestAsync` の `scope` を受ける版。#650）。
  **`openid` を含めないと、id_token が返らず、UserInfo も `insufficient_scope` で断られる**（認証に使えない）

- **外部のパッケージは `Newtonsoft.Json` だけ。** `System.CommandLine`（beta）・`Sharprompt` を使わないのは、
  **net48 の CLI サンプルが落とされた原因**だったため（`5_Build_CLI_sample.bat`）

**MPAS スタブでの確認（2026/11/04）** : `device` と `pkce` を、net48・net10.0 の両方で通した
（テスト利用者 `tanaka@example.com` でログインし、承認・同意して、id_token の検証と UserInfo まで）。
`device` で拒否（Deny）すると、`access_denied` で終了コード 1 になることも確かめた。
**雛形の `Device_Client`（当時の名前は `TestClient3`）に切り替えた後も、`device`・`pkce` を net48・net10.0 で通した**（スタブは MPAS の既定の設定のまま）。

**旧サンプル（`Samples4NetCore/Legacy/CLI_sample/DAG_Login_CLI`・`LIR_Login_CLI`）との違い**

| | 旧 | `SpRp_CLI` |
|---|---|---|
| 対象 | net10.0 のみ | **net48 と net10.0** |
| エンドポイント | 設定に 1 つずつ書く | **Discovery から引く** |
| `verification_uri` | 設定のルートを前に付けていた | **そのまま使う**（§3.2 で絶対 URI） |
| ポーリング | 指数バックオフ（`interval`・`slow_down` を見ない） | **IdP の指示に従う** |
| `state`・`nonce`・id_token | 確かめていない | **確かめる** |

## 5. サンプルを足すとき

1. **MPAS の雛形から、使うクライアントを選ぶ**（3 節の表に足す）。**無ければ、設定の JSON を書いて `-Settings` で渡す**
2. サンプル側の設定（3 節）を、その値に揃える
3. **1 節の表の状態を更新する**

**サーバ側のアプリ（MVC・WebForms）は、`client_secret` を持つコンフィデンシャル クライアントを使う。**
**ブラウザ（SPA）・ネイティブ（WinForms・CLI）は、秘密を持たないクライアント ＋ PKCE にする。**

## 6. エージェントが気を付けること

- **コンテナを勝手に起動・停止しない**（`AGENTS.md`）。`StubCompose.ps1 -Action Config` は起動しないので、設定の確認に使える
- **ビルドは `CS\11_Build_SpRp_sample.bat` に詰める**（`1_BuildAll.ps1` の `SpRp_sample`、`CS\0_ExecAllBat.bat` から呼ばれる）。
  **SpRp のサンプルを足したら、このバッチに足す**（net48 と net10.0 の両方）
- **単体テスト（`2_RunAllTests.ps1`）・疎通テスト（`3_SmokeTest.ps1`）・CI には載っていない。**
  外部の IdP（スタブ）が要るため。**WebApp のサンプルの外部ログインは `E2E/ExternalLoginE2E.ps1` で確かめる**（2.1 節。
  スタブと SQL Server の起動が要るので、**施主に確かめてから回す**）。CLI は手で確かめる
- **`SelectTargets.ps1` は、ここを変えると「バッチ : 11_Build_SpRp_sample.bat ＋ 手で確かめる（SpRp）」に分ける**
  （ビルドだけを回し、通しにはしない）。**手で確かめるものが残るので、検証の記録は進まない**（確かめたら `-Record`）
- **スタブは MPAS の net10.0 版**である。net48 版の MPAS に固有の振る舞いは再現しない
