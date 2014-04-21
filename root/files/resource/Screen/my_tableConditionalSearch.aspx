<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master"
    AutoEventWireup="true" CodeFile="my_tableConditionalSearch.aspx.cs"
    Inherits="my_tableConditionalSearch" Title="my_tableConditionalSearch" %>

<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" runat="Server">
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    データアクセス制御クラス（データプロバイダ）を選択<br />
    <cc1:WebCustomDropDownList ID="ddlDap" runat="server">
        <asp:ListItem Value="SQL">SQL Server / SQL Client</asp:ListItem>
        <asp:ListItem Value="ODP">Oracle / ODP.NET</asp:ListItem>
    </cc1:WebCustomDropDownList>
    <br />
    <table width="100%">
        <tr>
            <td>
                AND = 条件<br />
                <table>
                    <tr>
                        <td>
                            columna
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumna_And" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnb
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnb_And" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnc
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnc_And" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnd
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnd_And" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                </table>
                <br />
                AND Like 条件<br />
                <table>
                    <tr>
                        <td>
                            columna
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumna_And_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnb
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnb_And_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnc
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnc_And_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnd
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnd_And_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                OR = 条件<br />
                <table>
                    <tr>
                        <td>
                            columna
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumna_OR" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnb
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnb_OR" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnc
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnc_OR" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnd
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnd_OR" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                </table>
                <br />
                OR Like 条件<br />
                <table>
                    <tr>
                        <td>
                            columna
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumna_OR_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnb
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnb_OR_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnc
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnc_OR_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            columnd
                        </td>
                        <td>
                            <cc1:WebCustomTextBox ID="txtcolumnd_OR_Like" runat="server">
                            </cc1:WebCustomTextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnSearch" runat="server" Text="上記の条件で検索" />
    <asp:Button ID="btnInsert" runat="server" Text="レコードを追加する。" />
    <asp:GridView ID="gvwGridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="columna, columnb"
        AllowPaging="True" AllowSorting="True" PageSize="30" Width="100%" BorderWidth="1px">
        <HeaderStyle BackColor="darkturquoise" />
        <EditRowStyle BackColor="LightYellow" />
        <Columns>
            <asp:CommandField ShowSelectButton="true" ButtonType="Button" SelectText="選択" />
            <asp:TemplateField SortExpression="columna">
                <ItemTemplate>
                    <asp:TextBox ReadOnly="true" BackColor="lightgray" ID="txtcolumna" runat="server"
                        Text='<%# Bind("columna") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="columnb">
                <ItemTemplate>
                    <asp:TextBox ReadOnly="true" BackColor="lightgray" ID="txtcolumnb" runat="server"
                        Text='<%# Bind("columnb") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="columnc">
                <ItemTemplate>
                    <asp:TextBox ReadOnly="true" ID="txtcolumnc" runat="server" Text='<%# Bind("columnc") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="columnd">
                <ItemTemplate>
                    <asp:TextBox ReadOnly="true" ID="txtcolumnd" runat="server" Text='<%# Bind("columnd") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" EnablePaging="True" TypeName="my_tableTableAdapter"
        SelectCountMethod="SelectCountMethod" SelectMethod="SelectMethod" MaximumRowsParameterName="maximumRows"
        StartRowIndexParameterName="startRowIndex"></asp:ObjectDataSource>
</asp:Content>
