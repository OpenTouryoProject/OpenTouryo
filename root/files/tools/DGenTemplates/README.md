# DGenTemplates — Ｄ層／画面 自動生成テンプレート

`DaoGen_Tool`（墨壺）と `DPQuery_Tool` が使う**テンプレート置き場**。

**格納物はテンプレートのみで、生成物は含まない。**
全ファイルが置換トークン（`_Xxx_`）または制御コメント（`ControlComment:`）を含む。

生成時に**フォルダをツールの画面で指定する**（既定値は `appsettings.json` の
`InputFilesRoot`）。ツール側に本フォルダのパスは埋め込まれていない。

---

## 1. 何がどのツールで使われるか

**2 つのツールが同じフォルダを共有している。** 片方だけを見て「未使用」と判断しないこと。

| テンプレート | DaoGen_Tool | DPQuery_Tool |
|---|---|---|
| `DaoTemplate.cs` / `.vb` | — | **○**（`DaoTemplate`） |
| `DaoTemplate2.cs` / `.vb` | — | — |
| `DaoTemplate3.cs` / `.vb` | **○**（`DaoTemplate3`） | — |
| `EntityTemplate.cs` / `.vb` | ○ | ○ |
| `EntityTemplate_bk.cs` / `.vb` | —（3 節） | — |
| `DataSetTemplate.xsd` | ○ | ○ |
| `TableAdapter.cs` / `.vb` | ○ | ○ |
| `ConditionalSearch` / `SearchAndUpdate` / `Detail`（`.aspx` ＋ `.cs` / `.vb`） | **○** 単票 | — |
| `_Screen_*`（同 3 画面） | — | **○** 結合表 |
| `s1_`〜`s4_`（静的 SQL） | ○ | ○ |
| `d1_`〜`d5_`（動的 SQL） | ○ | ○ |

- **`_Screen_` の有無は「新旧」ではなく「結合表か単票か」。**
  接頭辞なしは単一テーブル（`_TableName_` / `_ColumnName_`）、
  `_Screen_` 付きは結合テーブル（`_JoinTableName_` / `_JoinTextboxColumnName_`）。
- 対応は各ツールの `appsettings.json`（`*TemplateFileName`）が正。

---

## 2. DaoTemplate の 3 世代

**いずれも現役の選択肢**で、設定で切り替える。既定はツールごとに異なる。

| | クエリ キャッシュ | パラメタの型指定 | 使用ツール |
|---|---|---|---|
| `DaoTemplate` | — | — | DPQuery_Tool |
| `DaoTemplate2` | あり | — | （既定では未使用） |
| `DaoTemplate3` | あり | **あり**（`DaoParam` で `DbType` / `Size` / `Direction`） | DaoGen_Tool |

`2` → `3` の差は 13 行で、`DaoParam` による型指定への対応のみ（2020/06/20）。

---

## 3. EntityTemplate と EntityTemplate_bk

> **`_bk` は「単に古い控え」ではない。** 機能は `_bk` の方が多いが、**意図的に既定から外されている。**

| | `EntityTemplate`（既定） | `EntityTemplate_bk` |
|---|---|---|
| 主キー列のプロパティ名 | `_ColumnName_`（＝列名） | `PK__ColumnName_` |
| 設定済みフラグ | なし | `IsSet__ColumnName_` |
| ループ ブロック | `PKColumn` / `ElseColumn` | 左記 ＋ `PPUpdSet` / `PPLike` |

### なぜ機能が多い方が使われないのか

**項目移送の方式が変わったため。**

`_bk` は「自動生成 Dao への**項目移送**」を目的とした設計で、主キーを名前で区別していた。
その後、`Public/Dto` に POCO 変換部品が入り、**プロパティ名を列名として突合する**方式になった。

```csharp
// DataToPoco.cs — プロパティ名でそのまま列を引く
string srcName = ai_dst.AccessorName;   // POCO のプロパティ名
if (hs.Contains(srcName)) { object srcValue = dr[srcName]; }
```

この方式では **`PK_` 接頭辞は列名と一致せず、主キー列が黙って未設定になる**
（`map` 引数で対応表を渡せば回避できるが、自動生成 DTO ごとに用意することになる）。

`IsSet` フラグと `PPUpdSet` / `PPLike` も、列名をキーとする `Dictionary` 方式
（`_3TierParameterValue` の `InsertUpdateValues` など）で同等のことが表現できる。

### 経緯

```
2018/07/19  Public/Dto/DataToPoco.cs  新規作成（Dapper / AutoMapper.Data 代替）
2018/07/20  Public/Dto/PocoToPoco.cs  新規作成（AutoMapper 代替）
2018/07/25  Issue #293「EntityTemplate is redundant.」起票
2018/07/30  d7023598  EntityTemplate を単純化し、_bk として従来版を退避
2018/08/01  Issue #293 クローズ
```

[#293](https://github.com/OpenTouryoProject/OpenTouryo/issues/293) の起票者の記述。

> 当初は自動生成 Dao への項目移送を目的としてこう設計されたのだと思う。
> しかし ViewModel を生成する目的からすると、このテンプレートは適切でない。
> **既定のテンプレートは POCO 形式であるべき**だと考える。

**`_bk` を既定に戻す場合は、`DataToPoco` / `PocoToPoco` を使う箇所で
`map` 引数による対応付けが要る**ことに注意する。

---

## 4. テンプレートの記法

### 置換トークン `_Xxx_`

生成時に実際の値へ置換される。主なもの（出現数は画面テンプレート群での実測）。

| トークン | 意味 |
|---|---|
| `_TableName_` / `_ColumnName_` | 表名・列名（単票） |
| `_JoinTableName_` / `_JoinColumnName_` / `_JoinTextboxColumnName_` | 同（結合表） |
| `_TimeStampColName_` | 排他制御に使うタイム スタンプ列 |
| `_DAP_` / `_DBMS_` | データ プロバイダ・DBMS の切替 |
| `_CodebehindLanguage_` / `_ClassTemplateFileExtension_` | C# / VB の切替 |

置換対象の一覧は、各ツールの `appsettings.json` の **`Rp` 接頭辞のキー**にある。

### 制御コメント `ControlComment:`

繰り返しの範囲を示す。`.aspx` では HTML コメント、コード ファイルでは行コメント。

```
ControlComment:LoopStart-PKColumn   … 主キー列の繰り返し
ControlComment:LoopStart-ElseColumn … 主キー以外の列
ControlComment:LoopStart-JoinTables … 結合テーブル
ControlComment:LoopStart-PPUpdSet   … EntityTemplate_bk のみ
ControlComment:LoopStart-PPLike     … EntityTemplate_bk のみ
```

---

## 5. 変更するときの注意

- **C# 版と VB 版は対で維持する。** 片方だけ直すと生成結果が言語で食い違う。
- **プロパティ名を列名から変えない。** `DataToPoco` / `PocoToPoco` が
  プロパティ名で突合するため（3 節）。
- 画面テンプレートは Entity（DTO）を使わず、`_3TierParameterValue` の
  `Dictionary<string, object>`（`AndEqualSearchConditions` / `InsertUpdateValues`）で値を渡す。
  **DTO の形を変えても画面には影響しない**が、逆も同様で、DTO 側の都合で画面を直す必要はない。
- ツールの CLI からの使い方は
  [`DaoGen_Tool/README.md`](../../../programs/CS/Frameworks/Tools/DaoGen_Tool/README.md) を参照。
