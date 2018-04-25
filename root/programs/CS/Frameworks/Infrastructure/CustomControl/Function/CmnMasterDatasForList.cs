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
//* クラス名        ：CmnMasterDatasForList
//* クラス日本語名  ：リスト用マスタデータ関連処理クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System.Collections;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;

namespace Touryo.Infrastructure.CustomControl
{
    /// <summary>リスト用マスタデータ関連処理クラス</summary>
    public class CmnMasterDatasForList
    {
        #region マスタデータ名の収集

        /// <summary>マスタデータ名の収集</summary>
        /// <param name="parentCtrl"></param>
        /// <param name="masterDataNames"></param>
        public static void GetMasterDataNames(Control parentCtrl, List<string> masterDataNames)
        {
            if (masterDataNames == null)
            { masterDataNames = new List<string>(); }

            // 対象のコントロールなら、
            if (parentCtrl is WebCustomDropDownList)// || WinCustomXXXX.etc)
            {
                // 新規か？
                bool isNew = true;

                // マスタデータ名を取得
                IMasterData im = (IMasterData)parentCtrl;

                foreach (string mdn in masterDataNames)
                {
                    if (mdn == im.MasterDataName)
                    {
                        // 一致 → 新規でない。
                        isNew = false;
                    }
                }

                // 新規か？
                if (isNew)
                {
                    // 新規の場合は追加する。
                    masterDataNames.Add(im.MasterDataName);
                }
            }

            // コントロールを再起検索する。
            foreach (Control childctrl in parentCtrl.Controls)
            {
                CmnMasterDatasForList.GetMasterDataNames(childctrl, masterDataNames);
            }
        }

        #endregion

        #region マスタデータの設定

        /// <summary>マスタデータの保管場所（Sessionプロパティ化）</summary>
        private static Dictionary<string, IEnumerable> MasterDatas
        {
            set
            {
                HttpContext.Current.Session["wcc_masterDatas"] = value;
            }
            get
            {
                // 初期化
                if (HttpContext.Current.Session["wcc_masterDatas"] == null)
                {
                    HttpContext.Current.Session["wcc_masterDatas"]
                        = new Dictionary<string, IEnumerable>();
                }

                return (Dictionary<string, IEnumerable>)
                    HttpContext.Current.Session["wcc_masterDatas"];
            }
        }

        /// <summary>マスタデータを設定</summary>
        public static void ClearMasterData()
        {
            CmnMasterDatasForList.MasterDatas 
                = new Dictionary<string, IEnumerable>();
        }

        /// <summary>マスタデータを設定</summary>
        /// <param name="name">マスタデータ名</param>
        /// <param name="obj">マスタデータ</param>
        public static void SetMasterData(string name, IEnumerable obj)
        {
            if (name == null) { name = ""; }
            name = name.Replace("　", "").Replace(" ", "").ToUpper();

            CmnMasterDatasForList.MasterDatas[name] = obj;
        }

        /// <summary>マスタデータを取得</summary>
        /// <param name="name">マスタデータ名</param>
        /// <returns>マスタデータ</returns>
        /// <remarks>データソースに指定する用</remarks>
        public static IEnumerable GetMasterData(string name)
        {
            if (name == null) { name = ""; }
            name = name.Replace("　", "").Replace(" ", "").ToUpper();

            if (CmnMasterDatasForList.MasterDatas.ContainsKey(name))
            {
                return CmnMasterDatasForList.MasterDatas[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>マスタデータを取得</summary>
        /// <param name="name">マスタデータ名</param>
        /// <param name="items">itemsプロパティ</param>
        /// <remarks>itemsプロパティに設定する用</remarks>
        public static void GetMasterData(string name, IList items)
        {
            if (name == null) { name = ""; }
            name = name.Replace("　", "").Replace(" ", "").ToUpper();

            if (CmnMasterDatasForList.MasterDatas.ContainsKey(name))
            {
                IEnumerable ie = CmnMasterDatasForList.MasterDatas[name];
                
                if (ie == null) return;

                foreach (object obj in ie)
                {
                    items.Add(obj);
                }
            }
        }

        /// <summary>マスタデータを削除</summary>
        /// <param name="name">マスタデータ名</param>
        public static void DeleteMasterData(string name)
        {
            if (name == null) { name = ""; }
            name = name.Replace("　", "").Replace(" ", "").ToUpper();

            if (CmnMasterDatasForList.MasterDatas.ContainsKey(name))
            {
                CmnMasterDatasForList.MasterDatas.Remove(name);
            }
        }

        #endregion
    }
}
