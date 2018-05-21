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
//* クラス名        ：BaseAsyncFunc
//* クラス日本語名  ：非同期コード親クラス１
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/10/29  西野 大介         新規作成
//*  2010/11/19  西野 大介         非同期メッセージボックスと、進捗報告デリゲード追加
//*  2010/12/06  西野 大介         スレッド数管理と画面ロック、アンロック
//*  2010/12/30  西野 大介         Control.Invoke前にIsDisposedをチェック(WPFでは不要)
//*  2011/02/24  西野 大介         Windows Formsの多重ロック・アンロック対応
//*  2011/05/17  西野 大介         thisWinForm, thisWPFのgetterをpublicに。
//*  2011/05/18  西野 大介         BaseControllerWin以外でも多重ロック対応（Tag利用）。
//*  2011/06/09  西野 大介         WindowUnlockデッドロック対応（Invoke→BeginInvoke）
//*  2011/06/13  西野 大介         WPFの多重ロック・アンロック対応（は不可能である模様）
//**********************************************************************************

using System;
using System.Windows;
using System.Windows.Forms;

using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Framework.Exceptions;

namespace Touryo.Infrastructure.Framework.RichClient.Asynchronous
{
    /// <summary>
    /// 非同期コード親クラス１
    /// </summary>
    /// <remarks>
    /// this.CmnCallback、CmnCallbackPは副スレッドから呼び出されるので、
    /// Control、Dispatcher.Invokeを使用してthis.SetResultを
    /// キューイングし、主スレッドから実行するようにする。
    /// ※ this.SetResultは、delegate（SetResultDelegate）
    /// 
    /// 以下コードをクラスライブラリ内で実装する場合は、
    /// PresentationFramework.dll , WindowsBase.dll
    /// の2つのアセンブリを参照設定する必要がある
    /// </remarks>
    public class BaseAsyncFunc
    {
        #region メンバ変数

        #region 静的変数

        /// <summary>スレッド数</summary>
        /// <remarks>初期値は０</remarks>
        private static volatile int _threadCount = 0;

        /// <summary>スレッド数</summary>
        /// <remarks>
        /// スレッド数を管理
        /// （ベース２から行う）
        /// </remarks>
        public static int ThreadCount
        {
            get
            {
                // get
                return BaseAsyncFunc._threadCount;
            }
            protected set
            {
                // set
                BaseAsyncFunc._threadCount = value;
            }
        }

        #endregion

        #region インスタンス変数

        #region 引数・戻り値

        /// <summary>非同期処理を実行するDelegateメソッドに渡すパラメタ</summary>
        /// <remarks>スレッドプールの場合もコチラを使用すること</remarks>
        public object Parameter { protected get; set; }

        /// <summary>各画面を更新するDelegateメソッド渡すパラメタ</summary>
        protected object ReturnValue { get; set; }

        #endregion

        #region Delegate

        /// <summary>ExecAsyncFuncのDelegate型</summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値（SetResultへ渡される）</returns>
        public delegate object AsyncFuncDelegate(object param);

        /// <summary>非同期処理を実行するDelegate</summary>
        public AsyncFuncDelegate AsyncFunc { get; set; }

        /// <summary>ChangeProgressのDelegate型</summary>
        /// <param name="param">引数</param>
        public delegate void ChangeProgressDelegate(object param);

        /// <summary>各画面を更新するDelegate（進捗の反映）</summary>
        public ChangeProgressDelegate ChangeProgress { get; set; }

        /// <summary>SetResultのDelegate型</summary>
        /// <param name="result">結果（ExecAsyncFuncの戻り値）</param>
        public delegate void SetResultDelegate(object result);

        /// <summary>各画面を更新するDelegate（結果の反映）</summary>
        /// <remarks>nullの場合は呼び出されない。</remarks>
        public SetResultDelegate SetResult { get; set; }

        /// <summary>ウィンドウのロック・アンロックのDelegate</summary>
        /// <param name="isEnabled">
        /// true：アンロック、false：ロック
        /// </param>
        private delegate void SetEnabledDelegate(bool isEnabled);

        #endregion

        #region UI要素

        /// <summary>WPFの要素（ウィンドウやコントロールなど）</summary>
        public DependencyObject thisWPF { get; set; }

