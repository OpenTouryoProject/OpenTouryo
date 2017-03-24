//コードはここに挿入。
$(function () {
    var menu = $('#slide_menu'), // スライドインするメニューを指定
        menuBtn = $('#btnOpenSideMenu'), // メニューボタンを指定
        body = $(document.body),
        menuWidth = menu.outerWidth();

    // メニューボタンをクリックしたときの動き
    menuBtn.on('click', function () {
        // body に open クラスを付与する
        body.toggleClass('open');
        if (body.hasClass('open')) {
            // open クラスが body についていたらメニューをスライドインする
            body.animate({ 'left': menuWidth }, 300);
            menu.animate({ 'left': 0 }, 300);
            menuBtn.find('span#buttontext').text('オプションを閉じる');
        } else {
            // open クラスが body についていなかったらスライドアウトする
            menu.animate({ 'left': -menuWidth }, 300);
            body.animate({ 'left': 0 }, 300);
            menuBtn.find('span#buttontext').text('オプションを開く');
        }
    });

    // Web API のサーバーの URL
    var baseUrl = 'http://localhost:63877/SPA_Sample/';

    // 件数表示ボタンをクリックしたときの動き
    $('#btnGetCount').on('click', function () {
        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $('#ddlDap').val(),
            ddlMode1: $('#ddlMode1').val(),
            ddlMode2: $('#ddlMode2').val(),
            ddlExRollback: $('#ddlExRollback').val()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/GetCount',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    $('#result').text(data.error);
                }
                else {
                    // 結果格納
                    $('#result').text(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                $('#result').text(XMLHttpRequest.responseText);
            }
        });
    });

    // 一覧検索ボタンをクリックしたときの動き
    $('#btnGetList').on('click', function () {
        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $('#ddlDap').val(),
            ddlMode1: $('#ddlMode1').val(),
            ddlMode2: $('#ddlMode2').val(),
            ddlExRollback: $('#ddlExRollback').val()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/SelectDT',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    $('#result').text(data.error);
                }
                else {
                    // 結果格納
                    $('#result').text('正常終了しました');

                    // 一旦、リストをクリアする
                    $('#lstTable tbody').empty();

                    // リストを表示する
                    $.each(data, function (i, item) {
                        $('#lstTable tbody').append('<tr></tr>');
                        $('#lstTable tbody tr:last').append('<td>' + item.shipperID + '</td>');
                        $('#lstTable tbody tr:last').append('<td>' + item.companyName + '</td>');
                        $('#lstTable tbody tr:last').append('<td>' + item.phone + '</td>');
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                $('#result').text(XMLHttpRequest.responseText);
            }
        });
    });

    // 一件検索ボタンをクリックしたときの動き
    $('#btnGetRecord').on('click', function () {
        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $('#ddlDap').val(),
            ddlMode1: $('#ddlMode1').val(),
            ddlMode2: $('#ddlMode2').val(),
            ddlExRollback: $('#ddlExRollback').val(),
            ShipperId: $('#txtShipperId').val()
        };

        if (param.ShipperId === "") {
            $('#result').text('検索する Shipper Id が入力されていません。１件表示の Shipper Id テキストボックスに、検索する Shipper Id を入力してください。');
            return;
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Select',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    $('#result').text(data.error);
                }
                else {
                    // 結果格納
                    $('#result').text('正常終了しました');
                    $('#txtShipperId').val(data.shipperID);
                    $('#txtCompanyName').val(data.companyName);
                    $('#txtPhone').val(data.phone);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                $('#result').text(XMLHttpRequest.responseText);
            }
        });
    });

    // 追加ボタンをクリックしたときの動き
    $('#btnInsert').on('click', function () {
        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $('#ddlDap').val(),
            ddlMode1: $('#ddlMode1').val(),
            ddlMode2: $('#ddlMode2').val(),
            ddlExRollback: $('#ddlExRollback').val(),
            CompanyName: $('#txtCompanyName').val(),
            Phone: $('#txtPhone').val()
        };

        if (param.CompanyName === "" || param.Phone === "") {
            $('#result').text('Company Name, Phone のいずれかが入力されていません。１件表示の Compnay Name, Phone テキストボックスに、追加する値を入力してください。（Shipper Id は、自動採番されます）');
            return;
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Insert',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    $('#result').text(data.error);
                }
                else {
                    // 結果格納
                    $('#result').text(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                $('#result').text(XMLHttpRequest.responseText);
            }
        });
    });

    // 更新ボタンをクリックしたときの動き
    $('#btnUpdate').on('click', function () {
        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $('#ddlDap').val(),
            ddlMode1: $('#ddlMode1').val(),
            ddlMode2: $('#ddlMode2').val(),
            ddlExRollback: $('#ddlExRollback').val(),
            ShipperId: $('#txtShipperId').val(), 
            CompanyName: $('#txtCompanyName').val(),
            Phone: $('#txtPhone').val()
        };

        if (param.ShipperId === "" || param.CompanyName === "" || param.Phone === "") {
            $('#result').text('Shipper Id, Company Name, Phone のいずれかが入力されていません。１件表示の Shipper Id テキストボックスに更新対象の Shipper Id の値を、Compnay Name, Phone テキストボックスに、更新する値をそれぞれ入力してください。');
            return;
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Update',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    $('#result').text(data.error);
                }
                else {
                    // 結果格納
                    $('#result').text(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                $('#result').text(XMLHttpRequest.responseText);
            }
        });
    });

    // 削除ボタンをクリックしたときの動き
    $('#btnDelete').on('click', function () {
        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $('#ddlDap').val(),
            ddlMode1: $('#ddlMode1').val(),
            ddlMode2: $('#ddlMode2').val(),
            ddlExRollback: $('#ddlExRollback').val(),
            ShipperId: $('#txtShipperId').val()
        };

        if (param.ShipperId === "") {
            $('#result').text('削除する Shipper Id が入力されていません。１件表示の Shipper Id テキストボックスに削除対象の Shipper Id の値を入力してください。');
            return;
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Delete',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    $('#result').text(data.error);
                }
                else {
                    // 結果格納
                    $('#result').text(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                $('#result').text(XMLHttpRequest.responseText);
            }
        });
    });

    // クリアボタンをクリックしたときの動き
    $('#btnClear').on('click', function () {
        // リストをクリアする
        $('#lstTable tbody').empty();
    });
});
