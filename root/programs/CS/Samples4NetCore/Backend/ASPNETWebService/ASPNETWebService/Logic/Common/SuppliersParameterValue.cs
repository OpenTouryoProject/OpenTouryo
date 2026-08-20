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
//* クラス名        ：SuppliersParameterValue
//* クラス日本語名  ：Suppliers の引数クラス
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/20  玄人 幸道         新規作成（#570）
//**********************************************************************************

using System.Data;

using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

namespace ASPNETWebService.Logic.Common
{
    /// <summary>Suppliers の引数クラス</summary>
    public class SuppliersParameterValue : MyParameterValue
    {
        /// <summary>バッチ更新の対象</summary>
        /// <remarks>
        /// **RowState と Original を持ったまま渡ってくる。**
        /// WebAPI 越しでは DTTables を経由するため、
        /// DTTable.FromDataTable(dt, keepOriginal: true) で作られている必要がある（#567）。
        /// </remarks>
        public DataTable Suppliers { get; set; }

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="screenId">画面ID</param>
        /// <param name="controlId">コントロールID</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="user">ユーザ情報</param>
        public SuppliersParameterValue(
            string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}
