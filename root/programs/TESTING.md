# TESTING.md — 単体テストの実行と判定

対象: `root/programs/CS/Frameworks/Tests/Golden`（C# 側。フォルダの分け方は [`Tests/README.md`](CS/Frameworks/Tests/README.md)）
配置: `root/programs`
本書は、リリース時に行っていた「単体テスト スクリプトの実行結果を diff で目視」を、
**機械判定に置き換える**ための手順と判定基準を記述する（#513 段階 1）。

リリース時の作業全体は [`RELEASE.md`](RELEASE.md) を参照。
実行順は `1_BuildAll.ps1` → `2_RunAllTests.ps1` → `3_SmokeTest.ps1`。

> **DB のコンテナは、LocalServicesOnDocker の [`CHEATSHEET.md`](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker/blob/master/CHEATSHEET.md) に従って起動・停止する。**
> **メモリを節約しないと、最悪の場合、コンテナがシステムごとハングさせる**（Docker の VM がメモリを使い切る）。
>
> - **必要なサービスだけを、名前を指定して起動する。** 引数なしの `Start-Services.ps1` は全サービスを起動するので使わない
> - **必須でないコンテナを同時に複数起動しない。** クロス DB は、SQL Server（必須）に 1 つずつ足して回し、測り終えたら止める
>   （`2_RunAllTests.ps1 -CrossDb` がこれを自動で行う。下の `TestPublicDb` の説明）
> - **使い終わったら止める**（`Stop-Services.ps1 <サービス名>`）。状態は `Show-Services.ps1`（読み取り専用）で確かめる
> - Rancher Desktop 版（`*.ps1`）と WSL2 版（`*_wsl2.ps1`）は**別の Docker を見る。** 混ぜて使うと、同じサービスが二重に立つ
> - `oracle` と `sqlserver` は特に大きい

---

## 1. 使い方

```powershell
cd root\programs

# ビルド バッチを実行し、再生成された結果を HEAD 版と比較して一覧表示する
.\2_RunAllTests.ps1

# バッチを実行せず、いま手元にある結果ファイルだけを比較する
.\2_RunAllTests.ps1 -SkipBuild

# 対象を絞る（部分一致・複数可）。指定できる名前は -List で出す
.\2_RunAllTests.ps1 -Only "TestBatch", "NUnit"
.\2_RunAllTests.ps1 -List

# Linux のコンテナでも回す（2 節「Linux のテスト」。docker が要る）
.\2_RunAllTests.ps1 -Linux

# クロス DB（2 節「NUnit のテスト」の TestPublicDb）。DBMS を 1 つずつ起動 → 回す → 停止
.\2_RunAllTests.ps1 -CrossDb -LocalServicesDir <LocalServicesOnDocker のフォルダ>

# カバレッジを測って Tests\COVERAGE.md を書き直す（NUnit の全部・Linux の回・クロス DB。docker が要る）
.\2_RunAllTests.ps1 -Coverage -LocalServicesDir <LocalServicesOnDocker のフォルダ>

# 個別に比較する
.\CompareResult.ps1 -Expected <HEAD 版> -Actual Golden\TestCode\Result48.txt
.\CompareResult.ps1 -Expected <HEAD 版> -Actual Golden\TestBatch\ResultSimpleBatch48.txt -SkipLog4netTrace
```

終了コードは `0` = 全 OK、`1` = NG あり、`2` = ファイル無し。

- **コンテナを起動するのは `-Linux` / `-CrossDb` / `-Coverage` を付けたときだけ。** 付けなければ、今までどおり手元の DB を使うだけ
- `-Coverage` は `-Only` / `-SkipBuild` / `-Linux` / `-CrossDb` と、`-Linux` は `-CrossDb` と、同時に指定できない
- Linux のコンテナのイメージとメモリの上限は `-LinuxImage` / `-LinuxMemory` で変えられる（既定は `mcr.microsoft.com/dotnet/sdk:10.0` / `4g`）
- **CI（GitHub Actions）は、オプション無しの通常の実行だけ**を回す（windows-latest は Linux のコンテナを動かせない）。
  Linux・クロス DB・カバレッジは手元で回す

---

## 2. 何を何と比較しているか

**従来の運用をそのまま機械化している。** 置き換えたのは最後の「目視」だけ。

