# ANALYSIS.md — フレームワーク中核（Infrastructure/Framework）

対象: `root/programs/CS/Frameworks/Infrastructure/Framework`（`RichClient/` を含む） / 最終更新: 2026-10-16

上位: [`Infrastructure/ANALYSIS.md`](../ANALYSIS.md) / 全体: [`Frameworks/ANALYSIS.md`](../../ANALYSIS.md)（呼び出しフロー・例外モデルの俯瞰）

---

## 1. これは何か

| アセンブリ | 名前空間 | TFM |
|---|---|---|
| `OpenTouryo.Framework` | `Touryo.Infrastructure.Framework` | net48 / net10.0 |
| `OpenTouryo.Framework.RichClient` | `Touryo.Infrastructure.Framework.RichClient` | net48 / net10.0-windows7.0 |

**P 層・B 層・D 層の「親クラス 1」**（フレームワークが持つ基底）と、その間をつなぐ**通信制御**、
**例外**、**メッセージ・共有情報**、**認証（OAuth2 / OIDC / SAML2）**を持つ。

利用者はこれを直接継承せず、`Business` の**「親クラス 2」（テンプレート）**を継承する
（[`Business/ANALYSIS.md`](../Business/ANALYSIS.md)）。

```
親クラス 1（Framework）        親クラス 2（Business、テンプレート）     アプリ
BaseController            ←  MyBaseController                  ←  画面（Web Forms）
BaseMVController          ←  MyBaseMVController                ←  コントローラ（MVC5）
BaseMVControllerCore      ←  MyBaseMVControllerCore            ←  コントローラ（ASP.NET Core）
BaseLogic                 ←  MyBaseLogic / MyFcBaseLogic       ←  業務ロジック
BaseDao                   ←  MyBaseDao ← CmnDao                ←  Dao
BaseControllerWin         ←  MyBaseControllerWin               ←  画面（WinForms / WPF）
BaseLogic2CS              ←  MyBaseLogic2CS / MyFcBaseLogic2CS ←  業務ロジック（2 層 C/S）
BaseAsyncFunc             ←  MyBaseAsyncFunc                   ←  非同期処理（リッチ クライアント）
```

---

## 2. 構成

