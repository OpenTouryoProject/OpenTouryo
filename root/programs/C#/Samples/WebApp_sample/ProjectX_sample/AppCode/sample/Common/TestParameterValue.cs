//**********************************************************************************
//* フレームワーク・テストクラス
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：TestParameterValue
//* クラス日本語名  ：テスト用の引数クラス
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

// ベースクラス
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Common;

namespace MyType
{
    /// <summary>
    /// TestParameterValue の概要の説明です
    /// </summary>
    public class TestParameterValue : MyParameterValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>ShipperID</summary>
        public int ShipperID;

        /// <summary>CompanyName</summary>
        public string CompanyName;

        /// <summary>Phone</summary>
        public string Phone;

        /// <summary>OrderColumn</summary>
        public string OrderColumn;

        /// <summary>OrderSequence</summary>
        public string OrderSequence;

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public TestParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}