`y_Build_TestCode*.bat` は「ビルド → テスト実行 → **結果を `Result*.txt` へ出力**」まで行う。
この `Result*.txt` は Git 管理下にあり、バッチを実行すると上書きされる。
つまり従来の手順は次のとおりだった。

1. バッチを実行して `Result*.txt` を再生成する
2. `git diff` で「前回のリリース時の結果」との差分を目視する

`2_RunAllTests.ps1` はこの構造を変えず、2 を機械比較に置き換える。

| | 実体 | 取得方法 |
|---|---|---|
| **期待値** | HEAD にコミットされている `Result*.txt` | `git show HEAD:<path>`（参照系のみ） |
| **実測値** | バッチが再生成したワーキング ツリーの `Result*.txt` | バッチの実行 |

> **副作用**: ワーキング ツリーの `Result*.txt` が書き換わる。これは従来のバッチ運用と同じ。

**生 `git diff` を目視してはいけない。** 実行日時が全行に入るため、
内容が同じでも**ほぼ全行が差分になる**（実測で 6 ファイル約 2,458 行）。

```diff
-[2025/11/18 15:19:08,286],[INFO ],[1],,,,----->>,...
+[2026/08/01 22:40:19,772],[INFO ],[1],,,,----->>,...
```

判定は本スクリプトの**正規化後の差分**で行う。0 なら内容は同じ。
生 diff を読むのは、正規化後に差分が出たときだけでよい。
`Result*.txt` のコミットは、内容が変わった（新しい基準にしたい）ときだけでよい。

### 対象テスト（Golden 8 ケース）

| テスト | 結果ファイル（＝期待値） | ビルド バッチ | 備考 |
|---|---|---|---|
| TestCode (net48) | `Golden/TestCode/Result48.txt` | `y_Build_TestCode_Public.bat` | 部品層の総合テスト（約 2,500 行）。**DB に接続しない** |
| TestCode (net10.0) | `Golden/TestCode/ResultCore100.txt` | 同上 | 同上 |
| TestDataAccess (net48) | `Golden/TestDataAccess/Result48.txt` | `y_Build_TestCode_DataAccess.bat` | データ アクセス（#520）。**実行モードで対象 DBMS が変わる** |
| TestDataAccess (net10.0) | `Golden/TestDataAccess/ResultCore100.txt` | 同上 | 同上 |
| TestBatch (net48) | `Golden/TestBatch/ResultSimpleBatch48.txt` | `y_Build_TestCode_Batch.bat` | **DB 接続あり**（Northwind） |
| TestBatch (net10.0) | `Golden/TestBatch/ResultSimpleBatchCore100.txt` | 同上 | 同上 |
| EncAndDecUtilCUI (net48) | `Golden/EncAndDecUtilCUI/Result48.txt` | `y_Build_TestCode_SecCUI.bat` | 暗号・JWT・XML 署名 |
| EncAndDecUtilCUI (net10.0) | `Golden/EncAndDecUtilCUI/ResultCore100.txt` | 同上 | 同上 |

`Golden/EncAndDecUtilCUI/ResultCore100OnLinux.txt` は Linux 実行時の期待結果で、
**`-Linux` を付けたときだけ**比べる（下の「Linux のテスト」）。

### NUnit のテスト（8 ケース、#610）

**期待値ファイルを持たない。テスト自身が合否を出す。**
HEAD とは比較せず、バッチが書き出す `*.trx` の集計で判定する。

| テスト | 結果ファイル | ビルド バッチ | 判定 |
|---|---|---|---|
| NUnit TestPublic (net48) | `NUnit/TestResults/TestPublic/TestPublic_net48.trx` | `y_Build_TestCode_NUnit.bat` | 失敗 0 件、かつ 1 件以上実行 |
| NUnit TestPublic (net10.0) | `NUnit/TestResults/TestPublic/TestPublic_net10.0.trx` | 同上 | 同上。**カバレッジも表示する** |
| NUnit TestPublicNLog (net48) | `NUnit/TestResults/TestPublicNLog/TestPublicNLog_net48.trx` | 同上 | 同上 |
| NUnit TestPublicNLog (net10.0) | `NUnit/TestResults/TestPublicNLog/TestPublicNLog_net10.0.trx` | 同上 | 同上。**カバレッジも表示する** |
| NUnit TestPublicDb (net48) | `NUnit/TestResults/TestPublicDb/TestPublicDb_net48.trx` | 同上 | 同上。**DB 接続あり** |
| NUnit TestPublicDb (net10.0) | `NUnit/TestResults/TestPublicDb/TestPublicDb_net10.0.trx` | 同上 | 同上。**DB 接続あり**。カバレッジも表示する |
| NUnit TestFramework (net48) | `NUnit/TestResults/TestFramework/TestFramework_net48.trx` | 同上 | 同上 |
| NUnit TestFramework (net10.0) | `NUnit/TestResults/TestFramework/TestFramework_net10.0.trx` | 同上 | 同上。**カバレッジも表示する** |

