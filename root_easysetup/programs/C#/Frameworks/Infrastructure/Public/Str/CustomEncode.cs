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
//* クラス名        ：CustomEncode
//* クラス日本語名  ：エンコード処理のユーティリティクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2011/05/27  西野  大介        HtmlDecode、UrlDecode、ToBase64String、FromBase64Stringを追加
//*  2013/02/12  西野  大介        ToHexString、FormHexStringを追加
//**********************************************************************************

// System
using System;
using System.Text;
using System.Collections;

using System.Data;

using System.Web;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>エンコード処理のユーティリティクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class CustomEncode
    {
        #region 定数

        #region JIS

        /// <summary>日本語 (シフト JIS)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int shift_jis = 932;

        /// <summary>日本語 (JIS)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int ISO_2022_JP = 50220;

        /// <summary>日本語 (JIS-Allow 1 byte Kana)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int _iso_2022_jp_Dollar_ESC = 50221;

        /// <summary>日本語 (JIS-Allow 1 byte Kana - SO/SI)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int _iso_2022_jp_Dollar_SIO = 50222;

        #endregion

        #region Unicode

        /// <summary>Unicode  (UTF-16)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int UTF_16LE = 1200;

        /// <summary>Unicode (unicodeFFFE)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int UTF_16BE = 1201;

        /// <summary>Unicode (UTF-7)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int UTF_7 = 65000;

        /// <summary>Unicode (UTF-8)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int UTF_8 = 65001;

        #endregion

        #region その他

        /// <summary>日本語 (Mac)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int x_mac_japanese = 10001;

        /// <summary>US_ASCII</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int us_ascii = 20127;

        /// <summary>日本語 (EUC)</summary>
        /// <remarks>自由に利用できる。</remarks>
        public const int EUC_JP = 51932;

        #endregion 

        #endregion

        #region メソッド

        #region コードページ

        /// <summary>EncodingのDataTable返す。</summary>
        /// <returns>EncodingのDataTable</returns>
        /// <remarks>コンボにバインドするため</remarks>
        public static DataTable GetEncodings()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("key", typeof(int));
            dt.Columns.Add("value", typeof(string));

            DataRow dr = null;
            
            // 以下のコードで出力（.NET4.0を使用

            //for (int i = 0; i < 65535; i++)
            //{
            //    try
            //    {
            //        Encoding enc = Encoding.GetEncoding(i);
            //
            //        Console.WriteLine(" // " + enc.EncodingName);
            //        Console.WriteLine("dr = dt.NewRow();");
            //        Console.WriteLine(string.Format("dr[\"key\"] = {0};", i.ToString()));
            //        Console.WriteLine(string.Format("dr[\"value\"] = \"{0}\";", enc.WebName));
            //        Console.WriteLine("dt.Rows.Add(dr);");
            //
            //        Console.WriteLine();
            //    }
            //    catch { }
            //}

            // 日本語 (シフト JIS)
            dr = dt.NewRow();
            dr["key"] = 0;
            dr["value"] = "shift_jis";
            dt.Rows.Add(dr);

            // IBM EBCDIC (US - カナダ)
            dr = dt.NewRow();
            dr["key"] = 37;
            dr["value"] = "IBM037";
            dt.Rows.Add(dr);

            // OEM アメリカ合衆国
            dr = dt.NewRow();
            dr["key"] = 437;
            dr["value"] = "IBM437";
            dt.Rows.Add(dr);

            // IBM EBCDIC (インターナショナル)
            dr = dt.NewRow();
            dr["key"] = 500;
            dr["value"] = "IBM500";
            dt.Rows.Add(dr);

            // アラビア語 (ASMO 708)
            dr = dt.NewRow();
            dr["key"] = 708;
            dr["value"] = "ASMO-708";
            dt.Rows.Add(dr);

            // アラビア語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 720;
            dr["value"] = "DOS-720";
            dt.Rows.Add(dr);

            // ギリシャ語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 737;
            dr["value"] = "ibm737";
            dt.Rows.Add(dr);

            // バルト言語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 775;
            dr["value"] = "ibm775";
            dt.Rows.Add(dr);

            // 西ヨーロッパ言語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 850;
            dr["value"] = "ibm850";
            dt.Rows.Add(dr);

            // 中央ヨーロッパ言語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 852;
            dr["value"] = "ibm852";
            dt.Rows.Add(dr);

            // OEM キリル
            dr = dt.NewRow();
            dr["key"] = 855;
            dr["value"] = "IBM855";
            dt.Rows.Add(dr);

            // トルコ語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 857;
            dr["value"] = "ibm857";
            dt.Rows.Add(dr);

            // OEM マルチリンガル ラテン I
            dr = dt.NewRow();
            dr["key"] = 858;
            dr["value"] = "IBM00858";
            dt.Rows.Add(dr);

            // ポルトガル語  (DOS)
            dr = dt.NewRow();
            dr["key"] = 860;
            dr["value"] = "IBM860";
            dt.Rows.Add(dr);

            // アイスランド語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 861;
            dr["value"] = "ibm861";
            dt.Rows.Add(dr);

            // ヘブライ語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 862;
            dr["value"] = "DOS-862";
            dt.Rows.Add(dr);

            // フランス語 (カナダ) (DOS)
            dr = dt.NewRow();
            dr["key"] = 863;
            dr["value"] = "IBM863";
            dt.Rows.Add(dr);

            // アラビア語 (864)
            dr = dt.NewRow();
            dr["key"] = 864;
            dr["value"] = "IBM864";
            dt.Rows.Add(dr);

            // 北欧 (DOS)
            dr = dt.NewRow();
            dr["key"] = 865;
            dr["value"] = "IBM865";
            dt.Rows.Add(dr);

            // キリル言語 (DOS)
            dr = dt.NewRow();
            dr["key"] = 866;
            dr["value"] = "cp866";
            dt.Rows.Add(dr);

            // ギリシャ語, Modern (DOS)
            dr = dt.NewRow();
            dr["key"] = 869;
            dr["value"] = "ibm869";
            dt.Rows.Add(dr);

            // IBM EBCDIC (多国語ラテン 2)
            dr = dt.NewRow();
            dr["key"] = 870;
            dr["value"] = "IBM870";
            dt.Rows.Add(dr);

            // タイ語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 874;
            dr["value"] = "windows-874";
            dt.Rows.Add(dr);

            // IBM EBCDIC (ギリシャ語 Modern)
            dr = dt.NewRow();
            dr["key"] = 875;
            dr["value"] = "cp875";
            dt.Rows.Add(dr);

            // 日本語 (シフト JIS)
            dr = dt.NewRow();
            dr["key"] = 932;
            dr["value"] = "shift_jis";
            dt.Rows.Add(dr);

            // 簡体字中国語 (GB2312)
            dr = dt.NewRow();
            dr["key"] = 936;
            dr["value"] = "gb2312";
            dt.Rows.Add(dr);

            // 韓国語
            dr = dt.NewRow();
            dr["key"] = 949;
            dr["value"] = "ks_c_5601-1987";
            dt.Rows.Add(dr);

            // 繁体字中国語 (Big5)
            dr = dt.NewRow();
            dr["key"] = 950;
            dr["value"] = "big5";
            dt.Rows.Add(dr);

            // IBM EBCDIC (トルコ語ラテン 5)
            dr = dt.NewRow();
            dr["key"] = 1026;
            dr["value"] = "IBM1026";
            dt.Rows.Add(dr);

            // IBM ラテン-1
            dr = dt.NewRow();
            dr["key"] = 1047;
            dr["value"] = "IBM01047";
            dt.Rows.Add(dr);

            // IBM EBCDIC (US - カナダ - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1140;
            dr["value"] = "IBM01140";
            dt.Rows.Add(dr);

            // IBM EBCDIC (ドイツ - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1141;
            dr["value"] = "IBM01141";
            dt.Rows.Add(dr);

            // IBM EBCDIC (デンマーク - ノルウェー - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1142;
            dr["value"] = "IBM01142";
            dt.Rows.Add(dr);

            // IBM EBCDIC (フィンランド - スウェーデン - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1143;
            dr["value"] = "IBM01143";
            dt.Rows.Add(dr);

            // IBM EBCDIC (イタリア - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1144;
            dr["value"] = "IBM01144";
            dt.Rows.Add(dr);

            // IBM EBCDIC (スペイン - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1145;
            dr["value"] = "IBM01145";
            dt.Rows.Add(dr);

            // IBM EBCDIC (UK - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1146;
            dr["value"] = "IBM01146";
            dt.Rows.Add(dr);

            // IBM EBCDIC (フランス - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1147;
            dr["value"] = "IBM01147";
            dt.Rows.Add(dr);

            // IBM EBCDIC (インターナショナル - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1148;
            dr["value"] = "IBM01148";
            dt.Rows.Add(dr);

            // IBM EBCDIC (アイスランド語 - ヨーロッパ)
            dr = dt.NewRow();
            dr["key"] = 1149;
            dr["value"] = "IBM01149";
            dt.Rows.Add(dr);

            // Unicode
            dr = dt.NewRow();
            dr["key"] = 1200;
            dr["value"] = "utf-16";
            dt.Rows.Add(dr);

            // Unicode (Big-Endian)
            dr = dt.NewRow();
            dr["key"] = 1201;
            dr["value"] = "unicodeFFFE";
            dt.Rows.Add(dr);

            // 中央ヨーロッパ言語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1250;
            dr["value"] = "windows-1250";
            dt.Rows.Add(dr);

            // キリル言語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1251;
            dr["value"] = "windows-1251";
            dt.Rows.Add(dr);

            // 西ヨーロッパ言語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1252;
            dr["value"] = "Windows-1252";
            dt.Rows.Add(dr);

            // ギリシャ語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1253;
            dr["value"] = "windows-1253";
            dt.Rows.Add(dr);

            // トルコ語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1254;
            dr["value"] = "windows-1254";
            dt.Rows.Add(dr);

            // ヘブライ語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1255;
            dr["value"] = "windows-1255";
            dt.Rows.Add(dr);

            // アラビア語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1256;
            dr["value"] = "windows-1256";
            dt.Rows.Add(dr);

            // バルト言語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1257;
            dr["value"] = "windows-1257";
            dt.Rows.Add(dr);

            // ベトナム語 (Windows)
            dr = dt.NewRow();
            dr["key"] = 1258;
            dr["value"] = "windows-1258";
            dt.Rows.Add(dr);

            // 韓国語 (Johab)
            dr = dt.NewRow();
            dr["key"] = 1361;
            dr["value"] = "Johab";
            dt.Rows.Add(dr);

            // 西ヨーロッパ言語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10000;
            dr["value"] = "macintosh";
            dt.Rows.Add(dr);

            // 日本語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10001;
            dr["value"] = "x-mac-japanese";
            dt.Rows.Add(dr);

            // 繁体字中国語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10002;
            dr["value"] = "x-mac-chinesetrad";
            dt.Rows.Add(dr);

            // 韓国語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10003;
            dr["value"] = "x-mac-korean";
            dt.Rows.Add(dr);

            // アラビア語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10004;
            dr["value"] = "x-mac-arabic";
            dt.Rows.Add(dr);

            // ヘブライ語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10005;
            dr["value"] = "x-mac-hebrew";
            dt.Rows.Add(dr);

            // ギリシャ語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10006;
            dr["value"] = "x-mac-greek";
            dt.Rows.Add(dr);

            // キリル言語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10007;
            dr["value"] = "x-mac-cyrillic";
            dt.Rows.Add(dr);

            // 簡体字中国語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10008;
            dr["value"] = "x-mac-chinesesimp";
            dt.Rows.Add(dr);

            // ルーマニア語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10010;
            dr["value"] = "x-mac-romanian";
            dt.Rows.Add(dr);

            // ウクライナ語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10017;
            dr["value"] = "x-mac-ukrainian";
            dt.Rows.Add(dr);

            // タイ語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10021;
            dr["value"] = "x-mac-thai";
            dt.Rows.Add(dr);

            // 中央ヨーロッパ言語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10029;
            dr["value"] = "x-mac-ce";
            dt.Rows.Add(dr);

            // アイスランド語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10079;
            dr["value"] = "x-mac-icelandic";
            dt.Rows.Add(dr);

            // トルコ語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10081;
            dr["value"] = "x-mac-turkish";
            dt.Rows.Add(dr);

            // クロアチア語 (Mac)
            dr = dt.NewRow();
            dr["key"] = 10082;
            dr["value"] = "x-mac-croatian";
            dt.Rows.Add(dr);

            // Unicode (UTF-32)
            dr = dt.NewRow();
            dr["key"] = 12000;
            dr["value"] = "utf-32";
            dt.Rows.Add(dr);

            // Unicode (UTF-32 ビッグ エンディアン)
            dr = dt.NewRow();
            dr["key"] = 12001;
            dr["value"] = "utf-32BE";
            dt.Rows.Add(dr);

            // 繁体字中国語 (CNS)
            dr = dt.NewRow();
            dr["key"] = 20000;
            dr["value"] = "x-Chinese-CNS";
            dt.Rows.Add(dr);

            // TCA 台湾
            dr = dt.NewRow();
            dr["key"] = 20001;
            dr["value"] = "x-cp20001";
            dt.Rows.Add(dr);

            // 繁体字中国語 (Eten)
            dr = dt.NewRow();
            dr["key"] = 20002;
            dr["value"] = "x-Chinese-Eten";
            dt.Rows.Add(dr);

            // IBM5550 台湾
            dr = dt.NewRow();
            dr["key"] = 20003;
            dr["value"] = "x-cp20003";
            dt.Rows.Add(dr);

            // TeleText 台湾
            dr = dt.NewRow();
            dr["key"] = 20004;
            dr["value"] = "x-cp20004";
            dt.Rows.Add(dr);

            // Wang 台湾
            dr = dt.NewRow();
            dr["key"] = 20005;
            dr["value"] = "x-cp20005";
            dt.Rows.Add(dr);

            // 西ヨーロッパ言語 (IA5)
            dr = dt.NewRow();
            dr["key"] = 20105;
            dr["value"] = "x-IA5";
            dt.Rows.Add(dr);

            // ドイツ語 (IA5)
            dr = dt.NewRow();
            dr["key"] = 20106;
            dr["value"] = "x-IA5-German";
            dt.Rows.Add(dr);

            // スウェーデン語 (IA5)
            dr = dt.NewRow();
            dr["key"] = 20107;
            dr["value"] = "x-IA5-Swedish";
            dt.Rows.Add(dr);

            // ノルウェー語 (IA5)
            dr = dt.NewRow();
            dr["key"] = 20108;
            dr["value"] = "x-IA5-Norwegian";
            dt.Rows.Add(dr);

            // US-ASCII
            dr = dt.NewRow();
            dr["key"] = 20127;
            dr["value"] = "us-ascii";
            dt.Rows.Add(dr);

            // T.61
            dr = dt.NewRow();
            dr["key"] = 20261;
            dr["value"] = "x-cp20261";
            dt.Rows.Add(dr);

            // ISO-6937
            dr = dt.NewRow();
            dr["key"] = 20269;
            dr["value"] = "x-cp20269";
            dt.Rows.Add(dr);

            // IBM EBCDIC (ドイツ)
            dr = dt.NewRow();
            dr["key"] = 20273;
            dr["value"] = "IBM273";
            dt.Rows.Add(dr);

            // IBM EBCDIC (デンマーク - ノルウェー)
            dr = dt.NewRow();
            dr["key"] = 20277;
            dr["value"] = "IBM277";
            dt.Rows.Add(dr);

            // IBM EBCDIC (フィンランド - スウェーデン)
            dr = dt.NewRow();
            dr["key"] = 20278;
            dr["value"] = "IBM278";
            dt.Rows.Add(dr);

            // IBM EBCDIC (イタリア)
            dr = dt.NewRow();
            dr["key"] = 20280;
            dr["value"] = "IBM280";
            dt.Rows.Add(dr);

            // IBM EBCDIC (スペイン)
            dr = dt.NewRow();
            dr["key"] = 20284;
            dr["value"] = "IBM284";
            dt.Rows.Add(dr);

            // IBM EBCDIC (UK)
            dr = dt.NewRow();
            dr["key"] = 20285;
            dr["value"] = "IBM285";
            dt.Rows.Add(dr);

            // IBM EBCDIC (日本語カタカナ)
            dr = dt.NewRow();
            dr["key"] = 20290;
            dr["value"] = "IBM290";
            dt.Rows.Add(dr);

            // IBM EBCDIC (フランス)
            dr = dt.NewRow();
            dr["key"] = 20297;
            dr["value"] = "IBM297";
            dt.Rows.Add(dr);

            // IBM EBCDIC (アラビア語)
            dr = dt.NewRow();
            dr["key"] = 20420;
            dr["value"] = "IBM420";
            dt.Rows.Add(dr);

            // IBM EBCDIC (ギリシャ語)
            dr = dt.NewRow();
            dr["key"] = 20423;
            dr["value"] = "IBM423";
            dt.Rows.Add(dr);

            // IBM EBCDIC (ヘブライ語)
            dr = dt.NewRow();
            dr["key"] = 20424;
            dr["value"] = "IBM424";
            dt.Rows.Add(dr);

            // IBM EBCDIC (韓国語 Extended)
            dr = dt.NewRow();
            dr["key"] = 20833;
            dr["value"] = "x-EBCDIC-KoreanExtended";
            dt.Rows.Add(dr);

            // IBM EBCDIC (タイ語)
            dr = dt.NewRow();
            dr["key"] = 20838;
            dr["value"] = "IBM-Thai";
            dt.Rows.Add(dr);

            // キリル言語 (KOI8-R)
            dr = dt.NewRow();
            dr["key"] = 20866;
            dr["value"] = "koi8-r";
            dt.Rows.Add(dr);

            // IBM EBCDIC (アイスランド語)
            dr = dt.NewRow();
            dr["key"] = 20871;
            dr["value"] = "IBM871";
            dt.Rows.Add(dr);

            // IBM EBCDIC (キリル言語 - ロシア語)
            dr = dt.NewRow();
            dr["key"] = 20880;
            dr["value"] = "IBM880";
            dt.Rows.Add(dr);

            // IBM EBCDIC (トルコ語)
            dr = dt.NewRow();
            dr["key"] = 20905;
            dr["value"] = "IBM905";
            dt.Rows.Add(dr);

            // IBM ラテン-1
            dr = dt.NewRow();
            dr["key"] = 20924;
            dr["value"] = "IBM00924";
            dt.Rows.Add(dr);

            // 日本語 (JIS 0208-1990 および 0212-1990)
            dr = dt.NewRow();
            dr["key"] = 20932;
            dr["value"] = "EUC-JP";
            dt.Rows.Add(dr);

            // 簡体字中国語 (GB2312-80)
            dr = dt.NewRow();
            dr["key"] = 20936;
            dr["value"] = "x-cp20936";
            dt.Rows.Add(dr);

            // 韓国語 Wansung
            dr = dt.NewRow();
            dr["key"] = 20949;
            dr["value"] = "x-cp20949";
            dt.Rows.Add(dr);

            // IBM EBCDIC (キリル言語 セルビア - ブルガリア)
            dr = dt.NewRow();
            dr["key"] = 21025;
            dr["value"] = "cp1025";
            dt.Rows.Add(dr);

            // キリル言語 (KOI8-U)
            dr = dt.NewRow();
            dr["key"] = 21866;
            dr["value"] = "koi8-u";
            dt.Rows.Add(dr);

            // 西ヨーロッパ言語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28591;
            dr["value"] = "iso-8859-1";
            dt.Rows.Add(dr);

            // 中央ヨーロッパ言語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28592;
            dr["value"] = "iso-8859-2";
            dt.Rows.Add(dr);

            // ラテン 3 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28593;
            dr["value"] = "iso-8859-3";
            dt.Rows.Add(dr);

            // バルト言語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28594;
            dr["value"] = "iso-8859-4";
            dt.Rows.Add(dr);

            // キリル言語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28595;
            dr["value"] = "iso-8859-5";
            dt.Rows.Add(dr);

            // アラビア語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28596;
            dr["value"] = "iso-8859-6";
            dt.Rows.Add(dr);

            // ギリシャ語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28597;
            dr["value"] = "iso-8859-7";
            dt.Rows.Add(dr);

            // ヘブライ語 (ISO-Visual)
            dr = dt.NewRow();
            dr["key"] = 28598;
            dr["value"] = "iso-8859-8";
            dt.Rows.Add(dr);

            // トルコ語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28599;
            dr["value"] = "iso-8859-9";
            dt.Rows.Add(dr);

            // エストニア語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28603;
            dr["value"] = "iso-8859-13";
            dt.Rows.Add(dr);

            // ラテン 9 (ISO)
            dr = dt.NewRow();
            dr["key"] = 28605;
            dr["value"] = "iso-8859-15";
            dt.Rows.Add(dr);

            // ヨーロッパ
            dr = dt.NewRow();
            dr["key"] = 29001;
            dr["value"] = "x-Europa";
            dt.Rows.Add(dr);

            // ヘブライ語 (ISO-Logical)
            dr = dt.NewRow();
            dr["key"] = 38598;
            dr["value"] = "iso-8859-8-i";
            dt.Rows.Add(dr);

            // 日本語 (JIS)
            dr = dt.NewRow();
            dr["key"] = 50220;
            dr["value"] = "iso-2022-jp";
            dt.Rows.Add(dr);

            // 日本語 (JIS 1 バイト カタカナ可)
            dr = dt.NewRow();
            dr["key"] = 50221;
            dr["value"] = "csISO2022JP";
            dt.Rows.Add(dr);

            // 日本語 (JIS 1 バイト カタカナ可 - SO/SI)
            dr = dt.NewRow();
            dr["key"] = 50222;
            dr["value"] = "iso-2022-jp";
            dt.Rows.Add(dr);

            // 韓国語 (ISO)
            dr = dt.NewRow();
            dr["key"] = 50225;
            dr["value"] = "iso-2022-kr";
            dt.Rows.Add(dr);

            // 簡体字中国語 (ISO-2022)
            dr = dt.NewRow();
            dr["key"] = 50227;
            dr["value"] = "x-cp50227";
            dt.Rows.Add(dr);

            // 日本語 (EUC)
            dr = dt.NewRow();
            dr["key"] = 51932;
            dr["value"] = "euc-jp";
            dt.Rows.Add(dr);

            // 簡体字中国語 (EUC)
            dr = dt.NewRow();
            dr["key"] = 51936;
            dr["value"] = "EUC-CN";
            dt.Rows.Add(dr);

            // 韓国語 (EUC)
            dr = dt.NewRow();
            dr["key"] = 51949;
            dr["value"] = "euc-kr";
            dt.Rows.Add(dr);

            // 簡体字中国語 (HZ)
            dr = dt.NewRow();
            dr["key"] = 52936;
            dr["value"] = "hz-gb-2312";
            dt.Rows.Add(dr);

            // 簡体字中国語 (GB18030)
            dr = dt.NewRow();
            dr["key"] = 54936;
            dr["value"] = "GB18030";
            dt.Rows.Add(dr);

            // ISCII デバナガリ文字
            dr = dt.NewRow();
            dr["key"] = 57002;
            dr["value"] = "x-iscii-de";
            dt.Rows.Add(dr);

            // ISCII ベンガル語
            dr = dt.NewRow();
            dr["key"] = 57003;
            dr["value"] = "x-iscii-be";
            dt.Rows.Add(dr);

            // ISCII タミール語
            dr = dt.NewRow();
            dr["key"] = 57004;
            dr["value"] = "x-iscii-ta";
            dt.Rows.Add(dr);

            // ISCII テルグ語
            dr = dt.NewRow();
            dr["key"] = 57005;
            dr["value"] = "x-iscii-te";
            dt.Rows.Add(dr);

            // ISCII アッサム語
            dr = dt.NewRow();
            dr["key"] = 57006;
            dr["value"] = "x-iscii-as";
            dt.Rows.Add(dr);

            // ISCII オリヤー語
            dr = dt.NewRow();
            dr["key"] = 57007;
            dr["value"] = "x-iscii-or";
            dt.Rows.Add(dr);

            // ISCII カナラ語
            dr = dt.NewRow();
            dr["key"] = 57008;
            dr["value"] = "x-iscii-ka";
            dt.Rows.Add(dr);

            // ISCII マラヤラム語
            dr = dt.NewRow();
            dr["key"] = 57009;
            dr["value"] = "x-iscii-ma";
            dt.Rows.Add(dr);

            // ISCII グジャラート語
            dr = dt.NewRow();
            dr["key"] = 57010;
            dr["value"] = "x-iscii-gu";
            dt.Rows.Add(dr);

            // ISCII パンジャブ語
            dr = dt.NewRow();
            dr["key"] = 57011;
            dr["value"] = "x-iscii-pa";
            dt.Rows.Add(dr);

            // Unicode (UTF-7)
            dr = dt.NewRow();
            dr["key"] = 65000;
            dr["value"] = "utf-7";
            dt.Rows.Add(dr);

            // Unicode (UTF-8)
            dr = dt.NewRow();
            dr["key"] = 65001;
            dr["value"] = "utf-8";
            dt.Rows.Add(dr);

            return dt;
        }

        /// <summary>文字列をバイト配列に変換する（エンコード）</summary>
        /// <param name="str">文字列</param>
        /// <param name="codePageNum">エンコーディングに使用するコードページ（パブリック定数を使用して設定の事）</param>
        /// <returns>バイト配列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static byte[] StringToByte(string str, int codePageNum)
        {
            // エンコーディング
            Encoding enc = Encoding.GetEncoding(codePageNum);

            // エンコード
            return enc.GetBytes(str);
        }

        /// <summary>バイト配列を文字列に変換する（デコード）</summary>
        /// <param name="abyt">バイト配列</param>
        /// <param name="codePageNum">エンコーディングに使用するコードページ（パブリック定数を使用して設定の事）</param>
        /// <returns>文字列</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string ByteToString(byte[] abyt, int codePageNum)
        {
            // エンコーディング
            Encoding enc = Encoding.GetEncoding(codePageNum);

            // デコード
            return enc.GetString(abyt);
        }

        #endregion

        #region Webエンコード

        /// <summary>HTMLエンコードする。</summary>
        /// <param name="input">HTML出力する文字列</param>
        /// <returns>HTMLエンコードされた文字列</returns>
        /// <remarks>
        /// TextBoxはデフォルトでサニタイジングされているが、
        /// それ以外のWebコントロールには、必要に応じて、
        /// HTMLエンコードによるサニタイジング処理が必要になる。
        /// 
        /// また、数値文字参照をHTMLエンコードすると、二重エンコード
        /// になるため、多くの場合、数値文字参照は、正しく処理できない。
        /// </remarks>
        public static string HtmlEncode(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        /// <summary>HTMLデコードする。</summary>
        /// <param name="input">HTMLエンコードされた文字列</param>
        /// <returns>HTMLデコードされた文字列</returns>
        public static string HtmlDecode(string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        /// <summary>Urlエンコードする。</summary>
        /// <param name="input">Url</param>
        /// <returns>UrlエンコードされたUrl</returns>
        /// <remarks>
        /// URLエンコードとは
        /// http://ja.wikipedia.org/wiki/URLエンコード
        /// 
        /// 用途は、クエリ文字列の
        /// ・変数名
        /// ・変数値
        /// などに限定する。
        /// （デコードはASP.NETでは自動になるため、不要である。）
        /// </remarks>
        public static string UrlEncode(string input)
        {
            return HttpUtility.UrlEncodeUnicode(input);
        }

        /// <summary>Urlデコードする。</summary>
        /// <param name="input">UrlエンコードされたUrl</param>
        /// <returns>UrlデコードされたUrl</returns>
        public static string UrlDecode(string input)
        {
            return HttpUtility.UrlEncodeUnicode(input);
        }

        #endregion

        #region その他

        /// <summary>バイト配列をBase64文字列に変換</summary>
        /// <param name="aryByt">Base64文字列に変更するバイト配列</param>
        /// <returns>Base64文字列</returns>
        public static string ToBase64String(byte[] aryByt)
        {
            return Convert.ToBase64String(aryByt);
        }

        /// <summary>Base64文字列をバイト配列に変換</summary>
        /// <param name="base64Str">バイト配列に変換するBase64文字列</param>
        /// <returns>バイト配列</returns>
        public static byte[] FromBase64String(string base64Str)
        {
            return Convert.FromBase64String(base64Str);
        }

        /// <summary>バイト配列をHex文字列に変換</summary>
        /// <param name="bytes">Hex文字列に変換するバイト配列</param>
        /// <returns>Hex文字列</returns>
        public static string ToHexString(byte[] bytes)
        {
            string ret = "";

            foreach (byte b in bytes)
            {
                ret += b.ToString("X2") + " ";
            }

            if (0 < ret.Length)
            {
                ret = ret.Substring(0, ret.Length - 1);
            }

            return ret;
        }

        /// <summary>Hex文字列をバイト配列に変換</summary>
        /// <param name="input">バイト配列に変換するHex文字列</param>
        /// <returns>バイト配列</returns>
        public static byte[] FormHexString(string input)
        {
            string[] bs = input.Split(' ');
            byte[] ret = new byte[bs.Length];
            
            for (int i = 0; i < bs.Length; i++)
            {
                ret[i] = (byte)Convert.ToUInt32(bs[i], 16);
            }

            return ret;
        }

        #endregion

        #endregion
    }
}
