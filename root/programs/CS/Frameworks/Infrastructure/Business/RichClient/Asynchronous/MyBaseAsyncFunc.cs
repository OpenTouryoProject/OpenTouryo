//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：MyBaseAsyncFunc
//* クラス日本語名  ：非同期コード親クラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/10/29  西野 大介         新規作成
//*  2010/12/06  西野 大介         スタート メソッドの追加
//*  2010/12/06  西野 大介         スレッド数管理と画面ロック、アンロック
//*  2011/02/27  西野 大介         上記処理をクリティカルセクションに格納
//**********************************************************************************

using System;
using System.Threading;

using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.RichClient.Asynchronous
{
    /// <summary>
    /// 非同期コード親クラス２
    /// </summary>
    public class MyBaseAsyncFunc : BaseAsyncFunc
    {
        /// <summary>ロック オブジェクト</summary>
        /// <remarks>クリティカルセクション化のため</remarks>
        private static object _lock = new object();

        /// <summary>ログ出力の可否フラグ</summary>
        /// <remarks>
        /// 自動デプロイ環境ではログ出力が出来ない
        /// ことがあるので、その場合に利用すること。
        /// </remarks>
        protected static bool CanOutPutLog = true;

        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        /// <summary>コンストラクタ</summary>
        /// <param name="_this">WPFやWinFormの要素</param>
        public MyBaseAsyncFunc(object _this) : base(_this) { }

        /// <summary>開始処理</summary>
        protected override void UOC_Pre()
        {
            // ACCESSログ出力 ----------------------------------------------

            if (MyBaseAsyncFunc.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + "－" +
                    "," + "-----*" +
                    "," + this.UIElementName +
                    "," + this.AsyncFunc.Method.Name;

                // Log4Netへログ出力
                LogIF.InfoLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------

            // 性能測定開始
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();
        }

        /// <summary>終了処理</summary>
        protected override void UOC_After()
        {
            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            // ACCESSログ出力 ----------------------------------------------

            if (MyBaseAsyncFunc.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + "－" +
                    "," + "*-----" +
                    "," + this.UIElementName +
                    "," + this.AsyncFunc.Method.Name +
                    "," + perfRec.ExecTime +
                    "," + perfRec.CpuTime;

                // Log4Netへログ出力
                LogIF.InfoLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------
        }

        /// <summary>例外処理</summary>
        protected override void UOC_ABEND(Exception ex)
        {
            // 性能測定終了

            // イベント処理開始前にエラーが発生した場合は、
            // this.perfRecがnullの場合があるので、null対策コードを挿入する。
            if (this.perfRec == null)
            {
                // nullの場合、新しいインスタンスを生成し、性能測定開始。
                this.perfRec = new PerformanceRecorder();
                perfRec.StartsPerformanceRecord();
            }

            this.perfRec.EndsPerformanceRecord();

            // ACCESSログ出力-----------------------------------------------

            if (MyBaseAsyncFunc.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + "－" +
                    "," + "*-----" +
                    "," + this.UIElementName +
                    "," + this.AsyncFunc.Method.Name +
                    "," + this.perfRec.ExecTime +
                    "," + this.perfRec.CpuTime +
                    "," + ex.Message;

                // Log4Netへログ出力
                LogIF.WarnLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------    
        }

        /// <summary>最終処理</summary>
        protected override void UOC_Finally()
        {
            // ★ ここのクリティカルセクションで
            // ★ 同期呼び出し（Invoke）すると、
            // ★ デッドロックが発生するので注意する。
            lock (MyBaseAsyncFunc._lock)
            {
                // スレッド数デクリメント＆画面アンロック
                BaseAsyncFunc.ThreadCount--;
                this.WindowUnlock();
            }
        }

        #region 開始方法の既定

        /// <summary>開始方法を規定する</summary>
        /// <returns>
        /// true：開始した
        /// false：開始できなかった
        /// </returns>
        public bool Start()
        {
            lock (MyBaseAsyncFunc._lock)
            {
                // スレッド数の最大数（既定値は、１）
                int maxThreadCount = FxCmnFunction.GetNumFromConfig(FxLiteral.MAX_THREAD_COUNT, 1);

                if (BaseAsyncFunc.ThreadCount >= maxThreadCount)
                {
                    // 開始できなかった
                    return false;
                }

                // スレッド数インクリメント＆画面ロック
                BaseAsyncFunc.ThreadCount++;
                this.WindowLock();

                // 非同期実行（スレッド）
                Thread th = new Thread(this.CmnCallback);
                th.Start();

                // 開始した
                return true;
            }
        }

        /// <summary>開始方法を規定する（スレッドプール）</summary>
        /// <returns>
        /// true：開始した
        /// false：開始できなかった
        /// </returns>
        public bool StartByThreadPool()
        {
            lock (MyBaseAsyncFunc._lock)
            {
                // スレッド数の最大数（既定値は、１）
                int maxThreadCount
                    = FxCmnFunction.GetNumFromConfig(FxLiteral.MAX_THREAD_COUNT, 1);

                if (BaseAsyncFunc.ThreadCount >= maxThreadCount)
                {
                    // 開始できなかった
                    return false;
                }

                // スレッド数インクリメント＆画面ロック
                BaseAsyncFunc.ThreadCount++;
                this.WindowLock();

                // 非同期実行（スレッドプール）
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.CmnCallbackP), this);

                // 開始した
                return true;
            }
        }

        #endregion
    }
}
