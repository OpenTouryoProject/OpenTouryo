//**********************************************************************************
//* サンプル画面（Ｐ層）
//**********************************************************************************

// サンプル画面なので、必要に応じて流用 or 削除して下さい。

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

using Touryo.Infrastructure.Business.Presentation;

namespace WebForms_Sample.Aspx.Start
{
    /// <summary>メニュー画面</summary>
    public partial class menu : MyBaseController
    {
        #region Page LoadのUOCメソッド

        /// <summary>
        /// Page LoadのUOCメソッド（個別：初回Load）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit()
        {
            // Form初期化（初回Load）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない
        }

        /// <summary>
        /// Page LoadのUOCメソッド（個別：Post Back）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // Form初期化（Post Back）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない
        }

        #endregion
    } 
}
