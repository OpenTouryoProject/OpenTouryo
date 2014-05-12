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
//* クラス名        ：RcFxCmnFunction
//* クラス日本語名  ：FrameWork.RichClient層の共通クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/11/20  西野  大介        新規作成
//*  2012/06/14  西野  大介        コントロール検索の再帰処理性能の集約＆効率化。
//**********************************************************************************

using System.Diagnostics;

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

// WPFアプリケーション
using System.Windows;

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

namespace Touryo.Infrastructure.Framework.RichClient.Util
{
    /// <summary>FrameWork.RichClient層の共通クラス</summary>
    public class RcFxCmnFunction
    {
        /// <summary>コントロール取得＆イベントハンドラ設定（下位互換）</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="prefix">プレフィックス</param>
        /// <param name="eventHandler">イベント ハンドラ</param>
        /// <param name="controlHt">コントロールのディクショナリ</param>
        internal static void GetCtrlAndSetClickEventHandler(Control ctrl, string prefix, object eventHandler, Dictionary<string, Control> controlHt)
        {
            #region チェック処理

            // コントロール指定が無い場合
            if (ctrl == null)
            {
                // 何もしないで戻る。
                return;
            }

            // プレフィックス指定が無い場合
            if (prefix == null || prefix == "")
            {
                // 何もしないで戻る。
                return;
            }

            #endregion

            #region コントロール取得＆イベントハンドラ設定

            // コントロールのNameチェック
            if (ctrl.Name == null)
            {
                // コントロールName無し
            }
            else
            {
                // コントロールName有り

                // コントロールのName長確認
                if (prefix.Length <= ctrl.Name.Length)
                {
                    // 指定のプレフィックス
                    if (prefix == ctrl.Name.Substring(0, prefix.Length))
                    {
                        // イベントハンドラを設定する。
                        if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON))
                        {
                            // BUTTON
                            Button button = null;

                            try
                            {
                                // キャストできる
                                button = (Button)ctrl;
                            }
                            catch (Exception ex)
                            {
                                // キャストできない
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()), ex);
                            }

                            button.Click += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.Name, ctrl);
                            controlHt[ctrl.Name] = ctrl; // 2009/08/10-この行
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_PICTURE_BOX))
                        {
                            // PICTURE BOX
                            PictureBox pictureBox = null;

                            try
                            {
                                // キャストできる
                                pictureBox = (PictureBox)ctrl;
                            }
                            catch (Exception ex)
                            {
                                // キャストできない
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()), ex);
                            }

                            pictureBox.Click += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.Name, ctrl);
                            controlHt[ctrl.Name] = ctrl; // 2009/08/10-この行
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_COMBO_BOX))
                        {
                            // COMBO BOX
                            ComboBox comboBox = null;

                            try
                            {
                                // キャストできる
                                comboBox = (ComboBox)ctrl;
                            }
                            catch (Exception ex)
                            {
                                // キャストできない
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()), ex);
                            }

                            comboBox.SelectedIndexChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.Name, ctrl);
                            controlHt[ctrl.Name] = ctrl; // 2009/08/10-この行
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX))
                        {
                            // LIST BOX
                            ListBox listBox = null;

                            try
                            {
                                // キャストできる
                                listBox = (ListBox)ctrl;
                            }
                            catch (Exception ex)
                            {
                                // キャストできない
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()), ex);
                            }

                            listBox.SelectedIndexChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.Name, ctrl);
                            controlHt[ctrl.Name] = ctrl; // 2009/08/10-この行
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON))
                        {
                            // RADIO BUTTON
                            RadioButton radioButton = null;

                            try
                            {
                                // キャストできる
                                radioButton = (RadioButton)ctrl;
                            }
                            catch (Exception ex)
                            {
                                // キャストできない
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()), ex);
                            }

                            radioButton.CheckedChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.Name, ctrl);
                            controlHt[ctrl.Name] = ctrl; // 2009/08/10-この行
                        }
                    }
                }
            }

            #endregion

            #region 再起

            // 子コントロールがある場合、
            if (ctrl.Controls.Count != 0)
            {
                // 子コントロール毎に
                foreach (Control childCtrl in ctrl.Controls)
                {
                    // 再起する。
                    RcFxCmnFunction.GetCtrlAndSetClickEventHandler(childCtrl, prefix, eventHandler, controlHt);
                }
            }

            #endregion
        }

        /// <summary>コントロール取得＆イベントハンドラ設定</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="prefixAndEvtHndHt">プレフィックスとイベント ハンドラのディクショナリ</param>
        /// <param name="controlHt">コントロールのディクショナリ</param>
        internal static void GetCtrlAndSetClickEventHandler2(Control ctrl, Dictionary<string, object> prefixAndEvtHndHt, Dictionary<string, Control> controlHt)
        {
            // ループ
            foreach (string prefix in prefixAndEvtHndHt.Keys)
            {
                object eventHandler = prefixAndEvtHndHt[prefix];

                #region チェック処理

                // コントロール指定が無い場合
                if (ctrl == null)
                {
                    // 何もしないで戻る。
                    return;
                }

                // プレフィックス指定が無い場合
                if (prefix == null || prefix == "")
                {
                    // 何もしないで戻る。
                    return;
                }

                #endregion

                #region コントロール取得＆イベントハンドラ設定

                // コントロールのNameチェック
                if (ctrl.Name == null)
                {
                    // コントロールName無し
                }
                else
                {
                    // コントロールName有り

                    // コントロールのName長確認
                    if (prefix.Length <= ctrl.Name.Length)
                    {
                        // 指定のプレフィックス
                        if (prefix == ctrl.Name.Substring(0, prefix.Length))
                        {
                            // イベントハンドラを設定する。
                            if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON))
                            {
                                // BUTTON
                                Button button = null;

                                if (ctrl is Button)
                                {
                                    // キャストできる
                                    button = (Button)ctrl;
                                }
                                else
                                {
                                    // キャストできない
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()));
                                }

                                button.Click += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.Name] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_PICTURE_BOX))
                            {
                                // PICTURE BOX
                                PictureBox pictureBox = null;

                                if (ctrl is PictureBox)
                                {
                                    // キャストできる
                                    pictureBox = (PictureBox)ctrl;
                                }
                                else
                                {
                                    // キャストできない
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()));
                                }

                                pictureBox.Click += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.Name] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_COMBO_BOX))
                            {
                                // COMBO BOX
                                ComboBox comboBox = null;

                                if (ctrl is ComboBox)
                                {
                                    // キャストできる
                                    comboBox = (ComboBox)ctrl;
                                }
                                else
                                {
                                    // キャストできない
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()));
                                }

                                comboBox.SelectedIndexChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.Name] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX))
                            {
                                // LIST BOX
                                ListBox listBox = null;

                                if (ctrl is ListBox)
                                {
                                    // キャストできる
                                    listBox = (ListBox)ctrl;
                                }
                                else
                                {
                                    // キャストできない
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()));
                                }

                                listBox.SelectedIndexChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.Name] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON))
                            {
                                // RADIO BUTTON
                                RadioButton radioButton = null;

                                if (ctrl is RadioButton)
                                {
                                    // キャストできる
                                    radioButton = (RadioButton)ctrl;
                                }
                                else
                                {
                                    // キャストできない
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1], prefix, ctrl.GetType().ToString()));
                                }

                                radioButton.CheckedChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.Name] = ctrl;
                                break;
                            }
                        }
                    }
                }

                #endregion
            }

            #region 再起

            // 子コントロールがある場合、
            if (ctrl.Controls.Count != 0)
            {
                // 子コントロール毎に
                foreach (Control childCtrl in ctrl.Controls)
                {
                    // 再起する。
                    RcFxCmnFunction.GetCtrlAndSetClickEventHandler2(childCtrl, prefixAndEvtHndHt, controlHt);
                }
            }

            #endregion
        }

        /// <summary>
        /// デザイン・モードであるかチェックする。
        /// </summary>
        /// <returns>
        /// true:デザインモード時
        /// false:実行時時
        /// </returns>
        /// <remarks>
        /// DesignModeプロパティというデザイン・モードであるかチェックするものが用意されていますが、バグがあるため使用禁止です。
        /// </remarks>
        public static bool IsDesignMode()
        {
            // 開発環境のプロセス名を全て列挙する（VC++にもC++/CLIがあるので）。
            if (Process.GetCurrentProcess().ProcessName.ToLower() == "devenv"
                || Process.GetCurrentProcess().ProcessName.ToLower() == "vcexpress" 
                || Process.GetCurrentProcess().ProcessName.ToLower() == "vcsexpress"
                || Process.GetCurrentProcess().ProcessName.ToLower() == "vbexpress"
                || Process.GetCurrentProcess().ProcessName.ToLower() == "vwdexpress")
            {
                // デザインモード時
                return true;
            }
            else
            {
                // 実行時時
                return false;
            }
        }
        
    }
}