**`TestFramework` は、`OpenTouryo.Framework` のうち単体で使える部品だけのテスト**（`Authentication` の鍵・値を引数で受ける版と、`Exceptions`）。
親クラス（`Business`・`Dao`・`Presentation`・`Transmission`）は組み合わせて使うものなので、疎通に任せる。
設定（`CmnClientParams` 等）を読む版や、サーバが要る `OAuth2AndOIDCClient`・`JwkSetStore` も対象外。

**`TestPublicNLog` は、`LogIF` を NLog に切り替えた状態のテスト**で、別のプロセスで動かすために分けている（使うライブラリはプロセスで 1 回しか決まらない）。カバレッジはプロジェクトごとに別に出る。

**`TestPublicDb` は Dam のクロス DB テスト**（DB に接続する）。同じテストを DBMS ごとに回す。
**SQL Server は必須**（接続できなければ失敗。CI でも接続する）。Oracle / MySQL / PostgreSQL は**接続できなければスキップ**し、件数と名前が「実行せず」に出る。
クロス DB で確かめるなら LocalServicesOnDocker の各 DBMS を起動してから回す（接続先は Golden の `TestDataAccess` と同じ）。PostgreSQL は `DamPstGrS` が .NET 専用のため net10.0 だけ。
**DBMS は 1 つずつ起動して回す**（冒頭の CHEATSHEET の注意）。起動していない DBMS はスキップされるだけなので、1 つずつでも漏れなく確かめられる。

**`-CrossDb -LocalServicesDir <LocalServicesOnDocker のフォルダ>` を付けると、これを自動で行う**（#623）。

```powershell
.\2_RunAllTests.ps1 -CrossDb -LocalServicesDir <LocalServicesOnDocker のフォルダ>
```

- 対象は `TestPublicDb` だけになる。まずバッチでビルドして SQL Server だけの回を回し、
  続けて **Oracle → MySQL → PostgreSQL を 1 つずつ「起動 → `--no-build` で回す → 停止」** する
- 起動・停止は LocalServicesOnDocker のスクリプトで行う。**`docker` が使えれば Rancher Desktop 版（`*.ps1`）、使えなければ WSL2 版（`*_wsl2.ps1`）**
- **始める前から動いていた DBMS は、起動も停止もしない。** SQL Server が動いていなければ起動し、終わったら止める
- **起動したのに、その DBMS のテストが 1 件も実行されなければ NG**（スキップのまま OK にしない）
- 回ごとの結果は `NUnit\TestResults\CrossDb\<サービス名>` に残る（カバレッジも回ごと）。サマリの名前は `TestPublicDb (net10.0) [oracle]` の形
- **コンテナを起動・停止するのは `-CrossDb` を付けたときだけ。** 付けなければ、今までどおり動いている DB を使うだけ

手で 1 つずつ回す場合、**バッチは実行のたびに `TestResults` を消す**ので、カバレッジは回した回の分しか残らない（Dam ごとの値は、その DBMS を起動した回の表示で見る）。

- **サマリの「差分」の列は、NUnit では失敗の件数を表す。**
- **0 件の合格は OK にしない。** テストが見つからない構成ミスを見逃すため
- **スキップ（`Assert.Ignore`）は OK に数えるが、件数と名前を出す。**
  いまは net48 のイベント ログのテスト 3 件が、**管理者でないとスキップされる**
  （イベント ソースの作成に管理者権限が要る。CI の windows-latest は管理者なので走る）。
  手元で確かめるなら、管理者の PowerShell で回す
