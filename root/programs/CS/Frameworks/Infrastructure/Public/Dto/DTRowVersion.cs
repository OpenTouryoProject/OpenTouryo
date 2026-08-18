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
//* クラス名        ：DTRowVersion
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（セルのバージョン列挙型）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/18  玄人 幸道         新規作成（#567）
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>セルのバージョン</summary>
    /// <remarks>
    /// System.Data.DataRowVersion に相当する。
    /// DTTable.KeepOriginal が真のときだけ Original が意味を持つ。
    /// </remarks>
    public enum DTRowVersion : int
    {
        /// <summary>
        /// 現在の値
        /// </summary>
        Current = 1,

        /// <summary>
        /// 変更前の値
        /// </summary>
        /// <remarks>
        /// 変更していない行（Unchanged / Added / Deleted）では、現在の値と同じです。
        /// DTTable.KeepOriginal が偽の場合も、現在の値が返ります。
        /// </remarks>
        Original = 2
    }
}
