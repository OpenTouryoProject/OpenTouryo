# MVC_Sample を Docker で動かす

`Samples4NetCore/Backend/MVC_Sample`（net10.0）を Linux コンテナで動かす（#548）。

方式は [ASPNETMVCOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/ASPNETMVCOnDocker) に倣い、**ホストビルド型**を採っている。

## 前提

| | |
|---|---|
| Docker エンジン | Docker Desktop / Rancher Desktop（Linux コンテナ） |
| .NET 10 SDK | `dotnet publish` と `dotnet dev-certs` に使う |
| **フレームワークのビルド** | `root/programs/1_BuildAll.ps1` を先に通しておく |
| **DB** | [LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker) を先に起動しておく |

DB は向こうが提供する。ネットワーク `common_link` に相乗りし、**サービス名 `sqlserver`** で届く。
Northwind は向こうの `sqlserver/init/start-up.sh` が自動で作る。

## 手順

```powershell
# 1. HTTPS の開発用証明書を作る（初回のみ）
powershell -NoProfile -ExecutionPolicy Bypass -File .\0_SetupCert.ps1

# 2. publish してコンテナを起動する
.\1_PublishAndUp.bat

# 3. 止める
.\2_Down.bat
```

| プロトコル | URL |
|---|---|
| HTTP | http://localhost:8080 （HTTPS へ 307 リダイレクト） |
| HTTPS | https://localhost:8081 |

ログイン画面でユーザ名を入れると入れる（**ユーザ名が空でなければ認証する**実装）。

### 毎回 publish とイメージ ビルドが走る

**`1_PublishAndUp.bat` は、起動のたびに `publish` を作り直す。**

```
if exist ".\publish" rmdir /s /q ".\publish"
dotnet publish ... -o ".\publish"
docker compose up --build -d
```

意図的である。**ソースとコンテナの内容が必ず一致する**ことを保証している。

実測でも重くない。

```
dotnet publish       7 秒ほど   （obj\ は消さないので増分ビルドが効く）
docker compose up    6 秒ほど   （ベース イメージと RUN 層はキャッシュが効く）
```

> **`rmdir` をやめてはいけない。**
> 出力先を消さずに publish すると、**ソースから消したファイルが残り続ける。**
> 古い DLL を抱えたまま動き、原因の分かりにくい不具合になる。

> **「起動だけ」の口を足すのも見送っている。**
> 古い `publish` のままコンテナが立ち上がると、
> **直したはずのものが直っていない**という形で時間を失う。
> 実際、`#578` では古いビルド成果物のまま測って原因を見誤った。

## ファイル構成

| | 役割 |
|---|---|
| `Dockerfile` | ホストビルド型。publish 成果物をランタイム イメージへ置く |
| `docker-compose.yml` | ポート・ボリューム・環境変数・ネットワーク |
| `appsettings.Container.json` | コンテナ用の設定（`appsettings.json` を**上書き**する） |
| `conf/SampleLogConf.xml` | コンテナ用の log4net 定義（出力先が違う） |
| `0_SetupCert.ps1` | 開発用証明書を **PEM** で書き出す（パスワード無し） |
| `1_PublishAndUp.bat` | `dotnet publish` → `docker compose up --build` |
| `2_Down.bat` | `docker compose down` |
| `publish/` `https/` | 生成物。**Git 管理対象外** |

---

## 設定をどこに書くか

読み込み順は次のとおりで、**後が勝つ**。

```
appsettings.json → appsettings.Container.json → 環境変数（compose の environment）
```

`appsettings.Container.json` は `ASPNETCORE_ENVIRONMENT=Container` のときだけ読まれ、
**書いたキーだけを上書きする**（丸ごと差し替えではない）。
書いていないキーは `appsettings.json` の値がそのまま生きる。

| 置き場所 | 入れるもの |
|---|---|
| `appsettings.Container.json` | 配置に依らない「コンテナならこう」（パス・HTTPS・Cookie・鍵） |
| compose の `environment` | 配置ごとに変わる値、秘密（リソースの場所・接続文字列・証明書） |

