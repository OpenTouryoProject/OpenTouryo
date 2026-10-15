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
//* クラス名        ：FormatCheckerTests
//* クラス日本語名  ：FormatChecker の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>FormatChecker の単体テスト</summary>
    [TestFixture]
    public class FormatCheckerTests
    {
        #region 郵便番号

        /// <summary>7 桁の郵便番号（ハイフンの有無）。</summary>
        [TestCase("100-0001", true, true, false)]
        [TestCase("1000001", true, false, true)]
        [TestCase("100-001", false, false, false)]
        [TestCase("1000-001", false, false, false)]
        [TestCase("100_0001", false, false, false)]
        public void IsJpZipCode7(string input, bool any, bool hyphen, bool noHyphen)
        {
            Assert.That(FormatChecker.IsJpZipCode7(input), Is.EqualTo(any), "IsJpZipCode7");
            Assert.That(FormatChecker.IsJpZipCode7_Hyphen(input), Is.EqualTo(hyphen), "_Hyphen");
            Assert.That(FormatChecker.IsJpZipCode7_NoHyphen(input), Is.EqualTo(noHyphen), "_NoHyphen");
        }

        /// <summary>
        /// 5 桁（郵便区番号）は「3 桁」か「3 桁-2 桁 / 5 桁」。
        /// **3 桁だけのものは、ハイフンの有無の両方で true になる。**
        /// </summary>
        [TestCase("100", true, true)]
        [TestCase("100-01", true, false)]
        [TestCase("10001", false, true)]
        [TestCase("1000", false, false)]
        public void IsJpZipCode5(string input, bool hyphen, bool noHyphen)
        {
            Assert.That(FormatChecker.IsJpZipCode5_Hyphen(input), Is.EqualTo(hyphen), "_Hyphen");
            Assert.That(FormatChecker.IsJpZipCode5_NoHyphen(input), Is.EqualTo(noHyphen), "_NoHyphen");
            Assert.That(FormatChecker.IsJpZipCode5(input), Is.EqualTo(hyphen || noHyphen), "IsJpZipCode5");
        }

        /// <summary>IsJpZipCode は 7 桁・5 桁のいずれか。</summary>
        [TestCase("100-0001", ExpectedResult = true)]
        [TestCase("100-01", ExpectedResult = true)]
        [TestCase("100", ExpectedResult = true)]
        [TestCase("10", ExpectedResult = false)]
        public bool IsJpZipCode(string input)
        {
            return FormatChecker.IsJpZipCode(input);
        }

        /// <summary>
        /// **全角数字も通る。** 判定の正規表現が \d（Unicode の数字）を使うため。
        /// 半角に限りたいときは、先に StringChecker.IsHankaku などで絞る。
        /// </summary>
        [Test]
        public void IsJpZipCode7_AcceptsZenkakuDigits()
        {
            Assert.That(FormatChecker.IsJpZipCode7_Hyphen("１００-０００１"), Is.True);
        }

        #endregion

        #region 電話番号

        /// <summary>固定電話。ハイフン付きは全体で 12 文字、なしは 0 始まりの 10 桁。</summary>
        [TestCase("03-1234-5678", true, false)]
        [TestCase("045-123-4567", true, false)]
        [TestCase("0312345678", false, true)]
        [TestCase("03-1234-567", false, false)]
        [TestCase("13-1234-5678", false, false)]
        public void IsJpFixedLinePhoneNumber(string input, bool hyphen, bool noHyphen)
        {
            Assert.That(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(input), Is.EqualTo(hyphen), "_Hyphen");
            Assert.That(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(input), Is.EqualTo(noHyphen), "_NoHyphen");
            Assert.That(FormatChecker.IsJpFixedLinePhoneNumber(input), Is.EqualTo(hyphen || noHyphen), "any");
        }

        /// <summary>携帯は 020 / 060 / 070 / 080 / 090 で始まる 11 桁。050 は含まない（IP 電話）。</summary>
        [TestCase("090-1234-5678", true, false)]
        [TestCase("020-1234-5678", true, false)]
        [TestCase("08012345678", false, true)]
        [TestCase("050-1234-5678", false, false)]
        [TestCase("030-1234-5678", false, false)]
        public void IsJpCellularPhoneNumber(string input, bool hyphen, bool noHyphen)
        {
            Assert.That(FormatChecker.IsJpCellularPhoneNumber_Hyphen(input), Is.EqualTo(hyphen), "_Hyphen");
            Assert.That(FormatChecker.IsJpCellularPhoneNumber_NoHyphen(input), Is.EqualTo(noHyphen), "_NoHyphen");
            Assert.That(FormatChecker.IsJpCellularPhoneNumber(input), Is.EqualTo(hyphen || noHyphen), "any");
        }

        /// <summary>IP 電話は 050 で始まる 11 桁。</summary>
        [TestCase("050-1234-5678", true, false)]
        [TestCase("05012345678", false, true)]
        [TestCase("090-1234-5678", false, false)]
        public void IsJpIpPhoneNumber(string input, bool hyphen, bool noHyphen)
        {
            Assert.That(FormatChecker.IsJpIpPhoneNumber_Hyphen(input), Is.EqualTo(hyphen), "_Hyphen");
            Assert.That(FormatChecker.IsJpIpPhoneNumber_NoHyphen(input), Is.EqualTo(noHyphen), "_NoHyphen");
            Assert.That(FormatChecker.IsJpIpPhoneNumber(input), Is.EqualTo(hyphen || noHyphen), "any");
        }

        /// <summary>IsJpTelephoneNumber は固定・携帯・IP のいずれか。</summary>
        [TestCase("03-1234-5678", true, true, false)]
        [TestCase("09012345678", true, false, true)]
        [TestCase("050-1234-5678", true, true, false)]
        [TestCase("12345", false, false, false)]
        public void IsJpTelephoneNumber(string input, bool any, bool hyphen, bool noHyphen)
        {
            Assert.That(FormatChecker.IsJpTelephoneNumber(input), Is.EqualTo(any), "any");
            Assert.That(FormatChecker.IsJpTelephoneNumber_Hyphen(input), Is.EqualTo(hyphen), "_Hyphen");
            Assert.That(FormatChecker.IsJpTelephoneNumber_NoHyphen(input), Is.EqualTo(noHyphen), "_NoHyphen");
        }

        #endregion
    }
}
