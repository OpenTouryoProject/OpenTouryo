# チートシート

**手順だけを並べたもの。** 理由・詳細・落とし穴の背景は、各項のリンク先が一次情報。

> ここは**意図的に二重管理**している。手順は「思い出すため」に転記し、
> **判断が要ることは書かない。** 迷ったらリンク先を読むこと。

---

## 1. 検証（変更したら必ず）

```powershell
cd root\programs
.\0_RunAll.ps1                 # 1 → 2 → 3 を順に実行する
```

個別に回すとき。**順序は固定**（1 のクリーンとアセンブリ配置が 2・3 の前提）。

```powershell
.\1_BuildAll.ps1 -IgnoreErrors 'error MSB(3482|3325|3321):.*WSClientWinCone_sample\.csproj'
.\2_RunAllTests.ps1
.\3_SmokeTest.ps1
```

| | 合格の目安 | 詳細 |
|---|---|---|
| ビルド | 全ステップ OK | [`BUILDING.md`](BUILDING.md) |
| 単体テスト | 8/8 OK、差分 0 | [`TESTING.md`](TESTING.md) |
| 疎通 | 22/22 OK | [`SMOKETEST.md`](SMOKETEST.md) |

- **`-IgnoreErrors` を付けないと `MSB3482` で NG になる。** ClickOnce の署名で、
  証明書が無い環境では必ず出る（[`BUILDING.md`](BUILDING.md) 4 節）。
  **`0_RunAll.ps1` はこれを渡さない**ので、`1_BuildAll.ps1` が `1` を返すことがある。
  終了コードだけで判断せず、エラー一覧の内容を見ること
- `2_RunAllTests.ps1` は `Result*.txt` を書き換える。**差分 0 なら中身は同じ**
- 前提（DB・サービス・IIS Express）は [`RELEASE.md`](RELEASE.md) 2 節

---

## 2. リリース（NuGet 公開）

**一次情報は [`RELEASE.md`](RELEASE.md) と [`CS/NuGet/README.md`](CS/NuGet/README.md)。**

```
1. develop → master へマージ（--no-ff）し、タグをプッシュ
2. .\0_SetVersion.ps1 -Version 3.3.0
3. コミット & push                      ← Source Link はこのコミットに固定される
4. CS\0_Release4Nuget.bat               ← 版はアセンブリに焼き込まれる
5. CS\NuGet\_NuGetPack.bat              ← 版の一致（ソース・DLL）を自動検査
6. 確認 5 点                            ← README.md 2 節
7. set NUGET_API_KEY=＜キー＞
   CS\NuGet\out\sp\_NuGetPush.bat
8. キーを Revoke（Delete しない）
```

### バージョンを上げる

```powershell
cd root\programs
.\0_SetVersion.ps1 -Version 3.3.0-alpha1 -WhatIf   # 変更内容の確認
.\0_SetVersion.ps1 -Version 3.3.0-alpha1           # 実行
```

`Directory.Build.props` と net48 6 本の `AssemblyInfo.cs` を一括更新する。
**プレリリース サフィックスはアセンブリの版には入らない。**
→ [`RELEASE.md`](RELEASE.md) フェーズ 0

### 公開前の確認 5 点

→ [`CS/NuGet/README.md`](CS/NuGet/README.md) 2 節

```
1. .nupkg と .snupkg が対で出ている（.nupkg に pdb が入っていない）
2. PDB が portable（先頭 4 バイトが BSJB）
3. Source Link の URL が入っている（net48 と net10.0 の両方）
4. nuspec の <repository commit> が PDB のコミットと一致
5. そのコミットが GitHub 上にある（raw URL が 200）＋ ワーキング ツリーが綺麗
```

### テスト公開（`Erutcurtsarfni.Oyruot.Public`）

```
CS\0_Release4Nuget.bat
CS\NuGet\_T_NuGetPack.bat 3.3.0-alpha2      ← 版は引数で渡す
set NUGET_API_KEY=＜キー＞
CS\NuGet\out\sp\_T_NuGetPush.bat
```

作業ブランチ上で行ってよい。→ [`CS/NuGet/README.md`](CS/NuGet/README.md) 4 節

### 生成物を消す

```
CS\NuGet\_Cleanup.bat                       ← in\ と out\ の全パッケージ
CS\NuGet\_Cleanup.bat Touryo.Infrastructure ← その接頭辞だけ
```

パック バッチが自動で呼ぶので、通常は不要。→ [`CS/NuGet/README.md`](CS/NuGet/README.md) 6 節

