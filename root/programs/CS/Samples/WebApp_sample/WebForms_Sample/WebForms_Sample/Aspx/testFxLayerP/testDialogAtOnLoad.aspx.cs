//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testDialogAtOnLoad
//* クラス日本語名  ：onloadでのDialog表示のテスト画面（Ｐ層）
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

namespace WebForms_Sample.Aspx.TestFxLayerP
{
    /// <summary>onloadでのDialog表示のテスト画面（Ｐ層）</summary>
    public partial class testDialogAtOnLoad : MyBaseController
    {
        #region Page LoadのUOCメソッド

        /// <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // Form初期化（初回Load）時に実行する処理を実装する
            // TODO:

            // 子画面表示メソッドをForm初期化（初回Load）時に実行するテスト
            // （過去、サポートしていない時期があった）。

            // 取得
            string dialogAtOnLoad = (string)Session["DialogAtOnLoad"];

            // Sessionをクリア
            Session["DialogAtOnLoad"] = "";

            // ViewStateへ移動
            ViewState["DialogAtOnLoad"] = dialogAtOnLoad;

            // ---

            if (dialogAtOnLoad == "ok")
            {
                // OKMessage Dialog表示
                this.ShowOKMessageDialog(
                    "onload（初回Load）で",
                    "OKMessage Dialog表示",
                    FxEnum.IconType.Information,
                    "onloadでのテスト");
            }
            else if (dialogAtOnLoad == "yesno")
            {
                // Yes・NoMessage Dialog表示
                this.ShowYesNoMessageDialog(
                    "onload（初回Load）で",
                    "Yes・NoMessage Dialog表示",
                    "onloadでのテスト");
            }
            else if (dialogAtOnLoad == "modal")
            {
                // 業務Modal Dialog表示
                this.ShowModalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");
            }
            else if (dialogAtOnLoad == "modaless")
            {
                // 業務Modeless画面表示
                this.ShowNormalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");
            }
        }

        /// <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // Form初期化（Post Back）時に実行する処理を実装する
            // TODO:

            // こちらは、Page LoadのUOCメソッド（個別：Post Back）の動作確認用。

            if (((CheckBox)this.GetContentWebControl("CheckBox1")).Checked)
            {
                string dialogAtOnLoad = (string)ViewState["DialogAtOnLoad"];

                if (dialogAtOnLoad == "ok")
                {
                    // OKMessage Dialog表示
                    this.ShowOKMessageDialog(
                        "onload（Post Back）で",
                        "OKMessage Dialog表示",
                        FxEnum.IconType.Information,
                        "onloadでのテスト");
                }
                else if (dialogAtOnLoad == "yesno")
                {
                    // YES・NOMessage Dialog表示
                    this.ShowYesNoMessageDialog(
                        "onload（Post Back）で",
                        "YES・NOMessage Dialog表示",
                        "onloadでのテスト");

                    // →　Post BackのonloadでShowYesNoMessageDialogは実行できない。
                    //     ShowYesNoMessageDialogの後処理のPost Backで無限ループになる。

                    //     1. Post BackのonloadでShowYesNoMessageDialog
                    //     2. YES・NOMessage Dialog表示
                    //     3. YES・NOMessage Dialogの後処理のPost Back
                    //     4. 1.に戻る（無限ループ）。
                }
                else if (dialogAtOnLoad == "modal")
                {
                    // 業務Modal Dialog表示
                    this.ShowModalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");

                    //→　Post BackのonloadでShowModalScreenは実行できない。
                    //    ShowModalScreenの後処理のPost Backで無限ループになる。

                    //     1. Post BackのonloadでShowModalScreen
                    //     2. 業務Modal Dialog表示
                    //     3. 業務Modal Dialogの後処理のPost Back
                    //     4. 1.に戻る（無限ループ）。

                    //    ※ 閉じる処理が、NoPostback（WithAllParent中間を飛ばす場合）なら可能

                    //    ※ サンプルでは、後処理で、ShowOKMessageDialogも実行され、
                    //       コチラが優先されるので、無限ループにはならない。
                }
                else if (dialogAtOnLoad == "modaless")
                {
                    // 業務Modeless画面表示
                    this.ShowNormalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");
                }
            }
        }

        #endregion

        #region Content Page上のフレームワーク対象Control

        /// <summary>
        /// btnButton1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // 画面を閉じる
            this.CloseModalScreen();

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnButton2のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
        {
            // 画面を閉じる
            this.CloseModalScreen_NoPostback();

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnButton2のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
        {
            // 画面を閉じる
            this.CloseModalScreen_WithAllParent();

            // 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            return "";
        }

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
