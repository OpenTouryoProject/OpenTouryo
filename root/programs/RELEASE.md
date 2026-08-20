# RELEASE.md — リリース チェックリスト

対象: `root/programs/CS`（C# 側）
配置: `root/programs`
本書は、リリース時に何を・どの順で確認し、**どこまでを機械が行い、どこからを人が行うか**を 1 枚にまとめたもの。

> **一次情報は本書ではない。** 迷ったら次を見ること。
>
> | 内容 | 一次情報 |
> |---|---|
> | ビルド構成・バージョン管理 | [`CS/Frameworks/ANALYSIS.md`](CS/Frameworks/ANALYSIS.md) 7 章 |
> | 全ビルドの実行と判定 | [`BUILDING.md`](BUILDING.md) |
> | 単体テストの実行と判定 | [`TESTING.md`](TESTING.md) |
> | サンプルの疎通確認 | [`SMOKETEST.md`](SMOKETEST.md) |
> | NuGet パッケージ化・公開の手順 | [`CS/NuGet/README.md`](CS/NuGet/README.md) |

---

## 1. 全体の流れ

| # | フェーズ | 手段 | 実施者 |
|---|---|---|---|
| 0 | 準備 | 手作業 | 人（エージェントは充足状況の確認・報告まで） |
| 1 | 検証（ビルド・単体テスト・疎通） | `1_BuildAll.ps1` / `2_RunAllTests.ps1` / `3_SmokeTest.ps1` | **エージェント可** |
| 2 | 検証（UI 系・ツール） | 手作業（GUI 操作） | 人（自動化は見送り。7 節） |
| 3 | パッケージ化 | `CS\0_Release4Nuget.bat` → `_NuGetPack.bat` | エージェント可（**指示があれば**） |
| 4 | 公開 | `_NuGetPush.bat` ＋ Wiki 手順 | **人のみ** |
| 5 | 後始末 | 手作業 | 人（エージェントは差分の報告まで） |

### エージェントが実行してよい範囲

**取り消しにくい・外部に出る・システム設定を変えるものは人が行う。**

この線引きは `AGENTS.md`（git 操作をしない／公開リポジトリへの投稿は承認を得てから）と同じ考え方。

| 人が行うフェーズ | 人が行う理由 |
|---|---|
| 0 準備 | `Start-Service` `aspnet_state` は**システム設定の変更**。DB の初期化も同様。<br>エージェントは不足を検知して**対処方法とともに報告する**に留める |
| 2 検証（UI 系） | GUI 操作のため |
| 4 公開 | `_NuGetPush.bat` は**外部公開で取り消しが困難**。かつ API キーを扱う |
| 5 後始末 | revert の確定はワーキング ツリーの検収と同じ扱い |

フェーズ 1 をエージェントが実行してよいのは、**失敗しても被害が無く、結果がワーキング ツリーに残るだけ**だから。

フェーズ 3 は成果物をローカルに作るだけだが、`CS\z_Common.bat` の `DEBUG_TYPE` 変更を伴い、
フェーズ 4 と地続きのため、**指示があったときだけ**実行する。

**フェーズ 1 は 3 本を順に実行するだけで済む。**

```powershell
cd root\programs
.\1_BuildAll.ps1                 # 全ビルド
```

```powershell
.\2_RunAllTests.ps1              # 単体テスト
```

```powershell
.\3_SmokeTest.ps1                # サンプルの疎通
```

クリーン ビルドから通しで **約 9.5 分**。
`2_RunAllTests.ps1` と `3_SmokeTest.ps1` は終了コードだけで合否が分かるが、
`1_BuildAll.ps1` は既知の署名エラーで 1 になるため、内容の確認が要る（3 節）。

---

## 2. フェーズ 0 : 準備

### 環境

- [ ] **Visual Studio を閉じた**
      … `1_DeleteDir.bat` が `.vs` を削除するため
