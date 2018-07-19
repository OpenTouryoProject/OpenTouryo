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
//* クラス日本語名  ：System.Data型を、POCO（POCO配列）に変換（Dapper, AutoMapper代替）
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
using System.Reflection;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Public.Db
{
    /// <summary>System.Data型を、POCO（POCO配列）に変換</summary>
    public class DataToPoco
    {
        #region DataTable

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
        /// <param name="map">Dictionary(poco property string, data property string)</param>
        /// <returns>List(T)</returns>
        public static List<T> DataTableToList<T>(DataTable dt, Dictionary<string, string> map)
        {
            return DataToPoco.DataReaderToList<T>(dt.CreateDataReader(), map);
        }

        #endregion

        #region DataReader

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
        /// <param name="map">Dictionary(poco property string, data property string)</param>
        /// <returns>List(T)</returns>
        public static List<T> DataReaderToList<T>(IDataReader dr, Dictionary<string, string> map)
        {
            // List<POCO>の生成
            List<T> list = new List<T>();

            // POCOの型
            T obj = default(T);

            while (dr.Read())
            {
                // POCOのnew()
                obj = Activator.CreateInstance<T>();

                // プロパティで回す。
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    string src = "";

                    // マップの有無
                    if (map == null)
                    {
                        // マップ無
                        src = prop.Name;
                    }
                    else
                    {
                        // マップ有
                        src = map[prop.Name];
                    }

                    if (object.Equals(dr[src], DBNull.Value))
                    {
                        // DBNullの場合、指定しない。
                    }
                    else
                    {
                        // DBNullで無い場合、指定する。
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }

                // List<POCO>に追加。
                list.Add(obj);
            }

            // List<POCO>を返す。
            return list;
        }

        #endregion
    }
}
