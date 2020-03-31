//**********************************************************************************
//* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Form0
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

using System.Windows.Forms;

using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace WSClientWin2_sample
{
    /// <summary>Form0</summary>
    public partial class Form0 : MyBaseControllerWin
    {
        /// <summary>コンストラクタ</summary>
        public Form0()
        {
            InitializeComponent();
        }

        /// <summary>フォームロードのUOCメソッド</summary>
        protected override void UOC_FormInit()
        {
        }

        /// <summary>Form1を表示</summary>
        private void UOC_btnOpenForm1_Click(RcFxEventArgs rcFxEventArgs)
        {
            Form f = new Form1();
            f.Show();
        }

        /// <summary>Form3を表示</summary>
        private void UOC_btnOpenForm3_Click(RcFxEventArgs rcFxEventArgs)
        {
            Form f = new Form3();
            f.Show();
        }

        /// <summary>自分を閉じる</summary>
        private void UOC_btnClose_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.Close();
        }
    }
}
