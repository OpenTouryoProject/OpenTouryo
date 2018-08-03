//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：LayerB
//* クラス日本語名  ：Ｂ層のテスト
//*
//* 作成日時        ：－
//* 作成者          ：生技セ
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using RerunnableBatch_sample2.Common;

using System.Data;
using System.Text;
using System.Collections;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Public.Db;

namespace RerunnableBatch_sample2.Business
{
    /// <summary>
    /// LayerB の概要の説明です
    /// </summary>
    public class LayerB : MyFcBaseLogic
    {
        #region UOCメソッド

        #region SelectPkList

        /// <summary>主キー一覧を取得</summary>
        /// <param name="parameter">引数クラス</param>
        private void UOC_SelectPkList(VoidParameterValue parameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            SelectPkListReturnValue returnValue = new SelectPkListReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            DataTable pkTable = new DataTable();

            // ↓DBアクセス-----------------------------------------------------
            // 共通Daoを生成
            CmnDao cmnDao = new CmnDao(this.GetDam());
         
            // 動的SQLを指定
            cmnDao.SQLFileName = "SelectAllOrderID.xml";

            // 共通Daoを実行
            cmnDao.ExecSelectFill_DT(pkTable);
            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            ArrayList pkList = new ArrayList();
            for (int index = 0; index < pkTable.Rows.Count; index++)
            {
                //データテーブルからArrayListに詰め直す
                pkList.Add(pkTable.Rows[index]["OrderID"]);
            }
            returnValue.PkList = pkList;

            // ↑業務処理-----------------------------------------------------

        }

        #endregion

        #region ExecuteBatchProcess

        /// <summary>バッチ処理を実行する</summary>
        /// <param name="parameter">引数クラス</param>
        private void UOC_ExecuteBatchProcess(ExecuteBatchProcessParameterValue parameter)
        {
            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            this.ReturnValue = new VoidReturnValue();

            // ↓業務処理-----------------------------------------------------

            ArrayList pkList = parameter.SubPkList; //主キー一覧(1トランザクション分)
            DataTable dataTable = new DataTable();  //データ一覧(主キーを元に検索したデータ)

            //Ordersテーブルからデータを検索する
            // ↓DBアクセス-----------------------------------------------------
            // 共通Daoを生成
            CmnDao cmnDao = new CmnDao(this.GetDam());
            
            // 動的SQLを指定
            cmnDao.SQLFileName = "SelectInOrderID.xml";

            // パラメータを設定
            cmnDao.SetParameter("OrderID", pkList);

            // 共通Daoを実行
            cmnDao.ExecSelectFill_DT(dataTable);
            // ↑DBアクセス-----------------------------------------------------

            
            //Orders2テーブルに複数件まとめて追加する。
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < dataTable.Rows.Count; index++)
            {
                DataRow row = dataTable.Rows[index];    //1件分のデータ

                //todo：編集処理など

                // ↓DBアクセス-----------------------------------------------------
                // 自動生成Daoを生成
                DaoOrders2 dao = new DaoOrders2(this.GetDam());

                // パラメータを設定
                dao.PK_OrderID = row["OrderID"];
                dao.CustomerID = row["CustomerID"];
                dao.EmployeeID = row["EmployeeID"];
                dao.OrderDate = row["OrderDate"];
                dao.RequiredDate = row["RequiredDate"];
                dao.ShippedDate = row["ShippedDate"];
                dao.ShipVia = row["ShipVia"];
                dao.Freight = row["Freight"];
                dao.ShipName = row["ShipName"];
                dao.ShipAddress = row["ShipAddress"];
                dao.ShipCity = row["ShipCity"];
                dao.ShipRegion = row["ShipRegion"];
                dao.ShipPostalCode = row["ShipPostalCode"];
                dao.ShipCountry = row["ShipCountry"];

                // 自動生成Daoを実行
                sb.Append(dao.ExecGenerateSQL(
                    "DaoOrders2_S1_Insert.sql", new SQLUtility(DbEnum.DBMSType.SQLServer)) + ";\r\n");
                    // "DaoOrders2_D1_Insert.xml", new SQLUtility(DbEnum.DBMSType.SQLServer)) + ";\r\n"); // 性能比較用

                // ↑DBアクセス-----------------------------------------------------
            }

            // 共通Daoでバッチ更新
            CmnDao cd = new CmnDao(this.GetDam());
            cd.SQLText = sb.ToString();
            cd.ExecInsUpDel_NonQuery();

            // todo:中間コミット情報をDBに登録 ※最終処理主キー値の登録など

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #endregion
    }
}
