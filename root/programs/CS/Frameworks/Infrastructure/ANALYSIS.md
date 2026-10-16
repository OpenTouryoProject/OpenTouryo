# ANALYSIS.md — フレームワーク本体（CS/Frameworks/Infrastructure）

対象: `root/programs/CS/Frameworks/Infrastructure` / 最終更新: 2026-10-16

上位: [`Frameworks/ANALYSIS.md`](../ANALYSIS.md)（全体の俯瞰・ビルド・条件コンパイル・横断の落とし穴）

本書は Infrastructure 配下の**アセンブリの分け方・参照の仕方・ソリューションの構成**を扱う。
各アセンブリの中身は、下の表の文書が詳細を持つ。

---

## 1. これは何か

Open棟梁 の本体。**NuGet で配布する部品**と、**利用者がソースを取り込んで改造するテンプレート**の 2 種類がある。

| 種類 | アセンブリ | 配布 | 改造 |
|---|---|---|---|
| **部品** | `Public` / `Public.Security` / `Dam*` / `Framework` / `Framework.RichClient` | NuGet（`Touryo.Infrastructure.*`） | しない（バージョンで追随する） |
| **テンプレート** | `Business` / `Business.RichClient` / `CustomControl` / `CustomControl.RichClient` | しない（ソースのまま） | **利用者が改造する前提** |
| ホスト | `ServiceInterface/`（ASP.NET WebAPI / WCF-TCP） | しない | 配備先に合わせて手を入れる |

**この区別は、変更の影響を読むときの基本になる。**
部品の公開 API を変えると、ビルド済みの利用者が壊れる。
テンプレートは利用者の手元で改造されているので、変えても利用者には届かない（次に取り込むまで）。

---

## 2. 下位の文書

| ディレクトリ | 文書 | 扱うこと |
|---|---|---|
| `Public/` | [`Public/ANALYSIS.md`](Public/ANALYSIS.md) | 汎用ライブラリ（Db / Dto / Str / IO / Log / Util ほか）と `Dam*` |
| `Public/Security/` | [`Public/Security/ANALYSIS.md`](Public/Security/ANALYSIS.md) | 暗号・署名・JWT・鍵交換・パスワード ハッシュ |
| `Framework/` | [`Framework/ANALYSIS.md`](Framework/ANALYSIS.md) | P/B/D の親クラス、通信制御、例外、認証（`Framework.RichClient` を含む） |
| `Business/` | [`Business/ANALYSIS.md`](Business/ANALYSIS.md) | 業務フレームワーク（サーバ側のテンプレート） |
| `Business/RichClient/` | [`Business/RichClient/ANALYSIS.md`](Business/RichClient/ANALYSIS.md) | 同上（WinForms / WPF） |
| `CustomControl/` | [`CustomControl/ANALYSIS.md`](CustomControl/ANALYSIS.md) | ASP.NET Web Forms のカスタム コントロール |
| `CustomControl/RichClient/` | [`CustomControl/RichClient/ANALYSIS.md`](CustomControl/RichClient/ANALYSIS.md) | WinForms のカスタム コントロール |
| `ServiceInterface/` | 本書 7 節 | サービスを公開するホスト（net48 のみ） |

---

## 3. アセンブリと依存の向き

```
段 1  Public
段 2  Public.Security / Dam* / CustomControl（Web）
段 3  Framework
段 4  Framework.RichClient / Business
段 5  CustomControl.RichClient / Business.RichClient
```

各段は、自分より上の段だけを参照する。正確な対応は次の表（`Dam*` を参照するのは `Business` 系で、`Framework` は参照しない）。

| アセンブリ | 直接参照するもの（`OpenTouryo.*`） |
|---|---|
| `Public` | なし |
| `Public.Security` | `Public` |
| `DamManagedOdp` / `DamMySQL` / `DamPstGrS` | `Public` |
| `Framework` | `Public` / `Public.Security` |
| `Framework.RichClient` | `Public` / `Framework` |
| `Business` | `Public` / `Public.Security` / `Framework` / `Dam*` |
| `Business.RichClient` | 上記 ＋ `Business` / `Framework.RichClient` |
| `CustomControl` | `Public` |
| `CustomControl.RichClient` | `Public` / `Framework` / `Framework.RichClient` |

**依存は上から下への一方向である。** `Public` に `Framework` の型を持ち込まない
（`Public` は単体で使える汎用ライブラリとして配布している）。

**`Business` は `Dam*` を全部参照する**（`UOC_ConnectionOpen` で DBMS を選ぶため）。
使わない DBMS のドライバも、利用者の配備に入る。

