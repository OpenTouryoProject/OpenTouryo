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

**しかも、出るエラーのコードがローカルと CI で違う。**

| 環境 | コード | 失敗する箇所 |
|---|---|---|
| ローカル | `MSB3482` | マニフェストの署名。`.pfx` が既にキー コンテナへ取り込まれているため、最後まで進む |
| CI | `MSB3325` ＋ `MSB3321` | `ResolveKeySource` によるキー ファイルの取り込み。署名に到達しない |

```
error MSB3325: Cannot import the following key file: WSClientWinCone_sample_TemporaryKey.pfx.
               The key file may be password protected. ...
error MSB3321: Importing key file "WSClientWinCone_sample_TemporaryKey.pfx" was canceled.
```

このため CI では 3 つのコードをまとめて、**当該プロジェクトに限定して**除外している（9 節）。

```powershell
-IgnoreErrors 'error MSB(3482|3325|3321):.*WSClientWinCone_sample\.csproj'
```

プロジェクト名まで含めているのは、同じコードが他のプロジェクトで出たときに
見逃さないため。コードだけで除外すると範囲が広すぎる。

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
      │
      │ ⓪ 人が deps を develop の先端に合わせる ← マージの前に必ず行う
      │ ② 人がマージ
      ▼
   deps ─── ③ .github/workflows/build-windows.yml が windows-latest で検証
      │ ④ 結果が OK なら、人が PR を作って develop へマージ
      ▼
  develop
```

①③がワークフロー、⓪②④が人の作業。`AGENTS.md` の線引き（マージは人が行う）に従う。

### ⓪ を飛ばしてはいけない

**`deps` が古いまま②を行うと、③が「`develop` に入る予定の状態」を検証しなくなる。**
`deps` と `develop` の差（他の作業で進んだ分）が混ざるためで、④の PR もその差を巻き込む。

```powershell
git fetch origin
git push origin origin/develop:deps    # 早送り。できないときは弾かれるので事故にならない
```

**早送りできるのは、`deps` に独自コミットが無い間だけ。**
②の後の `deps` は独自コミットを持つため、この形は使えない。
④で `develop` へ戻すと再び祖先になり、また早送りできる。**この周期を保つ。**

> ④の直後に⓪を済ませておくと、次の PR が来たときには既に揃っている。
> ただし、その後に `develop` が別の作業で進むこともあるため、
> **判断の基準は「④の後」ではなく「②の前」。**

> ⓪の push 自体が③を起こす（約 12 分）。`develop` の現状を検証する意味はある。
> 公開リポジトリなので、GitHub ホステッド runner の費用はかからない。

### 複数の PR を `deps` に溜める

**溜めてよい。** `deps` は使い捨てではなく常設のブランチで、②を何回か繰り返してから
④でまとめて `develop` へ戻せる。**それが受けブランチを置いた本来の使い方。**

```
PR ─┐
PR ─┼→ deps（②を繰り返す。マージのたびに③）→ ④ 1 本の PR → develop
PR ─┘
```

**切り分けは失われない。** ③のトリガは `push: branches: [deps]` なので、
5 本マージすれば③が 5 回走る。どのコミットで赤くなったかが残る。

| やり方 | 向き |
|---|---|
| 1 本ずつ③の緑を確認して次へ | 確実。1 本あたり約 12 分 |
| まとめてマージし、最後の③だけ見る | 速い。赤いときに戻す手間がかかる |

溜めるときの注意が 4 つある。

**① `develop` が進んだら取り込む。** 溜めている間に `develop` が動くと④で競合する。
⓪の基準が「②の前」なのは、この繰り返しでも同じ（**②のたびに⓪を見る**）。

**② 出力が変わる更新は `Result*.txt` を同じコミットに入れる。**
`deps` の上で `2_RunAllTests.ps1` を回して再生成する。
**ソースだけ先にコミットすると③が必ず赤くなる**（③は期待値を `HEAD` から取るため）。
実際に `System.Security.Cryptography.Xml` は `EncAndDecUtilCUI` の期待値に影響し得る。

**③ 同時実行を制御していない。** `build-windows.yml` に `concurrency` の指定が無いため、
連続してマージすると**古いコミットの run も最後まで走る**（12 分 × 本数）。
「まとめて最後だけ見る」を常用するなら次を足すと無駄が消える。

```yaml
concurrency:
  group: build-${{ github.ref }}
  cancel-in-progress: true
