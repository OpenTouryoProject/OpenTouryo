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
//* クラス名        ：MyDebug
//* クラス日本語名  ：MyDebugクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/12  西野 大介         新規作成（X PlatformのDebugで利用拡大？
//*  2018/11/29  西野 大介         ASP.NET Coreの出力で2つ出力されるので...
//*  2019/03/20  西野 大介         リネーム（名前空間、クラス名、メソッド名）
//**********************************************************************************

using System;
using System.Diagnostics;

namespace Touryo.Infrastructure.Public.Diagnostics
{
    /// <summary>MyDebugクラス</summary>
    public class MyDebug
    {
        /// <summary>OutputDebugAndConsole</summary>
        /// <param name="testLabel">string</param>
        /// <param name="s">string</param>
        public static void OutputDebugAndConsole(string testLabel, string s)
        {
            OutputDebugAndConsole(testLabel + " > " + s);
        }

        /// <summary>CanOutputToConsole</summary>
        public static bool CanOutputToConsole = true;

        /// <summary>CanOutputToDebug</summary>
        public static bool CanOutputToDebug = true;

        /// <summary>OutputDebugAndConsole</summary>
        /// <param name="s">string</param>
        public static void OutputDebugAndConsole(string s)
        {
            if (MyDebug.CanOutputToConsole) Console.WriteLine(s);
            if (MyDebug.CanOutputToDebug) Debug.WriteLine(s);
        }
    }
}
