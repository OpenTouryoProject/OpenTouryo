//**********************************************************************************
//* サンプル アプリ・モデル
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：ShipperViweModel
//* クラス日本語名  ：サンプル アプリ・モデル
//*
//* 作成日時        ：2018/8/1
//* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System;

namespace MVC_Sample.Models.ViewModels
{
    /// <summary>自動生成Entityクラス</summary>
    [Serializable()]
    public class ShipperViweModel
    {
        #region メンバ変数

        /// <summary>メンバ変数：ShipperID</summary>
        private System.Int64? _PK_ShipperID; // Oracle対応 32 -> 64
        
        /// <summary>プロパティ：ShipperID</summary>
        public System.Int64? ShipperID  // Oracle対応 32 -> 64
        {
            get
            {
                return this._PK_ShipperID;
            }
            set
            {
                this._PK_ShipperID = value;
            }
        }

        /// <summary>メンバ変数：CompanyName</summary>
        private System.String _CompanyName;

        /// <summary>プロパティ：CompanyName</summary>
        public System.String CompanyName
        {
            get
            {
                return this._CompanyName;
            }
            set
            {
                this._CompanyName = value;
            }
        }
        /// <summary>メンバ変数：Phone</summary>
        private System.String _Phone;

        /// <summary>プロパティ：Phone</summary>
        public System.String Phone
        {
            get
            {
                return this._Phone;
            }
            set
            {
                this._Phone = value;
            }
        }

        #endregion
    }
}
