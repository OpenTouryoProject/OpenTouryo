//**********************************************************************************
//* バッチ更新処理・サンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：TestParameterValue
//* クラス日本語名  ：テスト用の引数クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Data;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

namespace GenDaoAndBatUpd_sample.Common
{
    /// <summary>
    /// TestParameterValue の概要の説明です
    /// </summary>
    public class TestParameterValue : MyParameterValue
    {
        #region コンストラクタ

        public TestParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion

        #region フィールド

        // 値（インサート、主キー値など）
        public object field1;
        public object field2;
        public object field3;
        public object field4;
        public object field5;
        public object field6;
        public object field7;
        public object field8;
        public object field9;
        public object field10;
        public object field11;
        public object field12;
        public object field13;
        public object field14;
        public object field15;
        public object field16;
        public object field17;
        public object field18;
        public object field19;
        public object field20;

        // 更新時
        public object field1_ForUpd;
        public object field2_ForUpd;
        public object field3_ForUpd;
        public object field4_ForUpd;
        public object field5_ForUpd;
        public object field6_ForUpd;
        public object field7_ForUpd;
        public object field8_ForUpd;
        public object field9_ForUpd;
        public object field10_ForUpd;
        public object field11_ForUpd;
        public object field12_ForUpd;
        public object field13_ForUpd;
        public object field14_ForUpd;
        public object field15_ForUpd;
        public object field16_ForUpd;
        public object field17_ForUpd;
        public object field18_ForUpd;
        public object field19_ForUpd;
        public object field20_ForUpd;

        // 検索条件
        public object field1_ForSearch;
        public object field2_ForSearch;
        public object field3_ForSearch;
        public object field4_ForSearch;
        public object field5_ForSearch;
        public object field6_ForSearch;
        public object field7_ForSearch;
        public object field8_ForSearch;
        public object field9_ForSearch;
        public object field10_ForSearch;
        public object field11_ForSearch;
        public object field12_ForSearch;
        public object field13_ForSearch;
        public object field14_ForSearch;
        public object field15_ForSearch;
        public object field16_ForSearch;
        public object field17_ForSearch;
        public object field18_ForSearch;
        public object field19_ForSearch;
        public object field20_ForSearch;

        #endregion

        public DataTable dt;
        public object obj;
    }
}
