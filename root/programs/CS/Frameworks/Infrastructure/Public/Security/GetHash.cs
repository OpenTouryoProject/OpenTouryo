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
//* クラス名        ：GetHash
//* クラス日本語名  ：ハッシュを取得するクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/02/15  西野 大介         新規作成
//*  2014/03/13  西野 大介         devps(1703):Createメソッドを使用してcryptoオブジェクトを作成します。
//*  2014/03/13  西野 大介         devps(1725):暗号クラスの使用終了時にデータをクリアする。
//*  2017/01/10  西野 大介         ストレッチ回数を指定可能にし、新設したGetPasswordを利用するように変更。
//*  2017/01/10  西野 大介         saltedPasswdのformat変更(salt+stretchCount+hashedPassword)。
//*  2017/01/10  西野 大介         上記のformat変更に伴い、EqualSaltedPasswd側のI/F変更が発生。
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2018/10/30  西野 大介         各種プロバイダのサポートを追加
//*  2019/11/15  西野 大介         GetHashBytesのStretchCountの既定値 = 0
//**********************************************************************************

using System.Security.Cryptography;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>ハッシュを取得するクラス</summary>
    public class GetHash
    {
        #region String

        /// <summary>文字列のハッシュ値を計算して返す。</summary>
        /// <param name="sourceString">文字列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>ハッシュ値（文字列）</returns>
        public static string GetHashString(string sourceString, EnumHashAlgorithm eha)
        {
            // overloadへ
            return GetHash.GetHashString(sourceString, eha, 1);
        }

        /// <summary>文字列のハッシュ値を計算して返す。</summary>
        /// <param name="sourceString">文字列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="stretchCount">ストレッチ回数</param>
        /// <returns>ハッシュ値（文字列）</returns>
        public static string GetHashString(string sourceString, EnumHashAlgorithm eha, int stretchCount)
        {
            return CustomEncode.ToBase64String(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(sourceString, CustomEncode.UTF_8), eha, stretchCount));
        }

        #endregion

        #region Bytes

        /// <summary>バイト配列のハッシュ値を計算して返す。</summary>
        /// <param name="asb">バイト配列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetHashBytes(byte[] asb, EnumHashAlgorithm eha)
        {
            // overloadへ
            return GetHash.GetHashBytes(asb, eha, 0);
        }

        /// <summary>
        /// バイト配列のハッシュ値を計算して返す
        /// （StretchCount = 1 の 下位互換用）。
        /// </summary>
        /// <param name="asb">バイト配列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetHashBytes_org(byte[] asb, EnumHashAlgorithm eha)
        {
            // overloadへ
            return GetHash.GetHashBytes(asb, eha, 1);
        }

        /// <summary>バイト配列のハッシュ値を計算して返す。</summary>
        /// <param name="asb">バイト配列</param>
        /// <param name="eha">ハッシュ・アルゴリズム列挙型</param>
        /// <param name="stretchCount">ストレッチ回数</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetHashBytes(byte[] asb, EnumHashAlgorithm eha, int stretchCount)
        {
            byte[] temp = null;

#if NETSTD
            // NETSTDの場合の実装
            if (eha == EnumHashAlgorithm.RIPEMD160_M)
            {
                // ハッシュ値を計算して返す。
                temp = GetHash.GetDigestBytesByBC(asb, new RipeMD160Digest());

                for (int i = 0; i < stretchCount; i++)
                {
                    // stretchCountが1以上なら繰り返す。
                    temp = GetHash.GetDigestBytesByBC(temp, new RipeMD160Digest());
                }

                return temp;
            }
#endif
            // ハッシュ（キー無し）サービスプロバイダを生成
            HashAlgorithm ha = HashAlgorithmCmnFunc.CreateHashAlgorithmSP(eha);

            // ハッシュ値を計算して返す。
            temp = ha.ComputeHash(asb);

            for (int i = 0; i < stretchCount; i++)
            {
                // stretchCountが1以上なら繰り返す。
                temp = ha.ComputeHash(temp);
            }

            ha.Clear(); // devps(1725)
            return temp;
        }

#if NETSTD
        /// <summary>BouncyCastleで、各種IDigestのハッシュ値を計算して返す。</summary>
        /// <param name="data">データ（バイト配列）</param>
        /// <param name="digest">IDigest</param>
        /// <returns>ハッシュ値（バイト配列）</returns>
        public static byte[] GetDigestBytesByBC(byte[] data, IDigest digest)
        {
            digest.BlockUpdate(data, 0, data.Length);
            byte[] rtnVal = new byte[digest.GetDigestSize()];
            digest.DoFinal(rtnVal, 0);
            return rtnVal;
        }
#endif
        #endregion
    }
}
