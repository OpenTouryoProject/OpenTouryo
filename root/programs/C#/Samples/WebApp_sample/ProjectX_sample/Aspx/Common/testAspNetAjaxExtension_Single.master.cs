//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testAspNetAjaxExtension_Single
//* クラス日本語名  ：Ajaxテスト用のマスタ ページ（updateパネルを親から纏めて使用）
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System
using System;

// OpenTouryo
using Touryo.Infrastructure.Framework.Presentation;

namespace ProjectX_sample.Aspx.Common
{
    /// <summary>Ajaxテスト用のマスタ ページ（updateパネルを親から纏めて使用）</summary>
    public partial class testAspNetAjaxExtension_Single : BaseMasterController
    {
        /// <summary>btnMButton1のクリックイベント</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton1_Click(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox1.Text = "ajaxのポストバック（ボタンクリック）";

            // ajaxのイベントハンドラでは画面遷移しないこと。
            return "";
        }

        /// <summary>btnMButton2のクリックイベント</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton2_Click(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox2.Text = "通常のポストバック（ボタンクリック）";

            return "";
        }

        /// <summary>
        /// ddlMDropDownList1のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_ddlMDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox3.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）";

            // ajaxのイベントハンドラでは画面遷移しないこと。
            return "";
        }

        /// <summary>
        /// ddlMDropDownList2のSelectedIndexChangedイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_ddlMDropDownList2_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            // テキストボックスの値を変更
            this.TextBox4.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）";

            return "";
        }

        /// <summary>btnMButton3のクリックイベント</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton3_Click(FxEventArgs fxEventArgs)
        {
            throw new Exception("Ajaxでエラー");

            //return "";
        }
    } 
}
