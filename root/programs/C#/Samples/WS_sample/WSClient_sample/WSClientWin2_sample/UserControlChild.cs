//**********************************************************************************
//* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：UserControlChild
//* クラス日本語名  ：UserControlChild
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Diagnostics;
using System.Windows.Forms;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace WSClientWin2_sample
{
    /// <summary>UserControlChild</summary>
    public partial class UserControlChild : UserControl
    {
        /// <summary>constructor</summary>
        public UserControlChild()
        {
            InitializeComponent();
        }

        /// <summary>UOC_btnUCButton1_Click</summary>
        /// <param name="rcFxEventArgs">RcFxEventArgs</param>
        protected void UOC_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Debug.WriteLine("UOC_btnUCButton1_Click");
        }
    }
}
