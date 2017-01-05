//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：LayerB_rt
//* クラス日本語名  ：Ｂ層のテスト（REST汎用Webメソッドのテスト用）
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

using Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest;

using WinStore_sample.Web.Dao;

namespace WinStore_sample.Web.Business
{
    /// <summary>
    /// LayerB_rt の概要の説明です
    /// </summary>
    public class LayerB_rt : MyFcBaseLogic
    {
        #region CRUDPage

        #region テンプレ

        /// <summary>業務処理を実装</summary>
        /// <param name="muParameter">汎用引数クラス</param>
        private void UOC_メソッド名(MuParameterValue muParameter)
        { //メソッド引数にBaseParameterValueの派生の型を定義可能。

            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            MuReturnValue muReturn = new MuReturnValue();
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            DataTable dt = DataContractHelper.ToDataTable(((Informations)muParameter.Bean).DicList);

            // ↓業務処理-----------------------------------------------------

            // 個別Dao
            LayerD myDao = new LayerD(this.GetDam());
            //myDao.xxxx(muParameter.ActionType, dtts, muReturn);

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());
            cmnDao.ExecSelectScalar();

            // 戻り値をマーシャリングして設定
            muReturn.Bean = new Informations("");
            muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

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
                    muReturn.Bean = new Informations(cmnDao.ExecSelectScalar().ToString());

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Bean = new Informations(genDao.D5_SelCnt().ToString());

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    string ret = "";
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectCount(muParameter.ActionType, out ret);

                    // 戻り値を設定
                    muReturn.Bean = new Informations(ret);

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

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
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 自動生成Daoを実行
                    genDao.D2_Select(dt);

                    // 戻り値をマーシャリングして設定
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DT(muParameter.ActionType, out dt);

                    // 戻り値をマーシャリングして設定
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

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
                    muReturn.Bean = new Informations(DataContractHelper.ToList(ds.Tables[0]));

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
                    muReturn.Bean = new Informations(DataContractHelper.ToList(ds.Tables[0]));

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DS(muParameter.ActionType, out ds);

                    // 戻り値をマーシャリングして設定
                    muReturn.Bean = new Informations(DataContractHelper.ToList(ds.Tables[0]));

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

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
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

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
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DR(muParameter.ActionType, out dt);

                    // 戻り値をマーシャリングして設定
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            Dictionary<string, string> dic = ((Informations)muParameter.Bean).Dictionary;

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

                    if (dic["OrderColumn"].ToString() == "c1")
                    {
                        orderColumn = "ShipperID";
                    }
                    else if (dic["OrderColumn"].ToString() == "c2")
                    {
                        orderColumn = "CompanyName";
                    }
                    else if (dic["OrderColumn"].ToString() == "c3")
                    {
                        orderColumn = "Phone";
                    }
                    else { }

                    if (dic["OrderSequence"].ToString() == "A")
                    {
                        orderSequence = "ASC";
                    }
                    else if (dic["OrderSequence"].ToString() == "D")
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
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

                    break;

                //case "generate": // 自動生成Daoを使用する。
                //    // 当該SQLなし
                //    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DSQL(
                        muParameter.ActionType,
                        dic["OrderColumn"].ToString(),
                        dic["OrderSequence"].ToString(), out dt);

                    // 戻り値をマーシャリングして設定
                    muReturn.Bean = new Informations(DataContractHelper.ToList(dt));

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            string shipperID = ((Informations)muParameter.Bean).Str;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;
            Dictionary<string, string> dic = null;

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
                    cmnDao.SetParameter("P1", shipperID);

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // 戻り値を設定
                    dic = new Dictionary<string, string>();
                    dic["ShipperID"] = dt.Rows[0].ItemArray.GetValue(0).ToString();
                    dic["CompanyName"] = (string)dt.Rows[0].ItemArray.GetValue(1);
                    dic["Phone"] = (string)dt.Rows[0].ItemArray.GetValue(2);
                    muReturn.Bean = new Informations(dic);

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = shipperID;

                    // 戻り値 dt
                    dt = new DataTable("rtn");

                    // 自動生成Daoを実行
                    genDao.S2_Select(dt);

                    // 戻り値を設定
                    dic = new Dictionary<string, string>();
                    dic["ShipperID"] = dt.Rows[0].ItemArray.GetValue(0).ToString();
                    dic["CompanyName"] = (string)dt.Rows[0].ItemArray.GetValue(1);
                    dic["Phone"] = (string)dt.Rows[0].ItemArray.GetValue(2);
                    muReturn.Bean = new Informations(dic);

                    break;

                default: // 個別Daoを使用する。
                    string companyName;
                    string phone;

                    // 個別Daoを実行
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Select(muParameter.ActionType, shipperID,
                        out companyName, out phone);

                    // 戻り値を設定
                    dic = new Dictionary<string, string>();
                    dic["CompanyName"] = companyName;
                    dic["Phone"] = phone;
                    muReturn.Bean = new Informations(dic);

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            Dictionary<string, string> dic = ((Informations)muParameter.Bean).Dictionary;

            // ↓業務処理-----------------------------------------------------
            
            switch ((muParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    cmnDao.SQLFileName = "ShipperInsert.sql";

                    // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P2", dic["CompanyName"]);
                    cmnDao.SetParameter("P3", dic["Phone"]);

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Bean = new Informations(cmnDao.ExecInsUpDel_NonQuery().ToString());

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.CompanyName = dic["CompanyName"];
                    genDao.Phone = dic["Phone"];

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    muReturn.Bean = new Informations(genDao.D1_Insert().ToString());

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    string ret = "";
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Insert(muParameter.ActionType,
                        dic["CompanyName"],
                        dic["Phone"],
                        out ret);

                    // 戻り値を設定
                    muReturn.Bean = new Informations(ret);

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            Dictionary<string, string> dic = ((Informations)muParameter.Bean).Dictionary;

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
                    cmnDao.SetParameter("P1", dic["ShipperID"]);
                    cmnDao.SetParameter("P2", dic["CompanyName"]);
                    cmnDao.SetParameter("P3", dic["Phone"]);

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Bean = new Informations(cmnDao.ExecInsUpDel_NonQuery().ToString());

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = dic["ShipperID"];
                    genDao.Set_CompanyName_forUPD = dic["CompanyName"];
                    genDao.Set_Phone_forUPD = dic["Phone"];

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    muReturn.Bean = new Informations(genDao.S3_Update().ToString());

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    string ret = "";
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Update(muParameter.ActionType,
                        dic["ShipperID"],
                        dic["CompanyName"],
                        dic["Phone"],
                        out ret);

                    // 戻り値を設定
                    muReturn.Bean = new Informations(ret);

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
            muReturn.Bean = new Informations("");
            this.ReturnValue = muReturn;

            // 引数をアンマーシャル
            string shipperID = ((Informations)muParameter.Bean).Str;

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
                    cmnDao.SetParameter("P1", shipperID);

                    // 共通Daoを実行
                    // 戻り値を設定
                    muReturn.Bean = new Informations(cmnDao.ExecInsUpDel_NonQuery().ToString());

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = shipperID;

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    muReturn.Bean = new Informations(genDao.S4_Delete().ToString());

                    break;

                default: // 個別Daoを使用する。

                    // 個別Daoを実行
                    string ret = "";
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Delete(muParameter.ActionType, shipperID, out ret);

                    // 戻り値を設定
                    muReturn.Bean = new Informations(ret);

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
    }
}
