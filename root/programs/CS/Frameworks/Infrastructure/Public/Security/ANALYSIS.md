# ANALYSIS.md — 暗号・署名・JWT（Infrastructure/Public/Security）

対象: `root/programs/CS/Frameworks/Infrastructure/Public/Security` / 最終更新: 2026-10-16

上位: [`Public/ANALYSIS.md`](../ANALYSIS.md) / [`Infrastructure/ANALYSIS.md`](../../ANALYSIS.md) / 全体: [`Frameworks/ANALYSIS.md`](../../../ANALYSIS.md)

---

## 1. これは何か

`OpenTouryo.Public.Security`（名前空間 `Touryo.Infrastructure.Public.Security`）。
**.NET の暗号 API を、アルゴリズムを列挙型で選べる形に包んだもの**と、その上の JWT（JWS / JWE / JWK）である。
`Framework` の認証（OAuth2 / OIDC / SAML2。[`Framework/ANALYSIS.md`](../../Framework/ANALYSIS.md)）はこれに乗っている。

フォルダは `Public/` の下だが、**別のアセンブリ**である（`Public_netcore100.csproj` は `Security/**` を外している）。

---

## 2. 構成

| フォルダ・ファイル | 主なクラス | 一言で |
|---|---|---|
| 直下（ハッシュ） | `GetHash` `GetKeyedHash` `MsgAuthCode` `HashAlgorithmCmnFunc` | ハッシュ・HMAC。`EnumHashAlgorithm` / `EnumKeyedHashAlgorithm` で選ぶ |
| 直下（暗号） | `SymmetricCryptography` `ASymmetricCryptography` `AsymmetricAlgorithmCmnFunc` | 共通鍵（パスワードから鍵を導出）・公開鍵の暗号化 |
| 直下（署名） | `DigitalSign`（抽象）→ `DigitalSignParam` / `DigitalSignX509` / `DigitalSignXML` / `DigitalSignECDsaCng` / `DigitalSignECDsaX509` / `DigitalSignECDsaOpenSsl` | 鍵の持ち方（パラメタ・証明書・XML 形式の鍵・CNG）ごとの実装 |
| 直下（その他） | `PrivacyEnhancedMail` `IdentityImpersonation` `MyDebug` | PEM の読み書き、なりすまし（net48 のみ）、`Diagnostics.MyDebug` の派生 |
| `Aead/` | `AuthEncrypt` → `AeadAesCbc` / `AeadAesGcm` → `AeadA128CbcHS256` … `AeadA256Gcm` | 認証付き暗号（JWE の内容暗号化に使う） |
| `Jwt/` | `JWS` / `JWE` / `JwkSet`、鍵コンバータ（`RsaPublicKeyConverter` など）、`JwtConst` | 4 節 |
| `KeyExg/` | `BaseKeyExchange` → `RsaKeyExchange`（`RsaOaep*` / `RsaPkcs1*`）/ `EcdhKeyExchange`（`EcdhCng*`） | 鍵交換（Alice / Bob の対） |
| `Pwd/` | `GetPassword` `GetPasswordHashV1` `GetPasswordHashV2` | パスワードの生成と、ソルト付きハッシュ（5 節） |
| `Xml/` | `SignedXml2` | XML 署名（SAML2 が使う） |

---

## 3. TFM 差

| | net48 | net10.0 |
|---|---|---|
| 条件コンパイル | `NET48` | **`NETSTD` を明示的に定義**（`#if NETSTD` が約 80 箇所） |
| 外しているもの | `DigitalSignECDsaOpenSsl.cs`（列挙に無い） | `IdentityImpersonation.cs` / `KeyExg/EcdhCngAlice.cs` / `EcdhCngBob.cs` / `EcdhKeyExchange.cs`（`Compile Remove`） |
| XML 署名 | BCL の `System.Security.dll` | `System.Security.Cryptography.Xml`（パッケージ） |
| 依存 | `BouncyCastle.NetCore` / `Newtonsoft.Json` | 同左 ＋ `System.Security.Cryptography.Xml` |

