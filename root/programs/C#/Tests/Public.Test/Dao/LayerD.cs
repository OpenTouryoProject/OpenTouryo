//**********************************************************************************
//* フレームワーク・テストクラス（Ｄ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：LayerD.cs
//* クラス日本語名  ：
//*
//* 作成者          ：Rituparna & Santosh
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  06/24/2014   Rituparna & Santosh   Testcode development for CRUDTest(Public classes).
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
/// LayerD の概要の説明です
/// </summary>
public class LayerD : MyBaseDao
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LayerD(BaseDam dam) : base(dam) { }

    #region テンプレ

    /// <summary>テンプレ</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void テンプレ(TestParameterValue testParameter, TestReturnValue testReturn)
    {

        // ↓DBアクセス-----------------------------------------------------

        // ● 下記のいづれかの方法でSQLを設定する。

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2("ファイル名");

        //   -- 直接指定する場合。
        this.SetSqlByCommand("SQL文");

        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
        this.SetParameter("P1", testParameter.ShipperID);

        object obj;

        //   -- 追加、更新、削除の場合（件数を確認できる）
        obj = this.ExecInsUpDel_NonQuery();

        //   -- 先頭の１セル分の情報を返すSELECTクエリを実行する場合
        obj = this.ExecSelectScalar();

        //   -- テーブル（or レコード）の情報を返す
        //      SELECTクエリを実行する場合（引数 = データテーブル）
        obj = new DataTable();
        this.ExecSelectFill_DT((DataTable)obj);

        //   -- テーブル（or レコード）の情報を返す
        //      SELECTクエリを実行する場合（引数 = データセット）
        obj = new DataSet();
        this.ExecSelectFill_DS((DataSet)obj);

        //   -- データリーダを返す
        IDataReader idr = (IDataReader)this.ExecSelect_DR();

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = obj;
    }

    #endregion

    #region 参照系

    #region 件数取得（SelectCount）

    /// <summary>件数情報を返すSELECTクエリを実行する</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void SelectCount(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "ShipperCount.sql";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "ShipperCount.xml";
        }

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2(filename);

        object obj;

        //   -- 件数情報を返すSELECTクエリを実行する
        obj = this.ExecSelectScalar();

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = obj;
    }

    #endregion

    #region 一覧取得（SelectAll）

    /// <summary>一覧を返すSELECTクエリを実行する（DT）</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void SelectAll_DT(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string commandText = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            commandText = "SELECT * FROM Shippers";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            commandText =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }

        //   -- 直接指定する場合。
        this.SetSqlByCommand(commandText);

        // 戻り値 dt
        DataTable dt = new DataTable();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DT(dt);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = dt;
    }

    /// <summary>一覧を返すSELECTクエリを実行する（DS）</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void SelectAll_DS(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string commandText = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            commandText = "SELECT * FROM Shippers";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            commandText =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }

        //   -- 直接指定する場合。
        this.SetSqlByCommand(commandText);

        // 戻り値 ds
        DataSet ds = new DataSet();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DS(ds);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = ds;
    }

    /// <summary>一覧を返すSELECTクエリを実行する（DR）</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void SelectAll_DR(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string commandText = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            commandText = "SELECT * FROM Shippers";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            commandText =
                "<?xml version=\"1.0\" encoding=\"shift_jis\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }

        //   -- 直接指定する場合。
        this.SetSqlByCommand(commandText);

        // 戻り値 dt
        DataTable dt = new DataTable();

        // ３列生成
        dt.Columns.Add("c1", System.Type.GetType("System.String"));
        dt.Columns.Add("c2", System.Type.GetType("System.String"));
        dt.Columns.Add("c3", System.Type.GetType("System.String"));

        //   -- 一覧を返すSELECTクエリを実行する
        IDataReader idr = (IDataReader)this.ExecSelect_DR();

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

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = dt;
    }

    /// <summary>一覧を返すSELECTクエリを実行する</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void SelectAll_DSQL(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "ShipperSelectOrder.sql";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "ShipperSelectOrder.xml";
        }

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2(filename);

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
        this.SetParameter("P1", "test");

        // ユーザ入力は指定しない。
        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
        //    必要であれば、前後の空白を明示的に指定する必要がある。
        this.SetUserParameter("COLUMN", " " + orderColumn + " ");
        this.SetUserParameter("SEQUENCE", " " + orderSequence + " ");

        // 戻り値 dt
        DataTable dt = new DataTable();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DT(dt);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = dt;
    }

    #endregion

    #region 参照

    /// <summary>１レコードを返すSELECTクエリを実行する</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void Select(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL          
            filename = "ShipperSelect.sql";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "ShipperSelect.xml";
        }

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2(filename);

        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
        this.SetParameter("P1", testParameter.ShipperID);


        // 戻り値 dt
        DataTable dt = new DataTable();

        //   -- １レコードを返すSELECTクエリを実行する
        this.ExecSelectFill_DT(dt);

        // ↑DBアクセス-----------------------------------------------------

        //// 戻り値を設定 // 不要
        //testReturn.Obj = dt;

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
    }

    #endregion

    #endregion

    #region 更新系

    #region 追加

    /// <summary>Insertクエリを実行する</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void Insert(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2("ShipperInsert.sql");

        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
        this.SetParameter("P2", testParameter.CompanyName);
        this.SetParameter("P3", testParameter.Phone);

        object obj;

        //   -- 追加（件数を確認できる）
        obj = this.ExecInsUpDel_NonQuery();

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = obj;
    }

    #endregion

    #region 更新

    /// <summary>Updateクエリを実行する</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void Update(TestParameterValue testParameter, TestReturnValue testReturn)
    {

        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "ShipperUpdate.sql";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "ShipperUpdate.xml";
        }

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2(filename);

        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
        this.SetParameter("P1", testParameter.ShipperID);
        this.SetParameter("P2", testParameter.CompanyName);
        this.SetParameter("P3", testParameter.Phone);

        object obj;

        //   -- 更新（件数を確認できる）
        obj = this.ExecInsUpDel_NonQuery();

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = obj;
    }

    #endregion

    #region 削除

    /// <summary>Deleteクエリを実行する</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void Delete(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "ShipperDelete.sql";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "ShipperDelete.xml";
        }

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2(filename);

        // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
        this.SetParameter("P1", testParameter.ShipperID);

        object obj;

        //   -- 追削除（件数を確認できる）
        obj = this.ExecInsUpDel_NonQuery();

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = obj;
    }
    #region SelectJoin1
    /// <summary>
    /// SelectJoin1 Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    /// <param name="testReturn">testReturn</param>
    public void SelectJoin1(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "join_sub_where1.dpq.xml";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "join_sub_where1.dpq.xml";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }
        this.SetSqlByFile2(filename);
        string orderColumn = "";
        string orderSequence = "";

        if (testParameter.OrderColumn == "c1")
        {
            orderColumn = "companyname";
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
        this.SetParameter("P1", "test");
        this.SetParameter("P2", "test1");

        // ユーザ入力は指定しない。
        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
        //    必要であれば、前後の空白を明示的に指定する必要がある。
        this.SetUserParameter("COLUMN", orderColumn);
        this.SetUserParameter("SEQUENCE", " " + orderSequence + " ");
        this.SetParameter("j1", true);
        this.SetParameter("if1", true);
        this.SetParameter("s1", true);

        this.SetParameter("j2", true);
        this.SetParameter("if2", true);
        this.SetParameter("s2", true);

        this.SetParameter("j3", true);
        this.SetParameter("if3", true);
        this.SetParameter("s3", true);

        this.SetParameter("j4", true);
        this.SetParameter("if4", true);
        this.SetParameter("s4", true);

        // 戻り値 ds
        DataSet ds = new DataSet();
        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DS(ds);
        // ↑DBアクセス-----------------------------------------------------
        // 戻り値を設定
        testReturn.Obj = ds;
    }
    #endregion
    #region SelectJoin2
    /// <summary>
    /// SelectJoin2 Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    /// <param name="testReturn">testReturn</param>
    public void SelectJoin2(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------
        string filename = "";
        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "join_sub_where2.dpq.xml";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "join_sub_where2.dpq.xml";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }
        this.SetSqlByFile2(filename);
        string orderColumn = "";
        string orderSequence = "";

        if (testParameter.OrderColumn == "c1")
        {
            orderColumn = "companyname";
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
        this.SetParameter("P1", "test");
        this.SetParameter("P2", "test1");

        // ユーザ入力は指定しない。
        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
        //    必要であれば、前後の空白を明示的に指定する必要がある。
        this.SetUserParameter("COLUMN", orderColumn);
        this.SetUserParameter("SEQUENCE", " " + orderSequence + " ");
        this.SetParameter("j1", false);
        this.SetParameter("if1", true);
        this.SetParameter("s1", true);

        this.SetParameter("j2", true);
        this.SetParameter("if2", true);
        this.SetParameter("s2", false);

        this.SetParameter("j3", true);
        this.SetParameter("if3", true);
        this.SetParameter("s3", false);

        this.SetParameter("j4", true);
        this.SetParameter("if4", true);
        this.SetParameter("s4", true);
        // 戻り値 ds
        DataSet ds = new DataSet();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DS(ds);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = ds;
    }
    #endregion
    #region TestSqlsvr4c
    /// <summary>
    /// TestSqlsvr4c Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    /// <param name="testReturn">testReturn</param>
    public void TestSqlsvr4c(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "testSqlsvr4c.dpq.xml";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "testSqlsvr4c.dpq.xml";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }
        this.SetSqlByFile2(filename);
        string orderColumn = "";
        string orderSequence = "";

        if (testParameter.OrderColumn == "c1")
        {
            orderColumn = "companyname";
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
        this.SetParameter("P2", "test");

        // ユーザ入力は指定しない。
        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
        //    必要であれば、前後の空白を明示的に指定する必要がある。
        this.SetUserParameter("COLUMN", orderColumn);
        this.SetUserParameter("SEQUENCE", " " + orderSequence + " ");

        this.SetParameter("j2", true);
        // 戻り値 ds
        DataSet ds = new DataSet();
        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DS(ds);
        // ↑DBアクセス-----------------------------------------------------
        // 戻り値を設定
        testReturn.Obj = ds;
    }
    #endregion
    #region TestSqlsvr4b
    /// <summary>
    /// TestSqlsvr4b Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    /// <param name="testReturn">testReturn</param>
    public void TestSqlsvr4b(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------
        string filename = "";
        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "testSqlsvr4c.dpq.xml";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "testSqlsvr4c.dpq.xml";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }
        this.SetSqlByFile2(filename);
        string orderColumn = "";
        string orderSequence = "";

        if (testParameter.OrderColumn == "c1")
        {
            orderColumn = "companyname";
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
        this.SetParameter("P1", "test");

        // ユーザ入力は指定しない。
        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
        //    必要であれば、前後の空白を明示的に指定する必要がある。
        this.SetUserParameter("COLUMN", orderColumn);
        this.SetUserParameter("SEQUENCE", " " + orderSequence + " ");
        this.SetParameter("j1", true);

        // 戻り値 ds
        DataSet ds = new DataSet();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DS(ds);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = ds;
    }
    #endregion
    #region TestSqlsvr4a
    /// <summary>
    /// TestSqlsvr4a Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    /// <param name="testReturn">testReturn</param>
    public void TestSqlsvr4a(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------
        string filename = "";
        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            filename = "testSqlsvr4a.dpq.xml";
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            filename = "testSqlsvr4a.dpq.xml";
            // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
        }
        this.SetSqlByFile2(filename);
        string orderColumn = "";
        string orderSequence = "";

        if (testParameter.OrderColumn == "c1")
        {
            orderColumn = "companyname";
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
        this.SetParameter("P1", "test");
        this.SetParameter("P2", "test");

        // ユーザ入力は指定しない。
        // ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
        //    必要であれば、前後の空白を明示的に指定する必要がある。
        this.SetUserParameter("COLUMN", orderColumn);
        this.SetUserParameter("SEQUENCE", " " + orderSequence + " ");

        this.SetParameter("j1", true);
        this.SetParameter("j2", true);
        // 戻り値 ds
        DataSet ds = new DataSet();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DS(ds);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = ds;
    }
    #endregion
    #region Select Case and Select Case Default

    /// <summary>一覧を返すSELECTクエリを実行する</summary>
    /// <param name="testParameter">引数クラス</param>
    /// <param name="testReturn">戻り値クラス</param>
    public void SelectCase(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------

        string filename = "";

        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            switch (testParameter.SelectCase)
            {
                case "SelectCase1a":
                    filename = "select-case1a.dpq.xml";
                    break;
                case "SelectCase1b":
                    filename = "select-case1b.dpq.xml";
                    break;
                case "SelectCase2a":
                    filename = "select-case2a.dpq.xml";
                    break;
                case "SelectCase2b":
                    filename = "select-case2b.dpq.xml";
                    break;
                case "SelectCase3a":
                    filename = "select-case3a.dpq.xml";
                    break;
                case "SelectCase3b":
                    filename = "select-case3b.dpq.xml";
                    break;
                case "SelectCase4a":
                    filename = "select-case4a.dpq.xml";
                    break;
                case "SelectCase4b":
                    filename = "select-case4b.dpq.xml";
                    break;
                case "SelectCaseDefault1a":
                    filename = "select-case-default1a.dpq.xml";
                    break;
                case "SelectCaseDefault1b":
                    filename = "select-case-default1b.dpq.xml";
                    break;
                case "SelectCaseDefault2a":
                    filename = "select-case-default2a.dpq.xml";
                    break;
                case "SelectCaseDefault2b":
                    filename = "select-case-default2b.dpq.xml";
                    break;
                case "SelectCaseDefault3a":
                    filename = "select-case-default3a.dpq.xml";
                    break;
                case "SelectCaseDefault3b":
                    filename = "select-case-default3b.dpq.xml";
                    break;
                case "SelectCaseDefault4a":
                    filename = "select-case-default4a.dpq.xml";
                    break;
                case "SelectCaseDefault4b":
                    filename = "select-case-default4b.dpq.xml";
                    break;
            }
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            switch (testParameter.SelectCase)
            {
                case "SelectCase1a":
                    filename = "select-case1a.dpq.xml";
                    break;
                case "SelectCase1b":
                    filename = "select-case1b.dpq.xml";
                    break;
                case "SelectCase2a":
                    filename = "select-case2a.dpq.xml";
                    break;
                case "SelectCase2b":
                    filename = "select-case2b.dpq.xml";
                    break;
                case "SelectCase3a":
                    filename = "select-case3a.dpq.xml";
                    break;
                case "SelectCase3b":
                    filename = "select-case3b.dpq.xml";
                    break;
                case "SelectCase4a":
                    filename = "select-case4a.dpq.xml";
                    break;
                case "SelectCase4b":
                    filename = "select-case4b.dpq.xml";
                    break;
                case "SelectCaseDefault1a":
                    filename = "select-case-default1a.dpq.xml";
                    break;
                case "SelectCaseDefault1b":
                    filename = "select-case-default1b.dpq.xml";
                    break;
                case "SelectCaseDefault2a":
                    filename = "select-case-default2a.dpq.xml";
                    break;
                case "SelectCaseDefault2b":
                    filename = "select-case-default2b.dpq.xml";
                    break;
                case "SelectCaseDefault3a":
                    filename = "select-case-default3a.dpq.xml";
                    break;
                case "SelectCaseDefault3b":
                    filename = "select-case-default3b.dpq.xml";
                    break;
                case "SelectCaseDefault4a":
                    filename = "select-case-default4a.dpq.xml";
                    break;
                case "SelectCaseDefault4b":
                    filename = "select-case-default4b.dpq.xml";
                    break;
            }
        }

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2(filename);
        switch (testParameter.SelectCase)
        {
            // Select Case
            case "SelectCase1a":
                this.SetParameter("sel", "a1");
                break;
            case "SelectCase1b":
                this.SetParameter("sel", 111);
                break;
            case "SelectCase2a":
                this.SetParameter("sel", "b2");
                break;
            case "SelectCase2b":
                this.SetParameter("sel", 222);
                break;
            case "SelectCase3a":
                this.SetParameter("sel", "c3");
                break;
            case "SelectCase3b":
                this.SetParameter("sel", 333);
                break;
            case "SelectCase4a":
                this.SetParameter("sel", "xxx");
                break;
            case "SelectCase4b":
                this.SetParameter("sel", 999);
                break;
            //Select Case Default
            case "SelectCaseDefault1a":
                this.SetParameter("sel", "a1");
                break;
            case "SelectCaseDefault1b":
                this.SetParameter("sel", 111);
                break;
            case "SelectCaseDefault2a":
                this.SetParameter("sel", "b2");
                break;
            case "SelectCaseDefault2b":
                this.SetParameter("sel", 222);
                break;
            case "SelectCaseDefault3a":
                this.SetParameter("sel", "c3");
                break;
            case "SelectCaseDefault3b":
                this.SetParameter("sel", 333);
                break;
            case "SelectCaseDefault4a":
                this.SetParameter("sel", "xxx");
                break;
            case "SelectCaseDefault4b":
                this.SetParameter("sel", 999);
                break;
        }

        // 戻り値 dt
        DataTable dt = new DataTable();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DT(dt);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = dt;
    }

    #endregion

    #region Check

    /// <summary>
    /// check Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    /// <param name="testReturn">testReturn</param>
    public void check(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        // ↓DBアクセス-----------------------------------------------------
        string filename = "";
        if ((testParameter.ActionType.Split('%'))[2] == "static")
        {
            // 静的SQL
            switch (testParameter.check)
            {
                case "check_7a":
                    filename = "check_7a.xml";
                    break;
                case "check_11a":
                    filename = "check_11a.xml";
                    break;
                case "check_11c":
                    filename = "check_11c.xml";
                    break;
            }
        }
        else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
        {
            // 動的SQL
            switch (testParameter.check)
            {
                case "check_7a":
                    filename = "check_7a.xml";
                    break;
                case "check_11a":
                    filename = "check_11a.xml";
                    break;
                case "check_11c":
                    filename = "check_11c.xml";
                    break;
            }
        }

        //   -- ファイルから読み込む場合。
        this.SetSqlByFile2(filename);
        switch (testParameter.check)
        {
            case "check_7a":
                ArrayList arr = new ArrayList();
                arr.Add(1);
                arr.Add(2);
                arr.Add(3);
                arr.Add(4);
                this.SetParameter("PLIST", arr);
                break;
            case "check_11a":
                this.SetParameter("P", false);
                break;
            case "check_11c":
                this.SetParameter("P", true);
                break;
        }

        // 戻り値 ds
        DataSet ds = new DataSet();

        //   -- 一覧を返すSELECTクエリを実行する
        this.ExecSelectFill_DS(ds);

        // ↑DBアクセス-----------------------------------------------------

        // 戻り値を設定
        testReturn.Obj = ds;
    }

    #endregion

    #endregion
    /// <summary>
    /// TestMethodForParamTag Method
    /// </summary>
    /// <param name="testParameter">testParameter</param>
    /// <param name="testReturn">testReturn</param>
    public void TestMethodForParamTag(TestParameterValue testParameter, TestReturnValue testReturn)
    {
        BaseDam bd = (BaseDam)this.GetDam();
        bd.SetSqlByFile(testParameter.Filepath);

        DataTable dt = new DataTable();
        dt = bd.GetParametersFromPARAMTag();
        testReturn.Obj = dt;
    }
}
    #endregion