- [ ] **SQL Server の Northwind に接続できる**
- [ ] **Northwind が初期状態**（`Shippers` 3 件 / `Orders` 830 件）  
      … 汚れている場合の戻し方は [`TESTING.md`](TESTING.md) 「テスト データの戻し方」  
      … `Orders2`（Northwind 標準ではない）は `3_SmokeTest.ps1` が無ければ作るため、事前準備は不要
- [ ] **サービスが開始されている**（`Start-Service`、`aspnet_state`、要管理者権限）
  - Start-Service … アプリが 使うデータストア（Dockerコンテナ）を起動、初期化する。
  - aspnet_state … net48 の Web アプリが使うASP.NET 状態サービスを起動する。
- [ ] **IIS Express がインストールされている**

### バージョン番号

- [ ] **`0_SetVersion.ps1` でバージョンを更新した**

      ```powershell
      cd root\programs
      .\0_SetVersion.ps1 -Version 3.3.0-alpha1 -WhatIf   # 変更内容の確認
      .\0_SetVersion.ps1 -Version 3.3.0-alpha1           # 実行
      ```

      … **バージョンの定義箇所は 2 系統に分かれており、手作業では追随を忘れやすい**ため、
      一括で更新する（#531）

      | 更新先 | 書き込む値 |
      |---|---|
      | `CS/Frameworks/Infrastructure/Directory.Build.props` の `OpenTouryoVersion` | `3.3.0-alpha1`（指定値そのまま） |
      | net48 6 本の `Properties\AssemblyInfo.cs` の `AssemblyVersion` | `3.3.0.0`（サフィックスを落とし 4 桁目に `0`） |

      … net48 の対象は**パッケージに入る 6 本**。
      `DamPstGrS` は net48 が無く、`Business` は非パッケージかつ別系統のため対象外

      ```
      Public / Public.Security / Framework / Framework.RichClient
      Public\Db\DamManagedOdp / Public\Db\DamMySQL
      ```

      … **該当行が見つからなければ NG で停止する**（黙って素通りさせない）。
      同じ版で再実行しても「変更なし」になる
- [ ] **`Business` 系は 1.0.0 のまま**であることを確認した
      … 意図的に別系統。`0_SetVersion.ps1` は触らない
- [ ] **nuspec の `<dependencies>` が csproj の `PackageReference` と一致している**
      … 依存を増減したら nuspec 側も合わせる。相互依存の版は `$version$` で自動追随

> **プレリリース版は、サフィックス付きで指定する**（`3.3.0-alpha1`）。
> サフィックスは `OpenTouryoVersion` にだけ入り、**アセンブリの版には入らない。**
> SDK 形式 csproj の `<Version>` も、`AssemblyVersion` / `FileVersion` には
> `VersionPrefix`（`3.3.0`）を、`InformationalVersion` には全体を割り当てる。
> その挙動に合わせてある。
>
> [`CS/NuGet/README.md`](CS/NuGet/README.md) 1 節（0）の
> 「α・β版などを使用して 2 回以上繰り返す」は、次のように回す。
>
> ```powershell
> .\0_SetVersion.ps1 -Version 3.3.0-alpha1   # プレ公開（develop 段階）
> .\0_SetVersion.ps1 -Version 3.3.0          # 本番（master マージ＋タグ後）
> ```

> **`OpenTouryoVersion` はアセンブリに焼き込まれる。**
> `*_netcore100.csproj` が `<Version>$(OpenTouryoVersion)</Version>` で参照するため、
> **書き換えたら必ずリビルド（フェーズ 3 の `0_Release4Nuget.bat`）が要る。**
> 先にパッケージ化すると、古いアセンブリに新しい版番号が付く。

> **`_NuGetPack.bat` が、net48 の `AssemblyVersion` と `OpenTouryoVersion` の
> 一致を検査して、ずれていれば停止する**（#531）。
> 追随を忘れたまま公開すると、**同じパッケージの net48 と net10.0 で
> アセンブリの版が食い違い、公開後には直せない。**
>
> ```
>   NG  Public : 3.0.0  expected 3.1.0
> [ERROR] The net48 AssemblyVersion does not match 3.1.0
> ```
>
> **`0_SetVersion.ps1` を使っていれば、この検査に引っかかることはない。**
> 手で書き換えた場合や、片方だけ revert した場合の保険である。

