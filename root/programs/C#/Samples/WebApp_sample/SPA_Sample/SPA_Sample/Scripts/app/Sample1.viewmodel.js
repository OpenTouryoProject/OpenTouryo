function Sample1ViewModel() {
    // shippers テーブルのレコードリスト (JSON 形式)
    this.dataLists = ko.observableArray();

    // データアクセス制御クラス
    this.ddlDapItems = ko.observableArray([
        { displayText: "SQL Server / SQL Client", value: "SQL" }, 
        { displayText: "Multi-DB / OLEDB.NET", value: "OLE" },
        { displayText: "Multi-DB / ODBC.NET", value: "ODB" },
        { displayText: "Oracle / ODP.NET", value: "ODP" },
        { displayText: "DB2 / DB2.NET", value: "DB2" },
        { displayText: "HiRDB / HiRDB-DP", value: "HIR" },
        { displayText: "MySQL Cnn/NET", value: "MCN" },
        { displayText: "PostgreSQL / Npgsql", value: "NPS" }
    ]);
    this.ddlDap = ko.observable("SQL");

    // 静的、動的のクエリ モード
    this.ddlMode1Items = ko.observableArray([
        { displayText: "個別Ｄａｏ", value: "individual" }, 
        { displayText: "共通Ｄａｏ", value: "common" },
        { displayText: "自動生成Ｄａｏ（更新のみ）", value: "generate" }
    ]);
    this.ddlMode1 = ko.observable("individual");

    // 静的、動的のクエリ モード
    this.ddlMode2Items = ko.observableArray([
        { displayText: "静的クエリ", value: "static" },
        { displayText: "動的クエリ", value: "dynamic" }
    ]);
    this.ddlMode2 = ko.observable("static");

    // 分離レベル
    this.ddlIsoItems = ko.observableArray([
        { displayText: "ノットコネクト", value: "NC" },
        { displayText: "ノートランザクション", value: "NT" },
        { displayText: "ダーティリード", value: "RU" },
        { displayText: "リードコミット", value: "RC" },
        { displayText: "リピータブルリード", value: "RR" },
        { displayText: "シリアライザブル", value: "SZ" },
        { displayText: "スナップショット", value: "SS" },
        { displayText: "デフォルト", value: "DF" }
    ]);
    this.ddlIso = ko.observable("NT");

    // コミット、ロールバックを設定
    this.ddlExRollbackItems = ko.observableArray([
        { displayText: "正常時", value: "-" },
        { displayText: "業務例外", value: "Business" },
        { displayText: "システム例外", value: "System" },
        { displayText: "その他、一般的な例外", value: "Other" },
        { displayText: "業務例外への振替", value: "Other-Business" },
        { displayText: "システム例外への振替", value: "Other-System" }
    ]);
    this.ddlExRollback = ko.observable("-");

    // 通信制御
    this.ddlTransmissionItems = ko.observableArray([
        { displayText: "Webサービス呼出", value: "testWebService" },
        { displayText: "インプロセス呼出", value: "testInProcess" }
    ]);
    this.ddlTransmission = ko.observable("testWebService");

    // Shipper テーブルの各項目
    this.ShipperId = ko.observable("");
    this.CompanyName = ko.observable("");
    this.Phone = ko.observable("");

    // 並び替え対象列
    this.ddlOrderColumnItems = ko.observableArray([
        { displayText: "c1", value: "c1" },
        { displayText: "c2", value: "c2" },
        { displayText: "c3", value: "c3" }
    ]);
    this.ddlOrderColumn = ko.observable("c1");

    // 昇順・降順
    this.ddlOrderSequenceItems = ko.observableArray([
        { displayText: "ASC", value: "A" },
        { displayText: "DESC", value: "D" }
    ]);
    this.ddlOrderSequence = ko.observable("A");

    // 処理結果 (正常系)
    this.Result = ko.observable("");

    // 処理結果 (異常系)
    this.ErrorMessage = ko.observable("");

    // ボタンコマンド (件数取得)
    this.GetCount = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback()
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
                    self.ErrorMessage(data.error);
                }
                else {
                    // 結果格納
                    self.Result(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    // ボタンコマンド (一覧取得（dt）)
    this.GetList_dt = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback()
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
                    self.ErrorMessage(data.error);
                }
                else {
                    // 一旦レコードリストをクリアする
                    self.ClearList();

                    // 結果格納
                    self.dataLists(data);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.GetList_ds = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/SelectDS',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    self.ErrorMessage(data.error);
                }
                else {
                    // 一旦レコードリストをクリアする
                    self.ClearList();

                    // 結果格納
                    self.dataLists(data);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.GetList_dr = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/SelectDR',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    self.ErrorMessage(data.error);
                }
                else {
                    // 一旦レコードリストをクリアする
                    self.ClearList();

                    // 結果格納
                    self.dataLists(data);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.GetList_dsql = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback(),
            OrderColumn: this.ddlOrderColumn(),
            OrderSequence: this.ddlOrderSequence()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/SelectDSQL',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    self.ErrorMessage(data.error);
                }
                else {
                    // 一旦レコードリストをクリアする
                    self.ClearList();

                    // 結果格納
                    self.dataLists(data);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.GetDetail = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback(),
            ShipperId: this.ShipperId()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Select',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    self.ErrorMessage(data.error);
                }
                else {
                    // 結果格納
                    self.ShipperId(data.shipperID);
                    self.CompanyName(data.companyName);
                    self.Phone(data.phone);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.InsertRecord = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback(),
            CompanyName: this.CompanyName(),
            Phone: this.Phone()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Insert',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    self.ErrorMessage(data.error);
                }
                else {
                    // 結果格納
                    self.Result(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.UpdateRecord = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback(),
            ShipperId: this.ShipperId(),
            CompanyName: this.CompanyName(),
            Phone: this.Phone()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Update',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーダイアログを表示する
                    self.ErrorMessage(data.error);
                }
                else {
                    // 結果格納
                    self.Result(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.DeleteRecord = function () {
        var self = this;

        // エラーメッセージをクリアする
        this.ErrorMessage("");

        // パラメタを JSON 形式で纏める
        var param = {
            ddlDap: this.ddlDap(),
            ddlMode1: this.ddlMode1(),
            ddlMode2: this.ddlMode2(),
            ddlExRollback: this.ddlExRollback(),
            ShipperId: this.ShipperId()
        };

        // Ajax でリクエストを送信
        $.ajax({
            type: 'POST',
            url: baseUrl + 'api/Delete',
            data: param,
            dataType: 'json',
            success: function (data, dataType) {
                if (data.error) {
                    // エラーメッセージ格納
                    self.ErrorMessage(data.error);
                }
                else {
                    // 結果格納
                    self.Result(data.message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // エラーメッセージ格納
                self.ErrorMessage(XMLHttpRequest.responseText);
            }
        });
    };

    this.ClearList = function () {
        // レコードリストをクリアする
        this.dataLists([]);
    };
}

// ViewModel を作成
var model = new Sample1ViewModel();

// エラーメッセージが格納されたら、ダイアログを出す
model.ErrorMessage.subscribe(function (newValue) {
    if (newValue !== '') {
        $('#modal_box').modal('show');
    }
});

//ko.applyBindings(new Sample1ViewModel());
ko.applyBindings(model);
