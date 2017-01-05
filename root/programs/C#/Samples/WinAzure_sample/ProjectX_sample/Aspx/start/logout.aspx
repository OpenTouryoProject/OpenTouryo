<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="True" Codebehind="logout.aspx.cs" Inherits="Aspx_Start_logout" %>

<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2016 Hitachi Solutions,Ltd. -->
    <div>
        <cc1:WebCustomButton ID="btnButton1" runat="server" Text="ログアウト" Width="150px" />
    </div>
</asp:Content>
