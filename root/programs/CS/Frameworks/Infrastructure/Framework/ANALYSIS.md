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
- **認可コードを `private_key_jwt` で引き換え、PKCE も併せるなら、`GetAccessTokenByCodeWithPkceAsync` を使う**（#670）。
  アサーションの版（`GetAccessTokenByCodeAsync(uri, redirect_uri, code, assertion, authMethod)`）は `code_verifier` を送らず、
  `code_challenge` を付けて求めたコードを引き換えられない（RFC 7636 §4.5）。**名前を分けた**のは、(Uri, string × 4, AuthMethods) の版がすでに在り、
  引数の型の並びが同じ版を足せないため。`code_verifier`・`assertion` は必須で、`authMethod` は受けない（本文の形は `client_secret_jwt` でも同じ）
- **CIBA の Token 要求で `private_key_jwt` / `client_secret_jwt` / `tls_client_auth` を使うなら、`client_assertion` を受ける `GetAccessTokenByCibaAsync` を使う。**
  受けない既存の版は Basic / post だけで、それ以外は従来どおり `ArgumentException`
- `OAuth2AndOIDCConst` / `OAuth2AndOIDCEnum` / `OAuth2AndOIDCParams` / `CmnClientParams` / `CmnServerParams`（#634）
- `JwkSetStore` … JWK Set のキャッシュ（**設定の `SpRp_JwkSetUri` 1 本**＝この配備が信頼する認可サーバ。改名前の `JwkSetUri` は読まない。#666）

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

- **`response_type`・`prompt`・PKCE のメソッド名は、大文字小文字を区別する**（RFC 6749 1.9・OIDC Core 3.1.2.1・RFC 7636 6.2.1。#643）。
  2026/10/30 までは、小文字にしてから比べていた（`CODE`・`NONE`・`s256` を受けていた）。MPAS から移したときの扱いである
- **Basic の資格情報は、最初の `:` で分ける**（`password` は `:` を含みうる。RFC 7617 2。#643）。
  2026/10/30 までは、3 つ以上に分かれると `false` だった
- **Basic の復号前の値での照合（H-3）は、4.0 で既定ではやめる予定である**（RFC 6749 2.3.1 に合わせる。#643）。
  3.3 では振る舞いを変えず、**`out bool usedRawValue` を受ける版**を足した（復号前の値で通ったときだけ `true`）。
  利用側は、これでログを出すなどして、**符号化しないクライアント（#592 より前の Open棟梁 など）を見つけて直す**

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

- **`DPoPProof.Verify` の `htu` は、RFC 3986 の正規化の後に完全一致で比べる**（RFC 9449 4.3。#642）。
  **末尾の `/` と、パスの大文字小文字は区別する。** 2026/10/30 までは、どちらも区別せずに比べていた。
  設定から作る `htu` の期待値は、**クライアントが使う URL（ディスカバリで広告した値）と同じ書き方にする**

- **`OAuth2AndOIDCConst.UrnCnfJktClaim` は `urn:oauth:claim:cnf:jkt`**（`UrnCnfX5tClaim` と同じ組み立て）。
  MPAS の `Const.UrnCnfJktClaim`（`urn:oauth:cnf:jkt`）とは値が違う。プロセスの外に出ない値だが、置き換えるときは揃えること
- `DPoPProof.Create` は `iat` を**数値**で入れる（`CmnJwtToken.CreateIatClaim` は文字列を返すので使わない）。`Verify` は文字列の `iat` を断る。
  **小数の `iat` は受ける**（RFC 7519 2 の NumericDate。#647。2026/11/05 までは整数でなければ断っていた）
- `SendWithDPoPAsync` の `DPoP-Nonce` の記憶は `static`（オリジンごと）で、プロセス全体に効く

**`CmnClientParams`（`SpRp_*`）と `SpRp_JwkSetUri` は、クライアント役が読む設定である**（#602）。

