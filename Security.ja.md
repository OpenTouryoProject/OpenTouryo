# セキュリティ ポリシー

英語版は [SECURITY.md](SECURITY.md) を参照。

## 対象バージョン

**セキュリティ修正は、最新のリリース系統にのみ適用する。**

| バージョン | 対象 |
|---|---|
| 3.0.x | :white_check_mark: |
| 2.x | :x: |

プレリリース版（`-preview*` / `-alpha*`）は評価用であり、対象外。

## 脆弱性の報告

**セキュリティに関する問題を、公開の Issue に書かないでください。**

**[Private vulnerability reporting](https://github.com/OpenTouryoProject/OpenTouryo/security/advisories/new)**
を使ってください。修正が公開されるまで非公開のまま扱われ、やり取りも同じ場所で行えます。

報告には次を含めてください。

- **どのアセンブリの、どのバージョンか**（例: `OpenTouryo.Public.Security` 3.0.0）
- **どのターゲット フレームワークか**（`net48` / `net10.0`）。
  **両者は複数の箇所で別実装**であり、**片方にしか存在しない問題**があり得る
- 再現手順、または問題があると考えるコード パス
- 攻撃者が何を得られるか

少人数で開発しているため、**受領と対応方針は連絡しますが、期限の確約はできません。**

## 対象範囲

本リポジトリには、フレームワーク本体と、その使い方を示すサンプルの両方が含まれる。

| パス | 範囲 |
|---|---|
| `root/programs/CS/Frameworks/Infrastructure/` | **対象。** NuGet パッケージとして配布しているもの |
| `root/programs/CS/Frameworks/Tools/` | **対象** |
| `root/programs/CS/Samples/`、`Samples4NetCore/`、`root/programs/VB/` | サンプル。**報告は歓迎する**が、教材であり配布物ではない |
| `root/files/resource/X509/` | **対象外。** **テスト専用**の自己署名証明書と秘密鍵 |

## 既知であり、意図的なもの

**本リポジトリでは静的解析（CodeQL）を実施し、検出結果をトリアージ済みである。**
スキャナの結果を報告する前に、
**[#536](https://github.com/OpenTouryoProject/OpenTouryo/issues/536)** を確認してください。
**何を修正し、何を棄却し、その理由は何か**を記録している。

次は既知であり、意図的なものである。

- **`CipherMode_ECB`** には `[Obsolete]` を付与済み。利用者が選べる 5 つの暗号モードの 1 つで、
  **既定ではない**（モードを指定しなければ .NET の既定である CBC が使われる）。
  **下位互換のために残している**
- **`BinarySerialize`**（`BinaryFormatter`）は **`net48` のみ**に存在する。
  `net10.0` のビルドからは除外している（`Public_netcore100.csproj` の `<Compile Remove>`）
- **サンプルの `Web.config` が `requireSSL="false"`** なのは、
  **サンプルを HTTP で動かす前提**のため。本番用の設定は、その隣にコメントアウトで併記し、
  有効にするよう注記してある

**上記について具体的な攻撃が成立することを示す報告**は、引き続き歓迎する。

## 本リポジトリでの取り組み

| | |
|---|---|
| Secret scanning ＋ Push protection | 有効 |
| Code scanning（CodeQL） | 有効。`csharp` / `javascript-typescript` / `actions` |
| Dependabot alerts / security updates | 有効 |
| Private vulnerability reporting | 有効 |
| ブランチ保護（`master`） | レビュー必須 ＋ CI の成功が必須 |

設定の実体は GitHub 側にあり、**リポジトリのファイルからは見えない**ため、
[`GitHubUsage.md`](GitHubUsage.md) に記録している。
