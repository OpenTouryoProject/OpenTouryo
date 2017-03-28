//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testClientCallback
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

using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;

namespace ProjectX_sample.Aspx.TestFxLayerP.WithAjax
{
    /// <summary>ClientCallbackのテスト画面（Ｐ層）</summary>
    public partial class testClientCallback : MyBaseController,
        System.Web.UI.ICallbackEventHandler // 注意：System.Web.UI.ICallbackEventHandlerの実装のため必要になる。
    {
        #region Page LoadのUOCメソッド

        /// <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // Form初期化（初回Load）時に実行する処理を実装する
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

        /// <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // Form初期化（Post Back）時に実行する処理を実装する
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

        /// <summary>btnButton1のClickイベント</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
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
            //System.Threading.Thread.Sleep(this.SleepCnt);

            // 処理結果を値を戻す。
            return this.CallbackResult;
        }

        #endregion
    }
    
}