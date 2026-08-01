# RELEASE.md — リリース チェックリスト

対象: `root/programs/CS`
本書は、リリース時に何を・どの順で確認し、**どこまでを機械が行い、どこからを人が行うか**を
1 枚にまとめたもの（#513 段階 4）。

> **一次情報は本書ではない。** 迷ったら次を見ること。
>
> | 内容 | 一次情報 |
> |---|---|
> | リリース エンジニアリングの全体像 | [Open 棟梁 Wiki - リリース・エンジニアリング](https://opentouryo.osscons.jp/index.php?%E3%83%AA%E3%83%AA%E3%83%BC%E3%82%B9%E3%83%BB%E3%82%A8%E3%83%B3%E3%82%B8%E3%83%8B%E3%82%A2%E3%83%AA%E3%83%B3%E3%82%B0) |
> | NuGet パッケージ化・公開の手順 | [`NuGet/_手順の説明.txt`](NuGet/_手順の説明.txt) |
> | ビルド構成・バージョン管理 | [`Frameworks/ANALYSIS.md`](Frameworks/ANALYSIS.md) 7 章 |
> | 全ビルドの実行と判定 | [`BUILDING.md`](BUILDING.md) |
> | 単体テストの実行と判定 | [`Frameworks/Tests/TESTING.md`](Frameworks/Tests/TESTING.md) |
> | サンプルの疎通確認 | [`SMOKETEST.md`](SMOKETEST.md) |

---

## 1. 全体の流れ

| # | フェーズ | 手段 | 自動化 |
|---|---|---|---|
| 0 | 準備 | 人 | — |
| 1 | 検証（ビルド・単体テスト・疎通） | `BuildAll.ps1` / `RunAllTests.ps1` / `SmokeTest.ps1` | **済** |
| 2 | 検証（UI 系・ツール） | 人 | 見送り（7 節） |
| 3 | パッケージ化 | `0_Release4Nuget.bat` → `_NuGetPack.bat` | bat |
| 4 | 公開 | `_NuGetPush.bat` ＋ Wiki 手順 | 人 |
| 5 | 後始末 | 人 | — |

**フェーズ 1 は 3 本を順に実行するだけで済む。**

```powershell
cd root\programs\CS
.\BuildAll.ps1                 # 全ビルド
cd Frameworks\Tests
.\RunAllTests.ps1              # 単体テスト
cd ..\..
.\SmokeTest.ps1                # サンプルの疎通
```

クリーン ビルドから通しで **約 8.5 分**。
`RunAllTests.ps1` と `SmokeTest.ps1` は終了コードだけで合否が分かるが、
`BuildAll.ps1` は既知の署名エラーで 1 になるため、内容の確認が要る（3 節）。

---

## 2. フェーズ 0 : 準備

### 環境

- [ ] **Visual Studio を閉じた**
      … `1_DeleteDir.bat` が `.vs` を削除するため
- [ ] **SQL Server の Northwind に接続できる**
- [ ] **Northwind が初期状態**（`Shippers` 3 件 / `Orders` 830 件）
      … 汚れている場合の戻し方は [`TESTING.md`](Frameworks/Tests/TESTING.md) 「テスト データの戻し方」
- [ ] **`Orders2` テーブルが存在する**
      … Northwind 標準ではない。無ければ
      `Samples\Bat_sample\RerunnableBatch_sample\CREATE ORDERS2.sql` を実行
- [ ] **ASP.NET 状態サービスが開始されている**（`Start-Service aspnet_state`、要管理者権限）
      … net48 の Web アプリが `mode="StateServer"` を使うため
- [ ] **IIS Express がインストールされている**

### バージョン番号

- [ ] **`Infrastructure/Directory.Build.props` の `OpenTouryoVersion` を更新した**
      … SDK 形式アセンブリ 7 個と NuGet パッケージの唯一の定義箇所
- [ ] **net48（旧形式 csproj）の `Properties\AssemblyInfo.cs` を更新した**
      … `Directory.Build.props` が効かないため別管理
- [ ] **`Business` 系は 1.0.0 のまま**であることを確認した
      … Public / Framework / Public.Security の 3.0.0 とは意図的に別系統
- [ ] **nuspec の `<dependencies>` が csproj の `PackageReference` と一致している**
      … 依存を増減したら nuspec 側も合わせる。相互依存の版は `$version$` で自動追随

> `Directory.Build.props` の XML コメントに `--`（ハイフン 2 個）を書くと
> MSBuild がプロジェクトの読み込みに失敗する。区切り線に使わないこと。

---

## 3. フェーズ 1 : 検証（自動）

| スクリプト | 見るもの | 期待値 | 文書 |
|---|---|---|---|
| `BuildAll.ps1` | ビルドが通るか | エラー 0 件 | [`BUILDING.md`](BUILDING.md) |
| `RunAllTests.ps1` | 出力が前回と同じか | HEAD の `Result*.txt` | [`TESTING.md`](Frameworks/Tests/TESTING.md) |
| `SmokeTest.ps1` | 起動して想定どおり動くか | 定義側の判定条件 | [`SMOKETEST.md`](SMOKETEST.md) |

- [ ] **`BuildAll.ps1` のエラーが「既知の 1 件」だけである**
      … `-SkipClean` は**使わない**。前回の成果物が残っていると通ったように見える
- [ ] **`RunAllTests.ps1` が終了コード 0**（6 ケース）
- [ ] **`SmokeTest.ps1` が終了コード 0**（12 件）

> **`BuildAll.ps1` は現状ここで終了コード 1 になる。**
> `WSClnt_sample (net48)` の ClickOnce 署名エラー（`MSB3482`）が残るため。
> **終了コードだけで判断せず、エラー一覧が下記 1 件だけであることを確認すること**（4 節）。
>
> ```
> [WSClnt_sample (net48)] ... error MSB3482: 署名中にエラーが発生しました:
> ... WSClientWinCone_sample.exe の署名に失敗しました。
> SignTool Error: No certificates were found that met all the given criteria.
> ```
>
> 証明書をストアに入れた環境では 0 になる。

所要時間の実測（クリーン ビルドから通しで **約 8.5 分**）。

| スクリプト | 所要 |
|---|---|
| `BuildAll.ps1`（31 ステップ） | 5.8 分 |
| `RunAllTests.ps1`（6 ケース） | 1.3 分 |
| `SmokeTest.ps1`（12 件） | 1.4 分 |

### 順序を守る

**`BuildAll.ps1` → `RunAllTests.ps1` → `SmokeTest.ps1` の順で行う。**
`BuildAll.ps1` はクリーンを行い、`4_Build_CopyAssemblies.bat` がテストとサンプルの
参照先（`Build_net48` / `Build_netcore100`）を更新するため、
逆順では古いアセンブリを見ることになる。

### NG が出たときの切り分け

| スクリプト | NG の意味 |
|---|---|
| `BuildAll.ps1` | コンパイル エラー、または restore の失敗 |
| `RunAllTests.ps1` | 退行／期待結果の陳腐化／**テスト データの汚染**のいずれか |
| `SmokeTest.ps1` | 起動時の失敗（構成・ネイティブ DLL・前提サービス）が多い |

`RunAllTests.ps1` の「実測のみ／期待のみ」に**件数の差**が出た場合は、
まずテスト データの汚染を疑う（`TESTING.md` 5 節）。

### `Result*.txt` の扱い

`RunAllTests.ps1` はワーキング ツリーの `Result*.txt` を書き換える（従来のバッチ運用と同じ）。

**この生 diff を目視してはいけない。** 実行日時が全行に入るため、
内容が同じでも**ほぼ全行が差分になる**。実測では 6 ファイルで約 2,458 行。

```diff
-[2025/11/18 15:19:08,286],[INFO ],[1],,,,----->>,...
+[2026/08/01 22:40:19,772],[INFO ],[1],,,,----->>,...
```

**判定は `RunAllTests.ps1` の「正規化後の差分」で行う。** それが 0 なら内容は同じ。
生 diff を読むのは、正規化後に差分が出たときだけでよい。

- [ ] 正規化後の差分が 0 であることを確認した
- [ ] `Result*.txt` をコミットするかどうかを判断した
      … 内容が変わっていないなら**コミットしなくてよい**（日時だけの差分が積み上がるため）。
      仕様変更で内容が変わった場合は、新しい基準としてコミットする

---

## 4. フェーズ 2 : 検証（手作業）

自動化から外した対象。**起動して主要な画面が出ることを確認する。**

### UI 系サンプル（18 本）

| 区分 | net48 | net10.0 |
|---|---|---|
| 2 層 C/S | `2CSClientWin_sample`<br>`2CSClientWPF_sample`<br>`AsyncEvent_sample`<br>`CustCtrl_sample`<br>`GenDaoAndBatUpd_sample`<br>`TimeStamp_sample` | `2CSClientWin_sample`<br>`2CSClientWPF_sample`<br>`CustCtrl_sample`<br>`GenDaoAndBatUpd_sample`<br>`TimeStamp_sample` |
| WS クライアント | `WSClientWin_sample`<br>`WSClientWPF_sample`<br>`WSClientWin2_sample`<br>`WSClientWinCone_sample` | `WSClientWin_sample`<br>`WSClientWPF_sample`<br>`WSClientWin2_sample` |

- [ ] 2 層 C/S 系（net48 6 本 / net10.0 5 本）が起動し、CRUD 画面が操作できる
- [ ] WS クライアント系（net48 4 本 / net10.0 3 本）が起動する

> **WS クライアントの疎通には別リポジトリが要る。**
> 呼び先の Web サービスは
> [`OpenTouryoProject/ResourceServerTemplates`](https://github.com/OpenTouryoProject/ResourceServerTemplates)
> へ移設済みで、本リポジトリだけでは接続先が無い。

### フレームワーク付属ツール

| ツール | 確認内容 |
|---|---|
| `DaoGen_Tool`（墨壺） | GUI 起動 ＋ D 層自動生成。**CUI モードあり**（`/HELP` `/CUI /MODE ...`） |
| `DPQuery_Tool` | GUI 起動 |
| `EncAndDecUtil` | GUI 起動（CUI 版は `RunAllTests.ps1` が網羅済み） |

- [ ] `DaoGen_Tool` が GUI で起動し、D 層定義・SQL が生成できる
- [ ] `DPQuery_Tool` が GUI で起動する
- [ ] `EncAndDecUtil` が GUI で起動する

### 既知の環境依存（NG でも可）

- [ ] `WSClientWinCone_sample` の ClickOnce 署名エラー（`MSB3482`）を確認した
      … 拇印で証明書ストアを検索するため、当該証明書が無い環境ではビルドできない。
      **環境依存でありコード側の不具合ではない**（[`BUILDING.md`](BUILDING.md) 4 節）

---

## 5. フェーズ 3・4 : パッケージ化と公開

手順の一次情報は [`NuGet/_手順の説明.txt`](NuGet/_手順の説明.txt)。
**ここに書き写すと二重管理になるため、要点と抜けやすい点だけを挙げる。**

- [ ] `z_Common.bat` の `DEBUG_TYPE` を `full` → **`portable`** に変更した
- [ ] `0_Release4Nuget.bat` を実行した
      … `1_DeleteDir` → `2_Build_NuGet_net48` → `1_DeleteDir` →
      `2_Build_NuGet_netcore100` → `4_Build_CopyAssemblies` のみ。サンプルはビルドしない
- [ ] `NuGet\_NuGetPack.bat` でパッケージ化した
- [ ] `NuGet\out\sp\_NuGetPush.bat` に API キーを設定し、push した
      … 最新は `sp`（シンボル付き）のみでよい
- [ ] Wiki の手順（NuGet 利用リポジトリの参照貼り直し）を実施した

---

## 6. フェーズ 5 : 後始末

**revert を忘れやすい。** 特に API キーはリポジトリに残してはならない。

- [ ] `z_Common.bat` の `DEBUG_TYPE` を `full` に戻した
- [ ] `NuGet\out\sp\_NuGetPush.bat` を**プレースホルダに戻した**
      … コミットされている状態は `nuget.exe SetApiKey [ApiKey]`。実キーを残さない
- [ ] `git status` に意図しない変更が残っていない
      … 特に `Result*.txt`（`RunAllTests.ps1` が再生成する）と
      `Frameworks\Tests\EncAndDecUtilCUI\*.cer` / `*.pfx`（Git 管理外の作業用コピー）

---

## 7. 自動化の範囲と、その理由

| 対象 | 自動化 | 理由 |
|---|---|---|
| 全ビルド（31 ステップ） | 済 | 既存バッチを呼ぶだけで済む |
| 単体テスト（6 ケース） | 済 | 期待結果ファイルが既にあり、正規化で機械比較できる |
| バッチ・CLI サンプル（9 件） | 済 | プロセス実行のみ。DB 疎通まで確認できる |
| Web アプリ（3 件） | 済 | ログインまで通せば認証・セッションまで確認できる |
| **UI 系サンプル（18 本）** | **見送り** | UI Automation が必要。画面定義の変更で壊れやすく維持費が高い。<br>通す B 層／D 層は Web 系・バッチ系と重複し、回帰検出力の増分が小さい |
| **Web サービス** | **不可** | 本リポジトリにホストが無い（別リポジトリへ移設済み） |

### 今後の候補

- **`DaoGen_Tool` の CUI モード**（#508 で追加）
  `/HELP` と `/CUI /MODE DAODEFGEN` は非対話で実行できるため、
  `SmokeTest.ps1` の対象に加えられる。GUI 側の確認は手作業に残る。

---

## 8. エージェント向け作業チェックリスト

- [ ] `AGENTS.md` のポリシー遵守（**git 操作をしない**）
      … 検証で `Result*.txt` が書き換わるが、コミットの要否とタイミングは人が判断する
- [ ] 検証は **`BuildAll.ps1` → `RunAllTests.ps1` → `SmokeTest.ps1` の順**（3 節）
- [ ] `BuildAll.ps1` に `-SkipClean` を付けない（リリース判定では前回成果物を残さない）
- [ ] **NG を「既知」で片付けない。** 既知として扱ってよいのは
      `WSClientWinCone_sample` の署名エラーと NuGet 脆弱性警告のみ（4 節・[`BUILDING.md`](BUILDING.md) 3 節）
- [ ] `RunAllTests.ps1` の NG は、退行／期待結果の陳腐化／テスト データの汚染を切り分けてから報告
- [ ] **前提サービス・DB の状態を勝手に変えない。** 不足は対処方法とともに報告する
      （`SmokeTest.ps1` が `aspnet_state` を自動起動しないのと同じ理由）
- [ ] パッケージ化・公開（5 節）と Issue のクローズは**人が行う**。エージェントは提案に留める
- [ ] 後始末（6 節）の revert 漏れが無いか `git status` で確認して報告