> **`_NuGetPack.bat` は、詰めたアセンブリの版も検査する**（#531）。
> 上の検査は**ソースどうしを突き合わせているだけ**なので、
> `0_SetVersion.ps1` の後に `0_Release4Nuget.bat` を飛ばしても通ってしまう。
> `in\` へ複製した後の**実際の DLL** を見て、ずれていれば停止する。
>
> ```
>   NG  in\net48\OpenTouryo.Public.dll : 3.3.0.0  expected 3.4.0.x
> [ERROR] The packaged assemblies do not carry 3.4.0
>         so the rebuild was skipped. Run CS\0_Release4Nuget.bat,
> ```
>
> **`_T_NuGetPack.bat`（テスト用）には、この検査は無い。**
> テスト用パッケージは版を**引数で受け取り**、`OpenTouryoVersion` を読まない。
> 版はパッケージに名前を付けるだけで、**アセンブリには書き込まれない**ため、
> 食い違いようがない。

> **補足 : パッケージの版とアセンブリの版は、一致しなくてよい**（NuGet の制約ではなく慣習）。
> テスト公開した `Erutcurtsarfni.Oyruot.Public 3.3.0-alpha2` は、
> パッケージが `3.3.0-alpha2`、中の DLL が `3.0.0.0` で、正常に動作した。
>
> **検査が見ているのは、パッケージの版とアセンブリの版の一致ではなく、
> 同じパッケージに入る net48 と net10.0 のアセンブリどうしが揃っているか**である。

> `Directory.Build.props` の XML コメントに `--`（ハイフン 2 個）を書くと
> MSBuild がプロジェクトの読み込みに失敗する。区切り線に使わないこと。

---

## 3. フェーズ 1 : 検証（自動）

| スクリプト | 見るもの | 期待値 | 文書 |
|---|---|---|---|
| `1_BuildAll.ps1` | ビルドが通るか | エラー 0 件 | [`BUILDING.md`](BUILDING.md) |
| `2_RunAllTests.ps1` | 出力が前回と同じか | HEAD の `Result*.txt` | [`TESTING.md`](TESTING.md) |
| `3_SmokeTest.ps1` | 起動して想定どおり動くか | 定義側の判定条件 | [`SMOKETEST.md`](SMOKETEST.md) |

- [ ] **`1_BuildAll.ps1` のエラーが「既知の 1 件」だけである**
      … `-SkipClean` は**使わない**。前回の成果物が残っていると通ったように見える
- [ ] **`2_RunAllTests.ps1` が終了コード 0**（8 ケース）
- [ ] **`3_SmokeTest.ps1` が終了コード 0**（25 件）

> **`1_BuildAll.ps1` は現状ここで終了コード 1 になる。**
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
>
> `-IgnoreErrors "MSB3482"` を付けると判定から外れて 0 になる（除外内容は一覧に残る）。
> **リリース判定では付けないこと。** 目視で 1 件だけであることを確かめるのが本筋で、
> この引数は人が見ない CI（[`BUILDING.md`](BUILDING.md) 9 節）のためのもの。

所要時間の実測（クリーン ビルドから通しで **約 9.5 分**）。

| スクリプト | 所要 |
|---|---|
| `1_BuildAll.ps1`（31 ステップ） | 5.8 分 |
| `2_RunAllTests.ps1`（8 ケース） | 1.3 分 |
| `3_SmokeTest.ps1`（25 件） | 7.4 分 |

### VB 側は、この 3 本に含めない

上の表はすべて C# 側（`-Lang` の既定）である。VB 側は `-Lang VB` で別に回す。

```powershell
.\0_RunAll.ps1 -Lang VB     # 1 と 3 だけを VB で通す（2 は対象外）
```

- **NuGet パッケージは C# 側から作る。** VB 側はリリース成果物ではない
- VB にテスト プロジェクトは無く、単体テストは C# の `Frameworks\Tests` に集約されている
- 所要は全ビルド 4.3 分 ＋ 疎通 1.5 分（6 件）

**リリースのたびに回す必要は無いが、VB 側に手を入れたリリースでは通すこと。**
詳細は [`BUILDING.md`](BUILDING.md) 10 節と [`SMOKETEST.md`](SMOKETEST.md) 10 節。

### 順序を守る

**`1_BuildAll.ps1` → `2_RunAllTests.ps1` → `3_SmokeTest.ps1` の順で行う。**
`1_BuildAll.ps1` はクリーンを行い、`4_Build_CopyAssemblies.bat` がテストとサンプルの
参照先（`Build_net48` / `Build_netcore100`）を更新するため、
逆順では古いアセンブリを見ることになる。

### NG が出たときの切り分け

| スクリプト | NG の意味 |
|---|---|
| `1_BuildAll.ps1` | コンパイル エラー、または restore の失敗 |
| `2_RunAllTests.ps1` | 退行／期待結果の陳腐化／**テスト データの汚染**のいずれか |
| `3_SmokeTest.ps1` | 起動時の失敗（構成・ネイティブ DLL・前提サービス）が多い |

`2_RunAllTests.ps1` の「実測のみ／期待のみ」に**件数の差**が出た場合は、
まずテスト データの汚染を疑う（`TESTING.md` 5 節）。

### `Result*.txt` の扱い

`2_RunAllTests.ps1` はワーキング ツリーの `Result*.txt` を書き換える（従来のバッチ運用と同じ）。

**この生 diff を目視してはいけない。** 実行日時が全行に入るため、
内容が同じでも**ほぼ全行が差分になる**。実測では 6 ファイルで約 2,458 行。

```diff
-[2025/11/18 15:19:08,286],[INFO ],[1],,,,----->>,...
+[2026/08/01 22:40:19,772],[INFO ],[1],,,,----->>,...
```

**判定は `2_RunAllTests.ps1` の「正規化後の差分」で行う。** それが 0 なら内容は同じ。
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

> **WS クライアントの呼び先は、本リポジトリにある。**
> `TMProtocolDefinition.xml` で生きているのは次の 3 つで、
> **ASMX（`protocol="2"`）と WCF-HTTP（`protocol="3"`）はコメントアウト済み**（呼ばれない）。
>
> | 定義 | 接続先 | 用意するもの |
> |---|---|---|
> | `testInProcess`（1） | インプロセス | **不要** |
> | `testWebService3`（4） | `net.tcp://localhost:7777/WCFService/WCFTCPSvcForFx/` | `ServiceInterface\WCFService`（自己ホストの exe）を起動 |
> | `testWebService4`（5） | `https://localhost/WebAPIControllerForFx` | `ServiceInterface\ASPNETWebService` を `https://localhost/` で公開 |
>
> **どちらも `1_BuildAll.ps1` の `Framework_WS` 段が建てている**（`7_Build_Framework_WS.bat`）。
> `WCFService\App.config` の待ち受けは、クライアントの定義と一致している。
>
> **要るのは別リポジトリではなく、上記のホスティングである。**
> `3_SmokeTest.ps1` が WS クライアントを対象外にしているのは、
> **WinForms / WPF で UI Automation が要るため**であって、接続先が無いからではない
> （プロトコル自体は `TestTransmission` が自前のスタブで確認している。#546 / #561）。

