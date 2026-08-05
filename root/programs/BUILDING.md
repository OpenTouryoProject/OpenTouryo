# BUILDING.md — 全ビルドの実行と判定

対象: `root/programs/CS`（C# 側）
配置: `root/programs`
本書は、リリース時に行っていた「build バッチで全ビルドが通ることを確認」を、
**合否が出る形に機械化**するための手順と判定基準を記述する（#513 段階 2）。

> **ビルド構成そのもの**（bat の一覧、ビルド順、`z_Common.bat` と `z_Common2.bat` の関係、
> バージョン番号の一元管理、NuGet パッケージ化）は
> [`CS/Frameworks/ANALYSIS.md`](CS/Frameworks/ANALYSIS.md) の **7 章「ビルド」** が一次情報。
> 本書はその上で「どう実行し、どう合否を判定するか」だけを扱う。

---

## 1. 使い方

```powershell
cd root\programs

# 全ビルド（0_ExecAllBat.bat 相当）を実行し、合否を一覧表示する
.\1_BuildAll.ps1

# 一部のステップだけ実行する（動作確認用）
.\1_BuildAll.ps1 -Only "WebApp_sample" -SkipClean

# ログの出力先を変える
.\1_BuildAll.ps1 -OutputDir D:\logs

# 環境に依存して必ず出るエラーを、判定から外す（4 節）
.\1_BuildAll.ps1 -IgnoreErrors "MSB3482"
```

終了コードは `0` = 全ステップ OK、`1` = NG あり。

| オプション | 内容 |
|---|---|
| `-Only <文字列>` | ステップ名／bat 名の部分一致で対象を絞る |
| `-SkipClean` | クリーン処理（`1_DeleteDir` / `1_DeleteFile`）を省略 |
| `-OutputDir <パス>` | ステップ別ログの保存先（既定 `%TEMP%\OpenTouryoBuildLogs`） |
| `-IgnoreErrors <正規表現…>` | 既知のエラーを合否判定から外す（複数指定可） |

### `-SkipClean` の注意

**リリース判定では使わないこと。** 前回のビルド成果物が残っていると、
実際にはビルドできない状態でも通ったように見える。

`1_DeleteDir.bat` は配下から `packages` `obj` `bin` `bld` `Temp` `PrecompiledWeb`
`MigrationBackup` `.vs` を再帰的に削除する。**Visual Studio は閉じてから実行する**
（`.vs` が削除されるため）。

---

## 2. なぜラッパーが必要か

既存のビルド バッチをそのまま呼び出す構成にしてある。
「何をビルドするか」の正はバッチ側に残り、`1_BuildAll.ps1` は実行と判定のみを担う。

ラッパーが必要な理由は次の 3 点。

1. **各バッチは MSBuild の終了コードを伝播しない**（`%ERRORLEVEL%` を見ていない）
2. **各バッチの末尾に `pause` がある**（対話入力を待つ）
3. **`-v:d`（詳細）で出力が膨大**（1 ステップで 6,000 行を超えることがある）

このため、stdin を与えて実行し、出力から `error` / `warning` 行を抽出して判定している。

---

## 3. 判定基準

**エラー行が 1 件でもあれば NG**、無ければ OK。警告は件数を報告するだけで合否に影響しない。

エラー・警告の抽出は、次の形式の行を対象とする。

```
xxx.csproj(12,5): error CS1002: ...          ← コードあり
Microsoft.NuGet.targets(198,5): error : ...  ← コードなし（NuGet の restore 失敗）
```

「ビルドに成功しました」等のサマリ文言は**ロケールで変わるため使わない**。
`error` / `warning` とその後のコードは英語のまま出力されるため、そちらで判定する。

同一の指摘が複数プロジェクトから重複して出るため、一意化してから件数を数える。

### 既知のエラーの除外

`-IgnoreErrors` に指定した正規表現に一致するエラーは、**合否判定から外れる**。
環境に依存して必ず出るものを、終了コードを汚さずに扱うための仕組み。

