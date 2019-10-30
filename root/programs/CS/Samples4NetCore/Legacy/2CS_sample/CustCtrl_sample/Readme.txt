Windows Formカスタム・コントロールの利用サンプル
デザインタイム・プロパティからチェック処理を変更可能。

以下のクラスの仕様を説明する。
  ・WinCustomTextBox.cs(.vb)
  ・WinCustomMaskedTextBox.cs(.vb)

プロパティ
  ●は双方のクラス共通のプロパティ
  t)はWinCustomTextBoxのみのプロパティ
  m)はWinCustomMaskedTextBoxのみのプロパティ

＜拡張プロパティ（既存のプロパティのoverrideや追加のプロパティ）＞
  ●Text（override）：
    ・型：string（既定値：空文字列）
    ・説明：元のTextプロパティ（表示されているTextと同じ値を返す）
    
  t)Value（追加）：
    ・型：object型
    ・説明：単位指定可能なValueプロパティ（使用する際はDisplayUnitsに0以上の値を設定）
      ・Textプロパティに変換する際に単位変換を行う（例：実値＝円、表示値＝M円など）。
      ・また、EditDigitsAfterDPよりEditDigitsAfterDP_Editingの方が精度が高い場合、
        Valueを使用すると、EditDigitsAfterDP_Editingの精度でデータを取得できるようになる。
        
  m)TextMaskFormat（override）：
    ・型：MaskFormat列挙体
    ・説明：読み取り専用に動作を変更してある（IncludeLiterals固定）。

    ・MaskFormat列挙体：
      ・IncludeLiterals：
        ・型：int
        ・説明：ユーザーによって入力されたテキストのほか、マスクで定義されたリテラル文字を返します。
        
  m)Text2（追加）：
    ・型：string（既定値：空文字列）
    ・説明：編集処理を施す前のユーザ入力のTextプロパティ（WinCustomMaskedTextBoxではマスクを除いた値）
    
  m)Text3（追加）：
    ・型：string（既定値：空文字列）
    ・説明：編集処理を施した後のTextプロパティ（WinCustomMaskedTextBoxでは表示時マスク適用時の値）
    
