//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

// Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

//**********************************************************************************
//* ファイル名        ：common.js
//* ファイル日本語名  ：共通のJS処理
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  201?/0?/0?  西野 大介         新規作成
//*  2015/02/06  Supragyan         Added condition for check AjaxPostBackElement in Fx_AjaxExtensionInitializeRequest
//*  2015/02/06  Supragyan         Added condition for check AjaxPostBackElement in Fx_AjaxExtensionEndRequest
//*  2015/02/09  Supragyan         Added condition for Trident on Internet Explorer
//*  2015/09/09  Sandeep           Added condition code to detect IE-9, IE-10 and IE-11, to suppress double transmission
//*  2016/01/12  Sai	           Changed interval in method window.setInterval(HttpPing, 5000)
//*  2016/01/19  Sandeep           Implemented ResolveServerUrl method to resolve URL issue in javascript
//*  2016/03/15  daisukenishino    Fix of issue that is occurred by IFRAME of IE11 correspondance
//*  2016/03/17  Bhagya            Implemented code to resolve the progress dialog mask issue in IE9 or more and other browsers
//*  2016/04/15  Sandeep           Implemented cross-browser detection method, to suppress double transmission
//*  2016/04/20  Sandeep           Created form submission flag, to suppress double transmission
//*  2016/07/05  Sandeep           Added cache property in the Ajax ping request to prevent the session timeout.
//*  2017/04/20  西野 大介         showModalDialogのないモダン・ブラウザをサポートするための実装。
//*  2017/05/11  西野 大介         擬似dialog系のstyleについて色々調整を行った（css化やsize計算方法の変更）。
//*  2019/03/04  西野 大介         リネーム（ResolveServerUrl -> Fx_ResolveServerUrl）
//*  2019/03/04  西野 大介         二重送信防止のブラウザ判定処理を修正
//*  2019/03/04  西野 大介         WebForms版とMVC版のDiffを取り易く修正。
//*  2020/03/19  西野 大介         モダン・ブラウザ・サポート（2017/04/20）の制限事項解除
//**********************************************************************************

// ---------------------------------------------------------------
// ページがロードされた時に呼ばれる
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_Document_OnLoad() {
    // OnLoad処理を別スレッドで実行
    setTimeout("Fx_Document_OnLoad2()", 1);
}

// Form submission flag, to suppress double transmission
var Form_IsSubmitted = false;

// ---------------------------------------------------------------
// ページがロードされた時に呼ばれる
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_Document_OnLoad2() {

    window.returnValue = "";

    // Cross-browser detection(先頭に移動)
    Fx_WhichBrowser();

    // Dialogの初期化
    Fx_InitDialogMask();      // Dialog Maskの初期化
    Fx_InitProgressDialog();  // Progress Dialogの初期化
    Fx_InitPseudoDialog();    // OK and Yes/No Pseudo Dialogの初期化

    // 子ウィンドウ（Dialog、Window）を 
    Fx_ShowChildScreen(); // 開く関数 
    Fx_CloseModalScreen(); // 閉じる関数

    // AjaxExtensionの初期化処理
    Fx_AjaxExtensionInit();

    // 標準スタイルのWindowを表示
    //Fx_StandardStyleWindow();

    // Sessionタイムアウト防止機能 - Open 棟梁 Wiki
    // https://opentouryo.osscons.jp/index.php?Session%E3%82%BF%E3%82%A4%E3%83%A0%E3%82%A2%E3%82%A6%E3%83%88%E9%98%B2%E6%AD%A2%E6%A9%9F%E8%83%BD
    // Webサーバへ一定時間ごとにpingを行う
    //window.setInterval(HttpPing, 5 * 60 * 1000);
}

// ---------------------------------------------------------------
// セッションタイムアウトを防ぐため、Webサーバへ一定期間ごとにPINGを行う
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
//function HttpPing() {
//    $.ajax({
//        type: 'GET',
//        url: Fx_ResolveServerUrl("~/Aspx/Framework/ping.aspx"),
//        cache:false
//    });
//}

// for diff

// ---------------------------------------------------------------
// 画面のスタイルなどを調整
// StandardStyleWindowと言う名称でルートとなるのWindowを開く。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_StandardStyleWindow() {

    var childScreenType = GetElementByName_SuffixSearch("ChildScreenType");

    // hiddenがnullになることがある・・・
    if (childScreenType === null || childScreenType === undefined) {
        return;
    }

    // childScreenTypeが「0」の場合、画面最大化を表示
    if (childScreenType.value === "0") {

        // 幅と高さ、スタイルを指定する。
        var width = 1024;
        var height = 768;
        var wt = (screen.width - width) / 2;
        var wl = (screen.height - height) / 2;
        var style = ",statusbar=false,addressbar=false,menuBar=false,toolbar=false,resizable=false,visible=true";

        var arg = "top=" + wl + ",left=" + wt + ",width=" + width + ",height=" + height + style;

        if (this.name !== 'StandardStyleWindow') {
            // 自分を指定のスタイルで開きなおし、
            if (window.open(this.location, 'StandardStyleWindow', arg) !== null) {

                // 自身（開いた元の画面）を閉じる。
                //window.opener = null;
                //window.close();

                // JavaScript：いろいろ（仮）：So-netブログ
                // http://magnus.blog.so-net.ne.jp/archive/c35376109-1
                (window.open('', '_self').opener = window).close();
            }
        }
    }
}

// ---------------------------------------------------------------
// このDialogを閉じた時に呼ばれる（ダミー）
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_Document_OnClose() {
}

// ダウンロード処理の場合、Dialogを表示しない。
var IsDownload = false;

