// ---------------------------------------------------------------
// IE限定版（キーイベント抑止、ショートカットの処理）
// ---------------------------------------------------------------

// ショートカット一覧
// Internet Explorer のキーボード ショートカット | マイクロソフト アクセシビリティ
// http://www.microsoft.com/ja-jp/enable/products/keyboard/default.aspx#ln02

// Internet Explorer 6 のショートカット キー(IE6)
// http://www.microsoft.com/ja-jp/enable/products/keyboard/ie6.aspx

// Internet Explorer で使うショートカット キー(IE7)
// http://windows.microsoft.com/ja-JP/windows-vista/Internet-Explorer-keyboard-shortcuts

// Internet Explorer 8 で使うショートカット キー(IE8)
// http://windows.microsoft.com/ja-JP/windows-vista/Internet-Explorer-8-keyboard-shortcuts

// Internet Explorer で使うショートカット キー(IE9)
// http://windows.microsoft.com/ja-jp/windows7/Internet-Explorer-keyboard-shortcuts
//   New and Useful Keyboard Shortcuts for Internet Explorer 9
//   http://technet.microsoft.com/en-us/magazine/hh145613.aspx

// Internet Explorer で使うキーボード ショートカット - Microsoft Windows ヘルプ(IE10)
// http://windows.microsoft.com/ja-JP/internet-explorer/ie-keyboard-shortcuts#ie=ie-10

