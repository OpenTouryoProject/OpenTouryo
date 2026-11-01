# Wiki のサイトの公開

GitHub Wiki には `x-robots-tag: none` が付き、検索エンジンにインデックスされない。
そこで、Wiki を原本にしたまま、GitHub Pages に静的サイトとして公開する。

- 公開先：<https://opentouryoproject.github.io/OpenTouryo/>
- 変換：`build_site.py`（Wiki のファイルは書き換えない。LLM は使わない）
- 実行：[`publish-wiki-site.yml`](../workflows/publish-wiki-site.yml)（Wiki の更新、このフォルダの更新、手動）

**`.github/` の下に置く理由：** `SelectTargets.ps1` は `.github/` の下を「影響なし」とする。
ルートの `tools/` などに置くと、対応が引けずに通しの検証を求められる。

## 公開するもの

| 対象 | 扱い |
| --- | --- |
| ページ（`*.md`） | 公開する。出力は平らに置く（ページ名は一意） |
| 添付・画像 | ページから参照されたものだけ、`images/...` のパスを保って公開する（同じ名前が別のフォルダにある） |
| `AGENTS`・`CLAUDE`・`DONE`・`TODO`・`WATCH`・`README`、先頭が `_` のファイル | 公開しない。リンクは Wiki へ向ける（`_Footer.md` はフッタ、`_Sidebar.md` は目次に使う） |

- `<html lang>` は、ページ名の末尾が `.ja` なら `ja`、それ以外は `en`
- `_Sidebar.md` が無ければ、目次の枠を出さない。本文が幅いっぱいになる
- トップ（`index.html`）は `Home.md`。日本語のページのヘッダは `Home.ja.html` へ戻る

## 導入の手順（1 回だけ。人の作業）

1. Settings → Pages → Source を「GitHub Actions」にする
2. このワークフローが `develop` に入ったら、Actions で `publish-wiki-site` を手動で実行し、公開を確かめる。
   **`gollum` は既定ブランチ（`develop`）の定義で動く**ので、入る前は発火しない
3. Wiki を `git push` で更新し、`publish-wiki-site` が動くかを確かめる。動かなければ、週次の `schedule` を足す
4. Google Search Console に URL プレフィックスのプロパティを追加する
   - 確認の方法に HTML タグを選び、コードを Actions の変数 `GOOGLE_SITE_VERIFICATION` に入れて、もう一度実行する
   - `sitemap.xml` を登録する（<https://opentouryoproject.github.io/OpenTouryo/sitemap.xml>）。
     プロジェクト サイトでは `robots.txt` が効かないので、ここで登録する

## 手元で試す

```powershell
git clone https://github.com/OpenTouryoProject/OpenTouryo.wiki.git <Wiki のクローン>
pip install -r .github\wiki-site\requirements.txt
python .github\wiki-site\build_site.py --wiki <Wiki のクローン> --out <出力先> `
  --base-url https://opentouryoproject.github.io/OpenTouryo/ --report <レポート>.json
python -m http.server 8000 --directory <出力先>
```

`--report` の JSON に、リンク切れ、アンカーの切れ、崩れた太字、参照されていない添付が出る。
Actions では `build-report` の成果物に入る。**リンク切れがあっても、公開は止めない**（原本の Wiki を直す）。
