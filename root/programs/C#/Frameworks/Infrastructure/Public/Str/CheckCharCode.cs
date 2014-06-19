//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：CheckCharCode
//* クラス日本語名  ：汎用文字コード範囲チェック用クラス
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/20  西野  大介        新規作成
//**********************************************************************************

using System;
using System.Text;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>文字コード範囲チェック用クラス</summary>
    public class CheckCharCode
    {
        /// <summary>開始文字</summary>
        private string StartChar;
        /// <summary>終了文字</summary>
        private string EndChar;
        /// <summary>エンコーディング</summary>
        private Encoding StringEncoding;

        /// <summary>開始文字コードデータ</summary>
        private long StartCode;
        /// <summary>終了文字コードデータ</summary>
        private long EndCode;

        /// <summary>コンストラクタ</summary>
        public CheckCharCode(string startChar, string endChar, Encoding stringEncoding)
        {
            this.StartChar = startChar;
            this.EndChar = endChar;
            this.StringEncoding = stringEncoding;

            // １文字のバイトデータを数値データ（long）に変換
            this.StartCode = PubCmnFunction.GetLongFromByte(stringEncoding.GetBytes(startChar));
            this.EndCode = PubCmnFunction.GetLongFromByte(stringEncoding.GetBytes(endChar));
        }

        
        /// <summary>
        /// 文字コード範囲チェック
        /// </summary>
        /// <param name="ch">
        /// チェックする文字（１文字）
        /// </param>
        /// <returns>
        /// true：範囲内
        /// false：範囲外
        /// </returns>
        public bool IsInRange(string ch)
        {
            // １文字のバイトデータを数値データ（long）に変換
            long charCode = PubCmnFunction.GetLongFromByte(StringEncoding.GetBytes(ch));

            // 範囲チェック
            if (charCode >= this.StartCode && charCode <= this.EndCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