// ---------------------------------------------------------------
// サブミットする時に呼ばれ、２重送信を抑止する。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_OnSubmit() {

    // ----------

    // このカバレージ（onSubmit）は、
    // ・ポスト バック
    // ・ASP.NET Ajax Extension
    // のどちらも、通過する。

    // ----------

    // In Chrome, Safari and Firefox, document.readyState always returns complete
    // Hence double transmission is prevented using Form_IsSubmitted flag

    // ----------

    if (Browser_IsIE) {
        
        // Detected browser is Internet Explorer

        if (navigator.appVersion.indexOf("MSIE 6.0") !== -1) {
            // IE6.0では、hrefのdoPostBackの２重送信を抑止できない。
            // （onSubmitイベントがハンドルされないため）
        }
        else if (navigator.appVersion.indexOf("MSIE 7.0") !== -1) {
            // IE7.0では完全に有効
        }
        else if (navigator.appVersion.indexOf("MSIE 8.0") !== -1) {
            // IE8.0では完全に有効
        }
        else if (navigator.appVersion.indexOf("MSIE 9.0") !== -1) {
            // IE9.0で問題の報告を受けていません。 
        }
        else if (navigator.appVersion.indexOf("MSIE 10.0") !== -1) {
            // IE10.0で問題の報告を受けていません。 
        }
        else if (navigator.appVersion.indexOf("Trident/7") !== -1) {
            // IE11.0で問題があった場合、報告をお願いします。
        }

        if (document.readyState === "complete") {

            // 受信完了

            // Ajaxでは、completeのままになるので、
            // フラグでのチェックが必要になる。
            if (Ajax_IsProgressed) {

                // An Ajax request is processing
                // Prevent other transmissions
                return false;
            }
            else {

                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }
        }
        else {

            // 受信未完了
            // Prevent other transmissions
            return false;
        }
    }
    else if (Browser_IsEdge) {

        // Detected browser is Edge

        if (document.readyState === "complete") {

            // 受信完了

            // Ajaxでは、completeのままになるので、
            // フラグでのチェックが必要になる。
            if (Ajax_IsProgressed) {

                // An Ajax request is processing
                // Prevent other transmissions
                return false;
            }
            else {

                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }
        }
        else {

            // 受信未完了
            // Prevent other transmissions
            return false;
        }
    }
    else if (Browser_IsChrome || Browser_IsSafari) {

        // Detected browser is Chrome or Safari

        if (Form_IsSubmitted) {

            // A postback or an Ajax request is processing
            // Prevent other transmissions
            return false;
        }
        else {

            // 送信許可
            Fx_SetProgressDialog();

            // Set double submission prevention flag
            Form_IsSubmitted = true;
            return true;
        }
    }
    else if (Browser_IsFirefox) {

        // Detected browser is Firefox

        if (Form_IsSubmitted) {

            // A postback or an Ajax request is processing
            // Prevent other transmissions
            return false;
        }
        else {

            // 送信許可
            Fx_SetProgressDialog();

            // Set double submission prevention flag
            Form_IsSubmitted = true;
            return true;
        }
    }    
    else if (Browser_IsOpera) {

        // Detected browser is Opera

        if (Form_IsSubmitted) {

            // A postback or an Ajax request is processing
            // Prevent other transmissions
            return false;
        }
        else {

            // 送信許可
            Fx_SetProgressDialog();

            // Set double submission prevention flag
            Form_IsSubmitted = true;
            return true;
        }
    }
    else {

        // Other browsers

        if (Form_IsSubmitted) {

            // A postback or an Ajax request is processing
            // Prevent other transmissions
            return false;
        }
        else {

            // 送信許可
            Fx_SetProgressDialog();

            // Set double submission prevention flag
            Form_IsSubmitted = true;
            return true;
        }
    }
}

//**********************************************************************************
// Dialog Mask
//**********************************************************************************

// Ajax：マスク（div）
var Fx_AjaxDialogMask;

// ---------------------------------------------------------------
// マスクの初期化
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_InitDialogMask() {

    var _div = document.createElement("div");
    _div.className = "dialog-mask";

    //"100%";では、初期表示画面サイズになってしまう。
    _div.style.height = Math.max.apply(null, [Fx_getBrowserHeight(), Fx_getContentsHeight()]) + "px";
    _div.style.width = Math.max.apply(null, [Fx_getBrowserWidth(), Fx_getContentsWidth()]) + "px";

    // div → Dialog Mask
    Fx_AjaxDialogMask = _div;
}

// ---------------------------------------------------------------
// マスクのリサイズ
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
var Fx_AjaxDialogMaskResizeTimer;

window.addEventListener('resize', function (event) {
    
    if (Fx_AjaxDialogMaskResizeTimer !== false) {
        clearTimeout(Fx_AjaxDialogMaskResizeTimer);
    }

    Fx_AjaxDialogMaskResizeTimer = setTimeout(function () {
        // マスクのサイズの再計算
        Fx_AjaxDialogMask.style.height = Math.max.apply(null, [Fx_getBrowserHeight(), Fx_getContentsHeight()]) + "px";
        Fx_AjaxDialogMask.style.width = Math.max.apply(null, [Fx_getBrowserWidth(), Fx_getContentsWidth()]) + "px";        
    }, 100); // 100 msec 間隔
});

// ---------------------------------------------------------------
// マスクする。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_DialogMaskOn() {
    document.body.appendChild(Fx_AjaxDialogMask);
}

// ---------------------------------------------------------------
// マスクを外す。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_DialogMaskOff() {
    document.body.removeChild(Fx_AjaxDialogMask);
}

//**********************************************************************************
//  Progress Dialog
//**********************************************************************************
// Ajax：Progress Dialog（div）
var Fx_AjaxProgressDialog;

