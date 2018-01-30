<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/Master/testBlankScreenNoJs.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestFxLayerP.Table.testJQGridJson" EnableEventValidation="false" Codebehind="testJQGridJson.aspx.cs" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.12.1/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/jqgrid/4.6.0/css/ui.jqgrid.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" runat="Server">
    <div>
        <table id="list">
        </table>
        <br />
        <table id="list2">
        </table>
        <div id="pager">
        </div>
    </div>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" runat="Server">

    <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.3.min.js" type="text/javascript"></script>
    <script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.11.2/jquery-ui.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jqgrid/4.6.0/js/i18n/grid.locale-ja.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jqgrid/4.6.0/js/jquery.jqGrid.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(function () {
            $('#list').jqGrid({
                url: '<% = ResolveUrl("~/WebService/JSONService.svc/GetProductData")%>',
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
                rowNum: 9999,
                sortname: 'ProductID',
                sortorder: 'desc',
                mtype: 'GET',
                loadonce: false,
                caption: 'JSON Sample (ページングなし)'
            });

            $('#list2').jqGrid({
                datatype: 'json',
                url: '<%=ResolveUrl("~/WebService/JSONService.svc/GetProductData")%>',
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
                rowNum: 30,
                sortname: 'ProductID',
                viewrecords: true,
                sortorder: 'desc',
                loadonce: false,
                beforeProcessing: function (data, status, xhr) {  // レスポンスの受信時に呼び出されるイベント
                    // サーバからエラーが返ってきた場合にDialogを表示し、以降の処理を中止する
                    // Check current page number will not be greater than maximum page
                    if ((data.page) > (data.total)) {
                        alert("You will not specify a greater than value to the maximum page");
                        return false;
                    }
                },
                caption: 'JSON Sample (ページングあり)'
            });
            $('#list2').navGrid('#pager', { del: false, add: false, edit: false, search: true, refresh: true });
        });
    </script>
</asp:Content>
