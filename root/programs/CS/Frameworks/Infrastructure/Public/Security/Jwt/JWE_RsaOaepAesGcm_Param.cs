﻿//**********************************************************************************
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
//* クラス名        ：JWE_RsaOaepAesGcm_Param
//* クラス日本語名  ：ParamによるJWE RSAES-OAEP and AES GCM生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/01/30  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>ParamによるJWE RSAES-OAEP and AES GCM生成クラス</summary>
    public class JWE_RsaOaepAesGcm_Param : JWE_RsaOaepAesGcm
    {
        #region mem & prop & constructor

        /// <summary>Constructor</summary>
        /// <param name="algorithm">EnumASymmetricAlgorithm</param>
        public JWE_RsaOaepAesGcm_Param(EnumASymmetricAlgorithm algorithm)
        {
            this.ASymmetricCryptography = new ASymmetricCryptography(algorithm);
        }

        /// <summary>Constructor</summary>
        /// <param name="algorithm">EnumASymmetricAlgorithm</param>
        /// <param name="param">RSAParameters</param>
        public JWE_RsaOaepAesGcm_Param(EnumASymmetricAlgorithm algorithm, RSAParameters param)
        {
            this.ASymmetricCryptography = new ASymmetricCryptography(algorithm);
            ((RSA)ASymmetricCryptography.AsymmetricAlgorithm).ImportParameters(param);
        }

        #endregion
    }
}
