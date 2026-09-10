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
//* クラス名        ：TestIdToken
//* クラス日本語名  ：Framework.Authentication.IdTokenのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/09/10  玄人 幸道         新規作成（#584）
//**********************************************************************************

using System;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Framework.Authentication.IdTokenのテスト</summary>
    /// <remarks>
    /// at_hash / c_hash / s_hash の計算（CreateHash / VerifyHash）（#584）。
    ///
    /// ＜期待値は、実装と独立に求めた値を使う＞
    ///   VerifyHash は、同じ CreateHash で作り直した値と比べるだけである。
    ///   **作る側が誤っていても、本フレームワーク同士では一致する。**
    ///   2026/09/10 まで、CreateHash は左半分ではなく左右の XOR を返していたが、
    ///   これでは気付けなかった。
    ///
    ///   そのため期待値には、OpenID Connect Core 1.0 付録 A の例示値と、
    ///   #584 で実測に使った値（Python の hashlib で求めたもの）を置く。
    /// </remarks>
    public class TestIdToken
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestIdToken.TestCreateHash();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestIdToken.TestVerifyHash();
        }

        #endregion

        #region private

        /// <summary>CreateHash</summary>
        /// <remarks>
        /// SHA-256 の**左半分**（先頭 16 バイト）を base64url にした値になること。
        /// </remarks>
        private static void TestCreateHash()
        {
            MyDebug.OutputDebugAndConsole("IdToken.CreateHash");

            // OpenID Connect Core 1.0 付録 A の例（access_token → at_hash）
            TestIdToken.OutputCreateHash("at_hash（OIDC Core 付録 A）",
                "jHkWEdUXMU1BwAsC4vtUsZwnNvTIxEl0z9K3vx5KF0Y",
                "77QmUPtjPfzWtF2AnpK9RQ");

            // OpenID Connect Core 1.0 付録 A の例（code → c_hash）
            TestIdToken.OutputCreateHash("c_hash（OIDC Core 付録 A）",
                "Qcb0Orv1zh30vL1MPRsbm-diHiMwcLyZvn1arpZv-Jxf_11jnpEX3Tgfvk",
                "LDktKdoQak3Pk0cnXxCltA");

            // #584 で実測に使った state → s_hash
            TestIdToken.OutputCreateHash("s_hash（#584）",
                "state-hybrid",
                "QCiR2bEHBPw1Pd2r2ZtS_Q");
        }

        /// <summary>VerifyHash</summary>
        /// <remarks>
        /// **修正前の版で発行された値（左右の XOR）は、検証に失敗する。**
        /// 互換性が切れることを、ケースとして残しておく。
        /// </remarks>
        private static void TestVerifyHash()
        {
            MyDebug.OutputDebugAndConsole("IdToken.VerifyHash");

            // 左半分（OIDC の定義）
            TestIdToken.OutputVerifyHash("左半分（OIDC の定義）",
                "state-hybrid", "QCiR2bEHBPw1Pd2r2ZtS_Q");

            // 左右の XOR（2026/09/10 までの CreateHash が返していた値）
            TestIdToken.OutputVerifyHash("左右の XOR（修正前の値）",
                "state-hybrid", "iZPw3gFUI31HD9TANpAVbQ");
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>CreateHash の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="input">入力（access_token / code / state）</param>
        /// <param name="expected">期待値（実装と独立に求めた値）</param>
        private static void OutputCreateHash(string caseName, string input, string expected)
        {
            try
            {
                string actual = IdToken.CreateHash(input);

                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）");
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>VerifyHash の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="input">入力（access_token / code / state）</param>
        /// <param name="hash">検証するハッシュ値</param>
        private static void OutputVerifyHash(string caseName, string input, string hash)
        {
            try
            {
                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + IdToken.VerifyHash(input, hash));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        #endregion
    }
}