**このプロジェクトでは `#if NETSTD` が正しく効く**（明示的に定義しているため）。
既存のコードは `#if NETSTD` で書かれているので、**周りに合わせてよい**。新しく書くなら `#if NETCOREAPP` でも同じ意味になる
（[`Frameworks/ANALYSIS.md`](../../../ANALYSIS.md) 6.2 節）。

### 3.1 ECDSA は OS で実装が分かれる

`DigitalSignECDsaCng` は **Windows の CNG**（`ECDsaCng`）で、Windows 以外では `NotImplementedException` を投げる。
net10.0 の `JWS_ES*_Param` は、**Windows なら CNG、それ以外なら `DigitalSignECDsaOpenSsl`** を使う。

```
JWS_ES256_Param（など）
  Windows        → DigitalSignECDsaCng
  Windows 以外   → DigitalSignECDsaOpenSsl（net10.0 のみ。net48 では NotImplementedException）
```

**同じテストの期待値が、Windows と Linux で違っていたことがある**（`ES384` / `ES512` の `Verify`。#595）。
`DigitalSignECDsaCng` がハッシュを受け取らず SHA-256 に固定されていたためで、修正済み。
**`Verify` の期待値が `False` なら、仕様か不具合かを確かめること。**

---

## 4. JWT

### 4.1 JWS（署名）

```
JWS（抽象。JWSHeader / Create / Verify / Verify(jwt, typ)）
  ├ JWS_HMACSHA → JWS_HS256 / HS384 / HS512
  ├ JWS_RSA     → JWS_RS256 / RS384 / RS512 / PS256 / PS384 / PS512
  │                 └ それぞれ _Param（RSAParameters） / _X509（証明書）、RS は _XML も
  └ JWS_ECDSA   → JWS_ES256 / ES384 / ES512
                    └ それぞれ _Param（ECParameters） / _X509
```

- **ヘッダ（`JWSHeader`）は基底の `JWS` が持つ**（#615）。`alg` は方式で決まるので、派生の `Init` で作る
- **`Verify(jwt)` は `typ` を照合しない。** 照合するなら `Verify(jwt, typ)`（#611）。
  照合は RFC 7515 4.1.9 の規則（大文字小文字を区別しない、`/` が無ければ `application/` を補う）。
  **ヘッダに `typ` が無ければ不一致**
- RFC 9068 の access_token を検証するリソース サーバは `JwtConst.AT_JWT` で照合する

### 4.2 JWE（暗号化）

```
JWE（抽象。JWEHeader / Create / Decrypt / Decrypt(jwt, typ, out payload)）
  ├ JWE_Rsa15A128CbcHS256 → _Param / _X509    （RSA1_5 + A128CBC-HS256）
  └ JWE_RsaOaepAesGcm     → _Param / _X509    （RSA-OAEP + AES-GCM）
```

- `JWEHeader` は `public get / protected set`。`typ` は設定できる（#615）
- **`Decrypt(jwt, out payload)` は `typ` を照合しない。** 照合するなら `typ` を受ける版。規則は JWS と同じ（`JWS.GetHeaderTyp` / `NormalizeTyp` を共有している）

### 4.3 JWK

- `JwkSet` … JWK Set（`keys` の配列）の読み書き
- `RsaPublicKeyConverter` / `RsaPrivateKeyConverter` / `EccPublicKeyConverter` / `EccPrivateKeyConverter` …
  JWK と `RSAParameters` / `ECParameters` / 証明書の相互変換
- `JwkOperator`（#634）… **任意の JWK の RFC 7638 の指紋**（`Thumbprint`。既定は SHA-256）、公開鍵だけか（`IsPublicOnly`）、秘密鍵の項目を落とす（`ToPublic`）。
  **変換器が作る `kid` はハッシュが変換器に連動する**（`RS384` なら SHA-384）ので、DPoP の `jkt`（常に SHA-256）に `kid` を流用しないこと

### 4.4 alg から作る・検証せずに読む（#634）

