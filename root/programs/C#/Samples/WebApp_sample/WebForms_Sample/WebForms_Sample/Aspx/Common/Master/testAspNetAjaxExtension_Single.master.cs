//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testAspNetAjaxExtension_Single
//* クラス日本語名  ：Ajaxテスト用のMaster Page（updateパネルを親から纏めて使用）
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using Touryo.Infrastructure.Framework.Presentation;

namespace WebForms_Sample.Aspx.Common.Master
{
    /// <summary>Ajaxテスト用のMaster Page（updateパネルを親から纏めて使用）</summary>
    public partial class testAspNetAjaxExtension_Single : BaseMasterController
    {
        /// <summary>btnMButton1のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton1_Click(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox1.Text = "ajaxのPost Back（Button Click）";

            // ajaxのEvent Handlerでは画面遷移しないこと。
            return "";
        }

        /// <summary>btnMButton2のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton2_Click(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox2.Text = "通常のPost Back（Button Click）";

            return "";
        }

        /// <summary>
        /// ddlMDropDownList1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_ddlMDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox3.Text = "ajaxのPost Back（DDLのSelected Index Changed）";

            // ajaxのEvent Handlerでは画面遷移しないこと。
            return "";
        }

        /// <summary>
        /// ddlMDropDownList2のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_ddlMDropDownList2_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox4.Text = "通常のPost Back（DDLのSelected Index Changed）";

            return "";
        }

        /// <summary>btnMButton3のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton3_Click(FxEventArgs fxEventArgs)
        {
            throw new Exception("Ajaxでエラー");

            //return "";
        }
    } 
}
