//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testScreen2nest
//* クラス日本語名  ：テスト画面２のネスト版（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.UI.WebControls;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;

namespace WebForms_Sample.Aspx.TestFxLayerP.Nest
{
    /// <summary>テスト画面２のネスト版（Ｐ層）</summary>
    public partial class testScreen2nest : MyBaseController
    {
        #region Page LoadのUOCメソッド

        /// <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // Form初期化（初回Load）時に実行する処理を実装する
            // TODO:
            Response.Write(this.ContentPageFileNoEx + "<br/>");

            // クライアントからの業務Modal画面起動
            // スタイル指定なし
            this.btnButton2.OnClientClick =
                "return " + this.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx") + ";";
            this.btnButton3.OnClientClick =
                "return " + this.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test") + ";";

            // スタイル指定あり（空）
            this.btnButton4.OnClientClick =
                "return " + this.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx", "") + ";";
            this.btnButton5.OnClientClick =
                "return " + this.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test", "") + ";";

            // ---

            // クライアントからの業務Modeless画面起動
            this.btnButton9.OnClientClick =
                this.GetScriptToShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx")
                + "; return false;";

            this.btnButton10.OnClientClick =
                this.GetScriptToShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test", "")
                + "; return false;";

            this.btnButton11.OnClientClick =
                this.GetScriptToShowNormalScreen("~/Aspx/TestFxLayerP/Nest/testScreen1nest.aspx?test=test", "", "t")
                + "; return false;";
        }