// Ajax：Progress Dialogのサイズ（div）
var Fx_AjaxProgressDialog_Width = 300;
var Fx_AjaxProgressDialog_Height = 200;

// Ajax：Progress Dialogの表示タイマ
var Fx_ProgressDialogTimer;

// ---------------------------------------------------------------
// Progress Dialogの初期化
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ★★★  Dialogのデザインを変える場合は、ここを直接編集
// ---------------------------------------------------------------
function Fx_InitProgressDialog() {

    // divを生成   
    var _div = document.createElement("div");
    _div.id = "AjaxProgressDialog";
    _div.className = "progress-dialog";

    // 幅を指定
    _div.style.width = Fx_AjaxProgressDialog_Width + "px";
    _div.style.height = Fx_AjaxProgressDialog_Height + "px";

    // 内容を指定
    _div.innerHTML = "処理中です。しばらくお待ち下さい・・・<hr />";
    
    // imgを生成
    var _img = document.createElement("img");
    _img.src = Fx_ResolveServerUrl("~/images/touryo/loading.gif");
    _img.style.width = "50px";
    _img.style.height = "50px";
    _img.alt = "処理中画像";

    // divにimgを追加
    _div.appendChild(_img);

    // div → Progress Dialog
    Fx_AjaxProgressDialog = _div;
}

// ---------------------------------------------------------------
// Progress Dialog表示を仕掛ける。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_SetProgressDialog() {
    if (IsDownload) {
        // ダウンロードの場合

        // フラグを戻す
        IsDownload = false;
    }
    else {
        // ダウンロードでない場合

        // Dialog表示（２秒後）
        Fx_ProgressDialogTimer = setTimeout("Fx_DisplayProgressDialog()", 2000);
    }
}

// ---------------------------------------------------------------
// Progress Dialog表示
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_DisplayProgressDialog() {
    // はじめにタイマをクリアする。
    clearTimeout(Fx_ProgressDialogTimer);

    try {
        // 表示位置の計算
        Fx_AjaxProgressDialog.style.top = (Fx_getBrowserHeight() / 2) - (Fx_AjaxProgressDialog_Height / 2) + "px";
        Fx_AjaxProgressDialog.style.left = (Fx_getBrowserWidth() / 2) - (Fx_AjaxProgressDialog_Width / 2) + "px";
        
        // Progress Dialogを表示する。
        Fx_DialogMaskOn();
        document.body.appendChild(Fx_AjaxProgressDialog);

    } catch (e) {
        //alert( e );//エラー内容
    }
}

//**********************************************************************************
// Pseudo Dialog（擬似ダイアログ）
//**********************************************************************************
// Ajax：Pseudo Dialog（div）
var Fx_AjaxOKPseudoDialog;
var Fx_AjaxYesNoPseudoDialog;

// Ajax：Pseudo Dialogのサイズ（div）
var Fx_AjaxOKPseudoDialog_Width = 500;
var Fx_AjaxOKPseudoDialog_Height = 300;
var Fx_AjaxYesNoPseudoDialog_Width = 500;
var Fx_AjaxYesNoPseudoDialog_Height = 300;

// Ajax：Pseudo Dialogのタイムラグ
var TimeLag4BusinessPseudoModelDialog = 300;

// ---------------------------------------------------------------
// OK Pseudo Dialogの初期化
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ★★★  Dialogのデザインを変える場合は、ここを直接編集
// ---------------------------------------------------------------
function Fx_InitPseudoDialog() {

    // divを生成   
    var _div = null;

    // ---------------------------------------------------------------------
    // OK Pseudo Dialog ----------------------------------------------------
     
    _div = document.createElement("div");
    _div.id = "AjaxOKPseudoDialog";
    _div.className = "ok-dialog";

    // 幅を指定
    _div.style.width = Fx_AjaxOKPseudoDialog_Width + "px";
    _div.style.height = Fx_AjaxOKPseudoDialog_Height + "px";

    // 内容を指定
    _div.innerHTML =
            "<iframe id=\"FxIFrame\" src=\"dummy\" scrolling=\"no\" " +
                "width=\"" + (Fx_AjaxYesNoPseudoDialog_Width - 30) + "\" " +
                "height=\"" + (Fx_AjaxYesNoPseudoDialog_Height -30) + "\"></iframe>";

    // div → OK Pseudo Dialog
    Fx_AjaxOKPseudoDialog = _div;

    // ---------------------------------------------------------------------
    // Yes/No Pseudo Dialog ------------------------------------------------

    _div = document.createElement("div");
    _div.id = "AjaxYesNoPseudoDialog";
    _div.className = "yesno-dialog";

    // 幅を指定
    _div.style.width = Fx_AjaxYesNoPseudoDialog_Width + "px";
    _div.style.height = Fx_AjaxYesNoPseudoDialog_Height + "px";

    // 内容を指定
    _div.innerHTML =
            "<iframe id=\"FxIFrame\" src=\"dummy\" scrolling=\"no\" " +
                "width=\"" + (Fx_AjaxYesNoPseudoDialog_Width - 30) + "\" " +
                "height=\"" + (Fx_AjaxYesNoPseudoDialog_Height -30) + "\"></iframe>";
    
    // div → Yes/No Pseudo Dialog
    Fx_AjaxYesNoPseudoDialog = _div;

    // ---------------------------------------------------------------------

}

