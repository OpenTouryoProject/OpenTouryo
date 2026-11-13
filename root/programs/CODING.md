# コーディング規約

**既存コードに合わせること。**

この文書は `root/programs/` 配下（C# / VB / bat / ps1）に共通で適用する。
領域ごとの分析・ビルド手順・落とし穴は、各領域の `ANALYSIS.md` にある。

| 対象 | 文書 |
|---|---|
| フレームワーク本体 | [`CS/Frameworks/ANALYSIS.md`](CS/Frameworks/ANALYSIS.md) |
| net48 サンプル | [`CS/Samples/ANALYSIS.md`](CS/Samples/ANALYSIS.md) |
| netcore サンプル | [`CS/Samples4NetCore/ANALYSIS.md`](CS/Samples4NetCore/ANALYSIS.md) |

上の 3 つは俯瞰である。アセンブリ・サンプルごとの下位の `ANALYSIS.md` は、各文書の冒頭の案内表から辿る（#613）。

> **既存箇所の一括置換はしない。** 下位互換の維持が最優先のため
> （[`Contributing.ja.md`](../../Contributing.ja.md)）、新規・修正箇所から適用する。

---

## 1. ファイル ヘッダ（**新規追加時も必須**。ただし新規と既存で書式が異なる）

### 新規ファイルに付けるヘッダ（これが現行の書式）

```csharp
#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// ...（定型 15 行）
//
#endregion

//**********************************************************************************
//* クラス名        ：CallController
//* クラス日本語名  ：クライアント ライブラリ
//*
//* 作成者          ：xxx
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/07/31  xxx               新規作成
//**********************************************************************************
```

※ 更新者は ClaudeCode：玄人 幸道、GitHubCopilot：後輩 郎党 で。

> **`Copyright (C) ... Hitachi Solutions,Ltd.` のブロックは、新規ファイルには付けない。**
> 開発元が企業からコミュニティに移ったため。
> Apache License の region と、クラス名・日本語名・更新履歴のブロックは従来どおり必要。

### 既存ファイルの場合

既存ファイルの先頭には次の Copyright ブロックが付いている。**これは削除せず、そのまま残す。**

```csharp
//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************
```

**既存ファイルを変更した場合は、更新履歴に 1 行追記するのがこのリポジトリの慣習。**

## 2. その他の規約

- **コメント・XML ドキュメントは日本語**。`<summary>` は全 public/protected メンバに付与
  （`DocumentationFile` を出力しているため、欠けると警告）。
- `#region` / `#endregion` による細かいブロック分割が徹底されている（`BaseController` は 100 以上）。
- `<remarks>自由に利用できる。</remarks>` … 業務コードから直接呼んでよい API の目印。
  `<remarks>業務コード親クラス１から利用される派生の末端</remarks>` … オーバーライド専用の目印。
- 拡張ポイントは **`UOC_` プレフィクス**（`FxLiteral.UOC_METHOD_HEADER`）。
  P層の集約イベント ハンドラも `UOC_<ControlId>_Click` のような命名規則でリフレクション解決される。
- 定数は `FxLiteral`（Framework, 777 行）/ `MyLiteral`（Business）/ `PubLiteral`（Public）に集約。
  **文字列リテラル直書きではなく、これらに定数を追加する。**
- **利用者に見せる文言は `Resources` の `.resx` に置く**（国際化のため）。
  `FxLiteral` 等は「変わらない値」、`.resx` は「訳し分ける文言」で、役割が違う。
  **フレームワークにもツールにもある。** 対応は次のとおり。

  | 場所 | リソース | 取り出し方 |
  |---|---|---|
  | `Infrastructure/Public/Resources/` | `PublicExceptionMessageResource` | `PublicExceptionMessage.XXX`（プロパティ名がキー） |
  | `Infrastructure/Framework/Resources/` | `FrameworkExceptionMessageResource` | `FrameworkExceptionMessage.XXX` |
  | `Infrastructure/Business/Resources/` | `MyBusiness{Application,System}ExceptionMessageResource` | 同上 |
  | `Tools/*/Resources/` | `Resource` | `ResourceMgr.GetString("キー")` |

  **`.resx` は 2 つ 1 組。** 既定（英語）と `*.ja-JP.resx`（日本語）の**両方**に足す。
  `*.Designer.cs` も更新が要る（VS でデザイナを開けば再生成される）。

  例外メッセージには別系統もある。**`MSGDefinition.xml` / `MSGDefinition_ja-JP.xml`**
  を `GetMessage.GetMessageDescription("I0011")` で引く形で、
  こちらは `root/files/resource/Xml/` と各ツールの配下にある。**既存に合わせること。**
