//**********************************************************************************
//* All Rights Reserved, Copyright (C) 2007,2012 Hitachi Solutions,Ltd.
//**********************************************************************************

// ------------------------------------------------------------
//  イベントハンドラ
// ------------------------------------------------------------

// ---------------------------------------------------------------
// ページがロードされた時に呼ばれる
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/02/06  Supragyan         Added condition for check AjaxPostBackElement in Fx_AjaxExtensionInitializeRequest
//*  2015/02/06  Supragyan         Added condition for check AjaxPostBackElement in Fx_AjaxExtensionEndRequest
//*  2015/02/09  Supragyan         Added condition for Trident on Internet Explorer
//*  2015/09/09  Sandeep           Added condition code to detect IE-9, IE-10 and IE-11, to suppress double transmission
//*  2016/01/12  Sai	           Changed interval in method window.setInterval(HttpPing, 5000)
//*  2016/01/19  Sandeep           Implemented ResolveServerUrl method to resolve URL issue in javascript
//*  2016/03/15  daisukenishino    Fix of issue that is occurred by IFRAME of IE11 correspondance
//*  2016/03/17  Bhagya            Implemented code to resolve the progress dialog mask issue in IE9 or more and other browsers
//**********************************************************************************

function Fx_Document_OnLoad() {
    // OnLoad処理を別スレッドで実行
    setTimeout("Fx_Document_OnLoad2()", 1);
}

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

    // 子ウィンドウを開く関数  
    Fx_ShowChildScreen();

    // 子ウィンドウ（ダイアログ）を閉じる関数
    Fx_CloseModalScreen();

    // Ajaxの初期化処理
    Fx_AjaxExtensionInit();

    // 標準スタイルのウィンドウを表示
    //Fx_StandardStyleWindow();

    // Webサーバへ一定時間ごとにpingを行う
    //window.setInterval(HttpPing, 5 * 60 * 1000);
}

//// ---------------------------------------------------------------
//// セッションタイムアウトを防ぐため、Webサーバへ一定期間ごとにPINGを行う
//// ---------------------------------------------------------------
//// 引数    －
//// 戻り値  －
//// ---------------------------------------------------------------
//function HttpPing() {
//    var httpObj = jQuery.get("/ProjectX_sample/Framework/ping.aspx", null, function () {
//        //動作確認用コード(ping先の画面のソースをアラート表示)
////        alert(httpObj.responseText);
//    });
//}

