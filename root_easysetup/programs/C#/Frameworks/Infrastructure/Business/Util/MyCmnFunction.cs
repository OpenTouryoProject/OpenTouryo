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
//* クラス名        ：MyCmnFunction
//* クラス日本語名  ：Business層の共通クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#5  ： コントロール数取得処理（デフォルト値不正）
//*  2009/07/21  西野  大介        コントロール取得処理の仕様変更
//*  2009/08/10  西野  大介        同名のコントロール追加に対応（GridView/ItemTemplate）。
//*  2010/09/24  西野  大介        ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/10/21  西野  大介        幾つかのイベント処理の正式対応（ベースクラス２→１へ）
//*  2012/06/14  西野  大介        コントロール検索の再帰処理性能の集約＆効率化。
//**********************************************************************************

using System.Text;

// System
using System;
using System.Collections;
using System.Collections.Generic;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

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

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>Business層の共通クラス</summary>
    public class MyCmnFunction
    {
        // 2009/07/21-start

        #region コントロール取得＆イベントハンドラ設定

        /// <summary>コントロール取得＆イベントハンドラ設定（下位互換）</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="prefix">プレフィックス</param>
        /// <param name="eventHandler">イベント ハンドラ</param>
        /// <param name="controlHt">ディクショナリ</param>
        internal static void GetCtrlAndSetClickEventHandler(
            Control ctrl, string prefix, object eventHandler, Dictionary<string, Control> controlHt)
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

            // コントロールのIDチェック
            if (ctrl.ID == null)
            {
                // コントロールID無し
            }
            else
            {
                // コントロールID有り

                // コントロールのID長確認
                if (prefix.Length <= ctrl.ID.Length)
                {
                    // 指定のプレフィックス
                    if (prefix == ctrl.ID.Substring(0, prefix.Length))
                    {
                        // イベントハンドラを設定する。
                        if (prefix == GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX))
                        {
                            // CHECK BOX
                            CheckBox checkBox = null;

                            try
                            {
                                // キャストできる
                                checkBox = (CheckBox)ctrl;
                            }
                            catch (Exception ex)
                            {
                                // キャストできない
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1],
                                    prefix, ctrl.GetType().ToString()), ex);
                            }

                            checkBox.CheckedChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                    }
                }
            }

            #endregion

            #region 再帰

            // 子コントロールがある場合、
            if (ctrl.HasControls())
            {
                // 子コントロール毎に
                foreach (Control childCtrl in ctrl.Controls)
                {
                    // 再帰する。
                    MyCmnFunction.GetCtrlAndSetClickEventHandler(childCtrl, prefix, eventHandler, controlHt);
                }
            }

            #endregion
        }

        /// <summary>コントロール取得＆イベントハンドラ設定</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="prefixAndEvtHndHt">プレフィックスとイベント ハンドラのディクショナリ</param>
        /// <param name="controlHt">コントロールのディクショナリ</param>
        internal static void GetCtrlAndSetClickEventHandler2(
            Control ctrl, Dictionary<string, object> prefixAndEvtHndHt, Dictionary<string, Control> controlHt)
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

                // コントロールのIDチェック
                if (ctrl.ID == null)
                {
                    // コントロールID無し
                }
                else
                {
                    // コントロールID有り

                    // コントロールのID長確認
                    if (prefix.Length <= ctrl.ID.Length)
                    {
                        // 指定のプレフィックス
                        if (prefix == ctrl.ID.Substring(0, prefix.Length))
                        {
                            // イベントハンドラを設定する。
                            if (prefix == GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX))
                            {
                                // CHECK BOX
                                CheckBox checkBox = null;

                                if(ctrl is CheckBox)
                                {
                                    // キャストできる
                                    checkBox = (CheckBox)ctrl;
                                }
                                else
                                {
                                    // キャストできない
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1],
                                        prefix, ctrl.GetType().ToString()));
                                }

                                checkBox.CheckedChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                        }
                    }
                }

                #endregion
            }

            #region 再帰

            // 子コントロールがある場合、
            if (ctrl.HasControls())
            {
                // 子コントロール毎に
                foreach (Control childCtrl in ctrl.Controls)
                {
                    // 再帰する。
                    MyCmnFunction.GetCtrlAndSetClickEventHandler2(childCtrl, prefixAndEvtHndHt, controlHt);
                }
            }

            #endregion
        }

        #endregion

        // 2009/07/21-end
    }
}
