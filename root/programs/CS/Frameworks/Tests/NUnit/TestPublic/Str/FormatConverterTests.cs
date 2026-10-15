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
//* クラス名        ：FormatConverterTests
//* クラス日本語名  ：FormatConverter の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/15  玄人 幸道         #612 の修正に合わせ、修正後の挙動を確かめるケースを追加
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>FormatConverter の単体テスト</summary>
    /// <remarks>
    /// **数値の文字列化は現在のカルチャに依存する**（decimal.ToString / TryParse）。
    /// 実行環境で結果が変わらないよう、ja-JP に固定する。
    /// </remarks>
    [TestFixture]
    [SetCulture("ja-JP")]
    public class FormatConverterTests
    {
        #region 数値丸め

        /// <summary>Round_Banker は最近接偶数への丸め、Round_4sya5nyu は四捨五入。</summary>
        [TestCase("2.5", 0, "2", "3")]
        [TestCase("3.5", 0, "4", "4")]
        [TestCase("-2.5", 0, "-2", "-3")]
        [TestCase("1.125", 2, "1.12", "1.13")]
        public void Round(string number, int digits, string banker, string halfUp)
        {
            Assert.That(FormatConverter.Round_Banker(number, digits), Is.EqualTo(banker), "Round_Banker");
            Assert.That(FormatConverter.Round_4sya5nyu(number, digits), Is.EqualTo(halfUp), "Round_4sya5nyu");
        }

        /// <summary>
        /// Floor の既定は RM（負の無限大へ）、RZ は 0 へ。
        /// **小数部の桁が足りなければ 0 で埋める。**
        /// </summary>
        [TestCase("1.567", 2u, "1.56", "1.56")]
        [TestCase("-1.567", 2u, "-1.57", "-1.56")]
        [TestCase("1.9", 0u, "1", "1")]
        [TestCase("1.5", 3u, "1.500", "1.500")]
        public void Floor(string number, uint digits, string rm, string rz)
        {
            Assert.That(FormatConverter.Floor(number, digits), Is.EqualTo(rm), "既定");
            Assert.That(FormatConverter.Floor(number, digits, FloorToward.RM), Is.EqualTo(rm), "RM");
            Assert.That(FormatConverter.Floor(number, digits, FloorToward.RZ), Is.EqualTo(rz), "RZ");
        }

        /// <summary>Ceiling の既定は RP（正の無限大へ）。正の数では RI も同じ。</summary>
        [TestCase("1.561", 2u, "1.57")]
        [TestCase("1.1", 0u, "2")]
        [TestCase("2", 1u, "2.0")]
        public void Ceiling_Positive(string number, uint digits, string expected)
        {
            Assert.That(FormatConverter.Ceiling(number, digits), Is.EqualTo(expected), "既定");
            Assert.That(FormatConverter.Ceiling(number, digits, CeilingToward.RP), Is.EqualTo(expected), "RP");
            Assert.That(FormatConverter.Ceiling(number, digits, CeilingToward.RI), Is.EqualTo(expected), "RI");
        }

        /// <summary>負の数の RP は 0 の側へ寄る（正の無限大へ）。</summary>
        [Test]
        public void Ceiling_Negative_RP()
        {
            Assert.That(FormatConverter.Ceiling("-1.567", 2u), Is.EqualTo("-1.56"));
            Assert.That(FormatConverter.Ceiling("-1.567", 2u, CeilingToward.RP), Is.EqualTo("-1.56"));
        }

        /// <summary>
        /// 負の数の RI は 0 から遠ざかる（無限大へ。絶対値の切り上げ）。
        /// **以前は .NET だけ 0 の方向に丸めていた**（-1.56。#612 で修正）。
        /// </summary>
        [Test]
        public void Ceiling_Negative_RI()
        {
            Assert.That(FormatConverter.Ceiling("-1.567", 2u, CeilingToward.RI), Is.EqualTo("-1.57"));
            Assert.That(FormatConverter.Ceiling("-1.1", 0u, CeilingToward.RI), Is.EqualTo("-2"));
        }

        /// <summary>数値として解釈できないものは "0"。</summary>
        [Test]
        public void NotNumber_ReturnsZero()
        {
            Assert.That(FormatConverter.Round_Banker("abc", 0), Is.EqualTo("0"));
            Assert.That(FormatConverter.Round_4sya5nyu("abc", 0), Is.EqualTo("0"));
            Assert.That(FormatConverter.Floor("abc", 2u), Is.EqualTo("0"));
            Assert.That(FormatConverter.Ceiling("abc", 2u), Is.EqualTo("0"));
            Assert.That(FormatConverter.AddFigure3("abc"), Is.EqualTo("0"));
        }

        #endregion

        #region 数値桁区切り

        /// <summary>整数部だけを区切る。小数部はそのまま。</summary>
        [TestCase(1234567, "1,234,567")]
        [TestCase(-1234567, "-1,234,567")]
        [TestCase(999, "999")]
        public void AddFigure3_Integer(int number, string expected)
        {
            Assert.That(FormatConverter.AddFigure3(number), Is.EqualTo(expected));
        }

        /// <summary>小数を含む値。</summary>
        [Test]
        public void AddFigure3_Decimal()
        {
            Assert.That(FormatConverter.AddFigure3(-1234.5m), Is.EqualTo("-1,234.5"));
        }

        /// <summary>
        /// **文字列で渡した "n.000" は、小数部の 0 を残す。**
        /// decimal で渡すと 0 は値として消えるため、文字列のときだけ元の表記を復元する。
        /// </summary>
        [Test]
        public void AddFigure3_StringKeepsTrailingZeros()
        {
            Assert.That(FormatConverter.AddFigure3("1000.00"), Is.EqualTo("1,000.00"));
            Assert.That(FormatConverter.AddFigure3("1000"), Is.EqualTo("1,000"));
        }

        /// <summary>4 桁区切り（日本の万・億）と任意の桁。</summary>
        [Test]
        public void AddFigure4AndX()
        {
            Assert.That(FormatConverter.AddFigure4(123456789), Is.EqualTo("1,2345,6789"));
            Assert.That(FormatConverter.AddFigureX(123456, 2), Is.EqualTo("12,34,56"));
        }

        /// <summary>サプレスは左詰めの埋め。</summary>
        [Test]
        public void Suppress()
        {
            Assert.That(FormatConverter.Suppress("12", 5, '0'), Is.EqualTo("00012"));
            Assert.That(FormatConverter.Suppress("123456", 5, '0'), Is.EqualTo("123456"));
        }

        #endregion

        #region 時間

        /// <summary>UNIX 時間（秒）との相互変換。</summary>
        [Test]
        public void UnixTime_RoundTrip()
        {
            DateTime utc = new DateTime(2001, 8, 2, 10, 45, 23, DateTimeKind.Utc);

            Assert.That(FormatConverter.ToUnixTime(utc), Is.EqualTo(996749123L));
            Assert.That(FormatConverter.FromUnixTime(996749123L), Is.EqualTo(utc));
            Assert.That(FormatConverter.FromUnixTime(0).Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        /// <summary>W3C-DTF（既定の書式は秒まで、末尾 Z）。</summary>
        [Test]
        public void W3cTimestamp()
        {
            DateTime utc = new DateTime(2001, 8, 2, 10, 45, 23, DateTimeKind.Utc);

            Assert.That(FormatConverter.ToW3cTimestamp(utc), Is.EqualTo("2001-08-02T10:45:23Z"));
            Assert.That(FormatConverter.ToW3cTimestamp(utc, "yyyy-MM-dd"), Is.EqualTo("2001-08-02"));

            // 末尾の Z を落として解釈するので、時刻はそのまま（タイムゾーンの変換はしない）
            DateTime parsed = FormatConverter.FromW3cTimestamp("2001-08-02T10:45:23Z");
            Assert.That(parsed, Is.EqualTo(new DateTime(2001, 8, 2, 10, 45, 23)));
        }

        /// <summary>和暦 ⇔ 西暦。</summary>
        [Test]
        public void Wareki_RoundTrip()
        {
            DateTime seireki = new DateTime(2026, 10, 15);

            Assert.That(FormatConverter.SeirekiToWareki(seireki, "ggy年M月d日"), Is.EqualTo("令和8年10月15日"));
            Assert.That(FormatConverter.WarekiToSeireki("令和8年10月15日", "ggy年M月d日"), Is.EqualTo(seireki));
            Assert.That(FormatConverter.SeirekiToWareki(new DateTime(1989, 1, 8), "gg"), Is.EqualTo("平成"));
        }

        #endregion
    }
}
