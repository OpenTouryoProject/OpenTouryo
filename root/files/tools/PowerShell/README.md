# ClaudeSpace PowerShell ツール

## フォルダ（#687）

`root/programs` の直下を、利用者が叩く入口（`0_RunAll.ps1`〜`3_SmokeTest.ps1`）に絞るため、補助のスクリプトをここへ移した。

| フォルダ | 中身 | 使い方の一次情報 |
|---|---|---|
| （直下） | 開発機の道具（コンテナ・システムの監視、関連リポジトリの場所） | 本書 |
| `Verify/` | 検証の補助（回す範囲の割り出し `SelectTargets.ps1`、設定・版・脆弱性の点検 `Compare*.ps1`・`CheckVulnerability.ps1`、カバレッジ・根拠の集計、入口が読む部品 `SummaryTable.ps1`・`Prerequisite.ps1`） | [`CHEATSHEET.md`](../../../programs/CHEATSHEET.md) 0・1 節 |
| `SmokeTest/` | 疎通（`root/programs/3_SmokeTest.ps1`）がドット ソースで読む部品（`st_*.ps1`）。**単体では動かない** | [`SMOKETEST.md`](../../../programs/SMOKETEST.md) 8.5 節 |
| `Dev/` | 開発環境の準備（エージェント用のスキルの取得 `GetAgentSkills.ps1`） | [`AGENTS.md`](../../../../AGENTS.md) |

- **`Verify/`・`SmokeTest/`・`Dev/` のスクリプトは、`root/programs` を自分の場所からの相対パス（`..\..\..\..\programs`）で引く。** フォルダの深さを変えたら、そこを直す
- `SelectTargets.ps1` の記録（`.verified-commit`）と、`CoverageReport.ps1` が使う ReportGenerator のマニフェスト（`.config/dotnet-tools.json`）は、`root/programs` に置いたまま

## ワンライナー集

このフォルダで実行します。`powershell.exe`(5.1)でも `pwsh` でも動作します。

```powershell
# --- コンテナ状況 (Check-Containers.ps1) ---
.\Check-Containers.ps1                      # 実行中のコンテナ一覧
.\Check-Containers.ps1 -All                 # 停止中も含める
.\Check-Containers.ps1 -All -Stats          # CPU/メモリも表示
.\Check-Containers.ps1 -Stats -Watch 5      # 5秒ごとに自動更新
.\Check-Containers.ps1 -All -K8s            # k8s_ コンテナも個別表示
.\Check-Containers.ps1 -Engine podman       # Podman を使う

# --- システムモニター (SystemMonitor.ps1) ---
.\SystemMonitor.ps1                         # 3秒間隔、上位5プロセス
.\SystemMonitor.ps1 -Interval 5 -Top 10     # 5秒間隔、上位10プロセス
.\SystemMonitor.ps1 -Interval 1 -Top 3      # 1秒間隔で素早く確認

# --- 実行ポリシーで止められる場合 (そのセッションのみ許可) ---
powershell -ExecutionPolicy Bypass -File .\Check-Containers.ps1 -All -Stats
powershell -ExecutionPolicy Bypass -File .\SystemMonitor.ps1
```

終了は `Ctrl+C` です(`-Watch` 指定時と SystemMonitor)。

---

## Check-Containers.ps1

Docker / Podman のコンテナ実行状況を1つの表で表示します。エンジンは `docker`、`podman` の順に自動検出します。

### パラメータ

| パラメータ | 説明 |
|---|---|
| `-All` | 停止中のコンテナも表示 |
| `-Stats` | CPU / メモリ列を追加(実行中のコンテナのみ値が入る) |
| `-K8s` | `k8s_` で始まるコンテナを個別表示(既定は件数のみ。pause コンテナは常に非表示) |
| `-Image` | イメージ列を追加 |
| `-Engine` | `docker` / `podman` を明示指定 |
| `-Watch <秒>` | 指定秒数ごとに画面を更新 |

### 出力例

```
[docker] 18:20:25  実行中 16 / 全 34
NAME                    STATE    STATUS               CPU    MEM       PORTS
mpas-store-oracle-1     running  2 minutes (healthy)  4.17%  2.362GiB  1522->1521
mpas-store-sqlserver-1  exited   Exited (137) 2 minutes ago
k8s: 実行中 11 / 全 26  (個別表示は -K8s)
```

### 色の意味

| 色 | 意味 |
|---|---|
| 緑 | running |
| 黄 | starting / restarting / paused |
| 赤 | unhealthy、その他の異常状態 |
| 濃い黄 | 異常終了(終了コードが 0 以外の Exited) |
| 灰 | 正常終了または created |

### 備考

