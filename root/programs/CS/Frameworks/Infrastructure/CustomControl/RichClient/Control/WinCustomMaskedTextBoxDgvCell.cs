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
//* クラス名        ：WinCustomMaskedTextBoxDgvCell
//* クラス日本語名  ：DataGridViewのWinCustomMaskedTextBoxセル（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System;
using System.Windows.Forms;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>DataGridViewのWinCustomMaskedTextBoxセル</summary>
    public class WinCustomMaskedTextBoxDgvCell : DataGridViewTextBoxCell
    {
        /// <summary>コンストラクタ</summary>
        public WinCustomMaskedTextBoxDgvCell() : base() { }

        /// <summary>編集コントロールを初期化</summary>
        /// <param name="rowIndex">行</param>
        /// <param name="initialFormattedValue">フォーマットバリュー</param>
        /// <param name="dataGridViewCellStyle">セル スタイル</param>
        public override void InitializeEditingControl(
            int rowIndex, object initialFormattedValue,
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            //System.Diagnostics.Debug.WriteLine("InitializeEditingControl");
            //System.Diagnostics.Debug.WriteLine("・rowIndex：" + rowIndex);

            // ベースへ。
            base.InitializeEditingControl(rowIndex,
                initialFormattedValue, dataGridViewCellStyle);

            // 編集コントロールであるWinCustomMaskedTextBoxDgvHostの取得
            WinCustomMaskedTextBoxDgvHost winCustomMaskedTextBoxDgvHost =
                this.DataGridView.EditingControl as WinCustomMaskedTextBoxDgvHost;

            // 編集コントロールであるWinCustomMaskedTextBoxDgvHostが取得できた場合
            if (winCustomMaskedTextBoxDgvHost != null)
            {
                // カスタム列のプロパティを反映させる
                WinCustomMaskedTextBoxDgvCol column =
                    this.OwningColumn as WinCustomMaskedTextBoxDgvCol;

                // プロパティの移植
                if (column != null)
                {
                    // 追加したプロパティをコピー
                    // チェック系は不要、編集系を設定
                    winCustomMaskedTextBoxDgvHost.EditInitialValue = column.EditInitialValue;

                    winCustomMaskedTextBoxDgvHost.Mask = column.Mask;
                    winCustomMaskedTextBoxDgvHost.Mask_Editing = column.Mask_Editing;

                    winCustomMaskedTextBoxDgvHost.EditToHankaku = column.EditToHankaku;
                    winCustomMaskedTextBoxDgvHost.EditToYYYYMMDD = column.EditToYYYYMMDD;
                }

                //try
                //{

                // Textを設定（３項演算）
                //System.Diagnostics.Debug.WriteLine("InitializeEditingControl");
                //System.Diagnostics.Debug.WriteLine("・this.RowIndex：" + this.RowIndex.ToString());

                // DataGridView で DateTimePicker をホストすると ArgumentOutOfException が発生する
                //http://social.msdn.microsoft.com/Forums/ja-JP/7079fb1c-d171-44f8-81b1-751f3fe1ba6f/datagridview-datetimepicker-argumentoutofexception-

                //winCustomMaskedTextBoxDgvHost.Text =
                //    this.Value == null ? "" : this.Value.ToString();

                winCustomMaskedTextBoxDgvHost.Text =
                    this.GetValue(rowIndex) == null ? "" : this.GetValue(rowIndex).ToString();

                //}
                //catch (ArgumentOutOfRangeException aoorEx)
                //{
                //    // この例外は潰す。
                //}
            }
        }

        /// <summary>編集コントロールの型を指定する</summary>
        public override Type EditType
        {
            get
            {
                return typeof(WinCustomMaskedTextBoxDgvHost);
            }
        }

        ///// <summary>セルの値のデータ型を指定する。</summary>
        ///// <remarks>ここでは、Object型とする。基本クラスと同じなので、オーバーライドの必要なし</remarks>
        //public override Type ValueType
        //{
        //    get
        //    {
        //        return typeof(object);
        //    }
        //}

        /// <summary>新しいレコード行のセルの既定値を指定する</summary>
        public override object DefaultNewRowValue
        {
            get
            {
                // ベースへ。
                return base.DefaultNewRowValue;
            }
        }
    }
}
