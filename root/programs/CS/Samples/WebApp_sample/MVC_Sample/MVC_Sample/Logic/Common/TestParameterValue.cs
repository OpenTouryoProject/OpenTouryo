//**********************************************************************************
//* フレームワーク・テストクラス（引数・戻り値）
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

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

using MVC_Sample.Models.ViewModels;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

namespace MVC_Sample.Logic.Common
{
    public class TestParameterValue : MyParameterValue
    {
        /// <summary>shipper</summary>
        public ShipperViweModel Shipper { get; set; }

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