// ---------------------------------------------------------------
// Pseudo Dialog表示
// ---------------------------------------------------------------
// 引数    ajaxPseudoDialog, url
// 戻り値  －
// ---------------------------------------------------------------
function Fx_DisplayPseudoDialog(ajaxPseudoDialog, url) {
    try {
        // ”画面表示領域”に固定
        ajaxPseudoDialog.style.position = "fixed";

        // 表示位置の計算
        //alert(ajaxPseudoDialog.style.height);
        ajaxPseudoDialog.style.top = ((Fx_getBrowserHeight() / 2) - (parseInt(ajaxPseudoDialog.style.height, 10) / 2)) + "px";
        ajaxPseudoDialog.style.left = ((Fx_getBrowserWidth() / 2) - (parseInt(ajaxPseudoDialog.style.width, 10) / 2)) + "px";

        // urlの組み込み
        var elementChildren = ajaxPseudoDialog.children;
        for (var i = 0; i < elementChildren.length; i++) {
            if (elementChildren[i].id === "FxIFrame")
            {
                elementChildren[i].src = url;
            }
        }

        // Pseudo Dialogを表示する。
        Fx_DialogMaskOn();
        document.body.appendChild(ajaxPseudoDialog);

    } catch (e) {
        //alert( e );//エラー内容
    }
}

//**********************************************************************************
//  フレームワーク機能（Dialog）
//**********************************************************************************

// ---------------------------------------------------------------
// 子画面表示
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_ShowChildScreen() {

    // ASP.NET Web Forms の Form objectを取得
    var fobj = Fx_GetFormObject();

    // 子画面型と子画面URLは必須
    var childScreenType = GetElementByName_SuffixSearch("ctl00$ChildScreenType");
    var childScreenUrl = GetElementByName_SuffixSearch("ctl00$ChildScreenUrl");

    // hiddenがnullになることがある・・・
    if (childScreenType === null || childScreenType === undefined) {
        return;
    }
    if (childScreenUrl === null || childScreenUrl === undefined) {
        return;
    }

    // childScreenTypeが「1」の場合、「OK」Message Dialogを表示
    if (childScreenType.value === "1") {
        // Cookieフラグを確認（バックButton押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") === "TRUE") {
            // Cookieを更新し、戻るButtonで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            // OK Message Dialog を表示する。
            return Fx_ShowMessageDialog(childScreenUrl.value, true);
        }
        else {
            // 戻るButtonで戻ったので、画面は表示しない。
            return;
        }
    }

    // childScreenTypeが「2」の場合、「YES」・「NO」Message Dialogを表示
    if (childScreenType.value === "2") {
        // Cookieフラグを確認（バックButton押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") === "TRUE") {
            // Cookieを更新し、戻るButtonで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            //// myFlagの初期化
            //var myFlag = 0;

            // Yes/No Message Dialog を表示する。
            return Fx_ShowMessageDialog(childScreenUrl.value, false);
        }
        else {
            // 戻るButtonで戻ったので、画面は表示しない。
            return;
        }
    }

    // childScreenTypeが「3」の場合、業務Modal Dialogを表示
    if (childScreenType.value === "3") {
        // Cookieフラグを確認（バックButton押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") === "TRUE") {
            // Cookieを更新し、戻るButtonで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            // 業務Modal Dialog
            Fx_ShowModalScreen(childScreenUrl.value);

            return;
        }
        else {
            // 戻るButtonで戻ったので、画面は表示しない。
            return;
        }
    }

    // childScreenTypeが「4」の場合、Modeless画面を表示
    if (childScreenType.value === "4") {
        // Cookieフラグを確認（バックButton押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") === "TRUE") {
            // Cookieを更新し、戻るButtonで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            // Modeless画面
            Fx_ShowNormalScreen(childScreenUrl.value);

            return;
        }
        else {
            // 戻るButtonで戻ったので、画面は表示しない。
            return;
        }
    }
}

// ---------------------------------------------------------------
// Message Dialogを表示
// ---------------------------------------------------------------
// 引数    url
// 戻り値  window.showModalDialogの戻り値
// ---------------------------------------------------------------
function Fx_ShowMessageDialog(url, isOK) {

    var args = new Array();

    // urlキャッシュ対応処理
    var d = new Date();
    var year_now = d.getYear().toString();
    var month_now = d.getMonth().toString();
    var date_now = d.getDate().toString();
    var day_now = d.getDay().toString();
    var hour_now = d.getHours().toString();
    var minute_now = d.getMinutes().toString();
    var second_now = d.getSeconds();

    // 既にQueryStringが設定されているので、？ではなく、＆を使用する。
    url = url + "&Time=" + year_now + month_now + date_now + day_now + hour_now + minute_now + second_now;

    // マスクする。
    Fx_DialogMaskOn();

    if (Browser_IsIE) {

        try {
            // Message Dialogを表示

            // 第1引数 = URL
            // 第2引数 = 該当ページに引き渡すArrayクラス(不要ならば null)
            // 第3引数 = オプション(「項目1:値1;項目2:値2;…;項目n:値n」の形式)
            // 戻り値  = Dialog側の window.returnValue プロパティ設定値

            // ★Dialogのサイズはここに記述する。
            var ret = window.showModalDialog(url, args,
                         GetElementByName_SuffixSearch("ctl00$FxDialogStyle").value);

        } finally {
            
            // 後処理
            if (isOK) {
                Fx_CallbackOfOKMessageDialog();
            }
            else {
                Fx_CallbackOfYesNoMessageDialog(ret);
            }
        }
    }
    else {

        // モダンブラウザは擬似ダイアログを使用する。

        if (isOK) {
            Fx_DisplayPseudoDialog(Fx_AjaxOKPseudoDialog, url);
        }
        else {
            Fx_DisplayPseudoDialog(Fx_AjaxYesNoPseudoDialog, url);
        }
    }

    return ret;
}

