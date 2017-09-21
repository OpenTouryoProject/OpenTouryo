<%@ Page Language="VB" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.TestPublic.testScreen" ValidateRequest="false" Codebehind="testScreen.aspx.vb" %>
<%@ Register Assembly="CustomControl" Namespace="Touryo.Infrastructure.CustomControl" TagPrefix="cc1" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>無題のページ</title>
</head>
<body>
    <form id="form1" runat="server" autocomplete="on">
        <div>
            <a href="../start/menu.aspx">メニュー画面</a><br />
            <br />
            <table>
                <tr>
                    <td style="width:33%;vertical-align:top">
                        <br />
                        ＜ログ出力部品のテスト＞<br />
                        <br />
                        <cc1:WebCustomDropDownList ID="ddlLog" runat="server">
                            <asp:ListItem>ACCESS</asp:ListItem>
                            <asp:ListItem>SQLTRACE</asp:ListItem>
                            <asp:ListItem>OPERATION</asp:ListItem>
                        </cc1:WebCustomDropDownList><br />
                        <cc1:WebCustomButton ID="btnDebugLog" runat="server" Text="DEBUGログの出力" Width="200px" OnClick="btnDebugLog_Click"/><br />
                        <cc1:WebCustomButton ID="btnInfoLog" runat="server" Text="INFORMATIONログの出力" Width="200px" OnClick="btnInfoLog_Click"/><br />
                        <cc1:WebCustomButton ID="btnWarnLog" runat="server" Text="WARNINGログの出力" Width="200px" OnClick="btnWarnLog_Click"/><br />
                        <cc1:WebCustomButton ID="btnErrLog" runat="server" Text="ERRORログの出力" Width="200px" OnClick="btnErrLog_Click"/><br />
                        <cc1:WebCustomButton ID="btnFatalLog" runat="server" Text="FATALログの出力" Width="200px" OnClick="btnFatalLog_Click" /><br />
                        <br />
                        <hr />
                        <br />
                        ＜性能測定部品のテスト＞<br />
                        <br />
                        実行回数：<cc1:WebCustomTextBox ID="txtExecCnt" runat="server" Width="125px"></cc1:WebCustomTextBox><br />
                        <cc1:WebCustomButton ID="btnRoop" runat="server"  Text="ループ処理（回）" Width="200px" OnClick="btnRoop_Click" /><br />
                        <cc1:WebCustomButton ID="btnSleep" runat="server"  Text="待機（msec）" Width="200px" OnClick="btnSleep_Click" /><br />
                        <br />
                        ファイルパス：<cc1:WebCustomTextBox ID="txtFilePath1" runat="server" Width="98px"></cc1:WebCustomTextBox><br />
                        <cc1:WebCustomButton ID="btnFileIOO" runat="server"  Text="ファイル書込（回）" Width="200px" OnClick="btnFileIOO_Click" /><br />
                        <cc1:WebCustomButton ID="btnFileIOI" runat="server"  Text="ファイル読込（回）" Width="200px" OnClick="btnFileIOI_Click" /><br />
                        読込：<cc1:WebCustomLabel ID="lblFileData" runat="server" Text="Label"></cc1:WebCustomLabel><br />
                        <br />
                        戻り値：<cc1:WebCustomLabel ID="lblPrefRec1" runat="server" Text="Label"></cc1:WebCustomLabel><br />
                        <br />
                        <table>
                            <tr>
                                <td style="width: 170px">
                                    実行時間
                                </td>
                                <td style="width: 200px">
                                    <cc1:WebCustomLabel ID="lblPrefRec2" runat="server" Text="Label"></cc1:WebCustomLabel>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 170px">
                                    CPU時間
                                </td>
                                <td style="width: 200px">
                                    <cc1:WebCustomLabel ID="lblPrefRec3" runat="server" Text="Label"></cc1:WebCustomLabel>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 170px">
                                    CPU（カーネル）時間
                                </td>
                                <td style="width: 200px">
                                    <cc1:WebCustomLabel ID="lblPrefRec4" runat="server" Text="Label"></cc1:WebCustomLabel>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 170px">
                                    CPU（ユーザ）時間
                                </td>
                                <td style="width: 200px">
                                    <cc1:WebCustomLabel ID="lblPrefRec5" runat="server" Text="Label"></cc1:WebCustomLabel>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <hr />
                        <br />
                        ＜リソースのロード処理＞<br />
                        <br />
                        <cc1:WebCustomCheckBox ID="cbxBinaryMode" runat="server" Text="バイナリ モード" /><br />
                        <table>
                            <tr>
                                <td>ファイルパス</td>
                                <td>：</td>
                                <td style="width: 158px"><cc1:WebCustomTextBox ID="txtFilePath2" runat="server"></cc1:WebCustomTextBox></td>
                            </tr>
                            <tr>
                                <td>ファイル名</td>
                                <td>：</td>
                                <td style="width: 158px"><cc1:WebCustomTextBox ID="txtFileName" runat="server"></cc1:WebCustomTextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    引数１のみ利用</td>
                                <td>：</td>
                                <td style="width: 158px"><cc1:WebCustomButton ID="btnLoadResource1" runat="server" Text="リソースのロード１" Width="170px" OnClick="btnLoadResource1_Click" /></td>
                            </tr>
                            <tr>
                                <td>
                                    引数２のみ利用</td>
                                <td>：</td>
                                <td style="width: 158px"><cc1:WebCustomButton ID="btnLoadResource2" runat="server" Text="リソースのロード２" Width="170px" OnClick="btnLoadResource2_Click" /></td>
                            </tr>
                        </table>
                        <br />
                        結果の表示<br />
                        <cc1:WebCustomTextBox ID="txtRetFileLoad" runat="server" Height="100px" TextMode="MultiLine" Width="300px"></cc1:WebCustomTextBox>
                        <br />
                        <hr />
                        <br />
                        ＜共有情報取得＞<br />
                        <br />
                        共有情報ID：<cc1:WebCustomTextBox ID="txtSPID" runat="server"></cc1:WebCustomTextBox><br />
                        共有情報値：<cc1:WebCustomLabel ID="lblSP" runat="server" Text="Label"></cc1:WebCustomLabel><br />
                        <cc1:WebCustomButton ID="btnGetSP" runat="server" Text="共有情報値の所得" Width="200px" OnClick="btnGetSP_Click"/><br />
                        <br />
                        <hr />
                        <br />
                        ＜Message取得＞<br />
                        <br />
                        MessageID：<cc1:WebCustomTextBox ID="txtMSGID" runat="server"></cc1:WebCustomTextBox><br />
                        Message記述：<cc1:WebCustomLabel ID="lblMSG" runat="server" Text="Label"></cc1:WebCustomLabel><br />
                        <cc1:WebCustomButton ID="btnGetMSG" runat="server" Text="Message記述の所得" Width="200px" OnClick="btnGetMSG_Click"/>
                        <br />
                    </td>
                    <td style="width:33%;vertical-align:top;">
                        <br />
                        <hr />
                        <br />
                        ＜トランザクション制御＞<br />
                        <br />
                        オフ：シングル コネクション モード<br />
                        オン：マルチ コネクション モード<br />
                        <cc1:WebCustomCheckBox ID="cbxCnnMode" runat="server" /><br />
                        <br />
                        データアクセス制御クラス<br />
                        （データプロバイダ）を選択<br />
                        <cc1:WebCustomDropDownList ID="ddlDap" runat="server">
                            <asp:ListItem Value="SQL">SQLClient</asp:ListItem>
                            <asp:ListItem Value="ODP">ODP.NET</asp:ListItem>
                            <asp:ListItem Value="DB2">DB2.NET</asp:ListItem>
                            <asp:ListItem Value="MCN">MySQL Cnn/NET</asp:ListItem>
                        </cc1:WebCustomDropDownList><br />
                        <cc1:WebCustomButton ID="btnTxPID" runat="server" Text="トランザクション パターンＩＤ" Width="250px" OnClick="btnTxPID_Click"/><br />
                        <br />
                        トランザクショングループIDを設定<br />
                        <cc1:WebCustomDropDownList ID="ddlTxGpID" runat="server">
                            <asp:ListItem Value="SQL">SQL（SQLClient）</asp:ListItem>
                            <asp:ListItem Value="ODP">ODP（ODP.NET）</asp:ListItem>
                            <asp:ListItem Value="DB2">DB2（DB2.NET）</asp:ListItem>
                            <asp:ListItem Value="MCN">MCN（MySQL Cnn/NET）</asp:ListItem>
                        </cc1:WebCustomDropDownList><br />
                        <cc1:WebCustomButton ID="btnTxGID" runat="server" Text="トランザクション グループＩＤ" Width="250px" OnClick="btnTxGID_Click"/><br />
                        <br />
                        トランザクションID：<br />
                        <cc1:WebCustomLabel ID="lblTxID" runat="server" Text=""></cc1:WebCustomLabel><br />
                        <br />
                        終了時のステータス<br />
                        <cc1:WebCustomDropDownList ID="ddlExStatus" runat="server">
                            <asp:ListItem Value="UT">トランザクションあり</asp:ListItem>
                            <asp:ListItem Value="NT">トランザクションなし</asp:ListItem>
                            <asp:ListItem Value="NC">コネクションなし</asp:ListItem>
                            <asp:ListItem Value="NULL">データアクセス制御クラス = Null</asp:ListItem>
                        </cc1:WebCustomDropDownList><br />
                        <br />
                        コミット、ロールバックを設定<br />
                        （例外発生時、ロールバック）<br />
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
                        <br />
                        ＜JIS2004対応＞<br />
                        <br />
                        JIS2004追加文字列：<cc1:WebCustomLabel ID="lblJis2K4" runat="server" Text=""></cc1:WebCustomLabel>
                        <cc1:WebCustomButton ID="btnDispJis2K4" runat="server" Text="表示" Width="100px" OnClick="btnDispJis2K4_Click" /><br />
                        <br />
                        入力：<cc1:WebCustomTextBox ID="txtJis2K4Input" runat="server"></cc1:WebCustomTextBox><br />
                        置換：<cc1:WebCustomTextBox ID="txtJis2K4Replace" runat="server"></cc1:WebCustomTextBox><br />
                        出力：<cc1:WebCustomLabel ID="lblJis2K4Output" runat="server" Text=""></cc1:WebCustomLabel><br />
                        <br />
                        文字列情報の出力：
                        <cc1:WebCustomButton ID="btnDispJis2K4Info" runat="server" Text="実行" Width="100px" OnClick="btnDispJis2K4Info_Click" /><br />
                        <br />
                        サロゲート ペア文字：<br />
                        <cc1:WebCustomButton ID="btnCheckSPC1" runat="server" Text="チェック１" Width="100px" OnClick="btnCheckSPC1_Click" />
                        <cc1:WebCustomButton ID="btnCheckSPC2" runat="server" Text="チェック２" Width="100px" OnClick="btnCheckSPC2_Click" /><br />
                        <cc1:WebCustomButton ID="btnDelSPC" runat="server" Text="削除" Width="60px" OnClick="btnDelSPC_Click" />
                        <cc1:WebCustomButton ID="btnRepSPC1" runat="server" Text="文字置換" Width="100px" OnClick="btnRepSPC1_Click" />
                        <cc1:WebCustomButton ID="btnRepSPC2" runat="server" Text="文字列置換" Width="120px" OnClick="btnRepSPC2_Click" /><br />
                        <br />
                        JIS2004追加文字：<br />
                        <cc1:WebCustomButton ID="btnCheckJis2K4_1" runat="server" Text="チェック１" Width="100px" OnClick="btnCheckJis2K4_1_Click" />
                        <cc1:WebCustomButton ID="btnCheckJis2K4_2" runat="server" Text="チェック２" Width="100px" OnClick="btnCheckJis2K4_2_Click"/><br />
                        <cc1:WebCustomButton ID="btnDelJis2K4" runat="server" Text="削除" Width="60px" OnClick="btnDelJis2K4_Click" />
                        <cc1:WebCustomButton ID="btnRepJis2K4_1" runat="server" Text="文字置換" Width="100px" OnClick="btnRepJis2K4_1_Click" />
                        <cc1:WebCustomButton ID="btnRepJis2K4_2" runat="server" Text="文字列置換" Width="120px" OnClick="btnRepJis2K4_2_Click" />
                        <br />
                        <hr />
                        <br />
                        ＜JISX0208-1983対応＞<br />
                        <cc1:WebCustomTextBox ID="txtCheckJISX0208_1983" runat="server" Height="100px" TextMode="MultiLine" Width="300px"></cc1:WebCustomTextBox><br />
                        入力：<cc1:WebCustomButton ID="btnCheckJISX0208_1983" runat="server" Text="チェック" Width="100px" OnClick="btnCheckJISX0208_1983_Click"/><br />
                        出力：<cc1:WebCustomLabel ID="lblCheckJISX0208_1983" runat="server" Text=""></cc1:WebCustomLabel>
                        <br />
                        <hr />
                        <br />
                        ＜ローカル⇔UTC対応＞<br />
                        <asp:Calendar ID="Calendar1" runat="server"></asp:Calendar>
                        タイムゾーン：<cc1:WebCustomCheckBox ID="cbxTimeZone" runat="server" />
                        <cc1:WebCustomDropDownList ID="ddlTimeZone" runat="server"></cc1:WebCustomDropDownList><br />
                        <cc1:WebCustomButton ID="btnLocalToUtc" runat="server" Text="ローカル→UTC" Width="100px" OnClick="btnLocalToUtc_Click" />
                        <cc1:WebCustomButton ID="btnUtcToLocal" runat="server" Text="UTC→ローカル" Width="100px" OnClick="btnUtcToLocal_Click" /><br />
                        出力：<cc1:WebCustomLabel ID="lblDateString" runat="server" Text=""></cc1:WebCustomLabel>
                    </td>
                    <td style="width:33%;vertical-align:top; font-family: Times New Roman;">
                        <br />
                        ＜各種文字列処理＞<br />
                        <br />
                        入力：<cc1:WebCustomTextBox ID="txtStrIn" runat="server"></cc1:WebCustomTextBox><br />
                        出力：<cc1:WebCustomLabel ID="lblStrOut" runat="server" Text=""></cc1:WebCustomLabel><br />
                        <cc1:WebCustomButton ID="btnCopy" runat="server" Text="出力を入力に設定" Width="200px" OnClick="btnCopy_Click" /><br />
                        <br />
                        
                        <table>
                            <tr>
                                <td>
                                    ＜文字列変換＞<br />
                                    <cc1:WebCustomButton ID="btnHtmlEncode" runat="server" Text="HTMLエンコード" Width="120px" OnClick="btnHtmlEncode_Click" /><br />
                                    <cc1:WebCustomButton ID="btnUrlEncode" runat="server" Text="URLエンコード" Width="120px" OnClick="btnUrlEncode_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnToHankaku" runat="server" Text="半角へ" Width="120px" OnClick="btnToHankaku_Click" /><br />
                                    <cc1:WebCustomButton ID="btnToZenkaku" runat="server" Text="全角へ" Width="120px" OnClick="btnToZenkaku_Click" /><br />
                                    <cc1:WebCustomButton ID="btnToKatakana" runat="server" Text="片仮名へ" Width="120px" OnClick="btnToKatakana_Click" /><br />
                                    <cc1:WebCustomButton ID="btnToHiragana" runat="server" Text="平仮名へ" Width="120px" OnClick="btnToHiragana_Click" /><br />
                                    <br />
                                    ＜文字列チェック＞<br />
                                    <cc1:WebCustomButton ID="btnIsNumbers" runat="server" Text="数字" Width="120px" OnClick="btnIsNumbers_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsNumbers_Hankaku" runat="server" Text="数字（半角）" Width="120px" OnClick="btnIsNumbers_Hankaku_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsNumbers_Zenkaku" runat="server" Text="数字（全角）" Width="120px" OnClick="btnIsNumbers_Zenkaku_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsAlphabet" runat="server" Text="英字" Width="120px" OnClick="btnIsAlphabet_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsAlphabet_Hankaku" runat="server" Text="英字（半角）" Width="120px" OnClick="btnIsAlphabet_Hankaku_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsAlphabet_Zenkaku" runat="server" Text="英字（全角）" Width="120px" OnClick="btnIsAlphabet_Zenkaku_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsHiragana" runat="server" Text="平仮名" Width="120px" OnClick="btnIsHiragana_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsKatakana_Zenkaku" runat="server" Text="片仮名（全角）" Width="120px" OnClick="btnIsKatakana_Zenkaku_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsKatakana_Hankaku" runat="server" Text="片仮名（半角）" Width="120px" OnClick="btnIsKatakana_Hankaku_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsKanji" runat="server" Text="漢字" Width="120px" OnClick="btnIsKanji_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsShift_Jis" runat="server" Text="SJIS" Width="120px" OnClick="btnIsShift_Jis_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsShift_Jis_Zenkaku" runat="server" Text="SJIS（全角）" Width="120px" OnClick="btnIsShift_Jis_Zenkaku_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsShift_Jis_Hankaku" runat="server" Text="SJIS（半角）" Width="120px" OnClick="btnIsShift_Jis_Hankaku_Click" /><br />
                                </td>
                                <td>
                                    <br />
                                    ＜フォーマット変換＞<br />
                                    <cc1:WebCustomButton ID="btnSeirekiToWareki" runat="server" Text="和暦→西暦" Width="120px" OnClick="btnSeirekiToWareki_Click"  /><br />
                                    <cc1:WebCustomButton ID="btnWarekiToSeireki" runat="server" Text="西暦→和暦" Width="120px" OnClick="btnWarekiToSeireki_Click"  /><br />
                                    <cc1:WebCustomButton ID="btnAddFigure3" runat="server" Text="３桁区切り" Width="120px" OnClick="btnAddFigure3_Click"  /><br />
                                    <cc1:WebCustomButton ID="btnAddFigure4" runat="server" Text="４桁区切り" Width="120px" OnClick="btnAddFigure4_Click" /><br />
                                    <cc1:WebCustomButton ID="btnSuppress" runat="server" Text="サプレス" Width="120px" OnClick="btnSuppress_Click" /><br />
                                    <br />
                                    ＜フォーマット チェック＞<br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode" runat="server" Text="郵便（区）番号" Width="150px" OnClick="btnIsJpZipCode_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode_H" runat="server" Text="郵便（区）番号 H" Width="150px" OnClick="btnIsJpZipCode_H_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode_N" runat="server" Text="郵便（区）番号 N" Width="150px" OnClick="btnIsJpZipCode_N_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode7" runat="server" Text="郵便 番号" Width="120px" OnClick="btnIsJpZipCode7_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode7_H" runat="server" Text="郵便 番号 H" Width="120px" OnClick="btnIsJpZipCode7_H_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode7_N" runat="server" Text="郵便 番号 N" Width="120px" OnClick="btnIsJpZipCode7_N_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode5" runat="server" Text="郵便 区 番号" Width="120px" OnClick="btnIsJpZipCode5_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode5_H" runat="server" Text="郵便 区 番号 H" Width="120px" OnClick="btnIsJpZipCode5_H_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpZipCode5_N" runat="server" Text="郵便 区 番号 N" Width="120px" OnClick="btnIsJpZipCode5_N_Click" /><br />
                                    <br />
                                    <hr />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsJpTelephoneNumber" runat="server" Text="電話番号" Width="120px" OnClick="btnIsJpTelephoneNumber_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpTelephoneNumber_H" runat="server" Text="電話番号 H" Width="120px" OnClick="btnIsJpTelephoneNumber_H_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpTelephoneNumber_N" runat="server" Text="電話番号 N" Width="120px" OnClick="btnIsJpTelephoneNumber_N_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsJpFixedLinePhoneNumber" runat="server" Text="固定電話番号" Width="150px" OnClick="btnIsJpFixedLinePhoneNumber_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpFixedLinePhoneNumber_H" runat="server" Text="固定電話番号 H" Width="150px" OnClick="btnIsJpFixedLinePhoneNumber_H_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpFixedLinePhoneNumber_N" runat="server" Text="固定電話番号 N" Width="150px" OnClick="btnIsJpFixedLinePhoneNumber_N_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsJpCellularPhoneNumber" runat="server" Text="携帯電話番号" Width="150px" OnClick="btnIsJpCellularPhoneNumber_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpCellularPhoneNumber_H" runat="server" Text="携帯電話番号 H" Width="150px" OnClick="btnIsJpCellularPhoneNumber_H_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpCellularPhoneNumber_N" runat="server" Text="携帯電話番号 N" Width="150px" OnClick="btnIsJpCellularPhoneNumber_N_Click" /><br />
                                    <br />
                                    <cc1:WebCustomButton ID="btnIsJpIpPhoneNumber" runat="server" Text="IP電話番号" Width="150px" OnClick="btnIsJpIpPhoneNumber_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpIpPhoneNumber_H" runat="server" Text="IP電話番号 H" Width="150px" OnClick="btnIsJpIpPhoneNumber_H_Click" /><br />
                                    <cc1:WebCustomButton ID="btnIsJpIpPhoneNumber_N" runat="server" Text="IP電話番号 N" Width="150px" OnClick="btnIsJpIpPhoneNumber_N_Click" /><br />
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <cc1:WebCustomButton ID="btnSessionSize" runat="server" Text="Sessionサイズ" Width="200px" OnClick="btnSessionSize_Click"/><br />
                        <cc1:WebCustomButton ID="WebCustomButton1" runat="server" Text="Sessionサイズ(KB)" Width="200px" OnClick="btnSessionSizeKB_Click"/><br />
                        <cc1:WebCustomButton ID="WebCustomButton2" runat="server" Text="Sessionサイズ(MB)" Width="200px" OnClick="btnSessionSizeMB_Click"/><br />
                        <cc1:WebCustomButton ID="btnImpersonation" runat="server" Text="偽装" Width="200px" OnClick="btnImpersonation_Click"/><br />
                        <cc1:WebCustomButton ID="btnElse" runat="server" Text="その他" Width="200px" OnClick="btnElse_Click"/><br />
                        出力：<cc1:WebCustomLabel ID="lblElse" runat="server"></cc1:WebCustomLabel>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
