#!/usr/bin/env python3
"""GitHub Wiki のリポジトリを、GitHub Pages 用の静的サイトに変換する。

Wiki が原本で、サイトはその写しである。Wiki のファイルは書き換えない。

- Wiki のページ名は一意なので、全ページを 1 つのフォルダに平らに置く。
- 添付は、別のフォルダで同じ名前が使われているので、images/... のパスを保つ。
- 公開するのはページと、ページから参照された添付だけである。
- <html lang> は、ページ名の末尾が .ja なら ja、それ以外は en にする。
- 作業用の文書（EXCLUDED_PAGES）は公開せず、そこへのリンクは Wiki へ向ける。

使い方:
    python build_site.py --wiki <Wiki のクローン> --out <出力先> --base-url <公開 URL>
"""

import argparse
import html
import json
import posixpath
import re
import shutil
import subprocess
import sys
import unicodedata
from collections import Counter
from pathlib import Path
from urllib.parse import quote, unquote

import nh3
from markdown_it import MarkdownIt
from mdit_py_plugins.footnote import footnote_plugin
from mdit_py_plugins.tasklists import tasklists_plugin

SITE_NAME = "OpenTouryo Wiki"
WIKI_URL = "https://github.com/OpenTouryoProject/OpenTouryo/wiki"

# 公開しない作業用の文書（小文字）。先頭が "_" のファイルも公開しない。
EXCLUDED_PAGES = {"agents", "claude", "done", "todo", "watch", "readme"}

ALLOWED_TAGS = set(nh3.ALLOWED_TAGS) | {
    "details", "summary", "input", "kbd", "section", "del", "s", "hr", "br",
}
ALLOWED_ATTRIBUTES = {tag: set(attrs) for tag, attrs in nh3.ALLOWED_ATTRIBUTES.items()}
ALLOWED_ATTRIBUTES.setdefault("*", set()).update({"id", "class"})
ALLOWED_ATTRIBUTES.setdefault("a", set()).update({"href", "title", "name"})
ALLOWED_ATTRIBUTES.setdefault("img", set()).update({"src", "alt", "title", "width", "height"})
ALLOWED_ATTRIBUTES.setdefault("details", set()).add("open")
ALLOWED_ATTRIBUTES.setdefault("input", set()).update({"type", "checked", "disabled"})
for cell in ("th", "td"):
    ALLOWED_ATTRIBUTES.setdefault(cell, set()).update({"style", "align", "colspan", "rowspan"})

FENCE = re.compile(r"^\s*(?:>\s*)*(```|~~~)")
TABLE_DELIMITER = re.compile(r"^\s*(?:>\s*)*\|?\s*:?-+:?\s*(?:\|\s*:?-+:?\s*)*\|?\s*$")
ATTRIBUTE_URL = re.compile(r'\b(href|src)="([^"]*)"')
UNRENDERED_BOLD = re.compile(r">[^<]*\*\*[^*<]+\*\*[^<]*<")
CODE_ELEMENT = re.compile(r"<pre.*?</pre>|<code>.*?</code>", re.DOTALL)
# Wiki は <a name="..."> をアンカーに使っているので、name も数える。
ANY_ID = re.compile(r'\b(?:id|name)="([^"]*)"')
HAS_SCHEME = re.compile(r"^[a-zA-Z][a-zA-Z0-9+.-]*:")
WIKI_PREFIX = re.compile(
    r"^https?://github\.com/OpenTouryoProject/OpenTouryo/wiki/?",
    re.IGNORECASE,
)

# サイトが足す文言（Wiki の本文は、そのまま）。
LABELS = {
    "ja": {"source": "原本（GitHub Wiki）で見る", "nav": "サイトの目次"},
    "en": {"source": "View the original (GitHub Wiki)", "nav": "Site navigation"},
}


def page_language(stem):
    return "ja" if stem.lower().endswith(".ja") else "en"