```

**ただし「1 本ずつ確認する」ときは邪魔になる**（確認したい run が消える）。
運用を決めてから入れること。現状は入れていない。

**④ Dependabot が上書き PR を出すことがある。** 溜めている間に次の版が出ると、
Dependabot は**古い PR を閉じて新しい PR を開く**。既にマージ済みの分は `deps` に残るため、
**同じパッケージの版が 2 回進む**コミットが並ぶ。害は無いが履歴は読みにくくなる。

### 任意のブランチで手動実行する

`build-windows.yml` は `workflow_dispatch` を持つので、**`deps` 以外のブランチでも同じ検証を回せる。**

```
Actions → Build on Windows → Run workflow → Branch: <任意のブランチ>
```

実行されるのは**選んだブランチ側の定義**。`deps` への push と同じ内容
（ビルド → 単体テスト → 疎通）が走る。

成立の条件は 2 つ。

| 条件 | 内容 |
|---|---|
| `develop`（既定ブランチ）に `workflow_dispatch` があること | 「Run workflow」ボタンの表示条件 |
| **選ぶブランチにも `.github/workflows/build-windows.yml` があること** | 定義はそのブランチから読まれるため |

2 つ目が実務上の注意になる。**`develop` より前に分岐した古いブランチにはファイルが無い**ので、
選んでも動かない。先に `develop` を取り込むこと。

> **手動実行でもクロス DB にはならない。** 対象は `SQLONLY`（SQL Server のみ）のまま。
> 各 DBMS は Linux コンテナで、`windows-latest` では動かせない。
> クロス DB は手元で行う（[`TESTING.md`](TESTING.md)）。

### ワークフローの前提

- **`deps` ブランチをあらかじめ作っておく。** `gh pr edit --base` は既存のブランチしか指せない
- **`dependabot-retarget.yml` は `develop`（既定ブランチ）に無いと動かない。**
  `pull_request_target` はベース ブランチ側の定義で動くため

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

### CI に載せている範囲

| スクリプト | CI | 備考 |
|---|---|---|
| `1_BuildAll.ps1` | **○** | DB を必要としない |
| `2_RunAllTests.ps1` | **○** | SQL Server を runner に導入して対応 |
| `3_SmokeTest.ps1` | **○** | `aspnet_state` を開始すれば動く |

**リリース時の検証 3 本が、そのまま同じ順で回っている。**

**対象外は明示しておく。**

- **GUI アプリケーション**（WinForms / WPF / 各ツールの画面）… `RELEASE.md` 4 節の手作業
- **SQL Server 以外の DBMS への接続** … 自動テストは `/Dap SQL` しか使わない
  （`TestSQLUtility` は 4 DBMS 分の SQL 生成を検証するが、**接続はしない**ので対象内）

### SQL Server をどう用意するか

**`LocalServicesOnDocker` の `docker-compose.yml` はそのままでは使えない。**

```yaml
sqlserver:
  image: mcr.microsoft.com/mssql/server:2022-latest   # ← Linux イメージ
```

`windows-latest` は Linux コンテナを動かせない。Docker は入っているが Windows コンテナ専用で、
Linux コンテナには Hyper-V による VM が要り、runner 自体が既に入れ子の VM のため
多段のネストができない。

**そこで、runner へ直接 SQL Server を入れ、同リポジトリの `instnwnd.sql` だけを使う。**

| 要素 | 入手先 |
|---|---|
| SQL Server 2022 | `ankane/setup-sqlserver`（**コミットで固定**。`v1` はタグではなくブランチ） |
| `instnwnd.sql`（Microsoft 公式の Northwind DDL、約 1 MB） | `LocalServicesOnDocker`（**コミットで固定**） |
| `CREATE ORDERS2.sql` | 本リポジトリに同梱 |

初期化で押さえている点は 3 つ。

1. **`sa` のパスワードを構成ファイルに合わせる。**
   構成側を CI 用に書き換えると「実際に使われる設定」と乖離するため、DB 側を
   `seigi@123`（`app.config` / `appsettings.json` の値）に変更する
2. **照合順序を `Japanese_CI_AS` にする。**
   `docker-compose.yml` の `MSSQL_COLLATION` と同じ。`CREATE DATABASE ... COLLATE` で
   DB 既定として与える。Northwind の列は `COLLATE` 句を持たないため、各列がこれを継承する
3. **ロードの完了を `Shippers` が 3 行あることで確認し、不完全なら作り直す。**
   起動直後は一部のバッチが失敗して表だけできることがある。
   判定方法は `LocalServicesOnDocker` の `start-up.sh` に合わせている

> **`instnwnd.sql` は DB を作らない。** 対象 DB の中で実行するスクリプトなので、
> 先に `CREATE DATABASE Northwind` が要る。

### テストが前提とする環境（DB 以外に 2 つある）

初回の実行で 6 件すべてが NG になり、次の 2 つが不足していると分かった。

#### ① `C:\root` が要る

**構成ファイルが絶対パスを直書きしている。**

| ファイル | キー | 値 |
|---|---|---|
| `TestCode/App.config` | `FxXMLMSGDefinition` | `C:\root\files\resource\Xml\MSGDefinition.xml` |
| `TestCode/App.config` | `FxXMLSPDefinition` | `C:\root\files\resource\Xml\SPDefinition.xml` |
| `TestBatch/SimpleBatch/app.config` | `SqlTextFilePath` | `C:\root\files\resource\Sql` |

無いと、`GetMessage` は**例外を出さずに空を返し**、SimpleBatch は次で落ちる。

```
resource file [C:\...\ShipperCount.sql] was not found.
   at Touryo.Infrastructure.Public.IO.ResourceLoader.LoadAsString(...)
