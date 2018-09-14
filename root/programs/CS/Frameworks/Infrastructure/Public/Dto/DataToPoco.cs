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
//**********************************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

using Touryo.Infrastructure.Public.Util;

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
            // List<POCO>の生成
            List<T> list = new List<T>();

            // POCOの型
            T obj = default(T);

            do
            {
                obj = DataToPoco.DataReaderToPOCO<T>(dr, map);

                // List<POCO>に追加。
                if(obj != null) list.Add(obj);
            }
            while (obj != null);
            
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

            // POCOの型
            T obj = default(T);

            // IDataReader の既定の位置は、先頭のレコードの前
            if (dr.Read())
            {
                // POCOのnew()
                obj = Activator.CreateInstance<T>();

                // Propertiesで回す。
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    string srcPropName = prop.Name;

                    // マップの有無
                    if (map == null)
                    {
                        // マップ無
                    }
                    else
                    {
                        // マップ有
                        if (map.ContainsKey(srcPropName))
                        {
                            // 値あり
                            srcPropName = map[srcPropName];
                        }
                        else
                        {
                            // 値なし
                        }
                    }

                    try
                    {
                        object o = dr[srcPropName]; // 検証

                        if (object.Equals(dr[srcPropName], DBNull.Value))
                        {
                            // DBNullの場合、指定しない。
                        }
                        else
                        {
                            // DBNullで無い場合、指定する（Nullable対応の型変換を追加）。
                            prop.SetValue(obj, PubCmnFunction.ChangeType(dr[srcPropName], prop.PropertyType), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }
                }

                // Fieldsで回す。
                foreach (FieldInfo field in obj.GetType().GetFields())
                {
                    string srcFieldName = field.Name;

                    // マップの有無
                    if (map == null)
                    {
                        // マップ無
                    }
                    else
                    {
                        // マップ有
                        if (map.ContainsKey(srcFieldName))
                        {
                            // 値あり
                            srcFieldName = map[srcFieldName];
                        }
                        else
                        {
                            // 値なし
                        }
                    }

                    try
                    {
                        object o = dr[srcFieldName]; // 検証

                        if (object.Equals(dr[srcFieldName], DBNull.Value))
                        {
                            // DBNullの場合、指定しない。
                        }
                        else
                        {
                            // DBNullで無い場合、指定する（Nullable対応の型変換を追加）。
                            field.SetValue(obj, PubCmnFunction.ChangeType(dr[srcFieldName], field.FieldType));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }
                }
            }

            return obj;
        }

        #endregion

        #endregion
    }
}
