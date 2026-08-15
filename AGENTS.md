# プロジェクト ポリシー
アプリケーション・フレームワーク

https://github.com/OpenTouryoProject/OpenTouryo/

と、ソレを使用したサンプル・アプリケーションの開発エージェント用。

環境のセットアップとビルド手順は [root/Readme.ja.md](root/Readme.ja.md) を参照。
本書は、その上で**エージェントが守るべきこと**と、**どの文書を見るか**を示す。

**プロジェクト共通の投稿規約は [Contributing.ja.md](Contributing.ja.md) に従う。**
コメント量の目安、クロスコンパイルと下位互換の方針、ブランチ運用（git-flow）、
"プルリクエスト" の粒度は、人もエージェントも同じ規約による。
本書は、そこに書かれていない**エージェント固有の制約**を扱う。

> **手順を思い出したいだけなら [`CHEATSHEET.md`](root/programs/CHEATSHEET.md) を見る。**
> 検証・リリース・VB ビルド・ツール・規約の要点と、よく踏む落とし穴を 1 枚にまとめてある。
> **理由や判断が要ることは書いていない**ので、迷ったらそこからリンク先へ辿ること。

### Git 操作は行わない（状態を変える操作をしない）

**成果物の検収は人が行う。** エージェントは作業結果をワーキング ツリーに残すところまでを担当し、
Git 操作は人が手動で行う。

したがって、指示がない限り次を実行してはならない。

- `git add` / `commit` / `push`（検収前・未レビューの変更を確定・送信しない）
- `git checkout` / `switch` / `branch` / `reset` / `restore` / `stash`（人の作業状態や未保存の作業を壊す）

**参照系は制限しない。** 何を変更したかを正確に報告するために必要なため、次は自由に実行してよい。

- `git status` / `diff`（`--cached` 含む）/ `log` / `show` / `ls-files` / `check-ignore` / `blame`

作業が完了したら**何を変更したかを報告するに留める**。コミットの要否とタイミングは人が判断する。

**状態を報告する直前に、必ず取り直すこと。** 前のターンの出力や記憶から書かない。

```
git status --porcelain                      # 未コミットの変更
gh issue view <番号> / gh pr view <番号>    # Issue / PR の状態
gh api ...                                  # 設定・アラートの状態
```

**人はエージェントの報告とは独立にコミットし、Issue や PR を操作する。**
数ターン前の状態は、高い確率で古い。
**古い一覧を出すと「まだ残っている」と誤認させ、検収の判断材料そのものが誤りになる。**

参照系は制限していないので、回数を惜しむ理由はない。
**0 件なら「ワーキング ツリーはクリーン」と書く。前回の一覧を再掲しない。**

<!--
  補足（執筆者向け）:
  インストラクションは「文脈」であって強制力を持たない。上記は遵守されやすい書き方に
  しているが、確実に阻止したい場合は仕組み側で塞ぐ必要がある。
    - Claude Code : PreToolUse フックで Bash(git commit:*) 等を deny する
    - 各プロダクト: 同等の機構があればそれを使う
-->

### GitHub 操作は gh で行う

Issue の調査・起票・コメントは、**`gh` コマンドで実行してよい**（ブラウザ操作を人に依頼しない）。
上記の「Git 操作は行わない」はワーキング ツリーとコミット履歴に対する制約であり、
GitHub 側のやり取りは対象外。

```
gh issue view <番号> --repo OpenTouryoProject/OpenTouryo
gh issue list --repo OpenTouryoProject/OpenTouryo
gh issue comment <番号> --repo OpenTouryoProject/OpenTouryo --body-file <path>
gh issue create --repo OpenTouryoProject/OpenTouryo --title <title> --body-file <path>
```

ただし**公開リポジトリへの投稿は取り消しにくい**ため、次を守ること。

- **投稿前に文面を提示し、承認を得てから実行する。** 承認なしに投稿しない
- 本文は一時ファイルに書き、`--body-file` で渡す（改行・記号の欠落を避ける）
- 投稿後は URL を報告する

**テンプレートは自動では適用されない。読んで、その構成に沿って書くこと。**