### フレームワーク付属ツール

| ツール | 確認内容 |
|---|---|
| `DaoGen_Tool`（墨壺） | **GUI 起動のみ**。CUI（`/HELP` `/CUI /MODE ...`）は `3_SmokeTest.ps1` が網羅済み |
| `DPQuery_Tool` | GUI 起動 |
| `EncAndDecUtil` | GUI 起動（CUI 版は `2_RunAllTests.ps1` が網羅済み） |
| `DeployZipPackWithHTTP` | GUI 起動 ＋ **圧縮・解凍**（ZIP 部品の唯一の利用者。#528） |

- [ ] `DaoGen_Tool` が GUI で起動し、D 層定義・SQL が生成できる
      … 生成ロジック自体は CUI 側で自動確認済み。ここで見るのは **GUI が動くこと**
- [ ] `DPQuery_Tool` が GUI で起動する
- [ ] `EncAndDecUtil` が GUI で起動する
- [ ] `DeployZipPackWithHTTP` が GUI で起動し、**圧縮と解凍ができる**
      … `ZipperV2` / `UnZipperV2` を使う唯一の利用者。ZIP 部品を変えたら必ず見ること。
      設定と履歴は `current.json` / `histories.json`（旧 `.bin` は読まない）

### 既知の環境依存（NG でも可）

