<%@ Page Language="_CodebehindLanguage_" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" CodeFile="_TableName_Detail.aspx._ClassTemplateFileExtension_" Inherits="_TableName_Detail" Title="_TableName_Detail" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">

	データアクセス制御クラス（データプロバイダ）を選択<br />
    <cc1:WebCustomDropDownList ID="ddlDap" runat="server">
        <asp:ListItem Value="SQL">SQL Server / SQL Client</asp:ListItem>
        <asp:ListItem Value="ODP">Oracle / ODP.NET</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    
	詳細表示<br/>
    <table>
	<!-- ControlComment:LoopStart-PKColumn -->
        <tr>
            <td>_ColumnName_</td>
            <td><cc1:WebCustomTextBox ID="txt_ColumnName_" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
	<!-- ControlComment:LoopEnd-PKColumn -->
    <!-- ControlComment:LoopStart-ElseColumn -->
        <tr>
            <td>_ColumnName_</td>
            <td><cc1:WebCustomTextBox ID="txt_ColumnName_" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
    <!-- ControlComment:LoopEnd-ElseColumn -->
    </table>
    
    <asp:Button ID="btnEdit" runat="server" Text="編集を可能にする" />
    <asp:Button ID="btnUpdate" runat="server" Text="編集結果で更新する" />
    <asp:Button ID="btnDelete" runat="server" Text="上記レコードを削除する" />
    <asp:Button ID="btnInsert" runat="server" Text="編集結果で追加する" /><br/>
    結果：<asp:Label ID="lblResult" runat="server" ></asp:Label>
    
</asp:Content>

