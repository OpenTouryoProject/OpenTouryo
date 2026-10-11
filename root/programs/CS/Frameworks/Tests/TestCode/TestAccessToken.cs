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
//* クラス名        ：TestAccessToken
//* クラス日本語名  ：Framework.Authentication.AccessTokenのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/12  玄人 幸道         新規作成（#601）
//**********************************************************************************

using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestCode
{
    /// <summary>Framework.Authentication.AccessTokenのテスト</summary>
    /// <remarks>
    /// 検証鍵と iss / aud の集合を渡す Verify（#601）。
    ///
    /// ＜鍵は、実行のたびに作る＞
    ///   発行者（認可サーバ）の鍵と、それとは別の鍵を作る。access_token も実行のたびに署名するため、
    ///   出すのは検証の結果（bool）だけにする。
    /// </remarks>
    public class TestAccessToken
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestAccessToken.TestVerifyByJws();
        }

        #endregion

        #region private

        /// <summary>Verify（jws / iss / auds）</summary>
        /// <remarks>
        /// 渡した鍵・iss で検証され、aud が期待する集合のいずれかと一致すること（#601）。
        /// 既存の版は aud を確認しない（Keycloakコンパチ）。
        /// </remarks>
        private static void TestVerifyByJws()
        {
            MyDebug.OutputDebugAndConsole("AccessToken.Verify（jws / iss / auds）");

            const string iss = "https://as.example.com/";
            string[] auds = new string[] { "client01", "client02" };

            // 発行者（認可サーバ）の鍵（kid は、自分の JWK Set に無い値にする）
            JWS_RS256_Param asJws = new JWS_RS256_Param();
            asJws.JWSHeader.kid = "as-kid";
            string accessToken = TestAccessToken.CreateAccessToken(asJws, iss, "client02");

            JWS asPublicJws = new JWS_RS256_Param(asJws.RsaPublicParameters);
            JWS otherPublicJws = new JWS_RS256_Param(new JWS_RS256_Param().RsaPublicParameters);

            TestAccessToken.OutputVerifyByJws("発行者の鍵・iss・aud の集合", accessToken, asPublicJws, iss, auds, true);
            TestAccessToken.OutputVerifyByJws("別の鍵", accessToken, otherPublicJws, iss, auds, false);
            TestAccessToken.OutputVerifyByJws("iss が違う", accessToken, asPublicJws, "https://other.example.com/", auds, false);
            TestAccessToken.OutputVerifyByJws("aud が集合に無い", accessToken, asPublicJws, iss, new string[] { "client03" }, false);

            // aud を array で返す認可サーバもある（RFC 9068 でも array を許す）
            string accessTokenAudArray = TestAccessToken.CreateAccessToken(asJws, iss, new JArray("rs01", "client01"));
            TestAccessToken.OutputVerifyByJws("aud が array（集合と重なる）", accessTokenAudArray, asPublicJws, iss, auds, true);
            TestAccessToken.OutputVerifyByJws("aud が array（集合と重ならない）", accessTokenAudArray, asPublicJws, iss, new string[] { "client03" }, false);

            // aud の無いトークンは、集合に空の要素があっても一致させない
            string accessTokenNoAud = TestAccessToken.CreateAccessToken(asJws, iss, null);
            TestAccessToken.OutputVerifyByJws("aud が無い", accessTokenNoAud, asPublicJws, iss, new string[] { "", null }, false);

            // 期待値を渡さないのは誤用（設定に落とさず、ArgumentNullException にする）
            TestAccessToken.OutputVerifyByJws("iss が空（例外になること）", accessToken, asPublicJws, "", auds, false);
            TestAccessToken.OutputVerifyByJws("auds が空（例外になること）", accessToken, asPublicJws, iss, new string[] { }, false);
        }

        #endregion

        #region 組み立てのヘルパ

        /// <summary>access_token を作る</summary>
        /// <param name="jws">署名に使う鍵</param>
        /// <param name="iss">iss</param>
        /// <param name="aud">aud（string か JArray。null なら付けない）</param>
        /// <returns>access_token</returns>
        private static string CreateAccessToken(JWS jws, string iss, JToken aud)
        {
            JObject payload = new JObject();
            payload[OAuth2AndOIDCConst.iss] = iss;
            if (aud != null)
            {
                payload[OAuth2AndOIDCConst.aud] = aud;
            }
            payload[OAuth2AndOIDCConst.sub] = "user01";
            payload[OAuth2AndOIDCConst.scopes] = new JArray("openid", "profile");
            payload[OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds().ToString();

            return jws.Create(payload.ToString(Formatting.None));
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>Verify（jws / iss / auds）の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="accessToken">access_token</param>
        /// <param name="jws">検証に使う鍵</param>
        /// <param name="iss">期待する iss</param>
        /// <param name="auds">期待する aud の集合</param>
        /// <param name="expected">期待値</param>
        private static void OutputVerifyByJws(string caseName,
            string accessToken, JWS jws, string iss, string[] auds, bool expected)
        {
            try
            {
                string sub = "";
                List<string> roles = null;
                List<string> scopes = null;
                JObject jobj = null;

                bool actual = AccessToken.Verify(accessToken,
                    jws, iss, auds, out sub, out roles, out scopes, out jobj);

                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）"
                    + (actual ? "、sub : " + sub + "、scopes : " + string.Join(" ", scopes) : ""));
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
