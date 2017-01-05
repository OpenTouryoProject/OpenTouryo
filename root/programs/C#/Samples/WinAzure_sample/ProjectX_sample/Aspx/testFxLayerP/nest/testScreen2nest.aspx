<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testNest/testScreen2bmp2.master" AutoEventWireup="true" Inherits="Aspx_testFxLayerP_normal_testScreen2nest" Title="Untitled Page" Codebehind="testScreen2nest.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A2" Runat="Server">
    <!-- Copyright (C) 2007,2016 Hitachi Solutions,Ltd. -->
    コンテンツ ページ１（個別）<br />
    <table border="1">
        <tr>
            <th colspan="2">
                追加イベント
            </th>
            <th>
                アイコン違い（外部パラメータ）
            </th>
            <th>
                クライアントからの業務モーダル画面起動
            </th>
            <th>
                クライアントからの業務モードレス画面起動
            </th>
        </tr>
        <tr>
            <td>
                ・DropDownList<br />
                <cc1:WebCustomDropDownList ID="ddlDropDownList1" runat="server" AutoPostBack="True" Width="150px">
                    <asp:ListItem>あああ</asp:ListItem>
                    <asp:ListItem>いいい</asp:ListItem>
                    <asp:ListItem>ううう</asp:ListItem>
                    <asp:ListItem>えええ</asp:ListItem>
                    <asp:ListItem>おおお</asp:ListItem>
                </cc1:WebCustomDropDownList>
                <br />
                ※ AutoPostBack = True<br />
                <br />
                ・RadioButton<br />
                <cc1:WebCustomRadioButton ID="rbnRadioButton1" runat="server" AutoPostBack="True" GroupName="Group1" Text="ＣＲＢ１" />
                <cc1:WebCustomRadioButton ID="rbnRadioButton2" runat="server" AutoPostBack="True" GroupName="Group1" Text="ＣＲＢ２" />
                <br />
                ※ AutoPostBack = True<br />
                <br />
                ・CheckBox<br />
                <cc1:WebCustomCheckBox ID="cbxCheckBox1" runat="server" AutoPostBack="True" Text="ＣＣＢ１" />
                <cc1:WebCustomCheckBox ID="cbxCheckBox2" runat="server" AutoPostBack="True" Text="ＣＣＢ２" />
                <br />
                ※ AutoPostBack = True<br />
            </td>
            <td>
                ・ListBox<br />
                <cc1:WebCustomListBox ID="lbxListBox1" runat="server" AutoPostBack="True" Width="150px">
                    <asp:ListItem>あああ</asp:ListItem>
                    <asp:ListItem>いいい</asp:ListItem>
                    <asp:ListItem>ううう</asp:ListItem>
                    <asp:ListItem>えええ</asp:ListItem>
                    <asp:ListItem>おおお</asp:ListItem>
                </cc1:WebCustomListBox>
                <br />
                ※ AutoPostBack = True<br />
            </td>
            <td>
                <cc1:WebCustomButton ID="btnButton1" runat="server" Text="Ｙｅｓ・Ｎｏ" Width="220px" /><br />
                <cc1:WebCustomLinkButton ID="lbnLinkButton1" runat="server" Width="220px">ＯＫ（ｉ）</cc1:WebCustomLinkButton><br />
                <cc1:WebCustomImageButton ID="ibnImageButton1" runat="server" ToolTip="ＯＫ（！）" /><br />
                <cc1:WebCustomImageMap ID="impImageMap1" runat="server" ToolTip="ＯＫ（×）" >
                    <asp:CircleHotSpot HotSpotMode="PostBack" PostBackValue="spot1" X="100" Y="50" Radius="30" />
                    <asp:RectangleHotSpot Bottom="180" HotSpotMode="PostBack" Left="120" PostBackValue="spot2" Right="180" Top="120" />
                    <asp:PolygonHotSpot Coordinates="25,110,10,190,90,190" HotSpotMode="PostBack" PostBackValue="spot3" />
                </cc1:WebCustomImageMap>
            </td>
            <td>
                クライアントからの業務モーダル画面起動<br />
                デフォルトのスタイル<br />
                <cc1:WebCustomButton ID="btnButton2" runat="server" Text="QueryString無し" Width="220px" /><br />
                <cc1:WebCustomButton ID="btnButton3" runat="server" Text="QueryString有り" Width="220px" /><br />
                空のスタイル<br />
                <cc1:WebCustomButton ID="btnButton4" runat="server" Text="QueryString無し" Width="220px" /><br />
                <cc1:WebCustomButton ID="btnButton5" runat="server" Text="QueryString有り" Width="220px" /><br />
                <br />
                業務モーダル画面のＩ / Ｆ<br />
                （親画面別セッション領域）<br />
                （画面GUIDを使用して識別する）<br />
                <cc1:WebCustomTextBox ID="TextBox1" runat="server"></cc1:WebCustomTextBox><br />
                <cc1:WebCustomButton ID="btnButton6" runat="server" Text="設定" Width="50px" />
                <cc1:WebCustomButton ID="btnButton7" runat="server" Text="取得" Width="50px" />
                <cc1:WebCustomButton ID="btnButton8" runat="server" Text="削除" Width="50px" />
            </td>
            <td>
                クライアントからの業務モードレス画面起動<br />
                デフォルトのスタイル、空のターゲット<br />
                <cc1:WebCustomButton ID="btnButton9" runat="server" Text="QueryString無し" Width="220px" /><br />
                空のスタイル、空のターゲット<br />
                <cc1:WebCustomButton ID="btnButton10" runat="server" Text="QueryString有り" Width="220px" /><br />
                空のスタイル、ターゲット（t）<br />
                <cc1:WebCustomButton ID="btnButton11" runat="server" Text="QueryString有り" Width="220px" /><br />
            </td>
        </tr>
    </table>
</asp:Content>