**除外したものは消えない。** 件数はサマリの `既知` 列に出し、内容も末尾に一覧する。

```
  OK  エラー 0 / 警告 0 / 既知 1  (8.7 秒)

======== 既知として除外したエラー ========
  除外条件 : MSB3482
  [WSClnt_sample (net48)] ... error MSB3482: 署名中にエラーが発生しました: ...
```

握り潰しを防ぐための表示なので、**除外件数が想定より多いときは条件が広すぎることを疑う。**

### 警告について

**警告 0 にはならない。** 現状の内訳は次のとおりで、いずれもコンパイル警告ではない。

| ステップ | 警告数 | 内容 |
|---|---|---|
| NuGet (netcore100) | 52 | `NU1903` 等、パッケージの脆弱性アドバイザリ |
| Business (netcore100) | 38 | 同上 |

これらは依存パッケージ側の問題であり、ビルドの合否とは切り離している。
**件数が大きく増減した場合は依存関係の変化を疑う**、という使い方をする。

---

## 4. 既知の環境依存

### WSClientWinCone_sample の署名エラー

```
error MSB3482: 署名中にエラーが発生しました: bin\Debug\app.publish\WSClientWinCone_sample.exe
の署名に失敗しました。SignTool Error: No certificates were found that met all the given criteria.
```

`WSClientWinCone_sample.csproj` は ClickOnce のマニフェスト署名が有効になっている。

```xml
<SignManifests>true</SignManifests>
<ManifestCertificateThumbprint>A69CDE3C92D8862D42E7A239134686E32089B679</ManifestCertificateThumbprint>
<ManifestKeyFile>WSClientWinCone_sample_TemporaryKey.pfx</ManifestKeyFile>
```

`.pfx` はリポジトリに同梱されているが、**MSBuild は拇印で証明書ストアを検索する**ため、
当該証明書が入っていない環境ではビルドできない。
`_TemporaryKey.pfx` の名のとおり Visual Studio が自動生成した開発用の一時証明書であり、
同じ `WSClient_sample` 配下の `WSClientWin_sample` / `WSClientWPF_sample` に署名設定は無い。

**環境依存であり、コード側の不具合ではない。** ビルドを通すには、
`.pfx` を証明書ストアにインポートするか、`SignManifests` を `false` にする
（`Install` は `false` のため配布用途でもない）。

**CI では証明書を用意できない。** 拇印での検索であるうえ、同梱の `.pfx` は
パスワードで保護されており、実行のたびに作り直される環境には入れられない。
このため CI では `-IgnoreErrors "MSB3482"` で判定から外している（9 節）。

---

## 5. 修正の経緯 : nuget.exe の MSBuild 誤検出

2026/08/01 時点で、net48 側の 7 ステップ・エラー 14 件が次の 2 種類で失敗していた。

```
error MSB4226: インポートされたプロジェクト "...WebApplications\Microsoft.WebApplication.targets"
               が見つかりませんでした。
error : Your project file doesn't list 'win' as a "RuntimeIdentifier".
```

原因は、**`nuget.exe` が MSBuild を自動検出し、同居する SQL Server Management Studio の
MSBuild を選んでいた**こと。ログに次が出ていた。

```
MSBuild 自動検出: 'C:\Program Files\Microsoft SQL Server Management Studio 22\Release\MSBuild\Current\bin'
                  から MSBuild バージョン '18.8.2.30814' を使用します。
```

その MSBuild には Web アプリ用の `Microsoft.WebApplication.targets` が無く、
また生成される `project.assets.json` が実際のビルド（Visual Studio の MSBuild）と噛み合わない。

`z_Common.bat` は `vswhere` で MSBuild を正しく解決していたが、
**その値が `nuget.exe` に渡っていなかった**。このため次を追加した。

```bat
for %%i in (%BUILDFILEPATH%) do set MSBUILDDIR=%%~dpi
if defined MSBUILDDIR set MSBUILDDIR=%MSBUILDDIR:~0,-1%
set NUGET_MSBUILD=-MSBuildPath "%MSBUILDDIR%"
```

