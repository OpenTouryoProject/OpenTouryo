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
//* クラス名        ：EcdhKeyExchange
//* クラス日本語名  ：ECDHのキー交換抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security.KeyExg
{
    /// <summary>ECDHのキー交換基本クラス</summary>
    public abstract class EcdhKeyExchange : BaseKeyExchange
    {
        /// <summary>ECDiffieHellmanプロバイダ情報の取得用</summary>
        public ECDiffieHellman ECDiffieHellman
        {
            get
            {
                return (ECDiffieHellman)this._asa;
            }
        }
        
        // 暗号化・復号化に使用する秘密鍵

        /// <summary>暗号化・復号化に使用する秘密鍵</summary>
        protected byte[] _privateKey;
        /// <summary>暗号化・復号化に使用する秘密鍵</summary>
        public byte[] PrivateKey
        {
            get
            {
                return this._privateKey;
            }
        }

        /// <summary>キー・マテリアルを派生</summary>
        /// <param name="exchangeKeyOfPartner">相方の交換鍵</param>
        public void DeriveKeyMaterial(byte[] exchangeKeyOfPartner)
        {
            this.DeriveKeyMaterial(exchangeKeyOfPartner, CngKeyBlobFormat.EccPublicBlob);
        }

        /// <summary>キー・マテリアルを派生</summary>
        /// <param name="exchangeKeyOfPartner">相方の交換鍵</param>
        /// <param name="ckbf">CngKeyBlobFormat</param>
        public void DeriveKeyMaterial(byte[] exchangeKeyOfPartner, CngKeyBlobFormat ckbf)
        {
            // 交換鍵から、暗号化に使用する秘密鍵を生成
            this._privateKey = ((ECDiffieHellmanCng)this._asa).
                DeriveKeyMaterial(CngKey.Import(exchangeKeyOfPartner, ckbf));
        }
    }
}
