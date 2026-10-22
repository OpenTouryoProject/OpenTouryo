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
//* クラス名        ：JWS_Header
//* クラス日本語名  ：JWS Header
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/16  西野 大介         新規作成
//*  2026/10/15  玄人 幸道         typ を設定できるようにした（readonly を外した。#611）
//*  2026/10/22  玄人 幸道         jwk を追加（DPoP の証明はヘッダに公開鍵を持つ。#634）
//**********************************************************************************

using Newtonsoft.Json.Linq;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWS Header</summary>
    public class JWS_Header
    {
        /// <summary>
        /// alg=HS256 or RS256 or ES256
        /// </summary>
        public string alg = "";

        /// <summary>
        /// jku=JWK Set URL
        /// </summary>
        public string jku = null;

        /// <summary>
        /// kid=Jwk鍵のID
        /// </summary>
        public string kid = null;

        /// <summary>
        /// typ=JWT(JWSもJWTらしい)
        /// アサーションのタイプを指定。既定は JWT。
        /// </summary>
        /// <remarks>
        /// 署名の前に設定できる（例: RFC 9068 の access_token なら JwtConst.AT_JWT）。（#611）
        /// null にすると、ヘッダに typ を出さない。
        /// 以前は readonly で、設定できず、検証時にもヘッダの値が読み込まれなかった。
        /// </remarks>
        public string typ = JwtConst.JWT;

        /// <summary>
        /// jwk=署名に使った鍵の公開鍵（RFC 7515 4.1.3）
        /// </summary>
        /// <remarks>
        /// DPoP の証明（RFC 9449 4.2）が使う。既定は null で、ヘッダに出さない。（#634）
        /// 型を JToken にしているのは、検証時の読み込みで、
        /// オブジェクトでない値が来ても例外にしないため（従来どおり Verify が判定する）。
        /// **公開鍵だけを入れること**（秘密鍵の項目を入れると、そのまま送られる）。
        /// </remarks>
        public JToken jwk = null;
    }
}
