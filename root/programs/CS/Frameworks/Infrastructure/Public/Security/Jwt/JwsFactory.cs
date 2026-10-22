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
//* クラス名        ：JwsFactory
//* クラス日本語名  ：alg から JWS（署名・検証器）を作る
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の D / G-8）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>alg から JWS（署名・検証器）を作る</summary>
    /// <remarks>
    /// JWS は alg ごとに型が分かれている（JWS_RS256 〜 JWS_ES512）ので、
    /// ヘッダの alg の文字列から 1 本で引く口が無かった（#634）。
    /// **alg と型の対応は、ここだけが持つ。**
    ///
    /// 扱うのは非対称鍵の alg（RS* / PS* / ES*）だけである。
    /// HS*（共通鍵）と none は扱わない（相手の公開鍵で検証する場面のための部品であるため）。
    ///
    /// | alg | kty | crv |
    /// |---|---|---|
    /// | RS256 / RS384 / RS512 / PS256 / PS384 / PS512 | RSA | （無し。1 本の鍵で 6 つ） |
    /// | ES256 | EC | P-256 |
    /// | ES384 | EC | P-384 |
    /// | ES512 | EC | P-521 |
    /// </remarks>
    public class JwsFactory
    {
        #region 表

        /// <summary>扱う alg（並びは広告にもそのまま使える）</summary>
        private static readonly string[] _supportedAlgs = new string[]
        {
            JwtConst.RS256, JwtConst.RS384, JwtConst.RS512,
            JwtConst.PS256, JwtConst.PS384, JwtConst.PS512,
            JwtConst.ES256, JwtConst.ES384, JwtConst.ES512
        };

        /// <summary>扱う alg の一覧</summary>
        /// <remarks>
        /// 自由に利用できる。
        /// 毎回、新しい配列を返す（呼び出し側が書き換えても表は変わらない）。
        /// </remarks>
        public static string[] SupportedAlgs
        {
            get { return (string[])JwsFactory._supportedAlgs.Clone(); }
        }

        /// <summary>扱う alg か</summary>
        /// <param name="alg">alg</param>
        /// <returns>扱うなら true</returns>
        /// <remarks>自由に利用できる。大文字小文字を区別する（RFC 7515 4.1.1）。</remarks>
        public static bool IsSupported(string alg)
        {
            return !string.IsNullOrEmpty(alg)
                && Array.IndexOf(JwsFactory._supportedAlgs, alg) >= 0;
        }

        /// <summary>alg が要る kty</summary>
        /// <param name="alg">alg</param>
        /// <returns>RSA / EC（扱わない alg なら null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetKty(string alg)
        {
            if (!JwsFactory.IsSupported(alg))
            {
                return null;
            }

            return (JwsFactory.GetCrv(alg) != null) ? JwtConst.EC : JwtConst.RSA;
        }

        /// <summary>alg が要る crv</summary>
        /// <param name="alg">alg</param>
        /// <returns>P-256 / P-384 / P-521（RSA と扱わない alg なら null）</returns>
        /// <remarks>自由に利用できる。EC は alg と曲線が対応する（RFC 7518 3.4）。</remarks>
        public static string GetCrv(string alg)
        {
            if (alg == JwtConst.ES256) { return JwtConst.P256; }
            if (alg == JwtConst.ES384) { return JwtConst.P384; }
            if (alg == JwtConst.ES512) { return JwtConst.P521; }

            return null;
        }

        /// <summary>JWK の鍵が、alg に合っているか</summary>
        /// <param name="jwk">JWK</param>
        /// <param name="alg">ヘッダの alg</param>
        /// <returns>合っていれば true</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// **アルゴリズムの取り違えを防ぐ。** 鍵の材料を検証器に渡す前に確かめる。
        /// EC は crv まで見る。kty だけでは、P-256 の鍵で ES512 を名乗る JWS を通してしまう。
        /// JWK の alg（RFC 7517 4.4。任意）とは照合しない（RSA の 1 本の鍵を RS* / PS* で共有するため）。
        /// </remarks>
        public static bool IsSameKeyType(JObject jwk, string alg)
        {
            string kty = JwsFactory.GetKty(alg);

            if (jwk == null || kty == null)
            {
                return false;
            }

            if (JwsFactory.GetString(jwk, JwtConst.kty) != kty)
            {
                return false;
            }

            string crv = JwsFactory.GetCrv(alg);
            return crv == null || JwsFactory.GetString(jwk, JwtConst.crv) == crv;
        }

        #endregion

        #region 検証器（公開鍵）

        /// <summary>JWK（公開鍵）から、検証する JWS を作る</summary>
        /// <param name="alg">alg</param>
        /// <param name="jwk">JWK（公開鍵）</param>
        /// <returns>JWS（扱わない alg・鍵が合わない・読めない場合は null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// jwks_uri から kid で引いた鍵や、DPoP の証明のヘッダの jwk を検証に使う経路である。
        /// IsSameKeyType で鍵と alg の対応を確かめてから作る。
        /// 外から来る値を扱うため、読めなければ例外にせず null を返す。
        /// </remarks>
        public static JWS CreateFromJwk(string alg, JObject jwk)
        {
            if (!JwsFactory.IsSameKeyType(jwk, alg))
            {
                return null;
            }

            try
            {
                if (JwsFactory.GetKty(alg) == JwtConst.RSA)
                {
                    // RSA の 6 つは同じ鍵なので、変換器のハッシュは結果に影響しない。
                    RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter(JWS_RSA.RS._256);
                    return JwsFactory.CreateFromRsaParameters(alg, rpkc.JwkToParam(jwk));
                }
                else
                {
                    EccPublicKeyConverter epkc = new EccPublicKeyConverter(JwsFactory.GetES(alg));
                    return JwsFactory.CreateFromEcParameters(alg, epkc.JwkToParam(jwk));
                }
            }
            catch (Exception)
            {
                // 項目が無い・BASE64URL でない（外から来た値なので、例外にしない）。
                return null;
            }
        }

        /// <summary>RSA の鍵のパラメタから、JWS を作る</summary>
        /// <param name="alg">alg（RS* / PS*）</param>
        /// <param name="param">RSAParameters（秘密鍵を含めれば署名もできる）</param>
        /// <returns>JWS（alg が RSA でなければ null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static JWS CreateFromRsaParameters(string alg, RSAParameters param)
        {
            if (alg == JwtConst.RS256) { return new JWS_RS256_Param(param); }
            if (alg == JwtConst.RS384) { return new JWS_RS384_Param(param); }
            if (alg == JwtConst.RS512) { return new JWS_RS512_Param(param); }
            if (alg == JwtConst.PS256) { return new JWS_PS256_Param(param); }
            if (alg == JwtConst.PS384) { return new JWS_PS384_Param(param); }
            if (alg == JwtConst.PS512) { return new JWS_PS512_Param(param); }

            return null;
        }

        /// <summary>EC の鍵のパラメタから、JWS を作る</summary>
        /// <param name="alg">alg（ES*）</param>
        /// <param name="param">ECParameters</param>
        /// <param name="isPrivate">秘密鍵か（署名するなら true）</param>
        /// <returns>JWS（alg が EC でなければ null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static JWS CreateFromEcParameters(string alg, ECParameters param, bool isPrivate = false)
        {
            if (alg == JwtConst.ES256) { return new JWS_ES256_Param(param, isPrivate); }
            if (alg == JwtConst.ES384) { return new JWS_ES384_Param(param, isPrivate); }
            if (alg == JwtConst.ES512) { return new JWS_ES512_Param(param, isPrivate); }

            return null;
        }

        /// <summary>証明書（*.cer）から、検証する JWS を作る</summary>
        /// <param name="alg">alg</param>
        /// <param name="cerFilePath">*.cer のパス</param>
        /// <returns>JWS（扱わない alg なら null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 読めないパスでは例外になる（設定の誤りを、黙って「検証できない」にしないため）。
        /// </remarks>
        public static JWS CreateFromCertificate(string alg, string cerFilePath)
        {
            return JwsFactory.CreateFromX509(alg, cerFilePath, "");
        }

        #endregion

        #region 署名器（秘密鍵）

        /// <summary>JWK（秘密鍵）から、署名する JWS を作る</summary>
        /// <param name="alg">alg</param>
        /// <param name="jwk">JWK（秘密鍵）</param>
        /// <returns>JWS（扱わない alg・鍵が合わない場合は null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// DPoP の証明や private_key_jwt のアサーションを、クライアントが自分の鍵で作るときに使う。
        /// 自分の鍵なので、読めなければ例外にする。
        /// </remarks>
        public static JWS CreateFromPrivateJwk(string alg, JObject jwk)
        {
            if (!JwsFactory.IsSameKeyType(jwk, alg))
            {
                return null;
            }

            if (JwsFactory.GetString(jwk, JwtConst.d) == null)
            {
                throw new ArgumentException(PublicExceptionMessage.ARGUMENT_INCORRECT, nameof(jwk));
            }

            if (JwsFactory.GetKty(alg) == JwtConst.RSA)
            {
                RsaPrivateKeyConverter rpkc = new RsaPrivateKeyConverter(JWS_RSA.RS._256);
                return JwsFactory.CreateFromRsaParameters(alg, rpkc.JwkToParam(jwk));
            }
            else
            {
                Dictionary<string, string> dic = new Dictionary<string, string>()
                {
                    { JwtConst.crv, JwsFactory.GetString(jwk, JwtConst.crv) },
                    { JwtConst.x, JwsFactory.GetString(jwk, JwtConst.x) },
                    { JwtConst.y, JwsFactory.GetString(jwk, JwtConst.y) },
                    { JwtConst.d, JwsFactory.GetString(jwk, JwtConst.d) }
                };

                EccPrivateKeyConverter epkc = new EccPrivateKeyConverter(JwsFactory.GetES(alg));
                return JwsFactory.CreateFromEcParameters(alg, epkc.JwkToParam(dic), true);
            }
        }

        /// <summary>証明書（*.pfx）から、署名する JWS を作る</summary>
        /// <param name="alg">alg</param>
        /// <param name="pfxFilePath">*.pfx のパス</param>
        /// <param name="password">*.pfx のパスワード</param>
        /// <returns>JWS（扱わない alg なら null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static JWS CreateFromPfx(string alg, string pfxFilePath, string password)
        {
            return JwsFactory.CreateFromX509(alg, pfxFilePath, password);
        }

        #endregion

        #region private

        /// <summary>証明書から JWS を作る</summary>
        /// <param name="alg">alg</param>
        /// <param name="path">証明書のパス</param>
        /// <param name="password">パスワード（*.cer なら空）</param>
        /// <returns>JWS（扱わない alg なら null）</returns>
        private static JWS CreateFromX509(string alg, string path, string password)
        {
            if (alg == JwtConst.RS256) { return new JWS_RS256_X509(path, password); }
            if (alg == JwtConst.RS384) { return new JWS_RS384_X509(path, password); }
            if (alg == JwtConst.RS512) { return new JWS_RS512_X509(path, password); }
            if (alg == JwtConst.PS256) { return new JWS_PS256_X509(path, password); }
            if (alg == JwtConst.PS384) { return new JWS_PS384_X509(path, password); }
            if (alg == JwtConst.PS512) { return new JWS_PS512_X509(path, password); }
            if (alg == JwtConst.ES256) { return new JWS_ES256_X509(path, password); }
            if (alg == JwtConst.ES384) { return new JWS_ES384_X509(path, password); }
            if (alg == JwtConst.ES512) { return new JWS_ES512_X509(path, password); }

            return null;
        }

        /// <summary>EC の alg に対応する JWS_ECDSA.ES</summary>
        /// <param name="alg">alg（ES*）</param>
        /// <returns>JWS_ECDSA.ES</returns>
        private static JWS_ECDSA.ES GetES(string alg)
        {
            if (alg == JwtConst.ES384) { return JWS_ECDSA.ES._384; }
            if (alg == JwtConst.ES512) { return JWS_ECDSA.ES._512; }

            return JWS_ECDSA.ES._256;
        }

        /// <summary>文字列の項目を読む（文字列でなければ null）</summary>
        /// <param name="jwk">JWK</param>
        /// <param name="name">項目名</param>
        /// <returns>値</returns>
        private static string GetString(JObject jwk, string name)
        {
            JToken token = jwk[name];
            return (token != null && token.Type == JTokenType.String) ? (string)token : null;
        }

        #endregion
    }
}