def separate_tables(text):
    """段落の直後の表の前に空行を入れる（GitHub は表として描画するが、markdown-it は描画しない）。"""
    lines = text.splitlines()
    result = []
    fence = None
    for index, line in enumerate(lines):
        marker = FENCE.match(line)
        if marker:
            mark = marker.group(1)[0]
            fence = None if fence == mark else (fence or mark)
        following = lines[index + 1] if index + 1 < len(lines) else ""
        previous = lines[index - 1] if index else ""
        if (
            fence is None
            and "|" in line
            and TABLE_DELIMITER.match(following)
            and previous.strip()
            and "|" not in previous
        ):
            result.append("")
        result.append(line)
    return chr(10).join(result) + chr(10)


def create_markdown():
    """GitHub の表記（表・取り消し線・URL の自動リンク・脚注・タスク）に合わせる。"""
    md = MarkdownIt("gfm-like", {"linkify": True, "html": True, "breaks": False})
    md.linkify.set({"fuzzy_url": False, "fuzzy_email": False, "fuzzy_link": False})
    md.use(footnote_plugin).use(tasklists_plugin)
    return md


def slugify(text, used):
    """GitHub の見出しのアンカーと同じ形にする（同じ見出しには -1, -2 と付ける）。"""
    kept = "".join(
        ch for ch in text.strip().lower()
        if ch in " -_" or unicodedata.category(ch)[0] in "LNM"
    )
    base = kept.replace(" ", "-")
    slug = base
    while slug in used:
        used[base] = used.get(base, 0) + 1
        slug = f"{base}-{used[base]}"
    used.setdefault(slug, 0)
    return slug


def inline_text(token):
    return "".join(c.content for c in (token.children or []) if c.type in ("text", "code_inline"))


def add_heading_ids(tokens):
    used = {}
    for index, token in enumerate(tokens):
        if token.type == "heading_open":
            token.attrSet("id", slugify(inline_text(tokens[index + 1]), used))


def page_title(tokens, fallback):
    for index, token in enumerate(tokens):
        if token.type == "heading_open" and token.tag == "h1":
            return inline_text(tokens[index + 1]).strip() or fallback
    return fallback


def page_description(tokens, limit=120):
    """箇条書き（パンくず）を除いた、最初の段落を要約として使う。"""
    depth = 0
    for index, token in enumerate(tokens):
        if token.type in ("bullet_list_open", "ordered_list_open", "blockquote_open", "table_open"):
            depth += 1
        elif token.type in ("bullet_list_close", "ordered_list_close", "blockquote_close", "table_close"):
            depth -= 1
        elif token.type == "paragraph_open" and depth == 0:
            text = " ".join(inline_text(tokens[index + 1]).split())
            if text:
                return text[:limit]
    return ""


