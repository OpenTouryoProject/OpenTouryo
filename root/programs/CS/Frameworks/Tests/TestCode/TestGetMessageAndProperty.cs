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
//* クラス名        ：TestGetMessageAndProperty
//* クラス日本語名  ：メッセージ・プロパティ取得のテスト
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/07/31  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestGetMessageAndProperty
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            MyDebug.OutputDebugAndConsole("GetMessage: " + GetMessage.GetMessageDescription("I0001"));
            MyDebug.OutputDebugAndConsole("GetMessage: " + GetMessage.GetMessageDescription("E0001"));

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            MyDebug.OutputDebugAndConsole("GetSharedProperty: " + GetSharedProperty.GetSharedPropertyValue("ConnectionString1"));
            MyDebug.OutputDebugAndConsole("GetSharedProperty: " + GetSharedProperty.GetSharedPropertyValue("HostName1"));
        }
        #endregion
    }
}