末尾の `\` を除去しているのは、`-MSBuildPath "...\"` だと `\"` がエスケープと解釈され、
引数が壊れるため。この `%NUGET_MSBUILD%` を、`nuget.exe restore` を呼ぶ
**11 バッチ・20 箇所**に付与した。

結果、**7 ステップ NG → 1 ステップ NG**（残りは上記の署名エラーのみ）となった。

> MSBuild を同梱する製品（SSMS、Build Tools、旧 VS 等）が同居する環境では
> 同種の問題が起こり得る。`nuget.exe` 側にも MSBuild を明示する、という対処が要る。

---

## 6. 実行結果の例

```
ステップ                     結果 エラー 既知 警告    秒
--------                     ---- ------ ---- ----    --
Clean (net48 基盤)           OK        0    0    0  8.30
NuGet (net48)                OK        0    0    0 12.80
Business (net48)             OK        0    0    0  7.90
...
NuGet (netcore100)           OK        0    0   52 22.80
Business (netcore100)        OK        0    0   38  6.30
...
WSClnt_sample (net48)        NG        1    0    0 21.20
...

  所要時間 : 5.3 分
  1 ステップが NG
```

`既知` 列は `-IgnoreErrors` で除外した件数。指定しなければ常に `0`。

全 31 ステップ（クリーン 8 ＋ 実ビルド 23）で **約 5 分**。

---

## 7. 単体テストとの関係

ビルドが通ったら、単体テストの実行と判定を行う。
手順は [`TESTING.md`](TESTING.md) を参照。

```powershell
.\2_RunAllTests.ps1
```

**必ず `1_BuildAll.ps1` → `2_RunAllTests.ps1` の順で行う。**
`1_BuildAll.ps1` はクリーンを行い、`4_Build_CopyAssemblies.bat` が
テストの参照先（`Build_net48` / `Build_netcore100`）を更新するため、
逆順ではテストが古いアセンブリを見ることになる。

なお `0_ExecAllBat.bat` は `y_Build_TestCode*.bat`（単体テスト）を含まない。
テスト側のビルドは `2_RunAllTests.ps1` がバッチ経由で行うため、二重に実行する必要はない。

## 8. サンプルの疎通確認

単体テストが通ったら、サンプル アプリの疎通を確認する。
手順は [`SMOKETEST.md`](SMOKETEST.md) を参照。

```powershell
.\3_SmokeTest.ps1
```

**`1_BuildAll.ps1` はクリーンの繰り返しにより、完走後に net48 サンプルのバイナリを残さない。**
`1_DeleteDir.bat` が配下の `bin` / `obj` を再帰的に削除するため、最後にビルドされた
Core サンプルだけが残る。このため `3_SmokeTest.ps1` は対象を自分でビルドする。

### リリース時の実行順

```powershell
cd root\programs
.\1_BuildAll.ps1                 # 全ビルドの合否
.\2_RunAllTests.ps1              # 単体テストの回帰
.\3_SmokeTest.ps1                # サンプルの疎通
```

---

## 9. GitHub Actions で回す（#517）

依存パッケージの更新（Dependabot）を `develop` に入れる前にビルドで確かめるため、
**受けブランチ `deps` を挟む**。

```
Dependabot が PR を作成（base: develop）
      │
      │ ① .github/workflows/dependabot-retarget.yml が base を deps へ書き換え
      ▼
 PR（base: deps）
      │ ② 人がマージ
      ▼
   deps ─── ③ .github/workflows/build-windows.yml が windows-latest でビルド
      │ ④ 結果が OK なら、人が PR を作って develop へマージ
      ▼
  develop
```

①③がワークフロー、②④が人の作業。`AGENTS.md` の線引き（マージは人が行う）に従う。

### ワークフローの前提

