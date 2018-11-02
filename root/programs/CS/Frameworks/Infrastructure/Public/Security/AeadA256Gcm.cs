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
//* クラス名        ：AeadA256Gcm
//* クラス日本語名  ：認証付き暗号（A128CBC-HS256）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/01  西野 大介         新規作成
//**********************************************************************************

using Touryo.Infrastructure.Public.Util;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

// https://qiita.com/hidelafoglia/items/d12550c8ffe6eca993c8
// https://github.com/cose-wg/cose-implementations/blob/master/csharp/JOSE/EncryptMessage.cs

namespace Touryo.Infrastructure.Public.Security
{
    
    /// <summary>
    /// AeadA256Gcm
    /// 認証付き暗号（A128CBC-HS256）クラス
    /// A256GCM(AES-GCM)
    /// </summary>
    public class AeadA256Gcm : AuthEncrypt
    {
        /// <summary>
        /// GCMで計算されるTagサイズは 128ビット:
        /// https://tools.ietf.org/html/rfc7518#section-4.7
        ///  The requested size of the Authentication Tag output MUST be 128 bits, regardless of the key size.
        /// </summary>
        public const int GCM_TAG_LEN = 16;
        
        /// <summary>Aesをエンジンに指定してGcmBlockCipherを生成</summary>
        private GcmBlockCipher _gcm = new GcmBlockCipher(new AesEngine());        

        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadA256Gcm(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad)
        {   
            // GCM実装を初期化
            this._gcm.Init(true, new AeadParameters(new KeyParameter(cek), 8 * AeadA256Gcm.GCM_TAG_LEN, iv, aad));
        }

        /// <summary>暗号化</summary>
        /// <param name="plaint">平文（plaintext）</param>
        /// <returns>AEAD実行結果オブジェクト</returns>
        public override void Encrypt(byte[] plaint)
        {   
            // 出力バッファの準備
            byte[] ciphert = new byte[this._gcm.GetOutputSize(plaint.Length)];
            // GCM操作の実行
            int len = this._gcm.ProcessBytes(plaint, 0, plaint.Length, ciphert, 0);
            // GCM操作を終了
            len += this._gcm.DoFinal(ciphert, len);
            // GetMacで認証タグ（MAC）を取得委
            byte[] tag = this._gcm.GetMac();

            // 結果を返す
            this._result = new AeadResult()
            {
                Ciphert = PubCmnFunction.ShortenByteArray(ciphert, plaint.Length),
                Tag = tag,
            };
        }

        /// <summary>復号化</summary>
        /// <returns>平文（plaintext）</returns>
        public override byte[] Decrypt()
        {
            // Decrypt ( ciphert + tag )
            byte[] marged = this._result.CombineByteArrayForDecrypt();
            // 出力バッファの準備
            byte[] plaint = new byte[this._gcm.GetOutputSize(marged.Length)];
            // GCM操作の実行
            int len = this._gcm.ProcessBytes(marged, 0, marged.Length, plaint, 0);
            // GCM操作を終了
            len += this._gcm.DoFinal(plaint, len);

            // 平文を返す。
            return plaint;
        }
    }
}
