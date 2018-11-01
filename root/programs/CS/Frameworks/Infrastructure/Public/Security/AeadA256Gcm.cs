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

        /// <summary>暗号化</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="plaint">平文（plaintext）</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        /// <returns>AEAD実行結果オブジェクト</returns>
        public static AeadResult Encrypt(byte[] cek, byte[] iv, byte[] plaint, byte[] aad)
        {
            // Aesをエンジンに指定してGcmBlockCipherを生成
            GcmBlockCipher gcm = new GcmBlockCipher(new AesEngine());
            // GCM実装を初期化
            gcm.Init(true, new AeadParameters(new KeyParameter(cek), 8 * GCM_TAG_LEN, iv, aad));
            // 出力バッファの準備
            byte[] ciphert = new byte[gcm.GetOutputSize(plaint.Length)];
            // GCM操作の実行
            int len = gcm.ProcessBytes(plaint, 0, plaint.Length, ciphert, 0);
            // GCM操作を終了
            len += gcm.DoFinal(ciphert, len);
            // GetMacで認証タグ（MAC）を取得委
            byte[] tag = gcm.GetMac();

            // 結果を返す
            return new AeadResult()
            {
                Ciphert = PubCmnFunction.ShortenByteArray(ciphert, plaint.Length),
                Tag = tag,
            };

        }

        /// <summary>復号化</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        /// <param name="aeadRet">
        /// AeadResult
        /// ・暗号文
        /// ・認証タグ（MAC）
        /// </param>
        /// <returns>平文（plaintext）</returns>
        public static byte[] Decrypt(byte[] cek, byte[] iv, byte[] aad, AeadResult aeadRet)
        {
            // Aesをエンジンに指定してGcmBlockCipherを生成
            GcmBlockCipher gcm = new GcmBlockCipher(new AesEngine());
            // GCM実装を初期化
            gcm.Init(false, new AeadParameters(new KeyParameter(cek), 8 * GCM_TAG_LEN, iv, aad));
            // Decrypt ( ciphert + tag )
            byte[] marged = aeadRet.CombineByteArrayForDecrypt();
            // 出力バッファの準備
            byte[] plaint = new byte[gcm.GetOutputSize(marged.Length)];
            // GCM操作の実行
            int len = gcm.ProcessBytes(marged, 0, marged.Length, plaint, 0);
            // GCM操作を終了
            len += gcm.DoFinal(plaint, len);

            // 平文を返す。
            return plaint;
        }
    }
}
