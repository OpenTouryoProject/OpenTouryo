//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス名        ：JwtAssertion
//* クラス日本語名  ：JwtAssertion
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/26  西野 大介         新規作成
//*  2018/03/28  西野 大介         .NET Standard対応で、幾らか、I/F変更あり。
//*  2018/11/27  西野 大介         XML(Base64) ---> Jwk(Base64Url)に変更。
//*  2018/11/27  西野 大介         秘密鍵 <---> JWKのサポートを追加
//*  2020/03/04  西野 大介         ...ECDsaのサポートを追加
//*  2020/03/04  西野 大介         Claim生成メソッドの利用
//*  2026/10/30  玄人 幸道         Create・Verify で、kty の大文字小文字を区別するようにした（RFC 7517 4.1。#643）
//*  2026/11/05  玄人 幸道         VerifyByRsa・VerifyByECDsaで、exp を小数の NumericDate でも読み、
//*                                無い・読めない値を例外にせず false にした（#647）
//*  2026/11/05  玄人 幸道         期待する iss・aud を受けて照合する Verify・VerifyByRsa・VerifyByECDsa を追加し、
//*                                受けない版を Obsolete（警告）にした。Create は sub（= iss）を入れる（RFC 7523 3。#654）
//*  2026/11/05  玄人 幸道         期待する iss・aud を受ける版：aud を唯一の値に限った（draft-ietf-oauth-rfc7523bis 4）。
//*                                形の壊れた入力を、例外にせず false にした（#654 のレビューの指摘 1・5）
//*  2026/11/06  玄人 幸道         期待する iss・aud を受ける Verify に、検証鍵（JWS）を受けて署名の alg を選べるオーバーロードを追加（#664）
//*  2026/11/06  玄人 幸道         remarks に、登録どおりの鍵の型だけを渡すことを書いた（#664 のレビューの指摘 2）
//**********************************************************************************

