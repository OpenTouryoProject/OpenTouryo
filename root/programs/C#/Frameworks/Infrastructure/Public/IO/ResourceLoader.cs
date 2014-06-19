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
//* クラス名        ：ResourceLoader
//* クラス日本語名  ：リソース ファイル読み込みクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/03/13  西野  大介        存在チェック処理メソッドを追加
//*  2011/01/14  西野  大介        環境変数の組み込み処理に対応
//**********************************************************************************

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>リソース読み込みクラス</summary>
    /// <remarks>利用箇所：利用箇所を問わない</remarks>
    /// <remarks>自由に利用できる。</remarks>
    public static class ResourceLoader
    {
        #region 存在チェック

        /// <summary>存在チェックのみのメソッド</summary>
        /// <param name="loadfilepath">[リソース ファイル]へのフルパス</param>
        /// <param name="throwException">存在しない場合例外をスローするかどうかを指定</param>
        /// <returns>存在する：true、存在しない：false</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool Exists(string loadfilepath, bool throwException)
        {
            // 環境変数の組み込み処理に対応
            loadfilepath = PubCmnFunction.BuiltStringIntoEnvironmentVariable(loadfilepath);

            // 存在チェック
            if (File.Exists(loadfilepath))
            {
                // 存在する。
                return true;
            }
            else
            {
                // 存在しない。
                if (throwException)
                {
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfilepath));
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>存在チェックのみのメソッド</summary>
        /// <param name="filePath">[リソース ファイル]格納フォルダのパス</param>
        /// <param name="fileName">[リソース ファイル]名</param>
        /// <param name="throwException">存在しない場合例外をスローするかどうかを指定</param>
        /// <returns>存在する：true、存在しない：false</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool Exists(string filePath, string fileName, bool throwException)
        {
            // パス文字結合
            string loadfilepath = Path.Combine(filePath, fileName);

            // 環境変数の組み込み処理に対応
            loadfilepath = PubCmnFunction.BuiltStringIntoEnvironmentVariable(loadfilepath);

            // 存在チェック
            if (File.Exists(loadfilepath))
            {
                // 存在する。
                return true;
            }
            else
            {
                // 存在しない。
                if (throwException)
                {
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfilepath));
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region [リソース ファイル]から文字列を読込

        /// <summary>[リソース ファイル]から文字列を読み込む。</summary>
        /// <param name="loadfilepath">[リソース ファイル]へのフルパス</param>
        /// <param name="enc">エンコード</param>
        /// <returns>[リソース ファイル]から読み込んだ文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string LoadAsString(string loadfilepath, Encoding enc)
        {
            // 環境変数の組み込み処理に対応
            loadfilepath = PubCmnFunction.BuiltStringIntoEnvironmentVariable(loadfilepath);

            StreamReader sr = null;

            try
            {
                // 存在チェック
                if (File.Exists(loadfilepath))
                {
                    // 存在する。
                }
                else
                {
                    // 存在しない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfilepath));
                }

                // 開く
                sr = new StreamReader(loadfilepath, enc);

                // 読む
                return sr.ReadToEnd();
            }
            finally
            {
                //nullチェック
                if (sr == null)
                {
                    //何もしない。
                }
                else
                {
                    //閉じる
                    sr.Close();
                }
            }
        }

        /// <summary>[リソース ファイル]から文字列を読み込む。</summary>
        /// <param name="filePath">[リソース ファイル]格納フォルダのパス</param>
        /// <param name="fileName">[リソース ファイル]名</param>
        /// <param name="enc">エンコード</param>
        /// <returns>[リソース ファイル]から読み込んだ文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string LoadAsString(string filePath, string fileName, Encoding enc)
        {
            // パス文字結合
            string loadfilepath = Path.Combine(filePath, fileName);

            // 環境変数の組み込み処理に対応
            loadfilepath = PubCmnFunction.BuiltStringIntoEnvironmentVariable(loadfilepath);

            StreamReader sr  = null;

            try
            {
                // 存在チェック
                if (File.Exists(loadfilepath))
                {
                    // 存在する。
                }
                else
                {
                    //存在しない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND, loadfilepath));
                }

                // 開く
                sr = new StreamReader(loadfilepath, enc);

                // 読む
                return sr.ReadToEnd();
            }
            finally
            {
                // nullチェック
                if(sr == null)
                {
                    // 何もしない。
                }
                else
                {
                    // 閉じる
                    sr.Close();
                }
            }
        }

        #endregion
    }
}
