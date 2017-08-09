//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testNestMasterScreen
//* クラス日本語名  ：Master Pageのテスト画面（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.UI;
using System.Web.UI.WebControls;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;

namespace WebForms_Sample.Aspx.TestFxLayerP.Nest
{
    /// <summary>テスト画面ネスト（Ｐ層）</summary>
    public partial class testNestMasterScreen : MyBaseController
    {
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

        #region 共通処理

        /// <summary>共通処理１</summary>
        /// <param name="buttonID">ButtonID</param>
        /// <param name="x">CPFを表す文字数</param>
        private void commonM(string buttonID, int x)
        {
            // Controlの取得
            ((Label)this.GetMasterWebControl("lblMSG")).Text = buttonID;

            // ラベルを非表示
            Control ctrl = this.GetMasterWebControl(
                "lblTest" + buttonID.Substring(buttonID.Length - x, x));

            ctrl.Visible = !(ctrl.Visible);
        }

        /// <summary>共通処理２</summary>
        /// <param name="buttonID">ButtonID</param>
        /// <param name="x">CPFを表す文字数</param>
        private void commonC(string buttonID, int x)
        {
            // Controlの取得
            ((Label)this.GetMasterWebControl("lblMSG")).Text = buttonID;

            // ラベルを非表示
            Control ctrl = this.GetContentWebControl(
                "lblTest" + buttonID.Substring(buttonID.Length - x, x));

            ctrl.Visible = !(ctrl.Visible);
        }

        #endregion

        #region イベント処理

        #region Master Page

        #region rootMasterPage

        /// <summary>btnButtonのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_rootMasterPage_btnButton_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 0);
            return System.String.Empty;
        }

        #endregion

        #region branchMasterPage1

        /// <summary>btnButtonAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage1_btnButtonA_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 1);
            return System.String.Empty;
        }

        /// <summary>btnButtonBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage1_btnButtonB_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 1);
            return System.String.Empty;
        }

        /// <summary>btnButtonCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage1_btnButtonC_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 1);
            return System.String.Empty;
        }

        #endregion

        #region branchMasterPage2

        /// <summary>btnButtonAAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonAA_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonABのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonAB_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonACのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonAC_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonBAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonBA_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonBBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonBB_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonBCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonBC_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonCAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonCA_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonCBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonCB_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        /// <summary>btnButtonCCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_branchMasterPage2_btnButtonCC_Click(FxEventArgs fxEventArgs)
        {
            this.commonM(fxEventArgs.ButtonID, 2);
            return System.String.Empty;
        }

        #endregion

        #endregion

        #region コンテンツ

        /// <summary>btnButtonAAAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonAAA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonAABのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonAAB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonAACのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonAAC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonABAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonABA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonABBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonABB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonABCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonABC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonACAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonACA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonACBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonACB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonACCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonACC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBAAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBAA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBABのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBAB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBACのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBAC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBBAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBBA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBBBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBBB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBBCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBBC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBCAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBCA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBCBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBCB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonBCCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonBCC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCAAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCAA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCABのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCAB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCACのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCAC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCBAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCBA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCBBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCBB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCBCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCBC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCCAのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCCA_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCCBのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCCB_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        /// <summary>btnButtonCCCのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButtonCCC_Click(FxEventArgs fxEventArgs)
        {
            this.commonC(fxEventArgs.ButtonID, 3);
            return System.String.Empty;
        }

        ////// ちなみに存在しないControlを検索した場合どうなるかチェックする。
        ////Control ctrl = null;
        ////ctrl = this.GetMasterWebControl("xxxx");
        ////ctrl = this.GetContentWebControl("xxxx");

        #endregion

        #endregion
    } 
}