- 命名: `Base*`（Framework 提供の抽象）→ `My*`（Business 層テンプレート、アプリで改変前提）。
  アプリ側は `LayerB` / `LayerD` / `TestParameterValue` / `TestReturnValue` を実装（`Samples/` 参照）。
- 変数はプライベート フィールド `_xxx` ＋ 明示的プロパティ（自動プロパティは新しい箇所のみ）。

## 3. 引数を検査する例外（`ArgumentException` だけ引数の順が違う）

**3 つとも「引数名」を渡すが、渡す位置が違う。**
`ArgumentException` **だけメッセージが先**で、ここが取り違えの温床になる。

| 例外 | 第 1 引数 | 第 2 引数 |
|---|---|---|
| `ArgumentNullException` | **引数名** | メッセージ |
| `ArgumentOutOfRangeException` | **引数名** | 実際の値（3 引数版）→ メッセージ |
| `ArgumentException` | **メッセージ** | **引数名** |

`ArgumentException("userId")` と書くと、**引数名ではなくメッセージが "userId" になる**。
引数名を伝えたいなら第 2 引数に置く。

```csharp
throw new ArgumentNullException(nameof(bytes));
throw new ArgumentOutOfRangeException("ecc", ecc, "Invalid");
throw new ArgumentException("Length is less than 128 bits.", "cek");   // 引数名は第 2
```

既存コードは 148 箇所すべてこの規約どおり
（`ArgumentException` は 139 箇所あり、大半はメッセージのみ。
メッセージは `PublicExceptionMessage` の定数を使う）。

**引数名は `nameof` で書く。** 引数名を変えたときに追随するため。
文字列リテラルだと乖離しても誰も気付かない。実際、`ArrayOperator.GetLongFromByte` は
分割時（2019/05/28）の旧名 `"bytData"` が残り、**存在しない引数名**を渡していた
（2026/08/06 に修正、#522）。

> **既存箇所の一括置換はしない。** 下位互換の維持が最優先のため
> （[`Contributing.ja.md`](../../Contributing.ja.md)）、新規・修正箇所から適用する。

## 4. bat ファイルの文字コード

**bat は、非 ASCII を一切書かない。** コメントも `echo` も英語にする。BOM も付けない（ASCII なら不要で、差分ノイズになるだけ）。
**リポジトリのすべての bat が対象**である（#671。汎用認証サイトの方針に揃えた）。日本語の説明は、そのフォルダの `README.md` などに書く。
**例外は、外部プログラムへ渡す引数が日本語の bat だけ**（下の「例外」）。

- **新しく書く bat も同じ。** 重要な bat には、先頭に `NOTE: keep this file pure ASCII.` と理由を書く（`z_Common.bat`・`NuGet\*.bat` と同じ）
- **英語にするときは、`if ( … )` などの括弧の中に半角の `)` を書かない**（`echo` でも `rem` でも、そこでブロックが閉じる）。
  日本語の全角の括弧は問題にならなかったので、**訳すときに踏みやすい**
- 確かめるには、非 ASCII のバイトを含む bat を探す（例外の 2 本だけが出るのが正）

```powershell
git ls-files '*.bat' | Where-Object { [IO.File]::ReadAllBytes($_) | Where-Object { $_ -gt 0x7F } | Select-Object -First 1 }
```

### なぜ ASCII にするか

cmd.exe はバッチを**バイト オフセットで読み進める**ため、非 ASCII があると、コンソールのコード ページ次第で文字境界がずれ、
**`@rem` コメントの途中から先がコマンドとして実行される**ことがある
（`'xxx' は、内部コマンドまたは外部コマンド…として認識されていません` が出る）。

以前は「非 ASCII を含む bat は UTF-8 BOM 付きにする」としていた。BOM で減りはするが、下のとおり保証にはならない。

| BOM | 起動 CP=932 | 起動 CP=65001 |
|---|---|---|
| なし | エラーなし | **エラーあり（間欠）** |
| あり | エラーなし（**ただし、対話コンソールで出ることがある**。下記） | エラーなし |

- 実害は「紛らわしいエラー表示」に留まり、**後続の実コマンドは飛ばない**。

