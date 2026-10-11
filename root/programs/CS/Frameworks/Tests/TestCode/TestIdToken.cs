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
//*  2026/10/11  玄人 幸道         IdToken.Verifyの、検証鍵とiss / audを渡す版のテストを追加（#601）
//**********************************************************************************

using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Security.Jwt;

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
    ///
    /// 検証鍵と iss / aud を渡す Verify（#601）。
    ///
    /// ＜鍵は、実行のたびに作る＞
    ///   相手（OP）の鍵と、それとは別の鍵を作る。id_token も実行のたびに署名するため、
    ///   出すのは検証の結果（bool）だけにする。
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

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestIdToken.TestVerifyByJws();
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

        /// <summary>Verify（jws / iss / aud）</summary>
        /// <remarks>
        /// 渡した鍵・iss・aud で検証されること（#601）。
        /// 2026/10/11 まで、鍵と iss は設定から判定され、相手（OP）の id_token を検証できなかった。
        /// </remarks>
        private static void TestVerifyByJws()
        {
            MyDebug.OutputDebugAndConsole("IdToken.Verify（jws / iss / aud）");

            const string iss = "https://op.example.com/";
            const string aud = "client01";

            // 相手（OP）の鍵（kid は、自分の JWK Set に無い値にする）
            JWS_RS256_Param opJws = new JWS_RS256_Param();
            opJws.JWSHeader.kid = "op-kid";
            string idToken = TestIdToken.CreateIdToken(opJws, iss, aud);

            JWS opPublicJws = new JWS_RS256_Param(opJws.RsaPublicParameters);
            JWS otherPublicJws = new JWS_RS256_Param(new JWS_RS256_Param().RsaPublicParameters);

            TestIdToken.OutputVerifyByJws("相手の鍵・iss・aud", idToken, opPublicJws, iss, aud, true);
            TestIdToken.OutputVerifyByJws("別の鍵", idToken, otherPublicJws, iss, aud, false);
            TestIdToken.OutputVerifyByJws("iss が違う", idToken, opPublicJws, "https://other.example.com/", aud, false);
            TestIdToken.OutputVerifyByJws("aud が違う", idToken, opPublicJws, iss, "client02", false);

            // aud を array で返す OP もある（含まれていれば、相手の client_id 宛である）
            string idTokenAudArray = TestIdToken.CreateIdToken(opJws, iss, new JArray(aud, "client02"));
            TestIdToken.OutputVerifyByJws("aud が array（含まれる）", idTokenAudArray, opPublicJws, iss, aud, true);
            TestIdToken.OutputVerifyByJws("aud が array（含まれない）", idTokenAudArray, opPublicJws, iss, "client03", false);

            // 期待値を渡さないのは誤用（設定に落とさず、ArgumentNullException にする）
            TestIdToken.OutputVerifyByJws("iss が空（例外になること）", idToken, opPublicJws, "", aud, false);
            TestIdToken.OutputVerifyByJws("aud が空（例外になること）", idToken, opPublicJws, iss, "", false);
        }

        #endregion

        #region 組み立てのヘルパ

        /// <summary>id_token を作る</summary>
        /// <param name="jws">署名に使う鍵</param>
        /// <param name="iss">iss</param>
        /// <param name="aud">aud（string か JArray）</param>
        /// <returns>id_token</returns>
        private static string CreateIdToken(JWS jws, string iss, JToken aud)
        {
            JObject payload = new JObject();
            payload[OAuth2AndOIDCConst.iss] = iss;
            payload[OAuth2AndOIDCConst.aud] = aud;
            payload[OAuth2AndOIDCConst.sub] = "user01";
            payload[OAuth2AndOIDCConst.nonce] = "nonce01";
            payload[OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds().ToString();

            return jws.Create(payload.ToString(Formatting.None));
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

        /// <summary>Verify（jws / iss / aud）の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="idToken">id_token</param>
        /// <param name="jws">検証に使う鍵</param>
        /// <param name="iss">期待する iss</param>
        /// <param name="aud">期待する aud</param>
        /// <param name="expected">期待値</param>
        private static void OutputVerifyByJws(string caseName,
            string idToken, JWS jws, string iss, string aud, bool expected)
        {
            try
            {
                string sub = "";
                string nonce = "";
                JObject jobj = null;

                bool actual = IdToken.Verify(idToken, "", "", "",
                    jws, iss, aud, out sub, out nonce, out jobj);

                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）"
                    + (actual ? "、sub : " + sub : ""));
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
