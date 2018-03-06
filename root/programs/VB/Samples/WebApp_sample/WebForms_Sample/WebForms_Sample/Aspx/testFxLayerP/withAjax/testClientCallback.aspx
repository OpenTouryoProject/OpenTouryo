<%@ Page Language="VB" MasterPageFile="~/Aspx/Common/Master/testClientCallback.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestFxLayerP.WithAjax.testClientCallback" Codebehind="testClientCallback.aspx.vb" %>
<%@ Register Assembly="OpenTouryo.CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <br />
    Content Page（個別）<br />
    <cc1:WebCustomTextBox ID="TextBox1" runat="server"></cc1:WebCustomTextBox>
    →　
    <cc1:WebCustomTextBox ID="TextBox2" runat="server"></cc1:WebCustomTextBox><br />
    <br />
    <cc1:WebCustomButton ID="btnButton1" runat="server"  Text="Post Backを起こすButton" Width="250px" /><br />   
    <script type="text/javascript">
        function CCREH_Reverse(arg, context)
        {
            //// デバッグ：argは処理結果
            //alert(arg.toString());
            
            //// デバッグ：contextは処理対象のControl
            //alert(context.toString());
            
            // 元Controlと、先Controlとのマップ
            // context のTextBox1はTextBox2へ置き換える。
            // ※ TextBox1の入力をTextBox2に指定。
            context = context.replace("TextBox1", "TextBox2");
            
            // 値を設定する（ByNameで処理すること）。
            document.getElementsByName(context)[0].value = arg;
        }
    </script>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
