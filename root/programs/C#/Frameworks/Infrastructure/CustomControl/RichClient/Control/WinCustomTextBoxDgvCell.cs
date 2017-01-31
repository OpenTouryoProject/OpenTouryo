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
//* クラス名        ：WinCustomTextBoxDgvCell
//* クラス日本語名  ：DataGridViewのWinCustomTextBoxセル（テンプレート）
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
    /// <summary>DataGridViewのWinCustomTextBoxセル</summary>
    public class WinCustomTextBoxDgvCell : DataGridViewTextBoxCell
    {
        /// <summary>コンストラクタ</summary>
        public WinCustomTextBoxDgvCell() : base() { }

        ///// <summary>DataGridViewTextBoxCell.InitializeEditingControl経由のset_Textを無視するための関数</summary>
        //private void _83B12F0F_CEA3_4f93_9233_B86EFA149BB2(
        //    int rowIndex, object initialFormattedValue,
        //    DataGridViewCellStyle dataGridViewCellStyle)
        //{
        //    // ベースへ
        //    base.InitializeEditingControl(rowIndex,
        //        initialFormattedValue, dataGridViewCellStyle);
        //}

        /// <summary>編集コントロールを初期化</summary>
        /// <param name="rowIndex">行</param>
        /// <param name="initialFormattedValue">フォーマットバリュー</param>
        /// <param name="dataGridViewCellStyle">セル スタイル</param>
        public override void InitializeEditingControl(
            int rowIndex, object initialFormattedValue,
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            //// DataGridViewTextBoxCell.InitializeEditingControlを呼び出す。
            //this._83B12F0F_CEA3_4f93_9233_B86EFA149BB2(
            //    rowIndex, initialFormattedValue, dataGridViewCellStyle);

            // ベースへ
            base.InitializeEditingControl(rowIndex,
                initialFormattedValue, dataGridViewCellStyle);

            // 編集コントロールであるWinCustomTextBoxDgvHostの取得
            WinCustomTextBoxDgvHost winCustomTextBoxDgvHost =
                this.DataGridView.EditingControl as WinCustomTextBoxDgvHost;

            // 編集コントロールであるWinCustomTextBoxDgvHostが取得できた場合
            if (winCustomTextBoxDgvHost != null)
            {
                // カスタム列のプロパティを反映させる
                WinCustomTextBoxDgvCol column =
                    this.OwningColumn as WinCustomTextBoxDgvCol;

                // プロパティの移植
                if (column != null)
                {
                    // 追加したプロパティをコピー
                    // チェック系は不要、編集系を設定
                    winCustomTextBoxDgvHost.MaxLength = column.MaxLength;

                    winCustomTextBoxDgvHost.IsNumeric = column.IsNumeric;
                    winCustomTextBoxDgvHost.EditInitialValue = column.EditInitialValue;
                    //winCustomTextBoxDgvHost.EditAddFigure = column.EditAddFigure;
                    winCustomTextBoxDgvHost.EditPadding = column.EditPadding;
                    //winCustomTextBoxDgvHost.EditDigitsAfterDP = column.EditDigitsAfterDP;
                    winCustomTextBoxDgvHost.EditDigitsAfterDP_Editing = column.EditDigitsAfterDP_Editing;
                    //winCustomTextBoxDgvHost.DisplayUnits = column.DisplayUnits;
                }

                //try
                //{

                // Textを設定（３項演算）
                //System.Diagnostics.Debug.WriteLine("InitializeEditingControl");
                //System.Diagnostics.Debug.WriteLine("・this.RowIndex：" + this.RowIndex.ToString());

                // DataGridView で DateTimePicker をホストすると ArgumentOutOfException が発生する
                //http://social.msdn.microsoft.com/Forums/ja-JP/7079fb1c-d171-44f8-81b1-751f3fe1ba6f/datagridview-datetimepicker-argumentoutofexception-

                //winCustomTextBoxDgvHost.Text =
                //    this.Value == null ? "" : this.Value.ToString();

                winCustomTextBoxDgvHost.Text =
                    this.GetValue(rowIndex) == null ? "" : this.GetValue(rowIndex).ToString();

                //}
                //catch (ArgumentOutOfRangeException aoorEx)
                //{
                //    // この例外は潰す。
                //}

                //// バインド先を変更
                //if (winCustomTextBoxDgvHost.DisplayUnits == null)
                //{
                //    // Textを設定（３項演算）
                //    winCustomTextBoxDgvHost.Text =
                //        this.Value == null ? "" : this.Value.ToString();
                //}
                //else
                //{
                //    // Valueを設定
                //    winCustomTextBoxDgvHost.Value = this.Value;
                //}
            }
        }

        /// <summary>編集コントロールの型を指定する</summary>
        public override Type EditType
        {
            get
            {
                return typeof(WinCustomTextBoxDgvHost);
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
