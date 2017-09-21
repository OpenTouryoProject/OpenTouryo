//**********************************************************************************
//* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：UserControl3
//* クラス日本語名  ：UserControl3
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System.Diagnostics;
using System.Windows.Forms;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace WSClientWin2_sample
{
    /// <summary>UserControl3</summary>
    public partial class UserControl3 : UserControl
    {
        /// <summary>constructor</summary>
        public UserControl3()
        {
            InitializeComponent();
        }

        /// <summary>UOC_btnUCButton1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_btnUCButton1_Click");
        }

        /// <summary>UOC_pbxUCPictureBox1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_pbxUCPictureBox1_Click");
        }

        /// <summary>UOC_rbnUCRadioButton1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_rbnUCRadioButton1_CheckedChanged");
        }

        /// <summary>UOC_cbxUCCheckBox1_CheckedChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_cbxUCCheckBox1_CheckedChanged");
        }

        /// <summary>UOC_cbbUCComboBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_cbbUCComboBox1_SelectedIndexChanged");
        }

        /// <summary>UOC_lbxUCListBox1_SelectedIndexChanged</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_lbxUCListBox1_SelectedIndexChanged");
        }
    }
}
