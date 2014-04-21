<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="Aspx_testFxLayerP_normal_testScreen0" Title="Untitled Page" Codebehind="testScreen0.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <!-- Copyright (C) 2007,2014 Hitachi Solutions,Ltd. -->
    ＜例外処理のテスト＞<br />
    <cc1:WebCustomButton id="btnAppEx" runat="server" text="業務例外" width="171px"></cc1:WebCustomButton><br/>
    <cc1:WebCustomButton id="btnSysEx" runat="server" text="システム例外" width="171px"></cc1:WebCustomButton><br/>
    <cc1:WebCustomButton id="btnElseEx" runat="server" text="その他、一般的な例外" width="171px"></cc1:WebCustomButton><br/>
    <br />
    <hr />
    <br />
    ＜ユーザ情報の設定・取得・削除＞<br />
    <br />
    ユーザ情報の入力（ユーザ名）：
    <cc1:WebCustomTextBox ID="txtUserName" runat="server"></cc1:WebCustomTextBox><br />
    <!-- ↓はRis対策用（コメントアウト済み）
    キー：
    <cc1:WebCustomTextBox ID="txtUserInfoKey" runat="server"></cc1:WebCustomTextBox><br />
    -->
    ユーザ情報の出力（ユーザ名）：
    <cc1:WebCustomLabel ID="lblUserName" runat="server" Text=""></cc1:WebCustomLabel><br />
    <br />
    共通：
    <cc1:WebCustomButton ID="btnSetUserInfo" runat="server" Text="設定" Width="70px" />
    <cc1:WebCustomButton ID="btnGetUserInfo" runat="server" Text="取得" Width="70px" />
    <cc1:WebCustomButton ID="btnUpdUserInfo" runat="server" Text="更新" Width="70px" />
    <cc1:WebCustomButton ID="btnDelUserInfo" runat="server" Text="削除" Width="70px" /><br />
    <br />
    取得、更新処理は、ベースクラス２経由。<br />
    <br />
    削除後は、新規作成されるまでベースクラス２<br />
    で（ログイン時の情報を元にして）復元される。<br />
    <!-- ↓はRis対策用（コメントアウト済み）
    個別：
    <cc1:WebCustomButton ID="btnSetUserInfos" runat="server" Text="設定" Width="70px" />
    <cc1:WebCustomButton ID="btnGetUserInfos" runat="server" Text="取得" Width="70px" />
    <cc1:WebCustomButton ID="btnDelUserInfos" runat="server" Text="削除" Width="70px" />
    <cc1:WebCustomButton ID="btnGetAllKeys" runat="server" Text="キー一覧" Width="70px" /><br />
    -->
    <br />
    <hr />
    <br />
    ＜サブシステム情報の設定・取得・削除＞<br />
    （ベースクラス２経由で処理）<br />
    <br />
    <table>
        <tr>
            <td>サブシステムＩＤ</td>
            <td><cc1:WebCustomTextBox ID="txtSubSysID" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>キー</td>
            <td><cc1:WebCustomTextBox ID="txtSubSysInfoKey" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>設定情報</td>
            <td><cc1:WebCustomTextBox ID="txtSubSysInfo" runat="server"></cc1:WebCustomTextBox></td>
        </tr>
        <tr>
            <td>取得情報</td>
            <td><cc1:WebCustomLabel ID="lblSubSysInfo" runat="server"></cc1:WebCustomLabel></td>
        </tr>
    </table>
    <cc1:WebCustomButton id="btnSetSubSysInfo" runat="server" text="設定（更新）" width="70px" ></cc1:WebCustomButton>
    <cc1:WebCustomButton id="btnGetSubSysInfo" runat="server" text="取得" width="70px" ></cc1:WebCustomButton>
    <cc1:WebCustomButton id="btnDelSubSysInfo" runat="server" text="削除" width="70px" ></cc1:WebCustomButton><br />
    <br />
    インデクサでハッシュテーブルを取得し、<br />
    ハッシュテーブルを直接操作している。<br />
    <br />
    <hr />
    <br />
    キーイベント抑止で動作がおかしくなっていないか確認（↓）。<br />
    <asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" Height="100px" Width="200px"></asp:TextBox>
    <br />
    <hr />
    <br />
    <asp:Button ID="btnIllegalOperationCheckON" runat="server" Text="IllegalOperationCheckON" />
    <asp:Button ID="btnIllegalOperationCheckOFF" runat="server" Text="IllegalOperationCheckOFF" />
</asp:Content>

