<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/Master/TestScreenCtrl.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestScreenCtrl.WebForm2" Codebehind="WebForm2.aspx.cs" %>
<%@ Register Assembly="OpenTouryo.CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <br />
    画面遷移（部品使用）：<br />
    <cc1:WebCustomButton ID="btnButton1" runat="server" Text="画面１へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton2" runat="server" Text="画面２へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton3" runat="server" Text="画面３へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton4" runat="server" Text="画面４へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton5" runat="server" Text="画面５へ" Width="80px" /><br />
    <br />
    <cc1:WebCustomCheckBox ID="CheckBox1" runat="server" AutoPostBack="False" Checked="False" Text="Fx付与" /><br />
    <br />
    画面遷移（Transfer or FxTransfer）：<br />
    <cc1:WebCustomButton ID="btnButton6" runat="server" Text="画面１へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton7" runat="server" Text="画面２へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton8" runat="server" Text="画面３へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton9" runat="server" Text="画面４へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton10" runat="server" Text="画面５へ" Width="80px" /><br />
    <br />
    画面遷移（Redirect or FxRedirect）：<br />
    <cc1:WebCustomButton ID="btnButton11" runat="server" Text="画面１へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton12" runat="server" Text="画面２へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton13" runat="server" Text="画面３へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton14" runat="server" Text="画面４へ" Width="80px" />
    <cc1:WebCustomButton ID="btnButton15" runat="server" Text="画面５へ" Width="80px" /><br />
    <br />
    画面遷移（部品使用しない直リン）：<br />
    <a href="WebForm1.aspx">画面１へ</a> 
    <a href="WebForm2.aspx">画面２へ</a> 
    <a href="WebForm3.aspx">画面３へ</a> 
    <a href="WebForm4.aspx">画面４へ</a> 
    <a href="WebForm5.aspx">画面５へ</a><br />
    <br />
    Post Back：<cc1:WebCustomButton ID="btnButton16" runat="server" Text="Post Back" Width="120px" /><br />
    <br />
    初期画面からは、全画面に遷移可能。Getは初期画面と画面３のみ許可<br />
    <br />
    子画面など：<br />
    <cc1:WebCustomButton ID="btnButton17" runat="server" Text="画面１（window open）" Width="170px" />
    <cc1:WebCustomButton ID="btnButton18" runat="server" Text="画面２（window open）" Width="170px" />
    <cc1:WebCustomButton ID="btnButton19" runat="server" Text="画面３（window open）" Width="170px" />
    <cc1:WebCustomButton ID="btnButton20" runat="server" Text="画面４（window open）" Width="170px" />
    <cc1:WebCustomButton ID="btnButton21" runat="server" Text="画面５（window open）" Width="170px" /><br />
    <cc1:WebCustomButton ID="btnButton22" runat="server" Text="画面１（dialog）" Width="130px" />
    <cc1:WebCustomButton ID="btnButton23" runat="server" Text="画面２（dialog）" Width="130px" />
    <cc1:WebCustomButton ID="btnButton24" runat="server" Text="画面３（dialog）" Width="130px" />
    <cc1:WebCustomButton ID="btnButton25" runat="server" Text="画面４（dialog）" Width="130px" />
    <cc1:WebCustomButton ID="btnButton26" runat="server" Text="画面５（dialog）" Width="130px" /><br />
    <br />
    ブラウザ ウィンドウ別セッション領域のテスト：<br />
    <cc1:WebCustomTextBox ID="TextBox1" runat="server"></cc1:WebCustomTextBox>
    <cc1:WebCustomButton ID="btnButton27" runat="server" Text="設定" />
    <cc1:WebCustomButton ID="btnButton28" runat="server" Text="取得" /> 
    <br />
    <asp:Image ID="Image1" runat="server" ImageUrl="./sc2.PNG" /><br />
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
