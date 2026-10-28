# COVERAGE.md — NUnit のカバレッジ

対象: `root/programs/CS/Frameworks/Tests/NUnit`（C# 側の NUnit テスト）

本書の表は、**`2_RunAllTests.ps1 -Coverage` が書く**（[`CoverageReport.ps1`](../../../CoverageReport.ps1)）。
**`COVERAGE:BEGIN` から `COVERAGE:END` の間は手で直さない。** 次の `-Coverage` で上書きされる。

> **カバレッジは判定に使わない。** どこを書き足すかを選ぶための指針である（[`README.md`](README.md)「カバレッジを見て対象を選ぶ」）。
> **Golden のテストが通している行は数えていない。** 低い数字は「NUnit で確かめていない」という意味である。

## 更新のしかた

```powershell
cd root\programs
.\2_RunAllTests.ps1 -Coverage -LocalServicesDir <LocalServicesOnDocker のフォルダ>
```

1. NUnit の 4 プロジェクト（`TestPublic`・`TestPublicNLog`・`TestPublicDb`・`TestFramework`）を 1 回のバッチで回して判定する
2. `-Linux` と同じく、`TestPublic` を Linux のコンテナで回す（**docker が要る**）
3. `-CrossDb` と同じく、Oracle → MySQL → PostgreSQL を **1 つずつ**「起動 → `TestPublicDb` → 停止」する
4. **全部 OK なら**、この実行で作られた net10.0 のカバレッジを合算して、表を丸ごと置き換える

- **普通の実行（`-Coverage` なし）は、本書を書き換えない。** カバレッジは画面に表示するだけ
- **1 つでも NG があれば書かない**（前の値のまま）。`-Only` / `-SkipBuild` / `-Linux` / `-CrossDb` とは同時に指定できない
- DB のコンテナは LocalServicesOnDocker の CHEATSHEET に従って扱われる（[`TESTING.md`](../../../TESTING.md) の冒頭）
- 本書が書き換わるのは `Result*.txt` と同じ副作用である。**コミットするかは人が決める**

## 読み方

- **合算**：アセンブリごとに、**どれかの計測で 1 回でも通った行**を数える（行の和集合）。
  たとえば `OpenTouryo.Public` には、`TestPublic` で通った行に加えて、
  `TestPublicNLog`（NLog の経路）と `TestPublicDb`（`BaseDam`・`DamSqlSvr` など、各 DBMS の回を含む）で通った行も入る。
  Dam（`DamManagedOdp` / `DamMySQL` / `DamPstGrS`）には、その DBMS を起動した回で通った行が入る。
  **Linux の回の行も入る**（`DigitalSignECDsaOpenSsl` など、OpenSSL を使う Linux 専用のコード）
- **`OpenTouryo.Framework` は、アセンブリ全体ではなく、`TestFramework` の対象モジュールごとの行と、その計の行で出す。**
  対象は単体で使える部品（`Authentication`・`Exceptions`）だけで、親クラス（`Business`・`Dao`・`Presentation`・`Transmission`）は
  組み合わせて使うもので、NUnit では通さない（疎通でカバレッジを取る案は別件）。全体の割合を出しても低く出るだけで、指針にならない。
  対象モジュールの一覧は `2_RunAllTests.ps1` の TestFramework の定義（`CoverageModules`）が持つ。
  **`CmnJwtToken` は、設定（`CmnClientParams` 等）を読む版を対象にしていない**ので、その分だけ低く出る。
  partial に分けたクラス（`SAML2Bindings`・`SAML2Client`・`OAuth2AndOIDCClient`）は、ファイルごとの行で出す（#634）。
  **`SAML2Bindings.cs` の XSD による検証（`VerifyByXsd`）は未実装の雛形**（`XXXX.xsd`）で、通していない
- **行**：通った行 / 全体の行。行は (ファイル, 行番号) で重複を除いて数える
- **分岐（下限）**：分岐のある行ごとに、**最も多くの分岐を通した計測**の数を採る。
  どの分岐を通ったかまではカバレッジのファイルに無いため、計測をまたいだ和集合は取れない。
  **実際の値はこれ以上**である
