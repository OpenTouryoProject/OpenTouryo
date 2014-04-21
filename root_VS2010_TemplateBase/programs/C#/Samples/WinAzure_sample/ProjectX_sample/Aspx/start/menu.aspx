<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="True" Codebehind="menu.aspx.cs" Inherits="Aspx_start_menu" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    -------------------------<br />
    サンプル プログラム<br />
    -------------------------<br />
    <br />
    ● サンプル（全層結合テスト）<br />
    　・ <a href="/Aspx/sample/crud/sampleScreen.aspx">ノーマル</a><br />
    　・ <a href="/Aspx/sample/crud/sampleScreen_cc.aspx">通信制御</a><br />
    <br />
    ● サンプル（３層データバインド・テスト）<br />
    　・ <a href="/Aspx/sample/3Tier/ProductsConditionalSearch.aspx">一覧→更新</a><br />
    　・ <a href="/Aspx/sample/3Tier/ProductsSearchAndUpdate.aspx">一覧＆更新</a><br />
    <br />
    -------------------------<br />
    単体テスト プログラム<br />
    -------------------------<br />
    以下は、テストプログラムのため、<br />
    ユーザ入力をそのままHTML表示する等<br />
    脆弱性が含まれる部分があります。<br />
    -------------------------<br />
    <br />
    ● Ｐ層フレームワーク<br />
    　<br />
    　・ Ｐ層イベント処理<br />
　　　　－ <a href="/Aspx/testFxLayerP/normal/testScreen0.aspx">単機能</a><br />
　　　　－ <a href="/Aspx/testFxLayerP/normal/testScreen1.aspx">基本パターン</a><br />
　　　　－ <a href="/Aspx/testFxLayerP/normal/testScreen2.aspx">特殊パターン</a><br />
　　　　<br />
　　　　・ネスト系<br />
　　　　　－ <a href="/Aspx/testFxLayerP/nest/testNestMasterScreen.aspx">ネスト パターン</a><br />
　　　　　－ <a href="/Aspx/testFxLayerP/nest/testScreen1nest.aspx">基本パターン（ネスト）</a><br />
　　　　　－ <a href="/Aspx/testFxLayerP/nest/testScreen2nest.aspx">特殊パターン（ネスト）</a><br />
　　　　<br />
　　　　・ テーブル系<br />
　　　　　－ <a href="/Aspx/testFxLayerP/table/testRepeater.aspx">Repeaterのイベントテスト</a><br />
　　　　　－ <a href="/Aspx/testFxLayerP/table/testGridView.aspx">GridViewのイベントテスト</a><br />
　　　　<br />
　　　　・ Webカスタム・コントロール系<br />
　　　　　－ <a href="/Aspx/testFxLayerP/wcc/testWCTextBox.aspx">WebCustomTextBoxのテスト</a><br />
　　　　<br />
　　　　・ Ａｊａｘ対応処理<br />
　　　　　－ <a href="/Aspx/testFxLayerP/withAjax/testClientCallback.aspx">ClientCallbackのテスト</a><br />
　　　　　－ <a href="/Aspx/testFxLayerP/withAjax/testExtension_Single.aspx">ASP.NET AJAX Extensionのテスト（単一UpdPnl）</a><br />
　　　　　－ <a href="/Aspx/testFxLayerP/withAjax/testExtension_Separate.aspx">ASP.NET AJAX Extensionのテスト（分割UpdPnl）</a><br />
　　　　<br />
　　　・ その他<br />
　　　　－ <a href="/Aspx/testScreenCtrl/WebForm0.aspx">画面遷移制御</a><br />
　　　　－ <a href="/Aspx/testScreenCtrl/WebForm0.aspx">画面遷移制御</a><br />
　　<br />
    ● <a href="/Aspx/testPublic/testScreen.aspx">単体テスト（共通部品）</a><br />
    <br />
    ● <a href="/WSClientSL_sampleTestPage.aspx">Silverlightサンプル</a><br />
    <br />
    -------------------------<br />
    <br />
    ● <a href="logout.aspx">ログアウト画面</a><br />
    <br />
</asp:Content>

