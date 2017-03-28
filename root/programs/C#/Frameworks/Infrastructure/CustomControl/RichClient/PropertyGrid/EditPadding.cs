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
//* クラス名        ：EditPadding
//* クラス日本語名  ：デザインタイム プロパティ用　パディング指定クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System;
using System.ComponentModel;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>PadDirection</summary>
    public enum PadDirection
    {
        /// <summary>パッドなし</summary>
        None,
        /// <summary>右側にパッド</summary>
        Right,
        /// <summary>左側にパッド</summary>
        Left
    }

    /// <summary>パディング指定クラス</summary>
    /// <remarks>デザインタイム プロパティ用</remarks>
    [TypeConverter(typeof(EditPaddingConverter))]
    public class EditPadding
    {
        /// <summary>コンストラクタ</summary>
        public EditPadding() { }

        /// <summary>コンストラクタ</summary>
        /// <param name="padDir">パッド方向</param>
        /// <param name="padChar">パッド文字</param>
        public EditPadding(PadDirection padDir, char? padChar)
        {
            this.PadDir = padDir;
            this.PadChar = padChar;
        }

        /// <summary>パッド方向</summary>
        public PadDirection PadDir { get; set; }
        /// <summary>パッド文字</summary>
        public char? PadChar { get; set; }

        #region 比較処理

        /// <summary>ハッシュを返す</summary>
        /// <returns>ハッシュコード</returns>
        /// <remarks>全メンバのハッシュコードのXOR</remarks>
        public override int GetHashCode()
        {
            int hc = 0;
            
            hc ^= this.PadDir.GetHashCode();

            if (this.PadChar != null)
            {
                hc ^= this.PadChar.GetHashCode();
            }

            return hc;
        }

        /// <summary>Equals</summary>
        /// <param name="ep">EditPadding</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        /// <remarks>全メンバの==のAND</remarks>
        public bool Equals(EditPadding ep)
        {
            // null対応
            if (ep == null) { return false; }

            return
                (this.PadDir == ep.PadDir)
                && (this.PadChar == ep.PadChar);
        }

        /// <summary>Equals</summary>
        /// <param name="obj">EditPadding</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                // nullの場合（ベースへ）
                return base.Equals(obj);
            }
            else
            {
                // nullでない場合
                if (!(obj is EditPadding))
                {
                    // 型が違う
                    return false;
                }
                else
                {
                    // 型が一致（オーバロードヘ）
                    return Equals(obj as EditPadding);
                }
            }
        }

        /// <summary>比較演算子（==）</summary>
        /// <param name="l">右辺</param>
        /// <param name="r">左辺</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public static bool operator ==(EditPadding l, EditPadding r)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(l, null))
            {
                // Check for null on right side.
                if (Object.ReferenceEquals(r, null))
                {
                    // null == null = true.
                    return true;
                }
                else
                {
                    // Only the left side is null.
                    return false;
                }
            }
            else
            {
                // Equals handles case of null on right side.
                return l.Equals(r);
            }
        }

        /// <summary>比較演算子（!=）</summary>
        /// <param name="l">右辺</param>
        /// <param name="r">左辺</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public static bool operator !=(EditPadding l, EditPadding r)
        {
            // ==演算子の逆
            return !(l == r);
        }

        #endregion
    }
}
