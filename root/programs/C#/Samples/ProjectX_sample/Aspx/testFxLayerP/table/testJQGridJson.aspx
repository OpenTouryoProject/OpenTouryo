<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Aspx/Common/testBlankScreen.master"
    CodeFile="testJQGridJson.aspx.cs" Inherits="Aspx_testFxLayerP_table_testJQGridJson" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder_A">
    <table id="jQGridDemo">
    </table>
    <div id="jQGridDemoPager">
    </div>
    <script type="text/javascript">
        jQuery("#jQGridDemo").jqGrid({
            url: 'JQGridHandler.ashx',
            datatype: "json",
            colNames: ['id', 'name', 'email', 'phone'],
            colModel: [
                    { name: 'id' },
                    { name: 'name', sortable: false },
                    { name: 'email', sortable: false },
                    { name: 'phone', sortable: false },

            ],
            rowNum: 10,
            mtype: 'GET',
            loadonce: true,
            rowList: [10, 20, 30],
            sortname: 'id',
            viewrecords: true,
            sortorder: 'desc',
            gridview: true,
            caption: "List Details",
            editurl: 'JQGridHandler.ashx'
        });

        $('#jQGridDemo').jqGrid('navGrid', '#jQGridDemoPager',
                   {
                       // Closes the popup on pressing escape key
                       closeOnEscape: true
                   },

                   {
                       // SEARCH
                       closeOnEscape: true
                   }
            );
    </script>
</asp:Content>
