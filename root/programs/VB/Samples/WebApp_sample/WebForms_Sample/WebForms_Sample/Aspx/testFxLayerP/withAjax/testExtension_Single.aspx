<%@ Page Language="VB" MasterPageFile="~/Aspx/Common/testAspNetAjaxExtension_Single.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestFxLayerP.WithAjax.testExtension_Single" Codebehind="testExtension_Single.aspx.vb" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <asp:ScriptManagerProxy ID="ContentsScriptManagerProxy" runat="server"></asp:ScriptManagerProxy>
    
    Content Page（個別） → 3秒待ちます。<br />
    <cc1:WebCustomTextBox ID="TextBox1" runat="server"></cc1:WebCustomTextBox>
    <cc1:WebCustomButton ID="btnButton1" runat="server"  Text="Ajax Button" Width="180px" /><br />
    <cc1:WebCustomTextBox ID="TextBox2" runat="server"></cc1:WebCustomTextBox>
    <cc1:WebCustomButton ID="btnButton2" runat="server"  Text="通常 Button" Width="180px" /><br />
    <br />
    ※ AutoPostBack = True<br />
    <cc1:WebCustomDropDownList ID="ddlDropDownList1" runat="server" AutoPostBack="True">
        <asp:ListItem>あああ</asp:ListItem>
        <asp:ListItem>いいい</asp:ListItem>
        <asp:ListItem>ううう</asp:ListItem>
        <asp:ListItem>えええ</asp:ListItem>
        <asp:ListItem>おおお</asp:ListItem>
    </cc1:WebCustomDropDownList>
    <cc1:WebCustomTextBox ID="TextBox3" runat="server"></cc1:WebCustomTextBox><br />
    <cc1:WebCustomDropDownList ID="ddlDropDownList2" runat="server" AutoPostBack="True">
        <asp:ListItem>あああ</asp:ListItem>
        <asp:ListItem>いいい</asp:ListItem>
        <asp:ListItem>ううう</asp:ListItem>
        <asp:ListItem>えええ</asp:ListItem>
        <asp:ListItem>おおお</asp:ListItem>
    </cc1:WebCustomDropDownList>
    <cc1:WebCustomTextBox ID="TextBox4" runat="server"></cc1:WebCustomTextBox><br />
    <br />
    <cc1:WebCustomButton ID="btnButton3" runat="server"  Text="例外 Button" Width="180px" /><br />    
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
