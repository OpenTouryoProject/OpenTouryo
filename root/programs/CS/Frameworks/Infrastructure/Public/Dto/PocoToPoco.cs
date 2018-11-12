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
//* クラス名        ：PocoToPoco
//* クラス日本語名  ：POCO型を、POCO型にマップ（AutoMapper代替）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/07/20  西野 大介         新規作成
//*  2018/10/03  西野 大介         性能対策
//*  2018/10/23  西野 大介         微調整
//**********************************************************************************

using System.Collections.Generic;

using Touryo.Infrastructure.Public.FastReflection;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>POCO型を、POCO型にマップ</summary>
    public class PocoToPoco
    {
        /// <summary>POCO型を、POCO型にマップ</summary>
        /// <typeparam name="TSource">srcのPOCO型</typeparam>
        /// <typeparam name="TDestination">dstのPOCO型</typeparam>
        /// <param name="src">srcのPOCOインスタンス</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src)
        {
            return PocoToPoco.Map<TSource, TDestination>(src, InstanceCreator<TDestination>.Factory(), null);
        }

        /// <summary>POCO型を、POCO型にマップ</summary>
        /// <typeparam name="TSource">srcのPOCO型</typeparam>
        /// <typeparam name="TDestination">dstのPOCO型</typeparam>
        /// <param name="src">srcのPOCOインスタンス</param>
        /// <param name="dst">dstのPOCOインスタンス(null可)</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src, TDestination dst)
        {
            return PocoToPoco.Map<TSource, TDestination>(src, dst, null);
        }

        /// <summary>POCO型を、POCO型にマップ</summary>
        /// <typeparam name="TSource">srcのPOCO型</typeparam>
        /// <typeparam name="TDestination">dstのPOCO型</typeparam>
        /// <param name="src">srcのPOCOインスタンス</param>
        /// <param name="map">Dictionary(dst property or field string, src property or field string)</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src, Dictionary<string, string> map)
        {
            return PocoToPoco.Map<TSource, TDestination>(src, InstanceCreator<TDestination>.Factory(), map);
        }

        /// <summary>POCO型を、POCO型にマップ</summary>
        /// <typeparam name="TSource">srcのPOCO型</typeparam>
        /// <typeparam name="TDestination">dstのPOCO型</typeparam>
        /// <param name="src">srcのPOCOインスタンス</param>
        /// <param name="dst">dstのPOCOインスタンス(null可)</param>
        /// <param name="map">Dictionary(dst property or field string, src property or field string)</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src, TDestination dst, Dictionary<string, string> map)
        {
            // POCOのnew()
            if (dst == null)
            {
                dst = InstanceCreator<TDestination>.Factory();
            }

            AccessorCacher.CacheAccessor(dst); // dstの型
            AccessorCacher.CacheAccessor(src); // srcの型

            // dst
            foreach (AccessorInfo dst_ai in AccessorCacher.CncDic[dst.GetType()])
            {
                string dstName = dst_ai.AccessorName;
                string srcName = "";

                // マップの有無
                if (map == null)
                {
                    // マップ無
                }
                else
                {
                    // マップ有

                    if (map.ContainsKey(dstName))
                    {
                        // 値あり
                        srcName = map[dstName];
                    }
                    else
                    {
                        // 値なし
                        srcName = dstName;
                    }
                }

                // src
                foreach (AccessorInfo src_ai in AccessorCacher.CncDic[src.GetType()])
                {
                    if (srcName == src_ai.AccessorName)
                    {
                        dst_ai.SetDelegate(dst, src_ai.GetDelegate(src));
                    }
                }
            }

            return dst;
        }
    }
}
