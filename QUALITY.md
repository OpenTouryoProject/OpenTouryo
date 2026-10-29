# 品質の羅針盤（QUALITY.md）

**どこを・何で・どれだけ確かめるか**を決めた文書。人が決め、エージェントが従う。

- **エージェントは、変更の前に本書を読む**（[`AGENTS.md`](AGENTS.md) から参照し、`CLAUDE.md` から読み込んでいる）
- 方針の経緯と、汎用認証サイトとの共通部分は Wiki の
  [品質の羅針盤（方針案）](https://github.com/NetDevInfraWGinOSSConsortium/NetDevInfraWiki/wiki/OTR_QualityCompass)。
  導入は [#640](https://github.com/OpenTouryoProject/OpenTouryo/issues/640)
  （汎用認証サイトは [MultiPurposeAuthSite#315](https://github.com/OpenTouryoProject/MultiPurposeAuthSite/issues/315)）

**本書には方針と表だけを置く。** 手順は各文書が一次情報である。

| 内容 | 一次情報 |
|---|---|
| 検証の手順・回す範囲 | [`CHEATSHEET.md`](root/programs/CHEATSHEET.md) 1 節、[`TESTING.md`](root/programs/TESTING.md) |
| コーディング規約 | [`CODING.md`](root/programs/CODING.md) |
| カバレッジの値 | [`Tests/COVERAGE.md`](root/programs/CS/Frameworks/Tests/COVERAGE.md) |
| セキュリティの指摘の扱い | [`AGENTS.md`](AGENTS.md)「セキュリティの指摘は、公開される前に確認を取る」、[`Security.ja.md`](Security.ja.md) |

> **人は個々の変更を細かく見るより、羅針盤を決める役を担う。**
> エージェントは、作業がどの名前空間に当たるかを本書で引き、その重みで確かめ、報告する。

---

## 1. 品質マップ

**リスクの高い名前空間ほど、検証の手段を重ね、人の確認点を増やす。**
リスクの低い名前空間はエージェントに任せ、壊れたら作り直す。

リスクは、壊れたときの影響（セキュリティ・データ・後方互換性）と、仕様の複雑さで付ける。
名前空間の接頭辞 `Touryo.Infrastructure.` は省略する（フォルダは `root/programs/CS/Frameworks/Infrastructure/` の下）。

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

- **表に無い名前空間は「中〜低」として扱う。** 迷ったら、高い方に倒して報告する
- **「人が必ず見る箇所」に触れたら、報告の冒頭に書く。** 人が差分のどこを見ればよいかを示すためである
- 検証の手段の具体（どのスクリプトを、どの引数で回すか）は [`CHEATSHEET.md`](root/programs/CHEATSHEET.md) 1 節の対応表による

---

## 2. 正の定義

**テストが通ることは、正しいことの証明ではない。** 同じエージェントが書いたテストとコードは、仕様の読み違いを共有しうる。

- **期待値には根拠を書く。** 仕様の該当箇所（RFC の節番号など）か、Issue の番号
  - リスクが「高」の名前空間（`Public.Security`・`Framework.Authentication`）から入れる。書き方は 7 節
  - 根拠を書けない期待値は、**今の振る舞いを写しただけ**である。そう分かるように書く（「観測」。下記）
- **「検証」と「観測」を分ける**（汎用認証サイトの `TESTCASES.md` と同じ考え方）
  - 検証：仕様に照らして、正しいと言える値を確かめる
  - 観測：仕様に幅がある、または根拠を確かめていない値を、変わったことに気付くために残す
  - **観測から検証へ上げるのは、人が根拠を確かめてから**
  - **汎用認証サイトとの違い：Open棟梁 の観測は落ちる。** 汎用認証サイトの E2E は観測を判定しないが、
    Open棟梁 は部品のライブラリで、**振る舞いが変わること自体が利用者への影響**（後方互換性）になる。
    落ちたときの意味が違う（検証＝不具合、観測＝変わってよいかを判断する）。書き方は 7 節
- **外部の基準を、最終的な正とする。** CodeQL などの静的解析、相互運用（汎用認証サイトの E2E）

---

## 3. 期待値の変更と、その承認

エージェントは、**テストを書き換えて通す**ことがある。
人の注意に頼らず、**仕組みでスルーできないようにする。**

### 期待値の変更とは

機械的に検出できるものに絞る。

| 対象 | 期待値の変更に当たるもの |
|---|---|
| Golden（`root/programs/*/Frameworks/Tests/Golden/**/Result*.txt`） | **正規化しても差分が残る変更**（正規化は `CompareResult.ps1`。`2_RunAllTests.ps1` が判定に使うものと同じ） |
| NUnit（`root/programs/*/Frameworks/Tests/NUnit/**/*.cs`） | テストの削除（`[Test]`・`[TestCase]`・`[TestCaseSource]` の行の削除）、`[Ignore]`・`[Explicit]`・`Assert.Ignore` の追加、**`[Basis]` の削除**（検証から観測への格下げ。7 節） |

- **Golden は、実行のたびに書き換わる。** 日時・スレッド ID・署名値などの揺れは、期待値の変更に数えない。
  数えると、ほぼ毎回の `fixed` のコミットに承認が要り、**承認が形だけになる**
- `Assert.Ignore` には正当な使い方がある（管理者権限が無い、DB に接続できない）。**足すなら理由を書いて承認を受ける**
- **アサーションの緩和**（比較を弱める、許容範囲を広げる）は、機械的な検出が難しい。4 節のレビューの観点で拾う

### 承認の記録：コミットのトレーラー

期待値の変更を含むコミットには、**コミット メッセージの末尾**にトレーラーで理由を書く。

```text
fixed #NNN

Expectation-Change: <何が・なぜ変わったか（仕様の該当箇所・Issue 番号）>
```

- **コミットするのは人なので、トレーラーを書くことが、人が差分を見て承認した記録になる**
- 理由が複数あれば、行を分けて複数書く
- 揺れだけの変更（上記）にはトレーラーは要らない

### 3 段で塞ぐ

| 段 | 仕組み | 何を塞ぐか | 状態 |
|---|---|---|---|
| 1. エージェント | Claude Code の PreToolUse フック（[`.claude/settings.json`](.claude/settings.json) → [`ExpectationGuard.ps1`](.claude/hooks/ExpectationGuard.ps1)）：期待値になりうる編集を、確認付きにする | エージェントが黙って期待値を書き換えること | 導入済み（リポジトリに入れば効く） |
| 2. 人のコミット | commit-msg フック（[`.githooks/commit-msg`](.githooks/commit-msg) → [`CheckExpectationChange.ps1`](root/programs/CheckExpectationChange.ps1)）：トレーラーの無い期待値の変更を拒否する | 理由の無いコミット | 導入済み（**各自の手元で有効にする**。下記） |
| 3. PR | GitHub Actions（[`expectation-change.yml`](.github/workflows/expectation-change.yml)）：develop・master 宛の PR のコミットを検査する（検出は段 2 と同じ `CheckExpectationChange.ps1` の、**ベース側の版**） | フックの未設定や `--no-verify` での迂回。**最終的に塞ぐのはここ** | 導入済み（**必須のチェックにするのは人**。[`GitHubUsage.md`](GitHubUsage.md) 2 節） |

- **何を期待値の変更と数えるかの実装は `CheckExpectationChange.ps1` にある**（段 1 は、編集の前に推測する簡易版）
- **段 1 は止めない。** 人に見せるだけで、許可すれば編集は進む。承認の記録は段 2・段 3 のトレーラーである
- 段 1 は Claude Code の機能である。**ほかのエージェントでは効かない**が、段 2・段 3 で塞がる
- **マージ コミットは見ない。** 取り込むコミットのほうで確かめているため
- **仕組み自体のファイル**（`.githooks/`・`.claude/settings.json`・`.claude/hooks/`・`CheckExpectationChange.ps1`・`CompareResult.ps1`・
  `expectation-change.yml`）**の変更・削除も、トレーラーが要る。** 仕組みを緩めれば、期待値をいくら変えても通るため（追加は数えない）
- PR を出す前に手元で確かめるなら `.\CheckExpectationChange.ps1 -Range develop..HEAD`（`root\programs` で）
- **`-Range`（段 3）は、導入より前のコミット（`CheckExpectationChange.ps1` がまだ無いコミット）を見ない。**
  規約が無かった時点のコミットだからである。`-Commit` で個別に見れば、導入前でも見る（6 節の計測に使える）

#### 段 2 を有効にする（各自の手元で 1 回）

```powershell
git config core.hooksPath .githooks
```

- フックは `sh` で動き、`powershell.exe`（無ければ `pwsh`）で `CheckExpectationChange.ps1` を呼ぶ
- 拒否されたら、差分を確かめてトレーラーを書く。**`--no-verify` で迂回しても、段 3 で止まる**
- 過去のコミットを確かめるには `.\CheckExpectationChange.ps1 -Commit <コミット>`（`root\programs` で）

### エージェントがすること

- **期待値を変えたら、報告の冒頭に書く。** どのファイルの、どの値が、なぜ変わったか
- **トレーラーの文面を提案する。** コミットは人が行う（[`AGENTS.md`](AGENTS.md)「Git 操作は行わない」）
- **テストを通すために期待値を変えない。** 先にコードを疑う。期待値が誤っていると判断したら、その根拠（仕様の該当箇所）を示す

---

## 4. レビューの観点

**名前空間に応じて、次を確かめる。** エージェントがレビューするときも、自分の変更を見直すときも同じである。

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
- [ ] 検証を省く分岐（設定やパラメタで検証を飛ばせる経路）が増えていないか
- [ ] 公開 API のパラメタ・既定値を変えていないか

### DB（`Public.Db`）

- [ ] リテラル・識別子を文字列で組み立てていないか。組み立てるなら、DBMS ごとのエスケープ
- [ ] パラメタ化できる値を、パラメタにしているか
- [ ] SQL Server 以外（Oracle・MySQL・PostgreSQL）でも同じ意味になるか

### 全般

- [ ] 期待値の変更（3 節）を含むか。含むなら、根拠があるか
- [ ] テストが、変更した分岐を実際に通しているか（[`COVERAGE.md`](root/programs/CS/Frameworks/Tests/COVERAGE.md) の HTML で行を見る）

### 作る役と検証する役を分ける

- **リスクが「高」の名前空間に触れる変更は、書いたセッションとは別のセッションでレビューする**（文脈を引き継がない）
- さらに、**書いたモデルとは別のモデルでレビューする。** 同じモデルは、同じ読み違いをしやすい
- **CodeQL のような、LLM とは別系統の検査も併用する**（[`GitHubUsage.md`](GitHubUsage.md) 1 節）

> **見つけた問題がセキュリティの弱点なら、公開の文書に書かない。**
> 本書のチェックリストに足すのも、修正と同じコミットにする（[`AGENTS.md`](AGENTS.md)）。

---

## 5. フィードバックの輪

見つかった問題は、次の 3 か所に戻す。

1. **回帰テスト**：Issue に紐付けて残す（テスト名・コメントに Issue の番号）
2. **レビューの観点**：4 節のチェックリストに足し、次からレビューで拾えるようにする
3. **品質マップ**：その名前空間のリスク・検証の手段・人の確認点を見直す（1 節）

**本書を直すのは、問題を直したのと同じコミットにする。** 先に書くと、未修正の弱点を公開することになる。

---

## 6. 計測

リリースごとに見る（Wiki の「開発基盤の KPI と KGI」の「AI 駆動開発の品質の指標」）。

- 期待値の変更のうち、トレーラーが付いたものの割合（3 段で塞げば 100%。漏れがあれば仕組みの穴）
- リスクが「高」の名前空間で、根拠付きのテストの割合
- 別モデルのレビューで見つかり、書いたモデルのレビューでは見つからなかった指摘の数

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
[Observation("RFC 7517 付録 A.1 の鍵の指紋を、今の値で固定した（RFC に、この鍵の指紋の値は載っていない）")]
public void Thumbprint_EcFixedValue() { ... }
```

| 根拠の種類 | 書き方の例 | 備考 |
|---|---|---|
| 仕様 | `RFC 4231 4.3（Test Case 2）`、`RFC 9449 7.1（…）` | **節まで書く。** 何の例・どの規定かを括弧で |
| 別実装 | `別実装：.NET の SHA256 と一致` | 独立した実装と突き合わせたもの |
| 設計 | `設計：<文書や XML コメントの場所>` | Open棟梁 自身の文書に書いた約束。**書かれていない約束は観測** |
| Issue | `#634` | 不具合の回帰 |

- **節番号は、原典で確かめてから書く。** 誤った節は、書かないより悪い
- **1 つのテストに複数の `[Basis]` を書いてよい**（RFC が複数にまたがるテスト ベクタなど）。クラスに付ければ全テストに掛かる
- **`[TestCase]` の一部だけ根拠が違う**ときは、XML コメントの `<remarks>` に書く。分けられるならメソッドを分ける
- **付いていないテストは「未分類」。** 既存のテストは、リスクが「高」の名前空間（1 節）から順に付ける
- **`[Basis]` を消すのは期待値の変更**（3 節）。`[Observation]` を足すだけなら当たらない
- **観測から検証へ上げる（`[Observation]` を `[Basis]` に替える）のは、人が根拠を確かめてから**（2 節）

### Golden：観測とする

Golden（`Result*.txt`）は、コンソール EXE の出力を丸ごと写したもので、**全体が観測**である。

- 行ごとの根拠は書かない。**仕様に照らせる値（テスト ベクタなど）は、NUnit に `[Basis]` 付きで書く**
- Golden の役割は、net48・net10.0・Linux の**出力が変わらないこと**の確認（後方互換性）である

---

## 8. エージェント向けチェックリスト

着手の前と、報告の前に確かめる。

- [ ] 作業の対象が、品質マップ（1 節）のどの名前空間で、リスクはどれか
- [ ] 「人が必ず見る箇所」に触れたか。触れたなら、報告の冒頭に書いたか
- [ ] 期待値の変更（3 節）を含むか。含むなら、報告の冒頭に書き、トレーラーの文面を提案したか
- [ ] テストを足したなら、`[Basis]`（根拠を原典で確かめて）か `[Observation]`（理由を書いて）を付けたか（7 節）
- [ ] リスクが「高」なら、4 節の観点で見直したか。別のセッション・別のモデルのレビューを人に勧めたか
- [ ] 見つけた問題を、5 節の 3 か所に戻したか（セキュリティの弱点なら、まず人に報告したか）
