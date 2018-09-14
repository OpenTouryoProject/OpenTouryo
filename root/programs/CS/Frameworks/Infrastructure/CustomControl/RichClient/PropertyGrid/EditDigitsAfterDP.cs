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
//* クラス名        ：EditDigitsAfterDP
//* クラス日本語名  ：デザインタイム プロパティ用
//*                   小数点数以下ｘ桁編集方法の指定クラス（テンプレート）
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
    /// <summary>CutMethod</summary>
    public enum CutMethod
    {
        /// <summary>なし</summary>
        None,
        /// <summary>最近接偶数編集</summary>
        Banker,
        /// <summary>四捨五入</summary>
        _4sya5nyu,
        /// <summary>切り捨て（絶対値：0への丸め）</summary>
        Floor,
        /// <summary>切り上げ（絶対値：無限大への丸め）</summary>
        Ceiling,
        /// <summary>切り捨て（大小：負の無限大への丸め）</summary>
        FloorRM,
        /// <summary>切り上げ（大小：正の無限大への丸め）</summary>
        CeilingRP
    }

    /// <summary>小数点数以下ｘ桁編集方法の指定クラス</summary>
    /// <remarks>デザインタイム プロパティ用</remarks>
    [TypeConverter(typeof(EditDigitsAfterDPConverter))]
    public class EditDigitsAfterDP
    {
        /// <summary>コンストラクタ</summary>
        public EditDigitsAfterDP() { }

        /// <summary>コンストラクタ</summary>
        /// <param name="howToCut">切り方</param>
        /// <param name="digitsAfterDP">小数点数以下ｘ桁</param>
        public EditDigitsAfterDP(CutMethod howToCut, uint digitsAfterDP)
        {
            this.HowToCut = howToCut;
            this.DigitsAfterDP = digitsAfterDP;
        }

        /// <summary>切り方</summary>
        public CutMethod? HowToCut { get; set; }
        /// <summary>小数点数以下ｘ桁</summary>
        public uint DigitsAfterDP { get; set; }

        #region 比較処理

        /// <summary>ハッシュを返す</summary>
        /// <returns>ハッシュコード</returns>
        /// <remarks>全メンバのハッシュコードのXOR</remarks>
        public override int GetHashCode()
        {
            int hc = 0;

            hc ^= this.HowToCut.GetHashCode();
            hc ^= this.DigitsAfterDP.GetHashCode();

            return hc;
        }

        /// <summary>Equals</summary>
        /// <param name="edadp">EditDigitsAfterDP</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        /// <remarks>全メンバの==のAND</remarks>
        public bool Equals(EditDigitsAfterDP edadp)
        {
            // null対応
            if (edadp == null) { return false; }

            return
                (this.HowToCut == edadp.HowToCut)
                && (this.DigitsAfterDP == edadp.DigitsAfterDP);
        }

        /// <summary>Equals</summary>
        /// <param name="obj">EditDigitsAfterDP</param>
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
                if (!(obj is EditDigitsAfterDP))
                {
                    // 型が違う
                    return false;
                }
                else
                {
                    // 型が一致（オーバロードヘ）
                    return Equals(obj as EditDigitsAfterDP);
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
        public static bool operator ==(EditDigitsAfterDP l, EditDigitsAfterDP r)
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
        public static bool operator !=(EditDigitsAfterDP l, EditDigitsAfterDP r)
        {
            // ==演算子の逆
            return !(l == r);
        }

        #endregion
    }
}
