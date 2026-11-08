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
| net | **EXE（CLI。サーバで動くバッチを模す）** | **Client Credentials**・**JWT Bearer**・**ROPC**（非推奨） | **済**（#668。MPAS の自己テストから移した。4 節） | 同上 |
| net | EXE（CLI） | SAML2・AuthZCode・AuthNCode | 未（CLI には向かない。下記） | — |
| net | EXE（CLI、旧サンプル・net10.0 のみ） | Device・PKCE（LIR） | **済**（スタブに合わせた。4 節） | `Samples4NetCore/Legacy/CLI_sample/` |
| net | MVC | SAML2・AuthZCode・AuthNCode・PKCE・Device | **一部**：既存の `MVC_Sample`（C#・VB の net48、C# の net10.0）の外部ログイン（OIDC・SAML2）をスタブで通す（2.1 節）。専用のサンプルは未 | `Samples/`・`Samples4NetCore/` の `MVC_Sample` |
| net | **MVC（net10.0 のみ）** | **FAPI 1.0**（認可コード ＋ `private_key_jwt` ＋ PKCE / PKCE だけ、JARM） | **済**（#668。MPAS の自己テストから移した。`private_key_jwt` と PKCE の併用は #670。4.2 節）。net48 は未。FAPI 2.0 は見送り | [`DotNet/MVC/`](DotNet/MVC/) |
| net | WebForms（net48 のみ） | 同上 | **一部**：既存の `WebForms_Sample`（C#・VB）の外部ログイン（OIDC・SAML2）をスタブで通す（2.1 節）。専用のサンプルは未 | `Samples/` の `WebForms_Sample` |
| net | WinForms | 同上 | **一部**：既存の WS クライアント（`WSClientWin_sample`・`WSClientWinCone_sample`、VB・.NET 10 版）の外部ログインを、ROPC から PKCE（LIR）に替えた。専用のサンプルは未 | `Samples/WS_sample/WSClient_sample/` ほか |
| browser | SPA（React・Blazor） | PKCE | **済**（Bearer でのリソース アクセスを含む。4.1 節） | [`SPA/`](SPA/) |
| browser | SPA（React・Blazor） | **Hybrid（`code id_token`）**（**新しく使わない**） | **済**（#668。MPAS の自己テストから移した。4.1 節）。`code token`・`code id_token token` は扱わない（Implicit 相当） | 同上 |

> **CLI に SAML2・AuthZCode・AuthNCode を載せていないのは、秘密を守れないからである。**
> 認可コードを秘密（`client_secret`）で引き換えるのはコンフィデンシャル クライアント（サーバ側のアプリ）で、
> **CLI のようなネイティブ アプリは PKCE を使う**（RFC 8252 §8.4・§8.5）。SAML2 の SP もサーバ側に置くものである。
>
> **CLI の `cc`・`jwtbearer`・`ropc` だけは、秘密と鍵を設定ファイルに持つ**（コンフィデンシャル クライアント）。
> **利用者の端末に配る CLI ではなく、サーバで動くバッチを模したもの**である（4 節）。

## 2. IdP（MPAS スタブ）を動かす

**スタブの定義は MPAS 側に在り、ここには起動の手順（[`Stub/StubCompose.ps1`](Stub/StubCompose.ps1)）と、
雛形に無いクライアントの設定（[`Stub/Sample4SpRp.stub.json`](Stub/Sample4SpRp.stub.json)）だけを置く。**
**`StubCompose.ps1` は、既定で `Sample4SpRp.stub.json` を渡す**（#668）。**雛形（`_appsettings.json`）のクライアントも、そのまま残る**（3 節）。
**スタブの使い方の一次情報は、MPAS の `container\Stub\README.md`** である。

**前提** : Docker（compose v2）、.NET 10 SDK、MPAS のリポジトリ（場所は `-MpasRoot` ＞ 環境変数 `MPAS_PATH` ＞ `root\files\tools\PowerShell\RepoPaths.csv`（無ければ雛形の `_RepoPaths.csv`）の既定。雛形は `C:\MultiPurposeAuthSite`。#652）、
リソース（`C:\root\files\resource`。`root\Readme.ja.md` の手順）。

```powershell
# 1. MPAS 側で、最初に一度だけ（publish と証明書。MPAS を更新したときも）
C:\MultiPurposeAuthSite\container\Stub\1_PublishStub.ps1

# 2. このリポジトリ側で、起動・停止する
cd root\programs\CS\Sample4SpRp\Stub
.\StubCompose.ps1                  # 起動（Sample4SpRp.stub.json を渡す）
.\StubCompose.ps1 -Action Config   # 設定の解決だけを見る（起動しない）
.\StubCompose.ps1 -Action Down     # 止めて消す（状態は残らない）
.\StubCompose.ps1 -MpasDefault     # MPAS の既定（appsettings.Stub.sample.json）で起動する（雛形に無いクライアントは居ない）
```