- `JwsFactory` … **alg の文字列から JWS を作る**（JWK・パラメタ・証明書・秘密鍵の JWK）。扱うのは `RS*` / `PS*` / `ES*` の 9 つで、`HS*` と `none` は扱わない。
  **`IsSameKeyType` で alg と鍵の種類（EC は `crv` まで）を照合してから作る**（アルゴリズムの取り違えの防止）。読めない JWK は `null`
- `JwtReader` … **検証の前に**ヘッダ・ペイロード・`alg` / `kid` / `typ` を読む。読めなければ `null`（例外にしない）。読んだ値を検証の代わりにしない
- `JWS_Header.jwk`（`JToken`）… DPoP の証明がヘッダに公開鍵を持つため。既定の `null` ではヘッダに出ない（既存の JWS は変わらない）
- `JwsFactory.CreateFromPfx` の鍵の格納先は `JWS_*_X509` の既定（`MachineKeySet` を含む）。管理者でない環境の試験では、フラグを渡して直に作る

---

## 5. パスワード

| クラス | 形式 | 備考 |
|---|---|---|
| `GetPasswordHashV1` | 固定のソルト（`Salt` プロパティ）とストレッチ回数を**静的に**持つ | 旧形式 |
| `GetPasswordHashV2` | ソルトとストレッチ回数を**結果の文字列に含める**（`.` 区切り）。`EnumHashAlgorithm` / `EnumKeyedHashAlgorithm` を選べる | 現行 |

**V1 と V2 は、同じ入力でも結果が違う。これは仕様である。**
V2 はハッシュの生成を一般化したもので、**V1 で作ったハッシュを V2 で照合すると `False` になる**
（`EncAndDecUtilCUI` の `GetPasswordHashV2.EqualSaltedPassword (old) > False` は期待どおり）。

---

## 6. 共通鍵暗号の鍵の導出（`SymmetricCryptography`）

**パスワードから共有キーと初期化ベクタを導出する方式を変えると、既存の暗号文を復号できなくなる。**

- 方式は **PBKDF2-HMAC-SHA1**（`Rfc2898DeriveBytes` の従来の既定）。**共有キー → 初期化ベクタの順に、同じ導出から続けて取り出す**
- 2025/08/01 にハッシュを受ける版を足した際、7 引数版が自分自身を呼ぶ形になり、
  `EncryptString` / `DecryptString` が **`StackOverflowException` でプロセスごと落ちていた**（`catch` できない。#608 で修正）
- 同じとき net10.0 側は 2 回に分けて導出しており、**初期化ベクタが共有キーの先頭と同じ**になっていた（net48 とも暗号文が食い違う）。#608 で従来の方式に戻した

**オーバーロードへ委譲するときに引数を落とすと、自分自身を呼んでもコンパイルが通る**（警告も出ない）。

---

## 7. 依存パッケージ

| パッケージ | 使っている所 |
|---|---|
| `BouncyCastle.NetCore` | `Aead/`（AES-GCM / AES-CBC）、ハッシュの一部（`GetHash` / `GetKeyedHash` / `HashAlgorithmCmnFunc`）、`GetPasswordHashV1` |
| `Newtonsoft.Json` | JWT のヘッダ・ペイロード、JWK |
| `System.Security.Cryptography.Xml`（net10.0 のみ） | `SignedXml2`（XML 署名。`Framework` の `SAML2Bindings` が使う） |

**`System.Security.Cryptography.Xml` は、参照している 4 箇所を同じ版で揃える**
（`Public.Security_netcore100.csproj` / `Business_netcore100.csproj` / `EncAndDecUtilCUICore.csproj` / `NuGet/Symbol_Public.Security.nuspec`）。
Dependabot は 1 プロジェクトずつしか上げないので、取り込んだら他も揃える（経緯は [`Frameworks/ANALYSIS.md`](../../../ANALYSIS.md) 12 節）。
net48 はこのパッケージを参照しない（BCL を使う）。

`jose-jwt` は**このアセンブリの依存ではない**。`Tests/Golden/EncAndDecUtilCUI` が、
相互運用の確認相手として参照している。

