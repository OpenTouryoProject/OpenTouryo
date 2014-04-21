//**********************************************************************************
//* フレームワーク・テストクラス（Ｄ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：LayerD_mu
//* クラス日本語名  ：Ｄ層のテスト
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

namespace WSClientSL_sample.Web.Dao
{
    /// <summary>
    /// LayerD の概要の説明です
    /// </summary>
    public class LayerD_mu : MyBaseDao
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LayerD_mu(BaseDam dam) : base(dam) { }

        #region テンプレ

        /// <summary>テンプレ</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="input">入力</param>
        /// <param name="output">出力</param>
        public void テンプレ(string actionType, object input, object output)
        {
            // ↓DBアクセス-----------------------------------------------------

            // ● 下記のいづれかの方法でSQLを設定する。

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2("ファイル名");

            //   -- 直接指定する場合。
            this.SetSqlByCommand("SQL文");

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P1", input);//ShipperID);

            object obj;

            //   -- 追加、更新、削除の場合（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            //   -- 先頭の１セル分の情報を返すSELECTクエリを実行する場合
            obj = this.ExecSelectScalar();

            //   -- テーブル（or レコード）の情報を返す
            //      SELECTクエリを実行する場合（引数 = データテーブル）
            obj = new DataTable("rtn");
            this.ExecSelectFill_DT((DataTable)obj);

            //   -- テーブル（or レコード）の情報を返す
            //      SELECTクエリを実行する場合（引数 = データセット）
            obj = new DataSet();
            this.ExecSelectFill_DS((DataSet)obj);

            //   -- データリーダを返す
            IDataReader idr = (IDataReader)this.ExecSelect_DR();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            output = obj.ToString();
        }

        #endregion

        #region 参照系

        #region 件数取得（SelectCount）

        /// <summary>件数情報を返すSELECTクエリを実行する</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="output">出力</param>
        public void SelectCount(string actionType, out string output)
        {
            // ↓DBアクセス-----------------------------------------------------

            string filename = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                filename = "ShipperCount.sql";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
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
            output = obj.ToString();
        }

        #endregion

        #region 一覧取得（SelectAll）

        /// <summary>一覧を返すSELECTクエリを実行する（DT）</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="output">出力</param>
        public void SelectAll_DT(string actionType, out DataTable output)
        {
            // ↓DBアクセス-----------------------------------------------------

            string commandText = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                commandText = "SELECT * FROM Shippers";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                commandText =
                    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
                // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
            }

            //   -- 直接指定する場合。
            this.SetSqlByCommand(commandText);

            // 戻り値 dt
            DataTable dt = new DataTable("rtn");