> **`-Settings` を付けずに MPAS 側の `2_DockerComposeUp.bat` や `docker compose` で起動すると、MPAS の既定の設定になる。**
> **雛形に無いクライアント（`SpRp_MVC`・`SPA_Hybrid` など）が居らず、スタブのエラー画面に「unauthorized_client / Invalid client_id.」が出る。**
> **起動は `StubCompose.ps1` で行う。** 起動の表示（`設定 : …`）で、どの設定を渡したかが分かる。

**起動の確認** : `https://localhost:44310/.well-known/openid-configuration` が 200 を返せばよい。

| 値 | 内容 |
|---|---|
| issuer・公開 URL | `https://localhost:44310`（MPAS のスタブの既定） |
| テスト利用者 | `tanaka@example.com` / `super_tanaka@example.com`（パスワードは、スタブの土台の `appsettings.json` の `TestUserPWD`。既定は MPAS の手元の `MultiPurposeAuthSiteCore/appsettings.json`） |
| クライアント | **MPAS の雛形に在るもの ＋ `Sample4SpRp.stub.json` で足したもの**（下の 3 節） |

- **起動・停止は、MPAS の `container\Stub\2_DockerComposeUp.bat`・`3_DockerComposeDown.bat` に任せる**（MPAS#340）。
  **publish の成果物が MPAS のソースより古ければ、起動のときに警告が出る**（起動は止めない）。出たら 1 をやり直す
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

## 2.2 CLI のログインの E2E（[`E2E/CliLoginE2E.ps1`](E2E/CliLoginE2E.ps1)）

**CLI のサンプル（`SpRp_CLI` と、旧サンプルの `DAG_Login_CLI`・`LIR_Login_CLI`）のログインを、スタブで通す。**
CLI を起動し、表示された URL を、ブラウザの代わりに HTTP で辿る。
**ブラウザを使わないグラント（`cc`・`jwtbearer`・`ropc`。#668）は、CLI を起動して終わるのを待つだけ**である。

```powershell
cd root\programs\CS\Sample4SpRp\E2E
.\CliLoginE2E.ps1                                   # 12 対象 × 正常・対照（24 件）
.\CliLoginE2E.ps1 -Flow pkce -Only "net48"          # 絞る（-Flow は device / pkce / cc / jwtbearer / ropc / All。-SkipControl で対照を省く）
```

**`cc`・`jwtbearer`・`ropc` は、`Sample4SpRp.stub.json` を渡したスタブが要る**（クライアント `SpRp_CLI_Batch` と、ROPC を開ける設定。`StubCompose.ps1` の既定）。
`device`・`pkce` は、どちらの起動でも通る。

| 対象 | フロー | 起動のしかた |
|---|---|---|
| `SpRp_CLI (net48)`・`SpRp_CLI (net10.0)` | `device` | `device` |
| `SpRp_CLI (net48)`・`SpRp_CLI (net10.0)` | `pkce` | `pkce --no-browser` |
| `DAG_Login_CLI (net10.0)` | `device` | `login --yes` |
| `LIR_Login_CLI (net10.0)` | `pkce` | `login --no-browser` |
| `SpRp_CLI (net48)`・`SpRp_CLI (net10.0)` | `cc` | `cc` |
| `SpRp_CLI (net48)`・`SpRp_CLI (net10.0)` | `jwtbearer` | `jwtbearer` |
| `SpRp_CLI (net48)`・`SpRp_CLI (net10.0)` | `ropc` | `ropc tanaka`（パスワードは標準入力） |

**1 件で確かめること**

| フロー | 辿り方 | 正常 | 対照 |
|---|---|---|---|
| `device` | 表示された `verification_uri_complete` を開き、ログインして承認する | 終了コード 0、id_token の検証と UserInfo の行 | **拒否（Deny）する** → 終了コード 1、`access_denied` |
| `pkce` | 表示された認可要求の URL を開き、ログイン・同意する。**戻り（`http://localhost:12345/?code=…`）を、E2E が CLI の待ち受けへ届ける** | 同上 | **戻りの `state` を書き換える** → 終了コード 1、state の不一致 |
| `cc` | （辿らない） | 終了コード 0、access_token の行 | **秘密を書き換えた設定で動かす** → 終了コード 1、`invalid_client` |
| `jwtbearer` | （辿らない） | 同上 | **別の鍵（`SHA256RSA_Server.pfx`）で署名させる** → 終了コード 1、`invalid_client` |
| `ropc` | （辿らない） | 終了コード 0、access_token の行、id_token の検証と UserInfo の行 | **違うパスワードを入れる** → 終了コード 1、`access_denied` |

