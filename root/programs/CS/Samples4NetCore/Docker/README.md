# Docker — Backend のサンプルを Linux コンテナで動かす

`Samples4NetCore/Backend/` のサンプルを Linux コンテナで動かす一式（#548、#582）。
**サンプルのコードは持たない。** `Backend/` を `dotnet publish` した成果物をイメージへ置く（ホストビルド型）。

上位: [`Samples4NetCore/ANALYSIS.md`](../ANALYSIS.md) / サンプル本体: [`Backend/ANALYSIS.md`](../Backend/ANALYSIS.md)

**対象ごとにフォルダが分かれ、それぞれの `README.md` が一次情報である。** 本書はその案内に留める。

| フォルダ | 対象 | ポート（ホスト側） | 一次情報 |
|---|---|---|---|
| [`MVC_Sample/`](MVC_Sample/README.md) | `Backend/MVC_Sample`（画面あり、Cookie 認証） | HTTP 8080 / HTTPS 8081 | **共通の方式・設定・証明書・本番化の一次情報** |
| [`ASPNETWebService/`](ASPNETWebService/README.md) | `Backend/ASPNETWebService`（ResourceServer、Bearer） | HTTP 8090 / HTTPS 8091（IDL は `/openapi/v1.json`） | `MVC_Sample` との**違いだけ** |

ポートをずらしてあるので、**2 つを同時に起動できる**（MVC から ResourceServer を呼ぶ構成を試せる）。

---

## 1. 前提（両方に共通）

| | |
|---|---|
| Docker エンジン | Docker Desktop / Rancher Desktop（Linux コンテナ） |
| .NET 10 SDK | `dotnet publish` と `dotnet dev-certs` |
| フレームワークのビルド | `root/programs/1_BuildAll.ps1` を先に通す |
| DB | [LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker) を先に起動する。ネットワーク `common_link` に相乗りし、**サービス名 `sqlserver`** で届く |

---

## 2. 手順（両方に共通の形）

| スクリプト | 何をするか |
|---|---|
| `0_SetupCert.ps1` | 開発用の証明書を **PEM**（パスワード無し）で書き出す（初回のみ） |
| `1_PublishAndUp.bat` | **毎回** `publish/` を消して `dotnet publish` → `docker compose up --build -d` |
| `2_Down.bat` | `docker compose down`（**`-v` なし**。データ保護の鍵とログのボリュームを残す） |
| `3_Test.ps1`（`ASPNETWebService` のみ） | 画面が無いので、これがクライアント。リダイレクト・IDL・Web API・DB までの到達を確かめる |

`publish/` と `https/` は生成物で、Git の管理対象外。

---

## 3. どこに何が書いてあるか

| 知りたいこと | 一次情報 |
|---|---|
| なぜ毎回 publish し直すのか（`rmdir` をやめてはいけない理由） | `MVC_Sample/README.md`「毎回 publish とイメージ ビルドが走る」 |
| ファイル構成（`Dockerfile` / `docker-compose.yml` / `appsettings.Container.json` / `conf/`） | `MVC_Sample/README.md`「ファイル構成」 |
| 設定の読み込み順（`appsettings.json` → `appsettings.Container.json` → 環境変数）と、置き場所の分担 | `MVC_Sample/README.md`「設定をどこに書くか」 |
| `%OT_RESOURCE_ROOT%` を展開しているのは誰か | `MVC_Sample/README.md`（`ResourceLoader.ResolveFilePath`。仕組みは [`CONFIGURATION.md`](../../../CONFIGURATION.md) 5 節） |
| 踏みやすいところ（`WORKDIR`、`Xml` の大文字小文字、`Data Source=sqlserver`、読み取り専用のリソース、非 root、鍵のボリューム） | `MVC_Sample/README.md`「踏みやすいところ」 |
| 証明書とパスワード、秘密を渡す口 | `MVC_Sample/README.md`「証明書とパスワード」 |
| 本番へ持って行くときに変わるもの（`UseHttpsRedirection` / `UseForwardedHeaders` など） | `MVC_Sample/README.md`「本番へ持って行くとき」 |
| Cookie を使わない分の違い、リダイレクト先のポート（`ASPNETCORE_HTTPS_PORT=8091`） | `ASPNETWebService/README.md` |
| `UseHttpsRedirection` などの切り替えキーそのものと、実測した落とし穴 | [`Backend/ANALYSIS.md`](../Backend/ANALYSIS.md) 4 節 |

---

## 4. エージェントが気を付けること

- **コンテナを勝手に起動・停止しない**（`AGENTS.md`）。DB のコンテナ（LocalServicesOnDocker）も同じ。必要なら施主に確かめる
- **CI と疎通テストには載っていない。** CI は `windows-latest` で Linux コンテナを動かせず、
  `3_SmokeTest.ps1` はコンテナを経由しない経路で `Backend/` を確かめる。**ここを変えても自動では何も確かめられない**
- **`Backend/` の設定キーを足したら、`appsettings.Container.json` と `docker-compose.yml` も見る。**
  コンテナでは `appsettings.Container.json` が書いたキーだけを上書きする（書いていないキーは `appsettings.json` の値が生きる）
- **バインド マウントに変えない・`down -v` を常用しない。** 前者は Linux ホストで書けなくなり、後者は鍵を捨てて認証 Cookie とセッションを失効させる
  （`MVC_Sample/README.md`「非 root で動く」「データ保護の鍵を捨てない」）
- **net48 版（`Samples/`）は対象外**（クラシック ASP.NET は Linux コンテナで動かない）
