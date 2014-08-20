﻿//**********************************************************************************
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

// System.Web
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Touryo.Infrastructure.CustomControl
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

        #region プロパティ

        /// <summary>必須</summary>
        private bool _isIndispensabile;

        /// <summary>必須</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsIndispensabile
        {
            get 
            {
                return this._isIndispensabile;
            }

            set
            {
                this._isIndispensabile = value;
            }
        }

        /// <summary>半角</summary>
        private bool _isHankaku;

        /// <summary>半角</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsHankaku
        {
            get
            {
                return this._isHankaku;
            }

            set
            {
                this._isHankaku = value;
            }
        }

        /// <summary>全角</summary>
        private bool _isZenkaku;

        /// <summary>全角</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsZenkaku
        {
            get
            {
                return this._isZenkaku;
            }

            set
            {
                this._isZenkaku = value;
            }
        }

        /// <summary>数値</summary>
        private bool _isNumeric;

        /// <summary>数値</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsNumeric
        {
            get
            {
                return this._isNumeric;
            }

            set
            {
                this._isNumeric = value;
            }
        }

        /// <summary>片仮名</summary>
        private bool _isKatakana;

        /// <summary>片仮名</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsKatakana
        {
            get
            {
                return this._isKatakana;
            }

            set
            {
                this._isKatakana = value;
            }
        }

        /// <summary>半角片仮名</summary>
        private bool _isHanKatakana;

        /// <summary>半角片仮名</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsHanKatakana
        {
            get
            {
                return this._isHanKatakana;
            }

            set
            {
                this._isHanKatakana = value;
            }
        }

        /// <summary>平仮名</summary>
        private bool _isHiragana;

        /// <summary>平仮名</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsHiragana
        {
            get
            {
                return this._isHiragana;
            }

            set
            {
                this._isHiragana = value;
            }
        }

        /// <summary>日付</summary>
        private bool _isDate;

        /// <summary>日付</summary>
        [DefaultValue(false), NotifyParentProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool IsDate
        {
            get
            {
                return this._isDate;
            }

            set
            {
                this._isDate = value;
            }
        }

        #endregion

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
