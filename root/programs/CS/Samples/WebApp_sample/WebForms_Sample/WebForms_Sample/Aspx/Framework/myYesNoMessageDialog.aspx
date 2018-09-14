<%@ Page Language="C#" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.Framework.myYesNoMessageDialog" Codebehind="myYesNoMessageDialog.aspx.cs" %>
<%@ Register Assembly="OpenTouryo.CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<%--Copyright (C) 2007,2016 Hitachi Solutions,Ltd.--%> <%-- Apache License, Version 2.0 --%>
<!DOCTYPE html>
<html>
    <head id="Head1" runat="server">
        <title>Yes/No Message Dialog</title>
        <asp:PlaceHolder runat="server">
            <%: Scripts.Render("~/bundles/modernizr") %>
        </asp:PlaceHolder>
        <webopt:bundlereference runat="server" path="~/bundles/css" />

        <script type="text/javascript">
            // Resolve URL in the javascript
            var baseUrl = "<%= this.ResolveUrl("~/") %>";
        
            // 初期値は０、Dialog右上の[×]で閉じた場合に戻る値
            var dialogreturn = 0;
        
            // 開始処理
            function Document_OnLoad() {
                Fx_WhichBrowser();

                // 擬似ダイアログの場合のtitle設定
                if (window.dialogArguments === null || window.dialogArguments === undefined){
                    document.getElementById("PseudoDialogTitle").innerText = document.title;
                }
                else {
                    document.getElementById("PseudoDialogTitle").style.display = "none";
                }
            }
        
            // 終了処理
            function Document_OnClose(){
                window.returnValue = dialogreturn;
            }
        
            // [YES]Button押下、戻り値を 1 に書き換えDialogを閉じる
            function onYes() {
                if (Browser_IsIE) {
                    dialogreturn = 1;
                    window.close();
                }
                else {
                    window.parent.Fx_CallbackOfYesNoMessageDialog(1);
                }
            }
        
            // [NO]Button押下、戻り値を 2 に書き換えDialogを閉じる
            function onNo() {
                if (Browser_IsIE) {
                    dialogreturn = 2;
                    window.close();
                }
                else {
                    window.parent.Fx_CallbackOfYesNoMessageDialog(2);
                }
            }

            // 擬似ダイアログに[×]は無いので、0は返らない。
        </script>
    </head>
    
    <body onload="Document_OnLoad();" onunload="Document_OnClose();">
        <div style="text-align:left;">
            <div id="PseudoDialogTitle" style="font-size: large"></div>
            <hr />
            <table style="border-spacing:10px;" border="0">
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
        <div style="text-align:center; height: 40px;">
            <input type="button" class="btn btn-primary" id="btnYes" name="btnYes" value="はい" onclick="onYes();" tabindex="1" style="width: 100px" />
            <input type="button" class="btn btn-primary" id="btnNo" name="btnNo" value="いいえ" onclick="onNo();" tabindex="2" style="width: 100px" />
        </div>

        <%: Scripts.Render("~/bundles/touryo") %>
        <%: Scripts.Render("~/bundles/app") %>
    </body>
</html>
