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
//* クラス名        ：JWS_HS384
//* クラス日本語名  ：HS384 JWS生成
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/13  西野 大介         新規作成
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2017/12/25  西野 大介         暗号化ライブラリ追加に伴うコード追加・修正
//*  2018/08/15  西野 大介         jwks_uri & kid 対応
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>HS384 JWS生成クラス</summary>
    public class JWS_HS384 : JWS_HMACSHA
    {
        #region mem & prop & constructor
        
        /// <summary>Constructor</summary>
        /// <param name="key">byte[]</param>
        public JWS_HS384(byte[] key)
        {
            this.JwtConstHSnnn = JwtConst.HS384;
            base.Init(key);
        }

        /// <summary>Constructor</summary>
        /// <param name="jwkString">string</param>
        public JWS_HS384(string jwkString)
        {
            this.JwtConstHSnnn = JwtConst.HS384;
            base.Init(jwkString);
        }

        #endregion

        #region HS384署名・検証

        /// <summary>HMACSHA384生成</summary>
        /// <param name="key">byte[]</param>
        /// <returns>HMACSHA384</returns>
        public override HMAC CreateHMACSHA(byte[] key)
        {
            return new HMACSHA384(key);
        }

        #endregion
    }
}
