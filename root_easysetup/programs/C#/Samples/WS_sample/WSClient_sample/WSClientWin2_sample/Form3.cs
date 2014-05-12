//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Form3
//* クラス日本語名  ：サンプル アプリ画面
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Business.RichClient.Presentation;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using System.Threading;
using System.Collections.Generic;

namespace WSClientWin2_sample
{
    public partial class Form3 : MyBaseControllerWin
    {
        /// <summary>コンストラクタ</summary>
        public Form3()
        {
            InitializeComponent();
        }

        /// <summary>フォームロードのUOCメソッド</summary>
        protected override void UOC_FormInit()
        {
            // 表示する。
            this.ContextMenuStrip = this.contextMenuStrip1;

            // ここで設定する。
            this.contextMenuStrip1.Items[0].Click += new EventHandler(this.Item_Click);
            this.contextMenuStrip1.Items[1].Click += new EventHandler(this.Item_Click);
            this.contextMenuStrip1.Items[2].Click += new EventHandler(this.Item_Click);

            this.tsmiItem21ToolStripMenuItem.Click += new EventHandler(this.Item_Click);
            this.tsmiItem22ToolStripMenuItem.Click += new EventHandler(this.Item_Click);
            this.tsmiItem221ToolStripMenuItem.Click += new EventHandler(this.Item_Click);
            this.tsmiItem222ToolStripMenuItem.Click += new EventHandler(this.Item_Click);
        }

        #region Ctrlイベント

        protected void UOC_btnButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_btnButton1_Click");
        }

        protected void UOC_pbxPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_pbxPictureBox1_Click");
        }

        protected void UOC_rbnRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_rbnRadioButton1_CheckedChanged");
        }

        protected void UOC_cbxCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_cbxCheckBox1_CheckedChanged");
        }

        protected void UOC_cbbComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_cbbComboBox1_SelectedIndexChanged");
        }

        protected void UOC_lbxListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_lbxListBox1_SelectedIndexChanged");
        }

        protected void UOC_tsmiItem1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_tsmiItem1_Click");
        }

        protected void UOC_tsmiItem2_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_tsmiItem2_Click");
        }

        protected void UOC_tsmiItem21_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_tsmiItem21_Click");
        }

        protected void UOC_tsmiItem22_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_tsmiItem22_Click");
        }

        protected void UOC_tsmiItem221_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_tsmiItem221_Click");
        }

        protected void UOC_tsmiItem222_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_tsmiItem222_Click");
        }

        protected void UOC_tsmiItem3_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_tsmiItem3_Click");
        }
        
        //---

        // UserControlよりFormに実装されたメソッドが優先される。
        // ※ ボタン名は一意である必要がある（イベントを識別できなくなる）。

        //protected void UOC_userControl3_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_userControl3_btnUCButton1_Click");
        //}

        //protected void UOC_userControl3_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_userControl3_pbxUCPictureBox1_Click");
        //}

        //protected void UOC_userControl3_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_userControl3_rbnUCRadioButton1_CheckedChanged");
        //}

        //protected void UOC_userControl3_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_userControl3_cbxUCCheckBox1_CheckedChanged");
        //}

        //protected void UOC_userControl3_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_userControl3_cbbUCComboBox1_SelectedIndexChanged");
        //}

        //protected void UOC_userControl3_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_userControl3_lbxUCListBox1_SelectedIndexChanged");
        //}

        /// <summary>テスト１</summary>
        protected void UOC_btnElse1_Click(RcFxEventArgs rcFxEventArgs)
        {
            // newだけした場合・・・ 
            Form f = new Form2();

            MessageBox.Show("画面総数:" + BaseControllerWin.GetWindowsCount().ToString()
                + ", Form2総数:" + BaseControllerWin.GetWindowsCount(typeof(Form2)).ToString());
        }

        /// <summary>テスト２</summary>
        protected void UOC_btnElse2_Click(RcFxEventArgs rcFxEventArgs)
        {
            //throw new Exception("てすと");

            Thread th = new Thread(new ThreadStart(this.ThMe));
            th.Start();
        }

        private void ThMe()
        {
            throw new Exception("てすと");
        }

        #endregion

        #region Formイベント
        // プロジェクト独自

        protected void UOC_Form_Enter_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_Enter_KeyDown");
        }
        protected void UOC_Form_F1_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F1_KeyDown");
        }
        protected void UOC_Form_F2_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F2_KeyDown");
        }
        protected void UOC_Form_F3_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F3_KeyDown");
        }
        protected void UOC_Form_F4_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F4_KeyDown");
        }
        protected void UOC_Form_F5_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F5_KeyDown");
        }
        protected void UOC_Form_F6_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F6_KeyDown");
        }
        protected void UOC_Form_F7_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F7_KeyDown");
        }
        protected void UOC_Form_F8_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F8_KeyDown");
        }
        protected void UOC_Form_F9_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F9_KeyDown");
        }
        protected void UOC_Form_F10_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F10_KeyDown");
        }
        protected void UOC_Form_F11_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F11_KeyDown");
        }
        protected void UOC_Form_F12_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_F12_KeyDown");
        }
        protected void UOC_Form_AltAndF4_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_AltAndF4_KeyDown");
        }
        protected void UOC_Form_Closing(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_Form_Closing");
        }

        #endregion
        
        #region 未解放イベント
        // ログが出過ぎるので

        //protected void UOC_Form_KeyDown(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_Form_KeyDown");
        //}
        //protected void UOC_Form_KeyPress(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_Form_KeyPress");
        //}
        //protected void UOC_Form_KeyUp(RcFxEventArgs rcFxEventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("UOC_Form_KeyUp");
        //}

        #endregion

        #region UserControlイベント

        protected void UOC_userControl31_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl31_btnUCButton1_Click");
        }

        protected void UOC_userControl31_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl31_pbxUCPictureBox1_Click");
        }

        protected void UOC_userControl31_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl31_rbnUCRadioButton1_CheckedChanged");
        }

        protected void UOC_userControl31_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl31_cbxUCCheckBox1_CheckedChanged");
        }

        protected void UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged");
        }

        protected void UOC_userControl31_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl31_lbxUCListBox1_SelectedIndexChanged");
        }

        // ---

        protected void UOC_userControl32_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl32_btnUCButton1_Click");
        }

        protected void UOC_userControl32_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl32_pbxUCPictureBox1_Click");
        }

        protected void UOC_userControl32_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl32_rbnUCRadioButton1_CheckedChanged");
        }

        protected void UOC_userControl32_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl32_cbxUCCheckBox1_CheckedChanged");
        }

        protected void UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged");
        }

        protected void UOC_userControl32_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_userControl32_lbxUCListBox1_SelectedIndexChanged");
        }

        #endregion
    }
}