        /// <summary>WinFormの要素（ウィンドウやコントロールなど）</summary>
        public Control thisWinForm { get; set; }

        /// <summary>UI要素の名称</summary>
        protected string UIElementName { get; private set; }

        #endregion

        #endregion

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="_this">WPFやWinFormの要素</param>
        public BaseAsyncFunc(object _this)
        {
            // オブジェクトの型によって振り分け
            if (_this is DependencyObject)
            {
                // WPF
                this.thisWPF = (DependencyObject)_this;
                this.thisWinForm = null;

                this.UIElementName = _this.GetType().Name;
            }
            else if (_this is Control)
            {
                // WinForm
                this.thisWPF = null;
                this.thisWinForm = (Control)_this;

                this.UIElementName = _this.GetType().Name;
            }
            else
            {
                // 型の不一致
                throw new FrameworkException(
                    FrameworkExceptionMessage.ASYNC_FUNC_CHECK_ERROR[0],
                    FrameworkExceptionMessage.ASYNC_FUNC_CHECK_ERROR[1]);
            }
        }

        #endregion

        #region Thread関数（非同期処理～結果の反映）

        /// <summary>Thread関数（Threadクラス用）</summary>
        public void CmnCallback()
        {
            try
            {
                // 前
                this.UOC_Pre();

                #region 処理の実行と結果の反映

                // 処理の実行
                this.ReturnValue = this.AsyncFunc(this.Parameter);

                // 結果の反映
                if (this.SetResult != null)
                {
                    // オブジェクトの型によって振り分け
                    if (this.thisWPF != null)
                    {
                        // 副スレッドなので、Dispatcher.Invokeを経由して主スレッドで呼び出す。
                        this.thisWPF.Dispatcher.Invoke(this.SetResult, this.ReturnValue);
                    }
                    else if (this.thisWinForm != null)
                    {
                        // 副スレッドなので、Control.Invokeを経由して主スレッドで呼び出す。
                        if (this.thisWinForm.IsDisposed)
                        {
                            // Closeされた
                            return;
                        }
                        else
                        {
                            // Closeされていない
                            this.thisWinForm.Invoke(this.SetResult, this.ReturnValue);
                        }
                    }
                }

                #endregion

                // 後
                this.UOC_After();
            }
            catch (Exception ex)
            {
                // 例外
                this.UOC_ABEND(ex);

                // 結果の反映
                if (this.SetResult != null)
                {
                    // オブジェクトの型によって振り分け
                    if (this.thisWPF != null)
                    {
                        // 副スレッドなので、Dispatcher.Invokeを経由して主スレッドで呼び出す。
                        this.thisWPF.Dispatcher.Invoke(this.SetResult, ex);
                    }
                    else if (this.thisWinForm != null)
                    {
                        // 副スレッドなので、Control.Invokeを経由して主スレッドで呼び出す。
                        if (this.thisWinForm.IsDisposed)
                        {
                            // Closeされた
                            return;
                        }
                        else
                        {
                            // Closeされていない
                            this.thisWinForm.Invoke(this.SetResult, ex);
                        }
                    }
                }
            }
            finally
            {
                // 終了
                this.UOC_Finally();
            }
        }

        /// <summary>Thread関数（ThreadPoolクラス用）</summary>
        /// <remarks>引数を渡すときは、Parameterメンバ変数を使用する。</remarks>
        public void CmnCallbackP(object state)
        {
            try
            {
                // 前
                this.UOC_Pre();

                #region 処理の実行と結果の反映

                // 処理の実行
                this.ReturnValue = this.AsyncFunc(this.Parameter);

                // 結果の反映
                if (this.SetResult != null)
                {
                    // オブジェクトの型によって振り分け
                    if (this.thisWPF != null)
                    {
                        // 副スレッドなので、Dispatcher.Invokeを経由して主スレッドで呼び出す。
                        this.thisWPF.Dispatcher.Invoke(this.SetResult, this.ReturnValue);
                    }
                    else if (this.thisWinForm != null)
                    {
                        // 副スレッドなので、Control.Invokeを経由して主スレッドで呼び出す。
                        if (this.thisWinForm.IsDisposed)
                        {
                            // Closeされた
                            return;
                        }
                        else
                        {
                            // Closeされていない
                            this.thisWinForm.Invoke(this.SetResult, this.ReturnValue);
                        }
                    }
                }

                #endregion

                // 後
                this.UOC_After();
            }
            catch (Exception ex)
            {
                // 例外
                this.UOC_ABEND(ex);

                // 結果の反映
                if (this.SetResult != null)
                {
                    // オブジェクトの型によって振り分け
                    if (this.thisWPF != null)
                    {
                        // 副スレッドなので、Dispatcher.Invokeを経由して主スレッドで呼び出す。
                        this.thisWPF.Dispatcher.Invoke(this.SetResult, ex);
                    }
                    else if (this.thisWinForm != null)
                    {
                        // 副スレッドなので、Control.Invokeを経由して主スレッドで呼び出す。
                        if (this.thisWinForm.IsDisposed)
                        {
                            // Closeされた
                            return;
                        }
                        else
                        {
                            // Closeされていない
                            this.thisWinForm.Invoke(this.SetResult, ex);
                        }
                    }
                }
            }
            finally
            {
                // 終了
                this.UOC_Finally();
            }
        }

