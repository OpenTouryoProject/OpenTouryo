//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：AsyncEventFx
//* クラス日本語名  ：非同期イベント フレームワーク
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/12/21  西野  大介        新規作成
//*  2011/02/21  西野  大介        メッセージＩＤ（一意性）はサポートしないようにした。
//*                                一意性を保証するなら、同期イベントフレームワークを開発。
//**********************************************************************************

// System
using System;
using System.Collections.Generic;

using System.IO;
using System.IO.Pipes;

using System.Threading;
using System.Diagnostics;

using System.Windows;
using System.Windows.Forms;

// 業務フレームワーク（循環参照になるため、参照しない）

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

namespace Touryo.Infrastructure.Framework.RichClient.Asynchronous
{
    /// <summary>非同期イベント フレームワーク</summary>
    public class AsyncEventFx
    {
        /// <summary>SetResultのDelegate型</summary>
        /// <param name="result">ヘッダ部構造体と、データ部（バイト表現）</param>
        public delegate void SetResultDelegate(object result);

        #region メンバ変数（静的変数）

        /// <summary>ロック オブジェクト</summary>
        /// <remarks>
        /// staticメンバ メソッドで利用する。
        /// インスタンス メンバ メソッドはシングルトン
        /// </remarks>
        private static object _lock = new object();

        /// <summary>名前付きパイプ サーバ</summary>
        /// <remarks>自プロセス内のサーバ１つ</remarks>
        private static NamedPipeServer NPS = null;

        /// <summary>名前付きパイプ クライアント（Dictionary化）</summary>
        /// <remarks>接続先：接続先は複数ありえるのでDic化する。</remarks>
        private static Dictionary<string, NamedPipeClient> NPCS
            = new Dictionary<string, NamedPipeClient>();

        /// <summary>接続待ちの待機時間（ミリ秒）</summary>
        /// <remarks>読み取り専用</remarks>
        private static int _waitTime_msec;

        /// <summary>接続待ちの待機時間（ミリ秒）</summary>
        /// <remarks>読み取り専用</remarks>
        public static int WaitTime_msec
        {
            private set
            {
                AsyncEventFx._waitTime_msec = value;
            }
            get
            {
                return AsyncEventFx._waitTime_msec;
            }
        }

        #endregion

        #region 開始・終了処理

        /// <summary>初期化</summary>
        /// <param name="serverPipeName">名前付きパイプ サーバの名前</param>
        /// <param name="clientPipeNames">名前付きパイプ クライアントの名前の配列</param>
        /// <param name="waitTime_msec">接続待ちの待機時間（ミリ秒）</param>
        /// <remarks>スレッド セーフ</remarks>
        public static void Init(string serverPipeName, string[] clientPipeNames, int waitTime_msec)
        {
            // 初期化前にクリーンナップ
            AsyncEventFx.Final(); // デッドロックに注意

            lock (AsyncEventFx._lock) // staticなのでロックする。
            {
                // 接続待ちの待機時間（ミリ秒）を初期化
                AsyncEventFx.WaitTime_msec = waitTime_msec;

                // 名前付きパイプ（クラ・サバ）を初期化

                // ・初めに名前付きパイプ（サーバ）を初期化（new）
                AsyncEventFx.NPS = new NamedPipeServer(serverPipeName);

                // ・次に名前付きパイプ（クライアント）を初期化（new）
                AsyncEventFx.NPCS = new Dictionary<string, NamedPipeClient>();

                foreach (string cpn in clientPipeNames)
                {
                    // ディクショナリへ追加
                    AsyncEventFx.NPCS.Add(cpn, new NamedPipeClient(cpn));
                }
            }
        }

        /// <summary>終了</summary>
        /// <remarks>スレッド セーフ</remarks>
        public static void Final()
        {
            lock (AsyncEventFx._lock) // staticなのでロックする。
            {
                // NPCS
                // GC ＋ デストラクタで問題ない。
                AsyncEventFx.NPCS = null;

                // NPS
                if (AsyncEventFx.NPS != null)
                {
                    // リスナーを停止させる必要がある（ping）。
                    // NPS.Finalは本メソッドとは別のメソッド
                    AsyncEventFx.NPS.Final();

                    // ping後は、GC ＋ デストラクタで問題ない。
                    AsyncEventFx.NPS = null;
                }
            }
        }

