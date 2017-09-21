//**********************************************************************************
//* フレームワーク・テストクラス（引数・戻り値）
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：WebApiParams
//* クラス日本語名  ：WebApiParams
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

namespace WSServer_sample.Common
{
    public class WebApiParams
    {
        /// <summary>
        /// データアクセス プロバイダ
        /// </summary>
        public string ddlDap { get; set; }

        /// <summary>
        /// クエリモード
        /// </summary>
        public string ddlMode1 { get; set; }

        /// <summary>
        /// クエリモード
        /// </summary>
        public string ddlMode2 { get; set; }

        /// <summary>
        /// コミット、ロールバックの設定
        /// </summary>
        public string ddlExRollback { get; set; }

        /// <summary>
        /// 並び替え対象列
        /// </summary>
        public string OrderColumn { get; set; }

        /// <summary>
        /// 降順・昇順
        /// </summary>
        public string OrderSequence { get; set; }

        /// <summary>
        /// 荷主
        /// </summary>
        public Shipper Shipper { get; set; }
    }
}