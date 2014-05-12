<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" CodeFile="ProductsDetail.aspx.cs" Inherits="ProductsDetail" Title="ProductsDetail" %>
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
    
	詳細表示<br/>
    <table>
        <tr>
            <td>ProductID</td>
            <td><cc1:WebCustomTextBox ID="txtProductID" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>ProductName</td>
            <td><cc1:WebCustomTextBox ID="txtProductName" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>SupplierID</td>
            <td><cc1:WebCustomTextBox ID="txtSupplierID" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>CategoryID</td>
            <td><cc1:WebCustomTextBox ID="txtCategoryID" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>QuantityPerUnit</td>
            <td><cc1:WebCustomTextBox ID="txtQuantityPerUnit" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>UnitPrice</td>
            <td><cc1:WebCustomTextBox ID="txtUnitPrice" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>UnitsInStock</td>
            <td><cc1:WebCustomTextBox ID="txtUnitsInStock" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>UnitsOnOrder</td>
            <td><cc1:WebCustomTextBox ID="txtUnitsOnOrder" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>ReorderLevel</td>
            <td><cc1:WebCustomTextBox ID="txtReorderLevel" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>Discontinued</td>
            <td><cc1:WebCustomTextBox ID="txtDiscontinued" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
    </table>
    
    <asp:Button ID="btnEdit" runat="server" Text="編集を可能にする" />
    <asp:Button ID="btnUpdate" runat="server" Text="編集結果で更新する" />
    <asp:Button ID="btnDelete" runat="server" Text="上記レコードを削除する" />
    <asp:Button ID="btnInsert" runat="server" Text="編集結果で追加する" /><br/>
    結果：<asp:Label ID="lblResult" runat="server" ></asp:Label>
    
</asp:Content>

