# DaoGen_Tool（Ｄ層自動生成ツール／墨壺）— CLI

DB のスキーマから、Ｄ層の定義情報・Dao クラス・DTO・SQL を生成するツール。
GUI と CUI の両方を持ち、**CUI は非対話**なのでエージェントから実行できる（#508）。

本書は**エージェントが実行するため**の情報を記す。
**引数の一覧は書かない。** `/HELP` を実行すれば得られ、二重管理になるため。

---

## 1. ヘルプの実行（まずこれを見る）

```powershell
# net48
cd root\programs\CS\Frameworks\Tools\DaoGen_Tool\bin\Debug
.\OpenTouryo.DaoGen_Tool.exe /HELP

# .NET 10
cd root\programs\CS\Frameworks\Tools\DaoGen_Tool\bin\Debug\net10.0-windows7.0
.\OpenTouryo.DaoGen_Tool.exe /HELP
```

`/HELP` が**引数仕様の一次情報**。次が得られる。

- 起動の切り替え（引数なし＝GUI、`/HELP`、`/CUI`）
- `/CUI` 時の共通引数（`/MODE <DAODEFGEN|DAOSQLGEN>`）
- モードごとの必須・任意の引数
- 終了コード
- パス区切りと `/PRIMARYKEYS` の書式

**引数を組み立てる前に必ず `/HELP` を実行すること。** 本書の記述より `/HELP` が正である。

---

## 2. 実行ファイルの場所（`/HELP` からは分からない）

**アセンブリ名がプロジェクト名と異なる。** `DaoGen_Tool.exe` ではない。

| ターゲット | パス（`DaoGen_Tool` からの相対） |
|---|---|
| net48 | `bin\Debug\OpenTouryo.DaoGen_Tool.exe` |
| .NET 10 | `bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe` |

ビルドは既定のバッチで行う（`root\programs\CS` をカレントにして実行）。

```powershell
cmd /c "echo. | call 4_Build_Framework_Tool.bat"      # net48
cmd /c "echo. | call 4_Build_Framework_ToolCore.bat"  # .NET 10
```

> `1_DeleteDir.bat` は配下の `bin` / `obj` を再帰的に削除する。
> フル ビルド（`1_BuildAll.ps1`）の後は**再ビルドが必要**なことがある。

---

## 3. エージェントが踏みやすい罠

### 3.1 パスの区切りは `/` にする

コマンドライン解析（`StringVariableOperator.GetCommandArgs`）は
**`\` をエスケープ文字として扱う**。`\` は消えるため、別のパスとして解釈される。

```
OK : /OUTPUT "C:/temp/out"
OK : /OUTPUT "C:\\temp\\out"
NG : /OUTPUT "C:\temp\out"    ← \ が消える
```

PowerShell から渡す場合は変換しておく。

```powershell
$out = (Join-Path $env:TEMP "daogen/DaoDef.csv").Replace("\", "/")
```

> **終了コードでは検出できない。**
> `\` を使っても処理は成功し、**`0` が返る**。
> ただし出力先が別のパスになるため、指定した場所にファイルが無い。
> **生成物の存在を必ず確認すること**（5 節）。

### 3.2 標準出力は PowerShell のリダイレクトで受ける

本ツールは `WinExe` で、CUI 時は `AttachConsole(-1)` でコンソールに接続する。
このため**リダイレクトの方法によって出力が取れない**。

| 方法 | 結果 |
|---|---|
| `& $exe /HELP *>&1 \| Out-File $out` | **取れる** |
| `cmd /c "$exe /HELP > $out"` | **取れない**（0 バイト） |

### 3.3 GUI が開く条件

`/HELP` も `/CUI` も無い場合は `Application.Run(new Form1())` になり、
**画面が開いて応答が返らない**。エージェントは必ずどちらかを付ける。

---

## 4. 前提

### 4.1 接続文字列

`/CONNSTR` を省略すると設定ファイルの `ConnectionString_*` を使う。
`/DAP` の値に対応するキーが選ばれる。

| ターゲット | 設定ファイル |
|---|---|
| net48 | `app.config`（`appSettings`） |
| .NET 10 | `appsettings.json`（`appSettings`） |

いずれも**実行ファイルと同じフォルダ**に配置される。
`FamilyName` / `PersonalName`（生成物のヘッダに入る作成者名）も同じ場所。

### 4.2 テンプレート

`/MODE DAOSQLGEN` の `/TEMPLATE` にはテンプレートのルート フォルダを渡す。
リポジトリ内では次の場所にある（`DaoTemplate*.cs` などが平置き）。

```
root/files/tools/DGenTemplates
```

### 4.3 DB

`/MODE DAODEFGEN` は DB のスキーマを読むため、接続できる必要がある。
リポジトリの検証では SQL Server の Northwind を使っている。

---

## 5. 実行例（2 モードを連続して使う）

`DAODEFGEN` が出力した定義 CSV を `DAOSQLGEN` の入力に使う。

```powershell
$exe  = "...\bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe"
$work = "C:/temp/daogen"
$csv  = "$work/DaoDef.csv"
$tmpl = "C:/OpenTouryo/root/files/tools/DGenTemplates"

# DB のスキーマ → Ｄ層定義情報（*.csv）
& $exe /CUI /MODE DAODEFGEN /OUTPUT $csv /DAP SQL /TABLES "Shippers,Orders"

# Ｄ層定義情報 → Dao・DTO・SQL
& $exe /CUI /MODE DAOSQLGEN /DAODEF $csv /TEMPLATE $tmpl /OUTPUT "$work/gen" `
       /DAP SQL /LANG CS /ENTITY
```

**判定は終了コードと生成物の両方で行う。**
終了コードの意味は `/HELP` を参照。生成物は次を確認するとよい。

- `DAODEFGEN` … 定義 CSV に対象テーブルが並んでいること
- `DAOSQLGEN` … Dao(`.cs`)・動的 SQL(`.xml`)・静的 SQL(`.sql`) が揃っていること

---

## 6. 疎通確認との関係

`3_SmokeTest.ps1` が上記 3 つ（`/HELP`・`DAODEFGEN`・`DAOSQLGEN`）を
net48 / .NET 10 の両方で実行している。
**動く引数の組み合わせを確かめたい場合は、そちらの定義が参考になる。**

詳細は [`SMOKETEST.md`](../../../../SMOKETEST.md)。

なお **GUI の確認は手作業**として残している（[`RELEASE.md`](../../../../RELEASE.md)）。
CUI で確認できるのは生成ロジックまでで、画面の動作は含まない。
