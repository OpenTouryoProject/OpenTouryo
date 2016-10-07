<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.testFxLayerP.table.testRepeater" Title="Untitled Page" EnableEventValidation="false" Codebehind="testRepeater.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <asp:Repeater id="rptRepeater1" runat="server">
        <HeaderTemplate>
            <table border="1" style="width:100%;">
                <tr style="background-color: darkturquoise">
                    <th><% = this.HeaderInfo["col0"] %></th>
                    <th><% = this.HeaderInfo["col1"] %></th>
                    <th><% = this.HeaderInfo["col2"] %></th>
                    <th><% = this.HeaderInfo["col3"] %></th>
                    <th><% = this.HeaderInfo["col4"] %></th>
                    <th>ボタン</th>
                </tr>
        </HeaderTemplate>
        
        <ItemTemplate>
            <tr>
                <td style="text-align: center">
                    <cc1:WebCustomRadioButton ID="rbnRadioButton" runat="server" GroupName="radio-grp1"/>
                </td>
                <td style="text-align: center">
                    <%# DataBinder.Eval(Container.DataItem, "fileid")%>
                </td>
                <td>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "textbox") %>'/>
                </td>
                <td>
                    <asp:CheckBox ID="cbxCheckBox1" runat="server" AutoPostBack="true"
                     Checked='<%# DataBinder.Eval(Container.DataItem, "checkbox") %>'/>
                </td>
                <td>
                    <asp:DropDownList ID="ddlDropDownList1" runat="server" AutoPostBack="true"
                        DataSource="<%# this.DropDownListDataSource %>" DataValueField="value" DataTextField="text"
                        SelectedValue='<%# DataBinder.Eval(Container.DataItem, "dropdownlist") %>'/>
                </td>
                <td>
                    <asp:Button ID="command1" runat="server" Text="コマンド" CommandName="<%# Container.ItemIndex%>"/>
                    <asp:LinkButton ID="command2" runat="server" Text="コマンド" CommandName="<%# Container.ItemIndex%>"/>
                </td>
            </tr>
        </ItemTemplate>
                    
        <FooterTemplate>
            </table>
        </FooterTemplate>
                    
    </asp:Repeater>
                
    <asp:Button ID="btnButton1" runat="server" Text="ポストバック"/>
    <asp:Button ID="btnButton2" runat="server" Text="変更の反映"/>
</asp:Content>