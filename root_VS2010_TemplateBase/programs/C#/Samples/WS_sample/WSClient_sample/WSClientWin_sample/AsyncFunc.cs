//**********************************************************************************
//* サンプル アプリ（非同期処理クラス）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：AsyncFunc
//* クラス日本語名  ：サンプル アプリ 非同期処理クラス
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

// 型情報
using WSIFType_sample;

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

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

using Touryo.Infrastructure.Framework.RichClient.Asynchronous;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace WSClientWin_sample
{
    public class AsyncFunc : MyBaseAsyncFunc
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="_this">WPFやWinFormの要素</param>
        public AsyncFunc(object _this) : base(_this) { }

        /// <summary>サービスの論理名</summary>
        public string LogicalName = "";

        /// <summary>非同期</summary>
        /// <param name="param">引数</param>
        /// <returns>結果</returns>
        /// <remarks>
        /// ここは副スレッドから実行されるので注意。
        /// 非同期処理クラスに非同期処理を定義すると、
        /// メンバ変数を引数として利用できる。
        /// </remarks>
        public object btn6_Exec(object param)
        {
            // 戻り値（キャスト）
            TestParameterValue testParameterValue = (TestParameterValue)param;

            // 戻り値
            TestReturnValue testReturnValue;

            // 呼出し制御部品（スレッドセーフでないため副スレッド内で作る）
            CallController callCtrl = new CallController("");

            // Invoke
            testReturnValue = (TestReturnValue)callCtrl.Invoke(
                this.LogicalName, testParameterValue);

            //// 進捗表示のテスト
            //this.ChangeProgress = delegate(object o)
            //{
            //    MessageBox.Show(o.ToString());
            //};

            //this.ExecChangeProgress("進捗表示");

            //// 非同期メッセージボックス表示のテスト
            //DialogResult dr = this.ShowAsyncMessageBoxWin(
            //    "メッセージ", "タイトル", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            ////// 非同期メッセージボックス表示のテスト（エラー）
            ////System.Windows.MessageBoxResult mr = this.ShowAsyncMessageBoxWPF("メッセージ", "タイトル",
            ////    System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information);

            // 結果表示
            return testReturnValue;
        }
    }
}