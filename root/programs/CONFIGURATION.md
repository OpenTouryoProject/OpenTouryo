# CONFIGURATION.md — 設定ファイルの扱い

対象: `root/programs` 配下（CS / VB、net48 / net10.0）
配置: `root/programs`

本書は**「設定がどう読まれ、どう上書きされ、どこで踏むか」**を扱う。
**個々のキーが何を意味するかは書かない。**それは値の隣（各設定ファイルのコメント）にある。

---

## 0. どこに何を書くか

設定ファイルは **86 個**ある。全部に同じ説明を書くことはできないし、書けば必ずずれる。

| | 書く場所 | 例 |
|---|---|---|
| **仕組み**（読み込み順・上書き・展開・落とし穴） | **本書だけ** | 「環境変数は `__` 区切りで上書きする」 |
| **そのキーが何か**（値の意味・既定値・選択肢） | **値の隣のコメント** | 「`FxSqlTraceLog` : Ｄ層のパフォーマンス ログ出力の on / off」 |
| **そのサンプル固有の事情** | そのサンプルの `ANALYSIS.md` / `README.md` | 「`Legacy/*` は相対パスで書く」 |

**設定ファイルに仕組みを書き足したくなったら、本書に書いて、そこから参照する。**
一部のファイルにだけ詳しい説明がある状態は、**書かれていないファイルを「該当しない」と
誤読させる。**（実際、86 個中 3 個にだけ詳細が書かれている状態になっていた）

---

## 1. 設定ファイルの種類

| | net48 | net10.0 |
|---|---|---|
| デスクトップ・バッチ・CLI | `app.config` | `appsettings.json` |
| Web | `Web.config`（＋ `app.config`） | `appsettings.json`（＋ `appsettings.{環境名}.json`） |

net48 の Web では、`Web.config` が `appSettings` を**別ファイルへ委譲**していることがある。

```xml
<appSettings file="app.config"/>
```

このとき**キーの実体は `app.config` 側**にある。`Web.config` だけ見て「無い」と判断しない。

### net10.0 では初期化が要る

```csharp
GetConfigParameter.InitConfiguration(configuration);   // Startup のコンストラクタ
```

**これが無いと `Fx*` 系の設定がすべて null になる。** net48 は暗黙に読まれるので不要。

---

## 2. セクション名の流儀

**`appSettings` / `connectionStrings` というセクション名を、JSON でもそのまま使う。**

```json
{
  "connectionStrings": { "ConnectionString_SQL": "..." },
  "appSettings":       { "FxXMLSPDefinition": "..." }
}
```

ASP.NET Core の標準（`ConnectionStrings` / 任意のセクション）とは綴りが違う。
`GetConfigParameter` がこの名前で引くため、**変えると読めなくなる。**

> **`appsettings.json` はコメント付き JSON（JSONC）で書いてよい。**
> `Microsoft.Extensions.Configuration.Json` は `//` を許容する。
> ただし**厳密な JSON パーサで読むと壊れる**ので、外部ツールに食わせるときは注意。

---

## 3. 読み込み順と優先順位（net10.0）

`Host.CreateDefaultBuilder` が次の順に読む。**後が勝つ。**

```
appsettings.json → appsettings.{環境名}.json → user-secrets（Development のみ）
  → 環境変数 → コマンドライン
```

`{環境名}` は `ASPNETCORE_ENVIRONMENT` の値。未指定なら `Production`。

**`appsettings.{環境名}.json` は「キー単位」の上書きであって、丸ごと差し替えではない。**
書いたキーだけが上書きされ、書いていないキーは `appsettings.json` の値が生きる（実測）。

| 実験 | 結果 |
|---|---|
| `appsettings.Container.json` 無し（Production） | 200 |
| 有り ＋ `ASPNETCORE_ENVIRONMENT=Container` | **500**（1 キーだけ壊した値を書いた。読まれている） |
| 同じファイル ＋ 環境名なし | 200（読まれない） |