| フォルダ | 主なクラス | 一言で |
|---|---|---|
| `Presentation/` | `BaseController`（Web Forms、約 4800 行）`BaseMasterController` `BaseMVController`（MVC5）`BaseMVControllerCore`（ASP.NET Core）`FxEventArgs` `JsonContent` | P 層の親クラス 1（4 節） |
| `Transmission/` | `CallController` `ProtocolNameService` `InProcessNameService` `WSErrorInfo` `IWCFTCPSvcForFx` | 通信制御（3 節） |
| `Business/` | `BaseLogic` `TransactionControl` | B 層の親クラス 1（5 節） |
| `Dao/` | `BaseDao` `DaoParam` | D 層の親クラス 1（6 節） |
| `Common/` | `BaseParameterValue` `BaseReturnValue` | 引数・戻り値の親クラス 1 |
| `Exceptions/` | `BusinessApplicationException` `BusinessSystemException` `FrameworkException` `FrameworkExceptionMessage` | 例外（[`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 3.3 節） |
| `Util/` | `GetMessage` `GetSharedProperty` `FxCmnFunction` `FxLiteral` `FxEnum` `Fx*Index` `UserInfo(Handle)` `SubsysInfo(Handle)` `FxSessionUtil` `GetClientIpAddress` | メッセージ・共有情報・定数（7 節） |
| `StdMigration/` | `MyHttpContext` `HttpContextExtensions` `CookieExtensions` `SessionExtensions` | `System.Web` の書き方を ASP.NET Core へ移すための補助（**net10.0 のみ**） |
| `Authentication/` | `OAuth2AndOIDCClient` `SAML2Client` `SAML2Bindings` `CmnJwtToken` `IdToken` `AccessToken` … | 認証（8 節） |
| `AsyncProcessingService/` | `AsyncCommand` `AsyncStatus` `AsyncErrorMessageID` `AsyncTaskUpdate` | 非同期処理サービスの共通定義（本リポジトリの中では使っていない） |
| `RichClient/` | `BaseControllerWin` `BaseLogic2CS` `BaseAsyncFunc` `AsyncEventFx` … | リッチ クライアント（9 節。別アセンブリ） |
| `Resources/` | `FrameworkExceptionMessageResource` | 例外メッセージ（既定と `ja-JP`） |

---

## 3. 通信制御（Transmission）

### 3.1 名前解決の 2 段

```
CallController.Invoke(serviceName, parameterValue)
  ↓ ProtocolNameService   … serviceName → protocol / URL / タイムアウト / プロパティ（TMProtocolDefinition.xml）
  ↓ protocol が InProcess なら
  ↓ InProcessNameService  … serviceName → assembly / class（TMInProcessDefinition.xml）
  ↓ Latebind.InvokeMethod(assembly, class, "DoBusinessLogic", ...)
```

定義ファイルの場所は `FxXMLTMProtocolDefinition` / `FxXMLTMInProcessDefinition`
（[`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 5.1 節）。

### 3.2 プロトコル（`FxEnum.TmProtocol`）

| 値 | プロトコル | net48 | net10.0 | 受ける側 |
|---|---|:-:|:-:|---|
| 1 | `InProcess` | ✓ | ✓ | 同じプロセス |
| 2 | `AspNetWs`（ASP.NET Web サービス） | ✓ | — | — |
| 3 | `WCF_HTTP` | ✓ | — | — |
| 4 | `WCF_TCPIP` | ✓ | — | `ServiceInterface/WCFService` |
| 5 | `AspNetWebAPI`（JSON-RPC） | ✓ | — | `ServiceInterface/ASPNETWebService`、各サンプルの `ASPNETWebService` |

- **net10.0 は `InProcess` だけ。** 列挙体からも 2〜5 を落としてある
- それでも定義ファイルに `protocol="4"` などがあれば到達する（`ProtocolNameService` は値を検査しない。
  配布している `TMProtocolDefinition.xml` にも 4・5 がある）。**`null` ではなく `FrameworkException` を投げる**（#543）
- `WCF_TCPIP` の呼び出しは、#546 でメソッドに切り出した際に欠落し、#561 で戻した

### 3.3 HTTP の接続（`AspNetWebAPI`）

- `HttpClient` / `HttpClientHandler` は**内部で都度作らず、`serviceName` ごとに持ち回る**。
  そのため **1 つの `CallController` を複数のスレッドから同時に使えない**
- `props`（定義ファイルのプロパティ）から、タイムアウト・プロキシ（`ProxyUrl` / `NetworkCredentialToProxy`）・Cookie などを反映する
- 接続オプションが要求に反映されることは、`Tests/Else/TestTransmission` が自前のオリジンとプロキシを立てて確かめる（#546。[`SMOKETEST.md`](../../../../SMOKETEST.md) 3 節）

### 3.4 `~Async`

`CallController.InvokeAsync` も `BaseLogic.DoBusinessLogicAsync` も、**別スレッドで同期版を走らせているだけ**である
（[`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 4.4 節）。

---

## 4. P 層（Presentation）

| クラス | 対象 | TFM |
|---|---|---|
| `BaseController` / `BaseMasterController` | ASP.NET Web Forms のページ・マスタ ページ | net48 |
| `BaseMVController` | ASP.NET MVC5 | net48 |
| `BaseMVControllerCore` | ASP.NET Core MVC | net10.0 |

### 4.1 Web Forms の集約イベント ハンドラ

`BaseController` は、**コントロール ID の接頭辞でイベント ハンドラを自動で結び付ける。**

1. 設定の `FxPrefixOfButton`（例 `btn`）などで、接頭辞と種類を対応付ける（15 種）
2. ページ・マスタ ページ・ユーザ コントロールのコントロールを走査し、接頭辞が合うものに共通のハンドラを付ける
3. イベントが起きると、`UOC_<コントロール ID>_<イベント名>` を `Latebind` で探して呼ぶ
   （ページ → マスタ ページ → ユーザ コントロールの順）

**接頭辞の設定が無い種類は結び付けない。** `CustomControl` の各コントロールもこの仕組みに乗る
（[`CustomControl/ANALYSIS.md`](../CustomControl/ANALYSIS.md)）。

画面遷移の制御（`FxScreenTransitionMode` / `SCDefinition.xml`）、二重送信の防止、セッション タイムアウトの検出も `BaseController` が持つ。
**約 4800 行と巨大**なので、変更は該当の `#region` に閉じる。

### 4.2 `StdMigration`（net10.0 のみ）

`MyHttpContext.Current` など、**`System.Web` の静的な書き方を ASP.NET Core に持ち込む**ための補助。
`HttpContext` / Cookie / Session に、`System.Web` に近い拡張メソッドを足す。

---

## 5. B 層（`BaseLogic`）

`DoBusinessLogic(parameterValue, iso)` が、**接続 → 前処理 → 業務処理 → コミット → 後処理、失敗ならロールバック**の型を固定する。
流れと例外ごとの振る舞いは [`Frameworks/ANALYSIS.md`](../../ANALYSIS.md) 3 節。

- **Dam は 2 通りで持つ。** 既定の 1 つ（`SetDam(dam)` / `GetDam()`）と、名前付きの複数（`SetDam(key, dam)` / `GetDam(key)`）。
  **コミット・ロールバック・切断は、両方のすべてに対して行う**
- `GetDam` と `ReturnValue` の setter は **`DoBusinessLogic` の中でしか使えない**（`WasCalledFromDoBusinessLogic`）。外で呼ぶと `FrameworkException`
- `TransactionControl` は `TCDefinition.xml` から、トランザクション パターン（接続文字列のキーと分離レベル）を引く
  （`GetTransactionPatterns` / `InitDam`）

---

## 6. D 層（`BaseDao`）

`BaseDam` を受け取り、`SetSqlByFile` / `SetSqlByCommand` / `SetParameter` / `ExecSelectFill_DT` / `ExecSelect_DR` /
`ExecSelectScalar` / `ExecInsUpDel_NonQuery` を **`protected`** で提供する。
UOC は `UOC_PreQuery` / `UOC_AfterQuery(sql)` / `UOC_AfterQuery(sql, ex)`。

**アプリは `BaseDao` を直接継承しない。** `MyBaseDao`（SQL ファイルのパス解決・SQLTRACE）か `CmnDao`（Dao を書かずに使う）を使う
（[`Business/ANALYSIS.md`](../Business/ANALYSIS.md)）。

---

## 7. メッセージ・共有情報・定数

| クラス | 読むもの | 設定キー |
|---|---|---|
| `GetMessage.GetMessageDescription(id[, culture])` | `MSGDefinition[_ja / _zh-CN].xml` | `FxXMLMSGDefinition`、`FxBusinessMessageCulture` |
| `GetSharedProperty.GetSharedPropertyValue(key)` | `SPDefinition.xml` | `FxXMLSPDefinition` |
| `TransactionControl` | `TCDefinition.xml` | `FxXMLTCDefinition` |

- 定義ファイルは**埋め込みリソース → 物理ファイル → 未設定なら空（OFF 扱い）**の順で探す
- `FxLiteral` は `Framework` の文字列定数（設定キー名を含む）。**新しい文字列リテラルはここに足す**
- `FxEnum` は列挙型（`TmProtocol` など）

---

## 8. 認証（`Authentication/`）

比較的新しく、直近も更新されている領域（#600〜#606）。

### 8.1 OAuth2 / OIDC

- `OAuth2AndOIDCClient` … Authorization Code / PKCE(S256) / Client Credentials / ROPC /
  Refresh / UserInfo / Revoke / Introspect / JWT Bearer / Device AuthZ / CIBA / Request Object / JWK Set。
  **すべて `static async Task<string>`（生の JSON を返す）**。`HttpClient` は `static` プロパティで差し替えられる
- **`OAuth2AndOIDCClient` は処理区分ごとに `partial` クラスのファイルへ分けている**（公開 API は 1 つの型のまま）。
  **区分は RFC の役割で切り、「FAPI」では切らない**（FAPI は PAR・JAR・private_key_jwt・mTLS・DPoP などの組み合わせ方の profile で、部品は FAPI 以外でも使う）

  | ファイル | 区分 | 中身 |
  |---|---|---|
  | `OAuth2AndOIDCClient.cs` | 共有基盤 | `HttpClient`・`SendAsync`、**クライアント認証 `SetClientAuthentication`**（PAR・CIBA・DPoP の Token 要求が共有する） |
  | `.Grants.cs` | 基本フロー | 認可コード（PKCE・FAPI1 の版を含む）・`PKCE_S256_CodeChallengeMethod`・Client Credentials・ROPC |
  | `.TokenManagement.cs` | トークン運用・管理 | Refresh・Revoke（RFC 7009）・Introspect（RFC 7662） |
  | `.OpenIDConnect.cs` | OIDC・メタデータ・鍵 | UserInfo・JWK Set・Discovery |
  | `.ExtensionGrants.cs` | 拡張グラント・分離フロー | JWT Bearer（RFC 7523）・Device AuthZ（RFC 8628）・CIBA |
  | `.AuthZRequest.cs` | 認可要求の保護 | Request Object の預け先（RFC 9101）・PAR（RFC 9126） |
  | `.DPoP.cs` | 送信者制約トークン | DPoP（RFC 9449） |
  | `.ClientRegistration.cs` | 動的クライアント登録 | DCR（RFC 7591 / 7592） |

  **新しい口は、区分の合うファイルに足す。** 置くのは **SP・RP（クライアント）側の処理が基本**で、IdP 側の処理（発行・口の実装）は置かない。
  **ただし、SP・RP 側の処理と対になる IdP 側の処理**（同じ形式を作る / 読む、署名する / 検証する）は、例外として含める（下の表）。
  **ログアウトは部品化を保留している**（Web Forms / MVC の SP・RP の口の共通化の観点から）。net48 の csproj は `Compile Include` を列挙しているので、ファイルを足したら並べること
- **公開情報（JWK Set・Discovery・SAML2 メタデータ）の取得は `GetStringAsync`（`internal`）に集めている。** 失敗しても例外にせず空文字を返す。
  `GetJwkSetAsync` と `GetDiscoveryDocumentAsync` はこれを呼ぶだけ（以前は `GetJwkSetAsync` を汎用の GET として代用していた）
- **CIBA の Token 要求で `private_key_jwt` / `client_secret_jwt` / `tls_client_auth` を使うなら、`client_assertion` を受ける `GetAccessTokenByCibaAsync` を使う。**
  受けない既存の版は Basic / post だけで、それ以外は従来どおり `ArgumentException`
- `OAuth2AndOIDCConst` / `OAuth2AndOIDCEnum` / `OAuth2AndOIDCParams` / `CmnClientParams` / `CmnServerParams`（#634）
- `JwkSetStore` … JWK Set のキャッシュ（**設定の `JwkSetUri` 1 本**＝自分の認可サーバ）

**#634 で、汎用認証サイト（MPAS）から次を移した**（どれも追加で、既存のシグネチャは変えていない）。

| クラス | 何をするか | 使う側 |
|---|---|---|
| `DPoPProof` | **DPoP の証明（RFC 9449）を作る・検証する**。`VerifyForResource` は `ath` と `cnf.jkt` も照合する | クライアント / AS / RS |
| `OAuth2AndOIDCClient` | `HttpClient` の get・`SendAsync`（H-1）、**`SendWithDPoPAsync`**（`DPoP-Nonce` を覚え、`use_dpop_nonce` なら 1 回だけ送り直す）・`TokenRequestWithDPoPAsync`・`GetUserInfoAsync`（DPoP 版）、**DCR**（`GetRegistrationEndpointAsync`・`RegisterClientAsync`・`Read` / `Update` / `DeleteClientRegistrationAsync`） | クライアント |
| `RemoteJwkSetStore` | **相手の `jwks_uri`（discovery）から `kid` で鍵を引く**。URL ごとのキャッシュ、取得の下限（既定 1 分）と同時 1 本、**同期の経路は `PickJwksFromCache` で待たない**（背景で補充） | RP / AS |
| `SAML2Metadata` | **相手のメタデータを読む**（`entityID` を必ず照合。署名鍵は複数） | SP |
| `SAML2Attributes` | **`AttributeStatement` を書く（IdP）・読む（SP）**。メアド・利用者名・`email_verified` の別名を吸収 | IdP / SP |
| `SAML2Client.VerifyResponseWithReason` / `SAML2VerifyResult` | **落ちた理由を返す**応答の検証（鍵は複数を順に試す） | SP |
| `OAuth2AndOIDCSpec` | 仕様が決めている小物（`error` → HTTP ステータス、`response_type` の正規化、`prompt` の集合、`max_age`、認可応答の URL（`iss` を付ける）、空白区切り、`client_assertion` と `assertion` の両受け） | AS / RS |
| `AuthenticationHeader` | `DPoP` スキームの読み書き（`GetAccessToken` はスキームも返す）、`WWW-Authenticate` の組み立て、**Basic を復号前後のどちらでも照合する版**（H-3） | AS / RS |

**SP・RP 側と対になる IdP 側の処理**（#634 の続き。例外として同じ所に置く）。どれも、配備の決めは引数で受ける
（自分の値と鍵を設定から読む版もある。下の `CmnServerParams`）。

| 置き場 | IdP 側の処理 | 対になる SP・RP 側の処理 | 呼び出し側に残す決め |
|---|---|---|---|
| `SAML2Client.VerifyRequest` | 要求の検証（Issuer から鍵を選ぶ版と、鍵を固定で渡す版） | `SAML2Client.VerifyResponse` | 鍵をどこから引くか。**鍵が無い SP をどう扱うか**（部品は通さない）。ACS URL の登録値との照合 |
| `SAML2Client.CreateRedirectResponse` / `CreatePostResponse` | 応答の作成・署名（成功のときだけ Assertion。属性も足せる） | `SAML2Client.CreateRedirectRequest` / `CreatePostRequest` | 返す先（事前登録の ACS URL）・NameID（PPID など）・出す属性・Binding・鍵 |
| `SAML2Bindings.AppendSigningCertificates` | メタデータに署名用の証明書を書き足す（鍵の入れ替え中に新旧を並べる） | `SAML2Metadata.GetSigningKeys`（複数を読む） | どの証明書を並べるか |
| `ResponseObject.Create` | JARM の ResponseObject を作る | `ResponseObject.Verify` | 鍵（alg・kid）・有効期限 |
| `OAuth2AndOIDCSpec.VerifyPkce` | PKCE の検証 | `OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod` | `plain` を受けるか |
| `JwkSetDocument` | 自分の JWK Set を出す（ファイルの変更を検知して読み直す。ETag） | `RemoteJwkSetStore`（相手の JWK Set を読む） | `Cache-Control` の秒数・304 の返し方 |

**移していないもの**（対にはなるが、汎用の部分を切り出せない、または保留）:
id_token / access_token の発行（汎用認証サイトの作り（access_token からの作り替え・利用者ストア・鍵の表）と絡み合っている）、
logout_token（ログアウトは保留）、DCR・Discovery・PAR などの受け口（配備の決めそのもの）。

**配備ごとの決めは持ち込んでいない**（MPAS 側に残る）。`jti` の使い回しを拒む台帳（`DPoPProof.CreateProofId` は鍵だけ作る）、
`nonce` の作り方、`htu` の期待値の決め方（**要求の `Host` を信じず設定から作る**）、どの口で `cnf.jkt` / `dpop_jkt` を見るか、
DCR の口の認可、クライアント登録の持ち方。

- **`OAuth2AndOIDCConst.UrnCnfJktClaim` は `urn:oauth:claim:cnf:jkt`**（`UrnCnfX5tClaim` と同じ組み立て）。
  MPAS の `Const.UrnCnfJktClaim`（`urn:oauth:cnf:jkt`）とは値が違う。プロセスの外に出ない値だが、置き換えるときは揃えること
- `DPoPProof.Create` は `iat` を**数値**で入れる（`CmnJwtToken.CreateIatClaim` は文字列を返すので使わない）。`Verify` は文字列の `iat` を断る
- `SendWithDPoPAsync` の `DPoP-Nonce` の記憶は `static`（オリジンごと）で、プロセス全体に効く

**`CmnClientParams`（`SpRp_*`）と `JwkSetUri` は、クライアント役が読む設定である**（#602）。

| キー | フレームワークでの意味 |
|---|---|
| `SpRp_Isser` / `SpRp_*Uri` / `SpRp_*CerFilePath` / `JwkSetUri` | **相手（OP / IdP）の値**（Issuer・エンドポイント・検証用の公開鍵・JWK Set の URL） |
| `SpRp_DiscoveryUri` / `SpRp_Saml2MetadataUri` / `SpRp_Saml2EntityId` | **相手（OP / IdP）の値**（Discovery・SAML2 メタデータの所在、期待する entityID）。#634 で追加 |
| `SpRp_Saml2AcsUri` | **自分の ACS の URL**（SAML2 の Audience / Recipient に照合する。空なら照合しない）。#634 で追加 |
| `SpRp_*PfxFilePath` / `SpRp_*PfxPassword` | **クライアント自身の鍵**（要求の署名・JWE の復号） |
| `JwkSetFilePath` | **認可サーバ自身の JWK Set**（`CmnJwtToken.Verify` の分岐にも使われる） |

**#634 の部品は、相手の値を引数で受ける版が基本である。** 設定を読む版（引数の少ない版）は、**相手が 1 つの配備**のための便宜で、
次の表のキーを読む。**キーが空なら何もせずに null（検証は `NoKey`）を返す**（通信もしない）。相手が複数なら、引数で受ける版を使う。

| 設定を読む版 | 読むキー |
|---|---|
| `RemoteJwkSetStore.GetJwkAsync(kid)` | `JwkSetUri` |
| `RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid)` | `SpRp_DiscoveryUri`・`SpRp_Isser` |
| `SAML2Metadata.GetAsync()` | `SpRp_Saml2MetadataUri`・`SpRp_Saml2EntityId` |
| `SAML2Client.VerifyResponseWithReason(queryString, samlResponse, expectedInResponseTo, relayState, expectedRelayState)` | `SpRp_RsaCerFilePath`・`SpRp_Saml2EntityId`・`SpRp_Saml2AcsUri` |

**配布している設定ファイル（サンプルの `appsettings.json` / `app.config`）には、これらのキーを書いていない**（どのサンプルもこの口を使っていないため）。使うアプリが足すこと。
調整値（取り直しの下限・DPoP の `iat` の窓・メタデータのキャッシュの期間）は設定にしていない。引数か static プロパティで変える。

**認可サーバを作るアプリが、`SpRp_*` を「自分の値」（自分の Issuer・`/jwkcerts` の口・署名鍵の対）として
流用すると、同じ配備でクライアント役を兼ねたときに、相手の値を置けなくなる。**
サーバ役の値は、`SpRp_*` とは別のキーに置く。上の IdP 側の処理が読むキーは、次の `CmnServerParams` にまとめた。

**`CmnServerParams` は、IdP / 認可サーバ自身の値である**（#634）。**キー名は汎用認証サイトの設定と同じ**（`SpRp_` を付けない）で、
汎用認証サイトの設定がそのまま効く。読むのは、上の IdP 側の処理の**設定を読む版（引数の少ない版）だけ**で、引数で受ける版は設定を読まない。

| 設定を読む版 | 読むキー | 無いとき |
|---|---|---|
| `ResponseObject.Create(responseParams, aud, expires)` | `IssuerId`・`RsaPfxFilePath`・`RsaPfxPassword`（RS256。`kid` はその鍵の JWK の `kid`） | `FrameworkException` |
| `SAML2Client.CreateRedirectResponse` / `CreatePostResponse`（issuer・有効期間・鍵を受けない版） | `IssuerId`・`Saml2AssertionExpireTimeSpanFromMinutes`（分）・`RsaPfxFilePath`・`RsaPfxPassword` | `FrameworkException`（**有効期間を、id_token の期間などで黙って補わない**） |
| `OAuth2AndOIDCSpec.VerifyPkce(method, challenge, verifier, out usedS256)` | `RequirePkceS256`（`true` なら S256 だけ） | `plain` も受ける。`true` / `false` 以外は `FrameworkException` |
| `SAML2Bindings.AppendSigningCertificates(metadata)` | `PreviousSigningCerFilePaths`（*.cer。空白・カンマ・タブ区切り） | 何も足さない。**読めないファイルは飛ばす**（汎用認証サイトの従来の扱い。並べた本数は戻り値で分かる） |

`JwkSetDocument.Get(out etag)` は、従来からの `JwkSetFilePath`（`OAuth2AndOIDCParams`）を読む。
これらのキーも、配布している設定ファイルには書いていない。

### 8.2 トークンの検証

型: `CmnJwtToken` `AccessToken` `IdToken` `JwtAssertion` `RequestObject` `ResponseObject` `ClaimsInRO`。

- `CmnJwtToken.Verify` の検証鍵は、`OAuth2AndOIDCParams.JwkSetFilePath` の有無（＝自分が認可サーバか）で選ぶ
- **認可サーバ兼クライアントで相手（OP）の `id_token` を検証するなら、
  `IdToken.Verify` の `jws` / `iss` / `aud` を受けるオーバーロードを使う**（#601）。`ResponseObject.Verify` にも同じ形の版がある
- 設定で判定する既存の版（`IdToken` / `ResponseObject`）も、**`JwkSetFilePath` の有無によらず `aud` を確認する**
  （`OAuth2AndOidcClientID` / `OAuth2AndOidcClientIDs` のいずれかを含むこと。#603）。
  **認可サーバ兼クライアントの自己テストでは、テスト用クライアントの `client_id` を `OAuth2AndOidcClientIDs` に並べる。**
  鍵の選び方（`JwkSetFilePath` の有無）は変えていない
- **`ResponseObject.Verify` は、`error` の応答も `iss`・`aud`・`exp` を確認する**（JARM 2.1・2.4。#644）。
  2026/10/30 までは、`error` の応答は署名だけを確かめて `true` を返していた。
  **認可サーバ側は、`error` の応答にも `iss`・`aud`・`exp` を入れること**（`ResponseObject.Create` は入れる）。
  入れない認可サーバの `error` の応答は、`false`（検証の失敗）として届く。署名が正しければ、`false` でも `jobj` は返る（検証されていない値）
- `AccessToken.Verify` の既存の版は `aud` を確認しない（Keycloak 互換）。
  **`aud` を確認するなら `jws` / `iss` / `auds`（期待する `aud` の集合）を受ける版を使う。**
  `aud` に `client_id` が入る形式では、受け付けるクライアントの `client_id` の一覧を渡す
- `AccessToken` は `typ`（`at+jwt`）を確認しない（RFC 9068 の形式には準拠していない）。
  JWS の層で照合するなら `JWS.Verify(jwt, typ)`（[`Public/Security/ANALYSIS.md`](../Public/Security/ANALYSIS.md) 4.1 節）

### 8.3 SAML2

`SAML2Client`（`CreateRedirectRequest` / `CreatePostRequest` / `VerifyResponse`）、`SAML2Bindings` `SAML2Const` `SAML2Enum` `SAML2Params`。
XML 署名は `Public.Security` の `SignedXml2`。

**`SAML2Client` は処理区分ごとに `partial` クラスのファイルへ分けている**（公開 API は 1 つの型のまま）。
各ファイルの Region は「**設定を読む版 → 引数で受ける版 → private**」の順に並べる。

| ファイル | 区分 |
|---|---|
| `SAML2Client.cs` | 要求の作成（SP → IdP。`CreateRedirectRequest` / `CreatePostRequest`） |
| `SAML2Client.VerifyResponse.cs` | 応答の検証（bool を返す従来の版） |
| `SAML2Client.VerifyResponseWithReason.cs` | 応答の検証（理由を返す版。#634。結果は `SAML2VerifyResult`） |
| `SAML2Client.VerifyRequest.cs` | 要求の検証（**IdP 側**。SP → IdP。#634） |
| `SAML2Client.CreateResponse.cs` | 応答の作成（**IdP 側**。IdP → SP。Redirect / POST。#634） |

IdP 側の 2 つは、SP 側の処理と対になるので、例外として同じクラスに置いている（`SAML2Bindings` と同じ扱い）。

**`SAML2Bindings` も、処理区分（共通・SP 側・IdP 側）ごとに `partial` クラスのファイルへ分けている。**
SAML の署名・検証は SP と IdP の双方が使うため、**IdP 側の処理も、例外として同じクラスに置いている**
（MPAS からの移行は SP・RP 側が基本だが、この様に双方が使う処理は移行の対象に含める）。

| ファイル | 区分 |
|---|---|
| `SAML2Bindings.cs` | 共通（`CreateNamespaceManager`・`GetIdIn*`・スキーマ検証） |
| `SAML2Bindings.Encoding.cs` | 共通（符号化・署名 / 復号・検証。HTTP-Redirect / HTTP-POST） |
| `SAML2Bindings.SP.cs` | SP 側（`CreateRequest`、応答を読む `Get*InResponse` / `Get*InAssertion`） |
| `SAML2Bindings.IdP.cs` | IdP 側（`CreateResponse` / `CreateAssertion` / `CreateMetadata`、要求を読む `Get*InRequest`） |

- XML の名前（XPath・要素名・属性名・接頭辞）と ID の接頭辞（`s2`）は `SAML2Const` に寄せてある。
  **テンプレートの置換キー（`{ID}` など）は、`SAML2Const` のテンプレートと対の局所的な名前なので、直書きのまま**にしている
- **`VerifyByXsd`（private）は中身の無い雛形**（XSD のファイル名と名前空間が `XXXX`）。呼ばれていないが、そのまま残している
- **`CreateMetadata` は `WantAuthnRequestsSigned` を `true` で固定している**（引数で選べない）。
  SP と IdP の双方が使う処理を移行するときに、合わせて扱う

| 何の鍵 | 既定（引数の少ない版が読む設定） | 相手ごとに替えるなら |
|---|---|---|
| `VerifyResponse` の検証用証明書 | `CmnClientParams.RsaCerFilePath`（配備に 1 本） | `cerFilePath` を受けるオーバーロード（#600）。**null・空は受けない** |
| 要求（`AuthnRequest`）の署名鍵 | `CmnClientParams.RsaPfxFilePath`（配備に 1 本） | `CreateRedirectRequest` は `digitalSign`、`CreatePostRequest` は `rsa` を受けるオーバーロード（#606）。**null は受けない** |

- **設定を読むのは、鍵を受けない版（引数の少ない版）だけである。** そこで設定から鍵を作り、鍵を受ける版へ渡す
- **鍵を受ける版は、null・空を `ArgumentNullException` にする。** 設定へは倒さない。
  `SAML2Bindings.EncodeAndSign*` は `null` なら**署名しない**ので、素通しにすると**黙って無署名の要求を出す**ことになるため。
  （#600 / #606 の時点では「null・空なら設定の鍵」だった。リリース前に、オーバーロードの形へ揃えた）
- #606 の引数は `out id` の手前に置く
- 省略可能な引数ではなくオーバーロードにしたのは、既存の版のシグネチャを変えると、
  **ビルド済みの呼び出し元が `MissingMethodException` で落ちる**ため
- Redirect の `SigAlg` は、`EncodeAndSignRedirect` に渡した `DigitalSign` のアルゴリズムで書く
  （既定の SHA256 なら `rsa-sha256`。#604 まで `rsa-sha1` 固定だった）

**`VerifyRedirect` は 2 つ在る**（#604）。

| 版 | 検証のしかた |
|---|---|
| `DigitalSign` を受ける | 渡された `DigitalSign` で検証し、**`SigAlg` がそのアルゴリズムと一致しなければ失敗する**（`SAML2Client.VerifyResponse` はこちら。ハッシュは `hashAlgorithmName`、既定は SHA256） |
| `RSA` を受ける | **許可リスト（`sigAlgs`）の中から `SigAlg` でハッシュを選ぶ**（既定は SHA256 / 384 / 512 で、SHA1 は受けない） |

- **#604 より前の Open棟梁 が送った要求・応答（`rsa-sha1` と名乗る SHA256 の署名）は、どちらでも失敗する。**
  受け手で `SigAlg` が `rsa-sha1` であることを確かめていると、#604 以降の送り手の要求・応答で落ちる
- `EncodeAndSignRedirect` / `VerifyRedirect` の引数名は、#604 で `dsRSAwithSHA1` から `ds` に変えた（SHA1 に限らないため）。
  **名前付き引数で渡している呼び出し元は、コンパイルが通らなくなる**

---

## 9. リッチ クライアント（`RichClient/`、`OpenTouryo.Framework.RichClient`）

| フォルダ | 主なクラス | 一言で |
|---|---|---|
| `Presentation/` | `BaseControllerWin` `RcFxEventArgs` `HiddenButton` | WinForms の画面の親クラス 1。`BaseController` と同じく**接頭辞でイベント ハンドラを結び付ける**（`CreatePrefixAndEvtHndHt`） |
| `Business/` | `BaseLogic2CS` | **2 層 C/S 用**の B 層の親クラス 1（クライアントのプロセスで B 層を動かし、DB に直接つなぐ） |
| `Asynchronous/` | `BaseAsyncFunc` `AsyncEventFx` `AsyncEventHeader` `AsyncEventEntry` | 画面を固めずに処理を走らせる親クラス（`WindowLock` / `WindowUnlock`、進捗の通知、`UOC_Pre` / `UOC_After` / `UOC_ABEND` / `UOC_Finally`）。`AsyncEventFx` は名前付きパイプでプロセス間のイベントを通知する |
| `Util/` | `RcFxCmnFunction` | コントロールの走査など |

- net10.0 版は `net10.0-windows7.0`（`UseWindowsForms` / `UseWPF`）。出力は `Build_netcore100\net10.0-windows7.0\`
- net48 版は `Framework` を `HintPath`（`Build_net48\`）で、net10.0 版は `ProjectReference` で参照する
- **NuGet の `Symbol_Framework.RichClient.nuspec` は、`net10.0-windows7.0` の成果物を `lib\net10.0` に置いている**（本来は `lib\net10.0-windows`）。
  直すとパッケージ解決の意味が変わるため据え置き

---

## 10. TFM 差

| | net48 | net10.0 |
|---|---|---|
| 条件コンパイル | `NET48` | **`NETSTD` を明示的に定義**（`$(DefineConstants);NETSTD`） |
| 外しているもの | `StdMigration/**` `BaseMVControllerCore.cs` `Util/GetClientIpAddress.cs`（列挙に無い） | `BaseController.cs` `BaseMasterController.cs` `BaseMVController.cs` `FxEventArgs.cs` `IWCFTCPSvcForFx.cs` `FxSessionUtil.cs` `FxHttpQueryStringIndex.cs` `RichClient/**`（`Compile Remove`） |
| 通信 | 5 プロトコル | `InProcess` のみ（3.2 節） |
| 依存 | — | `Microsoft.AspNetCore.{Http, Http.Extensions, Mvc, Session}` 2.3.0、`Microsoft.Extensions.PlatformAbstractions` 1.1.0 |

条件コンパイルは `#if (NETSTD || NETCOREAPP)` / `#if NETSTD` / `#if NETCOREAPP` が混在しているが、
**このプロジェクトではどれも同じ意味になる**（`NETSTD` を定義しているため）。

**`Microsoft.AspNetCore.*` は 2.3.0（互換のための旧パッケージ）を使い続けている。**
net10.0 のフレームワーク参照（`Microsoft.AspNetCore.App`）ではない。安易に上げると壊れる可能性が高い。

---

## 11. 落とし穴

1. **`ReturnValue` / `GetDam` は `DoBusinessLogic` の外では使えない**（5 節）。
2. **名前付きの Dam（`SetDam(key, dam)`）も、コミット・ロールバックの対象になる**（5 節）。手で閉じなくてよい。
3. **net10.0 でリモート プロトコルの定義があると `FrameworkException`**（3.2 節）。定義ファイルを net48 と共有するときに踏む。
4. **`CallController` はスレッド間で共有できない**（3.3 節）。
5. **`BaseController` は巨大**。`#region` に閉じた修正に留め、全体のリファクタリングは避ける（互換性の維持が最優先）。
6. **認証の公開 API はオーバーロードで足す**（8.3 節）。既存の版のシグネチャ・引数名を変えると、ビルド済みの呼び出し元が壊れる。
   **設定は、引数の少ない版だけが読む。** 引数で受ける版は、受けた値だけを使い、null・空で設定へ倒さない（8.1 節・8.3 節）。

---

## 12. 作業チェックリスト

- [ ] 変えるのは親クラス 1（ここ）か、親クラス 2（`Business`）か。**利用者が触るのは 2 の方**
- [ ] net48 だけ・net10.0 だけのファイルではないか（10 節）
- [ ] 公開 API を変えない（NuGet で配布している）。足すならオーバーロード
- [ ] 文字列リテラルは `FxLiteral` に、例外メッセージは `FrameworkExceptionMessage` とリソース（既定と `ja-JP`）に足す
- [ ] 通信を変えたら `TestTransmission` と疎通テスト（[`SMOKETEST.md`](../../../../SMOKETEST.md)）を回す
- [ ] 認証を変えたら、サンプルの認可サーバ・クライアント（別リポジトリを含む）への影響を報告する
