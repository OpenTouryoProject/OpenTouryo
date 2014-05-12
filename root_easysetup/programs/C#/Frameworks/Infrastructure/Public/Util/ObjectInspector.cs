//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：ObjectInspector
//* クラス日本語名  ：オブジェクトのプロパティ分析クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/03/16  西野  大介        新規作成
//*  2012/03/23  西野  大介        DateTime、TimeSpanの対応（最小化）
//**********************************************************************************

using System.Reflection;
using System.Collections.Generic;

using System.Diagnostics;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
//using Touryo.Infrastructure.Public.Db;
//using Touryo.Infrastructure.Public.IO;
//using Touryo.Infrastructure.Public.Log;
//using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>オブジェクトのプロパティ分析クラス</summary>
    public class ObjectInspector
    {
        /// <summary>除外する完全修飾名の一部</summary>
        public static string[] ExclusionFullyQualifiedNameParts = new string[] { };

        /// <summary>DateTimeのFormat</summary>
        public static string DateTimeFormat = "";

        /// <summary>オブジェクトのプロパティ分析</summary>
        /// <param name="obj">分析対象オブジェクト</param>
        /// <returns>分析結果</returns>
        public static string Inspect(object obj)
        {
            // nullチェック
            if (obj == null)
            {
                return "null";
            }

            // 戻り値
            int ret = 0;
            StringBuilder sb = new StringBuilder();

            if (obj is object[]) // Object配列の場合
            {
                // 初回判定
                bool isFirstTime = true;

                // 最初と最後に角括弧付与
                sb.Append("[");
                foreach (object temp in (IEnumerable)obj)
                {
                    if (isFirstTime)
                    {
                        isFirstTime = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    // GetStringInfo
                    sb.Append(GetObjectInfo(temp, ref ret));
                }
                sb.Append("]");
            }
            else
            {
                // GetStringInfo
                sb.Append(GetObjectInfo(obj, ref ret));
            }

            // 戻り値
            return sb.ToString();
        }

        /// <summary>インスペクト処理メソッド</summary>
        /// <param name="obj">オブジェクト</param>
        /// <param name="norc">再帰呼出数</param>
        /// <returns>文字列化したプロパティの情報</returns>
        private static string GetObjectInfo(object obj, ref int norc)
        {
            // nullチェック
            if (obj == null)
            {
                return "null";
            }
            else
            {
                // number of recursive calls
                if (norc < 5)
                {
                    norc++;
                }
                else
                {
                    //return "-number of recursive calls is over-";
                    return "-over-";
                }
            }

            // 戻り値
            string ret = "";
            StringBuilder sb = new StringBuilder();

            // 型チェック
            Type type = obj.GetType();
            //Debug.WriteLine(type.StructLayoutAttribute.Value.ToString());

            if (type.IsPrimitive)
            {
                // Primitive

                // リターン
                ret = obj.ToString();
            }
            else if (type == typeof(string) || type == typeof(StringBuilder))
            {
                // String

                // ダブルクォーテ付与
                sb.Append("\"");
                sb.Append(obj.ToString());
                sb.Append("\"");

                // リターン
                ret = sb.ToString();
            }
            else if (type == typeof(DateTime))
            {
                // DateTime

                // リターン
                if (string.IsNullOrEmpty(ObjectInspector.DateTimeFormat))
                {
                    // フォーマット指定なし
                    ret = ((DateTime)obj).ToString();
                }
                else
                {
                    // フォーマット指定なし
                    ret = ((DateTime)obj).ToString(ObjectInspector.DateTimeFormat);
                }
            }
            else if (type == typeof(TimeSpan))
            {
                // TimeSpan

                // リターン
                ret = obj.ToString();
            }
            else if (type.IsEnum)
            {
                // Enum

                // ダブルクォーテ付与
                sb.Append(obj.ToString());

                // リターン
                ret = sb.ToString();
            }
            else if (type.IsArray)
            {
                // Array
                Array ary = (Array)obj;
                // 初回判定
                bool isFirstTime = true;
                StringBuilder sb2 = new StringBuilder();

                // 最初と最後に角括弧付与
                sb2.Append("[");
                foreach (object temp in ary)
                {
                    if (isFirstTime)
                    {
                        isFirstTime = false;
                    }
                    else
                    {
                        sb2.Append(", ");
                    }

                    // 再帰呼び出し
                    sb2.Append(GetObjectInfo(temp, ref norc));
                }
                sb2.Append("]");

                if (isFirstTime)
                {
                    sb.Append("-empty-");
                }
                else
                {
                    sb.Append(sb2.ToString());
                }

                // リターン
                ret = sb.ToString();
            }
            else if (obj is IDictionary) // Hashtableも処理可能。
            {
                // IDictionary
                IDictionary idc = (IDictionary)obj;
                // 初回判定
                bool isFirstTime = true;
                StringBuilder sb2 = new StringBuilder();

                // 最初と最後に角括弧付与
                sb2.Append("[");
                foreach (object key in idc.Keys)
                {
                    if (isFirstTime)
                    {
                        isFirstTime = false;
                    }
                    else
                    {
                        sb2.Append(", ");
                    }

                    // 再帰呼び出し
                    sb2.Append(key + " = " + GetObjectInfo(idc[key], ref norc));
                }
                sb2.Append("]");

                if (isFirstTime)
                {
                    sb.Append("-empty-");
                }
                else
                {
                    sb.Append(sb2.ToString());
                }

                // リターン
                ret = sb.ToString();
            }
            else if (obj is IEnumerable)
            {
                // IEnumerable
                IEnumerable ien = (IEnumerable)obj;
                // 初回判定
                bool isFirstTime = true;
                StringBuilder sb2 = new StringBuilder();

                // 最初と最後に角括弧付与
                sb2.Append("[");
                foreach (object temp in ien)
                {
                    if (isFirstTime)
                    {
                        isFirstTime = false;
                    }
                    else
                    {
                        sb2.Append(", ");
                    }

                    // 再帰呼び出し
                    sb2.Append(GetObjectInfo(temp, ref norc));
                }
                sb2.Append("]");

                if (isFirstTime)
                {
                    sb.Append("-empty-");
                }
                else
                {
                    sb.Append(sb2.ToString());
                }

                // リターン
                ret = sb.ToString();
            }
            else if (type.StructLayoutAttribute != null)
                // type.IsClass) // IsClassだとstructが含まれなくなる。
            {
                // 除外処理
                bool isExclusion = false;
                foreach (string s in ObjectInspector.ExclusionFullyQualifiedNameParts)
                {
                    if (type.FullName.IndexOf(s) != -1)
                    {
                        isExclusion = true;
                        break;
                    }
                }

                if (!isExclusion)
                {
                    //  classか？structか？
                    PropertyInfo[] props = type.GetProperties();
                    FieldInfo[] fields = type.GetFields();

                    // 初回判定
                    bool isFirstTime1 = true;
                    StringBuilder sb2 = new StringBuilder();

                    string name = "";
                    object temp = null;
                    MethodInfo method;
                    ParameterInfo[] _params;

                    sb2.Append("[");

                    // Property
                    foreach (PropertyInfo prop in props)
                    {
                        if (isFirstTime1)
                        {
                            isFirstTime1 = false;
                        }
                        else
                        {
                            sb2.Append(", ");
                        }

                        name = prop.Name;
                        method = prop.GetGetMethod();
                        _params = prop.GetGetMethod().GetParameters();

                        // インデクサ？
                        if (_params != null)
                        {
                            if (_params.Length == 0)
                            {
                                // プロパティ
                                try
                                {
                                    // 値を取得
                                    temp = method.Invoke(obj, null);

                                    // 再帰呼び出し
                                    sb2.Append(name + " = " + GetObjectInfo(temp, ref norc));
                                }
                                catch (TargetInvocationException taEx)
                                {
                                    Exception ex = taEx; // ← 警告を出さないため
                                    sb2.Append(name + " = Error. can't process");
                                }
                            }
                            else if (_params.Length == 1)
                            {
                                // インデクサ
                                if (_params[0].ParameterType.FullName == "System.Int32")
                                {
                                    // index
                                    int i = 0;

                                    // 初回判定
                                    bool isFirstTime2 = true;
                                    StringBuilder sb3 = new StringBuilder();

                                    try
                                    {
                                        sb3.Append("item[");
                                        for (i = 0; ; i++)
                                        {
                                            if (!isFirstTime2)
                                            {
                                                sb3.Append(", ");
                                            }

                                            // 値を取得
                                            temp = method.Invoke(obj, new object[] { i });

                                            if (isFirstTime2)
                                            {
                                                isFirstTime2 = false;
                                            }
                                            
                                            // 再帰呼び出し
                                            sb3.Append(i.ToString() + " = " + GetObjectInfo(temp, ref norc));
                                        }
                                    }
                                    catch (TargetInvocationException taEx)
                                    {
                                        if (taEx.InnerException is IndexOutOfRangeException)
                                        {
                                            // 抜けた場合
                                            sb3.Append("]");
                                        }
                                        else
                                        {
                                            // 例外の場合
                                            sb3.Append(i.ToString() + " = Error. can't process");
                                            sb3.Append("]");
                                        }

                                        if (isFirstTime2)
                                        {
                                            sb2.Append("item[-empty-]");
                                        }
                                        else
                                        {
                                            sb2.Append(sb3.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    // int以外のインデクサ
                                    sb2.Append("-else indexer (not int)-");
                                }
                            }
                            else
                            {
                                // 引数が２以上のインデクサ
                                sb2.Append("-else indexer (num of param isn't one)-");
                            }
                        }
                        else
                        {
                            // ここはあり得ないと思われる。
                        }
                    }

                    // Field
                    foreach (FieldInfo field in fields)
                    {
                        if (isFirstTime1)
                        {
                            isFirstTime1 = false;
                        }
                        else
                        {
                            sb2.Append(", ");
                        }

                        name = field.Name;
                        temp = field.GetValue(obj);

                        // 再帰呼び出し
                        sb2.Append(name + " = " + GetObjectInfo(temp, ref norc));
                    }

                    sb2.Append("]");

                    if (isFirstTime1)
                    {
                        sb.Append("-empty-");
                    }
                    else
                    {
                        sb.Append(sb2.ToString());
                    }

                    // リターン 
                    ret = sb.ToString();
                }
                else
                {
                    ret = "excluded. " + obj.ToString(); 
                }
            }
            else
            {
                ret = "can't process. " + obj.ToString();
            }

            norc--;
            return ret;
        }
    }
}
