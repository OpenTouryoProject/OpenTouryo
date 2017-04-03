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
//* クラス名        ：MuParameterValue
//* クラス日本語名  ：汎用引数クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/06/18  西野 大介         汎用サービスインターフェイスの追加のため
//*  2010/03/03  西野 大介         自動振り分け処理に使用するメソッド名を追加
//*  2010/03/11  西野 大介         引数の順番を変更した。
//*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
//*  2010/12/13  西野 大介         汎用サービスI/F追加に伴いデータセット配列を追加
//*  2011/12/02  西野 大介         上記を汎用利用可能なobjectに変更した（Bean、DS・DT）。
//**********************************************************************************

using System;
using Touryo.Infrastructure.Business.Util;

namespace Touryo.Infrastructure.Business.Common
{
    /// <summary>汎用引数クラス</summary>
    /// <remarks>
    /// シリアライズ可能にする（WS対応）自由に（拡張して）利用できる。
    /// </remarks>
    [Serializable()]
    public class MuParameterValue : MyParameterValue
    {
        /// <summary>文字列</summary>
        public string Value;

        /// <summary>Bean</summary>
        /// <remarks>DS・DT、配列可</remarks>
        public object Bean;

        #region コンストラクタ

        /// <summary>コンストラクタ（下位互換）</summary>
        public MuParameterValue(string screenId, string controlId, string actionType, MyUserInfo user)
            : base(screenId, controlId, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        /// <summary>コンストラクタ</summary>
        public MuParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}