- **対照が要るのは、確かめる処理を省いた実装でも、正常の側は通ってしまうため**
- **`cc`・`jwtbearer` の対照は、出力フォルダを `-OutputDir` に写し、設定ファイルだけを書き換えて動かす**（ビルドの成果物は変えない）。
  `jwtbearer` の別の鍵のパスワードは、スタブの土台の `appsettings.json` の `RsaPfxPassword`
- **対照は正常の後に回す。** MPAS は、クライアント認証と利用者のパスワードの**連続した**失敗を数え、5 回で断る（成功で消える。MPAS#305）
- **`ropc` の前に、スタブのログイン画面を一度開く。** MPAS は、アカウントの画面を初めて開いたときにテスト利用者を作るので、
  **スタブを起動した直後は、利用者が居ない**（`access_denied` になる）
- **CLI の標準入力に BOM を書かせない**（Windows PowerShell 5.1 は、子プロセスの標準入力に BOM を書くことがある。`[Console]::InputEncoding` を BOM 無しにしている）
- **ブラウザを開かせない・対話をさせない**ために、サンプルのオプション（`--no-browser`・`--yes`）を使う（4 節）
- **前提（起動はしない）** : スタブ、ビルド（`CS\11_Build_SpRp_sample.bat`・`CS\5_Build_CLICore_sample.bat`）、
  **ポート 12345 が空いている**（どの CLI も同じ戻り先で待ち受けるので、**1 件ずつ動かす**）。SQL Server は要らない。
  **足りなければ終了コード 2**（0 = すべて OK、1 = NG あり）
- **CLI の出力は、1 件ごとに `-OutputDir`（既定 `%TEMP%\OpenTouryoE2E`）の `cli_*.log` に残す**
- **HTTP の部品（`New-Browser` など）は `ExternalLoginE2E.ps1` と同じ作り**だが、こちらは**戻り先（12345）へのリダイレクトの手前で止まる**

## 2.3 MVC（FAPI 1.0）の E2E（[`E2E/MvcFapi1E2E.ps1`](E2E/MvcFapi1E2E.ps1)）

**`SpRp_MVC`（4.2 節）の FAPI 1.0 のフローを、スタブで通す**（#668）。**スタブには、`SpRp_MVC` のクライアントを登録して起動する。**

```powershell
cd root\programs\CS\Sample4SpRp\Stub
.\StubCompose.ps1                                     # 既定で Sample4SpRp.stub.json を渡す（SpRp_MVC ほか。雛形のクライアントも残る）

cd ..\E2E
.\MvcFapi1E2E.ps1                                  # 5 つのフロー × 正常・対照（13 件）
.\MvcFapi1E2E.ps1 -Only "pkce" -SkipControl        # 絞る（ケース名の部分一致）
```

| flow / response_mode | 正常 | 対照（書き換えると NG になること） |
|---|---|---|
| `oauth2` / `query` | access_token を得る | `state` を書き換える |
| `oidc` / `query` | ＋ id_token の検証・UserInfo（`sub` の一致） | `state` を書き換える / **認可要求の `code_challenge` を書き換える** |
| `oidc` / `query.jwt`（JARM） | 同上 | 応答（JWT）の署名を書き換える / **素の応答（`code`・`state`）に落とす** |
| `pkce` / `query` | 同上 | `state` を書き換える / **認可要求の `code_challenge` を書き換える** |
| `pkce` / `query.jwt` | 同上 | 応答（JWT）の署名を書き換える |

- **戻り（スタブ → アプリ）のリダイレクトの手前で止め、URL を書き換えてからアプリへ届ける**
- **PKCE（#670）：どの flow も、認可要求に `code_challenge`（S256）が付くことを見る。** 対照の `code_challenge` の書き換えは、
  アプリ → スタブの認可要求で行う（code が別の値に結び付くので、アプリが正しい `code_verifier` を送っても `invalid_grant` で断られる）。
  **アプリが `code_verifier` を送らなくなれば、正常の側が落ちる**（スタブは、`code_challenge` 付きの code の引き換えに `code_verifier` を求める。`private_key_jwt` でも同じ）
