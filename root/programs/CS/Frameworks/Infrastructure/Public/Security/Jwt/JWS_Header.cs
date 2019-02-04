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
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWS Header</summary>
    public class JWS_Header
    {
        /// <summary>
        /// alg=HS256 or RS256
        /// JWSのデジタル署名アルゴリズムを指定する。
        /// HS256 or RS256の署名アルゴリズムのみサポート。
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
        /// アサーションのタイプを指定。JWT固定。
        /// </summary>
        public readonly string typ = JwtConst.JWT;
    }
}
