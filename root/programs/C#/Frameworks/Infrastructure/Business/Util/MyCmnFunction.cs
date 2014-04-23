//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
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

using System.Text;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

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

            if (ctrl == null)
            {
                return;
            }

            if (prefix == null || prefix == "")
            {
                return;
            }

            #endregion

            #region コントロール取得＆イベントハンドラ設定

            if (ctrl.ID == null)
            {
            }
            else
            {
                if (prefix.Length <= ctrl.ID.Length)
                {
                    if (prefix == ctrl.ID.Substring(0, prefix.Length))
                    {
                        if (prefix == GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX))
                        {
                            CheckBox checkBox = null;

                            try
                            {
                                checkBox = (CheckBox)ctrl;
                            }
                            catch (Exception ex)
                            {
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1],
                                    prefix, ctrl.GetType().ToString()), ex);
                            }

                            checkBox.CheckedChanged += (EventHandler)eventHandler;

                            FxCmnFunction.AddControlToDic(ctrl, controlHt);
                        }
                    }
                }
            }

            #endregion

            #region 再帰

            if (ctrl.HasControls())
            {
                foreach (Control childCtrl in ctrl.Controls)
                {
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
            foreach (string prefix in prefixAndEvtHndHt.Keys)
            {
                object eventHandler = prefixAndEvtHndHt[prefix];

                #region チェック処理

                if (ctrl == null)
                {
                    return;
                }

                if (prefix == null || prefix == "")
                {
                    return;
                }

                #endregion

                #region コントロール取得＆イベントハンドラ設定

                if (ctrl.ID == null)
                {
                }
                else
                {
                    if (prefix.Length <= ctrl.ID.Length)
                    {
                        if (prefix == ctrl.ID.Substring(0, prefix.Length))
                        {
                            if (prefix == GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX))
                            {
                                CheckBox checkBox = null;

                                if(ctrl is CheckBox)
                                {
                                    checkBox = (CheckBox)ctrl;
                                }
                                else
                                {
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1],
                                        prefix, ctrl.GetType().ToString()));
                                }

                                checkBox.CheckedChanged += (EventHandler)eventHandler;

                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                        }
                    }
                }

                #endregion
            }

            #region 再帰

            if (ctrl.HasControls())
            {
                foreach (Control childCtrl in ctrl.Controls)
                {
                    MyCmnFunction.GetCtrlAndSetClickEventHandler2(childCtrl, prefixAndEvtHndHt, controlHt);
                }
            }

            #endregion
        }

        #endregion
    }
}