`gh issue create` / `gh pr create` の `--template` は「エディタで編集する前提の
開始テキスト」であり、**`--body-file` と併用すると本文で上書きされる。**
エージェントは `--body-file` を使うため、テンプレートは効かない。

```
.github/ISSUE_TEMPLATE/bug.md          不具合
.github/ISSUE_TEMPLATE/enhancement.md  機能追加・改善
.github/ISSUE_TEMPLATE/quality.md      品質改善（リファクタリング・規約・CI・文書）
.github/pull_request_template.md       PR
```

**テンプレートは任意**（`blank_issues_enabled: true`）だが、
**「利用者への影響」は、無いなら「無し」と明記する。**
空欄だと、確認したのか未確認なのかが読み手に分からない。

### GitHub Actions を書き足すとき

**ワークフローはエージェントが書き足すことが多い。厳し目に倒すこと。**

- **`permissions:` を必ず書く。** 既定（`read`）に頼らない。
  何を必要としているかが、ファイルを見て分かる方がよい
- **第三者製のアクションは SHA で固定する。**
  `@v7` のようなタグは**作者側で別のコミットへ付け替えられる**ため、
  こちらが何も変えていなくても動くコードが変わる
- **`pull_request_target` を使うなら、PR のコードを `checkout` しない。**
  ベース側の定義を書き込み権限付きで動かすトリガであり、
  PR に任意のコードを書ける相手へ権限を渡すことになる（pwn request）
- 新しいアクションを増やすときは、**本当に必要かを先に検討する**

現状と方針は [`GitHubUsage.md`](GitHubUsage.md) 3 節・8 節。

**このリポジトリでは `OsscJpDevInfra` アカウントを使用する。**
`gh auth status` に複数のアカウントが登録されていることがあるため、
投稿前にアクティブなアカウントを確認すること。異なる場合は `gh auth switch` で切り替える。

```
gh auth status                        # アクティブなアカウントの確認
gh auth switch --user OsscJpDevInfra  # 異なる場合は切り替え
```

**Issue のクローズ・ラベル変更・アサイン、PR の作成やマージは人が行う。**
これらは成果物の検収と同じ扱いとし、エージェントは提案に留める。

**PR のレビューは、`--comment` だけ行ってよい。**

| 操作 | 誰が行うか |
|---|---|
| `gh pr review --comment` | **エージェント可**（文面を提示し、承認を得てから） |
| `gh pr review --approve` / `--request-changes` | **人のみ。検収に当たる** |
| PR の作成・マージ | **人のみ** |

`--comment` は `COMMENTED` として記録され、**必須レビューを満たさない**。
`reviewDecision` は `REVIEW_REQUIRED` のまま変わらないので、
**マージを進めてしまう心配は無い。**

> **承認が必須レビューとして数えられるのは `write` 以上の人だけ。**
> 公開リポジトリなので誰でも `read` は持つが、それでは足りない。
> **自分が出した PR は、自分で承認できない。**
> レビュアーの構成は [`GitHubUsage.md`](GitHubUsage.md) 2 節。

### コーディング規約は CODING.md、領域ごとの事情は ANALYSIS.md に従う

**規約は全領域に共通、分析は領域ごと**に分かれている。

| 内容 | 読む文書 |
|---|---|
| **コーディング規約（全領域共通）** | [`CODING.md`](root/programs/CODING.md) |
| **設定ファイルの扱い（全領域共通）** | [`Configuration.md`](root/programs/Configuration.md) |
| `root/programs/CS/Frameworks/` の分析 | [`Frameworks/ANALYSIS.md`](root/programs/CS/Frameworks/ANALYSIS.md) |
| `root/programs/CS/Samples/` の分析 | [`Samples/ANALYSIS.md`](root/programs/CS/Samples/ANALYSIS.md) |
| `root/programs/CS/Samples4NetCore/` の分析 | [`Samples4NetCore/ANALYSIS.md`](root/programs/CS/Samples4NetCore/ANALYSIS.md) |

**設定ファイルに「仕組み」を書き足したくなったら、[`Configuration.md`](root/programs/Configuration.md)
に書く。** 設定ファイルは 86 個あり、一部にだけ詳しい説明があると、
**書かれていないファイルを「該当しない」と誤読させる。**
値の隣に書くのは「そのキーが何か」までにする（分担は同書 0 節）。