- **対照は、NG になり、しかも狙った確認（理由の文言）で落ちたことを見る**（別の理由で落ちたのなら、狙いは確かめられていない）
- **前提（起動はしない）** : スタブ（`StubCompose.ps1` の既定 = `Sample4SpRp.stub.json`）、ビルド（`CS\11_Build_SpRp_sample.bat`）、**ポート 44336 が空いている**。
  SQL Server は要らない。**足りなければ終了コード 2**（0 = すべて OK、1 = NG あり）
- **スタブを `-MpasDefault`（または MPAS 側のバッチ）で起動していると、`SpRp_MVC` のクライアントが無いので「スタブのエラー画面」で NG**。
  2.1・2.2 の E2E は、どちらの起動でも通る（雛形のクライアントは残る）
- **アプリのログは `-OutputDir`（既定 `%TEMP%\OpenTouryoE2E`）の `app_SpRp_MVC.log`**

## 3. 設定（サンプルと IdP で揃える値）

**サンプル側の設定と、IdP のクライアントの登録は、1 文字も違ってはならない。**
**登録は、MPAS の雛形（`root\programs\MultiPurposeAuthSiteCore\MultiPurposeAuthSiteCore\_appsettings.json` の `OAuth2ClientsInformation`）を使う。**
スタブの `appsettings.json` は、この雛形そのものである（MPAS の `container\Stub\README.md` 3 節）。

| サンプル | 雛形のクライアント | client_id | 戻り先（`redirect_uri_code`） |
|---|---|---|---|
| CLI（`DotNet/CLI`） | `Device_Client`（`client_mode` = `device`。秘密なし。旧 `TestClient3`） | `ae5a179813234ca290c8de93ef2e31dc` | `http://localhost:12345/` |
| SPA（`SPA/SpRp_React`・`SPA/SpRp_Blazor`） | `SPA_Application`（**秘密を外して使う**。4.1 節） | `f374a155909d486a9234693c34e94479` | `http://localhost:5173/RedirectEndpoint` |
| WS クライアントの外部ログイン（`Samples/WS_sample/WSClient_sample/WSClientWin_sample`・`WSClientWinCone_sample`、VB・.NET 10 版の `WSClientWin_sample`） | 同上（LIR で使う） | 同上 | 同上 |
| MVC（`DotNet/MVC`）の `oauth2`・`oidc` | **雛形に無い**：`SpRp_MVC`（[`Stub/Sample4SpRp.stub.json`](Stub/Sample4SpRp.stub.json)。`fapi1`、`private_key_jwt`） | `75d74cb5a4ae44e3afec154623f0cbb4` | `https://localhost:44336/Fapi1/Callback` |
| MVC（`DotNet/MVC`）の `pkce` | **雛形に無い**：`SpRp_MVC_PKCE`（同上。`fapi1`、秘密なし） | `9e53bded5e7d4ee4bc9e7e30f7a9f0a6` | 同上 |
| CLI（`DotNet/CLI`）の `cc`・`jwtbearer`・`ropc` | **雛形に無い**：`SpRp_CLI_Batch`（同上。normal、秘密と `jwk_rsa_publickey`） | `b9582fe575bf4ba9984f3fc81147408c` | （使わない） |
| SPA（`SPA/`）の Hybrid | **雛形に無い**：`SPA_Hybrid`（同上。normal、秘密なし） | `2ee77c67c8624f39a423c9b6c319c106` | **`redirect_uri_token`** = `http://localhost:5173/RedirectEndpoint`（Hybrid はこちらを使う） |

**`Stub/Sample4SpRp.stub.json` は、雛形に無いクライアントと設定を 1 つに集めたもの**（スタブに渡せる JSON は 1 つ）。
**ROPC も、ここで開けている**（雛形は `EnableResourceOwnerPasswordCredentialsGrantType` = false）。

**雛形には、ほかのフェーズで使えそうなものも在る**（`MVC_Sample`・`WebForms_Sample`・`SPA_Application`・`Native_Application`・
`IdFederation`（SAML2））。**どれを使うかは、そのフェーズで決める。**
**雛形に無いクライアントが要るときは、`Stub/Sample4SpRp.stub.json` に足す**（`StubCompose.ps1` が既定で渡す。別のファイルを渡すなら `-Settings <パス>`）
（書き方は MPAS の `container\Stub\appsettings.Stub.sample.json`）。

