<%@ Page Language="VB" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.Framework.myOKMessageDialog" Codebehind="myOKMessageDialog.aspx.vb" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<%--Copyright (C) 2007,2016 Hitachi Solutions,Ltd.--%> <%-- Apache License, Version 2.0 --%>
<!DOCTYPE html>
<html>
    <head id="Head1" runat="server">
        <title>myOKMessageBox</title>
    </head>
    <script type="text/javascript">
        <!--
        
        // 戻り値
        // 初期戻り値(-1)は、Dialog右上の[×]で閉じた場合に戻る値
        // [OK]Button押下時点で、この変数値を書き換える
        var dialogreturn = -1;
        
        // [OK]Button押下
        // 戻り値を 1 に書き換えた上で、このDialogを閉じる
        function onOK(){
            window.returnValue = 1;
            window.close();
        }
        
        //-->
    </script>
    <body>
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
            <input type="button" id="Button1" name="btnOK" value="OK" onclick="onOK();" tabindex="1" style="width: 100px" />
        </div>
    </body>
</html>
