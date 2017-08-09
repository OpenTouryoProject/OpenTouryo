//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testScreen0
//* クラス日本語名  ：例外テスト画面（Ｐ層）
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

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;

namespace WebForms_Sample.Aspx.TestFxLayerP.Normal
{
    /// <summary>例外テスト画面（Ｐ層）</summary>
    public partial class testScreen0 : MyBaseController
    {
        /// <summary>不正操作防止機能の局所化</summary>
        void Page_Init(object sender, EventArgs e)
        {

            // OFFの場合、当該画面だけ、不正操作防止機能をONにする。
            this.CanCheckIllegalOperation = true;

            foreach (string key in Request.Form.Keys)
            {
                if (key.IndexOf("btnIllegalOperationCheckOFF") != -1)
                {
                    // btnIllegalOperationCheckOFFButtonによりサブミットされた。
                    this.CanCheckIllegalOperation = false;
                }

                if (key.IndexOf("btnIllegalOperationCheckON") != -1)
                {
                    // btnIllegalOperationCheckONButtonによりサブミットされた。
                    this.CanCheckIllegalOperation = true;
                }
            }
        }

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

        #region Content Page上のフレームワーク対象Control

        #region 例外処理

        /// <summary>
        /// btnAppExのClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnAppEx_Click(FxEventArgs fxEventArgs)
        {
            // 業務例外のスロー
            throw new BusinessApplicationException("E0001", "システム", "停止");
        }

        /// <summary>
        /// btnSysExのClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnSysEx_Click(FxEventArgs fxEventArgs)
        {
            // システム例外
            throw new BusinessSystemException("xxxxx", "P層でスローしたシステム例外");
        }

        /// <summary>
        /// btnElseExのClickイベント
        /// </summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnElseEx_Click(FxEventArgs fxEventArgs)
        {
            // システム例外
            throw new Exception("P層でスローしたその他、一般的な例外");
        }

        #endregion

        #region ユーザ情報のハンドル

        #region キー無し


        /// <summary>btnSetUserInfoのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnSetUserInfo_Click(FxEventArgs fxEventArgs)
        {
            // ユーザ情報を設定
            UserInfoHandle.SetUserInformation(new MyUserInfo(this.txtUserName.Text, Request.UserHostAddress));
            return string.Empty;
        }

        /// <summary>btnGetUserInfoのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnGetUserInfo_Click(FxEventArgs fxEventArgs)
        {
            // ユーザ情報を取得（ベースクラス２経由）
            if (this.UserInfo == null)
            {
                // nullはありえない
                lblUserName.Text = "インスタンスが設定されていません。";
            }
            else
            {
                lblUserName.Text = this.UserInfo.UserName;
            }
            return string.Empty;
        }

        /// <summary>btnUpdUserInfoのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnUpdUserInfo_Click(FxEventArgs fxEventArgs)
        {
            // ユーザ情報を更新（ベースクラス２経由）
            if (this.UserInfo == null)
            {
                // nullはありえない
                lblUserName.Text = "インスタンスが設定されていません。";
            }
            else
            {
                this.UserInfo.UserName = this.txtUserName.Text;
            }
            return string.Empty;
        }

        /// <summary>btnDelUserInfoのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnDelUserInfo_Click(FxEventArgs fxEventArgs)
        {
            // ユーザ情報を削除
            UserInfoHandle.DeleteUserInformation();
            return string.Empty;
        }

        #endregion

        #endregion

        #region サブシステム情報のハンドル

        /// <summary>btnSetSubSysInfoのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnSetSubSysInfo_Click(FxEventArgs fxEventArgs)
        {
            // サブシステム情報の設定
            this.SubsysInfo[this.txtSubSysID.Text][this.txtSubSysInfoKey.Text] = this.txtSubSysInfo.Text;
            return string.Empty;
        }

        /// <summary>btnGetSubSysInfoのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnGetSubSysInfo_Click(FxEventArgs fxEventArgs)
        {
            // サブシステム情報の取得
            this.lblSubSysInfo.Text = (string)this.SubsysInfo[this.txtSubSysID.Text][this.txtSubSysInfoKey.Text];
            return string.Empty;
        }

        /// <summary>btnDelSubSysInfoのClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnDelSubSysInfo_Click(FxEventArgs fxEventArgs)
        {
            // サブシステム情報の取得

            if (this.txtSubSysInfoKey.Text == "")
            {
                // キーが無い場合、ハッシュテーブルごと削除
                this.SubsysInfo[this.txtSubSysID.Text].Clear();
            }
            else
            {
                // キーが有る場合、キー毎に削除
                this.SubsysInfo[this.txtSubSysID.Text].Remove(this.txtSubSysInfoKey.Text);
            }

            return string.Empty;
        }

        #endregion

        #endregion
    } 
}
