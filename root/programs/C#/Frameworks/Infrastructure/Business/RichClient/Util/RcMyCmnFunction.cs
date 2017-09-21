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
//* クラス名        ：RcMyCmnFunction
//* クラス日本語名  ：Business.RichClient層の共通クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/11/21  西野 大介         新規作成
//*  2012/06/14  西野 大介         コントロール検索の再帰処理性能の集約＆効率化。
//*  2014/05/16  西野 大介         キャスト可否チェックのロジックを見直した。
//*  2017/09/12  西野 大介         UserControlの動的配置対応のためアクセス修飾子を変更。
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.RichClient.Util;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.RichClient.Util
{
    /// <summary>Business.RichClient層の共通クラス</summary>
    public class RcMyCmnFunction
    {
        /// <summary>コントロール取得＆イベントハンドラ設定</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="prefix">プレフィックス</param>
        /// <param name="eventHandler">イベント ハンドラ</param>
        /// <param name="ControlHt">ディクショナリ</param>
        public static void GetCtrlAndSetClickEventHandler(Control ctrl, string prefix, object eventHandler, Dictionary<string, Control> ControlHt)
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
                        if (prefix == GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX))
                        {
                            // CHECK BOX
                            CheckBox checkBox = RcFxCmnFunction.CastByAsOperator<CheckBox>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            checkBox.CheckedChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.Name, ctrl);
                            ControlHt[ctrl.Name] = ctrl; // 2009/08/10-この行
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
                    RcMyCmnFunction.GetCtrlAndSetClickEventHandler(childCtrl, prefix, eventHandler, ControlHt);
                }
            }

            #endregion
        }

        /// <summary>コントロール取得＆イベントハンドラ設定</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="prefixAndEvtHndHt">プレフィックスとイベント ハンドラのディクショナリ</param>
        /// <param name="controlHt">コントロールのディクショナリ</param>
        public static void GetCtrlAndSetClickEventHandler2(Control ctrl, Dictionary<string, object> prefixAndEvtHndHt, Dictionary<string, Control> controlHt)
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
                            if (prefix == GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX))
                            {
                                // CHECK BOX
                                CheckBox checkBox = RcFxCmnFunction.CastByAsOperator<CheckBox>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                checkBox.CheckedChanged += (EventHandler)eventHandler;

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
                    RcMyCmnFunction.GetCtrlAndSetClickEventHandler2(childCtrl, prefixAndEvtHndHt, controlHt);
                }
            }

            #endregion
        }

        /// <summary>
        /// ユーザー・フレンドリなダイアログを表示するメソッド
        /// </summary>
        public static void ShowErrorMessageWin(Exception ex, string extraMessage)
        {
            System.Windows.Forms.MessageBox.Show(
                extraMessage + 
                " \r\n――――――――\r\n\r\n" +
                "エラーが発生しました。開発元にお知らせください\r\n\r\n" +
                "ex.Message : \r\n" + ex.Message + "\r\n\r\n" +
                "ex.ToString() : \r\n" + ex.ToString());
        }

        /// <summary>
        /// ユーザー・フレンドリなダイアログを表示するメソッド
        /// </summary>
        public static void ShowErrorMessageWPF(Exception ex, string extraMessage)
        {
            System.Windows.MessageBox.Show(
                extraMessage + 
                " \r\n――――――――\r\n\r\n" +
                "エラーが発生しました。開発元にお知らせください\r\n\r\n" +
                "ex.Message : \r\n" + ex.Message + "\r\n\r\n" +
                "ex.ToString() : \r\n" + ex.ToString());
        }
    }
}
