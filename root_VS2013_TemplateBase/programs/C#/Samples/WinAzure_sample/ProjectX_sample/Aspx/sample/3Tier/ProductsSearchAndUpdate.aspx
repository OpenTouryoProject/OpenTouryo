<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="Aspx_sample_3Tier_ProductsSearchAndUpdate" Title="ProductsSearchAndUpdate" Codebehind="ProductsSearchAndUpdate.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    
    <!--
        [ProductID] [int] IDENTITY(1,1) NOT NULL,
	    [ProductName] [nvarchar](40) NOT NULL,
	    [SupplierID] [int] NULL,
	    [CategoryID] [int] NULL,
	    [QuantityPerUnit] [nvarchar](20) NULL,
	    [UnitPrice] [money] NULL,
	    [UnitsInStock] [smallint] NULL,
	    [UnitsOnOrder] [smallint] NULL,
	    [ReorderLevel] [smallint] NULL,
	    [Discontinued] [bit] NOT NULL,
	-->
	
	データアクセス制御クラス（データプロバイダ）を選択<br />
    <cc1:WebCustomDropDownList ID="ddlDap" runat="server">
        <asp:ListItem Value="SQL">SQL Server / SQL Client</asp:ListItem>
        <asp:ListItem Value="ODP">Oracle / ODP.NET</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    
    <table width="100%">
        <tr>
            <td>
                AND = 条件<br/>
                <table>
                    <tr>
                        <td>ProductID</td>
                        <td><asp:TextBox ID="txtProductID_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ProductName</td>
                        <td><asp:TextBox ID="txtProductName_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>SupplierID</td>
                        <td><asp:TextBox ID="txtSupplierID_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>CategoryID</td>
                        <td><asp:TextBox ID="txtCategoryID_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>QuantityPerUnit</td>
                        <td><asp:TextBox ID="txtQuantityPerUnit_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitPrice</td>
                        <td><asp:TextBox ID="txtUnitPrice_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsInStock</td>
                        <td><asp:TextBox ID="txtUnitsInStock_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsOnOrder</td>
                        <td><asp:TextBox ID="txtUnitsOnOrder_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ReorderLevel</td>
                        <td><asp:TextBox ID="txtReorderLevel_And" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Discontinued</td>
                        <td><asp:TextBox ID="txtDiscontinued_And" runat="server"></asp:TextBox></td>
                    </tr>
                </table>
                <br/>
                AND Like 条件<br/>
                <table>
                    <tr>
                        <td>ProductID</td>
                        <td><asp:TextBox ID="txtProductID_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ProductName</td>
                        <td><asp:TextBox ID="txtProductName_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>SupplierID</td>
                        <td><asp:TextBox ID="txtSupplierID_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>CategoryID</td>
                        <td><asp:TextBox ID="txtCategoryID_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>QuantityPerUnit</td>
                        <td><asp:TextBox ID="txtQuantityPerUnit_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitPrice</td>
                        <td><asp:TextBox ID="txtUnitPrice_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsInStock</td>
                        <td><asp:TextBox ID="txtUnitsInStock_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsOnOrder</td>
                        <td><asp:TextBox ID="txtUnitsOnOrder_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ReorderLevel</td>
                        <td><asp:TextBox ID="txtReorderLevel_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Discontinued</td>
                        <td><asp:TextBox ID="txtDiscontinued_And_Like" runat="server"></asp:TextBox></td>
                    </tr>
                </table>
            </td>
            <td>
                OR = 条件<br/>
                <table>
                    <tr>
                        <td>ProductID</td>
                        <td><asp:TextBox ID="txtProductID_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ProductName</td>
                        <td><asp:TextBox ID="txtProductName_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>SupplierID</td>
                        <td><asp:TextBox ID="txtSupplierID_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>CategoryID</td>
                        <td><asp:TextBox ID="txtCategoryID_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>QuantityPerUnit</td>
                        <td><asp:TextBox ID="txtQuantityPerUnit_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitPrice</td>
                        <td><asp:TextBox ID="txtUnitPrice_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsInStock</td>
                        <td><asp:TextBox ID="txtUnitsInStock_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsOnOrder</td>
                        <td><asp:TextBox ID="txtUnitsOnOrder_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ReorderLevel</td>
                        <td><asp:TextBox ID="txtReorderLevel_OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Discontinued</td>
                        <td><asp:TextBox ID="txtDiscontinued_OR" runat="server"></asp:TextBox></td>
                    </tr>
                </table>
                <br/>
                OR Like 条件<br/>
                <table>
                    <tr>
                        <td>ProductID</td>
                        <td><asp:TextBox ID="txtProductID_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ProductName</td>
                        <td><asp:TextBox ID="txtProductName_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>SupplierID</td>
                        <td><asp:TextBox ID="txtSupplierID_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>CategoryID</td>
                        <td><asp:TextBox ID="txtCategoryID_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>QuantityPerUnit</td>
                        <td><asp:TextBox ID="txtQuantityPerUnit_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitPrice</td>
                        <td><asp:TextBox ID="txtUnitPrice_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsInStock</td>
                        <td><asp:TextBox ID="txtUnitsInStock_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>UnitsOnOrder</td>
                        <td><asp:TextBox ID="txtUnitsOnOrder_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>ReorderLevel</td>
                        <td><asp:TextBox ID="txtReorderLevel_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Discontinued</td>
                        <td><asp:TextBox ID="txtDiscontinued_OR_Like" runat="server"></asp:TextBox></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    
    <asp:Button ID="btnSearch" runat="server" Text="上記の条件で検索" />
    <asp:Button ID="btnInsert" runat="server" Text="新しいレコードを追加する。" />
    <asp:Button ID="btnBatUpd" runat="server" Text="下記の結果セットをバッチ更新する" />
    
    <asp:GridView ID="gvwGridView1" runat="server" 
        AutoGenerateColumns="False" DataKeyNames="ProductID"
        AllowPaging="True" AllowSorting="True" PageSize="30"
        Width="100%" BorderWidth="1px">
        
        <HeaderStyle BackColor="darkturquoise" />
        <EditRowStyle BackColor="LightYellow" />
        	    
        <Columns>
            <asp:ButtonField CommandName="Delete" Text="削除"/>
            <asp:ButtonField CommandName="Update" Text="更新"/>
            
            <asp:TemplateField SortExpression="ProductID">
                <ItemTemplate>
                    <asp:TextBox ReadOnly="true" BackColor="lightgray" ID="txtProductID" runat="server" Text='<%# Bind("ProductID") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="ProductName">
                <ItemTemplate>
                    <asp:TextBox ID="txtProductName" runat="server" Text='<%# Bind("ProductName") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="SupplierID">
                <ItemTemplate>
                    <cc1:WebCustomDropDownList ID="ddlSupplierID" runat="server" AutoPostBack="false"
                        DataSource="<%# this.ddldsdt_Suppliers %>" DataValueField="value" DataTextField="text"
                        SelectedValue='<%# Bind(Container.DataItem, "SupplierID") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="CategoryID">
                <ItemTemplate>
                    <cc1:WebCustomDropDownList ID="ddlCategoryID" runat="server" AutoPostBack="false"
                        DataSource="<%# this.ddldsdt_Categories %>" DataValueField="value" DataTextField="text"
                        SelectedValue='<%# Bind(Container.DataItem, "CategoryID") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="QuantityPerUnit">
                <ItemTemplate>
                    <asp:TextBox ID="txtQuantityPerUnit" runat="server" Text='<%# Bind("QuantityPerUnit") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="UnitPrice">
                <ItemTemplate>
                    <asp:TextBox ID="txtUnitPrice" runat="server" Text='<%# Bind("UnitPrice") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="UnitsInStock">
                <ItemTemplate>
                    <asp:TextBox ID="txtUnitsInStock" runat="server" Text='<%# Bind("UnitsInStock") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="UnitsOnOrder">
                <ItemTemplate>
                    <asp:TextBox ID="txtUnitsOnOrder" runat="server" Text='<%# Bind("UnitsOnOrder") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="ReorderLevel">
                <ItemTemplate>
                    <asp:TextBox ID="txtReorderLevel" runat="server" Text='<%# Bind("ReorderLevel") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="Discontinued">
                <ItemTemplate>
                    <asp:TextBox ID="txtDiscontinued" runat="server" Text='<%# Bind("Discontinued") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        
    </asp:GridView>
    
    <asp:ObjectDataSource ID="ObjectDataSource1"
        runat="server" EnablePaging="True"
        TypeName="ProductsTableAdapter" 
        SelectCountMethod="SelectCountMethod"
        SelectMethod="SelectMethod"
        MaximumRowsParameterName="maximumRows"
        StartRowIndexParameterName="startRowIndex">
    </asp:ObjectDataSource>
    
</asp:Content>

