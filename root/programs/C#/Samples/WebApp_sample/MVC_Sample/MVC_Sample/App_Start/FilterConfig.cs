﻿//**********************************************************************************
//* クラス名        ：FilterConfig
//* クラス日本語名  ：グローバルフィルタに関する指定
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web;
using System.Web.Mvc;

namespace MVC_Sample
{
    /// <summary>
    /// グローバルフィルタに関する指定
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// ASP.NET MVC 3 の新機能、グローバルフィルタは地味だけどイケてる - しばやん雑記
        /// http://shiba-yan.hatenablog.jp/entry/20110104/1294073715
        /// 　ASP.NET MVC 3 ではグローバルフィルタという機能が追加されました。
        /// 　Razor や DI に比べてかなり地味ですが、今までコントローラクラスに毎回付ける必要があった
        /// 　アクションフィルタを Global.asax で一括指定できるようになりました。
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // デフォルトで HandleError アクションフィルタを全てのコントローラへ適用するようになっている。
            filters.Add(new HandleErrorAttribute());

            //// OutputCache アクションフィルタ
            //// 全てのページを 60 秒間キャッシュする
            //filters.Add(new OutputCacheAttribute { Duration = 60 });
        }
    }
}