---

## 8. テスト

| 置き場 | 確かめること |
|---|---|
| `Tests/Golden/EncAndDecUtilCUI` | ほぼ全部の API を net48 / net10.0（Windows・Linux）で回し、結果ファイルと比べる。`jose-jwt` との相互運用も見る |
| `Tests/NUnit/TestPublic/Security/` | 単体テスト（カバレッジを意識）。ハッシュ・HMAC（RFC のテスト ベクタ）、共通鍵暗号（鍵の導出を .NET の API で組み立てて一致を見る）、パスワード、署名（パラメタ・XML・証明書・CNG）、公開鍵暗号、JWK（RFC 7638 の Thumbprint）、JWS、PEM、鍵交換、XML 署名。`JwsTypTests` / `JweTypTests` は `typ` の照合（#611）。**CNG を使うものは Windows だけで回す**（`[Platform("Win")]`） |
| `Tests/Else/EncAndDecUtil` | GUI での確認 |

**結果ファイルは 3 つある**（`Result48.txt` / `ResultCore100.txt` / `ResultCore100OnLinux.txt`）。
**OS で期待値が違うなら、それ自体を疑う**（3.1 節）。

---

## 9. 落とし穴

1. **ECDSA の CNG は Windows だけ**（3.1 節）。Linux では `_Param` の OpenSSL 版に分かれる。net48 には OpenSSL 版が無い。
2. **`Verify(jwt)` / `Decrypt(jwt, out ...)` は `typ` を見ない**（4 節）。見る版を使い分ける。
3. **V1 と V2 のパスワード ハッシュは互換でない。仕様**（5 節）。
4. **鍵の導出を変えると既存の暗号文が読めない**（6 節）。
5. **委譲で引数を落としても、自分自身を呼んでコンパイルが通る**（6 節）。呼ばれないコードは結果ファイルにも現れない。
6. **`System.Security.Cryptography.Xml` は 4 箇所を揃える**（7 節）。
7. **net10.0（Windows）の `X509Certificate2.GetDSAPublicKey()` は、2048 ビットの DSA 証明書を読めない**（「プロバイダーの公開キーは無効です」）。
   `X509Certificate2.PublicKey.GetDSAPublicKey()` なら読めるので、`AsymmetricAlgorithmCmnFunc.GetPublicKey` はこちらを使う。
   net48 の DSACng の秘密鍵は平文でエクスポートできないので、`DigitalSignX509` は写し直せなければそのまま使う（#626）。
8. **`RSACng.Create()` / `RSACryptoServiceProvider.Create()` は親の `RSA.Create()` で、派生クラスの名前で呼んでも既定の実装を返す。**
   `EnumASymmetricAlgorithm.RsaCng` は `new RSACng()` で作る（Windows のみ。Windows 以外は既定の実装）。
   **`RsaCsp` は `RSA.Create()` のまま**：net48 では RSACryptoServiceProvider になり、.NET ではその環境の既定の実装（Windows は CNG、Linux は OpenSSL）に読み替える。
   .NET の `RSACryptoServiceProvider` は OAEP-SHA256 などを持たないので、寄せると動いている呼び出しが例外になる（#628）。

---

## 10. 作業チェックリスト

- [ ] net10.0 は `NETSTD` が定義されている。既存の書き方（`#if NETSTD`）に合わせてよい
- [ ] 鍵の導出・ハッシュの既定・出力の形式を変えない（既存のデータが読めなくなる）。変えるなら新しいオーバーロードか新しいクラスにする
- [ ] Windows と Linux で振る舞いが分かれる箇所（CNG / OpenSSL）を両方考える
- [ ] `EncAndDecUtilCUI` の結果ファイル 3 つの差分を見る。`False` の期待値は仕様か確かめる
- [ ] パッケージの版を変えたら 4 箇所（7 節）と nuspec を揃える
- [ ] 未修正の弱点を見つけたら、**文書に書かず先に報告する**（`AGENTS.md`「セキュリティの指摘」）
