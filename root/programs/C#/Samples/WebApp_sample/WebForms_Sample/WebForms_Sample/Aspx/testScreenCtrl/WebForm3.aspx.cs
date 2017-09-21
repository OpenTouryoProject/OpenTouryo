//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：WebForm3
//* クラス日本語名  ：画面遷移制御機能テスト画面３（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.UI.WebControls;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;

namespace WebForms_Sample.Aspx.TestScreenCtrl
{
    /// <summary>画面遷移制御機能テスト画面３（Ｐ層）</summary>
    public partial class WebForm3 : MyBaseController
    {
        // 部品使用する・しないフラグ
        private bool IsFx = false;

        #region Page LoadのUOCメソッド

        /// <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // Form初期化（初回Load）時に実行する処理を実装する
            // TODO:

            // 状態の通知
            Label lblStatus = (Label)this.GetMasterWebControl("lblStatus");

            if (Request.HttpMethod.ToUpper() == "GET")
            {
                lblStatus.Text = "これは、Redirectによる遷移";
            }
            else if (Request.HttpMethod.ToUpper() == "POST")
            {
                lblStatus.Text = "これは、Transferによる遷移";
            }
            else
            {
                lblStatus.Text = "不明な遷移";
            }

            // ---

            // QueryStringの通知
            Label lblQueryString = (Label)this.GetMasterWebControl("lblQueryString");

            foreach (string qsKey in Request.QueryString.AllKeys)
            {
                lblQueryString.Text += qsKey + "=" + Request.QueryString[qsKey] + ";";
            }
        }

        /// <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // Form初期化（Post Back）時に実行する処理を実装する
            // TODO:

            // 状態の通知
            Label lblStatus = (Label)this.GetMasterWebControl("lblStatus");
            lblStatus.Text = "これは、ポスト（Post Backです）";

            // ---