#671 の時点（2026/11/09）で、例外の 2 本を除くすべての bat を ASCII にした（BOM だけが非 ASCII だった 12 本は BOM を外し、コメント・`echo` に日本語があった 8 本は英語にした）。

### BOM は万能ではない。危ない bat は ASCII のみにする（#532）

**BOM を付けても、対話コンソール（CP=932）で解析ずれが起きることがある。**

#531 で、NuGet の push bat（UTF-8 BOM ＋ CRLF ＋ `chcp` なし）を
利用者が cmd から実行したところ、日本語の `@rem` が実行されて次が出た。

```
'onfig' is not recognized as an internal or external command,
'…ても、そちらは消えない。' is not recognized as an internal or external command,
'NuGet.Config' is not recognized as an internal or external command,
```

- **非対話（`cmd /c`）では再現しない。** 手元で確認しても出ないため気付けない
- **`chcp 65001` を入れても直らない。**
  むしろコードページが途中で変わる分、条件が増える
- 上の表（BOM あり＝エラーなし）は**目安であって保証ではない**

**実害は紛らわしいエラー表示に留まり、後続の実コマンドは飛ばない**
（#531 でも、エラー表示の後で push 自体は成功していた）。
とはいえ、公開作業中に大量のエラーが出るのは危険である。

> **#532 では、重要な bat（リリース・公開に使うもの）だけを ASCII にした。**
> `root\programs\CS\NuGet\*.bat` と `root\programs\CS\0_Release4Nuget.bat`（各ファイルの先頭に `NOTE: keep this file pure ASCII` と記載）。
> #671 で、これをすべての bat に広げた（上の 4 節の冒頭）。

**日本語を `echo` する場合も同じ。**
cmd.exe は BOM 付き UTF-8 でも、内容をコンソールのコードページで解釈するため、
CP=932 では文字化けする（`chcp` を入れても、それ自体が解析ずれの種になる）。

### 例外 : 非 ASCII が「引数」である bat（#532）

**ASCII 化できるのは、非 ASCII がコメントか表示のときだけ。**
外部プログラムに**引数として渡す**文字列が日本語なら、消しようがない。

| 非 ASCII の役割 | 対処 |
|---|---|
| コメント・`echo` | **ASCII 化する**（すべての bat。#671） |
| **外部プログラムへ渡す引数** | **消せない。** コンソールのコードページに合わせて符号化する（この環境では **Shift-JIS・BOM なし**） |

`root\programs\bak\BackupCICDLog\z_GrepResult1.bat`・`z_GrepResult2.bat` がこれに当たる（**リポジトリで非 ASCII が残る bat は、この 2 本だけ**）。
`z_GrepResult3.bat` は検索キーワードが ASCII（`warning`）なので、#671 で ASCII にした。

```bat
rem このファイルは、SJISでないと動かない。
set PATH=C:\Program Files (x86)\sakura;%PATH%
sakura -GREPMODE -GKEY="ビルドに" -GFOLDER="." -GFILE="*.log" -GOPT=P -GCODE=4
```

検索キーワードは、cmd.exe がバッチのバイト列を読んで `sakura.exe` の
コマンドラインへ渡す。**UTF-8 で保存すると、CP=932 のコンソールでは
文字化けしたキーワードが渡り、grep が何も見つけない。**

**エラーにならず、静かに結果が空になる**ため、解析ずれより気付きにくい。
**この 2 本は Shift-JIS のままにしてある。BOM を付けてはならない。**

> `-GCODE` は**検索対象ファイル**の文字コード指定であって、
> バッチ自身の符号化とは別である。

## 5. ps1 ファイルの文字コードと、PowerShell 5.1 / 7 の両対応

**`.ps1` は Windows PowerShell 5.1 と PowerShell 7 の両方で動くこと。**

開発時は `pwsh`（7）で確認しがちだが、利用者は `powershell.exe`（5.1）で実行する。
7 だけで確認すると 5.1 で落ちる。既踏の落とし穴は次のとおり。