- [ ] `WSClientWinCone_sample` の ClickOnce 署名エラー（`MSB3482`）を確認した
      … 拇印で証明書ストアを検索するため、当該証明書が無い環境ではビルドできない。
      **環境依存でありコード側の不具合ではない**（[`BUILDING.md`](BUILDING.md) 4 節）

---

## 5. フェーズ 3・4 : パッケージ化と公開

手順の一次情報は [`CS/NuGet/README.md`](CS/NuGet/README.md)。
**ここに書き写すと二重管理になるため、要点と抜けやすい点だけを挙げる。**

- [ ] **正式版は、`develop` → `master` をマージ（`--no-ff`）し、タグを打った後に詰めた**
      … パッケージは**詰めた時のコミットに永久に固定される**。
      同じバージョンは一度しか公開できないため、develop 段階で出すなら
      **プレリリース版**（`3.3.0-alpha1` など）にする（`README.md` 1 節（0）・7 節）
      … **`master` は PR 経由でしか入らない。** ブランチ保護で
      **レビュー 1 名 ＋ CI（`build`）の成功**が必須。**CI は約 14 分かかる**
      （ローカルの検証 3 本に加え、CI 側は DB の導入と初期化が入るため）
- [ ] **そのコミットを push 済みである**
      … 未 push だと Source Link が 404 になる。**公開後には直せない**
- [ ] `CS\0_Release4Nuget.bat` を実行した
      … `1_DeleteDir` → `2_Build_NuGet_net48` → `1_DeleteDir` →
      `2_Build_NuGet_netcore100` → `4_Build_CopyAssemblies` のみ。サンプルはビルドしない
      … `DEBUG_TYPE` は**このバッチが指定する。手で書き換えない**（#531）
