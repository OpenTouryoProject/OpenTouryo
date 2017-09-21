<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/testBlankScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestFxLayerP.Wcc.testWCTextBox" Codebehind="testWCTextBox.aspx.cs" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <br/>
    ＜基本＞<br/>
    <table>
        <tr>
            <td>チェック</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox1" runat="server">
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>必須入力</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox2" runat="server" BackColor="skyblue">
                    <CheckType Required="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>半角</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox3" runat="server">
                    <CheckType IsHankaku="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>全角</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox4" runat="server" BackColor="skyblue">
                    <CheckType IsZenkaku="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>数値</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox5" runat="server">
                    <CheckType IsNumeric="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>片仮名</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox6" runat="server" BackColor="skyblue">
                    <CheckType IsKatakana="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>半角片仮名</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox7" runat="server">
                    <CheckType IsHanKatakana="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>平仮名</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox8" runat="server" BackColor="skyblue">
                    <CheckType IsHiragana="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>日付</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox9" runat="server">
                    <CheckType IsDate="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>正規表現（メアド）</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox10" runat="server" BackColor="skyblue"
                    CheckRegExp="^([a-zA-Z0-9])+([a-zA-Z0-9\\._-])*@([a-zA-Z0-9_-])+([a-zA-Z0-9\\._-]+)+$">
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>禁則</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox11" runat="server"
                    CheckProhibitedChar="True">
                </cc1:WebCustomTextBox>
            </td>
        </tr>
    </table>
    <br/>
    ＜組合＞<br/>
    <table>
        <tr>
            <td>半角＆禁則</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox12" runat="server" CheckProhibitedChar="True">
                    <CheckType IsHankaku="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
         <tr>
            <td>全角＆数値</td>
            <td>：
                <cc1:WebCustomTextBox ID="WebCustomTextBox13" runat="server">
                    <CheckType IsZenkaku="True" IsNumeric="True"></CheckType>
                </cc1:WebCustomTextBox>
            </td>
        </tr>
    </table>
    <br/>
    ＜グリッド＞<br/>
    <asp:GridView ID="gvwGridView1" runat="server" 
        AutoGenerateColumns="false" DataKeyNames="fileid"
        AllowPaging="false" AllowSorting="false" PageSize="10"
        Width="100%" BorderWidth="1px">
        
        <HeaderStyle BackColor="darkturquoise" />
        <EditRowStyle BackColor="LightYellow" />
        
        <Columns>
            <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("fileid") %>'></asp:Label>
                    </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox1_gvw" runat="server" Text='<%# Bind("field1") %>'>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox2_gvw" runat="server" BackColor="skyblue" Text='<%# Bind("field2") %>'>
                        <CheckType Required="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox3_gvw" runat="server" Text='<%# Bind("field3") %>'>
                        <CheckType IsHankaku="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox4_gvw" runat="server" BackColor="skyblue" Text='<%# Bind("field4") %>'>
                        <CheckType IsZenkaku="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox5_gvw" runat="server" Text='<%# Bind("field5") %>'>
                        <CheckType IsNumeric="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox6_gvw" runat="server" BackColor="skyblue" Text='<%# Bind("field6") %>'>
                        <CheckType IsKatakana="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox7_gvw" runat="server" Text='<%# Bind("field7") %>'>
                        <CheckType IsHanKatakana="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox8_gvw" runat="server" BackColor="skyblue" Text='<%# Bind("field8") %>'>
                        <CheckType IsHiragana="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox9_gvw" runat="server" Text='<%# Bind("field9") %>'>
                        <CheckType IsDate="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox10_gvw" runat="server" BackColor="skyblue" Text='<%# Bind("field10") %>'
                        CheckRegExp="^([a-zA-Z0-9])+([a-zA-Z0-9\\._-])*@([a-zA-Z0-9_-])+([a-zA-Z0-9\\._-]+)+$">
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox11_gvw" runat="server" Text='<%# Bind("field11") %>'
                        CheckProhibitedChar="True">
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox12_gvw" runat="server" BackColor="skyblue" Text='<%# Bind("field12") %>'
                        CheckProhibitedChar="True"><CheckType IsHankaku="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:WebCustomTextBox ID="WebCustomTextBox13_gvw" runat="server" Text='<%# Bind("field13") %>'>
                        <CheckType IsZenkaku="True" IsNumeric="True"></CheckType>
                    </cc1:WebCustomTextBox>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br/>
    <asp:Button ID="btnCheckText" runat="server" Text="チェック処理を実行" OnClick="btnCheckText_Click" /><br />：制限事項：再起検索チェックではGridView内の行数が取得できない。
    <asp:TextBox ID="TextBox1" runat="server" Height="500px" TextMode="MultiLine" ReadOnly="true" Width="500px"></asp:TextBox>
    
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
