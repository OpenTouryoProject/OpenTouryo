<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" CodeFile="testListView.aspx.cs" Inherits="Aspx_testFxLayerP_table_testListView" Title="Untitled Page" EnableEventValidation="false" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">

    <asp:ListView ID="ListView1" runat="server" 
        onpagepropertieschanged="ListView1_PagePropertiesChanged" >
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

</asp:Content>