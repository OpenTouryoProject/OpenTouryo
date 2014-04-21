//**********************************************************************************
//* サンプル アプリ・モデル
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：CrudModel
//* クラス日本語名  ：サンプル アプリ・モデル
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

//System
using System;
using System.Web;
using System.Web.Mvc;

using System.Collections.Generic;

// DataSet をインポート
using MVC_Sample.DataSets;

namespace MVC_Sample.Models
{
    /// <summary>
    /// サンプル アプリ・モデル
    /// </summary>
    public class CrudModel
    {
        /// <summary>shippersテーブル</summary>
        public DsNorthwind.ShippersDataTable shippers { get; set; }
        
        /// <summary>メッセージ</summary>
        public string Message { get; set; }

        #region ドロップダウンリストに表示するアイテム

        /// <summary>
        /// ddlDap に表示するアイテムリスト
        /// </summary>
        public List<SelectListItem> DdlDapItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "SQL Server / SQL Client", Value = "SQL", Selected = true }, 
                    new SelectListItem() { Text = "Multi-DB / OLEDB.NET", Value = "OLE" }, 
                    new SelectListItem() { Text = "Multi-DB / ODBC.NET", Value = "ODB" }, 
                    new SelectListItem() { Text = "Oracle / ODP.NET", Value = "SQL" }, 
                    new SelectListItem() { Text = "DB2 / DB2.NET", Value = "DB2" }, 
                    new SelectListItem() { Text = "HiRDB / HiRDB-DP", Value = "HIR" }, 
                    new SelectListItem() { Text = "MySQL Cnn/NET", Value = "MCN" }, 
                    new SelectListItem() { Text = "PostgreSQL / Npgsql", Value = "NPS" }
                };
            }
        }

        /// <summary>
        /// ddlMode1 に表示するアイテムリスト
        /// </summary>
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

        /// <summary>
        /// ddlMode2 に表示するアイテムリスト
        /// </summary>
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

        /// <summary>
        /// ddlIso に表示するアイテムリスト
        /// </summary>
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

        /// <summary>
        /// ddlExRollback に表示するアイテムリスト
        /// </summary>
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

        /// <summary>
        /// ddlTransmission に表示するアイテムリスト
        /// </summary>
        public List<SelectListItem> DdlTransmissionItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Webサービス呼出", Value = "testWebService", Selected = true }, 
                    new SelectListItem() { Text = "インプロセス呼出", Value = "testInProcess" }
                };
            }
        }

        /// <summary>
        /// ddlOrderColumn に表示するアイテムリスト
        /// </summary>
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

        /// <summary>
        /// ddlOrderSequence に表示するアイテムリスト
        /// </summary>
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

        #region HTML.BeginFormで値を復元用途
        
        /// <summary>HTML.BeginFormで値を復元するためのワーク領域</summary>
        public Dictionary<string, string> InputValues { get; set; }

        /// <summary>HTML.BeginFormDe値を復元するためのワーク領域の初期化</summary>
        /// <param name="form">入力フォームの情報</param>
        public void CopyInputValues(FormCollection form)
        {
            InputValues = new Dictionary<string, string>();

            foreach (string key in form.AllKeys)
            {
                InputValues.Add(key, form[key]);
            }
        }

        #endregion
    }
}