        #endregion

        #region 非同期処理の進捗表示

        /// <summary>各画面を更新する（進捗表示）</summary>
        /// <param name="param">引数</param>
        public void ExecChangeProgress(object param)
        {
            // 引数の作成
            object[] args = new object[] { param };

            // 進捗の反映
            if (this.ChangeProgress != null)
            {
                // オブジェクトの型によって振り分け
                if (this.thisWPF != null)
                {
                    // 副スレッドなので、Dispatcher.Invokeを経由して主スレッドで呼び出す。
                    this.thisWPF.Dispatcher.Invoke(this.ChangeProgress, args);
                }
                else if (this.thisWinForm != null)
                {
                    // 副スレッドなので、Control.Invokeを経由して主スレッドで呼び出す。
                    if (this.thisWinForm.IsDisposed)
                    {
                        // Closeされた
                        return;
                    }
                    else
                    {
                        // Closeされていない
                        this.thisWinForm.Invoke(this.ChangeProgress, args);
                    }
                }
            }
        }

        #endregion

        #region 非同期メッセージボックス

        /// <summary>
        /// メッセージボックス表示のためのdelegate宣言(WPF)
        /// </summary>
        private delegate MessageBoxResult
            ShowAsyncMessageBoxDelegateWPF(
                string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);

        /// <summary>
        /// メッセージボックス表示のためのdelegate宣言(Win)
        /// </summary>
        private delegate DialogResult
            ShowAsyncMessageBoxDelegateWin(
                string messageBoxText, string caption, MessageBoxButtons button, MessageBoxIcon icon);

        /// <summary>
        /// 非同期処理からメッセージボックスを表示する（WPF）。
        /// </summary>
        /// <param name="messageBoxText">メッセージ テキスト</param>
        /// <param name="caption">Caption</param>
        /// <param name="button">表示するボタン</param>
        /// <param name="icon">表示するアイコン</param>
        /// <returns>MessageBoxResult</returns>
        /// <remarks>主スレッドで表示するメッセージボックスを呼び出す)</remarks>
        public MessageBoxResult ShowAsyncMessageBoxWPF(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            // 非同期処理からメッセージボックスを表示するメソッドのdelegate
            ShowAsyncMessageBoxDelegateWPF showAsyncMessageBoxDelegateWPF
                = new ShowAsyncMessageBoxDelegateWPF(this._ShowAsyncMessageBoxWPF);

            // 引数の作成
            object[] args = new object[] { messageBoxText, caption, button, icon };

            // オブジェクトの型によって振り分け
            if (this.thisWPF != null)
            {
                // 副スレッドなので、Dispatcher.Invokeを経由して主スレッドで呼び出す。
                return (MessageBoxResult)this.thisWPF.Dispatcher.Invoke(showAsyncMessageBoxDelegateWPF, args);
            }
            else
            {
                // 型の不一致
                throw new FrameworkException(
                    FrameworkExceptionMessage.ASYNC_MSGBOX_ERROR[0],
                    FrameworkExceptionMessage.ASYNC_MSGBOX_ERROR[1]);
            }
        }

