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
//* クラス名        ：MsgAuthCode
//* クラス日本語名  ：メッセージ認証コード（MAC）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//*  2018/10/31  西野 大介         クラスの再編（GetKeyedHash -> GetKeyedHash, GetPasswordHashVn, MsgAuthCode）
//**********************************************************************************

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>メッセージ認証コード（MAC）クラス</summary>
    public class MsgAuthCode : GetKeyedHash
    {
        /// <summary>MAC値を返す。</summary>
        /// <param name="ekha">MACアルゴリズム列挙型</param>
        /// <param name="msg">メッセージ（文字列）</param>
        /// <param name="key">キー（文字列）</param>
        /// <returns>MAC値（base64文字列）</returns>
        public static string GetMAC(EnumKeyedHashAlgorithm ekha, string msg, string key)
        {
            return CustomEncode.ToBase64String(MsgAuthCode.GetMAC(ekha,
                CustomEncode.StringToByte(msg, CustomEncode.UTF_8),
                CustomEncode.StringToByte(key, CustomEncode.UTF_8)));
        }

        /// <summary>MAC値を返す。</summary>
        /// <param name="ekha">MACアルゴリズム列挙型</param>
        /// <param name="msg">メッセージ（バイト配列）</param>
        /// <param name="key">キー（バイト配列）</param>
        /// <returns>MAC値（バイト配列）</returns>
        public static byte[] GetMAC(EnumKeyedHashAlgorithm ekha, byte[] msg, byte[] key)
        {   
            return MsgAuthCode.GetKeyedHashBytes(ekha, msg, key);
        }

        /// <summary>MAC値を検証</summary>
        /// <param name="ekha">MACアルゴリズム列挙型</param>
        /// <param name="msg">メッセージ（文字列）</param>
        /// <param name="key">キー（文字列）</param>
        /// <param name="mac">MAC値（base64文字列）</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public static bool VerifyMAC(EnumKeyedHashAlgorithm ekha, string msg, string key, string mac)
        {
            return MsgAuthCode.VerifyMAC(ekha,
                CustomEncode.StringToByte(msg, CustomEncode.UTF_8),
                CustomEncode.StringToByte(key, CustomEncode.UTF_8),
                CustomEncode.FromBase64String(mac));
        }

        /// <summary>MAC値を検証</summary>
        /// <param name="ekha">MACアルゴリズム列挙型</param>
        /// <param name="msg">メッセージ（バイト配列）</param>
        /// <param name="key">キー（バイト配列）</param>
        /// <param name="mac">MAC値（バイト配列）</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public static bool VerifyMAC(EnumKeyedHashAlgorithm ekha, byte[] msg, byte[] key, byte[] mac)
        {
            // 文字列にしてから計算
            string paramMac = CustomEncode.ToBase64String(mac);
            string calcMac = CustomEncode.ToBase64String(MsgAuthCode.GetMAC(ekha, msg, key));

            return (paramMac == calcMac);
        }
    }
}