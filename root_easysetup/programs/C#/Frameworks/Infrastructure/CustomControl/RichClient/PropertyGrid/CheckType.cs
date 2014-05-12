//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：CheckType
//* クラス日本語名  ：デザインタイム プロパティ用　チェック タイプ指定クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

// System
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

// System.Windows
using System.Windows;
using System.Windows.Forms;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>チェック タイプ</summary>
    [TypeConverter(typeof(CheckTypeConverter))]
    public class CheckType
    {
        /// <summary>コンストラクタ</summary>
        public CheckType()
        {
            this.IsIndispensabile = false;
            this.IsHankaku = false;
            this.IsZenkaku = false;
            this.IsNumeric = false;
            this.IsKatakana = false;
            this.IsHanKatakana = false;
            this.IsHiragana = false;
            this.IsDate = false;
        }

        /// <summary>必須</summary>
        public bool IsIndispensabile { get; set; }
        /// <summary>半角</summary>
        public bool IsHankaku { get; set; }
        /// <summary>全角</summary>
        public bool IsZenkaku { get; set; }
        /// <summary>数値</summary>
        public bool IsNumeric { get; set; }
        /// <summary>片仮名</summary>
        public bool IsKatakana { get; set; }
        /// <summary>半角片仮名</summary>
        public bool IsHanKatakana { get; set; }
        /// <summary>平仮名</summary>
        public bool IsHiragana { get; set; }
        /// <summary>日付</summary>
        public bool IsDate { get; set; }

        #region 比較処理

        /// <summary>ハッシュを返す</summary>
        /// <returns>ハッシュコード</returns>
        /// <remarks>
        /// 全メンバのハッシュコードのXORではなく、
        /// ビットマスクの演算（boolなので）
        /// </remarks>
        public override int GetHashCode()
        {
            int hc = 0;

            if (this.IsIndispensabile) { hc += 1; }
            if (this.IsHankaku) { hc += 4; }
            if (this.IsZenkaku) { hc += 2; }
            if (this.IsNumeric) { hc += 8; }
            if (this.IsKatakana) { hc += 16; }
            if (this.IsHanKatakana) { hc += 32; }
            if (this.IsHiragana) { hc += 64; }
            if (this.IsDate) { hc += 128; }

            return hc;
        }

        /// <summary>Equals</summary>
        /// <param name="ct">CheckType</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        /// <remarks>全メンバの==のAND</remarks>
        public bool Equals(CheckType ct)
        {
            // null対応
            if (ct == null) { return false; }

            return
                (this.IsIndispensabile == ct.IsIndispensabile)
                && (this.IsHankaku == ct.IsHankaku)
                && (this.IsZenkaku == ct.IsZenkaku)
                && (this.IsNumeric == ct.IsNumeric)
                && (this.IsKatakana == ct.IsKatakana)
                && (this.IsHanKatakana == ct.IsHanKatakana)
                && (this.IsHiragana == ct.IsHiragana)
                && (this.IsDate == ct.IsDate);
        }

        /// <summary>Equals</summary>
        /// <param name="obj">CheckType</param>
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
                if (!(obj is CheckType))
                {
                    // 型が違う
                    return false;
                }
                else
                {
                    // 型が一致（オーバロードヘ）
                    return Equals(obj as CheckType);
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
        public static bool operator ==(CheckType l, CheckType r)
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
        public static bool operator !=(CheckType l, CheckType r)
        {
            // ==演算子の逆
            return !(l == r);
        }

        #endregion
    }
}
