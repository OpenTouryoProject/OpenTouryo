<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testAspNetAjaxExtension_Single.master" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.testFxLayerP.withAjax.testExtension_Single" Title="Untitled Page" Codebehind="testExtension_Single.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2016 Hitachi Solutions,Ltd. -->
    <asp:ScriptManagerProxy ID="ContentsScriptManagerProxy" runat="server"></asp:ScriptManagerProxy>
    
    コンテンツ ページ（個別）　→　３秒待ちます。<br />
    <cc1:WebCustomTextBox ID="TextBox1" runat="server"></cc1:WebCustomTextBox>
    <cc1:WebCustomButton ID="btnButton1" runat="server"  Text="Ajaxボタン" Width="180px" /><br />
    <cc1:WebCustomTextBox ID="TextBox2" runat="server"></cc1:WebCustomTextBox>
    <cc1:WebCustomButton ID="btnButton2" runat="server"  Text="通常ボタン" Width="180px" /><br />
    <br />
    ※ ＡｕｔｏＰｏｓｔＢａｃｋ ＝ Ｔｒｕｅ<br />
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
    <cc1:WebCustomButton ID="btnButton3" runat="server"  Text="例外ボタン" Width="180px" /><br />    
</asp:Content>