/*

  // ＜凡例＜
  //・抑止不可能　　　　　：×
  //・既定で抑止する　　　：○
  //・既定で抑止しない　　：－
  //・不明なショートカット：？

  // Web ページを表示および閲覧する
  ○：F1                            ヘルプを表示する
  ○：F11                           ブラウザー ウィンドウの全画面表示と通常表示を切り替える
  －：Tab                           Web ページ、アドレス バー、またはお気に入りバーの次の項目に移動する
  －：Shift + Tab                   Web ページ、アドレス バー、またはお気に入りバーの前の項目に移動する
  ○：F7                            カーソル ブラウズを開始する
  ×：Alt + Home                    ホーム ページに移動する
  ○：Alt + →                      次のページに移動する
  ○：Alt + ←                      前のページに移動する
  ○：BackSpace                     前のページに移動する
  ○：Shift + F10                   リンクのショートカット メニューを表示する
  ○：F6                            次のフレームおよびブラウザー要素に移動する (タブ ブラウズが無効の場合にのみ有効)
  ○：Ctrl + Tab                    次のフレームおよびブラウザー要素に移動する (タブ ブラウズが無効の場合にのみ有効)
  ○：Ctrl + Shift + Tab            前のフレームおよびブラウザー要素に移動する (タブ ブラウズが無効の場合にのみ有効)
  －：↑                            ページの先頭に向かってスクロールする
  －：↓                            ページの末尾に向かってスクロールする
  －：PageUp                        ページの先頭に向かって大きくスクロールする
  －：PageDown                      ページの末尾に向かって大きくスクロールする
  －：Home                          ページの先頭に移動する
  －：End                           ページの末尾に移動する
  －：Ctrl + F                      このページで検索を行う
  ○：F5                            現在の Web ページを更新する
  ○：Ctrl + F5                     Web 上と自分のコンピューター上でページのタイム スタンプが同じ場合も、現在の Web ページを更新する
  ○：Esc                           ページのダウンロードを中止する
  ×：Ctrl + O                      新しい Web サイトまたはページを開く
  ○：Ctrl + N                      新しいウィンドウを開く
  ○：Ctrl + Shift + P              新しい InPrivate ブラウズ ウィンドウを表示する
  －：Ctrl + Shift + Delete         閲覧履歴を削除する
  ○：Ctrl + K                      タブを複製する (現在のタブを新しいタブで開く)
  ○：Ctrl + Shift + T              最後に閉じたタブをもう一度開く
  ○：Ctrl + W                      現在のウィンドウを閉じる (1 つのタブのみが開かれている場合)
  ○：Ctrl + S                      現在のページを保存する
  ○：Ctrl + P                      現在のページまたはアクティブなフレームを印刷する
  ○：Enter                           選択したリンク先に移動する
  ○：Ctrl + I                      お気に入りを開く
  ○：Ctrl + H                      履歴を開く
  ○：Ctrl + J                      ダウンロード マネージャーを開く
  ×：Alt + P                       [ページ] メニューを開く (コマンド バーが表示されている場合)
  ×：Alt + T                       [ツール] メニューを開く (コマンド バーが表示されている場合)
  ×：Alt + H                       [ヘルプ] メニューを開く (コマンド バーが表示されている場合)
  
  // タブを操作する
  ×：Ctrl + クリック               バックグラウンドで新しいタブにリンクを開く
  ×：Ctrl + Shift + クリック       フォアグラウンドで新しいタブにリンクを開く
  ○：Ctrl + T                      フォアグラウンドで新しいタブを開く
  ×：Ctrl + Tab                    タブを切り替える
  ×：Ctrl + Shift + Tab            タブを切り替える
  ○：Ctrl + W                      現在のタブを閉じる (または、タブ ブラウズが無効の場合は現在のウィンドウを閉じる)
  ？：Ctrl + n (n は 1 ～ 8 の数字)   特定のタブ番号に切り替える
  ？：Ctrl + 9                        最後のタブに切り替える
  ○：Ctrl + Alt + F4                 他のタブを閉じる
  
  // 拡大縮小を使用する（抑止不可能、WB.ExecWBを使えば少し制御できる）
  ×：Ctrl + 正符号 (+) キー        拡大する (+ 10%)
  ×：Ctrl + 負符号 (-) キー        縮小する (- 10%)
  ×：Ctrl + 0                      100% にする
  
  // アドレス バーで検索を使用する
  ○：Ctrl + E                      アドレス バーで検索クエリを開く
  ×：Alt + Enter                   新しいタブで検索クエリを開く
  ？：Ctrl + ↓                       [アドレス バー] メニューを開く (履歴、お気に入り、検索プロバイダーを表示するため)
  ○：Ctrl + Shift + L              コピーしたテキストを使って検索する
  
  // 印刷プレビューの使用
  ×：Alt + P                       ページの印刷オプションを設定し、印刷する
  ×：Alt + U                       用紙サイズ、ヘッダーとフッター、印刷の向き、ページの余白を変更する
  ×：Alt + Home                    印刷される先頭ページを表示する
  ×：Alt + ←                      印刷されるページの前のページを表示する
  ×：Alt + A                       表示するページのページ番号を入力する
  ×：Alt + →                      印刷されるページの次のページを表示する
  ×：Alt + End                     印刷される末尾のページを表示する
  ×：Alt + F                       フレームの印刷方法を指定する (このオプションは、フレームを使った Web ページを印刷する場合のみ有効)
  ×：Alt + C                       印刷プレビューを終了する
      
  // アドレス バーを使用する
  ×：Alt + D                       アドレス バーの文字列を選択する
  ○：F4                            以前入力したアドレスの一覧を表示する
  ×：Ctrl + ←                     アドレス バー上にあるカーソルを左側の次の論理単位 ("." または "/") に移動する
  ×：Ctrl + →                     アドレス バー上にあるカーソルを右側の次の論理単位 ("." または "/") に移動する
  ×：Ctrl + Enter                  アドレス バーに入力した文字列の前後に「www.」と「.com」を追加する
  ×：↑                            オートコンプリートの候補の一覧で、次の候補に移動する
  ×：↓                            オートコンプリートの候補の一覧で、前の候補に移動する

  // Internet Explorer のツールバー メニューを開く
  ×：Alt + M                       [ホーム] メニューを開く
  ×：Alt + R                       [印刷] メニューを開く
  ×：Alt + J                       RSS メニューを開く
  ×：Alt + O                       [ツール] メニューを開く
  ×：Alt + S                       [セーフティ] メニューを開く
  ×：Alt + L                       [ヘルプ] メニューを開く

  // フィード、履歴、およびお気に入りを操作する
  ○：Ctrl + D                      現在のページをお気に入りに追加する (または、フィードのプレビュー時にフィードを購読する)
  －：Ctrl + Shift + Delete         閲覧履歴を削除する
  ○：Ctrl + Shift + P              InPrivate ブラウズ ウィンドウを表示する
  ○：Ctrl + B                      [お気に入りの整理] ダイアログ ボックスを表示する
  ×：Alt + ↑                      [お気に入りの整理] ダイアログ ボックスのお気に入りの一覧で選択した項目を上に移動する
  ×：Alt + ↓                      [お気に入りの整理] ダイアログ ボックスのお気に入りの一覧で選択した項目を下に移動する
  ×：Alt + C                       お気に入りセンターを開いて、お気に入りを表示する
  ○：Ctrl + H                      お気に入りセンターを開いて、履歴を表示する
  ○：Ctrl + Shift + H              お気に入りセンターを固定して、履歴を表示する
  ○：Ctrl + Shift + J              お気に入りセンターを開いてドッキングし、フィードを表示する
  ×：Alt + Z                       [お気に入りに追加] メニューを開く (または、フィードのプレビュー時にフィードの購読を開く)
  ×：Alt + A                       メニュー バーから [お気に入り] メニューを開く

  // 編集する
  －：Ctrl + X                      選択した項目を切り取ってクリップボードにコピーする
  －：Ctrl + C                      選択した項目をクリップボードにコピーする
  －：Ctrl + V                      選択した位置へクリップボードの内容を挿入する
  －：Ctrl + A                      現在の Web ページのすべての項目を選択する
  ○：F12                           Internet Explorer 開発者ツールを開く

  // 通知バーを使用する
  ？：Alt + N                         通知バーにフォーカスを移動する
  ？：Space キー                      通知バーをクリックする

*/

