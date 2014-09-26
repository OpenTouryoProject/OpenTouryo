//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_JoinTableName__Screen_Detail
//* クラス日本語名  ：三層データバインド・詳細表示画面（_JoinTableName_）
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：自動生成ツール（墨壺２）, _UserName_
//* 更新履歴        ：
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

/// <summary>三層データバインド・サンプル アプリ画面（詳細表示）</summary>
public partial class _JoinTableName__Screen_Detail : MyBaseController
{
    #region ページロードのUOCメソッド UOC Method of Page Load

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
            parameterValue.TableName = "_JoinTableName_";

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
            this.txt_JoinTextboxColumnName_.Text = returnValue.Dt.Rows[0]["_JoinColumnName_"].ToString();
            // ControlComment:LoopEnd-PKColumn
            // ControlComment:LoopStart-ElseColumn
            this.txt_JoinTextboxColumnName_.Text = returnValue.Dt.Rows[0]["_JoinColumnName_"].ToString();
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

    #region イベントハンドラ EVENT HANDLER

    #region 編集状態の変更 EDIT CHANGE STATE

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

    #region 更新系 CRUD SYSTEM

    #region Insert Record
    /// <summary>追加ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnInsert_Click(FxEventArgs fxEventArgs)
    {
        #region  Create the instance of classes here
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "InsertRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        //Initialize the data access procedure
        _3TierReturnValue returnValue = null;
        // B layer Initialize
        _3TierEngine b = new _3TierEngine();
        #endregion

        // ControlComment:LoopStart-JoinTables
        #region  Set the values to be inserted into to the _TableName_ . Then insert into database
        //Declare InsertUpdateValue dictionary and add the values to it
        parameterValue.InsertUpdateValues = new Dictionary<string, object>();
        // ControlComment:LoopStart-PKColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", this.txt_JoinTextboxColumnName_.Text);
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", this.txt_JoinTextboxColumnName_.Text);
        // ControlComment:LoopEnd-ElseColumn  

        //Reset returnvalue with null;
        returnValue = null;
        //Name of the table  _TableName_
        parameterValue.TableName = "_TableName_";

        // Run the Database access process
        returnValue =
           (_3TierReturnValue)b.DoBusinessLogic(
               (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        this.lblResult_TableName_.Text = returnValue.Obj.ToString() + " Data is Inserted into table: _TableName_";
        #endregion

        // ControlComment:LoopEnd-JoinTables
        //Return empty string since there is no need to redirect to any other page.
        return string.Empty;
    }
    #endregion

    #region Update Record
    /// <summary>更新ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnUpdate_Click(FxEventArgs fxEventArgs)
    {

        #region  Create the instance of classes here

        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "UpdateRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        //Initialize the data access procedure
        _3TierReturnValue returnValue = null;
        // B layer Initialize
        _3TierEngine b = new _3TierEngine();
        Dictionary<string, object> UpdateWhereConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];
        #endregion

        // ControlComment:LoopStart-JoinTables
        #region  Set the values to be updated to the _TableName_. Then Update to database
        // Remove '_TableName__' from the PrimaryKeyandTimeStamp dictionary Key values so developer need not to change the values manually in Dao_TableName__S3_UPDATE.xml
        parameterValue.AndEqualSearchConditions = new Dictionary<string, object>();
        foreach (string k in UpdateWhereConditions.Keys)
        {
            if (k.Split('_')[0] == "_TableName_")
            {
                parameterValue.AndEqualSearchConditions.Add(k.Split('_')[1], UpdateWhereConditions[k]);
            }
        }
        //Declare InsertUpdateValue dictionary and add the values to it
        parameterValue.InsertUpdateValues = new Dictionary<string, object>();
        // ControlComment:LoopStart-PKColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", this.txt_JoinTextboxColumnName_.Text);
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", this.txt_JoinTextboxColumnName_.Text);
        // ControlComment:LoopEnd-ElseColumn  

        //Reset returnvalue with null;
        returnValue = null;
        //Name of the table  _TableName_
        parameterValue.TableName = "_TableName_";

        // Run the Database access process
        returnValue =
           (_3TierReturnValue)b.DoBusinessLogic(
               (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        this.lblResult_TableName_.Text = returnValue.Obj.ToString() + " Data is Updated to the table: _TableName_";
        #endregion

        // ControlComment:LoopEnd-JoinTables
        //Return empty string since there is no need to redirect to any other page.
        return string.Empty;
    } 
    #endregion

    #region Delete Record
    /// <summary>削除ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnDelete_Click(FxEventArgs fxEventArgs)
    {
        #region  Create the instance of classes here
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "DeleteRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        //Initialize the data access procedure
        _3TierReturnValue returnValue = null;
        // B layer Initialize
        _3TierEngine b = new _3TierEngine();
        Dictionary<string, object> DeleteWhereConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];
        #endregion

        // ControlComment:LoopStart-JoinTables
        #region  Delete the data from the _TableName_  table
        // Remove '_TableName__' from the PrimaryKeyandTimeStamp dictionary Key values so developer need not to change the values manually in Dao_TableName__S4_Delete.xml 
        parameterValue.AndEqualSearchConditions = new Dictionary<string, object>();
        foreach (string k in DeleteWhereConditions.Keys)
        {
            if (k.Split('_')[0] == "_TableName_")
            {
                parameterValue.AndEqualSearchConditions.Add(k.Split('_')[1], DeleteWhereConditions[k]);
            }
        }
        //Reset returnvalue with null;
        returnValue = null;
        //Name of the table  _TableName_
        parameterValue.TableName = "_TableName_";

        // Run the Database access process
        returnValue =
           (_3TierReturnValue)b.DoBusinessLogic(
               (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        this.lblResult_TableName_.Text = returnValue.Obj.ToString() + " Data is Deleted from the table: _TableName_";
        #endregion

        // ControlComment:LoopEnd-JoinTables
        //Return empty string since there is no need to redirect to any other page.
        return string.Empty;
    }
    #endregion

    #endregion

    #region Toggle Control Read only Property
    /// <summary>編集可否の制御</summary>
    /// <param name="readOnly">読取専用プロパティ</param>
    private void SetControlReadOnly(bool readOnly)
    {
        // 編集可否
        // ReadOnly

        // 主キー
        // ControlComment:LoopStart-PKColumn
        this.txt_JoinTextboxColumnName_.ReadOnly = false;
        // ControlComment:LoopEnd-PKColumn

        // 主キー以外
        // ControlComment:LoopStart-ElseColumn
        this.txt_JoinTextboxColumnName_.ReadOnly = readOnly;
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
        this.txt_JoinTextboxColumnName_.BackColor = System.Drawing.Color.LightGray;
        // ControlComment:LoopEnd-PKColumn

        // 主キー以外
        // ControlComment:LoopStart-ElseColumn
        this.txt_JoinTextboxColumnName_.BackColor = backColor;
        // ControlComment:LoopEnd-ElseColumn
    }
    #endregion

    #endregion
}
