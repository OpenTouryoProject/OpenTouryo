# ClaudeSpace PowerShell ツール

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
