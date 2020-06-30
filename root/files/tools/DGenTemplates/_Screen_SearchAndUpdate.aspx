<%@ Page Language="_CodebehindLanguage_" MasterPageFile="~/Aspx/Common/Master/testBlankScreen.master" AutoEventWireup="true" CodeFile="_JoinTableName__Screen_SearchAndUpdate.aspx._ClassTemplateFileExtension_" Inherits="_JoinTableName__Screen_SearchAndUpdate" Title="_JoinTableName__Screen_SearchAndUpdate" %>
<%@ Register Assembly="OpenTouryo.CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
       <table style="width:100%;">
        <tr>
            <td>
                <table>
                    <!-- ControlComment:LoopStart-PKColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_JoinTextboxColumnName__And" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-PKColumn -->
                    <!-- ControlComment:LoopStart-ElseColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_JoinTextboxColumnName__And" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-ElseColumn -->
                </table>
                <br/>
            </td>
        </tr>
    </table>

    <asp:Button ID="btnSearch" runat="server" Text="Search Record" />
    <asp:Button ID="btnBatUpd" runat="server" Text="Update the Result Set Using Batch Update" />

    <asp:GridView ID="gvwGridView1" runat="server" 
        AutoGenerateColumns="False" DataKeyNames="_PKFirstColumn_"
        AllowPaging="True" AllowSorting="True" PageSize="30"
        Width="100%" BorderWidth="1px">
        
        <HeaderStyle BackColor="darkturquoise" />
        <EditRowStyle BackColor="LightYellow" />
        
        <Columns>
            <asp:ButtonField CommandName="Delete" Text="Delete"/>
            <asp:ButtonField CommandName="Update" Text="Update"/>
            <!-- ControlComment:LoopStart-PKColumn -->
            <asp:TemplateField SortExpression="_JoinColumnName_">
                <ItemTemplate>
                   <cc1:WebCustomTextBox ReadOnly="true" BackColor="lightgray" ID="txt_JoinTextboxColumnName_" runat="server" Text='<%#DataBinder.GetPropertyValue(Container.DataItem, "_JoinColumnName_") %>'></cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <!-- ControlComment:LoopEnd-PKColumn -->
            <!-- ControlComment:LoopStart-ElseColumn -->
            <asp:TemplateField SortExpression="_JoinColumnName_">
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="txt_JoinTextboxColumnName_" runat="server" Text='<%#DataBinder.GetPropertyValue(Container.DataItem, "_JoinColumnName_") %>'></cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <!-- ControlComment:LoopEnd-ElseColumn -->
        </Columns>
        </asp:GridView>

</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>