- `TestResults/` は Git 管理外。**ワーキング ツリーは書き換わらない**
- バッチは実行前に古い結果を消す。ビルドに失敗したとき、前回の「合格」が残らないようにするため
- **カバレッジは表示だけで、判定には使わない。** 収集は net10.0 だけで行う
  （コードはほぼ共通で、両方で取っても指針としては変わらないため）
- **カバレッジを [`Tests/COVERAGE.md`](CS/Frameworks/Tests/COVERAGE.md) に残すのは `-Coverage` の回だけ**（`CoverageReport.ps1`）。
  `.\2_RunAllTests.ps1 -Coverage -LocalServicesDir <LocalServicesOnDocker のフォルダ>` は、NUnit の全部（Linux の回を含む）とクロス DB を 1 回で回し、
  全部 OK なら、アセンブリごとに通った行の和集合で表を作り直す（`OpenTouryo.Public` には NLog・DB の回で通った行も入る）。
  **普通の実行は COVERAGE.md を書かない。** `-Coverage` の回は `Result*.txt` と同じく**ワーキング ツリーが書き換わる**（コミットは人が決める）

テストの書き方と置き場は [`Tests/README.md`](CS/Frameworks/Tests/README.md) を参照。

### Linux のテスト（`-Linux`、#632）

**Windows でビルドした net10.0 の出力を、そのまま Linux のコンテナで回す。**
net10.0 の DLL は OS によらないので、Windows で作ったものを Linux に置く、という使われ方をそのまま確かめられる。

```powershell
.\2_RunAllTests.ps1 -Linux
```

| テスト | 結果ファイル | 判定 |
|---|---|---|
| EncAndDecUtilCUI (Linux) | `Golden/EncAndDecUtilCUI/ResultCore100OnLinux.txt` | HEAD 版と比べる（Windows の回と同じ） |
| NUnit TestPublic (Linux) | `NUnit/TestResults/Linux/TestPublic_linux.trx` | 失敗 0 件、かつ 1 件以上実行 |

- **`docker` コマンドが要る**（Rancher Desktop）。無ければ回さずに止める
- コンテナは**1 つだけ、`--rm` で起動する**（イメージは `mcr.microsoft.com/dotnet/sdk:10.0`、メモリの上限は 4g。`-LinuxImage` / `-LinuxMemory` で変えられる）。
  同じ名前（`opentouryo-linuxtest`）のコンテナが既にあれば、触らずに止める
- 出力のフォルダ（`bin\Debug\net10.0`）は**読み取り専用でマウント**し、コンテナの中で写してから動かす。ワーキング ツリーの bin / obj は変わらない
- Golden の結果は、Windows の回と同じくワーキング ツリーの `ResultCore100OnLinux.txt` を書き換える
- `[Platform("Win")]` のテスト（CNG など）は、Linux ではスキップに出る。`[Platform("Linux")]` のテスト（`DSAOpenSsl` など）は、Linux の回でしか走らない
- **`-Linux` を付けなければ、Linux の対象は回らない。** `-CrossDb` とは同時に指定できない
- **`SelectTargets.ps1` は、Public・Public.Security（と、それを Linux で回す `TestPublic` / `EncAndDecUtilCUI`）に触れたとき、`-Linux` を付ける**（fixed のみ。fixing では後回し）
- **TestPublic の回はカバレッジも取る**（表示する。COVERAGE.md に残すのは `-Coverage` の回）。
  Linux の coverlet は Windows 形式（full）の PDB を読めないので、**この回のためだけに基盤（Public・Public.Security）を
  portable の PDB でビルドし直し**（`--artifacts-path` で中間ファイルも分ける）、コンテナの中で差し替える。
  コレクタは NuGet のキャッシュの `coverlet.collector` を使う。用意できなければカバレッジ無しで回す
- **NUnit のテストは、portable の PDB でビルドする**（`y_Build_TestCode_NUnit.bat`）。
  Linux の .NET は Windows の PDB（full）を読めず、スタック フレームからソースのファイル名を取るテストが null になっていた

### `TestCode` / `TestDataAccess` は詳細を別文書に持つ

**「何をテストしているか」「ケースを書き足すときの決まり」は各プロジェクトの
`README.md` が一次情報。** 本書はここに写さない（二重管理になるため）。

