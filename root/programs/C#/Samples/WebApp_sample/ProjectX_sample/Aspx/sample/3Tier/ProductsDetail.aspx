<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.sample._3Tier.ProductsDetail" Title="ProductsDetail" Codebehind="ProductsDetail.aspx.cs" %>
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
            <td><asp:TextBox ID="txtProductID" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>ProductName</td>
            <td><asp:TextBox ID="txtProductName" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>SupplierID</td>
            <td><cc1:WebCustomDropDownList ID="ddlSupplierID" runat="server" AutoPostBack="false"/></td>
        </tr>
        <tr>
            <td>CategoryID</td>
            <td><cc1:WebCustomDropDownList ID="ddlCategoryID" runat="server" AutoPostBack="false"/></td>
        </tr>
        <tr>
            <td>QuantityPerUnit</td>
            <td><asp:TextBox ID="txtQuantityPerUnit" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>UnitPrice</td>
            <td><asp:TextBox ID="txtUnitPrice" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>UnitsInStock</td>
            <td><asp:TextBox ID="txtUnitsInStock" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>UnitsOnOrder</td>
            <td><asp:TextBox ID="txtUnitsOnOrder" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>ReorderLevel</td>
            <td><asp:TextBox ID="txtReorderLevel" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Discontinued</td>
            <td><asp:TextBox ID="txtDiscontinued" runat="server"></asp:TextBox></td>
        </tr>
    </table>
    
    <asp:Button ID="btnEdit" runat="server" Text="編集を可能にする" />
    <asp:Button ID="btnUpdate" runat="server" Text="編集結果で更新する" />
    <asp:Button ID="btnDelete" runat="server" Text="上記レコードを削除する" />
    <asp:Button ID="btnInsert" runat="server" Text="編集結果で追加する" /><br/>
    結果：<asp:Label ID="lblResult" runat="server" ></asp:Label>
    
</asp:Content>

