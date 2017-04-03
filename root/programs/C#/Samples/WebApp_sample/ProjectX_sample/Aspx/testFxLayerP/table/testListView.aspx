<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.TestFxLayerP.Table.testListView" EnableEventValidation="false" Codebehind="testListView.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label><br />

    <asp:ListView ID="lvwListView1" runat="server" OnItemEditing="lvwListView1_ItemEditing"
        OnItemCanceling="lvwListView1_ItemCanceling" DataKeyNames="fileid">
        <LayoutTemplate>
            <table id="Table1" runat="server">
                <tr id="Tr1" runat="server">
                    <td id="Td1" runat="server">
                        <table id="itemPlaceholderContainer" runat="server" border="1" style="">
                            <tr id="Tr2" runat="server" style="background-color: Silver">
                                <th id="Th1" runat="server">
                                    Select1
                                </th>
                                <th id="Th6" runat="server">
                                    Select2
                                </th>
                                <th id="Th7" runat="server">
                                    Custom
                                </th>
                                <th id="Th2" runat="server">
                                    File ID
                                </th>
                                <th id="Th8" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnSortByFileName" CommandName="Sort" Text="File Name"
                                        CommandArgument="FileName" />
                                </th>
                                <th id="Th3" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnSortByReadonly" CommandName="Sort" Text="Readonly"
                                        CommandArgument="Readonly" />
                                </th>
                                <th id="Th4" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnSortByFileSize" CommandName="Sort" Text="File Size"
                                        CommandArgument="FileSize" />
                                </th>
                                <th id="Th5" runat="server">
                                    Date
                                </th>
                                <th id="Th9" runat="server">
                                    Edit
                                </th>
                                <th id="Th10" runat="server">
                                    Delete
                                </th>
                                <th id="Th12" runat="server">
                                    ItemCommand
                                </th>
                                <th id="Th11" runat="server">
                                    Dropdown
                                </th>
                            </tr>
                            <tr id="itemPlaceholder" runat="server">
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="Tr3" runat="server">
                    <td id="Td2" runat="server" style="">
                    </td>
                </tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr style="">
                <td>
                    <asp:LinkButton ID="LinkButton1" CommandName="CustomCommand" Text="カスタム" runat="server" />
                </td>
                <td>
                    <cc1:WebCustomRadioButton ID="rbnRadioButton" runat="server" GroupName="radio-grp1" />
                </td>
                <td>
                    <asp:LinkButton CommandName="CustomCommand" Text="カスタム" runat="server" />
                </td>
                <td>
                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("fileid") %>'></asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("filename") %>'></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="cbxCheckBox3" runat="server" Enabled="false" Checked='<%# Bind("readonly") %>'>
                    </asp:CheckBox>
                </td>
                <td>
                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("filesize") %>'></asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("date") %>'></asp:Label>
                </td>
                <td>
                    <asp:LinkButton ID="lbnEdit" runat="server" Text="Edit" CommandName="Edit" />
                </td>
                <td>
                    <asp:LinkButton ID="lbnDelete" runat="server" CommandName="Delete" Text="Delete" />
                </td>
                <td>
                    <asp:LinkButton runat="server" ID="lbnItemCommand" Text="Add To List" CommandName="GetFiedID"
                        CommandArgument='<%#Eval("fileid") %>' />
                </td>
                <td>
                    <asp:DropDownList ID="ddlDropDownList1" runat="server" AutoPostBack="True" Width="150px">
                        <asp:ListItem>あああ</asp:ListItem>
                        <asp:ListItem>いいい</asp:ListItem>
                        <asp:ListItem>ううう</asp:ListItem>
                        <asp:ListItem>えええ</asp:ListItem>
                        <asp:ListItem>おおお</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </ItemTemplate>
        <EditItemTemplate>
            <tr>
                <td>
                </td>
                <td style="float: inherit">
                    <cc1:WebCustomRadioButton ID="rbnRadioButton" runat="server" GroupName="radio-grp1" />
                </td>
                <td style="float: inherit">
                    <asp:LinkButton ID="LinkButton2" CommandName="CustomCommand" Text="カスタム" runat="server" />
                </td>
                <td style="float: inherit">
                    <asp:TextBox ID="txtFileID" runat="server" Text='<%# Bind("fileid") %>'></asp:TextBox>
                </td>
                <td style="float: inherit">
                    <asp:TextBox ID="txtFileName" runat="server" Text='<%# Bind("filename") %>'></asp:TextBox>
                </td>
                <td style="float: inherit">
                    <asp:CheckBox ID="cbxReadonly" runat="server" AutoPostBack="true" Checked='<%# Bind("readonly") %>'>
                    </asp:CheckBox>
                </td>
                <td style="float: inherit">
                    <asp:TextBox ID="txtFileSize" runat="server" Text='<%# Bind("filesize") %>'></asp:TextBox>
                </td>
                <td style="float: inherit">
                    <asp:TextBox ID="txtDate" runat="server" Text='<%# Bind("date") %>'></asp:TextBox>
                </td>
                <td style="float: inherit">
                    <asp:LinkButton ID="lbnUpdate" runat="server" Text="Update" CommandName="Update" />
                    <asp:LinkButton ID="lbnCancel" runat="server" Text="Cancel" CommandName="Cancel" />
                </td>
                <td style="float: inherit">
                    <asp:LinkButton ID="lbnDelete" runat="server" CommandName="Delete" Text="Delete" />
                </td>
                <td style="float: inherit">
                    <asp:DropDownList ID="ddlDropDownList1" runat="server" AutoPostBack="True" Width="150px">
                        <asp:ListItem>あああ</asp:ListItem>
                        <asp:ListItem>いいい</asp:ListItem>
                        <asp:ListItem>ううう</asp:ListItem>
                        <asp:ListItem>えええ</asp:ListItem>
                        <asp:ListItem>おおお</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </EditItemTemplate>
        <SelectedItemTemplate>
        </SelectedItemTemplate>
    </asp:ListView>
    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lvwListView1" PageSize="5">
        <Fields>
            <asp:NumericPagerField />
        </Fields>
    </asp:DataPager>
    <p>
        <asp:Button ID="btnButton1" runat="server" Text="Post Back" />
    </p>
   <asp:Label ID="lblResultOfItemCommand" runat="server"></asp:Label>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
