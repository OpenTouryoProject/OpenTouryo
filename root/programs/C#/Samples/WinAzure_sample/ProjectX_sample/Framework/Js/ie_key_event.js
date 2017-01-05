// ---------------------------------------------------------------
// IE限定版（キーイベント抑止、ショートカットの処理）
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// キーイベント抑止、ショートカットの処理
// ---------------------------------------------------------------
document.onkeydown = Fx_OnKeyDown;

// ---------------------------------------------------------------
// 右クリック（コンテキスト メニュー表示）を抑止
// ---------------------------------------------------------------
// document.oncontextmenu = function () { return false; }

//---

// ---------------------------------------------------------------
// document.onkeydown で呼び出される Fx_OnKeyDown 関数
// ---------------------------------------------------------------
// ALTのキーイベントは無効化できない
// （ブラクラをalt+F4で強制終了などの操作があるため）。
// ---------------------------------------------------------------
function Fx_OnKeyDown() {
    
    var btn;

    // -----------------------------------------------------------
    // キーイベント抑止
    // -----------------------------------------------------------
    
    // Enterによるサブミットを抑止
    if ((event.keyCode == 13) &&
        !(event.srcElement.type == "submit" || event.srcElement.type == "textarea")
        ) {
        return false;
    }

    // BackSpaceによる、戻る操作を無効化
    if ((event.keyCode == 8) &&
        !(event.srcElement.type == "text" || event.srcElement.type == "password" || event.srcElement.type == "textarea")
        ) {
        return false;
    }

    // altキー ＋ ←・→による、戻る操作を無効化
    if (event.altKey && (event.keyCode == 37 || event.keyCode == 39)) { return false; }
    
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