```

実体はリポジトリ内（`root/files`、984 ファイル / 4.5 MB）にあるので、
**コピーせずジャンクションで繋ぐ**（実体を 1 つに保つ）。

```powershell
New-Item -ItemType Junction -Path 'C:\root' -Target "$env:GITHUB_WORKSPACE\root"
```

#### ② 日本語環境が要る（「書式」と「言語」は別物）

**期待値は日本語環境で生成されている。** runner の既定は `en-US` で、**2 つの軸がずれる。**

| 軸 | 何が変わるか | 直し方 |
|---|---|---|
| `CurrentCulture` | 日付・数値の**書式** | `Set-Culture ja-JP` |
| `CurrentUICulture` | メッセージの**言語** | 言語パックの導入 |

**`Set-Culture` は前者しか変えない。** 順に潰す必要がある。

##### 書式（`CurrentCulture`）

```
期待 : 昭和52年4月24日（日）, ggy年M月d日（ddd）: <DATE> 0:00:00
実測 : 昭和52年4月24日（日）, ggy年M月d日（ddd）: 4/24/1977 12:00:00 AM
```

`CompareResult.ps1` の `<DATE>` は `\d{4}/\d{1,2}/\d{1,2}`、つまり
**`1977/4/24` の形にしか一致しない。** `4/24/1977` は素通りする。

##### 言語（`CurrentUICulture`）

**2 種類あるが、根は同じ。**

**1. `GetMessage` はカルチャでファイルを選び分ける。**

```
実測 : GetMessage: - Description corresponding to the message-ID:I0001(normal system) -
期待 : GetMessage: ～メッセージID:I0001に対応する記述（正常系）～
```

`root/files/resource/Xml/` には 3 つある。

```
MSGDefinition.xml        ← 英語（既定・フォールバック）
MSGDefinition_ja.xml     ← 日本語
MSGDefinition_zh-CN.xml
```

`GetMessage.cs` が `CurrentUICulture` から `_ja` 付きの名前を組み立てる。
`en-US` では該当が無く、既定の英語版に落ちる。**例外にならないので気付きにくい。**

> 設定で固定することもできる。`GetMessage.cs` は `FxBusinessMessageCulture`（`appSettings`）が
> あればそれを優先する。ただし構成ファイルを CI の都合で変えると、
> **`CurrentUICulture` を見る経路自体がテストされなくなる。**

**2. 暗号系の例外文字列は Windows が返す。**

```
実測 : ... System.Security.Cryptography.<B64URL>, Key does not exist.
期待 : ... System.Security.Cryptography.<B64URL>, キーがありません。
```

##### 対処

**書式は runner 側で直せる。言語は直せない。**

```powershell
Set-Culture ja-JP     # これは効く（現在のセッションには反映されないので子プロセスで確認する）
```

**`CurrentUICulture` を日本語にする手段は、実質存在しない。** 試して駄目だったものを挙げる。

| 試したこと | 結果 |
|---|---|
| `Install-Language -Language ja-JP` | **ハング。** Windows Update 経由で取得しようとして応答が返らず、ステップが 16 分以上停止（run 30976630947） |
| `Set-WinUILanguageOverride` | 言語パックの導入が前提のため使えない |
| `HKCU\Control Panel\Desktop\PreferredUILanguages` を直接書く | **無視された。** Windows は未インストールの言語を候補から外す |

ワークフローの出力で確定している。

```
新しいプロセスの Culture / UICulture : ja-JP / en-US
```

そこで、言語に依存する 2 か所を**別々に処理する。**

**1. `GetMessage` は設定で固定する。**

`TestCode` の `App.config` と `appsettings.json` に次を足した。

```xml
<add key="FxBusinessMessageCulture" value="ja-JP" />
```

`GetMessage.cs` は、この指定があれば `CurrentUICulture` より優先する。
`ja-JP` → `ja` のフォールバック（`GetMessage.cs` の `currentUICulture.Parent`）で
`MSGDefinition_ja.xml` に解決されるため、**ローカルで起きている経路と同じ**になる。

> **これは CI のための小細工ではなく、テストの決定性の問題。**
> 固定しないと「どのマシンで動かすか」で結果が変わる。

**2. OS のメッセージは正規化する。**

`CompareResult.ps1` に規則を足し、**文言だけ**を `<OSMSG>` に潰す。

```powershell
@{ Name = 'OSメッセージ'
   Pattern = 'キーがありません。|Key does not exist\.|プロバイダーの公開キーは無効です。|Provider''s public key is invalid\.'
   Replace = '<OSMSG>' }