// ---------------------------------------------------------------
// キーイベント抑止、ショートカットの処理
// ---------------------------------------------------------------
document.onkeydown = Fx_OnKeyDown;
document.onkeyup = Fx_OnKeyUp;
document.onclick = Fx_OnClick;

// ---------------------------------------------------------------
// 右クリック（コンテキスト メニュー表示）を抑止
// ---------------------------------------------------------------
// document.oncontextmenu = function () { return false; }

//---

// ---------------------------------------------------------------
// フラグ
// ---------------------------------------------------------------
var IsCrlKeyPressed = false;
var IsShiftKeyPressed = false;

// ---------------------------------------------------------------
// document.onclick で呼び出される Fx_OnClick 関数
// ---------------------------------------------------------------
function Fx_OnClick() {
    /*
    if(event.srcElement.href == null || event.srcElement.href == undefined) {
      //alert("1");
    }
    else {
        alert(event.srcElement.href.substr(0, 4).toUpperCase());
        if(event.srcElement.href.substr(0, 4).toUpperCase() == "HTTP") {
            //alert("2");
            if(IsCrlKeyPressed) {
                //alert("3");
                return false;
            }
        }
        else {
            //alert("4");
        }
    }
    */
}

// ---------------------------------------------------------------
// document.onkeyup で呼び出される Fx_OnKeyUp 関数
// ---------------------------------------------------------------
function Fx_OnKeyUp() {
    // フラグの制御
    /*
    if(event.ctrlKey) {
        IsCrlKeyPressed = false;
    }
    if(event.shiftKey) {
        IsShiftKeyPressed = false;
    }
    */
}