---

## 4. ソリューションと参照の方式

### 4.1 ソリューション

| sln | 中身 | ビルドする bat |
|---|---|---|
| `Nuget_net48.sln` | `Public` / `Public.Security` / `Framework` / `DamMySQL` / `DamManagedOdp` | `2_Build_NuGet_net48.bat` |
| `Nuget_netcore100.sln` | 上記 ＋ `DamPstGrS` | `2_Build_NuGet_netcore100.bat` |
| `Nuget_RichClient_*.sln` | `Framework.RichClient` | 同上 |
| `Business_net48.sln` | `Business` / `CustomControl` | `3_Build_Business_net48.bat` |
| `Business_netcore100.sln` | `Business` | `3_Build_Business_netcore100.bat` |
| `BusinessRichClient_*.sln` | `Business.RichClient` / `CustomControl.RichClient` | `3_Build_BusinessRichClient_*.bat` |
| `ServiceInterface/*/*.sln` | `ASPNETWebService` / `WCFService` | `7_Build_Framework_WS.bat`（net48 のみ） |

**`Nuget_*.sln` は NuGet で配布する範囲**と一致する（`root/programs/CS/NuGet/*.nuspec` の 7 パッケージ）。

### 4.2 参照の方式が 2 通りある

| 方式 | 使っているもの | 意味 |
|---|---|---|
| `ProjectReference` | `Nuget_*.sln` の中（`Framework` → `Public` など） | 同じ sln の中で依存順にビルドされる |
| `HintPath`（`Build_*` の DLL） | `Business` 系・`CustomControl` 系・`Framework.RichClient`（net48）・`ServiceInterface` | **先に出力された DLL を拾うだけ。** 無ければ失敗し、古ければ古いまま使う |

**`Business` 系は `Nuget_*.sln` の出力を `HintPath` で拾う。**
このため bat の番号順（`2_` → `3_`）が、そのままビルド順の制約になる。
`Business` だけを直してビルドしても、`Framework` 側の変更は入らない。

| 拾う場所 | 拾うもの |
|---|---|
| `Build_net48\` | net48 の `Business` 系・`CustomControl` 系 |
| `Build\` | `ServiceInterface`・`Tools`・`Tests`・net48 のサンプル（`4_Build_CopyAssemblies.bat` が `Build_net48\` を複写する） |
| `Build_netcore100\net10.0\` | net10.0 のすべて |
| `Build_netcore100\net10.0-windows7.0\` | `Framework.RichClient` と、それを参照するもの |

---

## 5. TFM と出力先

| 系統 | csproj | TFM | 出力先 |
|---|---|---|---|
| .NET Framework | `*_net48.csproj`（旧形式） | `v4.8` | `Build_net48\` |
| .NET | `*_netcore100.csproj`（SDK 形式） | `net10.0` | `Build_netcore100\net10.0\` |
| .NET（リッチ クライアント） | 同上（`UseWindowsForms` / `UseWPF`） | `net10.0-windows7.0` | `Build_netcore100\net10.0-windows7.0\` |

**net10.0 版が無いもの** : `CustomControl`（Web Forms）と `ServiceInterface`（WCF / ASP.NET WebAPI のホスト）。
**net48 版が無いもの** : `DamPstGrS`（net48 では PostgreSQL を使えない。`Business` の `"NPS"` も `#if NET48` で外してある）。

SDK 形式はワイルドカードでファイルを拾うため、**net10.0 で使えないファイルは `Compile Remove` で外している。**
旧形式は `<Compile Include>` の列挙なので、**csproj に書き足さないと net48 には入らない。**
新しいファイルを足すときは、両方の csproj を見ること（各下位文書の「TFM 差」の節に、外しているファイルを挙げてある）。

### 5.1 バージョン

**`Directory.Build.props` の `OpenTouryoVersion` は、`OpenTouryoVersion` を参照する SDK 形式の 7 プロジェクトにだけ効く**
（`Public` / `Public.Security` / `Framework` / `Framework.RichClient` / `DamManagedOdp` / `DamMySQL` / `DamPstGrS`）。
net48 は各 `Properties\AssemblyInfo.cs` が持ち、`Business` 系は `1.0.0.0` で別系統。
仕組みと理由は [`Frameworks/ANALYSIS.md`](../ANALYSIS.md) 7.2 節。

---

## 6. ビルド対象外のファイル（触っても何も起きない）

**どの sln・csproj にも入っていないファイルが残っている。** 直しても、ビルドにもテストにも現れない。

