<%@ Page Language="VB" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.TestFxLayerP.testDialogAtOnLoad" Codebehind="testDialogAtOnLoad.aspx.vb" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <cc1:WebCustomCheckBox ID="CheckBox1" runat="server" AutoPostBack="False" Checked="true" Text="Post Backのテスト" /><br />
    <cc1:WebCustomButton ID="WebCustomButton1" runat="server" Text="Post Backのテスト用" Width="250px" /><br />
    <br />
    <cc1:WebCustomButton ID="btnButton1" runat="server" Text="閉じるButton" Width="250px" /><br />
    <cc1:WebCustomButton ID="btnButton2" runat="server" Text="閉じるButton（NoPostback）" Width="250px" /><br />
    <cc1:WebCustomButton ID="btnButton3" runat="server" Text="閉じるButton（WithAllParent）" Width="250px" /><br />
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
