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

### （1）リリース・エンジニアリングの手順を実施する

[`RELEASE.md`](../../RELEASE.md) に従い、検証 3 本を通しておく。

### （2）ビルドを行う

```
root\programs\CS\0_Release4Nuget.bat
```

**`DebugType` を手で書き換える必要は無い**（#531）。
このバッチが `DEBUG_TYPE=portable` を指定し、`z_Common.bat` は
**呼び出し側が設定済みならその値を尊重する。**

### （3）パッケージングを行う

```
root\programs\CS\NuGet\_NuGetPack.bat
```

このバッチが次を行う。

- `Build_*` から `in\` へ dll / pdb / xml を複製する
- バージョンを `Directory.Build.props` から読む
- **コミット ハッシュを git から読み、nuspec の `<repository>` へ渡す**
- `.nupkg` と `.snupkg` を `out\sp` に出す

**プッシュの前に 2 節の確認を行うこと。**

### （4）NuGet にプッシュする

API キーを NuGet サイトから取得し、次の bat 内に設定してから実行する。

```
root\programs\CS\NuGet\out\sp\_NuGetPush.bat
```

**最新では `sp` の方だけでよい。** `pp` の方はシンボルを登録しないケースで利用可。

### （5）後始末として revert する

- `_NuGetPush.bat` の ApiKey

> `z_Common.bat` の `DEBUG_TYPE` は、**もう書き換えないので revert 不要**（#531）。

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

**2 節の確認 4 点を、本番と同じように行う。**
`T_` 系は本番用と別のファイルなので、**本番側に入れた改善が
入っていないことがある**（#531 では `<repository>` と `-Properties` が
`T_` 系にだけ無かった）。

#### （4）プッシュする

API キーを設定してから実行する。

```
root\programs\CS\NuGet\out\sp\_T_NuGetPush.bat
```

**プッシュ対象はワイルドカード（`Erutcurtsarfni.Oyruot.Public.*.nupkg`）である。**
`out\sp` に古い版が残っていると**それも公開される**ので、
実行前にフォルダの中身を見ること。

#### （5）後始末として revert する

- `_T_NuGetPush.bat` の ApiKey

#### （6）確認する

3 節の設定を行った上で、テスト プロジェクトから
`Erutcurtsarfni.Oyruot.Public` を参照し、**F11 でステップイン**できることを見る。

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
| `0_Release4Nuget.bat` | `DEBUG_TYPE=portable` |
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
