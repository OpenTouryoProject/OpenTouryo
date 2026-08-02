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
//* クラス名        ：ResourceLoader
//* クラス日本語名  ：リソース ファイル読み込みクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2009/03/13  西野 大介         存在チェック処理メソッドを追加
//*  2011/01/14  西野 大介         環境変数の組み込み処理に対応
//*  2026/08/01  玄人 幸道         ResolveFilePathメソッドを追加し、探索処理を集約した。
//*                                相対パスがカレント ディレクトリ基準で見つからない場合、
//*                                AppContext.BaseDirectory（EXEの配置フォルダ）基準で再探索する。
//*                                ※ CLIが任意のカレント ディレクトリから起動される問題への対応。
//**********************************************************************************

using System;
using System.IO;
using System.Text;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>リソース読み込みクラス</summary>
    /// <remarks>利用箇所：利用箇所を問わない</remarks>
    /// <remarks>自由に利用できる。</remarks>
    public static class ResourceLoader
    {
        // ↓↓↓【追加】↓↓↓

        #region パス解決

        /// <summary>[リソース ファイル]の実パスを解決する</summary>
        /// <param name="loadfilepath">[リソース ファイル]へのパス</param>
        /// <returns>見つかった実パス。見つからない場合は null。</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 探索順は下記のとおり。
        /// (1) 指定されたパス（絶対パス、または カレント ディレクトリ基準の相対パス）
        /// (2) AppContext.BaseDirectory（EXEの配置フォルダ）基準の相対パス
        /// (1) を優先するため、従来動作していた構成の挙動は変わらない。
        /// (2) は、CLIが任意のカレント ディレクトリから起動される場合への対応。
        /// ※ AppContext.BaseDirectoryは、PublishSingleFile（単一ファイル発行）でも
        ///    正しい値を返す（Assembly.Locationは空文字になるため使用しない）。
        /// </remarks>
        public static string ResolveFilePath(string loadfilepath)
        {
            // 環境変数の組み込み処理に対応
            loadfilepath = StringVariableOperator.BuiltStringIntoEnvironmentVariable(loadfilepath);

            if (string.IsNullOrEmpty(loadfilepath))
            {
                // 指定なし
                return null;
            }

            // (1) 指定されたパスで探す。
            if (File.Exists(loadfilepath))
            {
                return loadfilepath;
            }

            // (2) 見つからない場合、EXEの配置フォルダ基準で探す（相対パスの場合のみ）。
            if (!Path.IsPathRooted(loadfilepath))
            {
                string loadfilepath2 = Path.Combine(AppContext.BaseDirectory, loadfilepath);

                if (File.Exists(loadfilepath2))
                {
                    return loadfilepath2;
                }
            }

            // 見つからない。
            return null;
        }

        #endregion

        // ↑↑↑【追加】↑↑↑

        #region 存在チェック

        /// <summary>存在チェックのみのメソッド</summary>
        /// <param name="loadfilepath">[リソース ファイル]へのフルパス</param>
        /// <param name="throwException">存在しない場合例外をスローするかどうかを指定</param>
        /// <returns>存在する：true、存在しない：false</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool Exists(string loadfilepath, bool throwException)
        {
            // 存在チェック（【変更】探索処理を ResolveFilePath に集約した）
            if (ResourceLoader.ResolveFilePath(loadfilepath) != null)
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

            // 存在チェック（【変更】探索処理を ResolveFilePath に集約した）
            return ResourceLoader.Exists(loadfilepath, throwException);
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
            // 存在チェック（【変更】探索処理を ResolveFilePath に集約した）
            string resolvedPath = ResourceLoader.ResolveFilePath(loadfilepath);

            StreamReader sr = null;

            try
            {
                // 存在チェック
                if (resolvedPath != null)
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
                sr = new StreamReader(resolvedPath, enc);

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

            // 存在チェック（【変更】探索処理を ResolveFilePath に集約した）
            string resolvedPath = ResourceLoader.ResolveFilePath(loadfilepath);

            StreamReader sr  = null;

            try
            {
                // 存在チェック
                if (resolvedPath != null)
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
                sr = new StreamReader(resolvedPath, enc);

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