| サンプル側のキー | 意味 | IdP 側（雛形） |
|---|---|---|
| `SpRp_Issuer` | 期待する issuer | （スタブの `STUB_ISSUER`） |
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
| `4_ClientCredentials.bat [net48\|net10.0]` | `cc` を実行する（既定 net10.0） |
| `5_JwtBearer.bat [net48\|net10.0]` | `jwtbearer` を実行する（既定 net10.0） |
| `6_Ropc.bat [利用者名] [net48\|net10.0]` | `ropc` を実行する（既定 `tanaka`・net10.0。パスワードは表示せずに読む） |
| `z_Run.bat <device\|pkce\|cc\|jwtbearer\|ropc> [net48\|net10.0] [利用者名]` | 2〜6 が呼ぶ共通部分（`pause` しない。**終了コードは `SpRp_CLI` と同じ**） |

| サブコマンド | フロー | 何が起きるか |
|---|---|---|
| `device` | Device Authorization Grant（RFC 8628） | URL とコードを表示する。**別のブラウザで開いてコードを入れ、承認する**と、端末が UserInfo を表示する |
| `pkce` | 認可コード ＋ PKCE（RFC 7636）、ループバック（RFC 8252 §7.3） | **既定のブラウザが開く。** ログインして承認すると、`http://localhost:12345/` で応答を受け、UserInfo を表示する |
| `cc` | Client Credentials（RFC 6749 §4.4） | `client_secret`（Basic）で access_token を得る |
| `jwtbearer` | JWT Bearer（RFC 7523 §2.1） | **クライアントの鍵（`SpRp_RsaPfxFilePath`）で署名したアサーション**で access_token を得る（`iss`・`sub` は client_id、`aud` はトークン エンドポイント、有効期間 1 分） |
| `ropc <利用者名>` | Resource Owner Password Credentials（RFC 6749 §4.3）。**非推奨**（RFC 9700 §2.4） | **パスワードを標準入力から読み**（端末なら表示しない）、`client_secret`（Basic）と一緒に送る。id_token を検証し、UserInfo を表示する |

**`cc`・`jwtbearer`・`ropc` は、サーバで動くバッチを模したコンフィデンシャル クライアント**（`SpRp_BatchClientID`。秘密 `SpRp_BatchClientSecret` と鍵を設定ファイルに持つ）。
**利用者の端末に配る CLI に、この形を使ってはならない**（秘密を守れない）。**スタブには `Sample4SpRp.stub.json` が要る**（`StubCompose.ps1` の既定。2.2 節）。
- **`ropc` の利用者名は、MPAS のログイン画面で入れるメール アドレスではない**（テスト利用者は `tanaka`）。
  **パスワードは、コマンド ラインに書かない**（プロセスの一覧から見える）。**先頭の BOM は読み飛ばす**（Windows PowerShell 5.1 から流し込むと付くことがある）
- **`ropc` は、スタブを起動した直後には通らない**（テスト利用者は、スタブのアカウントの画面を初めて開いたときに作られる。ログイン画面を一度開く）
- **ROPC は新しく使わない。** MPAS の自己テストから移したもので、**利用者がパスワードをクライアントに渡す**形である。`device` か `pkce` にする

**`pkce --no-browser` なら、ブラウザを開かない**（認可要求の URL を表示するだけ。手で、または別の端末で開く）。E2E（2.2 節）はこれを使う。

**終了コード** : 0 = 成功、1 = 失敗（拒否・期限切れ・検証の失敗・IdP に繋がらない）、2 = 引数の誤り。

**確かめていること**（サンプルとして、省かずに書いてある）

| フロー | 確かめること |
|---|---|
| 共通 | **Discovery の `issuer` が `SpRp_Issuer` と一致するか**（OpenID Connect Discovery 1.0 §4.3） |
| `device` | **ポーリングの間隔は IdP の指示に従う**（`interval`、`slow_down` で 5 秒延ばす。§3.5）。`expires_in` を過ぎたらやめる。id_token が来れば、署名・`iss`・`aud`・`exp` を見る（nonce は無い） |
| 共通 | **UserInfo のエラー（`insufficient_scope` など）を失敗にする**（エラーも JSON で返るので、表示するだけでは成功と区別できない） |
| `pkce` | `state`、応答の `iss`（在れば。RFC 9207）、**id_token の署名・`iss`・`aud`・`exp`・`at_hash`・`c_hash`・`nonce`** |
| `cc`・`jwtbearer` | トークン応答の `error` を失敗にする。`access_token` が在るか（**トークンそのものは表示しない**） |
| `ropc` | 同上。**id_token の署名・`iss`・`aud`・`exp`・`at_hash`**、**UserInfo の `sub` が id_token と同じか**（OpenID Connect Core 1.0 §5.3.2） |

