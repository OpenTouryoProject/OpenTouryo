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
//* クラス名        ：JWT_RS256
//* クラス日本語名  ：JWT(JWS)RS256生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//*  2017/12/25  西野 大介         暗号化ライブラリ追加に伴うコード追加・修正
//**********************************************************************************

using System.Security.Cryptography.X509Certificates;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>JWT(JWS)RS256生成クラス（X.509証明書による）</summary>
    /// <remarks>後方互換維持のため</remarks>
    public class JWT_RS256 : JWT_RS256_X509
    {
        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">DigitalSignX509に渡すcertificateFilePathパラメタ</param>
        /// <param name="password">DigitalSignX509に渡すpasswordパラメタ</param>
        public JWT_RS256(string certificateFilePath, string password)
            : this(certificateFilePath, password, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">DigitalSignX509に渡すcertificateFilePathパラメタ</param>
        /// <param name="password">DigitalSignX509に渡すpasswordパラメタ</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public JWT_RS256(string certificateFilePath, string password, X509KeyStorageFlags flag)
            : base(certificateFilePath, password, flag) { }
    }
}
