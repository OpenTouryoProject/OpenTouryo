<%@ Page Async="true" Language="VB" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" CodeBehind="OAuthAuthorizationCodeGrantClient.aspx.vb" Inherits="WebForms_Sample.Aspx.Auth.OAuthAuthorizationCodeGrantClient" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" runat="server">

</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
    <script type="text/javascript">
        (function () {
            window.location.href = "http://localhost:63359/MultiPurposeAuthSite/Account/OAuthAuthorize?client_id=b6b393fe861b430eb4ee061006826b03&response_type=code&scope=profile%20email%20phone%20address%20userid%20auth%20openid&state=<%=Me.State %>&nonce=<%=Me.Nonce %>";
        }());
    </script>
</asp:Content>