**コードを変更する前に、[`CODING.md`](root/programs/CODING.md) と、
作業対象を所管する `ANALYSIS.md` の両方を読むこと。**

`CODING.md` には、ファイル ヘッダの書式と**更新者名**、新規ファイルにおける
Copyright ブロックの扱い、`ArgumentException` 系の引数の順、`.bat` / `.ps1` の
文字コードなど、**エージェントが見落としやすい指定**が含まれる。

各 `ANALYSIS.md` は、その領域におけるアーキテクチャ・ビルド手順・条件コンパイル・
既知の落とし穴の**一次情報**である。末尾の
「エージェント向け作業チェックリスト」には着手前に目を通すこと。

規約の実体はこのファイル（AGENTS.md）には書かない。二重管理になり、
どちらが正なのか分からなくなる。

### ビルド・テスト・リリースの検証は、専用の文書に従う

変更を加えたあとの**検証**は、次の文書が一次情報である。

| 目的 | 読む文書 |
|---|---|
| **手順だけを引く（早見）** | [`CHEATSHEET.md`](root/programs/CHEATSHEET.md) |
| リリース時の作業全体 | [`RELEASE.md`](root/programs/RELEASE.md) |
| 全ビルドの実行と判定 | [`BUILDING.md`](root/programs/BUILDING.md) |
| 単体テストの実行と判定 | [`TESTING.md`](root/programs/TESTING.md) |
| サンプルの疎通確認 | [`SMOKETEST.md`](root/programs/SMOKETEST.md) |

検証は次の 3 本で、いずれも終了コードで合否が分かる。**この順で実行すること。**

```powershell
cd root\programs
.\1_BuildAll.ps1                 # 全ビルド
.\2_RunAllTests.ps1              # 単体テスト
.\3_SmokeTest.ps1                # サンプルの疎通
```

`2_RunAllTests.ps1` はワーキング ツリーの `Result*.txt` を書き換える（従来のバッチ運用と同じ）。
**コミットの要否は人が判断する**ため、エージェントは差分を報告するに留める。

**上記の既定は C# 側である。VB 側に手を入れたときは `-Lang` で回す。**

```powershell
.\0_RunAll.ps1 -Lang VB          # 1 と 3 を VB で通す（2 は VB に対象が無い）
```

理由と対象は [`BUILDING.md`](root/programs/BUILDING.md) 10 節・
[`SMOKETEST.md`](root/programs/SMOKETEST.md) 10 節。

**`.ps1` は Windows PowerShell 5.1 と PowerShell 7 の両方で動くこと。**
エージェントの実行環境は 7 だが、利用者は 5.1（`powershell.exe`）で実行するため、
7 だけで確認すると 5.1 で落ちる。**変更したら 5.1 でも実行して確かめること。**

規約の実体は
[`Frameworks/ANALYSIS.md`](root/programs/CS/Frameworks/ANALYSIS.md) の
**8.5 節「ps1 ファイルの文字コードと、PowerShell 5.1 / 7 の両対応」**にある。

前提となるサービスや DB の状態が足りない場合は、**勝手に変えず、対処方法とともに報告する。**

### 付属ツールを CLI で使うときは、各ツールの README に従う

エージェントから実行できる（非対話の）ツールには README を置く。

| ツール | 読む文書 |
|---|---|
| `DaoGen_Tool`（Ｄ層自動生成ツール／墨壺） | [`README.md`](root/programs/CS/Frameworks/Tools/DaoGen_Tool/README.md) |
| `DeployZipPackWithHTTP`（ZIP パッケージの HTTP 配布） | [`README.md`](root/programs/CS/Frameworks/Tools/DeployZipPackWithHTTP/README.md) |

**引数の一覧は README に書かない。** ツールの `/HELP` が一次情報であり、
書き写すと二重管理になる。README には README にしか書けないこと
（実行ファイルの場所、ヘルプの出し方、踏みやすい罠、前提）を置く。

- **引数を組み立てる前に、まず `/HELP` を実行する**
- 終了コードだけで判断せず、**生成物の存在も確認する**
  （パス区切りを誤ると、成功（`0`）を返しつつ別の場所に出力される）

