<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/sampleScreen.master" AutoEventWireup="true" Inherits="Aspx_sample_crud_sampleScreen" Title="Untitled Page" Codebehind="sampleScreen.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    データアクセス制御クラス（データプロバイダ）を選択<br />
    <cc1:WebCustomDropDownList ID="ddlDap" runat="server">
        <asp:ListItem Value="SQL">SQL Server / SQL Client</asp:ListItem>
        <asp:ListItem Value="OLE">Multi-DB / OLEDB.NET</asp:ListItem>
        <asp:ListItem Value="ODB">Multi-DB / ODCB.NET</asp:ListItem>
        <asp:ListItem Value="ODP">Oracle / ODP.NET</asp:ListItem>
        <asp:ListItem Value="DB2">DB2 / DB2.NET</asp:ListItem>
        <asp:ListItem Value="HIR">HiRDB / HiRDB-DP</asp:ListItem>
        <asp:ListItem Value="MCN">MySQL / Cnn/NET</asp:ListItem>
        <asp:ListItem Value="NPS">PostgreSQL / Npgsql</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    個別、共通、自動生成のＤａｏ種別を選択<br />
    <cc1:WebCustomDropDownList ID="ddlMode1" runat="server">
        <asp:ListItem Value="individual">個別Ｄａｏ</asp:ListItem>
        <asp:ListItem Value="common">共通Ｄａｏ</asp:ListItem>
        <asp:ListItem Value="generate">自動生成Ｄａｏ（更新のみ）</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    静的、動的のクエリ モードを選択<br />
    <cc1:WebCustomDropDownList ID="ddlMode2" runat="server">
        <asp:ListItem Value="static">静的クエリ</asp:ListItem>
        <asp:ListItem Value="dynamic">動的クエリ</asp:ListItem>
    </cc1:WebCustomDropDownList><br />
    <br />
    分離レベルを選択<br />
    <cc1:WebCustomDropDownList ID="ddlIso" runat="server">
        <asp:ListItem Value="NC">ノットコネクト</asp:ListItem>
        <asp:ListItem Value="NT">ノートランザクション</asp:ListItem>
        <asp:ListItem Value="RU">ダーティリード</asp:ListItem>
        <asp:ListItem Value="RC">リードコミット</asp:ListItem>
        <asp:ListItem Value="RR">リピータブルリード</asp:ListItem>
        <asp:ListItem Value="SZ">シリアライザブル</asp:ListItem>
        <asp:ListItem Value="SS">スナップショット</asp:ListItem>
        <asp:ListItem Value="DF">デフォルト</asp:ListItem>
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
            <td><asp:GridView ID="GridView1" runat="server" EmptyDataText="グリッド ビュー"></asp:GridView></td>
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

