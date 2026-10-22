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
//* クラス名        ：ExceptionTests
//* クラス日本語名  ：Framework の例外（業務・システム・フレームワーク）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Exceptions;

namespace TestFramework.Exceptions
{
    /// <summary>Framework の例外（業務・システム・フレームワーク）の単体テスト</summary>
    /// <remarks>
    /// どれも、メッセージ ID とメッセージを持ち、内部例外を受ける版がある。
    /// 業務例外（BusinessApplicationException）は、さらにエラー情報（Information）を持つ。
    /// </remarks>
    [TestFixture]
    public class ExceptionTests
    {
        /// <summary>BusinessApplicationException：メッセージ ID・メッセージ・エラー情報・内部例外を保持する。</summary>
        [Test]
        public void BusinessApplicationException()
        {
            BusinessApplicationException e = new BusinessApplicationException("E0001", "msg", "info");
            Assert.That(e.messageID, Is.EqualTo("E0001"));
            Assert.That(e.Message, Is.EqualTo("msg"));
            Assert.That(e.Information, Is.EqualTo("info"));
            Assert.That(e.InnerException, Is.Null);

            Exception inner = new InvalidOperationException("inner");
            BusinessApplicationException withInner = new BusinessApplicationException("E0002", "msg2", "info2", inner);
            Assert.That(withInner.messageID, Is.EqualTo("E0002"));
            Assert.That(withInner.Information, Is.EqualTo("info2"));
            Assert.That(withInner.InnerException, Is.SameAs(inner));
        }

        /// <summary>BusinessSystemException：メッセージ ID・メッセージ・内部例外を保持する。</summary>
        [Test]
        public void BusinessSystemException()
        {
            BusinessSystemException e = new BusinessSystemException("S0001", "msg");
            Assert.That(e.messageID, Is.EqualTo("S0001"));
            Assert.That(e.Message, Is.EqualTo("msg"));

            Exception inner = new InvalidOperationException("inner");
            Assert.That(new BusinessSystemException("S0002", "msg2", inner).InnerException, Is.SameAs(inner));
        }

        /// <summary>FrameworkException：メッセージ ID・メッセージ・内部例外を保持する。</summary>
        [Test]
        public void FrameworkException()
        {
            FrameworkException e = new FrameworkException("F0001", "msg");
            Assert.That(e.messageID, Is.EqualTo("F0001"));
            Assert.That(e.Message, Is.EqualTo("msg"));

            Exception inner = new InvalidOperationException("inner");
            Assert.That(new FrameworkException("F0002", "msg2", inner).InnerException, Is.SameAs(inner));
        }
    }
}
