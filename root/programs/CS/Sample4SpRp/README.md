# SpRp サンプル — 外部の IdP と連携する SP / RP を、アプリ・フローごとに示す

**Open棟梁 の SP / RP の部品（`Touryo.Infrastructure.Framework.Authentication`）を使って、
外部の IdP と連携するアプリを、アプリの種類とフローごとに示す**（#649）。
**IdP には、汎用認証サイト（MultiPurposeAuthSite、MPAS）のスタブを使う**（MPAS の `container\Stub\`）。

**1 つのアプリに 1 つのプロジェクトを置き、フローは画面やコマンドで選ぶ。**
**net48 と net10.0 の両方に出す**（WebForms は net48 だけ）。

## 1. 計画と進み具合

| 系統 | アプリ | フロー | 状態 | 場所 |
|---|---|---|---|---|
| net | **EXE（CLI）** | **Device**・**PKCE** | **済** | [`CLI/`](CLI/) |
| net | EXE（CLI） | SAML2・AuthZCode・AuthNCode | 未（CLI には向かない。下記） | — |
| net | MVC | SAML2・AuthZCode・AuthNCode・PKCE・Device | 未 | — |
| net | WebForms（net48 のみ） | 同上 | 未 | — |
| net | WinForms | 同上 | 未 | — |
| browser | SPA（React・Blazor） | PKCE | 未 | — |

> **CLI に SAML2・AuthZCode・AuthNCode を載せていないのは、秘密を守れないからである。**
> 認可コードを秘密（`client_secret`）で引き換えるのはコンフィデンシャル クライアント（サーバ側のアプリ）で、
> **CLI のようなネイティブ アプリは PKCE を使う**（RFC 8252 §8.4・§8.5）。SAML2 の SP もサーバ側に置くものである。

## 2. IdP（MPAS スタブ）を動かす

**スタブの定義は MPAS 側に在り、ここには設定（[`Stub/sprp-stub.json`](Stub/sprp-stub.json)）だけを置く。**
**スタブの使い方の一次情報は、MPAS の `container\Stub\README.md`** である。

**前提** : Docker（compose v2）、.NET 10 SDK、MPAS のリポジトリ（既定 `C:\MultiPurposeAuthSite`）、
リソース（`C:\root\files\resource`。`root\Readme.ja.md` の手順）。

```powershell
# 1. MPAS 側で、最初に一度だけ（publish と証明書。MPAS を更新したときも）
C:\MultiPurposeAuthSite\container\Stub\1_PublishStub.ps1

# 2. このリポジトリ側で、起動・停止する
cd root\programs\CS\Sample4SpRp\Stub
.\StubCompose.ps1                  # 起動（sprp-stub.json を設定として渡す）
.\StubCompose.ps1 -Action Config   # 設定の解決だけを見る（起動しない）
.\StubCompose.ps1 -Action Down     # 止めて消す（状態は残らない）
```

**起動の確認** : `https://localhost:44310/.well-known/openid-configuration` が 200 を返せばよい。

| 値 | 内容 |
|---|---|
| issuer・公開 URL | `https://localhost:44310`（MPAS のスタブの既定） |
| テスト利用者 | `tanaka@example.com` / `super_tanaka@example.com`（パスワードは MPAS の `.env.sample` の `STUB_TEST_USER_PWD`） |
| クライアント | **`sprp-stub.json` に、サンプルごとに 1 つ**（下の 3 節） |

- **MPAS のスタブと同じプロジェクト名（`mpas-stub`）で動く。** MPAS 側でスタブを別の設定で動かしているなら、先に止める
- **状態は残らない**（利用者・同意・セッションは、再起動で消える）

## 3. 設定（サンプルと IdP で揃える値）

**サンプル側の設定と、`sprp-stub.json` のクライアントの登録は、1 文字も違ってはならない。**

| サンプル側のキー | 意味 | IdP 側（`sprp-stub.json`） |
|---|---|---|
| `SpRp_Isser` | 期待する issuer | （スタブの `STUB_ISSUER`） |
| `SpRp_DiscoveryUri` | Discovery の URL。**エンドポイントと `jwks_uri` はここから引く** | — |
| `OAuth2AndOidcClientID` | client_id | `OAuth2ClientsInformation` のキー |
| `SpRp_RedirectUri` | 戻り先 | `redirect_uri_code` |
| `SpRp_Scope` | 求める scope（サンプル独自のキー） | — |

**キーは部品が読むもの**（`CmnClientParams`・`OAuth2AndOIDCParams`）をそのまま使う。
**値の置き場は net48 が `App.config`、net10.0 が `appsettings.json`**（両方を同じ値にする）。

