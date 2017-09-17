//**********************************************************************************
//* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Form3
//* クラス日本語名  ：サンプル アプリ画面
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Business.RichClient.Util;
using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Util;

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

        /// <summary>UOC_btnButton1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_btnButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_btnButton1_Click");
        }

        /// <summary>UOC_pbxPictureBox1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_pbxPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_pbxPictureBox1_Click");
        }

        /// <summary>UOC_rbnRadioButton1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_rbnRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_rbnRadioButton1_CheckedChanged");
        }

        /// <summary>UOC_cbxCheckBox1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_cbxCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_cbxCheckBox1_CheckedChanged");
        }

        /// <summary>UOC_cbbComboBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_cbbComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_cbbComboBox1_SelectedIndexChanged");
        }

        /// <summary>UOC_lbxListBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_lbxListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_lbxListBox1_SelectedIndexChanged");
        }

        /// <summary>UOC_tsmiItem1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_tsmiItem1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_tsmiItem1_Click");
        }

        /// <summary>UOC_tsmiItem2_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_tsmiItem2_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_tsmiItem2_Click");
        }

        /// <summary>UOC_tsmiItem21_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_tsmiItem21_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_tsmiItem21_Click");
        }

        /// <summary>UOC_tsmiItem22_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_tsmiItem22_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_tsmiItem22_Click");
        }

        /// <summary>UOC_tsmiItem221_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_tsmiItem221_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_tsmiItem221_Click");
        }

        /// <summary>UOC_tsmiItem222_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_tsmiItem222_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_tsmiItem222_Click");
        }

        /// <summary>UOC_tsmiItem3_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_tsmiItem3_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_tsmiItem3_Click");
        }

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

        /// <summary>バックグラウンド・スレッド</summary>
        private void ThMe()
        {
            throw new Exception("てすと");
        }

        #endregion

        #region Formイベント
        // プロジェクト独自

        /// <summary>UOC_Form_Enter_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_Enter_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_Enter_KeyDown");
        }

        /// <summary>UOC_Form_F1_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F1_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F1_KeyDown");
        }

        /// <summary>UOC_Form_F2_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F2_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F2_KeyDown");
        }

        /// <summary>UOC_Form_F3_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F3_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F3_KeyDown");
        }

        /// <summary>UOC_Form_F4_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F4_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F4_KeyDown");
        }

        /// <summary>UOC_Form_F5_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F5_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F5_KeyDown");
        }

        /// <summary>UOC_Form_F6_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F6_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F6_KeyDown");
        }

        /// <summary>UOC_Form_F7_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F7_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F7_KeyDown");
        }

        /// <summary>UOC_Form_F8_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F8_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F8_KeyDown");
        }

        /// <summary>UOC_Form_F9_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F9_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F9_KeyDown");
        }

        /// <summary>UOC_Form_F10_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F10_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F10_KeyDown");
        }

        /// <summary>UOC_Form_F11_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F11_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F11_KeyDown");
        }

        /// <summary>UOC_Form_F12_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_F12_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_F12_KeyDown");
        }

        /// <summary>UOC_Form_AltAndF4_KeyDown</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_AltAndF4_KeyDown(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_AltAndF4_KeyDown");
        }

        /// <summary>UOC_Form_Closing</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_Form_Closing(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_Form_Closing");
        }

        #endregion

        #region 未解放イベント
        // ログが出過ぎるので

        ///// <summary>UOC_Form_KeyDown</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_Form_KeyDown(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_Form_KeyDown");
        //}

        ///// <summary>UOC_Form_KeyPress</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_Form_KeyPress(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_Form_KeyPress");
        //}

        ///// <summary>UOC_Form_KeyUp</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_Form_KeyUp(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_Form_KeyUp");
        //}

        #endregion

        #region UserControlイベント

        #region userControl3

        // UserControlよりFormに実装されたメソッドが優先される。
        // ※ ボタン名は一意である必要がある（イベントを識別できなくなる）。

        ///// <summary>UOC_userControl31_btnUCButton1_Click</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_userControl3_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_userControl3_btnUCButton1_Click");
        //}

        ///// <summary>UOC_userControl31_btnUCButton1_Click</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_userControl3_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_userControl3_pbxUCPictureBox1_Click");
        //}

        ///// <summary>UOC_userControl31_btnUCButton1_Click</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_userControl3_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_userControl3_rbnUCRadioButton1_CheckedChanged");
        //}

        ///// <summary>UOC_userControl31_btnUCButton1_Click</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_userControl3_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_userControl3_cbxUCCheckBox1_CheckedChanged");
        //}

        ///// <summary>UOC_userControl31_btnUCButton1_Click</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_userControl3_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_userControl3_cbbUCComboBox1_SelectedIndexChanged");
        //}

        ///// <summary>UOC_userControl31_btnUCButton1_Click</summary>
        ///// <param name="rcFxEventArgs">RcFxEventArgs</param>
        //protected void UOC_userControl3_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        //{
        //    Debug.WriteLine("UOC_userControl3_lbxUCListBox1_SelectedIndexChanged");
        //}
        
        #endregion

        #region userControl31

        /// <summary>UOC_userControl31_btnUCButton1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl31_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl31_btnUCButton1_Click");
        }

        /// <summary>UOC_userControl31_pbxUCPictureBox1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl31_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl31_pbxUCPictureBox1_Click");
        }

        /// <summary>UOC_userControl31_rbnUCRadioButton1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl31_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl31_rbnUCRadioButton1_CheckedChanged");
        }

        /// <summary>UOC_userControl31_cbxUCCheckBox1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl31_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl31_cbxUCCheckBox1_CheckedChanged");
        }

        /// <summary>UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged");
        }

        /// <summary>UOC_userControl31_lbxUCListBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl31_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl31_lbxUCListBox1_SelectedIndexChanged");
        }

        #endregion

        #region userControl32

        /// <summary>UOC_userControl32_btnUCButton1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl32_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl32_btnUCButton1_Click");
        }

        /// <summary>UOC_userControl32_pbxUCPictureBox1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl32_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl32_pbxUCPictureBox1_Click");
        }

        /// <summary>UOC_userControl32_rbnUCRadioButton1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl32_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl32_rbnUCRadioButton1_CheckedChanged");
        }

        /// <summary>UOC_userControl32_cbxUCCheckBox1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl32_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl32_cbxUCCheckBox1_CheckedChanged");
        }

        /// <summary>UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged");
        }

        /// <summary>UOC_userControl32_lbxUCListBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControl32_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControl32_lbxUCListBox1_SelectedIndexChanged");
        }

        #endregion

        #region userControlChild

        /// <summary>UOC_userControlChild_btnUCButton1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_userControlChild_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_userControlChild_btnUCButton1_Click");
        }

        #endregion

        #endregion

        #region UserControlの動的な追加/削除

        /// <summary>UOC_btnUCAdd_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_btnUCAdd_Click(RcFxEventArgs rcFxEventArgs)
        {
            // userControl32の動的ロード
            UserControl3 userControl32 = new UserControl3();
            userControl32.Location = new System.Drawing.Point(8, 23);
            userControl32.Margin = new Padding(5);
            userControl32.Name = "userControl32";
            userControl32.Size = new System.Drawing.Size(283, 330);
            userControl32.TabIndex = 0;
            this.groupBox3.Controls.Add(userControl32);

            // userControlParentの動的ロード
            UserControlParent userControlParent = new UserControlParent();
            userControlParent.Location = new System.Drawing.Point(5, 17);
            userControlParent.Name = "userControlParent";
            userControlParent.Size = new System.Drawing.Size(160, 40);
            userControlParent.TabIndex = 3;
            this.groupBox4.Controls.Add(userControlParent);
        }

        /// <summary>UOC_btnUCRemove_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_btnUCRemove_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.groupBox3.Controls.RemoveByKey("userControl32");
            this.groupBox4.Controls.RemoveByKey("userControlParent");
        }
        
        /// <summary>MethodInvoker</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">T</param>
        delegate void MethodInvoker<T>(T obj);

        /// <summary>動的に追加したコントロールをLstUserControlに追加する</summary>
        /// <param name="sender">object</param>
        /// <param name="e">ControlEventArgs</param>
        private void groupBox_ControlAdded(object sender, ControlEventArgs e)
        {
            // UOC_イベントハンドラ内で追加/削除すると例外が発生するのでBeginInvokeで書く。
            this.BeginInvoke(
                (MethodInvoker<Control>)((x) =>
                {
                    // UserControlの追加処理
                    if (x is UserControl)
                    {
                        // コントロール検索＆イベントハンドラ設定（ルートから１回だけ行う）
                        RcFxCmnFunction.GetCtrlAndSetClickEventHandler2(
                            x, this.CreatePrefixAndEvtHndHt(), this.ControlHt);   // Base
                        RcMyCmnFunction.GetCtrlAndSetClickEventHandler2(
                            x, this.MyCreatePrefixAndEvtHndHt(), this.ControlHt); // MyBase

                        // UserControlのLstUserControlへの追加（は再帰的に行う）
                        this.AddToLstUserControl(x);
                    }
                }),
                new object[] { e.Control });
        }

        /// <summary>AddToLstUserControl</summary>
        /// <param name="c">Control</param>
        private void AddToLstUserControl(Control c)
        {
            // UserControlの追加
            if (c is UserControl)
            {
                this.LstUserControl.Add((UserControl)c);
            }

            // 再帰検索
            foreach (Control _c in c.Controls)
            {
                this.AddToLstUserControl(_c);
            }
        }

        /// <summary>動的に追加したコントロールをLstUserControlから削除する</summary>
        /// <param name="sender">object</param>
        /// <param name="e">ControlEventArgs</param>
        private void groupBox_ControlRemoved(object sender, ControlEventArgs e)
        {
            // UOC_イベントハンドラ内で追加/削除すると例外が発生するのでBeginInvokeで書く。
            this.BeginInvoke(
                (MethodInvoker<Control>) ((x) =>
                {
                    // UserControlのLstUserControlからの削除（は再帰的に行う）
                    if (x is UserControl)
                    {
                        this.RemoveFromLstUserControl(x);
                    }
                }),
                new object[] { e.Control });
        }

        /// <summary>RemoveFromLstUserControl</summary>
        /// <param name="c">Control</param>
        private void RemoveFromLstUserControl(Control c)
        {
            // UserControlの削除
            if (c is UserControl)
            {
                this.LstUserControl.Remove((UserControl)c);
            }

            // 再帰検索
            foreach (Control _c in c.Controls)
            {
                this.RemoveFromLstUserControl(_c);
            }
        }

        #endregion
    }
}