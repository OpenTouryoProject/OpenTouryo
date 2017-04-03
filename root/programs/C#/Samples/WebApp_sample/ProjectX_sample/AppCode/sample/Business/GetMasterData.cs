//**********************************************************************************
//* テーブル・メンテナンス自動生成・テストクラス（マスタデータ・ロード部品
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：GetMasterData
//* クラス日本語名  ：マスタデータ・ロード部品
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System.Data;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;

namespace ProjectX_sample
{
    /// <summary>マスタデータ・ロード部品</summary>
    public class GetMasterData : MyFcBaseLogic
    {
        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_Invoke(_3TierParameterValue parameterValue)
        { //メソッド引数にBaseParameterValueの派生の型を定義可能。

            // 戻り値クラスを生成して、事前に戻り値に設定しておく。
            _3TierReturnValue returnValue = new _3TierReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            // データアクセス クラスを生成する
            DaoSuppliers daoSuppliers = new DaoSuppliers(this.GetDam());

            // 全件参照
            DataTable dt1 = new DataTable();
            daoSuppliers.D2_Select(dt1);

            // データアクセス クラスを生成する
            DaoCategories daoCategories = new DaoCategories(this.GetDam());

            // 実行
            DataTable dt2 = new DataTable();
            daoCategories.D2_Select(dt2);

            // 戻り値を戻す
            returnValue.Obj = new DataTable[] { dt1, dt2 };

            // ↑業務処理-----------------------------------------------------
        }
    } 
}