<%@ Page Language="C#" AutoEventWireup="true" CodeFile="myYesNoMessageDialog.aspx.cs" Inherits="Aspx_Framework_myYesNoMessageDialog" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>myYesNoMessageBox</title>
    </head>
    <script type="text/javascript">
        <!--
        
        // 戻り値
        
        // 初期値は０、ダイアログ右上の[×]で閉じた場合に戻る値
        var dialogreturn = 0;
        
        // 開始処理
        function Document_OnLoad(){
        }
        
        // 終了処理
        function Document_OnClose(){
            window.returnValue = dialogreturn;
        }
        
        // [YES]ボタン押下、戻り値を１に書き換えダイアログを閉じる
        function onYes(){
            dialogreturn = 1;
            window.close();
        }
        
        // [NO]ボタン押下、戻り値を２に書き換えダイアログを閉じる
        function onNo(){
            dialogreturn = 2;
            window.close();
        }
        
        //-->
    </script>
    <body onload="Document_OnLoad()" onunload="Document_OnClose()">
        <div style="text-align:left">
            <table border="0" cellpadding="10" cellspacing="10">
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
