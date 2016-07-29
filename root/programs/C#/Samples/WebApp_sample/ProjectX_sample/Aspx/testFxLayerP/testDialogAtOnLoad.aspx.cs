//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_testDialogAtOnLoad
//* クラス日本語名  ：オンロードでのダイアログ表示のテスト画面（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System～
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace ProjectX_sample.Aspx.testFxLayerP
{
    /// <summary>オンロードでのダイアログ表示のテスト画面（Ｐ層）</summary>
    public partial class testDialogAtOnLoad : MyBaseController
    {
        #region ページロードのUOCメソッド

        /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する
            // TODO:

            // 子画面表示メソッドをフォーム初期化（初回ロード）時に実行するテスト
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
                // ＯＫメッセージ ダイアログ表示
                this.ShowOKMessageDialog(
                    "オンロード（初回ロード）で",
                    "ＯＫメッセージ ダイアログ表示",
                    FxEnum.IconType.Information,
                    "オンロードでのテスト");
            }
            else if (dialogAtOnLoad == "yesno")
            {
                // Ｙｅｓ Ｎｏメッセージ ダイアログ表示
                this.ShowYesNoMessageDialog(
                    "オンロード（初回ロード）で",
                    "Ｙｅｓ Ｎｏメッセージ ダイアログ表示",
                    "オンロードでのテスト");
            }
            else if (dialogAtOnLoad == "modal")
            {
                // 業務モーダル ダイアログ表示
                this.ShowModalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");
            }
            else if (dialogAtOnLoad == "modaless")
            {
                // 業務モーダレス画面表示
                this.ShowNormalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");
            }
        }

        /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する
            // TODO:

            // こちらは、ページロードのUOCメソッド（個別：ポストバック）の動作確認用。

            if (((CheckBox)this.GetContentWebControl("CheckBox1")).Checked)
            {
                string dialogAtOnLoad = (string)ViewState["DialogAtOnLoad"];

                if (dialogAtOnLoad == "ok")
                {
                    // ＯＫメッセージ・ダイアログ表示
                    this.ShowOKMessageDialog(
                        "オンロード（ポストバック）で",
                        "ＯＫメッセージ・ダイアログ表示",
                        FxEnum.IconType.Information,
                        "オンロードでのテスト");
                }
                else if (dialogAtOnLoad == "yesno")
                {
                    // Ｙｅｓ・Ｎｏメッセージ・ダイアログ表示
                    this.ShowYesNoMessageDialog(
                        "オンロード（ポストバック）で",
                        "Ｙｅｓ・Ｎｏメッセージ・ダイアログ表示",
                        "オンロードでのテスト");

                    // →　ポストバックのオンロードでShowYesNoMessageDialogは実行できない。
                    //     ShowYesNoMessageDialogの後処理のポストバックで無限ループになる。

                    //     1. ポストバックのオンロードでShowYesNoMessageDialog
                    //     2. Ｙｅｓ・Ｎｏメッセージ・ダイアログ表示
                    //     3. Ｙｅｓ・Ｎｏメッセージ・ダイアログの後処理のポストバック
                    //     4. 1.に戻る（無限ループ）。
                }
                else if (dialogAtOnLoad == "modal")
                {
                    // 業務モーダル ダイアログ表示
                    this.ShowModalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");

                    //→　ポストバックのオンロードでShowModalScreenは実行できない。
                    //    ShowModalScreenの後処理のポストバックで無限ループになる。

                    //     1. ポストバックのオンロードでShowModalScreen
                    //     2. 業務モーダル ダイアログ表示
                    //     3. 業務モーダル ダイアログの後処理のポストバック
                    //     4. 1.に戻る（無限ループ）。

                    //    ※ 閉じる処理が、NoPostback（WithAllParent中間を飛ばす場合）なら可能

                    //    ※ サンプルでは、後処理で、ShowOKMessageDialogも実行され、
                    //       コチラが優先されるので、無限ループにはならない。
                }
                else if (dialogAtOnLoad == "modaless")
                {
                    // 業務モーダレス画面表示
                    this.ShowNormalScreen("~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx");
                }
            }
        }

        #endregion

        #region コンテンツ ページ上のフレームワーク対象コントロール

        /// <summary>
        /// btnButton1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // 画面を閉じる
            this.CloseModalScreen();

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnButton2のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
        {
            // 画面を閉じる
            this.CloseModalScreen_NoPostback();

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnButton2のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
        {
            // 画面を閉じる
            this.CloseModalScreen_WithAllParent();

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region 後処理のUOCメソッド

        /// <summary>「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装する。</summary>
        /// <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
        protected override void UOC_YesNoDialog_X_Click(FxEventArgs parentFxEventArgs)
        {
            // 「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装
            // TODO:

            // switch文

            // メッセージ表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」メッセージ・ダイアログ",
                "[×]ボタンを押した時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        /// <summary>「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装する。</summary>
        /// <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
        protected override void UOC_YesNoDialog_Yes_Click(FxEventArgs parentFxEventArgs)
        {
            // 「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装
            // TODO:

            // switch文

            // メッセージ表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」メッセージ・ダイアログ",
                "[Yes]ボタンを押した時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        /// <summary>「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装する。</summary>
        /// <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
        protected override void UOC_YesNoDialog_No_Click(FxEventArgs parentFxEventArgs)
        {
            // 「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装
            // TODO:

            // switch文

            // メッセージ表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」メッセージ・ダイアログ",
                "[No]ボタンを押した時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        /// <summary>業務モーダル画面の後処理を実装する。</summary>
        /// <param name="parentFxEventArgs">業務モーダル画面を開いた（親画面側の）ボタンのボタン履歴</param>
        /// <param name="childFxEventArgs">業務モーダル画面を閉じた（若しくは一番最後に押された子画面側の）ボタンのボタン履歴</param>
        protected override void UOC_ModalDialog_End(FxEventArgs parentFxEventArgs, FxEventArgs childFxEventArgs)
        {
            // 業務モーダル画面の後処理を実装
            // TODO:

            // switch文

            // メッセージ表示
            this.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID + "で開いた業務モーダル・ダイアログの",
                childFxEventArgs.ButtonID + "ボタンを押して閉じた時の後処理",
                FxEnum.IconType.Information, "テスト結果");
        }

        #endregion
    } 
}
