# TESTING.md — 単体テストの実行と判定

対象: `root/programs/CS/Frameworks/Tests`
本書は、リリース時に行っていた「単体テスト スクリプトの実行結果を diff で目視」を、
**機械判定に置き換える**ための手順と判定基準を記述する（#513 段階 1）。

---

## 1. 使い方

```powershell
cd root\programs\CS\Frameworks\Tests

# 全テストをビルド・実行し、期待結果と比較して一覧表示する
.\RunAllTests.ps1

# ビルドを省略して実行だけ行う
.\RunAllTests.ps1 -SkipBuild

# 個別に比較する
.\CompareResult.ps1 -Expected TestCode\Result48.txt -Actual <実行結果>
.\CompareResult.ps1 -Expected TestBatch\ResultSimpleBatch48.txt -Actual <実行結果> -SkipLog4netTrace
```

終了コードは `0` = 全 OK、`1` = NG あり、`2` = ファイル無し。

---

## 2. 対象テスト（6 ケース）

| テスト | 期待結果ファイル | 備考 |
|---|---|---|
| TestCode (net48) | `TestCode/Result48.txt` | 部品層の総合テスト（約 2,300 行） |
| TestCode (net10.0) | `TestCode/ResultCore100.txt` | 同上 |
| SimpleBatch (net48) | `TestBatch/ResultSimpleBatch48.txt` | **DB 接続あり**（Northwind） |
| SimpleBatch (net10.0) | `TestBatch/ResultSimpleBatchCore100.txt` | 同上 |
| EncAndDecUtilCUI (net48) | `EncAndDecUtilCUI/Result48.txt` | 暗号・JWT・XML 署名 |
| EncAndDecUtilCUI (net10.0) | `EncAndDecUtilCUI/ResultCore100.txt` | 同上 |

`EncAndDecUtilCUI/ResultCore100OnLinux.txt` は Linux 実行時の期待結果のため、
Windows からの `RunAllTests.ps1` の対象外。

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

`RunAllTests.ps1` は不足を検知して**自動で配置**するため、手動操作は不要。
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

`3` の実例（2026-08-01 時点で発生中）:

```
SimpleBatch (net48 / net10.0)
  [実測のみ] 4件のデータがあります
  [期待のみ] 3件のデータがあります
```

Northwind の `Shippers` は標準で 3 件だが、`ShipperID=4` に `Speedy Express` の重複が
存在する。CRUD サンプルの「追加」操作で挿入されたものと見られる。
**テスト データを汚す操作を行った後は、DB を戻してから単体テストを実行すること。**

```sql
-- 確認
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
