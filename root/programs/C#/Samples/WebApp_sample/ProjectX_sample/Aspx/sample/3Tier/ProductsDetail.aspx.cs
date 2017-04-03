//**********************************************************************************
//* テーブル・メンテナンス自動生成・テスト画面
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：_Detail_
//* クラス日本語名  ：三層データバインド・詳細表示画面（_TableName_）
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：自動生成ツール（墨壺２）, _UserName_
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/12/22 Sai         Modified ReadOnly property of the primary key column textbox to true.  
//**********************************************************************************

using System.Data;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Public.Db;

namespace ProjectX_sample.Aspx.Sample._3Tier
{
    /// <summary>三層データバインド・詳細表示画面</summary>
    public partial class ProductsDetail : MyBaseController
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
                parameterValue.TableName = "Products";

                // 主キーとタイムスタンプ列
                parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];

                // B層を生成
                _3TierEngine b = new _3TierEngine();

                // データ取得処理を実行
                returnValue = (_3TierReturnValue)b.DoBusinessLogic(
                    (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

                // 編集状態の初期化

                // 値
                this.txtProductID.Text = returnValue.Dt.Rows[0]["ProductID"].ToString();
                this.txtProductName.Text = returnValue.Dt.Rows[0]["ProductName"].ToString();

                //this.txtSupplierID.Text = returnValue.Dt.Rows[0]["SupplierID"].ToString();
                this.ddlSupplierID.SelectedValue = returnValue.Dt.Rows[0]["SupplierID"].ToString();
                //this.txtCategoryID.Text = returnValue.Dt.Rows[0]["CategoryID"].ToString();
                this.ddlCategoryID.SelectedValue = returnValue.Dt.Rows[0]["CategoryID"].ToString();

                this.txtQuantityPerUnit.Text = returnValue.Dt.Rows[0]["QuantityPerUnit"].ToString();
                this.txtUnitPrice.Text = returnValue.Dt.Rows[0]["UnitPrice"].ToString();
                this.txtUnitsInStock.Text = returnValue.Dt.Rows[0]["UnitsInStock"].ToString();
                this.txtUnitsOnOrder.Text = returnValue.Dt.Rows[0]["UnitsOnOrder"].ToString();
                this.txtReorderLevel.Text = returnValue.Dt.Rows[0]["ReorderLevel"].ToString();
                this.txtDiscontinued.Text = returnValue.Dt.Rows[0]["Discontinued"].ToString();

                // 編集
                this.SetControlReadOnly(true);
            }

            #region マスタ・データのロードと設定

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            parameterValue
                = new _3TierParameterValue(
                    this.ContentPageFileNoEx,
                    "FormInit_PostBack", "Invoke",
                    this.ddlDap.SelectedValue,
                    this.UserInfo);

            // B層を生成
            GetMasterData getMasterData = new GetMasterData();

            // 業務処理を実行
            returnValue = (_3TierReturnValue)getMasterData.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            DataTable[] dts = (DataTable[])returnValue.Obj;
            DataTable dt = null;
            DataRow dr = null;

            // daoSuppliers
            _3TierEngine.CreateDropDownListDataSourceDataTable(
                dts[0], "SupplierID", "CompanyName", out dt, "value", "text");

            dr = dt.NewRow();
            dr["value"] = "";
            dr["text"] = "empty";
            dt.Rows.Add(dr);
            dt.AcceptChanges();

            this.ddlSupplierID.DataValueField = "value";
            this.ddlSupplierID.DataTextField = "text";

            this.ddlSupplierID.DataSource = dt;
            this.ddlSupplierID.DataBind();

            // daoCategories
            _3TierEngine.CreateDropDownListDataSourceDataTable(
                dts[1], "CategoryID", "CategoryName", out dt, "value", "text");

            dr = dt.NewRow();
            dr["value"] = "";
            dr["text"] = "empty";
            dt.Rows.Add(dr);
            dt.AcceptChanges();

            this.ddlCategoryID.DataValueField = "value";
            this.ddlCategoryID.DataTextField = "text";

            this.ddlCategoryID.DataSource = dt;
            this.ddlCategoryID.DataBind();

            // 追加時はSelectedValueに空の値を設定
            if (Session["PrimaryKeyAndTimeStamp"] == null)
            {
                this.ddlSupplierID.SelectedValue = "";
                this.ddlCategoryID.SelectedValue = "";
            }

            #endregion
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
            Session["DAP"] = this.ddlDap.SelectedValue;
        }

        #endregion

        #region Event Handler

        #region 編集状態の変更

        /// <summary>編集Button</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
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

        /// <summary>追加Button</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnInsert_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            _3TierParameterValue parameterValue = new _3TierParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "InsertRecord",
                    (string)Session["DAP"], (MyUserInfo)this.UserInfo);

            // テーブル
            parameterValue.TableName = "Products";

            // 追加値（TimeStamp列は外す。主キーは採番方法次第。
            parameterValue.InsertUpdateValues = new Dictionary<string, object>();
            //parameterValue.InsertUpdateValues.Add("ProductID", this.txtProductID.Text);
            parameterValue.InsertUpdateValues.Add("ProductName", this.txtProductName.Text);

            //parameterValue.InsertUpdateValues.Add("SupplierID", this.txtSupplierID.Text);
            parameterValue.InsertUpdateValues.Add("SupplierID", this.ddlSupplierID.SelectedValue);
            //parameterValue.InsertUpdateValues.Add("CategoryID", this.txtCategoryID.Text);
            parameterValue.InsertUpdateValues.Add("CategoryID", this.ddlCategoryID.SelectedValue);

            parameterValue.InsertUpdateValues.Add("QuantityPerUnit", this.txtQuantityPerUnit.Text);
            parameterValue.InsertUpdateValues.Add("UnitPrice", this.txtUnitPrice.Text);
            parameterValue.InsertUpdateValues.Add("UnitsInStock", this.txtUnitsInStock.Text);
            parameterValue.InsertUpdateValues.Add("UnitsOnOrder", this.txtUnitsOnOrder.Text);
            parameterValue.InsertUpdateValues.Add("ReorderLevel", this.txtReorderLevel.Text);
            parameterValue.InsertUpdateValues.Add("Discontinued", this.txtDiscontinued.Text);

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

        /// <summary>更新Button</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnUpdate_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            _3TierParameterValue parameterValue = new _3TierParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "UpdateRecord",
                    (string)Session["DAP"], (MyUserInfo)this.UserInfo);

            // テーブル
            parameterValue.TableName = "Products";

            // 主キーとタイムスタンプ列
            parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];

            // 更新値（TimeStamp列は外す。主キーは採番方法次第。
            parameterValue.InsertUpdateValues = new Dictionary<string, object>();
            //parameterValue.InsertUpdateValues.Add("ProductID", this.txtProductID.Text);
            parameterValue.InsertUpdateValues.Add("ProductName", this.txtProductName.Text);

            //parameterValue.InsertUpdateValues.Add("SupplierID", this.txtSupplierID.Text);
            parameterValue.InsertUpdateValues.Add("SupplierID", this.ddlSupplierID.SelectedValue);
            //parameterValue.InsertUpdateValues.Add("CategoryID", this.txtCategoryID.Text);
            parameterValue.InsertUpdateValues.Add("CategoryID", this.ddlCategoryID.SelectedValue);

            parameterValue.InsertUpdateValues.Add("QuantityPerUnit", this.txtQuantityPerUnit.Text);
            parameterValue.InsertUpdateValues.Add("UnitPrice", this.txtUnitPrice.Text);
            parameterValue.InsertUpdateValues.Add("UnitsInStock", this.txtUnitsInStock.Text);
            parameterValue.InsertUpdateValues.Add("UnitsOnOrder", this.txtUnitsOnOrder.Text);
            parameterValue.InsertUpdateValues.Add("ReorderLevel", this.txtReorderLevel.Text);
            parameterValue.InsertUpdateValues.Add("Discontinued", this.txtDiscontinued.Text);

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

        /// <summary>削除Button</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnDelete_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            _3TierParameterValue parameterValue = new _3TierParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "DeleteRecord",
                    (string)Session["DAP"], (MyUserInfo)this.UserInfo);

            // テーブル
            parameterValue.TableName = "Products";

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
            this.txtProductID.ReadOnly = true;

            // 主キー以外
            this.txtProductName.ReadOnly = readOnly;

            //this.txtSupplierID.ReadOnly = readOnly;
            this.ddlSupplierID.ReadOnly = readOnly;
            //this.txtCategoryID.ReadOnly = readOnly;
            this.ddlCategoryID.ReadOnly = readOnly;

            this.txtQuantityPerUnit.ReadOnly = readOnly;
            this.txtUnitPrice.ReadOnly = readOnly;
            this.txtUnitsInStock.ReadOnly = readOnly;
            this.txtUnitsOnOrder.ReadOnly = readOnly;
            this.txtReorderLevel.ReadOnly = readOnly;
            this.txtDiscontinued.ReadOnly = readOnly;

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
            this.txtProductID.BackColor = System.Drawing.Color.LightGray;

            // 主キー以外
            this.txtProductName.BackColor = backColor;

            //this.txtSupplierID.BackColor = backColor;
            this.ddlSupplierID.BackColor = backColor;
            //this.txtCategoryID.BackColor = backColor;
            this.ddlCategoryID.BackColor = backColor;

            this.txtQuantityPerUnit.BackColor = backColor;
            this.txtUnitPrice.BackColor = backColor;
            this.txtUnitsInStock.BackColor = backColor;
            this.txtUnitsOnOrder.BackColor = backColor;
            this.txtReorderLevel.BackColor = backColor;
            this.txtDiscontinued.BackColor = backColor;
        }

        #endregion
    } 
}