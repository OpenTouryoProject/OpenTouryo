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
//* クラス名        ：AccessorCacher
//* クラス日本語名  ：AccessorCacher
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/04  西野 大介         新規作成
//*  2018/10/23  西野 大介         微調整
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.FastReflection
{
    /// <summary>AccessorCacher</summary>
    public class AccessorCacher
    {
        // [雑記] 動的コード生成のパフォーマンス - C# によるプログラミング入門 | ++C++; // 未確認飛行 C
        // https://ufcpp.net/study/csharp/misc_dynamic.html#expressiontree

        // 構文木レベルで生成したアクセッサのキャッシュ
        // ・静的なコードの数倍程度までは速くできる。
        // ・IL 生成するものと比べてそう大きな差はない。

        /// <summary>AccessorCacher</summary>
        private static ConcurrentDictionary<Type, List<AccessorInfo>> _CncDic = new ConcurrentDictionary<Type, List<AccessorInfo>>();

        /// <summary>AccessorCacher</summary>
        public static ConcurrentDictionary<Type, List<AccessorInfo>> CncDic
        {
            get { return _CncDic; }
        }

        /// <summary>CacheAccessor</summary>
        /// <param name="obj">object</param>
        public static void CacheAccessor(object obj)
        {
            Type t = obj.GetType();

            if (!AccessorCacher.CncDic.ContainsKey(t))
            {
                AccessorInfo ai = null;
                List<AccessorInfo> aiList = new List<AccessorInfo>();

                // PropertyInfo
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    // PropertyInfoは、GetGetMethod, GetSetMethodでチェック
                    ai = new AccessorInfo()
                    {
                        AccessorName = prop.Name,
                        AccessorType = prop.PropertyType,
                        UnderlyingType = PubCmnFunction.GetUnderlyingType(prop.PropertyType),
                        GetDelegate = prop.GetGetMethod() == null ? null 
                            : CompiledExpressionCreater.CreateGetterOfPropertyOrField(t, prop.Name),
                        SetDelegate = prop.GetSetMethod() == null ? null
                            : CompiledExpressionCreater.CreateSetterOfPropertyOrField(t, prop.Name)
                    };
                    aiList.Add(ai);
                }

                // FieldInfo
                foreach (FieldInfo field in obj.GetType().GetFields())
                {
                    ai = new AccessorInfo()
                    {
                        AccessorName = field.Name,
                        AccessorType = field.FieldType,
                        UnderlyingType = PubCmnFunction.GetUnderlyingType(field.FieldType),
                        GetDelegate = CompiledExpressionCreater.CreateGetterOfPropertyOrField(t, field.Name),
                        SetDelegate = CompiledExpressionCreater.CreateSetterOfPropertyOrField(t, field.Name)
                    };
                    aiList.Add(ai);
                }

                AccessorCacher.CncDic.TryAdd(t, aiList);
            }
        }
    }
}
