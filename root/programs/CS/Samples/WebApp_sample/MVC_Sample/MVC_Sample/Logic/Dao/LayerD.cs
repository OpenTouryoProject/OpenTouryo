//**********************************************************************************
//* フレームワーク・テストクラス（Ｄ層）
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：LayerD
//* クラス日本語名  ：Ｄ層のテスト
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using MVC_Sample.Logic.Common;
using MVC_Sample.Models.ViewModels;

using System.Data;
using System.Collections.Generic;
using System.Diagnostics;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Diagnostics;

namespace MVC_Sample.Logic.Dao
{
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
            this.SetParameter("P1", testParameter.Shipper.ShipperID);

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

            // DataTableToList
            List<ShipperViweModel> list = DataToPoco.DataTableToList<ShipperViweModel>(dt);

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            testReturn.Obj = list;
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

            // DataTableToList
            List<ShipperViweModel> list = DataToPoco.DataTableToList<ShipperViweModel>(ds.Tables[0]);

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            testReturn.Obj = list;
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

            //   -- 一覧を返すSELECTクエリを実行する
            IDataReader idr = (IDataReader)this.ExecSelect_DR();

            // DataReaderToList
            List<ShipperViweModel> list = DataToPoco.DataReaderToList<ShipperViweModel>(idr);

            // 終了したらクローズ
            idr.Close();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            testReturn.Obj = list;
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
            // DataTableToList
            List<ShipperViweModel> list = DataToPoco.DataTableToList<ShipperViweModel>(dt);

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            testReturn.Obj = list;
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
            this.SetParameter("P1", testParameter.Shipper.ShipperID);

            // 戻り値 dt
            DataTable dt = new DataTable();

            //   -- １レコードを返すSELECTクエリを実行する
            this.ExecSelectFill_DT(dt);

            // ↑DBアクセス-----------------------------------------------------

            // 一部、DataToPocoのテストコード
            ShipperViweModel svm = DataToPoco.DataTableToPOCO<ShipperViweModel>(dt);
            Debug.WriteLine("svm:" + ObjectInspector.Inspect(svm));

            TestShipperViweModel tsvm = DataToPoco.DataTableToPOCO<TestShipperViweModel>(dt,
                // mapの書き方は、Key-Valueでdst-srcのproperty field名を書く
                new Dictionary<string, string>()
                {
                    { "_ShipperID", "ShipperID"},
                    { "_CompanyName", "CompanyName"},
                    { "_Phone", "Phone"}
                });

            Debug.WriteLine("tsvm:" + ObjectInspector.Inspect(tsvm));

            testReturn.Obj = svm;
            testReturn.Obj2 = tsvm;
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
            this.SetParameter("P2", testParameter.Shipper.CompanyName);
            this.SetParameter("P3", testParameter.Shipper.Phone);

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
            this.SetParameter("P1", testParameter.Shipper.ShipperID);
            this.SetParameter("P2", testParameter.Shipper.CompanyName);
            this.SetParameter("P3", testParameter.Shipper.Phone);

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
            this.SetParameter("P1", testParameter.Shipper.ShipperID);

            object obj;

            //   -- 削除（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            testReturn.Obj = obj;
        }

        #endregion

        #endregion
    }
}