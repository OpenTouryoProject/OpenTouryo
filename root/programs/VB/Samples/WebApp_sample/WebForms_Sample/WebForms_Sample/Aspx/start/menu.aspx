<%@ Page Language="VB" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.Start.menu" Codebehind="menu.aspx.vb" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    -------------------------<br />
    サンプル プログラム<br />
    -------------------------<br />
    <ul>
        <li>サンプル（全層結合テスト）
            <ul>
                <li><a href="<%= Me.ResolveUrl("~/Aspx/Sample/Crud/sampleScreen.aspx") %>">ノーマル</a></li>
                <li><a href="<%= Me.ResolveUrl("~/Aspx/Sample/Crud/sampleScreen_cc.aspx") %>">通信制御</a></li>
            </ul>
        </li>
        <li>サンプル（3層データバインド・テスト）
            <ul>
                <li><a href="<%= Me.ResolveUrl("~/Aspx/Sample/3Tier/ProductsConditionalSearch.aspx") %>">一覧 → 更新</a></li>
                <li><a href="<%= Me.ResolveUrl("~/Aspx/Sample/3Tier/ProductsSearchAndUpdate.aspx") %>">一覧 & 更新</a></li>
            </ul>
        </li>
    </ul>
    -------------------------<br />
    単体テスト プログラム<br />
    -------------------------<br />
    以下は、テストプログラムのため、<br />
    ユーザ入力をそのままHTML表示する等<br />
    脆弱性が含まれる部分があります。<br />
    -------------------------<br />
    <ul>
        <li>P層フレームワーク
            <ul>
                <li>P層イベント処理
                    <ul>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Normal/testScreen0.aspx") %>">単機能</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx") %>">基本パターン</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Normal/testScreen2.aspx") %>">特殊パターン</a></li>
                    </ul>
                </li>
                <li>ネスト系
                    <ul>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Nest/testNestMasterScreen.aspx") %>">ネスト パターン</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx") %>">基本パターン（ネスト）</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Nest/testScreen2nest.aspx") %>">特殊パターン（ネスト）</a></li>
                    </ul>
                </li>
                <li>テーブル系
                    <ul>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Table/testRepeater.aspx") %>">Repeaterのイベントテスト</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Table/testGridView.aspx") %>">GridViewのイベントテスト</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Table/testListView.aspx") %>">ListViewのイベントテスト</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Table/testJQGridJson.aspx") %>">JQGridのテスト</a></li>
                    </ul>
                </li>
                <li>Web Custom Control系
                    <ul>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/Wcc/testWCTextBox.aspx") %>">WebCustomTextBoxのテスト</a></li>
                    </ul>
                </li>
                <li>Ajax対応処理
                    <ul>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/WithAjax/testClientCallback.aspx") %>">ClientCallbackのテスト</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/WithAjax/testExtension_Single.aspx") %>">ASP.NET AJAX Extensionのテスト（単一UpdPnl）</a></li>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestFxLayerP/WithAjax/testExtension_Separate.aspx") %>">ASP.NET AJAX Extensionのテスト（分割UpdPnl）</a></li>
                    </ul>
                </li>
                <li>その他
                    <ul>
                        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestScreenCtrl/WebForm0.aspx") %>">画面遷移制御</a></li>
                    </ul>
                </li>
            </ul>
        </li>
        <li><a href="<%= Me.ResolveUrl("~/Aspx/TestPublic/testScreen.aspx") %>">単体テスト（共通部品）</a></li>
    </ul>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
