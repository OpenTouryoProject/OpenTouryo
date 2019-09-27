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
//* クラス名        ：JWS_RS384_XML
//* クラス日本語名  ：XMLによるJWS RS384生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/25  西野 大介         新規作成（分割
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>XMLによるJWS RS384生成クラス</summary>
    public class JWS_RS384_XML : JWS_RS384
    {
        #region mem & prop & constructor

        /// <summary>公開鍵</summary>
        public string XMLPublicKey { get; protected set; }

        /// <summary>秘密鍵</summary>
        public string XMLPrivateKey { get; protected set; }

        /// <summary>DigitalSignXML</summary>
        private DigitalSignXML DigitalSignXML = null;

        /// <summary>Constructor</summary>
        public JWS_RS384_XML()
        {
            this.DigitalSignXML = new DigitalSignXML(JWS_RS384.DigitalSignAlgorithm);

            this.XMLPrivateKey = this.DigitalSignXML.PrivateKey;
            this.XMLPublicKey = this.DigitalSignXML.PublicKey;
        }

        /// <summary>Constructor</summary>
        /// <param name="xmlKey">string</param>
        public JWS_RS384_XML(string xmlKey)
        {
            this.DigitalSignXML = new DigitalSignXML(xmlKey, JWS_RS384.DigitalSignAlgorithm);
            this.XMLPrivateKey = this.DigitalSignXML.PrivateKey;
            this.XMLPublicKey = this.DigitalSignXML.PublicKey;
        }

        #endregion

        #region RS384署名・検証

        /// <summary>Create2</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected override byte[] Create2(byte[] data)
        {
            return this.DigitalSignXML.Sign(data);
        }

        /// <summary>Verify2</summary>
        /// <param name="data">byte[]</param>
        /// <param name="sign">byte[]</param>
        /// <returns>bool</returns>
        protected override bool Verify2(byte[] data, byte[] sign)
        {
            return this.DigitalSignXML.Verify(data, sign);
        }

        #endregion
    }
}