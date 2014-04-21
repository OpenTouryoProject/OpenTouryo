<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="True" Codebehind="testDialogAtOnLoad.aspx.cs" Inherits="Aspx_testFxLayerP_testDialogAtOnLoad" Title="Untitled Page" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    <cc1:WebCustomCheckBox ID="CheckBox1" runat="server" AutoPostBack="False" Checked="true" Text="ポストバックのテスト" /><br />
    <cc1:WebCustomButton ID="WebCustomButton1" runat="server" Text="ポストバックのテスト用" Width="250px" /><br />
    <br />
    <cc1:WebCustomButton ID="btnButton1" runat="server" Text="閉じるボタン" Width="250px" /><br />
    <cc1:WebCustomButton ID="btnButton2" runat="server" Text="閉じるボタン（NoPostback）" Width="250px" /><br />
    <cc1:WebCustomButton ID="btnButton3" runat="server" Text="閉じるボタン（WithAllParent）" Width="250px" /><br />
</asp:Content>

