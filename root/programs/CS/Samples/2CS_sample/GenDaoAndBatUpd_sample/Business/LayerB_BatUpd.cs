//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：LayerB_BatUpd
//* クラス日本語名  ：Ｂ層（静的SQLのCRUD：Productsテーブル）
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using GenDaoAndBatUpd_sample.Common;

using System.Data;

using Touryo.Infrastructure.Business.RichClient.Business;
using Touryo.Infrastructure.Framework.Common;

namespace GenDaoAndBatUpd_sample.Business
{
    /// <summary>Ｂ層（静的SQLのCRUD：Productsテーブル）</summary>
    class LayerB_BatUpd : MyFcBaseLogic2CS
    {
        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_BatUpd(BaseParameterValue parameterValue)
        {
            // 引数クラスをアップキャスト
            TestParameterValue testParameter = (TestParameterValue)parameterValue;

            // 戻り値クラスを生成
            TestReturnValue testReturn = new TestReturnValue();

            // ↓業務処理-----------------------------------------------------

            // データアクセス クラスを生成する
            DaoProducts daoProducts = new DaoProducts(this.GetDam());

            // ROW毎に処理
            foreach (DataRow dr in testParameter.dt.Rows)
            {
                // パラメタをクリアする。
                daoProducts.ClearParametersFromHt();

                switch (dr.RowState)
                {
                    case DataRowState.Added:

                        #region １件挿入

                        // 設定（インサート値）
                        daoProducts.PK_ProductID = dr["ProductID"].ToString();
                        daoProducts.ProductName = dr["ProductName"].ToString();
                        daoProducts.SupplierID = dr["SupplierID"].ToString();
                        daoProducts.CategoryID = dr["CategoryID"].ToString();
                        daoProducts.QuantityPerUnit = dr["QuantityPerUnit"].ToString();
                        daoProducts.UnitPrice = dr["UnitPrice"].ToString();
                        daoProducts.UnitsInStock = dr["UnitsInStock"].ToString();
                        daoProducts.UnitsOnOrder = dr["UnitsOnOrder"].ToString();
                        daoProducts.ReorderLevel = dr["ReorderLevel"].ToString();
                        daoProducts.Discontinued = dr["Discontinued"].ToString();

                        // インサート（S1でよい）
                        testReturn.obj = daoProducts.S1_Insert();

                        #endregion

                        break;

                    case DataRowState.Deleted:

                        #region １件削除

                        // 設定（主キー）
                        daoProducts.PK_ProductID = dr["ProductID", DataRowVersion.Original].ToString();
                        // ★ 楽観排他をする場合は、ここにタイムスタンプを追加する。

                        // デリート（タイムスタンプを指定する場合は、D4_Delete）
                        testReturn.obj = daoProducts.D4_Delete();

                        #endregion

                        break;

                    case DataRowState.Modified:

                        #region １件更新

                        // 設定（主キー）
                        daoProducts.PK_ProductID = dr["ProductID"].ToString();

                        // ★ 楽観排他をする場合は、ここにタイムスタンプを追加する。
                        // ↓は、DataRowVersion.Originalを使用した楽観排他の例
                        daoProducts.ProductName = dr["ProductName", DataRowVersion.Original].ToString();
                        daoProducts.SupplierID = dr["SupplierID", DataRowVersion.Original].ToString();
                        daoProducts.CategoryID = dr["CategoryID", DataRowVersion.Original].ToString();
                        daoProducts.QuantityPerUnit = dr["QuantityPerUnit", DataRowVersion.Original].ToString();
                        daoProducts.UnitPrice = dr["UnitPrice", DataRowVersion.Original].ToString();
                        daoProducts.UnitsInStock = dr["UnitsInStock", DataRowVersion.Original].ToString();
                        daoProducts.UnitsOnOrder = dr["UnitsOnOrder", DataRowVersion.Original].ToString();
                        daoProducts.ReorderLevel = dr["ReorderLevel", DataRowVersion.Original].ToString();
                        daoProducts.Discontinued = dr["Discontinued", DataRowVersion.Original].ToString();

                        // 更新値設定
                        daoProducts.Set_ProductName_forUPD = dr["ProductName"].ToString();
                        daoProducts.Set_SupplierID_forUPD = dr["SupplierID"].ToString();
                        daoProducts.Set_CategoryID_forUPD = dr["CategoryID"].ToString();
                        daoProducts.Set_QuantityPerUnit_forUPD = dr["QuantityPerUnit"].ToString();
                        daoProducts.Set_UnitPrice_forUPD = dr["UnitPrice"].ToString();
                        daoProducts.Set_UnitsInStock_forUPD = dr["UnitsInStock"].ToString();
                        daoProducts.Set_UnitsOnOrder_forUPD = dr["UnitsOnOrder"].ToString();
                        daoProducts.Set_ReorderLevel_forUPD = dr["ReorderLevel"].ToString();
                        daoProducts.Set_Discontinued_forUPD = dr["Discontinued"].ToString();

                        // アップデート（タイムスタンプを指定する場合は、D3_Update）
                        testReturn.obj = daoProducts.D3_Update();

                        #endregion

                        break;

                    default:
                        break;
                }
            }

            // ↑業務処理-----------------------------------------------------

            // 戻り値クラスをダウンキャストして戻す
            this.ReturnValue = (BaseReturnValue)testReturn;
        }

        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_SelectAll(BaseParameterValue parameterValue)
        {
            // 引数クラスをアップキャスト
            TestParameterValue testParameter = (TestParameterValue)parameterValue;

            // 戻り値クラスを生成
            TestReturnValue testReturn = new TestReturnValue();

            // ↓業務処理-----------------------------------------------------

            // データアクセス クラスを生成する
            DaoProducts daoProducts = new DaoProducts(this.GetDam());

            // 全件取得
            DataTable dt = new DataTable();
            daoProducts.D2_Select(dt);

            // 戻り値を戻す
            testReturn.dt = dt;

            // ↑業務処理-----------------------------------------------------

            // 戻り値クラスをダウンキャストして戻す
            this.ReturnValue = (BaseReturnValue)testReturn;
        }
    }
}
