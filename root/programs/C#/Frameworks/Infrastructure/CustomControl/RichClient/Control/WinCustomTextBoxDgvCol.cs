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
//* クラス名        ：WinCustomTextBoxDgvCol
//* クラス日本語名  ：DataGridViewのWinCustomTextBoxカラム（テンプレート）
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
using System.Windows.Forms;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>DataGridViewのWinCustomTextBoxカラム</summary>
    public class WinCustomTextBoxDgvCol : DataGridViewColumn
    {
        /// <summary>
        /// CellTemplateとするWinCustomTextBoxDgvCellを
        /// 指定して基本クラスのコンストラクタを呼び出す
        /// </summary>
        public WinCustomTextBoxDgvCol()
            : base(new WinCustomTextBoxDgvCell()) { }

        /// <summary>MaxLengthプロパティの設定用</summary>
        public int MaxLength { get; set; }

        /// <summary>入力制限専用プロパティに適用する値</summary>
        [DefaultValue(false),
        Category("Edit"),
        Description("入力制限専用")]
        public bool IsNumeric { get; set; }

        /// <summary>初期値編集プロパティに適用する値</summary>
        [Category("Edit"),
        Description("初期値編集")]
        public EditInitialValue EditInitialValue { get; set; }

        /// <summary>文字埋め編集プロパティに適用する値</summary>
        [Category("Edit"),
        Description("文字埋め編集")]
        public EditPadding EditPadding { get; set; }

        /// <summary>小数点以下編集（入力中）プロパティに適用する値</summary>
        [Category("Edit"),
        Description("小数点以下ｘ桁編集（入力中）")]
        public EditDigitsAfterDP EditDigitsAfterDP_Editing { get; set; }

        /// <summary>クローンの作製</summary>
        /// <returns>クローン</returns>
        public override object Clone()
        {
            // base.Cloneの後に、
            WinCustomTextBoxDgvCol col =
                (WinCustomTextBoxDgvCol)base.Clone();

            // 追加したプロパティをコピー
            // チェック系は不要、編集系を設定
            col.MaxLength = this.MaxLength;

            col.IsNumeric = this.IsNumeric;
            col.EditInitialValue = this.EditInitialValue;
            //col.EditAddFigure = this.EditAddFigure;
            col.EditPadding = this.EditPadding;
            //col.EditDigitsAfterDP = this.EditDigitsAfterDP;
            col.EditDigitsAfterDP_Editing = this.EditDigitsAfterDP_Editing;

            //col.DisplayUnits = this.DisplayUnits;

            return col;
        }

        /// <summary>CellTemplateの取得と設定</summary>
        public override DataGridViewCell CellTemplate
        {
            set
            {
                // WinCustomTextBoxDgvCellしか
                // CellTemplateに設定できないようにする
                if (!(value is WinCustomTextBoxDgvCell))
                {
                    throw new InvalidCastException(
                        "WinCustomTextBoxDgvCellオブジェクトを指定してください。");
                }
                base.CellTemplate = value;
            }
            get
            {
                // 取得
                return base.CellTemplate;
            }
        }
    }
}
