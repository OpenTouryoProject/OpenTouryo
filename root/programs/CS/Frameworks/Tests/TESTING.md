# TESTING.md — 単体テストの実行と判定

対象: `root/programs/CS/Frameworks/Tests`
本書は、リリース時に行っていた「単体テスト スクリプトの実行結果を diff で目視」を、
**機械判定に置き換える**ための手順と判定基準を記述する（#513 段階 1）。

リリース時の作業全体は [`RELEASE.md`](../../RELEASE.md) を参照。
実行順は `BuildAll.ps1` → `RunAllTests.ps1` → `SmokeTest.ps1`。

---

## 1. 使い方

```powershell
cd root\programs\CS\Frameworks\Tests

# ビルド バッチを実行し、再生成された結果を HEAD 版と比較して一覧表示する
.\RunAllTests.ps1

# バッチを実行せず、いま手元にある結果ファイルだけを比較する
.\RunAllTests.ps1 -SkipBuild

# 個別に比較する
.\CompareResult.ps1 -Expected <HEAD 版> -Actual TestCode\Result48.txt
.\CompareResult.ps1 -Expected <HEAD 版> -Actual TestBatch\ResultSimpleBatch48.txt -SkipLog4netTrace
```

終了コードは `0` = 全 OK、`1` = NG あり、`2` = ファイル無し。

---

## 2. 何を何と比較しているか

**従来の運用をそのまま機械化している。** 置き換えたのは最後の「目視」だけ。

`y_Build_TestCode*.bat` は「ビルド → テスト実行 → **結果を `Result*.txt` へ出力**」まで行う。
この `Result*.txt` は Git 管理下にあり、バッチを実行すると上書きされる。
つまり従来の手順は次のとおりだった。

1. バッチを実行して `Result*.txt` を再生成する
2. `git diff` で「前回のリリース時の結果」との差分を目視する

`RunAllTests.ps1` はこの構造を変えず、2 を機械比較に置き換える。

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

### 対象テスト（6 ケース）

| テスト | 結果ファイル（＝期待値） | ビルド バッチ | 備考 |
|---|---|---|---|
| TestCode (net48) | `TestCode/Result48.txt` | `y_Build_TestCode_Public.bat` | 部品層の総合テスト（約 2,300 行） |
| TestCode (net10.0) | `TestCode/ResultCore100.txt` | 同上 | 同上 |
| SimpleBatch (net48) | `TestBatch/ResultSimpleBatch48.txt` | `y_Build_TestCode_Batch.bat` | **DB 接続あり**（Northwind） |
| SimpleBatch (net10.0) | `TestBatch/ResultSimpleBatchCore100.txt` | 同上 | 同上 |
| EncAndDecUtilCUI (net48) | `EncAndDecUtilCUI/Result48.txt` | `y_Build_TestCode_SecCUI.bat` | 暗号・JWT・XML 署名 |
| EncAndDecUtilCUI (net10.0) | `EncAndDecUtilCUI/ResultCore100.txt` | 同上 | 同上 |

`EncAndDecUtilCUI/ResultCore100OnLinux.txt` は Linux 実行時の期待結果のため、
Windows からの `RunAllTests.ps1` の対象外。

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
cd "Frameworks\Tests\TestBatch\SimpleBatch\bin\Debug"
SimpleBatch.exe /Dap SQL ... > ..\..\..\ResultSimpleBatch48.txt
```

この変数が効いていると次のようになり、**結果ファイルが空のまま、バッチは成功したように進む**。

```
'SimpleBatch.exe' is not recognized as an internal or external command,
```

`RunAllTests.ps1` は、バッチを直接実行した場合と同じ条件にするため、この変数を解除している。

---

## 3. 前提条件

- **Frameworks をビルド済み**であること（`Build_net48` / `Build_netcore100` が存在）
- **SQL Server の Northwind に接続できる**こと（SimpleBatch が使用）
- **Northwind のテスト データが標準状態**であること（後述の「4. テスト データ」）

### テスト証明書について

`EncAndDecUtilCUI` の csproj は `*.cer` / `*.pfx` を `CopyToOutputDirectory` しているため、
**これらが無いとビルドが `MSB3030` で失敗する**。

これらは **Git 管理外の作業用コピー**であり、**正本はリポジトリ内の
`root/files/resource/X509/`**（10 件が Git 管理下）にある。

`y_Build_TestCode_SecCUI.bat` は先頭で `copy_cert.bat` を呼ぶが、こちらは `C:\root\files\...`
を参照するため、リポジトリだけを clone した環境では配置できない。
`RunAllTests.ps1` はリポジトリ内の正本から不足を**自動で配置**するため、手動操作は不要。
個別にビルドする場合のみ、次のいずれかを実行する。

```powershell
# リポジトリ内の正本から配置（推奨・配置場所に依存しない）
Copy-Item ..\..\..\..\files\resource\X509\SHA*.??? EncAndDecUtilCUI\

# あるいは同梱のバッチ（C:\root\files\resource\X509\ を参照する）
cd EncAndDecUtilCUI
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

`3` の実例（2026-08-01 に発生。復旧済み）:

```
SimpleBatch (net48 / net10.0)
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

| 環境 | 使用するスクリプト |
|---|---|
| Rancher Desktop 導入時（Windows から `docker` が使える） | `Stop-Services.ps1` → `Start-Services.ps1` |
| WSL2 内の Docker を使う場合 | `Stop-Services_wsl2.ps1` → `Start-Services_wsl2.ps1` |

```powershell
cd <LocalServicesOnDocker>
.\Stop-Services.ps1      # docker compose down（コンテナを破棄）
.\Start-Services.ps1     # docker compose up -d ＋ DB 初期化完了待ち
```

> **`docker restart` では戻らない。** 同一コンテナが再開されるだけで書き込み層が保持されるため。
> コンテナの破棄（`down`）と再作成（`up`）が必要。
> なお compose には `restart: always` が指定されているため、ホスト再起動でも戻らない。

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
