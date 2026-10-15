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
//* クラス名        ：TestResponseObject
//* クラス日本語名  ：Framework.Authentication.ResponseObjectのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/12  玄人 幸道         新規作成（#601）
//*  2026/10/12  玄人 幸道         設定で判定するResponseObject.Verifyの、audの確認のテストを追加（#603）
//**********************************************************************************

using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestCode
{
    /// <summary>Framework.Authentication.ResponseObjectのテスト</summary>
    /// <remarks>
    /// 検証鍵と iss / aud を渡す Verify（#601）。
    ///
    /// ＜鍵は、実行のたびに作る＞
    ///   相手（認可サーバ）の鍵と、それとは別の鍵を作る。response も実行のたびに署名するため、
    ///   出すのは検証の結果（bool）だけにする。
    ///
    /// 設定で判定する Verify の aud（#603）。設定と JWK Set の作り方は TestIdToken と同じ。
    /// </remarks>
    public class TestResponseObject
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestResponseObject.TestVerifyByJws();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestResponseObject.TestVerifyBySettings();
        }

        #endregion

        #region private

        /// <summary>Verify（jws / iss / aud）</summary>
        /// <remarks>
        /// 渡した鍵・iss・aud で検証されること（#601）。
        /// </remarks>
        private static void TestVerifyByJws()
        {
            MyDebug.OutputDebugAndConsole("ResponseObject.Verify（jws / iss / aud）");

            const string iss = "https://as.example.com/";
            const string aud = "client01";

            // 相手（認可サーバ）の鍵（kid は、自分の JWK Set に無い値にする）
            JWS_RS256_Param asJws = new JWS_RS256_Param();
            asJws.JWSHeader.kid = "as-kid";
            string response = TestResponseObject.CreateResponse(asJws, iss, aud);

            JWS asPublicJws = new JWS_RS256_Param(asJws.RsaPublicParameters);
            JWS otherPublicJws = new JWS_RS256_Param(new JWS_RS256_Param().RsaPublicParameters);

            TestResponseObject.OutputVerifyByJws("相手の鍵・iss・aud", response, asPublicJws, iss, aud, true);
            TestResponseObject.OutputVerifyByJws("別の鍵", response, otherPublicJws, iss, aud, false);
            TestResponseObject.OutputVerifyByJws("iss が違う", response, asPublicJws, "https://other.example.com/", aud, false);
            TestResponseObject.OutputVerifyByJws("aud が違う", response, asPublicJws, iss, "client02", false);

            // aud を array で返す認可サーバもある（含まれていれば、自分の client_id 宛である）
            string responseAudArray = TestResponseObject.CreateResponse(asJws, iss, new JArray(aud, "client02"));
            TestResponseObject.OutputVerifyByJws("aud が array（含まれる）", responseAudArray, asPublicJws, iss, aud, true);
            TestResponseObject.OutputVerifyByJws("aud が array（含まれない）", responseAudArray, asPublicJws, iss, "client03", false);

            // 期待値を渡さないのは誤用（設定に落とさず、ArgumentNullException にする）
            TestResponseObject.OutputVerifyByJws("iss が空（例外になること）", response, asPublicJws, "", aud, false);
            TestResponseObject.OutputVerifyByJws("aud が空（例外になること）", response, asPublicJws, iss, "", false);
        }

        /// <summary>Verify（設定で判定する版）の aud</summary>
        /// <remarks>
        /// 認可サーバを兼ねる配備（JwkSetFilePath が在る）でも、aud を確認すること（#603）。
        /// JARM の応答はブラウザを経由して届くため、別のクライアント宛の応答を拒否できる必要がある。
        /// </remarks>
        private static void TestVerifyBySettings()
        {
            MyDebug.OutputDebugAndConsole("ResponseObject.Verify（設定で判定する版。JwkSetFilePath が在る配備）");

            JWS_RS256_Param jws = TestIdToken.SaveTestJwkSet();
            try
            {
                const string iss = TestIdToken.TestIssuer;

                TestResponseObject.OutputVerifyBySettings("aud が ClientID",
                    TestResponseObject.CreateResponse(jws, iss, "client01"), true);
                TestResponseObject.OutputVerifyBySettings("aud が ClientIDs に在る",
                    TestResponseObject.CreateResponse(jws, iss, "client02"), true);
                TestResponseObject.OutputVerifyBySettings("aud が設定に無い",
                    TestResponseObject.CreateResponse(jws, iss, "client03"), false);
                TestResponseObject.OutputVerifyBySettings("aud が array（ClientID を含む）",
                    TestResponseObject.CreateResponse(jws, iss, new JArray("client03", "client01")), true);
            }
            finally
            {
                File.Delete(TestIdToken.TestJwkSetFilePath);
            }
        }

        #endregion

        #region 組み立てのヘルパ

        /// <summary>response（JARM）を作る</summary>
        /// <param name="jws">署名に使う鍵</param>
        /// <param name="iss">iss</param>
        /// <param name="aud">aud（string か JArray）</param>
        /// <returns>response</returns>
        private static string CreateResponse(JWS jws, string iss, JToken aud)
        {
            JObject payload = new JObject();
            payload[OAuth2AndOIDCConst.iss] = iss;
            payload[OAuth2AndOIDCConst.aud] = aud;
            payload[OAuth2AndOIDCConst.code] = "code01";
            payload[OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds().ToString();

            return jws.Create(payload.ToString(Formatting.None));
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>Verify（jws / iss / aud）の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="response">response</param>
        /// <param name="jws">検証に使う鍵</param>
        /// <param name="iss">期待する iss</param>
        /// <param name="aud">期待する aud</param>
        /// <param name="expected">期待値</param>
        private static void OutputVerifyByJws(string caseName,
            string response, JWS jws, string iss, string aud, bool expected)
        {
            try
            {
                JObject jobj = null;

                bool actual = ResponseObject.Verify(response,
                    jws, iss, aud, out jobj);

                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）"
                    + (actual ? "、code : " + (string)jobj[OAuth2AndOIDCConst.code] : ""));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>Verify（設定で判定する版）の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="response">response</param>
        /// <param name="expected">期待値</param>
        private static void OutputVerifyBySettings(string caseName, string response, bool expected)
        {
            try
            {
                JObject jobj = null;

                bool actual = ResponseObject.Verify(response, out jobj);

                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）");
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
