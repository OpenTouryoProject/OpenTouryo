//**********************************************************************************
//* サンプル
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：menu
//* クラス日本語名  ：メニュー画面
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System
// System.Web

// OpenTouryo
using Touryo.Infrastructure.Business.Presentation;

namespace ProjectX_sample.Aspx.Start
{
    /// <summary>メニュー画面</summary>
    public partial class menu : MyBaseController
    {
        #region ページロードのUOCメソッド

        /// <summary>
        /// ページロードのUOCメソッド（個別：初回ロード）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない
            Response.Write(Session["test"]);
        }

        /// <summary>
        /// ページロードのUOCメソッド（個別：ポストバック）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない
        }

        #endregion
    } 
}
