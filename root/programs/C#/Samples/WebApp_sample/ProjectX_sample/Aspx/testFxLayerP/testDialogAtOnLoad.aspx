<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.TestFxLayerP.testDialogAtOnLoad" Title="Untitled Page" Codebehind="testDialogAtOnLoad.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeader" ContentPlaceHolderID="cphHeader" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <cc1:WebCustomCheckBox ID="CheckBox1" runat="server" AutoPostBack="False" Checked="true" Text="ポストバックのテスト" /><br />
    <cc1:WebCustomButton ID="WebCustomButton1" runat="server" Text="ポストバックのテスト用" Width="250px" /><br />
    <br />
    <cc1:WebCustomButton ID="btnButton1" runat="server" Text="閉じるボタン" Width="250px" /><br />
    <cc1:WebCustomButton ID="btnButton2" runat="server" Text="閉じるボタン（NoPostback）" Width="250px" /><br />
    <cc1:WebCustomButton ID="btnButton3" runat="server" Text="閉じるボタン（WithAllParent）" Width="250px" /><br />
</asp:Content>

<asp:Content ID="cphFooter" ContentPlaceHolderID="cphFooter" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
