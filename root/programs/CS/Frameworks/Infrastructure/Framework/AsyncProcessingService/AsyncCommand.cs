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
//* クラス名        ：AsyncCommand
//* クラス日本語名  ：AsyncCommand
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/24  西野 大介         新規作成（非同期処理サービスから）
//**********************************************************************************

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.AsyncProcessingService
{
    /// <summary>
    /// AsyncCommand Enum for storing command values
    /// </summary>
    public enum AsyncCommand
    {
        /// <summary>Stop</summary>
        Stop = 1,

        /// <summary>Abort</summary>
        Abort
    }
}
