<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/Master/testBlankScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestFxLayerP.testDLFrame" Codebehind="testDLFrame.aspx.cs" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <div style="text-align:center;">
        <iframe id="iframe1" runat="server" style="height:90%; width:100%"></iframe><br/>
        <input id="button1" type="button" value="閉じる" onclick="window.close();" />
    </div>
</asp:Content>
        
<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