| キー | フレームワークでの意味 |
|---|---|
| `SpRp_Issuer` / `SpRp_*Uri` / `SpRp_*CerFilePath` / `SpRp_JwkSetUri` | **相手（OP / IdP）の値**（Issuer・エンドポイント・検証用の公開鍵・JWK Set の URL） |
| `SpRp_DiscoveryUri` / `SpRp_Saml2MetadataUri` / `SpRp_Saml2EntityId` | **相手（OP / IdP）の値**（Discovery・SAML2 メタデータの所在、期待する entityID）。#634 で追加 |
| `SpRp_Saml2AcsUri` | **自分の ACS の URL**（SAML2 の Audience / Recipient に照合する。空なら照合しない）。#634 で追加 |
| `SpRp_*PfxFilePath` / `SpRp_*PfxPassword` | **クライアント自身の鍵**（要求の署名・JWE の復号） |
| `JwkSetFilePath` | **認可サーバ自身の JWK Set**（`CmnJwtToken.Verify` の分岐にも使われる） |

**改名した設定キーがある**（#666）。プロパティ名（`CmnClientParams.Isser`・`OAuth2AndOIDCParams.JwkSetUri`・`RequestObjectRegUri`・`AuthRequestPushUri`）は変えていない。

| 新しい名前 | 改名前の名前 | 改名前の名前を | 理由 |
|---|---|---|---|
| `SpRp_Issuer` | `SpRp_Isser` | **読み続ける**（期限は決めていない。表は `CmnClientParams.RenamedKeys`） | 綴りの誤り（2017 年の `OAuth2AndOidcIsser` からの名前） |
| `SpRp_JwkSetUri` | `JwkSetUri` | **読まない** | 相手（OP）の値なので、ほかの相手の値（`SpRp_*`）に揃えた |
| `SpRp_RequestObjectRegUri` | `RequestObjectRegUri` | **読まない** | 同上（Request Object を預ける口の URL。フレームワークは読まない） |
| `SpRp_AuthRequestPushUri` | `AuthRequestPushUri` | **読まない** | 同上（PAR の口の URL。フレームワークは読まない） |

**URL の 3 つは、改名前の名前を読まない。** 改名前の名前は、汎用認証サイトが**認可サーバ自身の口のパス**として使っている。
汎用認証サイトには、自分に送る自己テストのクライアントがあり、**1 つの環境にサーバとクライアントが共存しうる。**
読み替えると、クライアント役の値にサーバのパスが黙って入り、下の警告の関数も誤って検出する。

- **改名前の名前（`JwkSetUri` など）をクライアント役の値として書いていた配備は、新しい名前に書き換える。** 書き換えないと値は空になる
  （`JwkSetStore` は空の JWK Set を持ち、`kid` での検証は鍵が無く失敗する。通してしまう側には倒れない）
- **サーバの値は、アプリの設定で読む**（汎用認証サイトは `Config`。OpenTouryoProject/MultiPurposeAuthSite#339）
- **自己テスト**（同じ環境で、自分に送るクライアント）は例外として、サーバの値をそのまま使ってよい
- **本物のクライアント**（サンプルなど、別の配備で動くもの）は、`SpRp_*` の値を使う

`SpRp_Issuer` の読み方：

- **新しい名前を優先する。** 新しい名前が空（null・空文字）のときだけ、改名前の名前を読む
- **警告はフレームワークでは出さない。** アプリが起動時に次を呼び、ログに出す（汎用認証サイトの `Config.OldKeysStillUsed` と同じ分担）。どちらも名前だけを返し、値は返さない

  | 関数 | 返すもの |
  |---|---|
  | `CmnClientParams.OldKeysStillUsed()` | 改名前の名前だけが設定されているもの（動くが、書き換えを促す） |
  | `CmnClientParams.RenamedKeysInConflict()` | 新旧の両方に違う値があるもの（**使われるのは新しい名前の値**。書き間違いか、移行の途中） |

- **環境変数で上書きしている配備は、変数の名前も合わせる**（CONFIGURATION.md 4 節）。設定ファイルだけを新しい名前にすると、改名前の名前の環境変数は読まれなくなる（`RenamedKeysInConflict` に出る）
- Golden の設定（`Tests/Golden/TestCode`）は、改名前の名前のまま残している（改名前の名前を読めることの確かめを兼ねる）

**#634 の部品は、相手の値を引数で受ける版が基本である。** 設定を読む版（引数の少ない版）は、**相手が 1 つの配備**のための便宜で、
次の表のキーを読む。**キーが空なら何もせずに null（検証は `NoKey`）を返す**（通信もしない）。相手が複数なら、引数で受ける版を使う。