            //   -- 一覧を返すSELECTクエリを実行する
            this.ExecSelectFill_DT(dt);

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            output = dt;
        }

        /// <summary>一覧を返すSELECTクエリを実行する（DS）</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="output">出力</param>
        public void SelectAll_DS(string actionType, out DataSet output)
        {
            // ↓DBアクセス-----------------------------------------------------

            string commandText = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                commandText = "SELECT * FROM Shippers";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
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
            output = ds;
        }

        /// <summary>一覧を返すSELECTクエリを実行する（DR）</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="output">出力</param>
        public void SelectAll_DR(string actionType, out DataTable output)
        {
            // ↓DBアクセス-----------------------------------------------------

            string commandText = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                commandText = "SELECT * FROM Shippers";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                commandText =
                    "<?xml version=\"1.0\" encoding=\"shift_jis\" ?><ROOT>SELECT * FROM Shippers</ROOT>";
                // 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
            }

            //   -- 直接指定する場合。
            this.SetSqlByCommand(commandText);

            // 戻り値 dt
            DataTable dt = new DataTable("rtn");

            // ３列生成
            dt.Columns.Add("ShipperID", System.Type.GetType("System.String"));
            dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
            dt.Columns.Add("Phone", System.Type.GetType("System.String"));

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
            output = dt;
        }

        /// <summary>一覧を返すSELECTクエリを実行する</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="OrderColumn">OrderColumn</param>
        /// <param name="OrderSequence">OrderSequence</param>
        /// <param name="output">出力</param>
        public void SelectAll_DSQL(string actionType, string orderColumn, string orderSequence, out DataTable output)
        {
            // ↓DBアクセス-----------------------------------------------------

            string filename = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                filename = "ShipperSelectOrder.sql";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                filename = "ShipperSelectOrder.xml";
            }

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // ユーザ定義パラメタに対して、動的に値を設定する。
            if (orderColumn == "c1")
            {
                orderColumn = "ShipperID";
            }
            else if (orderColumn == "c2")
            {
                orderColumn = "CompanyName";
            }
            else if (orderColumn == "c3")
            {
                orderColumn = "Phone";
            }
            else { }

            if (orderSequence == "A")
            {
                orderSequence = "ASC";
            }
            else if (orderSequence == "D")
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
            DataTable dt = new DataTable("rtn");

            //   -- 一覧を返すSELECTクエリを実行する
            this.ExecSelectFill_DT(dt);

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            output = dt;
        }

        #endregion

        #region 参照


        /// <summary>１レコードを返すSELECTクエリを実行する</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="shipperID">shipperID</param>
        /// <param name="companyName">companyName</param>
        /// <param name="phone">phone</param>
        public void Select(string actionType, string shipperID, out string companyName, out string phone)
        {
            // ↓DBアクセス-----------------------------------------------------

            string filename = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                filename = "ShipperSelect.sql";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                filename = "ShipperSelect.xml";
            }

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P1", shipperID);

            // 戻り値 dt
            DataTable dt = new DataTable("rtn");

            //   -- 一覧を返すSELECTクエリを実行する
            this.ExecSelectFill_DT(dt);

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            companyName = (string)dt.Rows[0].ItemArray.GetValue(1);
            phone = (string)dt.Rows[0].ItemArray.GetValue(2);
        }

        #endregion

        #endregion

        #region 更新系

        #region 追加


        /// <summary>Insertクエリを実行する</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="companyName">companyName</param>
        /// <param name="phone">phone</param>
        /// <param name="output">出力</param>
        public void Insert(string actionType, string companyName, string phone, out string output)
        {
            // ↓DBアクセス-----------------------------------------------------

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2("ShipperInsert.sql");

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P2", companyName);
            this.SetParameter("P3", phone);

            object obj;

            //   -- 追加（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            output = obj.ToString();
        }

        #endregion

        #region 更新

        /// <summary>Updateクエリを実行する</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="shipperID">shipperID</param>
        /// <param name="companyName">companyName</param>
        /// <param name="phone">phone</param>
        /// <param name="output">出力</param>
        public void Update(string actionType, string shipperID, string companyName, string phone, out string output)
        {

            // ↓DBアクセス-----------------------------------------------------

            string filename = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                filename = "ShipperUpdate.sql";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                filename = "ShipperUpdate.xml";
            }

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P1", shipperID);
            this.SetParameter("P2", companyName);
            this.SetParameter("P3", phone);

            object obj;

            //   -- 更新（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            output = obj.ToString();
        }

        #endregion

        #region 削除

        /// <summary>Deleteクエリを実行する</summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="shipperID">shipperID</param>
        /// <param name="output">出力</param>
        public void Delete(string actionType, string shipperID, out string output)
        {
            // ↓DBアクセス-----------------------------------------------------

            string filename = "";

            if ((actionType.Split('%'))[2] == "static")
            {
                // 静的SQL
                filename = "ShipperDelete.sql";
            }
            else if ((actionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                filename = "ShipperDelete.xml";
            }

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P1", shipperID);

            object obj;

            //   -- 削除（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            output = obj.ToString();
        }

        #endregion

        #endregion
    }
}
