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
//* クラス名        ：CustMsgBox
//* クラス日本語名  ：カスタムのメッセージ ボックス画面
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/06/16  西野 大介         新規作成
//*  2011/07/01  西野 大介         アイコンの変更を可能に
//**********************************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeployZipPackWithHTTP
{
    /// <summary>カスタムのメッセージ ボックス画面</summary>
    public partial class CustMsgBox : Form
    {
        /// <summary>タイトル</summary>
        public string Title { set; private get; }
        /// <summary>メッセージ</summary>
        public string Message { set; private get; }
        /// <summary>メッセージ</summary>
        public Icon SysIco { set; private get; }

        /// <summary>コンストラクタ</summary>
        /// <param name="title">タイトル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="sysIco">アイコン</param>
        public CustMsgBox(string title, string message, Icon sysIco)
        {
            this.Title = title;
            this.Message = message;
            this.SysIco = sysIco;

            InitializeComponent();
        }

        /// <summary>ロード</summary>
        private void CustMsgBox_Load(object sender, EventArgs e)
        {
            this.Text = this.Title;
            this.textBox1.Text = this.Message;

            Bitmap bmp = new Bitmap(288, 32);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawIcon(this.SysIco, 32, 0); // SystemIcons.Error
            pictureBox1.Image = bmp; 
        }
    }
}
