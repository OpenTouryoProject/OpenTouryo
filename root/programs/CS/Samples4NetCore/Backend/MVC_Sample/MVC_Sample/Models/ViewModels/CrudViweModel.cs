//**********************************************************************************
//* サンプル アプリ・モデル
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：CrudViweModel
//* クラス日本語名  ：サンプル アプリ・モデル
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Sample.Models.ViewModels
{
    /// <summary>
    /// サンプル アプリ・モデル
    /// </summary>
    [Serializable]
    public class CrudViweModel : BaseViewModel
    {
        /// <summary>shipper</summary>
        public ShipperViweModel Shipper { get; set; }

        /// <summary>shippers</summary>
        public List<ShipperViweModel> Shippers { get; set; }
        
        /// <summary>メッセージ</summary>
        public string Message { get; set; }
        
        #region ドロップダウンリストに表示するアイテム

        /// <summary>データアクセス制御クラス（データプロバイダ）</summary>
        public string DdlDap { get; set; }
        
        /// <summary>個別、共通、自動生成のＤａｏ種別</summary>
        public string DdlMode1 { get; set; }
        
        /// <summary>静的、動的のクエリ モード</summary>
        public string DdlMode2 { get; set; }
        
        /// <summary>分離レベル</summary>
        public string DdlIso { get; set; }

        /// <summary>コミット、ロールバック</summary>
        public string DdlExRollback { get; set; }

        /// <summary>コミット、ロールバック</summary>
        public string DdlOrderColumn { get; set; }

        /// <summary>コミット、ロールバック</summary>
        public string DdlOrderSequence { get; set; }
        
        /// <summary>データアクセス制御クラス（データプロバイダ） アイテムリスト</summary>
        public List<SelectListItem> DdlDapItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "SQL Server / SQL Client", Value = "SQL", Selected = true }, 
                    new SelectListItem() { Text = "Multi-DB / OLEDB.NET", Value = "OLE" }, 
                    new SelectListItem() { Text = "Multi-DB / ODBC.NET", Value = "ODB" }, 
                    new SelectListItem() { Text = "Oracle / ODP.NET", Value = "ODP" }, 
                    new SelectListItem() { Text = "DB2 / DB2.NET", Value = "DB2" }, 
                    new SelectListItem() { Text = "HiRDB / HiRDB-DP", Value = "HIR" }, 
                    new SelectListItem() { Text = "MySQL Cnn/NET", Value = "MCN" }, 
                    new SelectListItem() { Text = "PostgreSQL / Npgsql", Value = "NPS" }
                };
            }
        }

        /// <summary>個別、共通、自動生成のＤａｏ種別 アイテムリスト</summary>
        public List<SelectListItem> DdlMode1Items
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "個別Ｄａｏ", Value = "individual", Selected = true }, 
                    new SelectListItem() { Text = "共通Ｄａｏ", Value = "common" }, 
                    new SelectListItem() { Text = "自動生成Ｄａｏ（更新のみ）", Value = "generate" }
                };
            }
        }

        /// <summary>静的、動的のクエリ モード アイテムリスト</summary>
        public List<SelectListItem> DdlMode2Items
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "静的クエリ", Value = "static", Selected = true }, 
                    new SelectListItem() { Text = "動的クエリ", Value = "dynamic" }
                };
            }
        }

        /// <summary>分離レベル アイテムリスト</summary>
        public List<SelectListItem> DdlIsoItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "ノットコネクト", Value = "NC" }, 
                    new SelectListItem() { Text = "ノートランザクション", Value = "NT", Selected = true }, 
                    new SelectListItem() { Text = "ダーティリード", Value = "RU" }, 
                    new SelectListItem() { Text = "リードコミット", Value = "RC" }, 
                    new SelectListItem() { Text = "リピータブルリード", Value = "RR" }, 
                    new SelectListItem() { Text = "シリアライザブル", Value = "SZ" }, 
                    new SelectListItem() { Text = "スナップショット", Value = "SS" }, 
                    new SelectListItem() { Text = "デフォルト", Value = "DF" }
                };
            }
        }

        /// <summary>コミット、ロールバック アイテムリスト</summary>
        public List<SelectListItem> DdlExRollbackItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "正常時", Value = "-", Selected = true }, 
                    new SelectListItem() { Text = "業務例外", Value = "Business" }, 
                    new SelectListItem() { Text = "システム例外", Value = "System" }, 
                    new SelectListItem() { Text = "その他、一般的な例外", Value = "Other" }, 
                    new SelectListItem() { Text = "業務例外への振替", Value = "Other-Business" }, 
                    new SelectListItem() { Text = "システム例外への振替", Value = "Other-System" }
                };
            }
        }

        /// <summary>並び替え対象列 アイテムリスト</summary>
        public List<SelectListItem> DdlOrderColumnItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "c1", Value = "c1", Selected = true }, 
                    new SelectListItem() { Text = "c2", Value = "c2" }, 
                    new SelectListItem() { Text = "c3", Value = "c3" }
                };
            }
        }

        /// <summary>昇順・降順 アイテムリスト</summary>
        public List<SelectListItem> DdlOrderSequenceItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "ASC", Value = "A", Selected = true }, 
                    new SelectListItem() { Text = "DESC", Value = "D" }
                };
            }
        }

        #endregion
    }
}
