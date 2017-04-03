//**********************************************************************************
//* フレームワーク・テストクラス
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：ExecuteBatchProcessParameterValue
//* クラス日本語名  ：テスト用の引数クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技セ
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Collections;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

namespace RerunnableBatch_sample2.Common
{
    /// <summary>
    /// ExecuteBatchProcessParameterValue の概要の説明です
    /// </summary>
    public class ExecuteBatchProcessParameterValue : MyParameterValue
    {
        /// <summary>1トランザクションで処理を行う主キー一覧</summary>
        public ArrayList SubPkList;

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public ExecuteBatchProcessParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}