> **環境名を変えると副作用がある。** `Development` にすると `env.IsDevelopment()` が真になり、
> 開発者例外ページが出る（本番向けの `UseExceptionHandler` / `UseHsts` 側に入らない）。
> 実例では `Container` という名前を使っている。

---

## 4. 環境変数で上書きする

### 4-1. `__`（下線 2 つ）区切り

```
appSettings__FxXMLSPDefinition=/app/files/resource/Xml/SPDefinition.xml
connectionStrings__ConnectionString_SQL=Data Source=db;...
```

`__` が `:`（セクション区切り）に読み替えられる。**上の 3 節のとおり、環境変数は
`appsettings.json` にも `appsettings.{環境名}.json` にも勝つ。**

`Startup` が受け取った `IConfiguration` をそのまま `InitConfiguration` へ渡しているため、
**フレームワーク側から読む値にも効く。**

> **Docker / Kubernetes の secrets も同じキー名で渡せる。**
> `Microsoft.Extensions.Configuration.KeyPerFile` を足し、
> `builder.Configuration.AddKeyPerFile("/run/secrets", optional: true)` とすると、
> **ファイル名がそのまま構成キーになる**（`__` がセクション区切りとして効く）。
> `Host.CreateDefaultBuilder` は既定では読まないので、1 行足す必要がある。

### 4-2. `FxContainerization` は別の仕組み

**混同しないこと。** こちらは ON のとき、**接頭辞なしのキー名**で環境変数を読む。

```csharp
// Public/Util/GetConfigParameter.cs : CheckContainerization
if (containerization.ToUpper() == PubLiteral.ON)
{
    return System.Environment.GetEnvironmentVariable(key);   // 例: FxXMLSPDefinition
}
```

| | 読むもの | 有効化 |
|---|---|---|
| `appSettings__` 方式 | `appSettings__FxXMLSPDefinition` | **不要**（既定で効く） |
| `FxContainerization` | `FxXMLSPDefinition` | `FxContainerization=ON` |

**net10.0 のコンテナで使っているのは前者。** `FxContainerization` は使っていない。

---

## 5. 設定値の中の `%変数%` はフレームワークが展開する

**OS の機能ではない。Open棟梁 が自前で展開している。**

```csharp
// Public/IO/ResourceLoader.cs : ResolveFilePath
loadfilepath = StringVariableOperator.BuiltStringIntoEnvironmentVariable(loadfilepath);
```

`%` で分割して `Environment.GetEnvironmentVariable(名前, Process)` を引くだけの実装で、
**Windows の書式に依存しない。Linux コンテナでも効く。**

```json
"FxXMLSPDefinition": "%OT_RESOURCE_ROOT%/Xml/SPDefinition.xml"
```

### 効く範囲

**`ResourceLoader` を経由してファイルを開くパスは、すべて展開される**
（`ResolveFilePath` / `Exists` / `LoadAsString` のいずれも `ResolveFilePath` を通る）。

- 定義 XML（`FxXML*Definition`）
- log4net / NLog の定義ファイルの**場所**（`FxLog4NetConfFile`）
- SQL ファイル（`SqlTextFilePath` ＋ ファイル名。`BaseDam.Load2` → `ResourceLoader.LoadAsString`）

`TransactionControl` / `BaseController` / `*NameService` は、`BuiltStringIntoEnvironmentVariable` を
明示的に呼んでいる。

> **`ResourceLoader` を経由しない読み方をしている箇所では効かない。**
> 新しいパス系キーを足すときは、どちらの経路かを確かめること。

### 定義ファイルの「中身」は展開しない

**Open棟梁 が展開するのは設定ファイルの「場所」までで、中身はライブラリへそのまま渡す。**
したがって、ログ定義の中の変数は**ログ ライブラリの書式**で書く。

```xml
<!-- log4net : PatternString の %env{}。<param name="File"> ではなく型付きの <file> -->
<file type="log4net.Util.PatternString" value="%env{OT_LOG_ROOT}/ACCESS" />
```

