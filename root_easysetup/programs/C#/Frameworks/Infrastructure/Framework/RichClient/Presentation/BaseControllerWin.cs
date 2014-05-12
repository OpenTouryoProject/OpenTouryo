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
//* クラス名        ：BaseControllerWin
//* クラス日本語名  ：画面コード親クラス１（Windowアプリケーション）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/11/20  西野  大介        新規作成。以後、BaseControllerの実装にあわせエンハンス
//*  2010/11/21  西野  大介        集約イベント ハンドラをprotectedに変更（動的追加を考慮）
//*  2010/12/06  西野  大介        メソッドが無い場合、トレースしない仕様に変更
//*  2010/12/06  西野  大介        ウィンドウ数管理機能の追加
//*  2011/02/04  西野  大介        ウィンドウ インスタンス管理機能の追加
//*  2011/02/10  西野  大介        ユーザ コントロール上にイベントハンドラを実装可能にする。
//*  2011/02/24  西野  大介        ウィンドウ数管理（全Form型、当該Form型）と全Form型追加。
//*  2011/02/24  西野  大介        多重ロック・アンロック管理のカウンタ、メソッドを追加。
//*  2011/02/24  西野  大介        主スレッドでロック不要のため、ロックステートメントを削除
//*  2011/03/01  西野  大介        Formのキーイベント処理用のP層イベント処理の追加したが、
//*                                （キーイベントを処理するとログが膨大になるので採用見送り）
//*  2011/03/02  西野  大介        ThreadAbortExceptionのハンドルが冗長であったため削除。
//*  2011/03/03  西野  大介        FormLoadに対応するCosedの仮想関数イベントハンドラを準備。
//*  2011/03/08  西野  大介        ウィンドウ管理処理をコンストラクタからLoadイベントへ移動
//*  2012/06/14  西野  大介        コントロール検索の再帰処理性能の集約＆効率化。
//*  2012/06/18  西野  大介        OriginalStackTrace（ログ出力）の品質向上
//*  2012/09/19  西野  大介        UOC_CMNAfterFormInitの追加
//*  2012/09/19  西野  大介        イベント名不正を修正（EVENT_FORM_LOAD、EVENT_FORM_CLOSED）
//*  2013/03/05  西野  大介        UOC_CMNAfterFormInit、UOC_CMNAfterFormEndの呼出処理を追加
//*  2014/03/03  西野  大介        ユーザ コントロールのインスタンスの区別。
//**********************************************************************************

using System.Reflection;

// System
using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

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

using Touryo.Infrastructure.Framework.RichClient.Util;

namespace Touryo.Infrastructure.Framework.RichClient.Presentation
{
    /// <summary>画面コード親クラス１（Windowアプリケーション）</summary>
    /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
    public abstract class BaseControllerWin : Form
    {
        #region 静的変数

        ///// <summary>
        ///// 排他のためのクラス変数
        ///// </summary>
        //private static readonly object _lock = new object();

        #region ウィンドウ数

        /// <summary>ウィンドウ数の管理（全Form型）</summary>
        private static int _intWindowsCount = 0;

        /// <summary>全Form型のウィンドウ数を取得</summary>
        /// <returns>全Form型のウィンドウ数</returns>
        public static int GetWindowsCount()
        {
            // ただ返すだけ。
            return _intWindowsCount;
        }

        /// <summary>ウィンドウ数の管理（当該Form型）</summary>
        private static Dictionary<Type, int> _dicWindowsCount = new Dictionary<Type, int>();

        /// <summary>当該Form型ウィンドウ数を取得</summary>
        /// <param name="formType">当該Form型情報</param>
        /// <returns>当該Form型ウィンドウ数</returns>
        public static int GetWindowsCount(Type formType)
        {
            if (Latebind.CheckTypeOfBaseClass(formType, typeof(Form)))
            {
                // 正しい型

                //lock (_lock)
                //{

                if (BaseControllerWin._dicWindowsCount.ContainsKey(formType))
                {
                    // エントリあり
                    return BaseControllerWin._dicWindowsCount[formType];
                }
                else
                {
                    // エントリなし
                    return 0;
                }

                //}
            }
            else
            {
                // 不正な型
                return -1;
            }
        }