- [ ] `CS\NuGet\_NuGetPack.bat` でパッケージ化した
      … **ビルドとパッケージ化は同じコミットで行う。**
      PDB のコミットは**ビルド時**、nuspec のコミットは**パッケージ化時**に決まるため、
      間にコミットすると食い違う（`README.md` 2 節 確認 4）
      … `in\` と `out\` は**このバッチが先に消す**ので、手で片付けなくてよい
      （`README.md` 6 節）
- [ ] `README.md` 2 節の**確認 5 点**を通した
- [ ] `set NUGET_API_KEY=＜キー＞` の上で `CS\NuGet\out\sp\_NuGetPush.bat` を実行し、push した
      … 最新は `sp`（シンボル付き）のみでよい。**キーを bat に直書きしない**（#531）
- [ ] Wiki の手順（NuGet 利用リポジトリの参照貼り直し）を実施した

---

## 6. フェーズ 5 : 後始末

**revert する項目は無くなった**（#531）。忘れやすい作業を、そもそも作らない形に変えた。

| 以前 revert していたもの | 現在 |
|---|---|
| `CS\z_Common.bat` の `DEBUG_TYPE` | `0_Release4Nuget.bat` が指定するので**書き換えない** |
| `_NuGetPush.bat` の API キー | **環境変数 `NUGET_API_KEY` で渡す**ので、コンソールを閉じれば消える |

- [ ] **API キーを `Revoke` した**（`Delete` はしない）
      … `Revoke` は即座に無効化するが、**行は `Revoked` として残り、
      次回は `Regenerate` で再び使える**。スコープ（グロブ）の設定を作り直さずに済む
      … `Delete` は**キーの定義ごと消える**ため、その系統をもう使わないときだけ
      … 有効期限を最短にしてあるなら、失効に任せてもよい（`README.md` 8 節）
- [ ] `%AppData%\NuGet\NuGet.Config` の `<apikeys>` に**キーが残っていない**
      … 過去に `nuget.exe SetApiKey` を使っていた場合、そこに永続化されている。
      削除オプションが無いため、該当する `<add>` 行を手で削除する（`README.md` 8 節）
- [ ] `git status` に意図しない変更が残っていない
      … 特に `Result*.txt`（`2_RunAllTests.ps1` が再生成する）と
      `CS\Frameworks\Tests\EncAndDecUtilCUI\*.cer` / `*.pfx`（Git 管理外の作業用コピー）

---

## 7. 自動化の範囲と、その理由

| 対象 | 自動化 | 理由 |
|---|---|---|
| 全ビルド（31 ステップ） | 済 | 既存バッチを呼ぶだけで済む |
| 単体テスト（8 ケース） | 済 | 期待結果ファイルが既にあり、正規化で機械比較できる |
| バッチ・CLI サンプル（9 件） | 済 | プロセス実行のみ。DB 疎通まで確認できる |
| `DaoGen_Tool` の CUI（6 件） | 済 | #508 で CUI 化。DB → 定義 CSV → Dao・SQL まで通せる |
| Web アプリ（3 件） | 済 | ログインまで通せば認証・セッションまで確認できる |
| **UI 系サンプル（18 本）** | **見送り** | UI Automation が必要。画面定義の変更で壊れやすく維持費が高い。<br>通す B 層／D 層は Web 系・バッチ系と重複し、回帰検出力の増分が小さい |
| **GUI ツール（4 本）** | **見送り** | 同上。`DaoGen_Tool` は生成ロジックを CUI 側で確認済みのため、<br>手作業で見るのは GUI が起動することだけでよい。<br>`DeployZipPackWithHTTP` は ZIP の圧縮・解凍まで見る（CUI が無いため） |
| Web サービス（WebAPI Client 2 件） | 済 | #566 で引き戻し、#571 で統合。**CRUD 一巡 ＋ DTO の往復 ＋ 楽観排他**を 1 本で見る |

**自動化した対象が「起動する」ことは、手作業側の確認範囲を狭める。**
`DaoGen_Tool` は CUI で生成ロジックまで確認できるようになったため、
GUI 側で見るのは画面が動くことだけになった。

---

## 8. エージェント向け作業チェックリスト

- [ ] `AGENTS.md` のポリシー遵守（**git 操作をしない**）
      … 検証で `Result*.txt` が書き換わるが、コミットの要否とタイミングは人が判断する
- [ ] 検証は **`1_BuildAll.ps1` → `2_RunAllTests.ps1` → `3_SmokeTest.ps1` の順**（3 節）
- [ ] `1_BuildAll.ps1` に `-SkipClean` を付けない（リリース判定では前回成果物を残さない）
- [ ] **NG を「既知」で片付けない。** 既知として扱ってよいのは
      `WSClientWinCone_sample` の署名エラーと NuGet 脆弱性警告のみ（4 節・[`BUILDING.md`](BUILDING.md) 3 節）
- [ ] `2_RunAllTests.ps1` の NG は、退行／期待結果の陳腐化／テスト データの汚染を切り分けてから報告
- [ ] **前提サービス・DB の状態を勝手に変えない。** 不足は対処方法とともに報告する
      （`3_SmokeTest.ps1` が `aspnet_state` を自動起動しないのと同じ理由）
- [ ] **フェーズ 1（検証）は自分で実行してよい。** フェーズ 0・2・4・5 は人が行う（1 節）
- [ ] **公開（`_NuGetPush.bat`）は実行しない。** 外部公開で取り消しが困難、かつ API キーを扱う。
      パッケージ化（フェーズ 3）は**指示があったときだけ**
- [ ] Issue のクローズ・ラベル変更は**人が行う**。エージェントは提案に留める
- [ ] 後始末（6 節）の revert 漏れが無いか `git status` で確認して報告
