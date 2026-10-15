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
//* クラス名        ：ArrayOperatorTests
//* クラス日本語名  ：ArrayOperator の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Util;

namespace TestPublic.Util
{
    /// <summary>ArrayOperator の単体テスト</summary>
    [TestFixture]
    public class ArrayOperatorTests
    {
        #region 配列操作

        /// <summary>CopyArray は先頭から指定の長さだけ複製する。</summary>
        [Test]
        public void CopyArray_FromHead()
        {
            int[] src = new int[] { 1, 2, 3, 4, 5 };

            Assert.That(ArrayOperator.CopyArray(src, 3), Is.EqualTo(new int[] { 1, 2, 3 }));
        }

        /// <summary>複製元の開始位置を指定できる。</summary>
        [Test]
        public void CopyArray_WithSourceIndex()
        {
            int[] src = new int[] { 1, 2, 3, 4, 5 };

            Assert.That(ArrayOperator.CopyArray(src, 2, 3, 0), Is.EqualTo(new int[] { 4, 5 }));
        }

        /// <summary>CombineArray は 2 つの配列を連結する（要素の型の大きさを問わない）。</summary>
        [Test]
        public void CombineArray()
        {
            Assert.That(
                ArrayOperator.CombineArray(new byte[] { 1, 2 }, new byte[] { 3 }),
                Is.EqualTo(new byte[] { 1, 2, 3 }));

            Assert.That(
                ArrayOperator.CombineArray(new long[] { long.MaxValue }, new long[] { -1, 0 }),
                Is.EqualTo(new long[] { long.MaxValue, -1, 0 }));
        }

        #endregion

        #region バイト操作

        /// <summary>ShortenByteArray は、はみ出した分を先頭から XOR で畳み込む。</summary>
        [Test]
        public void ShortenByteArray_XorFolding()
        {
            byte[] bytes = new byte[] { 0x01, 0x02, 0x04, 0x08, 0x10 };

            // [0] = 0x01 ^ 0x04 ^ 0x10、[1] = 0x02 ^ 0x08
            Assert.That(ArrayOperator.ShortenByteArray(bytes, 2), Is.EqualTo(new byte[] { 0x15, 0x0A }));
        }

        /// <summary>短くしない場合は、そのまま（残りは 0）。</summary>
        [Test]
        public void ShortenByteArray_NotShorter()
        {
            Assert.That(
                ArrayOperator.ShortenByteArray(new byte[] { 0xAA }, 3),
                Is.EqualTo(new byte[] { 0xAA, 0x00, 0x00 }));
        }

        /// <summary>GetLongFromByte は先頭のバイトを上位としてビッグ エンディアンで積む。</summary>
        [TestCase(new byte[] { 0x01 }, 1L)]
        [TestCase(new byte[] { 0x01, 0x00 }, 256L)]
        [TestCase(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, long.MaxValue)]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, -1L)]
        public void GetLongFromByte(byte[] bytes, long expected)
        {
            Assert.That(ArrayOperator.GetLongFromByte(bytes), Is.EqualTo(expected));
        }

        /// <summary>null は ArgumentNullException、長さ 0 と 9 以上は ArgumentOutOfRangeException。</summary>
        [Test]
        public void GetLongFromByte_InvalidArguments()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(new TestDelegate(() => ArrayOperator.GetLongFromByte(null)));
            Assert.That(ex.ParamName, Is.EqualTo("bytes"));
            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(() => ArrayOperator.GetLongFromByte(new byte[0])));
            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(() => ArrayOperator.GetLongFromByte(new byte[9])));
        }

        #endregion
    }
}
