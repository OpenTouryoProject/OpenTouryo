//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：CustomMarshaler
//* クラス日本語名  ：カスタム マーシャリング クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/11/19  西野  大介        新規作成
//*  2015/01/15	 Supragyan         Added  StringFromPrimitivetype method to convert 
//*                                premitive data to string.
//*  2015/01/15	 Supragyan         Added  PrimitivetypeFromString method to convert 
//*                                string to premitive data.
//**********************************************************************************

// Config
using System.Configuration;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

using System.Runtime.InteropServices;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Dto;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>カスタム マーシャリング機能を提供する。</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class CustomMarshaler
    {
        #region BytesToStructure(s)

        /// <summary>
        /// 「Unmanage構造体」のバイト表現を
        /// 「Manage構造体」へマーシャリング
        /// </summary>
        /// <param name="bytes">「Unmanage構造体」のバイト表現</param>
        /// <param name="cStructureType">「Manage構造体」の型</param>
        /// <returns>「Manage構造体」</returns>
        public static object BytesToStructure(byte[] bytes, Type cStructureType)
        {
            // ワーク変数
            IntPtr pCst = IntPtr.Zero;
            int sizeCst = Marshal.SizeOf(cStructureType);

            // 戻り値
            object o = null;

            try
            {
                // メモリ（ポインタ）を確保
                pCst = Marshal.AllocHGlobal(sizeCst);

                // 「Unmanage構造体」をメモリ（ポインタ）へコピー
                Marshal.Copy(bytes, 0, pCst, sizeCst);

                // メモリ（ポインタ）から「Manage構造体」を生成
                o = Marshal.PtrToStructure(pCst, cStructureType);
            }
            finally // メモリ（ポインタ）は必ず開放するように。
            {
                // 確保したメモリ（ポインタ）を解放する。
                Marshal.FreeHGlobal(pCst);
            }

            return o;
        }

        /// <summary>
        /// 「Unmanage構造体」配列のバイト表現を
        /// 「Manage構造体」配列へマーシャリング
        /// </summary>
        /// <param name="bytes">「Unmanage構造体」配列のバイト表現</param>
        /// <param name="cStructureType">「Manage構造体」の型</param>
        /// <param name="count">「Manage構造体」の要素数</param>
        /// <returns>「Manage構造体」配列</returns>
        public static object[] BytesToStructures(byte[] bytes, Type cStructureType, int count)
        {
            // ワーク変数
            IntPtr pCst = IntPtr.Zero;
            int sizeCst = Marshal.SizeOf(cStructureType);

            // 戻り値
            object[] o = new object[count];

            try
            {
                // メモリ（ポインタ）を確保
                pCst = Marshal.AllocHGlobal(sizeCst);

                for (int i = 0; i < count; i++)
                {
                    // 「Unmanage構造体」をメモリ（ポインタ）へコピー
                    Marshal.Copy(bytes, sizeCst * i, pCst, sizeCst);

                    // メモリ（ポインタ）から「Manage構造体」を生成
                    o[i] = Marshal.PtrToStructure(pCst, cStructureType);
                }
            }
            finally // メモリ（ポインタ）は必ず開放するように。
            {
                // 確保したメモリ（ポインタ）を解放する。
                Marshal.FreeHGlobal(pCst);
            }

            return o;
        }

        #endregion

        #region Structure(s)ToBytes

        /// <summary>
        /// 「Manage構造体」を「Unmanage構造体」
        /// のバイト表現へマーシャリング
        /// </summary>
        /// <param name="cStructure">「Manage構造体」</param>
        /// <returns>「Unmanage構造体」のバイト表現</returns>
        public static byte[] StructureToBytes(object cStructure)
        {
            // ワーク変数
            IntPtr pCst = IntPtr.Zero;
            int sizeCst = Marshal.SizeOf(cStructure);

            // 戻り値
            byte[] cstBytes = null;

            try
            {
                // メモリ（ポインタ）を確保
                pCst = Marshal.AllocHGlobal(sizeCst);

                // 「Unmanage構造体」をメモリ（ポインタ）へコピー
                Marshal.StructureToPtr(cStructure, pCst, false);

                // メモリ（ポインタ）からバイト配列を取得
                cstBytes = new byte[sizeCst];
                Marshal.Copy(pCst, cstBytes, 0, sizeCst);
            }
            finally // メモリ（ポインタ）は必ず開放するように。
            {
                // 確保したメモリ（ポインタ）を解放する。
                Marshal.FreeHGlobal(pCst);
            }

            return cstBytes;
        }


        /// <summary>
        /// 「Manage構造体」配列を
        /// 「Unmanage構造体」配列の
        /// バイト表現へマーシャリング
        /// </summary>
        /// <param name="cStructures">「Manage構造体」配列</param>
        /// <param name="count">「Manage構造体」の要素数</param>
        /// <returns>「Unmanage構造体」配列のバイト表現</returns>
        public static byte[] StructuresToBytes(object[] cStructures, int count)
        {
            // ワーク変数
            IntPtr pCst = IntPtr.Zero;
            int sizeCst = Marshal.SizeOf(cStructures[0]);

            // 戻り値
            byte[] cstBytes = null;
            byte[] cstsBytes = new byte[sizeCst * count];

            try
            {
                // メモリ（ポインタ）を確保
                pCst = Marshal.AllocHGlobal(sizeCst);

                for (int i = 0; i < count; i++)
                {
                    // 「Unmanage構造体」をメモリ（ポインタ）へコピー
                    Marshal.StructureToPtr(cStructures[i], pCst, false);

                    // メモリ（ポインタ）からバイト配列を取得
                    cstBytes = new byte[sizeCst];
                    Marshal.Copy(pCst, cstBytes, 0, sizeCst);

                    // バイト配列にマージ
                    Array.Copy(cstBytes, 0, cstsBytes, sizeCst * i, sizeCst);
                }
            }
            finally // メモリ（ポインタ）は必ず開放するように。
            {
                // 確保したメモリ（ポインタ）を解放する。
                Marshal.FreeHGlobal(pCst);
            }

            return cstsBytes;
        }

        #endregion

        #region StringFromPrimitivetype

        /// <summary>
        /// To converts all premitive data to string data.
        /// </summary>
        /// <param name="primitiveType">Column data type</param>
        /// <param name="checkType">Check sting Type data</param>
        /// <returns>returns String data</returns>
        public static string StringFromPrimitivetype(object primitiveType, bool checkType)
        {
            string convertedString = null;

            if (primitiveType != null)
            {
                if (DTColumn.CheckType(primitiveType, DTType.DateTime))
                {
                    DateTime dateTime = (DateTime)primitiveType;
                    convertedString += dateTime.Year + "/";
                    convertedString += dateTime.Month + "/";
                    convertedString += dateTime.Day + "-";

                    convertedString += dateTime.Hour + ":";
                    convertedString += dateTime.Minute + ":";
                    convertedString += dateTime.Second + ".";
                    convertedString += dateTime.Millisecond;
                }
                else if (DTColumn.CheckType(primitiveType, DTType.ByteArray))
                {
                    convertedString = Convert.ToBase64String((byte[])primitiveType);
                }
                else if (DTColumn.CheckType(primitiveType, DTType.String))
                {
                    if (checkType == true)
                    {
                        convertedString = primitiveType.ToString().Replace("\r", "\rrnr:");
                        convertedString = primitiveType.ToString().Replace("\n", "\rrnn:");

                        convertedString = primitiveType.ToString().Replace("\r", "\r\n");
                    }
                    else
                    {
                        convertedString = primitiveType.ToString();
                    }
                }
                else
                {
                    convertedString = primitiveType.ToString();
                }

            }
            return convertedString;
        }

        #endregion

        #region PrimitivetypeFromString

        /// <summary>
        /// To converts all string data to premitive Type by colType and cellString.
        /// </summary>
        /// <param name="colType">Column data type</param>
        /// <param name="cellString">Cell data</param>
        /// <returns>return Premitive type object</returns>
        public static object PrimitivetypeFromString(DTType colType, string cellString)
        {
            object convertedPrimitiveType = null;
            if (cellString != null)
            {
                // ByteArray
                if (colType == DTType.ByteArray)
                {
                    byte[] cellByte = Convert.FromBase64String(cellString);
                    convertedPrimitiveType = cellByte;
                }
                // DateTime
                else if (colType == DTType.DateTime)
                {
                    string ymd = cellString.Split('-')[0];
                    string hmsf = cellString.Split('-')[1];

                    DateTime cellDttm = new DateTime(
                        int.Parse(ymd.Split('/')[0]),
                        int.Parse(ymd.Split('/')[1]),
                        int.Parse(ymd.Split('/')[2]),
                        int.Parse(hmsf.Split(':')[0]),
                        int.Parse(hmsf.Split(':')[1]),
                        int.Parse(hmsf.Split(':')[2].Split('.')[0]),
                        int.Parse(hmsf.Split(':')[2].Split('.')[1]));

                    convertedPrimitiveType = cellDttm;
                }
                else
                {
                    convertedPrimitiveType = DTColumn.AutoCast(colType, cellString.ToString());
                }
            }

            return convertedPrimitiveType;
        }

        #endregion

    }
}
