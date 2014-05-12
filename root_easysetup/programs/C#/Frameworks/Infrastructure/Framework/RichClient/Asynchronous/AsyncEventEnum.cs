//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：AsyncEventEnum
//* クラス日本語名  ：Framework.RichClient.Asynchronous名前空間で使用する列挙型クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/12/21  西野  大介        新規作成
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.RichClient.Asynchronous
{
    /// <summary>Framework.RichClient.Asynchronous名前空間で使用する列挙型クラス</summary>
    /// <remarks>特定の箇所で利用できる。</remarks>
    public struct AsyncEventEnum
    {
        /// <summary>イベント区分の列挙型</summary>
        /// <remarks>自由に利用できる。</remarks>
        public enum EventClass : int
        {
            /// <summary>スレッド</summary>
            /// <remarks>通常0スタート、C言語から使うので明記した</remarks>
            Thread = 0,

            /// <summary>スレッド プール</summary>
            ThreadPool,

            /// <summary>Windows Forms</summary>
            WinForm,
            
            /// <summary>WPF</summary>
            WPF
        }
    }
}
