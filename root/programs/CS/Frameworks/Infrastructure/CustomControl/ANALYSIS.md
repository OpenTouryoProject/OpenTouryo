# ANALYSIS.md — カスタム コントロール（Web）（Infrastructure/CustomControl）

対象: `root/programs/CS/Frameworks/Infrastructure/CustomControl`（`RichClient/` を除く） / 最終更新: 2026-10-16

上位: [`Infrastructure/ANALYSIS.md`](../ANALYSIS.md) / 全体: [`Frameworks/ANALYSIS.md`](../../ANALYSIS.md)
下位: [`RichClient/ANALYSIS.md`](RichClient/ANALYSIS.md)（WinForms 版）

---

## 1. これは何か

`OpenTouryo.CustomControl`（名前空間 `Touryo.Infrastructure.CustomControl`）。
**ASP.NET Web Forms の標準コントロールを継承したカスタム コントロール**の**テンプレート**である。

- **net48 のみ**（Web Forms が .NET に無いため。`*_netcore100.csproj` は無い）
- 利用者がソースを取り込み、**自分の画面の共通仕様（入力チェック・描画）を足す場所**として用意してある。NuGet には無い
- 参照するのは `Public` だけ（`Build_net48\` の DLL を `HintPath`）。ビルドは `Business_net48.sln` に入っている

---

## 2. 構成

| フォルダ | クラス | 一言で |
|---|---|---|
| `Control/` | `WebCustomButton` `WebCustomCheckBox` `WebCustomCheckBoxList` `WebCustomDropDownList` `WebCustomFileUpload` `WebCustomImageButton` `WebCustomImageMap` `WebCustomLabel` `WebCustomLinkButton` `WebCustomListBox` `WebCustomPanel` `WebCustomRadioButton` `WebCustomRadioButtonList` `WebCustomTextBox` | 標準コントロールの派生（14 種） |
| `Interface/` | `ICheck` `IGetValue` `IMasterData` | 入力チェック、値の取得、マスタ データ名 |
| `Function/` | `CmnCheckFunction` `CheckResult` `CmnMasterDatasForList` `ListItem` | チェック結果の集計、リスト用マスタ データ |
| `PropertyGrid/` | `CheckType` `CheckTypeConverter` | デザイン時のプロパティ（チェックの種類） |

### 2.1 コントロールの中身

**ほとんどのコントロールは、描画（`RenderControl` / `Render`）を上書きできる形にしてあるだけ**で、機能は足していない。
利用者が共通の描画を差し込むための足場である。

**機能を持つのは `WebCustomTextBox` だけ**（`ICheck` / `IGetValue` を実装）。

| プロパティ | 意味 |
|---|---|
| `CheckType` | 入力チェックの種類（ビット フィールド。下表）。デザイン時は `CheckTypeConverter` で展開して編集する |
| `CheckRegExp` | 正規表現によるチェック |
| `CheckProhibitedChar` | 禁止文字のチェック |

| `CheckType` のビット | 値 | 判定に使うもの |
|---|---|---|
| `Required` | 1 | 空でないこと |
| `IsHankaku` / `IsZenkaku` | 2 / 4 | `Public.Str.StringChecker` |
| `IsNumeric` | 8 | 同上 |
| `IsKatakana` / `IsHanKatakana` / `IsHiragana` | 16 / 32 / 64 | 同上 |
| `IsDate` | 128 | 同上 |

`Validate(out string[] result)` がエラー メッセージを返し、`CmnCheckFunction.HasErrors(親コントロール, 結果)` で画面全体を集計する。
メッセージは `CmnCheckFunction` の定数（`RequiredCheckErrorMessage` など）。

`WebCustomDropDownList` の `IMasterData`（マスタ データ名から選択肢を埋める）は**コメントアウトされている**。
仕組み（`CmnMasterDatasForList`）だけが残っている。

### 2.2 フレームワークとの関係

カスタム コントロールそのものは `Framework` に依存しない。
**イベント ハンドラの結び付けは、コントロール ID の接頭辞で `BaseController` が行う**
（`FxPrefixOfButton` など。[`Framework/ANALYSIS.md`](../Framework/ANALYSIS.md) 4.1 節）。
カスタム コントロールでも標準コントロールでも、接頭辞が合えば同じように結び付く。

---

## 3. VB 版

`root/programs/VB/Frameworks/Infrastructure/CustomControl/` に VB.NET 版がある。仕様を変えたら追随の要否を報告する。

---

## 4. 落とし穴

1. **net10.0 には無い。** ASP.NET Core のサンプル（`Samples4NetCore`）では使えない。
2. **チェックの判定は `Public` の `StringChecker` に委ねている。** 判定を変えたいなら、まず `StringChecker` の仕様を確かめる
   （例: `IsNumeric("")` は false。[`Public/ANALYSIS.md`](../Public/ANALYSIS.md)）。
3. **WinForms 版（`RichClient/`）と、同じ名前のクラス（`CheckType` / `CmnCheckFunction` / `ICheck` など）がある**が、
   別の名前空間・別の実装である。片方を直しても、もう片方は変わらない。

---

## 5. 作業チェックリスト

- [ ] net48 のみ。新しいファイルは `CustomControl_net48.csproj` の `<Compile Include>` に足す
- [ ] WinForms 版（`RichClient/`）にも同じ変更が要るか
- [ ] `Samples/WebApp_sample/WebForms_Sample` で表示とチェックを確かめる
- [ ] VB 版の追随の要否を報告する
