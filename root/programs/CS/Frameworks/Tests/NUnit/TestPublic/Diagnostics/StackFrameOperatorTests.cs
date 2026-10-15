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
//* クラス名        ：StackFrameOperatorTests
//* クラス日本語名  ：StackFrameOperator / MyDebug の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;
using System.IO;
using System.Runtime.CompilerServices;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Diagnostics;

namespace TestPublic.Diagnostics
{
    /// <summary>StackFrameOperator / MyDebug の単体テスト</summary>
    /// <remarks>
    /// StackFrameOperator は**呼び出し元のフレーム**を見る。
    /// インライン化されると呼び出し元が変わるため、呼ぶ側に NoInlining を付ける。
    /// ファイル名と行番号は pdb が要る（Debug ビルド）。
    /// </remarks>
    [TestFixture]
    public class StackFrameOperatorTests
    {
        #region StackFrameOperator

        /// <summary>呼び出し元のメソッド名を返す。</summary>
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GetCurrentMethodName()
        {
            Assert.That(StackFrameOperator.GetCurrentMethodName(), Is.EqualTo("GetCurrentMethodName"));
        }

        /// <summary>プロパティの getter / setter から呼ぶと、プロパティ名（get_ / set_ を除く）を返す。</summary>
        private string Prop
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return StackFrameOperator.GetCurrentPropertyName(); }
        }

        /// <summary>setter 側で取った名前</summary>
        private string _setName;

        /// <summary>setter から取る。</summary>
        private string Prop2
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            set { this._setName = StackFrameOperator.GetCurrentPropertyName(); }
        }

        /// <summary>プロパティ名の取得。プロパティでなければ空文字。</summary>
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GetCurrentPropertyName()
        {
            Assert.That(this.Prop, Is.EqualTo("Prop"));

            this.Prop2 = "x";
            Assert.That(this._setName, Is.EqualTo("Prop2"));

            Assert.That(StackFrameOperator.GetCurrentPropertyName(), Is.EqualTo(""));
        }

        /// <summary>呼び出し元のファイル・行・メソッドのシグネチャを返す。</summary>
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GetCurrentCodeInfo()
        {
            string filePath, line, signature;
            StackFrameOperator.GetCurrentCodeInfo(out filePath, out line, out signature);

            Assert.That(Path.GetFileName(filePath), Is.EqualTo("StackFrameOperatorTests.cs"));
            Assert.That(int.Parse(line), Is.GreaterThan(0));
            Assert.That(signature, Is.EqualTo("Void GetCurrentCodeInfo()"));
        }

        #endregion

        #region MyDebug

        /// <summary>コンソールへの出力は CanOutputToConsole で止められる。ラベル付きは「ラベル > 値」。</summary>
        [Test]
        public void MyDebug_Console()
        {
            TextWriter original = Console.Out;
            bool canConsole = MyDebug.CanOutputToConsole;
            StringWriter sw = new StringWriter();
            try
            {
                Console.SetOut(sw);

                MyDebug.CanOutputToConsole = true;
                MyDebug.OutputDebugAndConsole("label", "value");

                MyDebug.CanOutputToConsole = false;
                MyDebug.OutputDebugAndConsole("hidden");
            }
            finally
            {
                Console.SetOut(original);
                MyDebug.CanOutputToConsole = canConsole;
            }

            Assert.That(sw.ToString(), Is.EqualTo("label > value" + Environment.NewLine));
        }

        #endregion
    }
}