// ---------------------------------------------------------------
// OK Message Dialogの後処理
// ---------------------------------------------------------------
// 引数    myFlag
// 戻り値  －
// ---------------------------------------------------------------
function Fx_CallbackOfOKMessageDialog(myFlag) {
    // マスクを外す。
    Fx_DialogMaskOff();

    if (Browser_IsIE) {
        // なにもしない。
    }
    else {
        // 擬似ダイアログを閉じる。
        document.body.removeChild(Fx_AjaxOKPseudoDialog);
    }
}

// ---------------------------------------------------------------
// Yes/No Message Dialogの後処理
// ---------------------------------------------------------------
// 引数    myFlag
// 戻り値  －
// ---------------------------------------------------------------
function Fx_CallbackOfYesNoMessageDialog(myFlag) {
    // マスクを外す。
    Fx_DialogMaskOff();

    if (Browser_IsIE) {
        // なにもしない。
    }
    else {
        // 擬似ダイアログを閉じる。
        document.body.removeChild(Fx_AjaxYesNoPseudoDialog);
    }

    // ASP.NET Web Forms の Form objectを取得
    var fobj = Fx_GetFormObject();

    // サブミット フラグの確認
    var submitFlag = GetElementByName_SuffixSearch("ctl00$SubmitFlag");

    if (myFlag === 0) {
        // myFlagが「0」の場合、
        // 「×」Buttonが押されたことを意味する。

        // submitFlagを「1」に設定
        submitFlag.value = "1";

        // サーバ後処理を実行するため、サブミット
        Fx_SetProgressDialog();
        fobj.submit();
    }
    else if (myFlag === 1) {
        // myFlagが「1」の場合、
        // 「YES」Buttonが押されたことを意味する。

        // submitFlagを「2」に設定
        submitFlag.value = "2";

        // サーバ後処理を実行するため、サブミット
        Fx_SetProgressDialog();
        fobj.submit();
    }
    else if (myFlag === 2) {
        // myFlagが「2」の場合、
        // 「NO」Buttonが押されたことを意味する。

        // submitFlagを「3」に設定
        submitFlag.value = "3";

        // サーバ後処理を実行するため、サブミット
        Fx_SetProgressDialog();
        fobj.submit();
    }
}


// ---------------------------------------------------------------
// Business Modal Dialogを表示
// ---------------------------------------------------------------
// 引数    url, style
// 戻り値  false（クライアント側起動の際に、イベントを無効にするため）
// ---------------------------------------------------------------
function Fx_ShowModalScreen(url, style) {

    // マスクする。
    Fx_DialogMaskOn();

    if (Browser_IsIE) {
        try {
                // Business Modal Dialogを表示

                // 引数の個数を判別
                switch (arguments.length) {
                    case 1:
                        // スタイルの指定が無い場合（サーバ起動）
                        style = GetElementByName_SuffixSearch("ctl00$BusinessDialogStyle").value;
                        break;
                    case 2:
                        // スタイルの指定が有る場合（クライアント起動）
                        break;
                    default:
                }

                // Dialog フレームへのURL
                var dialogFrameUrl = GetElementByName_SuffixSearch("ctl00$DialogFrameUrl");

                var args = new Array();
                args[0] = url;
                // 第1引数 = DialogFrameのURL
                // 第2引数 = DialogFrame → DialogLoader.htmから起動するModal画面のURL
                // 第3引数 = 画面のスタイル(「項目1:値1;項目2:値2;…;項目n:値n」の形式) 
                var ret = window.showModalDialog(dialogFrameUrl.value, args, style);
            
        } finally {
            // 後処理
            Fx_CallbackOfBusinessModalScreen();
        }
    }
    else {
        // モダンブラウザは window.open を使用する。

        // Dialog フレーム有り
        //// args代替でCookieを使用する。
        //Fx_SetCookie("fx_window_args", url, "path=/");
        //Fx_ShowNormalScreen(dialogFrameUrl.value);

        // Dialog フレーム無し
        Fx_ShowNormalScreen2(url);
    }

    return false;
}

// ---------------------------------------------------------------
// Business Modal Screenの後処理
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_CallbackOfBusinessModalScreen() {
    // Window.closed 検知までの時間差
    setTimeout(
        "Fx_CallbackOfBusinessModalScreen2()",
        TimeLag4BusinessPseudoModelDialog);
}

// ---------------------------------------------------------------
// Business Modal Screenの後処理
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_CallbackOfBusinessModalScreen2() {

    // イベントを外す
    if (Fx_BusinessPseudoModelDialog.closed) {
        // 閉じている。
        $(Fx_BusinessPseudoModelDialog).off("unload"); //, Fx_CallbackOfBusinessModalScreen);
        Fx_BusinessPseudoModelDialog = null;
    } else {
        // 閉じていない。
        // unloadのCallbackを"再度"仕掛ける。
        Fx_SetCallbackOfBusinessModalScreen();
        return; // このケースでは抜ける。
    }

    // マスクを外す。
    Fx_DialogMaskOff();

    // ASP.NET Web Forms の Form objectを取得
    var fobj = Fx_GetFormObject();

    // サブミット フラグの設定用
    var submitFlag = GetElementByName_SuffixSearch("ctl00$SubmitFlag");

    // 戻り値を確認
    var ret = Fx_GetCookie("fx_window_returnValue");

    if (ret === "1") {
        // 戻り値が１の場合、Post Back（後処理）を実行する。
        // →  後処理のためのPost Backを実行する。

        // submitFlagを「4」に設定
        submitFlag.value = "4";
        // サーバ後処理を実行するため、サブミット
        Fx_SetProgressDialog();
        fobj.submit();
    }
    else if (ret === "2") {
        // closeFlagが２の場合、Post Back（後処理）を実行しない。
        // →  なにもしない。
    }
    else if (ret === "3") {
        // closeFlagが３の場合、当該画面が、Business Modal Dialogかどうかを判定する。
        if (window.dialogArguments === null || window.dialogArguments === undefined) {
            // 当該画面が、Business Modal Dialogでない場合、Post Back（後処理）を実行する。

            // submitFlagを「4」に設定
            submitFlag.value = "4";
            // サーバ後処理を実行するため、サブミット
            Fx_SetProgressDialog();
            fobj.submit();
        }
        else {
            // 当該画面が、Business Modal Dialogの場合、自分を閉じる。
            // window.returnValue = "3";
            Fx_SetCookie("fx_window_returnValue", "3", "path=/");
            window.close();
        }
    }
    else {
        // 不明なステータスPost Back（後処理）を実行する。
        // →  後処理のためのPost Backを実行する（★ プロジェクトによって動作を変更）。

        // submitFlagを「4」に設定
        submitFlag.value = "4";
        // サーバ後処理を実行するため、サブミット
        Fx_SetProgressDialog();
        fobj.submit();
    }
}

