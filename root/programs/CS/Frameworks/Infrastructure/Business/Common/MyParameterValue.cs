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
//* クラス名        ：MyParameterValue
//* クラス日本語名  ：引数親クラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/04/02  西野 大介         シリアライズ可能にする（WS対応）
//*  2010/03/03  西野 大介         自動振り分け処理に使用するメソッド名を追加
//*  2010/03/11  西野 大介         引数の順番を変更した。
//*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納
//**********************************************************************************

using System;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Common;

namespace Touryo.Infrastructure.Business.Common
{
    /// <summary>引数親クラス２</summary>
    /// <remarks>
    /// シリアライズ可能にする（WS対応）自由に（拡張して）利用できる。
    /// </remarks>
    [Serializable()]
    public class MyParameterValue : BaseParameterValue
    {
        #region インスタンス変数

        /// <summary>ユーザ情報</summary>
        private MyUserInfo _user;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="screenId">スクリーンID</param>
        /// <param name="controlId">コントロールID</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="user">ユーザ情報</param>
        /// <remarks>
        /// コンストラクタは継承されないので、派生先で呼び出す必要がある。
        /// ※ コンストラクタの実行順は、基本クラス→派生クラスの順
        /// ※ VB.NET では、MyBase.New() を派生クラスのコンストラクタから呼ぶ。
        /// 自由に利用できる（互換性の維持）。
        /// </remarks>
        public MyParameterValue(string screenId, string controlId, string actionType, MyUserInfo user)
            : base(screenId, controlId, actionType)
        {
            // ユーザ情報
            this._user = user;
        }
        
        /// <summary>コンストラクタ</summary>
        /// <param name="screenId">スクリーンID</param>
        /// <param name="controlId">コントロールID</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="user">ユーザ情報</param>
        /// <remarks>
        /// コンストラクタは継承されないので、派生先で呼び出す必要がある。
        /// ※ コンストラクタの実行順は、基本クラス→派生クラスの順
        /// ※ VB.NET では、MyBase.New() を派生クラスのコンストラクタから呼ぶ。
        /// 自由に利用できる。
        /// </remarks>
        public MyParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType)
        {
            // ユーザ情報
            this._user = user;
        }

        #endregion

        #region プロパティ

        /// <summary>ユーザ情報（読み取り専用）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public MyUserInfo User
        {
            get
            {
                return this._user;
            }
        }

        #endregion        
    }
}