---

## 3. VB 側のビルド

```
root\programs\VB\0_ExecAllBat.bat           ← 通し（先に CS 側を建てる）
```

- 先頭で `cd "..\CS"` して C# 側の `2_Build_NuGet_net48.bat` を呼ぶ。**VB は C# の成果物に依存する**
- **個別実行では見つからない不具合がある。** `1_DeleteDir.bat` が `obj` / `packages` を
  消した後にだけ露見するものがあるため、**通しで確かめること**（#533）
- 合格の目安 : ビルド成功 22 / 失敗 0

---

## 4. ツールを CLI で使う

**引数の一覧は README に無い。`/HELP` が一次情報。**

| ツール | 場所 |
|---|---|
| `DaoGen_Tool`（墨壺） | [`README.md`](CS/Frameworks/Tools/DaoGen_Tool/README.md) |
| `DeployZipPackWithHTTP` | [`README.md`](CS/Frameworks/Tools/DeployZipPackWithHTTP/README.md) |

終了コードだけで判断せず、**生成物の存在も確認する**（パス区切りを誤ると成功を返しつつ別の場所に出る）。

---

## 5. コードを書く前に

**規約は [`CODING.md`](CODING.md)、領域ごとの事情は各 `ANALYSIS.md`。**

| | |
|---|---|
| ファイル ヘッダ・更新者名・Copyright ブロック | [`CODING.md`](CODING.md) 1 節 |
| `ArgumentException` だけ引数の順が違う | [`CODING.md`](CODING.md) 3 節 |
| フレームワーク本体 | [`CS/Frameworks/ANALYSIS.md`](CS/Frameworks/ANALYSIS.md) |
| net48 サンプル | [`CS/Samples/ANALYSIS.md`](CS/Samples/ANALYSIS.md) |
| netcore サンプル | [`CS/Samples4NetCore/ANALYSIS.md`](CS/Samples4NetCore/ANALYSIS.md) |

### `.bat` を書くとき

- **リリース・公開に使う bat は非 ASCII を書かない**（コメントも英語）。BOM も不要
- **`chcp` を bat の中で使わない**
- 非 ASCII が**外部プログラムへ渡す引数**なら消せない。その場合は
  コンソールのコード ページに合わせる（Shift-JIS・BOM なし）
- 改行は **CRLF**

→ [`CODING.md`](CODING.md) 4 節

### `.ps1` を書くとき

- **UTF-8 BOM 付き**
- **PowerShell 5.1 と 7 の両方で動くこと。** 変更したら 5.1 でも実行する

```powershell
powershell.exe -NoProfile -Command "Set-Location 'root\programs'; .\3_SmokeTest.ps1"
```

→ [`CODING.md`](CODING.md) 5 節

---

## 6. よく踏む落とし穴

| 症状 | 原因 | 対処 |
|---|---|---|
| `'xxx' is not recognized` が大量に出る | bat の非 ASCII とコード ページ | ASCII 化。[`CODING.md`](CODING.md) 4 節 |
| `MSB4226`（`Microsoft.WebApplication.targets`） | nuget が別製品の MSBuild を拾った | `nuget.exe restore ... %NUGET_MSBUILD%` |

**NuGet パッケージ作成の落とし穴は
[`CS/NuGet/README.md`](CS/NuGet/README.md) 9 節**にまとめてある。

```
.snupkg だけ 403 / F11 でローカルが開く / nuspec と PDB のコミット不一致
リビルド忘れ / net48 だけ効かない / 公開したコミットが消えた
```

**上 5 つは公開前に机上で分かる。** 2 節の確認 5 点を省かないこと。

---

## 7. エージェントとして守ること

**一次情報は [`AGENTS.md`](../../AGENTS.md)。**

- **Git 操作をしない**（`add` / `commit` / `push` / `checkout` / `switch` / `branch` /
  `reset` / `restore` / `stash`）。参照系（`status` / `diff` / `log` / `show` 等）は自由
- **GitHub への投稿は、文面を提示して承認を得てから。** `gh` で行い、`--body-file` を使う。
  アカウントは `OsscJpDevInfra`（`gh auth status` で確認）
- **Issue のクローズ・ラベル・アサイン、PR の作成やマージは人が行う**
- **NuGet への push は人のみ**（外部公開で取り消しが困難）
- 前提となるサービスや DB の状態が足りないときは、**勝手に変えず対処方法とともに報告する**