using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// JWT bearer token authorizationグラント種別
    /// </summary>
    public class JwtAssertion
    {
        #region Create
        /// <summary>Create</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jwkPrivateKey">RS256用のJWK秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string Create(
            string iss, string aud, TimeSpan forExp, string scopes, string jwkPrivateKey)
        {
            JObject temp = JsonConvert.DeserializeObject<JObject>(jwkPrivateKey);
            if (temp.ContainsKey("kty"))
            {
                // kty の大文字小文字を区別する（RFC 7517 4.1。#643）
                if ((string)temp["kty"] == "RSA")
                {
                    RsaPrivateKeyConverter rpkc = new RsaPrivateKeyConverter(JWS_RSA.RS._256);
                    return JwtAssertion.CreateByRsa(iss, aud, forExp, scopes,
                        rpkc.JwkToParam(jwkPrivateKey));
                }
                else if ((string)temp["kty"] == "EC")
                {
                    EccPrivateKeyConverter epkc = new EccPrivateKeyConverter(JWS_ECDSA.ES._256);
                    return JwtAssertion.CreateByECDsa(iss, aud, forExp, scopes,
                        epkc.JwkToParam(jwkPrivateKey));
                }
            }

            return "";
        }

        /// <summary>CreateByRsa</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateByRsa(
            string iss, string aud, TimeSpan forExp, string scopes, RSAParameters rsaPrivateKey)
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.sub, iss); // client_id（RFC 7523 3 の 2。#654）
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token EndPointのuri。

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, CmnJwtToken.CreateExpClaim(forExp));
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, CmnJwtToken.CreateIatClaim());

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPrivateKey);
            return jwtRS256.Create(json);

            #endregion
        }

        /// <summary>CreateByECDsa</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="eccPrivateKey">ES256用のECParameters秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateByECDsa(
            string iss, string aud, TimeSpan forExp, string scopes, ECParameters eccPrivateKey)
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.sub, iss); // client_id（RFC 7523 3 の 2。#654）
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token EndPointのuri。

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, CmnJwtToken.CreateExpClaim(forExp));
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, CmnJwtToken.CreateIatClaim());

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWS_ES256_Param jwtES256 = new JWS_ES256_Param(eccPrivateKey, true);
            return jwtES256.Create(json);

            #endregion
        }

        /// <summary>CreateByECDsa</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="ecdsaX509FilePath">ES256用の X.509秘密鍵 の File Path</param>
        /// <param name="ecdsaX509Password">ES256用の X.509秘密鍵 の Password</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateByECDsa(
            string iss, string aud, TimeSpan forExp, string scopes,
            string ecdsaX509FilePath, string ecdsaX509Password)
        ///// <param name="eccPrivateKey">ES256用のECParameters秘密鍵</param>
        //ECParameters ecPrivateKey) // ECDsa.ExportParameters(true)が動かねぇ。
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.sub, iss); // client_id（RFC 7523 3 の 2。#654）
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token EndPointのuri。

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, CmnJwtToken.CreateExpClaim(forExp));
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, CmnJwtToken.CreateIatClaim());

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWS_ES256_X509 jwtES256 = new JWS_ES256_X509(ecdsaX509FilePath, ecdsaX509Password);
            return jwtES256.Create(json);

            #endregion
        }
        #endregion

        #region Verify
        /// <summary>Verify</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        [Obsolete("This overload does not verify the iss and aud claims, please use the overload that takes the expected iss and auds instead.")]
        public static bool Verify(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, string jwkPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JObject temp = JsonConvert.DeserializeObject<JObject>(jwkPublicKey);
            if (temp.ContainsKey("kty"))
            {
                // kty の大文字小文字を区別する（RFC 7517 4.1。#643）
                if ((string)temp["kty"] == "RSA")
                {
                    RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
                    return JwtAssertion.VerifyByRsa(jwtAssertion,
                        out iss, out aud, out scopes, out jobj,
                        rpkc.JwkToParam(jwkPublicKey));
                }
                else if ((string)temp["kty"] == "EC")
                {
                    EccPublicKeyConverter epkc = new EccPublicKeyConverter();
                    return JwtAssertion.VerifyByECDsa(jwtAssertion,
                        out iss, out aud, out scopes, out jobj,
                        epkc.JwkToParam(jwkPublicKey));
                }
            }

            return false;
        }

        /// <summary>VerifyByRsa</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        [Obsolete("This overload does not verify the iss and aud claims, please use the overload that takes the expected iss and auds instead.")]
        public static bool VerifyByRsa(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, RSAParameters rsaPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPublicKey);

            if (jwtRS256.Verify(jwtAssertion))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(jwtAssertion.Split('.')[1]), CustomEncode.UTF_8);

                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                scopes = (string)jobj[OAuth2AndOIDCConst.scope];

                long unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

                // exp は小数の NumericDate も読む。無い・読めない値は、例外にせず検証の失敗にする（RFC 7519 2。#647）
                if (CmnJwtToken.VerifyExpClaim(jobj[OAuth2AndOIDCConst.exp], unixTimeSeconds))
                {
                    return true;
                }
                else
                {
                    // JWTの内容検証に失敗
                }
            }
            else
            {
                // JWTの署名検証に失敗
            }

            // 認証に失敗
            return false;
        }

        /// <summary>VerifyByECDsa</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="eccPublicKey">ES256用のECParameters公開鍵</param>
        /// <returns>検証結果</returns>
        [Obsolete("This overload does not verify the iss and aud claims, please use the overload that takes the expected iss and auds instead.")]
        public static bool VerifyByECDsa(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, ECParameters eccPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JWS_ES256_Param jwtES256 = new JWS_ES256_Param(eccPublicKey, false);

            if (jwtES256.Verify(jwtAssertion))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(jwtAssertion.Split('.')[1]), CustomEncode.UTF_8);

                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                scopes = (string)jobj[OAuth2AndOIDCConst.scope];

                long unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

                // exp は小数の NumericDate も読む。無い・読めない値は、例外にせず検証の失敗にする（RFC 7519 2。#647）
                if (CmnJwtToken.VerifyExpClaim(jobj[OAuth2AndOIDCConst.exp], unixTimeSeconds))
                {
                    return true;
                }
                else
                {
                    // JWTの内容検証に失敗
                }
            }
            else
            {
                // JWTの署名検証に失敗
            }

            // 認証に失敗
            return false;
        }
        #endregion

        #region Verify（期待する iss・aud と照合する。#654）

        /// <summary>Verify（期待する iss・aud と照合する）</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">期待する iss（client_id）</param>
        /// <param name="auds">
        /// 期待する aud の集合。**認可サーバの issuer だけを渡すのを推奨する**（draft-ietf-oauth-rfc7523bis 4）。
        /// トークン エンドポイントの URL などを並べるのは、それを aud にする旧いクライアントとの互換のためだけにする。
        /// JWT の aud が唯一の値で、それがこのいずれかと一致すれば通す
        /// </param>
        /// <param name="sub">out string（JWT の sub）</param>
        /// <param name="scopes">out string（JWT の scope。無ければ空文字）</param>
        /// <param name="jobj">out JObject（ペイロード。検証に通ったときだけ。通らなければ null）</param>
        /// <param name="jwkPublicKey">JWK公開鍵（kty が RSA なら RS256、EC なら ES256）</param>
        /// <returns>検証結果</returns>
        /// <remarks>VerifyByRsa（期待する iss・aud を受ける版）を参照。</remarks>
        /// <exception cref="ArgumentNullException">iss が null か空、auds が null か空</exception>
        public static bool Verify(string jwtAssertion, string iss, string[] auds,
            out string sub, out string scopes, out JObject jobj, string jwkPublicKey)
        {
            JwtAssertion.CheckExpected(iss, auds);

            sub = "";
            scopes = "";
            jobj = null;

            JObject temp = JsonConvert.DeserializeObject<JObject>(jwkPublicKey);
            if (temp.ContainsKey("kty"))
            {
                // kty の大文字小文字を区別する（RFC 7517 4.1。#643）
                if ((string)temp["kty"] == "RSA")
                {
                    RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
                    return JwtAssertion.VerifyByRsa(jwtAssertion, iss, auds,
                        out sub, out scopes, out jobj, rpkc.JwkToParam(jwkPublicKey));
                }
                else if ((string)temp["kty"] == "EC")
                {
                    EccPublicKeyConverter epkc = new EccPublicKeyConverter();
                    return JwtAssertion.VerifyByECDsa(jwtAssertion, iss, auds,
                        out sub, out scopes, out jobj, epkc.JwkToParam(jwkPublicKey));
                }
            }

            return false;
        }

        /// <summary>VerifyByRsa（期待する iss・aud と照合する）</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">期待する iss（client_id）</param>
        /// <param name="auds">期待する aud の集合（**認可サーバの issuer だけを推奨**。ほかは互換のため）</param>
        /// <param name="sub">out string（JWT の sub）</param>
        /// <param name="scopes">out string（JWT の scope。無ければ空文字）</param>
        /// <param name="jobj">out JObject（ペイロード。検証に通ったときだけ。通らなければ null）</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 自由に利用できる。署名に加えて、次を確かめる（RFC 7523 3。#654）。
        ///
        /// | クレーム | 確かめること |
        /// |---|---|
        /// | iss | 期待する値と一致する（1） |
        /// | sub | 在る（2）。**client_id と一致するかは見ない**（クライアント認証なら client_id、認可グラントなら利用者。2 の A・B）。**クライアント認証に使うなら、out sub が iss と一致することを、呼び出し側で必ず照合する**（照合しないと、別の sub の JWT が通る） |
        /// | aud | **唯一の値**（文字列か、要素 1 つの配列）で、それが期待する値のいずれかと一致する（3。draft-ietf-oauth-rfc7523bis 4） |
        /// | exp | 在り、過ぎていない（4） |
        /// | nbf | 在れば、その時刻以後である（5） |
        ///
        /// - **aud は唯一の値に限る。** ほかの値も並べた aud（例：issuer とトークン エンドポイントの URL）は断る。
        ///   RFC 7523 3 の 3 は、自分を含めば受けるが、別の用途の JWT を流用される攻撃（audience injection）があるため、
        ///   改訂案（draft-ietf-oauth-rfc7523bis 4）は issuer を唯一の値とすることを求めている。**auds にも issuer だけを渡すのを推奨する**
        /// - 比較は大文字・小文字を区別する完全一致で、**正規化しない**（末尾のスラッシュの有無も別物）
        /// - **時計のずれは許さない**（許容幅 0。IdToken などと同じ。#647）
        /// - **jti の使い回しは見ない**（7。MAY。状態を持つので、呼び出し側の台帳で行う）
        /// - 文字列のクレームが日付の形でも、日付に変えずに読む（DateParseHandling.None。#653）
        /// - 欠けている・読めない・型の違う値は false（例外にしない）
        /// </remarks>
        /// <exception cref="ArgumentNullException">iss が null か空、auds が null か空</exception>
        public static bool VerifyByRsa(string jwtAssertion, string iss, string[] auds,
            out string sub, out string scopes, out JObject jobj, RSAParameters rsaPublicKey)
        {
            JwtAssertion.CheckExpected(iss, auds);

            return JwtAssertion.VerifyCore(jwtAssertion, new JWS_RS256_Param(rsaPublicKey), iss, auds,
                out sub, out scopes, out jobj);
        }

        /// <summary>VerifyByECDsa（期待する iss・aud と照合する）</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">期待する iss（client_id）</param>
        /// <param name="auds">期待する aud の集合（**認可サーバの issuer だけを推奨**。ほかは互換のため）</param>
        /// <param name="sub">out string（JWT の sub）</param>
        /// <param name="scopes">out string（JWT の scope。無ければ空文字）</param>
        /// <param name="jobj">out JObject（ペイロード。検証に通ったときだけ。通らなければ null）</param>
        /// <param name="eccPublicKey">ES256用のECParameters公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>VerifyByRsa（期待する iss・aud を受ける版）と同じ確認をする。</remarks>
        /// <exception cref="ArgumentNullException">iss が null か空、auds が null か空</exception>
        public static bool VerifyByECDsa(string jwtAssertion, string iss, string[] auds,
            out string sub, out string scopes, out JObject jobj, ECParameters eccPublicKey)
        {
            JwtAssertion.CheckExpected(iss, auds);

            return JwtAssertion.VerifyCore(jwtAssertion, new JWS_ES256_Param(eccPublicKey, false), iss, auds,
                out sub, out scopes, out jobj);
        }

        /// <summary>Verify（期待する iss・aud と照合する。署名の alg は、渡した検証鍵で選ぶ）</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">期待する iss（client_id）</param>
        /// <param name="auds">期待する aud の集合（**認可サーバの issuer だけを推奨**。ほかは互換のため）</param>
        /// <param name="sub">out string（JWT の sub）</param>
        /// <param name="scopes">out string（JWT の scope。無ければ空文字）</param>
        /// <param name="jobj">out JObject（ペイロード。検証に通ったときだけ。通らなければ null）</param>
        /// <param name="jws">
        /// 署名の検証に使う、クライアントの鍵（JWS_PS256_Param など）。
        /// JWK からなら JwsFactory.CreateFromJwk(alg, jwk) で作る
        /// </param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// VerifyByRsa（期待する iss・aud を受ける版）と同じ確認をする。違いは、**署名の alg を呼び出し側が選べる**ことだけである（#664）。
        /// JWK を受ける版は kty で RS256 か ES256 に決め、ヘッダの alg を見ない。
        /// FAPI 1.0 Advanced 8.6 は、PS256 か ES256 を使うとする（RS256 は使わないほうがよい）。
        /// - **ヘッダの alg が、渡した鍵の alg と違えば false**（大文字・小文字を区別する。RFC 8725 3.1）
        /// - **受ける alg は、呼び出し側が決める**（クライアントの登録の token_endpoint_auth_signing_alg など）。
        ///   ヘッダの alg を読んで鍵を作ると、照合の意味が無くなる
        /// - 渡せる鍵の型は制限しない（client_secret_jwt なら、client_secret を鍵にした JWS_HS256 などを渡す。OIDC Core 9）
        /// - **鍵の型は、クライアントの登録どおりのものだけを渡す。** 公開鍵の値を HS* の鍵に使うと、alg の取り違えになる（RFC 8725 3.1 : 鍵は 1 つの alg だけで使う）
        /// </remarks>
        /// <exception cref="ArgumentNullException">iss が null か空、auds が null か空、jws が null</exception>
        public static bool Verify(string jwtAssertion, string iss, string[] auds,
            out string sub, out string scopes, out JObject jobj, JWS jws)
        {
            JwtAssertion.CheckExpected(iss, auds);

            if (jws == null)
            {
                throw new ArgumentNullException(nameof(jws));
            }

            return JwtAssertion.VerifyCore(jwtAssertion, jws, iss, auds,
                out sub, out scopes, out jobj);
        }

        /// <summary>aud が唯一の値で、それが期待する値のいずれかと一致するか</summary>
        /// <param name="aud">aud のクレーム</param>
        /// <param name="auds">期待する aud の集合</param>
        /// <returns>文字列か、要素 1 つの配列で、その値が auds のいずれかと一致すれば true</returns>
        /// <remarks>draft-ietf-oauth-rfc7523bis 4（issuer を唯一の値とする。audience injection の対策）。</remarks>
        private static bool IsSoleAud(JToken aud, string[] auds)
        {
            if (aud != null && aud.Type == JTokenType.Array)
            {
                JArray array = (JArray)aud;
                if (array.Count != 1)
                {
                    // 値が無い・複数
                    return false;
                }
                aud = array[0];
            }

            return aud != null && aud.Type == JTokenType.String && CmnJwtToken.ContainsAud(aud, auds);
        }

        /// <summary>期待する iss・aud の引数を確かめる</summary>
        /// <param name="iss">期待する iss</param>
        /// <param name="auds">期待する aud の集合</param>
        private static void CheckExpected(string iss, string[] auds)
        {
            if (string.IsNullOrEmpty(iss))
            {
                throw new ArgumentNullException(nameof(iss));
            }

            if (auds == null || auds.Length == 0 || Array.TrueForAll(auds, x => string.IsNullOrEmpty(x)))
            {
                throw new ArgumentNullException(nameof(auds));
            }
        }

        /// <summary>期待する iss・aud を受ける版の本体</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="jws">検証鍵（ヘッダの alg と、この鍵の alg が一致すること。#664）</param>
        /// <param name="iss">期待する iss</param>
        /// <param name="auds">期待する aud の集合</param>
        /// <param name="sub">out string</param>
        /// <param name="scopes">out string</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        private static bool VerifyCore(string jwtAssertion, JWS jws, string iss, string[] auds,
            out string sub, out string scopes, out JObject jobj)
        {
            sub = "";
            scopes = "";
            jobj = null;

            if (!CmnJwtToken.TryVerifySignature(jws, jwtAssertion))
            {
                // JWTの署名検証に失敗
                return false;
            }

            JObject payload = CmnJwtToken.ReadPayloadObject(jwtAssertion);

            if (payload == null)
            {
                // ペイロードが JSON のオブジェクトでない
                return false;
            }

            string actualSub = CmnJwtToken.ReadStringClaim(payload, OAuth2AndOIDCConst.sub);

            if (!string.Equals(CmnJwtToken.ReadStringClaim(payload, OAuth2AndOIDCConst.iss), iss, StringComparison.Ordinal) ||
                actualSub == null ||
                !JwtAssertion.IsSoleAud(payload[OAuth2AndOIDCConst.aud], auds))
            {
                // iss が違う・sub が無い・aud が違う（無い）
                return false;
            }

            long unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            if (!CmnJwtToken.VerifyExpClaim(payload[OAuth2AndOIDCConst.exp], unixTimeSeconds))
            {
                // 期限切れ・読めない・無い
                return false;
            }

            JToken nbf = payload[OAuth2AndOIDCConst.nbf];
            if (nbf != null && !CmnJwtToken.VerifyNbfClaim(nbf, unixTimeSeconds))
            {
                // まだ使えない・読めない
                return false;
            }

            sub = actualSub;
            scopes = CmnJwtToken.ReadStringClaim(payload, OAuth2AndOIDCConst.scope) ?? "";
            jobj = payload;
            return true;
        }

        #endregion
    }
}
