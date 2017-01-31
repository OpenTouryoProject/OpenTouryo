//**********************************************************************************
//* サンプル画面（Ｐ層）
//**********************************************************************************

// サンプル画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：logout
//* クラス日本語名  ：ログアウト画面（Forms認証対応）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.Security;

using Touryo.Infrastructure.Business.Presentation;

namespace ProjectX_sample.Aspx.Start
{
    /// <summary>ログアウト画面（Forms認証対応）</summary>
    public partial class logout : MyBaseController
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

            // ログオフ（認証チケットを削除する）
            FormsAuthentication.SignOut();

            // ログインページ（login.aspx）に遷移する。
            Response.Redirect("login.aspx");
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
