# GitHub の使用状況

**このリポジトリで GitHub のどの機能を、どう設定して使っているか。**

設定は GitHub 側（Web UI / API）にあり、**リポジトリのファイルからは見えない**。
何をどういう理由で設定したかを、ここに残す。

> 変更したら本書も更新すること。**設定と本書が食い違うと、本書の方が害になる。**

**本書はリポジトリ内部の記録である。** 利用者に影響する変更は Issue に書く。

| 内容 | 一次情報 |
|---|---|
| CodeQL のトリアージと、**利用者に影響する変更** | [#536](https://github.com/OpenTouryoProject/OpenTouryo/issues/536) |
| Secret scanning の除外パス | [`.github/secret_scanning.yml`](.github/secret_scanning.yml) |
| 手順の早見 | [`CHEATSHEET.md`](root/programs/CHEATSHEET.md) |

最終更新: 2026-08-10

---

## 1. セキュリティ

### 適用している構成

組織 `OpenTouryoProject` の **Code security configuration**「**OpenTouryo standard**」（id `265927`）を、
**このリポジトリにのみ**適用している（組織の既定にはしていない）。

https://github.com/organizations/OpenTouryoProject/settings/security_products

| 項目 | 値 | 理由 |
|---|---|---|
| Dependency graph | 有効 | 公開リポジトリでは常時 |
| Dependabot alerts | 有効 | |
| Dependabot security updates | 有効 | 既に運用中。構成として明示的に固定した |
| Code scanning（CodeQL 既定セットアップ） | 有効 | ワークフローを自前で持たずに済む |
| Private vulnerability reporting | 有効 | **暗号・認証ライブラリを公開している**ため、非公開の報告窓口が要る |
| Secret scanning | 有効 | |
| **Secret scanning: Push protection** | **有効** | 2026-08-10 に有効化（後述） |
| **Secret scanning: Non-provider patterns** | **無効** | **後述** |
| **Secret scanning: Validity checks** | **無効** | **後述** |
| GitHub Advanced Security | 有効 | 公開リポジトリでは無料 |
| Enforcement | Unenforced | リポジトリ個別の調整余地を残す |

> **`GitHub recommended`（GitHub 提供のグローバル構成）は使っていない。**
> 上の 3 項目が推奨と食い違うため、独自構成を作った。

> **`advanced_security: enabled` は公開リポジトリだから無料である。**
> この構成を**非公開リポジトリへ適用するとライセンスを消費する**。使い回す際は注意。

### 3 項目を無効にしている理由

#### Push protection

**段階分けのため、最初は無効にした。** 有効にすると、検知した push をその場で止める。

```
1. .github/secret_scanning.yml を develop へ入れる
2. Secret scanning だけ先に有効化   → アラート 0 件で立ち上がり、棚卸しは不要だった
3. Push protection を有効化         → 2026-08-10 実施
```

いきなり両方入れると、**リリース作業の途中で不意に止まる**。
なお Push protection は `.github/secret_scanning.yml` の除外を**見ない**。

#### Non-provider patterns

汎用のパスワードらしき文字列まで拾う。
このリポジトリには **`Password=` を含む設定ファイルが 49 件**あり（テスト・サンプルの接続文字列）、
**有効にすると本物が埋もれる。**

#### Validity checks

検知した候補を**発行元へ問い合わせて有効性を確認**する。外部通信であり、必要性がない。

### 除外パス

[`.github/secret_scanning.yml`](.github/secret_scanning.yml) を参照。
**テスト専用と分かっている資材だけ**を除外している。

- `root/files/resource/X509/**` … 暗号・署名テスト用の証明書と秘密鍵 22 件
- ClickOnce の `WSClientWinCone_sample_TemporaryKey.pfx`

**先にこのファイルを `develop` へ入れてから Secret scanning を有効化したので、
アラートは 0 件で立ち上がった**（棚卸しが発生しなかった）。

### 有効化した時点のアラート

| | 件数 |
|---|---|
| Secret scanning | **0** |
| Code scanning（CodeQL） | **30**（critical 1 / high 23 / medium 9） |

CodeQL の内訳（上位）。

```
8  Inefficient regular expression
8  Missing X-Frame-Options HTTP header
6  Unsafe jQuery plugin
4  'requireSSL' attribute is not set to true
2  Cookie 'Secure' attribute is not set to true
2  DOM text reinterpreted as HTML
1  Deserialization of untrusted data     ← critical
```

### トリアージの方針

> **経緯と実施内容は
> [#536](https://github.com/OpenTouryoProject/OpenTouryo/issues/536) が一次情報。**
> **利用者に影響する変更が 2 点ある**（`CipherMode_ECB` の非推奨化と、
> Cookie の `Secure` 属性）。リリース ノートからはそちらを参照する。

**アラート 1 件ごとに Issue は立てない。** Security タブが追跡するので、
同じものを Issue でも管理すると二重管理になる。
**Issue に書くのは「何を決めたか」と「利用者への影響」**であり、
一覧そのものではない。

| 分類 | 件数 | 方針 |
|---|---|---|
| **同梱のサードパーティ製 JS** | 15 | **棄却済み**（`won't fix`）。jQuery / jQuery Validation であり、修正対象ではない。更新はライブラリの版上げで行う。`-vsdoc.js` は IntelliSense 用で実行されない |
| **`BinarySerialize.cs`（critical）** | 1 | **棄却済み**（`won't fix`）。**net48 でのみ継続利用**。`Public_netcore100.csproj` で `<Compile Remove>` しており、netcore100 ではコンパイルされない（`BinaryFormatter` は .NET 9 で削除済み）。net48 の下位互換のために残す |
| **`Missing X-Frame-Options`** | 8 | **6 件は対応済み**（下表）。残る 2 件は MVC の `Views/Web.config` で、**応答ヘッダを出す設定ファイルではない**ため対応しない |
| **`requireSSL` が `true` でない** | 4 | **対応済み。** `requireSSL="true"` の行を**コメントアウトで併記**し、「本番ではコメントアウトを外す」と明記。**サンプルは HTTP で動かす**ため、既定は `false` のまま |
| **`Encryption using ECB`** | 1 | **対応済み → 棄却。** `EnumSymmetricAlgorithm.CipherMode_ECB` に **`[Obsolete]` を付与**した。利用者が指定したときだけ通る 5 択の 1 つで、**既定ではない**（指定しなければ .NET 既定の CBC）。リポジトリ内に指定箇所は 0 件。**削除は下位互換を壊す**ため、非推奨化に留めた。コードは残るのでアラートは消えず、`won't fix` で棄却する |
| **`Cookie 'Secure'` 未設定** | 2 | **対応済み → 棄却。** 指摘されたのは `StdMigration/CookieExtensions.cs` の `Set` 3 つのうち **`CookieOptions` を受け取らない 2 つ**で、**リポジトリ内に呼び出しが 1 件も無い**（利用者向けに残している移植 API）。実際に Cookie を作るのは `CookieOptions` 版を呼ぶ `FxCmnFunction` で、そちらの **4 箇所**（net48 / netcore100 各 2）を修正した。**`HttpOnly` は設定済みだったが `Secure` が無かった**（前者は XSS 対策、後者は盗聴対策で別物） |
| **`DOM text reinterpreted as HTML`** | 2 | **多層防御を追加したが、アラートは残る → 棄却。** 擬似ダイアログの `iframe.src` に URL を設定する前に `Fx_IsSafeDialogUrl()` でスキームを検証するようにした。**CodeQL はこの検証関数をサニタイザと認識しない**ため、指摘は消えない。**既存の脆弱性を塞いだのではない**（URL はフレームワークの `ShowModalScreen` が組み立て、利用者入力は入らず、AppScan もクリア済み） |

**棄却は削除ではない。** Security タブに `Dismissed` として残り、
棄却した人・日時・理由・コメントが記録される。復帰もできる。

### `X-Frame-Options` の値を対象ごとに分けている理由

| 対象 | 値 | 理由 |
|---|---|---|
| `WebForms_Sample`（CS / VB） | **`SAMEORIGIN`** | 擬似ダイアログが**同一オリジンの `iframe`** を使う（`Scripts/touryo/common.js` の `FxIFrame`）。**`DENY` にすると動作しなくなる** |
| `MVC_Sample`（CS / VB） | `DENY` | MVC 版の `common.js` は**別物（818 行）で `iframe` を使わない**。WebForms 版は 1656 行で `iframe` 6 箇所 |
| `ASPNETWebService`（CS / VB） | `DENY` | SOAP の Web サービス。フレームに表示する用途がない |

**既定は `DENY`、必要な所だけ `SAMEORIGIN` に緩める。**
一律に緩めると、緩める必要のない画面まで枠に入れられるようになる。

### Cookie の `Secure` は「HTTPS のときだけ」立てる

```csharp
cookieOptions.HttpOnly = true;
cookieOptions.Secure = MyHttpContext.Current.Request.IsHttps;        // netcore100
newCookie.Secure = HttpContext.Current.Request.IsSecureConnection;   // net48
```

**無条件に `true` にしてはならない。** HTTP で動かす開発環境とサンプルで、
ブラウザが Cookie を保存しなくなり、**セッション タイムアウト検出が機能しなくなる**
（疎通も通らなくなる）。

**CodeQL が指摘した場所と、直すべき場所が違った例。**

`StdMigration/CookieExtensions.cs` の `Set` は 3 つある。

| オーバーロード | 指摘 | 呼び出し |
|---|---|---|
| `Set(key, value)` | **あり** | **0 件**（利用者向けに残している移植 API） |
| `Set(key, value, int expireTime)` | **あり** | **0 件**（同上） |
| `Set(key, value, CookieOptions)` | なし | `FxCmnFunction` が使用 |

**クラス自体は使われている。**
`Samples4NetCore/Backend/MVC_Sample` も `MyHttpContext` / `SessionExtensions` とあわせ、
`FxCmnFunction.DeleteCookieForSessionTimeoutDetection()` 経由で
`CookieOptions` 版を呼んでいる。

**直したのは `FxCmnFunction` 側**（Cookie を実際に組み立てている場所）。
**net48 側は指摘されていないが、同じ設計の別実装なので揃えた。**

> `Samples4NetCore` の `Startup.cs` には
> `//Secure = CookieSecurePolicy.Always` がコメントアウトで残っている。
> アプリ全体の Cookie ポリシーで一括指定する方法もある（未検討）。

### CodeQL の挙動（踏んだので記録する）

#### ① 棄却は、コードが動くと外れる

**アラートの同一性は「ルール ＋ 位置」で判定される。**
棄却した箇所の周辺を編集して行番号がずれると、
**別のアラートとして作り直され、`open` に戻る。**

`Encryption using ECB` がこれに当たった。棄却済みだったが、
`#pragma warning disable` とコメントを足したことで **287 行 → 291 行**にずれ、
新しい番号で `open` に戻ったため、**同じ理由で棄却し直した。**

**棄却済みの箇所を触ったら、棄却が残っているかを確認すること。**

#### ② `fixed` は「直った」とは限らない

`#537` のマージ後、`fixed` が 12 件になったが、
**本当に解消したのは `Missing X-Frame-Options` の 6 件だけ**だった。

```
fixed 12 件  ≠  12 件が直った
   6 件  解消（新しいアラートが出ていない）
   6 件  位置が変わっただけ（同数が open に出ている）
```

**`fixed` の件数だけを見て「減った」と判断してはならない。**
`open` の中身とあわせて確認する。

#### ③ PR のチェックと、ブランチ全体のスキャンは別物

**PR のチェックは差分中心で、ブランチ全体の状態とは一致しない。**

- `#537` の `CodeQL` チェックは `fail` だったが、**新しい脆弱性ではなかった**。
  行番号がずれた 2 件を「この PR で追加された」と数えたため
- 逆に、PR 時点では `'requireSSL'` が解消したように見えたが、
  **`develop` の全体スキャンでは残っていた**

**判断は `develop`（既定ブランチ）のスキャン結果で行うこと。**

#### ④ 検証を足してもアラートは消えないことがある

`DOM text reinterpreted as HTML` に対して `Fx_IsSafeDialogUrl()` を追加したが、
**CodeQL はこの関数をサニタイザと認識せず、指摘は残った。**

**「アラートが消えること」と「安全になること」は別である。**

| | アラート | 実際の安全性 |
|---|---|---|
| `'requireSSL'`（コメントで併記） | **消えない** | **変わらない**（有効な設定は `false` のまま） |
| `DOM text ...`（検証を追加） | **消えない** | **上がった**（危険なスキームを実際に弾く） |
| `X-Frame-Options`（ヘッダ追加） | **消えた** | **上がった** |

**アラート件数を目的にすると判断を誤る。** 何を守りたいかで決めること。

---

## 2. ブランチと保護

| ブランチ | 役割 | 保護 |
|---|---|---|
| `develop` | 既定ブランチ。開発の集約先 | force-push 禁止 / 削除禁止 |
| `master` | リリース | 上記 ＋ **レビュー 1 名必須** ＋ **必須チェック `build`** |
| `deps` | Dependabot PR の受け先 | なし |

ブランチ運用は git-flow。**規約は [`Contributing.ja.md`](Contributing.ja.md)。**

### レビュアー

`master` へのマージには**レビュー 1 名の承認**が要る（必須チェック `build` とあわせて 2 つの関所）。

| | ロール | 承認 |
|---|---|---|
| `daisukenishino2` | admin | 可 |
| `OsscJpDevInfra` | admin | 可 |
| `daisukenishino77` | **write**（2026-08-10 に追加） | 可 |

> **承認が必須レビューとして数えられるのは、`write` 以上の人だけ。**
> `read` / `triage` でもレビューは書けるが、**ブランチ保護は満たさない。**
> 公開リポジトリなので誰でも `read` は持つが、それでは足りない。

> **自分が出した PR は、自分で承認できない。**
> 3 名いるので、誰が PR を出しても残り 2 名から選べる。

組織の `default_repository_permission` は `none`。
リポジトリへの権限は**明示的に付与する**（`affiliation=direct` で確認できる）。

```bash
gh api repos/OpenTouryoProject/OpenTouryo/collaborators/<login>/permission --jq '{permission, role_name}'
gh api "repos/OpenTouryoProject/OpenTouryo/collaborators?affiliation=direct" --jq '.[] | "\(.login) \(.role_name)"'
gh api repos/OpenTouryoProject/OpenTouryo/invitations   # 承諾待ちの招待
```

### 意図的にそうしている設定

**弱点に見えるが、この運用では正しい**もの。理由を書いておかないと、また提案が出る。


- **`develop` に必須ステータス チェックは置かない**（意図的）。
  `deps` ⇔ `develop` ⇔ feature と往復が多く、毎回のマージが CI 待ちになるため。
  代わりに **`master` 宛の PR で CI を動かす**（3 節・7 節）
- `master` の `enforce_admins` は無効（少人数運用のため意図的）
- **`delete_branch_on_merge` は無効にしておく**（意図的）。
  **作業ブランチを継続利用する運用**のため。`3rd_agent` は 4 回、`2nd_agent` は 6 回と、
  同じブランチを何度も `develop` へマージしている。
  **自動削除にすると 1 回目で消えて、毎回作り直しになる。**
  `deps`（Dependabot の受けブランチ）が消えると、`dependabot-retarget.yml` も機能しなくなる。
  この設定は **PR ごとに使い捨てるブランチ**を前提としたもので、この運用には合わない

### マージ方式は「通常のマージ」だけ

```
allow_merge_commit   true
allow_squash_merge   false   … 2026-08-10 に無効化
allow_rebase_merge   false   … 同上
```

**squash と rebase はコミットを消す**（rebase は SHA が変わる）。
公開済みの NuGet パッケージは**詰めた時のコミットに固定される**ため、
そのコミットが到達不能になると **Source Link が壊れる。公開後には直せない**
（[`root/programs/CS/NuGet/README.md`](root/programs/CS/NuGet/README.md) 7 節）。

**以前は「`master` へは `--no-ff` で」という申し合わせだけで担保していた。**
実績としても squash は未使用、rebase も 1 度だけだったため、
**選択肢ごと無くして、仕組みで防ぐことにした。**

> ローカルの `git rebase`（push 前の整理）には影響しない。
> PR のマージ ボタンから選択肢が消えるだけである。

---

## 3. GitHub Actions

### `.github/` の中身

| ファイル | 役割 |
|---|---|
| [`workflows/build-windows.yml`](.github/workflows/build-windows.yml) | 検証 3 本（ビルド・単体テスト・疎通）を windows-latest で |
| [`workflows/dependabot-retarget.yml`](.github/workflows/dependabot-retarget.yml) | Dependabot PR の向き先を `deps` へ変更 |
| [`secret_scanning.yml`](.github/secret_scanning.yml) | Secret scanning のアラートから除外するパス（1 節） |
| [`ISSUE_TEMPLATE/`](.github/ISSUE_TEMPLATE) | Issue テンプレート 3 種 ＋ `config.yml`（5 節） |
| [`pull_request_template.md`](.github/pull_request_template.md) | PR テンプレート（5 節） |

**`dependabot.yml` は置いていない**（4 節）。

### ワークフロー

| ファイル | 発火 | `permissions` |
|---|---|---|
| `build-windows.yml` | `push: [deps]` / **`pull_request: [master]`** / 手動 | `contents: read` |
| `dependabot-retarget.yml` | `pull_request_target`（`opened`） | `pull-requests: write` |
| CodeQL（既定セットアップ） | GitHub 管理。`push` / `pull_request` / 週次 | ワークフロー ファイルを持たない |

`build-windows.yml` は既知の署名エラー（`MSB3482` / `MSB3325` / `MSB3321`）を除外する。
**ローカルと CI で出るコードが違う**理由はファイル冒頭のコメントにある。

> **`dependabot-retarget.yml` は `pull_request_target` を使う。**
> これは**ベース ブランチ側の定義を、書き込み権限付きで動かす**トリガである。
> ここで PR のコードを `checkout` して実行すると、
> **PR に任意のコードを書ける相手へ権限を渡すことになる**（pwn request）。
>
> 本ワークフローは **`checkout` を行わず**、`gh` コマンドだけを実行し、
> 権限も `pull-requests: write` の 1 つに絞っている。
> **この 2 点は変更しないこと。** 理由はファイル冒頭のコメントにある。

### GitHub 側の設定

```
Actions                            enabled
allowed_actions                    all          … 使用できるアクションを制限していない
sha_pinning_required               false        … アクションの SHA 固定を強制していない
default_workflow_permissions       read         … 2026-08-10 に write から変更
can_approve_pull_request_reviews   false        … 2026-08-10 に true から変更
Secrets / Variables                なし
```

**既定を `read` にした。** 各ワークフローは `permissions:` を明示して
最小権限にしているため、**既定を下げても動く**（実測で確認）。
**`permissions:` を書き忘れた新しいワークフローが、
書き込み権限を持ってしまう状態を無くすため。**

> それでも**新しいワークフローを足すときは `permissions:` を書くこと。**
> 何を必要としているかが、ファイルを見て分かる方がよい。

**`can_approve_pull_request_reviews` も無効にした。**
Actions が PR を承認できると、`master` のレビュー必須が形骸化するため。

`Secrets` は 1 つも登録していない。**認証が要る操作は入れていない**ということであり、
足すときは「本当に必要か」を先に考える。

---

## 4. Dependabot

- **version updates はリポジトリに `.github/dependabot.yml` を置いていない**（GitHub UI 側の設定で稼働）
- PR は `dependabot-retarget.yml` が `deps` へ向け直す
- `deps` で CI を通してから `develop` へまとめてマージする

> **`dependabot.yml` を置けば `target-branch: deps` を直接指定でき、
> retarget ワークフローが不要になる可能性がある。**
> ただし retarget の実挙動が未検証のため保留（#517）。

---

## 5. Issue と PR

### ラベル

11 個を定義している。**設定は人が行う**（`AGENTS.md`）。

| ラベル | 用途 |
|---|---|
| `bug` / `enhancement` / `question` | 種別 |
| `duplicate` / `invalid` / `wontfix` | 処理の結果 |
| **`quality improvement`** | 品質改善。**リファクタリング・規約整備・CI/セキュリティの整備**はここ |
| `good first issue` / `help wanted` | 外部の参加者向け |
| `dependencies` / `.NET` | **Dependabot が自動で付ける**。手で付けない |

```bash
gh label list --repo OpenTouryoProject/OpenTouryo
gh issue view <番号> --repo OpenTouryoProject/OpenTouryo --json labels
```

### テンプレート

```
.github/ISSUE_TEMPLATE/config.yml       任意化 ＋ Security への導線
.github/ISSUE_TEMPLATE/bug.md           不具合          → labels: bug
.github/ISSUE_TEMPLATE/enhancement.md   機能追加・改善  → labels: enhancement
.github/ISSUE_TEMPLATE/quality.md       品質改善        → labels: quality improvement
.github/pull_request_template.md        PR
```

**強制しない。** そのために次の 2 点を選んでいる。

| | 理由 |
|---|---|
| **Markdown 形式（`.md`）** | YAML フォーム（`.yml`）は**必須項目を強制できる**。書きたいことが書けなくなる |
| **`blank_issues_enabled: true`** | 「Open a blank issue」から**素の Issue も起票できる** |

`config.yml` の `contact_links` で、**セキュリティ問題を Private vulnerability reporting へ
誘導している**（Issue の選択画面で分岐するので、公開 Issue に書かれる前に止まる）。

**ラベルはテンプレートの front matter が自動で付ける。**
画面から起票した場合のみで、`gh` の `--body-file` では付かない。

> **エージェントにはテンプレートが自動適用されない。**
> `--template` は「エディタで編集する前提の開始テキスト」で、
> **`--body-file` と併用すると本文で上書きされる。**
> エージェントはテンプレートを**読んで、その構成に沿って書く**
> （[`AGENTS.md`](AGENTS.md)）。

---

## 6. 有効にしていない機能

| 機能 | 判断 |
|---|---|
| **Discussions** | **無効のまま。** Issue で回っており、有効にすると窓口が分散する |
| Projects | 有効だが未使用 |
| Wiki | 有効。[リリース エンジニアリング等の文書](https://github.com/OpenTouryoProject/OpenTouryo/wiki)を置いている |
| Auto-merge | 無効 |
| Web commit signoff | 無効 |

---

## 7. 実行したコマンドの記録

**エージェントが `gh` で直接実行した設定変更。** 参照系（GET）は除く。

### 2026-08-10 : セキュリティ構成の作成と適用

前提として `admin:org` スコープが要る（**リポジトリ admin だけでは足りない**）。

```bash
gh auth refresh -h github.com -s admin:org      # 対話的。人が実行
```

構成の作成。

```bash
cat > cfg.json <<'JSON'
{
  "name": "OpenTouryo standard",
  "description": "Secret scanning は有効、Push protection は棚卸し後に有効化する段階分け構成。non-provider patterns と validity checks は無効。",
  "advanced_security": "enabled",
  "dependency_graph": "enabled",
  "dependabot_alerts": "enabled",
  "dependabot_security_updates": "enabled",
  "code_scanning_default_setup": "enabled",
  "secret_scanning": "enabled",
  "secret_scanning_push_protection": "disabled",
  "secret_scanning_non_provider_patterns": "disabled",
  "secret_scanning_validity_checks": "disabled",
  "private_vulnerability_reporting": "enabled",
  "enforcement": "unenforced"
}
JSON

gh api -X POST orgs/OpenTouryoProject/code-security/configurations --input cfg.json
# → id 265927
```

このリポジトリにのみ適用。

```bash
gh api -X POST orgs/OpenTouryoProject/code-security/configurations/265927/attach \
  -f scope=selected -F 'selected_repository_ids[]=18209571'
```

確認。

```bash
gh api repos/OpenTouryoProject/OpenTouryo --jq '.security_and_analysis'
gh api repos/OpenTouryoProject/OpenTouryo/private-vulnerability-reporting
gh api repos/OpenTouryoProject/OpenTouryo/code-scanning/default-setup --jq '{state, languages}'
gh api orgs/OpenTouryoProject/code-security/configurations/265927/repositories \
  --jq '.[] | {status, repo: .repository.full_name}'
```

### 2026-08-10 : Push protection の有効化

Secret scanning のアラートが 0 件で安定したことを確認した上で実施。
**構成の値を変えるだけ**で、リポジトリ個別の設定は触らない。

```bash
gh api -X PATCH orgs/OpenTouryoProject/code-security/configurations/265927 \
  -f secret_scanning_push_protection=enabled

# リポジトリ側へ反映されたかを確認する
gh api repos/OpenTouryoProject/OpenTouryo --jq '.security_and_analysis'
```

**戻すとき**も同じ形（`-f secret_scanning_push_protection=disabled`）。

### 2026-08-10 : Actions の既定権限を絞る

**リポジトリ設定**（組織の構成ではない）。`repo` スコープで実行できる。

```bash
gh api -X PUT repos/OpenTouryoProject/OpenTouryo/actions/permissions/workflow   -f default_workflow_permissions=read -F can_approve_pull_request_reviews=false

# 確認
gh api repos/OpenTouryoProject/OpenTouryo/actions/permissions/workflow
```

**下げる前に、各ワークフローが `permissions:` を明示しているかを確認すること。**
既定に頼っているワークフローがあると、権限不足で失敗する。

```bash
grep -A3 '^permissions:' .github/workflows/*.yml
```

### 2026-08-10 : master に必須チェックを設定

`develop → master` の PR で `build-windows.yml` を発火させ（`pull_request: [master]`）、
その成功をマージの条件にする。**リリースの関所。**

**チェック名は `build`**（ワークフロー名 `Build on Windows` ではなく **ジョブ名**）。
実測で確認すること。**名前を誤ると、通らないチェックを待ち続けてマージできなくなる。**

```bash
gh api repos/OpenTouryoProject/OpenTouryo/actions/runs/<run_id>/jobs --jq '.jobs[].name'
```

**`PATCH .../required_status_checks` は使えない**（未設定の状態では 404）。
**ブランチ保護全体を `PUT` する**ため、**既存の設定を取得してから同じ値を明示的に渡す。**
渡し漏れた項目は既定値に戻ってしまう。

```bash
# 1. 現在の設定を確認する
gh api repos/OpenTouryoProject/OpenTouryo/branches/master/protection

# 2. 既存値を保ったまま required_status_checks を足して PUT する
cat > prot.json <<'JSON'
{
  "required_status_checks": { "strict": false, "contexts": ["build"] },
  "enforce_admins": false,
  "required_pull_request_reviews": {
    "dismiss_stale_reviews": false,
    "require_code_owner_reviews": false,
    "require_last_push_approval": false,
    "required_approving_review_count": 1
  },
  "restrictions": null,
  "required_linear_history": false,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "block_creations": false,
  "required_conversation_resolution": false,
  "lock_branch": false,
  "allow_fork_syncing": false
}
JSON

gh api -X PUT repos/OpenTouryoProject/OpenTouryo/branches/master/protection --input prot.json

# 3. 全項目を照合する（渡し漏れが無いか）
gh api repos/OpenTouryoProject/OpenTouryo/branches/master/protection
```

`strict`（Require branches to be up to date）は **`false`**。
`true` にすると、`master` が動くたびに PR の再更新と CI 再実行が要る。
リリース時の 1 回きりの操作なので不要。

> **詰まったら。** `enforce_admins` は `false` なので管理者権限で回避できる。
> 解除は同じ `PUT` で `required_status_checks` を `null` にする。

**設定した直後に、検証用の PR で動作を確かめた**（`#539`、マージせずクローズ）。

```
build           pass  13m57s   ← pull_request:[master] で発火し、成功
CodeQL          pass
mergeStateStatus  BLOCKED
reviewDecision    REVIEW_REQUIRED   ← 止まっているのはレビュー未承認だけ
```

**チェック名が違っていれば、`build` が「Required だが未実行」として別の形で止まる。**
`BLOCKED` の理由がレビューだけであることを確認すれば、名前が一致していると分かる。

**CI は約 14 分**かかる。ローカルの実測（ビルド 9 分 ＋ テスト ＋ 疎通）より長いのは、
DB の導入と初期化が入るため。**リリース時はこれを見込むこと。**

### 有効化後の運用

**新しく push する内容だけ**が検査される。既存の履歴は対象外。

| | |
|---|---|
| 止まるもの | **発行元を特定できる形のキー**（NuGet の `oy2...`、AWS、GitHub PAT など） |
| 止まらないもの | `.pfx` / `.cer`、接続文字列の `Password=`（`Non-provider patterns` が無効のため） |

止まった場合は理由を選んでバイパスできる（**管理者に通知され、記録が残る**）。

> **`.github/secret_scanning.yml` の除外は Push protection には効かない。**
> あちらはアラート（Secret scanning）の抑制であり、push の判定は見ていない。

---

## 8. 未着手の提案

| | 内容 |
|---|---|
| `allowed_actions` を絞る / SHA 固定 | 現在 `all` / 強制なし。サプライ チェーン対策。**運用が重くなる**ので、必要性とあわせて判断する |
| `.github/dependabot.yml` | #517 の決着後 |

> **`develop` を必須チェックの対象にはしない。**
> `deps` ⇔ `develop` ⇔ feature と**往復が多いハブ**であり、
> 毎回のマージが CI 待ちになる。
> `deps` 由来の変更は**すでに `deps` で検証済み**なので、二度手間でもある。
>
> **`master` は往復しない。** `develop` からのマージはリリース時だけで、
> 1 回あたり 9 分程度の待ちは受け入れられる。
> `RELEASE.md` フェーズ 1 の「検証 3 本」を、機械的に担保できる。

