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
//* クラス名       : FxSessionUtil
//* クラス日本語名 : ダイアログの親画面と子画面との間で受け渡すデータを管理するクラス
//* 
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2009/03/13  西野 大介         修正（汎用化）
//**********************************************************************************

using System.Collections;
using System.Web;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>ダイアログの親画面と子画面との間で受け渡すデータを管理するクラス</summary>
    internal class FxSessionUtil
    {
        #region internal

        /// <summary>フレームワークで管理されたHttpSessionにオブジェクトを追加する</summary>
        /// <param name="fxSessionType">タイプ識別タグ</param>
        /// <param name="guid">各種GUID</param>        
        /// <param name="name">キー名</param>
        /// <param name="value">値</param>
        internal static void Add(string fxSessionType, string guid, string name, object value)
        {
            // フレームワークで管理されたHttpSessionから、Listを取得
            SortedList businessDataList =
                (SortedList)HttpContext.Current.Session[fxSessionType + guid];

            if (businessDataList == null)
            {
                // ListがNUllの場合、オブジェクトを生成。
                businessDataList = new SortedList();
            }
            else
            {
                // Listがあり、かつ、既に同一のキー名が存在する場合は、一旦削除。
                if (businessDataList.ContainsKey(name))
                {
                    // キーが有る
                    businessDataList.Remove(name);
                }
            }

            // Listにオブジェクトを追加
            businessDataList.Add(name, value);

            // フレームワークで管理されたHttpSessionにListを設定
            HttpContext.Current.Session[fxSessionType + guid] = businessDataList;
        }

        /// <summary>フレームワークで管理されたHttpSessionからオブジェクトを取得する。</summary>
        /// <param name="fxSessionType">タイプ識別タグ</param>
        /// <param name="guid">各種GUID</param>    
        /// <param name="name">キー名</param>
        /// <returns>値（対象のオブジェクト）</returns>
        internal static object Item(string fxSessionType, string guid, string name)
        {
            // フレームワークで管理されたHttpSessionから、Listを取得
            SortedList businessDataList =
                (SortedList)HttpContext.Current.Session[fxSessionType + guid];

            if (businessDataList == null)
            {
                // Listが見つからなかった場合、Nullを返す。
                return null;
            }
            else
            {
                // Listが見つかった場合、引数で渡された名前を持つ値を返す。
                return businessDataList[name];
            }
        }

        /// <summary>フレームワークで管理されたHttpSessionからオブジェクトを削除する。</summary>
        /// <param name="fxSessionType">タイプ識別タグ</param>
        /// <param name="guid">各種GUID</param>    
        internal static void Remove(string fxSessionType, string guid)
        {
            // フレームワークで管理されたHttpSessionから、Listを削除
            HttpContext.Current.Session[fxSessionType + guid] = null;
        }

        /// <summary>フレームワークで管理されたHttpSessionからオブジェクトを削除する。</summary>
        /// <param name="fxSessionType">タイプ識別タグ</param>
        /// <param name="guid">各種GUID</param>    
        /// <param name="name">キー名</param>
        internal static void Remove(string fxSessionType, string guid, string name)
        {
            // フレームワークで管理されたHttpSessionから、Listを取得
            SortedList businessDataList =
                (SortedList)HttpContext.Current.Session[fxSessionType + guid];
            
            if (businessDataList != null)
            {
                // Listが見つかった場合、引数で渡された名前を持つ値を削除
                businessDataList.Remove(name);

                // フレームワークで管理されたHttpSessionにListを設定
                HttpContext.Current.Session[fxSessionType + guid] = businessDataList;
            }
        }

        #endregion
    }
}