```

**行ごと落とさないのが要点。** 例外が起きたこと自体は「行の存在」で分かるため、
行を残しておけば、**例外が起きなくなったときに差分として検知できる。**

##### 潰した順序

段階的に切り分けた。**一度に直そうとすると、どれが効いたのか分からなくなる。**

| 実行 | 差分 | 何が分かったか |
|---|---|---|
| 1 回目 | 18 / 18 / 19 / 20 / 2 / 4 | `C:\root` の欠落と、ロケールの両方が出ていた |
| 2 回目（`C:\root` ＋ `Set-Culture`） | 4 / 4 / **0** / **0** / 2 / 4 | SimpleBatch は解決。日付も解決。言語だけが残った |
| 3 回目（`PreferredUILanguages`） | 4 / 4 / 0 / 0 / 2 / 4 | **変化なし** → UI 言語は runner 側では直せないと確定 |
| 4 回目（設定 ＋ 正規化） | **すべて 0** | 完了 |

### `2_RunAllTests.ps1` を CI で回すときの注意

**これは回帰テストで、期待値は `HEAD` にコミットされた `Result*.txt`。**
その期待値は開発環境で生成されたものなので、**照合順序・ロケール・タイム ゾーンが
1 つでもずれると差分が出る。**

サーバー レベルの照合順序までは合わせていない（変更にはインスタンスの再構築が要る）。
DB レベルだけを合わせているため、`tempdb` を経由する比較などでは差が出る余地が残る。

差分が出たときのために、ワークフローは**期待値と実測値の両方を artifact に採取する**。

| artifact | 中身 |
|---|---|
| `build-logs` | `1_BuildAll.ps1` のステップ別ログ |
| `test-results` | 期待値（`HEAD` 版）・ビルド ログ・再生成された `Result*.txt` |
| `smoke-logs` | 各対象の標準出力（Web 系は IIS Express の起動ログも含む） |

### `3_SmokeTest.ps1` の前提

**DB は追加不要。** `3_SmokeTest.ps1` も `/DAP SQL` しか使わないため、
`2_RunAllTests.ps1` のために用意した SQL Server で足りる。
`Orders2` は `Set up Northwind` で作成済み（無くても `3_SmokeTest.ps1` が作る）。

足りないのは 1 つだけだった。

```powershell
Start-Service aspnet_state
```

**`3_SmokeTest.ps1` は、これを自分では行わない。** サービスの開始はシステムの状態を
変える操作であり、足りない場合は対処方法を示して NG にする設計（`AGENTS.md` の線引き）。
**runner は使い捨てでこの判断が当てはまらない**ため、ワークフロー側で開始する。

IIS Express は同梱されている。

```
[有] C:\Program Files\IIS Express\iisexpress.exe        (v10.0.26013.1000)
[有] C:\Program Files (x86)\IIS Express\iisexpress.exe  (v10.0.26013.1000)

[有] aspnet_state : Status=Stopped StartType=Manual
     開始しました : Status=Running
