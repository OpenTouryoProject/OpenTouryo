<%@ Page Language="VB" MasterPageFile="~/Aspx/Common/Master/sampleScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.Sample.Crud.sampleScreen_cc" Codebehind="sampleScreen_cc.aspx.vb" %>
<%@ Register Assembly="OpenTouryo.CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label><br />

    データアクセス制御クラス（データプロバイダ）を選択<br />
    <cc1:WebCustomDropDownList ID="ddlDap" runat="server">
        <asp:ListItem Value="SQL">SQL Server / SQL Client</asp:ListItem>
        <asp:ListItem Value="OLE">Multi-DB / OLEDB.NET</asp:ListItem>
        <asp:ListItem Value="ODB">Multi-DB / ODCB.NET</asp:ListItem>
        <asp:ListItem Value="ODP">Oracle / ODP.NET</asp:ListItem>
        <asp:ListItem Value="MCN">MySQL / Cnn/NET</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    個別、共通、自動生成のDao種別を選択<br />
    <cc1:WebCustomDropDownList ID="ddlMode1" runat="server">
        <asp:ListItem Value="individual">個別Dao</asp:ListItem>
        <asp:ListItem Value="common">共通Dao</asp:ListItem>
        <asp:ListItem Value="generate">自動生成Dao（更新のみ）</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    静的、動的のクエリ モードを選択<br />
    <cc1:WebCustomDropDownList ID="ddlMode2" runat="server">
        <asp:ListItem Value="static">静的クエリ</asp:ListItem>
        <asp:ListItem Value="dynamic">動的クエリ</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    コミット、ロールバックを設定（例外発生時、ロールバック）<br />
    <cc1:WebCustomDropDownList ID="ddlExRollback" runat="server">
        <asp:ListItem Value="-">正常時</asp:ListItem>
        <asp:ListItem Value="Business">業務例外</asp:ListItem>
        <asp:ListItem Value="System">システム例外</asp:ListItem>
        <asp:ListItem Value="Other">その他、一般的な例外</asp:ListItem>
        <asp:ListItem Value="Other-Business">業務例外への振替</asp:ListItem>
        <asp:ListItem Value="Other-System">システム例外への振替</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    通信制御<br />
    <cc1:WebCustomDropDownList ID="ddlCmctCtrl" runat="server">
        <asp:ListItem Value="testInProcess">インプロセス呼出</asp:ListItem>
        <asp:ListItem Value="testWebService4">ASP.NET WebAPI呼出</asp:ListItem>
        <asp:ListItem Value="testWebService3">WCF TCPサービス呼出</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <hr />
    <table>
        <tr>
            <td>ShipperID：</td>
            <td><cc1:WebCustomTextBox ID="TextBox1" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>CompanyName：</td>
            <td><cc1:WebCustomTextBox ID="TextBox2" runat="server" ></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>Phone：</td>
            <td><cc1:WebCustomTextBox ID="TextBox3" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
    </table>
    <br />
    並び替え対象列<br />
    <cc1:WebCustomDropDownList ID="ddlOrderColumn" runat="server">
        <asp:ListItem Value="c1">c1</asp:ListItem>
        <asp:ListItem Value="c2">c2</asp:ListItem>
        <asp:ListItem Value="c3">c3</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    昇順・降順<br />
    <cc1:WebCustomDropDownList ID="ddlOrderSequence" runat="server">
        <asp:ListItem Value="A">ASC</asp:ListItem>
        <asp:ListItem Value="D">DESC</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    <table>
        <tr>
            <td><asp:GridView ID="GridView1" runat="server" EmptyDataText="グリッド ビュー" CssClass="table"></asp:GridView></td>
            <td style="width: 50px"><cc1:WebCustomButton ID="btnButton4" runat="server" Text="クリア" Width="82px" /></td>
        </tr>
    </table>
    <br />
    <hr />
    Ｐ層で例外をスロー<br />
    <cc1:WebCustomButton ID="btnButton1" runat="server" Text="業務例外" Width="89px" />
    &nbsp;<cc1:WebCustomButton ID="btnButton2" runat="server" Text="システム例外" Width="122px" />
    &nbsp;<cc1:WebCustomButton ID="btnButton3" runat="server" Text="その他、一般的な例外" Width="190px" />
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
