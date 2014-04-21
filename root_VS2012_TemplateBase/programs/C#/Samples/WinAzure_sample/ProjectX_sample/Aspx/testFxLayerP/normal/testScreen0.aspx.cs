//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_normal_testScreen0
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

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

/// <summary>例外テスト画面（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_normal_testScreen0 : MyBaseController
{
    /// <summary>不正操作防止機能の局所化</summary>
    void Page_Init(object sender, EventArgs e)
    {

        // OFFの場合、当該画面だけ、不正操作防止機能をONにする。
        this.CanCheckIllegalOperation = true;

        foreach(string key in  Request.Form.Keys)
        {
            if (key.IndexOf("btnIllegalOperationCheckOFF") != -1)
            {
                // btnIllegalOperationCheckOFFボタンによりサブミットされた。
                this.CanCheckIllegalOperation = false;
            }
            
            if (key.IndexOf("btnIllegalOperationCheckON") != -1)
            {
                // btnIllegalOperationCheckONボタンによりサブミットされた。
                this.CanCheckIllegalOperation = true;
            }
        }
    }

    #region ページロードのUOCメソッド

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:
    }

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    #region 例外処理

    /// <summary>
    /// btnAppExのクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnAppEx_Click(FxEventArgs fxEventArgs)
    {
        // 業務例外のスロー
        throw new BusinessApplicationException("E0001", "システム","停止");
    }

    /// <summary>
    /// btnSysExのクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnSysEx_Click(FxEventArgs fxEventArgs)
    {
        // システム例外
        throw new BusinessSystemException("xxxxx", "P層でスローしたシステム例外");
    }

    /// <summary>
    /// btnElseExのクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnElseEx_Click(FxEventArgs fxEventArgs)
    {
        // システム例外
        throw new Exception("P層でスローしたその他、一般的な例外");
    }

    #endregion

    #region ユーザ情報のハンドル

    #region キー無し


    /// <summary>btnSetUserInfoのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnSetUserInfo_Click(FxEventArgs fxEventArgs)
    {
        // ユーザ情報を設定
        UserInfoHandle.SetUserInformation(new MyUserInfo(this.txtUserName.Text, Request.UserHostAddress));
        return string.Empty;
    }

    /// <summary>btnGetUserInfoのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
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

    /// <summary>btnUpdUserInfoのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
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

    /// <summary>btnDelUserInfoのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
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

    /// <summary>btnSetSubSysInfoのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnSetSubSysInfo_Click(FxEventArgs fxEventArgs)
    {
        // サブシステム情報の設定
        this.SubsysInfo[this.txtSubSysID.Text][this.txtSubSysInfoKey.Text] = this.txtSubSysInfo.Text;
        return string.Empty;
    }

    /// <summary>btnGetSubSysInfoのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnGetSubSysInfo_Click(FxEventArgs fxEventArgs)
    {
        // サブシステム情報の取得
        this.lblSubSysInfo.Text = (string)this.SubsysInfo[this.txtSubSysID.Text][this.txtSubSysInfoKey.Text];
        return string.Empty;
    }

    /// <summary>btnDelSubSysInfoのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
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
