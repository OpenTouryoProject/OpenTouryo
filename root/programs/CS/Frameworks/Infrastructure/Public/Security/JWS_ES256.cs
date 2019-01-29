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
//* クラス名        ：JWS_ES256
//* クラス日本語名  ：JWS ES256生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/01/28  西野 大介         新規作成
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// JWS ES256生成クラス
    /// ES256(ECDSA using P-256 and SHA-256)
    /// </summary>
    public abstract class JWS_ES256 : JWS
    {
        /// <summary>_JWSHeader</summary>
        private JWS_Header _JWSHeader = new JWS_Header
        {
            alg = JwtConst.ES256
        };

        /// <summary>JWSHeader</summary>
        public JWS_Header JWSHeader
        {
            protected set
            {
                this._JWSHeader = value;
            }

            get
            {
                return this._JWSHeader;
            }
        }

        /// <summary>EnumDigitalSignAlgorithm</summary>
        /// <remarks>Constructorで使うのでstaticとなった</remarks>
        public static EnumDigitalSignAlgorithm DigitalSignAlgorithm
        {
            get
            {
#if NETSTD
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    return EnumDigitalSignAlgorithm.ECDsaCng_P256;
                }
                else
                {
                    return EnumDigitalSignAlgorithm.ECDsaOpenSsl_P256;
                }
#else
                return EnumDigitalSignAlgorithm.ECDsaCng_P256;
#endif
            }
        }
    }
}
