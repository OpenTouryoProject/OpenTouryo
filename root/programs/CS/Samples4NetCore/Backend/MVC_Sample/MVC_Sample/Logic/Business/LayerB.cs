//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：LayerB
//* クラス日本語名  ：Ｂ層のテスト
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using MVC_Sample.Logic.Dao;
using MVC_Sample.Logic.Common;
using MVC_Sample.Models.ViewModels;

using System;
using System.Data;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Dto;

namespace MVC_Sample.Logic.Business
{
    public class LayerB : MyFcBaseLogic
    {
        #region テンプレ

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_メソッド名(TestParameterValue testParameter)
        { //メソッド引数にBaseParameterValueの派生の型を定義可能。

            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------

            // 個別Dao
            LayerD myDao = new LayerD(this.GetDam());
            //myDao.xxxx(testParameter, ref testReturn);

            // 共通Dao
            CmnDao cmnDao = new CmnDao(this.GetDam());
            cmnDao.ExecSelectScalar();

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region UOCメソッド

        #region SelectCount

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_SelectCount(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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
                    testReturn.Obj = cmnDao.ExecSelectScalar();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 共通Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = genDao.D5_SelCnt();

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectCount(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region SelectAll_DT

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_SelectAll_DT(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;
            List<ShipperViweModel> list = null;

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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
                    dt = new DataTable();

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // DataTableToList
                    list = DataToPoco.DataTableToList<ShipperViweModel>(dt);

                    // 戻り値を設定
                    testReturn.Obj = list;

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 dt
                    dt = new DataTable();

                    // 自動生成Daoを実行
                    genDao.D2_Select(dt);

                    // DataTableToList
                    list = DataToPoco.DataTableToList<ShipperViweModel>(dt);

                    // 戻り値を設定
                    testReturn.Obj = list;

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DT(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region SelectAll_DS

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_SelectAll_DS(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------
            DataSet ds = null;
            //DataTable dt = null;
            List<ShipperViweModel> list = null;

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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

                    // DataTableToList
                    list = DataToPoco.DataTableToList<ShipperViweModel>(ds.Tables[0]);

                    // 戻り値を設定
                    testReturn.Obj = list;

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 ds
                    ds = new DataSet();
                    ds.Tables.Add(new DataTable());

                    // 自動生成Daoを実行
                    genDao.D2_Select(ds.Tables[0]);

                    // DataTableToList
                    list = DataToPoco.DataTableToList<ShipperViweModel>(ds.Tables[0]);

                    // 戻り値を設定
                    testReturn.Obj = list;

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DS(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region SelectAll_DR

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_SelectAll_DR(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;
            List<ShipperViweModel> list = null;

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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

                    // 共通Daoを実行
                    IDataReader idr = cmnDao.ExecSelect_DR();

                    // DataReaderToList
                    list = DataToPoco.DataReaderToList<ShipperViweModel>(idr);

                    // 終了したらクローズ
                    idr.Close();

                    // 戻り値を設定
                    testReturn.Obj = list;

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // DRのI/Fなし

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 dt
                    dt = new DataTable();

                    // 自動生成Daoを実行
                    genDao.D2_Select(dt);

                    // DataTableToList
                    list = DataToPoco.DataTableToList<ShipperViweModel>(dt);

                    // 戻り値を設定
                    testReturn.Obj = list;

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DR(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region SelectAll_DSQL

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_SelectAll_DSQL(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;
            List<ShipperViweModel> list = null;

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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

                    if (testParameter.OrderColumn == "c1")
                    {
                        orderColumn = "ShipperID";
                    }
                    else if (testParameter.OrderColumn == "c2")
                    {
                        orderColumn = "CompanyName";
                    }
                    else if (testParameter.OrderColumn == "c3")
                    {
                        orderColumn = "Phone";
                    }
                    else { }

                    if (testParameter.OrderSequence == "A")
                    {
                        orderSequence = "ASC";
                    }
                    else if (testParameter.OrderSequence == "D")
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
                    dt = new DataTable();

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // DataTableToList
                    list = DataToPoco.DataTableToList<ShipperViweModel>(dt);

                    // 自動生成Daoを実行
                    testReturn.Obj = list;

                    break;

                //case "generate": // 自動生成Daoを使用する。
                //    // 当該SQLなし
                //    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.SelectAll_DSQL(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region Select

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_Select(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------
            DataTable dt = null;

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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
                    cmnDao.SetParameter("P1", testParameter.Shipper.ShipperID);

                    // 戻り値 dt
                    dt = new DataTable();

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // DataTableToPOCO
                    testReturn.Obj = DataToPoco.DataTableToPOCO<ShipperViweModel>(dt);

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.Shipper.ShipperID;

                    // 戻り値 dt
                    dt = new DataTable();

                    // 自動生成Daoを実行
                    genDao.S2_Select(dt);

                    // DataTableToPOCO
                    testReturn.Obj = DataToPoco.DataTableToPOCO<ShipperViweModel>(dt);

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Select(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region Insert

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_Insert(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    cmnDao.SQLFileName = "ShipperInsert.sql";

                    // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P2", testParameter.Shipper.CompanyName);
                    cmnDao.SetParameter("P3", testParameter.Shipper.Phone);

                    // 共通Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.CompanyName = testParameter.Shipper.CompanyName;
                    genDao.Phone = testParameter.Shipper.Phone;

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = genDao.D1_Insert();

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Insert(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region Update

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_Update(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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
                    cmnDao.SetParameter("P1", testParameter.Shipper.ShipperID);
                    cmnDao.SetParameter("P2", testParameter.Shipper.CompanyName);
                    cmnDao.SetParameter("P3", testParameter.Shipper.Phone);

                    // 共通Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.Shipper.ShipperID;
                    genDao.Set_CompanyName_forUPD = testParameter.Shipper.CompanyName;
                    genDao.Set_Phone_forUPD = testParameter.Shipper.Phone;

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = genDao.S3_Update();

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Update(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #region Delete

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_Delete(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------

            switch ((testParameter.ActionType.Split('%'))[1])
            {
                case "common": // 共通Daoを使用する。

                    // 共通Daoを生成
                    CmnDao cmnDao = new CmnDao(this.GetDam());

                    switch ((testParameter.ActionType.Split('%'))[2])
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
                    cmnDao.SetParameter("P1", testParameter.Shipper.ShipperID);

                    // 共通Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.Shipper.ShipperID;

                    // 自動生成Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = genDao.S4_Delete();

                    break;

                default: // 個別Daoを使用する。
                    LayerD myDao = new LayerD(this.GetDam());
                    myDao.Delete(testParameter, testReturn);
                    break;
            }

            // ↑業務処理-----------------------------------------------------

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        #endregion

        #endregion

        #region ロールバックのテスト

        /// <summary>ロールバックのテスト</summary>
        /// <param name="testParameter">引数クラス</param>
        private void TestRollback(TestParameterValue testParameter)
        {
            switch ((testParameter.ActionType.Split('%'))[3])
            {

                case "Business":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // 業務例外のスロー
                    throw new BusinessApplicationException(
                        "ロールバックのテスト",
                        "ロールバックのテスト",
                        "エラー情報");
                //break; // 到達できないためコメントアウト

                case "System":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // システム例外のスロー
                    throw new BusinessSystemException(
                        "ロールバックのテスト",
                        "ロールバックのテスト");
                //break; // 到達できないためコメントアウト

                case "Other":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // その他、一般的な例外のスロー
                    throw new Exception("ロールバックのテスト");
                //break; // 到達できないためコメントアウト

                case "Other-Business":
                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // その他、一般的な例外（業務例外へ振り替え）のスロー
                    throw new Exception("Other-Business");
                //break; // 到達できないためコメントアウト

                case "Other-System":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // その他、一般的な例外（システム例外へ振り替え）のスロー
                    throw new Exception("Other-System");
                //break; // 到達できないためコメントアウト
            }
        }

        #endregion
    }
}