- **net10.0 だけ**を数える（net48 の回はカバレッジを取らない）。Linux の回も net10.0 である
- **コミット**：計測したときの HEAD。ワーキング ツリーに未コミットの変更があれば `+` が付く
  （テストの副産物の `Result*.txt` と、本書の変更は数えない）

クラス・行の単位の詳細は、HTML の [`CoverageHtml/index.html`](./CoverageHtml/index.html) で見る。

- `-Coverage` の回に、表と同じカバレッジ ファイルから ReportGenerator が作る（`dotnet tool install -g dotnet-reportgenerator-globaltool` が要る。無ければ作らない）
- **Git 管理外**（`.gitignore`）なので、リンクは手元で `-Coverage` を回した後にだけ開ける
- 合算は ReportGenerator が行うため、**割合は上の表と一致しないことがある**（Framework を対象モジュールに絞らない、分岐の数え方が違う）

元のデータは、各計測の `coverage.cobertura.xml`（`NUnit\TestResults` の下。`2_RunAllTests.ps1` が場所を表示する）である。

<!-- COVERAGE:BEGIN -->

- 計測した日時 : 2026/10/29 02:16
- コミット : cf7da234
- 合算した計測（8 件） : NUnit TestPublic (net10.0) / NUnit TestPublicNLog (net10.0) / NUnit TestPublicDb (net10.0) / NUnit TestFramework (net10.0) / NUnit TestPublic (Linux) / TestPublicDb (net10.0) [oracle] / TestPublicDb (net10.0) [mysql] / TestPublicDb (net10.0) [postgres]