```xml
<!-- NLog : ${...} -->
<target xsi:type="File" name="ACCESS" fileName="${OT_LOG_ROOT}/ACCESS..." />
```

---

## 6. コンテンツ ルート（＝プロセスの作業ディレクトリ）

**`appsettings*.json` は、実行ファイルの場所ではなく「作業ディレクトリ」から読まれる。**

作業ディレクトリが違うと、**設定ファイルが一度も読まれないまま起動する。**
それでも画面は出るので気付きにくい。起動ログで確かめられる。

```
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\OpenTouryo\root\programs      ← 意図した場所か？
```

- コンテナでは `Dockerfile` に **`WORKDIR /app`** を置く
- `dotnet <パス>\App.dll` で起動するときは、**出力フォルダを作業ディレクトリにする**

---

## 7. パスの落とし穴

### 7-1. 大文字小文字（#550）

**実フォルダは `Xml` / `Log` / `Sql` / `Test` / `X509`。**
`XML` や `test` と書いても **Windows では通り、Linux では開けない。**

### 7-2. 絶対パス

サンプルは `C:/root/files/resource/...` を直書きしている（リポジトリを `C:\root\` に
展開する前提）。**別の場所に置くならビルドは通るが実行時に落ちる。**
環境変数（4 節）か `%変数%`（5 節）で差し替える。

### 7-3. 相対パスの基準

相対パスは**まず作業ディレクトリ基準**で探し、見つからなければ
**`AppContext.BaseDirectory`（実行ファイルの場所）基準**で再探索する
（`ResolveFilePath`）。CLI が任意の場所から起動される場合への対応。

`Samples4NetCore/Legacy/*` は、この仕組みに乗って**ファイル名だけ**を書いている。

```json
"FxXMLSPDefinition": "SPDefinition.xml"
```

csproj の `CopyToOutputDirectory` で出力フォルダへ配る前提なので、
**`%変数%` 化すると壊れる。**

---

## 8. HTTPS 関連 — 「検知」と「宣言」

リバース プロキシで TLS を終端すると、**アプリから見た接続は HTTP になる**。
`Request.IsHttps`（net48 は `IsSecureConnection`）は false のままで、
**ブラウザは HTTPS なのに Cookie に `Secure` が付かない**（#549）。

対処は 2 通りある。

| 方針 | 手段（net10.0） | 手段（net48） | 実測 |
|---|---|---|---|
| **検知**（実態を知らせる） | `UseForwardedHeaders` | IIS の URL Rewrite で `HTTPS` を立てる | 4 件中 **3 件**に `secure` |
| **宣言**（そう決め打つ） | `CookieSecurePolicy=always` | `forms` の `requireSSL="true"` | 4 件中 **4 件** |

**宣言の方が依存が少なく確実。** `CookiePolicy` はすべての `Set-Cookie` を後段で
上書きするため、個々の箇所が何を判断しているかに依存しない。

### 検知を選ぶなら 3 点が要る

| | 内容 | 欠けると |
|---|---|---|
| ① 前段が伝播する | `X-Forwarded-Proto: https` を付ける | 何も起きない |
| ② **アプリが受け取る設定になっている** | `UseForwardedHeaders` | **付いていても捨てられる** |
| ③ 付けられるのが前段だけ | ネットワークで閉じる／`KnownProxies` | クライアントが詐称できる |

> **`KnownIPNetworks` / `KnownProxies` を設定しないと黙って無視される。**
> 既定ではループバックからの転送しか信用しない。
> 「on にしたのに直らない」の原因はほぼこれ。

### 何を矯正でき、何ができないか

**分かれ目は「リクエストごとに変わるか」。**

| 情報 | 性質 | 内部パラメタで矯正できるか |
|---|---|---|
| スキーム（HTTPS か） | 環境ごとに固定 | **できる**（`CookieSecurePolicy` / `requireSSL`） |
| 配置パス（サブパス） | 環境ごとに固定 | **できる**（`app.UsePathBase`） |
| 外部から見た URL | 環境ごとに固定 | **できる**（設定に絶対 URL を書く） |
| **クライアント IP** | リクエストごとに変わる | **できない**（伝播が必須） |
| **クライアント証明書** | リクエストごとに変わる | **できない**（伝播が必須） |

**クライアント IP は既に動く。** `GetClientIpAddress` と `MyBaseAsyncApiController` が
`X-Forwarded-For` を**ヘッダから直読み**しており、`UseForwardedHeaders` に依存しない。

### 宣言のトレードオフ

**間違えるとはっきり壊れる。** 本当に平文 HTTP の環境で `always` にすると、
Antiforgery の Cookie が返らず**ログインの POST が 400** になる（実測）。

**ただしこれは利点でもある。**

| | 間違えたときの現れ方 |
|---|---|
| 検知 | **静かに効かない。** 画面は動き、`Secure` だけが付かない |
| 宣言 | **はっきり壊れる。** すぐ気付いて直せる |

---

## 9. 秘密の扱い

サンプルは**パスワードを直書きしている**（すぐ動かせることを優先しているため）。
実際の環境では、次のいずれかで渡す。

| | net48 | net10.0 |
|---|---|---|
| 別ファイルへ逃がす | `<connectionStrings configSource="..."/>` | `appsettings.{環境名}.json` |
| 暗号化する | `aspnet_regiis -pef connectionStrings <パス>` | — |
| 環境変数 | — | `connectionStrings__ConnectionString_SQL=...` |
| 開発機のみ | — | `dotnet user-secrets`（Development のみ読まれる） |
| コンテナ | — | secrets ＋ `AddKeyPerFile`（4-1 節） |
| そもそも持たない | `Integrated Security=SSPI` | 同左 |

**証明書は「パスワードを無くす」選択肢がある。** PFX ＋ パスワードではなく
PEM ＋ 秘密鍵ファイルにすると、管理対象が 1 つ減る。

```
dotnet dev-certs https --format Pem -ep ./https/aspnetapp.pem -np
```

---

## 10. net48 / net10.0 の対応表

| | net48 | net10.0 |
|---|---|---|
| ファイル | `app.config` / `Web.config` | `appsettings.json` |
| 環境別 | （なし。`configSource` で分ける） | `appsettings.{環境名}.json` |
| 初期化 | 暗黙 | **`InitConfiguration()` が必須** |
| 環境変数での上書き | （標準では無い） | `__` 区切り |
| Cookie の `Secure` を必ず立てる | `requireSSL="true"` | `CookieSecurePolicy=always` |
| 転送ヘッダの取り込み | IIS の URL Rewrite | `UseForwardedHeaders` |
| 鍵の共有（複数台） | `machineKey` | DataProtection（`DataProtectionKeyPath`） |
| セッションの外出し | `sessionState mode="StateServer"` ほか | `AddDistributedSqlServerCache` ほか |

---

## 11. 実例はどこにあるか

| 見たいもの | 場所 |
|---|---|
| net10.0 の Web（本番向けの切り替えを含む） | `CS/Samples4NetCore/Backend/MVC_Sample/MVC_Sample/appsettings.json` |
| 環境変数・`%変数%`・環境別ファイルの実運用 | `CS/Samples4NetCore/Docker/`（[`README.md`](CS/Samples4NetCore/Docker/README.md)） |
| net48 の Web（`machineKey` / 転送ヘッダの注記） | `CS/Samples/WebApp_sample/MVC_Sample/MVC_Sample/Web.config` |
| 相対パスで自己完結させる書き方 | `CS/Samples4NetCore/Legacy/Bat_sample/*/appsettings.json` |
| 領域ごとの事情 | 各 `ANALYSIS.md`（[Frameworks](CS/Frameworks/ANALYSIS.md) / [Samples](CS/Samples/ANALYSIS.md) / [Samples4NetCore](CS/Samples4NetCore/ANALYSIS.md)） |
