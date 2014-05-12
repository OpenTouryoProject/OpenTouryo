﻿<%@ Page Language="_CodebehindLanguage_" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" CodeFile="_TableName_SearchAndUpdate.aspx._ClassTemplateFileExtension_" Inherits="_TableName_SearchAndUpdate" Title="_TableName_SearchAndUpdate" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    
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
                    <!-- ControlComment:LoopStart-PKColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_ColumnName__And" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-PKColumn -->
                    <!-- ControlComment:LoopStart-ElseColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_ColumnName__And" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-ElseColumn -->
                </table>
                <br/>
                AND Like 条件<br/>
                <table>                    
                    <!-- ControlComment:LoopStart-PKColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_ColumnName__And_Like" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-PKColumn -->
                    <!-- ControlComment:LoopStart-ElseColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_ColumnName__And_Like" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-ElseColumn -->
                </table>
            </td>
            <td>
                OR = 条件<br/>
                <table>
                    <!-- ControlComment:LoopStart-PKColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><asp:TextBox ID="txt_ColumnName__OR" runat="server"></asp:TextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-PKColumn -->
                    <!-- ControlComment:LoopStart-ElseColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_ColumnName__OR" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-ElseColumn -->
                </table>
                <br/>
                OR Like 条件<br/>
                <table>
                    <!-- ControlComment:LoopStart-PKColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_ColumnName__OR_Like" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-PKColumn -->
                    <!-- ControlComment:LoopStart-ElseColumn -->
                    <tr>
                        <td>_ColumnName_</td>
                        <td><cc1:WebCustomTextBox ID="txt_ColumnName__OR_Like" runat="server"></cc1:WebCustomTextBox></td>
                    </tr>
                    <!-- ControlComment:LoopEnd-ElseColumn -->
                </table>
            </td>
        </tr>
    </table>

    <asp:Button ID="btnSearch" runat="server" Text="上記の条件で検索" />
    <asp:Button ID="btnInsert" runat="server" Text="レコードを追加する。" />
    <asp:Button ID="btnBatUpd" runat="server" Text="下記の結果セットをバッチ更新する" />
        
    <asp:GridView ID="gvwGridView1" runat="server" 
        AutoGenerateColumns="False" DataKeyNames="_PKFirstColumn_"
        AllowPaging="True" AllowSorting="True" PageSize="30"
        Width="100%" BorderWidth="1px">
        
        <HeaderStyle BackColor="darkturquoise" />
        <EditRowStyle BackColor="LightYellow" />
       	    
        <Columns>
            <asp:ButtonField CommandName="Delete" Text="削除"/>
            <asp:ButtonField CommandName="Update" Text="更新"/>

            <!-- ControlComment:LoopStart-PKColumn -->
            <asp:TemplateField SortExpression="_ColumnName_">
                <ItemTemplate>
                    <asp:TextBox ReadOnly="true" BackColor="lightgray" ID="txt_ColumnName_" runat="server" Text='<%# Bind("_ColumnName_") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <!-- ControlComment:LoopEnd-PKColumn -->
            <!-- ControlComment:LoopStart-ElseColumn -->
            <asp:TemplateField SortExpression="_ColumnName_">
                <ItemTemplate>
                    <asp:TextBox ID="txt_ColumnName_" runat="server" Text='<%# Bind("_ColumnName_") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <!-- ControlComment:LoopEnd-ElseColumn -->

        </Columns>
            </asp:GridView>

    <asp:ObjectDataSource ID="ObjectDataSource1"
        runat="server" EnablePaging="True"
        TypeName="_TableName_TableAdapter" 
        SelectCountMethod="SelectCountMethod"
        SelectMethod="SelectMethod"
        MaximumRowsParameterName="maximumRows"
        StartRowIndexParameterName="startRowIndex">
    </asp:ObjectDataSource>

 </asp:Content>
