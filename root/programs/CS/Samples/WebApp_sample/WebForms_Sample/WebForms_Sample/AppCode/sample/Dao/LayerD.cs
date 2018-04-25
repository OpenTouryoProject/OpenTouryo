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

using MyType;

using System.Data;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Public.Db;

namespace WebForms_Sample
{
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
            dt.Columns.Add("c1", typeof(string));
            dt.Columns.Add("c2", typeof(string));
            dt.Columns.Add("c3", typeof(string));

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

        #endregion

        #endregion
    } 
}