            // Fxを使用するモード
            if (((CheckBox)this.GetContentWebControl("CheckBox1")).Checked)
            {
                this.IsFx = true;
            }
        }

        #endregion

        #region Master Page上のフレームワーク対象Control

        #region チェック可能な画面遷移（外サイトへ）

        /// <summary>
        /// ibnMImageButton1のClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_testScreenCtrl_ibnMImageButton1_Click(FxEventArgs fxEventArgs)
        {
            // 外部サイトへ（QueryString付き）
            return "google?q=WebForm3";
        }

        #endregion

        #endregion

        #region Content Page上のフレームワーク対象Control

        #region チェック可能な画面遷移

        /// <summary>btnButton1のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>画面遷移不可能（×）</remarks>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            return "3→1";
        }

        /// <summary>btnButton2のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>画面遷移不可能（×）</remarks>
        protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
        {
            return "3→2";
        }

        /// <summary>btnButton3のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>画面遷移不可能（×）</remarks>
        protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
        {
            return "3→3";
        }

        /// <summary>btnButton3のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>画面遷移可能（○）</remarks>
        protected string UOC_btnButton4_Click(FxEventArgs fxEventArgs)
        {
            return "3→4";
        }

        /// <summary>btnButton3のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>画面遷移不可能（×）</remarks>
        protected string UOC_btnButton5_Click(FxEventArgs fxEventArgs)
        {
            return "3→5";
        }

        #endregion

        #region チェック不可能な画面遷移

        #region Transfer or FxTransfer

        /// <summary>btnButton6のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
        protected string UOC_btnButton6_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxTransfer("./WebForm1.aspx");
            }
            else
            {
                Server.Transfer("./WebForm1.aspx");
            }

            return "";
        }

        /// <summary>btnButton7のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
        protected string UOC_btnButton7_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxTransfer("./WebForm2.aspx");
            }
            else
            {
                Server.Transfer("./WebForm2.aspx");
            }

            return "";
        }

        /// <summary>btnButton8のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
        protected string UOC_btnButton8_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxTransfer("./WebForm3.aspx");
            }
            else
            {
                Server.Transfer("./WebForm3.aspx");
            }

            return "";
        }

        /// <summary>btnButton9のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
        protected string UOC_btnButton9_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxTransfer("./WebForm4.aspx");
            }
            else
            {
                Server.Transfer("./WebForm4.aspx");
            }

            return "";
        }

        /// <summary>btnButton10のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
        protected string UOC_btnButton10_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxTransfer("./WebForm5.aspx");
            }
            else
            {
                Server.Transfer("./WebForm5.aspx");
            }

            return "";
        }

        #endregion

        #region Redirect or FxRedirect

        /// <summary>btnButton11のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
        protected string UOC_btnButton11_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxRedirect("./WebForm1.aspx");
            }
            else
            {
                Response.Redirect("./WebForm1.aspx");
            }

            return "";
        }

        /// <summary>btnButton12のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
        protected string UOC_btnButton12_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxRedirect("./WebForm2.aspx");
            }
            else
            {
                Response.Redirect("./WebForm2.aspx");
            }

            return "";
        }

        /// <summary>btnButton13のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
        protected string UOC_btnButton13_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxRedirect("./WebForm3.aspx");
            }
            else
            {
                Response.Redirect("./WebForm3.aspx");
            }

            return "";
        }

        /// <summary>btnButton14のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
        protected string UOC_btnButton14_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxRedirect("./WebForm4.aspx");
            }
            else
            {
                Response.Redirect("./WebForm4.aspx");
            }

            return "";
        }

        /// <summary>btnButton15のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
        protected string UOC_btnButton15_Click(FxEventArgs fxEventArgs)
        {
            if (this.IsFx)
            {
                this.FxRedirect("./WebForm5.aspx");
            }
            else
            {
                Response.Redirect("./WebForm5.aspx");
            }

            return "";
        }

        #endregion

        #endregion

        #region Post Back

        /// <summary>btnButton16のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
        protected string UOC_btnButton16_Click(FxEventArgs fxEventArgs)
        {
            this.ShowOKMessageDialog("Post Backの", "テストです", FxEnum.IconType.Information, "テスト");
            return "";
        }

        #endregion

        #region 子画面の表示

        #region window open

        /// <summary>btnButton17のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>window open</remarks>
        protected string UOC_btnButton17_Click(FxEventArgs fxEventArgs)
        {
            this.ShowNormalScreen("./WebForm1.aspx");
            return "";
        }

        /// <summary>btnButton18のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>window open</remarks>
        protected string UOC_btnButton18_Click(FxEventArgs fxEventArgs)
        {
            this.ShowNormalScreen("./WebForm2.aspx");
            return "";
        }

        /// <summary>btnButton19のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>window open</remarks>
        protected string UOC_btnButton19_Click(FxEventArgs fxEventArgs)
        {
            this.ShowNormalScreen("./WebForm3.aspx");
            return "";
        }

        /// <summary>btnButton20のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>window open</remarks>
        protected string UOC_btnButton20_Click(FxEventArgs fxEventArgs)
        {
            this.ShowNormalScreen("./WebForm4.aspx");
            return "";
        }

        /// <summary>btnButton21のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>window open</remarks>
        protected string UOC_btnButton21_Click(FxEventArgs fxEventArgs)
        {
            this.ShowNormalScreen("./WebForm5.aspx");
            return "";
        }

        #endregion

        #region dialog

        /// <summary>btnButton22のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>dialog</remarks>
        protected string UOC_btnButton22_Click(FxEventArgs fxEventArgs)
        {
            this.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm1.aspx");
            return "";
        }

        /// <summary>btnButton23のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>dialog</remarks>
        protected string UOC_btnButton23_Click(FxEventArgs fxEventArgs)
        {
            this.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm2.aspx");
            return "";
        }

        /// <summary>btnButton24のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>dialog</remarks>
        protected string UOC_btnButton24_Click(FxEventArgs fxEventArgs)
        {
            this.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm3.aspx");
            return "";
        }

        /// <summary>btnButton25のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>dialog</remarks>
        protected string UOC_btnButton25_Click(FxEventArgs fxEventArgs)
        {
            this.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm4.aspx");
            return "";
        }

        /// <summary>btnButton26のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>dialog</remarks>
        protected string UOC_btnButton26_Click(FxEventArgs fxEventArgs)
        {
            this.ShowModalScreen("~/Aspx/TestScreenCtrl/WebForm5.aspx");
            return "";
        }

        #endregion

        #endregion

        #region ブラウザ ウィンドウ別セッション領域

        /// <summary>btnButton27のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>dialog</remarks>
        protected string UOC_btnButton27_Click(FxEventArgs fxEventArgs)
        {
            // ブラウザ ウィンドウ別セッション領域 - 設定
            this.SetDataToBrowserWindow("msg", this.TextBox1.Text);
            return "";
        }

        /// <summary>btnButton28のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>dialog</remarks>
        protected string UOC_btnButton28_Click(FxEventArgs fxEventArgs)
        {
            // ブラウザ ウィンドウ別セッション領域 - 取得
            this.TextBox1.Text = (string)this.GetDataFromBrowserWindow("msg");
            return "";
        }

        #endregion

        #endregion
    } 
}
