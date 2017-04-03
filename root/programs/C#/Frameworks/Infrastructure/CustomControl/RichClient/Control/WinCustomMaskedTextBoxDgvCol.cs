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
//* クラス名        ：WinCustomMaskedTextBoxDgvCol
//* クラス日本語名  ：DataGridViewのWinCustomMaskedTextBoxカラム（テンプレート）
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
    /// <summary>DataGridViewのWinCustomMaskedTextBoxカラム</summary>
    public class WinCustomMaskedTextBoxDgvCol : DataGridViewColumn
    {
        /// <summary>
        /// CellTemplateとするDataGridViewMaskedTextBoxCellを
        /// 指定して基本クラスのコンストラクタを呼び出す
        /// </summary>
        public WinCustomMaskedTextBoxDgvCol()
            : base(new WinCustomMaskedTextBoxDgvCell()) { }

        /// <summary>初期値編集プロパティに適用する値</summary>
        [Category("Edit"),
        Description("初期値編集")]
        public EditInitialValue EditInitialValue { get; set; }

        /// <summary>Maskプロパティに適用する値</summary>
        [Category("Edit"),
        Description("入力後のマスク")]
        public string Mask { set; get; }
        
        /// <summary>Mask_Editingプロパティに適用する値</summary>
        [Category("Edit"),
        Description("入力中のマスク")]
        public string Mask_Editing { set; get; }

        /// <summary>半角指定（マスクで指定できないため）</summary>
        [DefaultValue(false),
        Category("Edit"),
        Description("半角指定（マスクで指定できないため）")]
        public bool EditToHankaku { get; set; }

        /// <summary>YYYYMMDDのM、Dが１桁の時に補正処理を行う。</summary>
        [DefaultValue(false),
        Category("Edit"),
        Description("YYYYMMDDのM、Dが１桁の時に補正処理を行う。")]
        public bool EditToYYYYMMDD { get; set; }        

        /// <summary>クローンの作製</summary>
        /// <returns>クローン</returns>
        public override object Clone()
        {
            // base.Cloneの後に、
            WinCustomMaskedTextBoxDgvCol col =
                (WinCustomMaskedTextBoxDgvCol)base.Clone();

            // 追加したプロパティをコピー
            // チェック系は不要、編集系を設定
            col.EditInitialValue = this.EditInitialValue;

            col.Mask = this.Mask;
            col.Mask_Editing = this.Mask_Editing;

            col.EditToHankaku = this.EditToHankaku;
            col.EditToYYYYMMDD = this.EditToYYYYMMDD;

            return col;
        }

        /// <summary>CellTemplateの取得と設定</summary>
        public override DataGridViewCell CellTemplate
        {
            set
            {
                // DataGridViewMaskedTextBoxCellしか
                // CellTemplateに設定できないようにする
                if (!(value is WinCustomMaskedTextBoxDgvCell))
                {
                    throw new InvalidCastException(
                        "WinCustomMaskedTextBoxDgvCellオブジェクトを指定してください。");
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
