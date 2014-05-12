﻿//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：SimpleTextBoxWindow
//* クラス日本語名  ：D層自動生成ツール（墨壺）：（簡易情報確認画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野  大介        新規作成
//**********************************************************************************

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// 業務フレームワーク（参照しない）
// フレームワーク（参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace DaoGen_Tool
{
    /// <summary>墨壺 - 簡易情報確認画面</summary>
    public partial class SimpleTextBoxWindow : Form
    {
        // 引数
        public string _title = "";
        public string _param = "";

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public SimpleTextBoxWindow()
        {
            InitializeComponent();

            #region フローレイアウト風にする。

            // テキスト
            this.tbxMulti.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            // ボタン
            this.btnClose.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            #endregion
        }
        
        /// <summary>初期処理</summary>
        private void SimpleTextBoxWindow_Load(object sender, EventArgs e)
        {
            this.Text = this._title;
            this.tbxMulti.Text = this._param;
        }

        #endregion

        /// <summary>閉じるボタン</summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }
}
