//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testScreenCtrl
//* クラス日本語名  ：画面遷移制御機能テスト画面用のマスタ ページ
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using Touryo.Infrastructure.Framework.Presentation;

namespace ProjectX_sample.Aspx.Common
{
    /// <summary>画面遷移制御機能テスト画面用のマスタ ページ</summary>
    public partial class testScreenCtrl : BaseMasterController
    {
        /// <summary>
        /// btnMButton1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton1_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm0";
        }

        //---

        /// <summary>
        /// lbnMLinkButton1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton1_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm3";
        }

        /// <summary>
        /// lbnMLinkButton2のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton2_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm1";
        }

        /// <summary>
        /// lbnMLinkButton3のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton3_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm2";
        }

        /// <summary>
        /// lbnMLinkButton4のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton4_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm4";
        }

        /// <summary>
        /// lbnMLinkButton5のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton5_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm5";
        }
    } 
}
