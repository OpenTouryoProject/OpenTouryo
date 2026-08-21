# ASPNETWebService を Docker で動かす

`Samples4NetCore/Backend/ASPNETWebService`（ResourceServerTemplate、net10.0）を
Linux コンテナで動かす（#582）。

**構成は [`../MVC_Sample/README.md`](../MVC_Sample/README.md) と同じである。**
方式（ホストビルド型）・設定の置き方・証明書の扱い・本番へ持って行くときの注意は、
**あちらが一次情報**なので、ここには書かない。

ここに書くのは**この Resource Server だけの事情**である。

---

## 前提

`MVC_Sample` と同じ。DB は
[LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker)
が提供するものに相乗りするので、**先に起動しておくこと。**

## 手順

```powershell
# 1. HTTPS の開発用証明書を作る（初回のみ）
powershell -NoProfile -ExecutionPolicy Bypass -File .\0_SetupCert.ps1

# 2. publish してコンテナを起動する
.\1_PublishAndUp.bat

# 3. 止める
.\2_Down.bat
```

```
HTTP  : http://localhost:8090   （HTTPS へリダイレクト）
HTTPS : https://localhost:8091
IDL   : https://localhost:8091/openapi/v1.json
```

---

## `MVC_Sample` との違い

### ① ホスト側のポートが `8090` / `8091`

**`MVC_Sample`（8080 / 8081）と同時に起動できるようにしてある。**
Resource Server を MVC から呼ぶ構成を、そのまま試せる。

コンテナ内の待ち受けは両者とも `8080` / `8081` のままである
（`ASPNETCORE_HTTP_PORTS` / `ASPNETCORE_HTTPS_PORTS`）。
**変えているのは `docker-compose.yml` の `ports` の左側だけ。**

### ② Cookie を使わない

**OAuth2 の Resource Server であり、認証は `Authorization: Bearer` ヘッダである。**
Cookie 認証・セッションを持たないため、次は設定していない。

| | `MVC_Sample` | こちら |
|---|---|---|
| `CookieSecurePolicy` | `always` | **書かない**（効かない） |
| `DataProtectionKeyPath` | `/app/keys` | **書かない**（同上） |

`Startup.cs` 側もコメントアウトで残してある。
**消していないのは、Cookie 認証へ広げるときの手本になるためである。**

> **`/app/keys` のボリュームだけは用意してある。**
> 今は使わないが、Cookie 認証を足した時点で**必ず要る**
> （コンテナを作り直すたびに鍵が変わり、認証 Cookie が失効するため）。
> そのとき `docker-compose.yml` を書き換えずに済む。

### ③ 画面が無い

静的ファイル（`/wwwroot`）も Razor も持たない。
ブラウザで確認するなら **IDL（`/openapi/v1.json`）** を開く。

---

## 動作の確認

```powershell
# IDL が返るか（openapi の版と paths が入っていること）
curl -k https://localhost:8091/openapi/v1.json

# WebAPI が応答するか
curl -k -X POST https://localhost:8091/api/Json/test
```

**IDL は 200 が返るだけでは足りない。**
`openapi` の版と `paths` が入っていることまで見る
（疎通テストの判定も同じ。[`SMOKETEST.md`](../../../../SMOKETEST.md) 3 節）。

---

## 対象外

**`Samples/WS_sample/ASPNETWebService`（net48）は対象外。**
クラシック ASP.NET であり、Linux コンテナでは動かない。
