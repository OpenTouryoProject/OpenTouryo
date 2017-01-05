using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA_Sample.Models
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
        /// 荷主 ID
        /// </summary>
        public int ShipperId { get; set; }

        /// <summary>
        /// 会社名
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 電話番号
        /// </summary>
        public string Phone { get; set; }
    }
}