        #endregion

        #region ウィンドウ インスタンス

        /// <summary>ウィンドウ インスタンスの管理</summary>
        private static Dictionary<Type, List<Form>> _windowInstances = new Dictionary<Type, List<Form>>();

        /// <summary>ウィンドウ インスタンスを取得</summary>
        /// <param name="formType">型情報（form）</param>
        /// <returns>ウィンドウ インスタンス一覧</returns>
        public static List<Form> GetWindowInstances(Type formType)
        {
            if (Latebind.CheckTypeOfBaseClass(formType, typeof(Form)))
            {
                // 正しい型

                //lock (_lock)
                //{

                if (BaseControllerWin._windowInstances.ContainsKey(formType))
                {
                    // エントリあり
                    return BaseControllerWin._windowInstances[formType];
                }
                else
                {
                    // エントリなし（空のリスト）
                    return new List<Form>();
                }

                //}
            }
            else
            {
                // 不正な型（空のリスト）
                return new List<Form>();
            }
        }

        #endregion

        #endregion

        #region インスタンス変数

        #region P層イベント処理

        /// <summary>フレームワークのイベント処理対応コントロールを保持する</summary>
        /// <remarks>画面コード親クラス２から利用する。</remarks>
        protected Dictionary<string, Control> ControlHt = new Dictionary<string, Control>();

        /// <summary>全てのユーザ コントロールを保存するワーク領域</summary>
        protected List<UserControl> LstUserControl = null;

        /// <summary>オリジナルのスタックトレース値</summary>
        /// <remarks>画面コード親クラス２から利用する。</remarks>
        protected string OriginalStackTrace = "";

        #endregion

        #region 多重ロック・アンロック管理

        /// <summary>Formの多重ロック・アンロック管理</summary>
        private int _lockCount = 0;

        /// <summary>Formの多重ロック・アンロック管理</summary>
        /// <param name="isEnabled">
        /// true：アンロック
        /// false：ロック
        /// </param>
        public void SetEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                // Formのアンロック

                --this._lockCount;

