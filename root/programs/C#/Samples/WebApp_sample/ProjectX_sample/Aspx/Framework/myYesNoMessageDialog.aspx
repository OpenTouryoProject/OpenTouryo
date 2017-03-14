<%@ Page Language="C#" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.Framework.myYesNoMessageDialog" Codebehind="myYesNoMessageDialog.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<%--Copyright (C) 2007,2016 Hitachi Solutions,Ltd.--%> <%-- Apache License, Version 2.0 --%>
<!DOCTYPE html>
<html>
    <head id="Head1" runat="server">
        <title>myYesNoMessageBox</title>
    </head>
    <script type="text/javascript">
        <!--
        
        // 初期値は０、Dialog右上の[×]で閉じた場合に戻る値
        var dialogreturn = 0;
        
        // 開始処理
        function Document_OnLoad(){
        }
        
        // 終了処理
        function Document_OnClose(){
            window.returnValue = dialogreturn;
        }
        
        // [YES]Button押下、戻り値を１に書き換えDialogを閉じる
        function onYes(){
            dialogreturn = 1;
            window.close();
        }
        
        // [NO]Button押下、戻り値を２に書き換えDialogを閉じる
        function onNo(){
            dialogreturn = 2;
            window.close();
        }
        
        //-->
    </script>
    <body onload="Document_OnLoad()" onunload="Document_OnClose()">
        <div style="text-align:left">
            <table style="border-spacing:10px; border-collapse:collapse;" border="0">
                <tr>
                    <td rowspan="2">
                        <asp:Image ID="imgIcon" runat="server" />
                    </td>
                    <td>
                        <cc1:WebCustomLabel ID="lblmessageID" runat="server" Text="Label"></cc1:WebCustomLabel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:WebCustomLabel ID="lblmessage" runat="server" Text="Label"></cc1:WebCustomLabel>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <div style="text-align:center">
            <input type="button" id="btnYes" name="btnYes" value="はい" onclick="onYes();" tabindex="1" style="width: 100px" />
            <input type="button" id="btnNo" name="btnNo" value="いいえ" onclick="onNo();" tabindex="2" style="width: 100px" />
        </div>
    </body>
</html>
