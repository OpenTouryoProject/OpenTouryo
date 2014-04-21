//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Form2
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

using System.Collections.Generic;

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