| 事象 | 原因 | 対処 |
|---|---|---|
| 構文エラー・文字化け（`繧ｹ繝・ャ繝・`） | 5.1 は BOM 無しの `.ps1` を **ANSI（Shift_JIS）**として読む | **UTF-8 BOM 付き**で保存する |
| 同じファイルなのに差分が出る | `Get-Content` の既定エンコードが 5.1 は ANSI、7 は UTF-8 | **`-Encoding UTF8`** を明示する |
| HTTP が常に失敗（状態コード `-1`） | `-SkipHttpErrorCheck` は **7 以降にしかない** | バージョンを見て付け外しする |
| 実行中に画面がクリアされ、それまでの結果が消える | 子プロセスの `chcp 65001` はコンソール全体に影響する | スクリプト冒頭で先に切り替える |
| 表の見出し・罫線・データがずれる | 5.1 の `Format-Table` は**桁数ではなく文字数**で幅を決める（全角は 1 文字で 2 桁） | `SummaryTable.ps1` の `Write-SummaryTable` を使う |
| 子プロセスの標準入力に渡した値（パスワードなど）が合わない。**5.1 からだけ**起き、7 からは通る | 5.1（.NET Framework）は子プロセスの標準入力を `[Console]::InputEncoding` で開き、**それが BOM 付きの UTF-8 だと、起動した時点で BOM を書く**。net10.0 の子プロセスは、それを値の先頭の 1 文字（U+FEFF）として読む（net48 の子プロセスは読み飛ばす） | 子プロセスを起動する前に **`[Console]::InputEncoding` を BOM 無しの UTF-8 にする**。受け取る側も、先頭の BOM を読み飛ばすとよい（#668。`Sample4SpRp\E2E\CliLoginE2E.ps1`・`SpRp_CLI`） |
| 非 ASCII の `echo` を持つ `.bat` を呼ぶと、その行が化ける | **cmd は、バッチをコンソールの出力のコード ページで読む**（日本語の Windows の既定は 932）。UTF-8 で書いた bat の日本語は、932 では読み違える | **呼ぶ間だけ `[Console]::OutputEncoding` を UTF-8 にし、`finally` で戻す**（#668。`Sample4SpRp\container\Stub\StubCompose.ps1`）。根本は bat を ASCII にする（4 節） |

```powershell
# 7 専用の引数は、バージョンを見て付け外しする
if ($PSVersionTable.PSVersion.Major -ge 6) { $p.SkipHttpErrorCheck = $true }

# コンソールのコード ページと、PowerShell の出力エンコードは別物。判定を分けること
if ((cmd /c chcp) -notmatch '65001') { cmd /c chcp 65001 | Out-Null }
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}
```

```powershell
# 子プロセスの標準入力に BOM を書かせない（5.1。起動の前に）
[Console]::InputEncoding = New-Object Text.UTF8Encoding $false

# 非 ASCII の bat を呼ぶ間だけ、コンソールを UTF-8 にして、元に戻す（呼んだ側のコンソールを変えたままにしない）
$old = [Console]::OutputEncoding
try
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
    & $bat
}
finally
{
    [Console]::OutputEncoding = $old
}
```

- **bat の化けは、出力を取り込んで（パイプ・リダイレクトで）確かめると見えない。**
  932 で読み違えた文字を 932 で書き戻すので、バイト列としては元に戻る。**コンソールに直接出したときにだけ見える**
- **標準入力の BOM は、7 から動かすと起きない。** 5.1 から動かして確かめること（下の「変更したら 5.1 でも実行」）

```powershell
# 集計表は Format-Table ではなく、桁数を自前で数える整形を使う（root\programs のスクリプトから。#687）
. (Join-Path $PSScriptRoot "..\files\tools\PowerShell\Verify\SummaryTable.ps1")
Write-SummaryTable $results
```

- **`.bat` は「非 ASCII を含むときだけ」BOM 付き**（8.4）だが、
  **`.ps1` は非 ASCII を含むなら必ず BOM 付き**。5.1 が既定で ANSI として読むため。
- **要素 1 個の配列は、返した時点でスカラーに展開される。** そのまま `[0]` を取ると
  **文字列の 1 文字目**になる。関数の戻り値を添字で使うなら `@()` で受けること。
- 5.1 の `[Console]::OutputEncoding` は**起動時の値のまま**で、実行中にコード ページが
  変わっても追随しない。同じ画面で 2 回目を実行したときに化ける原因になる。
- **変更したら 5.1 でも実行して確かめること。**

```powershell
powershell.exe -NoProfile -Command "Set-Location 'root\programs'; .\3_SmokeTest.ps1"
```

検証スクリプト側での具体的な適用例は
[`SMOKETEST.md`](SMOKETEST.md) 「PowerShell 5.1 と 7 の両対応」を参照。

---
