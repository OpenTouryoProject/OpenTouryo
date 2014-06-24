//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：DataRowState
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（行ステータス列挙型）
//*
//* 作成者          ：生技 前川
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/11/11  前川  祐介        新規作成
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>
    /// 行ステータスを表す列挙型
    /// </summary>
    [FlagsAttribute]
    public enum DataRowState
    {
        /// <summary>
        /// 行が、どの行リストにも属さない状態
        /// </summary>
        Detached = 1, 

        /// <summary>
        /// 行リストに行が追加された状態
        /// </summary>
        Added = 2,

        /// <summary>
        /// 行の値が変更された状態
        /// </summary>
        Modified = 4,

        /// <summary>
        /// 行が削除された状態
        /// </summary>
        Deleted = 8,

        /// <summary>
        /// 行の値が確定している状態
        /// </summary>
        Unchanged = 16
    }
}
