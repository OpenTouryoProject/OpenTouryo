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
//* クラス名        ：EditDigitsAfterDPConverter
//* クラス日本語名  ：デザインタイム プロパティ用
//*                   小数点数以下ｘ桁編集方法の指定クラスのコンバータ（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System;
using System.Globalization;
using System.ComponentModel;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>デザインタイム プロパティ用　小数点数以下ｘ桁編集方法の指定クラスのコンバータ（テンプレート）</summary>
    public class EditDigitsAfterDPConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// ConvertTo（プロパティグリッド表示値への変換）を実行可能か。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="destinationType">変換後の型</param>
        /// <returns>
        /// ConvertTo実行可：true。
        /// ConvertTo実行不可：false。
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            // 型をチェック
            if (destinationType == typeof(EditDigitsAfterDP))
            {
                // EditDigitsAfterDP型ならtrueを返す。
                return true;
            }

            // 上記以外の場合、ベースへ。
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// ConvertTo（プロパティグリッド表示値への変換）を実行する。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="culture">カルチャと</param>
        /// <param name="value">EditDigitsAfterDPオブジェクト</param>
        /// <param name="destinationType">変換後の型</param>
        /// <returns>文字列</returns>
        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            // 型をチェック
            if (destinationType == typeof(string))
            {
                // 文字列へ変換
                if (value is EditDigitsAfterDP)
                {
                    EditDigitsAfterDP edadp = (EditDigitsAfterDP)value;
                    return edadp.HowToCut.ToString() + " : " + edadp.DigitsAfterDP.ToString();
                }
            }

            // 上記以外の場合、ベースへ。
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// ConvertFrom（プロパティグリッドからの変換）を実行可能か。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="sourceType">文字列</param>
        /// <returns>
        /// ConvertFrom実行可：true。
        /// ConvertFrom実行不可：false。
        /// </returns>
        public override bool CanConvertFrom(
            ITypeDescriptorContext context, Type sourceType)
        {
            // 型をチェック
            if (sourceType == typeof(string))
            {
                // 文字列型ならtrueを返す。
                return true;
            }

            // 上記以外の場合、ベースへ。
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// ConvertFrom（プロパティグリッドからの変換）を実行する。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="culture">カルチャ</param>
        /// <param name="value">文字列</param>
        /// <returns>EditDigitsAfterDPオブジェクト</returns>
        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // 文字列型の場合
            if (value is string)
            {
                string[] s = value.ToString().Split(':');

                EditDigitsAfterDP edadp = new EditDigitsAfterDP();

                string howToCut = s[0].Trim();
                if (howToCut == CutMethod.Banker.ToString())
                {
                    edadp.HowToCut = CutMethod.Banker;
                }
                else if (howToCut == CutMethod._4sya5nyu.ToString())
                {
                    edadp.HowToCut = CutMethod._4sya5nyu;
                }
                else if (howToCut == CutMethod.Ceiling.ToString())
                {
                    edadp.HowToCut = CutMethod.Floor;
                }
                else
                {
                    edadp.HowToCut = CutMethod.Ceiling;
                }

                if (s.Length <= 1 || s[1].Trim() == "")
                {
                    edadp.DigitsAfterDP = 0;
                }
                else
                {
                    edadp.DigitsAfterDP = uint.Parse(s[1].Trim());
                }

                return edadp;
            }

            // 上記以外の場合、ベースへ。
            return base.ConvertFrom(context, culture, value);
        }
    }
}
