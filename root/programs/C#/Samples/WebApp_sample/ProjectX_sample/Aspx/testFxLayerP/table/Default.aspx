<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Aspx_testFxLayerP_table_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ListView ID="ListView1" runat="server" 
            onpagepropertieschanged="ListView1_PagePropertiesChanged">
        <LayoutTemplate>
            <table>
                <tr runat="server" ID="itemPlaceholder"></tr>
            </table>
            
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("fileid")%></td>
                <td><%# Eval("filename")%></td>
                <td><%# Eval("readonly")%></td>
                <td><%# Eval("filesize")%></td>
                <td><%# Eval("date")%></td>
            </tr>
        </ItemTemplate>
    </asp:ListView>

    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListView1" PageSize="3">
        <Fields>
            <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="True" />
            <asp:NumericPagerField />
            <asp:NextPreviousPagerField ButtonType="Button" ShowLastPageButton="True" ShowNextPageButton="True" ShowPreviousPageButton="False" />
        </Fields>
    </asp:DataPager>
    </div>
    </form>
</body>
</html>
