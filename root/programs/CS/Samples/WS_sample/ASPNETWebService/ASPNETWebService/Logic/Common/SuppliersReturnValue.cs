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
//* クラス名        ：SuppliersReturnValue
//* クラス日本語名  ：Suppliers の戻り値クラス
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

namespace ASPNETWebService.Logic.Common
{
    /// <summary>Suppliers の戻り値クラス</summary>
    public class SuppliersReturnValue : MyReturnValue
    {
        /// <summary>件数</summary>
        public int Count;

        /// <summary>一覧</summary>
        public DataTable Suppliers;

        /// <summary>追加した件数</summary>
        public int InsertCount;

        /// <summary>更新した件数</summary>
        public int UpdateCount;

        /// <summary>削除した件数</summary>
        public int DeleteCount;
    }
}
