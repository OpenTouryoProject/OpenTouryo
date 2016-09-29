//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

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
