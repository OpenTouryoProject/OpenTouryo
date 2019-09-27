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
//* クラス名        ：JwtConst
//* クラス日本語名  ：JwtConst
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/30  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWT系定数</summary>
    public class JwtConst
    {
        /// <summary>type:JWT</summary>
        public const string JWT = "JWT";

        #region JWS

        #region HS
        /// <summary>alg:HS256</summary>
        public const string HS256 = "HS256";
        /// <summary>alg:HS384</summary>
        public const string HS384 = "HS384";
        /// <summary>alg:HS512</summary>
        public const string HS512 = "HS512";
        #endregion

        #region RS
        /// <summary>alg:RS256</summary>
        public const string RS256 = "RS256";
        /// <summary>alg:RS384</summary>
        public const string RS384 = "RS384";
        /// <summary>alg:RS512</summary>
        public const string RS512 = "RS512";
        #endregion

        #region ES
        /// <summary>alg:ES256</summary>
        public const string ES256 = "ES256";
        /// <summary>alg:ES384</summary>
        public const string ES384 = "ES384";
        /// <summary>alg:ES512</summary>
        public const string ES512 = "ES512";
        #endregion

        #endregion

        #region JWE

        #region RSAES-OAEP and AES GCM
        /// <summary>alg:RSA-OAEP</summary>
        public const string RSA_OAEP = "RSA-OAEP";

        /// <summary>enc:A256GCM</summary>
        public const string A256GCM = "A256GCM";
        #endregion

        #region RSAES-PKCS1-v1_5 and AES_128_CBC_HMAC_SHA_256
        /// <summary>alg:RSA1_5</summary>
        public const string RSA1_5 = "RSA1_5";

        /// <summary>enc:A128CBC-HS256</summary>
        public const string A128CBC_HS256 = "A128CBC-HS256";
        #endregion

        #endregion

        #region JWK

        /// <summary>keys</summary>
        public const string keys = "keys";

        /// <summary>kid: Key IDパラメタ</summary>
        public const string kid = "kid";

        /// <summary>alg: algorithmパラメタ</summary>
        public const string alg = "alg";

        /// <summary>kty: key typeパラメタ</summary>
        public const string kty = "kty";

        /// <summary>use : key useパラメタ</summary>
        public const string use = "use";

        #region Keys

        #region Symmetric Keys

        /// <summary>Symmetric Keys: k</summary>
        public const string k = "k";

        #endregion

        #region ASymmetric Keys

        #region RSA Keys

        /// <summary>kty : RSA</summary>
        public const string RSA = "RSA";

        #region RSA Public Keys

        /// <summary>RSA Public Key: n</summary>
        public const string n = "n";

        /// <summary>RSA Public Key: e</summary>
        public const string e = "e";

        #endregion

        #region RSA Private Keys

        /// <summary>
        /// d :
        /// - RSA Private Key
        /// - ECC Private Key
        /// </summary>
        public const string d = "d";

        /// <summary>p : RSA Private Key</summary>
        public const string p = "p";

        /// <summary>q : RSA Private Key</summary>
        public const string q = "q";

        /// <summary>dp : RSA Private Key</summary>
        public const string dp = "dp";

        /// <summary>dq : RSA Private Key</summary>
        public const string dq = "dq";

        /// <summary>qi : RSA Private Key</summary>
        public const string qi = "qi";

        /// <summary>oth : RSA Private Key</summary>
        public const string oth = "oth";

        /// <summary>r(oth) : RSA Private Key</summary>
        public const string oth_r = "r";

        /// <summary>d(oth) : RSA Private Key</summary>
        public const string oth_d = "d";

        /// <summary>t(oth) : RSA Private Key</summary>
        public const string oth_t = "t";

        #endregion

        #endregion

        #region ECC  Keys

        /// <summary>ktyパラメタ値 : EC</summary>
        public const string EC = "EC";

        /// <summary>crv : Curveパラメタ</summary>
        public const string crv = "crv";

        /// <summary>Curveパラメタ値 : P-256</summary>
        public const string P256 = "P-256";

        /// <summary>Curveパラメタ値 : P-384</summary>
        public const string P384 = "P-384";

        /// <summary>Curveパラメタ値 : P-521</summary>
        public const string P521 = "P-521";

        #region ECC Public Keys
        /// <summary>x : X Coordinateパラメタ</summary>
        public const string x = "x";

        /// <summary>y : Y Coordinateパラメタ</summary>
        public const string y = "y";
        #endregion

        #region ECC Private Keys
        ///// <summary>d : ECC Private Keyパラメタ</summary>
        //public const string d = "d";　// RSAと重複
        #endregion

        #endregion

        #endregion

        #endregion

        #endregion
    }
}

