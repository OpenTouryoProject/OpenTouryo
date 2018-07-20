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
//**********************************************************************************

using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

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
            return PocoToPoco.Map<TSource, TDestination>(src, Activator.CreateInstance<TDestination>(), null);
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
            return PocoToPoco.Map<TSource, TDestination>(src, Activator.CreateInstance<TDestination>(), map);
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
                dst = Activator.CreateInstance<TDestination>();
            }

            // Propertiesで回す。
            foreach (PropertyInfo dstProp in dst.GetType().GetProperties())
            {
                string dstPropName = dstProp.Name;

                // マップの有無
                if (map == null)
                {
                    // マップ無
                }
                else
                {
                    // マップ有

                    if (map.ContainsKey(dstPropName))
                    {
                        // 値あり
                        dstPropName = map[dstPropName];
                    }
                    else
                    {
                        // 値なし
                    }
                }
                // Propertyから
                foreach (PropertyInfo srcProp in src.GetType().GetProperties())
                {
                    if (dstPropName == srcProp.Name)
                    {
                        dstProp.SetValue(dst, srcProp.GetValue(src, null));
                    }
                }
                // Fieldから
                foreach (FieldInfo srcField in src.GetType().GetFields())
                {
                    if (dstPropName == srcField.Name)
                    {
                        dstProp.SetValue(dst, srcField.GetValue(src));
                    }
                }
            }

            // Fieldsで回す。
            foreach (FieldInfo dstField in dst.GetType().GetFields())
            {
                string dstFieldName = dstField.Name;

                // マップの有無
                if (map == null)
                {
                    // マップ無
                }
                else
                {
                    // マップ有

                    if (map.ContainsKey(dstFieldName))
                    {
                        // 値あり
                        dstFieldName = map[dstFieldName];
                    }
                    else
                    {
                        // 値なし
                    }
                }
                // Propertyから
                foreach (PropertyInfo srcProp in src.GetType().GetProperties())
                {
                    if (dstFieldName == srcProp.Name)
                    {
                        dstField.SetValue(dst, srcProp.GetValue(src, null));
                    }
                }
                // Fieldから
                foreach (FieldInfo srcField in src.GetType().GetFields())
                {
                    if (dstFieldName == srcField.Name)
                    {
                        dstField.SetValue(dst, srcField.GetValue(src));
                    }
                }
            }

            return dst;
        }
    }
}
