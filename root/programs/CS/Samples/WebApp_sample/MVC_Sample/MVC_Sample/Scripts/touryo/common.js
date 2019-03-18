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
//*  2015/12/28  Sai               Added Java script method for preventing session timeout.
//*  2015/12/28  Sai               Commented out window.setInterval method.
//*  2016/01/11  Sai               Removed unnecessary code.
//*  2016/01/12  Sai               Changed interval in method window.setInterval(HttpPing, 5000)
//*  2016/01/13  Sai               Removed Ajax extensions code and added JQuery's Ajax Complete method.
//*  2016/01/22  Sai               Added .ajaxSend method to prevent double submit functionality in Ajax form.
//*  2016/01/22  Sai               Added flag variable 'PreventAjaxDoubleSubmit', added if condiation to this flag in 
//*                                ajaxSend method to skip progress dialogue also added codign for setting this flag to 
//*                                false in ajaxComplete method.   
//*  2016/02/01  Sai               Fixed Progress dialog mask not displaying problem.
//*  2016/02/11  Nishi             Finish up of the prevent double submition function.
//*  2016/04/15  Sandeep           Implemented cross-browser detection method, to suppress double transmission
//*  2016/04/20  Sandeep           Created form submission flag, to suppress double transmission
//*  2016/07/05  Sandeep           Added cache property in the Ajax ping request to prevent the session timeout.
//*  2017/04/20  西野 大介         showModalDialogのないモダン・ブラウザをサポートするための実装。
//*  2017/05/11  西野 大介         擬似dialog系のstyleについて色々調整を行った（css化やsize計算方法の変更）。
//*  2019/03/04  西野 大介         リネーム（ResolveServerUrl -> Fx_ResolveServerUrl）
//*  2019/03/04  西野 大介         二重送信防止のブラウザ判定処理を修正
//*  2019/03/04  西野 大介         WebForms版とMVC版のDiffを取り易く修正。
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
function HttpPing() {
    $.ajax({
        type: 'GET',
        url: Fx_ResolveServerUrl('~/Ping'),
        contentType: "application/json; charset=utf-8",
        data: {},
        cache:false,
        dataType: "json",
        success: function () {},
        error: function () {}
    });
}

// for diff

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
//  フレームワーク機能（Ajax）
//**********************************************************************************

// Ajax：処理中かどうか
var Ajax_IsProgressed = false;
// Flag variable for enable/disable "Prevent Double Submit functionality" for Ajax.BeginForm.
var PreventAjaxDoubleSubmit = false;
//  Flag variable for control "Prevent Double Submit functionality" for Ajax.BeginForm.
var IsAborted = false;

// ---------------------------------------------------------------
// Ajaxの開始前イベント処理
// ---------------------------------------------------------------
// Enables the functionality of Prevent Double Submit for Ajax.BeginForm based on the button submitted. 
$(document).ajaxSend(function (eo, jqXHRo, settings) {
    // checks and disables progress dialogue if PreventAjaxDoubleSubmit set to true.
    if (PreventAjaxDoubleSubmit) {
        if ( Fx_OnSubmit() ) {
            // 二重送信フラグの設定
            Ajax_IsProgressed = true;
        }
        else {
            IsAborted = true;
            jqXHRo.abort();
        }
    }
});

// ---------------------------------------------------------------
// Ajaxの終了後イベント処理
// ---------------------------------------------------------------
//$(document).ajaxComplete(function (eo, jqXHRo, settings) {

// ---------------------------------------------------------------
// Ajaxの正常終了後イベント処理
// ---------------------------------------------------------------
// 引数
//     ・第1引数：イベントオブジェクト
//     ・第2引数：jqXHRオブジェクト
//     ・第3引数：ajaxのセッティング情報
// 戻り値  －
// ---------------------------------------------------------------
$(document).ajaxSuccess(function (eo, jqXHRo, settings) {
    // ajax通信が正常終了した場合
    Fx_ClearPreventDoubleSubmissionSettings();
});

// ---------------------------------------------------------------
// Ajaxの異常終了後イベント処理
// ---------------------------------------------------------------
// 引数
//     ・第1引数：イベントオブジェクト
//     ・第2引数：jqXHRオブジェクト
//     ・第3引数：ajaxのセッティング情報
//     ・第4引数：例外オブジェクト
// 戻り値  －
// ---------------------------------------------------------------
$(document).ajaxError(function (eo, jqXHRo, settings, error) {
    // ajax通信が異常終了した場合、

    if (IsAborted)
    {
        // 二重送信防止した場合。
        // ・・・何もしない。
    }
    else
    {
        // その他の異常終了。
        Fx_ClearPreventDoubleSubmissionSettings();
    }
});

// ---------------------------------------------------------------
// 二重送信防止機能の設定を解除
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_ClearPreventDoubleSubmissionSettings()
{
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
    //Disables Prevent Double Submit finctionality by setting flag to flase.
    PreventAjaxDoubleSubmit = false;
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