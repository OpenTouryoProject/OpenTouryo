<%@ Page Language="_CodebehindLanguage_" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" CodeFile="_JoinTableName__Screen_Detail.aspx._ClassTemplateFileExtension_" Inherits="_JoinTableName__Screen_Detail" Title="_JoinTableName__Screen_Detail" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">

	Details of Record<br/>
    <table>
	<!-- ControlComment:LoopStart-PKColumn -->
        <tr>
            <td>_ColumnName_</td>
            <td><cc1:WebCustomTextBox ID="txt_JoinTextboxColumnName_" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
	<!-- ControlComment:LoopEnd-PKColumn -->
    <!-- ControlComment:LoopStart-ElseColumn -->
        <tr>
            <td>_ColumnName_</td>
            <td><cc1:WebCustomTextBox ID="txt_JoinTextboxColumnName_" runat="server"></cc1:WebCustomTextBox></td>
        </tr>       
    <!-- ControlComment:LoopEnd-ElseColumn -->
    </table>
    
    <asp:Button ID="btnEdit" runat="server" Text="Edit Record" />
    <asp:Button ID="btnUpdate" runat="server" Text="Update Record" />
    <asp:Button ID="btnDelete" runat="server" Text="Delete Record" />
    <asp:Button ID="btnInsert" runat="server" Text="Insert New Record" /><br/>
     Result ：<br />
     <!--  ControlComment:LoopStart-JoinTables-->
   <asp:Label ID="lblResult_TableName_" runat="server" ></asp:Label> <br />
    <!--  ControlComment:LoopEnd-JoinTables-->
</asp:Content>

