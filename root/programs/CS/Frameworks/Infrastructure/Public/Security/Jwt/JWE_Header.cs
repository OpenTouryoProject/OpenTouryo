
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
//* クラス名        ：JWE_Header
//* クラス日本語名  ：JWE Header
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/01/29  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWE Header</summary>
    public class JWE_Header
    {
        /// <summary>
        /// alg=RSA-OAEP
        /// JWEの鍵交換アルゴリズムを指定する。
        /// A256GCMの鍵交換アルゴリズムのみサポート。
        /// </summary>
        public string alg = "";

        /// <summary>
        /// alg=A256GCM
        /// JWEの暗号化アルゴリズムを指定する。
        /// A256GCMの暗号化アルゴリズムのみサポート。
        /// </summary>
        public string enc = "";

        /// <summary>
        /// typ=JWT(JWSもJWTらしい)
        /// アサーションのタイプを指定。JWT固定。
        /// </summary>
        public readonly string typ = JwtConst.JWT;
    }
}
