<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testClientCallback.master" AutoEventWireup="true" Inherits="Aspx_testFxLayerP_withAjax_testClientCallback" Title="Untitled Page" Codebehind="testClientCallback.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    <br />
    コンテンツ ページ（個別）<br />
    <cc1:WebCustomTextBox ID="TextBox1" runat="server"></cc1:WebCustomTextBox>
    →　
    <cc1:WebCustomTextBox ID="TextBox2" runat="server"></cc1:WebCustomTextBox><br />
    <br />
    <cc1:WebCustomButton ID="btnButton1" runat="server"  Text="ポストバックを起こすボタン" Width="250px" /><br />   
    <script type="text/javascript">
        function CCREH_Reverse(arg, context)
        {
            //// デバッグ：argは処理結果
            //alert(arg.toString());
            
            //// デバッグ：contextは処理対象のコントロール
            //alert(context.toString());
            
            // 元コントロールと、先コントロールとのマップ
            // context のTextBox1はTextBox2へ置き換える。
            // ※ TextBox1の入力をTextBox2に指定。
            context = context.replace("TextBox1", "TextBox2");
            
            // 値を設定する（ByNameで処理すること）。
            document.getElementsByName(context)[0].value = arg;
        }
    </script>
</asp:Content>

