//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：LayerB.cs
//* クラス日本語名  ：
//*
//* 作成者          ：Rituparna & Santosh
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  06/24/2014   Rituparna & Santosh   Testcode development for CRUDTest(Public classes).
//*  07/02/2014   Santosh               Added code and modified test cases to prevent database changes after running the test cases
//*  07/04/2014   Rituparna             Added code and modified test cases to Increase the code coverage of BaseDam.cs Class
//*  07/10/2014   Rituparna             Added code and modified test cases to Increase the code coverage of BaseDam.cs Class
//**********************************************************************************

// 型情報
using MyType;

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
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

/// <summary>
/// LayerB の概要の説明です
/// </summary>
public class LayerB : MyFcBaseLogic
{

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
            case "generate": // 自動生成Daoを使用する。             
                break;
            case "common": // 共通Daoを使用する。
            default:
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
            case "generate": // 自動生成Daoを使用する。              
                break;

            case "common": // 共通Daoを使用する。
            default:
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
            case "generate": // 自動生成Daoを使用する。          
                break;
            case "common": // 共通Daoを使用する。
            default:
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
            case "generate": // 自動生成Daoを使用する。
                break;
            case "common": // 共通Daoを使用する。
            default:
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
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion

    #region UOC_ArgumentException
    /// <summary>
    /// UOC_ArgumentException Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_ArgumentException(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------
        string orderColumn = "";
        string orderSequence = "";
        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        switch (testParameter.SelectCase)
                        {
                            case "ArgumentException0":
                                cmnDao.SQLFileName = "ArgumentException0.xml";
                                break;
                            case "ArgumentException1":
                                cmnDao.SQLFileName = "ArgumentException1.xml";
                                break;
                            case "ArgumentException2":
                                cmnDao.SQLFileName = "ArgumentException2.xml";
                                break;
                            case "ArgumentException3":
                                cmnDao.SQLFileName = "ArgumentException3.xml";
                                break;
                            case "ArgumentException4":
                                cmnDao.SQLFileName = "ArgumentException4.xml";
                                break;
                            case "ArgumentException5":
                                cmnDao.SQLFileName = "ArgumentException5.xml";
                                break;
                            case "ArgumentException6":
                                cmnDao.SQLFileName = "ArgumentException6.xml";
                                break;
                            case "ArgumentException7":
                                cmnDao.SQLFileName = "ArgumentException7.xml";
                                break;
                            case "ArgumentException8":
                                cmnDao.SQLFileName = "ArgumentException8.xml";
                                break;
                            case "ArgumentException9":
                                cmnDao.SQLFileName = "ArgumentException9.xml";
                                break;
                            case "ArgumentException10":
                                cmnDao.SQLFileName = "ArgumentException10.xml";
                                break;
                            case "ArgumentException11":
                                cmnDao.SQLFileName = "ArgumentException11.xml";
                                break;
                            case "ArgumentException12":
                                cmnDao.SQLFileName = "ArgumentException12.xml";
                                break;
                            case "ArgumentException13":
                                cmnDao.SQLFileName = "ArgumentException13.xml";
                                break;
                            case "ArgumentException14":
                                cmnDao.SQLFileName = "ArgumentException14.xml";
                                break;
                            case "ArgumentException15":
                                cmnDao.SQLFileName = "ArgumentException15.xml";
                                break;
                            case "ArgumentException16":
                                cmnDao.SQLFileName = "ArgumentException16.xml";
                                break;
                            case "ArgumentException17":
                                cmnDao.SQLFileName = "ArgumentException17.xml";
                                break;
                            case "ArgumentException18":
                                cmnDao.SQLFileName = "ArgumentException18.xml";
                                break;
                            case "ArgumentException19":
                                cmnDao.SQLFileName = "ArgumentException19.xml";
                                break;
                            case "ArgumentException20":
                                cmnDao.SQLFileName = "ArgumentException20.xml";
                                break;
                            case "ArgumentException21":
                                cmnDao.SQLFileName = "ArgumentException21.xml";
                                break;
                            case "ArgumentException22":
                                cmnDao.SQLFileName = "ArgumentException22.xml";
                                break;
                            case "ArgumentException23":
                                cmnDao.SQLFileName = "ArgumentException23.xml";
                                break;
                        }
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        switch (testParameter.SelectCase)
                        {
                            case "ArgumentException0":
                                cmnDao.SQLFileName = "ArgumentException0.xml";
                                break;
                            case "ArgumentException1":
                                cmnDao.SQLFileName = "ArgumentException1.xml";
                                break;
                            case "ArgumentException2":
                                cmnDao.SQLFileName = "ArgumentException2.xml";
                                break;
                            case "ArgumentException3":
                                cmnDao.SQLFileName = "ArgumentException3.xml";
                                break;
                            case "ArgumentException4":
                                cmnDao.SQLFileName = "ArgumentException5.xml";
                                break;
                            case "ArgumentException5":
                                cmnDao.SQLFileName = "ArgumentException4.xml";
                                break;
                            case "ArgumentException6":
                                cmnDao.SQLFileName = "ArgumentException6.xml";
                                break;
                            case "ArgumentException8":
                                cmnDao.SQLFileName = "ArgumentException8.xml";
                                break;
                            case "ArgumentException9":
                                cmnDao.SQLFileName = "ArgumentException9.xml";
                                break;
                            case "ArgumentException10":
                                cmnDao.SQLFileName = "ArgumentException10.xml";
                                break;
                            case "ArgumentException11":
                                cmnDao.SQLFileName = "ArgumentException11.xml";
                                break;
                            case "ArgumentException12":
                                cmnDao.SQLFileName = "ArgumentException12.xml";
                                break;
                            case "ArgumentException13":
                                cmnDao.SQLFileName = "ArgumentException13.xml";
                                break;
                            case "ArgumentException14":
                                cmnDao.SQLFileName = "ArgumentException14.xml";
                                break;
                            case "ArgumentException15":
                                cmnDao.SQLFileName = "ArgumentException15.xml";
                                break;
                            case "ArgumentException16":
                                cmnDao.SQLFileName = "ArgumentException16.xml";
                                break;
                            case "ArgumentException17":
                                cmnDao.SQLFileName = "ArgumentException17.xml";
                                break;
                            case "ArgumentException18":
                                cmnDao.SQLFileName = "ArgumentException18.xml";
                                break;
                            case "ArgumentException19":
                                cmnDao.SQLFileName = "ArgumentException19.xml";
                                break;
                            case "ArgumentException20":
                                cmnDao.SQLFileName = "ArgumentException20.xml";
                                break;
                            case "ArgumentException21":
                                cmnDao.SQLFileName = "ArgumentException21.xml";
                                break;
                            case "ArgumentException22":
                                cmnDao.SQLFileName = "ArgumentException22.xml";
                                break;
                            case "ArgumentException23":
                                cmnDao.SQLFileName = "ArgumentException23.xml";
                                break;
                        }
                        break;
                }
                switch (testParameter.SelectCase)
                {
                    case "ArgumentException0":
                        if (testParameter.OrderColumn == "c1")
                        {
                            orderColumn = "CompanyName";
                        }
                        else if (testParameter.OrderColumn == "c2")
                        {
                            orderColumn = "CompanyName";
                        }
                        else if (testParameter.OrderColumn == "c3")
                        {
                            orderColumn = "CompanyName";
                        }

                        if (testParameter.OrderSequence == "A")
                        {
                            orderSequence = "ASC";
                        }
                        else if (testParameter.OrderSequence == "D")
                        {
                            orderSequence = "DESC";
                        }

                        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                        cmnDao.SetParameter("P1", "test");
                        cmnDao.SetParameter("P2", "test");

                        // ユーザ入力は指定しない。
                        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                        //    必要であれば、前後の空白を明示的に指定する必要がある。
                        cmnDao.SetUserParameter("COLUMN", orderColumn);
                        cmnDao.SetUserParameter("SEQUENCE", orderSequence);

                        cmnDao.SetParameter("j1", true);
                        cmnDao.SetParameter("if1", true);
                        cmnDao.SetParameter("s1", true);

                        cmnDao.SetParameter("j2", true);
                        cmnDao.SetParameter("if2", true);
                        cmnDao.SetParameter("s2", true);

                        cmnDao.SetParameter("j3", true);
                        cmnDao.SetParameter("if3", true);
                        cmnDao.SetParameter("s3", true);

                        cmnDao.SetParameter("j4", true);
                        cmnDao.SetParameter("if4", true);
                        cmnDao.SetParameter("s4", true);

                        // 戻り値 ds
                        DataSet ds = new DataSet();
                        //   -- 一覧を返すSELECTクエリを実行する
                        cmnDao.ExecSelectFill_DS(ds);
                        // ↑DBアクセス-----------------------------------------------------

                        // 戻り値を設定
                        testReturn.Obj = ds;
                        break;

                    case "ArgumentException1":
                        // ユーザ定義パラメタに対して、動的に値を設定する。
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

                    case "ArgumentException2":
                        cmnDao.SetParameter("CompanyName", "test");
                        cmnDao.SetParameter("Phone", "testdescription");
                        //// 共通Daoを実行
                        //// 戻り値を設定
                        testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();
                        break;

                    case "ArgumentException3":
                        cmnDao.SetParameter("CompanyName", "test");
                        cmnDao.SetParameter("Phone", "testdescription");
                        //// 共通Daoを実行
                        //// 戻り値を設定
                        testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();
                        break;
                    case "ArgumentException4":
                        // ユーザ定義パラメタに対して、動的に値を設定する。


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
                        DataTable dt1 = new DataTable();

                        // 共通Daoを実行
                        cmnDao.ExecSelectFill_DT(dt1);

                        // 自動生成Daoを実行
                        testReturn.Obj = dt1;
                        break;
                    case "ArgumentException5":
                        cmnDao.SetParameter("CompanyName", "test");
                        cmnDao.SetParameter("Phone", "testdescription");
                        //// 共通Daoを実行
                        //// 戻り値を設定
                        testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();
                        break;
                    case "ArgumentException6":
                        cmnDao.SetParameter("CompanyName", "test");
                        cmnDao.SetParameter("Phone", "testdescription");
                        //// 共通Daoを実行
                        //// 戻り値を設定
                        testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();
                        break;
                    case "ArgumentException7":
                        cmnDao.SetParameter("ShipperID", testParameter.ShipperID);
                        cmnDao.SetParameter("Set_CompanyName_forUPD", testParameter.CompanyName);
                        cmnDao.SetParameter("Set_Phone_forUPD", testParameter.Phone);
                        testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();
                        break;
                    case "ArgumentException8":
                        cmnDao.SetParameter("sel", "a1");
                        DataTable dt2 = new DataTable();
                        // 共通Daoを実行
                        cmnDao.ExecSelectFill_DT(dt2);

                        // 自動生成Daoを実行
                        testReturn.Obj = dt2;
                        break;
                    case "ArgumentException9":
                        cmnDao.SetParameter("sel", "a1");
                        DataTable dt3 = new DataTable();
                        // 共通Daoを実行
                        cmnDao.ExecSelectFill_DT(dt3);

                        // 自動生成Daoを実行
                        testReturn.Obj = dt3;
                        break;
                    case "ArgumentException10":
                    case "ArgumentException21":
                        cmnDao.SetParameter("sel", "a1");
                        DataTable dt4 = new DataTable();
                        // 共通Daoを実行
                        cmnDao.ExecSelectFill_DT(dt4);

                        // 自動生成Daoを実行
                        testReturn.Obj = dt4;
                        break;
                    case "ArgumentException11":
                        cmnDao.SetParameter("up", 28.5);
                        cmnDao.SetParameter("uis", 113);
                        cmnDao.SetParameter("uoo", 0);
                        cmnDao.SetParameter("rol", 25);
                        cmnDao.SetParameter("bit", 25);
                        ArrayList arr = new ArrayList();
                        arr.Add(1);
                        arr.Add(2);
                        arr.Add(3);
                        arr.Add(4);
                        arr.Add(5);
                        arr.Add(6);
                        arr.Add(7);
                        arr.Add(8);
                        cmnDao.SetParameter("PLIST", arr);
                        // ユーザ入力は指定しない。
                        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                        //    必要であれば、前後の空白を明示的に指定する必要がある。
                        cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                        cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                        cmnDao.SetParameter("ISNOTNULL1", false);
                        cmnDao.SetParameter("ISNOTNULL2", false);
                        cmnDao.SetParameter("ISNOTNULL3", false);
                        // 戻り値 ds
                        // 戻り値 ds
                        DataSet ds1 = new DataSet();

                        //   -- 一覧を返すSELECTクエリを実行する
                        cmnDao.ExecSelectFill_DS(ds1);

                        // ↑DBアクセス-----------------------------------------------------

                        // 戻り値を設定
                        testReturn.Obj = ds1;
                        break;
                    case "ArgumentException12":
                        cmnDao.SetParameter("P", false);
                        DataSet ds2 = new DataSet();

                        //   -- 一覧を返すSELECTクエリを実行する
                        cmnDao.ExecSelectFill_DS(ds2);

                        // ↑DBアクセス-----------------------------------------------------

                        // 戻り値を設定
                        testReturn.Obj = ds2;
                        break;
                    case "ArgumentException13":
                        cmnDao.SetParameter("og", true);
                        DataSet ds3 = new DataSet();

                        //   -- 一覧を返すSELECTクエリを実行する
                        cmnDao.ExecSelectFill_DS(ds3);

                        // ↑DBアクセス-----------------------------------------------------

                        // 戻り値を設定
                        testReturn.Obj = ds3;
                        break;
                    case "ArgumentException14":
                    case "ArgumentException15":
                    case "ArgumentException16":

                        if (testParameter.OrderColumn == "c1")
                        {
                            orderColumn = "SUPPLIERID";
                        }
                        else if (testParameter.OrderColumn == "c2")
                        {
                            orderColumn = "SUPPLIERID";
                        }
                        else if (testParameter.OrderColumn == "c3")
                        {
                            orderColumn = "SUPPLIERID";
                        }

                        if (testParameter.OrderSequence == "A")
                        {
                            orderSequence = "ASC";
                        }
                        else if (testParameter.OrderSequence == "D")
                        {
                            orderSequence = "DESC";
                        }

                        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                        cmnDao.SetParameter("up", 28.5);
                        cmnDao.SetParameter("uis", 113);
                        cmnDao.SetParameter("uoo", 0);
                        cmnDao.SetParameter("rol", 25);
                        cmnDao.SetParameter("bit", 25);
                        ArrayList arr1 = new ArrayList();
                        arr1.Add(1);
                        arr1.Add(2);
                        arr1.Add(3);
                        arr1.Add(4);
                        arr1.Add(5);
                        arr1.Add(6);
                        arr1.Add(7);
                        arr1.Add(8);
                        cmnDao.SetParameter("PLIST", arr1);
                        // ユーザ入力は指定しない。
                        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                        //    必要であれば、前後の空白を明示的に指定する必要がある。
                        cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                        cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                        cmnDao.SetParameter("ISNOTNULL1", false);
                        cmnDao.SetParameter("ISNOTNULL2", false);
                        cmnDao.SetParameter("ISNOTNULL3", false);
                        // 戻り値 ds
                        // 戻り値 ds
                        DataSet ds5 = new DataSet();

                        //   -- 一覧を返すSELECTクエリを実行する
                        cmnDao.ExecSelectFill_DS(ds5);

                        // ↑DBアクセス-----------------------------------------------------

                        // 戻り値を設定
                        testReturn.Obj = ds5;
                        break;
                    case "ArgumentException17":
                    case "ArgumentException18":
                        cmnDao.SetParameter("sel", "a1");
                        // 戻り値 dt                
                        DataTable dt5 = new DataTable();
                        // 共通Daoを実行
                        cmnDao.ExecSelectFill_DT(dt5);

                        // 自動生成Daoを実行
                        testReturn.Obj = dt5;
                        break;
                    case "ArgumentException19":
                    case "ArgumentException20":
                        if (testParameter.OrderColumn == "c1")
                        {
                            orderColumn = "CompanyName";
                        }
                        else if (testParameter.OrderColumn == "c2")
                        {
                            orderColumn = "CompanyName";
                        }
                        else if (testParameter.OrderColumn == "c3")
                        {
                            orderColumn = "CompanyName";
                        }

                        if (testParameter.OrderSequence == "A")
                        {
                            orderSequence = "ASC";
                        }
                        else if (testParameter.OrderSequence == "D")
                        {
                            orderSequence = "DESC";
                        }

                        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                        cmnDao.SetParameter("P1", "test");
                        cmnDao.SetParameter("P2", "test1");

                        // ユーザ入力は指定しない。
                        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                        //    必要であれば、前後の空白を明示的に指定する必要がある。
                        cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                        cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                        cmnDao.SetParameter("j1", true);
                        cmnDao.SetParameter("j2", true);
                        // 戻り値 ds
                        DataSet ds4 = new DataSet();

                        //   -- 一覧を返すSELECTクエリを実行する
                        cmnDao.ExecSelectFill_DS(ds4);

                        // ↑DBアクセス-----------------------------------------------------

                        // 戻り値を設定
                        testReturn.Obj = ds4;
                        break;
                    case "ArgumentException22":
                    case "ArgumentException23":
                        cmnDao.SetParameter("og", 1);
                        // 戻り値 ds
                        DataSet ds6 = new DataSet();

                        //   -- 一覧を返すSELECTクエリを実行する
                        cmnDao.ExecSelectFill_DS(ds6);

                        // ↑DBアクセス-----------------------------------------------------

                        // 戻り値を設定
                        testReturn.Obj = ds6;
                        break;
                }
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
            default:
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
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
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
            case "generate": // 自動生成Daoを使用する。           
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());
                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        cmnDao.SQLFileName = "ShipperInsert.sql";

                        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                        cmnDao.SetParameter("P2", testParameter.CompanyName);
                        cmnDao.SetParameter("P3", testParameter.Phone);

                        // 共通Daoを実行
                        // 戻り値を設定
                        testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();
                        break;

                    case "dynamic":
                        cmnDao.SQLFileName = "ShipperInsert.xml";
                        ////// パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                    
                        cmnDao.SetParameter("CompanyName", "test");
                        cmnDao.SetParameter("Phone", "testdescription");
                        //// 共通Daoを実行
                        //// 戻り値を設定
                        testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

                        break;
                }
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
            case "generate": // 自動生成Daoを使用する。           
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "ShipperUpdate.sql";
                        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                        cmnDao.SetParameter("P1", testParameter.ShipperID);
                        cmnDao.SetParameter("P2", testParameter.CompanyName);
                        cmnDao.SetParameter("P3", testParameter.Phone);

                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "ShipperUpdate.xml";
                        cmnDao.SetParameter("ShipperID", testParameter.ShipperID);
                        cmnDao.SetParameter("Set_CompanyName_forUPD", testParameter.CompanyName);
                        cmnDao.SetParameter("Set_Phone_forUPD", testParameter.Phone);
                        break;
                }

                // 共通Daoを実行
                // 戻り値を設定
                testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery();

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
            case "generate": // 自動生成Daoを使用する。
                break;

            case "common": // 共通Daoを使用する。
            default:
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
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion
    #region UOC_SelectJoin0
    /// <summary>
    /// UOC_SelectJoin0 Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_SelectJoin0(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        // 共通Daoを生成
        CmnDao cmnDao = new CmnDao(this.GetDam());

        string orderColumn = "";
        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。              
            default:
                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "join_sub_where0.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "join_sub_where0.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。


                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "CompanyName";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                cmnDao.SetParameter("P1", "test");
                cmnDao.SetParameter("P2", "test");

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", orderColumn);

                cmnDao.SetParameter("j1", false);
                cmnDao.SetParameter("if1", true);
                cmnDao.SetParameter("s1", true);

                cmnDao.SetParameter("j2", false);
                cmnDao.SetParameter("if2", true);
                cmnDao.SetParameter("s2", true);

                cmnDao.SetParameter("j3", false);
                cmnDao.SetParameter("if3", true);
                cmnDao.SetParameter("s3", true);

                cmnDao.SetParameter("j4", false);
                cmnDao.SetParameter("if4", true);
                cmnDao.SetParameter("s4", true);

                // 戻り値 ds
                DataSet ds = new DataSet();
                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);
                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }
        this.TestRollback(testParameter);

    }
    #endregion

    #region UOC_SelectJoin1
    /// <summary>
    /// UOC_SelectJoin1 Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_SelectJoin1(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        // 共通Daoを生成
        CmnDao cmnDao = new CmnDao(this.GetDam());

        string orderColumn = "";
        string orderSequence = "";
        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。              
            default:
                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "join_sub_where1.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "join_sub_where1.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。


                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "CompanyName";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                cmnDao.SetParameter("P1", "test");
                cmnDao.SetParameter("P2", "test");

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", orderColumn);
                cmnDao.SetUserParameter("SEQUENCE", orderSequence);

                cmnDao.SetParameter("j1", true);
                cmnDao.SetParameter("if1", true);
                cmnDao.SetParameter("s1", true);

                cmnDao.SetParameter("j2", true);
                cmnDao.SetParameter("if2", true);
                cmnDao.SetParameter("s2", true);

                cmnDao.SetParameter("j3", true);
                cmnDao.SetParameter("if3", true);
                cmnDao.SetParameter("s3", true);

                cmnDao.SetParameter("j4", true);
                cmnDao.SetParameter("if4", true);
                cmnDao.SetParameter("s4", true);

                // 戻り値 ds
                DataSet ds = new DataSet();
                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);
                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }
        this.TestRollback(testParameter);

    }
    #endregion

    #region UOC_SelectJoin2
    /// <summary>
    /// UOC_SelectJoin2 Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_SelectJoin2(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "join_sub_where2.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "join_sub_where2.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "CompanyName";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                cmnDao.SetParameter("P1", "test");
                cmnDao.SetParameter("P2", "test");

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("j1", false);
                cmnDao.SetParameter("if1", true);
                cmnDao.SetParameter("s1", true);

                cmnDao.SetParameter("j2", true);
                cmnDao.SetParameter("if2", true);
                cmnDao.SetParameter("s2", false);

                cmnDao.SetParameter("j3", true);
                cmnDao.SetParameter("if3", true);
                cmnDao.SetParameter("s3", false);

                cmnDao.SetParameter("j4", true);
                cmnDao.SetParameter("if4", true);
                cmnDao.SetParameter("s4", true);

                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;

        }
        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region UOC_TestSqlsvr4c
    /// <summary>
    /// UOC_TestSqlsvr4c Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr4c(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr4c.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr4c.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "CompanyName";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                cmnDao.SetParameter("P2", "test");

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", orderColumn);
                cmnDao.SetUserParameter("SEQUENCE", orderSequence);

                cmnDao.SetParameter("j2", true);

                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region UOC_TestSqlsvr4b
    /// <summary>
    /// UOC_TestSqlsvr4b Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr4b(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr4b.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr4b.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "CompanyName";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                cmnDao.SetParameter("P1", "test");

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("j1", true);

                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region UOC_TestSqlsvr4a
    /// <summary>
    /// UOC_TestSqlsvr4a Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr4a(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr4a.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr4a.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "CompanyName";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "CompanyName";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                cmnDao.SetParameter("P1", "test");
                cmnDao.SetParameter("P2", "test1");

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("j1", true);
                cmnDao.SetParameter("j2", true);
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion

    #region UOC_testSqlsvr2c
    /// <summary>
    /// UOC_TestSqlsvr2c Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr2c(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2c.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2c.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "SUPPLIERID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                cmnDao.SetParameter("up", 28.5);
                cmnDao.SetParameter("uis", 113);
                cmnDao.SetParameter("uoo", 0);
                cmnDao.SetParameter("rol", 25);
                cmnDao.SetParameter("bit", 25);

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("ISNOTNULL1", true);
                cmnDao.SetParameter("ISNOTNULL2", true);
                cmnDao.SetParameter("ISNOTNULL3", true);
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion

    #region "UOC_testSqlsvr2d"
    /// <summary>
    /// UOC_TestSqlsvr2d Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr2d(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2d.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2d.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "SUPPLIERID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                cmnDao.SetParameter("up", 28.5);
                cmnDao.SetParameter("uis", 113);
                cmnDao.SetParameter("uoo", 0);
                cmnDao.SetParameter("rol", 25);
                cmnDao.SetParameter("bit", 25);
                ArrayList arr = new ArrayList();
                arr.Add(1);
                arr.Add(2);
                arr.Add(3);
                arr.Add(4);
                arr.Add(5);
                arr.Add(6);
                arr.Add(7);
                arr.Add(8);
                cmnDao.SetParameter("PLIST", arr);
                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("ISNOTNULL1", false);
                cmnDao.SetParameter("ISNOTNULL2", false);
                cmnDao.SetParameter("ISNOTNULL3", false);
                // 戻り値 ds
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion

    #region "UOC_testSqlsvr2e"
    /// <summary>
    /// UOC_TestSqlsvr2e Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr2e(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2d.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2d.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "SUPPLIERID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                cmnDao.SetParameter("up", 28.5);
                cmnDao.SetParameter("uis", 113);
                cmnDao.SetParameter("uoo", 0);
                cmnDao.SetParameter("rol", 25);
                cmnDao.SetParameter("bit", 25);
                ArrayList arr = new ArrayList();

                cmnDao.SetParameter("PLIST", arr);
                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("ISNOTNULL1", false);
                cmnDao.SetParameter("ISNOTNULL2", false);
                cmnDao.SetParameter("ISNOTNULL3", false);
                // 戻り値 ds
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion

    #region "UOC_testSqlsvr2f"
    /// <summary>
    /// UOC_TestSqlsvr2f Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr2f(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2d.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr2d.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "SUPPLIERID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                cmnDao.SetParameter("up", 28.5);
                cmnDao.SetParameter("uis", 113);
                cmnDao.SetParameter("uoo", 0);
                cmnDao.SetParameter("rol", 25);
                cmnDao.SetParameter("bit", 25);
                List<int> arr = new List<int>();
                arr.Add(1);
                arr.Add(2);
                arr.Add(3);
                arr.Add(4);
                arr.Add(5);
                arr.Add(6);
                arr.Add(7);
                arr.Add(8);
                cmnDao.SetParameter("PLIST", arr);
                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("ISNOTNULL1", false);
                cmnDao.SetParameter("ISNOTNULL2", false);
                cmnDao.SetParameter("ISNOTNULL3", false);
                // 戻り値 ds
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion
    #region "UOC_TestSqlsvr_n"
    /// <summary>
    /// UOC_TestSqlsvr_n Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr_n(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr_n.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr_n.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "SUPPLIERID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                

                ArrayList arr = new ArrayList();
                arr.Add(1);
                arr.Add(2);
                arr.Add(3);
                arr.Add(4);
                arr.Add(5);
                arr.Add(6);
                arr.Add(7);
                arr.Add(8);
                cmnDao.SetParameter("PLIST", arr);
                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("BIT", false);
                cmnDao.SetParameter("SUB1", false);
                cmnDao.SetParameter("SUB2", false);
                cmnDao.SetParameter("ISNOTNULL1", false);
                cmnDao.SetParameter("ISNOTNULL2", false);
                cmnDao.SetParameter("ISNOTNULL3", false);
                // 戻り値 ds
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region "UOC_TestSqlsvr1a"
    /// <summary>
    /// UOC_TestSqlsvr1a Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr1a(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr1a.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr1a.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "SUPPLIERID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                

                ArrayList arr = new ArrayList();
                arr.Add(1);
                arr.Add(2);
                arr.Add(3);
                arr.Add(4);
                arr.Add(5);
                arr.Add(6);
                arr.Add(7);
                arr.Add(8);
                cmnDao.SetParameter("PLIST", arr);
                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("BIT", false);
                cmnDao.SetParameter("SUB1", false);
                cmnDao.SetParameter("SUB2", false);
                cmnDao.SetParameter("ISNOTNULL1", false);
                cmnDao.SetParameter("ISNOTNULL2", false);
                cmnDao.SetParameter("ISNOTNULL3", false);
                // 戻り値 ds
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region "UOC_TestSqlsvr1b"
    /// <summary>
    /// UOC_TestSqlsvr1b Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr1b(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr1b.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr1b.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "SUPPLIERID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "SUPPLIERID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                ArrayList arr = new ArrayList();
                arr.Add(1);
                arr.Add(2);
                arr.Add(3);
                arr.Add(4);
                arr.Add(5);
                arr.Add(6);
                arr.Add(7);
                arr.Add(8);
                cmnDao.SetParameter("PLIST", arr);
                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("BIT", false);
                cmnDao.SetParameter("SUB1", false);
                cmnDao.SetParameter("SUB2", false);
                cmnDao.SetParameter("ISNOTNULL1", false);
                cmnDao.SetParameter("ISNOTNULL2", false);
                cmnDao.SetParameter("ISNOTNULL3", false);
                // 戻り値 ds
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region "UOC_TestSqlsvr_1e"
    /// <summary>
    /// UOC_TestSqlsvr_1e Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_TestSqlsvr_1e(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        // 静的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr_-1e.dpq.xml";
                        break;

                    case "dynamic":
                        // 動的SQLを指定
                        cmnDao.SQLFileName = "testSqlsvr_-1e.dpq.xml";
                        break;
                }

                // ユーザ定義パラメタに対して、動的に値を設定する。
                string orderColumn = "";
                string orderSequence = "";

                if (testParameter.OrderColumn == "c1")
                {
                    orderColumn = "EmployeeID";
                }
                else if (testParameter.OrderColumn == "c2")
                {
                    orderColumn = "EmployeeID";
                }
                else if (testParameter.OrderColumn == "c3")
                {
                    orderColumn = "EmployeeID";
                }

                if (testParameter.OrderSequence == "A")
                {
                    orderSequence = "ASC";
                }
                else if (testParameter.OrderSequence == "D")
                {
                    orderSequence = "DESC";
                }

                // ユーザ入力は指定しない。
                // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                //    必要であれば、前後の空白を明示的に指定する必要がある。
                cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                cmnDao.SetParameter("FN", "Nancy");
                cmnDao.SetParameter("LN1", "Davolio");
                cmnDao.SetParameter("LN2", "Davolio");
                cmnDao.SetParameter("P1", "2");
                cmnDao.SetParameter("P2", "5");
                // 戻り値 ds
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }

    #endregion

    #region UOC_SelectCase
    /// <summary>
    /// UOC_SelectCase Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_SelectCase(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        switch (testParameter.SelectCase)
                        {
                            case "SelectCase1a":
                                cmnDao.SQLFileName = "select-case1a.dpq.xml";
                                break;
                            case "SelectCase1b":
                                cmnDao.SQLFileName = "select-case1b.dpq.xml";
                                break;
                            case "SelectCase2a":
                                cmnDao.SQLFileName = "select-case2a.dpq.xml";
                                break;
                            case "SelectCase2b":
                                cmnDao.SQLFileName = "select-case2b.dpq.xml";
                                break;
                            case "SelectCase3a":
                                cmnDao.SQLFileName = "select-case3a.dpq.xml";
                                break;
                            case "SelectCase3b":
                                cmnDao.SQLFileName = "select-case3b.dpq.xml";
                                break;
                            case "SelectCase4a":
                                cmnDao.SQLFileName = "select-case4a.dpq.xml";
                                break;
                            case "SelectCase4b":
                                cmnDao.SQLFileName = "select-case4b.dpq.xml";
                                break;
                            case "SelectCase5a":
                                cmnDao.SQLFileName = "select-case5a.dpq.xml";
                                break;
                            case "SelectCase5b":
                                cmnDao.SQLFileName = "select-case5b.dpq.xml";
                                break;
                            case "SelectCaseDefault1a":
                                cmnDao.SQLFileName = "select-case-default1a.dpq.xml";
                                break;
                            case "SelectCaseDefault1b":
                                cmnDao.SQLFileName = "select-case-default1b.dpq.xml";
                                break;
                            case "SelectCaseDefault2a":
                                cmnDao.SQLFileName = "select-case-default2a.dpq.xml";
                                break;
                            case "SelectCaseDefault2b":
                                cmnDao.SQLFileName = "select-case-default2b.dpq.xml";
                                break;
                            case "SelectCaseDefault3a":
                                cmnDao.SQLFileName = "select-case-default3a.dpq.xml";
                                break;
                            case "SelectCaseDefault3b":
                                cmnDao.SQLFileName = "select-case-default3b.dpq.xml";
                                break;
                            case "SelectCaseDefault4a":
                                cmnDao.SQLFileName = "select-case-default4a.dpq.xml";
                                break;
                            case "SelectCaseDefault4b":
                                cmnDao.SQLFileName = "select-case-default4b.dpq.xml";
                                break;
                           
                        }
                        break;

                    case "dynamic":
                        switch (testParameter.SelectCase)
                        {
                            case "SelectCase1a":
                                cmnDao.SQLFileName = "select-case1a.dpq.xml";
                                break;
                            case "SelectCase1b":
                                cmnDao.SQLFileName = "select-case1b.dpq.xml";
                                break;
                            case "SelectCase2a":
                                cmnDao.SQLFileName = "select-case2a.dpq.xml";
                                break;
                            case "SelectCase2b":
                                cmnDao.SQLFileName = "select-case2b.dpq.xml";
                                break;
                            case "SelectCase3a":
                                cmnDao.SQLFileName = "select-case3a.dpq.xml";
                                break;
                            case "SelectCase3b":
                                cmnDao.SQLFileName = "select-case3b.dpq.xml";
                                break;
                            case "SelectCase4a":
                                cmnDao.SQLFileName = "select-case4a.dpq.xml";
                                break;
                            case "SelectCase5a":
                                cmnDao.SQLFileName = "select-case5a.dpq.xml";
                                break;
                            case "SelectCase5b":
                                cmnDao.SQLFileName = "select-case5b.dpq.xml";
                                break;
                            case "SelectCase4b":
                                cmnDao.SQLFileName = "select-case4b.dpq.xml";
                                break;
                            case "SelectCaseDefault1a":
                                cmnDao.SQLFileName = "select-case-default1a.dpq.xml";
                                break;
                            case "SelectCaseDefault1b":
                                cmnDao.SQLFileName = "select-case-default1b.dpq.xml";
                                break;
                            case "SelectCaseDefault2a":
                                cmnDao.SQLFileName = "select-case-default2a.dpq.xml";
                                break;
                            case "SelectCaseDefault2b":
                                cmnDao.SQLFileName = "select-case-default2b.dpq.xml";
                                break;
                            case "SelectCaseDefault3a":
                                cmnDao.SQLFileName = "select-case-default3a.dpq.xml";
                                break;
                            case "SelectCaseDefault3b":
                                cmnDao.SQLFileName = "select-case-default3b.dpq.xml";
                                break;
                            case "SelectCaseDefault4a":
                                cmnDao.SQLFileName = "select-case-default4a.dpq.xml";
                                break;
                            case "SelectCaseDefault4b":
                                cmnDao.SQLFileName = "select-case-default4b.dpq.xml";
                                break;
                           
                        }
                        break;
                }

                switch (testParameter.SelectCase)
                {
                    // Select Case
                    case "SelectCase1a":
                        cmnDao.SetParameter("sel", "a1");
                        break;
                    case "SelectCase1b":
                        cmnDao.SetParameter("sel", 111);
                        break;
                    case "SelectCase2a":
                        cmnDao.SetParameter("sel", "b2");
                        break;
                    case "SelectCase2b":
                        cmnDao.SetParameter("sel", 222);
                        break;
                    case "SelectCase3a":
                        cmnDao.SetParameter("sel", "c3");
                        break;
                    case "SelectCase3b":
                        cmnDao.SetParameter("sel", 333);
                        break;
                    case "SelectCase4a":
                        cmnDao.SetParameter("sel", "xxx");
                        break;
                    case "SelectCase4b":
                        cmnDao.SetParameter("sel", 999);
                        break;
                    case "SelectCase5a":
                        break;
                    case "SelectCase5b":
                         cmnDao.SetParameter("sel", "");
                        break;

                    //Select Case Default
                    case "SelectCaseDefault1a":
                        cmnDao.SetParameter("sel", "a1");
                        break;
                    case "SelectCaseDefault1b":
                        cmnDao.SetParameter("sel", 111);
                        break;
                    case "SelectCaseDefault2a":
                        cmnDao.SetParameter("sel", "b2");
                        break;
                    case "SelectCaseDefault2b":
                        cmnDao.SetParameter("sel", 222);
                        break;
                    case "SelectCaseDefault3a":
                        cmnDao.SetParameter("sel", "c3");
                        break;
                    case "SelectCaseDefault3b":
                        cmnDao.SetParameter("sel", 333);
                        break;
                    case "SelectCaseDefault4a":
                        cmnDao.SetParameter("sel", "xxx");
                        break;
                    case "SelectCaseDefault4b":
                        cmnDao.SetParameter("sel", 999);
                        break;
                }
                // 戻り値 dt                
                DataTable dt = new DataTable();
                // 共通Daoを実行
                cmnDao.ExecSelectFill_DT(dt);

                // 自動生成Daoを実行
                testReturn.Obj = dt;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region UOC_check
    /// <summary>
    /// UOC_check Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_check(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        switch (testParameter.check)
                        {
                            case "check_7a":
                                cmnDao.SQLFileName = "check_7a.xml";
                                break;
                            case "check_11a":
                                cmnDao.SQLFileName = "check_11a.xml";
                                break;
                            case "check_11c":
                                cmnDao.SQLFileName = "check_11c.xml";
                                break;
                            case "check_3f":
                                cmnDao.SQLFileName = "check_3f.xml";
                                break;
                            case "check_9c":
                                cmnDao.SQLFileName = "check_9c.xml";
                                break;
                            case "check_1":
                                cmnDao.SQLFileName = "check_1.xml";
                                break;
                            case "check_2":
                                cmnDao.SQLFileName = "check_2.xml";
                                break;
                            case "check_3":
                                cmnDao.SQLFileName = "check_3.xml";
                                break;
                            case "check_4":
                                cmnDao.SQLFileName = "check_4.xml";
                                break;
                            case "check_5":
                                cmnDao.SQLFileName = "check_5.xml";
                                break;
                        }
                        break;

                    case "dynamic":
                        switch (testParameter.check)
                        {
                            case "check_7a":
                                cmnDao.SQLFileName = "check_7a.xml";
                                break;
                            case "check_11a":
                                cmnDao.SQLFileName = "check_11a.xml";
                                break;
                            case "check_11c":
                                cmnDao.SQLFileName = "check_11c.xml";
                                break;
                            case "check_3f":
                                cmnDao.SQLFileName = "check_3f.xml";
                                break;
                            case "check_9c":
                                cmnDao.SQLFileName = "check_9c.xml";
                                break;
                            case "check_1":
                                cmnDao.SQLFileName = "check_1.xml";
                                break;
                            case "check_2":
                                cmnDao.SQLFileName = "check_2.xml";
                                break;
                            case "check_3":
                                cmnDao.SQLFileName = "check_3.xml";
                                break;
                            case "check_4":
                                cmnDao.SQLFileName = "check_4.xml";
                                break;
                            case "check_5":
                                cmnDao.SQLFileName = "check_5.xml";
                                break;
                        }
                        break;
                }

                switch (testParameter.check)
                {
                    case "check_7a":
                        ArrayList arr = new ArrayList();
                        arr.Add(1);
                        arr.Add(2);
                        arr.Add(3);
                        arr.Add(4);
                        cmnDao.SetParameter("PLIST", arr);
                        break;
                    case "check_11a":
                        cmnDao.SetParameter("P", false);
                        break;
                    case "check_11c":
                        cmnDao.SetParameter("P", true);
                        break;
                    case "check_3f":
                        cmnDao.SetParameter("og", true);
                        break;
                    case "check_9c":
                        cmnDao.SetParameter("P", null);
                        break;
                    case "check_1":
                        cmnDao.SetParameter("P", false);
                        break;
                    case "check_2":
                        // ユーザ定義パラメタに対して、動的に値を設定する。
                        string orderColumn = "";
                        string orderSequence = "";

                        if (testParameter.OrderColumn == "c1")
                        {
                            orderColumn = "CompanyName";
                        }
                        else if (testParameter.OrderColumn == "c2")
                        {
                            orderColumn = "CompanyName";
                        }
                        else if (testParameter.OrderColumn == "c3")
                        {
                            orderColumn = "CompanyName";
                        }

                        if (testParameter.OrderSequence == "A")
                        {
                            orderSequence = "ASC";
                        }
                        else if (testParameter.OrderSequence == "D")
                        {
                            orderSequence = "DESC";
                        }

                        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。                
                        cmnDao.SetParameter("P1", "test");
                        cmnDao.SetParameter("P2", "test1");

                        // ユーザ入力は指定しない。
                        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                        //    必要であれば、前後の空白を明示的に指定する必要がある。
                        cmnDao.SetUserParameter("COLUMN", "" + orderColumn + "");
                        cmnDao.SetUserParameter("SEQUENCE", "" + orderSequence + "");

                        cmnDao.SetParameter("j1", "TEST");
                        cmnDao.SetParameter("j2", true);
                        break;
                    case "check_3":
                         cmnDao.SetParameter("P", 1);
                        break;
                    case "check_4":
                        cmnDao.SetParameter("Q", true);
                        break;
                    case "check_5":
                        cmnDao.SetParameter("P", true);
                        break;
                }
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }

        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
    #endregion

    #region UOC_edit
    /// <summary>
    /// UOC_edit Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_edit(TestParameterValue testParameter)
    {
        // 戻り値クラスを生成して、事前に戻り地に設定しておく。
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;

        // ↓業務処理-----------------------------------------------------

        switch ((testParameter.ActionType.Split('%'))[1])
        {
            case "generate":
                break;
            case "common": // 共通Daoを使用する。
            default:
                // 共通Daoを生成
                CmnDao cmnDao = new CmnDao(this.GetDam());

                switch ((testParameter.ActionType.Split('%'))[2])
                {
                    case "static":
                        switch (testParameter.SelectCase)
                        {
                            case "edit_8e":
                                cmnDao.SQLFileName = "edit_8e.xml";
                                break;
                            case "edit_4e":
                                cmnDao.SQLFileName = "edit_4e.xml";
                                break;
                            case "edit_3e":
                                cmnDao.SQLFileName = "edit_3e.xml";
                                break;
                            case "edit_9a":
                                cmnDao.SQLFileName = "edit_9a.xml";
                                break;
                            case "edit_9b1":
                                cmnDao.SQLFileName = "edit_9b1.xml";
                                break;
                            case "edit_9c1":
                                cmnDao.SQLFileName = "edit_9c1.xml";
                                break;
                            case "edit_2a":
                                cmnDao.SQLFileName = "edit_2a.xml";
                                break;
                            case "edit_5e":
                                cmnDao.SQLFileName = "edit_5e.xml";
                                break;
                            case "edit_9e":
                                cmnDao.SQLFileName = "edit_9e.xml";
                                break;
                            case "edit_10e":
                                cmnDao.SQLFileName = "edit_10e.xml";
                                break;
                            case "edit_11e":
                                cmnDao.SQLFileName = "edit_11e.xml";
                                break;                            
                        }
                        break;

                    case "dynamic":
                        switch (testParameter.SelectCase)
                        {
                            case "edit_8e":
                                cmnDao.SQLFileName = "edit_8e.xml";
                                break;
                            case "edit_4e":
                                cmnDao.SQLFileName = "edit_4e.xml";
                                break;
                            case "edit_3e":
                                cmnDao.SQLFileName = "edit_3e.xml";
                                break;
                            case "edit_9a":
                                cmnDao.SQLFileName = "edit_9a.xml";
                                break;
                            case "edit_9b1":
                                cmnDao.SQLFileName = "edit_9b1.xml";
                                break;
                            case "edit_9c1":
                                cmnDao.SQLFileName = "edit_9c1.xml";
                                break;
                            case "edit_2a":
                                cmnDao.SQLFileName = "edit_2a.xml";
                                break;
                            case "edit_5e":
                                cmnDao.SQLFileName = "edit_5e.xml";
                                break;
                            case "edit_9e":
                                cmnDao.SQLFileName = "edit_9e.xml";
                                break;
                            case "edit_10e":
                                cmnDao.SQLFileName = "edit_10e.xml";
                                break;
                            case "edit_11e":
                                cmnDao.SQLFileName = "edit_11e.xml";
                                break;                          
                        }
                        break;
                }

                switch (testParameter.SelectCase)
                {
                    case "edit_4e":
                        ArrayList arr = new ArrayList();
                        arr.Add(1);
                        arr.Add(2);
                        cmnDao.SetParameter("PLIST", arr);
                        break;
                    case "edit_8e":
                        cmnDao.SetParameter("P", true);
                        break;
                    case "edit_3e":
                        cmnDao.SetParameter("og", null);
                        break;
                    case "edit_9a":
                        cmnDao.SetParameter("P", 1);
                        break;
                    case "edit_9b1":
                        cmnDao.SetParameter("P", 1);
                        break;
                    case "edit_9c1":
                        cmnDao.SetParameter("P", 1);
                        break;
                    case "edit_2a":
                        cmnDao.SetParameter("og", true);
                        break;
                    case "edit_5e":
                        break;
                    case "edit_9e":
                        cmnDao.SetParameter("P", null);
                        break;
                    case "edit_10e":
                        cmnDao.SetParameter("P", null);
                        break;
                    case "edit_11e":
                        cmnDao.SetParameter("P", null);
                        break;                  
                }
                // 戻り値 ds
                DataSet ds = new DataSet();

                //   -- 一覧を返すSELECTクエリを実行する
                cmnDao.ExecSelectFill_DS(ds);

                // ↑DBアクセス-----------------------------------------------------

                // 戻り値を設定
                testReturn.Obj = ds;
                break;
        }
        // ↑業務処理-----------------------------------------------------

        // ロールバックのテスト
        this.TestRollback(testParameter);
    }
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

    #region UOC_GetParametersFromPARAMTag
    /// <summary>
    /// UOC_GetParametersFromPARAMTag Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_GetParametersFromPARAMTag(TestParameterValue testParameter)
    {
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;
        BaseDam bd = (BaseDam)this.GetDam();
        bd.SetSqlByFile(testParameter.Filepath);
        DataTable dt = bd.GetParametersFromPARAMTag();
        testReturn.Obj = dt;
    }
    #endregion

    #region Revert Database after Test case Run
    /// <summary>
    /// UOC_GetList Method to get the list of Shipper ID which are available before running test cases.
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_GetList(TestParameterValue testParameter)
    {
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;
        BaseDam bd = (BaseDam)this.GetDam();
        bd.SetSqlByCommand(testParameter.ScreenId, CommandType.Text);
        DataTable dt = new DataTable();
        bd.ExecSelectFill_DT(dt);
        testReturn.Obj = dt;
    }
    /// <summary>
    /// UOC_GetDelete Method to Delete data added to shipper table after running test cases.
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_GetDelete(TestParameterValue testParameter)
    {
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;
        BaseDam bd = (BaseDam)this.GetDam();
        bd.SetSqlByCommand(testParameter.ScreenId, CommandType.Text);
        int dlt = bd.ExecInsUpDel_NonQuery();
        testReturn.Obj = dlt;
    }

    /// <summary>
    /// UOC_GetID Method to get the current Identity value from Shippers table before running the test cases.
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    private void UOC_GetID(TestParameterValue testParameter)
    {
        TestReturnValue testReturn = new TestReturnValue();
        this.ReturnValue = testReturn;
        BaseDam bd = (BaseDam)this.GetDam();
        bd.SetSqlByCommand(testParameter.ScreenId, CommandType.Text);
        DataTable dt = new DataTable();
        bd.ExecSelectFill_DT(dt);
        testReturn.Obj = dt.Rows[0].ItemArray.GetValue(0);
    }
    #endregion

}
