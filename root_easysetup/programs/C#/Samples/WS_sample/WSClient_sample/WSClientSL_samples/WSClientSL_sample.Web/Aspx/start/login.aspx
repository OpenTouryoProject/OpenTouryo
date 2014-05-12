<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/TestBlankScreen.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="Aspx_Start_login" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">    
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    <div>
        <table id="Table1" border="1">
            <tr>
		        <td>ユーザID</td>
		        <td>
		            <cc1:WebCustomTextBox id="txtUserID" runat="server"></cc1:WebCustomTextBox>
                </td>
		    </tr>
		    <tr>
		        <td>パスワード</td>
		        <td>
		            <cc1:WebCustomTextBox id="txtPassword" runat="server" TextMode="Password"></cc1:WebCustomTextBox>
    		    </td>
		    </tr>
		    <tr>
	    	    <td colspan="2" align="right">
                    <cc1:WebCustomButton ID="btnButton1" runat="server" Text="ログイン" Width="150px" />
                    <!--<cc1:WebCustomButton ID="btnButton2" runat="server" Text="ログイン" Width="150px" />-->
                </td>
		    </tr>
	    </table>
	    <cc1:WebCustomLabel id="lblMessage" runat="server" Width="250px">Label</cc1:WebCustomLabel>    
    </div>
</asp:Content>
