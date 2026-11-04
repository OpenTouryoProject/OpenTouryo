# ANALYSIS.md — CLI のサンプル（CS/Samples/CLI_sample）

対象: `root/programs/CS/Samples/CLI_sample`（net48） / 最終更新: 2026-11-05

上位: [`Samples/ANALYSIS.md`](../ANALYSIS.md)

---

## 1. これは何か

**net48 版にはコードが無い。** 3 つのフォルダに `README.md` が 1 つずつあるだけである。
**実装は .NET 10 版（`Samples4NetCore/Legacy/CLI_sample/`）にだけある。**

| フォルダ | 何のサンプルか | net48 版が無い理由（各 README） |
|---|---|---|
| `Simple_CLI` | `System.CommandLine` と `Sharprompt` を使った CLI のサンプル・雛形 | `Sharprompt` が .NET Framework のサポートを終えたため、ドロップした |
| `DAG_Login_CLI` | OAuth 2.0 Device Authorization Grant でログインする CLI | `System.CommandLine` の beta が解除され、Ctrl-C で `CancellationToken` がハングする問題が直ったら移植する |
| `LIR_Login_CLI` | OAuth 2.0 for Native Apps の Loopback Interface Redirection でログインする CLI | 同上 |

> **net48 で動く CLI の認証サンプルは、別の場所にある。**
> [`Sample4SpRp/DotNet/CLI/SpRp_CLI`](../../Sample4SpRp/DotNet/CLI/)（#649）が、Device Authorization Grant（`device`）と
> LIR（`pkce`。認可コード ＋ PKCE、ループバック）を **net48 と net10.0 の両方**で持つ。
> **外部のパッケージは `Newtonsoft.Json` だけ**で、ここで移植を止めている `System.CommandLine`（beta）・`Sharprompt` を使わない。
> 使い方は [`Sample4SpRp/README.md`](../../Sample4SpRp/README.md) の 4 節。ビルドは `11_Build_SpRp_sample.bat`（本書の `5_` ではない）。

---

## 2. ビルドと検証

- `5_Build_CLI_sample.bat` は**何もビルドしない**（メッセージを出して `pause` するだけ）。
  `1_BuildAll.ps1` の段として残してある
- 疎通テストの対象は .NET 10 版の `Simple_CLI` だけ（`Simple_CLI (net10.0)`。[`SMOKETEST.md`](../../../SMOKETEST.md)）

---

## 3. .NET 10 版の状態（参考）

.NET 10 版の 3 本は、いまも `System.CommandLine` **`2.0.0-beta1.21216.1`**（beta）と `Sharprompt` 3.0.0 を使っている。
net48 へ移植する条件（beta の解除）は、.NET 10 版の依存を上げることとも関係する。
詳細は [`Samples4NetCore/ANALYSIS.md`](../../Samples4NetCore/ANALYSIS.md)。

---

## 4. 落とし穴

1. **ここを探しても実装は無い**（1 節）。直すなら .NET 10 版。**net48 で動く認証の CLI が要るなら `SpRp_CLI`**（1 節の注記）。
2. `5_Build_CLI_sample.bat` を単独で実行すると `pause` で止まる（`0_ExecAllBat.bat` は `echo |` で流している）。

---

## 5. 作業チェックリスト

- [ ] 直す対象は .NET 10 版（`Samples4NetCore/Legacy/CLI_sample/`）ではないか
- [ ] net48 版を復活させるなら、`5_Build_CLI_sample.bat`・`st_Targets.ps1`・本書・各 README を合わせて直す
