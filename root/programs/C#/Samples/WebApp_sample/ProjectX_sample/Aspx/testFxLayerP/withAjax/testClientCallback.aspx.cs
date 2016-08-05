//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_withAjax_testClientCallback
//* クラス日本語名  ：ClientCallbackのテスト画面（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace ProjectX_sample.Aspx.testFxLayerP.withAjax
{
    /// <summary>ClientCallbackのテスト画面（Ｐ層）</summary>
    public partial class testClientCallback : MyBaseController,
        System.Web.UI.ICallbackEventHandler // 注意：System.Web.UI.ICallbackEventHandlerの実装のため必要になる。
    {
        #region ページロードのUOCメソッド

        /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する
            // TODO:

            // クライアント コールバックを有効にする
            // Init、PostBackの双方で都度実行する必要がある。
            string CallServerScript = this.InitClientCallback();

            // イベント（ロスト フォーカス）にWebForm_DoCallbackを仕掛ける。

            // スクリプトの編集
            CallServerScript = CallServerScript.Replace("$SvrParam$", "this.value");
            CallServerScript = CallServerScript.Replace("$ClientCallbackReceiveEventHandler$", "CCREH_Reverse");
            CallServerScript = CallServerScript.Replace("$CliParam$", "this.name");

            // スクリプトの設定
            ((TextBox)this.GetMasterWebControl("TextBox1")).Attributes.Add("onblur", CallServerScript);
            ((TextBox)this.GetContentWebControl("TextBox1")).Attributes.Add("onblur", CallServerScript);
        }

        /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する
            // TODO:

            // クライアント コールバックを有効にする
            // Init、PostBackの双方で都度実行する必要がある。
            string CallServerScript = this.InitClientCallback();
        }

        /// <summary>クライアント コールバックを有効にする。</summary>
        private string InitClientCallback()
        {
            //// 第一引数：サーバの画面インスタンス
            //// 第二引数：WebForm_DoCallback（サーバ呼び出し）関数への引数
            ////           getElementById（javascript） ＋ Control.ClientIDなどを併用することもある。

            //// 第三引数：コールバック関数
            //// 第四引数：コールバック関数への引数
            ////           getElementById（javascript） ＋ Control.ClientIDなどを併用することもある。

            // 第二引数、第三引数は可変にする。
            return this.ClientScript.GetCallbackEventReference(this,
                "$SvrParam$", "$ClientCallbackReceiveEventHandler$", "$CliParam$");
        }

        #endregion

        /// <summary>btnButton1のクリックイベント</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // なにもしない。
            return "";
        }

        #region ICallbackEventHandler メンバ

        // 処理結果を記憶する変数
        private string CallbackResult = "";

        /// <summary>ClientCallbackにおいて、処理を実行するイベント ハンドラ</summary>
        /// <param name="eventArgument"></param>
        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            StringBuilder sbr = new StringBuilder();

            for (int i = eventArgument.Length - 1; i >= 0; i--)
            {
                sbr.Append(eventArgument[i]);
            }

            // 処理結果を設定する。
            this.CallbackResult = sbr.ToString(); ;
        }

        /// <summary>ClientCallbackにおいて、値を戻すイベント ハンドラ</summary>
        /// <returns>処理結果</returns>
        string ICallbackEventHandler.GetCallbackResult()
        {
            //// テスト用スリープ
            //System.Threading.Thread.Sleep(3000);

            // 処理結果を値を戻す。
            return this.CallbackResult;
        }

        #endregion
    }
    
}