こうすると compose が短くなり、**なぜその値なのかをコメント付き JSON で説明できる。**

### `%OT_RESOURCE_ROOT%` はフレームワークが展開している

OS の機能ではない。`ResourceLoader.ResolveFilePath` が
`StringVariableOperator.BuiltStringIntoEnvironmentVariable` を呼び、
`%` で区切って `Environment.GetEnvironmentVariable(..., Process)` を引いている。
**Windows の書式に依存しない自前の実装なので、Linux コンテナでも効く。**

定義ファイル（`FxXML*Definition`）・log4net 定義・SQL ファイルは、いずれも
`ResolveFilePath` を通るため展開される。

なお、**この `OT_RESOURCE_ROOT` という命名と「絶対パスを環境変数へ張り替える」規約は、
[Open棟梁の CA スキル](https://github.com/OpenTouryoProject/OpenTouryoCodingAgentAssets)
に倣ったもの**である。あちらはセットアップ時に設定ファイルを書き換える側で、
**実行時に展開するのはフレームワーク側**。両者は別の役割なので、
展開の実装を探すときは `ResourceLoader` を見ること。

> **`FxContainerization` とは別の仕組みである。**
> あちらは ON のとき「接頭辞なしのキー名」で環境変数を読む。ここでは使っていない。

---

## 踏みやすいところ

### `WORKDIR` が要る

`appsettings*.json` は**コンテンツ ルート＝プロセスの作業ディレクトリ**から読まれる。
`WORKDIR /app` を置かないと、**設定ファイルが一切読まれないまま起動する**
（それでも画面は出るので気付きにくい）。

### フォルダ名の大文字小文字は合わせる

実フォルダは `Xml`（`XML` ではない）。**Windows は大文字小文字を区別しないので
間違っていても通るが、Linux では開けない。**

`appsettings.json` はもともと `resource/XML/...` と書いており、
このサンプルを作る過程で見つかった。**#550 で本体側を `Xml` に直してある。**
`resource` 配下の他のフォルダ（`Log` / `Sql` / `Test` / `X509`）も同様。

### `Data Source` は `localhost` ではなく `sqlserver`

コンテナから見た `localhost` は自分自身であり、DB には届かない。

### リソースは読み取り専用。ログは別の場所へ

`files/resource` は `:ro` でマウントしている。log4net の出力先をその中にすると書けない。
`OT_LOG_ROOT`（`/app/logs`）に向け、名前付きボリュームを当てている。

**log4net 定義の「中身」は Open棟梁が展開しない**（ログ ライブラリへそのまま渡す）ため、
中の変数は log4net の書式で書く必要がある。

```xml
<file type="log4net.Util.PatternString" value="%env{OT_LOG_ROOT}/ACCESS" />
```

`<param name="File">` ではなく、**型付きの `<file>`** である点に注意。

### 非 root で動く。書き込み先の所有者は `Dockerfile` が用意する（手動作業は無い）

`USER $APP_UID` で動かしているので、`/app/logs` と `/app/keys` は非 root で書ける必要がある。
これは**イメージのビルド時に自動で済む**。利用者が手で `chown` することは無い。

```dockerfile
RUN mkdir -p /app/logs /app/keys && chown -R $APP_UID /app/logs /app/keys
USER $APP_UID
```

**空の名前付きボリュームは、初回マウント時にイメージ側の中身・所有者・権限を引き継ぐ。**
上のとおり `USER` へ切り替える前（＝root のうち）に作って `chown` してあるので、
ボリュームも `app` 所有で作られる。実際、起動後はこうなる。

```
/app/logs:  drwxr-xr-x 2 app root
            -rw-r--r-- 1 app app  ACCESS.2026-08-15.log
/app/keys:  -rw------- 1 app app  key-....xml
```

ただし、この仕組みが効く条件が 2 つある。**変えるときは注意すること。**

| | |
|---|---|
| **名前付きボリュームであること** | **バインド マウント（ホストのパス）に変えると引き継がれない。** ホスト側の所有者がそのまま見えるため、Linux ホストでは書けなくなる（Docker Desktop / Rancher Desktop では緩く見えることが多く、**気付かないまま Linux で落ちる**） |
| **ボリュームが空であること** | 既に中身のあるボリュームには引き継がれない。`Dockerfile` の `chown` を後から直しても**既存のボリュームは直らない**ので、`docker compose down -v` で作り直す |

### データ保護の鍵を捨てない

`2_Down.bat` は `docker compose down`（`-v` なし）である。
鍵のボリュームを消すと、**認証 Cookie とセッションが失効する。**

---

## 証明書とパスワード

**PEM ＋ 秘密鍵にして、パスワードという管理対象を無くしてある。**

```
dotnet dev-certs https --format Pem -ep .\https\aspnetapp.pem -np
```

```yaml
- Kestrel__Certificates__Default__Path=/https/aspnetapp.pem
- Kestrel__Certificates__Default__KeyPath=/https/aspnetapp.key
```

参照元は PFX ＋ `.env` の `CERT_PASSWORD` だが、こちらは `.env` を作らない。
守る対象は **localhost 限定・自己署名・1 年で失効する開発用証明書**であり、
本番の TLS は前段のリバース プロキシが終端する想定だからである。
PFX にしても PFX とパスワードの両方がホストに置かれるので、実質の防御力は変わらない。

### 秘密を渡す口（本番向けの一例）

接続文字列のパスワードは、**本番でも実在する秘密**である。
ここではサンプルとして compose に直接書いているが、実際には次のような口がある。

```yaml
# Docker / Kubernetes の secrets（ファイルとしてマウントする）
secrets:
  - connectionStrings__ConnectionString_SQL
```

```csharp
// ファイル名がそのまま構成キーになる（__ がセクション区切り）
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
```

`Microsoft.Extensions.Configuration.KeyPerFile` が要る。
**キー名は環境変数のときと同じ**なので、置き換えても設定の書き方は変わらない。
Kubernetes の Secret もファイル マウントなので、そのまま移行できる。

どれを選ぶかは運用側の方式（Pod の信頼方式など）による。ここでは一例を示すに留める。

---

## 本番へ持って行くとき

**この compose は開発用である。本番向けは書き直す前提でよい。**
ただし実際に変わるのは、ほぼ compose 側だけである。

| | 本番で変わるか |
|---|---|
| `Dockerfile` | **ほぼそのまま** |
| 証明書のマウント・8081 の公開・`Kestrel__Certificates__*` | **消える**（TLS は前段で終端） |
| `UseHttpsRedirection` | `on` → **`off`**（前段で終端するなら、on にすると無限リダイレクト） |
| `CookieSecurePolicy` | **`always` のまま** |
| `UseForwardedHeaders` | `off` → **`on`**（前段の `X-Forwarded-Proto` を取り込む） |
| 接続文字列・リソースの場所 | 環境ごとに差し替え |

**前段で TLS を終端すると、アプリから見た接続は HTTP になる。**
`Request.IsHttps` が false のままなので、そのままでは
**フレームワーク側の `Secure` 自動判定（#536）が効かない。**

`UseForwardedHeaders=on` にすると `X-Forwarded-Proto` を取り込み、`IsHttps` が正しくなる（#549）。
**`ForwardedHeadersKnownProxies` も併せて確認すること。**
既定ではループバックからの転送しか信用しないため、
前段が別アドレス（コンテナや Kubernetes では通常そうなる）だと**黙って無視される。**

`CookieSecurePolicy=always` は、その保険として残しておく。
こちらは `IsHttps` を見ずに明示的に立てるため、転送ヘッダの設定を誤っても効く。

---

## 対象外

- **CI には載せていない。** 現行の疎通は `windows-latest` で回っており、Linux コンテナを動かせない
- **net48 側（`Samples/`）の Windows コンテナ対応**は行わない
- `3_SmokeTest.ps1` は従来どおり、コンテナを経由しない経路で `MVC_Sample (net10.0)` を確認する
