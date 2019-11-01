//**********************************************************************************
//* ３層型 サンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：AsyncFunc
//* クラス日本語名  ：サンプル アプリ 非同期処理クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using WSIFType_sample;

using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.Transmission;

namespace WSClientWPF_sample
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
            //MessageBoxResult mr = this.ShowAsyncMessageBoxWPF(
            //    "メッセージ", "タイトル", MessageBoxButton.YesNo, MessageBoxImage.Information);
            //// 非同期メッセージボックス表示のテスト（エラー）
            //System.Windows.Forms.DialogResult dr = this.ShowAsyncMessageBoxWin("メッセージ", "タイトル",
            //    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information);

            // 結果表示
            return testReturnValue;
        }
    }
}