| 設定を読む版 | 読むキー |
|---|---|
| `RemoteJwkSetStore.GetJwkAsync(kid)` | `SpRp_JwkSetUri` |
| `RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid)` | `SpRp_DiscoveryUri`・`SpRp_Issuer` |
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
- **`AccessToken.Verify` の設定を読む版は、設定の `client_id`（`OAuth2AndOidcClientID` / `OAuth2AndOidcClientIDs`）が在れば、
  `aud` がそのいずれかを含むことを確かめる**（#655。`IdToken` の設定を読む版（#603）と同じ `VerifyAudByClientIDs`）。
  基盤の Web API（`MyBaseAsyncApiController`・`FxController`・`WCFTCPSvcForFx`）は、この版を使う。
  **リソース サーバは、受け付けるクライアントの `client_id` を `OAuth2AndOidcClientIDs` に並べる。**
  **どちらも設定されていなければ、従来どおり `aud` を確かめない**（壊さないため。**4.0 で、設定が無いときも断る既定に変える予定**）。
  2026/11/05 までは、設定によらず `aud` を確かめていなかった（Keycloak 互換。Keycloak は Audience mapper で `aud` を付ける）。
  **移行のときに当たりやすいこと：**
  - **`OAuth2AndOidcClientID`（単数。クライアントとして自分の `client_id` を持つキー）を設定しているだけでも、確かめるようになる。**
    クライアント兼 Web API の配備では、ほかのクライアント向けのトークンや、`aud` が自分の `client_id` でないトークン
    （リソース サーバの識別子が入る RFC 9068 の形式など）は 401 になる。受けるなら、その `client_id` を `OAuth2AndOidcClientIDs` に並べる
  - 基盤の `ServiceInterface`（ASPNETWebService・WCFService）の `app.config` には、既に `OAuth2AndOidcClientIDs` が入っている。
    **ここに並べた `client_id` 以外は断る**（サンプルの ASPNETWebService と同じ 5 件にした）
  - **空白・空文字だけの値は、設定が無いのと同じ**に扱う。**`OAuth2AndOidcClientIDs` が文字列の配列として読めなければ、すべて断る**（例外にしない。net48 は JSON が壊れているとき、net10.0 は要素がオブジェクト・配列のときと、配列でなく 1 つの値のとき。net10.0 の構成のバインドは、形が違っても例外にせず要素を飛ばすので、形を先に見る）
  - **`aud` は文字列か、文字列の配列に限る**（数値・オブジェクト・文字列でない要素は `false`）。`IdToken`・`ResponseObject` の
    設定を読む版も同じ判定（`VerifyAudByClientIDs`）なので、同じく変わる（以前は数値を文字列に変えて照合し、オブジェクトは例外だった）
  - **リソースごとの分離にはならない。** 判定は「並べた `client_id` のいずれか向けか」なので、同じクライアント向けのトークンは、
    そのクライアントを並べた別の Web API でも通る。分けるなら、リソース サーバの識別子を `aud` に入れる認可サーバと、`auds` を受ける版を使う
  - `typ` を見ないので、同じ発行者が出した ID トークン（`aud` が並べた `client_id`）も通る（下の `typ` の項）
  **`aud` を別の値で確かめるなら `jws` / `iss` / `auds`（期待する `aud` の集合）を受ける版を使う。**
  `aud` に `client_id` が入る形式では、受け付けるクライアントの `client_id` の一覧を渡す
- **`exp`・`nbf`・`iat`（NumericDate）は、小数・指数表記でも読む**（RFC 7519 2。#647）。
  読み方は `CmnJwtToken.TryReadNumericDate` の 1 か所（`decimal`・`CultureInfo.InvariantCulture`）。
  **小数はそのまま、いまの時刻（整数の秒）と比べる**ので、切り捨て・切り上げてから比べるのと同じ結果になり、有効期間は広がらない。
  **小数の double は 17 桁（`G17`）で字面にしてから読む**（net48 の既定の書式は 15 桁で、10 桁の秒だと小数が丸まり、
  `exp` が最大 1 秒長く通るため。net48 と net10.0 で結果を揃える）。
  **`long` の範囲を超える値は読めない**（以前の `long.Parse` が受けなかった範囲。極端に遠い未来の `exp` を通さない）。
  **`exp` と同じ秒は通す**（従来どおり。RFC 7519 4.1.4 の「`exp` 以後は受けない」より 1 秒緩い）。
  **数値の文字列も、従来どおり読む**（`CreateExpClaim` などが文字列を返すため。DPoP の `iat` だけは数値に限る）。
  **無い・読めない値は、例外にせず検証の失敗（`false`）**。2026/11/05 までは、`IdToken`・`AccessToken`・`JwtAssertion` の `Verify`
  と `CmnJwtToken.VerifyExp`・`VerifyNbf` が、`long.Parse` の例外を投げていた（`ResponseObject.Verify` は #644 で先に `false` にしていた）。
  **JWT のペイロード（JToken）を持つ呼び出し側は、公開した `TryReadNumericDate`・`VerifyExpClaim`・`VerifyNbfClaim` を使う**（#661）。
  文字列に直して `VerifyExp`・`VerifyNbf` に渡さない。`ToString()` はカルチャに依存し、たとえば de-DE では小数が `1000,5` になって読めない。
  `VerifyNbfClaim` は**無い `nbf` も `false`** なので、`nbf` を任意とするなら、先に null を見る。
