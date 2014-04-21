<%@ Page Language="VB" AutoEventWireup="true" CodeFile="myOKMessageDialog.aspx.vb" Inherits="Aspx_Framework_myOKMessageDialog" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>myOKMessageBox</title>
    </head>
    <script type="text/javascript">
        <!--
        
        // 戻り値
        // 初期戻り値(-1)は、ダイアログ右上の[×]で閉じた場合に戻る値
        // [OK]ボタン押下時点で、この変数値を書き換える
        var dialogreturn = -1;
        
        // [ＯＫ]ボタン押下
        // 戻り値を 1 に書き換えた上で、このダイアログを閉じる
        function onOK(){
            window.returnValue = 1;
            window.close();
        }
        
        //-->
    </script>
    <body>
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
            <input type="button" id="Button1" name="btnOK" value="ＯＫ" onclick="onOK();" tabindex="1" style="width: 100px" />
        </div>
    </body>
</html>
