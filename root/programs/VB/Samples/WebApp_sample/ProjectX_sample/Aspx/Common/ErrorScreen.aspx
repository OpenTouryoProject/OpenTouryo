<%@ Page Language="VB" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="ProjectX_sample.Aspx.Common.ErrorScreen" Codebehind="ErrorScreen.aspx.vb" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<%--Copyright (C) 2007,2016 Hitachi Solutions,Ltd.--%> <%-- Apache License, Version 2.0 --%>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" runat="Server">
    <h1>開発用エラー画面</h1>
    <p>
        システム障害が発生しました。<br/>
        Error Messageおよびスタックトレース情報を以下に示します。<br />
        ※ スタック トレースの正確な情報はアクセス トレース ログから確認下さい。<br />
    </p>
    <p>
        本画面は、Form、Session情報をダンプしますので<br />
        本番用エラー画面としては使用しないで下さい（差し替えて下さい）。
    </p>
    <hr />
    <div>
       Error Message：<br />
       <pre><cc1:WebCustomLabel ID="Label1" runat="server"></cc1:WebCustomLabel></pre>
    </div>
    <hr />
    <div>
       その他の情報：<br />
       <pre><cc1:WebCustomLabel ID="Label2" runat="server"></cc1:WebCustomLabel></pre>
    </div>
    <hr />
    <div>
        ＨＴＴＰリクエスト・フォーム情報の一覧：<br />
        <br />
        <asp:Repeater id="Repeater1" runat="server">
          <HeaderTemplate>
             <table border="1" style="width:1000px;">
                <tr>
                   <td style="width:300px"><b>キー</b></td>
                   <td style="width:700px"><b>バリュー</b></td>
                </tr>
          </HeaderTemplate>
             
          <ItemTemplate>
             <tr>
                <td style="width:300px"> <%# DataBinder.Eval(Container.DataItem, "key") %> </td>
                <td style="width:700px"> <%# DataBinder.Eval(Container.DataItem, "Value") %> </td>
             </tr>
          </ItemTemplate>
             
          <FooterTemplate>
             </table>
          </FooterTemplate>
             
       </asp:Repeater>
    </div>
    <div>
        <br />
        <br />
       ＨＴＴＰセッション情報の一覧：<br />
        <br />
        <asp:Repeater id="Repeater2" runat="server">
          <HeaderTemplate>
             <table border="1" style="width:600px;">
                <tr>
                  <td style="width:300px"><b>キー</b></td>
                  <td style="width:700px"><b>バリュー</b></td>
                </tr>
          </HeaderTemplate>
             
          <ItemTemplate>
             <tr>
                <td style="width:300px"> <%# DataBinder.Eval(Container.DataItem, "key") %> </td>
                <td style="width:700px"> <%# DataBinder.Eval(Container.DataItem, "Value") %> </td>
             </tr>
          </ItemTemplate>
             
          <FooterTemplate>
             </table>
          </FooterTemplate>
             
       </asp:Repeater>
    </div>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" runat="Server">

</asp:Content>