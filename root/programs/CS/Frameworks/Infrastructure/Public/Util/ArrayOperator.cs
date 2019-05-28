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
//* クラス名        ：ArrayOperation
//* クラス日本語名  ：配列操作クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/28  西野 大介         新規作成（分割
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>配列操作クラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class ArrayOperator
    {
        #region 配列操作

        /// <summary>配列のコピー</summary>
        /// <param name="srcArray">コピー元配列</param>
        /// <param name="dstArraySize">コピー先配列の長さ</param>
        /// <returns>コピー後の配列</returns>
        public static T[] CopyArray<T>(T[] srcArray, int dstArraySize)
        {
            return CopyArray<T>(srcArray, dstArraySize, 0, 0);
        }

        /// <summary>配列のコピー</summary>
        /// <param name="srcArray">コピー元配列</param>
        /// <param name="dstArraySize">コピー先配列の長さ</param>
        /// <param name="srcStartIndex">読取開始位置</param>
        /// <param name="dstStartIndex">書込開始位置</param>
        /// <returns>コピー後の配列</returns>
        public static T[] CopyArray<T>(T[] srcArray, int dstArraySize, int srcStartIndex, int dstStartIndex)
        {
            T[] dstArray = new T[dstArraySize];
            Array.Copy(srcArray, srcStartIndex, dstArray, dstStartIndex, dstArraySize);
            return dstArray;
        }

        /// <summary>配列の結合</summary>
        /// <param name="array1">配列１</param>
        /// <param name="array2">配列２</param>
        /// <returns>結合された配列</returns>
        public static T[] CombineArray<T>(T[] array1, T[] array2)
        {
            T[] combinedArray = new T[array1.Length + array2.Length];
            int typeSize = Marshal.SizeOf(array1.GetType().GetElementType());
            Buffer.BlockCopy(array1, 0, combinedArray, 0, array1.Length * typeSize);
            Buffer.BlockCopy(array2, 0, combinedArray, array1.Length * typeSize, array2.Length * typeSize);

            return combinedArray;
        }

        #endregion 

        #region バイト操作

        /// <summary>
        /// バイト配列を排他的論理和で切り詰め
        /// 単純に切り詰めるなら、CopyArrayを使用する。
        /// </summary>
        /// <param name="bytes">バイト配列</param>
        /// <param name="newSize">バイト配列のサイズ</param>
        /// <returns>指定のサイズに切り詰められたバイト配列</returns>
        /// <remarks>暗号化のキー作成等で使用</remarks>
        public static byte[] ShortenByteArray(byte[] bytes, int newSize)
        {
            byte[] newBytes = new byte[newSize];

            int newBytesIndex = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                // （重なったら）排他的論理和を取る
                // 1 ∧ 1 = 0 、1 ∧ 0 = 1 、0 ∧ 1 = 1 、0 ∧ 0 = 0
                newBytes[newBytesIndex] ^= bytes[i];

                if (newBytesIndex == newSize - 1)
                {
                    newBytesIndex = 0;
                }
                else
                {
                    newBytesIndex++;
                }
            }

            return newBytes;
        }

        /// <summary>バイトデータを数値（Int64）データに変換</summary>
        /// <param name="bytes">バイトデータ（byte[]（8 byte以内））</param>
        /// <returns>数値（Int64）データ</returns>
        public static long GetLongFromByte(byte[] bytes)
        {
            long rtnCode = 0;

            if (bytes.Length <= 0)
            {
                // bytData.Length <= 0（ArgumentOutOfRangeException
                throw new ArgumentOutOfRangeException("bytData");
            }
            else if (bytes.Length <= 8)
            {
                int j = 0; // 256 ( = 8 bit、= 1 byte)
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    // 数値化→bitシフト→加算。
                    int bitShift = 256 * j;
                    if (bitShift == 0) bitShift = 1;
                    rtnCode += Convert.ToInt32(bytes[i]) * bitShift;

                    j++; // 8 bit シフトする。
                }
            }
            else
            {
                // 8 < bytData（ArgumentOutOfRangeException
                throw new ArgumentOutOfRangeException("bytData");
            }

            // 戻す
            return rtnCode;
        }

        #endregion
    }
}