## 4. CLI（`CLI/SpRp_CLI`）

```powershell
cd root\programs\CS\Sample4SpRp\CLI
dotnet build SpRp_CLI.sln                         # net48 と net10.0 の両方が出る

.\SpRp_CLI\bin\Debug\net48\SpRp_CLI.exe device     # net48
dotnet .\SpRp_CLI\bin\Debug\net10.0\SpRp_CLI.dll pkce   # net10.0
```

**前提** : 基盤のビルド（`root\programs\1_BuildAll.ps1`）。`Build_net48` と `Build_netcore100` の DLL を参照する。

**ダブルクリックで回すなら、`CLI\` のバッチを使う**（中身は上と同じ。ASCII のみ）。

| バッチ | 何をするか |
|---|---|
| `1_Build.bat` | 基盤の DLL が在るかを見てから、`dotnet build`（net48 と net10.0） |
| `2_Device.bat [net48\|net10.0]` | `device` を実行する（既定 net10.0） |
| `3_Pkce.bat [net48\|net10.0]` | `pkce` を実行する（既定 net10.0） |
| `z_Run.bat <device\|pkce> [net48\|net10.0]` | 2・3 が呼ぶ共通部分（`pause` しない。**終了コードは `SpRp_CLI` と同じ**） |

| サブコマンド | フロー | 何が起きるか |
|---|---|---|
| `device` | Device Authorization Grant（RFC 8628） | URL とコードを表示する。**別のブラウザで開いてコードを入れ、承認する**と、端末が UserInfo を表示する |
| `pkce` | 認可コード ＋ PKCE（RFC 7636）、ループバック（RFC 8252 §7.3） | **既定のブラウザが開く。** ログインして承認すると、`http://localhost:54649/` で応答を受け、UserInfo を表示する |

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
- **パブリック クライアントである**（`token_endpoint_auth_method` = `none`）。秘密は持たず、送らない
- **デバイス認可要求には `scope`（`SpRp_Scope`）を付ける**（`DeviceAuthZRequestAsync` の `scope` を受ける版。#650）。
  **`openid` を含めないと、id_token が返らず、UserInfo も `insufficient_scope` で断られる**（認証に使えない）

**MPAS スタブでの確認（2026/11/04）** : `device` と `pkce` を、net48・net10.0 の両方で通した
（テスト利用者 `tanaka@example.com` でログインし、承認・同意して、id_token の検証と UserInfo まで）。
`device` で拒否（Deny）すると、`access_denied` で終了コード 1 になることも確かめた。
- **外部のパッケージは `Newtonsoft.Json` だけ。** `System.CommandLine`（beta）・`Sharprompt` を使わないのは、
  **net48 の CLI サンプルが落とされた原因**だったため（`5_Build_CLI_sample.bat`）

**旧サンプル（`Samples4NetCore/Legacy/CLI_sample/DAG_Login_CLI`・`LIR_Login_CLI`）との違い**

| | 旧 | `SpRp_CLI` |
|---|---|---|
| 対象 | net10.0 のみ | **net48 と net10.0** |
| エンドポイント | 設定に 1 つずつ書く | **Discovery から引く** |
| `verification_uri` | 設定のルートを前に付けていた | **そのまま使う**（§3.2 で絶対 URI） |
| ポーリング | 指数バックオフ（`interval`・`slow_down` を見ない） | **IdP の指示に従う** |
| `state`・`nonce`・id_token | 確かめていない | **確かめる** |

## 5. サンプルを足すとき

1. **`sprp-stub.json` にクライアントを 1 つ足す**（client_id は 32 桁の 16 進。サンプルごとに分ける）
2. サンプル側の設定（3 節）を、その値に揃える
3. **1 節の表の状態を更新する**

**サーバ側のアプリ（MVC・WebForms）は、`client_secret` を持つコンフィデンシャル クライアントとして登録する。**
**ブラウザ（SPA）・ネイティブ（WinForms・CLI）は、パブリック（`none`）＋ PKCE にする。**

## 6. エージェントが気を付けること

- **コンテナを勝手に起動・停止しない**（`AGENTS.md`）。`StubCompose.ps1 -Action Config` は起動しないので、設定の確認に使える
- **CI・疎通テスト（`3_SmokeTest.ps1`）には載っていない。** ブラウザでのログインと承認が要るため、**動作の確認は手で行う**
- **ビルドの一括スクリプト（`1_BuildAll.ps1`・`CS\*.bat`）にも、まだ載っていない**
- **スタブは MPAS の net10.0 版**である。net48 版の MPAS に固有の振る舞いは再現しない