        /// <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // Form初期化（Post Back）時に実行する処理を実装する
            // TODO:
        }

        #endregion

        #region 外部パラメータ（アイコン）

        #region Content Page上のフレームワーク対象Control

        /// <summary>
        /// btnButton1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowYesNoMessageDialog(
                "MessageID",
                "Message",
                "テスト結果");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// lbnLinkButton1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_lbnLinkButton1_Click(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "MessageID",
                "Message",
                FxEnum.IconType.Information, "テスト結果（情報）");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// ibnImageButton1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_ibnImageButton1_Click(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "MessageID",
                "Message",
                FxEnum.IconType.Exclamation, "テスト結果（警告）");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// impImageMap1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_impImageMap1_Click(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "MessageID",
                "Message",
                FxEnum.IconType.StopMark, "テスト結果（エラー）");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion

        #region イベント追加

        #region DropDownList

        #region Master Page上

        #region 既存のルートのMaster Page上

        /// <summary>
        /// ddlMDropDownList1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_TestScreen2_ddlMDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "ddlMDropDownList1のSelectedIndexChangedイベント",
                 ((DropDownList)this.GetFxWebControl("ddlMDropDownList1")).SelectedValue,
                FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region 追加のブランチのMaster Page上

        /// <summary>
        /// ddlCPF_AのSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testScreen2bmp1_ddlCPF_A_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "ddlCPF_AのSelectedIndexChangedイベント",
                 ((DropDownList)this.GetFxWebControl("ddlCPF_A")).SelectedValue,
                FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// ddlCPF_A1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testScreen2bmp2_ddlCPF_A1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "ddlCPF_A1のSelectedIndexChangedイベント",
                 ((DropDownList)this.GetFxWebControl("ddlCPF_A1")).SelectedValue,
                FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion

        #region Content Page上

        /// <summary>
        /// ddlDropDownList1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_ddlDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "ddlDropDownList1のSelectedIndexChangedイベント",
                 ((DropDownList)this.GetFxWebControl("ddlDropDownList1")).SelectedValue,
                FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion

        #region ListBox

        #region Master Page上

        /// <summary>
        /// lbxMListBox1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_TestScreen2_lbxMListBox1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "lbxMListBox1のSelectedIndexChangedイベント",
                 ((ListBox)this.GetFxWebControl("lbxMListBox1")).SelectedValue,
                FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region Content Page上

        /// <summary>
        /// lbxListBox1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_lbxListBox1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "lbxListBox1のSelectedIndexChangedイベント",
                 ((ListBox)this.GetFxWebControl("lbxListBox1")).SelectedValue,
                FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion

        #region RadioButton

        #region Master Page上

        /// <summary>
        /// rbnMRadioButton1のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_TestScreen2_rbnMRadioButton1_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "rbnMRadioButton1のCheckedChangedイベント",
                 ((RadioButton)(this.GetFxWebControl("rbnMRadioButton1"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// rbnMRadioButton2のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_TestScreen2_rbnMRadioButton2_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "rbnMRadioButton2のCheckedChangedイベント",
                 ((RadioButton)(this.GetFxWebControl("rbnMRadioButton2"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region Content Page上

        /// <summary>
        /// rbnRadioButton1のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_rbnRadioButton1_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "rbnRadioButton1のCheckedChangedイベント",
                 ((RadioButton)(this.GetFxWebControl("rbnRadioButton1"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// rbnRadioButton2のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_rbnRadioButton2_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "rbnRadioButton2のCheckedChangedイベント",
                 ((RadioButton)(this.GetFxWebControl("rbnRadioButton2"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion

        #region CheckBox

        #region Master Page上

        /// <summary>
        /// cbxMCheckBox1のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_TestScreen2_cbxMCheckBox1_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "cbxMCheckBox1のCheckedChangedイベント",
                 ((CheckBox)(this.GetFxWebControl("cbxMCheckBox1"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// cbxMCheckBox2のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_TestScreen2_cbxMCheckBox2_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "cbxMCheckBox2のCheckedChangedイベント",
                 ((CheckBox)(this.GetFxWebControl("cbxMCheckBox2"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region Content Page上

        /// <summary>
        /// cbxCheckBox1のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_cbxCheckBox1_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "cbxCheckBox1のCheckedChangedイベント",
                 ((CheckBox)(this.GetFxWebControl("cbxCheckBox1"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// cbxCheckBox2のCheckedChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_cbxCheckBox2_CheckedChanged(FxEventArgs fxEventArgs)
        {
            // Message表示
            this.ShowOKMessageDialog(
                 "cbxCheckBox2のCheckedChangedイベント",
                 ((CheckBox)(this.GetFxWebControl("cbxCheckBox2"))).Checked.ToString(),
                 FxEnum.IconType.Information, "GJ!!");

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion

        #region Modal DialogのI/F

        /// <summary>
        /// UOC_btnButton6のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton6_Click(FxEventArgs fxEventArgs)
        {
            // 親画面別セッション領域 - 設定
            this.SetDataToModalInterface("msg", this.TextBox1.Text);
            return "";
        }

        /// <summary>
        /// UOC_btnButton7のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton7_Click(FxEventArgs fxEventArgs)
        {
            // 親画面別セッション領域 - 取得

            // Message表示
            this.ShowOKMessageDialog(
                "親画面別セッション（キー：msg）は、",
                (string)this.GetDataFromModalInterface("msg"),
                FxEnum.IconType.Information, "テスト結果");

            return "";
        }

        /// <summary>
        /// UOC_btnButton8のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton8_Click(FxEventArgs fxEventArgs)
        {
            // 親画面別セッション領域 - キー：msgのみ削除
            this.DeleteDataFromModalInterface("msg");
            return "";
        }

        #endregion

        #endregion

        #region 後処理のUOCメソッド

        /// <summary>「YES」・「NO」Message Dialogの「×」が押され閉じられた場合の処理を実装する。</summary>
        /// <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        protected override void UOC_YesNoDialog_X_Click(FxEventArgs parentFxEventArgs)
        {
            // 「YES」・「NO」Message Dialogの「×」が押され閉じられた場合の処理を実装
            // TODO:

            // switch文

            // Message表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」Message Dialog",
                "[×]Buttonを押した時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        /// <summary>「YES」・「NO」Message Dialogの「YES」が押され閉じられた場合の処理を実装する。</summary>
        /// <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        protected override void UOC_YesNoDialog_Yes_Click(FxEventArgs parentFxEventArgs)
        {
            // 「YES」・「NO」Message Dialogの「YES」が押され閉じられた場合の処理を実装
            // TODO:

            // switch文

            // Message表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」Message Dialog",
                "[Yes]Buttonを押した時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        /// <summary>「YES」・「NO」Message Dialogの「NO」が押され閉じられた場合の処理を実装する。</summary>
        /// <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        protected override void UOC_YesNoDialog_No_Click(FxEventArgs parentFxEventArgs)
        {
            // 「YES」・「NO」Message Dialogの「NO」が押され閉じられた場合の処理を実装
            // TODO:

            // switch文

            // Message表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」Message Dialog",
                "[No]Buttonを押した時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        /// <summary>業務Modal画面の後処理を実装する。</summary>
        /// <param name="parentFxEventArgs">業務Modal画面を開いた（親画面側の）ButtonのButton履歴</param>
        /// <param name="childFxEventArgs">業務Modal画面を閉じた（若しくは一番最後に押された子画面側の）ButtonのButton履歴</param>
        protected override void UOC_ModalDialog_End(FxEventArgs parentFxEventArgs, FxEventArgs childFxEventArgs)
        {
            // 業務Modal画面の後処理を実装
            // TODO:

            // switch文

            // Message表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた業務Modal Dialogの",
                childFxEventArgs.ButtonID + "Buttonを押して閉じた時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        #endregion

    } 
}