        /// <summary>
        /// 非同期処理からメッセージボックスを表示する（Win）。
        /// </summary>
        /// <param name="messageBoxText">メッセージ テキスト</param>
        /// <param name="caption">Caption</param>
        /// <param name="button">表示するボタン</param>
        /// <param name="icon">表示するアイコン</param>
        /// <returns>MessageBoxResult</returns>
        /// <remarks>主スレッドで表示するメッセージボックスを呼び出す)</remarks>
        public DialogResult ShowAsyncMessageBoxWin(string messageBoxText, string caption, MessageBoxButtons button, MessageBoxIcon icon)
        {
            // 非同期処理からメッセージボックスを表示するメソッドのdelegate
            ShowAsyncMessageBoxDelegateWin showAsyncMessageBoxDelegateWPF
                = new ShowAsyncMessageBoxDelegateWin(this._ShowAsyncMessageBoxWin);

            // 引数の作成
            object[] args = new object[] { messageBoxText, caption, button, icon };

            // オブジェクトの型によって振り分け
            if (this.thisWinForm != null)
            {
                // 副スレッドなので、Dispatcher.Invokeを経由して主スレッドで呼び出す。
                if (this.thisWinForm.IsDisposed)
                {
                    // Closeされた
                    return DialogResult.None;
                }
                else
                {
                    // Closeされていない
                    return (DialogResult)this.thisWinForm.Invoke(showAsyncMessageBoxDelegateWPF, args);
                }
            }
            else
            {
                // 型の不一致
                throw new FrameworkException(
                    FrameworkExceptionMessage.ASYNC_MSGBOX_ERROR[0],
                    FrameworkExceptionMessage.ASYNC_MSGBOX_ERROR[1]);
            }
        }

        /// <summary>
        /// メッセージボックス表示のためのdelegate(WPF)
        /// </summary>
        /// <param name="messageBoxText">メッセージ テキスト</param>
        /// <param name="caption">Caption</param>
        /// <param name="button">表示するボタン</param>
        /// <param name="icon">表示するアイコン</param>
        /// <returns>MessageBoxResult</returns>
        /// <remarks>本メソッドは主スレッドで実行される</remarks>
        private MessageBoxResult _ShowAsyncMessageBoxWPF(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);
        }

        /// <summary>
        /// メッセージボックス表示のためのdelegate(WPF)
        /// </summary>
        /// <param name="messageBoxText">メッセージ テキスト</param>
        /// <param name="caption">Caption</param>
        /// <param name="button">表示するボタン</param>
        /// <param name="icon">表示するアイコン</param>
        /// <returns>DialogResult</returns>
        /// <remarks>本メソッドは主スレッドで実行される</remarks>
        private DialogResult _ShowAsyncMessageBoxWin(string messageBoxText, string caption, MessageBoxButtons button, MessageBoxIcon icon)
        {
            return System.Windows.Forms.MessageBox.Show(messageBoxText, caption, button, icon);
        }

        #endregion

        #region ウィンドウのロック・アンロック

        /// <summary>ウィンドウのロック</summary>
        /// <remarks>
        /// 通常、ベース２から使用する
        /// －－－－－－－－－－
        /// 呼び元はLockオブジェクトAを使用する
        /// クリティカルセクションAであること。
        /// </remarks>
        protected void WindowLock()
        {
            // ロック
            bool isEnabled = false;

            // Windows Forms
            if (this.thisWinForm != null)
            {
                if (this.thisWinForm.IsDisposed)
                {
                    // Closeされた
                    return;
                }
                else
                {
                    // Closeされていない

                    // BaseControllerWin？
                    if (this.thisWinForm is BaseControllerWin)
                    {
                        // BaseControllerWinの場合は専用メソッド
                        this.thisWinForm.Invoke(
                            new SetEnabledDelegate(
                                    ((BaseControllerWin)this.thisWinForm).SetEnabled), isEnabled);
                    }
                    else
                    {
                        // BaseControllerWinでない場合は通常メソッド

                        // ロック カウントの管理にはTagを利用する。
                        object tag = this.thisWinForm.Tag;

                        if (tag == null)
                        {
                            // 初期化
                            tag = 0;
                        }

                        int lockCount = 0;
                        if (int.TryParse(tag.ToString(), out lockCount))
                        {
                            // 未ロック（== 0）の場合のみロック
                            if (lockCount == 0)
                            {
                                this.thisWinForm.Invoke(
                                    new SetEnabledDelegate(this.SetEnabled), isEnabled);
                            }

                            // ロック カウントのインクリメント
                            lockCount++;
                            this.thisWinForm.Tag = lockCount;
                        }
                    }

                    return;
                }
            }

            // WPF 
            if (this.thisWPF != null)
            {
                // IsDisposedのチェックは不要
                // WPFではエラーにならないので。

                // WPFはクリティカルセクションにしてもTagを使用できない。
                // ※ 副スレッドからアクセスすると、例外になる。

                // 他の方法を検討しても、カウンタの保持場所が無いので、
                // WPFの多重ロック・アンロック対応は不可能と考える。

                this.thisWPF.Dispatcher.Invoke(
                    new SetEnabledDelegate(this.SetEnabled), isEnabled);

                return;
            }
        }