- **外から来た要求のクレームを取り出すなら `CmnJwtToken.TryGetStringClaim` を使う**（#662）。無い・読めないクレームは
  `invalid_request`（CIBA Core 13）で返し、例外にしない。文字列・数値・真偽値は文字列で返し（数値はカルチャに依らない字面。
  CIBA の `requested_expiry` は文字列でも数値でも来る）、オブジェクト・配列・日付は読めないもの、**明示的な `null` は無いのと同じ**。
  **`CheckClaims` は `[Obsolete]`（警告）にした**（無いクレームを `server_error` で返すため）。振る舞いは、値がオブジェクト・配列のとき
  **例外（`ArgumentException`）にせず `false`（`server_error`）にした**ことだけを変えた。
  **日付の形の文字列に注意する。** 既定の `JObject.Parse` は `"2026-01-01T00:00:00Z"` のような文字列を日付に変え、元の字面を戻せないので、
  `TryGetStringClaim` は読めないもの（`invalid_request`）にする。**ペイロードは `DateParseHandling.None` で読んでおく**
- **`RequestObject.Verify`・`VerifyCiba` は、署名に加えて必須のクレームと `exp`・`nbf` を確かめる**（#653）。
  2026/11/05 までは、署名が正しければクレームによらず `true` を返していた（「必須項目の不足」の分岐も `true`）。
  **必須の範囲は仕様に合わせた。** `Verify` は `iss`・`aud`・`response_type`・`client_id`・`scope`（**`state`・`nonce` は必須にしない**。
  `Create` も空なら入れない）で、`exp`・`nbf` は在れば確かめる。`VerifyCiba` は `aud`・`iss`・`exp`・`iat`・`nbf`・`jti`・`scope`
  （CIBA Core 7.1.1・7.1）と、**ヒント（`login_hint_token`・`id_token_hint`・`login_hint`）がちょうど 1 つ**。
  **`client_notification_token`・`binding_message` は必須にしない**（Ping・Push のときだけ必須／任意）。
  **既存の版は `aud` の値を照合しない**（期待する値を受けないため）。**既存の版は `[Obsolete]`（警告。CS0618）にした**
  （`aud` を見なければ、同じクライアントが別の宛先に向けて署名したものも通るため。RFC 9101 10.8）。振る舞いは変えていない。
  **照合するなら、期待する `aud` を受ける版**（`Verify(ro, aud, out iss, key)`・`VerifyCiba(ro, aud, out iss, key)`）**を使う**（#653 ph2）。
  完全一致（大文字・小文字を区別）で、配列ならそれを含むこと。期待する値は、RFC 9101 4・CIBA Core 7.1.1 では
  **認可サーバ（OP）の issuer**（エンドポイントの URL ではない）。null・空を渡すと `ArgumentNullException`。
  **正規化はしない**（末尾のスラッシュの有無も別物）ので、**メタデータ（RFC 8414・Discovery）の `issuer` の文字列をそのまま渡す**。
  配列に文字列でない要素があれば、期待する値を含んでいても `false`（ph1 から同じ）
  **欠けている・読めない・期限の過ぎたものは `false`**（例外にしない）。受け入れる範囲が狭まるので、利用側は E2E で確かめる。
  **移行のときに当たりやすいのは次の 2 つ。** `Verify` で **`client_id` が新たに必須**になった（以前は見ていなかったので、
  `client_id` を入れない要求オブジェクトは断られる。`Create` は入れる）。**時計のずれは許さない**（`exp`・`nbf` の許容幅は 0。
  `IdToken` などと同じ）ので、CIBA ではクライアントの時計が進んでいると `nbf` で断られる。
  **日付の形の文字列のクレーム**（`"2026-11-05T00:00:00Z"` など）は、日付に変えずに文字列として読む（`DateParseHandling.None`）
