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
//* クラス名        ：JWS_RS512_Param
//* クラス日本語名  ：ParamによるJWS RS512生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/25  西野 大介         新規作成（分割
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>ParamによるJWS RS512生成クラス</summary>
    public class JWS_RS512_Param : JWS_RS512
    {
        #region mem & prop & constructor

        /// <summary>DigitalSignParam</summary>
        private DigitalSignParam DigitalSignParam { get; set; }

        /// <summary>秘密鍵のRSAParameters</summary>
        public RSAParameters RsaPrivateParameters
        {
            get
            {
                return ((RSA)this.DigitalSignParam.AsymmetricAlgorithm).ExportParameters(true);
            }
        }

        /// <summary>公開鍵のRSAParameters</summary>
        public RSAParameters RsaPublicParameters
        {
            get
            {
                return ((RSA)this.DigitalSignParam.AsymmetricAlgorithm).ExportParameters(false);
            }
        }

        /// <summary>Constructor</summary>
        public JWS_RS512_Param()
        {
            this.DigitalSignParam = new DigitalSignParam(JWS_RS512.DigitalSignAlgorithm);
        }

        /// <summary>Constructor</summary>
        /// <param name="param">RSAParameters</param>
        public JWS_RS512_Param(RSAParameters param)
        {
            this.DigitalSignParam = new DigitalSignParam(param, JWS_RS512.DigitalSignAlgorithm);
        }

        #endregion

        #region RS512署名・検証

        /// <summary>Create2</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected override byte[] Create2(byte[] data)
        {
            return this.DigitalSignParam.Sign(data);
        }

        /// <summary>Verify2</summary>
        /// <param name="data">byte[]</param>
        /// <param name="sign">byte[]</param>
        /// <returns>bool</returns>
        protected override bool Verify2(byte[] data, byte[] sign)
        {
            return this.DigitalSignParam.Verify(data, sign);
        }

        #endregion
    }
}
