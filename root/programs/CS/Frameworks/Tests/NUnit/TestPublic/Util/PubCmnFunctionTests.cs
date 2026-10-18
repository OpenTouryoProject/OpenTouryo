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
//* クラス名        ：PubCmnFunctionTests
//* クラス日本語名  ：PubCmnFunction の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/15  玄人 幸道         #612 の修正に合わせ、修正後の挙動を確かめるケースを追加
//*  2026/10/16  玄人 幸道         転送用メソッド（Obsolete）のケースを追加（カバレッジの拡充）
//*  2026/10/16  玄人 幸道         StackFrame を使う転送用メソッドが、呼び出し元を返すことのケースを追加（#616）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Util;

namespace TestPublic.Util
{
    /// <summary>PubCmnFunction の単体テスト</summary>
    [TestFixture]
    [SetCulture("ja-JP")]
    public class PubCmnFunctionTests
    {
        #region 型変換

        /// <summary>ChangeType は Nullable を剥がしてから変換する。</summary>
        [Test]
        public void ChangeType_Nullable()
        {
            Assert.That(PubCmnFunction.ChangeType("123", typeof(int?)), Is.EqualTo(123));
            Assert.That(PubCmnFunction.ChangeType(null, typeof(int?)), Is.Null);
            Assert.That(PubCmnFunction.ChangeType("1.5", typeof(decimal)), Is.EqualTo(1.5m));
        }

        /// <summary>Nullable でない型への null は変換できない。</summary>
        [Test]
        public void ChangeType_NullToValueType_Throws()
        {
            Assert.Catch<Exception>(() => PubCmnFunction.ChangeType(null, typeof(int)));
        }

        /// <summary>GetUnderlyingType は Nullable の中身の型。Nullable でなければ null。</summary>
        [Test]
        public void GetUnderlyingType()
        {
            Assert.That(PubCmnFunction.GetUnderlyingType(typeof(int?)), Is.EqualTo(typeof(int)));
            Assert.That(PubCmnFunction.GetUnderlyingType(typeof(int)), Is.Null);
            Assert.That(PubCmnFunction.GetUnderlyingType(typeof(string)), Is.Null);
        }

        #endregion

        #region 互換性

        /// <summary>ToUnixTime はオフセットを考慮して UTC の秒にする。</summary>
        [Test]
        public void ToUnixTime()
        {
            Assert.That(PubCmnFunction.ToUnixTime(new DateTimeOffset(1970, 1, 1, 9, 0, 0, TimeSpan.FromHours(9))),
                Is.EqualTo(0L));
            Assert.That(PubCmnFunction.ToUnixTime(new DateTimeOffset(1970, 1, 2, 0, 0, 0, TimeSpan.Zero)),
                Is.EqualTo(86400L));
        }

        #endregion

        #region その他

        /// <summary>GetFileNameNoEx はパスから拡張子を除いたファイル名を取る（最後の拡張子だけ除く）。</summary>
        [TestCase(@"C:\dir\a.txt", '\\', "a")]
        [TestCase(@"C:\dir\a.b.txt", '\\', "a.b")]
        [TestCase("/var/log/x.log", '/', "x")]
        // 拡張子が無ければ、ファイル名をそのまま返す（以前は空文字。#612 で修正）
        [TestCase(@"C:\dir\abc", '\\', "abc")]
        public void GetFileNameNoEx(string path, char div, string expected)
        {
            Assert.That(PubCmnFunction.GetFileNameNoEx(path, div), Is.EqualTo(expected));
        }

        /// <summary>GetDataReaderColumnInfo は列名の集合を返す。</summary>
        [Test]
        public void GetDataReaderColumnInfo()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));

            using (IDataReader dr = dt.CreateDataReader())
            {
                HashSet<string> cols = PubCmnFunction.GetDataReaderColumnInfo(dr);

                Assert.That(cols, Is.EquivalentTo(new[] { "ID", "Name" }));
            }
        }

        #endregion

        #region 互換のために残した転送用メソッド

#pragma warning disable CS0618 // 互換のために残した転送用メソッドを、あえて呼ぶ。
        /// <summary>
        /// 移動先のクラスへ転送するだけのメソッドは、移動先と同じ結果を返す。
        /// </summary>
        /// <remarks>StackFrame を使う 3 つは、下の別のテストで見る。</remarks>
        [Test]
        public void ObsoleteForwarders_SameAsNewLocation()
        {
            Dictionary<string, string> props = PubCmnFunction.GetPropsFromPropString("a=1;b=2");
            Assert.That(props, Is.EqualTo(Touryo.Infrastructure.Public.Str.StringVariableOperator.GetPropsFromPropString("a=1;b=2")));

            Dictionary<string, string> argsDic;
            List<string> valsLst;
            PubCmnFunction.GetCommandArgs('-', out argsDic, out valsLst);
            Assert.That(argsDic, Is.Not.Null);
            Assert.That(valsLst, Is.Not.Null);

            Assert.That(PubCmnFunction.BuiltStringIntoEnvironmentVariable("abc"), Is.EqualTo("abc"));

            int[] src = new int[] { 1, 2, 3, 4, 5 };
            Assert.That(PubCmnFunction.CopyArray(src, 3), Is.EqualTo(ArrayOperator.CopyArray(src, 3)));
            Assert.That(PubCmnFunction.CopyArray(src, 2, 3, 0), Is.EqualTo(ArrayOperator.CopyArray(src, 2, 3, 0)));
            Assert.That(PubCmnFunction.CombineArray(src, src), Is.EqualTo(ArrayOperator.CombineArray(src, src)));

            byte[] bytes = new byte[] { 0x01, 0x02, 0x03 };
            Assert.That(PubCmnFunction.ShortenByteArray(bytes, 2), Is.EqualTo(ArrayOperator.ShortenByteArray(bytes, 2)));
            Assert.That(PubCmnFunction.GetLongFromByte(bytes), Is.EqualTo(0x010203));
        }

        /// <summary>
        /// **StackFrame を使う 3 つは、本メソッド自身ではなく、呼び出し元の情報を返す。**
        /// 以前は StackFrameOperator へそのまま転送しており、"GetCurrentMethodName" などを返していた。
        /// </summary>
        [Test]
        public void ObsoleteStackFrame_ReturnsCaller()
        {
            Assert.That(PubCmnFunction.GetCurrentMethodName(), Is.EqualTo(nameof(ObsoleteStackFrame_ReturnsCaller)));
            Assert.That(this.CurrentPropertyName, Is.EqualTo(nameof(CurrentPropertyName)));
            Assert.That(PubCmnFunction.GetCurrentPropertyName(), Is.Empty, "プロパティでなければ空文字");

            string filePath, fileLineNumber, methodSignature;
            PubCmnFunction.GetCurrentCodeInfo(out filePath, out fileLineNumber, out methodSignature);
            Assert.That(methodSignature, Does.Contain(nameof(ObsoleteStackFrame_ReturnsCaller)));
            Assert.That(filePath, Does.EndWith("PubCmnFunctionTests.cs"));
            Assert.That(int.Parse(fileLineNumber), Is.GreaterThan(0));
        }

        /// <summary>GetCurrentPropertyName を呼ぶプロパティ（インライン展開させない）</summary>
        private string CurrentPropertyName
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return PubCmnFunction.GetCurrentPropertyName(); }
        }
#pragma warning restore CS0618

        #endregion
    }
}
