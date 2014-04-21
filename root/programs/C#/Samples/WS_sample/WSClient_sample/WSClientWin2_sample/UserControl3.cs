//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：UserControl3
//* クラス日本語名  ：ユーザコントロール
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Business.RichClient.Presentation;

using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace WSClientWin2_sample
{
    public partial class UserControl3 : UserControl
    {
        public UserControl3()
        {
            InitializeComponent();
        }

        protected void UOC_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_btnUCButton1_Click");
        }

        protected void UOC_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_pbxUCPictureBox1_Click");
        }

        protected void UOC_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_rbnUCRadioButton1_CheckedChanged");
        }

        protected void UOC_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_cbxUCCheckBox1_CheckedChanged");
        }

        protected void UOC_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_cbbUCComboBox1_SelectedIndexChanged");
        }

        protected void UOC_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("UOC_lbxUCListBox1_SelectedIndexChanged");
        }
    }
}
