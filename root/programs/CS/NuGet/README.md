# NuGet パッケージの作成と公開

パッケージの作成と公開の手順。
**シンボル サーバー（`.snupkg`）とソース サーバー（Source Link）を機能させる**ところまでを含む。

> **一次情報は本書ではない部分がある。**
>
> | 内容 | 一次情報 |
> |---|---|
> | リリース時の作業全体 | [`RELEASE.md`](../../RELEASE.md) |
> | ビルド構成・バージョン管理 | [`Frameworks/ANALYSIS.md`](../Frameworks/ANALYSIS.md) 7 章 |
> | リリース・エンジニアリング | [Open 棟梁 Wiki](https://opentouryo.osscons.jp/index.php?%E3%83%AA%E3%83%AA%E3%83%BC%E3%82%B9%E3%83%BB%E3%82%A8%E3%83%B3%E3%82%B8%E3%83%8B%E3%82%A2%E3%83%AA%E3%83%B3%E3%82%B0) |
> | パッケージ作成 | [Wiki : HowToCreateOpenTouryoNuGetPackages.ja](https://github.com/OpenTouryoProject/OpenTouryo/wiki/HowToCreateOpenTouryoNuGetPackages.ja) |
>
> 上 2 つの Wiki とは重複があるので、最終的には統合する。

---

## 1. 本番手順

**α・β版などを使用して、2 回以上繰り返す**（プレ、本番）。

### （0）どのコミットから詰めるかを決める

**`_NuGetPack.bat` は `git rev-parse HEAD` の値を nuspec と PDB に埋め込む。**
以後、そのパッケージは**そのコミットに固定される**（詳細は 7 節）。

したがって、**正式版は master へマージし、タグを打った後に詰める。**

```
1. develop → master へマージ（--no-ff）
2. タグ（03-30 など）をプッシュ
3. master（＝タグのコミット）で（2）以降を行う
```

こうすると**タグ・コミット・パッケージの 3 つが一致する。**

develop 段階で先に詰めると、次の 2 点で困る。

- **同じバージョンは一度しか公開できない。** nuget.org は再アップロードを受け付けず、
  取り消しも unlist（一覧から隠す）だけである。公開後にマージ時の修正が入っても、
  **公開済みの `3.3.0` は永久に古いまま**になる
- 公開後に develop へコミットが載ると、**タグとパッケージが別のコミットを指す。**
  `<repository commit>` は nuget.org 上に表示されるため、外から食い違いが分かる

**develop 段階で試したい場合はプレリリース版を使う**（`3.3.0-alpha1` など）。
それがこの節の冒頭「2 回以上繰り返す」の意味である。
**`3.3.0` という番号は、タグ付きコミットのために取っておく。**

### （1）リリース・エンジニアリングの手順を実施する

[`RELEASE.md`](../../RELEASE.md) に従い、検証 3 本を通しておく。

### （2）ビルドを行う

```
root\programs\CS\0_Release4Nuget.bat
```

**`DebugType` を手で書き換える必要は無い**（#531）。
このバッチが次の 2 つを指定し、`z_Common.bat` は
**呼び出し側が設定済みならその値を尊重する。**

| 変数 | 渡す値 | 既定（通常のビルド） |
|---|---|---|
| `DEBUG_TYPE` | `portable` | `full` |
| `CI_BUILD` | `true` → `/p:ContinuousIntegrationBuild=true` ＋ `/p:DeterministicSourcePaths=true` | `false` |

`ContinuousIntegrationBuild` は、**PDB に記録されるソースのパスを `/_/...` に正規化する。**

```
無効  C:\OpenTouryo\root\programs\CS\Frameworks\Infrastructure\Public\Db\DamODBC.cs
有効  /_/root/programs/CS/Frameworks/Infrastructure/Public/Db/DamODBC.cs
```

これが要る理由は 2 つある。

- **公開物に、ビルドしたマシンのローカル パスが埋まらない**
- **Visual Studio は「PDB のパスにファイルが在れば、それを開く」。**
  絶対パスのままだと、**ビルドしたマシンでは Source Link を通らない**ため、
  4 節の確認が「ローカルのソースが開いただけ」になり、検証にならない

> **`DeterministicSourcePaths` も渡す必要がある。**
> `ContinuousIntegrationBuild` から `DeterministicSourcePaths` への変換は
> `Microsoft.NET.Sdk` のターゲットが行うため、**旧形式 csproj には効かない。**
> 渡さないと **net48 側だけ絶対パスのまま残る**（Source Link の自動組み込みが
> 効かないのと同じ構図。5 節を参照）。

### （3）パッケージングを行う

```
root\programs\CS\NuGet\_NuGetPack.bat
```

このバッチが次を行う。

- バージョンを `Directory.Build.props` から読む
- **net48 の `AssemblyVersion` が、それと一致するかを検査する**（後述）
- `Build_*` から `in\` へ dll / pdb / xml を複製する
- **コミット ハッシュを git から読み、nuspec の `<repository>` へ渡す**
- `.nupkg` と `.snupkg` を `out\sp` に出す

#### net48 の版の検査

**`Directory.Build.props` は旧形式 csproj に効かない。**
net48 の版は各プロジェクトの `Properties\AssemblyInfo.cs` が持つ。

`OpenTouryoVersion` だけ上げて追随を忘れると、**同じパッケージの中で
net48 と net10.0 のアセンブリの版が食い違う。公開後には直せない。**

そこで、パッケージ化の前に 6 本を照合し、ずれていれば停止する。

```
  OK  Public : 3.0.0
  NG  Framework : 3.0.0  expected 3.1.0
[ERROR] The net48 AssemblyVersion does not match OpenTouryoVersion = 3.1.0
```

対象は**パッケージに入る 6 本**（`DamPstGrS` は net48 が無い。
`Business` は非パッケージかつ意図的に別系統の `1.0.0`）。

**`_T_NuGetPack.bat` にこの検査は無い。**
テスト用パッケージは版を**引数で受け取り**、`OpenTouryoVersion` を読まない。
版はパッケージに名前を付けるだけで**アセンブリには書き込まれない**ため、
食い違いようがない。

**プッシュの前に 2 節の確認を行うこと。**

### （4）NuGet にプッシュする

API キーを NuGet サイトから取得し、**環境変数で渡してから**実行する。

```
set NUGET_API_KEY=＜nuget.org で発行したキー＞
root\programs\CS\NuGet\out\sp\_NuGetPush.bat
```

**最新では `sp` の方だけでよい。** `pp` の方はシンボルを登録しないケースで利用可。

**キーを bat に直書きしてはならない**（#531）。理由は 8 節。

キーは**スコープ（対象パッケージ）と有効期限を絞り、使用後に nuget.org 側で削除**する。

> **Trusted Publishing は使えない。**
> nuget.org は API キーより Trusted Publishing を推奨しているが、
> これは **GitHub Actions からの公開にのみ対応**する仕組みである。
> 本手順は人がコマンドラインから実行するため、対象外（nuget.org の案内にも
> 「コマンドラインからの公開……APIキーは引き続き使用できます」とある）。
> Actions へ移すなら別途検討する。

### （5）後始末

**revert するものは無い**（#531）。

| | |
|---|---|
| API キー | **環境変数なので、コンソールを閉じれば消える** |
| `z_Common.bat` の `DEBUG_TYPE` | **もう書き換えないので不要** |

### （6）テスト プロジェクトで確認する

シンボル サーバー、ソース サーバーが機能するかどうかを確認する（4 節）。

---

## 2. 公開前の確認

**プッシュは取り消しが困難**なので、（3）の後・（4）の前に確認する。

### 確認 1 : `.nupkg` と `.snupkg` が対で出ていること

`out\sp` に、同じ名前・同じバージョンで両方があること。
**`.nupkg` に `.pdb` が入っていないこと**（`nuget.exe` が `.snupkg` 側へ振り分ける）。

### 確認 2 : PDB が portable であること

`.snupkg` 内の `.pdb` の**先頭 4 バイトが `BSJB`** であること。

```
BSJB  → portable PDB（正しい）
Micr  → Windows PDB（nuget.org は受け付けない）
```

`Micr` なら `DEBUG_TYPE` が `portable` になっていない。（2）をやり直す。

### 確認 3 : Source Link の情報が入っていること

`.pdb` の中に次の形の URL があること。

```
https://raw.githubusercontent.com/OpenTouryoProject/OpenTouryo/<commit>/*
```

**net48 と net10.0 の両方を見ること。**
net10.0 は .NET 8 以降の SDK が自動で入れるが、
**net48（旧形式 csproj）は `Microsoft.SourceLink.GitHub` を明示的に参照している。**
参照が外れると **net48 側だけ静かに機能しなくなる。**

```powershell
Add-Type -AssemblyName System.IO.Compression.FileSystem
$a = [IO.Compression.ZipFile]::OpenRead("out\sp\Touryo.Infrastructure.Public.3.0.0.snupkg")
foreach ($e in $a.Entries | Where-Object { $_.Name -like "*.pdb" }) {
    $tf = Join-Path $env:TEMP ($e.FullName -replace "/","_")
    [IO.Compression.ZipFileExtensions]::ExtractToFile($e, $tf, $true)
    $by = [IO.File]::ReadAllBytes($tf)
    $txt = [Text.Encoding]::ASCII.GetString($by)
    "{0,-46} {1,-9} SourceLink={2}" -f $e.FullName,
        $(if ([Text.Encoding]::ASCII.GetString($by[0..3]) -eq "BSJB") { "portable" } else { "Windows" }),
        $(if ($txt -match "raw\.githubusercontent") { "あり" } else { "なし" })
}
$a.Dispose()
```

### 確認 4 : `<repository>` にコミットが入っていること

`.nupkg` 内の `.nuspec` に次があること。

```xml
<repository type="git" url="https://github.com/OpenTouryoProject/OpenTouryo" commit="＜40 桁＞" />
```

空なら、git からハッシュを取得できていない。

> **Source Link 自体は PDB の情報で動く**ので、ここが空でもデバッグはできる。
> nuget.org 上で辿りやすくするための情報。

**確認 3 で見た PDB のコミットと、一致していること。** 出どころが違う。

| | コミットが決まる時点 |
|---|---|
| nuspec の `<repository>` | **`_NuGetPack.bat` の実行時**（`-Properties commit=`） |
| **PDB の Source Link** | **`0_Release4Nuget.bat` の実行時**（SourceLink がコンパイル時に git を読む） |

**詰め直すだけでは PDB のコミットは変わらない。**
ビルド後にコミットしてから詰めると、nuspec だけが新しくなって食い違う。
**（2）から通しでやり直すこと。**

### 確認 5 : そのコミットが GitHub 上にあること

**Source Link はコミットを SHA で指すため、未プッシュのまま公開すると 404 になる。**
公開後には直せない（7 節）。

確認 3 で得た URL を実際に叩く。

```powershell
$c = "＜確認 3 で得た 40 桁＞"
$f = "root/programs/CS/Frameworks/Infrastructure/Public/Util/ArrayOperator.cs"
(Invoke-WebRequest "https://raw.githubusercontent.com/OpenTouryoProject/OpenTouryo/$c/$f" -Method Head).StatusCode
```

`200` であること。

**あわせて、ワーキング ツリーが綺麗であること**（`git status`）。
未コミットの変更があると、PDB のチェックサムと GitHub 上のソースが食い違い、
Visual Studio がソースを開かない。

> 追跡外のファイルは `EmbedUntrackedSources` により PDB へ埋め込まれるため影響しない。

---

## 3. 利用側（Visual Studio）の設定

**パッケージ側が正しくても、利用側の設定が要る。**

| 場所 | 設定 |
|---|---|
| 「ツール」→「オプション」→「デバッグ」→「全般」 | **「マイ コードのみを有効にする」のチェックを外す** |
| 同上 | **「Source Link サポートを有効にする」にチェックを入れる** |
| 「ツール」→「オプション」→「デバッグ」→「シンボル」 | **「NuGet.org シンボル サーバー」にチェックを入れる** |

これで、F11 でステップインすると該当コミットのソースが取得される。

---

## 4. テスト手順

- 本番手順のうち、（テストを意味する）**`T_` プレフィックス**のあるファイルを使用する
- 対象は、`Touryo.Infrastructure.Public`（`OpenTouryo.Public`）のみ
- **`Erutcurtsarfni.Oyruot.Public`** と言う偽名を使用したα版として登録
- これを使用して、シンボル サーバー、ソース サーバーが機能するかどうかを確認する

> シンボル サーバー、ソース サーバーが機能する手順を確立したら移行テストは不要。

**ローカル フィードでは代替できない。**
Source Link は確認できるが、**`.snupkg` は nuget.org が展開する**ため、
シンボル サーバーの確認にはならない。

### 手順

#### （1）ビルドを行う

```
root\programs\CS\0_Release4Nuget.bat
```

**本番手順と同じものを使う。** ここを省いてはならない。

`1_BuildAll.ps1` や Visual Studio で建てた直後の `Build_*` は
**`DebugType=full`（Windows PDB）**になっており、そのまま詰めると
確認 2 で失格になる。**成果物は共通の `Build_*` に出るため、後から上書きされる。**

#### （2）パッケージングを行う

**版を引数で渡す。**

```
root\programs\CS\NuGet\_T_NuGetPack.bat 3.3.0-alpha1
```

本番用（`Symbol_*.nuspec`）は `Directory.Build.props` の
`OpenTouryoVersion` を使うが、**テスト用は本番の版とは無関係に付ける**ため、
`T_Symbol_Public.nuspec` の `<version>` は `$version$` のままにして
バッチの引数から渡している。

**公開済みより大きい版でなければ nuget.org は受け付けない。**
公開済みの一覧は次で分かる。

```powershell
(Invoke-RestMethod "https://api.nuget.org/v3-flatcontainer/erutcurtsarfni.oyruot.public/index.json").versions
```

#### （3）公開前の確認を行う

**2 節の確認 5 点を、本番と同じように行う。**

`T_` 系は本番用と**別のファイル**であり、**ずれやすい。**
現在は揃っているが、本番側に手を入れたら `T_` 側も見ること。

| 本番 | テスト | 意図的な差 |
|---|---|---|
| `Symbol_Public.nuspec` | `T_Symbol_Public.nuspec` | `<id>` / `<title>` が偽名 |
| `_NuGetPack.bat` | `_T_NuGetPack.bat` | 版の出どころ（`Directory.Build.props` ／ **引数**） |
| `out\sp\_NuGetPush.bat` | `out\sp\_T_NuGetPush.bat` | push 対象のワイルドカード |

**これ以外は同じであるべき。**
`<repository>` の有無、`-Properties` で渡す `version` / `commit`、
API キーの渡し方（8 節）は、本番・テストで揃っている。

> #531 では、本番側にだけ `<repository>` と `-Properties` を入れて
> `T_` 系を取りこぼした。`Symbol_*.nuspec` という**ワイルドカードで一括置換したため、
> `T_` で始まるファイルが対象から漏れた**。
> テスト用パッケージを詰めて初めて分かる類の漏れなので、確認 5 点は省かない。

#### （4）プッシュする

本番と同じく、**環境変数で渡す**。

```
set NUGET_API_KEY=＜nuget.org で発行したキー＞
root\programs\CS\NuGet\out\sp\_T_NuGetPush.bat
```

テスト用のキーは **`Erutcurtsarfni.Oyruot.*` にスコープを絞る**とよい。

**プッシュ対象はワイルドカード（`Erutcurtsarfni.Oyruot.Public.*.nupkg`）である。**
`out\sp` に古い版が残っていると**それも公開される**ので、
実行前にフォルダの中身を見ること。

#### （5）後始末

**revert するものは無い**（本番手順の（5）と同じ）。

#### （6）確認する

3 節の設定を行った上で、テスト プロジェクトから
`Erutcurtsarfni.Oyruot.Public` を参照し、**F11 でステップイン**できることを見る。

### 作業ブランチ上で行ってよい

テストは作業ブランチ上で、プレリリース版（`3.3.0-alpha1` など）として行ってよい。
**master へのマージやタグは要らない**（本番手順の（0）とは異なる）。

ただし **7 節に従い、詰めた時のコミットを消さないこと。**
消すと、公開済みのテスト用パッケージの Source Link が壊れる。

> 壊してしまった場合は、生き残ったコミットから `3.3.0-alpha2` を詰め直せば復旧できる。
> **プレリリースなので番号を消費してよい。**

---

## 5. 仕組みの要点（#531）

### シンボル サーバー（`.snupkg`）

PDB を別パッケージにして公開する。利用者は明示的に取得しなくてよい。
**portable PDB でなければ受け付けられない。**

### ソース サーバー（Source Link）

PDB の中に「このソースはどの URL から取れるか」を書いておく仕組み。
ビルド時に git のリモート URL とコミットから生成される。

| | Source Link |
|---|---|
| **net10.0** | .NET 8 以降の SDK が**自動で組み込む** |
| **net48** | 旧形式 csproj は自動にならないため、**`Microsoft.SourceLink.GitHub` を `PackageReference` で入れている**（`PrivateAssets=all` なので、利用者の依存関係には現れない） |

### 押さえるべき設定

| ファイル | 設定 |
|---|---|
| `*_net48.csproj`（6 本） | `PublishRepositoryUrl` / `EmbedUntrackedSources` ＋ `Microsoft.SourceLink.GitHub` |
| `0_Release4Nuget.bat` | `DEBUG_TYPE=portable` ＋ `CI_BUILD=true` |
| `z_Common.bat` | `CI_BUILD` を `ContinuousIntegrationBuild` と **`DeterministicSourcePaths` の両方**へ渡す |
| `_NuGetPack.bat` | `version` と `commit` を nuspec へ渡す |
| `Symbol_*.nuspec` | `<repository ... commit="$commit$" />` |

net48 で Source Link を入れているのは次の 6 本
（`DamPstGrS` は net48 が無いため対象外）。

```
Public / Public.Security / Framework / Framework.RichClient
DamManagedOdp / DamMySQL
```

---

## 6. 生成物は追跡しない

`in\` と `out\` の中身は `_NuGetPack.bat` が毎回作り直すため、`.gitignore` で除外している。

**フォルダごとではなく、生成物の種類で除外している。**
各フォルダの説明用 `.txt` と `_NuGetPush.bat` は**追跡し続ける必要がある**ため。

---

## 7. 詰めたコミットを消してはならない

**公開したパッケージは、詰めた時のコミットに永久に固定される。**

`_NuGetPack.bat` / `_T_NuGetPack.bat` は `git rev-parse HEAD` の値を、
nuspec の `<repository commit>` と PDB の Source Link に埋め込む。
Source Link はソースを次の URL から取得する。

```
https://raw.githubusercontent.com/OpenTouryoProject/OpenTouryo/＜SHA＞/＜パス＞
```

### ブランチ名もタグも関係ない

**SHA 指定なので、どのブランチで詰めたかは問われない。**
そのコミットが**リモートの何らかの ref から到達可能でありさえすれば**引ける。
`develop` に入っていなくても、作業ブランチに残っていれば機能する。

**守るべきなのはブランチ名ではなく、コミットが GitHub 上に残り続けることである。**

### 消えるケース

| 操作 | 詰めたコミットは残るか |
|---|---|
| `--no-ff` マージ／通常のマージ／fast-forward | **残る** |
| **squash merge** | **消える**（新しい 1 コミットに潰れ、元は到達不能になる） |
| **rebase して push** | **消える**（SHA が変わる） |
| force-push でその履歴を巻き戻す | **消える** |
| そのコミットを含む唯一のブランチを削除 | **消える** |

> 到達不能になったコミットも GitHub 上でしばらく引ける場合があるが、
> **保証されないので当てにしない。**

**したがって、詰めたコミットは `--no-ff` で develop → master へ流して残す。**
master は履歴を書き換えないため、これが最も確実である。

### 「先頭コミット」ではない

守るべきは**詰めた時点の HEAD** であり、そのブランチの先端であり続ける必要はない。
後からコミットが載れば先端ではなくなるが、**祖先であれば到達可能なので問題ない。**

### 壊れた場合

**公開後には直せない。** 同じバージョンの再アップロードはできず、
取り消しも unlist（一覧から隠す）だけである。**次のバージョンを出すしかない。**

だからこそ、公開前に確認 5 を行う。

---

## 8. API キーは環境変数で渡す（#531）

`_NuGetPush.bat` / `_T_NuGetPush.bat` は `NUGET_API_KEY` から読む。
未設定なら**エラーで止まる**（キー無しで push を試みない）。

```
set NUGET_API_KEY=＜nuget.org で発行したキー＞
```

### `-ApiKey` と `-SymbolApiKey` の両方を渡す

**`nuget.exe` はシンボル サーバーには `-SymbolApiKey` を使う。`-ApiKey` は効かない。**

```
nuget.exe push xxx.nupkg -ApiKey %KEY% -SymbolApiKey %KEY% -source https://api.nuget.org/v3/index.json
```

`-SymbolApiKey` を省くと、**`.nupkg` は通るが `.snupkg` だけが 403 になる。**

```
Pushing Erutcurtsarfni.Oyruot.Public.3.3.0-alpha1.snupkg to 'https://www.nuget.org/api/v2/symbolpackage'...
  Forbidden https://www.nuget.org/api/v2/symbolpackage/
403 (The specified API key is invalid, has expired, or does not have permission
     to access the specified package.)
```

**メッセージはキーが無効であるかのように読めるが、実際には
「シンボル用のキーが渡されていない」**という意味である。

> 旧方式（`nuget.exe SetApiKey`）でこれが問題にならなかったのは、
> キーが `NuGet.Config` に保存され、**シンボル側も同じ保存値を拾っていた**ため。
> 環境変数方式に変えるなら、**両方のオプションを明示する必要がある。**

### なぜ `SetApiKey` をやめたか

以前は bat 内に `nuget.exe SetApiKey [ApiKey]` と書き、
実行前に実キーへ差し替え、実行後にプレースホルダへ戻す運用だった。
**戻し忘れが 2 か所で漏洩につながる。**

| | 問題 |
|---|---|
| **bat ファイル** | **Git で追跡されている。** キーを直書きしたままコミットすると、公開リポジトリに載る |
| **`NuGet.Config`** | `SetApiKey` はキーを `%AppData%\NuGet\NuGet.Config` の `<apikeys>` へ**永続化する**。bat を戻しても消えない |

環境変数なら**コンソールを閉じれば消える**ため、後始末が要らない。

> 過去に `SetApiKey` を実行していた場合、`%AppData%\NuGet\NuGet.Config` に
> キーが残っている。`nuget.exe 6.13` の `setApiKey` に削除オプションは無いので、
> **`<apikeys>` の該当する `<add>` 行を手で削除する。**

### キーの発行

nuget.org でキーを作る際は、次を絞る。

- **スコープ** … 対象パッケージを限定する（テスト用なら `Erutcurtsarfni.Oyruot.*`）
- **有効期限** … 最短にする
- **使用後** … nuget.org 側で削除する
