//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testScreenCtrl
//* クラス日本語名  ：画面遷移制御機能テスト画面用のMaster Page
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

namespace WebForms_Sample.Aspx.Common.Master
{
    /// <summary>画面遷移制御機能テスト画面用のMaster Page</summary>
    public partial class testScreenCtrl : BaseMasterController
    {
        /// <summary>
        /// btnMButton1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_btnMButton1_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm0";
        }

        //---

        /// <summary>
        /// lbnMLinkButton1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton1_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm3";
        }

        /// <summary>
        /// lbnMLinkButton2のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton2_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm1";
        }

        /// <summary>
        /// lbnMLinkButton3のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton3_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm2";
        }

        /// <summary>
        /// lbnMLinkButton4のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton4_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm4";
        }

        /// <summary>
        /// lbnMLinkButton5のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_lbnMLinkButton5_Click(FxEventArgs fxEventArgs)
        {

            return "WebForm5";
        }
    } 
}
