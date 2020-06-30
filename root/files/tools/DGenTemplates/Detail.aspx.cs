//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_TableName_Detail
//* クラス日本語名  ：三層データバインド・詳細表示画面（_TableName_）
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：自動生成ツール（墨壺２）, _UserName_
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/12/22  Sai               Modified ReadOnly property of the primary key column textbox to true.  
//**********************************************************************************

using MyType;

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

/// <summary>三層データバインド・サンプル アプリ画面（詳細表示）</summary>
public partial class _TableName_Detail : MyBaseController
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
        _3TierParameterValue parameterValue = null;
        _3TierReturnValue returnValue = null;

        if (Session["PrimaryKeyAndTimeStamp"] == null)
        {
            // 追加処理のみ。
            this.btnEdit.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            // 編集
            this.SetControlReadOnly(false);
        }
        else
        {
            // 詳細表示処理

            // 引数クラスを生成
            parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, "FormInit", "SelectRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

            // テーブル
            parameterValue.TableName = "_TableName_";

            // 主キーとタイムスタンプ列
            parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];

            // B層を生成
            _3TierEngine b = new _3TierEngine();

            // データ取得処理を実行
            returnValue = (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // 編集状態の初期化

            // 値
            // ControlComment:LoopStart-PKColumn
            this.txt_ColumnName_.Text = returnValue.Dt.Rows[0]["_ColumnName_"].ToString();
            // ControlComment:LoopEnd-PKColumn
            // ControlComment:LoopStart-ElseColumn
            this.txt_ColumnName_.Text = returnValue.Dt.Rows[0]["_ColumnName_"].ToString();
            // ControlComment:LoopEnd-ElseColumn
            // 編集
            this.SetControlReadOnly(true);
        }        
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
        Session["DAP"] = "_DAP_";
    }

    #endregion

    #region イベントハンドラ

    #region 編集状態の変更

    /// <summary>編集ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnEdit_Click(FxEventArgs fxEventArgs)
    {
        // 編集状態の変更

        // 編集
        this.SetControlReadOnly(false);

        // 画面遷移しない。
        return string.Empty;
    }

    #endregion

    #region 更新系

    /// <summary>追加ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnInsert_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "InsertRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        // テーブル
        parameterValue.TableName = "_TableName_";

        // 追加値（TimeStamp列は外す。主キーは採番方法次第。
        parameterValue.InsertUpdateValues = new Dictionary<string, object>();
        // ControlComment:LoopStart-ElseColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", this.txt_ColumnName_.Text);
        // ControlComment:LoopEnd-ElseColumn

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 結果表示
        this.lblResult.Text = returnValue.Obj.ToString() + "件追加しました。";

        // 画面遷移しない。
        return string.Empty;
    }

    /// <summary>更新ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnUpdate_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "UpdateRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        // テーブル
        parameterValue.TableName = "_TableName_";

        // 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];

        // 更新値（TimeStamp列は外す。主キーは採番方法次第。
        parameterValue.InsertUpdateValues = new Dictionary<string, object>();
        // ControlComment:LoopStart-ElseColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", this.txt_ColumnName_.Text);
        // ControlComment:LoopEnd-ElseColumn      

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 結果表示
        this.lblResult.Text = returnValue.Obj.ToString() + "件更新しました。";

        // 画面遷移しない。
        return string.Empty;
    }

    /// <summary>削除ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnDelete_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "DeleteRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        // テーブル
        parameterValue.TableName = "_TableName_";

        // 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 結果表示
        this.lblResult.Text = returnValue.Obj.ToString() + "件削除しました。";

        // 画面遷移しない。
        return string.Empty;
    }

    #endregion

    /// <summary>編集可否の制御</summary>
    /// <param name="readOnly">読取専用プロパティ</param>
    private void SetControlReadOnly(bool readOnly)
    {
        // 編集可否
        // ReadOnly

        // 主キー
        // ControlComment:LoopStart-PKColumn
        this.txt_ColumnName_.ReadOnly = true;
        // ControlComment:LoopEnd-PKColumn

        // 主キー以外
        // ControlComment:LoopStart-ElseColumn
        this.txt_ColumnName_.ReadOnly = readOnly;
        // ControlComment:LoopEnd-ElseColumn


        // 背景色
        // BackColor
        System.Drawing.Color backColor;

        if (readOnly)
        {
            backColor = System.Drawing.Color.LightGray;
        }
        else
        {
            backColor = System.Drawing.Color.Empty;
        }

        // 主キー
        // ControlComment:LoopStart-PKColumn
        this.txt_ColumnName_.BackColor = backColor; // System.Drawing.Color.LightGray;
        // ControlComment:LoopEnd-PKColumn

        // 主キー以外
        // ControlComment:LoopStart-ElseColumn
        this.txt_ColumnName_.BackColor = backColor;
        // ControlComment:LoopEnd-ElseColumn


    }

    #endregion
}
