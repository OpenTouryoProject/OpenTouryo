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
//* クラス名        ：WinCustomTextBoxDgvHost
//* クラス日本語名  ：テキスト ボックス（Win）のカスタム・コントロールをDataGridViewでホストする（テンプレート）
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
    /// <summary>WinCustomTextBoxをDataGridViewでホストする。</summary>
    /// <remarks>IDataGridViewEditingControlを実装する。</remarks>
    public class WinCustomTextBoxDgvHost :
        WinCustomTextBox, IDataGridViewEditingControl
    {
        /// <summary>コンストラクタ</summary>
        public WinCustomTextBoxDgvHost() : base()
        {
            this.TabStop = false;
        }

        #region IDataGridViewEditingControl メンバ

        /// <summary>編集コントロールで変更されたセルの値</summary>
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            ////return this.Text;// 編集処理がある場合工夫が必要
            //System.Diagnostics.Debug.WriteLine("GetEditingControlFormattedValue");
            //System.Diagnostics.Debug.WriteLine("・DataGridViewDataErrorContexts：" + context.ToString());
            //System.Diagnostics.Debug.WriteLine("・this.Text：" + this.Text);

            if (context ==
                (DataGridViewDataErrorContexts.Formatting
                | DataGridViewDataErrorContexts.Display))
            {
                // 編集モードに入るとき
            }
            else if (context ==
                (DataGridViewDataErrorContexts.Parsing
                | DataGridViewDataErrorContexts.Commit))
            {
                // マウスで抜けた場合

                // 例外あり。
                if (this.Edited)
                {
                    // 下端でEnterで抜けた場合
                    this.PreValidate();
                    this.ReEdit();
                }
            }
            else if (context ==
                (DataGridViewDataErrorContexts.Parsing
                | DataGridViewDataErrorContexts.Commit
                | DataGridViewDataErrorContexts.CurrentCellChange))
            {
                // Tab、Enterで抜けた場合
                this.PreValidate();
                this.ReEdit();
            }
            else if (context ==
                (DataGridViewDataErrorContexts.Parsing
                | DataGridViewDataErrorContexts.Commit
                | DataGridViewDataErrorContexts.LeaveControl))
            {
                // 上端でShift + Tabで抜けた場合
                this.PreValidate();
                this.ReEdit();
            }
            else
            {
                // 不明
            }

            return this.Text;

            //// バインド先を変更
            //if (this.DisplayUnits == null)
            //{
            //    // 単位変換無し。
            //    return this.Text;
            //}
            //else
            //{
            //    // 単位変換有り。
            //    return this.Value;
            //}
        }

        /// <summary>編集コントロールで変更されたセルの値</summary>
        public object EditingControlFormattedValue
        {
            set
            {
                this.Text = (string)value;

                //// バインド先を変更
                //if (this.DisplayUnits == null)
                //{
                //    // 単位変換無し。
                //    this.Text = (string)value;
                //}
                //else
                //{
                //    // 単位変換有り。
                //    this.Value = (string)value;
                //}
            }
            get
            {
                // GetEditingControlFormattedValueに任せる。
                return this.GetEditingControlFormattedValue(
                    DataGridViewDataErrorContexts.Formatting);
            }
        }

        /// <summary>セルスタイルを編集コントロールに適用する</summary>
        /// <param name="dataGridViewCellStyle">セルのスタイル</param>
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {

            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;

            // dataGridViewCellStyle.Alignment → this.TextAlign
            switch (dataGridViewCellStyle.Alignment)
            {
                case DataGridViewContentAlignment.BottomCenter:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.TopCenter:
                    this.TextAlign = HorizontalAlignment.Center;
                    break;

                case DataGridViewContentAlignment.BottomRight:
                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.TopRight:
                    this.TextAlign = HorizontalAlignment.Right;
                    break;

                default:
                    this.TextAlign = HorizontalAlignment.Left;
                    break;
            }
        }

        /// <summary>編集しているセルがあるDataGridView</summary>
        public DataGridView EditingControlDataGridView { set; get; }

        /// <summary>編集しているセルの行があるインデックス</summary>
        public int EditingControlRowIndex { set; get; }

        /// <summary>編集されたか（編集コントロールとセルの値が違うか）</summary>
        public bool EditingControlValueChanged { set; get; }

        /// <summary>
        /// 指定されたキーが、編集コントロールによって処理される通常の入力キーか、
        /// DataGridView によって処理される特殊なキーであるかを確認します。
        /// </summary>
        /// <param name="keyData">入力キー</param>
        /// <param name="dataGridViewWantsInputKey">
        /// keyData に格納された Keys を、DataGridView に
        /// 処理させる場合は true。それ以外の場合は false。
        /// </param>
        /// <returns>
        /// true：編集コントロールに処理される入力キー
        /// false：それ以外の場合
        /// </returns>
        public bool EditingControlWantsInputKey(
            Keys keyData, bool dataGridViewWantsInputKey)
        {
            if (dataGridViewWantsInputKey)
            {
                // DataGridView に処理される入力キー

                // Keys.Left、Right、Home、Endを
                // 編集コントロールに処理される通常の入力キーに加える。
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Right:
                    case Keys.Left:
                    case Keys.Home:
                    case Keys.End:
                        //case Keys.Tab:
                        //case Keys.Enter:
                        return true; // 編集コントロールに処理
                    default:
                        return false; // DataGridView に処理
                }
            }
            else
            {
                // DataGridView に処理されないキー
                return true; // 編集コントロールに処理
            }
        }

        /// <summary>マウスカーソルがEditingPanel上にあるときのカーソル</summary>
        /// <remarks>EditingPanel：編集コントロールをホストするパネル</remarks>
        public Cursor EditingPanelCursor
        {
            get
            {
                // ベースに委譲
                return base.Cursor;
            }
        }

        /// <summary>コントロールで編集する準備をする</summary>
        /// <param name="selectAll"></param>
        /// <remarks>テキストを選択状態にしたり、挿入ポインタを末尾にしたりする</remarks>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // カスタム コントロールに実装
            // されているためコメントアウト

            //if (selectAll)
            //{
            //    //選択状態にする
            //    this.SelectAll();
            //}
            //else
            //{
            //    //挿入ポインタを末尾にする
            //    this.SelectionStart = this.TextLength;
            //}
        }

        /// <summary>
        /// 値が変更した時に、セルの位置を変更するかどうか
        /// </summary>
        /// <remarks>
        /// true：大きさを変更する場合
        /// false：大きさを変更しない場合
        /// </remarks>
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                // 大きさを変更しない
                return false;
            }
        }

        #endregion

        /// <summary>Text値が変更された時</summary>
        protected override void OnTextChanged(EventArgs e)
        {
            // ベースに委譲 
            base.OnTextChanged(e);

            // 値が変更された
            this.EditingControlValueChanged = true;
            // ことをDataGridViewに通知する
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);

            //// 単位変換無し。
            //if (this.DisplayUnits == null)
            //{
            //    // 値が変更された
            //    this.EditingControlValueChanged = true;
            //    // ことをDataGridViewに通知する
            //    this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            //}
        }

        ///// <summary>Value値が変更された時</summary>
        //protected override void OnValueChanged(EventArgs e)
        //{
        //    // ベースに委譲 
        //    base.OnValueChanged(e);

        //    // 単位変換有り。
        //    if (this.DisplayUnits != null)
        //    {
        //        // 値が変更された
        //        this.EditingControlValueChanged = true;
        //        // ことをDataGridViewに通知する
        //        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        //    }
        //}
    }
}