// ---------------------------------------------------------------
// 画面のスタイルなどを調整
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_StandardStyleWindow() {

    var childScreenType = GetElementByName_SuffixSearch("ChildScreenType");

    // hiddenがnullになることがある・・・
    if (childScreenType == null || childScreenType == undefined) {
        return;
    }

    // childScreenTypeが「0」の場合、画面最大化を表示
    if (childScreenType.value == "0") {

        // 幅と高さ、スタイルを指定する。
        var width = 1024;
        var height = 768;
        var wt = (screen.width - width) / 2;
        var wl = (screen.height - height) / 2;
        var style = ",statusbar=false,addressbar=false,menuBar=false,toolbar=false,resizable=false,visible=true";

        var arg = "top=" + wl + ",left=" + wt + ",width=" + width + ",height=" + height + style;

        if (this.name != 'StandardStyleWindow') {
            // 自分を指定のスタイルで開きなおし、
            if (window.open(this.location, 'StandardStyleWindow', arg) != null) {

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
// このダイアログを閉じた時に呼ばれる（ダミー）
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_Document_OnClose() {
}

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

    // ・IE6.0のhrefのdoPostBackだと動かない。
    // ・OperaのhrefのdoPostBack（二重送信時限定）だと動かない。

    // alert(navigator.appName);
    // alert(navigator.appVersion);

    // ----------

    // ↑Operaの場合、ポスト後直ちにタイマーが無効になるので、実質機能しない。
    //   ※ ただし、Ajaxの場合は、タイマーが無効にならない。

    // ----------

    if (navigator.appName == "Microsoft Internet Explorer" || navigator.appVersion.indexOf("Trident") != -1) {

        // スレイプニルもこちらに含まれる。

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

            // Ajaxがないページ
            if (Ajax_IsProgressed == null || Ajax_IsProgressed == undefined) {
                // 送信許可
                return true;
            }

            // Ajaxでは、completeのままになるので、
            // フラグでのチェックが必要になる。
            if (Ajax_IsProgressed) {

                // Ajax有効で、処理中の場合

                //                // 送信不許可
                //                alert("二重送信です(IE + Ajax)");

                // → ダイアログ表示中は、pageRequestManagerの
                //    initializeRequest、endRequestのコールバック
                //    をブロックするのでコメントアウトした。
                return false;
            }
            else {

                // Ajax有効で、処理中でない場合

                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }
        }
        else {

            // 受信未完了

            //            // 送信不許可
            //            alert("二重送信です(IE)");

            // → ダイアログ表示中は、pageRequestManagerの
            //    initializeRequest、endRequestのコールバック
            //    をブロックするのでコメントアウトした。
            return false;
        }
    }
    else if (navigator.appName == "Netscape") {
        // 実際には、Netscape、Firefox、Safari、Chromeのカバレージ

        // Netscapeはブラウザ側の機能で２重送信を抑止していたが、
        // サポートがなくなったため、サポートしない。

        // ブラウザがデフォルトでonSubmit×２を抑止している。
        // ・Firefoxはブラウザ側の機能で２重送信を抑止している。
        // ・Safariはブラウザ側の機能で２重送信を抑止している。
        // ・Chromeはブラウザ側の機能で２重送信を抑止している。

        if (document.readyState == "complete") {
            // 実際には、Safari、Chromeのカバレージ

            // Ajaxがないページ
            if (Ajax_IsProgressed == null || Ajax_IsProgressed == undefined) {
                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }

            // Ajaxでは、completeのままになるので、
            // フラグでのチェックが必要になる。
            if (Ajax_IsProgressed) {

                // Ajax有効で、処理中の場合

                //                // 送信不許可
                //                alert("二重送信です(Netscape + Ajax)");

                // → ダイアログ表示中は、pageRequestManagerの
                //    initializeRequest、endRequestのコールバック
                //    をブロックするのでコメントアウトした。
                return false;
            }
            else {

                // Ajax有効で、処理中でない場合

                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }
        }
        else if (document.readyState == null || document.readyState == undefined) {

            // 実際には、Firefoxのカバレージ

            // FirefoxでFxの２重送信抑止機能をONにした場合、
            // hrefのdoPostBackでPOSTできなくなる現象を確認した。  

            // Ajaxがないページ
            if (Ajax_IsProgressed == null || Ajax_IsProgressed == undefined) {
                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }

            // Ajaxでは、completeのままになるので、
            // フラグでのチェックが必要になる。
            if (Ajax_IsProgressed) {

                // Ajax有効で、処理中の場合

                //                // 送信不許可
                //                alert("二重送信です(Firefox + Ajax)");

                // → ダイアログ表示中は、pageRequestManagerの
                //    initializeRequest、endRequestのコールバック
                //    をブロックするのでコメントアウトした。
                return false;
            }
            else {

                // Ajax有効で、処理中でない場合

                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }

            // Firefoxの制限事項は、通常のポストバック中にajaxの２重送信を抑止できない。
            // 逆（ajax中に通常のポストバックの２重送信）は抑止できる。

        }
        else {

            // 受信未完了（こちらは通過しない）
            alert(document.readyState); // ← 故に、表示されない。

            //            // 送信不許可
            //            alert("二重送信です(Firefox)");

            // → ダイアログ表示中は、pageRequestManagerの
            //    initializeRequest、endRequestのコールバック
            //    をブロックするのでコメントアウトした。
            return false;
        }
    }
    else if (navigator.appName == "Opera") {

        // Operaでは完全に有効

        // ただし、二重送信時のhrefのdoPostBackはハンドルできない。
        // しかし、こちらは、ブラウザ側の機能で抑止している模様。

        if (document.readyState == "complete") {

            // 受信完了

            // Ajaxがないページ
            if (Ajax_IsProgressed == null || Ajax_IsProgressed == undefined) {
                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }

            // Ajaxでは、completeのままになるので、
            // フラグでのチェックが必要になる。
            if (Ajax_IsProgressed) {

                // Ajax有効で、処理中の場合

                //                // 送信不許可
                //                alert("二重送信です(Opera + Ajax)");

                // → ダイアログ表示中は、pageRequestManagerの
                //    initializeRequest、endRequestのコールバック
                //    をブロックするのでコメントアウトした。
                return false;
            }
            else {

                // Ajax有効で、処理中でない場合

                // 送信許可
                Fx_SetProgressDialog();
                return true;
            }
        }
        else {

            // 受信未完了

            //            // 送信不許可
            //            alert("二重送信です(Opera)");

            // → ダイアログ表示中は、pageRequestManagerの
            //    initializeRequest、endRequestのコールバック
            //    をブロックするのでコメントアウトした。
            return false;
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

    _img.src = ResolveServerUrl("~/Framework/Img/loading.gif");
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

// ---------------------------------------------------------------
//  フレームワーク機能（ダイアログ）
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// 子画面表示
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_ShowChildScreen() {

    var fobj = document.aspnetForm;

    if (fobj == null || fobj == undefined) {
        fobj = document.getElementById("form1");
    }

    if (fobj == null || fobj == undefined) {
        fobj = document.getElementById("aspnetForm");
    }

    // 子画面型と子画面URLは必須
    var childScreenType = GetElementByName_SuffixSearch("ctl00$ChildScreenType");
    var childScreenUrl = GetElementByName_SuffixSearch("ctl00$ChildScreenUrl");

    // hiddenがnullになることがある・・・
    if (childScreenType == null || childScreenType == undefined) {
        return;
    }
    if (childScreenUrl == null || childScreenUrl == undefined) {
        return;
    }

    // childScreenTypeが「1」の場合、「OK」メッセージ・ダイアログを表示
    if (childScreenType.value == "1") {
        // Cookieフラグを確認（バックボタン押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") == "TRUE") {
            // Cookieを更新し、戻るボタンで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            // OKMessageBox
            return Fx_ShowMessageDialog(childScreenUrl.value);
        }
        else {
            // 戻るボタンで戻ったので、画面は表示しない。
            return;
        }
    }

    // childScreenTypeが「2」の場合、「YES」・「NO」メッセージ・ダイアログを表示
    if (childScreenType.value == "2") {
        // Cookieフラグを確認（バックボタン押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") == "TRUE") {
            // Cookieを更新し、戻るボタンで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            // myFlagの初期化
            var myFlag = 0;

            // YesNoMessageBoxを表示する
            myFlag = Fx_ShowMessageDialog(childScreenUrl.value);

            // サブミット フラグの確認
            var submitFlag = GetElementByName_SuffixSearch("ctl00$SubmitFlag");

            if (myFlag == 0) {
                // myFlagが「0」の場合、
                // 「×」ボタンが押されたことを意味する。

                // submitFlagを「1」に設定
                submitFlag.value = "1";

                // サーバ後処理を実行するため、サブミット
                Fx_SetProgressDialog();
                fobj.submit();
            }
            else if (myFlag == 1) {
                // myFlagが「1」の場合、
                // 「YES」ボタンが押されたことを意味する。

                // submitFlagを「2」に設定
                submitFlag.value = "2";

                // サーバ後処理を実行するため、サブミット
                Fx_SetProgressDialog();
                fobj.submit();
            }
            else if (myFlag == 2) {
                // myFlagが「2」の場合、
                // 「NO」ボタンが押されたことを意味する。

                // submitFlagを「3」に設定
                submitFlag.value = "3";

                // サーバ後処理を実行するため、サブミット
                Fx_SetProgressDialog();
                fobj.submit();
            }

            return;
        }
        else {
            // 戻るボタンで戻ったので、画面は表示しない。
            return;
        }
    }

    // childScreenTypeが「3」の場合、業務モーダル・ダイアログを表示
    if (childScreenType.value == "3") {
        // Cookieフラグを確認（バックボタン押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") == "TRUE") {
            // Cookieを更新し、戻るボタンで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            // 業務モーダル・ダイアログ
            Fx_ShowModalScreen(childScreenUrl.value);

            return;
        }
        else {
            // 戻るボタンで戻ったので、画面は表示しない。
            return;
        }
    }

    // childScreenTypeが「4」の場合、モードレス画面を表示
    if (childScreenType.value == "4") {
        // Cookieフラグを確認（バックボタン押下時の不具合対応）
        if (Fx_GetCookie("BackButtonControl") == "TRUE") {
            // Cookieを更新し、戻るボタンで戻った時に画面を表示しないようにする。
            Fx_SetCookie("BackButtonControl", "FALSE", "path=/");

            // モードレス画面
            Fx_ShowNormalScreen(childScreenUrl.value);

            return;
        }
        else {
            // 戻るボタンで戻ったので、画面は表示しない。
            return;
        }
    }
}

// ---------------------------------------------------------------
// メッセージ ダイアログを表示
// ---------------------------------------------------------------
// 引数    url
// 戻り値  window.showModalDialogの戻り値
// ---------------------------------------------------------------
function Fx_ShowMessageDialog(url) {

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
    Fx_MaskOn();

    try {
        // ダイアログを表示(window.showModalDialog)
        // 第1引数 = URL
        // 第2引数 = 該当ページに引き渡すArrayクラス(不要ならば null)
        // 第3引数 = オプション(「項目1:値1;項目2:値2;…;項目n:値n」の形式)
        // 戻り値  = ダイアログ側の window.returnValue プロパティ設定値

        // ★ダイアログのサイズはここに記述する。
        var ret = window.showModalDialog(url, args,
                     GetElementByName_SuffixSearch("ctl00$FxDialogStyle").value);
    } finally {
        // マスクを外す。
        Fx_MaskOff();
    }

    return ret;
}


// ---------------------------------------------------------------
// モーダル画面を表示
// ---------------------------------------------------------------
// 引数    url, style
// 戻り値  false（クライアント側起動の際に、イベントを無効にするため）
// ---------------------------------------------------------------
function Fx_ShowModalScreen(url, style) {

    var fobj = document.aspnetForm;

    if (fobj == null || fobj == undefined) {
        fobj = document.getElementById("form1");
    }

    if (fobj == null || fobj == undefined) {
        fobj = document.getElementById("aspnetForm");
    }

    // 引数の個数を判別
    switch (arguments.length) {
        case 1:
            // スタイルの指定が無い場合（サーバ起動）
            style = GetElementByName_SuffixSearch("ctl00$BusinessDialogStyle").value
        case 2:
            // スタイルの指定が有る場合（クライアント起動）
        default:
    }

    // ダイアログ フレームへのURL
    var dialogFrameUrl = GetElementByName_SuffixSearch("ctl00$DialogFrameUrl");

    // サブミット フラグの設定用
    var submitFlag = GetElementByName_SuffixSearch("ctl00$SubmitFlag");

    var args = new Array();
    args[0] = url;

    // マスクする。
    Fx_MaskOn();

    try {
        // モーダル画面を表示
        // 第1引数 = DialogFrameのURL
        // 第2引数 = DialogFrame → DialogLoader.htmから起動するモーダル画面のURL
        // 第3引数 = 画面のスタイル(「項目1:値1;項目2:値2;…;項目n:値n」の形式) 
        var ret = window.showModalDialog(dialogFrameUrl.value, args, style);
        ret = Fx_GetCookie("fx_window_returnValue");

    } finally {
        // マスクを外す。
        Fx_MaskOff();
    }

    if (ret == "1") {
        // 戻り値が１の場合、ポストバック（後処理）を実行する。
        // →  後処理のためのポストバックを実行する。

        // submitFlagを「4」に設定
        submitFlag.value = "4";
        // サーバ後処理を実行するため、サブミット
        Fx_SetProgressDialog();
        fobj.submit();
    }
    else if (ret == "2") {
        // closeFlagが２の場合、ポストバック（後処理）を実行しない。
        // →  なにもしない。
    }
    else if (ret == "3") {
        // closeFlagが３の場合、当該画面が、モーダル画面かどうかを判定する。
        if (window.dialogArguments == null || window.dialogArguments == undefined) {
            // 当該画面が、モーダル画面でない場合、ポストバック（後処理）を実行する。

            // submitFlagを「4」に設定
            submitFlag.value = "4";
            // サーバ後処理を実行するため、サブミット
            Fx_SetProgressDialog();
            fobj.submit();
        }
        else {
            // 当該画面が、モーダル画面の場合、自分を閉じる。
            //window.returnValue = "3";
            Fx_SetCookie("fx_window_returnValue", "3", "path=/");
             window.close();
        }
    }
    else {
        // 不明なステータスポストバック（後処理）を実行する。
        // →  後処理のためのポストバックを実行する（★ プロジェクトによって動作を変更）。

        // submitFlagを「4」に設定
        submitFlag.value = "4";
        // サーバ後処理を実行するため、サブミット
        Fx_SetProgressDialog();
        fobj.submit();
    }

    return false;
}

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

// ---------------------------------------------------------------
// マスクする。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_MaskOn() {
    document.body.appendChild(AjaxMask);
}

// ---------------------------------------------------------------
// マスクを外す。
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_MaskOff() {
    document.body.removeChild(AjaxMask);
}

// ---------------------------------------------------------------
// モーダル画面をポストバックで閉じるためのメソッド
// ---------------------------------------------------------------
// 引数    －
// 戻り値  －
// ---------------------------------------------------------------
function Fx_CloseModalScreen() {

    var closeFlag = GetElementByName_SuffixSearch("ctl00$CloseFlag");

    // hiddenがnullになることがある・・・
    if (closeFlag == null || closeFlag == undefined) {
        return;
    }

    if (closeFlag.value == "1") {
        // closeFlagが１の場合、自画面を閉じ、
        // 親画面でポストバック（後処理）を実行する。
        //window.returnValue = "1";
        Fx_SetCookie("fx_window_returnValue", "1", "path=/");
        window.close();
    }
    else if (closeFlag.value == "2") {
        // closeFlagが２の場合、自画面を閉じ、
        // 親画面でポストバック（後処理）を実行しない。
        //window.returnValue = "2";
        Fx_SetCookie("fx_window_returnValue", "2", "path=/");
        window.close();
    }
    else if (closeFlag.value == "3") {
        // closeFlagが３の場合、自画面を閉じ、
        // 親のモーダル画面も閉じる。
        //window.returnValue = "3";
        Fx_SetCookie("fx_window_returnValue", "3", "path=/");
        window.close();
    }
}


// ---------------------------------------------------------------
// モードレス画面を表示
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

//**********************************************************************************

// ---------------------------------------------------------------
//  フレームワーク機能（Ajax）
// ---------------------------------------------------------------

// Ajax：ポストバックエレメント
var AjaxPostBackElement;

// Ajax：プログレス ダイアログのサイズ（div）
var AjaxProgressDialog_Width = 300;
var AjaxProgressDialog_Height = 100;

// Ajax：プログレス中かどうか
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
    // 非同期ポストバックの開始前、終了後に呼び出されるイベント・ハンドラを定義

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
    //     // 非同期通信中である場合にはエラー メッセージを表示
    //     alert("二重送信です(ajax)");
    // 
    //     // 後続の処理をキャンセル
    //     args.set_cancel(true);
    // }

    // ★★ Fx_OnSubmitが呼ばれるのは、Ajax Extensionのみ。
    // 　　 ClientCallbackや、WebServiceBridgeでは、呼ばれない。

    // ポストバック エレメントを取得
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

    var fobj = document.aspnetForm;

    if (fobj == null || fobj == undefined) {
        fobj = document.getElementById("form1");
    }

    if (fobj == null || fobj == undefined) {
        fobj = document.getElementById("aspnetForm");
    }

    // 必要なHiddenは後方にあるので、
    // 後方から検索するように変更。
    var i = fobj.elements.length - 1;

    for (i; i >= 0; i--) {
        // element.nameを取得
        var e = fobj.elements[i];
        elementName = e.name;

        // name属性の存在チェック
        if (elementName == null || elementName == undefined) {
            // name属性がない。
        }
        else {

            // name属性がある。
            elementNameLength = elementName.length;

            // name属性のlengthチェック
            if (nameLength <= elementNameLength) {

                // 検索名と同じか、長い場合
                if (elementName.substring((elementNameLength - nameLength), elementNameLength) == name) {
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

// ---------------------------------------------------------------
//  ユーティリティ：Cookie処理関数
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

    if (start != -1) {
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
    if ((name != null) && (value != null)) {
        // データ保存
        document.cookie = name + "=" + escape(value) + ";" + option;
        return true;
    }
    else {
        return false;
    }
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
// Resolves the path of a specified url based on the application server
// ---------------------------------------------------------------
// Parameter     － Relative url
// Return value  － Resolved relative url
// ---------------------------------------------------------------
function ResolveServerUrl(url) {
    if (url.indexOf("~/") == 0) {
        url = baseUrl + url.substring(2);
    }
    return url;
}
