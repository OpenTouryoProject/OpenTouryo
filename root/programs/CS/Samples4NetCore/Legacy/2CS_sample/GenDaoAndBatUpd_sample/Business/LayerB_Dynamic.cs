//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：LayerB_Dynamic
//* クラス日本語名  ：Ｂ層（動的SQLのCRUD：Categoryテーブル）
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
    /// <summary>Ｂ層（動的SQLのCRUD：Categoryテーブル）</summary>
    class LayerB_Dynamic : MyFcBaseLogic2CS
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
            DaoCategories daoCategories = new DaoCategories(this.GetDam());

            // １件挿入
            //daoCategories.PK_CategoryID = testParameter.field1;
            daoCategories.CategoryName = testParameter.field2;
            daoCategories.Description = testParameter.field3;
            //daoCategories.Picture = testParameter.field4;

            // インサート
            testReturn.obj = daoCategories.D1_Insert();

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
            DaoCategories daoCategories = new DaoCategories(this.GetDam());

            // ｎ件参照
            if (testParameter.field1_ForSearch.ToString().Trim() == "") { }
            else
            { daoCategories.PK_CategoryID = testParameter.field1_ForSearch; }

            if (testParameter.field2_ForSearch.ToString().Trim() == "") { }
            else
            { daoCategories.CategoryName = testParameter.field2_ForSearch; }

            DataTable dt = new DataTable();
            daoCategories.D2_Select(dt);

            testReturn.dt = dt;

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
            DaoCategories daoCategories = new DaoCategories(this.GetDam());

            // ｎ件更新

            // 更新値設定
            if (testParameter.field2_ForUpd.ToString().Trim() == "") { }
            else
            { daoCategories.Set_CategoryName_forUPD = testParameter.field2_ForUpd; }

            if (testParameter.field3_ForUpd.ToString().Trim() == "") { }
            else
            { daoCategories.Set_Description_forUPD = testParameter.field3_ForUpd; }

            // 検索条件設定
            if (testParameter.field1_ForSearch.ToString().Trim() == "") { }
            else
            { daoCategories.PK_CategoryID = testParameter.field1_ForSearch; }

            if (testParameter.field2_ForSearch.ToString().Trim() == "") { }
            else
            { daoCategories.CategoryName = testParameter.field2_ForSearch; }

            // アップデート
            testReturn.obj = daoCategories.D3_Update();

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
            DaoCategories daoCategories = new DaoCategories(this.GetDam());

            // ｎ件削除

            // 検索条件設定
            if (testParameter.field1_ForSearch.ToString().Trim() == "") { }
            else
            { daoCategories.PK_CategoryID = testParameter.field1_ForSearch; }

            if (testParameter.field2_ForSearch.ToString().Trim() == "") { }
            else
            { daoCategories.CategoryName = testParameter.field2_ForSearch; }

            // デリート
            testReturn.obj = daoCategories.D4_Delete();

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
            DaoCategories daoCategories = new DaoCategories(this.GetDam());

            // 全件取得

            // 実行
            DataTable dt = new DataTable();
            daoCategories.D2_Select(dt);

            // 戻り値を戻す
            testReturn.dt = dt;

            // ↑業務処理-----------------------------------------------------

            // 戻り値クラスをダウンキャストして戻す
            this.ReturnValue = (BaseReturnValue)testReturn;
        }
    }
}