- **`deps` ブランチをあらかじめ作っておく。** `gh pr edit --base` は既存のブランチしか指せない
- **`dependabot-retarget.yml` は `develop`（既定ブランチ）に無いと動かない。**
  `pull_request_target` はベース ブランチ側の定義で動くため
- **④の後、`deps` を `develop` に合わせて進める。**
  古いままだと次の PR の差分に無関係な変更が混ざる

### なぜ `dependabot.yml` の `target-branch` ではないのか

**セキュリティ アップデートには効かないため。** `target-branch` が効くのはバージョン更新だけで、
セキュリティ アップデートは常に既定ブランチ（`develop`）へ出る。
振り替えるには Actions で `gh pr edit --base` するしかない。

### なぜ `pull_request_target` なのか

Dependabot が起こしたイベントでは `GITHUB_TOKEN` が既定で read-only になり、base を書き換えられない。
`pull_request_target` なら read-write にできる。

> **PR のコードを checkout してはいけない。**
> `pull_request_target` はベース ブランチ側の定義を**書き込み権限付きで**動かすため、
> PR の中身を実行すると、そこに任意のコードを書ける相手へ権限を渡すことになる。
> `dependabot-retarget.yml` は checkout せず、`gh` コマンドだけを実行し、
> 権限も `pull-requests: write` の 1 つに絞っている。

### CI に載せているのはビルドだけ

| スクリプト | CI | 理由 |
|---|---|---|
| `1_BuildAll.ps1` | **○** | DB を必要としない |
| `2_RunAllTests.ps1` | — | 6 件中 2 件（SimpleBatch）が Northwind を使う |
| `3_SmokeTest.ps1` | — | 18 件のほぼ全てが Northwind を使う |

**GitHub の Windows イメージに SQL Server が無い**（LocalDB と `sqlcmd` のみ）。
サンプルの接続文字列は `Data Source=localhost; User ID=sa` で、LocalDB
（`(localdb)\MSSQLLocalDB`・Windows 認証）では代用できない。
Docker は入っているが **Windows コンテナのみ**で、Linux の SQL Server イメージは動かない。
**Northwind の作成スクリプトもリポジトリに無い**（`CREATE ORDERS2.sql` だけ）。

これらを解消すれば 2・3 も載せられる。

### イメージ側の充足状況

`windows-latest`（windows-2025）で確認した対応関係。

| 必要なもの | イメージ |
|---|---|
| MSBuild | Visual Studio 2022 **Enterprise** 17.14 |
| .NET Framework 4.8 の参照アセンブリ | あり（4.8 / 4.8.1） |
| .NET 10 SDK | あり（10.0.302 ほか） |
| IIS Express | あり |
| `nuget.exe` | リポジトリに同梱（`root/programs/nuget.exe`） |
| SQL Server | **無し**（LocalDB と `sqlcmd` のみ） |

`z_Common.bat` は `vswhere` で MSBuild を解決するため、エディションが Enterprise でも通る。

### `VisualStudioVersion` を固定値から変更した

CI で Web アプリをビルドするために `z_Common.bat` を直した。

```bat
@rem 変更前
set VisualStudioVersion=18.0
```

Web アプリの csproj は、この値で targets のパスを組み立てる。

```xml
<VSToolsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
<Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" />
```

固定値 `18.0` は VS 18 のある環境でしか成立しない。CI の VS 2022（17.x）に渡すと
`v18.0\WebApplications\` を探し、**5 節と同じ `MSB4226`** になる。
このため、MSBuild を解決したのと同じ `vswhere` からバージョンを求めるようにした。

```bat
for /f "usebackq tokens=1 delims=." %%i in (
  `%VSWHERE% -latest -products * -requires Microsoft.Component.MSBuild -property installationVersion`
) do set VSVER_MAJOR=%%i
if not defined VSVER_MAJOR set VSVER_MAJOR=18
set VisualStudioVersion=%VSVER_MAJOR%.0
```

VS 18 のある環境では従来どおり `18.0` になるため、**ローカルの挙動は変わらない。**