        /// <summary>ウィンドウのアンロック</summary>
        /// <remarks>
        /// 通常、ベース２から使用する
        /// －－－－－－－－－－
        /// 呼び元はLockオブジェクトAを使用する
        /// クリティカルセクションBであること。
        /// －－－－－－－－－－
        /// ここでは、Invokeを使用しない（デッドロックになる）。
        /// </remarks>
        protected void WindowUnlock()
        {
            // アンロック
            bool isEnabled = true;

            // Windows Forms
            if (this.thisWinForm != null)
            {
                if (this.thisWinForm.IsDisposed)
                {
                    // Closeされた
                    return;
                }
                else
                {
                    // Closeされていない

                    // BaseControllerWin？
                    if (this.thisWinForm is BaseControllerWin)
                    {
                        // BaseControllerWinの場合は専用メソッド
                        this.thisWinForm.BeginInvoke(
                            new SetEnabledDelegate(
                                    ((BaseControllerWin)this.thisWinForm).SetEnabled), isEnabled);
                    }
                    else
                    {
                        // BaseControllerWinでない場合は通常メソッド

                        // ロック カウントの管理にはTagを利用する。
                        object tag = this.thisWinForm.Tag;

                        if (tag == null)
                        {
                            // 初期化
                            tag = 0;
                        }

                        int lockCount = 0;
                        if (int.TryParse(tag.ToString(), out lockCount))
                        {
                            // ロック カウントのデクリメント
                            lockCount--;
                            this.thisWinForm.Tag = lockCount;

                            // 未ロック（== 0）の場合のみアンロック
                            if (lockCount == 0)
                            {
                                this.thisWinForm.BeginInvoke(
                                    new SetEnabledDelegate(this.SetEnabled), isEnabled);
                            }
                        }
                    }

                    return;
                }
            }

            // WPF 
            if (this.thisWPF != null)
            {
                // IsDisposedのチェックは不要
                // WPFではエラーにならないので。

                // WPFはクリティカルセクションにしてもTagを使用できない。
                // ※ 副スレッドからアクセスすると、例外になる。

                // 他の方法を検討しても、カウンタの保持場所が無いので、
                // WPFの多重ロック・アンロック対応は不可能と考える。

                this.thisWPF.Dispatcher.BeginInvoke(
                    new SetEnabledDelegate(this.SetEnabled), isEnabled);

                return;
            }
        }

        /// <summary>ウィンドウのロック・アンロック</summary>
        /// <param name="isEnabled">
        /// true：アンロック
        /// false：ロック
        /// </param>
        /// <remarks>主スレッドから実行される。</remarks>
        protected void SetEnabled(bool isEnabled)
        {
            if (this.thisWinForm != null)
            {
                this.thisWinForm.Enabled = isEnabled;
                return;
            }

            if (this.thisWPF != null)
            {
                // DependencyObjectだとIsEnabledプロパティがない。

                // なので、下（派生）の階層のUIElement
                // にキャスト可能かチェックし処理する。

                if (thisWPF is UIElement)
                {
                    (thisWPF as UIElement).IsEnabled = isEnabled;
                    return;
                }
            }
        }

        #endregion

        #region 仮想関数

        /// <summary>開始処理</summary>
        protected virtual void UOC_Pre() { }

        /// <summary>終了処理</summary>
        protected virtual void UOC_After() { }

        /// <summary>例外処理</summary>
        protected virtual void UOC_ABEND(Exception ex) { }

        /// <summary>最終処理</summary>
        protected virtual void UOC_Finally() { }

        #endregion
    }
}
