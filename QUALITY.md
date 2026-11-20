# 品質の羅針盤（QUALITY.md）

**どこを・何で・どれだけ確かめるか**を、Open棟梁 について決めた文書。人が決め、エージェントが従う。

- **共通の決まりは [`QUALITY.common.md`](QUALITY.common.md)**（OpenTouryoCodingAgentAssets の配布物。ここでは直さない）。**節の番号は同じ**で、本書には各節の Open棟梁 の中身だけを書く
- **エージェントは、変更の前に本書と `QUALITY.common.md` を読む**（[`AGENTS.md`](AGENTS.md) から参照し、`CLAUDE.md` から読み込んでいる）
- 方針の経緯と、汎用認証サイトとの共通部分は Wiki の
  [品質の羅針盤（方針案）](https://github.com/NetDevInfraWGinOSSConsortium/NetDevInfraWiki/wiki/OTR_QualityCompass)。
  導入は [#640](https://github.com/OpenTouryoProject/OpenTouryo/issues/640)
  （汎用認証サイトは [MultiPurposeAuthSite#315](https://github.com/OpenTouryoProject/MultiPurposeAuthSite/issues/315)）。
  共通と固有に分けたのは OpenTouryoCodingAgentAssets#21

> **人は個々の変更を細かく見るより、羅針盤を決める役を担う。**
> エージェントは、作業がどの名前空間に当たるかを本書で引き、その重みで確かめ、報告する。

## このリポジトリの値

`QUALITY.common.md` の「このリポジトリの値」は、この表を指す。

| 項目 | 値 |
|---|---|
| 検出器の置き場所 | `root/programs`（`CheckExpectationChange.ps1`・`rules.common.psd1`・`rules.psd1`） |
| 判定の用例の置き場所 | `root/files/tools/ExpectationGuard.Tests`（製品のソースの隣を避けて、道具の置き場所に置く） |
| 入れ直しのコマンド | `<アセット>\install\install.ps1 -TargetRoot <このリポジトリ> -IncludeQualityGuard -QualityGuardPath root/programs -QualityGuardTestsPath root/files/tools/ExpectationGuard.Tests`（`-Product` は付けない。`AGENTS.md`・`CLAUDE.md` はこのリポジトリが持つ） |
| 宛先のブランチ（段 3 で検査する PR の宛先） | `develop`・`master`（[`expectation-change.yml`](.github/workflows/expectation-change.yml)） |
| 必須のチェック | `expectation-change`（`develop`・`master`。[`GitHubUsage.md`](GitHubUsage.md) 2 節） |
| 共有の作業ブランチ | `5th_agent` など（ほかの人・セッションも push する。メッセージを書き直さない） |
| 変更のレビュー役（4 節） | [`quality-reviewer`](.claude/agents/quality-reviewer.md)（`model: sonnet`。#680） |
| 回帰テストの名前 | テスト名・コメントに Issue の番号 |

### 一次情報

**本書には方針と表だけを置く。** 手順は各文書が一次情報である。

| 内容 | 一次情報 |
|---|---|
| 検証の手順・回す範囲 | [`10_CHEATSHEET.md`](root/programs/10_CHEATSHEET.md) 1 節、[`14_TESTING.md`](root/programs/14_TESTING.md) |
| コーディング規約 | [`11_CODING.md`](root/programs/11_CODING.md) |
| カバレッジの値 | [`Tests/COVERAGE.md`](root/programs/CS/Frameworks/Tests/COVERAGE.md) |
| セキュリティの指摘の扱い | [`AGENTS.md`](AGENTS.md)「セキュリティの指摘は、公開される前に確認を取る」、[`Security.ja.md`](Security.ja.md) |

---

## 1. 品質マップ

**単位**：名前空間。接頭辞 `Touryo.Infrastructure.` は省略する（フォルダは `root/programs/CS/Frameworks/Infrastructure/` の下）。

| 名前空間 | リスク | 正の根拠 | 検証の手段 | 人が必ず見る箇所 |
|---|---|---|---|---|
| `Public.Security`（`.Aead`・`.Jwt`・`.KeyExg`・`.Pwd`・`.Xml` を含む） | **高** | RFC（JWS・JWE・JWK）、XML 署名（W3C）、NIST の推奨 | NUnit、Golden（net48・net10.0・Linux）、CodeQL | 既定値（アルゴリズム・鍵長）の変更、暗号文の互換性、署名の検証の対象 |
| `Framework.Authentication` | **高** | OAuth2 / OIDC・SAML2 の仕様 | NUnit、汎用認証サイトの E2E | 検証を省く分岐、後方互換性（パラメタ・既定） |
| `Public.Db` | **中〜高** | 各 DBMS の文法（リテラル・エスケープ） | NUnit、クロス DB（Oracle・MySQL・PostgreSQL） | リテラル・識別子の組み立て（SQL インジェクション） |
| `Public.Str`（`CustomEncode`）・`Public.Util`（`RandomValueGenerator`） | **中〜高** | エンコードの仕様（HTML・URL・Base64）、乱数 | NUnit | エンコードする文字の範囲、乱数の生成元 |
| `Framework.Business`・`.Dao`・`.Presentation`・`.Transmission`・`.Exceptions`、`ServiceInterface` | 中 | 方式の設計書・各 `ANALYSIS.md` | 疎通（Golden のサンプル） | 方式の変更（例外・トランザクション・ログ） |
| `Business.*`（親クラス 2） | 中 | 方式の設計書 | 疎通 | 利用者がカスタマイズする箇所の互換性 |
| `Public.Log`・`.IO`・`.Str`・`.Dto`・`.Util`・`.Xml` ほか（上の行に挙げたクラスを除く） | 中〜低 | — | NUnit | 例外の型・既定値の変更（後方互換性） |
| `CustomControl`・`Framework.RichClient` など、UI 寄りの部品 | 低 | — | 疎通 | 原則として見ない |
| ツール・サンプル（`Frameworks/Tools`・`Samples`・`Samples4NetCore`） | 低 | — | 疎通 | 原則として見ない（使い捨てが効く） |

- 検証の手段の具体（どのスクリプトを、どの引数で回すか）は [`10_CHEATSHEET.md`](root/programs/10_CHEATSHEET.md) 1 節の対応表による

---

## 2. 正の定義

- **根拠**：仕様の節（RFC の節番号など）か、Issue の番号。リスクが「高」の名前空間（`Public.Security`・`Framework.Authentication`）から入れる。書き方は 7 節
- **観測の扱い：Open棟梁 の観測は落ちる**（汎用認証サイトの `TESTCASES.md` と同じく検証と観測を分けるが、扱いが違う）。
  汎用認証サイトの E2E は観測を判定しないが、Open棟梁 は部品のライブラリで、**振る舞いが変わること自体が利用者への影響**（後方互換性）になる。
  落ちたときの意味が違う（検証＝不具合、観測＝変わってよいかを判断する）。書き方は 7 節
- **外部の基準**：CodeQL などの静的解析（[`GitHubUsage.md`](GitHubUsage.md) 1 節）、相互運用（汎用認証サイトの E2E）

---

## 3. 期待値の変更と、その承認

### 期待値の変更に当たるもの

| 対象 | 期待値の変更に当たるもの |
|---|---|
| Golden（`root/programs/*/Frameworks/Tests/Golden/**/Result*.txt`） | **正規化しても差分が残る変更**（正規化は `CompareResult.ps1`。`2_RunAllTests.ps1` が判定に使うものと同じ） |
| NUnit（`root/programs/*/Frameworks/Tests/NUnit/**/*.cs`） | テストの削除（`[Test]`・`[TestCase]`・`[TestCaseSource]` の行の削除）、`[Ignore]`・`[Explicit]`・`Assert.Ignore` の追加、**`[Basis]` の削除**（検証から観測への格下げ。7 節）、**検証の削除**（`Assert.` の行の削除。`Assert.Ignore` を除く。#671） |

- **Golden は、実行のたびに書き換わる。** 日時・スレッド ID・署名値などの揺れは、期待値の変更に数えない（`QUALITY.common.md` 3 節）。揺れの戻し方は [`10_CHEATSHEET.md`](root/programs/10_CHEATSHEET.md)
- `Assert.Ignore` の正当な使い方：管理者権限が無い、DB に接続できない
- **行の数のファイルごとの差し引き**（`QUALITY.common.md` 3 節）は、#671 で、導入の後の 97 コミットに当てて過検出が 0 件であることを確かめた
- **仕組みのファイル**には、共通のものに加えて `CompareResult.ps1`・`CompareGolden.ps1` が当たる（`rules.psd1` が足した）

### このリポジトリの規則（#657）

| ファイル（`root/programs/`） | 持ち主 | 中身 |
|---|---|---|
| `rules.psd1` | このリポジトリ | OpenTouryo の規則（Golden・NUnit のテストの属性・無効化・`[Basis]` の削除・`TrailerRequirements`） |
| `CompareGolden.ps1` | このリポジトリ | Golden の比べ方（`CompareResult.ps1` で正規化して比べる。`rules.psd1` から差し込む） |

- 写し（本体・共通の規則・判定の用例・フック・突き合わせ役）は `QUALITY.common.md` 3 節の表のとおり。汎用認証サイトも同じ本体を写して使う
- **要確認**の範囲（`rules.psd1` の `ReviewScope`）は書かない＝リポジトリ全体（`DiffScope` はテストの範囲で、要確認には使わない）
- 段 2 の有効化・PR の前の確かめ・CI で止まったときは `QUALITY.common.md` 3 節。手元のコマンドの一覧は [`10_CHEATSHEET.md`](root/programs/10_CHEATSHEET.md)

---

## 4. レビューの観点

**名前空間に応じて、次を確かめる。** どの名前空間でも見る観点は `QUALITY.common.md` 4 節にある。

### 署名・暗号（`Public.Security`）

- [ ] 既定値（アルゴリズム・鍵長・モード）を変えていないか。変えるなら後方互換性と、利用者への影響
- [ ] ECB を使っていないか。IV・ノンスを使い回していないか
- [ ] 乱数は暗号論的な生成元か
- [ ] 鍵の導出（反復回数・ソルト）が弱くなっていないか
- [ ] 署名の検証が、**署名の対象と、実際に使う要素の一致**まで見ているか（XML 署名の付け替え）
- [ ] 既存の暗号文・署名を、変更後も読めるか（互換性）

### SAML2・OAuth2 / OIDC のクライアント（`Framework.Authentication`）

- [ ] SAML2：署名の対象と消費する要素の一致、`InResponseTo`・リプレイ、`Audience`、`NotBefore` / `NotOnOrAfter`、アルゴリズム
- [ ] OIDC / OAuth2：`state`・`nonce`、`redirect_uri` の完全一致、PKCE、トークンの紐付け（mTLS・DPoP）、`alg` の固定（`none` を受けない）
- [ ] 署名付きの応答（JARM など）で、**エラーの応答だけ検証を省いていないか**。エラーの応答も `iss`・`aud`・`exp` を確かめる（#644）
- [ ] 公開 API のパラメタ・既定値を変えていないか

### Web のセキュリティ（`Framework.Presentation`・`Business.Presentation`）

画面の基底クラス（`BaseController`・`MyBaseController`）と、MVC のフィルタ（`MyMVCCoreFilterAttribute`）など、
**利用者の画面すべてに効く部品**を変えるときに見る（#671。汎用認証サイトの `Controllers` の観点を、Open棟梁 の部品に合わせた）。

- [ ] 画面遷移の確かめ（`SCDefinition`・`directLink`）、セッションの確かめを、設定やパラメタで飛ばせる経路が増えていないか
- [ ] リダイレクトの行き先（`FxRedirect`・エラー画面への遷移）に、利用者の入力が素通しで入らないか（オープン リダイレクト）
- [ ] 状態を変える要求で、CSRF の対策（MVC の `ValidateAntiForgeryToken` など）を外していないか。既定のフィルタの順序・既定値を変えていないか
- [ ] 例外の画面・ログに、利用者に見せてはいけない内容（スタック トレース・接続文字列）を出していないか
- [ ] **読めない入力で 500 にしないか**（汎用認証サイトの #331：NUL 文字を含むフォームは、net10.0 だけが例外で断る）。net48 と net10.0 で振る舞いが分かれないか

### DB（`Public.Db`）

- [ ] リテラル・識別子を文字列で組み立てていないか。組み立てるなら、DBMS ごとのエスケープ
- [ ] パラメタ化できる値を、パラメタにしているか
- [ ] SQL Server 以外（Oracle・MySQL・PostgreSQL）でも同じ意味になるか

### 全般（Open棟梁 の補足）

- [ ] テストが変更した分岐を通っているかは、[`COVERAGE.md`](root/programs/CS/Frameworks/Tests/COVERAGE.md) の HTML で行を見る
- レビュー役を回すのは、[`AGENTS.md`](AGENTS.md)「Issue に対応する流れ」の段 3（`fixed` の前）。回し方は `QUALITY.common.md` 4 節「サブエージェントで回す」
- LLM とは別系統の検査（CodeQL）は [`GitHubUsage.md`](GitHubUsage.md) 1 節
- 見つけた問題がセキュリティの弱点なら、[`AGENTS.md`](AGENTS.md)「セキュリティの指摘は、公開される前に確認を取る」による

---

## 5. フィードバックの輪

- `QUALITY.common.md` 5 節のとおり。回帰テストの名前は「このリポジトリの値」
- `Found-By:` を導入の前のコミットへ遡って数えるなら、Wiki の集計で行う（コミットは書き換えない）

---

## 6. 計測

リリースごとに見る（Wiki の「開発基盤の KPI と KGI」の「AI 駆動開発の品質の指標」）。指標は `QUALITY.common.md` 6 節。

- リスクが「高」の名前空間で、根拠付きのテストの割合は [`CountBasis.ps1`](root/files/tools/PowerShell/Verify/CountBasis.ps1) で数える（フォルダごとに、検証・観測・未分類と根拠の種類。#671）

---

## 7. 期待値の根拠の書き方

**NUnit は属性で書く。Golden は観測とする。**

### NUnit：`[Basis]`（検証）と `[Observation]`（観測）

属性は [`Tests/NUnit/Common/QualityAttributes.cs`](root/programs/CS/Frameworks/Tests/NUnit/Common/QualityAttributes.cs)
（`TestPublic`・`TestFramework` がリンクで共有する。名前空間は `TestCommon`）。
NUnit のプロパティ（`Basis`・`Observation`）になる。**trx には出ない**ので、6 節の計測はソースの属性を数える
（属性で書くので、`[Basis(` を数えれば拾える）。

```csharp
[Test]
[Basis("RFC 7638 3.1（JWK Thumbprint の計算の例）")]
public void Thumbprint_Rfc7638Example() { ... }

[Test]
[Observation("Default が HMACSHA1 になることは、EnumKeyedHashAlgorithm に約束が書かれていない（今の振る舞いを写した）")]
public void GetKeyedHashBytes_Default() { ... }
```

- 根拠の種類と書き方の例は `QUALITY.common.md` 7 節。仕様は `RFC 4231 4.3（Test Case 2）`、`RFC 9449 7.1（…）` のように、**何の例・どの規定かを括弧で**書く。設計は Open棟梁 自身の文書に書いた約束
- **1 つのテストに複数の `[Basis]` を書いてよい**（RFC が複数にまたがるテスト ベクタなど）。クラスに付ければ全テストに掛かる
- **`[TestCase]` の一部だけ根拠が違う**ときは、XML コメントの `<remarks>` に書く。分けられるならメソッドを分ける
  （例：既定値（`Default`）が何になるかを、テスト ベクタのメソッドから分けて `[Observation]` にした）
- **`[Basis]` を消すのは期待値の変更**（3 節）。`[Observation]` を足すだけなら当たらない。`[Observation]` を `[Basis]` に替えるのは、人が根拠を確かめてから

### Golden：観測とする

Golden（`Result*.txt`）は、コンソール EXE の出力を丸ごと写したもので、**全体が観測**である。

- 行ごとの根拠は書かない。**仕様に照らせる値（テスト ベクタなど）は、NUnit に `[Basis]` 付きで書く**
- Golden の役割は、net48・net10.0・Linux の**出力が変わらないこと**の確認（後方互換性）である

---

## 8. エージェント向けチェックリスト

共通の項目は `QUALITY.common.md` 8 節。Open棟梁 では、次も確かめる。

- [ ] 期待値の変更に、Golden の正規化しても残る差分が入っていないか（揺れは戻す。3 節）
- [ ] テストを足したなら、`[Basis]`（根拠を原典で確かめて）か `[Observation]`（理由を書いて）を付けたか（7 節）
- [ ] リスクが「高」の名前空間を `fixed` で変えたなら、`quality-reviewer` を回し、`Reviewed-with: quality-reviewer (<モデル ID>)` を書いたか（4 節）
