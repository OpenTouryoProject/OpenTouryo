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
//* クラス名        ：WinCustomDropDownList
//* クラス日本語名  ：コンボ ボックス（Win）のカスタム・コントロール（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System.ComponentModel;
using System.Windows.Forms;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>System.Windows.Forms.ComboBoxのカスタム・コントロール</summary>
    /// <remarks>選択専用</remarks>
    [DefaultProperty("Items")]
    public class WinCustomDropDownList : ComboBox, IMasterData
    {
        /// <summary>マスタデータ名</summary>
        public string _masterDataName = "";

        /// <summary>マスタデータ名</summary>
        [DefaultValue(""),
        Description("マスタデータ名")]
        public string MasterDataName
        {
            get { return this._masterDataName; }
            set
            {
                // 全半角スペースは詰め、大文字に揃えて設定する。
                if (value == null)
                {
                    this._masterDataName = value;
                }
                else
                {
                    this._masterDataName = value.Replace("　", "").Replace(" ", "").ToUpper();
                }
            }
        }

        #region 初期処理
        
        /// <summary>コンストラクタ</summary>
        public WinCustomDropDownList()
            : base()
        {
            this.InitializeComponent();
        }

        /// <summary>初期化</summary>
        private void InitializeComponent()
        {   
            this.SuspendLayout();
            // 
            // WinCustomComboBox
            // 
            this.ResumeLayout(false);
        }

        /// <summary>初期処理（Items）</summary>
        public void InitItems()
        {
            // マスタデータ設定
            if (this.Items.Count == 0)
            {
                CmnMasterDatasForList.GetMasterData(this.MasterDataName, this.Items);
            }
            // 初期値を設定
            if (this.Items.Count != 0)
            {
                this.DropDownStyle = ComboBoxStyle.DropDownList;
                this.SelectedIndex = 0;// Itemsの場合有効
            }
        }

        /// <summary>初期処理（DataSource）</summary>
        public void InitDataSource()
        {
            // マスタデータ設定
            if (this.Items.Count == 0)
            {
                this.DataSource = CmnMasterDatasForList.GetMasterData(this.MasterDataName);
            }
            // 初期値を設定
            if (this.Items.Count != 0)
            {
                this.DropDownStyle = ComboBoxStyle.DropDownList;
                this.SelectedIndex = 0;// Itemsの場合有効
            }
        }

        //// <summary>初期処理（Items）</summary>
        //// <remarks>
        //// コンストラクタのInitializeComponent前に実行する必要があるので、NG
        //// （コンストラクタでWS呼び出しを実装すると、デザイナが上手く表示できない。）
        //// </remarks>
        //protected override void InitLayout()
        //{
        //    base.InitLayout();

        //    // InitDataSourceは
        //    // SelectedIndex設定後の動きが微妙
        //    this.InitItems();
        //    //this.InitDataSource();

        //    // 初期値を設定
        //    if (this.Items.Count != 0)
        //    {
        //        this.DropDownStyle = ComboBoxStyle.DropDownList;
        //        this.SelectedIndex = 0;// Itemsの場合有効
        //    }
        //}

        #endregion
    }
}