＜デザインタイム・プロパティ（追加のプロパティ）＞
  
  ＜チェック系＞
    ●CheckValidatingプロパティ：
      ・型：bool（既定値：false）
      ・説明：Validatingイベントでチェックするかどうか
      
    ●CheckTypeプロパティ：
      ・型：CheckTypeクラス
      ・説明：チェック・パターンを定義
      
      ・CheckTypeクラス：
        ・IsIndispensabile：
          ・型：bool（既定値：false）
          ・説明：必須入力チェックを有効にする。
        ・IsHankaku：
          ・型：bool（既定値：false）
          ・説明：半角チェックを有効にする。
                  空文字列はチェック対象外。
        ・IsZenkaku：
          ・型：bool（既定値：false）
          ・説明：全角チェックを有効にする。
                  空文字列はチェック対象外。
        ・IsNumeric：
          ・型：bool（既定値：false）
          ・説明：数値チェックを有効にする（Double型にTryParseできるかどうか）。
                  空文字列はチェック対象外。
        ・IsKatakana：
          ・型：bool（既定値：false）
          ・説明：片仮名チェックを有効にする。
                  空文字列はチェック対象外。
        ・IsHanKatakana：
          ・型：bool（既定値：false）
          ・説明：半角片仮名チェックを有効にする。
                  空文字列はチェック対象外。
        ・IsHiragana：
          ・型：bool（既定値：false）
          ・説明：平仮名チェックを有効にする。
                  空文字列はチェック対象外。
        ・IsDate：
          ・型：bool（既定値：false）
          ・説明：日付チェックを有効にする（DateTime型にTryParseできるかどうか）。
                  空文字列はチェック対象外。

    ●CheckRegExpプロパティ：
      ・型：string（既定値：空文字列）
      ・説明：正規表現チェック・パターンを定義

    ●CheckProhibitedCharプロパティ：
      ・型：bool（既定値：false）
      ・説明：禁則文字チェックを有効にする（禁則文字はライブラリの固定値）。

  ＜編集系＞
    ●EditInitialValueプロパティ：
      ・型：EditInitialValue列挙型
      ・説明：初期値を定義
              空文字初期化時や空文字クリア時の初期値を指定する。
              デザイナ中では動作しない。Textプロパティを優先する。
              
      ・EditInitialValue列挙型：
        ・Blank（既定値）：
          ・型：int
          ・説明：空文字列
        ・Zero：
          ・型：int
          ・説明：「0」

    t)EditAddFigureプロパティ：
      ・型：EditAddFigure列挙型
      ・説明：桁区切りを定義
      
      ・EditAddFigure列挙型：
        ・None（既定値）：
          ・型：int
          ・説明：編集なし
        ・Af3：
          ・型：int
          ・説明：三桁区切り
        ・Af4：
          ・型：int
          ・説明：四桁区切り

    t)EditPaddingプロパティ：
      ・型：EditPaddingクラス
      ・説明：文字埋め編集を定義
      
      ・EditPaddingクラス：
        ・PadDirection：
          ・型：PadDirection列挙型
          ・説明：パッド方向を指定
        ・PadChar：
          ・型：char?
          ・説明：パッド文字を指定（nullは半角スペースを意味する）
          
      ・PadDirection列挙型：
        ・None（既定値）：
          ・型：int
          ・説明：編集なし
        ・Right：
          ・型：int
          ・説明：右側にパッド
        ・Left：
          ・型：int
          ・説明：左側にパッド
          
    t)EditDigitsAfterDPプロパティ：
      ・型：EditDigitsAfterDPクラス
      ・説明：（編集後）小数点以下ｘ桁編集を定義
      
      ・EditDigitsAfterDPクラス：
        ・HowToCut：
          ・型：CutMethod?列挙型
          ・説明：切り方を指定
        ・DigitsAfterDP：
          ・型：uint（既定値：0）
          ・説明：小数点数以下ｘ桁を指定
          
      ・CutMethod列挙型：
        ・None（既定値）：
          ・型：int
          ・説明：編集なし
        ・Banker：
          ・型：int
          ・説明：最近接偶数編集
        ・_4sya5nyu：
          ・型：int
          ・説明：四捨五入
        ・Floor：
          ・型：int
          ・説明：切り捨て
        ・Ceiling：
          ・型：int
          ・説明：切り上げ
          
    t)EditDigitsAfterDP_Editingプロパティ：
      ・型：EditDigitsAfterDPクラス
      ・説明：（編集中）小数点以下ｘ桁編集を定義
      
      ・EditDigitsAfterDPクラス：同上
      ・CutMethod列挙型：同上
          
    t)DisplayUnitsプロパティ：
      ・型：uint?
      ・説明：
        ・単位編集を定義（10の^ｎ乗のｎを定義）
        ・編集時は、Value値 / 10の^ｎ乗で単位を変更する。
        ・ちなみに、10の^0乗は、1で単位変更無しとなる。
        
    m)Mask_Editingプロパティ：
      ・型：string（既定値：空文字列）
      ・説明：編集中のマスク（このコントロールに許可される入力を管理する文字列を設定します。
      
    m)HankakuOnlyプロパティ：
      ・型：bool（既定値：false）
      ・説明：編集中のマスクが半角制限できないので、別途半角編集を設定する。

・・・

＜データバインディングの対象プロパティ＞
 - WinCustomTextBox
   - Text  : 通常のTextプロパティ（画面表示用）
             バインド可能だが、単位変換に対応していない。
   - Text2 : ユーザ入力のTextを取得するプロパティ
             ココにバインドしてばダメ（編集処理が動作しない）。
   - Text3 : 編集処理込のTextを取得するプロパティ
             バインド可能だが、単位変換に対応していない。
   - Value : 単位変換に対応したプロパティ
             単位変換がある場合、Valueを使用しないと動作しない。

 - WinCustomMaskedTextBox
   - Text  : 通常のTextプロパティ（画面表示用）
             データバインディングで利用可能。
   - Text2 : マスクを除いた値を設定・取得するプロパティ
             データバインディングで利用不可能。
   - Text3 : 表示時マスク適用時の値を取得するプロパティ
             取得専用のため入力単方向のデータバインディングのみ利用可能。
