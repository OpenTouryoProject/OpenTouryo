<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/Master/testBlankScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestFxLayerP.Table.testGridView" EnableEventValidation="false" Codebehind="testGridView.aspx.cs" %>
<%@ Register Assembly="OpenTouryo.CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label><br />

    <asp:GridView ID="gvwGridView1" runat="server" 
        AutoGenerateColumns="False" DataKeyNames="fileid"
        AllowPaging="True" AllowSorting="True" PageSize="5"
        Width="100%" BorderWidth="1px">
        
        <HeaderStyle BackColor="darkturquoise" />
        <EditRowStyle BackColor="LightYellow" />
        
        <Columns>
            <asp:CommandField ShowSelectButton="True" />
                
                <asp:TemplateField>
                    <ItemTemplate>
                        <cc1:WebCustomRadioButton ID="rbnRadioButton" runat="server" GroupName="radio-grp1"/>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:ButtonField CommandName="CustomCommand" Text="カスタム"/>
                
                <asp:TemplateField>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("fileid") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("fileid") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="filename">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("filename") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("filename") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField SortExpression="readonly">
                    <EditItemTemplate>
                        <asp:CheckBox ID="cbxCheckBox3" runat="server" AutoPostBack="true" Checked='<%# Bind("readonly") %>'></asp:CheckBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbxCheckBox3" runat="server" Enabled="false" Checked='<%# Bind("readonly") %>'></asp:CheckBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="filesize">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("filesize") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("filesize") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("date") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("date") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:CommandField ShowEditButton="True" />
                
                <%-- 削除列 (TemplatedFieldを用い、削除時に確認Messageを表示するサンプル) --%>
                <asp:TemplateField ShowHeader="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Delete" Text="削除" OnClientClick="return confirm('選択されたレコードを削除してよろしいですか？');" />
                    </ItemTemplate>
                </asp:TemplateField>
                
                <%-- 削除列 (CommandFieldを用いるサンプル。削除確認Messageは表示されない) --%>
                <%--
                <asp:CommandField ShowDeleteButton="True" />
                --%>
                
                <asp:TemplateField ShowHeader="false">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlDropDownList1" runat="server" AutoPostBack="True" Width="150px">
                            <asp:ListItem>あああ</asp:ListItem>
                            <asp:ListItem>いいい</asp:ListItem>
                            <asp:ListItem>ううう</asp:ListItem>
                            <asp:ListItem>えええ</asp:ListItem>
                            <asp:ListItem>おおお</asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                
            </Columns>
        </asp:GridView>
        
        <asp:Button ID="btnButton1" runat="server" Text="Post Back"/>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