class Site:
    def __init__(self, wiki, out, base_url, verification):
        self.wiki = wiki
        self.out = out
        self.base_url = base_url.rstrip("/") + "/"
        self.verification = verification
        self.md = create_markdown()
        self.pages = {}          # 小文字のページ名 -> 相対パス
        self.excluded = set()    # 公開しないページの小文字の名前
        self.assets = {}         # 小文字の Wiki からの相対パス -> 相対パス
        self.used_assets = {}    # 相対パス（/ 区切り） -> 相対パス
        self.asset_case_mismatch = set()  # 大文字小文字が実物と違う参照（Wiki では切れる）
        self.ids = {}            # ページ名 -> アンカーの集合
        self.anchor_refs = []    # (参照元, 参照先, アンカー)
        self.broken = Counter()
        self.broken_fragments = set()  # 同じページの、見出しに無いアンカー
        self.unrendered_bold = []      # 太字にならなかった ** が残るページ

    def scan(self):
        listed = subprocess.run(
            ["git", "-c", "core.quotepath=off", "-C", str(self.wiki), "ls-files", "-z"],
            check=True, capture_output=True, text=True, encoding="utf-8",
        ).stdout
        duplicated = []
        for rel in filter(None, listed.split("\0")):
            path = Path(rel)
            if (self.wiki / path).is_symlink():
                continue
            if path.suffix.lower() == ".md":
                stem = path.stem.lower()
                if stem in EXCLUDED_PAGES or stem.startswith("_"):
                    self.excluded.add(stem)
                elif stem in self.pages:
                    duplicated.append(rel)
                else:
                    self.pages[stem] = path
            elif not path.name.startswith(("_", ".")):
                self.assets[path.as_posix().lower()] = path
        if duplicated:
            raise SystemExit("ファイル名が一意ではありません: " + ", ".join(duplicated))
        if "home" not in self.pages:
            raise SystemExit("Home.md がありません")

    def output_name(self, stem):
        return "index.html" if stem == "home" else f"{self.pages[stem].stem}.html"

    def resolve_fragment(self, url, source, page_ids):
        """同じページのアンカーを、見出しの ID に合わせる（大文字小文字の違いを吸収する）。"""
        anchor = unquote(url[1:])
        if anchor in page_ids:
            return url
        if anchor.lower() in page_ids:
            return "#" + anchor.lower()
        if anchor:
            self.broken_fragments.add(f"{source} -> {url}")
        return url

    def resolve(self, url, source, page_ids):
        """Wiki 形式のリンクを、サイトのリンクに直す。"""
        url = WIKI_PREFIX.sub("", url) if WIKI_PREFIX.match(url) else url
        if not url:
            return "index.html"
        if url.startswith("#"):
            return self.resolve_fragment(url, source, page_ids)
        if url.startswith("//") or HAS_SCHEME.match(url):
            return url

        target, hash_mark, anchor = url.partition("#")
        target = target.partition("?")[0]
        name = unquote(target).replace("\\", "/").rstrip("/").rsplit("/", 1)[-1]
        stem = name[:-3] if name.lower().endswith(".md") else name
        key = stem.lower()
        for candidate in (key, key.replace(" ", "-")):
            if candidate in self.pages:
                self.anchor_refs.append((source, candidate, unquote(anchor)))
                return self.output_name(candidate) + hash_mark + anchor
        # 添付は Wiki の根からのパスで引く（./images/... はどのページからでも同じ場所を指す）。
        written = posixpath.normpath(unquote(target).replace("\\", "/"))
        asset = self.assets.get(written.lower())
        if asset is not None:
            if asset.as_posix() != written:
                self.asset_case_mismatch.add(f"{source} -> {url}")
            self.used_assets[asset.as_posix()] = asset
            return quote(asset.as_posix()) + hash_mark + anchor
        if key not in self.excluded:
            self.broken[f"{source} -> {url}"] += 1
        return f"{WIKI_URL}/{quote(stem)}{hash_mark}{anchor}"

    def render(self, text, source):
        tokens = self.md.parse(separate_tables(text))
        add_heading_ids(tokens)
        body = self.md.renderer.render(tokens, self.md.options, {})
        page_ids = set(ANY_ID.findall(body))
        body = ATTRIBUTE_URL.sub(
            lambda m: f'{m.group(1)}="{html.escape(self.resolve(html.unescape(m.group(2)), source, page_ids))}"',
            body,
        )
        body = nh3.clean(
            body,
            tags=ALLOWED_TAGS,
            attributes=ALLOWED_ATTRIBUTES,
            url_schemes={"http", "https", "mailto"},
            link_rel=None,
            filter_style_properties={"text-align"},
        )
        return tokens, body

    def read(self, rel):
        return (self.wiki / rel).read_text(encoding="utf-8-sig")

    def special(self, name):
        path = self.wiki / name
        if not path.exists():
            return ""
        return self.render(path.read_text(encoding="utf-8-sig"), name)[1]

    def layout(self, title, description, name, stem, body, sidebar, footer):
        canonical = self.base_url + ("" if name == "index.html" else quote(name))
        full_title = SITE_NAME if name == "index.html" else f"{title} - {SITE_NAME}"
        lang = page_language(stem)
        labels = LABELS[lang]
        home = "Home.ja.html" if lang == "ja" and "home.ja" in self.pages else "index.html"
        # _Sidebar.md が無ければ、空の枠を出さない。
        nav = f'<nav class="sidebar" aria-label="{labels["nav"]}">\n{sidebar}\n</nav>\n' if sidebar else ""
        meta = [f'<meta name="description" content="{html.escape(description)}">'] if description else []
        if self.verification:
            meta.append(f'<meta name="google-site-verification" content="{html.escape(self.verification)}">')
        return f"""<!DOCTYPE html>
<html lang="{lang}">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{html.escape(full_title)}</title>
<link rel="canonical" href="{html.escape(canonical)}">
{chr(10).join(meta)}
<link rel="stylesheet" href="style.css">
</head>
<body>
<header class="site-header"><a href="{home}">{SITE_NAME}</a></header>
<div class="site-body">
<main class="content">
{body}
<p class="source"><a href="{WIKI_URL}/{quote(stem)}">{labels["source"]}</a></p>
</main>
{nav}</div>
<footer class="site-footer">{footer}</footer>
</body>
</html>
"""

    def last_modified(self):
        """ファイルごとの最終更新日を、履歴を 1 回だけ読んで取る。"""
        log = subprocess.run(
            ["git", "-c", "core.quotepath=off", "-C", str(self.wiki), "log",
             "--format=%x00%cs", "--name-only"],
            check=True, capture_output=True, text=True, encoding="utf-8",
        ).stdout
        dates, current = {}, ""
        for line in log.splitlines():
            if line.startswith("\0"):
                current = line[1:]
            elif line and line not in dates:
                dates[line] = current
        return dates

    def build(self):
        self.scan()
        if self.out.exists():
            shutil.rmtree(self.out)
        self.out.mkdir(parents=True)

        sidebar = self.special("_Sidebar.md")
        footer = self.special("_Footer.md")
        dates = self.last_modified()
        urls = []

        for stem, rel in sorted(self.pages.items()):
            tokens, body = self.render(self.read(rel), rel.stem)
            self.ids[stem] = set(ANY_ID.findall(body))
            if UNRENDERED_BOLD.search(CODE_ELEMENT.sub("", body)):
                self.unrendered_bold.append(rel.stem)
            name = self.output_name(stem)
            page = self.layout(
                page_title(tokens, rel.stem), page_description(tokens),
                name, rel.stem, body, sidebar, footer,
            )
            (self.out / name).write_text(page, encoding="utf-8", newline="\n")
            url = self.base_url + ("" if name == "index.html" else quote(name))
            urls.append((url, dates.get(rel.as_posix(), "")))

        for name, rel in self.used_assets.items():
            (self.out / name).parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(self.wiki / rel, self.out / name)
        shutil.copyfile(Path(__file__).with_name("style.css"), self.out / "style.css")
        (self.out / ".nojekyll").write_text("", encoding="utf-8")
        self.write_sitemap(urls)
        return self.report(len(urls))

    def write_sitemap(self, urls):
        items = "".join(
            f"<url><loc>{html.escape(url)}</loc>" + (f"<lastmod>{date}</lastmod>" if date else "") + "</url>\n"
            for url, date in urls
        )
        (self.out / "sitemap.xml").write_text(
            '<?xml version="1.0" encoding="UTF-8"?>\n'
            '<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n'
            f"{items}</urlset>\n",
            encoding="utf-8", newline="\n",
        )

    def report(self, page_count):
        broken_anchors = sorted(self.broken_fragments | {
            f"{source} -> {target}#{anchor}"
            for source, target, anchor in self.anchor_refs
            if anchor and anchor not in self.ids.get(target, set())
        })
        size = sum(p.stat().st_size for p in self.out.rglob("*") if p.is_file())
        result = {
            "pages": page_count,
            "assets_published": len(self.used_assets),
            "assets_not_referenced": sorted(
                rel.as_posix() for rel in self.assets.values() if rel.as_posix() not in self.used_assets
            ),
            "asset_case_mismatch": sorted(self.asset_case_mismatch),
            "broken_links": sorted(self.broken),
            "broken_anchors": broken_anchors,
            "unrendered_bold": self.unrendered_bold,
            "size_mb": round(size / 1024 / 1024, 1),
        }
        print(json.dumps({
            key: (len(value) if isinstance(value, list) else value) for key, value in result.items()
        }, ensure_ascii=False))
        return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--wiki", required=True, type=Path)
    parser.add_argument("--out", required=True, type=Path)
    parser.add_argument("--base-url", required=True)
    parser.add_argument("--google-verification", default="", help="Search Console の確認コード")
    parser.add_argument("--report", type=Path, help="リンク切れなどの詳細を JSON で書き出す")
    args = parser.parse_args()

    result = Site(args.wiki, args.out, args.base_url, args.google_verification).build()
    if args.report:
        args.report.write_text(json.dumps(result, ensure_ascii=False, indent=1), encoding="utf-8")
    return 0


if __name__ == "__main__":
    sys.exit(main())
