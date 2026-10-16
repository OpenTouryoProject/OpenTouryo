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
//* クラス名        ：CustomMarshalerTests
//* クラス日本語名  ：CustomMarshaler の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Runtime.InteropServices;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.Util;

namespace TestPublic.Util
{
    /// <summary>CustomMarshaler の単体テスト</summary>
    [TestFixture]
    public class CustomMarshalerTests
    {
        /// <summary>テスト用の構造体（詰め物なしで 7 バイト）</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Sample
        {
            /// <summary>4 バイト</summary>
            public int A;
            /// <summary>2 バイト</summary>
            public short B;
            /// <summary>1 バイト</summary>
            public byte C;
        }

        /// <summary>構造体 → バイト配列 → 構造体 で元に戻る。配置はメモリ上のまま（リトル エンディアン）。</summary>
        [Test]
        public void StructureToBytes_And_Back()
        {
            Sample s = new Sample() { A = 0x01020304, B = 0x0506, C = 0x07 };

            byte[] bytes = CustomMarshaler.StructureToBytes(s);

            Assert.That(bytes, Is.EqualTo(new byte[] { 0x04, 0x03, 0x02, 0x01, 0x06, 0x05, 0x07 }));
            Assert.That((Sample)CustomMarshaler.BytesToStructure(bytes, typeof(Sample)), Is.EqualTo(s));
        }

        /// <summary>複数の構造体を連結したバイト配列と、相互に変換できる。</summary>
        [Test]
        public void StructuresToBytes_And_Back()
        {
            object[] src = new object[]
            {
                new Sample() { A = 1, B = 2, C = 3 },
                new Sample() { A = -1, B = -2, C = 255 },
                new Sample() { A = int.MaxValue, B = short.MinValue, C = 0 },
            };

            byte[] bytes = CustomMarshaler.StructuresToBytes(src, src.Length);
            Assert.That(bytes.Length, Is.EqualTo(7 * src.Length));

            object[] back = CustomMarshaler.BytesToStructures(bytes, typeof(Sample), src.Length);
            Assert.That(back, Is.EqualTo(src));
        }

#pragma warning disable CS0618 // 互換のために残した転送用メソッドを、あえて呼ぶ。
        /// <summary>DTColumn へ移したメソッドは、転送するだけ（#544）。結果は移動先と同じ。</summary>
        [Test]
        public void ObsoleteForwarders_SameAsDTColumn()
        {
            Assert.That(CustomMarshaler.StringFromPrimitivetype(123, true),
                Is.EqualTo(DTColumn.StringFromPrimitivetype(123, true)));
            Assert.That(CustomMarshaler.PrimitivetypeFromString(DTType.Int32, "123"),
                Is.EqualTo(DTColumn.PrimitivetypeFromString(DTType.Int32, "123")));
        }
#pragma warning restore CS0618
    }
}