- **id_token の鍵は、ヘッダの `kid` で `jwks_uri` から引く**（`RemoteJwkSetStore.GetJwkByDiscoveryAsync`）。
  **`alg` は非対称のもの（`JwsFactory.SupportedAlgs`）に限る**（`none`・HS* を受けない）
- **`device`・`pkce` は、パブリック クライアントとして振る舞う**（秘密は持たず、送らない）。`Device_Client` は秘密を登録しておらず、
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

**旧サンプルの認証処理も、`SpRp_CLI` に合わせた（2026/11/05）。** 設定（`appsettings.json`）のキーも同じで、接続先はスタブである。
Discovery から引く・`scope` を送る・`verification_uri` をそのまま使う・ポーリングを IdP の指示に従わせる・
`state`・応答の `iss`・`nonce`・id_token を確かめる・終了コードを返す、の各点は同じになった。**残る違いは次のとおり。**

| | 旧 | `SpRp_CLI` |
|---|---|---|
| 対象 | net10.0 のみ | **net48 と net10.0** |
| 外部のパッケージ | `System.CommandLine`（beta）・`Sharprompt` | `Newtonsoft.Json` だけ |
| 対話 | `DAG` はポーリングの前に `Sharprompt` で確認する（**対話環境でしか動かない。`--yes` で飛ばせる**） | 確認しない |
| ビルド | `CS\5_Build_CLICore_sample.bat` | `CS\11_Build_SpRp_sample.bat` |

## 4.1 SPA（`SPA/SpRp_React`・`SPA/SpRp_Blazor`）

