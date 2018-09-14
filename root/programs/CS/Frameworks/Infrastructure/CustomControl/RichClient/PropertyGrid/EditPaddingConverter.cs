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
//* クラス名        ：EditPaddingConverter
//* クラス日本語名  ：デザインタイム プロパティ用　パディング指定クラスのコンバータ（テンプレート）
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
    /// <summary>デザインタイム プロパティ用　パディング指定クラスのコンバータ（テンプレート）</summary>
    public class EditPaddingConverter : ExpandableObjectConverter
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
            if (destinationType == typeof(EditPadding))
            {
                // Length型ならtrueを返す。
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
        /// <param name="value">EditPaddingオブジェクト</param>
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
                if (value is EditPadding)
                {
                    EditPadding ep = (EditPadding)value;
                    if (ep.PadChar == null)
                    {
                        return ep.PadDir.ToString() + " : -";
                    }
                    else
                    {
                        return ep.PadDir.ToString() + " : " + ep.PadChar;
                    }
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
        /// <returns>EditPaddingオブジェクト</returns>
        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // 文字列型の場合
            if (value is string)
            {
                string[] s = value.ToString().Split(':');

                EditPadding ep = new EditPadding();

                string padDir = s[0].Trim();
                if (padDir == PadDirection.Left.ToString())
                {
                    ep.PadDir = PadDirection.Left;
                }
                else if (padDir == PadDirection.Right.ToString())
                {
                    ep.PadDir = PadDirection.Right;
                }
                else
                {
                    ep.PadDir = PadDirection.None;
                }

                if (s.Length <= 1 || s[1].Trim() == "")
                {
                    ep.PadChar = '-';
                }
                else
                {
                    ep.PadChar = char.Parse(s[1].Trim());
                }

                return ep;
            }

            // 上記以外の場合、ベースへ。
            return base.ConvertFrom(context, culture, value);
        }
    }
}
