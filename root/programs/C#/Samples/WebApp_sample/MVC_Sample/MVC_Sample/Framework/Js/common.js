//**********************************************************************************
//* All Rights Reserved, Copyright (C) 2007,2012 Hitachi Solutions,Ltd.
//**********************************************************************************

// ---------------------------------------------------------------
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
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

    // マスクの初期化
    Fx_CreateMask();

    // プログレス ダイアログの初期化
    Fx_InitProgressDialog();

    // Webサーバへ一定時間ごとにpingを行う
    //window.setInterval(HttpPing, 5 * 60 * 1000);

    // Cross-browser detection
    Fx_WhichBrowser();
}

//// ---------------------------------------------------------------
//// セッションタイムアウトを防ぐため、Webサーバへ一定期間ごとにPINGを行う
//// ---------------------------------------------------------------
//// 引数    －
//// 戻り値  －
//// ---------------------------------------------------------------
//function HttpPing() {
//    $.ajax({
//        type: 'GET',
//        url: URL,
//        contentType: "application/json; charset=utf-8",
//        data: {},
//        cache:false,
//        dataType: "json",
//        success: function () {},
//        error: function () {}
//    });
//}

// ダウンロード処理の場合、ダイアログを表示しない。
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

        if (navigator.appVersion.indexOf("MSIE 6.0") != -1) {
            // IE6.0では、hrefのdoPostBackの２重送信を抑止できない。
            // （onSubmitイベントがハンドルされないため）
        }
        else if (navigator.appVersion.indexOf("MSIE 7.0") != -1) {
            // IE7.0では完全に有効
        }
        else if (navigator.appVersion.indexOf("MSIE 8.0") != -1) {
            // IE8.0では完全に有効
        }
        else if (navigator.appVersion.indexOf("MSIE 9.0") != -1) {
            // IE9.0で問題の報告を受けていません。 
        }
        else if (navigator.appVersion.indexOf("MSIE 10.0") != -1) {
            // IE10.0で問題の報告を受けていません。 
        }
        else if (navigator.appVersion.indexOf("Trident/7") != -1) {
            // IE11.0で問題があった場合、報告をお願いします。
        }

        if (document.readyState == "complete") {

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

        if (document.readyState == "complete") {

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
        // その他のブラウザ
        // ・・・
    }
}


// ---------------------------------------------------------------
// プログレス ダイアログ表示を仕掛ける。
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

        // ダイアログ表示（２秒後）
        ProgressDialogTimer = setTimeout("Fx_DisplayProgressDialog()", 2000);
    }
}

//**********************************************************************************

// ------------------------------------------------------------
//  プログレス ダイアログの表示
// ------------------------------------------------------------

// Ajax：プログレス ダイアログ（div）
var AjaxProgressDialog;

// Ajax：プログレス ダイアログの表示タイマ
var ProgressDialogTimer;

// ---------------------------------------------------------------
// プログレス ダイアログの初期化
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ★★★  ダイアログのデザインを変える場合は、ここを直接編集
// ---------------------------------------------------------------
function Fx_InitProgressDialog() {

    // divを生成   
    var _div = document.createElement("div");
    _div.id = "AjaxProgressDialog";

    // 幅を指定
    _div.style.width = AjaxProgressDialog_Width + "px";
    _div.style.height = AjaxProgressDialog_Height + "px";

    // スタイルを指定
    _div.style.top = "0px";
    _div.style.left = "0px";

    _div.style.paddingTop = "10px";
    _div.style.paddingLeft = "10px";
    _div.style.paddingRight = "10px";

    _div.style.textAlign = "center";
    _div.style.overflow = "auto";

    _div.style.position = "absolute";

    // 1000なら最前面だろうという仕様（ToMost相当が無い）
    _div.style.zIndex = "1001"; // Maskより前面に出す。

    // 内容を指定
    _div.innerHTML = "処理中です。しばらくお待ち下さい・・・<hr />";
    _div.style.backgroundColor = "lightcyan";

    // imgを生成
    var _img = document.createElement("img");

    _img.src = "/MVC_sample/Framework/Img/loading.gif";
    _img.style.width = "50px";
    _img.style.height = "50px";
    _img.alt = "処理中画像";

    // divにimgを追加
    _div.appendChild(_img);

    // div → プログレス ダイアログ
    AjaxProgressDialog = _div
}