// ---------------------------------------------------------------
// Business Modal DialogをPost Backで閉じるためのメソッド
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_CloseModalScreen() {

    var closeFlag = GetElementByName_SuffixSearch("ctl00$CloseFlag");

    // hiddenがnullになることがある・・・
    if (closeFlag === null || closeFlag === undefined) {
        return;
    }

    if (closeFlag.value === "1") {
        // closeFlagが１の場合、自画面を閉じ、親画面でPost Back（後処理）を実行する。
        //window.returnValue = "1";
        Fx_SetCookie("fx_window_returnValue", closeFlag.value, "path=/");
        Fx_WindowClose(closeFlag.value);
    }
    else if (closeFlag.value === "2") {
        // closeFlagが2の場合、自画面を閉じ、親画面でPost Back（後処理）を実行しない。
        //window.returnValue = "2";
        Fx_SetCookie("fx_window_returnValue", closeFlag.value, "path=/");
        Fx_WindowClose(closeFlag.value);
    }
    else if (closeFlag.value === "3") {
        // closeFlagが3の場合、自画面を閉じ、親のBusiness Modal Dialogも閉じる。
        //window.returnValue = "3";
        Fx_SetCookie("fx_window_returnValue", closeFlag.value, "path=/");
        Fx_WindowClose(closeFlag.value);
    }
    else {
        return;
    }
}

// ---------------------------------------------------------------
// Business Modal Dialogを閉じるwindow.closeのバリエーション
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_WindowClose(val) {
    
    if (Browser_IsIE) {
        window.close();
    }
    else {
        // $(win).on("unload", に移動した。
        //window.opener.Fx_CallbackOfBusinessModalScreen();
        window.close();
    }
}

// ---------------------------------------------------------------
// ウィンドウ（Business Modeless Screenなど）を表示
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_ShowNormalScreen(url) {

    // ウィンドウを表示(window.open)

    // 第1引数 = URL
    // 第2引数 = ウィンドウ名
    // 第3引数 = 画面のスタイル(「項目1:値1;項目2:値2;…;項目n:値n」の形式)

    // ウィンドウ名を固定にすると、複数のウィンドウが開かなくなる。
    window.open(url,
           GetElementByName_SuffixSearch("ctl00$NormalScreenTarget").value,
           GetElementByName_SuffixSearch("ctl00$NormalScreenStyle").value);
}

// Business疑似Model（実体はModeless）Screen
var Fx_BusinessPseudoModelDialog = null;

// ---------------------------------------------------------------
// モダンブラウザ版のBusiness疑似Model（実体はModeless）Screen
// ---------------------------------------------------------------
// 引数    url
// 戻り値  －
// ---------------------------------------------------------------
function Fx_ShowNormalScreen2(url) {

    // ウィンドウを表示(window.open)

    // 第1引数 = URL
    // 第2引数 = ウィンドウ名
    // 第3引数 = 画面のスタイル(「項目1:値1;項目2:値2;…;項目n:値n」の形式)

    // ウィンドウ名を、Fx_GetRandomString() を使用して可変に実装。
    Fx_BusinessPseudoModelDialog = window.open(url,
           Fx_GetRandomString(10), //Math.random().toString(),
           GetElementByName_SuffixSearch("ctl00$NormalScreenStyle").value);

    // unloadのCallbackを仕掛ける。
    Fx_SetCallbackOfBusinessModalScreen();
}

// ---------------------------------------------------------------
// モダンブラウザ版（unloadのcallbackを仕掛ける）
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_SetCallbackOfBusinessModalScreen() {
	// https://qiita.com/bgn_nakazato/items/0b0c1b8a23dcaa7959df
    setTimeout(
        "Fx_SetCallbackOfBusinessModalScreen2()",
        TimeLag4BusinessPseudoModelDialog);
}

// ---------------------------------------------------------------
// モダンブラウザ版（unloadのcallbackを仕掛ける）
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_SetCallbackOfBusinessModalScreen2() {
    $(Fx_BusinessPseudoModelDialog).on("unload", Fx_CallbackOfBusinessModalScreen);
}

//**********************************************************************************
//  フレームワーク機能（Ajax Extension）
//**********************************************************************************

// Ajax：Post Backエレメント
var AjaxPostBackElement;

// Ajax：Progress中かどうか
var Ajax_IsProgressed = false;

