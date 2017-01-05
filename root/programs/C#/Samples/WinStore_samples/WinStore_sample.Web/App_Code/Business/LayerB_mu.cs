//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：LayerB_mu
//* クラス日本語名  ：Ｂ層のテスト（SOAP汎用Webメソッドのテスト用）
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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
using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using WinStore_sample.Web.Dao;

namespace WinStore_sample.Web.Business
{
    /// <summary>
    /// LayerB_mu の概要の説明です
    /// </summary>
    public class LayerB_mu : MyFcBaseLogic
    {
        #region CRUDPage

        #region テンプレ

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_メソッド名(MuParameterValue muParameter)
        { //メソッド引数にBaseParameterValueの派生の型を定義可能。

            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            DTRow dtrow_in = dtt_in.Rows[0];

            DTTables dtts_out = null;
            DTTable dtt_out = null;
            DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------

            // 個別Dao
            LayerD myDao = new LayerD(this.GetDam());
            //myDao.xxxx(muParameter.ActionType, dtts, muReturn);

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());
            cmnDao.ExecSelectScalar();

            // 戻り値をマーシャリングして設定
            dtts_out = new DTTables();
            dtt_out = new DTTable("ret");
            dtt_out.Cols.Add(new DTColumn("ret", DTType.String));
            dtrow_out = dtt_out.Rows.AddNew();
            dtrow_out["ret"] = "戻り値";
            dtts_out.Add(dtt_out);

            muReturn.Value = DTTables.DTTablesToString(dtts_out);

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region UOCメソッド

        #region SelectCount

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_SelectCount(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            //DTRow dtrow_in = dtt_in.Rows[0];

            //DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------

            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperCount.sql";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperCount.xml";
                            break;
                    }

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Value = cmnDao.ExecSelectScalar().ToString();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Value = genDao.D5_SelCnt().ToString();

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    string ret = "";
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectCount(muParameter.ActionType, out ret);

                    // 戻り値を設定
                    muReturn.Value = ret;

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region SelectAll_DT

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_SelectAll_DT(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            //DTRow dtrow_in = dtt_in.Rows[0];

            DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;

            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLText = "SELECT * FROM Shippers";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLText = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
                            break;
                    }

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 自動生成Daoを実行
                    genDao.D2_Select(dt);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DT(muParameter.ActionType, out dt);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region SelectAll_DS

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_SelectAll_DS(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            //DTRow dtrow_in = dtt_in.Rows[0];

            DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            DataSet ds = null;

            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLText = "SELECT * FROM Shippers";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLText = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
                            break;
                    }

                    // 戻り値 ds
                    ds = new DataSet();

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DS(ds);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(ds.Tables[0]));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 ds
                    ds = new DataSet();
                    ds.Tables.Add(new DataTable("rtn"));

