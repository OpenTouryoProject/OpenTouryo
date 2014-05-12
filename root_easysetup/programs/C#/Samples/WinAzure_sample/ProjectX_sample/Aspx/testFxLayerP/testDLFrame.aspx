<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="True" Codebehind="testDLFrame.aspx.cs" Inherits="Aspx_testFxLayerP_testDLFrame" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    <div style="text-align:center;">
        <iframe id="iframe1" runat="server" style="height:90%; width:100%"></iframe><br/>
        <input id="button1" type="button" value="閉じる" onclick="window.close();" />
    </div>
</asp:Content>
        
