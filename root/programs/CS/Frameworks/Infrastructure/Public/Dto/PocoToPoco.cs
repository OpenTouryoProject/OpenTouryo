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
//*  2018/12/11  西野 大介         バグ修正（マップ無し時の動作
//*  2026/10/17  玄人 幸道         公開の set を持たない移送先・公開の get を持たない移送元の
//*                                プロパティで、NullReferenceException になるのを修正（#618）
//*  2026/10/18  玄人 幸道         同名で型の違う項目を、DataToPoco と同じく変換して移送するようにした（#619）
//*  2026/10/18  玄人 幸道         移送先が構造体のとき、値が入らないのを修正（#620）
//*  2026/10/18  玄人 幸道         移送元が null なら ArgumentNullException にした（#621）
//**********************************************************************************

using System;
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
        /// <param name="src">srcのPOCOインスタンス(null不可。nullならArgumentNullException)</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src)
        {
            return PocoToPoco.Map<TSource, TDestination>(src, InstanceCreator<TDestination>.Factory(), null);
        }

        /// <summary>POCO型を、POCO型にマップ</summary>
        /// <typeparam name="TSource">srcのPOCO型</typeparam>
        /// <typeparam name="TDestination">dstのPOCO型</typeparam>
        /// <param name="src">srcのPOCOインスタンス(null不可。nullならArgumentNullException)</param>
        /// <param name="dst">dstのPOCOインスタンス(null可)</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src, TDestination dst)
        {
            return PocoToPoco.Map<TSource, TDestination>(src, dst, null);
        }

        /// <summary>POCO型を、POCO型にマップ</summary>
        /// <typeparam name="TSource">srcのPOCO型</typeparam>
        /// <typeparam name="TDestination">dstのPOCO型</typeparam>
        /// <param name="src">srcのPOCOインスタンス(null不可。nullならArgumentNullException)</param>
        /// <param name="map">Dictionary(dst property or field string, src property or field string)</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src, Dictionary<string, string> map)
        {
            return PocoToPoco.Map<TSource, TDestination>(src, InstanceCreator<TDestination>.Factory(), map);
        }

        /// <summary>POCO型を、POCO型にマップ</summary>
        /// <typeparam name="TSource">srcのPOCO型</typeparam>
        /// <typeparam name="TDestination">dstのPOCO型</typeparam>
        /// <param name="src">srcのPOCOインスタンス(null不可。nullならArgumentNullException)</param>
        /// <param name="dst">dstのPOCOインスタンス(null可)</param>
        /// <param name="map">Dictionary(dst property or field string, src property or field string)</param>
        /// <returns>TDestination型のPOCO</returns>
        public static TDestination Map<TSource, TDestination>(TSource src, TDestination dst, Dictionary<string, string> map)
        {
            // 移送元は必須（#621）。他のオーバーロードもここに集まる。
            // 2026/10/18 まで、src.GetType() で NullReferenceException になっていた。
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            // POCOのnew()
            if (dst == null)
            {
                dst = InstanceCreator<TDestination>.Factory();
            }

            AccessorCacher.CacheAccessor(dst); // dstの型
            AccessorCacher.CacheAccessor(src); // srcの型

            // **値を入れる先は、1 度だけボックス化したもの**（#620）。
            // SetDelegate は object を受けるので、構造体の dst をそのまま渡すと、
            // 呼ぶたびにボックス化したコピーに値が入り、dst は変わらない。
            // 参照型なら、boxedDst は dst と同じインスタンス。
            object boxedDst = dst;

            // dst
            foreach (AccessorInfo dst_ai in AccessorCacher.CncDic[dst.GetType()])
            {
                if (dst_ai.SetDelegate == null)
                {
                    // 公開の set を持たないプロパティ（private set を含む）は、移送の対象から外す（#618）。
                    // AccessorCacher は、公開の set が無いと SetDelegate を null にする。
                    continue;
                }

                string dstName = dst_ai.AccessorName;
                string srcName = "";

                // マップの有無
                if (map == null)
                {
                    // マップ無
                    srcName = dstName;
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
                    // 公開の get を持たない移送元のプロパティは、値を取れないので飛ばす（#618）。
                    if (srcName == src_ai.AccessorName && src_ai.GetDelegate != null)
                    {
                        object dstValue = null;
                        if (PocoToPoco.TryConvert(src_ai.GetDelegate(src), dst_ai, out dstValue))
                        {
                            dst_ai.SetDelegate(boxedDst, dstValue);
                        }
                    }
                }
            }

            // ボックス化したものから取り出す（構造体の場合。#620）
            return (TDestination)boxedDst;
        }

        /// <summary>移送元の値を、移送先の型に合わせる（#619）</summary>
        /// <param name="srcValue">移送元の値</param>
        /// <param name="dst_ai">移送先の AccessorInfo</param>
        /// <param name="dstValue">移送先に設定する値</param>
        /// <returns>設定するなら true。設定しない（既定値のまま）なら false</returns>
        /// <remarks>
        /// 2026/10/18 まで変換をせず、同名で型が違うと（int → long でも）InvalidCastException になっていた。
        /// DataToPoco は Convert.ChangeType で変換するので、それに合わせる。
        /// ただし、キャストで済む組み合わせ（派生型 → 基底型・インタフェイス、T → Nullable(T)、
        /// 整数 ⇔ 列挙型）は、以前と同じくそのまま設定する（変換すると、かえって通らなくなる）。
        /// </remarks>
        private static bool TryConvert(object srcValue, AccessorInfo dst_ai, out object dstValue)
        {
            dstValue = srcValue;

            // 移送先の型（Nullable なら中身の型）
            Type dstType = dst_ai.UnderlyingType ?? dst_ai.AccessorType;

            if (srcValue == null)
            {
                // null は、Nullable でない値型には入らない。既定値のまま（DataToPoco の DBNull と同じ）。
                return !(dst_ai.AccessorType.IsValueType && dst_ai.UnderlyingType == null);
            }

            Type srcType = srcValue.GetType();

            if (dst_ai.AccessorType.IsAssignableFrom(srcType))
            {
                // そのまま入る（型一致・派生型 → 基底型・T → Nullable(T)）
            }
            else if (dstType.IsEnum && PocoToPoco.IsIntegral(srcType))
            {
                // 整数・列挙型 → 列挙型（Convert.ChangeType は列挙型へ変換できない）
                dstValue = Enum.ToObject(dstType, srcValue);
            }
            else
            {
                // Convert.ChangeType() doesn't handle nullables -> use UnderlyingType.
                dstValue = Convert.ChangeType(srcValue, dstType);
            }

            return true;
        }

        /// <summary>整数型（列挙型は中身の型で見る）か</summary>
        /// <param name="type">Type</param>
        /// <returns>整数型なら true</returns>
        private static bool IsIntegral(Type type)
        {
            // TypeCode は SByte ～ UInt64 が整数型。列挙型は中身の型の TypeCode を返す。
            TypeCode tc = Type.GetTypeCode(type);
            return TypeCode.SByte <= tc && tc <= TypeCode.UInt64;
        }
    }
}