                // カウンタが「0」の場合だけ、アンロックする。
                if (this._lockCount == 0)
                {
                    this.Enabled = isEnabled;
                }
            }
            else
            {
                // Formのロック
                ++this._lockCount;
                this.Enabled = isEnabled;
            }
        }

        #endregion

        #endregion

        #region コンストラクタ

        /// <summary>BaseControllerWinクラスのコンストラクタ</summary>
        public BaseControllerWin()
        {
            // Loadイベントへ移動
            InitializeComponent();
        }

        /// <summary>InitializeComponent</summary>
        /// <remarks>Page_Loadイベントハンドラを登録する。</remarks>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseControllerWin
            // 
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Name = "BaseControllerWin";

            this.Load += new EventHandler(this.Form_Load);
            this.FormClosed += new FormClosedEventHandler(this.Form_Closed);

            // 各種Formイベント
            //// 現状・未サポート（ログが膨大になってしまう）
            //this.KeyDown += new KeyEventHandler(this.Form_KeyDown);
            //this.KeyPress += new KeyPressEventHandler(this.Form_KeyPress);
            //this.KeyUp += new KeyEventHandler(this.Form_KeyUp);

            this.ResumeLayout(false);
        }

        #endregion

        #region イベント ハンドラ

        #region フォームロードのイベントハンドラ

        /// <summary>Form_Loadのイベントハンドラ</summary>
        private void Form_Load(object sender, EventArgs e)
        {
            try
            {
                #region ウィンドウ数・インスタンス管理

                //lock (_lock)
                //{

                // ウィンドウ数管理

                // 全Form型
                ++BaseControllerWin._intWindowsCount;

                // 当該Form型
                if (BaseControllerWin._dicWindowsCount.ContainsKey(this.GetType()))
                {
                    // 初回以降（インクリメント）
                    int i = BaseControllerWin._dicWindowsCount[this.GetType()];
                    BaseControllerWin._dicWindowsCount[this.GetType()] = ++i;
                }
                else
                {
                    // 初回（初期化）
                    BaseControllerWin._dicWindowsCount[this.GetType()] = 1;
                }

                // ・・・

                // ウィンドウ インスタンス管理
                if (BaseControllerWin._windowInstances.ContainsKey(this.GetType()))
                {
                    // 初回以降（インクリメント）
                    List<Form> list = BaseControllerWin._windowInstances[this.GetType()];

                    list.Add(this);
                    BaseControllerWin._windowInstances[this.GetType()] = list;
                }
                else
                {
                    // 初回（初期化）
                    List<Form> list = new List<Form>();

                    list.Add(this);
                    BaseControllerWin._windowInstances[this.GetType()] = list;
                }

                //}

                #endregion

                // ユーザ コントロールの初期化
                this.GetUserControl(this);

                #region コントロール取得処理

                #region 旧処理
                //// BUTTON
                //RcFxCmnFunction.GetCtrlAndSetClickEventHandler(
                //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON),
                //    new System.EventHandler(this.Button_Click), this.ControlHt);

                //// PICTURE BOX
                //RcFxCmnFunction.GetCtrlAndSetClickEventHandler(
                //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_PICTURE_BOX),
                //    new System.EventHandler(this.Button_Click), this.ControlHt);

                //// COMBO BOX
                //RcFxCmnFunction.GetCtrlAndSetClickEventHandler(
                //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_COMBO_BOX),
                //    new System.EventHandler(this.List_SelectedIndexChanged), this.ControlHt);

                //// LIST BOX
                //RcFxCmnFunction.GetCtrlAndSetClickEventHandler(
                //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX),
                //    new System.EventHandler(this.List_SelectedIndexChanged), this.ControlHt);

                //// RADIO BUTTON
                //RcFxCmnFunction.GetCtrlAndSetClickEventHandler(
                //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON),
                //    new System.EventHandler(this.Check_CheckedChanged), this.ControlHt);
                #endregion

                // プレフィックス
                string prefix = "";
                // プレフィックスとイベント ハンドラのディクショナリを生成
                Dictionary<string, object> prefixAndEvtHndHt = new Dictionary<string, object>();

                // BUTTON
                prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON);
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.Button_Click));
                }

                // PICTURE BOX
                prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_PICTURE_BOX);
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.Button_Click));
                }

                // COMBO BOX
                prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_COMBO_BOX);
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.List_SelectedIndexChanged));
                }

                // LIST BOX
                prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX);
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.List_SelectedIndexChanged));
                }

                // RADIO BUTTON
                prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON);
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.Check_CheckedChanged));
                }

                // コントロール検索＆イベントハンドラ設定
                RcFxCmnFunction.GetCtrlAndSetClickEventHandler2(this, prefixAndEvtHndHt, this.ControlHt);

                #endregion

                #region 画面の初期処理

                this.UOC_CMNFormInit();
                this.UOC_FormInit();
                this.UOC_CMNAfterFormInit();

                #endregion

            }
            catch (BusinessApplicationException baEx)
            {
                // アプリケーション例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(baEx, new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_LOAD, "", sender, e));

                // アプリケーション例外はリスローしない。
            }
            catch (BusinessSystemException bsEx)
            {
                // システム例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(bsEx, new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_LOAD, "", sender, e));

                // システム例外はリスローする。
                throw;
            }
            //catch (System.Threading.ThreadAbortException taEx)
            //{
            //    // スレッド中断エラーの場合は何もしない
            //    Exception ex = taEx; // ← 警告を出さないため
            //}
            catch (Exception ex)
            {
                // 一般的な例外発生時の処理は派生クラスに記述する
                this.UOC_ABEND(ex, new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_LOAD, "", sender, e));

                // 一般的な例外はリスローする。
                throw;
            }
            finally
            {
                // Finally節のUOCメソッド 
                this.UOC_Finally(new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_LOAD, "", sender, e));
            }
        }

        #endregion

        #region フォームクローズドのイベントハンドラ

        /// <summary>Form_Closedのイベントハンドラ</summary>
        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            //lock (_lock)
            //{
            //if (BaseControllerWin._windowsCount.ContainsKey(this.GetType()))
            //{
            // 絶対あるのでチェック不要。
            //}
            //}

            try
            {
                #region 画面の終了処理

                this.UOC_CMNFormEnd();
                this.UOC_FormEnd();
                this.UOC_CMNAfterFormEnd();

                #endregion
            }
            catch (BusinessApplicationException baEx)
            {
                // アプリケーション例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(baEx, new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_CLOSED, "", sender, e));

                // アプリケーション例外はリスローしない。
            }
            catch (BusinessSystemException bsEx)
            {
                // システム例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(bsEx, new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_CLOSED, "", sender, e));

                // システム例外はリスローする。
                throw;
            }
            //catch (System.Threading.ThreadAbortException taEx)
            //{
            //    // スレッド中断エラーの場合は何もしない
            //    Exception ex = taEx; // ← 警告を出さないため
            //}
            catch (Exception ex)
            {
                // 一般的な例外発生時の処理は派生クラスに記述する
                this.UOC_ABEND(ex, new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_CLOSED, "", sender, e));

                // 一般的な例外はリスローする。
                throw;
            }
            finally
            {
                // Finally節のUOCメソッド 
                this.UOC_Finally(new RcFxEventArgs(
                    FxLiteral.EVENT_FORM_CLOSED, "", sender, e));

                // ウィンドウ数管理

                // 全Form型
                --BaseControllerWin._intWindowsCount;

                // 当該Form型
                int i = BaseControllerWin._dicWindowsCount[this.GetType()];
                BaseControllerWin._dicWindowsCount[this.GetType()] = --i;

                // ウィンドウ インスタンス管理
                List<Form> list = BaseControllerWin._windowInstances[this.GetType()];
                list.Remove(this);
            }
        }

        #endregion

        # region   集約イベント ハンドラ

        #region 各種イベントに対応した集約イベント ハンドラ

        /// <summary>
        /// ボタン系のClickイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Button_Click(object sender, EventArgs e)
        {
            string name = ((Control)(sender)).Name;

            // イベント ハンドラの共通引数の作成
            RcFxEventArgs rcFxEventArgs
                = new RcFxEventArgs(name,
                    this.GetMethodName(((Control)(sender)),
                    FxLiteral.UOC_METHOD_FOOTER_CLICK), sender, e);

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(rcFxEventArgs);
        }

        /// <summary>
        /// リスト系のSelectedIndexChangedイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = ((Control)(sender)).Name;

            // イベント ハンドラの共通引数の作成
            RcFxEventArgs rcFxEventArgs
                = new RcFxEventArgs(name,
                    this.GetMethodName(((Control)(sender)),
                    FxLiteral.UOC_METHOD_FOOTER_SELECTED_INDEX_CHANGED), sender, e);

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(rcFxEventArgs);
        }

        /// <summary>
        /// ボタン系のCheckedChangedイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Check_CheckedChanged(object sender, EventArgs e)
        {
            string name = ((Control)(sender)).Name;

            // イベント ハンドラの共通引数の作成
            RcFxEventArgs rcFxEventArgs
                = new RcFxEventArgs(name,
                    this.GetMethodName(((Control)(sender)),
                    FxLiteral.UOC_METHOD_FOOTER_CHECKED_CHANGED), sender, e);

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(rcFxEventArgs);
        }

        #region Form

        /// <summary>
        /// FormのKeyDownイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Form_KeyDown(object sender, KeyEventArgs e)
        {
            string name = ((Control)(sender)).Name;

            // イベント ハンドラの共通引数の作成
            RcFxEventArgs rcFxEventArgs
                = new RcFxEventArgs(name + "_KeyDown",
                    this.GetMethodName(((Control)(sender)),
                    FxLiteral.UOC_METHOD_FOOTER_KEY_DOWN), sender, e);

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(rcFxEventArgs);
        }

        /// <summary>
        /// FormのKeyPressイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            string name = ((Control)(sender)).Name;

            // イベント ハンドラの共通引数の作成
            RcFxEventArgs rcFxEventArgs
                = new RcFxEventArgs(name + "_KeyPress",
                    this.GetMethodName(((Control)(sender)),
                    FxLiteral.UOC_METHOD_FOOTER_KEY_PRESS), sender, e);

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(rcFxEventArgs);
        }

        /// <summary>
        /// FormのKeyUpイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Form_KeyUp(object sender, KeyEventArgs e)
        {
            string name = ((Control)(sender)).Name;

            // イベント ハンドラの共通引数の作成
            RcFxEventArgs rcFxEventArgs
                = new RcFxEventArgs(name + "_KeyUp",
                    this.GetMethodName(((Control)(sender)),
                    FxLiteral.UOC_METHOD_FOOTER_KEY_UP), sender, e);

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(rcFxEventArgs);
        }

        #endregion

        #endregion

        /// <summary>ユーザ コントロール名を記憶しておくワーク</summary>
        private string UserControlImplementingMethod = "";

        /// <summary>レイトバインドする際に使用するメソッド名を生成する</summary>
        /// <param name="sender">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <returns>レイトバインドする際に使用するメソッド名</returns>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected string GetMethodName(Control sender, string eventName)
        {
            string controlID = sender.Name;

            //// 以下のメソッド名でレイトバインド
            //return FxLiteral.UOC_METHOD_HEADER + ControlID + "_" + EventName;

            if (this.GetUCControl(sender, out this.UserControlImplementingMethod) == null)
            {
                // ユーザ コントロール上でない場合

                // 以下のメソッド名でレイトバインド
                return FxLiteral.UOC_METHOD_HEADER + controlID + "_" + eventName;
            }
            else
            {
                // ユーザ コントロール上の場合

                // 以下のメソッド名でレイトバインド
                // ユーザ コントロールのヘッダ有り
                return FxLiteral.UOC_METHOD_HEADER
                    + this.UserControlImplementingMethod
                    + "_" + controlID + "_" + eventName;
            }
        }

        #region イベント処理の共通メソッド

        /// <summary>イベント処理の共通メソッド</summary>
        /// <param name="rcFxEventArgs">イベント ハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected void CMN_Event_Handler(RcFxEventArgs rcFxEventArgs)
        {
            // UOCメソッドの戻り値、urlを受ける。
            //string url = "";

            // オリジナルのスタックトレース値のクリア
            this.OriginalStackTrace = "";

            #region メソッドが無ければ、何もしないコードを追加。

            // Formをチェック
            if (!Latebind.CheckTypeOfMethodByName(this, rcFxEventArgs.MethodName))
            {
                // Formで発見できなかった

                bool isExist = false;

                // UserControlをチェック
                string newMethodName = rcFxEventArgs.MethodName.Replace(this.UserControlImplementingMethod + "_", "");

                foreach (UserControl uc in this.LstUserControl)
                {
                    if (Latebind.CheckTypeOfMethodByName(uc, newMethodName))
                    {
                        // UserControlで発見できた。
                        isExist = true;
                        break;
                    }
                }

                // 発見できなかったら、何もせずに戻る。
                if (!isExist)
                {
                    return;
                }
            }

            #endregion

            try
            {
                // ★ イベントの開始前のUOC処理
                this.UOC_PreAction(rcFxEventArgs);

                // ★ イベントのUOC処理
                this.LateBind(rcFxEventArgs);

                // ★ イベントの終了後のUOC処理
                this.UOC_AfterAction(rcFxEventArgs);
            }
            catch (BusinessApplicationException baEx)
            {
                // アプリケーション例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(baEx, rcFxEventArgs);

                // アプリケーション例外はリスローしない。
            }
            catch (BusinessSystemException bsEx)
            {
                // システム例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(bsEx, rcFxEventArgs);

                // システム例外はリスローする。
                throw;
            }
            //catch (System.Threading.ThreadAbortException taEx)
            //{
            //    // スレッド中断エラーの場合は何もしない
            //    Exception ex = taEx; // ← 警告を出さないため
            //}
            catch (Exception ex)
            {
                // 一般的な例外発生時の処理は派生クラスに記述する
                this.UOC_ABEND(ex, rcFxEventArgs);

                // 一般的な例外はリスローする。
                throw;
            }
            finally
            {
                // Finally節のUOCメソッド 
                this.UOC_Finally(rcFxEventArgs);
            }
        }

        #endregion

        #region レイトバインドするメソッド

        /// <summary>レイトバインドするメソッド</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>画面遷移する場合のURL</returns>
        private void LateBind(RcFxEventArgs rcFxEventArgs)
        {
            // 引数を格納するオブジェクト配列
            object[] parameter = new object[] { rcFxEventArgs };

            try
            {
                // Latebind部品の追加により上記コードから変更（2009/04/15）
                if (Latebind.CheckTypeOfMethodByName(this, rcFxEventArgs.MethodName))
                {
                    // 本画面中にメソッドがある。
                    Latebind.InvokeMethod_NoErr(this, rcFxEventArgs.MethodName, parameter);
                }
                else
                {
                    // 本画面中にメソッドがない。

                    // ユーザ コントロールの可能性
                    foreach (UserControl uc in this.LstUserControl)
                    {
                        // 比較してイコールであること。
                        if (uc.Name == this.UserControlImplementingMethod)
                        {
                            // メソッドを実装するユーザ コントロールの参照を取得できた場合、

                            // メソッド名からユーザ コントロール名のプレフィックスを削除し、
                            string newMethodName = rcFxEventArgs.MethodName.
                                Replace(this.UserControlImplementingMethod + "_", "");

                            // マスタ ページに対してレイトバインド。
                            Latebind.InvokeMethod_NoErr(uc, newMethodName, parameter);

                            // ・・・fxEventArgs.MethodNameと一致しないが、こういう仕様ということで。
                        }
                    }
                }
            }
            catch (System.Reflection.TargetInvocationException rtEx)
            {
                //InnerExceptionのスタックトレースを保存しておく（以下のリスローで消去されるため）。
                this.OriginalStackTrace = rtEx.InnerException.StackTrace;

                // InnerExceptionを投げなおす。
                throw rtEx.InnerException;
            }

            //return url;
        }

        #endregion

        #endregion

        #endregion

        #region ユーティリティ メソッド

        #region コントロール取得メソッド

        /// <summary>Fxでハンドルしているコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected Control GetFxFormControl(string controlName)
        {
            // Fxでハンドルしているコントロールの参照を取得する。

            // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
            if (this.ControlHt.ContainsKey(controlName))
            {
                return (Control)this.ControlHt[controlName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>コントロールを取得する</summary>
        /// <param name="ctrl">検索を開始するルートのコントロール</param>
        /// <param name="ctrlName">コントロール名</param>
        /// <returns>コントロール</returns>
        private Control FindControl(Control ctrl, string ctrlName)
        {
            if (ctrl.Name == ctrlName)
            {
                // 一致した。
                return ctrl;
            }
            else
            {
                // 一致しなかった。

                // 子が・・・
                if (ctrl.HasChildren)
                {
                    // ある場合、再起検索する。
                    foreach (Control child in ctrl.Controls)
                    {
                        // 再起
                        Control ret = this.FindControl(child, ctrlName);

                        // 有り
                        if (ret != null)
                        {
                            return ret;
                        }
                    }
                }
                else
                {
                    // ない場合、何もしない。
                }

                // 何もない場合は、nullをリターンする。
                return null;
            }
        }

        /// <summary>コントロールを取得する</summary>
        /// <param name="ctrl">検索を開始するルートのコントロール</param>
        /// <param name="ctrlName">コントロール名</param>
        /// <returns>コントロール</returns>
        /// <remarks>（ネストした）ユーザ コントロール以下を検索しない。</remarks>
        private Control FindUCControl(Control ctrl, string ctrlName)
        {
            if (ctrl.Name == ctrlName)
            {
                // 一致した。
                return ctrl;
            }
            else
            {
                // 一致しなかった。

                // 子が・・・
                if (ctrl.HasChildren)
                {
                    // ある場合、再起検索する。
                    foreach (Control child in ctrl.Controls)
                    {
                        // 子が・・・
                        if (child is UserControl)
                        {
                            // ユーザコントロールの場合、検索しない。
                        }
                        else
                        {
                            // ユーザコントロールの場合、再起検索する。
                            Control ret = this.FindControl(child, ctrlName);

                            // 有り
                            if (ret != null)
                            {
                                return ret;
                            }
                        }
                    }
                }
                else
                {
                    // ない場合、何もしない。
                }

                // 何もない場合は、nullをリターンする。
                return null;
            }
        }

        #region ユーザ コントロールの情報を初期化する

        /// <summary>ユーザ コントロールの情報を初期化する</summary>
        /// <param name="ctrl">コントロール</param>
        private void GetUserControl(Control ctrl)
        {
            // 必要なら初期化する。
            if (this.LstUserControl == null)
            {
                this.LstUserControl = new List<UserControl>();
            }

            // ユーザ コントロールならば、
            if (ctrl is UserControl)
            {
                // ユーザ コントロールのリストに追加
                this.LstUserControl.Add((UserControl)ctrl);
            }

            // 子があれば
            if (ctrl.HasChildren)
            {
                // 再起検索する。
                foreach (Control child in ctrl.Controls)
                {
                    this.GetUserControl(child);
                }
            }
            else
            {
                // 何もない場合は、リターンする。
                return;
            }
        }

        #endregion

        #region ユーザ コントロール上のコントロールを取得するメソッド

        /// <summary>ユーザ コントロール上のコントロールを取得するメソッド</summary>
        /// <param name="sender">取得したいコントロール</param>
        /// <param name="userControlName">ユーザ コントロール名</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>ユーザコントロールのネストを考慮する。</remarks>
        private Control GetUCControl(Control sender, out string userControlName)
        {
            string controlName = sender.Name;

            // 検索
            foreach (UserControl uc in this.LstUserControl)
            {
                Control ctrl = this.FindUCControl(uc, controlName);

                if (ctrl == null)
                {
                    // この[UserControl]には無かった。
                }
                else
                {
                    // この[UserControl]に在った。

                    // ユーザ コントロール名
                    userControlName = uc.Name;

                    // イベントの発生源を特定。
                    Control temp = sender;
                    while (true)
                    {
                        if (temp.Parent == null)
                        {
                            // ルート
                            break;
                        }
                        else
                        {
                            // Parentの確認。
                            if (temp.Parent.Name == userControlName)
                            {
                                // この[UserControl]である。
                                return ctrl;
                            }
                            else
                            {
                                // この[UserControl]ではない。
                            }

                            // 遡る。
                            temp = temp.Parent;
                        }
                    }
                }
            }

            // すべての[UserControl]に無かった。
            userControlName = "";
            return null;
        }

        #endregion

        #endregion

        #endregion

        #region ＵＯＣメソッド

        #region ページ ロード イベント内のＵＯＣメソッド

        /// <summary>画面開始に対応した共通UOCメソッド</summary>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected abstract void UOC_CMNFormInit();

        /// <summary>画面開始に対応した個別UOCメソッド</summary>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected abstract void UOC_FormInit();

        /// <summary>画面開始に対応した共通UOCメソッド</summary>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected abstract void UOC_CMNAfterFormInit();

        /// <summary>画面終了に対応した共通UOCメソッド</summary>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected abstract void UOC_CMNFormEnd();

        /// <summary>画面終了に対応した共通UOCメソッド</summary>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected abstract void UOC_CMNAfterFormEnd();

        /// <summary>画面終了に対応した個別UOCメソッド</summary>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected abstract void UOC_FormEnd();

        #endregion

        #region イベント処理の開始前、終了後処理のＵＯＣメソッド

        /// <summary>イベントの開始前の処理を実装</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_PreAction(RcFxEventArgs rcFxEventArgs) { }

        /// <summary>イベントの終了後の処理を実装</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_AfterAction(RcFxEventArgs rcFxEventArgs) { }

        /// <summary>Finally節の処理を実装</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_Finally(RcFxEventArgs rcFxEventArgs) { }

        #endregion

        #region エラー処理のＵＯＣメソッド

        /// <summary>BusinessApplicationExceptionの例外処理用のUOCメソッド</summary>
        /// <param name="baEx">BusinessApplicationException</param>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(BusinessApplicationException baEx, RcFxEventArgs rcFxEventArgs) { }

        /// <summary>BusinessSystemExceptionの例外処理用のUOCメソッド</summary>
        /// <param name="bsEx">BusinessSystemException</param>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(BusinessSystemException bsEx, RcFxEventArgs rcFxEventArgs) { }

        /// <summary>Exceptionの例外処理用のUOCメソッド</summary>
        /// <param name="ex">Exception</param>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(Exception ex, RcFxEventArgs rcFxEventArgs) { }

        #endregion

        #endregion
    }
}
