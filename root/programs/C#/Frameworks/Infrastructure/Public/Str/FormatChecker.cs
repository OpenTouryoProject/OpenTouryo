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
//* クラス名        ：FormatChecker
//* クラス日本語名  ：正規表現を用いた書式チェック処理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/xx/xx  西野  大介        新規作成
//**********************************************************************************

// メール アドレス クラスを活用
using System.Net.Mail;

// System
using System;
using System.Text;
using System.Collections;

using System.Globalization;
using System.Text.RegularExpressions;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>正規表現を用いた汎用書式チェック処理クラス</summary>
    public class FormatChecker
    {
        #region 郵便番号（日本）形式チェック

        #region 郵便（区）番号

        /// <summary>指定された文字列が郵便（区）番号 形式であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便（区）番号 形式である。
        /// false：郵便（区）番号 形式でない。
        /// </returns>
        public static bool IsJpZipCode(string input)
        {
            // 郵便（区）番号 形式
            return (FormatChecker.IsJpZipCode_Hyphen(input)
                | FormatChecker.IsJpZipCode_NoHyphen(input));
        }

        /// <summary>
        /// 指定された文字列が郵便（区）番号
        /// （日本, ハイフン有り）形式であるか。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便（区）番号（日本, ハイフン有り）形式である。
        /// false：郵便（区）番号（日本, ハイフン有り）形式でない。
        /// </returns>
        public static bool IsJpZipCode_Hyphen(string input)
        {
            // 郵便（区）番号（日本, ハイフン有り）形式
            return (FormatChecker.IsJpZipCode7_Hyphen(input) 
                | FormatChecker.IsJpZipCode5_Hyphen(input));
        }

        /// <summary>
        /// 指定された文字列が郵便 番号
        /// （日本, ハイフン無し）形式であるか。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便（区）番号（日本, ハイフン無し）形式である。
        /// false：郵便（区）番号（日本, ハイフン無し）形式でない。
        /// </returns>
        public static bool IsJpZipCode_NoHyphen(string input)
        {
            // 郵便（区）番号（日本, ハイフン無し）形式
            return (FormatChecker.IsJpZipCode7_NoHyphen(input) 
                | FormatChecker.IsJpZipCode5_NoHyphen(input));
        }

        #endregion

        #region 郵便 番号

        /// <summary>
        /// 指定された文字列が郵便 番号 形式であるか。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便 番号 形式である。
        /// false：郵便 番号 形式でない。
        /// </returns>
        public static bool IsJpZipCode7(string input)
        {
            // 先頭xxx-xxxx末尾（xは10進数）
            // 先頭xxxxxxx末尾（xは10進数）

            return (FormatChecker.IsJpZipCode7_Hyphen(input)
                | FormatChecker.IsJpZipCode7_NoHyphen(input));
        }

        /// <summary>
        /// 指定された文字列が郵便 番号
        /// （日本, ハイフン有り）形式であるか。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便 番号（日本, ハイフン有り）形式である。
        /// false：郵便 番号（日本, ハイフン有り）形式でない。
        /// </returns>
        public static bool IsJpZipCode7_Hyphen(string input)
        {
            // 先頭xxx-xxxx末尾（xは10進数）

            // return StringChecker.Match(input, "^[0-9]{3}[-][0-9]{4}$");
            return StringChecker.Match(input, @"^\d{3}[-]\d{4}$");
        }

        /// <summary>
        /// 指定された文字列が郵便 番号
        /// （日本, ハイフン無し）形式であるか。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便 番号（日本, ハイフン無し）形式である。
        /// false：郵便 番号（日本, ハイフン無し）形式でない。
        /// </returns>
        public static bool IsJpZipCode7_NoHyphen(string input)
        {
            // 先頭xxxxxxx末尾（xは10進数）

            // return StringChecker.Match(input, "^[0-9]{7}$");
            return StringChecker.Match(input, "^\\d{7}$");
        }

        #endregion

        #region 郵便 区 番号

        /// <summary>指定された文字列が郵便 区 番号 形式であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便 区 番号 形式である。
        /// false：郵便 区 番号 形式でない。
        /// </returns>
        public static bool IsJpZipCode5(string input)
        {
            // 先頭xxx末尾（xは10進数）
            // 先頭xxx-xx末尾（xは10進数）

            // 先頭xxx末尾（xは10進数）
            // 先頭xxxxx末尾（xは10進数）

            return (FormatChecker.IsJpZipCode5_Hyphen(input)
                | FormatChecker.IsJpZipCode5_NoHyphen(input));
        }

        /// <summary>
        /// 指定された文字列が郵便 区 番号
        /// （日本, ハイフン有り）形式であるか。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便 区 番号（日本, ハイフン有り）形式である。
        /// false：郵便 区 番号（日本, ハイフン有り）形式でない。
        /// </returns>
        public static bool IsJpZipCode5_Hyphen(string input)
        {
            // 先頭xxx末尾（xは10進数）
            // 先頭xxx-xx末尾（xは10進数）
            
            return (StringChecker.Match(input, @"^\d{3}$") 
                | StringChecker.Match(input, @"^\d{3}[-]\d{2}$"));
        }

        /// <summary>
        /// 指定された文字列が郵便 区 番号
        /// （日本, ハイフン無し）形式であるか。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：郵便 区 番号（日本, ハイフン無し）形式である。
        /// false：郵便 区 番号（日本, ハイフン無し）形式でない。
        /// </returns>
        public static bool IsJpZipCode5_NoHyphen(string input)
        {
            // 先頭xxx末尾（xは10進数）
            // 先頭xxxxx末尾（xは10進数）
            
            return (StringChecker.Match(input, "^\\d{3}$") 
                | StringChecker.Match(input, "^\\d{5}$"));
        }

        #endregion

        #endregion

        #region 電話番号（日本）チェック

        #region 電話番号（日本）チェック

        /// <summary>指定された文字列が電話番号（日本）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：電話番号（日本）形式である。
        /// false：電話番号（日本）形式でない。
        /// </returns>
        public static bool IsJpTelephoneNumber(string input)
        {
            // 電話番号（日本）形式
            return (
                FormatChecker.IsJpFixedLinePhoneNumber(input)
                | FormatChecker.IsJpCellularPhoneNumber(input)
                | FormatChecker.IsJpIpPhoneNumber(input));
        }

        /// <summary>指定された文字列が電話番号（日本, ハイフン有り）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：電話番号（日本, ハイフン有り）である。
        /// false：電話番号（日本, ハイフン有り）でない。
        /// </returns>
        public static bool IsJpTelephoneNumber_Hyphen(string input)
        {
            // 電話番号（日本, ハイフン有り）形式
            return (
                FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(input)
                | FormatChecker.IsJpCellularPhoneNumber_Hyphen(input)
                | FormatChecker.IsJpIpPhoneNumber_Hyphen(input));
        }

        /// <summary>指定された文字列が電話番号（日本, ハイフン無し）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：電話番号（日本, ハイフン無し）である。
        /// false：電話番号（日本, ハイフン無し）でない。
        /// </returns>
        public static bool IsJpTelephoneNumber_NoHyphen(string input)
        {
            // 電話番号（日本, ハイフン無し）形式
            return (
                FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(input)
                | FormatChecker.IsJpCellularPhoneNumber_NoHyphen(input)
                | FormatChecker.IsJpIpPhoneNumber_NoHyphen(input));
        }

        #endregion

        #region 固定電話番号（日本）チェック

        /// <summary>指定された文字列が固定電話番号（日本）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：固定電話番号（日本）形式である。
        /// false：固定電話番号（日本）形式でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況－固定電話の電話番号
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/fixed.html
        /// ※ FMC（Fixed Mobile Convergence）は含まず。
        /// </remarks>
        public static bool IsJpFixedLinePhoneNumber(string input)
        {
            // 固定電話番号（日本）形式
            return (FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(input)
                | FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(input));
        }

        /// <summary>指定された文字列が固定電話番号（日本, ハイフン有り）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：固定電話番号（日本, ハイフン有り）である。
        /// false：固定電話番号（日本, ハイフン有り）でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況－固定電話の電話番号
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/fixed.html
        /// ※ FMC（Fixed Mobile Convergence）は含まず。
        /// </remarks>
        public static bool IsJpFixedLinePhoneNumber_Hyphen(string input)
        {
            // 先頭0x(1～4個)-x(1～4個)-x(4個)末尾（xは10進数）
            // そして、全体が10桁（ハイフン入れて12文字）であること。
            if (input.Length == 12)
            {
                return StringChecker.Match(input, "^0\\d{1,4}-\\d{1,4}-\\d{4}$");
            }

            return false;
        }

        /// <summary>指定された文字列が固定電話番号（日本, ハイフン無し）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：固定電話番号（日本, ハイフン無し）である。
        /// false：固定電話番号（日本, ハイフン無し）でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況－固定電話の電話番号
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/fixed.html
        /// ※ FMC（Fixed Mobile Convergence）は含まず。
        /// </remarks>
        public static bool IsJpFixedLinePhoneNumber_NoHyphen(string input)
        {
            // 先頭0x(1～4個)x(1～4個)x(4個)末尾（xは10進数）
            // そして、全体が10桁（ハイフンが無いので10文字）であること。
            return StringChecker.Match(input, "^0\\d{9}$");
        }

        #endregion

        #region 携帯電話番号（日本）チェック

        /// <summary>指定された文字列が携帯電話番号（日本）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：携帯電話番号（日本）形式である。
        /// false：携帯電話番号（日本）形式でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/number_shitei.html
        /// 電気通信サービスＦＡＱ（よくある質問）－６．電話番号について
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/d_faq/6Telephonenumber.htm
        /// ※ 020はポケベル、060はFMC、070はPHS、080・090は携帯、11桁。
        /// </remarks>
        public static bool IsJpCellularPhoneNumber(string input)
        {
            // 携帯定電話番号（日本）形式
            return (FormatChecker.IsJpCellularPhoneNumber_Hyphen(input)
                | FormatChecker.IsJpCellularPhoneNumber_NoHyphen(input));
        }

        /// <summary>指定された文字列が携帯電話番号（日本, ハイフン有り）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：携帯電話番号（日本, ハイフン有り）である。
        /// false：携帯電話番号（日本, ハイフン有り）でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/number_shitei.html
        /// 電気通信サービスＦＡＱ（よくある質問）－６．電話番号について
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/d_faq/6Telephonenumber.htm
        /// ※ 020はポケベル、060はFMC、070はPHS、080・090は携帯、11桁。
        /// </remarks>
        public static bool IsJpCellularPhoneNumber_Hyphen(string input)
        {
            // 先頭0n0-x(4個)-x(4個)末尾（nは2 or 6 or 7 or 8 or 9、xは10進数）
            // そして、全体が11桁（ハイフン入れて13文字）であること。
            return StringChecker.Match(input, "^0[26789]0-\\d{4}-\\d{4}$");
        }

        /// <summary>指定された文字列が携帯電話番号（日本, ハイフン無し）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：携帯電話番号（日本, ハイフン無し）である。
        /// false：携帯電話番号（日本, ハイフン無し）でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/number_shitei.html
        /// 電気通信サービスＦＡＱ（よくある質問）－６．電話番号について
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/d_faq/6Telephonenumber.htm
        /// ※ 020はポケベル、060はFMC、070はPHS、080・090は携帯、11桁。
        /// </remarks>
        public static bool IsJpCellularPhoneNumber_NoHyphen(string input)
        {
            // 先頭0n0x(8個)末尾（nは2 or 6 or 7 or 8 or 9、xは10進数）
            // そして、全体が11桁（ハイフンが無いので11文字）であること。
            return StringChecker.Match(input, "^0[26789]0\\d{8}$");
        }

        #endregion

        #region IP電話番号（日本）チェック

        /// <summary>指定された文字列がIP電話番号（日本）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：IP電話番号（日本）形式である。
        /// false：IP電話番号（日本）形式でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/number_shitei.html
        /// 電気通信サービスＦＡＱ（よくある質問）－６．電話番号について
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/d_faq/6Telephonenumber.htm
        /// ※ IP電話は050から始まる11桁。
        /// </remarks>
        public static bool IsJpIpPhoneNumber(string input)
        {
            // IP電話番号（日本）形式
            return (FormatChecker.IsJpIpPhoneNumber_Hyphen(input)
                | FormatChecker.IsJpIpPhoneNumber_NoHyphen(input));
        }

        /// <summary>指定された文字列がIP電話番号（日本, ハイフン有り）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：IP電話番号（日本, ハイフン有り）である。
        /// false：IP電話番号（日本, ハイフン有り）でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/number_shitei.html
        /// 電気通信サービスＦＡＱ（よくある質問）－６．電話番号について
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/d_faq/6Telephonenumber.htm
        /// ※ IP電話は050から始まる11桁。
        /// </remarks>
        public static bool IsJpIpPhoneNumber_Hyphen(string input)
        {
            // 先頭050-x(4個)-x(4個)末尾（xは10進数）
            // そして、全体が11桁（ハイフン入れて13文字）であること。
            return StringChecker.Match(input, "^050-\\d{4}-\\d{4}$");
        }

        /// <summary>指定された文字列がIP電話番号（日本, ハイフン無し）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：IP電話番号（日本, ハイフン無し）である。
        /// false：IP電話番号（日本, ハイフン無し）でない。
        /// </returns>
        /// <remarks>
        /// ＜総務省＞
        /// 電話番号に関するＱ＆Ａ
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/q_and_a-2001aug.html
        /// 電気通信番号指定状況
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/top/tel_number/number_shitei.html
        /// 電気通信サービスＦＡＱ（よくある質問）－６．電話番号について
        /// http://www.soumu.go.jp/main_sosiki/joho_tsusin/d_faq/6Telephonenumber.htm
        /// ※ IP電話は050から始まる11桁。
        /// </remarks>
        public static bool IsJpIpPhoneNumber_NoHyphen(string input)
        {
            // 先頭050x(8個)末尾（xは10進数）
            // そして、全体が11桁（ハイフンが無いので11文字）であること。
            return StringChecker.Match(input, "^050\\d{8}$");
        }

        #endregion

        #endregion
    }
}
