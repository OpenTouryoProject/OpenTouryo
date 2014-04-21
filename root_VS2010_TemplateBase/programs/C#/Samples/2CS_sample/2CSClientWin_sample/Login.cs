//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：login
//* クラス日本語名  ：ログイン画面
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