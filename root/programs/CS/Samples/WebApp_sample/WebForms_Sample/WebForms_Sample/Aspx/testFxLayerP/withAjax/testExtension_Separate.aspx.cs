//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testExtension_Separate
//* クラス日本語名  ：AspnetAjaxのテスト画面（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using System.Web.UI.WebControls;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;

namespace WebForms_Sample.Aspx.TestFxLayerP.WithAjax
{
    /// <summary>AspnetAjaxのテスト画面（Ｐ層）</summary>
    public partial class testExtension_Separate : MyBaseController
    {
        /// <summary>二重送信防止機能の確認用</summary>
        private int SleepCnt = 5000;

        #region Page LoadのUOCメソッド

        /// <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // Form初期化（初回Load）時に実行する処理を実装する
            // TODO:
        }

        /// <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // Form初期化（Post Back）時に実行する処理を実装する
            // TODO:
        }

        #endregion

        #region Master Page上のフレームワーク対象Control

        /// <summary>btnMButton4のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testAspNetAjaxExtension_Separate_btnMButton4_Click(FxEventArgs fxEventArgs)
        {
            // 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox5");
            textBox.Text = "ajaxのPost Back（Button Click）";

            // ajaxのEvent Handlerでは画面遷移しないこと。
            return "";
        }

        /// <summary>btnMButton5のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testAspNetAjaxExtension_Separate_btnMButton5_Click(FxEventArgs fxEventArgs)
        {
            // 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox6");
            textBox.Text = "通常のPost Back（Button Click）";

            return "";
        }

        /// <summary>
        /// ddlMDropDownList3のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testAspNetAjaxExtension_Separate_ddlMDropDownList3_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox7");
            textBox.Text = "ajaxのPost Back（DDLのSelected Index Changed）";

            // ajaxのEvent Handlerでは画面遷移しないこと。
            return "";
        }

        /// <summary>
        /// ddlMDropDownList4のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testAspNetAjaxExtension_Separate_ddlMDropDownList4_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox8");
            textBox.Text = "通常のPost Back（DDLのSelected Index Changed）";

            return "";
        }

        /// <summary>btnMButton6のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testAspNetAjaxExtension_Separate_btnMButton6_Click(FxEventArgs fxEventArgs)
        {
            // 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            throw new Exception("Ajaxでエラー");

            //return "";
        }

        #endregion

        #region Content Page上のフレームワーク対象Control

        /// <summary>btnButton1のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // Ajaxを制御する場合は、ScriptManagerを使用する。
            // このクラスを使用すると、Ajax中であるかどうかを判別できる。
            bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
            FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

            // 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetContentWebControl("TextBox1");
            textBox.Text = "ajaxのPost Back（Button Click）";

            // ajaxのEvent Handlerでは画面遷移しないこと。
            return "";
        }

        /// <summary>btnButton2のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
        {
            // Ajaxを制御する場合は、ScriptManagerを使用する。
            // このクラスを使用すると、Ajax中であるかどうかを判別できる。
            bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
            FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

            // 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetContentWebControl("TextBox2");
            textBox.Text = "通常のPost Back（Button Click）";

            return "";
        }

        /// <summary>
        /// ddlDropDownList1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_ddlDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Ajaxを制御する場合は、ScriptManagerを使用する。
            // このクラスを使用すると、Ajax中であるかどうかを判別できる。
            bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
            FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

            // 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetContentWebControl("TextBox3");
            textBox.Text = "ajaxのPost Back（DDLのSelected Index Changed）";

            // ajaxのEvent Handlerでは画面遷移しないこと。
            return "";
        }

        /// <summary>
        /// ddlDropDownList2のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_ddlDropDownList2_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // Ajaxを制御する場合は、ScriptManagerを使用する。
            // このクラスを使用すると、Ajax中であるかどうかを判別できる。
            bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
            FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

            // 待機する（二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            // テキストボックスの値を変更
            TextBox textBox = (TextBox)this.GetContentWebControl("TextBox4");
            textBox.Text = "通常のPost Back（DDLのSelected Index Changed）";

            return "";
        }

        /// <summary>btnButton3のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
        {
            // Ajaxを制御する場合は、ScriptManagerを使用する。
            // このクラスを使用すると、Ajax中であるかどうかを判別できる。
            bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
            FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

            // 待機する（UpdateProgress、二重送信確認用）
            System.Threading.Thread.Sleep(this.SleepCnt);

            throw new Exception("Ajaxでエラー");

            //return "";
        }

        #endregion
    }
    
}