                    // 自動生成Daoを実行
                    genDao.D2_Select(ds.Tables[0]);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(ds.Tables[0]));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DS(muParameter.ActionType, out ds);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(ds.Tables[0]));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region SelectAll_DR

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_SelectAll_DR(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            //DTRow dtrow_in = dtt_in.Rows[0];

            DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;

            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLText = "SELECT * FROM Shippers";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLText = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
                            break;
                    }

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // ３列生成
                    dt.Columns.Add("ShipperID", System.Type.GetType("System.String"));
                    dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
                    dt.Columns.Add("Phone", System.Type.GetType("System.String"));

                    // 共通Daoを実行
                    IDataReader idr = cmnDao.ExecSelect_DR();

                    while (idr.Read())
                    {
                        // DRから読む
                        object[] objArray = new object[3];
                        idr.GetValues(objArray);

                        // DTに設定する。
                        DataRow dr = dt.NewRow();
                        dr.ItemArray = objArray;
                        dt.Rows.Add(dr);
                    }

                    // 終了したらクローズ
                    idr.Close();

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // DRのI/Fなし

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 自動生成Daoを実行
                    genDao.D2_Select(dt);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DR(muParameter.ActionType, out dt);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region SelectAll_DSQL

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_SelectAll_DSQL(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            DTRow dtrow_in = dtt_in.Rows[0];

            DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;

            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelectOrder.sql";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelectOrder.xml";
                            break;
                    }

                    // ユーザ定義パラメタに対して、動的に値を設定する。
                    string orderColumn = "";
                    string orderSequence = "";

                    if (dtrow_in["OrderColumn"].ToString() == "c1")
                    {
                        orderColumn = "ShipperID";
                    }
                    else if (dtrow_in["OrderColumn"].ToString() == "c2")
                    {
                        orderColumn = "CompanyName";
                    }
                    else if (dtrow_in["OrderColumn"].ToString() == "c3")
                    {
                        orderColumn = "Phone";
                    }
                    else { }

                    if (dtrow_in["OrderSequence"].ToString() == "A")
                    {
                        orderSequence = "ASC";
                    }
                    else if (dtrow_in["OrderSequence"].ToString() == "D")
                    {
                        orderSequence = "DESC";
                    }
                    else { }

                    // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P1", "test");

                    // ユーザ入力は指定しない。
                    // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                    //    必要であれば、前後の空白を明示的に指定する必要がある。
                    cmnDao.SetUserParameter("COLUMN", " " + orderColumn + " ");
                    cmnDao.SetUserParameter("SEQUENCE", " " + orderSequence + " ");

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                //case "generate": // 自動生成Daoを使用する。
                //    // 当該SQLなし
                //    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DSQL(
                        muParameter.ActionType,
                        dtrow_in["OrderColumn"].ToString(),
                        dtrow_in["OrderSequence"].ToString(), out dt);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(DTTable.FromDataTable(dt));
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region Select

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_Select(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            //DTTables dtts_in = Marshalling.StringToDTTables(muParameter.Value);
            //DTTable dtt_in = dtts_in[0];
            //DTRow dtrow_in = dtt_in.Rows[0];

            DTTables dtts_out = null;
            DTTable dtt_out = null;
            DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;

            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelect.sql";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelect.xml";
                            break;
                    }

                    // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P1", muParameter.Value);

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // キャストの対策コードを挿入
                    dtt_out = new DTTable("ret");
                    dtt_out.Cols.Add(new DTColumn("ShipperID", DTType.Int32));
                    dtt_out.Cols.Add(new DTColumn("CompanyName", DTType.String));
                    dtt_out.Cols.Add(new DTColumn("Phone", DTType.String));
                    dtrow_out = dtt_out.Rows.AddNew();

                    // ・SQLの場合、ShipperIDのintがInt32型にマップされる。
                    // ・ODPの場合、ShipperIDのNUMBERがInt64型にマップされる。
                    // ・DB2の場合、ShipperIDのDECIMALがｘｘｘ型にマップされる。
                    if (dt.Rows[0].ItemArray.GetValue(0).GetType().ToString() == "System.Int32")
                    {
                        // Int32なのでキャスト
                        dtrow_out["ShipperID"] = (int)dt.Rows[0].ItemArray.GetValue(0);
                    }
                    else
                    {
                        // それ以外の場合、一度、文字列に変換してInt32.Parseする。
                        dtrow_out["ShipperID"] = int.Parse(dt.Rows[0].ItemArray.GetValue(0).ToString());
                    }

                    dtrow_out["CompanyName"] = (string)dt.Rows[0].ItemArray.GetValue(1);
                    dtrow_out["Phone"] = (string)dt.Rows[0].ItemArray.GetValue(2);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(dtt_out);
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = muParameter.Value;

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 自動生成Daoを実行
                    genDao.S2_Select(dt);

                    // キャストの対策コードを挿入
                    dtt_out = new DTTable("ret");
                    dtt_out.Cols.Add(new DTColumn("ShipperID", DTType.Int32));
                    dtt_out.Cols.Add(new DTColumn("CompanyName", DTType.String));
                    dtt_out.Cols.Add(new DTColumn("Phone", DTType.String));
                    dtrow_out = dtt_out.Rows.AddNew();

                    // ・SQLの場合、ShipperIDのintがInt32型にマップされる。
                    // ・ODPの場合、ShipperIDのNUMBERがInt64型にマップされる。
                    // ・DB2の場合、ShipperIDのDECIMALがｘｘｘ型にマップされる。
                    if (dt.Rows[0].ItemArray.GetValue(0).GetType().ToString() == "System.Int32")
                    {
                        // Int32なのでキャスト
                        dtrow_out["ShipperID"] = (int)dt.Rows[0].ItemArray.GetValue(0);
                    }
                    else
                    {
                        // それ以外の場合、一度、文字列に変換してInt32.Parseする。
                        dtrow_out["ShipperID"] = int.Parse(dt.Rows[0].ItemArray.GetValue(0).ToString());
                    }

                    dtrow_out["CompanyName"] = (string)dt.Rows[0].ItemArray.GetValue(1);
                    dtrow_out["Phone"] = (string)dt.Rows[0].ItemArray.GetValue(2);

                    // 戻り値をマーシャリングして設定
                    dtts_out = new DTTables();
                    dtts_out.Add(dtt_out);
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    string companyName;
                    string phone;

                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Select(muParameter.ActionType, muParameter.Value,
                        out companyName, out phone);

                    // 戻り値をマーシャリングして設定
                    dtt_out = new DTTable("ret");
                    dtt_out.Cols.Add(new DTColumn("ShipperID", DTType.Int32));
                    dtt_out.Cols.Add(new DTColumn("CompanyName", DTType.String));
                    dtt_out.Cols.Add(new DTColumn("Phone", DTType.String));
                    dtrow_out = dtt_out.Rows.AddNew();

                    dtrow_out["ShipperID"] = int.Parse(muParameter.Value);
                    dtrow_out["CompanyName"] = companyName;
                    dtrow_out["Phone"] = phone;
                    
                    dtts_out = new DTTables();
                    dtts_out.Add(dtt_out);
                    muReturn.Value = DTTables.DTTablesToString(dtts_out);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region Insert

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_Insert(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            DTRow dtrow_in = dtt_in.Rows[0];

            //DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            
            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    cmnDao.SQLFileName = "ShipperInsert.sql";

                    // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P2", dtrow_in["CompanyName"]);
                    cmnDao.SetParameter("P3", dtrow_in["Phone"]);

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Value = cmnDao.ExecInsUpDel_NonQuery().ToString();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.CompanyName = dtrow_in["CompanyName"];
                    genDao.Phone = dtrow_in["Phone"];

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    muReturn.Value = genDao.D1_Insert().ToString();

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    // 戻り値を設定
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Insert(muParameter.ActionType,
                        (string)dtrow_in["CompanyName"],
                        (string)dtrow_in["Phone"],
                        out muReturn.Value);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region Update

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_Update(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            DTRow dtrow_in = dtt_in.Rows[0];

            //DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            
            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperUpdate.sql";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperUpdate.xml";
                            break;
                    }

                    // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P1", dtrow_in["ShipperID"]);
                    cmnDao.SetParameter("P2", dtrow_in["CompanyName"]);
                    cmnDao.SetParameter("P3", dtrow_in["Phone"]);

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Value = cmnDao.ExecInsUpDel_NonQuery().ToString();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = dtrow_in["ShipperID"];
                    genDao.Set_CompanyName_forUPD = dtrow_in["CompanyName"];
                    genDao.Set_Phone_forUPD = dtrow_in["Phone"];

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    muReturn.Value = genDao.S3_Update().ToString();

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    // 戻り値を設定
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Update(muParameter.ActionType, 
                        (string)dtrow_in["ShipperID"],
                        (string)dtrow_in["CompanyName"],
                        (string)dtrow_in["Phone"],
                        out muReturn.Value);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #region Delete

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_Delete(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            /// 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            //DTTable dtt_in = dtts_in[0];
            //DTRow dtrow_in = dtt_in.Rows[0];

            //DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // ↓業務処理-----------------------------------------------------
            
            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((muParameter.ActionType.Split('%'))[2])
                    {
                        case "static":
                            // 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperDelete.sql";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperDelete.xml";
                            break;
                    }

                    // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P1", muParameter.Value);

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Value = cmnDao.ExecInsUpDel_NonQuery().ToString();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = muParameter.Value;

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    muReturn.Value = genDao.S4_Delete().ToString();

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    // 戻り値を設定
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Delete(muParameter.ActionType, muParameter.Value, out muReturn.Value);

                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(muParameter);
        }

        #endregion

        #endregion

        #region ロールバックのテスト

        /// <summary>ロールバックのテスト</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void TestRollback(MuParameterValue muParameter)
        {
            switch ((muParameter.ActionType.Split('%'))[3])
            {
                case "Business":

                    // 戻り値が見えるか確認する。
                    ((MuReturnValue)this.ReturnValue).Value = "戻り値が戻るか？";

                    // 業務例外のスロー
                    throw new BusinessApplicationException(
                        "ロールバックのテスト",
                        "ロールバックのテスト",
                        "エラー情報");
                //break; // 到達できないためコメントアウト

                case "System":

                    // 戻り値が見えるか確認する。
                    ((MuReturnValue)this.ReturnValue).Value = "戻り値が戻るか？";

                    // システム例外のスロー
                    throw new BusinessSystemException(
                        "ロールバックのテスト",
                        "ロールバックのテスト");
                //break; // 到達できないためコメントアウト

                case "Other":

                    // 戻り値が見えるか確認する。
                    ((MuReturnValue)this.ReturnValue).Value = "戻り値が戻るか？";

                    // その他、一般的な例外のスロー
                    throw new Exception("ロールバックのテスト");
                //break; // 到達できないためコメントアウト

                case "Other-Business":
                    // 戻り値が見えるか確認する。
                    ((MuReturnValue)this.ReturnValue).Value = "戻り値が戻るか？";

                    // その他、一般的な例外（業務例外へ振り替え）のスロー
                    throw new Exception("Other-Business");
                //break; // 到達できないためコメントアウト

                case "Other-System":

                    // 戻り値が見えるか確認する。
                    ((MuReturnValue)this.ReturnValue).Value = "戻り値が戻るか？";

                    // その他、一般的な例外（システム例外へ振り替え）のスロー
                    throw new Exception("Other-System");
                //break; // 到達できないためコメントアウト
            }
        }

        #endregion

        #endregion

        #region DTOPage

        #region UOCメソッド

        /// <summary>Silverlight＆汎用DTOのテスト（DataGrid初期化処理）</summary>
        /// <param name="muParameter">引数クラス</param>
        protected void UOC_InitDataGrid(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            //DTTables dtts_in = Marshalling.StringToDTTables(muParameter.Value);
            //DTTable dtt_in = dtts_in[0];
            //DTRow dtrow_in = dtt_in.Rows[0];

            DTTables dtts_out = null;
            DTTable dtt_out = null;
            DTRow dtrow_out = null;

            // テーブル生成＠汎用DTO
            dtt_out = new DTTable("test");

            // カラム定義＠汎用DTOテーブル
            dtt_out.Cols.Add(new DTColumn("boolVal", DTType.Boolean));
            dtt_out.Cols.Add(new DTColumn("charVal", DTType.Char));
            dtt_out.Cols.Add(new DTColumn("dateVal", DTType.DateTime));
            dtt_out.Cols.Add(new DTColumn("decimalVal", DTType.Decimal));
            dtt_out.Cols.Add(new DTColumn("doubleVal", DTType.Double));
            dtt_out.Cols.Add(new DTColumn("shortVal", DTType.Int16));
            dtt_out.Cols.Add(new DTColumn("intVal", DTType.Int32));
            dtt_out.Cols.Add(new DTColumn("longVal", DTType.Int64));
            dtt_out.Cols.Add(new DTColumn("singleVal", DTType.Single));
            dtt_out.Cols.Add(new DTColumn("stringVal", DTType.String));

            // 行追加＠汎用DTOテーブル
            
            // 1行目
            dtrow_out = dtt_out.Rows.AddNew();
            dtrow_out["boolVal"] = true;
            dtrow_out["charVal"] = 'a';
            dtrow_out["dateVal"] = new DateTime(1977, 7, 22, 10, 20, 30, 444);
            dtrow_out["decimalVal"] = 10000;
            dtrow_out["doubleVal"] = 3.55D;
            dtrow_out["shortVal"] = 100;
            dtrow_out["intVal"] = 1000000;
            dtrow_out["longVal"] = 1000000000000;
            dtrow_out["singleVal"] = 3.5f;
            dtrow_out["stringVal"] = "test";

            // 2行目
            dtrow_out = dtt_out.Rows.AddNew();
            dtrow_out["boolVal"] = false;
            dtrow_out["charVal"] = 'b';
            dtrow_out["dateVal"] = new DateTime(1976, 4, 23, 10, 20, 30, 444);
            dtrow_out["decimalVal"] = 20000;
            dtrow_out["doubleVal"] = 6.11D;
            dtrow_out["shortVal"] = 200;
            dtrow_out["intVal"] = 2000000;
            dtrow_out["longVal"] = 2000000000000;
            dtrow_out["singleVal"] = 6.5f;
            dtrow_out["stringVal"] = "test2";

            // 3行目
            dtrow_out = dtt_out.Rows.AddNew();
            dtrow_out["boolVal"] = true;
            dtrow_out["charVal"] = 'c';
            dtrow_out["dateVal"] = new DateTime(1975, 1, 1, 10, 20, 30, 444);
            dtrow_out["decimalVal"] = 30000;
            dtrow_out["doubleVal"] = 8.25D;
            dtrow_out["shortVal"] = 300;
            dtrow_out["intVal"] = 3000000;
            dtrow_out["longVal"] = 3000000000000;
            dtrow_out["singleVal"] = 7.2f;
            dtrow_out["stringVal"] = "test3";

            // ここで変更を確定させる
            dtt_out.AcceptChanges();

            // 戻り値をマーシャリングして設定
            dtts_out = new DTTables();
            dtts_out.Add(dtt_out);
            ((MuReturnValue)this.ReturnValue).Value = DTTables.DTTablesToString(dtts_out);
        }

        /// <summary>Silverlight＆汎用DTOのテスト（行追加）</summary>
        /// <param name="muParameter">引数クラス</param>
        protected void UOC_AddRow(MuParameterValue muParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DTTables dtts_in = DTTables.StringToDTTables(muParameter.Value);
            DTTable dtt_in = dtts_in[0];
            DTRow dtrow_in = null;// dtt_in.Rows[0];

            //DTTables dtts_out = null;
            //DTTable dtt_out = null;
            //DTRow dtrow_out = null;

            // 1行追加
            dtrow_in = dtt_in.Rows.AddNew();
            dtrow_in["boolVal"] = true;
            dtrow_in["charVal"] = 'z';
            dtrow_in["dateVal"] = new DateTime(1946, 12, 11, 10, 20, 30, 444);
            dtrow_in["decimalVal"] = 99999;
            dtrow_in["doubleVal"] = 9.99D;
            dtrow_in["shortVal"] = 900;
            dtrow_in["intVal"] = 9000000;
            dtrow_in["longVal"] = 9000000000000;
            dtrow_in["singleVal"] = 9.9f;
            dtrow_in["stringVal"] = "test" + dtt_in.Rows.Count.ToString();

            // 戻り値をマーシャリングして設定
            ((MuReturnValue)this.ReturnValue).Value = DTTables.DTTablesToString(dtts_in);
        }

        #endregion

        #endregion
    }
}