        #endregion

        #region 非同期イベント登録・削除

        /// <summary>非同期イベント登録</summary>
        /// <param name="aee">非同期イベント エントリ</param>
        /// <returns>
        /// ・成功：true
        /// ・失敗：false
        /// </returns>
        public static bool RegisterAsyncEvent(AsyncEventEntry aee)
        {
            lock (AsyncEventFx._lock) // staticなのでロックする。
            {
                AsyncEventFx.NPS.RegisterAsyncEvent(aee);
            }

            return true; // 将来的にfalse対応があるかもしれないので。
        }

        /// <summary>非同期イベント エントリ削除</summary>
        /// <param name="aee">非同期イベント エントリ</param>
        /// <returns>
        /// ・成功：true
        /// ・失敗：false
        /// </returns>
        /// <remarks>メッセージID付きのエントリは自動的に削除される。</remarks>
        public static bool UnRegisterAsyncEvent(AsyncEventEntry aee)
        {
            lock (AsyncEventFx._lock) // staticなのでロックする。
            {
                return AsyncEventFx.NPS.UnRegisterAsyncEvent(aee);
            }
        }

        #endregion

        #region 非同期イベント通知

        /// <summary>非同期イベント通知</summary>
        /// <param name="dstEventClass">送信先イベント区分（最大36文字）</param>
        /// <param name="dstFuncID">送信先機能ＩＤ（最大36文字）</param>
        /// <param name="srcEventClass">送信元イベント区分（最大36文字）</param>
        /// <param name="srcFuncID">送信元機能ＩＤ（最大36文字）</param>
        /// <param name="dstPipeName">送信先 名前付きパイプ名</param>
        /// <param name="srcPipeName">送信元 名前付きパイプ名</param>
        /// <param name="dataLength">データ部のバイト長</param>
        /// <param name="bodyBytes">データ部のバイト表現</param>
        /// <returns>
        /// ・成功：true
        /// ・失敗：false（名前付きパイプ クライアントが見つからない）
        /// </returns>
        /// <remarks>スレッド セーフ</remarks>
        public static bool SendAsyncEvent(
            AsyncEventEnum.EventClass dstEventClass, string dstFuncID,
            AsyncEventEnum.EventClass srcEventClass, string srcFuncID,
            string dstPipeName, string srcPipeName,
            uint dataLength, byte[] bodyBytes)
        {
            lock (AsyncEventFx._lock) // staticなのでロックする。
            {
                if (AsyncEventFx.NPCS.ContainsKey(dstPipeName))
                {
                    // 名前付きパイプ クライアントを取得
                    NamedPipeClient npc = AsyncEventFx.NPCS[dstPipeName];

                    // ヘッダ部のバイト表現を生成

                    // ヘッダ部を生成
                    AsyncEventHeader aeh = new AsyncEventHeader(
                        dstEventClass, dstFuncID,
                        srcEventClass, srcFuncID,
                        srcPipeName, dataLength);

                    // ヘッダ部のマーシャリング
                    byte[] headerBytes = CustomMarshaler.StructureToBytes(aeh);

                    // ヘッダ・データ部のバイト表現をマージ
                    byte[] bytes = new byte[headerBytes.Length + bodyBytes.Length];
                    Array.Copy(headerBytes, bytes, headerBytes.Length);
                    Array.Copy(bodyBytes, 0, bytes, headerBytes.Length, bodyBytes.Length);

                    // Threadを生成してThread関数（SendData）を実行
                    Thread th = new Thread(npc.SendData);
                    th.Start(bytes);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region プライベートクラス

        #region NamedPipeServer

        /// <summary>名前付きパイプ サーバ</summary>
        /// <remarks>
        /// ・名前付きパイプ サーバ
        /// ・非同期イベント エントリ
        /// 持つプライベート クラス
        /// </remarks>
        private class NamedPipeServer
        {
            #region メンバ変数（インスタンス変数）

            /// <summary>名前付きパイプ サーバ ストリーム</summary>
            /// <remarks>.NET3.5組み込みオブジェクト</remarks>
            private NamedPipeServerStream NPSS = null;

            /// <summary>名前付きパイプ サーバの名前</summary>
            private string NPSS_Name = "";

            /// <summary>ループ変数</summary>
            private volatile bool loop = true;

            /// <summary>非同期イベント エントリ（Dictionary化）</summary>
            /// <remarks>１つの機能ＩＤに対して複数のエントリを登録可能</remarks>
            private Dictionary<string, List<AsyncEventEntry>> AEES
                = new Dictionary<string, List<AsyncEventEntry>>();

            #endregion

            #region エントリの登録・削除

            /// <summary>非同期イベント エントリ登録</summary>
            /// <param name="aee">非同期イベント エントリ</param>
            public void RegisterAsyncEvent(AsyncEventEntry aee)
            {
                // 非同期イベント エントリのリスト
                // （１つの機能ＩＤに対して複数のエントリを登録可能）
                List<AsyncEventEntry> aee_lst = null;

                if (this.AEES.ContainsKey(aee.FuncID))
                {
                    // 既に登録済み

                    // 非同期イベント エントリのリストを取得
                    aee_lst = this.AEES[aee.FuncID];
                }
                else
                {
                    // 未登録

                    // 非同期イベント エントリのリストを生成して追加
                    aee_lst = new List<AsyncEventEntry>();
                    this.AEES.Add(aee.FuncID, aee_lst);
                }

                // 非同期イベント エントリのリストにエントリを追加
                aee_lst.Add(aee);
            }

            /// <summary>非同期イベント エントリ削除</summary>
            /// <param name="aee">非同期イベント エントリ</param>
            /// <returns>
            /// ・成功：true
            /// ・失敗：false
            /// </returns>
            /// <remarks>メッセージID付きのエントリは自動的に削除される。</remarks>
            public bool UnRegisterAsyncEvent(AsyncEventEntry aee)
            {
                // 非同期イベント エントリのリスト
                List<AsyncEventEntry> aee_lst = null;

                if (this.AEES.ContainsKey(aee.FuncID))
                {
                    // 既に登録済み

                    // 非同期イベント エントリのリストを取得
                    aee_lst = this.AEES[aee.FuncID];

                    // 非同期イベント エントリを削除
                    // IEquatable対応済み。
                    return aee_lst.Remove(aee);
                }
                else
                {
                    // 未登録
                    return false;
                }
            }

            #endregion

            #region 開始処理

            /// <summary>コンストラクタ</summary>
            /// <param name="pipeName">自身の名前付きパイプ名</param>
            public NamedPipeServer(string pipeName)
            {
                // 初期化

                // 名前を退避する。
                this.NPSS_Name = pipeName;

                // Threadを生成してThread関数（ListeningNamedPipe）を実行
                Thread th = new Thread(
                    new ThreadStart(this.ListeningNamedPipeServer));

                // 初期化としてはリスニングを開始して終わってOK！
                th.Start();
            }

            /// <summary>サーバ起動 - listenerスレッド関数</summary>
            private void ListeningNamedPipeServer()
            {
                try
                {
                    // ヘッダ部のバッファを用意
                    byte[] buff = new byte[CustomMarshaler.StructureToBytes(new AsyncEventHeader()).Length];

                    while (this.loop)// 停止されるまでループ
                    {
                        // 名前付きパイプ サーバ ストリーム×１（待機）
                        this.NPSS = new NamedPipeServerStream(this.NPSS_Name, PipeDirection.In, 1);
                        this.NPSS.WaitForConnection();
                        
                        #region 受信処理

                        // 通知処理は非同期化する
                        if (this.NPSS.Read(buff, 0, buff.Length) == 1)
                        {
                            // 制御文字を受信
                            byte b = buff[0]; // １バイトの書き込みは制御文字

                            // 制御コードとしては、0-256を利用可能
                            switch (Convert.ToUInt16(b))
                            {
                                case 0:

                                    // 終了
                                    return;

                                //case 1-256:
                                //    break;

                                default:

                                    // 現在の接続を切断してから、
                                    this.NPSS.Disconnect();
                                    this.NPSS.Close();

                                    continue; // ★ continue（whileに対応）
                            }
                        }
                        else
                        {
                            // ヘッダ部を受信

                            // データ部のマーシャリング
                            AsyncEventHeader aeh = (AsyncEventHeader)
                                CustomMarshaler.BytesToStructure(buff, typeof(AsyncEventHeader));

                            // データ部のバッファを用意
                            byte[] bodyBytes = new byte[aeh.DataLength];

                            // データ部を受信
                            if (this.NPSS.Read(bodyBytes, 0, bodyBytes.Length) == bodyBytes.Length)
                            {
                                // 正常
                            }
                            else
                            {
                                // 異常

                                // デバッグ出力
                                Debug.WriteLine(
                                    "受信した非同期イベントのデータ部の長さに不正があります。");

                                // 現在の接続を切断してから、
                                this.NPSS.Disconnect();
                                this.NPSS.Close();

                                continue; // ★ continue（whileに対応）
                            }

                            #region 非同期イベント通知処理

                            // ここは、シングルトンだが、スレッド関数で、
                            // 呼出元から切り離されるのでロック必要（AEES参照）
                            lock (this)
                            {
                                // 即切り離し
                                string dstFuncID =(new string(aeh.DstFuncID)).Trim();
                                if (this.AEES.ContainsKey(dstFuncID))
                                {
                                    List<AsyncEventEntry> aee_lst
                                        = (List<AsyncEventEntry>)this.AEES[dstFuncID];

                                    // 全エントリ（デレゲード）にブロードキャスト。
                                    foreach (AsyncEventEntry aee in aee_lst)
                                    {
                                        if ((int)aee.EventClass == aeh.DstEventClass) // イベント区分のチェック
                                        {
                                            #region 非同期呼び出し（区分ごとに呼び方が変わる）

                                            // 引数
                                            object param = new object[] { aeh, bodyBytes };

                                            switch ((int)aee.EventClass)
                                            {
                                                case (int)AsyncEventEnum.EventClass.Thread: // Thread
                                                    // ワーカ系（Thread）
                                                    // Thread.Start メソッド (Object)
                                                    Thread th = new Thread((ParameterizedThreadStart)aee.Callback);
                                                    th.Start(param);
                                                    break;

                                                case (int)AsyncEventEnum.EventClass.ThreadPool: // ThreadPool
                                                    // ワーカ系（ThreadPool）
                                                    // ThreadPool.QueueUserWorkItem メソッド (WaitCallback, Object)
                                                    ThreadPool.QueueUserWorkItem(
                                                        (WaitCallback)aee.Callback, param);
                                                    break;

                                                case (int)AsyncEventEnum.EventClass.WinForm: // WinForm
                                                    // GUI系（Windows Forms）
                                                    // Control.BeginInvoke メソッド (Delegate, Object[])
                                                    ((Control)(aee.Control)).BeginInvoke(
                                                        (SetResultDelegate)aee.Callback, param);
                                                    break;

                                                case (int)AsyncEventEnum.EventClass.WPF: // WPF
                                                    // GUI系（WPF）
                                                    // Dispatcher.BeginInvoke メソッド (Delegate, Object[])
                                                    ((DependencyObject)(aee.Control)).Dispatcher.BeginInvoke(
                                                        (SetResultDelegate)aee.Callback, param);
                                                    break;

                                                default:
                                                    break;
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            // デバッグ出力
                                            Debug.WriteLine("非同期イベントのイベント区分が不一致です。");

                                            // 現在の接続を切断してから、
                                            this.NPSS.Disconnect();
                                            this.NPSS.Close();

                                            break; // ★ foreachに対応
                                        }
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        // 現在の接続を切断してから、
                        this.NPSS.Disconnect();
                        this.NPSS.Close();
                    }
                }
                catch (Exception ex)
                {
                    // デバッグ出力
                    Debug.WriteLine("ListeningNamedPipeServer" 
                        + "でエラーが発生しました。:" + ex.ToString());
                }
                finally
                {
                    if (this.NPSS != null)
                    {
                        // 名前付きパイプ サーバ ストリームをクローズ
                        this.NPSS.Close();
                    }
                }
            }

            #endregion

            #region 終了処理

            /// <summary>終了処理</summary>
            public void Final()
            {
                // ループの終了
                this.loop = false;

                // Threadを生成してThread関数（StopListenNamedPipe）を実行
                Thread th = new Thread(
                    new ThreadStart(this.StopListenNamedPipeServer));

                th.Start();
            }

            /// <summary>リスナのループ＆待機を終了させる</summary>
            /// <remarks>非同期化しないとハングするため</remarks>
            private void StopListenNamedPipeServer()
            {
                // 名前付きパイプ クライアント ストリーム
                NamedPipeClientStream npcs = null;

                try
                {
                    // 待機していた場合、止まらないので、
                    // ここで、PING（みたいなもの）を送信する。

                    // 名前付きパイプ クライアント ストリームを生成
                    npcs = new NamedPipeClientStream(
                        ".", this.NPSS_Name, PipeDirection.Out, PipeOptions.None);

                    // 接続
                    npcs.Connect(AsyncEventFx.WaitTime_msec);

                    // 終了（１バイトのデータ）
                    byte[] byt = new byte[] { 0 };

                    // 送信
                    npcs.Write(byt, 0, 1);
                }
                catch (Exception ex)
                {
                    // デバッグ出力
                    Debug.WriteLine("StopListenNamedPipeServer"
                        + "でエラーが発生しました。:" + ex.ToString());
                }
                finally
                {
                    if (npcs != null)
                    {
                        // 名前付きパイプ クライアント ストリームをクローズ
                        npcs.Close();
                    }
                }
            }

            #endregion
        }

        #endregion

        #region NamedPipeClient

        /// <summary>名前付きパイプ クライアント</summary>
        /// <remarks>プライベート クラス</remarks>
        private class NamedPipeClient
        {
            #region メンバ変数（インスタンス変数）

            /// <summary>名前付きパイプ・クライアント</summary>
            /// <remarks>.NET3.5組み込みオブジェクト</remarks>
            private NamedPipeClientStream NPCS = null;

            /// <summary>名前付きパイプ・サーバの名前</summary>
            private string NPSS_Name = "";

            #endregion

            #region 初期処理

            /// <summary>コンストラクタ</summary>
            public NamedPipeClient(string pipeName)
            {
                // 初めに名前を覚えさせる。
                this.NPSS_Name = pipeName;
            }

            #endregion

            #region 送信処理

            /// <summary>データ送信</summary>
            /// <param name="parameter">送信データ（バイト配列）</param>
            /// <remarks>object型なのは、ThreadStartデリゲートのため</remarks>
            public void SendData(object parameter)
            {
                // ここは、スレッド関数で、
                // 呼出元から切り離されるのでロック必要。
                lock (this) 
                {
                    try
                    {
                        // コネクションは都度

                        // 名前付きパイプ クライアント ストリーム
                        this.NPCS = new NamedPipeClientStream(
                                ".", this.NPSS_Name, PipeDirection.Out, PipeOptions.None);

                        // キャスト
                        byte[] byts = (byte[])parameter;

                        // 接続
                        this.NPCS.Connect(AsyncEventFx.WaitTime_msec);

                        // 送信
                        this.NPCS.Write(byts, 0, byts.Length);
                    }
                    catch (Exception ex)
                    {
                        // デバッグ出力
                        Debug.WriteLine("SendData"
                            + "でエラーが発生しました。:" + ex.ToString());
                    }
                    finally
                    {
                        // 終了処理
                        if (this.NPCS != null)
                        {
                            // 名前付きパイプ クライアント ストリームをクローズ
                            this.NPCS.Close();
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #endregion
    }
}