- **`RequestObject.Create` で `exp`・`nbf` を入れるなら、有効期間（`TimeSpan`）を受ける版を使う**（#660）。
  `nbf`・`iat` に今の時刻（UTC）、`exp` に `nbf` + 有効期間を、**JSON の数値**で入れる（RFC 7519 2。`CreateCiba` は文字列）。
  FAPI（1.0 Advanced 5.2.3-11・14）はクライアントに `exp`・`nbf` を求めるので、**受けない版で作った要求オブジェクトは、
  FAPI の認可サーバに断られる。** 受けない版は従来どおりどれも入れない（振る舞いは変えていない）。
  **有効期間の上限は見ない**（FAPI の 60 分は呼び出し側が守る）。1 秒未満は `ArgumentOutOfRangeException`。
  **`nbf` は今の時刻ちょうどで、時計のずれの余裕を取らない**ので、認可サーバの時計が遅れていて許容幅なしで確かめる相手には、
  `nbf` で断られうる（この基盤の `Verify` も許容幅は 0）
- **`JwtAssertion` は、期待する `iss`・`aud` を受ける版**（`Verify(jwt, iss, auds, out sub, out scopes, out jobj, key)`・
  `VerifyByRsa`・`VerifyByECDsa`）**を使う**（RFC 7523 3。#654）。`iss` の一致、`sub` が在ること、`aud`、`exp`、在れば `nbf` を確かめる。
  **`aud` は唯一の値**（文字列か、要素 1 つの配列）で、それが `auds` のいずれかと一致すること（完全一致で正規化しない）。
  ほかの値も並べた `aud` は断る（別の用途の JWT を流用される攻撃（audience injection）への対策。draft-ietf-oauth-rfc7523bis 4。
  RFC 7523 3 の 3 は、自分を含めば受ける）。**`auds` には認可サーバの issuer だけを渡すのを推奨する**
  （トークン エンドポイントの URL などを並べるのは、それを `aud` にする旧いクライアントとの互換のためだけにする）。
  **`sub` が `client_id` と一致するかは見ない**（クライアント認証なら `client_id`、認可グラントなら利用者。2 の A・B）。
  **クライアント認証に使うなら、`out sub` が `iss` と一致することを、呼び出し側で必ず照合する**（照合しないと、別の `sub` の JWT が通る）。
  **`jti` の使い回しも見ない**（呼び出し側の台帳で行う）。**形の壊れた入力も、例外にせず `false`**（`RequestObject` も同じ。`CmnJwtToken.TryVerifySignature`）。
  **受けない版（`iss`・`aud` を `out` で返すだけ）は `[Obsolete]`（警告）にした**（振る舞いは変えていない。`aud` が配列だと例外になる）。
  **`Create` は `sub`（= `iss`）を入れる**ようにした（2026/11/05 まで入れていなかった。RFC 7523 3 の 2 で必須）
