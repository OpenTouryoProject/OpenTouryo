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
//* クラス名        ：FxCmnFunction
//* クラス日本語名  ：Framework層の共通クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/03/13  西野 大介         新規作成
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#5  ： コントロール数取得処理（デフォルト値不正）
//*                                ・#33 ： プロパティの大文字 / 小文字をなくす。
//*  2009/07/21  西野 大介         コントロール取得処理の仕様変更
//*  2009/07/31  西野 大介         セッション情報の自動削除機能を追加
//*  2009/08/10  西野 大介         同名のコントロール追加に対応（GridView/ItemTemplate）。
//*  2009/09/18  西野 大介         セッション タイム アウト検出用cookieのパス属性を明示
//*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/10/21  西野 大介         幾つかのイベント処理の正式対応（ベースクラス２→１へ）
//*  2010/10/21  西野 大介         RepeaterコントロールのItemCommandイベント追加
//*  2010/11/20  西野 大介         リッチクライアント用P層フレームワークを追加
//*  2011/01/14  西野 大介         GetPropsFromPropStringをPubCmnFunctionに移動
//*  2011/01/18  西野 大介         GridViewコントロールのRowCommand、SelectedIndexChanged、
//*                                RowUpdating、RowDeleting、PageIndexChanging、Sortingイベントを追加する。
//*  2012/06/14  西野 大介         コントロール検索の再帰処理性能の集約＆効率化。
//*  2013/03/05  西野 大介         cookieのパス属性のApplicationPathが「/」になるケースの考慮
//*  2014/05/16  西野 大介         キャスト可否チェックのロジックを見直した。
//*  2014/08/18  Sai-San           Added code for adding events dynamically for ListView events. 
//*  2014/10/03  Rituparna         Added code for Supporting ItemCommand event to ListViewControl. 
//*  2014/10/03  Rituparna         Added code SelectedIndexChanged for RadiobuttonList and CheckBoxList.
//*  2014/11/19  Sandeep           Removed Redundant Code "FxCmnFunction.AddControlToDic" in method GetCtrlAndSetClickEventHandler
//*  2014/04/16  Supragyan         Added TextChanged event to TextBox control in method GetCtrlAndSetClickEventHandler.
//**********************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>Framework層の共通クラス</summary>
    public class FxCmnFunction
    {
        #region 数定義（コンフィグ）の取得処理

        /// <summary>数定義（コンフィグ）の取得処理</summary>
        /// <param name="configKey">コンフィグのキー</param>
        /// <param name="defaultNum">デフォルト値</param>
        /// <returns>数</returns>
        public static int GetNumFromConfig(string configKey, int defaultNum)
        {
            int temp;

            // null チェック
            if (GetConfigParameter.GetConfigValue(configKey) == null)
            {
                // デフォルト値
                return defaultNum;
            }
            else
            {
                // int チェック
                if (int.TryParse(GetConfigParameter.GetConfigValue(configKey), out temp))
                {
                    // 変換完了
                    return temp;
                }
                else
                {
                    // 変換ミス
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_NUMVAL[0],
                        String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_NUMVAL[1], configKey));
                }
            }
        }

        #endregion

        #region コントロール取得＆イベントハンドラ設定

        /// <summary>コントロール取得＆イベントハンドラ設定（下位互換）</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="prefix">プレフィックス</param>
        /// <param name="eventHandler">イベント ハンドラ</param>
        /// <param name="controlHt">コントロールのディクショナリ</param>
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
                        if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON))
                        {
                            // BUTTON
                            Button button = FxCmnFunction.CastByAsOperator<Button>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            button.Click += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LINK_BUTTON))
                        {
                            // LINK BUTTON
                            LinkButton linkButton = FxCmnFunction.CastByAsOperator<LinkButton>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            linkButton.Click += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_BUTTON))
                        {
                            // IMAGE BUTTON
                            ImageButton imageButton = FxCmnFunction.CastByAsOperator<ImageButton>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            imageButton.Click += (ImageClickEventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_MAP))
                        {
                            // IMAGE MAP
                            ImageMap imageMap = FxCmnFunction.CastByAsOperator<ImageMap>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            imageMap.Click += (ImageMapEventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        //else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_COMMAND))
                        //{
                        //    // COMMAND
                        //    Command command = FxCmnFunction.CastByAsOperator<Command>(ctrl, prefix);

                        //    // ハンドラをキャストして設定
                        //    command.Click += (EventHandler)eventHandler;

                        //    // ディクショナリに格納
                        //    // ControlHt.Add(ctrl.ID, ctrl);
                        //    // ControlHt[ctrl.ID] = ctrl;
                        //    FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        //}
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_DROP_DOWN_LIST))
                        {
                            // DROP DOWN LIST
                            DropDownList dropDownList = FxCmnFunction.CastByAsOperator<DropDownList>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            dropDownList.SelectedIndexChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX))
                        {
                            // LIST BOX
                            ListBox listBox = FxCmnFunction.CastByAsOperator<ListBox>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            listBox.SelectedIndexChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON))
                        {
                            // RADIO BUTTON
                            RadioButton radioButton = FxCmnFunction.CastByAsOperator<RadioButton>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            radioButton.CheckedChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIOBUTTONLIST))
                        {
                            // RADIO BUTTON
                            RadioButtonList radioButtonlist = FxCmnFunction.CastByAsOperator<RadioButtonList>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            radioButtonlist.SelectedIndexChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt);
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_CHECKBOXLIST))
                        {
                            // CHECKBOXLIST
                            CheckBoxList checkboxlist = FxCmnFunction.CastByAsOperator<CheckBoxList>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            checkboxlist.SelectedIndexChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            controlHt[ctrl.ID] = ctrl;                            
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_REPEATER))
                        {
                            // REPEATER
                            Repeater repeater = FxCmnFunction.CastByAsOperator<Repeater>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            repeater.ItemCommand += (RepeaterCommandEventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_GRIDVIEW))
                        {
                            // GRIDVIEW
                            GridView gridView = FxCmnFunction.CastByAsOperator<GridView>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            object[] eventHandlers = ((object[])eventHandler);

                            // 全コマンド
                            gridView.RowCommand += (GridViewCommandEventHandler)eventHandlers[0];
                            // 選択
                            gridView.SelectedIndexChanged += (EventHandler)eventHandlers[1];
                            // 編集（更新）
                            gridView.RowUpdating += (GridViewUpdateEventHandler)eventHandlers[2];
                            // 編集（削除）
                            gridView.RowDeleting += (GridViewDeleteEventHandler)eventHandlers[3];
                            // ページング
                            gridView.PageIndexChanging += (GridViewPageEventHandler)eventHandlers[4];
                            // ソーティング
                            gridView.Sorting += (GridViewSortEventHandler)eventHandlers[5];

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); // 2011/02/12
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LISTVIEW))
                        {
                            // LISTVIEW
                            ListView listView = FxCmnFunction.CastByAsOperator<ListView>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            object[] eventHandlers = ((object[])eventHandler);

                            //Delete event handler
                            listView.ItemDeleting += (EventHandler<ListViewDeleteEventArgs>)eventHandlers[0];
                            //Update event handler
                            listView.ItemUpdating += (EventHandler<ListViewUpdateEventArgs>)eventHandlers[1];
                            // Paging event handler
                            listView.PagePropertiesChanged += (EventHandler)eventHandlers[2];
                            //Sorting event handler
                            listView.Sorting += (EventHandler<ListViewSortEventArgs>)eventHandlers[3];
                            //itemcommand event handler                           
                            listView.ItemCommand += (EventHandler<ListViewCommandEventArgs>)eventHandlers[5];

                            // ディクショナリに格納
                            FxCmnFunction.AddControlToDic(ctrl, controlHt);
                        }
                        else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_TEXTBOX))
                        {
                            // Text Box
                            System.Web.UI.WebControls.TextBox textBox = FxCmnFunction.CastByAsOperator<System.Web.UI.WebControls.TextBox>(ctrl, prefix);

                            // ハンドラをキャストして設定
                            textBox.TextChanged += (EventHandler)eventHandler;

                            // ディクショナリに格納
                            // ControlHt.Add(ctrl.ID, ctrl);
                            // ControlHt[ctrl.ID] = ctrl;
                            FxCmnFunction.AddControlToDic(ctrl, controlHt); 
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
                    FxCmnFunction.GetCtrlAndSetClickEventHandler(childCtrl, prefix, eventHandler, controlHt);
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
                            if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON))
                            {
                                // BUTTON
                                Button button = FxCmnFunction.CastByAsOperator<Button>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                button.Click += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LINK_BUTTON))
                            {
                                // LINK BUTTON
                                LinkButton linkButton = FxCmnFunction.CastByAsOperator<LinkButton>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                linkButton.Click += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_BUTTON))
                            {
                                // IMAGE BUTTON
                                ImageButton imageButton = FxCmnFunction.CastByAsOperator<ImageButton>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                imageButton.Click += (ImageClickEventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_MAP))
                            {
                                // IMAGE MAP
                                ImageMap imageMap = FxCmnFunction.CastByAsOperator<ImageMap>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                imageMap.Click += (ImageMapEventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            //else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_COMMAND))
                            //{
                            //    // COMMAND
                            //    Command command = FxCmnFunction.CastByAsOperator<Command>(ctrl, prefix);

                            //    // ハンドラをキャストして設定
                            //    command.Click += (EventHandler)eventHandler;

                            //    // ディクショナリに格納
                            //    controlHt[ctrl.ID] = ctrl;
                            //    break;
                            //}
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_DROP_DOWN_LIST))
                            {
                                // DROP DOWN LIST
                                DropDownList dropDownList = FxCmnFunction.CastByAsOperator<DropDownList>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                dropDownList.SelectedIndexChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX))
                            {
                                // LIST BOX
                                ListBox listBox = FxCmnFunction.CastByAsOperator<ListBox>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                listBox.SelectedIndexChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON))
                            {
                                // RADIO BUTTON
                                RadioButton radioButton = FxCmnFunction.CastByAsOperator<RadioButton>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                radioButton.CheckedChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }

                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIOBUTTONLIST))
                            {
                                // RADIOBUTTONLIST
                                RadioButtonList radioButtonlist = FxCmnFunction.CastByAsOperator<RadioButtonList>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                radioButtonlist.SelectedIndexChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }

                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_CHECKBOXLIST))
                            {
                                // CHECKBOXLIST
                                CheckBoxList checkboxlist = FxCmnFunction.CastByAsOperator<CheckBoxList>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                checkboxlist.SelectedIndexChanged += (EventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_REPEATER))
                            {
                                // REPEATER
                                Repeater repeater = FxCmnFunction.CastByAsOperator<Repeater>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                repeater.ItemCommand += (RepeaterCommandEventHandler)eventHandler;

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_GRIDVIEW))
                            {
                                // GRIDVIEW
                                GridView gridView = FxCmnFunction.CastByAsOperator<GridView>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                object[] eventHandlers = ((object[])eventHandler);

                                // 全コマンド
                                gridView.RowCommand += (GridViewCommandEventHandler)eventHandlers[0];
                                // 選択
                                gridView.SelectedIndexChanged += (EventHandler)eventHandlers[1];
                                // 編集（更新）
                                gridView.RowUpdating += (GridViewUpdateEventHandler)eventHandlers[2];
                                // 編集（削除）
                                gridView.RowDeleting += (GridViewDeleteEventHandler)eventHandlers[3];
                                // ページング
                                gridView.PageIndexChanging += (GridViewPageEventHandler)eventHandlers[4];
                                // ソーティング
                                gridView.Sorting += (GridViewSortEventHandler)eventHandlers[5];

                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                break;
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LISTVIEW))
                            {
                                // LISTVIEW
                                ListView listView = FxCmnFunction.CastByAsOperator<ListView>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                object[] eventHandlers = ((object[])eventHandler);

                                // Delete
                                listView.ItemDeleting += (EventHandler<ListViewDeleteEventArgs>)eventHandlers[0];
                                //Update
                                listView.ItemUpdating += (EventHandler<ListViewUpdateEventArgs>)eventHandlers[1];
                                // Paging
                                listView.PagePropertiesChanged += (EventHandler)eventHandlers[2];
                                //Sorting event handler
                                listView.Sorting += (EventHandler<ListViewSortEventArgs>)eventHandlers[3];
                                //ItemCommand event handler
                                listView.ItemCommand += (EventHandler<ListViewCommandEventArgs>)eventHandlers[5];
                                FxCmnFunction.AddControlToDic(ctrl, controlHt);
                                // ディクショナリに格納
                                controlHt[ctrl.ID] = ctrl;
                                FxCmnFunction.AddControlToDic(ctrl, controlHt);
                            }
                            else if (prefix == GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_TEXTBOX))
                            {
                                // Text box
                                System.Web.UI.WebControls.TextBox textBox = FxCmnFunction.CastByAsOperator<System.Web.UI.WebControls.TextBox>(ctrl, prefix);

                                // ハンドラをキャストして設定
                                textBox.TextChanged += (EventHandler)eventHandler;

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
                    FxCmnFunction.GetCtrlAndSetClickEventHandler2(childCtrl, prefixAndEvtHndHt, controlHt);
                }
            }

            #endregion
        }


        /// <summary>キャスト可否チェック</summary>
        /// <typeparam name="TResult">キャストする型</typeparam>
        /// <param name="target">Control</param>
        /// <param name="prefix">プレフィックス</param>
        /// <returns>キャスト結果</returns>
        public static TResult CastByAsOperator<TResult>(Control target, string prefix) where TResult : Control
        {
            // Cast By As Operator
            TResult result = target as TResult;

            if (result == null)
            {
                // キャストできない場合
                throw new FrameworkException(
                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR[0],
                    String.Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR[1],
                    prefix, target.GetType().ToString()));
            }
            else
            {
                // キャストできた場合
                return result;
            }
        }

        #region 旧処理
        /// <summary>コントロールの追加処理（下位互換）</summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="ControlDic">コントロールのディクショナリ</param>
        public static void AddControlToDic(Control ctrl, Dictionary<string, Control> ControlDic)
        {
            ControlDic[ctrl.ID] = ctrl;

            // ↓RepeaterのRepeaterItemにユーザ コントロールが入ってる場合など、やはり考慮できない。

            // フレームワークの仕様としては、コントロール名から、バインドするだけ。
            // コントロール名により、イベントハンドラ名が決まるので、重複したコントロール名
            // により、予期せぬイベントハンドラが呼び出されることがあるので注意が必要である。

            //if (ControlDic.ContainsKey(ctrl.ID))
            //{
            //    // 登録済みのコントロール

            //    // BindingContainerの・・・
            //    Control bindingContainer = ctrl.BindingContainer;

            //    if (bindingContainer != null)
            //    {
            //        // BindingContainerのBindingContainerが
            //        bindingContainer = bindingContainer.BindingContainer;

            //        if (bindingContainer != null)
            //        {
            //            // 以下の一覧系のコントロール内にある場合
            //            if (bindingContainer is Repeater ||
            //            bindingContainer is DataGrid ||
            //            bindingContainer is GridView ||
            //            bindingContainer.GetType().ToString().IndexOf("ListView") != -1) // ListViewは3.5でサポート
            //            {
            //                // 繰り返しコントロール内にある場合は、上書きする。
            //                ControlDic[ctrl.ID] = ctrl;
            //                return;
            //            }
            //        }
            //    }

            //    // 重複エラー
            //    FxCmnFunction.ThowRepetitionError(ctrl);
            //}
            //else
            //{
            //    // 未登録のコントロール
            //    ControlDic.Add(ctrl.ID, ctrl);
            //    return;
            //}
        }

        ///// <summary>コントロール重複例外を（下位互換）</summary>
        ///// <param name="ctrl">コントロール</param>
        //private static void ThowRepetitionError(Control ctrl)
        //{
        //    throw new FrameworkException(
        //                FrameworkExceptionMessage.CONTROL_REPETITION_ERROR1[0],
        //                String.Format(FrameworkExceptionMessage.CONTROL_REPETITION_ERROR1[1], ctrl.ID));
        //}
        #endregion

        #endregion

        #region LRU的にキューを再構築

        /// <summary>GUIDをキーにして、LRU的にキューを再構築する。</summary>
        /// <param name="currentQueue">再構築するキュー</param>
        /// <param name="guid">キーにするGUIDの文字列</param>
        /// <param name="maxLength">キューの要素の最大値</param>
        /// <returns>再構築したキュー</returns>
        /// <remarks>要素が、そのままGUIDの場合</remarks>
        internal static Queue<string> RestructuringLRUQueue1(
            Queue<string> currentQueue, string guid, int maxLength)
        {
            // オーバーロード
            return FxCmnFunction.RestructuringLRUQueue1(currentQueue, guid, guid, maxLength);
        }

        /// <summary>GUIDをキーにして、LRU的にキューを再構築する（更新機能付き）。</summary>
        /// <param name="currentQueue">再構築するキュー</param>
        /// <param name="oldGuid">キーにするGUIDの文字列（検索用）</param>
        /// <param name="newGuid">キーにするGUIDの文字列（更新用）</param>
        /// <param name="maxLength">キューの要素の最大値</param>
        /// <returns>再構築したキュー</returns>
        /// <remarks>要素が、そのままGUIDの場合</remarks>
        internal static Queue<string> RestructuringLRUQueue1(
            Queue<string> currentQueue, string oldGuid, string newGuid, int maxLength)
        {
            //新しいキューの作成 
            Queue<string> tempQueue = new Queue<string>(maxLength);

            // 詰替ワーク
            string tempGuid = null;

            // 詰替発生フラグ
            bool flg = false;

            // 新しいキューへ詰替
            while (0 < currentQueue.Count)
            {
                // 元からデキュー
                tempGuid = (string)currentQueue.Dequeue();

                // GUIDを確認
                if (tempGuid == oldGuid)
                {
                    // oldGuid（更新時：newGuid）は、
                    // 先頭に移動させるため、後にエンキューする。
                    flg = true;
                }
                else
                {
                    // 詰め替え先にエンキュー
                    tempQueue.Enqueue(tempGuid);
                }
            }

            if (flg)
            {
                // 検索し、一致するGUIDを発見できた。

                // 検索したoldGuidを、先頭に移動。
                // （更新時は、newGuidを先頭に移動）
                tempQueue.Enqueue(newGuid);
            }
            else
            {
                // 検索し、一致するGUIDを発見できなかった。

                // 指定のguidは既に消失したものである場合。
                // ・ 自動削除機能の場合 → 無視する。
                // ・ 不正操作防止機能の場合 → 有り得ない。
            }

            // 再構築したキューを返す。
            return tempQueue;
        }

        /// <summary>GUIDをキーにして、LRU的にキューを再構築する。</summary>
        /// <param name="currentQueue">再構築するキュー</param>
        /// <param name="guid">キーにするGUIDの文字列</param>
        /// <param name="maxLength">キューの要素の最大値</param>
        /// <returns>再構築したキュー</returns>
        /// <remarks>要素がArrayListでArrayList[0]がGUIDの場合</remarks>
        internal static Queue<ArrayList> RestructuringLRUQueue2(
            Queue<ArrayList> currentQueue, string guid, int maxLength)
        {
            //新しいキューの作成 
            Queue<ArrayList> tempQueue = new Queue<ArrayList>(maxLength);

            // そのまま詰替える要素
            ArrayList tempArrayList = null;
            // 先頭に移動する要素
            ArrayList tempArrayListThis = null;

            // 新しいキューへ詰替
            while (0 < currentQueue.Count)
            {
                // 元からデキュー
                tempArrayList = (ArrayList)currentQueue.Dequeue();

                // GUIDを確認
                if (tempArrayList[0].ToString() == guid)
                {
                    // これ（tempArrayListThis）は、
                    // 先頭に移動させるため、後にエンキューする。
                    tempArrayListThis = tempArrayList;
                }
                else
                {
                    // 詰め替え先にエンキュー
                    tempQueue.Enqueue(tempArrayList);
                }
            }

            // これ（tempArrayListThis）を、先頭に移動
            tempQueue.Enqueue(tempArrayListThis);

            // 再構築したキューを返す。
            return tempQueue;
        }

        #endregion

        #region セッションタイムアウト検出用クッキー

        /// <summary>セッションタイムアウト検出用Cookieを生成</summary>
        /// <returns>セッションタイムアウト検出用Cookie（データ有）</returns>
        /// <remarks>
        /// 戻りのHttpCookieをResponse.Cookies.Setメソッド
        /// に指定し、Set-Cookie HTTPヘッダをレスポンスする。
        /// </remarks>
        public static HttpCookie CreateCookieForSessionTimeoutDetection()
        {
            // Cookie生成（デバッグしやすいように都度、値を変更する）
            HttpCookie newCookie
                = new HttpCookie(FxHttpCookieIndex.SESSION_TIMEOUT, Environment.TickCount.ToString());

            // Path属性を設定
            if (HttpContext.Current.Request.ApplicationPath == "/")
            {
                // 「//」になってしまうので、「/」になるよう修正
                newCookie.Path = "/";
            }
            else
            {
                // 例えば「/ProjectX_sample」+「/」＝「/ProjectX_sample/」となる。
                newCookie.Path = HttpContext.Current.Request.ApplicationPath + "/";
            }

            // ※ Request.ApplicationPathは、URLのホスト名以上のパス情報を含まず、
            // アプリケーション・ディレクトリより下のパス情報を含まない。

            // HttpOnly属性を設定
            newCookie.HttpOnly = true;

            // セッションタイムアウト検出用Cookie（データ有）
            return newCookie;
        }

        /// <summary>セッションタイムアウト検出用Cookieを削除</summary>
        /// <returns>セッションタイムアウト検出用Cookie（データ空）</returns>
        /// <remarks>
        /// 戻りのHttpCookieをResponse.Cookies.Setメソッド
        /// に指定し、Set-Cookie HTTPヘッダをレスポンスする。
        /// </remarks>
        public static HttpCookie DeleteCookieForSessionTimeoutDetection()
        {
            // Cookie生成（削除時は、空の値を指定）
            HttpCookie newCookie
                = new HttpCookie(FxHttpCookieIndex.SESSION_TIMEOUT, "");

            // Path属性を設定
            if (HttpContext.Current.Request.ApplicationPath == "/")
            {
                // 「//」になってしまうので、「/」になるよう修正
                newCookie.Path = "/";
            }
            else
            {
                // 例えば「/ProjectX_sample」+「/」＝「/ProjectX_sample/」となる。
                newCookie.Path = HttpContext.Current.Request.ApplicationPath + "/";
            }

            // ※ Request.ApplicationPathは、URLのホスト名以上のパス情報を含まず、
            // アプリケーション・ディレクトリより下のパス情報を含まない。

            // HttpOnly属性を設定
            newCookie.HttpOnly = true;

            // セッションタイムアウト検出用Cookie（データ空）
            return newCookie;
        }

        #endregion
    }
}
