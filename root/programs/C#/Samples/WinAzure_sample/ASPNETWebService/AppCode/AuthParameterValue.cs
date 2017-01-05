//**********************************************************************************
//* フレームワーク・テストクラス
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：AuthParameterValue
//* クラス日本語名  ：認証処理用の引数クラス
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//**********************************************************************************

// System
using System;

// ベースクラス
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Common;

namespace Touryo.Infrastructure.Framework.ServiceInterface.ASPNETWebService
{
    /// <summary>
    /// AuthParameterValue の概要の説明です
    /// </summary>
    /// <remarks>シリアライズ可能にする（WS対応）</remarks>
    [Serializable()]
    public class AuthParameterValue : MyParameterValue
    {
        /// <summary>パスワード</summary>
        public object Password;

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public AuthParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user, string password)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
            // パスワードを設定する。
            this.Password = password;
        }

        #endregion
    }
}
