var Sample2ViewModel = function ($scope) {
    // shippers テーブルのレコードリスト (JSON 形式)
    $scope.dataLists = [];

    // データアクセス制御クラス
    $scope.ddlDapItems = [
        { displayText: "SQL Server / SQL Client", value: "SQL" },
        { displayText: "Multi-DB / OLEDB.NET", value: "OLE" },
        { displayText: "Multi-DB / ODBC.NET", value: "ODB" },
        { displayText: "Oracle / ODP.NET", value: "ODP" },
        { displayText: "DB2 / DB2.NET", value: "DB2" },
        { displayText: "HiRDB / HiRDB-DP", value: "HIR" },
        { displayText: "MySQL Cnn/NET", value: "MCN" },
        { displayText: "PostgreSQL / Npgsql", value: "NPS" }
    ];
    $scope.ddlDap = 'SQL';

    // 静的、動的のクエリ モード
    $scope.ddlMode1Items = [
        { displayText: "個別Ｄａｏ", value: "individual" },
        { displayText: "共通Ｄａｏ", value: "common" },
        { displayText: "自動生成Ｄａｏ（更新のみ）", value: "generate" }
    ];
    $scope.ddlMode1 = 'individual';

    // 静的、動的のクエリ モード
    $scope.ddlMode2Items = [
        { displayText: "静的クエリ", value: "static" },
        { displayText: "動的クエリ", value: "dynamic" }
    ];
    $scope.ddlMode2 = 'static';

    // 分離レベル
    $scope.ddlIsoItems = [
        { displayText: "ノットコネクト", value: "NC" },
        { displayText: "ノートランザクション", value: "NT" },
        { displayText: "ダーティリード", value: "RU" },
        { displayText: "リードコミット", value: "RC" },
        { displayText: "リピータブルリード", value: "RR" },
        { displayText: "シリアライザブル", value: "SZ" },
        { displayText: "スナップショット", value: "SS" },
        { displayText: "デフォルト", value: "DF" }
    ];
    $scope.ddlIso = 'NT';

    // コミット、ロールバックを設定
    $scope.ddlExRollbackItems = [
        { displayText: "正常時", value: "-" },
        { displayText: "業務例外", value: "Business" },
        { displayText: "システム例外", value: "System" },
        { displayText: "その他、一般的な例外", value: "Other" },
        { displayText: "業務例外への振替", value: "Other-Business" },
        { displayText: "システム例外への振替", value: "Other-System" }
    ];
    $scope.ddlExRollback = '-';

    // 通信制御
    $scope.ddlTransmissionItems = [
        { displayText: "Webサービス呼出", value: "testWebService" },
        { displayText: "インプロセス呼出", value: "testInProcess" }
    ];
    $scope.ddlTransmission = 'testWebService';

    // Shipper テーブルの各項目
    $scope.ShipperId = '';
    $scope.CompanyName = '';
    $scope.Phone = '';

    // 並び替え対象列
    $scope.ddlOrderColumnItems = [
        { displayText: 'c1', value: 'c1' },
        { displayText: 'c2', value: 'c2' },
        { displayText: 'c3', value: 'c3' }
    ];
    $scope.ddlOrderColumn = 'c1';

    // 昇順・降順
    $scope.ddlOrderSequenceItems = [
        { displayText: "ASC", value: "A" },
        { displayText: "DESC", value: "D" }
    ];
    $scope.ddlOrderSequence = 'A';

    // 処理結果 (正常系)
    $scope.Result = '';

    // 処理結果 (異常系)
    $scope.ErrorMessage = '';

    // ボタンコマンド (件数取得)
    $scope.GetCount = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/GetCount',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function(){
                        // 結果格納
                        $scope.Result = data.message;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    // ボタンコマンド (一覧取得（dt）)
    $scope.GetList_dt = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/SelectDT',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 一旦レコードリストをクリアする
                        $scope.ClearList();

                        // 結果格納
                        $scope.dataLists = data;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.GetList_ds = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/SelectDS',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 一旦レコードリストをクリアする
                        $scope.ClearList();

                        // 結果格納
                        $scope.dataLists = data;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.GetList_dr = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/SelectDR',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 一旦レコードリストをクリアする
                        $scope.ClearList();

                        // 結果格納
                        $scope.dataLists = data;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.GetList_dsql = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback,
            OrderColumn: $scope.ddlOrderColumn,
            OrderSequence: $scope.ddlOrderSequence
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/SelectDSQL',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 一旦レコードリストをクリアする
                        $scope.ClearList();

                        // 結果格納
                        $scope.dataLists = data;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.GetDetail = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback,
            ShipperId: $scope.ShipperId
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/Select',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 結果格納
                        $scope.ShipperId = data.shipperID;
                        $scope.CompanyName = data.companyName;
                        $scope.Phone = data.phone;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.InsertRecord = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback,
            CompanyName: $scope.CompanyName,
            Phone: $scope.Phone
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/Insert',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 結果格納
                        $scope.Result = data.message;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.UpdateRecord = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback,
            ShipperId: $scope.ShipperId,
            CompanyName: $scope.CompanyName,
            Phone: $scope.Phone
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/Update',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 結果格納
                        $scope.Result = data.message;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.DeleteRecord = function () {
        // エラーメッセージをクリアする
        $scope.ErrorMessage = "";

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: $scope.ddlDap,
            ddlMode1: $scope.ddlMode1,
            ddlMode2: $scope.ddlMode2,
            ddlExRollback: $scope.ddlExRollback,
            ShipperId: $scope.ShipperId
        }

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: '/api/Delete',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    $scope.$apply(function () {
                        // エラーメッセージ格納
                        $scope.ErrorMessage = data.error;
                    });
                }
                else {
                    $scope.$apply(function () {
                        // 結果格納
                        $scope.Result = data.message;
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $scope.$apply(function () {
                    // エラーメッセージ格納
                    $scope.ErrorMessage = XMLHttpRequest.responseText;
                });
            }
        });
    };

    $scope.ClearList = function () {
        // レコードリストをクリアする
        $scope.dataLists = [];
    }

    // エラーメッセージを監視し、メッセージが設定されたらエラーダイアログを出す
    $scope.$watch('ErrorMessage', function (newVal, oldVal) {
        if (newVal != '') {
            $('<div>' + newVal + '</div>').dialog({
                title: 'エラーが発生しました',
                modal: true,
                resizable: false,
                height: 600,
                width: 800,
                buttons: {
                    'OK': function (event) {
                        $(this).dialog('close');
                    }
                }
            });
        }
    }, true);
}
