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
//* クラス名        ：PubCmnFunction
//* クラス日本語名  ：Public層の共通クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/01/14  西野 大介         新規作成
//*  2011/02/10  西野 大介         GetFileNameNoExメソッドを追加
//*  2011/02/14  西野 大介         BuiltStringIntoEnvironmentVariableにチェック追加
//*  2011/06/01  西野 大介         コマンドライン引数の取得処理の追加
//*  2011/10/09  西野 大介         国際化対応
//*  2012/06/14  西野 大介         CalculateSessionSizeメソッドを追加
//*  2012/06/21  西野 大介         1～8バイト → 数値変換メソッドを追加
//*  2012/12/26  西野 大介         コマンドライン引数の取得処理の修正（.exe無しのケース）
//*  2013/02/12  西野 大介         ShortenByteArrayメソッドを追加（暗号化ツールで使用）
//*  2013/12/23  西野 大介         ファイル・行数・メソッド・プロパティなどコード情報の取得メソッドを追加
//*  2017/01/31  西野 大介         System.Webを使用しているCalculateSessionSizeメソッドをBusinessへ移動
//*  2017/11/29  西野 大介         DateTimeOffset.ToUnixTimeSecondsの前方互換メソッドを追加
//*  2018/08/01  西野 大介         Nullable対応の型変換メソッドを追加
//*  2018/10/03  西野 大介         GetDataReaderColumnInfoメソッドを追加。
//*  2018/10/03  西野 大介         CombineByteArrayメソッドを追加。
//*  2018/10/03  西野 大介         StringVariableOperator, StackFrameOperator, ArrayOperatorに分割
//**********************************************************************************