// ---------------------------------------------------------------
// document.onkeydown で呼び出される Fx_OnKeyDown 関数
// ---------------------------------------------------------------
function Fx_OnKeyDown() {
    
    var btn;
    
    // フラグの制御
    /*
    if(event.ctrlKey) {
        IsCrlKeyPressed = true;
    }
    if(event.shiftKey) {
        IsShiftKeyPressed = true;
    }
    */
    
    // 各ブラウザのキーコード表[JavaScript]
    // http://www.programming-magic.com/file/20080205232140/keycode_table.html
    // （数字はDownイベントのものを利用）
    
    // -----------------------------------------------------------
    // キーイベント抑止
    // -----------------------------------------------------------

    // Enter(13)によるサブミットを抑止
    if ((event.keyCode == 13) &&
        !(event.srcElement.type == "submit"
         || event.srcElement.type == "textarea")
        ) {
        // enterの場合 (submitボタンやtextarea領域以外)
        // Can check the undefined or null 
        if (event.srcElement.attributes.cansubmitbyenter == undefined || event.srcElement.attributes.cansubmitbyenter.value == undefined) {
            // cansubmitbyenter属性が定義されていない場合、enter不許可
            return false;
        }
        else {
            // cansubmitbyenter属性が定義されている場合
            if (event.srcElement.attributes.cansubmitbyenter.value.toLowerCase() == "true") {
                // cansubmitbyenter属性がtrueの場合は、enter許可(TextBoxの場合submit)。
            }
            else {
                // cansubmitbyenter属性がfalseの場合は、enter不許可。
                return false;
            }
        }
    }
    else {
        // enterの場合もsubmitボタンやtextarea領域はenter許可
    }

    // BackSpace(8)による、戻る操作を無効化（テキスト編集中は押下可能にする）
    if ((event.keyCode == 8) &&
        !(event.srcElement.type == "text"
        || event.srcElement.type == "password"
        || event.srcElement.type == "textarea")
        ) {
        return false;
    }

    // Alt ＋ Home(36)・←(37)・→(39)による、戻る操作を無効化
    if (event.altKey && (event.keyCode == 36 || event.keyCode == 37 || event.keyCode == 39)) { return false; }
    
    ////////////////////（新しく追加した）////////////////////
    
    //////////////////////////////////////////////////////////
    // Web ページを表示および閲覧する
    //////////////////////////////////////////////////////////
    
    // F1(112)による、ヘルプ表示操作を無効化（HTML側で）
    /*if (event.keyCode == 112) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }*/
    
    // F5(116)による、リロード操作を無効化（Ctrl + F5も）
    if (event.keyCode == 116) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    // F6(117)による、次フレーム・要素移動操作を無効化
    if (event.keyCode == 117) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    // F7(118)による、カーソル・ブラウズ操作を無効化
    if (event.keyCode == 118) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    // F10(121)による、ショートカット・メニュー表示操作を無効化
    if (event.keyCode == 121) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    // F11(122)による、全画面表示操作を無効化
    if (event.keyCode == 122) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    // Esc(27)による、ダウンロード中止操作を無効化
    if (event.keyCode == 27) { return false; }
    
    // Ctrl + H(72)による、履歴を開く操作を無効化
    if (event.ctrlKey && event.keyCode == 72) { return false; }
    
    // Ctrl + I(73)による、お気に入りを開く操作を無効化
    if (event.ctrlKey && event.keyCode == 73) { return false; }
        
    // Ctrl + J(74)による、DLマネージャを開く操作を無効化
    if (event.ctrlKey && event.keyCode == 74) { return false; }
    
    // Ctrl + K(75)による、タブ複製操作を無効化
    if (event.ctrlKey && event.keyCode == 75) { return false; }
    
    // Ctrl + N(78)による、新ウィンドウを開く操作を無効化
    if (event.ctrlKey && event.keyCode == 78) { return false; }
    
    // Ctrl + O(79)による、新ページを開く操作を無効化
    if (event.ctrlKey && event.keyCode == 79) { return false; }
    
    // Ctrl + P(80)による、ページ印刷操作を無効化
    if (event.ctrlKey && event.keyCode == 80) { return false; }
    
    // Ctrl + S(83)による、ページ保存操作を無効化
    if (event.ctrlKey && event.keyCode == 83) { return false; }
    
    // Ctrl + W(87)による、タブを閉じる操作を無効化
    if (event.ctrlKey && event.keyCode == 87) { return false; }
    
    // Ctrl + Tab(9)による、次フレーム・要素移動操作を無効化
    if (event.ctrlKey && event.keyCode == 9) { return false; }
    
    // Ctrl + Shift + Tab(9)による、次フレーム・要素移動操作を無効化
    if (event.ctrlKey && event.shiftKey && event.keyCode == 9) { return false; }
    
    // Ctrl + Shift + P(80)による、新しいInPrivate ブラウズ ウィンドウ表示操作を無効化
    if (event.ctrlKey && event.shiftKey && event.keyCode == 80) { return false; }
        
    // Ctrl + Shift + T(84)による、タブ再表示操作を無効化
    if (event.ctrlKey && event.shiftKey && event.keyCode == 84) { return false; }
    
    // Ctrl + Shift + Delete(46)による、履歴削除操作を無効化
    //if (event.ctrlKey && event.shiftKey && event.keyCode == 46) { return false; }
    
    // Alt ＋ H(72)による、[ヘルプ] メニューを開く操作を無効化
    if (event.altKey && event.keyCode == 72) { return false; }
    
    // Alt ＋ P(80)による、[ページ] メニューを開く操作を無効化
    if (event.altKey && event.keyCode == 80) { return false; }
    
    // Alt ＋ T(84)による、[ツール] メニューを開く操作を無効化
    if (event.altKey && event.keyCode == 84) { return false; }
    
    //////////////////////////////////////////////////////////
    // タブを操作する
    //////////////////////////////////////////////////////////
    
    // Ctrl + Alt + F4(115)による、他のタブを閉じる操作を無効化
    if (event.ctrlKey && event.altKey && event.keyCode == 115) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    // Ctrl + T(84)による、フォアグラウンドで新タブを開く操作を無効化
    if (event.ctrlKey && event.keyCode == 84) { return false; }
    
    // Ctrl + Tab(9)による、タブ切替操作を無効化
    if (event.ctrlKey && event.keyCode == 9) { return false; }
    
    // Ctrl + Shift + Tab(9)による、タブ切替操作を無効化
    if (event.ctrlKey && event.shiftKey && event.keyCode == 9) { return false; }
    
    // Ctrl + 1 - 9(97 - 105)による、タブ切替操作を無効化
    if (event.ctrlKey && (97 <= event.keyCode && event.keyCode <= 105)) { return false; }
    
    //////////////////////////////////////////////////////////
    // アドレス バーで検索を使用する
    //////////////////////////////////////////////////////////
    
    // Ctrl + ↓(40)による、[アドレス バー] メニューを開く操作を無効化
    if (event.ctrlKey && event.keyCode == 40) { return false; }
    
    // Ctrl + E(69)による、Google等検索操作を無効化
    if (event.ctrlKey && event.keyCode == 69) { return false; }
        
    // Ctrl + Shift + L(76)による、コピーしたテキストでの検索操作を無効化
    if (event.ctrlKey && event.shiftKey && event.keyCode == 76) { return false; }
    
    // Alt + Enter(13)による、新タブでGoogle等検索操作を無効化
    if (event.altKey && event.keyCode == 13) { return false; }
    
    //////////////////////////////////////////////////////////
    // アドレス バーを使用する
    //////////////////////////////////////////////////////////
    
    // F4(115)による、アドレス一覧表示操作を無効化
    if (event.keyCode == 115) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    // Alt + D(68)による、文字列選択操作を無効化
    if (event.altKey && event.keyCode == 68) { return false; }
    
    //////////////////////////////////////////////////////////
    // フィード、履歴、およびお気に入りを操作する
    //////////////////////////////////////////////////////////
    
    // Ctrl + B(66)による、[お気に入りの整理] ダイアログ ボックス表示操作を無効化
    if (event.ctrlKey && event.keyCode == 66) { return false; }
    
    // Ctrl + D(68)による、お気に入りに追加操作を無効化
    if (event.ctrlKey && event.keyCode == 68) { return false; }
        
    // Alt ＋ C(67)による、お気に入りセンター・お気に入り表示操作を無効化
    if (event.altKey && event.keyCode == 67) { return false; }
    
    // Ctrl + H(72)による、お気に入りセンター・履歴表示操作を無効化
    if (event.ctrlKey && event.keyCode == 72) { return false; }
    
    // Ctrl + Shift + H(72)による、お気に入りセンター・履歴固定表示操作を無効化
    if (event.ctrlKey && event.shiftKey && event.keyCode == 72) { return false; }
    
    // Ctrl + Shift + J(74)による、お気に入りセンター・ドッキング＆フィード表示操作を無効化
    if (event.ctrlKey && event.shiftKey && event.keyCode == 74) { return false; }
    
    //////////////////////////////////////////////////////////
    // 編集する
    //////////////////////////////////////////////////////////
    
    // F12(123)による、開発者ツール表示操作を無効化
    if (event.keyCode == 123) {
        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    
    /*
    // Ctrl + A(65)による、すべて選択操作を無効化
    if (event.ctrlKey && event.keyCode == 65) { return false; }
    
    // Ctrl + C(67)による、コピー操作を無効化
    if (event.ctrlKey && event.keyCode == 67) { return false; }
    
    // Ctrl + V(86)による、貼り付け操作を無効化
    if (event.ctrlKey && event.keyCode == 86) { return false; }
    
    // Ctrl + X(88)による、切り取り操作を無効化
    if (event.ctrlKey && event.keyCode == 88) { return false; }
    */
    
    //////////////////////////////////////////////////////////
    // 通知バーを使用する
    //////////////////////////////////////////////////////////
    
    // Alt ＋ N(78)による、通知バーフォーカス操作を無効化
    if (event.altKey && event.keyCode == 78) { return false; }
    
    //////////////////////////////////////////////////////////
    
    // -----------------------------------------------------------
    // 独自ショートカット（ctrl + shift + 〇キー）の実装テンプレート
    // -----------------------------------------------------------
    
    // 〇キーをJavaSctiptの処理にマップする例
    // XXXに〇キーに対応するkeyCodeを指定する。
    /*
    if ((event.ctrlKey) && (event.shiftKey) && (event.keyCode == XXX)) {
        // JavaSctiptの処理を実装
        alert("〇キーが押されました。");
        
        // 既定の操作を無効化
        event.returnValue = false;
        return false;
    }
    */
    
    // xキーをJavaSctiptの処理にマップする例
    /*
    if ((event.ctrlKey) && (event.shiftKey) && (event.keyCode == 88)) {
        // JavaSctiptの処理を実装
        alert("xキーが押されました。");
        
        // 既定の操作を無効化
        event.returnValue = false;
        return false;
    }
    */

    // -----------------------------------------------------------
    // PFキーに画面上のボタンをマップする実装テンプレート
    // -----------------------------------------------------------
    
    // GetElementByName_SuffixSearchは後方一致で検索可能なので、
    // ボタン：「ctl00$ContentPlaceHolder_A$btnButton1」の場合、
    // GetElementByName_SuffixSearch("btnButton1")で検索可能。
    // 遅い場合は、getElementsByName、getElementByIdを使用。
    
    // F1キー
    /*
    if (event.keyCode == 112) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。
        
        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
    // F2キー
    /*
    if (event.keyCode == 113) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。
        
        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
    // F3キー
    /*
    if (event.keyCode == 114) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。

        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
    // F4キー
    /*
    if (event.keyCode == 115) {
        if (event.altKey) {
            // ブラウザのショートカットが動く
        }
        else {
            // ボタンをクリック
            btn = GetElementByName_SuffixSearch("HTML上のボタン名");
            // 遅い場合は、getElementsByName、getElementByIdを使用。

            if (btn == null || btn == undefined) {
                // ボタンが無い場合。
            }
            else if (btn.isDisabled == true) {
                // ボタンが非活性の場合。
            }
            else {
                // ボタンがあり、且つ活性の場合。
                btn.focus();
                btn.click();
            }

            // 既定の操作を無効化
            event.keyCode = 0;
            return false;
        }
    }
    */
    // F5キー
    /*
    if (event.keyCode == 116) {
        if (event.ctrlKey) {
            // ブラウザのショートカットが動く
        }
        else {
            // ボタンをクリック
            btn = GetElementByName_SuffixSearch("HTML上のボタン名");
            // 遅い場合は、getElementsByName、getElementByIdを使用。

            if (btn == null || btn == undefined) {
                // ボタンが無い場合。
            }
            else if (btn.isDisabled == true) {
                // ボタンが非活性の場合。
            }
            else {
                // ボタンがあり、且つ活性の場合。
                btn.focus();
                btn.click();
            }

            // 既定の操作を無効化
            event.keyCode = 0;
            return false;
        }
    }
    */
    // F6キー
    /*
    if (event.keyCode == 117) {
        if (event.ctrlKey) {
            // ブラウザのショートカットが動く
        }
        else {
            // ボタンをクリック
            btn = GetElementByName_SuffixSearch("HTML上のボタン名");
            // 遅い場合は、getElementsByName、getElementByIdを使用。

            if (btn == null || btn == undefined) {
                // ボタンが無い場合。
            }
            else if (btn.isDisabled == true) {
                // ボタンが非活性の場合。
            }
            else {
                // ボタンがあり、且つ活性の場合。
                btn.focus();
                btn.click();
            }

            // 既定の操作を無効化
            event.keyCode = 0;
            return false;
        }
    }
    */
    // F7キー
    /*
    if (event.keyCode == 118) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。

        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
    // F8キー
    /*
    if (event.keyCode == 119) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。

        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
    // F9キー
    /*
    if (event.keyCode == 120) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。

        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
    // F10キー
    /*
    if (event.keyCode == 121) {
        if (event.shiftKey) {
            // ブラウザのショートカットが動く
        }
        else {
            // ボタンをクリック
            btn = GetElementByName_SuffixSearch("HTML上のボタン名");
            // 遅い場合は、getElementsByName、getElementByIdを使用。

            if (btn == null || btn == undefined) {
                // ボタンが無い場合。
            }
            else if (btn.isDisabled == true) {
                // ボタンが非活性の場合。
            }
            else {
                // ボタンがあり、且つ活性の場合。
                btn.focus();
                btn.click();
            }

            // 既定の操作を無効化
            event.keyCode = 0;
            return false;
        }

    }
    */
    // F11キー
    /*
    if (event.keyCode == 122) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。

        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
    // F12キー
    /*
    if (event.keyCode == 123) {
        // ボタンをクリック
        btn = GetElementByName_SuffixSearch("HTML上のボタン名");
        // 遅い場合は、getElementsByName、getElementByIdを使用。

        if (btn == null || btn == undefined) {
            // ボタンが無い場合。
        }
        else if (btn.isDisabled == true) {
            // ボタンが非活性の場合。
        }
        else {
            // ボタンがあり、且つ活性の場合。
            btn.focus();
            btn.click();
        }

        // 既定の操作を無効化
        event.keyCode = 0;
        return false;
    }
    */
}

// ---------------------------------------------------------------
// Prevents the F1 key event
// ---------------------------------------------------------------
window.onhelp = function () {
    return false;
}