| ファイル | 状態 |
|---|---|
| `Public/Db/DamDB2/` / `DamHiRDB/` / `DamSqlDbWithMultiShard/` | csproj は `v4.6` で、存在しない `Public.csproj` を参照している。どの sln にも無い。`Business` の `"DB2"` / `"HIR"` もコメントアウトされている |
| `Public/Db/DamOraClient.cs` | net48 の列挙に無く、net10.0 は `Compile Remove`。`System.Data.OracleClient`（廃止済み）用 |
| `Public/IO/ZipBase.cs` / `Zipper.cs` / `UnZipper.cs` | 旧 ZIP 部品（DotNetZip）。現役は `ZipperV2` / `UnZipperV2`（[`Frameworks/ANALYSIS.md`](../ANALYSIS.md) 12 節 13） |

**`grep` で見つけた箇所が、これらの中でないかを確かめてから直すこと。**

---

## 7. ServiceInterface（サービスを公開するホスト、net48 のみ）

`CallController` のリモート プロトコルを受ける側である。**net10.0 版は無い**
（`7_Build_Framework_WSCore.bat` は空の手順として残してある。`1_BuildAll.ps1` の段を「失敗」に見せないため）。

| プロジェクト | 種類 | 受けるプロトコル | 参照 |
|---|---|---|---|
| `ASPNETWebService` | ASP.NET WebAPI（Web アプリ） | `AspNetWebAPI`（`FxController` の `WebAPIControllerForFx`） | `Build\` の `Public` / `Public.Security` / `Framework` |
| `WCFService` | コンソール EXE（自己ホスト） | `WCF_TCPIP`（`WCFTCPSvcForFx`） | `Build\` の `Public` / `Framework` / `Business` |

- 受けた要求は、`InProcessNameService` で `serviceName` を assembly / class に解決し、`DoBusinessLogic` を呼ぶ。
  **呼び出し側と同じ `TMInProcessDefinition.xml` の考え方**で、こちら側にも定義が要る
- 引数と戻り値は `BinarySerialize`（net48 専用。`Public/IO`）でバイト列にして受け渡す。
  **net10.0 にリモート プロトコルが無いのは、これを廃止したためである**
- 疎通テストが使う Web サービスは、こちらではなく `Samples/WS_sample/ASPNETWebService`（net48）と
  `Samples4NetCore/Backend/ASPNETWebService`（net10.0）である（[`SMOKETEST.md`](../../../SMOKETEST.md)）

---

## 8. VB 版

`root/programs/VB/Frameworks/Infrastructure/` に、**テンプレートとホストの VB.NET 版**がある
（`Business` / `CustomControl` / `ServiceInterface`。net48 のみ）。
**部品（`Public` / `Framework` など）は C# 版の DLL を共有する**ので、VB 版は無い。

**テンプレートの仕様を変えたら、VB 版の追随が要るかを報告すること。**
ビルドと疎通は `-Lang VB` で回す（[`BUILDING.md`](../../../BUILDING.md) 10 節）。

---

## 9. 落とし穴

1. **`HintPath` 参照は、古い DLL でも黙ってビルドが通る**（4.2 節）。`Framework` を直したら、`2_` から回す。
2. **新しいファイルは、net48 では列挙に足さないと入らず、net10.0 では外さないと入る**（5 節）。
3. **ビルド対象外のファイルを直しても何も起きない**（6 節）。
4. **`Business` は `Dam*` を全部参照する**ため、`Dam*` を足す・外すと `Business` の csproj も変わる（3 節）。
5. `CustomControl_net48.csproj` の `<Reference Include="OpenTouryo.Public, Version=2.4.0.0, ...">` の版は古いが、
   `<SpecificVersion>False</SpecificVersion>` と `HintPath` で解決するので実害は無い。

---

## 10. 作業チェックリスト

- [ ] 触るのは**部品かテンプレートか**（1 節）。部品なら公開 API の互換を、テンプレートなら VB 版を気にする
- [ ] 下位の `ANALYSIS.md`（2 節）の「落とし穴」と「チェックリスト」を読む
- [ ] 新しいファイルは net48 の `<Compile Include>` と net10.0 の `Compile Remove` の両方を確かめる（5 節）
- [ ] 直す場所がビルド対象外でないか（6 節）
- [ ] ビルドは `2_` → `3_` →（net48 なら）`4_Build_CopyAssemblies` の順（4.2 節）
- [ ] テンプレートを変えたら、VB 版の追随の要否を報告する（8 節）
