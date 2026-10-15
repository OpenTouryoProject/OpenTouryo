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
//* クラス名        ：JWS
//* クラス日本語名  ：JWS抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2017/12/25  西野 大介         暗号化ライブラリ追加に伴うコード追加・修正
//*  2018/08/15  西野 大介         jwks_uri & kid 対応
//*  2026/10/15  玄人 幸道         typ を照合する Verify(jwtString, typ) を追加（#611）
//*  2026/10/15  玄人 幸道         typ の読み取りと正規化を JWE からも使えるよう internal にした（#615）
//*  2026/10/16  玄人 幸道         JWSHeader を派生（JWS_RSA / JWS_ECDSA / JWS_HMACSHA）から移した（#615）
//**********************************************************************************

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWS</summary>
    public abstract class JWS
    {
        /// <summary>JWSHeader</summary>
        /// <remarks>
        /// 署名の前に typ などを設定できる（#611）。
        /// alg は方式ごとに決まるので、派生（JWS_RSA / JWS_ECDSA / JWS_HMACSHA）の Init で作る。
        /// 以前は派生ごとに持っていた（JWS_HMACSHA には無かった）のを、基底に移した（#615）。
        /// </remarks>
        public JWS_Header JWSHeader { get; protected set; }

        /// <summary>JWS生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWSの文字列表現</returns>
        public abstract string Create(string payloadJson);

        /// <summary>JWS検証メソッド</summary>
        /// <param name="jwtString">JWSの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        /// <remarks>
        /// **typ は照合しない。**（#611）
        /// 以前は JWS_Header.typ が readonly で、ヘッダの typ が読み込まれず常に "JWT" に見えたため、
        /// typ の照合は実際には効いていなかった。その振る舞いをそのまま保つ。
        /// typ を照合したいときは Verify(jwtString, typ) を使う。
        /// </remarks>
        public abstract bool Verify(string jwtString);

        /// <summary>
        /// JWS検証メソッド（typ の照合パート）
        /// 本体の検証は派生で定義されるVerify(string jwtString)で行う。
        /// </summary>
        /// <param name="jwtString">JWSの文字列表現</param>
        /// <param name="typ">
        /// 期待する typ（例: JwtConst.AT_JWT、JwtConst.JWT）。
        /// null または空文字なら照合せず、Verify(jwtString) と同じ。
        /// </param>
        /// <returns>署名と typ の検証結果</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 照合の規則（RFC 7515 4.1.9）
        /// - 大文字・小文字を区別しない
        /// - "/" を含まない値は "application/" を前に付けて比べる
        /// - （"at+jwt" と "application/at+jwt" は同じ）
        /// - **ヘッダに typ が無ければ不一致**（期待値を指定したときだけ）
        /// RFC 9068 のリソース サーバは、access_token を JwtConst.AT_JWT で検証する。
        /// </remarks>
        public bool Verify(string jwtString, string typ)
        {
            if (string.IsNullOrEmpty(typ))
            {
                return this.Verify(jwtString);
            }

            string actual = JWS.GetHeaderTyp(jwtString);

            if (actual == null || JWS.NormalizeTyp(actual) != JWS.NormalizeTyp(typ))
            {
                return false;
            }

            return this.Verify(jwtString);
        }

        /// <summary>ヘッダの typ を、ヘッダの JSON から直接読む</summary>
        /// <param name="jwtString">JWS / JWE の文字列表現（先頭の区切りがヘッダ）</param>
        /// <returns>typ（無い、または文字列でなければ null）</returns>
        /// <remarks>
        /// JWS_Header に読み込むと、typ が無いときも初期値（"JWT"）に見えてしまうため、
        /// 「無い」を見分けるには JSON を直接読む必要がある。
        /// JWE.Decrypt(jwtString, typ, out payloadJson) からも使う（#615）。
        /// </remarks>
        internal static string GetHeaderTyp(string jwtString)
        {
            string header = jwtString.Split('.')[0];
            JObject headerObject = JObject.Parse(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(header), CustomEncode.UTF_8));

            JToken typ = headerObject["typ"];
            return (typ != null && typ.Type == JTokenType.String) ? (string)typ : null;
        }

        /// <summary>typ を比較用に正規化する（RFC 7515 4.1.9）</summary>
        /// <param name="typ">typ</param>
        /// <returns>正規化した typ</returns>
        internal static string NormalizeTyp(string typ)
        {
            string t = typ.Trim();
            if (t.IndexOf('/') < 0)
            {
                t = "application/" + t;
            }
            return t.ToLowerInvariant();
        }
    }
}