| 文書 | そちらで扱うもの |
|---|---|
| [`TestCode/README.md`](CS/Frameworks/Tests/Golden/TestCode/README.md) | `Test*.cs` と対象名前空間の対応、環境依存の値を出さないための決まり、記録している「仕様」（読み違えやすい挙動） |
| [`TestDataAccess/README.md`](CS/Frameworks/Tests/Golden/TestDataAccess/README.md) | 実行モードと対象 DBMS、更新系・動的パラメタライズドクエリのケース設計の根拠、クロス DB テストの実施手順 |

`TestCode` と `TestDataAccess` を分けているのは、**`TestDataAccess` が DB を前提にする**ため。
混ぜると、DB が無い環境では `TestCode` ごと動かなくなる。

### `TestDataAccess` の実行モード

**クロス DB は CI では実行できない。** LocalServicesOnDocker の各 DBMS は Linux コンテナで、
`windows-latest` は Linux コンテナを動かせないため（[`BUILDING.md`](BUILDING.md) 9 節）。
このため `/MODE` で対象を切り替える。**期待値は `SQLONLY`（SQL Server のみ）で記録している。**

```
TestDataAccessFx.exe /MODE SQLONLY     … SQL Server だけ（既定。CI はこちら）
TestDataAccessFx.exe /MODE LOCAL       … ローカルで起動している DBMS をすべて
```

> **「接続するテスト」と一括りにはできない。** `TestDataAccess` の中でも、
> SQL を組み立てるだけで接続を必要としないものがある。詳細は上記の README。

### ビルドをバッチに委ねている理由

`csproj` / `sln` を直接 MSBuild してはならない。
`nuget.exe restore` が行う**ネイティブ DLL の配置が漏れる**ためで、
ビルドは成功するのに実行時に落ちる、という分かりにくい失敗になる。

```
System.DllNotFoundException
   at Microsoft.Data.SqlClient.SNINativeManagedWrapperX64.SNIInitialize(IntPtr)
```

`Microsoft.Data.SqlClient` は SNI をネイティブ DLL で持ち、
`bin\Debug\Microsoft.Data.SqlClient.SNI.x64.dll` 等が必要になる。
「何をどうビルドするか」の正はバッチ側に置く。

### PowerShell から `.bat` を呼ぶときの注意

PowerShell は子プロセスに `NoDefaultCurrentDirectoryInExePath=1` を渡す。
一方バッチは、出力フォルダへ `cd` したうえでパス区切りを含まない名前で実行している。

```bat
cd "Frameworks\Tests\Golden\TestBatch\SimpleBatch\bin\Debug"
SimpleBatch.exe /Dap SQL ... > ..\..\..\ResultSimpleBatch48.txt
```

この変数が効いていると次のようになり、**結果ファイルが空のまま、バッチは成功したように進む**。

```
'SimpleBatch.exe' is not recognized as an internal or external command,
```

`2_RunAllTests.ps1` は、バッチを直接実行した場合と同じ条件にするため、この変数を解除している。

---

## 3. 前提条件

- **Frameworks をビルド済み**であること（`Build_net48` / `Build_netcore100` が存在）
- **SQL Server の Northwind に接続できる**こと
  （`TestDataAccess` と `TestBatch` が使用。**14 本中 6 本**）
- **Northwind のテスト データが標準状態**であること（後述の「4. テスト データ」）

### 冒頭の「前提の確認」（#588）

**落ちるまで分からないと、差分を見て初めて気付くことになる。**
`2_RunAllTests.ps1` は、DB を使う対象が選ばれているときだけ、実行の冒頭で確認する。

```
=== 前提の確認 ===
  SQL Server   localhost / Northwind
```

- **接続文字列は `TestDataAccess\App.config` から読む**（対象が実際に使う設定）
- **サービス名では見ない。** 理由とタイムアウトの扱いは
  [`SMOKETEST.md`](SMOKETEST.md) 4 節と共通（実体も `Prerequisite.ps1` で共有）
- **表示だけで、判定は変えない。** 足りなくても止めない

> **ASP.NET 状態サービス（`aspnet_state`）は要らない。**
> `System.Web` / `System.Web.Extensions` は csproj の**参照だけ**で、
> `HttpContext` も `sessionState` も使っていない。
> CI も、単体テストの**後**で `aspnet_state` を開始している
> （`build-windows.yml`）。要るのは `3_SmokeTest.ps1` の Web アプリだけ。

