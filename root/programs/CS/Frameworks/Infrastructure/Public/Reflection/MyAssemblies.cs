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
//* クラス名        ：MyAssemblies
//* クラス日本語名  ：アセンブリ取得用
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/29  西野 大介         新規作成（分割
//**********************************************************************************

using System;
using System.Reflection;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Reflection
{
    /// <summary>アセンブリ取得用</summary>
    /// <remarks>自由に利用できる。</remarks>
    public static class MyAssemblies
    {
        #region Assemblyのラッパ
        
        /// <summary>GetEntryAssembly</summary>
        /// <returns>Assembly</returns>
        public static Assembly GetEntryAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
        /// <summary>GetExecutingAssembly</summary>
        /// <returns>Assembly</returns>
        public static Assembly GetExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
        /// <summary>GetCallingAssembly</summary>
        /// <returns>Assembly</returns>
        public static Assembly GetCallingAssembly()
        {
            return Assembly.GetCallingAssembly();
        }

        /// <summary>GetAssembly</summary>
        /// <param name="t">Type</param>
        /// <returns>Assembly</returns>
        public static Assembly GetAssembly(Type t)
        {
            return Assembly.GetAssembly(t);
        }

        #endregion

        /// <summary>アセンブリを取得する</summary>
        /// <param name="assemblyString">
        /// アセンブリ名（"既定の名前空間"とは異なる）
        /// </param>
        /// <returns>Assembly</returns>
        public static Assembly GetAssembly(string assemblyString)
        {
            // Azureスイッチ
            string azure = GetConfigParameter.GetConfigValue("Azure");

            if (string.IsNullOrEmpty(azure))
            {
                // Azureスイッチ・OFFの場合（通常時
                if (string.IsNullOrEmpty(assemblyString))
                {
                    // assemblyString 指定なし
                    return Assembly.GetEntryAssembly();
                }
                else
                {
                    // assemblyString 指定あり
                    return Assembly.Load(assemblyString);
                }
            }
            else
            {
                // Azureスイッチ・ONの場合
                if (string.IsNullOrEmpty(assemblyString))
                {
                    // assemblyString 指定なし
                    return Assembly.Load(azure);
                }
                else
                {
                    // assemblyString 指定あり
                    return Assembly.Load(assemblyString);
                }

            }
        }
    }
}