**SPA 用の PKCE プロファイル**（認可コード ＋ PKCE S256、`response_mode=fragment`、**秘密なし**）でサインインし、
**アクセス トークンを Bearer で付けて、`Samples4NetCore/Backend/ASPNETWebService` の `GET api/json/claims` を呼ぶ**
（検証に通れば sub、通らなければ「未認証」が返る）。
**簡素なサンプルである。** 正式なものは [FrontendTemplates](https://github.com/OpenTouryoProject/FrontendTemplates) にある。

**動かし方と確かめ方は [`SPA/README.md`](SPA/README.md)。** 起動は各フォルダの `StartUp.bat`（どちらも `http://localhost:5173`）。

| | React | Blazor（WebAssembly スタンドアロン） |
|---|---|---|
| 認証の部品 | `src/touryo/`（FrontendTemplates の `vite-react-ts/src/touryo` を取り込み、手を入れた） | `Touryo/OAuthOidc.cs`（React 版を C# に移した） |
| 接続先 | `src/const.ts` | `Constants.cs` |
| 戻り先の画面 | `src/components/RedirectOfAuth.tsx` | `Pages/RedirectEndpoint.razor` |

**確かめていること** : state、応答の `iss`（在れば。RFC 9207）、**id_token の `iss`・`aud`・`exp`・`nonce`**、
UserInfo の `sub` が id_token と同じか。**乱数は Web Crypto API / `RandomNumberGenerator`**（RFC 7636 §7.1）。
**id_token の署名は確かめない**（トークン エンドポイントから TLS で直接受けるため。OpenID Connect Core 1.0 §3.1.3.7 の 6）。

**Hybrid（`code id_token`。#668）** : MPAS の自己テストから移した。**ボタンを分けた**（**Sign In（Hybrid : code id_token。非推奨）**）。

> **Hybrid は新しく使わない**（ROPC と同じ扱い）。**認可コード ＋ PKCE に `openid` を付ければ、ほぼ代替できる**（上の PKCE のボタン）。
> Hybrid が `c_hash`・`nonce` で守っていたもの（code の差し替え・リプレイ）は、PKCE と `nonce`・応答の `iss`（RFC 9207）で守られる。
> 逆に、**id_token が URL のフラグメントに載り**（履歴に残りうる）、**ブラウザで署名を確かめる必要がある**（Blazor は JS 相互運用が要った）。
> **`code token`・`code id_token token` は、access_token をフロント チャネルで返す Implicit 相当なので、扱わない。**
> **このサンプルは、ブラウザで JWS の署名を確かめる例として残す。**

| | 内容 |
|---|---|
| クライアント | **`SPA_Hybrid`**（`2ee77c67c8624f39a423c9b6c319c106`。秘密なし）。**雛形に無い**（MPAS は Hybrid の戻り先を `redirect_uri_token` から引き、`SPA_Application` はそこが空）。`Stub/Sample4SpRp.stub.json` に登録 |
| code の引き換え | **PKCE**（`code_verifier`。秘密は送らない） |
| 認可応答の id_token | **署名を確かめる**（フロント チャネルで届くので、TLS で発行者を確かめられない。§3.3.2.12 → §3.2.2.11）。鍵は `jwks_uri` から `kid` で引き、**alg は RS256・ES256・ES384・ES512 だけ**。**検証は Web Crypto**（Blazor は `wwwroot/js/jwsVerify.js` を JS 相互運用で呼ぶ。ブラウザでは .NET の RSA・ECDSA を使えない） |
| | `iss`・`aud`・`exp`・`nonce`、**`c_hash`（必須。§3.3.2.11）** |
| トークン応答の id_token | 上の PKCE と同じクレーム、**認可応答の id_token と同じ `iss`・`sub` か**（§3.3.3.6） |
| 扱わないもの | **`code token`・`code id_token token`**（access_token をフロント チャネルで返す **Implicit 相当**。MPAS の雛形も Implicit を閉じており、通らない） |

- **リソース サーバ（`ASPNETWebService`）の `OAuth2AndOidcClientIDs` に `SPA_Hybrid` を足した**（`aud` を確かめるため。#655）

- **前提 : MPAS の `SPA_Application` から `client_secret` を外す**（雛形 `_appsettings.json` と手元の `appsettings.json` の両方）。
  **MPAS は、`/token`・`/userinfo` の CORS で許すオリジンを、秘密を持たないクライアントの戻り先から導く**ため、
  秘密があると `http://localhost:5173` からの要求がブラウザで止まる（プリフライトに `Access-Control-Allow-Origin` が付かない）
- **React と Blazor は、同じ戻り先（5173）を使う。** 同時には動かさない
- **Blazor の `Properties/launchSettings.json` は、リポジトリの `.gitignore` の対象**（`**/Properties/launchSettings.json`）。
  ポートを 5173 にしてあるので、**コミットするなら `git add -f`**（net10.0 の MVC_Sample と同じ）
- **保存先は sessionStorage**（state・nonce・code_verifier は往復だけに使い、使ったら消す）。
  access_token も置くので、**XSS があれば読まれる**（簡素さを優先した。実運用では BFF などを検討する）

## 4.2 MVC（`DotNet/MVC/SpRp_MVC`、FAPI 1.0）

**MPAS の自己テストの FAPI1 を、別の配備で動く本物のクライアントに移したもの**（#668）。
**自己テストは 1 つの環境でサーバとクライアントを兼ねるので、クライアント役の設定（`SpRp_*`）だけで通ることは、ここで確かめる**（#666・MPAS#339）。
**net10.0（ASP.NET Core MVC）だけ。** DB と、Open棟梁 の P 層の基底クラスは使わない（SP / RP の部品の使い方だけを見せる）。

```powershell
cd root\programs\CS\Sample4SpRp\DotNet\MVC
.\1_Build.bat        # dotnet build SpRp_MVC.sln
.\2_Run.bat          # https://localhost:44336 で待ち受ける。ブラウザで開き、フローを選ぶ
```

**前提** : 基盤のビルド（`Build_netcore100`）、**スタブには `Sample4SpRp.stub.json` が要る**（`StubCompose.ps1` の既定。2.3 節）。
**待ち受けの URL は `appsettings.json` の `Kestrel` の節**（戻り先は登録と 1 文字も違ってはならないので、`launchSettings.json` に頼らない）。

| flow | client_id（設定のキー） | トークン要求のクライアント認証 |
|---|---|---|
| `oauth2`（openid なし。`SpRp_Fapi1OAuth2Scope`） | `OAuth2AndOidcClientID` | **`private_key_jwt`（RFC 7523 §2.2。鍵は `SpRp_RsaPfxFilePath`）＋ PKCE S256**（`GetAccessTokenByCodeWithPkceAsync`。#670） |
| `oidc`（`SpRp_Scope`） | 同上 | 同上 |
| `pkce`（`SpRp_Scope`） | **`SpRp_Fapi1PkceClientID`**（サンプル独自のキー） | **無し**（PKCE S256 の `code_verifier` だけ） |

**response_mode は `query`（既定）か `query.jwt`（JARM）。** 画面のリンクで選ぶ。**どの flow も code_challenge（S256）を送る**（E2E も確かめる）。

**確かめていること**（サンプルとして、省かずに書いてある）

| 段 | 確かめること |
|---|---|
| 共通 | **Discovery の `issuer` が `SpRp_Issuer` と一致するか**（OpenID Connect Discovery 1.0 §4.3） |
| 戻り（JARM） | **署名・`iss`・`aud`・`exp`**（`ResponseObject.Verify`。JARM §4.4）。**JARM を求めたのに素の応答なら断る**（署名の無い応答に落とさせない）。使うのは、検証した応答の中の `code`・`state` だけ |
| 戻り | `state`（RFC 6749 §10.12）、応答の `iss`（在れば。RFC 9207） |
| トークン | `error` を失敗にする。`access_token` が在るか |
| `oidc`・`pkce` | **id_token の署名・`iss`・`aud`・`exp`・`at_hash`・`c_hash`（在れば）・`nonce`**。**UserInfo の `sub` が id_token と同じか**（OpenID Connect Core 1.0 §5.3.2） |

- **id_token・JARM の鍵は、ヘッダの `kid` で `jwks_uri` から引く**（`RemoteJwkSetStore`）。**`alg` は非対称のもの（`JwsFactory.SupportedAlgs`）に限る**
- **state・nonce・code_verifier は、セッションに置き、戻りで 1 回だけ使って消す**
- **トークンそのものは画面に出さない**（id_token のクレームと UserInfo だけ）

**MPAS の fapi1 の登録について（スタブで確かめたこと）**
- **認可コードの引き換えは、PKCE S256 か `private_key_jwt`（か mTLS）だけが通る。** `client_secret` では通らない。Hybrid（`code id_token`）も通らない
- **`private_key_jwt` は、登録に `token_endpoint_auth_method` の明示が要る**（鍵を登録しただけでは、その方法で通らない。MPAS#307）
- **`private_key_jwt` を登録したクライアントは、PKCE だけでは引き換えられない**（コンフィデンシャル クライアントの認証は省けない）。
  **このため、`pkce` の flow は、秘密も鍵も持たない別のクライアント（`SpRp_MVC_PKCE`）で通す**

**扱っていないもの**（#668）

| もの | 理由 |
|---|---|
| **FAPI 2.0**（`/ros`・PAR） | 見送り（MPAS の自己テストにも、本物のクライアントとして通す形が無い） |
| **mTLS**（`tls_client_auth`） | スタブがクライアント証明書を受けるかを、まだ確かめていない |
| **net48**（ASP.NET MVC 5） | 後の段で、別のプロジェクトとして足す |

## 5. サンプルを足すとき

1. **MPAS の雛形から、使うクライアントを選ぶ**（3 節の表に足す）。**無ければ、`Stub/Sample4SpRp.stub.json` に足す**
2. サンプル側の設定（3 節）を、その値に揃える
3. **1 節の表の状態を更新する**

**サーバ側のアプリ（MVC・WebForms）は、`client_secret` を持つコンフィデンシャル クライアントを使う。**
**FAPI の登録（`fapi1`）は `client_secret` を通さないので、`private_key_jwt` にする**（4.2 節。`token_endpoint_auth_method` の明示が要る）。
**ブラウザ（SPA）・ネイティブ（WinForms・CLI）は、秘密を持たないクライアント ＋ PKCE にする。**

## 6. エージェントが気を付けること

- **コンテナを勝手に起動・停止しない**（`AGENTS.md`）。`StubCompose.ps1 -Action Config` は起動しないので、設定の確認に使える
- **ビルドは `CS\11_Build_SpRp_sample.bat` に詰める**（`1_BuildAll.ps1` の `SpRp_sample`、`CS\0_ExecAllBat.bat` から呼ばれる）。
  **SpRp のサンプルを足したら、このバッチに足す**（net48 と net10.0 の両方。`SpRp_MVC` は、いまは net10.0 だけ）
- **単体テスト（`2_RunAllTests.ps1`）・疎通テスト（`3_SmokeTest.ps1`）・CI には載っていない。**
  外部の IdP（スタブ）が要るため。**WebApp のサンプルの外部ログインは `E2E/ExternalLoginE2E.ps1` で確かめる**（2.1 節。
  スタブと SQL Server の起動が要るので、**施主に確かめてから回す**）。**CLI は `E2E/CliLoginE2E.ps1` で確かめる**（2.2 節。スタブだけが要る。`cc`・`jwtbearer`・`ropc` は `Sample4SpRp.stub.json` を渡したスタブ）。
  **MVC（FAPI 1.0）は `E2E/MvcFapi1E2E.ps1` で確かめる**（2.3 節。スタブには `Sample4SpRp.stub.json` が要る）
- **`SelectTargets.ps1` は、ここを変えると「バッチ : 11_Build_SpRp_sample.bat ＋ 手で確かめる（SpRp）」に分ける**
  （ビルドだけを回し、通しにはしない）。**手で確かめるものが残るので、検証の記録は進まない**（確かめたら `-Record`）
- **スタブは MPAS の net10.0 版**である。net48 版の MPAS に固有の振る舞いは再現しない
