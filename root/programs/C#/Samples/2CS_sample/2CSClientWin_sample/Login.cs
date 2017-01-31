//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：login
//* クラス日本語名  ：ログイン画面
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
using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

namespace _2CSClientWin_sample
{
    /// <summary>login</summary>
    public partial class Login : MyBaseControllerWin
    {
        /// <summary>コンストラクタ</summary>
        public Login()
        {
            InitializeComponent();

            Program.FlagEnd = true; //フラグ初期化
        }

        /// <summary>フォームロードのUOCメソッド</summary>
        protected override void UOC_FormInit()
        {   
        }

        /// <summary>ログイン</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            MyBaseControllerWin.UserInfo.UserName = this.textBox1.Text;
            MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName;

            Program.FlagEnd = false; // フラグ完了
            this.Close();
        }
    }
}