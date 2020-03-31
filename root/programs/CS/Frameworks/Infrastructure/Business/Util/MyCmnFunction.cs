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
//* クラス名        ：MyCmnFunction
//* クラス日本語名  ：Business層の共通クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#5  ： コントロール数取得処理（デフォルト値不正）
//*  2009/07/21  西野 大介         コントロール取得処理の仕様変更
//*  2009/08/10  西野 大介         同名のコントロール追加に対応（GridView/ItemTemplate）。
//*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/10/21  西野 大介         幾つかのイベント処理の正式対応（ベースクラス２→１へ）
//*  2012/06/14  西野 大介         コントロール検索の再帰処理性能の集約＆効率化。
//*  2014/05/16  西野 大介         キャスト可否チェックのロジックを見直した。
//*  2017/01/31  西野 大介         System.Webを使用しているCalculateSessionSizeメソッドをPublicから移動
//*  2018/03/29  西野 大介         .NET Standard対応で、削除機能に関連するメソッドを削除
//*  2018/03/29  西野 大介         .NET Standard対応で、HttpSessionのポーティング
//**********************************************************************************

using System;
using System.Collections.Generic;

using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;

#if NETCOREAPP
using Touryo.Infrastructure.Framework.StdMigration;
using Microsoft.AspNetCore.Http;
#else
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
#endif

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>Business層の共通クラス</summary>
    public class MyCmnFunction
    {
        #region CalculateSessionSize

        /// <summary>Sessionサイズ測定</summary>
        /// <returns>Sessionサイズ（MB）</returns>
        /// <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        public static long CalculateSessionSizeMB()
        {
            //return MyCmnFunction.CalculateSessionSizeKB() / 1000;
            return (long)Math.Round(MyCmnFunction.CalculateSessionSize() / 1000000.0, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>Sessionサイズ測定</summary>
        /// <returns>Sessionサイズ（KB）</returns>
        /// <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        public static long CalculateSessionSizeKB()
        {
            //return MyCmnFunction.CalculateSessionSize() / 1000;
            return (long)Math.Round(MyCmnFunction.CalculateSessionSize() / 1000.0, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>Sessionサイズ測定</summary>
        /// <returns>Sessionサイズ（バイト）</returns>
        /// <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        public static long CalculateSessionSize()
        {
            // ワーク変数
            long size = 0;

            // SessionのオブジェクトをBinarySerializeしてサイズを取得。
#if NETCOREAPP
            foreach (string key in MyHttpContext.Current.Session.Keys)
            {
                // 当該キーのオブジェクト・サイズを足しこむ。
                size += BinarySerialize.ObjectToBytes(MyHttpContext.Current.Session.GetString(key)).Length;
            }
#else
            foreach (string key in HttpContext.Current.Session.Keys)
            {
                // 当該キーのオブジェクト・サイズを足しこむ。
                size += BinarySerialize.ObjectToBytes(HttpContext.Current.Session[key]).Length;
            }
#endif
            // Sessionサイズ（バイト）
            return size;
        }

        #endregion

        // 2009/07/21-start

        #region コントロール取得＆イベントハンドラ設定

#if NETCOREAPP
#else
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
                            CheckBox checkBox = FxCmnFunction.CastByAsOperator<CheckBox>(ctrl, prefix);

                            // ハンドラをキャストして設定
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
                                CheckBox checkBox = FxCmnFunction.CastByAsOperator<CheckBox>(ctrl, prefix);

                                // ハンドラをキャストして設定
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
#endif
        #endregion

        // 2009/07/21-end
    }
}
