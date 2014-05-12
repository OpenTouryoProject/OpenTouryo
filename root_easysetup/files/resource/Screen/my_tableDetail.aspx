<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" CodeFile="my_tableDetail.aspx.cs" Inherits="my_tableDetail" Title="my_tableDetail" %>
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
            <td>columna</td>
            <td><cc1:WebCustomTextBox ID="txtcolumna" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>columnb</td>
            <td><cc1:WebCustomTextBox ID="txtcolumnb" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>columnc</td>
            <td><cc1:WebCustomTextBox ID="txtcolumnc" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
        <tr>
            <td>columnd</td>
            <td><cc1:WebCustomTextBox ID="txtcolumnd" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
    </table>
    
    <asp:Button ID="btnEdit" runat="server" Text="編集を可能にする" />
    <asp:Button ID="btnUpdate" runat="server" Text="編集結果で更新する" />
    <asp:Button ID="btnDelete" runat="server" Text="上記レコードを削除する" />
    <asp:Button ID="btnInsert" runat="server" Text="編集結果で追加する" /><br/>
    結果：<asp:Label ID="lblResult" runat="server" ></asp:Label>
    
</asp:Content>