// ---------------------------------------------------------------
// プログレス ダイアログ表示
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------

function Fx_DisplayProgressDialog() {
    // はじめにタイマをクリアする。
    clearTimeout(ProgressDialogTimer);

    try {
        // 表示位置の計算
        AjaxProgressDialog.style.top = (Fx_getContentsHeight() / 2) //(Fx_getBrowserHeight() / 2)
            - (AjaxProgressDialog_Height / 2) + "px";
        AjaxProgressDialog.style.left = (Fx_getBrowserWidth() / 2)
            - (AjaxProgressDialog_Width / 2) + "px";

        // プログレス ダイアログを表示する。
        document.body.appendChild(AjaxMask);
        document.body.appendChild(AjaxProgressDialog);

    } catch (e) {
        //alert( e );//エラー内容
    }
}

//**********************************************************************************

// ------------------------------------------------------------
//  マスクの表示
// ------------------------------------------------------------

// Ajax：マスク（div）
var AjaxMask;

// ---------------------------------------------------------------
// マスク生成
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_CreateMask() {

    var _div = document.createElement("div");

    _div.style.top = "0px";
    _div.style.left = "0px";
    _div.style.height = Fx_getContentsHeight() + "px"; //"100%";では、初期表示画面サイズになってしまう。
    _div.style.width = Fx_getBrowserWidth() + "px"; //"100%";では、初期表示画面サイズになってしまう。
    _div.style.position = "absolute";

    // 1000なら最前面だろうという仕様（ToMost相当が無い）
    _div.style.zIndex = "1000";

    // 分かりやすいように半透明 (0に設定しても良い)
    _div.style.opacity = 0.5;
    // IE 8用の透明度の設定(0に設定しても良い)
    _div.style.filter = "alpha(opacity=50)";
    // 分かりやすいように着色(若しくは無色)
    _div.style.backgroundColor = 'gray';

    AjaxMask = _div;
}

//**********************************************************************************

// ---------------------------------------------------------------
//  フレームワーク機能（Ajax）
// ---------------------------------------------------------------

// Ajax：プログレス ダイアログのサイズ（div）
var AjaxProgressDialog_Width = 300;
var AjaxProgressDialog_Height = 100;

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
    clearTimeout(ProgressDialogTimer);
    // プログレス ダイアログを非表示にする。
    try {
        document.body.removeChild(AjaxMask);
        document.body.removeChild(AjaxProgressDialog);
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

// ---------------------------------------------------------------
//  ユーティリティ：画面サイズ取得関数
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// 画面の幅取得
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_getBrowserWidth() {
    if (window.innerWidth) {
        return window.innerWidth;
    }

    // documentがnullになることがある・・・
    if (document == null || document == undefined) {
        // 処理しない。
    }
    else {
        // 処理する。

        if (document.documentElement && document.documentElement.clientWidth != 0) {
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
    if (document == null || document == undefined) {
        // 処理しない。
    }
    else {
        // 処理する。

        if (document.documentElement && document.documentElement.clientHeight != 0) {
            return document.documentElement.clientHeight;
        }

        if (document.body) {
            return document.body.clientHeight;
        }
    }

    return 0;
}

// ---------------------------------------------------------------
// コンテンツ全体の高さ取得
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_getContentsHeight() {
    // コンテンツ全体の高さを取得する
    return Math.max.apply(null, [document.body.clientHeight, document.body.scrollHeight, document.documentElement.scrollHeight, document.documentElement.clientHeight]);
}

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
    Browser_IsChrome = !!window.chrome && !!window.chrome.webstore;

    // Value will true, when the client browser is Opera 8.0+
    Browser_IsOpera = (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;

    // Value will true, when the client browser is Safari 3+
    Browser_IsSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;

    // Value will true, when the client browser having Blink engine
    //Browser_IsBlink = (Browser_IsChrome || Browser_IsOpera) && !!window.CSS;
}
