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
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using SPA_Sample.Codes.Common;
using SPA_Sample.Codes.Dao;

namespace SPA_Sample.Codes.Business
{
    public class LayerB : MyFcBaseLogic
    {
        #region テンプレ

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        private void UOC_メソッド名(TestParameterValue testParameter)
        { //メソッド引数にBaseParameterValueの派生の型を定義可能。

            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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

                    // 戻り値を設定
                    testReturn.Obj = dt;

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 dt
                    dt = new DataTable();

                    // 自動生成Daoを実行
                    genDao.D2_Select(dt);

                    // 戻り値を設定
                    testReturn.Obj = (DataTable)dt;
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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            // ↓業務処理-----------------------------------------------------
            DataSet ds = null;

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

                    // 戻り値を設定
                    testReturn.Obj = ds;

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 ds
                    ds = new DataSet();
                    ds.Tables.Add(new DataTable());

                    // 自動生成Daoを実行
                    genDao.D2_Select(ds.Tables[0]);

                    // 戻り値を設定
                    testReturn.Obj = ds;
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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
                            cmnDao.SQLText = "SELECT * FROM Shippers";
                            break;

                        case "dynamic":
                            // 動的SQLを指定
                            cmnDao.SQLText = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
                            break;
                    }

                    // 戻り値 dt
                    dt = new DataTable();

                    // ３列生成
                    dt.Columns.Add("c1", System.Type.GetType("System.String"));
                    dt.Columns.Add("c2", System.Type.GetType("System.String"));
                    dt.Columns.Add("c3", System.Type.GetType("System.String"));

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

                    // 戻り値を設定
                    testReturn.Obj = dt;

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // DRのI/Fなし

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // 戻り値 dt
                    dt = new DataTable();

                    // 自動生成Daoを実行
                    genDao.D2_Select(dt);

                    // 戻り値を設定
                    testReturn.Obj = (DataTable)dt;

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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
                    DataTable dt = new DataTable();

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // 自動生成Daoを実行
                    testReturn.Obj = dt;

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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
                    cmnDao.SetParameter("P1", testParameter.ShipperID);

                    // 戻り値 dt
                    dt = new DataTable();

                    // 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt);

                    // キャストの対策コードを挿入

                    // ・SQLの場合、ShipperIDのintがInt32型にマップされる。
                    // ・ODPの場合、ShipperIDのNUMBERがInt64型にマップされる。
                    // ・DB2の場合、ShipperIDのDECIMALがｘｘｘ型にマップされる。
                    if (dt.Rows[0].ItemArray.GetValue(0).GetType().ToString() == "System.Int32")
                    {
                        // Int32なのでキャスト
                        testReturn.ShipperID = (int)dt.Rows[0].ItemArray.GetValue(0);
                    }
                    else
                    {
                        // それ以外の場合、一度、文字列に変換してInt32.Parseする。
                        testReturn.ShipperID = int.Parse(dt.Rows[0].ItemArray.GetValue(0).ToString());
                    }

                    testReturn.CompanyName = (string)dt.Rows[0].ItemArray.GetValue(1);
                    testReturn.Phone = (string)dt.Rows[0].ItemArray.GetValue(2);

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.ShipperID;

                    // 戻り値 dt
                    dt = new DataTable();

                    // 自動生成Daoを実行
                    genDao.S2_Select(dt);

                    // キャストの対策コードを挿入

                    // ・SQLの場合、ShipperIDのintがInt32型にマップされる。
                    // ・ODPの場合、ShipperIDのNUMBERがInt64型にマップされる。
                    // ・DB2の場合、ShipperIDのDECIMALがｘｘｘ型にマップされる。
                    if (dt.Rows[0].ItemArray.GetValue(0).GetType().ToString() == "System.Int32")
                    {
                        // Int32なのでキャスト
                        testReturn.ShipperID = (int)dt.Rows[0].ItemArray.GetValue(0);
                    }
                    else
                    {
                        // それ以外の場合、一度、文字列に変換してInt32.Parseする。
                        testReturn.ShipperID = int.Parse(dt.Rows[0].ItemArray.GetValue(0).ToString());
                    }

                    testReturn.CompanyName = (string)dt.Rows[0].ItemArray.GetValue(1);
                    testReturn.Phone = (string)dt.Rows[0].ItemArray.GetValue(2);

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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
                    cmnDao.SetParameter("P2", testParameter.CompanyName);
                    cmnDao.SetParameter("P3", testParameter.Phone);

                    // 共通Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.CompanyName = testParameter.CompanyName;
                    genDao.Phone = testParameter.Phone;

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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
                    cmnDao.SetParameter("P1", testParameter.ShipperID);
                    cmnDao.SetParameter("P2", testParameter.CompanyName);
                    cmnDao.SetParameter("P3", testParameter.Phone);

                    // 共通Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.ShipperID;
                    genDao.Set_CompanyName_forUPD = testParameter.CompanyName;
                    genDao.Set_Phone_forUPD = testParameter.Phone;

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
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
                    cmnDao.SetParameter("P1", testParameter.ShipperID);

                    // 共通Daoを実行
                    // 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

                    break;

                case "generate": // 自動生成Daoを使用する。

                    // 自動生成Daoを生成
                    DaoShippers genDao = new DaoShippers(this.GetDam());

                    // パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.ShipperID;

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