- ポートは IPv6 と `0.0.0.0:` の重複を除いて `ホスト->コンテナ` 形式で表示します。
- デーモンに接続できない場合は、メッセージを表示して終了コード 1 で終了します。
- 日本語を含むため、ファイルは **UTF-8 (BOM 付き)** で保存してください。BOM なしだと Windows PowerShell 5.1 で構文エラーになります。

---

## SystemMonitor.ps1

CPU とメモリの使用状況を一定間隔で表示します。

### パラメータ

| パラメータ | 既定値 | 説明 |
|---|---|---|
| `-Interval` | 3 | 更新間隔(秒) |
| `-Top` | 5 | 表示する上位プロセス数 |

### 表示内容

- 全体の CPU 使用率とメモリ使用量(バー表示)
- コア別の CPU 使用率
- CPU 使用率の上位プロセス
- メモリ使用量の上位プロセス

使用率が 70% 以上で黄、90% 以上で赤になります。

### 備考

- CPU 値は CIM のパフォーマンスクラスから取得するため、OS の言語設定に依存しません。
- プロセスのメモリは WorkingSetPrivate(プライベートワーキングセット)です。
- Windows 専用です。

---

## Register-RepoPaths.ps1・RepoPath.ps1・_RepoPaths.csv（関連リポジトリの場所。#652）

**ほかのリポジトリの場所を、ユーザーの環境変数に登録し、スクリプトから引く。**

| ファイル | 役割 |
|---|---|
| `_RepoPaths.csv` | **変数名・リポジトリ・既定の場所の一覧（雛形。コミットする）**（列 : `Variable`・`Repository`・`DefaultPath`・`UsedBy`）。**唯一の出どころ**で、スクリプトに既定の場所を書き込まない |
| `RepoPaths.csv` | **手元用**（`.gitignore` の対象）。手元の配置が雛形と違うなら、`_RepoPaths.csv` を写して `DefaultPath` を直す。**在れば雛形より優先する** |
| `Register-RepoPaths.ps1` | CSV の行ごとに、ユーザーの環境変数を登録する。**`-Apply` を付けなければ、どうなるかを見るだけ** |
| `RepoPath.ps1` | ドット ソースで読む関数 `Resolve-RepoPath`。**引数 ＞ 環境変数 ＞ CSV（`RepoPaths.csv`、無ければ `_RepoPaths.csv`）の既定（そのフォルダが在るときだけ）** の順に決め、どれを使ったかを表示する。決まらなければ `$null` |

```powershell
.\Register-RepoPaths.ps1                                  # 見るだけ
Copy-Item .\_RepoPaths.csv .\RepoPaths.csv              # 手元の配置に直すとき（写して DefaultPath を直す）
.\Register-RepoPaths.ps1 -Apply                           # RepoPaths.csv（無ければ _RepoPaths.csv）の既定の場所で登録する
.\Register-RepoPaths.ps1 -Only OT_MPAS_PATH,OT_LSOD_PATH -Apply  # 絞る（CSV に無い名前は警告する）
.\Register-RepoPaths.ps1 -Csv <パス> -Apply               # 別の CSV で登録する
```

- **フォルダが無い行は登録しない**（打ち間違い・まだ clone していないリポジトリ）
- **雛形（`_RepoPaths.csv`）の既定を使ったとき・雛形で登録するときは、警告を出す**（手元の配置と合っている保証が無い）
- **登録した後は、コンソールや Visual Studio を開き直す**（開いているプロセスには反映されない）
- **変数名は `OT_<略号>_PATH`。略号はリポジトリごとに決める**（`OT_OTR_PATH`・`OT_MPAS_PATH`・`OT_LSOD_PATH` など）。値はリポジトリの直下（`.git` のあるフォルダ）で、末尾に `\` を付けない
  - 頭の `OT_` は、Open棟梁 の関連の変数の印である（`OT_RESOURCE_ROOT` と同じ頭。環境変数の一覧で並ぶ）
  - **2026/11/09 に、頭の `OT_` の無い名前（`OTR_PATH` など）から変えた**（#652）。古い名前は読まない。古い名前だけが登録されていれば、`Resolve-RepoPath` が警告を出す。
    `Register-RepoPaths.ps1 -Apply` で登録し直し、表示される古い名前を消す。手元用の `RepoPaths.csv` も、`Variable` の列を直す
- **docker compose（`${...}`）からも使える**よう、変数名は英数字と `_` だけにする
- **共有のリソース（`C:\root\files`）は対象外**（開発者向けのリポジトリでは、昔からの慣習で固定のパス）

```powershell
# 使う側（例：Sample4SpRp\Stub\StubCompose.ps1）
. (Join-Path <root\files\tools\PowerShell> 'RepoPath.ps1')
$MpasRoot = Resolve-RepoPath -Variable OT_MPAS_PATH -Argument $MpasRoot -ArgumentName '-MpasRoot'
if (-not $MpasRoot) { throw '...' }
```
