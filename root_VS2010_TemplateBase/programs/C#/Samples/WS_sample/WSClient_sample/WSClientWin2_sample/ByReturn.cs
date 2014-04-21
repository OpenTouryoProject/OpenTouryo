//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：ByReturn
//* クラス日本語名  ：初期化画面
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

namespace WSClientWin2_sample
{
    /// <summary>ByReturn</summary>
    public partial class ByReturn : MyBaseControllerWin
    {
        /// <summary>コンストラクタ</summary>
        public ByReturn()
        {
            InitializeComponent();

            Program.FlagEnd = true; //フラグ初期化
        }

        /// <summary>現在の折り返し処理回数</summary>
        private int Current = 1;

        /// <summary>フォームロードのUOCメソッド</summary>
        protected override void UOC_FormInit()
        {
            int wait = 1;
            int max = 5;

            MyBaseAsyncFunc af = new MyBaseAsyncFunc(this);

            // 非同期処理本体
            af.AsyncFunc = delegate(object param)
            {
                for (this.Current = 1; this.Current <= max; this.Current++)
                {
                    // ダミー
                    System.Threading.Thread.Sleep(wait * 1000);

                    // 進捗表示
                    af.ExecChangeProgress(string.Format(
                        "処理中です・・・:{0}/{1}", this.Current.ToString(), max.ToString()));
                }

                return "処理が完了しました。";
            };

            // 進捗報告・無名関数デレゲード
            af.ChangeProgress = delegate(object param)
            {
                this.label1.Text = (string)param;
            };

            // 結果設定・無名関数デレゲード
            af.SetResult = delegate(object retVal)
            {
                if (retVal is Exception)
                {
                    // 例外発生時
                    this.label1.Text = (retVal as Exception).ToString();
                }
                else
                {
                    this.label1.Text = (string)retVal;
                    this.btnStart.Visible = true;
                }
            };

            if (af.Start())
            {
                //正常に実行
                this.label1.Text = string.Format(
                    "処理中です・・・:{0}/{1}", this.Current.ToString(), max.ToString());
            }
            else
            {
                // ここは通らないが念のため
                this.label1.Text = string.Format(
                    "非同期スレッドが最大数に達しています。:{0}", BaseAsyncFunc.ThreadCount.ToString());
            }
        }

        /// <summary>開始</summary>
        private void UOC_btnStart_Click(RcFxEventArgs rcFxEventArgs)
        {
            Program.FlagEnd = false; // フラグ完了
            this.Close();
        }
    }
}