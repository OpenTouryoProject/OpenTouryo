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

### Git 操作は行わない（状態を変える操作をしない）

**成果物の検収は人が行う。** エージェントは作業結果をワーキング ツリーに残すところまでを担当し、
Git 操作は人が手動で行う。

したがって、指示がない限り次を実行してはならない。

- `git add` / `commit` / `push`（検収前・未レビューの変更を確定・送信しない）
- `git checkout` / `switch` / `branch` / `reset` / `restore` / `stash`（人の作業状態や未保存の作業を壊す）

**参照系は制限しない。** 何を変更したかを正確に報告するために必要なため、次は自由に実行してよい。

- `git status` / `diff`（`--cached` 含む）/ `log` / `show` / `ls-files` / `check-ignore` / `blame`

作業が完了したら**何を変更したかを報告するに留める**。コミットの要否とタイミングは人が判断する。

<!--
  補足（執筆者向け）:
  インストラクションは「文脈」であって強制力を持たない。上記は遵守されやすい書き方に
  しているが、確実に阻止したい場合は仕組み側で塞ぐ必要がある。
    - Claude Code : PreToolUse フックで Bash(git commit:*) 等を deny する
    - 各プロダクト: 同等の機構があればそれを使う
  必要になったら install.ps1 に設定の配布を追加することを検討する。
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

**このリポジトリでは `OsscJpDevInfra` アカウントを使用する。**
`gh auth status` に複数のアカウントが登録されていることがあるため、
投稿前にアクティブなアカウントを確認すること。異なる場合は `gh auth switch` で切り替える。

```
gh auth status                        # アクティブなアカウントの確認
gh auth switch --user OsscJpDevInfra  # 異なる場合は切り替え
```

**Issue のクローズ・ラベル変更・アサイン、PR の作成やマージは人が行う。**
これらは成果物の検収と同じ扱いとし、エージェントは提案に留める。

### コーディング規約は、領域ごとの ANALYSIS.md に従う

`root/programs/CS/` 配下は領域ごとに基準が異なる。
**コードを変更する前に、作業対象を所管する `ANALYSIS.md` を読むこと。**

| 作業対象 | 読む文書 |
|---|---|
| `root/programs/CS/Frameworks/` | [`Frameworks/ANALYSIS.md`](root/programs/CS/Frameworks/ANALYSIS.md) |
| `root/programs/CS/Samples/` | [`Samples/ANALYSIS.md`](root/programs/CS/Samples/ANALYSIS.md) |
| `root/programs/CS/Samples4NetCore/` | [`Samples4NetCore/ANALYSIS.md`](root/programs/CS/Samples4NetCore/ANALYSIS.md) |

各 `ANALYSIS.md` が、その領域におけるアーキテクチャ・ビルド手順・条件コンパイル・
コーディング規約・既知の落とし穴の**一次情報**である。末尾の
「エージェント向け作業チェックリスト」には着手前に目を通すこと。

ファイル ヘッダの書式と**更新者名**、新規ファイルにおける Copyright ブロックの扱い、
`.bat` の文字コードなど、**エージェントが見落としやすい指定**が含まれる。

規約の実体はこのファイル（AGENTS.md）には書かない。領域ごとに基準が分かれるため、
二重管理になり、どちらが正なのか分からなくなる。

### ビルド・テスト・リリースの検証は、専用の文書に従う

変更を加えたあとの**検証**は、次の文書が一次情報である。

| 目的 | 読む文書 |
|---|---|
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

**`.ps1` は Windows PowerShell 5.1 と PowerShell 7 の両方で動くこと。**
エージェントの実行環境は 7 だが、利用者は 5.1（`powershell.exe`）で実行するため、
7 だけで確認すると 5.1 で落ちる。**変更したら 5.1 でも実行して確かめること。**

規約の実体は
[`Frameworks/ANALYSIS.md`](root/programs/CS/Frameworks/ANALYSIS.md) の
**8.4 節「ps1 ファイルの文字コードと、PowerShell 5.1 / 7 の両対応」**にある。

前提となるサービスや DB の状態が足りない場合は、**勝手に変えず、対処方法とともに報告する。**

### 付属ツールを CLI で使うときは、各ツールの README に従う

エージェントから実行できる（非対話の）ツールには README を置く。

| ツール | 読む文書 |
|---|---|
| `DaoGen_Tool`（Ｄ層自動生成ツール／墨壺） | [`README.md`](root/programs/CS/Frameworks/Tools/DaoGen_Tool/README.md) |

**引数の一覧は README に書かない。** ツールの `/HELP` が一次情報であり、
書き写すと二重管理になる。README には README にしか書けないこと
（実行ファイルの場所、ヘルプの出し方、踏みやすい罠、前提）を置く。

- **引数を組み立てる前に、まず `/HELP` を実行する**
- 終了コードだけで判断せず、**生成物の存在も確認する**
  （パス区切りを誤ると、成功（`0`）を返しつつ別の場所に出力される）

