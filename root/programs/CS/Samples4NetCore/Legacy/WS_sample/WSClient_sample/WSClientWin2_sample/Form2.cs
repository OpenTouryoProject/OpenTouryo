//**********************************************************************************
//* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Form2
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
using System.Collections.Generic;
using System.Windows.Forms;

using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace WSClientWin2_sample
{
    /// <summary>Form2</summary>
    public partial class Form2 : MyBaseControllerWin
    {
        /// <summary>テストのため画面を識別するID</summary>
        public string ID = "";

        /// <summary>コンストラクタ</summary>
        public Form2()
        {
            InitializeComponent();

            // テストのため画面を識別するIDを付与する。
             this.ID = Guid.NewGuid().ToString();
        }

        /// <summary>フォームロードのUOCメソッド</summary>
        protected override void UOC_FormInit()
        {
            //base.UOC_FormInit();

            // 画面数とIDを画面に表示する。
            this.txtStatus.Text = string.Format("現在 {0}枚目の表示",
                MyBaseControllerWin.GetWindowsCount(this.GetType()));

            this.txtGuid.Text = string.Format("画面を識別するID:{0}", this.ID);
        }

        /// <summary>Formを識別するIDをリストする</summary>
        protected void UOC_btnFormList_Click(RcFxEventArgs rcFxEventArgs)
        {
            string temp = "";

            // 当該Formインスタンスリストを取得する。
            List<Form> fl = 
                MyBaseControllerWin.GetWindowInstances(this.GetType());

            // 表示する文字列を作成する。
            foreach (Form2 f2 in fl)
            {
                temp += "・" + f2.ID +"\r\n";
            }

            // メッセージボックスにリストする。
            MessageBox.Show(temp, "Form2のID一覧",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>全Formインスタンス数を表示する</summary>
        protected void UOC_btnFormCount_Click(RcFxEventArgs rcFxEventArgs)
        {
            // メッセージボックスに表示する。
            MessageBox.Show(BaseControllerWin.GetWindowsCount().ToString(), 
                "全Formインスタンス数", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>自分を閉じる</summary>
        protected void UOC_btnClose_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.Close();
        }
    }
}
