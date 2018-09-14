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
//* クラス名        ：BinarySerialize
//* クラス日本語名  ：バイナリシリアライズクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2010/11/21  西野 大介         DeepCloneメソッドを追加
//**********************************************************************************

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>バイナリシリアライズクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class BinarySerialize
    {
        /// <summary>指定したオブジェクトをバイナリシリアライズし、byte配列に変換する。</summary>
        /// <param name="obj">シリアライズするオブジェクト</param>
        /// <returns>オブジェクトをバイナリシリアライズしたbyte配列</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// ※ 引数オブジェクトのSerialize対応が前提
        /// </remarks>
        public static byte[] ObjectToBytes(object obj)
        {
            // チェック
            if (obj == null)
            {
                throw new ArgumentNullException("obj",
                    String.Format(PublicExceptionMessage.PARAM_IS_NULL, "obj"));
            }

            // メモリストリーム
            MemoryStream mem = new MemoryStream();

            // バイナリシリアライズクラス
            BinaryFormatter bformatter = new BinaryFormatter();

            // シリアライズ（メモリに書き込む）
            bformatter.Serialize(mem, obj);
            mem.Close();

            // バイト配列として返す
            return mem.ToArray();
        }

        /// <summary>バイナリシリアライズしたbyte配列から、オブジェクトを復元する。</summary>
        /// <param name="abytObject">オブジェクトをバイナリシリアライズしたbyte配列</param>
        /// <returns>byte配列から復元したオブジェクト</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// </remarks>
        public static object BytesToObject(byte[] abytObject)
        {
            // チェック
            if (abytObject == null)
            {
                throw new ArgumentNullException("abytObject",
                    String.Format(PublicExceptionMessage.PARAM_IS_NULL, "abytObject"));
            }

            // メモリストリーム
            MemoryStream mem = new MemoryStream();

            // バイト配列をメモリに書き込む
            mem.Write(abytObject, 0, abytObject.Length);
            mem.Flush();
            mem.Seek(0, SeekOrigin.Begin);

            // バイナリシリアライズクラス
            BinaryFormatter bformatter = new BinaryFormatter();

            // デシリアライズ（メモリから読み込む）
            object obj = bformatter.Deserialize(mem);
            mem.Close();

            // オブジェクトとして返す。
            return obj;
        }

        /// <summary>
        /// オブジェクトをディープコピーする。
        /// http://csharp.yaminabe.info/2006/08/post_1.html
        /// </summary>
        /// <param name="sourceObject">コピー元オブジェクト</param>
        /// <returns>コピー先オブジェクト</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// ※ 引数オブジェクトのSerialize対応が前提
        /// </remarks>
        public static object DeepClone(object sourceObject)
        {
            // チェック
            if (sourceObject == null)
            {
                throw new ArgumentNullException("sourceObject",
                    String.Format(PublicExceptionMessage.PARAM_IS_NULL, "sourceObject"));
            }

            // Dispose、Closeが確実に呼び出されるようにする
            // http://dobon.net/vb/dotnet/beginner/calldispose.html

            // C# Tips －usingを使え、使えったら使え(^^)－ 
            // http://www.divakk.co.jp/aoyagi/csharp_tips_using.html

            // GCでDispose、Closeされるので、基本的に問題ない（必須ではないが）が、
            // 以降は、usingステートメントをなるべく取り入れていこうと考える。

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Serialize → Deserializeでディープコピー
                formatter.Serialize(stream, sourceObject);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }
    }
}
