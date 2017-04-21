<%@ Page Language="C#" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.Framework.myOKMessageDialog" Codebehind="myOKMessageDialog.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<%--Copyright (C) 2007,2016 Hitachi Solutions,Ltd.--%> <%-- Apache License, Version 2.0 --%>
<!DOCTYPE html>
<html>
    <head id="Head1" runat="server">
        <title>myOKMessageBox</title>
        <asp:PlaceHolder runat="server">
            <%: Scripts.Render("~/bundles/modernizr") %>
        </asp:PlaceHolder>
        <webopt:bundlereference runat="server" path="~/bundles/css" />

        <script type="text/javascript">
            // Resolve URL in the javascript
            var baseUrl = "<%= this.ResolveUrl("~/") %>";

            // 初期戻り値(-1)は、Dialog右上の[×]で閉じた場合に戻る値
            var dialogreturn = -1;
        
            // [OK]Button押下時点で、戻り値を 1 に書き換えた上で、このDialogを閉じる
            function onOK() {
                if (Browser_IsIE) {
                    window.returnValue = 1;
                    window.close();
                }
                else {
                    window.parent.Fx_CallbackOfOKMessageDialog(1);
                }
            }
        </script>

    </head>
        
    <body onload="Fx_WhichBrowser();">
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

        <%: Scripts.Render("~/bundles/touryo") %>
        <%: Scripts.Render("~/bundles/app") %>
    </body>
</html>