> **GitHub Actions でも実行している。** 前提の揃え方（SQL Server の導入、Northwind の
> ロード、照合順序）は [`BUILDING.md`](BUILDING.md) 9 節が一次情報。
> **期待値は開発環境で生成されたもの**なので、CI 側の環境がずれると差分が出る。
> そのため CI は期待値と実測値の両方を artifact に残す。

### テスト証明書について

`EncAndDecUtilCUI` の csproj は `*.cer` / `*.pfx` を `CopyToOutputDirectory` しているため、
**これらが無いとビルドが `MSB3030` で失敗する**。

これらは **Git 管理外の作業用コピー**であり、**正本はリポジトリ内の
`root/files/resource/X509/`**（10 件が Git 管理下）にある。

`y_Build_TestCode_SecCUI.bat` は先頭で `copy_cert.bat` を呼ぶが、こちらは `C:\root\files\...`
を参照するため、リポジトリだけを clone した環境では配置できない。
`2_RunAllTests.ps1` はリポジトリ内の正本から不足を**自動で配置**するため、手動操作は不要。
個別にビルドする場合のみ、次のいずれかを実行する。

```powershell
# リポジトリ内の正本から配置（推奨・配置場所に依存しない）
Copy-Item ..\..\..\..\files\resource\X509\SHA*.??? Golden\EncAndDecUtilCUI\

# あるいは同梱のバッチ（C:\root\files\resource\X509\ を参照する）
cd Golden\EncAndDecUtilCUI
.\copy_cert.bat
```

`copy_cert.bat` は `C:\root\files\...` を参照するが、そちらは Samples の**実行時**要件であり、
単体テストのビルドには不要。リポジトリ内の正本から取る方が確実。

---

## 4. なぜ「目視」だったのか（正規化の必要性）

期待結果と実行結果には、**実行のたびに変わる値**が多数含まれる。
そのため素の diff では必ず差分が出る。実測値は次のとおり。

| テスト | 素の diff | 正規化後 |
|---|---|---|
| TestCode (net48) | 10 件 | **0 件** |
| TestCode (net10.0) | 11 件 | **0 件** |
| EncAndDecUtilCUI (net48) | 52 件 | **0 件** |
| EncAndDecUtilCUI (net10.0) | 52 件 | **0 件** |

`CompareResult.ps1` は次を正規化してから比較する。

| 種別 | 例 |
|---|---|
| 日時 | `[2026/08/01 14:39:50,027]` → `<DATETIME>` |
| 処理時間 | アクセス ログ末尾の `234,125` → `<ELAPSED>` |
| 絶対パス | `C:\OpenTouryo\...` → `<PATH>` |
| GUID | → `<GUID>` |
| XML 署名値 | `<SignatureValue>...</SignatureValue>` → `<XMLSIG>` |
| Base64URL | JWT・JWE の IV・認証タグ・鍵 → `<B64URL>` |
| Base64 | 鍵・ハッシュ → `<B64>` |

加えて、次の行は比較対象から除外する。

- **空行**
- **`-SkipLog4netTrace` 指定時の `log4net: ` 行**
  … `TestBatch` は 149 行中 145 行が log4net の内部トレースで、判定は残り 4 行
- **非対話実行の副産物**
  … サンプルは末尾に `Console.ReadKey()` を持つものがあり、
  出力をリダイレクトすると必ず例外で終わる。テスト内容とは無関係

---

## 5. 判定基準

**正規化後に残った差分は「実質的な差分」**であり、次のいずれかを意味する。

1. **退行** … コードの不具合。要調査
2. **期待結果ファイルの陳腐化** … 仕様変更に追随していない。期待結果を更新する
3. **テスト データの汚染** … DB のレコードが増減している。データを戻す

### 差分の「件数」は当てにならないことがある

**テストを追加したときなど、大きな挿入があると件数が実態より大きく出る。**

`CompareResult.ps1` は `Compare-Object -SyncWindow 20` で比較する。
**挿入した行数が同期窓（20 行）を超えると、以降の行が対応付かなくなり、
変わっていない行まで左右に並ぶ。**

実測（#522 でテストを 3 本追加したとき）。

| | 値 |
|---|---|
| 実際に増えた行数 | **119 行** |
| 表示された差分件数 | **309 件** |

