# ANALYSIS.md — 開発支援ツール（CS/Frameworks/Tools）

対象: `root/programs/CS/Frameworks/Tools` / 最終更新: 2026-10-16

上位: [`Frameworks/ANALYSIS.md`](../ANALYSIS.md)（全体の俯瞰・ビルド）

---

## 1. これは何か

フレームワークを使う開発を助ける **WinForms のツール**（3 本）。どれも net48 版と net10.0 版を持つ。
**依存の末端**であり、ここを変えても基盤やサンプルは変わらない（回す範囲は [`CHEATSHEET.md`](../../../CHEATSHEET.md) 1 節）。

| ツール | 何をするか | CLI | 実行する人向けの文書 |
|---|---|---|---|
| `DaoGen_Tool`（Ｄ層自動生成ツール／墨壺） | DB のスキーマから、Ｄ層の定義情報（CSV）→ Dao クラス・DTO・SQL を生成する | **あり**（`/CUI`。#508） | [`DaoGen_Tool/README.md`](DaoGen_Tool/README.md) |
| `DeployZipPackWithHTTP` | ZIP を HTTP で配り、クライアントで展開する（差分配布・マニフェスト） | **あり**（#528 で復元） | [`DeployZipPackWithHTTP/README.md`](DeployZipPackWithHTTP/README.md) |
| `DPQuery_Tool` | 動的パラメタライズド クエリ（DPQ）を、DB に対して組み立て・実行して確かめる | **なし（GUI のみ）** | なし |

**CLI のあるツールは、README が一次情報**（実行ファイルの場所・`/HELP` の出し方・踏みやすい罠・前提）。
**引数の一覧は `/HELP` が一次情報**で、README にも本書にも書かない（`AGENTS.md`「付属ツールを CLI で使うときは」）。

---

## 2. 構成

| ツール | プロジェクト | TFM | 参照する部品（`HintPath`） |
|---|---|---|---|
| `DaoGen_Tool` | `DaoGen_Tool.csproj` / `DaoGen_ToolCore.csproj` | `v4.8` / `net10.0-windows7.0` | net48: `Public`。net10.0: `Public` / `Dam*` |
| `DPQuery_Tool` | `DPQuery_Tool.csproj` / `DPQuery_ToolCore.csproj` | 同上 | `Public` / `Dam*` |
| `DeployZipPackWithHTTP` | `DeployZipPackWithHTTP.csproj` / `DeployZipPackWithHTTPCore.csproj` | 同上 | `Public` / `Framework` / `Framework.RichClient` / `Business.RichClient` |

- **net48 版は `Infrastructure\Build\`**（`4_Build_CopyAssemblies.bat` が作る）、**net10.0 版は `Build_netcore100\` を参照する**
- **`HintPath` 参照では推移的な依存が効かない**ので、net10.0 版は DB ドライバや設定のパッケージ（`Microsoft.Data.SqlClient` / `MySql.Data` / `Npgsql` / `Oracle.ManagedDataAccess.Core` / `System.Data.Odbc` / `Microsoft.Extensions.Configuration*` など）を自分の csproj にも並べている
- `DaoGen_Tool` と `DPQuery_Tool` は `CmnMethods.cs` / `CheckException.cs` を**それぞれ複製して持つ**（名前空間だけが違う）
- `DPQuery_Tool/DummyDaps.cs` は、ビルド対象外の Dam（`DamOraClient` / `DamDB2` / `DamHiRDB`）と Oracle の旧クラスの**ダミー**で、ツールのコードを残したままビルドを通すためのもの
- `DeployZipPackWithHTTP` は **`ZipperV2` / `UnZipperV2` を使う唯一の利用者**。`Sample/SampleConsoleApp` は配布の動作確認用

ビルドは `4_Build_Framework_Tool.bat`（net48）/ `4_Build_Framework_ToolCore.bat`（net10.0）。

---

## 3. 検証

| ツール | 疎通テスト（`3_SmokeTest.ps1`） | 内容 |
|---|---|---|
| `DaoGen_Tool` | あり（6 件） | `/HELP`・定義情報の生成・Dao と SQL の生成を net48 / net10.0 で |
| `DeployZipPackWithHTTP` | あり（8 件） | マニフェストの生成・配置・プロキシ経由・プロキシ認証を net48 / net10.0 で。IIS Express で配信する |
| `DPQuery_Tool` | **なし** | GUI のみのため、ビルドが通ることまでしか確かめられない |

詳細は [`SMOKETEST.md`](../../../SMOKETEST.md)（「DaoGen_Tool（墨壺）の CUI モード」「DeployZipPackWithHTTP の CUI モード」）。

**`DPQuery_Tool` を変えたら、GUI で確かめていないことを報告する。**

---

## 4. 落とし穴

1. **ZIP 部品（`Public/IO` の `ZipperV2` / `UnZipperV2`）を変えたら、`DeployZipPackWithHTTP` のビルドと疎通も回す**（唯一の利用者）。
2. **`4_Build_CopyAssemblies.bat` を飛ばすと、net48 版がビルドできない**（`Build\` を参照する）。
3. **パッケージの版が部品側とずれやすい**（2 節）。部品の依存を上げたら、ツールの csproj も見る。
   いまも `System.Data.Odbc` は `Public`・`DaoGen_ToolCore`・`DPQuery_ToolCore` で版が違う。
4. **`CmnMethods.cs` は 2 つある**（2 節）。片方だけ直さない。
5. CLI の罠（パス区切り、標準出力の受け方、GUI が開く条件など）は各 README の「エージェントが踏みやすい罠」。
   **終了コードだけで判断せず、生成物の存在も確かめる。**

---

## 5. 作業チェックリスト

- [ ] CLI を使うなら、README を読み、まず `/HELP` を実行する
- [ ] 部品を先にビルドする（net48 は `4_Build_CopyAssemblies.bat` まで）
- [ ] 変えたツールだけを、`-Only` で疎通する（依存の末端なので、他は回さなくてよい）
- [ ] `DPQuery_Tool` は GUI でしか確かめられない。確かめていなければ、そう報告する
- [ ] 新しい CLI を足したら、README を置き、`AGENTS.md` と `CHEATSHEET.md` の表に足す