```

`3_SmokeTest.ps1` が見るのも `%ProgramFiles%` 側なので、そのまま使える。

> **調べ方。** 一時的に `workflow_dispatch` だけのワークフローを置いて実行した。
> 12 分のビルドを回さずに 10 秒で答えが出る。
> 同種の疑問が出たら、また作って捨てればよい。

### イメージ側の充足状況

| 必要なもの | イメージ |
|---|---|
| MSBuild | **Visual Studio 18 Enterprise** |
| .NET Framework 4.8 の参照アセンブリ | あり |
| .NET 10 SDK | あり |
| **IIS Express** | **あり**（`%ProgramFiles%` / `%ProgramFiles(x86)%` の両方） |
| **`aspnet_state`** | **あり**（`StartType=Manual`。開始できることを確認済み） |
| `nuget.exe` | リポジトリに同梱（`root/programs/nuget.exe`） |
| SQL Server | **無し**（LocalDB と `sqlcmd` のみ） |

`windows-latest` の実体は次のとおり（実行ログで確認）。

```
Image   : win25-vs2026
OS      : Microsoft Windows Server 2025 Datacenter (10.0.26100)
Version : win25-vs2026/20260728.188
```

`z_Common.bat` は `vswhere` で MSBuild を解決するため、**エディションが Enterprise でも通る。**
固定パスの並びは Community しか見ていないので、そちらだけでは解決できない。

```
BUILDFILEPATH15
BUILDFILEPATH16
BUILDFILEPATH17
BUILDFILEPATH18
BUILDFILEPATH "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
```

> **どの README を見ているかを確かめること。** 本書は当初「VS 2022 Enterprise 17.14」と
> 書いていたが、これは `Windows2025-Readme.md`（VS 2022 のイメージ）を見ていたため。
> `windows-latest` の実体は `windows-2025-vs2026` で、正は `Windows2025-VS2026-Readme.md`。
> **バージョンや実在に依存する判断は、実行ログで裏を取ってから行う。**

### `VisualStudioVersion` を固定値から変更した

`z_Common.bat` が固定値を持っていたのを、`vswhere` から求めるようにした。

```bat
@rem 変更前
set VisualStudioVersion=18.0
```

Web アプリの csproj は、この値で targets のパスを組み立てる。

```xml
<VSToolsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
<Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" />
```

固定値 `18.0` は **VS 18 のある環境でしか成立しない。** 例えば VS 2022（17.x）だけの環境に
渡すと `v18.0\WebApplications\` を探し、**5 節と同じ `MSB4226`** になる。
このため、MSBuild を解決したのと同じ `vswhere` からバージョンを求めるようにした。

```bat
for /f "usebackq tokens=1 delims=." %%i in (
  `%VSWHERE% -latest -products * -requires Microsoft.Component.MSBuild -property installationVersion`
) do set VSVER_MAJOR=%%i
if not defined VSVER_MAJOR set VSVER_MAJOR=18
set VisualStudioVersion=%VSVER_MAJOR%.0
```

VS 18 のある環境では従来どおり `18.0` になるため、**挙動は変わらない。**

> **結果としては、この変更が無くても CI は通っていた。** runner にも VS 18 が入っており、
> 固定値 `18.0` と一致していたため（`VisualStudioVersion 18.0` がログに出ている）。
> ただし一致は偶然で、イメージの更新で崩れる。導出に変えたこと自体は妥当と判断した。

### 実測（run 30984111639 : 全ステップ成功）

| ステップ | CI | ローカル |
|---|---|---|
| Build all（31 ステップ） | 4 分 56 秒 | 5.8 分 |
| **Install SQL Server** | **5 分 22 秒** | －（導入済み） |
| Set up Northwind | 14 秒 | －（導入済み） |
| Set up C:\root and locale | 11 秒 | －（導入済み） |
| Run all tests（6 件） | 1 分 29 秒 | 1.3 分 |
| Start aspnet_state | 1 秒 | －（手動） |
| Smoke test（18 件） | 2 分 25 秒 | 2.4 分 |
| **合計** | **約 15 分** | 約 9.5 分 |

**検証そのものの所要はローカルとほぼ同じ。差は環境の用意（約 5 分半）。**
短縮したい場合は `Install SQL Server` が対象になる。

疎通 18 件はすべて OK だった。Web 系 3 件（IIS Express の起動、`aspnet_state` を使う
セッション、`__VIEWSTATE` を伴うポストバック、認証後のリダイレクト）も
**追加の調整なしで通っている。**

```
MVC_Sample (net48)                OK   ログイン後 /Crud1/Index = 200
WebForms_Sample (net48)           OK   ログイン後 menu.aspx = 200
MVC_Sample (net10.0)              OK   ログイン後 /Crud1/Index = 200
```

`CLI_sample (net48)` と `Framework_WSCore` はログが 27 行しかないが、**失敗ではない。**
どちらもバッチ側で意図的に無効化されており、メッセージだけを出して終わる。

```
.NET Fx系のSystem.CommandLine と Sharprompt 問題で一時的？ドロップ
Core系のBinarySerializeの完全廃止対応
```

### アクションのバージョン

`actions/checkout` と `actions/upload-artifact` は **`@v7`** を使う。
`@v4` は Node.js 20 を対象としており、次の警告が出る。

```
Node.js 20 is deprecated. The following actions target Node.js 20 but are being
forced to run on Node.js 24: actions/checkout@v4, actions/upload-artifact@v4
```
