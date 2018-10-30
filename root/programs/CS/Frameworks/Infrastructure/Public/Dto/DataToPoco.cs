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
//* クラス名        ：DataToPoco
//* クラス日本語名  ：System.Data型を、POCO（POCO配列）に変換（Dapper, AutoMapper.Data代替）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/07/19  西野 大介         新規作成
//*  2018/10/03  西野 大介         性能対策
//*  2018/10/23  西野 大介         微調整
//**********************************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Diagnostics;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.FastReflection;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>System.Data型を、POCO（POCO配列）に変換</summary>
    public class DataToPoco
    {
        #region DataTable

        #region List

        /// <summary>DataTableからList(POCO)に変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dt">IDataReader</param>
        /// <returns>List(T)</returns>
        public static List<T> DataTableToList<T>(DataTable dt)
        {
            return DataToPoco.DataReaderToList<T>(dt.CreateDataReader(), null);
        }

        /// <summary>DataTableからList(POCO)に変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dt">IDataReader</param>
        /// <param name="map">Dictionary(poco property or field string, data field string)</param>
        /// <returns>List(T)</returns>
        public static List<T> DataTableToList<T>(DataTable dt, Dictionary<string, string> map)
        {
            return DataToPoco.DataReaderToList<T>(dt.CreateDataReader(), map);
        }

        #endregion

        #region T型のPOCO

        /// <summary>DataTableからPOCOに変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dt">IDataReader</param>
        /// <returns>T型のPOCO</returns>
        public static T DataTableToPOCO<T>(DataTable dt)
        {
            return DataToPoco.DataReaderToPOCO<T>(dt.CreateDataReader(), null);
        }

        /// <summary>DataTableからPOCOに変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dt">IDataReader</param>
        /// <param name="map">Dictionary(poco property or field string, data field string)</param>
        /// <returns>T型のPOCO</returns>
        public static T DataTableToPOCO<T>(DataTable dt, Dictionary<string, string> map)
        {
            return DataToPoco.DataReaderToPOCO<T>(dt.CreateDataReader(), map);
        }

        #endregion

        #endregion

        #region DataReader

        #region List

        /// <summary>DataReaderからList(POCO)に変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dr">IDataReader</param>
        /// <returns>List(T)</returns>
        public static List<T> DataReaderToList<T>(IDataReader dr)
        {
            return DataToPoco.DataReaderToList<T>(dr, null);
        }

        /// <summary>DataReaderからList(POCO)に変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dr">IDataReader</param>
        /// <param name="map">Dictionary(poco property or field string, data field string)</param>
        /// <returns>List(T)</returns>
        public static List<T> DataReaderToList<T>(IDataReader dr, Dictionary<string, string> map)
        {
            // drのDataTableスキーマ情報 .net coreで動かない。
            //DataTable dt = dr.GetSchemaTable();

            // https://stackoverflow.com/questions/373230/check-for-column-name-in-a-sqldatareader-object
            HashSet<string> hs = PubCmnFunction.GetDataReaderColumnInfo(dr);

            // List<POCO>の生成
            List<T> list = new List<T>();

            //PerformanceRecorder pr = new PerformanceRecorder();
            //pr.StartsPerformanceRecord();

            // POCOの型
            T obj = default(T);
            //obj = Activator.CreateInstance<T>();
            obj = InstanceCreator<T>.Factory();
            AccessorCacher.CacheAccessor(obj);

            int counter = 0;

            // IDataReader の既定の位置は、先頭のレコードの前
            while (dr.Read())
            {
                counter++;

                // POCOのnew()
                obj = InstanceCreator<T>.Factory();

                foreach (AccessorInfo ai_dst in AccessorCacher.CncDic[obj.GetType()])
                {
                    string srcName = ai_dst.AccessorName;

                    // マップの有無
                    if (map == null)
                    {
                        // マップ無
                    }
                    else
                    {
                        // マップ有
                        if (map.ContainsKey(srcName))
                        {
                            // 値あり
                            srcName = map[srcName];
                        }
                        else
                        {
                            // 値なし
                        }
                    }

                    if (hs.Contains(srcName))
                    {
                        if (object.Equals(dr[srcName], DBNull.Value))
                        {
                            // DBNullの場合、指定しない。
                        }
                        else
                        {
                            // DBNullで無い場合、指定する（Nullable対応の型変換を追加）。
                            //prop.SetValue(obj, PubCmnFunction.ChangeType(dr[srcPropName], prop.PropertyType), null);
                            //ai.Delegate(obj, PubCmnFunction.ChangeType(dr[srcName], ai.Type));

                            object srcValue = dr[srcName];
                            object dstValue = null;
                            Type srcType = srcValue.GetType();

                            if (ai_dst.UnderlyingType == null)
                            {
                                // Nullableではない
                                if (srcType == ai_dst.AccessorType)
                                {
                                    // 型一致の場合は変換不要
                                    dstValue = srcValue;
                                }
                                else
                                {
                                    dstValue = Convert.ChangeType(srcValue, ai_dst.AccessorType);
                                }
                            }
                            else
                            {
                                // Nullableである
                                if (srcType == ai_dst.AccessorType)
                                {
                                    // 型一致の場合は変換不要
                                    dstValue = srcValue;
                                }
                                else
                                {
                                    // Convert.ChangeType() doesn't handle nullables -> use UnderlyingType.
                                    dstValue = (srcValue == null ? null : Convert.ChangeType(srcValue, ai_dst.UnderlyingType));
                                }
                            }

                            ai_dst.SetDelegate(obj, dstValue);
                        }
                    }
                }

                // List<POCO>に追加。
                if (obj != null) list.Add(obj);
            }

            //Debug.WriteLine(counter + " 回: " + pr.EndsPerformanceRecord());

            // List<POCO>を返す。
            return list;
        }

        #endregion

        #region T型のPOCO

        /// <summary>DataReaderからPOCOに変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dr">IDataReader</param>
        /// <returns>T型のPOCO</returns>
        public static T DataReaderToPOCO<T>(IDataReader dr)
        {
            return DataToPoco.DataReaderToPOCO<T>(dr, null);
        }

        /// <summary>DataReaderからPOCOに変換する。</summary>
        /// <typeparam name="T">POCOの型</typeparam>
        /// <param name="dr">IDataReader</param>
        /// <param name="map">Dictionary(poco property or field string, data field string)</param>
        /// <returns>T型のPOCO</returns>
        public static T DataReaderToPOCO<T>(IDataReader dr, Dictionary<string, string> map)
        {
            // drのDataTableスキーマ情報 .net coreで動かない。
            //DataTable dt = dr.GetSchemaTable();

            // https://stackoverflow.com/questions/373230/check-for-column-name-in-a-sqldatareader-object
            HashSet<string> hs = PubCmnFunction.GetDataReaderColumnInfo(dr);

            //PerformanceRecorder pr = new PerformanceRecorder();
            //pr.StartsPerformanceRecord();

            // POCOの型
            T obj = default(T);
            //obj = Activator.CreateInstance<T>();
            obj = InstanceCreator<T>.Factory();
            AccessorCacher.CacheAccessor(obj);
            
            // IDataReader の既定の位置は、先頭のレコードの前
            if (dr.Read())
            {
                // POCOのnew()

                // InstanceCreator<T>.Factory()の性能測定の名残
                //PerformanceRecorder pr = new PerformanceRecorder();
                //pr.StartsPerformanceRecord();
                //for (int i = 0; i < 1000; i++)
                //{
                obj = InstanceCreator<T>.Factory();
                //}
                //Debug.WriteLine(pr.EndsPerformanceRecord());

                foreach (AccessorInfo ai_dst in AccessorCacher.CncDic[obj.GetType()])
                {
                    string srcName = ai_dst.AccessorName;

                    // マップの有無
                    if (map == null)
                    {
                        // マップ無
                    }
                    else
                    {
                        // マップ有
                        if (map.ContainsKey(srcName))
                        {
                            // 値あり
                            srcName = map[srcName];
                        }
                        else
                        {
                            // 値なし
                        }
                    }

                    if (hs.Contains(srcName))
                    {
                        if (object.Equals(dr[srcName], DBNull.Value))
                        {
                            // DBNullの場合、指定しない。
                        }
                        else
                        {
                            // DBNullで無い場合、指定する（Nullable対応の型変換を追加）。
                            //prop.SetValue(obj, PubCmnFunction.ChangeType(dr[srcPropName], prop.PropertyType), null);
                            //ai.Delegate(obj, PubCmnFunction.ChangeType(dr[srcName], ai.Type));

                            object srcValue = dr[srcName];
                            object dstValue = null;
                            Type srcType = srcValue.GetType();

                            if (ai_dst.UnderlyingType == null)
                            {
                                // Nullableではない
                                if (srcType == ai_dst.AccessorType)
                                {
                                    // 型一致の場合は変換不要
                                    dstValue = srcValue;
                                }
                                else
                                {
                                    dstValue = Convert.ChangeType(srcValue, ai_dst.AccessorType);
                                }
                            }
                            else
                            {
                                // Nullableである
                                if (srcType == ai_dst.AccessorType)
                                {
                                    // 型一致の場合は変換不要
                                    dstValue = srcValue;
                                }
                                else
                                {
                                    // Convert.ChangeTypeの性能測定の名残
                                    //PerformanceRecorder pr = new PerformanceRecorder();
                                    //pr.StartsPerformanceRecord();
                                    //for (int i = 0; i < 1000; i++)
                                    //{
                                    // Convert.ChangeType() doesn't handle nullables -> use UnderlyingType.
                                    dstValue = (srcValue == null ? null : Convert.ChangeType(srcValue, ai_dst.UnderlyingType));
                                    //}
                                    //Debug.WriteLine(pr.EndsPerformanceRecord());
                                }
                            }

                            // AccessorInfo.SetDelegateの性能測定の名残
                            //PerformanceRecorder pr = new PerformanceRecorder();
                            //pr.StartsPerformanceRecord();
                            //for (int i = 0; i < 1000; i++)
                            //{
                            ai_dst.SetDelegate(obj, dstValue);
                            //}
                            //Debug.WriteLine(pr.EndsPerformanceRecord());
                        }
                    }
                }
            }

            //Debug.WriteLine(pr.EndsPerformanceRecord());

            return obj;
        }

        #endregion

        #endregion
    }
}
