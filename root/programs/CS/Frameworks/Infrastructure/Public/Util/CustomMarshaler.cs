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
//* クラス名        ：CustomMarshaler
//* クラス日本語名  ：カスタム マーシャリング クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/11/19  西野 大介         新規作成
//*  2015/01/15	 Supragyan         Added StringFromPrimitivetype method to convert premitive data to string.
//*  2015/01/15	 Supragyan         Added PrimitivetypeFromString method to convert string to premitive data.
//*  2026/08/14  玄人 幸道         StringFromPrimitivetype / PrimitivetypeFromString を
//*                                DTColumn へ移動し、ここには転送のみ残した（#544）。
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

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

        #region 値と文字列の相互変換（DTColumnへ移動）

        // ＜なぜ移したか＞（#544）
        //   本クラスの本体はアンマネージ構造体の相互運用（Marshal クラス）で、
        //   DTType の変換とは責務が違う。同じ「マーシャリング」という語が
        //   2 つの意味で使われていたため、同居していた。
        //
        //   また、この 2 メソッドのためだけに Util から Dto を参照しており、
        //   Dto → Util の参照と合わせて循環していた。
        //
        //   対になる DTColumn.AutoCast / CheckType / EnumToString は
        //   もともと Dto 側にあるため、そちらへ集約した。
        //
        // ＜ここに残してある理由＞
        //   公開している API のため、いきなり消すと利用者のビルドが壊れる。
        //   当面は転送するだけのメソッドを残す。
        //   これを削除すると、Util から Dto への参照も無くなる。

        /// <summary>
        /// To converts all premitive data to string data.
        /// </summary>
        /// <param name="primitiveType">Column data type</param>
        /// <param name="checkType">Check sting Type data</param>
        /// <returns>returns String data</returns>
        [Obsolete("DTColumn.StringFromPrimitivetype へ移動しました。")]
        public static string StringFromPrimitivetype(object primitiveType, bool checkType)
        {
            return DTColumn.StringFromPrimitivetype(primitiveType, checkType);
        }

        /// <summary>
        /// To converts all string data to premitive Type by colType and cellString.
        /// </summary>
        /// <param name="colType">Column data type</param>
        /// <param name="cellString">Cell data</param>
        /// <returns>return Premitive type object</returns>
        [Obsolete("DTColumn.PrimitivetypeFromString へ移動しました。")]
        public static object PrimitivetypeFromString(DTType colType, string cellString)
        {
            return DTColumn.PrimitivetypeFromString(colType, cellString);
        }
        #endregion

    }
}
