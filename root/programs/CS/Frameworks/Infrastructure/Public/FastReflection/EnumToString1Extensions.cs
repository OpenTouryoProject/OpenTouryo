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
//* クラス名        ：EnumToString1Extensions
//* クラス日本語名  ：EnumToString1Extensions
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/16  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Collections.Concurrent;

namespace Touryo.Infrastructure.Public.FastReflection
{
    // 元も子もないケド、
    // https://twitter.com/xin9le/status/699123907937185792
    // 単なる、キャッシュ版

    // 因みに。
    // - EnumToString2Extensions.ToString2は、Emit版
    // - EnumToString3Extensions.ToString3は、式木版（予）

    /// <summary>EnumToString1Extensions</summary>
    public static class EnumToString1Extensions
    {
        /// <summary>スレッドセーフ</summary>
        private static ConcurrentDictionary<Type, string>
            ToStringValue = new ConcurrentDictionary<Type, string>();

        /// <summary>ToString1（キャッシュ版）</summary>
        /// <typeparam name="T">struct(Enum Field)</typeparam>
        /// <param name="value">値</param>
        /// <returns>列挙型を文字列化</returns>
        public static string ToString1<T>(this Nullable<T> value) where T : struct
        {
            if (value.HasValue == true)
            {
                return EnumToString1Extensions.ToString1(value.Value);
            }
            else
            {
                return "";
            }
        }

        /// <summary>ToString1（キャッシュ版）</summary>
        /// <typeparam name="T">struct(Enum Field)</typeparam>
        /// <param name="value">値</param>
        /// <returns>列挙型を文字列化</returns>
        public static string ToString1<T>(this T value) where T : struct
        {
            // Enum Field
            Type type = typeof(T);

            if (type.IsEnum == false)
            {
                throw new ArgumentException("value must be a enum type");
            }
            else
            {
                string toStringValue = "";

                // ToStringValueのロード
                if (!EnumToString1Extensions.ToStringValue.TryGetValue(type, out toStringValue))
                {
                    // コレが遅いらしい。
                    toStringValue = value.ToString();
                    // toStringValueをキャッシュ
                    EnumToString1Extensions.ToStringValue[type] = toStringValue;
                }
                
                return toStringValue;
            }
        }
    }
}