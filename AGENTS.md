# プロジェクト ポリシー
アプリケーション・フレームワーク

https://github.com/OpenTouryoProject/OpenTouryo/

と、ソレを使用したサンプル・アプリケーションの開発エージェント用。

### Git 操作は行わない

**成果物の検収は人が行う。** エージェントは作業結果をワーキング ツリーに残すところまでを担当し、
Git 操作は人が手動で行う。

したがって、指示がない限り次を実行してはならない。

- `git add` / `commit` / `push`（検収前・未レビューの変更を確定・送信しない）
- `git checkout` / `switch` / `branch` / `reset` / `restore` / `stash`（人の作業状態や未保存の作業を壊す）

作業が完了したら**何を変更したかを報告するに留める**。コミットの要否とタイミングは人が判断する。

<!--
  補足（執筆者向け）:
  インストラクションは「文脈」であって強制力を持たない。上記は遵守されやすい書き方に
  しているが、確実に阻止したい場合は仕組み側で塞ぐ必要がある。
    - Claude Code : PreToolUse フックで Bash(git commit:*) 等を deny する
    - 各プロダクト: 同等の機構があればそれを使う
  必要になったら install.ps1 に設定の配布を追加することを検討する。
-->

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

