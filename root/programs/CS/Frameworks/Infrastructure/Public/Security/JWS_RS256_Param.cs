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
//* クラス名        ：JWS_RS256_Param
//* クラス日本語名  ：ParamによるJWS RS256生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//*  2018/08/15  西野 大介         jwks_uri & kid 対応
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//*  2019/01/29  西野 大介         リファクタリング（プロバイダ処理を末端に）
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>ParamによるJWS RS256生成クラス</summary>
    public class JWS_RS256_Param : JWS_RS256
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
        public JWS_RS256_Param()
        {
            this.DigitalSignParam = new DigitalSignParam(JWS_RS256.DigitalSignAlgorithm);
        }

        /// <summary>Constructor</summary>
        /// <param name="param">RSAParameters</param>
        public JWS_RS256_Param(RSAParameters param)
        {
            this.DigitalSignParam = new DigitalSignParam(param, JWS_RS256.DigitalSignAlgorithm);
        }

        #endregion

        #region RS256署名・検証

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
