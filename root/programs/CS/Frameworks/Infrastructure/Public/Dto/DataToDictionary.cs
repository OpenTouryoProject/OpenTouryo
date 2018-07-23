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
//* クラス名        ：DataToDictionary
//* クラス日本語名  ：System.Data型を、Dictionary配列に変換（Dapper, AutoMapper.Data代替）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/07/23  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Data;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>System.Data型を、Dictionary配列に変換</summary>
    public class DataToDictionary
    {
        /// <summary>Dictionary(data field string, dictionary field string)</summary>
        Dictionary<string, string> Mapping = null;

        /// <summary>DateTimeのFormat</summary>
        public string DateTimeFormat = "";

        /// <summary>TimeSpanのFormat</summary>
        public string TimeSpanFormat = "";

        /// <summary>constructor</summary>
        /// <param name="mapping">Dictionary(data field string, dictionary field string)</param>
        /// <param name="dateTimeFormat">DateTimeのFormat</param>
        /// <param name="timeSpanFormat">TimeSpanのFormat</param>
        public DataToDictionary(Dictionary<string, string> mapping, string dateTimeFormat, string timeSpanFormat)
        {
            this.Mapping = mapping;
            this.DateTimeFormat = timeSpanFormat;
            this.TimeSpanFormat = dateTimeFormat;
        }

        #region DataTable

        #region List
        
        /// <summary>DataTableからDictionary配列に変換する。</summary>
        /// <param name="dt">IDataReader</param>
        /// <returns>List(Dictionary(string, string))</returns>
        public List<Dictionary<string, string>> DataTableToDictionaryList(DataTable dt)
        {
            return this.DataReaderToDictionaryList(dt.CreateDataReader());
        }

        #endregion

        #region Dictionary
        
        /// <summary>DataTableからDictionaryに変換する。</summary>
        /// <param name="dt">IDataReader</param>
        /// <param name="map">Dictionary(data field string, dictionary field string)</param>
        /// <returns>Dictionary(string, string)</returns>
        public Dictionary<string, string> DataTableToDictionary(DataTable dt)
        {
            return this.DataReaderToDictionary(dt.CreateDataReader());
        }

        #endregion

        #endregion

        #region DataReader

        #region List
        
        /// <summary>DataReaderからDictionary配列に変換する。</summary>
        /// <param name="dr">IDataReader</param>
        /// <returns>List(Dictionary(string, string))</returns>
        public List<Dictionary<string, string>> DataReaderToDictionaryList(IDataReader dr)
        {
            Dictionary<string, string> obj = null;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            
            do
            {
                obj = this.DataReaderToDictionary(dr);

                // List(Dictionary(string, string))に追加。
                if (obj != null) list.Add(obj);
            }
            while (obj != null);

            // List(Dictionary(string, string))を返す。
            return list;
        }

        #endregion

        #region Dictionary
        
        /// <summary>DataReaderからDictionaryに変換する。</summary>
        /// <param name="dr">IDataReader</param>
        /// <returns>Dictionary(string, string)</returns>
        public Dictionary<string, string> DataReaderToDictionary(IDataReader dr)
        {
            // drのDataTableスキーマ情報 .net coreで動かない。
            //DataTable dt = dr.GetSchemaTable();

            Dictionary<string, string> obj = null;

            // IDataReader の既定の位置は、先頭のレコードの前
            if (dr.Read())
            {
                // Dictionary
                obj = new Dictionary<string, string>();

                // dr.FieldCountで回す。
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string srcPropName = dr.GetName(i);
                    string dstPropName = srcPropName;

                    // マップの有無
                    if (this.Mapping == null)
                    {
                        // マップ無
                    }
                    else
                    {
                        // マップ有
                        if (this.Mapping.ContainsKey(srcPropName))
                        {
                            // 値あり
                            dstPropName = this.Mapping[srcPropName];
                        }
                        else
                        {
                            // 値なし
                        }
                    }

                    try
                    {
                        object o = dr[srcPropName]; // 検証

                        // TimeSpan型の書式指定をする方法と注意点(C#)
                        // https://ict119.com/timespan_format/#DateTimeTimeSpan
                        //   2.1 DateTime型とTimeSpan型の書式指定子の違い

                        if (o.GetType() == typeof(DateTime))
                        {
                            if (string.IsNullOrEmpty(this.DateTimeFormat))
                            {
                                // 精度を保つための仕様
                                obj.Add(dstPropName, ((DateTime)o).Ticks.ToString());
                            }
                            else
                            {
                                // dateTimeFormatでフォーマット
                                obj.Add(dstPropName, ((DateTime)o).ToString(this.DateTimeFormat));
                            }
                        }
                        else if(o.GetType() == typeof(TimeSpan))
                        {
                            if (string.IsNullOrEmpty(this.TimeSpanFormat))
                            {
                                // 精度を保つための仕様
                                obj.Add(dstPropName, ((TimeSpan)o).Ticks.ToString());
                            }
                            else
                            {
                                // dateTimeFormatでフォーマット
                                obj.Add(dstPropName, ((TimeSpan)o).ToString(this.TimeSpanFormat));
                            }
                        }
                        else
                        {
                            // 通常時
                            obj.Add(dstPropName, o.ToString());
                        }   
                    }
                    catch
                    {
                        // ...
                    }
                }
            }

            return obj;
        }

        #endregion

        #endregion
    }
}
