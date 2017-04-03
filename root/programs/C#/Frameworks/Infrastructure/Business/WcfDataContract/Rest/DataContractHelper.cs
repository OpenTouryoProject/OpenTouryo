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
//* クラス名        ：DataContractHelper
//* クラス日本語名  ：WCF Webサービス（サービス インターフェイス基盤）
//*                   REST（XML、JSON）汎用Webメソッド用のList相互変換クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/08/13  西野 大介         新規作成
//**********************************************************************************

using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest
{
    /// <summary>DataTable を List 型に変換するクラス</summary>
    public class DataContractHelper
    {
        /// <summary>DataTable を List に変換する</summary>
        /// <param name="dt">変換元の DataTable</param>
        /// <returns>変換先の List</returns>
        /// <remarks>列のデータ型はすべて文字列型となる</remarks>
        public static List<Dictionary<string, string>> ToList(DataTable dt)
        {
            // テーブルデータを格納する List
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

            // DataTable を List に変換する
            IEnumerable<DataColumn> columns = dt.Columns.Cast<DataColumn>();
            table = (List<Dictionary<string, string>>)dt.AsEnumerable().Select(
                dataRow => columns.Select(
                    column => new { Column = column.ColumnName, Value = dataRow[column].ToString() }).ToDictionary(
                    data => data.Column, data => data.Value)).ToList();

            // テーブルデータを返す
            return table;
        }

        /// <summary>List を DataTable に変換する</summary>
        /// <param name="dt">変換元の List</param>
        /// <returns>変換先の DataTable</returns>
        /// <remarks>列のデータ型はすべて文字列型とする</remarks>
        public static DataTable ToDataTable(List<Dictionary<string, string>> list)
        {
            // テーブルデータを格納する DataTable
            DataTable dt = new DataTable();

            // 1 件分のデータ
            DataRow dr = null;

            // 列を変換する
            dt.Columns.AddRange(list.First().Select(
                column => new DataColumn() { ColumnName = column.Key, DataType = typeof(string) })
                .AsEnumerable().ToArray());

            // データを変換する
            foreach (Dictionary<string, string> dic in list)
            {
                // 行データを作成する
                dr = dt.NewRow();

                foreach (string key in dic.Keys)
                {
                    dr[key] = dic[key];
                }

                // テーブルに行データを追加する
                dt.Rows.Add(dr);
            }

            // テーブルデータを返す
            return dt;
        }
    }
}
