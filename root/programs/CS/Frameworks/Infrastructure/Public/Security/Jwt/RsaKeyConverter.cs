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
//* クラス名        ：RsaKeyConverter
//* クラス日本語名  ：RsaKeyConverterクラス
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
    /// <summary>RSA関係のカギ変換処理を実装する。</summary>
    public abstract class RsaKeyConverter
    {
        #region mem & prop
        /// <summary>アルゴリズム</summary>
        protected JWS_RSA.RS RSnnn = JWS_RSA.RS._256;
        /// <summary>アルゴリズム</summary>
        protected string JwtConstRSnnn = JwtConst.RS256;
        /// <summary>アルゴリズム</summary>
        protected string HashName = HashNameConst.SHA256;
        /// <summary>アルゴリズム</summary>
        protected EnumHashAlgorithm HashAlgorithm = EnumHashAlgorithm.SHA256;
        #endregion

        #region constructor
        /// <summary>constructor</summary>
        /// <param name="rsNNN">JWS_RSA.RS</param>
        public RsaKeyConverter(JWS_RSA.RS rsNNN)
        {
            this.RSnnn = rsNNN;

            switch (this.RSnnn)
            {
                case JWS_RSA.RS._256:
                    this.JwtConstRSnnn = JwtConst.RS256;
                    this.HashName = HashNameConst.SHA256;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA256;
                    break;

                case JWS_RSA.RS._384:
                    this.JwtConstRSnnn = JwtConst.RS384;
                    this.HashName = HashNameConst.SHA384;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA384;
                    break;

                case JWS_RSA.RS._512:
                    this.JwtConstRSnnn = JwtConst.RS512;
                    this.HashName = HashNameConst.SHA512;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA512;
                    break;
            }
        }
        #endregion
    }
}
