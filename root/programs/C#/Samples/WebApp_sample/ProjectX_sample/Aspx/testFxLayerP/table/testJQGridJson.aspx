<%@ Page Language="C#" AutoEventWireup="true" CodeFile="testJQGridJson.aspx.cs" Inherits="Aspx_testFxLayerP_table_testJQGridJson" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../../Content/themes/base/all.css" rel="stylesheet" type="text/css" />
    <link href="../../../Content/jquery.jqGrid/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <script src="../../../Scripts/jquery-2.1.3.min.js" type="text/javascript"></script>
    <script src="../../../Scripts/jquery-ui-1.11.2.min.js" type="text/javascript"></script>
    <script src="../../../Scripts/i18n/grid.locale-da.js" type="text/javascript"></script>
    <script src="../../../Scripts/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#list').jqGrid({
                url: '/ProjectX_sample/WebService/JSONService.svc/GetProductData',
                datatype: 'json',
                colNames: ['ProductID', 'ProductName', 'SupplierID', 'CategoryID', 'QuantityPerUnit', 'UnitPrice', 'UnitsInStock', 'UnitsOnOrder', 'ReorderLevel', 'Discontinued'],
                colModel: [
                    { name: 'ProductID', width: 100 },
                    { name: 'ProductName', sortable: false, width: 150 },
                    { name: 'SupplierID', sortable: false, width: 150 },
                    { name: 'CategoryID', sortable: false, width: 150 },
                    { name: 'QuantityPerUnit', sortable: false, width: 150 },
                    { name: 'UnitPrice', sortable: false, width: 50 },
                    { name: 'UnitsInStock', sortable: false, width: 150 },
                    { name: 'UnitsOnOrder', sortable: false, width: 150 },
                    { name: 'ReorderLevel', sortable: false, width: 100 },
                    { name: 'Discontinued', sortable: false, width: 150 }

                ],
                rowNum: 1000,
                sortname: 'ProductID',
                sortorder: 'desc',
                mtype: 'GET',
                loadonce: false,
                caption: 'JSON Sample (ページングなし)'
            });

            $('#list2').jqGrid({
                datatype: 'json',
                url: '/ProjectX_sample/WebService/JSONService.svc/GetProductData',
                colNames: ['ProductID', 'ProductName', 'SupplierID', 'CategoryID', 'QuantityPerUnit', 'UnitPrice', 'UnitsInStock', 'UnitsOnOrder', 'ReorderLevel', 'Discontinued'],
                colModel: [
                    { name: 'ProductID', width: 100 },
                    { name: 'ProductName', sortable: false, width: 150 },
                    { name: 'SupplierID', sortable: false, width: 150 },
                    { name: 'CategoryID', sortable: false, width: 150 },
                    { name: 'QuantityPerUnit', sortable: false, width: 150 },
                    { name: 'UnitPrice', sortable: false, width: 50 },
                    { name: 'UnitsInStock', sortable: false, width: 150 },
                    { name: 'UnitsOnOrder', sortable: false, width: 150 },
                    { name: 'ReorderLevel', sortable: false, width: 100 },
                    { name: 'Discontinued', sortable: false, width: 150 }

                ],
                pager: $('#pager'),
                rowNum: 29,
                sortname: 'ProductID',
                viewrecords: true,
                sortorder: 'desc',
                loadonce: false,
                beforeProcessing: function (data, status, xhr) {  // レスポンスの受信時に呼び出されるイベント
                    // サーバからエラーが返ってきた場合にダイアログを表示し、以降の処理を中止する
                    // エラー時には、サーバからuserdataに情報を格納する
                    if (data.userdata == "error") {
                        alert("最大ページ以上を指定しています");
                        return false;
                    }
                },
                caption: 'JSON Sample (ページングあり)'
            });
            $('#list2').navGrid('#pager', { del: false, add: false, edit: false, search: true, refresh: true });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="list">
        </table>
        <br />
        <table id="list2">
        </table>
        <div id="pager">
        </div>
    </div>
    </form>
</body>
</html>
