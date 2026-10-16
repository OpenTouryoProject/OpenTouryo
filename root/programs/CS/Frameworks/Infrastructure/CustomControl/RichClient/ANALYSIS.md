# ANALYSIS.md — カスタム コントロール（WinForms）（Infrastructure/CustomControl/RichClient）

対象: `root/programs/CS/Frameworks/Infrastructure/CustomControl/RichClient` / 最終更新: 2026-10-16

上位: [`CustomControl/ANALYSIS.md`](../ANALYSIS.md) / [`Infrastructure/ANALYSIS.md`](../../ANALYSIS.md) / 全体: [`Frameworks/ANALYSIS.md`](../../../ANALYSIS.md)

---

## 1. これは何か

`OpenTouryo.CustomControl.RichClient`（名前空間 `Touryo.Infrastructure.CustomControl.RichClient`）。
**WinForms の標準コントロールを継承したカスタム コントロール**の**テンプレート**である。

- net48 と `net10.0-windows7.0` の両方がある
- Web 版（[`CustomControl/ANALYSIS.md`](../ANALYSIS.md)）と比べ、**入力チェックに加えて、編集（書式）の機能を持つ**
- 参照するのは `Public` / `Framework` / `Framework.RichClient`（`HintPath`）。ビルドは `BusinessRichClient_*.sln` に入っている

---

## 2. 構成

| フォルダ | クラス | 一言で |
|---|---|---|
| `Control/` | `WinCustomTextBox`（約 2000 行） | テキスト ボックス。`ICheck` / `IEdit` / `IGetValue` |
| `Control/` | `WinCustomMaskedTextBox` | マスク付きテキスト ボックス。`ICheck` / `IGetValue` |
| `Control/` | `WinCustomDropDownList` | コンボ ボックス。`IMasterData` |
| `Control/` | `WinCustom*TextBoxDgvCell` / `DgvCol` / `DgvHost` | 上の 2 つを `DataGridView` のセル・列として使う |
| `Interface/` | `ICheck` `IEdit` `IGetValue` `IMasterData` | Web 版に `IEdit` を足したもの |
| `Function/` | `CmnCheckFunction` `CheckResult` `CmnMasterDatasForList` `ListItem` | チェック結果の集計、リスト用マスタ データ |
| `PropertyGrid/` | `CheckType` `CheckTypeConverter`、`EditInitialValue` `EditAddFigure` `EditDigitsAfterDP(Converter)` `EditPadding(Converter)` | デザイン時のプロパティ |

### 2.1 チェックと編集

| 機能 | プロパティ | 意味 |
|---|---|---|
| チェック（`ICheck`） | `CheckType` / `CheckRegExp` / `CheckProhibitedChar` | Web 版と同じ考え方（判定は `Public.Str.StringChecker`） |
| 編集（`IEdit`） | `EditInitialValue` | 初期値 |
| | `EditAddFigure` | 桁区切り |
| | `EditDigitsAfterDP` | 小数点以下の桁数 |
| | `EditPadding` | 埋め文字 |

`WinCustomTextBox` は `Enter` で編集（`Edit`）を、`Validating` でチェック前の変換（`PreValidate`）とチェックを行う。
`Validating` でチェックするかどうかは `CheckValidating` プロパティで選べる。
値の編集は `Text` プロパティの中でも行う（`TextChanged` から移した）ので、**`Text` の setter を直すと編集の振る舞いが変わる**。

---

## 3. TFM 差

| | net48 | net10.0 |
|---|---|---|
| TFM | `v4.8` | `net10.0-windows7.0`（`UseWindowsForms` / `UseWPF`） |
| 外しているもの | なし | `Properties/AssemblyInfo.cs` のみ |
| `#if NETCOREAPP` の主な用途 | — | デザイン時のプロパティに `DesignerSerializationVisibility.Visible` を付ける、文字列の複製のしかたを変える |

**net10.0 の WinForms デザイナは、複合型のプロパティを既定で保存しないことがある。**
`CheckType` などに `DesignerSerializationVisibility.Visible` を付けているのはこのため。
デザイン時のプロパティを足すなら、同じ形にする。

---

## 4. 落とし穴

1. **Web 版と同じ名前のクラスがある**が、別の実装である（[`CustomControl/ANALYSIS.md`](../ANALYSIS.md) 4 節）。
2. **`DataGridView` 用の列（`DgvCol`）は、本体のコントロールのプロパティを自分でも持ち、複製するときに写す。**
   本体にプロパティを足したら、`DgvCol` / `DgvHost` にも足さないと、グリッドの中では効かない
   （`EditAddFigure` は写す行がコメントアウトされている）。
3. デザイン時のプロパティは、net10.0 で `DesignerSerializationVisibility` が要る（3 節）。

---

## 5. 作業チェックリスト

- [ ] Web 版にも同じ変更が要るか
- [ ] プロパティを足したら、`DataGridView` 用のクラスにも足したか
- [ ] net48 は `<Compile Include>` に足す。net10.0 はワイルドカードで入る
- [ ] `Samples/2CS_sample/CustCtrl_sample`（net48）と `Samples4NetCore/Legacy/2CS_sample/CustCtrl_sample`（net10.0）で確かめる
- [ ] VB 版（`root/programs/VB/Frameworks/Infrastructure/CustomControl/RichClient`）の追随の要否を報告する