// ---------------------------------------------------------------
// Ajax Extensionの初期化
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_AjaxExtensionInit() {
    try {
        // チェック（Sysが無い場合はエラーとなりなにもしない）
        var sys = Sys;

        // ここのエラーは握りつぶされるので、余計なコード
        // は入れないで↓のコードブロックに追加すること。
    }
    catch (e) {
        // なにもしないで戻る。
        return;
    }

    // エラーとならなかった場合、

    // Ajax Extensionの開始終了処理の登録
    Fx_AjaxExtensionRegPreAndAfter(sys.WebForms.PageRequestManager.getInstance());
}

// ---------------------------------------------------------------
// Ajax Extensionの開始終了処理の登録
// ---------------------------------------------------------------
// 引数    pageRequestManager
// 戻り値  －
// ---------------------------------------------------------------
function Fx_AjaxExtensionRegPreAndAfter(pageRequestManager) {
    // 非同期Post Backの開始前、終了後に呼び出されるイベント・ハンドラを定義

    // 開始前イベント
    pageRequestManager.add_initializeRequest(Fx_AjaxExtensionInitializeRequest);

    // 終了後イベント
    pageRequestManager.add_endRequest(Fx_AjaxExtensionEndRequest);
}

// ---------------------------------------------------------------
// Ajax Extensionの開始前イベント処理
// ---------------------------------------------------------------
// 引数    sender, args
// ---------------------------------------------------------------
function Fx_AjaxExtensionInitializeRequest(sender, args) {

    // これが呼ばれるのは、Fx_OnSubmitの後

    // ∴ 下記の処理は、Fx_OnSubmitの二重送信防止機能に統合

    // // 現在、実行中の非同期通信が存在するかを判定
    // if (pageRequestManager.get_isInAsyncPostBack())
    // { 
    //     // 非同期通信中である場合にはエラー Messageを表示
    //     alert("二重送信です(ajax)");
    // 
    //     // 後続の処理をキャンセル
    //     args.set_cancel(true);
    // }

    // ★★ Fx_OnSubmitが呼ばれるのは、Ajax Extensionのみ。
    // 　　 ClientCallbackや、WebServiceBridgeでは、呼ばれない。

    // Post Back エレメントを取得
    AjaxPostBackElement = args.get_postBackElement();

    if (AjaxPostBackElement) {
        AjaxPostBackElement.disabled = true;
    }

    // 二重送信フラグの設定
    Ajax_IsProgressed = true;
    // ★★ ここの処理が動く前に、
    // 他が、Fx_OnSubmitを通過しないか？
}

// ---------------------------------------------------------------
// Ajax Extensionの終了後イベント処理
// ---------------------------------------------------------------
// 引数    sender, args
// ---------------------------------------------------------------
function Fx_AjaxExtensionEndRequest(sender, args) {
    // はじめにタイマをクリアする。
    clearTimeout(Fx_ProgressDialogTimer);

    // Progress Dialogを非表示にする。
    try {

        Fx_DialogMaskOff();
        document.body.removeChild(Fx_AjaxProgressDialog);

    } catch (e) {
        //alert( e );//エラー内容
    }

    // 二重送信フラグの設定
    Ajax_IsProgressed = false;

    // Reset the form submission flag.
    Form_IsSubmitted = false;

    // イベント発生元の要素を有効化

    if (AjaxPostBackElement) {
        AjaxPostBackElement.disabled = false;
    }
}

// ---------------------------------------------------------------
// ClientCallbackや、WebServiceBridge、
// XMLHTTPRequest、jQueryなど、Ajax非同期通信用
// 汎用の開始前処理、終了後処理関数を用意する（予定）。
// 
// 技術毎に、開始終了処理のイベント実装の機構を持つが、
// それを使うか、どうか、などの検討も必要になる。
// ---------------------------------------------------------------

//**********************************************************************************

// ---------------------------------------------------------------
//  ユーティリティ：element取得関数
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// elementをnameの後方一致で取得関数する関数
// ---------------------------------------------------------------
// 引数    elementのname（後方一致）
// 戻り値  取得したelement
// ---------------------------------------------------------------
function GetElementByName_SuffixSearch(name) {

    var elementName = "";

    var nameLength = name.length;
    var elementNameLength = 0;

    // ASP.NET Web Forms の Form objectを取得
    var fobj = Fx_GetFormObject();

    // 必要なHiddenは後方にあるので、
    // 後方から検索するように変更。
    var i = fobj.elements.length - 1;

    for (i; i >= 0; i--) {
        // element.nameを取得
        var e = fobj.elements[i];
        elementName = e.name;

        // name属性の存在チェック
        if (elementName === null || elementName === undefined) {
            // name属性がない。
        }
        else {

            // name属性がある。
            elementNameLength = elementName.length;

            // name属性のlengthチェック
            if (nameLength <= elementNameLength) {

                // 検索名と同じか、長い場合
                if (elementName.substring((elementNameLength - nameLength), elementNameLength) === name) {
                    // 対象elementである。
                    // elementを返す
                    return e;
                }
                else {
                    // 対象elementでない。
                    // element検索を続行
                }
            }
            else {
                // 検索名のほうが長い
            }
        }
    }
}

//**********************************************************************************
//  ユーティリティ
//**********************************************************************************

// ---------------------------------------------------------------
// Cookie処理関数
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// Cookieを参照する関数（Cookieから指定されたデータを抜きだす）
// ---------------------------------------------------------------
// 引数    Cookie名
// 戻り値  成功した時はCookie値、失敗した時はfalseを返す
// ---------------------------------------------------------------
function Fx_GetCookie(name) {
    // "="を追加
    name += "=";

    // 検索時最終項目で-1になるのを防ぐ
    myCookie = document.cookie + ";";

    // 指定されたセクション名を検索する
    start = myCookie.indexOf(name);

    if (start !== -1) {
        // 見つかった場合

        // データを抜きだす
        end = myCookie.indexOf(";", start);
        return unescape(myCookie.substring(start + name.length, end));
    }
    else {
        // 見つからなかった場合
        return false;
    }
}