| アセンブリ（: モジュール） | 行 | 行数（通過／全体） | 分岐（下限） | 分岐数（通過／全体） |
|---|---:|---:|---:|---:|
| OpenTouryo.DamManagedOdp | 89.1% | 261 / 293 | 83.3% | 55 / 66 |
| OpenTouryo.DamMySQL | 89.1% | 261 / 293 | 83.3% | 55 / 66 |
| OpenTouryo.DamPstGrS | 89.0% | 259 / 291 | 83.3% | 55 / 66 |
| OpenTouryo.Framework : Authentication/AuthenticationHeader.cs | 99.4% | 157 / 158 | 95.3% | 61 / 64 |
| OpenTouryo.Framework : Authentication/SAML2Enum.cs | 100.0% | 142 / 142 | 100.0% | 56 / 56 |
| OpenTouryo.Framework : Authentication/SAML2Bindings.cs | 88.6% | 101 / 114 | 56.2% | 36 / 64 |
| OpenTouryo.Framework : Authentication/SAML2Bindings.Encoding.cs | 99.2% | 129 / 130 | 98.0% | 49 / 50 |
| OpenTouryo.Framework : Authentication/SAML2Bindings.SP.cs | 100.0% | 117 / 117 | 68.2% | 15 / 22 |
| OpenTouryo.Framework : Authentication/SAML2Bindings.IdP.cs | 100.0% | 162 / 162 | 96.9% | 31 / 32 |
| OpenTouryo.Framework : Authentication/SAML2Client.cs | 100.0% | 33 / 33 | 83.3% | 5 / 6 |
| OpenTouryo.Framework : Authentication/SAML2Client.VerifyResponse.cs | 100.0% | 121 / 121 | 92.9% | 26 / 28 |
| OpenTouryo.Framework : Authentication/SAML2Client.VerifyResponseWithReason.cs | 97.0% | 196 / 202 | 83.7% | 82 / 98 |
| OpenTouryo.Framework : Authentication/SAML2Client.VerifyRequest.cs | 95.2% | 59 / 62 | 90.9% | 20 / 22 |
| OpenTouryo.Framework : Authentication/SAML2Client.CreateResponse.cs | 100.0% | 55 / 55 | 92.9% | 13 / 14 |
| OpenTouryo.Framework : Authentication/SAML2VerifyResult.cs | 100.0% | 40 / 40 | - | 0 / 0 |
| OpenTouryo.Framework : Authentication/SAML2Metadata.cs | 94.6% | 141 / 149 | 92.3% | 48 / 52 |
| OpenTouryo.Framework : Authentication/SAML2Attributes.cs | 97.1% | 132 / 136 | 92.9% | 52 / 56 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.cs | 100.0% | 65 / 65 | 100.0% | 16 / 16 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.Grants.cs | 100.0% | 120 / 120 | 100.0% | 16 / 16 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.TokenManagement.cs | 100.0% | 91 / 91 | 100.0% | 8 / 8 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.ExtensionGrants.cs | 100.0% | 139 / 139 | 100.0% | 4 / 4 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.AuthZRequest.cs | 100.0% | 46 / 46 | - | 0 / 0 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.OpenIDConnect.cs | 100.0% | 28 / 28 | 100.0% | 6 / 6 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.ClientRegistration.cs | 100.0% | 33 / 33 | 100.0% | 4 / 4 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCClient.DPoP.cs | 99.1% | 106 / 107 | 90.0% | 27 / 30 |
| OpenTouryo.Framework : Authentication/OAuth2AndOIDCSpec.cs | 100.0% | 138 / 138 | 97.4% | 76 / 78 |
| OpenTouryo.Framework : Authentication/DPoPProof.cs | 95.0% | 170 / 179 | 93.6% | 73 / 78 |
| OpenTouryo.Framework : Authentication/RemoteJwkSetStore.cs | 99.0% | 193 / 195 | 93.9% | 62 / 66 |
| OpenTouryo.Framework : Authentication/JwkSetDocument.cs | 93.9% | 46 / 49 | 85.7% | 12 / 14 |
| OpenTouryo.Framework : Authentication/CmnClientParams.cs | 100.0% | 51 / 51 | - | 0 / 0 |
| OpenTouryo.Framework : Authentication/CmnServerParams.cs | 100.0% | 44 / 44 | 100.0% | 12 / 12 |
| OpenTouryo.Framework : Authentication/CmnJwtToken.cs | 32.4% | 56 / 173 | 26.0% | 13 / 50 |
| OpenTouryo.Framework : Authentication/AccessToken.cs | 86.3% | 63 / 73 | 80.8% | 21 / 26 |
| OpenTouryo.Framework : Authentication/IdToken.cs | 92.2% | 83 / 90 | 97.1% | 33 / 34 |
| OpenTouryo.Framework : Authentication/ResponseObject.cs | 92.5% | 86 / 93 | 96.4% | 27 / 28 |
| OpenTouryo.Framework : Authentication/JwtAssertion.cs | 97.7% | 128 / 131 | 90.0% | 18 / 20 |
| OpenTouryo.Framework : Authentication/RequestObject.cs | 96.5% | 137 / 142 | 63.5% | 33 / 52 |
| OpenTouryo.Framework : Authentication/ClaimsInRO.cs | 100.0% | 34 / 34 | 85.7% | 12 / 14 |
| OpenTouryo.Framework : Exceptions/BusinessApplicationException.cs | 100.0% | 16 / 16 | - | 0 / 0 |
| OpenTouryo.Framework : Exceptions/BusinessSystemException.cs | 100.0% | 11 / 11 | - | 0 / 0 |
| OpenTouryo.Framework : Exceptions/FrameworkException.cs | 100.0% | 11 / 11 | - | 0 / 0 |
| **OpenTouryo.Framework（対象モジュールの計）** | 94.6% | 3,480 / 3,680 | 86.3% | 967 / 1,120 |
| OpenTouryo.Public | 81.3% | 6,267 / 7,711 | 69.8% | 1,485 / 2,128 |
| OpenTouryo.Public.Security | 93.3% | 3,303 / 3,542 | 85.0% | 600 / 706 |

<!-- COVERAGE:END -->
