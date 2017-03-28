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
//* クラス名        ：Form1
//* クラス日本語名  ：動的パラメタライズド・クエリ実行ツール（設定入力画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*
//*  2014/04/24  Rituparna         Created Resource files for UI language changes and moved
//*                                the English and Japanese languages to proper Resouce files.
//**********************************************************************************

using System;
using System.Windows.Forms;

namespace DPQuery_Tool
{
    /// <summary>動的パラメタライズド・クエリ実行ツール（設定入力画面）</summary>
    public partial class InputDialog : Form
    {
        /// <summary>コンストラクタ</summary>
        public InputDialog()
        {
            InitializeComponent();

            // 固定
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        /// <summary>パスワードかどうかのフラグ</summary>
        public bool _isPWD = false;

        /// <summary>ラベル</summary>
        public string _lbl = "";
        /// <summary>入力値</summary>
        public string _ret = "";

        /// <summary>
        /// ラベルに値を設定する。
        /// </summary>
        private void InputDialog_Load(object sender, EventArgs e)
        {
            // パスワードの場合
            if(this._isPWD)
            {
                this.textBox1.PasswordChar = '*';
                this.textBox1.MaxLength = 100;
            }           

            // ラベル設定
            this.label1.Text = this._lbl;
        }

        /// <summary>
        /// OK
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            this._ret = textBox1.Text;
            this.Close();
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this._ret = "";
            this.Close();
        }

        /// <summary>
        /// テキスト ボックス内でエンターが押された場合の処理
        /// </summary>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // エンターが押されたら、
            if (e.KeyChar == (char)Keys.Return)
            {
                // OKボタンのクリック イベントを発生させる。
                this.btnOK.PerformClick();
            }
        }
    }
}
