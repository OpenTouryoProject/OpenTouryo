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
//* クラス名        ：SAML2EnumTests
//* クラス日本語名  ：SAML2Enum（列挙と URN の変換）の単体テスト
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

using Touryo.Infrastructure.Framework.Authentication;

namespace TestFramework.Authentication
{
    /// <summary>SAML2Enum（列挙と URN の変換）の単体テスト</summary>
    /// <remarks>列挙のどの値も、URN にして戻すと元に戻る。知らない文字列は null。</remarks>
    [TestFixture]
    public class SAML2EnumTests
    {
        /// <summary>NameIDFormat</summary>
        [Test]
        public void NameIDFormat_RoundTrip()
        {
            foreach (SAML2Enum.NameIDFormat v in Enum.GetValues(typeof(SAML2Enum.NameIDFormat)))
            {
                string urn = SAML2Enum.EnumToString(v);
                Assert.That(urn, Does.StartWith("urn:"), v.ToString());

                SAML2Enum.NameIDFormat? back;
                SAML2Enum.StringToEnum(urn, out back);
                Assert.That(back, Is.EqualTo(v));
            }

            SAML2Enum.NameIDFormat? unknown;
            SAML2Enum.StringToEnum("urn:unknown", out unknown);
            Assert.That(unknown, Is.Null);
        }

        /// <summary>ProtocolBinding</summary>
        [Test]
        public void ProtocolBinding_RoundTrip()
        {
            foreach (SAML2Enum.ProtocolBinding v in Enum.GetValues(typeof(SAML2Enum.ProtocolBinding)))
            {
                SAML2Enum.ProtocolBinding? back;
                SAML2Enum.StringToEnum(SAML2Enum.EnumToString(v), out back);
                Assert.That(back, Is.EqualTo(v));
            }

            SAML2Enum.ProtocolBinding? unknown;
            SAML2Enum.StringToEnum("urn:unknown", out unknown);
            Assert.That(unknown, Is.Null);
        }

        /// <summary>AuthnContextClassRef</summary>
        [Test]
        public void AuthnContextClassRef_RoundTrip()
        {
            foreach (SAML2Enum.AuthnContextClassRef v in Enum.GetValues(typeof(SAML2Enum.AuthnContextClassRef)))
            {
                SAML2Enum.AuthnContextClassRef? back;
                SAML2Enum.StringToEnum(SAML2Enum.EnumToString(v), out back);
                Assert.That(back, Is.EqualTo(v));
            }

            SAML2Enum.AuthnContextClassRef? unknown;
            SAML2Enum.StringToEnum("urn:unknown", out unknown);
            Assert.That(unknown, Is.Null);
        }

        /// <summary>StatusCode</summary>
        [Test]
        public void StatusCode_RoundTrip()
        {
            foreach (SAML2Enum.StatusCode v in Enum.GetValues(typeof(SAML2Enum.StatusCode)))
            {
                SAML2Enum.StatusCode? back;
                SAML2Enum.StringToEnum(SAML2Enum.EnumToString(v), out back);
                Assert.That(back, Is.EqualTo(v));
            }

            SAML2Enum.StatusCode? unknown;
            SAML2Enum.StringToEnum("urn:unknown", out unknown);
            Assert.That(unknown, Is.Null);
        }
    }
}