using System;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>Public層の共通クラス</summary>
    public class PubCmnFunction
    {
        #region 型変換

        /// <summary>Nullable対応の型変換メソッド</summary>
        /// <param name="srcValue">変換前の値</param>
        /// <param name="dstType">変換後の型</param>
        /// <returns>変換後の値</returns>
        public static object ChangeType(object srcValue, Type dstType)
        {
            Type t = dstType;

            if (t.IsGenericType
                && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // dstTypeがNullableの場合

                if (srcValue == null)
                {
                    // srcValueがnullの場合、変換不要。
                    return null;
                }
                else
                {
                    // srcValueがnullで無い場合、非Nullable化
                    t = Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                // dstTypeがNullableで無い場合、型情報はそのまま。
            }

            // 上記の t を使用してConvertする。
            // Convert.ChangeType() doesn't handle nullables -> use UnderlyingType.
            return Convert.ChangeType(srcValue, t);
        }

        /// <summary>Nullable対応メソッド</summary>
        /// <param name="t">型</param>
        /// <returns>tがNullableの場合、Nullableでない元の型を返す。</returns>
        public static Type GetUnderlyingType(Type t)
        {
            if (t.IsGenericType
                && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // tがNullableの場合、非Nullable化
                return Nullable.GetUnderlyingType(t);
            }
            else
            {
                // tがNullableで無い場合、null。
                return null;
            }
        }

        #endregion

        #region 互換性

        /// <summary>UNIXエポック</summary>
        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>DateTimeOffset.ToUnixTimeSeconds代替(net46未満で使用)</summary>
        /// <param name="targetTime">DateTimeOffset</param>
        /// <returns>UnixTimeSeconds</returns>
        public static long ToUnixTime(DateTimeOffset targetTime)
        {
            // UTC時間に変換
            targetTime = targetTime.ToUniversalTime();
            // UNIXエポックからの経過時間を取得
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;
            // 経過秒数に変換して返す（UnixTime）
            return (long)elapsedTime.TotalSeconds;
        }

        #endregion

        #region その他

        /// <summary>拡張子無しのファイル名を取得する</summary>
        /// <param name="str">ファイル名を含むパス</param>
        /// <param name="divChar">パスの分割文字</param>
        /// <returns>拡張子無しのファイル名</returns>
        public static string GetFileNameNoEx(string str, char divChar)
        {
            // ワーク変数
            string ret = "";
            string[] aryTemp;

            // 分解して組み立て
            aryTemp = str.Split(divChar);
            aryTemp = aryTemp[aryTemp.Length - 1].Split('.');

            for (int i = 0; i < aryTemp.Length - 1; i++)
            {
                if (ret == "")
                {
                    ret = aryTemp[i];
                }
                else
                {
                    ret += '.' + aryTemp[i];
                }
            }

            return ret;
        }

        /// <summary>IDataReaderの列情報を格納したHashSet(string)を返す。</summary>
        /// <param name="dr">IDataReader</param>
        /// <returns>HashSet(string)</returns>
        public static HashSet<string> GetDataReaderColumnInfo(IDataReader dr)
        {
            HashSet<string> hs = new HashSet<string>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                hs.Add(dr.GetName(i));
            }

            return hs;
        }

        #endregion

        #region 下位互換

        #region StringVariableOperator

        #region プロパティ文字列の処理

        /// <summary>プロパティ文字列からプロパティ名・値の組を取得</summary>
        /// <param name="propString">プロパティ文字列</param>
        /// <returns>プロパティ名・値（ディクショナリ）</returns>
        [Obsolete("Please substitute by StringVariableOperator.GetPropsFromPropString().")]
        public static Dictionary<string, string> GetPropsFromPropString(string propString)
        {
            return StringVariableOperator.GetPropsFromPropString(propString);
        }

        #endregion

        #region コマンドライン引数の処理

        /// <summary>
        /// コマンドライン引数を取得する。
        /// </summary>
        /// <param name="prefixChar">
        /// コマンドの接頭文字
        /// </param>
        /// <param name="argsDic">
        /// コマンドライン引数のコマンド＆値
        /// （コマンドは全て大文字に揃える）
        /// </param>
        /// <param name="valsLst">
        /// コマンドライン引数の値
        /// </param>
        [Obsolete("Please substitute by StringVariableOperator.GetCommandArgs().")]
        public static void GetCommandArgs(char prefixChar, out Dictionary<string, string> argsDic, out List<string> valsLst)
        {
            StringVariableOperator.GetCommandArgs(prefixChar, out argsDic, out valsLst);
            return;
        }

        #endregion

        #region 環境変数の組み込み処理

        /// <summary>環境変数名入り文字列に、環境変数値を組み込む。</summary>
        /// <param name="builtString">環境変数名入り文字列</param>
        /// <returns>環境変数値を組み込んだ文字列</returns>
        /// <remarks>環境変数名は、環境変数名入り文字列中に、%環境変数名%と指定する。</remarks>
        [Obsolete("Please substitute by StringVariableOperator.BuiltStringIntoEnvironmentVariable().")]
        public static string BuiltStringIntoEnvironmentVariable(string builtString)
        {
            return StringVariableOperator.BuiltStringIntoEnvironmentVariable(builtString);
        }

        #endregion

        #endregion

        #region StackFrameOperator

        #region コード情報を取得

        /// <summary>カンレントMethod名を取得する。</summary>
        /// <returns>カンレントMethod名</returns>
        public static string GetCurrentMethodName()
        {
            return StackFrameOperator.GetCurrentMethodName();
        }

        /// <summary>カンレントProperty名を取得する。</summary>
        /// <returns>カンレントProperty名</returns>
        public static string GetCurrentPropertyName()
        {
            return StackFrameOperator.GetCurrentPropertyName();
        }

        /// <summary>カンレント・コード情報を取得する。</summary>
        /// <param name="filePath">ファイル・パス</param>
        /// <param name="fileLineNumber">ファイル行数</param>
        /// <param name="methodSignature">メソッド・シグネチャ</param>
        public static void GetCurrentCodeInfo(out string filePath, out string fileLineNumber, out string methodSignature)
        {
            StackFrameOperator.GetCurrentCodeInfo(out filePath, out fileLineNumber, out methodSignature);
            return;
        }

        #endregion

        #endregion

        #region ArrayOperator

        #region 配列操作

        /// <summary>配列のコピー</summary>
        /// <param name="srcArray">コピー元配列</param>
        /// <param name="dstArraySize">コピー先配列の長さ</param>
        /// <returns>コピー後の配列</returns>
        [Obsolete("Please substitute by ArrayOperator.CopyArray().")]
        public static T[] CopyArray<T>(T[] srcArray, int dstArraySize)
        {
            return ArrayOperator.CopyArray<T>(srcArray, dstArraySize, 0, 0);
        }

        /// <summary>配列のコピー</summary>
        /// <param name="srcArray">コピー元配列</param>
        /// <param name="dstArraySize">コピー先配列の長さ</param>
        /// <param name="srcStartIndex">読取開始位置</param>
        /// <param name="dstStartIndex">書込開始位置</param>
        /// <returns>コピー後の配列</returns>
        [Obsolete("Please substitute by ArrayOperator.CopyArray().")]
        public static T[] CopyArray<T>(T[] srcArray, int dstArraySize, int srcStartIndex, int dstStartIndex)
        {
            return ArrayOperator.CopyArray<T>(srcArray, dstArraySize, srcStartIndex, dstStartIndex); ;
        }

        /// <summary>配列の結合</summary>
        /// <param name="array1">配列１</param>
        /// <param name="array2">配列２</param>
        /// <returns>結合された配列</returns>
        [Obsolete("Please substitute by ArrayOperator.CombineArray().")]
        public static T[] CombineArray<T>(T[] array1, T[] array2)
        {
            return ArrayOperator.CombineArray<T>(array1, array2);
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
        [Obsolete("Please substitute by ArrayOperator.ShortenByteArray().")]
        public static byte[] ShortenByteArray(byte[] bytes, int newSize)
        {
            return ArrayOperator.ShortenByteArray(bytes, newSize);
        }

        /// <summary>バイトデータを数値（Int64）データに変換</summary>
        /// <param name="bytes">バイトデータ（byte[]（8 byte以内））</param>
        /// <returns>数値（Int64）データ</returns>
        [Obsolete("Please substitute by ArrayOperator.GetLongFromByte().")]
        public static long GetLongFromByte(byte[] bytes)
        {
            return ArrayOperator.GetLongFromByte(bytes);
        }

        #endregion

        #endregion

        #endregion
    }
}