// ---------------------------------------------------------------
// Cookieを設定する関数（Cookieにデータを保存する）
// ---------------------------------------------------------------
// 引数    Cookie名
// 戻り値  成功した時はtrue、失敗した時はfalseを返す
// ---------------------------------------------------------------
function Fx_SetCookie(name, value, option) {
    // nullチェック
    if ((name !== null) && (value !== null)) {
        // データ保存
        document.cookie = name + "=" + escape(value) + ";" + option;
        return true;
    }
    else {
        return false;
    }
}

// ---------------------------------------------------------------
// サイズ取得関数
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// ブラウザ画面の幅取得
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_getBrowserWidth() {
    if (window.innerWidth) {
        return window.innerWidth;
    }

    // documentがnullになることがある・・・
    if (document === null || document === undefined) {
        // 処理しない。
    }
    else {
        // 処理する。

        if (document.documentElement && document.documentElement.clientWidth !== 0) {
            return document.documentElement.clientWidth;
        }

        if (document.body) {
            return document.body.clientWidth;
        }
    }

    return 0;
}

// ---------------------------------------------------------------
// ブラウザ画面の高さ取得
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_getBrowserHeight() {

    if (window.innerHeight) {
        return window.innerHeight;
    }

    // documentがnullになることがある・・・
    if (document === null || document === undefined) {
        // 処理しない。
    }
    else {
        // 処理する。

        if (document.documentElement && document.documentElement.clientHeight !== 0) {
            return document.documentElement.clientHeight;
        }

        if (document.body) {
            return document.body.clientHeight;
        }
    }

    return 0;
}

// ---------------------------------------------------------------
// コンテンツ全体の幅を取得
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_getContentsWidth() {
    // コンテンツ全体の幅を取得する
    return Math.max.apply(
        null,
        [document.body.clientWidth,
            document.body.scrollWidth,
            document.documentElement.scrollWidth,
            document.documentElement.clientWidth]);
}

// ---------------------------------------------------------------
// コンテンツ全体の高さ取得
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_getContentsHeight() {
    // コンテンツ全体の高さを取得する
    return Math.max.apply(
        null,
        [document.body.clientHeight,
            document.body.scrollHeight,
            document.documentElement.scrollHeight,
            document.documentElement.clientHeight]);
}

// ---------------------------------------------------------------
// Cross-browser関連の関数
// ---------------------------------------------------------------

// ---------------------------------------------------------------
//  Cross-browser detection function
// ---------------------------------------------------------------

// To store client browser information
var Browser_IsIE = false;
var Browser_IsEdge = false;
var Browser_IsFirefox = false;
var Browser_IsChrome = false;
var Browser_IsOpera = false;
var Browser_IsSafari = false;
//var Browser_IsBlink = false;

// ---------------------------------------------------------------
// To detect client browser information
// ---------------------------------------------------------------
function Fx_WhichBrowser() {

    // Value will true, when the client browser is Internet Explorer 6-11
    Browser_IsIE = /*@cc_on!@*/false || !!document.documentMode;

    // Value will true, when the client browser is Edge 20+
    Browser_IsEdge = !Browser_IsIE && !!window.StyleMedia;

    // Value will true, when the client browser is Firefox 1.0+
    Browser_IsFirefox = typeof InstallTrigger !== 'undefined';

    // Value will true, when the client browser is Chrome 1+
    //Browser_IsChrome = !!window.chrome && !!window.chrome.webstore;
    Browser_IsChrome = !!window.chrome;

    // Value will true, when the client browser is Opera 8.0+
    Browser_IsOpera = (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;

    // Value will true, when the client browser is Safari 3+
    Browser_IsSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;

    // Value will true, when the client browser having Blink engine
    //Browser_IsBlink = (Browser_IsChrome || Browser_IsOpera) && !!window.CSS;
}

// ---------------------------------------------------------------
// その他
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// Get Form object in the ASP.NET Web Forms.
// ---------------------------------------------------------------
// Parameter     －
// Return value  － Form object in the ASP.NET Web Forms
// ---------------------------------------------------------------
function Fx_GetFormObject() {
    var fobj = document.aspnetForm;

    if (fobj === null || fobj === undefined) {
        fobj = document.getElementById("form1");
    }

    if (fobj === null || fobj === undefined) {
        fobj = document.getElementById("aspnetForm");
    }

    return fobj;
}

// ---------------------------------------------------------------
// Resolves the path of a specified url based on the application server
// ---------------------------------------------------------------
// Parameter     － Relative url
// Return value  － Resolved relative url
// ---------------------------------------------------------------
function Fx_ResolveServerUrl(url) {
    if (url.indexOf("~/") === 0) {
        url = baseUrl + url.substring(2);
    }
    return url;
}

// ---------------------------------------------------------------
// ランダムな文字列を生成する
// ---------------------------------------------------------------
// 引数    len
// 戻り値  Random String
// ---------------------------------------------------------------
function Fx_GetRandomString(len) {
    //使用文字の定義
    var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&=~/*-+";

    //ランダムな文字列の生成
    var result = "";
    for (var i = 0; i < len; i++) {
        result += str.charAt(Math.floor(Math.random() * str.length));
    }
    return result;
}

// ---------------------------------------------------------------
// Debug出力
// ---------------------------------------------------------------
// 引数    testLabel: ラベル, object: オブジェクト
// 戻り値  －
// ---------------------------------------------------------------
function Fx_DebugOutput(testLabel, object) {
    console.log(testLabel);
    if (object) {
        console.log(object);
        console.log(JSON.stringify(object));
    }
}