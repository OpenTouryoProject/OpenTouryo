//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：LayerB_Static
//* クラス日本語名  ：Ｂ層（静的SQLのCRUD：Suppliersテーブル）
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// 型情報
using GenDaoAndBatUpd_sample.Common;

// System
using System;

// データセット利用
using System.Data;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.RichClient.Business;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.RichClient.Business;

// 部品
using Touryo.Infrastructure.Public.Util;

namespace GenDaoAndBatUpd_sample.Business
{
    /// <summary>Ｂ層（静的SQLのCRUD：Suppliersテーブル）</summary>
    class LayerB_Static : MyFcBaseLogic2CS
    {
        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_Insert(BaseParameterValue parameterValue)
        {
            // 引数クラスをアップキャスト
            TestParameterValue testParameter = (TestParameterValue)parameterValue;

            // 戻り値クラスを生成
            TestReturnValue testReturn = new TestReturnValue();

            // ↓業務処理-----------------------------------------------------

            // データアクセス クラスを生成する
            DaoSuppliers daoSuppliers = new DaoSuppliers(this.GetDam());

            // １件挿入
            //daoSuppliers.PK_SupplierID = testParameter.field1;
            daoSuppliers.CompanyName = testParameter.field2;
            daoSuppliers.ContactName = testParameter.field3;
            daoSuppliers.ContactTitle = testParameter.field4;
            daoSuppliers.Address = testParameter.field5;
            daoSuppliers.City = testParameter.field6;
            daoSuppliers.Region = testParameter.field7;
            daoSuppliers.PostalCode = testParameter.field8;
            daoSuppliers.Country = testParameter.field9;
            daoSuppliers.Phone = testParameter.field10;
            daoSuppliers.Fax = testParameter.field11;
            daoSuppliers.HomePage = testParameter.field12;

            testReturn.obj = daoSuppliers.S1_Insert();

            // ↑業務処理-----------------------------------------------------

            // 戻り値クラスをダウンキャストして戻す
            this.ReturnValue = (BaseReturnValue)testReturn;
        }

        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_Select(BaseParameterValue parameterValue)
        {
            // 引数クラスをアップキャスト
            TestParameterValue testParameter = (TestParameterValue)parameterValue;

            // 戻り値クラスを生成
            TestReturnValue testReturn = new TestReturnValue();

            // ↓業務処理-----------------------------------------------------

            // データアクセス クラスを生成する
            DaoSuppliers daoSuppliers = new DaoSuppliers(this.GetDam());

            // １件参照
            daoSuppliers.PK_SupplierID = testParameter.field1;

            DataTable dt = new DataTable();
            daoSuppliers.S2_Select(dt);

            testReturn.field1 = dt.Rows[0][0];
            testReturn.field2 = dt.Rows[0][1];
            testReturn.field3 = dt.Rows[0][2];
            testReturn.field4 = dt.Rows[0][3];
            testReturn.field5 = dt.Rows[0][4];
            testReturn.field6 = dt.Rows[0][5];
            testReturn.field7 = dt.Rows[0][6];
            testReturn.field8 = dt.Rows[0][7];
            testReturn.field9 = dt.Rows[0][8];
            testReturn.field10 = dt.Rows[0][9];
            testReturn.field11 = dt.Rows[0][10];
            testReturn.field12 = dt.Rows[0][11];

            // ↑業務処理-----------------------------------------------------

            // 戻り値クラスをダウンキャストして戻す
            this.ReturnValue = (BaseReturnValue)testReturn;
        }

        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_Update(BaseParameterValue parameterValue)
        {
            // 引数クラスをアップキャスト
            TestParameterValue testParameter = (TestParameterValue)parameterValue;

            // 戻り値クラスを生成
            TestReturnValue testReturn = new TestReturnValue();

            // ↓業務処理-----------------------------------------------------

            // データアクセス クラスを生成する
            DaoSuppliers daoSuppliers = new DaoSuppliers(this.GetDam());

            // １件更新
            daoSuppliers.PK_SupplierID = testParameter.field1;

            if (testParameter.field2_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_CompanyName_forUPD = testParameter.field2_ForUpd; }

            if (testParameter.field3_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_ContactName_forUPD = testParameter.field3_ForUpd; }

            if (testParameter.field4_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_ContactTitle_forUPD = testParameter.field4_ForUpd; }

            if (testParameter.field5_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_Address_forUPD = testParameter.field5_ForUpd; }

            if (testParameter.field6_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_City_forUPD = testParameter.field6_ForUpd; }

            if (testParameter.field7_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_Region_forUPD = testParameter.field7_ForUpd; }

            if (testParameter.field8_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_PostalCode_forUPD = testParameter.field8_ForUpd; }

            if (testParameter.field9_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_Country_forUPD = testParameter.field9_ForUpd; }

            if (testParameter.field10_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_Phone_forUPD = testParameter.field10_ForUpd; }

            if (testParameter.field11_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_Fax_forUPD = testParameter.field11_ForUpd; }

            if (testParameter.field12_ForUpd.ToString().Trim() == "") { }
            else
            { daoSuppliers.Set_HomePage_forUPD = testParameter.field12_ForUpd; }

            testReturn.obj = daoSuppliers.S3_Update();

            // ↑業務処理-----------------------------------------------------

            // 戻り値クラスをダウンキャストして戻す
            this.ReturnValue = (BaseReturnValue)testReturn;
        }

        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_Delete(BaseParameterValue parameterValue)
        {
            // 引数クラスをアップキャスト
            TestParameterValue testParameter = (TestParameterValue)parameterValue;

            // 戻り値クラスを生成
            TestReturnValue testReturn = new TestReturnValue();

            // ↓業務処理-----------------------------------------------------

            // データアクセス クラスを生成する
            DaoSuppliers daoSuppliers = new DaoSuppliers(this.GetDam());

            // １件削除
            daoSuppliers.PK_SupplierID = testParameter.field1;

            testReturn.obj = daoSuppliers.S4_Delete();

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
            DaoSuppliers daoSuppliers = new DaoSuppliers(this.GetDam());

            // 全件参照
            DataTable dt = new DataTable();
            daoSuppliers.D2_Select(dt);

            // 戻り値を戻す
            testReturn.dt = dt;
            
            // ↑業務処理-----------------------------------------------------

            // 戻り値クラスをダウンキャストして戻す
            this.ReturnValue = (BaseReturnValue)testReturn;
        }
    }
}
