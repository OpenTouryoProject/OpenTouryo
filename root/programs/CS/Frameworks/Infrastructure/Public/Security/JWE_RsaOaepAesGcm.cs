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
//* クラス名        ：JWE_RsaOaepAesGcm
//* クラス日本語名  ：JWE RSAES-OAEP and AES GCM生成クラス
//*
//*                  RFC 7516 - JSON Web Encryption (JWE)
//*                  > A.1.  Example JWE using RSAES-OAEP and AES GCM
//*                  https://tools.ietf.org/html/rfc7516#appendix-A.1.1 
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/01/29  西野 大介         新規作成
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>JWE RSAES-OAEP and AES GCM生成クラス</summary>
    public abstract class JWE_RsaOaepAesGcm : JWE
    {
        /// <summary>_JWEHeader</summary>
        private JWE_Header _JWEHeader = new JWE_Header
        {
            alg = JwtConst.RSA_OAEP,
            enc = JwtConst.A256GCM
        };

        /// <summary>JWEHeader</summary>
        public JWE_Header JWEHeader
        {
            protected set
            {
                this._JWEHeader = value;
            }

            get
            {
                return this._JWEHeader;
            }
        }
    }
}
