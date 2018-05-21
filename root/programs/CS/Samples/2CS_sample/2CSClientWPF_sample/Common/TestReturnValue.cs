//**********************************************************************************
//* フレームワーク・テストクラス
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：TestReturnValue
//* クラス日本語名  ：テスト用の戻り値クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using Touryo.Infrastructure.Business.Common;

namespace _2CSClientWPF_sample.Common
{
    /// <summary>
    /// TestReturnValueの概要の説明です
    /// </summary>
    public class TestReturnValue : MyReturnValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>ShipperID</summary>
        public int ShipperID;

        /// <summary>CompanyName</summary>
        public string CompanyName;

        /// <summary>Phone</summary>
        public string Phone;
    }
}
