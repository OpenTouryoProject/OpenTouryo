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
//* クラス名        ：SetPrimaryKeyWindow
//* クラス日本語名  ：D層自動生成ツール（墨壺）：（主キー設定画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2009/12/09  西野 大介         SQL Server用主キー取得機能の追加
//*  2009/12/09  西野 大介         カラム名をキーにしたハッシュテーブルを追加
//**********************************************************************************

using System;
using System.Collections;
using System.Windows.Forms;

namespace DaoGen_Tool
{
    /// <summary>墨壺 - 主キー設定画面</summary>
    public partial class SetPrimaryKeyWindow : Form
    {
        // 引数
        public string _title = "";
        public Hashtable _htColumns_Position = null;

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public SetPrimaryKeyWindow()
        {
            InitializeComponent();

            #region フローレイアウト風にする。

            // カラム
            this.clbColumns.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            // ボタン
            this.btnClose.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            #endregion
        }

        /// <summary>初期処理</summary>
        private void SetPrimaryKeyWindow_Load(object sender, EventArgs e)
        {
            // タイトル
            this.Text = this._title;

            // チェックド リスト ボックス
            this.clbUpdateItems();
        }

        #endregion        

        #region チェックド リスト ボックス 関連

        /// <summary>チェックド リスト ボックスを更新する</summary>
        private void clbUpdateItems()
        {
            // カラム ポジションを昇順にソートする
            ArrayList sort = CmnMethods.sortColumn(this._htColumns_Position);

            // ソート後のカラム ポジション配列を廻す
            foreach (Int32 position in sort)
            {
                // カラムを取得
                CColumn column = (CColumn)this._htColumns_Position[position.ToString()];
                this.clbColumns.Items.Add(position.ToString().PadLeft(3, '0') + " : " + column.Name, column.IsKey);
            }
        }

        #endregion

        #region イベント
        
        /// <summary>チェックの変更イベント</summary>
        private void clbColumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 主キー
            string search = " : ";
            string temp = this.clbColumns.Items[e.Index].ToString();
            string columnName = temp.Substring(temp.IndexOf(search) + search.Length);

            // フラグの変更
            foreach (CColumn column in this._htColumns_Position.Values)
            {
                if (column.Name == columnName)
                {
                    if (e.CurrentValue.ToString() == "Checked")
                    {
                        // この後、アンチェック状態
                        column.IsKey = false;
                    }
                    else
                    {
                        // この後、チェック状態
                        column.IsKey = true;
                    }
                }
            }            
        }

        /// <summary>閉じるボタン</summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
