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
//* クラス名        ：PubCmnFunction
//* クラス日本語名  ：Public層の共通クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/01/14  西野  大介        新規作成
//*  2011/02/10  西野  大介        GetFileNameNoExメソッドを追加
//*  2011/02/14  西野  大介        BuiltStringIntoEnvironmentVariableにチェック追加
//*  2011/06/01  西野  大介        コマンドライン引数の取得処理の追加
//*  2011/10/09  西野  大介        国際化対応
//*  2012/06/14  西野  大介        CalculateSessionSizeメソッドを追加
//*  2012/06/21  西野  大介        1～8バイト → 数値変換メソッドを追加
//*  2012/12/26  西野  大介        コマンドライン引数の取得処理の修正（.exe無しのケース）
//*  2013/02/12  西野  大介        ShortenByteArrayメソッドを追加（暗号化ツールで使用）
//*  2013/12/23  西野  大介        ファイル・行数・メソッド・プロパティなどコード情報の取得メソッドを追加
//**********************************************************************************

// 処理に必要
using System.Collections.Generic;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Web;
using System.Collections;
using System.Diagnostics;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Win32;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>Public層の共通クラス</summary>
    public class PubCmnFunction
    {
        #region プロパティ文字列の処理

        /// <summary>プロパティ文字列からプロパティ名・値の組を取得</summary>
        /// <param name="propString">プロパティ文字列</param>
        /// <returns>プロパティ名・値（ディクショナリ）</returns>
        /// <remarks>
        /// プロパティ文字列のフォーマット
        /// [property=value[;property=value]] 
        /// 
        /// [ property ]、[ value ]に[ = ]、[ ; ]が混じる場合は、
        /// { }で囲い（ {=}、{;} ）エスケープできる。
        /// ただし、{ }のエスケープはできない。
        /// ※ 特殊文字は、=、;、{、}
        /// </remarks>
        public static Dictionary<string, string> GetPropsFromPropString(string propString)
        {
            // ディクショナリ
            Dictionary<string, string> ht = new Dictionary<string, string>();

            // ストリング ビルダ
            StringBuilder sb = new StringBuilder();

            // プロパティ名・値
            string propName = "";
            string propValue = "";

            // ステータス
            bool isFirst = true;

            bool isMidPtHead = false;
            bool isInMidPt = false;
            bool isMidPtFoot = false;

            bool isInPropName = false;
            bool isInPropValue = false;

            foreach (char ch in propString)
            {
                if (isFirst)
                {
                    // 先頭

                    // 先頭終わり
                    isFirst = false;

                    // プロパティ開始
                    isInPropName = true;
                    isInPropValue = false;

                    if (ch == '=' || ch == ';' || ch == '}')
                    {
                        // エラー
                        PubCmnFunction.ThrowPropStringFormatException(
                            PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_START_CHARACTER);
                    }
                    else if (ch == '{')
                    {
                        // 中括弧開始
                        isMidPtHead = true;
                        isInMidPt = false;
                        isMidPtFoot = false;
                    }
                    else
                    {
                        // 追加
                        sb.Append(ch);
                    }
                }
                else
                {
                    // 先頭後

                    if (isMidPtHead)
                    {
                        if (ch == '=' || ch == ';')
                        {
                            // 中括弧中
                            isMidPtHead = false;
                            isInMidPt = true;
                            isMidPtFoot = false;

                            // 追加
                            sb.Append(ch);
                        }
                        else
                        {
                            // エラー
                            PubCmnFunction.ThrowPropStringFormatException(String.Format(
                                PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_ESCAPE_CHARACTER, ch));
                        }
                    }
                    else if (isInMidPt)
                    {
                        if (ch == '}')
                        {
                            // 中括弧後
                            isMidPtHead = false;
                            isInMidPt = false;
                            isMidPtFoot = true;
                        }
                        else
                        {
                            // エラー
                            PubCmnFunction.ThrowPropStringFormatException(
                                PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_CURLY_BRACE + "-1");
                        }
                    }
                    else
                    {
                        // 中括弧外になった
                        if (isMidPtFoot)
                        {
                            // 初期化
                            isMidPtHead = false;
                            isInMidPt = false;
                            isMidPtFoot = false;
                        }

                        // 中括弧外
                        if (ch == '{')
                        {
                            // 中括弧開始
                            isMidPtHead = true;
                            isInMidPt = false;
                            isMidPtFoot = false;
                        }
                        if (ch == '}')
                        {
                            // エラー
                            PubCmnFunction.ThrowPropStringFormatException(
                                PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_CURLY_BRACE + "-2");
                        }
                        else if (ch == '=')
                        {
                            // プロパティ名の終わり
                            if (isInPropName)
                            {
                                if (sb.ToString() == "")
                                {
                                    // エラー
                                    PubCmnFunction.ThrowPropStringFormatException(
                                        PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_PROPERTY_NAME_IS_EMPTY);
                                }
                                else
                                {
                                    // プロパティ名が在
                                    propName = sb.ToString();

                                    // ストリング ビルダのクリア
                                    sb = new StringBuilder();

                                    // 状態変更
                                    isInPropName = false;
                                    isInPropValue = true;
                                }
                            }
                            else
                            {
                                // エラー
                                PubCmnFunction.ThrowPropStringFormatException(
                                    PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_VALUE);
                            }
                        }
                        else if (ch == ';')
                        {
                            // プロパティ値の終わり
                            if (isInPropValue)
                            {
                                if (sb.ToString() == "")
                                {
                                    // エラー
                                    PubCmnFunction.ThrowPropStringFormatException(
                                        PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_PROPERTY_VALUE_IS_EMPTY);
                                }
                                else
                                {
                                    // プロパティ値が在
                                    propValue = sb.ToString();

                                    // ストリング ビルダのクリア
                                    sb = new StringBuilder();

                                    // 状態変更
                                    isInPropName = true;
                                    isInPropValue = false;
                                }
                            }
                            else
                            {
                                // エラー
                                PubCmnFunction.ThrowPropStringFormatException(
                                    PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_NAME);
                            }
                        }
                        else
                        {
                            // 追加
                            sb.Append(ch);
                        }

                        // 両方に値が入った
                        if (propName != "" && propValue != "")
                        {
                            // 重複していたら上書きという仕様
                            ht[propName.ToUpper()] = propValue; // #33-この行

                            // 値をクリアする。
                            propName = "";
                            propValue = "";
                        }
                    }
                }
            }

            // プロパティ値の終わり
            if (isInPropValue)
            {
                if (sb.ToString() == "")
                {
                    // プロパティ値が空
                    // エラー
                }
                else
                {
                    // プロパティ値が在
                    propValue = sb.ToString();

                    // ストリング ビルダのクリア
                    sb = new StringBuilder();

                    // 状態変更
                    isInPropName = true;
                    isInPropValue = false;
                }
            }
            else
            {
                //// エラー
                //Form1.ThrowPropStringFormatException("6");
                // →　最後が;で終わっている場合が有るので、エラーを上げない。
            }

            // 両方に値が入った
            if (propName != "" && propValue != "")
            {
                // 重複していたら上書きという仕様
                ht[propName.ToUpper()] = propValue; // #33-この行

                // 値をクリアする。
                propName = "";
                propValue = "";
            }

            return ht;
        }

        /// <summary>プロパティ文字列のフォーマット エラー</summary>
        private static void ThrowPropStringFormatException(string detail)
        {
            throw new ArgumentException(
                String.Format(PublicExceptionMessage.PROP_STRING_FORMAT_ERROR, detail));
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
        public static void GetCommandArgs(char prefixChar, out Dictionary<string, string> argsDic, out List<string> valsLst)
        {
            #region 初期処理

            // 戻り値の初期化
            valsLst = new List<string>();
            argsDic = new Dictionary<string, string>();

            // exe名
            string exeName = Process.GetCurrentProcess().MainModule.ModuleName;

            // CommandLine
            string commandLine = Environment.CommandLine;

            // 最初の半角スペースのインデックスを検索
            int index = 0;
            bool isInDoubleQuote = false;
            foreach (char ch in commandLine)
            {
                // インクリメント
                index++;

                // 出たり入ったり。
                if (ch == '\"')
                {
                    isInDoubleQuote = !isInDoubleQuote;
                }

                // ""外の半角スペース
                if (!isInDoubleQuote && ch == ' ')
                {
                    break;
                }
            }

            // コマンドラインが存在しない場合。
            if (commandLine.Length <= index)
            {
                return;
            }

            string temp = commandLine.Substring(index).Trim();

            //// CommandLineからargsを切り取る
            //try
            //{
            //    // パスの最後に\が付く（[+ 1] は[\]の分）
            //    temp = commandLine.Substring(commandLine.IndexOf(exeName) + exeName.Length + 1).Trim();
            //}
            //catch
            //{
            //    // パスの最後に\が付かない
            //    temp = commandLine.Substring(commandLine.IndexOf(exeName) + exeName.Length).Trim();
            //}

            #endregion

            // ここから引数をバラす。
            #region argsLstを取得する。

            // ビルダ
            List<string> argsLst = new List<string>();
            StringBuilder arg = new StringBuilder();

            // ダブルクォーテの中か？
            bool isInDoubleQuotation = false;

            // 前の文字
            char preChar = 'z';
            bool haveEscapeSequence = false;

            if (temp == "")
            {
                // 空ならnull
                return;
            }

            foreach (char ch in temp)
            {
                if (ch == '\\')  // 特殊文字（エスケープ文字）
                {
                    if (preChar == '\\' && haveEscapeSequence)
                    {
                        // エスケープされている。
                        arg.Append(ch);
                        // ステータス変更
                        haveEscapeSequence = false;
                    }
                    else
                    {
                        // エスケープされていない。
                        // Appendしない。
                        // ステータス変更
                        haveEscapeSequence = true;
                    }
                }
                else if (ch == ' ') // 特殊文字（半角スペース）
                {
                    if (isInDoubleQuotation)
                    {
                        // ダブルクォーテ内なら追加
                        arg.Append(ch);
                    }
                    else if (preChar == ' ')
                    {
                        // 連続する半角スペース（無視）
                    }
                    else
                    {
                        // 連続しない半角スペース（初出）
                        // 切り出し
                        argsLst.Add(arg.ToString());
                        arg = new StringBuilder();

                        // ステータス変更
                        isInDoubleQuotation = false;
                    }
                }
                else if (ch == '"')  // 特殊文字（ダブルクォーテ）
                {
                    if (preChar == '\\' && haveEscapeSequence)
                    {
                        // エスケープされている。
                        arg.Append(ch);
                    }
                    else
                    {
                        // エスケープされていない。
                        // Appendしない。

                        // ステータス変更
                        isInDoubleQuotation = !isInDoubleQuotation;
                    }
                }
                else // その他の文字
                {
                    arg.Append(ch);
                }

                preChar = ch;
            }

            // 切り出し
            argsLst.Add(arg.ToString());
            arg = new StringBuilder();

            #endregion

            // バラした引数を再構成。
            #region argsDic、valLstを取得する。

            bool isKey_cur = false;
            bool isKey_pre = false;

            string valString = "";
            string keyString = "";
            string keyString_pre = "";

            foreach (string str in argsLst)
            {
                // 判別
                if (str != string.Empty)
                {
                    // キーか値か（ステータス更新）
                    if (str[0] == prefixChar)
                    {
                        // キー
                        isKey_pre = isKey_cur;
                        isKey_cur = true;
                    }
                    else
                    {
                        // 値
                        isKey_pre = isKey_cur;
                        isKey_cur = false;
                    }

                    // キーか値か（文字列取得）
                    if (isKey_cur)
                    {
                        // キー
                        if (isKey_pre)
                        {
                            // 前のキー
                            keyString_pre = keyString;
                        }

                        keyString = str.ToUpper();
                    }
                    else
                    {
                        // 値
                        valString = str;
                    }

                    // 戻り値の生成
                    if (isKey_cur && isKey_pre)
                    {
                        // キーのみ（連続したキー）
                        argsDic[keyString_pre] = "";
                    }
                    else if (!isKey_cur && isKey_pre)
                    {
                        // キーの値（キーでない、前はキー）
                        argsDic[keyString] = valString;
                    }
                    else if (!isKey_cur && !isKey_pre)
                    {
                        // ただの値（キーでない、前も値）
                        valsLst.Add(valString);
                    }
                }
            }

            // 戻り値の生成（一つ前の値）
            if (isKey_cur)
            {
                // キーのみ（末端のキー）
                argsDic[keyString] = "";
            }

            #endregion

            return;
        }

        #endregion

        #region 環境変数の組み込み処理

        /// <summary>環境変数名入り文字列に、環境変数値を組み込む。</summary>
        /// <param name="builtString">環境変数名入り文字列</param>
        /// <returns>環境変数値を組み込んだ文字列</returns>
        /// <remarks>環境変数名は、環境変数名入り文字列中に、%環境変数名%と指定する。</remarks>
        public static string BuiltStringIntoEnvironmentVariable(string builtString)
        {   
            if (builtString == null || builtString == "")
            {
                // nullや、空文字列の場合は、そのまま返す。
                return builtString;
            }

            // インデックス
            int index = 0;
            // 環境変数値を組み込んだ文字列
            string retStr = "";

            // 前の要素が環境変数名だったかどうかを示すフラグ
            bool preEmtIsEnvVar = false;

            if (builtString.IndexOf('%') != -1)
            {
                // 「%」を含む文字列を、Splitする。
                string[] aryTempStr = builtString.Split('%');

                // ループ処理
                foreach (string tempStr in aryTempStr)
                {
                    // 「%」に区切り、偶数番目要素が環境変数名と認識する。
                    // ただし、最後の要素は、環境変数名と認識しない。
                    if (((index % 2) == 1)
                        && (index < (aryTempStr.Length - 1)))
                    {
                        // 変換対象 = 環境変数名（%の間）
                        string envVar = Environment.GetEnvironmentVariable(tempStr, EnvironmentVariableTarget.Process);

                        // 環境変数値を確認
                        if (envVar == null || envVar == "")
                        {
                            // 環境変数無し
                        }
                        else
                        {
                            // 環境変数有り
                            retStr += envVar;
                        }

                        // フラグの設定
                        preEmtIsEnvVar = true;
                    }
                    else
                    {
                        // 非変換対象 = 非環境変数名（%の外）

                        // 元の形に復元する。
                        if (index == 0)
                        {
                            // 先頭
                            retStr += tempStr;
                        }
                        else
                        {
                            // 中間～末端
                            if (preEmtIsEnvVar)
                            {
                                // 前の要素が環境変数名
                                retStr += tempStr;
                            }
                            else
                            {
                                // 前の要素が環境変数名でない。
                                retStr += ("%" + tempStr);
                            }
                        }

                        // フラグの設定
                        preEmtIsEnvVar = false;
                    }

                    index++; // インデックスのカウントアップ
                }
            }
            else
            {
                // 「%」を含まない文字列は、そのまま返す。
                return builtString;
            }

            // 環境変数値を組み込んだ文字列を返す。
            return retStr;
        }

        #endregion

        #region Web系

        /// <summary>Sessionサイズ測定</summary>
        /// <returns>Sessionサイズ（MB）</returns>
        /// <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        public static long CalculateSessionSizeMB()
        {
            return PubCmnFunction.CalculateSessionSizeKB() / 1000;
        }

        /// <summary>Sessionサイズ測定</summary>
        /// <returns>Sessionサイズ（KB）</returns>
        /// <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        public static long CalculateSessionSizeKB()
        {
            return PubCmnFunction.CalculateSessionSize() / 1000;
        }

        /// <summary>Sessionサイズ測定</summary>
        /// <returns>Sessionサイズ（バイト）</returns>
        /// <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        public static long CalculateSessionSize()
        {
            // ワーク変数
            long size = 0;

            // SessionのオブジェクトをBinarySerializeしてサイズを取得。
            foreach (string key in HttpContext.Current.Session.Keys)
            {
                // 当該キーのオブジェクト・サイズを足しこむ。
                size += BinarySerialize.ObjectToBytes(HttpContext.Current.Session[key]).Length;
            }

            // Sessionサイズ（バイト）
            return size;
        }

        #endregion

        #region コード情報を取得

        /// <summary>カンレントMethod名を取得する。</summary>
        /// <returns>カンレントMethod名</returns>
        public static string GetCurrentMethodName()
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        /// <summary>カンレントProperty名を取得する。</summary>
        /// <returns>カンレントProperty名</returns>
        public static string GetCurrentPropertyName()
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);
            string currentMethodName = sf.GetMethod().Name;

            // Property名に変換
            if (string.IsNullOrEmpty(currentMethodName))
            {
                return "";
            }
            else
            {
                if (currentMethodName.IndexOf("get_") == 0 ||
                    currentMethodName.IndexOf("set_") == 0)
                {
                    // 先頭のget_、set_がある場合、これを削る。
                    return currentMethodName.Substring(4);
                }
                else
                {
                    // Propertyではない。
                    return "";
                }
            }
        }

        /// <summary>カンレント・コード情報を取得する。</summary>
        /// <param name="filePath">ファイル・パス</param>
        /// <param name="fileLineNumber">ファイル行数</param>
        /// <param name="methodSignature">メソッド・シグネチャ</param>
        public static void GetCurrentCodeInfo(out string filePath, out string fileLineNumber, out string methodSignature)
        {
            // 呼び出し元のStackFrameを取得
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);

            // ファイル・パス
            filePath = sf.GetFileName();
            // ファイル行数
            fileLineNumber = sf.GetFileLineNumber().ToString();
            // メソッド・シグネチャ
            methodSignature = sf.GetMethod().ToString();
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

        /// <summary>バイト配列の切り詰め</summary>
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
        /// <param name="bytData">バイトデータ（byte[]（8 byte以内））</param>
        /// <returns>数値（Int64）データ</returns>
        public static long GetLongFromByte(byte[] bytData)
        {
            long rtnCode = 0;

            if (bytData.Length <= 0)
            {
                // bytData.Length <= 0（ArgumentOutOfRangeException
                throw new ArgumentOutOfRangeException("bytData");
            }
            else if (bytData.Length <= 8)
            {
                int j = 0; // 256 ( = 8 bit、= 1 byte)
                for (int i = bytData.Length - 1; i >= 0; i--)
                {
                    // 数値化→bitシフト→加算。
                    int bitShift = 256 * j;
                    if (bitShift == 0) bitShift = 1;
                    rtnCode += Convert.ToInt32(bytData[i]) * bitShift;

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
