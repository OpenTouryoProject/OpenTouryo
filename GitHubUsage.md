# GitHub の使用状況

**このリポジトリで GitHub のどの機能を、どう設定して使っているか。**

設定は GitHub 側（Web UI / API）にあり、**リポジトリのファイルからは見えない**。
何をどういう理由で設定したかを、ここに残す。

> 変更したら本書も更新すること。**設定と本書が食い違うと、本書の方が害になる。**

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
| **Secret scanning: Push protection** | **無効** | **後述。棚卸し後に有効化する** |
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

**段階分けのため。** 有効にすると、検知した push をその場で止める。

```
1. Secret scanning だけ先に有効化      ← いまここ
2. アラートを棚卸し（現在 0 件）
3. Push protection を有効化
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

- **critical 1 件は `Public/IO/BinarySerialize.cs`**（`Deserialization of untrusted data`）。
  **フレームワーク本体であり、要検討**
- 上位の多くは**サンプルの同梱 JS（`jquery.validate*.js`）と `Web.config`** に集中している。
  サンプルであることを踏まえた判断が要る

**未トリアージ。** 対応方針は別途決める。

---

## 2. ブランチと保護

| ブランチ | 役割 | 保護 |
|---|---|---|
| `develop` | 既定ブランチ。開発の集約先 | force-push 禁止 / 削除禁止 |
| `master` | リリース | 上記 ＋ **レビュー 1 名必須** |
| `deps` | Dependabot PR の受け先 | なし |

ブランチ運用は git-flow。**規約は [`Contributing.ja.md`](Contributing.ja.md)。**

### 現状の弱点

- **`develop` に必須ステータス チェックが無い。** CI は `push: [deps]` でしか動かないため、
  `develop` へ入る変更は CI で検証されていない
- `master` の `enforce_admins` は無効（少人数運用のため意図的）
- `delete_branch_on_merge` は無効。マージ済みブランチが残る

> **squash merge / rebase merge を許可している。**
> ただし **squash はコミットを消すため、NuGet パッケージの Source Link を壊す**
> （[`root/programs/CS/NuGet/README.md`](root/programs/CS/NuGet/README.md) 7 節）。
> `master` へは `--no-ff` で入れること。**運用で担保している。**

---

## 3. ワークフロー

| ファイル | 発火 | 内容 |
|---|---|---|
| [`build-windows.yml`](.github/workflows/build-windows.yml) | `push: [deps]` / 手動 | 検証 3 本（ビルド・単体テスト・疎通）を windows-latest で |
| [`dependabot-retarget.yml`](.github/workflows/dependabot-retarget.yml) | `pull_request_target` | Dependabot PR の向き先を `deps` へ変更 |
| CodeQL（既定セットアップ） | GitHub 管理 | ワークフロー ファイルを持たない |

`build-windows.yml` は既知の署名エラー（`MSB3482` / `MSB3325` / `MSB3321`）を除外する。
**ローカルと CI で出るコードが違う**理由はファイル冒頭のコメントにある。

---

## 4. Dependabot

- **version updates はリポジトリに `.github/dependabot.yml` を置いていない**（GitHub UI 側の設定で稼働）
- PR は `dependabot-retarget.yml` が `deps` へ向け直す
- `deps` で CI を通してから `develop` へまとめてマージする

> **`dependabot.yml` を置けば `target-branch: deps` を直接指定でき、
> retarget ワークフローが不要になる可能性がある。**
> ただし retarget の実挙動が未検証のため保留（#517）。

---

## 5. 有効にしていない機能

| 機能 | 判断 |
|---|---|
| **Discussions** | **無効のまま。** Issue で回っており、有効にすると窓口が分散する |
| Projects | 有効だが未使用 |
| Wiki | 有効。[リリース エンジニアリング等の文書](https://github.com/OpenTouryoProject/OpenTouryo/wiki)を置いている |
| Auto-merge | 無効 |
| Web commit signoff | 無効 |

---

## 6. 実行したコマンドの記録

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

### 次に Push protection を有効化するとき

**構成の値を変えるだけでよい**（リポジトリ個別の設定は触らない）。

```bash
gh api -X PATCH orgs/OpenTouryoProject/code-security/configurations/265927 \
  -f secret_scanning_push_protection=enabled
```

---

## 7. 未着手の提案

| | 内容 |
|---|---|
| **`develop` の CI 必須化** | `build-windows.yml` の `on: push` に `develop` を追加し、ブランチ保護で必須チェックにする |
| **Push protection の有効化** | Secret scanning のアラートが 0 件で安定していることを確認してから |
| `delete_branch_on_merge` | マージ済みブランチを自動削除する |
| `SECURITY.md` | Private vulnerability reporting は有効にしたが、文書は未整備 |
| Issue / PR テンプレート | 「調査 → 実装 → 検証」の型が定まっているのでテンプレート化できる |
| `.github/dependabot.yml` | #517 の決着後 |
| **CodeQL アラート 30 件のトリアージ** | 特に critical 1 件（`BinarySerialize.cs`） |