- **署名の alg を選ぶなら、検証鍵（`JWS`）を受ける版を使う**（#664）。`RequestObject.Verify(ro, aud, out iss, jws)`・
  `VerifyCiba(ro, aud, out iss, jws)`・`JwtAssertion.Verify(jwt, iss, auds, out sub, out scopes, out jobj, jws)`。
  ほかの版は、`RequestObject.Verify` が RS256、`VerifyCiba` が ES256、`JwtAssertion` が JWK の `kty` で RS256 か ES256 に決まる。
  FAPI 1.0 Advanced 8.6 は PS256 か ES256 を使うとする（RS256 は使わないほうがよい）。確認するクレームは、ほかの版と同じ。
  **ヘッダの `alg` が、渡した鍵の `alg`（`JWS.JWSHeader.alg`）と違えば `false`**（大文字・小文字を区別する。RFC 8725 3.1。
  `CmnJwtToken.TryVerifySignature` で見るので、照合しない `JWS` の派生を渡されても通さない）。
  **受ける `alg` は呼び出し側が決める**（クライアントの登録など）。**ヘッダの `alg` を読んで鍵を作ると、照合の意味が無くなる。**
  JWK からなら `JwsFactory.CreateFromJwk(alg, jwk)` で作る。鍵の型は制限しない（`client_secret_jwt` なら `JWS_HS256` など。OIDC Core 9）ので、
  **クライアントの登録どおりの型の鍵だけを渡す**（公開鍵の値を HS* の鍵に使うと、alg の取り違えになる。RFC 8725 3.1 : 鍵は 1 つの alg だけで使う）。
  **鍵の引数に `null` のリテラルを書くと、JWK（`string`）の版と区別できず、コンパイル エラーになる**（`(JWS)null` と書けば `ArgumentNullException`）
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
| `SAML2Client.VerifyResponse.cs` | 応答の検証（bool を返す従来の版）。**`[Obsolete]`（警告）**：Issuer・Audience・Recipient・InResponseTo を照合しないため、照合する `VerifyResponseWithReason` を使う。振る舞いは変えていない |
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
- **RSA-PSS・ECDSA も、RFC 9231 の URI で書く**（`sha256-rsa-MGF1`・`ecdsa-sha256` など。2.3.10・2.3.6。#643）。
  **決められないアルゴリズム（RSA-PSS の SHA1 など）は `ArgumentException`**。2026/10/30 までは、どちらも `rsa-sha1` と書いていた。
  **RSA-PSS・ECDSA で署名し、`DigitalSign` 版の `VerifyRedirect` で受けている相手とは、版を揃える**（古い版の `rsa-sha1` は照合で落ちる）

**`VerifyRedirect` は 2 つ在る**（#604）。

| 版 | 検証のしかた |
|---|---|
| `DigitalSign` を受ける | 渡された `DigitalSign` で検証し、**`SigAlg` がそのアルゴリズムと一致しなければ失敗する**（`SAML2Client.VerifyResponse` はこちら。ハッシュは `hashAlgorithmName`、既定は SHA256） |
| `RSA` を受ける | **許可リスト（`sigAlgs`）の中から `SigAlg` でハッシュとパディングを選ぶ**（既定は SHA256 / 384 / 512 で、SHA1 は受けない。**RSA-PSS は既定では受けず、`RSAPSSwithSHA256` などを並べたときだけ**。#643） |

- **署名の対象は、`SAMLRequest`（`SAMLResponse`）・`RelayState`（在れば）・`SigAlg` の 3 つだけから、受け取ったままの値で組み立てる**
  （どちらの版も。SAML 2.0 Bindings 3.4.4.1。#649）。**ほかのパラメタは無視し、並びも問わない**。
  2026/11/05 までは、クエリ文字列の全体から `Signature` だけを除いたものを署名の対象にしていたので、
  **SP 側の画面遷移が付けたパラメタ（Open棟梁 の Web Forms の `BrowserWindowGUID` など）があると、正しい署名でも失敗していた**。
  署名に関わるパラメタの重複、`SAMLRequest` と `SAMLResponse` の両方、`SigAlg`・`Signature` が無いものは失敗にする。
  **署名の検証と SAML の取り出し（`DecodeRedirect`）、理由を返す版の `SigAlg` は、同じ読み方（`TryReadRedirectParameters`）で読む**
  （署名を確かめた値と、実際に使う値を一致させるため。QUALITY.md 4 節）。パラメタ名は完全一致で見て、
  **URL エンコードされた名前で署名に関わるパラメタを名乗るものは、曖昧なので読まない**。
  **読み方を足すときも、この関数を通すこと**（`UriOperator.GetParameterFromQueryString` は名前を部分一致で探すので、ここでは使わない）。
  **`DecodeRedirect` は、読めないもの**（`SAMLRequest` と `SAMLResponse` の両方・どちらも無い、重複、エンコードされた名前）**に空文字を返す**
  （2026/11/05 までは、名前を部分一致で探し、両方あれば `SAMLRequest` を、重複があれば最初の値を返していた）。
  **`RelayState` の値が無ければ、パラメタ全体を署名の演算から除外する**（3.4.4.1。空のパラメタとして含めてはならない。
  送る側の `EncodeAndSignRedirect` も付けない）。ただし、**仕様に反して空の `RelayState=` を含めて署名する相手との互換のため、
  含める版でも照合する**（どちらかで通れば受ける。どちらも全体に署名が掛かるので、偽造は通らない）
  **送る側（Web Forms）も、外部の IdP への遷移は基盤の画面遷移（`FxRedirect`）を通さない**（ウィンドウ GUID を付け足すため。サンプルの `login.aspx` の `RedirectToIdP`）
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