**合否（OK / NG）は正しい。** 誤るのは件数と一覧の見え方だけ。

#### 「追加しかしていない」ことの確かめ方

件数を当てにせず、**生の行集合で突き合わせる。**
`HEAD` にあって実測に無い行が（日時などの揮発値を除いて）無ければ、
消えた出力は無い＝追加のみと分かる。

```powershell
$new = Get-Content <実測> -Encoding UTF8
$old = (git show "HEAD:<リポジトリ相対パス>") -split "`n" | ForEach-Object { $_.TrimEnd("`r") }
$old | Where-Object { $new -notcontains $_ }
```

> ここは**正規化前の生の行**を比べるため、日時を含む行は必ず残る
> （`TestOutputLog` の `LogIF.*Log` など）。それ以外が出なければ追加のみ。

`3` の実例（2026-08-01 に発生。復旧済み）:

```
TestBatch (net48 / net10.0)
  [実測のみ] 4件のデータがあります
  [期待のみ] 3件のデータがあります
```

Northwind の `Shippers` は標準で 3 件だが、`ShipperID=4` に `Speedy Express` の重複が
存在していた。CRUD サンプルの「追加」操作で挿入されたものと見られる。

このように、**件数などテスト対象の出力そのものは正規化してはならない**。
正規化すると、退行とデータ汚染のいずれも検出できなくなる。

**テスト データを汚す操作を行った後は、DB を戻してから単体テストを実行すること。**

```sql
-- 確認（標準は 3 件）
SELECT ShipperID, CompanyName FROM Shippers ORDER BY ShipperID;
```

### テスト データの戻し方

DB コンテナは**永続ボリュームを使用していない**（`docker-compose.yml` の
`./sqlserver/mssql-db:/var/opt/mssql` はコメントアウト）。
Northwind は起動のたびに `start-up.sh` が再作成するため、
**コンテナを作り直せば初期データに戻る**。

`LocalServicesOnDocker` の PowerShell 版スクリプトを使用する
（**DB の初期化完了待ちを行う**ため、`.bat` 版よりテスト用途に適している）。
**サービス名を必ず指定する**（引数なしは全サービスが対象になる。冒頭の CHEATSHEET の注意）。

| 環境 | 使用するスクリプト |
|---|---|
| Rancher Desktop 導入時（Windows から `docker` が使える） | `Stop-Services.ps1 sqlserver` → `Start-Services.ps1 sqlserver` |
| WSL2 内の Docker を使う場合 | `Stop-Services_wsl2.ps1 sqlserver` → `Start-Services_wsl2.ps1 sqlserver` |

```powershell
cd <LocalServicesOnDocker>
.\Stop-Services.ps1 sqlserver      # docker compose down（コンテナを破棄）
.\Start-Services.ps1 sqlserver     # docker compose up -d ＋ DB 初期化完了待ち
```

> **`docker restart` では戻らない。** 同一コンテナが再開されるだけで書き込み層が保持されるため。
> コンテナの破棄（`down`）と再作成（`up`）が必要。
> なお compose は `restart: "no"` なので、Docker エンジンを起動してもコンテナは自動では起動しない。

初期化完了待ちを省略したい場合は `-NoWait` を指定できるが、
その場合は Northwind のロード完了前にテストが始まり得るため、テスト用途では非推奨。

コンテナを作り直さず、個別に戻すこともできる。

```sql
DELETE FROM Shippers WHERE ShipperID > 3;
```

### 期待結果に含まれる「正常な失敗」

`EncAndDecUtilCUI` の期待結果には `False` を含む行が **3 件**含まれる。
これはプラットフォーム非対応等による既知の失敗であり、**正常**。
実測でも同数であることを確認すること（`CompareResult.ps1` は行単位で比較するため自動的に検出される）。

---

## 6. 正規化ルールを追加するとき

新しい非決定値が現れた場合は、`CompareResult.ps1` の `$normalizers` に追加する。

**追加してよいのは「実行のたびに変わる値」だけ**。
テスト対象の出力（件数・判定結果・型名など）を正規化してはならない。
差分を消すために安易にパターンを広げると、退行を検出できなくなる。

判断に迷う場合は、**同じ環境で 2 回実行して差分が出る値かどうか**を確認するとよい。
